using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainer
{
    public partial class PreTrainingTest
    {
        protected override void OnLoad(EventArgs e)
        {
            // AutoEventWireup would otherwise execute Page_Load, whose old gate
            // redirected the trainer before a draft could be saved. Rebuild the
            // same initialisation here, but keep attendance as a publish gate.
            if (!IsPostBack)
            {
                if (Session["TrainerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
                if (Session["TrainingID"] == null || Session["SessionID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }

                ViewState["SessionID"] = Session["SessionID"].ToString();
                SessionSummary1.LoadSession(Session["SessionID"].ToString());
                LoadSessionDetails();
                if (!CheckPreTrainingRequired()) return;
                LoadQuestionPool();
                CheckExistingTest();
                return;
            }
            // No Page_Load work is required on postback.
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
