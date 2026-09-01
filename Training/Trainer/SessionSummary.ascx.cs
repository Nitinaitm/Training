using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace Training.Trainer
{
    public partial class SessionSummary : System.Web.UI.UserControl
    {
        clsDataAccess obj = new clsDataAccess();

        public void LoadSession(string sessionID)
        {
            LoadSessionDetails(sessionID);

            LoadAttendanceSummary(sessionID);

            LoadMaterialSummary(sessionID);

            //LoadWorkflow(sessionID);
        }

        private void LoadSessionDetails(string sessionID)
        {
            string query = @"SELECT SM.SessionID,SM.SessionNo,SM.SessionName,SM.SessionDate,SM.StartTime,SM.EndTime,SM.TotalHours,SM.SessionStatus,SM.AttendanceStatus,SM.Remarks,TM.TopicName,TR.TrainerType,CASE WHEN TR.TrainerType='Internal' THEN E.EmpName ELSE TR.NameExternal END TrainerName FROM SessionMaster SM LEFT JOIN TopicMaster TM ON SM.TopicID=TM.TopicID LEFT JOIN TrainerMaster TR ON SM.TrainerID=TR.TrainerID LEFT JOIN EmpBasicMaster E ON TR.EmpID=E.EmpID WHERE SM.SessionID=@SessionID";

            SqlParameter[] param =
            {
                new SqlParameter("@SessionID",sessionID)
            };

            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0) return;

            DataRow dr = dt.Rows[0];

            lblSessionID.Text = dr["SessionID"].ToString();

            lblSessionNo.Text = dr["SessionNo"].ToString();

            lblSessionName.Text = dr["SessionName"].ToString();

            lblTopic.Text = dr["TopicName"].ToString();

            lblTrainer.Text = dr["TrainerName"].ToString();

            lblTrainerType.Text = dr["TrainerType"].ToString();

            lblSessionDate.Text = dr["SessionDate"].ToString();

            lblStartTime.Text = dr["StartTime"].ToString();

            lblEndTime.Text = dr["EndTime"].ToString();

            lblHours.Text = dr["TotalHours"].ToString();

            lblSessionStatus.Text = dr["SessionStatus"].ToString();

            lblAttendanceStatus.Text = dr["AttendanceStatus"].ToString();

            //lblRemarks.Text = dr["Remarks"].ToString();

            SetStatusColor();
        }

        private void LoadAttendanceSummary(string sessionID)
        {
            string query = @"SELECT COUNT(TA.EmpID) TotalTrainee,SUM(CASE WHEN SA.AttendanceStatus='Present' THEN 1 ELSE 0 END) PresentCount,SUM(CASE WHEN SA.AttendanceStatus='Absent' THEN 1 ELSE 0 END) AbsentCount FROM SessionMaster SM INNER JOIN TrainingAssignment TA ON SM.TrainingID=TA.TrainingID AND TA.AssignmentStatus='Assigned' LEFT JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=TA.EmpID WHERE SM.SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter("@SessionID",sessionID)
    };

            DataTable dt =
                obj.GetDataTable(
                query,
                param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblTotalTrainee.Text =
                dr["TotalTrainee"].ToString();

            lblPresent.Text =
                dr["PresentCount"] == DBNull.Value
                ? "0"
                : dr["PresentCount"].ToString();

            lblAbsent.Text =
                dr["AbsentCount"] == DBNull.Value
                ? "0"
                : dr["AbsentCount"].ToString();
        }

        private void LoadMaterialSummary(string sessionID)
        {
            string materialQuery = @"SELECT COUNT(*) FROM TrainingMaterial WHERE SessionID=@SessionID AND IsActive=1";

            SqlParameter[] materialParam =
            {
                new SqlParameter("@SessionID",sessionID)
            };

            object material = obj.ExecuteScalar(materialQuery, materialParam);

            lblMaterial.Text = material == null ? "0" : material.ToString();

            string preQuery = @"SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType='PRE' AND IsActive=1";

            SqlParameter[] preParam =
            {
                new SqlParameter("@SessionID",sessionID)
            };

            object pre = obj.ExecuteScalar(preQuery, preParam);

            lblPreTest.Text = pre == null ? "0" : pre.ToString();

            string postQuery = @"SELECT COUNT(*) FROM TestMaster WHERE SessionID=@SessionID AND TestType='POST' AND IsActive=1";

            SqlParameter[] postParam =
            {
                new SqlParameter("@SessionID",sessionID)
            };

            object post = obj.ExecuteScalar(postQuery, postParam);

            lblPostTest.Text = post == null ? "0" : post.ToString();
        }

        private void SetStatusColor()
        {
            lblSessionStatus.CssClass = "badge status-badge";

            lblAttendanceStatus.CssClass = "badge status-badge";

            switch (lblSessionStatus.Text)
            {
                case "Draft":

                    lblSessionStatus.BackColor = Color.Gray;

                    break;

                case "Scheduled":

                    lblSessionStatus.BackColor = Color.RoyalBlue;

                    break;

                case "Completed":

                    lblSessionStatus.BackColor = Color.Green;

                    break;

                case "Cancelled":

                    lblSessionStatus.BackColor = Color.Red;

                    break;

                default:

                    lblSessionStatus.BackColor = Color.DarkOrange;

                    break;
            }

            switch (lblAttendanceStatus.Text)
            {
                case "Completed":

                    lblAttendanceStatus.BackColor = Color.Green;

                    break;

                case "Pending":

                    lblAttendanceStatus.BackColor = Color.DarkOrange;

                    break;

                case "Not Started":

                    lblAttendanceStatus.BackColor = Color.Gray;

                    break;

                default:

                    lblAttendanceStatus.BackColor = Color.RoyalBlue;

                    break;
            }
        }
    //    private void LoadWorkflow(string sessionID)
    //    {
    //        string query = @"SELECT AttendanceStatus FROM SessionMaster WHERE SessionID=@SessionID";

    //        SqlParameter[] param =
    //        {
    //    new SqlParameter("@SessionID",sessionID)
    //};

    //        object attendance = obj.ExecuteScalar(query, param);

    //        string status = attendance == null ? "" : attendance.ToString();

    //        if (status == "Completed")
    //        {
    //            lblAttendanceWorkflow.Text = "Completed";

    //            lblAttendanceWorkflow.CssClass = "badge bg-success";

    //            lblWorkflow.Text = "Attendance Completed";

    //            pnlProgress.Style["width"] = "100%";
    //        }
    //        else
    //        {
    //            lblAttendanceWorkflow.Text = "Pending";

    //            lblAttendanceWorkflow.CssClass = "badge bg-warning";

    //            lblWorkflow.Text = "Attendance Pending";

    //            pnlProgress.Style["width"] = "40%";
    //        }

    //        lblMaterialStatus.Text = Convert.ToInt32(lblMaterial.Text) > 0 ? "Uploaded" : "Pending";

    //        lblMaterialStatus.CssClass = Convert.ToInt32(lblMaterial.Text) > 0 ? "badge bg-success" : "badge bg-warning";

    //        lblPreTestStatus.Text = Convert.ToInt32(lblPreTest.Text) > 0 ? "Created" : "Pending";

    //        lblPreTestStatus.CssClass = Convert.ToInt32(lblPreTest.Text) > 0 ? "badge bg-success" : "badge bg-warning";

    //        lblPostTestStatus.Text = Convert.ToInt32(lblPostTest.Text) > 0 ? "Created" : "Pending";

    //        lblPostTestStatus.CssClass = Convert.ToInt32(lblPostTest.Text) > 0 ? "badge bg-success" : "badge bg-warning";
    //    }
    }
}