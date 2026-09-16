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
            if (Session["TrainerID"] == null || string.IsNullOrWhiteSpace(Session["TrainerID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindTrainings();
                BindGrid();
            }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindTrainings()
        {
            string query = @"SELECT DISTINCT TrainingID
                             FROM SessionMaster
                             WHERE TrainerID=@TrainerID
                             ORDER BY TrainingID";

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@TrainerID", TrainerID)
            };

            DataTable dt = obj.GetDataTable(query, param);

            ddlTraining.DataSource = dt;
            ddlTraining.DataTextField = "TrainingID";
            ddlTraining.DataValueField = "TrainingID";
            ddlTraining.DataBind();
            ddlTraining.Items.Insert(0, new ListItem("-- All Trainings --", ""));
        }

        protected void ddlTraining_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlTraining.SelectedIndex = 0;
            BindGrid();
        }

        private void BindGrid()
        {
            string query = @"SELECT
                                E.EmpID,
                                E.EmpName,
                                COUNT(DISTINCT S.SessionID) AS TotalSessions,
                                COUNT(DISTINCT CASE
                                    WHEN SA.AttendanceStatus='Present' THEN S.SessionID
                                END) AS Attended,
                                CASE
                                    WHEN COUNT(DISTINCT S.SessionID)=0 THEN 0
                                    ELSE CAST(COUNT(DISTINCT CASE
                                        WHEN SA.AttendanceStatus='Present' THEN S.SessionID
                                    END) * 100.0 / COUNT(DISTINCT S.SessionID) AS decimal(10,2))
                                END AS Percentage
                            FROM EmpBasicMaster E
                            INNER JOIN TrainingAssignment TA
                                ON E.EmpID=TA.EmpID
                            INNER JOIN SessionMaster S
                                ON TA.TrainingID=S.TrainingID
                               AND S.TrainerID=@TrainerID
                            LEFT JOIN SessionAttendance SA
                                ON S.SessionID=SA.SessionID
                               AND SA.EmpID=E.EmpID
                            WHERE EXISTS
                            (
                                SELECT 1
                                FROM TrainingAssignment TA2
                                INNER JOIN SessionMaster SM2
                                    ON TA2.TrainingID=SM2.TrainingID
                                   AND SM2.TrainerID=@TrainerID
                                WHERE TA2.EmpID=E.EmpID
                            )";

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TrainerID", TrainerID));

            if (!string.IsNullOrEmpty(ddlTraining.SelectedValue))
            {
                query += " AND TA.TrainingID=@TrainingID";
                parameters.Add(new SqlParameter("@TrainingID", ddlTraining.SelectedValue));
            }

            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                query += " AND (E.EmpID LIKE @Search OR E.EmpName LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%"));
            }

            query += " GROUP BY E.EmpID, E.EmpName ORDER BY Percentage DESC, E.EmpName";

            DataTable dt = obj.GetDataTable(query, parameters.ToArray());
            gvProgress.DataSource = dt;
            gvProgress.DataBind();
        }
    }
}