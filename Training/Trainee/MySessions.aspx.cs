using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MySessions : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        private bool AttendanceRequired, PreRequired, PostRequired, PreSkipped, PostSkipped;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (Session["TrainingID"] == null || Session["SessionID"] == null) { Response.Redirect("MyTrainings.aspx"); return; }
            if (!IsPostBack)
            {
                string trainingID = Session["TrainingID"].ToString(), sessionID = Session["SessionID"].ToString(), empID = Session["EmpID"].ToString().ToUpperInvariant();
                SessionSummary1.LoadSession(trainingID, sessionID, empID);
                LoadSessionDetails(); LoadRequirements(); LoadTestStatus();
            }
        }

        private void LoadRequirements()
        {
            DataTable dt = objDB.GetDataTable("SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,ISNULL((SELECT PreAssessmentSkipped FROM SessionMaster WHERE SessionID=@SessionID),0) PreSkipped,ISNULL((SELECT PostAssessmentSkipped FROM SessionMaster WHERE SessionID=@SessionID),0) PostSkipped FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()), new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            if (dt.Rows.Count == 0) { Response.Redirect("MyTrainings.aspx"); return; }
            AttendanceRequired = Convert.ToBoolean(dt.Rows[0]["AttendanceRequired"]);
            PreRequired = Convert.ToBoolean(dt.Rows[0]["InitialAssessmentRequired"]); PostRequired = Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);
            PreSkipped = Convert.ToBoolean(dt.Rows[0]["PreSkipped"]); PostSkipped = Convert.ToBoolean(dt.Rows[0]["PostSkipped"]);
            btnPreTest.Visible = PreRequired; btnPostTest.Visible = PostRequired;
            lblPreStatus.Parent.Visible = PreRequired; lblPostStatus.Parent.Visible = PostRequired;
        }

        private bool SessionAttendanceDone()
        {
            object v = objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM SessionAttendance WHERE SessionID=@SessionID AND EmpID=@EmpID AND AttendanceStatus IN ('Present','Completed')) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@SessionID", Session["SessionID"].ToString()), new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()) });
            return v != null && Convert.ToInt32(v) == 1;
        }

        private void LoadSessionDetails()
        {
            string sql = "SELECT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,SM.SessionID,SM.SessionNo,SM.SessionName,TM.TopicName,CASE WHEN TR.TrainerType='Internal' THEN EB.EmpName ELSE TR.NameExternal END TrainerName,TRY_CONVERT(date,SM.SessionDate,105) SessionDate,SM.StartTime,SM.EndTime,SM.TotalHours FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID LEFT JOIN TopicMaster TM ON TM.TopicID=SM.TopicID LEFT JOIN TrainerMaster TR ON TR.TrainerID=SM.TrainerID LEFT JOIN EmpBasicMaster EB ON EB.EmpID=TR.EmpID WHERE SM.TrainingID=@TrainingID AND SM.SessionID=@SessionID";
            DataTable dt=objDB.GetDataTable(sql,new SqlParameter[]{new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),new SqlParameter("@SessionID",Session["SessionID"].ToString())});
            if(dt.Rows.Count==0){Response.Redirect("TrainingDetails.aspx");return;} DataRow d=dt.Rows[0];
            lblTrainingID.Text=d["TrainingID"].ToString();lblCourse.Text=d["CourseName"].ToString();lblTrainingType.Text=d["TrainingType"].ToString();lblOrganizer.Text=d["TrainingOrganizer"].ToString();lblSessionNo.Text=d["SessionNo"].ToString();lblSessionName.Text=d["SessionName"].ToString();lblTopic.Text=d["TopicName"].ToString();lblTrainer.Text=d["TrainerName"].ToString();lblSessionDate.Text=Convert.ToDateTime(d["SessionDate"]).ToString("dd-MMM-yyyy");lblStartTime.Text=d["StartTime"].ToString();lblEndTime.Text=d["EndTime"].ToString();lblDuration.Text=d["TotalHours"].ToString()+" Hours";
        }

        private void LoadTestStatus()
        {
            if(!PreRequired){SetPreNotRequired();} else if(PreSkipped){SetPreSkipped();} else LoadOneTest("Pre");
            if(!PostRequired){SetPostNotRequired();} else if(PostSkipped){SetPostSkipped();} else LoadOneTest("Post");
        }

        private void LoadOneTest(string type)
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP 1 TestID,IsPublished FROM TestMaster WHERE SessionID=@SessionID AND TestType=@Type ORDER BY TestID DESC",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@Type",type)});
            bool pre=type=="Pre"; if(dt.Rows.Count==0||!Convert.ToBoolean(dt.Rows[0]["IsPublished"])){if(pre)SetPreNotPublished();else SetPostNotPublished();return;}
            string testID=dt.Rows[0]["TestID"].ToString(); DataTable a=objDB.GetDataTable("SELECT SUM(CASE WHEN Submitted=0 THEN 1 ELSE 0 END) RunningAttempt,SUM(CASE WHEN Submitted=1 THEN 1 ELSE 0 END) SubmittedAttempt FROM TestAttempt WHERE TestID=@TestID AND EmpID=@EmpID",new SqlParameter[]{new SqlParameter("@TestID",testID),new SqlParameter("@EmpID",Session["EmpID"].ToString().ToUpperInvariant())});
            int running=0,submitted=0;if(a.Rows.Count>0){if(a.Rows[0]["RunningAttempt"]!=DBNull.Value)running=Convert.ToInt32(a.Rows[0]["RunningAttempt"]);if(a.Rows[0]["SubmittedAttempt"]!=DBNull.Value)submitted=Convert.ToInt32(a.Rows[0]["SubmittedAttempt"]);}
            Label status=pre?lblPreStatus:lblPostStatus;Button button=pre?btnPreTest:btnPostTest;
            if(AttendanceRequired&&!SessionAttendanceDone()){status.Text="Waiting for Attendance";status.CssClass="badge badge-warning status-badge";button.Text="Start "+type+" Test";button.Enabled=false;button.CommandArgument="";return;}
            if(!pre && PreRequired && !PreSkipped && !IsPreCompleted()){status.Text="Waiting for Pre Test";status.CssClass="badge badge-warning status-badge";button.Text="Start Post Test";button.Enabled=false;button.CommandArgument="";return;}
            if(running>0){status.Text="In Progress";status.CssClass="badge badge-warning status-badge";button.Text="Resume "+type+" Test";button.Enabled=true;button.CommandArgument="Resume";return;}
            if(submitted>0){status.Text="Completed";status.CssClass="badge badge-success status-badge";button.Text="View Result";button.Enabled=true;button.CommandArgument="Result";return;}
            status.Text="Available";status.CssClass="badge badge-primary status-badge";button.Text="Start "+type+" Test";button.Enabled=true;button.CommandArgument="Start";
        }
        private bool IsPreCompleted(){object v=objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 1 ELSE 0 END",new SqlParameter[]{new SqlParameter("@SessionID",Session["SessionID"].ToString()),new SqlParameter("@EmpID",Session["EmpID"].ToString().ToUpperInvariant())});return v!=null&&Convert.ToInt32(v)==1;}
        private void SetPreNotRequired(){lblPreStatus.Text="Not Required";lblPreStatus.CssClass="badge badge-secondary status-badge";btnPreTest.Visible=false;}
        private void SetPostNotRequired(){lblPostStatus.Text="Not Required";lblPostStatus.CssClass="badge badge-secondary status-badge";btnPostTest.Visible=false;}
        private void SetPreSkipped(){lblPreStatus.Text="Skipped";lblPreStatus.CssClass="badge badge-secondary status-badge";btnPreTest.Text="Pre Test Skipped";btnPreTest.Enabled=false;btnPreTest.CommandArgument="";}
        private void SetPostSkipped(){lblPostStatus.Text="Skipped";lblPostStatus.CssClass="badge badge-secondary status-badge";btnPostTest.Text="Post Test Skipped";btnPostTest.Enabled=false;btnPostTest.CommandArgument="";}
        private void SetPreNotPublished(){lblPreStatus.Text="Not Published";lblPreStatus.CssClass="badge badge-secondary status-badge";btnPreTest.Text="Pre Test Not Available";btnPreTest.Enabled=false;btnPreTest.CommandArgument="";}
        private void SetPostNotPublished(){lblPostStatus.Text="Not Published";lblPostStatus.CssClass="badge badge-secondary status-badge";btnPostTest.Text="Post Test Not Available";btnPostTest.Enabled=false;btnPostTest.CommandArgument="";}
        protected void btnPreTest_Click(object sender,EventArgs e){if(btnPreTest.CommandArgument=="Result"){Session["ResultTestType"]="Pre";Response.Redirect("MyExamResult.aspx");return;}LoadRequirements();if(!PreRequired||PreSkipped)return;Response.Redirect("PreTrainingExam.aspx");}
        protected void btnPostTest_Click(object sender,EventArgs e){if(btnPostTest.CommandArgument=="Result"){Session["ResultTestType"]="Post";Response.Redirect("MyExamResult.aspx");return;}LoadRequirements();if(!PostRequired||PostSkipped)return;Response.Redirect("PostTrainingExam.aspx");}
        protected void btnBack_Click(object sender,EventArgs e){Response.Redirect("TrainingDetails.aspx");}
        protected void btnExam_Click(object sender,EventArgs e){Response.Redirect("MyExamResult.aspx");}
    }
}
