using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class MySessions
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (btnPreTest != null) { btnPreTest.Click -= btnPreTest_Click; btnPreTest.Click += FlexiblePreTest_Click; }
            if (btnPostTest != null) { btnPostTest.Click -= btnPostTest_Click; btnPostTest.Click += FlexiblePostTest_Click; }
        }

        private void FlexiblePreTest_Click(object sender, EventArgs e)
        {
            if (!PreRequired) return;
            if (AttendanceBlocksTests()) { ClientScript.RegisterStartupScript(GetType(), "msg", "alert('Please complete attendance for this session first.');", true); return; }
            if (btnPreTest.CommandArgument == "Result") { Session["ResultTestType"] = "Pre"; Response.Redirect("MyExamResult.aspx"); return; }
            Response.Redirect("PreTrainingExam.aspx");
        }

        private void FlexiblePostTest_Click(object sender, EventArgs e)
        {
            if (!PostRequired) return;
            if (AttendanceBlocksTests()) { ClientScript.RegisterStartupScript(GetType(), "msg", "alert('Please complete attendance for this session first.');", true); return; }
            if (btnPostTest.CommandArgument == "Result") { Session["ResultTestType"] = "Post"; Response.Redirect("MyExamResult.aspx"); return; }
            Response.Redirect("PostTrainingExam.aspx");
        }

        private bool AttendanceBlocksTests()
        {
            if (!AttendanceRequired) return false;
            object value = objDB.ExecuteScalar("SELECT ISNULL(AttendanceSkipped,0) FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            if (value != null && value != DBNull.Value && Convert.ToInt32(value) == 1) return false;
            return !SessionAttendanceDone();
        }

        protected override void OnPreRender(EventArgs e)
        {
            string trainingID = Convert.ToString(Session["TrainingID"]);
            string sessionID = Convert.ToString(Session["SessionID"]);
            if (!string.IsNullOrWhiteSpace(trainingID) && !string.IsNullOrWhiteSpace(sessionID))
            {
                DataTable dt = objDB.GetDataTable(@"SELECT TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,ISNULL(SM.PreAssessmentSkipped,0) PreSkipped,ISNULL(SM.PostAssessmentSkipped,0) PostSkipped,ISNULL(SM.AttendanceSkipped,0) AttendanceSkipped,ISNULL((SELECT TOP 1 TM.IsPublished FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post'),0) PostPublished FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });
                if (dt.Rows.Count > 0)
                {
                    bool preRequired = Convert.ToBoolean(dt.Rows[0]["InitialAssessmentRequired"]);
                    bool postRequired = Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);
                    bool preSkipped = Convert.ToBoolean(dt.Rows[0]["PreSkipped"]);
                    bool postSkipped = Convert.ToBoolean(dt.Rows[0]["PostSkipped"]);
                    bool postPublished = Convert.ToBoolean(dt.Rows[0]["PostPublished"]);
                    btnPreTest.Visible = preRequired && !preSkipped;
                    btnPostTest.Visible = postRequired && !postSkipped;
                    if (postRequired && !postSkipped && postPublished)
                    {
                        btnPostTest.Enabled = true;
                        if (btnPostTest.Text == "Post Test Not Available") btnPostTest.Text = "Start Post Test";
                    }
                }
            }
            base.OnPreRender(e);
        }
    }
}
