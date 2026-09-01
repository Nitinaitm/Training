using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Training.Models
{
    public class TrainerAssignmentModel
    {
        public int ID { get; set; }

        public string TrainerAssignmentID { get; set; }

        public string TrainingID { get; set; }

        public string TrainerID { get; set; }

        public string TrainerType { get; set; }

        public string SessionID { get; set; }

        public string AssignedBy { get; set; }

        public DateTime? AssignedOn { get; set; }

        public bool Active { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }
    }
}