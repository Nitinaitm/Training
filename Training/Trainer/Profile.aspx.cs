using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class Profile : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");

            if (!IsPostBack)
            {
                BindDropdowns();
                LoadProfile();
                LoadPlugins();
            }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindDropdowns()
        {
            // Expertise
            string expQuery = "SELECT ExpertiseID, ExpertiseName FROM AreaOfExpertiseMaster ORDER BY ExpertiseName";
            DataTable dtExp = obj.GetDataTable(expQuery, null);
            ddlExpertise.DataSource = dtExp;
            ddlExpertise.DataTextField = "ExpertiseName";
            ddlExpertise.DataValueField = "ExpertiseID";
            ddlExpertise.DataBind();
            ddlExpertise.Items.Insert(0, new ListItem("-- Select Expertise --", ""));

            // Qualification
            string qualQuery = "SELECT QualificationID, QualificationName FROM QualificationMaster ORDER BY QualificationName";
            DataTable dtQual = obj.GetDataTable(qualQuery, null);
            ddlQualification.DataSource = dtQual;
            ddlQualification.DataTextField = "QualificationName";
            ddlQualification.DataValueField = "QualificationID";
            ddlQualification.DataBind();
            ddlQualification.Items.Insert(0, new ListItem("-- Select Qualification --", ""));
        }

        private void LoadProfile()
        {
            string query = @"SELECT TM.TrainerID, TM.EmpID, E.EmpName, E.EmailId, E.MobileNo, E.EmpDesignation, E.EmpCompany, 
                                   TM.AreaOfExpertiseID, TM.QualificationID, TM.ExperienceYears, TM.Certifications, 
                                   TM.TrainerAvailability, TM.ActiveStatus, TM.Profile, TM.TrainerType, TM.TrainerPhoto 
                            FROM TrainerMaster TM 
                            LEFT JOIN EmpBasicMaster E ON TM.EmpID = E.EmpID 
                            WHERE TM.TrainerID = @TrainerID";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);

            if (dt.Rows.Count == 0) return;

            DataRow dr = dt.Rows[0];

            txtTrainerID.Text = dr["TrainerID"].ToString();
            txtEmpID.Text = dr["EmpID"]?.ToString() ?? "";
            txtFullName.Text = dr["EmpName"]?.ToString() ?? "";
            txtEmail.Text = dr["EmailId"]?.ToString() ?? "";
            txtMobile.Text = dr["MobileNo"]?.ToString() ?? "";
            txtDesignation.Text = dr["EmpDesignation"]?.ToString() ?? "";
            txtOrganization.Text = dr["EmpCompany"]?.ToString() ?? "";

            if (ddlExpertise.Items.FindByValue(dr["AreaOfExpertiseID"]?.ToString() ?? "") != null)
                ddlExpertise.SelectedValue = dr["AreaOfExpertiseID"].ToString();

            if (ddlQualification.Items.FindByValue(dr["QualificationID"]?.ToString() ?? "") != null)
                ddlQualification.SelectedValue = dr["QualificationID"].ToString();

            txtExperience.Text = dr["ExperienceYears"]?.ToString() ?? "";

            if (ddlAvailability.Items.FindByValue(dr["TrainerAvailability"]?.ToString() ?? "") != null)
                ddlAvailability.SelectedValue = dr["TrainerAvailability"].ToString();

            if (ddlStatus.Items.FindByValue(dr["ActiveStatus"]?.ToString() ?? "") != null)
                ddlStatus.SelectedValue = dr["ActiveStatus"].ToString();

            txtCertifications.Text = dr["Certifications"]?.ToString() ?? "";
            txtProfile.Text = dr["Profile"]?.ToString() ?? "";

            lblTrainerName.Text = dr["EmpName"]?.ToString() ?? "";
            lblTrainerID.Text = dr["TrainerID"].ToString();
            lblTrainerType.Text = dr["TrainerType"]?.ToString() ?? "";

            // Photo
            string photoPath = dr["TrainerPhoto"]?.ToString() ?? "";
            if (!string.IsNullOrEmpty(photoPath) && File.Exists(Server.MapPath(photoPath)))
                imgPhoto.ImageUrl = photoPath;
            else
                imgPhoto.ImageUrl = "~/Images/default-user.png";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // Validation
            if (string.IsNullOrEmpty(txtFullName.Text.Trim()))
            { lblMessage.Text = "Please enter Full Name."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            { lblMessage.Text = "Please enter Email."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            if (string.IsNullOrEmpty(txtMobile.Text.Trim()) || txtMobile.Text.Trim().Length != 10)
            { lblMessage.Text = "Please enter valid 10 digit Mobile Number."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            try
            {
                // Update Trainer Master
                string trainerQuery = @"UPDATE TrainerMaster SET 
                                        AreaOfExpertiseID = @AreaOfExpertiseID, 
                                        QualificationID = @QualificationID, 
                                        ExperienceYears = @ExperienceYears, 
                                        Certifications = @Certifications, 
                                        TrainerAvailability = @TrainerAvailability, 
                                        ActiveStatus = @ActiveStatus, 
                                        Profile = @Profile 
                                        WHERE TrainerID = @TrainerID";
                SqlParameter[] trainerParam = new SqlParameter[] {
                    new SqlParameter("@AreaOfExpertiseID", ddlExpertise.SelectedValue == "" ? (object)DBNull.Value : ddlExpertise.SelectedValue),
                    new SqlParameter("@QualificationID", ddlQualification.SelectedValue == "" ? (object)DBNull.Value : ddlQualification.SelectedValue),
                    new SqlParameter("@ExperienceYears", string.IsNullOrEmpty(txtExperience.Text.Trim()) ? (object)DBNull.Value : Convert.ToDecimal(txtExperience.Text.Trim())),
                    new SqlParameter("@Certifications", txtCertifications.Text.Trim()),
                    new SqlParameter("@TrainerAvailability", ddlAvailability.SelectedValue),
                    new SqlParameter("@ActiveStatus", ddlStatus.SelectedValue),
                    new SqlParameter("@Profile", txtProfile.Text.Trim()),
                    new SqlParameter("@TrainerID", TrainerID)
                };
                obj.ExecuteSql(trainerQuery, trainerParam);

                // Update EmpBasicMaster
                string empQuery = @"UPDATE EmpBasicMaster SET 
                                    EmpName = @EmpName, 
                                    EmailId = @Email, 
                                    MobileNo = @Mobile, 
                                    EmpDesignation = @Designation, 
                                    EmpCompany = @Organization 
                                    WHERE EmpID = (SELECT EmpID FROM TrainerMaster WHERE TrainerID = @TrainerID)";
                SqlParameter[] empParam = new SqlParameter[] {
                    new SqlParameter("@EmpName", txtFullName.Text.Trim()),
                    new SqlParameter("@Email", txtEmail.Text.Trim()),
                    new SqlParameter("@Mobile", txtMobile.Text.Trim()),
                    new SqlParameter("@Designation", txtDesignation.Text.Trim()),
                    new SqlParameter("@Organization", txtOrganization.Text.Trim()),
                    new SqlParameter("@TrainerID", TrainerID)
                };
                obj.ExecuteSql(empQuery, empParam);

                lblMessage.Text = "Profile updated successfully!";
                lblMessage.ForeColor = System.Drawing.Color.Green;

                // Update Session Labels
                lblTrainerName.Text = txtFullName.Text.Trim();
                LoadProfile();
                LoadPlugins();
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            LoadProfile();
            lblMessage.Text = "";
            LoadPlugins();
        }

        protected void btnUploadPhoto_Click(object sender, EventArgs e)
        {
            lblPhotoMsg.Text = "";

            if (!fuPhoto.HasFile)
            { lblPhotoMsg.Text = "Please select a photo."; lblPhotoMsg.ForeColor = System.Drawing.Color.Red; return; }

            try
            {
                // Check file extension
                string ext = Path.GetExtension(fuPhoto.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif")
                { lblPhotoMsg.Text = "Only JPG, PNG, GIF files allowed."; lblPhotoMsg.ForeColor = System.Drawing.Color.Red; return; }

                // Check file size (max 2MB)
                if (fuPhoto.PostedFile.ContentLength > 2 * 1024 * 1024)
                { lblPhotoMsg.Text = "File size should be less than 2MB."; lblPhotoMsg.ForeColor = System.Drawing.Color.Red; return; }

                // Create folder if not exists
                string folder = Server.MapPath("~/Uploads/TrainerPhotos/");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // Save photo
                string fileName = TrainerID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string filePath = Path.Combine(folder, fileName);
                fuPhoto.SaveAs(filePath);

                string dbPath = "~/Uploads/TrainerPhotos/" + fileName;

                // Update database
                string updateQuery = "UPDATE TrainerMaster SET TrainerPhoto = @Photo WHERE TrainerID = @TrainerID";
                SqlParameter[] updateParam = new SqlParameter[] {
                    new SqlParameter("@Photo", dbPath),
                    new SqlParameter("@TrainerID", TrainerID)
                };
                obj.ExecuteSql(updateQuery, updateParam);

                imgPhoto.ImageUrl = dbPath + "?t=" + DateTime.Now.Ticks;
                lblPhotoMsg.Text = "Photo uploaded successfully!";
                lblPhotoMsg.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                lblPhotoMsg.Text = "Error: " + ex.Message;
                lblPhotoMsg.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Select2",
                "$('#ddlExpertise,#ddlQualification').select2({width:'100%',placeholder:'Select'});", true);
        }
    }
}