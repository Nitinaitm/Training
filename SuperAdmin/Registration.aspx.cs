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
    public partial class Registration : System.Web.UI.Page
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
               
                getCompanyName(ddlCompany);
                getDesignation(ddlDesignation_Selected);
         
                bindLogin();
                bindEmpBasic();
            }

        }
        public void bindLogin()
        {
            //clsDataAccess cls = new clsDataAccess();
            //string sql = "select * from [Login] where [LoginIDUserID]= '" + txtUsername.Text + "'";
            //DataTable dt = cls.GetDataTable(sql);
            string sql = "SELECT * FROM [Login] WHERE [LoginIDUserID] = @LoginIDUserID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@LoginIDUserID", txtUsername.Text)
            };

            // Execute securely
            DataTable dt = cls.GetDataTable(sql, parameters);

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
            string sql = "select * from [EmpBasicMaster] inner join Login on EmpBasicMaster.EmpID= Login.CorrespondingEmpID  WHERE Login.LoginIDUserID = @LoginIDUserID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@LoginIDUserID", txtUsername.Text)
            };

            // Execute securely
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

                string mobile = dt.Rows[0]["MobileNo"].ToString();
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
            update_Mobile.Visible = false;
            final_submit_all_data.Visible = false;
            update_basic_master.Visible = false;
            login_not_found.Visible = false;
            Label1.Text = "";
            Label2.Text = "";
            txtUsernameLogin.Text = "";
            ddlRole.SelectedValue = "";
            txtCorrespondingLogin.Text = "";
            txtMobile.Text = "";
            txtMail.Text = "";
            txtEmp_basic.Text = "";
            txtName_basic.Text = "";
            txtMobile_basic.Text = "";
            txtEmail_basic.Text = "";
            ddlCompany.SelectedValue = "";
            ddlDesignation_Selected.SelectedValue = "";
            Session["correspondingEmpID"] = "";

            bindLogin();
            bindEmpBasic();
            string sql = "SELECT * FROM [Login] WHERE [LoginIDUserID] = @LoginIDUserID";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@LoginIDUserID", txtUsername.Text)
            };

            // Execute securely
            DataTable dt = cls.GetDataTable(sql, parameters);

            if (dt.Rows.Count > 0)
            {
                string username = dt.Rows[0]["LoginIDUserID"].ToString();
                string role = dt.Rows[0]["Role"].ToString();
                string correspondingEmpID = dt.Rows[0]["CorrespondingEmpID"].ToString();
                Session["correspondingEmpID"] = correspondingEmpID;
                showMobile(correspondingEmpID);
                //if (!return_val)
                //{
                //    showCompleteBasic(correspondingEmpID);
                //}
            }
            else
            {
                login_not_found.Visible = true;
                txtUsernameLogin.Text = txtUsername.Text;
                // txtEmp_basic.Enabled = true;
                // Session["correspondingEmpID"] = "";
                showMobile(txtUsername.Text);
            }
        }

        public void showMobile(string corressponding_id)
        {
            //string sql1 = "select * from EmpBasicMaster where EmpId = '" + corressponding_id + "'";
            //DataTable dt1 = cls.GetDataTable(sql1);
            string sql1 = "SELECT * FROM EmpBasicMaster WHERE EmpId = @EmpId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpId", corressponding_id)
            };

            // Execute securely
            DataTable dt1 = cls.GetDataTable(sql1, parameters);

            if (dt1.Rows.Count > 0)
            {
                if (login_not_found.Visible == false)
                {
                    update_Mobile.Visible = true;
                    final_submit_all_data.Visible = false;
                    update_basic_master.Visible = false;
                    // txtName.Text = dt1.Rows[0]["EmpName"].ToString();
                    txtMobile.Text = dt1.Rows[0]["MobileNo"].ToString();
                    string email_id = dt1.Rows[0]["EmailId"].ToString();
                    txtMail.Text = email_id;
                    //Encryptor2 encry = new Encryptor2();
                    //txtMail.Text = encry.Decrypt(email_id);                         //    return true;
                }
                else
                {
                    update_Mobile.Visible = true;
                    final_submit_all_data.Visible = false;
                    update_basic_master.Visible = false;
                    login_not_found.Visible = true;
                    // txtName.Text = dt1.Rows[0]["EmpName"].ToString();
                    txtMobile.Text = dt1.Rows[0]["MobileNo"].ToString();
                    string email_id = dt1.Rows[0]["EmailId"].ToString();
                    txtMail.Text = email_id;

                    //Encryptor2 encry = new Encryptor2();
                    //txtMail.Text = encry.Decrypt(email_id);
                    Session["correspondingEmpID"] = corressponding_id;
                    //    return true;
                }

            }
            else
            {
                //nameVisible.Visible = false;
                //nametextboxVisible.Visible = false;
                update_Mobile.Visible = false;
                final_submit_all_data.Visible = false;
                update_basic_master.Visible = true;
                if (Session["correspondingEmpID"] != null)
                {
                    txtEmp_basic.Text = Session["correspondingEmpID"].ToString();
                }

                // return false;
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtMobile.Text == "")
            {
                Label1.Text = "Please Enter Mobile No. ";
                return;
            }
            if (txtMail.Text == "")
            {
                Label2.Text = "Please Enter Email ID ";
                return;
            }
            bool validEmail = IsValidEmail1(txtMail.Text);
            if (!validEmail)
            {
                Label2.Text = "Please Enter Correct EmailID ";
                return;
            }
          //  admin = Session["admin"].ToString();
            //Encryptor2 encry1 = new Encryptor2();
            string email_id = txtMail.Text;
            if (login_not_found.Visible == false)
            {
                string corressponding_id = Session["correspondingEmpID"].ToString();
                //string sql = "update [EmpBasicMaster] set [MobileNo] = '" + txtMobile.Text + "', [EmailId] = '" + txtMail.Text + "', [CreatedBy] = '" + admin + "'  where [EmpID] = '" + corressponding_id + "'";
                //cls.ExecuteSql(sql);
                string sql = @" UPDATE [EmpBasicMaster]  SET [MobileNo] = @MobileNo,  [EmailId] = @EmailId  WHERE [EmpID] = @EmpID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MobileNo", txtMobile.Text),
                    new SqlParameter("@EmailId", email_id),
                    new SqlParameter("@EmpID", corressponding_id)
                };

                // Execute securely
                cls.ExecuteSql(sql, parameters);

                //  Label1.Text = "Mobile No. Updated to " + txtMobile.Text + ", " + "Email ID Updated to " + txtMail.Text;
                showMobile(corressponding_id);
                bindLogin();
                bindEmpBasic();
            }
            else
            {
                if (ddlRole.SelectedValue == "")
                {
                    Label1.Text = "Please Select Role";
                    return;
                }
                Encryptor2 encry = new Encryptor2();
                string pass = encry.Encrypt("Bsphcl*123");
                string re_val = encry.Encrypt("Y");
                string sql_insert_login = "INSERT INTO [Login] ([LoginIDUserID], [Role], [CorrespondingEmpID], Password, re) VALUES (@usernameLogin, @role, @correspondingLogin, @password, @re)";
                List<SqlParameter> param2 = new List<SqlParameter>();
                param2.Add(new SqlParameter("@usernameLogin", txtUsernameLogin.Text));
                param2.Add(new SqlParameter("@role", ddlRole.SelectedValue));
                param2.Add(new SqlParameter("@correspondingLogin", txtCorrespondingLogin.Text));
                param2.Add(new SqlParameter("@password", pass));
                param2.Add(new SqlParameter("@re", re_val));
                // string corressponding_id = Session["correspondingEmpID"].ToString();
                cls.ExecuteSql(sql_insert_login, param2, Label2);


                string corressponding_id = Session["correspondingEmpID"].ToString();
                //string sql = "update [EmpBasicMaster] set [MobileNo] = '" + txtMobile.Text + "', [EmailId] = '" + txtMail.Text + "', [CreatedBy] = '" + admin + "'  where [EmpID] = '" + corressponding_id + "'";
                //cls.ExecuteSql(sql);
                string sql = @"    UPDATE [EmpBasicMaster]     SET [MobileNo] = @MobileNo,         [EmailId] = @EmailId    WHERE [EmpID] = @EmpID";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@MobileNo", txtMobile.Text),
                    new SqlParameter("@EmailId", email_id),
                    new SqlParameter("@EmpID", corressponding_id)
                };

                // Execute securely
                cls.ExecuteSql(sql, parameters);

                // Label1.Text = "Mobile No. Updated to " + txtMobile.Text + ", " + "Email ID Updated to " + txtMail.Text;

                login_not_found.Visible = false;
                showMobile(corressponding_id);
                bindLogin();
                bindEmpBasic();
            }

        }

        protected void BtnFinalSubmit_Click(object sender, EventArgs e)
        {
            if (ddlRole.SelectedValue == "")
            {
                Label2.Text = "Please Select Role";
                return;
            }
            if (txtCorrespondingLogin.Text == "")
            {
                Label2.Text = "Please Enter Corresponding Emp ID ";
                return;
            }
            Encryptor2 encry = new Encryptor2();
            string pass = encry.Encrypt("Bsphcl*123");
            string re_val = encry.Encrypt("Y");
           // admin = Session["admin"].ToString();
            string sql_insert_login = "INSERT INTO [Login] ([LoginIDUserID], [Role], [CorrespondingEmpID], Password, re) VALUES (@usernameLogin, @role, @correspondingLogin, @password, @re)";
            List<SqlParameter> param2 = new List<SqlParameter>();
            param2.Add(new SqlParameter("@usernameLogin", txtUsernameLogin.Text));
            param2.Add(new SqlParameter("@role", ddlRole.SelectedValue));
            param2.Add(new SqlParameter("@correspondingLogin", txtCorrespondingLogin.Text));
            param2.Add(new SqlParameter("@password", pass));
            param2.Add(new SqlParameter("@re", re_val));
            // string corressponding_id = Session["correspondingEmpID"].ToString();
            cls.ExecuteSql(sql_insert_login, param2, Label2);
            login_not_found.Visible = false;
            showMobile(txtCorrespondingLogin.Text);
            bindLogin();
            bindEmpBasic();
        }

        protected void btnInsert_basic_Click(object sender, EventArgs e)
        {

            if (txtName_basic.Text == "")
            {
                Label2.Text = "Please Enter Name ";
                return;
            }
            if (from_period.Text == "")
            {
                Label2.Text = "Please Enter DOB ";
                return;
            }
            if (to_period.Text == "")
            {
                Label2.Text = "Please Enter DOJ ";
                return;
            }
            if (txtMobile_basic.Text == "")
            {
                Label2.Text = "Please Enter Mobile No. ";
                return;
            }
            if (txtEmail_basic.Text == "")
            {
                Label2.Text = "Please Enter Email ID ";
                return;
            }
            bool validEmail = IsValidEmail1(txtEmail_basic.Text);
            if (!validEmail)
            {
                Label2.Text = "Please Enter Correct EmailID ";
                return;
            }
            if (ddlCompany.SelectedValue == "")
            {
                Label2.Text = "Please Enter Company ";
                return;
            }
            if (ddlDesignation_Selected.SelectedValue == "")
            {
                Label2.Text = "Please Enter Designation";
                return;
            }
           // admin = Session["admin"].ToString();
            if (login_not_found.Visible == true)
            {
                if (ddlRole.SelectedValue == "")
                {
                    Label2.Text = "Please Select Role";
                    return;
                }
                if (txtCorrespondingLogin.Text == "")
                {
                    Label2.Text = "Please Enter Corresponding Emp ID ";
                    return;
                }
                Encryptor2 encry = new Encryptor2();
                string pass = encry.Encrypt("Bsphcl*123");
                string re_val = encry.Encrypt("Y");
                string sql_insert_login = "INSERT INTO [Login] ([LoginIDUserID], [Role], [CorrespondingEmpID], Password, re) VALUES (@usernameLogin, @role, @correspondingLogin, @password, @re)";
                List<SqlParameter> param2 = new List<SqlParameter>();
                param2.Add(new SqlParameter("@usernameLogin", txtUsernameLogin.Text));
                param2.Add(new SqlParameter("@role", ddlRole.SelectedValue));
                param2.Add(new SqlParameter("@correspondingLogin", txtCorrespondingLogin.Text));
                param2.Add(new SqlParameter("@password", pass));
                param2.Add(new SqlParameter("@re", re_val));
                // string corressponding_id = Session["correspondingEmpID"].ToString();
                cls.ExecuteSql(sql_insert_login, param2, Label2);
                string email_id = txtEmail_basic.Text;


                string sql_insert = "INSERT INTO [EmpBasicMaster] ([EmpID], [EmpName], [DOB], [DOJ], [MobileNo], [EmailId], [EmpCompany], [EmpDesignation]) VALUES (@emp_id, @emp_name, @dob, @doj, @mobile_no, @email_id, @company, @designation)";
                List<SqlParameter> param1 = new List<SqlParameter>();
                param1.Add(new SqlParameter("@emp_id", txtEmp_basic.Text));
                param1.Add(new SqlParameter("@emp_name", txtName_basic.Text));
                param1.Add(new SqlParameter("@dob", from_period.Text));
                param1.Add(new SqlParameter("@doj", to_period.Text));
                param1.Add(new SqlParameter("@mobile_no", txtMobile_basic.Text));
                param1.Add(new SqlParameter("@email_id", email_id));
                param1.Add(new SqlParameter("@company", ddlCompany.SelectedItem.Value));
                param1.Add(new SqlParameter("@designation", ddlDesignation_Selected.SelectedValue));
                cls.ExecuteSql(sql_insert, param1, Label2);

                bindLogin();
                bindEmpBasic();
                login_not_found.Visible = false;
                showMobile(txtEmp_basic.Text);
                // Response.Redirect("~/Admin/Registration.aspx");

                Label2.Text = "Details have been Saved";
                // showMobile(corressponding_id);
            }
            else
            {
                if (txtEmp_basic.Text == "")
                {
                    Label2.Text = "Please Enter Emp ID ";
                    return;
                }
                string corressponding_id = Session["correspondingEmpID"].ToString();
              //  admin = Session["admin"].ToString();
                Encryptor2 encry = new Encryptor2();
                string email_id = txtEmail_basic.Text;
                string sql_insert = "INSERT INTO [EmpBasicMaster] ([EmpID], [EmpName], [DOB], [DOJ], [MobileNo], [EmailId], [EmpCompany], [EmpDesignation]) VALUES (@emp_id, @emp_name, @dob, @doj, @mobile_no, @email_id, @company, @designation)";
                List<SqlParameter> param1 = new List<SqlParameter>();
                param1.Add(new SqlParameter("@emp_id", txtEmp_basic.Text));
                param1.Add(new SqlParameter("@emp_name", txtName_basic.Text));
                param1.Add(new SqlParameter("@dob", from_period.Text));
                param1.Add(new SqlParameter("@doj", to_period.Text));
                param1.Add(new SqlParameter("@mobile_no", txtMobile_basic.Text));
                param1.Add(new SqlParameter("@email_id", email_id));
                param1.Add(new SqlParameter("@company", ddlCompany.SelectedItem.Value));
                param1.Add(new SqlParameter("@designation", ddlDesignation_Selected.SelectedValue));
                cls.ExecuteSql(sql_insert, param1, Label2);

                bindLogin();
                bindEmpBasic();
                showMobile(txtEmp_basic.Text);
                // Response.Redirect("~/Admin/Registration.aspx");

                Label2.Text = "Details have been Saved";
                // showMobile(corressponding_id);
            }

        }
        public void getDesignation(DropDownList ddlDesignation)
        {

            ddlDesignation.Items.Clear();

            ddlDesignation.Items.Insert(0, new ListItem("Select a Designation", ""));

            string sql = "";
            sql = "SELECT [DesignationID], [DesignationName] FROM [DesignationMaster] ";


            DataTable dt = cls.GetDataTable(sql);
            ddlDesignation.DataTextField = "DesignationName";
            ddlDesignation.DataValueField = "DesignationName";
            ddlDesignation.DataSource = dt;
            ddlDesignation.DataBind();

        }
        public void getCompanyName(DropDownList ddlCompany)
        {

            ddlCompany.Items.Clear();

            ddlCompany.Items.Insert(0, new ListItem("Select a Company", ""));

            string sql = "";
            sql = "SELECT [CompanyID], [CompanyName] FROM [CompanyMaster] ";
            DataTable dt = cls.GetDataTable(sql);
            ddlCompany.DataTextField = "CompanyName";
            ddlCompany.DataValueField = "CompanyName";
            ddlCompany.DataSource = dt;
            ddlCompany.DataBind();

        }
        private bool IsValidEmail1(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                // An exception is thrown if the email address is not valid
                return false;
            }
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                // An exception is thrown if the email address is not valid
                return false;
            }
        }

        protected void gridView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void gridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ddlRole_SelectedIndexChanged(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label2.Text = "";
            if (ddlRole.SelectedValue == "Officer")
            {
                txtCorrespondingLogin.Text = txtUsernameLogin.Text;
                txtCorrespondingLogin.Enabled = false;
                txtEmp_basic.Text = txtUsernameLogin.Text;
                txtEmp_basic.Enabled = false;
                //string sql1 = "select * from EmpBasicMaster where EmpId = '" + txtEmp_basic.Text + "'";
                //DataTable dt1 = cls.GetDataTable(sql1);
                string sql1 = "SELECT * FROM EmpBasicMaster WHERE EmpId = @EmpId";

                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@EmpId", txtEmp_basic.Text)
                };

                // Execute securely
                DataTable dt1 = cls.GetDataTable(sql1, parameters);

                if (dt1.Rows.Count > 0)
                {
                    update_basic_master.Visible = false;
                    final_submit_all_data.Visible = true;
                }
                else
                {

                    update_basic_master.Visible = true;
                    final_submit_all_data.Visible = false;
                }
            }
            else
            {
                txtCorrespondingLogin.Enabled = true;
                txtCorrespondingLogin.Text = "";
                txtEmp_basic.Text = "";
                txtEmp_basic.Enabled = false;
                update_Mobile.Visible = false;
                update_basic_master.Visible = true;
                //string sql1 = "select * from EmpBasicMaster where EmpId = '" + txtEmp_basic.Text + "'";
                //DataTable dt1 = cls.GetDataTable(sql1);

                //if (dt1.Rows.Count > 0)
                //{
                //    update_basic_master.Visible = false;
                //}
                //else
                //{

                //    update_basic_master.Visible = true;
                //}
            }
        }

        protected void txtCorrespondingLogin_TextChanged(object sender, EventArgs e)
        {
            Label1.Text = "";
            Label2.Text = "";
            txtEmp_basic.Text = txtCorrespondingLogin.Text;
            txtEmp_basic.Enabled = false;
            //string sql1 = "select * from EmpBasicMaster where EmpId = '" + txtEmp_basic.Text + "'";
            //DataTable dt1 = cls.GetDataTable(sql1);
            string sql1 = "SELECT * FROM EmpBasicMaster WHERE EmpId = @EmpId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@EmpId", txtEmp_basic.Text)
            };

            // Execute securely
            DataTable dt1 = cls.GetDataTable(sql1, parameters);

            if (dt1.Rows.Count > 0)
            {
                update_basic_master.Visible = false;
                final_submit_all_data.Visible = true;
                txtEmp_basic.Text = "";
                txtName_basic.Text = "";
                txtMobile_basic.Text = "";
                txtEmail_basic.Text = "";
                ddlCompany.SelectedValue = "";
                ddlDesignation_Selected.SelectedValue = "";
            }
            else
            {

                update_basic_master.Visible = true;
            }
        }
    }
}