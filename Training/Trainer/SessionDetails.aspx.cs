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
            
            if (Session["TrainingID"] == null) Response.Redirect("~/Trainer/Default.aspx");
            
            if (Session["SessionID"] == null) Response.Redirect("~/Trainer/Default.aspx");

            if (!IsPostBack)
            {
                TrainerSummary1.LoadTraining(Session["TrainingID"].ToString());

                SessionSummary1.LoadSession(Session["SessionID"].ToString());

                LoadWorkflow();
            }
        }

        private void LoadWorkflow()
        {
            string query = @"SELECT TrainingStatus,WorkflowStatus FROM TrainingDetails WHERE TrainingID=@TrainingID";

            SqlParameter[] param =
            {
        new SqlParameter("@TrainingID",Session["TrainingID"].ToString())
    };

            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            lblTrainingStatus.Text = dt.Rows[0]["TrainingStatus"].ToString();

            lblWorkflow.Text = dt.Rows[0]["WorkflowStatus"].ToString();

            SetButtonStatus();
        }



        private bool SessionAttendanceCompleted()
        {
            string query = @"SELECT ISNULL(AttendanceStatus,'Pending') FROM SessionMaster WHERE SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter("@SessionID",Session["SessionID"].ToString())
    };

            object objValue = obj.ExecuteScalar(query, param);

            if (objValue == null)
            {
                return false;
            }

            return objValue.ToString() == "Completed";
        }

        private void SetButtonStatus()
        {
            string workflow = lblWorkflow.Text.Trim();

            bool attendanceCompleted = SessionAttendanceCompleted();

            btnAttendance.Visible = false;

            btnMaterial.Visible = false;

            btnQuestionBank.Visible = false;

            btnPreTest.Visible = false;

            btnPostTest.Visible = false;

            if (workflow == "ABCDE")
            {
                btnAttendance.Visible = true;

                btnMaterial.Visible = true;

                btnQuestionBank.Visible = true;

                return;
            }

            if (workflow == "ABCDEF")
            {
                btnMaterial.Visible = true;

                btnQuestionBank.Visible = true;

                btnPreTest.Visible = true;

                btnAttendance.Visible = true;

                btnAttendance.Text = "Attendance Completed";

                btnAttendance.Enabled = false;

                return;
            }

            if (workflow == "ABCDEFG")
            {
                btnMaterial.Visible = true;

                btnQuestionBank.Visible = true;

                btnPreTest.Visible = true;

                btnPostTest.Visible = true;

                btnAttendance.Visible = true;

                btnAttendance.Text = "Attendance Completed";

                btnAttendance.Enabled = false;

                btnPreTest.Text = "Pre Test Completed";

                btnPreTest.Enabled = false;

                return;
            }

            if (workflow == "ABCDEFGH")
            {
                btnMaterial.Visible = true;

                btnQuestionBank.Visible = true;

                btnAttendance.Visible = true;

                btnAttendance.Text = "Attendance Completed";

                btnAttendance.Enabled = false;

                btnPreTest.Visible = true;

                btnPreTest.Text = "Pre Test Completed";

                btnPreTest.Enabled = false;

                btnPostTest.Visible = true;

                btnPostTest.Text = "Post Test Completed";

                btnPostTest.Enabled = false;

                return;
            }

            if (workflow == "ABCDEFGHI")
            {
                btnMaterial.Visible = false;

                btnQuestionBank.Visible = false;

                btnAttendance.Visible = false;

                btnPreTest.Visible = false;

                btnPostTest.Visible = false;

                return;
            }

            if (workflow == "ABCDEFGHIJ")
            {
                btnMaterial.Visible = false;

                btnQuestionBank.Visible = false;

                btnAttendance.Visible = false;

                btnPreTest.Visible = false;

                btnPostTest.Visible = false;
            }
        }

        protected void btnAttendance_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/SessionAttendance.aspx");
        }
        protected void btnDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/Default.aspx");
        }

        protected void btnMaterial_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/TrainingMaterial.aspx");
        }

        protected void btnQuestionBank_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/QuestionBank.aspx");
        }

        protected void btnPreTest_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/PreTrainingTest.aspx");
        }

        protected void btnPostTest_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/PostTrainingTest.aspx");
        }
    }
}