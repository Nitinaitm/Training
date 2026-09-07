using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class SessionDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null || Session["SessionID"] == null)
            { Response.Redirect("~/Trainer/Default.aspx"); return; }
            if (!IsPostBack)
            {
                TrainerSummary1.LoadTraining(Session["TrainingID"].ToString());
                SessionSummary1.LoadSession(Session["SessionID"].ToString());
                LoadWorkflow();
            }
        }

        private void LoadWorkflow()
        {
            DataTable dt = obj.GetDataTable(
                "SELECT TrainingStatus,WorkflowStatus,AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            if (dt.Rows.Count == 0) { Response.Redirect("~/Trainer/Default.aspx"); return; }
            DataRow r = dt.Rows[0];
            lblTrainingStatus.Text = r["TrainingStatus"].ToString();
            lblWorkflow.Text = r["WorkflowStatus"].ToString();

            // These actions are controlled only by the requirements selected for this batch.
            btnMaterial.Visible = true;
            btnQuestionBank.Visible = true;
            btnAttendance.Visible = Convert.ToBoolean(r["AttendanceRequired"]);
            btnPreTest.Visible = Convert.ToBoolean(r["InitialAssessmentRequired"]);
            btnPostTest.Visible = Convert.ToBoolean(r["FinalAssessmentRequired"]);
        }

        protected void btnAttendance_Click(object sender, EventArgs e)
        {
            if (!IsRequired("AttendanceRequired")) return;
            Response.Redirect("~/Trainer/SessionAttendance.aspx");
        }
        protected void btnDashboard_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/Default.aspx"); }
        protected void btnMaterial_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/TrainingMaterial.aspx"); }
        protected void btnQuestionBank_Click(object sender, EventArgs e) { Response.Redirect("~/Trainer/QuestionBank.aspx"); }
        protected void btnPreTest_Click(object sender, EventArgs e)
        {
            if (!IsRequired("InitialAssessmentRequired")) return;
            Response.Redirect("~/Trainer/PreTrainingTest.aspx");
        }
        protected void btnPostTest_Click(object sender, EventArgs e)
        {
            if (!IsRequired("FinalAssessmentRequired")) return;
            Response.Redirect("~/Trainer/PostTrainingTest.aspx");
        }
        private bool IsRequired(string column)
        {
            if (column != "AttendanceRequired" && column != "InitialAssessmentRequired" && column != "FinalAssessmentRequired") return false;
            object value = obj.ExecuteScalar("SELECT " + column + " FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }
    }
}