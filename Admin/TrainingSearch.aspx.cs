using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingSearch : System.Web.UI.Page
    {
        string constr =
        ConfigurationManager
        .ConnectionStrings["constr"]
        .ConnectionString;
        bool showAll = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //if (
                //   Session["InternalRedirect_Admin"] == null)
                //{
                //    Response.Redirect(
                //    "~/Default.aspx");
                //}
                BindFilters();

                gvTraining.DataSource = null;
                gvTraining.DataBind();
            }
            //if (!IsPostBack)
            //{
            //    BindFilters();
            //    BindGrid();
            //}
        }


        private void BindFilters()
        {
            BindCheckBox(
            @"SELECT DISTINCT EmpDesignation
              FROM EmpBasicMaster
              WHERE EmpDesignation IS NOT NULL
              ORDER BY EmpDesignation",
            chkDesignation);


            BindCheckBox(
            @"SELECT DISTINCT EmpCompany
              FROM EmpBasicMaster
              WHERE EmpCompany IS NOT NULL
              ORDER BY EmpCompany",
            chkCompany);


            BindCheckBox(
            @"SELECT DISTINCT EmpPostingPlace
              FROM EmpBasicMaster
              WHERE EmpPostingPlace IS NOT NULL
              ORDER BY EmpPostingPlace",
            chkPostingPlace);
        }



        private void BindCheckBox(
        string query,
        CheckBoxList chk)
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

                chk.DataSource = dt;

                chk.DataTextField =
                dt.Columns[0].ColumnName;

                chk.DataValueField =
                dt.Columns[0].ColumnName;

                chk.DataBind();
            }
        }



        protected void btnSearch_Click(
        object sender,
        EventArgs e)
        {
            BindGrid();
        }



        protected void btnReset_Click(
        object sender,
        EventArgs e)
        {
            txtEmpID.Text = "";
            txtEmpName.Text = "";
            txtMobile.Text = "";
            txtEmail.Text = "";

            foreach (ListItem x in chkDesignation.Items)
                x.Selected = false;

            foreach (ListItem x in chkCompany.Items)
                x.Selected = false;

            foreach (ListItem x in chkPostingPlace.Items)
                x.Selected = false;

            BindGrid();
        }
        protected void btnShowAll_Click(
object sender,
EventArgs e)
        {
            showAll = true;

            BindGrid();

            showAll = false;
        }


        private void AddMultiSelectFilter(
        StringBuilder query,
        SqlCommand cmd,
        CheckBoxList chk,
        string dbField,
        string prefix)
        {
            var selected =
            chk.Items
            .Cast<ListItem>()
            .Where(x => x.Selected)
            .ToList();


            if (selected.Count > 0)
            {
                query.Append(
                " AND " +
                dbField +
                " IN(");

                for (int i = 0;
                i < selected.Count;
                i++)
                {
                    string p =
                    "@" +
                    prefix + i;

                    query.Append(p);

                    if (i <
                    selected.Count - 1)
                    {
                        query.Append(",");
                    }

                    cmd.Parameters
                    .AddWithValue(
                    p,
                    selected[i].Value);
                }

                query.Append(")");
            }
        }



        private void BindGrid()
        {
            if (
!showAll
&& txtEmpID.Text.Trim() == ""
&& txtEmpName.Text.Trim() == ""
&& txtMobile.Text.Trim() == ""
&& txtEmail.Text.Trim() == ""
&& !chkDesignation.Items.Cast<ListItem>().Any(x => x.Selected)
&& !chkCompany.Items.Cast<ListItem>().Any(x => x.Selected)
&& !chkPostingPlace.Items.Cast<ListItem>().Any(x => x.Selected)
)
            {
                gvTraining.DataSource = null;

                gvTraining.DataBind();

                return;
            }
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                StringBuilder query =
                new StringBuilder();


                query.Append(@"

                SELECT

                E.EmpID,
                E.EmpName,
                E.MobileNo,
                E.EmailId,
                E.EmpCompany,
                E.EmpDesignation,
                E.EmpPostingPlace,

                TD.TrainingID,
                TD.TrainingType,
                TD.TrainingOrganizer,
                TD.Batch,

                ISNULL(
                TA.TrainingAttended,
                'Pending')
                Attendance,

                CONVERT(
                varchar(10),
                TD.DateFrom,
                103)
                AS DateFrom,

                CONVERT(
                varchar(10),
                TD.DateTo,
                103)
                AS DateTo,

                TD.TrainingLocation
                AS LocationOfInduction

                FROM EmpBasicMaster E

                LEFT JOIN
                TrainingAssignment TA

                ON E.EmpID=
                TA.EmpID

                LEFT JOIN
                TrainingDetails TD

                ON TA.TrainingID=
                TD.TrainingID

                WHERE 1=1
                ");


                SqlCommand cmd =
                new SqlCommand();

                cmd.Connection =
                con;


                if (!string
                .IsNullOrWhiteSpace(
                txtEmpID.Text))
                {
                    query.Append(
                    " AND E.EmpID LIKE @EmpID");

                    cmd.Parameters
                    .AddWithValue(
                    "@EmpID",

                    "%" +
                    txtEmpID.Text.Trim()
                    + "%");
                }



                if (!string
                .IsNullOrWhiteSpace(
                txtEmpName.Text))
                {
                    query.Append(
                    " AND E.EmpName LIKE @EmpName");

                    cmd.Parameters
                    .AddWithValue(
                    "@EmpName",

                    "%" +
                    txtEmpName.Text.Trim()
                    + "%");
                }



                if (!string
                .IsNullOrWhiteSpace(
                txtMobile.Text))
                {
                    query.Append(
                    " AND E.MobileNo LIKE @Mobile");

                    cmd.Parameters
                    .AddWithValue(
                    "@Mobile",

                    "%" +
                    txtMobile.Text.Trim()
                    + "%");
                }



                if (!string
                .IsNullOrWhiteSpace(
                txtEmail.Text))
                {
                    query.Append(
                    " AND E.EmailId LIKE @Email");

                    cmd.Parameters
                    .AddWithValue(
                    "@Email",

                    "%" +
                    txtEmail.Text.Trim()
                    + "%");
                }



                AddMultiSelectFilter(
                query,
                cmd,
                chkDesignation,
                "E.EmpDesignation",
                "des");


                AddMultiSelectFilter(
                query,
                cmd,
                chkCompany,
                "E.EmpCompany",
                "comp");


                AddMultiSelectFilter(
                query,
                cmd,
                chkPostingPlace,
                "E.EmpPostingPlace",
                "post");



                query.Append(
                " ORDER BY E.EmpID");


                cmd.CommandText =
                query.ToString();

                SqlDataAdapter da =
                new SqlDataAdapter(
                cmd);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvTraining.DataSource =
                dt;

                gvTraining.DataBind();
            }
        }



        protected void lnkTopicFeedback_Click(
        object sender,
        EventArgs e)
        {
            LinkButton btn =
            (LinkButton)sender;

            string[] arr =
            btn.CommandArgument
            .Split('|');


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

                SELECT

                EmpID,
                Topic,
                Report

                FROM FeedbackReport

                WHERE
                EmpID=@EmpID

                AND
                TrainingID=@TrainingID",

                con);


                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@EmpID",
                arr[0]);

                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@TrainingID",
                arr[1]);


                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvFeedback.DataSource =
                dt;

                gvFeedback.DataBind();

                Session["FeedbackData"] =
                dt;
            }


            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "fb1",

            "var m=bootstrap.Modal.getOrCreateInstance(document.getElementById('feedbackModal'));m.show();",

            true);
        }




        protected void lnkTrainingFeedback_Click(
        object sender,
        EventArgs e)
        {
            LinkButton btn =
            (LinkButton)sender;

            string[] arr =
            btn.CommandArgument
            .Split('|');


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

                SELECT

                EmpID,

                TrainingRelatedAspects,

                OrganizedBy,

                Remarks,

                Grading

                FROM
                FeedbackTrainingRelated

                WHERE
                EmpID=@EmpID

                AND
                TrainingID=@TrainingID",

                con);


                da.SelectCommand.Parameters
                .AddWithValue(
                "@EmpID",
                arr[0]);

                da.SelectCommand.Parameters
                .AddWithValue(
                "@TrainingID",
                arr[1]);


                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvFeedback.DataSource =
                dt;

                gvFeedback.DataBind();

                Session["FeedbackData"] =
                dt;
            }


            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "fb2",

            "var m=bootstrap.Modal.getOrCreateInstance(document.getElementById('feedbackModal'));m.show();",

            true);
        }




        protected void btnFeedbackExport_Click(
        object sender,
        EventArgs e)
        {
            DataTable dt =
            Session["FeedbackData"]
            as DataTable;

            if (dt == null)
                return;

            gvFeedback.DataSource =
            dt;

            gvFeedback.DataBind();

            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",
            "attachment;filename=Feedback.xls");

            Response.ContentType =
            "application/ms-excel";

            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);

            gvFeedback.RenderControl(hw);

            Response.Write(
            sw.ToString());

            Response.End();
        }

        protected void lnkOverallResponse_Click(
       object sender,
       EventArgs e)
        {
            LinkButton btn =
            (LinkButton)sender;

            string[] arr =
            btn.CommandArgument
            .Split('|');


            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

                SELECT

                EmpID,

                TrainingID,
      OverallResponse

                FROM
                FeedbackOverall

                WHERE
                EmpID=@EmpID

                AND
                TrainingID=@TrainingID",

                con);


                da.SelectCommand.Parameters
                .AddWithValue(
                "@EmpID",
                arr[0]);

                da.SelectCommand.Parameters
                .AddWithValue(
                "@TrainingID",
                arr[1]);


                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvOverallResponse.DataSource =
                dt;

                gvOverallResponse.DataBind();

                Session["OverallResponse"] =
                dt;
            }


            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            "fb2",

            "var m=bootstrap.Modal.getOrCreateInstance(document.getElementById('overallResponseModal'));m.show();",

            true);
        }


        protected void btnOverallResponse_Click(
        object sender,
        EventArgs e)
        {
            DataTable dt =
            Session["OverallResponse"]
            as DataTable;

            if (dt == null)
                return;

            gvOverallResponse.DataSource =
            dt;

            gvOverallResponse.DataBind();

            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",
            "attachment;filename=OverallResponse.xls");

            Response.ContentType =
            "application/ms-excel";

            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);

            gvOverallResponse.RenderControl(hw);

            Response.Write(
            sw.ToString());

            Response.End();
        }

        public override void VerifyRenderingInServerForm(
        Control control)
        {

        }
    }
}