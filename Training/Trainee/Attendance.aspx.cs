using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class Attendance : System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        private string
            TrainingID =
            "";

        private string
            EmpID =
            "";

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if
            (
                Session["EmpID"] == null
                ||
                string.IsNullOrWhiteSpace(
                    Session["EmpID"].ToString())
            )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                Session["TrainingID"] == null
                ||
                string.IsNullOrWhiteSpace(
                    Session["TrainingID"].ToString())
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            EmpID =
                Session["EmpID"]
                .ToString()
                .ToUpperInvariant();

            TrainingID =
                Session["TrainingID"]
                .ToString();

            string accessSql =
                @"SELECT TA.EmpID
                  FROM TrainingAssignment TA
                  WHERE TA.TrainingID=@TrainingID
                    AND TA.EmpID=@EmpID
                    AND TA.AssignmentStatus='Assigned'";

            DataTable accessDt =
                objDB.GetDataTable(
                    accessSql,
                    new SqlParameter[]
                    {
                        new SqlParameter(
                            "@TrainingID",
                            TrainingID),
                        new SqlParameter(
                            "@EmpID",
                            EmpID)
                    });

            if
            (
                accessDt == null
                ||
                accessDt.Rows.Count == 0
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            if
            (
                !IsPostBack
            )
            {
                TraineeTrainingSummary1.LoadTraining(
                    TrainingID,
                    EmpID);

                BindAttendanceGrid();
                LoadAttendanceMessage();
                BindPendingSessions();
            }
        }

        private void BindAttendanceGrid()
        {
            string sql =
            @"SELECT
SM.SessionID,
SM.SessionNo,
SM.SessionName,
SM.SessionDate,
SM.StartTime,
SM.EndTime,
ISNULL(SA.AttendanceStatus,'Pending') AS AttendanceStatus,
ISNULL(SA.ModifiedOn, SA.CreatedOn) AS MarkedOn,
CASE
WHEN TM.TrainerType='Internal'
THEN ISNULL(EBM.EmpName,'')
ELSE ISNULL(TM.NameExternal,'')
END
AS MarkedBy,
ISNULL(SA.Remarks,'') AS Remarks
FROM SessionMaster SM
LEFT JOIN SessionAttendance SA
ON SM.SessionID = SA.SessionID
AND SA.EmpID = @EmpID
LEFT JOIN TrainerMaster TM
ON TM.TrainerID =
ISNULL(SA.ModifiedBy,SA.CreatedBy)
LEFT JOIN EmpBasicMaster EBM
ON TM.EmpID = EBM.EmpID
WHERE SM.TrainingID = @TrainingID
ORDER BY SM.SessionDate, SM.SessionNo";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    TrainingID),

                new SqlParameter(
                    "@EmpID",
                    EmpID)
            };

            gvAttendance.DataSource =
                objDB.GetDataTable(
                    sql,
                    param);

            gvAttendance.DataBind();
        }

        private void LoadAttendanceMessage()
        {
            string sql =
            @"SELECT
COUNT(*) AS TotalSession,
SUM(
CASE
WHEN SA.AttendanceStatus IS NOT NULL
THEN 1
ELSE 0
END
) AS CompletedSession
FROM SessionMaster SM
LEFT JOIN SessionAttendance SA
ON SM.SessionID=SA.SessionID
AND SA.EmpID=@EmpID
WHERE SM.TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    TrainingID),

                new SqlParameter(
                    "@EmpID",
                    EmpID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            if
            (
                dt == null
                ||
                dt.Rows.Count == 0
            )
            {
                lblMessage.Text =
                    "No attendance data available.";

                lblMessage.CssClass =
                    "text-danger fw-bold";

                return;
            }

            int total =
                Convert.ToInt32(
                    dt.Rows[0]["TotalSession"]);

            int completed =
                dt.Rows[0]["CompletedSession"] == DBNull.Value
                ?
                0
                :
                Convert.ToInt32(
                    dt.Rows[0]["CompletedSession"]);

            if
            (
                total > 0
                &&
                completed == total
            )
            {
                lblMessage.Text =
                    "Attendance Completed";

                lblMessage.CssClass =
                    "text-success fw-bold";
            }
            else
            {
                lblMessage.Text =
                    completed
                    +
                    " of "
                    +
                    total
                    +
                    " Sessions Completed";

                lblMessage.CssClass =
                    "text-danger fw-bold";
            }
        }

        private void BindPendingSessions()
        {
            string sql =
            @"SELECT
SM.SessionNo,
SM.SessionName,
SM.SessionDate
FROM SessionMaster SM
LEFT JOIN SessionAttendance SA
ON SM.SessionID=SA.SessionID
AND SA.EmpID=@EmpID
WHERE SM.TrainingID=@TrainingID
AND SA.AttendanceID IS NULL
ORDER BY SM.SessionNo";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    TrainingID),

                new SqlParameter(
                    "@EmpID",
                    EmpID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    param);

            blPending.Items.Clear();

            foreach
            (
                DataRow dr
                in
                dt.Rows
            )
            {
                if
                (
                    dr["SessionDate"] == DBNull.Value
                )
                {
                    blPending.Items.Add(
                        "Session "
                        + dr["SessionNo"]
                        + " - "
                        + dr["SessionName"]);

                    continue;
                }

                blPending.Items.Add(
                    "Session "
                    +
                    dr["SessionNo"]
                    +
                    " - "
                    +
                    dr["SessionName"]
                    +
                    " ("
                    +
                    Convert.ToDateTime(
                        dr["SessionDate"])
                    .ToString("dd-MMM-yyyy")
                    +
                    ")");
            }
        }
    }
}