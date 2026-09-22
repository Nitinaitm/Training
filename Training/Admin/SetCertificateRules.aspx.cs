using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class SetCertificateRules : Page
    {
        private readonly clsDataAccess db = new clsDataAccess();
        private string TrainingID { get { return Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString(); } }
        private string Actor { get { return Session["UserID"] == null ? "Admin" : Session["UserID"].ToString(); } }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (Session["Role"] == null || (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin" && Session["Role"].ToString() != "Nodal"))
            {
                Response.Redirect("~/Default.aspx", true);
                return;
            }
            if (string.IsNullOrWhiteSpace(TrainingID))
            {
                Response.Redirect("TrainingList.aspx", true);
                return;
            }
            if (IsTrainingCompleted())
            {
                Response.Redirect("ManageTraining.aspx", true);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblTraining.Text = "Training: " + TrainingID;
                LoadRules();
                LoadSessions();
            }
        }

        private bool IsTrainingCompleted()
        {
            object value = db.ExecuteScalar("SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ' THEN 1 ELSE 0 END FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private void LoadRules()
        {
            DataTable dt = db.GetDataTable("SELECT ISNULL(AttendanceRequired,0) AttendanceRequired,MinimumAttendancePercentage FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
            if (dt.Rows.Count == 0)
            {
                Response.Redirect("TrainingList.aspx", true);
                return;
            }

            bool required = Convert.ToBoolean(dt.Rows[0]["AttendanceRequired"]);
            txtMinimumAttendance.Text = dt.Rows[0]["MinimumAttendancePercentage"] == DBNull.Value ? "" : Convert.ToDecimal(dt.Rows[0]["MinimumAttendancePercentage"]).ToString("0.##");
            lblAttendanceStatus.Text = required ? (string.IsNullOrWhiteSpace(txtMinimumAttendance.Text) ? "Attendance required - minimum percentage not set" : "Attendance required - minimum " + txtMinimumAttendance.Text + "%") : "Attendance is not required for this training.";
            lblAttendanceStatus.ForeColor = required && string.IsNullOrWhiteSpace(txtMinimumAttendance.Text) ? Color.Red : Color.Green;
        }

        private void LoadSessions()
        {
            DataTable dt = db.GetDataTable("SELECT SM.SessionID,SM.SessionNo,ISNULL(SM.SessionName,'') SessionName,ISNULL(TD.InitialAssessmentRequired,0) InitialAssessmentRequired,ISNULL(TD.FinalAssessmentRequired,0) FinalAssessmentRequired,ISNULL(SM.PreAssessmentSkipped,0) PreAssessmentSkipped,ISNULL(SM.PostAssessmentSkipped,0) PostAssessmentSkipped,ISNULL(SM.PreTestCertificateRule,'') PreTestCertificateRule,ISNULL(SM.PostTestCertificateRule,'') PostTestCertificateRule FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.TrainingID=@TrainingID ORDER BY TRY_CONVERT(int,SM.SessionNo),SM.SessionID", P("@TrainingID", TrainingID));

            if (!dt.Columns.Contains("PreApplicable")) dt.Columns.Add("PreApplicable", typeof(bool));
            if (!dt.Columns.Contains("PostApplicable")) dt.Columns.Add("PostApplicable", typeof(bool));
            if (!dt.Columns.Contains("PreState")) dt.Columns.Add("PreState", typeof(string));
            if (!dt.Columns.Contains("PostState")) dt.Columns.Add("PostState", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                bool preRequired = Convert.ToBoolean(row["InitialAssessmentRequired"]);
                bool postRequired = Convert.ToBoolean(row["FinalAssessmentRequired"]);
                bool preSkipped = Convert.ToBoolean(row["PreAssessmentSkipped"]);
                bool postSkipped = Convert.ToBoolean(row["PostAssessmentSkipped"]);
                string preRule = Convert.ToString(row["PreTestCertificateRule"]).Trim().ToUpperInvariant();
                string postRule = Convert.ToString(row["PostTestCertificateRule"]).Trim().ToUpperInvariant();
                bool preApplicable = preRequired && !preSkipped;
                bool postApplicable = postRequired && !postSkipped;

                row["PreApplicable"] = preApplicable;
                row["PostApplicable"] = postApplicable;
                row["PreState"] = !preRequired ? "Not Required" : preSkipped ? "Skipped" : string.IsNullOrWhiteSpace(preRule) ? "Rule Not Set" : preRule == "PASS" ? "PASS" : "ALL";
                row["PostState"] = !postRequired ? "Not Required" : postSkipped ? "Skipped" : string.IsNullOrWhiteSpace(postRule) ? "Rule Not Set" : postRule == "PASS" ? "PASS" : "ALL";
            }

            gvSessions.DataSource = dt;
            gvSessions.DataBind();
        }

        protected void gvSessions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DataRowView row = e.Row.DataItem as DataRowView;
            if (row == null) return;

            DropDownList ddlPre = e.Row.FindControl("ddlPreRule") as DropDownList;
            DropDownList ddlPost = e.Row.FindControl("ddlPostRule") as DropDownList;

            string preRule = Convert.ToString(row["PreTestCertificateRule"]).Trim().ToUpperInvariant();
            string postRule = Convert.ToString(row["PostTestCertificateRule"]).Trim().ToUpperInvariant();

            if (ddlPre != null && (preRule == "PASS" || preRule == "ALL")) ddlPre.SelectedValue = preRule;
            if (ddlPost != null && (postRule == "PASS" || postRule == "ALL")) ddlPost.SelectedValue = postRule;
        }

        protected void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            decimal percentage;
            if (!decimal.TryParse(txtMinimumAttendance.Text.Trim(), out percentage))
            {
                ShowError("Enter minimum attendance percentage between 0 and 100.");
                return;
            }

            if (percentage < 0 || percentage > 100)
            {
                ShowError("Minimum attendance percentage must be between 0 and 100.");
                return;
            }

            bool required = Convert.ToBoolean(db.ExecuteScalar("SELECT ISNULL(AttendanceRequired,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID)));
            if (!required)
            {
                ShowError("Attendance is not required for this training.");
                return;
            }

            db.ExecuteSql("UPDATE TrainingDetails SET MinimumAttendancePercentage=@Percentage,UpdatedOn=GETDATE(),UpdatedBy=@By WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@Percentage", percentage), new SqlParameter("@By", Actor), new SqlParameter("@TrainingID", TrainingID) });
            ShowSuccess("Minimum attendance rule saved successfully.");
            LoadRules();
        }

        protected void gvSessions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SavePre" && e.CommandName != "SavePost") return;

            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string sessionID = Convert.ToString(e.CommandArgument);
            bool isPre = e.CommandName == "SavePre";
            DropDownList ddl = row.FindControl(isPre ? "ddlPreRule" : "ddlPostRule") as DropDownList;

            if (ddl == null || (ddl.SelectedValue != "PASS" && ddl.SelectedValue != "ALL"))
            {
                ShowError("Please select PASS or ALL before saving the rule.");
                return;
            }

            string requiredColumn = isPre ? "InitialAssessmentRequired" : "FinalAssessmentRequired";
            string skipColumn = isPre ? "PreAssessmentSkipped" : "PostAssessmentSkipped";
            string ruleColumn = isPre ? "PreTestCertificateRule" : "PostTestCertificateRule";

            bool applicable = Convert.ToBoolean(db.ExecuteScalar("SELECT CASE WHEN ISNULL(TD." + requiredColumn + ",0)=1 AND ISNULL(SM." + skipColumn + ",0)=0 THEN 1 ELSE 0 END FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) }));
            if (!applicable)
            {
                ShowError("This session/test is not required or is skipped. A certificate rule is not applicable.");
                return;
            }

            db.ExecuteSql("UPDATE SessionMaster SET " + ruleColumn + "=@Rule WHERE SessionID=@SessionID AND TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@Rule", ddl.SelectedValue), new SqlParameter("@SessionID", sessionID), new SqlParameter("@TrainingID", TrainingID) });
            ShowSuccess((isPre ? "Pre-Test" : "Post-Test") + " certificate rule saved for session " + sessionID + ".");
            LoadSessions();
        }

        private SqlParameter[] P(string name, object value)
        {
            return new SqlParameter[] { new SqlParameter(name, value) };
        }

        private void ShowError(string text)
        {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = text;
        }

        private void ShowSuccess(string text)
        {
            lblMessage.ForeColor = Color.Green;
            lblMessage.Text = text;
        }
    }
}