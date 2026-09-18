using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class Default : System.Web.UI.Page
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
                Session["InternalRedirect_SuperAdmin"]
                == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }

                BindDesignation();

                BindCompany();

                BindPostingPlace();
            }
        }



        private void BindDesignation()
        {
            BindCheckBoxList(
            chkDesignation,

            @"SELECT DISTINCT
            EmpDesignation

            FROM EmpBasicMaster

            WHERE
            EmpDesignation IS NOT NULL
            AND EmpDesignation<>''

            ORDER BY EmpDesignation",

            "EmpDesignation");
        }



        private void BindCompany()
        {
            BindCheckBoxList(
            chkCompany,

            @"SELECT DISTINCT
            EmpCompany

            FROM EmpBasicMaster

            WHERE
            EmpCompany IS NOT NULL
            AND EmpCompany<>''

            ORDER BY EmpCompany",

            "EmpCompany");
        }



        private void BindPostingPlace()
        {
            BindCheckBoxList(
            chkPostingPlace,

            @"SELECT DISTINCT
            EmpPostingPlace

            FROM EmpBasicMaster

            WHERE
            EmpPostingPlace IS NOT NULL
            AND EmpPostingPlace<>''

            ORDER BY EmpPostingPlace",

            "EmpPostingPlace");
        }



        private void BindCheckBoxList(
        CheckBoxList chk,
        string query,
        string field)
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(
                query,
                con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                chk.DataSource = dt;

                chk.DataTextField = field;

                chk.DataValueField = field;

                chk.DataBind();
            }
        }



        protected void btnSearch_Click(
        object sender,
        EventArgs e)
        {
            BindEmployee();
        }



        private void BindEmployee()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                StringBuilder query =
                new StringBuilder();


                query.Append(@"

SELECT
ID,
EmpID,
EmpName,
MobileNo,
EmailId,
EmpCompany,
EmpDesignation,
EmpPostingPlace

FROM EmpBasicMaster

WHERE 1=1

");


                SqlCommand cmd =
                new SqlCommand();

                cmd.Connection = con;


                if (!string.IsNullOrWhiteSpace(
                    txtEmpID.Text))
                {
                    query.Append(
                    " AND EmpID LIKE @EmpID");

                    cmd.Parameters
                    .AddWithValue(
                    "@EmpID",
                    "%" +
                    txtEmpID.Text.Trim()
                    + "%");
                }



                if (!string.IsNullOrWhiteSpace(
                    txtEmpName.Text))
                {
                    query.Append(
                    " AND EmpName LIKE @EmpName");

                    cmd.Parameters
                    .AddWithValue(
                    "@EmpName",
                    "%" +
                    txtEmpName.Text.Trim()
                    + "%");
                }



                if (!string.IsNullOrWhiteSpace(
                    txtMobile.Text))
                {
                    query.Append(
                    " AND MobileNo LIKE @Mobile");

                    cmd.Parameters
                    .AddWithValue(
                    "@Mobile",
                    "%" +
                    txtMobile.Text.Trim()
                    + "%");
                }



                if (!string.IsNullOrWhiteSpace(
                    txtEmail.Text))
                {
                    query.Append(
                    " AND EmailId LIKE @Email");

                    cmd.Parameters
                    .AddWithValue(
                    "@Email",
                    "%" +
                    txtEmail.Text.Trim()
                    + "%");
                }



                AddMultiSelectFilter(
                query,
                cmd,
                chkDesignation,
                "EmpDesignation",
                "Designation");

                AddMultiSelectFilter(
                query,
                cmd,
                chkCompany,
                "EmpCompany",
                "Company");

                AddMultiSelectFilter(
                query,
                cmd,
                chkPostingPlace,
                "EmpPostingPlace",
                "Posting");


                query.Append(
                " ORDER BY EmpID");


                cmd.CommandText =
                query.ToString();


                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvEmployee.DataSource =
                dt;

                gvEmployee.DataBind();
            }
        }



        private void AddMultiSelectFilter(
        StringBuilder query,
        SqlCommand cmd,
        CheckBoxList chk,
        string columnName,
        string paramPrefix)
        {
            int count = 0;

            StringBuilder selected =
            new StringBuilder();


            foreach (
            ListItem item
            in chk.Items)
            {
                if (item.Selected)
                {
                    string p =
                    "@"
                    + paramPrefix
                    + count;


                    if (count == 0)
                    {
                        selected.Append(
                        " AND "
                        + columnName
                        + " IN(");
                    }

                    selected.Append(
                    p + ",");


                    cmd.Parameters
                    .AddWithValue(
                    p,
                    item.Value);

                    count++;
                }
            }


            if (count > 0)
            {
                selected.Length--;

                selected.Append(")");

                query.Append(
                selected.ToString());
            }
        }




        protected void gvEmployee_RowEditing(
        object sender,
        GridViewEditEventArgs e)
        {
            gvEmployee.EditIndex =
            e.NewEditIndex;

            BindEmployee();
        }



        protected void gvEmployee_RowCancelingEdit(
        object sender,
        GridViewCancelEditEventArgs e)
        {
            gvEmployee.EditIndex =
            -1;

            BindEmployee();
        }




        protected void gvEmployee_RowDataBound(
        object sender,
        GridViewRowEventArgs e)
        {
            if (
            e.Row.RowType ==
            DataControlRowType.DataRow
            &&
            e.Row.RowIndex ==
            gvEmployee.EditIndex)
            {
                DropDownList ddlComp =
                (DropDownList)
                e.Row.FindControl(
                "ddlCompanyEdit");


                DropDownList ddlDes =
                (DropDownList)
                e.Row.FindControl(
                "ddlDesignationEdit");


                DropDownList ddlPost =
                (DropDownList)
                e.Row.FindControl(
                "ddlPostingEdit");



                BindDropDown(
                ddlComp,
                "select distinct EmpCompany from EmpBasicMaster order by EmpCompany",
                "EmpCompany");


                BindDropDown(
                ddlDes,
                "select distinct EmpDesignation from EmpBasicMaster order by EmpDesignation",
                "EmpDesignation");


                BindDropDown(
                ddlPost,
                "select distinct EmpPostingPlace from EmpBasicMaster order by EmpPostingPlace",
                "EmpPostingPlace");



                ddlComp.SelectedValue =
                DataBinder.Eval(
                e.Row.DataItem,
                "EmpCompany")
                .ToString();


                ddlDes.SelectedValue =
                DataBinder.Eval(
                e.Row.DataItem,
                "EmpDesignation")
                .ToString();


                ddlPost.SelectedValue =
                DataBinder.Eval(
                e.Row.DataItem,
                "EmpPostingPlace")
                .ToString();
            }
        }




        void BindDropDown(
        DropDownList ddl,
        string query,
        string field)
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(
                query,
                con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddl.DataSource =
                dt;

                ddl.DataTextField =
                field;

                ddl.DataValueField =
                field;

                ddl.DataBind();
            }
        }




        protected void gvEmployee_RowUpdating(
        object sender,
        GridViewUpdateEventArgs e)
        {
            int id =
            Convert.ToInt32(
            gvEmployee
            .DataKeys[e.RowIndex]
            .Value);


            GridViewRow row =
            gvEmployee
            .Rows[e.RowIndex];


            DropDownList ddlComp =
            (DropDownList)
            row.FindControl(
            "ddlCompanyEdit");


            DropDownList ddlDes =
            (DropDownList)
            row.FindControl(
            "ddlDesignationEdit");


            DropDownList ddlPost =
            (DropDownList)
            row.FindControl(
            "ddlPostingEdit");



            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

UPDATE
EmpBasicMaster

SET
EmpCompany=@Company,
EmpDesignation=@Designation,
EmpPostingPlace=@Posting

WHERE
ID=@ID

", con);


                cmd.Parameters
                .AddWithValue(
                "@Company",
                ddlComp.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@Designation",
                ddlDes.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@Posting",
                ddlPost.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@ID",
                id);


                con.Open();

                cmd.ExecuteNonQuery();
            }


            gvEmployee.EditIndex =
            -1;

            BindEmployee();
        }



        protected void btnReset_Click(
        object sender,
        EventArgs e)
        {
            txtEmpID.Text = "";

            txtEmpName.Text = "";

            txtMobile.Text = "";

            txtEmail.Text = "";


            foreach (
            ListItem item
            in chkDesignation.Items)
            {
                item.Selected = false;
            }

            foreach (
            ListItem item
            in chkCompany.Items)
            {
                item.Selected = false;
            }

            foreach (
            ListItem item
            in chkPostingPlace.Items)
            {
                item.Selected = false;
            }


            gvEmployee.DataSource =
            null;

            gvEmployee.DataBind();
        }
    }
}