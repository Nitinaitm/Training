using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Common
{

    public class Constants
    {
        public class UserRole
        {
            public const string SuperAdmin =
                "Super Admin";

            public const string Admin =
                "Admin";

            public const string Organizer =
                "Organizer";

            public const string Trainer =
                "Trainer";

            public const string Trainee =
                "Trainee";
        }

        public class TrainingStatus
        {
            public const string Draft =
                "Draft";

            public const string Published =
                "Published";

            public const string Running =
                "Running";

            public const string Completed =
                "Completed";

            public const string Closed =
                "Closed";

            public const string Cancelled =
                "Cancelled";
        }

        public class SessionStatus
        {
            public const string Scheduled =
                "Scheduled";

            public const string Running =
                "Running";

            public const string Completed =
                "Completed";

            public const string Cancelled =
                "Cancelled";

            public const string Rescheduled =
                "Rescheduled";
        }

        public class AttendanceStatus
        {
            public const string Present =
                "Present";

            public const string Absent =
                "Absent";

            public const string Leave =
                "Leave";

            public const string Late =
                "Late";

            public const string Pending =
                "Pending";
        }

        public class AssessmentMode
        {
            public const string Batch =
                "Batch";

            public const string Session =
                "Session";

            public const string Both =
                "Both";
        }

        public class AssessmentStage
        {
            public const string Initial =
                "Initial";

            public const string Session =
                "Session";

            public const string Final =
                "Final";
        }

        public class AssessmentConductedBy
        {
            public const string Trainer =
                "Trainer";

            public const string Organizer =
                "Organizer";

            public const string Admin =
                "Admin";
        }

        public class WorkflowStage
        {
            public const string BatchCreated =
                "WF01";

            public const string SessionAssigned =
                "WF02";

            public const string TrainerAssigned =
                "WF03";

            public const string TraineeAssigned =
                "WF04";

            public const string Ready =
                "WF05";

            public const string Attendance =
                "WF06";

            public const string Assessment =
                "WF07";

            public const string Feedback =
                "WF08";

            public const string Certificate =
                "WF09";

            public const string Completed =
                "WF10";

            public const string Closed =
                "WF11";
        }

        public class CertificateStatus
        {
            public const string Pending =
                "Pending";

            public const string Generated =
                "Generated";

            public const string Downloaded =
                "Downloaded";

            public const string Cancelled =
                "Cancelled";
        }

        public class AssignmentStatus
        {
            public const string Assigned =
                "Assigned";

            public const string Cancelled =
                "Cancelled";
        }

        public class TrainerType
        {
            public const string Internal =
                "Internal";

            public const string External =
                "External";
        }

        public class EmployeeType
        {
            public const string Internal =
                "Internal";

            public const string External =
                "External";
        }

        public class Gender
        {
            public const string Male =
                "Male";

            public const string Female =
                "Female";

            public const string Other =
                "Other";
        }

        public class YesNo
        {
            public const string Yes =
                "Yes";

            public const string No =
                "No";
        }

        #region Question Approval

        public class QuestionApproval
        {
            public const string Pending =
                "Pending";

            public const string Approved =
                "Approved";

            public const string Rejected =
                "Rejected";
        }

        #endregion

        #region Difficulty Level

        public class Difficulty
        {
            public const string Easy =
                "Easy";

            public const string Medium =
                "Medium";

            public const string Hard =
                "Hard";
        }

        #endregion

        #region Question Type

        public class QuestionType
        {
            public const string MCQ =
                "MCQ";

            public const string TrueFalse =
                "TrueFalse";

            public const string FillBlank =
                "FillBlank";

            public const string Subjective =
                "Subjective";
        }

        #endregion

        #region Question Language

        public class QuestionLanguage
        {
            public const string English =
                "English";

            public const string Hindi =
                "Hindi";

            public const string Bilingual =
                "Bilingual";
        }

        #endregion

        #region Question Owner

        public class QuestionOwner
        {
            public const string Admin =
                "Admin";

            public const string Trainer =
                "Trainer";

            public const string Organization =
                "Organization";
        }

        #endregion

        #region Test Status

        public class TestStatus
        {
            public const string Draft =
                "Draft";

            public const string Published =
                "Published";

            public const string Closed =
                "Closed";
        }

        #endregion

        #region Assessment Scope

        public class AssessmentScope
        {
            public const string Batch =
                "Batch";

            public const string Session =
                "Session";

            public const string Final =
                "Final";
        }

        #endregion

        #region Assessment Level

        public class AssessmentLevel
        {
            public const string Initial =
                "Initial";

            public const string Session =
                "Session";

            public const string Final =
                "Final";
        }

        #endregion

        #region Question Selection

        public class QuestionSelectionMode
        {
            public const string Random =
                "Random";

            public const string Manual =
                "Manual";
        }

        #endregion

        public static class ResultStatus
        {
            public const string Pass =
                "Pass";

            public const string Fail =
                "Fail";
        }
    }
}