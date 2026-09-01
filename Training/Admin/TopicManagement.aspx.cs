using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Training.Admin
{
    public partial class TopicManagement :
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
ID,
TopicID,
TopicName,
Category,
Description

FROM TopicMaster

WHERE 1=1 ";

            if (txtSearchTopic.Text.Trim() != "")
            {
                query += @"

AND TopicName LIKE '%"
+ txtSearchTopic.Text.Trim()
.Replace("'", "''")
+ "%'";
            }

            if (ddlSearchCategory.SelectedValue != "")
            {
                query += @"

AND Category='"
+ ddlSearchCategory.SelectedValue
+ "'";
            }

            query +=
            " ORDER BY ID DESC";

            gvTopic.DataSource =
                obj.GetDataTable(query);

            gvTopic.DataBind();
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
            txtSearchTopic.Text = "";

            ddlSearchCategory
            .SelectedIndex = 0;

            BindGrid();
        }

        protected void gvTopic_RowCommand(
            object sender,
            System.Web.UI.WebControls
            .GridViewCommandEventArgs e)
        {
            if (e.CommandName ==
                "DeleteTopic")
            {
                DeleteTopic(
                    e.CommandArgument
                    .ToString());
            }

            if (e.CommandName ==
                "EditTopic")
            {
                LoadTopicForEdit(
                    e.CommandArgument
                    .ToString());
            }
        }

        private void LoadTopicForEdit(
            string id)
        {
            DataTable dt =
                obj.GetDataTable(@"

SELECT *
FROM TopicMaster
WHERE ID='"
+ id + "'");

            if (dt.Rows.Count == 0)
                return;

            pnlEdit.Visible = true;

            hfID.Value =
                id;

            txtEditTopicName.Text =
                dt.Rows[0]["TopicName"]
                .ToString();

            ddlEditCategory
                .SelectedValue =
                dt.Rows[0]["Category"]
                .ToString();

            txtEditDescription.Text =
                dt.Rows[0]["Description"]
                .ToString();
        }

        private void DeleteTopic(
            string id)
        {
            DataTable dtTopic =
                obj.GetDataTable(@"

SELECT TopicID
FROM TopicMaster
WHERE ID='"
+ id + "'");

            if (dtTopic.Rows.Count == 0)
                return;

            string topicID =
                dtTopic.Rows[0]["TopicID"]
                .ToString();

            DataTable dtCheck =
                obj.GetDataTable(@"

SELECT *
FROM TrainingDetails
WHERE TopicID='"
+ topicID + "'");

            if (dtCheck.Rows.Count > 0)
            {
                lblMessage.Text =
                    "Topic is already mapped with Training. Cannot Delete.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            int i =
                obj.ExecuteSql(@"

DELETE FROM TopicMaster
WHERE ID='"
+ id + "'");

            if (i > 0)
            {
                lblMessage.Text =
                    "Topic Deleted Successfully";

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                BindGrid();
            }
        }
        protected void btnUpdate_Click(
    object sender,
    EventArgs e)
        {
            if (txtEditTopicName.Text.Trim() == "")
            {
                lblMessage.Text =
                    "Topic Name Required";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            DataTable dtDup =
                obj.GetDataTable(@"

SELECT *
FROM TopicMaster

WHERE UPPER(TopicName)=UPPER('"
+ txtEditTopicName.Text.Trim()
.Replace("'", "''")
+ @"')

AND ID <> '"
+ hfID.Value + "'");

            if (dtDup.Rows.Count > 0)
            {
                lblMessage.Text =
                    "Topic Name Already Exists";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            string query = @"

UPDATE TopicMaster

SET

TopicName='"
+ txtEditTopicName.Text.Trim()
.Replace("'", "''")
+ @"',

Category='"
+ ddlEditCategory.SelectedValue
+ @"',

Description='"
+ txtEditDescription.Text.Trim()
.Replace("'", "''")
+ @"'

WHERE ID='"
+ hfID.Value + "'";

            int i =
                obj.ExecuteSql(query);

            if (i > 0)
            {
                lblMessage.Text =
                    "Topic Updated Successfully";

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                pnlEdit.Visible = false;

                BindGrid();
            }
            else
            {
                lblMessage.Text =
                    "Error While Updating Topic";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;
            }
        }

        protected void btnCancel_Click(
            object sender,
            EventArgs e)
        {
            pnlEdit.Visible = false;

            hfID.Value = "";

            txtEditTopicName.Text = "";

            txtEditDescription.Text = "";

            ddlEditCategory.SelectedIndex = 0;

            lblMessage.Text = "";
        }

    }
}