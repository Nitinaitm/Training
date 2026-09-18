using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{


    public class SessionModel
    {
        public string SessionID
        {
            get;
            set;
        }

        public string TrainingID
        {
            get;
            set;
        }

        public string SessionNo
        {
            get;
            set;
        }

        public string SessionName
        {
            get;
            set;
        }

        public string TopicID
        {
            get;
            set;
        }

        public string TrainerID
        {
            get;
            set;
        }

        public DateTime? SessionDate
        {
            get;
            set;
        }

        public TimeSpan? StartTime
        {
            get;
            set;
        }

        public TimeSpan? EndTime
        {
            get;
            set;
        }

        public decimal TotalHours
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

        public int DisplayOrder
        {
            get;
            set;
        }
    }
}