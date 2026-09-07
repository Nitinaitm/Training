using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class TrainingDetails : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        private string TrainingID { get { return Convert.ToString(Session["TrainingID"]); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrEmpty(TrainingID)) { Response.Redirect("~/Trainee/MyTrainings.aspx"); return; }
            if (!IsPostBack) { LoadTrainingSummary(); LoadProgress(); LoadWorkflow(); }
        }

        private void LoadTrainingSummary()
        {
            string sql = @"SELECT TD.TrainingID,TD.Batch,TD.TrainingType,TD.TrainingOrganizer,TD.TrainingLocation,TD.DateFrom,TD.DateTo,TD.TrainingCategory,TD.NoOfDays,TD.StartTime,TD.Remarks,CM.CourseName,TD.AttendanceRequired,TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,TD.FeedbackRequired,TD.CertificateRequired FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TD.TrainingID=@TrainingID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            if (dt.Rows.Count == 0) { Response.Redirect("~/Trainee/MyTrainings.aspx"); return; }
            DataRow r = dt.Rows[0];
            lblTrainingID.Text = Convert.ToString(r["TrainingID"]); lblBatch.Text = Convert.ToString(r["Batch"]); lblTrainingType.Text = Convert.ToString(r["TrainingType"]); lblOrganizer.Text = Convert.ToString(r["TrainingOrganizer"]); lblLocation.Text = Convert.ToString(r["TrainingLocation"]); lblDateFrom.Text = Convert.ToString(r["DateFrom"]); lblDateTo.Text = Convert.ToString(r["DateTo"]); lblCategory.Text = Convert.ToString(r["TrainingCategory"]); lblDays.Text = Convert.ToString(r["NoOfDays"]); lblStartTime.Text = Convert.ToString(r["StartTime"]); lblRemarks.Text = Convert.ToString(r["Remarks"]); lblCourse.Text = Convert.ToString(r["CourseName"]);
        }

        private void LoadProgress()
        {
            string empID = Session["EmpID"].ToString();
            DataTable dt = objDB.GetDataTable(@"SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            if (dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];
            bool attendanceRequired = Convert.ToBoolean(r["AttendanceRequired"]), preRequired = Convert.ToBoolean(r["InitialAssessmentRequired"]), postRequired = Convert.ToBoolean(r["FinalAssessmentRequired"]), feedbackRequired = Convert.ToBoolean(r["FeedbackRequired"]), certificateRequired = Convert.ToBoolean(r["CertificateRequired"]);
            bool attendanceDone = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN NOT EXISTS(SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID) OR NOT EXISTS(SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND NOT EXISTS(SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus='Completed')) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", empID) })) == 1;
            bool preDone = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN NOT EXISTS(SELECT 1 FROM TestMaster T INNER JOIN SessionMaster SM ON SM.SessionID=T.SessionID WHERE SM.TrainingID=@TrainingID AND T.TestType='Pre') OR NOT EXISTS(SELECT 1 FROM TestMaster T INNER JOIN SessionMaster SM ON SM.SessionID=T.SessionID WHERE SM.TrainingID=@TrainingID AND T.TestType='Pre' AND NOT EXISTS(SELECT 1 FROM TestAttempt A WHERE A.TestID=T.TestID AND A.EmpID=@EmpID)) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", empID) })) == 1;
            bool postDone = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN NOT EXISTS(SELECT 1 FROM TestMaster T INNER JOIN SessionMaster SM ON SM.SessionID=T.SessionID WHERE SM.TrainingID=@TrainingID AND T.TestType='Post') OR NOT EXISTS(SELECT 1 FROM TestMaster T INNER JOIN SessionMaster SM ON SM.SessionID=T.SessionID WHERE SM.TrainingID=@TrainingID AND T.TestType='Post' AND NOT EXISTS(SELECT 1 FROM TestAttempt A WHERE A.TestID=T.TestID AND A.EmpID=@EmpID)) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", empID) })) == 1;
            bool batchFeedbackDone = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN EXISTS(SELECT 1 FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", empID) })) == 1;
            bool questionnaireAvailable = Convert.ToInt32(objDB.ExecuteScalar("SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingFeedbackCategory TFC INNER JOIN FeedbackQuestionMaster FQM ON FQM.CategoryID=TFC.CategoryID WHERE TFC.TrainingID=@TrainingID AND FQM.Active=1) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) })) == 1;
            bool attendanceGate = !attendanceRequired || attendanceDone, requiredTestsDone = (!preRequired || preDone) && (!postRequired || postDone), feedbackGate = !feedbackRequired || batchFeedbackDone;
            if (btnBatchFeedback != null) { btnBatchFeedback.Visible = feedbackRequired; btnBatchFeedback.Enabled = feedbackRequired && questionnaireAvailable && attendanceGate && requiredTestsDone && !batchFeedbackDone; }
            if (btnCertificate != null) { btnCertificate.Visible = certificateRequired; btnCertificate.Enabled = certificateRequired && attendanceGate && requiredTestsDone && feedbackGate; }
        }

        private void LoadWorkflow() { }
    }
}