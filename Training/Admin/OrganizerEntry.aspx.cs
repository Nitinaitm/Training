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
    public partial class OrganizerEntry : System.Web.UI.Page
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
                "SELECT * FROM TrainingOrganizerMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingOrganizer.DataSource = dt;
            gvTrainingOrganizer.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM TrainingOrganizerMaster WHERE TrainingOrganizer LIKE @Search ORDER BY ID DESC",
                con);

            da.SelectCommand.Parameters.AddWithValue("@Search",
                "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingOrganizer.DataSource = dt;
            gvTrainingOrganizer.DataBind();
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtOrganizer.Text.Trim() == "")
            {
                lblMessage.Text = "Enter Training Organizer.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT COUNT(*) FROM TrainingOrganizerMaster WHERE TrainingOrganizer=@TrainingOrganizer", con);

            chk.Parameters.AddWithValue("@TrainingOrganizer", txtOrganizer.Text.Trim());

            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
            {
                lblMessage.Text = "Training Organizer already exists.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                con.Close();
                return;
            }

            string TrainingOrganizerID = "TO-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            SqlCommand cmd = new SqlCommand(@"
    INSERT INTO TrainingOrganizerMaster
    (
        TrainingOrganizerID,
        TrainingOrganizer,
        CreatedOn,
        CreatedBy
    )
    VALUES
    (
        @TrainingOrganizerID,
        @TrainingOrganizer,
        GETDATE(),
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@TrainingOrganizerID", TrainingOrganizerID);
            cmd.Parameters.AddWithValue("@TrainingOrganizer", txtOrganizer.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Saved Successfully.";

            txtOrganizer.Text = "";
            txtOrganizer.Focus();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtOrganizer.Text = "";
            txtOrganizer.Focus();
        }
        protected void gvTrainingOrganizer_RowEditing(object sender,
           System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvTrainingOrganizer.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvTrainingOrganizer_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvTrainingOrganizer.EditIndex = -1;
            BindGrid();
        }

        protected void gvTrainingOrganizer_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingOrganizer.DataKeys[e.RowIndex].Value);

            TextBox txtTrainingOrganizer = (TextBox)gvTrainingOrganizer.Rows[e.RowIndex].Cells[2].Controls[0];

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE TrainingOrganizerMaster SET TrainingOrganizer=@TrainingOrganizer WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@TrainingOrganizer", txtTrainingOrganizer.Text.Trim());

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            gvTrainingOrganizer.EditIndex = -1;

            BindGrid();
        }

        protected void gvTrainingOrganizer_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingOrganizer.DataKeys[e.RowIndex].Value);

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM TrainingOrganizerMaster WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            BindGrid();
        }
    }
}