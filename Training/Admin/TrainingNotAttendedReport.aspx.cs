using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingNotAttendedReport
        : System.Web.UI.Page
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
                //if (
                //Session["InternalRedirect_Admin"]
                //== null)
                //{
                //    Response.Redirect(
                //    "~/Default.aspx");
                //}

                BindTrainingType();

                ddlOrganizer.Items.Clear();
                ddlOrganizer.Items.Insert(
                0,
                new ListItem(
                "--All--",
                ""));

                ddlLocation.Items.Clear();
                ddlLocation.Items.Insert(
                0,
                new ListItem(
                "--All--",
                ""));

                BindCompany();

                BindDesignation();

               
                LoadCounts();
            }
        }
        //        protected void btnTotalEmployees_Click(
        //object sender,
        //EventArgs e)
        //        {
        //            BindTotalEmployeesGrid();
        //        }

        //        protected void btnNeverAttended_Click(
        //        object sender,
        //        EventArgs e)
        //        {
        //            BindNeverAttendedGrid();
        //        }

        //        protected void btnAttended_Click(
        //        object sender,
        //        EventArgs e)
        //        {
        //            BindAttendedGrid();
        //        }
        private void BindTrainingType()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
TrainingType

FROM TrainingDetails

ORDER BY TrainingType

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlTrainingType.DataSource =
                dt;

                ddlTrainingType.DataTextField =
                "TrainingType";

                ddlTrainingType.DataValueField =
                "TrainingType";

                ddlTrainingType.DataBind();

                ddlTrainingType.Items.Insert(
                0,
                new ListItem(
                "--All--",
                ""));
            }
        }

        private void BindOrganizer()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT DISTINCT
TrainingOrganizer

FROM TrainingDetails

WHERE TrainingType=@Type

ORDER BY TrainingOrganizer

", con);

                cmd.Parameters.AddWithValue(
                "@Type",
                ddlTrainingType.SelectedValue);

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlOrganizer.DataSource =
                dt;

                ddlOrganizer.DataTextField =
                "TrainingOrganizer";

                ddlOrganizer.DataValueField =
                "TrainingOrganizer";

                ddlOrganizer.DataBind();

                ddlOrganizer.Items.Insert(
                0,
                new ListItem(
                "--All--",
                ""));
            }
        }

        private void BindLocation()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT DISTINCT
TrainingLocation

FROM TrainingDetails

WHERE TrainingType=@Type
AND TrainingOrganizer=@Organizer

ORDER BY TrainingLocation

", con);

                cmd.Parameters.AddWithValue(
                "@Type",
                ddlTrainingType.SelectedValue);

                cmd.Parameters.AddWithValue(
                "@Organizer",
                ddlOrganizer.SelectedValue);

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlLocation.DataSource =
                dt;

                ddlLocation.DataTextField =
                "TrainingLocation";

                ddlLocation.DataValueField =
                "TrainingLocation";

                ddlLocation.DataBind();

                ddlLocation.Items.Insert(
                0,
                new ListItem(
                "--All--",
                ""));
            }
        }

        private void BindCompany()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
EmpCompany

FROM EmpBasicMaster

ORDER BY EmpCompany

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                chkCompany.DataSource =
                dt;

                chkCompany.DataTextField =
                "EmpCompany";

                chkCompany.DataValueField =
                "EmpCompany";

                chkCompany.DataBind();
            }
        }

        private void BindDesignation()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
EmpDesignation

FROM EmpBasicMaster

ORDER BY EmpDesignation

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                chkDesignation.DataSource =
                dt;

                chkDesignation.DataTextField =
                "EmpDesignation";

                chkDesignation.DataValueField =
                "EmpDesignation";

                chkDesignation.DataBind();
            }
        }

        protected void ddlTrainingType_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindOrganizer();

            ddlLocation.Items.Clear();

            ddlLocation.Items.Insert(
            0,
            new ListItem(
            "--All--",
            ""));
        }

        protected void ddlOrganizer_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindLocation();
        }

        protected void btnSearch_Click(
        object sender,
        EventArgs e)
        {
            //BindGrid();
            LoadCounts();

        }

        private string GetCompanyFilter()
        {
            StringBuilder sb =
            new StringBuilder();

            foreach (ListItem item
            in chkCompany.Items)
            {
                if (item.Selected)
                {
                    if (sb.Length > 0)
                        sb.Append(",");

                    sb.Append("'" +
                    item.Value.Replace("'", "''")
                    + "'");
                }
            }

            return sb.ToString();
        }

        private string GetDesignationFilter()
        {
            StringBuilder sb =
            new StringBuilder();

            foreach (ListItem item
            in chkDesignation.Items)
            {
                if (item.Selected)
                {
                    if (sb.Length > 0)
                        sb.Append(",");

                    sb.Append("'" +
                    item.Value.Replace("'", "''")
                    + "'");
                }
            }

            return sb.ToString();
        }

        private void BindGrid()
        {
            StringBuilder q =
            new StringBuilder();

           

            string company =
            GetCompanyFilter();

            if (company != "")
            {
                q.Append(@"

AND EmpCompany IN
(" + company + @")

");
            }

            string desig =
            GetDesignationFilter();

            if (desig != "")
            {
                q.Append(@"

AND EmpDesignation IN
(" + desig + @")

");
            }

            q.Append(@"

ORDER BY
EmpID

");

            FillGrid(
            q.ToString());
        }

        private void ApplyTrainingFilters(
        StringBuilder q)
        {
            if (
            ddlTrainingType.SelectedValue
            != "")
            {
                q.Append(@"

AND TD.TrainingType =
'" +
ddlTrainingType.SelectedValue
.Replace("'", "''")
+ @"'

");
            }

            if (
            ddlOrganizer.SelectedValue
            != "")
            {
                q.Append(@"

AND TD.TrainingOrganizer =
'" +
ddlOrganizer.SelectedValue
.Replace("'", "''")
+ @"'

");
            }

            if (
            ddlLocation.SelectedValue
            != "")
            {
                q.Append(@"

AND TD.TrainingLocation =
'" +
ddlLocation.SelectedValue
.Replace("'", "''")
+ @"'

");
            }

            if (
            txtBatch.Text.Trim()
            != "")
            {
                q.Append(@"

AND TD.Batch LIKE
'%" +
txtBatch.Text
.Replace("'", "''")
+ @"%'

");
            }

            if (
            txtDateFrom.Text.Trim()
            != "")
            {
                q.Append(@"

AND TRY_CONVERT(
date,
TD.DateFrom,
105
) >= '" +
txtDateFrom.Text
+ @"'

");
            }

            if (
            txtDateTo.Text.Trim()
            != "")
            {
                q.Append(@"

AND TRY_CONVERT(
date,
TD.DateTo,
105
) <= '" +
txtDateTo.Text
+ @"'

");
            }
        }

        private void FillGrid(
string query)
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


                gvReport.DataSource =
    dt;

                gvReport.DataBind();

                lblRecordCount.Text =
                "Total Records : " +
                dt.Rows.Count.ToString();
            }
        }
        private int GetTotalEmployees()
        {
            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT COUNT(*)

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(
                q.ToString(),
                con);

                con.Open();

                return Convert.ToInt32(
                cmd.ExecuteScalar());
            }
        }

        private int GetNeverAttendedEver()
        {
            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT COUNT(*)

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            q.Append(@"

AND NOT EXISTS
(
    SELECT 1
    FROM TrainingAssignment TA
    WHERE TA.EmpID = E.EmpID
)

");

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(
                q.ToString(),
                con);

                con.Open();

                return Convert.ToInt32(
                cmd.ExecuteScalar());
            }
        }

        private int GetNeverAttendedSelectedTraining()
        {
            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT COUNT(*)

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            q.Append(@"

AND NOT EXISTS
(
    SELECT 1

    FROM TrainingAssignment TA

    INNER JOIN TrainingDetails TD
    ON TD.TrainingID =
    TA.TrainingID

    WHERE TA.EmpID =
    E.EmpID

");

            ApplyTrainingFilters(q);

            q.Append(@"

)

");

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(
                q.ToString(),
                con);

                con.Open();

                return Convert.ToInt32(
                cmd.ExecuteScalar());
            }
        }

        protected void btnTotalEmployees_Click(
object sender,
EventArgs e)
        {
            BindTotalEmployeesGrid();
        }

        protected void btnNeverAttendedEver_Click(
        object sender,
        EventArgs e)
        {
            BindNeverAttendedEverGrid();
        }

        protected void btnNeverAttendedSelected_Click(
        object sender,
        EventArgs e)
        {
            BindNeverAttendedSelectedGrid();
        }

        protected void btnAttendedSelected_Click(
        object sender,
        EventArgs e)
        {
            BindAttendedSelectedGrid();
        }



        private void BindTotalEmployeesGrid()
        {
            lblCurrentView.Text =
            "Current View : Total Employees";

            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT

E.EmpID,
E.EmpName,
E.EmpCompany,
E.EmpDesignation,
E.EmpPostingPlace,
E.MobileNo,
E.EmailId,

'Employee'
AS Status,

(
SELECT COUNT(*)
FROM TrainingAssignment TA
WHERE TA.EmpID = E.EmpID
)
AS TotalTrainingsAttended

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            q.Append(@"

ORDER BY E.EmpID

");

            FillGrid(q.ToString());
        }

        private void BindNeverAttendedEverGrid()
        {
            lblCurrentView.Text =
            "Current View : Never Attended Ever";

            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT

E.EmpID,
E.EmpName,
E.EmpCompany,
E.EmpDesignation,
E.EmpPostingPlace,
E.MobileNo,
E.EmailId,

'Never Attended Ever'
AS Status,

0 AS TotalTrainingsAttended

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            q.Append(@"

AND NOT EXISTS
(
    SELECT 1
    FROM TrainingAssignment TA
    WHERE TA.EmpID = E.EmpID
)

ORDER BY E.EmpID

");

            FillGrid(q.ToString());
        }


        private void BindNeverAttendedSelectedGrid()
        {
            lblCurrentView.Text =
            "Current View : Never Attended Selected Training";

            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT

E.EmpID,
E.EmpName,
E.EmpCompany,
E.EmpDesignation,
E.EmpPostingPlace,
E.MobileNo,
E.EmailId,

'Never Attended Selected Training'
AS Status,

(
SELECT COUNT(*)
FROM TrainingAssignment TA2
WHERE TA2.EmpID = E.EmpID
)
AS TotalTrainingsAttended

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            q.Append(@"

AND NOT EXISTS
(
    SELECT 1

    FROM TrainingAssignment TA

    INNER JOIN TrainingDetails TD
    ON TD.TrainingID =
    TA.TrainingID

    WHERE TA.EmpID =
    E.EmpID

");

            ApplyTrainingFilters(q);

            q.Append(@"

)

ORDER BY E.EmpID

");

            FillGrid(q.ToString());
        }

        private void BindAttendedSelectedGrid()
        {
            lblCurrentView.Text =
            "Current View : Attended Selected Training";

            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT

E.EmpID,
E.EmpName,
E.EmpCompany,
E.EmpDesignation,
E.EmpPostingPlace,
E.MobileNo,
E.EmailId,

'Attended Selected Training'
AS Status,

(
SELECT COUNT(*)
FROM TrainingAssignment TA2
WHERE TA2.EmpID = E.EmpID
)
AS TotalTrainingsAttended

FROM EmpBasicMaster E

WHERE 1=1

");

            ApplyEmployeeFiltersToQuery(q);

            q.Append(@"

AND EXISTS
(
    SELECT 1

    FROM TrainingAssignment TA

    INNER JOIN TrainingDetails TD
    ON TD.TrainingID =
    TA.TrainingID

    WHERE TA.EmpID =
    E.EmpID

");

            ApplyTrainingFilters(q);

            q.Append(@"

)

ORDER BY E.EmpID

");

            FillGrid(q.ToString());
        }
       

        private void ApplyEmployeeFiltersToQuery(
StringBuilder q)
        {
            string company =
            GetCompanyFilter();

            if (company != "")
            {
                q.Append(@"

AND E.EmpCompany IN
(" + company + @")

");
            }

            string desig =
            GetDesignationFilter();

            if (desig != "")
            {
                q.Append(@"

AND E.EmpDesignation IN
(" + desig + @")

");
            }
        }

        private void LoadCounts()
        {
            int total =
            GetTotalEmployees();

            int neverEver =
            GetNeverAttendedEver();

            int neverSelected =
            GetNeverAttendedSelectedTraining();

            int attendedSelected =
            total - neverSelected;

            btnTotalEmployees.Text =
            "Total Employees (" +
            total +
            ")";

            btnNeverAttendedEver.Text =
            "Never Attended Ever (" +
            neverEver +
            ")";

            btnNeverAttendedSelected.Text =
            "Never Attended Selected Training (" +
            neverSelected +
            ")";

            btnAttendedSelected.Text =
            "Attended Selected Training (" +
            attendedSelected +
            ")";
        }

        protected void btnExport_Click(
        object sender,
        EventArgs e)
        {
            //BindGrid();

            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",

            "attachment;filename=TrainingNotAttendedReport.xls");

            Response.Charset = "";

            Response.ContentType =
            "application/vnd.ms-excel";

            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);

            gvReport.RenderControl(hw);

            Response.Output.Write(
            sw.ToString());

            Response.Flush();

            Response.End();
        }

        public override void
        VerifyRenderingInServerForm(
        Control control)
        {
        }

        protected void btnReset_Click(
        object sender,
        EventArgs e)
        {
            txtBatch.Text = "";

            txtDateFrom.Text = "";

            txtDateTo.Text = "";

            ddlTrainingType.SelectedIndex = 0;

            ddlOrganizer.Items.Clear();

            ddlOrganizer.Items.Insert(
            0,
            new ListItem(
            "--All--",
            ""));

            ddlLocation.Items.Clear();

            ddlLocation.Items.Insert(
            0,
            new ListItem(
            "--All--",
            ""));

            foreach (ListItem item
            in chkCompany.Items)
            {
                item.Selected = false;
            }

            foreach (ListItem item
            in chkDesignation.Items)
            {
                item.Selected = false;
            }

          

            gvReport.DataSource =
            null;

            gvReport.DataBind();

 

            btnTotalEmployees.Text =
 "Total Employees (0)";

            btnNeverAttendedEver.Text =
            "Never Attended Ever (0)";

            btnNeverAttendedSelected.Text =
            "Never Attended Selected Training (0)";

            btnAttendedSelected.Text =
            "Attended Selected Training (0)";

            lblCurrentView.Text = "";

            lblRecordCount.Text = "";
        }
    }
}