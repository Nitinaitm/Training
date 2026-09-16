using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class AnswerDetails : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");

            if (Session["ResultID"] == null)
            {
                Response.Redirect("~/Trainer/TestResult.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadTraineeInfo();
                LoadAnswers();
            }
        }

        private string ResultID => Session["ResultID"].ToString();

        private void LoadTraineeInfo()
        {
            string query = @"SELECT R.ResultID, R.TestID, R.EmpID, R.TotalQuestions, R.CorrectAnswers, R.Score, R.Status, R.SubmittedOn,
                                   E.EmpName, E.EmpDesignation 
                            FROM TestResult R 
                            INNER JOIN EmpBasicMaster E ON R.EmpID = E.EmpID 
                            WHERE R.ResultID = @ResultID";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@ResultID", ResultID) };
            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("~/Trainer/TestResult.aspx");
                return;
            }

            DataRow dr = dt.Rows[0];

            lblEmpID.Text = dr["EmpID"].ToString();
            lblEmpName.Text = dr["EmpName"].ToString();
            lblDesignation.Text = dr["EmpDesignation"].ToString();
            lblTestID.Text = dr["TestID"].ToString();
            lblTotalQ.Text = dr["TotalQuestions"].ToString();
            lblCorrect.Text = dr["CorrectAnswers"].ToString();

            decimal score = Convert.ToDecimal(dr["Score"]);
            lblScore.Text = score.ToString("F2") + "%";

            string status = dr["Status"].ToString();
            lblStatus.Text = status;
            if (status == "Pass")
                lblStatus.CssClass = "badge bg-success fs-5 fw-bold";
            else
                lblStatus.CssClass = "badge bg-danger fs-5 fw-bold";
        }

        private void LoadAnswers()
        {
            string query = @"SELECT 
                                    QB.Question, 
                                    QB.Type, 
                                    QB.OptionA, 
                                    QB.OptionB, 
                                    QB.OptionC, 
                                    QB.OptionD, 
                                    QB.Answer AS CorrectAnswer, 
                                    TA.SelectedAnswer, 
                                    TA.IsCorrect 
                            FROM TestAttempt TA 
                            INNER JOIN QuestionBank QB ON TA.QuestionID = QB.QuestionID 
                            WHERE TA.ResultID = @ResultID 
                            ORDER BY TA.SequenceNo";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@ResultID", ResultID) };
            DataTable dt = obj.GetDataTable(query, param);

            // Add IsCorrect as string for display
            dt.Columns.Add("IsCorrectStr", typeof(string));
            foreach (DataRow row in dt.Rows)
            {
                row["IsCorrectStr"] = row["IsCorrect"].ToString();
            }

            gvAnswers.DataSource = dt;
            gvAnswers.DataBind();
        }

        

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/TestResult.aspx");
        }

        protected string GetOptions(object optA, object optB, object optC, object optD, object selected, object correct)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div style='padding:5px;'>");

            string[] options = { optA?.ToString() ?? "", optB?.ToString() ?? "", optC?.ToString() ?? "", optD?.ToString() ?? "" };
            string[] labels = { "A", "B", "C", "D" };
            string selectedAns = selected?.ToString() ?? "";
            string correctAns = correct?.ToString() ?? "";

            for (int i = 0; i < options.Length; i++)
            {
                if (string.IsNullOrEmpty(options[i])) continue;

                string label = labels[i];
                bool isSelected = (selectedAns == label);
                bool isCorrect = (correctAns == label);

                string bgColor = "";
                if (isSelected && isCorrect)
                    bgColor = "background-color:#d4edda;";
                else if (isSelected && !isCorrect)
                    bgColor = "background-color:#f8d7da;";
                else if (isCorrect)
                    bgColor = "background-color:#cce5ff;";

                sb.Append($"<div style='padding:4px 8px;margin:2px 0;border-radius:4px;{bgColor}'>");
                sb.Append($"<span style='font-weight:bold;width:25px;display:inline-block;'>{label}.</span>");
                sb.Append($"<span>{options[i]}</span>");

                if (isSelected)
                    sb.Append(" <span class='badge bg-secondary ms-2'>Selected</span>");
                if (isCorrect)
                    sb.Append(" <span class='badge bg-primary ms-2'>Correct</span>");

                sb.Append("</div>");
            }

            sb.Append("</div>");
            return sb.ToString();
        }
    }
}