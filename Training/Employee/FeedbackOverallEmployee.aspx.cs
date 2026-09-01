using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Employee
{
    public partial class
    FeedbackOverallEmployee
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
                if (
                Session["InternalRedirect"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }

                BindTraining();
            }
        }



        void BindTraining()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
TrainingID

FROM TrainingAssignment

WHERE EmpID=@Emp

", con);


                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@Emp",
                Session["UserId"]);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlTraining.DataSource =
                dt;

                ddlTraining.DataTextField =
                "TrainingID";

                ddlTraining.DataValueField =
                "TrainingID";

                ddlTraining.DataBind();

                ddlTraining.Items.Insert(
                0,
                new ListItem(
                "--Select Training--",
                ""));
            }
        }



        protected void
        ddlTraining_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            CheckEntry();

            BindGrid();
        }



        void CheckEntry()
        {
            txtOverall.Enabled = true;

            btnSave.Enabled = true;


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

SELECT COUNT(*)

FROM FeedbackOverall

WHERE
EmpID=@Emp

AND
TrainingID=@Training

", con);


                cmd.Parameters
                .AddWithValue(
                "@Emp",
                Session["UserId"]);

                cmd.Parameters
                .AddWithValue(
                "@Training",
                ddlTraining.SelectedValue);


                int cnt =
                Convert.ToInt32(
                cmd.ExecuteScalar());


                if (cnt > 0)
                {
                    txtOverall.Enabled =
                    false;

                    btnSave.Enabled =
                    false;

                    lblMsg.Text =
                    "Already submitted";
                }
                else
                {
                    lblMsg.Text = "";
                }
            }
        }



        void BindGrid()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT
OverallResponse,
CreatedOn

FROM FeedbackOverall

WHERE
EmpID=@Emp

AND
TrainingID=@Training

", con);


                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@Emp",
                Session["UserId"]);

                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@Training",
                ddlTraining.SelectedValue);


                DataTable dt =
                new DataTable();

                da.Fill(dt);


                gvOverall.DataSource =
                dt;

                gvOverall.DataBind();
            }
        }



        string GenerateID()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

SELECT
ISNULL(MAX(ID),0)+1

FROM FeedbackOverall

", con);

                int no =
                Convert.ToInt32(
                cmd.ExecuteScalar());

                return
                "FOR" +
                no.ToString("0000");
            }
        }



        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

INSERT INTO
FeedbackOverall
(
OverallID,
EmpID,
TrainingID,
OverallResponse,
CreatedOn,
CreatedBy
)

VALUES
(
@ID,
@Emp,
@Training,
@Response,
GETDATE(),
@By
)

", con);


                cmd.Parameters
                .AddWithValue(
                "@ID",
                GenerateID());

                cmd.Parameters
                .AddWithValue(
                "@Emp",
                Session["UserId"]);

                cmd.Parameters
                .AddWithValue(
                "@Training",
                ddlTraining.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@Response",
                txtOverall.Text);

                cmd.Parameters
                .AddWithValue(
                "@By",
                Session["UserId"]);

                cmd.ExecuteNonQuery();
            }


            CheckEntry();

            BindGrid();

            lblMsg.Text =
            "Saved Successfully";
        }
    }
}