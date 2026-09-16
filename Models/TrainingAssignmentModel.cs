using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{
    public class TrainingAssignmentModel
    {
        public int ID
        {
            get;
            set;
        }

        public string AssignmentID
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

        public string TrainingAttended
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

        public string AssignmentMode
        {
            get;
            set;
        }

        public string AssignmentStatus
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