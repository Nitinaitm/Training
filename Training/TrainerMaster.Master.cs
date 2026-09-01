using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainer
{
    public partial class TrainerMaster : System.Web.UI.MasterPage
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainerID"] == null)
                {
                    // For demo - Remove in production
                    Session["TrainerID"] = "TRIN002";
                    Session["TrainerName"] = "Trainer";
                    // Response.Redirect("~/TrainerLogin.aspx");
                }

                LoadTrainerInfo();
            }
        }

        private void LoadTrainerInfo()
        {
            try
            {
                if (Session["TrainerID"] == null) return;

                string trainerID = Session["TrainerID"].ToString();

                string query = @"SELECT 
                                    TM.TrainerID, 
                                    CASE WHEN TM.TrainerType='Internal' THEN E.EmpName ELSE TM.NameExternal END AS TrainerName,
                                    CASE WHEN TM.TrainerType='Internal' THEN E.EmpDesignation ELSE TM.DesignationExternal END AS Designation
                                FROM TrainerMaster TM 
                                LEFT JOIN EmpBasicMaster E ON TM.EmpID = E.EmpID 
                                WHERE TM.TrainerID = @TrainerID";

                SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", trainerID) };
                DataTable dt = obj.GetDataTable(query, param);

                if (dt.Rows.Count > 0)
                {
                    DataRow dr = dt.Rows[0];
                    lblTrainerName.Text = dr["TrainerName"]?.ToString() ?? "Trainer";
                    lblDesignation.Text = dr["Designation"].ToString();
                    // Session["TrainerName"] = lblTrainerName.Text;
                }
                else
                {
                    lblTrainerName.Text = "Trainer";
                }
            }
            catch
            {
                lblTrainerName.Text = "Trainer";
            }
        }
    }
}