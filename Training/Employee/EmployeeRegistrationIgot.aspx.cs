using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Employee
{
    public partial class EmployeeRegistrationIgot : System.Web.UI.Page
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
                if (
                Session["InternalRedirect"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                BindDesignation();

                LoadEmployeeData();
            }
        }



        void LoadEmployeeData()
        {
            if (Session["UserId"] == null)
                return;

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT TOP 1
EmpID,
EmpName,
DOB,
MobileNo,
EmailId,
EmpDesignation

FROM EmpBasicMaster

WHERE EmpID=@EmpID

", con);


                da.SelectCommand.Parameters
                .AddWithValue(
                "@EmpID",
                Session["UserId"]
                .ToString());


                DataTable dt =
                new DataTable();

                da.Fill(dt);


                if (dt.Rows.Count > 0)
                {
                    txtEmpID.Text =
                    dt.Rows[0]["EmpID"]
                    .ToString();


                    txtName.Text =
                    dt.Rows[0]["EmpName"]
                    .ToString();


                    txtEmail.Text =
                    dt.Rows[0]["EmailId"]
                    .ToString();


                    txtMobile.Text =
                    dt.Rows[0]["MobileNo"]
                    .ToString();


                    if (
                    dt.Rows[0]["DOB"]
                    != DBNull.Value)
                    {
                        txtDOB.Text =
                        Convert
                        .ToDateTime(
                        dt.Rows[0]["DOB"])
                        .ToString(
                        "dd-MM-yyyy");
                    }


                    // Designation auto-select
                    string desig =
                    dt.Rows[0]
                    ["EmpDesignation"]
                    .ToString();


                    if (
                    ddlDesignation
                    .Items
                    .FindByValue(
                    desig) != null)
                    {
                        ddlDesignation
                        .SelectedValue =
                        desig;
                    }
                }
            }
        }




        void BindDesignation()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
EmpDesignation

FROM EmpBasicMaster

WHERE
EmpDesignation IS NOT NULL
AND LTRIM(RTRIM(EmpDesignation))<>''

ORDER BY EmpDesignation

", con);


                DataTable dt =
                new DataTable();

                da.Fill(dt);


                ddlDesignation.Items.Clear();


                ddlDesignation.DataSource =
                dt;


                ddlDesignation.DataTextField =
                "EmpDesignation";


                ddlDesignation.DataValueField =
                "EmpDesignation";


                ddlDesignation.DataBind();


                ddlDesignation.Items.Insert(
                0,
                new ListItem(
                "--Select Designation--",
                ""));
            }
        }




        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                    return;


                if (
                string.IsNullOrWhiteSpace(
                ddlDesignation.SelectedValue))
                {
                    lblMsg.Text =
                    "Please select designation";

                    lblMsg.ForeColor =
                    System.Drawing.Color.Red;

                    return;
                }


                using (SqlConnection con =
                new SqlConnection(constr))
                {
                    con.Open();



                    SqlCommand chk =
                    new SqlCommand(@"

SELECT COUNT(*)

FROM EmployeeRegistration

WHERE EmployeeID=@EmpID

", con);


                    chk.Parameters
                    .AddWithValue(
                    "@EmpID",
                    txtEmpID.Text);


                    int cnt =
                    Convert
                    .ToInt32(
                    chk.ExecuteScalar());


                    if (cnt > 0)
                    {
                        lblMsg.Text =
                        "You already submitted details";

                        lblMsg.ForeColor =
                        System.Drawing.Color.Red;

                        return;
                    }




                    SqlCommand cmd =
                    new SqlCommand(@"

INSERT INTO
EmployeeRegistration
(
FullName,
Email,
MobileNumber,
EmpGroup,
Designation,
Gender,
Category,
DOB,
MotherTongue,
EmployeeID,
OfficePinCode,
ExternalSystemID,
ExternalSystemName,
Tags,
CreatedOn,
CreatedBy
)

VALUES
(
@FullName,
@Email,
@Mobile,
@Group,
@Designation,
@Gender,
@Category,
@DOB,
@Mother,
@EmpID,
@Pin,
@ExtID,
@ExtName,
@Tags,
GETDATE(),
@CreatedBy
)

", con);



                    cmd.Parameters
                    .AddWithValue(
                    "@FullName",
                    txtName.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@Email",
                    txtEmail.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@Mobile",
                    txtMobile.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@Group",
                    ddlGroup.SelectedValue);



                    cmd.Parameters
                    .AddWithValue(
                    "@Designation",
                    ddlDesignation.SelectedValue);



                    cmd.Parameters
                    .AddWithValue(
                    "@Gender",
                    ddlGender.SelectedValue);



                    cmd.Parameters
                    .AddWithValue(
                    "@Category",
                    txtCategory.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@DOB",
                    txtDOB.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@Mother",
                    txtMother.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@EmpID",
                    txtEmpID.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@Pin",
                    txtPin.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@ExtID",
                    txtExternalID.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@ExtName",
                    txtExternalName.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@Tags",
                    txtTags.Text.Trim());



                    cmd.Parameters
                    .AddWithValue(
                    "@CreatedBy",
                    Session["UserId"]
                    .ToString());



                    cmd.ExecuteNonQuery();
                }



                lblMsg.Text =
                "Saved Successfully";

                lblMsg.ForeColor =
                System.Drawing.Color.Green;


                Clear();
            }

            catch (Exception ex)
            {
                lblMsg.Text =
                ex.Message;

                lblMsg.ForeColor =
                System.Drawing.Color.Red;
            }
        }





        void Clear()
        {
            ddlGroup.SelectedIndex = 0;

            ddlGender.SelectedIndex = 0;

            txtCategory.Text = "";

            txtMother.Text = "";

            txtPin.Text = "";

            txtExternalID.Text = "";

            txtExternalName.Text = "";

            txtTags.Text = "";


            BindDesignation();

            LoadEmployeeData();

        }
    }
}