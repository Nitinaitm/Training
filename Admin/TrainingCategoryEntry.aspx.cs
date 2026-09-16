using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingCategoryEntry : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        void BindGrid()
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM TrainingCategoryMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingCategory.DataSource = dt;
            gvTrainingCategory.DataBind();
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM TrainingCategoryMaster WHERE TrainingCategory LIKE @Search ORDER BY ID DESC",
                con);

            da.SelectCommand.Parameters.AddWithValue("@Search",
                "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingCategory.DataSource = dt;
            gvTrainingCategory.DataBind();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtTrainingCategory.Text.Trim() == "")
            {
                lblMessage.Text = "Enter Training Category.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT COUNT(*) FROM TrainingCategoryMaster WHERE TrainingCategory=@TrainingCategory", con);

            chk.Parameters.AddWithValue("@TrainingCategory", txtTrainingCategory.Text.Trim());

            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
            {
                lblMessage.Text = "Training Category already exists.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                con.Close();
                return;
            }

            string TrainingCategoryID = "TC-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            SqlCommand cmd = new SqlCommand(@"
    INSERT INTO TrainingCategoryMaster
    (
        TrainingCategoryID,
        TrainingCategory,
        CreatedOn,
        CreatedBy
    )
    VALUES
    (
        @TrainingCategoryID,
        @TrainingCategory,
        GETDATE(),
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@TrainingCategoryID", TrainingCategoryID);
            cmd.Parameters.AddWithValue("@TrainingCategory", txtTrainingCategory.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Saved Successfully.";

            txtTrainingCategory.Text = "";
            txtTrainingCategory.Focus();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtTrainingCategory.Text = "";
            txtTrainingCategory.Focus();
        }
        protected void gvTrainingCategory_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvTrainingCategory.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvTrainingCategory_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvTrainingCategory.EditIndex = -1;
            BindGrid();
        }

        protected void gvTrainingCategory_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingCategory.DataKeys[e.RowIndex].Value);

            TextBox txtCategory = (TextBox)gvTrainingCategory.Rows[e.RowIndex].Cells[2].Controls[0];

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE TrainingCategoryMaster SET TrainingCategory=@TrainingCategory WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@TrainingCategory", txtCategory.Text.Trim());

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            gvTrainingCategory.EditIndex = -1;

            BindGrid();
        }

        protected void gvTrainingCategory_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingCategory.DataKeys[e.RowIndex].Value);

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM TrainingCategoryMaster WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            BindGrid();
        }
    }
}