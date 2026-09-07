using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
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

        private bool IsFeedbackRequired()
        {
            if (Session["TrainingID"] == null) return false;
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand("SELECT FeedbackRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", con))
            {
                cmd.Parameters.AddWithValue("@TrainingID", Session["TrainingID"].ToString());
                con.Open();
                object value = cmd.ExecuteScalar();
                return value != null && value != DBNull.Value && Convert.ToBoolean(value);
            }
        }

        private bool IsFeedbackAssigned()
        {
            if (Session["TrainingID"] == null) return false;
            clsDataAccess db = new clsDataAccess();
            object value = db.ExecuteScalar(
                "SELECT COUNT(*) FROM TrainingFeedbackCategory WHERE TrainingID=@TrainingID",
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            return Convert.ToInt32(value) > 0;
        }

        private void LoadWorkflow()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT HostelRequiredTrainee,
                           TrainerHostelRequired,
                           TraineeHostelRequired,
                           TrainingStatus,
                           WorkflowStatus,
                           FeedbackRequired,
                           CertificateRequired
                    FROM TrainingDetails
                    WHERE TrainingID=@TrainingID", con);

                cmd.Parameters.AddWithValue("@TrainingID", Session["TrainingID"]);
                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();
                if (!dr.Read()) return;

                lblStatus.Text = dr["TrainingStatus"].ToString();
                string workflow = dr["WorkflowStatus"].ToString();

                bool feedbackRequired = Convert.ToBoolean(dr["FeedbackRequired"]);
                bool certificateRequired = Convert.ToBoolean(dr["CertificateRequired"]);
                bool feedbackAssigned = IsFeedbackAssigned();

                bool trainerHostelRequired = dr["TrainerHostelRequired"] != DBNull.Value && Convert.ToBoolean(dr["TrainerHostelRequired"]);
                bool traineeHostelRequired = dr["TraineeHostelRequired"] != DBNull.Value && Convert.ToBoolean(dr["TraineeHostelRequired"]);

                // Keep compatibility with the existing HostelRequiredTrainee field.
                bool legacyTraineeHostelRequired = string.Equals(
                    dr["HostelRequiredTrainee"].ToString(), "Yes", StringComparison.OrdinalIgnoreCase);

                bool hostelRequired = trainerHostelRequired || traineeHostelRequired || legacyTraineeHostelRequired;

                btnUpdateTraining.Visible = true;
                btnAssignSession.Visible = true;
                btnAssignTrainee.Visible = true;
                btnStartTraining.Visible = false;
                btnAttendance.Visible = false;

                // These actions are shown only when the corresponding requirement is enabled.
                btnAssignHostel.Visible = hostelRequired;
                btnCertificateTemplate.Visible = certificateRequired;
                btnCertificateTemplate.Enabled = false;

                bool traineeAssigned = IsTraineeAssigned();
                bool certificateConfigured = !certificateRequired || IsCertificateTemplateConfigured();

                if (certificateRequired && traineeAssigned && !workflow.Contains("E"))
                {
                    btnCertificateTemplate.Enabled = true;
                    btnCertificateTemplate.Text = certificateConfigured
                        ? "Certificate Template ✓"
                        : "Certificate Template";
                }

                // Start Training stays visible once the normal structural prerequisites
                // (sessions/trainers and trainees) are complete. Additional requirement-
                // dependent checks are performed when the button is clicked.
                bool canStart = workflow.Contains("A") &&
                                workflow.Contains("B") &&
                                workflow.Contains("C") &&
                                workflow.Contains("D") &&
                                !workflow.Contains("E");

                btnStartTraining.Visible = canStart;

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

                dr.Close();
            }
        }

        protected void btnCertificateTemplate_Click(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null) { Response.Redirect("TrainingList.aspx"); return; }
            if (!IsCertificateRequired())
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Certificate is not required for this training.";
                return;
            }
            if (!IsTraineeAssigned())
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Please assign trainee before configuring certificate template.";
                return;
            }
            Response.Redirect("CertificateTemplate.aspx");
        }

        private bool IsTraineeAssigned()
        {
            string query = "SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID";
            SqlParameter[] param = { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) };
            clsDataAccess objDB = new clsDataAccess();
            return Convert.ToInt32(objDB.ExecuteScalar(query, param)) > 0;
        }

        private bool IsCertificateRequired()
        {
            if (Session["TrainingID"] == null) return false;
            string query = "SELECT CertificateRequired FROM TrainingDetails WHERE TrainingID=@TrainingID";
            SqlParameter[] param = { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) };
            object value = new clsDataAccess().ExecuteScalar(query, param);
            return value != null && value != DBNull.Value && Convert.ToBoolean(value);
        }

        private bool IsCertificateTemplateConfigured()
        {
            string query = "SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>'' AND ISNULL(CourseTitle,'')<>''";
            SqlParameter[] param = { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()) };
            clsDataAccess objDB = new clsDataAccess();
            return Convert.ToInt32(objDB.ExecuteScalar(query, param)) > 0;
        }

        private void ShowStartValidation(List<string> missingSteps)
        {
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Text = "Cannot start training. Please complete: " +
                              "<br/>" + string.Join("<br/>", missingSteps.ToArray());
        }

        protected void btnStartTraining_Click(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null) { Response.Redirect("TrainingList.aspx"); return; }

            List<string> missingSteps = new List<string>();

            // Structural prerequisites are always mandatory.
            if (!HasSessionsAndTrainers())
                missingSteps.Add("1. Assign Sessions & Trainers");

            if (!IsTraineeAssigned())
                missingSteps.Add("2. Assign Trainees");

            // Optional activities are checked only when the admin marked them required.
            if (IsFeedbackRequired() && !IsFeedbackAssigned())
                missingSteps.Add("3. Assign Feedback Questionnaire");

            if (IsCertificateRequired() && !IsCertificateTemplateConfigured())
                missingSteps.Add("4. Configure Certificate Template");

            if (missingSteps.Count > 0)
            {
                ShowStartValidation(missingSteps);
                return;
            }

            // Hostel button is displayed when required. Hostel assignment itself is
            // handled from Assign Hostel and is not assumed here because the existing
            // database does not expose a single completion flag for both hostel types.
            StartTraining();
        }

        private bool HasSessionsAndTrainers()
        {
            if (Session["TrainingID"] == null) return false;

            string query = @"
                SELECT COUNT(*)
                FROM SessionMaster
                WHERE TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString())
            };

            object value = new clsDataAccess().ExecuteScalar(query, param);
            return Convert.ToInt32(value) > 0;
        }

        protected void btnHostelNo_Click(object sender, EventArgs e)
        {
            UpdateHostelRequirement("No");
            StartTraining();
        }

        protected void btnHostelYes_Click(object sender, EventArgs e)
        {
            UpdateHostelRequirement("Yes");
            pnlHostelConfirmation.Visible = false;
            Response.Redirect("AssignHostel.aspx");
        }

        private void UpdateHostelRequirement(string hostelRequired)
        {
            string query = "UPDATE TrainingDetails SET HostelRequiredTrainee=@HostelRequiredTrainee,UpdatedOn=GETDATE(),UpdatedBy=@UpdatedBy WHERE TrainingID=@TrainingID";
            SqlParameter[] param =
            {
                new SqlParameter("@HostelRequiredTrainee", hostelRequired),
                new SqlParameter("@UpdatedBy", Session["AdminID"] == null ? "Admin" : Session["AdminID"].ToString()),
                new SqlParameter("@TrainingID", Session["TrainingID"].ToString())
            };
            new clsDataAccess().ExecuteSql(query, param);
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