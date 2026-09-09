using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MyTrainings
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string empID = Convert.ToString(Session["EmpID"]).ToUpperInvariant();
            for (int i = 0; i < gvTraining.Rows.Count; i++)
            {
                GridViewRow row = gvTraining.Rows[i];
                if (row.RowType != DataControlRowType.DataRow) continue;
                string trainingID = Convert.ToString(gvTraining.DataKeys[i].Value);
                if (string.IsNullOrWhiteSpace(trainingID)) continue;

                bool attendanceRequired = GetBool("SELECT AttendanceRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                bool preRequired = GetBool("SELECT InitialAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                bool postRequired = GetBool("SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                bool feedbackRequired = GetBool("SELECT FeedbackRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                bool certificateRequired = GetBool("SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                bool feedbackSkipped = GetBool("SELECT ISNULL(FeedbackSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);
                bool certificateSkipped = GetBool("SELECT ISNULL(CertificateSkipped,0) FROM TrainingDetails WHERE TrainingID=@TrainingID", trainingID);

                bool attendanceDone = !attendanceRequired || GetCount(@"SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.AttendanceSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=S.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus='Completed')", trainingID, empID) == 0;
                bool preDone = !preRequired || GetCount(@"SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.PreAssessmentSkipped,0)=0 AND EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1) AND NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=S.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1)", trainingID, empID) == 0;
                bool postDone = !postRequired || GetCount(@"SELECT COUNT(*) FROM SessionMaster S WHERE S.TrainingID=@TrainingID AND ISNULL(S.PostAssessmentSkipped,0)=0 AND EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType='Post' AND TM.IsPublished=1) AND NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=S.SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1)", trainingID, empID) == 0;
                bool feedbackDone = !feedbackRequired || feedbackSkipped || GetCount("SELECT COUNT(*) FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND Submitted=1", trainingID, empID) > 0;

                // Attendance is available while pending, not after it is completed.
                SetButton(row, "lnkAttendance", attendanceRequired && !attendanceDone);
                SetButton(row, "lnkFeedback", feedbackRequired && !feedbackSkipped && attendanceDone && preDone && postDone);
                SetButton(row, "lnkCertificate", certificateRequired && !certificateSkipped && attendanceDone && preDone && postDone && feedbackDone);
            }
        }

        private void SetButton(GridViewRow row, string id, bool enabled)
        {
            LinkButton b = row.FindControl(id) as LinkButton;
            if (b == null) return;
            b.Enabled = enabled;
            if (!enabled && !b.CssClass.Contains("disabled")) b.CssClass += " disabled";
        }

        private bool GetBool(string sql, string trainingID)
        {
            object value = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private int GetCount(string sql, string trainingID, string empID)
        {
            object value = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
            return value == null || value == DBNull.Value ? 0 : Convert.ToInt32(value);
        }
    }
}
