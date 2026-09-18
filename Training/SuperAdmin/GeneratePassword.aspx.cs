using System;

namespace Training.SuperAdmin
{
    public partial class GeneratePassword :
    System.Web.UI.Page
    {
        protected void btnGenerate_Click(
        object sender,
        EventArgs e)
        {

            Encryptor2 encry = new Encryptor2();
            string pass = encry.Encrypt(txtPassword.Text);

            lblPassword.Text =
            "Generated Password : "
            + pass;

           
        }

        protected void btnDecrypt_Click(
        object sender,
        EventArgs e)
        {

            Encryptor2 encry = new Encryptor2();
            

            string decryptedPassword = encry.Decrypt(txtEncPassword.Text);
            lblDecryptedPassword.Text =
           "Decrypted Password : "
           + decryptedPassword;
        }
    }
}