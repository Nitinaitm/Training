using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainer
{
    public partial class SessionAttendance
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (IsPostBack && IsTrainingCompleted())
            {
                Response.Redirect("~/Trainer/Default.aspx", true);
            }
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
