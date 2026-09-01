using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class MySessions : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) BindGrid();
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindGrid()
        {
            string query = @"SELECT SM.SessionID, SM.TrainingID, TD.Batch, SM.SessionNo, SM.SessionName, TM.TopicName, SM.SessionDate, SM.StartTime + ' - ' + SM.EndTime AS SessionTime, ISNULL(SM.AttendanceStatus,'Pending') AS AttendanceStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID = TD.TrainingID LEFT JOIN TopicMaster TM ON TM.TopicID = SM.TopicID WHERE SM.TrainerID = @TrainerID AND TD.WorkflowStatus LIKE '%C%' ORDER BY TRY_CONVERT(date, SM.SessionDate, 105), CAST(SM.SessionNo AS INT)";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            gvSessions.DataSource = dt;
            gvSessions.DataBind();
        }

        protected void gvSessions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ViewSession")
            {
                string sessionID = e.CommandArgument.ToString();
                Session["SessionID"] = sessionID;
                Response.Redirect("~/Trainer/SessionDetails.aspx");
            }
        }

        protected void gvSessions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Button btn = (Button)e.Row.FindControl("btnAction");
                Label lbl = (Label)e.Row.FindControl("lblAttendance");
                if (btn != null && lbl != null)
                {
                    btn.Text = lbl.Text == "Completed" ? "View" : "Take Attendance";
                    btn.CssClass = lbl.Text == "Completed" ? "btn btn-info btn-sm btn-action" : "btn btn-success btn-sm btn-action";
                }
            }
        }
    }
}