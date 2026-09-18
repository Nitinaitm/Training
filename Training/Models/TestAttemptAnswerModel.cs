using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{


    public class TestAttemptAnswerModel
    {
        public string AttemptAnswerID
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

        public string QuestionID
        {
            get;
            set;
        }

        public int QuestionOrder
        {
            get;
            set;
        }

        public string SelectedOption
        {
            get;
            set;
        }

        public string CorrectOption
        {
            get;
            set;
        }

        public decimal Marks
        {
            get;
            set;
        }

        public bool Answered
        {
            get;
            set;
        }

        public bool MarkForReview
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