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
    public partial class CourseMaster : System.Web.UI.Page
    {

        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCourseCategory();
                BindGrid();
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ScriptManager.RegisterStartupScript(this, GetType(), "ddl", "$('#ddlCourseCategory').select2();", true);
        }

        private void GenerateCourseID()
        {
            SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(ID),0)+1 FROM CourseMaster", con);

            con.Open();

            int NextID = Convert.ToInt32(cmd.ExecuteScalar());

            con.Close();

            
        }

        private void BindCourseCategory()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CategoryName FROM CourseCategoryMaster ORDER BY CategoryName", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            ddlCourseCategory.DataSource = dt;

            ddlCourseCategory.DataTextField = "CategoryName";

            ddlCourseCategory.DataValueField = "CategoryName";

            ddlCourseCategory.DataBind();

            ddlCourseCategory.Items.Insert(0, new ListItem("-- Select Category --", ""));
        }

        private void BindGrid()
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT CourseID,CourseName,CourseCategory,PassingPercentage,AttendancePercentage,CreatedOn FROM CourseMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvCourse.DataSource = dt;

            gvCourse.DataBind();
        }

        private void ClearControls()
        {
            txtCourseName.Text = "";

            ddlCourseCategory.SelectedIndex = 0;

            txtCourseDescription.Text = "";

            txtPassingPercentage.Text = "50";

            txtAttendancePercentage.Text = "90";

            txtRemarks.Text = "";

            lblMessage.Text = "";

            ViewState["CourseID"] = null;

            btnSave.Visible = true;

            btnUpdate.Visible = false;

        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateCourse())
            {
                return;
            }
            string CourseID = "";

            SqlCommand cmdID = new SqlCommand("SELECT 'CR'+RIGHT('000000'+CAST(ISNULL(MAX(ID),0)+1 AS VARCHAR(6)),6) FROM CourseMaster", con);

            con.Open();

            CourseID = Convert.ToString(cmdID.ExecuteScalar());

            con.Close();

            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM CourseMaster WHERE CourseName=@CourseName", con);

            cmdCheck.Parameters.AddWithValue("@CourseName", txtCourseName.Text.Trim());

            con.Open();

            int Count = Convert.ToInt32(cmdCheck.ExecuteScalar());

            con.Close();

            if (Count > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Course Name already exists.";

                return;
            }

            SqlCommand cmd = new SqlCommand(@"INSERT INTO CourseMaster
    (
        CourseID,
        CourseName,
        CourseDescription,
        CourseCategory,
        PassingPercentage,
        AttendancePercentage,
        Remarks,
        CreatedBy
    )
    VALUES
    (
        @CourseID,
        @CourseName,
        @CourseDescription,
        @CourseCategory,
        @PassingPercentage,
        @AttendancePercentage,
        @Remarks,
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@CourseID", CourseID);

            cmd.Parameters.AddWithValue("@CourseName", txtCourseName.Text.Trim());

            cmd.Parameters.AddWithValue("@CourseDescription", txtCourseDescription.Text.Trim());

            cmd.Parameters.AddWithValue("@CourseCategory", ddlCourseCategory.SelectedValue);

            cmd.Parameters.AddWithValue("@PassingPercentage", Convert.ToDecimal(txtPassingPercentage.Text.Trim()));

            cmd.Parameters.AddWithValue("@AttendancePercentage", Convert.ToDecimal(txtAttendancePercentage.Text.Trim()));

            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

            cmd.Parameters.AddWithValue("@CreatedBy", Session["UserID"] == null ? "" : Session["UserID"].ToString());
            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Course saved successfully.";

            ClearControls();

            BindGrid();
        }
        protected void gvCourse_RowCommand(object sender, GridViewCommandEventArgs e)
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
        private void LoadCourse(string CourseID)
        {
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM CourseMaster WHERE CourseID=@CourseID", con);

            da.SelectCommand.Parameters.AddWithValue("@CourseID", CourseID);

            DataTable dt = new DataTable();

            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                ViewState["CourseID"] = CourseID;

                txtCourseName.Text = dt.Rows[0]["CourseName"].ToString();

                txtCourseDescription.Text = dt.Rows[0]["CourseDescription"].ToString();

                ddlCourseCategory.SelectedValue = dt.Rows[0]["CourseCategory"].ToString();

                txtPassingPercentage.Text = dt.Rows[0]["PassingPercentage"].ToString();

                txtAttendancePercentage.Text = dt.Rows[0]["AttendancePercentage"].ToString();

                txtRemarks.Text = dt.Rows[0]["Remarks"].ToString();

                btnSave.Visible = false;

                btnUpdate.Visible = true;

            }
        }
        private void DeleteCourse(string CourseID)
        {
            SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM TrainingDetails WHERE CourseID=@CourseID", con);

            chk.Parameters.AddWithValue("@CourseID", CourseID);

            con.Open();

            int Used = Convert.ToInt32(chk.ExecuteScalar());

            con.Close();

            if (Used > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Course is already assigned.";

                return;
            }

            SqlCommand cmd = new SqlCommand("DELETE FROM CourseMaster WHERE CourseID=@CourseID", con);

            cmd.Parameters.AddWithValue("@CourseID", CourseID);

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
            SqlCommand chk = new SqlCommand("SELECT COUNT(*) FROM CourseMaster WHERE CourseName=@CourseName AND CourseID<>@CourseID", con);

            chk.Parameters.AddWithValue("@CourseName", txtCourseName.Text.Trim());

            chk.Parameters.AddWithValue("@CourseID", ViewState["CourseID"].ToString());

            con.Open();

            int cnt = Convert.ToInt32(chk.ExecuteScalar());

            con.Close();

            if (cnt > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Course Name already exists.";

                return;
            }

            SqlCommand cmd = new SqlCommand(@"UPDATE CourseMaster SET
CourseName=@CourseName,
CourseDescription=@CourseDescription,
CourseCategory=@CourseCategory,
PassingPercentage=@PassingPercentage,
AttendancePercentage=@AttendancePercentage,
Remarks=@Remarks
WHERE CourseID=@CourseID", con);

            cmd.Parameters.AddWithValue("@CourseName", txtCourseName.Text.Trim());

            cmd.Parameters.AddWithValue("@CourseDescription", txtCourseDescription.Text.Trim());

            cmd.Parameters.AddWithValue("@CourseCategory", ddlCourseCategory.SelectedValue);

            cmd.Parameters.AddWithValue("@PassingPercentage", Convert.ToDecimal(txtPassingPercentage.Text));

            cmd.Parameters.AddWithValue("@AttendancePercentage", Convert.ToDecimal(txtAttendancePercentage.Text));

            cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

            cmd.Parameters.AddWithValue("@CourseID", ViewState["CourseID"].ToString());

            con.Open();

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Course updated successfully.";

            ClearControls();

            BindGrid();
        }

      

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
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

        protected void gvCourse_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCourse.PageIndex = e.NewPageIndex;

            BindGrid();
        }

        protected void gvCourse_Sorting(object sender, GridViewSortEventArgs e)
        {
            BindGrid();
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
        private bool ValidateCourse()
        {
            if (txtCourseName.Text.Trim() == "")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Enter Course Name.";

                txtCourseName.Focus();

                return false;
            }

            if (ddlCourseCategory.SelectedIndex == 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Select Course Category.";

                ddlCourseCategory.Focus();

                return false;
            }

            decimal Passing;

            if (!decimal.TryParse(txtPassingPercentage.Text.Trim(), out Passing))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Invalid Passing Percentage.";

                txtPassingPercentage.Focus();

                return false;
            }

            if (Passing < 0 || Passing > 100)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Passing Percentage must be between 0 and 100.";

                txtPassingPercentage.Focus();

                return false;
            }

            decimal Attendance;

            if (!decimal.TryParse(txtAttendancePercentage.Text.Trim(), out Attendance))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Invalid Attendance Percentage.";

                txtAttendancePercentage.Focus();

                return false;
            }

            if (Attendance < 0 || Attendance > 100)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Attendance Percentage must be between 0 and 100.";

                txtAttendancePercentage.Focus();

                return false;
            }

            return true;
        }
    }
}