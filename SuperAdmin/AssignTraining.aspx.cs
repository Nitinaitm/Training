
using OfficeOpenXml;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class AssignTraining : System.Web.UI.Page
    {
        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                if (
                Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                BindTraining();
                BindEmployee();
                LoadPlugins();
            }
        }

        //private void BindTraining()
        //{
        //    using (SqlConnection con = new SqlConnection(constr))
        //    {
        //        string query = @"SELECT TrainingID, TrainingID + ' | ' + TrainingType + ' | ' + Batch AS TrainingName
        //                         FROM TrainingDetails
        //                         ORDER BY ID DESC";

        //        using (SqlCommand cmd = new SqlCommand(query, con))
        //        {
        //            con.Open();

        //            ddlTraining.DataSource = cmd.ExecuteReader();
        //            ddlTraining.DataTextField = "TrainingName";
        //            ddlTraining.DataValueField = "TrainingID";
        //            ddlTraining.DataBind();

        //            con.Close();
        //        }
        //    }

        //    ddlTraining.Items.Insert(0, new ListItem("Select Training", ""));

        //    ddlBulkTraining.DataSource = ddlTraining.Items;
        //    ddlBulkTraining.DataBind();
        //}
        private void BindTraining()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT TrainingID,
                         TrainingID + ' | ' + TrainingType + ' | ' + Batch AS TrainingName
                         FROM TrainingDetails
                         ORDER BY ID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    SqlDataReader dr = cmd.ExecuteReader();

                    ddlTraining.DataSource = dr;
                    ddlTraining.DataTextField = "TrainingName";
                    ddlTraining.DataValueField = "TrainingID";
                    ddlTraining.DataBind();

                    dr.Close();

                    dr = cmd.ExecuteReader();

                    ddlBulkTraining.DataSource = dr;
                    ddlBulkTraining.DataTextField = "TrainingName";
                    ddlBulkTraining.DataValueField = "TrainingID";
                    ddlBulkTraining.DataBind();

                    con.Close();
                }
            }

            ddlTraining.Items.Insert(0, new ListItem("Select Training", ""));

            ddlBulkTraining.Items.Insert(0, new ListItem("Select Training", ""));
        }
        private void BindEmployee()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT EmpID, EmpID + ' | ' + EmpName + ' | ' + EmpDesignation AS EmpDetails
                                 FROM EmpBasicMaster
                                 ORDER BY EmpName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    lstEmployee.DataSource = cmd.ExecuteReader();
                    lstEmployee.DataTextField = "EmpDetails";
                    lstEmployee.DataValueField = "EmpID";
                    lstEmployee.DataBind();

                    con.Close();
                }
            }
        }

        private string GenerateAssignmentID()
        {
            string assignmentID = "";

            using (SqlConnection con = new SqlConnection(constr))
            {
                string query = @"SELECT TOP 1 AssignmentID FROM TrainingAssignment ORDER BY ID DESC";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    object result = cmd.ExecuteScalar();

                    con.Close();

                    int nextNumber = 1;

                    if (result != null)
                    {
                        string lastID = result.ToString();

                        nextNumber = Convert.ToInt32(lastID.Replace("ASN", "")) + 1;
                    }

                    assignmentID = "ASN" + nextNumber.ToString("0000");
                }
            }

            return assignmentID;
        }

        protected void btnAssign_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            try
            {
                bool employeeSelected = false;

                foreach (ListItem item in lstEmployee.Items)
                {
                    if (item.Selected)
                    {
                        employeeSelected = true;
                        break;
                    }
                }

                if (!employeeSelected)
                {
                    lblMessage.Text = "Select at least one employee.";
                    lblMessage.ForeColor = Color.Red;
                    return;
                }

                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();

                    foreach (ListItem item in lstEmployee.Items)
                    {
                        if (item.Selected)
                        {
                            string query = @"INSERT INTO TrainingAssignment
                                             (AssignmentID,TrainingID,EmpID,CreatedBy)
                                             VALUES
                                             (@AssignmentID,@TrainingID,@EmpID,@AssignedBy)";

                            using (SqlCommand cmd = new SqlCommand(query, con))
                            {
                                cmd.Parameters.AddWithValue("@AssignmentID", GenerateAssignmentID());
                                cmd.Parameters.AddWithValue("@TrainingID", ddlTraining.SelectedValue);
                                cmd.Parameters.AddWithValue("@EmpID", item.Value);
                                cmd.Parameters.AddWithValue("@AssignedBy", "Admin");

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    con.Close();
                }

                lblMessage.Text = "Training assigned successfully.";
                lblMessage.ForeColor = Color.Green;

                ddlTraining.SelectedIndex = 0;

                foreach (ListItem item in lstEmployee.Items)
                {
                    item.Selected = false;
                }

                LoadPlugins();
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                lblMessage.ForeColor = Color.Red;
            }
        }

        protected void btnBulkUpload_Click(object sender, EventArgs e)
        {
            lblBulkMessage.Text = "";

            try
            {
                if (!fuExcel.HasFile)
                {
                    lblBulkMessage.Text = "Please select excel file.";
                    lblBulkMessage.ForeColor = Color.Red;
                    return;
                }

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(fuExcel.FileContent))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    int rowCount = worksheet.Dimension.Rows;

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        con.Open();

                        for (int row = 2; row <= rowCount; row++)
                        {
                            string empid = worksheet.Cells[row, 1].Text.Trim();

                            if (empid != "")
                            {
                                string query = @"INSERT INTO TrainingAssignment
                                                 (AssignmentID,TrainingID,EmpID,CreatedBy)
                                                 VALUES
                                                 (@AssignmentID,@TrainingID,@EmpID,@AssignedBy)";

                                using (SqlCommand cmd = new SqlCommand(query, con))
                                {
                                    cmd.Parameters.AddWithValue("@AssignmentID", GenerateAssignmentID());
                                    cmd.Parameters.AddWithValue("@TrainingID", ddlBulkTraining.SelectedValue);
                                    cmd.Parameters.AddWithValue("@EmpID", empid);
                                    cmd.Parameters.AddWithValue("@AssignedBy", "Admin");

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        con.Close();
                    }
                }

                lblBulkMessage.Text = "Bulk assignment completed successfully.";
                lblBulkMessage.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                lblBulkMessage.Text = ex.Message;
                lblBulkMessage.ForeColor = Color.Red;
            }
        }

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "plugins",
            "$('#ddlTraining').select2({width:'100%'});" +
            "$('#ddlBulkTraining').select2({width:'100%'});" +
            "$('#lstEmployee').select2({placeholder:'Select Employee(s)',width:'100%'});", true);
        }
    }
}
