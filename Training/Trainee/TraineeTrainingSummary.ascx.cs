using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class TraineeTrainingSummary :
        System.Web.UI.UserControl
    {
        clsDataAccess objDB =
            new clsDataAccess();


        public void LoadTraining(
            string trainingID,
            string empID)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    trainingID)
                ||
                string.IsNullOrWhiteSpace(
                    empID)
            )
            {
                ClearSummary();

                return;
            }

            LoadTrainingDetails(
                trainingID);

            LoadTrainerDetails(
                trainingID);

            LoadAttendanceSummary(
                trainingID,
                empID);

            LoadCompletionStatus(
                trainingID,
                empID);
        }


        /*
         * =====================================================
         * TRAINING DETAILS
         * =====================================================
         */

        private void LoadTrainingDetails(
            string trainingID)
        {
            string sql =
                "SELECT " +
                "TD.TrainingID," +
                "ISNULL(CM.CourseName,'') AS CourseName," +
                "ISNULL(TD.TrainingType,'') AS TrainingType," +
                "ISNULL(TD.TrainingOrganizer,'') AS TrainingOrganizer," +
                "ISNULL(TD.TrainingLocation,'') AS TrainingLocation," +
                "ISNULL(TD.Batch,'') AS Batch," +
                "TD.DateFrom," +
                "TD.DateTo," +
                "ISNULL(TD.NoOfDays,0) AS NoOfDays," +
                "ISNULL(TD.TrainingStatus,'') AS TrainingStatus," +

                "(" +
                "SELECT COUNT(*) " +
                "FROM SessionMaster SM " +
                "WHERE SM.TrainingID=TD.TrainingID" +
                ") AS TotalSessions " +

                "FROM TrainingDetails TD " +

                "LEFT JOIN CourseMaster CM " +
                "ON TD.CourseID=CM.CourseID " +

                "WHERE TD.TrainingID=@TrainingID";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID)
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
                return;
            }


            DataRow row =
                dt.Rows[0];


            lblTrainingID.Text =
                row["TrainingID"]
                .ToString();


            lblCourse.Text =
                row["CourseName"]
                .ToString();


            lblTrainingType.Text =
                row["TrainingType"]
                .ToString();


            lblOrganizer.Text =
                row["TrainingOrganizer"]
                .ToString();


            lblLocation.Text =
                row["TrainingLocation"]
                .ToString();


            lblBatch.Text =
                row["Batch"]
                .ToString();


            lblTotalSessions.Text =
                row["TotalSessions"]
                .ToString();


            lblDateFrom.Text =
                FormatDate(
                    row["DateFrom"]);


            lblDateTo.Text =
                FormatDate(
                    row["DateTo"]);


            int noOfDays =
                GetIntValue(
                    row["NoOfDays"]);


            if
            (
                noOfDays
                >
                0
            )
            {
                lblDuration.Text =
                    noOfDays
                    +
                    (
                        noOfDays
                        ==
                        1
                        ?
                        " Day"
                        :
                        " Days"
                    );
            }
            else
            {
                lblDuration.Text =
                    "-";
            }


            string trainingStatus =
                row["TrainingStatus"]
                .ToString();


            if
            (
                string.IsNullOrWhiteSpace(
                    trainingStatus)
            )
            {
                trainingStatus =
                    "Pending";
            }


            SetStatus(
                lblTrainingStatus,
                trainingStatus);
        }


        /*
         * =====================================================
         * TRAINER DETAILS
         *
         * Training level:
         * all distinct trainers used in sessions
         * =====================================================
         */

        private void LoadTrainerDetails(
            string trainingID)
        {
            string sql =
                "SELECT DISTINCT " +
                "TR.TrainerID," +
                "ISNULL(TR.TrainerType,'') AS TrainerType," +

                "CASE " +
                "WHEN TR.TrainerType='Internal' " +
                "THEN ISNULL(EB.EmpName,'') " +
                "ELSE ISNULL(TR.NameExternal,'') " +
                "END AS TrainerName " +

                "FROM SessionMaster SM " +

                "INNER JOIN TrainerMaster TR " +
                "ON SM.TrainerID=TR.TrainerID " +

                "LEFT JOIN EmpBasicMaster EB " +
                "ON TR.EmpID=EB.EmpID " +

                "WHERE SM.TrainingID=@TrainingID " +

                "ORDER BY TrainerName";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID)
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
                lblTrainer.Text =
                    "-";

                lblTrainerType.Text =
                    "-";

                return;
            }


            List<string> trainerNames =
                new List<string>();


            List<string> trainerTypes =
                new List<string>();


            foreach
            (
                DataRow row
                in
                dt.Rows
            )
            {
                string trainerName =
                    row["TrainerName"]
                    .ToString();


                string trainerType =
                    row["TrainerType"]
                    .ToString();


                if
                (
                    !string.IsNullOrWhiteSpace(
                        trainerName)
                    &&
                    !trainerNames.Contains(
                        trainerName)
                )
                {
                    trainerNames.Add(
                        trainerName);
                }


                if
                (
                    !string.IsNullOrWhiteSpace(
                        trainerType)
                    &&
                    !trainerTypes.Contains(
                        trainerType)
                )
                {
                    trainerTypes.Add(
                        trainerType);
                }
            }


            lblTrainer.Text =
                trainerNames.Count
                >
                0
                ?
                string.Join(
                    ", ",
                    trainerNames.ToArray())
                :
                "-";


            if
            (
                trainerTypes.Count
                ==
                0
            )
            {
                lblTrainerType.Text =
                    "-";
            }
            else if
            (
                trainerTypes.Count
                ==
                1
            )
            {
                lblTrainerType.Text =
                    trainerTypes[0];
            }
            else
            {
                lblTrainerType.Text =
                    "Multiple";
            }
        }


        /*
         * =====================================================
         * ATTENDANCE SUMMARY
         *
         * SessionMaster       = total sessions
         * SessionAttendance   = trainee attendance
         * =====================================================
         */

        private void LoadAttendanceSummary(
            string trainingID,
            string empID)
        {
            string sql =
                "SELECT " +

                "(" +
                "SELECT COUNT(*) " +
                "FROM SessionMaster SM " +
                "WHERE SM.TrainingID=@TrainingID" +
                ") AS TotalSessions," +

                "(" +
                "SELECT COUNT(*) " +
                "FROM SessionAttendance SA " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=SA.SessionID " +
                "WHERE SM.TrainingID=@TrainingID " +
                "AND SA.EmpID=@EmpID " +
                "AND SA.AttendanceStatus='Present'" +
                ") AS PresentSessions";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@EmpID",
                    empID)
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
                return;
            }


            int totalSessions =
                GetIntValue(
                    dt.Rows[0]["TotalSessions"]);


            int presentSessions =
                GetIntValue(
                    dt.Rows[0]["PresentSessions"]);


            decimal percentage =
                0;


            if
            (
                totalSessions
                >
                0
            )
            {
                percentage =
                    (
                        presentSessions
                        *
                        100m
                    )
                    /
                    totalSessions;
            }


            lblPresent.Text =
                presentSessions
                .ToString();


            lblTotalAttendanceSessions.Text =
                totalSessions
                .ToString();


            lblAttendancePercent.Text =
                Math.Round(
                    percentage,
                    2)
                .ToString(
                    "0.##")
                +
                "%";


            string attendanceStatus =
                "Pending";


            if
            (
                totalSessions
                ==
                0
            )
            {
                attendanceStatus =
                    "Not Started";
            }
            else if
            (
                presentSessions
                ==
                totalSessions
            )
            {
                attendanceStatus =
                    "Completed";
            }
            else
            {
                attendanceStatus =
                    "Pending";
            }


            SetStatus(
                lblAttendanceStatus,
                attendanceStatus);
        }


        /*
         * =====================================================
         * COMPLETION STATUS
         * =====================================================
         */

        private void LoadCompletionStatus(
            string trainingID,
            string empID)
        {
            string sql =
                "SELECT " +

                "CASE " +
                "WHEN EXISTS " +
                "(" +
                "SELECT 1 " +
                "FROM Feedback F " +
                "WHERE F.TrainingID=@TrainingID " +
                "AND F.EmpID=@EmpID " +
                "AND F.Submitted=1" +
                ") " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS FeedbackCompleted," +

                "CASE " +
                "WHEN EXISTS " +
                "(" +
                "SELECT 1 " +
                "FROM TrainingCertificate TC " +
                "WHERE TC.TrainingID=@TrainingID " +
                "AND TC.EmpID=@EmpID " +
                "AND TC.CertificateStatus='A'" +
                ") " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS CertificateGenerated," +

                "CASE " +
                "WHEN EXISTS " +
                "(" +
                "SELECT 1 " +
                "FROM TrainingProgress TP " +
                "WHERE TP.TrainingID=@TrainingID " +
                "AND TP.EmpID=@EmpID " +
                "AND TP.AttendanceCompleted=1" +
                ") " +
                "THEN 1 " +
                "ELSE 0 " +
                "END AS AttendanceCompleted";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@EmpID",
                    empID)
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
                return;
            }


            bool feedbackCompleted =
                GetIntValue(
                    dt.Rows[0]["FeedbackCompleted"])
                ==
                1;


            bool certificateGenerated =
                GetIntValue(
                    dt.Rows[0]["CertificateGenerated"])
                ==
                1;


            bool attendanceCompleted =
                GetIntValue(
                    dt.Rows[0]["AttendanceCompleted"])
                ==
                1;


            if
            (
                feedbackCompleted
            )
            {
                SetStatus(
                    lblFeedbackStatus,
                    "Completed");
            }
            else
            {
                SetStatus(
                    lblFeedbackStatus,
                    "Pending");
            }


            if
            (
                certificateGenerated
            )
            {
                SetStatus(
                    lblCertificateStatus,
                    "Generated");
            }
            else
            {
                SetStatus(
                    lblCertificateStatus,
                    "Not Generated");
            }


            /*
             * Overall Status is deliberately simple.
             *
             * Exams are session-specific and therefore
             * NOT calculated here.
             *
             * Actual eligibility for feedback/certificate
             * remains in their respective pages/services.
             */

            if
            (
                certificateGenerated
            )
            {
                SetStatus(
                    lblOverallStatus,
                    "Completed");
            }
            else if
            (
                feedbackCompleted
            )
            {
                SetStatus(
                    lblOverallStatus,
                    "Feedback Completed");
            }
            else if
            (
                attendanceCompleted
            )
            {
                SetStatus(
                    lblOverallStatus,
                    "In Progress");
            }
            else
            {
                SetStatus(
                    lblOverallStatus,
                    "In Progress");
            }
        }


        /*
         * =====================================================
         * STATUS CSS
         * =====================================================
         */

        private void SetStatus(
       System.Web.UI.WebControls.Label label,
       string status)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    status)
            )
            {
                status =
                    "Pending";
            }

            label.Text =
                status;

            string value =
                status
                .Trim()
                .ToLower();

            string cssClass =
                "status-badge status-secondary";

            if
            (
                value == "completed"
                ||
                value == "generated"
                ||
                value == "present"
                ||
                value == "passed"
                ||
                value == "active"
            )
            {
                cssClass =
                    "status-badge status-success";
            }
            else if
            (
                value == "pending"
                ||
                value == "in progress"
                ||
                value == "resume"
                ||
                value == "not generated"
            )
            {
                cssClass =
                    "status-badge status-warning";
            }
            else if
            (
                value == "failed"
                ||
                value == "absent"
                ||
                value == "cancelled"
            )
            {
                cssClass =
                    "status-badge status-danger";
            }
            else if
            (
                value == "published"
            )
            {
                cssClass =
                    "status-badge status-info";
            }
            else if
            (
                value == "not published"
                ||
                value == "not started"
            )
            {
                cssClass =
                    "status-badge status-secondary";
            }

            label.CssClass =
                cssClass;
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


        private string FormatDate(
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


            DateTime date;


            if
            (
                DateTime.TryParse(
                    value.ToString(),
                    out date)
            )
            {
                return
                    date.ToString(
                        "dd-MM-yyyy");
            }


            return
                value.ToString();
        }


        private void ClearSummary()
        {
            lblTrainingID.Text =
                "-";

            lblCourse.Text =
                "-";

            lblTrainingType.Text =
                "-";

            lblTrainingStatus.Text =
                "-";

            lblLocation.Text =
                "-";

            lblOrganizer.Text =
                "-";

            lblBatch.Text =
                "-";

            lblDuration.Text =
                "-";

            lblDateFrom.Text =
                "-";

            lblDateTo.Text =
                "-";

            lblTotalSessions.Text =
                "0";

            lblTrainer.Text =
                "-";

            lblTrainerType.Text =
                "-";

            lblPresent.Text =
                "0";

            lblTotalAttendanceSessions.Text =
                "0";

            lblAttendancePercent.Text =
                "0%";

            lblAttendanceStatus.Text =
                "Pending";

            lblFeedbackStatus.Text =
                "Pending";

            lblCertificateStatus.Text =
                "Not Generated";

            lblOverallStatus.Text =
                "In Progress";
        }
    }
}