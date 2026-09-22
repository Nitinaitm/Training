using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Manager
{
    public partial class Default : Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!LoadManager())
                {
                    Response.Redirect("~/Default.aspx");
                    return;
                }
                BindTraining();
                BindSessions();
            }
        }

        private bool LoadManager()
        {
            if (Session["ManagerID"] == null || Session["ManagerEmpID"] == null)
            {
                return false;
            }

            string managerID = Session["ManagerID"].ToString();
            DataTable dt = objDB.GetDataTable("SELECT M.ManagerID,M.EmpID,E.EmpName,E.EmpDesignation,E.EmpPostingPlace,M.MapForLocation,M.TrainingLocationID,L.TrainingLocation FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID LEFT JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y'", new SqlParameter[] { new SqlParameter("@ManagerID", managerID) });

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            lblManagerID.Text = dt.Rows[0]["ManagerID"].ToString();
            lblEmpID.Text = dt.Rows[0]["EmpID"].ToString();
            lblName.Text = dt.Rows[0]["EmpName"].ToString();
            lblDesignation.Text = dt.Rows[0]["EmpDesignation"].ToString();
            lblPosting.Text = dt.Rows[0]["EmpPostingPlace"].ToString();
            lblMapForLocation.Text = dt.Rows[0]["MapForLocation"].ToString();
            lblTrainingLocation.Text = dt.Rows[0]["TrainingLocation"].ToString();
            Session["ManagerMapForLocation"] = dt.Rows[0]["MapForLocation"].ToString();
            Session["ManagerTrainingLocationID"] = dt.Rows[0]["TrainingLocationID"].ToString();
            return true;
        }

        private void BindTraining()
        {
            string mapForLocation = Session["ManagerMapForLocation"] == null ? "" : Session["ManagerMapForLocation"].ToString();
            DataTable dt = objDB.GetDataTable("SELECT TrainingID,TrainingType,TrainingOrganizer,TrainingLocation,Batch,DateFrom,DateTo,TrainingStatus FROM TrainingDetails WHERE TrainingLocation=@TrainingLocation AND ISNULL(TrainingStatus,'') NOT IN ('Completed','TrainingCompleted') ORDER BY TRY_CONVERT(date,DateFrom,105) DESC", new SqlParameter[] { new SqlParameter("@TrainingLocation", mapForLocation) });
            gvTraining.DataSource = dt;
            gvTraining.DataBind();
        }
    }
}