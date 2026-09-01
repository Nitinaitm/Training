using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FindTrainingID : System.Web.UI.Page
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

                BindDesignation();
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
                new ListItem("--All--", ""));
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
                new ListItem("--All--", ""));
            }
        }
        //        private void BindOrganizer()
        //        {
        //            using (SqlConnection con =
        //            new SqlConnection(constr))
        //            {
        //                SqlDataAdapter da =
        //                new SqlDataAdapter(@"

        //SELECT DISTINCT
        //TrainingOrganizer

        //FROM TrainingDetails

        //ORDER BY TrainingOrganizer

        //", con);

        //                DataTable dt =
        //                new DataTable();

        //                da.Fill(dt);

        //                ddlOrganizer.DataSource =
        //                dt;

        //                ddlOrganizer.DataTextField =
        //                "TrainingOrganizer";

        //                ddlOrganizer.DataValueField =
        //                "TrainingOrganizer";

        //                ddlOrganizer.DataBind();

        //                ddlOrganizer.Items.Insert(
        //                0,
        //                new ListItem("--All--", ""));
        //            }
        //        }

        //        private void BindLocation()
        //        {
        //            using (SqlConnection con =
        //            new SqlConnection(constr))
        //            {
        //                SqlDataAdapter da =
        //                new SqlDataAdapter(@"

        //SELECT DISTINCT
        //TrainingLocation

        //FROM TrainingDetails

        //ORDER BY TrainingLocation

        //", con);

        //                DataTable dt =
        //                new DataTable();

        //                da.Fill(dt);

        //                ddlLocation.DataSource =
        //                dt;

        //                ddlLocation.DataTextField =
        //                "TrainingLocation";

        //                ddlLocation.DataValueField =
        //                "TrainingLocation";

        //                ddlLocation.DataBind();

        //                ddlLocation.Items.Insert(
        //                0,
        //                new ListItem("--All--", ""));
        //            }
        //        }
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
                new ListItem("--All--", ""));
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

                chkDesignation.DataSource =
                dt;

                chkDesignation.DataTextField =
                "EmpDesignation";

                chkDesignation.DataValueField =
                "EmpDesignation";

                chkDesignation.DataBind();
            }
        }

        protected void btnSearch_Click(
        object sender,
        EventArgs e)
        {
            BindGrid();
        }
        private void BindGrid()
        {
            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT DISTINCT

TD.TrainingID,
TD.TrainingType,
TD.TrainingOrganizer,
TD.TrainingLocation,
TD.Batch,
TD.DateFrom,
TD.DateTo,

(
    SELECT STRING_AGG(
    TDES.EmpDesignation,
    ', ')
    
    FROM TrainingDesignation TDES
    
    WHERE TDES.TrainingID =
    TD.TrainingID
) AS Designation

FROM TrainingDetails TD

WHERE 1=1
");

            SqlCommand cmd =
            new SqlCommand();

            if (ddlTrainingType.SelectedValue != "")
            {
                q.Append(
                " AND TD.TrainingType=@Type");

                cmd.Parameters.AddWithValue(
                "@Type",
                ddlTrainingType.SelectedValue);
            }

            if (ddlOrganizer.SelectedValue != "")
            {
                q.Append(
                " AND TD.TrainingOrganizer=@Organizer");

                cmd.Parameters.AddWithValue(
                "@Organizer",
                ddlOrganizer.SelectedValue);
            }

            if (ddlLocation.SelectedValue != "")
            {
                q.Append(
                " AND TD.TrainingLocation=@Location");

                cmd.Parameters.AddWithValue(
                "@Location",
                ddlLocation.SelectedValue);
            }

            if (txtBatch.Text.Trim() != "")
            {
                q.Append(
                " AND TD.Batch LIKE @Batch");

                cmd.Parameters.AddWithValue(
                "@Batch",
                "%" +
                txtBatch.Text.Trim() +
                "%");
            }

            if (txtDateFrom.Text.Trim() != "")
            {
                q.Append(@"

AND TRY_CONVERT(
date,
TD.DateFrom,
105
) >= @DateFrom

");

                cmd.Parameters.AddWithValue(
                "@DateFrom",
                Convert.ToDateTime(
                txtDateFrom.Text));
            }

            if (txtDateTo.Text.Trim() != "")
            {
                q.Append(@"

AND TRY_CONVERT(
date,
TD.DateTo,
105
) <= @DateTo

");

                cmd.Parameters.AddWithValue(
                "@DateTo",
                Convert.ToDateTime(
                txtDateTo.Text));
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

    WHERE TDES.TrainingID =
    TD.TrainingID

    AND TDES.EmpDesignation IN
    (" + desig.ToString() + @")
)

");
            }

            q.Append(@"

ORDER BY
TD.TrainingID DESC
");

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

                gvTraining.DataSource =
                dt;

                gvTraining.DataBind();
            }
        }
    }
}