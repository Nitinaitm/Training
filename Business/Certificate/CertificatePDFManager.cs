using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


using System.IO;
using System.Data;
using System.Data.SqlClient;


using iTextSharp.text;
using iTextSharp.text.pdf;
using QRCoder;

namespace Training.Business.Certificate
{
   
     public class CertificatePDFManager
    {
        private clsDataAccess objDB =
            new clsDataAccess();

        #region Generate Certificate PDF

        public bool GenerateCertificatePDF(
            string certificateID,
            string generatedBy,
            out string pdfPath,
            out string pdfName,
            out string message)
        {
            pdfPath = "";
            pdfName = "";
            message = "";

            try
            {
                //------------------------------------
                // Get Certificate + Training +
                // Employee + Training Template +
                // Certificate Master Template
                //------------------------------------

                string sql =

                    "SELECT " +

                    "TC.CertificateID," +
                    "TC.CertificateNo," +
                    "TC.TrainingID," +
                    "TC.EmpID," +
                    "TC.TemplateID," +
                    "TC.VerificationCode," +
                    "TC.VerificationURL," +

                    "TD.TrainingType," +
                    "TD.TrainingOrganizer," +
                    "TD.TrainingLocation," +
                    "TD.Batch," +
                    "TD.DateFrom," +
                    "TD.DateTo," +
                    "TD.NoOfDays," +
                    "TD.Hours," +

                    "EM.EmpName," +
                    "EM.EmpDesignation," +
                    "EM.EmpPostingPlace," +

                    "TCT.TrainingTemplateID," +
                    "TCT.CourseTitle," +
                    "TCT.LeftSignature," +
                    "TCT.LeftName," +
                    "TCT.LeftDesignation," +
                    "TCT.RightSignature," +
                    "TCT.RightName," +
                    "TCT.RightDesignation," +

                    "CTM.TemplateName," +
                    "CTM.Orientation," +
                    "CTM.PageWidth," +
                    "CTM.PageHeight," +
                    "CTM.BackgroundImage," +
                    "CTM.LogoImage," +
                    "CTM.HeaderText," +
                    "CTM.FooterText," +
                    "CTM.CourseTitleFontSize," +
                    "CTM.HeaderFontSize," +
                    "CTM.FooterFontSize," +
                    "CTM.BodyFontSize," +
                    "CTM.NameFontSize," +

                    "CTM.LogoX," +
                    "CTM.LogoY," +
                    "CTM.HeaderY," +
                    "CTM.TitleY," +
                    "CTM.BodyY," +
                    "CTM.LeftSignatureX," +
                    "CTM.RightSignatureX," +
                    "CTM.SignatureY," +
                    "CTM.FooterY " +

                    "FROM TrainingCertificate TC " +

                    "INNER JOIN TrainingDetails TD " +
                    "ON TC.TrainingID=TD.TrainingID " +

                    "INNER JOIN EmpbasicMaster EM " +
                    "ON TC.EmpID=EM.EmpID " +

                    "INNER JOIN TrainingCertificateTemplate TCT " +
                    "ON TC.TrainingID=TCT.TrainingID " +
                    "AND TC.TemplateID=TCT.TemplateID " +

                    "INNER JOIN CertificateTemplateMaster CTM " +
                    "ON TCT.TemplateID=CTM.TemplateID " +

                    "WHERE TC.CertificateID=@CertificateID " +

                    "AND TC.Active=1 " +
                    "AND TCT.Active=1 " +
                    "AND CTM.Active=1";

                SqlParameter[] param =
                {
                    new SqlParameter(
                        "@CertificateID",
                        certificateID)
                };

                DataTable dt =
                    objDB.GetDataTable(
                        sql,
                        param);

                if
                (
                    dt.Rows.Count == 0
                )
                {
                    message =
                        "Certificate or template not found.";

                    return false;
                }

                DataRow dr =
                    dt.Rows[0];

                //------------------------------------
                // Page Size
                //------------------------------------

                float pageWidth =
                    GetFloat(
                        dr["PageWidth"],
                        794);

                float pageHeight =
                    GetFloat(
                        dr["PageHeight"],
                        1123);

                Rectangle pageSize =
                    new Rectangle(
                        pageWidth,
                        pageHeight);

                if
                (
                    dr["Orientation"]
                    .ToString()
                    .Equals(
                        "Landscape",
                        StringComparison.OrdinalIgnoreCase)
                )
                {
                    if
                    (
                        pageHeight > pageWidth
                    )
                    {
                        pageSize =
                            pageSize.Rotate();
                    }
                }
                else
                {
                    if
                    (
                        pageWidth > pageHeight
                    )
                    {
                        pageSize =
                            pageSize.Rotate();
                    }
                }

                //------------------------------------
                // Folder
                //------------------------------------

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

                //------------------------------------
                // File Name
                //------------------------------------

                pdfName =
                    dr["EmpID"].ToString()
                    +
                    "_"
                    +
                    dr["CertificateNo"].ToString()
                    +
                    "_"
                    +
                    DateTime.Now.ToString(
                        "yyyyMMddHHmmss")
                    +
                    ".pdf";

                string physicalPath =
                    Path.Combine(
                        folder,
                        pdfName);

                //------------------------------------
                // Document
                //------------------------------------

                Document document =
                    new Document(
                        pageSize,
                        0,
                        0,
                        0,
                        0);

                PdfWriter writer =
                    PdfWriter.GetInstance(
                        document,
                        new FileStream(
                            physicalPath,
                            FileMode.Create));

                document.Open();

                //------------------------------------
                // Background
                //------------------------------------

                AddBackground(
                    writer,
                    dr["BackgroundImage"]
                    .ToString());

                //------------------------------------
                // Border
                //------------------------------------

                AddBorder(
                    writer);

                //------------------------------------
                // Logo
                //------------------------------------

                AddLogo(
                    writer,
                    dr["LogoImage"].ToString(),
                    GetFloat(
                        dr["LogoX"],
                        50),
                    GetFloat(
                        dr["LogoY"],
                        pageSize.Height - 100));

                //------------------------------------
                // Header
                //------------------------------------

                string headerText =
                    dr["HeaderText"]
                    .ToString();

                if
                (
                    !string.IsNullOrWhiteSpace(
                        headerText)
                )
                {
                    AddText(
                        writer,
                        headerText,
                        GetFloat(
                            dr["HeaderFontSize"],
                            18),
                        GetFloat(
                            dr["HeaderY"],
                            pageSize.Height - 80),
                        pageSize.Width / 2);
                }

                //------------------------------------
                // Certificate Title
                //------------------------------------

                AddText(
                    writer,
                    "CERTIFICATE OF COMPLETION",
                    GetFloat(
                        dr["CourseTitleFontSize"],
                        26),
                    GetFloat(
                        dr["TitleY"],
                        pageSize.Height - 180),
                    pageSize.Width / 2);

                //------------------------------------
                // Course Title
                //------------------------------------

                AddText(
                    writer,
                    dr["CourseTitle"]
                    .ToString(),
                    GetFloat(
                        dr["CourseTitleFontSize"],
                        20),
                    GetFloat(
                        dr["TitleY"],
                        pageSize.Height - 230),
                    pageSize.Width / 2);

                //------------------------------------
                // Body
                //------------------------------------

                float bodyY =
                    GetFloat(
                        dr["BodyY"],
                        pageSize.Height - 330);

                float bodyFont =
                    GetFloat(
                        dr["BodyFontSize"],
                        16);

                AddText(
                    writer,
                    "This certificate is proudly awarded to",
                    bodyFont,
                    bodyY,
                    pageSize.Width / 2);

                //------------------------------------
                // Employee Name
                //------------------------------------

                AddText(
                    writer,
                    dr["EmpName"]
                    .ToString(),
                    GetFloat(
                        dr["NameFontSize"],
                        28),
                    bodyY - 50,
                    pageSize.Width / 2);

                //------------------------------------
                // Designation
                //------------------------------------

                AddText(
                    writer,
                    dr["EmpDesignation"]
                    .ToString(),
                    bodyFont - 2,
                    bodyY - 80,
                    pageSize.Width / 2);

                //------------------------------------
                // Training Details
                //------------------------------------

                string trainingDetails =

                    "Training Period: "
                    +
                    GetDate(
                        dr["DateFrom"])
                    +
                    " to "
                    +
                    GetDate(
                        dr["DateTo"])

                    +
                    "\n"

                    +
                    "Training Type: "
                    +
                    dr["TrainingType"]
                    .ToString()

                    +
                    "\n"

                    +
                    "Organizer: "
                    +
                    dr["TrainingOrganizer"]
                    .ToString()

                    +
                    "\n"

                    +
                    "Location: "
                    +
                    dr["TrainingLocation"]
                    .ToString();

                AddMultilineText(
                    writer,
                    trainingDetails,
                    bodyFont - 3,
                    pageSize.Width / 2,
                    bodyY - 125);

                //------------------------------------
                // Left Signature
                //------------------------------------

                AddSignature(
                    writer,
                    dr["LeftSignature"]
                    .ToString(),
                    dr["LeftName"]
                    .ToString(),
                    dr["LeftDesignation"]
                    .ToString(),
                    GetFloat(
                        dr["LeftSignatureX"],
                        pageSize.Width * 0.25f),
                    GetFloat(
                        dr["SignatureY"],
                        120));

                //------------------------------------
                // Right Signature
                //------------------------------------

                AddSignature(
                    writer,
                    dr["RightSignature"]
                    .ToString(),
                    dr["RightName"]
                    .ToString(),
                    dr["RightDesignation"]
                    .ToString(),
                    GetFloat(
                        dr["RightSignatureX"],
                        pageSize.Width * 0.75f),
                    GetFloat(
                        dr["SignatureY"],
                        120));

                //------------------------------------
                // Certificate Number
                //------------------------------------

                float footerY =
                    GetFloat(
                        dr["FooterY"],
                        50);

                AddTextLeft(
                    writer,
                    "Certificate No: "
                    +
                    dr["CertificateNo"]
                    .ToString(),
                    9,
                    40,
                    footerY + 25);

                //------------------------------------
                // Verification Code
                //------------------------------------

                AddTextLeft(
                    writer,
                    "Verification Code: "
                    +
                    dr["VerificationCode"]
                    .ToString(),
                    8,
                    40,
                    footerY + 10);

                //------------------------------------
                // Footer
                //------------------------------------

                string footerText =
                    dr["FooterText"]
                    .ToString();

                if
                (
                    !string.IsNullOrWhiteSpace(
                        footerText)
                )
                {
                    AddText(
                        writer,
                        footerText,
                        GetFloat(
                            dr["FooterFontSize"],
                            12),
                        footerY,
                        pageSize.Width / 2);
                }

                //------------------------------------
                // QR
                //------------------------------------

                //AddQRCode(
                //    writer,
                //    dr["VerificationURL"]
                //    .ToString(),
                //    pageSize.Width - 80,
                //    footerY + 45);
                //------------------------------------
                // QR Verification URL
                //------------------------------------

                string verificationURL =

                    "https://training.bsphcl.co.in/CertificateVerification.aspx"
                    +
                    "?CertificateNo="
                    +
                    HttpUtility.UrlEncode(
                        dr["CertificateNo"].ToString())
                    +
                    "&VerificationCode="
                    +
                    HttpUtility.UrlEncode(
                        dr["VerificationCode"].ToString());


                //------------------------------------
                // QR
                //------------------------------------

                AddQRCode(
                    writer,
                    verificationURL,
                    pageSize.Width - 80,
                    footerY + 45);
                //------------------------------------
                // Close
                //------------------------------------

                document.Close();

                //------------------------------------
                // Relative Path
                //------------------------------------

                pdfPath =
                    "~/Uploads/Certificates/"
                    +
                    pdfName;

                message =
                    "Certificate PDF generated successfully.";

                return true;
            }
            catch
            (
                Exception ex
            )
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Background

        private void AddBackground(
            PdfWriter writer,
            string backgroundPath)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    backgroundPath)
            )
            {
                return;
            }

            string physicalPath =
                HttpContext.Current.Server.MapPath(
                    backgroundPath);

            if
            (
                !File.Exists(
                    physicalPath)
            )
            {
                return;
            }

            Image background =
                Image.GetInstance(
                    physicalPath);

            Rectangle page =
                writer.PageSize;

            background.SetAbsolutePosition(
                0,
                0);

            background.ScaleAbsolute(
                page.Width,
                page.Height);

            writer.DirectContentUnder.AddImage(
                background);
        }

        #endregion


        #region Border

        private void AddBorder(
            PdfWriter writer)
        {
            Rectangle page =
                writer.PageSize;

            PdfContentByte canvas =
                writer.DirectContent;

            canvas.SetLineWidth(
                2f);

            canvas.Rectangle(
                20,
                20,
                page.Width - 40,
                page.Height - 40);

            canvas.Stroke();
        }

        #endregion


        #region Logo

        private void AddLogo(
            PdfWriter writer,
            string logoPath,
            float x,
            float y)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    logoPath)
            )
            {
                return;
            }

            string physicalPath =
                HttpContext.Current.Server.MapPath(
                    logoPath);

            if
            (
                !File.Exists(
                    physicalPath)
            )
            {
                return;
            }

            Image logo =
                Image.GetInstance(
                    physicalPath);

            logo.ScaleToFit(
                100,
                70);

            logo.SetAbsolutePosition(
                x,
                y);

            writer.DirectContent.AddImage(
                logo);
        }

        #endregion


        #region Text

        private void AddText(
            PdfWriter writer,
            string text,
            float fontSize,
            float y,
            float x)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    text)
            )
            {
                return;
            }

            BaseFont baseFont =
                BaseFont.CreateFont(
                    BaseFont.HELVETICA,
                    BaseFont.CP1252,
                    BaseFont.NOT_EMBEDDED);

            PdfContentByte canvas =
                writer.DirectContent;

            canvas.BeginText();

            canvas.SetFontAndSize(
                baseFont,
                fontSize);

            canvas.ShowTextAligned(
                Element.ALIGN_CENTER,
                text,
                x,
                y,
                0);

            canvas.EndText();
        }


        private void AddTextLeft(
            PdfWriter writer,
            string text,
            float fontSize,
            float x,
            float y)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    text)
            )
            {
                return;
            }

            BaseFont baseFont =
                BaseFont.CreateFont(
                    BaseFont.HELVETICA,
                    BaseFont.CP1252,
                    BaseFont.NOT_EMBEDDED);

            PdfContentByte canvas =
                writer.DirectContent;

            canvas.BeginText();

            canvas.SetFontAndSize(
                baseFont,
                fontSize);

            canvas.ShowTextAligned(
                Element.ALIGN_LEFT,
                text,
                x,
                y,
                0);

            canvas.EndText();
        }

        #endregion


        #region Multiline Text

        private void AddMultilineText(
            PdfWriter writer,
            string text,
            float fontSize,
            float x,
            float y)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    text)
            )
            {
                return;
            }

            BaseFont baseFont =
                BaseFont.CreateFont(
                    BaseFont.HELVETICA,
                    BaseFont.CP1252,
                    BaseFont.NOT_EMBEDDED);

            PdfContentByte canvas =
                writer.DirectContent;

            string[] lines =
                text.Split(
                    new string[]
                    {
                        "\n"
                    },
                    StringSplitOptions.None);

            float currentY =
                y;

            canvas.BeginText();

            canvas.SetFontAndSize(
                baseFont,
                fontSize);

            foreach
            (
                string line
                in
                lines
            )
            {
                canvas.ShowTextAligned(
                    Element.ALIGN_CENTER,
                    line.Trim(),
                    x,
                    currentY,
                    0);

                currentY -=
                    fontSize + 5;
            }

            canvas.EndText();
        }

        #endregion


        #region Signature

        private void AddSignature(
            PdfWriter writer,
            string signaturePath,
            string name,
            string designation,
            float x,
            float y)
        {
            //------------------------------------
            // Signature Image
            //------------------------------------

            if
            (
                !string.IsNullOrWhiteSpace(
                    signaturePath)
            )
            {
                string physicalPath =
                    HttpContext.Current.Server.MapPath(
                        signaturePath);

                if
                (
                    File.Exists(
                        physicalPath)
                )
                {
                    Image signature =
                        Image.GetInstance(
                            physicalPath);

                    signature.ScaleToFit(
                        120,
                        55);

                    signature.SetAbsolutePosition(
                        x - 60,
                        y + 35);

                    writer.DirectContent.AddImage(
                        signature);
                }
            }

            //------------------------------------
            // Line
            //------------------------------------

            PdfContentByte canvas =
                writer.DirectContent;

            canvas.SetLineWidth(
                0.7f);

            canvas.MoveTo(
                x - 70,
                y + 25);

            canvas.LineTo(
                x + 70,
                y + 25);

            canvas.Stroke();

            //------------------------------------
            // Name
            //------------------------------------

            AddText(
                writer,
                name,
                10,
                y + 8,
                x);

            //------------------------------------
            // Designation
            //------------------------------------

            AddText(
                writer,
                designation,
                8,
                y - 5,
                x);
        }

        #endregion


        #region QR Code

        private void AddQRCode(
            PdfWriter writer,
            string verificationURL,
            float x,
            float y)
        {
            if
            (
                string.IsNullOrWhiteSpace(
                    verificationURL)
            )
            {
                return;
            }

            QRCodeGenerator qrGenerator =
                new QRCodeGenerator();

            QRCodeData qrCodeData =
                qrGenerator.CreateQrCode(
                    verificationURL,
                    QRCodeGenerator.ECCLevel.Q);

            QRCode qrCode =
                new QRCode(
                    qrCodeData);

            System.Drawing.Bitmap bitmap =
                qrCode.GetGraphic(10);

            using
            (
                MemoryStream stream =
                    new MemoryStream()
            )
            {
                bitmap.Save(
                    stream,
                    System.Drawing.Imaging.ImageFormat.Png);

                Image qrImage =
                    Image.GetInstance(
                        stream.ToArray());

                qrImage.ScaleToFit(
                    70,
                    70);

                qrImage.SetAbsolutePosition(
                    x - 35,
                    y);

                writer.DirectContent.AddImage(
                    qrImage);
            }

            AddText(
                writer,
                "Scan to Verify",
                7,
                y - 12,
                x);
        }

        #endregion


        #region Helpers

        private float GetFloat(
            object value,
            float defaultValue)
        {
            if
            (
                value == null ||
                value == DBNull.Value
            )
            {
                return defaultValue;
            }

            float result;

            if
            (
                float.TryParse(
                    value.ToString(),
                    out result)
            )
            {
                return result;
            }

            return defaultValue;
        }


        private string GetDate(
            object value)
        {
            if
            (
                value == null ||
                value == DBNull.Value
            )
            {
                return "";
            }

            return
                Convert.ToDateTime(
                    value)
                .ToString(
                    "dd-MM-yyyy");
        }

        #endregion
    }
}