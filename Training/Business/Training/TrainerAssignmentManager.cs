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
    public class TrainerAssignmentManager
    {
        private clsDataAccess objDB;
        private IDGenerator objID;
        private CommonFunctions objCommon;

        public TrainerAssignmentManager()
        {
            objDB =
                new clsDataAccess();

            objID =
                new IDGenerator();

            objCommon =
                new CommonFunctions();
        }


        #region Get

        public DataTable GetByID(
            string trainerAssignmentID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainerAssignment " +
                "WHERE TrainerAssignmentID=@TrainerAssignmentID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainerAssignmentID",
                    trainerAssignmentID)
            };

            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        public DataTable GetByTraining(
            string trainingID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainerAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "AND Active=1 " +
                "ORDER BY ID";

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


        public DataTable GetByTrainer(
            string trainerID)
        {
            string sql =
                "SELECT * " +
                "FROM TrainerAssignment " +
                "WHERE TrainerID=@TrainerID " +
                "AND Active=1 " +
                "ORDER BY ID DESC";

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


        #region Exists

        public bool Exists(
            string trainingID,
            string trainerID)
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainerAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "AND TrainerID=@TrainerID " +
                "AND Active=1";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@TrainerID",
                    trainerID)
            };

            return
                objCommon.GetCount(
                    sql,
                    param) > 0;
        }

        #endregion


        #region Validation

        private string Validate(
            TrainerAssignmentModel model)
        {
            if (model == null)
            {
                return
                    "Trainer assignment is required.";
            }


            if (String.IsNullOrWhiteSpace(
                model.TrainingID))
            {
                return
                    "Training ID is required.";
            }


            if (String.IsNullOrWhiteSpace(
                model.TrainerID))
            {
                return
                    "Trainer ID is required.";
            }


            return "";
        }

        #endregion


        #region Assign

        public bool AssignTrainer(
            TrainerAssignmentModel model,
            out string message)
        {
            message = "";


            try
            {
                message =
                    Validate(model);


                if (message != "")
                {
                    return false;
                }


                if (Exists(
                    model.TrainingID,
                    model.TrainerID))
                {
                    message =
                        "Trainer is already assigned to this training.";

                    return false;
                }


                if (String.IsNullOrWhiteSpace(
                    model.TrainerAssignmentID))
                {
                    model.TrainerAssignmentID =
                        objID.GenerateTrainerAssignmentID();
                }


                string sql =
                    @"
                    INSERT INTO TrainerAssignment
                    (
                        TrainerAssignmentID,
                        TrainingID,
                        TrainerID,
                        TrainerType,
                        SessionID,
                        AssignedBy,
                        AssignedOn,
                        Active,
                        CreatedOn,
                        CreatedBy
                    )
                    VALUES
                    (
                        @TrainerAssignmentID,
                        @TrainingID,
                        @TrainerID,
                        @TrainerType,
                        @SessionID,
                        @AssignedBy,
                        GETDATE(),
                        1,
                        GETDATE(),
                        @CreatedBy
                    )
                    ";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@TrainerAssignmentID",
                        model.TrainerAssignmentID),

                    new SqlParameter(
                        "@TrainingID",
                        model.TrainingID),

                    new SqlParameter(
                        "@TrainerID",
                        model.TrainerID),

                    new SqlParameter(
                        "@TrainerType",
                        String.IsNullOrWhiteSpace(
                            model.TrainerType)
                        ? (object)DBNull.Value
                        : model.TrainerType),

                    new SqlParameter(
                        "@SessionID",
                        String.IsNullOrWhiteSpace(
                            model.SessionID)
                        ? (object)DBNull.Value
                        : model.SessionID),

                    new SqlParameter(
                        "@AssignedBy",
                        String.IsNullOrWhiteSpace(
                            model.AssignedBy)
                        ? (object)DBNull.Value
                        : model.AssignedBy),

                    new SqlParameter(
                        "@CreatedBy",
                        String.IsNullOrWhiteSpace(
                            model.CreatedBy)
                        ? (object)DBNull.Value
                        : model.CreatedBy)
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
                    "Trainer assigned successfully.";

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


        #region Remove

        public bool RemoveTrainer(
            string trainerAssignmentID,
            string modifiedBy,
            out string message)
        {
            message = "";


            try
            {
                if (String.IsNullOrWhiteSpace(
                    trainerAssignmentID))
                {
                    message =
                        "Trainer assignment ID is required.";

                    return false;
                }


                string sql =
                    @"
                    UPDATE TrainerAssignment
                    SET
                        Active=0,
                        ModifiedOn=GETDATE(),
                        ModifiedBy=@ModifiedBy
                    WHERE
                        TrainerAssignmentID=
                        @TrainerAssignmentID
                    ";


                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@TrainerAssignmentID",
                        trainerAssignmentID),

                    new SqlParameter(
                        "@ModifiedBy",
                        modifiedBy)
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
                    "Trainer assignment removed successfully.";

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


        #region Active Check

        public bool IsTrainerAssigned(
            string trainingID,
            string trainerID)
        {
            return
                Exists(
                    trainingID,
                    trainerID);
        }

        #endregion
    }
}