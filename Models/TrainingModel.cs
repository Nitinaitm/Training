using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{


    public class TrainingModel
    {
        public string TrainingID
        {
            get;
            set;
        }

        public string TrainingType
        {
            get;
            set;
        }

        public string TrainingOrganizer
        {
            get;
            set;
        }

        public string TrainingLocation
        {
            get;
            set;
        }

        public string CourseID
        {
            get;
            set;
        }

        public DateTime? DateFrom
        {
            get;
            set;
        }

        public DateTime? DateTo
        {
            get;
            set;
        }

        public string TrainingCategory
        {
            get;
            set;
        }

        public int NoOfDays
        {
            get;
            set;
        }

        public decimal Hours
        {
            get;
            set;
        }

        public int BatchStrength
        {
            get;
            set;
        }

        public bool AttendanceRequired
        {
            get;
            set;
        }

        public bool AssessmentRequired
        {
            get;
            set;
        }

        public string AssessmentMode
        {
            get;
            set;
        }

        public bool InitialAssessmentRequired
        {
            get;
            set;
        }

        public bool SessionAssessmentRequired
        {
            get;
            set;
        }

        public bool FinalAssessmentRequired
        {
            get;
            set;
        }

        public string AssessmentConductedBy
        {
            get;
            set;
        }

        public bool FeedbackRequired
        {
            get;
            set;
        }

        public bool CertificateRequired
        {
            get;
            set;
        }

        public bool TrainerHostelRequired
        {
            get;
            set;
        }

        public bool TraineeHostelRequired
        {
            get;
            set;
        }

        public string OfficeOrderNo
        {
            get;
            set;
        }

        public DateTime? OfficeOrderDate
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public string UpdatedBy
        {
            get;
            set;
        }

        public string LastUpdatedByRole
        {
            get;
            set;
        }

        public string AttendanceMode
        {
            get;
            set;
        }

        public int BatchAttendanceFrequency
        {
            get;
            set;
        }

        public bool BatchAttendanceRequired
        {
            get;
            set;
        }

        public bool SessionAttendanceRequired
        {
            get;
            set;
        }
    }
}