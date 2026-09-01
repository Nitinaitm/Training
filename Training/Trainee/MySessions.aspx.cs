using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Training.Trainee
{
    public partial class MySessions : System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
             if
            (
                  Session["EmpID"] == null

            )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                 Session["TrainingID"] == null
                ||
                Session["SessionID"] == null
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            if (!IsPostBack)
            {
                string trainingID =
         Session["TrainingID"].ToString();

                string sessionID =
                    Session["SessionID"].ToString();

                string empID =
                    Session["EmpID"].ToString().ToUpperInvariant();

                SessionSummary1.LoadSession(
                    trainingID,
                    sessionID,
                    empID);

                LoadSessionDetails();

                LoadTestStatus();
            }
        }

        private void LoadSessionDetails()
        {
            string sql =
            "SELECT " +
            "TD.TrainingID," +
            "CM.CourseName," +
            "TD.TrainingType," +
            "TD.TrainingOrganizer," +
            "SM.SessionID," +
            "SM.SessionNo," +
            "SM.SessionName," +
            "TM.TopicName," +
            "CASE " +
            "WHEN TR.TrainerType='Internal' THEN EB.EmpName " +
            "ELSE TR.NameExternal " +
            "END AS TrainerName," +
            "TRY_CONVERT(date,SM.SessionDate,105) AS SessionDate," +
            "SM.StartTime," +
            "SM.EndTime," +
            "SM.TotalHours " +
            "FROM SessionMaster SM " +
            "INNER JOIN TrainingDetails TD " +
            "ON TD.TrainingID=SM.TrainingID " +
            "INNER JOIN CourseMaster CM " +
            "ON CM.CourseID=TD.CourseID " +
            "LEFT JOIN TopicMaster TM " +
            "ON TM.TopicID=SM.TopicID " +
            "LEFT JOIN TrainerMaster TR " +
            "ON TR.TrainerID=SM.TrainerID " +
            "LEFT JOIN EmpBasicMaster EB " +
            "ON EB.EmpID=TR.EmpID " +
            "WHERE SM.TrainingID=@TrainingID " +
            "AND SM.SessionID=@SessionID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@TrainingID",
                Session["TrainingID"].ToString()),

            new SqlParameter(
                "@SessionID",
                Session["SessionID"].ToString())
        };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if (dt.Rows.Count == 0)
            {
                Response.Redirect(
                    "TrainingDetails.aspx");

                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblTrainingID.Text =
                dr["TrainingID"].ToString();

            lblCourse.Text =
                dr["CourseName"].ToString();

            lblTrainingType.Text =
                dr["TrainingType"].ToString();

            lblOrganizer.Text =
                dr["TrainingOrganizer"].ToString();

            lblSessionNo.Text =
                dr["SessionNo"].ToString();

            lblSessionName.Text =
                dr["SessionName"].ToString();

            lblTopic.Text =
                dr["TopicName"].ToString();

            lblTrainer.Text =
                dr["TrainerName"].ToString();

            lblSessionDate.Text =
                Convert.ToDateTime(
                    dr["SessionDate"])
                .ToString("dd-MMM-yyyy");

            lblStartTime.Text =
                dr["StartTime"].ToString();

            lblEndTime.Text =
                dr["EndTime"].ToString();

            lblDuration.Text =
                dr["TotalHours"].ToString()
                + " Hours";
        }

        private void LoadTestStatus()
        {

            string sql =
            "SELECT " +
            "MAX(CASE WHEN TM.TestType='Pre' THEN TM.TestID END) AS PreTestID," +
            "MAX(CASE WHEN TM.TestType='Pre' THEN TM.IsPublished END) AS PrePublished," +
            "MAX(CASE WHEN TM.TestType='Post' THEN TM.TestID END) AS PostTestID," +
            "MAX(CASE WHEN TM.TestType='Post' THEN TM.IsPublished END) AS PostPublished " +
            "FROM TestMaster TM " +
            "WHERE TM.SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@SessionID",
            Session["SessionID"].ToString())
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if (dt.Rows.Count == 0)
            {
                SetPreNotPublished();

                SetPostNotPublished();

                return;
            }

            DataRow dr =
                dt.Rows[0];

            LoadPreStatus(
                dr["PreTestID"].ToString(),
                dr["PrePublished"].ToString());

            LoadPostStatus(
                dr["PostTestID"].ToString(),
                dr["PostPublished"].ToString());
        }

        private void LoadPreStatus(
    string testID,
    string published)
        {
            if (testID == "")
            {
                SetPreNotPublished();

                return;
            }

            if
            (
                published != "True"
                &&
                published != "1"
            )
            {
                SetPreNotPublished();

                return;
            }

            string sql =
                "SELECT " +
                "SUM(CASE WHEN Submitted=0 THEN 1 ELSE 0 END) AS RunningAttempt," +
                "SUM(CASE WHEN Submitted=1 THEN 1 ELSE 0 END) AS SubmittedAttempt " +
                "FROM TestAttempt " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID),

        new SqlParameter(
            "@EmpID",
            Session["EmpID"].ToString().ToUpperInvariant())
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            int runningAttempt =
                0;

            int submittedAttempt =
                0;

            if
            (
                dt.Rows.Count > 0
            )
            {
                if
                (
                    dt.Rows[0]["RunningAttempt"]
                    !=
                    DBNull.Value
                )
                {
                    runningAttempt =
                        Convert.ToInt32(
                            dt.Rows[0]["RunningAttempt"]);
                }

                if
                (
                    dt.Rows[0]["SubmittedAttempt"]
                    !=
                    DBNull.Value
                )
                {
                    submittedAttempt =
                        Convert.ToInt32(
                            dt.Rows[0]["SubmittedAttempt"]);
                }
            }

            if
            (
                runningAttempt > 0
            )
            {
                lblPreStatus.Text =
                    "In Progress";

                lblPreStatus.CssClass =
                    "badge badge-warning status-badge";

                btnPreTest.Text =
                    "Resume Pre Test";

                btnPreTest.Enabled =
                    true;

                btnPreTest.CommandArgument =
                    "Resume";

                return;
            }

            if
            (
                submittedAttempt > 0
            )
            {
                lblPreStatus.Text =
                    "Completed";

                lblPreStatus.CssClass =
                    "badge badge-success status-badge";

                btnPreTest.Text =
                    "View Result";

                btnPreTest.Enabled =
                    true;

                btnPreTest.CommandArgument =
                    "Result";

                return;
            }

            lblPreStatus.Text =
                "Available";

            lblPreStatus.CssClass =
                "badge badge-primary status-badge";

            btnPreTest.Text =
                "Start Pre Test";

            btnPreTest.Enabled =
                true;

            btnPreTest.CommandArgument =
                "Start";
        }

        private void LoadPostStatus(
    string testID,
    string published)
        {
            if (testID == "")
            {
                SetPostNotPublished();

                return;
            }

            if
            (
                published != "True"
                &&
                published != "1"
            )
            {
                SetPostNotPublished();

                return;
            }

            string sql =
                "SELECT " +
                "SUM(CASE WHEN Submitted=0 THEN 1 ELSE 0 END) AS RunningAttempt," +
                "SUM(CASE WHEN Submitted=1 THEN 1 ELSE 0 END) AS SubmittedAttempt " +
                "FROM TestAttempt " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID),

        new SqlParameter(
            "@EmpID",
            Session["EmpID"].ToString().ToUpperInvariant())
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            int runningAttempt =
                0;

            int submittedAttempt =
                0;

            if
            (
                dt.Rows.Count > 0
            )
            {
                if
                (
                    dt.Rows[0]["RunningAttempt"]
                    !=
                    DBNull.Value
                )
                {
                    runningAttempt =
                        Convert.ToInt32(
                            dt.Rows[0]["RunningAttempt"]);
                }

                if
                (
                    dt.Rows[0]["SubmittedAttempt"]
                    !=
                    DBNull.Value
                )
                {
                    submittedAttempt =
                        Convert.ToInt32(
                            dt.Rows[0]["SubmittedAttempt"]);
                }
            }

            if
            (
                runningAttempt > 0
            )
            {
                lblPostStatus.Text =
                    "In Progress";

                lblPostStatus.CssClass =
                    "badge badge-warning status-badge";

                btnPostTest.Text =
                    "Resume Post Test";

                btnPostTest.Enabled =
                    true;

                btnPostTest.CommandArgument =
                    "Resume";

                return;
            }

            if
            (
                submittedAttempt > 0
            )
            {
                lblPostStatus.Text =
                    "Completed";

                lblPostStatus.CssClass =
                    "badge badge-success status-badge";

                btnPostTest.Text =
                    "View Result";

                btnPostTest.Enabled =
                    true;

                btnPostTest.CommandArgument =
                    "Result";

                return;
            }

            lblPostStatus.Text =
                "Available";

            lblPostStatus.CssClass =
                "badge badge-primary status-badge";

            btnPostTest.Text =
                "Start Post Test";

            btnPostTest.Enabled =
                true;

            btnPostTest.CommandArgument =
                "Start";
        }

        private void SetPreNotPublished()
        {
            lblPreStatus.Text =
                "Not Published";

            lblPreStatus.CssClass =
                "badge badge-secondary status-badge";

            btnPreTest.Text =
                "Pre Test Not Available";

            btnPreTest.Enabled =
                false;

            btnPreTest.CommandArgument =
                "";
        }

        private void SetPostNotPublished()
        {
            lblPostStatus.Text =
                "Not Published";

            lblPostStatus.CssClass =
                "badge badge-secondary status-badge";

            btnPostTest.Text =
                "Post Test Not Available";

            btnPostTest.Enabled =
                false;

            btnPostTest.CommandArgument =
                "";
        }
        protected void btnPreTest_Click(
            object sender,
            EventArgs e)
        {
            if
            (
                btnPreTest.CommandArgument
                ==
                "Result"
            )
            {
                Session["ResultTestType"] =
                    "Pre";

                Response.Redirect(
                    "MyExamResult.aspx");

                return;
            }

            Response.Redirect(
                "PreTrainingExam.aspx");
        }

        protected void btnPostTest_Click(
     object sender,
     EventArgs e)
        {
            if
            (
                btnPostTest.CommandArgument
                ==
                "Result"
            )
            {
                Session["ResultTestType"] =
                    "Post";

                Response.Redirect(
                    "MyExamResult.aspx");

                return;
            }

            Response.Redirect(
                "PostTrainingExam.aspx");
        }

        protected void btnBack_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "TrainingDetails.aspx");
        }
        protected void btnExam_Click(
           object sender,
           EventArgs e)
        {
            Response.Redirect(
                "MyExamResult.aspx");
        }
    }
}