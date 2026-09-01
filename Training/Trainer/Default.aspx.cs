using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class Default : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["TrainerID"] = "TRIN002";
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                BindCourse();

                BindBatch();

                BindSummary();

                BindGrid();
            }
        }

        private string TrainerID
        {
            get
            {
                return Session["TrainerID"].ToString();
            }
        }

        private void BindCourse()
        {
            string query = @"SELECT DISTINCT CM.CourseID,CM.CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE SM.TrainerID=@TrainerID ORDER BY CM.CourseName";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainerID",TrainerID)
            };

            DataTable dt = obj.GetDataTable(query, param);

            ddlCourse.DataSource = dt;

            ddlCourse.DataTextField = "CourseName";

            ddlCourse.DataValueField = "CourseID";

            ddlCourse.DataBind();

            ddlCourse.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", ""));
        }

        private void BindBatch()
        {
            string query = @"SELECT DISTINCT Batch FROM TrainingDetails TD INNER JOIN SessionMaster SM ON TD.TrainingID=SM.TrainingID WHERE SM.TrainerID=@TrainerID ORDER BY Batch";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainerID",TrainerID)
            };

            DataTable dt = obj.GetDataTable(query, param);

            ddlBatch.DataSource = dt;

            ddlBatch.DataTextField = "Batch";

            ddlBatch.DataValueField = "Batch";

            ddlBatch.DataBind();

            ddlBatch.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All", ""));
        }

        private void BindSummary()
        {
            lblTodaySession.Text = GetCount(@"SELECT COUNT(*) FROM SessionMaster SM
INNER JOIN TrainingDetails TD
ON SM.TrainingID=TD.TrainingID
WHERE SM.TrainerID=@TrainerID
AND AND TD.TrainingStatus IN ('InProgress','AttendanceCompleted')
AND SM.SessionDate=@Today");

            lblPendingAttendance.Text = GetCount(@"SELECT COUNT(*) FROM SessionMaster SM
INNER JOIN TrainingDetails TD
ON SM.TrainingID=TD.TrainingID
WHERE SM.TrainerID=@TrainerID
AND TD.TrainingStatus='InProgress'
AND ISNULL(SM.AttendanceStatus,'Pending')<>'Completed'");

            lblPendingPreTest.Text = GetCount(@"SELECT COUNT(*) FROM TestMaster TM INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE SM.TrainerID=@TrainerID AND TM.TestType='PRE' AND ISNULL(TM.TestStatus,'Pending')='Pending'");

            lblPendingPostTest.Text = GetCount(@"SELECT COUNT(*) FROM TestMaster TM INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE SM.TrainerID=@TrainerID AND TM.TestType='POST' AND ISNULL(TM.TestStatus,'Pending')='Pending'");
        }

        private string GetCount(string query)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@TrainerID",TrainerID),
                new SqlParameter("@Today",DateTime.Now.ToString("dd-MM-yyyy"))
            };

            object count = obj.ExecuteScalar(query, param);

            if (count == null) return "0";

            return count.ToString();
        }

        private void BindGrid()
        {
            string query = @"SELECT
SM.SessionID,
SM.TrainingID,
CM.CourseName,
TD.Batch,
SM.SessionNo,
SM.SessionName,
TP.TopicName,
SM.SessionDate,
SM.StartTime,
SM.EndTime,
TD.WorkflowStatus,
TD.TrainingStatus,
ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus
FROM SessionMaster SM
INNER JOIN TrainingDetails TD
ON SM.TrainingID=TD.TrainingID
INNER JOIN CourseMaster CM
ON TD.CourseID=CM.CourseID
LEFT JOIN TopicMaster TP
ON SM.TopicID=TP.TopicID
WHERE SM.TrainerID=@TrainerID
AND TD.TrainingStatus IN ('InProgress','AttendanceCompleted')
ORDER BY
TRY_CONVERT(date,SM.SessionDate,105),
CAST(SM.SessionNo AS INT)";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainerID",TrainerID)
            };

            DataTable dt = obj.GetDataTable(query, param);

            gvSession.DataSource = dt;

            gvSession.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = @"SELECT SM.SessionID,SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,TP.TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,TD.WorkflowStatus,TD.TrainingStatus,ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.TrainerID=@TrainerID AND TD.TrainingStatus IN ('InProgress','AttendanceCompleted')";

            if (ddlCourse.SelectedValue != "") query += " AND TD.CourseID=@CourseID";

            if (ddlBatch.SelectedValue != "") query += " AND TD.Batch=@Batch";

            if (txtFromDate.Text.Trim() != "") query += " AND TRY_CONVERT(date,SM.SessionDate,105)>=TRY_CONVERT(date,@FromDate,105)";

            if (txtToDate.Text.Trim() != "") query += " AND TRY_CONVERT(date,SM.SessionDate,105)<=TRY_CONVERT(date,@ToDate,105)";

            query += " ORDER BY TRY_CONVERT(date,SM.SessionDate,105),CAST(SM.SessionNo AS INT)";

            SqlParameterCollectionDummy();
        }

        private void SqlParameterCollectionDummy()
        {
            System.Collections.Generic.List<SqlParameter> param = new System.Collections.Generic.List<SqlParameter>();

            param.Add(new SqlParameter("@TrainerID", TrainerID));

            if (ddlCourse.SelectedValue != "") param.Add(new SqlParameter("@CourseID", ddlCourse.SelectedValue));

            if (ddlBatch.SelectedValue != "") param.Add(new SqlParameter("@Batch", ddlBatch.SelectedValue));

            if (txtFromDate.Text.Trim() != "") param.Add(new SqlParameter("@FromDate", txtFromDate.Text.Trim()));

            if (txtToDate.Text.Trim() != "") param.Add(new SqlParameter("@ToDate", txtToDate.Text.Trim()));

            string query = @"SELECT SM.SessionID,SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,TP.TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,TD.WorkflowStatus,TD.TrainingStatus,ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.TrainerID=@TrainerID AND TD.TrainingStatus IN ('InProgress','AttendanceCompleted')";

            if (ddlCourse.SelectedValue != "") query += " AND TD.CourseID=@CourseID";

            if (ddlBatch.SelectedValue != "") query += " AND TD.Batch=@Batch";

            if (txtFromDate.Text.Trim() != "") query += " AND TRY_CONVERT(date,SM.SessionDate,105)>=TRY_CONVERT(date,@FromDate,105)";

            if (txtToDate.Text.Trim() != "") query += " AND TRY_CONVERT(date,SM.SessionDate,105)<=TRY_CONVERT(date,@ToDate,105)";

            query += " ORDER BY TRY_CONVERT(date,SM.SessionDate,105),CAST(SM.SessionNo AS INT)";

            DataTable dt = obj.GetDataTable(query, param.ToArray());

            gvSession.DataSource = dt;

            gvSession.DataBind();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlCourse.SelectedIndex = 0;

            ddlBatch.SelectedIndex = 0;

            txtFromDate.Text = "";

            txtToDate.Text = "";

            BindGrid();
        }

        protected void gvSession_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "View")
            {
                return;
            }

            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;

            Session["SessionID"] =
                gvSession.DataKeys[row.RowIndex].Values["SessionID"].ToString();

            Session["TrainingID"] =
                gvSession.DataKeys[row.RowIndex].Values["TrainingID"].ToString();

            Response.Redirect("~/Trainer/SessionDetails.aspx");
        }

        protected void gvSession_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            Label lblWorkflow = (Label)e.Row.FindControl("lblWorkflow");

            if (lblWorkflow == null) return;

            switch (lblWorkflow.Text)
            {
                case "A":

                    lblWorkflow.Text = "Draft";

                    lblWorkflow.CssClass = "badge bg-secondary";

                    break;

                case "B":

                    lblWorkflow.Text = "Trainer Assigned";

                    lblWorkflow.CssClass = "badge bg-info";

                    break;

                case "C":

                    lblWorkflow.Text = "Sessions Created";

                    lblWorkflow.CssClass = "badge bg-primary";

                    break;

                case "D":

                    lblWorkflow.Text = "Trainees Assigned";

                    lblWorkflow.CssClass = "badge bg-warning";

                    break;

                case "ABCDE":
                    lblWorkflow.Text = "Training Started";
                    break;

                case "ABCDEF":
                    lblWorkflow.Text = "Attendance Completed";
                    break;

                case "ABCDEFG":
                    lblWorkflow.Text = "Pre Test Completed";
                    break;

                case "ABCDEFGH":
                    lblWorkflow.Text = "Post Test Completed";
                    break;

                case "ABCDEFGHI":
                    lblWorkflow.Text = "Feedback Submitted";
                    break;

                case "ABCDEFGHIJ":
                    lblWorkflow.Text = "Certificate Generated";
                    break;

                default:

                    lblWorkflow.CssClass = "badge bg-secondary";

                    break;
            }
        }
    }
}