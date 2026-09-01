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
    public class TrainingAssignmentManager
    {
        private clsDataAccess objDB;

        private IDGenerator objID;

        private CommonFunctions objCommon;


        public TrainingAssignmentManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }


        #region Get Methods

        public DataTable GetAssignmentByID(
            string assignmentID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainingAssignment " +
                "WHERE AssignmentID=@AssignmentID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@AssignmentID",
                    assignmentID)
            };

            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        public DataTable GetAssignmentsByTraining(
            string trainingID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainingAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "ORDER BY ID DESC";

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


        public DataTable GetAssignmentsByEmployee(
            string empID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainingAssignment " +
                "WHERE EmpID=@EmpID " +
                "ORDER BY ID DESC";

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


        public DataTable GetEmployeeAssignments(
            string trainingID)
        {
            string sql =
                "SELECT " +
                "TA.AssignmentID," +
                "TA.TrainingID," +
                "TA.EmpID," +
                "TA.TrainingAttended," +
                "TA.AssignmentMode," +
                "TA.AssignmentStatus," +
                "TA.Remarks," +
                "TA.CreatedOn," +
                "TA.CreatedBy," +
                "E.EmpName," +
                "E.EmpDesignation," +
                "E.EmpCompany," +
                "E.EmpPostingPlace " +
                "FROM TrainingAssignment TA " +
                "INNER JOIN EmpBasicMaster E " +
                "ON TA.EmpID=E.EmpID " +
                "WHERE TA.TrainingID=@TrainingID " +
                "ORDER BY TA.ID DESC";

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


        #region Exists

        public bool AssignmentExists(
            string assignmentID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
                "WHERE AssignmentID=@AssignmentID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@AssignmentID",
                    assignmentID)
            };

            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }


        public bool AlreadyAssigned(
            string trainingID,
            string empID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
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
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }


        public bool EmployeeExists(
            string empID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM EmpBasicMaster " +
                "WHERE EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    empID)
            };

            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }


        public bool IsTrainingAssigned(
            string trainingID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
                "WHERE TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID)
            };

            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }


        public bool IsAttendanceMarked(
            string trainingID,
            string empID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID " +
                "AND TrainingAttended='Present'";

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


        #region Validation

        public string ValidateAssignment(
            TrainingAssignmentModel model)
        {
            if
            (
                model == null
            )
            {
                return
                    "Assignment data is required.";
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
                    model.EmpID)
            )
            {
                return
                    "Employee is required.";
            }


            if
            (
                !EmployeeExists(
                    model.EmpID)
            )
            {
                return
                    "Employee not found.";
            }


            if
            (
                AlreadyAssigned(
                    model.TrainingID,
                    model.EmpID)
            )
            {
                return
                    "Employee already assigned.";
            }


            return "";
        }

        #endregion


        #region Generate ID

        public string GenerateAssignmentID()
        {
            return
                objID.GenerateAssignmentID();
        }

        #endregion


        #region Save Assignment

        public bool SaveAssignment(
            TrainingAssignmentModel model,
            out string message)
        {
            message = "";


            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateAssignment(
                        model);


                if
                (
                    message != ""
                )
                {
                    return false;
                }


                //---------------------------------------
                // Generate ID
                //---------------------------------------

                if
                (
                    String.IsNullOrWhiteSpace(
                        model.AssignmentID)
                )
                {
                    model.AssignmentID =
                        GenerateAssignmentID();
                }


                //---------------------------------------
                // Default Values
                //---------------------------------------

                if
                (
                    String.IsNullOrWhiteSpace(
                        model.TrainingAttended)
                )
                {
                    model.TrainingAttended =
                        "Pending";
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        model.AssignmentStatus)
                )
                {
                    model.AssignmentStatus =
                        "Assigned";
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        model.AssignmentMode)
                )
                {
                    model.AssignmentMode =
                        "Manual";
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        model.CreatedBy)
                )
                {
                    model.CreatedBy =
                        "System";
                }


                //---------------------------------------
                // Insert
                //---------------------------------------

                string sql =
                    "INSERT INTO TrainingAssignment " +
                    "(" +
                    "AssignmentID," +
                    "TrainingID," +
                    "EmpID," +
                    "TrainingAttended," +
                    "CreatedOn," +
                    "CreatedBy," +
                    "AssignmentMode," +
                    "AssignmentStatus," +
                    "Remarks" +
                    ")" +
                    " VALUES " +
                    "(" +
                    "@AssignmentID," +
                    "@TrainingID," +
                    "@EmpID," +
                    "@TrainingAttended," +
                    "GETDATE()," +
                    "@CreatedBy," +
                    "@AssignmentMode," +
                    "@AssignmentStatus," +
                    "@Remarks" +
                    ")";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@AssignmentID",
                        model.AssignmentID),

                    new SqlParameter(
                        "@TrainingID",
                        model.TrainingID),

                    new SqlParameter(
                        "@EmpID",
                        model.EmpID),

                    new SqlParameter(
                        "@TrainingAttended",
                        model.TrainingAttended),

                    new SqlParameter(
                        "@CreatedBy",
                        model.CreatedBy),

                    new SqlParameter(
                        "@AssignmentMode",
                        model.AssignmentMode),

                    new SqlParameter(
                        "@AssignmentStatus",
                        model.AssignmentStatus),

                    new SqlParameter(
                        "@Remarks",
                        String.IsNullOrWhiteSpace(
                            model.Remarks)
                        ? (object)DBNull.Value
                        : model.Remarks)
                };


                if
                (
                    objDB.ExecuteSql(
                        sql,
                        param) <= 0
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }


                return true;
            }
            catch
            (
                Exception ex
            )
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Remove Assignment

        public bool RemoveAssignment(
            string assignmentID,
            out string message)
        {
            message = "";


            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                if
                (
                    String.IsNullOrWhiteSpace(
                        assignmentID)
                )
                {
                    message =
                        "Assignment ID is required.";

                    return false;
                }


                if
                (
                    !AssignmentExists(
                        assignmentID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }


                //---------------------------------------
                // Delete
                //---------------------------------------

                string sql =
                    "DELETE FROM TrainingAssignment " +
                    "WHERE AssignmentID=@AssignmentID";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@AssignmentID",
                        assignmentID)
                };


                if
                (
                    objDB.ExecuteSql(
                        sql,
                        param) <= 0
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }


                message =
                    Messages.DeleteSuccess;

                return true;
            }
            catch
            (
                Exception ex
            )
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Update Assignment Status

        public bool UpdateAssignmentStatus(
            string assignmentID,
            string status,
            string remarks,
            string updatedBy,
            out string message)
        {
            message = "";


            try
            {
                if
                (
                    String.IsNullOrWhiteSpace(
                        assignmentID)
                )
                {
                    message =
                        "Assignment ID is required.";

                    return false;
                }


                if
                (
                    !AssignmentExists(
                        assignmentID)
                )
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        status)
                )
                {
                    message =
                        "Assignment status is required.";

                    return false;
                }


                string sql =
                    "UPDATE TrainingAssignment SET " +
                    "AssignmentStatus=@AssignmentStatus," +
                    "Remarks=@Remarks " +
                    "WHERE AssignmentID=@AssignmentID";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@AssignmentStatus",
                        status),

                    new SqlParameter(
                        "@Remarks",
                        String.IsNullOrWhiteSpace(
                            remarks)
                        ? (object)DBNull.Value
                        : remarks),

                    new SqlParameter(
                        "@AssignmentID",
                        assignmentID)
                };


                if
                (
                    objDB.ExecuteSql(
                        sql,
                        param) <= 0
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }


                return true;
            }
            catch
            (
                Exception ex
            )
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Mark Attendance

        public bool MarkAttendance(
            string trainingID,
            string empID,
            string attendanceStatus,
            string updatedBy,
            out string message)
        {
            message = "";


            try
            {
                if
                (
                    String.IsNullOrWhiteSpace(
                        trainingID)
                )
                {
                    message =
                        "Training ID is required.";

                    return false;
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        empID)
                )
                {
                    message =
                        "Employee ID is required.";

                    return false;
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        attendanceStatus)
                )
                {
                    message =
                        "Attendance status is required.";

                    return false;
                }


                //---------------------------------------
                // Existing Assignment
                //---------------------------------------

                if
                (
                    AlreadyAssigned(
                        trainingID,
                        empID)
                )
                {
                    string update =
                        "UPDATE TrainingAssignment SET " +
                        "TrainingAttended=@TrainingAttended " +
                        "WHERE TrainingID=@TrainingID " +
                        "AND EmpID=@EmpID";


                    SqlParameter[] updateParam =
                    {
                        new SqlParameter(
                            "@TrainingAttended",
                            attendanceStatus),

                        new SqlParameter(
                            "@TrainingID",
                            trainingID),

                        new SqlParameter(
                            "@EmpID",
                            empID)
                    };


                    if
                    (
                        objDB.ExecuteSql(
                            update,
                            updateParam) <= 0
                    )
                    {
                        message =
                            Messages.DatabaseError;

                        return false;
                    }


                    return true;
                }


                //---------------------------------------
                // Assignment Not Found
                //---------------------------------------

                if
                (
                    !EmployeeExists(
                        empID)
                )
                {
                    message =
                        "Employee not found.";

                    return false;
                }


                //---------------------------------------
                // Create Assignment + Attendance
                //---------------------------------------

                TrainingAssignmentModel model =
                    new TrainingAssignmentModel();


                model.TrainingID =
                    trainingID;

                model.EmpID =
                    empID;

                model.TrainingAttended =
                    attendanceStatus;

                model.AssignmentMode =
                    "Attendance";

                model.AssignmentStatus =
                    "Assigned";

                model.CreatedBy =
                    String.IsNullOrWhiteSpace(
                        updatedBy)
                    ? "System"
                    : updatedBy;


                return
                    SaveAssignment(
                        model,
                        out message);
            }
            catch
            (
                Exception ex
            )
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Check Same Topic Completion

        public bool AlreadyCompletedSameTopic(
            string trainingID,
            string empID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment TA " +
                "INNER JOIN TrainingDetails TD " +
                "ON TA.TrainingID=TD.TrainingID " +
                "WHERE TA.EmpID=@EmpID " +
                "AND TD.TopicID=( " +
                "SELECT TopicID " +
                "FROM TrainingDetails " +
                "WHERE TrainingID=@TrainingID" +
                ") " +
                "AND ISNULL(TA.TrainingAttended,'')='Present'";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@EmpID",
                    empID),

                new SqlParameter(
                    "@TrainingID",
                    trainingID)
            };


            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }

        #endregion
    }
}