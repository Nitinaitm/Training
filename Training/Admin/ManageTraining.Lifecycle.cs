using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Admin
{
    public partial class ManageTraining
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (IsPostBack && IsTrainingCompleted())
            {
                Response.Redirect("TrainingList.aspx", true);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (IsTrainingCompleted())
            {
                btnUpdateTraining.Visible = false;
                btnAssignSession.Visible = false;
                btnAssignTrainee.Visible = false;
                btnAssignFeedback.Visible = false;
                btnAssignHostel.Visible = false;
                btnCertificateTemplate.Visible = false;
                btnStartTraining.Visible = false;
                btnAttendance.Visible = true;
                btnRequirements.Visible = true;
            }

            base.OnPreRender(e);
        }

        private bool IsTrainingCompleted()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID))
                return false;

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted')
                 OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ'
            THEN 1 ELSE 0 END
FROM TrainingDetails
WHERE TrainingID=@TrainingID", con))
            {
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                con.Open();
                object value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
            }
        }
    }
}
