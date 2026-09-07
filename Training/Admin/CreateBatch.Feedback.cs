using System;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CreateBatch : System.Web.UI.Page
    {
        protected Button btnAssignFeedback;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            btnAssignFeedback.Visible = false;

            if (string.IsNullOrWhiteSpace(txtTrainingID.Text))
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd = new SqlCommand(@"
SELECT
    FeedbackRequired,
    TrainingStatus
FROM TrainingDetails
WHERE TrainingID=@TrainingID", con);

                cmd.Parameters.AddWithValue("@TrainingID", txtTrainingID.Text.Trim());

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        return;
                    }

                    bool feedbackRequired = Convert.ToBoolean(dr["FeedbackRequired"]);
                    string trainingStatus = Convert.ToString(dr["TrainingStatus"]);

                    if (!feedbackRequired || string.Equals(trainingStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }
                }

                SqlCommand traineeCmd = new SqlCommand(@"
SELECT COUNT(*)
FROM TrainingAssignment
WHERE TrainingID=@TrainingID
AND ISNULL(AssignmentStatus,'Assigned')='Assigned'", con);
                traineeCmd.Parameters.AddWithValue("@TrainingID", txtTrainingID.Text.Trim());
                int traineeCount = Convert.ToInt32(traineeCmd.ExecuteScalar());

                SqlCommand feedbackCmd = new SqlCommand(@"
SELECT COUNT(*)
FROM TrainingFeedbackCategory
WHERE TrainingID=@TrainingID", con);
                feedbackCmd.Parameters.AddWithValue("@TrainingID", txtTrainingID.Text.Trim());
                int feedbackCount = Convert.ToInt32(feedbackCmd.ExecuteScalar());

                btnAssignFeedback.Visible = true;
                btnAssignFeedback.Enabled = traineeCount > 0;
                btnAssignFeedback.Text = feedbackCount > 0 ? "Feedback Assigned ✓" : "Assign Feedback";
            }
        }

        protected void btnAssignFeedback_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTrainingID.Text))
            {
                return;
            }

            Session["TrainingID"] = txtTrainingID.Text.Trim();
            Response.Redirect("~/Admin/AssignFeedback.aspx");
        }
    }
}"}