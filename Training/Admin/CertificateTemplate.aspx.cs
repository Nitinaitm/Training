using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CertificateTemplate : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        string TrainingID = "";
        string AdminID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["AdminID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            AdminID = Session["AdminID"].ToString();
            TrainingID = Convert.ToString(Session["TrainingID"]);

            if (string.IsNullOrWhiteSpace(TrainingID))
            {
                Response.Redirect("~/Admin/ManageTraining.aspx");
                return;
            }

            hfTrainingID.Value = TrainingID;

            if (!IsPostBack)
                InitializePage();
        }

        private void InitializePage()
        {
            pnlMessage.Visible = false;
            pnlExisting.Visible = true;
            pnlNew.Visible = false;
            pnlReusable.Visible = false;
            btnApplyConfiguration.Enabled = false;

            LoadTemplates();
            LoadReusableConfigurations();
            LoadExistingTrainingConfiguration();
            TrainingSummary1.LoadTraining(TrainingID);
        }

        private void LoadTemplates()
        {
            DataTable dt = objDB.GetDataTable(@"
SELECT TemplateID,
       TemplateName + ' (' + PaperSize + ')' AS TemplateName
FROM CertificateTemplateMaster
WHERE Active=1
ORDER BY DisplayOrder, TemplateName");

            ddlTemplate.DataSource = dt;
            ddlTemplate.DataTextField = "TemplateName";
            ddlTemplate.DataValueField = "TemplateID";
            ddlTemplate.DataBind();
            ddlTemplate.Items.Insert(0, new ListItem("-- Select Template --", ""));
        }

        private void LoadReusableConfigurations()
        {
            DataTable dt = objDB.GetDataTable(@"
SELECT TrainingTemplateID,
       ConfigurationName +
       CASE WHEN ISNULL(Description,'')='' THEN ''
            ELSE ' - ' + Description END AS ConfigurationName
FROM TrainingCertificateTemplate
WHERE IsReusable=1
ORDER BY ConfigurationName");

            ddlConfiguration.DataSource = dt;
            ddlConfiguration.DataTextField = "ConfigurationName";
            ddlConfiguration.DataValueField = "TrainingTemplateID";
            ddlConfiguration.DataBind();
            ddlConfiguration.Items.Insert(0, new ListItem("-- Select Configuration --", ""));
        }

        private void LoadExistingTrainingConfiguration()
        {
            DataTable dt = objDB.GetDataTable(@"
SELECT TrainingTemplateID, TemplateID, CourseTitle,
       LeftSignature, LeftName, LeftDesignation,
       RightSignature, RightName, RightDesignation,
       ConfigurationName, Description, IsReusable
FROM TrainingCertificateTemplate
WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];
            hfTrainingTemplateID.Value = dr["TrainingTemplateID"].ToString();
            SelectTemplate(dr["TemplateID"].ToString());
            txtCourseTitle.Text = dr["CourseTitle"].ToString();
            txtLeftName.Text = dr["LeftName"].ToString();
            txtLeftDesignation.Text = dr["LeftDesignation"].ToString();
            txtRightName.Text = dr["RightName"].ToString();
            txtRightDesignation.Text = dr["RightDesignation"].ToString();
            txtConfigurationName.Text = dr["ConfigurationName"].ToString();
            txtDescription.Text = dr["Description"].ToString();
            chkReusable.Checked = Convert.ToBoolean(dr["IsReusable"]);
            pnlReusable.Visible = chkReusable.Checked;
            imgLeftSignature.ImageUrl = dr["LeftSignature"].ToString();
            imgRightSignature.ImageUrl = dr["RightSignature"].ToString();
        }

        private void SelectTemplate(string templateID)
        {
            ListItem item = ddlTemplate.Items.FindByValue(templateID);
            if (item != null)
                ddlTemplate.SelectedValue = templateID;
        }

        protected void rblMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool existing = rblMode.SelectedValue == "Existing";
            pnlExisting.Visible = existing;
            pnlNew.Visible = !existing;
            pnlMessage.Visible = false;
        }

        protected void chkReusable_CheckedChanged(object sender, EventArgs e)
        {
            pnlReusable.Visible = chkReusable.Checked;
        }

        protected void ddlConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlExistingDetails.Visible = false;
            btnApplyConfiguration.Enabled = false;

            if (ddlConfiguration.SelectedIndex <= 0)
                return;

            hfSelectedConfigurationID.Value = ddlConfiguration.SelectedValue;
            LoadConfigurationDetails(ddlConfiguration.SelectedValue);
            pnlExistingDetails.Visible = true;
            btnApplyConfiguration.Enabled = true;
        }

        private void LoadConfigurationDetails(string trainingTemplateID)
        {
            DataTable dt = objDB.GetDataTable(@"
SELECT TCT.*, CTM.TemplateName
FROM TrainingCertificateTemplate TCT
INNER JOIN CertificateTemplateMaster CTM ON TCT.TemplateID=CTM.TemplateID
WHERE TCT.TrainingTemplateID=@TrainingTemplateID",
                new SqlParameter[] { new SqlParameter("@TrainingTemplateID", trainingTemplateID) });

            if (dt.Rows.Count == 0)
            {
                pnlExistingDetails.Visible = false;
                return;
            }

            DataRow dr = dt.Rows[0];
            lblConfigurationName.Text = dr["ConfigurationName"].ToString();
            lblTemplateName.Text = dr["TemplateName"].ToString();
            lblCourseTitle.Text = dr["CourseTitle"].ToString();
            lblConfigurationDescription.Text = dr["Description"].ToString();
            lblPreviewLeftName.Text = dr["LeftName"].ToString();
            lblPreviewLeftDesignation.Text = dr["LeftDesignation"].ToString();
            lblPreviewRightName.Text = dr["RightName"].ToString();
            lblPreviewRightDesignation.Text = dr["RightDesignation"].ToString();
            imgPreviewLeft.ImageUrl = dr["LeftSignature"].ToString();
            imgPreviewRight.ImageUrl = dr["RightSignature"].ToString();
        }

        protected void btnApplyConfiguration_Click(object sender, EventArgs e)
        {
            if (ddlConfiguration.SelectedIndex <= 0)
            {
                ShowMessage("Please select a configuration.", false);
                return;
            }

            CopyReusableConfiguration(ddlConfiguration.SelectedValue);
            LoadExistingTrainingConfiguration();
            LoadReusableConfigurations();

            rblMode.SelectedValue = "Existing";
            pnlExisting.Visible = true;
            pnlNew.Visible = false;
            pnlExistingDetails.Visible = false;
            btnApplyConfiguration.Enabled = false;

            ShowMessage("Certificate configuration applied successfully to this training.", true);
        }

        private void CopyReusableConfiguration(string sourceTrainingTemplateID)
        {
            object sourceExists = objDB.ExecuteScalar(@"
SELECT COUNT(*)
FROM TrainingCertificateTemplate
WHERE TrainingTemplateID=@SourceTrainingTemplateID
  AND IsReusable=1",
                new SqlParameter[] { new SqlParameter("@SourceTrainingTemplateID", sourceTrainingTemplateID) });

            if (Convert.ToInt32(sourceExists) == 0)
                throw new Exception("Selected certificate configuration is no longer available.");

            object targetExists = objDB.ExecuteScalar(@"
SELECT COUNT(*)
FROM TrainingCertificateTemplate
WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });

            if (Convert.ToInt32(targetExists) > 0)
            {
                objDB.ExecuteSql(@"
UPDATE T
SET T.TemplateID=S.TemplateID,
    T.CourseTitle=S.CourseTitle,
    T.LeftSignature=S.LeftSignature,
    T.LeftName=S.LeftName,
    T.LeftDesignation=S.LeftDesignation,
    T.RightSignature=S.RightSignature,
    T.RightName=S.RightName,
    T.RightDesignation=S.RightDesignation,
    T.ConfigurationName=S.ConfigurationName,
    T.Description=S.Description,
    T.ModifiedOn=GETDATE(),
    T.ModifiedBy=@AdminID
FROM TrainingCertificateTemplate T
INNER JOIN TrainingCertificateTemplate S
    ON S.TrainingTemplateID=@SourceTrainingTemplateID
WHERE T.TrainingID=@TrainingID",
                    new SqlParameter[]
                    {
                        new SqlParameter("@SourceTrainingTemplateID", sourceTrainingTemplateID),
                        new SqlParameter("@TrainingID", TrainingID),
                        new SqlParameter("@AdminID", AdminID)
                    });
            }
            else
            {
                objDB.ExecuteSql(@"
INSERT INTO TrainingCertificateTemplate
(
 TrainingTemplateID, TrainingID, CourseID, TemplateID, CourseTitle,
 LeftSignature, LeftName, LeftDesignation,
 RightSignature, RightName, RightDesignation,
 CreatedOn, CreatedBy, ConfigurationName, Description, IsReusable
)
SELECT
 @NewTrainingTemplateID, @TrainingID, CourseID, TemplateID, CourseTitle,
 LeftSignature, LeftName, LeftDesignation,
 RightSignature, RightName, RightDesignation,
 GETDATE(), @AdminID, ConfigurationName, Description, 0
FROM TrainingCertificateTemplate
WHERE TrainingTemplateID=@SourceTrainingTemplateID
  AND IsReusable=1",
                    new SqlParameter[]
                    {
                        new SqlParameter("@NewTrainingTemplateID", GenerateTrainingTemplateID()),
                        new SqlParameter("@TrainingID", TrainingID),
                        new SqlParameter("@SourceTrainingTemplateID", sourceTrainingTemplateID),
                        new SqlParameter("@AdminID", AdminID)
                    });
            }
        }

        private void LoadConfigurationToControls(string trainingTemplateID)
        {
            DataTable dt = objDB.GetDataTable(@"
SELECT * FROM TrainingCertificateTemplate
WHERE TrainingTemplateID=@TrainingTemplateID",
                new SqlParameter[] { new SqlParameter("@TrainingTemplateID", trainingTemplateID) });

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];
            SelectTemplate(dr["TemplateID"].ToString());
            txtCourseTitle.Text = dr["CourseTitle"].ToString();
            txtLeftName.Text = dr["LeftName"].ToString();
            txtLeftDesignation.Text = dr["LeftDesignation"].ToString();
            txtRightName.Text = dr["RightName"].ToString();
            txtRightDesignation.Text = dr["RightDesignation"].ToString();
            imgLeftSignature.ImageUrl = dr["LeftSignature"].ToString();
            imgRightSignature.ImageUrl = dr["RightSignature"].ToString();
        }

        private void ShowMessage(string message, bool success)
        {
            pnlMessage.Visible = true;
            lblMessage.Text = message;
            pnlMessage.CssClass = success ? "alert alert-success mt-3" : "alert alert-danger mt-3";
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            Response.Redirect(Request.RawUrl);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddlTemplate.SelectedIndex == 0)
            {
                ShowMessage("Please select certificate template.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtCourseTitle.Text))
            {
                ShowMessage("Please enter course title.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtLeftName.Text) || string.IsNullOrWhiteSpace(txtLeftDesignation.Text))
            {
                ShowMessage("Please enter left signatory name and designation.", false);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtRightName.Text) || string.IsNullOrWhiteSpace(txtRightDesignation.Text))
            {
                ShowMessage("Please enter right signatory name and designation.", false);
                return;
            }
            if (chkReusable.Checked && string.IsNullOrWhiteSpace(txtConfigurationName.Text))
            {
                ShowMessage("Please enter configuration name.", false);
                return;
            }

            try
            {
                SaveTrainingConfiguration();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        private void SaveTrainingConfiguration()
        {
            object obj = objDB.ExecuteScalar(@"
SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });

            if (Convert.ToInt32(obj) == 0)
                InsertTrainingConfiguration();
            else
                UpdateTrainingConfiguration();
        }

        private string UploadLeftSignature()
        {
            if (!fuLeftSignature.HasFile)
                return imgLeftSignature.ImageUrl;

            return SaveSignature(fuLeftSignature, "Left");
        }

        private string UploadRightSignature()
        {
            if (!fuRightSignature.HasFile)
                return imgRightSignature.ImageUrl;

            return SaveSignature(fuRightSignature, "Right");
        }

        private string SaveSignature(FileUpload upload, string side)
        {
            string extension = Path.GetExtension(upload.FileName).ToLower();
            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                throw new Exception(side + " Signature must be JPG, JPEG or PNG.");

            string folder = Server.MapPath("~/Uploads/Certificate/Signature/");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Guid.NewGuid().ToString() + extension;
            upload.SaveAs(Path.Combine(folder, fileName));
            return "~/Uploads/Certificate/Signature/" + fileName;
        }

        private void InsertTrainingConfiguration()
        {
            string leftSignature = UploadLeftSignature();
            string rightSignature = UploadRightSignature();
            string trainingTemplateID = GenerateTrainingTemplateID();
            string courseID = Convert.ToString(objDB.ExecuteScalar(@"
SELECT CourseID FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) }));

            int result = objDB.ExecuteSql(@"
INSERT INTO TrainingCertificateTemplate
(
 TrainingTemplateID, TrainingID, CourseID, TemplateID, CourseTitle,
 LeftSignature, LeftName, LeftDesignation,
 RightSignature, RightName, RightDesignation,
 CreatedOn, CreatedBy, ConfigurationName, Description, IsReusable
)
VALUES
(
 @TrainingTemplateID, @TrainingID, @CourseID, @TemplateID, @CourseTitle,
 @LeftSignature, @LeftName, @LeftDesignation,
 @RightSignature, @RightName, @RightDesignation,
 GETDATE(), @CreatedBy, @ConfigurationName, @Description, @IsReusable
)",
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingTemplateID", trainingTemplateID),
                    new SqlParameter("@TrainingID", TrainingID),
                    new SqlParameter("@CourseID", courseID),
                    new SqlParameter("@TemplateID", ddlTemplate.SelectedValue),
                    new SqlParameter("@CourseTitle", txtCourseTitle.Text.Trim()),
                    new SqlParameter("@LeftSignature", leftSignature),
                    new SqlParameter("@LeftName", txtLeftName.Text.Trim()),
                    new SqlParameter("@LeftDesignation", txtLeftDesignation.Text.Trim()),
                    new SqlParameter("@RightSignature", rightSignature),
                    new SqlParameter("@RightName", txtRightName.Text.Trim()),
                    new SqlParameter("@RightDesignation", txtRightDesignation.Text.Trim()),
                    new SqlParameter("@CreatedBy", AdminID),
                    new SqlParameter("@ConfigurationName", chkReusable.Checked ? (object)txtConfigurationName.Text.Trim() : DBNull.Value),
                    new SqlParameter("@Description", chkReusable.Checked ? (object)txtDescription.Text.Trim() : DBNull.Value),
                    new SqlParameter("@IsReusable", chkReusable.Checked)
                });

            ShowMessage(result > 0 ? "Certificate configuration saved successfully." : "Unable to save certificate configuration.", result > 0);
            LoadExistingTrainingConfiguration();
            LoadReusableConfigurations();
            rblMode.SelectedValue = "Existing";
            pnlExisting.Visible = true;
            pnlNew.Visible = false;
        }

        private string GenerateTrainingTemplateID()
        {
            object obj = objDB.ExecuteScalar(@"
SELECT ISNULL(MAX(CAST(RIGHT(TrainingTemplateID,4) AS INT)),0)+1
FROM TrainingCertificateTemplate");
            return "TCT" + Convert.ToInt32(obj).ToString("0000");
        }

        private void UpdateTrainingConfiguration()
        {
            string leftSignature = UploadLeftSignature();
            string rightSignature = UploadRightSignature();

            int result = objDB.ExecuteSql(@"
UPDATE TrainingCertificateTemplate
SET TemplateID=@TemplateID,
    CourseTitle=@CourseTitle,
    LeftSignature=@LeftSignature,
    LeftName=@LeftName,
    LeftDesignation=@LeftDesignation,
    RightSignature=@RightSignature,
    RightName=@RightName,
    RightDesignation=@RightDesignation,
    ModifiedOn=GETDATE(),
    ModifiedBy=@ModifiedBy,
    ConfigurationName=@ConfigurationName,
    Description=@Description,
    IsReusable=@IsReusable
WHERE TrainingID=@TrainingID",
                new SqlParameter[]
                {
                    new SqlParameter("@TemplateID", ddlTemplate.SelectedValue),
                    new SqlParameter("@CourseTitle", txtCourseTitle.Text.Trim()),
                    new SqlParameter("@LeftSignature", leftSignature),
                    new SqlParameter("@LeftName", txtLeftName.Text.Trim()),
                    new SqlParameter("@LeftDesignation", txtLeftDesignation.Text.Trim()),
                    new SqlParameter("@RightSignature", rightSignature),
                    new SqlParameter("@RightName", txtRightName.Text.Trim()),
                    new SqlParameter("@RightDesignation", txtRightDesignation.Text.Trim()),
                    new SqlParameter("@ModifiedBy", AdminID),
                    new SqlParameter("@TrainingID", TrainingID),
                    new SqlParameter("@ConfigurationName", chkReusable.Checked ? (object)txtConfigurationName.Text.Trim() : DBNull.Value),
                    new SqlParameter("@Description", chkReusable.Checked ? (object)txtDescription.Text.Trim() : DBNull.Value),
                    new SqlParameter("@IsReusable", chkReusable.Checked)
                });

            ShowMessage(result > 0 ? "Certificate configuration updated successfully." : "Unable to update certificate configuration.", result > 0);
            LoadExistingTrainingConfiguration();
            LoadReusableConfigurations();
            rblMode.SelectedValue = "Existing";
            pnlExisting.Visible = true;
            pnlNew.Visible = false;
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null)
            {
                Response.Redirect("~/Admin/ManageTraining.aspx");
                return;
            }

            string trainingID = Session["TrainingID"].ToString();
            object configured = objDB.ExecuteScalar(@"
SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });

            if (Convert.ToInt32(configured) == 0)
            {
                ShowMessage("Please save the certificate configuration before preview.", false);
                return;
            }

            Response.Redirect("~/Admin/CertificatePreview.aspx?TrainingID=" + Server.UrlEncode(trainingID));
        }

        protected void btnPreviewConfiguration_Click(object sender, EventArgs e)
        {
            if (ddlConfiguration.SelectedIndex == 0)
            {
                ShowMessage("Please select configuration.", false);
                return;
            }

            LoadConfigurationDetails(ddlConfiguration.SelectedValue);
            ShowMessage("Configuration preview loaded. Click Apply to use it for this training.", true);
        }
    }
}