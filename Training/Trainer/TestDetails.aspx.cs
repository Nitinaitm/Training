using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class TestDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");

            if (Session["TestID"] == null)
            {
                Response.Redirect("~/Trainer/PreTrainingTest.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTestDetails();
                LoadQuestions();
            }
        }

        private string TestID => Session["TestID"].ToString();

        private void LoadTestDetails()
        {
            string query = @"SELECT TestID, Title, TrainingID, Duration, TotalQuestions, PassingPercent, Status, CreatedOn FROM TestMaster WHERE TestID=@TestID AND IsActive=1";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("~/Trainer/PreTrainingTest.aspx");
                return;
            }

            DataRow dr = dt.Rows[0];

            lblTestID.Text = dr["TestID"].ToString();
            lblTitle.Text = dr["Title"].ToString();
            lblTrainingID.Text = dr["TrainingID"].ToString();
            lblDuration.Text = dr["Duration"].ToString();
            lblTotalQuestions.Text = dr["TotalQuestions"].ToString();
            lblPassing.Text = dr["PassingPercent"].ToString() + "%";

            string status = dr["Status"].ToString();
            lblStatus.Text = status;
            if (status == "Completed")
                lblStatus.CssClass = "badge bg-success fs-5 fw-bold";
            else
                lblStatus.CssClass = "badge bg-warning text-dark fs-5 fw-bold";
        }

        private void LoadQuestions()
        {
            string query = @"SELECT QB.Question, QB.Type, QB.Category, QB.OptionA, QB.OptionB, QB.OptionC, QB.OptionD, QB.Answer, QB.Marks FROM TestDetail TD INNER JOIN QuestionBank QB ON TD.QuestionID = QB.QuestionID WHERE TD.TestID = @TestID ORDER BY TD.SequenceNo";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            DataTable dt = obj.GetDataTable(query, param);
            gvQuestions.DataSource = dt;
            gvQuestions.DataBind();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            // Check which page called (Pre or Post)
            string referrer = Request.UrlReferrer?.ToString() ?? "";
            if (referrer.Contains("PostTrainingTest"))
                Response.Redirect("~/Trainer/PostTrainingTest.aspx");
            else
                Response.Redirect("~/Trainer/PreTrainingTest.aspx");
        }
    }
}