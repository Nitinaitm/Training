<%@ Page Title="Session Trainees" Language="C#" MasterPageFile="~/TrainerMaster.Master" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>.trainee-page{padding:20px}.trainee-title{font-size:24px;font-weight:600;margin-bottom:18px}.trainee-grid th{background:#198754;color:#fff;text-align:center}.trainee-grid td{vertical-align:middle}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid trainee-page">
    <div class="trainee-title">List of Trainees</div>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger"></asp:Label>
    <asp:GridView ID="gvTrainees" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover trainee-grid" EmptyDataText="No trainee assigned to this training.">
        <Columns>
            <asp:TemplateField HeaderText="Sl No"><ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate><ItemStyle Width="60px" HorizontalAlign="Center" /></asp:TemplateField>
            <asp:BoundField DataField="TraineeID" HeaderText="Trainee ID" />
            <asp:BoundField DataField="TraineeName" HeaderText="Trainee Name" />
            <asp:BoundField DataField="Designation" HeaderText="Designation" />
            <asp:BoundField DataField="Organization" HeaderText="Organization" />
            <asp:BoundField DataField="MobileNo" HeaderText="Mobile No." />
            <asp:BoundField DataField="EmailID" HeaderText="Email" />
        </Columns>
    </asp:GridView>
</div>
</asp:Content>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["TrainerID"] == null || !string.Equals(Convert.ToString(Session["Role"]), "Trainer", StringComparison.OrdinalIgnoreCase))
        {
            Response.Redirect("~/Default.aspx");
            return;
        }

        if (!IsPostBack)
        {
            BindTrainees();
        }
    }

    private void BindTrainees()
    {
        string sessionID = Request.QueryString["SessionID"];
        if (string.IsNullOrWhiteSpace(sessionID))
        {
            lblMessage.Text = "Invalid session.";
            return;
        }

        clsDataAccess obj = new clsDataAccess();
        string trainerID = Session["TrainerID"].ToString();

        string query = @"
SELECT DISTINCT
    TA.EmpID AS TraineeID,
    COALESCE(E.EmpName, TE.TraineeName, TA.EmpID) AS TraineeName,
    COALESCE(E.EmpDesignation, TE.Designation, '') AS Designation,
    COALESCE(E.EmpCompany, TE.OrganizationName, '') AS Organization,
    COALESCE(E.MobileNo, TE.MobileNo, '') AS MobileNo,
    COALESCE(E.EmailId, TE.EmailID, '') AS EmailID
FROM SessionMaster SM
INNER JOIN TrainingAssignment TA ON TA.TrainingID = SM.TrainingID
LEFT JOIN EmpBasicMaster E ON E.EmpID = TA.EmpID
LEFT JOIN TraineeMasterExternal TE ON TE.EmpIDExternal = TA.EmpID OR TE.TraineeID = TA.EmpID
WHERE SM.SessionID = @SessionID
  AND SM.TrainerID = @TrainerID
  AND ISNULL(TA.AssignmentStatus, 'Assigned') = 'Assigned'
ORDER BY TraineeName";

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@SessionID", sessionID),
            new SqlParameter("@TrainerID", trainerID)
        };

        DataTable dt = obj.GetDataTable(query, parameters);
        gvTrainees.DataSource = dt;
        gvTrainees.DataBind();
    }
</script>