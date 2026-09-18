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
    public partial class TrainingTypeEntry : System.Web.UI.Page
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
                "SELECT * FROM TrainingMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingType.DataSource = dt;
            gvTrainingType.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM TrainingMaster WHERE TrainingType LIKE @Search ORDER BY ID DESC",
                con);

            da.SelectCommand.Parameters.AddWithValue("@Search",
                "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingType.DataSource = dt;
            gvTrainingType.DataBind();
        }

        protected void gvTrainingType_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvTrainingType.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvTrainingType_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvTrainingType.EditIndex = -1;
            BindGrid();
        }

        protected void gvTrainingType_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingType.DataKeys[e.RowIndex].Value);

            TextBox txtType = (TextBox)gvTrainingType.Rows[e.RowIndex].Cells[2].Controls[0];

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE TrainingMaster SET TrainingType=@TrainingType WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@TrainingType", txtType.Text.Trim());

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            gvTrainingType.EditIndex = -1;

            BindGrid();
        }

        protected void gvTrainingType_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingType.DataKeys[e.RowIndex].Value);

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM TrainingMaster WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtTrainingType.Text.Trim() == "")
            {
                lblMessage.Text = "Enter Training Type.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT COUNT(*) FROM TrainingMaster WHERE TrainingType=@TrainingType", con);

            chk.Parameters.AddWithValue("@TrainingType", txtTrainingType.Text.Trim());

            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
            {
                lblMessage.Text = "Training Type already exists.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                con.Close();
                return;
            }

            string TrainingTypeID = "TT-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            SqlCommand cmd = new SqlCommand(@"
    INSERT INTO TrainingMaster
    (
        TrainingTypeID,
        TrainingType,
        CreatedOn,
        CreatedBy
    )
    VALUES
    (
        @TrainingTypeID,
        @TrainingType,
        GETDATE(),
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@TrainingTypeID", TrainingTypeID);
            cmd.Parameters.AddWithValue("@TrainingType", txtTrainingType.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Saved Successfully.";

            txtTrainingType.Text = "";
            txtTrainingType.Focus();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtTrainingType.Text = "";
            txtTrainingType.Focus();
        }
    }
}