using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Training.Business.Certificate;

namespace Training.Trainee
{
    public partial class MyCertificate :
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
            if
           (
                 Session["EmpID"] == null

           )
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if
            (
                 Session["TrainingID"] == null
                ||
                Session["SessionID"] == null
            )
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            if
            (
                !IsPostBack
            )
            {
                TryGeneratePendingCertificate();

                BindCertificate();

                Session.Remove(
                    "CertificateFromTraining");
            }
        }

        //-----------------------------------------------------
        // Bind Certificate
        //-----------------------------------------------------

        private void BindCertificate()
        {
            string query =
                "SELECT " +
                "TC.CertificateID," +
                "TC.CertificateNo," +
                "TC.TrainingID," +
                "TC.EmpID," +
                "TC.TemplateID," +
                "TC.PDFPath," +
                "TC.PDFName," +
                "TC.GeneratedOn," +
                "TC.CertificateStatus," +
                "ISNULL(TCT.CourseTitle,CM.CourseName) AS CourseTitle," +
                "CONVERT(VARCHAR(10),TD.DateFrom,105) " +
                "+ ' to ' + " +
                "CONVERT(VARCHAR(10),TD.DateTo,105) " +
                "AS TrainingDuration " +

                "FROM TrainingCertificate TC " +

                "INNER JOIN TrainingDetails TD " +
                "ON TD.TrainingID=TC.TrainingID " +

                "LEFT JOIN TrainingCertificateTemplate TCT " +
                "ON TCT.TrainingID=TC.TrainingID " +
                "AND TCT.TemplateID=TC.TemplateID " +

                "LEFT JOIN CourseMaster CM " +
                "ON CM.CourseID=TD.CourseID " +

                "WHERE TC.EmpID=@EmpID " +
                "AND TC.CertificateStatus='A' ";

            List<SqlParameter> param =
                new List<SqlParameter>();

            param.Add(
                new SqlParameter(
                    "@EmpID",
                    Session["EmpID"]
                    .ToString().ToUpperInvariant()));

            if
            (
                Session["CertificateFromTraining"]
                !=
                null
                &&
                Session["TrainingID"]
                !=
                null
            )
            {
                query +=
                    "AND TC.TrainingID=@TrainingID ";

                param.Add(
                    new SqlParameter(
                        "@TrainingID",
                        Session["TrainingID"]
                        .ToString()));
            }

            query +=
                "ORDER BY TC.GeneratedOn DESC";

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param.ToArray());

            gvCertificate.DataSource =
                dt;

            gvCertificate.DataBind();

            if
            (
                dt.Rows.Count == 0
            )
            {
                lblMessage.Text =
                    "";

                return;
            }

            lblMessage.Text =
                "";
        }

        private void TryGeneratePendingCertificate()
        {
            if
            (
                Session["EmpID"] == null
                ||
                Session["TrainingID"] == null
                ||
                Session["CertificateFromTraining"] == null
            )
            {
                return;
            }

            string trainingID =
                Session["TrainingID"]
                .ToString();

            string empID =
                Session["EmpID"]
                .ToString().ToUpperInvariant();

            string query =
                "SELECT COUNT(*) " +
                "FROM TrainingCertificate " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID " +
                "AND CertificateStatus='A'";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@EmpID",
            empID)
    };

            object result =
                objDB.ExecuteScalar(
                    query,
                    param);

            int count =
                result == null
                ?
                0
                :
                Convert.ToInt32(
                    result);

            if
            (
                count > 0
            )
            {
                return;
            }

            try
            {
                CertificateGenerator generator =
                    new CertificateGenerator();

                bool generated =
                    generator.GenerateCertificate(
                        trainingID,
                        empID);

                if
 (
     !generated
 )
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Certificate generation failed: "
                        +
                        generator.LastError;
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Certificate generation error: "
                    +
                    ex.Message;
            }
        }

        //-----------------------------------------------------
        // Grid Row Command
        //-----------------------------------------------------

        protected void gvCertificate_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            try
            {
                string certificateID =
                    e.CommandArgument
                    .ToString();

                if
                (
                    String.IsNullOrWhiteSpace(
                    certificateID)
                )
                {
                    ShowError(
                        "Invalid certificate.");

                    return;
                }

                if
                (
                    e.CommandName
                    ==
                    "ViewCertificate"
                )
                {
                    ViewCertificate(
                        certificateID);
                }
                else if
                (
                    e.CommandName
                    ==
                    "DownloadCertificate"
                )
                {
                    DownloadCertificate(
                        certificateID);
                }
            }
            catch (Exception ex)
            {
                ShowError(
                    "Unable to process certificate. "
                    +
                    ex.Message);
            }
        }

        //-----------------------------------------------------
        // Get Certificate
        //-----------------------------------------------------

        private DataRow GetCertificate(
            string certificateID)
        {
            string query =
@"
SELECT
CertificateID,
CertificateNo,
TrainingID,
EmpID,
PDFPath,
PDFName,
GeneratedOn,
CertificateStatus
FROM
TrainingCertificate
WHERE
CertificateID=@CertificateID
AND
EmpID=@EmpID
AND
CertificateStatus='A'
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@CertificateID",
                    certificateID),

                new SqlParameter(
                    "@EmpID",
                    Session["EmpID"]
                    .ToString().ToUpperInvariant())
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
                return null;
            }

            return
                dt.Rows[0];
        }

        //-----------------------------------------------------
        // View Certificate
        //-----------------------------------------------------

        private void ViewCertificate(
            string certificateID)
        {
            DataRow dr =
                GetCertificate(
                    certificateID);

            if
            (
                dr
                ==
                null
            )
            {
                ShowError(
                    "Certificate not found.");

                return;
            }

            string pdfPath =
                dr["PDFPath"]
                .ToString();

            if
            (
                String.IsNullOrWhiteSpace(
                pdfPath)
            )
            {
                ShowError(
                    "Certificate PDF is not available.");

                return;
            }

            string physicalPath =
                Server.MapPath(
                    pdfPath);

            if
            (
                !File.Exists(
                physicalPath)
            )
            {
                ShowError(
                    "Certificate PDF file could not be found.");

                return;
            }

            string pdfName =
                GetPDFName(
                    dr,
                    physicalPath);

            SendPDF(
                physicalPath,
                pdfName,
                false);
        }

        //-----------------------------------------------------
        // Download Certificate
        //-----------------------------------------------------

        private void DownloadCertificate(
            string certificateID)
        {
            DataRow dr =
                GetCertificate(
                    certificateID);

            if
            (
                dr
                ==
                null
            )
            {
                ShowError(
                    "Certificate not found.");

                return;
            }

            string pdfPath =
                dr["PDFPath"]
                .ToString();

            if
            (
                String.IsNullOrWhiteSpace(
                pdfPath)
            )
            {
                ShowError(
                    "Certificate PDF is not available.");

                return;
            }

            string physicalPath =
                Server.MapPath(
                    pdfPath);

            if
            (
                !File.Exists(
                physicalPath)
            )
            {
                ShowError(
                    "Certificate PDF file could not be found.");

                return;
            }

            string pdfName =
                GetPDFName(
                    dr,
                    physicalPath);

            UpdateDownloadDetails(
                certificateID);

            SendPDF(
                physicalPath,
                pdfName,
                true);
        }

        //-----------------------------------------------------
        // Get PDF Name
        //-----------------------------------------------------

        private string GetPDFName(
            DataRow dr,
            string physicalPath)
        {
            string pdfName =
                dr["PDFName"]
                .ToString();

            if
            (
                String.IsNullOrWhiteSpace(
                pdfName)
            )
            {
                pdfName =
                    Path.GetFileName(
                    physicalPath);
            }

            return pdfName;
        }

        //-----------------------------------------------------
        // Send PDF
        //-----------------------------------------------------

        private void SendPDF(
            string physicalPath,
            string pdfName,
            bool download)
        {
            Response.Clear();

            Response.ClearHeaders();

            Response.ClearContent();

            Response.ContentType =
                "application/pdf";

            if (download)
            {
                Response.AddHeader(
                    "Content-Disposition",
                    "attachment; filename=\""
                    +
                    pdfName
                    +
                    "\"");
            }
            else
            {
                Response.AddHeader(
                    "Content-Disposition",
                    "inline; filename=\""
                    +
                    pdfName
                    +
                    "\"");
            }

            Response.AddHeader(
                "Content-Length",
                new FileInfo(
                    physicalPath)
                .Length
                .ToString());

            Response.TransmitFile(
                physicalPath);

            Response.Flush();

            HttpContext.Current
                .ApplicationInstance
                .CompleteRequest();
        }

        //-----------------------------------------------------
        // Update Download Details
        //-----------------------------------------------------

        private void UpdateDownloadDetails(
            string certificateID)
        {
            string query =
@"
UPDATE
TrainingCertificate
SET
DownloadedOn=GETDATE(),
DownloadedBy=@DownloadedBy
WHERE
CertificateID=@CertificateID
AND
EmpID=@EmpID
AND
CertificateStatus='A'
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@DownloadedBy",
                    Session["EmpID"]
                    .ToString().ToUpperInvariant()),

                new SqlParameter(
                    "@CertificateID",
                    certificateID),

                new SqlParameter(
                    "@EmpID",
                    Session["EmpID"]
                    .ToString().ToUpperInvariant())
            };

            objDB.ExecuteSql(
                query,
                param);
        }

        //-----------------------------------------------------
        // Show Error
        //-----------------------------------------------------

        private void ShowError(
            string message)
        {
            lblMessage.Text =
                message;

            lblMessage.ForeColor =
                System.Drawing.Color.Red;
        }
    }
}