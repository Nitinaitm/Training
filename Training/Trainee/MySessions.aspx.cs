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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (Session["TrainingID"] == null || Session["SessionID"] == null) { Response.Redirect("MyTrainings.aspx"); return; }
            if (!IsPostBack)
            {
                string trainingID = Session["TrainingID"].ToString();
                string sessionID = Session["SessionID"].ToString();
                string empID = Session["EmpID"].ToString().ToUpperInvariant();
                SessionSummary1.LoadSession(trainingID, sessionID, empID);
                LoadSessionDetails();
                LoadRequirements();
                LoadTestStatus();
            }
        }

        private bool AttendanceRequired;
        private bool PreRequired;
        private bool PostRequired;

        private void LoadRequirements()
        {
            DataTable dt = objDB.GetDataTable(
                "SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) });
            if (dt.Rows.Count == 0) { Response.Redirect("MyTrainings.aspx"); return; }
            AttendanceRequired = Convert.ToBoolean(dt.Rows[0]["AttendanceRequired"]);
            PreRequired = Convert.ToBoolean(dt.Rows[0]["InitialAssessmentRequired"]);
            PostRequired = Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);
            btnPreTest.Visible = PreRequired;
            lblPreStatus.Parent.Visible = PreRequired;
            btnPostTest.Visible = PostRequired;
            lblPostStatus.Parent.Visible = PostRequired;
        }

        private bool SessionAttendanceDone()
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM SessionAttendance WHERE SessionID=@SessionID AND EmpID=@EmpID AND AttendanceStatus='Completed') THEN 1 ELSE 0 END",
                new SqlParameter[] {
                    new SqlParameter("@SessionID", Session["SessionID"].ToString()),
                    new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant())
                });
            return value != null && Convert.ToInt32(value) == 1;
        }

        private void LoadSessionDetails()
        {
            string sql = "SELECT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,SM.SessionID,SM.SessionNo,SM.SessionName,TM.TopicName,CASE WHEN TR.TrainerType='Internal' THEN EB.EmpName ELSE TR.NameExternal END AS TrainerName,TRY_CONVERT(date,SM.SessionDate,105) AS SessionDate,SM.StartTime,SM.EndTime,SM.TotalHours FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID LEFT JOIN TopicMaster TM ON TM.TopicID=SM.TopicID LEFT JOIN TrainerMaster TR ON TR.TrainerID=SM.TrainerID LEFT JOIN EmpBasicMaster EB ON EB.EmpID=TR.EmpID WHERE SM.TrainingID=@TrainingID AND SM.SessionID=@SessionID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()), new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            if (dt.Rows.Count == 0) { Response.Redirect("TrainingDetails.aspx"); return; }
            DataRow dr = dt.Rows[0];
            lblTrainingID.Text = dr["TrainingID"].ToString(); lblCourse.Text = dr["CourseName"].ToString(); lblTrainingType.Text = dr["TrainingType"].ToString(); lblOrganizer.Text = dr["TrainingOrganizer"].ToString();
            lblSessionNo.Text = dr["SessionNo"].ToString(); lblSessionName.Text = dr["SessionName"].ToString(); lblTopic.Text = dr["TopicName"].ToString(); lblTrainer.Text = dr["TrainerName"].ToString();
            lblSessionDate.Text = Convert.ToDateTime(dr["SessionDate"]).ToString("dd-MMM-yyyy"); lblStartTime.Text = dr["StartTime"].ToString(); lblEndTime.Text = dr["EndTime"].ToString(); lblDuration.Text = dr["TotalHours"].ToString()+" Hours";
        }

        private void LoadTestStatus()
        {
            if (!PreRequired) SetPreNotRequired();
            if (!PostRequired) SetPostNotRequired();
            string sql = "SELECT MAX(CASE WHEN TM.TestType='Pre' THEN TM.TestID END) AS PreTestID,MAX(CASE WHEN TM.TestType='Pre' THEN TM.IsPublished END) AS PrePublished,MAX(CASE WHEN TM.TestType='Post' THEN TM.TestID END) AS PostTestID,MAX(CASE WHEN TM.TestType='Post' THEN TM.IsPublished END) AS PostPublished FROM TestMaster TM WHERE TM.SessionID=@SessionID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            if (dt.Rows.Count == 0) { if (PreRequired) SetPreNotPublished(); if (PostRequired) SetPostNotPublished(); return; }
            DataRow dr=dt.Rows[0];
            if (PreRequired) LoadPreStatus(dr["PreTestID"].ToString(),dr["PrePublished"].ToString());
            if (PostRequired) LoadPostStatus(dr["PostTestID"].ToString(),dr["PostPublished"].ToString());
        }

        private void LoadPreStatus(string testID,string published)
        {
            if (testID=="" || (published!="True" && published!="1")) { SetPreNotPublished(); return; }
            SetAttemptStatus(testID,true);
        }
        private void LoadPostStatus(string testID,string published)
        {
            if (testID=="" || (published!="True" && published!="1")) { SetPostNotPublished(); return; }
            SetAttemptStatus(testID,false);
        }
        private void SetAttemptStatus(string testID,bool pre)
        {
            DataTable dt=objDB.GetDataTable("SELECT SUM(CASE WHEN Submitted=0 THEN 1 ELSE 0 END) RunningAttempt,SUM(CASE WHEN Submitted=1 THEN 1 ELSE 0 END) SubmittedAttempt FROM TestAttempt WHERE TestID=@TestID AND EmpID=@EmpID",new SqlParameter[]{new SqlParameter("@TestID",testID),new SqlParameter("@EmpID",Session["EmpID"].ToString().ToUpperInvariant())});
            int running=0,submitted=0; if(dt.Rows.Count>0){if(dt.Rows[0]["RunningAttempt"]!=DBNull.Value)running=Convert.ToInt32(dt.Rows[0]["RunningAttempt"]);if(dt.Rows[0]["SubmittedAttempt"]!=DBNull.Value)submitted=Convert.ToInt32(dt.Rows[0]["SubmittedAttempt"]);}
            Label status=pre?lblPreStatus:lblPostStatus; Button button=pre?btnPreTest:btnPostTest; string type=pre?"Pre":"Post";
            if(running>0){status.Text="In Progress";status.CssClass="badge badge-warning status-badge";button.Text="Resume "+type+" Test";button.Enabled=true;button.CommandArgument="Resume";return;}
            if(submitted>0){status.Text="Completed";status.CssClass="badge badge-success status-badge";button.Text="View Result";button.Enabled=true;button.CommandArgument="Result";return;}
            status.Text="Available";status.CssClass="badge badge-primary status-badge";button.Text="Start "+type+" Test";button.Enabled=true;button.CommandArgument="Start";
        }
        private void SetPreNotRequired(){lblPreStatus.Text="Not Required";lblPreStatus.CssClass="badge badge-secondary status-badge";btnPreTest.Visible=false;}
        private void SetPostNotRequired(){lblPostStatus.Text="Not Required";lblPostStatus.CssClass="badge badge-secondary status-badge";btnPostTest.Visible=false;}
        private void SetPreNotPublished(){lblPreStatus.Text="Not Published";lblPreStatus.CssClass="badge badge-secondary status-badge";btnPreTest.Text="Pre Test Not Available";btnPreTest.Enabled=false;btnPreTest.CommandArgument="";}
        private void SetPostNotPublished(){lblPostStatus.Text="Not Published";lblPostStatus.CssClass="badge badge-secondary status-badge";btnPostTest.Text="Post Test Not Available";btnPostTest.Enabled=false;btnPostTest.CommandArgument="";}

        protected void btnPreTest_Click(object sender,EventArgs e)
        {
            if(!PreRequired){return;}
            if(AttendanceRequired && !SessionAttendanceDone()){ClientScript.RegisterStartupScript(GetType(),"msg","alert('Please complete attendance for this session first.');",true);return;}
            if(btnPreTest.CommandArgument=="Result"){Session["ResultTestType"]="Pre";Response.Redirect("MyExamResult.aspx");return;} Response.Redirect("PreTrainingExam.aspx");
        }
        protected void btnPostTest_Click(object sender,EventArgs e)
        {
            if(!PostRequired){return;}
            if(AttendanceRequired && !SessionAttendanceDone()){ClientScript.RegisterStartupScript(GetType(),"msg","alert('Please complete attendance for this session first.');",true);return;}
            if(btnPostTest.CommandArgument=="Result"){Session["ResultTestType"]="Post";Response.Redirect("MyExamResult.aspx");return;} Response.Redirect("PostTrainingExam.aspx");
        }
        protected void btnBack_Click(object sender,EventArgs e){Response.Redirect("TrainingDetails.aspx");}
        protected void btnExam_Click(object sender,EventArgs e){Response.Redirect("MyExamResult.aspx");}
    }
}