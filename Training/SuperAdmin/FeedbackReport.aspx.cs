using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using OfficeOpenXml;

namespace Training.SuperAdmin
{
    public partial class FeedbackReport : System.Web.UI.Page
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
                   Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                BindTraining();
            }
        }



        void BindTraining()
        {
            try
            {
                using (SqlConnection con =
                new SqlConnection(constr))
                {
                    SqlDataAdapter da =
                    new SqlDataAdapter(@"

SELECT
TrainingID,
TrainingID + ' - ' +
ISNULL(TrainingType,'')
AS TName

FROM TrainingDetails
ORDER BY ID DESC

", con);

                    DataTable dt =
                    new DataTable();

                    da.Fill(dt);

                    ddlTrainingID.DataSource = dt;
                    ddlTrainingID.DataTextField = "TName";
                    ddlTrainingID.DataValueField = "TrainingID";
                    ddlTrainingID.DataBind();

                    ddlTrainingID.Items.Insert(
                    0,
                    new ListItem(
                    "--Select Training--",
                    ""));
                }
            }
            catch (Exception ex)
            {
                lblSingleMessage.Text =
                ex.Message;
            }
        }



        void BindEmp()
        {
            try
            {
                ddlEmpID.Items.Clear();

                if (ddlTrainingID.SelectedIndex <= 0)
                    return;


                using (SqlConnection con =
                new SqlConnection(constr))
                {
                    SqlDataAdapter da =
                    new SqlDataAdapter(@"

SELECT DISTINCT
EmpID

FROM TrainingAssignment

WHERE TrainingID=@TrainingID

ORDER BY EmpID

", con);


                    da.SelectCommand
                    .Parameters
                    .AddWithValue(
                    "@TrainingID",
                    ddlTrainingID.SelectedValue);


                    DataTable dt =
                    new DataTable();

                    da.Fill(dt);

                    ddlEmpID.DataSource = dt;
                    ddlEmpID.DataTextField = "EmpID";
                    ddlEmpID.DataValueField = "EmpID";
                    ddlEmpID.DataBind();

                    ddlEmpID.Items.Insert(
                    0,
                    new ListItem(
                    "--Select Employee--",
                    ""));
                }

            }
            catch (Exception ex)
            {
                lblSingleMessage.Text =
                ex.Message;
            }
        }



        protected void
        ddlTrainingID_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindEmp();
        }



        private int GetLastFeedbackNo()
        {
            int no = 0;

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

SELECT TOP 1
FeedbackReportID

FROM FeedbackReport

ORDER BY ID DESC

", con);


                object obj =
                cmd.ExecuteScalar();

                if (obj != null)
                {
                    string lastID =
                    obj.ToString();

                    if (lastID.StartsWith("FBR"))
                    {
                        int.TryParse(
                        lastID.Replace(
                        "FBR", ""),
                        out no);
                    }
                }

                con.Close();
            }

            return no;
        }



        private string GenerateFeedbackID()
        {
            int no =
            GetLastFeedbackNo() + 1;

            return
            "FBR" +
            no.ToString("0000");
        }




        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            try
            {
                if (
                ddlTrainingID.SelectedIndex <= 0)
                {
                    lblSingleMessage.Text =
                    "Select Training";

                    return;
                }

                if (
                ddlEmpID.SelectedIndex <= 0)
                {
                    lblSingleMessage.Text =
                    "Select Employee";

                    return;
                }


                string createdBy =
                Session["EmpID"] != null
                ?
                Session["EmpID"]
                .ToString()
                :
                "Admin";


                string feedbackID =
                GenerateFeedbackID();


                using (
                SqlConnection con =
                new SqlConnection(
                constr))
                {

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
@FeedbackReportID,
@EmpID,
@TrainingID,
@Topic,
@Report,
GETDATE(),
@CreatedBy
)

", con);


                    cmd.Parameters
                    .AddWithValue(
                    "@FeedbackReportID",
                    feedbackID);

                    cmd.Parameters
                    .AddWithValue(
                    "@EmpID",
                    ddlEmpID.SelectedValue);

                    cmd.Parameters
                    .AddWithValue(
                    "@TrainingID",
                    ddlTrainingID.SelectedValue);

                    cmd.Parameters
                    .AddWithValue(
                    "@Topic",
                    txtTopic.Text.Trim());

                    cmd.Parameters
                    .AddWithValue(
                    "@Report",
                    txtReport.Text.Trim());

                    cmd.Parameters
                    .AddWithValue(
                    "@CreatedBy",
                    createdBy);


                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }


                lblSingleMessage.ForeColor =
                System.Drawing.Color.Green;

                lblSingleMessage.Text =
                "Saved Successfully";


                txtTopic.Text = "";
                txtReport.Text = "";
            }

            catch (Exception ex)
            {
                lblSingleMessage.ForeColor =
                System.Drawing.Color.Red;

                lblSingleMessage.Text =
                ex.ToString();
            }
        }




        protected void btnUpload_Click(
        object sender,
        EventArgs e)
        {
            try
            {
                if (!fuExcel.HasFile)
                {
                    lblBulkMessage.Text =
                    "Please select Excel";

                    return;
                }


                string createdBy =
                Session["EmpID"] != null
                ?
                Session["EmpID"]
                .ToString()
                :
                "Admin";


                ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;


                using (
                ExcelPackage package =
                new ExcelPackage(
                fuExcel.FileContent))
                {

                    ExcelWorksheet ws =
                    package.Workbook
                    .Worksheets[0];


                    if (
                    ws == null ||
                    ws.Dimension == null)
                    {
                        lblBulkMessage.Text =
                        "Excel empty";

                        return;
                    }


                    int rowCount =
                    ws.Dimension.Rows;


                    int lastNo =
                    GetLastFeedbackNo();


                    using (
                    SqlConnection con =
                    new SqlConnection(
                    constr))
                    {

                        con.Open();

                        for (
                        int row = 2;
                        row <= rowCount;
                        row++)
                        {
                            lastNo++;

                            string feedbackID =
                            "FBR" +
                            lastNo.ToString(
                            "0000");


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
@FeedbackReportID,
@EmpID,
@TrainingID,
@Topic,
@Report,
GETDATE(),
@CreatedBy
)

", con);


                            cmd.Parameters
                            .AddWithValue(
                            "@FeedbackReportID",
                            feedbackID);

                            cmd.Parameters
                            .AddWithValue(
                            "@EmpID",
                            ws.Cells[row, 1]
                            .Text.Trim());

                            cmd.Parameters
                            .AddWithValue(
                            "@TrainingID",
                            ws.Cells[row, 2]
                            .Text.Trim());

                            cmd.Parameters
                            .AddWithValue(
                            "@Topic",
                            ws.Cells[row, 3]
                            .Text.Trim());

                            cmd.Parameters
                            .AddWithValue(
                            "@Report",
                            ws.Cells[row, 4]
                            .Text.Trim());

                            cmd.Parameters
                            .AddWithValue(
                            "@CreatedBy",
                            createdBy);


                            cmd.ExecuteNonQuery();
                        }

                        con.Close();
                    }
                }

                lblBulkMessage.ForeColor =
                System.Drawing.Color.Green;

                lblBulkMessage.Text =
                "Excel Uploaded Successfully";
            }

            catch (Exception ex)
            {
                lblBulkMessage.ForeColor =
                System.Drawing.Color.Red;

                lblBulkMessage.Text =
                ex.ToString();
            }
        }

    }
}