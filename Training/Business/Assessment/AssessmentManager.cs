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


    public class AssessmentManager
    {
        private clsDataAccess objDB;

        private IDGenerator objID;

        private CommonFunctions objCommon;

        public AssessmentManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }

        #region Generate Test ID

        public string GenerateTestID()
        {
            return
                objID.GenerateTestID();
        }

        #endregion

        #region Get Test By ID

        public DataTable GetTestByID(
            string testID)
        {
            string sql =
                "SELECT * " +
                "FROM TestMaster " +
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

        #region Get Tests By Training

        public DataTable GetTestsByTraining(
            string trainingID)
        {
            string sql =
                "SELECT * " +
                "FROM TestMaster " +
                "WHERE TrainingID=@TrainingID " +
                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@TrainingID",
                trainingID)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Tests By Session

        public DataTable GetTestsBySession(
            string sessionID)
        {
            string sql =
                "SELECT * " +
                "FROM TestMaster " +
                "WHERE SessionID=@SessionID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@SessionID",
                sessionID)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Tests By Scope

        public DataTable GetTestsByScope(
            string trainingID,
            string scope)
        {
            string sql =
                "SELECT * " +
                "FROM TestMaster " +
                "WHERE TrainingID=@TrainingID " +
                "AND AssessmentScope=@AssessmentScope";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@TrainingID",
                trainingID),

            new SqlParameter(
                "@AssessmentScope",
                scope)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Test Exists

        public bool TestExists(
            string testID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TestMaster " +
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

        #region Validation

        public string ValidateAssessment(
            AssessmentModel model)
        {
            if (model.ConductedByRole == "Trainer")
            {
                if
                (
                    !Validator.Required(
                    model.TrainerID)
                )
                {
                    return
                        "Trainer is required.";
                }
            }
            if
        (
            !Validator.Required(
            model.TrainingID)
        )
            {
                return
                    "Training is required.";
            }

            if
            (
                !Validator.Required(
                model.TestTitle)
            )
            {
                return
                    "Test Title is required.";
            }

            if
            (
                !Validator.Required(
                model.AssessmentScope)
            )
            {
                return
                    "Assessment Scope is required.";
            }

            if
            (
                !Validator.Required(
                model.AssessmentLevel)
            )
            {
                return
                    "Assessment Level is required.";
            }

            if
            (
                !Validator.Required(
                model.ConductedByRole)
            )
            {
                return
                    "Conducted By is required.";
            }

            if
            (
                !Validator.Required(
                model.QuestionSelectionMode)
            )
            {
                return
                    "Question Selection Mode is required.";
            }

            if
            (
                model.Duration <= 0
            )
            {
                return
                    "Duration should be greater than zero.";
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
                model.PassingMarks < 0
            )
            {
                return
                    "Passing Marks cannot be negative.";
            }

            if
            (
                model.PassingMarks >
                model.TotalMarks
            )
            {
                return
                    "Passing Marks cannot exceed Total Marks.";
            }

            if
            (
                model.StartDateTime.HasValue
                &&
                model.EndDateTime.HasValue
            )
            {
                if
                (
                    model.StartDateTime.Value >=
                    model.EndDateTime.Value
                )
                {
                    return
                        "End Date/Time must be greater than Start Date/Time.";
                }
            }

            //---------------------------------------
            // Session Test Validation
            //---------------------------------------

            if
            (
                model.AssessmentScope ==
                "Session"
            )
            {
                if
                (
                    !Validator.Required(
                    model.SessionID)
                )
                {
                    return
                        "Session is required.";
                }

                if
                (
                    !Validator.Required(
                    model.TrainerID)
                )
                {
                    return
                        "Trainer is required.";
                }
            }

            //---------------------------------------
            // Final Assessment Validation
            //---------------------------------------

            if
            (
                model.AssessmentLevel ==
                "Final"
            )
            {
                if
                (
                    model.TotalQuestions < 10
                )
                {
                    return
                        "Final assessment should contain at least 10 questions.";
                }
            }

            return "";
        }

        #endregion
        #region Save Assessment

        public bool SaveAssessment(
            AssessmentModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateAssessment(
                    model);

                if (message != "")
                {
                    return false;
                }

                //---------------------------------------
                // Generate Test ID
                //---------------------------------------

                if
                (
                    string.IsNullOrWhiteSpace(
                    model.TestID)
                )
                {
                    model.TestID =
                        GenerateTestID();
                }

                //---------------------------------------
                // Duplicate Check
                //---------------------------------------

                string sqlCheck =
                    "SELECT COUNT(*) " +
                    "FROM TestMaster " +
                    "WHERE " +
                    "TrainingID=@TrainingID " +
                    "AND TestTitle=@TestTitle " +
                    "AND AssessmentLevel=@AssessmentLevel";

                SqlParameter[] checkParam =
                {
            new SqlParameter(
                "@TrainingID",
                model.TrainingID),

            new SqlParameter(
                "@TestTitle",
                model.TestTitle),

            new SqlParameter(
                "@AssessmentLevel",
                model.AssessmentLevel)
        };

                if
                (
                    objCommon.GetCount(
                    sqlCheck,
                    checkParam) > 0
                )
                {
                    message =
                        Messages.RecordExists;

                    return false;
                }

                //---------------------------------------
                // Default Values
                //---------------------------------------

                if
                (
                    model.PassingPercentage <= 0
                    &&
                    model.TotalMarks > 0
                )
                {
                    model.PassingPercentage =
                        Math.Round
                        (
                            (
                            model.PassingMarks
                            /
                            model.TotalMarks
                            )
                            * 100,
                            2
                        );
                }

                //---------------------------------------
                // Save
                //---------------------------------------

                string sql =
                    GetInsertAssessmentSQL();

                SqlParameter[] param =
                    CreateInsertParameters(
                    model);

                int i =
                    objDB.ExecuteSql(
                    sql,
                    param);

                if (i > 0)
                {
                    message =
                        Messages.SaveSuccess;

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

        private string GetInsertAssessmentSQL()
        {
            return

            "INSERT INTO TestMaster " +

            "(" +

            "TestID," +
            "TrainingID," +
            "SessionID," +
            "TopicID," +
            "TrainerID," +
            "TestType," +
            "TestTitle," +
            "Duration," +
            "TotalQuestions," +
            "TotalMarks," +
            "PassingMarks," +
            "PassingPercentage," +
            "RandomQuestion," +
            "ShuffleOption," +
            "TestStatus," +
            "CreatedOn," +
            "CreatedBy," +
            "AllowRetest," +
            "MaxAttempt," +
            "ShowResultImmediately," +
            "ShowCorrectAnswer," +
            "NegativeMarking," +
            "StartDateTime," +
            "EndDateTime," +
            "AssessmentScope," +
            "AssessmentLevel," +
            "ConductedByRole," +
            "QuestionSelectionMode," +
            "AllowResume," +
            "AllowReview," +
            "IsPublished," +
            "IsActive," +
            "Published," +
            "Closed" +

            ")" +

            " VALUES " +

            "(" +

            "@TestID," +
            "@TrainingID," +
            "@SessionID," +
            "@TopicID," +
            "@TrainerID," +
            "@TestType," +
            "@TestTitle," +
            "@Duration," +
            "@TotalQuestions," +
            "@TotalMarks," +
            "@PassingMarks," +
            "@PassingPercentage," +
            "@RandomQuestion," +
            "@ShuffleOption," +
            "'Draft'," +
            "GETDATE()," +
            "@CreatedBy," +
            "@AllowRetest," +
            "@MaxAttempt," +
            "@ShowResultImmediately," +
            "@ShowCorrectAnswer," +
            "@NegativeMarking," +
            "@StartDateTime," +
            "@EndDateTime," +
            "@AssessmentScope," +
            "@AssessmentLevel," +
            "@ConductedByRole," +
            "@QuestionSelectionMode," +
            "@AllowResume," +
            "@AllowReview," +
            "0," +
            "1," +
            "0," +
            "0" +

            ")";
        }

        private SqlParameter[] CreateInsertParameters(
    AssessmentModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(new SqlParameter("@TestID", model.TestID));

            param.Add(new SqlParameter("@TrainingID", model.TrainingID));

            param.Add(new SqlParameter("@SessionID",
                string.IsNullOrWhiteSpace(model.SessionID)
                ? (object)DBNull.Value
                : model.SessionID));

            param.Add(new SqlParameter("@TopicID",
                DBNull.Value));

            param.Add(new SqlParameter("@TrainerID",
                string.IsNullOrWhiteSpace(model.TrainerID)
                ? (object)DBNull.Value
                : model.TrainerID));

            param.Add(new SqlParameter("@TestType",
                model.AssessmentLevel));

            param.Add(new SqlParameter("@TestTitle",
                model.TestTitle));

            param.Add(new SqlParameter("@Duration",
                model.Duration));

            param.Add(new SqlParameter("@TotalQuestions",
                model.TotalQuestions));

            param.Add(new SqlParameter("@TotalMarks",
                model.TotalMarks));

            param.Add(new SqlParameter("@PassingMarks",
                model.PassingMarks));

            param.Add(new SqlParameter("@PassingPercentage",
                model.PassingPercentage));

            param.Add(new SqlParameter("@RandomQuestion",
                model.RandomQuestion));

            param.Add(new SqlParameter("@ShuffleOption",
                model.ShuffleOption));

            param.Add(new SqlParameter("@CreatedBy",
                model.CreatedBy));

            param.Add(new SqlParameter("@AllowRetest",
                model.AllowRetest));

            param.Add(new SqlParameter("@MaxAttempt",
                model.MaxAttempt));

            param.Add(new SqlParameter("@ShowResultImmediately",
                model.ShowResultImmediately));

            param.Add(new SqlParameter("@ShowCorrectAnswer",
                model.ShowCorrectAnswer));

            param.Add(new SqlParameter("@NegativeMarking",
                model.NegativeMarking));

            param.Add(new SqlParameter("@StartDateTime",
                model.StartDateTime.HasValue
                ? (object)model.StartDateTime.Value
                : DBNull.Value));

            param.Add(new SqlParameter("@EndDateTime",
                model.EndDateTime.HasValue
                ? (object)model.EndDateTime.Value
                : DBNull.Value));

            param.Add(new SqlParameter("@AssessmentScope",
                model.AssessmentScope));

            param.Add(new SqlParameter("@AssessmentLevel",
                model.AssessmentLevel));

            param.Add(new SqlParameter("@ConductedByRole",
                model.ConductedByRole));

            param.Add(new SqlParameter("@QuestionSelectionMode",
                model.QuestionSelectionMode));

            param.Add(new SqlParameter("@AllowResume",
                model.AllowResume));

            param.Add(new SqlParameter("@AllowReview",
                model.AllowReview));

            return
                param.ToArray();
        }
        private string GetUpdateAssessmentSQL()
        {
            return

            "UPDATE TestMaster SET " +

            "SessionID=@SessionID," +
            "TrainerID=@TrainerID," +
            "TestTitle=@TestTitle," +
            "Duration=@Duration," +
            "TotalQuestions=@TotalQuestions," +
            "TotalMarks=@TotalMarks," +
            "PassingMarks=@PassingMarks," +
            "PassingPercentage=@PassingPercentage," +
            "RandomQuestion=@RandomQuestion," +
            "ShuffleOption=@ShuffleOption," +
            "AllowRetest=@AllowRetest," +
            "MaxAttempt=@MaxAttempt," +
            "ShowResultImmediately=@ShowResultImmediately," +
            "ShowCorrectAnswer=@ShowCorrectAnswer," +
            "NegativeMarking=@NegativeMarking," +
            "StartDateTime=@StartDateTime," +
            "EndDateTime=@EndDateTime," +
            "AssessmentScope=@AssessmentScope," +
            "AssessmentLevel=@AssessmentLevel," +
            "ConductedByRole=@ConductedByRole," +
            "QuestionSelectionMode=@QuestionSelectionMode," +
            "AllowResume=@AllowResume," +
            "AllowReview=@AllowReview," +
            "ModifiedOn=GETDATE()," +
            "ModifiedBy=@ModifiedBy " +

            "WHERE TestID=@TestID";
        }
        private SqlParameter[] CreateUpdateParameters(
    AssessmentModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                "@TestID",
                model.TestID));

            param.Add(
                new SqlParameter(
                "@SessionID",
                string.IsNullOrWhiteSpace(model.SessionID)
                ?
                (object)DBNull.Value
                :
                model.SessionID));

            param.Add(
                new SqlParameter(
                "@TrainerID",
                string.IsNullOrWhiteSpace(model.TrainerID)
                ?
                (object)DBNull.Value
                :
                model.TrainerID));

            param.Add(
                new SqlParameter(
                "@TestTitle",
                model.TestTitle));

            param.Add(
                new SqlParameter(
                "@Duration",
                model.Duration));

            param.Add(
                new SqlParameter(
                "@TotalQuestions",
                model.TotalQuestions));

            param.Add(
                new SqlParameter(
                "@TotalMarks",
                model.TotalMarks));

            param.Add(
                new SqlParameter(
                "@PassingMarks",
                model.PassingMarks));

            param.Add(
                new SqlParameter(
                "@PassingPercentage",
                model.PassingPercentage));

            param.Add(
                new SqlParameter(
                "@RandomQuestion",
                model.RandomQuestion));

            param.Add(
                new SqlParameter(
                "@ShuffleOption",
                model.ShuffleOption));

            param.Add(
                new SqlParameter(
                "@AllowRetest",
                model.AllowRetest));

            param.Add(
                new SqlParameter(
                "@MaxAttempt",
                model.MaxAttempt));

            param.Add(
                new SqlParameter(
                "@ShowResultImmediately",
                model.ShowResultImmediately));

            param.Add(
                new SqlParameter(
                "@ShowCorrectAnswer",
                model.ShowCorrectAnswer));

            param.Add(
                new SqlParameter(
                "@NegativeMarking",
                model.NegativeMarking));

            param.Add(
                new SqlParameter(
                "@StartDateTime",
                model.StartDateTime.HasValue
                ?
                (object)model.StartDateTime.Value
                :
                DBNull.Value));

            param.Add(
                new SqlParameter(
                "@EndDateTime",
                model.EndDateTime.HasValue
                ?
                (object)model.EndDateTime.Value
                :
                DBNull.Value));

            param.Add(
                new SqlParameter(
                "@AssessmentScope",
                model.AssessmentScope));

            param.Add(
                new SqlParameter(
                "@AssessmentLevel",
                model.AssessmentLevel));

            param.Add(
                new SqlParameter(
                "@ConductedByRole",
                model.ConductedByRole));

            param.Add(
                new SqlParameter(
                "@QuestionSelectionMode",
                model.QuestionSelectionMode));

            param.Add(
                new SqlParameter(
                "@AllowResume",
                model.AllowResume));

            param.Add(
                new SqlParameter(
                "@AllowReview",
                model.AllowReview));

            param.Add(
                new SqlParameter(
                "@ModifiedBy",
                model.UpdatedBy));

            return
                param.ToArray();
        }
        #region Publish Assessment

        public bool PublishAssessment(
            string testID,
            string userID,
            out string message)
        {
            message = "";

            try
            {
                if
                (
                    !TestExists(
                    testID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                if
                (
                    !CanEditAssessment(
                    testID)
                )
                {
                    message =
                        "Assessment cannot be published.";

                    return false;
                }

                string sql =

                    "UPDATE TestMaster SET " +

                    "Published=1," +
                    "IsPublished=1," +
                    "TestStatus='Published'," +
                    "PublishedOn=GETDATE()," +
                    "PublishedBy=@UserID," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@UserID " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@UserID",
                userID),

            new SqlParameter(
                "@TestID",
                testID)
        };

                int i =
                    objDB.ExecuteSql(
                    sql,
                    param);

                if (i > 0)
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
        #region Close Assessment

        public bool CloseAssessment(
            string testID,
            string userID,
            out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TestMaster SET " +

                    "Closed=1," +
                    "TestStatus='Closed'," +
                    "ClosedOn=GETDATE()," +
                    "ClosedBy=@UserID," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@UserID " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@UserID",
                userID),

            new SqlParameter(
                "@TestID",
                testID)
        };

                int i =
                    objDB.ExecuteSql(
                    sql,
                    param);

                if (i > 0)
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
        #region Delete Assessment

        public bool DeleteAssessment(
            string testID,
            out string message)
        {
            message = "";


            if
            (
                !CanDeleteAssessment(
                testID)
            )
            {
                message =
                    "Assessment cannot be deleted.";

                return false;
            }

            //---------------------------------------
            // Attempt Check
            //---------------------------------------

            string sqlAttempt =

                "SELECT COUNT(*) " +

                "FROM TestAttempt " +

                "WHERE TestID=@TestID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@TestID",
                testID)
        };

            if
            (
                objCommon.GetCount(
                sqlAttempt,
                param) > 0
            )
            {
                message =
                    "Assessment already attempted.";

                return false;
            }
            objDB.OpenConnection();

            objDB.BeginTransaction();

            try
            {
                //---------------------------------------
                // Delete Questions
                //---------------------------------------

                objDB.ExecuteSql
            (
                "DELETE FROM TestQuestion WHERE TestID=@TestID",
                param, objDB.Transaction);


                objDB.ExecuteSql
                (
                    "DELETE FROM TestTopicMapping WHERE TestID=@TestID",
                    param, objDB.Transaction);


                //---------------------------------------
                // Delete Test
                //---------------------------------------

                int i =
                    objDB.ExecuteSql
                    (
                        "DELETE FROM TestMaster WHERE TestID=@TestID",
                        param, objDB.Transaction);


                if (i > 0)
                {
                    message =
                        Messages.DeleteSuccess;

                    return true;
                }

                message =
                    Messages.DatabaseError;

                return false;
            }
            catch (Exception ex)
            {
                objDB.Rollback();

                message =
       ex.Message;


                throw;
            }
        }

        #endregion
        #region Can Edit Assessment

        public bool CanEditAssessment(
            string testID)
        {
            DataTable dt =
                GetTestByID(
                testID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            if
            (
                Convert.ToBoolean(
                dt.Rows[0]["Published"])
            )
            {
                return false;
            }

            if
            (
                Convert.ToBoolean(
                dt.Rows[0]["Closed"])
            )
            {
                return false;
            }

            return true;
        }

        #endregion
        #region Can Delete Assessment

        public bool CanDeleteAssessment(
            string testID)
        {
            DataTable dt =
                GetTestByID(
                testID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            if
            (
                Convert.ToBoolean(
                dt.Rows[0]["Published"])
            )
            {
                return false;
            }

            if
            (
                Convert.ToBoolean(
                dt.Rows[0]["Closed"])
            )
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}