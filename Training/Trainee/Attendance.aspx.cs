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
               
            )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                Session["TrainingID"]
                ==
                null
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            EmpID =
                Session["UserID"]
                .ToString().ToUpperInvariant();

            TrainingID =
                Session["TrainingID"]
                .ToString();

            if
            (
                !IsPostBack
            )
            {
                string trainingID =
        Session["TrainingID"].ToString();

                string empID =
                    Session["EmpID"].ToString().ToUpperInvariant();

                TraineeTrainingSummary1.LoadTraining(
                    trainingID,
                    empID);

                BindAttendanceGrid();
                LoadAttendanceMessage();
                BindPendingSessions();

            }
        }

//        private void LoadTrainingDetails()
//        {
//            string sql =
//            @"
//SELECT
//TD.TrainingID,
//CM.CourseName,
//TD.TrainingType,
//TOM.TrainingOrganizer,
//TD.TrainingLocation,
//TD.Batch,
//TD.DateFrom,
//TD.DateTo,
//CASE
//WHEN TM.TrainerType='Internal'
//THEN ISNULL(
//EBM.EmpName,
//'')
//ELSE ISNULL(
//TM.NameExternal,
//'')
//END
//AS TrainerName
//FROM
//TrainingDetails TD
//LEFT JOIN
//CourseMaster CM
//ON
//TD.CourseID=
//CM.CourseID
//LEFT JOIN
//TrainingOrganizerMaster TOM
//ON
//TD.TrainingOrganizer=
//TOM.TrainingOrganizerID
//LEFT JOIN
//SessionMaster SM
//ON
//TD.TrainingID=
//SM.TrainingID
//LEFT JOIN
//TrainerMaster TM
//ON
//SM.TrainerID=
//TM.TrainerID
//LEFT JOIN
//EmpBasicMaster EBM
//ON
//TM.EmpID=
//EBM.EmpID
//WHERE
//TD.TrainingID=
//@TrainingID
//";

//            SqlParameter[] param =
//            {
//                new SqlParameter(
//                    "@TrainingID",
//                    TrainingID)
//            };

//            DataTable dt =
//                objDB.GetDataTable(
//                    sql,
//                    param);

//            if
//            (
//                dt.Rows.Count
//                ==
//                0
//            )
//            {
//                Response.Redirect(
//                    "MyTrainings.aspx");

//                return;
//            }

//            DataRow dr =
//                dt.Rows[0];

//            lblTrainingID.Text =
//                dr["TrainingID"]
//                .ToString();

//            lblCourse.Text =
//                dr["CourseName"]
//                .ToString();

//            lblTrainingType.Text =
//                dr["TrainingType"]
//                .ToString();

//            lblOrganizer.Text =
//                dr["TrainingOrganizer"]
//                .ToString();

//            lblLocation.Text =
//                dr["TrainingLocation"]
//                .ToString();

//            lblBatch.Text =
//                dr["Batch"]
//                .ToString();

//            lblTrainer.Text =
//                dr["TrainerName"]
//                .ToString();

//            lblDuration.Text =
//                Convert.ToDateTime(
//                dr["DateFrom"])
//                .ToString(
//                "dd-MMM-yyyy")
//                +
//                " To "
//                +
//                Convert.ToDateTime(
//                dr["DateTo"])
//                .ToString(
//                "dd-MMM-yyyy");
//        }

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
        new SqlParameter("@TrainingID",TrainingID),
        new SqlParameter("@EmpID",EmpID)
    };

            //DataTable dt =
            //    objDB.GetDataTable(
            //        sql,
            //        param);
            DataTable dt =
    objDB.GetDataTable(
        sql,
        param);

            if (dt == null)
            {
                lblMessage.Text = "DataTable is null.";
                return;
            }

            if (dt.Rows.Count == 0)
            {
                lblMessage.Text =
                    "Rows = 0<br/>TrainingID = "
                    + TrainingID
                    + "<br/>EmpID = "
                    + EmpID;

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
                completed
                ==
                total
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
ORDER BY
SM.SessionNo";

            SqlParameter[] param =
            {
        new SqlParameter("@TrainingID",TrainingID),
        new SqlParameter("@EmpID",EmpID)
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