using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
        }

        private string TrainerID => Session["TrainerID"].ToString();

        protected void btnChange_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            // Validation
            if (string.IsNullOrEmpty(txtCurrentPassword.Text.Trim()))
            { lblMessage.Text = "Please enter Current Password."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            if (string.IsNullOrEmpty(txtNewPassword.Text.Trim()))
            { lblMessage.Text = "Please enter New Password."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            if (txtNewPassword.Text.Trim().Length < 6)
            { lblMessage.Text = "New Password must be at least 6 characters."; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            if (txtNewPassword.Text.Trim() != txtConfirmPassword.Text.Trim())
            { lblMessage.Text = "New Password and Confirm Password do not match!"; lblMessage.ForeColor = System.Drawing.Color.Red; return; }

            try
            {
                // Get current password from database
                string getQuery = @"SELECT Password FROM Login WHERE CorrespondingEmpID = (SELECT EmpID FROM TrainerMaster WHERE TrainerID = @TrainerID)";
                SqlParameter[] getParam = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
                string currentPwd = obj.ExecuteScalar(getQuery, getParam)?.ToString() ?? "";

                // If password is stored in plain text (for demo) - In production, use hashing
                // For now, we'll compare plain text (you should use encryption in production)
                // If your passwords are encrypted, use Encryptor.Encrypt(txtCurrentPassword.Text.Trim())
                // For demo, we'll use plain text comparison
                if (!string.IsNullOrEmpty(currentPwd))
                {
                    // If using encryption, uncomment the line below and comment the plain text line
                    // string encryptedCurrent = Encryptor.Encrypt(txtCurrentPassword.Text.Trim());
                    // if (currentPwd != encryptedCurrent)

                    // Plain text comparison (for demo only)
                    if (currentPwd != txtCurrentPassword.Text.Trim())
                    {
                        lblMessage.Text = "Current Password is incorrect.";
                        lblMessage.ForeColor = System.Drawing.Color.Red;
                        return;
                    }
                }

                // Update password
                string updateQuery = @"UPDATE Login SET Password = @Password WHERE CorrespondingEmpID = (SELECT EmpID FROM TrainerMaster WHERE TrainerID = @TrainerID)";

                // If using encryption, use Encryptor.Encrypt(txtNewPassword.Text.Trim())
                // string encryptedNew = Encryptor.Encrypt(txtNewPassword.Text.Trim());
                // SqlParameter[] updateParam = new SqlParameter[] { new SqlParameter("@Password", encryptedNew) };

                // Plain text for demo
                SqlParameter[] updateParam = new SqlParameter[] {
                    new SqlParameter("@Password", txtNewPassword.Text.Trim())
                };
                int rows = obj.ExecuteSql(updateQuery, updateParam);

                if (rows > 0)
                {
                    lblMessage.Text = "Password changed successfully!";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    ClearForm();
                }
                else
                {
                    lblMessage.Text = "Failed to change password. Please try again.";
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Error: " + ex.Message;
                lblMessage.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
            lblMessage.Text = "";
        }

        private void ClearForm()
        {
            txtCurrentPassword.Text = "";
            txtNewPassword.Text = "";
            txtConfirmPassword.Text = "";
        }
    }
}