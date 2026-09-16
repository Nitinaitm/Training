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
            if (Session["EmpID"] == null || string.IsNullOrWhiteSpace(Session["EmpID"].ToString()))
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
            DataTable dt = objDB.GetDataTable("SELECT DISTINCT CM.CourseID,CM.CourseName FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TA.EmpID=@EmpID ORDER BY CM.CourseName",
                new SqlParameter[] { new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()) });
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
                    feedback.Text = "Feedback Submitted";
                    feedback.Enabled = false;
                    feedback.CssClass = "btn btn-success btn-sm disabled";
                    feedback.ToolTip = "Batch feedback has already been submitted.";
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
                    bool allowed = true;
                    if (attendanceRequired) allowed = allowed && attendanceDone;
                    if (preRequired) allowed = allowed && preDone;
                    if (postRequired) allowed = allowed && postDone;
                    if (feedbackRequired && !feedbackSkipped) allowed = allowed && feedbackDone;

                    certificate.Enabled = allowed;
                    certificate.CssClass = allowed ? "btn btn-info btn-sm" : "btn btn-info btn-sm disabled";
                    certificate.ToolTip = allowed ? "Download Certificate" : "Complete the required training workflow before downloading the certificate.";
                }
            }

            if (attendance != null && !attendance.Enabled)
                attendance.CssClass = "btn btn-primary btn-sm disabled";
        }

        protected void gvTraining_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string trainingID = Convert.ToString(e.CommandArgument);
            if (string.IsNullOrWhiteSpace(trainingID)) return;

            Session["TrainingID"] = trainingID;
            if (e.CommandName == "ViewTraining") { Response.Redirect("TrainingDetails.aspx", false); return; }
            if (e.CommandName == "Attendance") { Response.Redirect("Attendance.aspx", false); return; }
            if (e.CommandName == "BatchFeedback") { Response.Redirect("TraineeFeedback.aspx", false); return; }
            if (e.CommandName == "Certificate")
            {
                Session["CertificateFromTraining"] = true;
                Response.Redirect("MyCertificate.aspx", false);
            }
        }

        private void LoadTraining()
        {
            string sql = @"SELECT TA.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,
TRY_CONVERT(date,TD.DateFrom,105) DateFrom,TRY_CONVERT(date,TD.DateTo,105) DateTo,
TD.AttendanceRequired,TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,
TD.FeedbackRequired,ISNULL(TD.FeedbackSkipped,0) FeedbackSkipped,
TD.CertificateRequired,ISNULL(TD.CertificateSkipped,0) CertificateSkipped,
CASE WHEN ISNULL(TD.AttendanceRequired,0)=0 THEN 1
     WHEN NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(SM.AttendanceStatus,'')<>'Completed') THEN 1 ELSE 0 END AS AttendanceDone,
CASE WHEN ISNULL(TD.InitialAssessmentRequired,0)=0 THEN 1
     WHEN NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.PreAssessmentSkipped,0)=0
         AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1)
              OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND AT.EmpID=TA.EmpID AND AT.Submitted=1)
              OR (ISNULL(TD.AttendanceRequired,0)=1 AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(SM.AttendanceStatus,'')<>'Completed'))) THEN 1 ELSE 0 END AS PreDone,
CASE WHEN ISNULL(TD.FinalAssessmentRequired,0)=0 THEN 1
     WHEN NOT EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND ISNULL(SM.PostAssessmentSkipped,0)=0
         AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1)
              OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND AT.EmpID=TA.EmpID AND AT.Submitted=1)
              OR (ISNULL(TD.AttendanceRequired,0)=1 AND ISNULL(SM.AttendanceSkipped,0)=0 AND ISNULL(SM.AttendanceStatus,'')<>'Completed')
              OR (ISNULL(TD.InitialAssessmentRequired,0)=1 AND ISNULL(SM.PreAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt AT ON AT.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND AT.EmpID=TA.EmpID AND AT.Submitted=1)))) THEN 1 ELSE 0 END AS PostDone,
CASE WHEN ISNULL(TD.FeedbackRequired,0)=0 OR ISNULL(TD.FeedbackSkipped,0)=1 THEN 1
     WHEN EXISTS (SELECT 1 FROM BatchFeedback BF WHERE BF.TrainingID=TA.TrainingID AND BF.EmpID=TA.EmpID AND ISNULL(BF.Submitted,0)=1) THEN 1 ELSE 0 END AS FeedbackDone,
CASE WHEN ISNULL(TD.CertificateRequired,0)=0 OR ISNULL(TD.CertificateSkipped,0)=1 THEN 1
     WHEN EXISTS (SELECT 1 FROM TrainingCertificate TC WHERE TC.TrainingID=TA.TrainingID AND TC.EmpID=TA.EmpID AND TC.CertificateStatus='A') THEN 1 ELSE 0 END AS CertificateDone
FROM TrainingAssignment TA
INNER JOIN TrainingDetails TD ON TD.TrainingID=TA.TrainingID
INNER JOIN CourseMaster CM ON CM.CourseID=TD.CourseID
WHERE TA.EmpID=@EmpID ";

            if (txtTrainingID.Text.Trim() != "") sql += " AND TA.TrainingID LIKE @TrainingID ";
            if (ddlCourse.SelectedValue != "") sql += " AND TD.CourseID=@CourseID ";
            sql += " ORDER BY " + SortColumn() + " " + SortDirection();

            SqlParameter[] p =
            {
                new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()),
                new SqlParameter("@TrainingID", "%" + txtTrainingID.Text.Trim() + "%"),
                new SqlParameter("@CourseID", ddlCourse.SelectedValue)
            };

            DataTable dt = objDB.GetDataTable(sql, p);
            dt.Columns.Add("ProgressPercent", typeof(int));
            dt.Columns.Add("StatusText");
            dt.Columns.Add("StatusClass");

            foreach (DataRow r in dt.Rows)
            {
                int done = 0, total = 0;
                if (Convert.ToBoolean(r["AttendanceRequired"])) { total++; if (Convert.ToBoolean(r["AttendanceDone"])) done++; }
                if (Convert.ToBoolean(r["InitialAssessmentRequired"])) { total++; if (Convert.ToBoolean(r["PreDone"])) done++; }
                if (Convert.ToBoolean(r["FinalAssessmentRequired"])) { total++; if (Convert.ToBoolean(r["PostDone"])) done++; }
                if (Convert.ToBoolean(r["FeedbackRequired"]) && !Convert.ToBoolean(r["FeedbackSkipped"])) { total++; if (Convert.ToBoolean(r["FeedbackDone"])) done++; }
                if (Convert.ToBoolean(r["CertificateRequired"]) && !Convert.ToBoolean(r["CertificateSkipped"])) { total++; if (Convert.ToBoolean(r["CertificateDone"])) done++; }
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
            else gvTraining.DataSource = dt;
            gvTraining.DataBind();
        }
    }
}