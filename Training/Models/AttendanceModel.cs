using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{


    public class AttendanceModel
    {
        public string AttendanceID
        {
            get;
            set;
        }

        public string TrainingID
        {
            get;
            set;
        }

        public string SessionID
        {
            get;
            set;
        }

        public string EmpID
        {
            get;
            set;
        }

        public string AttendanceStatus
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

        public string ModifiedBy
        {
            get;
            set;
        }
    }
}