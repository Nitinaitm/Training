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
        private bool AttendanceRequired;
        private bool PreRequired;
        private bool PostRequired;
        private bool PreSkipped;
        private bool PostSkipped;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null)
            {
                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string empID = Convert.ToString(Session["EmpID"]).Trim().ToUpperInvariant();
            string sessionID = Convert.ToString(Session["SessionID"]).Trim();
            string trainingID = Convert.ToString(Session["TrainingID"]).Trim();

            if (string.IsNullOrWhiteSpace(sessionID))
            {
                Response.Redirect("MyTrainings.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (string.IsNullOrWhiteSpace(trainingID))
            {
                object value = objDB.ExecuteScalar("SELECT TrainingID FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });
                trainingID = value == null || value == DBNull.Value ? "" : Convert.ToString(value).Trim();

                if (!string.IsNullOrWhiteSpace(trainingID))
                {
                    Session["TrainingID"] = trainingID;
                }
            }

            if (string.IsNullOrWhiteSpace(trainingID))
            {
                Response.Redirect("MyTrainings.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Session["EmpID"] = empID;
            Session["SessionID"] = sessionID;
            Session["TrainingID"] = trainingID;

            if (!IsPostBack)
            {
                LoadSessionDetails();

                if (Response.IsRequestBeingRedirected)
                {
                    return;
                }

                LoadRequirements();

                if (Response.IsRequestBeingRedirected)
                {
                    return;
                }

                SessionSummary1.LoadSession(trainingID, sessionID, empID);
                LoadTestStatus();
            }
        }

        private void LoadRequirements()
        {
            string trainingID = Convert.ToString(Session["TrainingID"]).Trim();
            string sessionID = Convert.ToString(Session["SessionID"]).Trim();

            if (string.IsNullOrWhiteSpace(trainingID) || string.IsNullOrWhiteSpace(sessionID))
            {
                Response.Redirect("MyTrainings.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            DataTable dt = objDB.GetDataTable("SELECT TD.AttendanceRequired,TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,ISNULL(SM.PreAssessmentSkipped,0) AS PreSkipped,ISNULL(SM.PostAssessmentSkipped,0) AS PostSkipped FROM TrainingDetails TD INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE TD.TrainingID=@TrainingID AND SM.SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID) });

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("TrainingDetails.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            AttendanceRequired = dt.Rows[0]["AttendanceRequired"] != DBNull.Value && Convert.ToBoolean(dt.Rows[0]["AttendanceRequired"]);
            PreRequired = dt.Rows[0]["InitialAssessmentRequired"] != DBNull.Value && Convert.ToBoolean(dt.Rows[0]["InitialAssessmentRequired"]);
            PostRequired = dt.Rows[0]["FinalAssessmentRequired"] != DBNull.Value && Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);
            PreSkipped = dt.Rows[0]["PreSkipped"] != DBNull.Value && Convert.ToBoolean(dt.Rows[0]["PreSkipped"]);
            PostSkipped = dt.Rows[0]["PostSkipped"] != DBNull.Value && Convert.ToBoolean(dt.Rows[0]["PostSkipped"]);

            btnPreTest.Visible = PreRequired;
            btnPostTest.Visible = PostRequired;

            if (lblPreStatus.Parent != null)
            {
                lblPreStatus.Parent.Visible = PreRequired;
            }

            if (lblPostStatus.Parent != null)
            {
                lblPostStatus.Parent.Visible = PostRequired;
            }
        }

        private bool SessionAttendanceDone()
        {
            string sessionID = Convert.ToString(Session["SessionID"]).Trim();
            string empID = Convert.ToString(Session["EmpID"]).Trim().ToUpperInvariant();

            object value = objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM SessionAttendance WHERE SessionID=@SessionID AND EmpID=@EmpID AND AttendanceStatus IN ('Present','Completed')) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@EmpID", empID) });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private void LoadSessionDetails()
        {
            string trainingID = Convert.ToString(Session["TrainingID"]).Trim();
            string sessionID = Convert.ToString(Session["SessionID"]).Trim();

            string sql = "SELECT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,SM.SessionID,SM.SessionNo,SM.SessionName,TM.TopicName,CASE WHEN TR.TrainerType='Internal' THEN EB.EmpName ELSE TR.NameExternal END AS TrainerName,TRY_CONVERT(date,SM.SessionDate,105) AS SessionDate,SM.StartTime,SM.EndTime,SM.TotalHours FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID LEFT JOIN CourseMaster CM ON CM.CourseID=TD.CourseID LEFT JOIN TopicMaster TM ON TM.TopicID=SM.TopicID LEFT JOIN TrainerMaster TR ON TR.TrainerID=SM.TrainerID LEFT JOIN EmpBasicMaster EB ON EB.EmpID=TR.EmpID WHERE SM.TrainingID=@TrainingID AND SM.SessionID=@SessionID";

            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@SessionID", sessionID) });

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("TrainingDetails.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            DataRow row = dt.Rows[0];

            lblTrainingID.Text = Convert.ToString(row["TrainingID"]);
            lblCourse.Text = Convert.ToString(row["CourseName"]);
            lblTrainingType.Text = Convert.ToString(row["TrainingType"]);
            lblOrganizer.Text = Convert.ToString(row["TrainingOrganizer"]);
            lblSessionNo.Text = Convert.ToString(row["SessionNo"]);
            lblSessionName.Text = Convert.ToString(row["SessionName"]);
            lblTopic.Text = Convert.ToString(row["TopicName"]);
            lblTrainer.Text = Convert.ToString(row["TrainerName"]);

            DateTime sessionDate;

            if (DateTime.TryParse(Convert.ToString(row["SessionDate"]), out sessionDate))
            {
                lblSessionDate.Text = sessionDate.ToString("dd-MMM-yyyy");
            }
            else
            {
                lblSessionDate.Text = "-";
            }

            lblStartTime.Text = Convert.ToString(row["StartTime"]);
            lblEndTime.Text = Convert.ToString(row["EndTime"]);

            if (row["TotalHours"] == DBNull.Value || string.IsNullOrWhiteSpace(Convert.ToString(row["TotalHours"])))
            {
                lblDuration.Text = "-";
            }
            else
            {
                lblDuration.Text = Convert.ToString(row["TotalHours"]) + " Hours";
            }
        }

        private void LoadTestStatus()
        {
            if (!PreRequired)
            {
                SetPreNotRequired();
            }
            else if (PreSkipped)
            {
                SetPreSkipped();
            }
            else
            {
                LoadOneTest("Pre");
            }

            if (!PostRequired)
            {
                SetPostNotRequired();
            }
            else if (PostSkipped)
            {
                SetPostSkipped();
            }
            else
            {
                LoadOneTest("Post");
            }
        }

        private void LoadOneTest(string type)
        {
            string sessionID = Convert.ToString(Session["SessionID"]).Trim();
            string empID = Convert.ToString(Session["EmpID"]).Trim().ToUpperInvariant();
            bool isPre = string.Equals(type, "Pre", StringComparison.OrdinalIgnoreCase);

            DataTable dt = objDB.GetDataTable("SELECT TOP 1 TestID,IsPublished FROM TestMaster WHERE SessionID=@SessionID AND TestType=@Type ORDER BY TestID DESC", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@Type", type) });

            if (dt.Rows.Count == 0 || dt.Rows[0]["IsPublished"] == DBNull.Value || !Convert.ToBoolean(dt.Rows[0]["IsPublished"]))
            {
                if (isPre)
                {
                    SetPreNotPublished();
                }
                else
                {
                    SetPostNotPublished();
                }

                return;
            }

            string testID = Convert.ToString(dt.Rows[0]["TestID"]).Trim();

            DataTable attemptDT = objDB.GetDataTable("SELECT SUM(CASE WHEN Submitted=0 THEN 1 ELSE 0 END) AS RunningAttempt,SUM(CASE WHEN Submitted=1 THEN 1 ELSE 0 END) AS SubmittedAttempt FROM TestAttempt WHERE TestID=@TestID AND EmpID=@EmpID", new SqlParameter[] { new SqlParameter("@TestID", testID), new SqlParameter("@EmpID", empID) });

            int running = 0;
            int submitted = 0;

            if (attemptDT.Rows.Count > 0)
            {
                if (attemptDT.Rows[0]["RunningAttempt"] != DBNull.Value)
                {
                    running = Convert.ToInt32(attemptDT.Rows[0]["RunningAttempt"]);
                }

                if (attemptDT.Rows[0]["SubmittedAttempt"] != DBNull.Value)
                {
                    submitted = Convert.ToInt32(attemptDT.Rows[0]["SubmittedAttempt"]);
                }
            }

            Label status = isPre ? lblPreStatus : lblPostStatus;
            Button button = isPre ? btnPreTest : btnPostTest;

            if (AttendanceRequired && !SessionAttendanceDone())
            {
                status.Text = "Waiting for Attendance";
                status.CssClass = "badge badge-warning status-badge";
                button.Text = "Start " + type + " Test";
                button.Enabled = false;
                button.CommandArgument = "";
                return;
            }

            if (!isPre && PreRequired && !PreSkipped)
            {
                object prePublished = objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM TestMaster WHERE SessionID=@SessionID AND TestType='Pre' AND IsPublished=1) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });

                if (prePublished != null && prePublished != DBNull.Value && Convert.ToInt32(prePublished) == 1 && !IsPreCompleted())
                {
                    status.Text = "Waiting for Pre Test";
                    status.CssClass = "badge badge-warning status-badge";
                    button.Text = "Start Post Test";
                    button.Enabled = false;
                    button.CommandArgument = "";
                    return;
                }
            }

            if (running > 0)
            {
                status.Text = "In Progress";
                status.CssClass = "badge badge-warning status-badge";
                button.Text = "Resume " + type + " Test";
                button.Enabled = true;
                button.CommandArgument = "Resume";
                return;
            }

            if (submitted > 0)
            {
                status.Text = "Completed";
                status.CssClass = "badge badge-success status-badge";
                button.Text = "View Result";
                button.Enabled = true;
                button.CommandArgument = "Result";
                return;
            }

            status.Text = "Available";
            status.CssClass = "badge badge-primary status-badge";
            button.Text = "Start " + type + " Test";
            button.Enabled = true;
            button.CommandArgument = "Start";
        }

        private bool IsPreCompleted()
        {
            string sessionID = Convert.ToString(Session["SessionID"]).Trim();
            string empID = Convert.ToString(Session["EmpID"]).Trim().ToUpperInvariant();

            object value = objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@EmpID", empID) });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private void SetPreNotRequired()
        {
            lblPreStatus.Text = "Not Required";
            lblPreStatus.CssClass = "badge badge-secondary status-badge";
            btnPreTest.Visible = false;
        }

        private void SetPostNotRequired()
        {
            lblPostStatus.Text = "Not Required";
            lblPostStatus.CssClass = "badge badge-secondary status-badge";
            btnPostTest.Visible = false;
        }

        private void SetPreSkipped()
        {
            lblPreStatus.Text = "Skipped";
            lblPreStatus.CssClass = "badge badge-secondary status-badge";
            btnPreTest.Text = "Pre Test Skipped";
            btnPreTest.Enabled = false;
            btnPreTest.CommandArgument = "";
        }

        private void SetPostSkipped()
        {
            lblPostStatus.Text = "Skipped";
            lblPostStatus.CssClass = "badge badge-secondary status-badge";
            btnPostTest.Text = "Post Test Skipped";
            btnPostTest.Enabled = false;
            btnPostTest.CommandArgument = "";
        }

        private void SetPreNotPublished()
        {
            lblPreStatus.Text = "Not Published";
            lblPreStatus.CssClass = "badge badge-secondary status-badge";
            btnPreTest.Text = "Pre Test Not Available";
            btnPreTest.Enabled = false;
            btnPreTest.CommandArgument = "";
        }

        private void SetPostNotPublished()
        {
            lblPostStatus.Text = "Not Published";
            lblPostStatus.CssClass = "badge badge-secondary status-badge";
            btnPostTest.Text = "Post Test Not Available";
            btnPostTest.Enabled = false;
            btnPostTest.CommandArgument = "";
        }

        protected void btnPreTest_Click(object sender, EventArgs e)
        {
            if (btnPreTest.CommandArgument == "Result")
            {
                Session["ResultTestType"] = "Pre";
                Response.Redirect("MyExamResult.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            LoadRequirements();

            if (Response.IsRequestBeingRedirected)
            {
                return;
            }

            if (!PreRequired || PreSkipped || (AttendanceRequired && !SessionAttendanceDone()))
            {
                return;
            }

            Response.Redirect("PreTrainingExam.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnPostTest_Click(object sender, EventArgs e)
        {
            if (btnPostTest.CommandArgument == "Result")
            {
                Session["ResultTestType"] = "Post";
                Response.Redirect("MyExamResult.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            LoadRequirements();

            if (Response.IsRequestBeingRedirected)
            {
                return;
            }

            if (!PostRequired || PostSkipped || (AttendanceRequired && !SessionAttendanceDone()))
            {
                return;
            }

            if (PreRequired && !PreSkipped)
            {
                string sessionID = Convert.ToString(Session["SessionID"]).Trim();
                object prePublished = objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM TestMaster WHERE SessionID=@SessionID AND TestType='Pre' AND IsPublished=1) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });

                if (prePublished != null && prePublished != DBNull.Value && Convert.ToInt32(prePublished) == 1 && !IsPreCompleted())
                {
                    return;
                }
            }

            Response.Redirect("PostTrainingExam.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("TrainingDetails.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnExam_Click(object sender, EventArgs e)
        {
            Response.Redirect("MyExamResult.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
