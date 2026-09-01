using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class Default : System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        string EmpID =            "";

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if
            (
                Session["EmpID"] == null
                &&
                Session["UserID"] == null
            )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                Session["EmpID"] != null
            )
            {
                EmpID =
                    Session["EmpID"]
                    .ToString();
            }
            else
            {
                EmpID =
                    Session["UserID"]
                    .ToString();

                Session["EmpID"] =
                    EmpID;
            }

            if
            (
                !IsPostBack
            )
            {
                LoadDashboard();
            }
        }


        private void LoadDashboard()
        {
            LoadTraineeDetails();

            LoadDashboardSummary();

            LoadProgress();
        }


        /*
         * =====================================================
         * TRAINEE DETAILS
         * Internal + External
         * =====================================================
         */

        private void LoadTraineeDetails()
        {
            lblTraineeID.Text =
                EmpID;

            string sql =
                "SELECT " +
                "EmpName," +
                "'Internal' AS TraineeType " +
                "FROM EmpBasicMaster " +
                "WHERE EmpID=@EmpID " +

                "UNION ALL " +

                "SELECT " +
                "TraineeName AS EmpName," +
                "'External' AS TraineeType " +
                "FROM TraineeMasterExternal " +
                "WHERE EmpIDExternal=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    EmpID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                lblTraineeName.Text =
                    EmpID;

                lblTraineeType.Text =
                    "Trainee";

                return;
            }

            lblTraineeName.Text =
                dt.Rows[0]["EmpName"]
                .ToString();

            lblTraineeType.Text =
                dt.Rows[0]["TraineeType"]
                .ToString();
        }


        /*
         * =====================================================
         * MAIN DASHBOARD COUNTS
         * =====================================================
         */

        private void LoadDashboardSummary()
        {
            string sql =
                "SELECT " +

                "(" +
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment TA " +
                "WHERE TA.EmpID=@EmpID " +
                "AND TA.AssignmentStatus='Assigned'" +
                ") AS TotalTraining," +

                "(" +
                "SELECT COUNT(*) " +
                "FROM TrainingProgress TP " +
                "WHERE TP.EmpID=@EmpID " +
                "AND TP.AttendanceCompleted=1" +
                ") AS AttendanceCompleted," +

                "(" +
                "SELECT COUNT(*) " +
                "FROM TestMaster TM " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "INNER JOIN TrainingAssignment TA " +
                "ON TA.TrainingID=SM.TrainingID " +
                "WHERE TA.EmpID=@EmpID " +
                "AND TA.AssignmentStatus='Assigned' " +
                "AND TM.IsPublished=1" +
                ") AS PublishedTests," +

                "(" +
                "SELECT COUNT(*) " +
                "FROM TestMaster TM " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "INNER JOIN TrainingAssignment TA " +
                "ON TA.TrainingID=SM.TrainingID " +
                "WHERE TA.EmpID=@EmpID " +
                "AND TA.AssignmentStatus='Assigned' " +
                "AND TM.IsPublished=1 " +
                "AND EXISTS " +
                "(" +
                "SELECT 1 " +
                "FROM TestAttempt TAT " +
                "WHERE TAT.TestID=TM.TestID " +
                "AND TAT.EmpID=@EmpID " +
                "AND TAT.Submitted=1" +
                ")" +
                ") AS CompletedTests," +

                "(" +
                "SELECT COUNT(*) " +
                "FROM TrainingProgress TP " +
                "WHERE TP.EmpID=@EmpID " +
                "AND TP.BatchFeedbackCompleted=1" +
                ") AS FeedbackCompleted," +

                "(" +
                "SELECT COUNT(DISTINCT TC.TrainingID) " +
                "FROM TrainingCertificate TC " +
                "WHERE TC.EmpID=@EmpID " +
                "AND TC.CertificateStatus='A'" +
                ") AS CertificateGenerated";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    EmpID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                SetDashboardZero();

                return;
            }

            int totalTraining =
                GetIntValue(
                    dt.Rows[0]["TotalTraining"]);

            int attendanceCompleted =
                GetIntValue(
                    dt.Rows[0]["AttendanceCompleted"]);

            int publishedTests =
                GetIntValue(
                    dt.Rows[0]["PublishedTests"]);

            int completedTests =
                GetIntValue(
                    dt.Rows[0]["CompletedTests"]);

            int feedbackCompleted =
                GetIntValue(
                    dt.Rows[0]["FeedbackCompleted"]);

            int certificateGenerated =
                GetIntValue(
                    dt.Rows[0]["CertificateGenerated"]);

            int pendingTests =
                publishedTests
                -
                completedTests;

            if
            (
                pendingTests
                <
                0
            )
            {
                pendingTests =
                    0;
            }

            int feedbackPending =
                totalTraining
                -
                feedbackCompleted;

            if
            (
                feedbackPending
                <
                0
            )
            {
                feedbackPending =
                    0;
            }


            /*
             * SUMMARY CARDS
             */

            lblTrainingCount.Text =
                totalTraining.ToString();

            lblAttendance.Text =
                attendanceCompleted.ToString();

            lblPublishedTests.Text =
                publishedTests.ToString();

            lblCompletedTests.Text =
                completedTests.ToString();

            lblPendingTests.Text =
                pendingTests.ToString();

            lblBatchFeedback.Text =
                feedbackCompleted.ToString();

            lblCertificate.Text =
                certificateGenerated.ToString();


            /*
             * CURRENT STATUS
             */

            lblStatusTraining.Text =
                totalTraining.ToString();

            lblStatusTests.Text =
                publishedTests.ToString();

            lblStatusPendingTests.Text =
                pendingTests.ToString();

            lblStatusFeedback.Text =
                feedbackPending.ToString();

            lblStatusCertificate.Text =
                certificateGenerated.ToString();
        }


        /*
         * =====================================================
         * PROGRESS
         * =====================================================
         */

        private void LoadProgress()
        {
            int totalTraining =
                GetLabelValue(
                    lblTrainingCount.Text);

            int attendanceCompleted =
                GetLabelValue(
                    lblAttendance.Text);

            int publishedTests =
                GetLabelValue(
                    lblPublishedTests.Text);

            int completedTests =
                GetLabelValue(
                    lblCompletedTests.Text);

            int feedbackCompleted =
                GetLabelValue(
                    lblBatchFeedback.Text);

            int certificateGenerated =
                GetLabelValue(
                    lblCertificate.Text);


            /*
             * ATTENDANCE
             */

            lblProgressAttendance.Text =
                attendanceCompleted
                +
                "/"
                +
                totalTraining;

            SetProgressBar(
                barAttendance,
                attendanceCompleted,
                totalTraining);


            /*
             * TESTS
             */

            lblProgressTests.Text =
                completedTests
                +
                "/"
                +
                publishedTests;

            SetProgressBar(
                barTests,
                completedTests,
                publishedTests);


            /*
             * FEEDBACK
             */

            lblProgressFeedback.Text =
                feedbackCompleted
                +
                "/"
                +
                totalTraining;

            SetProgressBar(
                barFeedback,
                feedbackCompleted,
                totalTraining);


            /*
             * CERTIFICATE
             */

            lblProgressCertificate.Text =
                certificateGenerated
                +
                "/"
                +
                totalTraining;

            SetProgressBar(
                barCertificate,
                certificateGenerated,
                totalTraining);
        }


        /*
         * =====================================================
         * PROGRESS BAR
         * =====================================================
         */

        private void SetProgressBar(
            System.Web.UI.WebControls.Panel panel,
            int completed,
            int total)
        {
            int percentage =
                0;

            if
            (
                total
                >
                0
            )
            {
                percentage =
                    Convert.ToInt32(
                        (
                            completed
                            *
                            100.0
                        )
                        /
                        total);
            }

            if
            (
                percentage
                >
                100
            )
            {
                percentage =
                    100;
            }

            if
            (
                percentage
                <
                0
            )
            {
                percentage =
                    0;
            }

            panel.Style["width"] =
                percentage.ToString()
                +
                "%";

            panel.Attributes["aria-valuenow"] =
                percentage.ToString();

            panel.Attributes["aria-valuemin"] =
                "0";

            panel.Attributes["aria-valuemax"] =
                "100";
        }


        /*
         * =====================================================
         * HELPERS
         * =====================================================
         */

        private int GetIntValue(
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
                value.ToString()
                ==
                ""
            )
            {
                return 0;
            }

            int result =
                0;

            Int32.TryParse(
                value.ToString(),
                out result);

            return result;
        }


        private int GetLabelValue(
            string value)
        {
            int result =
                0;

            Int32.TryParse(
                value,
                out result);

            return result;
        }


        private void SetDashboardZero()
        {
            lblTrainingCount.Text =
                "0";

            lblAttendance.Text =
                "0";

            lblPublishedTests.Text =
                "0";

            lblCompletedTests.Text =
                "0";

            lblPendingTests.Text =
                "0";

            lblBatchFeedback.Text =
                "0";

            lblCertificate.Text =
                "0";

            lblStatusTraining.Text =
                "0";

            lblStatusTests.Text =
                "0";

            lblStatusPendingTests.Text =
                "0";

            lblStatusFeedback.Text =
                "0";

            lblStatusCertificate.Text =
                "0";
        }


        /*
         * =====================================================
         * NAVIGATION
         * =====================================================
         */

        protected void lnkMyTraining_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "MyTrainings.aspx");
        }


        protected void lnkAttendance_Click(
            object sender,
            EventArgs e)
        {
            Response.Redirect(
                "Attendance.aspx");
        }


        protected void lnkPendingTests_Click(
            object sender,
            EventArgs e)
        {
            /*
             * Tests session-wise available hote hain.
             * MyTrainings se session open hoga.
             */

            Response.Redirect(
                "MyTrainings.aspx");
        }


        protected void lnkBatchFeedback_Click(
            object sender,
            EventArgs e)
        {
            /*
             * Training select kiye bina direct feedback page
             * nahi kholenge.
             */

            Response.Redirect(
                "MyTrainings.aspx");
        }


        protected void lnkCertificate_Click(
            object sender,
            EventArgs e)
        {
            /*
             * Dashboard se ALL generated certificates.
             * Isliye training-specific filter remove kar rahe hain.
             */

            Session.Remove(
                "CertificateFromTraining");

            Response.Redirect(
                "MyCertificate.aspx");
        }
    }
}