using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class ManageTraining : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainingID"] == null) { Response.Redirect("TrainingList.aspx"); return; }
                TrainingSummary1.LoadTraining(Session["TrainingID"].ToString());
                LoadWorkflow();
            }
        }

        private bool GetRequirement(string column)
        {
            string query = "SELECT " + column + " FROM TrainingDetails WHERE TrainingID=@TrainingID";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private bool IsFeedbackRequired() { return GetRequirement("FeedbackRequired"); }
        private bool IsCertificateRequired() { return GetRequirement("CertificateRequired"); }
        private bool IsAttendanceRequired() { return GetRequirement("AttendanceRequired"); }
        private bool IsPreTestRequired() { return GetRequirement("InitialAssessmentRequired"); }
        private bool IsPostTestRequired() { return GetRequirement("FinalAssessmentRequired"); }

        private bool IsFeedbackAssigned()
        {
            string query = "SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) > 0;
        }

        private bool IsTraineeAssigned()
        {
            string query = "SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) > 0;
        }

        private bool IsCertificateTemplateConfigured()
        {
            string query = "SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>'' AND ISNULL(CourseTitle,'')<>''";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) > 0;
        }

        private bool HasSessionsAndTrainers()
        {
            string query = @"SELECT CASE WHEN COUNT(*) > 0 AND COUNT(*) = SUM(CASE WHEN ISNULL(TrainerID,'')<>'' THEN 1 ELSE 0 END) THEN 1 ELSE 0 END FROM SessionMaster WHERE TrainingID=@TrainingID";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) == 1;
        }

        private bool AreAllAttendanceCompleted()
        {
            string query = @"
SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND EXISTS (SELECT 1 FROM SessionMaster S WHERE S.TrainingID=@TrainingID)
AND NOT EXISTS (
    SELECT 1 FROM TrainingAssignment A
    CROSS JOIN SessionMaster S
    WHERE A.TrainingID=@TrainingID AND S.TrainingID=@TrainingID
      AND NOT EXISTS (
          SELECT 1 FROM SessionAttendance SA
          WHERE SA.SessionID=S.SessionID AND SA.EmpID=A.EmpID AND SA.AttendanceStatus='Completed'
      )
) THEN 1 ELSE 0 END";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) == 1;
        }

        private bool AreAllTestsCompleted(string testType)
        {
            string query = @"
SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND EXISTS (SELECT 1 FROM SessionMaster S WHERE S.TrainingID=@TrainingID)
AND NOT EXISTS (
    SELECT 1 FROM TrainingAssignment A
    CROSS JOIN SessionMaster S
    WHERE A.TrainingID=@TrainingID AND S.TrainingID=@TrainingID
      AND NOT EXISTS (
          SELECT 1
          FROM TestMaster TM
          INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID
          WHERE TM.SessionID=S.SessionID
            AND TM.TestType=@TestType
            AND TM.IsPublished=1
            AND TA.EmpID=A.EmpID
            AND TA.Submitted=1
      )
) THEN 1 ELSE 0 END";
            SqlParameter[] p = {
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString()),
                new SqlParameter("@TestType", testType)
            };
            object value = new clsDataAccess().ExecuteScalar(query, p);
            return Convert.ToInt32(value) == 1;
        }

        private bool IsFeedbackSubmitted()
        {
            string query = @"
SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND NOT EXISTS (
    SELECT 1 FROM TrainingAssignment A
    WHERE A.TrainingID=@TrainingID
      AND NOT EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=@TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)
) THEN 1 ELSE 0 END";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) == 1;
        }

        private bool IsCertificateGenerated()
        {
            string query = @"
SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND NOT EXISTS (
    SELECT 1 FROM TrainingAssignment A
    WHERE A.TrainingID=@TrainingID
      AND NOT EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=@TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')
) THEN 1 ELSE 0 END";
            object value = new clsDataAccess().ExecuteScalar(query, new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) == 1;
        }

        private string Stage(string label, bool required, bool complete, ref int number)
        {
            string css = !required ? "na" : (complete ? "done" : "pending");
            string state = !required ? "Not Required" : (complete ? "✓ Completed" : "Pending");
            string bubble = !required ? "–" : (complete ? "✓" : number.ToString());
            if (required) number++;
            return "<div class='stage-item " + css + "'><div class='stage-bubble'>" + bubble + "</div><div class='stage-label'>" + label + "</div><div class='stage-state'>" + state + "</div></div>";
        }

        private void BuildLifecycle()
        {
            bool feedbackRequired = IsFeedbackRequired();
            bool certificateRequired = IsCertificateRequired();
            bool attendanceRequired = IsAttendanceRequired();
            bool preRequired = IsPreTestRequired();
            bool postRequired = IsPostTestRequired();
            bool traineeAssigned = IsTraineeAssigned();
            bool sessionsAssigned = HasSessionsAndTrainers();
            bool feedbackAssigned = !feedbackRequired || IsFeedbackAssigned();
            bool certificateTemplate = !certificateRequired || IsCertificateTemplateConfigured();
            bool attendanceComplete = !attendanceRequired || AreAllAttendanceCompleted();
            bool preComplete = !preRequired || AreAllTestsCompleted("Pre");
            bool postComplete = !postRequired || AreAllTestsCompleted("Post");
            bool feedbackSubmitted = !feedbackRequired || IsFeedbackSubmitted();
            bool certificateGenerated = !certificateRequired || IsCertificateGenerated();

            int number = 1;
            StringBuilder html = new StringBuilder();
            html.Append(Stage("Create Batch", true, true, ref number));
            html.Append(Stage("Assign Sessions & Trainers", true, sessionsAssigned, ref number));
            html.Append(Stage("Assign Trainees", true, traineeAssigned, ref number));
            html.Append(Stage("Feedback Assigned", feedbackRequired, feedbackAssigned, ref number));
            html.Append(Stage("Certificate Template", certificateRequired, certificateTemplate, ref number));
            html.Append(Stage("Attendance Completed", attendanceRequired, attendanceComplete, ref number));
            html.Append(Stage("Pre-Test Completed", preRequired, preComplete, ref number));
            html.Append(Stage("Post-Test Completed", postRequired, postComplete, ref number));
            html.Append(Stage("Feedback Submitted", feedbackRequired, feedbackSubmitted, ref number));
            html.Append(Stage("Certificate Generated", certificateRequired, certificateGenerated, ref number));
            litBatchLifecycle.Text = "<div class='stage-line'>" + html.ToString() + "</div>";
        }

        private void LoadWorkflow()
        {
            string workflow = "";
            bool hostelRequired = false;
            bool certificateRequired = false;
            bool traineeAssigned = false;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT HostelRequiredTrainee,TrainerHostelRequired,TraineeHostelRequired,TrainingStatus,WorkflowStatus,CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", con);
                cmd.Parameters.AddWithValue("@TrainingID", Session["TrainingID"]);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.Read()) return;
                lblStatus.Text = dr["TrainingStatus"].ToString();
                workflow = dr["WorkflowStatus"].ToString();
                certificateRequired = Convert.ToBoolean(dr["CertificateRequired"]);
                bool trainerHostelRequired = dr["TrainerHostelRequired"] != DBNull.Value && Convert.ToBoolean(dr["TrainerHostelRequired"]);
                bool traineeHostelRequired = dr["TraineeHostelRequired"] != DBNull.Value && Convert.ToBoolean(dr["TraineeHostelRequired"]);
                bool legacy = string.Equals(dr["HostelRequiredTrainee"].ToString(), "Yes", StringComparison.OrdinalIgnoreCase);
                hostelRequired = trainerHostelRequired || traineeHostelRequired || legacy;
                dr.Close();
            }

            traineeAssigned = IsTraineeAssigned();
            btnUpdateTraining.Visible = true;
            btnAssignSession.Visible = true;
            btnAssignTrainee.Visible = true;
            btnStartTraining.Visible = workflow.Contains("A") && workflow.Contains("B") && workflow.Contains("C") && workflow.Contains("D") && !workflow.Contains("E");
            btnAttendance.Visible = false;
            btnAssignHostel.Visible = hostelRequired;
            btnCertificateTemplate.Visible = certificateRequired;
            btnCertificateTemplate.Enabled = certificateRequired && traineeAssigned && !workflow.Contains("E");
            if (certificateRequired && traineeAssigned && !workflow.Contains("E"))
                btnCertificateTemplate.Text = IsCertificateTemplateConfigured() ? "Certificate Template ✓" : "Certificate Template";

            if (workflow.Contains("E"))
            {
                btnUpdateTraining.Visible = false;
                btnAssignSession.Visible = false;
                btnAssignTrainee.Visible = false;
                btnStartTraining.Visible = false;
                btnAssignHostel.Visible = false;
                btnCertificateTemplate.Visible = false;
                btnCertificateTemplate.Enabled = false;
                btnAttendance.Visible = true;
            }

            BuildLifecycle();
        }

        private void ShowStartValidation(List<string> missingSteps)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "Cannot start training. Please complete:<br/>" + string.Join("<br/>", missingSteps.ToArray());
        }

        protected void btnStartTraining_Click(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null) { Response.Redirect("TrainingList.aspx"); return; }
            List<string> missing = new List<string>();
            if (!HasSessionsAndTrainers()) missing.Add("1. Assign Sessions & Trainers");
            if (!IsTraineeAssigned()) missing.Add("2. Assign Trainees");
            if (IsFeedbackRequired() && !IsFeedbackAssigned()) missing.Add("3. Assign Feedback Questionnaire");
            if (IsCertificateRequired() && !IsCertificateTemplateConfigured()) missing.Add("4. Configure Certificate Template");
            if (missing.Count > 0) { ShowStartValidation(missing); return; }
            StartTraining();
        }

        protected void btnCertificateTemplate_Click(object sender, EventArgs e)
        {
            if (!IsCertificateRequired()) { lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Certificate is not required for this training."; return; }
            if (!IsTraineeAssigned()) { lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Please assign trainee before configuring certificate template."; return; }
            Response.Redirect("CertificateTemplate.aspx");
        }

        protected void btnHostelNo_Click(object sender, EventArgs e) { UpdateHostelRequirement("No"); StartTraining(); }
        protected void btnHostelYes_Click(object sender, EventArgs e) { UpdateHostelRequirement("Yes"); pnlHostelConfirmation.Visible = false; Response.Redirect("AssignHostel.aspx"); }

        private void UpdateHostelRequirement(string hostelRequired)
        {
            string query = "UPDATE TrainingDetails SET HostelRequiredTrainee=@HostelRequiredTrainee,UpdatedOn=GETDATE(),UpdatedBy=@UpdatedBy WHERE TrainingID=@TrainingID";
            SqlParameter[] param = { new SqlParameter("@HostelRequiredTrainee", hostelRequired), new SqlParameter("@UpdatedBy", Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString()), new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) };
            new clsDataAccess().ExecuteSql(query, param);
        }

        private void StartTraining()
        {
            if (IsFeedbackRequired() && !IsFeedbackAssigned()) { pnlHostelConfirmation.Visible = false; lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Feedback is required. Please assign Feedback before starting training."; return; }
            if (IsCertificateRequired() && !IsCertificateTemplateConfigured()) { pnlHostelConfirmation.Visible = false; lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Certificate is required. Please configure Certificate Template before starting training."; return; }
            clsWorkflow.UpdateWorkflow(Session["TrainingID"].ToString(), "InProgress", "E");
            pnlHostelConfirmation.Visible = false;
            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Training has started successfully.";
            LoadWorkflow();
        }

        protected void btnAttendance_Click(object sender, EventArgs e) { Response.Redirect("TrainingAttendance.aspx"); }
        protected void btnUpdateTraining_Click(object sender, EventArgs e) { Response.Redirect("CreateBatch.aspx?mode=edit"); }
        protected void btnAssignSession_Click(object sender, EventArgs e) { Response.Redirect("AssignSession.aspx"); }
        protected void btnAssignHostel_Click(object sender, EventArgs e) { Response.Redirect("AssignHostel.aspx"); }
        protected void btnAssignTrainee_Click(object sender, EventArgs e) { Response.Redirect("AssignTrainee.aspx"); }
    }
}