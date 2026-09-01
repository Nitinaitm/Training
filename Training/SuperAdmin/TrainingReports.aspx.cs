using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.SuperAdmin
{
    public partial class TrainingReports : System.Web.UI.Page
    {
        string constr =
        ConfigurationManager
        .ConnectionStrings["constr"]
        .ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (
                   Session["InternalRedirect_SuperAdmin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                LoadReport();
            }
        }

        protected void ddlReport_SelectedIndexChanged(
        object sender,
        EventArgs e)
        {
            LoadReport();
        }

        protected void btnLoad_Click(
        object sender,
        EventArgs e)
        {
            LoadReport();
        }

        private void LoadReport()
        {
            string q = "";

            switch (ddlReport.SelectedValue)
            {
                case "1":

                    q = @"

                    SELECT
                    TD.TrainingID,
                    TD.TrainingType,
                    TD.TrainingOrganizer,
                    TD.TrainingLocation,
                    TD.Batch,

                    COUNT(TA.EmpID)
                    AS TotalAssigned,

                    SUM(
                    CASE
                    WHEN TA.TrainingAttended='Yes'
                    THEN 1
                    ELSE 0
                    END
                    )
                    AS TotalAttended

                    FROM TrainingDetails TD

                    LEFT JOIN
                    TrainingAssignment TA
                    ON TD.TrainingID=
                    TA.TrainingID

                    GROUP BY

                    TD.TrainingID,
                    TD.TrainingType,
                    TD.TrainingOrganizer,
                    TD.TrainingLocation,
                    TD.Batch";

                    break;



                case "2":

                    q = @"

                    SELECT

                    E.EmpID,
                    E.EmpName,
                    E.EmpDesignation,

                    T.TrainingID,

                    TD.TrainingType,

                    T.TrainingAttended

                    FROM TrainingAssignment T

                    INNER JOIN
                    EmpBasicMaster E

                    ON E.EmpID=T.EmpID

                    INNER JOIN
                    TrainingDetails TD

                    ON TD.TrainingID=
                    T.TrainingID";

                    break;



                case "3":

                    q = @"

                    SELECT

                    TrainingID,

                    COUNT(*)
                    Assigned,

                    SUM(
                    CASE
                    WHEN TrainingAttended='Yes'
                    THEN 1
                    ELSE 0
                    END
                    )
                    Attended,

                    CAST(

                    SUM(
                    CASE
                    WHEN TrainingAttended='Yes'
                    THEN 1
                    ELSE 0
                    END
                    )

                    *100.0/

                    COUNT(*)

                    AS DECIMAL(5,2)

                    )

                    AttendancePercent

                    FROM TrainingAssignment

                    GROUP BY
                    TrainingID";

                    break;



                case "4":

                    q = @"

                    SELECT

                    T.TrainingID,

                    E.EmpID,
                    E.EmpName,
                    E.EmpDesignation

                    FROM TrainingAssignment T

                    INNER JOIN
                    EmpBasicMaster E

                    ON E.EmpID=T.EmpID

                    WHERE
                    ISNULL(
                    T.TrainingAttended,'')
                    =''";

                    break;



                case "5":

                    q = @"

                    SELECT

                    EmpID,
                    EmpName,
                    EmpDesignation

                    FROM EmpBasicMaster

                    WHERE EmpID
                    NOT IN

                    (
                    SELECT EmpID
                    FROM TrainingAssignment
                    )";

                    break;
            }

            using (SqlConnection con =
            new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(q, con);

                DataTable dt =
                new DataTable();

                da.Fill(dt);

                gvReport.DataSource = dt;

                gvReport.DataBind();

                Session["ReportData"] = dt;
            }
        }


        protected void gvReport_PageIndexChanging(
        object sender,
        GridViewPageEventArgs e)
        {
            gvReport.PageIndex =
            e.NewPageIndex;

            gvReport.DataSource =
            (DataTable)
            Session["ReportData"];

            gvReport.DataBind();
        }


        protected void btnExcel_Click(
        object sender,
        EventArgs e)
        {
            gvReport.AllowPaging = false;

            LoadReport();

            Response.Clear();

            Response.Buffer = true;

            Response.AddHeader(
            "content-disposition",
            "attachment;filename=TrainingReport.xls");

            Response.Charset = "";

            Response.ContentType =
            "application/ms-excel";


            StringWriter sw =
            new StringWriter();

            HtmlTextWriter hw =
            new HtmlTextWriter(sw);


            gvReport.HeaderRow.Style.Add(
            "background-color",
            "#FFFFFF");

            for (int i = 0;
            i < gvReport.HeaderRow.Cells.Count;
            i++)
            {
                gvReport.HeaderRow.Cells[i]
                .Style.Add(
                "background-color",
                "#d9edf7");
            }


            gvReport.RenderControl(hw);


            Response.Output.Write(
            sw.ToString());

            Response.Flush();

            Response.End();


            gvReport.AllowPaging = true;

            LoadReport();
        }



        public override void VerifyRenderingInServerForm(
        Control control)
        {

        }

    }
}