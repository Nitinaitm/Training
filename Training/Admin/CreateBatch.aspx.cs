using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CreateBatch : System.Web.UI.Page
    {
        protected CheckBox chkAttendanceRequired;
        protected CheckBox chkPreTrainingAssessment;
        protected CheckBox chkPostTrainingAssessment;
        protected CheckBox chkFeedbackRequired;
        protected CheckBox chkCertificateRequired;
        protected CheckBox chkTrainerHostelRequired;
        protected CheckBox chkTraineeHostelRequired;
        protected Button btnAssignFeedback;

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.UnobtrusiveValidationMode = System.Web.UI.UnobtrusiveValidationMode.None;
            if (!IsPostBack)
            {
                if (Request.QueryString["mode"] != "edit") Session.Remove("TrainingID");
                BindTrainingType(); BindTrainingCategory(); BindOrganizer(); BindLocation();
                ddlTrainingType.Items.Insert(0, new ListItem("Select Training Type", ""));
                ddlTrainingCategory.Items.Insert(0, new ListItem("Select Training Category", ""));
                ddlTrainingOrganizer.Items.Insert(0, new ListItem("Select Organizer", ""));
                ddlTrainingLocation.Items.Insert(0, new ListItem("Select Location", ""));
                BindCourse(); BindStartTime();
                if (Request.QueryString["mode"] == "edit" && Session["TrainingID"] != null)
                    LoadTrainingForEdit(Session["TrainingID"].ToString());
                else SetButtonStatus();
                LoadPlugins();
            }
        }

        private void LoadTrainingForEdit(string trainingID)
        {
            clsDataAccess obj = new clsDataAccess();
            DataTable dt = obj.GetDataTable("SELECT * FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            if (dt.Rows.Count == 0) return;
            DataRow r = dt.Rows[0];
            txtTrainingID.Text = Convert.ToString(r["TrainingID"]);
            txtDateFrom.Text = Convert.ToString(r["DateFrom"]);
            txtDateTo.Text = Convert.ToString(r["DateTo"]);
            if (ddlTrainingType.Items.FindByText(Convert.ToString(r["TrainingType"])) != null) ddlTrainingType.SelectedValue = Convert.ToString(r["TrainingType"]);
            if (ddlTrainingOrganizer.Items.FindByText(Convert.ToString(r["TrainingOrganizer"])) != null) ddlTrainingOrganizer.SelectedValue = Convert.ToString(r["TrainingOrganizer"]);
            if (ddlTrainingLocation.Items.FindByText(Convert.ToString(r["TrainingLocation"])) != null) ddlTrainingLocation.SelectedValue = Convert.ToString(r["TrainingLocation"]);
            txtBatch.Text = Convert.ToString(r["Batch"]); txtNoOfDays.Text = Convert.ToString(r["NoOfDays"]); txtStrength.Text = Convert.ToString(r["BatchStrength"]); txtRemarks.Text = Convert.ToString(r["Remarks"]); txtHours.Text = Convert.ToString(r["Hours"]);
            if (ddlCourse.Items.FindByValue(Convert.ToString(r["CourseID"])) != null) ddlCourse.SelectedValue = Convert.ToString(r["CourseID"]);
            if (ddlTrainingCategory.Items.FindByText(Convert.ToString(r["TrainingCategory"])) != null) ddlTrainingCategory.SelectedValue = Convert.ToString(r["TrainingCategory"]);
            if (ddlStartTime.Items.FindByValue(Convert.ToString(r["StartTime"])) != null) ddlStartTime.SelectedValue = Convert.ToString(r["StartTime"]);
            chkAttendanceRequired.Checked = r["AttendanceRequired"] != DBNull.Value && Convert.ToBoolean(r["AttendanceRequired"]);
            chkPreTrainingAssessment.Checked = r["InitialAssessmentRequired"] != DBNull.Value && Convert.ToBoolean(r["InitialAssessmentRequired"]);
            chkPostTrainingAssessment.Checked = r["FinalAssessmentRequired"] != DBNull.Value && Convert.ToBoolean(r["FinalAssessmentRequired"]);
            chkFeedbackRequired.Checked = r["FeedbackRequired"] != DBNull.Value && Convert.ToBoolean(r["FeedbackRequired"]);
            chkCertificateRequired.Checked = r["CertificateRequired"] != DBNull.Value && Convert.ToBoolean(r["CertificateRequired"]);
            chkTrainerHostelRequired.Checked = r["TrainerHostelRequired"] != DBNull.Value && Convert.ToBoolean(r["TrainerHostelRequired"]);
            chkTraineeHostelRequired.Checked = r["TraineeHostelRequired"] != DBNull.Value && Convert.ToBoolean(r["TraineeHostelRequired"]);
            SetButtonStatus();
        }

        private void BindTrainingType() { using (SqlConnection con = new SqlConnection(constr)) using (SqlCommand cmd = new SqlCommand("SELECT TrainingType FROM TrainingMaster ORDER BY TrainingType", con)) { con.Open(); ddlTrainingType.DataSource = cmd.ExecuteReader(); ddlTrainingType.DataTextField = "TrainingType"; ddlTrainingType.DataValueField = "TrainingType"; ddlTrainingType.DataBind(); } }
        private void BindTrainingCategory() { using (SqlConnection con = new SqlConnection(constr)) using (SqlCommand cmd = new SqlCommand("SELECT TrainingCategory FROM TrainingCategoryMaster ORDER BY TrainingCategory", con)) { con.Open(); ddlTrainingCategory.DataSource = cmd.ExecuteReader(); ddlTrainingCategory.DataTextField = "TrainingCategory"; ddlTrainingCategory.DataValueField = "TrainingCategory"; ddlTrainingCategory.DataBind(); } }
        private void BindOrganizer() { using (SqlConnection con = new SqlConnection(constr)) using (SqlCommand cmd = new SqlCommand("SELECT TrainingOrganizer FROM TrainingOrganizerMaster ORDER BY TrainingOrganizer", con)) { con.Open(); ddlTrainingOrganizer.DataSource = cmd.ExecuteReader(); ddlTrainingOrganizer.DataTextField = "TrainingOrganizer"; ddlTrainingOrganizer.DataValueField = "TrainingOrganizer"; ddlTrainingOrganizer.DataBind(); } }
        private void BindLocation() { using (SqlConnection con = new SqlConnection(constr)) using (SqlCommand cmd = new SqlCommand("SELECT TrainingLocation FROM TrainingLocationMaster ORDER BY TrainingLocation", con)) { con.Open(); ddlTrainingLocation.DataSource = cmd.ExecuteReader(); ddlTrainingLocation.DataTextField = "TrainingLocation"; ddlTrainingLocation.DataValueField = "TrainingLocation"; ddlTrainingLocation.DataBind(); } }
        private void BindCourse() { using (SqlConnection con = new SqlConnection(constr)) using (SqlCommand cmd = new SqlCommand("SELECT CourseID,CourseName FROM CourseMaster ORDER BY CourseName", con)) { con.Open(); ddlCourse.DataSource = cmd.ExecuteReader(); ddlCourse.DataTextField = "CourseName"; ddlCourse.DataValueField = "CourseID"; ddlCourse.DataBind(); } }
        private void BindStartTime() { ddlStartTime.Items.Clear(); ddlStartTime.Items.Add(new ListItem("Select Start Time", "")); for (int h = 0; h < 24; h++) for (int m = 0; m < 60; m += 30) { DateTime t = DateTime.Today.AddHours(h).AddMinutes(m); ddlStartTime.Items.Add(new ListItem(t.ToString("hh:mm tt"), t.ToString("HH:mm"))); } }

        private void GenerateTrainingID()
        {
            try
            {
                string trainingType = ddlTrainingType.SelectedItem.Text.Trim().ToUpper();
                trainingType = trainingType.Length >= 2 ? trainingType.Substring(0, 2) : trainingType;
                string organizer = ddlTrainingOrganizer.SelectedItem.Text.Replace(" ", "").ToUpper();
                string location = ddlTrainingLocation.SelectedItem.Text.Replace(" ", "").ToUpper();
                location = location.Length >= 3 ? location.Substring(0, 3) : location;
                string courseID = ddlCourse.SelectedValue.ToString();
                string batch = txtBatch.Text.Trim().Replace(" ", "").ToUpper();
                DateTime fromDate = DateTime.ParseExact(txtDateFrom.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime toDate = DateTime.ParseExact(txtDateTo.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);
                string prefix = "TR-" + courseID + "-" + trainingType + "-" + organizer + "-" + location + "-" + batch + "-" + fromDate.ToString("ddMMyy") + "-" + toDate.ToString("ddMMyy");
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM TrainingDetails WHERE TrainingID LIKE @Prefix+'%'", con))
                    {
                        cmd.Parameters.AddWithValue("@Prefix", prefix);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        txtTrainingID.Text = prefix + "-" + (count + 1).ToString("000");
                    }
                }
            }
            catch { }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fromDate, toDate;
                if (!DateTime.TryParseExact(txtDateFrom.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate) || !DateTime.TryParseExact(txtDateTo.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate)) { lblMessage.Text = "Please enter valid From/To dates in dd-MM-yyyy format."; lblMessage.ForeColor = Color.Red; return; }
                if (toDate < fromDate) { lblMessage.Text = "To Date cannot be before From Date."; lblMessage.ForeColor = Color.Red; return; }
                if (string.IsNullOrWhiteSpace(txtBatch.Text) || ddlTrainingType.SelectedValue == "" || ddlTrainingOrganizer.SelectedValue == "" || ddlTrainingLocation.SelectedValue == "" || ddlTrainingCategory.SelectedValue == "" || ddlCourse.SelectedValue == "") { lblMessage.Text = "Please complete all mandatory batch details."; lblMessage.ForeColor = Color.Red; return; }

                string trainingID = txtTrainingID.Text.Trim();
                string oldTrainingID = Session["TrainingID"] == null ? null : Session["TrainingID"].ToString();

                if (string.IsNullOrEmpty(oldTrainingID) && string.IsNullOrEmpty(trainingID))
                {
                    GenerateTrainingID();
                    trainingID = txtTrainingID.Text.Trim();
                    if (string.IsNullOrEmpty(trainingID)) { lblMessage.Text = "Unable to generate Training ID. Please check Training Type, Organizer, Location, Course, Batch and dates."; lblMessage.ForeColor = Color.Red; return; }
                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    if (!string.IsNullOrEmpty(oldTrainingID))
                    {
                        trainingID = oldTrainingID;
                        txtTrainingID.Text = oldTrainingID;
                        using (SqlCommand cmd = new SqlCommand(@"UPDATE TrainingDetails SET TrainingType=@TrainingType,TrainingOrganizer=@TrainingOrganizer,TrainingLocation=@TrainingLocation,Batch=@Batch,DateFrom=@DateFrom,DateTo=@DateTo,CourseID=@CourseID,TrainingCategory=@TrainingCategory,NoOfDays=@NoOfDays,StartTime=@StartTime,Remarks=@Remarks,BatchStrength=@BatchStrength,Hours=@Hours,UpdatedOn=GETDATE(),UpdatedBy='Admin',HostelRequiredTrainee=@HostelRequiredTrainee,AttendanceRequired=@AttendanceRequired,AssessmentRequired=@AssessmentRequired,AssessmentMode=@AssessmentMode,InitialAssessmentRequired=@InitialAssessmentRequired,SessionAssessmentRequired=@SessionAssessmentRequired,FinalAssessmentRequired=@FinalAssessmentRequired,FeedbackRequired=@FeedbackRequired,CertificateRequired=@CertificateRequired,TrainerHostelRequired=@TrainerHostelRequired,TraineeHostelRequired=@TraineeHostelRequired WHERE TrainingID=@TrainingID", con))
                        {
                            AddParameters(cmd, trainingID, fromDate, toDate);
                            cmd.ExecuteNonQuery();
                        }
                        Session["TrainingID"] = oldTrainingID;
                        lblMessage.Text = "Batch Updated Successfully";
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand(@"INSERT INTO TrainingDetails(TrainingID,TrainingType,TrainingOrganizer,TrainingLocation,Batch,DateFrom,DateTo,CourseID,TrainingCategory,NoOfDays,StartTime,Remarks,BatchStrength,Hours,CreatedOn,CreatedBy,HostelRequiredTrainee,AttendanceRequired,AssessmentRequired,AssessmentMode,InitialAssessmentRequired,SessionAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,CertificateRequired,TrainerHostelRequired,TraineeHostelRequired) VALUES(@TrainingID,@TrainingType,@TrainingOrganizer,@TrainingLocation,@Batch,@DateFrom,@DateTo,@CourseID,@TrainingCategory,@NoOfDays,@StartTime,@Remarks,@BatchStrength,@Hours,GETDATE(),@CreatedBy,@HostelRequiredTrainee,@AttendanceRequired,@AssessmentRequired,@AssessmentMode,@InitialAssessmentRequired,@SessionAssessmentRequired,@FinalAssessmentRequired,@FeedbackRequired,@CertificateRequired,@TrainerHostelRequired,@TraineeHostelRequired)", con))
                        {
                            AddParameters(cmd, trainingID, fromDate, toDate);
                            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");
                            cmd.ExecuteNonQuery();
                        }
                        Session["TrainingID"] = trainingID;
                        lblMessage.Text = "Batch Created Successfully";
                    }
                    lblMessage.ForeColor = Color.Green;
                    SetButtonStatus();
                }
            }
            catch (Exception ex) { lblMessage.Text = ex.Message; lblMessage.ForeColor = Color.Red; }
        }

        protected void btnUpdate_Click(object sender, EventArgs e) { btnSave_Click(sender, e); }

        private void AddParameters(SqlCommand cmd, string trainingID, DateTime fromDate, DateTime toDate)
        {
            cmd.Parameters.AddWithValue("@TrainingID", trainingID);
            cmd.Parameters.AddWithValue("@TrainingType", ddlTrainingType.SelectedItem.Text); cmd.Parameters.AddWithValue("@TrainingOrganizer", ddlTrainingOrganizer.SelectedItem.Text); cmd.Parameters.AddWithValue("@TrainingLocation", ddlTrainingLocation.SelectedItem.Text); cmd.Parameters.AddWithValue("@Batch", txtBatch.Text.Trim()); cmd.Parameters.AddWithValue("@DateFrom", fromDate.ToString("dd-MM-yyyy")); cmd.Parameters.AddWithValue("@DateTo", toDate.ToString("dd-MM-yyyy")); cmd.Parameters.AddWithValue("@CourseID", ddlCourse.SelectedValue); cmd.Parameters.AddWithValue("@TrainingCategory", ddlTrainingCategory.SelectedItem.Text); cmd.Parameters.AddWithValue("@NoOfDays", txtNoOfDays.Text.Trim()); cmd.Parameters.AddWithValue("@StartTime", ddlStartTime.SelectedValue); cmd.Parameters.AddWithValue("@BatchStrength", txtStrength.Text.Trim()); cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim()); cmd.Parameters.AddWithValue("@Hours", txtHours.Text.Trim()); cmd.Parameters.AddWithValue("@HostelRequiredTrainee", chkTraineeHostelRequired.Checked ? "Yes" : "No"); cmd.Parameters.AddWithValue("@AttendanceRequired", chkAttendanceRequired.Checked); cmd.Parameters.AddWithValue("@AssessmentRequired", chkPreTrainingAssessment.Checked || chkPostTrainingAssessment.Checked); cmd.Parameters.AddWithValue("@AssessmentMode", DBNull.Value); cmd.Parameters.AddWithValue("@InitialAssessmentRequired", chkPreTrainingAssessment.Checked); cmd.Parameters.AddWithValue("@SessionAssessmentRequired", false); cmd.Parameters.AddWithValue("@FinalAssessmentRequired", chkPostTrainingAssessment.Checked); cmd.Parameters.AddWithValue("@FeedbackRequired", chkFeedbackRequired.Checked); cmd.Parameters.AddWithValue("@CertificateRequired", chkCertificateRequired.Checked); cmd.Parameters.AddWithValue("@TrainerHostelRequired", chkTrainerHostelRequired.Checked); cmd.Parameters.AddWithValue("@TraineeHostelRequired", chkTraineeHostelRequired.Checked);
        }

        private void SetButtonStatus() { }
        protected void btnCreateSessions_Click(object sender, EventArgs e) { Session["TrainingID"] = txtTrainingID.Text; Response.Redirect("~/Admin/AssignSession.aspx"); }
        protected void btnAssignTrainee_Click(object sender, EventArgs e) { Session["TrainingID"] = txtTrainingID.Text; Response.Redirect("~/Admin/AssignTrainee.aspx"); }
        protected void btnAssignFeedback_Click(object sender, EventArgs e) { Session["TrainingID"] = txtTrainingID.Text; Response.Redirect("~/Admin/AssignFeedback.aspx"); }
        private void LoadPlugins() { ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "$('#ddlCourse').select2({width:'100%'});$('#ddlTrainingType').select2({width:'100%'});$('#ddlTrainingCategory').select2({width:'100%'});$('#ddlTrainingOrganizer').select2({width:'100%'});$('#ddlTrainingLocation').select2({width:'100%'});", true); }
    }
}