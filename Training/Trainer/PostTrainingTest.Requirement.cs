using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainer
{
    public partial class PostTrainingTest : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session["TrainerID"] == null || Session["TrainingID"] == null || Session["SessionID"] == null)
            {
                base.OnInit(e);
                return;
            }

            clsDataAccess db = new clsDataAccess();
            object result = db.ExecuteScalar(
                "SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));

            if (result == null || result == DBNull.Value || !Convert.ToBoolean(result))
            {
                string sessionId = Session["SessionID"].ToString().Replace("'", "");
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "PostTrainingRequiredGuard",
                    "window.location='SessionDetails.aspx?SessionID=" + sessionId + "';",
                    true);
                Context.Items["PostTrainingBlocked"] = true;
            }

            base.OnInit(e);
        }
    }
}