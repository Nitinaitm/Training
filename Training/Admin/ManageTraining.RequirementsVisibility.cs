using System;

namespace Training.Admin
{
    public partial class ManageTraining
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (string.IsNullOrWhiteSpace(TrainingID))
            {
                btnRequirements.Visible = false;
                return;
            }

            object workflow = new clsDataAccess().ExecuteScalar("SELECT WorkflowStatus FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
            object status = new clsDataAccess().ExecuteScalar("SELECT TrainingStatus FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));

            bool started = workflow != null && workflow != DBNull.Value && string.Equals(Convert.ToString(workflow), "E", StringComparison.OrdinalIgnoreCase);
            bool closed = status != null && status != DBNull.Value && string.Equals(Convert.ToString(status), "Closed", StringComparison.OrdinalIgnoreCase);

            btnRequirements.Visible = started || closed;
        }
    }
}