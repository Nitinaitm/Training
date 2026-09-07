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
            string query =
                "SELECT " +
                "TD.AttendanceRequired," +
                "TD.InitialAssessmentRequired," +
                "TD.FinalAssessmentRequired," +
                "TD.FeedbackRequired," +
                "TD.CertificateRequired," +
                "ISNULL(TP.PreExamCompleted,0) AS PreExamCompleted," +
                "ISNULL(TP.PostExamCompleted,0) AS PostExamCompleted," +
                "ISNULL(TP.CertificateGenerated,0) AS CertificateGenerated " +
                "FROM TrainingDetails TD " +
                "INNER JOIN TrainingProgress TP " +
                "ON TP.TrainingID=TD.TrainingID " +
                "AND TP.EmpID=@EmpID " +
                "WHERE TD.TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID", trainingID),
                new SqlParameter("@EmpID", empID)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return false;
            }

            DataRow row = dt.Rows[0];

            if
            (
                !Convert.ToBoolean(row["CertificateRequired"])
                ||
                Convert.ToBoolean(row["CertificateGenerated"])
            )
            {
                return false;
            }

            bool attendanceRequired =
                Convert.ToBoolean(row["AttendanceRequired"]);

            if
            (
                attendanceRequired
            )
            {
                string attendanceQuery =
                    "SELECT COUNT(*) AS TotalSessions," +
                    "SUM(CASE WHEN ISNULL((SELECT TOP 1 SA.AttendanceStatus " +
                    "FROM SessionAttendance SA " +
                    "WHERE SA.SessionID=SM.SessionID " +
                    "AND SA.EmpID=@EmpID),'Pending')='Completed' " +
                    "THEN 1 ELSE 0 END) AS CompletedSessions " +
                    "FROM SessionMaster SM " +
                    "WHERE SM.TrainingID=@TrainingID";

                DataTable attendance =
                    objDB.GetDataTable(
                        attendanceQuery,
                        new SqlParameter[]
                        {
                            new SqlParameter("@TrainingID", trainingID),
                            new SqlParameter("@EmpID", empID)
                        });

                if
                (
                    attendance.Rows.Count == 0
                )
                {
                    return false;
                }

                int total =
                    Convert.ToInt32(
                        attendance.Rows[0]["TotalSessions"]);

                int completed =
                    attendance.Rows[0]["CompletedSessions"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(
                        attendance.Rows[0]["CompletedSessions"]);

                if
                (
                    total == 0
                    ||
                    total != completed
                )
                {
                    return false;
                }
            }

            if
            (
                Convert.ToBoolean(row["InitialAssessmentRequired"])
                &&
                !Convert.ToBoolean(row["PreExamCompleted"])
            )
            {
                return false;
            }

            if
            (
                Convert.ToBoolean(row["FinalAssessmentRequired"])
                &&
                !Convert.ToBoolean(row["PostExamCompleted"])
            )
            {
                return false;
            }

            if
            (
                Convert.ToBoolean(row["FeedbackRequired"])
            )
            {
                object feedbackResult =
                    objDB.ExecuteScalar(
                        "SELECT COUNT(*) FROM Feedback " +
                        "WHERE TrainingID=@TrainingID " +
                        "AND EmpID=@EmpID " +
                        "AND Submitted=1",
                        param);

                if
                (
                    feedbackResult == null
                    ||
                    Convert.ToInt32(feedbackResult) == 0
                )
                {
                    return false;
                }
            }

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
                20f;

            canvas.BeginText();
            canvas.SetFontAndSize(
                baseFont,
                8);
            canvas.SetColorFill(
                BaseColor.BLACK);

            canvas.ShowTextAligned(
                PdfContentByte.ALIGN_LEFT,
                "Certificate No: " + certificateNo,
                leftX,
                bottomY + 10,
                0);

            canvas.ShowTextAligned(
                PdfContentByte.ALIGN_LEFT,
                "Verification Code: " + verificationCode,
                leftX,
                bottomY,
                0);

            canvas.EndText();
        }

        //-------------------------------------------------------
        // Get Base Font
        //-------------------------------------------------------

        private BaseFont GetBaseFont()
        {
            if
            (
                _baseFont
                !=
                null
            )
            {
                return
                    _baseFont;
            }

            string fontPath =
                HttpContext.Current.Server.MapPath(
                    "~/Fonts/ARIAL.TTF");

            if
            (
                File.Exists(
                    fontPath)
            )
            {
                _baseFont =
                    BaseFont.CreateFont(
                        fontPath,
                        BaseFont.IDENTITY_H,
                        BaseFont.EMBEDDED);
            }
            else
            {
                _baseFont =
                    BaseFont.CreateFont(
                        BaseFont.HELVETICA,
                        BaseFont.CP1252,
                        BaseFont.NOT_EMBEDDED);
            }

            return
                _baseFont;
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
                paperSize.ToUpper()
            )
            {
                case "A4":
                    page =
                        PageSize.A4;
                    break;

                case "A3":
                    page =
                        PageSize.A3;
                    break;

                case "LETTER":
                    page =
                        PageSize.LETTER;
                    break;

                default:
                    page =
                        PageSize.A4;
                    break;
            }

            if
            (
                orientation.ToUpper()
                ==
                "LANDSCAPE"
            )
            {
                page =
                    page.Rotate();
            }

            return
                page;
        }

        //-------------------------------------------------------
        // Get PDF Path
        //-------------------------------------------------------

        private string GetPDFPath(
            string pdfName)
        {
            string folder =
                HttpContext.Current.Server.MapPath(
                    "~/Certificates/");

            if
            (
                !Directory.Exists(
                    folder)
            )
            {
                Directory.CreateDirectory(
                    folder);
            }

            return
                Path.Combine(
                    folder,
                    pdfName);
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
            string query =
                "INSERT INTO TrainingCertificate " +
                "(" +
                "CertificateID," +
                "CertificateNo," +
                "TrainingID," +
                "EmpID," +
                "TemplateID," +
                "CertificateFile," +
                "VerificationCode," +
                "CertificateStatus," +
                "GeneratedOn" +
                ") VALUES (" +
                "@CertificateID," +
                "@CertificateNo," +
                "@TrainingID," +
                "@EmpID," +
                "@TemplateID," +
                "@CertificateFile," +
                "@VerificationCode," +
                "'A'," +
                "GETDATE()" +
                ")";

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
                    dr["TrainingID"]),

                new SqlParameter(
                    "@EmpID",
                    dr["EmpID"]),

                new SqlParameter(
                    "@TemplateID",
                    dr["TemplateID"]),

                new SqlParameter(
                    "@CertificateFile",
                    pdfName),

                new SqlParameter(
                    "@VerificationCode",
                    verificationCode)
            };

            objDB.ExecuteNonQuery(
                query,
                param);

            UpdateTrainingProgress(
                dr["TrainingID"]
                .ToString(),
                dr["EmpID"]
                .ToString());
        }

        //-------------------------------------------------------
        // Update Training Progress
        //-------------------------------------------------------

        private void UpdateTrainingProgress(
            string trainingID,
            string empID)
        {
            string query =
                "UPDATE TrainingProgress " +
                "SET CertificateGenerated=1," +
                "CertificateGeneratedOn=GETDATE() " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    trainingID),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };

            objDB.ExecuteNonQuery(
                query,
                param);
        }
    }
}
