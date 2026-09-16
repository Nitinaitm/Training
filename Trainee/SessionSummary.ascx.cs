using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class SessionSummary :
        System.Web.UI.UserControl
    {
        clsDataAccess objDB =
            new clsDataAccess();


        public void LoadSession(
            string trainingID,
            string sessionID,
            string empID)
        {
            if
            (
                string.IsNullOrWhiteSpace(trainingID)
                ||
                string.IsNullOrWhiteSpace(sessionID)
                ||
                string.IsNullOrWhiteSpace(empID)
            )
            {
                ClearSummary();

                return;
            }

            LoadSessionDetails(
                trainingID,
                sessionID);

            LoadAttendance(
                sessionID,
                empID);

            LoadPreTest(
                sessionID,
                empID);

            LoadPostTest(
                sessionID,
                empID);
        }


        /*
         * =====================================================
         * SESSION DETAILS
         * =====================================================
         */

        private void LoadSessionDetails(
            string trainingID,
            string sessionID)
        {
            string sql =
                "SELECT " +
                "SM.SessionID," +
                "SM.SessionNo," +
                "SM.SessionName," +
                "SM.SessionDate," +
                "SM.StartTime," +
                "SM.EndTime," +
                "SM.AttendanceStatus AS SessionStatus," +
                "ISNULL(TM.TopicName,'') AS TopicName," +
                "ISNULL(TR.TrainerType,'') AS TrainerType," +

                "CASE " +
                "WHEN TR.TrainerType='Internal' " +
                "THEN ISNULL(EB.EmpName,'') " +
                "ELSE ISNULL(TR.NameExternal,'') " +
                "END AS TrainerName " +

                "FROM SessionMaster SM " +

                "LEFT JOIN TopicMaster TM " +
                "ON SM.TopicID=TM.TopicID " +

                "LEFT JOIN TrainerMaster TR " +
                "ON SM.TrainerID=TR.TrainerID " +

                "LEFT JOIN EmpBasicMaster EB " +
                "ON TR.EmpID=EB.EmpID " +

                "WHERE SM.TrainingID=@TrainingID " +
                "AND SM.SessionID=@SessionID";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@SessionID",
                    sessionID)
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


            lblSessionNo.Text =
                GetText(
                    row["SessionNo"]);


            lblSessionName.Text =
                GetText(
                    row["SessionName"]);


            lblTopic.Text =
                GetText(
                    row["TopicName"]);


            lblTrainer.Text =
                GetText(
                    row["TrainerName"]);


            lblTrainerType.Text =
                GetText(
                    row["TrainerType"]);


            lblSessionDate.Text =
                FormatDate(
                    row["SessionDate"]);


            lblStartTime.Text =
                FormatTime(
                    row["StartTime"]);


            lblEndTime.Text =
                FormatTime(
                    row["EndTime"]);


            lblDuration.Text =
                CalculateDuration(
                    row["StartTime"],
                    row["EndTime"]);


            string sessionStatus =
                GetText(
                    row["SessionStatus"]);


            if
            (
                sessionStatus
                ==
                "-"
            )
            {
                sessionStatus =
                    "Pending";
            }


            SetStatus(
                lblSessionStatus,
                sessionStatus);
        }


        /*
         * =====================================================
         * TRAINEE ATTENDANCE
         * =====================================================
         */

        private void LoadAttendance(
            string sessionID,
            string empID)
        {
            string sql =
                "SELECT TOP 1 " +
                "ISNULL(SA.AttendanceStatus,'Pending') " +
                "AS AttendanceStatus " +
                "FROM SessionAttendance SA " +
                "WHERE SA.SessionID=@SessionID " +
                "AND SA.EmpID=@EmpID";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@SessionID",
                    sessionID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };


            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);


            string status =
                "Pending";


            if
            (
                dt.Rows.Count
                >
                0
            )
            {
                status =
                    dt.Rows[0]["AttendanceStatus"]
                    .ToString();


                if
                (
                    string.IsNullOrWhiteSpace(
                        status)
                )
                {
                    status =
                        "Pending";
                }
            }


            SetStatus(
                lblAttendance,
                status);
        }


        /*
         * =====================================================
         * PRE TEST
         * =====================================================
         */

        private void LoadPreTest(
            string sessionID,
            string empID)
        {
            LoadTestSummary(
                sessionID,
                empID,
                "Pre",
                lblPreStatus,
                lblPreAttempts,
                lblPreScore,
                lblPreResult);
        }


        /*
         * =====================================================
         * POST TEST
         * =====================================================
         */

        private void LoadPostTest(
            string sessionID,
            string empID)
        {
            LoadTestSummary(
                sessionID,
                empID,
                "Post",
                lblPostStatus,
                lblPostAttempts,
                lblPostScore,
                lblPostResult);
        }


        /*
         * =====================================================
         * COMMON TEST SUMMARY
         *
         * TestMaster = Published test
         * TestAttempt = Attempts
         * TestResult = Submitted result
         * =====================================================
         */

        private void LoadTestSummary(
            string sessionID,
            string empID,
            string testType,
            System.Web.UI.WebControls.Label lblStatus,
            System.Web.UI.WebControls.Label lblAttempts,
            System.Web.UI.WebControls.Label lblScore,
            System.Web.UI.WebControls.Label lblResult)
        {
            string testSql =
                "SELECT TOP 1 " +
                "TM.TestID " +
                "FROM TestMaster TM " +
                "WHERE TM.SessionID=@SessionID " +
                "AND TM.TestType=@TestType " +
                "AND TM.IsPublished=1 " +
                "ORDER BY TM.ID DESC";


            SqlParameter[] testParam =
            {
                new SqlParameter(
                    "@SessionID",
                    sessionID),

                new SqlParameter(
                    "@TestType",
                    testType)
            };


            DataTable testDT =
                objDB.GetDataTable(
                    testSql,
                    testParam);


            /*
             * Test not published
             */

            if
            (
                testDT.Rows.Count
                ==
                0
            )
            {
                SetStatus(
                    lblStatus,
                    "Not Published");

                lblAttempts.Text =
                    "0";

                lblScore.Text =
                    "-";

                lblResult.Text =
                    "-";

                return;
            }


            string testID =
                testDT.Rows[0]["TestID"]
                .ToString();


            /*
             * Count submitted attempts.
             *
             * Attempt means an actual submitted exam.
             * An unfinished/resumable attempt is not counted
             * as completed attempt here.
             */

            string attemptSql =
                "SELECT " +
                "COUNT(*) " +
                "FROM TestAttempt TA " +
                "WHERE TA.TestID=@TestID " +
                "AND TA.EmpID=@EmpID " +
                "AND TA.Submitted=1";


            SqlParameter[] attemptParam =
            {
                new SqlParameter(
                    "@TestID",
                    testID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };


            object attemptObj =
                objDB.ExecuteScalar(
                    attemptSql,
                    attemptParam);


            int attempts =
                GetIntValue(
                    attemptObj);


            lblAttempts.Text =
                attempts.ToString();


            /*
             * Check incomplete/resumable attempt.
             */

            string resumeSql =
                "SELECT COUNT(*) " +
                "FROM TestAttempt TA " +
                "WHERE TA.TestID=@TestID " +
                "AND TA.EmpID=@EmpID " +
                "AND ISNULL(TA.Submitted,0)=0";


            SqlParameter[] resumeParam =
            {
                new SqlParameter(
                    "@TestID",
                    testID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };


            int incompleteAttempt =
                GetIntValue(
                    objDB.ExecuteScalar(
                        resumeSql,
                        resumeParam));


            /*
             * No submitted attempt
             */

            if
            (
                attempts
                ==
                0
            )
            {
                if
                (
                    incompleteAttempt
                    >
                    0
                )
                {
                    SetStatus(
                        lblStatus,
                        "Resume");
                }
                else
                {
                    SetStatus(
                        lblStatus,
                        "Pending");
                }


                lblScore.Text =
                    "-";

                lblResult.Text =
                    "-";

                return;
            }


            /*
             * Latest submitted result
             */

            string resultSql =
                "SELECT TOP 1 " +
                "TR.AttemptNo," +
                "TR.ObtainedMarks," +
                "TR.TotalMarks," +
                "TR.Percentage," +
                "TR.ResultStatus " +

                "FROM TestResult TR " +

                "WHERE TR.TestID=@TestID " +
                "AND TR.EmpID=@EmpID " +

                "ORDER BY " +
                "TR.AttemptNo DESC," +
                "TR.ID DESC";


            SqlParameter[] resultParam =
            {
                new SqlParameter(
                    "@TestID",
                    testID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };


            DataTable resultDT =
                objDB.GetDataTable(
                    resultSql,
                    resultParam);


            if
            (
                resultDT.Rows.Count
                ==
                0
            )
            {
                SetStatus(
                    lblStatus,
                    "Completed");

                lblScore.Text =
                    "-";

                lblResult.Text =
                    "-";

                return;
            }


            DataRow resultRow =
                resultDT.Rows[0];


            decimal obtainedMarks =
                GetDecimalValue(
                    resultRow["ObtainedMarks"]);


            decimal totalMarks =
                GetDecimalValue(
                    resultRow["TotalMarks"]);


            decimal percentage =
                GetDecimalValue(
                    resultRow["Percentage"]);


            string resultStatus =
                resultRow["ResultStatus"]
                .ToString();


            lblScore.Text =
                obtainedMarks.ToString(
                    "0.##")
                +
                " / "
                +
                totalMarks.ToString(
                    "0.##")
                +
                " ("
                +
                percentage.ToString(
                    "0.##")
                +
                "%)";


            if
            (
                string.IsNullOrWhiteSpace(
                    resultStatus)
            )
            {
                resultStatus =
                    "-";
            }


            //lblResult.Text =
            //    resultStatus;
            SetStatus(
    lblResult,
    resultStatus);

            /*
             * Test has at least one submitted attempt.
             */

            SetStatus(
                lblStatus,
                "Completed");
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
                value == "pass"
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
                value == "fail"
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
                ||
                value == "-"
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
         * DATE
         * =====================================================
         */

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
                ||
                string.IsNullOrWhiteSpace(
                    value.ToString())
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


        /*
         * =====================================================
         * TIME
         * =====================================================
         */

        private string FormatTime(
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
                string.IsNullOrWhiteSpace(
                    value.ToString())
            )
            {
                return "-";
            }


            DateTime time;


            if
            (
                DateTime.TryParse(
                    value.ToString(),
                    out time)
            )
            {
                return
                    time.ToString(
                        "hh:mm tt");
            }


            return
                value.ToString();
        }


        /*
         * =====================================================
         * DURATION
         * =====================================================
         */

        private string CalculateDuration(
            object startValue,
            object endValue)
        {
            if
            (
                startValue
                ==
                null
                ||
                endValue
                ==
                null
                ||
                startValue
                ==
                DBNull.Value
                ||
                endValue
                ==
                DBNull.Value
            )
            {
                return "-";
            }


            DateTime startTime;
            DateTime endTime;


            bool startValid =
                DateTime.TryParse(
                    startValue.ToString(),
                    out startTime);


            bool endValid =
                DateTime.TryParse(
                    endValue.ToString(),
                    out endTime);


            if
            (
                !startValid
                ||
                !endValid
            )
            {
                return "-";
            }


            TimeSpan duration =
                endTime
                -
                startTime;


            if
            (
                duration.TotalMinutes
                <
                0
            )
            {
                duration =
                    duration
                    .Add(
                        TimeSpan.FromDays(
                            1));
            }


            if
            (
                duration.TotalMinutes
                <=
                0
            )
            {
                return "-";
            }


            int hours =
                Convert.ToInt32(
                    Math.Floor(
                        duration.TotalHours));


            int minutes =
                duration.Minutes;


            if
            (
                hours
                >
                0
                &&
                minutes
                >
                0
            )
            {
                return
                    hours
                    +
                    " Hr "
                    +
                    minutes
                    +
                    " Min";
            }


            if
            (
                hours
                >
                0
            )
            {
                return
                    hours
                    +
                    (
                        hours
                        ==
                        1
                        ?
                        " Hour"
                        :
                        " Hours"
                    );
            }


            return
                minutes
                +
                " Minutes";
        }


        /*
         * =====================================================
         * HELPERS
         * =====================================================
         */

        private string GetText(
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
                string.IsNullOrWhiteSpace(
                    value.ToString())
            )
            {
                return "-";
            }


            return
                value.ToString();
        }


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


        private decimal GetDecimalValue(
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


            decimal result =
                0;


            Decimal.TryParse(
                value.ToString(),
                out result);


            return result;
        }


        /*
         * =====================================================
         * CLEAR
         * =====================================================
         */

        private void ClearSummary()
        {
            lblSessionNo.Text =
                "-";

            lblSessionName.Text =
                "-";

            lblTopic.Text =
                "-";

            lblSessionStatus.Text =
                "Pending";

            lblTrainer.Text =
                "-";

            lblTrainerType.Text =
                "-";

            lblSessionDate.Text =
                "-";

            lblStartTime.Text =
                "-";

            lblEndTime.Text =
                "-";

            lblDuration.Text =
                "-";

            lblAttendance.Text =
                "Pending";

            lblPreStatus.Text =
                "Not Published";

            lblPreAttempts.Text =
                "0";

            lblPreScore.Text =
                "-";

            lblPreResult.Text =
                "-";

            lblPostStatus.Text =
                "Not Published";

            lblPostAttempts.Text =
                "0";

            lblPostScore.Text =
                "-";

            lblPostResult.Text =
                "-";
        }
    }
}