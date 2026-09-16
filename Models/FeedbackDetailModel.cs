using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{
    public class FeedbackDetailModel
    {
        public string FeedbackDetailID { get; set; }

        public string FeedbackID { get; set; }

        public string TrainingID { get; set; }

        public string EmpID { get; set; }

        public string CategoryID { get; set; }

        public string QuestionID { get; set; }

        public string TrainerID { get; set; }

        public string TrainerType { get; set; }

        public string AnswerType { get; set; }

        public int? Rating { get; set; }

        public string Answer { get; set; }

        public DateTime? CreatedOn { get; set; }
    }
}