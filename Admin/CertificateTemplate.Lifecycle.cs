using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class CertificateTemplate
    {
        private Button btnNextToManage;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnNextToManage = new Button
            {
                ID = "btnNextToManage",
                Text = "Next",
                CssClass = "btn btn-primary ms-2",
                CausesValidation = false,
                Visible = false
            };
            btnNextToManage.Click += btnNextToManage_Click;

            if (btnSave != null && btnSave.Parent != null)
                btnSave.Parent.Controls.Add(btnNextToManage);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (btnNextToManage != null)
            {
                string trainingID = Convert.ToString(Session["TrainingID"]);
                btnNextToManage.Visible = HasConfiguration(trainingID);
            }
            base.OnPreRender(e);
        }

        private bool HasConfiguration(string trainingID)
        {
            if (string.IsNullOrWhiteSpace(trainingID)) return false;
            string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = new SqlCommand(@"SELECT COUNT(*) FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND ISNULL(TemplateID,'')<>'' AND ISNULL(CourseTitle,'')<>''", con))
            {
                cmd.Parameters.AddWithValue("@TrainingID", trainingID);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private void btnNextToManage_Click(object sender, EventArgs e)
        {
            Response.Redirect("ManageTraining.aspx");
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
