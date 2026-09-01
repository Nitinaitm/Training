using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training
{
    public partial class TraineeMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ////if (Session["TraineeID"] == null)
                ////{
                ////    // For demo - Remove in production
                ////    //Session["TraineeID"] = "TRIN002";
                ////    //Session["TraineeName"] = "Trainee";
                ////     Response.Redirect("~/Default.aspx");
                ////}

                ////LoadTrainerInfo();
            }
        }

        private void LoadTrainerInfo()
        {
            try
            {
                if (Session["TraineeID"] == null) return;

                string traineeID = Session["TraineeID"].ToString();

                //string query = @"SELECT 
                //                    TM.TrainerID, 
                //                    CASE WHEN TM.TrainerType='Internal' THEN E.EmpName ELSE TM.NameExternal END AS TrainerName,
                //                    CASE WHEN TM.TrainerType='Internal' THEN E.EmpDesignation ELSE TM.DesignationExternal END AS Designation
                //                FROM TrainerMaster TM 
                //                LEFT JOIN EmpBasicMaster E ON TM.EmpID = E.EmpID 
                //                WHERE TM.TrainerID = @TrainerID";

                //SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", trainerID) };
                //DataTable dt = obj.GetDataTable(query, param);

                //if (dt.Rows.Count > 0)
                //{
                //    DataRow dr = dt.Rows[0];
                //    lblTrainerName.Text = dr["TrainerName"]?.ToString() ?? "Trainer";
                //    lblDesignation.Text = dr["Designation"].ToString();
                //    // Session["TrainerName"] = lblTrainerName.Text;
                //}
                //else
                //{
                //    lblTrainerName.Text = "Trainer";
                //}
            }
            catch
            {
                //lblTrainerName.Text = "Trainer";
            }
        }
    }
}