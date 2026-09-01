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
    public partial class AllCourses : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            BindGrid();
        }
        private void BindGrid()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CourseID,CourseName,CourseCategory,PassingPercentage,AttendancePercentage,CreatedOn FROM CourseMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvCourse.DataSource = dt;

            gvCourse.DataBind();
        }
        protected void txtSearch_TextChanged(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT CourseID,
CourseName,
CourseCategory,
PassingPercentage,
AttendancePercentage,
CreatedOn
FROM CourseMaster
WHERE CourseName LIKE @Search
OR CourseCategory LIKE @Search
ORDER BY CourseName", con);

            da.SelectCommand.Parameters.AddWithValue("@Search", "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvCourse.DataSource = dt;

            gvCourse.DataBind();
        }
        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"SELECT
CourseName AS [Course Name],
CourseCategory AS [Category],
PassingPercentage AS [Passing %],
AttendancePercentage AS [Attendance %],
Remarks,
CreatedOn AS [Created On]
FROM CourseMaster
ORDER BY CourseName", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Course Master");

                ws.Cells["A1"].LoadFromDataTable(dt, true);

                Response.Clear();

                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                Response.AddHeader("content-disposition", "attachment; filename=CourseMaster.xlsx");

                Response.BinaryWrite(pck.GetAsByteArray());

                Response.End();
            }
        }
    }
}