using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;

namespace Training.Admin
{
    public partial class ManageTraining : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        private SqlParameter[] P(string name, object value) { return new SqlParameter[] { new SqlParameter(name, value) }; }
        private string TrainingID { get { return Session["TrainingID"] == null ? "" : Session["TrainingID"].ToString(); } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainingID"] == null) { Response.Redirect("TrainingList.aspx"); return; }
                TrainingSummary1.LoadTraining(TrainingID);
                LoadWorkflow();
            }
        }

        private bool GetRequirement(string column)
        {
            object value = new clsDataAccess().ExecuteScalar("SELECT " + column + " FROM TrainingDetails WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID));
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private bool IsFeedbackRequired() { return GetRequirement("FeedbackRequired") && !GetRequirement("FeedbackSkipped"); }
        private bool IsCertificateRequired() { return GetRequirement("CertificateRequired") && !GetRequirement("CertificateSkipped"); }
        private bool IsAttendanceRequired() { return GetRequirement("AttendanceRequired"); }
        private bool IsPreTestRequired() { return GetRequirement("InitialAssessmentRequired"); }
        private bool IsPostTestRequired() { return GetRequirement("FinalAssessmentRequired"); }

        private bool IsFeedbackAssigned() { return Convert.ToInt32(new clsDataAccess().ExecuteScalar("SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID))) > 0; }
        private bool IsTraineeAssigned() { return Convert.ToInt32(new clsDataAccess().ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID))) > 0; }
        private bool IsCertificateTemplateConfigured() { return Convert.ToInt32(new clsDataAccess().ExecuteScalar("SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>'' AND ISNULL(CourseTitle,'')<>''", P("@TrainingID", TrainingID))) > 0; }
        private bool HasSessionsAndTrainers() { object value = new clsDataAccess().ExecuteScalar("SELECT CASE WHEN COUNT(*) > 0 AND COUNT(*) = SUM(CASE WHEN ISNULL(TrainerID,'')<>'' THEN 1 ELSE 0 END) THEN 1 ELSE 0 END FROM SessionMaster WHERE TrainingID=@TrainingID", P("@TrainingID", TrainingID)); return Convert.ToInt32(value) == 1; }

        private bool AreAllAttendanceCompleted()
        {
            string q = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND EXISTS (SELECT 1 FROM SessionMaster S WHERE S.TrainingID=@TrainingID)
AND NOT EXISTS (
 SELECT 1 FROM TrainingAssignment A CROSS JOIN SessionMaster S
 WHERE A.TrainingID=@TrainingID AND S.TrainingID=@TrainingID AND ISNULL(S.AttendanceSkipped,0)=0
 AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=S.SessionID AND SA.EmpID=A.EmpID AND SA.AttendanceStatus='Completed')
) THEN 1 ELSE 0 END";
            return Convert.ToInt32(new clsDataAccess().ExecuteScalar(q, P("@TrainingID", TrainingID))) == 1;
        }

        private bool AreAllTestsCompleted(string testType)
        {
            string skipColumn = testType == "Pre" ? "PreAssessmentSkipped" : "PostAssessmentSkipped";
            string q = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND EXISTS (SELECT 1 FROM SessionMaster S WHERE S.TrainingID=@TrainingID)
AND NOT EXISTS (
 SELECT 1 FROM TrainingAssignment A CROSS JOIN SessionMaster S
 INNER JOIN TrainingDetails TD ON TD.TrainingID=A.TrainingID
 WHERE A.TrainingID=@TrainingID AND S.TrainingID=@TrainingID AND ISNULL(S." + skipColumn + @",0)=0
 AND EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=S.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1)
 AND (ISNULL(TD.AttendanceRequired,0)=0 OR ISNULL(S.AttendanceSkipped,0)=1 OR EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=S.SessionID AND SA.EmpID=A.EmpID AND SA.AttendanceStatus='Present'))
 AND NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=S.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 AND TA.EmpID=A.EmpID AND TA.Submitted=1)
) THEN 1 ELSE 0 END";
            return Convert.ToInt32(new clsDataAccess().ExecuteScalar(q, new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID), new SqlParameter("@TestType", testType) })) == 1;
        }

        private bool IsFeedbackSubmitted()
        {
            string q = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND NOT EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND NOT EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=@TrainingID AND F.EmpID=A.EmpID AND F.Submitted=1)) THEN 1 ELSE 0 END";
            return Convert.ToInt32(new clsDataAccess().ExecuteScalar(q, P("@TrainingID", TrainingID))) == 1;
        }

        private bool IsCertificateGenerated()
        {
            string q = @"SELECT CASE WHEN EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID)
AND NOT EXISTS (SELECT 1 FROM TrainingAssignment A WHERE A.TrainingID=@TrainingID AND NOT EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=@TrainingID AND C.EmpID=A.EmpID AND C.CertificateStatus='A')) THEN 1 ELSE 0 END";
            return Convert.ToInt32(new clsDataAccess().ExecuteScalar(q, P("@TrainingID", TrainingID))) == 1;
        }

        private string Stage(string label, bool required, bool complete, bool skipped, ref int number)
        {
            string css;
            string state;
            string bubble;
            if (skipped)
            {
                css = "skipped";
                state = "Skipped";
                bubble = "–";
            }
            else if (!required)
            {
                css = "na";
                state = "Not Required";
                bubble = "–";
            }
            else if (complete)
            {
                css = "done";
                state = "✓ Completed";
                bubble = "✓";
                number++;
            }
            else
            {
                css = "pending";
                state = "Pending";
                bubble = number.ToString();
                number++;
            }
            return "<div class='stage-item " + css + "'><div class='stage-bubble'>" + bubble + "</div><div class='stage-label'>" + label + "</div><div class='stage-state'>" + state + "</div></div>";
        }

        private void BuildLifecycle()
        {
            bool feedbackRaw = GetRequirement("FeedbackRequired");
            bool certificateRaw = GetRequirement("CertificateRequired");
            bool feedbackSkipped = GetRequirement("FeedbackSkipped");
            bool certificateSkipped = GetRequirement("CertificateSkipped");
            bool ar = IsAttendanceRequired(), pr = IsPreTestRequired(), por = IsPostTestRequired();
            bool ta = IsTraineeAssigned(), sa = HasSessionsAndTrainers();
            bool fa = !feedbackRaw || feedbackSkipped || IsFeedbackAssigned();
            bool ct = !certificateRaw || certificateSkipped || IsCertificateTemplateConfigured();
            bool ac = !ar || AreAllAttendanceCompleted();
            bool pc = !pr || AreAllTestsCompleted("Pre");
            bool poc = !por || AreAllTestsCompleted("Post");
            bool fs = !feedbackRaw || feedbackSkipped || IsFeedbackSubmitted();
            bool cg = !certificateRaw || certificateSkipped || IsCertificateGenerated();

            int n = 1;
            StringBuilder h = new StringBuilder();
            h.Append(Stage("Create Batch", true, true, false, ref n));
            h.Append(Stage("Assign Sessions & Trainers", true, sa, false, ref n));
            h.Append(Stage("Assign Trainees", true, ta, false, ref n));
            h.Append(Stage("Feedback Assigned", feedbackRaw, fa, feedbackSkipped, ref n));
            h.Append(Stage("Certificate Template", certificateRaw, ct, certificateSkipped, ref n));
            h.Append(Stage("Attendance Completed", ar, ac, false, ref n));
            h.Append(Stage("Pre-Test Completed", pr, pc, false, ref n));
            h.Append(Stage("Post-Test Completed", por, poc, false, ref n));
            h.Append(Stage("Feedback Submitted", feedbackRaw, fs, feedbackSkipped, ref n));
            h.Append(Stage("Certificate Generated", certificateRaw, cg, certificateSkipped, ref n));
            litBatchLifecycle.Text = "<div class='stage-line'>" + h.ToString() + "</div>";
        }

        private void LoadWorkflow()
        {
            string workflow = "";
            bool hostelRequired = false;
            bool certificateRequired = false;
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"SELECT HostelRequiredTrainee,TrainerHostelRequired,TraineeHostelRequired,TrainingStatus,WorkflowStatus,CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", con);
                cmd.Parameters.AddWithValue("@TrainingID", TrainingID);
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.Read()) return;
                lblStatus.Text = dr["TrainingStatus"].ToString();
                workflow = dr["WorkflowStatus"].ToString();
                certificateRequired = Convert.ToBoolean(dr["CertificateRequired"]);
                bool th = dr["TrainerHostelRequired"] != DBNull.Value && Convert.ToBoolean(dr["TrainerHostelRequired"]);
                bool trh = dr["TraineeHostelRequired"] != DBNull.Value && Convert.ToBoolean(dr["TraineeHostelRequired"]);
                bool legacy = string.Equals(dr["HostelRequiredTrainee"].ToString(), "Yes", StringComparison.OrdinalIgnoreCase);
                hostelRequired = th || trh || legacy;
                dr.Close();
            }

            bool ta = IsTraineeAssigned();
            bool sa = HasSessionsAndTrainers();
            bool fr = IsFeedbackRequired();
            bool feedbackSkipped = GetRequirement("FeedbackSkipped");
            bool certificateSkipped = GetRequirement("CertificateSkipped");
            bool fa = !fr || IsFeedbackAssigned();
            bool ct = !IsCertificateRequired() || IsCertificateTemplateConfigured();
            bool ac = !IsAttendanceRequired() || AreAllAttendanceCompleted();

            btnUpdateTraining.Visible = true;
            btnAssignSession.Visible = true;
            btnAssignTrainee.Visible = true;
            btnRequirements.Visible = true;
            btnAssignFeedback.Visible = fr;
            btnAssignFeedback.Enabled = fr;
            btnAssignFeedback.Text = fa ? "Feedback Template ✓" : "Feedback Template";
            btnStartTraining.Visible = true;
            btnStartTraining.Enabled = !workflow.Contains("E");
            btnAttendance.Visible = false;
            btnAssignHostel.Visible = hostelRequired;
            btnCertificateTemplate.Visible = certificateRequired && !certificateSkipped;
            btnCertificateTemplate.Enabled = certificateRequired && !ta && false;
            btnCertificateTemplate.Enabled = certificateRequired && ta && !workflow.Contains("E") && !certificateSkipped;
            btnAssignSession.Text = sa ? "Assign Sessions & Trainers ✓" : "Assign Sessions & Trainers";
            btnAssignTrainee.Text = ta ? "Assign Trainee ✓" : "Assign Trainee";

            if (certificateRequired && ta && !workflow.Contains("E") && !certificateSkipped)
                btnCertificateTemplate.Text = ct ? "Certificate Template ✓" : "Certificate Template";

            if (workflow.Contains("E"))
            {
                btnUpdateTraining.Visible = false;
                btnAssignSession.Visible = false;
                btnAssignTrainee.Visible = false;
                btnStartTraining.Visible = true;
                btnStartTraining.Enabled = false;
                btnAssignHostel.Visible = false;
                btnCertificateTemplate.Visible = false;
                btnCertificateTemplate.Enabled = false;
                btnAttendance.Visible = true;
                btnAttendance.Text = ac ? "Attendance ✓" : "Attendance";
            }
            else if (hostelRequired) btnAssignHostel.Text = "Assign Hostel";

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

        protected void btnRequirements_Click(object sender, EventArgs e) { Response.Redirect("TrainingRequirements.aspx"); }

        protected void btnAssignFeedback_Click(object sender, EventArgs e)
        {
            if (!IsFeedbackRequired()) { lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Feedback is not required for this training."; return; }
            Response.Redirect("AssignFeedback.aspx");
        }

        protected void btnCertificateTemplate_Click(object sender, EventArgs e)
        {
            if (!IsCertificateRequired()) { lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Certificate is not required for this training."; return; }
            if (!IsTraineeAssigned()) { lblMessage.ForeColor = System.Drawing.Color.Red; lblMessage.Text = "Please assign trainee before configuring certificate template."; return; }
            Response.Redirect("CertificateTemplate.aspx");
        }

        protected void btnHostelNo_Click(object sender, EventArgs e) { UpdateHostelRequirement("No"); StartTraining(); }

        protected void btnHostelYes_Click(object sender, EventArgs e)
        {
            UpdateHostelRequirement("Yes");
            pnlHostelConfirmation.Visible = false;
            Response.Redirect("AssignHostel.aspx");
        }

        private void UpdateHostelRequirement(string hostelRequired)
        {
            new clsDataAccess().ExecuteSql("UPDATE TrainingDetails SET HostelRequiredTrainee=@HostelRequiredTrainee,UpdatedOn=GETDATE(),UpdatedBy=@UpdatedBy WHERE TrainingID=@TrainingID", new SqlParameter[]
            {
                new SqlParameter("@HostelRequiredTrainee", hostelRequired),
                new SqlParameter("@UpdatedBy", Session["UserID"] == null ? "Admin" : Session["UserID"].ToString()),
                new SqlParameter("@TrainingID", TrainingID)
            });
        }

        private void StartTraining()
        {
            if (IsFeedbackRequired() && !IsFeedbackAssigned())
            {
                pnlHostelConfirmation.Visible = false;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Feedback is required. Please assign Feedback before starting training.";
                return;
            }
            if (IsCertificateRequired() && !IsCertificateTemplateConfigured())
            {
                pnlHostelConfirmation.Visible = false;
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Certificate is required. Please configure Certificate Template before starting training.";
                return;
            }
            clsWorkflow.UpdateWorkflow(TrainingID, "InProgress", "E");
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