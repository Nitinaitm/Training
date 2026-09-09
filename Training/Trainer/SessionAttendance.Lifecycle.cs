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

            if (IsPostBack && IsAttendanceSkipped())
            {
                Response.Redirect("~/Trainer/SessionDetails.aspx", true);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (IsAttendanceSkipped())
            {
                gvAttendance.Enabled = false;
                btnSaveAttendance.Enabled = false;
                btnCompleteAttendance.Enabled = false;
                btnUploadExcel.Enabled = false;
                btnUploadAttendanceSheet.Enabled = false;
                lblMessage.ForeColor = System.Drawing.Color.DarkOrange;
                lblMessage.Text = "Attendance has been skipped for this session.";
            }
            base.OnPreRender(e);
        }

        private bool IsAttendanceSkipped()
        {
            string sessionID = Session["SessionID"] == null ? "" : Session["SessionID"].ToString();
            if (string.IsNullOrWhiteSpace(sessionID)) return false;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand("SELECT ISNULL(AttendanceSkipped,0) FROM SessionMaster WHERE SessionID=@SessionID", con))
            {
                cmd.Parameters.AddWithValue("@SessionID", sessionID);
                con.Open();
                object value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
            }
        }

        private bool IsTrainingCompleted()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID)) return false;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ' THEN 1 ELSE 0 END FROM TrainingDetails WHERE TrainingID=@TrainingID", con))
            {
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                con.Open();
                object value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
            }
        }
    }
}
