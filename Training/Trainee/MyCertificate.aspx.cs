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
    public partial class MyCertificate : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null || String.IsNullOrWhiteSpace(Session["EmpID"].ToString()))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            if (Session["TrainingID"] == null || String.IsNullOrWhiteSpace(Session["TrainingID"].ToString()))
            {
                Response.Redirect("MyTrainings.aspx");
                return;
            }

            if (!IsPostBack)
            {
                TryGeneratePendingCertificate();
                BindCertificate();
                Session.Remove("CertificateFromTraining");
            }
        }

        private void BindCertificate()
        {
            string query = "SELECT TC.CertificateID,TC.CertificateNo,TC.TrainingID,TC.EmpID,TC.TemplateID,TC.PDFPath,TC.PDFName,TC.GeneratedOn,TC.CertificateStatus,ISNULL(TCT.CourseTitle,CM.CourseName) AS CourseTitle,CONVERT(VARCHAR(10),TD.DateFrom,105) + ' to ' + CONVERT(VARCHAR(10),TD.DateTo,105) AS TrainingDuration FROM TrainingCertificate TC INNER JOIN TrainingDetails TD ON TD.TrainingID=TC.TrainingID LEFT JOIN TrainingCertificateTemplate TCT ON TCT.TrainingID=TC.TrainingID AND TCT.TemplateID=TC.TemplateID LEFT JOIN CourseMaster CM ON CM.CourseID=TD.CourseID WHERE TC.EmpID=@EmpID AND TC.CertificateStatus='A' ";
            List<SqlParameter> param = new List<SqlParameter>();
            param.Add(new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()));

            if (Session["CertificateFromTraining"] != null && Session["TrainingID"] != null)
            {
                query += "AND TC.TrainingID=@TrainingID ";
                param.Add(new SqlParameter("@TrainingID", Session["TrainingID"].ToString()));
            }

            query += "ORDER BY TC.GeneratedOn DESC";
            DataTable dt = objDB.GetDataTable(query, param.ToArray());
            gvCertificate.DataSource = dt;
            gvCertificate.DataBind();
            lblMessage.Text = "";
        }

        private void TryGeneratePendingCertificate()
        {
            if (Session["EmpID"] == null || Session["TrainingID"] == null || Session["CertificateFromTraining"] == null) return;

            string trainingID = Session["TrainingID"].ToString();
            string empID = Session["EmpID"].ToString().ToUpperInvariant();
            string query = "SELECT COUNT(*) FROM TrainingCertificate WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND CertificateStatus='A'";
            SqlParameter[] param = { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) };
            object result = objDB.ExecuteScalar(query, param);
            int count = result == null ? 0 : Convert.ToInt32(result);
            if (count > 0) return;

            if (!CanGenerateCertificate(trainingID, empID))
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Certificate is available only after completing the required training workflow.";
                return;
            }

            try
            {
                CertificateGenerator generator = new CertificateGenerator();
                bool generated = generator.GenerateCertificate(trainingID, empID);
                if (!generated)
                {
                    lblMessage.ForeColor = System.Drawing.Color.Red;
                    lblMessage.Text = "Certificate generation failed: " + generator.LastError;
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;
                lblMessage.Text = "Certificate generation error: " + ex.Message;
            }
        }

        private bool CanGenerateCertificate(string trainingID, string empID)
        {
            DataTable dt = objDB.GetDataTable("SELECT AttendanceRequired,InitialAssessmentRequired,FinalAssessmentRequired,FeedbackRequired,CertificateRequired,ISNULL(FeedbackSkipped,0) FeedbackSkipped,ISNULL(CertificateSkipped,0) CertificateSkipped,PreTestCertificateRule,PostTestCertificateRule FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
            if (dt.Rows.Count == 0) return false;
            DataRow r = dt.Rows[0];

            bool certificateRequired = Convert.ToBoolean(r["CertificateRequired"]);
            bool certificateSkipped = Convert.ToBoolean(r["CertificateSkipped"]);
            if (!certificateRequired || certificateSkipped) return false;

            bool attendanceRequired = Convert.ToBoolean(r["AttendanceRequired"]);
            bool preRequired = Convert.ToBoolean(r["InitialAssessmentRequired"]);
            bool postRequired = Convert.ToBoolean(r["FinalAssessmentRequired"]);
            bool feedbackRequired = Convert.ToBoolean(r["FeedbackRequired"]);
            bool feedbackSkipped = Convert.ToBoolean(r["FeedbackSkipped"]);

            object assignment = objDB.ExecuteScalar("SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND AssignmentStatus='Assigned'", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
            if (assignment == null || Convert.ToInt32(assignment) == 0) return false;

            if (attendanceRequired && !AreAllRequiredSessionAttendanceCompleted(trainingID)) return false;

            string preRule = r["PreTestCertificateRule"] == DBNull.Value ? "" : r["PreTestCertificateRule"].ToString().Trim().ToUpperInvariant();
            string postRule = r["PostTestCertificateRule"] == DBNull.Value ? "" : r["PostTestCertificateRule"].ToString().Trim().ToUpperInvariant();

            bool preApplicable = preRequired && HasUnskippedSessions(trainingID, "PreAssessmentSkipped");
            bool postApplicable = postRequired && HasUnskippedSessions(trainingID, "PostAssessmentSkipped");

            if (preApplicable && (preRule != "ALL" && preRule != "PASS")) return false;
            if (postApplicable && (postRule != "ALL" && postRule != "PASS")) return false;

            if (preApplicable && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Pre", "PreAssessmentSkipped")) return false;
            if (postApplicable && !AreAllRequiredSessionTestsCompleted(trainingID, empID, "Post", "PostAssessmentSkipped")) return false;

            if (preApplicable && preRule == "PASS" && !AreAllRequiredSessionTestsPassed(trainingID, empID, "Pre", "PreAssessmentSkipped")) return false;
            if (postApplicable && postRule == "PASS" && !AreAllRequiredSessionTestsPassed(trainingID, empID, "Post", "PostAssessmentSkipped")) return false;

            if (feedbackRequired && !feedbackSkipped && !IsFeedbackSubmitted(trainingID, empID)) return false;
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

        private bool IsFeedbackSubmitted(string trainingID, string empID)
        {
            object v = objDB.ExecuteScalar("SELECT COUNT(*) FROM Feedback WHERE TrainingID=@TrainingID AND EmpID=@EmpID AND ISNULL(Submitted,0)=1", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
            return v != null && Convert.ToInt32(v) > 0;
        }

        protected void gvCertificate_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                string certificateID = e.CommandArgument.ToString();
                if (String.IsNullOrWhiteSpace(certificateID)) { ShowError("Invalid certificate."); return; }
                if (e.CommandName == "ViewCertificate") ViewCertificate(certificateID);
                else if (e.CommandName == "DownloadCertificate") DownloadCertificate(certificateID);
            }
            catch (Exception ex) { ShowError("Unable to process certificate. " + ex.Message); }
        }

        private DataRow GetCertificate(string certificateID)
        {
            string query = @"SELECT CertificateID,CertificateNo,TrainingID,EmpID,PDFPath,PDFName,GeneratedOn,CertificateStatus FROM TrainingCertificate WHERE CertificateID=@CertificateID AND EmpID=@EmpID AND CertificateStatus='A'";
            SqlParameter[] param = { new SqlParameter("@CertificateID", certificateID), new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()) };
            DataTable dt = objDB.GetDataTable(query, param);
            return dt.Rows.Count == 0 ? null : dt.Rows[0];
        }

        private void ViewCertificate(string certificateID)
        {
            DataRow dr = GetCertificate(certificateID);
            if (dr == null) { ShowError("Certificate not found."); return; }
            string pdfPath = dr["PDFPath"].ToString();
            if (String.IsNullOrWhiteSpace(pdfPath)) { ShowError("Certificate PDF is not available."); return; }
            string physicalPath = Server.MapPath(pdfPath);
            if (!File.Exists(physicalPath)) { ShowError("Certificate PDF file could not be found."); return; }
            SendPDF(physicalPath, GetPDFName(dr, physicalPath), false);
        }

        private void DownloadCertificate(string certificateID)
        {
            DataRow dr = GetCertificate(certificateID);
            if (dr == null) { ShowError("Certificate not found."); return; }
            string pdfPath = dr["PDFPath"].ToString();
            if (String.IsNullOrWhiteSpace(pdfPath)) { ShowError("Certificate PDF is not available."); return; }
            string physicalPath = Server.MapPath(pdfPath);
            if (!File.Exists(physicalPath)) { ShowError("Certificate PDF file could not be found."); return; }
            string pdfName = GetPDFName(dr, physicalPath);
            UpdateDownloadDetails(certificateID);
            SendPDF(physicalPath, pdfName, true);
        }

        private string GetPDFName(DataRow dr, string physicalPath)
        {
            string pdfName = dr["PDFName"].ToString();
            return String.IsNullOrWhiteSpace(pdfName) ? Path.GetFileName(physicalPath) : pdfName;
        }

        private void SendPDF(string physicalPath, string pdfName, bool download)
        {
            Response.Clear(); Response.ClearHeaders(); Response.ClearContent(); Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition", (download ? "attachment" : "inline") + "; filename=\"" + pdfName + "\"");
            Response.AddHeader("Content-Length", new FileInfo(physicalPath).Length.ToString());
            Response.TransmitFile(physicalPath); Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private void UpdateDownloadDetails(string certificateID)
        {
            string query = @"UPDATE TrainingCertificate SET DownloadedOn=GETDATE(),DownloadedBy=@DownloadedBy WHERE CertificateID=@CertificateID AND EmpID=@EmpID AND CertificateStatus='A'";
            SqlParameter[] param = { new SqlParameter("@DownloadedBy", Session["EmpID"].ToString().ToUpperInvariant()), new SqlParameter("@CertificateID", certificateID), new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()) };
            objDB.ExecuteSql(query, param);
        }

        private void ShowError(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
        }
    }
}