using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class TrainingUnAssign : System.Web.UI.Page
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
            if (txtEmpID.Text.Trim() == "")
            {
                gvTraining.DataSource = null;
                gvTraining.DataBind();
                return;
            }

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

WHERE TA.EmpID=@EmpID

ORDER BY
TRY_CONVERT(
date,
TD.DateFrom,
105
) DESC

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
            if (e.CommandName == "UnAssign")
            {
                int id =
                Convert.ToInt32(
                e.CommandArgument);

                using (SqlConnection con =
                new SqlConnection(constr))
                {
                    SqlCommand cmd =
                    new SqlCommand(@"

DELETE FROM TrainingAssignment
WHERE ID=@ID

", con);

                    cmd.Parameters.AddWithValue(
                    "@ID",
                    id);

                    con.Open();

                    int result =
                    cmd.ExecuteNonQuery();

                    con.Close();

                    //if (result > 0)
                    //{
                    //    ScriptManager.RegisterStartupScript(
                    //    this,
                    //    GetType(),
                    //    "msg",
                    //    "alert('Employee has been unassigned successfully.');",
                    //    true);
                    //}
                    //else
                    //{
                    //    ScriptManager.RegisterStartupScript(
                    //    this,
                    //    GetType(),
                    //    "msg",
                    //    "alert('No record found.');",
                    //    true);
                    //}
                }

                BindGrid();
            }
        }
    }
}