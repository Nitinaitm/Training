using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class PostTrainingExam
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["EmpID"] == null || Session["TrainingID"] == null || Session["SessionID"] == null)
            {
                return;
            }

            string empID = Session["EmpID"].ToString().ToUpperInvariant();
            string trainingID = Session["TrainingID"].ToString();
            string sessionID = Session["SessionID"].ToString();

            DataTable dt = objDB.GetDataTable(
                "SELECT TD.FinalAssessmentRequired, " +
                "ISNULL(SM.PostAssessmentSkipped,0) AS PostAssessmentSkipped, " +
                "ISNULL(SM.AttendanceSkipped,0) AS AttendanceSkipped, " +
                "ISNULL(SM.PreAssessmentSkipped,0) AS PreAssessmentSkipped, " +
                "ISNULL(TD.AttendanceRequired,0) AS AttendanceRequired, " +
                "ISNULL(TD.InitialAssessmentRequired,0) AS InitialAssessmentRequired " +
                "FROM TrainingDetails TD " +
                "INNER JOIN SessionMaster SM ON SM.TrainingID=TD.TrainingID AND SM.SessionID=@SessionID " +
                "WHERE TD.TrainingID=@TrainingID",
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingID", trainingID),
                    new SqlParameter("@SessionID", sessionID)
                });

            if (dt.Rows.Count == 0)
            {
                DenyAccess("This session is not available for the selected training.");
                return;
            }

            DataRow row = dt.Rows[0];
            bool postRequired = Convert.ToBoolean(row["FinalAssessmentRequired"]);
            bool postSkipped = Convert.ToBoolean(row["PostAssessmentSkipped"]);
            bool attendanceRequired = Convert.ToBoolean(row["AttendanceRequired"]);
            bool attendanceSkipped = Convert.ToBoolean(row["AttendanceSkipped"]);
            bool preRequired = Convert.ToBoolean(row["InitialAssessmentRequired"]);
            bool preSkipped = Convert.ToBoolean(row["PreAssessmentSkipped"]);

            if (!postRequired)
            {
                DenyAccess("Post-Training Test is not required for this training.");
                return;
            }

            if (postSkipped)
            {
                DenyAccess("Post-Training Test has been skipped for this session.");
                return;
            }

            if (!IsPublished(sessionID))
            {
                DenyAccess("Post-Training Test has not been published for this session yet.");
                return;
            }

            if (attendanceRequired && !attendanceSkipped && !IsAttendanceCompleted(sessionID, empID))
            {
                DenyAccess("Please complete your attendance before starting the Post-Training Test.");
                return;
            }

            if (preRequired && !preSkipped && !IsSubmitted(sessionID, empID, "Pre"))
            {
                DenyAccess("Please complete the Pre-Training Test before starting the Post-Training Test.");
                return;
            }
        }

        private bool IsPublished(string sessionID)
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM TestMaster WHERE SessionID=@SessionID AND TestType='Post' AND IsPublished=1) THEN 1 ELSE 0 END",
                new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private bool IsAttendanceCompleted(string sessionID, string empID)
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM SessionAttendance WHERE SessionID=@SessionID AND EmpID=@EmpID AND AttendanceStatus IN ('Present','Completed')) THEN 1 ELSE 0 END",
                new SqlParameter[]
                {
                    new SqlParameter("@SessionID", sessionID),
                    new SqlParameter("@EmpID", empID)
                });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private bool IsSubmitted(string sessionID, string empID, string type)
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM TestMaster TM " +
                "INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID " +
                "WHERE TM.SessionID=@SessionID AND TM.TestType=@Type AND TM.IsPublished=1 " +
                "AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 1 ELSE 0 END",
                new SqlParameter[]
                {
                    new SqlParameter("@SessionID", sessionID),
                    new SqlParameter("@Type", type),
                    new SqlParameter("@EmpID", empID)
                });

            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private void DenyAccess(string message)
        {
            Session["PostTrainingAccessMessage"] = message;
            Response.Redirect("~/Trainee/MySessions.aspx");
        }
    }
}
