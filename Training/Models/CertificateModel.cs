using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{
    public class CertificateModel
    {
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

        public string PDFPath
        {
            get;
            set;
        }

        public DateTime? GeneratedOn
        {
            get;
            set;
        }

        public string GeneratedBy
        {
            get;
            set;
        }

        public string TemplateID
        {
            get;
            set;
        }

        public string CertificateStatus
        {
            get;
            set;
        }

        public string Remarks
        {
            get;
            set;
        }

        public string CertificateHash
        {
            get;
            set;
        }

        public string VerificationCode
        {
            get;
            set;
        }

        public string VerificationURL
        {
            get;
            set;
        }

        public string DownloadedBy
        {
            get;
            set;
        }

        public string PDFName
        {
            get;
            set;
        }

        public string GeneratedMode
        {
            get;
            set;
        }

        public string GeneratedFrom
        {
            get;
            set;
        }

        public int CertificateVersion
        {
            get;
            set;
        }

        public int DownloadCount
        {
            get;
            set;
        }

        public string GeneratedIP
        {
            get;
            set;
        }

        public bool Active
        {
            get;
            set;
        }
    }
}