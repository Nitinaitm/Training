using System;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class ManageTraining : System.Web.UI.Page
    {
        private bool IsFeedbackRequiredButNotAssigned()
        {
            if (Session["TrainingID"] == null)
            {
                return false;
            }

            clsDataAccess db = new clsDataAccess();
            object required = db.ExecuteScalar(
                "SELECT FeedbackRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));

            if (required == null || required == DBNull.Value || !Convert.ToBoolean(required))
            {
                return false;
            }

            object assigned = db.ExecuteScalar(
                "SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID",
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));

            return Convert.ToInt32(assigned) == 0;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (IsPostBack && Request.Form[btnStartTraining.UniqueID] != null && IsFeedbackRequiredButNotAssigned())
            {
                Session["TrainingID"] = Session["TrainingID"].ToString();
                Response.Redirect("CreateBatch.aspx?mode=edit", true);
                return;
            }

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (IsFeedbackRequiredButNotAssigned())
            {
                btnStartTraining.Visible = false;
            }
        }
    }
}