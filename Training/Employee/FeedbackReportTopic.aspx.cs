using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Employee
{
    public partial class FeedbackReportTopic :
    System.Web.UI.Page
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
                if (Session["InternalRedirect"] == null || ((bool)Session["InternalRedirect"] == false))
                {
                    Response.Redirect("~/Default.aspx");
                }
                BindTraining();

                CreateTable();
            }
        }



        void BindTraining()
        {
            string emp =
    Session["UserId"].ToString() != null
    ?
    Session["UserId"].ToString()
    :
    "";

            if (emp == "")
            {
                Response.Redirect(
                "~/Default.aspx");

                return;
            }


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
T.TrainingID

FROM TrainingAssignment A

INNER JOIN
TrainingDetails T

ON A.TrainingID=
T.TrainingID

WHERE
A.EmpID=@EmpID

", con);


                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@EmpID",
                emp);

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
                "--Select--", ""));
            }
        }



        void CreateTable()
        {
            DataTable dt =
            new DataTable();

            dt.Columns.Add(
            "Topic");

            dt.Columns.Add(
            "Report");

            dt.Rows.Add();

            ViewState["tbl"] =
            dt;

            gvTopic.DataSource =
            dt;

            gvTopic.DataBind();
        }



        void SaveRows()
        {
            DataTable dt =
            ViewState["tbl"]
            as DataTable;

            dt.Rows.Clear();

            foreach (
            GridViewRow r
            in gvTopic.Rows)
            {
                TextBox topic =
                (TextBox)
                r.FindControl(
                "txtTopic");

                TextBox report =
                (TextBox)
                r.FindControl(
                "txtReport");


                dt.Rows.Add(
                topic.Text,
                report.Text);
            }

            ViewState["tbl"] =
            dt;
        }



        protected void btnAdd_Click(
        object sender,
        EventArgs e)
        {
            SaveRows();

            DataTable dt =
            ViewState["tbl"]
            as DataTable;

            dt.Rows.Add();

            gvTopic.DataSource =
            dt;

            gvTopic.DataBind();
        }

        void BindSubmittedGrid()
        {
            if (
            ddlTraining.SelectedIndex <= 0)
            {
                gvSubmitted.DataSource = null;

                gvSubmitted.DataBind();

                return;
            }

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT

Topic,
Report,
CreatedOn

FROM
FeedbackReport

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


                gvSubmitted.DataSource =
                dt;

                gvSubmitted.DataBind();
            }
        }

        protected void
ddlTraining_SelectedIndexChanged(
object sender,
EventArgs e)
        {
            BindSubmittedGrid();
        }

        protected void gvTopic_RowDeleting(
        object sender,
        GridViewDeleteEventArgs e)
        {
            SaveRows();

            DataTable dt =
            ViewState["tbl"]
            as DataTable;

            dt.Rows[e.RowIndex]
            .Delete();

            gvTopic.DataSource =
            dt;

            gvTopic.DataBind();
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

FROM FeedbackReport

", con);

                int no =
                Convert.ToInt32(
                cmd.ExecuteScalar());

                return
                "FBR" +
                no.ToString(
                "0000");
            }
        }



        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            SaveRows();

            DataTable dt =
            ViewState["tbl"]
            as DataTable;

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                foreach (
                DataRow r
                in dt.Rows)
                {
                    if (
                    r["Topic"]
                    .ToString() == "")
                        continue;


                    SqlCommand cmd =
                    new SqlCommand(@"

INSERT INTO
FeedbackReport
(
FeedbackReportID,
EmpID,
TrainingID,
Topic,
Report,
CreatedOn,
CreatedBy
)

VALUES
(
@ID,
@Emp,
@Training,
@Topic,
@Report,
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
                    Session["UserId"].ToString());

                    cmd.Parameters
                    .AddWithValue(
                    "@Training",
                    ddlTraining
                    .SelectedValue);

                    cmd.Parameters
                    .AddWithValue(
                    "@Topic",
                    r["Topic"]);

                    cmd.Parameters
                    .AddWithValue(
                    "@Report",
                    r["Report"]);

                    cmd.Parameters
                    .AddWithValue(
                    "@By",
                    Session["UserId"].ToString());

                    cmd.ExecuteNonQuery();
                }
            }


            BindSubmittedGrid();

            CreateTable();

            lblMsg.Text =
            "Saved Successfully";
        }
    }
}