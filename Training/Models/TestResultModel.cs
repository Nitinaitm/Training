using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{

    public class TestResultModel
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

        public string Result
        {
            get;
            set;
        }

        public DateTime ResultDate
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