using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training
{
    public partial class TrainingHistory : Page
    {
        clsDataAccess obj = new clsDataAccess();

        private string TrainingID { get { return Request.QueryString["TrainingID"]; } }
        private string Role { get { return Session["Role"] == null ? "" : Session["Role"].ToString(); } }
        private string EmpID { get { return Session["EmpID"] == null ? "" : Session["EmpID"].ToString().Trim().ToUpperInvariant(); } }
        private string TrainerID { get { return Session["TrainerID"] == null ? "" : Session["TrainerID"].ToString().Trim(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TrainingID)) { GoDashboard(); return; }
            if (string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(EmpID) || !HasTraineeAccess()) { GoDashboard(); return; }
            }
            else if (string.Equals(Role, "Trainer", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(TrainerID) || !HasTrainerAccess()) { GoDashboard(); return; }
            }
            else { GoDashboard(); return; }
            if (!IsClosedTraining()) { GoDashboard(); return; }
            if (!IsPostBack)
            {
                LoadTraining();
                BindAttendance();
                BindTests();
                BindQuestions();
                BindFeedback();
                SetCertificateButton();
            }
        }

        private bool HasTraineeAccess()
        {
            object value = obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND AssignmentStatus='Assigned'", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        private bool HasTrainerAccess()
        {
            object value = obj.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND TrainerID=@TrainerID", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@TrainerID", TrainerID) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        private bool IsClosedTraining()
        {
            object value = obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingDetails WHERE TrainingID=@TrainingID AND TrainingStatus='Closed'", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        private void LoadTraining()
        {
            string sql = "SELECT TD.TrainingID,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TD.DateFrom,TD.DateTo,TD.TrainingStatus FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID AND TD.TrainingStatus='Closed'";
            DataTable dt = obj.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            if (dt.Rows.Count == 0) { GoDashboard(); return; }
            DataRow r = dt.Rows[0];
            lblTrainingID.Text = r["TrainingID"].ToString();
            lblTrainingType.Text = r["TrainingType"].ToString();
            lblOrganizer.Text = r["TrainingOrganizer"].ToString();
            lblBatch.Text = r["Batch"].ToString();
            lblDateFrom.Text = FormatDate(r["DateFrom"]);
            lblDateTo.Text = FormatDate(r["DateTo"]);
            lblStatus.Text = "Closed";
        }

        private string FormatDate(object value)
        {
            if (value == null || value == DBNull.Value) return "";
            DateTime date;
            if (DateTime.TryParse(value.ToString(), out date)) return date.ToString("dd-MM-yyyy");
            return value.ToString();
        }

        private void BindAttendance()
        {
            string sql;
            SqlParameter[] parameters;
            if (string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase))
            {
                sql = "SELECT SM.SessionNo,SM.SessionName,SM.SessionDate,ISNULL(SA.AttendanceStatus,'Not Marked') AS AttendanceStatus,ISNULL(SM.TrainerID,'') AS TrainerName FROM SessionMaster SM LEFT JOIN SessionAttendance SA ON SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID WHERE SM.TrainingID=@TrainingID ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo)";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) };
            }
            else
            {
                sql = "SELECT SM.SessionNo,SM.SessionName,SM.SessionDate,ISNULL(SM.AttendanceStatus,'Pending') AS AttendanceStatus,SM.TrainerID AS TrainerName FROM SessionMaster SM WHERE SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(int,SM.SessionNo)";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@TrainerID", TrainerID) };
            }
            gvAttendance.DataSource = obj.GetDataTable(sql, parameters);
            gvAttendance.DataBind();
        }

        private void BindTests()
        {
            string sql;
            SqlParameter[] parameters;
            if (string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase))
            {
                sql = @"SELECT TM.TestType,TM.TestTitle,SM.SessionName,CONVERT(varchar(10),R.SubmittedOn,105) AS TestDate,
                        DENSE_RANK() OVER(PARTITION BY TM.TestID,R.EmpID ORDER BY R.SubmittedOn) AS AttemptNo,
                        R.TotalQuestions,R.CorrectAnswers,R.Score,R.Status,CONVERT(varchar(16),R.SubmittedOn,105) AS SubmittedOn
                        FROM TestResult R INNER JOIN TestMaster TM ON TM.TestID=R.TestID INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID
                        WHERE SM.TrainingID=@TrainingID AND R.EmpID=@EmpID
                        ORDER BY TRY_CONVERT(date,R.SubmittedOn),TM.TestType,R.SubmittedOn";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) };
            }
            else
            {
                sql = @"SELECT TM.TestType,TM.TestTitle,SM.SessionName,CONVERT(varchar(10),R.SubmittedOn,105) AS TestDate,
                        DENSE_RANK() OVER(PARTITION BY TM.TestID,R.EmpID ORDER BY R.SubmittedOn) AS AttemptNo,
                        R.EmpID,R.TotalQuestions,R.CorrectAnswers,R.Score,R.Status,CONVERT(varchar(16),R.SubmittedOn,105) AS SubmittedOn
                        FROM TestResult R INNER JOIN TestMaster TM ON TM.TestID=R.TestID INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID
                        WHERE SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID AND TM.TrainerID=@TrainerID
                        ORDER BY TRY_CONVERT(date,R.SubmittedOn),TM.TestType,R.SubmittedOn";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@TrainerID", TrainerID) };
            }
            gvTests.DataSource = obj.GetDataTable(sql, parameters);
            gvTests.DataBind();
        }

        private void BindQuestions()
        {
            string sql;
            SqlParameter[] parameters;
            if (string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase))
            {
                sql = @"SELECT TM.TestType,TM.TestTitle,R.EmpID,
                        DENSE_RANK() OVER(PARTITION BY TM.TestID,R.EmpID ORDER BY R.SubmittedOn) AS AttemptNo,
                        ROW_NUMBER() OVER(PARTITION BY TA.ResultID ORDER BY TA.SequenceNo) AS QuestionNo,
                        QB.Question,TA.SelectedAnswer,QB.Answer AS CorrectAnswer,
                        CASE WHEN ISNULL(TA.IsCorrect,0)=1 THEN 'Correct' ELSE 'Incorrect' END AS IsCorrect
                        FROM TestAttempt TA INNER JOIN TestResult R ON R.ResultID=TA.ResultID
                        INNER JOIN TestMaster TM ON TM.TestID=R.TestID INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID
                        INNER JOIN QuestionBank QB ON QB.QuestionID=TA.QuestionID
                        WHERE SM.TrainingID=@TrainingID AND R.EmpID=@EmpID
                        ORDER BY TRY_CONVERT(date,R.SubmittedOn),TM.TestType,R.SubmittedOn,TA.SequenceNo";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) };
            }
            else
            {
                sql = @"SELECT TM.TestType,TM.TestTitle,R.EmpID,
                        DENSE_RANK() OVER(PARTITION BY TM.TestID,R.EmpID ORDER BY R.SubmittedOn) AS AttemptNo,
                        ROW_NUMBER() OVER(PARTITION BY TA.ResultID ORDER BY TA.SequenceNo) AS QuestionNo,
                        QB.Question,TA.SelectedAnswer,QB.Answer AS CorrectAnswer,
                        CASE WHEN ISNULL(TA.IsCorrect,0)=1 THEN 'Correct' ELSE 'Incorrect' END AS IsCorrect
                        FROM TestAttempt TA INNER JOIN TestResult R ON R.ResultID=TA.ResultID
                        INNER JOIN TestMaster TM ON TM.TestID=R.TestID INNER JOIN SessionMaster SM ON SM.SessionID=TM.SessionID
                        INNER JOIN QuestionBank QB ON QB.QuestionID=TA.QuestionID
                        WHERE SM.TrainingID=@TrainingID AND SM.TrainerID=@TrainerID AND TM.TrainerID=@TrainerID
                        ORDER BY TRY_CONVERT(date,R.SubmittedOn),TM.TestType,R.SubmittedOn,TA.SequenceNo";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@TrainerID", TrainerID) };
            }
            gvQuestions.DataSource = obj.GetDataTable(sql, parameters);
            gvQuestions.DataBind();
        }

        private void BindFeedback()
        {
            string sql;
            SqlParameter[] parameters;
            if (string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase))
            {
                sql = @"SELECT CONVERT(varchar(16),F.SubmittedOn,105) AS SubmittedOn,ISNULL(FCM.CategoryName,'') AS CategoryName,
                        FQ.QuestionText,FD.AnswerType,FD.Rating,FD.Answer,ISNULL(SM.SessionName,'') AS SessionName,
                        ISNULL(FD.TrainerID,'') AS TrainerName,F.EmpID
                        FROM Feedback F INNER JOIN FeedbackDetail FD ON FD.FeedbackID=F.FeedbackID
                        LEFT JOIN FeedbackCategoryMaster FCM ON FCM.CategoryID=FD.CategoryID LEFT JOIN FeedbackQuestionMaster FQ ON FQ.QuestionID=FD.QuestionID
                        LEFT JOIN SessionMaster SM ON SM.SessionID=FD.SessionID
                        WHERE F.TrainingID=@TrainingID AND F.EmpID=@EmpID AND F.Submitted=1
                        ORDER BY F.SubmittedOn,FD.CategoryID,FD.QuestionID";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) };
            }
            else
            {
                sql = @"SELECT CONVERT(varchar(16),F.SubmittedOn,105) AS SubmittedOn,ISNULL(FCM.CategoryName,'') AS CategoryName,
                        FQ.QuestionText,FD.AnswerType,FD.Rating,FD.Answer,ISNULL(SM.SessionName,'') AS SessionName,F.EmpID
                        FROM Feedback F INNER JOIN FeedbackDetail FD ON FD.FeedbackID=F.FeedbackID
                        LEFT JOIN FeedbackCategoryMaster FCM ON FCM.CategoryID=FD.CategoryID LEFT JOIN FeedbackQuestionMaster FQ ON FQ.QuestionID=FD.QuestionID
                        LEFT JOIN SessionMaster SM ON SM.SessionID=FD.SessionID
                        WHERE F.TrainingID=@TrainingID AND F.Submitted=1
                        ORDER BY F.SubmittedOn,F.EmpID,FD.CategoryID,FD.QuestionID";
                parameters = new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) };
            }
            gvFeedback.DataSource = obj.GetDataTable(sql, parameters);
            gvFeedback.DataBind();
        }

        private void SetCertificateButton()
        {
            if (!string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase))
            {
                btnCertificate.Visible = false;
                return;
            }
            object value = obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A'", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@EmpID", EmpID) });
            btnCertificate.Visible = value != null && Convert.ToInt32(value) > 0;
        }

        protected void btnCertificate_Click(object sender, EventArgs e)
        {
            if (!string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase)) return;
            if (!HasTraineeAccess() || !IsClosedTraining()) return;
            Session["TrainingID"] = TrainingID;
            Session["CertificateFromTraining"] = true;
            Response.Redirect("~/Trainee/MyCertificate.aspx");
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            GoDashboard();
        }

        private void GoDashboard()
        {
            if (string.Equals(Role, "Trainer", StringComparison.OrdinalIgnoreCase)) Response.Redirect("~/Trainer/Default.aspx");
            else if (string.Equals(Role, "Emp", StringComparison.OrdinalIgnoreCase)) Response.Redirect("~/Trainee/Default.aspx");
            else Response.Redirect("~/Default.aspx");
        }
    }
}
