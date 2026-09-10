using System;
using System.Web.UI;

namespace Training
{
    public partial class AdminMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);
            if (string.IsNullOrWhiteSpace(role) ||
                !(role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                  role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
                  role.Equals("Nodal", StringComparison.OrdinalIgnoreCase)))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }
        }
    }
}