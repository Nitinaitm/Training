using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using OfficeOpenXml;
using System.Text;

namespace Training.Admin
{
    public partial class AssignTrainee :
        System.Web.UI.Page
    {
        string constr =
        ConfigurationManager
        .ConnectionStrings["constr"]
        .ConnectionString;

        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            if (Session["TrainingID"] == null)
            {
                Response.Redirect("TrainingList.aspx");
                return;
            }
            TrainingSummary1.LoadTraining(Session["TrainingID"].ToString());
            if (!IsPostBack)
            {

                //LoadBatchSummary();

                BindCompany();

                BindAssignedEmployee();

                SetButtonStatus();

                pnlEmpWise.Visible = true;

                pnlBulk.Visible = false;

                pnlCompany.Visible = false;

                ViewState["Mode"] = "EMP";
            }
            else
            {
                RestoreMode();
            }
            LoadPlugins();

        }
        private void RestoreMode()
        {
            if (ViewState["Mode"] == null)
                return;

            string mode =
            ViewState["Mode"]
            .ToString();

            pnlEmpWise.Visible = false;

            pnlBulk.Visible = false;

            pnlCompany.Visible = false;

            switch (mode)
            {
                case "EMP":

                    pnlEmpWise.Visible =
                    true;

                    break;

                case "COMPANY":

                    pnlCompany.Visible =
                    true;

                    break;

                case "BULK":

                    pnlBulk.Visible =
                    true;

                    break;
            }
        }


     
        private void BindCompany()
        {
            lstCompany.Items.Clear();

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT DISTINCT
EmpCompany

FROM EmpBasicMaster

WHERE ISNULL(EmpCompany,'')<>''

ORDER BY EmpCompany", con);

                con.Open();

                SqlDataReader dr =
                cmd.ExecuteReader();

                lstCompany.Items.Add(
                new ListItem(
                "ALL COMPANIES",
                "ALL"));

                while (dr.Read())
                {
                    lstCompany.Items.Add(
                    new ListItem(
                    dr["EmpCompany"].ToString(),
                    dr["EmpCompany"].ToString()));
                }

                con.Close();
            }
        }

        private void BindDesignation()
        {
            lstDesignation.Items.Clear();

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                StringBuilder query =
                new StringBuilder();

                query.Append(@"

SELECT DISTINCT
EmpDesignation

FROM EmpBasicMaster

WHERE ISNULL(EmpDesignation,'')<>''

");

                SqlCommand cmd =
                new SqlCommand();

                cmd.Connection = con;

                List<string> company =
                new List<string>();

                int i = 0;

                bool all = false;

                foreach (ListItem item in lstCompany.Items)
                {
                    if (item.Selected)
                    {
                        if (item.Value == "ALL")
                        {
                            all = true;
                            break;
                        }

                        string pname =
                        "@Comp" + i;

                        company.Add(pname);

                        cmd.Parameters.AddWithValue(
                        pname,
                        item.Value);

                        i++;
                    }
                }

                if (!all &&
                    company.Count > 0)
                {
                    query.Append(" AND EmpCompany IN(");

                    query.Append(
                    string.Join(",", company));

                    query.Append(")");
                }

                query.Append(" ORDER BY EmpDesignation");

                cmd.CommandText =
                query.ToString();

                con.Open();

                SqlDataReader dr =
                cmd.ExecuteReader();

                while (dr.Read())
                {
                    lstDesignation.Items.Add(
                    new ListItem(
                    dr["EmpDesignation"].ToString(),
                    dr["EmpDesignation"].ToString()));
                }

                con.Close();
            }
        }

        private void BindPostingPlace()
        {
            lstPostingPlace.Items.Clear();

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                StringBuilder query =
                new StringBuilder();

                query.Append(@"

SELECT DISTINCT
EmpPostingPlace

FROM EmpBasicMaster

WHERE ISNULL(EmpPostingPlace,'')<>''

");

                SqlCommand cmd =
                new SqlCommand();

                cmd.Connection = con;

                List<string> company =
                new List<string>();

                int i = 0;

                bool all = false;

                foreach (ListItem item in lstCompany.Items)
                {
                    if (item.Selected)
                    {
                        if (item.Value == "ALL")
                        {
                            all = true;
                            break;
                        }

                        string pname =
                        "@Comp" + i;

                        company.Add(pname);

                        cmd.Parameters.AddWithValue(
                        pname,
                        item.Value);

                        i++;
                    }
                }

                if (!all &&
                    company.Count > 0)
                {
                    query.Append(" AND EmpCompany IN(");

                    query.Append(
                    string.Join(",", company));

                    query.Append(")");
                }

                query.Append(" ORDER BY EmpPostingPlace");

                cmd.CommandText =
                query.ToString();

                con.Open();

                SqlDataReader dr =
                cmd.ExecuteReader();

                while (dr.Read())
                {
                    lstPostingPlace.Items.Add(
                    new ListItem(
                    dr["EmpPostingPlace"].ToString(),
                    dr["EmpPostingPlace"].ToString()));
                }

                con.Close();
            }
        }

        protected void lstCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> selectedDesignation = lstDesignation.Items
                                                .Cast<ListItem>()
                                                .Where(x => x.Selected)
                                                .Select(x => x.Value)
                                                .ToList();

            List<string> selectedPosting = lstPostingPlace.Items
                                            .Cast<ListItem>()
                                            .Where(x => x.Selected)
                                            .Select(x => x.Value)
                                            .ToList();

            BindDesignation();
            BindPostingPlace();

            foreach (ListItem item in lstDesignation.Items)
            {
                if (selectedDesignation.Contains(item.Value))
                    item.Selected = true;
            }

            foreach (ListItem item in lstPostingPlace.Items)
            {
                if (selectedPosting.Contains(item.Value))
                    item.Selected = true;
            }
        }

        protected void btnEmpWise_Click(
object sender,
EventArgs e)
        {
            pnlEmpWise.Visible = true;

            pnlCompany.Visible = false;

            pnlBulk.Visible = false;

            ViewState["Mode"] = "EMP";
        }

        protected void btnCompanyWise_Click(
        object sender,
        EventArgs e)
        {
            pnlEmpWise.Visible = false;

            pnlCompany.Visible = true;

            pnlBulk.Visible = false;

            ViewState["Mode"] = "COMPANY";
        }

        protected void btnBulkWise_Click(
        object sender,
        EventArgs e)
        {
            pnlEmpWise.Visible = false;

            pnlCompany.Visible = false;

            pnlBulk.Visible = true;

            ViewState["Mode"] = "BULK";
        }
        private bool AlreadyAssignedSameCourse(
string empID)
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string query = @"

SELECT COUNT(*)

FROM TrainingAssignment TA

INNER JOIN TrainingDetails TD

ON TA.TrainingID = TD.TrainingID

WHERE

TA.EmpID = @EmpID

AND

TA.AssignmentStatus <> 'Cancelled'

AND

TD.CourseID =
(
SELECT CourseID

FROM TrainingDetails

WHERE TrainingID = @TrainingID
)

AND

TA.TrainingID <> @TrainingID

";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                "@EmpID",
                empID);

                cmd.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                con.Open();

                bool exists =
                Convert.ToInt32(
                cmd.ExecuteScalar()) > 0;

                con.Close();

                return exists;
            }
        }
        protected void btnAddEmployee_Click(
        object sender,
        EventArgs e)
        {
            lblEmpMessage.Text = "";

            if (txtEmpID.Text.Trim() == "")
            {
                lblEmpMessage.Text =
                "Enter Employee ID.";

                return;
            }

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                //------------------------------------
                // Employee Exists
                //------------------------------------

                SqlCommand cmdEmp =
                new SqlCommand(@"

SELECT

EmpID

FROM EmpBasicMaster

WHERE EmpID=@EmpID

", con);

                cmdEmp.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim());

                object objEmp =
                cmdEmp.ExecuteScalar();

                if (objEmp == null)
                {
                    lblEmpMessage.Text =
                    "Employee not found.";

                    con.Close();

                    return;
                }

                //------------------------------------
                // Already Assigned
                //------------------------------------

                SqlCommand cmdDup =
                new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment

WHERE TrainingID=@TrainingID

AND EmpID=@EmpID

", con);

                cmdDup.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                cmdDup.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim());

                int cnt =
                Convert.ToInt32(
                cmdDup.ExecuteScalar());

                if (cnt > 0)
                {
                    lblEmpMessage.Text =
                    "Employee already assigned.";

                    con.Close();

                    return;
                }

                //------------------------------------
                // Same Topic Already Attended
                //------------------------------------

                SqlCommand cmdTopic =
                new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment TA

INNER JOIN TrainingDetails TD
ON TA.TrainingID = TD.TrainingID

WHERE
TA.EmpID = @EmpID

AND TD.CourseID =
(
    SELECT CourseID
    FROM TrainingDetails
    WHERE TrainingID = @TrainingID
)

AND ISNULL(TA.AssignmentStatus,'Assigned') <> 'Cancelled'

AND TA.TrainingID <> @TrainingID", con);

                cmdTopic.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim());

                cmdTopic.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                int topicCnt =
                Convert.ToInt32(
                cmdTopic.ExecuteScalar());

                if (topicCnt > 0)
                {
                    lblEmpMessage.Text =
                    "Employee is already assigned for this Course in another batch.";

                    con.Close();

                    return;
                }
                if (IsBatchFull(con, null))
                {
                    lblEmpMessage.ForeColor =
                    Color.Red;

                    lblEmpMessage.Text =
                    "Batch Strength Reached.";

                    con.Close();

                    return;
                }
                //------------------------------------
                // Save
                //------------------------------------

                SqlCommand cmd =
                new SqlCommand(@"

INSERT INTO

TrainingAssignment
(
AssignmentID,
TrainingID,
EmpID,
TrainingAttended,
CreatedOn,
CreatedBy,
AssignmentMode,
AssignmentStatus,
Remarks
)

VALUES
(
@AssignmentID,
@TrainingID,
@EmpID,
'Pending',
GETDATE(),
'Admin',
@AssignmentMode,
'Assigned',
''
)

", con);

                cmd.Parameters.AddWithValue(
                "@AssignmentID",
                GenerateAssignmentID());

                cmd.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                cmd.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim().ToUpperInvariant());
                cmd.Parameters.AddWithValue(
"@AssignmentMode",
ViewState["Mode"].ToString());
                cmd.ExecuteNonQuery();
                SqlCommand cmd1 =
new SqlCommand(@"

INSERT INTO TrainingProgress
(
ProgressID,
TrainingID,
EmpID,
CreatedOn,
CreatedBy
)

VALUES
(
@ProgressID,
@TrainingID,
@EmpID,
GETDATE(),
@CreatedBy
)

", con);

                cmd1.Parameters.AddWithValue(
                "@ProgressID",
                GenerateProgressID());

                cmd1.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                cmd1.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim().ToUpperInvariant());

                cmd1.Parameters.AddWithValue(
                "@CreatedBy",
                "Admin");

                cmd1.ExecuteNonQuery();
                con.Close();
            }

            


            txtEmpID.Text = "";

            BindAssignedEmployee();


            //LoadBatchSummary();
            lblEmpMessage.ForeColor =
            System.Drawing.Color.Green;

            lblEmpMessage.Text =
            "Employee assigned successfully.";
        }

        private void BindAssignedEmployee()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                string query = @"

SELECT

TA.AssignmentID,

TA.EmpID,

TA.AssignmentStatus,

E.EmpName,

E.EmpDesignation,

E.EmpCompany,

E.EmpPostingPlace

FROM TrainingAssignment TA

INNER JOIN EmpBasicMaster E

ON TA.EmpID=E.EmpID

WHERE TA.TrainingID=@TrainingID

AND ISNULL(TA.AssignmentStatus,'Assigned')='Assigned'
ORDER BY TA.ID";

                SqlCommand cmd =
                new SqlCommand(query, con);

                cmd.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvAssignedEmployee.DataSource =
                dt;

                gvAssignedEmployee.DataBind();

                if (dt.Rows.Count > 0)
                {
                    messageTotalAssigned.Visible = true;

                    messageTotalAssigned.InnerText =
                        "Total Trainee Assigned : " + dt.Rows.Count;
                }
                else
                {
                    messageTotalAssigned.Visible = false;
                }
            }
        }

        protected void gvAssignedEmployee_RowCommand(
        object sender,
        GridViewCommandEventArgs e)
        {
            if (e.CommandName == "RemoveEmployee")
            {
                using (SqlConnection con =
                    new SqlConnection(constr))
                {
                    SqlCommand cmd =
                    new SqlCommand(@"

UPDATE TrainingAssignment
SET AssignmentStatus='Cancelled'
WHERE AssignmentID=@AssignmentID

", con);

                    cmd.Parameters.AddWithValue(
                    "@AssignmentID",
                    e.CommandArgument.ToString());

                    con.Open();

                    cmd.ExecuteNonQuery();

                    SqlCommand cmd1 = new SqlCommand(@"DELETE
FROM TrainingProgress
WHERE TrainingID=@TrainingID
AND EmpID=
(
SELECT EmpID
FROM TrainingAssignment
WHERE AssignmentID=@AssignmentID
)", con);
                    cmd1.Parameters.AddWithValue(
                                       "@AssignmentID",
                                       e.CommandArgument.ToString());
                    cmd1.Parameters.AddWithValue(
    "@TrainingID",
    Session["TrainingID"]);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                }

                BindAssignedEmployee();

                //  LoadBatchSummary();

                lblEmpMessage.ForeColor =
                System.Drawing.Color.Green;

                lblEmpMessage.Text =
                "Employee removed successfully.";
            }
        }

        private string GenerateAssignmentID()
        {
            return Guid.NewGuid()
                       .ToString("N")
                       .Substring(0, 18)
                       .ToUpper();
        }
        protected void btnLoadEmployee_Click(
        object sender,
        EventArgs e)
        {
            lblMessage.Text = "";

            //------------------------------------
            // Company Mandatory
            //------------------------------------

            bool companySelected = false;

            foreach (ListItem item in lstCompany.Items)
            {
                if (item.Selected)
                {
                    companySelected = true;
                    break;
                }
            }

            if (!companySelected)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Please select Company.";
                return;
            }

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                StringBuilder query =
                new StringBuilder();

                query.Append(@"

SELECT

EmpID,
EmpName,
EmpDesignation,
EmpCompany,
EmpPostingPlace

FROM EmpBasicMaster

WHERE 1=1

");

                SqlCommand cmd =
                new SqlCommand();

                cmd.Connection = con;

                //------------------------------------
                // Company Filter
                //------------------------------------

                List<string> companyParams =
                new List<string>();

                int c = 0;

                bool allCompany = false;

                foreach (ListItem item in lstCompany.Items)
                {
                    if (item.Selected)
                    {
                        if (item.Value == "ALL")
                        {
                            allCompany = true;
                            break;
                        }

                        string pname =
                        "@Comp" + c;

                        companyParams.Add(pname);

                        cmd.Parameters.AddWithValue(
                        pname,
                        item.Value);

                        c++;
                    }
                }

                if (!allCompany)
                {
                    query.Append(" AND EmpCompany IN (");
                    query.Append(string.Join(",", companyParams));
                    query.Append(")");
                }

                //------------------------------------
                // Designation Filter
                //------------------------------------

                List<string> desParams =
                new List<string>();

                int d = 0;

                foreach (ListItem item in lstDesignation.Items)
                {
                    if (item.Selected)
                    {
                        string pname =
                        "@Des" + d;

                        desParams.Add(pname);

                        cmd.Parameters.AddWithValue(
                        pname,
                        item.Value);

                        d++;
                    }
                }

                if (desParams.Count > 0)
                {
                    query.Append(" AND EmpDesignation IN (");
                    query.Append(string.Join(",", desParams));
                    query.Append(")");
                }

                //------------------------------------
                // Posting Place Filter
                //------------------------------------

                List<string> placeParams =
                new List<string>();

                int p = 0;

                foreach (ListItem item in lstPostingPlace.Items)
                {
                    if (item.Selected)
                    {
                        string pname =
                        "@Place" + p;

                        placeParams.Add(pname);

                        cmd.Parameters.AddWithValue(
                        pname,
                        item.Value);

                        p++;
                    }
                }

                if (placeParams.Count > 0)
                {
                    query.Append(" AND EmpPostingPlace IN (");
                    query.Append(string.Join(",", placeParams));
                    query.Append(")");
                }

                //------------------------------------

                query.Append(" ORDER BY EmpName");

                cmd.CommandText =
                query.ToString();

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvCompanyEmployee.DataSource =
                dt;

                gvCompanyEmployee.DataBind();

                if (dt.Rows.Count == 0)
                {

                    lblMessage.ForeColor =
                    Color.Red;

                    lblMessage.Text =
                    "No Employee Found.";
                }
                else
                {
                    lblMessage.ForeColor =
                    Color.Green;

                    lblMessage.Text =
                    dt.Rows.Count +
                    " Employee(s) Loaded.";
                }
            }
        }

     
        protected void btnAssignSelected_Click(
object sender,
EventArgs e)
        {
            lblMessage.Text = "";

            int assigned = 0;

            int skipped = 0;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran =
                con.BeginTransaction();

                try
                {
                    foreach (GridViewRow row in
                        gvCompanyEmployee.Rows)
                    {
                        CheckBox chk =
                        (CheckBox)row.FindControl(
                        "chkSelect");

                        if (chk == null ||
                            !chk.Checked)
                        {
                            continue;
                        }

                        Label lblEmpID =
                        (Label)row.FindControl(
                        "lblEmpID");

                        string empid =
                        lblEmpID.Text.Trim().ToUpperInvariant();

                        //----------------------------------
                        // Validation
                        //----------------------------------

                        if (!CanAssignEmployee(
                            con,
                            tran,
                            empid))
                        {
                            skipped++;

                            continue;
                        }

                        //----------------------------------
                        // Save
                        //----------------------------------

                        if (IsBatchFull(con, tran))
                        {
                            skipped++;

                            continue;
                        }

                        SaveAssignment(
                            con,
                            tran,
                            empid,
                            "COMPANY");

                        assigned++;
                    }

                    tran.Commit();

                    BindAssignedEmployee();


                    // LoadBatchSummary();

                    lblMessage.ForeColor =
                    Color.Green;

                    lblMessage.Text =
                    assigned.ToString()
                    + " Employee Assigned Successfully.";

                    if (skipped > 0)
                    {
                        lblMessage.Text +=
                        "  "
                        + skipped.ToString()
                        + " Employee Skipped.";
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

        private bool CanAssignEmployee(
SqlConnection con,
SqlTransaction tran,
string empid)
        {
            //--------------------------------------------
            // Employee Exists
            //--------------------------------------------

            SqlCommand cmdEmp =
            new SqlCommand(@"

SELECT COUNT(*)

FROM EmpBasicMaster

WHERE EmpID=@EmpID

", con, tran);

            cmdEmp.Parameters.AddWithValue(
            "@EmpID",
            empid);

            if (Convert.ToInt32(
                cmdEmp.ExecuteScalar()) == 0)
            {
                return false;
            }

            //--------------------------------------------
            // Already Assigned in Current Training
            //--------------------------------------------

            SqlCommand cmdDuplicate =
            new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment

WHERE

TrainingID=@TrainingID

AND

EmpID=@EmpID

", con, tran);



            cmdDuplicate.Parameters.AddWithValue(
            "@TrainingID",
            Session["TrainingID"]);

            cmdDuplicate.Parameters.AddWithValue(
            "@EmpID",
            empid);

            if (Convert.ToInt32(
                cmdDuplicate.ExecuteScalar()) > 0)
            {
                return false;
            }

            //--------------------------------------------
            // Same Topic Already Attended
            //--------------------------------------------

            SqlCommand cmdTopic =
            new SqlCommand(@"
SELECT COUNT(*)

FROM TrainingAssignment TA

INNER JOIN TrainingDetails TD
ON TA.TrainingID = TD.TrainingID

WHERE
TA.EmpID = @EmpID

AND TD.CourseID =
(
    SELECT CourseID
    FROM TrainingDetails
    WHERE TrainingID = @TrainingID
)

AND ISNULL(TA.AssignmentStatus,'Assigned') <> 'Cancelled'

AND TA.TrainingID <> @TrainingID", con, tran);

            cmdTopic.Parameters.AddWithValue(
            "@EmpID",
            empid);

            cmdTopic.Parameters.AddWithValue(
            "@TrainingID",
            Session["TrainingID"]);

            if (Convert.ToInt32(
                cmdTopic.ExecuteScalar()) > 0)
            {
                return false;
            }

            return true;
        }

        private void SaveAssignment(
        SqlConnection con,
        SqlTransaction tran,
        string empid,
        string mode)
        {
            SqlCommand cmd =
            new SqlCommand(@"

INSERT INTO
TrainingAssignment
(
AssignmentID,
TrainingID,
EmpID,
TrainingAttended,
CreatedOn,
CreatedBy,
AssignmentMode,
AssignmentStatus,
Remarks
)

VALUES
(
@AssignmentID,
@TrainingID,
@EmpID,
'Pending',
GETDATE(),
@CreatedBy,
@Mode,
'Assigned',
''
)

", con, tran);

            cmd.Parameters.AddWithValue(
            "@AssignmentID",
            GenerateAssignmentID());

            cmd.Parameters.AddWithValue(
            "@TrainingID",
            Session["TrainingID"]);

            cmd.Parameters.AddWithValue(
            "@EmpID",
            empid.ToUpperInvariant());

            cmd.Parameters.AddWithValue(
            "@CreatedBy",
            Session["InternalRedirect_Admin"] == null
                ? "Admin"
                : Session["InternalRedirect_Admin"].ToString());

            cmd.Parameters.AddWithValue(
            "@Mode",
            mode);

            cmd.ExecuteNonQuery();
            cmd =
new SqlCommand(@"

INSERT INTO TrainingProgress
(
ProgressID,
TrainingID,
EmpID,
CreatedOn,
CreatedBy
)

VALUES
(
@ProgressID,
@TrainingID,
@EmpID,
GETDATE(),
@CreatedBy
)

", con, tran);

            cmd.Parameters.AddWithValue(
            "@ProgressID",
            GenerateProgressID());

            cmd.Parameters.AddWithValue(
            "@TrainingID",
            Session["TrainingID"]);

            cmd.Parameters.AddWithValue(
            "@EmpID",
            empid.ToUpperInvariant());

            cmd.Parameters.AddWithValue(
            "@CreatedBy",
            Session["InternalRedirect_Admin"] == null
            ?
            "Admin"
            :
            Session["InternalRedirect_Admin"].ToString());

            cmd.ExecuteNonQuery();
        }

        protected void btnDownloadFormat_Click(
object sender,
EventArgs e)
        {
            string file =
            Server.MapPath(
            "~/SampleFormat/TrainingAssignmentSample.xlsx");

            if (!System.IO.File.Exists(file))
            {
                lblBulkMessage.ForeColor =
                Color.Red;

                lblBulkMessage.Text =
                "Sample file not found.";

                return;
            }

            Response.Clear();

            Response.ContentType =
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.AppendHeader(
            "Content-Disposition",
            "attachment; filename=TrainingAssignmentSample.xlsx");

            Response.TransmitFile(file);

            Response.End();
        }

        protected void btnUploadExcel_Click(
        object sender,
        EventArgs e)
        {
            lblBulkMessage.Text = "";

            if (!fuExcel.HasFile)
            {
                lblBulkMessage.ForeColor =
                Color.Red;

                lblBulkMessage.Text =
                "Please select Excel file.";

                return;
            }

            string ext =
            System.IO.Path.GetExtension(
            fuExcel.FileName).ToLower();

            if (ext != ".xlsx")
            {
                lblBulkMessage.ForeColor =
                Color.Red;

                lblBulkMessage.Text =
                "Only .xlsx file allowed.";

                return;
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("EmpID");

            int duplicateInExcel = 0;

            HashSet<string> empList =
            new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

            ExcelPackage.LicenseContext =
            LicenseContext.NonCommercial;

            using (ExcelPackage package =
                new ExcelPackage(
                fuExcel.PostedFile.InputStream))
            {
                ExcelWorksheet ws =
                package.Workbook.Worksheets[0];

                int rowCount =
                ws.Dimension.End.Row;

                for (int i = 2; i <= rowCount; i++)
                {
                    string empid =
                    ws.Cells[i, 1]
                    .Text
                    .Trim().ToUpperInvariant();

                    if (string.IsNullOrWhiteSpace(empid))
                        continue;

                    // Duplicate in Excel
                    if (!empList.Add(empid))
                    {
                        duplicateInExcel++;
                        continue;
                    }

                    dt.Rows.Add(empid);
                }
            }
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
                        dr["EmpID"]
                        .ToString().ToUpperInvariant();

                        if (!CanAssignEmployee(
                            con,
                            tran,
                            empid))
                        {
                            skipped++;

                            continue;
                        }

                        if (IsBatchFull(con, tran))
                        {
                            skipped++;

                            continue;
                        }

                        SaveAssignment(
                            con,
                            tran,
                            empid,
                            "BULK");

                        success++;
                    }

                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblBulkMessage.ForeColor =
                    Color.Red;

                    lblBulkMessage.Text =
                    ex.Message;

                    return;
                }
            }

            BindAssignedEmployee();



            //   LoadBatchSummary();

            lblBulkMessage.ForeColor =
Color.Green;

            StringBuilder msg =
            new StringBuilder();

            msg.Append(success +
            " Employee Assigned Successfully.");

            if (skipped > 0)
            {
                msg.Append("<br/>");
                msg.Append(skipped +
                " Employee Skipped.");
            }

            if (duplicateInExcel > 0)
            {
                msg.Append("<br/>");
                msg.Append(duplicateInExcel +
                " Duplicate Employee ID(s) found in Excel.");
            }

            lblBulkMessage.Text =
            msg.ToString();
        }

        private bool IsBatchFull(
 SqlConnection con,
 SqlTransaction tran)
        {
            SqlCommand cmd;

            if (tran == null)
            {
                cmd = new SqlCommand(@"

SELECT
ISNULL(TD.BatchStrength,0)
-
(
SELECT COUNT(*)
FROM TrainingAssignment
WHERE TrainingID=TD.TrainingID
AND AssignmentStatus='Assigned'
)
FROM TrainingDetails TD
WHERE TrainingID=@TrainingID

", con);
            }
            else
            {
                cmd = new SqlCommand(@"

SELECT
ISNULL(TD.BatchStrength,0)
-
(
SELECT COUNT(*)
FROM TrainingAssignment
WHERE TrainingID=TD.TrainingID
AND AssignmentStatus='Assigned'
)
FROM TrainingDetails TD
WHERE TrainingID=@TrainingID

", con, tran);
            }

            cmd.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

            int remaining =
                Convert.ToInt32(cmd.ExecuteScalar());

            return remaining <= 0;
        }

        protected void btnPrevious_Click(
object sender,
EventArgs e)
        {
            Response.Redirect(
            "AssignSession.aspx");
        }

        protected void btnUpdateBatch_Click(
object sender,
EventArgs e)
        {
            Response.Redirect("CreateBatch.aspx?mode=edit");

        }

        protected void btnFinish_Click(
        object sender,
        EventArgs e)
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                //--------------------------------------
                // Check Any Trainee Assigned
                //--------------------------------------

                SqlCommand cmdCheck =
                new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment

WHERE

TrainingID=@TrainingID

AND

ISNULL(AssignmentStatus,'Assigned')='Assigned'

", con);

                cmdCheck.Parameters.AddWithValue(
                "@TrainingID",
                Session["TrainingID"]);

                int cnt =
                Convert.ToInt32(
                cmdCheck.ExecuteScalar());

                if (cnt == 0)
                {
                    lblMessage.ForeColor =
                    Color.Red;

                    lblMessage.Text =
                    "Please assign at least one trainee.";

                    con.Close();

                    return;
                }

                //--------------------------------------
                // Update Training Status
                //--------------------------------------
                //clsWorkflow.UpdateWorkflow(Session["TrainingID"].ToString(),"TraineeAssigned",4);
                clsWorkflow.UpdateWorkflow(Session["TrainingID"].ToString(), "TraineeAssigned", "D");
            }

            Response.Redirect(
            "ManageTraining.aspx");
        }


        private void ShowMessage(
        string message,
        Color color)
        {
            lblMessage.ForeColor =
            color;

            lblMessage.Text =
            message;
        }

        private void SetButtonStatus()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"
SELECT TrainingStatus
FROM TrainingDetails
WHERE TrainingID=@TrainingID", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                string status =
                    Convert.ToString(
                    cmd.ExecuteScalar());

                con.Close();

                // Default
                btnEmpWise.Enabled = true;
                btnCompanyWise.Enabled = true;
                btnBulkWise.Enabled = true;

                btnAddEmployee.Enabled = true;
                // btnLoadFilter.Enabled = true;
                btnLoadEmployee.Enabled = true;
                btnAssignSelected.Enabled = true;

                btnDownloadFormat.Enabled = true;
                btnUploadExcel.Enabled = true;

                btnFinish.Enabled = true;

                // Training Completed
                if (status == "Completed")
                {
                    btnEmpWise.Enabled = false;
                    btnCompanyWise.Enabled = false;
                    btnBulkWise.Enabled = false;

                    btnAddEmployee.Enabled = false;
                    // btnLoadFilter.Enabled = false;
                    btnLoadEmployee.Enabled = false;
                    btnAssignSelected.Enabled = false;

                    btnDownloadFormat.Enabled = false;
                    btnUploadExcel.Enabled = false;

                    btnFinish.Enabled = false;
                }
            }
        }
        private string GenerateProgressID()
        {
            return
                "PRG"
                + Guid.NewGuid().ToString("N").ToUpper();
        }
        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            Guid.NewGuid().ToString(),

             "$('#lstCompany').select2({width:'100%'});" +
             "$('#lstDesignation').select2({width:'100%'});" +
             "$('#lstPostingPlace').select2({width:'100%'});",
            //"$('#ddlTrainingType').select2({width:'100%'});" +
            //"$('#ddlTrainingCategory').select2({width:'100%'});" +
            //"$('#ddlTrainingOrganizer').select2({width:'100%'});" +
            //"$('#ddlTrainingLocation').select2({width:'100%'});",
            //"$('#lstDesignation').select2({placeholder:'Select Designation',width:'100%'});",

            true);
        }
    }
}