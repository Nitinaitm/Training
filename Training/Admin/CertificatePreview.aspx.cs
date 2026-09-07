using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CertificatePreview : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminID"] == null && Session["UserID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                string trainingID = Request.QueryString["TrainingID"];
                if (String.IsNullOrWhiteSpace(trainingID))
                {
                    Response.Redirect("ManageTraining.aspx");
                    return;
                }

                if (!LoadPreview(trainingID)) return;
                ApplyTemplate();
            }
        }

        private bool LoadPreview(string trainingID)
        {
            string sql = @"
SELECT TCT.CourseTitle,TCT.LeftSignature,TCT.LeftName,TCT.LeftDesignation,
TCT.RightSignature,TCT.RightName,TCT.RightDesignation,
CTM.TemplateName,CTM.HeaderText,CTM.FooterText,CTM.BackgroundImage,CTM.LogoImage,
CTM.CourseTitleFontSize,CTM.HeaderFontSize,CTM.FooterFontSize,CTM.BodyFontSize,CTM.NameFontSize,
CTM.LogoX,CTM.LogoY,CTM.HeaderY,CTM.TitleY,CTM.BodyY,
CTM.LeftSignatureX,CTM.RightSignatureX,CTM.SignatureY,CTM.FooterY,
CTM.Orientation,CTM.PaperSize,TD.DateFrom,TD.DateTo,CM.CourseName
FROM TrainingCertificateTemplate TCT
INNER JOIN CertificateTemplateMaster CTM ON TCT.TemplateID=CTM.TemplateID
INNER JOIN TrainingDetails TD ON TCT.TrainingID=TD.TrainingID
INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID
WHERE TCT.TrainingID=@TrainingID AND CTM.Active=1";

            DataTable dt = objDB.GetDataTable(sql, new SqlParameter("@TrainingID", trainingID));
            if (dt.Rows.Count == 0)
            {
                Response.Write("Certificate template is not configured for this training.");
                return false;
            }

            DataRow dr = dt.Rows[0];
            lblHeader.Text = dr["HeaderText"].ToString();
            lblFooter.Text = dr["FooterText"].ToString();
            lblTitle.Text = dr["TemplateName"].ToString();
            lblEmployee.Text = "Sample Trainee";
            lblCourse.Text = String.IsNullOrWhiteSpace(dr["CourseTitle"].ToString()) ? dr["CourseName"].ToString() : dr["CourseTitle"].ToString();
            lblDuration.Text = Convert.ToDateTime(dr["DateFrom"]).ToString("dd-MMM-yyyy") + " To " + Convert.ToDateTime(dr["DateTo"]).ToString("dd-MMM-yyyy");
            lblLeftName.Text = dr["LeftName"].ToString();
            lblLeftDesignation.Text = dr["LeftDesignation"].ToString();
            lblRightName.Text = dr["RightName"].ToString();
            lblRightDesignation.Text = dr["RightDesignation"].ToString();
            imgLeftSignature.ImageUrl = dr["LeftSignature"].ToString();
            imgRightSignature.ImageUrl = dr["RightSignature"].ToString();
            imgLogo.ImageUrl = dr["LogoImage"].ToString();

            ViewState["BackgroundImage"] = dr["BackgroundImage"].ToString();
            ViewState["Orientation"] = dr["Orientation"].ToString();
            ViewState["PaperSize"] = dr["PaperSize"].ToString();
            ViewState["HeaderFont"] = dr["HeaderFontSize"];
            ViewState["FooterFont"] = dr["FooterFontSize"];
            ViewState["TitleFont"] = dr["CourseTitleFontSize"];
            ViewState["BodyFont"] = dr["BodyFontSize"];
            ViewState["NameFont"] = dr["NameFontSize"];
            ViewState["LogoX"] = dr["LogoX"];
            ViewState["LogoY"] = dr["LogoY"];
            ViewState["HeaderY"] = dr["HeaderY"];
            ViewState["TitleY"] = dr["TitleY"];
            ViewState["BodyY"] = dr["BodyY"];
            ViewState["LeftSignatureX"] = dr["LeftSignatureX"];
            ViewState["RightSignatureX"] = dr["RightSignatureX"];
            ViewState["SignatureY"] = dr["SignatureY"];
            ViewState["FooterY"] = dr["FooterY"];
            return true;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("CertificateTemplate.aspx?TrainingID=" + Server.UrlEncode(Request.QueryString["TrainingID"]));
        }

        private void ApplyTemplate()
        {
            ApplyBackground();
            ApplyOrientation();
            ApplyFont();
            ApplyPositions();
            ToggleLogo();
            ToggleSignature();
        }

        private void ApplyBackground()
        {
            string b = Convert.ToString(ViewState["BackgroundImage"]);
            if (String.IsNullOrWhiteSpace(b)) return;
            divCertificate.Style["background-image"] = "url('" + ResolveUrl(b) + "')";
            divCertificate.Style["background-repeat"] = "no-repeat";
            divCertificate.Style["background-size"] = "100% 100%";
            divCertificate.Style["background-position"] = "center";
        }

        private void ApplyOrientation()
        {
            string o = Convert.ToString(ViewState["Orientation"]);
            string p = Convert.ToString(ViewState["PaperSize"]);
            if (p == "A4")
            {
                divCertificate.Style["width"] = o == "Landscape" ? "1123px" : "794px";
                divCertificate.Style["height"] = o == "Landscape" ? "794px" : "1123px";
            }
            else
            {
                divCertificate.Style["width"] = o == "Landscape" ? "1200px" : "850px";
                divCertificate.Style["height"] = o == "Landscape" ? "850px" : "1200px";
            }
        }

        private int Px(object value, int fallback)
        {
            if (value == null || value == DBNull.Value) return fallback;
            decimal d;
            return Decimal.TryParse(value.ToString(), out d) ? Convert.ToInt32(d) : fallback;
        }

        private void ApplyPositions()
        {
            divCertificate.Style["padding"] = "0";
            divCertificate.Style["box-sizing"] = "border-box";

            imgLogo.Style["left"] = Px(ViewState["LogoX"], 516) + "px";
            imgLogo.Style["top"] = Px(ViewState["LogoY"], 35) + "px";
            imgLogo.Style["transform"] = "none";

            lblHeader.Style["top"] = Px(ViewState["HeaderY"], 130) + "px";
            lblTitle.Style["top"] = Px(ViewState["TitleY"], 220) + "px";
            divBody.Style["top"] = Px(ViewState["BodyY"], 300) + "px";
            divLeftSignature.Style["left"] = Px(ViewState["LeftSignatureX"], 80) + "px";
            divRightSignature.Style["left"] = Px(ViewState["RightSignatureX"], 823) + "px";
            divRightSignature.Style["right"] = "auto";
            divLeftSignature.Style["bottom"] = "auto";
            divRightSignature.Style["bottom"] = "auto";
            divLeftSignature.Style["top"] = Px(ViewState["SignatureY"], 574) + "px";
            divRightSignature.Style["top"] = Px(ViewState["SignatureY"], 574) + "px";
            lblFooter.Style["top"] = Px(ViewState["FooterY"], 754) + "px";
            lblFooter.Style["bottom"] = "auto";
        }

        private void ApplyFont()
        {
            lblHeader.Font.Size = FontUnit.Point(Px(ViewState["HeaderFont"], 20));
            lblFooter.Font.Size = FontUnit.Point(Px(ViewState["FooterFont"], 12));
            lblTitle.Font.Size = FontUnit.Point(Px(ViewState["TitleFont"], 24));
            lblCourse.Font.Size = FontUnit.Point(Px(ViewState["BodyFont"], 18));
            lblEmployee.Font.Size = FontUnit.Point(Px(ViewState["NameFont"], 36));
            lblDuration.Font.Size = FontUnit.Point(Px(ViewState["BodyFont"], 18));
        }

        private void ToggleLogo()
        {
            imgLogo.Visible = !String.IsNullOrWhiteSpace(imgLogo.ImageUrl);
        }

        private void ToggleSignature()
        {
            imgLeftSignature.Visible = !String.IsNullOrWhiteSpace(imgLeftSignature.ImageUrl);
            imgRightSignature.Visible = !String.IsNullOrWhiteSpace(imgRightSignature.ImageUrl);
            divLeftSignature.Visible = !String.IsNullOrWhiteSpace(lblLeftName.Text) || imgLeftSignature.Visible;
            divRightSignature.Visible = !String.IsNullOrWhiteSpace(lblRightName.Text) || imgRightSignature.Visible;
        }
    }
}