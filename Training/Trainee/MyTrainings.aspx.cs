using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MyTrainings : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()) || Session["Role"] == null || !string.Equals(Session["Role"].ToString(), "Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadCourse();
                ViewState["SortExpression"] = "TrainingID";
                ViewState["SortDirection"] = "DESC";
                ddlStatus.SelectedIndex = 0;
                LoadTraining();
            }
        }

        private string SortColumn()
        {
            string s = Convert.ToString(ViewState["SortExpression"]);
            switch (s)
            {
                case "TrainingID":
                case "CourseName":
                case "TrainingType":
                case "TrainingOrganizer":
                case "Batch":
                case "DateFrom":
                case "DateTo":
                    return s;
                default:
                    return "TrainingID";
            }
        }

        private string SortDirection()
        {
            return Convert.ToString(ViewState["SortDirection"]) == "ASC" ? "ASC" : "DESC";
        }

        private void LoadCourse()
        {
            DataTable dt = objDB.GetDataTable("SELECT DISTINCT CM.CourseID,CM.CourseName FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TA.EmpID=@EmpID ORDER BY CM.CourseName",
                new SqlParameter[] { new SqlParameter("@EmpID", Session["EmpID"].ToString().Trim().ToUpperInvariant()) });
            ddlCourse.DataSource = dt;
            ddlCourse.DataTextField = "CourseName";
            ddlCourse.DataValueField = "CourseID";
            ddlCourse.DataBind();
            ddlCourse.Items.Insert(0, new ListItem("All", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            gvTraining.PageIndex = 0;
            LoadTraining();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtTrainingID.Text = "";
            ddlCourse.SelectedIndex = 0;
            ddlStatus.SelectedIndex = 0;
            ViewState["SortExpression"] = "TrainingID";
            ViewState["SortDirection"] = "DESC";
            gvTraining.PageIndex = 0;
            LoadTraining();
        }

        protected void gvTraining_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTraining.PageIndex = e.NewPageIndex;
            LoadTraining();
        }

        protected void gvTraining_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (Convert.ToString(ViewState["SortExpression"]) == e.SortExpression)
                ViewState["SortDirection"] = Convert.ToString(ViewState["SortDirection"]) == "ASC" ? "DESC" : "ASC";
            else
            {
                ViewState["SortExpression"] = e.SortExpression;
                ViewState["SortDirection"] = "ASC";
            }
            LoadTraining();
        }

        protected void gvTraining_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            LinkButton feedback = (LinkButton)e.Row.FindControl("lnkFeedback");
            LinkButton certificate = (LinkButton)e.Row.FindControl("lnkCertificate");
            LinkButton attendance = (LinkButton)e.Row.FindControl("lnkAttendance");
            DataRowView data = e.Row.DataItem as DataRowView;

            if (data == null) return;

            bool attendanceRequired = Convert.ToBoolean(data["AttendanceRequired"]);
            bool preRequired = Convert.ToBoolean(data["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(data["FinalAssessmentRequired"]);
            bool feedbackRequired = Convert.ToBoolean(data["FeedbackRequired"]);
            bool feedbackSkipped = Convert.ToBoolean(data["FeedbackSkipped"]);
            bool certificateRequired = Convert.ToBoolean(data["CertificateRequired"]);
            bool certificateSkipped = Convert.ToBoolean(data["CertificateSkipped"]);
            bool attendanceDone = Convert.ToBoolean(data["AttendanceDone"]);
            bool preDone = Convert.ToBoolean(data["PreDone"]);
            bool postDone = Convert.ToBoolean(data["PostDone"]);
            bool feedbackDone = Convert.ToBoolean(data["FeedbackDone"]);

            if (feedback != null)
            {
                if (!feedbackRequired || feedbackSkipped)
                {
                    feedback.Visible = false;
                }
                else if (feedbackDone)
                {
                    feedback.Text = "View Feedback";
                    feedback.Enabled = true;
                    feedback.CssClass = "btn btn-success btn-sm";
                    feedback.ToolTip = "View submitted feedback in read-only mode.";
                }
                else if (attendanceRequired && !attendanceDone)
                {
                    feedback.Enabled = false;
                    feedback.CssClass = "btn btn-warning btn-sm disabled";
                    feedback.ToolTip = "Complete required attendance first.";
                }
                else if (preRequired && !preDone)
                {
                    feedback.Enabled = false;
                    feedback.CssClass = "btn btn-warning btn-sm disabled";
                    feedback.ToolTip = "Complete all required Pre-Training Tests first. Required tests must be published.";
                }
                else if (postRequired && !postDone)
                {
                    feedback.Enabled = false;
                    feedback.CssClass = "btn btn-warning btn-sm disabled";
                    feedback.ToolTip = "Complete all required Post-Training Tests first. Required tests must be published.";
                }
                else
                {
                    feedback.Enabled = true;
                    feedback.CssClass = "btn btn-warning btn-sm";
                    feedback.ToolTip = "You can submit Batch Feedback now.";
                }
            }

            if (certificate != null)
            {
                if (!certificateRequired || certificateSkipped)
                {
                    certificate.Visible = false;
                }
                else
                {
                    bool allowed = attendanceRequired ? attendanceDone : true;
                    if (feedbackRequired && !feedbackSkipped) allowed = allowed && feedbackDone;
                    if (allowed) allowed = IsCertificateTestEligible(data["TrainingID"].ToString(), Session["EmpID"].ToString().Trim().ToUpperInvariant());
                    certificate.Enabled = allowed;
                    certificate.CssClass = allowed ? "btn btn-info btn-sm" : "btn btn-info btn-sm disabled";
                    certificate.ToolTip = allowed ? "Download Certificate" : "Complete the required training workflow before downloading the certificate.";
                }
            }

            if (attendance != null && !attendance.Enabled)
                attendance.CssClass = "btn btn-primary btn-sm disabled";
        }

        private bool IsCertificateTestEligible(string trainingID, string empID)
        {
            DataTable dt = objDB.GetDataTable("SELECT InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            if (dt.Rows.Count == 0) return false;
            bool preRequired = Convert.ToBoolean(dt.Rows[0]["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);

            if (preRequired && !AreSessionWiseTestsEligible(trainingID, empID, "Pre")) return false;
            if (postRequired && !AreSessionWiseTestsEligible(trainingID, empID, "Post")) return false;
            return true;
        }

        private bool IsAttendancePercentageEligible(string trainingID, string empID, object minimumValue)
        {
            if (minimumValue == null || minimumValue == DBNull.Value) return false;
            decimal minimum = Convert.ToDecimal(minimumValue);
            object totalValue = objDB.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(AttendanceSkipped,0)=0", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            int total = totalValue == null || totalValue == DBNull.Value ? 0 : Convert.ToInt32(totalValue);
            if (total == 0) return false;
            object presentValue = objDB.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster SM INNER JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND SA.AttendanceStatus IN ('Present','Completed')", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
            int present = presentValue == null || presentValue == DBNull.Value ? 0 : Convert.ToInt32(presentValue);
            decimal percentage = present * 100m / total;
            return percentage >= minimum;
        }

        private bool AreSessionWiseTestsEligible(string trainingID, string empID, string testType)
        {
            string skipColumn = testType == "Pre" ? "PreAssessmentSkipped" : "PostAssessmentSkipped";
            string ruleColumn = testType == "Pre" ? "PreTestCertificateRule" : "PostTestCertificateRule";
            string requiredColumn = testType == "Pre" ? "InitialAssessmentRequired" : "FinalAssessmentRequired";
            string sql = "SELECT SM.SessionID,ISNULL(SM." + skipColumn + ",0) Skipped,ISNULL(SM." + ruleColumn + ",'') RuleValue FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.TrainingID=@TrainingID AND ISNULL(TD." + requiredColumn + ",0)=1 ORDER BY SM.SessionID";
            DataTable sessions = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });

            foreach (DataRow session in sessions.Rows)
            {
                if (Convert.ToBoolean(session["Skipped"])) continue;
                string rule = Convert.ToString(session["RuleValue"]).Trim().ToUpperInvariant();
                if (rule != "PASS" && rule != "ALL") return false;
                string sessionID = Convert.ToString(session["SessionID"]);

                object testCount = objDB.ExecuteScalar("SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType=@TestType AND IsPublished=1", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TestType", testType) });
                if (testCount == null || Convert.ToInt32(testCount) == 0) return false;

                object submittedCount = objDB.ExecuteScalar("SELECT COUNT(*) FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=@SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TestType", testType), new SqlParameter("@EmpID", empID) });
                if (submittedCount == null || Convert.ToInt32(submittedCount) == 0) return false;

                if (rule == "PASS")
                {
                    object passCount = objDB.ExecuteScalar("SELECT COUNT(*) FROM TestMaster TM INNER JOIN TestResult TR ON TR.TestID=TM.TestID AND TR.EmpID=@EmpID WHERE TM.SessionID=@SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TR.IsFinalAttempt=1 AND TR.ResultStatus IN ('PASS','PASSED')", new SqlParameter[] { new SqlParameter("@SessionID", sessionID), new SqlParameter("@TestType", testType), new SqlParameter("@EmpID", empID) });
                    if (passCount == null || Convert.ToInt32(passCount) == 0) return false;
                }
            }

            return true;
        }

        protected void gvTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string trainingID = Convert.ToString(e.CommandArgument);
            if (string.IsNullOrWhiteSpace(trainingID)) return;

            Session["TrainingID"] = trainingID;
            if (e.CommandName == "ViewTraining") { Response.Redirect("TrainingDetails.aspx", false); return; }
            if (e.CommandName == "Attendance") { Response.Redirect("Attendance.aspx", false); return; }
            if (e.CommandName == "BatchFeedback") { Response.Redirect("TraineeFeedback.aspx?mode=view", false); return; }
            if (e.CommandName == "Certificate")
            {
                Session["CertificateFromTraining"] = true;
                Response.Redirect("MyCertificate.aspx", false);
            }
        }

        private void LoadTraining()
        {
            string sql = "SELECT TA.TrainingID,ISNULL(CM.CourseName,'') AS CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) AS DateFrom,TRY_CONVERT(date,TD.DateTo,105) AS DateTo,ISNULL(TD.AttendanceRequired,0) AS AttendanceRequired,ISNULL(TD.InitialAssessmentRequired,0) AS InitialAssessmentRequired,ISNULL(TD.FinalAssessmentRequired,0) AS FinalAssessmentRequired,ISNULL(TD.FeedbackRequired,0) AS FeedbackRequired,ISNULL(TD.FeedbackSkipped,0) AS FeedbackSkipped,ISNULL(TD.CertificateRequired,0) AS CertificateRequired,ISNULL(TD.CertificateSkipped,0) AS CertificateSkipped,TD.MinimumAttendancePercentage,ISNULL(TD.TrainingStatus,'') AS TrainingStatus FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID LEFT JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE LTRIM(RTRIM(TA.EmpID))=LTRIM(RTRIM(@EmpID))";

            if (txtTrainingID.Text.Trim() != "") sql += " AND TA.TrainingID LIKE @TrainingID";
            if (ddlCourse.SelectedValue != "") sql += " AND TD.CourseID=@CourseID";
            sql += " ORDER BY " + SortColumn() + " " + SortDirection();

            SqlParameter[] p =
            {
                new SqlParameter("@EmpID", Session["EmpID"].ToString().Trim().ToUpperInvariant()),
                new SqlParameter("@TrainingID", "%" + txtTrainingID.Text.Trim() + "%"),
                new SqlParameter("@CourseID", ddlCourse.SelectedValue)
            };

            DataTable dt = objDB.GetDataTable(sql, p);
            dt.Columns.Add("AttendanceDone", typeof(bool));
            dt.Columns.Add("PreDone", typeof(bool));
            dt.Columns.Add("PostDone", typeof(bool));
            dt.Columns.Add("FeedbackDone", typeof(bool));
            dt.Columns.Add("CertificateDone", typeof(bool));
            dt.Columns.Add("ProgressPercent", typeof(int));
            dt.Columns.Add("StatusText");
            dt.Columns.Add("StatusClass");

            string empID = Session["EmpID"].ToString().Trim().ToUpperInvariant();

            foreach (DataRow r in dt.Rows)
            {
                string trainingID = Convert.ToString(r["TrainingID"]);
                bool attendanceRequired = Convert.ToBoolean(r["AttendanceRequired"]);
                bool preRequired = Convert.ToBoolean(r["InitialAssessmentRequired"]);
                bool postRequired = Convert.ToBoolean(r["FinalAssessmentRequired"]);
                bool feedbackRequired = Convert.ToBoolean(r["FeedbackRequired"]);
                bool feedbackSkipped = Convert.ToBoolean(r["FeedbackSkipped"]);
                bool certificateRequired = Convert.ToBoolean(r["CertificateRequired"]);
                bool certificateSkipped = Convert.ToBoolean(r["CertificateSkipped"]);

                bool attendanceDone = !attendanceRequired || IsAttendancePercentageEligible(trainingID, empID, r["MinimumAttendancePercentage"]);
                bool preDone = !preRequired || Convert.ToInt32(objDB.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.PreAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND AT.EmpID=@EmpID AND AT.Submitted=1))", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) })) == 0;
                bool postDone = !postRequired || Convert.ToInt32(objDB.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND ISNULL(SM.PostAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND AT.EmpID=@EmpID AND AT.Submitted=1))", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) })) == 0;
                bool feedbackDone = !feedbackRequired || feedbackSkipped || Convert.ToInt32(objDB.ExecuteScalar("SELECT COUNT(*) FROM BatchFeedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(Submitted,0)=1", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) })) > 0;
                bool certificateDone = !certificateRequired || certificateSkipped || Convert.ToInt32(objDB.ExecuteScalar("SELECT COUNT(*) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) })) > 0;

                r["AttendanceDone"] = attendanceDone;
                r["PreDone"] = preDone;
                r["PostDone"] = postDone;
                r["FeedbackDone"] = feedbackDone;
                r["CertificateDone"] = certificateDone;

                int done = 0;
                int total = 0;
                if (attendanceRequired) { total++; if (attendanceDone) done++; }
                if (preRequired) { total++; if (preDone) done++; }
                if (postRequired) { total++; if (postDone) done++; }
                if (feedbackRequired && !feedbackSkipped) { total++; if (feedbackDone) done++; }
                if (certificateRequired && !certificateSkipped) { total++; if (certificateDone) done++; }

                r["ProgressPercent"] = total == 0 ? 100 : done * 100 / total;
                r["StatusText"] = done == total ? "Completed" : done > 0 ? "In Progress" : "Pending";
                r["StatusClass"] = done == total ? "badge badge-success badge-status" : done > 0 ? "badge badge-warning badge-status" : "badge badge-secondary badge-status";
            }

            string selectedStatus = ddlStatus.SelectedValue;
            if (selectedStatus != "")
            {
                DataView view = dt.DefaultView;
                if (selectedStatus == "P") view.RowFilter = "StatusText='Pending'";
                else if (selectedStatus == "I") view.RowFilter = "StatusText='In Progress'";
                else if (selectedStatus == "C") view.RowFilter = "StatusText='Completed'";
                gvTraining.DataSource = view;
            }
            else
            {
                gvTraining.DataSource = dt;
            }
            gvTraining.DataBind();
        }

    }
}