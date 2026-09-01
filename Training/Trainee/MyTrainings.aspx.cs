using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MyTrainings : System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["EmpID"] == null)
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if (!IsPostBack)
            {
                LoadCourse();

                ViewState["SortExpression"] =
                    "TrainingID";

                ViewState["SortDirection"] =
                    "DESC";

                ddlStatus.SelectedIndex =
                    0;

                LoadTraining();
            }
        }
        private void LoadCourse()
        {
            string sql =
                "SELECT DISTINCT " +
                "CM.CourseID, " +
                "CM.CourseName " +
                "FROM TrainingAssignment TA " +
                "INNER JOIN TrainingDetails TD " +
                "ON TA.TrainingID=TD.TrainingID " +
                "INNER JOIN CourseMaster CM " +
                "ON TD.CourseID=CM.CourseID " +
                "WHERE TA.EmpID=@EmpID " +
                "ORDER BY CM.CourseName";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@EmpID",
            Session["EmpID"].ToString().ToUpperInvariant())
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            ddlCourse.DataSource =
                dt;

            ddlCourse.DataTextField =
                "CourseName";

            ddlCourse.DataValueField =
                "CourseID";

            ddlCourse.DataBind();

            ddlCourse.Items.Insert(
                0,
                new ListItem(
                    "All",
                    ""));
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            gvTraining.PageIndex =
                0;

            LoadTraining();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtTrainingID.Text =
                "";

            ddlCourse.SelectedIndex =
     0;

            ddlStatus.SelectedIndex =
                0;

            ViewState["SortExpression"] =
                "TrainingID";

            ViewState["SortDirection"] =
                "DESC";

            gvTraining.PageIndex =
                0;

            LoadTraining();
        }

        protected void gvTraining_PageIndexChanging(
            object sender,
            GridViewPageEventArgs e)
        {
            gvTraining.PageIndex =
                e.NewPageIndex;

            LoadTraining();
        }

        protected void gvTraining_Sorting(
            object sender,
            GridViewSortEventArgs e)
        {
            if (ViewState["SortExpression"].ToString() ==
                e.SortExpression)
            {
                if (ViewState["SortDirection"].ToString() ==
                    "ASC")
                {
                    ViewState["SortDirection"] =
                        "DESC";
                }
                else
                {
                    ViewState["SortDirection"] =
                        "ASC";
                }
            }
            else
            {
                ViewState["SortExpression"] =
                    e.SortExpression;

                ViewState["SortDirection"] =
                    "ASC";
            }

            LoadTraining();
        }
        protected void gvTraining_RowDataBound(
    object sender,
    GridViewRowEventArgs e)
        {
            if
            (
                e.Row.RowType
                !=
                DataControlRowType.DataRow
            )
            {
                return;
            }

            LinkButton lnkFeedback =
                (LinkButton)e.Row.FindControl(
                    "lnkFeedback");

            LinkButton lnkCertificate =
                (LinkButton)e.Row.FindControl(
                    "lnkCertificate");

            if
            (
                !lnkFeedback.Enabled
            )
            {
                lnkFeedback.CssClass =
                    "btn btn-warning btn-sm disabled";
            }

            if
            (
                !lnkCertificate.Enabled
            )
            {
                lnkCertificate.CssClass =
                    "btn btn-info btn-sm disabled";
            }
        }
        protected void gvTraining_RowCommand(
    object sender,
    GridViewCommandEventArgs e)
        {
            string trainingID =
                e.CommandArgument
                .ToString();

            Session["TrainingID"] =
                trainingID;

            switch
            (
                e.CommandName
            )
            {
                case "ViewTraining":

                    Response.Redirect(
                        "TrainingDetails.aspx",
                        false);

                    break;

                case "Attendance":

                    Response.Redirect(
                        "Attendance.aspx",
                        false);

                    break;

                case "BatchFeedback":

                    Response.Redirect(
                        "TraineeFeedback.aspx",
                        false);

                    break;

                case "Certificate":

                    Session["CertificateFromTraining"] =
                        true;

                    Response.Redirect(
                        "MyCertificate.aspx",
                        false);

                    break;
            }
        }
        private string GetSortColumn()
        {
            string sortColumn =
                Convert.ToString(
                    ViewState["SortExpression"]);

            switch
            (
                sortColumn
            )
            {
                case "TrainingID":
                case "CourseName":
                case "TrainingType":
                case "TrainingOrganizer":
                case "Batch":
                case "DateFrom":
                case "DateTo":

                    return
                        sortColumn;

                default:

                    return
                        "TrainingID";
            }
        }

        private string GetSortDirection()
        {
            string direction =
                Convert.ToString(
                    ViewState["SortDirection"]);

            if
            (
                direction
                ==
                "ASC"
            )
            {
                return
                    "ASC";
            }

            return
                "DESC";
        }
        private void LoadTraining()
        {
            string sql =
"SELECT " +
"TA.TrainingID," +
"CM.CourseName," +
"TD.TrainingType," +
"TD.TrainingOrganizer," +
"TD.Batch," +
"TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom," +
"TRY_CONVERT(date,TD.DateTo,105) AS DateTo," +
"ISNULL(TP.AttendanceCompleted,0) AS AttendanceCompleted," +
"ISNULL(TP.PreExamCompleted,0) AS PreExamCompleted," +
"ISNULL(TP.PostExamCompleted,0) AS PostExamCompleted," +
"ISNULL(TP.SessionFeedbackCompleted,0) AS SessionFeedbackCompleted," +
"ISNULL(TP.BatchFeedbackCompleted,0) AS BatchFeedbackCompleted," +
"ISNULL(TP.CertificateGenerated,0) AS CertificateGenerated," +
"ISNULL(TP.WorkflowStatus,'P') AS WorkflowStatus," +

"CASE " +
"WHEN NOT EXISTS " +
"( " +
"SELECT 1 " +
"FROM SessionMaster SM " +
"WHERE SM.TrainingID=TA.TrainingID " +
"AND " +
"( " +

"ISNULL(SM.AttendanceStatus,'')<>'Completed' " +

"OR EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestMaster TM " +
"WHERE TM.SessionID=SM.SessionID " +
"AND TM.TestType='Pre' " +
"AND TM.IsPublished=1 " +
"AND NOT EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestAttempt TTA " +
"WHERE TTA.TestID=TM.TestID " +
"AND TTA.EmpID=TA.EmpID " +
"AND TTA.Submitted=1 " +
") " +
") " +

"OR EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestMaster TM " +
"WHERE TM.SessionID=SM.SessionID " +
"AND TM.TestType='Post' " +
"AND TM.IsPublished=1 " +
"AND NOT EXISTS " +
"( " +
"SELECT 1 " +
"FROM TestAttempt TTA " +
"WHERE TTA.TestID=TM.TestID " +
"AND TTA.EmpID=TA.EmpID " +
"AND TTA.Submitted=1 " +
") " +
") " +

") " +
") " +
"THEN CAST(1 AS BIT) " +
"ELSE CAST(0 AS BIT) " +
"END AS CanBatchFeedback," +
"CASE " +
"WHEN ISNULL(TP.BatchFeedbackCompleted,0)=1 " +
"THEN CAST(1 AS BIT) " +
"ELSE CAST(0 AS BIT) " +
"END AS CanCertificate " +

"FROM TrainingAssignment TA " +

"INNER JOIN TrainingDetails TD " +
"ON TD.TrainingID=TA.TrainingID " +

"INNER JOIN CourseMaster CM " +
"ON CM.CourseID=TD.CourseID " +

"LEFT JOIN TrainingProgress TP " +
"ON TP.TrainingID=TA.TrainingID " +
"AND TP.EmpID=TA.EmpID " +

"WHERE TA.EmpID=@EmpID ";

            if (txtTrainingID.Text.Trim() != "")
            {
                sql +=
                    "AND TA.TrainingID LIKE @TrainingID ";
            }

            if (ddlCourse.SelectedValue != "")
            {
                sql +=
                    "AND TD.CourseID=@CourseID ";
            }


            if (ddlStatus.SelectedValue != "")
            {
                sql +=
                    "AND ISNULL(TP.WorkflowStatus,'P')=@WorkflowStatus ";
            }
            sql +=
    "ORDER BY " +
    GetSortColumn()
    +
    " "
    +
    GetSortDirection();


            SqlParameter[] param =
     new SqlParameter[]
     {
        new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()),
        new SqlParameter("@TrainingID", "%" + txtTrainingID.Text.Trim() + "%"),
        new SqlParameter("@CourseID", ddlCourse.SelectedValue),
        new SqlParameter("@WorkflowStatus", ddlStatus.SelectedValue)
     };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            dt.Columns.Add(
                "ProgressPercent");

            dt.Columns.Add(
                "StatusText");

            dt.Columns.Add(
                "StatusClass");

            dt.Columns.Add(
                "CanAttendance",
                typeof(bool));

            foreach (DataRow dr in dt.Rows)
            {
                dr["CanAttendance"] =
                    true;

                int completed =
                    0;

                if (Convert.ToBoolean(dr["AttendanceCompleted"]))
                {
                    completed++;
                }

                if (Convert.ToBoolean(dr["PreExamCompleted"]))
                {
                    completed++;
                }

                if (Convert.ToBoolean(dr["PostExamCompleted"]))
                {
                    completed++;
                }

                if (Convert.ToBoolean(dr["BatchFeedbackCompleted"]))
                {
                    completed++;
                }

                if (Convert.ToBoolean(dr["CertificateGenerated"]))
                {
                    completed++;
                }

                int totalSteps =
                    5;

                dr["ProgressPercent"] =
                    completed * 100 / totalSteps;

                //dr["ProgressPercent"] =
                //    progress;

                bool attendance =
     Convert.ToBoolean(
         dr["AttendanceCompleted"]);

                bool pre =
                    Convert.ToBoolean(
                        dr["PreExamCompleted"]);

                bool post =
                    Convert.ToBoolean(
                        dr["PostExamCompleted"]);

                bool feedback =
                    Convert.ToBoolean(
                        dr["BatchFeedbackCompleted"]);

                bool certificate =
                    Convert.ToBoolean(
                        dr["CertificateGenerated"]);

                if (certificate)
                {
                    dr["StatusText"] =
                        "Completed";

                    dr["StatusClass"] =
                        "badge badge-success badge-status";
                }
                else if
                (
                    attendance ||
                    pre ||
                    post ||
                    feedback
                )
                {
                    dr["StatusText"] =
                        "In Progress";

                    dr["StatusClass"] =
                        "badge badge-warning badge-status";
                }
                else
                {
                    dr["StatusText"] =
                        "Pending";

                    dr["StatusClass"] =
                        "badge badge-secondary badge-status";
                }
            }

            gvTraining.DataSource =
                dt;

            gvTraining.DataBind();
        }

    }
}

