using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Training.Common;
using Training.Helper;
using Training.Models;

namespace Training.Business.Training
{
    public class TrainingProgressManager
    {
        private clsDataAccess objDB;

        private IDGenerator objID;

        private CommonFunctions objCommon;


        public TrainingProgressManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }


        #region Get Methods

        public DataTable GetProgress(
            string trainingID,
            string empID)
        {
            string sql =

                "SELECT * " +

                "FROM TrainingProgress " +

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


        public DataTable GetProgressByID(
            string progressID)
        {
            string sql =

                "SELECT * " +

                "FROM TrainingProgress " +

                "WHERE ProgressID=@ProgressID";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@ProgressID",
                    progressID)
            };


            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        public bool ProgressExists(
            string trainingID,
            string empID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TrainingProgress " +

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


        #endregion


        #region Create Progress

        public bool CreateProgress(
            string trainingID,
            string empID,
            string createdBy,
            out string progressID,
            out string message)
        {
            progressID = "";
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
                    ProgressExists(
                        trainingID,
                        empID)
                )
                {
                    DataTable dt =
                        GetProgress(
                            trainingID,
                            empID);


                    if
                    (
                        dt.Rows.Count > 0
                    )
                    {
                        progressID =
                            dt.Rows[0][
                                "ProgressID"]
                            .ToString();
                    }


                    message =
                        "Training progress already exists.";

                    return false;
                }


                progressID =
                    objID.GenerateProgressID();


                string sql =

                    "INSERT INTO TrainingProgress " +

                    "(" +

                    "ProgressID," +
                    "TrainingID," +
                    "EmpID," +
                    "AttendanceCompleted," +
                    "PreExamCompleted," +
                    "PostExamCompleted," +
                    "SessionFeedbackCompleted," +
                    "BatchFeedbackCompleted," +
                    "CertificateGenerated," +
                    "WorkflowStatus," +
                    "CreatedOn," +
                    "CreatedBy" +

                    ")" +

                    " VALUES " +

                    "(" +

                    "@ProgressID," +
                    "@TrainingID," +
                    "@EmpID," +
                    "0," +
                    "0," +
                    "0," +
                    "0," +
                    "0," +
                    "0," +
                    "@WorkflowStatus," +
                    "GETDATE()," +
                    "@CreatedBy" +

                    ")";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@ProgressID",
                        progressID),

                    new SqlParameter(
                        "@TrainingID",
                        trainingID),

                    new SqlParameter(
                        "@EmpID",
                        empID),

                    new SqlParameter(
                        "@WorkflowStatus",
                        Constants.WorkflowStage.BatchCreated),

                    new SqlParameter(
                        "@CreatedBy",
                        createdBy)
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
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Update Progress

        public bool UpdateProgress(
            TrainingProgressModel model,
            out string message)
        {
            message = "";


            try
            {
                if
                (
                    model == null
                )
                {
                    message =
                        "Progress information is required.";

                    return false;
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        model.TrainingID)
                )
                {
                    message =
                        "Training ID is required.";

                    return false;
                }


                if
                (
                    String.IsNullOrWhiteSpace(
                        model.EmpID)
                )
                {
                    message =
                        "Employee ID is required.";

                    return false;
                }


                string sql =

                    "UPDATE TrainingProgress SET " +

                    "AttendanceCompleted=@AttendanceCompleted," +

                    "PreExamCompleted=@PreExamCompleted," +

                    "PostExamCompleted=@PostExamCompleted," +

                    "SessionFeedbackCompleted=@SessionFeedbackCompleted," +

                    "BatchFeedbackCompleted=@BatchFeedbackCompleted," +

                    "CertificateGenerated=@CertificateGenerated," +

                    "WorkflowStatus=@WorkflowStatus," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE TrainingID=@TrainingID " +

                    "AND EmpID=@EmpID";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@AttendanceCompleted",
                        model.AttendanceCompleted),

                    new SqlParameter(
                        "@PreExamCompleted",
                        model.PreExamCompleted),

                    new SqlParameter(
                        "@PostExamCompleted",
                        model.PostExamCompleted),

                    new SqlParameter(
                        "@SessionFeedbackCompleted",
                        model.SessionFeedbackCompleted),

                    new SqlParameter(
                        "@BatchFeedbackCompleted",
                        model.BatchFeedbackCompleted),

                    new SqlParameter(
                        "@CertificateGenerated",
                        model.CertificateGenerated),

                    new SqlParameter(
                        "@WorkflowStatus",
                        model.WorkflowStatus),

                    new SqlParameter(
                        "@UpdatedBy",
                        model.UpdatedBy),

                    new SqlParameter(
                        "@TrainingID",
                        model.TrainingID),

                    new SqlParameter(
                        "@EmpID",
                        model.EmpID)
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
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Mark Attendance

        public bool MarkAttendanceCompleted(
            string trainingID,
            string empID,
            string updatedBy,
            out string message)
        {
            return
                UpdateSingleFlag(
                    trainingID,
                    empID,
                    "AttendanceCompleted",
                    updatedBy,
                    out message);
        }

        #endregion


        #region Mark Pre Exam

        public bool MarkPreExamCompleted(
            string trainingID,
            string empID,
            string updatedBy,
            out string message)
        {
            return
                UpdateSingleFlag(
                    trainingID,
                    empID,
                    "PreExamCompleted",
                    updatedBy,
                    out message);
        }

        #endregion


        #region Mark Post Exam

        public bool MarkPostExamCompleted(
            string trainingID,
            string empID,
            string updatedBy,
            out string message)
        {
            return
                UpdateSingleFlag(
                    trainingID,
                    empID,
                    "PostExamCompleted",
                    updatedBy,
                    out message);
        }

        #endregion


        #region Mark Session Feedback

        public bool MarkSessionFeedbackCompleted(
            string trainingID,
            string empID,
            string updatedBy,
            out string message)
        {
            return
                UpdateSingleFlag(
                    trainingID,
                    empID,
                    "SessionFeedbackCompleted",
                    updatedBy,
                    out message);
        }

        #endregion


        #region Mark Batch Feedback

        public bool MarkBatchFeedbackCompleted(
            string trainingID,
            string empID,
            string updatedBy,
            out string message)
        {
            return
                UpdateSingleFlag(
                    trainingID,
                    empID,
                    "BatchFeedbackCompleted",
                    updatedBy,
                    out message);
        }

        #endregion


        #region Mark Certificate

        public bool MarkCertificateGenerated(
            string trainingID,
            string empID,
            string updatedBy,
            out string message)
        {
            message = "";


            try
            {
                string sql =

                    "UPDATE TrainingProgress SET " +

                    "CertificateGenerated=1," +

                    "CertificateGeneratedOn=GETDATE()," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE TrainingID=@TrainingID " +

                    "AND EmpID=@EmpID";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@UpdatedBy",
                        updatedBy),

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
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Workflow

        public bool UpdateWorkflowStatus(
            string trainingID,
            string empID,
            string workflowStatus,
            string updatedBy,
            out string message)
        {
            message = "";


            try
            {
                if
                (
                    String.IsNullOrWhiteSpace(
                        workflowStatus)
                )
                {
                    message =
                        "Workflow status is required.";

                    return false;
                }


                string sql =

                    "UPDATE TrainingProgress SET " +

                    "WorkflowStatus=@WorkflowStatus," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE TrainingID=@TrainingID " +

                    "AND EmpID=@EmpID";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@WorkflowStatus",
                        workflowStatus),

                    new SqlParameter(
                        "@UpdatedBy",
                        updatedBy),

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
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Completion Check

        public bool IsProgressCompleted(
            string trainingID,
            string empID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TrainingProgress " +

                "WHERE TrainingID=@TrainingID " +

                "AND EmpID=@EmpID " +

                "AND AttendanceCompleted=1 " +

                "AND PreExamCompleted=1 " +

                "AND PostExamCompleted=1 " +

                "AND SessionFeedbackCompleted=1 " +

                "AND BatchFeedbackCompleted=1";


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


        #region Private Methods

        private bool UpdateSingleFlag(
            string trainingID,
            string empID,
            string columnName,
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


                string sql =

                    "UPDATE TrainingProgress SET " +

                    columnName +
                    "=1," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE TrainingID=@TrainingID " +

                    "AND EmpID=@EmpID";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@UpdatedBy",
                        updatedBy),

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