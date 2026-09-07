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
            if (!IsPostBack)
            {
                string trainingID = Request.QueryString["TrainingID"];
                if (String.IsNullOrWhiteSpace(trainingID)) { Response.Redirect("ManageTraining.aspx"); return; }
                if (!LoadPreview(trainingID)) return;
                ApplyTemplate();
                AdjustLayout();
            }
        }

        private bool LoadPreview(string trainingID)
        {
            string sql=@"
SELECT TCT.CourseTitle,TCT.LeftSignature,TCT.LeftName,TCT.LeftDesignation,TCT.RightSignature,TCT.RightName,TCT.RightDesignation,
CTM.TemplateName,CTM.HeaderText,CTM.FooterText,CTM.BackgroundImage,CTM.LogoImage,
CTM.CourseTitleFontSize,CTM.HeaderFontSize,CTM.FooterFontSize,CTM.BodyFontSize,CTM.NameFontSize,
CTM.Orientation,CTM.PaperSize,TD.DateFrom,TD.DateTo,CM.CourseName
FROM TrainingCertificateTemplate TCT
INNER JOIN CertificateTemplateMaster CTM ON TCT.TemplateID=CTM.TemplateID
INNER JOIN TrainingDetails TD ON TCT.TrainingID=TD.TrainingID
INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID
WHERE TCT.TrainingID=@TrainingID AND CTM.Active=1";
            DataTable dt=objDB.GetDataTable(sql,new SqlParameter("@TrainingID",trainingID));
            if(dt.Rows.Count==0){Response.Write("Certificate template is not configured for this training.");return false;}
            DataRow dr=dt.Rows[0];
            lblHeader.Text=dr["HeaderText"].ToString(); lblFooter.Text=dr["FooterText"].ToString(); lblTitle.Text=dr["TemplateName"].ToString(); lblEmployee.Text="Sample Trainee";
            lblCourse.Text=String.IsNullOrWhiteSpace(dr["CourseTitle"].ToString())?dr["CourseName"].ToString():dr["CourseTitle"].ToString();
            lblDuration.Text=Convert.ToDateTime(dr["DateFrom"]).ToString("dd-MMM-yyyy")+" To "+Convert.ToDateTime(dr["DateTo"]).ToString("dd-MMM-yyyy");
            lblLeftName.Text=dr["LeftName"].ToString(); lblLeftDesignation.Text=dr["LeftDesignation"].ToString(); lblRightName.Text=dr["RightName"].ToString(); lblRightDesignation.Text=dr["RightDesignation"].ToString();
            imgLeftSignature.ImageUrl=dr["LeftSignature"].ToString(); imgRightSignature.ImageUrl=dr["RightSignature"].ToString(); imgLogo.ImageUrl=dr["LogoImage"].ToString();
            ViewState["BackgroundImage"]=dr["BackgroundImage"].ToString(); ViewState["Orientation"]=dr["Orientation"].ToString(); ViewState["PaperSize"]=dr["PaperSize"].ToString();
            ViewState["HeaderFont"]=dr["HeaderFontSize"]; ViewState["FooterFont"]=dr["FooterFontSize"]; ViewState["TitleFont"]=dr["CourseTitleFontSize"]; ViewState["BodyFont"]=dr["BodyFontSize"]; ViewState["NameFont"]=dr["NameFontSize"];
            return true;
        }
        protected void btnBack_Click(object sender,EventArgs e){Response.Redirect("CertificateTemplate.aspx?TrainingID="+Server.UrlEncode(Request.QueryString["TrainingID"]));}
        private void ApplyTemplate(){ApplyBackground();ApplyOrientation();ApplyFont();ApplyTitle();}
        private void ApplyBackground(){string b=Convert.ToString(ViewState["BackgroundImage"]);if(String.IsNullOrWhiteSpace(b))return;divCertificate.Style["background-image"]="url('"+ResolveUrl(b)+"')";divCertificate.Style["background-repeat"]="no-repeat";divCertificate.Style["background-size"]="100% 100%";divCertificate.Style["background-position"]="center";}
        private void ApplyOrientation(){string o=Convert.ToString(ViewState["Orientation"]),p=Convert.ToString(ViewState["PaperSize"]);if(p=="A4"){divCertificate.Style["width"]=o=="Landscape"?"1123px":"794px";divCertificate.Style["height"]=o=="Landscape"?"794px":"1123px";}else{divCertificate.Style["width"]=o=="Landscape"?"1200px":"850px";divCertificate.Style["height"]=o=="Landscape"?"850px":"1200px";}}
        private void ApplyFont(){lblHeader.Font.Size=FontUnit.Point(Convert.ToInt32(ViewState["HeaderFont"]));lblFooter.Font.Size=FontUnit.Point(Convert.ToInt32(ViewState["FooterFont"]));lblTitle.Font.Size=FontUnit.Point(Convert.ToInt32(ViewState["TitleFont"]));lblCourse.Font.Size=FontUnit.Point(Convert.ToInt32(ViewState["BodyFont"]));lblEmployee.Font.Size=FontUnit.Point(Convert.ToInt32(ViewState["NameFont"]));lblDuration.Font.Size=FontUnit.Point(Convert.ToInt32(ViewState["BodyFont"]));}
        private void ApplyTitle(){lblTitle.Font.Bold=true;lblEmployee.Font.Bold=true;lblCourse.Font.Bold=true;lblHeader.Font.Bold=true;lblFooter.Font.Bold=true;lblLeftName.Font.Bold=true;lblRightName.Font.Bold=true;}
        private void AdjustLayout(){ToggleLogo();ToggleSignature();divCertificate.Style["padding"]="20px";divCertificate.Style["box-sizing"]="border-box";}
        private void ToggleLogo(){imgLogo.Visible=!String.IsNullOrWhiteSpace(imgLogo.ImageUrl);}
        private void ToggleSignature(){imgLeftSignature.Visible=!String.IsNullOrWhiteSpace(imgLeftSignature.ImageUrl);imgRightSignature.Visible=!String.IsNullOrWhiteSpace(imgRightSignature.ImageUrl);lblLeftName.Visible=!String.IsNullOrWhiteSpace(lblLeftName.Text);lblRightName.Visible=!String.IsNullOrWhiteSpace(lblRightName.Text);lblLeftDesignation.Visible=!String.IsNullOrWhiteSpace(lblLeftDesignation.Text);lblRightDesignation.Visible=!String.IsNullOrWhiteSpace(lblRightDesignation.Text);}
    }
}