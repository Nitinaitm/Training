using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Training
{
    public partial class BatchLifecycle : UserControl
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            try
            {
                string trainingID = Convert.ToString(Session["TrainingID"]).Trim();
                string sessionID = Convert.ToString(Session["SessionID"]).Trim();
                string empID = Convert.ToString(Session["EmpID"]).Trim();
                string trainerID = Convert.ToString(Session["TrainerID"]).Trim();
                string role = Convert.ToString(Session["Role"]).Trim();

                if (string.IsNullOrWhiteSpace(trainingID) && !string.IsNullOrWhiteSpace(sessionID))
                {
                    trainingID = Convert.ToString(new clsDataAccess().ExecuteScalar("SELECT TrainingID FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) }));
                }

                if (string.IsNullOrWhiteSpace(trainingID))
                {
                    pnlLifecycle.Visible = false;
                    return;
                }

                DataTable td = new clsDataAccess().GetDataTable("SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,CertificateRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,ISNULL(CertificateSkipped,0) CertificateSkipped FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
                if (td.Rows.Count == 0)
                {
                    pnlLifecycle.Visible = false;
                    return;
                }

                DataRow r = td.Rows[0];
                bool attendanceRequired = Convert.ToBoolean(r["AttendanceRequired"]);
                bool preRequired = Convert.ToBoolean(r["InitialAssessmentRequired"]);
                bool postRequired = Convert.ToBoolean(r["FinalAssessmentRequired"]);
                bool feedbackRequired = Convert.ToBoolean(r["FeedbackRequired"]);
                bool certificateRequired = Convert.ToBoolean(r["CertificateRequired"]);
                bool feedbackSkipped = Convert.ToBoolean(r["FeedbackSkipped"]);
                bool certificateSkipped = Convert.ToBoolean(r["CertificateSkipped"]);
                bool trainee = string.Equals(role, "Trainee", StringComparison.OrdinalIgnoreCase) || (!string.IsNullOrWhiteSpace(empID) && string.IsNullOrWhiteSpace(trainerID));
                bool trainer = !trainee && !string.IsNullOrWhiteSpace(trainerID);

                StringBuilder html = new StringBuilder();
                BuildBatchCycle(html, trainingID, empID, trainee, trainer, feedbackRequired, certificateRequired, feedbackSkipped, certificateSkipped);
                BuildSessionCycles(html, trainingID, empID, trainerID, trainee, trainer, attendanceRequired, preRequired, postRequired);

                litLifecycle.Text = html.ToString();
                pnlLifecycle.Visible = true;
            }
            catch
            {
                pnlLifecycle.Visible = false;
            }
        }

        private void BuildBatchCycle(StringBuilder html, string trainingID, string empID, bool trainee, bool trainer, bool feedbackRequired, bool certificateRequired, bool feedbackSkipped, bool certificateSkipped)
        {
            int number = 1;
            int traineeCount = GetCount("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')='Assigned'", trainingID);
            bool trainingStarted = GetBool("SELECT CASE WHEN ISNULL(WorkflowStatus,'') LIKE '%E%' OR ISNULL(TrainingStatus,'') IN ('InProgress','AttendanceCompleted','TrainingCompleted') THEN 1 ELSE 0 END", trainingID);
            StringBuilder stages = new StringBuilder();

            if (!trainee && !trainer)
            {
                int sessionCount = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID", trainingID);
                int trainerCount = GetCount("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(TrainerID,'')<>''", trainingID);
                bool feedbackAssigned = GetCount("SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", trainingID) > 0;
                bool certificateConfigured = GetCount("SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>''", trainingID) > 0;
                int feedbackDone = feedbackRequired && !feedbackSkipped ? GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=A.TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)", trainingID) : 0;
                int certificateDone = certificateRequired && !certificateSkipped ? GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')", trainingID) : 0;

                stages.Append(Stage("Create Batch", true, true, false, 0, 0, ref number));
                stages.Append(Stage("Assign Sessions & Trainers", true, sessionCount > 0 && sessionCount == trainerCount, false, 0, 0, ref number));
                stages.Append(Stage("Assign Trainees", true, traineeCount > 0, false, 0, 0, ref number));
                stages.Append(Stage("Feedback Assigned", feedbackRequired, feedbackAssigned, feedbackSkipped, 0, 0, ref number));
                stages.Append(Stage("Certificate Template", certificateRequired, certificateConfigured, certificateSkipped, 0, 0, ref number));
                stages.Append(Stage("Training Started", true, trainingStarted, false, 0, 0, ref number));
                stages.Append(Stage("Feedback Submitted", feedbackRequired, feedbackDone >= traineeCount && traineeCount > 0, feedbackSkipped, feedbackDone, traineeCount, ref number));
                stages.Append(Stage("Certificate Generated", certificateRequired, certificateDone >= traineeCount && traineeCount > 0, certificateSkipped, certificateDone, traineeCount, ref number));
            }
            else
            {
                int feedbackDone;
                int certificateDone;
                int total = trainee ? 1 : traineeCount;

                if (trainee)
                {
                    feedbackDone = feedbackRequired && !feedbackSkipped && GetCount("SELECT COUNT(*) FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND Submitted=1", trainingID, new SqlParameter("@EmpID", empID)) > 0 ? 1 : 0;
                    certificateDone = certificateRequired && !certificateSkipped && GetCount("SELECT COUNT(*) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A'", trainingID, new SqlParameter("@EmpID", empID)) > 0 ? 1 : 0;
                }
                else
                {
                    feedbackDone = feedbackRequired && !feedbackSkipped ? GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=A.TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)", trainingID) : 0;
                    certificateDone = certificateRequired && !certificateSkipped ? GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')", trainingID) : 0;
                }

                stages.Append(Stage("Training Started", true, trainingStarted, false, 0, 0, ref number));
                stages.Append(Stage("Feedback Submitted", feedbackRequired, feedbackDone >= total && total > 0, feedbackSkipped, feedbackDone, total, ref number));
                stages.Append(Stage("Certificate Generated", certificateRequired, certificateDone >= total && total > 0, certificateSkipped, certificateDone, total, ref number));
            }

            html.Append("<div class='lifecycle-section'>");
            html.Append("<div class='lifecycle-title'>Batch Cycle</div>");
            html.Append("<div class='stage-line'>");
            html.Append(stages.ToString());
            html.Append("</div>");
            html.Append("</div>");
        }

        private void BuildSessionCycles(StringBuilder html, string trainingID, string empID, string trainerID, bool trainee, bool trainer, bool attendanceRequired, bool preRequired, bool postRequired)
        {
            string sql = "SELECT SessionID,ISNULL(CONVERT(varchar(50),SessionNo),'') AS SessionNo,ISNULL(SessionName,'') AS SessionName FROM SessionMaster WHERE TrainingID=@TrainingID";
            if (trainer)
                sql += " AND TrainerID=@TrainerID";
            sql += " ORDER BY ID";

            DataTable sessions = new clsDataAccess().GetDataTable(sql, trainer ? new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@TrainerID", trainerID) } : new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });

            int index = 1;
            foreach (DataRow row in sessions.Rows)
            {
                string sessionID = Convert.ToString(row["SessionID"]).Trim();
                bool attendanceSkipped = GetBool("SELECT ISNULL(AttendanceSkipped,0) FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));
                bool preSkipped = GetBool("SELECT ISNULL(PreAssessmentSkipped,0) FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));
                bool postSkipped = GetBool("SELECT ISNULL(PostAssessmentSkipped,0) FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));

                bool attendanceDone = GetBool("SELECT CASE WHEN ISNULL(AttendanceStatus,'')='Completed' THEN 1 ELSE 0 END FROM SessionMaster WHERE TrainingID=@TrainingID AND SessionID=@SessionID", trainingID, new SqlParameter("@SessionID", sessionID));
                if (trainee)
                    attendanceDone = GetCount("SELECT COUNT(*) FROM SessionAttendance WHERE SessionID=@SessionID AND EmpID=@EmpID AND AttendanceStatus IN ('Present','Completed')", trainingID, new SqlParameter("@SessionID", sessionID), new SqlParameter("@EmpID", empID)) > 0;

                bool publishedPre = GetCount("SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType='Pre' AND IsPublished=1", trainingID, new SqlParameter("@SessionID", sessionID)) > 0;
                bool publishedPost = GetCount("SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType='Post' AND IsPublished=1", trainingID, new SqlParameter("@SessionID", sessionID)) > 0;
                int traineeTotal = GetCount("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')='Assigned'", trainingID);

                int preTotal = 0;
                int preCompleted = 0;
                if (publishedPre)
                {
                    if (trainee)
                    {
                        preTotal = 1;
                        preCompleted = GetCount("SELECT COUNT(*) FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1", trainingID, new SqlParameter("@SessionID", sessionID), new SqlParameter("@EmpID", empID)) > 0 ? 1 : 0;
                    }
                    else
                    {
                        preTotal = traineeTotal;
                        preCompleted = GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)", trainingID, new SqlParameter("@SessionID", sessionID));
                    }
                }

                int postTotal = 0;
                int postCompleted = 0;
                if (publishedPost)
                {
                    if (trainee)
                    {
                        postTotal = 1;
                        postCompleted = GetCount("SELECT COUNT(*) FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1", trainingID, new SqlParameter("@SessionID", sessionID), new SqlParameter("@EmpID", empID)) > 0 ? 1 : 0;
                    }
                    else
                    {
                        postTotal = traineeTotal;
                        postCompleted = GetCount("SELECT COUNT(*) FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)", trainingID, new SqlParameter("@SessionID", sessionID));
                    }
                }

                if (trainer)
                {
                    preTotal = publishedPre ? 1 : 0;
                    preCompleted = publishedPre ? 1 : 0;
                    postTotal = publishedPost ? 1 : 0;
                    postCompleted = publishedPost ? 1 : 0;
                }

                StringBuilder stages = new StringBuilder();
                int number = 1;
                stages.Append(Stage("Attendance", attendanceRequired, attendanceSkipped || attendanceDone, attendanceSkipped, 0, 0, ref number));
                stages.Append(TestStage("Pre-Test", preRequired, preSkipped, publishedPre, preCompleted, preTotal, ref number));
                stages.Append(TestStage("Post-Test", postRequired, postSkipped, publishedPost, postCompleted, postTotal, ref number));

                html.Append("<div class='lifecycle-section session-cycle'>");
                html.Append("<div class='lifecycle-title'>" + HttpUtility.HtmlEncode(GetSessionDisplayName(row, index)) + "</div>");
                html.Append("<div class='stage-line'>" + stages.ToString() + "</div>");
                html.Append("</div>");
                index++;
            }
        }

        private string TestStage(string label, bool required, bool skipped, bool published, int completed, int total, ref int number)
        {
            if (!required)
                return Stage(label, false, false, false, 0, 0, ref number);
            if (skipped)
                return Stage(label, true, false, true, 0, 0, ref number);
            if (!published)
                return Stage(label, true, false, false, 0, 0, ref number);
            return Stage(label, true, completed >= total && total > 0, false, completed, total, ref number);
        }

        private string Stage(string label, bool required, bool complete, bool skipped, int completed, int total, ref int number)
        {
            string css;
            string state;
            string bubble;
            string title = "";
            string style = "";

            if (skipped)
            {
                css = "bl-skipped";
                state = "Skipped";
                bubble = "–";
            }
            else if (!required)
            {
                css = "bl-na";
                state = "Not Required";
                bubble = "–";
            }
            else if (total > 0)
            {
                if (completed < 0) completed = 0;
                if (completed > total) completed = total;
                int percent = (int)Math.Round(completed * 100.0 / total);
                title = " title='" + completed + "/" + total + " (" + percent + "%)'";
                if (completed >= total)
                {
                    css = "bl-done";
                    state = "✓ Completed";
                    bubble = "✓";
                }
                else if (completed > 0)
                {
                    css = "bl-partial";
                    state = "In Progress";
                    bubble = number.ToString();
                    style = " style='background:conic-gradient(#198754 0% " + percent + "%, #dc3545 " + percent + "% 100%);'";
                }
                else
                {
                    css = "bl-pending";
                    state = "Pending";
                    bubble = number.ToString();
                }
                number++;
            }
            else if (complete)
            {
                css = "bl-done";
                state = "✓ Completed";
                bubble = "✓";
            }
            else
            {
                css = "bl-pending";
                state = "Pending";
                bubble = number.ToString();
                number++;
            }

            return "<div class='bl-item " + css + "'><div class='bl-bubble'" + title + style + ">" + bubble + "</div><div class='bl-label'>" + HttpUtility.HtmlEncode(label) + "</div><div class='bl-state'>" + HttpUtility.HtmlEncode(state) + "</div></div>";
        }

        private bool GetBool(string sql, string trainingID, params SqlParameter[] parameters)
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

        private int GetCount(string sql, string trainingID, params SqlParameter[] parameters)
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

        private string GetSessionDisplayName(DataRow row, int index)
        {
            string sessionNo = row["SessionNo"] == DBNull.Value ? "" : Convert.ToString(row["SessionNo"]).Trim();
            string sessionName = row["SessionName"] == DBNull.Value ? "" : Convert.ToString(row["SessionName"]).Trim();
            if (string.IsNullOrWhiteSpace(sessionName))
                sessionName = "Session " + index.ToString();
            if (!string.IsNullOrWhiteSpace(sessionNo))
                return "Session " + sessionNo + " - " + sessionName;
            return sessionName;
        }
    }
}