using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{

    public class ResultModel
    {
        public string ResultID
        {
            get;
            set;
        }

        public string AttemptID
        {
            get;
            set;
        }

        public string TestID
        {
            get;
            set;
        }

        public string EmpID
        {
            get;
            set;
        }

        public decimal ObtainedMarks
        {
            get;
            set;
        }

        public decimal TotalMarks
        {
            get;
            set;
        }

        public decimal Percentage
        {
            get;
            set;
        }

        public string ResultStatus
        {
            get;
            set;
        }

        public bool CertificateEligible
        {
            get;
            set;
        }

        public bool Published
        {
            get;
            set;
        }

        public DateTime CreatedOn
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }
    }
}