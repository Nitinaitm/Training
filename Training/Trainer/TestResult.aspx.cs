using System;
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
            if (Session["TrainerID"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (Session["TrainingID"] == null || string.IsNullOrWhiteSpace(Session["TrainingID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (Session["SessionID"] == null || string.IsNullOrWhiteSpace(Session["SessionID"].ToString()))
            {
                Response.Redirect("~/Trainer/SessionDetails.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadSessionInfo();
                LoadSummary();
                BindGrid();
            }
        }

        private string TrainingID => Session["TrainingID"].ToString();
        private string SessionID => Session["SessionID"].ToString();

        private void LoadSessionInfo()
        {
            string query = @"SELECT SM.SessionID, SM.TrainingID, SM.Topic,
                                    CONVERT(varchar(10), SM.SessionDate, 105) AS SessionDate,
                                    TD.TrainingType
                             FROM SessionMaster SM
                             LEFT JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID
                             WHERE SM.SessionID=@SessionID AND SM.TrainingID=@TrainingID";

            DataTable dt = obj.GetDataTable(query, new SqlParameter[]
            {
                new SqlParameter("@SessionID", SessionID),
                new SqlParameter("@TrainingID", TrainingID)
            });

            if (dt.Rows.Count == 0)
            {
                Response.Redirect("~/Trainer/SessionDetails.aspx");
                return;
            }

            DataRow dr = dt.Rows[0];
            lblTrainingID.Text = dr["TrainingID"].ToString();
            lblSessionID.Text = dr["SessionID"].ToString();
            lblTitle.Text = dr["Topic"].ToString();
            lblPassing.Text = dr["SessionDate"].ToString();
            lblQuestions.Text = dr["TrainingType"].ToString();
        }

        private void LoadSummary()
        {
            string query = @"SELECT
                COUNT(DISTINCT R.EmpID) AS Total,
                COUNT(DISTINCT CASE WHEN R.Status='Pass' THEN R.EmpID END) AS Passed,
                COUNT(DISTINCT CASE WHEN R.Status='Fail' THEN R.EmpID END) AS Failed,
                ISNULL(AVG(R.Score),0) AS AvgScore
                FROM TestResult R
                INNER JOIN TestMaster TM ON TM.TestID=R.TestID
                INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID
                WHERE SM.TrainingID=@TrainingID AND SM.SessionID=@SessionID";

            DataTable dt = obj.GetDataTable(query, new SqlParameter[]
            {
                new SqlParameter("@TrainingID", TrainingID),
                new SqlParameter("@SessionID", SessionID)
            });

            if (dt.Rows.Count > 0)
            {
                lblTotal.Text = dt.Rows[0]["Total"].ToString();
                lblPassed.Text = dt.Rows[0]["Passed"].ToString();
                lblFailed.Text = dt.Rows[0]["Failed"].ToString();
                lblAvgScore.Text = Math.Round(Convert.ToDecimal(dt.Rows[0]["AvgScore"]), 2).ToString() + "%";
            }
        }

        private void BindGrid()
        {
            string query = @"SELECT
                TA.EmpID,
                E.EmpName,
                E.EmpDesignation,
                MAX(CASE WHEN TM.TestType='Pre' THEN R.ResultID END) AS PreResultID,
                MAX(CASE WHEN TM.TestType='Pre' THEN R.Score END) AS PreScore,
                MAX(CASE WHEN TM.TestType='Pre' THEN R.Status END) AS PreStatus,
                MAX(CASE WHEN TM.TestType='Post' THEN R.ResultID END) AS PostResultID,
                MAX(CASE WHEN TM.TestType='Post' THEN R.Score END) AS PostScore,
                MAX(CASE WHEN TM.TestType='Post' THEN R.Status END) AS PostStatus
                FROM TrainingAssignment TA
                INNER JOIN EmpBasicMaster E ON E.EmpID=TA.EmpID
                LEFT JOIN TestResult R ON R.EmpID=TA.EmpID
                LEFT JOIN TestMaster TM ON TM.TestID=R.TestID AND TM.SessionID=@SessionID
                WHERE TA.TrainingID=@TrainingID
                  AND TA.AssignmentStatus='Assigned'
                  AND EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=@SessionID AND SA.EmpID=TA.EmpID)
                  AND (TM.TestID IS NOT NULL OR EXISTS (SELECT 1 FROM SessionAttendance SA2 WHERE SA2.SessionID=@SessionID AND SA2.EmpID=TA.EmpID))";

            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
                query += " AND (E.EmpID LIKE @Search OR E.EmpName LIKE @Search)";

            query += " GROUP BY TA.EmpID, E.EmpName, E.EmpDesignation ORDER BY E.EmpName";

            SqlParameter[] parameters = string.IsNullOrEmpty(txtSearch.Text.Trim())
                ? new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@SessionID", SessionID) }
                : new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@SessionID", SessionID), new SqlParameter("@Search", "%" + txtSearch.Text.Trim() + "%") };

            DataTable dt = obj.GetDataTable(query, parameters);
            gvResults.DataSource = dt;
            gvResults.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e) => BindGrid();

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            BindGrid();
        }

        protected void gvResults_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            Label pre = (Label)e.Row.FindControl("lblPreStatus");
            Label post = (Label)e.Row.FindControl("lblPostStatus");
            if (pre != null && pre.Text == "Pass") pre.CssClass = "badge bg-success status-badge";
            else if (pre != null && !string.IsNullOrEmpty(pre.Text)) pre.CssClass = "badge bg-danger status-badge";
            if (post != null && post.Text == "Pass") post.CssClass = "badge bg-success status-badge";
            else if (post != null && !string.IsNullOrEmpty(post.Text)) post.CssClass = "badge bg-danger status-badge";
        }

        protected void gvResults_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                Session["ResultID"] = e.CommandArgument.ToString();
                Response.Redirect("~/Trainer/AnswerDetails.aspx");
            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string query = @"SELECT TA.EmpID, E.EmpName, E.EmpDesignation,
                MAX(CASE WHEN TM.TestType='Pre' THEN R.Score END) AS PreScore,
                MAX(CASE WHEN TM.TestType='Pre' THEN R.Status END) AS PreStatus,
                MAX(CASE WHEN TM.TestType='Post' THEN R.Score END) AS PostScore,
                MAX(CASE WHEN TM.TestType='Post' THEN R.Status END) AS PostStatus
                FROM TrainingAssignment TA
                INNER JOIN EmpBasicMaster E ON E.EmpID=TA.EmpID
                LEFT JOIN TestResult R ON R.EmpID=TA.EmpID
                LEFT JOIN TestMaster TM ON TM.TestID=R.TestID AND TM.SessionID=@SessionID
                WHERE TA.TrainingID=@TrainingID AND TA.AssignmentStatus='Assigned'
                AND EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=@SessionID AND SA.EmpID=TA.EmpID)
                GROUP BY TA.EmpID,E.EmpName,E.EmpDesignation ORDER BY E.EmpName";

            DataTable dt = obj.GetDataTable(query, new SqlParameter[]
            {
                new SqlParameter("@TrainingID", TrainingID),
                new SqlParameter("@SessionID", SessionID)
            });
            if (dt.Rows.Count == 0) return;

            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=SessionTestResult.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            hw.Write("<table border='1'><tr>");
            foreach (DataColumn col in dt.Columns) hw.Write("<th>" + col.ColumnName + "</th>");
            hw.Write("</tr>");
            foreach (DataRow row in dt.Rows)
            {
                hw.Write("<tr>");
                foreach (DataColumn col in dt.Columns) hw.Write("<td>" + row[col].ToString() + "</td>");
                hw.Write("</tr>");
            }
            hw.Write("</table>");
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/SessionDetails.aspx");
        }
    }
}