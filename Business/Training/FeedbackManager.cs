using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Training.Helper;
using Training.Models;

namespace Training.Business.Training
{
    public class FeedbackManager
    {
        private clsDataAccess objDB;

        private IDGenerator objID;

        private CommonFunctions objCommon;

        public FeedbackManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }


        #region Get Methods

        public DataTable GetFeedbackByID(
            string feedbackID)
        {
            string sql =
                "SELECT * " +
                "FROM Feedback " +
                "WHERE FeedbackID=@FeedbackID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@FeedbackID",
                    feedbackID)
            };

            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        public DataTable GetFeedbackByTrainingEmployee(
            string trainingID,
            string empID)
        {
            string sql =
                "SELECT * " +
                "FROM Feedback " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };

            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        public DataTable GetFeedbackDetails(
            string feedbackID)
        {
            string sql =
                @"
                SELECT
                    FeedbackDetailID,
                    FeedbackID,
                    TrainingID,
                    EmpID,
                    CategoryID,
                    QuestionID,
                    TrainerID,
                    TrainerType,
                    AnswerType,
                    Rating,
                    Answer,
                    CreatedOn
                FROM FeedbackDetail
                WHERE FeedbackID=@FeedbackID
                ORDER BY CreatedOn
                ";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@FeedbackID",
                    feedbackID)
            };

            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        public bool FeedbackExists(
            string feedbackID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM Feedback " +
                "WHERE FeedbackID=@FeedbackID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@FeedbackID",
                    feedbackID)
            };

            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }


        public bool IsFeedbackSubmitted(
            string trainingID,
            string empID)
        {
            string sql =
                @"
                SELECT COUNT(*)
                FROM Feedback
                WHERE TrainingID=@TrainingID
                AND EmpID=@EmpID
                AND Submitted=1
                ";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };

            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }

        #endregion


        #region ID Generation

        public string GenerateFeedbackID()
        {
            return
                Guid.NewGuid()
                .ToString("N")
                .ToUpper();
        }


        public string GenerateFeedbackDetailID()
        {
            return
                Guid.NewGuid()
                .ToString("N")
                .ToUpper();
        }

        #endregion


        #region Validation

        public string ValidateFeedback(
            string trainingID,
            string empID)
        {
            if (String.IsNullOrWhiteSpace(
                trainingID))
            {
                return
                    "Training ID is required.";
            }


            if (String.IsNullOrWhiteSpace(
                empID))
            {
                return
                    "Employee ID is required.";
            }


            return "";
        }


        public string ValidateFeedbackDetail(
            FeedbackDetailModel model)
        {
            if (model == null)
            {
                return
                    "Feedback detail is required.";
            }


            if (String.IsNullOrWhiteSpace(
                model.FeedbackID))
            {
                return
                    "Feedback ID is required.";
            }


            if (String.IsNullOrWhiteSpace(
                model.TrainingID))
            {
                return
                    "Training ID is required.";
            }


            if (String.IsNullOrWhiteSpace(
                model.EmpID))
            {
                return
                    "Employee ID is required.";
            }


            if (String.IsNullOrWhiteSpace(
                model.QuestionID))
            {
                return
                    "Question ID is required.";
            }


            if (String.Equals(
                model.AnswerType,
                "Rating",
                StringComparison.OrdinalIgnoreCase))
            {
                if (!model.Rating.HasValue)
                {
                    return
                        "Rating is required.";
                }


                if (model.Rating.Value < 1 ||
                    model.Rating.Value > 5)
                {
                    return
                        "Rating must be between 1 and 5.";
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(
                    model.Answer))
                {
                    return
                        "Answer is required.";
                }
            }


            return "";
        }

        #endregion


        #region Save Feedback

        public bool SaveFeedback(
            string trainingID,
            string empID,
            out string feedbackID,
            out string message)
        {
            feedbackID = "";
            message = "";


            try
            {
                message =
                    ValidateFeedback(
                        trainingID,
                        empID);


                if (message != "")
                {
                    return false;
                }


                if (IsFeedbackSubmitted(
                    trainingID,
                    empID))
                {
                    message =
                        "Feedback already submitted.";

                    return false;
                }


                feedbackID =
                    GenerateFeedbackID();


                string sql =
                    @"
                    INSERT INTO Feedback
                    (
                        FeedbackID,
                        TrainingID,
                        EmpID,
                        Submitted,
                        SubmittedOn,
                        CreatedOn,
                        CreatedBy
                    )
                    VALUES
                    (
                        @FeedbackID,
                        @TrainingID,
                        @EmpID,
                        1,
                        GETDATE(),
                        GETDATE(),
                        @EmpID
                    )
                    ";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@FeedbackID",
                        feedbackID),

                    new SqlParameter(
                        "@TrainingID",
                        trainingID),

                    new SqlParameter(
                        "@EmpID",
                        empID)
                };


                objDB.ExecuteSql(
                    sql,
                    param);


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


        #region Save Feedback Detail

        public bool SaveFeedbackDetail(
            FeedbackDetailModel model,
            out string message)
        {
            message = "";


            try
            {
                message =
                    ValidateFeedbackDetail(
                        model);


                if (message != "")
                {
                    return false;
                }


                if (String.IsNullOrWhiteSpace(
                    model.FeedbackDetailID))
                {
                    model.FeedbackDetailID =
                        GenerateFeedbackDetailID();
                }


                string sql =
                    @"
                    INSERT INTO FeedbackDetail
                    (
                        FeedbackDetailID,
                        FeedbackID,
                        TrainingID,
                        EmpID,
                        CategoryID,
                        QuestionID,
                        TrainerID,
                        TrainerType,
                        AnswerType,
                        Rating,
                        Answer,
                        CreatedOn
                    )
                    VALUES
                    (
                        @FeedbackDetailID,
                        @FeedbackID,
                        @TrainingID,
                        @EmpID,
                        @CategoryID,
                        @QuestionID,
                        @TrainerID,
                        @TrainerType,
                        @AnswerType,
                        @Rating,
                        @Answer,
                        GETDATE()
                    )
                    ";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@FeedbackDetailID",
                        model.FeedbackDetailID),

                    new SqlParameter(
                        "@FeedbackID",
                        model.FeedbackID),

                    new SqlParameter(
                        "@TrainingID",
                        model.TrainingID),

                    new SqlParameter(
                        "@EmpID",
                        model.EmpID),

                    new SqlParameter(
                        "@CategoryID",
                        String.IsNullOrWhiteSpace(
                            model.CategoryID)
                        ? (object)DBNull.Value
                        : model.CategoryID),

                    new SqlParameter(
                        "@QuestionID",
                        model.QuestionID),

                    new SqlParameter(
                        "@TrainerID",
                        String.IsNullOrWhiteSpace(
                            model.TrainerID)
                        ? (object)DBNull.Value
                        : model.TrainerID),

                    new SqlParameter(
                        "@TrainerType",
                        String.IsNullOrWhiteSpace(
                            model.TrainerType)
                        ? (object)DBNull.Value
                        : model.TrainerType),

                    new SqlParameter(
                        "@AnswerType",
                        model.AnswerType),

                    new SqlParameter(
                        "@Rating",
                        model.Rating.HasValue
                        ? (object)model.Rating.Value
                        : DBNull.Value),

                    new SqlParameter(
                        "@Answer",
                        String.IsNullOrWhiteSpace(
                            model.Answer)
                        ? (object)DBNull.Value
                        : model.Answer)
                };


                objDB.ExecuteSql(
                    sql,
                    param);


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


        #region Training Progress

        public bool UpdateFeedbackProgress(
            string trainingID,
            string empID,
            out string message)
        {
            message = "";


            try
            {
                if (String.IsNullOrWhiteSpace(
                    trainingID))
                {
                    message =
                        "Training ID is required.";

                    return false;
                }


                if (String.IsNullOrWhiteSpace(
                    empID))
                {
                    message =
                        "Employee ID is required.";

                    return false;
                }


                string sql =
                    @"
                    UPDATE TrainingProgress
                    SET
                        FeedbackCompleted=1,
                        WorkflowStatus='F',
                        UpdatedOn=GETDATE(),
                        UpdatedBy=@EmpID
                    WHERE
                        TrainingID=@TrainingID
                        AND EmpID=@EmpID
                    ";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@TrainingID",
                        trainingID),

                    new SqlParameter(
                        "@EmpID",
                        empID)
                };


                objDB.ExecuteSql(
                    sql,
                    param);


                return true;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }


        public bool UpdateBatchFeedbackProgress(
            string trainingID,
            string empID,
            out string message)
        {
            message = "";


            try
            {
                if (String.IsNullOrWhiteSpace(
                    trainingID))
                {
                    message =
                        "Training ID is required.";

                    return false;
                }


                if (String.IsNullOrWhiteSpace(
                    empID))
                {
                    message =
                        "Employee ID is required.";

                    return false;
                }


                string sql =
                    @"
                    UPDATE TrainingProgress
                    SET
                        BatchFeedbackCompleted=1,
                        UpdatedOn=GETDATE(),
                        UpdatedBy=@EmpID
                    WHERE
                        TrainingID=@TrainingID
                        AND EmpID=@EmpID
                    ";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@TrainingID",
                        trainingID),

                    new SqlParameter(
                        "@EmpID",
                        empID)
                };


                objDB.ExecuteSql(
                    sql,
                    param);


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
    }
}