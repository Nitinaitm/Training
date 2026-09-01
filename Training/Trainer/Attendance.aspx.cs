using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class Attendance : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) { BindSummary(); BindGrid(); }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindSummary()
        {
            string totalQuery = @"SELECT COUNT(*) FROM SessionMaster S INNER JOIN TrainingDetails T ON S.TrainingID=T.TrainingID WHERE S.TrainerID=@TrainerID AND T.WorkflowStatus LIKE '%C%'";
            SqlParameter[] totalParam = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            int total = Convert.ToInt32(obj.ExecuteScalar(totalQuery, totalParam) ?? "0");
            lblTotal.Text = total.ToString();

            string completedQuery = @"SELECT COUNT(*) FROM SessionMaster S INNER JOIN TrainingDetails T ON S.TrainingID=T.TrainingID WHERE S.TrainerID=@TrainerID AND S.AttendanceStatus='Completed' AND T.WorkflowStatus LIKE '%C%'";
            SqlParameter[] completedParam = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            int completed = Convert.ToInt32(obj.ExecuteScalar(completedQuery, completedParam) ?? "0");
            lblCompleted.Text = completed.ToString();

            string pendingQuery = @"SELECT COUNT(*) FROM SessionMaster S INNER JOIN TrainingDetails T ON S.TrainingID=T.TrainingID WHERE S.TrainerID=@TrainerID AND T.WorkflowStatus LIKE '%E%' AND ISNULL(S.AttendanceStatus,'')<>'Completed' AND T.WorkflowStatus LIKE '%C%'";
            SqlParameter[] pendingParam = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            int pending = Convert.ToInt32(obj.ExecuteScalar(pendingQuery, pendingParam) ?? "0");
            lblPending.Text = pending.ToString();

            decimal percent = total > 0 ? Math.Round((decimal)completed / total * 100, 2) : 0;
            lblPercent.Text = percent.ToString("0.00") + "%";
        }

        private void BindGrid()
        {
            string query = @"SELECT S.SessionID, S.TrainingID, TD.Batch, S.SessionNo, S.SessionName, S.SessionDate, S.StartTime, S.EndTime, ISNULL(S.AttendanceStatus,'Pending') AS AttendanceStatus, (SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID = S.TrainingID AND AssignmentStatus='Assigned') AS TotalTrainees, (SELECT COUNT(*) FROM SessionAttendance WHERE SessionID = S.SessionID AND AttendanceStatus='Present') AS Present, (SELECT COUNT(*) FROM SessionAttendance WHERE SessionID = S.SessionID AND AttendanceStatus='Absent') AS Absent FROM SessionMaster S INNER JOIN TrainingDetails TD ON S.TrainingID = TD.TrainingID WHERE S.TrainerID = @TrainerID AND TD.WorkflowStatus LIKE '%C%' ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TrainerID", TrainerID));

            if (!string.IsNullOrEmpty(txtTrainingID.Text.Trim()))
            { query += " AND S.TrainingID LIKE @TrainingID"; parameters.Add(new SqlParameter("@TrainingID", "%" + txtTrainingID.Text.Trim() + "%")); }

            if (!string.IsNullOrEmpty(txtFrom.Text.Trim()))
            { query += " AND TRY_CONVERT(date, S.SessionDate, 105) >= @DateFrom"; parameters.Add(new SqlParameter("@DateFrom", Convert.ToDateTime(txtFrom.Text))); }

            if (!string.IsNullOrEmpty(txtTo.Text.Trim()))
            { query += " AND TRY_CONVERT(date, S.SessionDate, 105) <= @DateTo"; parameters.Add(new SqlParameter("@DateTo", Convert.ToDateTime(txtTo.Text))); }

            if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
            {
                if (ddlStatus.SelectedValue == "Pending") query += " AND ISNULL(S.AttendanceStatus,'') <> 'Completed'";
                else if (ddlStatus.SelectedValue == "Completed") query += " AND S.AttendanceStatus = 'Completed'";
            }

            query += " ORDER BY TRY_CONVERT(date, S.SessionDate, 105) DESC, CAST(S.SessionNo AS INT)";
            DataTable dt = obj.GetDataTable(query, parameters.ToArray());
            gvAttendance.DataSource = dt;
            gvAttendance.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e) => BindGrid();

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtTrainingID.Text = ""; txtFrom.Text = ""; txtTo.Text = ""; ddlStatus.SelectedIndex = 0;
            BindGrid();
        }

        protected void gvAttendance_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "TakeAttendance")
            {
                string sessionID = e.CommandArgument.ToString();
                Session["SessionID"] = sessionID;
                Response.Redirect("~/Trainer/SessionDetails.aspx");
            }
        }
    }
}