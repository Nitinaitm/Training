using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String activepage = Request.RawUrl;

            //if (activepage.Contains("/Rules"))
            //{
            //    test2.Attributes.Add("class", "nav-item nav-link active");
            //}
            //else 
            if (activepage.Contains("/Contact"))
            {
                test2.Attributes.Add("class", "nav-item nav-link active");
            }



            else
            {
                test1.Attributes.Add("class", "nav-item nav-link active");
            }
        }
    }
}