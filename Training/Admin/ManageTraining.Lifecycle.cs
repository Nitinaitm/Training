using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Training.Admin
{
    public partial class ManageTraining
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
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
            if (IsPostBack && IsTrainingCompleted())
                Response.Redirect("TrainingList.aspx", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            bool completed = IsTrainingCompleted();
            bool started = GetLifecycleBool("SELECT CASE WHEN ISNULL(WorkflowStatus,'') LIKE '%E%' OR ISNULL(TrainingStatus,'') IN ('InProgress','AttendanceCompleted','TrainingCompleted') THEN 1 ELSE 0 END", trainingID);

            if (completed)
            {
                btnUpdateTraining.Visible = false;
                btnAssignSession.Visible = false;
                btnAssignTrainee.Visible = false;
                btnRequirements.Visible = false;
                btnAssignFeedback.Visible = false;
                btnAssignHostel.Visible = false;
                btnCertificateTemplate.Visible = false;
                btnStartTraining.Visible = false;
                btnAttendance.Visible = true;
                btnCloseTraining.Visible = false;
            }
            else
            {
                btnRequirements.Visible = true;
                btnRequirements.Enabled = true;
                btnCloseTraining.Visible = started;
                btnCloseTraining.Enabled = started;

                if (!started)
                {
                    bool certificateRequired = GetLifecycleBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                    bool certificateSkipped = GetLifecycleBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                    btnCertificateTemplate.Visible = certificateRequired && !certificateSkipped;
                    btnCertificateTemplate.Enabled = certificateRequired && !certificateSkipped;
                }
                else
                {
                    btnCertificateTemplate.Visible = false;
                    btnCertificateTemplate.Enabled = false;
                    btnStartTraining.Enabled = false;
                }
            }

            BuildLifecycle(trainingID);
            base.OnPreRender(e);
        }

        private bool GetLifecycleBool(string sql, string trainingID, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(trainingID))
                return false;

            SqlParameter[] allParameters = new SqlParameter[parameters.Length + 1];
            allParameters[0] = new SqlParameter("@TrainingID", trainingID);
            for (int i = 0; i < parameters.Length; i++)
                allParameters[i + 1] = parameters[i];

            object value = new clsDataAccess().ExecuteScalar(sql, allParameters);
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private int GetLifecycleCount(string sql, string trainingID, params SqlParameter[] parameters)
        {
            if (string.IsNullOrWhiteSpace(trainingID))
                return 0;

            SqlParameter[] allParameters = new SqlParameter[parameters.Length + 1];
            allParameters[0] = new SqlParameter("@TrainingID", trainingID);
            for (int i = 0; i < parameters.Length; i++)
                allParameters[i + 1] = parameters[i];

            object value = new clsDataAccess().ExecuteScalar(sql, allParameters);
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }

        private bool IsTrainingCompleted()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID))
                return false;

            return GetLifecycleBool("SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ' THEN 1 ELSE 0 END FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
        }

        private string LifecycleStage(string label, bool required, bool complete, bool skipped, int completed, int total, ref int number)
        {
            string css;
            string state;
            string bubble;
            string title = "";
            string style = "";

            if (skipped)
            {
                css = "done";
                state = "Skipped";
                bubble = "–";
            }
            else if (!required)
            {
                css = "na";
                state = "Not Required";
                bubble = "–";
            }
            else if (total > 0)
            {
                if (completed < 0)
                    completed = 0;
                if (completed > total)
                    completed = total;

                int percent = (int)Math.Round(completed * 100.0 / total);
                title = " title='" + completed + "/" + total + " (" + percent + "%)'";

                if (completed >= total)
                {
                    css = "done";
                    state = "✓ Completed";
                    bubble = "✓";
                }
                else if (completed > 0)
                {
                    css = "partial";
                    state = "In Progress";
                    bubble = number.ToString();
                    style = " style='background:conic-gradient(#198754 0% " + percent + "%, #dc3545 " + percent + "% 100%);'";
                }
                else
                {
                    css = "pending";
                    state = "Pending";
                    bubble = number.ToString();
                }

                number++;
            }
            else if (complete)
            {
                css = "done";
                state = "✓ Completed";
                bubble = "✓";
            }
            else
            {
                css = "pending";
                state = "Pending";
                bubble = number.ToString();
                number++;
            }

            return "<div class='stage-item " + css + "'><div class='stage-bubble'" + title + style + ">" + bubble + "</div><div class='stage-label'>" + HttpUtility.HtmlEncode(label) + "</div><div class='stage-state'>" + HttpUtility.HtmlEncode(state) + "</div></div>";
        }

        private void BuildLifecycle(string trainingID)
        {
            if (string.IsNullOrWhiteSpace(trainingID))
            {
                litBatchLifecycle.Text = "";
                return;
            }

            bool attendanceRequired = GetLifecycleBool("SELECT AttendanceRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool preRequired = GetLifecycleBool("SELECT InitialAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool postRequired = GetLifecycleBool("SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool feedbackRequired = GetLifecycleBool("SELECT FeedbackRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool certificateRequired = GetLifecycleBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool feedbackSkipped = GetLifecycleBool("SELECT ISNULL(FeedbackSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
            bool certificateSkipped = GetLifecycleBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);

            int traineeTotal = GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')='Assigned'", trainingID);
            int sessionTotal = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID", trainingID);
            int trainerAssigned = GetLifecycleCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>''", trainingID);

            bool feedbackAssigned = GetLifecycleCount("SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", trainingID) > 0;
            bool certificateConfigured = GetLifecycleCount("SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>''", trainingID) > 0;
            bool trainingStarted = GetLifecycleBool("SELECT CASE WHEN ISNULL(WorkflowStatus,'') LIKE '%E%' OR ISNULL(TrainingStatus,'') IN ('InProgress','AttendanceCompleted','TrainingCompleted') THEN 1 ELSE 0 END", trainingID);

            int feedbackDone = feedbackRequired && !feedbackSkipped ? GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=A.TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)", trainingID) : 0;
            int certificateDone = certificateRequired && !certificateSkipped ? GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')", trainingID) : 0;

            StringBuilder html = new StringBuilder();
            StringBuilder batch = new StringBuilder();
            int number = 1;

            batch.Append(LifecycleStage("Create Batch", true, true, false, 0, 0, ref number));
            batch.Append(LifecycleStage("Assign Sessions & Trainers", true, sessionTotal > 0 && sessionTotal == trainerAssigned, false, 0, 0, ref number));
            batch.Append(LifecycleStage("Assign Trainees", true, traineeTotal > 0, false, 0, 0, ref number));
            batch.Append(LifecycleStage("Feedback Assigned", feedbackRequired, feedbackAssigned, feedbackSkipped, 0, 0, ref number));
            batch.Append(LifecycleStage("Certificate Template", certificateRequired, certificateConfigured, certificateSkipped, 0, 0, ref number));
            batch.Append(LifecycleStage("Training Started", true, trainingStarted, false, 0, 0, ref number));
            batch.Append(LifecycleStage("Feedback Submitted", feedbackRequired, feedbackDone >= traineeTotal && traineeTotal > 0, feedbackSkipped, feedbackDone, traineeTotal, ref number));
            batch.Append(LifecycleStage("Certificate Generated", certificateRequired, certificateDone >= traineeTotal && traineeTotal > 0, certificateSkipped, certificateDone, traineeTotal, ref number));

            html.Append("<div class='lifecycle-section'>");
            html.Append("<div class='lifecycle-title'>Batch Cycle</div>");
            html.Append("<div class='stage-line'>");
            html.Append(batch.ToString());
            html.Append("</div>");
            html.Append("</div>");

            DataTable sessions = new DataTable();
            string sessionSql = "SELECT SessionID,ISNULL(CONVERT(varchar(50),SessionNo),'') AS SessionNo,ISNULL(SessionName,'') AS SessionName,ISNULL(TrainerID,'') AS TrainerID FROM SessionMaster WHERE TrainingID=@TrainingID ORDER BY ID";

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand(sessionSql, con))
                {
                    cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(sessions);
                    }
                }
            }

            int sessionNumber = 1;

            foreach (DataRow row in sessions.Rows)
            {
                string sessionID = Convert.ToString(row["SessionID"]).Trim();
                string sessionName = Convert.ToString(row["SessionName"]).Trim();
                string sessionNo = Convert.ToString(row["SessionNo"]).Trim();
                if (string.IsNullOrWhiteSpace(sessionName))
                    sessionName = "Session " + sessionNumber.ToString();

                string title = string.IsNullOrWhiteSpace(sessionNo) ? sessionName : "Session " + sessionNo + " - " + sessionName;

                bool attendanceSkipped = GetLifecycleBool("SELECT ISNULL(AttendanceSkipped,0) FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));
                bool preSkipped = GetLifecycleBool("SELECT ISNULL(PreAssessmentSkipped,0) FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));
                bool postSkipped = GetLifecycleBool("SELECT ISNULL(PostAssessmentSkipped,0) FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));

                bool attendanceDone = GetLifecycleBool("SELECT CASE WHEN ISNULL(AttendanceStatus,'')='Completed' THEN 1 ELSE 0 END FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));

                bool publishedPre = GetLifecycleCount("SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType='Pre' AND IsPublished=1", trainingID, new SqlParameter("@SessionID", sessionID)) > 0;
                bool publishedPost = GetLifecycleCount("SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType='Post' AND IsPublished=1", trainingID, new SqlParameter("@SessionID", sessionID)) > 0;

                int preTotal = publishedPre ? traineeTotal : 0;
                int postTotal = publishedPost ? traineeTotal : 0;
                int preDone = publishedPre ? GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)", trainingID, new SqlParameter("@SessionID", sessionID)) : 0;
                int postDone = publishedPost ? GetLifecycleCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)", trainingID, new SqlParameter("@SessionID", sessionID)) : 0;

                StringBuilder sessionHtml = new StringBuilder();
                int sn = 1;
                sessionHtml.Append(LifecycleStage("Attendance", attendanceRequired, attendanceDone, attendanceSkipped, 0, 0, ref sn));

                if (preRequired)
                    sessionHtml.Append(!preSkipped && publishedPre ? LifecycleStage("Pre-Test", true, preDone >= preTotal && preTotal > 0, false, preDone, preTotal, ref sn) : LifecycleStage("Pre-Test", true, false, preSkipped, 0, 0, ref sn));
                else
                    sessionHtml.Append(LifecycleStage("Pre-Test", false, false, false, 0, 0, ref sn));

                if (postRequired)
                    sessionHtml.Append(!postSkipped && publishedPost ? LifecycleStage("Post-Test", true, postDone >= postTotal && postTotal > 0, false, postDone, postTotal, ref sn) : LifecycleStage("Post-Test", true, false, postSkipped, 0, 0, ref sn));
                else
                    sessionHtml.Append(LifecycleStage("Post-Test", false, false, false, 0, 0, ref sn));

                html.Append("<div class='lifecycle-section session-cycle'>");
                html.Append("<div class='lifecycle-title'>" + HttpUtility.HtmlEncode(title) + "</div>");
                html.Append("<div class='stage-line'>");
                html.Append(sessionHtml.ToString());
                html.Append("</div>");
                html.Append("</div>");

                sessionNumber++;
            }

            litBatchLifecycle.Text = html.ToString();
        }
    }
}