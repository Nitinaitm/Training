using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class PreTrainingExam
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (IsPostBack)
                return;

            if (Session["EmpID"] == null ||
                Session["TrainingID"] == null ||
                Session["SessionID"] == null)
                return;

            string empID = Session["EmpID"].ToString().ToUpperInvariant();
            string trainingID = Session["TrainingID"].ToString();
            string sessionID = Session["SessionID"].ToString();

            string sql =
                "SELECT CASE " +
                "WHEN ISNULL(TD.AttendanceRequired,0)=0 " +
                "OR ISNULL(SM.AttendanceSkipped,0)=1 THEN 1 " +
                "WHEN EXISTS (SELECT 1 FROM SessionAttendance SA " +
                "WHERE SA.SessionID=SM.SessionID " +
                "AND SA.EmpID=@EmpID " +
                "AND SA.AttendanceStatus IN ('Present','Completed')) THEN 1 " +
                "ELSE 0 END " +
                "FROM SessionMaster SM " +
                "INNER JOIN TrainingDetails TD ON TD.TrainingID=SM.TrainingID " +
                "WHERE SM.SessionID=@SessionID " +
                "AND SM.TrainingID=@TrainingID";

            SqlParameter[] parameters =
            {
                new SqlParameter("@SessionID", sessionID),
                new SqlParameter("@TrainingID", trainingID),
                new SqlParameter("@EmpID", empID)
            };

            clsDataAccess db = new clsDataAccess();
            object result = db.ExecuteScalar(sql, parameters);

            if (result == null ||
                result == DBNull.Value ||
                Convert.ToInt32(result) != 1)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "AttendanceRequired",
                    "alert('Please complete attendance before starting the Pre-Training Test.');window.location='MySessions.aspx';",
                    true);
            }
        }
    }
}
