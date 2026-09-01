using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class GeneratedCertificateList :
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
                BindTraining();

                BindCourse();

                BindCertificate();
            }
        }

        //-----------------------------------------------------
        // Bind Training
        //-----------------------------------------------------

        private void BindTraining()
        {
            string query =
                "SELECT TrainingID, TrainingID + ' | ' + ISNULL(TrainingLocation,'') + ' | Batch ' + ISNULL(Batch,'') AS TrainingName FROM TrainingDetails ORDER BY DateFrom DESC, TrainingID DESC";

            DataTable dt =
                objDB.GetDataTable(
                query);

            ddlTraining.DataSource =
                dt;

            ddlTraining.DataTextField =
                "TrainingName";

            ddlTraining.DataValueField =
                "TrainingID";

            ddlTraining.DataBind();

            ddlTraining.Items.Insert(
                0,
                new ListItem(
                    "-- All Training --",
                    ""));
        }

        //-----------------------------------------------------
        // Bind Course
        //-----------------------------------------------------

        private void BindCourse()
        {
            string query =
                "SELECT CourseID, CourseName FROM CourseMaster ORDER BY CourseName";

            DataTable dt =
                objDB.GetDataTable(
                query);

            ddlCourse.DataSource =
                dt;

            ddlCourse.DataTextField =
                "CourseName";

            ddlCourse.DataValueField =
                "CourseID";

            ddlCourse.DataBind();

            ddlCourse.Items.Insert(
                0,
                new ListItem(
                    "-- All Course --",
                    ""));
        }

        //-----------------------------------------------------
        // Bind Certificate
        //-----------------------------------------------------

        private void BindCertificate()
        {
            lblMessage.Text =
                "";

            DateTime fromDate;

            DateTime toDate;

            if
            (
                !ValidateSearchDates(
                    out fromDate,
                    out toDate)
            )
            {
                gvCertificate.DataSource =
                    null;

                gvCertificate.DataBind();

                return;
            }

            string query =
                "SELECT TC.CertificateID, TC.CertificateNo, TC.TrainingID, TC.EmpID, TC.PDFPath, TC.PDFName, TC.GeneratedOn, TC.CertificateStatus, TD.CourseID, CM.CourseName, TCT.CourseTitle, ISNULL(EBM.EmpName,TME.TraineeName) AS TraineeName, CONVERT(VARCHAR(10),TD.DateFrom,105) + ' to ' + CONVERT(VARCHAR(10),TD.DateTo,105) AS TrainingDuration FROM TrainingCertificate TC INNER JOIN TrainingDetails TD ON TC.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID INNER JOIN TrainingCertificateTemplate TCT ON TC.TrainingID=TCT.TrainingID LEFT JOIN EmpBasicMaster EBM ON EBM.EmpID=TC.EmpID LEFT JOIN TraineeMasterExternal TME ON TME.EmpIDExternal=TC.EmpID WHERE 1=1";

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlTraining.SelectedValue)
            )
            {
                query +=
                    " AND TC.TrainingID=@TrainingID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlCourse.SelectedValue)
            )
            {
                query +=
                    " AND TD.CourseID=@CourseID";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtCourseTitle.Text)
            )
            {
                query +=
                    " AND TCT.CourseTitle LIKE @CourseTitle";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtCertificateNo.Text)
            )
            {
                query +=
                    " AND TC.CertificateNo LIKE @CertificateNo";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtEmployee.Text)
            )
            {
                query +=
                    " AND (TC.EmpID LIKE @Employee OR EBM.EmpName LIKE @Employee OR TME.TraineeName LIKE @Employee)";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    ddlStatus.SelectedValue)
            )
            {
                query +=
                    " AND TC.CertificateStatus=@CertificateStatus";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtFromDate.Text)
            )
            {
                query +=
                    " AND TC.GeneratedOn>=@FromDate";
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                query +=
                    " AND TC.GeneratedOn<DATEADD(DAY,1,@ToDate)";
            }

            query +=
                " ORDER BY TC.GeneratedOn DESC, TC.CertificateNo DESC";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    String.IsNullOrWhiteSpace(
                        ddlTraining.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlTraining.SelectedValue),

                new SqlParameter(
                    "@CourseID",
                    String.IsNullOrWhiteSpace(
                        ddlCourse.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlCourse.SelectedValue),

                new SqlParameter(
                    "@CourseTitle",
                    "%"
                    +
                    txtCourseTitle.Text.Trim()
                    +
                    "%"),

                new SqlParameter(
                    "@CertificateNo",
                    "%"
                    +
                    txtCertificateNo.Text.Trim()
                    +
                    "%"),

                new SqlParameter(
                    "@Employee",
                    "%"
                    +
                    txtEmployee.Text.Trim()
                    +
                    "%"),

                new SqlParameter(
                    "@CertificateStatus",
                    String.IsNullOrWhiteSpace(
                        ddlStatus.SelectedValue)
                    ? (object)DBNull.Value
                    : ddlStatus.SelectedValue),

                new SqlParameter(
                    "@FromDate",
                    String.IsNullOrWhiteSpace(
                        txtFromDate.Text)
                    ? (object)DBNull.Value
                    : fromDate),

                new SqlParameter(
                    "@ToDate",
                    String.IsNullOrWhiteSpace(
                        txtToDate.Text)
                    ? (object)DBNull.Value
                    : toDate)
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            gvCertificate.DataSource =
                dt;

            gvCertificate.DataBind();
        }

        //-----------------------------------------------------
        // Validate Search Dates
        //-----------------------------------------------------

        private bool ValidateSearchDates(
            out DateTime fromDate,
            out DateTime toDate)
        {
            fromDate =
                DateTime.MinValue;

            toDate =
                DateTime.MinValue;

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtFromDate.Text)
            )
            {
                if
                (
                    !DateTime.TryParseExact(
                        txtFromDate.Text.Trim(),
                        "dd-MM-yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out fromDate)
                )
                {
                    ShowError(
                        "Generated From date must be in dd-MM-yyyy format.");

                    return false;
                }
            }

            if
            (
                !String.IsNullOrWhiteSpace(
                    txtToDate.Text)
            )
            {
                if
                (
                    !DateTime.TryParseExact(
                        txtToDate.Text.Trim(),
                        "dd-MM-yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out toDate)
                )
                {
                    ShowError(
                        "Generated To date must be in dd-MM-yyyy format.");

                    return false;
                }
            }

            if
            (
                fromDate
                !=
                DateTime.MinValue
                &&
                toDate
                !=
                DateTime.MinValue
                &&
                fromDate
                >
                toDate
            )
            {
                ShowError(
                    "Generated From date cannot be greater than Generated To date.");

                return false;
            }

            return true;
        }

        //-----------------------------------------------------
        // Search
        //-----------------------------------------------------

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindCertificate();
        }

        //-----------------------------------------------------
        // Reset
        //-----------------------------------------------------

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            ddlTraining.SelectedIndex =
                0;

            ddlCourse.SelectedIndex =
                0;

            txtCourseTitle.Text =
                "";

            txtCertificateNo.Text =
                "";

            txtEmployee.Text =
                "";

            ddlStatus.SelectedIndex =
                0;

            txtFromDate.Text =
                "";

            txtToDate.Text =
                "";

            lblMessage.Text =
                "";

            BindCertificate();
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
                    Convert.ToString(
                        e.CommandArgument);

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
                "SELECT CertificateID, CertificateNo, TrainingID, EmpID, PDFPath, PDFName, GeneratedOn, CertificateStatus FROM TrainingCertificate WHERE CertificateID=@CertificateID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@CertificateID",
                    certificateID)
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

            string physicalPath =
                GetCertificatePhysicalPath(
                    dr);

            if
            (
                String.IsNullOrWhiteSpace(
                    physicalPath)
            )
            {
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

            string physicalPath =
                GetCertificatePhysicalPath(
                    dr);

            if
            (
                String.IsNullOrWhiteSpace(
                    physicalPath)
            )
            {
                return;
            }

            string pdfName =
                GetPDFName(
                    dr,
                    physicalPath);

            SendPDF(
                physicalPath,
                pdfName,
                true);
        }

        //-----------------------------------------------------
        // Get Certificate Physical Path
        //-----------------------------------------------------

        private string GetCertificatePhysicalPath(
            DataRow dr)
        {
            string pdfPath =
                Convert.ToString(
                    dr["PDFPath"]);

            if
            (
                String.IsNullOrWhiteSpace(
                    pdfPath)
            )
            {
                ShowError(
                    "Certificate PDF path is not available.");

                return null;
            }

            string physicalPath;

            try
            {
                physicalPath =
                    Server.MapPath(
                        pdfPath);
            }
            catch
            {
                ShowError(
                    "Invalid certificate PDF path.");

                return null;
            }

            if
            (
                !File.Exists(
                    physicalPath)
            )
            {
                ShowError(
                    "Certificate PDF file could not be found.");

                return null;
            }

            return physicalPath;
        }

        //-----------------------------------------------------
        // Get PDF Name
        //-----------------------------------------------------

        private string GetPDFName(
            DataRow dr,
            string physicalPath)
        {
            string pdfName =
                Convert.ToString(
                    dr["PDFName"]);

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

            pdfName =
                Path.GetFileName(
                    pdfName);

            if
            (
                String.IsNullOrWhiteSpace(
                    pdfName)
            )
            {
                pdfName =
                    "Certificate.pdf";
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

            string disposition =
                download
                ? "attachment"
                : "inline";

            Response.AddHeader(
                "Content-Disposition",
                disposition
                +
                "; filename=\""
                +
                pdfName.Replace(
                    "\"",
                    "")
                +
                "\"");

            FileInfo file =
                new FileInfo(
                    physicalPath);

            Response.AddHeader(
                "Content-Length",
                file.Length
                .ToString());

            Response.TransmitFile(
                physicalPath);

            Response.Flush();

            HttpContext.Current
                .ApplicationInstance
                .CompleteRequest();
        }

        //-----------------------------------------------------
        // Show Error
        //-----------------------------------------------------

        private void ShowError(
            string message)
        {
            lblMessage.ForeColor =
                System.Drawing.Color.Red;

            lblMessage.Text =
                message;
        }
    }
}