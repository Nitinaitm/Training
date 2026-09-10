<%@ Page Title="Edit Employee" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<style>.edit-card{background:#fff;border-radius:12px;box-shadow:0 0 10px #d9d9d9;padding:25px;margin:20px auto;max-width:900px}.heading{font-size:26px;font-weight:700;color:#198754;margin-bottom:20px}.readonly{background:#eef3f8!important}</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="container-fluid"><div class="edit-card"><div class="heading">Edit Employee</div>
<asp:HiddenField ID="hfEmpID" runat="server"/><div class="row">
<div class="col-md-6 mb-3"><label>Employee ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control readonly" ReadOnly="true"/></div>
<div class="col-md-6 mb-3"><label>Employee Name</label><asp:TextBox ID="txtEmpName" runat="server" CssClass="form-control readonly" ReadOnly="true"/></div>
<div class="col-md-6 mb-3"><label>Mobile No</label><asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" MaxLength="10"/></div>
<div class="col-md-6 mb-3"><label>Email ID</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"/></div>
<div class="col-md-4 mb-3"><label>Company</label><asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-select"/></div>
<div class="col-md-4 mb-3"><label>Designation</label><asp:DropDownList ID="ddlDesignation" runat="server" CssClass="form-select"/></div>
<div class="col-md-4 mb-3"><label>Posting Place</label><asp:DropDownList ID="ddlPostingPlace" runat="server" CssClass="form-select"/></div>
<div class="col-12 mt-2"><asp:Button ID="btnUpdate" runat="server" Text="Update Employee" CssClass="btn btn-success" OnClick="btnUpdate_Click"/><button type="button" class="btn btn-secondary" onclick="window.parent.location.reload();">Close</button></div>
<div class="col-12 mt-3"><asp:Label ID="lblMessage" runat="server" Font-Bold="true"/></div></div></div></div>
</asp:Content>
<script runat="server">
using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

protected void Page_Load(object sender, EventArgs e)
{
    if (!string.Equals(Convert.ToString(Session["Role"]), "Admin", StringComparison.OrdinalIgnoreCase) && !string.Equals(Convert.ToString(Session["Role"]), "SuperAdmin", StringComparison.OrdinalIgnoreCase) && !string.Equals(Convert.ToString(Session["Role"]), "Nodal", StringComparison.OrdinalIgnoreCase)) { Response.Redirect("~/Default.aspx"); return; }
    if (!IsPostBack) LoadEmployee(Request.QueryString["EmpID"]);
}
private void LoadEmployee(string empID)
{
    if (string.IsNullOrWhiteSpace(empID)) { lblMessage.Text="Invalid Employee ID."; return; }
    DataTable dt=new clsDataAccess().GetDataTable("SELECT TOP 1 EmpID,EmpName,MobileNo,EmailId,EmpCompany,EmpDesignation,EmpPostingPlace FROM EmpBasicMaster WHERE EmpID=@EmpID",new SqlParameter[]{new SqlParameter("@EmpID",empID)});
    if(dt.Rows.Count==0){lblMessage.Text="Employee not found.";return;}
    DataRow r=dt.Rows[0]; hfEmpID.Value=Convert.ToString(r["EmpID"]); txtEmpID.Text=Convert.ToString(r["EmpID"]); txtEmpName.Text=Convert.ToString(r["EmpName"]); txtMobile.Text=Convert.ToString(r["MobileNo"]); txtEmail.Text=Convert.ToString(r["EmailId"]);
    BindList(ddlCompany,"SELECT DISTINCT EmpCompany FROM EmpBasicMaster WHERE ISNULL(EmpCompany,'')<>'' ORDER BY EmpCompany","EmpCompany",Convert.ToString(r["EmpCompany"]));
    BindList(ddlDesignation,"SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>'' ORDER BY EmpDesignation","EmpDesignation",Convert.ToString(r["EmpDesignation"]));
    BindList(ddlPostingPlace,"SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>'' ORDER BY EmpPostingPlace","EmpPostingPlace",Convert.ToString(r["EmpPostingPlace"]));
}
private void BindList(DropDownList ddl,string sql,string field,string selected){DataTable dt=new clsDataAccess().GetDataTable(sql);ddl.Items.Clear();ddl.Items.Add(new ListItem("Select",""));foreach(DataRow r in dt.Rows)ddl.Items.Add(new ListItem(Convert.ToString(r[field]),Convert.ToString(r[field])));if(ddl.Items.FindByValue(selected)!=null)ddl.SelectedValue=selected;}
protected void btnUpdate_Click(object sender,EventArgs e)
{
    if(string.IsNullOrWhiteSpace(hfEmpID.Value)){lblMessage.Text="Invalid Employee ID.";return;}
    if(string.IsNullOrWhiteSpace(txtMobile.Text)||!System.Text.RegularExpressions.Regex.IsMatch(txtMobile.Text.Trim(),"^[0-9]{10}$")){lblMessage.Text="Enter valid 10 digit mobile number.";return;}
    string sql="UPDATE EmpBasicMaster SET MobileNo=@MobileNo,EmailId=@EmailId,EmpCompany=@EmpCompany,EmpDesignation=@EmpDesignation,EmpPostingPlace=@EmpPostingPlace WHERE EmpID=@EmpID";
    int rows=new clsDataAccess().ExecuteSql(sql,new SqlParameter[]{new SqlParameter("@MobileNo",txtMobile.Text.Trim()),new SqlParameter("@EmailId",txtEmail.Text.Trim()),new SqlParameter("@EmpCompany",ddlCompany.SelectedValue),new SqlParameter("@EmpDesignation",ddlDesignation.SelectedValue),new SqlParameter("@EmpPostingPlace",ddlPostingPlace.SelectedValue),new SqlParameter("@EmpID",hfEmpID.Value)});
    if(rows>0){lblMessage.ForeColor=System.Drawing.Color.Green;lblMessage.Text="Employee updated successfully.";string script="if(window.parent&&window.parent!==window){window.parent.location.reload();}";ClientScript.RegisterStartupScript(GetType(),"reloadParent",script,true);}else{lblMessage.ForeColor=System.Drawing.Color.Red;lblMessage.Text="No changes were saved.";}
}
</script>