using System;
using System.Data;
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
                string role = Convert.ToString(Session["Role"]);
                string trainingID = Convert.ToString(Session["TrainingID"]);
                string sessionID = Convert.ToString(Session["SessionID"]);

                if (string.IsNullOrWhiteSpace(trainingID) && !string.IsNullOrWhiteSpace(sessionID))
                {
                    trainingID = Convert.ToString(
                        new clsDataAccess().ExecuteScalar(
                            "SELECT TrainingID FROM SessionMaster WHERE SessionID=@SessionID",
                            new System.Data.SqlClient.SqlParameter[]
                            {
                                new System.Data.SqlClient.SqlParameter("@SessionID", sessionID)
                            }));
                }

                if (string.IsNullOrWhiteSpace(trainingID))
                    return;

                DataTable td = new clsDataAccess().GetDataTable(
                    "SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,CertificateRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,ISNULL(CertificateSkipped,0) CertificateSkipped FROM TrainingDetails WHERE TrainingID=@TrainingID",
                    new System.Data.SqlClient.SqlParameter[]
                    {
                        new System.Data.SqlClient.SqlParameter("@TrainingID", trainingID)
                    });

                if (td.Rows.Count == 0)
                    return;

                DataRow r = td.Rows[0];
                bool trainee = string.Equals(role, "Trainee", StringComparison.OrdinalIgnoreCase);
                string empID = Convert.ToString(Session["EmpID"]);
                string trainerID = Convert.ToString(Session["TrainerID"]);
                StringBuilder h = new StringBuilder();

                AddStage(h, "Attendance", Convert.ToBoolean(r["AttendanceRequired"]), GetAttendance(trainingID, empID, trainerID, trainee));
                AddStage(h, "Pre-Training Test", Convert.ToBoolean(r["InitialAssessmentRequired"]), GetAssessment(trainingID, "Pre", empID, trainerID, trainee));
                AddStage(h, "Post-Training Test", Convert.ToBoolean(r["FinalAssessmentRequired"]), GetAssessment(trainingID, "Post", empID, trainerID, trainee));
                AddStage(h, "Batch Feedback", Convert.ToBoolean(r["FeedbackRequired"]), GetFeedback(trainingID, empID, trainee), Convert.ToBoolean(r["FeedbackSkipped"]));
                AddStage(h, "Certificate", Convert.ToBoolean(r["CertificateRequired"]), GetCertificate(trainingID, empID, trainee), Convert.ToBoolean(r["CertificateSkipped"]));

                litLifecycle.Text = "<div class='bl-line'>" + h.ToString() + "</div>";
                pnlLifecycle.Visible = true;
            }
            catch
            {
                pnlLifecycle.Visible = false;
            }
        }

        private void AddStage(StringBuilder h, string name, bool required, StageInfo i, bool skipped = false)
        {
            string css;
            string state;
            string bubble;
            string tooltip;
            string style = "";

            if (skipped)
            {
                css = "skipped";
                state = "Skipped";
                bubble = "–";
                tooltip = name + ": Skipped";
            }
            else if (!required)
            {
                css = "na";
                state = "Not Required";
                bubble = "–";
                tooltip = name + ": Not Required";
            }
            else
            {
                if (i.Total > 0)
                {
                    if (i.Completed < 0)
                        i.Completed = 0;
                    if (i.Completed > i.Total)
                        i.Completed = i.Total;

                    int percent = (int)Math.Round(i.Completed * 100.0 / i.Total);

                    if (i.Completed >= i.Total)
                    {
                        css = "done";
                        state = "✓ Completed";
                        bubble = "✓";
                        tooltip = name + ": " + i.Completed + "/" + i.Total + " (100%) - Completed";
                    }
                    else
                    {
                        css = "partial";
                        state = i.Completed > 0 ? "In Progress" : "Pending";
                        bubble = "";
                        style = "background:conic-gradient(#198754 0% " + percent + "%, #dc3545 " + percent + "% 100%);";
                        tooltip = name + ": " + i.Completed + "/" + i.Total + " (" + percent + "%) - " + state;
                    }
                }
                else
                {
                    css = "pending";
                    state = "Pending";
                    bubble = "";
                    tooltip = name + ": Pending";
                }
            }

            tooltip = " data-tooltip='" + HttpUtility.HtmlAttributeEncode(tooltip) + "' title='" + HttpUtility.HtmlAttributeEncode(tooltip) + "'";

            h.Append(
                "<div class='bl-item " + css + "'>" +
                "<div class='bl-bubble'" + tooltip + (style == "" ? "" : " style='" + style + "'") + ">" +
                bubble +
                "</div>" +
                "<div class='bl-label'>" + HttpUtility.HtmlEncode(name) + "</div>" +
                "<div class='bl-state'>" + HttpUtility.HtmlEncode(state) + "</div>" +
                "</div>");
        }

        private StageInfo GetAttendance(string t, string e, string tr, bool trainee)
        {
            string q = trainee
                ? "SELECT COUNT(*) Total,SUM(CASE WHEN EXISTS(SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus IN ('Present','Completed')) THEN 1 ELSE 0 END) Completed FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0"
                : "SELECT COUNT(*) Total,SUM(CASE WHEN EXISTS(SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.AttendanceStatus IN ('Present','Completed')) THEN 1 ELSE 0 END) Completed FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND SM.TrainerID=@TrainerID";

            return ReadStage(
                new clsDataAccess().GetDataTable(
                    q,
                    trainee
                        ? new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t),
                            new System.Data.SqlClient.SqlParameter("@EmpID", e)
                        }
                        : new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t),
                            new System.Data.SqlClient.SqlParameter("@TrainerID", tr)
                        }));
        }

        private StageInfo GetAssessment(string t, string type, string e, string tr, bool trainee)
        {
            string skip = type == "Pre" ? "PreAssessmentSkipped" : "PostAssessmentSkipped";
            string q = trainee
                ? "SELECT COUNT(*) Total,SUM(CASE WHEN EXISTS(SELECT 1 FROM TestMaster T WHERE T.SessionID=SM.SessionID AND T.TestType=@Type AND T.IsPublished=1) AND EXISTS(SELECT 1 FROM TestMaster T INNER JOIN TestAttempt A ON A.TestID=T.TestID WHERE T.SessionID=SM.SessionID AND T.TestType=@Type AND T.IsPublished=1 AND A.EmpID=@EmpID AND A.Submitted=1) THEN 1 WHEN ISNULL(SM." + skip + ",0)=1 THEN 1 ELSE 0 END) Completed FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND (ISNULL(SM." + skip + ",0)=1 OR EXISTS(SELECT 1 FROM TestMaster T WHERE T.SessionID=SM.SessionID AND T.TestType=@Type AND T.IsPublished=1))"
                : "SELECT COUNT(*) Total,SUM(CASE WHEN ISNULL(SM." + skip + ",0)=1 OR EXISTS(SELECT 1 FROM TestMaster T WHERE T.SessionID=SM.SessionID AND T.TestType=@Type AND T.IsPublished=1) THEN 1 ELSE 0 END) Completed FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID AND (ISNULL(SM." + skip + ",0)=1 OR EXISTS(SELECT 1 FROM TestMaster T WHERE T.SessionID=SM.SessionID AND T.TestType=@Type AND T.IsPublished=1))";

            return ReadStage(
                new clsDataAccess().GetDataTable(
                    q,
                    trainee
                        ? new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t),
                            new System.Data.SqlClient.SqlParameter("@Type", type),
                            new System.Data.SqlClient.SqlParameter("@EmpID", e)
                        }
                        : new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t),
                            new System.Data.SqlClient.SqlParameter("@Type", type),
                            new System.Data.SqlClient.SqlParameter("@TrainerID", tr)
                        }));
        }

        private StageInfo GetFeedback(string t, string e, bool trainee)
        {
            string q = trainee
                ? "SELECT COUNT(*) Total,SUM(CASE WHEN Submitted=1 THEN 1 ELSE 0 END) Completed FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID"
                : "SELECT (SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')='Assigned') Total,(SELECT COUNT(DISTINCT EmpID) FROM Feedback WHERE TrainingID=@TrainingID AND Submitted=1) Completed";

            return ReadStage(
                new clsDataAccess().GetDataTable(
                    q,
                    trainee
                        ? new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t),
                            new System.Data.SqlClient.SqlParameter("@EmpID", e)
                        }
                        : new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t)
                        }));
        }

        private StageInfo GetCertificate(string t, string e, bool trainee)
        {
            string q = trainee
                ? "SELECT 1 Total,CASE WHEN EXISTS(SELECT 1 FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(CertificateStatus,'A')='A') THEN 1 ELSE 0 END Completed"
                : "SELECT (SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')='Assigned') Total,(SELECT COUNT(DISTINCT EmpID) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND ISNULL(CertificateStatus,'A')='A') Completed";

            return ReadStage(
                new clsDataAccess().GetDataTable(
                    q,
                    trainee
                        ? new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t),
                            new System.Data.SqlClient.SqlParameter("@EmpID", e)
                        }
                        : new System.Data.SqlClient.SqlParameter[]
                        {
                            new System.Data.SqlClient.SqlParameter("@TrainingID", t)
                        }));
        }

        private StageInfo ReadStage(DataTable d)
        {
            int total = 0;
            int done = 0;

            if (d.Rows.Count > 0)
            {
                total = d.Rows[0]["Total"] == DBNull.Value ? 0 : Convert.ToInt32(d.Rows[0]["Total"]);
                done = d.Rows[0]["Completed"] == DBNull.Value ? 0 : Convert.ToInt32(d.Rows[0]["Completed"]);
            }

            decimal p = total > 0 ? Math.Round(done * 100m / total, 0) : 0;

            return new StageInfo
            {
                Total = total,
                Completed = done,
                Percent = p,
                State = total == 0 ? "Pending" : done >= total ? "Completed" : done == 0 ? "Pending" : "In Progress"
            };
        }

        private class StageInfo
        {
            public int Total;
            public int Completed;
            public decimal Percent;
            public string State;
        }
    }
}