using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class TrainingDetails : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        private string TrainingID = "";
        private string EmpID = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (Session["TrainingID"] == null) { Response.Redirect("MyTrainings.aspx"); return; }
            EmpID = Session["EmpID"].ToString().ToUpperInvariant();
            TrainingID = Session["TrainingID"].ToString();
            if (!IsPostBack)
            {
                TraineeTrainingSummary1.LoadTraining(TrainingID, EmpID);
                LoadTrainingSummary(); LoadSessionGrid(); LoadProgress(); LoadWorkflow();
            }
        }

        protected void gvSession_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            string pre = DataBinder.Eval(e.Row.DataItem, "PreStatus").ToString(); string post = DataBinder.Eval(e.Row.DataItem, "PostStatus").ToString();
            Label lblPre = (Label)e.Row.FindControl("lblPre"); Label lblPost = (Label)e.Row.FindControl("lblPost");
            if (lblPre != null) lblPre.CssClass = GetBadgeClass(pre); if (lblPost != null) lblPost.CssClass = GetBadgeClass(post);
        }

        private string GetBadgeClass(string status)
        {
            switch (status) { case "Completed": return "badge badge-success"; case "Available": return "badge badge-primary"; case "Skipped": return "badge badge-secondary"; case "Locked": return "badge badge-secondary"; case "Pending": return "badge badge-warning"; default: return "badge badge-light"; }
        }

        private void LoadSessionGrid()
        {
            string sql = "SELECT SM.SessionID,SM.SessionNo,SM.SessionName,TM.TopicName,CASE WHEN TR.TrainerType='Internal' THEN ISNULL(EB.EmpName,'') ELSE ISNULL(TR.NameExternal,'') END AS TrainerName,TRY_CONVERT(date,SM.SessionDate,105) AS SessionDate,SM.StartTime,SM.EndTime,CASE WHEN TD.AttendanceRequired=0 THEN '-' WHEN ISNULL(SM.AttendanceSkipped,0)=1 THEN 'Skipped' ELSE ISNULL(SA.AttendanceStatus,'Pending') END AS AttendanceStatus,CASE WHEN TD.InitialAssessmentRequired=0 THEN '-' WHEN ISNULL(SM.PreAssessmentSkipped,0)=1 THEN 'Skipped' WHEN TD.AttendanceRequired=1 AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(SA.AttendanceStatus,'Pending')<>'Present' THEN 'Locked' WHEN NOT EXISTS (SELECT 1 FROM TestMaster TT WHERE TT.SessionID=SM.SessionID AND TT.TestType='Pre' AND TT.IsPublished=1) THEN '-' WHEN EXISTS (SELECT 1 FROM TestMaster TT INNER JOIN TestAttempt TA ON TT.TestID=TA.TestID WHERE TT.SessionID=SM.SessionID AND TT.TestType='Pre' AND TT.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 'Completed' ELSE 'Available' END AS PreStatus,CASE WHEN TD.FinalAssessmentRequired=0 THEN '-' WHEN ISNULL(SM.PostAssessmentSkipped,0)=1 THEN 'Skipped' WHEN TD.AttendanceRequired=1 AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(SA.AttendanceStatus,'Pending')<>'Present' THEN 'Locked' WHEN NOT EXISTS (SELECT 1 FROM TestMaster TT WHERE TT.SessionID=SM.SessionID AND TT.TestType='Post' AND TT.IsPublished=1) THEN '-' WHEN EXISTS (SELECT 1 FROM TestMaster TT INNER JOIN TestAttempt TA ON TT.TestID=TA.TestID WHERE TT.SessionID=SM.SessionID AND TT.TestType='Post' AND TT.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 'Completed' ELSE 'Available' END AS PostStatus FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID LEFT JOIN TopicMaster TM ON TM.TopicID=SM.TopicID LEFT JOIN TrainerMaster TR ON TR.TrainerID=SM.TrainerID LEFT JOIN EmpBasicMaster EB ON EB.EmpID=TR.EmpID LEFT JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID WHERE SM.TrainingID=@TrainingID ORDER BY TRY_CONVERT(INT,SM.SessionNo),SM.SessionNo";
            SqlParameter[] param = { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) };
            DataTable dt = objDB.GetDataTable(sql, param); gvSession.DataSource = dt; gvSession.DataBind(); ViewState["CompletedSession"] = dt.Select("AttendanceStatus='Completed' OR AttendanceStatus='Skipped'").Length; ViewState["PendingSession"] = dt.Rows.Count - Convert.ToInt32(ViewState["CompletedSession"]);
        }

        private void LoadTrainingSummary()
        {
            string sql = "SELECT TD.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) DateFrom,TRY_CONVERT(date,TD.DateTo,105) DateTo,(SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID) TotalSession FROM TrainingDetails TD INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TD.TrainingID=@TrainingID";
            SqlParameter[] param = { new SqlParameter("@TrainingID", TrainingID) }; DataTable dt = objDB.GetDataTable(sql, param); if (dt.Rows.Count == 0) { Response.Redirect("MyTrainings.aspx"); return; } ViewState["TotalSession"] = dt.Rows[0]["TotalSession"];
        }

        protected void gvSession_RowCommand(object sender, GridViewCommandEventArgs e) { if (e.CommandName == "ViewSession") { Session["SessionID"] = e.CommandArgument.ToString(); Response.Redirect("MySessions.aspx", false); } }

        private void LoadProgress()
        {
            string sql = "SELECT COUNT(*) TotalSession,SUM(CASE WHEN ISNULL(SM.AttendanceSkipped,0)=1 OR ISNULL(SA.AttendanceStatus,'Pending')='Completed' THEN 1 ELSE 0 END) AttendanceCompleted FROM SessionMaster SM LEFT JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID WHERE SM.TrainingID=@TrainingID";
            SqlParameter[] param = { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) }; DataTable dt = objDB.GetDataTable(sql, param); if (dt.Rows.Count == 0) { progressBar.Style["width"] = "0%"; lblProgress.Text = "0%"; return; }
            int total = Convert.ToInt32(dt.Rows[0]["TotalSession"]); int completed = dt.Rows[0]["AttendanceCompleted"] == DBNull.Value ? 0 : Convert.ToInt32(dt.Rows[0]["AttendanceCompleted"]); int percentage = total > 0 ? completed * 100 / total : 0; progressBar.Style["width"] = percentage + "%"; progressBar.Attributes["aria-valuenow"] = percentage.ToString(); lblProgress.Text = percentage + "%"; lblNextActivity.Text = completed == total ? "Complete required training activities" : "Complete Remaining Sessions";
        }

        private bool AreTestsDoneForTrainee(string testType)
        {
            string skipColumn = testType == "Pre" ? "PreAssessmentSkipped" : "PostAssessmentSkipped";
            string q = @"SELECT CASE WHEN NOT EXISTS (
SELECT 1 FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID
WHERE SM.TrainingID=@TrainingID AND ISNULL(SM." + skipColumn + @",0)=0
AND (ISNULL(TD.AttendanceRequired,0)=0 OR ISNULL(SM.AttendanceSkipped,0)=1 OR EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus='Present'))
AND NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1)
) THEN 1 ELSE 0 END";
            return Convert.ToInt32(objDB.ExecuteScalar(q, new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID), new SqlParameter("@TestType", testType) })) == 1;
        }

        private void LoadWorkflow()
        {
            string sql = "SELECT TD.AttendanceRequired,TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,TD.FeedbackRequired,TD.CertificateRequired,ISNULL(TP.BatchFeedbackCompleted,0) AS BatchFeedbackCompleted,CASE WHEN NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL((SELECT TOP 1 AttendanceStatus FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID),'Pending')<>'Completed') THEN 1 ELSE 0 END AS AttendanceDone,CASE WHEN EXISTS (SELECT 1 FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A') THEN 1 ELSE 0 END AS CertificateReady FROM TrainingDetails TD LEFT JOIN TrainingProgress TP ON TP.TrainingID=TD.TrainingID AND TP.EmpID=@EmpID WHERE TD.TrainingID=@TrainingID";
            SqlParameter[] param = { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) }; DataTable dt = objDB.GetDataTable(sql, param); if (dt.Rows.Count == 0) { btnBatchFeedback.Visible = false; btnCertificate.Visible = false; btnBatchFeedback.Enabled = false; btnCertificate.Enabled = false; return; }
            DataRow dr = dt.Rows[0]; bool attendanceRequired = Convert.ToBoolean(dr["AttendanceRequired"]), preRequired = Convert.ToBoolean(dr["InitialAssessmentRequired"]), postRequired = Convert.ToBoolean(dr["FinalAssessmentRequired"]), feedbackRequired = Convert.ToBoolean(dr["FeedbackRequired"]), certificateRequired = Convert.ToBoolean(dr["CertificateRequired"]); bool attendanceDone = Convert.ToBoolean(dr["AttendanceDone"]), batchFeedbackDone = Convert.ToBoolean(dr["BatchFeedbackCompleted"]), certificateReady = Convert.ToBoolean(dr["CertificateReady"]);
            bool questionnaireAvailable = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingFeedbackCategory TFC INNER JOIN FeedbackQuestionMaster FQM ON FQM.CategoryID=TFC.CategoryID WHERE TFC.TrainingID=@TrainingID AND FQM.Active=1) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) })) == 1;
            bool preDone = !preRequired || AreTestsDoneForTrainee("Pre"); bool postDone = !postRequired || AreTestsDoneForTrainee("Post");
            bool attendanceGate = !attendanceRequired || attendanceDone; bool requiredTestsDone = preDone && postDone; bool feedbackGate = !feedbackRequired || batchFeedbackDone; bool workflowComplete = certificateRequired && attendanceGate && requiredTestsDone && feedbackGate;
            if (workflowComplete && !certificateReady) { Training.Business.Certificate.CertificateGenerator generator = new Training.Business.Certificate.CertificateGenerator(); generator.GenerateCertificate(TrainingID, EmpID); certificateReady = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A') THEN 1 ELSE 0 END", param)) == 1; }
            btnBatchFeedback.Visible = feedbackRequired; btnCertificate.Visible = certificateRequired; btnBatchFeedback.Enabled = feedbackRequired && questionnaireAvailable && attendanceGate && requiredTestsDone && !batchFeedbackDone; btnCertificate.Enabled = certificateRequired && attendanceGate && requiredTestsDone && feedbackGate;
            if (gvSession.Columns.Count >= 10) { gvSession.Columns[6].Visible = attendanceRequired; gvSession.Columns[7].Visible = preRequired; gvSession.Columns[8].Visible = postRequired; }
        }

        protected void btnBatchFeedback_Click(object sender, EventArgs e) { Session["TrainingID"] = TrainingID; Response.Redirect("TraineeFeedback.aspx", false); }
        protected void btnCertificate_Click(object sender, EventArgs e) { Session["TrainingID"] = TrainingID; Session["CertificateFromTraining"] = true; Response.Redirect("MyCertificate.aspx", false); }
        protected void btnBack_Click(object sender, EventArgs e) { Response.Redirect("MyTrainings.aspx"); }
    }
}