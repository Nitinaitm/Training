using System;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class PostTrainingTest : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session["TrainerID"] != null && Session["TrainingID"] != null && Session["SessionID"] != null)
            {
                clsDataAccess db = new clsDataAccess();
                object result = db.ExecuteScalar(
                    "SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                    new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));

                if (result == null || result == DBNull.Value || !Convert.ToBoolean(result))
                {
                    string sessionId = Session["SessionID"].ToString();
                    Response.Redirect("SessionDetails.aspx?SessionID=" + Server.UrlEncode(sessionId), true);
                    return;
                }
            }

            base.OnInit(e);
        }
    }
}