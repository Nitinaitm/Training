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
            if (!GetBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", Convert.ToString(Session["TrainingID"])))
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
            bool started = GetBool("SELECT CASE WHEN ISNULL(WorkflowStatus,'') LIKE '%E%' THEN 1 ELSE 0 END", trainingID);
            bool completed = IsTrainingCompleted();
            if (completed)
            {
                btnUpdateTraining.Visible = false; btnAssignSession.Visible = false; btnAssignTrainee.Visible = false; btnRequirements.Visible = false;
                btnAssignFeedback.Visible = false; btnAssignHostel.Visible = false; btnCertificateTemplate.Visible = false; btnStartTraining.Visible = false; btnAttendance.Visible = true;
            }
            else
            {
                btnRequirements.Visible = started;
                if (!started)
                {
                    bool certificateRequired = GetBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                    bool certificateSkipped = GetBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
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

        private bool GetBool(string sql, string trainingID)
        {
            if (string.IsNullOrWhiteSpace(trainingID)) return false;
            object value = new clsDataAccess().ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }
        private int GetCount(string sql, string trainingID)
        {
            if (string.IsNullOrWhiteSpace(trainingID)) return 0;
            object value = new clsDataAccess().ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }
        private bool IsTrainingCompleted()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID)) return false;
            return GetBool("SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ' THEN 1 ELSE 0 END FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
        }
        private string StageHtml(string label, bool required, bool complete, bool skipped, ref int number)
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
            if (string.IsNullOrWhiteSpace(trainingID)) { litBatchLifecycle.Text = "<div class='stage-line'>" + StageHtml("Create Batch", true, false, false, ref n) + "</div>"; return; }
            bool attendanceReq = GetBool("SELECT AttendanceRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool preReq = GetBool("SELECT InitialAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool postReq = GetBool("SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool feedbackReq = GetBool("SELECT FeedbackRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool certReq = GetBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool feedbackSkip = GetBool("SELECT ISNULL(FeedbackSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool certSkip = GetBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            int sessionCount = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID", trainingID);
            int trainerCount = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>''", trainingID);
            bool allSessionsTrainers = sessionCount > 0 && trainerCount == sessionCount;
            bool trainees = GetCount("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND AssignmentStatus='Assigned'", trainingID) > 0;
            bool feedbackAssigned = GetCount("SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", trainingID) > 0;
            bool certConfigured = GetCount("SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>'' AND ISNULL(CourseTitle,'')<>''", trainingID) > 0;
            int attendanceApplicable = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(AttendanceSkipped,0)=0", trainingID);
            bool attendanceSkipped = sessionCount > 0 && attendanceApplicable == 0;
            bool attendanceDone = !attendanceReq || attendanceSkipped || (sessionCount > 0 && trainees && GetCount(@"SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.AttendanceSkipped,0)=0 AND EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned') AND EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=S.SessionID AND SA.EmpID=A.EmpID AND SA.AttendanceStatus='Completed'))", trainingID) == 0);
            int preApplicable = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(PreAssessmentSkipped,0)=0", trainingID);
            bool preSkipped = sessionCount > 0 && preApplicable == 0;
            bool prePublished = sessionCount > 0 && preApplicable > 0 && GetCount("SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.PreAssessmentSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1)", trainingID) == 0;
            int postApplicable = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(PostAssessmentSkipped,0)=0", trainingID);
            bool postSkipped = sessionCount > 0 && postApplicable == 0;
            bool postPublished = sessionCount > 0 && postApplicable > 0 && GetCount("SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.PostAssessmentSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType='Post' AND TM.IsPublished=1)", trainingID) == 0;
            bool feedbackDone = !feedbackReq || feedbackSkip || (trainees && GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=A.TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)", trainingID) == 0);
            bool certDone = !certReq || certSkip || (trainees && GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND A.AssignmentStatus='Assigned' AND NOT EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')", trainingID) == 0);
            StringBuilder h = new StringBuilder();
            h.Append(StageHtml("Create Batch", true, true, false, ref n));
            h.Append(StageHtml("Assign Sessions & Trainers", true, allSessionsTrainers, false, ref n));
            h.Append(StageHtml("Assign Trainees", true, trainees, false, ref n));
            h.Append(StageHtml("Feedback Assigned", feedbackReq, feedbackAssigned, feedbackSkip, ref n));
            h.Append(StageHtml("Certificate Template", certReq, certConfigured, certSkip, ref n));
            h.Append(StageHtml("Attendance Completed", attendanceReq, attendanceDone, attendanceSkipped, ref n));
            h.Append(StageHtml("Pre-Test Completed", preReq, prePublished, preSkipped, ref n));
            h.Append(StageHtml("Post-Test Completed", postReq, postPublished, postSkipped, ref n));
            h.Append(StageHtml("Feedback Submitted", feedbackReq, feedbackDone, feedbackSkip, ref n));
            h.Append(StageHtml("Certificate Generated", certReq, certDone, certSkip, ref n));
            litBatchLifecycle.Text = "<div class='stage-line'>" + h.ToString() + "</div>";
        }
    }
}
