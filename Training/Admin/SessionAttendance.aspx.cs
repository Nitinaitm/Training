using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using OfficeOpenXml;

namespace Training.Admin
{
    public partial class SessionAttendance : System.Web.UI.Page
    {
        string constr =
            ConfigurationManager
            .ConnectionStrings["constr"]
            .ConnectionString;

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainingID"] == null)
                {
                    Response.Redirect(
                        "TrainingList.aspx");

                    return;
                }

                if (Session["SessionID"] == null)
                {
                    Response.Redirect(
                        "TrainingAttendance.aspx");

                    return;
                }

                TrainingSummary1.LoadTraining(
                    Session["TrainingID"].ToString());
                using (SqlConnection con =
    new SqlConnection(constr))
                {
                    SqlCommand cmd =
                    new SqlCommand(@"

SELECT WorkflowStatus

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                    cmd.Parameters.AddWithValue(
                        "@TrainingID",
                        Session["TrainingID"]);

                    con.Open();

                    string workflow =
                        Convert.ToString(
                        cmd.ExecuteScalar());

                    con.Close();

                    if (String.IsNullOrWhiteSpace(workflow) ||
                        !workflow.Contains("E"))
                    {
                        lblMessage.ForeColor =
                            Color.Red;

                        lblMessage.Text =
                            "Training has not started yet. Attendance cannot be marked.";

                        gvAttendance.Visible = false;

                        btnSaveAttendance.Visible = false;

                        btnPresentAll.Visible = false;

                        btnUploadExcel.Visible = false;

                        btnUploadAttendanceSheet.Visible = false;

                        return;
                    }
                }
                LoadSessionSummary();

                BindEmployeeGrid();

                BindAttendanceSummary();


                pnlManual.Visible = true;

                    pnlBulk.Visible = false;

                    ViewState["Mode"] = "MANUAL";
                
                
            }
            else
            {
                RestoreMode();
            }
        }
        private void RestoreMode()
        {
            if (ViewState["Mode"] == null)
                return;

            pnlManual.Visible = false;
            pnlBulk.Visible = false;

            switch (ViewState["Mode"].ToString())
            {
                case "MANUAL":

                    pnlManual.Visible = true;

                    break;

                case "BULK":

                    pnlBulk.Visible = true;

                    break;
            }
        }

        protected void btnManualAttendance_Click(
object sender,
EventArgs e)
        {
            pnlManual.Visible = true;

            pnlBulk.Visible = false;

            ViewState["Mode"] = "MANUAL";
        }

        protected void btnBulkAttendance_Click(
object sender,
EventArgs e)
        {
            pnlManual.Visible = false;

            pnlBulk.Visible = true;

            ViewState["Mode"] = "BULK";
        }
        //---------------------------------------------------
        // Session Summary
        //---------------------------------------------------

        private void LoadSessionSummary()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

SM.SessionNo,
SM.SessionName,
SM.SessionDate,
SM.StartTime,
SM.EndTime,
SM.TotalHours,
SM.AttendanceStatus,

TM.TopicName,

CASE
WHEN TR.TrainerType='Internal'
THEN E.EmpName
ELSE TR.NameExternal
END TrainerName

FROM SessionMaster SM

LEFT JOIN TopicMaster TM
ON TM.TopicID=SM.TopicID

LEFT JOIN TrainerMaster TR
ON TR.TrainerID=SM.TrainerID

LEFT JOIN EmpBasicMaster E
ON E.EmpID=TR.EmpID

WHERE SM.SessionID=@SessionID

", con);

                cmd.Parameters.AddWithValue(
                    "@SessionID",
                    Session["SessionID"]);

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblSessionNo.Text =
                        dr["SessionNo"].ToString();

                    lblSessionName.Text =
                        dr["SessionName"].ToString();

                    lblSessionDate.Text =
                        Convert.ToDateTime(
                        dr["SessionDate"])
                        .ToString("dd-MM-yyyy");

                    lblTopic.Text =
                        dr["TopicName"].ToString();

                    lblTrainer.Text =
                        dr["TrainerName"].ToString();

                    lblStartTime.Text =
                        dr["StartTime"].ToString();

                    lblEndTime.Text =
                        dr["EndTime"].ToString();

                    lblHours.Text =
                        dr["TotalHours"].ToString();

                    lblAttendanceStatus.Text =
                        dr["AttendanceStatus"].ToString();
                }

                dr.Close();

                con.Close();
            }
        }

        //---------------------------------------------------
        // Employee Grid
        //---------------------------------------------------

        private void BindEmployeeGrid()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT

TA.EmpID,

E.EmpName,

E.EmpDesignation,

E.EmpCompany

FROM TrainingAssignment TA

INNER JOIN EmpBasicMaster E

ON TA.EmpID=E.EmpID

WHERE

TA.TrainingID=@TrainingID

AND

TA.AssignmentStatus='Assigned'

ORDER BY E.EmpName

", con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                DataTable dt =
                    new DataTable();

                da.Fill(dt);

                gvAttendance.DataSource =
                    dt;

                gvAttendance.DataBind();
            }

            LoadAlreadyMarkedAttendance();
        }

        //---------------------------------------------------
        // Load Existing Attendance
        //---------------------------------------------------

        private void LoadAlreadyMarkedAttendance()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                foreach (GridViewRow row in
                    gvAttendance.Rows)
                {
                    string empid =
                        row.Cells[0].Text.ToUpperInvariant();

                    SqlCommand cmd =
                        new SqlCommand(@"

SELECT

AttendanceStatus,
Remarks

FROM SessionAttendance

WHERE

SessionID=@SessionID

AND

EmpID=@EmpID

", con);

                    cmd.Parameters.AddWithValue(
                        "@SessionID",
                        Session["SessionID"]);

                    cmd.Parameters.AddWithValue(
                        "@EmpID",
                        empid);

                    SqlDataReader dr =
                        cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        DropDownList ddl =
                            (DropDownList)row.FindControl(
                            "ddlAttendance");

                        TextBox txt =
                            (TextBox)row.FindControl(
                            "txtRemarks");

                        ddl.SelectedValue =
                            dr["AttendanceStatus"].ToString();

                        txt.Text =
                            dr["Remarks"].ToString();
                    }

                    dr.Close();
                }

                con.Close();
            }

            CheckReadOnlyMode();
        }

        //---------------------------------------------------
        // Read Only Mode
        //---------------------------------------------------

        private void CheckReadOnlyMode()
        {
            bool completed =
                lblAttendanceStatus.Text
                .Equals(
                "Completed",
                StringComparison.OrdinalIgnoreCase);

            btnSaveAttendance.Visible =
                !completed;

            btnPresentAll.Visible =
                !completed;

            btnUploadExcel.Visible =
                !completed;

            btnUploadAttendanceSheet.Visible =
                !completed;

            fuExcel.Visible =
                !completed;

            fuAttendanceSheet.Visible =
                !completed;

            foreach (GridViewRow row in
                gvAttendance.Rows)
            {
                DropDownList ddl =
                (DropDownList)
                row.FindControl(
                "ddlAttendance");

                TextBox txt =
                (TextBox)
                row.FindControl(
                "txtRemarks");

                ddl.Enabled =
                    !completed;

                txt.ReadOnly =
                    completed;
            }
        }
        protected void btnPresentAll_Click(
    object sender,
    EventArgs e)
        {
            foreach (GridViewRow row in gvAttendance.Rows)
            {
                DropDownList ddl =
                    (DropDownList)row.FindControl(
                    "ddlAttendance");

                if (ddl != null)
                {
                    ddl.SelectedValue = "Present";
                }
            }
        }
        protected void btnSaveAttendance_Click(
    object sender,
    EventArgs e)
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran =
                    con.BeginTransaction();

                try
                {
                    foreach (GridViewRow row in
                        gvAttendance.Rows)
                    {
                        string empid =
                            row.Cells[0].Text.Trim().ToUpperInvariant();

                        DropDownList ddl =
                            (DropDownList)row.FindControl(
                            "ddlAttendance");

                        TextBox txt =
                            (TextBox)row.FindControl(
                            "txtRemarks");

                        SaveAttendance(
                            con,
                            tran,
                            empid,
                            ddl.SelectedValue,
                            txt.Text.Trim());
                    }

                    tran.Commit();

                    CompleteSessionAttendance();

                    lblMessage.ForeColor =
                        Color.Green;

                    lblMessage.Text =
                        "Attendance Marked Successfully.";

                    lblAttendanceStatus.Text =
    "Completed";

                    LoadSessionSummary();

                    BindEmployeeGrid();

                    BindAttendanceSummary();

                    CheckReadOnlyMode();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.ForeColor =
                        Color.Red;

                    lblMessage.Text =
                        ex.Message;
                }
            }
        }
        private void SaveAttendance(
    SqlConnection con,
    SqlTransaction tran,
    string empid,
    string attendance,
    string remarks)
        {
            //-----------------------------------
            // Already Exists ?
            //-----------------------------------

            SqlCommand chk =
                new SqlCommand(@"

SELECT COUNT(*)

FROM SessionAttendance

WHERE

SessionID=@SessionID

AND

EmpID=@EmpID

", con, tran);

            chk.Parameters.AddWithValue(
                "@SessionID",
                Session["SessionID"]);

            chk.Parameters.AddWithValue(
                "@EmpID",
                empid);

            int cnt =
                Convert.ToInt32(
                chk.ExecuteScalar());

            //-----------------------------------

            if (cnt == 0)
            {
                SqlCommand cmd =
                    new SqlCommand(@"

INSERT INTO

SessionAttendance
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
@Remarks,
GETDATE(),
@CreatedBy
)

", con, tran);

                cmd.Parameters.AddWithValue(
                    "@AttendanceID",
                    Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 18)
                    .ToUpper());

                cmd.Parameters.AddWithValue(
                    "@SessionID",
                    Session["SessionID"]);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                cmd.Parameters.AddWithValue(
                    "@EmpID",
                    empid);

                cmd.Parameters.AddWithValue(
                    "@AttendanceStatus",
                    attendance);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    remarks);

                cmd.Parameters.AddWithValue(
                    "@CreatedBy",
                    Session["UserID"] == null
                    ? "Admin"
                    : Session["UserID"].ToString());

                cmd.ExecuteNonQuery();
            }
            else
            {
                SqlCommand cmd =
                    new SqlCommand(@"

UPDATE SessionAttendance

SET

AttendanceStatus=@AttendanceStatus,

Remarks=@Remarks,

ModifiedOn=GETDATE(),

ModifiedBy=@ModifiedBy

WHERE

SessionID=@SessionID

AND

EmpID=@EmpID

", con, tran);

                cmd.Parameters.AddWithValue(
                    "@AttendanceStatus",
                    attendance);

                cmd.Parameters.AddWithValue(
                    "@Remarks",
                    remarks);

                cmd.Parameters.AddWithValue(
                    "@ModifiedBy",
                    Session["UserID"] == null
                    ? "Admin"
                    : Session["UserID"].ToString());

                cmd.Parameters.AddWithValue(
                    "@SessionID",
                    Session["SessionID"]);

                cmd.Parameters.AddWithValue(
                    "@EmpID",
                    empid);

                cmd.ExecuteNonQuery();
            }
        }
        private void CompleteSessionAttendance()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

UPDATE SessionMaster

SET

AttendanceStatus='Completed',

AttendanceCompletedOn=GETDATE(),

AttendanceCompletedBy=@User

WHERE SessionID=@SessionID

", con);

                cmd.Parameters.AddWithValue(
                    "@SessionID",
                    Session["SessionID"]);

                cmd.Parameters.AddWithValue(
                    "@User",
                    Session["UserID"] == null
                    ? "Admin"
                    : Session["UserID"].ToString());

                cmd.ExecuteNonQuery();

                con.Close();
            }
        }
        protected void btnBack_Click(
object sender,
EventArgs e)
        {
            Response.Redirect(
                "TrainingAttendance.aspx");
        }
        protected void btnDownloadExcel_Click(
object sender,
EventArgs e)
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT

TA.EmpID,

E.EmpName,

'Present' AS Attendance,

'' AS Remarks

FROM TrainingAssignment TA

INNER JOIN EmpBasicMaster E

ON TA.EmpID=E.EmpID

WHERE

TA.TrainingID=@TrainingID

AND

TA.AssignmentStatus='Assigned'

ORDER BY E.EmpName

", con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                DataTable dt =
                    new DataTable();

                da.Fill(dt);

                ExcelPackage.LicenseContext =
                    LicenseContext.NonCommercial;

                using (ExcelPackage package =
                    new ExcelPackage())
                {
                    ExcelWorksheet ws =
                        package.Workbook.Worksheets.Add(
                        "Attendance");

                    ws.Cells["A1"]
                        .LoadFromDataTable(
                        dt,
                        true);

                    ws.Cells.AutoFitColumns();

                    Response.Clear();

                    Response.ContentType =
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    Response.AddHeader(
                    "content-disposition",
                    "attachment; filename=AttendanceFormat.xlsx");

                    Response.BinaryWrite(
                    package.GetAsByteArray());

                    Response.End();
                }
            }
        }
        protected void btnUploadExcel_Click(
object sender,
EventArgs e)
        {
            lblMessage.Text = "";

            if (!fuExcel.HasFile)
            {
                lblMessage.ForeColor =
                    Color.Red;

                lblMessage.Text =
                    "Please select Excel file.";

                return;
            }

            string ext =
                System.IO.Path.GetExtension(
                fuExcel.FileName)
                .ToLower();

            if (ext != ".xlsx")
            {
                lblMessage.ForeColor =
                    Color.Red;

                lblMessage.Text =
                    "Only Excel (.xlsx) file allowed.";

                return;
            }

            DataTable dt =
                new DataTable();

            ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;

            using (ExcelPackage package =
                new ExcelPackage(
                fuExcel.PostedFile.InputStream))
            {
                ExcelWorksheet ws =
                    package.Workbook.Worksheets[0];

                int rows =
                    ws.Dimension.End.Row;

                dt.Columns.Add("EmpID");
                dt.Columns.Add("Attendance");
                dt.Columns.Add("Remarks");

                for (int i = 2;
                    i <= rows;
                    i++)
                {
                    dt.Rows.Add(

                    ws.Cells[i, 1].Text.Trim(),

                    ws.Cells[i, 3].Text.Trim(),

                    ws.Cells[i, 4].Text.Trim()

                    );
                }
            }

            SaveBulkAttendance(dt);
        }
        private void SaveBulkAttendance(
DataTable dt)
        {
            int success = 0;

            int skipped = 0;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran =
                    con.BeginTransaction();

                try
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string empid =
                            dr["EmpID"].ToString().ToUpperInvariant();

                        string attendance =
                            dr["Attendance"].ToString();

                        string remarks =
                            dr["Remarks"].ToString();

                        //---------------------------------

                        if (attendance != "Present" &&
                            attendance != "Absent")
                        {
                            skipped++;

                            continue;
                        }

                        //---------------------------------

                        SaveAttendance(
                            con,
                            tran,
                            empid,
                            attendance,
                            remarks);

                        success++;
                    }

                    tran.Commit();

                    CompleteSessionAttendance();

                    LoadSessionSummary();

                    BindEmployeeGrid();

                    BindAttendanceSummary();

                   
                    lblMessage.ForeColor =
                        Color.Green;

                    lblMessage.Text =
                        success +
                        " Attendance Updated.";

                    if (skipped > 0)
                    {
                        lblMessage.Text +=

                        "  " +

                        skipped +

                        " Skipped.";
                    }
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.ForeColor =
                        Color.Red;

                    lblMessage.Text =
                        ex.Message;
                }
            }
        }

        protected void btnUploadAttendanceSheet_Click(
object sender,
EventArgs e)
        {
            if (!fuAttendanceSheet.HasFile)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Please select PDF.";
                return;
            }

            string ext =
                System.IO.Path.GetExtension(
                fuAttendanceSheet.FileName).ToLower();

            if (ext != ".pdf")
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Only PDF allowed.";
                return;
            }

            string folder =
                Server.MapPath("~/AttendanceSheet/");

            if (!System.IO.Directory.Exists(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
            }

            string fileName =
                Session["SessionID"].ToString()
                + ".pdf";

            string path =
                folder + fileName;

            fuAttendanceSheet.SaveAs(path);

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

UPDATE SessionMaster

SET AttendanceSheet=@File

WHERE SessionID=@SessionID

", con);

                cmd.Parameters.AddWithValue(
                    "@File",
                    "~/AttendanceSheet/" + fileName);

                cmd.Parameters.AddWithValue(
                    "@SessionID",
                    Session["SessionID"]);

                con.Open();

                cmd.ExecuteNonQuery();

                con.Close();
            }

            lblMessage.ForeColor =
                Color.Green;

            lblMessage.Text =
                "Attendance Sheet Uploaded Successfully.";
        }
        private void BindAttendanceSummary()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

(SELECT COUNT(*)
 FROM TrainingAssignment
 WHERE TrainingID=@TrainingID
 AND AssignmentStatus='Assigned') AS Total,

(SELECT COUNT(*)
 FROM SessionAttendance
 WHERE SessionID=@SessionID
 AND AttendanceStatus='Present') AS Present,

(SELECT COUNT(*)
 FROM SessionAttendance
 WHERE SessionID=@SessionID
 AND AttendanceStatus='Absent') AS Absent

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                cmd.Parameters.AddWithValue(
                    "@SessionID",
                    Session["SessionID"]);

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    int total =
                        Convert.ToInt32(dr["Total"]);

                    int present =
                        Convert.ToInt32(dr["Present"]);

                    int absent =
                        Convert.ToInt32(dr["Absent"]);

                    int pending =
                        total - (present + absent);

                    if (pending < 0)
                        pending = 0;

                    lblTotalTrainee.Text =
                        total.ToString();

                    lblPresent.Text =
                        present.ToString();

                    lblAbsent.Text =
                        absent.ToString();

                    lblPending.Text =
                        pending.ToString();

                    decimal per = 0;

                    if (total > 0)
                    {
                        per =
                            (decimal)present * 100 / total;
                    }

                    lblAttendancePercent.Text =
                        per.ToString("0.00") + "%";
                }

                dr.Close();
            }
        }
    }
}