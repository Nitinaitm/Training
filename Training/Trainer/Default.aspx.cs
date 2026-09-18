using System;
using System.Collections.Generic;
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
            if (Session["TrainerID"] == null || string.IsNullOrWhiteSpace(Session["TrainerID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindCourse();
                BindBatch();
                BindSummary();
                BindGrid();
                BindClosedTraining();
            }
        }

        private string TrainerID
        {
            get { return Session["TrainerID"].ToString(); }
        }

        private void BindCourse()
        {
            string query = "SELECT DISTINCT CM.CourseID,CM.CourseName FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE SM.TrainerID=@TrainerID ORDER BY CM.CourseName";
            SqlParameter[] param = { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, new ListItem("All", ""));
        }

        private void BindBatch()
        {
            string query = "SELECT DISTINCT TD.Batch FROM TrainingDetails TD INNER JOIN SessionMaster SM ON TD.TrainingID=SM.TrainingID WHERE SM.TrainerID=@TrainerID ORDER BY TD.Batch";
            SqlParameter[] param = { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            ddlBatch.DataSource = dt;
            ddlBatch.DataTextField = "Batch";
            ddlBatch.DataValueField = "Batch";
            ddlBatch.DataBind();
            ddlBatch.Items.Insert(0, new ListItem("All", ""));
        }

        private void BindSummary()
        {
            lblTodaySession.Text = GetCount("SELECT COUNT(*) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID WHERE SM.TrainerID=@TrainerID AND TD.TrainingStatus IN ('InProgress','AttendanceCompleted') AND TRY_CONVERT(date,SM.SessionDate,105)=CAST(GETDATE() AS date)");
            lblPendingAttendance.Text = GetCount("SELECT COUNT(*) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID WHERE SM.TrainerID=@TrainerID AND TD.TrainingStatus='InProgress' AND ISNULL(SM.AttendanceStatus,'Pending')<>'Completed'");
            lblPendingPreTest.Text = GetCount("SELECT COUNT(*) FROM TestMaster TM INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE SM.TrainerID=@TrainerID AND TM.TestType='PRE' AND ISNULL(TM.TestStatus,'Pending')='Pending'");
            lblPendingPostTest.Text = GetCount("SELECT COUNT(*) FROM TestMaster TM INNER JOIN SessionMaster SM ON TM.SessionID=SM.SessionID WHERE SM.TrainerID=@TrainerID AND TM.TestType='POST' AND ISNULL(TM.TestStatus,'Pending')='Pending'");
        }

        private string GetCount(string query)
        {
            SqlParameter[] param = { new SqlParameter("@TrainerID", TrainerID) };
            object count = obj.ExecuteScalar(query, param);
            return count == null || count == DBNull.Value ? "0" : count.ToString();
        }

        private void BindGrid()
        {
            string query = "SELECT SM.SessionID,SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,TP.TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,TD.WorkflowStatus,TD.TrainingStatus,ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.TrainerID=@TrainerID AND ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed','TrainingCompleted') ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo)";
            SqlParameter[] param = { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            gvSession.DataSource = dt;
            gvSession.DataBind();
        }

        private void BindClosedTraining()
        {
            string query = "SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateFrom,105),105) AS DateFrom,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateTo,105),105) AS DateTo FROM TrainingDetails TD INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID WHERE SM.TrainerID=@TrainerID AND TD.TrainingStatus='Closed' ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            SqlParameter[] param = { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            gvClosedTraining.DataSource = dt;
            gvClosedTraining.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = "SELECT SM.SessionID,SM.TrainingID,CM.CourseName,TD.Batch,SM.SessionNo,SM.SessionName,TP.TopicName,SM.SessionDate,SM.StartTime,SM.EndTime,TD.WorkflowStatus,TD.TrainingStatus,ISNULL(SM.AttendanceStatus,'Pending') AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN TopicMaster TP ON SM.TopicID=TP.TopicID WHERE SM.TrainerID=@TrainerID AND ISNULL(TD.TrainingStatus,'') NOT IN ('Closed','Completed','TrainingCompleted')";
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@TrainerID", TrainerID));
            if (!string.IsNullOrEmpty(ddlCourse.SelectedValue))
            {
                query += " AND TD.CourseID=@CourseID";
                param.Add(new SqlParameter("@CourseID", ddlCourse.SelectedValue));
            }
            if (!string.IsNullOrEmpty(ddlBatch.SelectedValue))
            {
                query += " AND TD.Batch=@Batch";
                param.Add(new SqlParameter("@Batch", ddlBatch.SelectedValue));
            }
            if (!string.IsNullOrEmpty(txtFromDate.Text.Trim()))
            {
                query += " AND TRY_CONVERT(date,SM.SessionDate,105)>=TRY_CONVERT(date,@FromDate,105)";
                param.Add(new SqlParameter("@FromDate", txtFromDate.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(txtToDate.Text.Trim()))
            {
                query += " AND TRY_CONVERT(date,SM.SessionDate,105)<=TRY_CONVERT(date,@ToDate,105)";
                param.Add(new SqlParameter("@ToDate", txtToDate.Text.Trim()));
            }
            query += " ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo)";
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
            if (e.CommandName != "View") return;
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string sessionID = gvSession.DataKeys[row.RowIndex].Values["SessionID"].ToString();
            string trainingID = gvSession.DataKeys[row.RowIndex].Values["TrainingID"].ToString();
            Session["SessionID"] = sessionID;
            Session["TrainingID"] = trainingID;
            Response.Redirect("~/Trainer/SessionDetails.aspx");
        }

        protected void gvClosedTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "History") return;
            string trainingID = e.CommandArgument == null ? "" : e.CommandArgument.ToString();
            if (string.IsNullOrWhiteSpace(trainingID)) return;
            object access = obj.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND TrainerID=@TrainerID AND EXISTS (SELECT 1 FROM TrainingDetails TD WHERE TD.TrainingID=SessionMaster.TrainingID AND TD.TrainingStatus='Closed')", new SqlParameter[]
            {
                new SqlParameter("@TrainingID", trainingID),
                new SqlParameter("@TrainerID", TrainerID)
            });
            if (access == null || Convert.ToInt32(access) == 0) return;
            Response.Redirect("~/TrainingHistory.aspx?TrainingID=" + Server.UrlEncode(trainingID));
        }

        protected void gvSession_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            Label lblWorkflow = (Label)e.Row.FindControl("lblWorkflow");
            Label lblAttendance = (Label)e.Row.FindControl("lblAttendance");
            if (lblWorkflow != null)
            {
                switch (lblWorkflow.Text)
                {
                    case "A": lblWorkflow.Text = "Draft"; lblWorkflow.CssClass = "badge bg-secondary"; break;
                    case "B": lblWorkflow.Text = "Trainer Assigned"; lblWorkflow.CssClass = "badge bg-info"; break;
                    case "C": lblWorkflow.Text = "Sessions Created"; lblWorkflow.CssClass = "badge bg-primary"; break;
                    case "D": lblWorkflow.Text = "Trainees Assigned"; lblWorkflow.CssClass = "badge bg-warning"; break;
                    case "ABCDE": lblWorkflow.Text = "Training Started"; lblWorkflow.CssClass = "badge bg-success"; break;
                    case "ABCDEF": lblWorkflow.Text = "Attendance Completed"; lblWorkflow.CssClass = "badge bg-success"; break;
                    case "ABCDEFG": lblWorkflow.Text = "Pre Test Completed"; lblWorkflow.CssClass = "badge bg-success"; break;
                    case "ABCDEFGH": lblWorkflow.Text = "Post Test Completed"; lblWorkflow.CssClass = "badge bg-success"; break;
                    case "ABCDEFGHI": lblWorkflow.Text = "Feedback Submitted"; lblWorkflow.CssClass = "badge bg-success"; break;
                    case "ABCDEFGHIJ": lblWorkflow.Text = "Certificate Generated"; lblWorkflow.CssClass = "badge bg-success"; break;
                    default: lblWorkflow.CssClass = "badge bg-secondary"; break;
                }
            }
            if (lblAttendance != null)
            {
                string status = lblAttendance.Text.Trim();
                lblAttendance.CssClass = status.Equals("Completed", StringComparison.OrdinalIgnoreCase) ? "badge bg-success" : "badge bg-danger";
            }
        }
    }
}