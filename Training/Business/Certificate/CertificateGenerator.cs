using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Security.Cryptography;
using System.Text;

namespace Training.Business.Certificate
{
    public class CertificateGenerator
    {
        clsDataAccess objDB =
            new clsDataAccess();
        private BaseFont _baseFont;

        public string LastError
        {
            get;
            private set;
        }
        //-------------------------------------------------------
        // Generate Certificate
        //-------------------------------------------------------

        //-------------------------------------------------------
        // Generate Certificate
        //-------------------------------------------------------

        public bool GenerateCertificate(
         string trainingID,
         string empID)
        {
            LastError =
                "";

            if
            (
                String.IsNullOrWhiteSpace(
                    trainingID)
                ||
                String.IsNullOrWhiteSpace(
                    empID)
            )
            {
                LastError =
                    "TrainingID or EmpID is blank.";

                return false;
            }

            if
            (
                IsAlreadyGenerated(
                    trainingID,
                    empID)
            )
            {
                LastError =
                    "Certificate is already generated.";

                return true;
            }

            if
            (
                !IsEligibleForCertificate(
                    trainingID,
                    empID)
            )
            {
                LastError =
                    "Certificate eligibility check failed.";

                return false;
            }

            string templateID =
    GetTrainingTemplateID(
        trainingID);

            if
            (
                String.IsNullOrWhiteSpace(
                    templateID)
            )
            {
                LastError =
                    "Certificate template is not configured for this training.";

                return false;
            }


            DataRow dr =
    LoadCertificateData(
        trainingID,
        empID,
        templateID);




            if
            (
                dr
                ==
                null
            )
            {
                LastError =
                    "LoadCertificateData returned no record.";

                return false;
            }

            string certificateID =
                GenerateCertificateID();

            string certificateNo =
                GenerateCertificateNumber();

            string pdfName =
                GeneratePDFName(
                    trainingID,
                    empID);

            string verificationCode =
                GenerateVerificationCode();

            bool result =
                CreatePDF(
                    dr,
                    certificateID,
                    certificateNo,
                    pdfName,
                    verificationCode);

            if
            (
                !result
            )
            {
                LastError =
                    "CreatePDF returned false.";

                return false;
            }

            return true;
        }

        //-------------------------------------------------------
        // Check Certificate Eligibility
        //-------------------------------------------------------

        private bool IsEligibleForCertificate(
         string trainingID,
         string empID)
        {
            /*
             * ------------------------------------------------------
             * 1. Training Progress
             * ------------------------------------------------------
             */

            string progressQuery =
                "SELECT " +
                "BatchFeedbackCompleted," +
                "CertificateGenerated " +
                "FROM TrainingProgress " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID";

            SqlParameter[] progressParam =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@EmpID",
            empID)
    };

            DataTable dtProgress =
                objDB.GetDataTable(
                    progressQuery,
                    progressParam);

            if
            (
                dtProgress.Rows.Count
                ==
                0
            )
            {
                return false;
            }

            bool batchFeedbackCompleted =
                Convert.ToBoolean(
                    dtProgress.Rows[0]
                    ["BatchFeedbackCompleted"]);

            bool certificateGenerated =
                Convert.ToBoolean(
                    dtProgress.Rows[0]
                    ["CertificateGenerated"]);

            if
            (
                !batchFeedbackCompleted
            )
            {
                return false;
            }

            if
            (
                certificateGenerated
            )
            {
                return false;
            }


            /*
             * ------------------------------------------------------
             * 2. Session Attendance Completion
             * ------------------------------------------------------
             */

            string sessionQuery =
                "SELECT " +
                "COUNT(*) AS TotalSessions," +
                "SUM(" +
                "CASE " +
                "WHEN AttendanceStatus='Completed' " +
                "THEN 1 " +
                "ELSE 0 " +
                "END" +
                ") AS CompletedSessions " +
                "FROM SessionMaster " +
                "WHERE TrainingID=@TrainingID";

            SqlParameter[] sessionParam =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID)
    };

            DataTable dtSession =
                objDB.GetDataTable(
                    sessionQuery,
                    sessionParam);

            if
            (
                dtSession.Rows.Count
                ==
                0
            )
            {
                return false;
            }

            int totalSessions =
                Convert.ToInt32(
                    dtSession.Rows[0]
                    ["TotalSessions"]);

            int completedSessions =
                dtSession.Rows[0]
                ["CompletedSessions"] == DBNull.Value
                ?
                0
                :
                Convert.ToInt32(
                    dtSession.Rows[0]
                    ["CompletedSessions"]);

            if
            (
                totalSessions
                ==
                0
            )
            {
                return false;
            }

            if
            (
                totalSessions
                !=
                completedSessions
            )
            {
                return false;
            }


            /*
             * ------------------------------------------------------
             * 3. Published Test Completion
             * ------------------------------------------------------
             */

            string testQuery =
                "SELECT " +
                "COUNT(*) AS PublishedTests," +
                "COUNT(TA.TestID) AS CompletedTests " +

                "FROM TestMaster TM " +

                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +

                "LEFT JOIN " +
                "(" +
                "SELECT DISTINCT TestID " +
                "FROM TestAttempt " +
                "WHERE EmpID=@EmpID " +
                "AND Submitted=1" +
                ") TA " +
                "ON TA.TestID=TM.TestID " +

                "WHERE SM.TrainingID=@TrainingID " +
                "AND TM.IsPublished=1";

            SqlParameter[] testParam =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@EmpID",
            empID)
    };

            DataTable dtTest =
                objDB.GetDataTable(
                    testQuery,
                    testParam);

            if
            (
                dtTest.Rows.Count
                ==
                0
            )
            {
                return false;
            }

            int publishedTests =
                Convert.ToInt32(
                    dtTest.Rows[0]
                    ["PublishedTests"]);

            int completedTests =
                Convert.ToInt32(
                    dtTest.Rows[0]
                    ["CompletedTests"]);

            if
            (
                publishedTests
                ==
                0
            )
            {
                return false;
            }

            if
            (
                publishedTests
                !=
                completedTests
            )
            {
                return false;
            }


            /*
             * ------------------------------------------------------
             * 4. Actual Batch Feedback
             * ------------------------------------------------------
             */

            string feedbackQuery =
                "SELECT COUNT(*) " +
                "FROM Feedback " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID " +
                "AND Submitted=1";

            SqlParameter[] feedbackParam =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@EmpID",
            empID)
    };

            object feedbackResult =
                objDB.ExecuteScalar(
                    feedbackQuery,
                    feedbackParam);

            if
            (
                feedbackResult
                ==
                null
            )
            {
                return false;
            }

            int feedbackCount =
                Convert.ToInt32(
                    feedbackResult);

            if
            (
                feedbackCount
                ==
                0
            )
            {
                return false;
            }


            /*
             * ------------------------------------------------------
             * All Conditions Completed
             * ------------------------------------------------------
             */

            return true;
        }

        //-------------------------------------------------------
        // Already Generated
        //-------------------------------------------------------

        private bool IsAlreadyGenerated(
            string trainingID,
            string empID)
        {
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

            return
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param))
                >
                0;
        }

        //-------------------------------------------------------
        // Generate Certificate ID
        //-------------------------------------------------------

        //-------------------------------------------------------
        // Generate Certificate ID
        //-------------------------------------------------------

        private string GenerateCertificateID()
        {
            string timeStamp =
                DateTime.Now
                .ToString("yyyyMMddHHmmssfff");

            string randomPart =
                Guid.NewGuid()
                .ToString("N")
                .Substring(
                0,
                6)
                .ToUpper();

            return
                "CID"
                +
                timeStamp
                +
                randomPart;
        }

        //-------------------------------------------------------
        // Generate Certificate Number
        //-------------------------------------------------------

        //-------------------------------------------------------
        // Generate Certificate Number
        //-------------------------------------------------------

        private string GenerateCertificateNumber()
        {
            string timeStamp =
                DateTime.Now
                .ToString("yyyyMMddHHmmssfff");

            string randomPart =
                Guid.NewGuid()
                .ToString("N")
                .Substring(
                0,
                8)
                .ToUpper();

            return
                "CERT"
                +
                timeStamp
                +
                randomPart;
        }

        //-------------------------------------------------------
        // Generate PDF Name
        //-------------------------------------------------------

        private string GeneratePDFName(
            string trainingID,
            string empID)
        {
            return
                empID
                +
                "_"
                +
                trainingID
                +
                "_"
                +
                DateTime.Now
                .ToString("yyyyMMddHHmmss")
                +
                ".pdf";
        }

        //-------------------------------------------------------
        // Load Certificate Data
        //-------------------------------------------------------

        private DataRow LoadCertificateData(
    string trainingID,
    string empID,
    string templateID)
        {
            string query =
        @"
SELECT
TCT.TrainingID,
@EmpID AS EmpID,
TCT.TemplateID,
TCT.CourseTitle,
TCT.LeftSignature,
TCT.LeftName,
TCT.LeftDesignation,
TCT.RightSignature,
TCT.RightName,
TCT.RightDesignation,
CTM.TemplateName,
CTM.HeaderText,
CTM.FooterText,
CTM.BackgroundImage,
CTM.LogoImage,
CTM.HeaderFontSize,
CTM.FooterFontSize,
CTM.CourseTitleFontSize,
CTM.BodyFontSize,
CTM.Orientation,
CTM.PaperSize,
CTM.LogoX,
CTM.LogoY,
CTM.HeaderY,
CTM.TitleY,
CTM.BodyY,
CTM.LeftSignatureX,
CTM.RightSignatureX,
CTM.SignatureY,
CTM.FooterY,
TD.DateFrom,
TD.DateTo,
CM.CourseName,
ISNULL(
EBM.EmpName,
TME.TraineeName
)
AS
EmpName
FROM
TrainingCertificateTemplate TCT
INNER JOIN
CertificateTemplateMaster CTM
ON
TCT.TemplateID=CTM.TemplateID
INNER JOIN
TrainingDetails TD
ON
TCT.TrainingID=TD.TrainingID
INNER JOIN
CourseMaster CM
ON
TD.CourseID=CM.CourseID
LEFT JOIN
EmpBasicMaster EBM
ON
EBM.EmpID=@EmpID
LEFT JOIN
TraineeMasterExternal TME
ON
TME.EmpIDExternal=@EmpID
WHERE
TCT.TrainingID=@TrainingID
AND
TCT.TemplateID=@TemplateID
AND
TCT.Active=1
AND
CTM.Active=1
";

            SqlParameter[] param =
            {
            new SqlParameter(
    "@TrainingID",
    trainingID),

new SqlParameter(
    "@EmpID",
    empID),

new SqlParameter(
    "@TemplateID",
    templateID)
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

        //-------------------------------------------------------
        // Part-2
        //-------------------------------------------------------


        private bool CreatePDF(
        DataRow dr,
        string certificateID,
        string certificateNo,
        string pdfName,
        string verificationCode)
        {
            string pdfPath =
                GetPDFPath(
                pdfName);

            Document document =
                null;

            PdfWriter writer =
                null;

            try
            {
                Rectangle page =
        GetPageSize(
        dr["PaperSize"]
        .ToString(),
        dr["Orientation"]
        .ToString());

                document =
                    new Document(
                    page,
                    20,
                    20,
                    20,
                    20);

                writer =
                    PdfWriter.GetInstance(
                    document,
                    new FileStream(
                    pdfPath,
                    FileMode.Create));

                document.Open();

                DrawBackground(
        writer,
        document,
        dr);

                DrawLogo(
                    writer,
                    document,
                    dr);

                DrawHeader(
                   writer,
                    document,
                    dr);

                DrawBody(
                    writer,
                    document,
                    dr);

                DrawSignature(
                    writer,
                    document,
                    dr);

                DrawVerificationBlock(
        writer,
        document,
        certificateNo,
        verificationCode);

                DrawFooter(
                    writer,
                    document,
                    dr);

                document.Close();

                SaveCertificate(
         certificateID,
         certificateNo,
         dr,
         pdfName,
         verificationCode);

                return true;
            }
            catch
            {
                if
                (
                    document != null
                    &&
                    document.IsOpen()
                )
                {
                    document.Close();
                }

                if
                (
                    File.Exists(
                    pdfPath)
                )
                {
                    File.Delete(
                        pdfPath);
                }

                throw;
            }
        }

        //-------------------------------------------------------
        // Draw Verification Block
        //-------------------------------------------------------

        private void DrawVerificationBlock(
            PdfWriter writer,
            Document document,
            string certificateNo,
            string verificationCode)
        {
            PdfContentByte canvas =
                writer.DirectContent;

            BaseFont baseFont =
                GetBaseFont();

            Font smallFont =
                new Font(
                    baseFont,
                    8,
                    Font.NORMAL,
                    BaseColor.BLACK);

            Font boldFont =
                new Font(
                    baseFont,
                    8,
                    Font.BOLD,
                    BaseColor.BLACK);

            float leftX =
                40f;

            float bottomY =
                55f;

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_LEFT,
                new Phrase(
                    "Certificate No: "
                    +
                    certificateNo,
                    boldFont),
                leftX,
                bottomY + 28f,
                0);

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_LEFT,
                new Phrase(
                    "Verification Code: "
                    +
                    verificationCode,
                    smallFont),
                leftX,
                bottomY + 14f,
                0);

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_LEFT,
                new Phrase(
                    "Scan QR code to verify this certificate",
                    smallFont),
                leftX,
                bottomY,
                0);

            DrawVerificationQRCode(
                writer,
                document,
                certificateNo,
                verificationCode);
        }

        //-------------------------------------------------------
        // Draw Verification QR Code
        //-------------------------------------------------------

        private void DrawVerificationQRCode(
            PdfWriter writer,
            Document document,
            string certificateNo,
            string verificationCode)
        {
            string verificationURL =
                BuildVerificationURL(
                    certificateNo,
                    verificationCode);

            BarcodeQRCode qrCode =
                new BarcodeQRCode(
                    verificationURL,
                    150,
                    150,
                    null);

            Image qrImage =
                qrCode.GetImage();

            qrImage.ScaleAbsolute(
                65f,
                65f);

            float qrX =
                document.PageSize.Width
                -
                105f;

            float qrY =
                45f;

            qrImage.SetAbsolutePosition(
                qrX,
                qrY);

            writer.DirectContent.AddImage(
                qrImage);
        }

        //-------------------------------------------------------
        // Build Verification URL
        //-------------------------------------------------------

        private string BuildVerificationURL(
         string certificateNo,
         string verificationCode)
        {
            string baseURL =
                ConfigurationManager
                .AppSettings[
                    "CertificateVerificationBaseUrl"];

            if
            (
                String.IsNullOrWhiteSpace(
                    baseURL)
            )
            {
                throw new Exception(
                    "Certificate verification base URL is not configured.");
            }

            baseURL =
                baseURL.TrimEnd('/');

            string verificationURL =
                baseURL
                +
                "/VerifyCertificate.aspx"
                +
                "?CertificateNo="
                +
                HttpUtility.UrlEncode(
                    certificateNo)
                +
                "&Code="
                +
                HttpUtility.UrlEncode(
                    verificationCode);

            return
                verificationURL;
        }

        private string GetPDFPath(
        string pdfName)
        {
            return
                Path.Combine(
                GetCertificateFolder(),
                pdfName);
        }

        private string GetCertificateFolder()
        {
            string folder =
                HttpContext.Current.Server.MapPath(
                "~/Uploads/Certificates/");

            if
            (
                !Directory.Exists(
                folder)
            )
            {
                Directory.CreateDirectory(
                folder);
            }

            return folder;
        }

        //-------------------------------------------------------
        // Draw Background
        //-------------------------------------------------------

        private void DrawBackground(
            PdfWriter writer,
            Document document,
            DataRow dr)
        {
            string background =
                dr["BackgroundImage"]
                .ToString();

            if
            (
                String.IsNullOrWhiteSpace(
                background)
            )
            {
                return;
            }

            string filePath =
                HttpContext.Current.Server.MapPath(
                background);

            if
            (
                !File.Exists(
                filePath)
            )
            {
                return;
            }

            Image image =
                Image.GetInstance(
                filePath);

            image.SetAbsolutePosition(
                0,
                0);

            image.ScaleAbsolute(
                document.PageSize.Width,
                document.PageSize.Height);

            writer.DirectContentUnder.AddImage(
                image);
        }

        //-------------------------------------------------------
        // Draw Logo
        //-------------------------------------------------------

        private void DrawLogo(
            PdfWriter writer,
            Document document,
            DataRow dr)
        {
            string logo =
                dr["LogoImage"]
                .ToString();

            if
            (
                String.IsNullOrWhiteSpace(
                logo)
            )
            {
                return;
            }

            string filePath =
                HttpContext.Current.Server.MapPath(
                logo);

            if
            (
                !File.Exists(
                filePath)
            )
            {
                return;
            }

            Image image =
                Image.GetInstance(
                filePath);

            image.ScaleToFit(
                80f,
                80f);

            image.SetAbsolutePosition(
                Convert.ToSingle(
                dr["LogoX"]),
                Convert.ToSingle(
                dr["LogoY"]));

            writer.DirectContent.AddImage(
                image);
        }

        //-------------------------------------------------------
        // Draw Header
        //-------------------------------------------------------

        private void DrawHeader(
            PdfWriter writer,
            Document document,
            DataRow dr)
        {
            string header =
                dr["HeaderText"]
                .ToString();

            if
            (
                String.IsNullOrWhiteSpace(
                header)
            )
            {
                return;
            }

            PdfContentByte canvas =
                writer.DirectContent;

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_CENTER,
                new Phrase(
                    header,
                    GetHeaderFont(
                    dr)),
                document.PageSize.Width
                /
                2,
                Convert.ToSingle(
                dr["HeaderY"]),
                0);
        }

        //-------------------------------------------------------
        // Draw Body
        //-------------------------------------------------------

        private void DrawBody(
            PdfWriter writer,
            Document document,
            DataRow dr)
        {
            Font titleFont =
                GetTitleFont(
                dr);

            Font bodyFont =
                GetBodyFont(
                dr);

            Font nameFont =
                new Font(
                GetBaseFont(),
                28,
                Font.BOLD,
                BaseColor.BLACK);

            PdfPTable table =
                new PdfPTable(1);

            table.TotalWidth =
                document.PageSize.Width
                -
                120;

            table.LockedWidth =
                true;

            table.HorizontalAlignment =
                Element.ALIGN_CENTER;

            PdfPCell cell =
                new PdfPCell();

            cell.Border =
                Rectangle.NO_BORDER;

            cell.HorizontalAlignment =
                Element.ALIGN_CENTER;

            cell.Padding =
                5;

            cell.AddElement(
                new Paragraph(
                "CERTIFICATE OF COMPLETION",
                titleFont)
                {
                    Alignment =
                        Element.ALIGN_CENTER
                });

            cell.AddElement(
                new Paragraph(
                "\nThis Certificate is proudly presented to\n",
                bodyFont)
                {
                    Alignment =
                        Element.ALIGN_CENTER
                });

            cell.AddElement(
                new Paragraph(
                dr["EmpName"]
                .ToString(),
                nameFont)
                {
                    Alignment =
                        Element.ALIGN_CENTER
                });

            cell.AddElement(
                new Paragraph(
                "\nFor Successfully Completing\n",
                bodyFont)
                {
                    Alignment =
                        Element.ALIGN_CENTER
                });

            cell.AddElement(
                new Paragraph(
                dr["CourseTitle"]
                .ToString(),
                titleFont)
                {
                    Alignment =
                        Element.ALIGN_CENTER
                });

            cell.AddElement(
                new Paragraph(
                "\nDuration : "
                +
                Convert.ToDateTime(
                dr["DateFrom"])
                .ToString("dd MMM yyyy")
                +
                "  To  "
                +
                Convert.ToDateTime(
                dr["DateTo"])
                .ToString("dd MMM yyyy"),
                bodyFont)
                {
                    Alignment =
                        Element.ALIGN_CENTER
                });

            table.AddCell(
                cell);

            table.WriteSelectedRows(
                0,
                -1,
                60,
                Convert.ToSingle(
                dr["BodyY"]),
                writer.DirectContent);
        }

        //-------------------------------------------------------
        // Draw Signature
        //-------------------------------------------------------

        //-------------------------------------------------------
        // Draw Signature
        //-------------------------------------------------------

        private void DrawSignature(
            PdfWriter writer,
            Document document,
            DataRow dr)
        {
            DrawSingleSignature(
                writer,
                dr,
                dr["LeftSignature"].ToString(),
                dr["LeftName"].ToString(),
                dr["LeftDesignation"].ToString(),
                Convert.ToSingle(
                dr["LeftSignatureX"]),
                Convert.ToSingle(
                dr["SignatureY"]));

            DrawSingleSignature(
                writer,
                dr,
                dr["RightSignature"].ToString(),
                dr["RightName"].ToString(),
                dr["RightDesignation"].ToString(),
                Convert.ToSingle(
                dr["RightSignatureX"]),
                Convert.ToSingle(
                dr["SignatureY"]));
        }

        //-------------------------------------------------------
        // Draw Single Signature
        //-------------------------------------------------------

        //-------------------------------------------------------
        // Draw Single Signature
        //-------------------------------------------------------

        private void DrawSingleSignature(
            PdfWriter writer,
            DataRow dr,
            string imagePath,
            string name,
            string designation,
            float x,
            float y)
        {
            PdfContentByte canvas =
                writer.DirectContent;

            if
            (
                !String.IsNullOrWhiteSpace(
                imagePath)
            )
            {
                string filePath =
                    HttpContext.Current.Server.MapPath(
                    imagePath);

                if
                (
                    File.Exists(
                    filePath)
                )
                {
                    Image img =
                        Image.GetInstance(
                        filePath);

                    img.ScaleToFit(
                        120f,
                        50f);

                    img.SetAbsolutePosition(
                        x,
                        y);

                    canvas.AddImage(
                        img);
                }
            }

            Font nameFont =
                new Font(
                GetBaseFont(),
                Convert.ToSingle(
                dr["BodyFontSize"]),
                Font.BOLD,
                BaseColor.BLACK);

            Font designationFont =
                GetFooterFont(
                dr);

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_CENTER,
                new Phrase(
                    name,
                    nameFont),
                x + 60f,
                y - 15f,
                0);

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_CENTER,
                new Phrase(
                    designation,
                    designationFont),
                x + 60f,
                y - 32f,
                0);
        }

        //-------------------------------------------------------
        // Draw Footer
        //-------------------------------------------------------

        private void DrawFooter(
            PdfWriter writer,
            Document document,
            DataRow dr)
        {
            PdfContentByte canvas =
                writer.DirectContent;

            ColumnText.ShowTextAligned(
                canvas,
                Element.ALIGN_CENTER,
                new Phrase(
                    dr["FooterText"]
                    .ToString(),
                    GetFooterFont(
                    dr)),
                document.PageSize.Width
                /
                2,
                Convert.ToSingle(
                dr["FooterY"]),
                0);
        }

        //-------------------------------------------------------
        // Save Certificate
        //-------------------------------------------------------

        private void SaveCertificate(
        string certificateID,
        string certificateNo,
        DataRow dr,
        string pdfName,
        string verificationCode)
        {
            string trainingID =
                dr["TrainingID"]
                .ToString();

            string empID =
                dr["EmpID"]
                .ToString();

            string templateID =
                dr["TemplateID"]
                .ToString();

            string relativePDFPath =
                "~/Uploads/Certificates/"
                +
                pdfName;

            //string verificationCode =
            //    GenerateVerificationCode();

            string certificateHash =
                GenerateCertificateHash(
                certificateNo,
                trainingID,
                empID,
                verificationCode);

            string query =
        @"
INSERT INTO TrainingCertificate
(
CertificateID,
CertificateNo,
TrainingID,
EmpID,
TemplateID,
PDFPath,
PDFName,
GeneratedOn,
GeneratedBy,
CertificateStatus,
CertificateHash,
VerificationCode,
Remarks
)
VALUES
(
@CertificateID,
@CertificateNo,
@TrainingID,
@EmpID,
@TemplateID,
@PDFPath,
@PDFName,
GETDATE(),
@GeneratedBy,
@CertificateStatus,
@CertificateHash,
@VerificationCode,
@Remarks
)
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CertificateID",
            certificateID),

        new SqlParameter(
            "@CertificateNo",
            certificateNo),

        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@EmpID",
            empID),

        new SqlParameter(
            "@TemplateID",
            templateID),

        new SqlParameter(
            "@PDFPath",
            relativePDFPath),

        new SqlParameter(
            "@PDFName",
            pdfName),

        new SqlParameter(
            "@GeneratedBy",
            empID),

        new SqlParameter(
            "@CertificateStatus",
            "A"),

        new SqlParameter(
            "@CertificateHash",
            certificateHash),

        new SqlParameter(
            "@VerificationCode",
            verificationCode),

        new SqlParameter(
            "@Remarks",
            DBNull.Value)
    };

            int result =
        objDB.ExecuteSql(
            query,
            param);

            if
            (
                result
                <=
                0
            )
            {
                throw new Exception(
                    "Certificate record could not be saved.");
            }

            UpdateTrainingProgress(
                trainingID,
                empID);
        }

        //-------------------------------------------------------
        // Update Training Progress
        //-------------------------------------------------------

        private void UpdateTrainingProgress(
            string trainingID,
            string empID)
        {
            string query =
        @"
UPDATE TrainingProgress
SET
CertificateGenerated=1,
CertificateGeneratedOn=GETDATE(),
UpdatedOn=GETDATE(),
UpdatedBy=@UpdatedBy
WHERE
TrainingID=@TrainingID
AND
EmpID=@EmpID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@UpdatedBy",
            empID),

        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@EmpID",
            empID)
    };

            int result =
         objDB.ExecuteSql(
             query,
             param);

            if
            (
                result
                <=
                0
            )
            {
                throw new Exception(
                    "Training progress could not be updated after certificate generation.");
            }
        }
        //-------------------------------------------------------
        // Generate Verification Code
        //-------------------------------------------------------

        private string GenerateVerificationCode()
        {
            return
                Guid.NewGuid()
                .ToString("N")
                .Substring(
                    0,
                    12)
                .ToUpper();
        }

        //-------------------------------------------------------
        // Generate Certificate Hash
        //-------------------------------------------------------

        private string GenerateCertificateHash(
            string certificateNo,
            string trainingID,
            string empID,
            string verificationCode)
        {
            string value =
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
                SHA256 sha256 =
                SHA256.Create()
            )
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(
                    value);

                byte[] hash =
                    sha256.ComputeHash(
                    bytes);

                StringBuilder result =
                    new StringBuilder();

                foreach
                (
                    byte item
                    in
                    hash
                )
                {
                    result.Append(
                        item.ToString("x2"));
                }

                return
                    result
                    .ToString();
            }
        }
        //-------------------------------------------------------
        // Get Page Size
        //-------------------------------------------------------

        private Rectangle GetPageSize(
            string paperSize,
            string orientation)
        {
            Rectangle page;

            switch
            (
                paperSize
                .ToUpper()
            )
            {
                case "A3":

                    page =
                        PageSize.A3;

                    break;

                case "LETTER":

                    page =
                        PageSize.LETTER;

                    break;

                case "LEGAL":

                    page =
                        PageSize.LEGAL;

                    break;

                default:

                    page =
                        PageSize.A4;

                    break;
            }

            if
            (
                orientation
                .Equals(
                "Landscape",
                StringComparison.OrdinalIgnoreCase)
            )
            {
                page =
                    page.Rotate();
            }

            return page;
        }
        //-------------------------------------------------------
        // Get Base Font
        //-------------------------------------------------------

        private BaseFont GetBaseFont()
        {
            if
            (
                _baseFont
                ==
                null
            )
            {
                string fontPath =
                    Environment.GetFolderPath(
                    Environment.SpecialFolder.Fonts)
                    +
                    "\\arial.ttf";

                _baseFont =
                    BaseFont.CreateFont(
                    fontPath,
                    BaseFont.IDENTITY_H,
                    BaseFont.EMBEDDED);
            }

            return
                _baseFont;
        }
        //-------------------------------------------------------
        // Header Font
        //-------------------------------------------------------

        private Font GetHeaderFont(
            DataRow dr)
        {
            return
                new Font(
                GetBaseFont(),
                Convert.ToSingle(
                dr["HeaderFontSize"]),
                Font.BOLD,
                BaseColor.BLACK);
        }
        //-------------------------------------------------------
        // Title Font
        //-------------------------------------------------------

        private Font GetTitleFont(
            DataRow dr)
        {
            return
                new Font(
                GetBaseFont(),
                Convert.ToSingle(
                dr["CourseTitleFontSize"]),
                Font.BOLD,
                BaseColor.BLACK);
        }
        //-------------------------------------------------------
        // Body Font
        //-------------------------------------------------------

        private Font GetBodyFont(
            DataRow dr)
        {
            return
                new Font(
                GetBaseFont(),
                Convert.ToSingle(
                dr["BodyFontSize"]),
                Font.NORMAL,
                BaseColor.BLACK);
        }
        //-------------------------------------------------------
        // Footer Font
        //-------------------------------------------------------

        private Font GetFooterFont(
            DataRow dr)
        {
            return
                new Font(
                GetBaseFont(),
                Convert.ToSingle(
                dr["FooterFontSize"]),
                Font.NORMAL,
                BaseColor.BLACK);
        }
        private string GetTrainingTemplateID(
    string trainingID)
        {
            string sql =

                "SELECT TOP 1 " +

                "TemplateID " +

                "FROM TrainingCertificateTemplate " +

                "WHERE TrainingID=@TrainingID " +

                "AND Active=1 " +

                "ORDER BY " +

                "DefaultConfiguration DESC," +

                "CreatedOn DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID)
    };

            object result =
                objDB.ExecuteScalar(
                    sql,
                    param);

            if
            (
                result == null
                ||
                result == DBNull.Value
            )
            {
                return "";
            }

            return result.ToString();
        }
    }
}