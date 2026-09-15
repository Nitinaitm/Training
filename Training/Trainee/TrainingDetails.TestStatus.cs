using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class TrainingDetails
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            RefreshSessionTestStatuses();
        }

        private void RefreshSessionTestStatuses()
        {
            if (string.IsNullOrWhiteSpace(TrainingID) || string.IsNullOrWhiteSpace(EmpID))
            {
                return;
            }

            DataTable req = objDB.GetDataTable(
                "SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });

            if (req.Rows.Count == 0) return;

            bool attendanceRequired = Convert.ToBoolean(req.Rows[0]["AttendanceRequired"]);
            bool preRequired = Convert.ToBoolean(req.Rows[0]["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(req.Rows[0]["FinalAssessmentRequired"]);

            foreach (GridViewRow row in gvSession.Rows)
            {
                string sessionID = gvSession.DataKeys[row.RowIndex].Value.ToString();
                Label lblPre = row.FindControl("lblPre") as Label;
                Label lblPost = row.FindControl("lblPost") as Label;

                DataTable dt = objDB.GetDataTable(
                    "SELECT ISNULL(SM.AttendanceSkipped,0) AttendanceSkipped, " +
                    "ISNULL(SM.PreAssessmentSkipped,0) PreSkipped, " +
                    "ISNULL(SM.PostAssessmentSkipped,0) PostSkipped, " +
                    "ISNULL(SA.AttendanceStatus,'Pending') AttendanceStatus " +
                    "FROM SessionMaster SM " +
                    "LEFT JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID " +
                    "WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID",
                    new SqlParameter[]
                    {
                        new SqlParameter("@SessionID", sessionID),
                        new SqlParameter("@TrainingID", TrainingID),
                        new SqlParameter("@EmpID", EmpID)
                    });

                if (dt.Rows.Count == 0) continue;

                DataRow sr = dt.Rows[0];
                bool attendanceSkipped = Convert.ToBoolean(sr["AttendanceSkipped"]);
                bool preSkipped = Convert.ToBoolean(sr["PreSkipped"]);
                bool postSkipped = Convert.ToBoolean(sr["PostSkipped"]);
                string attendance = sr["AttendanceStatus"].ToString();
                bool attendanceDone = !attendanceRequired || attendanceSkipped || attendance == "Present" || attendance == "Completed";

                SetGridTestStatus(lblPre, sessionID, "Pre", preRequired, preSkipped, attendanceDone, true);

                bool preDone = !preRequired || preSkipped || IsSubmitted(sessionID, "Pre");
                SetGridTestStatus(lblPost, sessionID, "Post", postRequired, postSkipped, attendanceDone && preDone, false);
            }
        }

        private void SetGridTestStatus(Label label, string sessionID, string type, bool required, bool skipped, bool gateOpen, bool pre)
        {
            if (label == null) return;

            if (!required)
            {
                label.Text = "Not Required";
                label.CssClass = "badge badge-secondary";
                return;
            }

            if (skipped)
            {
                label.Text = "Skipped";
                label.CssClass = "badge badge-secondary";
                return;
            }

            if (!IsPublished(sessionID, type))
            {
                label.Text = "Not Published";
                label.CssClass = "badge badge-secondary";
                return;
            }

            if (!gateOpen)
            {
                label.Text = pre ? "Locked - Attendance Pending" : "Locked";
                label.CssClass = "badge badge-warning";
                return;
            }

            if (IsSubmitted(sessionID, type))
            {
                label.Text = "Completed";
                label.CssClass = "badge badge-success";
                return;
            }

            label.Text = "Available";
            label.CssClass = "badge badge-primary";
        }

        private bool IsPublished(string sessionID, string type)
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM TestMaster WHERE SessionID=@SessionID AND TestType=@Type AND IsPublished=1) THEN 1 ELSE 0 END",
                new SqlParameter[]
                {
                    new SqlParameter("@SessionID", sessionID),
                    new SqlParameter("@Type", type)
                });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private bool IsSubmitted(string sessionID, string type)
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType=@Type AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 1 ELSE 0 END",
                new SqlParameter[]
                {
                    new SqlParameter("@SessionID", sessionID),
                    new SqlParameter("@Type", type),
                    new SqlParameter("@EmpID", EmpID)
                });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }
    }
}
