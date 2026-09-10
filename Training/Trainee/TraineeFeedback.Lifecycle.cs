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

            if (IsPostBack || Session["EmpID"] == null || Session["TrainingID"] == null)
            {
                return;
            }

            if (IsFeedbackSubmitted())
            {
                return;
            }

            if (!CanSubmitFeedbackForAllRequiredSessions())
            {
                return;
            }

            lblMessage.Text = "";
            btnSubmit.Enabled = true;
            phFeedback.Visible = true;
            BuildFeedback();
        }

        private bool CanSubmitFeedbackForAllRequiredSessions()
        {
            string trainingID = Session["TrainingID"].ToString();
            string empID = Session["EmpID"].ToString().ToUpperInvariant();

            string sql = @"
SELECT
    TD.AttendanceRequired,
    TD.FeedbackRequired,
    ISNULL(TD.FeedbackSkipped,0) AS FeedbackSkipped,
    ISNULL(TD.InitialAssessmentRequired,0) AS InitialAssessmentRequired,
    ISNULL(TD.FinalAssessmentRequired,0) AS FinalAssessmentRequired
FROM TrainingDetails TD
WHERE TD.TrainingID=@TrainingID";

            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[]
            {
                new SqlParameter("@TrainingID", trainingID)
            });

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            DataRow row = dt.Rows[0];
            bool attendanceRequired = Convert.ToBoolean(row["AttendanceRequired"]);
            bool feedbackRequired = Convert.ToBoolean(row["FeedbackRequired"]);
            bool feedbackSkipped = Convert.ToBoolean(row["FeedbackSkipped"]);
            bool preRequired = Convert.ToBoolean(row["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(row["FinalAssessmentRequired"]);

            if (!feedbackRequired || feedbackSkipped)
            {
                return false;
            }

            if (attendanceRequired && !AreAllRequiredSessionAttendanceCompleted(trainingID, empID))
            {
                return false;
            }

            if (preRequired && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Pre", "PreAssessmentSkipped"))
            {
                return false;
            }

            if (postRequired && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Post", "PostAssessmentSkipped"))
            {
                return false;
            }

            return true;
        }

        private bool AreAllRequiredSessionAttendanceCompleted(string trainingID, string empID)
        {
            string sql = @"
SELECT CASE WHEN NOT EXISTS
(
    SELECT 1
    FROM SessionMaster SM
    WHERE SM.TrainingID=@TrainingID
      AND ISNULL(SM.AttendanceSkipped,0)=0
      AND NOT EXISTS
      (
          SELECT 1
          FROM SessionAttendance SA
          WHERE SA.SessionID=SM.SessionID
            AND SA.EmpID=@EmpID
            AND SA.AttendanceStatus='Completed'
      )
) THEN 1 ELSE 0 END";

            return Convert.ToInt32(objDB.ExecuteScalar(sql, new SqlParameter[]
            {
                new SqlParameter("@TrainingID", trainingID),
                new SqlParameter("@EmpID", empID)
            })) == 1;
        }

        private bool AreAllRequiredSessionTestsCompleted(string trainingID, string empID, string testType, string skipColumn)
        {
            string sql = @"
SELECT CASE WHEN NOT EXISTS
(
    SELECT 1
    FROM SessionMaster SM
    WHERE SM.TrainingID=@TrainingID
      AND ISNULL(SM." + skipColumn + @",0)=0
      AND
      (
          NOT EXISTS
          (
              SELECT 1
              FROM TestMaster TM
              WHERE TM.SessionID=SM.SessionID
                AND TM.TestType=@TestType
                AND TM.IsPublished=1
          )
          OR EXISTS
          (
              SELECT 1
              FROM TestMaster TM
              WHERE TM.SessionID=SM.SessionID
                AND TM.TestType=@TestType
                AND TM.IsPublished=1
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM TestAttempt TA
                    WHERE TA.TestID=TM.TestID
                      AND TA.EmpID=@EmpID
                      AND TA.Submitted=1
                )
          )
      )
) THEN 1 ELSE 0 END";

            return Convert.ToInt32(objDB.ExecuteScalar(sql, new SqlParameter[]
            {
                new SqlParameter("@TrainingID", trainingID),
                new SqlParameter("@EmpID", empID),
                new SqlParameter("@TestType", testType)
            })) == 1;
        }
    }
}
