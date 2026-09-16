using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class AllCategory : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            BindGrid();
        }
        private void BindGrid()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CategoryID,CategoryName,Remarks,CreatedOn FROM CourseCategoryMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvCategory.DataSource = dt;

            gvCategory.DataBind();
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
    }
}