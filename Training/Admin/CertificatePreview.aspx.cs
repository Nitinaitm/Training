using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CertificatePreview :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                if
                (
                    Request.QueryString["TrainingID"]
                    ==
                    null
                )
                {
                    Response.Redirect(
                        "ManageTraining.aspx");

                    return;
                }

                LoadPreview();
                ApplyTemplate();
                AdjustLayout();
            }
        }

        //---------------------------------------------------
        // Load Preview
        //---------------------------------------------------

        private void LoadPreview()
        {
            string query =
@"
SELECT
TCT.CourseTitle,
TCT.LeftSignature,
TCT.LeftName,
TCT.LeftDesignation,
TCT.RightSignature,
TCT.RightName,
TCT.RightDesignation,
CTM.TemplateName,
CTM.HeaderText,
CTM.FooterText,
CTM.BackgroundImage,
CTM.LogoImage,
CTM.HeaderFontSize,
CTM.FooterFontSize,
CTM.TitleFontSize,
CTM.BodyFontSize,
CTM.Orientation,
CTM.PaperSize,
TD.DateFrom,
TD.DateTo,
CM.CourseName
FROM
TrainingCertificateTemplate TCT
INNER JOIN
CertificateTemplateMaster CTM
ON
TCT.TemplateID=CTM.TemplateID
INNER JOIN
TrainingDetails TD
ON
TCT.TrainingID=TD.TrainingID
INNER JOIN
CourseMaster CM
ON
TD.CourseID=CM.CourseID
WHERE
TCT.TrainingID=@TrainingID
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    Request.QueryString["TrainingID"])
            };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblHeader.Text =
                dr["HeaderText"]
                .ToString();

            lblFooter.Text =
                dr["FooterText"]
                .ToString();

            lblTitle.Text =
    dr["TemplateName"]
    .ToString();

            lblEmployee.Text =
                "Sample Trainee";

            if
 (
     String.IsNullOrWhiteSpace(
     dr["CourseTitle"]
     .ToString())
 )
            {
                lblCourse.Text =
                    dr["CourseName"]
                    .ToString();
            }
            else
            {
                lblCourse.Text =
                    dr["CourseTitle"]
                    .ToString();
            }
            lblDuration.Text =
                Convert.ToDateTime(
                dr["DateFrom"])
                .ToString("dd-MMM-yyyy")
                +
                " To "
                +
                Convert.ToDateTime(
                dr["DateTo"])
                .ToString("dd-MMM-yyyy");

            lblLeftName.Text =
                dr["LeftName"]
                .ToString();

            lblLeftDesignation.Text =
                dr["LeftDesignation"]
                .ToString();

            lblRightName.Text =
                dr["RightName"]
                .ToString();

            lblRightDesignation.Text =
                dr["RightDesignation"]
                .ToString();

            imgLeftSignature.ImageUrl =
                dr["LeftSignature"]
                .ToString();

            imgRightSignature.ImageUrl =
                dr["RightSignature"]
                .ToString();

            imgLogo.ImageUrl =
                dr["LogoImage"]
                .ToString();

            ViewState["BackgroundImage"] =
                dr["BackgroundImage"]
                .ToString();

            ViewState["Orientation"] =
                dr["Orientation"]
                .ToString();

            ViewState["PaperSize"] =
                dr["PaperSize"]
                .ToString();

            ViewState["HeaderFont"] =
                dr["HeaderFontSize"];

            ViewState["FooterFont"] =
                dr["FooterFontSize"];

            ViewState["TitleFont"] =
                dr["TitleFontSize"];

            ViewState["BodyFont"] =
                dr["BodyFontSize"];
        }

        //---------------------------------------------------
        // Back
        //---------------------------------------------------

        protected void btnBack_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "CertificateTemplate.aspx?TrainingID="
                +
                Request.QueryString["TrainingID"]);
        }
        //---------------------------------------------------
        // Apply Template
        //---------------------------------------------------

        private void ApplyTemplate()
        {
            ApplyBackground();

            ApplyOrientation();

            ApplyFont();

            ApplyTitle();
        }
        //---------------------------------------------------
        // Apply Background
        //---------------------------------------------------

        private void ApplyBackground()
        {
            string background =
                Convert.ToString(
                ViewState["BackgroundImage"]);

            if
            (
                String.IsNullOrWhiteSpace(
                background)
            )
            {
                return;
            }

            divCertificate.Style["background-image"] =
                "url('" +
                ResolveUrl(background) +
                "')";

            divCertificate.Style["background-repeat"] =
                "no-repeat";

            divCertificate.Style["background-size"] =
                "100% 100%";

            divCertificate.Style["background-position"] =
                "center";
        }
        //---------------------------------------------------
        // Apply Orientation
        //---------------------------------------------------

        private void ApplyOrientation()
        {
            string orientation =
                Convert.ToString(
                ViewState["Orientation"]);

            string paperSize =
                Convert.ToString(
                ViewState["PaperSize"]);

            if
            (
                paperSize
                ==
                "A4"
            )
            {
                if
                (
                    orientation
                    ==
                    "Landscape"
                )
                {
                    divCertificate.Style["width"] =
                        "1123px";

                    divCertificate.Style["height"] =
                        "794px";
                }
                else
                {
                    divCertificate.Style["width"] =
                        "794px";

                    divCertificate.Style["height"] =
                        "1123px";
                }
            }
            else
            {
                if
                (
                    orientation
                    ==
                    "Landscape"
                )
                {
                    divCertificate.Style["width"] =
                        "1200px";

                    divCertificate.Style["height"] =
                        "850px";
                }
                else
                {
                    divCertificate.Style["width"] =
                        "850px";

                    divCertificate.Style["height"] =
                        "1200px";
                }
            }
        }
        //---------------------------------------------------
        // Apply Font
        //---------------------------------------------------

        private void ApplyFont()
        {
            lblHeader.Font.Size =
                FontUnit.Point(
                Convert.ToInt32(
                ViewState["HeaderFont"]));

            lblFooter.Font.Size =
                FontUnit.Point(
                Convert.ToInt32(
                ViewState["FooterFont"]));

            lblTitle.Font.Size =
                FontUnit.Point(
                Convert.ToInt32(
                ViewState["TitleFont"]));

            lblCourse.Font.Size =
                FontUnit.Point(
                Convert.ToInt32(
                ViewState["BodyFont"]));

            lblEmployee.Font.Size =
                FontUnit.Point(
                Convert.ToInt32(
                ViewState["BodyFont"])
                +
                8);

            lblDuration.Font.Size =
                FontUnit.Point(
                Convert.ToInt32(
                ViewState["BodyFont"]));
        }
        //---------------------------------------------------
        // Apply Title
        //---------------------------------------------------

        private void ApplyTitle()
        {
            lblTitle.Font.Bold =
                true;

            lblEmployee.Font.Bold =
                true;

            lblCourse.Font.Bold =
                true;

            lblHeader.Font.Bold =
                true;

            lblFooter.Font.Bold =
                true;

            lblLeftName.Font.Bold =
                true;

            lblRightName.Font.Bold =
                true;
        }
        //---------------------------------------------------
        // Adjust Layout
        //---------------------------------------------------

        private void AdjustLayout()
        {
            ToggleLogo();

            ToggleSignature();

            ApplyPaperMargin();
        }
        //---------------------------------------------------
        // Toggle Logo
        //---------------------------------------------------

        private void ToggleLogo()
        {
            if
            (
                String.IsNullOrWhiteSpace(
                imgLogo.ImageUrl)
            )
            {
                imgLogo.Visible =
                    false;
            }
            else
            {
                imgLogo.Visible =
                    true;
            }
        }
        //---------------------------------------------------
        // Toggle Signature
        //---------------------------------------------------

        private void ToggleSignature()
        {
            imgLeftSignature.Visible =
                !String.IsNullOrWhiteSpace(
                imgLeftSignature.ImageUrl);

            imgRightSignature.Visible =
                !String.IsNullOrWhiteSpace(
                imgRightSignature.ImageUrl);

            lblLeftName.Visible =
                !String.IsNullOrWhiteSpace(
                lblLeftName.Text);

            lblRightName.Visible =
                !String.IsNullOrWhiteSpace(
                lblRightName.Text);

            lblLeftDesignation.Visible =
                !String.IsNullOrWhiteSpace(
                lblLeftDesignation.Text);

            lblRightDesignation.Visible =
                !String.IsNullOrWhiteSpace(
                lblRightDesignation.Text);
        }
        //---------------------------------------------------
        // Apply Paper Margin
        //---------------------------------------------------

        private void ApplyPaperMargin()
        {
            divCertificate.Style["padding"] =
                "20px";

            divCertificate.Style["box-sizing"] =
                "border-box";
        }
    }
}