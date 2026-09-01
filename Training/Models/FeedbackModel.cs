using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{


    public class FeedbackModel
    {
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

        public decimal OverallRating
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
    }
}