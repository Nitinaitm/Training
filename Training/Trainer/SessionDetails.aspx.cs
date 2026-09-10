using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace Training.Trainer
{
    public partial class SessionDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null || Session["SessionID"] == null)
            {
                Response.Redirect("~/Trainer/Default.aspx");
                return;
            }
            if (!IsPostBack)
            {
                TrainerSummary1.LoadTraining(Session["TrainingID"].ToString());
                SessionSummary1.LoadSession(Session["SessionID"].ToString());
                LoadWorkflow();
                LoadSkipStatus();
            }
        }

        private void LoadWorkflow()
        {
            DataTable dt = obj.GetDataTable("SELECT TrainingStatus,WorkflowStatus,AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) });
            if (dt.Rows.Count == 0) { Response.Redirect("~/Trainer/Default.aspx"); return; }
            DataRow r = dt.Rows[0];
            lblTrainingStatus.Text = r["TrainingStatus"].ToString();
            lblWorkflow.Text = r["WorkflowStatus"].ToString();
            btnMaterial.Visible = true;
            btnQuestionBank.Visible = true;
            btnAttendance.Visible = Convert.ToBoolean(r["AttendanceRequired"]);
            btnPreTest.Visible = Convert.ToBoolean(r["InitialAssessmentRequired"]);
            btnPostTest.Visible = Convert.ToBoolean(r["FinalAssessmentRequired"]);
        }

        private void LoadSkipStatus()
        {
            DataTable dt = obj.GetDataTable("SELECT AttendanceSkipped,PreAssessmentSkipped,PostAssessmentSkipped FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            if (dt.Rows.Count == 0) return;
            bool attendance = Convert.ToBoolean(dt.Rows[0]["AttendanceSkipped"]);
            bool pre = Convert.ToBoolean(dt.Rows[0]["PreAssessmentSkipped"]);
            bool post = Convert.ToBoolean(dt.Rows[0]["PostAssessmentSkipped"]);
            lblAttendanceSkip.Text = attendance ? "Skipped" : "Required";
            lblPreSkip.Text = pre ? "Skipped" : "Required";
            lblPostSkip.Text = post ? "Skipped" : "Required";
            btnAttendance.Visible = btnAttendance.Visible && !attendance;
            btnPreTest.Visible = btnPreTest.Visible && !pre;
            btnPostTest.Visible = btnPostTest.Visible && !post;
            btnSkipAttendance.Visible = !attendance && btnAttendance.Visible;
            btnSkipPre.Visible = !pre && btnPreTest.Visible;
            btnSkipPost.Visible = !post && btnPostTest.Visible;
        }

        private bool Skip(string flagColumn, string reasonColumn, TextBoxValue reason)
        {
            if (string.IsNullOrWhiteSpace(reason.Value))
            {
                lblSkipMessage.ForeColor = Color.Red;
                lblSkipMessage.Text = "Skip reason is mandatory.";
                return false;
            }
            string actor = Session["UserID"] == null ? "Trainer" : Session["UserID"].ToString();
            string sql = "UPDATE SessionMaster SET " + flagColumn + "=1," + reasonColumn + "=@Reason," + reasonColumn.Replace("Reason", "By") + "=@By," + reasonColumn.Replace("Reason", "On") + "=GETDATE() WHERE SessionID=@SessionID";
            obj.ExecuteSql(sql, new SqlParameter[] { new SqlParameter("@Reason", reason.Value.Trim()), new SqlParameter("@By", actor), new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            return true;
        }

        protected void btnSkipAttendance_Click(object sender, EventArgs e)
        {
            if (!IsRequired("AttendanceRequired")) return;
            if (Skip("AttendanceSkipped", "AttendanceSkipReason", new TextBoxValue(txtAttendanceSkipReason.Text))) { lblSkipMessage.ForeColor = Color.Green; lblSkipMessage.Text = "Attendance has been skipped for this session."; LoadSkipStatus(); }
        }

        protected void btnSkipPre_Click(object sender, EventArgs e)
        {
            if (!IsRequired("InitialAssessmentRequired")) return;
            if (Skip("PreAssessmentSkipped", "PreAssessmentSkipReason", new TextBoxValue(txtPreSkipReason.Text))) { lblSkipMessage.ForeColor = Color.Green; lblSkipMessage.Text = "Pre-Test has been skipped for this session."; LoadSkipStatus(); }
        }

        protected void btnSkipPost_Click(object sender, EventArgs e)
        {
            if (!IsRequired("FinalAssessmentRequired")) return;
            if (Skip("PostAssessmentSkipped", "PostAssessmentSkipReason", new TextBoxValue(txtPostSkipReason.Text))) { lblSkipMessage.ForeColor = Color.Green; lblSkipMessage.Text = "Post-Test has been skipped for this session."; LoadSkipStatus(); }
        }

        protected void btnAttendance_Click(object sender, EventArgs e) { if (!IsRequired("AttendanceRequired")) return; Response.Redirect("~/Trainer/SessionAttendance.aspx"); }
        protected void btnDashboard_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/Default.aspx"); }
        protected void btnMaterial_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/TrainingMaterial.aspx"); }
        protected void btnQuestionBank_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/QuestionBank.aspx"); }
        protected void btnPreTest_Click(object sender, EventArgs e) { if (!IsRequired("InitialAssessmentRequired")) return; Response.Redirect("~/Trainer/PreTrainingTest.aspx"); }
        protected void btnPostTest_Click(object sender, EventArgs e) { if (!IsRequired("FinalAssessmentRequired")) return; Response.Redirect("~/Trainer/PostTrainingTest.aspx"); }
        protected void btnTestResult_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/TestResult.aspx"); }

        private bool IsRequired(string column)
        {
            if (column != "AttendanceRequired" && column != "InitialAssessmentRequired" && column != "FinalAssessmentRequired") return false;
            object value = obj.ExecuteScalar("SELECT " + column + " FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) });
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private sealed class TextBoxValue
        {
            public string Value { get; private set; }
            public TextBoxValue(string value) { Value = value; }
        }
    }
}