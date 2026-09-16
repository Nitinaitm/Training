using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{

    public class AssessmentModel
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

        public string TestTitle
        {
            get;
            set;
        }

        public string TestType
        {
            get;
            set;
        }

        public string AssessmentScope
        {
            get;
            set;
        }

        public string AssessmentStage
        {
            get;
            set;
        }

        public int Duration
        {
            get;
            set;
        }

        public int TotalQuestions
        {
            get;
            set;
        }

        public decimal PassingMarks
        {
            get;
            set;
        }

        public bool RandomQuestion
        {
            get;
            set;
        }

        public bool ShuffleOption
        {
            get;
            set;
        }

        public bool AllowRetest
        {
            get;
            set;
        }

        public int MaxAttempt
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }



        public string AssessmentLevel
        {
            get;
            set;
        }

        public string ConductedByRole
        {
            get;
            set;
        }


        public string QuestionSelectionMode
        {
            get;
            set;
        }



        public decimal TotalMarks
        {
            get;
            set;
        }


        public decimal PassingPercentage
        {
            get;
            set;
        }


        public bool AllowResume
        {
            get;
            set;
        }

        public bool AllowReview
        {
            get;
            set;
        }



        public bool ShowResultImmediately
        {
            get;
            set;
        }

        public bool ShowCorrectAnswer
        {
            get;
            set;
        }

        public decimal NegativeMarking
        {
            get;
            set;
        }

        public DateTime? StartDateTime
        {
            get;
            set;
        }

        public DateTime? EndDateTime
        {
            get;
            set;
        }

      

        public string UpdatedBy
        {
            get;
            set;
        }

    }
}