using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Employee
{
    public partial class
    FeedbackTrainingRelatedEmployee
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
            BindAspect();
            BindGrid();
        }



        void BindAspect()
        {
            ddlAspect.Items.Clear();

            ddlAspect.Items.Add(
            "Travel Arrangements");

            ddlAspect.Items.Add(
            "Accommodation");

            ddlAspect.Items.Add(
            "Food Arrangements");


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

SELECT
TrainingRelatedAspects

FROM
FeedbackTrainingRelated

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


                SqlDataReader dr =
                cmd.ExecuteReader();

                while (dr.Read())
                {
                    ListItem x =
                    ddlAspect.Items
                    .FindByText(
                    dr["TrainingRelatedAspects"]
                    .ToString());

                    if (x != null)
                        ddlAspect.Items
                        .Remove(x);
                }

                dr.Close();
            }


            if (
            ddlAspect.Items.Count == 0)
            {
                btnSave.Enabled =
                false;

                lblMsg.Text =
                "All aspects submitted";
            }
            else
            {
                btnSave.Enabled =
                true;

                lblMsg.Text = "";
            }
        }

        void BindGrid()
        {
            if (
            ddlTraining.SelectedIndex <= 0)
            {
                gvFeedback.DataSource = null;

                gvFeedback.DataBind();

                return;
            }


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT

TrainingRelatedAspects,
OrganizedBy,
Remarks,
Grading,
CreatedOn

FROM
FeedbackTrainingRelated

WHERE
EmpID=@Emp

AND
TrainingID=@Training

ORDER BY
ID

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


                gvFeedback.DataSource =
                dt;

                gvFeedback.DataBind();
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
ISNULL(
MAX(ID),0)+1

FROM
FeedbackTrainingRelated

", con);

                int no =
                Convert.ToInt32(
                cmd.ExecuteScalar());

                return
                "FTR" +
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
FeedbackTrainingRelated
(
FeedbackTrainingRelated,
EmpID,
TrainingID,
TrainingRelatedAspects,
OrganizedBy,
Remarks,
Grading,
CreatedOn,
CreatedBy
)

VALUES
(
@ID,
@Emp,
@Training,
@Aspect,
@Org,
@Remarks,
@Grade,
GETDATE(),
@CreatedBy
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
                "@Aspect",
                ddlAspect.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@Org",
                txtOrganizedBy.Text);

                cmd.Parameters
                .AddWithValue(
                "@Remarks",
                txtRemarks.Text);

                cmd.Parameters
                .AddWithValue(
                "@Grade",
                ddlGrade.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@CreatedBy",
                Session["UserId"]);

                cmd.ExecuteNonQuery();
            }
            BindAspect();

            BindGrid();

            txtOrganizedBy.Text = "";
            txtRemarks.Text = "";
            ddlGrade.SelectedIndex = 0;

            lblMsg.Text =
            "Saved Successfully";
           
        }
    }
}