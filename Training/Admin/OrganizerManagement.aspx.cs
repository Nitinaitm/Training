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
    public partial class OrganizerManagement : System.Web.UI.Page
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