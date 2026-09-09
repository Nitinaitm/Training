using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainee
{
    public partial class MySessions
    {
        protected override void OnPreRender(EventArgs e)
        {
            string trainingID = Convert.ToString(Session["TrainingID"]);
            string sessionID = Convert.ToString(Session["SessionID"]);
            string empID = Convert.ToString(Session["EmpID"]);
            if (!string.IsNullOrWhiteSpace(trainingID) && !string.IsNullOrWhiteSpace(sessionID) && !string.IsNullOrWhiteSpace(empID))
            {
                DataTable dt = objDB.GetDataTable(@"SELECT TD.FinalAssessmentRequired,ISNULL(SM.PostAssessmentSkipped,0) PostSkipped,ISNULL(TD.AttendanceRequired,0) AttendanceRequired,ISNULL(SM.AttendanceSkipped,0) AttendanceSkipped,ISNULL(SM.AttendanceStatus,'') AttendanceStatus,ISNULL((SELECT TOP 1 TM.IsPublished FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post'),0) PostPublished FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });
                if (dt.Rows.Count > 0)
                {
                    bool postRequired = Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);
                    bool postSkipped = Convert.ToBoolean(dt.Rows[0]["PostSkipped"]);
                    bool postPublished = Convert.ToBoolean(dt.Rows[0]["PostPublished"]);
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
