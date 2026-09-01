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
    public class QuestionPaperManager
    {
        private clsDataAccess objDB;

        private CommonFunctions objCommon;

        private IDGenerator objID;

        public QuestionPaperManager()
        {
            objDB =
                new clsDataAccess();

            objCommon =
                new CommonFunctions();

            objID =
               new IDGenerator();
        }
        #region Validation

        public string ValidateQuestionPaper(
            QuestionPaperModel model)
        {
            if
            (
                !Validator.Required(
                model.TestID)
            )
            {
                return
                    "Test is required.";
            }

            if
            (
                model.TotalQuestions <= 0
            )
            {
                return
                    "Total Questions should be greater than zero.";
            }

            if
            (
                model.TotalMarks <= 0
            )
            {
                return
                    "Total Marks should be greater than zero.";
            }

            if
            (
                model.EasyQuestions +
                model.MediumQuestions +
                model.HardQuestions
                !=
                model.TotalQuestions
            )
            {
                return
                    "Difficulty distribution is not matching Total Questions.";
            }

            return "";
        }

        #endregion

        #region Paper Exists

        public bool PaperExists(
            string testID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TestQuestion " +

                "WHERE TestID=@TestID";

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

        #endregion
        #region Can Generate

        public bool CanGeneratePaper(
            string testID)
        {
            return
                !PaperExists(
                testID);
        }

        #endregion
        #region Create Question Paper

        public bool CreateQuestionPaper(
            QuestionPaperModel model,
            out string message)
        {
            message = "";

            try
            {
                //-----------------------------------

                message =
                    ValidateQuestionPaper(
                    model);

                if (message != "")
                {
                    return false;
                }

                //-----------------------------------

                if
                (
                    !CanGeneratePaper(
                    model.TestID)
                )
                {
                    message =
                        "Question Paper already generated.";

                    return false;
                }

                //-----------------------------------

                if
                (
                    model.QuestionSelectionMode ==
                    Constants.QuestionSelectionMode.Random
                )
                {
                    return
                        GenerateRandomPaper(
                        model,
                        out message);
                }

                //-----------------------------------

                return
                    GenerateManualPaper(
                    model,
                    out message);
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion
        #region Paper Summary

        public DataTable GetPaperSummary(
            string testID)
        {
            string sql =

                "SELECT " +

                "COUNT(*) TotalQuestions," +

                "SUM(Marks) TotalMarks " +

                "FROM TestQuestion " +

                "WHERE TestID=@TestID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Generate Random Paper

        public bool GenerateRandomPaper(
            QuestionPaperModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Mapping Exists
                //---------------------------------------

                DataTable dtMapping =
                    GetTopicMapping(
                    model.TestID);

                if (dtMapping.Rows.Count == 0)
                {
                    message =
                        "Question mapping not found.";

                    return false;
                }

                //---------------------------------------
                // Delete Old Paper
                //---------------------------------------

                

                //---------------------------------------
                // Delete Old Paper
                //---------------------------------------

                if
                (
                    !DeletePaperQuestions(
                    model.TestID)
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                //---------------------------------------
                // Generate Topic Wise
                //---------------------------------------

                foreach
                (
                    DataRow dr
                    in
                    dtMapping.Rows
                )
                {

                    string topicID =
                        dr["TopicID"]
                        .ToString();

                    string difficulty =
                        dr["DifficultyLevel"]
                        .ToString();

                    //int requiredQuestions =
                    //    Convert.ToInt32(
                    //    dr["RequiredQuestions"]);
                    int requiredQuestions =
    Convert.ToInt32(
    dr["RequiredQuestions"]);

                    if
                    (
                        requiredQuestions <= 0
                    )
                    {
                        continue;
                    }

                    //decimal marks =
                    //    Convert.ToDecimal(
                    //    dr["MarksPerQuestion"]);
                    decimal marks = 0;

                    if
                    (
                        dr["MarksPerQuestion"] != DBNull.Value
                    )
                    {
                        marks =
                            Convert.ToDecimal(
                            dr["MarksPerQuestion"]);
                    }
                    DataTable dtQuestion =
                        GetRandomQuestions
                        (
                            model,
                            topicID,
                            difficulty,
                            requiredQuestions
                        );

                    if
                    (
                        dtQuestion.Rows.Count <
                        requiredQuestions
                    )
                    {
                        message =
                            "Not enough approved questions available for Topic : "
                            +
                            topicID;

                        return false;
                    }

                    bool result =
                        SavePaperQuestions
                        (
                            model.TestID,
                            dtQuestion,
                            marks,
                            model.CreatedBy
                        );

                    if (!result)
                    {
                        message =
                            Messages.DatabaseError;

                        return false;
                    }
                }

                //---------------------------------------
                // Success
                //---------------------------------------

                message =
                    "Question paper generated successfully.";

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

        private DataTable GetTopicMapping(
    string testID)
        {
            string sql =

                "SELECT * " +

                "FROM TestTopicMapping " +

                "WHERE TestID=@TestID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }
        private DataTable GetRandomQuestions
 (
     QuestionPaperModel model,
     string topicID,
     string difficultyLevel,
     int requiredQuestions
 )
        {
            if
            (
                requiredQuestions <= 0
            )
            {
                return
                    new DataTable();
            }

            string sql =

                "SELECT TOP (" +

                requiredQuestions +

                ") " +

                "QuestionID," +

                "Marks " +

                "FROM QuestionBank " +

                "WHERE " +

                "CourseID=@CourseID " +

                "AND TopicID=@TopicID " +

                "AND DifficultyLevel=@DifficultyLevel " +

                "AND ApprovalStatus=@ApprovalStatus " +

                "AND IsActive=1 " +

                "AND QuestionID NOT IN " +

                "(" +

                "SELECT QuestionID " +

                "FROM TestQuestion " +

                "WHERE TestID=@TestID" +

                ") " +

                "ORDER BY NEWID()";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CourseID",
            model.CourseID),

        new SqlParameter(
            "@TopicID",
            topicID),

        new SqlParameter(
            "@DifficultyLevel",
            difficultyLevel),

        new SqlParameter(
            "@ApprovalStatus",
            Constants.QuestionApproval.Approved),

        new SqlParameter(
            "@TestID",
            model.TestID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }


        #region Save Paper Questions

        private bool SavePaperQuestions
        (
            string testID,
            DataTable dtQuestion,
            decimal marks,
            string createdBy
        )
        {
            try
            {
                int questionOrder =
                    GetNextQuestionOrder(
                    testID);

                foreach
                (
                    DataRow dr
                    in
                    dtQuestion.Rows
                )
                {
                    string sql =

                        "INSERT INTO TestQuestion " +

                        "(" +

                        "TestQuestionID," +
                        "TestID," +
                        "QuestionID," +
                        "QuestionOrder," +
                        "Marks," +
                        "CreatedOn," +
                        "CreatedBy" +

                        ")" +

                        " VALUES " +

                        "(" +

                        "@TestQuestionID," +
                        "@TestID," +
                        "@QuestionID," +
                        "@QuestionOrder," +
                        "@Marks," +
                        "GETDATE()," +
                        "@CreatedBy" +

                        ")";

                    SqlParameter[] param =
                    {
                new SqlParameter(
                    "@TestQuestionID",
                    objID.GenerateTestQuestionID()),

                new SqlParameter(
                    "@TestID",
                    testID),

                new SqlParameter(
                    "@QuestionID",
                    dr["QuestionID"]),

                new SqlParameter(
                    "@QuestionOrder",
                    questionOrder),

                new SqlParameter(
                    "@Marks",
                    marks),

                new SqlParameter(
                    "@CreatedBy",
                    createdBy)
            };

                    int i =
                        objDB.ExecuteSql(
                        sql,
                        param);

                    if (i <= 0)
                    {
                        return false;
                    }

                    questionOrder++;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion



        #region Delete Paper Questions

        public bool DeletePaperQuestions(
            string testID)
        {
            try
            {
                string sql =

                    "DELETE FROM TestQuestion " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TestID",
                testID)
        };
                objDB.ExecuteSql(
sql,
param);

                return true;
                //return
                //    objDB.ExecuteSql(
                //    sql,
                //    param) > 0;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Get Paper Questions

        public DataTable GetPaperQuestions(
            string testID)
        {
            string sql =

                "SELECT " +

                "TQ.QuestionOrder," +

                "QB.QuestionID," +

                "QB.Question," +

                "QB.OptionA," +

                "QB.OptionB," +

                "QB.OptionC," +

                "QB.OptionD," +

                "QB.CorrectOption," +

                "QB.Marks " +

                "FROM TestQuestion TQ " +

                "INNER JOIN QuestionBank QB " +

                "ON TQ.QuestionID=QB.QuestionID " +

                "WHERE TQ.TestID=@TestID " +

                "ORDER BY TQ.QuestionOrder";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion
        #region Generate Manual Paper

        public bool GenerateManualPaper(
            QuestionPaperModel model,
            out string message)
        {
            message = "";

            try
            {
                if
                (
                    PaperExists(
                    model.TestID)
                )
                {
                    message =
                        "Question paper already generated.";

                    return false;
                }

                message =
                    "Manual Question Paper mode selected.";

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
        #region Regenerate Paper

        public bool RegeneratePaper(
    QuestionPaperModel model,
    out string message)
        {
            message = "";

            try
            {
                if
                (
                    !DeletePaperQuestions(
                    model.TestID)
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                return
                    GenerateRandomPaper(
                    model,
                    out message);
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion
        #region Publish Paper

        public bool PublishPaper(
    string testID,
    string userID,
    out string message)
        {
            message = "";

            try
            {
                //----------------------------------
                // Paper Generated ?
                //----------------------------------

                if
                (
                    GetQuestionCount(
                    testID) == 0
                )
                {
                    message =
                        "Question paper has not been generated.";

                    return false;
                }

                //----------------------------------
                // Publish
                //----------------------------------

                string sql =

                    "UPDATE TestMaster SET " +

                    "IsPublished=1," +

                    "Published=1," +

                    "TestStatus=@Status," +

                    "ModifiedOn=GETDATE()," +

                    "ModifiedBy=@UserID " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@Status",
                Constants.TestStatus.Published),

            new SqlParameter(
                "@UserID",
                userID),

            new SqlParameter(
                "@TestID",
                testID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) > 0
                )
                {
                    message =
                        Messages.UpdateSuccess;

                    return true;
                }

                message =
                    Messages.DatabaseError;

                return false;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion
        #region UnPublish Paper

        public bool UnPublishPaper(
            string testID,
            string userID,
            out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TestMaster SET " +

                    "IsPublished=0," +

                    "TestStatus='Draft'," +

                    "ModifiedOn=GETDATE()," +

                    "ModifiedBy=@UserID " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
            new SqlParameter("@UserID",userID),
            new SqlParameter("@TestID",testID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) > 0
                )
                {
                    message =
                        Messages.UpdateSuccess;

                    return true;
                }

                message =
                    Messages.DatabaseError;

                return false;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion

        public int GetQuestionCount(
    string testID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TestQuestion " +

                "WHERE TestID=@TestID";

            SqlParameter[] param =
            {
        new SqlParameter("@TestID",testID)
    };

            return
                objCommon.GetCount(
                sql,
                param);
        }
        public int GetTopicQuestionCount(
    string testID,
    string topicID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TestQuestion TQ " +

                "INNER JOIN QuestionBank QB " +

                "ON QB.QuestionID=TQ.QuestionID " +

                "WHERE TQ.TestID=@TestID " +

                "AND QB.TopicID=@TopicID";

            SqlParameter[] param =
            {
        new SqlParameter("@TestID",testID),
        new SqlParameter("@TopicID",topicID)
    };

            return
                objCommon.GetCount(
                sql,
                param);
        }

        public int GetDifficultyQuestionCount(
    string testID,
    string difficulty)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TestQuestion TQ " +

                "INNER JOIN QuestionBank QB " +

                "ON QB.QuestionID=TQ.QuestionID " +

                "WHERE TQ.TestID=@TestID " +

                "AND QB.DifficultyLevel=@DifficultyLevel";

            SqlParameter[] param =
            {
        new SqlParameter("@TestID",testID),
        new SqlParameter("@DifficultyLevel",difficulty)
    };

            return
                objCommon.GetCount(
                sql,
                param);
        }

        private int GetNextQuestionOrder(
    string testID)
        {
            string sql =

                "SELECT " +

                "ISNULL(MAX(QuestionOrder),0)+1 " +

                "FROM TestQuestion " +

                "WHERE TestID=@TestID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID)
    };

            object obj =
                objDB.ExecuteScalar(
                sql,
                param);

            if
            (
                obj == null ||
                obj == DBNull.Value
            )
            {
                return 1;
            }

            return
                Convert.ToInt32(
                obj);
        }
    }
}