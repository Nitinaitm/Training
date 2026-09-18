using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class Notifications : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/Default.aspx");
            if (!IsPostBack) BindGrid();
        }

        private string TrainerID => Session["TrainerID"].ToString();

        private void BindGrid()
        {
            string query = "SELECT NotificationID, Message, IsRead, CreatedOn FROM Notification WHERE TrainerID=@TrainerID ORDER BY CreatedOn DESC";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            DataTable dt = obj.GetDataTable(query, param);
            gvNotifications.DataSource = dt;
            gvNotifications.DataBind();
        }

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            string query = "UPDATE Notification SET IsRead=1 WHERE TrainerID=@TrainerID";
            SqlParameter[] param = new SqlParameter[] { new SqlParameter("@TrainerID", TrainerID) };
            obj.ExecuteSql(query, param);
            BindGrid();
        }
    }
}