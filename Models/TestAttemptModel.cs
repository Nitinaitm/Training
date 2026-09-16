using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{

    public class TestAttemptModel
    {
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

        public int AttemptNo
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }

        public int TotalQuestions
        {
            get;
            set;
        }

        public int CorrectAnswers
        {
            get;
            set;
        }

        public int WrongAnswers
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

        public bool Submitted
        {
            get;
            set;
        }

        public int CurrentQuestionNo
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