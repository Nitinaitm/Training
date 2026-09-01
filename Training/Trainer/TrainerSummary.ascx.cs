using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Trainer
{
    public partial class TrainerSummary : System.Web.UI.UserControl
    {
        clsDataAccess obj = new clsDataAccess();

        public void LoadTraining(string trainingID)
        {
            LoadTrainingSummary(trainingID);

            LoadSessionSummary(trainingID);
        }

        private void LoadTrainingSummary(string trainingID)
        {
            string query = @"SELECT TD.TrainingID,CM.CourseName,TD.TrainingCategory,TD.TrainingType,TD.Batch,TD.TrainingOrganizer,TD.TrainingLocation,TD.DateFrom,TD.DateTo,TD.NoOfDays,TD.Hours,TD.BatchStrength,TD.TrainingStatus,CM.CourseDescription FROM TrainingDetails TD LEFT JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TD.TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID",trainingID)
            };

            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0) return;

            DataRow dr = dt.Rows[0];

            lblTrainingID.Text = dr["TrainingID"].ToString();

            lblCourse.Text = dr["CourseName"].ToString();

            lblCategory.Text = dr["TrainingCategory"].ToString();

            lblTrainingType.Text = dr["TrainingType"].ToString();

            lblBatch.Text = dr["Batch"].ToString();

            lblOrganizer.Text = dr["TrainingOrganizer"].ToString();

            lblLocation.Text = dr["TrainingLocation"].ToString();

            lblDuration.Text = dr["DateFrom"] + " To " + dr["DateTo"];

            lblDays.Text = dr["NoOfDays"].ToString();

            lblHours.Text = dr["Hours"].ToString();

            lblBatchStrength.Text = dr["BatchStrength"].ToString();

            lblStatus.Text = dr["TrainingStatus"].ToString();

            lblDescription.Text = dr["CourseDescription"].ToString();
        }

        private void LoadSessionSummary(string trainingID)
        {
            string query = @"SELECT COUNT(*) TotalSession,SUM(CASE WHEN AttendanceStatus='Completed' THEN 1 ELSE 0 END) Completed,SUM(CASE WHEN ISNULL(AttendanceStatus,'')<>'Completed' THEN 1 ELSE 0 END) Pending,SUM(CASE WHEN SessionDate=@Today THEN 1 ELSE 0 END) TodaySession FROM SessionMaster WHERE TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID",trainingID),
                new SqlParameter("@Today",DateTime.Now.ToString("dd-MM-yyyy"))
            };

            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0) return;

            DataRow dr = dt.Rows[0];

            lblTotalSession.Text = dr["TotalSession"].ToString();

            lblCompleted.Text = dr["Completed"].ToString();

            lblPending.Text = dr["Pending"].ToString();

            lblToday.Text = dr["TodaySession"].ToString();

            int total = Convert.ToInt32(dr["TotalSession"]);

            int completed = Convert.ToInt32(dr["Completed"]);

            if (total == 0)

                lblAttendance.Text = "0 %";

            else

                lblAttendance.Text = Math.Round((completed * 100.0) / total, 2).ToString() + " %";
        }
    }
}