using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Training.Admin
{
    public partial class AllTrainers : System.Web.UI.Page
    {
        clsDataAccess obj =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string query = @"

SELECT

TM.TrainerID,
TM.TrainerType,

CASE
WHEN TM.TrainerType='Internal'
THEN TM.EmpID
ELSE TM.EmpIDExternal
END AS EmpID,

CASE
WHEN TM.TrainerType='Internal'
THEN E.EmpName
ELSE TM.NameExternal
END AS TrainerName,

CASE
WHEN TM.TrainerType='Internal'
THEN E.EmpDesignation
ELSE TM.DesignationExternal
END AS Designation,

CASE
WHEN TM.TrainerType='Internal'
THEN E.EmpCompany
ELSE TM.TrainerOrganizerExternal
END AS Organization,

TM.Remarks

FROM TrainerMaster TM

LEFT JOIN EmpBasicMaster E
ON TM.EmpID = E.EmpID

WHERE 1=1 ";

            if (ddlTrainerType.SelectedValue != "")
            {
                query +=
                " AND TM.TrainerType='"
                + ddlTrainerType.SelectedValue
                + "'";
            }

            if (txtEmpID.Text.Trim() != "")
            {
                query +=
                @" AND
                (
                TM.EmpID LIKE '%" +
                txtEmpID.Text.Trim().Replace("'", "''")
                + @"%'
                OR
                TM.EmpIDExternal LIKE '%" +
                txtEmpID.Text.Trim().Replace("'", "''")
                + @"%'
                )";
            }

            if (txtTrainerName.Text.Trim() != "")
            {
                query +=
                @" AND
                (
                E.EmpName LIKE '%" +
                txtTrainerName.Text.Trim().Replace("'", "''")
                + @"%'
                OR
                TM.NameExternal LIKE '%" +
                txtTrainerName.Text.Trim().Replace("'", "''")
                + @"%'
                )";
            }

            if (txtOrganization.Text.Trim() != "")
            {
                query +=
                @" AND
                (
                E.EmpCompany LIKE '%" +
                txtOrganization.Text.Trim().Replace("'", "''")
                + @"%'
                OR
                TM.TrainerOrganizerExternal LIKE '%" +
                txtOrganization.Text.Trim().Replace("'", "''")
                + @"%'
                )";
            }

            query +=
            " ORDER BY TM.ID DESC";

            DataTable dt =
                obj.GetDataTable(query);

            gvTrainer.DataSource = dt;
            gvTrainer.DataBind();

            lblCount.Text =
                "Total Trainers : "
                + dt.Rows.Count;
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
            ddlTrainerType.SelectedIndex = 0;
            txtEmpID.Text = "";
            txtTrainerName.Text = "";
            txtOrganization.Text = "";

            BindGrid();
        }
    }
}