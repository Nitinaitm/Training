using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class CreateTraining : System.Web.UI.Page
    {
        string constr =
        ConfigurationManager
        .ConnectionStrings["constr"]
        .ConnectionString;


        protected void Page_Load(
        object sender,
        EventArgs e)
        {
            UnobtrusiveValidationMode =
            UnobtrusiveValidationMode.None;

            if (!IsPostBack)
            {
                if (
                Session["InternalRedirect_SuperAdmin"]
                == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }

                BindTrainingType();

                BindDesignation();

                ddlTrainingOrganizer.Items.Insert(
                0,
                new ListItem(
                "Select Organizer",
                ""));

                ddlTrainingLocation.Items.Insert(
                0,
                new ListItem(
                "Select Location",
                ""));

                LoadPlugins();
            }
        }



        private void GenerateTrainingID()
        {
            try
            {
                string trainingType =
                ddlTrainingType
                .SelectedItem
                .Text
                .Trim()
                .ToUpper();

                trainingType =
                trainingType.Length >= 2
                ?
                trainingType.Substring(0, 2)
                :
                trainingType;



                string organizer =
                ddlTrainingOrganizer
                .SelectedItem
                .Text
                .Replace(" ", "")
                .ToUpper();



                string location =
                ddlTrainingLocation
                .SelectedItem
                .Text
                .Replace(" ", "")
                .ToUpper();

                location =
                location.Length >= 3
                ?
                location.Substring(0, 3)
                :
                location;



                string batch =
                txtBatch.Text
                .Trim()
                .Replace(" ", "")
                .ToUpper();



                DateTime fromDate =
                DateTime.ParseExact(
                txtDateFrom.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);


                DateTime toDate =
                DateTime.ParseExact(
                txtDateTo.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);



                string fromPart =
                fromDate.ToString(
                "ddMMyy");

                string toPart =
                toDate.ToString(
                "ddMMyy");



                StringBuilder desig =
                new StringBuilder();


                //foreach (
                //ListItem item
                //in lstDesignation.Items)
                //{
                //    if (item.Selected)
                //    {
                //        string[] words =
                //        item.Text.Split(' ');

                //        foreach (
                //        string w
                //        in words)
                //        {
                //            if (
                //            !string.IsNullOrWhiteSpace(w))
                //            {
                //                desig.Append(
                //                w.Substring(0, 1)
                //                .ToUpper());
                //            }
                //        }
                //    }
                //}

                foreach (
ListItem item
in lstDesignation.Items)
{
    if (item.Selected)
    {
        string[] words =
        item.Text.Split(' ');

        foreach (string w in words)
        {
            if (!string.IsNullOrWhiteSpace(w))
            {
                char firstValidChar = '\0';

                foreach (char ch in w)
                {
                    if (char.IsLetterOrDigit(ch))
                    {
                        firstValidChar = char.ToUpper(ch);
                        break;
                    }
                }

                if (firstValidChar != '\0')
                {
                    desig.Append(firstValidChar);
                }
            }
        }
    }
}

                string prefix =

                "TR"

                + "-"

                + trainingType

                + "-"

                + organizer

                + "-"

                + location

                + "-"

                + batch

                + "-"

                + fromPart

                + "-"

                + toPart

                + "-"

                + desig.ToString();



                using (
                SqlConnection con =
                new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand cmd =
                    new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingDetails

WHERE TrainingID
LIKE @Prefix+'%'

", con);


                    cmd.Parameters
                    .AddWithValue(
                    "@Prefix",
                    prefix);


                    int count =
                    Convert.ToInt32(
                    cmd.ExecuteScalar());


                    txtTrainingID.Text =

                    prefix

                    + "-"

                    +

                    (count + 1)
                    .ToString("000");
                }

            }

            catch
            {
            }
        }




        private string GenerateTrainingDesignationID()
        {
            string id = "";

            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT TOP 1
TrainingDesignationID

FROM TrainingDesignation

ORDER BY ID DESC";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);

                con.Open();

                object result =
                cmd.ExecuteScalar();

                con.Close();

                int next = 1;

                if (result != null)
                {
                    string last =
                    result.ToString();

                    next =
                    Convert.ToInt32(
                    last.Replace(
                    "TRDES", "")) + 1;
                }

                id =
                "TRDES"
                +
                next.ToString(
                "0000");
            }

            return id;
        }





        private void BindTrainingType()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingTypeID,
TrainingType

FROM TrainingMaster

ORDER BY TrainingType";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);

                con.Open();

                ddlTrainingType
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingType
                .DataTextField =
                "TrainingType";

                ddlTrainingType
                .DataValueField =
                "TrainingTypeID";

                ddlTrainingType
                .DataBind();

                con.Close();

                ddlTrainingType.Items.Insert(
                0,
                new ListItem(
                "Select Training Type",
                ""));
            }
        }




        protected void
        ddlTrainingType_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindOrganizer();

            LoadPlugins();
        }




        private void BindOrganizer()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingOrganizerID,
TrainingOrganizer

FROM
TrainingOrganizerMaster

WHERE
TrainingTypeID=
@TrainingTypeID";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);

                cmd.Parameters
                .AddWithValue(
                "@TrainingTypeID",
                ddlTrainingType.SelectedValue);

                con.Open();

                ddlTrainingOrganizer
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingOrganizer
                .DataTextField =
                "TrainingOrganizer";

                ddlTrainingOrganizer
                .DataValueField =
                "TrainingOrganizerID";

                ddlTrainingOrganizer
                .DataBind();

                con.Close();

                ddlTrainingOrganizer.Items.Insert(
                0,
                new ListItem(
                "Select Organizer",
                ""));
            }
        }




        protected void
        ddlTrainingOrganizer_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindLocation();

            LoadPlugins();
        }




        private void BindLocation()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT
TrainingLocationID,
TrainingLocation

FROM
TrainingLocationMaster

WHERE
TrainingTypeID=
@TrainingTypeID

AND

TrainingOrganizerID=
@TrainingOrganizerID";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);


                cmd.Parameters.AddWithValue(
                "@TrainingTypeID",
                ddlTrainingType.SelectedValue);


                cmd.Parameters.AddWithValue(
                "@TrainingOrganizerID",
                ddlTrainingOrganizer.SelectedValue);


                con.Open();

                ddlTrainingLocation
                .DataSource =
                cmd.ExecuteReader();

                ddlTrainingLocation
                .DataTextField =
                "TrainingLocation";

                ddlTrainingLocation
                .DataValueField =
                "TrainingLocation";

                ddlTrainingLocation
                .DataBind();

                con.Close();

                ddlTrainingLocation.Items.Insert(
                0,
                new ListItem(
                "Select Location",
                ""));
            }
        }



        private void BindDesignation()
        {
            using (
            SqlConnection con =
            new SqlConnection(constr))
            {
                string query = @"

SELECT DISTINCT
EmpDesignation

FROM EmpBasicMaster

WHERE
ISNULL(
EmpDesignation,'')<>''

ORDER BY
EmpDesignation";


                SqlCommand cmd =
                new SqlCommand(
                query,
                con);

                con.Open();

                lstDesignation
                .DataSource =
                cmd.ExecuteReader();

                lstDesignation
                .DataTextField =
                "EmpDesignation";

                lstDesignation
                .DataValueField =
                "EmpDesignation";

                lstDesignation
                .DataBind();

                con.Close();
            }
        }



        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            lblMessage.Text = "";

            try
            {
                GenerateTrainingID();

                if (!Page.IsValid)
                    return;

                bool selected = false;

                foreach (
                ListItem item
                in lstDesignation.Items)
                {
                    if (item.Selected)
                    {
                        selected = true;
                        break;
                    }
                }

                if (!selected)
                {
                    lblMessage.Text =
                    "Select at least one designation";

                    lblMessage.ForeColor =
                    Color.Red;

                    return;
                }


                DateTime fromDate =
                DateTime.ParseExact(
                txtDateFrom.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

                DateTime toDate =
                DateTime.ParseExact(
                txtDateTo.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);


                using (
                SqlConnection con =
                new SqlConnection(constr))
                {
                    con.Open();

                    SqlCommand cmd =
                    new SqlCommand(@"

INSERT INTO
TrainingDetails
(
TrainingID,
TrainingType,
TrainingOrganizer,
TrainingLocation,
Batch,
DateFrom,
DateTo,
CreatedOn,
CreatedBy
)

VALUES
(
@TrainingID,
@TrainingType,
@TrainingOrganizer,
@TrainingLocation,
@Batch,
@DateFrom,
@DateTo,
GETDATE(),
@CreatedBy
)

", con);


                    cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    txtTrainingID.Text);

                    cmd.Parameters.AddWithValue(
                    "@TrainingType",
                    ddlTrainingType.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@TrainingOrganizer",
                    ddlTrainingOrganizer.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@TrainingLocation",
                    ddlTrainingLocation.SelectedItem.Text);

                    cmd.Parameters.AddWithValue(
                    "@Batch",
                    txtBatch.Text.Trim());

                    cmd.Parameters.AddWithValue(
                    "@DateFrom",
                    fromDate.ToString(
                    "dd-MM-yyyy"));

                    cmd.Parameters.AddWithValue(
                    "@DateTo",
                    toDate.ToString(
                    "dd-MM-yyyy"));

                    cmd.Parameters.AddWithValue(
                    "@CreatedBy",
                    "Admin");


                    cmd.ExecuteNonQuery();


                    foreach (
                    ListItem item
                    in lstDesignation.Items)
                    {
                        if (item.Selected)
                        {
                            SqlCommand d =
                            new SqlCommand(@"

INSERT INTO
TrainingDesignation
(
TrainingDesignationID,
TrainingID,
EmpDesignation,
CreatedOn,
CreatedBy
)

VALUES
(
@TrainingDesignationID,
@TrainingID,
@EmpDesignation,
GETDATE(),
@CreatedBy
)

", con);


                            d.Parameters.AddWithValue(
                            "@TrainingDesignationID",
                            GenerateTrainingDesignationID());

                            d.Parameters.AddWithValue(
                            "@TrainingID",
                            txtTrainingID.Text);

                            d.Parameters.AddWithValue(
                            "@EmpDesignation",
                            item.Value);

                            d.Parameters.AddWithValue(
                            "@CreatedBy",
                            "Admin");

                            d.ExecuteNonQuery();
                        }
                    }
                }


                lblMessage.Text =
                "Training created successfully";

                lblMessage.ForeColor =
                Color.Green;

            }
            catch (Exception ex)
            {
                lblMessage.Text =
                ex.Message;

                lblMessage.ForeColor =
                Color.Red;
            }
        }



        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(
            this,
            GetType(),
            Guid.NewGuid().ToString(),

            "$('#ddlTrainingType').select2({width:'100%'});" +
            "$('#ddlTrainingOrganizer').select2({width:'100%'});" +
            "$('#ddlTrainingLocation').select2({width:'100%'});" +
            "$('#lstDesignation').select2({placeholder:'Select Designation',width:'100%'});",

            true);
        }
    }
}