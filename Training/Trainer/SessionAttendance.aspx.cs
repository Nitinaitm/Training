using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.IO;
namespace Training.Trainer
{
    public partial class SessionAttendance : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainingID"] == null)
            {
                Response.Redirect("~/Trainer/Default.aspx");

                return;
            }

            if (Session["SessionID"] == null)
            {
                Response.Redirect("~/Trainer/Default.aspx");

                return;
            }

            if (!IsPostBack)
            {
                pnlNormalAttendance.Visible = true;

                pnlBulkAttendance.Visible = false;

                SessionSummary1.LoadSession(Session["SessionID"].ToString());

                BindGrid();

                BindSummary();

                CheckAttendanceStatus();
            }
        }
        private void CheckAttendanceStatus()
        {
            string query = @"SELECT TD.WorkflowStatus,SM.AttendanceStatus FROM TrainingDetails TD INNER JOIN SessionMaster SM ON TD.TrainingID=SM.TrainingID WHERE SM.SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter("@SessionID",Session["SessionID"].ToString())
    };

            DataTable dt =
                obj.GetDataTable(
                query,
                param);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return;
            }

            string workflowStatus =
                dt.Rows[0]["WorkflowStatus"].ToString();

            string attendanceStatus =
                dt.Rows[0]["AttendanceStatus"].ToString();

            bool attendanceLocked =
                workflowStatus == "ABCDEF"
                ||
                workflowStatus == "ABCDEFG"
                ||
                workflowStatus == "ABCDEFGH"
                ||
                workflowStatus == "ABCDEFGHI"
                ||
                workflowStatus == "ABCDEFGHIJ";

            if
            (
                attendanceLocked
                ||
                attendanceStatus == "Completed"
            )
            {
                gvAttendance.Enabled =
                    false;

                btnSaveAttendance.Enabled =
                    false;

                btnCompleteAttendance.Enabled =
                    false;

                btnUploadExcel.Enabled =
                    false;

                btnUploadAttendanceSheet.Enabled =
                    false;

                lblMessage.ForeColor =
                    System.Drawing.Color.Blue;

                lblMessage.Text =
                    "Attendance already completed.";

                return;
            }

            gvAttendance.Enabled =
                true;

            btnSaveAttendance.Enabled =
                true;

            btnCompleteAttendance.Enabled =
                true;

            btnUploadExcel.Enabled =
                true;

            btnUploadAttendanceSheet.Enabled =
                true;
        }
      
        private void BindGrid()
        {
            string query = @"SELECT TA.AssignmentID,TA.EmpID,EBM.EmpName,EBM.EmpDesignation,ISNULL(SA.AttendanceStatus,'') AttendanceStatus,ISNULL(SA.Remarks,'') Remarks FROM TrainingAssignment TA INNER JOIN EmpBasicMaster EBM ON TA.EmpID=EBM.EmpID LEFT JOIN SessionAttendance SA ON TA.TrainingID=SA.TrainingID AND TA.EmpID=SA.EmpID AND SA.SessionID=@SessionID WHERE TA.TrainingID=@TrainingID AND TA.AssignmentStatus='Assigned' ORDER BY EBM.EmpName";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),
                new SqlParameter("@SessionID",Session["SessionID"].ToString())
            };

            DataTable dt = obj.GetDataTable(query, param);

            gvAttendance.DataSource = dt;

            gvAttendance.DataBind();
        }

        protected void gvAttendance_RowDataBound(
    object sender,
    GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
            {
                return;
            }

            DropDownList ddlAttendance =
                (DropDownList)e.Row.FindControl("ddlAttendance");

            if (ddlAttendance == null)
            {
                return;
            }

            string attendanceStatus =
                DataBinder.Eval(
                    e.Row.DataItem,
                    "AttendanceStatus").ToString();

            if (ddlAttendance.Items.FindByValue(attendanceStatus) != null)
            {
                ddlAttendance.SelectedValue =
                    attendanceStatus;
            }
            if (attendanceStatus == "Present")
            {
                ddlAttendance.CssClass = "form-select border-success";
            }

            else if (attendanceStatus == "Absent")
            {
                ddlAttendance.CssClass = "form-select border-danger";
            }
        }

        private void BindSummary()
        {
            string query = @"SELECT COUNT(*) TotalTrainee,SUM(CASE WHEN AttendanceStatus='Present' THEN 1 ELSE 0 END) PresentCount,SUM(CASE WHEN AttendanceStatus='Absent' THEN 1 ELSE 0 END) AbsentCount FROM SessionAttendance WHERE TrainingID=@TrainingID AND SessionID=@SessionID";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),
                new SqlParameter("@SessionID",Session["SessionID"].ToString())
            };

            DataTable dt = obj.GetDataTable(query, param);

            int total = GetTotalTrainee();

            int present = 0;

            int absent = 0;

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["PresentCount"] != DBNull.Value)
                {
                    present = Convert.ToInt32(dt.Rows[0]["PresentCount"]);
                }

                if (dt.Rows[0]["AbsentCount"] != DBNull.Value)
                {
                    absent = Convert.ToInt32(dt.Rows[0]["AbsentCount"]);
                }
            }

            int pending = total - present - absent;

            decimal percent = 0;

            if (total > 0)
            {
                percent = (decimal)present * 100 / total;
            }

            lblTotal.Text = total.ToString();

            lblPresent.Text = present.ToString();

            lblAbsent.Text = absent.ToString();

            lblPending.Text = pending.ToString();

            lblAttendancePercent.Text = percent.ToString("0.00") + " %";
        }

        private int GetTotalTrainee()
        {
            string query = @"SELECT COUNT(*) FROM TrainingAssignment WHERE TrainingID=@TrainingID AND AssignmentStatus='Assigned'";

            SqlParameter[] param =
            {
                new SqlParameter("@TrainingID",Session["TrainingID"].ToString())
            };

            object objCount = obj.ExecuteScalar(query, param);

            if (objCount == null)
            {
                return 0;
            }

            return Convert.ToInt32(objCount);
        }

        protected void btnNormalAttendance_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            pnlNormalAttendance.Visible = true;


            pnlBulkAttendance.Visible = false;
        }

        protected void btnBulkAttendance_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            pnlNormalAttendance.Visible = false;

            pnlBulkAttendance.Visible = true;
        }
        protected void btnSaveAttendance_Click(object sender, EventArgs e)
        {
            if (!btnSaveAttendance.Enabled)
            {
                return;
            }
            foreach (GridViewRow row in gvAttendance.Rows)
            {
                string attendanceID = "";

                string empID = gvAttendance.DataKeys[row.RowIndex].Values["EmpID"].ToString().ToUpperInvariant();

                DropDownList ddlAttendance = (DropDownList)row.FindControl("ddlAttendance");

                TextBox txtRemarks = (TextBox)row.FindControl("txtRemarks");

                if (ddlAttendance.SelectedValue == "")
                {
                    continue;
                }

                string query = @"SELECT COUNT(*) FROM SessionAttendance WHERE SessionID=@SessionID AND EmpID=@EmpID";

                SqlParameter[] param =
                {
                    new SqlParameter("@SessionID",Session["SessionID"].ToString()),
                    new SqlParameter("@EmpID",empID)
                };

                int count = Convert.ToInt32(obj.ExecuteScalar(query, param));

                if (count == 0)
                {
                    attendanceID = GetAttendanceID();

                    query = @"INSERT INTO SessionAttendance(AttendanceID,SessionID,TrainingID,EmpID,AttendanceStatus,Remarks,CreatedOn,CreatedBy) VALUES(@AttendanceID,@SessionID,@TrainingID,@EmpID,@AttendanceStatus,@Remarks,GETDATE(),@CreatedBy)";

                    param = new SqlParameter[]
                    {
        new SqlParameter("@AttendanceID",attendanceID),
        new SqlParameter("@SessionID",Session["SessionID"].ToString()),
        new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),
        new SqlParameter("@EmpID",empID),
        new SqlParameter("@AttendanceStatus",ddlAttendance.SelectedValue),
        new SqlParameter("@Remarks",txtRemarks.Text.Trim()),
        new SqlParameter("@CreatedBy",Session["TrainerID"].ToString())
                    };

                    obj.ExecuteSql(query, param);
                }
                else
                {
                    query = @"UPDATE SessionAttendance SET AttendanceStatus=@AttendanceStatus,Remarks=@Remarks,ModifiedOn=GETDATE(),ModifiedBy=@ModifiedBy WHERE SessionID=@SessionID AND EmpID=@EmpID";

                    param = new SqlParameter[]
                    {
        new SqlParameter("@AttendanceStatus",ddlAttendance.SelectedValue),
        new SqlParameter("@Remarks",txtRemarks.Text.Trim()),
        new SqlParameter("@ModifiedBy",Session["TrainerID"].ToString()),
        new SqlParameter("@SessionID",Session["SessionID"].ToString()),
        new SqlParameter("@EmpID",empID)
                    };

                    obj.ExecuteSql(query, param);
                }
            }
            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Attendance saved successfully.";

            BindGrid();

            BindSummary();

            SessionSummary1.LoadSession(Session["SessionID"].ToString());

            CheckAttendanceStatus();

        }

        

        protected void btnDownloadSample_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Sample/AttendanceSample.xlsx");
        }

        protected void btnSessionDetails_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Trainer/SessionDetails.aspx");
        }


        private string GetAttendanceID()
        {
            string query = @"SELECT ISNULL(MAX(ID),0)+1 FROM SessionAttendance";

            object objValue = obj.ExecuteScalar(query);

            int id = 1;

            if (objValue != null)
            {
                id = Convert.ToInt32(objValue);
            }

            return "ATT" +
                   DateTime.Now.ToString("yyMMdd") +
                   id.ToString("00000");
        }
        protected void btnCompleteAttendance_Click(object sender, EventArgs e)
        {
            string query = @"SELECT COUNT(*) FROM TrainingAssignment TA LEFT JOIN SessionAttendance SA ON TA.EmpID=SA.EmpID AND TA.TrainingID=SA.TrainingID AND SA.SessionID=@SessionID WHERE TA.TrainingID=@TrainingID AND TA.AssignmentStatus='Assigned' AND SA.AttendanceStatus IS NULL";

            SqlParameter[] param =
            {
        new SqlParameter("@TrainingID",Session["TrainingID"].ToString()),
        new SqlParameter("@SessionID",Session["SessionID"].ToString())
    };

            object objPending = obj.ExecuteScalar(query, param);

            int pending = 0;

            if (objPending != null)
            {
                pending = Convert.ToInt32(objPending);
            }
            if (pending > 0)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Please mark attendance of all trainees.";

                return;
            }

            query = @"UPDATE SessionMaster SET AttendanceStatus='Completed',AttendanceCompletedOn=GETDATE(),AttendanceCompletedBy=@TrainerID WHERE SessionID=@SessionID";

            param = new SqlParameter[]
            {
        new SqlParameter("@TrainerID",Session["TrainerID"].ToString()),
        new SqlParameter("@SessionID",Session["SessionID"].ToString())
            };

            obj.ExecuteSql(query, param);

            UpdateTrainingAttendanceWorkflow();

            BindGrid();

            BindSummary();

            SessionSummary1.LoadSession(Session["SessionID"].ToString());

            CheckAttendanceStatus();

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Attendance completed successfully.";


        }





        protected void btnUploadAttendanceSheet_Click(object sender, EventArgs e)
        {
            if (!fuAttendanceSheet.HasFile)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Please select PDF file.";

                return;
            }

            string extension = System.IO.Path.GetExtension(fuAttendanceSheet.FileName).ToLower();

            if (extension != ".pdf")
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Only PDF file allowed.";

                return;
            }

            string folder = Server.MapPath("~/Uploads/AttendanceSheet/");

            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            string fileName =

            Session["SessionID"].ToString() +
            "_" +
            DateTime.Now.ToString("yyyyMMddHHmmss") +
            ".pdf";
            fuAttendanceSheet.SaveAs(folder + fileName);

            string query = @"UPDATE SessionMaster SET AttendanceSheet=@AttendanceSheet WHERE SessionID=@SessionID";

            SqlParameter[] param =
            {
        new SqlParameter("@AttendanceSheet",fileName),
        new SqlParameter("@SessionID",Session["SessionID"].ToString())
    };

            obj.ExecuteSql(query, param);

            lblMessage.ForeColor = System.Drawing.Color.Green;

            lblMessage.Text = "Attendance sheet uploaded successfully.";
        }



        private void UpdateTrainingAttendanceWorkflow()
        {
            string query = @"SELECT COUNT(*) FROM SessionMaster WHERE TrainingID=@TrainingID AND (AttendanceStatus IS NULL OR LTRIM(RTRIM(AttendanceStatus))<>'Completed')";

            SqlParameter[] param =
            {
        new SqlParameter("@TrainingID",Session["TrainingID"].ToString())
    };

            int pending =
                Convert.ToInt32(
                obj.ExecuteScalar(
                query,
                param));

            if
            (
                pending != 0
            )
            {
                return;
            }

            query = @"UPDATE TrainingDetails SET WorkflowStatus=@WorkflowStatus,TrainingStatus=@TrainingStatus,UpdatedOn=GETDATE(),UpdatedBy=@UpdatedBy WHERE TrainingID=@TrainingID";

            param =
                new SqlParameter[]
                {
            new SqlParameter("@WorkflowStatus","ABCDEF"),
            new SqlParameter("@TrainingStatus","AttendanceCompleted"),
            new SqlParameter("@UpdatedBy",Session["TrainerID"].ToString()),
            new SqlParameter("@TrainingID",Session["TrainingID"].ToString())
                };

            obj.ExecuteSql(
                query,
                param);
            query = @"UPDATE TP SET AttendanceCompleted=1, WorkflowStatus='F', UpdatedOn=GETDATE(), UpdatedBy=@UpdatedBy FROM TrainingProgress TP WHERE TP.TrainingID=@TrainingID AND EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.TrainingID=TP.TrainingID AND SA.EmpID=TP.EmpID)";
            param =
               new SqlParameter[]
               {
                new SqlParameter("@UpdatedBy", Session["TrainerID"].ToString()),
                    new SqlParameter("@TrainingID", Session["TrainingID"].ToString())

            
               };

            obj.ExecuteSql(
                query,
                param);
        }
        protected void btnUploadExcel_Click(object sender, EventArgs e)
        {
            if (!fuAttendanceExcel.HasFile)
            {
                lblMessage.ForeColor = System.Drawing.Color.Red;

                lblMessage.Text = "Please select Excel file.";

                return;
            }

            string extension =
                Path.GetExtension(
                    fuAttendanceExcel.FileName).ToLower();

            if (extension != ".xlsx")
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Only Excel (.xlsx) file allowed.";

                return;
            }

            ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;

            using (ExcelPackage package =
                new ExcelPackage(
                    fuAttendanceExcel.PostedFile.InputStream))
            {
                if (package.Workbook.Worksheets.Count == 0)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Worksheet not found.";

                    return;
                }

                ExcelWorksheet ws =
                    package.Workbook.Worksheets[0];

                if (ws.Dimension == null)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Excel file is empty.";

                    return;
                }

                int totalRow =
                    ws.Dimension.End.Row;

                int totalColumn =
                    ws.Dimension.End.Column;

                if (totalColumn < 2)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Invalid Excel format.";

                    return;
                }

                string col1 =
                    ws.Cells[1, 1].Text.Trim();

                string col2 =
                    ws.Cells[1, 2].Text.Trim();

                if (col1 != "EmpID" ||
                    col2 != "AttendanceStatus")
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Excel format is invalid.";

                    return;
                }

                DataTable dtExcel =
    new DataTable();

                dtExcel.Columns.Add("EmpID");

                dtExcel.Columns.Add("AttendanceStatus");

                for (int i = 2; i <= totalRow; i++)
                {
                    string empID =
                        ws.Cells[i, 1].Text.Trim().ToUpperInvariant();

                    string attendance =
                        ws.Cells[i, 2].Text.Trim();

                    if (empID == "")
                    {
                        continue;
                    }

                    if (attendance != "Present" &&
                        attendance != "Absent")
                    {
                        lblMessage.ForeColor =
                            System.Drawing.Color.Red;

                        lblMessage.Text =
                            "Invalid Attendance Status at Row : " +
                            i.ToString();

                        return;
                    }

                    string query =
                        @"SELECT COUNT(*) FROM TrainingAssignment
                  WHERE TrainingID=@TrainingID
                  AND AssignmentStatus='Assigned'
                  AND EmpID=@EmpID";

                    SqlParameter[] param =
                    {
                new SqlParameter(
                    "@TrainingID",
                    Session["TrainingID"].ToString()),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };

                    object objCount =
                        obj.ExecuteScalar(
                            query,
                            param);

                    int count = 0;

                    if (objCount != null)
                    {
                        count =
                            Convert.ToInt32(
                                objCount);
                    }

                    if (count == 0)
                    {
                        lblMessage.ForeColor =
                            System.Drawing.Color.Red;

                        lblMessage.Text =
                            "Employee " +
                            empID +
                            " is not assigned in this training.";

                        return;
                    }

                    DataRow dr =
                        dtExcel.NewRow();

                    dr["EmpID"] =
                        empID;

                    dr["AttendanceStatus"] =
                        attendance;

                    dtExcel.Rows.Add(dr);
                }

                if (dtExcel.Rows.Count == 0)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "No attendance found in Excel.";

                    return;
                }

                // Part-3

                foreach (DataRow dr in dtExcel.Rows)
                {
                    string empID =
                        dr["EmpID"].ToString();

                    string attendance =
                        dr["AttendanceStatus"].ToString();

                    string attendanceID = "";

                    string query =
                        @"SELECT COUNT(*) FROM SessionAttendance
                  WHERE SessionID=@SessionID
                  AND EmpID=@EmpID";

                    SqlParameter[] param =
                    {
                new SqlParameter(
                    "@SessionID",
                    Session["SessionID"].ToString()),

                new SqlParameter(
                    "@EmpID",
                    empID)
            };

                    object objCount =
                        obj.ExecuteScalar(
                            query,
                            param);

                    int count = 0;

                    if (objCount != null)
                    {
                        count =
                            Convert.ToInt32(
                                objCount);
                    }

                    if (count == 0)
                    {
                        attendanceID =
                            GetAttendanceID();

                        query =
                            @"INSERT INTO SessionAttendance
                    (
                        AttendanceID,
                        SessionID,
                        TrainingID,
                        EmpID,
                        AttendanceStatus,
                        Remarks,
                        CreatedOn,
                        CreatedBy
                    )
                    VALUES
                    (
                        @AttendanceID,
                        @SessionID,
                        @TrainingID,
                        @EmpID,
                        @AttendanceStatus,
                        '',
                        GETDATE(),
                        @CreatedBy
                    )";

                        param =
                            new SqlParameter[]
                            {
                        new SqlParameter(
                            "@AttendanceID",
                            attendanceID),

                        new SqlParameter(
                            "@SessionID",
                            Session["SessionID"].ToString()),

                        new SqlParameter(
                            "@TrainingID",
                            Session["TrainingID"].ToString()),

                        new SqlParameter(
                            "@EmpID",
                            empID),

                        new SqlParameter(
                            "@AttendanceStatus",
                            attendance),

                        new SqlParameter(
                            "@CreatedBy",
                            Session["TrainerID"].ToString())
                            };

                        obj.ExecuteSql(
                            query,
                            param);
                    }
                    else
                    {
                        query =
                            @"UPDATE SessionAttendance
                    SET
                    AttendanceStatus=@AttendanceStatus,
                    ModifiedOn=GETDATE(),
                    ModifiedBy=@ModifiedBy
                    WHERE
                    SessionID=@SessionID
                    AND EmpID=@EmpID";

                        param =
                            new SqlParameter[]
                            {
                        new SqlParameter(
                            "@AttendanceStatus",
                            attendance),

                        new SqlParameter(
                            "@ModifiedBy",
                            Session["TrainerID"].ToString()),

                        new SqlParameter(
                            "@SessionID",
                            Session["SessionID"].ToString()),

                        new SqlParameter(
                            "@EmpID",
                            empID)
                            };

                        obj.ExecuteSql(
                            query,
                            param);
                    }
                }

                BindGrid();

                BindSummary();

                SessionSummary1.LoadSession(
                    Session["SessionID"].ToString());

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                lblMessage.Text =
                    "Attendance uploaded successfully.";
            }
        }
    }
}