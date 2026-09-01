using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class AssignFeedback : System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        //private string TrainingID
        //{
        //    get
        //    {
        //        return Session["TrainingID"] == null
        //            ? ""
        //            : Session["TrainingID"].ToString();
        //    }
        //}

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainingID"] == null)
                {
                    Response.Redirect("TrainingList.aspx");
                    return;
                }

                ucTrainingSummary.LoadTraining(Session["TrainingID"].ToString());


                BindCategories();

                LoadAssignedCategories();

            }
        }

        //------------------------------------------------------
        // Bind Feedback Categories
        //------------------------------------------------------

        private void BindCategories()
        {
            string query =
@"
SELECT
CategoryID,
CategoryName
FROM
FeedbackCategoryMaster
WHERE
Active=1
ORDER BY
DisplayOrder,
CategoryName
";

            DataTable dt =
                objDB.GetDataTable(
                query);

            chkCategory.DataSource =
                dt;

            chkCategory.DataTextField =
                "CategoryName";

            chkCategory.DataValueField =
                "CategoryID";

            chkCategory.DataBind();
        }

        //------------------------------------------------------
        // Load Already Assigned Categories
        //------------------------------------------------------

        private void LoadAssignedCategories()
        {
            string query =
@"
SELECT
CategoryID
FROM
TrainingFeedbackCategory
WHERE
TrainingID=@TrainingID
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    Session["TrainingID"].ToString())
            };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            foreach (DataRow dr in dt.Rows)
            {
                ListItem item =
                    chkCategory.Items.FindByValue(
                    dr["CategoryID"].ToString());

                if (item != null)
                {
                    item.Selected =
                        true;
                }
            }
        }

        //------------------------------------------------------
        // Clear Selection
        //------------------------------------------------------

        private void ClearSelection()
        {
            foreach (ListItem item in chkCategory.Items)
            {
                item.Selected =
                    false;
            }
        }

        //------------------------------------------------------
        // Generate Mapping ID
        //------------------------------------------------------

        private string GenerateMappingID()
        {
            string query =
@"
SELECT
ISNULL(MAX(ID),0)+1
FROM
TrainingFeedbackCategory
";

            int id =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query));

            return
                "TFM" +
                id.ToString("0000");
        }
        //------------------------------------------------------
        // Save
        //------------------------------------------------------

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            bool isSelected =
                false;

            foreach (ListItem item in chkCategory.Items)
            {
                if (item.Selected)
                {
                    isSelected =
                        true;

                    break;
                }
            }

            if (!isSelected)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Please select at least one Feedback Category.";

                return;
            }

            DeleteAssignedCategories();

            foreach (ListItem item in chkCategory.Items)
            {
                if (item.Selected)
                {
                    InsertCategory(
                        item.Value);
                }
            }

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Feedback Categories assigned successfully.";
        }

        //------------------------------------------------------
        // Delete Existing Mapping
        //------------------------------------------------------

        private void DeleteAssignedCategories()
        {
            string query =
        @"
DELETE
FROM
TrainingFeedbackCategory
WHERE
TrainingID=@TrainingID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"].ToString())
    };

            objDB.ExecuteSql(
                query,
                param);
        }

        //------------------------------------------------------
        // Insert Category
        //------------------------------------------------------

        private void InsertCategory(
            string categoryID)
        {
            string query =
        @"
INSERT INTO
TrainingFeedbackCategory
(
MappingID,
TrainingID,
CategoryID,
CreatedOn,
CreatedBy
)
VALUES
(
@MappingID,
@TrainingID,
@CategoryID,
GETDATE(),
@CreatedBy
)
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@MappingID",
            GenerateMappingID()),

        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"].ToString()),

        new SqlParameter(
            "@CategoryID",
            categoryID),

        new SqlParameter(
            "@CreatedBy",
            Session["EmpID"] == null
            ? ""
            : Session["EmpID"].ToString().ToUpperInvariant())
    };

            objDB.ExecuteSql(
                query,
                param);
        }
        //------------------------------------------------------
        // Back
        //------------------------------------------------------

        protected void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect("ManageTraining.aspx");

            Context.ApplicationInstance.CompleteRequest();
        }
    }
}