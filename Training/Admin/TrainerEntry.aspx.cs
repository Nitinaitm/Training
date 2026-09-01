using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Training.Admin
{
    public partial class TrainerEntry : System.Web.UI.Page
    {
        clsDataAccess obj =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                BindExpertise();

                BindQualification();

                BindTrainerGrid();
                lblCount.Text = "Total Trainers : " + gvTrainer.Rows.Count;

            }
        }
        private void BindExpertise()
        {
            DataTable dt =
                obj.GetDataTable(@"

SELECT
ExpertiseID,
ExpertiseName

FROM AreaOfExpertiseMaster

ORDER BY ExpertiseName");

            ddlExpertiseInternal.DataSource = dt;

            ddlExpertiseInternal.DataTextField = "ExpertiseName";

            ddlExpertiseInternal.DataValueField = "ExpertiseID";

            ddlExpertiseInternal.DataBind();

            ddlExpertiseInternal.Items.Insert(
                0,
                new ListItem(
                    "-- Select Expertise --",
                    ""));

            ddlExpertiseExternal.DataSource = dt;

            ddlExpertiseExternal.DataTextField = "ExpertiseName";

            ddlExpertiseExternal.DataValueField = "ExpertiseID";

            ddlExpertiseExternal.DataBind();

            ddlExpertiseExternal.Items.Insert(
                0,
                new ListItem(
                    "-- Select Expertise --",
                    ""));
        }

        private void BindQualification()
        {
            DataTable dt =
                obj.GetDataTable(@"

SELECT
QualificationID,
QualificationName

FROM QualificationMaster

ORDER BY QualificationName");

            ddlQualificationInternal.DataSource = dt;

            ddlQualificationInternal.DataTextField = "QualificationName";

            ddlQualificationInternal.DataValueField = "QualificationID";

            ddlQualificationInternal.DataBind();

            ddlQualificationInternal.Items.Insert(
                0,
                new ListItem(
                    "-- Select Qualification --",
                    ""));

            ddlQualificationExternal.DataSource = dt;

            ddlQualificationExternal.DataTextField = "QualificationName";

            ddlQualificationExternal.DataValueField = "QualificationID";

            ddlQualificationExternal.DataBind();

            ddlQualificationExternal.Items.Insert(
                0,
                new ListItem(
                    "-- Select Qualification --",
                    ""));
        }
        protected void ddlTrainerType_SelectedIndexChanged(
      object sender,
      EventArgs e)
        {
            pnlInternal.Visible = false;

            pnlExternal.Visible = false;

            if (ddlTrainerType.SelectedValue == "Internal")
            {
                pnlInternal.Visible = true;
            }

            if (ddlTrainerType.SelectedValue == "External")
            {
                pnlExternal.Visible = true;
            }
        }
        private void BindTrainerGrid()
        {
            string query = @"

SELECT

TM.TrainerID,

CASE
WHEN TM.TrainerType='Internal'
THEN TM.TrainerID + ' / ' + TM.EmpID
ELSE TM.TrainerID
END AS DisplayTrainerID,

TM.TrainerType,

CASE
WHEN TM.TrainerType='Internal'
THEN E.EmpName
ELSE TM.NameExternal
END AS TrainerName,

CASE

WHEN TM.TrainerType='Internal'

THEN E.EmpDesignation

ELSE TM.DesignationExternal

END AS Designation,

CASE

WHEN TM.TrainerType='Internal'

THEN E.EmpCompany

ELSE TM.TrainerOrganizerExternal

END AS Organization,



AEM.ExpertiseName,

ISNULL(TM.ExperienceYears,0) AS ExperienceYears,

ISNULL(TM.TrainerAvailability,'Available') AS TrainerAvailability,

ISNULL(TM.ActiveStatus,'Active') AS ActiveStatus

FROM TrainerMaster TM

LEFT JOIN EmpBasicMaster E
ON TM.EmpID=E.EmpID

LEFT JOIN AreaOfExpertiseMaster AEM
ON TM.AreaOfExpertiseID=AEM.ExpertiseID

WHERE 1=1";

            if (ddlSearchTrainerType.SelectedIndex > 0)
            {
                query += @"

AND TM.TrainerType='"
                + ddlSearchTrainerType.SelectedValue.Replace("'", "''")
                + "'";
            }

            if (txtSearchEmpID.Text.Trim().ToUpperInvariant() != "")
            {
                query += @"

AND
(
TM.EmpID LIKE '%"
            + txtSearchEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''")
            + @"%'

OR

TM.EmpIDExternal LIKE '%"
            + txtSearchEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''")
            + @"%'
)";
            }

            if (txtSearchTrainerName.Text.Trim() != "")
            {
                query += @"

AND
(
E.EmpName LIKE '%"
            + txtSearchTrainerName.Text.Trim().Replace("'", "''")
            + @"%'

OR

TM.NameExternal LIKE '%"
            + txtSearchTrainerName.Text.Trim().Replace("'", "''")
            + @"%'
)";
            }

            if (txtSearchOrganization.Text.Trim() != "")
            {
                query += @"

AND
(
E.EmpCompany LIKE '%"
            + txtSearchOrganization.Text.Trim().Replace("'", "''")
            + @"%'

OR

TM.TrainerOrganizerExternal LIKE '%"
            + txtSearchOrganization.Text.Trim().Replace("'", "''")
            + @"%'
)";
            }

            query += @"

ORDER BY TM.ID DESC";

            DataTable dt =
    obj.GetDataTable(query);

            gvTrainer.DataSource = dt;

            gvTrainer.DataBind();

            lblCount.Text =
                "Total Trainers : " + dt.Rows.Count;
        }
        protected void SearchChanged(
object sender,
EventArgs e)
        {
            BindTrainerGrid();
        }

        protected void ddlSearchTrainerType_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindTrainerGrid();

        }

        protected void btnSearch_Click(
           object sender,
           EventArgs e)
        {
            BindTrainerGrid();
        }

        protected void btnReset_Click(
    object sender,
    EventArgs e)
        {
            ddlSearchTrainerType.SelectedIndex = 0;

            txtSearchEmpID.Text = "";

            txtSearchTrainerName.Text = "";

            txtSearchOrganization.Text = "";

            BindTrainerGrid();
        }


        private string GenerateInternalTrainerID()
        {
            string query = @"
SELECT
ISNULL(MAX(ID),0)+1
FROM TrainerMaster
WHERE TrainerType='Internal'";

            int nextID =
                Convert.ToInt32(
                obj.ExecuteScalar(query));

            return
                "TRIN" +
                nextID.ToString("000");
        }

        private string GenerateExternalTrainerID()
        {
            string query = @"
SELECT
ISNULL(MAX(ID),0)+1
FROM TrainerMaster
WHERE TrainerType='External'";

            int nextID =
                Convert.ToInt32(
                obj.ExecuteScalar(query));

            return
                "TREX" +
                nextID.ToString("000");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlTrainerType.SelectedIndex == 0)
            {
                ShowMessage("Please Select Trainer Type.", System.Drawing.Color.Red);
                return;
            }

            if (ViewState["TrainerID"] == null)
            {
                if (ddlTrainerType.SelectedValue == "Internal")
                    SaveInternalTrainer();
                else
                    SaveExternalTrainer();
            }
            else
            {
                if (ddlTrainerType.SelectedValue == "Internal")
                    UpdateInternalTrainer();
                else
                    UpdateExternalTrainer();
            }
        }
        private void ClearForm()
        {
            ViewState["TrainerID"] = null;

            ddlTrainerType.SelectedIndex = 0;

            ddlTrainerType.Enabled = true;

            pnlInternal.Visible = false;

            pnlExternal.Visible = false;

            btnSave.Text = "Save Trainer";

            btnSave.CssClass = "btn btn-save";

            BindExpertise();

            BindQualification();

            // Internal

            txtEmpID.Text = "";

            ddlExpertiseInternal.SelectedIndex = 0;

            ddlQualificationInternal.SelectedIndex = 0;

            txtExperienceInternal.Text = "";

            txtCertificationInternal.Text = "";

            ddlAvailabilityInternal.SelectedIndex = 0;

            txtAvailableFromInternal.Text = "";

            txtAvailableToInternal.Text = "";

            txtRemarksInternal.Text = "";

            // External

            txtEmpIDExternal.Text = "";

            txtNameExternal.Text = "";

            txtDesignationExternal.Text = "";

            txtOrganizationExternal.Text = "";

            ddlExpertiseExternal.SelectedIndex = 0;

            ddlQualificationExternal.SelectedIndex = 0;

            txtExperienceExternal.Text = "";

            txtCertificationExternal.Text = "";

            ddlAvailabilityExternal.SelectedIndex = 0;

            txtAvailableFromExternal.Text = "";

            txtAvailableToExternal.Text = "";

            txtMobileExternal.Text = "";

            txtEmailExternal.Text = "";

            txtRemarksExternal.Text = "";

            // Employee Details Card

            lblEmpName.Text = "";

            lblEmpDesignation.Text = "";

            lblEmpCompany.Text = "";

            lblEmpMobile.Text = "";

            lblEmpEmail.Text = "";

            // Message

            lblMessage.Text = "";

            lblMessage.ForeColor = System.Drawing.Color.Black;
        }
        private void SaveInternalTrainer()
        {
            if (txtEmpID.Text.Trim() == "")
            {
                ShowMessage("Employee ID Required", System.Drawing.Color.Red);
                return;
            }

            if (ddlExpertiseInternal.SelectedIndex == 0)
            {
                ShowMessage("Please select Area of Expertise.", System.Drawing.Color.Red);
                return;
            }

            if (ddlQualificationInternal.SelectedIndex == 0)
            {
                ShowMessage("Please select Qualification.", System.Drawing.Color.Red);
                return;
            }

            DataTable dtEmp =
                obj.GetDataTable(@"

SELECT *

FROM EmpBasicMaster

WHERE EmpID='"
                + txtEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''")
                + "'");

            if (dtEmp.Rows.Count == 0)
            {
                ShowMessage("Employee ID Not Found.", System.Drawing.Color.Red);
                return;
            }

            DataTable dtDup =
                obj.GetDataTable(@"

SELECT *

FROM TrainerMaster

WHERE TrainerType='Internal'

AND EmpID='"
                + txtEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''")
                + "'");

            if (dtDup.Rows.Count > 0)
            {
                ShowMessage("Trainer Already Exists.", System.Drawing.Color.Red);
                return;
            }

            string trainerID = GenerateInternalTrainerID();

            string query = @"

INSERT INTO TrainerMaster
(
TrainerID,
TrainerType,
EmpID,
AreaOfExpertiseID,
QualificationID,
ExperienceYears,
Certifications,
TrainerAvailability,
AvailableFrom,
AvailableTo,
Remarks,
CreatedOn,
CreatedBy
)

VALUES
(
'"
        + trainerID + @"',
'Internal',
'"
        + txtEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''") + @"',
'"
        + ddlExpertiseInternal.SelectedValue + @"',
'"
        + ddlQualificationInternal.SelectedValue + @"',
'"
        + txtExperienceInternal.Text.Trim() + @"',
'"
        + txtCertificationInternal.Text.Trim().Replace("'", "''") + @"',
'"
        + ddlAvailabilityInternal.SelectedValue + @"',
'"
        + txtAvailableFromInternal.Text.Trim() + @"',
'"
        + txtAvailableToInternal.Text.Trim() + @"',
'"
        + txtRemarksInternal.Text.Trim().Replace("'", "''") + @"',
GETDATE(),
'Admin'
)";

            int i = obj.ExecuteSql(query);

            if (i > 0)
            {
                DataTable dtLogin =
obj.GetDataTable(@"

SELECT *

FROM Login

WHERE LoginIDUserID='"
+ trainerID + "'");

                if (dtLogin.Rows.Count == 0)
                {
                    Encryptor2 encryptor =
                        new Encryptor2();

                    string password =
                        encryptor.Encrypt("Bsphcl*123");

                    string firstLogin =
                        encryptor.Encrypt("Y");

                    string loginQuery = @"

INSERT INTO Login
(
LoginIDUserID,
Password,
Role,
CorrespondingEmpID,
Active,
re
)

VALUES
(
'"
                + trainerID + @"',
'"
                + password + @"',
'Trainer',
'"
                + txtEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''") + @"',
'Y',
'"
                + firstLogin + @"'
)";

                    obj.ExecuteSql(loginQuery);
                }
                ShowMessage("Internal Trainer Saved Successfully.", System.Drawing.Color.Green);

                txtEmpID.Text = "";

                ddlExpertiseInternal.SelectedIndex = 0;

                ddlQualificationInternal.SelectedIndex = 0;

                txtExperienceInternal.Text = "";

                txtCertificationInternal.Text = "";

                ddlAvailabilityInternal.SelectedIndex = 0;

                txtAvailableFromInternal.Text = "";

                txtAvailableToInternal.Text = "";

                txtRemarksInternal.Text = "";

                BindTrainerGrid();
            }
        }
        private void ShowMessage(string message, System.Drawing.Color color)
        {
            lblMessage.Text = message;

            lblMessage.ForeColor = color;
        }
        private void SaveExternalTrainer()
        {
            if (txtNameExternal.Text.Trim() == "")
            {
                ShowMessage("Trainer Name Required.", System.Drawing.Color.Red);
                return;
            }

            if (ddlExpertiseExternal.SelectedIndex == 0)
            {
                ShowMessage("Please select Area of Expertise.", System.Drawing.Color.Red);
                return;
            }

            if (ddlQualificationExternal.SelectedIndex == 0)
            {
                ShowMessage("Please select Qualification.", System.Drawing.Color.Red);
                return;
            }

            string trainerID = GenerateExternalTrainerID();

            string query = @"

INSERT INTO TrainerMaster
(
TrainerID,
TrainerType,
EmpIDExternal,
NameExternal,
DesignationExternal,
TrainerOrganizerExternal,
AreaOfExpertiseID,
QualificationID,
ExperienceYears,
Certifications,
TrainerAvailability,
AvailableFrom,
AvailableTo,
MobileNo,
EmailID,
Remarks,
CreatedOn,
CreatedBy
)

VALUES
(
'"
        + trainerID + @"',
'External',
'"
        + txtEmpIDExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + txtNameExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + txtDesignationExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + txtOrganizationExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + ddlExpertiseExternal.SelectedValue + @"',
'"
        + ddlQualificationExternal.SelectedValue + @"',
'"
        + txtExperienceExternal.Text.Trim() + @"',
'"
        + txtCertificationExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + ddlAvailabilityExternal.SelectedValue + @"',
'"
        + txtAvailableFromExternal.Text.Trim() + @"',
'"
        + txtAvailableToExternal.Text.Trim() + @"',
'"
        + txtMobileExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + txtEmailExternal.Text.Trim().Replace("'", "''") + @"',
'"
        + txtRemarksExternal.Text.Trim().Replace("'", "''") + @"',
GETDATE(),
'Admin'
)";

            int i = obj.ExecuteSql(query);

            if (i > 0)
            {
                DataTable dtLogin =
obj.GetDataTable(@"

SELECT *

FROM Login

WHERE LoginIDUserID='"
+ trainerID
+ "'");

                if (dtLogin.Rows.Count == 0)
                {
                    Encryptor2 encryptor =
                        new Encryptor2();

                    string password =
                        encryptor.Encrypt("Bsphcl*123");

                    string firstLogin =
                        encryptor.Encrypt("Y");

                    string loginQuery = @"

INSERT INTO Login
(
LoginIDUserID,
Password,
Role,
CorrespondingEmpID,
Active,
re
)

VALUES
(
'"
                + trainerID + @"',
'"
                + password + @"',
'Trainer',
'"
                + trainerID + @"',
'Y',
'"
                + firstLogin + @"'
)";

                    obj.ExecuteSql(loginQuery);
                }
                ShowMessage("External Trainer Saved Successfully.", System.Drawing.Color.Green);

                txtEmpIDExternal.Text = "";

                txtNameExternal.Text = "";

                txtDesignationExternal.Text = "";

                txtOrganizationExternal.Text = "";

                ddlExpertiseExternal.SelectedIndex = 0;

                ddlQualificationExternal.SelectedIndex = 0;

                txtExperienceExternal.Text = "";

                txtCertificationExternal.Text = "";

                ddlAvailabilityExternal.SelectedIndex = 0;

                txtAvailableFromExternal.Text = "";

                txtAvailableToExternal.Text = "";

                txtMobileExternal.Text = "";

                txtEmailExternal.Text = "";

                txtRemarksExternal.Text = "";

                BindTrainerGrid();
            }
        }

        protected void gvTrainer_RowCommand(
  object sender,
  GridViewCommandEventArgs e)
        {
            string trainerID = e.CommandArgument.ToString();

            if (e.CommandName == "ViewRecord")
            {
                LoadTrainerProfile(trainerID);

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "popup",
                    "var m=new bootstrap.Modal(document.getElementById('trainerModal'));m.show();",
                    true);
            }

            else if (e.CommandName == "EditRecord")
            {
                LoadTrainer(trainerID);
            }

            else if (e.CommandName == "DeleteRecord")
            {
                DeleteTrainer(trainerID);
            }
        }
        protected void txtEmpID_TextChanged(
object sender,
EventArgs e)
        {
            LoadEmployeeDetails();
        }
        private void LoadEmployeeDetails()
        {
            DataTable dt =
                obj.GetDataTable(@"

SELECT

EmpName,

EmpDesignation,

EmpCompany,

MobileNo,

EmailId

FROM EmpBasicMaster

WHERE EmpID='"
        + txtEmpID.Text.Trim().ToUpperInvariant().Replace("'", "''")
        + "'");

            if (dt.Rows.Count == 0)
            {
                lblEmpName.Text = "";

                lblEmpDesignation.Text = "";

                lblEmpCompany.Text = "";

                lblEmpMobile.Text = "";

                lblEmpEmail.Text = "";

                return;
            }

            lblEmpName.Text = dt.Rows[0]["EmpName"].ToString();

            lblEmpDesignation.Text = dt.Rows[0]["EmpDesignation"].ToString();

            lblEmpCompany.Text = dt.Rows[0]["EmpCompany"].ToString();

            lblEmpMobile.Text = dt.Rows[0]["MobileNo"].ToString();

            lblEmpEmail.Text = dt.Rows[0]["EmailId"].ToString();
        }
        private void LoadTrainer(string trainerID)
        {
            DataTable dt =
                obj.GetDataTable(@"

SELECT *

FROM TrainerMaster

WHERE TrainerID='"
        + trainerID + "'");

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];

            ViewState["TrainerID"] = trainerID;

            ddlTrainerType.SelectedValue =
                dr["TrainerType"].ToString();

            ddlTrainerType_SelectedIndexChanged(null, null);

            if (dr["TrainerType"].ToString() == "Internal")
            {
                txtEmpID.Text =
                    dr["EmpID"].ToString().ToUpperInvariant();
                LoadEmployeeDetails();
                ddlExpertiseInternal.SelectedValue =
                    dr["AreaOfExpertiseID"].ToString();

                ddlQualificationInternal.SelectedValue =
                    dr["QualificationID"].ToString();

                txtExperienceInternal.Text =
                    dr["ExperienceYears"].ToString();

                txtCertificationInternal.Text =
                    dr["Certifications"].ToString();

                string availability = dr["TrainerAvailability"].ToString();

                if (ddlAvailabilityInternal.Items.FindByValue(availability) != null)
                {
                    ddlAvailabilityInternal.SelectedValue = availability;
                }
                else
                {
                    ddlAvailabilityInternal.SelectedIndex = 0;
                }

                txtAvailableFromInternal.Text =
                    dr["AvailableFrom"].ToString();

                txtAvailableToInternal.Text =
                    dr["AvailableTo"].ToString();

                txtRemarksInternal.Text =
                    dr["Remarks"].ToString();
            }
            else
            {
                txtEmpIDExternal.Text =
                    dr["EmpIDExternal"].ToString().ToUpperInvariant();

                txtNameExternal.Text =
                    dr["NameExternal"].ToString();

                txtDesignationExternal.Text =
                    dr["DesignationExternal"].ToString();

                txtOrganizationExternal.Text =
                    dr["TrainerOrganizerExternal"].ToString();

                ddlExpertiseExternal.SelectedValue =
                    dr["AreaOfExpertiseID"].ToString();

                ddlQualificationExternal.SelectedValue =
                    dr["QualificationID"].ToString();

                txtExperienceExternal.Text =
                    dr["ExperienceYears"].ToString();

                txtCertificationExternal.Text =
                    dr["Certifications"].ToString();

                string availability = dr["TrainerAvailability"].ToString();


                if (ddlAvailabilityExternal.Items.FindByValue(availability) != null)
                {
                    ddlAvailabilityExternal.SelectedValue = availability;
                }
                else
                {
                    ddlAvailabilityExternal.SelectedIndex = 0;
                }

                txtAvailableFromExternal.Text =
                    dr["AvailableFrom"].ToString();

                txtAvailableToExternal.Text =
                    dr["AvailableTo"].ToString();

                txtMobileExternal.Text =
                    dr["MobileNo"].ToString();

                txtEmailExternal.Text =
                    dr["EmailID"].ToString();

                txtRemarksExternal.Text =
                    dr["Remarks"].ToString();
            }


            btnSave.Text = "Update Trainer";

            btnSave.CssClass = "btn btn-warning";

            ddlTrainerType.Enabled = false;
        }

        private void DeleteTrainer(string trainerID)
        {
            object cnt =
                obj.ExecuteScalar(@"

SELECT COUNT(*)

FROM SessionMaster

WHERE TrainerID='"
        + trainerID + "'");

            if (Convert.ToInt32(cnt) > 0)
            {
                ShowMessage(
                "Trainer already assigned in Session. Delete not allowed.",
                System.Drawing.Color.Red);

                return;
            }

            obj.ExecuteSql(@"

DELETE

FROM TrainerMaster

WHERE TrainerID='"
        + trainerID + "'");

            obj.ExecuteSql(@"

DELETE

FROM Login

WHERE LoginIDUserID='"
+ trainerID
+ @"'");

            ShowMessage(
            "Trainer Deleted Successfully.",
            System.Drawing.Color.Green);

            BindTrainerGrid();
        }

        private void UpdateInternalTrainer()
        {
            string query = @"

UPDATE TrainerMaster

SET

AreaOfExpertiseID='" + ddlExpertiseInternal.SelectedValue + @"',

QualificationID='" + ddlQualificationInternal.SelectedValue + @"',

ExperienceYears='" + txtExperienceInternal.Text.Trim() + @"',

Certifications='" + txtCertificationInternal.Text.Trim().Replace("'", "''") + @"',

TrainerAvailability='" + ddlAvailabilityInternal.SelectedValue + @"',

AvailableFrom='" + txtAvailableFromInternal.Text.Trim() + @"',

AvailableTo='" + txtAvailableToInternal.Text.Trim() + @"',

Remarks='" + txtRemarksInternal.Text.Trim().Replace("'", "''") + @"'

WHERE TrainerID='" + ViewState["TrainerID"].ToString() + "'";

            if (obj.ExecuteSql(query) > 0)
            {
                ShowMessage("Trainer Updated Successfully.", System.Drawing.Color.Green);

                ClearForm();

                BindTrainerGrid();
            }
        }

        private void UpdateExternalTrainer()
        {
            string query = @"

UPDATE TrainerMaster

SET

EmpIDExternal='" + txtEmpIDExternal.Text.Trim().ToUpperInvariant().Replace("'", "''") + @"',

NameExternal='" + txtNameExternal.Text.Trim().Replace("'", "''") + @"',

DesignationExternal='" + txtDesignationExternal.Text.Trim().Replace("'", "''") + @"',

TrainerOrganizerExternal='" + txtOrganizationExternal.Text.Trim().Replace("'", "''") + @"',

AreaOfExpertiseID='" + ddlExpertiseExternal.SelectedValue + @"',

QualificationID='" + ddlQualificationExternal.SelectedValue + @"',

ExperienceYears='" + txtExperienceExternal.Text.Trim() + @"',

Certifications='" + txtCertificationExternal.Text.Trim().Replace("'", "''") + @"',

TrainerAvailability='" + ddlAvailabilityExternal.SelectedValue + @"',

AvailableFrom='" + txtAvailableFromExternal.Text.Trim() + @"',

AvailableTo='" + txtAvailableToExternal.Text.Trim() + @"',

MobileNo='" + txtMobileExternal.Text.Trim().Replace("'", "''") + @"',

EmailID='" + txtEmailExternal.Text.Trim().Replace("'", "''") + @"',

Remarks='" + txtRemarksExternal.Text.Trim().Replace("'", "''") + @"'

WHERE TrainerID='" + ViewState["TrainerID"].ToString() + "'";

            if (obj.ExecuteSql(query) > 0)
            {
                ShowMessage("Trainer Updated Successfully.", System.Drawing.Color.Green);

                ClearForm();

                BindTrainerGrid();
            }
        }

        private void LoadTrainerProfile(string trainerID)
        {
            DataTable dt = obj.GetDataTable(@"

SELECT

TM.TrainerID,

TM.TrainerType,

CASE

WHEN TM.TrainerType='Internal'

THEN E.EmpName

ELSE TM.NameExternal

END AS TrainerName,

CASE

WHEN TM.TrainerType='Internal'

THEN E.EmpDesignation

ELSE TM.DesignationExternal

END AS Designation,

CASE

WHEN TM.TrainerType='Internal'

THEN E.EmpCompany

ELSE TM.TrainerOrganizerExternal

END AS Organization,

QM.QualificationName,

AEM.ExpertiseName,

TM.ExperienceYears,

TM.Certifications,

TM.TrainerAvailability,

TM.AvailableFrom,

TM.AvailableTo,

CASE
WHEN TM.TrainerType='Internal'
THEN E.MobileNo
ELSE TM.MobileNo
END AS MobileNo,

CASE
WHEN TM.TrainerType='Internal'
THEN E.EmailId
ELSE TM.EmailID
END AS EmailID,

TM.Remarks

FROM TrainerMaster TM

LEFT JOIN EmpBasicMaster E

ON TM.EmpID=E.EmpID

LEFT JOIN QualificationMaster QM

ON TM.QualificationID=QM.QualificationID

LEFT JOIN AreaOfExpertiseMaster AEM

ON TM.AreaOfExpertiseID=AEM.ExpertiseID

WHERE TM.TrainerID='" + trainerID + "'");

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];

            lblTrainerID.Text = dr["TrainerID"].ToString();

            lblTrainerName.Text = dr["TrainerName"].ToString();

            lblTrainerType.Text = dr["TrainerType"].ToString();

            lblOrganization.Text = dr["Organization"].ToString();

            lblDesignation.Text = dr["Designation"].ToString();

            lblQualification.Text = dr["QualificationName"].ToString();

            lblExpertise.Text = dr["ExpertiseName"].ToString();

            lblExperience.Text = dr["ExperienceYears"].ToString() + " Years";

            lblCertification.Text = dr["Certifications"].ToString();

            lblAvailability.Text = dr["TrainerAvailability"].ToString();

            lblTime.Text = dr["AvailableFrom"].ToString() + " To " + dr["AvailableTo"].ToString();

            lblMobile.Text = dr["MobileNo"].ToString();

            lblEmail.Text = dr["EmailID"].ToString();

            lblRemarks.Text = dr["Remarks"].ToString();
        }

    }
}