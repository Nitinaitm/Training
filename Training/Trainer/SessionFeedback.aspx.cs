using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class SessionFeedback : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");

            if (!CheckFeedbackRequired())
            {
                return;
            }

            if (!IsPostBack) { BindSessions(); BindGrid(); }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private bool CheckFeedbackRequired()
        {
            string trainingID = Request.QueryString["TrainingID"];

            if (string.IsNullOrEmpty(trainingID))
            {
                return true;
            }

            string sql =
                "SELECT FeedbackRequired " +
                "FROM TrainingDetails " +
                "WHERE TrainingID=@TrainingID";

            SqlParameter[] parameter =
            {
                new SqlParameter("@TrainingID", trainingID)
            };

            object result = obj.ExecuteScalar(sql, parameter);

            if
            (
                result == null
                ||
                result == DBNull.Value
                ||
                !Convert.ToBoolean(result)
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "FeedbackRequired",
                    "alert('Feedback is not required for this training.');window.location='TrainingDetails.aspx';",
                    true);

                return false;
            }

            return true;
        }

        private void BindSessions()
        {
            string query = "SELECT SessionID, TrainingID + ' | S-' + CAST(SessionNo AS VARCHAR) + ' | ' + SessionName AS DisplayName FROM SessionMaster WHERE TrainerID=@TrainerID ORDER BY TRY_CONVERT(date,SessionDate,105) DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            ddlSession.DataSource = dt;
            ddlSession.DataTextField = "DisplayName";
            ddlSession.DataValueField = "SessionID";
            ddlSession.DataBind();
            ddlSession.Items.Insert(0, new ListItem("-- All Sessions --", ""));
        }

        protected void ddlSession_SelectedIndexChanged(object sender, EventArgs e) => BindGrid();

        protected void btnSearch_Click(object sender, EventArgs e) => BindGrid();

        private void BindGrid()
        {
            string query = @"SELECT FB.EmpID, E.EmpName, FB.Feedback, FB.Rating, FB.CreatedOn FROM SessionFeedback FB INNER JOIN EmpBasicMaster E ON FB.EmpID=E.EmpID WHERE FB.SessionID IN (SELECT SessionID FROM SessionMaster WHERE TrainerID=@TrainerID) ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TrainerID", TrainerID));

            if (!string.IsNullOrEmpty(ddlSession.SelectedValue))
            { query += " AND FB.SessionID = @SessionID"; parameters.Add(new SqlParameter("@SessionID", ddlSession.SelectedValue)); }

            if (!string.IsNullOrEmpty(txtFrom.Text.Trim()))
            { query += " AND TRY_CONVERT(date,FB.CreatedOn,105) >= @From"; parameters.Add(new SqlParameter("@From", Convert.ToDateTime(txtFrom.Text))); }

            if (!string.IsNullOrEmpty(txtTo.Text.Trim()))
            { query += " AND TRY_CONVERT(date,FB.CreatedOn,105) <= @To"; parameters.Add(new SqlParameter("@To", Convert.ToDateTime(txtTo.Text))); }

            query += " ORDER BY FB.CreatedOn DESC";
            DataTable dt = obj.GetDataTable(query, parameters.ToArray());
            gvFeedback.DataSource = dt;
            gvFeedback.DataBind();
        }

        protected string GetStars(object rating)
        {
            int r = Convert.ToInt32(rating);
            string stars = "";
            for (int i = 1; i <= 5; i++) { stars += (i <= r) ? "★" : "☆"; }
            return stars;
        }
    }
}