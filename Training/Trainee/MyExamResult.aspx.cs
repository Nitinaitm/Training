using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MyExamResult :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            
            if
            (
                  Session["EmpID"] == null

            )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                 Session["TrainingID"] == null
                ||
                Session["SessionID"] == null
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            if (!IsPostBack)
            {
                string trainingID =
          Session["TrainingID"].ToString();

                string sessionID =
                    Session["SessionID"].ToString();

                string empID =
                    Session["EmpID"].ToString().ToUpperInvariant();

                SessionSummary1.LoadSession(
                    trainingID,
                    sessionID,
                    empID);

                SetDefaultFilter();

                LoadSessionHeader();

                LoadResult();
            }
        }

        private void LoadSessionHeader()
        {
            string sql =
                "SELECT " +
                "SM.SessionNo," +
                "SM.SessionName " +
                "FROM SessionMaster SM " +
                "WHERE SM.TrainingID=@TrainingID " +
                "AND SM.SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            GetTrainingID()),

        new SqlParameter(
            "@SessionID",
            GetSessionID())
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                lblResultSessionNo.Text =
                    "-";

                lblResultSessionName.Text =
                    "-";

                lblResultTestType.Text =
                    "-";

                return;
            }

            lblResultSessionNo.Text =
                dt.Rows[0]["SessionNo"]
                .ToString();

            lblResultSessionName.Text =
                dt.Rows[0]["SessionName"]
                .ToString();

            string testType =
                ddlTestType.SelectedValue;

            if
            (
                testType == "Pre"
            )
            {
                lblResultTestType.Text =
                    "Pre Training Test";

                lblResultTestType.CssClass =
                    "badge badge-success status-badge";
            }
            else if
            (
                testType == "Post"
            )
            {
                lblResultTestType.Text =
                    "Post Training Test";

                lblResultTestType.CssClass =
                    "badge badge-primary status-badge";
            }
            else
            {
                lblResultTestType.Text =
                    "Pre & Post Test";

                lblResultTestType.CssClass =
                    "badge badge-secondary status-badge";
            }
        }

        private string GetEmpID()
        {
            return
                Session["EmpID"]
                .ToString()
                .Trim().ToUpperInvariant();
        }

        private string GetTrainingID()
        {
            return
                Session["TrainingID"]
                .ToString()
                .Trim();
        }

        private string GetSessionID()
        {
            return
                Session["SessionID"]
                .ToString()
                .Trim();
        }

       

        private void SetDefaultFilter()
        {
            ddlAttempt.SelectedValue =
                "All";

            if
            (
                Session["ResultTestType"] != null
            )
            {
                string testType =
                    Session["ResultTestType"]
                    .ToString();

                if
                (
                    testType == "Pre"
                    ||
                    testType == "Post"
                )
                {
                    ddlTestType.SelectedValue =
                        testType;
                }
            }
        }

        private void LoadResult()
        {
            lblMessage.Text =
                "";

            BindResultGrid();

            BindComparison();
        }

        private void BindResultGrid()
        {
            string query =
                "SELECT " +
                "TR.ResultID," +
                "TR.TestID," +
                "SM.SessionID," +
                "SM.SessionNo," +
                "SM.SessionName," +
                "TM.TestType," +
                "TM.TestTitle," +
                "TR.AttemptNo," +
                "TR.TotalQuestions," +
                "TR.AttemptedQuestions," +
                "TR.CorrectAnswers," +
                "TR.WrongAnswers," +
                "TR.TotalMarks," +
                "TR.ObtainedMarks," +
                "TR.Percentage," +
                "TR.ResultStatus," +
                "TR.TimeTaken," +
                "TR.SubmittedOn," +
                "TR.RankNo," +
                "TR.IsFinalAttempt " +
                "FROM TestResult TR " +
                "INNER JOIN TestMaster TM " +
                "ON TM.TestID=TR.TestID " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "WHERE TR.EmpID=@EmpID " +
                "AND SM.TrainingID=@TrainingID " +
                "AND SM.SessionID=@SessionID ";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTestType.SelectedValue)
            )
            {
                query +=
                    "AND TM.TestType=@TestType ";
            }

            if
            (
                ddlAttempt.SelectedValue
                ==
                "Final"
            )
            {
                query +=
                    "AND TR.IsFinalAttempt=1 ";
            }

            query +=
                "ORDER BY " +
                "CASE " +
                "WHEN TM.TestType='Pre' THEN 1 " +
                "WHEN TM.TestType='Post' THEN 2 " +
                "ELSE 3 " +
                "END," +
                "TR.AttemptNo ASC," +
                "TR.SubmittedOn ASC";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    GetEmpID()),

                new SqlParameter(
                    "@TrainingID",
                    GetTrainingID()),

                new SqlParameter(
                    "@SessionID",
                    GetSessionID()),

                new SqlParameter(
                    "@TestType",
                    String.IsNullOrWhiteSpace(
                        ddlTestType.SelectedValue)
                    ?
                    (object)DBNull.Value
                    :
                    ddlTestType.SelectedValue)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            gvResult.DataSource =
                dt;

            gvResult.DataBind();

            btnExport.Enabled =
                dt.Rows.Count > 0;

            if
            (
                dt.Rows.Count == 0
            )
            {
                lblMessage.Text =
                    "No result is available for this session.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Gray;
            }
        }

        private void BindComparison()
        {
            string query =
                "SELECT " +
                "TM.TestType," +
                "AVG(CAST(TR.Percentage AS DECIMAL(10,2))) AS AveragePercentage " +
                "FROM TestResult TR " +
                "INNER JOIN TestMaster TM " +
                "ON TM.TestID=TR.TestID " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "WHERE TR.EmpID=@EmpID " +
                "AND SM.TrainingID=@TrainingID " +
                "AND SM.SessionID=@SessionID " +
                "AND TR.IsFinalAttempt=1 " +
                "AND TM.TestType IN ('Pre','Post') " +
                "GROUP BY TM.TestType";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    GetEmpID()),

                new SqlParameter(
                    "@TrainingID",
                    GetTrainingID()),

                new SqlParameter(
                    "@SessionID",
                    GetSessionID())
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            decimal? prePercentage =
                null;

            decimal? postPercentage =
                null;

            foreach
            (
                DataRow dr
                in dt.Rows
            )
            {
                if
                (
                    dr["AveragePercentage"]
                    ==
                    DBNull.Value
                )
                {
                    continue;
                }

                string testType =
                    dr["TestType"]
                    .ToString();

                decimal percentage =
                    Convert.ToDecimal(
                        dr["AveragePercentage"]);

                if
                (
                    testType.Equals(
                        "Pre",
                        StringComparison.OrdinalIgnoreCase)
                )
                {
                    prePercentage =
                        percentage;
                }

                if
                (
                    testType.Equals(
                        "Post",
                        StringComparison.OrdinalIgnoreCase)
                )
                {
                    postPercentage =
                        percentage;
                }
            }

            lblPrePercentage.Text =
                prePercentage.HasValue
                ?
                prePercentage.Value.ToString(
                    "0.00")
                +
                " %"
                :
                "-";

            lblPostPercentage.Text =
                postPercentage.HasValue
                ?
                postPercentage.Value.ToString(
                    "0.00")
                +
                " %"
                :
                "-";

            if
            (
                prePercentage.HasValue
                &&
                postPercentage.HasValue
            )
            {
                decimal improvement =
                    postPercentage.Value
                    -
                    prePercentage.Value;

                if
                (
                    improvement > 0
                )
                {
                    lblImprovement.Text =
                        "+"
                        +
                        improvement.ToString(
                            "0.00")
                        +
                        " %";

                    lblImprovement.CssClass =
                        "summary-value improvement-positive";
                }
                else if
                (
                    improvement < 0
                )
                {
                    lblImprovement.Text =
                        improvement.ToString(
                            "0.00")
                        +
                        " %";

                    lblImprovement.CssClass =
                        "summary-value improvement-negative";
                }
                else
                {
                    lblImprovement.Text =
                        "0.00 %";

                    lblImprovement.CssClass =
                        "summary-value";
                }
            }
            else
            {
                lblImprovement.Text =
                    "-";

                lblImprovement.CssClass =
                    "summary-value";
            }

            LoadPostResult();

            pnlComparison.Visible =
                prePercentage.HasValue
                ||
                postPercentage.HasValue;
        }

        private void LoadPostResult()
        {
            string query =
                "SELECT TOP 1 " +
                "TR.ResultStatus " +
                "FROM TestResult TR " +
                "INNER JOIN TestMaster TM " +
                "ON TM.TestID=TR.TestID " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "WHERE TR.EmpID=@EmpID " +
                "AND SM.TrainingID=@TrainingID " +
                "AND SM.SessionID=@SessionID " +
                "AND TM.TestType='Post' " +
                "AND TR.IsFinalAttempt=1 " +
                "ORDER BY " +
                "TR.SubmittedOn DESC," +
                "TR.ID DESC";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    GetEmpID()),

                new SqlParameter(
                    "@TrainingID",
                    GetTrainingID()),

                new SqlParameter(
                    "@SessionID",
                    GetSessionID())
            };

            object result =
                objDB.ExecuteScalar(
                    query,
                    param);

            if
            (
                result == null
                ||
                result == DBNull.Value
                ||
                String.IsNullOrWhiteSpace(
                    Convert.ToString(
                        result))
            )
            {
                lblPostResult.Text =
                    "-";

                lblPostResult.CssClass =
                    "summary-value";

                return;
            }

            string resultStatus =
                Convert.ToString(
                    result);

            lblPostResult.Text =
                resultStatus;

            if
            (
                resultStatus.Equals(
                    "PASS",
                    StringComparison.OrdinalIgnoreCase)
                ||
                resultStatus.Equals(
                    "PASSED",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                lblPostResult.CssClass =
                    "summary-value result-pass";
            }
            else
            {
                lblPostResult.CssClass =
                    "summary-value result-fail";
            }
        }

        protected void btnSearch_Click(
    object sender,
    EventArgs e)
        {
            lblMessage.Text =
                "";

            LoadSessionHeader();

            BindResultGrid();

            BindComparison();
        }

        protected void btnReset_Click(
    object sender,
    EventArgs e)
        {
            ddlTestType.SelectedIndex =
                0;

            ddlAttempt.SelectedValue =
                "All";

            lblMessage.Text =
                "";

            LoadSessionHeader();

            LoadResult();
        }

        protected void gvResult_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if
            (
                e.Row.RowType
                !=
                DataControlRowType.DataRow
            )
            {
                return;
            }

            Label lblResult =
                e.Row.FindControl(
                    "lblResult")
                as Label;

            if
            (
                lblResult == null
            )
            {
                return;
            }

            string result =
                lblResult.Text.Trim();

            if
            (
                result.Equals(
                    "PASS",
                    StringComparison.OrdinalIgnoreCase)
                ||
                result.Equals(
                    "PASSED",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                lblResult.CssClass =
                    "result-pass";
            }
            else if
            (
                result.Equals(
                    "FAIL",
                    StringComparison.OrdinalIgnoreCase)
                ||
                result.Equals(
                    "FAILED",
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                lblResult.CssClass =
                    "result-fail";
            }
        }

        protected string FormatTimeTaken(
            object value)
        {
            if
            (
                value == null
                ||
                value == DBNull.Value
                ||
                String.IsNullOrWhiteSpace(
                    Convert.ToString(
                        value))
            )
            {
                return "-";
            }

            int seconds;

            if
            (
                Int32.TryParse(
                    Convert.ToString(
                        value),
                    out seconds)
            )
            {
                if
                (
                    seconds < 0
                )
                {
                    seconds =
                        0;
                }

                TimeSpan time =
                    TimeSpan.FromSeconds(
                        seconds);

                if
                (
                    time.TotalHours >= 1
                )
                {
                    return
                        ((int)time.TotalHours)
                        .ToString("00")
                        +
                        ":"
                        +
                        time.Minutes.ToString("00")
                        +
                        ":"
                        +
                        time.Seconds.ToString("00");
                }

                return
                    time.Minutes.ToString("00")
                    +
                    ":"
                    +
                    time.Seconds.ToString("00");
            }

            return
                Convert.ToString(
                    value);
        }

        protected void btnExport_Click(
            object sender,
            EventArgs e)
        {
            string query =
                "SELECT " +
                "SM.SessionNo AS [Session No]," +
                "SM.SessionName AS [Session]," +
                "CASE " +
                "WHEN TM.TestType='Pre' THEN 'Pre Training' " +
                "WHEN TM.TestType='Post' THEN 'Post Training' " +
                "ELSE TM.TestType " +
                "END AS [Exam]," +
                "TM.TestTitle AS [Test Title]," +
                "TR.AttemptNo AS [Attempt]," +
                "TR.TotalQuestions AS [Total Questions]," +
                "TR.AttemptedQuestions AS [Attempted Questions]," +
                "TR.CorrectAnswers AS [Correct Answers]," +
                "TR.WrongAnswers AS [Wrong Answers]," +
                "TR.TotalMarks AS [Total Marks]," +
                "TR.ObtainedMarks AS [Obtained Marks]," +
                "TR.Percentage AS [Percentage]," +
                "TR.ResultStatus AS [Result]," +
                "TR.TimeTaken AS [Time Taken]," +
                "TR.SubmittedOn AS [Submitted On]," +
                "CASE " +
                "WHEN TR.IsFinalAttempt=1 THEN 'Yes' " +
                "ELSE 'No' " +
                "END AS [Final Attempt] " +
                "FROM TestResult TR " +
                "INNER JOIN TestMaster TM " +
                "ON TM.TestID=TR.TestID " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "WHERE TR.EmpID=@EmpID " +
                "AND SM.TrainingID=@TrainingID " +
                "AND SM.SessionID=@SessionID ";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTestType.SelectedValue)
            )
            {
                query +=
                    "AND TM.TestType=@TestType ";
            }

            if
            (
                ddlAttempt.SelectedValue
                ==
                "Final"
            )
            {
                query +=
                    "AND TR.IsFinalAttempt=1 ";
            }

            query +=
                "ORDER BY " +
                "CASE " +
                "WHEN TM.TestType='Pre' THEN 1 " +
                "WHEN TM.TestType='Post' THEN 2 " +
                "ELSE 3 " +
                "END," +
                "TR.AttemptNo ASC";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    GetEmpID()),

                new SqlParameter(
                    "@TrainingID",
                    GetTrainingID()),

                new SqlParameter(
                    "@SessionID",
                    GetSessionID()),

                new SqlParameter(
                    "@TestType",
                    String.IsNullOrWhiteSpace(
                        ddlTestType.SelectedValue)
                    ?
                    (object)DBNull.Value
                    :
                    ddlTestType.SelectedValue)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            ExportDataTable(
                dt);
        }

        private void ExportDataTable(
            DataTable dt)
        {
            if
            (
                dt == null
                ||
                dt.Rows.Count == 0
            )
            {
                ShowError(
                    "No result is available for export.");

                return;
            }

            GridView gvExport =
                new GridView();

            gvExport.DataSource =
                dt;

            gvExport.DataBind();

            Response.Clear();

            Response.Buffer =
                true;

            Response.AddHeader(
                "content-disposition",
                "attachment;filename=MyExamResult_"
                +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmss")
                +
                ".xls");

            Response.Charset =
                "";

            Response.ContentType =
                "application/vnd.ms-excel";

            StringWriter sw =
                new StringWriter();

            HtmlTextWriter hw =
                new HtmlTextWriter(
                    sw);

            gvExport.RenderControl(
                hw);

            Response.Output.Write(
                sw.ToString());

            Response.Flush();

            HttpContext.Current
                .ApplicationInstance
                .CompleteRequest();
        }

        public override void VerifyRenderingInServerForm(
            Control control)
        {
        }

        private void ShowError(
            string message)
        {
            lblMessage.ForeColor =
                System.Drawing.Color.Red;

            lblMessage.Text =
                message;
        }
    }
}