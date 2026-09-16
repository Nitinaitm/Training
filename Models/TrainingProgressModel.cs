using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{
    public class TrainingProgressModel
    {
        public int ID
        {
            get;
            set;
        }

        public string ProgressID
        {
            get;
            set;
        }

        public string TrainingID
        {
            get;
            set;
        }

        public string EmpID
        {
            get;
            set;
        }

        public bool AttendanceCompleted
        {
            get;
            set;
        }

        public bool PreExamCompleted
        {
            get;
            set;
        }

        public bool PostExamCompleted
        {
            get;
            set;
        }

        public bool SessionFeedbackCompleted
        {
            get;
            set;
        }

        public bool BatchFeedbackCompleted
        {
            get;
            set;
        }

        public bool CertificateGenerated
        {
            get;
            set;
        }

        public string WorkflowStatus
        {
            get;
            set;
        }

        public DateTime? UpdatedOn
        {
            get;
            set;
        }

        public string UpdatedBy
        {
            get;
            set;
        }

        public DateTime? CreatedOn
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public DateTime? CertificateGeneratedOn
        {
            get;
            set;
        }
    }
}