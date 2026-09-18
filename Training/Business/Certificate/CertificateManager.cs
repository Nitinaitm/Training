using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Training.Common;
using Training.Helper;
using Training.Models;

namespace Training.Business.Certificate
{
    public class CertificateManager
    {
        private clsDataAccess objDB;

        private CommonFunctions objCommon;

        private IDGenerator objID;

        public CertificateManager()
        {
            objDB =
                new clsDataAccess();

            objCommon =
                new CommonFunctions();

            objID =
                new IDGenerator();
        }

        #region Certificate Exists

        public bool CertificateExists(
            string trainingID,
            string empID)
        {
            string sql =

                "SELECT COUNT(*) " +

                "FROM TrainingCertificate " +

                "WHERE TrainingID=@TrainingID " +

                "AND EmpID=@EmpID " +

                "AND Active=1";

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
                objCommon.GetCount(
                sql,
                param) > 0;
        }

        #endregion

        #region Generate Certificate No

        private string GenerateCertificateNo()
        {
            return
                objID.GenerateCertificateNo();
        }

        #endregion

        #region Can Generate Certificate

        public bool CanGenerateCertificate(
            string resultID,
            out string message)
        {
            message = "";

            string sql =

                "SELECT " +

                "TrainingID," +
                "EmpID " +

                "FROM TestResult " +

                "INNER JOIN TestMaster " +

                "ON TestResult.TestID=TestMaster.TestID " +

                "WHERE TestResult.ResultID=@ResultID " +

                "AND TestResult.ResultStatus='Pass' " +

                "AND TestResult.CertificateEligible=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@ResultID",
            resultID)
    };

            DataTable dt =
                objDB.GetDataTable(
                sql,
                param);

            if (dt.Rows.Count == 0)
            {
                message =
                    "Candidate is not eligible for certificate.";

                return false;
            }

            //----------------------------------

            string trainingID =
                dt.Rows[0]["TrainingID"]
                .ToString();

            string empID =
                dt.Rows[0]["EmpID"]
                .ToString();

            //----------------------------------


            if
            (
                CertificateExists(
                trainingID,
                empID)
            )
            {
                message =
                    "Certificate already generated.";

                return false;
            }

            return true;
        }

        #endregion

        #region Generate Certificate

        public bool GenerateCertificate(
            string resultID,
            string generatedBy,
            string templateID,
            out string message)
        {
            message = "";

            try
            {
                //------------------------------------
                // Eligibility
                //------------------------------------

                if
                (
                    !CanGenerateCertificate(
                        resultID,
                        out message)
                )
                {
                    return false;
                }

                //------------------------------------
                // Get Result + Training
                //------------------------------------

                string sql =

                    "SELECT " +

                    "TR.TrainingID," +
                    "TR.EmpID " +

                    "FROM TestResult TR " +

                    "INNER JOIN TestMaster TM " +

                    "ON TR.TestID=TM.TestID " +

                    "WHERE TR.ResultID=@ResultID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@ResultID",
                resultID)
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
                        "Result not found.";

                    return false;
                }

                //------------------------------------
                // Training + Employee
                //------------------------------------

                string trainingID =
                    dt.Rows[0]["TrainingID"]
                    .ToString();

                string empID =
                    dt.Rows[0]["EmpID"]
                    .ToString();

                //------------------------------------
                // Get Certificate Template
                //------------------------------------

                DataTable dtTemplate =
                    GetCertificateTemplate(
                    trainingID,
                    templateID);

                if
                (
                    dtTemplate.Rows.Count == 0
                )
                {
                    message =
                        "Certificate template not found.";

                    return false;
                }

                string trainingTemplateID =
                    dtTemplate.Rows[0]
                    ["TrainingTemplateID"]
                    .ToString();

                //------------------------------------
                // Generate IDs
                //------------------------------------

                string certificateID =
                    objID.GenerateCertificateID();

                string certificateNo =
                    GenerateCertificateNo();

                //------------------------------------
                // Verification
                //------------------------------------

                string verificationCode =
                    GenerateVerificationCode();

                string certificateHash =
                    GenerateCertificateHash(
                        certificateNo,
                        trainingID,
                        empID,
                        verificationCode);


                string verificationURL =
    GenerateVerificationURL(
        verificationCode);
                //------------------------------------
                // Insert Certificate
                //------------------------------------

                sql =

                    "INSERT INTO TrainingCertificate " +

                    "(" +

                    "CertificateID," +
                    "CertificateNo," +
                    "TrainingID," +
                    "EmpID," +
                    "GeneratedOn," +
                    "GeneratedBy," +
                    "TemplateID," +
                    "CertificateStatus," +
                    "CertificateHash," +
"VerificationCode," +
"VerificationURL," +
"CertificateVersion," +
                    "DownloadCount," +
                    "Active" +

                    ")" +

                    " VALUES " +

                    "(" +

                    "@CertificateID," +
                    "@CertificateNo," +
                    "@TrainingID," +
                    "@EmpID," +
                    "GETDATE()," +
                    "@GeneratedBy," +
                    "@TemplateID," +
                    "'Generated'," +
                    "@CertificateHash," +
"@VerificationCode," +
"@VerificationURL," +
"1," +
                    "0," +
                    "1" +

                    ")";

                SqlParameter[] insertParam =
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
                "@GeneratedBy",
                generatedBy),

            new SqlParameter(
                "@TemplateID",
                templateID),

           new SqlParameter(
    "@CertificateHash",
    certificateHash),

new SqlParameter(
    "@VerificationCode",
    verificationCode),

new SqlParameter(
    "@VerificationURL",
    verificationURL)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    insertParam) <= 0
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                //------------------------------------
                // Update Result
                //------------------------------------

                sql =

                    "UPDATE TestResult SET " +

                    "CertificateGenerated=1," +

                    "CertificateGeneratedOn=GETDATE()," +

                    "CertificateGeneratedBy=@GeneratedBy " +

                    "WHERE ResultID=@ResultID";

                SqlParameter[] updateParam =
                {
            new SqlParameter(
                "@GeneratedBy",
                generatedBy),

            new SqlParameter(
                "@ResultID",
                resultID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    updateParam) <= 0
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                //------------------------------------
                // Update Template Usage
                //------------------------------------

                UpdateTemplateUsage(
                    trainingTemplateID);

                //------------------------------------
                // Generate PDF
                //------------------------------------

                CertificatePDFManager pdfManager =
                    new CertificatePDFManager();

                string pdfPath;
                string pdfName;
                string pdfMessage;

                if
                (
                    !pdfManager.GenerateCertificatePDF(
                        certificateID,
                        generatedBy,
                        out pdfPath,
                        out pdfName,
                        out pdfMessage)
                )
                {
                    message =
                        pdfMessage;

                    return false;
                }

                //------------------------------------
                // Update PDF Path
                //------------------------------------

                if
                (
                    !UpdateCertificatePath(
                        certificateID,
                        pdfPath,
                        pdfName,
                        generatedBy,
                        out message)
                )
                {
                    return false;
                }

                //------------------------------------
                // Success
                //------------------------------------

                message =
                    "Certificate generated successfully.";

                return true;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion


        #region Get Certificate

        public DataTable GetCertificate(
            string certificateID)
        {
            string sql =

                "SELECT * " +

                "FROM TrainingCertificate " +

                "WHERE CertificateID=@CertificateID " +

                "AND Active=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CertificateID",
            certificateID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Certificate By Employee

        public DataTable GetCertificateByEmployee(
            string empID)
        {
            string sql =

                "SELECT * " +

                "FROM TrainingCertificate " +

                "WHERE EmpID=@EmpID " +

                "AND Active=1 " +

                "ORDER BY GeneratedOn DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@EmpID",
            empID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Get Certificate By Training

        public DataTable GetCertificateByTraining(
            string trainingID)
        {
            string sql =

                "SELECT * " +

                "FROM TrainingCertificate " +

                "WHERE TrainingID=@TrainingID " +

                "AND Active=1 " +

                "ORDER BY GeneratedOn DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Verify Certificate

        public DataTable VerifyCertificate(
            string verificationCode)
        {
            string sql =

                "SELECT " +

                "CertificateID," +

                "CertificateNo," +

                "TrainingID," +

                "EmpID," +

                "GeneratedOn," +

                "CertificateStatus," +

                "CertificateVersion " +

                "FROM TrainingCertificate " +

                "WHERE VerificationCode=@VerificationCode " +

                "AND Active=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@VerificationCode",
            verificationCode)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Verify By Certificate Number

        public DataTable VerifyCertificateByNumber(
            string certificateNo)
        {
            string sql =

                "SELECT " +

                "CertificateID," +

                "CertificateNo," +

                "TrainingID," +

                "EmpID," +

                "GeneratedOn," +

                "CertificateStatus," +

                "CertificateVersion " +

                "FROM TrainingCertificate " +

                "WHERE CertificateNo=@CertificateNo " +

                "AND Active=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CertificateNo",
            certificateNo)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Update Certificate Path

        public bool UpdateCertificatePath(
            string certificateID,
            string pdfPath,
            string pdfName,
            string updatedBy,
            out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TrainingCertificate SET " +

                    "PDFPath=@PDFPath," +

                    "PDFName=@PDFName," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE CertificateID=@CertificateID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@PDFPath",
                pdfPath),

            new SqlParameter(
                "@PDFName",
                pdfName),

            new SqlParameter(
                "@UpdatedBy",
                updatedBy),

            new SqlParameter(
                "@CertificateID",
                certificateID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) > 0
                )
                {
                    message =
                        Messages.UpdateSuccess;

                    return true;
                }

                message =
                    Messages.DatabaseError;

                return false;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion

        #region Record Certificate Download

        public bool RecordCertificateDownload(
            string certificateID,
            string downloadedBy,
            out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TrainingCertificate SET " +

                    "DownloadedOn=GETDATE()," +

                    "DownloadedBy=@DownloadedBy," +

                    "LastDownloadedOn=GETDATE()," +

                    "LastDownloadedBy=@DownloadedBy," +

                    "DownloadCount=" +
                    "ISNULL(DownloadCount,0)+1 " +

                    "WHERE CertificateID=@CertificateID " +

                    "AND Active=1";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@DownloadedBy",
                downloadedBy),

            new SqlParameter(
                "@CertificateID",
                certificateID)
        };

                if
                (
                    objDB.ExecuteSql(
                        sql,
                        param) > 0
                )
                {
                    message =
                        "Certificate download recorded.";

                    return true;
                }

                message =
                    Messages.DatabaseError;

                return false;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion

        #region Generate Verification Code

        private string GenerateVerificationCode()
        {
            byte[] bytes =
                new byte[6];

            using
            (
                RNGCryptoServiceProvider rng =
                new RNGCryptoServiceProvider()
            )
            {
                rng.GetBytes(bytes);
            }

            StringBuilder code =
                new StringBuilder();

            foreach
            (
                byte b
                in
                bytes
            )
            {
                code.Append(
                    b.ToString("X2"));
            }

            return
                code.ToString();
        }

        #endregion

        #region Generate Certificate Hash

        private string GenerateCertificateHash(
            string certificateNo,
            string trainingID,
            string empID,
            string verificationCode)
        {
            string rawData =
                certificateNo +
                "|" +
                trainingID +
                "|" +
                empID +
                "|" +
                verificationCode;

            using
            (
                SHA256 sha256 =
                SHA256.Create()
            )
            {
                byte[] bytes =
                    Encoding.UTF8.GetBytes(
                    rawData);

                byte[] hash =
                    sha256.ComputeHash(
                    bytes);

                StringBuilder result =
                    new StringBuilder();

                foreach
                (
                    byte b
                    in
                    hash
                )
                {
                    result.Append(
                        b.ToString("x2"));
                }

                return
                    result.ToString();
            }
        }

        #endregion

        #region Cancel Certificate

        public bool CancelCertificate(
            string certificateID,
            string remarks,
            string updatedBy,
            out string message)
        {
            message = "";

            try
            {
                string sql =

                    "UPDATE TrainingCertificate SET " +

                    "CertificateStatus='Cancelled'," +

                    "Remarks=@Remarks," +

                    "Active=0," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE CertificateID=@CertificateID";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@Remarks",
                string.IsNullOrWhiteSpace(
                remarks)
                ?
                (object)DBNull.Value
                :
                remarks),

            new SqlParameter(
                "@UpdatedBy",
                updatedBy),

            new SqlParameter(
                "@CertificateID",
                certificateID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) > 0
                )
                {
                    message =
                        "Certificate cancelled successfully.";

                    return true;
                }

                message =
                    Messages.DatabaseError;

                return false;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion

        #region Re Generate Certificate

        public bool ReGenerateCertificate(
            string oldCertificateID,
            string generatedBy,
            string templateID,
            out string message)
        {
            message = "";

            try
            {
                //------------------------------------
                // Get Old Certificate
                //------------------------------------

                DataTable dt =
                    GetCertificate(
                    oldCertificateID);

                if
                (
                    dt.Rows.Count == 0
                )
                {
                    message =
                        "Certificate not found.";

                    return false;
                }

                //------------------------------------
                // Old Certificate Details
                //------------------------------------

                string trainingID =
                    dt.Rows[0]["TrainingID"]
                    .ToString();

                string empID =
                    dt.Rows[0]["EmpID"]
                    .ToString();

                //------------------------------------
                // Generate New IDs
                //------------------------------------

                string certificateID =
                    objID.GenerateCertificateID();

                string certificateNo =
                    objID.GenerateCertificateNo();

                string verificationCode =
                    GenerateVerificationCode();

                string certificateHash =
                    GenerateCertificateHash(
                        certificateNo,
                        trainingID,
                        empID,
                        verificationCode);

                //------------------------------------
                // Version
                //------------------------------------

                int version = 1;

                if
                (
                    dt.Rows[0]["CertificateVersion"]
                    != DBNull.Value
                )
                {
                    version =
                        Convert.ToInt32(
                        dt.Rows[0]
                        ["CertificateVersion"]) + 1;
                }

                //------------------------------------
                // Insert New Certificate
                //------------------------------------

                string sql =

                    "INSERT INTO TrainingCertificate " +

                    "(" +

                    "CertificateID," +
                    "CertificateNo," +
                    "TrainingID," +
                    "EmpID," +
                    "GeneratedOn," +
                    "GeneratedBy," +
                    "TemplateID," +
                    "CertificateStatus," +
                    "CertificateHash," +
                    "VerificationCode," +
                    "CertificateVersion," +
                    "GeneratedMode," +
                    "GeneratedFrom," +
                    "DownloadCount," +
                    "Active" +

                    ")" +

                    " VALUES " +

                    "(" +

                    "@CertificateID," +
                    "@CertificateNo," +
                    "@TrainingID," +
                    "@EmpID," +
                    "GETDATE()," +
                    "@GeneratedBy," +
                    "@TemplateID," +
                    "'Generated'," +
                    "@CertificateHash," +
                    "@VerificationCode," +
                    "@CertificateVersion," +
                    "'ReGenerated'," +
                    "@GeneratedFrom," +
                    "0," +
                    "1" +

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
                trainingID),

            new SqlParameter(
                "@EmpID",
                empID),

            new SqlParameter(
                "@GeneratedBy",
                generatedBy),

            new SqlParameter(
                "@TemplateID",
                templateID),

            new SqlParameter(
                "@CertificateHash",
                certificateHash),

            new SqlParameter(
                "@VerificationCode",
                verificationCode),

            new SqlParameter(
                "@CertificateVersion",
                version),

            new SqlParameter(
                "@GeneratedFrom",
                oldCertificateID)
        };

                if
                (
                    objDB.ExecuteSql(
                    sql,
                    param) <= 0
                )
                {
                    message =
                        Messages.DatabaseError;

                    return false;
                }

                //------------------------------------
                // Mark Old Certificate
                //------------------------------------

                sql =

                    "UPDATE TrainingCertificate SET " +

                    "CertificateStatus='Reissued'," +

                    "Active=0," +

                    "UpdatedOn=GETDATE()," +

                    "UpdatedBy=@UpdatedBy " +

                    "WHERE CertificateID=@CertificateID";

                SqlParameter[] oldParam =
                {
            new SqlParameter(
                "@UpdatedBy",
                generatedBy),

            new SqlParameter(
                "@CertificateID",
                oldCertificateID)
        };

                objDB.ExecuteSql(
                    sql,
                    oldParam);

                message =
                    "Certificate re-generated successfully.";

                return true;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                return false;
            }
        }

        #endregion

        #region Generate Bulk Certificates

        public int GenerateBulkCertificates(
            string trainingID,
            string generatedBy,
            string templateID)
        {
            int generatedCount = 0;

            string sql =

                "SELECT " +

                "ResultID " +

                "FROM TestResult TR " +

                "INNER JOIN TestMaster TM " +

                "ON TR.TestID=TM.TestID " +

                "WHERE TM.TrainingID=@TrainingID " +

                "AND TR.ResultStatus='Pass' " +

                "AND TR.CertificateEligible=1 " +

                "AND TR.CertificateGenerated=0";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID)
    };

            DataTable dt =
                objDB.GetDataTable(
                sql,
                param);

            foreach
            (
                DataRow dr
                in
                dt.Rows
            )
            {
                string message;

                if
                (
                    GenerateCertificate(
                        dr["ResultID"].ToString(),
                        generatedBy,
                        templateID,
                        out message)
                )
                {
                    generatedCount++;
                }
            }

            return
                generatedCount;
        }

        #endregion

        #region Get Certificate Template

        private DataTable GetCertificateTemplate(
            string trainingID,
            string templateID)
        {
            string sql =

                "SELECT TOP 1 " +

                "TrainingTemplateID," +
                "TrainingID," +
                "CourseID," +
                "TemplateID," +
                "CourseTitle," +
                "LeftSignature," +
                "LeftName," +
                "LeftDesignation," +
                "RightSignature," +
                "RightName," +
                "RightDesignation " +

                "FROM TrainingCertificateTemplate " +

                "WHERE TrainingID=@TrainingID " +

                "AND Active=1 " +

                "AND TemplateID=@TemplateID " +

                "ORDER BY DefaultConfiguration DESC," +
                "TrainingTemplateID DESC";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            trainingID),

        new SqlParameter(
            "@TemplateID",
            templateID)
    };

            return
                objDB.GetDataTable(
                sql,
                param);
        }

        #endregion

        #region Update Template Usage

        private bool UpdateTemplateUsage(
            string trainingTemplateID)
        {
            string sql =

                "UPDATE TrainingCertificateTemplate SET " +

                "UsageCount=ISNULL(UsageCount,0)+1," +

                "LastUsedOn=GETDATE() " +

                "WHERE TrainingTemplateID=@TrainingTemplateID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingTemplateID",
            trainingTemplateID)
    };

            return
                objDB.ExecuteSql(
                sql,
                param) > 0;
        }

        #endregion

        #region Verify Certificate Details

        public CertificateVerificationModel VerifyCertificateDetails(
            string searchValue,
            out string message)
        {
            message = "";

            CertificateVerificationModel model =
                new CertificateVerificationModel();

            try
            {
                if
                (
                    string.IsNullOrWhiteSpace(
                        searchValue)
                )
                {
                    message =
                        "Certificate number or verification code is required.";

                    return model;
                }

                //------------------------------------
                // Search Certificate
                //------------------------------------

                string sql =

                    "SELECT TOP 1 " +

                    "TC.CertificateID," +
                    "TC.CertificateNo," +
                    "TC.VerificationCode," +
                    "TC.TrainingID," +
                    "TC.EmpID," +
                    "TC.PDFPath," +
                    "TC.GeneratedOn," +
                    "TC.CertificateStatus," +
                    "TC.CertificateVersion," +
                    "TC.Remarks," +

                    "TD.TrainingType," +
                    "TD.TrainingOrganizer," +
                    "TD.TrainingLocation," +
                    "TD.DateFrom," +
                    "TD.DateTo," +

                    "EM.EmpName," +
                    "EM.EmpDesignation," +

                    "TCT.CourseTitle " +

                    "FROM TrainingCertificate TC " +

                    "INNER JOIN TrainingDetails TD " +
                    "ON TC.TrainingID=TD.TrainingID " +

                    "INNER JOIN EmpbasicMaster EM " +
                    "ON TC.EmpID=EM.EmpID " +

                    "LEFT JOIN TrainingCertificateTemplate TCT " +
                    "ON TC.TrainingID=TCT.TrainingID " +
                    "AND TC.TemplateID=TCT.TemplateID " +
                    "AND TCT.Active=1 " +

                    "WHERE " +

                    "(TC.CertificateNo=@SearchValue " +

                    "OR TC.VerificationCode=@SearchValue)";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@SearchValue",
                searchValue.Trim())
        };

                DataTable dt =
                    objDB.GetDataTable(
                    sql,
                    param);

                //------------------------------------
                // Not Found
                //------------------------------------

                if
                (
                    dt.Rows.Count == 0
                )
                {
                    message =
                        "Certificate not found.";

                    return model;
                }

                DataRow dr =
                    dt.Rows[0];

                //------------------------------------
                // Fill Model
                //------------------------------------

                model.CertificateID =
                    dr["CertificateID"]
                    .ToString();

                model.CertificateNo =
                    dr["CertificateNo"]
                    .ToString();

                model.VerificationCode =
                    dr["VerificationCode"]
                    .ToString();

                model.TrainingID =
                    dr["TrainingID"]
                    .ToString();

                model.EmpID =
                    dr["EmpID"]
                    .ToString();

                model.EmpName =
                    dr["EmpName"]
                    .ToString();

                model.EmpDesignation =
                    dr["EmpDesignation"]
                    .ToString();

                model.CourseTitle =
                    dr["CourseTitle"]
                    .ToString();

                model.TrainingType =
                    dr["TrainingType"]
                    .ToString();

                model.TrainingOrganizer =
                    dr["TrainingOrganizer"]
                    .ToString();

                model.TrainingLocation =
                    dr["TrainingLocation"]
                    .ToString();

                //------------------------------------
                // Dates
                //------------------------------------

                if
                (
                    dr["DateFrom"] != DBNull.Value
                )
                {
                    model.DateFrom =
                        Convert.ToDateTime(
                            dr["DateFrom"]);
                }

                if
                (
                    dr["DateTo"] != DBNull.Value
                )
                {
                    model.DateTo =
                        Convert.ToDateTime(
                            dr["DateTo"]);
                }

                if
                (
                    dr["GeneratedOn"] != DBNull.Value
                )
                {
                    model.GeneratedOn =
                        Convert.ToDateTime(
                            dr["GeneratedOn"]);
                }

                //------------------------------------
                // Other Details
                //------------------------------------

                model.CertificateStatus =
                    dr["CertificateStatus"]
                    .ToString();

                if
                (
                    dr["CertificateVersion"]
                    != DBNull.Value
                )
                {
                    model.CertificateVersion =
                        Convert.ToInt32(
                            dr["CertificateVersion"]);
                }

                model.PDFPath =
                    dr["PDFPath"]
                    .ToString();

                model.Remarks =
                    dr["Remarks"]
                    .ToString();

                //------------------------------------
                // Validity
                //------------------------------------

                if
                (
                    model.CertificateStatus
                    .Equals(
                        "Generated",
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    !string.IsNullOrWhiteSpace(
                        model.CertificateID)
                )
                {
                    model.IsValid =
                        true;

                    message =
                        "Certificate is valid.";

                    return model;
                }

                //------------------------------------
                // Invalid / Cancelled
                //------------------------------------

                model.IsValid =
                    false;

                message =
                    "Certificate is not valid.";

                return model;
            }
            catch (Exception ex)
            {
                message =
                    ex.Message;

                model.IsValid =
                    false;

                return model;
            }
        }

        #endregion

        #region Generate Verification URL

        private string GenerateVerificationURL(
            string verificationCode)
        {
            string url =
                "https://training.bsphcl.co.in/" +
                "CertificateVerification.aspx?code=" +
                HttpUtility.UrlEncode(
                    verificationCode);

            return url;
        }

        #endregion

        #region Update Verification URL

        private bool UpdateVerificationURL(
            string certificateID,
            string verificationURL)
        {
            string sql =

                "UPDATE TrainingCertificate SET " +

                "VerificationURL=@VerificationURL," +

                "UpdatedOn=GETDATE() " +

                "WHERE CertificateID=@CertificateID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@VerificationURL",
            verificationURL),

        new SqlParameter(
            "@CertificateID",
            certificateID)
    };

            return
                objDB.ExecuteSql(
                    sql,
                    param) > 0;
        }

        #endregion


        #region Get Certificate PDF

        public string GetCertificatePDFPath(
            string certificateID)
        {
            string sql =

                "SELECT PDFPath " +

                "FROM TrainingCertificate " +

                "WHERE CertificateID=@CertificateID " +

                "AND Active=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CertificateID",
            certificateID)
    };

            object value =
                objDB.ExecuteScalar(
                    sql,
                    param);

            if
            (
                value == null ||
                value == DBNull.Value
            )
            {
                return "";
            }

            return
                value.ToString();
        }

        #endregion
    }
}