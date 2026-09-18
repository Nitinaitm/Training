using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class Check_Password : System.Web.UI.Page
    {
        clsDataAccess cls = new clsDataAccess();
        string admin = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["InternalRedirect_SuperAdmin"] == null || ((bool)Session["InternalRedirect_SuperAdmin"] == false))
                {
                    Response.Redirect("~/Default.aspx");
                }
                getDesignationName();
            }
            //string sql = "select * from [Login] inner join EmpBasicMaster on CorrespondingEmpID = EmpID";

            //DataTable dt = cls.GetDataTable(sql);
            //Encryptor2 encry = new Encryptor2();
            //foreach (DataRow row in dt.Rows)
            //{
            //    string password = row["Password"].ToString();
            //    string re = row["re"].ToString();
            //    if (!string.IsNullOrEmpty(password))
            //    {

            //        string decrypted_grade = encry.Decrypt(password);
            //        row["Password"] = decrypted_grade;
            //    }
            //    if (!string.IsNullOrEmpty(re))
            //    {
            //        string decrypted_re = encry.Decrypt(re);
            //        row["re"] = decrypted_re;
            //    }
            //}

            //if (dt.Rows.Count > 0)
            //{
            //    P1.Visible = true;
            //}
            //else
            //{
            //    P1.Visible = false;
            //}

            //gridView.DataSource = dt;
            //gridView.DataBind();

           // BindGrid();
            
        }

        public void getDesignationName()
        {
            ddlDesignation.Items.Clear();
            ddlDesignation.Items.Insert(0, new ListItem("Select a Designation", ""));
            string sql = "";
            sql = "select DISTINCT EmpDesignation from EmpBasicMaster";
            DataTable dt = cls.GetDataTable(sql);
            ddlDesignation.DataTextField = "EmpDesignation";
            ddlDesignation.DataValueField = "EmpDesignation";
            ddlDesignation.DataSource = dt;
            ddlDesignation.DataBind();
        }
        private void BindGrid()
        {

            string sql = "SELECT EmpName, EmpDesignation, MobileNo, EmailId,  Password, re, CorrespondingEmpID, role, LoginIDUserID FROM EmpBasicMaster INNER JOIN LOGIN on EmpID = CorrespondingEmpID WHERE 1=1";
            List<SqlParameter> parameters = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(txtEmpID.Text))
            {
                sql += " AND LoginIDUserID LIKE @LoginIDUserID";
                parameters.Add(new SqlParameter("@LoginIDUserID", "%" + txtEmpID.Text.Trim() + "%"));
            }
            // Add filters dynamically
            if (!string.IsNullOrEmpty(txtName.Text))
            {
                sql += " AND EmpName LIKE @EmpName";
                parameters.Add(new SqlParameter("@EmpName", "%" + txtName.Text.Trim() + "%"));
            }
               
            if (!string.IsNullOrEmpty(ddlDesignation.SelectedValue))
            {
                sql += " AND EmpDesignation like @EmpDesignation";
                parameters.Add(new SqlParameter("@EmpDesignation", "%" + ddlDesignation.SelectedValue.Trim() + "%"));
            }

            if (!string.IsNullOrEmpty(ddlCompany.SelectedValue))
            {
                sql += " AND EmpCompany like @EmpCompany";
                parameters.Add(new SqlParameter("@EmpCompany", "%" + ddlCompany.SelectedValue.Trim() + "%"));
            }

            if (!string.IsNullOrEmpty(ddlRole.SelectedValue))
            {
                sql += " AND role like @role";
                parameters.Add(new SqlParameter("@role", "%" + ddlRole.SelectedValue.Trim() + "%"));
            }
            DataTable dt = cls.GetDataTable(sql, parameters.ToArray());
            Encryptor2 encry = new Encryptor2();
            foreach (DataRow row in dt.Rows)
            {
                string password = row["Password"].ToString();
                string re = row["re"].ToString();
                string email = row["EmailId"].ToString();
                if (!string.IsNullOrEmpty(password))
                {

                    string decrypted_grade = encry.Decrypt(password);
                    row["Password"] = decrypted_grade;
                }
                if (!string.IsNullOrEmpty(re))
                {
                    string decrypted_re = encry.Decrypt(re);
                    row["re"] = decrypted_re;
                }
                //if (!string.IsNullOrEmpty(email))
                //{
                //    string decrypted_email = encry.Decrypt(email);
                //    row["EmailId"] = decrypted_email;
                //}
                   
            }
            if (dt.Rows.Count > 0)
            {
                P1.Visible = true;
            }
            else
            {
                P1.Visible = false;
            }

            gridView.DataSource = dt;
            gridView.DataBind();

        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtName.Text = "";
            ddlDesignation.SelectedValue = "";
            ddlCompany.SelectedValue = "";
            ddlRole.SelectedValue = "";
            BindGrid(); // Reload all data
        }

        protected void gridView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=EmployeeAPARFlowDetails.xls");
            Response.Charset = "";
            Response.ContentType = "application/vnd.ms-excel";

            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);

            // Set up a form to contain the GridView
            Page page = new Page();
            HtmlForm form = new HtmlForm();

            gridView.EnableViewState = false; // You may need to disable ViewState

            page.EnableEventValidation = false;
            page.DesignerInitialize();
            page.Controls.Add(form);
            form.Controls.Add(gridView);
            page.RenderControl(hw);

            // Write the HTML back to the browser
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
            // Confirms that an HtmlForm control is rendered for the specified ASP.NET server control at run time.
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}