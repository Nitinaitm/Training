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
        clsDataAccess objDB = new clsDataAccess();
        private BaseFont _baseFont;
        public string LastError { get; private set; }

        public bool GenerateCertificate(string trainingID, string empID)
        {
            LastError = "";
            if (String.IsNullOrWhiteSpace(trainingID) || String.IsNullOrWhiteSpace(empID)) { LastError = "TrainingID or EmpID is blank."; return false; }
            if (IsAlreadyGenerated(trainingID, empID)) { LastError = "Certificate is already generated."; return true; }
            if (!IsEligibleForCertificate(trainingID, empID)) { LastError = "Certificate eligibility check failed."; return false; }
            string templateID = GetTrainingTemplateID(trainingID);
            if (String.IsNullOrWhiteSpace(templateID)) { LastError = "Certificate template is not configured for this training."; return false; }
            DataRow dr = LoadCertificateData(trainingID, empID, templateID);
            if (dr == null) { LastError = "LoadCertificateData returned no record."; return false; }
            string certificateID = GenerateCertificateID();
            string certificateNo = GenerateCertificateNumber();
            string pdfName = GeneratePDFName(trainingID, empID);
            string verificationCode = GenerateVerificationCode();
            bool result = CreatePDF(dr, certificateID, certificateNo, pdfName, verificationCode);
            if (!result) { LastError = "CreatePDF returned false."; return false; }
            return true;
        }

        private bool IsEligibleForCertificate(string trainingID, string empID)
        {
            string progressQuery = "SELECT CertificateGenerated FROM TrainingProgress WHERE TrainingID=@TrainingID AND EmpID=@EmpID";
            SqlParameter[] progressParam = { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) };
            DataTable dtProgress = objDB.GetDataTable(progressQuery, progressParam);
            if (dtProgress.Rows.Count == 0) return false;
            if (Convert.ToBoolean(dtProgress.Rows[0]["CertificateGenerated"])) return false;

            string requirementQuery = "SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,CertificateRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,ISNULL(CertificateSkipped,0) CertificateSkipped,PreTestCertificateRule,PostTestCertificateRule FROM TrainingDetails WHERE TrainingID=@TrainingID";
            DataTable dtRequirement = objDB.GetDataTable(requirementQuery, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            if (dtRequirement.Rows.Count == 0) return false;

            DataRow requirement = dtRequirement.Rows[0];
            bool attendanceRequired = Convert.ToBoolean(requirement["AttendanceRequired"]);
            bool preRequired = Convert.ToBoolean(requirement["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(requirement["FinalAssessmentRequired"]);
            bool feedbackRequired = Convert.ToBoolean(requirement["FeedbackRequired"]);
            bool feedbackSkipped = Convert.ToBoolean(requirement["FeedbackSkipped"]);
            bool certificateRequired = Convert.ToBoolean(requirement["CertificateRequired"]);
            bool certificateSkipped = Convert.ToBoolean(requirement["CertificateSkipped"]);

            if (!certificateRequired || certificateSkipped) return false;

            object assignment = objDB.ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND AssignmentStatus='Assigned'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
            if (assignment == null || Convert.ToInt32(assignment) == 0) return false;

            if (attendanceRequired && !AreAllRequiredSessionAttendanceCompleted(trainingID)) return false;

            string preRule = requirement["PreTestCertificateRule"] == DBNull.Value ? "" : requirement["PreTestCertificateRule"].ToString().Trim().ToUpperInvariant();
            string postRule = requirement["PostTestCertificateRule"] == DBNull.Value ? "" : requirement["PostTestCertificateRule"].ToString().Trim().ToUpperInvariant();
            bool preApplicable = preRequired && HasUnskippedSessions(trainingID, "PreAssessmentSkipped");
            bool postApplicable = postRequired && HasUnskippedSessions(trainingID, "PostAssessmentSkipped");

            if (preApplicable && (preRule != "ALL" && preRule != "PASS")) return false;
            if (postApplicable && (postRule != "ALL" && postRule != "PASS")) return false;

            if (preApplicable && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Pre", "PreAssessmentSkipped")) return false;
            if (postApplicable && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Post", "PostAssessmentSkipped")) return false;

            if (preApplicable && preRule == "PASS" && !AreAllRequiredSessionTestsPassed(trainingID, empID, "Pre", "PreAssessmentSkipped")) return false;
            if (postApplicable && postRule == "PASS" && !AreAllRequiredSessionTestsPassed(trainingID, empID, "Post", "PostAssessmentSkipped")) return false;

            if (feedbackRequired && !feedbackSkipped)
            {
                object feedbackResult = objDB.ExecuteScalar("SELECT COUNT(*) FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(Submitted,0)=1", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
                if (feedbackResult == null || Convert.ToInt32(feedbackResult) == 0) return false;
            }

            return true;
        }

        private bool HasUnskippedSessions(string trainingID, string skipColumn)
        {
            if (skipColumn != "PreAssessmentSkipped" && skipColumn != "PostAssessmentSkipped") return false;
            object value = objDB.ExecuteScalar("SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND ISNULL(" + skipColumn + ",0)=0", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return value != null && Convert.ToInt32(value) > 0;
        }

        private bool AreAllRequiredSessionAttendanceCompleted(string trainingID)
        {
            object v = objDB.ExecuteScalar(@"SELECT CASE WHEN NOT EXISTS (
                SELECT 1 FROM SessionMaster SM
                WHERE SM.TrainingID=@TrainingID
                  AND ISNULL(SM.AttendanceSkipped,0)=0
                  AND ISNULL(SM.AttendanceStatus,'')<>'Completed'
            ) THEN 1 ELSE 0 END", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            return v != null && Convert.ToInt32(v) == 1;
        }

        private bool AreAllRequiredSessionTestsCompleted(string trainingID, string empID, string testType, string skipColumn)
        {
            string sql = @"SELECT CASE WHEN NOT EXISTS (
                SELECT 1 FROM SessionMaster SM
                WHERE SM.TrainingID=@TrainingID
                  AND ISNULL(SM." + skipColumn + @",0)=0
                  AND (
                      NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1)
                      OR NOT EXISTS (
                          SELECT 1
                          FROM TestMaster TM
                          INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID
                          WHERE TM.SessionID=SM.SessionID
                            AND TM.TestType=@TestType
                            AND TM.IsPublished=1
                            AND TA.EmpID=@EmpID
                            AND TA.Submitted=1
                      )
                  )
            ) THEN 1 ELSE 0 END";
            object v = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID), new SqlParameter("@TestType", testType) });
            return v != null && Convert.ToInt32(v) == 1;
        }

        private bool AreAllRequiredSessionTestsPassed(string trainingID, string empID, string testType, string skipColumn)
        {
            string sql = @"SELECT CASE WHEN NOT EXISTS (
                SELECT 1
                FROM SessionMaster SM
                WHERE SM.TrainingID=@TrainingID
                  AND ISNULL(SM." + skipColumn + @",0)=0
                  AND (
                      NOT EXISTS (
                          SELECT 1
                          FROM TestMaster TM
                          WHERE TM.SessionID=SM.SessionID
                            AND TM.TestType=@TestType
                            AND TM.IsPublished=1
                      )
                      OR NOT EXISTS (
                          SELECT 1
                          FROM TestMaster TM
                          INNER JOIN TestResult TR
                          ON TR.TestID=TM.TestID
                          AND TR.EmpID=@EmpID
                          WHERE TM.SessionID=SM.SessionID
                            AND TM.TestType=@TestType
                            AND TM.IsPublished=1
                            AND TR.IsFinalAttempt=1
                            AND TR.ResultStatus IN ('PASS','PASSED')
                      )
                  )
            ) THEN 1 ELSE 0 END";
            object value = objDB.ExecuteScalar(
                sql,
                new SqlParameter[]
                {
                    new SqlParameter("@TrainingID", trainingID),
                    new SqlParameter("@EmpID", empID),
                    new SqlParameter("@TestType", testType)
                });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private string GetCertificateEligibilityMode(string trainingID)
        {
            object value = objDB.ExecuteScalar("SELECT ISNULL(CertificateEligibilityMode,'ALL') FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            string mode = value == null || value == DBNull.Value ? "ALL" : value.ToString().Trim().ToUpperInvariant();
            return mode == "PASS" || mode == "FAIL" ? mode : "ALL";
        }

        private bool HasAnyRequiredSessionTestFailed(string trainingID, string empID, string testType, string skipColumn)
        {
            string sql = "SELECT CASE WHEN EXISTS (SELECT 1 FROM SessionMaster S INNER JOIN TestMaster TM ON TM.SessionID=S.SessionID AND TM.TestType=@TestType AND TM.IsPublished=1 INNER JOIN TestResult TR ON TR.TestID=TM.TestID AND TR.EmpID=@EmpID AND TR.IsFinalAttempt=1 WHERE S.TrainingID=@TrainingID AND ISNULL(S." + skipColumn + ",0)=0 AND TR.ResultStatus IN ('FAIL','FAILED')) THEN 1 ELSE 0 END";
            object value = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID), new SqlParameter("@TestType", testType) });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }

        private bool IsAlreadyGenerated(string trainingID, string empID)
        {
            string query = "SELECT COUNT(*) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A'";
            return Convert.ToInt32(objDB.ExecuteScalar(query, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) })) > 0;
        }

        private string GenerateCertificateID() { return "CID" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper(); }
        private string GenerateCertificateNumber() { return "CERT" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(); }
        private string GeneratePDFName(string trainingID, string empID) { return empID + "_" + trainingID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf"; }

        private DataRow LoadCertificateData(string trainingID, string empID, string templateID)
        {
            string query = @"
SELECT TCT.TrainingID,@EmpID AS EmpID,TCT.TemplateID,TCT.CourseTitle,
TCT.LeftSignature,TCT.LeftName,TCT.LeftDesignation,TCT.RightSignature,TCT.RightName,TCT.RightDesignation,
CTM.TemplateName,CTM.HeaderText,CTM.FooterText,CTM.BackgroundImage,CTM.LogoImage,
CTM.HeaderFontSize,CTM.FooterFontSize,CTM.CourseTitleFontSize,CTM.BodyFontSize,
CTM.Orientation,CTM.PaperSize,CTM.LogoX,CTM.LogoY,CTM.HeaderY,CTM.TitleY,CTM.BodyY,
CTM.LeftSignatureX,CTM.RightSignatureX,CTM.SignatureY,CTM.FooterY,
TD.DateFrom,TD.DateTo,CM.CourseName,ISNULL(EBM.EmpName,TME.TraineeName) AS EmpName
FROM TrainingCertificateTemplate TCT
INNER JOIN CertificateTemplateMaster CTM ON TCT.TemplateID=CTM.TemplateID
INNER JOIN TrainingDetails TD ON TCT.TrainingID=TD.TrainingID
INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID
LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=@EmpID
LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=@EmpID
WHERE TCT.TrainingID=@TrainingID AND TCT.TemplateID=@TemplateID AND TCT.Active=1 AND CTM.Active=1";
            DataTable dt = objDB.GetDataTable(query, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID), new SqlParameter("@TemplateID", templateID) });
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        private bool CreatePDF(DataRow dr, string certificateID, string certificateNo, string pdfName, string verificationCode)
        {
            string pdfPath = GetPDFPath(pdfName); Document document = null; PdfWriter writer = null;
            try
            {
                Rectangle page = GetPageSize(dr["PaperSize"].ToString(), dr["Orientation"].ToString());
                document = new Document(page, 20, 20, 20, 20); writer = PdfWriter.GetInstance(document, new FileStream(pdfPath, FileMode.Create)); document.Open();
                DrawBackground(writer, document, dr); DrawLogo(writer, document, dr); DrawHeader(writer, document, dr); DrawBody(writer, document, dr); DrawSignature(writer, document, dr); DrawVerificationBlock(writer, document, certificateNo, verificationCode); DrawFooter(writer, document, dr);
                document.Close(); SaveCertificate(certificateID, certificateNo, dr, pdfName, verificationCode); return true;
            }
            catch { if (document != null && document.IsOpen()) document.Close(); if (File.Exists(pdfPath)) File.Delete(pdfPath); throw; }
        }

        private void DrawVerificationBlock(PdfWriter writer, Document document, string certificateNo, string verificationCode)
        {
            PdfContentByte canvas = writer.DirectContent; BaseFont baseFont = GetBaseFont(); Font smallFont = new Font(baseFont, 8, Font.NORMAL, BaseColor.BLACK); Font boldFont = new Font(baseFont, 8, Font.BOLD, BaseColor.BLACK); float leftX = 40f; float bottomY = 55f;
            ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("Certificate No: " + certificateNo, boldFont), leftX, bottomY + 28f, 0); ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("Verification Code: " + verificationCode, smallFont), leftX, bottomY + 14f, 0); ColumnText.ShowTextAligned(canvas, Element.ALIGN_LEFT, new Phrase("Scan QR code to verify this certificate", smallFont), leftX, bottomY, 0); DrawVerificationQRCode(writer, document, certificateNo, verificationCode);
        }

        private void DrawVerificationQRCode(PdfWriter writer, Document document, string certificateNo, string verificationCode)
        {
            string verificationURL = BuildVerificationURL(certificateNo, verificationCode); BarcodeQRCode qrCode = new BarcodeQRCode(verificationURL, 150, 150, null); Image qrImage = qrCode.GetImage(); qrImage.ScaleAbsolute(65f, 65f); float qrX = document.PageSize.Width - 105f; float qrY = 45f; qrImage.SetAbsolutePosition(qrX, qrY); writer.DirectContent.AddImage(qrImage);
        }

        private string BuildVerificationURL(string certificateNo, string verificationCode)
        {
            string baseURL = ConfigurationManager.AppSettings["CertificateVerificationBaseUrl"]; if (String.IsNullOrWhiteSpace(baseURL)) throw new Exception("Certificate verification base URL is not configured."); baseURL = baseURL.TrimEnd('/'); return baseURL + "/VerifyCertificate.aspx?CertificateNo=" + HttpUtility.UrlEncode(certificateNo) + "&Code=" + HttpUtility.UrlEncode(verificationCode);
        }
        private string GetPDFPath(string pdfName) { return Path.Combine(GetCertificateFolder(), pdfName); }
        private string GetCertificateFolder() { string folder = HttpContext.Current.Server.MapPath("~/Uploads/Certificates/"); if (!Directory.Exists(folder)) Directory.CreateDirectory(folder); return folder; }

        private void DrawBackground(PdfWriter writer, Document document, DataRow dr)
        {
            string background = dr["BackgroundImage"].ToString(); if (String.IsNullOrWhiteSpace(background)) return; string filePath = HttpContext.Current.Server.MapPath(background); if (!File.Exists(filePath)) return; Image image = Image.GetInstance(filePath); image.SetAbsolutePosition(0, 0); image.ScaleAbsolute(document.PageSize.Width, document.PageSize.Height); writer.DirectContentUnder.AddImage(image);
        }
        private void DrawLogo(PdfWriter writer, Document document, DataRow dr)
        {
            string logo = dr["LogoImage"].ToString(); if (String.IsNullOrWhiteSpace(logo)) return; string filePath = HttpContext.Current.Server.MapPath(logo); if (!File.Exists(filePath)) return; Image image = Image.GetInstance(filePath); image.ScaleToFit(80f, 80f); image.SetAbsolutePosition(Convert.ToSingle(dr["LogoX"]), Convert.ToSingle(dr["LogoY"])); writer.DirectContent.AddImage(image);
        }
        private void DrawHeader(PdfWriter writer, Document document, DataRow dr)
        {
            string header = dr["HeaderText"].ToString(); if (String.IsNullOrWhiteSpace(header)) return; PdfContentByte canvas = writer.DirectContent; ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(header, GetHeaderFont(dr)), document.PageSize.Width / 2, Convert.ToSingle(dr["HeaderY"]), 0);
        }
        private void DrawBody(PdfWriter writer, Document document, DataRow dr)
        {
            Font titleFont = GetTitleFont(dr); Font bodyFont = GetBodyFont(dr); Font nameFont = new Font(GetBaseFont(), 28, Font.BOLD, BaseColor.BLACK); PdfPTable table = new PdfPTable(1); table.TotalWidth = document.PageSize.Width - 120; table.LockedWidth = true; table.HorizontalAlignment = Element.ALIGN_CENTER; PdfPCell cell = new PdfPCell(); cell.Border = Rectangle.NO_BORDER; cell.HorizontalAlignment = Element.ALIGN_CENTER; cell.Padding = 5;
            cell.AddElement(new Paragraph("CERTIFICATE OF COMPLETION", titleFont) { Alignment = Element.ALIGN_CENTER }); cell.AddElement(new Paragraph("\nThis Certificate is proudly presented to\n", bodyFont) { Alignment = Element.ALIGN_CENTER }); cell.AddElement(new Paragraph(dr["EmpName"].ToString(), nameFont) { Alignment = Element.ALIGN_CENTER }); cell.AddElement(new Paragraph("\nFor Successfully Completing\n", bodyFont) { Alignment = Element.ALIGN_CENTER }); cell.AddElement(new Paragraph(dr["CourseTitle"].ToString(), titleFont) { Alignment = Element.ALIGN_CENTER }); cell.AddElement(new Paragraph("\nDuration : " + Convert.ToDateTime(dr["DateFrom"]).ToString("dd MMM yyyy") + "  To  " + Convert.ToDateTime(dr["DateTo"]).ToString("dd MMM yyyy"), bodyFont) { Alignment = Element.ALIGN_CENTER }); table.AddCell(cell); table.WriteSelectedRows(0, -1, 60, Convert.ToSingle(dr["BodyY"]), writer.DirectContent);
        }
        private void DrawSignature(PdfWriter writer, Document document, DataRow dr)
        {
            DrawSingleSignature(writer, dr, dr["LeftSignature"].ToString(), dr["LeftName"].ToString(), dr["LeftDesignation"].ToString(), Convert.ToSingle(dr["LeftSignatureX"]), Convert.ToSingle(dr["SignatureY"])); DrawSingleSignature(writer, dr, dr["RightSignature"].ToString(), dr["RightName"].ToString(), dr["RightDesignation"].ToString(), Convert.ToSingle(dr["RightSignatureX"]), Convert.ToSingle(dr["SignatureY"]));
        }
        private void DrawSingleSignature(PdfWriter writer, DataRow dr, string imagePath, string name, string designation, float x, float y)
        {
            PdfContentByte canvas = writer.DirectContent; if (!String.IsNullOrWhiteSpace(imagePath)) { string filePath = HttpContext.Current.Server.MapPath(imagePath); if (File.Exists(filePath)) { Image img = Image.GetInstance(filePath); img.ScaleToFit(120f, 50f); img.SetAbsolutePosition(x, y); canvas.AddImage(img); } } Font nameFont = new Font(GetBaseFont(), Convert.ToSingle(dr["BodyFontSize"]), Font.BOLD, BaseColor.BLACK); Font designationFont = GetFooterFont(dr); ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(name, nameFont), x + 60f, y - 15f, 0); ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(designation, designationFont), x + 60f, y - 32f, 0);
        }
        private void DrawFooter(PdfWriter writer, Document document, DataRow dr) { PdfContentByte canvas = writer.DirectContent; ColumnText.ShowTextAligned(canvas, Element.ALIGN_CENTER, new Phrase(dr["FooterText"].ToString(), GetFooterFont(dr)), document.PageSize.Width / 2, Convert.ToSingle(dr["FooterY"]), 0); }

        private void SaveCertificate(string certificateID, string certificateNo, DataRow dr, string pdfName, string verificationCode)
        {
            string trainingID = dr["TrainingID"].ToString(); string empID = dr["EmpID"].ToString(); string templateID = dr["TemplateID"].ToString(); string relativePDFPath = "~/Uploads/Certificates/" + pdfName; string certificateHash = GenerateCertificateHash(certificateNo, trainingID, empID, verificationCode);
            string query = @"INSERT INTO TrainingCertificate (CertificateID,CertificateNo,TrainingID,EmpID,TemplateID,PDFPath,PDFName,GeneratedOn,GeneratedBy,CertificateStatus,CertificateHash,VerificationCode,Remarks) VALUES (@CertificateID,@CertificateNo,@TrainingID,@EmpID,@TemplateID,@PDFPath,@PDFName,GETDATE(),@GeneratedBy,@CertificateStatus,@CertificateHash,@VerificationCode,@Remarks)";
            SqlParameter[] param = { new SqlParameter("@CertificateID", certificateID), new SqlParameter("@CertificateNo", certificateNo), new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID), new SqlParameter("@TemplateID", templateID), new SqlParameter("@PDFPath", relativePDFPath), new SqlParameter("@PDFName", pdfName), new SqlParameter("@GeneratedBy", empID), new SqlParameter("@CertificateStatus", "A"), new SqlParameter("@CertificateHash", certificateHash), new SqlParameter("@VerificationCode", verificationCode), new SqlParameter("@Remarks", DBNull.Value) };
            int result = objDB.ExecuteSql(query, param); if (result <= 0) throw new Exception("Certificate record could not be saved."); UpdateTrainingProgress(trainingID, empID);
        }
        private void UpdateTrainingProgress(string trainingID, string empID)
        {
            string query = @"UPDATE TrainingProgress SET CertificateGenerated=1,CertificateGeneratedOn=GETDATE(),UpdatedOn=GETDATE(),UpdatedBy=@UpdatedBy WHERE TrainingID=@TrainingID AND EmpID=@EmpID";
            int result = objDB.ExecuteSql(query, new SqlParameter[] { new SqlParameter("@UpdatedBy", empID), new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) }); if (result <= 0) throw new Exception("Training progress could not be updated after certificate generation.");
        }
        private string GenerateVerificationCode() { return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper(); }
        private string GenerateCertificateHash(string certificateNo, string trainingID, string empID, string verificationCode)
        {
            string value = certificateNo + "|" + trainingID + "|" + empID + "|" + verificationCode; using (SHA256 sha256 = SHA256.Create()) { byte[] bytes = Encoding.UTF8.GetBytes(value); byte[] hash = sha256.ComputeHash(bytes); StringBuilder result = new StringBuilder(); foreach (byte item in hash) result.Append(item.ToString("x2")); return result.ToString(); }
        }
        private Rectangle GetPageSize(string paperSize, string orientation)
        {
            Rectangle page; switch (paperSize.ToUpper()) { case "A3": page = PageSize.A3; break; case "LETTER": page = PageSize.LETTER; break; case "LEGAL": page = PageSize.LEGAL; break; default: page = PageSize.A4; break; } if (orientation.Equals("Landscape", StringComparison.OrdinalIgnoreCase)) page = page.Rotate(); return page;
        }
        private BaseFont GetBaseFont()
        {
            if (_baseFont != null) return _baseFont; string fontPath = HttpContext.Current.Server.MapPath("~/Fonts/ARIAL.TTF"); if (File.Exists(fontPath)) _baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED); else _baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED); return _baseFont;
        }
        private Font GetHeaderFont(DataRow dr) { return new Font(GetBaseFont(), Convert.ToSingle(dr["HeaderFontSize"]), Font.BOLD, BaseColor.BLACK); }
        private Font GetTitleFont(DataRow dr) { return new Font(GetBaseFont(), Convert.ToSingle(dr["CourseTitleFontSize"]), Font.BOLD, BaseColor.BLACK); }
        private Font GetBodyFont(DataRow dr) { return new Font(GetBaseFont(), Convert.ToSingle(dr["BodyFontSize"]), Font.NORMAL, BaseColor.BLACK); }
        private Font GetFooterFont(DataRow dr) { return new Font(GetBaseFont(), Convert.ToSingle(dr["FooterFontSize"]), Font.NORMAL, BaseColor.BLACK); }
        private string GetTrainingTemplateID(string trainingID)
        {
            string sql = "SELECT TOP 1 TemplateID FROM TrainingCertificateTemplate WHERE TrainingID=@TrainingID AND Active=1 ORDER BY DefaultConfiguration DESC,CreatedOn DESC";
            object result = objDB.ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) }); if (result == null || result == DBNull.Value) return ""; return result.ToString();
        }
    }
}