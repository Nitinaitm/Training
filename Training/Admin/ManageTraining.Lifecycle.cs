using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;

namespace Training.Admin
{
    public partial class ManageTraining
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            // Certificate configuration is allowed before Start Training and does not
            // depend on trainee assignment. Replace the old click handler for this page.
            if (btnCertificateTemplate != null)
            {
                btnCertificateTemplate.Click -= btnCertificateTemplate_Click;
                btnCertificateTemplate.Click += FlexibleCertificateTemplate_Click;
            }
        }

        private void FlexibleCertificateTemplate_Click(object sender, EventArgs e)
        {
            if (!GetLifecycleBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", Convert.ToString(Session["TrainingID"])))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Certificate is not required for this training.";
                return;
            }
            Response.Redirect("CertificateTemplate.aspx");
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (IsPostBack && IsTrainingCompleted()) Response.Redirect("TrainingList.aspx", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            bool started = GetLifecycleBool("SELECT CASE WHEN ISNULL(WorkflowStatus,'') LIKE '%E%' THEN 1 ELSE 0 END", trainingID);
            bool completed = IsTrainingCompleted();
            if (completed)
            {
                btnUpdateTraining.Visible = false; btnAssignSession.Visible = false; btnAssignTrainee.Visible = false; btnRequirements.Visible = false;
                btnAssignFeedback.Visible = false; btnAssignHostel.Visible = false; btnCertificateTemplate.Visible = false; btnStartTraining.Visible = false; btnAttendance.Visible = true; btnCloseTraining.Visible = false;
            }
            else
            {
                btnCloseTraining.Visible = started;
                btnCloseTraining.Enabled = started;
                btnRequirements.Visible = started;
                if (!started)
                {
                    bool certificateRequired = GetLifecycleBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                    bool certificateSkipped = GetLifecycleBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                    btnCertificateTemplate.Visible = certificateRequired && !certificateSkipped;
                    btnCertificateTemplate.Enabled = certificateRequired && !certificateSkipped;
                }
                else
                {
                    btnCertificateTemplate.Visible = false; btnCertificateTemplate.Enabled = false; btnStartTraining.Enabled = false; btnRequirements.Visible = true;
                }
            }
            BuildFlexibleLifecycle(trainingID);
            base.OnPreRender(e);
        }

        private bool GetLifecycleBool(string sql, string trainingID, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(trainingID)) return false;
            SqlParameter[] allParameters = new SqlParameter[parameters.Length + 1];
            allParameters[0] = new SqlParameter("@TrainingID", trainingID);
            for (int i = 0; i < parameters.Length; i++) allParameters[i + 1] = parameters[i];
            object value = new clsDataAccess().ExecuteScalar(sql, allParameters);
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private int GetLifecycleCount(string sql, string trainingID, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(trainingID)) return 0;
            SqlParameter[] allParameters = new SqlParameter[parameters.Length + 1];
            allParameters[0] = new SqlParameter("@TrainingID", trainingID);
            for (int i = 0; i < parameters.Length; i++) allParameters[i + 1] = parameters[i];
            object value = new clsDataAccess().ExecuteScalar(sql, allParameters);
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private bool IsTrainingCompleted()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID)) return false;
            return GetLifecycleBool("SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ' THEN 1 ELSE 0 END FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
        }
        private string LifecycleStageHtml(string label, bool required, bool complete, bool skipped, ref int number)
        {
            string css, state, bubble;
            if (!required) { css = "na"; state = "Not Required"; bubble = "–"; }
            else if (skipped) { css = "na"; state = "Skipped"; bubble = "–"; }
            else if (complete) { css = "done"; state = "✓ Completed"; bubble = "✓"; }
            else { css = "pending"; state = "Pending"; bubble = number.ToString(); }
            if (required && !skipped && !complete) number++;
            return "<div class='stage-item " + css + "'><div class='stage-bubble'>" + bubble + "</div><div class='stage-label'>" + label + "</div><div class='stage-state'>" + state + "</div></div>";
        }
        private void BuildFlexibleLifecycle(string trainingID)
        {
            int n = 1;
            if (string.IsNullOrWhiteSpace(trainingID)) { litBatchLifecycle.Text = "<div class='stage-line'>" + LifecycleStageHtml("Create Batch", true, false, false, ref n) + "</div>"; return; }
            bool attendanceReq = GetLifecycleBool("SELECT AttendanceRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool preReq = GetLifecycleBool("SELECT InitialAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool postReq = GetLifecycleBool("SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool feedbackReq = GetLifecycleBool("SELECT FeedbackRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool certReq = GetLifecycleBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool feedbackSkip = GetLifecycleBool("SELECT ISNULL(FeedbackSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool certSkip = GetLifecycleBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            int sessionCount = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID", trainingID);
            int trainerCount = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>''", trainingID);
            bool allSessionsTrainers = sessionCount > 0 && trainerCount == sessionCount;
            bool trainees = GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND AssignmentStatus='Assigned'", trainingID) > 0;
            bool feedbackAssigned = GetLifecycleCount("SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", trainingID) > 0;
            bool certConfigured = GetLifecycleCount("SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>'' AND ISNULL(CourseTitle,'')<>''", trainingID) > 0;
            int attendanceApplicable = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(AttendanceSkipped,0)=0", trainingID);
            bool attendanceSkipped = sessionCount > 0 && attendanceApplicable == 0;
            bool attendanceDone = !attendanceReq || attendanceSkipped || (sessionCount > 0 && trainees && GetLifecycleCount(@"SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.AttendanceSkipped,0)=0 AND ISNULL(S.AttendanceStatus,'')<>'Completed'", trainingID) == 0);
            int preApplicable = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(PreAssessmentSkipped,0)=0", trainingID);
            bool preSkipped = sessionCount > 0 && preApplicable == 0;
            bool preDone = !preReq || preSkipped || (trainees && preApplicable > 0 && GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.PreAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1) OR EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=S.TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=S.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)))", trainingID) == 0);
            int postApplicable = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(PostAssessmentSkipped,0)=0", trainingID);
            bool postSkipped = sessionCount > 0 && postApplicable == 0;
            bool postDone = !postReq || postSkipped || (trainees && postApplicable > 0 && GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.PostAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType='Post' AND TM.IsPublished=1) OR EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=S.TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=S.SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)))", trainingID) == 0);
            bool feedbackDone = !feedbackReq || feedbackSkip || (trainees && GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=A.TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)", trainingID) == 0);
            bool certDone = !certReq || certSkip || (trainees && GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')", trainingID) == 0);
            StringBuilder h = new StringBuilder();
            h.Append(LifecycleStageHtml("Create Batch", true, true, false, ref n));
            h.Append(LifecycleStageHtml("Assign Sessions & Trainers", true, allSessionsTrainers, false, ref n));
            h.Append(LifecycleStageHtml("Assign Trainees", true, trainees, false, ref n));
            h.Append(LifecycleStageHtml("Feedback Assigned", feedbackReq, feedbackAssigned, feedbackSkip, ref n));
            h.Append(LifecycleStageHtml("Certificate Template", certReq, certConfigured, certSkip, ref n));
            h.Append(LifecycleStageHtml("Attendance Completed", attendanceReq, attendanceDone, attendanceSkipped, ref n));
            h.Append(LifecycleStageHtml("Pre-Test Completed", preReq, preDone, preSkipped, ref n));
            h.Append(LifecycleStageHtml("Post-Test Completed", postReq, postDone, postSkipped, ref n));
            h.Append(LifecycleStageHtml("Feedback Submitted", feedbackReq, feedbackDone, feedbackSkip, ref n));
            h.Append(LifecycleStageHtml("Certificate Generated", certReq, certDone, certSkip, ref n));
            litBatchLifecycle.Text = "<div class='stage-line'>" + h.ToString() + "</div>";
        }
    }
}            int feedbackDone = feedbackReq && !feedbackSkip ? GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=A.TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)", trainingID) : 0;
            int certificateDone = certReq && !certSkip ? GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')", trainingID) : 0;
            bool trainingStarted = GetLifecycleBool("SELECT CASE WHEN ISNULL(WorkflowStatus,'') LIKE '%E%' OR ISNULL(TrainingStatus,'') IN ('InProgress','AttendanceCompleted','TrainingCompleted') THEN 1 ELSE 0 END", trainingID);

            StringBuilder batch = new StringBuilder();
            batch.Append(LifecycleStageHtml("Create Batch", true, true, false, 0, 0, ref n));
            batch.Append(LifecycleStageHtml("Assign Sessions & Trainers", true, allSessionsTrainers, false, 0, 0, ref n));
            batch.Append(LifecycleStageHtml("Assign Trainees", true, trainees, false, 0, 0, ref n));
            batch.Append(LifecycleStageHtml("Feedback Assigned", feedbackReq, feedbackAssigned, feedbackSkip, 0, 0, ref n));
            batch.Append(LifecycleStageHtml("Certificate Template", certReq, certConfigured, certSkip, 0, 0, ref n));
            batch.Append(LifecycleStageHtml("Training Started", true, trainingStarted, false, 0, 0, ref n));
            batch.Append(LifecycleStageHtml("Feedback Submitted", feedbackReq, feedbackDone >= traineeCount && traineeCount > 0, feedbackSkip, feedbackDone, traineeCount, ref n));
            batch.Append(LifecycleStageHtml("Certificate Generated", certReq, certificateDone >= traineeCount && traineeCount > 0, certSkip, certificateDone, traineeCount, ref n));

            StringBuilder all = new StringBuilder();
            all.Append("<div class='lifecycle-section'>");
            all.Append("<div class='lifecycle-title'>Batch Cycle</div>");
            all.Append("<div class='stage-line'>");
            all.Append(batch.ToString());
            all.Append("</div>");
            all.Append("</div>");


