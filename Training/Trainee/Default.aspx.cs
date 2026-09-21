using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class Default : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        string EmpID = "";

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString())) return;
            if (Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase)) return;
            EmpID = Session["EmpID"].ToString().Trim().ToUpperInvariant();
            BindClosedTraining();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
            EmpID = Session["EmpID"].ToString().Trim().ToUpperInvariant();
            if (!IsPostBack) LoadDashboard();
        }

        private void LoadDashboard()
        {
            LoadTraineeDetails();
            LoadDashboardSummary();
            LoadProgress();
        }

        private void LoadTraineeDetails()
        {
            lblTraineeID.Text = EmpID;
            string sql = "SELECT EmpName,'Internal' AS TraineeType FROM EmpBasicMaster WHERE EmpID=@EmpID UNION ALL SELECT TraineeName AS EmpName,'External' AS TraineeType FROM TraineeMasterExternal WHERE EmpIDExternal=@EmpID";
            SqlParameter[] param = { new SqlParameter("@EmpID", EmpID) };
            DataTable dt = objDB.GetDataTable(sql, param);
            if (dt == null || dt.Rows.Count == 0)
            {
                lblTraineeName.Text = EmpID;
                lblTraineeType.Text = "Trainee";
                return;
            }
            lblTraineeName.Text = dt.Rows[0]["EmpName"].ToString();
            lblTraineeType.Text = dt.Rows[0]["TraineeType"].ToString();
        }

        private void LoadDashboardSummary()
        {
            string sql = "SELECT (SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned') AS TotalTraining,(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0) AND NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(SM.AttendanceStatus,'')<>'Completed')) AS AttendanceCompleted,(SELECT COUNT(*) FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0))) AS PublishedTests,(SELECT COUNT(*) FROM TestMaster TM INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID INNER JOIN TrainingAssignment TA ON TA.TrainingID=SM.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TM.IsPublished=1 AND ((TM.TestType='Pre' AND TD.InitialAssessmentRequired=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0) OR (TM.TestType='Post' AND TD.FinalAssessmentRequired=1 AND ISNULL(SM.PostAssessmentSkipped,0)=0)) AND EXISTS (SELECT 1 FROM TestAttempt TAT WHERE TAT.TestID=TM.TestID AND TAT.EmpID=@EmpID AND TAT.Submitted=1)) AS CompletedTests,(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.FeedbackRequired=1 AND ISNULL(TD.FeedbackSkipped,0)=0) AS RequiredFeedback,(SELECT COUNT(DISTINCT BF.TrainingID) FROM BatchFeedback BF INNER JOIN TrainingAssignment TA ON TA.TrainingID=BF.TrainingID WHERE BF.EmpID=@EmpID AND ISNULL(BF.Submitted,0)=1 AND TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned') AS FeedbackCompleted,(SELECT COUNT(DISTINCT TC.TrainingID) FROM TrainingCertificate TC INNER JOIN TrainingAssignment TA ON TA.TrainingID=TC.TrainingID WHERE TC.EmpID=@EmpID AND TC.CertificateStatus='A' AND TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned') AS CertificateGenerated,(SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.CertificateRequired=1 AND ISNULL(TD.CertificateSkipped,0)=0) AS RequiredCertificate";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            if (dt == null || dt.Rows.Count == 0) { SetDashboardZero(); return; }
            int totalTraining = GetIntValue(dt.Rows[0]["TotalTraining"]);
            int attendanceCompleted = GetIntValue(dt.Rows[0]["AttendanceCompleted"]);
            int publishedTests = GetIntValue(dt.Rows[0]["PublishedTests"]);
            int completedTests = GetIntValue(dt.Rows[0]["CompletedTests"]);
            int requiredFeedback = GetIntValue(dt.Rows[0]["RequiredFeedback"]);
            int feedbackCompleted = GetIntValue(dt.Rows[0]["FeedbackCompleted"]);
            int certificateGenerated = GetIntValue(dt.Rows[0]["CertificateGenerated"]);
            int requiredCertificate = GetIntValue(dt.Rows[0]["RequiredCertificate"]);
            int pendingTests = publishedTests - completedTests;
            if (pendingTests < 0) pendingTests = 0;
            int feedbackPending = requiredFeedback - feedbackCompleted;
            if (feedbackPending < 0) feedbackPending = 0;
            lblTrainingCount.Text = totalTraining.ToString();
            lblAttendance.Text = attendanceCompleted.ToString();
            lblPublishedTests.Text = publishedTests.ToString();
            lblCompletedTests.Text = completedTests.ToString();
            lblPendingTests.Text = pendingTests.ToString();
            lblBatchFeedback.Text = feedbackCompleted.ToString();
            lblCertificate.Text = certificateGenerated.ToString();
            lblStatusTraining.Text = totalTraining.ToString();
            lblStatusTests.Text = publishedTests.ToString();
            lblStatusPendingTests.Text = pendingTests.ToString();
            lblStatusFeedback.Text = feedbackPending.ToString();
            lblStatusCertificate.Text = certificateGenerated.ToString();
            Session["DashboardRequiredFeedback"] = requiredFeedback;
            Session["DashboardRequiredCertificate"] = requiredCertificate;
            Session["DashboardRequiredAttendance"] = GetRequiredAttendanceCount();
        }

        private void BindClosedTraining()
        {
            if (string.IsNullOrWhiteSpace(EmpID)) return;
            string sql = "SELECT DISTINCT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateFrom,105),105) AS DateFrom,CONVERT(varchar(10),TRY_CONVERT(date,TD.DateTo,105),105) AS DateTo FROM TrainingDetails TD INNER JOIN TrainingAssignment TA ON TA.TrainingID=TD.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.TrainingStatus='Closed' ORDER BY TRY_CONVERT(date,TD.DateFrom,105) DESC";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            ContentPlaceHolder content = Master.FindControl("ContentPlaceHolder1") as ContentPlaceHolder;
            if (content == null) return;
            Literal heading = new Literal();
            heading.Text = "<div class='dashboard-section' style='margin-top:22px;'><div class='section-heading'><i class='fa fa-history'></i> Closed Trainings</div>";
            content.Controls.Add(heading);
            GridView gv = new GridView();
            gv.ID = "gvClosedTraining";
            gv.AutoGenerateColumns = false;
            gv.CssClass = "table table-bordered table-hover";
            gv.Width = Unit.Percentage(100);
            gv.EmptyDataText = "No Closed Training";
            gv.ShowHeaderWhenEmpty = true;
            gv.DataKeyNames = new[] { "TrainingID" };
            gv.RowCommand += gvClosedTraining_RowCommand;
            gv.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(108,117,125);
            gv.HeaderStyle.ForeColor = System.Drawing.Color.White;
            gv.HeaderStyle.Font.Bold = true;
            gv.Columns.Add(new BoundField { DataField = "TrainingID", HeaderText = "Training ID" });
            gv.Columns.Add(new BoundField { DataField = "TrainingType", HeaderText = "Training Type" });
            gv.Columns.Add(new BoundField { DataField = "TrainingOrganizer", HeaderText = "Organizer" });
            gv.Columns.Add(new BoundField { DataField = "Batch", HeaderText = "Batch" });
            gv.Columns.Add(new BoundField { DataField = "DateFrom", HeaderText = "From" });
            gv.Columns.Add(new BoundField { DataField = "DateTo", HeaderText = "To" });
            TemplateField status = new TemplateField();
            status.HeaderText = "Status";
            status.ItemTemplate = new ClosedStatusTemplate();
            gv.Columns.Add(status);
            TemplateField history = new TemplateField();
            history.HeaderText = "History";
            history.ItemTemplate = new ClosedHistoryTemplate();
            gv.Columns.Add(history);
            gv.DataSource = dt;
            gv.DataBind();
            content.Controls.Add(gv);
            content.Controls.Add(new Literal { Text = "</div>" });
        }

        private sealed class ClosedStatusTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Label label = new Label { Text = "Closed", CssClass = "badge bg-secondary" };
                container.Controls.Add(label);
            }
        }

        private sealed class ClosedHistoryTemplate : ITemplate
        {
            public void InstantiateIn(Control container)
            {
                Button button = new Button { Text = "View History", CommandName = "History", CssClass = "btn btn-secondary btn-sm", CausesValidation = false };
                container.Controls.Add(button);
            }
        }

        private void gvClosedTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "History") return;
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            GridView gv = (GridView)sender;
            string trainingID = gv.DataKeys[row.RowIndex].Value.ToString();
            object access = objDB.ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND AssignmentStatus='Assigned' AND EXISTS (SELECT 1 FROM TrainingDetails TD WHERE TD.TrainingID=TrainingAssignment.TrainingID AND TD.TrainingStatus='Closed')", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", EmpID) });
            if (access == null || Convert.ToInt32(access) == 0) return;
            Response.Redirect("~/TrainingHistory.aspx?TrainingID=" + Server.UrlEncode(trainingID));
        }

        private int GetRequiredAttendanceCount()
        {
            string sql = "SELECT COUNT(DISTINCT TA.TrainingID) FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID WHERE TA.EmpID=@EmpID AND TA.AssignmentStatus='Assigned' AND TD.AttendanceRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0)";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@EmpID", EmpID) });
            if (dt == null || dt.Rows.Count == 0) return 0;
            return GetIntValue(dt.Rows[0][0]);
        }

        private void LoadProgress()
        {
            int attendanceCompleted = GetLabelValue(lblAttendance.Text);
            int publishedTests = GetLabelValue(lblPublishedTests.Text);
            int completedTests = GetLabelValue(lblCompletedTests.Text);
            int feedbackCompleted = GetLabelValue(lblBatchFeedback.Text);
            int certificateGenerated = GetLabelValue(lblCertificate.Text);
            int requiredAttendance = Session["DashboardRequiredAttendance"] == null ? 0 : GetIntValue(Session["DashboardRequiredAttendance"]);
            int requiredFeedback = Session["DashboardRequiredFeedback"] == null ? 0 : GetIntValue(Session["DashboardRequiredFeedback"]);
            int requiredCertificate = Session["DashboardRequiredCertificate"] == null ? 0 : GetIntValue(Session["DashboardRequiredCertificate"]);
            lblProgressAttendance.Text = attendanceCompleted + "/" + requiredAttendance;
            SetProgressBar(barAttendance, attendanceCompleted, requiredAttendance);
            lblProgressTests.Text = completedTests + "/" + publishedTests;
            SetProgressBar(barTests, completedTests, publishedTests);
            lblProgressFeedback.Text = feedbackCompleted + "/" + requiredFeedback;
            SetProgressBar(barFeedback, feedbackCompleted, requiredFeedback);
            lblProgressCertificate.Text = certificateGenerated + "/" + requiredCertificate;
            SetProgressBar(barCertificate, certificateGenerated, requiredCertificate);
        }

        private void SetProgressBar(Panel panel, int completed, int total)
        {
            int percentage = 0;
            if (total > 0) percentage = Convert.ToInt32((completed * 100.0) / total);
            if (percentage > 100) percentage = 100;
            if (percentage < 0) percentage = 0;
            panel.Style["width"] = percentage + "%";
            panel.Attributes["aria-valuenow"] = percentage.ToString();
            panel.Attributes["aria-valuemin"] = "0";
            panel.Attributes["aria-valuemax"] = "100";
        }

        private int GetIntValue(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString())) return 0;
            int result = 0;
            Int32.TryParse(value.ToString(), out result);
            return result;
        }

        private int GetLabelValue(string value)
        {
            int result = 0;
            Int32.TryParse(value, out result);
            return result;
        }

        private void SetDashboardZero()
        {
            lblTrainingCount.Text = "0";
            lblAttendance.Text = "0";
            lblPublishedTests.Text = "0";
            lblCompletedTests.Text = "0";
            lblPendingTests.Text = "0";
            lblBatchFeedback.Text = "0";
            lblCertificate.Text = "0";
            lblStatusTraining.Text = "0";
            lblStatusTests.Text = "0";
            lblStatusPendingTests.Text = "0";
            lblStatusFeedback.Text = "0";
            lblStatusCertificate.Text = "0";
            Session["DashboardRequiredAttendance"] = 0;
            Session["DashboardRequiredFeedback"] = 0;
            Session["DashboardRequiredCertificate"] = 0;
        }

        protected void lnkMyTraining_Click(object sender, EventArgs e) { Response.Redirect("~/Trainee/MyTrainings.aspx"); }
        protected void lnkAttendance_Click(object sender, EventArgs e) { Response.Redirect("~/Trainee/Attendance.aspx"); }
        protected void lnkBatchFeedback_Click(object sender, EventArgs e) { Response.Redirect("~/Trainee/MyTrainings.aspx"); }
        protected void lnkCertificate_Click(object sender, EventArgs e) { Response.Redirect("~/Trainee/MyCertificate.aspx"); }
        protected void lnkPendingTests_Click(object sender, EventArgs e) { Response.Redirect("~/Trainee/MyTrainings.aspx"); }
    }
}
