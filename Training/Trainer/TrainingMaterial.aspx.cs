using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

namespace Training.Trainer
{
    public partial class TrainingMaterial : System.Web.UI.Page
    {
        clsDataAccess obj =
            new clsDataAccess();

        private readonly string UploadFolder =
            "~/Uploads/TrainingMaterial/";

        private string TrainerID
        {
            get
            {
                return
                    Session["TrainerID"].ToString();
            }
        }

        private string TrainingID
        {
            get
            {
                return
                    Session["TrainingID"].ToString();
            }
        }

        private string SessionID
        {
            get
            {
                return
                    Session["SessionID"].ToString();
            }
        }

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if
            (
                Session["TrainerID"] == null
            )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                Session["TrainingID"] == null
            )
            {
                Response.Redirect(
                    "~/Trainer/Default.aspx");

                return;
            }

            if
            (
                Session["SessionID"] == null
            )
            {
                Response.Redirect(
                    "~/Trainer/Default.aspx");

                return;
            }

            if
            (
                !IsPostBack
            )
            {
                TrainerSummary1.LoadTraining(
                    TrainingID);

                SessionSummary1.LoadSession(
                    SessionID);

                BindSummary();

                BindGrid();
            }
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }

        protected void btnBack_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect("~/Trainer/SessionDetails.aspx");
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtSearch.Text =
                "";

            ddlFilterType.SelectedIndex =
                0;

            BindGrid();
        }
        private void BindSummary()
        {
            string query =
                @"SELECT COUNT(*) TotalMaterial,SUM(CASE WHEN MaterialType='Document' THEN 1 ELSE 0 END) DocumentCount,SUM(CASE WHEN MaterialType='PDF' THEN 1 ELSE 0 END) PDFCount,SUM(CASE WHEN MaterialType='PPT' THEN 1 ELSE 0 END) PPTCount,SUM(CASE WHEN MaterialType='Video' THEN 1 ELSE 0 END) VideoCount,SUM(CASE WHEN MaterialType='Other' THEN 1 ELSE 0 END) OtherCount FROM TrainingMaterial WHERE SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@SessionID",
            SessionID)
    };

            DataTable dt =
                obj.GetDataTable(
                query,
                param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblTotal.Text =
                dr["TotalMaterial"] == DBNull.Value
                ? "0"
                : dr["TotalMaterial"].ToString();

            lblDocument.Text =
                dr["DocumentCount"] == DBNull.Value
                ? "0"
                : dr["DocumentCount"].ToString();

            lblPDF.Text =
                dr["PDFCount"] == DBNull.Value
                ? "0"
                : dr["PDFCount"].ToString();

            lblPPT.Text =
                dr["PPTCount"] == DBNull.Value
                ? "0"
                : dr["PPTCount"].ToString();

            lblVideo.Text =
                dr["VideoCount"] == DBNull.Value
                ? "0"
                : dr["VideoCount"].ToString();

            lblOther.Text =
                dr["OtherCount"] == DBNull.Value
                ? "0"
                : dr["OtherCount"].ToString();
        }

        private void BindGrid()
        {
            string query =
                @"SELECT MaterialID, Title,MaterialType,FileName,Description,VisibleToTrainee,DownloadAllowed,CreatedOn FROM TrainingMaterial WHERE SessionID=@SessionID";

            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                    "@SessionID",
                    SessionID));

            if
            (
                txtSearch.Text.Trim() != ""
            )
            {
                query +=
                    " AND Title LIKE @Title";

                param.Add(
                    new SqlParameter(
                        "@Title",
                        "%"
                        +
                        txtSearch.Text.Trim()
                        +
                        "%"));
            }

            if
(
    ddlFilterType.SelectedValue != ""
    &&
    ddlFilterType.SelectedValue != "All"
)
            {
                query +=
                    " AND MaterialType=@MaterialType";

                param.Add(
                    new SqlParameter(
                        "@MaterialType",
                        ddlFilterType.SelectedValue));
            }

            query +=
                " ORDER BY CreatedOn DESC";

            DataTable dt =
                obj.GetDataTable(
                query,
                param.ToArray());

            gvMaterial.DataSource =
                dt;

            gvMaterial.DataBind();
        }

        protected void btnUpload_Click(
    object sender,
    EventArgs e)
        {
            lblMessage.Text =
                "";

            lblMessage.ForeColor =
                System.Drawing.Color.Red;

            if
            (
                txtTitle.Text.Trim() == ""
            )
            {
                lblMessage.Text =
                    "Please enter Material Title.";

                txtTitle.Focus();

                return;
            }

            if
            (
                ddlType.SelectedValue == ""
            )
            {
                lblMessage.Text =
                    "Please select Material Type.";

                ddlType.Focus();

                return;
            }

            if
            (
                !fuMaterial.HasFile
            )
            {
                lblMessage.Text =
                    "Please select Material File.";

                return;
            }



            string duplicateQuery =
                @"SELECT COUNT(*) FROM TrainingMaterial WHERE SessionID=@SessionID AND TrainerID=@TrainerID AND UPPER(Title)=UPPER(@Title)";

            SqlParameter[] duplicateParam =
            {
        new SqlParameter(
            "@SessionID",
            SessionID),
         new SqlParameter(
            "@TrainerID",
            TrainerID),

        new SqlParameter(
            "@Title",
            txtTitle.Text.Trim())
    };

            int duplicateCount =
                Convert.ToInt32(
                obj.ExecuteScalar(
                duplicateQuery,
                duplicateParam));

            if
            (
                duplicateCount > 0
            )
            {
                lblMessage.Text =
                    "Material Title already exists.";

                return;
            }

            string extension =
                Path.GetExtension(
                fuMaterial.FileName)
                .ToLower();

            string[] allowedFile =
            {
        ".pdf",
        ".ppt",
        ".pptx",
        ".doc",
        ".docx",
        ".xls",
        ".xlsx",
        ".mp4",
        ".zip",
        ".rar",
        ".jpg",
        ".jpeg",
        ".png"
    };

            bool valid =
                false;

            foreach
            (
                string item
                in
                allowedFile
            )
            {
                if
                (
                    extension == item
                )
                {
                    valid =
                        true;

                    break;
                }
            }

            if
            (
                !valid
            )
            {
                lblMessage.Text =
                    "Invalid file type.";

                return;
            }

            if
            (
                fuMaterial.PostedFile.ContentLength
                >
                104857600
            )
            {
                lblMessage.Text =
                    "Maximum file size is 100 MB.";

                return;
            }



            if
            (
                !ValidateMaterialType(
                    ddlType.SelectedValue,
                    extension)
            )
            {
                lblMessage.Text =
                    "Selected file does not match Material Type.";

                return;
            }

            string folder = Server.MapPath(UploadFolder);

            if
            (
                !Directory.Exists(
                folder)
            )
            {
                Directory.CreateDirectory(
                folder);
            }

            string materialID =
                GenerateMaterialID();

            string fileName =
                materialID
                +
                extension;

            string savePath =
                Path.Combine(
                folder,
                fileName);

            fuMaterial.SaveAs(
                savePath);
            string topicQuery = @"SELECT TopicID FROM SessionMaster WHERE SessionID=@SessionID";

            SqlParameter[] topicParam =
            {
    new SqlParameter(
        "@SessionID",
        SessionID)
};

            object topicID =
                obj.ExecuteScalar(
                topicQuery,
                topicParam);

            if
            (
                topicID == null
            )
            {
                topicID =
                    DBNull.Value;
            }


            string query =
                @"INSERT INTO TrainingMaterial(MaterialID,TrainingID,SessionID,TopicID,TrainerID,Title,Description,MaterialType,FileName,FilePath,VideoURL,VisibleToTrainee,DownloadAllowed,CreatedOn,CreatedBy) VALUES(@MaterialID,@TrainingID,@SessionID,@TopicID,@TrainerID,@Title,@Description,@MaterialType,@FileName,@FilePath,@VideoURL,@VisibleToTrainee,@DownloadAllowed,GETDATE(),@CreatedBy)";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@MaterialID",
            materialID),

        new SqlParameter(
            "@TrainingID",
            TrainingID),

        new SqlParameter(
            "@SessionID",
            SessionID),

         new SqlParameter(
                "@TopicID",
                topicID ?? DBNull.Value),

        new SqlParameter(
            "@TrainerID",
            TrainerID),

        new SqlParameter(
            "@Title",
            txtTitle.Text.Trim()),

        new SqlParameter(
            "@Description",
            txtDescription.Text.Trim()),

        new SqlParameter(
            "@MaterialType",
            ddlType.SelectedValue),

        new SqlParameter(
            "@FileName",
            fuMaterial.FileName),

        new SqlParameter(
    "@FilePath",
    UploadFolder
    +

    fileName),

        new SqlParameter(
            "@VideoURL",
            DBNull.Value),

        new SqlParameter(
    "@VisibleToTrainee",
    chkVisibleToTrainee.Checked),

new SqlParameter(
    "@DownloadAllowed",
    chkDownloadAllowed.Checked),

        new SqlParameter(
            "@CreatedBy",
            TrainerID)
    };

            obj.ExecuteSql(
                query,
                param);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Training Material uploaded successfully.";

            txtTitle.Text =
                "";

            txtDescription.Text =
                "";

            ddlType.SelectedIndex =
                0;

            fuMaterial.Attributes.Clear();

            BindSummary();

            BindGrid();

            SessionSummary1.LoadSession(
                SessionID);
            chkVisibleToTrainee.Checked =
    true;

            chkDownloadAllowed.Checked =
                true;
        }

        private string GenerateMaterialID()
        {
            Random random =
        new Random();
            string query =
                @"SELECT ISNULL(MAX(ID),0)+1 FROM TrainingMaterial";

            int nextID =
                Convert.ToInt32(
                obj.ExecuteScalar(
                query,
                null));

            return
                "MAT"
                +

        random.Next(
        1000,
        9999)
        .ToString()
                +
                nextID.ToString("000000");
        }

        protected void gvMaterial_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            string materialID =
                e.CommandArgument.ToString();

            if
(
    e.CommandName == "DownloadMaterial"
)
            {
                DownloadMaterial(
                    materialID);

                return;
            }

            if
(
    e.CommandName == "DeleteMaterial"
)
            {
                DeleteMaterial(
                    materialID);

                return;
            }
        }

        private void DownloadMaterial(
            string materialID)
        {
            string query =
                @"SELECT FileName,FilePath,DownloadAllowed FROM TrainingMaterial WHERE MaterialID=@MaterialID AND TrainerID=@TrainerID AND SessionID=@SessionID";

            SqlParameter[] param =
 {
    new SqlParameter("@MaterialID", materialID),
    new SqlParameter("@TrainerID", TrainerID),
    new SqlParameter("@SessionID", SessionID)
};

            DataTable dt =
                obj.GetDataTable(
                query,
                param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                lblMessage.Text =
                    "Material not found.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            if
            (
                !Convert.ToBoolean(
                dt.Rows[0]["DownloadAllowed"])
            )
            {
                lblMessage.Text =
                    "Download is not allowed.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            string filePath =
                Server.MapPath(
                dt.Rows[0]["FilePath"].ToString());

            if
            (
                !File.Exists(
                filePath)
            )
            {
                lblMessage.Text =
                    "Physical file not found.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            Response.Clear();

            string extension =
                Path.GetExtension(
                filePath)
                .ToLower();

            switch (extension)
            {
                case ".pdf":

                    Response.ContentType =
                        "application/pdf";

                    break;

                case ".ppt":

                    Response.ContentType =
                        "application/vnd.ms-powerpoint";

                    break;

                case ".pptx":

                    Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.presentationml.presentation";

                    break;

                case ".doc":

                    Response.ContentType =
                        "application/msword";

                    break;

                case ".docx":

                    Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

                    break;

                case ".xls":

                    Response.ContentType =
                        "application/vnd.ms-excel";

                    break;

                case ".xlsx":

                    Response.ContentType =
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    break;

                case ".jpg":

                case ".jpeg":

                    Response.ContentType =
                        "image/jpeg";

                    break;

                case ".png":

                    Response.ContentType =
                        "image/png";

                    break;

                case ".mp4":

                    Response.ContentType =
                        "video/mp4";

                    break;

                case ".zip":

                    Response.ContentType =
                        "application/zip";

                    break;

                case ".rar":

                    Response.ContentType =
                        "application/x-rar-compressed";

                    break;

                default:

                    Response.ContentType =
                        "application/octet-stream";

                    break;
            }

            Response.AppendHeader(
                "Content-Disposition",
                "attachment; filename=\""
                +
                dt.Rows[0]["FileName"].ToString()
                +
                "\"");

            Response.TransmitFile(
                filePath);

            Response.Flush();

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        private bool ValidateMaterialType(
    string materialType,
    string extension)
        {
            materialType =
                materialType.Trim().ToUpper();

            extension =
                extension.Trim().ToLower();

            switch (materialType)
            {
                case "PDF":

                    return
                        extension.Equals(
                        ".pdf",
                        StringComparison.OrdinalIgnoreCase);

                case "PPT":

                    return
                        extension.Equals(".ppt", StringComparison.OrdinalIgnoreCase)
                        ||
                        extension.Equals(".pptx", StringComparison.OrdinalIgnoreCase);

                case "DOCUMENT":

                    return
                        extension.Equals(".doc", StringComparison.OrdinalIgnoreCase)
                        ||
                        extension.Equals(".docx", StringComparison.OrdinalIgnoreCase)
                        ||
                        extension.Equals(".xls", StringComparison.OrdinalIgnoreCase)
                        ||
                        extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase);

                case "VIDEO":

                    return
                        extension.Equals(".mp4", StringComparison.OrdinalIgnoreCase);

                case "OTHER":

                    return true;

                default:

                    return false;
            }
        }

        private void DeleteMaterial(
            string materialID)
        {
            string query =
                @"SELECT FilePath FROM TrainingMaterial WHERE MaterialID=@MaterialID AND TrainerID=@TrainerID AND SessionID=@SessionID";

            SqlParameter[] param =
{
    new SqlParameter("@MaterialID", materialID),
    new SqlParameter("@TrainerID", TrainerID),
    new SqlParameter("@SessionID", SessionID)
};

            DataTable dt =
                obj.GetDataTable(
                query,
                param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return;
            }

            string deleteQuery =
                @"DELETE FROM TrainingMaterial WHERE MaterialID=@MaterialID AND TrainerID=@TrainerID AND SessionID=@SessionID";

            SqlParameter[] deleteParam =
{
    new SqlParameter("@MaterialID", materialID),
    new SqlParameter("@TrainerID", TrainerID),
    new SqlParameter("@SessionID", SessionID)
};

            obj.ExecuteSql(
                deleteQuery,
                deleteParam);

            string filePath =
                Server.MapPath(
                dt.Rows[0]["FilePath"].ToString());

            if
            (
                File.Exists(
                filePath)
            )
            {
                File.Delete(
                    filePath);
            }

            BindSummary();

            BindGrid();

            SessionSummary1.LoadSession(
                SessionID);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Material deleted successfully.";
        }
    }
}