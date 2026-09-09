using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class CreateBatch
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            string mode = Request.QueryString["mode"] == null ? "" : Request.QueryString["mode"].ToString();

            if (mode.Equals("edit", StringComparison.OrdinalIgnoreCase) && IsTrainingCompleted(trainingID))
            {
                Response.Redirect("ManageTraining.aspx", true);
            }
        }

        private bool IsTrainingCompleted(string trainingID)
        {
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
