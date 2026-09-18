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

                LoadReport();
            }
        }

        //-------------------------------------------------------
        // Logged In Trainer ID
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
        // Load Trainer Details
        //-------------------------------------------------------

        private void LoadTrainerDetails()
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT TM.TrainerID, TM.TrainerType, CASE WHEN ISNULL(TM.TrainerType,'')='Internal' THEN ISNULL(EBM.EmpName,ISNULL(TM.EmpID,TM.TrainerID)) ELSE ISNULL(TM.NameExternal,ISNULL(TM.EmpIDExternal,TM.TrainerID)) END AS TrainerName FROM TrainerMaster TM LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TM.EmpID WHERE TM.TrainerID=@TrainerID";

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

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                lblTrainerID.Text =
                    trainerID;

                lblTrainerName.Text =
                    "";

                lblTrainerType.Text =
                    "";

                ShowError(
                    "Trainer details not found.");

                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblTrainerID.Text =
                Convert.ToString(
                    dr["TrainerID"]);

            lblTrainerName.Text =
                Convert.ToString(
                    dr["TrainerName"]);

            lblTrainerType.Text =
                Convert.ToString(
                    dr["TrainerType"]);
        }

        //-------------------------------------------------------
        // Bind Training
        //-------------------------------------------------------

        private void BindTraining()
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT DISTINCT TD.TrainingID, TD.TrainingID + ' | ' + ISNULL(CM.CourseName,'') + ' | Batch ' + ISNULL(TD.Batch,'') AS TrainingName, TD.DateFrom FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE FD.TrainerID=@TrainerID AND F.Submitted=1 ORDER BY TD.DateFrom DESC, TD.TrainingID DESC";

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
        // Load Report
        //-------------------------------------------------------

        private void LoadReport()
        {
            lblMessage.Text =
                "";

            pnlQuestionDetail.Visible =
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

            BindTrainingSummary(
                fromDate,
                toDate);
        }

        //-------------------------------------------------------
        // Bind Summary
        //-------------------------------------------------------

        private void BindSummary(
            DateTime fromDate,
            DateTime toDate)
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT COUNT(DISTINCT FD.TrainingID) AS TrainingCount, COUNT(DISTINCT FD.FeedbackID) AS ResponseCount, CAST(AVG(CAST(FD.Rating AS DECIMAL(10,2))) AS DECIMAL(10,2)) AS AverageRating FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID WHERE FD.TrainerID=@TrainerID AND F.Submitted=1";

            AddFilters(
                ref query);

            SqlParameter[] param =
                GetParameters(
                    trainerID,
                    fromDate,
                    toDate);

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            int trainingCount =
                0;

            int responseCount =
                0;

            decimal averageRating =
                0;

            if
            (
                dt.Rows.Count
                >
                0
            )
            {
                if
                (
                    dt.Rows[0]["TrainingCount"]
                    !=
                    DBNull.Value
                )
                {
                    trainingCount =
                        Convert.ToInt32(
                            dt.Rows[0][
                                "TrainingCount"]);
                }

                if
                (
                    dt.Rows[0]["ResponseCount"]
                    !=
                    DBNull.Value
                )
                {
                    responseCount =
                        Convert.ToInt32(
                            dt.Rows[0][
                                "ResponseCount"]);
                }

                if
                (
                    dt.Rows[0]["AverageRating"]
                    !=
                    DBNull.Value
                )
                {
                    averageRating =
                        Convert.ToDecimal(
                            dt.Rows[0][
                                "AverageRating"]);
                }
            }

            lblTrainingCount.Text =
                trainingCount.ToString();

            lblResponseCount.Text =
                responseCount.ToString();

            lblAverageRating.Text =
                averageRating.ToString(
                    "0.00")
                +
                " / 5";

            pnlSummary.Visible =
                true;
        }

        //-------------------------------------------------------
        // Training Wise Summary
        //-------------------------------------------------------

        private void BindTrainingSummary(
            DateTime fromDate,
            DateTime toDate)
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT FD.TrainingID, ISNULL(CM.CourseName,'') AS CourseName, ISNULL(TD.Batch,'') AS Batch, CASE WHEN TD.DateFrom IS NULL OR TD.DateTo IS NULL THEN '' WHEN CAST(TD.DateFrom AS DATE)=CAST(TD.DateTo AS DATE) THEN CONVERT(VARCHAR(10),TD.DateFrom,105) ELSE CONVERT(VARCHAR(10),TD.DateFrom,105) + ' to ' + CONVERT(VARCHAR(10),TD.DateTo,105) END AS TrainingDuration, COUNT(DISTINCT FD.FeedbackID) AS TotalResponses, CAST(AVG(CAST(FD.Rating AS DECIMAL(10,2))) AS DECIMAL(10,2)) AS AverageRating FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE FD.TrainerID=@TrainerID AND F.Submitted=1";

            AddFilters(
                ref query);

            query +=
                " GROUP BY FD.TrainingID, CM.CourseName, TD.Batch, TD.DateFrom, TD.DateTo ORDER BY TD.DateFrom DESC, FD.TrainingID DESC";

            SqlParameter[] param =
                GetParameters(
                    trainerID,
                    fromDate,
                    toDate);

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            gvTrainingSummary.DataSource =
                dt;

            gvTrainingSummary.DataBind();
        }

        //-------------------------------------------------------
        // Add Filters
        //-------------------------------------------------------

        private void AddFilters(
            ref string query)
        {
            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND FD.TrainingID=@TrainingID";
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
        // Parameters
        //-------------------------------------------------------

        private SqlParameter[] GetParameters(
            string trainerID,
            DateTime fromDate,
            DateTime toDate)
        {
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

            txtFromDate.Text =
                "";

            txtToDate.Text =
                "";

            lblMessage.Text =
                "";

            pnlQuestionDetail.Visible =
                false;

            LoadReport();
        }

        //-------------------------------------------------------
        // Training Row Command
        //-------------------------------------------------------

        protected void gvTrainingSummary_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if
            (
                e.CommandName
                !=
                "ViewTraining"
            )
            {
                return;
            }

            string trainingID =
                Convert.ToString(
                    e.CommandArgument);

            if
            (
                String.IsNullOrWhiteSpace(
                    trainingID)
            )
            {
                return;
            }

            LoadTrainingFeedbackDetail(
                trainingID);
        }

        //-------------------------------------------------------
        // Training Feedback Detail
        //-------------------------------------------------------

        private void LoadTrainingFeedbackDetail(
            string trainingID)
        {
            lblSelectedTraining.Text =
                trainingID;

            BindQuestionSummary(
                trainingID);

            BindComments(
                trainingID);

            pnlQuestionDetail.Visible =
                true;
        }

        //-------------------------------------------------------
        // Question Summary
        //-------------------------------------------------------

        private void BindQuestionSummary(
            string trainingID)
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT ISNULL(FCM.CategoryName,FD.CategoryID) AS CategoryName, FD.QuestionID, FQM.QuestionText, COUNT(DISTINCT FD.FeedbackID) AS TotalResponses, CAST(AVG(CAST(FD.Rating AS DECIMAL(10,2))) AS DECIMAL(10,2)) AS AverageRating FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN FeedbackQuestionMaster FQM ON FD.QuestionID=FQM.QuestionID LEFT JOIN FeedbackCategoryMaster FCM ON FD.CategoryID=FCM.CategoryID WHERE FD.TrainerID=@TrainerID AND FD.TrainingID=@TrainingID AND F.Submitted=1 AND FD.Rating IS NOT NULL GROUP BY FD.CategoryID, FCM.CategoryName, FCM.DisplayOrder, FD.QuestionID, FQM.QuestionText, FQM.DisplayOrder ORDER BY ISNULL(FCM.DisplayOrder,9999), FQM.DisplayOrder";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerID",
                    trainerID),

                new SqlParameter(
                    "@TrainingID",
                    trainingID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            gvQuestionSummary.DataSource =
                dt;

            gvQuestionSummary.DataBind();
        }

        //-------------------------------------------------------
        // Anonymous Comments
        //-------------------------------------------------------

        private void BindComments(
            string trainingID)
        {
            string trainerID =
                GetTrainerID();

            string query =
                "SELECT FQM.QuestionText, FD.Answer FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN FeedbackQuestionMaster FQM ON FD.QuestionID=FQM.QuestionID LEFT JOIN FeedbackCategoryMaster FCM ON FD.CategoryID=FCM.CategoryID WHERE FD.TrainerID=@TrainerID AND FD.TrainingID=@TrainingID AND F.Submitted=1 AND ISNULL(LTRIM(RTRIM(FD.Answer)),'')<>'' ORDER BY ISNULL(FCM.DisplayOrder,9999), FQM.DisplayOrder, FD.CreatedOn";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerID",
                    trainerID),

                new SqlParameter(
                    "@TrainingID",
                    trainingID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            rptComments.DataSource =
                dt;

            rptComments.DataBind();

            lblNoComments.Visible =
                dt.Rows.Count
                ==
                0;
        }

        //-------------------------------------------------------
        // Close Detail
        //-------------------------------------------------------

        protected void btnCloseDetail_Click(
            object sender,
            EventArgs e)
        {
            pnlQuestionDetail.Visible =
                false;
        }

        //-------------------------------------------------------
        // Export Training Summary
        //-------------------------------------------------------

        protected void btnExportTraining_Click(
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

            string trainerID =
                GetTrainerID();

            string query =
                "SELECT FD.TrainingID AS [Training ID], ISNULL(CM.CourseName,'') AS [Course], ISNULL(TD.Batch,'') AS [Batch], CONVERT(VARCHAR(10),TD.DateFrom,105) AS [Date From], CONVERT(VARCHAR(10),TD.DateTo,105) AS [Date To], COUNT(DISTINCT FD.FeedbackID) AS [Responses], CAST(AVG(CAST(FD.Rating AS DECIMAL(10,2))) AS DECIMAL(10,2)) AS [Average Rating] FROM FeedbackDetail FD INNER JOIN Feedback F ON FD.FeedbackID=F.FeedbackID INNER JOIN TrainingDetails TD ON FD.TrainingID=TD.TrainingID LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE FD.TrainerID=@TrainerID AND F.Submitted=1";

            AddFilters(
                ref query);

            query +=
                " GROUP BY FD.TrainingID, CM.CourseName, TD.Batch, TD.DateFrom, TD.DateTo ORDER BY TD.DateFrom DESC, FD.TrainingID DESC";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    GetParameters(
                        trainerID,
                        fromDate,
                        toDate));

            ExportDataTable(
                dt,
                "MyFeedbackReport");
        }

        //-------------------------------------------------------
        // Export DataTable
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

            pnlQuestionDetail.Visible =
                false;

            gvTrainingSummary.DataSource =
                null;

            gvTrainingSummary.DataBind();

            gvQuestionSummary.DataSource =
                null;

            gvQuestionSummary.DataBind();

            rptComments.DataSource =
                null;

            rptComments.DataBind();

            lblNoComments.Visible =
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