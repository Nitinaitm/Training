using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Goal_UPSC
{
    public partial class Login : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String activepage = Request.RawUrl;
            if (activepage.Contains("Default.aspx"))
            {
                test6.Attributes.Add("class", "nav-item nav-link active");
            }
            else if (activepage.Contains("View_Photo.aspx"))
            {
                test5.Attributes.Add("class", "nav-item nav-link active");
            }
            //else if (activepage.Contains("Search_Topic.aspx"))
            //{
            //    test3.Attributes.Add("class", "nav-item nav-link active");
            //}
            //else if (activepage.Contains("Edit_Doc.aspx"))
            //{
            //    test4.Attributes.Add("class", "nav-item nav-link active");
            //}
            //else if (activepage.Contains("Delete_Doc.aspx"))
            //{
            //    test5.Attributes.Add("class", "nav-item nav-link active");

            //}
            //else if (activepage.Contains("Add_Photo.aspx") || activepage.Contains("View_Photo.aspx") || activepage.Contains("Delete_Photo.aspx"))
            //{
            //    test7.Attributes.Add("class", "nav-link dropdown-toggle active");

            //}

            else
            {
                test7.Attributes.Add("class", "nav-item nav-link active");
            }
        }
    }
}