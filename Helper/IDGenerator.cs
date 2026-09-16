using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
namespace Training.Helper
{
    public class IDGenerator
    {
        private clsDataAccess objDB =
            new clsDataAccess();

        //--------------------------------------------------
        // Generic ID Generator
        //--------------------------------------------------

        private string GenerateID(
            string tableName,
            string columnName,
            string prefix,
            int length)
        {
            string sql =

                "SELECT " +

                "ISNULL(" +

                "MAX(CAST(RIGHT(" +

                columnName +

                "," +

                length +

                ") AS INT)),0) + 1 " +

                "FROM " +

                tableName +

                " " +

                "WHERE " +

                columnName +

                " LIKE @Prefix";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@Prefix",
                    prefix + "%")
            };

            object obj =
                objDB.ExecuteScalar(
                sql,
                param);

            int nextNo =
                1;

            if
            (
                obj != null &&
                obj != DBNull.Value
            )
            {
                int.TryParse(
                    obj.ToString(),
                    out nextNo);
            }

            return
                prefix +
                nextNo.ToString()
                .PadLeft(
                    length,
                    '0');
        }

        //--------------------------------------------------
        // Training
        //--------------------------------------------------

        public string GenerateTrainingID()
        {
            return
                GenerateID(
                "TrainingDetails",
                "TrainingID",
                "TRN",
                6);
        }

        //--------------------------------------------------
        // Session
        //--------------------------------------------------

        public string GenerateSessionID()
        {
            return
                GenerateID(
                "SessionMaster",
                "SessionID",
                "SES",
                6);
        }

        //--------------------------------------------------
        // Assignment
        //--------------------------------------------------

        public string GenerateAssignmentID()
        {
            return
                GenerateID(
                "TrainingAssignment",
                "AssignmentID",
                "ASN",
                6);
        }

        //--------------------------------------------------
        // Session Attendance
        //--------------------------------------------------

        public string GenerateAttendanceID()
        {
            return
                GenerateID(
                "SessionAttendance",
                "AttendanceID",
                "ATT",
                6);
        }

        //--------------------------------------------------
        // Trainer Attendance
        //--------------------------------------------------

        public string GenerateTrainerAttendanceID()
        {
            return
                GenerateID(
                "TrainerAttendance",
                "AttendanceID",
                "TAT",
                6);
        }

        //--------------------------------------------------
        // Question
        //--------------------------------------------------

        public string GenerateQuestionID()
        {
            return
                GenerateID(
                "QuestionBank",
                "QuestionID",
                "QUE",
                6);
        }

        //--------------------------------------------------
        // Test
        //--------------------------------------------------

        public string GenerateTestID()
        {
            return
                GenerateID(
                "TestMaster",
                "TestID",
                "TST",
                6);
        }

        //--------------------------------------------------
        // Test Topic Mapping
        //--------------------------------------------------

        public string GenerateMappingID()
        {
            return
                GenerateID(
                "TestTopicMapping",
                "MappingID",
                "MAP",
                6);
        }


        //--------------------------------------------------
        // Test Attempt
        //--------------------------------------------------

        public string GenerateAttemptID()
        {
            return
                GenerateID(
                "TestAttempt",
                "AttemptID",
                "ATP",
                6);
        }

        //--------------------------------------------------
        // Test Attempt Answer
        //--------------------------------------------------

        public string GenerateAttemptAnswerID()
        {
            return
                GenerateID(
                "TestAttemptAnswer",
                "AttemptAnswerID",
                "ATA",
                6);
        }

        //--------------------------------------------------
        // Test Result
        //--------------------------------------------------

        public string GenerateResultID()
        {
            return
                GenerateID(
                "TestResult",
                "ResultID",
                "RST",
                6);
        }

        //--------------------------------------------------
        // Certificate
        //--------------------------------------------------

        public string GenerateCertificateID()
        {
            return
                GenerateID(
                "TrainingCertificate",
                "CertificateID",
                "CRT",
                6);
        }

        public string GenerateCertificateNo()
        {
            return
                GenerateID(
                "TrainingCertificate",
                "CertificateNo",
                "CERT",
                6);
        }

        //--------------------------------------------------
        // Certificate Template
        //--------------------------------------------------

        public string GenerateTrainingTemplateID()
        {
            return
                GenerateID(
                "TrainingCertificateTemplate",
                "TrainingTemplateID",
                "TMP",
                4);
        }

        //--------------------------------------------------
        // Organizer
        //--------------------------------------------------

        public string GenerateOrganizerID()
        {
            return
                GenerateID(
                "OrganizerMaster",
                "OrganizerID",
                "ORG",
                4);
        }

        //--------------------------------------------------
        // Session Change
        //--------------------------------------------------

        public string GenerateSessionChangeID()
        {
            return
                GenerateID(
                "SessionChangeLog",
                "SCRID",
                "SCR",
                6);
        }

        //--------------------------------------------------
        // Workflow Audit
        //--------------------------------------------------

        public string GenerateWorkflowAuditID()
        {
            return
                GenerateID(
                "WorkflowAudit",
                "AuditID",
                "AUD",
                6);
        }

        //--------------------------------------------------
        // Notification
        //--------------------------------------------------

        public string GenerateNotificationID()
        {
            return
                GenerateID(
                "NotificationLog",
                "NotificationID",
                "NOT",
                6);
        }

        //--------------------------------------------------
        // Error Log
        //--------------------------------------------------

        public string GenerateErrorID()
        {
            return
                GenerateID(
                "ErrorLog",
                "ErrorID",
                "ERR",
                6);
        }

        //--------------------------------------------------
        // Feedback
        //--------------------------------------------------

        public string GenerateFeedbackID()
        {
            return
                GenerateID(
                "FeedbackReport",
                "FeedbackID",
                "FDB",
                6);
        }
        public string GenerateTestQuestionID()
        {
            return
                GenerateID(
                "TestQuestion",
                "TestQuestionID",
                "TQ",
                6);
        }

        //--------------------------------------------------
        // Training Progress
        //--------------------------------------------------

        public string GenerateProgressID()
        {
            return
                GenerateID(
                "TrainingProgress",
                "ProgressID",
                "PRG",
                6);
        }

        public string GenerateTrainerAssignmentID()
        {
            return
                GenerateID(
                "TrainerAssignmentID",
                "TrainerID",
                "TA",
                6);
        }
    }
}
