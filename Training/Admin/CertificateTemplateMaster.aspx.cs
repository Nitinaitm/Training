using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;

namespace Training.Admin
{
    public partial class CertificateTemplateMaster :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["AdminID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                BindGrid();

                ResetForm();
            }
        }

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
CreatedOn,
Active
FROM
CertificateTemplateMaster
WHERE
1=1
";

            List<SqlParameter> param =
                new List<SqlParameter>();

            if
            (
                !String.IsNullOrWhiteSpace(
                txtSearchTemplate.Text)
            )
            {
                query +=
        @"
AND
TemplateName
LIKE
@TemplateName
";

                param.Add(
                    new SqlParameter(
                    "@TemplateName",
                    "%"
                    +
                    txtSearchTemplate.Text.Trim()
                    +
                    "%"));
            }

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

            query +=
        @"
ORDER BY
DisplayOrder,
TemplateName
";

            gvTemplate.DataSource =
                objDB.GetDataTable(
                query,
                param.ToArray());

            gvTemplate.DataBind();
        }

        protected void btnPreview_Click(
        object sender,
        EventArgs e)
        {
            Session["PreviewTemplate"] =
                hfID.Value;

            Response.Redirect(
                "CertificatePreview.aspx");
        }
        //-----------------------------------------------------
        // Generate Template ID
        //-----------------------------------------------------

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
                nextID
                .ToString("0000");
        }

        //-----------------------------------------------------
        // Reset Form
        //-----------------------------------------------------

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

            ddlOrientation.SelectedIndex =
                0;

            ddlPaperSize.SelectedValue =
                "A4";

            txtPageWidth.Text =
                "";

            txtPageHeight.Text =
                "";

            txtHeader.Text =
                "";

            txtFooter.Text =
                "";

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

            chkActive.Checked =
                true;

            imgBackground.ImageUrl =
                "";

            imgLogo.ImageUrl =
                "";

            lblMessage.Text =
                "";
        }

        //-----------------------------------------------------
        // Upload Background
        //-----------------------------------------------------

        private string UploadBackground()
        {
            if (!fuBackground.HasFile)
            {
                return "";
            }

            string extension =
                Path.GetExtension(
                fuBackground.FileName)
                .ToLower();

            if (
                extension != ".jpg"
                &&
                extension != ".jpeg"
                &&
                extension != ".png")
            {
                throw new Exception(
                "Only JPG, JPEG and PNG background images are allowed.");
            }

            if
            (
                fuBackground.PostedFile.ContentLength
                >
                2
                *
                1024
                *
                1024
            )
            {
                throw new Exception(
                "Background image size should not exceed 2 MB.");
            }

            string folder =
                Server.MapPath(
                "~/Uploads/CertificateTemplate/Background/");

            if (!Directory.Exists(folder))
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

        //-----------------------------------------------------
        // Upload Logo
        //-----------------------------------------------------

        private string UploadLogo()
        {
            if (!fuLogo.HasFile)
            {
                return "";
            }

            string extension =
                Path.GetExtension(
                fuLogo.FileName)
                .ToLower();

            if (
                extension != ".jpg"
                &&
                extension != ".jpeg"
                &&
                extension != ".png")
            {
                throw new Exception(
                "Only JPG, JPEG and PNG logo images are allowed.");
            }

            if
            (
                fuLogo.PostedFile.ContentLength
                >
                2
                *
                1024
                *
                1024
            )
            {
                throw new Exception(
                "Logo image size should not exceed 2 MB.");
            }

            string folder =
                Server.MapPath(
                "~/Uploads/CertificateTemplate/Logo/");

            if (!Directory.Exists(folder))
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
        //-----------------------------------------------------
        // Save
        //-----------------------------------------------------

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if
                (
                    txtTemplateName.Text.Trim()
                    ==
                    ""
                )
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please enter Template Name.";

                    txtTemplateName.Focus();

                    return;
                }

                if
                (
                    ddlOrientation.SelectedIndex
                    ==
                    0
                )
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please select Orientation.";

                    ddlOrientation.Focus();

                    return;
                }
                if
(
    ddlPaperSize.SelectedIndex
    ==
    0
)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please select Paper Size.";

                    ddlPaperSize.Focus();

                    return;
                }
                if
                (
                    txtPageWidth.Text.Trim()
                    ==
                    ""
                )
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please enter Page Width.";

                    txtPageWidth.Focus();

                    return;
                }
                if
                (
                    txtPageHeight.Text.Trim()
                    ==
                    ""
                )
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please enter Page Height.";

                    txtPageHeight.Focus();

                    return;
                }
                if (IsDuplicateTemplate())
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Template Name already exists.";

                    txtTemplateName.Focus();

                    return;
                }

                if (!ValidateNumbers())
                {
                    return;
                }

                if
                (
                    hfID.Value
                    ==
                    ""
                )
                {
                    InsertTemplate();

                    lblMessage.ForeColor =
                        System.Drawing.Color.Green;

                    lblMessage.Text =
                        "Template saved successfully.";
                }
                else
                {
                    UpdateTemplate();

                    lblMessage.ForeColor =
                        System.Drawing.Color.Green;

                    lblMessage.Text =
                        "Template updated successfully.";
                }

                ResetForm();

                BindGrid();
            }
            catch
            (
                Exception ex
            )
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    ex.Message;
            }
        }

        protected void ddlOrientation_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            if
            (
                ddlOrientation.SelectedValue
                ==
                "Landscape"
            )
            {
                txtPageWidth.Text =
                    "1123";

                txtPageHeight.Text =
                    "794";
            }

            if
            (
                ddlOrientation.SelectedValue
                ==
                "Portrait"
            )
            {
                txtPageWidth.Text =
                    "794";

                txtPageHeight.Text =
                    "1123";
            }
        }

        private bool ValidateNumbers()
        {
            int value;

            if
            (
                !int.TryParse(
                txtDisplayOrder.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Display Order.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtPageWidth.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Page Width.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtPageHeight.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Page Height.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtCourseTitleFont.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Course Title Font.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtHeaderFont.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Header Font.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtFooterFont.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Footer Font.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtBodyFont.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Body Font.";

                return false;
            }

            if
            (
                !int.TryParse(
                txtNameFont.Text,
                out value)
            )
            {
                lblMessage.Text =
                    "Invalid Name Font.";

                return false;
            }

            lblMessage.ForeColor =
                System.Drawing.Color.Red;

            return true;
        }
        //-----------------------------------------------------
        // Insert Template
        //-----------------------------------------------------

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
GETDATE(),
@CreatedBy,
@Active
)
";

            SqlParameter[] param =
            {
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

            objDB.ExecuteSql(
                query,
                param);
        }
        //-----------------------------------------------------
        // Update Template
        //-----------------------------------------------------

        private void UpdateTemplate()
        {
            string oldBackground =
                "";

            string oldLogo =
                "";

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

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TemplateID",
            hfID.Value)
    };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            if (dt.Rows.Count > 0)
            {
                oldBackground =
                    dt.Rows[0]["BackgroundImage"]
                    .ToString();

                oldLogo =
                    dt.Rows[0]["LogoImage"]
                    .ToString();
            }

            string backgroundImage =
                oldBackground;

            string logoImage =
                oldLogo;

            if (fuBackground.HasFile)
            {
                backgroundImage =
                    UploadBackground();
            }

            if (fuLogo.HasFile)
            {
                logoImage =
                    UploadLogo();
            }

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
ModifiedOn=GETDATE(),
ModifiedBy=@ModifiedBy,
Active=@Active
WHERE
TemplateID=@TemplateID
";

            param =
            new SqlParameter[]
            {
        new SqlParameter(
            "@TemplateID",
            hfID.Value),

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

            objDB.ExecuteSql(
                query,
                param);
        }

        private bool IsDuplicateTemplate()
        {
            string query =
        @"
SELECT
COUNT(*)
FROM
CertificateTemplateMaster
WHERE
TemplateName=@TemplateName
AND
TemplateID<>@TemplateID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TemplateName",
            txtTemplateName.Text.Trim()),

        new SqlParameter(
            "@TemplateID",
            hfID.Value == ""
            ?
            "-1"
            :
            hfID.Value)
    };

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param));

            return count > 0;
        }

        protected void gvTemplate_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if
            (
                e.CommandName
                ==
                "EditTemplate"
            )
            {
                LoadTemplate(
                    e.CommandArgument.ToString());
            }
        }

        private void LoadTemplate(
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

            DataTable dt =
                objDB.GetDataTable(
                query,
                new SqlParameter[]
                {
            new SqlParameter(
                "@TemplateID",
                templateID)
                });

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr =
                dt.Rows[0];

            hfID.Value =
                dr["TemplateID"].ToString();

            txtTemplateName.Text =
                dr["TemplateName"].ToString();

            txtDescription.Text =
                dr["Description"].ToString();

            txtDisplayOrder.Text =
                dr["DisplayOrder"].ToString();

            ddlOrientation.SelectedValue =
                dr["Orientation"].ToString();

            ddlPaperSize.SelectedValue =
                dr["PaperSize"].ToString();

            txtPageWidth.Text =
                dr["PageWidth"].ToString();

            txtPageHeight.Text =
                dr["PageHeight"].ToString();

            txtHeader.Text =
                dr["HeaderText"].ToString();

            txtFooter.Text =
                dr["FooterText"].ToString();

            txtCourseTitleFont.Text =
                dr["CourseTitleFontSize"].ToString();

            txtHeaderFont.Text =
                dr["HeaderFontSize"].ToString();

            txtFooterFont.Text =
                dr["FooterFontSize"].ToString();

            txtBodyFont.Text =
                dr["BodyFontSize"].ToString();

            txtNameFont.Text =
                dr["NameFontSize"].ToString();

            chkActive.Checked =
                Convert.ToBoolean(
                dr["Active"]);

            imgBackground.ImageUrl =
                dr["BackgroundImage"].ToString();

            imgLogo.ImageUrl =
                dr["LogoImage"].ToString();
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }

        protected void btnClearSearch_Click(
            object sender,
            EventArgs e)
        {
            txtSearchTemplate.Text =
                "";

            ddlSearchStatus.SelectedIndex =
                0;

            BindGrid();
        }
    }
}
