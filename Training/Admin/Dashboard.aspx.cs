using System;
using System.Data;

namespace Training.Admin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null || Session["Role"] == null)
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (!IsPostBack)
            {
                LoadDashboard();
            }
        }

        private void LoadDashboard()
        {
            lblActiveTrainings.Text = Convert.ToString(obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingDetails WHERE ISNULL(TrainingStatus,'Draft') NOT IN ('Draft','Closed','Completed','TrainingCompleted')"));
            lblStarting7Days.Text = Convert.ToString(obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingDetails WHERE ISNULL(TrainingStatus,'Draft')<>'Closed' AND ISNULL(TrainingStatus,'Draft')<>'Completed' AND DateFrom>=CAST(GETDATE() AS date) AND DateFrom<DATEADD(day,8,CAST(GETDATE() AS date))"));
            lblConfirmationPending.Text = Convert.ToString(obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingDetails WHERE ISNULL(TrainingStatus,'Draft')='Draft'"));
            lblQuestionsPending.Text = Convert.ToString(obj.ExecuteScalar("SELECT COUNT(*) FROM QuestionBank WHERE ISNULL(ApprovalStatus,'') IN ('Pending','Submitted','Pending Approval')"));
            lblCertificatesPending.Text = Convert.ToString(obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment A INNER JOIN TrainingDetails TD ON TD.TrainingID=A.TrainingID WHERE ISNULL(A.AssignmentStatus,'Assigned')='Assigned' AND ISNULL(TD.CertificateRequired,0)=1 AND ISNULL(TD.CertificateSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM TrainingCertificate C WHERE C.TrainingID=A.TrainingID AND C.EmpID=A.EmpID AND ISNULL(C.CertificateStatus,'')='A')"));
            lblTrainingsThisMonth.Text = Convert.ToString(obj.ExecuteScalar("SELECT COUNT(*) FROM TrainingDetails WHERE DateFrom>=DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1) AND DateFrom<DATEADD(month,1,DATEFROMPARTS(YEAR(GETDATE()),MONTH(GETDATE()),1))"));
        }
    }
}