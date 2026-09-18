using System;
using System.Web.UI;

namespace Training
{
    public partial class TraineeMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Convert.ToString(Session["Role"]);
            if (Session["EmpID"] == null || !role.Equals("Trainee", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect("~/Default.aspx");
                return;
            }

            try
            {
                Control lifecycle = LoadControl("~/BatchLifecycle.ascx");
                ContentPlaceHolder1.Controls.Add(lifecycle);
            }
            catch { }
        }
    }
}