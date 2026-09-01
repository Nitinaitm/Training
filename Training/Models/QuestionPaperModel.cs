using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{

    public class QuestionPaperModel
    {
        public string TestID
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

        public string AssessmentScope
        {
            get;
            set;
        }

        public string AssessmentLevel
        {
            get;
            set;
        }

        public string QuestionSelectionMode
        {
            get;
            set;
        }

        public int TotalQuestions
        {
            get;
            set;
        }

        public decimal TotalMarks
        {
            get;
            set;
        }

        public int EasyQuestions
        {
            get;
            set;
        }

        public int MediumQuestions
        {
            get;
            set;
        }

        public int HardQuestions
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }
        public string CourseID
        {
            get;
            set;
        }
    }
}