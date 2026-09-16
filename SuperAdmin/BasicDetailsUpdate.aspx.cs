using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class BasicDetailsUpdate : System.Web.UI.Page
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
                

                bindLogin();
                bindEmpBasic();
            }
        }
        public void bindLogin()
        {
            //clsDataAccess cls = new clsDataAccess();
            string sql = "select * from [Login] where [LoginIDUserID]= @LoginIDUserID";
            SqlParameter[] parameters = new SqlParameter[]
            {
                        new SqlParameter("@LoginIDUserID", txtUsername.Text)
            };
            DataTable dt = cls.GetDataTable(sql, parameters);
            Encryptor2 encry = new Encryptor2();
            foreach (DataRow row in dt.Rows)
            {
                string password = row["Password"].ToString();
                string re = row["re"].ToString();
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
        public void bindEmpBasic()
        {
            //clsDataAccess cls = new clsDataAccess();
            string sql = "select * from [EmpBasicMaster] inner join Login on EmpBasicMaster.EmpID= Login.CorrespondingEmpID where  Login.LoginIDUserID = @LoginIDUserID";
            SqlParameter[] parameters = new SqlParameter[]
           {
                        new SqlParameter("@LoginIDUserID", txtUsername.Text)
           };
            DataTable dt = cls.GetDataTable(sql, parameters);
            Encryptor2 encry = new Encryptor2();
            foreach (DataRow row in dt.Rows)
            {
                string email = row["EmailID"].ToString();
                //if (!string.IsNullOrEmpty(email))
                //{

                //    string decrypted_grade = encry.Decrypt(email);
                //    row["EmailID"] = decrypted_grade;
                //}

            }
            if (dt.Rows.Count > 0)
            {
                P2.Visible = true;

            }
            else
            {
                P2.Visible = false;
            }
            gridView1.DataSource = dt;
            gridView1.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Label1.Text = "";
            bindLogin();
            bindEmpBasic();
            string sql1 = "select * from EmpBasicMaster inner join Login on EmpBasicMaster.EmpID= Login.CorrespondingEmpID where  Login.LoginIDUserID = @LoginIDUserID";
            SqlParameter[] parameters = new SqlParameter[]
           {
                        new SqlParameter("@LoginIDUserID", txtUsername.Text)
           };
            DataTable dt1 = cls.GetDataTable(sql1, parameters);

            if (dt1.Rows.Count > 0)
            {

                update_Mobile.Visible = true;

                txtName.Text = dt1.Rows[0]["EmpName"].ToString();
                dobBasic.Value = dt1.Rows[0]["DOB"].ToString();
                dojBasic.Value = dt1.Rows[0]["DOJ"].ToString();
                

            }
            string sql = "select * from Login where LoginIDUserID = @LoginIDUserID";
            SqlParameter[] parameters1 = new SqlParameter[]
           {
                        new SqlParameter("@LoginIDUserID", txtUsername.Text)
           };
            DataTable dt = cls.GetDataTable(sql, parameters1);

            if (dt.Rows.Count > 0)
            {
                string pass = dt.Rows[0]["Password"].ToString();
                string re_val = dt.Rows[0]["re"].ToString();
                Encryptor2 encry = new Encryptor2();
                password.Value = encry.Decrypt(pass);
                re.Value = encry.Decrypt(re_val);
            }
        }

        protected void gridView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Label1.Text = "";
            //admin = Session["admin"].ToString();
            if (txtName.Text == "")
            {
                Label1.Text = "Please Enter Name";
                return;
            }
            Encryptor2 encry = new Encryptor2();
            string pass = encry.Encrypt(password.Value);
            string re_val = encry.Encrypt(re.Value);

            //string sql = "update [EmpBasicMaster] set [EmpName] = '" + txtName.Text + "', [DOB] = '" + dobBasic.Value + "',  DOJ = '" + dojBasic.Value + "', [CreatedBy] = '" + admin + "'  where [EmpID] = '" + txtUsername.Text + "'";
            //cls.ExecuteSql(sql);
            string sql = "UPDATE [EmpBasicMaster] SET [EmpName] = @EmpName, [DOB] = @DOB, DOJ = @DOJ WHERE [EmpID] = @EmpID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpName", txtName.Text.Trim()),
                new SqlParameter("@DOB", dobBasic.Value.Trim()),
                new SqlParameter("@DOJ", dojBasic.Value.Trim()),
                new SqlParameter("@EmpID", txtUsername.Text.Trim())
            };

            // Execute the query using a method that supports parameters
            cls.ExecuteSql(sql, parameters);

            //string sql1 = "update [Login] set Password = '"+ pass + "', re = '"+ re_val + "'  where [LoginIDUserID] = '" + txtUsername.Text + "'";
            //cls.ExecuteSql(sql1);
            string sql1 = "UPDATE [Login] SET Password = @Password, re = @Re WHERE [LoginIDUserID] = @LoginID";

            SqlParameter[] parameters1 = new SqlParameter[]
            {
                new SqlParameter("@Password", pass),
                new SqlParameter("@Re", re_val),
                new SqlParameter("@LoginID", txtUsername.Text.Trim())
            };

            // Execute the query using a method that supports parameters
            cls.ExecuteSql(sql1, parameters1);

            //  Label1.Text = "Mobile No. Updated to " + txtMobile.Text + ", " + "Email ID Updated to " + txtMail.Text;
            bindLogin();
            bindEmpBasic();

        }
    }
}