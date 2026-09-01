using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Training.Helper;
using Training.Common;
using Training.Models;

namespace Training.Business.Assessment
{
    public class TestEngineManager
    {

        private clsDataAccess objDB;

        private CommonFunctions objCommon;

        private IDGenerator objID;

        public TestEngineManager()
        {
            objDB =
                new clsDataAccess();

            objCommon =
                new CommonFunctions();

            objID =
                new IDGenerator();
        }
        #region Start Test

        public bool StartTest(
            TestAttemptModel model,
            out string message)
        {
            message = "";

            try
            {
                //------------------------------------
                // Can Start ?
                //------------------------------------

                if
                (
                    !CanStartTest(
                    model.TestID,
                    model.EmpID,
                    out message)
                )
                {
                    return false;
                }

                //------------------------------------
                // Generate Attempt ID
                //------------------------------------

                model.AttemptID =
                    objID.GenerateAttemptID();

                //------------------------------------
                // Create Attempt
                //------------------------------------

                if
                (
                    !CreateAttempt(
                    model)
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                //------------------------------------
                // Create Attempt Answers
                //------------------------------------

                if
                (
                    !CreateAttemptAnswers(
                    model)
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                message =
                    "Test started successfully.";

                return true;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion
        public bool CanStartTest(
    string testID,
    string empID,
    out string message)
        {
            message = "";

            //----------------------------------

            if
            (
                !IsTestPublished(
                testID)
            )
            {
                message =
                    "Test is not published.";

                return false;
            }

            //----------------------------------

            if
            (
                IsTestClosed(
                testID)
            )
            {
                message =
                    "Test already closed.";

                return false;
            }

            //----------------------------------

            if
            (
                AttemptExists(
                testID,
                empID)
            )
            {
                message =
                    "Test already started.";

                return false;
            }

            return true;
        }
        public bool AttemptExists(
    string testID,
    string empID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TestAttempt " +

                "WHERE TestID=@TestID " +

                "AND EmpID=@EmpID " +

                "AND Submitted=0";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID),

        new SqlParameter(
            "@EmpID",
            empID)
    };

            return
                objCommon.GetCount(
                sql,
                param) > 0;
        }
        private bool CreateAttempt(
    TestAttemptModel model)
        {
            string sql =

                "INSERT INTO TestAttempt " +

                "(" +

                "AttemptID," +

                "TestID," +

                "EmpID," +

                "AttemptNo," +

                "StartTime," +

                "TotalQuestions," +

                "Submitted," +

                "CreatedOn," +

                "CurrentQuestionNo" +

                ")" +

                " VALUES " +

                "(" +

                "@AttemptID," +

                "@TestID," +

                "@EmpID," +

                "@AttemptNo," +

                "GETDATE()," +

                "@TotalQuestions," +

                "0," +

                "GETDATE()," +

                "1" +

                ")";

            SqlParameter[] param =
            {
        new SqlParameter("@AttemptID",model.AttemptID),
        new SqlParameter("@TestID",model.TestID),
        new SqlParameter("@EmpID",model.EmpID),
        new SqlParameter("@AttemptNo",model.AttemptNo),
        new SqlParameter("@TotalQuestions",model.TotalQuestions)
    };

            return
                objDB.ExecuteSql(
                sql,
                param) > 0;
        }
        private bool CreateAttemptAnswers(
    TestAttemptModel model)
        {
            try
            {
                string sql =

    "SELECT " +

    "TQ.QuestionID," +

    "TQ.Marks," +

    "TQ.NegativeMarks," +

    "TQ.QuestionOrder," +

    "TQ.DisplayOrder," +

    "TQ.DifficultyLevel," +

    "QB.CorrectOption " +

    "FROM TestQuestion TQ " +

    "INNER JOIN QuestionBank QB " +

    "ON QB.QuestionID=TQ.QuestionID " +

    "WHERE TQ.TestID=@TestID " +

    "AND TQ.Active=1 " +

    "ORDER BY TQ.QuestionOrder";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TestID",
                model.TestID)
        };

                DataTable dt =
                    objDB.GetDataTable(
                    sql,
                    param);

                foreach
                (
                    DataRow dr
                    in
                    dt.Rows
                )
                {
                    string insert =

                        "INSERT INTO TestAttemptAnswer " +

                        "(" +

                        "AttemptAnswerID," +
                        "AttemptID," +
                        "TestID," +
                        "EmpID," +
                        "QuestionID," +
                        "CorrectOption," +
                        "Marks," +
                        "QuestionOrder," +
                        "DisplayOrder," +
                        "CreatedOn," +
                        "MarkedForReview" +

                        ")" +

                        " VALUES " +

                        "(" +

                        "@AttemptAnswerID," +
                        "@AttemptID," +
                        "@TestID," +
                        "@EmpID," +
                        "@QuestionID," +
                        "@CorrectOption," +
                        "@Marks," +
                        "@QuestionOrder," +
                        "@DisplayOrder," +
                        "GETDATE()," +
                        "0" +

                        ")";

                    SqlParameter[] insertParam =
                    {
                new SqlParameter(
                    "@AttemptAnswerID",
                    objID.GenerateAttemptAnswerID()),

                new SqlParameter(
                    "@AttemptID",
                    model.AttemptID),

                new SqlParameter(
                    "@TestID",
                    model.TestID),

                new SqlParameter(
                    "@EmpID",
                    model.EmpID),

                new SqlParameter(
                    "@QuestionID",
                    dr["QuestionID"]),

                new SqlParameter(
    "@CorrectOption",
    dr["CorrectOption"]),

                new SqlParameter(
                    "@Marks",
                    dr["Marks"]),
                new SqlParameter(
    "@NegativeMarks",
    dr["NegativeMarks"]),

new SqlParameter(
    "@DifficultyLevel",
    dr["DifficultyLevel"]),
                new SqlParameter(
                    "@QuestionOrder",
                    dr["QuestionOrder"]),

                new SqlParameter(
                    "@DisplayOrder",
                    dr["DisplayOrder"])
            };

                    if
                    (
                        objDB.ExecuteSql(
                        insert,
                        insertParam) <= 0
                    )
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #region Load Question

        public DataTable LoadQuestion(
            string attemptID,
            int questionOrder)
        {
            string sql =

                "SELECT " +

                "TAA.AttemptAnswerID," +

                "TAA.AttemptID," +

                "TAA.QuestionID," +

                "TAA.QuestionOrder," +

                "TAA.DisplayOrder," +

                "TAA.SelectedOption," +

                "TAA.MarkedForReview," +

                "QB.Question," +

                "QB.ImagePath," +

                "QB.OptionA," +

                "QB.OptionB," +

                "QB.OptionC," +

                "QB.OptionD," +

                "QB.QuestionType," +

                "QB.Marks " +

                "FROM TestAttemptAnswer TAA " +

                "INNER JOIN QuestionBank QB " +

                "ON QB.QuestionID=TAA.QuestionID " +

                "WHERE " +

                "TAA.AttemptID=@AttemptID " +

                "AND TAA.QuestionOrder=@QuestionOrder";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@AttemptID",
            attemptID),

        new SqlParameter(
            "@QuestionOrder",
            questionOrder)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion
        #region Save Answer

   

        public bool SaveAnswer(
            string attemptID,
            string questionID,
            string selectedOption,
            bool markedForReview)
        {
            try
            {
                //------------------------------------
                // Get Correct Answer
                //------------------------------------

                string sql =

                    "SELECT " +

                    "CorrectOption," +

                    "Marks," +

                    "NegativeMarks " +

                    "FROM QuestionBank " +

                    "WHERE QuestionID=@QuestionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@QuestionID",
                questionID)
        };

                DataTable dt =
                    objDB.GetDataTable(
                    sql,
                    param);

                if (dt.Rows.Count == 0)
                {
                    return false;
                }

                string correctOption =
                    dt.Rows[0]["CorrectOption"]
                    .ToString();

                decimal marks =
                    Convert.ToDecimal(
                    dt.Rows[0]["Marks"]);

                decimal negativeMarks =
                    Convert.ToDecimal(
                    dt.Rows[0]["NegativeMarks"]);

                bool isCorrect =
                    selectedOption ==
                    correctOption;

                decimal obtainedMarks =
                    isCorrect
                    ?
                    marks
                    :
                    (
                        negativeMarks > 0
                        ?
                        -negativeMarks
                        :
                        0
                    );

                //------------------------------------
                // Update
                //------------------------------------

                sql =

                    "UPDATE TestAttemptAnswer SET " +

                    "SelectedOption=@SelectedOption," +

                    "CorrectOption=@CorrectOption," +

                    "IsCorrect=@IsCorrect," +

                    "ObtainedMarks=@ObtainedMarks," +

                    "AnsweredOn=GETDATE()," +

                    "MarkedForReview=@MarkedForReview " +

                    "WHERE AttemptID=@AttemptID " +

                    "AND QuestionID=@QuestionID";

                SqlParameter[] update =
                {
            new SqlParameter("@SelectedOption",selectedOption),
            new SqlParameter("@CorrectOption",correctOption),
            new SqlParameter("@IsCorrect",isCorrect),
            new SqlParameter("@ObtainedMarks",obtainedMarks),
            new SqlParameter("@MarkedForReview",markedForReview),
            new SqlParameter("@AttemptID",attemptID),
            new SqlParameter("@QuestionID",questionID)
        };

                return
                    objDB.ExecuteSql(
                    sql,
                    update) > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Clear Answer

        public bool ClearAnswer(
            string attemptID,
            string questionID)
        {
            string sql =

                "UPDATE TestAttemptAnswer SET " +

                "SelectedOption=NULL," +

                "AnsweredOn=NULL," +

                "MarkedForReview=0," +

                "ObtainedMarks=0 " +

                "WHERE " +

                "AttemptID=@AttemptID " +

                "AND QuestionID=@QuestionID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@AttemptID",
            attemptID),

        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            return
                objDB.ExecuteSql(
                sql,
                param) > 0;
        }

        #endregion
        #region Mark For Review

        public bool MarkForReview(
            string attemptID,
            string questionID,
            bool review)
        {
            string sql =

                "UPDATE TestAttemptAnswer SET " +

                "MarkedForReview=@Review " +

                "WHERE " +

                "AttemptID=@AttemptID " +

                "AND QuestionID=@QuestionID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@Review",
            review),

        new SqlParameter(
            "@AttemptID",
            attemptID),

        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            return
                objDB.ExecuteSql(
                sql,
                param) > 0;
        }

        #endregion
        #region Get Question By Order

        public DataTable GetQuestionByOrder(
            string attemptID,
            int questionOrder)
        {
            return
                LoadQuestion(
                attemptID,
                questionOrder);
        }

        #endregion
        public int NextQuestion(
    int currentQuestion,
    int totalQuestions)
        {
            if
            (
                currentQuestion >=
                totalQuestions
            )
            {
                return
                    totalQuestions;
            }

            return
                currentQuestion + 1;
        }
        public int PreviousQuestion(
    int currentQuestion)
        {
            if
            (
                currentQuestion <= 1
            )
            {
                return 1;
            }

            return
                currentQuestion - 1;
        }

        #region Submit Test

        public bool SubmitTest(
            string attemptID,
            out string message)
        {
            message = "";

            try
            {
                if
                (
                    !CalculateResult(
                    attemptID)
                )
                {
                    message =
                        "Result calculation failed.";

                    return false;
                }

                string sql =

                    "UPDATE TestAttempt SET " +

                    "Submitted=1," +

                    "EndTime=GETDATE() " +

                    "WHERE AttemptID=@AttemptID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@AttemptID",
                attemptID)
        };

                objDB.ExecuteSql(
                    sql,
                    param);

                message =
                    "Test submitted successfully.";

                return true;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion
        private bool CalculateResult(
    string attemptID)
        {
            try
            {
                string sql =

                    "SELECT " +

                    "COUNT(*) TotalQuestions," +

                    "SUM(CASE WHEN IsCorrect=1 THEN 1 ELSE 0 END) CorrectAnswers," +

                    "SUM(CASE WHEN IsCorrect=0 THEN 1 ELSE 0 END) WrongAnswers," +

                    "SUM(ObtainedMarks) ObtainedMarks " +

                    "FROM TestAttemptAnswer " +

                    "WHERE AttemptID=@AttemptID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@AttemptID",
                attemptID)
        };

                DataTable dt =
                    objDB.GetDataTable(
                    sql,
                    param);

                if (dt.Rows.Count == 0)
                {
                    return false;
                }

                return
                    SaveResult(
                    attemptID,
                    dt.Rows[0]);
            }
            catch
            {
                return false;
            }
        }
        private bool SaveResult(
    string attemptID,
    DataRow dr)
        {
            string sql =

                "UPDATE TestAttempt SET " +

                "CorrectAnswers=@CorrectAnswers," +

                "WrongAnswers=@WrongAnswers," +

                "ObtainedMarks=@ObtainedMarks " +

                "WHERE AttemptID=@AttemptID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CorrectAnswers",
            dr["CorrectAnswers"]),

        new SqlParameter(
            "@WrongAnswers",
            dr["WrongAnswers"]),

        new SqlParameter(
            "@ObtainedMarks",
            dr["ObtainedMarks"]),

        new SqlParameter(
            "@AttemptID",
            attemptID)
    };

            return
                objDB.ExecuteSql(
                sql,
                param) > 0;
        }
        public bool FinishAttempt(
    string attemptID)
        {
            string sql =

                "UPDATE TestAttempt SET " +

                "Submitted=1," +

                "EndTime=GETDATE() " +

                "WHERE AttemptID=@AttemptID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@AttemptID",
            attemptID)
    };

            return
                objDB.ExecuteSql(
                sql,
                param) > 0;
        }
        public int GetRemainingTime(
    DateTime startTime,
    int durationMinutes)
        {
            TimeSpan used =
                DateTime.Now -
                startTime;

            int remaining =
                durationMinutes -
                Convert.ToInt32(
                used.TotalMinutes);

            if (remaining < 0)
            {
                remaining = 0;
            }

            return remaining;
        }
        private bool IsTestPublished(
    string testID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TestMaster " +
                "WHERE TestID=@TestID " +
                "AND IsPublished=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID)
    };

            return
                objCommon.GetCount(
                sql,
                param) > 0;
        }

        private bool IsTestClosed(
    string testID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TestMaster " +
                "WHERE TestID=@TestID " +
                "AND Closed=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID)
    };

            return
                objCommon.GetCount(
                sql,
                param) > 0;
        }
    }
}