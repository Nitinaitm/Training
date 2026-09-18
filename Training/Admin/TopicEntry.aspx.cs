using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;


namespace Training.Admin
{
    public partial class TopicEntry : System.Web.UI.Page
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

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            string topicName =
                txtTopicName.Text.Trim();

            string checkQuery = @"

SELECT COUNT(*)
FROM TopicMaster
WHERE UPPER(TopicName)
=
UPPER('" + topicName.Replace("'", "''") + "')";

            int count =
                Convert.ToInt32(
                obj.ExecuteScalar(
                checkQuery));

            if (count > 0)
            {
                lblMessage.Text =
                    "Topic already exists.";

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                return;
            }

            string topicID =
                GenerateTopicID();

            string query = @"

INSERT INTO TopicMaster
(
TopicID,
TopicName,
Category,
Description,
CreatedOn,
CreatedBy
)

VALUES
(
'" + topicID + @"',
'" + topicName.Replace("'", "''") + @"',
'" + ddlCategory.SelectedValue + @"',
'" + txtDescription.Text.Replace("'", "''") + @"',
GETDATE(),
'Admin'
)";

            obj.ExecuteSql(query);

            lblMessage.Text =
                "Topic Saved Successfully.";

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            txtTopicName.Text = "";
            txtDescription.Text = "";
            ddlCategory.SelectedIndex = 0;

            BindGrid();
        }

        private string GenerateTopicID()
        {
            string query =
                "SELECT ISNULL(MAX(ID),0)+1 FROM TopicMaster";

            int nextID =
                Convert.ToInt32(
                obj.ExecuteScalar(query));

            return "TOP" +
                nextID.ToString("0000");
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

ORDER BY ID DESC";

            gvTopic.DataSource =
                obj.GetDataTable(query);

            gvTopic.DataBind();
        }
    }
}