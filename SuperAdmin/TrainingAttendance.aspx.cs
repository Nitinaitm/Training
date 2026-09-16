using OfficeOpenXml;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class TrainingAttendance : System.Web.UI.Page
    {
        string constr =
        ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode =
            UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                if (
                   Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                BindTraining();

                mvAttendance.ActiveViewIndex = 0;

                LoadPlugins();
            }
        }

        private void BindTraining()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string q = @"

                SELECT DISTINCT
                T.TrainingID,
                T.TrainingID + ' | ' +
                TD.TrainingType + ' | ' +
                TD.Batch AS TrainingName

                FROM TrainingAssignment T

                INNER JOIN TrainingDetails TD
                ON T.TrainingID = TD.TrainingID

                ORDER BY T.TrainingID DESC";

                SqlDataAdapter da =
                new SqlDataAdapter(q, con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlTraining.DataSource = dt;
                ddlTraining.DataTextField = "TrainingName";
                ddlTraining.DataValueField = "TrainingID";
                ddlTraining.DataBind();

                ddlBulkTraining.DataSource = dt.Copy();
                ddlBulkTraining.DataTextField = "TrainingName";
                ddlBulkTraining.DataValueField = "TrainingID";
                ddlBulkTraining.DataBind();
            }

            ddlTraining.Items.Insert(
            0,
            new ListItem(
            "Select Training", ""));

            ddlBulkTraining.Items.Insert(
            0,
            new ListItem(
            "Select Training", ""));
        }

        protected void btnManualTab_Click(
            object sender,
            EventArgs e)
        {
            mvAttendance.ActiveViewIndex = 0;

            LoadPlugins();
        }

        protected void btnBulkTab_Click(
            object sender,
            EventArgs e)
        {
            mvAttendance.ActiveViewIndex = 1;

            LoadPlugins();
        }

        protected void ddlTraining_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            mvAttendance.ActiveViewIndex = 0;

            lblMessage.Text = "";
            lblBulkMessage.Text = "";

            BindGrid();

            LoadPlugins();
        }

        protected void ddlBulkTraining_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            mvAttendance.ActiveViewIndex = 1;

            lblMessage.Text = "";
            lblBulkMessage.Text = "";

            LoadPlugins();
        }

        private void BindGrid()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string q = @"

                SELECT

                A.AssignmentID,
                A.EmpID,
                B.EmpName,
                B.EmpDesignation,
                ISNULL(
                A.TrainingAttended,'')
                AS TrainingAttended

                FROM TrainingAssignment A

                INNER JOIN EmpBasicMaster B
                ON A.EmpID=B.EmpID

                WHERE A.TrainingID=@TrainingID

                ORDER BY B.EmpName";

                SqlDataAdapter da =
                new SqlDataAdapter(q, con);

                da.SelectCommand.Parameters
                .AddWithValue(
                "@TrainingID",
                ddlTraining.SelectedValue);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvAttendance.DataSource = dt;
                gvAttendance.DataBind();
            }
        }

        protected void gvAttendance_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType ==
                DataControlRowType.DataRow)
            {
                DropDownList ddl =
                (DropDownList)e.Row
                .FindControl("ddlStatus");

                string status =
                DataBinder.Eval(
                e.Row.DataItem,
                "TrainingAttended")
                .ToString();

                if (ddl.Items.FindByValue(status) != null)
                {
                    ddl.SelectedValue = status;
                }
            }
        }

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                mvAttendance.ActiveViewIndex = 0;

                using (SqlConnection con =
                    new SqlConnection(constr))
                {
                    con.Open();

                    foreach (GridViewRow row
                        in gvAttendance.Rows)
                    {
                        string assignmentID =
                        gvAttendance.DataKeys[
                        row.RowIndex]
                        .Value.ToString();

                        DropDownList ddl =
                        (DropDownList)
                        row.FindControl(
                        "ddlStatus");

                        string q = @"

                        UPDATE TrainingAssignment

                        SET TrainingAttended=
                        @TrainingAttended

                        WHERE AssignmentID=
                        @AssignmentID";

                        SqlCommand cmd =
                        new SqlCommand(q, con);

                        cmd.Parameters
                        .AddWithValue(
                        "@TrainingAttended",
                        ddl.SelectedValue);

                        cmd.Parameters
                        .AddWithValue(
                        "@AssignmentID",
                        assignmentID);

                        cmd.ExecuteNonQuery();
                    }

                    con.Close();
                }

                lblMessage.Text =
                "Attendance updated successfully.";

                lblMessage.ForeColor =
                Color.Green;

                BindGrid();
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                ex.Message;

                lblMessage.ForeColor =
                Color.Red;
            }
        }

        protected void btnBulkAttendance_Click(
            object sender,
            EventArgs e)
        {
            mvAttendance.ActiveViewIndex = 1;

            lblBulkMessage.Text = "";

            try
            {
                if (ddlBulkTraining.SelectedIndex == 0)
                {
                    lblBulkMessage.Text =
                    "Please select training.";

                    lblBulkMessage.ForeColor =
                    Color.Red;

                    return;
                }

                if (!fuAttendance.HasFile)
                {
                    lblBulkMessage.Text =
                    "Please select Excel file.";

                    lblBulkMessage.ForeColor =
                    Color.Red;

                    return;
                }

                string ext =
                Path.GetExtension(
                fuAttendance.FileName)
                .ToLower();

                if (ext != ".xlsx")
                {
                    lblBulkMessage.Text =
                    "Only .xlsx file allowed.";

                    lblBulkMessage.ForeColor =
                    Color.Red;

                    return;
                }

                ExcelPackage.LicenseContext =
                OfficeOpenXml.LicenseContext.NonCommercial;

                using (var package =
                    new ExcelPackage(
                    fuAttendance.FileContent))
                {
                    var ws =
                    package.Workbook.Worksheets[0];

                    int rows =
                    ws.Dimension.Rows;

                    int updated = 0;

                    using (SqlConnection con =
                    new SqlConnection(constr))
                    {
                        con.Open();

                        for (int i = 2;
                             i <= rows;
                             i++)
                        {
                            string empid =
                            ws.Cells[i, 1]
                            .Text.Trim();

                            string attended =
                            ws.Cells[i, 2]
                            .Text.Trim();

                            if (string.IsNullOrWhiteSpace(empid))
                                continue;

                            if (attended != "Yes"
                                && attended != "No")
                                continue;

                            string q = @"

                            UPDATE TrainingAssignment

                            SET TrainingAttended=@Attended

                            WHERE EmpID=@EmpID
                            AND TrainingID=@TrainingID";

                            SqlCommand cmd =
                            new SqlCommand(q, con);

                            cmd.Parameters
                            .AddWithValue(
                            "@Attended",
                            attended);

                            cmd.Parameters
                            .AddWithValue(
                            "@EmpID",
                            empid);

                            cmd.Parameters
                            .AddWithValue(
                            "@TrainingID",
                            ddlBulkTraining.SelectedValue);

                            int x =
                            cmd.ExecuteNonQuery();

                            if (x > 0)
                                updated++;
                        }

                        con.Close();
                    }

                    lblBulkMessage.Text =
                    updated +
                    " record(s) updated successfully.";

                    lblBulkMessage.ForeColor =
                    Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblBulkMessage.Text =
                ex.Message;

                lblBulkMessage.ForeColor =
                Color.Red;
            }

            LoadPlugins();
        }

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "plugins",

            "$('#ddlTraining').select2({width:'100%'});" +
            "$('#ddlBulkTraining').select2({width:'100%'});",

            true);
        }
    }
}