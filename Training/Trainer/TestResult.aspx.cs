using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class TestResult : System.Web.UI.Page
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
                LoadTestInfo();
                LoadSummary();
                BindGrid();
            }
        }

        private string TestID => Session["TestID"].ToString();

        private void LoadTestInfo()
        {
            string query = @"SELECT TestID, Title, PassingPercent, TotalQuestions FROM TestMaster WHERE TestID=@TestID AND IsActive=1";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                lblTestID.Text = dr["TestID"].ToString();
                lblTitle.Text = dr["Title"].ToString();
                lblPassing.Text = dr["PassingPercent"].ToString() + "%";
                lblQuestions.Text = dr["TotalQuestions"].ToString();
            }
        }

        private void LoadSummary()
        {
            string totalQuery = @"SELECT COUNT(*) FROM TestResult WHERE TestID=@TestID";
            SqlParameter[] totalParam = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            int total = Convert.ToInt32(obj.ExecuteScalar(totalQuery, totalParam) ?? "0");
            lblTotal.Text = total.ToString();

            string passedQuery = @"SELECT COUNT(*) FROM TestResult WHERE TestID=@TestID AND Status='Pass'";
            SqlParameter[] passedParam = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            int passed = Convert.ToInt32(obj.ExecuteScalar(passedQuery, passedParam) ?? "0");
            lblPassed.Text = passed.ToString();

            string failedQuery = @"SELECT COUNT(*) FROM TestResult WHERE TestID=@TestID AND Status='Fail'";
            SqlParameter[] failedParam = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            int failed = Convert.ToInt32(obj.ExecuteScalar(failedQuery, failedParam) ?? "0");
            lblFailed.Text = failed.ToString();

            string avgQuery = @"SELECT ISNULL(AVG(Score),0) FROM TestResult WHERE TestID=@TestID";
            SqlParameter[] avgParam = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            object avg = obj.ExecuteScalar(avgQuery, avgParam);
            lblAvgScore.Text = avg == null ? "0%" : Math.Round(Convert.ToDecimal(avg), 2).ToString() + "%";
        }

        private void BindGrid()
        {
            string query = @"SELECT R.ResultID, R.EmpID, E.EmpName, E.EmpDesignation, R.TotalQuestions, R.CorrectAnswers, R.Score, R.Status, R.SubmittedOn FROM TestResult R INNER JOIN EmpBasicMaster E ON R.EmpID=E.EmpID WHERE R.TestID=@TestID ";
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@TestID", TestID));

            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            { query += " AND (E.EmpID LIKE @Search OR E.EmpName LIKE @Search)"; parameters.Add(new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%")); }

            if (!string.IsNullOrEmpty(ddlStatus.SelectedValue))
            { query += " AND R.Status = @Status"; parameters.Add(new SqlParameter("@Status", ddlStatus.SelectedValue)); }

            query += " ORDER BY R.Score DESC";
            DataTable dt = obj.GetDataTable(query, parameters.ToArray());
            gvResults.DataSource = dt;
            gvResults.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e) => BindGrid();

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            ddlStatus.SelectedIndex = 0;
            BindGrid();
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblStatus = (Label)e.Row.FindControl("lblStatus");
                if (lblStatus != null)
                {
                    if (lblStatus.Text == "Pass")
                        lblStatus.CssClass = "badge bg-success status-badge";
                    else
                        lblStatus.CssClass = "badge bg-danger status-badge";
                }
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            string referrer = Request.UrlReferrer?.ToString() ?? "";
            if (referrer.Contains("PostTrainingTest"))
                Response.Redirect("~/Trainer/PostTrainingTest.aspx");
            else
                Response.Redirect("~/Trainer/PreTrainingTest.aspx");
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string query = @"SELECT R.EmpID, E.EmpName, E.EmpDesignation, R.TotalQuestions, R.CorrectAnswers, R.Score, R.Status, R.SubmittedOn FROM TestResult R INNER JOIN EmpBasicMaster E ON R.EmpID=E.EmpID WHERE R.TestID=@TestID ORDER BY R.Score DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TestID", TestID) };
            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0) return;

            // Change column names for Excel
            dt.Columns["EmpID"].ColumnName = "Employee ID";
            dt.Columns["EmpName"].ColumnName = "Employee Name";
            dt.Columns["EmpDesignation"].ColumnName = "Designation";
            dt.Columns["TotalQuestions"].ColumnName = "Total Questions";
            dt.Columns["CorrectAnswers"].ColumnName = "Correct Answers";
            dt.Columns["Score"].ColumnName = "Score (%)";
            dt.Columns["Status"].ColumnName = "Status";
            dt.Columns["SubmittedOn"].ColumnName = "Submitted On";

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=TestResult.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Write header
            hw.Write("<table border='1'><tr>");
            foreach (DataColumn col in dt.Columns)
            {
                hw.Write("<th>" + col.ColumnName + "</th>");
            }
            hw.Write("</tr>");

            // Write data
            foreach (DataRow row in dt.Rows)
            {
                hw.Write("<tr>");
                foreach (DataColumn col in dt.Columns)
                {
                    hw.Write("<td>" + row[col].ToString() + "</td>");
                }
                hw.Write("</tr>");
            }
            hw.Write("</table>");

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        // TestResult.aspx.cs में Add करें

        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                Session["ResultID"] = e.CommandArgument.ToString();
                Response.Redirect("~/Trainer/AnswerDetails.aspx");
            }
        }
    }
}