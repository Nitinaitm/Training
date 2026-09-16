using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Admin
{
    public partial class AssignTrainee
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (IsPostBack && IsTrainingCompleted())
            {
                Response.Redirect("ManageTraining.aspx", true);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            ReopenAttendanceWhenNewTraineeNeedsAttendance();
            base.OnPreRender(e);
        }

        private bool IsTrainingCompleted()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID))
                return false;

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted')
                 OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ'
            THEN 1 ELSE 0 END
FROM TrainingDetails
WHERE TrainingID=@TrainingID", con))
            {
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                con.Open();
                object value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
            }
        }

        private void ReopenAttendanceWhenNewTraineeNeedsAttendance()
        {
            string trainingID = Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString();
            if (string.IsNullOrWhiteSpace(trainingID) || !IsPostBack || IsTrainingCompleted())
                return;

            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand check = new SqlCommand(@"
SELECT CASE WHEN EXISTS
(
    SELECT 1
    FROM SessionMaster S
    WHERE S.TrainingID=@TrainingID
      AND S.AttendanceStatus='Completed'
)
AND EXISTS
(
    SELECT 1
    FROM TrainingAssignment A
    INNER JOIN SessionMaster S ON S.TrainingID=A.TrainingID
    WHERE A.TrainingID=@TrainingID
      AND A.AssignmentStatus='Assigned'
      AND S.AttendanceStatus='Completed'
      AND ISNULL(S.AttendanceSkipped,0)=0
      AND NOT EXISTS
      (
          SELECT 1
          FROM SessionAttendance SA
          WHERE SA.SessionID=S.SessionID
            AND SA.TrainingID=A.TrainingID
            AND SA.EmpID=A.EmpID
            AND ISNULL(SA.AttendanceStatus,'') IN ('Present','Absent')
      )
)
THEN 1 ELSE 0 END", con);

                check.Parameters.AddWithValue("@TrainingID", trainingID);
                int needsReopen = Convert.ToInt32(check.ExecuteScalar());

                if (needsReopen == 1)
                {
                    SqlCommand reopen = new SqlCommand(@"
UPDATE SessionMaster
SET AttendanceStatus=NULL,
    AttendanceCompletedOn=NULL,
    AttendanceCompletedBy=NULL
WHERE TrainingID=@TrainingID
  AND AttendanceStatus='Completed'
  AND ISNULL(AttendanceSkipped,0)=0;

UPDATE TrainingDetails
SET WorkflowStatus='E',
    TrainingStatus='InProgress',
    UpdatedOn=GETDATE(),
    UpdatedBy=@UpdatedBy
WHERE TrainingID=@TrainingID", con);

                    reopen.Parameters.AddWithValue("@TrainingID", trainingID);
                    reopen.Parameters.AddWithValue("@UpdatedBy", Session["UserID"] == null ? "Admin" : Session["UserID"].ToString());
                    reopen.ExecuteNonQuery();
                }
            }
        }
    }
}
