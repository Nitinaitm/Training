using OfficeOpenXml;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class EmpBasicMaster : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                BindCompany();
                BindDesignation();
                BindPostingPlace();
                BindPostingPlaceOptions();
                SetPostingDetailMode();
                LoadSelect2();
            }
        }

        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        private void LoadSelect2()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "LoadSearchableDropdown", "LoadSearchableDropdown();", true);
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPostingDepartment();
            BindFieldHierarchy();
            SetPostingDetailMode();
            LoadSelect2();
        }

        protected void ddlPostingDetailPlace_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetPostingDetailMode();
            LoadSelect2();
        }

        protected void ddlAreaBoardZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCircle();
            LoadSelect2();
        }

        protected void ddlCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDivision();
            LoadSelect2();
        }

        protected void ddlDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubdivision();
            LoadSelect2();
        }

        protected void ddlSubdivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSection();
            LoadSelect2();
        }

        private void BindCompany()
        {
            DataTable dt = DB().GetDataTable("SELECT CompanyID,CompanyName,CompanyAlias FROM CompanyMaster ORDER BY CompanyName");
            ddlCompany.Items.Clear();
            ddlCompany.Items.Add(new ListItem("Select Company", ""));
            foreach (DataRow row in dt.Rows)
            {
                string name = Convert.ToString(row["CompanyName"]);
                string alias = Convert.ToString(row["CompanyAlias"]);
                string text = string.IsNullOrWhiteSpace(alias) ? name : name + " (" + alias + ")";
                ddlCompany.Items.Add(new ListItem(text, name));
            }
        }

        private void BindDesignation()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>'' ORDER BY EmpDesignation");
            BindSimpleList(ddlDesignation, dt, "EmpDesignation", "Select Designation");
        }

        private void BindPostingPlace()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>'' ORDER BY EmpPostingPlace");
            BindSimpleList(ddlPostingPlace, dt, "EmpPostingPlace", "Select HRMS Posting Place");
        }

        private void BindPostingPlaceOptions()
        {
            ddlPostingDetailPlace.Items.Clear();
            ddlPostingDetailPlace.Items.Add(new ListItem("Select Posting Place", ""));
            ddlPostingDetailPlace.Items.Add(new ListItem("HQ", "HQ"));

            if (!IsHqOnlyCompany())
            {
                ddlPostingDetailPlace.Items.Add(new ListItem("Field Office", "Field"));
            }
        }

        private void BindPostingDepartment()
        {
            ddlPostingDepartment.Items.Clear();
            ddlPostingDepartment.Items.Add(new ListItem("Select Department / Office / Cell", ""));

            if (string.IsNullOrWhiteSpace(ddlCompany.SelectedValue))
            {
                return;
            }

            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.EmpPostingDepartment FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE EBM.EmpCompany=@Company AND ISNULL(EPD.EmpPostingDepartment,'')<>'' ORDER BY EPD.EmpPostingDepartment", new SqlParameter[] { new SqlParameter("@Company", ddlCompany.SelectedValue) });
            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row["EmpPostingDepartment"]);
                ddlPostingDepartment.Items.Add(new ListItem(value, value));
            }
        }

        private void BindFieldHierarchy()
        {
            BindAreaBoardZone();
            ClearList(ddlCircle, "Select Circle");
            ClearList(ddlDivision, "Select Division");
            ClearList(ddlSubdivision, "Select Subdivision");
            ClearList(ddlSection, "Select Section");
        }

        private void BindAreaBoardZone()
        {
            ClearList(ddlAreaBoardZone, "Select Area Board / Zone");

            string companyID = GetCompanyID(ddlCompany.SelectedValue);

            if (companyID <= 0)
            {
                return;
            }

            DataTable dt = DB().GetDataTable("SELECT ZoneID,ZoneName FROM ZoneMaster WHERE CompanyID=@CompanyID ORDER BY ZoneName", new SqlParameter[] { new SqlParameter("@CompanyID", companyID) });
            foreach (DataRow row in dt.Rows)
            {
                ddlAreaBoardZone.Items.Add(new ListItem(Convert.ToString(row["ZoneName"]), Convert.ToString(row["ZoneID"])));
            }
        }

        private void BindCircle()
        {
            ClearList(ddlCircle, "Select Circle");

            int zoneID;
            int companyID = GetCompanyID(ddlCompany.SelectedValue);

            if (companyID <= 0 || !int.TryParse(ddlAreaBoardZone.SelectedValue, out zoneID))
            {
                return;
            }

            DataTable dt = DB().GetDataTable("SELECT CircleID,CircleName FROM CircleMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID ORDER BY CircleName", new SqlParameter[] { new SqlParameter("@CompanyID", companyID), new SqlParameter("@ZoneID", zoneID) });
            foreach (DataRow row in dt.Rows)
            {
                ddlCircle.Items.Add(new ListItem(Convert.ToString(row["CircleName"]), Convert.ToString(row["CircleID"])));
            }
        }

        private void BindDivision()
        {
            ClearList(ddlDivision, "Select Division");

            int circleID;
            int zoneID;
            int companyID = GetCompanyID(ddlCompany.SelectedValue);

            if (companyID <= 0 || !int.TryParse(ddlAreaBoardZone.SelectedValue, out zoneID) || !int.TryParse(ddlCircle.SelectedValue, out circleID))
            {
                return;
            }

            DataTable dt = DB().GetDataTable("SELECT DivisionID,DivisionName FROM DivisionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID ORDER BY DivisionName", new SqlParameter[] { new SqlParameter("@CompanyID", companyID), new SqlParameter("@ZoneID", zoneID), new SqlParameter("@CircleID", circleID) });
            foreach (DataRow row in dt.Rows)
            {
                ddlDivision.Items.Add(new ListItem(Convert.ToString(row["DivisionName"]), Convert.ToString(row["DivisionID"])));
            }
        }

        private void BindSubdivision()
        {
            ClearList(ddlSubdivision, "Select Subdivision");

            int divisionID;
            int circleID;
            int zoneID;
            int companyID = GetCompanyID(ddlCompany.SelectedValue);

            if (companyID <= 0 || !int.TryParse(ddlAreaBoardZone.SelectedValue, out zoneID) || !int.TryParse(ddlCircle.SelectedValue, out circleID) || !int.TryParse(ddlDivision.SelectedValue, out divisionID))
            {
                return;
            }

            DataTable dt = DB().GetDataTable("SELECT SubdivisionID,SubdivisionName FROM SubdivisionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID AND DivisionID=@DivisionID ORDER BY SubdivisionName", new SqlParameter[] { new SqlParameter("@CompanyID", companyID), new SqlParameter("@ZoneID", zoneID), new SqlParameter("@CircleID", circleID), new SqlParameter("@DivisionID", divisionID) });
            foreach (DataRow row in dt.Rows)
            {
                ddlSubdivision.Items.Add(new ListItem(Convert.ToString(row["SubdivisionName"]), Convert.ToString(row["SubdivisionID"])));
            }
        }

        private void BindSection()
        {
            ClearList(ddlSection, "Select Section");

            int subdivisionID;
            int divisionID;
            int circleID;
            int zoneID;
            int companyID = GetCompanyID(ddlCompany.SelectedValue);

            if (companyID <= 0 || !int.TryParse(ddlAreaBoardZone.SelectedValue, out zoneID) || !int.TryParse(ddlCircle.SelectedValue, out circleID) || !int.TryParse(ddlDivision.SelectedValue, out divisionID) || !int.TryParse(ddlSubdivision.SelectedValue, out subdivisionID))
            {
                return;
            }

            DataTable dt = DB().GetDataTable("SELECT SectionID,SectionName FROM SectionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID AND DivisionID=@DivisionID AND SubdivisionID=@SubdivisionID ORDER BY SectionName", new SqlParameter[] { new SqlParameter("@CompanyID", companyID), new SqlParameter("@ZoneID", zoneID), new SqlParameter("@CircleID", circleID), new SqlParameter("@DivisionID", divisionID), new SqlParameter("@SubdivisionID", subdivisionID) });
            foreach (DataRow row in dt.Rows)
            {
                ddlSection.Items.Add(new ListItem(Convert.ToString(row["SectionName"]), Convert.ToString(row["SectionID"])));
            }
        }

        private int GetCompanyID(string company)
        {
            if (string.IsNullOrWhiteSpace(company))
            {
                return 0;
            }

            object value = DB().ExecuteScalar("SELECT TOP 1 ID FROM CompanyMaster WHERE CompanyName=@Company OR CompanyAlias=@Company", new SqlParameter[] { new SqlParameter("@Company", company) });
            int companyID;
            return value != null && int.TryParse(Convert.ToString(value), out companyID) ? companyID : 0;
        }

        private bool IsHqOnlyCompany()
        {
            string company = ddlCompany.SelectedValue.Trim();
            return company.Equals("BSPHCL", StringComparison.OrdinalIgnoreCase) || company.Equals("BSPGCL", StringComparison.OrdinalIgnoreCase);
        }

        private void SetPostingDetailMode()
        {
            bool hq = string.Equals(ddlPostingDetailPlace.SelectedValue, "HQ", StringComparison.OrdinalIgnoreCase);
            bool field = string.Equals(ddlPostingDetailPlace.SelectedValue, "Field", StringComparison.OrdinalIgnoreCase);

            if (IsHqOnlyCompany())
            {
                if (ddlPostingDetailPlace.Items.FindByValue("Field") != null)
                {
                    ddlPostingDetailPlace.Items.Remove(ddlPostingDetailPlace.Items.FindByValue("Field"));
                }

                if (string.IsNullOrWhiteSpace(ddlPostingDetailPlace.SelectedValue))
                {
                    ddlPostingDetailPlace.SelectedValue = "HQ";
                    hq = true;
                }
            }

            divPostingDepartment.Visible = hq;
            divAreaBoardZone.Visible = field;
            divCircle.Visible = field;
            divDivision.Visible = field;
            divSubdivision.Visible = field;
            divSection.Visible = field;
        }

        private void ClearList(DropDownList ddl, string firstText)
        {
            ddl.Items.Clear();
            ddl.Items.Add(new ListItem(firstText, ""));
        }

        private void BindSimpleList(DropDownList ddl, DataTable dt, string field, string firstText)
        {
            ClearList(ddl, firstText);
            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row[field]);
                ddl.Items.Add(new ListItem(value, value));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblSingleMessage.Text = "";

            if (!Page.IsValid)
            {
                LoadSelect2();
                return;
            }

            DateTime dob;
            DateTime doj;

            if (!DateTime.TryParse(txtDOB.Text.Trim(), out dob))
            {
                ShowError("Invalid DOB.");
                return;
            }

            if (!DateTime.TryParse(txtDOJ.Text.Trim(), out doj))
            {
                ShowError("Invalid DOJ.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ddlPostingDetailPlace.SelectedValue))
            {
                ShowError("Select Posting Details Posting Place.");
                return;
            }

            if (IsHqOnlyCompany() && ddlPostingDetailPlace.SelectedValue != "HQ")
            {
                ShowError("BSPHCL and BSPGCL allow HQ posting only.");
                return;
            }

            if (ddlPostingDetailPlace.SelectedValue == "HQ" && string.IsNullOrWhiteSpace(ddlPostingDepartment.SelectedValue))
            {
                ShowError("Select Department / Office / Cell.");
                return;
            }

            if (ddlPostingDetailPlace.SelectedValue == "Field" && (string.IsNullOrWhiteSpace(ddlAreaBoardZone.SelectedValue) || string.IsNullOrWhiteSpace(ddlCircle.SelectedValue) || string.IsNullOrWhiteSpace(ddlDivision.SelectedValue) || string.IsNullOrWhiteSpace(ddlSubdivision.SelectedValue) || string.IsNullOrWhiteSpace(ddlSection.SelectedValue)))
            {
                ShowError("Select complete Field Office hierarchy.");
                return;
            }

            clsDataAccess db = DB();

            try
            {
                db.BeginTransaction();

                int existing = Convert.ToInt32(db.ExecuteScalar("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@EmpID", new SqlParameter[] { new SqlParameter("@EmpID", txtEmpID.Text.Trim().ToUpperInvariant()) }, db.Transaction));

                if (existing > 0)
                {
                    db.Rollback();
                    ShowError("Employee ID already exists.");
                    return;
                }

                db.ExecuteSql("INSERT INTO EmpBasicMaster(EmpID,EmpName,DOB,DOJ,MobileNo,EmailId,EmpCompany,EmpDesignation,EmpPostingPlace,CreatedOn,CreatedBy) VALUES(@EmpID,@EmpName,@DOB,@DOJ,@MobileNo,@EmailId,@EmpCompany,@EmpDesignation,@EmpPostingPlace,GETDATE(),@CreatedBy)", new SqlParameter[] { new SqlParameter("@EmpID", txtEmpID.Text.Trim().ToUpperInvariant()), new SqlParameter("@EmpName", txtEmpName.Text.Trim()), new SqlParameter("@DOB", dob.ToString("dd-MM-yyyy")), new SqlParameter("@DOJ", doj.ToString("dd-MM-yyyy")), new SqlParameter("@MobileNo", txtMobileNo.Text.Trim()), new SqlParameter("@EmailId", txtEmailId.Text.Trim()), new SqlParameter("@EmpCompany", ddlCompany.SelectedValue), new SqlParameter("@EmpDesignation", ddlDesignation.SelectedValue), new SqlParameter("@EmpPostingPlace", ddlPostingPlace.SelectedValue), new SqlParameter("@CreatedBy", "Admin") }, db.Transaction);

                SavePostingDetails(db, txtEmpID.Text.Trim().ToUpperInvariant(), false);

                db.Commit();

                lblSingleMessage.ForeColor = Color.Green;
                lblSingleMessage.Text = "Employee and Posting Details saved successfully.";
                ClearControls();
                BindCompany();
                BindDesignation();
                BindPostingPlace();
                BindPostingPlaceOptions();
                SetPostingDetailMode();
                LoadSelect2();
            }
            catch (Exception ex)
            {
                try
                {
                    db.Rollback();
                }
                catch
                {
                }

                ShowError(ex.Message);
            }
        }

        private void SavePostingDetails(clsDataAccess db, string empID, bool update)
        {
            string department = ddlPostingDetailPlace.SelectedValue == "HQ" ? ddlPostingDepartment.SelectedValue : "";
            string zone = ddlPostingDetailPlace.SelectedValue == "Field" ? ddlAreaBoardZone.SelectedItem.Text : "";
            string circle = ddlPostingDetailPlace.SelectedValue == "Field" ? ddlCircle.SelectedItem.Text : "";
            string division = ddlPostingDetailPlace.SelectedValue == "Field" ? ddlDivision.SelectedItem.Text : "";
            string subdivision = ddlPostingDetailPlace.SelectedValue == "Field" ? ddlSubdivision.SelectedItem.Text : "";
            string section = ddlPostingDetailPlace.SelectedValue == "Field" ? ddlSection.SelectedItem.Text : "";

            if (update)
            {
                db.ExecuteSql("UPDATE EmpPostingDetails SET EmpPostingPlace=@EmpPostingPlace,EmpPostingDepartment=@EmpPostingDepartment,AreaBoardZone=@AreaBoardZone,Circle=@Circle,Division=@Division,Subdivision=@Subdivision,Section=@Section,CreatedOn=GETDATE(),CreatedBy=@CreatedBy WHERE ID=(SELECT TOP 1 ID FROM EmpPostingDetails WHERE EmpID=@EmpID ORDER BY ID DESC)", new SqlParameter[] { new SqlParameter("@EmpPostingPlace", ddlPostingDetailPlace.SelectedValue), new SqlParameter("@EmpPostingDepartment", department), new SqlParameter("@AreaBoardZone", zone), new SqlParameter("@Circle", circle), new SqlParameter("@Division", division), new SqlParameter("@Subdivision", subdivision), new SqlParameter("@Section", section), new SqlParameter("@CreatedBy", "Admin"), new SqlParameter("@EmpID", empID) }, db.Transaction);
                return;
            }

            db.ExecuteSql("INSERT INTO EmpPostingDetails(EmpID,EmpPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,Subdivision,Section,EmpOnDeputation,CreatedOn,CreatedBy,AssessmentYear) VALUES(@EmpID,@EmpPostingPlace,@EmpPostingDepartment,@AreaBoardZone,@Circle,@Division,@Subdivision,@Section,@EmpOnDeputation,GETDATE(),@CreatedBy,@AssessmentYear)", new SqlParameter[] { new SqlParameter("@EmpID", empID), new SqlParameter("@EmpPostingPlace", ddlPostingDetailPlace.SelectedValue), new SqlParameter("@EmpPostingDepartment", department), new SqlParameter("@AreaBoardZone", zone), new SqlParameter("@Circle", circle), new SqlParameter("@Division", division), new SqlParameter("@Subdivision", subdivision), new SqlParameter("@Section", section), new SqlParameter("@EmpOnDeputation", "NO"), new SqlParameter("@CreatedBy", "Admin"), new SqlParameter("@AssessmentYear", GetAssessmentYear()) }, db.Transaction);
        }

        private string GetAssessmentYear()
        {
            DateTime now = DateTime.Now;
            int startYear = now.Month >= 4 ? now.Year : now.Year - 1;
            return startYear.ToString() + "-" + (startYear + 1).ToString().Substring(2);
        }

        private void ShowError(string message)
        {
            lblSingleMessage.ForeColor = Color.Red;
            lblSingleMessage.Text = message;
            LoadSelect2();
        }

        private void ClearControls()
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";
            txtDOB.Text = "";
            txtDOJ.Text = "";
            txtMobileNo.Text = "";
            txtEmailId.Text = "";
            ddlCompany.SelectedIndex = 0;
            ddlDesignation.SelectedIndex = 0;
            ddlPostingPlace.SelectedIndex = 0;
            BindPostingPlaceOptions();
            ddlPostingDetailPlace.SelectedValue = "";
            ClearList(ddlPostingDepartment, "Select Department / Office / Cell");
            BindFieldHierarchy();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            lblBulkMessage.Text = "";

            try
            {
                if (!fuExcel.HasFile)
                {
                    lblBulkMessage.ForeColor = Color.Red;
                    lblBulkMessage.Text = "Please select Excel file.";
                    return;
                }

                if (!string.Equals(System.IO.Path.GetExtension(fuExcel.FileName), ".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    lblBulkMessage.ForeColor = Color.Red;
                    lblBulkMessage.Text = "Please upload only .xlsx file.";
                    return;
                }

                int insertedCount = 0;
                int duplicateCount = 0;

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage(fuExcel.FileContent))
                {
                    ExcelWorksheet ws = package.Workbook.Worksheets[0];

                    if (ws == null || ws.Dimension == null || ws.Cells[1, 1].Text.Trim() != "EmpID")
                    {
                        lblBulkMessage.ForeColor = Color.Red;
                        lblBulkMessage.Text = "Invalid Excel format.";
                        return;
                    }

                    int rowCount = ws.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        string empid = ws.Cells[row, 1].Text.Trim();
                        string empname = ws.Cells[row, 2].Text.Trim();
                        string dob = ws.Cells[row, 3].Text.Trim();
                        string doj = ws.Cells[row, 4].Text.Trim();
                        string mobileno = ws.Cells[row, 5].Text.Trim();
                        string email = ws.Cells[row, 6].Text.Trim();
                        string company = ws.Cells[row, 7].Text.Trim();
                        string designation = ws.Cells[row, 8].Text.Trim();
                        string postingplace = ws.Cells[row, 9].Text.Trim();

                        if (string.IsNullOrWhiteSpace(empid) || string.IsNullOrWhiteSpace(empname) || string.IsNullOrWhiteSpace(dob) || string.IsNullOrWhiteSpace(doj) || string.IsNullOrWhiteSpace(mobileno) || string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(designation))
                        {
                            lblBulkMessage.ForeColor = Color.Red;
                            lblBulkMessage.Text = "Mandatory value missing at row " + row + ".";
                            return;
                        }

                        int count = Convert.ToInt32(DB().ExecuteScalar("SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@EmpID", new SqlParameter[] { new SqlParameter("@EmpID", empid) }));

                        if (count > 0)
                        {
                            duplicateCount++;
                            continue;
                        }

                        DateTime dobDate;
                        DateTime dojDate;

                        if (!DateTime.TryParseExact(dob, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dobDate) || !DateTime.TryParseExact(doj, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dojDate))
                        {
                            lblBulkMessage.ForeColor = Color.Red;
                            lblBulkMessage.Text = "Invalid date format at row " + row + ". Use dd-MM-yyyy.";
                            return;
                        }

                        DB().ExecuteSql("INSERT INTO EmpBasicMaster(EmpID,EmpName,DOB,DOJ,MobileNo,EmailId,EmpCompany,EmpDesignation,EmpPostingPlace,CreatedOn,CreatedBy) VALUES(@EmpID,@EmpName,@DOB,@DOJ,@MobileNo,@EmailId,@EmpCompany,@EmpDesignation,@EmpPostingPlace,GETDATE(),@CreatedBy)", new SqlParameter[] { new SqlParameter("@EmpID", empid), new SqlParameter("@EmpName", empname), new SqlParameter("@DOB", dobDate.ToString("dd-MM-yyyy")), new SqlParameter("@DOJ", dojDate.ToString("dd-MM-yyyy")), new SqlParameter("@MobileNo", mobileno), new SqlParameter("@EmailId", email), new SqlParameter("@EmpCompany", company), new SqlParameter("@EmpDesignation", designation), new SqlParameter("@EmpPostingPlace", postingplace), new SqlParameter("@CreatedBy", "Admin") });

                        insertedCount++;
                    }
                }

                lblBulkMessage.ForeColor = Color.Green;
                lblBulkMessage.Text = insertedCount + " records uploaded successfully. " + duplicateCount + " duplicate records skipped.";
            }
            catch (Exception ex)
            {
                lblBulkMessage.ForeColor = Color.Red;
                lblBulkMessage.Text = ex.Message;
            }
        }
    }
}