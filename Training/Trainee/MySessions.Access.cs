using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MySessions
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string message = Session["PreTrainingAccessMessage"] as string;
            if (!string.IsNullOrWhiteSpace(message))
            {
                Session.Remove("PreTrainingAccessMessage");
                string safe = message.Replace("\\", "\\\\").Replace("'", "\\'");
                ScriptManager.RegisterStartupScript(this, GetType(), "PreTrainingAccessMessage", "alert('" + safe + "');", true);
            }

            string postMessage = Session["PostTrainingAccessMessage"] as string;
            if (!string.IsNullOrWhiteSpace(postMessage))
            {
                Session.Remove("PostTrainingAccessMessage");
                string safe = postMessage.Replace("\\", "\\\\").Replace("'", "\\'");
                ScriptManager.RegisterStartupScript(this, GetType(), "PostTrainingAccessMessage", "alert('" + safe + "');", true);
            }

            RefreshTestStatuses();
        }

        private void RefreshTestStatuses()
        {
            if (Session["EmpID"] == null || Session["TrainingID"] == null || Session["SessionID"] == null)
            {
                return;
            }

            string empID = Session["EmpID"].ToString().ToUpperInvariant();
            string trainingID = Session["TrainingID"].ToString();

            DataTable req = objDB.GetDataTable(
                "SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired " +
                "FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });

            if (req.Rows.Count == 0 || gvSession.Rows.Count == 0)
            {
                return;
            }

            bool attendanceRequired = Convert.ToBoolean(req.Rows[0]["AttendanceRequired"]);
            bool preRequired = Convert.ToBoolean(req.Rows[0]["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(req.Rows[0]["FinalAssessmentRequired"]);

            foreach (GridViewRow row in gvSession.Rows)
            {
                string sessionID = gvSession.DataKeys[row.RowIndex].Value.ToString();
                Label lblPre = row.FindControl("lblPre") as Label;
                Label lblPost = row.FindControl("lblPost") as Label;

                DataTable status = objDB.GetDataTable(
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
                        new SqlParameter("@TrainingID", trainingID),
                        new SqlParameter("@EmpID", empID)
                    });

                if (status.Rows.Count == 0) continue;

                DataRow sr = status.Rows[0];
                bool attendanceSkipped = Convert.ToBoolean(sr["AttendanceSkipped"]);
                bool preSkipped = Convert.ToBoolean(sr["PreSkipped"]);
                bool postSkipped = Convert.ToBoolean(sr["PostSkipped"]);
                string attendance = sr["AttendanceStatus"].ToString();
                bool attendanceDone = !attendanceRequired || attendanceSkipped || attendance == "Present" || attendance == "Completed";

                if (lblPre != null)
                {
                    SetTestStatus(lblPre, sessionID, empID, "Pre", preRequired, preSkipped, attendanceDone, true);
                }

                if (lblPost != null)
                {
                    bool prePublished = preRequired && !preSkipped && IsPublished(sessionID, "Pre");
                    bool preDone = !prePublished || IsSubmitted(sessionID, empID, "Pre");
                    SetTestStatus(lblPost, sessionID, empID, "Post", postRequired, postSkipped, attendanceDone && preDone, false);
                }

                if (sessionID == Session["SessionID"].ToString())
                {
                    RefreshButtons(sessionID, empID, attendanceDone, preRequired, preSkipped, postRequired, postSkipped);
                }
            }
        }

        private void RefreshButtons(string sessionID, string empID, bool attendanceDone, bool preRequired, bool preSkipped, bool postRequired, bool postSkipped)
        {
            bool prePublished = preRequired && !preSkipped && IsPublished(sessionID, "Pre");
            bool postPublished = postRequired && !postSkipped && IsPublished(sessionID, "Post");
            bool preCompleted = preRequired && !preSkipped && IsSubmitted(sessionID, empID, "Pre");
            bool postCompleted = postRequired && !postSkipped && IsSubmitted(sessionID, empID, "Post");

            btnPreTest.Visible = preRequired;
            btnPostTest.Visible = postRequired;

            if (!preRequired)
            {
                btnPreTest.Text = "Pre Test Not Required";
                btnPreTest.Enabled = false;
                btnPreTest.CommandArgument = "";
            }
            else if (preSkipped)
            {
                btnPreTest.Text = "Pre Test Skipped";
                btnPreTest.Enabled = false;
                btnPreTest.CommandArgument = "";
            }
            else if (!prePublished)
            {
                btnPreTest.Text = "Pre Test Not Available";
                btnPreTest.Enabled = false;
                btnPreTest.CommandArgument = "";
            }
            else if (!attendanceDone)
            {
                btnPreTest.Text = "Start Pre Test";
                btnPreTest.Enabled = false;
                btnPreTest.CommandArgument = "";
            }
            else if (preCompleted)
            {
                btnPreTest.Text = "View Result";
                btnPreTest.Enabled = true;
                btnPreTest.CommandArgument = "Result";
            }
            else
            {
                btnPreTest.Text = "Start Pre Test";
                btnPreTest.Enabled = true;
                btnPreTest.CommandArgument = "Start";
            }

            if (!postRequired)
            {
                btnPostTest.Text = "Post Test Not Required";
                btnPostTest.Enabled = false;
                btnPostTest.CommandArgument = "";
            }
            else if (postSkipped)
            {
                btnPostTest.Text = "Post Test Skipped";
                btnPostTest.Enabled = false;
                btnPostTest.CommandArgument = "";
            }
            else if (!postPublished)
            {
                btnPostTest.Text = "Post Test Not Available";
                btnPostTest.Enabled = false;
                btnPostTest.CommandArgument = "";
            }
            else if (!attendanceDone)
            {
                btnPostTest.Text = "Start Post Test";
                btnPostTest.Enabled = false;
                btnPostTest.CommandArgument = "";
            }
            else if (prePublished && !preCompleted)
            {
                btnPostTest.Text = "Waiting for Pre Test";
                btnPostTest.Enabled = false;
                btnPostTest.CommandArgument = "";
            }
            else if (postCompleted)
            {
                btnPostTest.Text = "View Result";
                btnPostTest.Enabled = true;
                btnPostTest.CommandArgument = "Result";
            }
            else
            {
                btnPostTest.Text = "Start Post Test";
                btnPostTest.Enabled = true;
                btnPostTest.CommandArgument = "Start";
            }
        }

        private void SetTestStatus(Label label, string sessionID, string empID, string type, bool required, bool skipped, bool gateOpen, bool pre)
        {
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

            if (IsSubmitted(sessionID, empID, type))
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

        private bool IsSubmitted(string sessionID, string empID, string type)
        {
            object value = objDB.ExecuteScalar(
                "SELECT CASE WHEN EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType=@Type AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1) THEN 1 ELSE 0 END",
                new SqlParameter[]
                {
                    new SqlParameter("@SessionID", sessionID),
                    new SqlParameter("@Type", type),
                    new SqlParameter("@EmpID", empID)
                });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }
    }
}
