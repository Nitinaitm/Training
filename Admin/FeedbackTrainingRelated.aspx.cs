using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using OfficeOpenXml;

namespace Training.Admin
{
    public partial class FeedbackTrainingRelated :
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
                //if (
                //   Session["InternalRedirect_Admin"] == null)
                //{
                //    Response.Redirect(
                //    "~/Default.aspx");
                //}
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

SELECT
TrainingID,
TrainingID+' - '+
ISNULL(TrainingType,'')
AS TName

FROM TrainingDetails
ORDER BY ID DESC

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlTrainingID.DataSource = dt;

                ddlTrainingID.DataTextField =
                "TName";

                ddlTrainingID.DataValueField =
                "TrainingID";

                ddlTrainingID.DataBind();

                ddlTrainingID.Items.Insert(
                0,
                new ListItem(
                "--Select Training--",
                ""));
            }
        }




        void BindEmp()
        {
            ddlEmpID.Items.Clear();

            if (
            ddlTrainingID.SelectedIndex <= 0)
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


                ddlEmpID.DataSource =
                dt;

                ddlEmpID.DataTextField =
                "EmpID";

                ddlEmpID.DataValueField =
                "EmpID";

                ddlEmpID.DataBind();

                ddlEmpID.Items.Insert(
                0,
                new ListItem(
                "--Select Employee--",
                ""));
            }
        }




        void BindAspect()
        {
            ddlAspect.Items.Clear();

            if (
            ddlTrainingID.SelectedIndex <= 0
            ||
            ddlEmpID.SelectedIndex <= 0)
                return;


            DataTable dt =
            new DataTable();

            dt.Columns.Add(
            "Aspect");


            dt.Rows.Add(
            "Travel Arrangements");

            dt.Rows.Add(
            "Accommodation");

            dt.Rows.Add(
            "Food Arrangements");



            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT
TrainingRelatedAspects

FROM
FeedbackTrainingRelated

WHERE
TrainingID=@TID

AND EmpID=@EmpID

", con);


                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@TID",
                ddlTrainingID.SelectedValue);


                da.SelectCommand
                .Parameters
                .AddWithValue(
                "@EmpID",
                ddlEmpID.SelectedValue);


                DataTable used =
                new DataTable();

                da.Fill(used);


                foreach (
                DataRow r
                in used.Rows)
                {
                    string val =
                    r["TrainingRelatedAspects"]
                    .ToString();

                    DataRow[] rows =
                    dt.Select(
                    "Aspect='" +
                    val.Replace("'", "''")
                    + "'");

                    foreach (
                    DataRow x
                    in rows)
                    {
                        dt.Rows.Remove(x);
                    }
                }

                dt.AcceptChanges();


                ddlAspect.DataSource =
                dt;

                ddlAspect.DataTextField =
                "Aspect";

                ddlAspect.DataValueField =
                "Aspect";

                ddlAspect.DataBind();


                if (dt.Rows.Count == 0)
                {
                    ddlAspect.Items.Insert(
                    0,
                    new ListItem(
                    "All aspects submitted",
                    ""));
                }
                else
                {
                    ddlAspect.Items.Insert(
                    0,
                    new ListItem(
                    "--Select Aspect--",
                    ""));
                }
            }
        }



        protected void
        ddlTrainingID_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            BindEmp();

            ddlAspect.Items.Clear();
        }



        protected void
 ddlEmpID_SelectedIndexChanged(
 object sender,
 EventArgs e)
        {
            BindAspect();

            CheckOverallSubmitted();
        }

        void CheckOverallSubmitted()
        {
            txtOverall.Enabled = true;
            btnOverall.Enabled = true;

            if (
            ddlEmpID.SelectedIndex <= 0 ||
            ddlTrainingID.SelectedIndex <= 0)
                return;

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
TrainingID=@Tid

", con);

                cmd.Parameters
                .AddWithValue(
                "@Emp",
                ddlEmpID.SelectedValue);

                cmd.Parameters
                .AddWithValue(
                "@Tid",
                ddlTrainingID.SelectedValue);


                int cnt =
                Convert.ToInt32(
                cmd.ExecuteScalar());

                if (cnt > 0)
                {
                    txtOverall.Enabled = false;

                    btnOverall.Enabled = false;

                    lblOverall.Text =
                    "Overall response already submitted";
                }
                else
                {
                    lblOverall.Text = "";
                }
            }
        }

        protected void btnOverall_Click(
object sender,
EventArgs e)
        {
            try
            {
                if (
                ddlTrainingID.SelectedIndex <= 0)
                {
                    lblOverall.Text =
                    "Select training";

                    return;
                }


                if (
                ddlEmpID.SelectedIndex <= 0)
                {
                    lblOverall.Text =
                    "Select employee";

                    return;
                }


                if (
                txtOverall.Text.Trim() == "")
                {
                    lblOverall.Text =
                    "Enter overall response";

                    return;
                }


                string createdBy =
                Session["EmpID"] != null
                ?
                Session["EmpID"]
                .ToString()
                :
                "Admin";


                using (SqlConnection con =
                new SqlConnection(constr))
                {
                    con.Open();


                    SqlCommand chk =
                    new SqlCommand(@"

SELECT COUNT(*)

FROM FeedbackOverall

WHERE
EmpID=@Emp
AND
TrainingID=@Tid

", con);

                    chk.Parameters
                    .AddWithValue(
                    "@Emp",
                    ddlEmpID.SelectedValue);

                    chk.Parameters
                    .AddWithValue(
                    "@Tid",
                    ddlTrainingID.SelectedValue);


                    int cnt =
                    Convert.ToInt32(
                    chk.ExecuteScalar());

                    if (cnt > 0)
                    {
                        lblOverall.Text =
                        "Already submitted";

                        return;
                    }



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
@Tid,
@Response,
GETDATE(),
@CreatedBy
)

", con);


                    cmd.Parameters
                    .AddWithValue(
                    "@ID",
                    GenerateOverallID());

                    cmd.Parameters
                    .AddWithValue(
                    "@Emp",
                    ddlEmpID.SelectedValue);

                    cmd.Parameters
                    .AddWithValue(
                    "@Tid",
                    ddlTrainingID.SelectedValue);

                    cmd.Parameters
                    .AddWithValue(
                    "@Response",
                    txtOverall.Text.Trim());

                    cmd.Parameters
                    .AddWithValue(
                    "@CreatedBy",
                    createdBy);


                    cmd.ExecuteNonQuery();


                    txtOverall.Enabled = false;

                    btnOverall.Enabled = false;

                    lblOverall.Text =
                    "Saved Successfully";
                }

            }

            catch (Exception ex)
            {
                lblOverall.Text =
                ex.ToString();
            }
        }

        private int GetLastNo()
        {
            int no = 0;

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

SELECT TOP 1
FeedbackTrainingRelated

FROM FeedbackTrainingRelated

ORDER BY ID DESC

", con);


                object obj =
                cmd.ExecuteScalar();

                if (obj != null)
                {
                    string lastID =
                    obj.ToString();

                    if (
                    lastID.StartsWith("FTR"))
                    {
                        int.TryParse(
                        lastID.Replace(
                        "FTR", ""),
                        out no);
                    }
                }
            }

            return no;
        }




        private string GenerateID()
        {
            return
            "FTR" +
            (GetLastNo() + 1)
            .ToString("0000");
        }




        protected void btnSave_Click(
        object sender,
        EventArgs e)
        {
            try
            {
                if (
                ddlAspect.SelectedIndex <= 0)
                {
                    lblSingleMessage.Text =
                    "Select Aspect";

                    return;
                }


                string createdBy =
                Session["EmpID"] != null
                ?
                Session["EmpID"].ToString()
                :
                "Admin";


                string ID =
                GenerateID();


                using (SqlConnection con =
                new SqlConnection(constr))
                {
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
@EmpID,
@TrainingID,
@Aspect,
@Org,
@Remarks,
@Grade,
GETDATE(),
@CreatedBy
)

", con);


                    cmd.Parameters.AddWithValue("@ID", ID);

                    cmd.Parameters.AddWithValue(
                    "@EmpID",
                    ddlEmpID.SelectedValue);

                    cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    ddlTrainingID.SelectedValue);

                    cmd.Parameters.AddWithValue(
                    "@Aspect",
                    ddlAspect.SelectedValue);

                    cmd.Parameters.AddWithValue(
                    "@Org",
                    txtOrganizedBy.Text);

                    cmd.Parameters.AddWithValue(
                    "@Remarks",
                    txtRemarks.Text);

                    cmd.Parameters.AddWithValue(
                    "@Grade",
                    ddlGrade.SelectedValue);

                    cmd.Parameters.AddWithValue(
                    "@CreatedBy",
                    createdBy);


                    con.Open();

                    cmd.ExecuteNonQuery();

                    con.Close();
                }


                BindAspect();

                txtRemarks.Text = "";

                lblSingleMessage.Text =
                "Saved Successfully";
            }

            catch (Exception ex)
            {
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
                Session["EmpID"].ToString()
                :
                "Admin";


                ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;


                int inserted = 0;
                int skipped = 0;


                using (
                ExcelPackage package =
                new ExcelPackage(
                fuExcel.FileContent))
                {
                    ExcelWorksheet ws =
                    package.Workbook
                    .Worksheets[0];


                    if (ws == null ||
                       ws.Dimension == null)
                    {
                        lblBulkMessage.Text =
                        "Excel Empty";

                        return;
                    }


                    int rowCount =
                    ws.Dimension.Rows;

                    int lastNo =
                    GetLastNo();


                    using (SqlConnection con =
                    new SqlConnection(constr))
                    {
                        con.Open();

                        for (
                        int row = 2;
                        row <= rowCount;
                        row++)
                        {
                            string emp =
                            ws.Cells[row, 1]
                            .Text.Trim();

                            string tid =
                            ws.Cells[row, 2]
                            .Text.Trim();

                            string aspect =
                            ws.Cells[row, 3]
                            .Text.Trim();

                            string org =
                            ws.Cells[row, 4]
                            .Text.Trim();

                            string rem =
                            ws.Cells[row, 5]
                            .Text.Trim();

                            string grade =
                            ws.Cells[row, 6]
                            .Text.Trim();



                            // employee assigned check

                            SqlCommand assignChk =
                            new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment

WHERE
EmpID=@EmpID
AND
TrainingID=@TrainingID

", con);


                            assignChk.Parameters
                            .AddWithValue(
                            "@EmpID",
                            emp);

                            assignChk.Parameters
                            .AddWithValue(
                            "@TrainingID",
                            tid);


                            int assigned =
                            Convert.ToInt32(
                            assignChk.ExecuteScalar());


                            if (assigned == 0)
                            {
                                skipped++;
                                continue;
                            }



                            // allowed aspect check

                            if (
                            aspect != "Travel Arrangements"
                            &&
                            aspect != "Accommodation"
                            &&
                            aspect != "Food Arrangements")
                            {
                                skipped++;
                                continue;
                            }



                            // duplicate check

                            SqlCommand chk =
                            new SqlCommand(@"

SELECT COUNT(*)

FROM FeedbackTrainingRelated

WHERE
EmpID=@EmpID
AND
TrainingID=@TrainingID
AND
TrainingRelatedAspects=@Aspect

", con);


                            chk.Parameters
                            .AddWithValue(
                            "@EmpID",
                            emp);

                            chk.Parameters
                            .AddWithValue(
                            "@TrainingID",
                            tid);

                            chk.Parameters
                            .AddWithValue(
                            "@Aspect",
                            aspect);


                            int cnt =
                            Convert.ToInt32(
                            chk.ExecuteScalar());


                            if (cnt > 0)
                            {
                                skipped++;
                                continue;
                            }



                            lastNo++;

                            string id =
                            "FTR" +
                            lastNo
                            .ToString(
                            "0000");



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
@EmpID,
@TrainingID,
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
                            id);

                            cmd.Parameters
                            .AddWithValue(
                            "@EmpID",
                            emp);

                            cmd.Parameters
                            .AddWithValue(
                            "@TrainingID",
                            tid);

                            cmd.Parameters
                            .AddWithValue(
                            "@Aspect",
                            aspect);

                            cmd.Parameters
                            .AddWithValue(
                            "@Org",
                            org);

                            cmd.Parameters
                            .AddWithValue(
                            "@Remarks",
                            rem);

                            cmd.Parameters
                            .AddWithValue(
                            "@Grade",
                            grade);

                            cmd.Parameters
                            .AddWithValue(
                            "@CreatedBy",
                            createdBy);


                            cmd.ExecuteNonQuery();

                            inserted++;
                        }

                        con.Close();
                    }
                }


                lblBulkMessage.ForeColor =
                System.Drawing.Color.Green;

                lblBulkMessage.Text =
                "Inserted : "
                + inserted +
                " | Skipped : "
                + skipped;
            }

            catch (Exception ex)
            {
                lblBulkMessage.ForeColor =
                System.Drawing.Color.Red;

                lblBulkMessage.Text =
                ex.ToString();
            }
        }
        private string GenerateOverallID()
        {
            int no = 0;

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                new SqlCommand(@"

SELECT TOP 1
OverallID

FROM FeedbackOverall

ORDER BY ID DESC

", con);

                object obj =
                cmd.ExecuteScalar();

                if (obj != null)
                {
                    string id =
                    obj.ToString();

                    if (id.StartsWith("FOR"))
                    {
                        int.TryParse(
                        id.Replace(
                        "FOR", ""),
                        out no);
                    }
                }
            }

            return
            "FOR" +
            (no + 1)
            .ToString("0000");
        }
        protected void btnOverallUpload_Click(
object sender,
EventArgs e)
        {
            try
            {
                if (!fuOverall.HasFile)
                {
                    lblBulkMessage.Text =
                    "Please select overall excel";

                    return;
                }

                string createdBy =
                Session["EmpID"] != null
                ?
                Session["EmpID"].ToString()
                :
                "Admin";


                ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;

                int inserted = 0;
                int skipped = 0;


                using (
                ExcelPackage package =
                new ExcelPackage(
                fuOverall.FileContent))
                {
                    ExcelWorksheet ws =
                    package.Workbook
                    .Worksheets[0];

                    if (
                    ws == null ||
                    ws.Dimension == null)
                    {
                        lblBulkMessage.Text =
                        "Excel Empty";

                        return;
                    }


                    int rowCount =
                    ws.Dimension.Rows;


                    using (SqlConnection con =
                    new SqlConnection(constr))
                    {
                        con.Open();

                        for (
                        int row = 2;
                        row <= rowCount;
                        row++)
                        {
                            string emp =
                            ws.Cells[row, 1]
                            .Text.Trim();

                            string tid =
                            ws.Cells[row, 2]
                            .Text.Trim();

                            string response =
                            ws.Cells[row, 3]
                            .Text.Trim();


                            // assigned employee check

                            SqlCommand assign =
                            new SqlCommand(@"

SELECT COUNT(*)

FROM TrainingAssignment

WHERE
EmpID=@Emp
AND
TrainingID=@Tid

", con);


                            assign.Parameters
                            .AddWithValue(
                            "@Emp",
                            emp);

                            assign.Parameters
                            .AddWithValue(
                            "@Tid",
                            tid);


                            if (
                            Convert.ToInt32(
                            assign.ExecuteScalar()) == 0)
                            {
                                skipped++;
                                continue;
                            }


                            // duplicate check

                            SqlCommand chk =
                            new SqlCommand(@"

SELECT COUNT(*)

FROM FeedbackOverall

WHERE
EmpID=@Emp
AND
TrainingID=@Tid

", con);


                            chk.Parameters
                            .AddWithValue(
                            "@Emp",
                            emp);

                            chk.Parameters
                            .AddWithValue(
                            "@Tid",
                            tid);


                            if (
                            Convert.ToInt32(
                            chk.ExecuteScalar()) > 0)
                            {
                                skipped++;
                                continue;
                            }



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
@Tid,
@Response,
GETDATE(),
@CreatedBy
)

", con);


                            cmd.Parameters
                            .AddWithValue(
                            "@ID",
                            GenerateOverallID());

                            cmd.Parameters
                            .AddWithValue(
                            "@Emp",
                            emp);

                            cmd.Parameters
                            .AddWithValue(
                            "@Tid",
                            tid);

                            cmd.Parameters
                            .AddWithValue(
                            "@Response",
                            response);

                            cmd.Parameters
                            .AddWithValue(
                            "@CreatedBy",
                            createdBy);

                            cmd.ExecuteNonQuery();

                            inserted++;
                        }
                    }
                }


                lblBulkMessage.ForeColor =
                System.Drawing.Color.Green;

                lblBulkMessage.Text =
                "Overall Inserted : "
                + inserted +
                " | Skipped : "
                + skipped;
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