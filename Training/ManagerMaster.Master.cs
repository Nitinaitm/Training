using System;
using System.Web.UI;

namespace Training
{
    public partial class ManagerMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Manager" || Session["ManagerID"] == null)
            {
                Response.Redirect("~/Default.aspx");
            }
        }
    }
}