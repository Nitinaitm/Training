using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Training.Common;
using Training.Helper;
using Training.Models;

namespace Training.Business.Assessment
{
    public class ResultManager
    {
        private clsDataAccess objDB;

        private CommonFunctions objCommon;

        private IDGenerator objID;

        public ResultManager()
        {
            objDB =
                new clsDataAccess();

            objCommon =
                new CommonFunctions();

            objID =
                new IDGenerator();
        }
        public bool ResultExists(
    string attemptID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TestResult " +

                "WHERE AttemptID=@AttemptID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@AttemptID",
            attemptID)
    };

            return
                objCommon.GetCount(
                sql,
                param) > 0;
        }
        public DataTable GetResult(
    string resultID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE ResultID=@ResultID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@ResultID",
            resultID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }
        public DataTable GetResultByAttempt(
    string attemptID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE AttemptID=@AttemptID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@AttemptID",
            attemptID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }
        public decimal CalculatePercentage(
    decimal obtainedMarks,
    decimal totalMarks)
        {
            if
            (
                totalMarks <= 0
            )
            {
                return 0;
            }

            return
                Math.Round
                (
                    (
                        obtainedMarks
                        /
                        totalMarks
                    )
                    * 100,
                    2
                );
        }
        public string CalculateResultStatus(
    decimal percentage,
    decimal passingPercentage)
        {
            if
            (
                percentage >=
                passingPercentage
            )
            {
                return
                    Constants.ResultStatus.Pass;
            }

            return
                Constants.ResultStatus.Fail;
        }
        public bool SaveResult(
    ResultModel model,
    out string message)
        {
            message = "";

            try
            {
                //------------------------------------

                if
                (
                    ResultExists(
                    model.AttemptID)
                )
                {
                    message =
                        Messages.RecordExists;

                    return false;
                }

                //------------------------------------

                model.ResultID =
                    objID.GenerateResultID();

                //------------------------------------

                string sql =

                    "INSERT INTO TestResult " +

                    "(" +

                    "ResultID," +

                    "AttemptID," +

                    "TestID," +

                    "EmpID," +

                    "ObtainedMarks," +

                    "TotalMarks," +

                    "Percentage," +

                    "ResultStatus," +

                    "CreatedOn," +

                    "CreatedBy" +

                    ")" +

                    " VALUES " +

                    "(" +

                    "@ResultID," +

                    "@AttemptID," +

                    "@TestID," +

                    "@EmpID," +

                    "@ObtainedMarks," +

                    "@TotalMarks," +

                    "@Percentage," +

                    "@ResultStatus," +

                    "GETDATE()," +

                    "@CreatedBy" +

                    ")";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@ResultID",
                model.ResultID),

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
                "@ObtainedMarks",
                model.ObtainedMarks),

            new SqlParameter(
                "@TotalMarks",
                model.TotalMarks),

            new SqlParameter(
                "@Percentage",
                model.Percentage),

            new SqlParameter(
                "@ResultStatus",
                model.ResultStatus),

            new SqlParameter(
                "@CreatedBy",
                model.CreatedBy)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) > 0
                )
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
        public bool UpdateResult(
    ResultModel model,
    out string message)
        {
            message = "";

            try
            {
                if
                (
                    !ResultExists(
                    model.AttemptID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                model.Percentage =
    CalculatePercentage(
    model.ObtainedMarks,
    model.TotalMarks);

                model.ResultStatus =
                    CalculateResultStatus(
                    model.Percentage,
                    GetPassingPercentage(
                    model.TestID));

                string sql =

                    "UPDATE TestResult SET " +

                    "ObtainedMarks=@ObtainedMarks," +

                    "TotalMarks=@TotalMarks," +

                    "Percentage=@Percentage," +

                    "ResultStatus=@ResultStatus " +

                    "WHERE AttemptID=@AttemptID";

                SqlParameter[] param =
                {
            new SqlParameter("@ObtainedMarks",model.ObtainedMarks),
            new SqlParameter("@TotalMarks",model.TotalMarks),
            new SqlParameter("@Percentage",model.Percentage),
            new SqlParameter("@ResultStatus",model.ResultStatus),
            new SqlParameter("@AttemptID",model.AttemptID)
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
        public DataTable GetResultByEmployee(
    string empID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE EmpID=@EmpID " +

                "ORDER BY CreatedOn DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@EmpID",
            empID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        public DataTable GetResultByTest(
    string testID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE TestID=@TestID " +

                "ORDER BY Percentage DESC";

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

        public DataTable GetTrainingResults(
    string trainingID)
        {
            string sql =

                "SELECT " +

                "TR.* " +

                "FROM TestResult TR " +

                "INNER JOIN TestMaster TM " +

                "ON TM.TestID=TR.TestID " +

                "WHERE TM.TrainingID=@TrainingID";

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

        public DataTable GetSessionResults(
    string sessionID)
        {
            string sql =

                "SELECT " +

                "TR.* " +

                "FROM TestResult TR " +

                "INNER JOIN TestMaster TM " +

                "ON TM.TestID=TR.TestID " +

                "WHERE TM.SessionID=@SessionID";

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

        public bool IsCertificateEligible(
    decimal percentage,
    decimal passingPercentage)
        {
            return
                percentage >=
                passingPercentage;
        }

        private decimal GetPassingPercentage(
    string testID)
        {
            string sql =

                "SELECT PassingPercentage " +

                "FROM TestMaster " +

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
                return 0;
            }

            return
                Convert.ToDecimal(
                obj);
        }
        public bool PublishResult(
    string testID,
    out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TestResult SET " +

                    "Published=1 " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
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
        public bool UnPublishResult(
    string testID,
    out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TestResult SET " +

                    "Published=0 " +

                    "WHERE TestID=@TestID";

                SqlParameter[] param =
                {
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
        public bool DeleteResult(
    string resultID,
    out string message)
        {
            message = "";

            try
            {
                string sql =

                    "DELETE FROM TestResult " +

                    "WHERE ResultID=@ResultID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@ResultID",
                resultID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) > 0
                )
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
        public DataTable GetTopScorers(
    string testID,
    int top)
        {
            string sql =

                "SELECT TOP (" +

                top +

                ") * " +

                "FROM TestResult " +

                "WHERE TestID=@TestID " +

                "ORDER BY Percentage DESC, ObtainedMarks DESC";

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
        public DataTable GetPassedCandidates(
    string testID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE TestID=@TestID " +

                "AND ResultStatus=@ResultStatus " +

                "ORDER BY Percentage DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID),

        new SqlParameter(
            "@ResultStatus",
            Constants.ResultStatus.Pass)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }
        public DataTable GetFailedCandidates(
    string testID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE TestID=@TestID " +

                "AND ResultStatus=@ResultStatus " +

                "ORDER BY Percentage";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TestID",
            testID),

        new SqlParameter(
            "@ResultStatus",
            Constants.ResultStatus.Fail)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }
        public DataTable GetRankList(
    string testID)
        {
            string sql =

                "SELECT * " +

                "FROM TestResult " +

                "WHERE TestID=@TestID " +

                "ORDER BY Percentage DESC, ObtainedMarks DESC";

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
    }
}