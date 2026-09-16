using OfficeOpenXml;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
namespace Training.SuperAdmin
{
    public partial class EmpBasicMaster : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                if (
                   Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                BindCompany();
                BindDesignation();
                BindPostingPlace();

                LoadSelect2();
            }
        }

        #region LOAD SELECT2

        private void LoadSelect2()
        {
            ScriptManager.RegisterStartupScript(
                this,
                this.GetType(),
                "LoadSearchableDropdown",
                "LoadSearchableDropdown();",
                true
            );
        }

        #endregion

        #region SAVE SINGLE ENTRY

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblSingleMessage.Text = "";
            lblBulkMessage.Text = "";

            try
            {
                if (!Page.IsValid)
                {
                    LoadSelect2();
                    return;
                }

                DateTime dob;
                DateTime doj;

                if (!DateTime.TryParse(txtDOB.Text.Trim(), out dob))
                {
                    lblSingleMessage.Text = "Invalid DOB.";
                    lblSingleMessage.ForeColor = Color.Red;
                    return;
                }

                if (!DateTime.TryParse(txtDOJ.Text.Trim(), out doj))
                {
                    lblSingleMessage.Text = "Invalid DOJ.";
                    lblSingleMessage.ForeColor = Color.Red;
                    return;
                }
                DateTime dob1 =
    DateTime.Parse(txtDOB.Text);

                DateTime doj1 =
                    DateTime.Parse(txtDOJ.Text);
                using (SqlConnection con =
                    new SqlConnection(constr))
                {
                    string checkQuery =
                        @"SELECT COUNT(*)
                          FROM EmpBasicMaster
                          WHERE EmpID=@EmpID";

                    using (SqlCommand cmdCheck =
                        new SqlCommand(checkQuery, con))
                    {
                        cmdCheck.Parameters.AddWithValue(
                            "@EmpID",
                            txtEmpID.Text.Trim());

                        con.Open();

                        int count =
                            Convert.ToInt32(
                                cmdCheck.ExecuteScalar());

                        con.Close();

                        if (count > 0)
                        {
                            lblSingleMessage.Text =
                                "Employee ID already exists.";

                            lblSingleMessage.ForeColor =
                                Color.Red;

                            return;
                        }
                    }

                    string query =
                        @"INSERT INTO EmpBasicMaster
                        (
                            EmpID,
                            EmpName,
                            DOB,
                            DOJ,
                            MobileNo,
                            EmailId,
                            EmpCompany,
                            EmpDesignation,
                            EmpPostingPlace,
                            CreatedOn,
                            CreatedBy
                        )
                        VALUES
                        (
                            @EmpID,
                            @EmpName,
                            @DOB,
                            @DOJ,
                            @MobileNo,
                            @EmailId,
                            @EmpCompany,
                            @EmpDesignation,
                            @EmpPostingPlace,
                            GETDATE(),
                            @CreatedBy
                        )";

                    using (SqlCommand cmd =
                        new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue(
                            "@EmpID",
                            txtEmpID.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmpName",
                            txtEmpName.Text.Trim());

                        cmd.Parameters.AddWithValue(
     "@DOB",
     dob1.ToString("dd-MM-yyyy"));

                        cmd.Parameters.AddWithValue(
                            "@DOJ",
                            doj1.ToString("dd-MM-yyyy"));

                        cmd.Parameters.AddWithValue(
                            "@MobileNo",
                            txtMobileNo.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmailId",
                            txtEmailId.Text.Trim());

                        cmd.Parameters.AddWithValue(
                            "@EmpCompany",
                            ddlCompany.SelectedValue);

                        cmd.Parameters.AddWithValue(
                            "@EmpDesignation",
                            ddlDesignation.SelectedValue);

                        cmd.Parameters.AddWithValue(
                            "@EmpPostingPlace",
                            ddlPostingPlace.SelectedValue);

                        cmd.Parameters.AddWithValue(
                            "@CreatedBy",
                            "Admin");

                        con.Open();

                        cmd.ExecuteNonQuery();

                        con.Close();
                    }
                }

                lblSingleMessage.Text =
                    "Employee saved successfully.";

                lblSingleMessage.ForeColor =
                    Color.Green;

                ClearControls();

                BindCompany();
                BindDesignation();
                BindPostingPlace();

                LoadSelect2();

                ScriptManager.RegisterStartupScript(
                    this,
                    this.GetType(),
                    "ClearDropdown",
                    "ClearSearchableDropdown();",
                    true
                );
            }
            catch (Exception ex)
            {
                lblSingleMessage.Text =
                    ex.Message;

                lblSingleMessage.ForeColor =
                    Color.Red;
            }
        }

        #endregion

        #region BULK UPLOAD

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblBulkMessage.Text = "";

            try
            {
                if (!fuExcel.HasFile)
                {
                    lblBulkMessage.Text =
                        "Please select Excel file.";

                    lblBulkMessage.ForeColor =
                        Color.Red;

                    return;
                }

                string extension =
                    System.IO.Path.GetExtension(
                        fuExcel.FileName);

                if (extension != ".xlsx")
                {
                    lblBulkMessage.Text =
                        "Please upload only .xlsx file.";

                    lblBulkMessage.ForeColor =
                        Color.Red;

                    return;
                }

                int insertedCount = 0;
                int duplicateCount = 0;

                ExcelPackage.LicenseContext =
                    LicenseContext.NonCommercial;

                using (ExcelPackage package =
                    new ExcelPackage(fuExcel.FileContent))
                {
                    ExcelWorksheet ws =
                        package.Workbook.Worksheets[0];

                    int rowCount =
                        ws.Dimension.Rows;

                    // Header Validation

                    if (ws.Cells[1, 1].Text.Trim() != "EmpID")
                    {
                        lblBulkMessage.Text =
                            "Invalid Excel format.";

                        lblBulkMessage.ForeColor =
                            Color.Red;

                        return;
                    }

                    using (SqlConnection con =
                        new SqlConnection(constr))
                    {
                        con.Open();

                        for (int row = 2;
                             row <= rowCount;
                             row++)
                        {
                            try
                            {
                                string empid =
                                    ws.Cells[row, 1].Text.Trim();

                                string empname =
                                    ws.Cells[row, 2].Text.Trim();

                                string dob =
                                    ws.Cells[row, 3].Text.Trim();

                                string doj =
                                    ws.Cells[row, 4].Text.Trim();

                                string mobileno =
                                    ws.Cells[row, 5].Text.Trim();

                                string email =
                                    ws.Cells[row, 6].Text.Trim();

                                string company =
                                    ws.Cells[row, 7].Text.Trim();

                                string designation =
                                    ws.Cells[row, 8].Text.Trim();

                                string postingplace =
                                    ws.Cells[row, 9].Text.Trim();

                                // Mandatory Validation

                                if (empid == "")
                                {
                                    lblBulkMessage.Text =
                                        "EmpID missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                if (empname == "")
                                {
                                    lblBulkMessage.Text =
                                        "EmpName missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                if (dob == "")
                                {
                                    lblBulkMessage.Text =
                                        "DOB missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                if (doj == "")
                                {
                                    lblBulkMessage.Text =
                                        "DOJ missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                if (mobileno == "")
                                {
                                    lblBulkMessage.Text =
                                        "MobileNo missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                if (company == "")
                                {
                                    lblBulkMessage.Text =
                                        "EmpCompany missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                if (designation == "")
                                {
                                    lblBulkMessage.Text =
                                        "EmpDesignation missing at row "
                                        + row;

                                    lblBulkMessage.ForeColor =
                                        Color.Red;

                                    return;
                                }

                                // Duplicate Check

                                string checkQuery =
                                    @"SELECT COUNT(*)
                                      FROM EmpBasicMaster
                                      WHERE EmpID=@EmpID";

                                using (SqlCommand cmdCheck =
                                    new SqlCommand(checkQuery, con))
                                {
                                    cmdCheck.Parameters.AddWithValue(
                                        "@EmpID",
                                        empid);

                                    int count =
                                        Convert.ToInt32(
                                            cmdCheck.ExecuteScalar());

                                    if (count > 0)
                                    {
                                        duplicateCount++;
                                        continue;
                                    }
                                }
                                DateTime dobDate =
                                DateTime.ParseExact(
                                    dob,
                                    "dd-MM-yyyy",
                                    CultureInfo.InvariantCulture);

                                DateTime dojDate =
                                    DateTime.ParseExact(
                                        doj,
                                        "dd-MM-yyyy",
                                        CultureInfo.InvariantCulture);



                                string query =
                                    @"INSERT INTO EmpBasicMaster
                                    (
                                        EmpID,
                                        EmpName,
                                        DOB,
                                        DOJ,
                                        MobileNo,
                                        EmailId,
                                        EmpCompany,
                                        EmpDesignation,
                                        EmpPostingPlace,
                                        CreatedOn,
                                        CreatedBy
                                    )
                                    VALUES
                                    (
                                        @EmpID,
                                        @EmpName,
                                        @DOB,
                                        @DOJ,
                                        @MobileNo,
                                        @EmailId,
                                        @EmpCompany,
                                        @EmpDesignation,
                                        @EmpPostingPlace,
                                        GETDATE(),
                                        @CreatedBy
                                    )";

                                using (SqlCommand cmd =
                                    new SqlCommand(query, con))
                                {
                                    cmd.Parameters.AddWithValue(
                                        "@EmpID",
                                        empid);

                                    cmd.Parameters.AddWithValue(
                                        "@EmpName",
                                        empname);

                                    cmd.Parameters.AddWithValue(
                                   "@DOB",
                                   dobDate.ToString("dd-MM-yyyy"));

                                    cmd.Parameters.AddWithValue(
                                        "@DOJ",
                                        dojDate.ToString("dd-MM-yyyy"));
                                    // Insert

                                    cmd.Parameters.AddWithValue(
                                        "@MobileNo",
                                        mobileno);

                                    cmd.Parameters.AddWithValue(
                                        "@EmailId",
                                        email);

                                    cmd.Parameters.AddWithValue(
                                        "@EmpCompany",
                                        company);

                                    cmd.Parameters.AddWithValue(
                                        "@EmpDesignation",
                                        designation);

                                    cmd.Parameters.AddWithValue(
                                        "@EmpPostingPlace",
                                        postingplace);

                                    cmd.Parameters.AddWithValue(
                                        "@CreatedBy",
                                        "Admin");

                                    cmd.ExecuteNonQuery();

                                    insertedCount++;
                                }
                            }
                            catch (Exception exrow)
                            {
                                lblBulkMessage.Text =
                                    "Error at row "
                                    + row
                                    + " : "
                                    + exrow.Message;

                                lblBulkMessage.ForeColor =
                                    Color.Red;

                                return;
                            }
                        }

                        con.Close();
                    }
                }

                lblBulkMessage.Text =
                    insertedCount +
                    " records uploaded successfully. "
                    + duplicateCount +
                    " duplicate records skipped.";

                lblBulkMessage.ForeColor =
                    Color.Green;
            }
            catch (Exception ex)
            {
                lblBulkMessage.Text =
                    ex.Message;

                lblBulkMessage.ForeColor =
                    Color.Red;
            }
        }

        #endregion

        #region DROPDOWN BIND

        private void BindCompany()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string query =
                    @"SELECT DISTINCT EmpCompany
                      FROM EmpBasicMaster
                      WHERE ISNULL(EmpCompany,'') <> ''
                      ORDER BY EmpCompany";

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    con.Open();

                    ddlCompany.DataSource =
                        cmd.ExecuteReader();

                    ddlCompany.DataTextField =
                        "EmpCompany";

                    ddlCompany.DataValueField =
                        "EmpCompany";

                    ddlCompany.DataBind();

                    con.Close();
                }

                ddlCompany.Items.Insert(
                    0,
                    new ListItem("Select Company", "")
                );
            }
        }

        private void BindDesignation()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string query =
                    @"SELECT DISTINCT EmpDesignation
                      FROM EmpBasicMaster
                      WHERE ISNULL(EmpDesignation,'') <> ''
                      ORDER BY EmpDesignation";

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    con.Open();

                    ddlDesignation.DataSource =
                        cmd.ExecuteReader();

                    ddlDesignation.DataTextField =
                        "EmpDesignation";

                    ddlDesignation.DataValueField =
                        "EmpDesignation";

                    ddlDesignation.DataBind();

                    con.Close();
                }

                ddlDesignation.Items.Insert(
                    0,
                    new ListItem("Select Designation", "")
                );
            }
        }

        private void BindPostingPlace()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string query =
                    @"SELECT DISTINCT EmpPostingPlace
                      FROM EmpBasicMaster
                      WHERE ISNULL(EmpPostingPlace,'') <> ''
                      ORDER BY EmpPostingPlace";

                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    con.Open();

                    ddlPostingPlace.DataSource =
                        cmd.ExecuteReader();

                    ddlPostingPlace.DataTextField =
                        "EmpPostingPlace";

                    ddlPostingPlace.DataValueField =
                        "EmpPostingPlace";

                    ddlPostingPlace.DataBind();

                    con.Close();
                }

                ddlPostingPlace.Items.Insert(
                    0,
                    new ListItem("Select Posting Place", "")
                );
            }
        }

        #endregion

        #region CLEAR CONTROLS

        private void ClearControls()
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";
            txtDOB.Text = "";
            txtDOJ.Text = "";
            txtMobileNo.Text = "";
            txtEmailId.Text = "";

            ddlCompany.SelectedIndex = 0;

            ddlDesignation.ClearSelection();
            ddlPostingPlace.ClearSelection();

            ddlDesignation.SelectedIndex = 0;
            ddlPostingPlace.SelectedIndex = 0;
        }

        #endregion
    }
}