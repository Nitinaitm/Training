using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Training.Helper;
using Training.Common;
using Training.Models;

namespace Training.Business.Training
{


    public class SessionManager
    {
        private clsDataAccess objDB;

        private IDGenerator objID;

        private CommonFunctions objCommon;

        public SessionManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }

        #region Get Methods

        public string GenerateSessionID()
        {
            return
                objID.GenerateSessionID();
        }

        public DataTable GetSessionByID(
            string sessionID)
        {
            string sql =
                "SELECT * " +
                "FROM SessionMaster " +
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

        public DataTable GetSessionsByTraining(
            string trainingID)
        {
            string sql =
                "SELECT * " +
                "FROM SessionMaster " +
                "WHERE TrainingID=@TrainingID " +
                "ORDER BY DisplayOrder";

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

        public bool SessionExists(
            string sessionID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM SessionMaster " +
                "WHERE SessionID=@SessionID";

            SqlParameter[] param =
            {
            new SqlParameter(
                "@SessionID",
                sessionID)
        };

            return
                objCommon.GetCount(
                sql,
                param) > 0;
        }

        #endregion

        #region Validation

        public string ValidateSession(
            SessionModel model)
        {
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
                model.SessionName)
            )
            {
                return
                    "Session Name is required.";
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
                model.TrainerID)
            )
            {
                return
                    "Trainer is required.";
            }

            if
            (
                !model.SessionDate.HasValue
            )
            {
                return
                    "Session Date is required.";
            }

            if
            (
                !model.StartTime.HasValue
            )
            {
                return
                    "Start Time is required.";
            }

            if
            (
                !model.EndTime.HasValue
            )
            {
                return
                    "End Time is required.";
            }

            if
            (
                model.StartTime.Value >=
                model.EndTime.Value
            )
            {
                return
                    "End Time must be greater than Start Time.";
            }

            return "";
        }

        #endregion
        #region Save Session

        public bool SaveSession(
            SessionModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateSession(
                    model);

                if (message != "")
                {
                    return false;
                }

                //---------------------------------------
                // Generate Session ID
                //---------------------------------------

                if
                (
                    string.IsNullOrWhiteSpace(
                    model.SessionID)
                )
                {
                    model.SessionID =
                        GenerateSessionID();
                }

                //---------------------------------------
                // Duplicate Check
                //---------------------------------------

                string sqlCheck =
                    "SELECT COUNT(*) " +
                    "FROM SessionMaster " +
                    "WHERE " +
                    "TrainingID=@TrainingID " +
                    "AND SessionName=@SessionName " +
                    "AND SessionDate=@SessionDate";

                SqlParameter[] checkParam =
                {
            new SqlParameter(
                "@TrainingID",
                model.TrainingID),

            new SqlParameter(
                "@SessionName",
                model.SessionName),

            new SqlParameter(
                "@SessionDate",
                model.SessionDate.Value)
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
                // Calculate Hours
                //---------------------------------------

                model.TotalHours =
                    Convert.ToDecimal
                    (
                        (
                        model.EndTime.Value -
                        model.StartTime.Value
                        ).TotalHours
                    );

                //---------------------------------------
                // Insert
                //---------------------------------------

                string sql =
                    GetInsertSessionSQL();

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

        private string GetInsertSessionSQL()
        {
            return

            "INSERT INTO SessionMaster " +

            "(" +

            "SessionID," +
            "TrainingID," +
            "SessionNo," +
            "SessionName," +
            "SessionDate," +
            "StartTime," +
            "EndTime," +
            "TotalHours," +
            "SessionStatus," +
            "Remarks," +
            "CreatedOn," +
            "CreatedBy," +
            "TopicID," +
            "TrainerID," +
            "DisplayOrder" +

            ")" +

            " VALUES " +

            "(" +

            "@SessionID," +
            "@TrainingID," +
            "@SessionNo," +
            "@SessionName," +
            "@SessionDate," +
            "@StartTime," +
            "@EndTime," +
            "@TotalHours," +
            "@SessionStatus," +
            "@Remarks," +
            "GETDATE()," +
            "@CreatedBy," +
            "@TopicID," +
            "@TrainerID," +
            "@DisplayOrder" +

            ")";
        }

        private SqlParameter[] CreateInsertParameters(
    SessionModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                "@SessionID",
                model.SessionID));

            param.Add(
                new SqlParameter(
                "@TrainingID",
                model.TrainingID));

            param.Add(
                new SqlParameter(
                "@SessionNo",
                model.SessionNo));

            param.Add(
                new SqlParameter(
                "@SessionName",
                model.SessionName));

            param.Add(
                new SqlParameter(
                "@SessionDate",
                model.SessionDate.Value));

            param.Add(
                new SqlParameter(
                "@StartTime",
                model.StartTime.Value));

            param.Add(
                new SqlParameter(
                "@EndTime",
                model.EndTime.Value));

            param.Add(
                new SqlParameter(
                "@TotalHours",
                model.TotalHours));

            param.Add(
                new SqlParameter(
                "@SessionStatus",
                Constants.SessionStatus.Scheduled));

            param.Add(
                new SqlParameter(
                "@Remarks",
                model.Remarks));

            param.Add(
                new SqlParameter(
                "@CreatedBy",
                model.CreatedBy));

            param.Add(
                new SqlParameter(
                "@TopicID",
                model.TopicID));

            param.Add(
                new SqlParameter(
                "@TrainerID",
                model.TrainerID));

            param.Add(
                new SqlParameter(
                "@DisplayOrder",
                model.DisplayOrder));

            return
                param.ToArray();
        }
        #region Update Session

        public bool UpdateSession(
            SessionModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateSession(
                    model);

                if (message != "")
                {
                    return false;
                }

                //---------------------------------------
                // Session Exists
                //---------------------------------------

                if (!SessionExists(
                    model.SessionID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                //---------------------------------------
                // Calculate Hours
                //---------------------------------------

                model.TotalHours =
                    Convert.ToDecimal
                    (
                        (
                        model.EndTime.Value -
                        model.StartTime.Value
                        ).TotalHours
                    );

                //---------------------------------------
                // Update
                //---------------------------------------

                string sql =
                    GetUpdateSessionSQL();

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
        private string GetUpdateSessionSQL()
        {
            return

            "UPDATE SessionMaster SET " +

            "SessionNo=@SessionNo," +
            "SessionName=@SessionName," +
            "SessionDate=@SessionDate," +
            "StartTime=@StartTime," +
            "EndTime=@EndTime," +
            "TotalHours=@TotalHours," +
            "Remarks=@Remarks," +
            "TopicID=@TopicID," +
            "TrainerID=@TrainerID," +
            "DisplayOrder=@DisplayOrder," +
            "ModifiedOn=GETDATE()," +
            "ModifiedBy=@ModifiedBy " +

            "WHERE SessionID=@SessionID";
        }
        private SqlParameter[] CreateUpdateParameters(
    SessionModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                "@SessionID",
                model.SessionID));

            param.Add(
                new SqlParameter(
                "@SessionNo",
                model.SessionNo));

            param.Add(
                new SqlParameter(
                "@SessionName",
                model.SessionName));

            param.Add(
                new SqlParameter(
                "@SessionDate",
                model.SessionDate.Value));

            param.Add(
                new SqlParameter(
                "@StartTime",
                model.StartTime.Value));

            param.Add(
                new SqlParameter(
                "@EndTime",
                model.EndTime.Value));

            param.Add(
                new SqlParameter(
                "@TotalHours",
                model.TotalHours));

            param.Add(
                new SqlParameter(
                "@Remarks",
                model.Remarks));

            param.Add(
                new SqlParameter(
                "@TopicID",
                model.TopicID));

            param.Add(
                new SqlParameter(
                "@TrainerID",
                model.TrainerID));

            param.Add(
                new SqlParameter(
                "@DisplayOrder",
                model.DisplayOrder));

            param.Add(
                new SqlParameter(
                "@ModifiedBy",
                model.UpdatedBy));

            return
                param.ToArray();
        }

        #region Complete Session

        public bool CompleteSession(
            string sessionID,
            string completedBy,
            out string message)
        {
            message = "";

            try
            {
                if (!SessionExists(sessionID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =
                    "UPDATE SessionMaster " +
                    "SET " +
                    "SessionStatus=@SessionStatus," +
                    "SessionCompletedOn=GETDATE()," +
                    "CompletedBy=@CompletedBy," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@ModifiedBy " +
                    "WHERE SessionID=@SessionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@SessionStatus",
                Constants.SessionStatus.Completed),

            new SqlParameter(
                "@CompletedBy",
                completedBy),

            new SqlParameter(
                "@ModifiedBy",
                completedBy),

            new SqlParameter(
                "@SessionID",
                sessionID)
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
        #region Lock Session

        public bool LockSession(
            string sessionID,
            string lockedBy,
            out string message)
        {
            message = "";

            try
            {
                string sql =
                    "UPDATE SessionMaster " +
                    "SET " +
                    "SessionLocked=1," +
                    "LockedBy=@LockedBy," +
                    "LockedOn=GETDATE()," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@ModifiedBy " +
                    "WHERE SessionID=@SessionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@LockedBy",
                lockedBy),

            new SqlParameter(
                "@ModifiedBy",
                lockedBy),

            new SqlParameter(
                "@SessionID",
                sessionID)
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
        #region Cancel Session

        public bool CancelSession(
            string sessionID,
            string reason,
            string cancelledBy,
            out string message)
        {
            message = "";

            try
            {
                string sql =
                    "UPDATE SessionMaster " +
                    "SET " +
                    "SessionCancelled=1," +
                    "CancelledBy=@CancelledBy," +
                    "CancelledOn=GETDATE()," +
                    "CancellationReason=@CancellationReason," +
                    "SessionStatus=@SessionStatus," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@ModifiedBy " +
                    "WHERE SessionID=@SessionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@CancelledBy",
                cancelledBy),

            new SqlParameter(
                "@CancellationReason",
                reason),

            new SqlParameter(
                "@SessionStatus",
                Constants.SessionStatus.Cancelled),

            new SqlParameter(
                "@ModifiedBy",
                cancelledBy),

            new SqlParameter(
                "@SessionID",
                sessionID)
        };

                int i =
                    objDB.ExecuteSql(
                    sql,
                    param);

                if (i > 0)
                {
                    message =
                        Messages.CancelSuccess;

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
        #region Change Trainer

        public bool ChangeTrainer(
            string sessionID,
            string trainerID,
            string updatedBy,
            string remarks,
            out string message)
        {
            message = "";

            try
            {
                if (!SessionExists(sessionID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =
                    "UPDATE SessionMaster " +
                    "SET " +
                    "TrainerID=@TrainerID," +
                    "Remarks=@Remarks," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@ModifiedBy " +
                    "WHERE SessionID=@SessionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TrainerID",
                trainerID),

            new SqlParameter(
                "@Remarks",
                remarks),

            new SqlParameter(
                "@ModifiedBy",
                updatedBy),

            new SqlParameter(
                "@SessionID",
                sessionID)
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
        #region Reschedule Session

        public bool RescheduleSession(
            string sessionID,
            DateTime sessionDate,
            TimeSpan startTime,
            TimeSpan endTime,
            string updatedBy,
            out string message)
        {
            message = "";
            if (endTime <= startTime)
            {
                message =
                    "Invalid session timing.";

                return false;
            }
            try
            {
                decimal totalHours =
                    Convert.ToDecimal(
                    (
                    endTime -
                    startTime
                    ).TotalHours);

                string sql =
                    "UPDATE SessionMaster " +
                    "SET " +
                    "SessionDate=@SessionDate," +
                    "StartTime=@StartTime," +
                    "EndTime=@EndTime," +
                    "TotalHours=@TotalHours," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@ModifiedBy " +
                    "WHERE SessionID=@SessionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@SessionDate",
                sessionDate),

            new SqlParameter(
                "@StartTime",
                startTime),

            new SqlParameter(
                "@EndTime",
                endTime),

            new SqlParameter(
                "@TotalHours",
                totalHours),

            new SqlParameter(
                "@ModifiedBy",
                updatedBy),

            new SqlParameter(
                "@SessionID",
                sessionID)
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
        #region Delete Session

        public bool DeleteSession(
      string sessionID,
      out string message)
        {
            message = "";

            try
            {
                if (!CanDeleteSession(sessionID))
                {
                    message =
                        "Session cannot be deleted.";

                    return false;
                }

                SqlParameter[] param =
                {
            new SqlParameter(
                "@SessionID",
                sessionID)
        };

                //---------------------------------------
                // Attendance Check
                //---------------------------------------

                string sqlAttendance =
                    "SELECT COUNT(*) " +
                    "FROM SessionAttendance " +
                    "WHERE SessionID=@SessionID";

                if
                (
                    objCommon.GetCount(
                    sqlAttendance,
                    param) > 0
                )
                {
                    message =
                        "Attendance already marked. Session cannot be deleted.";

                    return false;
                }

                //---------------------------------------
                // Delete Session
                //---------------------------------------

                string sql =
                    "DELETE FROM SessionMaster " +
                    "WHERE SessionID=@SessionID";

                int i =
                    objDB.ExecuteSql(
                    sql,
                    param);

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
        #region Can Edit Session

        public bool CanEditSession(
            string sessionID)
        {
            DataTable dt =
                GetSessionByID(
                sessionID);

            if (dt.Rows.Count == 0)
                return false;

            string status =
                dt.Rows[0]["SessionStatus"]
                .ToString();

            return
                status != Constants.SessionStatus.Completed
                &&
                status != Constants.SessionStatus.Cancelled;
        }

        #endregion
        #region Can Delete Session

        public bool CanDeleteSession(
            string sessionID)
        {
            DataTable dt =
                GetSessionByID(
                sessionID);

            if (dt.Rows.Count == 0)
                return false;

            string status =
                dt.Rows[0]["SessionStatus"]
                .ToString();

            if (status == Constants.SessionStatus.Completed)
                return false;

            if (status == Constants.SessionStatus.Running)
                return false;

            return true;
        }

        #endregion
    }
}