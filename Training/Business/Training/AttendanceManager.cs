using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using Training.Common;
using Training.Helper;
using Training.Models;

namespace Training.Business.Training
{

    public class AttendanceManager
    {
        private clsDataAccess objDB;

        private CommonFunctions objCommon;

        private IDGenerator objID;

        public AttendanceManager()
        {
            objDB =
                new clsDataAccess();

            objCommon =
                new CommonFunctions();

            objID =
                new IDGenerator();
        }

        #region Get Methods

        public string GenerateAttendanceID()
        {
            return
                objID.GenerateAttendanceID();
        }

        public DataTable GetAttendanceBySession(
            string sessionID)
        {
            string sql =
                "SELECT * " +
                "FROM SessionAttendance " +
                "WHERE SessionID=@SessionID " +
                "ORDER BY EmpID";

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

        public DataTable GetAttendanceByTraining(
            string trainingID)
        {
            string sql =
                "SELECT * " +
                "FROM SessionAttendance " +
                "WHERE TrainingID=@TrainingID";

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

        public bool AttendanceExists(
            string sessionID,
            string empID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM SessionAttendance " +
                "WHERE SessionID=@SessionID " +
                "AND EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@SessionID",
                    sessionID),

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
        #region Validation

        public string ValidateAttendance(
            AttendanceModel model)
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
                model.SessionID)
            )
            {
                return
                    "Session is required.";
            }

            if
            (
                !Validator.Required(
                model.EmpID)
            )
            {
                return
                    "Employee is required.";
            }

            if
            (
                !Validator.Required(
                model.AttendanceStatus)
            )
            {
                return
                    "Attendance Status is required.";
            }

            return "";
        }

        #endregion

        #region Mark Attendance

        public bool MarkAttendance(
            AttendanceModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateAttendance(
                    model);

                if (message != "")
                {
                    return false;
                }

                //---------------------------------------
                // Duplicate
                //---------------------------------------

                if
                (
                    AttendanceExists(
                    model.SessionID,
                    model.EmpID)
                )
                {
                    message =
                        "Attendance already marked.";

                    return false;
                }

                //---------------------------------------
                // Generate ID
                //---------------------------------------

                if
                (
                    string.IsNullOrWhiteSpace(
                    model.AttendanceID)
                )
                {
                    model.AttendanceID =
                        GenerateAttendanceID();
                }

                //---------------------------------------
                // Insert
                //---------------------------------------

                string sql =
                    GetInsertAttendanceSQL();

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

        private string GetInsertAttendanceSQL()
        {
            return

            "INSERT INTO SessionAttendance " +

            "(" +

            "AttendanceID," +
            "SessionID," +
            "TrainingID," +
            "EmpID," +
            "AttendanceStatus," +
            "Remarks," +
            "CreatedOn," +
            "CreatedBy" +

            ")" +

            " VALUES " +

            "(" +

            "@AttendanceID," +
            "@SessionID," +
            "@TrainingID," +
            "@EmpID," +
            "@AttendanceStatus," +
            "@Remarks," +
            "GETDATE()," +
            "@CreatedBy" +

            ")";
        }
        private SqlParameter[] CreateInsertParameters(
AttendanceModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                "@AttendanceID",
                model.AttendanceID));

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
                "@EmpID",
                model.EmpID));

            param.Add(
                new SqlParameter(
                "@AttendanceStatus",
                model.AttendanceStatus));

            param.Add(
                new SqlParameter(
                "@Remarks",
                model.Remarks));

            param.Add(
                new SqlParameter(
                "@CreatedBy",
                model.CreatedBy));

            return
                param.ToArray();
        }
        #region Update Attendance

        public bool UpdateAttendance(
            AttendanceModel model,
            out string message)
        {
            message = "";

            try
            {
                message =
                    ValidateAttendance(
                    model);

                if (message != "")
                {
                    return false;
                }

                string sql =

                    "UPDATE SessionAttendance " +

                    "SET " +

                    "AttendanceStatus=@AttendanceStatus," +
                    "Remarks=@Remarks," +
                    "ModifiedOn=GETDATE()," +
                    "ModifiedBy=@ModifiedBy " +

                    "WHERE " +

                    "SessionID=@SessionID " +
                    "AND EmpID=@EmpID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@AttendanceStatus",
                model.AttendanceStatus),

            new SqlParameter(
                "@Remarks",
                model.Remarks),

            new SqlParameter(
                "@ModifiedBy",
                model.UpdatedBy),

            new SqlParameter(
                "@SessionID",
                model.SessionID),

            new SqlParameter(
                "@EmpID",
                model.EmpID)
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
        #region Bulk Attendance

        public bool BulkAttendance(
            List<AttendanceModel> list,
            string updatedBy,
            out string message)
        {
            message = "";

            try
            {
                foreach
                (
                    AttendanceModel model
                    in list
                )
                {
                    model.UpdatedBy =
                        updatedBy;

                    if
                    (
                        AttendanceExists(
                        model.SessionID,
                        model.EmpID)
                    )
                    {
                        UpdateAttendance(
                            model,
                            out message);
                    }
                    else
                    {
                        MarkAttendance(
                            model,
                            out message);
                    }
                }

                message =
                    Messages.SaveSuccess;

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
        #region Finalize Attendance

        public bool FinalizeAttendance(
            string sessionID,
            string userID,
            out string message)
        {
            message = "";

            try
            {
                string sql =

                "UPDATE SessionMaster " +

                "SET " +

                "AttendanceStatus='Completed'," +
                "AttendanceCompletedOn=GETDATE()," +
                "AttendanceCompletedBy=@UserID," +
                "ModifiedOn=GETDATE()," +
                "ModifiedBy=@UserID " +

                "WHERE SessionID=@SessionID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@UserID",
                userID),

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
        #region Pending Attendance

        public DataTable GetPendingAttendance(
            string trainerID)
        {
            string sql =

            "SELECT * " +

            "FROM SessionMaster " +

            "WHERE TrainerID=@TrainerID " +

            "AND AttendanceStatus IS NULL " +

            "AND SessionCancelled=0 " +

            "ORDER BY SessionDate,StartTime";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainerID",
            trainerID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

    }
}