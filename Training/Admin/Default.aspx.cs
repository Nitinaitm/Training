using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCompany();
                BindDesignation();
                BindPostingPlace();
                BindPostingDetailPlace();
                BindPostingDepartment();
                BindAreaBoardZone();
                BindCircle();
                BindDivision();
                BindSubdivision();
                BindSection();
            }

            LoadPlugins();
        }

        private clsDataAccess DB()
        {
            return new clsDataAccess();
        }

        private List<string> SelectedValues(ListBox listBox)
        {
            List<string> values = new List<string>();
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value != "ALL")
                {
                    values.Add(item.Value);
                }
            }
            return values;
        }

        private bool IsAllSelected(ListBox listBox)
        {
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value == "ALL")
                {
                    return true;
                }
            }
            return false;
        }

        private void BindCompany()
        {
            DataTable dt = DB().GetDataTable("SELECT CompanyName FROM CompanyMaster ORDER BY CompanyName");
            lstCompany.Items.Clear();
            lstCompany.Items.Add(new ListItem("ALL COMPANIES", "ALL"));
            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row["CompanyName"]);
                lstCompany.Items.Add(new ListItem(value, value));
            }
        }

        private void BindDesignation()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>''" + CompanyWhere("EmpCompany") + " ORDER BY EmpDesignation", CompanyParameters());
            BindList(lstDesignation, dt, "EmpDesignation");
        }

        private void BindPostingPlace()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>''" + CompanyWhere("EmpCompany") + " ORDER BY EmpPostingPlace", CompanyParameters());
            BindList(lstPostingPlace, dt, "EmpPostingPlace");
        }

        private void BindPostingDetailPlace()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.EmpPostingPlace FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.EmpPostingPlace,'')<>''" + CompanyWhere("EBM.EmpCompany") + " ORDER BY EPD.EmpPostingPlace", CompanyParameters());
            BindList(lstDetailPostingPlace, dt, "EmpPostingPlace");
            if (HasHqOnlyCompanySelection())
            {
                lstDetailPostingPlace.Items.Remove(lstDetailPostingPlace.Items.FindByValue("Field"));
            }
        }

        private void BindPostingDepartment()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.EmpPostingDepartment FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.EmpPostingDepartment,'')<>''" + CompanyWhere("EBM.EmpCompany") + DetailPlaceWhere("EPD.EmpPostingPlace") + " ORDER BY EPD.EmpPostingDepartment", MergeParameters(CompanyParameters(), DetailPlaceParameters("EPD.EmpPostingPlace")));
            BindList(lstPostingDepartment, dt, "EmpPostingDepartment");
        }

        private void BindAreaBoardZone()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.AreaBoardZone FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.AreaBoardZone,'')<>''" + CompanyWhere("EBM.EmpCompany") + DetailPlaceWhere("EPD.EmpPostingPlace") + " ORDER BY EPD.AreaBoardZone", MergeParameters(CompanyParameters(), DetailPlaceParameters("EPD.EmpPostingPlace")));
            BindList(lstAreaBoardZone, dt, "AreaBoardZone");
        }

        private void BindCircle()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.Circle FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.Circle,'')<>''" + CompanyWhere("EBM.EmpCompany") + DetailPlaceWhere("EPD.EmpPostingPlace") + ParentWhere("EPD.AreaBoardZone", lstAreaBoardZone) + " ORDER BY EPD.Circle", MergeParameters(CompanyParameters(), DetailPlaceParameters("EPD.EmpPostingPlace"), ParentParameters("EPD.AreaBoardZone", lstAreaBoardZone)));
            BindList(lstCircle, dt, "Circle");
        }

        private void BindDivision()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.Division FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.Division,'')<>''" + CompanyWhere("EBM.EmpCompany") + DetailPlaceWhere("EPD.EmpPostingPlace") + ParentWhere("EPD.AreaBoardZone", lstAreaBoardZone) + ParentWhere("EPD.Circle", lstCircle) + " ORDER BY EPD.Division", MergeParameters(CompanyParameters(), DetailPlaceParameters("EPD.EmpPostingPlace"), ParentParameters("EPD.AreaBoardZone", lstAreaBoardZone), ParentParameters("EPD.Circle", lstCircle)));
            BindList(lstDivision, dt, "Division");
        }

        private void BindSubdivision()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.Subdivision FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.Subdivision,'')<>''" + CompanyWhere("EBM.EmpCompany") + DetailPlaceWhere("EPD.EmpPostingPlace") + ParentWhere("EPD.AreaBoardZone", lstAreaBoardZone) + ParentWhere("EPD.Circle", lstCircle) + ParentWhere("EPD.Division", lstDivision) + " ORDER BY EPD.Subdivision", MergeParameters(CompanyParameters(), DetailPlaceParameters("EPD.EmpPostingPlace"), ParentParameters("EPD.AreaBoardZone", lstAreaBoardZone), ParentParameters("EPD.Circle", lstCircle), ParentParameters("EPD.Division", lstDivision)));
            BindList(lstSubdivision, dt, "Subdivision");
        }

        private void BindSection()
        {
            DataTable dt = DB().GetDataTable("SELECT DISTINCT EPD.Section FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE ISNULL(EPD.Section,'')<>''" + CompanyWhere("EBM.EmpCompany") + DetailPlaceWhere("EPD.EmpPostingPlace") + ParentWhere("EPD.AreaBoardZone", lstAreaBoardZone) + ParentWhere("EPD.Circle", lstCircle) + ParentWhere("EPD.Division", lstDivision) + ParentWhere("EPD.Subdivision", lstSubdivision) + " ORDER BY EPD.Section", MergeParameters(CompanyParameters(), DetailPlaceParameters("EPD.EmpPostingPlace"), ParentParameters("EPD.AreaBoardZone", lstAreaBoardZone), ParentParameters("EPD.Circle", lstCircle), ParentParameters("EPD.Division", lstDivision), ParentParameters("EPD.Subdivision", lstSubdivision)));
            BindList(lstSection, dt, "Section");
        }

        private void BindList(ListBox listBox, DataTable dt, string field)
        {
            listBox.Items.Clear();
            foreach (DataRow row in dt.Rows)
            {
                string value = Convert.ToString(row[field]);
                listBox.Items.Add(new ListItem(value, value));
            }
        }

        private string CompanyWhere(string column)
        {
            List<string> values = SelectedValues(lstCompany);
            if (values.Count == 0 || IsAllSelected(lstCompany))
            {
                return "";
            }

            List<string> p = new List<string>();
            for (int i = 0; i < values.Count; i++)
            {
                p.Add("@COMP" + i);
            }
            return " AND " + column + " IN (" + string.Join(",", p) + ")";
        }

        private SqlParameter[] CompanyParameters()
        {
            List<string> values = SelectedValues(lstCompany);
            List<SqlParameter> p = new List<SqlParameter>();
            if (IsAllSelected(lstCompany))
            {
                return p.ToArray();
            }
            for (int i = 0; i < values.Count; i++)
            {
                p.Add(new SqlParameter("@COMP" + i, values[i]));
            }
            return p.ToArray();
        }

        private string DetailPlaceWhere(string column)
        {
            List<string> values = SelectedValues(lstDetailPostingPlace);
            if (values.Count == 0)
            {
                return "";
            }

            List<string> p = new List<string>();
            for (int i = 0; i < values.Count; i++)
            {
                p.Add("@PLACE" + i);
            }
            return " AND " + column + " IN (" + string.Join(",", p) + ")";
        }

        private SqlParameter[] DetailPlaceParameters(string column)
        {
            List<string> values = SelectedValues(lstDetailPostingPlace);
            List<SqlParameter> p = new List<SqlParameter>();
            for (int i = 0; i < values.Count; i++)
            {
                p.Add(new SqlParameter("@PLACE" + i, values[i]));
            }
            return p.ToArray();
        }

        private string ParentWhere(string column, ListBox listBox)
        {
            List<string> values = SelectedValues(listBox);
            if (values.Count == 0)
            {
                return "";
            }

            List<string> p = new List<string>();
            string prefix = "PAR" + column.Replace("EPD.", "");
            for (int i = 0; i < values.Count; i++)
            {
                p.Add("@" + prefix + i);
            }
            return " AND " + column + " IN (" + string.Join(",", p) + ")";
        }

        private SqlParameter[] ParentParameters(string column, ListBox listBox)
        {
            List<string> values = SelectedValues(listBox);
            List<SqlParameter> p = new List<SqlParameter>();
            string prefix = column.Replace("EPD.", "").Substring(0, 1).ToUpperInvariant();
            for (int i = 0; i < values.Count; i++)
            {
                p.Add(new SqlParameter("@" + prefix + i, values[i]));
            }
            return p.ToArray();
        }

        private SqlParameter[] MergeParameters(params SqlParameter[][] arrays)
        {
            List<SqlParameter> result = new List<SqlParameter>();
            foreach (SqlParameter[] array in arrays)
            {
                result.AddRange(array);
            }
            return result.ToArray();
        }

        private bool HasHqOnlyCompanySelection()
        {
            List<string> values = SelectedValues(lstCompany);
            if (values.Count == 0 || IsAllSelected(lstCompany))
            {
                return false;
            }
            foreach (string value in values)
            {
                if (value.Equals("BSPHCL", StringComparison.OrdinalIgnoreCase) || value.Equals("BSPGCL", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private void ClearDependentLists()
        {
            lstCircle.Items.Clear();
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();
        }

        protected void lstCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDesignation();
            BindPostingPlace();
            BindPostingDetailPlace();
            BindPostingDepartment();
            BindAreaBoardZone();
            ClearDependentLists();
            LoadPlugins();
        }

        protected void lstDetailPostingPlace_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindPostingDepartment();
            BindAreaBoardZone();
            ClearDependentLists();
            LoadPlugins();
        }

        protected void lstAreaBoardZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCircle();
            lstDivision.Items.Clear();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();
            LoadPlugins();
        }

        protected void lstCircle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDivision();
            lstSubdivision.Items.Clear();
            lstSection.Items.Clear();
            LoadPlugins();
        }

        protected void lstDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubdivision();
            lstSection.Items.Clear();
            LoadPlugins();
        }

        protected void lstSubdivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSection();
            LoadPlugins();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindEmployee();
            LoadPlugins();
        }

        private void BindEmployee()
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT EBM.ID,EBM.EmpID,EBM.EmpName,EBM.MobileNo,EBM.EmailId,EBM.EmpCompany,EBM.EmpDesignation,EBM.EmpPostingPlace,EPD.EmpPostingPlace AS DetailPostingPlace,EPD.EmpPostingDepartment,EPD.AreaBoardZone,EPD.Circle,EPD.Division,EPD.Subdivision,EPD.Section FROM EmpBasicMaster EBM LEFT JOIN EmpPostingDetails EPD ON EPD.ID=(SELECT TOP 1 X.ID FROM EmpPostingDetails X WHERE X.EmpID=EBM.EmpID ORDER BY X.ID DESC) WHERE 1=1");

            using (SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = con;

                AddTextFilter(query, cmd, "EBM.EmpID", txtEmpID.Text, "@EmpID");
                AddTextFilter(query, cmd, "EBM.EmpName", txtEmpName.Text, "@EmpName");
                AddTextFilter(query, cmd, "EBM.MobileNo", txtMobile.Text, "@MobileNo");
                AddTextFilter(query, cmd, "EBM.EmailId", txtEmail.Text, "@EmailId");
                AddMultiSelectFilter(query, cmd, lstCompany, "EBM.EmpCompany", "Company");
                AddMultiSelectFilter(query, cmd, lstDesignation, "EBM.EmpDesignation", "Designation");
                AddMultiSelectFilter(query, cmd, lstPostingPlace, "EBM.EmpPostingPlace", "PostingPlace");
                AddMultiSelectFilter(query, cmd, lstDetailPostingPlace, "EPD.EmpPostingPlace", "DetailPlace");
                AddMultiSelectFilter(query, cmd, lstPostingDepartment, "EPD.EmpPostingDepartment", "Department");
                AddMultiSelectFilter(query, cmd, lstAreaBoardZone, "EPD.AreaBoardZone", "AreaBoard");
                AddMultiSelectFilter(query, cmd, lstCircle, "EPD.Circle", "Circle");
                AddMultiSelectFilter(query, cmd, lstDivision, "EPD.Division", "Division");
                AddMultiSelectFilter(query, cmd, lstSubdivision, "EPD.Subdivision", "Subdivision");
                AddMultiSelectFilter(query, cmd, lstSection, "EPD.Section", "Section");

                query.Append(" ORDER BY EBM.EmpID");
                cmd.CommandText = query.ToString();

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvEmployee.DataSource = dt;
                gvEmployee.DataBind();
            }
        }

        private void AddTextFilter(StringBuilder query, SqlCommand cmd, string column, string value, string parameter)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                query.Append(" AND " + column + " LIKE " + parameter);
                cmd.Parameters.AddWithValue(parameter, "%" + value.Trim() + "%");
            }
        }

        private void AddMultiSelectFilter(StringBuilder query, SqlCommand cmd, ListBox listBox, string columnName, string parameterPrefix)
        {
            List<string> parameters = new List<string>();
            if (IsAllSelected(listBox))
            {
                return;
            }

            int count = 0;
            foreach (ListItem item in listBox.Items)
            {
                if (item.Selected && item.Value != "ALL")
                {
                    string parameterName = "@" + parameterPrefix + count;
                    parameters.Add(parameterName);
                    cmd.Parameters.AddWithValue(parameterName, item.Value);
                    count++;
                }
            }

            if (parameters.Count > 0)
            {
                query.Append(" AND " + columnName + " IN (" + string.Join(",", parameters) + ")");
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";
            lstCompany.ClearSelection();
            lstDesignation.ClearSelection();
            lstPostingPlace.ClearSelection();
            lstDetailPostingPlace.ClearSelection();
            lstPostingDepartment.ClearSelection();
            lstAreaBoardZone.ClearSelection();
            lstCircle.ClearSelection();
            lstDivision.ClearSelection();
            lstSubdivision.ClearSelection();
            lstSection.ClearSelection();
            BindCompany();
            BindDesignation();
            BindPostingPlace();
            BindPostingDetailPlace();
            BindPostingDepartment();
            BindAreaBoardZone();
            ClearDependentLists();
            gvEmployee.DataSource = null;
            gvEmployee.DataBind();
            LoadPlugins();
        }

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "$('#" + lstCompany.ClientID + "').select2({width:'100%',placeholder:'Search / Select Company',closeOnSelect:false});" +
                "$('#" + lstDesignation.ClientID + "').select2({width:'100%',placeholder:'Search / Select Designation',closeOnSelect:false});" +
                "$('#" + lstPostingPlace.ClientID + "').select2({width:'100%',placeholder:'Search / Select HRMS Posting Place',closeOnSelect:false});" +
                "$('#" + lstDetailPostingPlace.ClientID + "').select2({width:'100%',placeholder:'Search / Select Posting Details',closeOnSelect:false});" +
                "$('#" + lstPostingDepartment.ClientID + "').select2({width:'100%',placeholder:'Search / Select Department / Office / Cell',closeOnSelect:false});" +
                "$('#" + lstAreaBoardZone.ClientID + "').select2({width:'100%',placeholder:'Search / Select Area Board / Zone',closeOnSelect:false});" +
                "$('#" + lstCircle.ClientID + "').select2({width:'100%',placeholder:'Search / Select Circle',closeOnSelect:false});" +
                "$('#" + lstDivision.ClientID + "').select2({width:'100%',placeholder:'Search / Select Division',closeOnSelect:false});" +
                "$('#" + lstSubdivision.ClientID + "').select2({width:'100%',placeholder:'Search / Select Subdivision',closeOnSelect:false});" +
                "$('#" + lstSection.ClientID + "').select2({width:'100%',placeholder:'Search / Select Section',closeOnSelect:false});", true);
        }
    }
}