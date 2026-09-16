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
    public partial class LocationEntry : System.Web.UI.Page
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
                "SELECT * FROM TrainingLocationMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingLocation.DataSource = dt;
            gvTrainingLocation.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM TrainingLocationMaster WHERE TrainingLocation LIKE @Search ORDER BY ID DESC",
                con);

            da.SelectCommand.Parameters.AddWithValue("@Search",
                "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvTrainingLocation.DataSource = dt;
            gvTrainingLocation.DataBind();


        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtLocation.Text.Trim() == "")
            {
                lblMessage.Text = "Enter Training Location.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT COUNT(*) FROM TrainingLocationMaster WHERE TrainingLocation=@TrainingLocation", con);

            chk.Parameters.AddWithValue("@TrainingLocation", txtLocation.Text.Trim());

            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
            {
                lblMessage.Text = "Training Location already exists.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                con.Close();
                return;
            }

            string TrainingLocationID = "TL-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            SqlCommand cmd = new SqlCommand(@"
    INSERT INTO TrainingLocationMaster
    (
        TrainingLocationID,
        TrainingLocation,
        CreatedOn,
        CreatedBy
    )
    VALUES
    (
        @TrainingLocationID,
        @TrainingLocation,
        GETDATE(),
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@TrainingLocationID", TrainingLocationID);
            cmd.Parameters.AddWithValue("@TrainingLocation", txtLocation.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Saved Successfully.";

            txtLocation.Text = "";
            txtLocation.Focus();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtLocation.Text = "";
            txtLocation.Focus();
        }

        protected void gvTrainingLocation_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvTrainingLocation.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvTrainingLocation_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvTrainingLocation.EditIndex = -1;
            BindGrid();
        }

        protected void gvTrainingLocation_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingLocation.DataKeys[e.RowIndex].Value);

            TextBox txtTrainingLocation = (TextBox)gvTrainingLocation.Rows[e.RowIndex].Cells[2].Controls[0];

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE TrainingLocationMaster SET TrainingLocation=@TrainingLocation WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@TrainingLocation", txtTrainingLocation.Text.Trim());

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            gvTrainingLocation.EditIndex = -1;

            BindGrid();
        }

        protected void gvTrainingLocation_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvTrainingLocation.DataKeys[e.RowIndex].Value);

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM TrainingLocationMaster WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            BindGrid();
        }
    }
}