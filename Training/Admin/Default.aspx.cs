using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        string constr =
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCompany();

                BindDesignation();

                BindPostingPlace();
            }

            LoadPlugins();
        }


        // =========================================================
        // COMPANY
        // =========================================================

        private void BindCompany()
        {
            lstCompany.Items.Clear();

            lstCompany.Items.Add(
                new ListItem(
                    "ALL COMPANIES",
                    "ALL"));

            string query =
                "SELECT DISTINCT EmpCompany FROM EmpBasicMaster WHERE ISNULL(EmpCompany,'')<>'' ORDER BY EmpCompany";

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                using (SqlCommand cmd =
                    new SqlCommand(query, con))
                {
                    con.Open();

                    SqlDataReader dr =
                        cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string company =
                            dr["EmpCompany"].ToString();

                        lstCompany.Items.Add(
                            new ListItem(
                                company,
                                company));
                    }
                }
            }
        }


        // =========================================================
        // DESIGNATION
        // =========================================================

        private void BindDesignation()
        {
            lstDesignation.Items.Clear();

            StringBuilder query =
                new StringBuilder();

            query.Append(
                "SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>''");

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                using (SqlCommand cmd =
                    new SqlCommand())
                {
                    cmd.Connection =
                        con;

                    AddCompanyFilter(
                        query,
                        cmd);

                    query.Append(
                        " ORDER BY EmpDesignation");

                    cmd.CommandText =
                        query.ToString();

                    con.Open();

                    SqlDataReader dr =
                        cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string designation =
                            dr["EmpDesignation"].ToString();

                        lstDesignation.Items.Add(
                            new ListItem(
                                designation,
                                designation));
                    }
                }
            }
        }


        // =========================================================
        // POSTING PLACE
        // =========================================================

        private void BindPostingPlace()
        {
            lstPostingPlace.Items.Clear();

            StringBuilder query =
                new StringBuilder();

            query.Append(
                "SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>''");

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                using (SqlCommand cmd =
                    new SqlCommand())
                {
                    cmd.Connection =
                        con;

                    AddCompanyFilter(
                        query,
                        cmd);

                    query.Append(
                        " ORDER BY EmpPostingPlace");

                    cmd.CommandText =
                        query.ToString();

                    con.Open();

                    SqlDataReader dr =
                        cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string postingPlace =
                            dr["EmpPostingPlace"].ToString();

                        lstPostingPlace.Items.Add(
                            new ListItem(
                                postingPlace,
                                postingPlace));
                    }
                }
            }
        }


        // =========================================================
        // COMPANY FILTER
        // Used while loading Designation / Posting Place
        // =========================================================

        private void AddCompanyFilter(
            StringBuilder query,
            SqlCommand cmd)
        {
            List<string> parameters =
                new List<string>();

            bool allSelected =
                false;

            int count =
                0;

            foreach (ListItem item in lstCompany.Items)
            {
                if (item.Selected)
                {
                    if (item.Value == "ALL")
                    {
                        allSelected =
                            true;

                        break;
                    }

                    string parameterName =
                        "@CompanyFilter" + count;

                    parameters.Add(
                        parameterName);

                    cmd.Parameters.AddWithValue(
                        parameterName,
                        item.Value);

                    count++;
                }
            }

            if (allSelected)
            {
                return;
            }

            if (parameters.Count > 0)
            {
                query.Append(
                    " AND EmpCompany IN (");

                query.Append(
                    string.Join(
                        ",",
                        parameters));

                query.Append(
                    ")");
            }
        }


        // =========================================================
        // COMPANY SELECTED INDEX CHANGED
        // =========================================================

        protected void lstCompany_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindDesignation();

            BindPostingPlace();

            gvEmployee.DataSource =
                null;

            gvEmployee.DataBind();

            LoadPlugins();
        }


        // =========================================================
        // SEARCH
        // =========================================================

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindEmployee();

            LoadPlugins();
        }


        // =========================================================
        // LOAD EMPLOYEE
        // =========================================================

        private void BindEmployee()
        {
            StringBuilder query =
                new StringBuilder();

            query.Append(
                "SELECT ID, EmpID, EmpName, MobileNo, EmailId, EmpCompany, EmpDesignation, EmpPostingPlace FROM EmpBasicMaster WHERE 1=1");

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                using (SqlCommand cmd =
                    new SqlCommand())
                {
                    cmd.Connection =
                        con;


                    // =================================================
                    // EMPLOYEE ID
                    // =================================================

                    if (!string.IsNullOrWhiteSpace(
                        txtEmpID.Text))
                    {
                        query.Append(
                            " AND EmpID LIKE @EmpID");

                        cmd.Parameters.AddWithValue(
                            "@EmpID",
                            "%"
                            +
                            txtEmpID.Text.Trim().ToUpperInvariant()
                            +
                            "%");
                    }


                    // =================================================
                    // EMPLOYEE NAME
                    // =================================================

                    if (!string.IsNullOrWhiteSpace(
                        txtEmpName.Text))
                    {
                        query.Append(
                            " AND EmpName LIKE @EmpName");

                        cmd.Parameters.AddWithValue(
                            "@EmpName",
                            "%"
                            +
                            txtEmpName.Text.Trim()
                            +
                            "%");
                    }


                    // =================================================
                    // MOBILE
                    // =================================================

                    if (!string.IsNullOrWhiteSpace(
                        txtMobile.Text))
                    {
                        query.Append(
                            " AND MobileNo LIKE @MobileNo");

                        cmd.Parameters.AddWithValue(
                            "@MobileNo",
                            "%"
                            +
                            txtMobile.Text.Trim()
                            +
                            "%");
                    }


                    // =================================================
                    // EMAIL
                    // =================================================

                    if (!string.IsNullOrWhiteSpace(
                        txtEmail.Text))
                    {
                        query.Append(
                            " AND EmailId LIKE @EmailId");

                        cmd.Parameters.AddWithValue(
                            "@EmailId",
                            "%"
                            +
                            txtEmail.Text.Trim()
                            +
                            "%");
                    }


                    // =================================================
                    // COMPANY
                    // =================================================

                    AddMultiSelectFilter(
                        query,
                        cmd,
                        lstCompany,
                        "EmpCompany",
                        "Company");


                    // =================================================
                    // DESIGNATION
                    // =================================================

                    AddMultiSelectFilter(
                        query,
                        cmd,
                        lstDesignation,
                        "EmpDesignation",
                        "Designation");


                    // =================================================
                    // POSTING PLACE
                    // =================================================

                    AddMultiSelectFilter(
                        query,
                        cmd,
                        lstPostingPlace,
                        "EmpPostingPlace",
                        "PostingPlace");


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
        }


        // =========================================================
        // COMMON MULTI SELECT FILTER
        // =========================================================

        private void AddMultiSelectFilter(
            StringBuilder query,
            SqlCommand cmd,
            ListBox listBox,
            string columnName,
            string parameterPrefix)
        {
            List<string> parameters =
                new List<string>();

            bool allSelected =
                false;

            int count =
                0;


            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected)
                {
                    if (item.Value == "ALL")
                    {
                        allSelected =
                            true;

                        break;
                    }

                    string parameterName =
                        "@"
                        +
                        parameterPrefix
                        +
                        count;

                    parameters.Add(
                        parameterName);

                    cmd.Parameters.AddWithValue(
                        parameterName,
                        item.Value);

                    count++;
                }
            }


            if (allSelected)
            {
                return;
            }


            if (parameters.Count > 0)
            {
                query.Append(
                    " AND "
                    +
                    columnName
                    +
                    " IN (");

                query.Append(
                    string.Join(
                        ",",
                        parameters));

                query.Append(
                    ")");
            }
        }


        // =========================================================
        // RESET
        // =========================================================

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            txtEmpID.Text =
                "";

            txtEmpName.Text =
                "";

            txtMobile.Text =
                "";

            txtEmail.Text =
                "";


            lstCompany.ClearSelection();

            lstDesignation.ClearSelection();

            lstPostingPlace.ClearSelection();


            BindDesignation();

            BindPostingPlace();


            gvEmployee.DataSource =
                null;

            gvEmployee.DataBind();


            LoadPlugins();
        }


        // =========================================================
        // SELECT2 PLUGIN
        // Same pattern as AssignTrainee
        // =========================================================

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                Guid.NewGuid().ToString(),
                "$('#" + lstCompany.ClientID + "').select2({width:'100%',placeholder:'Search / Select Company',closeOnSelect:false});" +
                "$('#" + lstDesignation.ClientID + "').select2({width:'100%',placeholder:'Search / Select Designation',closeOnSelect:false});" +
                "$('#" + lstPostingPlace.ClientID + "').select2({width:'100%',placeholder:'Search / Select Posting Place',closeOnSelect:false});",
                true);
        }
    }
}