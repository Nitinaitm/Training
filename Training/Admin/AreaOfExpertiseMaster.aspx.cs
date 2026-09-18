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
    public partial class AreaOfExpertiseMaster : System.Web.UI.Page
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
                "SELECT * FROM AreaOfExpertiseMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvAreaOfExpertise.DataSource = dt;
            gvAreaOfExpertise.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM AreaOfExpertiseMaster WHERE ExpertiseName LIKE @Search ORDER BY ID DESC",
                con);

            da.SelectCommand.Parameters.AddWithValue("@Search",
                "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvAreaOfExpertise.DataSource = dt;
            gvAreaOfExpertise.DataBind();
        }

        protected void gvAreaOfExpertise_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvAreaOfExpertise.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvAreaOfExpertise_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvAreaOfExpertise.EditIndex = -1;
            BindGrid();
        }

        protected void gvAreaOfExpertise_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int ID = Convert.ToInt32(gvAreaOfExpertise.DataKeys[e.RowIndex].Value);

            TextBox txtAreaOfExpertise = (TextBox)gvAreaOfExpertise.Rows[e.RowIndex].Cells[2].Controls[0];

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE AreaOfExpertiseMaster SET ExpertiseName=@ExpertiseName WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ExpertiseName", txtAreaOfExpertise.Text.Trim());

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            gvAreaOfExpertise.EditIndex = -1;

            BindGrid();
        }

        protected void gvAreaOfExpertise_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvAreaOfExpertise.DataKeys[e.RowIndex].Value);

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM AreaOfExpertiseMaster WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtAreaOfExpertise.Text.Trim() == "")
            {
                lblMessage.Text = "Enter Area of Expertise.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT COUNT(*) FROM AreaOfExpertiseMaster WHERE ExpertiseName=@ExpertiseName", con);

            chk.Parameters.AddWithValue("@ExpertiseName", txtAreaOfExpertise.Text.Trim());

            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
            {
                lblMessage.Text = "Area of Expertise already exists.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                con.Close();
                return;
            }

            string ExpertiseID = "AOE-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            SqlCommand cmd = new SqlCommand(@"
    INSERT INTO AreaOfExpertiseMaster
    (
        ExpertiseID,
        ExpertiseName,
        CreatedOn,
        CreatedBy
    )
    VALUES
    (
        @ExpertiseID,
        @ExpertiseName,
        GETDATE(),
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@ExpertiseID", ExpertiseID);
            cmd.Parameters.AddWithValue("@ExpertiseName", txtAreaOfExpertise.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Saved Successfully.";

            txtAreaOfExpertise.Text = "";
            txtAreaOfExpertise.Focus();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtAreaOfExpertise.Text = "";
            txtAreaOfExpertise.Focus();
        }
    }
}