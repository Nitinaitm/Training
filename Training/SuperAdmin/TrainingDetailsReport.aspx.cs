using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class TrainingDetailsReport : System.Web.UI.Page
    {
        string constr =
        ConfigurationManager
        .ConnectionStrings["constr"]
        .ConnectionString;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (
                   Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                BindType();
                BindOrganizer();
                BindLocation();
                BindCompany();

                BindDesignation();
                LoadData();
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

FROM TrainingDesignation

ORDER BY EmpDesignation

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);
                chkDesignation.DataSource = dt;

                chkDesignation.DataTextField =
                "EmpDesignation";

                chkDesignation.DataValueField =
                "EmpDesignation";

                chkDesignation.DataBind();
                //lstDesignation.DataSource =
                //dt;

                //lstDesignation.DataTextField =
                //"EmpDesignation";

                //lstDesignation.DataValueField =
                //"EmpDesignation";

                //lstDesignation.DataBind();
            }
        }
        private void BindType()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

                SELECT
                TrainingTypeID,
                TrainingType

                FROM TrainingMaster
                ORDER BY TrainingType",

                con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlType.DataSource = dt;

                ddlType.DataTextField =
                "TrainingType";

                ddlType.DataValueField =
                "TrainingTypeID";

                ddlType.DataBind();

                ddlType.Items.Insert(
                0,
                new ListItem(
                "All",
                ""));
            }
        }


        protected void ddlType_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindOrganizer();

            ddlLocation.Items.Clear();

            ddlLocation.Items.Insert(
            0,
            new ListItem(
            "All",
            ""));

            LoadData();
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

WHERE
ISNULL(EmpCompany,'')<>''

ORDER BY EmpCompany

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                chkCompany.DataSource = dt;

                chkCompany.DataTextField =
                "EmpCompany";

                chkCompany.DataValueField =
                "EmpCompany";

                chkCompany.DataBind();
            }
        }
        private void BindOrganizer()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                string q = @"

                SELECT
                TrainingOrganizerID,
                TrainingOrganizer

                FROM
                TrainingOrganizerMaster

                WHERE 1=1";


                SqlCommand cmd =
                new SqlCommand(q, con);


                if (ddlType.SelectedValue != "")
                {
                    q +=
                    " and TrainingTypeID=@id";

                    cmd.CommandText = q;

                    cmd.Parameters
                    .AddWithValue(
                    "@id",
                    ddlType.SelectedValue);
                }


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
                "TrainingOrganizerID";

                ddlOrganizer.DataBind();

                ddlOrganizer.Items.Insert(
                0,
                new ListItem(
                "All",
                ""));
            }
        }


        protected void ddlOrganizer_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindLocation();

            LoadData();
        }


        private void BindLocation()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                string q = @"

                SELECT

                TrainingLocationID,
                TrainingLocation

                FROM
                TrainingLocationMaster

                WHERE 1=1";


                SqlCommand cmd =
                new SqlCommand(q, con);


                if (ddlOrganizer.SelectedValue != "")
                {
                    q +=
                    " and TrainingOrganizerID=@id";

                    cmd.CommandText = q;

                    cmd.Parameters
                    .AddWithValue(
                    "@id",
                    ddlOrganizer.SelectedValue);
                }


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
                "TrainingLocationID";

                ddlLocation.DataBind();

                ddlLocation.Items.Insert(
                0,
                new ListItem(
                "All",
                ""));
            }
        }



        protected void btnSearch_Click(
        object sender,
        EventArgs e)
        {
            LoadData();
        }


        protected void btnReset_Click(
        object sender,
        EventArgs e)
        {
            txtTrainingID.Text = "";
            txtBatch.Text = "";
            txtDateFrom.Text = "";
            txtDateTo.Text = "";

            ddlType.SelectedIndex = 0;

            BindOrganizer();
            BindLocation();
            foreach (ListItem item
in chkCompany.Items)
            {
                item.Selected = false;
            }
            //ddlCompany.SelectedIndex = 0;
            //            foreach (ListItem item
            //in lstDesignation.Items)
            //            {
            //                item.Selected = false;
            //            }
            foreach (ListItem item
            in chkDesignation.Items)
            {
                item.Selected = false;
            }
            LoadData();
        }

        private int GetTotalEmployeeCount()
        {
            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT
COUNT(TA.EmpID)

FROM TrainingDetails TD

INNER JOIN TrainingAssignment TA
ON TD.TrainingID = TA.TrainingID

WHERE 1=1

");

            SqlCommand cmd =
            new SqlCommand();


            if (txtTrainingID.Text != "")
            {
                q.Append(
                " AND TD.TrainingID LIKE @id");

                cmd.Parameters.AddWithValue(
                "@id",
                "%" +
                txtTrainingID.Text +
                "%");
            }


            if (txtBatch.Text != "")
            {
                q.Append(
                " AND TD.Batch LIKE @batch");

                cmd.Parameters.AddWithValue(
                "@batch",
                "%" +
                txtBatch.Text +
                "%");
            }


            if (txtDateFrom.Text.Trim() != "")
            {
                DateTime fromDate =
                DateTime.ParseExact(
                txtDateFrom.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);

                q.Append(@"

AND TRY_CONVERT(
date,
TD.DateFrom,
105
) >= @from");

                cmd.Parameters.AddWithValue(
                "@from",
                fromDate);
            }


            if (txtDateTo.Text.Trim() != "")
            {
                DateTime toDate =
                DateTime.ParseExact(
                txtDateTo.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);

                q.Append(@"

AND TRY_CONVERT(
date,
TD.DateTo,
105
) <= @to");

                cmd.Parameters.AddWithValue(
                "@to",
                toDate);
            }


            if (ddlType.SelectedValue != "")
            {
                q.Append(
                " AND TD.TrainingType=@type");

                cmd.Parameters.AddWithValue(
                "@type",
                ddlType.SelectedItem.Text);
            }


            if (ddlOrganizer.SelectedValue != "")
            {
                q.Append(
                " AND TD.TrainingOrganizer=@org");

                cmd.Parameters.AddWithValue(
                "@org",
                ddlOrganizer.SelectedItem.Text);
            }


            if (ddlLocation.SelectedValue != "")
            {
                q.Append(
                " AND TD.TrainingLocation=@loc");

                cmd.Parameters.AddWithValue(
                "@loc",
                ddlLocation.SelectedItem.Text);
            }


            bool hasCompany = false;

            foreach (ListItem item
            in chkCompany.Items)
            {
                if (item.Selected)
                {
                    hasCompany = true;
                    break;
                }
            }

            //            if (hasCompany)
            //            {
            //                StringBuilder comp =
            //                new StringBuilder();

            //                int i = 0;

            //                foreach (ListItem item
            //                in chkCompany.Items)
            //                {
            //                    if (item.Selected)
            //                    {
            //                        if (i > 0)
            //                            comp.Append(",");

            //                        comp.Append(
            //                        "'" +
            //                        item.Value.Replace("'", "''")
            //                        + "'");

            //                        i++;
            //                    }
            //                }

            //                q.Append(@"

            //AND EXISTS
            //(
            //    SELECT 1

            //    FROM TrainingAssignment TA1

            //    INNER JOIN EmpBasicMaster E1
            //    ON E1.EmpID = TA1.EmpID

            //    WHERE TA1.TrainingID = TD.TrainingID

            //    AND E1.EmpCompany IN
            //    (" + comp.ToString() + @")
            //)

            //");
            //            }
            if (hasCompany)
            {
                StringBuilder comp =
                new StringBuilder();

                int i = 0;

                foreach (ListItem item in chkCompany.Items)
                {
                    if (item.Selected)
                    {
                        if (i > 0)
                            comp.Append(",");

                        comp.Append(
                        "'" +
                        item.Value.Replace("'", "''")
                        + "'");

                        i++;
                    }
                }

                q.Append(@"

AND EXISTS
(
    SELECT 1
    FROM EmpBasicMaster E1

    WHERE E1.EmpID = TA.EmpID

    AND E1.EmpCompany IN
    (" + comp.ToString() + @")
)

");
            }
            bool hasDesignation = false;

            foreach (ListItem item
            in chkDesignation.Items)
            {
                if (item.Selected)
                {
                    hasDesignation = true;
                    break;
                }
            }


            if (hasDesignation)
            {
                StringBuilder desig =
                new StringBuilder();

                int i = 0;

                foreach (ListItem item
                in chkDesignation.Items)
                {
                    if (item.Selected)
                    {
                        if (i > 0)
                            desig.Append(",");

                        desig.Append(
                        "'" +
                        item.Value.Replace("'", "''")
                        + "'");

                        i++;
                    }
                }

                q.Append(@"

AND EXISTS
(
    SELECT 1
    FROM TrainingDesignation TDES

    WHERE TDES.TrainingID =
    TD.TrainingID

    AND TDES.EmpDesignation IN
    (" + desig.ToString() + @")
)

");
            }


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                cmd.Connection = con;

                cmd.CommandText =
                q.ToString();

                con.Open();

                return Convert.ToInt32(
                cmd.ExecuteScalar());
            }
        }

        private void LoadData()
        {
            StringBuilder q =
            new StringBuilder();


            q.Append(@"

            SELECT

            TD.TrainingID,
            TD.TrainingType,
            TD.TrainingOrganizer,
            TD.TrainingLocation,
            TD.Batch,
            TD.DateFrom,
            TD.DateTo,

           
COUNT(TA.EmpID)
AS TotalAssigned,

(
    SELECT COUNT(*)
    FROM TrainingAssignment TA2
    WHERE TA2.TrainingID = TD.TrainingID
)
AS TotalBatchStrength,

SUM(
CASE
WHEN TA.TrainingAttended='Yes'
THEN 1
ELSE 0
END
)
AS TotalAttended
           

            FROM TrainingDetails TD

            LEFT JOIN TrainingAssignment TA
            ON TD.TrainingID=TA.TrainingID

            WHERE 1=1");


            SqlCommand cmd =
            new SqlCommand();


            if (txtTrainingID.Text != "")
            {
                q.Append(
                " and TD.TrainingID like @id");

                cmd.Parameters
                .AddWithValue(
                "@id",
                "%" +
                txtTrainingID.Text +
                "%");
            }


            if (txtBatch.Text != "")
            {
                q.Append(
                " and TD.Batch like @batch");

                cmd.Parameters
                .AddWithValue(
                "@batch",
                "%" +
                txtBatch.Text +
                "%");
            }


            //if (txtDateFrom.Text != "")
            //{
            //    q.Append(
            //    " and TD.DateFrom>=@from");

            //    cmd.Parameters
            //    .AddWithValue(
            //    "@from",
            //    txtDateFrom.Text);
            //}


            //if (txtDateTo.Text != "")
            //{
            //    q.Append(
            //    " and TD.DateTo<=@to");

            //    cmd.Parameters
            //    .AddWithValue(
            //    "@to",
            //    txtDateTo.Text);
            //}
            if (txtDateFrom.Text.Trim() != "")
            {
                DateTime fromDate =
                DateTime.ParseExact(
                txtDateFrom.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);

                q.Append(@"

    and
    TRY_CONVERT(
    date,
    TD.DateFrom,
    105
    )>=@from");

                cmd.Parameters
                .AddWithValue(
                "@from",
                fromDate);
            }



            if (txtDateTo.Text.Trim() != "")
            {
                DateTime toDate =
                DateTime.ParseExact(
                txtDateTo.Text,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture);

                q.Append(@"

    and
    TRY_CONVERT(
    date,
    TD.DateTo,
    105
    )<=@to");

                cmd.Parameters
                .AddWithValue(
                "@to",
                toDate);
            }

            if (ddlType.SelectedValue != "")
            {
                q.Append(
                " and TD.TrainingType=@type");

                cmd.Parameters
                .AddWithValue(
                "@type",
                ddlType.SelectedItem.Text);
            }


            if (ddlOrganizer.SelectedValue != "")
            {
                q.Append(
                " and TD.TrainingOrganizer=@org");

                cmd.Parameters
                .AddWithValue(
                "@org",
                ddlOrganizer.SelectedItem.Text);
            }


            if (ddlLocation.SelectedValue != "")
            {
                q.Append(
                " and TD.TrainingLocation=@loc");

                cmd.Parameters
                .AddWithValue(
                "@loc",
                ddlLocation.SelectedItem.Text);
            }

            bool hasCompany = false;

            foreach (ListItem item
            in chkCompany.Items)
            {
                if (item.Selected)
                {
                    hasCompany = true;
                    break;
                }
            }
            if (hasCompany)
            {
                StringBuilder comp =
                new StringBuilder();

                int i = 0;

                foreach (ListItem item in chkCompany.Items)
                {
                    if (item.Selected)
                    {
                        if (i > 0)
                            comp.Append(",");

                        comp.Append(
                        "'" +
                        item.Value.Replace("'", "''")
                        + "'");

                        i++;
                    }
                }

                q.Append(@"

AND EXISTS
(
    SELECT 1
    FROM EmpBasicMaster E1

    WHERE E1.EmpID = TA.EmpID

    AND E1.EmpCompany IN
    (" + comp.ToString() + @")
)

");
            }
            //            if (hasCompany)
            //            {
            //                StringBuilder comp =
            //                new StringBuilder();

            //                int i = 0;

            //                foreach (ListItem item
            //                in chkCompany.Items)
            //                {
            //                    if (item.Selected)
            //                    {
            //                        if (i > 0)
            //                            comp.Append(",");

            //                        comp.Append(
            //                        "'" +
            //                        item.Value.Replace("'", "''")
            //                        + "'");

            //                        i++;
            //                    }
            //                }

            //                q.Append(@"

            //AND EXISTS
            //(
            //    SELECT 1

            //    FROM TrainingAssignment TA1

            //    INNER JOIN EmpBasicMaster E1
            //    ON E1.EmpID = TA1.EmpID

            //    WHERE TA1.TrainingID = TD.TrainingID

            //    AND E1.EmpCompany IN
            //    (" + comp.ToString() + @")
            //)

            //");
            //            }

            bool hasDesignation = false;

            foreach (ListItem item in chkDesignation.Items)
            {
                if (item.Selected)
                {
                    hasDesignation = true;
                    break;
                }
            }

            if (hasDesignation)
            {
                StringBuilder desig =
                new StringBuilder();

                int i = 0;

                foreach (ListItem item
                in chkDesignation.Items)
                {
                    if (item.Selected)
                    {
                        if (i > 0)
                            desig.Append(",");

                        desig.Append(
                        "'" +
                        item.Value
                        .Replace("'", "''")
                        + "'");

                        i++;
                    }
                }

                q.Append(@"

AND EXISTS
(
    SELECT 1
    FROM TrainingDesignation TDES
    WHERE TDES.TrainingID = TD.TrainingID
    AND TDES.EmpDesignation IN
    (" + desig.ToString() + @")
)

");
            }

            q.Append(@"

            GROUP BY

            TD.TrainingID,
            TD.TrainingType,
            TD.TrainingOrganizer,
            TD.TrainingLocation,
            TD.Batch,
            TD.DateFrom,
            TD.DateTo

            ORDER BY TD.DateFrom DESC");


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                cmd.Connection = con;

                cmd.CommandText =
                q.ToString();

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvTraining.DataSource = dt;
                gvTraining.DataBind();
            }
            lblTotalEmployee.Text =
"Total Employees : " +
GetTotalEmployeeCount().ToString();
        }



        protected void gvTraining_RowCommand(
        object sender,
        GridViewCommandEventArgs e)
        {
            string trainingID =
            e.CommandArgument
            .ToString();

            string q = "";


            //if (e.CommandName == "Assigned")
            //{
            //    q = @"

            //    SELECT

            //    T.EmpID,

            //    ISNULL(E.EmpName,'Not Found')
            //    AS EmpName,

            //    ISNULL(E.EmpDesignation,'Not Found')
            //    AS EmpDesignation,

            //    ISNULL(E.EmpCompany,'Not Found')
            //    AS EmpCompany,

            //    ISNULL(E.EmpPostingPlace,'Not Found')
            //    AS EmpPostingPlace,

            //    ISNULL(
            //    T.TrainingAttended,
            //    'Pending')
            //    AS TrainingStatus

            //    FROM TrainingAssignment T

            //    LEFT JOIN EmpBasicMaster E

            //    ON E.EmpID=T.EmpID

            //    WHERE
            //    T.TrainingID=@id";
            //}

            if (e.CommandName == "TotalAssigned")
            {
                q = @"

    SELECT

    T.EmpID,

    ISNULL(E.EmpName,'Not Found')
    AS EmpName,

    ISNULL(E.EmpDesignation,'Not Found')
    AS EmpDesignation,

    ISNULL(E.EmpCompany,'Not Found')
    AS EmpCompany,

    ISNULL(E.EmpPostingPlace,'Not Found')
    AS EmpPostingPlace,

    ISNULL(
    T.TrainingAttended,
    'Pending')
    AS TrainingStatus

    FROM TrainingAssignment T

    LEFT JOIN EmpBasicMaster E
    ON E.EmpID=T.EmpID

    WHERE T.TrainingID=@id";
            }

            if (e.CommandName == "FilteredAssigned")
            {
                StringBuilder qBuilder =
                new StringBuilder();

                qBuilder.Append(@"

    SELECT

    T.EmpID,

    ISNULL(E.EmpName,'Not Found')
    AS EmpName,

    ISNULL(E.EmpDesignation,'Not Found')
    AS EmpDesignation,

    ISNULL(E.EmpCompany,'Not Found')
    AS EmpCompany,

    ISNULL(E.EmpPostingPlace,'Not Found')
    AS EmpPostingPlace,

    ISNULL(
    T.TrainingAttended,
    'Pending')
    AS TrainingStatus

    FROM TrainingAssignment T

    INNER JOIN EmpBasicMaster E
    ON E.EmpID=T.EmpID

    WHERE T.TrainingID=@id

    ");

                if (HasCompanyFilter())
                {
                    qBuilder.Append(@"

AND E.EmpCompany IN
(" + GetSelectedCompanies() + @")

");
                }

                if (HasDesignationFilter())
                {
                    qBuilder.Append(@"

AND E.EmpDesignation IN
(" + GetSelectedDesignations() + @")

");
                }

                q = qBuilder.ToString();
            }
            if (e.CommandName == "Attended")
            {
                q = @"

                SELECT

                T.EmpID,

                ISNULL(E.EmpName,'Not Found')
                AS EmpName,

                ISNULL(E.EmpDesignation,'Not Found')
                AS EmpDesignation,

                ISNULL(E.EmpCompany,'Not Found')
                AS EmpCompany,

                ISNULL(E.EmpPostingPlace,'Not Found')
                AS EmpPostingPlace,

                T.TrainingAttended
                AS TrainingStatus

                FROM TrainingAssignment T

                LEFT JOIN EmpBasicMaster E

                ON E.EmpID=T.EmpID

                WHERE
                T.TrainingID=@id

                AND
                T.TrainingAttended='Yes'";
            }


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(q, con);

                cmd.Parameters
                .AddWithValue(
                "@id",
                trainingID);

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvEmployeeDetails.DataSource = dt;
                gvEmployeeDetails.DataBind();

                Session["PopupData"] = dt;
            }


            ScriptManager
            .RegisterStartupScript(
            this,
            GetType(),
            "popup",

            "var m=new bootstrap.Modal(document.getElementById('empModal'));m.show();",

            true);
        }



        protected void btnExport_Click(
        object sender,
        EventArgs e)
        {
            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",
            "attachment;filename=TrainingReport.xls");

            Response.ContentType =
            "application/ms-excel";

            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);

            gvTraining.RenderControl(hw);

            Response.Write(sw.ToString());

            Response.End();
        }



        protected void btnPopupExport_Click(
        object sender,
        EventArgs e)
        {
            DataTable dt =
            Session["PopupData"]
            as DataTable;

            if (dt == null)
                return;

            gvEmployeeDetails.DataSource = dt;
            gvEmployeeDetails.DataBind();

            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",
            "attachment;filename=EmployeeDetails.xls");

            Response.ContentType =
            "application/ms-excel";

            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);

            gvEmployeeDetails.RenderControl(hw);

            //Response.Write(sw.ToString());

            //Response.End();

            Response.Write(sw.ToString());

            Response.Flush();

            Response.SuppressContent = true;

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }


        public override void VerifyRenderingInServerForm(
        Control control)
        {

        }
        private bool HasCompanyFilter()
        {
            foreach (ListItem item
            in chkCompany.Items)
            {
                if (item.Selected)
                    return true;
            }

            return false;
        }
        private string GetSelectedCompanies()
        {
            StringBuilder sb =
            new StringBuilder();

            int i = 0;

            foreach (ListItem item
            in chkCompany.Items)
            {
                if (item.Selected)
                {
                    if (i > 0)
                        sb.Append(",");

                    sb.Append("'" +
                    item.Value.Replace("'", "''")
                    + "'");

                    i++;
                }
            }

            return sb.ToString();
        }
        private bool HasDesignationFilter()
        {
            foreach (ListItem item
            in chkDesignation.Items)
            {
                if (item.Selected)
                    return true;
            }

            return false;
        }
        private string GetSelectedDesignations()
        {
            StringBuilder sb =
            new StringBuilder();

            int i = 0;

            foreach (ListItem item
            in chkDesignation.Items)
            {
                if (item.Selected)
                {
                    if (i > 0)
                        sb.Append(",");

                    sb.Append("'" +
                    item.Value.Replace("'", "''")
                    + "'");

                    i++;
                }
            }

            return sb.ToString();
        }

    }
}