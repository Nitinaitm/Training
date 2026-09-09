using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainer
{
    public partial class PostTrainingTest
    {
        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
                if (Session["TrainingID"] == null || Session["SessionID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }

                ViewState["SessionID"] = Session["SessionID"].ToString();
                SessionSummary1.LoadSession(Session["SessionID"].ToString());
                LoadSessionDetails();
                if (!CheckPostTrainingRequired()) return;
                LoadQuestionPool();
                CheckExistingTest();
                return;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            bool attendanceReady = AttendanceReadyOrSkipped();
            btnPublish.Enabled = attendanceReady && btnPublish.Text != "Published";
            if (!attendanceReady && btnPublish.Text != "Published") btnPublish.Text = "Publish (Attendance Pending)";
            base.OnPreRender(e);
        }

        private bool AttendanceReadyOrSkipped()
        {
            string sessionID = Convert.ToString(Session["SessionID"]);
            if (string.IsNullOrWhiteSpace(sessionID)) return false;
            object value = objDB.ExecuteScalar("SELECT CASE WHEN ISNULL(AttendanceSkipped,0)=1 OR ISNULL(AttendanceStatus,'')='Completed' THEN 1 ELSE 0 END FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }
    }
}
