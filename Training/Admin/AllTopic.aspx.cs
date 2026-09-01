using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Training.Admin
{
    public partial class AllTopic : System.Web.UI.Page
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
TopicID,
TopicName,
Category,
Description,
CreatedOn

FROM TopicMaster

WHERE 1=1 ";

            if (txtTopicName.Text.Trim() != "")
            {
                query +=
                " AND TopicName LIKE '%"
                + txtTopicName.Text.Trim().Replace("'", "''")
                + "%'";
            }

            if (ddlCategory.SelectedValue != "")
            {
                query +=
                " AND Category='"
                + ddlCategory.SelectedValue
                + "'";
            }

            query += " ORDER BY ID DESC";

            DataTable dt =
                obj.GetDataTable(query);

            gvTopic.DataSource = dt;
            gvTopic.DataBind();

            lblCount.Text =
                "Total Records : "
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
            txtTopicName.Text = "";
            ddlCategory.SelectedIndex = 0;

            BindGrid();
        }
    }
}