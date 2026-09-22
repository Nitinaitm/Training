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
            DataTable dt = db.GetDataTable(@"SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,CertificateRequired,ISNULL(CertificateSkipped,0) CertificateSkipped,PreTestCertificateRule,PostTestCertificateRule FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
            if (dt.Rows.Count == 0) { Response.Redirect("TrainingList.aspx"); return; }

            DataRow r = dt.Rows[0];
            lblTraining.Text = "Training: " + TrainingID;

            bool ar = Convert.ToBoolean(r["AttendanceRequired"]);
            bool pr = Convert.ToBoolean(r["InitialAssessmentRequired"]);
            bool por = Convert.ToBoolean(r["FinalAssessmentRequired"]);
            bool fr = Convert.ToBoolean(r["FeedbackRequired"]);
            bool fs = Convert.ToBoolean(r["FeedbackSkipped"]);
            bool cr = Convert.ToBoolean(r["CertificateRequired"]);
            bool cs = Convert.ToBoolean(r["CertificateSkipped"]);

            lblAttendanceStatus.Text = ar ? "Required" : "Not Required";
            lblPreStatus.Text = pr ? "Required" : "Not Required";
            lblPostStatus.Text = por ? "Required" : "Not Required";
            lblFeedbackStatus.Text = fs ? "Skipped" : (fr ? "Required" : "Not Required");
            lblCertificateStatus.Text = cs ? "Skipped" : (cr ? "Required" : "Not Required");
            string preRule = r["PreTestCertificateRule"] == DBNull.Value ? "" : r["PreTestCertificateRule"].ToString().Trim().ToUpperInvariant();
            string postRule = r["PostTestCertificateRule"] == DBNull.Value ? "" : r["PostTestCertificateRule"].ToString().Trim().ToUpperInvariant();
            if (ddlPreCertificateRule.Items.FindByValue(preRule) == null) preRule = "";
            if (ddlPostCertificateRule.Items.FindByValue(postRule) == null) postRule = "";
            ddlPreCertificateRule.SelectedValue = preRule;
            ddlPostCertificateRule.SelectedValue = postRule;
            bool preApplicable = pr && HasUnskippedSession("PreAssessmentSkipped");
            bool postApplicable = por && HasUnskippedSession("PostAssessmentSkipped");
            pnlPreCertificateRule.Visible = preApplicable;
            pnlPostCertificateRule.Visible = postApplicable;
            lblCertificateBasis.Text = BuildCertificateRuleStatus(preApplicable, preRule, postApplicable, postRule);

            btnAttendanceRequired.Enabled = !ar;
            btnAttendanceNotRequired.Enabled = ar;
            btnPreRequired.Enabled = !pr;
            btnPreNotRequired.Enabled = pr;
            btnPostRequired.Enabled = !por;
            btnPostNotRequired.Enabled = por;
            btnFeedbackRequired.Enabled = !fr;
            btnFeedbackNotRequired.Enabled = fr;
            btnCertificateRequired.Enabled = !cr;
            btnCertificateNotRequired.Enabled = cr;

            btnFeedback.Visible = fr || fs;
            btnCertificate.Visible = cr || cs;
            btnFeedback.Text = fs ? "Unskip Feedback" : "Skip Feedback";
            btnCertificate.Text = cs ? "Unskip Certificate" : "Skip Certificate";
            btnFeedback.CssClass = fs ? "btn btn-outline-success mt-2 action-btn" : "btn btn-outline-danger mt-2 action-btn";
            btnCertificate.CssClass = cs ? "btn btn-outline-success mt-2 action-btn" : "btn btn-outline-danger mt-2 action-btn";
        }

        private void LoadSessions()
        {
            gvSessions.DataSource = db.GetDataTable(@"SELECT SessionID,ISNULL(AttendanceSkipped,0) AttendanceSkipped,ISNULL(PreAssessmentSkipped,0) PreAssessmentSkipped,ISNULL(PostAssessmentSkipped,0) PostAssessmentSkipped,ISNULL(TD.AttendanceRequired,0) AttendanceRequired,ISNULL(TD.InitialAssessmentRequired,0) InitialAssessmentRequired,ISNULL(TD.FinalAssessmentRequired,0) FinalAssessmentRequired FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.TrainingID=@TrainingID ORDER BY SM.SessionID", P("@TrainingID", TrainingID));
            gvSessions.DataBind();
        }

        protected void btnAttendanceRequired_Click(object sender, EventArgs e) { SetBatchRequirement("AttendanceRequired", true, "AttendanceSkipped", "AttendanceSkipReason"); }
        protected void btnAttendanceNotRequired_Click(object sender, EventArgs e) { SetBatchRequirement("AttendanceRequired", false, "AttendanceSkipped", "AttendanceSkipReason"); }
        protected void btnPreRequired_Click(object sender, EventArgs e) { SetBatchRequirement("InitialAssessmentRequired", true, "PreAssessmentSkipped", "PreAssessmentSkipReason"); }
        protected void btnPreNotRequired_Click(object sender, EventArgs e) { SetBatchRequirement("InitialAssessmentRequired", false, "PreAssessmentSkipped", "PreAssessmentSkipReason"); }
        protected void btnPostRequired_Click(object sender, EventArgs e) { SetBatchRequirement("FinalAssessmentRequired", true, "PostAssessmentSkipped", "PostAssessmentSkipReason"); }
        protected void btnPostNotRequired_Click(object sender, EventArgs e) { SetBatchRequirement("FinalAssessmentRequired", false, "PostAssessmentSkipped", "PostAssessmentSkipReason"); }
        protected void btnFeedbackRequired_Click(object sender, EventArgs e) { SetBatchRequirement("FeedbackRequired", true, "FeedbackSkipped", "FeedbackSkipReason"); }
        protected void btnFeedbackNotRequired_Click(object sender, EventArgs e) { SetBatchRequirement("FeedbackRequired", false, "FeedbackSkipped", "FeedbackSkipReason"); }
        protected void btnCertificateRequired_Click(object sender, EventArgs e) { SetBatchRequirement("CertificateRequired", true, "CertificateSkipped", "CertificateSkipReason"); }

        protected void btnSavePreCertificateRule_Click(object sender, EventArgs e)
        {
            SaveCertificateRule("Pre", ddlPreCertificateRule.SelectedValue);
        }

        protected void btnSavePostCertificateRule_Click(object sender, EventArgs e)
        {
            SaveCertificateRule("Post", ddlPostCertificateRule.SelectedValue);
        }

        private void SaveCertificateRule(string testType, string rule)
        {
            if (rule != "ALL" && rule != "PASS")
            {
                ShowError("Please select a certificate rule.");
                return;
            }
            string skipColumn = testType == "Pre" ? "PreAssessmentSkipped" : "PostAssessmentSkipped";
            bool required = testType == "Pre" ? Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(InitialAssessmentRequired,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID))) : Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(FinalAssessmentRequired,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID)));
            bool applicable = required && HasUnskippedSession(skipColumn);
            if (!applicable)
            {
                ShowError(testType + "-Test is not applicable for any session.");
                return;
            }
            string column = testType == "Pre" ? "PreTestCertificateRule" : "PostTestCertificateRule";
            db.ExecuteSql("UPDATE TrainingDetails SET " + column + "=@Rule,UpdatedOn=GETDATE(),UpdatedBy=@By WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@Rule", rule), new SqlParameter("@By", Actor), new SqlParameter("@TrainingID", TrainingID) });
            ShowSuccess(testType + "-Test certificate rule saved successfully.");
            LoadBatchStatus();
        }

        private bool HasUnskippedSession(string skipColumn)
        {
            if (skipColumn != "PreAssessmentSkipped" && skipColumn != "PostAssessmentSkipped") return false;
            object value = db.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(" + skipColumn + ",0)=0", P("@TrainingID", TrainingID));
            return value != null && Convert.ToInt32(value) > 0;
        }

        private string BuildCertificateRuleStatus(bool preApplicable, string preRule, bool postApplicable, string postRule)
        {
            string preText = !preApplicable ? "Pre-Test: Not Applicable" : (preRule == "" ? "Pre-Test: Rule Not Set" : "Pre-Test: " + (preRule == "PASS" ? "Pass Only" : "All"));
            string postText = !postApplicable ? "Post-Test: Not Applicable" : (postRule == "" ? "Post-Test: Rule Not Set" : "Post-Test: " + (postRule == "PASS" ? "Pass Only" : "All"));
            return preText + " | " + postText;
        }
        protected void btnCertificateNotRequired_Click(object sender, EventArgs e) { SetBatchRequirement("CertificateRequired", false, "CertificateSkipped", "CertificateSkipReason"); }

        private void SetBatchRequirement(string requiredColumn, bool required, string sessionSkipColumn, string sessionReasonColumn)
        {
            string clearSession = "UPDATE SessionMaster SET " + sessionSkipColumn + "=0," + sessionReasonColumn + "=NULL," + sessionReasonColumn.Replace("Reason", "By") + "=NULL," + sessionReasonColumn.Replace("Reason", "On") + "=NULL WHERE TrainingID=@TrainingID";
            string updateTraining = "UPDATE TrainingDetails SET " + requiredColumn + "=@Required,UpdatedOn=GETDATE(),UpdatedBy=@By WHERE TrainingID=@TrainingID";
            db.ExecuteSql(updateTraining, new SqlParameter[] { new SqlParameter("@Required", required), new SqlParameter("@By", Actor), new SqlParameter("@TrainingID", TrainingID) });

            if (!required) db.ExecuteSql(clearSession, P("@TrainingID", TrainingID));

            ShowSuccess((required ? "Required" : "Not Required") + " setting updated successfully.");
            LoadBatchStatus();
            LoadSessions();
        }

        protected void btnFeedback_Click(object sender, EventArgs e) { ToggleBatch("Feedback", "FeedbackRequired", "FeedbackSkipped", "FeedbackSkipReason", txtFeedbackReason.Text.Trim()); }
        protected void btnCertificate_Click(object sender, EventArgs e) { ToggleBatch("Certificate", "CertificateRequired", "CertificateSkipped", "CertificateSkipReason", txtCertificateReason.Text.Trim()); }

        private void ToggleBatch(string label, string requiredColumn, string flag, string reasonColumn, string reason)
        {
            bool required = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(" + requiredColumn + ",0) FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID)));
            bool skipped = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(" + flag + ",0) FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID)));

            if (!skipped)
            {
                if (!required) { ShowError(label + " is not required for this training."); return; }
                if (string.IsNullOrWhiteSpace(reason)) { ShowError("Skip reason is mandatory for " + label + "."); return; }
                db.ExecuteSql("UPDATE TrainingDetails SET " + flag + "=1," + reasonColumn + "=@Reason," + reasonColumn.Replace("Reason", "By") + "=@By," + reasonColumn.Replace("Reason", "On") + "=GETDATE(),UpdatedOn=GETDATE(),UpdatedBy=@By WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@Reason", reason), new SqlParameter("@By", Actor), new SqlParameter("@TrainingID", TrainingID) });
                ShowSuccess(label + " has been skipped.");
            }
            else
            {
                db.ExecuteSql("UPDATE TrainingDetails SET " + flag + "=0," + reasonColumn + "=NULL," + reasonColumn.Replace("Reason", "By") + "=NULL," + reasonColumn.Replace("Reason", "On") + "=NULL,UpdatedOn=GETDATE(),UpdatedBy=@By WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@By", Actor), new SqlParameter("@TrainingID", TrainingID) });
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
            string requiredColumn = e.CommandName == "Attendance" ? "AttendanceRequired" : (e.CommandName == "Pre" ? "InitialAssessmentRequired" : "FinalAssessmentRequired");

            bool required = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(TD." + requiredColumn + ",0) FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) }));
            bool skipped = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(" + flag + ",0) FROM SessionMaster WHERE SessionID=@SessionID AND TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) }));

            if (!skipped)
            {
                if (!required) { ShowError("This requirement is not enabled for the training."); return; }
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