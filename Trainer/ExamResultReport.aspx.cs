using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class ExamResultReport :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        //-------------------------------------------------------
        // PAGE LOAD
        //-------------------------------------------------------

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if
            (
                Session["TrainerID"]
                ==
                null
            )
            {
                Response.Redirect(
                    "~/Login.aspx");

                return;
            }

            if (!IsPostBack)
            {
                LoadTrainerDetails();

                BindTraining();

                BindTest();

                LoadReport();
            }
        }

        //-------------------------------------------------------
        // TRAINER ID
        //-------------------------------------------------------

        private string GetTrainerID()
        {
            if
            (
                Session["TrainerID"]
                ==
                null
            )
            {
                return "";
            }

            return
                Session["TrainerID"]
                .ToString()
                .Trim();
        }

        //-------------------------------------------------------
        // LOAD TRAINER DETAILS
        //-------------------------------------------------------

        private void LoadTrainerDetails()
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT TM.TrainerID, CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(EBM.EmpName,ISNULL(TM.EmpID,TM.TrainerID)) ELSE ISNULL(TM.NameExternal,ISNULL(TM.EmpIDExternal,TM.TrainerID)) END AS TrainerName FROM TrainerMaster TM LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TM.EmpID WHERE TM.TrainerID=@TrainerID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerID",
                    trainerID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            lblTrainerID.Text =
                trainerID;

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                lblTrainerName.Text =
                    "";

                return;
            }

            lblTrainerName.Text =
                Convert.ToString(
                    dt.Rows[0]["TrainerName"]);
        }

        //-------------------------------------------------------
        // BIND TRAINING
        //-------------------------------------------------------

        private void BindTraining()
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT DISTINCT TD.TrainingID, TD.TrainingID + ' | ' + ISNULL(CM.CourseName,'') + ' | Batch ' + ISNULL(TD.Batch,'') AS TrainingName, TD.DateFrom FROM TestMaster TM INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TM.TrainerID=@TrainerID ORDER BY TD.DateFrom DESC, TD.TrainingID DESC";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerID",
                    trainerID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            ddlTraining.DataSource =
                dt;

            ddlTraining.DataTextField =
                "TrainingName";

            ddlTraining.DataValueField =
                "TrainingID";

            ddlTraining.DataBind();

            ddlTraining.Items.Insert(
                0,
                new ListItem(
                    "-- All Training --",
                    ""));
        }

        //-------------------------------------------------------
        // BIND TEST
        //-------------------------------------------------------

        private void BindTest()
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT TestID, TestTitle, TestType FROM TestMaster WHERE TrainerID=@TrainerID";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND TrainingID=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTestType.SelectedValue)
            )
            {
                query +=
                    " AND TestType=@TestType";
            }

            query +=
                " ORDER BY TestType, TestTitle";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerID",
                    trainerID),

                new SqlParameter(
                    "@TrainingID",
                    String.IsNullOrWhiteSpace(
                        ddlTraining.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTraining.SelectedValue),

                new SqlParameter(
                    "@TestType",
                    String.IsNullOrWhiteSpace(
                        ddlTestType.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTestType.SelectedValue)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            ddlTest.DataSource =
                dt;

            ddlTest.DataTextField =
                "TestTitle";

            ddlTest.DataValueField =
                "TestID";

            ddlTest.DataBind();

            ddlTest.Items.Insert(
                0,
                new ListItem(
                    "-- All Tests --",
                    ""));
        }

        //-------------------------------------------------------
        // TRAINING CHANGE
        //-------------------------------------------------------

        protected void ddlTraining_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindTest();
        }

        //-------------------------------------------------------
        // TEST TYPE CHANGE
        //-------------------------------------------------------

        protected void ddlTestType_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindTest();
        }

        //-------------------------------------------------------
        // LOAD REPORT
        //-------------------------------------------------------

        private void LoadReport()
        {
            lblMessage.Text =
                "";

            DateTime fromDate;

            DateTime toDate;

            if
            (
                !ValidateDates(
                    out fromDate,
                    out toDate)
            )
            {
                ClearReport();

                return;
            }

            BindSummary(
                fromDate,
                toDate);

            BindResultGrid(
                fromDate,
                toDate);

            BindComparison(
                fromDate,
                toDate);
        }

        //-------------------------------------------------------
        // SUMMARY
        //-------------------------------------------------------

        private void BindSummary(
            DateTime fromDate,
            DateTime toDate)
        {
            string query =
                "SELECT COUNT(*) AS TotalResults, SUM(CASE WHEN UPPER(ISNULL(TR.ResultStatus,'')) IN ('PASS','PASSED') THEN 1 ELSE 0 END) AS Passed, SUM(CASE WHEN UPPER(ISNULL(TR.ResultStatus,'')) IN ('FAIL','FAILED') THEN 1 ELSE 0 END) AS Failed, AVG(CAST(TR.Percentage AS DECIMAL(10,2))) AS AveragePercentage FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TR.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TR.EmpID WHERE TM.TrainerID=@TrainerID";

            AddResultFilters(
                ref query);

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            int totalResults =
                0;

            int passed =
                0;

            int failed =
                0;

            decimal averagePercentage =
                0;

            if
            (
                dt.Rows.Count
                >
                0
            )
            {
                DataRow dr =
                    dt.Rows[0];

                if
                (
                    dr["TotalResults"]
                    !=
                    DBNull.Value
                )
                {
                    totalResults =
                        Convert.ToInt32(
                            dr["TotalResults"]);
                }

                if
                (
                    dr["Passed"]
                    !=
                    DBNull.Value
                )
                {
                    passed =
                        Convert.ToInt32(
                            dr["Passed"]);
                }

                if
                (
                    dr["Failed"]
                    !=
                    DBNull.Value
                )
                {
                    failed =
                        Convert.ToInt32(
                            dr["Failed"]);
                }

                if
                (
                    dr["AveragePercentage"]
                    !=
                    DBNull.Value
                )
                {
                    averagePercentage =
                        Convert.ToDecimal(
                            dr["AveragePercentage"]);
                }
            }

            lblTotalResults.Text =
                totalResults.ToString();

            lblPassed.Text =
                passed.ToString();

            lblFailed.Text =
                failed.ToString();

            lblAveragePercentage.Text =
                averagePercentage.ToString(
                    "0.00")
                +
                " %";

            pnlSummary.Visible =
                true;
        }

        //-------------------------------------------------------
        // RESULT GRID
        //-------------------------------------------------------

        private void BindResultGrid(
            DateTime fromDate,
            DateTime toDate)
        {
            string query =
                GetResultQuery();

            AddResultFilters(
                ref query);

            query +=
                " ORDER BY TD.DateFrom DESC, TM.TrainingID DESC, CASE WHEN TM.TestType='Pre' THEN 1 WHEN TM.TestType='Post' THEN 2 ELSE 3 END, TM.TestTitle, TR.EmpID, TR.AttemptNo DESC";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            gvResult.DataSource =
                dt;

            gvResult.DataBind();

            btnExportResult.Enabled =
                dt.Rows.Count
                >
                0;
        }

        //-------------------------------------------------------
        // RESULT QUERY
        //-------------------------------------------------------

        private string GetResultQuery()
        {
            string query =
                "SELECT TR.ResultID, TR.TestID, TM.TrainingID, ISNULL(CM.CourseName,'') AS CourseName, ISNULL(TD.Batch,'') AS Batch, TM.TestType, TM.TestTitle, TR.EmpID, ISNULL(EBM.EmpName,TME.TraineeName) AS TraineeName, TR.AttemptNo, TR.TotalQuestions, TR.AttemptedQuestions, TR.CorrectAnswers, TR.WrongAnswers, TR.TotalMarks, TR.ObtainedMarks, TR.Percentage, TR.ResultStatus, TR.RankNo, TR.TimeTaken, TR.SubmittedOn, TR.IsFinalAttempt FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TR.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TR.EmpID WHERE TM.TrainerID=@TrainerID";

            return query;
        }

        //-------------------------------------------------------
        // ADD FILTERS
        //-------------------------------------------------------

        private void AddResultFilters(
            ref string query)
        {
            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND TM.TrainingID=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTestType.SelectedValue)
            )
            {
                query +=
                    " AND TM.TestType=@TestType";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTest.SelectedValue)
            )
            {
                query +=
                    " AND TM.TestID=@TestID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtTrainee.Text)
            )
            {
                query +=
                    " AND (TR.EmpID LIKE @Trainee OR EBM.EmpName LIKE @Trainee OR TME.TraineeName LIKE @Trainee)";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlResultStatus.SelectedValue)
            )
            {
                if
                (
                    ddlResultStatus.SelectedValue
                    ==
                    "PASS"
                )
                {
                    query +=
                        " AND UPPER(ISNULL(TR.ResultStatus,'')) IN ('PASS','PASSED')";
                }
                else if
                (
                    ddlResultStatus.SelectedValue
                    ==
                    "FAIL"
                )
                {
                    query +=
                        " AND UPPER(ISNULL(TR.ResultStatus,'')) IN ('FAIL','FAILED')";
                }
            }

            if
            (
                ddlAttempt.SelectedValue
                ==
                "Final"
            )
            {
                query +=
                    " AND TR.IsFinalAttempt=1";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtBatch.Text)
            )
            {
                query +=
                    " AND TD.Batch LIKE @Batch";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtFromDate.Text)
            )
            {
                query +=
                    " AND TR.SubmittedOn>=@FromDate";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                query +=
                    " AND TR.SubmittedOn<DATEADD(DAY,1,@ToDate)";
            }
        }

        //-------------------------------------------------------
        // PARAMETERS
        //-------------------------------------------------------

        private SqlParameter[] GetFilterParameters(
            DateTime fromDate,
            DateTime toDate)
        {
            string trainerID =
                GetTrainerID();

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerID",
                    trainerID),

                new SqlParameter(
                    "@TrainingID",
                    String.IsNullOrWhiteSpace(
                        ddlTraining.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTraining.SelectedValue),

                new SqlParameter(
                    "@TestType",
                    String.IsNullOrWhiteSpace(
                        ddlTestType.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTestType.SelectedValue),

                new SqlParameter(
                    "@TestID",
                    String.IsNullOrWhiteSpace(
                        ddlTest.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTest.SelectedValue),

                new SqlParameter(
                    "@Trainee",
                    "%"
                    +
                    txtTrainee.Text.Trim()
                    +
                    "%"),

                new SqlParameter(
                    "@Batch",
                    "%"
                    +
                    txtBatch.Text.Trim()
                    +
                    "%"),

                new SqlParameter(
                    "@FromDate",
                    String.IsNullOrWhiteSpace(
                        txtFromDate.Text)
                    ? (object)DBNull.Value
                    : fromDate),

                new SqlParameter(
                    "@ToDate",
                    String.IsNullOrWhiteSpace(
                        txtToDate.Text)
                    ? (object)DBNull.Value
                    : toDate)
            };

            return param;
        }

        //-------------------------------------------------------
        // COMPARISON
        //-------------------------------------------------------

        private void BindComparison(
            DateTime fromDate,
            DateTime toDate)
        {
            DataTable dt =
                GetComparisonData(
                    fromDate,
                    toDate);

            gvComparison.DataSource =
                dt;

            gvComparison.DataBind();

            btnExportComparison.Enabled =
                dt.Rows.Count
                >
                0;
        }

        //-------------------------------------------------------
        // GET COMPARISON DATA
        //-------------------------------------------------------

        private DataTable GetComparisonData(
            DateTime fromDate,
            DateTime toDate)
        {
            string query =
                "SELECT X.TrainingID, X.CourseName, X.Batch, X.EmpID, X.TraineeName, X.PrePercentage, X.PostPercentage, CASE WHEN X.PrePercentage IS NOT NULL AND X.PostPercentage IS NOT NULL THEN X.PostPercentage-X.PrePercentage ELSE NULL END AS Improvement FROM (SELECT TM.TrainingID, ISNULL(CM.CourseName,'') AS CourseName, ISNULL(TD.Batch,'') AS Batch, TR.EmpID, ISNULL(EBM.EmpName,TME.TraineeName) AS TraineeName, AVG(CASE WHEN TM.TestType='Pre' THEN CAST(TR.Percentage AS DECIMAL(10,2)) END) AS PrePercentage, AVG(CASE WHEN TM.TestType='Post' THEN CAST(TR.Percentage AS DECIMAL(10,2)) END) AS PostPercentage FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TR.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TR.EmpID WHERE TM.TrainerID=@TrainerID AND TR.IsFinalAttempt=1 AND TM.TestType IN ('Pre','Post')";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND TM.TrainingID=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtBatch.Text)
            )
            {
                query +=
                    " AND TD.Batch LIKE @Batch";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtTrainee.Text)
            )
            {
                query +=
                    " AND (TR.EmpID LIKE @Trainee OR EBM.EmpName LIKE @Trainee OR TME.TraineeName LIKE @Trainee)";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtFromDate.Text)
            )
            {
                query +=
                    " AND TR.SubmittedOn>=@FromDate";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                query +=
                    " AND TR.SubmittedOn<DATEADD(DAY,1,@ToDate)";
            }

            query +=
                " GROUP BY TM.TrainingID, CM.CourseName, TD.Batch, TR.EmpID, EBM.EmpName, TME.TraineeName) X WHERE X.PrePercentage IS NOT NULL OR X.PostPercentage IS NOT NULL ORDER BY X.TrainingID DESC, X.TraineeName, X.EmpID";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            return dt;
        }

        //-------------------------------------------------------
        // SEARCH
        //-------------------------------------------------------

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            LoadReport();
        }

        //-------------------------------------------------------
        // RESET
        //-------------------------------------------------------

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            ddlTraining.SelectedIndex =
                0;

            ddlTestType.SelectedIndex =
                0;

            BindTest();

            txtTrainee.Text =
                "";

            ddlResultStatus.SelectedIndex =
                0;

            ddlAttempt.SelectedValue =
                "Final";

            txtBatch.Text =
                "";

            txtFromDate.Text =
                "";

            txtToDate.Text =
                "";

            lblMessage.Text =
                "";

            LoadReport();
        }

        //-------------------------------------------------------
        // VALIDATE DATES
        //-------------------------------------------------------

        private bool ValidateDates(
            out DateTime fromDate,
            out DateTime toDate)
        {
            fromDate =
                DateTime.MinValue;

            toDate =
                DateTime.MinValue;

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtFromDate.Text)
            )
            {
                if
                (
                    !DateTime.TryParseExact(
                        txtFromDate.Text.Trim(),
                        "dd-MM-yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fromDate)
                )
                {
                    ShowError(
                        "Submitted From date must be in dd-MM-yyyy format.");

                    return false;
                }
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                if
                (
                    !DateTime.TryParseExact(
                        txtToDate.Text.Trim(),
                        "dd-MM-yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out toDate)
                )
                {
                    ShowError(
                        "Submitted To date must be in dd-MM-yyyy format.");

                    return false;
                }
            }

            if
            (
                fromDate
                !=
                DateTime.MinValue
                &&
                toDate
                !=
                DateTime.MinValue
                &&
                fromDate
                >
                toDate
            )
            {
                ShowError(
                    "Submitted From date cannot be greater than Submitted To date.");

                return false;
            }

            return true;
        }

        //-------------------------------------------------------
        // RESULT ROW
        //-------------------------------------------------------

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

            SetResultStyle(
                lblResult);
        }

        //-------------------------------------------------------
        // COMPARISON ROW
        //-------------------------------------------------------

        protected void gvComparison_RowDataBound(
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

            Label lblImprovement =
                e.Row.FindControl(
                    "lblImprovement")
                as Label;

            if
            (
                lblImprovement
                ==
                null
            )
            {
                return;
            }

            DataRowView drv =
                e.Row.DataItem
                as DataRowView;

            if
            (
                drv
                ==
                null
                ||
                drv["Improvement"]
                ==
                DBNull.Value
            )
            {
                return;
            }

            decimal improvement =
                Convert.ToDecimal(
                    drv["Improvement"]);

            if (improvement > 0)
            {
                lblImprovement.CssClass =
                    "improvement-positive";
            }
            else if (improvement < 0)
            {
                lblImprovement.CssClass =
                    "improvement-negative";
            }
        }

        //-------------------------------------------------------
        // RESULT STYLE
        //-------------------------------------------------------

        private void SetResultStyle(
            Label label)
        {
            if
            (
                label
                ==
                null
            )
            {
                return;
            }

            string result =
                label.Text.Trim();

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
                label.CssClass =
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
                label.CssClass =
                    "result-fail";
            }
        }

        //-------------------------------------------------------
        // FORMAT TIME
        //-------------------------------------------------------

        protected string FormatTimeTaken(
            object value)
        {
            if
            (
                value
                ==
                null
                ||
                value
                ==
                DBNull.Value
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
                if (seconds < 0)
                {
                    seconds =
                        0;
                }

                TimeSpan time =
                    TimeSpan.FromSeconds(
                        seconds);

                if
                (
                    time.TotalHours
                    >=
                    1
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

        //-------------------------------------------------------
        // FORMAT PERCENTAGE
        //-------------------------------------------------------

        protected string FormatPercentage(
            object value)
        {
            if
            (
                value
                ==
                null
                ||
                value
                ==
                DBNull.Value
            )
            {
                return "-";
            }

            decimal percentage =
                Convert.ToDecimal(
                    value);

            return
                percentage.ToString(
                    "0.00")
                +
                " %";
        }

        //-------------------------------------------------------
        // FORMAT IMPROVEMENT
        //-------------------------------------------------------

        protected string FormatImprovement(
            object value)
        {
            if
            (
                value
                ==
                null
                ||
                value
                ==
                DBNull.Value
            )
            {
                return "-";
            }

            decimal improvement =
                Convert.ToDecimal(
                    value);

            if (improvement > 0)
            {
                return
                    "+"
                    +
                    improvement.ToString(
                        "0.00")
                    +
                    " %";
            }

            return
                improvement.ToString(
                    "0.00")
                +
                " %";
        }

        //-------------------------------------------------------
        // EXPORT RESULT
        //-------------------------------------------------------

        protected void btnExportResult_Click(
            object sender,
            EventArgs e)
        {
            DateTime fromDate;

            DateTime toDate;

            if
            (
                !ValidateDates(
                    out fromDate,
                    out toDate)
            )
            {
                return;
            }

            string query =
                "SELECT TM.TrainingID AS [Training ID], ISNULL(CM.CourseName,'') AS [Course], ISNULL(TD.Batch,'') AS [Batch], CASE WHEN TM.TestType='Pre' THEN 'Pre Training' WHEN TM.TestType='Post' THEN 'Post Training' ELSE TM.TestType END AS [Exam], TM.TestTitle AS [Test Title], TR.EmpID AS [Trainee ID], ISNULL(EBM.EmpName,TME.TraineeName) AS [Trainee Name], TR.AttemptNo AS [Attempt], TR.TotalQuestions AS [Total Questions], TR.AttemptedQuestions AS [Attempted], TR.CorrectAnswers AS [Correct], TR.WrongAnswers AS [Wrong], TR.TotalMarks AS [Total Marks], TR.ObtainedMarks AS [Obtained Marks], TR.Percentage AS [Percentage], TR.ResultStatus AS [Result], TR.RankNo AS [Rank], TR.TimeTaken AS [Time Taken], TR.SubmittedOn AS [Submitted On], CASE WHEN TR.IsFinalAttempt=1 THEN 'Yes' ELSE 'No' END AS [Final Attempt] FROM TestResult TR INNER JOIN TestMaster TM ON TR.TestID=TM.TestID INNER JOIN TrainingDetails TD ON TM.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TR.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TR.EmpID WHERE TM.TrainerID=@TrainerID";

            AddResultFilters(
                ref query);

            query +=
                " ORDER BY TD.DateFrom DESC, TM.TrainingID DESC, TM.TestType, TM.TestTitle, TR.EmpID, TR.AttemptNo DESC";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            ExportDataTable(
                dt,
                "TrainerExamResult");
        }

        //-------------------------------------------------------
        // EXPORT COMPARISON
        //-------------------------------------------------------

        protected void btnExportComparison_Click(
            object sender,
            EventArgs e)
        {
            DateTime fromDate;

            DateTime toDate;

            if
            (
                !ValidateDates(
                    out fromDate,
                    out toDate)
            )
            {
                return;
            }

            DataTable dt =
                GetComparisonData(
                    fromDate,
                    toDate);

            if
            (
                dt
                !=
                null
            )
            {
                if
                (
                    dt.Columns.Contains(
                        "TrainingID")
                )
                {
                    dt.Columns[
                        "TrainingID"]
                        .ColumnName =
                        "Training ID";
                }

                if
                (
                    dt.Columns.Contains(
                        "CourseName")
                )
                {
                    dt.Columns[
                        "CourseName"]
                        .ColumnName =
                        "Course";
                }

                if
                (
                    dt.Columns.Contains(
                        "EmpID")
                )
                {
                    dt.Columns[
                        "EmpID"]
                        .ColumnName =
                        "Trainee ID";
                }

                if
                (
                    dt.Columns.Contains(
                        "TraineeName")
                )
                {
                    dt.Columns[
                        "TraineeName"]
                        .ColumnName =
                        "Trainee Name";
                }

                if
                (
                    dt.Columns.Contains(
                        "PrePercentage")
                )
                {
                    dt.Columns[
                        "PrePercentage"]
                        .ColumnName =
                        "Pre Percentage";
                }

                if
                (
                    dt.Columns.Contains(
                        "PostPercentage")
                )
                {
                    dt.Columns[
                        "PostPercentage"]
                        .ColumnName =
                        "Post Percentage";
                }
            }

            ExportDataTable(
                dt,
                "TrainerPrePostComparison");
        }

        //-------------------------------------------------------
        // EXPORT DATATABLE
        //-------------------------------------------------------

        private void ExportDataTable(
            DataTable dt,
            string fileName)
        {
            if
            (
                dt
                ==
                null
                ||
                dt.Rows.Count
                ==
                0
            )
            {
                ShowError(
                    "No data available for export.");

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
                "attachment;filename="
                +
                fileName
                +
                "_"
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

        //-------------------------------------------------------
        // VERIFY RENDERING
        //-------------------------------------------------------

        public override void VerifyRenderingInServerForm(
            Control control)
        {
        }

        //-------------------------------------------------------
        // CLEAR REPORT
        //-------------------------------------------------------

        private void ClearReport()
        {
            pnlSummary.Visible =
                false;

            gvResult.DataSource =
                null;

            gvResult.DataBind();

            gvComparison.DataSource =
                null;

            gvComparison.DataBind();

            btnExportResult.Enabled =
                false;

            btnExportComparison.Enabled =
                false;
        }

        //-------------------------------------------------------
        // ERROR
        //-------------------------------------------------------

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