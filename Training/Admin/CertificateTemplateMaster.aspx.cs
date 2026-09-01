using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Training.Admin
{
    public partial class CertificateTemplateMaster :
        System.Web.UI.Page
    {
        //---------------------------------------------------------
        // Database
        //---------------------------------------------------------

        clsDataAccess objDB =
            new clsDataAccess();


        //---------------------------------------------------------
        // Page Load
        //---------------------------------------------------------

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();

                ResetForm();
            }
        }


        //---------------------------------------------------------
        // Bind Grid
        //---------------------------------------------------------

        private void BindGrid()
        {
            string query =
        @"
SELECT
    TemplateID,
    TemplateName,
    Description,
    DisplayOrder,
    Orientation,
    PaperSize,
    PageWidth,
    PageHeight,
    BackgroundImage,
    LogoImage,
    HeaderText,
    FooterText,
    CourseTitleFontSize,
    HeaderFontSize,
    FooterFontSize,
    BodyFontSize,
    NameFontSize,

    LogoX,
    LogoY,
    HeaderY,
    TitleY,
    BodyY,
    LeftSignatureX,
    RightSignatureX,
    SignatureY,
    FooterY,

    CreatedOn,
    Active

FROM
    CertificateTemplateMaster

WHERE
    1=1
";

            List<SqlParameter> param =
                new List<SqlParameter>();


            //-----------------------------------------------------
            // Search Template Name
            //-----------------------------------------------------

            if
            (
                !String.IsNullOrWhiteSpace(
                txtSearchTemplate.Text)
            )
            {
                query +=
        @"
AND
    TemplateName LIKE @TemplateName
";

                param.Add(
                    new SqlParameter(
                        "@TemplateName",
                        "%" +
                        txtSearchTemplate.Text.Trim() +
                        "%"));
            }


            //-----------------------------------------------------
            // Search Status
            //-----------------------------------------------------

            if
            (
                !String.IsNullOrWhiteSpace(
                ddlSearchStatus.SelectedValue)
            )
            {
                query +=
        @"
AND
    Active=@Active
";

                param.Add(
                    new SqlParameter(
                        "@Active",
                        Convert.ToBoolean(
                            ddlSearchStatus.SelectedValue)));
            }


            //-----------------------------------------------------
            // Order
            //-----------------------------------------------------

            query +=
        @"
ORDER BY
    DisplayOrder,
    TemplateName
";


            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param.ToArray());


            gvTemplate.DataSource =
                dt;

            gvTemplate.DataBind();
        }


        //---------------------------------------------------------
        // Save Button
        //---------------------------------------------------------

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                lblMessage.Text = "";

                //-------------------------------------------------
                // Validation
                //-------------------------------------------------

                if (!ValidateTemplate())
                {
                    return;
                }


                //-------------------------------------------------
                // INSERT
                //-------------------------------------------------

                if
                (
                    String.IsNullOrWhiteSpace(
                        hfID.Value)
                )
                {
                    InsertTemplate();

                    lblMessage.ForeColor =
                        System.Drawing.Color.Green;

                    lblMessage.Text =
                        "Certificate template saved successfully.";
                }

                //-------------------------------------------------
                // UPDATE
                //-------------------------------------------------

                else
                {
                    UpdateTemplate();

                    lblMessage.ForeColor =
                        System.Drawing.Color.Green;

                    lblMessage.Text =
                        "Certificate template updated successfully.";
                }


                BindGrid();

                ResetForm();
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    ex.Message;
            }
        }


        //---------------------------------------------------------
        // Validate Template
        //---------------------------------------------------------

        private bool ValidateTemplate()
        {
            //-----------------------------------------------------
            // Template Name
            //-----------------------------------------------------

            if
            (
                String.IsNullOrWhiteSpace(
                    txtTemplateName.Text)
            )
            {
                ShowError(
                    "Template name is required.");

                return false;
            }


            //-----------------------------------------------------
            // Orientation
            //-----------------------------------------------------

            if
            (
                String.IsNullOrWhiteSpace(
                    ddlOrientation.SelectedValue)
            )
            {
                ShowError(
                    "Please select orientation.");

                return false;
            }


            //-----------------------------------------------------
            // Paper Size
            //-----------------------------------------------------

            if
            (
                String.IsNullOrWhiteSpace(
                    ddlPaperSize.SelectedValue)
            )
            {
                ShowError(
                    "Please select paper size.");

                return false;
            }


            //-----------------------------------------------------
            // Numeric fields
            //-----------------------------------------------------

            int displayOrder;

            if
            (
                !Int32.TryParse(
                    txtDisplayOrder.Text,
                    out displayOrder)
            )
            {
                ShowError(
                    "Display Order must be a valid number.");

                return false;
            }


            int pageWidth;

            if
            (
                !Int32.TryParse(
                    txtPageWidth.Text,
                    out pageWidth)
                ||
                pageWidth <= 0
            )
            {
                ShowError(
                    "Page Width must be greater than zero.");

                return false;
            }


            int pageHeight;

            if
            (
                !Int32.TryParse(
                    txtPageHeight.Text,
                    out pageHeight)
                ||
                pageHeight <= 0
            )
            {
                ShowError(
                    "Page Height must be greater than zero.");

                return false;
            }


            //-----------------------------------------------------
            // Font Sizes
            //-----------------------------------------------------

            if
            (
                !IsPositiveNumber(
                    txtCourseTitleFont.Text)
            )
            {
                ShowError(
                    "Course Title Font Size is invalid.");

                return false;
            }


            if
            (
                !IsPositiveNumber(
                    txtHeaderFont.Text)
            )
            {
                ShowError(
                    "Header Font Size is invalid.");

                return false;
            }


            if
            (
                !IsPositiveNumber(
                    txtFooterFont.Text)
            )
            {
                ShowError(
                    "Footer Font Size is invalid.");

                return false;
            }


            if
            (
                !IsPositiveNumber(
                    txtBodyFont.Text)
            )
            {
                ShowError(
                    "Body Font Size is invalid.");

                return false;
            }


            if
            (
                !IsPositiveNumber(
                    txtNameFont.Text)
            )
            {
                ShowError(
                    "Name Font Size is invalid.");

                return false;
            }


            //-----------------------------------------------------
            // Position fields
            //-----------------------------------------------------

            string positionError = "";

            if
            (
                !ValidatePositionFields(
                    out positionError)
            )
            {
                ShowError(
                    positionError);

                return false;
            }


            return true;
        }


        //---------------------------------------------------------
        // Validate Position Fields
        //---------------------------------------------------------

        private bool ValidatePositionFields(
            out string error)
        {
            error = "";


            //-----------------------------------------------------
            // Logo X
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtLogoX.Text)
            )
            {
                error =
                    "Logo X must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Logo Y
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtLogoY.Text)
            )
            {
                error =
                    "Logo Y must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Header Y
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtHeaderY.Text)
            )
            {
                error =
                    "Header Y must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Title Y
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtTitleY.Text)
            )
            {
                error =
                    "Title Y must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Body Y
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtBodyY.Text)
            )
            {
                error =
                    "Body Y must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Left Signature X
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtLeftSignatureX.Text)
            )
            {
                error =
                    "Left Signature X must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Right Signature X
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtRightSignatureX.Text)
            )
            {
                error =
                    "Right Signature X must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Signature Y
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtSignatureY.Text)
            )
            {
                error =
                    "Signature Y must be a valid number.";

                return false;
            }


            //-----------------------------------------------------
            // Footer Y
            //-----------------------------------------------------

            if
            (
                !IsNumber(
                    txtFooterY.Text)
            )
            {
                error =
                    "Footer Y must be a valid number.";

                return false;
            }


            return true;
        }


        //---------------------------------------------------------
        // Is Number
        //---------------------------------------------------------

        private bool IsNumber(
            string value)
        {
            decimal result;

            return Decimal.TryParse(
                value,
                out result);
        }


        //---------------------------------------------------------
        // Positive Number
        //---------------------------------------------------------

        private bool IsPositiveNumber(
            string value)
        {
            decimal result;

            return
                Decimal.TryParse(
                    value,
                    out result)
                &&
                result > 0;
        }


        //---------------------------------------------------------
        // Show Error
        //---------------------------------------------------------

        private void ShowError(
            string message)
        {
            lblMessage.ForeColor =
                System.Drawing.Color.Red;

            lblMessage.Text =
                message;
        }


        //---------------------------------------------------------
        // Insert Template
        //---------------------------------------------------------

        private void InsertTemplate()
        {
            string backgroundImage =
                UploadBackground();

            string logoImage =
                UploadLogo();


            string templateID =
                GenerateTemplateID();


            string query =
        @"
INSERT INTO
    CertificateTemplateMaster
(
    TemplateID,
    TemplateName,
    Description,
    DisplayOrder,
    Orientation,
    PaperSize,
    PageWidth,
    PageHeight,

    BackgroundImage,
    LogoImage,

    HeaderText,
    FooterText,

    CourseTitleFontSize,
    HeaderFontSize,
    FooterFontSize,
    BodyFontSize,
    NameFontSize,

    LogoX,
    LogoY,
    HeaderY,
    TitleY,
    BodyY,
    LeftSignatureX,
    RightSignatureX,
    SignatureY,
    FooterY,

    CreatedOn,
    CreatedBy,
    Active
)
VALUES
(
    @TemplateID,
    @TemplateName,
    @Description,
    @DisplayOrder,
    @Orientation,
    @PaperSize,
    @PageWidth,
    @PageHeight,

    @BackgroundImage,
    @LogoImage,

    @HeaderText,
    @FooterText,

    @CourseTitleFontSize,
    @HeaderFontSize,
    @FooterFontSize,
    @BodyFontSize,
    @NameFontSize,

    @LogoX,
    @LogoY,
    @HeaderY,
    @TitleY,
    @BodyY,
    @LeftSignatureX,
    @RightSignatureX,
    @SignatureY,
    @FooterY,

    GETDATE(),
    @CreatedBy,
    @Active
)
";


            SqlParameter[] param =
            {
                //-------------------------------------------------
                // Basic
                //-------------------------------------------------

                new SqlParameter(
                    "@TemplateID",
                    templateID),

                new SqlParameter(
                    "@TemplateName",
                    txtTemplateName.Text.Trim()),

                new SqlParameter(
                    "@Description",
                    String.IsNullOrWhiteSpace(
                        txtDescription.Text)
                    ?
                    (object)DBNull.Value
                    :
                    txtDescription.Text.Trim()),

                new SqlParameter(
                    "@DisplayOrder",
                    Convert.ToInt32(
                        txtDisplayOrder.Text)),

                new SqlParameter(
                    "@Orientation",
                    ddlOrientation.SelectedValue),

                new SqlParameter(
                    "@PaperSize",
                    ddlPaperSize.SelectedValue),

                new SqlParameter(
                    "@PageWidth",
                    Convert.ToInt32(
                        txtPageWidth.Text)),

                new SqlParameter(
                    "@PageHeight",
                    Convert.ToInt32(
                        txtPageHeight.Text)),


                //-------------------------------------------------
                // Images
                //-------------------------------------------------

                new SqlParameter(
                    "@BackgroundImage",
                    String.IsNullOrWhiteSpace(
                        backgroundImage)
                    ?
                    (object)DBNull.Value
                    :
                    backgroundImage),

                new SqlParameter(
                    "@LogoImage",
                    String.IsNullOrWhiteSpace(
                        logoImage)
                    ?
                    (object)DBNull.Value
                    :
                    logoImage),


                //-------------------------------------------------
                // Text
                //-------------------------------------------------

                new SqlParameter(
                    "@HeaderText",
                    String.IsNullOrWhiteSpace(
                        txtHeader.Text)
                    ?
                    (object)DBNull.Value
                    :
                    txtHeader.Text.Trim()),

                new SqlParameter(
                    "@FooterText",
                    String.IsNullOrWhiteSpace(
                        txtFooter.Text)
                    ?
                    (object)DBNull.Value
                    :
                    txtFooter.Text.Trim()),


                //-------------------------------------------------
                // Fonts
                //-------------------------------------------------

                new SqlParameter(
                    "@CourseTitleFontSize",
                    Convert.ToInt32(
                        txtCourseTitleFont.Text)),

                new SqlParameter(
                    "@HeaderFontSize",
                    Convert.ToInt32(
                        txtHeaderFont.Text)),

                new SqlParameter(
                    "@FooterFontSize",
                    Convert.ToInt32(
                        txtFooterFont.Text)),

                new SqlParameter(
                    "@BodyFontSize",
                    Convert.ToInt32(
                        txtBodyFont.Text)),

                new SqlParameter(
                    "@NameFontSize",
                    Convert.ToInt32(
                        txtNameFont.Text)),


                //-------------------------------------------------
                // Positions
                //-------------------------------------------------

                new SqlParameter(
                    "@LogoX",
                    Convert.ToDecimal(
                        txtLogoX.Text)),

                new SqlParameter(
                    "@LogoY",
                    Convert.ToDecimal(
                        txtLogoY.Text)),

                new SqlParameter(
                    "@HeaderY",
                    Convert.ToDecimal(
                        txtHeaderY.Text)),

                new SqlParameter(
                    "@TitleY",
                    Convert.ToDecimal(
                        txtTitleY.Text)),

                new SqlParameter(
                    "@BodyY",
                    Convert.ToDecimal(
                        txtBodyY.Text)),

                new SqlParameter(
                    "@LeftSignatureX",
                    Convert.ToDecimal(
                        txtLeftSignatureX.Text)),

                new SqlParameter(
                    "@RightSignatureX",
                    Convert.ToDecimal(
                        txtRightSignatureX.Text)),

                new SqlParameter(
                    "@SignatureY",
                    Convert.ToDecimal(
                        txtSignatureY.Text)),

                new SqlParameter(
                    "@FooterY",
                    Convert.ToDecimal(
                        txtFooterY.Text)),


                //-------------------------------------------------
                // Audit
                //-------------------------------------------------

                new SqlParameter(
                    "@CreatedBy",
                    Session["AdminID"] == null
                    ?
                    ""
                    :
                    Session["AdminID"].ToString()),

                new SqlParameter(
                    "@Active",
                    chkActive.Checked)
            };


            if
            (
                objDB.ExecuteSql(
                    query,
                    param) <= 0
            )
            {
                throw new Exception(
                    "Certificate template could not be saved.");
            }
        }


        //---------------------------------------------------------
        // Update Template
        //---------------------------------------------------------

        private void UpdateTemplate()
        {
            string oldBackground =
                "";

            string oldLogo =
                "";


            //-----------------------------------------------------
            // Get Existing Images
            //-----------------------------------------------------

            string query =
        @"
SELECT
    BackgroundImage,
    LogoImage
FROM
    CertificateTemplateMaster
WHERE
    TemplateID=@TemplateID
";


            SqlParameter[] getParam =
            {
                new SqlParameter(
                    "@TemplateID",
                    hfID.Value)
            };


            DataTable dt =
                objDB.GetDataTable(
                    query,
                    getParam);


            if
            (
                dt.Rows.Count == 0
            )
            {
                throw new Exception(
                    "Certificate template not found.");
            }


            oldBackground =
                dt.Rows[0]["BackgroundImage"]
                .ToString();

            oldLogo =
                dt.Rows[0]["LogoImage"]
                .ToString();


            //-----------------------------------------------------
            // Keep Old Images
            //-----------------------------------------------------

            string backgroundImage =
                oldBackground;

            string logoImage =
                oldLogo;


            //-----------------------------------------------------
            // Replace Background if uploaded
            //-----------------------------------------------------

            if
            (
                fuBackground.HasFile
            )
            {
                backgroundImage =
                    UploadBackground();
            }


            //-----------------------------------------------------
            // Replace Logo if uploaded
            //-----------------------------------------------------

            if
            (
                fuLogo.HasFile
            )
            {
                logoImage =
                    UploadLogo();
            }


            //-----------------------------------------------------
            // Update
            //-----------------------------------------------------

            query =
        @"
UPDATE
    CertificateTemplateMaster
SET

    TemplateName=@TemplateName,
    Description=@Description,
    DisplayOrder=@DisplayOrder,
    Orientation=@Orientation,
    PaperSize=@PaperSize,
    PageWidth=@PageWidth,
    PageHeight=@PageHeight,

    BackgroundImage=@BackgroundImage,
    LogoImage=@LogoImage,

    HeaderText=@HeaderText,
    FooterText=@FooterText,

    CourseTitleFontSize=@CourseTitleFontSize,
    HeaderFontSize=@HeaderFontSize,
    FooterFontSize=@FooterFontSize,
    BodyFontSize=@BodyFontSize,
    NameFontSize=@NameFontSize,

    LogoX=@LogoX,
    LogoY=@LogoY,
    HeaderY=@HeaderY,
    TitleY=@TitleY,
    BodyY=@BodyY,
    LeftSignatureX=@LeftSignatureX,
    RightSignatureX=@RightSignatureX,
    SignatureY=@SignatureY,
    FooterY=@FooterY,

    ModifiedOn=GETDATE(),
    ModifiedBy=@ModifiedBy,
    Active=@Active

WHERE
    TemplateID=@TemplateID
";


            SqlParameter[] param =
            {
                //-------------------------------------------------
                // ID
                //-------------------------------------------------

                new SqlParameter(
                    "@TemplateID",
                    hfID.Value),


                //-------------------------------------------------
                // Basic
                //-------------------------------------------------

                new SqlParameter(
                    "@TemplateName",
                    txtTemplateName.Text.Trim()),

                new SqlParameter(
                    "@Description",
                    String.IsNullOrWhiteSpace(
                        txtDescription.Text)
                    ?
                    (object)DBNull.Value
                    :
                    txtDescription.Text.Trim()),

                new SqlParameter(
                    "@DisplayOrder",
                    Convert.ToInt32(
                        txtDisplayOrder.Text)),

                new SqlParameter(
                    "@Orientation",
                    ddlOrientation.SelectedValue),

                new SqlParameter(
                    "@PaperSize",
                    ddlPaperSize.SelectedValue),

                new SqlParameter(
                    "@PageWidth",
                    Convert.ToInt32(
                        txtPageWidth.Text)),

                new SqlParameter(
                    "@PageHeight",
                    Convert.ToInt32(
                        txtPageHeight.Text)),


                //-------------------------------------------------
                // Images
                //-------------------------------------------------

                new SqlParameter(
                    "@BackgroundImage",
                    String.IsNullOrWhiteSpace(
                        backgroundImage)
                    ?
                    (object)DBNull.Value
                    :
                    backgroundImage),

                new SqlParameter(
                    "@LogoImage",
                    String.IsNullOrWhiteSpace(
                        logoImage)
                    ?
                    (object)DBNull.Value
                    :
                    logoImage),


                //-------------------------------------------------
                // Text
                //-------------------------------------------------

                new SqlParameter(
                    "@HeaderText",
                    String.IsNullOrWhiteSpace(
                        txtHeader.Text)
                    ?
                    (object)DBNull.Value
                    :
                    txtHeader.Text.Trim()),

                new SqlParameter(
                    "@FooterText",
                    String.IsNullOrWhiteSpace(
                        txtFooter.Text)
                    ?
                    (object)DBNull.Value
                    :
                    txtFooter.Text.Trim()),


                //-------------------------------------------------
                // Fonts
                //-------------------------------------------------

                new SqlParameter(
                    "@CourseTitleFontSize",
                    Convert.ToInt32(
                        txtCourseTitleFont.Text)),

                new SqlParameter(
                    "@HeaderFontSize",
                    Convert.ToInt32(
                        txtHeaderFont.Text)),

                new SqlParameter(
                    "@FooterFontSize",
                    Convert.ToInt32(
                        txtFooterFont.Text)),

                new SqlParameter(
                    "@BodyFontSize",
                    Convert.ToInt32(
                        txtBodyFont.Text)),

                new SqlParameter(
                    "@NameFontSize",
                    Convert.ToInt32(
                        txtNameFont.Text)),


                //-------------------------------------------------
                // Positions
                //-------------------------------------------------

                new SqlParameter(
                    "@LogoX",
                    Convert.ToDecimal(
                        txtLogoX.Text)),

                new SqlParameter(
                    "@LogoY",
                    Convert.ToDecimal(
                        txtLogoY.Text)),

                new SqlParameter(
                    "@HeaderY",
                    Convert.ToDecimal(
                        txtHeaderY.Text)),

                new SqlParameter(
                    "@TitleY",
                    Convert.ToDecimal(
                        txtTitleY.Text)),

                new SqlParameter(
                    "@BodyY",
                    Convert.ToDecimal(
                        txtBodyY.Text)),

                new SqlParameter(
                    "@LeftSignatureX",
                    Convert.ToDecimal(
                        txtLeftSignatureX.Text)),

                new SqlParameter(
                    "@RightSignatureX",
                    Convert.ToDecimal(
                        txtRightSignatureX.Text)),

                new SqlParameter(
                    "@SignatureY",
                    Convert.ToDecimal(
                        txtSignatureY.Text)),

                new SqlParameter(
                    "@FooterY",
                    Convert.ToDecimal(
                        txtFooterY.Text)),


                //-------------------------------------------------
                // Audit
                //-------------------------------------------------

                new SqlParameter(
                    "@ModifiedBy",
                    Session["AdminID"] == null
                    ?
                    ""
                    :
                    Session["AdminID"].ToString()),

                new SqlParameter(
                    "@Active",
                    chkActive.Checked)
            };


            if
            (
                objDB.ExecuteSql(
                    query,
                    param) <= 0
            )
            {
                throw new Exception(
                    "Certificate template could not be updated.");
            }
        }


        //---------------------------------------------------------
        // Search
        //---------------------------------------------------------

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }


        //---------------------------------------------------------
        // Reset Search
        //---------------------------------------------------------

        protected void btnResetSearch_Click(
            object sender,
            EventArgs e)
        {
            txtSearchTemplate.Text =
                "";

            ddlSearchStatus.SelectedIndex =
                0;

            BindGrid();
        }


        //---------------------------------------------------------
        // Reset
        //---------------------------------------------------------

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            ResetForm();
        }


        //---------------------------------------------------------
        // Grid Row Command
        //---------------------------------------------------------

        protected void gvTemplate_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            //-----------------------------------------------------
            // Edit
            //-----------------------------------------------------

            if
            (
                e.CommandName ==
                "EditRow"
            )
            {
                LoadTemplate1(
                    e.CommandArgument.ToString());

                return;
            }


            //-----------------------------------------------------
            // Change Status
            //-----------------------------------------------------

            if
            (
                e.CommandName ==
                "ChangeStatus"
            )
            {
                ChangeStatus(
                    e.CommandArgument.ToString());

                BindGrid();
            }
        }


        //---------------------------------------------------------
        // Load Template
        //---------------------------------------------------------

        private void LoadTemplate1(
            string templateID)
        {
            string query =
        @"
SELECT
    *
FROM
    CertificateTemplateMaster
WHERE
    TemplateID=@TemplateID
";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TemplateID",
                    templateID)
            };


            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);


            if
            (
                dt.Rows.Count == 0
            )
            {
                ShowError(
                    "Certificate template not found.");

                return;
            }


            DataRow dr =
                dt.Rows[0];


            //-----------------------------------------------------
            // Basic
            //-----------------------------------------------------

            hfID.Value =
                dr["TemplateID"]
                .ToString();

            txtTemplateName.Text =
                dr["TemplateName"]
                .ToString();

            txtDescription.Text =
                dr["Description"]
                .ToString();

            txtDisplayOrder.Text =
                dr["DisplayOrder"]
                .ToString();


            //-----------------------------------------------------
            // Page
            //-----------------------------------------------------

            ddlOrientation.SelectedValue =
                dr["Orientation"]
                .ToString();


            ddlPaperSize.SelectedValue =
                dr["PaperSize"]
                .ToString();


            txtPageWidth.Text =
                dr["PageWidth"]
                .ToString();

            txtPageHeight.Text =
                dr["PageHeight"]
                .ToString();


            //-----------------------------------------------------
            // Text
            //-----------------------------------------------------

            txtHeader.Text =
                dr["HeaderText"]
                .ToString();

            txtFooter.Text =
                dr["FooterText"]
                .ToString();


            //-----------------------------------------------------
            // Fonts
            //-----------------------------------------------------

            txtCourseTitleFont.Text =
                dr["CourseTitleFontSize"]
                .ToString();

            txtHeaderFont.Text =
                dr["HeaderFontSize"]
                .ToString();

            txtFooterFont.Text =
                dr["FooterFontSize"]
                .ToString();

            txtBodyFont.Text =
                dr["BodyFontSize"]
                .ToString();

            txtNameFont.Text =
                dr["NameFontSize"]
                .ToString();


            //-----------------------------------------------------
            // POSITION SETTINGS
            //-----------------------------------------------------

            txtLogoX.Text =
                dr["LogoX"]
                .ToString();

            txtLogoY.Text =
                dr["LogoY"]
                .ToString();

            txtHeaderY.Text =
                dr["HeaderY"]
                .ToString();

            txtTitleY.Text =
                dr["TitleY"]
                .ToString();

            txtBodyY.Text =
                dr["BodyY"]
                .ToString();

            txtLeftSignatureX.Text =
                dr["LeftSignatureX"]
                .ToString();

            txtRightSignatureX.Text =
                dr["RightSignatureX"]
                .ToString();

            txtSignatureY.Text =
                dr["SignatureY"]
                .ToString();

            txtFooterY.Text =
                dr["FooterY"]
                .ToString();


            //-----------------------------------------------------
            // Active
            //-----------------------------------------------------

            chkActive.Checked =
                Convert.ToBoolean(
                    dr["Active"]);


            //-----------------------------------------------------
            // Images
            //-----------------------------------------------------

            imgBackground.ImageUrl =
                dr["BackgroundImage"]
                .ToString();

            imgLogo.ImageUrl =
                dr["LogoImage"]
                .ToString();


            //-----------------------------------------------------
            // Message
            //-----------------------------------------------------

            lblMessage.Text =
                "Template loaded for editing.";

            lblMessage.ForeColor =
                System.Drawing.Color.Blue;
        }


        //---------------------------------------------------------
        // Change Status
        //---------------------------------------------------------

        private void ChangeStatus(
            string templateID)
        {
            string query =
        @"
UPDATE
    CertificateTemplateMaster
SET
    Active =
        CASE
            WHEN Active=1
            THEN 0
            ELSE 1
        END,

    ModifiedOn=GETDATE(),

    ModifiedBy=@ModifiedBy

WHERE
    TemplateID=@TemplateID
";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TemplateID",
                    templateID),

                new SqlParameter(
                    "@ModifiedBy",
                    Session["AdminID"] == null
                    ?
                    ""
                    :
                    Session["AdminID"].ToString())
            };


            if
            (
                objDB.ExecuteSql(
                    query,
                    param) <= 0
            )
            {
                ShowError(
                    "Template status could not be changed.");

                return;
            }


            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Template status changed successfully.";
        }


        //---------------------------------------------------------
        // Preview
        //---------------------------------------------------------

        protected void btnPreview_Click(
            object sender,
            EventArgs e)
        {
            if
            (
                String.IsNullOrWhiteSpace(
                    hfID.Value)
            )
            {
                ShowError(
                    "Please select a template first.");

                return;
            }


            Session["PreviewTemplate"] =
                hfID.Value;


            Response.Redirect(
                "CertificatePreview.aspx");
        }


        //---------------------------------------------------------
        // Generate Template ID
        //---------------------------------------------------------

        private string GenerateTemplateID()
        {
            string query =
        @"
SELECT
    ISNULL(MAX(ID),0)+1
FROM
    CertificateTemplateMaster
";


            int nextID =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        query));


            return
                "CTM" +
                nextID.ToString("0000");
        }


        //---------------------------------------------------------
        // Reset Form
        //---------------------------------------------------------

        private void ResetForm()
        {
            hfID.Value =
                "";

            txtTemplateName.Text =
                "";

            txtDescription.Text =
                "";

            txtDisplayOrder.Text =
                "1";


            //-----------------------------------------------------
            // Page
            //-----------------------------------------------------

            ddlOrientation.SelectedIndex =
                0;

            ddlPaperSize.SelectedValue =
                "A4";

            txtPageWidth.Text =
                "";

            txtPageHeight.Text =
                "";


            //-----------------------------------------------------
            // Text
            //-----------------------------------------------------

            txtHeader.Text =
                "";

            txtFooter.Text =
                "";


            //-----------------------------------------------------
            // Fonts
            //-----------------------------------------------------

            txtCourseTitleFont.Text =
                "26";

            txtHeaderFont.Text =
                "18";

            txtFooterFont.Text =
                "12";

            txtBodyFont.Text =
                "16";

            txtNameFont.Text =
                "28";


            //-----------------------------------------------------
            // Positions
            //-----------------------------------------------------

            txtLogoX.Text =
                "50";

            txtLogoY.Text =
                "700";

            txtHeaderY.Text =
                "730";

            txtTitleY.Text =
                "650";

            txtBodyY.Text =
                "520";

            txtLeftSignatureX.Text =
                "180";

            txtRightSignatureX.Text =
                "650";

            txtSignatureY.Text =
                "150";

            txtFooterY.Text =
                "50";


            //-----------------------------------------------------
            // Active
            //-----------------------------------------------------

            chkActive.Checked =
                true;


            //-----------------------------------------------------
            // Images
            //-----------------------------------------------------

            imgBackground.ImageUrl =
                "";

            imgLogo.ImageUrl =
                "";


            //-----------------------------------------------------
            // Message
            //-----------------------------------------------------

            lblMessage.Text =
                "";
        }


        //---------------------------------------------------------
        // Upload Background
        //---------------------------------------------------------

        private string UploadBackground()
        {
            if
            (
                !fuBackground.HasFile
            )
            {
                return "";
            }


            string extension =
                Path.GetExtension(
                    fuBackground.FileName)
                .ToLower();


            if
            (
                extension != ".jpg"
                &&
                extension != ".jpeg"
                &&
                extension != ".png"
            )
            {
                throw new Exception(
                    "Only JPG, JPEG and PNG background images are allowed.");
            }


            if
            (
                fuBackground.PostedFile.ContentLength
                >
                2 * 1024 * 1024
            )
            {
                throw new Exception(
                    "Background image size should not exceed 2 MB.");
            }


            string folder =
                Server.MapPath(
                    "~/Uploads/CertificateTemplate/Background/");


            if
            (
                !Directory.Exists(
                    folder)
            )
            {
                Directory.CreateDirectory(
                    folder);
            }


            string fileName =
                Guid.NewGuid()
                .ToString("N")
                +
                extension;


            fuBackground.SaveAs(
                Path.Combine(
                    folder,
                    fileName));


            return
                "~/Uploads/CertificateTemplate/Background/"
                +
                fileName;
        }


        //---------------------------------------------------------
        // Upload Logo
        //---------------------------------------------------------

        private string UploadLogo()
        {
            if
            (
                !fuLogo.HasFile
            )
            {
                return "";
            }


            string extension =
                Path.GetExtension(
                    fuLogo.FileName)
                .ToLower();


            if
            (
                extension != ".jpg"
                &&
                extension != ".jpeg"
                &&
                extension != ".png"
            )
            {
                throw new Exception(
                    "Only JPG, JPEG and PNG logo images are allowed.");
            }


            if
            (
                fuLogo.PostedFile.ContentLength
                >
                2 * 1024 * 1024
            )
            {
                throw new Exception(
                    "Logo image size should not exceed 2 MB.");
            }


            string folder =
                Server.MapPath(
                    "~/Uploads/CertificateTemplate/Logo/");


            if
            (
                !Directory.Exists(
                    folder)
            )
            {
                Directory.CreateDirectory(
                    folder);
            }


            string fileName =
                Guid.NewGuid()
                .ToString("N")
                +
                extension;


            fuLogo.SaveAs(
                Path.Combine(
                    folder,
                    fileName));


            return
                "~/Uploads/CertificateTemplate/Logo/"
                +
                fileName;
        }


        //---------------------------------------------------------
        // Orientation Changed
        //---------------------------------------------------------

        protected void ddlOrientation_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            if
            (
                ddlOrientation.SelectedValue ==
                "Landscape"
            )
            {
                //-------------------------------------------------
                // A4 Landscape
                //-------------------------------------------------

                if
                (
                    ddlPaperSize.SelectedValue ==
                    "A4"
                )
                {
                    txtPageWidth.Text =
                        "842";

                    txtPageHeight.Text =
                        "595";
                }
            }
            else if
            (
                ddlOrientation.SelectedValue ==
                "Portrait"
            )
            {
                //-------------------------------------------------
                // A4 Portrait
                //-------------------------------------------------

                if
                (
                    ddlPaperSize.SelectedValue ==
                    "A4"
                )
                {
                    txtPageWidth.Text =
                        "595";

                    txtPageHeight.Text =
                        "842";
                }
            }
        }
    }
}