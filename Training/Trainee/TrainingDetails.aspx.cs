using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class TrainingDetails : System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        private string
            TrainingID =
            "";

        private string
            EmpID =
            "";

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
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            EmpID =
                Session["EmpID"]
                .ToString().ToUpperInvariant();

            TrainingID =
                Session["TrainingID"]
                .ToString();

            if
            (
                !IsPostBack
            )
            {
                string trainingID =
        Session["TrainingID"].ToString();

                string empID =
                    Session["EmpID"].ToString().ToUpperInvariant();
                TraineeTrainingSummary1.LoadTraining(
        trainingID,
        empID);


                LoadTrainingSummary();

                LoadSessionGrid();

                LoadProgress();

                LoadWorkflow();
            }
        }
        protected void gvSession_RowDataBound(
    object sender,
    GridViewRowEventArgs e)
        {
            if
            (
                e.Row.RowType !=
                DataControlRowType.DataRow
            )
            {
                return;
            }

            string pre =
                DataBinder.Eval(
                    e.Row.DataItem,
                    "PreStatus")
                .ToString();

            string post =
                DataBinder.Eval(
                    e.Row.DataItem,
                    "PostStatus")
                .ToString();

            Label lblPre =
                (Label)e.Row.FindControl("lblPre");

            Label lblPost =
                (Label)e.Row.FindControl("lblPost");

            if
            (
                lblPre != null
            )
            {
                lblPre.CssClass =
                    GetBadgeClass(pre);
            }

            if
            (
                lblPost != null
            )
            {
                lblPost.CssClass =
                    GetBadgeClass(post);
            }
        }

        private string GetBadgeClass(
            string status)
        {
            switch (status)
            {
                case "Completed":
                    return "badge badge-success";

                case "Available":
                    return "badge badge-primary";

                case "Locked":
                    return "badge badge-secondary";

                case "Pending":
                    return "badge badge-warning";

                default:
                    return "badge badge-light";
            }
        }

        private string GetResultBadgeClass(
            string result)
        {
            if
            (
                result.StartsWith("Pass")
            )
            {
                return "badge badge-success";
            }

            if
            (
                result.StartsWith("Fail")
            )
            {
                return "badge badge-danger";
            }

            return "badge badge-secondary";
        }
        private void LoadSessionGrid()
        {
            string sql =
"SELECT " +
"SM.SessionID," +
"SM.SessionNo," +
"SM.SessionName," +
"TM.TopicName," +
"CASE " +
"WHEN TR.TrainerType='Internal' " +
"THEN ISNULL(EB.EmpName,'') " +
"ELSE ISNULL(TR.NameExternal,'') " +
"END AS TrainerName," +
"TRY_CONVERT(date,SM.SessionDate,105) AS SessionDate," +
"SM.StartTime," +
"SM.EndTime," +
"ISNULL(SA.AttendanceStatus,'Pending') AS AttendanceStatus," +

/************* PRE TEST *************/

"CASE " +

"WHEN ISNULL(SA.AttendanceStatus,'Pending')<>'Completed' " +
"THEN 'Locked' " +

"WHEN NOT EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestMaster TT " +
"WHERE TT.SessionID=SM.SessionID " +
"AND TT.TestType='Pre' " +
"AND TT.IsPublished=1 " +
") " +
"THEN '-' " +

"WHEN EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestMaster TT " +
"INNER JOIN TestAttempt TA " +
"ON TT.TestID=TA.TestID " +
"WHERE TT.SessionID=SM.SessionID " +
"AND TT.TestType='Pre' " +
"AND TT.IsPublished=1 " +
"AND TA.EmpID=@EmpID " +
"AND TA.Submitted=1 " +
") " +
"THEN 'Completed' " +

"ELSE 'Available' " +

"END AS PreStatus," +

/************* POST TEST *************/

"CASE " +

"WHEN ISNULL(SA.AttendanceStatus,'Pending')<>'Completed' " +
"THEN 'Locked' " +

"WHEN NOT EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestMaster TT " +
"WHERE TT.SessionID=SM.SessionID " +
"AND TT.TestType='Post' " +
"AND TT.IsPublished=1 " +
") " +
"THEN '-' " +

"WHEN EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestMaster TT " +
"INNER JOIN TestAttempt TA " +
"ON TT.TestID=TA.TestID " +
"WHERE TT.SessionID=SM.SessionID " +
"AND TT.TestType='Post' " +
"AND TT.IsPublished=1 " +
"AND TA.EmpID=@EmpID " +
"AND TA.Submitted=1 " +
") " +
"THEN 'Completed' " +

"ELSE 'Available' " +

"END AS PostStatus " +

"FROM SessionMaster SM " +

"LEFT JOIN TopicMaster TM " +
"ON TM.TopicID=SM.TopicID " +

"LEFT JOIN TrainerMaster TR " +
"ON TR.TrainerID=SM.TrainerID " +

"LEFT JOIN EmpBasicMaster EB " +
"ON EB.EmpID=TR.EmpID " +

"LEFT JOIN SessionAttendance SA " +
"ON SA.SessionID=SM.SessionID " +
"AND SA.EmpID=@EmpID " +

"WHERE SM.TrainingID=@TrainingID " +

"ORDER BY " +
"TRY_CONVERT(INT,SM.SessionNo)," +
"SM.SessionNo";


            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            TrainingID),

        new SqlParameter(
            "@EmpID",
            EmpID)
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            gvSession.DataSource =
                dt;

            gvSession.DataBind();

            ViewState["CompletedSession"] =
                dt.Select(
                    "AttendanceStatus='Completed'")
                .Length;

            ViewState["PendingSession"] =
                dt.Rows.Count
                -
                Convert.ToInt32(
                    ViewState["CompletedSession"]);
        }
        private void LoadTrainingSummary()
        {
            string sql =
                "SELECT " +
                "TD.TrainingID," +
                "CM.CourseName," +
                "TD.TrainingType," +
                "TD.TrainingOrganizer," +
                "TD.TrainingLocation," +
                "TD.Batch," +
                "TRY_CONVERT(date,TD.DateFrom,105) DateFrom," +
                "TRY_CONVERT(date,TD.DateTo,105) DateTo," +
                "(SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID) TotalSession " +
                "FROM TrainingDetails TD " +
                "INNER JOIN CourseMaster CM " +
                "ON TD.CourseID=CM.CourseID " +
                "WHERE TD.TrainingID=@TrainingID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            TrainingID)
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            ViewState["TotalSession"] =
                dt.Rows[0]["TotalSession"];
        }


        protected void gvSession_RowCommand(
object sender,
GridViewCommandEventArgs e)
        {
            if
            (
                e.CommandName
                ==
                "ViewSession"
            )
            {
                Session["SessionID"] =
                    e.CommandArgument
                    .ToString();

                Response.Redirect(
                    "MySessions.aspx",
                    false);
            }
        }
        private void LoadProgress()
        {
            string sql =
                "SELECT " +
                "COUNT(*) TotalSession," +
                "SUM(CASE WHEN ISNULL(SA.AttendanceStatus,'Pending')='Completed' THEN 1 ELSE 0 END) AttendanceCompleted " +
                "FROM SessionMaster SM " +
                "LEFT JOIN SessionAttendance SA " +
                "ON SA.SessionID=SM.SessionID " +
                "AND SA.EmpID=@EmpID " +
                "WHERE SM.TrainingID=@TrainingID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            TrainingID),

        new SqlParameter(
            "@EmpID",
            EmpID)
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                progressBar.Style["width"] =
                    "0%";

                lblProgress.Text =
                    "0%";

                return;
            }

            int total =
                Convert.ToInt32(
                    dt.Rows[0]["TotalSession"]);

            int completed =
                Convert.ToInt32(
                    dt.Rows[0]["AttendanceCompleted"]);

            int percentage =
                0;

            if
            (
                total > 0
            )
            {
                percentage =
                    completed * 100 / total;
            }

            progressBar.Style["width"] =
                percentage + "%";

            progressBar.Attributes["aria-valuenow"] =
                percentage.ToString();

            lblProgress.Text =
                percentage + "%";

            lblNextActivity.Text =
                completed == total
                ?
                "Complete Batch Feedback"
                :
                "Complete Remaining Sessions";
        }

      

        private void LoadWorkflow()
        {
            string sql =
                "SELECT " +
                "CASE " +
                "WHEN NOT EXISTS " +
                "( " +
                "SELECT 1 " +
                "FROM SessionMaster SM " +
                "WHERE SM.TrainingID=@TrainingID " +
                "AND ISNULL((SELECT TOP 1 AttendanceStatus FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID),'Pending')<>'Completed' " +
                ") " +
                "THEN 1 ELSE 0 END AttendanceDone," +
                "CASE " +
                "WHEN EXISTS " +
                "( " +
                "SELECT 1 " +
                "FROM TrainingCertificate " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID " +
                "AND CertificateStatus='A' " +
                ") " +
                "THEN 1 ELSE 0 END CertificateReady";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            TrainingID),

        new SqlParameter(
            "@EmpID",
            EmpID)
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            bool attendanceDone =
                Convert.ToBoolean(
                    dt.Rows[0]["AttendanceDone"]);

            bool certificateReady =
                Convert.ToBoolean(
                    dt.Rows[0]["CertificateReady"]);

            btnBatchFeedback.Enabled =
                attendanceDone;

            btnCertificate.Enabled =
                certificateReady;
        }
     
       

        

      

        protected void btnBatchFeedback_Click(
     object sender,
     EventArgs e)
        {
            Session["TrainingID"] =
                TrainingID;

            Response.Redirect(
                "BatchFeedback.aspx",
                false);
        }

        protected void btnCertificate_Click(
      object sender,
      EventArgs e)
        {
            Session["TrainingID"] =
                TrainingID;

            Response.Redirect(
                "MyCertificate.aspx",
                false);
        }
        protected void btnBack_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "MyTrainings.aspx");
        }
    }
}