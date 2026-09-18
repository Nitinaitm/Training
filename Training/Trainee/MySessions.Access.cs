using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MySessions
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            string message = Session["PreTrainingAccessMessage"] as string;
            if (!string.IsNullOrWhiteSpace(message))
            {
                Session.Remove("PreTrainingAccessMessage");
                string safe = message.Replace("\\", "\\\\").Replace("'", "\\'");
                ScriptManager.RegisterStartupScript(this, GetType(), "PreTrainingAccessMessage", "alert('" + safe + "');", true);
            }

            string postMessage = Session["PostTrainingAccessMessage"] as string;
            if (!string.IsNullOrWhiteSpace(postMessage))
            {
                Session.Remove("PostTrainingAccessMessage");
                string safe = postMessage.Replace("\\", "\\\\").Replace("'", "\\'");
                ScriptManager.RegisterStartupScript(this, GetType(), "PostTrainingAccessMessage", "alert('" + safe + "');", true);
            }}

    }
}
