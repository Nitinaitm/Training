using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;

namespace Training
{
    public partial class VerifyCertificate :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        //-----------------------------------------------------
        // Page Load
        //-----------------------------------------------------

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                ResetResult();

                LoadQueryStringVerification();
            }
        }

        //-----------------------------------------------------
        // Load Query String Verification
        //-----------------------------------------------------

        private void LoadQueryStringVerification()
        {
            string certificateNo =
                Request.QueryString["CertificateNo"];

            string verificationCode =
                Request.QueryString["Code"];

            if
            (
                String.IsNullOrWhiteSpace(
                    certificateNo)
                ||
                String.IsNullOrWhiteSpace(
                    verificationCode)
            )
            {
                return;
            }

            txtCertificateNo.Text =
                certificateNo.Trim();

            txtVerificationCode.Text =
                verificationCode.Trim();

            VerifyCertificateData();
        }

        //-----------------------------------------------------
        // Verify Button
        //-----------------------------------------------------

        protected void btnVerify_Click(
            object sender,
            EventArgs e)
        {
            ResetResult();

            if
            (
                String.IsNullOrWhiteSpace(
                    txtCertificateNo.Text)
            )
            {
                ShowError(
                    "Please enter Certificate Number.");

                return;
            }

            if
            (
                String.IsNullOrWhiteSpace(
                    txtVerificationCode.Text)
            )
            {
                ShowError(
                    "Please enter Verification Code.");

                return;
            }

            VerifyCertificateData();
        }

        //-----------------------------------------------------
        // Verify Certificate
        //-----------------------------------------------------

        private void VerifyCertificateData()
        {
            string certificateNo =
                txtCertificateNo.Text
                .Trim();

            string verificationCode =
                txtVerificationCode.Text
                .Trim();

            if
            (
                String.IsNullOrWhiteSpace(
                    certificateNo)
                ||
                String.IsNullOrWhiteSpace(
                    verificationCode)
            )
            {
                return;
            }

            string query =
                "SELECT TC.CertificateID, TC.CertificateNo, TC.TrainingID, TC.EmpID, TC.GeneratedOn, TC.VerificationCode, TC.CertificateStatus, TCT.CourseTitle, TD.DateFrom, TD.DateTo, TD.CourseID, CM.CourseName, ISNULL(EBM.EmpName,TME.TraineeName) AS TraineeName FROM TrainingCertificate TC INNER JOIN TrainingCertificateTemplate TCT ON TC.TrainingID=TCT.TrainingID INNER JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TC.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TC.EmpID WHERE TC.CertificateNo=@CertificateNo AND TC.VerificationCode=@VerificationCode AND TC.CertificateStatus='A'";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@CertificateNo",
                    certificateNo),

                new SqlParameter(
                    "@VerificationCode",
                    verificationCode)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                ShowInvalid();

                return;
            }

            ShowCertificate(
                dt.Rows[0]);
        }

        //-----------------------------------------------------
        // Show Certificate
        //-----------------------------------------------------

        private void ShowCertificate(
            DataRow dr)
        {
            pnlInvalid.Visible =
                false;

            pnlCertificate.Visible =
                true;

            lblMessage.Text =
                "";

            lblCertificateNo.Text =
                Convert.ToString(
                    dr["CertificateNo"]);

            lblTraineeName.Text =
                Convert.ToString(
                    dr["TraineeName"]);

            lblEmpID.Text =
                Convert.ToString(
                    dr["EmpID"]);

            lblTrainingID.Text =
                Convert.ToString(
                    dr["TrainingID"]);

            lblCourseName.Text =
                Convert.ToString(
                    dr["CourseName"]);

            lblCourseTitle.Text =
                Convert.ToString(
                    dr["CourseTitle"]);

            lblVerificationCode.Text =
                Convert.ToString(
                    dr["VerificationCode"]);

            lblTrainingDuration.Text =
                GetTrainingDuration(
                    dr);

            lblGeneratedOn.Text =
                GetGeneratedDate(
                    dr);
        }

        //-----------------------------------------------------
        // Training Duration
        //-----------------------------------------------------

        private string GetTrainingDuration(
            DataRow dr)
        {
            if
            (
                dr["DateFrom"]
                ==
                DBNull.Value
                ||
                dr["DateTo"]
                ==
                DBNull.Value
            )
            {
                return "";
            }

            DateTime dateFrom =
                Convert.ToDateTime(
                    dr["DateFrom"]);

            DateTime dateTo =
                Convert.ToDateTime(
                    dr["DateTo"]);

            if
            (
                dateFrom.Date
                ==
                dateTo.Date
            )
            {
                return
                    dateFrom
                    .ToString(
                        "dd-MM-yyyy");
            }

            return
                dateFrom
                .ToString(
                    "dd-MM-yyyy")
                +
                " to "
                +
                dateTo
                .ToString(
                    "dd-MM-yyyy");
        }

        //-----------------------------------------------------
        // Generated Date
        //-----------------------------------------------------

        private string GetGeneratedDate(
            DataRow dr)
        {
            if
            (
                dr["GeneratedOn"]
                ==
                DBNull.Value
            )
            {
                return "";
            }

            DateTime generatedOn =
                Convert.ToDateTime(
                    dr["GeneratedOn"]);

            return
                generatedOn
                .ToString(
                    "dd-MM-yyyy hh:mm tt");
        }

        //-----------------------------------------------------
        // Invalid Certificate
        //-----------------------------------------------------

        private void ShowInvalid()
        {
            pnlCertificate.Visible =
                false;

            pnlInvalid.Visible =
                true;

            lblMessage.Text =
                "";
        }

        //-----------------------------------------------------
        // Error
        //-----------------------------------------------------

        private void ShowError(
            string message)
        {
            pnlCertificate.Visible =
                false;

            pnlInvalid.Visible =
                false;

            lblMessage.ForeColor =
                System.Drawing.Color.Red;

            lblMessage.Text =
                message;
        }

        //-----------------------------------------------------
        // Reset Result
        //-----------------------------------------------------

        private void ResetResult()
        {
            pnlCertificate.Visible =
                false;

            pnlInvalid.Visible =
                false;

            lblMessage.Text =
                "";
        }

        //-----------------------------------------------------
        // Reset Button
        //-----------------------------------------------------

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtCertificateNo.Text =
                "";

            txtVerificationCode.Text =
                "";

            ResetResult();

            txtCertificateNo.Focus();
        }
    }
}