using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class TraineeProgress : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) { BindTrainings(); BindGrid(); }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindTrainings()
        {
            string query = "SELECT DISTINCT TrainingID FROM SessionMaster WHERE TrainerID=@TrainerID ORDER BY TrainingID";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingID";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0, new ListItem("-- All Trainings --", ""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender, EventArgs e) => BindGrid();

        protected void btnSearch_Click(object sender, EventArgs e) => BindGrid();

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlTraining.SelectedIndex = 0;
            BindGrid();
        }

        private void BindGrid()
        {
            string query = @"SELECT E.EmpID, E.EmpName, COUNT(DISTINCT S.SessionID) AS TotalSessions, COUNT(SA.AttendanceID) AS Attended, 
                                    CASE WHEN COUNT(DISTINCT S.SessionID)=0 THEN 0 ELSE (COUNT(SA.AttendanceID)*100/COUNT(DISTINCT S.SessionID)) END AS Percentage 
                                    FROM EmpBasicMaster E 
                                    LEFT JOIN TrainingAssignment TA ON E.EmpID=TA.EmpID 
                                    LEFT JOIN SessionMaster S ON TA.TrainingID=S.TrainingID AND S.TrainerID=@TrainerID 
                                    LEFT JOIN SessionAttendance SA ON S.SessionID=SA.SessionID AND SA.EmpID=E.EmpID AND SA.AttendanceStatus='Present' 
                                    WHERE E.EmpID IN (SELECT DISTINCT EmpID FROM TrainingAssignment WHERE TrainingID IN (SELECT TrainingID FROM SessionMaster WHERE TrainerID=@TrainerID)) ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TrainerID", TrainerID));

            if (!string.IsNullOrEmpty(ddlTraining.SelectedValue))
            { query += " AND TA.TrainingID = @TrainingID"; parameters.Add(new SqlParameter("@TrainingID", ddlTraining.SelectedValue)); }

            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            { query += " AND (E.EmpID LIKE @Search OR E.EmpName LIKE @Search)"; parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%")); }

            query += " GROUP BY E.EmpID, E.EmpName ORDER BY Percentage DESC";
            DataTable dt = obj.GetDataTable(query, parameters.ToArray());
            gvProgress.DataSource = dt;
            gvProgress.DataBind();
        }
    }
}