using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using OfficeOpenXml;

namespace Training.Admin
{
    public partial class CourseCategoryMaster : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void GenerateCategoryID()
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(ID),0)+1 FROM CourseCategoryMaster", con);

            con.Open();

            int NextID = Convert.ToInt32(cmd.ExecuteScalar());

            con.Close();


        }

      

        private void BindGrid()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CategoryID,CategoryName,Remarks, CreatedOn FROM CourseCategoryMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvCategory.DataSource = dt;

            gvCategory.DataBind();
        }

        private void ClearControls()
        {
            txtCategoryName.Text = "";

          
            txtRemarks.Text = "";

            lblMessage.Text = "";

            ViewState["CategoryID"] = null;

            btnSave.Visible = true;

            btnUpdate.Visible = false;

        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
           
            string CategoryID = "";

            SqlCommand cmdID = new SqlCommand("SELECT 'CR'+RIGHT('000000'+CAST(ISNULL(MAX(ID),0)+1 AS VARCHAR(6)),6) FROM CourseCategoryMaster", con);

            con.Open();

            CategoryID = Convert.ToString(cmdID.ExecuteScalar());

            con.Close();

            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM CourseCategoryMaster WHERE CategoryName=@CategoryName", con);

            cmdCheck.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());

            con.Open();

            int Count = Convert.ToInt32(cmdCheck.ExecuteScalar());

            con.Close();

            if (Count > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Category Name already exists.";

                return;
            }

            SqlCommand cmd = new SqlCommand(@"INSERT INTO CourseCategoryMaster
    (
        CategoryID,
        CategoryName,
        
        Remarks,
        CreatedBy
    )
    VALUES
    (
        @CategoryID,
        @CategoryName,
       
        @Remarks,
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@CategoryID", CategoryID);

            cmd.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());

          

            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

            cmd.Parameters.AddWithValue("@CreatedBy", Session["UserID"] == null ? "" : Session["UserID"].ToString());
            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Category saved successfully.";

            ClearControls();

            BindGrid();
        }
        protected void gvCategory_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                LoadCourse(e.CommandArgument.ToString());
            }

            if (e.CommandName == "DeleteRecord")
            {
                DeleteCourse(e.CommandArgument.ToString());
            }
        }
        private void LoadCourse(string CategoryID)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM CourseCategoryMaster WHERE CategoryID=@CategoryID", con);

            da.SelectCommand.Parameters.AddWithValue("@CategoryID", CategoryID);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                ViewState["CategoryID"] = CategoryID;

                txtCategoryName.Text = dt.Rows[0]["CategoryName"].ToString();

             

                txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();

                btnSave.Visible = false;

                btnUpdate.Visible = true;

            }
        }
        private void DeleteCourse(string CategoryID)
        {
            SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM CourseMaster WHERE CourseCategory=@CourseCategory", con);

            chk.Parameters.AddWithValue("@CourseCategory", CategoryID);

            con.Open();

            int Used = Convert.ToInt32(chk.ExecuteScalar());

            con.Close();

            if (Used > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Category is already assigned.";

                return;
            }

            SqlCommand cmd = new SqlCommand("DELETE FROM CourseCategoryMaster WHERE CategoryID=@CategoryID", con);

            cmd.Parameters.AddWithValue("@CategoryID", CategoryID);

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Deleted Successfully.";

            ClearControls();

            BindGrid();
        }
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM CourseCategoryMaster WHERE CategoryName=@CategoryName AND CategoryID<>@CategoryID", con);

            chk.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());

            chk.Parameters.AddWithValue("@CategoryID", ViewState["CategoryID"].ToString());

            con.Open();

            int cnt = Convert.ToInt32(chk.ExecuteScalar());

            con.Close();

            if (cnt > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Category Name already exists.";

                return;
            }

            SqlCommand cmd = new SqlCommand(@"UPDATE CourseCategoryMaster SET
CategoryName=@CategoryName,

Remarks=@Remarks
WHERE CategoryID=@CategoryID", con);

            cmd.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());

           

            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

            cmd.Parameters.AddWithValue("@CategoryID", ViewState["CategoryID"].ToString());

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Category updated successfully.";

            ClearControls();

            BindGrid();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {

            SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM CourseMaster WHERE CourseCategory=@CourseCategory", con);

            if (ViewState["CourseCategory"] == null)
            {
                lblMessage.Text = "Please select a category first.";
                return;
            }

            chk.Parameters.AddWithValue("@CourseCategory", ViewState["CourseCategory"].ToString());

            con.Open();

            object result = chk.ExecuteScalar();

            con.Close();

            int Used = (result == null || result == DBNull.Value) ? 0 : Convert.ToInt32(result);

            con.Close();

            if (Used > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "CourseCategory is already assigned to Batch. Delete not allowed.";

                return;
            }


            SqlCommand cmd = new SqlCommand("DELETE FROM CourseCategoryMaster WHERE CategoryID=@CategoryID", con);

            cmd.Parameters.AddWithValue("@CategoryID", ViewState["CategoryID"].ToString());

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Category deleted successfully.";

            ClearControls();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
        }

        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT CategoryID,
CategoryName,
Remarks,
CreatedOn
FROM CourseCategoryMaster
WHERE CategoryName LIKE @Search

ORDER BY CategoryName", con);

            da.SelectCommand.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvCategory.DataSource = dt;

            gvCategory.DataBind();
        }

        protected void gvCategory_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCategory.PageIndex = e.NewPageIndex;

            BindGrid();
        }

        protected void gvCategory_Sorting(object sender, GridViewSortEventArgs e)
        {
            BindGrid();
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT
CategoryName AS [Category Name],

Remarks,
CreatedOn AS [Created On]
FROM CourseCategoryMaster
ORDER BY CategoryName", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Category Master");

                ws.Cells["A1"].LoadFromDataTable(dt, true);

                Response.Clear();

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                Response.AddHeader("content-disposition", "attachment; filename=CategoryMaster.xlsx");

                Response.BinaryWrite(pck.GetAsByteArray());

                Response.End();
            }
        }
        private bool ValidateCategory()
        {
            if (txtCategoryName.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Enter Category Name.";

                txtCategoryName.Focus();

                return false;
            }

            

            return true;
        }
    }
}