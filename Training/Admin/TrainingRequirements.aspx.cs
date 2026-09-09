using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingRequirements : Page
    {
        private readonly clsDataAccess db = new clsDataAccess();
        private string TrainingID { get { return Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString(); } }
        private string Actor { get { return Session["UserID"] == null ? "Admin" : Session["UserID"].ToString(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TrainingID)) { Response.Redirect("TrainingList.aspx"); return; }
            if (!IsPostBack) { LoadBatchStatus(); LoadSessions(); }
        }

        private void LoadBatchStatus()
        {
            DataTable dt = db.GetDataTable(@"SELECT FeedbackRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,CertificateRequired,ISNULL(CertificateSkipped,0) CertificateSkipped FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
            if (dt.Rows.Count == 0) { Response.Redirect("TrainingList.aspx"); return; }
            DataRow r = dt.Rows[0];
            lblTraining.Text = "Training: " + TrainingID;
            bool fr = Convert.ToBoolean(r["FeedbackRequired"]);
            bool fs = Convert.ToBoolean(r["FeedbackSkipped"]);
            bool cr = Convert.ToBoolean(r["CertificateRequired"]);
            bool cs = Convert.ToBoolean(r["CertificateSkipped"]);
            lblFeedbackStatus.Text = fs ? "Skipped" : (fr ? "Required" : "Not Required");
            lblCertificateStatus.Text = cs ? "Skipped" : (cr ? "Required" : "Not Required");
            btnFeedback.Visible = fr || fs;
            btnCertificate.Visible = cr || cs;
            btnFeedback.Text = fs ? "Unskip Feedback" : "Skip Feedback";
            btnCertificate.Text = cs ? "Unskip Certificate" : "Skip Certificate";
            btnFeedback.CssClass = fs ? "btn btn-outline-success mt-2" : "btn btn-outline-danger mt-2";
            btnCertificate.CssClass = cs ? "btn btn-outline-success mt-2" : "btn btn-outline-danger mt-2";
        }

        private void LoadSessions()
        {
            gvSessions.DataSource = db.GetDataTable(@"SELECT SessionID,ISNULL(AttendanceSkipped,0) AttendanceSkipped,ISNULL(PreAssessmentSkipped,0) PreAssessmentSkipped,ISNULL(PostAssessmentSkipped,0) PostAssessmentSkipped FROM SessionMaster WHERE TrainingID=@TrainingID ORDER BY SessionID", P("@TrainingID", TrainingID));
            gvSessions.DataBind();
        }

        protected void btnFeedback_Click(object sender, EventArgs e) { ToggleBatch("Feedback", "FeedbackSkipped", "FeedbackSkipReason", txtFeedbackReason.Text.Trim()); }
        protected void btnCertificate_Click(object sender, EventArgs e) { ToggleBatch("Certificate", "CertificateSkipped", "CertificateSkipReason", txtCertificateReason.Text.Trim()); }

        private void ToggleBatch(string label, string flag, string reasonColumn, string reason)
        {
            bool skipped = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(" + flag + ",0) FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID)));
            if (!skipped)
            {
                if (string.IsNullOrWhiteSpace(reason)) { ShowError("Skip reason is mandatory for " + label + "."); return; }
                db.ExecuteSql("UPDATE TrainingDetails SET " + flag + "=1," + reasonColumn + "=@Reason," + reasonColumn.Replace("Reason", "By") + "=@By," + reasonColumn.Replace("Reason", "On") + "=GETDATE() WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@Reason", reason), new SqlParameter("@By", Actor), new SqlParameter("@TrainingID", TrainingID) });
                ShowSuccess(label + " has been skipped.");
            }
            else
            {
                db.ExecuteSql("UPDATE TrainingDetails SET " + flag + "=0," + reasonColumn + "=NULL," + reasonColumn.Replace("Reason", "By") + "=NULL," + reasonColumn.Replace("Reason", "On") + "=NULL WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
                ShowSuccess(label + " has been unskipped.");
            }
            LoadBatchStatus();
        }

        protected void gvSessions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "Attendance" && e.CommandName != "Pre" && e.CommandName != "Post") return;
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            TextBox reasonBox = null;
            if (e.CommandName == "Attendance") reasonBox = (TextBox)row.FindControl("txtAttendanceReason");
            if (e.CommandName == "Pre") reasonBox = (TextBox)row.FindControl("txtPreReason");
            if (e.CommandName == "Post") reasonBox = (TextBox)row.FindControl("txtPostReason");
            string sessionID = e.CommandArgument.ToString();
            string flag = e.CommandName == "Attendance" ? "AttendanceSkipped" : (e.CommandName == "Pre" ? "PreAssessmentSkipped" : "PostAssessmentSkipped");
            string reasonColumn = e.CommandName == "Attendance" ? "AttendanceSkipReason" : (e.CommandName == "Pre" ? "PreAssessmentSkipReason" : "PostAssessmentSkipReason");
            bool skipped = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(" + flag + ",0) FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) }));
            if (!skipped)
            {
                if (reasonBox == null || string.IsNullOrWhiteSpace(reasonBox.Text)) { ShowError("Skip reason is mandatory."); return; }
                db.ExecuteSql("UPDATE SessionMaster SET " + flag + "=1," + reasonColumn + "=@Reason," + reasonColumn.Replace("Reason", "By") + "=@By," + reasonColumn.Replace("Reason", "On") + "=GETDATE() WHERE SessionID=@SessionID AND TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@Reason", reasonBox.Text.Trim()), new SqlParameter("@By", Actor), new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) });
                ShowSuccess(e.CommandName + " requirement skipped for session " + sessionID + ".");
            }
            else
            {
                db.ExecuteSql("UPDATE SessionMaster SET " + flag + "=0," + reasonColumn + "=NULL," + reasonColumn.Replace("Reason", "By") + "=NULL," + reasonColumn.Replace("Reason", "On") + "=NULL WHERE SessionID=@SessionID AND TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) });
                ShowSuccess(e.CommandName + " requirement unskipped for session " + sessionID + ".");
            }
            LoadSessions();
        }

        private SqlParameter[] P(string name, object value) { return new SqlParameter[] { new SqlParameter(name, value) }; }
        private void ShowError(string text) { lblMessage.ForeColor = Color.Red; lblMessage.Text = text; }
        private void ShowSuccess(string text) { lblMessage.ForeColor = Color.Green; lblMessage.Text = text; }
    }
}