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
    public partial class QualificationMaster : System.Web.UI.Page
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
                "SELECT * FROM QualificationMaster ORDER BY ID DESC", con);

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvQualification.DataSource = dt;
            gvQualification.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(
                "SELECT * FROM QualificationMaster WHERE QualificationName LIKE @Search ORDER BY ID DESC",
                con);

            da.SelectCommand.Parameters.AddWithValue("@Search",
                "%" + txtSearch.Text.Trim() + "%");

            DataTable dt = new DataTable();

            da.Fill(dt);

            gvQualification.DataSource = dt;
            gvQualification.DataBind();
        }

        protected void gvQualification_RowEditing(object sender,
            System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            gvQualification.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvQualification_RowCancelingEdit(object sender,
            System.Web.UI.WebControls.GridViewCancelEditEventArgs e)
        {
            gvQualification.EditIndex = -1;
            BindGrid();
        }

        protected void gvQualification_RowUpdating(object sender,
            System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            int ID = Convert.ToInt32(gvQualification.DataKeys[e.RowIndex].Value);

            TextBox txtQualification = (TextBox)gvQualification.Rows[e.RowIndex].Cells[2].Controls[0];

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE QualificationMaster SET QualificationName=@QualificationName WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@QualificationName", txtQualification.Text.Trim());

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            gvQualification.EditIndex = -1;

            BindGrid();
        }

        protected void gvQualification_RowDeleting(object sender,
            System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int ID = Convert.ToInt32(gvQualification.DataKeys[e.RowIndex].Value);

            con.Open();

            SqlCommand cmd = new SqlCommand(
                "DELETE FROM QualificationMaster WHERE ID=@ID", con);

            cmd.Parameters.AddWithValue("@ID", ID);

            cmd.ExecuteNonQuery();

            con.Close();

            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (txtQualification.Text.Trim() == "")
            {
                lblMessage.Text = "Enter Qualification.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                return;
            }

            con.Open();

            SqlCommand chk = new SqlCommand(
                "SELECT COUNT(*) FROM QualificationMaster WHERE QualificationName=@QualificationName", con);

            chk.Parameters.AddWithValue("@QualificationName", txtQualification.Text.Trim());

            if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
            {
                lblMessage.Text = "Qualification already exists.";
                lblMessage.ForeColor = System.Drawing.Color.Red;
                con.Close();
                return;
            }

            string QualificationID = "Q-" + DateTime.Now.ToString("yyyyMMddHHmmss");

            SqlCommand cmd = new SqlCommand(@"
    INSERT INTO QualificationMaster
    (
        QualificationID,
        QualificationName,
        CreatedOn,
        CreatedBy
    )
    VALUES
    (
        @QualificationID,
        @QualificationName,
        GETDATE(),
        @CreatedBy
    )", con);

            cmd.Parameters.AddWithValue("@QualificationID", QualificationID);
            cmd.Parameters.AddWithValue("@QualificationName", txtQualification.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", "Admin");

            cmd.ExecuteNonQuery();

            con.Close();

            lblMessage.ForeColor = System.Drawing.Color.Green;
            lblMessage.Text = "Saved Successfully.";

            txtQualification.Text = "";
            txtQualification.Focus();

            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void Clear()
        {
            txtQualification.Text = "";
            txtQualification.Focus();
        }
    }
}