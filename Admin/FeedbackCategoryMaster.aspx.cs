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
    public partial class FeedbackCategoryMaster : System.Web.UI.Page
    {
        clsDataAccess objDB =
             new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearControls();

                BindGrid();
            }
        }

        private void BindGrid()
        {
            string query =
@"
SELECT
CategoryID,
CategoryName,
DisplayOrder,
Active,
CreatedOn
FROM
FeedbackCategoryMaster
WHERE
CategoryName LIKE @Search
ORDER BY
DisplayOrder,
CategoryName
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@Search",
                    "%" +
                    txtSearch.Text.Trim() +
                    "%")
            };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            gvCategory.DataSource =
                dt;

            gvCategory.DataBind();
        }
        //------------------------------------------------------
        // Generate Category ID
        //------------------------------------------------------

        private string GenerateCategoryID()
        {
            string query =
        @"
SELECT
ISNULL(MAX(ID),0)+1
FROM
FeedbackCategoryMaster
";

            int id =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query));

            return
                "FCAT" +
                id.ToString("0000");
        }

        //------------------------------------------------------
        // Clear Controls
        //------------------------------------------------------

        private void ClearControls()
        {
            hfCategoryID.Value =
                "";

            txtCategoryName.Text =
                "";

            txtDisplayOrder.Text =
                "1";

            chkActive.Checked =
                true;

            btnSave.Text =
                "Save";

            lblMessage.Text =
                "";
        }

        //------------------------------------------------------
        // Load Category
        //------------------------------------------------------

        private void LoadCategory(
            string categoryID)
        {
            string query =
        @"
SELECT
CategoryID,
CategoryName,
DisplayOrder,
Active
FROM
FeedbackCategoryMaster
WHERE
CategoryID=@CategoryID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryID",
            categoryID)
    };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            hfCategoryID.Value =
                dt.Rows[0]["CategoryID"]
                .ToString();

            txtCategoryName.Text =
                dt.Rows[0]["CategoryName"]
                .ToString();

            txtDisplayOrder.Text =
                dt.Rows[0]["DisplayOrder"]
                .ToString();

            chkActive.Checked =
                Convert.ToBoolean(
                dt.Rows[0]["Active"]);

            btnSave.Text =
                "Update";
        }

        //------------------------------------------------------
        // Search
        //------------------------------------------------------

        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }

        //------------------------------------------------------
        // Clear Button
        //------------------------------------------------------

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            ClearControls();

            BindGrid();
        }
        //------------------------------------------------------
        // Save
        //------------------------------------------------------

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            if (txtCategoryName.Text.Trim() == "")
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Please enter Category Name.";

                return;
            }

            if (IsDuplicateCategory())
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Category already exists.";

                return;
            }

            if (hfCategoryID.Value == "")
            {
                InsertCategory();
            }
            else
            {
                UpdateCategory();
            }

            ClearControls();

            BindGrid();
        }

        //------------------------------------------------------
        // Duplicate Validation
        //------------------------------------------------------

        private bool IsDuplicateCategory()
        {
            string query =
        @"
SELECT
COUNT(*)
FROM
FeedbackCategoryMaster
WHERE
CategoryName=@CategoryName
AND
CategoryID<>@CategoryID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryName",
            txtCategoryName.Text.Trim()),

        new SqlParameter(
            "@CategoryID",
            hfCategoryID.Value)
    };

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param));

            return count > 0;
        }

        //------------------------------------------------------
        // Insert
        //------------------------------------------------------

        private void InsertCategory()
        {
            string query =
        @"
INSERT INTO
FeedbackCategoryMaster
(
CategoryID,
CategoryName,
DisplayOrder,
Active,
CreatedOn
)
VALUES
(
@CategoryID,
@CategoryName,
@DisplayOrder,
@Active,
GETDATE()
)
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryID",
            GenerateCategoryID()),

        new SqlParameter(
            "@CategoryName",
            txtCategoryName.Text.Trim()),

        new SqlParameter(
            "@DisplayOrder",
            Convert.ToInt32(
            txtDisplayOrder.Text.Trim())),

        new SqlParameter(
            "@Active",
            chkActive.Checked)
    };

            objDB.ExecuteSql(
                query,
                param);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Category saved successfully.";
        }

        //------------------------------------------------------
        // Update
        //------------------------------------------------------

        private void UpdateCategory()
        {
            string query =
        @"
UPDATE
FeedbackCategoryMaster
SET
CategoryName=@CategoryName,
DisplayOrder=@DisplayOrder,
Active=@Active
WHERE
CategoryID=@CategoryID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryName",
            txtCategoryName.Text.Trim()),

        new SqlParameter(
            "@DisplayOrder",
            Convert.ToInt32(
            txtDisplayOrder.Text.Trim())),

        new SqlParameter(
            "@Active",
            chkActive.Checked),

        new SqlParameter(
            "@CategoryID",
            hfCategoryID.Value)
    };

            objDB.ExecuteSql(
                query,
                param);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Category updated successfully.";
        }
        //------------------------------------------------------
        // Grid Row Command
        //------------------------------------------------------

        protected void gvCategory_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                LoadCategory(
                    e.CommandArgument
                    .ToString());

                return;
            }

            if (e.CommandName == "DeleteRow")
            {
                DeleteCategory(
                    e.CommandArgument
                    .ToString());

                ClearControls();

                BindGrid();
            }
        }

        //------------------------------------------------------
        // Delete Category
        //------------------------------------------------------

        private void DeleteCategory(
            string categoryID)
        {
            string checkQuery =
        @"
SELECT
COUNT(*)
FROM
FeedbackQuestionMaster
WHERE
CategoryID=@CategoryID
";

            SqlParameter[] checkParam =
            {
        new SqlParameter(
            "@CategoryID",
            categoryID)
    };

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                checkQuery,
                checkParam));

            if (count > 0)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Category cannot be deleted because questions exist under this category.";

                return;
            }

            string deleteQuery =
        @"
DELETE
FROM
FeedbackCategoryMaster
WHERE
CategoryID=@CategoryID
";

            SqlParameter[] deleteParam =
            {
        new SqlParameter(
            "@CategoryID",
            categoryID)
    };

            objDB.ExecuteSql(
                deleteQuery,
                deleteParam);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Category deleted successfully.";
        }
    }
}