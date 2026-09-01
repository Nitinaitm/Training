using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingSummary : System.Web.UI.UserControl
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void LoadTraining(string trainingID)
        {
            string sql = @"
    SELECT
        TD.TrainingID,
        CM.CourseName,
        CM.CourseDescription,
        CM.CourseCategory,
        TD.TrainingCategory,
        TD.TrainingStatus,
        TD.TrainingType,
        TD.TrainingOrganizer,
        TD.TrainingLocation,
        TD.Batch,
        TD.DateFrom,
        TD.DateTo,
        TD.NoOfDays,
        TD.Hours,
        TD.BatchStrength,
        TD.HostelRequiredTrainee,

        ISNULL(A.Assigned,0) AS Assigned,
        TD.BatchStrength - ISNULL(A.Assigned,0) AS Remaining,

        ISNULL(TM.TopicName,'-') AS TopicName

    FROM TrainingDetails TD

    LEFT JOIN CourseMaster CM
        ON TD.CourseID = CM.CourseID

    LEFT JOIN
    (
        SELECT
            TrainingID,
            COUNT(*) AS Assigned
        FROM TrainingAssignment
        GROUP BY TrainingID
    ) A
        ON TD.TrainingID = A.TrainingID

    LEFT JOIN SessionMaster SM
        ON TD.TrainingID = SM.TrainingID

    LEFT JOIN TopicMaster TM
        ON SM.TopicID = TM.TopicID

    WHERE TD.TrainingID='" + trainingID.Replace("'", "''") + "'";

            DataTable dt = obj.GetDataTable(sql);

            if (dt.Rows.Count == 0)
            {
                ClearData();
                return;
            }

            DataRow dr = dt.Rows[0];

            lblTrainingID.Text = dr["TrainingID"].ToString();
            lblCourse.Text = dr["CourseName"].ToString();
            lblCourseCategory.Text = dr["CourseCategory"].ToString();
            lblCategory.Text = dr["TrainingCategory"].ToString();
            lblStatus.Text = dr["TrainingStatus"].ToString();

            lblTrainingType.Text = dr["TrainingType"].ToString();
            lblOrganizer.Text = dr["TrainingOrganizer"].ToString();
            lblLocation.Text = dr["TrainingLocation"].ToString();
            lblBatch.Text = dr["Batch"].ToString();
            lblHostelRequired.Text = dr["HostelRequiredTrainee"].ToString();

            DateTime fromDate, toDate;

            if (DateTime.TryParse(dr["DateFrom"].ToString(), out fromDate) &&
                DateTime.TryParse(dr["DateTo"].ToString(), out toDate))
            {
                lblTrainingDuration.Text = fromDate.ToString("dd-MM-yyyy") +
                                           " To " +
                                           toDate.ToString("dd-MM-yyyy");
            }
            else
            {
                lblTrainingDuration.Text = "-";
            }

            lblNoOfDays.Text = dr["NoOfDays"].ToString() + " Day(s)";
            lblPlannedHours.Text = dr["Hours"].ToString();
            lblBatchStrength.Text = dr["BatchStrength"].ToString();

            lblAssigned.Text = dr["Assigned"].ToString();
            lblRemaining.Text = dr["Remaining"].ToString();

            //lblTopic.Text = string.IsNullOrWhiteSpace(dr["TopicName"].ToString())
            //    ? "-"
            //    : dr["TopicName"].ToString();

            lblCourseDescription.Text = dr["CourseDescription"].ToString();
        }

        private string GetValue(DataRow dr, string columnName)
        {
            if (dr.Table.Columns.Contains(columnName))
            {
                if (dr[columnName] != DBNull.Value)
                {
                    string value = dr[columnName].ToString().Trim();

                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
            }

            return "-";
        }

        private void ClearData()
        {
            lblTrainingID.Text = "-";
            lblCourse.Text = "-";
            lblCategory.Text = "-";
            lblStatus.Text = "-";

            lblTrainingType.Text = "-";
            lblOrganizer.Text = "-";
            lblLocation.Text = "-";
            lblBatch.Text = "-";

            lblTrainingDuration.Text = "-";
            lblNoOfDays.Text = "-";
            lblPlannedHours.Text = "-";
            lblBatchStrength.Text = "-";

            lblCourseDescription.Text = "-";
            lblCourseCategory.Text = "-";
            lblAssigned.Text = "-";
            lblRemaining.Text = "-";
            lblHostelRequired.Text = "";
           // lblTopic.Text = "-";
        }
    }
}