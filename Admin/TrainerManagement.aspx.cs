using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Training.Admin
{
    public partial class TrainerManagement :
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
                BindGrid();
            }
        }

        private void BindGrid()
        {
            string query = @"

SELECT

TM.ID,
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

            if (ddlSearchTrainerType.SelectedValue != "")
            {
                query +=
                " AND TM.TrainerType='"
                + ddlSearchTrainerType.SelectedValue
                + "'";
            }

            if (txtSearchEmpID.Text.Trim() != "")
            {
                query += @"

AND
(
TM.EmpID LIKE '%"
+ txtSearchEmpID.Text.Trim().Replace("'", "''")
+ @"%'

OR

TM.EmpIDExternal LIKE '%"
+ txtSearchEmpID.Text.Trim().Replace("'", "''")
+ @"%'
)";
            }

            if (txtSearchName.Text.Trim() != "")
            {
                query += @"

AND
(
E.EmpName LIKE '%"
+ txtSearchName.Text.Trim().Replace("'", "''")
+ @"%'

OR

TM.NameExternal LIKE '%"
+ txtSearchName.Text.Trim().Replace("'", "''")
+ @"%'
)";
            }

            query +=
            " ORDER BY TM.ID DESC";

            gvTrainer.DataSource =
                obj.GetDataTable(query);

            gvTrainer.DataBind();
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
            ddlSearchTrainerType.SelectedIndex = 0;

            txtSearchEmpID.Text = "";

            txtSearchName.Text = "";

            BindGrid();
        }

        protected void gvTrainer_RowCommand(
            object sender,
            System.Web.UI.WebControls
            .GridViewCommandEventArgs e)
        {
            if (e.CommandName ==
                "DeleteTrainer")
            {
                string id =
                    e.CommandArgument
                    .ToString();

                obj.ExecuteSql(
                "DELETE FROM TrainerMaster WHERE ID='"
                + id + "'");

                BindGrid();

                return;
            }

            if (e.CommandName ==
                "EditTrainer")
            {
                string id =
                    e.CommandArgument
                    .ToString();

                LoadTrainerForEdit(id);
            }
        }

        private void LoadTrainerForEdit(
            string id)
        {
            DataTable dt =
                obj.GetDataTable(@"

SELECT *
FROM TrainerMaster
WHERE ID='"
+ id + "'");

            if (dt.Rows.Count == 0)
                return;

            hfID.Value =
                id;

            hfTrainerType.Value =
                dt.Rows[0]["TrainerType"]
                .ToString();

            pnlEdit.Visible = true;

            pnlEditInternal.Visible = false;
            pnlEditExternal.Visible = false;

            if (hfTrainerType.Value ==
                "Internal")
            {
                pnlEditInternal.Visible =
                    true;

                txtEditEmpID.Text =
                    dt.Rows[0]["EmpID"]
                    .ToString();

                txtEditRemarksInternal.Text =
                    dt.Rows[0]["Remarks"]
                    .ToString();
            }
            else
            {
                pnlEditExternal.Visible =
                    true;

                txtEditEmpIDExternal.Text =
                    dt.Rows[0]["EmpIDExternal"]
                    .ToString();

                txtEditName.Text =
                    dt.Rows[0]["NameExternal"]
                    .ToString();

                txtEditDesignation.Text =
                    dt.Rows[0]["DesignationExternal"]
                    .ToString();

                txtEditOrganization.Text =
                    dt.Rows[0]["TrainerOrganizerExternal"]
                    .ToString();

                txtEditRemarksExternal.Text =
                    dt.Rows[0]["Remarks"]
                    .ToString();
            }
        }
        protected void btnUpdate_Click(
    object sender,
    EventArgs e)
        {
            if (hfTrainerType.Value ==
                "Internal")
            {
                UpdateInternalTrainer();
            }
            else
            {
                UpdateExternalTrainer();
            }
        }

        private void UpdateInternalTrainer()
        {
            string query = @"

UPDATE TrainerMaster

SET

Remarks='"
+ txtEditRemarksInternal.Text
.Replace("'", "''")
+ @"'

WHERE ID='"
+ hfID.Value
+ "'";

            int i =
                obj.ExecuteSql(query);

            if (i > 0)
            {
                lblMessage.Text =
                    "Trainer Updated Successfully";

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                pnlEdit.Visible = false;

                BindGrid();
            }
            else
            {
                lblMessage.Text =
                    "Error While Updating";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;
            }
        }

        private void UpdateExternalTrainer()
        {
            if (txtEditName.Text.Trim() == "")
            {
                lblMessage.Text =
                    "Name Required";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            string query = @"

UPDATE TrainerMaster

SET

EmpIDExternal='"
+ txtEditEmpIDExternal.Text
.Replace("'", "''")
+ @"',

NameExternal='"
+ txtEditName.Text
.Replace("'", "''")
+ @"',

DesignationExternal='"
+ txtEditDesignation.Text
.Replace("'", "''")
+ @"',

TrainerOrganizerExternal='"
+ txtEditOrganization.Text
.Replace("'", "''")
+ @"',

Remarks='"
+ txtEditRemarksExternal.Text
.Replace("'", "''")
+ @"'

WHERE ID='"
+ hfID.Value
+ "'";

            int i =
                obj.ExecuteSql(query);

            if (i > 0)
            {
                lblMessage.Text =
                    "Trainer Updated Successfully";

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                pnlEdit.Visible = false;

                BindGrid();
            }
            else
            {
                lblMessage.Text =
                    "Error While Updating";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;
            }
        }

        protected void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            pnlEdit.Visible = false;

            pnlEditInternal.Visible = false;

            pnlEditExternal.Visible = false;

            hfID.Value = "";

            hfTrainerType.Value = "";

            lblMessage.Text = "";
        }
    }
}