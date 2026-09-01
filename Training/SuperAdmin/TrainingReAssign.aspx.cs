using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class TrainingReAssign : System.Web.UI.Page
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
                if (Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }

                BindTrainingType();

                ddlOrganizer.Items.Clear();
                ddlOrganizer.Items.Insert(
                0,
                new ListItem("--Select--", ""));

                ddlLocation.Items.Clear();
                ddlLocation.Items.Insert(
                0,
                new ListItem("--Select--", ""));
                BindDesignation();

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

        protected void ddlTrainingType_SelectedIndexChanged(
object sender,
EventArgs e)
        {
            BindOrganizer();

            ddlLocation.Items.Clear();

            ddlLocation.Items.Insert(
            0,
            new ListItem("--Select--", ""));
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
            BindGrid();
        }

        private void BindGrid()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"


SELECT

TA.ID,
TA.TrainingID,
TD.TrainingType,
TD.TrainingOrganizer,
TD.TrainingLocation,
TD.Batch,
TD.DateFrom,
TD.DateTo

FROM TrainingAssignment TA
INNER JOIN TrainingDetails TD
ON TA.TrainingID = TD.TrainingID




WHERE TA.EmpID = @EmpID

ORDER BY TD.DateFrom DESC

", con);

                cmd.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim());

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvTraining.DataSource = dt;
                gvTraining.DataBind();
            }
        }
        protected void gvTraining_RowCommand(
object sender,
GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ReAssign")
            {
                hfAssignmentID.Value =
                e.CommandArgument.ToString();

                pnlReAssign.Visible = true;

                rblTrainingID.Items.Clear();

                txtNewTrainingID.Text = "";
            }
        }
        //protected void gvTraining_RowCommand(
        //object sender,
        //GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName == "ReAssign")
        //    {
        //        hfOldTrainingID.Value =
        //        e.CommandArgument.ToString();

        //        pnlReAssign.Visible = true;
        //    }
        //}

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

                ddlTrainingType.DataSource = dt;
                ddlTrainingType.DataTextField =
                "TrainingType";
                ddlTrainingType.DataValueField =
                "TrainingType";
                ddlTrainingType.DataBind();

                ddlTrainingType.Items.Insert(
                0,
                new ListItem("--Select--", ""));
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

                ddlOrganizer.DataSource = dt;
                ddlOrganizer.DataTextField =
                "TrainingOrganizer";
                ddlOrganizer.DataValueField =
                "TrainingOrganizer";
                ddlOrganizer.DataBind();

                ddlOrganizer.Items.Insert(
                0,
                new ListItem("--Select--", ""));
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

        //                ddlOrganizer.DataSource = dt;
        //                ddlOrganizer.DataTextField =
        //                "TrainingOrganizer";
        //                ddlOrganizer.DataValueField =
        //                "TrainingOrganizer";
        //                ddlOrganizer.DataBind();

        //                ddlOrganizer.Items.Insert(
        //                0,
        //                new ListItem("--Select--", ""));
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

                ddlLocation.DataSource = dt;
                ddlLocation.DataTextField =
                "TrainingLocation";
                ddlLocation.DataValueField =
                "TrainingLocation";
                ddlLocation.DataBind();

                ddlLocation.Items.Insert(
                0,
                new ListItem("--Select--", ""));
            }
        }
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

        //                ddlLocation.DataSource = dt;
        //                ddlLocation.DataTextField =
        //                "TrainingLocation";
        //                ddlLocation.DataValueField =
        //                "TrainingLocation";
        //                ddlLocation.DataBind();

        //                ddlLocation.Items.Insert(
        //                0,
        //                new ListItem("--Select--", ""));
        //            }
        //        }
        protected void btnSearchTraining_Click(
object sender,
EventArgs e)
        {
            StringBuilder q =
            new StringBuilder();

            q.Append(@"

SELECT TD.TrainingID

FROM TrainingDetails TD

WHERE 1=1

");

            SqlCommand cmd =
            new SqlCommand();

            if (ddlTrainingType.SelectedValue != "")
            {
                q.Append(
                " AND TrainingType=@Type");

                cmd.Parameters.AddWithValue(
                "@Type",
                ddlTrainingType.SelectedValue);
            }

            if (ddlOrganizer.SelectedValue != "")
            {
                q.Append(
                " AND TrainingOrganizer=@Organizer");

                cmd.Parameters.AddWithValue(
                "@Organizer",
                ddlOrganizer.SelectedValue);
            }

            if (ddlLocation.SelectedValue != "")
            {
                q.Append(
                " AND TrainingLocation=@Location");

                cmd.Parameters.AddWithValue(
                "@Location",
                ddlLocation.SelectedValue);
            }

            if (txtBatch.Text.Trim() != "")
            {
                q.Append(
                " AND Batch=@Batch");

                cmd.Parameters.AddWithValue(
                "@Batch",
                txtBatch.Text.Trim());
            }

            if (txtDateFrom.Text.Trim() != "")
            {
                q.Append(
                " AND DateFrom=@DateFrom");

                cmd.Parameters.AddWithValue(
                "@DateFrom",
                txtDateFrom.Text.Trim());
            }

            if (txtDateTo.Text.Trim() != "")
            {
                q.Append(
                " AND DateTo=@DateTo");

                cmd.Parameters.AddWithValue(
                "@DateTo",
                txtDateTo.Text.Trim());
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
                cmd.CommandText = q.ToString();

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                rblTrainingID.DataSource =
                dt;

                rblTrainingID.DataTextField =
                "TrainingID";

                rblTrainingID.DataValueField =
                "TrainingID";

                rblTrainingID.DataBind();
            }
        }
        

        protected void btnSaveReAssign_Click(
object sender,
EventArgs e)
        {
            string newTrainingID = "";

            if (txtNewTrainingID.Text.Trim() != "")
            {
                newTrainingID =
                txtNewTrainingID.Text.Trim();
            }
            else if (
            rblTrainingID.SelectedIndex >= 0)
            {
                newTrainingID =
                rblTrainingID.SelectedValue;
            }

            if (newTrainingID == "")
            {
                ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('Please enter or select Training ID.');",
                true);

                return;
            }

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand chk =
                new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment

WHERE TrainingID=@TrainingID
AND EmpID=@EmpID

", con);

                chk.Parameters.AddWithValue(
                "@TrainingID",
                newTrainingID);

                chk.Parameters.AddWithValue(
                "@EmpID",
                txtEmpID.Text.Trim());

                int cnt =
                Convert.ToInt32(
                chk.ExecuteScalar());

                if (cnt > 0)
                {
                    con.Close();

                    ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Employee is already assigned to this training.');",
                    true);

                    return;
                }

                SqlCommand cmd =
                new SqlCommand(@"

UPDATE TrainingAssignment

SET TrainingID=@NewTrainingID

WHERE ID=@ID

", con);

                cmd.Parameters.AddWithValue(
                "@NewTrainingID",
                newTrainingID);

                cmd.Parameters.AddWithValue(
                "@ID",
                Convert.ToInt32(
                hfAssignmentID.Value));

                cmd.ExecuteNonQuery();

                con.Close();
            }

            pnlReAssign.Visible = false;

            //ScriptManager.RegisterStartupScript(
            //this,
            //GetType(),
            //"msg",
            //"alert('Training reassigned successfully.');",
            //true);

            BindGrid();
        }
    }
}