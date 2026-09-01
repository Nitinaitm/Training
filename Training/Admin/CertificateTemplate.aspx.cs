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
        clsDataAccess objDB =
            new clsDataAccess();

        string TrainingID =
            "";

        string UserID =
            "";

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            UserID =
                Session["UserID"].ToString();

            TrainingID =
                Session["TrainingID"].ToString();

            hfTrainingID.Value =
                TrainingID;

            if (!IsPostBack)
            {
                InitializePage();
            }
        }

        private void InitializePage()
        {
            pnlMessage.Visible =
                false;

            pnlExisting.Visible =
                true;

            pnlNew.Visible =
                false;

            pnlReusable.Visible =
                false;

            btnApplyConfiguration.Enabled =
                false;

            LoadTemplates();

            LoadReusableConfigurations();

            LoadExistingTrainingConfiguration();
            TrainingSummary1.LoadTraining(TrainingID);

        }

        private void LoadTemplates()
        {
            string sql =
@"
SELECT
TemplateID,
TemplateName
+
' ('
+
PaperSize
+
')'
AS
TemplateName
FROM
CertificateTemplateMaster
WHERE
Active=1
ORDER BY
DisplayOrder,
TemplateName
";

            DataTable dt =
                objDB.GetDataTable(
                sql);

            ddlTemplate.DataSource =
                dt;

            ddlTemplate.DataTextField =
                "TemplateName";

            ddlTemplate.DataValueField =
                "TemplateID";

            ddlTemplate.DataBind();

            ddlTemplate.Items.Insert(
                0,
                new ListItem(
                    "-- Select Template --",
                    ""));
        }

        private void LoadReusableConfigurations()
        {
            string sql =
@"
SELECT
TrainingTemplateID,
ConfigurationName
+
CASE
WHEN
ISNULL(Description,'')=''
THEN
''
ELSE
' - '
+
Description
END
AS
ConfigurationName
FROM
TrainingCertificateTemplate
WHERE
IsReusable=1
ORDER BY
ConfigurationName
";

            DataTable dt =
                objDB.GetDataTable(
                sql);

            ddlConfiguration.DataSource =
                dt;

            ddlConfiguration.DataTextField =
                "ConfigurationName";

            ddlConfiguration.DataValueField =
                "TrainingTemplateID";

            ddlConfiguration.DataBind();

            ddlConfiguration.Items.Insert(
                0,
                new ListItem(
                    "-- Select Configuration --",
                    ""));
        }
        private void LoadExistingTrainingConfiguration()
        {
            string sql =
        @"
SELECT
TrainingTemplateID,
TemplateID,
CourseTitle,
LeftSignature,
LeftName,
LeftDesignation,
RightSignature,
RightName,
RightDesignation,
ConfigurationName,
Description,
IsReusable
FROM
TrainingCertificateTemplate
WHERE
TrainingID=@TrainingID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            TrainingID)
    };

            DataTable dt =
                objDB.GetDataTable(
                sql,
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

            hfTrainingTemplateID.Value =
                dr["TrainingTemplateID"].ToString();

            ddlTemplate.SelectedValue =
                dr["TemplateID"].ToString();

            txtCourseTitle.Text =
                dr["CourseTitle"].ToString();

            txtLeftName.Text =
                dr["LeftName"].ToString();

            txtLeftDesignation.Text =
                dr["LeftDesignation"].ToString();

            txtRightName.Text =
                dr["RightName"].ToString();

            txtRightDesignation.Text =
                dr["RightDesignation"].ToString();

            txtConfigurationName.Text =
                dr["ConfigurationName"].ToString();

            txtDescription.Text =
                dr["Description"].ToString();

            chkReusable.Checked =
                Convert.ToBoolean(
                dr["IsReusable"]);

            pnlReusable.Visible =
                chkReusable.Checked;

            imgLeftSignature.ImageUrl =
                dr["LeftSignature"].ToString();

            imgRightSignature.ImageUrl =
                dr["RightSignature"].ToString();
        }
        protected void rblMode_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            bool existing =
                rblMode.SelectedValue
                ==
                "Existing";

            pnlExisting.Visible =
                existing;

            pnlNew.Visible =
                !existing;

            pnlMessage.Visible =
                false;
        }
        protected void chkReusable_CheckedChanged(
    object sender,
    EventArgs e)
        {
            pnlReusable.Visible =
                chkReusable.Checked;
        }
        protected void ddlConfiguration_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            pnlExistingDetails.Visible =
                false;

            btnApplyConfiguration.Enabled =
                false;

            if
            (
                ddlConfiguration.SelectedIndex
                <=
                0
            )
            {
                return;
            }

            hfSelectedConfigurationID.Value =
                ddlConfiguration.SelectedValue;

            LoadConfigurationDetails(
                ddlConfiguration.SelectedValue);

            pnlExistingDetails.Visible =
                true;

            btnApplyConfiguration.Enabled =
                true;
        }
        private void LoadConfigurationDetails(
    string trainingTemplateID)
        {
            string sql =
        @"
SELECT
TCT.*,
CTM.TemplateName
FROM
TrainingCertificateTemplate TCT
INNER JOIN
CertificateTemplateMaster CTM
ON
TCT.TemplateID=CTM.TemplateID
WHERE
TCT.TrainingTemplateID=@TrainingTemplateID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingTemplateID",
            trainingTemplateID)
    };

            DataTable dt =
                objDB.GetDataTable(
                sql,
                param);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                pnlExistingDetails.Visible =
                    false;

                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblConfigurationName.Text =
                dr["ConfigurationName"].ToString();

            lblTemplateName.Text =
                dr["TemplateName"].ToString();

            lblCourseTitle.Text =
                dr["CourseTitle"].ToString();

            lblConfigurationDescription.Text =
                dr["Description"].ToString();

            lblPreviewLeftName.Text =
                dr["LeftName"].ToString();

            lblPreviewLeftDesignation.Text =
                dr["LeftDesignation"].ToString();

            lblPreviewRightName.Text =
                dr["RightName"].ToString();

            lblPreviewRightDesignation.Text =
                dr["RightDesignation"].ToString();

            imgPreviewLeft.ImageUrl =
                dr["LeftSignature"].ToString();

            imgPreviewRight.ImageUrl =
                dr["RightSignature"].ToString();

            pnlExistingDetails.Visible =
                true;
        }

        protected void btnApplyConfiguration_Click(
      object sender,
      EventArgs e)
        {
            if
            (
                ddlConfiguration.SelectedIndex
                <=
                0
            )
            {
                ShowMessage(
                    "Please select configuration.",
                    false);

                return;
            }

            LoadConfigurationToControls(
                ddlConfiguration.SelectedValue);

            rblMode.SelectedValue =
                "New";

            pnlExisting.Visible =
                false;

            pnlNew.Visible =
                true;

            ShowMessage(
                "Configuration loaded successfully. You can modify and Save.",
                true);
        }
        private void LoadConfigurationToControls(
    string trainingTemplateID)
        {
            string sql =
        @"
SELECT
*
FROM
TrainingCertificateTemplate
WHERE
TrainingTemplateID=@TrainingTemplateID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingTemplateID",
            trainingTemplateID)
    };

            DataTable dt =
                objDB.GetDataTable(
                sql,
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

            ddlTemplate.SelectedValue =
                dr["TemplateID"].ToString();

            txtCourseTitle.Text =
                dr["CourseTitle"].ToString();

            txtLeftName.Text =
                dr["LeftName"].ToString();

            txtLeftDesignation.Text =
                dr["LeftDesignation"].ToString();

            txtRightName.Text =
                dr["RightName"].ToString();

            txtRightDesignation.Text =
                dr["RightDesignation"].ToString();

            imgLeftSignature.ImageUrl =
                dr["LeftSignature"].ToString();

            imgRightSignature.ImageUrl =
                dr["RightSignature"].ToString();

            chkReusable.Checked =
                false;

            pnlReusable.Visible =
                false;

            txtConfigurationName.Text =
                "";

            txtDescription.Text =
                "";
        }

        private void ShowMessage(
    string message,
    bool success)
        {
            pnlMessage.Visible =
                true;

            lblMessage.Text =
                message;

            pnlMessage.CssClass =
                success
                ?
                "alert alert-success mt-3"
                :
                "alert alert-danger mt-3";
        }

        protected void btnReset_Click(
    object sender,
    EventArgs e)
        {
            Response.Redirect(
                Request.RawUrl);
        }
        protected void btnSave_Click(
    object sender,
    EventArgs e)
        {
            if
            (
                ddlTemplate.SelectedIndex
                ==
                0
            )
            {
                ShowMessage(
                    "Please select certificate template.",
                    false);

                return;
            }

            if
            (
                txtCourseTitle.Text.Trim()
                ==
                ""
            )
            {
                ShowMessage(
                    "Please enter course title.",
                    false);

                return;
            }

            if
            (
                txtLeftName.Text.Trim()
                ==
                ""
            )
            {
                ShowMessage(
                    "Please enter left signatory name.",
                    false);

                return;
            }

            if
            (
                txtLeftDesignation.Text.Trim()
                ==
                ""
            )
            {
                ShowMessage(
                    "Please enter left signatory designation.",
                    false);

                return;
            }

            if
            (
                txtRightName.Text.Trim()
                ==
                ""
            )
            {
                ShowMessage(
                    "Please enter right signatory name.",
                    false);

                return;
            }

            if
            (
                txtRightDesignation.Text.Trim()
                ==
                ""
            )
            {
                ShowMessage(
                    "Please enter right signatory designation.",
                    false);

                return;
            }

            if
            (
                chkReusable.Checked
                &&
                txtConfigurationName.Text.Trim()
                ==
                ""
            )
            {
                ShowMessage(
                    "Please enter configuration name.",
                    false);

                return;
            }

            SaveTrainingConfiguration();
        }
        private void SaveTrainingConfiguration()
        {
            object obj =
                objDB.ExecuteScalar(
                @"
SELECT
COUNT(*)
FROM
TrainingCertificateTemplate
WHERE
TrainingID=@TrainingID",
                new SqlParameter[]
                {
            new SqlParameter(
                "@TrainingID",
                TrainingID)
                });

            int count =
                Convert.ToInt32(obj);

            if
            (
                count
                ==
                0
            )
            {
                InsertTrainingConfiguration();
            }
            else
            {
                UpdateTrainingConfiguration();
            }
        }
        private string UploadLeftSignature()
        {
            if
            (
                !fuLeftSignature.HasFile
            )
            {
                return
                    imgLeftSignature.ImageUrl;
            }

            string extension =
                Path.GetExtension(
                fuLeftSignature.FileName)
                .ToLower();

            if
            (
                extension != ".png"
                &&
                extension != ".jpg"
                &&
                extension != ".jpeg"
            )
            {
                throw new Exception(
                    "Left Signature must be JPG, JPEG or PNG.");
            }

            string folder =
                Server.MapPath(
                "~/Uploads/Certificate/Signature/");

            if
            (
                !Directory.Exists(folder)
            )
            {
                Directory.CreateDirectory(
                    folder);
            }

            string fileName =
                Guid.NewGuid().ToString()
                +
                extension;

            fuLeftSignature.SaveAs(
                Path.Combine(
                    folder,
                    fileName));

            return
                "~/Uploads/Certificate/Signature/"
                +
                fileName;
        }
        private string UploadRightSignature()
        {
            if
            (
                !fuRightSignature.HasFile
            )
            {
                return
                    imgRightSignature.ImageUrl;
            }

            string extension =
                Path.GetExtension(
                fuRightSignature.FileName)
                .ToLower();

            if
            (
                extension != ".png"
                &&
                extension != ".jpg"
                &&
                extension != ".jpeg"
            )
            {
                throw new Exception(
                    "Right Signature must be JPG, JPEG or PNG.");
            }

            string folder =
                Server.MapPath(
                "~/Uploads/Certificate/Signature/");

            if
            (
                !Directory.Exists(folder)
            )
            {
                Directory.CreateDirectory(
                    folder);
            }

            string fileName =
                Guid.NewGuid().ToString()
                +
                extension;

            fuRightSignature.SaveAs(
                Path.Combine(
                    folder,
                    fileName));

            return
                "~/Uploads/Certificate/Signature/"
                +
                fileName;
        }
        private void InsertTrainingConfiguration()
        {
            string leftSignature =
                UploadLeftSignature();

            string rightSignature =
                UploadRightSignature();

            string trainingTemplateID =
                GenerateTrainingTemplateID();

            string courseID =
                Convert.ToString(
                objDB.ExecuteScalar(
                @"
SELECT
CourseID
FROM
TrainingDetails
WHERE
TrainingID=@TrainingID",
                new SqlParameter[]
                {
            new SqlParameter(
                "@TrainingID",
                TrainingID)
                }));

            string sql =
        @"
INSERT INTO
TrainingCertificateTemplate
(
TrainingTemplateID,
TrainingID,
CourseID,
TemplateID,
CourseTitle,
LeftSignature,
LeftName,
LeftDesignation,
RightSignature,
RightName,
RightDesignation,
CreatedOn,
CreatedBy,
ConfigurationName,
Description,
IsReusable
)
VALUES
(
@TrainingTemplateID,
@TrainingID,
@CourseID,
@TemplateID,
@CourseTitle,
@LeftSignature,
@LeftName,
@LeftDesignation,
@RightSignature,
@RightName,
@RightDesignation,
GETDATE(),
@CreatedBy,
@ConfigurationName,
@Description,
@IsReusable
)";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingTemplateID",
            trainingTemplateID),

        new SqlParameter(
            "@TrainingID",
            TrainingID),

        new SqlParameter(
            "@CourseID",
            courseID),

        new SqlParameter(
            "@TemplateID",
            ddlTemplate.SelectedValue),

        new SqlParameter(
            "@CourseTitle",
            txtCourseTitle.Text.Trim()),

        new SqlParameter(
            "@LeftSignature",
            leftSignature),

        new SqlParameter(
            "@LeftName",
            txtLeftName.Text.Trim()),

        new SqlParameter(
            "@LeftDesignation",
            txtLeftDesignation.Text.Trim()),

        new SqlParameter(
            "@RightSignature",
            rightSignature),

        new SqlParameter(
            "@RightName",
            txtRightName.Text.Trim()),

        new SqlParameter(
            "@RightDesignation",
            txtRightDesignation.Text.Trim()),

        new SqlParameter(
            "@CreatedBy",
            UserID),

        new SqlParameter(
            "@ConfigurationName",
            chkReusable.Checked
            ?
            txtConfigurationName.Text.Trim()
            :
            (object)DBNull.Value),

        new SqlParameter(
            "@Description",
            chkReusable.Checked
            ?
            txtDescription.Text.Trim()
            :
            (object)DBNull.Value),

        new SqlParameter(
            "@IsReusable",
            chkReusable.Checked)
    };

            int result =
                objDB.ExecuteSql(
                sql,
                param);

            if
            (
                result
                >
                0
            )
            {
                hfTrainingTemplateID.Value =
                    trainingTemplateID;

                ShowMessage(
                    "Certificate configuration saved successfully.",
                    true);
            }
            else
            {
                ShowMessage(
                    "Unable to save certificate configuration.",
                    false);
            }

            LoadExistingTrainingConfiguration();

            LoadReusableConfigurations();

            rblMode.SelectedValue =
                "Existing";

            pnlExisting.Visible =
                true;

            pnlNew.Visible =
                false;
        }

        private string GenerateTrainingTemplateID()
        {
            string sql =
        @"
SELECT
ISNULL(
MAX(
CAST(
RIGHT(
TrainingTemplateID,
4)
AS INT)),
0)
+
1
FROM
TrainingCertificateTemplate
";

            object obj =
                objDB.ExecuteScalar(
                sql);

            int nextID =
                Convert.ToInt32(
                obj);

            return
                "TCT"
                +
                nextID.ToString("0000");
        }

        private void UpdateTrainingConfiguration()
        {
            string leftSignature =
                UploadLeftSignature();

            string rightSignature =
                UploadRightSignature();

            string sql =
        @"
UPDATE
TrainingCertificateTemplate

SET

TemplateID=@TemplateID,

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

WHERE

TrainingID=@TrainingID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TemplateID",
            ddlTemplate.SelectedValue),

        new SqlParameter(
            "@CourseTitle",
            txtCourseTitle.Text.Trim()),

        new SqlParameter(
            "@LeftSignature",
            leftSignature),

        new SqlParameter(
            "@LeftName",
            txtLeftName.Text.Trim()),

        new SqlParameter(
            "@LeftDesignation",
            txtLeftDesignation.Text.Trim()),

        new SqlParameter(
            "@RightSignature",
            rightSignature),

        new SqlParameter(
            "@RightName",
            txtRightName.Text.Trim()),

        new SqlParameter(
            "@RightDesignation",
            txtRightDesignation.Text.Trim()),

        new SqlParameter(
            "@ModifiedBy",
            UserID),

        new SqlParameter(
            "@TrainingID",
            TrainingID),

        new SqlParameter(
            "@ConfigurationName",
            chkReusable.Checked
            ?
            txtConfigurationName.Text.Trim()
            :
            (object)DBNull.Value),

        new SqlParameter(
            "@Description",
            chkReusable.Checked
            ?
            txtDescription.Text.Trim()
            :
            (object)DBNull.Value),

        new SqlParameter(
            "@IsReusable",
            chkReusable.Checked)
    };

            int result =
                objDB.ExecuteSql(
                sql,
                param);

            if
            (
                result
                >
                0
            )
            {
                ShowMessage(
                    "Certificate configuration updated successfully.",
                    true);
            }
            else
            {
                ShowMessage(
                    "Unable to update certificate configuration.",
                    false);
            }
            LoadExistingTrainingConfiguration();

            LoadReusableConfigurations();

            rblMode.SelectedValue =
                "Existing";

            pnlExisting.Visible =
                true;

            pnlNew.Visible =
                false;
        }

        protected void btnPreview_Click(
     object sender,
     EventArgs e)
        {
            ShowMessage(
                "Preview will be available in Version 2.",
                true);
        }
        protected void btnPreviewConfiguration_Click(
    object sender,
    EventArgs e)
        {
            if
            (
                ddlConfiguration.SelectedIndex
                ==
                0
            )
            {
                ShowMessage(
                    "Please select configuration.",
                    false);

                return;
            }

            LoadConfigurationToControls(
                ddlConfiguration.SelectedValue);

            ShowMessage(
                "Configuration loaded successfully.",
                true);
        }
    }
}