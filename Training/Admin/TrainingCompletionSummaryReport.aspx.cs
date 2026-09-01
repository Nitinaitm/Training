using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text;

namespace Training.Admin
{
    public partial class TrainingCompletionSummaryReport
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
                //if (
                //Session["InternalRedirect_Admin"]
                //== null)
                //{
                //    Response.Redirect(
                //    "~/Default.aspx");
                //}

                BindCompany();

                BindTrainingType();
            }
        }

        private void BindCompany()
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT DISTINCT
EmpCompany

FROM EmpBasicMaster

ORDER BY EmpCompany

", con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                ddlCompany.DataSource =
                dt;

                ddlCompany.DataTextField =
                "EmpCompany";

                ddlCompany.DataValueField =
                "EmpCompany";

                ddlCompany.DataBind();

                ddlCompany.Items.Insert(
                0,
                new System.Web.UI.WebControls.ListItem(
                "All",
                ""));
            }
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
                new System.Web.UI.WebControls.ListItem(
                "All",
                ""));
            }
        }
        private DataTable BuildReportData()
        {
            DataTable dtReport =
            new DataTable();

            dtReport.Columns.Add("SlNo");
            dtReport.Columns.Add("Designation");
            dtReport.Columns.Add("TotalEmployees");

            List<string> locations =
            new List<string>();

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                StringBuilder qLoc =
                new StringBuilder();

                qLoc.Append(@"

SELECT DISTINCT

RTRIM(
TrainingOrganizer
)
+
' '
+
RTRIM(
TrainingLocation
)

AS TrainingCenter

FROM TrainingDetails

WHERE 1=1

");

                if (
                ddlTrainingType.SelectedValue
                != "")
                {
                    qLoc.Append(@"

AND TrainingType =
@TrainingType

");
                }

                SqlCommand cmdLoc =
                new SqlCommand(
                qLoc.ToString(),
                con);

                if (
                ddlTrainingType.SelectedValue
                != "")
                {
                    cmdLoc.Parameters.AddWithValue(
                    "@TrainingType",
                    ddlTrainingType.SelectedValue);
                }

                con.Open();

                SqlDataReader dr =
                cmdLoc.ExecuteReader();

                while (dr.Read())
                {
                    locations.Add(
                    dr["TrainingCenter"]
                    .ToString());

                    dtReport.Columns.Add(
                    dr["TrainingCenter"]
                    .ToString());
                }

                dr.Close();

                con.Close();
            }

            dtReport.Columns.Add(
            "TotalCompleted");

            dtReport.Columns.Add(
            "NeverCompleted");

            dtReport.Columns.Add(
            "Remarks");

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                StringBuilder q =
                new StringBuilder();

                q.Append(@"

SELECT

EBM.EmpDesignation,

COUNT(
DISTINCT EBM.EmpID
)
AS TotalEmployees,

COUNT(
DISTINCT TA.EmpID
)
AS TotalCompleted

FROM EmpBasicMaster EBM

LEFT JOIN
TrainingAssignment TA
ON EBM.EmpID =
TA.EmpID

LEFT JOIN
TrainingDetails TD
ON TD.TrainingID =
TA.TrainingID

WHERE 1=1

");

                if (
                ddlCompany.SelectedValue
                != "")
                {
                    q.Append(@"

AND EBM.EmpCompany =
@Company

");
                }

                if (
                ddlTrainingType.SelectedValue
                != "")
                {
                    q.Append(@"

AND TD.TrainingType =
@TrainingType

");
                }

                if (
                txtDateFrom.Text.Trim()
                != "")
                {
                    q.Append(@"

AND TRY_CONVERT(
date,
TD.DateFrom,
105
)

>=

@DateFrom

");
                }

                if (
                txtDateTo.Text.Trim()
                != "")
                {
                    q.Append(@"

AND TRY_CONVERT(
date,
TD.DateTo,
105
)

<=

@DateTo

");
                }

                q.Append(@"

GROUP BY
EBM.EmpDesignation

HAVING
COUNT(
DISTINCT TA.EmpID
) > 0

ORDER BY
EBM.EmpDesignation

");

                SqlCommand cmd =
                new SqlCommand(
                q.ToString(),
                con);

                if (
                ddlCompany.SelectedValue
                != "")
                {
                    cmd.Parameters.AddWithValue(
                    "@Company",
                    ddlCompany.SelectedValue);
                }

                if (
                ddlTrainingType.SelectedValue
                != "")
                {
                    cmd.Parameters.AddWithValue(
                    "@TrainingType",
                    ddlTrainingType.SelectedValue);
                }

                if (
                txtDateFrom.Text.Trim()
                != "")
                {
                    cmd.Parameters.AddWithValue(
                    "@DateFrom",
                    Convert.ToDateTime(
                    txtDateFrom.Text));
                }

                if (
                txtDateTo.Text.Trim()
                != "")
                {
                    cmd.Parameters.AddWithValue(
                    "@DateTo",
                    Convert.ToDateTime(
                    txtDateTo.Text));
                }

                SqlDataAdapter da =
                new SqlDataAdapter(cmd);

                DataTable dtDesignation =
                new DataTable();

                da.Fill(dtDesignation);

                int slNo = 1;

                foreach (
                DataRow dr
                in dtDesignation.Rows)
                {
                    DataRow row =
                    dtReport.NewRow();

                    row["SlNo"] =
                    slNo++;

                    row["Designation"] =
                    dr["EmpDesignation"];

                    row["TotalEmployees"] =
                    dr["TotalEmployees"];

                    row["TotalCompleted"] =
                    dr["TotalCompleted"];

                    row["NeverCompleted"] =
                    Convert.ToInt32(
                    dr["TotalEmployees"])
                    -
                    Convert.ToInt32(
                    dr["TotalCompleted"]);

                    row["Remarks"] =
                    "";

                    foreach (
                    string location
                    in locations)
                    {
                        row[location] =
                        GetLocationCount(
                        dr["EmpDesignation"]
                        .ToString(),
                        location);
                    }

                    dtReport.Rows.Add(
                    row);
                }
            }

            return dtReport;
        }
        private int GetLocationCount(
string designation,
string location)
        {
            using (SqlConnection con =
            new SqlConnection(constr))
            {
                string[] arr =
                location.Split(' ');

                string organizer =
                arr[0];

                string loc =
                string.Join(
                " ",
                arr.Skip(1));

                SqlCommand cmd =
                new SqlCommand(@"

SELECT
COUNT(
DISTINCT TA.EmpID
)

FROM TrainingAssignment TA

INNER JOIN
EmpBasicMaster EBM
ON EBM.EmpID =
TA.EmpID

INNER JOIN
TrainingDetails TD
ON TD.TrainingID =
TA.TrainingID

WHERE

EBM.EmpDesignation =
@Designation

AND

TD.TrainingOrganizer =
@Organizer

AND

TD.TrainingLocation =
@Location

", con);

                cmd.Parameters.AddWithValue(
                "@Designation",
                designation);

                cmd.Parameters.AddWithValue(
                "@Organizer",
                organizer);

                cmd.Parameters.AddWithValue(
                "@Location",
                loc);

                con.Open();

                return Convert.ToInt32(
                cmd.ExecuteScalar());
            }
        }

        protected void btnShowReport_Click(
object sender,
EventArgs e)
        {
            BindReport();
        }

        private void BindReport()
        {
            DataTable dt =
            BuildReportData();

            AddGrandTotalRow(dt);

            gvReport.DataSource =
            dt;

            gvReport.DataBind();

            lblTotalDesignation.Text =
            "Total Designations : " +
            (dt.Rows.Count - 1)
            .ToString();
        }

        private void AddGrandTotalRow(
DataTable dt)
        {
            if (dt.Rows.Count == 0)
                return;

            DataRow totalRow =
            dt.NewRow();

            totalRow["SlNo"] = "";

            totalRow["Designation"] =
            "GRAND TOTAL";

            foreach (
            DataColumn col
            in dt.Columns)
            {
                string colName =
                col.ColumnName;

                if (
                colName == "SlNo"
                ||
                colName == "Designation"
                ||
                colName == "Remarks"
                )
                {
                    continue;
                }

                int total = 0;

                foreach (
                DataRow row
                in dt.Rows)
                {
                    int val = 0;

                    int.TryParse(
                    Convert.ToString(
                    row[colName]),
                    out val);

                    total += val;
                }

                totalRow[colName] =
                total;
            }

            totalRow["Remarks"] =
            "";

            dt.Rows.Add(
            totalRow);
        }

        protected void gvReport_RowDataBound(
object sender,
GridViewRowEventArgs e)
        {
            if (
            e.Row.RowType ==
            DataControlRowType.DataRow)
            {
                string designation =
                DataBinder.Eval(
                e.Row.DataItem,
                "Designation")
                .ToString();

                if (
                designation ==
                "GRAND TOTAL")
                {
                    e.Row.BackColor =
                    System.Drawing.Color.LightYellow;

                    e.Row.Font.Bold =
                    true;
                }
            }
        }

        protected void btnExportExcel_Click(
object sender,
EventArgs e)
        {
            DataTable dt =
            BuildReportData();

            AddGrandTotalRow(dt);

            GridView gv =
            new GridView();

            gv.DataSource =
            dt;

            gv.DataBind();

            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",
            "attachment;filename=TrainingCompletionSummaryReport.xls");

            Response.Charset = "";

            Response.ContentType =
            "application/vnd.ms-excel";

            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);

            gv.RenderControl(hw);

            Response.Output.Write(
            sw.ToString());

            Response.Flush();

            Response.End();
        }

        public override void
VerifyRenderingInServerForm(
Control control)
        {
        }

        protected void btnDownloadPDF_Click(
object sender,
EventArgs e)
        {
            DataTable dt =
            BuildReportData();

            AddGrandTotalRow(dt);

            ExportToPDF(dt);
        }

        private void ExportToPDF(
DataTable dt)
        {
            using (MemoryStream ms =
            new MemoryStream())
            {
                Document document =
                new Document(
                PageSize.A4.Rotate(),
                10f,
                10f,
                20f,
                20f);

                PdfWriter.GetInstance(
                document,
                ms);

                document.Open();

                Font titleFont =
                FontFactory.GetFont(
                FontFactory.HELVETICA_BOLD,
                16);

                Font headerFont =
                FontFactory.GetFont(
                FontFactory.HELVETICA_BOLD,
                9);

                Font bodyFont =
                FontFactory.GetFont(
                FontFactory.HELVETICA,
                8);

                Paragraph title =
                new Paragraph(
                "TRAINING COMPLETION SUMMARY REPORT",
                titleFont);

                title.Alignment =
                Element.ALIGN_CENTER;

                document.Add(title);

                document.Add(
                new Paragraph(" "));

                Paragraph generated =
                new Paragraph(
                "Generated On : "
                +
                DateTime.Now
                .ToString(
                "dd-MM-yyyy HH:mm"),
                bodyFont);

                generated.Alignment =
                Element.ALIGN_RIGHT;

                document.Add(
                generated);

                document.Add(
                new Paragraph(" "));

                PdfPTable table =
                new PdfPTable(
                dt.Columns.Count);

                table.WidthPercentage =
                100;

                float[] widths =
                new float[
                dt.Columns.Count];

                for (
                int i = 0;
                i < dt.Columns.Count;
                i++)
                {
                    widths[i] = 3f;
                }

                table.SetWidths(
                widths);

                foreach (
                DataColumn col
                in dt.Columns)
                {
                    PdfPCell cell =
                    new PdfPCell(
                    new Phrase(
                    col.ColumnName,
                    headerFont));

                    cell.HorizontalAlignment =
                    Element.ALIGN_CENTER;

                    cell.BackgroundColor =
                    BaseColor.LIGHT_GRAY;

                    table.AddCell(
                    cell);
                }

                foreach (
                DataRow row
                in dt.Rows)
                {
                    bool isGrandTotal =
                    row["Designation"]
                    .ToString()
                    ==
                    "GRAND TOTAL";

                    foreach (
                    DataColumn col
                    in dt.Columns)
                    {
                        Font fontToUse =
                        isGrandTotal
                        ?
                        headerFont
                        :
                        bodyFont;

                        PdfPCell cell =
                        new PdfPCell(
                        new Phrase(
                        Convert.ToString(
                        row[col]),
                        fontToUse));

                        if (isGrandTotal)
                        {
                            cell.BackgroundColor =
                            BaseColor.YELLOW;
                        }

                        table.AddCell(
                        cell);
                    }
                }

                document.Add(
                table);

                document.Close();

                byte[] bytes =
                ms.ToArray();

                Response.Clear();

                Response.ContentType =
                "application/pdf";

                Response.AddHeader(
                "content-disposition",
                "attachment;filename=TrainingCompletionSummaryReport.pdf");

                Response.Buffer = true;

                Response.Cache.SetCacheability(
                System.Web.HttpCacheability.NoCache);

                Response.BinaryWrite(
                bytes);

                Response.End();
            }
        }

    }
}