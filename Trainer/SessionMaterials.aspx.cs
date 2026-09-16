using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class SessionMaterials : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) { BindSessions(); BindGrid(); }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindSessions()
        {
            string query = "SELECT SessionID, TrainingID + ' | S-' + CAST(SessionNo AS VARCHAR) + ' | ' + SessionName AS DisplayName FROM SessionMaster WHERE TrainerID=@TrainerID ORDER BY TRY_CONVERT(date,SessionDate,105) DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            ddlSession.DataSource = dt;
            ddlSession.DataTextField = "DisplayName";
            ddlSession.DataValueField = "SessionID";
            ddlSession.DataBind();
            ddlSession.Items.Insert(0, new ListItem("-- Select Session --", ""));
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (ddlSession.SelectedIndex == 0) { lblMessage.Text = "Select Session."; return; }
            if (string.IsNullOrEmpty(txtTitle.Text.Trim())) { lblMessage.Text = "Enter Title."; return; }
            if (!fuMaterial.HasFile) { lblMessage.Text = "Select File."; return; }

            string folder = Server.MapPath("~/Uploads/SessionMaterials/");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Path.GetFileName(fuMaterial.FileName);
            string filePath = Path.Combine(folder, fileName);
            fuMaterial.SaveAs(filePath);

            string query = @"INSERT INTO SessionMaterial (MaterialID, SessionID, Title, FileName, FilePath, CreatedOn) 
                             VALUES (@MaterialID, @SessionID, @Title, @FileName, @FilePath, GETDATE())";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@MaterialID", Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper()),
                new SqlParameter("@SessionID", ddlSession.SelectedValue),
                new SqlParameter("@Title", txtTitle.Text.Trim()),
                new SqlParameter("@FileName", fileName),
                new SqlParameter("@FilePath", "~/Uploads/SessionMaterials/" + fileName)
            };
            obj.ExecuteSql(query, param);

            lblMessage.Text = "Material uploaded successfully!";
            lblMessage.ForeColor = System.Drawing.Color.Green;
            txtTitle.Text = "";
            BindGrid();
        }

        private void BindGrid()
        {
            string query = @"SELECT SM.MaterialID, SM.Title, SM.FileName, SM.CreatedOn, S.SessionName FROM SessionMaterial SM INNER JOIN SessionMaster S ON SM.SessionID=S.SessionID WHERE S.TrainerID=@TrainerID ORDER BY SM.CreatedOn DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            gvMaterials.DataSource = dt;
            gvMaterials.DataBind();
        }
    }
}