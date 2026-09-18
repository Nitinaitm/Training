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

            Page.ClientScript.RegisterClientScriptInclude("EmployeeGridEdit", ResolveUrl("~/Scripts/employee-edit-grid.js"));
            Page.ClientScript.RegisterStartupScript(GetType(), "Dashboard2Menu", "(function(){var s=document.getElementById('adminSidebar');if(!s||document.getElementById('dashboard2Menu'))return;var a=document.createElement('a');a.id='dashboard2Menu';a.href='Dashboard.aspx';a.className='menu-item';a.innerHTML='<i class=\"fas fa-tachometer-alt mr-2\"></i>Dashboard 2';s.insertBefore(a,s.firstElementChild);})();", true);
        }
    }
}
