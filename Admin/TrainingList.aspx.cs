using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;


namespace Training.Admin
{
    public partial class TrainingList :
        System.Web.UI.Page
    {
        clsDataAccess obj =
            new clsDataAccess();

        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            if (!IsPostBack)
            {
                Session.Remove("TrainingID");

                BindGrid();
            }
        }

        private void BindGrid()
        {
            string query = @"

SELECT
TD.TrainingID,
TM.CourseName,
TD.TrainingCategory,
TD.TrainingType,
TD.TrainingLocation,
TD.Batch,
TD.DateFrom,
TD.DateTo,
ISNULL(TD.TrainingStatus,'Draft')
AS TrainingStatus

FROM TrainingDetails TD

INNER JOIN CourseMaster TM
ON TD.CourseID = TM.CourseID

WHERE 1=1 ";

            if (txtTrainingID.Text.Trim() != "")
            {
                query += @"

AND TD.TrainingID LIKE '%"
                + txtTrainingID.Text.Trim()
                .Replace("'", "''")
                + "%'";
            }

            if (txtCourse.Text.Trim() != "")
            {
                query += @"

AND TM.CourseName LIKE '%"
                + txtCourse.Text.Trim()
                .Replace("'", "''")
                + "%'";
            }

            if (ddlStatus.SelectedValue != "")
            {
                query += @"

AND TD.TrainingStatus='"
                + ddlStatus.SelectedValue
                + "'";
            }

            query += @"

ORDER BY TD.ID DESC";

            gvTraining.DataSource =
                obj.GetDataTable(query);

            gvTraining.DataBind();
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
            txtTrainingID.Text = "";

            txtCourse.Text = "";

            ddlStatus.SelectedIndex = 0;

            BindGrid();
        }

        protected void gvTraining_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Manage")
            {
                Session["TrainingID"] =
                    e.CommandArgument
                    .ToString();

                Response.Redirect(
                    "ManageTraining.aspx");
            }
        }
    }
}