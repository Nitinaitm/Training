using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text;
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

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;


        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                if (Request.QueryString["mode"] != "edit")
                {
                    Session.Remove("TrainingID");
                }

                BindTrainingType();
                BindTrainingCategory();
                BindOrganizer();
                BindLocation();

                ddlTrainingType.Items.Insert(0, new ListItem("Select Training Type", ""));
                ddlTrainingCategory.Items.Insert(0, new ListItem("Select Training Category", ""));
                ddlTrainingOrganizer.Items.Insert(0, new ListItem("Select Organizer", ""));
                ddlTrainingLocation.Items.Insert(0, new ListItem("Select Location", ""));



                BindCourse();
                BindStartTime();

                if (Request.QueryString["mode"] == "edit"
                    && Session["TrainingID"] != null)
                {
                    LoadTrainingForEdit(
                        Session["TrainingID"].ToString());
                }
                else
                {
                    SetButtonStatus();
                }

                LoadPlugins();
            }
        }
        private void LoadTrainingForEdit(string trainingID)
        {
            clsDataAccess obj = new clsDataAccess();
            DataTable dt =
            obj.GetDataTable(@"

SELECT *
FROM TrainingDetails
WHERE TrainingID='"
        + trainingID + "'");

            if (dt.Rows.Count == 0)
                return;

            DataRow dr = dt.Rows[0];

            txtTrainingID.Text =
            dr["TrainingID"].ToString();

            //-----------------------------------
            // Training Type
            //-----------------------------------

            ListItem typeItem =
            ddlTrainingType.Items.FindByText(
            dr["TrainingType"].ToString());

            if (typeItem != null)
            {
                ddlTrainingType.ClearSelection();
                typeItem.Selected = true;
            }

            //-----------------------------------
            // Training Type
            //-----------------------------------

            ListItem categoryItem =
            ddlTrainingCategory.Items.FindByText(
            dr["TrainingCategory"].ToString());

            if (categoryItem != null)
            {
                ddlTrainingCategory.ClearSelection();
                categoryItem.Selected = true;
            }

            //-----------------------------------
            // Organizer
            //-----------------------------------


            ListItem orgItem =
            ddlTrainingOrganizer.Items.FindByText(
            dr["TrainingOrganizer"].ToString());

            if (orgItem != null)
            {
                ddlTrainingOrganizer.ClearSelection();
                orgItem.Selected = true;
            }

            //-----------------------------------
            // Location
            //-----------------------------------


            ListItem locItem =
            ddlTrainingLocation.Items.FindByText(
            dr["TrainingLocation"].ToString());

            if (locItem != null)
            {
                ddlTrainingLocation.ClearSelection();
                locItem.Selected = true;
            }

            //-----------------------------------
            // Topic
            //-----------------------------------

            if (ddlCourse.Items.FindByValue(
    dr["CourseID"].ToString()) != null)
            {
                ddlCourse.SelectedValue =
                    dr["CourseID"].ToString();
            }

            //-----------------------------------
            // Category
            //-----------------------------------



            //-----------------------------------
            // Other Fields
            //-----------------------------------

            txtBatch.Text =
            dr["Batch"].ToString();

            //txtBatchName.Text =
            //dr["BatchName"].ToString();

            txtDateFrom.Text =
Convert.ToDateTime(dr["DateFrom"])
.ToString("dd-MM-yyyy");

            txtDateTo.Text =
            Convert.ToDateTime(dr["DateTo"])
            .ToString("dd-MM-yyyy");

            txtNoOfDays.Text =
            dr["NoOfDays"].ToString();

            txtHours.Text =
            dr["Hours"].ToString();

            txtStrength.Text =
            dr["BatchStrength"].ToString();

            txtRemarks.Text =
            dr["Remarks"].ToString();

            chkAttendanceRequired.Checked = Convert.ToBoolean(dr["AttendanceRequired"]);
            chkPreTrainingAssessment.Checked = Convert.ToBoolean(dr["InitialAssessmentRequired"]);
            chkPostTrainingAssessment.Checked = Convert.ToBoolean(dr["FinalAssessmentRequired"]);
            chkFeedbackRequired.Checked = Convert.ToBoolean(dr["FeedbackRequired"]);
            chkCertificateRequired.Checked = Convert.ToBoolean(dr["CertificateRequired"]);
            chkTrainerHostelRequired.Checked = Convert.ToBoolean(dr["TrainerHostelRequired"]);
            chkTraineeHostelRequired.Checked = Convert.ToBoolean(dr["TraineeHostelRequired"]);

            if (ddlStartTime.Items.FindByValue(
                dr["StartTime"].ToString()) != null)
            {
                ddlStartTime.SelectedValue =
                dr["StartTime"].ToString();
            }

            SetButtonStatus();

        }

        private void BindStartTime()
        {
            ddlStartTime.Items.Clear();

            ddlStartTime.Items.Add(
                new System.Web.UI.WebControls.ListItem("Select Time", ""));

            DateTime dt = DateTime.Today.AddHours(6); // 06:00 AM

            DateTime endTime = DateTime.Today.AddHours(22); // 10:00 PM

            while (dt <= endTime)
            {
                ddlStartTime.Items.Add(new System.Web.UI.WebControls.ListItem(dt.ToString("hh:mm tt"),
                    dt.ToString("hh:mm tt")));

                dt = dt.AddMinutes(15);
            }
        }

        private void BindCourse()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@" SELECT
CourseID,CourseName FROM CourseMaster ORDER BY CourseName", con);

                con.Open();

                ddlCourse.DataSource = cmd.ExecuteReader();

                ddlCourse.DataTextField = "CourseName";

                ddlCourse.DataValueField = "CourseID";

                ddlCourse.DataBind();

                ddlCourse.Items.Insert(0, new ListItem("Select Course", ""));
            }
        }


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
                // string batchName = txtBatchName.Text.Trim().Replace(" ", "").ToUpper();



                DateTime fromDate = DateTime.ParseExact(txtDateFrom.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);


                DateTime toDate = DateTime.ParseExact(txtDateTo.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture);



                string fromPart = fromDate.ToString("ddMMyy");
                string toPart = toDate.ToString("ddMMyy");



                // StringBuilder desig = new StringBuilder();



                //foreach (ListItem item in lstDesignation.Items)
                //{
                //    if (item.Selected)
                //    {
                //        string[] words = item.Text.Split(' ');

                //        foreach (string w in words)
                //        {
                //            if (!string.IsNullOrWhiteSpace(w))
                //            {
                //                char firstValidChar = '\0';

                //                foreach (char ch in w)
                //                {
                //                    if (char.IsLetterOrDigit(ch))
                //                    {
                //                        firstValidChar = char.ToUpper(ch);
                //                        break;
                //                    }
                //                }

                //                if (firstValidChar != '\0')
                //                {
                //                    desig.Append(firstValidChar);
                //                }
                //            }
                //        }
                //    }
                //}

                string prefix =

                "TR"

                + "-"
                + courseID

                + "-"

                + trainingType

                + "-"

                + organizer

                + "-"

                + location

                + "-"

                + batch

                + "-"

                + fromPart

                + "-"

                + toPart;



                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(@" SELECT COUNT(*)

FROM TrainingDetails WHERE TrainingID LIKE @Prefix+'%'", con);


                    cmd.Parameters.AddWithValue("@Prefix", prefix);


                    int count = Convert.ToInt32(cmd.ExecuteScalar());


                    txtTrainingID.Text = prefix + "-" + (count + 1).ToString("000");
                }

            }

            catch
            {
            }
        }


        private void BindTrainingType()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingTypeID,
TrainingType

FROM TrainingMaster

ORDER BY TrainingType";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);

                con.Open();

                ddlTrainingType
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingType
                .DataTextField =
                "TrainingType";

                ddlTrainingType
                .DataValueField =
                "TrainingTypeID";

                ddlTrainingType
                .DataBind();

                con.Close();


            }
        }

        private void BindTrainingCategory()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingCategoryID,
TrainingCategory

FROM TrainingCategoryMaster

ORDER BY TrainingCategory";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);

                con.Open();

                ddlTrainingCategory
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingCategory
                .DataTextField =
                "TrainingCategory";

                ddlTrainingCategory
                .DataValueField =
                "TrainingCategoryID";

                ddlTrainingCategory
                .DataBind();

                con.Close();


            }
        }








        private void BindOrganizer()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingOrganizerID,
TrainingOrganizer

FROM
TrainingOrganizerMaster";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);



                con.Open();

                ddlTrainingOrganizer
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingOrganizer
                .DataTextField =
                "TrainingOrganizer";

                ddlTrainingOrganizer
                .DataValueField =
                "TrainingOrganizerID";

                ddlTrainingOrganizer
                .DataBind();

                con.Close();


            }
        }








        private void BindLocation()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingLocationID,
TrainingLocation

FROM
TrainingLocationMaster";



                SqlCommand cmd =
                new SqlCommand(
                query,
                con);





                con.Open();

                ddlTrainingLocation
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingLocation
                .DataTextField =
                "TrainingLocation";


                ddlTrainingLocation.DataValueField =
"TrainingLocationID";
                ddlTrainingLocation
                .DataBind();

                con.Close();


            }
        }



      
        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            lblMessage.Text = "";

            try
            {
                GenerateTrainingID();

                if (!Page.IsValid)
                    return;
                DateTime fromDate;

                DateTime toDate;

                if (!DateTime.TryParseExact(txtDateFrom.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDate))
                {
                    lblMessage.Text = "Please select valid Date From.";
                    lblMessage.ForeColor = Color.Red;
                    return;
                }

                if (!DateTime.TryParseExact(txtDateTo.Text.Trim(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDate))
                {
                    lblMessage.Text = "Please select valid Date To.";
                    lblMessage.ForeColor = Color.Red;
                    return;
                }

                txtNoOfDays.Text = ((toDate - fromDate).Days + 1).ToString();

                if (Convert.ToInt32(txtNoOfDays.Text) <= 0)
                {
                    lblMessage.Text = "Date To should be greater than or equal to Date From.";
                    lblMessage.ForeColor = Color.Red;
                    return;
                }
                int strength;

                if (!int.TryParse(txtStrength.Text.Trim(), out strength))
                {
                    lblMessage.Text = "Enter valid Batch Strength.";

                    lblMessage.ForeColor = System.Drawing.Color.Red;

                    return;
                }
                using (
                SqlConnection con =
                new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand cmd =
                    new SqlCommand(@"

INSERT INTO
TrainingDetails
(
TrainingID,
TrainingType,
TrainingOrganizer,
TrainingLocation,
Batch,
DateFrom,
DateTo,
CourseID,
TrainingCategory,
NoOfDays,
StartTime,
BatchStrength,
Remarks,
CreatedOn,
CreatedBy,
Hours,
HostelRequiredTrainee,
AttendanceRequired,
AssessmentRequired,
AssessmentMode,
InitialAssessmentRequired,
SessionAssessmentRequired,
FinalAssessmentRequired,
FeedbackRequired,
CertificateRequired,
TrainerHostelRequired,
TraineeHostelRequired
)

VALUES
(
@TrainingID,
@TrainingType,
@TrainingOrganizer,
@TrainingLocation,
@Batch,
@DateFrom,
@DateTo,
@CourseID,
@TrainingCategory,
@NoOfDays,
@StartTime,
@BatchStrength,
@Remarks,
GETDATE(),
@CreatedBy,
@Hours,
@HostelRequiredTrainee,
@AttendanceRequired,
@AssessmentRequired,
@AssessmentMode,
@InitialAssessmentRequired,
@SessionAssessmentRequired,
@FinalAssessmentRequired,
@FeedbackRequired,
@CertificateRequired,
@TrainerHostelRequired,
@TraineeHostelRequired
)

", con);


                    cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    txtTrainingID.Text);

                    cmd.Parameters.AddWithValue(
                    "@TrainingType",
                    ddlTrainingType.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@TrainingOrganizer",
                    ddlTrainingOrganizer.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@TrainingLocation",
                    ddlTrainingLocation.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@Batch",
                    txtBatch.Text.Trim());

                    //cmd.Parameters.AddWithValue(
                    //"@BatchName",
                    //txtBatchName.Text.Trim());

                    cmd.Parameters.AddWithValue("@DateFrom", fromDate.ToString("dd-MM-yyyy"));
                    cmd.Parameters.AddWithValue("@DateTo", toDate.ToString("dd-MM-yyyy"));

                    cmd.Parameters.AddWithValue(
                    "@CourseID",
                    ddlCourse.SelectedValue);

                    cmd.Parameters.AddWithValue(
                    "@TrainingCategory",
                    ddlTrainingCategory.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@NoOfDays",
                    txtNoOfDays.Text);

                    cmd.Parameters.AddWithValue(
    "@StartTime",
    ddlStartTime.SelectedValue);

                    cmd.Parameters.AddWithValue(
                    "@BatchStrength",
                    txtStrength.Text.Trim());

                    cmd.Parameters.AddWithValue(
                   "@Remarks",
                   txtRemarks.Text.Trim());


                    cmd.Parameters.AddWithValue(
                    "@CreatedBy",
                    "Admin");

                    cmd.Parameters.AddWithValue(
                   "@Hours",
                   txtHours.Text.Trim());

                    cmd.Parameters.AddWithValue(
                    "@HostelRequiredTrainee",
                    chkTraineeHostelRequired.Checked ? "Yes" : "No");

                    cmd.Parameters.AddWithValue("@AttendanceRequired", chkAttendanceRequired.Checked);
                    cmd.Parameters.AddWithValue("@AssessmentRequired", chkPreTrainingAssessment.Checked || chkPostTrainingAssessment.Checked);
                    cmd.Parameters.AddWithValue("@AssessmentMode", DBNull.Value);
                    cmd.Parameters.AddWithValue("@InitialAssessmentRequired", chkPreTrainingAssessment.Checked);
                    cmd.Parameters.AddWithValue("@SessionAssessmentRequired", false);
                    cmd.Parameters.AddWithValue("@FinalAssessmentRequired", chkPostTrainingAssessment.Checked);
                    cmd.Parameters.AddWithValue("@FeedbackRequired", chkFeedbackRequired.Checked);
                    cmd.Parameters.AddWithValue("@CertificateRequired", chkCertificateRequired.Checked);
                    cmd.Parameters.AddWithValue("@TrainerHostelRequired", chkTrainerHostelRequired.Checked);
                    cmd.Parameters.AddWithValue("@TraineeHostelRequired", chkTraineeHostelRequired.Checked);

                    cmd.ExecuteNonQuery();

                    //clsWorkflow.UpdateWorkflow(txtTrainingID.Text, "Draft", 1);
                    clsWorkflow.UpdateWorkflow(txtTrainingID.Text, "Draft", "A");


               }

                lblMessage.Text =
                "Batch created successfully";

                lblMessage.ForeColor =
                Color.Green;
                Session["TrainingID"] =
                txtTrainingID.Text;

                SetButtonStatus();
            }
            catch (Exception ex)
            {
                lblMessage.Text =
                ex.Message;

                lblMessage.ForeColor =
                Color.Red;
            }
        }

        protected void btnUpdate_Click(
object sender,
EventArgs e)
        {
            if (ddlTrainingType.SelectedIndex <= 0)
            {
                lblMessage.Text =
                "Select Training Type";
                return;
            }

            if (ddlTrainingOrganizer.SelectedIndex <= 0)
            {
                lblMessage.Text =
                "Select Training Organizer";
                return;
            }

            if (ddlTrainingLocation.SelectedIndex <= 0)
            {
                lblMessage.Text =
                "Select Training Location";
                return;
            }

            if (ddlCourse.SelectedIndex <= 0)
            {
                lblMessage.Text =
                "Select Course";
                return;
            }

            if (ddlTrainingCategory.SelectedIndex <= 0)
            {
                lblMessage.Text =
                "Select Training Category";
                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtBatch.Text))
            {
                lblMessage.Text =
                "Enter Batch";
                return;
            }


            if (string.IsNullOrWhiteSpace(
                txtDateFrom.Text))
            {
                lblMessage.Text =
                "Select Date From";
                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtDateTo.Text))
            {
                lblMessage.Text =
                "Select Date To";
                return;
            }

            if (string.IsNullOrWhiteSpace(
                txtHours.Text))
            {
                lblMessage.Text =
                "Enter Hours";
                return;
            }

            if (ddlStartTime.SelectedIndex <= 0)
            {
                lblMessage.Text =
                "Select Start Time";
                return;
            }

            GenerateTrainingID();

            UpdateTraining();
        }
        protected void btnAssignTrainee_Click(
    object sender,
    EventArgs e)
        {
            Session["TrainingID"] =
                txtTrainingID.Text;

            Response.Redirect(
                "~/Admin/AssignTrainee.aspx");
        }
        private void SetButtonStatus()
        {
            // New Record
            btnSave.Visible = String.IsNullOrWhiteSpace(txtTrainingID.Text);

            btnUpdate.Visible = !btnSave.Visible;

            btnUpdate.Enabled = true;
            btnCreateSessions.Enabled = true;
            btnAssignTrainee.Enabled = true;

            //-----------------------------------------
            // New Batch (Not Saved)
            //-----------------------------------------

            if (String.IsNullOrWhiteSpace(txtTrainingID.Text))
            {
                btnCreateSessions.Enabled = false;
                btnAssignTrainee.Enabled = false;
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                //-----------------------------------------
                // Training Completed
                //-----------------------------------------

                SqlCommand cmdStatus = new SqlCommand(@"
SELECT TrainingStatus
FROM TrainingDetails
WHERE TrainingID=@TrainingID", con);

                cmdStatus.Parameters.AddWithValue(
                    "@TrainingID",
                    txtTrainingID.Text);

                string status =
                    Convert.ToString(
                    cmdStatus.ExecuteScalar());

                if (status == "Completed")
                {
                    btnUpdate.Enabled = false;
                    btnCreateSessions.Enabled = false;
                    btnAssignTrainee.Enabled = false;
                    return;
                }

                //-----------------------------------------
                // Session Created ?
                //-----------------------------------------

                SqlCommand cmdSession = new SqlCommand(@"
SELECT COUNT(*)
FROM SessionMaster
WHERE TrainingID=@TrainingID", con);

                cmdSession.Parameters.AddWithValue(
                    "@TrainingID",
                    txtTrainingID.Text);

                int sessionCount =
                    Convert.ToInt32(
                    cmdSession.ExecuteScalar());

                //-----------------------------------------
                // Trainee Assigned ?
                //-----------------------------------------

                SqlCommand cmdTrainee = new SqlCommand(@"
SELECT COUNT(*)
FROM TrainingAssignment
WHERE TrainingID=@TrainingID
AND ISNULL(AssignmentStatus,'Assigned')='Assigned'
", con);

                cmdTrainee.Parameters.AddWithValue(
                    "@TrainingID",
                    txtTrainingID.Text);

                int traineeCount =
                    Convert.ToInt32(
                    cmdTrainee.ExecuteScalar());

                //-----------------------------------------
                // Batch Lock
                //-----------------------------------------

                if (sessionCount > 0 || traineeCount > 0)
                {
                    // Sirf Batch Update lock hoga
                    btnUpdate.Enabled = false;

                    // Session aur Trainee dono chalte rahenge
                    btnCreateSessions.Enabled = true;
                    btnAssignTrainee.Enabled = true;
                }
            }
        }

        private void UpdateTraining()
        {
            try
            {
                string oldTrainingID =
                Session["TrainingID"]
                .ToString();

                string newTrainingID =
                txtTrainingID.Text.Trim();

                DateTime fromDate =
                DateTime.ParseExact(
                txtDateFrom.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

                DateTime toDate =
                DateTime.ParseExact(
                txtDateTo.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

                using (SqlConnection con =
                new SqlConnection(constr))
                {
                    con.Open();

                    SqlTransaction trans =
                    con.BeginTransaction();

                    try
                    {
                        //--------------------------------------------------
                        // Update TrainingDetails
                        //--------------------------------------------------

                        SqlCommand cmd =
                        new SqlCommand(@"

UPDATE TrainingDetails

SET

TrainingID=@NewTrainingID,
TrainingType=@TrainingType,
TrainingOrganizer=@TrainingOrganizer,
TrainingLocation=@TrainingLocation,
Batch=@Batch,
DateFrom=@DateFrom,
DateTo=@DateTo,
CourseID=@CourseID,
TrainingCategory=@TrainingCategory,
NoOfDays=@NoOfDays,
StartTime=@StartTime,
Remarks=@Remarks,
BatchStrength=@BatchStrength,
Hours=@Hours,
UpdatedOn=GETDATE(),
UpdatedBy=@UpdatedBy,
HostelRequiredTrainee=@HostelRequiredTrainee,
AttendanceRequired=@AttendanceRequired,
AssessmentRequired=@AssessmentRequired,
AssessmentMode=@AssessmentMode,
InitialAssessmentRequired=@InitialAssessmentRequired,
SessionAssessmentRequired=@SessionAssessmentRequired,
FinalAssessmentRequired=@FinalAssessmentRequired,
FeedbackRequired=@FeedbackRequired,
CertificateRequired=@CertificateRequired,
TrainerHostelRequired=@TrainerHostelRequired,
TraineeHostelRequired=@TraineeHostelRequired

WHERE TrainingID=@OldTrainingID

", con, trans);

                        cmd.Parameters.AddWithValue(
                        "@NewTrainingID",
                        newTrainingID);

                        cmd.Parameters.AddWithValue(
                        "@OldTrainingID",
                        oldTrainingID);

                        cmd.Parameters.AddWithValue(
                        "@TrainingType",
                        ddlTrainingType.SelectedItem.Text);

                        cmd.Parameters.AddWithValue(
                        "@TrainingOrganizer",
                        ddlTrainingOrganizer.SelectedItem.Text);

                        cmd.Parameters.AddWithValue(
                        "@TrainingLocation",
                        ddlTrainingLocation.SelectedItem.Text);

                        cmd.Parameters.AddWithValue(
                        "@Batch",
                        txtBatch.Text.Trim());


                        cmd.Parameters.AddWithValue(
                        "@DateFrom",
                        fromDate.ToString("dd-MM-yyyy"));

                        cmd.Parameters.AddWithValue(
                        "@DateTo",
                        toDate.ToString("dd-MM-yyyy"));

                        cmd.Parameters.AddWithValue(
                        "@CourseID",
                        ddlCourse.SelectedValue);

                        cmd.Parameters.AddWithValue(
                        "@TrainingCategory",
                        ddlTrainingCategory.SelectedItem.Text);

                        cmd.Parameters.AddWithValue(
                        "@NoOfDays",
                        txtNoOfDays.Text.Trim());

                        cmd.Parameters.AddWithValue(
                        "@StartTime",
                        ddlStartTime.SelectedValue);

                        cmd.Parameters.AddWithValue(
                        "@BatchStrength",
                        txtStrength.Text.Trim());

                        cmd.Parameters.AddWithValue(
                        "@Remarks",
                        txtRemarks.Text.Trim());

                        cmd.Parameters.AddWithValue(
                        "@Hours",
                        txtHours.Text.Trim());

                        cmd.Parameters.AddWithValue(
                        "@UpdatedBy",
                        "Admin");

                        cmd.Parameters.AddWithValue(
                        "@HostelRequiredTrainee",
                        chkTraineeHostelRequired.Checked ? "Yes" : "No");

                        cmd.Parameters.AddWithValue("@AttendanceRequired", chkAttendanceRequired.Checked);
                        cmd.Parameters.AddWithValue("@AssessmentRequired", chkPreTrainingAssessment.Checked || chkPostTrainingAssessment.Checked);
                        cmd.Parameters.AddWithValue("@AssessmentMode", DBNull.Value);
                        cmd.Parameters.AddWithValue("@InitialAssessmentRequired", chkPreTrainingAssessment.Checked);
                        cmd.Parameters.AddWithValue("@SessionAssessmentRequired", false);
                        cmd.Parameters.AddWithValue("@FinalAssessmentRequired", chkPostTrainingAssessment.Checked);
                        cmd.Parameters.AddWithValue("@FeedbackRequired", chkFeedbackRequired.Checked);
                        cmd.Parameters.AddWithValue("@CertificateRequired", chkCertificateRequired.Checked);
                        cmd.Parameters.AddWithValue("@TrainerHostelRequired", chkTrainerHostelRequired.Checked);
                        cmd.Parameters.AddWithValue("@TraineeHostelRequired", chkTraineeHostelRequired.Checked);

                        cmd.ExecuteNonQuery();



                        trans.Commit();

                        Session["TrainingID"] =
                        newTrainingID;

                        lblMessage.Text =
                        "Batch Updated Successfully";

                        lblMessage.ForeColor =
                        Color.Green;

                        SetButtonStatus();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.ForeColor = Color.Red;
            }
        }

        protected void btnCreateSessions_Click(
        object sender,
        EventArgs e)
        {
            Session["TrainingID"] =
                txtTrainingID.Text;

            Response.Redirect(
                "~/Admin/CreateSession.aspx");
        }

        private void CalculateDays()
        {
            try
            {
                DateTime fromDate =
                DateTime.ParseExact(
                txtDateFrom.Text,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

                DateTime toDate =
                DateTime.ParseExact(
                txtDateTo.Text,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

                txtNoOfDays.Text =
                ((toDate - fromDate).Days + 1)
                .ToString();
            }
            catch
            {
                txtNoOfDays.Text = "";
            }
        }

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            Guid.NewGuid().ToString(),

             "$('#ddlCourse').select2({width:'100%'});" +
            "$('#ddlTrainingType').select2({width:'100%'});" +
            "$('#ddlTrainingCategory').select2({width:'100%'});" +
            "$('#ddlTrainingOrganizer').select2({width:'100%'});" +
            "$('#ddlTrainingLocation').select2({width:'100%'});",
            true);
        }


    }
}