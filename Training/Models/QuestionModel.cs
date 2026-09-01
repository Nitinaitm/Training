using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{

    public class QuestionModel
    {
        public string QuestionID
        {
            get;
            set;
        }

        public string QuestionOwnerType
        {
            get;
            set;
        }

        public string OwnerID
        {
            get;
            set;
        }

        public string CourseID
        {
            get;
            set;
        }

        public string TopicID
        {
            get;
            set;
        }

        public string Question
        {
            get;
            set;
        }

        public string OptionA
        {
            get;
            set;
        }

        public string OptionB
        {
            get;
            set;
        }

        public string OptionC
        {
            get;
            set;
        }

        public string OptionD
        {
            get;
            set;
        }

        public string CorrectOption
        {
            get;
            set;
        }

        public string DifficultyLevel
        {
            get;
            set;
        }

        public decimal Marks
        {
            get;
            set;
        }

        public decimal NegativeMarks
        {
            get;
            set;
        }

        public string Explanation
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        }

        public string QuestionType
        {
            get;
            set;
        }

        public string ImagePath
        {
            get;
            set;
        }

        public string ExplanationImage
        {
            get;
            set;
        }

        public bool IsActive
        {
            get;
            set;
        }

        public string ApprovalStatus
        {
            get;
            set;
        }

        public string ApprovedBy
        {
            get;
            set;
        }

        public DateTime? ApprovedOn
        {
            get;
            set;
        }

        public string RejectionReason
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
    }
}