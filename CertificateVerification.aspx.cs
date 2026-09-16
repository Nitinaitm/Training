using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;

namespace Training
{
    public partial class CertificateVerification :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();


        //---------------------------------------------------------
        // Page Load
        //---------------------------------------------------------

        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            if (!IsPostBack)
            {
                ClearResult();

                string certificateNo =
                    Request.QueryString["CertificateNo"];

                string verificationCode =
                    Request.QueryString["VerificationCode"];

                if
                (
                    !String.IsNullOrWhiteSpace(
                        certificateNo)
                    &&
                    !String.IsNullOrWhiteSpace(
                        verificationCode)
                )
                {
                    txtCertificateNo.Text =
                        certificateNo;

                    txtVerificationCode.Text =
                        verificationCode;

                    VerifyCertificate();
                }
            }
        }


        //---------------------------------------------------------
        // Verify Button
        //---------------------------------------------------------

        protected void btnVerify_Click(
     object sender,
     EventArgs e)
        {
            VerifyCertificate();
        }

        private void VerifyCertificate()
        {
            ClearResult();

            string certificateNo =
                txtCertificateNo.Text.Trim();

            string verificationCode =
                txtVerificationCode.Text.Trim();


            if
            (
                String.IsNullOrWhiteSpace(
                    certificateNo)
            )
            {
                ShowError(
                    "Please enter Certificate Number.");

                return;
            }


            if
            (
                String.IsNullOrWhiteSpace(
                    verificationCode)
            )
            {
                ShowError(
                    "Please enter Verification Code.");

                return;
            }


            DataTable dt =
                GetCertificate(
                    certificateNo,
                    verificationCode);


            if
            (
                dt.Rows.Count == 0
            )
            {
                ShowInvalid(
                    "Certificate could not be verified.");

                return;
            }


            DataRow dr =
                dt.Rows[0];


            string expectedHash =
                GenerateCertificateHash(
                    dr["CertificateNo"].ToString(),
                    dr["TrainingID"].ToString(),
                    dr["EmpID"].ToString(),
                    dr["VerificationCode"].ToString());


            string storedHash =
                dr["CertificateHash"].ToString();


            if
            (
                String.IsNullOrWhiteSpace(
                    storedHash)
                ||
                !String.Equals(
                    expectedHash,
                    storedHash,
                    StringComparison.OrdinalIgnoreCase)
            )
            {
                ShowInvalid(
                    "Certificate verification failed.");

                return;
            }


            ShowValid(
                dr);
        }

        //---------------------------------------------------------
        // Get Certificate
        //---------------------------------------------------------

        private DataTable GetCertificate(
            string certificateNo,
            string verificationCode)
        {
            string sql =
        @"
SELECT TOP 1

    TC.CertificateID,
    TC.CertificateNo,
    TC.TrainingID,
    TC.EmpID,
    TC.TemplateID,
    TC.PDFPath,
    TC.PDFName,
    TC.GeneratedOn,
    TC.CertificateStatus,
    TC.CertificateHash,
    TC.VerificationCode,
    TC.Active,

    TM.CourseID,
    TM.TestTitle,

    T.TrainingType,
    T.TrainingOrganizer,
    T.TrainingLocation,
    T.DateFrom,
    T.DateTo,

    E.EmpName

FROM
    TrainingCertificate TC

LEFT JOIN
    TestMaster TM
ON
    TM.TrainingID=TC.TrainingID

LEFT JOIN
    TrainingDetails T
ON
    T.TrainingID=TC.TrainingID

LEFT JOIN
    EmpbasicMaster E
ON
    E.EmpID=TC.EmpID

WHERE
    TC.CertificateNo=@CertificateNo

AND
    TC.VerificationCode=@VerificationCode

AND
    TC.Active=1

AND
    TC.CertificateStatus='A'
";


            SqlParameter[] param =
            {
                new SqlParameter(
                    "@CertificateNo",
                    certificateNo),

                new SqlParameter(
                    "@VerificationCode",
                    verificationCode)
            };


            return
                objDB.GetDataTable(
                    sql,
                    param);
        }


        //---------------------------------------------------------
        // Generate Hash
        //---------------------------------------------------------

        private string GenerateCertificateHash(
            string certificateNo,
            string trainingID,
            string empID,
            string verificationCode)
        {
            string raw =
                certificateNo
                +
                "|"
                +
                trainingID
                +
                "|"
                +
                empID
                +
                "|"
                +
                verificationCode;


            using
            (
                SHA256 sha =
                    SHA256.Create()
            )
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(
                        raw);


                byte[] hash =
                    sha.ComputeHash(
                        bytes);


                StringBuilder sb =
                    new StringBuilder();


                foreach
                (
                    byte b
                    in
                    hash
                )
                {
                    sb.Append(
                        b.ToString("x2"));
                }


                return
                    sb.ToString();
            }
        }


        //---------------------------------------------------------
        // Valid
        //---------------------------------------------------------

        private void ShowValid(
            DataRow dr)
        {
            lblVerificationStatus.Text =
                "✓ Certificate Verified";


            lblVerificationStatus.CssClass =
                "valid-status";


            lblCertificateNo.Text =
                dr["CertificateNo"].ToString();


            lblEmpName.Text =
                dr["EmpName"].ToString();


            lblTrainingID.Text =
                dr["TrainingID"].ToString();


            //-----------------------------------------------------
            // Course
            //-----------------------------------------------------

            string courseTitle =
                dr["TestTitle"].ToString();


            if
            (
                String.IsNullOrWhiteSpace(
                    courseTitle)
            )
            {
                courseTitle =
                    dr["CourseID"].ToString();
            }


            lblCourseTitle.Text =
                courseTitle;


            //-----------------------------------------------------
            // Training Period
            //-----------------------------------------------------

            string fromDate =
                FormatDate(
                    dr["DateFrom"]);


            string toDate =
                FormatDate(
                    dr["DateTo"]);


            lblTrainingPeriod.Text =
                fromDate
                +
                " to "
                +
                toDate;


            //-----------------------------------------------------
            // Generated
            //-----------------------------------------------------

            if
            (
                dr["GeneratedOn"] != DBNull.Value
            )
            {
                lblGeneratedOn.Text =
                    Convert.ToDateTime(
                        dr["GeneratedOn"])
                    .ToString(
                        "dd-MM-yyyy");
            }


            //-----------------------------------------------------
            // Status
            //-----------------------------------------------------

            lblStatus.Text =
                "Valid";


            //-----------------------------------------------------
            // PDF
            //-----------------------------------------------------

            string pdfPath =
                dr["PDFPath"].ToString();


            if
            (
                !String.IsNullOrWhiteSpace(
                    pdfPath)
            )
            {
                lnkCertificate.NavigateUrl =
                    ResolveUrl(
                        pdfPath);


                lnkCertificate.Visible =
                    true;
            }
            else
            {
                lnkCertificate.Visible =
                    false;
            }


            pnlResult.Visible =
                true;
        }


        //---------------------------------------------------------
        // Invalid
        //---------------------------------------------------------

        private void ShowInvalid(
            string message)
        {
            lblMessage.ForeColor =
                System.Drawing.Color.Red;


            lblMessage.Text =
                message;


            lblVerificationStatus.Text =
                "✕ Invalid Certificate";


            lblVerificationStatus.CssClass =
                "invalid-status";


            pnlResult.Visible =
                true;


            lnkCertificate.Visible =
                false;


            ClearCertificateDetails();
        }


        //---------------------------------------------------------
        // Error
        //---------------------------------------------------------

        private void ShowError(
            string message)
        {
            lblMessage.ForeColor =
                System.Drawing.Color.Red;


            lblMessage.Text =
                message;


            pnlResult.Visible =
                false;
        }


        //---------------------------------------------------------
        // Clear Result
        //---------------------------------------------------------

        private void ClearResult()
        {
            lblMessage.Text =
                "";


            lblVerificationStatus.Text =
                "";


            lblCertificateNo.Text =
                "";


            lblEmpName.Text =
                "";


            lblTrainingID.Text =
                "";


            lblCourseTitle.Text =
                "";


            lblTrainingPeriod.Text =
                "";


            lblGeneratedOn.Text =
                "";


            lblStatus.Text =
                "";


            lnkCertificate.NavigateUrl =
                "";


            lnkCertificate.Visible =
                false;


            pnlResult.Visible =
                false;
        }


        //---------------------------------------------------------
        // Clear Details
        //---------------------------------------------------------

        private void ClearCertificateDetails()
        {
            lblCertificateNo.Text =
                "";


            lblEmpName.Text =
                "";


            lblTrainingID.Text =
                "";


            lblCourseTitle.Text =
                "";


            lblTrainingPeriod.Text =
                "";


            lblGeneratedOn.Text =
                "";


            lblStatus.Text =
                "";
        }


        //---------------------------------------------------------
        // Format Date
        //---------------------------------------------------------

        private string FormatDate(
            object value)
        {
            if
            (
                value == null
                ||
                value == DBNull.Value
            )
            {
                return "";
            }


            DateTime date;


            if
            (
                DateTime.TryParse(
                    value.ToString(),
                    out date)
            )
            {
                return
                    date.ToString(
                        "dd-MM-yyyy");
            }


            return
                value.ToString();
        }
    }
}