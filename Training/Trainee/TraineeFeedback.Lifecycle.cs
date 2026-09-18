using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class TraineeFeedback
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (Session["EmpID"] == null || Session["TrainingID"] == null) return;
            if (IsFeedbackSubmitted()) return;

            string message;
            if (!CanSubmitFeedbackForAllRequiredSessions(out message))
            {
                lblMessage.Text = message;
                btnSubmit.Enabled = false;
                phFeedback.Visible = false;
                return;
            }

            lblMessage.Text = "";
            btnSubmit.Enabled = true;
            phFeedback.Visible = true;
            BuildFeedback();
        }

        private bool CanSubmitFeedbackForAllRequiredSessions(out string message)
        {
            message = "";
            string trainingID = Session["TrainingID"].ToString();
            string empID = Session["EmpID"].ToString().ToUpperInvariant();
            DataTable dt = objDB.GetDataTable(@"SELECT AttendanceRequired,FeedbackRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            if (dt.Rows.Count == 0) { message = "Training details are not available."; return false; }
            DataRow r = dt.Rows[0];
            if (!Convert.ToBoolean(r["FeedbackRequired"]) || Convert.ToBoolean(r["FeedbackSkipped"])) { message = "Feedback is not required for this training."; return false; }

            if (Convert.ToBoolean(r["AttendanceRequired"]) && !AreAllRequiredSessionAttendanceCompleted(trainingID, empID))
            {
                message = "Please complete your attendance for all required sessions before submitting feedback.";
                return false;
            }
            if (Convert.ToBoolean(r["InitialAssessmentRequired"]) && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Pre", "PreAssessmentSkipped"))
            {
                message = "Please complete all required Pre-Training Tests before submitting feedback. A required test must also be published by the trainer.";
                return false;
            }
            if (Convert.ToBoolean(r["FinalAssessmentRequired"]) && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Post", "PostAssessmentSkipped"))
            {
                message = "Please complete all required Post-Training Tests before submitting feedback. A required test must also be published by the trainer.";
                return false;
            }
            return true;
        }

        private bool CanSubmitFeedbackForAllRequiredSessions()
        {
            string ignored;
            return CanSubmitFeedbackForAllRequiredSessions(out ignored);
        }

        private bool AreAllRequiredSessionAttendanceCompleted(string trainingID, string empID)
        {
            object v = objDB.ExecuteScalar(@"SELECT CASE WHEN NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus IN ('Present','Completed'))) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
            return v != null && Convert.ToInt32(v) == 1;
        }

        private bool AreAllRequiredSessionTestsCompleted(string trainingID, string empID, string testType, string skipColumn)
        {
            if ((testType != "Pre" && testType != "Post") ||
                (skipColumn != "PreAssessmentSkipped" && skipColumn != "PostAssessmentSkipped")) return false;
            string sql = @"SELECT CASE WHEN NOT EXISTS (
                SELECT 1 FROM SessionMaster SM
                WHERE SM.TrainingID=@TrainingID AND ISNULL(SM." + skipColumn + @",0)=0
                AND (
                    NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1)
                    OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1)
                )
            ) THEN 1 ELSE 0 END";
            object v = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID), new SqlParameter("@TestType", testType) });
            return v != null && Convert.ToInt32(v) == 1;
        }
    }
}
