using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{
    public class CertificateVerificationModel
    {
        public bool IsValid
        {
            get;
            set;
        }

        public string CertificateID
        {
            get;
            set;
        }

        public string CertificateNo
        {
            get;
            set;
        }

        public string VerificationCode
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

        public string EmpName
        {
            get;
            set;
        }

        public string EmpDesignation
        {
            get;
            set;
        }

        public string CourseTitle
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

        public DateTime? GeneratedOn
        {
            get;
            set;
        }

        public string CertificateStatus
        {
            get;
            set;
        }

        public int CertificateVersion
        {
            get;
            set;
        }

        public string PDFPath
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }
    }
}