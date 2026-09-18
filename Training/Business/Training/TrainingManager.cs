using Microsoft.Ajax.Utilities;
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

    public class TrainingManager
    {
        private clsDataAccess objDB;

        private IDGenerator objID;

        private CommonFunctions objCommon;

        public TrainingManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }

        #region Get Methods

        public DataTable GetAllTraining()
        {
            string sql =
                "SELECT * " +
                "FROM TrainingDetails " +
                "ORDER BY DateFrom DESC";

            return
                objDB.GetDataTable(
                sql);
        }

        public DataTable GetTrainingByID(
            string trainingID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainingDetails " +
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

        public bool TrainingExists(
            string trainingID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingDetails " +
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

        public string GenerateTrainingID()
        {
            return
                objID.GenerateTrainingID();
        }

        #endregion
        #region Validation

        public string ValidateTraining(
            TrainingModel model)
        {
            if (!Validator.Required(
                model.TrainingType))
            {
                return
                    "Training Type is required.";
            }

            if (!Validator.Required(
                model.TrainingOrganizer))
            {
                return
                    "Training Organizer is required.";
            }

            if (!Validator.Required(
                model.TrainingLocation))
            {
                return
                    "Training Location is required.";
            }

            if (!Validator.Required(
                model.CourseID))
            {
                return
                    "Course is required.";
            }

            if (!model.DateFrom.HasValue)
            {
                return "Date From is required.";
            }

            if (!model.DateTo.HasValue)
            {
                return "Date To is required.";
            }

            if (model.BatchStrength <= 0)
            {
                return
                    "Batch Strength should be greater than zero.";
            }

            return "";
        }

        #endregion
        #region Save Training

        public bool SaveTraining(
            TrainingModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateTraining(
                    model);

                if (message != "")
                {
                    return false;
                }

                //---------------------------------------
                // Generate Training ID
                //---------------------------------------

                if (string.IsNullOrWhiteSpace(
                    model.TrainingID))
                {
                    model.TrainingID =
                        GenerateTrainingID();
                }

                //---------------------------------------
                // Duplicate Check
                //---------------------------------------

                string sqlCheck =
                    "SELECT COUNT(*) " +
                    "FROM TrainingDetails " +
                    "WHERE " +
                    "TrainingType=@TrainingType " +
                    "AND CourseID=@CourseID " +
                    "AND DateFrom=@DateFrom " +
                    "AND DateTo=@DateTo " +
                    "AND TrainingLocation=@TrainingLocation";

                SqlParameter[] checkParam =
                {
                new SqlParameter("@TrainingType",model.TrainingType),
                new SqlParameter("@CourseID",model.CourseID),
                new SqlParameter("@DateFrom",model.DateFrom),
                new SqlParameter("@DateTo",model.DateTo),
                new SqlParameter("@TrainingLocation",model.TrainingLocation)
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
                // Date Validation
                //---------------------------------------

                DateTime fromDate =
    model.DateFrom.Value;

                DateTime toDate =
                    model.DateTo.Value;

                if (fromDate > toDate)
                {
                    message =
                        "Date From cannot be greater than Date To.";

                    return false;
                }

                //---------------------------------------
                // Number Of Days
                //---------------------------------------

                model.NoOfDays =
                    Convert.ToInt32(
                    (
                    toDate -
                    fromDate
                    ).TotalDays
                    ) + 1;

                //---------------------------------------
                // Default Status
                //---------------------------------------

                string trainingStatus =
                    Constants.TrainingStatus.Draft;

                string workflowStage =
                    Constants.WorkflowStage.BatchCreated;
                //---------------------------------------
                // Save Record
                //---------------------------------------

                string sql =
                    GetInsertTrainingSQL();

                SqlParameter[] param =
                    CreateTrainingParameters(
                    model,
                    trainingStatus,
                    workflowStage);

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

        #region Update Training

        public bool UpdateTraining(
            TrainingModel model,
            out string message)
        {
            message = "";

            try
            {
                //---------------------------------------
                // Validation
                //---------------------------------------

                message =
                    ValidateTraining(
                    model);

                if (message != "")
                {
                    return false;
                }

                //---------------------------------------
                // Training Exists
                //---------------------------------------

                if (!TrainingExists(
                    model.TrainingID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                //---------------------------------------
                // Can Edit
                //---------------------------------------

                if (!CanEditTraining(
                    model.TrainingID))
                {
                    message =
                        "Training cannot be edited.";

                    return false;
                }


                //---------------------------------------
                // Number Of Days
                //---------------------------------------

                DateTime fromDate =
                    model.DateFrom.Value;

                DateTime toDate =
                    model.DateTo.Value;

                if (fromDate > toDate)
                {
                    message =
                        "Date From cannot be greater than Date To.";

                    return false;
                }

                model.NoOfDays =
                    Convert.ToInt32
                    (
                        (
                        toDate -
                        fromDate
                        ).TotalDays
                    ) + 1;
                //---------------------------------------
                // Update SQL
                //---------------------------------------

                string sql =
                    GetUpdateTrainingSQL();
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










        private string GetInsertTrainingSQL()
        {
            return

            "INSERT INTO TrainingDetails " +

            "(" +

            "TrainingID," +
            "TrainingType," +
            "TrainingOrganizer," +
            "TrainingLocation," +
            "CourseID," +
            "DateFrom," +
            "DateTo," +
            "TrainingCategory," +
            "NoOfDays," +
            "Hours," +
            "BatchStrength," +
            "Remarks," +

            "AttendanceRequired," +
            "AssessmentRequired," +
            "AssessmentMode," +
            "InitialAssessmentRequired," +
            "SessionAssessmentRequired," +
            "FinalAssessmentRequired," +
            "AssessmentConductedBy," +
            "FeedbackRequired," +
            "CertificateRequired," +
            "TrainerHostelRequired," +
            "TraineeHostelRequired," +

            "OfficeOrderNo," +
            "OfficeOrderDate," +

            "TrainingStatus," +
            "CurrentWorkflowStage," +
            "WorkflowPercent," +

            "CreatedOn," +
            "CreatedBy," +

            "AttendanceMode," +
            "BatchAttendanceFrequency," +

            "BatchAttendanceRequired," +
            "SessionAttendanceRequired" +

            ")" +

            " VALUES " +

            "(" +

            "@TrainingID," +
            "@TrainingType," +
            "@TrainingOrganizer," +
            "@TrainingLocation," +
            "@CourseID," +
            "@DateFrom," +
            "@DateTo," +
            "@TrainingCategory," +
            "@NoOfDays," +
            "@Hours," +
            "@BatchStrength," +
            "@Remarks," +

            "@AttendanceRequired," +
            "@AssessmentRequired," +
            "@AssessmentMode," +
            "@InitialAssessmentRequired," +
            "@SessionAssessmentRequired," +
            "@FinalAssessmentRequired," +
            "@AssessmentConductedBy," +
            "@FeedbackRequired," +
            "@CertificateRequired," +
            "@TrainerHostelRequired," +
            "@TraineeHostelRequired," +

            "@OfficeOrderNo," +
            "@OfficeOrderDate," +

            "@TrainingStatus," +
            "@CurrentWorkflowStage," +
            "@WorkflowPercent," +

            "GETDATE()," +
            "@CreatedBy," +
            "@AttendanceMode," +
            "@BatchAttendanceFrequency," +
            "@BatchAttendanceRequired," +
            "@SessionAttendanceRequired" +

            ")";
        }
        private SqlParameter[] CreateTrainingParameters(
        TrainingModel model,
        string trainingStatus,
        string workflowStage)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                "@TrainingID",
                model.TrainingID));

            param.Add(
                new SqlParameter(
                "@TrainingType",
                model.TrainingType));

            param.Add(
                new SqlParameter(
                "@TrainingOrganizer",
                model.TrainingOrganizer));

            param.Add(
                new SqlParameter(
                "@TrainingLocation",
                model.TrainingLocation));

            param.Add(
                new SqlParameter(
                "@CourseID",
                model.CourseID));

            param.Add(
                new SqlParameter(
                "@DateFrom",
                model.DateFrom.Value));

            param.Add(
                new SqlParameter(
                "@DateTo",
                model.DateTo.Value));

            param.Add(
                new SqlParameter(
                "@TrainingCategory",
                model.TrainingCategory));

            param.Add(
                new SqlParameter(
                "@NoOfDays",
                model.NoOfDays));

            param.Add(
                new SqlParameter(
                "@Hours",
                model.Hours));

            param.Add(
                new SqlParameter(
                "@BatchStrength",
                model.BatchStrength));

            param.Add(
                new SqlParameter(
                "@Remarks",
                model.Remarks));

            param.Add(
                new SqlParameter(
                "@AttendanceRequired",
                model.AttendanceRequired));

            param.Add(
                new SqlParameter(
                "@AssessmentRequired",
                model.AssessmentRequired));

            param.Add(
                new SqlParameter(
                "@AssessmentMode",
                model.AssessmentMode));

            param.Add(
                new SqlParameter(
                "@InitialAssessmentRequired",
                model.InitialAssessmentRequired));

            param.Add(
                new SqlParameter(
                "@SessionAssessmentRequired",
                model.SessionAssessmentRequired));

            param.Add(
                new SqlParameter(
                "@FinalAssessmentRequired",
                model.FinalAssessmentRequired));

            param.Add(
                new SqlParameter(
                "@AssessmentConductedBy",
                model.AssessmentConductedBy));

            param.Add(
                new SqlParameter(
                "@FeedbackRequired",
                model.FeedbackRequired));

            param.Add(
                new SqlParameter(
                "@CertificateRequired",
                model.CertificateRequired));

            param.Add(
                new SqlParameter(
                "@TrainerHostelRequired",
                model.TrainerHostelRequired));

            param.Add(
                new SqlParameter(
                "@TraineeHostelRequired",
                model.TraineeHostelRequired));

            param.Add(
                new SqlParameter(
                "@OfficeOrderNo",
                string.IsNullOrWhiteSpace(model.OfficeOrderNo)
                ?
                (object)DBNull.Value
                :
                model.OfficeOrderNo));

            param.Add(
    new SqlParameter(
    "@OfficeOrderDate",
    model.OfficeOrderDate.HasValue
    ?
    (object)model.OfficeOrderDate.Value
    :
    DBNull.Value));

            param.Add(
            new SqlParameter(
            "@TrainingStatus",
            trainingStatus));

            param.Add(
                new SqlParameter(
                "@CurrentWorkflowStage",
                workflowStage));

            param.Add(
                new SqlParameter(
                "@WorkflowPercent",
                5));

            param.Add(
                new SqlParameter(
                "@CreatedBy",
                model.CreatedBy));

            param.Add(
new SqlParameter(
"@AttendanceMode",
model.AttendanceMode));

            param.Add(
            new SqlParameter(
            "@BatchAttendanceFrequency",
            model.BatchAttendanceFrequency));

            param.Add(
            new SqlParameter(
            "@BatchAttendanceRequired",
            model.BatchAttendanceRequired));

            param.Add(
            new SqlParameter(
            "@SessionAttendanceRequired",
            model.SessionAttendanceRequired));

            return
                param.ToArray();
        }

        private string GetUpdateTrainingSQL()
        {
            return

            "UPDATE TrainingDetails SET " +

            "TrainingType=@TrainingType," +
            "TrainingOrganizer=@TrainingOrganizer," +
            "TrainingLocation=@TrainingLocation," +
            "CourseID=@CourseID," +
            "DateFrom=@DateFrom," +
            "DateTo=@DateTo," +
            "TrainingCategory=@TrainingCategory," +
            "NoOfDays=@NoOfDays," +
            "Hours=@Hours," +
            "BatchStrength=@BatchStrength," +
            "Remarks=@Remarks," +

            "AttendanceRequired=@AttendanceRequired," +
            "AssessmentRequired=@AssessmentRequired," +
            "AssessmentMode=@AssessmentMode," +
            "InitialAssessmentRequired=@InitialAssessmentRequired," +
            "SessionAssessmentRequired=@SessionAssessmentRequired," +
            "FinalAssessmentRequired=@FinalAssessmentRequired," +
            "AssessmentConductedBy=@AssessmentConductedBy," +
            "FeedbackRequired=@FeedbackRequired," +
            "CertificateRequired=@CertificateRequired," +
            "TrainerHostelRequired=@TrainerHostelRequired," +
            "TraineeHostelRequired=@TraineeHostelRequired," +

            "OfficeOrderNo=@OfficeOrderNo," +
            "OfficeOrderDate=@OfficeOrderDate," +

            "UpdatedOn=GETDATE()," +
            "UpdatedBy=@UpdatedBy," +

            "AttendanceMode=@AttendanceMode," +
            "BatchAttendanceFrequency=@BatchAttendanceFrequency," +

            "BatchAttendanceRequired=@BatchAttendanceRequired," +
            "SessionAttendanceRequired=@SessionAttendanceRequired " +

            "WHERE TrainingID=@TrainingID";
        }
        private SqlParameter[] CreateUpdateParameters(
    TrainingModel model)
        {
            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                "@TrainingID",
                model.TrainingID));

            param.Add(
                new SqlParameter(
                "@TrainingType",
                model.TrainingType));

            param.Add(
                new SqlParameter(
                "@TrainingOrganizer",
                model.TrainingOrganizer));

            param.Add(
                new SqlParameter(
                "@TrainingLocation",
                model.TrainingLocation));

            param.Add(
                new SqlParameter(
                "@CourseID",
                model.CourseID));

            param.Add(
                new SqlParameter(
                "@DateFrom",
                model.DateFrom.Value));

            param.Add(
                new SqlParameter(
                "@DateTo",
                model.DateTo.Value));

            param.Add(
                new SqlParameter(
                "@TrainingCategory",
                model.TrainingCategory));

            param.Add(
                new SqlParameter(
                "@NoOfDays",
                model.NoOfDays));

            param.Add(
                new SqlParameter(
                "@Hours",
                model.Hours));

            param.Add(
                new SqlParameter(
                "@BatchStrength",
                model.BatchStrength));

            param.Add(
                new SqlParameter(
                "@Remarks",
                model.Remarks));

            param.Add(
                new SqlParameter(
                "@AttendanceRequired",
                model.AttendanceRequired));

            param.Add(
                new SqlParameter(
                "@AssessmentRequired",
                model.AssessmentRequired));

            param.Add(
                new SqlParameter(
                "@AssessmentMode",
                model.AssessmentMode));

            param.Add(
                new SqlParameter(
                "@InitialAssessmentRequired",
                model.InitialAssessmentRequired));

            param.Add(
                new SqlParameter(
                "@SessionAssessmentRequired",
                model.SessionAssessmentRequired));

            param.Add(
                new SqlParameter(
                "@FinalAssessmentRequired",
                model.FinalAssessmentRequired));

            param.Add(
                new SqlParameter(
                "@AssessmentConductedBy",
                model.AssessmentConductedBy));

            param.Add(
                new SqlParameter(
                "@FeedbackRequired",
                model.FeedbackRequired));

            param.Add(
                new SqlParameter(
                "@CertificateRequired",
                model.CertificateRequired));

            param.Add(
                new SqlParameter(
                "@TrainerHostelRequired",
                model.TrainerHostelRequired));

            param.Add(
                new SqlParameter(
                "@TraineeHostelRequired",
                model.TraineeHostelRequired));

            param.Add(
                new SqlParameter(
                "@OfficeOrderNo",
                string.IsNullOrWhiteSpace(model.OfficeOrderNo)
                ?
                (object)DBNull.Value
                :
                model.OfficeOrderNo));

            param.Add(
                new SqlParameter(
                "@OfficeOrderDate",
                model.OfficeOrderDate.HasValue
                ?
                (object)model.OfficeOrderDate.Value
                :
                DBNull.Value));


            param.Add(
     new SqlParameter(
     "@UpdatedBy",
     model.UpdatedBy));

            param.Add(
    new SqlParameter(
    "@AttendanceMode",
    model.AttendanceMode));

            param.Add(
                new SqlParameter(
                "@BatchAttendanceFrequency",
                model.BatchAttendanceFrequency));

            param.Add(
                new SqlParameter(
                "@BatchAttendanceRequired",
                model.BatchAttendanceRequired));

            param.Add(
                new SqlParameter(
                "@SessionAttendanceRequired",
                model.SessionAttendanceRequired));

            return
                param.ToArray();
        }
        #region Publish Training

        public bool PublishTraining(
            string trainingID,
            string publishedBy,
            out string message)
        {
            message = "";

            try
            {
                if (!TrainingExists(trainingID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }
                if (!CanPublishTraining(trainingID))
                {
                    message =
                        "Training cannot be published.";

                    return false;
                }

                string sql =
                    "UPDATE TrainingDetails " +
                    "SET " +
                    "Published=1," +
                    "PublishedOn=GETDATE()," +
                    "PublishedBy=@PublishedBy," +
                    "TrainingStatus=@TrainingStatus," +
                    "CurrentWorkflowStage=@CurrentWorkflowStage," +
                    "WorkflowPercent=@WorkflowPercent " +
                    "WHERE TrainingID=@TrainingID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@PublishedBy",
                publishedBy),

            new SqlParameter(
                "@TrainingStatus",
                Constants.TrainingStatus.Published),

            new SqlParameter(
                "@CurrentWorkflowStage",
                Constants.WorkflowStage.Ready),

            new SqlParameter(
                "@WorkflowPercent",
                25),

            new SqlParameter(
                "@TrainingID",
                trainingID)
        };

                if (objDB.ExecuteSql(sql, param) > 0)
                {
                    message =
                        Messages.PublishSuccess;

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
        #region Cancel Training

        public bool CancelTraining(
            string trainingID,
            string remarks,
            string updatedBy,
            out string message)
        {
            message = "";

            try
            {
                if (!TrainingExists(trainingID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =
                    "UPDATE TrainingDetails " +
                    "SET " +
                    "TrainingStatus=@TrainingStatus," +
                    "Remarks=@Remarks," +
                    "UpdatedOn=GETDATE()," +
                    "UpdatedBy=@UpdatedBy " +
                    "WHERE TrainingID=@TrainingID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TrainingStatus",
                Constants.TrainingStatus.Cancelled),

            new SqlParameter(
                "@Remarks",
                remarks),

            new SqlParameter(
                "@UpdatedBy",
                updatedBy),

            new SqlParameter(
                "@TrainingID",
                trainingID)
        };

                if (objDB.ExecuteSql(sql, param) > 0)
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
        #region Close Training

        public bool CloseTraining(
            string trainingID,
            string updatedBy,
            out string message)
        {
            message = "";

            try
            {
                if (!TrainingExists(trainingID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =
                    "UPDATE TrainingDetails " +
                    "SET " +
                    "TrainingStatus=@TrainingStatus," +
                    "TrainingCompletedOn=GETDATE()," +
                    "UpdatedOn=GETDATE()," +
                    "UpdatedBy=@UpdatedBy," +
                    "CurrentWorkflowStage=@CurrentWorkflowStage," +
                    "WorkflowPercent=@WorkflowPercent " +
                    "WHERE TrainingID=@TrainingID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TrainingStatus",
                Constants.TrainingStatus.Completed),

            new SqlParameter(
                "@UpdatedBy",
                updatedBy),

            new SqlParameter(
                "@CurrentWorkflowStage",
                Constants.WorkflowStage.Completed),

            new SqlParameter(
                "@WorkflowPercent",
                100),

            new SqlParameter(
                "@TrainingID",
                trainingID)
        };

                if (objDB.ExecuteSql(sql, param) > 0)
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
        #region Delete Training

        public bool DeleteTraining(
            string trainingID,
            out string message)
        {
            message = "";
            if (!CanDeleteTraining(
    trainingID))
            {
                message =
                    "Published/Running/Completed training cannot be deleted.";

                return false;
            }
            try
            {
                if (!TrainingExists(
                    trainingID))
                {
                    message =
                        Messages.NoRecordFound;

                    return false;
                }

                string sql =
                    "DELETE FROM TrainingDetails " +
                    "WHERE TrainingID=@TrainingID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TrainingID",
                trainingID)
        };

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
        #region Can Publish Training

        public bool CanPublishTraining(
            string trainingID)
        {
            DataTable dt =
                GetTrainingByID(
                trainingID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            DataRow dr =
                dt.Rows[0];

            if (
                Convert.ToBoolean(
                dr["Published"]))
            {
                return false;
            }

            if
            (
                dr["TrainingStatus"]
                .ToString()
                ==
                Constants.TrainingStatus.Cancelled
            )
            {
                return false;
            }

            return true;
        }

        #endregion
        #region Can Close Training

        public bool CanCloseTraining(
            string trainingID)
        {
            DataTable dt =
                GetTrainingByID(
                trainingID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string status =
                dt.Rows[0]
                ["TrainingStatus"]
                .ToString();

            if
            (
                status ==
                Constants.TrainingStatus.Completed
            )
            {
                return false;
            }

            if
            (
                status ==
                Constants.TrainingStatus.Cancelled
            )
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Can Edit Training

        public bool CanEditTraining(
            string trainingID)
        {
            DataTable dt =
                GetTrainingByID(
                trainingID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string status =
                dt.Rows[0]["TrainingStatus"]
                .ToString();

            if
            (
                status ==
                Constants.TrainingStatus.Completed
            )
            {
                return false;
            }

            if
            (
                status ==
                Constants.TrainingStatus.Cancelled
            )
            {
                return false;
            }

            return true;
        }

        #endregion

        #region Can Delete Training

        public bool CanDeleteTraining(
            string trainingID)
        {

            DataTable dt =
                GetTrainingByID(
                trainingID);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            string status =
                dt.Rows[0]
                ["TrainingStatus"]
                .ToString();

            if
            (
                status ==
                Constants.TrainingStatus.Published
            )
            {
                return false;
            }

            if
            (
                status ==
                Constants.TrainingStatus.Running
            )
            {
                return false;
            }

            if
            (
                status ==
                Constants.TrainingStatus.Completed
            )
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}