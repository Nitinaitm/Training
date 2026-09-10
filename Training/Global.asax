<%@ Application Language="C#" %>
<script runat="server">
    protected void Application_AcquireRequestState(object sender, EventArgs e)
    {
        HttpContext context = HttpContext.Current;
        if (context == null || context.Session == null)
            return;

        string path = context.Request.AppRelativeCurrentExecutionFilePath ?? "";
        if (!path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
            return;

        string role = Convert.ToString(context.Session["Role"]);
        bool loggedIn = false;
        string redirect = "~/Default.aspx";

        if (path.StartsWith("~/Admin/", StringComparison.OrdinalIgnoreCase))
        {
            loggedIn = context.Session["UserID"] != null &&
                       (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
                        role.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
                        role.Equals("Nodal", StringComparison.OrdinalIgnoreCase));
        }
        else if (path.StartsWith("~/Trainer/", StringComparison.OrdinalIgnoreCase))
        {
            loggedIn = context.Session["TrainerID"] != null &&
                       role.Equals("Trainer", StringComparison.OrdinalIgnoreCase);
        }
        else if (path.StartsWith("~/Trainee/", StringComparison.OrdinalIgnoreCase))
        {
            loggedIn = context.Session["EmpID"] != null &&
                       role.Equals("Trainee", StringComparison.OrdinalIgnoreCase);
        }
        else
        {
            return;
        }

        if (!loggedIn)
        {
            context.Response.Redirect(redirect, false);
            context.ApplicationInstance.CompleteRequest();
        }
    }
</script>