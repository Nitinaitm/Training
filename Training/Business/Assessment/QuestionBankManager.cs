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


    public class QuestionBankManager
    {
        private clsDataAccess objDB;

        private CommonFunctions objCommon;

        private IDGenerator objID;

        public QuestionBankManager()
        {
            objDB =
                new clsDataAccess();

            objCommon =
                new CommonFunctions();

            objID =
                new IDGenerator();
        }

        #region Generate Question ID

        public string GenerateQuestionID()
        {
            return
                objID.GenerateQuestionID();
        }

        #endregion

        #region Get Question By ID

        public DataTable GetQuestionByID(
            string questionID)
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE QuestionID=@QuestionID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@QuestionID",
                questionID)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get All Questions

        public DataTable GetQuestions()
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "ORDER BY CreatedOn DESC";

            return
                objDB.GetDataTable(
                sql);
        }

        #endregion

        #region Get Questions By Course

        public DataTable GetQuestionsByCourse(
            string courseID)
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE CourseID=@CourseID " +

                "ORDER BY TopicID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@CourseID",
                courseID)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Questions By Topic

        public DataTable GetQuestionsByTopic(
            string topicID)
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE WHERE CourseID=@CourseID AND TopicID = @TopicID " +

                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@TopicID",
                topicID)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Questions By Difficulty

        public DataTable GetQuestionsByDifficulty(
            string difficulty)
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE DifficultyLevel=@DifficultyLevel " +

                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@DifficultyLevel",
                difficulty)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Questions By Owner

        public DataTable GetQuestionsByOwner(
            string ownerID)
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE OwnerID=@OwnerID " +

                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@OwnerID",
                ownerID)
        };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Question Exists

        public bool QuestionExists(
            string questionID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM QuestionBank " +

                "WHERE QuestionID=@QuestionID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@QuestionID",
                questionID)
        };

            return
                objCommon.GetCount(
                sql,
                param) > 0;
        }

        #endregion


        #region Validation

        public string ValidateQuestion(
            QuestionModel model)
        {
            if
            (
                !Validator.Required(
                model.CourseID)
            )
            {
                return
                    "Course is required.";
            }

            if
            (
                !Validator.Required(
                model.TopicID)
            )
            {
                return
                    "Topic is required.";
            }

            if
            (
                !Validator.Required(
                model.Question)
            )
            {
                return
                    "Question is required.";
            }

            if
            (
                !Validator.Required(
                model.OptionA)
            )
            {
                return
                    "Option A is required.";
            }

            if
            (
                !Validator.Required(
                model.OptionB)
            )
            {
                return
                    "Option B is required.";
            }

            if
            (
                !Validator.Required(
                model.OptionC)
            )
            {
                return
                    "Option C is required.";
            }

            if
            (
                !Validator.Required(
                model.OptionD)
            )
            {
                return
                    "Option D is required.";
            }

            if
            (
                !Validator.Required(
                model.CorrectOption)
            )
            {
                return
                    "Correct Option is required.";
            }

            if
            (
                !Validator.Required(
                model.QuestionType)
            )
            {
                return
                    "Question Type is required.";
            }

            if
            (
                !Validator.Required(
                model.DifficultyLevel)
            )
            {
                return
                    "Difficulty Level is required.";
            }

            if
            (
                model.Marks <= 0
            )
            {
                return
                    "Marks should be greater than zero.";
            }

            return "";
        }

        #endregion
        #region Save Question

        public bool SaveQuestion(
            QuestionModel model,
            out string message)
        {
            message = "";

            try
            {
                //------------------------------------

                message =
                    ValidateQuestion(
                    model);

                if (message != "")
                {
                    return false;
                }

                //------------------------------------

                if
                (
                    String.IsNullOrWhiteSpace(
                    model.QuestionID)
                )
                {
                    model.QuestionID =
                        objID.GenerateQuestionID();
                }

                //------------------------------------

                string sqlCheck =

                    "SELECT COUNT(*) " +

                    "FROM QuestionBank " +

                    "WHERE " +

                    "CourseID=@CourseID " +

                    "AND TopicID=@TopicID " +

                    "AND Question=@Question " +
                    "AND IsActive = 1 ";

                SqlParameter[] checkParam =
                {
            new SqlParameter(
                "@CourseID",
                model.CourseID),

            new SqlParameter(
                "@TopicID",
                model.TopicID),

            new SqlParameter(
                "@Question",
                model.Question)
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

                //------------------------------------

                string sql =
                    GetInsertQuestionSQL();

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

        private string GetInsertQuestionSQL()
        {
            return

            "INSERT INTO QuestionBank " +

            "(" +

            "QuestionID," +
            "QuestionOwnerType," +
            "OwnerID," +
            "CourseID," +
            "TopicID," +
            "Question," +
            "OptionA," +
            "OptionB," +
            "OptionC," +
            "OptionD," +
            "CorrectOption," +
            "DifficultyLevel," +
            "Marks," +
            "Explanation," +
            "IsActive," +
            "CreatedOn," +
            "CreatedBy," +
            "QuestionType," +
            "NegativeMarks," +
            "Language," +
            "ImagePath," +
            "ExplanationImage," +
            "ApprovalStatus" +

            ")" +

            " VALUES " +

            "(" +

            "@QuestionID," +
            "@QuestionOwnerType," +
            "@OwnerID," +
            "@CourseID," +
            "@TopicID," +
            "@Question," +
            "@OptionA," +
            "@OptionB," +
            "@OptionC," +
            "@OptionD," +
            "@CorrectOption," +
            "@DifficultyLevel," +
            "@Marks," +
            "@Explanation," +
            "@IsActive," +
            "GETDATE()," +
            "@CreatedBy," +
            "@QuestionType," +
            "@NegativeMarks," +
            "@Language," +
            "@ImagePath," +
            "@ExplanationImage," +
            "@ApprovalStatus" +

            ")";
        }

        private SqlParameter[] CreateInsertParameters(
        QuestionModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(new SqlParameter("@QuestionID", model.QuestionID));
            param.Add(new SqlParameter("@QuestionOwnerType", model.QuestionOwnerType));
            param.Add(new SqlParameter("@OwnerID", model.OwnerID));
            param.Add(new SqlParameter("@CourseID", model.CourseID));
            param.Add(new SqlParameter("@TopicID", model.TopicID));
            param.Add(new SqlParameter("@Question", model.Question));
            param.Add(new SqlParameter("@OptionA", model.OptionA));
            param.Add(new SqlParameter("@OptionB", model.OptionB));
            param.Add(new SqlParameter("@OptionC", model.OptionC));
            param.Add(new SqlParameter("@OptionD", model.OptionD));
            param.Add(new SqlParameter("@CorrectOption", model.CorrectOption));
            param.Add(new SqlParameter("@DifficultyLevel", model.DifficultyLevel));
            param.Add(new SqlParameter("@Marks", model.Marks));
            param.Add(new SqlParameter("@Explanation",
                string.IsNullOrWhiteSpace(model.Explanation)
                ? (object)DBNull.Value
                : model.Explanation));
            param.Add(new SqlParameter("@IsActive", model.IsActive));
            param.Add(new SqlParameter("@CreatedBy", model.CreatedBy));
            param.Add(new SqlParameter("@QuestionType", model.QuestionType));
            param.Add(new SqlParameter("@NegativeMarks", model.NegativeMarks));
            param.Add(new SqlParameter("@Language", model.Language));
            param.Add(new SqlParameter("@ImagePath",
                string.IsNullOrWhiteSpace(model.ImagePath)
                ? (object)DBNull.Value
                : model.ImagePath));
            param.Add(new SqlParameter("@ExplanationImage",
                string.IsNullOrWhiteSpace(model.ExplanationImage)
                ? (object)DBNull.Value
                : model.ExplanationImage));
            param.Add(new SqlParameter("@ApprovalStatus", model.ApprovalStatus));

            return param.ToArray();
        }
        #region Update Question

        public bool UpdateQuestion(
            QuestionModel model,
            out string message)
        {
            message = "";

            try
            {
                //------------------------------------
                // Validation
                //------------------------------------

                message =
                    ValidateQuestion(
                    model);

                if (message != "")
                {
                    return false;
                }

                //------------------------------------
                // Exists
                //------------------------------------

                if
                (
                    !QuestionExists(
                    model.QuestionID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                //------------------------------------
                // Can Edit
                //------------------------------------

                if
                (
                    !CanEditQuestion(
                    model.QuestionID)
                )
                {
                    message =
                        "Question cannot be edited.";

                    return false;
                }

                //------------------------------------
                // Update
                //------------------------------------

                string sql =
                    GetUpdateQuestionSQL();

                SqlParameter[] param =
                    CreateUpdateParameters(
                    model);

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

        #region Delete Question

        public bool DeleteQuestion(
            string questionID,
            out string message)
        {
            message = "";

            try
            {
                if
                (
                    !CanDeleteQuestion(
                    questionID)
                )
                {
                    message =
                        "Question cannot be deleted.";

                    return false;
                }

                //------------------------------------
                // Used In Test
                //------------------------------------

                string sqlCheck =

                    "SELECT COUNT(*) " +

                    "FROM TestQuestion " +

                    "WHERE QuestionID=@QuestionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@QuestionID",
                questionID)
        };

                if
                (
                    objCommon.GetCount(
                    sqlCheck,
                    param) > 0
                )
                {
                    message =
                        "Question already used in test.";

                    return false;
                }

                //------------------------------------

                int i =
                    objDB.ExecuteSql
                    (
                        "DELETE FROM QuestionBank " +
                        "WHERE QuestionID=@QuestionID",
                        param
                    );

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
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion
        #region Can Edit Question

        public bool CanEditQuestion(
            string questionID)
        {
            DataTable dt =
                GetQuestionByID(
                questionID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string status =
                dt.Rows[0]
                ["ApprovalStatus"]
                .ToString();

            if
            (
                status ==
                Constants.QuestionApproval.Approved
            )
            {
                return false;
            }

            return true;
        }

        #endregion
        #region Can Delete Question

        public bool CanDeleteQuestion(
            string questionID)
        {
            DataTable dt =
                GetQuestionByID(
                questionID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string status =
                dt.Rows[0]
                ["ApprovalStatus"]
                .ToString();

            if
            (
                status ==
                Constants.QuestionApproval.Approved
            )
            {
                return false;
            }

            return true;
        }

        #endregion
        private string GetUpdateQuestionSQL()
        {
            return

            "UPDATE QuestionBank SET " +

            "CourseID=@CourseID," +
            "TopicID=@TopicID," +
            "Question=@Question," +
            "OptionA=@OptionA," +
            "OptionB=@OptionB," +
            "OptionC=@OptionC," +
            "OptionD=@OptionD," +
            "CorrectOption=@CorrectOption," +
            "DifficultyLevel=@DifficultyLevel," +
            "Marks=@Marks," +
            "Explanation=@Explanation," +
            "QuestionType=@QuestionType," +
            "NegativeMarks=@NegativeMarks," +
            "Language=@Language," +
            "ImagePath=@ImagePath," +
            "ExplanationImage=@ExplanationImage," +
            "ModifiedOn=GETDATE()," +
            "ModifiedBy=@ModifiedBy " +

            "WHERE QuestionID=@QuestionID";
        }
        private SqlParameter[] CreateUpdateParameters(
    QuestionModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(new SqlParameter("@QuestionID", model.QuestionID));
            param.Add(new SqlParameter("@CourseID", model.CourseID));
            param.Add(new SqlParameter("@TopicID", model.TopicID));
            param.Add(new SqlParameter("@Question", model.Question));
            param.Add(new SqlParameter("@OptionA", model.OptionA));
            param.Add(new SqlParameter("@OptionB", model.OptionB));
            param.Add(new SqlParameter("@OptionC", model.OptionC));
            param.Add(new SqlParameter("@OptionD", model.OptionD));
            param.Add(new SqlParameter("@CorrectOption", model.CorrectOption));
            param.Add(new SqlParameter("@DifficultyLevel", model.DifficultyLevel));
            param.Add(new SqlParameter("@Marks", model.Marks));
            param.Add(new SqlParameter("@Explanation",
                String.IsNullOrWhiteSpace(model.Explanation)
                ? (object)DBNull.Value
                : model.Explanation));
            param.Add(new SqlParameter("@QuestionType", model.QuestionType));
            param.Add(new SqlParameter("@NegativeMarks", model.NegativeMarks));
            param.Add(new SqlParameter("@Language", model.Language));
            param.Add(new SqlParameter("@ImagePath",
                String.IsNullOrWhiteSpace(model.ImagePath)
                ? (object)DBNull.Value
                : model.ImagePath));
            param.Add(new SqlParameter("@ExplanationImage",
                String.IsNullOrWhiteSpace(model.ExplanationImage)
                ? (object)DBNull.Value
                : model.ExplanationImage));
            param.Add(new SqlParameter("@ModifiedBy", model.UpdatedBy));

            return
                param.ToArray();
        }
        #region Approve Question

        public bool ApproveQuestion(
            string questionID,
            string approvedBy,
            out string message)
        {
            message = "";

            try
            {
                if
                (
                    !QuestionExists(
                    questionID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =

                    "UPDATE QuestionBank SET " +

                    "ApprovalStatus=@ApprovalStatus," +

                    "ApprovedBy=@ApprovedBy," +

                    "ApprovedOn=GETDATE()," +

                    "ModifiedOn=GETDATE()," +

                    "ModifiedBy=@ApprovedBy " +

                    "WHERE QuestionID=@QuestionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@ApprovalStatus",
                Constants.QuestionApproval.Approved),

            new SqlParameter(
                "@ApprovedBy",
                approvedBy),

            new SqlParameter(
                "@QuestionID",
                questionID)
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
        #region Reject Question

        public bool RejectQuestion(
            string questionID,
            string rejectedBy,
            string reason,
            out string message)
        {
            message = "";

            try
            {
                if
                (
                    !QuestionExists(
                    questionID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =

                    "UPDATE QuestionBank SET " +

                    "ApprovalStatus=@ApprovalStatus," +

                    "ApprovedBy=@ApprovedBy," +

                    "ApprovedOn=GETDATE()," +

                    "RejectionReason=@RejectionReason," +

                    "ModifiedOn=GETDATE()," +

                    "ModifiedBy=@ApprovedBy " +

                    "WHERE QuestionID=@QuestionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@ApprovalStatus",
                Constants.QuestionApproval.Rejected),

            new SqlParameter(
                "@ApprovedBy",
                rejectedBy),

            new SqlParameter(
                "@RejectionReason",
                reason),

            new SqlParameter(
                "@QuestionID",
                questionID)
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
        #region Pending Questions

        public DataTable GetPendingQuestions()
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE ApprovalStatus=@ApprovalStatus " +

                "ORDER BY CreatedOn";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@ApprovalStatus",
            Constants.QuestionApproval.Pending)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion
        #region Approved Questions

        public DataTable GetApprovedQuestions()
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE ApprovalStatus=@ApprovalStatus " +

                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@ApprovalStatus",
            Constants.QuestionApproval.Approved)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion
        #region Rejected Questions

        public DataTable GetRejectedQuestions()
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE ApprovalStatus=@ApprovalStatus " +

                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@ApprovalStatus",
            Constants.QuestionApproval.Rejected)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion
        #region Get Approved Questions By Topic

        public DataTable GetApprovedQuestionsByTopic(
            string courseID,
            string topicID)
        {
            string sql =

                "SELECT * " +

                "FROM QuestionBank " +

                "WHERE CourseID=@CourseID " +

                "AND TopicID=@TopicID " +

                "AND IsActive=1 " +

                "AND ApprovalStatus=@ApprovalStatus";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CourseID",
            courseID),

        new SqlParameter(
            "@TopicID",
            topicID),

        new SqlParameter(
            "@ApprovalStatus",
            Constants.QuestionApproval.Approved)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion
    }
}
