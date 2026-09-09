using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class AssignFeedback : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainingID"] == null) { Response.Redirect("TrainingList.aspx"); return; }
                ucTrainingSummary.LoadTraining(Session["TrainingID"].ToString());
                BindCategories();
                LoadAssignedCategories();
                btnNext.Visible = HasAssignedCategories();
            }
        }

        private string TrainingID { get { return Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString(); } }

        private void BindCategories()
        {
            DataTable dt = objDB.GetDataTable(@"SELECT CategoryID,CategoryName FROM FeedbackCategoryMaster WHERE Active=1 ORDER BY DisplayOrder,CategoryName");
            chkCategory.DataSource = dt;
            chkCategory.DataTextField = "CategoryName";
            chkCategory.DataValueField = "CategoryID";
            chkCategory.DataBind();
        }

        private void LoadAssignedCategories()
        {
            DataTable dt = objDB.GetDataTable("SELECT CategoryID FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            foreach (DataRow dr in dt.Rows)
            {
                ListItem item = chkCategory.Items.FindByValue(dr["CategoryID"].ToString());
                if (item != null) item.Selected = true;
            }
        }

        private bool HasAssignedCategories()
        {
            return Convert.ToInt32(objDB.ExecuteScalar("SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) })) > 0;
        }

        private string GenerateMappingID()
        {
            int id = Convert.ToInt32(objDB.ExecuteScalar("SELECT ISNULL(MAX(ID),0)+1 FROM TrainingFeedbackCategory"));
            return "TFM" + id.ToString("0000");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            bool selected = false;
            foreach (ListItem item in chkCategory.Items) if (item.Selected) { selected = true; break; }
            if (!selected) { lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Please select at least one Feedback Category."; btnNext.Visible = false; return; }

            objDB.ExecuteSql("DELETE FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            foreach (ListItem item in chkCategory.Items)
            {
                if (!item.Selected) continue;
                objDB.ExecuteSql(@"INSERT INTO TrainingFeedbackCategory(MappingID,TrainingID,CategoryID,CreatedOn,CreatedBy) VALUES(@MappingID,@TrainingID,@CategoryID,GETDATE(),@CreatedBy)", new SqlParameter[]
                {
                    new SqlParameter("@MappingID", GenerateMappingID()),
                    new SqlParameter("@TrainingID", TrainingID),
                    new SqlParameter("@CategoryID", item.Value),
                    new SqlParameter("@CreatedBy", Session["EmpID"] == null ? "" : Session["EmpID"].ToString().ToUpperInvariant())
                });
            }
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Feedback Categories assigned successfully.";
            btnNext.Visible = true;
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageTraining.aspx");
            Context.ApplicationInstance.CompleteRequest();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageTraining.aspx");
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
