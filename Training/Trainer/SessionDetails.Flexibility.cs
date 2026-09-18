using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class SessionDetails
    {
        protected override void OnPreRender(EventArgs e)
        {
            string sessionID = Convert.ToString(Session["SessionID"]);
            if (!string.IsNullOrWhiteSpace(sessionID))
            {
                DataTable dt = obj.GetDataTable(@"SELECT TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,ISNULL(SM.AttendanceSkipped,0) AttendanceSkipped,ISNULL(SM.PreAssessmentSkipped,0) PreAssessmentSkipped,ISNULL(SM.PostAssessmentSkipped,0) PostAssessmentSkipped FROM SessionMaster SM INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID WHERE SM.SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", sessionID) });
                if (dt.Rows.Count > 0)
                {
                    bool attendanceSkipped = Convert.ToBoolean(dt.Rows[0]["AttendanceSkipped"]);
                    bool preRequired = Convert.ToBoolean(dt.Rows[0]["InitialAssessmentRequired"]);
                    bool postRequired = Convert.ToBoolean(dt.Rows[0]["FinalAssessmentRequired"]);
                    bool preSkipped = Convert.ToBoolean(dt.Rows[0]["PreAssessmentSkipped"]);
                    bool postSkipped = Convert.ToBoolean(dt.Rows[0]["PostAssessmentSkipped"]);

                    // Attendance skip removes the attendance dependency; it must not
                    // prevent the trainer from configuring Pre/Post tests.
                    if (attendanceSkipped)
                    {
                        btnAttendance.Visible = false;
                        btnSkipAttendance.Visible = false;
                        btnPreTest.Visible = preRequired && !preSkipped;
                        btnPostTest.Visible = postRequired && !postSkipped;
                    }
                }
            }
            base.OnPreRender(e);
        }
    }
}
