using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;

namespace Training
{
    public partial class Registration : System.Web.UI.Page
    {
        private readonly string constr =
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        // Hardcoded Training ID
        private const string TRAINING_ID =
            "TR-WO-BSPHCL-5TH-01-290626-290626-AGAGAECCECCEGCEGEGEGEGEGEECJGJGJSEC-001";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlInternal.Visible = true;
                pnlExternal.Visible = false;

                pnlEmployeeDetails.Visible = false;

                LoadNextEmpID();

                lblMessage.Text = "";
            }
        }

        protected void rblType_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            ClearInternal();

            ClearExternal();

            // Employee details panel hide
            pnlEmployeeDetails.Visible = false;

            if (rblType.SelectedValue == "Internal")
            {
                pnlInternal.Visible = true;
                pnlExternal.Visible = false;
            }
            else
            {
                pnlInternal.Visible = false;
                pnlExternal.Visible = true;

                LoadNextEmpID();
            }
        }
        private void ClearInternal()
        {
            txtSearchEmpID.Text = "";

            txtIEmpID.Text = "";

            txtIName.Text = "";

            txtIDesignation.Text = "";

            txtICompany.Text = "";

            txtIMobile.Text = "";

            txtIEmail.Text = "";
        }

        private void ClearExternal()
        {
            txtEmpName.Text = "";

            txtDesignation.Text = "";

            txtOrganization.Text = "";

            txtMobileNo.Text = "";

            txtEmailId.Text = "";

            LoadNextEmpID();
        }
        protected void btnView_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblMessage.ForeColor = Color.Red;



            if (string.IsNullOrWhiteSpace(txtSearchEmpID.Text))
            {
                lblMessage.Text = "Please enter Employee ID.";
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT
                            EmpID,
                            EmpName,
                            EmpDesignation,
                            EmpCompany,
                            MobileNo,
                            EmailId
                         FROM EmpBasicMaster
                         WHERE EmpID=@EmpID";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@EmpID",
                        txtSearchEmpID.Text.Trim());

                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        pnlEmployeeDetails.Visible = true;

                        txtIEmpID.Text = dr["EmpID"].ToString();
                        txtIName.Text = dr["EmpName"].ToString();
                        txtIDesignation.Text = dr["EmpDesignation"].ToString();
                        txtICompany.Text = dr["EmpCompany"].ToString();
                        txtIMobile.Text = dr["MobileNo"].ToString();
                        txtIEmail.Text = dr["EmailId"].ToString();

                        btnAttendance.Enabled = true;
                    }
                    else
                    {
                        pnlEmployeeDetails.Visible = false;

                        btnAttendance.Enabled = false;

                        lblMessage.ForeColor = Color.Red;
                        lblMessage.Text = "Employee not found.";
                    }

                    dr.Close();
                }
            }
        }
        protected void btnAttendance_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIEmpID.Text))
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = "Please search employee first.";
                return;
            }

            try
            {
                MarkAttendance(txtIEmpID.Text.Trim());

                lblMessage.ForeColor = Color.Green;
                lblMessage.Text = "Registration successful. Your Registration ID is " + txtIEmpID.Text;

                ClearInternal();

                txtSearchEmpID.Text = "";

                pnlEmployeeDetails.Visible = false;

                btnAttendance.Enabled = false;
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = ex.Message;
            }
        }

        private void MarkAttendance(string empId)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction trans = con.BeginTransaction();

                try
                {
                    // Already registered check
                    SqlCommand cmdAlready = new SqlCommand(@"
                SELECT COUNT(*)
                FROM TrainingAssignment
                WHERE TrainingID=@TrainingID
                AND EmpID=@EmpID
                AND TrainingAttended='Yes'", con, trans);

                    cmdAlready.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
                    cmdAlready.Parameters.AddWithValue("@EmpID", empId);

                    int already = Convert.ToInt32(cmdAlready.ExecuteScalar());

                    if (already > 0)
                    {
                        trans.Rollback();
                        throw new Exception("You are already registered.");
                    }

                    // Existing Assignment Check
                    SqlCommand cmdCheck = new SqlCommand(@"
                SELECT COUNT(*)
                FROM TrainingAssignment
                WHERE TrainingID=@TrainingID
                AND EmpID=@EmpID", con, trans);

                    cmdCheck.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
                    cmdCheck.Parameters.AddWithValue("@EmpID", empId);

                    int count = Convert.ToInt32(cmdCheck.ExecuteScalar());

                    if (count > 0)
                    {
                        SqlCommand cmdUpdate = new SqlCommand(@"
                    UPDATE TrainingAssignment
                    SET TrainingAttended='Yes'
                    WHERE TrainingID=@TrainingID
                    AND EmpID=@EmpID", con, trans);

                        cmdUpdate.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
                        cmdUpdate.Parameters.AddWithValue("@EmpID", empId);

                        cmdUpdate.ExecuteNonQuery();
                    }
                    else
                    {
                        string assignmentId = GenerateAssignmentID(con, trans);

                        SqlCommand cmdInsert = new SqlCommand(@"
                    INSERT INTO TrainingAssignment
                    (
                        AssignmentID,
                        TrainingID,
                        EmpID,
                        TrainingAttended,
                        AssignmentStatus,
                        CreatedOn,
                        CreatedBy
                    )
                    VALUES
                    (
                        @AssignmentID,
                        @TrainingID,
                        @EmpID,
                        'Yes',
                        'Assigned',
                        GETDATE(),
                        'QR'
                    )", con, trans);

                        cmdInsert.Parameters.AddWithValue("@AssignmentID", assignmentId);
                        cmdInsert.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
                        cmdInsert.Parameters.AddWithValue("@EmpID", empId);

                        cmdInsert.ExecuteNonQuery();
                    }

                    trans.Commit();
                }
                catch
                {
                    if (trans.Connection != null)
                        trans.Rollback();

                    throw;
                }
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            try
            {
                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    SqlTransaction trans = con.BeginTransaction();

                    try
                    {
                        // Generate External Employee ID
                        string empId = GenerateEmpID(con, trans);

                        // Insert Employee
                        string insertEmp = @"
                INSERT INTO EmpBasicMaster
                (
                    EmpID,
                    EmpName,
                    EmpDesignation,
                    EmpCompany,
                    MobileNo,
                    EmailId,
                    EmpType,
                    CreatedOn,
                    CreatedBy
                )
                VALUES
                (
                    @EmpID,
                    @EmpName,
                    @EmpDesignation,
                    @EmpCompany,
                    @MobileNo,
                    @EmailId,
                    'External',
                    GETDATE(),
                    'QR'
                )";

                        SqlCommand cmd = new SqlCommand(insertEmp, con, trans);

                        cmd.Parameters.AddWithValue("@EmpID", empId);
                        cmd.Parameters.AddWithValue("@EmpName", txtEmpName.Text.Trim());
                        cmd.Parameters.AddWithValue("@EmpDesignation", txtDesignation.Text.Trim());
                        cmd.Parameters.AddWithValue("@EmpCompany", txtOrganization.Text.Trim());
                        cmd.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text.Trim());
                        cmd.Parameters.AddWithValue("@EmailId", txtEmailId.Text.Trim());

                        cmd.ExecuteNonQuery();

                        // Attendance + Assignment
                        MarkAttendance(empId, con, trans);

                        trans.Commit();

                        lblMessage.ForeColor = Color.Green;
                        lblMessage.Text = " Registration successfully. " + "Your Registration ID is " + empId;

                        ClearExternal();

                        pnlExternal.Visible = false;

                        rblType.SelectedValue = "Internal";

                        pnlInternal.Visible = true;

                        pnlEmployeeDetails.Visible = false;

                        LoadNextEmpID();

                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor = Color.Red;
                lblMessage.Text = ex.Message;
            }
        }

        /// <summary>
        /// Attendance with existing transaction
        /// </summary>
        private void MarkAttendance(string empId, SqlConnection con, SqlTransaction trans)
        {
            string alreadyQuery = @"
SELECT COUNT(*)
FROM TrainingAssignment
WHERE TrainingID=@TrainingID
AND EmpID=@EmpID
AND TrainingAttended='Yes'";

            SqlCommand cmdAlready = new SqlCommand(alreadyQuery, con, trans);

            cmdAlready.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
            cmdAlready.Parameters.AddWithValue("@EmpID", empId);

            int already = Convert.ToInt32(cmdAlready.ExecuteScalar());

            if (already > 0)
            {
                throw new Exception("You are already registered.");
            }
            string check = @"
        SELECT COUNT(*)
        FROM TrainingAssignment
        WHERE TrainingID=@TrainingID
        AND EmpID=@EmpID";

            SqlCommand cmdCheck = new SqlCommand(check, con, trans);

            cmdCheck.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
            cmdCheck.Parameters.AddWithValue("@EmpID", empId);

            int cnt = Convert.ToInt32(cmdCheck.ExecuteScalar());

            if (cnt > 0)
            {
                string update = @"
        UPDATE TrainingAssignment
        SET TrainingAttended='Yes'
        WHERE TrainingID=@TrainingID
        AND EmpID=@EmpID";

                SqlCommand cmdUpdate = new SqlCommand(update, con, trans);

                cmdUpdate.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
                cmdUpdate.Parameters.AddWithValue("@EmpID", empId);

                cmdUpdate.ExecuteNonQuery();
            }
            else
            {
                string assignmentId = GenerateAssignmentID(con, trans);

                string insert = @"
        INSERT INTO TrainingAssignment
        (
            AssignmentID,
            TrainingID,
            EmpID,
            TrainingAttended,
AssignmentStatus,
            CreatedOn,
            CreatedBy
        )
        VALUES
        (
            @AssignmentID,
            @TrainingID,
            @EmpID,
            'Yes',
'Assigned',
            GETDATE(),
            'QR'
        )";

                SqlCommand cmdInsert = new SqlCommand(insert, con, trans);

                cmdInsert.Parameters.AddWithValue("@AssignmentID", assignmentId);
                cmdInsert.Parameters.AddWithValue("@TrainingID", TRAINING_ID);
                cmdInsert.Parameters.AddWithValue("@EmpID", empId);

                cmdInsert.ExecuteNonQuery();
            }
        }
        /// <summary>
/// Wrapper method for Internal Employee
/// </summary>

/// <summary>
/// Next External Employee ID
/// </summary>
 private void LoadNextEmpID()
{
    using (SqlConnection con = new SqlConnection(constr))
    {
        string query = @"SELECT ISNULL(MAX(ID),0)+1 FROM EmpBasicMaster";

        SqlCommand cmd = new SqlCommand(query, con);

        con.Open();

        int nextId = Convert.ToInt32(cmd.ExecuteScalar());

        txtEmpID.Text = "Ext" + nextId.ToString("000");
    }
}
        /// <summary>
        /// Generate External Employee ID
        /// </summary>
        private string GenerateEmpID(SqlConnection con, SqlTransaction trans)
        {
            string query = @"SELECT ISNULL(MAX(ID),0)+1 FROM EmpBasicMaster";

            SqlCommand cmd = new SqlCommand(query, con, trans);

            int nextId = Convert.ToInt32(cmd.ExecuteScalar());

            return "Ext" + nextId.ToString("000");
        }

        /// <summary>
        /// Generate Assignment ID
        /// </summary>
        private string GenerateAssignmentID(SqlConnection con, SqlTransaction trans)
        {
            string query = @"SELECT ISNULL(MAX(ID),0)+1 FROM TrainingAssignment";

            SqlCommand cmd = new SqlCommand(query, con, trans);

            int nextId = Convert.ToInt32(cmd.ExecuteScalar());

            return "ASN" + nextId.ToString("0000");
        }

    }
}