using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class PreTrainingExam
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["EmpID"] == null ||
                Session["TrainingID"] == null ||
                Session["SessionID"] == null)
            {
                return;
            }

            string empID = Session["EmpID"].ToString().ToUpperInvariant();
            string trainingID = Session["TrainingID"].ToString();
            string sessionID = Session["SessionID"].ToString();

            DataTable dt = objDB.GetDataTable(
                "SELECT TD.InitialAssessmentRequired, " +
                "ISNULL(SM.PreAssessmentSkipped,0) AS PreAssessmentSkipped, " +
                "TD.AttendanceRequired, " +
                "ISNULL(SM.AttendanceSkipped,0) AS AttendanceSkipped, " +
                "ISNULL(SA.AttendanceStatus,'Pending') AS AttendanceStatus " +
                "FROM SessionMaster SM " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID " +
                "LEFT JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID " +
                "WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID",
                new SqlParameter[]
                {
                    new SqlParameter("@EmpID", empID),
                    new SqlParameter("@SessionID", sessionID),
                    new SqlParameter("@TrainingID", trainingID)
                });

            if (dt.Rows.Count == 0)
            {
                DenyAccess("Session is not available for this training.");
                return;
            }

            DataRow row = dt.Rows[0];

            if (!Convert.ToBoolean(row["InitialAssessmentRequired"]))
            {
                DenyAccess("Pre-Training Test is not required for this training.");
                return;
            }

            if (Convert.ToBoolean(row["PreAssessmentSkipped"]))
            {
                DenyAccess("Pre-Training Test has been skipped for this session.");
                return;
            }

            if (!IsPublishedTestAvailable(sessionID))
            {
                DenyAccess("Pre-Training Test has not been published by the trainer yet.");
                return;
            }

            bool attendanceRequired = Convert.ToBoolean(row["AttendanceRequired"]);
            bool attendanceSkipped = Convert.ToBoolean(row["AttendanceSkipped"]);
            string attendanceStatus = row["AttendanceStatus"].ToString();

            if (attendanceRequired && !attendanceSkipped &&
                attendanceStatus != "Present" && attendanceStatus != "Completed")
            {
                DenyAccess("Your attendance has not been marked for this session. Please contact the trainer to mark your attendance before starting the Pre-Training Test.");
                return;
            }
        }

        private bool IsPublishedTestAvailable(string sessionID)
        {
            object result = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM TestMaster WHERE SessionID=@SessionID AND TestType='Pre' AND IsPublished=1) THEN 1 ELSE 0 END",
                new SqlParameter[]
                {
                    new SqlParameter("@SessionID", sessionID)
                });

            return result != null && result != DBNull.Value && Convert.ToInt32(result) == 1;
        }

        private void DenyAccess(string message)
        {
            Session["PreTrainingAccessMessage"] = message;
            Response.Redirect("~/Trainee/MySessions.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
