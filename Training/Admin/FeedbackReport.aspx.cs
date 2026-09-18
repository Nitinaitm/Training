using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FeedbackReport :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        //-------------------------------------------------------
        // Page Load
        //-------------------------------------------------------

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                BindTraining();

                BindCourse();

                LoadReport();
            }
        }

        //-------------------------------------------------------
        // Bind Training
        //-------------------------------------------------------

        private void BindTraining()
        {
            string query =
                "SELECT TrainingID, TrainingID + ' | ' + ISNULL(TrainingLocation,'') + ' | Batch ' + ISNULL(Batch,'') AS TrainingName FROM TrainingDetails ORDER BY DateFrom DESC, TrainingID DESC";

            DataTable dt =
                objDB.GetDataTable(
                    query);

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
        // Bind Course
        //-------------------------------------------------------

        private void BindCourse()
        {
            string query =
                "SELECT CourseID, CourseName FROM CourseMaster ORDER BY CourseName";

            DataTable dt =
                objDB.GetDataTable(
                    query);

            ddlCourse.DataSource =
                dt;

            ddlCourse.DataTextField =
                "CourseName";

            ddlCourse.DataValueField =
                "CourseID";

            ddlCourse.DataBind();

            ddlCourse.Items.Insert(
                0,
                new ListItem(
                    "-- All Course --",
                    ""));
        }

        //-------------------------------------------------------
        // Load Complete Report
        //-------------------------------------------------------

        private void LoadReport()
        {
            lblMessage.Text =
                "";

            pnlFeedbackDetail.Visible =
                false;

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

            BindQuestionSummary(
                fromDate,
                toDate);

            BindTrainerSummary(
                fromDate,
                toDate);

            BindTraineeFeedback(
                fromDate,
                toDate);
        }

        //-------------------------------------------------------
        // Summary
        //-------------------------------------------------------

        private void BindSummary(
     DateTime fromDate,
     DateTime toDate)
        {
            int assigned =
                GetAssignedTraineeCount();

            int submitted =
                GetSubmittedFeedbackCount(
                    fromDate,
                    toDate,
                    true);

            int totalSubmitted =
                GetSubmittedFeedbackCount(
                    fromDate,
                    toDate,
                    false);

            int pending =
                assigned
                -
                totalSubmitted;

            if (pending < 0)
            {
                pending =
                    0;
            }

            lblAssignedTrainees.Text =
                assigned.ToString();

            lblFeedbackSubmitted.Text =
                submitted.ToString();

            lblFeedbackPending.Text =
                pending.ToString();

            lblAverageRating.Text =
                GetOverallAverageRating(
                    fromDate,
                    toDate);

            pnlSummary.Visible =
                true;
        }

        private int GetAssignedTraineeCount()
        {
            string query =
                "SELECT COUNT(*) FROM (SELECT TA.TrainingID, TA.EmpID FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID WHERE 1=1";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND TA.TrainingID=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlCourse.SelectedValue)
            )
            {
                query +=
                    " AND TD.CourseID=@CourseID";
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

            query +=
                " GROUP BY TA.TrainingID, TA.EmpID) X";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            String.IsNullOrWhiteSpace(
                ddlTraining.SelectedValue)
            ? (object)DBNull.Value
            : ddlTraining.SelectedValue),

        new SqlParameter(
            "@CourseID",
            String.IsNullOrWhiteSpace(
                ddlCourse.SelectedValue)
            ? (object)DBNull.Value
            : ddlCourse.SelectedValue),

        new SqlParameter(
            "@Batch",
            "%"
            +
            txtBatch.Text.Trim()
            +
            "%")
    };

            object result =
                objDB.ExecuteScalar(
                    query,
                    param);

            if
            (
                result
                ==
                null
                ||
                result
                ==
                DBNull.Value
            )
            {
                return 0;
            }

            return
                Convert.ToInt32(
                    result);
        }

        private int GetSubmittedFeedbackCount(
        DateTime fromDate,
        DateTime toDate,
        bool applyDateFilter)
        {
            string query =
                "SELECT COUNT(*) FROM (SELECT F.TrainingID, F.EmpID FROM Feedback F INNER JOIN TrainingDetails TD ON F.TrainingID=TD.TrainingID WHERE F.Submitted=1";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND F.TrainingID=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlCourse.SelectedValue)
            )
            {
                query +=
                    " AND TD.CourseID=@CourseID";
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
                applyDateFilter
                &&
                !String.IsNullOrWhiteSpace(
                    txtFromDate.Text)
            )
            {
                query +=
                    " AND F.SubmittedOn>=@FromDate";
            }

            if
            (
                applyDateFilter
                &&
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                query +=
                    " AND F.SubmittedOn<DATEADD(DAY,1,@ToDate)";
            }

            query +=
                " GROUP BY F.TrainingID, F.EmpID) X";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            String.IsNullOrWhiteSpace(
                ddlTraining.SelectedValue)
            ? (object)DBNull.Value
            : ddlTraining.SelectedValue),

        new SqlParameter(
            "@CourseID",
            String.IsNullOrWhiteSpace(
                ddlCourse.SelectedValue)
            ? (object)DBNull.Value
            : ddlCourse.SelectedValue),

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

            object result =
                objDB.ExecuteScalar(
                    query,
                    param);

            if
            (
                result
                ==
                null
                ||
                result
                ==
                DBNull.Value
            )
            {
                return 0;
            }

            return
                Convert.ToInt32(
                    result);
        }

        //-------------------------------------------------------
        // Overall Average Rating
        //-------------------------------------------------------

        private string GetOverallAverageRating(
            DateTime fromDate,
            DateTime toDate)
        {
            string query =
                "SELECT AVG(CAST(FD.Rating AS DECIMAL(10,2))) FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID WHERE F.Submitted=1 AND FD.Rating IS NOT NULL";

            AddFeedbackFilters(
                ref query,
                true);

            object result =
                objDB.ExecuteScalar(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            if
            (
                result
                ==
                null
                ||
                result
                ==
                DBNull.Value
            )
            {
                return
                    "0.00 / 5";
            }

            decimal rating =
                Convert.ToDecimal(
                    result);

            return
                rating.ToString(
                    "0.00")
                +
                " / 5";
        }

        //-------------------------------------------------------
        // Question Summary
        //-------------------------------------------------------

        private void BindQuestionSummary(
            DateTime fromDate,
            DateTime toDate)
        {
            string query =
                "SELECT FD.CategoryID, ISNULL(FCM.CategoryName,FD.CategoryID) AS CategoryName, FD.QuestionID, FQM.QuestionText, FD.AnswerType, COUNT(FD.FeedbackDetailID) AS TotalResponses, CASE WHEN MAX(CASE WHEN FD.Rating IS NOT NULL THEN 1 ELSE 0 END)=1 THEN CAST(AVG(CAST(FD.Rating AS DECIMAL(10,2))) AS DECIMAL(10,2)) ELSE NULL END AS AverageRating FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN FeedbackQuestionMaster FQM ON FD.QuestionID=FQM.QuestionID LEFT JOIN FeedbackCategoryMaster FCM ON FD.CategoryID=FCM.CategoryID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID WHERE F.Submitted=1";

            AddFeedbackFilters(
                ref query,
                true);

            query +=
                " GROUP BY FD.CategoryID, FCM.CategoryName, FCM.DisplayOrder, FD.QuestionID, FQM.QuestionText, FQM.DisplayOrder, FD.AnswerType ORDER BY ISNULL(FCM.DisplayOrder,9999), FQM.DisplayOrder, FQM.QuestionText";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            FormatAverageRating(
                dt);

            gvQuestionSummary.DataSource =
                dt;

            gvQuestionSummary.DataBind();
        }

        //-------------------------------------------------------
        // Trainer Summary
        //-------------------------------------------------------

        private void BindTrainerSummary(
     DateTime fromDate,
     DateTime toDate)
        {
            string query =
                "SELECT FD.TrainerID, FD.TrainerType, CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(EBM.EmpName,ISNULL(TM.EmpID,FD.TrainerID)) ELSE ISNULL(TM.NameExternal,ISNULL(TM.EmpIDExternal,FD.TrainerID)) END AS TrainerName, COUNT(DISTINCT FD.FeedbackID) AS TotalResponses, CAST(AVG(CAST(FD.Rating AS DECIMAL(10,2))) AS DECIMAL(10,2)) AS AverageRating FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID LEFT JOIN TrainerMaster TM ON TM.TrainerID=FD.TrainerID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TM.EmpID WHERE F.Submitted=1 AND ISNULL(FD.TrainerID,'')<>'' AND FD.Rating IS NOT NULL";

            AddFeedbackFilters(
                ref query,
                true);

            query +=
                " GROUP BY FD.TrainerID, FD.TrainerType, TM.TrainerType, TM.EmpID, TM.EmpIDExternal, TM.NameExternal, EBM.EmpName ORDER BY TrainerName";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            FormatAverageRating(
                dt);

            gvTrainerSummary.DataSource =
                dt;

            gvTrainerSummary.DataBind();
        }
        //-------------------------------------------------------
        // Trainee Feedback
        //-------------------------------------------------------

        private void BindTraineeFeedback(
            DateTime fromDate,
            DateTime toDate)
        {
            string query =
                "SELECT F.FeedbackID, F.TrainingID, F.EmpID, ISNULL(EBM.EmpName,TME.TraineeName) AS TraineeName, CM.CourseName, TD.Batch, F.SubmittedOn FROM Feedback F INNER JOIN TrainingDetails TD ON F.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=F.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=F.EmpID WHERE F.Submitted=1";

            AddFeedbackFilters(
                ref query,
                false);

            query +=
                " ORDER BY F.SubmittedOn DESC";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetFilterParameters(
                        fromDate,
                        toDate));

            gvTraineeFeedback.DataSource =
                dt;

            gvTraineeFeedback.DataBind();
        }

        //-------------------------------------------------------
        // Add Feedback Filters
        //-------------------------------------------------------

        private void AddFeedbackFilters(
            ref string query,
            bool feedbackDetail)
        {
            string trainingColumn =
                feedbackDetail
                ? "FD.TrainingID"
                : "F.TrainingID";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND "
                    +
                    trainingColumn
                    +
                    "=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlCourse.SelectedValue)
            )
            {
                query +=
                    " AND TD.CourseID=@CourseID";
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
                    " AND F.SubmittedOn>=@FromDate";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                query +=
                    " AND F.SubmittedOn<DATEADD(DAY,1,@ToDate)";
            }
        }

        //-------------------------------------------------------
        // Filter Parameters
        //-------------------------------------------------------

        private SqlParameter[] GetFilterParameters(
            DateTime fromDate,
            DateTime toDate)
        {
            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    String.IsNullOrWhiteSpace(
                        ddlTraining.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTraining.SelectedValue),

                new SqlParameter(
                    "@CourseID",
                    String.IsNullOrWhiteSpace(
                        ddlCourse.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlCourse.SelectedValue),

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
        // Validate Dates
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
        // Search
        //-------------------------------------------------------

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            LoadReport();
        }

        //-------------------------------------------------------
        // Reset
        //-------------------------------------------------------

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            ddlTraining.SelectedIndex =
                0;

            ddlCourse.SelectedIndex =
                0;

            txtBatch.Text =
                "";

            txtFromDate.Text =
                "";

            txtToDate.Text =
                "";

            lblMessage.Text =
                "";

            pnlFeedbackDetail.Visible =
                false;

            LoadReport();
        }

        //-------------------------------------------------------
        // Trainee Feedback Row Command
        //-------------------------------------------------------

        protected void gvTraineeFeedback_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if
            (
                e.CommandName
                !=
                "ViewFeedback"
            )
            {
                return;
            }

            string feedbackID =
                Convert.ToString(
                    e.CommandArgument);

            LoadFeedbackDetail(
                feedbackID);
        }

        //-------------------------------------------------------
        // Load Feedback Detail
        //-------------------------------------------------------

        private void LoadFeedbackDetail(
       string feedbackID)
        {
            if
            (
                String.IsNullOrWhiteSpace(
                    feedbackID)
            )
            {
                ShowError(
                    "Invalid Feedback ID.");

                return;
            }

            LoadFeedbackHeader(
                feedbackID);

            string query =
                "SELECT ISNULL(FCM.CategoryName,FD.CategoryID) AS CategoryName, FQM.QuestionText, CASE WHEN ISNULL(FD.TrainerID,'')='' THEN '' WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(EBM.EmpName,ISNULL(TM.EmpID,FD.TrainerID)) ELSE ISNULL(TM.NameExternal,ISNULL(TM.EmpIDExternal,FD.TrainerID)) END AS TrainerName, FD.AnswerType, CASE WHEN FD.Rating IS NOT NULL THEN CONVERT(VARCHAR(20),FD.Rating) ELSE ISNULL(FD.Answer,'') END AS Response FROM FeedbackDetail FD INNER JOIN FeedbackQuestionMaster FQM ON FD.QuestionID=FQM.QuestionID LEFT JOIN FeedbackCategoryMaster FCM ON FD.CategoryID=FCM.CategoryID LEFT JOIN TrainerMaster TM ON TM.TrainerID=FD.TrainerID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TM.EmpID WHERE FD.FeedbackID=@FeedbackID ORDER BY ISNULL(FCM.DisplayOrder,9999), FQM.DisplayOrder";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@FeedbackID",
            feedbackID)
    };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            gvFeedbackDetail.DataSource =
                dt;

            gvFeedbackDetail.DataBind();

            pnlFeedbackDetail.Visible =
                true;
        }

        //-------------------------------------------------------
        // Load Feedback Header
        //-------------------------------------------------------

        private void LoadFeedbackHeader(
            string feedbackID)
        {
            string query =
                "SELECT F.TrainingID, F.EmpID, ISNULL(EBM.EmpName,TME.TraineeName) AS TraineeName, F.SubmittedOn FROM Feedback F LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=F.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=F.EmpID WHERE F.FeedbackID=@FeedbackID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@FeedbackID",
                    feedbackID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                lblDetailTraining.Text =
                    "";

                lblDetailTrainee.Text =
                    "";

                lblDetailSubmittedOn.Text =
                    "";

                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblDetailTraining.Text =
                Convert.ToString(
                    dr["TrainingID"]);

            lblDetailTrainee.Text =
                Convert.ToString(
                    dr["EmpID"])
                +
                " - "
                +
                Convert.ToString(
                    dr["TraineeName"]);

            if
            (
                dr["SubmittedOn"]
                !=
                DBNull.Value
            )
            {
                lblDetailSubmittedOn.Text =
                    Convert.ToDateTime(
                        dr["SubmittedOn"])
                    .ToString(
                        "dd-MM-yyyy hh:mm tt");
            }
            else
            {
                lblDetailSubmittedOn.Text =
                    "";
            }
        }

        //-------------------------------------------------------
        // Trainer Row Command
        //-------------------------------------------------------

        protected void gvTrainerSummary_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if
            (
                e.CommandName
                !=
                "ViewTrainer"
            )
            {
                return;
            }

            string trainerID =
                Convert.ToString(
                    e.CommandArgument);

            if
            (
                String.IsNullOrWhiteSpace(
                    trainerID)
            )
            {
                return;
            }

            Session["FeedbackTrainerID"] =
                trainerID;

            Response.Redirect(
                "TrainerFeedbackDetail.aspx");
        }

        //-------------------------------------------------------
        // Close Detail
        //-------------------------------------------------------

        protected void btnCloseDetail_Click(
            object sender,
            EventArgs e)
        {
            pnlFeedbackDetail.Visible =
                false;
        }

        //-------------------------------------------------------
        // Format Average Rating
        //-------------------------------------------------------

        private void FormatAverageRating(
            DataTable dt)
        {
            if
            (
                !dt.Columns.Contains(
                    "AverageRating")
            )
            {
                return;
            }

            foreach
            (
                DataRow dr
                in
                dt.Rows
            )
            {
                if
                (
                    dr["AverageRating"]
                    ==
                    DBNull.Value
                )
                {
                    dr["AverageRating"] =
                        DBNull.Value;

                    continue;
                }

                decimal rating =
                    Convert.ToDecimal(
                        dr["AverageRating"]);

                dr["AverageRating"] =
                    rating;
            }
        }

        //-------------------------------------------------------
        // Export Question
        //-------------------------------------------------------

        protected void btnExportQuestion_Click(
            object sender,
            EventArgs e)
        {
            ExportGrid(
                gvQuestionSummary,
                "QuestionWiseFeedback");
        }

        //-------------------------------------------------------
        // Export Trainer
        //-------------------------------------------------------

        protected void btnExportTrainer_Click(
            object sender,
            EventArgs e)
        {
            ExportGrid(
                gvTrainerSummary,
                "TrainerWiseFeedback");
        }

        //-------------------------------------------------------
        // Export Trainee
        //-------------------------------------------------------

        protected void btnExportTrainee_Click(
            object sender,
            EventArgs e)
        {
            ExportGrid(
                gvTraineeFeedback,
                "TraineeWiseFeedback");
        }

        //-------------------------------------------------------
        // Export Grid
        //-------------------------------------------------------

        private void ExportGrid(
            GridView grid,
            string fileName)
        {
            if
            (
                grid.Rows.Count
                ==
                0
            )
            {
                ShowError(
                    "No data available for export.");

                return;
            }

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

            grid.AllowPaging =
                false;

            grid.RenderControl(
                hw);

            Response.Output.Write(
                sw.ToString());

            Response.Flush();

            HttpContext.Current
                .ApplicationInstance
                .CompleteRequest();
        }

        //-------------------------------------------------------
        // Verify Rendering
        //-------------------------------------------------------

        public override void VerifyRenderingInServerForm(
            Control control)
        {
        }

        //-------------------------------------------------------
        // Clear Report
        //-------------------------------------------------------

        private void ClearReport()
        {
            pnlSummary.Visible =
                false;

            gvQuestionSummary.DataSource =
                null;

            gvQuestionSummary.DataBind();

            gvTrainerSummary.DataSource =
                null;

            gvTrainerSummary.DataBind();

            gvTraineeFeedback.DataSource =
                null;

            gvTraineeFeedback.DataBind();

            pnlFeedbackDetail.Visible =
                false;
        }

        //-------------------------------------------------------
        // Error
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