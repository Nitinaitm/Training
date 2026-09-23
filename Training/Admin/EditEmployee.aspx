<%@ Page Title="Edit Employee" Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head runat="server">
<meta charset="utf-8" />
<meta name="viewport" content="width=device-width, initial-scale=1" />
<title>Edit Employee</title>
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>
<style>
body{background:#f5f5f5;margin:0}.edit-card{background:#fff;border-radius:12px;box-shadow:0 0 10px #d9d9d9;padding:25px;margin:20px auto;max-width:1100px}.heading{font-size:26px;font-weight:700;color:#198754;margin-bottom:20px}.readonly{background:#eef3f8!important}.select2-container{width:100%!important}.select2-selection--single{height:38px!important;padding-top:4px;border:1px solid #ced4da!important}
</style>
<script>
function LoadSearchableDropdown(){
    $('[id*=ddlCompany]').select2({placeholder:'Search Company',allowClear:true,width:'100%'});
    $('[id*=ddlDesignation]').select2({placeholder:'Search Designation',allowClear:true,width:'100%'});
    $('[id*=ddlPostingPlace]').select2({placeholder:'Search HRMS Posting Place',allowClear:true,width:'100%'});
    $('[id*=ddlPostingDetailPlace]').select2({placeholder:'Search Posting Place',allowClear:true,width:'100%'});
    $('[id*=ddlPostingDepartment]').select2({placeholder:'Search Department / Office / Cell',allowClear:true,width:'100%'});
    $('[id*=ddlAreaBoardZone]').select2({placeholder:'Search Area Board / Zone',allowClear:true,width:'100%'});
    $('[id*=ddlCircle]').select2({placeholder:'Search Circle',allowClear:true,width:'100%'});
    $('[id*=ddlDivision]').select2({placeholder:'Search Division',allowClear:true,width:'100%'});
    $('[id*=ddlSubdivision]').select2({placeholder:'Search Subdivision',allowClear:true,width:'100%'});
    $('[id*=ddlSection]').select2({placeholder:'Search Section',allowClear:true,width:'100%'});
}
$(document).ready(function(){LoadSearchableDropdown();});
</script>
</head>
<body>
<form id="form1" runat="server">
<div class="container-fluid"><div class="edit-card"><div class="heading">Edit Employee</div>
<asp:HiddenField ID="hfEmpID" runat="server"/>
<div class="row">
<div class="col-md-6 mb-3"><label>Employee ID</label><asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control readonly" ReadOnly="true"/></div>
<div class="col-md-6 mb-3"><label>Employee Name</label><asp:TextBox ID="txtEmpName" runat="server" CssClass="form-control readonly" ReadOnly="true"/></div>
<div class="col-md-6 mb-3"><label>Mobile No</label><asp:TextBox ID="txtMobile" runat="server" CssClass="form-control" MaxLength="10"/></div>
<div class="col-md-6 mb-3"><label>Email ID</label><asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"/></div>
<div class="col-md-4 mb-3"><label>Company</label><asp:DropDownList ID="ddlCompany" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"/></div>
<div class="col-md-4 mb-3"><label>Designation</label><asp:DropDownList ID="ddlDesignation" runat="server" CssClass="form-select"/></div>
<div class="col-md-4 mb-3"><label>Posting Place (HRMS)</label><asp:DropDownList ID="ddlPostingPlace" runat="server" CssClass="form-select"/></div>

<div class="col-12 mt-3"><div class="border rounded p-3"><h5 class="text-primary mb-3">Posting Details</h5><div class="row">
<div class="col-md-4 mb-3"><label>Posting Place</label><asp:DropDownList ID="ddlPostingDetailPlace" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPostingDetailPlace_SelectedIndexChanged"/></div>
<div class="col-md-8 mb-3" id="divPostingDepartment" runat="server"><label>Department / Office / Cell</label><asp:DropDownList ID="ddlPostingDepartment" runat="server" CssClass="form-select"/></div>
<div class="col-md-4 mb-3" id="divAreaBoardZone" runat="server"><label>Area Board / Zone</label><asp:DropDownList ID="ddlAreaBoardZone" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlAreaBoardZone_SelectedIndexChanged"/></div>
<div class="col-md-4 mb-3" id="divCircle" runat="server"><label>Circle</label><asp:DropDownList ID="ddlCircle" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCircle_SelectedIndexChanged"/></div>
<div class="col-md-4 mb-3" id="divDivision" runat="server"><label>Division</label><asp:DropDownList ID="ddlDivision" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDivision_SelectedIndexChanged"/></div>
<div class="col-md-4 mb-3" id="divSubdivision" runat="server"><label>Subdivision</label><asp:DropDownList ID="ddlSubdivision" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSubdivision_SelectedIndexChanged"/></div>
<div class="col-md-4 mb-3" id="divSection" runat="server"><label>Section</label><asp:DropDownList ID="ddlSection" runat="server" CssClass="form-select"/></div>
</div></div></div>

<div class="col-12 mt-3"><asp:Button ID="btnUpdate" runat="server" Text="Update Employee" CssClass="btn btn-success" OnClick="btnUpdate_Click"/> <button type="button" class="btn btn-secondary" onclick="window.parent.location.reload();">Close</button></div>
<div class="col-12 mt-3"><asp:Label ID="lblMessage" runat="server" Font-Bold="true"/></div>
</div></div></div>
</form>
</body>
</html>

<script runat="server">
protected void Page_Load(object sender,System.EventArgs e)
{
    if(!string.Equals(System.Convert.ToString(Session["Role"]),"Admin",StringComparison.OrdinalIgnoreCase)&&!string.Equals(System.Convert.ToString(Session["Role"]),"SuperAdmin",StringComparison.OrdinalIgnoreCase)&&!string.Equals(System.Convert.ToString(Session["Role"]),"Nodal",StringComparison.OrdinalIgnoreCase)){Response.Redirect("~/Default.aspx");return;}
    if(!IsPostBack) LoadEmployee(Request.QueryString["EmpID"]);
}
private clsDataAccess DB(){return new clsDataAccess();}
private void LoadEmployee(string empID)
{
    if(string.IsNullOrWhiteSpace(empID)){lblMessage.Text="Invalid Employee ID.";return;}
    System.Data.DataTable dt=DB().GetDataTable("SELECT TOP 1 EmpID,EmpName,MobileNo,EmailId,EmpCompany,EmpDesignation,EmpPostingPlace FROM EmpBasicMaster WHERE EmpID=@EmpID",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@EmpID",empID)});
    if(dt.Rows.Count==0){lblMessage.Text="Employee not found.";return;}
    System.Data.DataRow r=dt.Rows[0];
    hfEmpID.Value=System.Convert.ToString(r["EmpID"]);txtEmpID.Text=System.Convert.ToString(r["EmpID"]);txtEmpName.Text=System.Convert.ToString(r["EmpName"]);txtMobile.Text=System.Convert.ToString(r["MobileNo"]);txtEmail.Text=System.Convert.ToString(r["EmailId"]);
    BindCompany(System.Convert.ToString(r["EmpCompany"]));
    BindDesignation(System.Convert.ToString(r["EmpDesignation"]));
    BindHRMSPostingPlace(System.Convert.ToString(r["EmpPostingPlace"]));
    LoadPostingDetails();
}
private void BindCompany(string selected)
{
    System.Data.DataTable dt=DB().GetDataTable("SELECT ID,CompanyName,CompanyAlias FROM CompanyMaster ORDER BY CompanyName");
    ddlCompany.Items.Clear();ddlCompany.Items.Add(new System.Web.UI.WebControls.ListItem("Select Company",""));
    foreach(System.Data.DataRow r in dt.Rows){string name=System.Convert.ToString(r["CompanyName"]);string alias=System.Convert.ToString(r["CompanyAlias"]);ddlCompany.Items.Add(new System.Web.UI.WebControls.ListItem(string.IsNullOrWhiteSpace(alias)?name:name+" ("+alias+")",name));}
    if(ddlCompany.Items.FindByValue(selected)!=null)ddlCompany.SelectedValue=selected;
    else
    {
        object name=DB().ExecuteScalar("SELECT TOP 1 CompanyName FROM CompanyMaster WHERE CompanyAlias=@Company",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@Company",selected)});
        if(name!=null&&ddlCompany.Items.FindByValue(System.Convert.ToString(name))!=null)ddlCompany.SelectedValue=System.Convert.ToString(name);
    }
}
private void BindDesignation(string selected){BindList(ddlDesignation,"SELECT DISTINCT EmpDesignation FROM EmpBasicMaster WHERE ISNULL(EmpDesignation,'')<>'' ORDER BY EmpDesignation","EmpDesignation","Select Designation",selected);}
private void BindHRMSPostingPlace(string selected){BindList(ddlPostingPlace,"SELECT DISTINCT EmpPostingPlace FROM EmpBasicMaster WHERE ISNULL(EmpPostingPlace,'')<>'' ORDER BY EmpPostingPlace","EmpPostingPlace","Select HRMS Posting Place",selected);}
private void BindList(System.Web.UI.WebControls.DropDownList ddl,string sql,string field,string first,string selected)
{
    System.Data.DataTable dt=DB().GetDataTable(sql);ddl.Items.Clear();ddl.Items.Add(new System.Web.UI.WebControls.ListItem(first,""));
    foreach(System.Data.DataRow r in dt.Rows)ddl.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r[field]),System.Convert.ToString(r[field])));
    if(ddl.Items.FindByValue(selected)!=null)ddl.SelectedValue=selected;
}
private int CompanyID(){object v=DB().ExecuteScalar("SELECT TOP 1 ID FROM CompanyMaster WHERE CompanyName=@Company OR CompanyAlias=@Company",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@Company",ddlCompany.SelectedValue)});int id;return v!=null&&int.TryParse(System.Convert.ToString(v),out id)?id:0;}
private bool HqOnly(){return ddlCompany.SelectedValue.Equals("BSPHCL",StringComparison.OrdinalIgnoreCase)||ddlCompany.SelectedValue.Equals("BSPGCL",StringComparison.OrdinalIgnoreCase);}
private void ClearList(System.Web.UI.WebControls.DropDownList ddl,string first){ddl.Items.Clear();ddl.Items.Add(new System.Web.UI.WebControls.ListItem(first,""));}
private void BindPostingPlaceOptions(string selected)
{
    ddlPostingDetailPlace.Items.Clear();ddlPostingDetailPlace.Items.Add(new System.Web.UI.WebControls.ListItem("Select Posting Place",""));ddlPostingDetailPlace.Items.Add(new System.Web.UI.WebControls.ListItem("HQ","HQ"));
    if(!HqOnly())ddlPostingDetailPlace.Items.Add(new System.Web.UI.WebControls.ListItem("Field Office","Field"));
    if(!string.IsNullOrWhiteSpace(selected)&&ddlPostingDetailPlace.Items.FindByValue(selected)!=null)ddlPostingDetailPlace.SelectedValue=selected;
    if(HqOnly()&&string.IsNullOrWhiteSpace(ddlPostingDetailPlace.SelectedValue))ddlPostingDetailPlace.SelectedValue="HQ";
}
private void BindDepartment(string selected)
{
    ClearList(ddlPostingDepartment,"Select Department / Office / Cell");
    if(string.IsNullOrWhiteSpace(ddlCompany.SelectedValue))return;
    System.Data.DataTable dt=DB().GetDataTable("SELECT DISTINCT EPD.EmpPostingDepartment FROM EmpPostingDetails EPD INNER JOIN EmpBasicMaster EBM ON EBM.EmpID=EPD.EmpID WHERE EBM.EmpCompany=@Company AND ISNULL(EPD.EmpPostingDepartment,'')<>'' ORDER BY EPD.EmpPostingDepartment",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@Company",ddlCompany.SelectedValue)});
    foreach(System.Data.DataRow r in dt.Rows)ddlPostingDepartment.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r["EmpPostingDepartment"]),System.Convert.ToString(r["EmpPostingDepartment"])));
    if(!string.IsNullOrWhiteSpace(selected)&&ddlPostingDepartment.Items.FindByValue(selected)!=null)ddlPostingDepartment.SelectedValue=selected;
}
private void BindZone(string selected)
{
    ClearList(ddlAreaBoardZone,"Select Area Board / Zone");int cid=CompanyID();if(cid<=0)return;
    System.Data.DataTable dt=DB().GetDataTable("SELECT ZoneID,ZoneName FROM ZoneMaster WHERE CompanyID=@CompanyID ORDER BY ZoneName",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@CompanyID",cid)});
    foreach(System.Data.DataRow r in dt.Rows)ddlAreaBoardZone.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r["ZoneName"]),System.Convert.ToString(r["ZoneID"])));
    SelectByText(ddlAreaBoardZone,selected);
}
private void BindCircle(string selected)
{
    ClearList(ddlCircle,"Select Circle");int cid=CompanyID(),zid; if(cid<=0||!int.TryParse(ddlAreaBoardZone.SelectedValue,out zid))return;
    System.Data.DataTable dt=DB().GetDataTable("SELECT CircleID,CircleName FROM CircleMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID ORDER BY CircleName",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlParameter("@CompanyID",cid),new System.Data.SqlParameter("@ZoneID",zid)});
    foreach(System.Data.DataRow r in dt.Rows)ddlCircle.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r["CircleName"]),System.Convert.ToString(r["CircleID"])));
    SelectByText(ddlCircle,selected);
}
private void BindDivision(string selected)
{
    ClearList(ddlDivision,"Select Division");int cid=CompanyID(),zid,cirid;if(cid<=0||!int.TryParse(ddlAreaBoardZone.SelectedValue,out zid)||!int.TryParse(ddlCircle.SelectedValue,out cirid))return;
    System.Data.DataTable dt=DB().GetDataTable("SELECT DivisionID,DivisionName FROM DivisionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID ORDER BY DivisionName",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlParameter("@CompanyID",cid),new System.Data.SqlParameter("@ZoneID",zid),new System.Data.SqlParameter("@CircleID",cirid)});
    foreach(System.Data.DataRow r in dt.Rows)ddlDivision.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r["DivisionName"]),System.Convert.ToString(r["DivisionID"])));
    SelectByText(ddlDivision,selected);
}
private void BindSubdivision(string selected)
{
    ClearList(ddlSubdivision,"Select Subdivision");int cid=CompanyID(),zid,cirid,divid;if(cid<=0||!int.TryParse(ddlAreaBoardZone.SelectedValue,out zid)||!int.TryParse(ddlCircle.SelectedValue,out cirid)||!int.TryParse(ddlDivision.SelectedValue,out divid))return;
    System.Data.DataTable dt=DB().GetDataTable("SELECT SubdivisionID,SubdivisionName FROM SubdivisionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID AND DivisionID=@DivisionID ORDER BY SubdivisionName",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@CompanyID",cid),new System.Data.SqlParameter("@ZoneID",zid),new System.Data.SqlParameter("@CircleID",cirid),new System.Data.SqlParameter("@DivisionID",divid)});
    foreach(System.Data.DataRow r in dt.Rows)ddlSubdivision.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r["SubdivisionName"]),System.Convert.ToString(r["SubdivisionID"])));
    SelectByText(ddlSubdivision,selected);
}
private void BindSection(string selected)
{
    ClearList(ddlSection,"Select Section");int cid=CompanyID(),zid,cirid,divid,subid;if(cid<=0||!int.TryParse(ddlAreaBoardZone.SelectedValue,out zid)||!int.TryParse(ddlCircle.SelectedValue,out cirid)||!int.TryParse(ddlDivision.SelectedValue,out divid)||!int.TryParse(ddlSubdivision.SelectedValue,out subid))return;
    System.Data.DataTable dt=DB().GetDataTable("SELECT SectionID,SectionName FROM SectionMaster WHERE CompanyID=@CompanyID AND ZoneID=@ZoneID AND CircleID=@CircleID AND DivisionID=@DivisionID AND SubdivisionID=@SubdivisionID ORDER BY SectionName",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@CompanyID",cid),new System.Data.SqlParameter("@ZoneID",zid),new System.Data.SqlParameter("@CircleID",cirid),new System.Data.SqlParameter("@DivisionID",divid),new System.Data.SqlParameter("@SubdivisionID",subid)});
    foreach(System.Data.DataRow r in dt.Rows)ddlSection.Items.Add(new System.Web.UI.WebControls.ListItem(System.Convert.ToString(r["SectionName"]),System.Convert.ToString(r["SectionID"])));
    SelectByText(ddlSection,selected);
}
private void SelectByText(System.Web.UI.WebControls.DropDownList ddl,string text){if(!string.IsNullOrWhiteSpace(text)){foreach(System.Web.UI.WebControls.ListItem item in ddl.Items){if(string.Equals(item.Text,text,StringComparison.OrdinalIgnoreCase)){ddl.SelectedValue=item.Value;return;}}}}
private void LoadPostingDetails()
{
    System.Data.DataTable dt=DB().GetDataTable("SELECT TOP 1 EmpPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,Subdivision,Section FROM EmpPostingDetails WHERE EmpID=@EmpID ORDER BY ID DESC",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@EmpID",hfEmpID.Value)});
    string place=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["EmpPostingPlace"]);
    string dept=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["EmpPostingDepartment"]);
    string zone=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["AreaBoardZone"]);
    string circle=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["Circle"]);
    string division=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["Division"]);
    string subdivision=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["Subdivision"]);
    string section=dt.Rows.Count==0?"":System.Convert.ToString(dt.Rows[0]["Section"]);
    BindPostingPlaceOptions(place);BindDepartment(dept);BindZone(zone);BindCircle(circle);BindDivision(division);BindSubdivision(subdivision);BindSection(section);SetMode();
}
private void SetMode()
{
    bool hq=ddlPostingDetailPlace.SelectedValue=="HQ";bool field=ddlPostingDetailPlace.SelectedValue=="Field";
    if(HqOnly()){if(ddlPostingDetailPlace.Items.FindByValue("Field")!=null)ddlPostingDetailPlace.Items.Remove(ddlPostingDetailPlace.Items.FindByValue("Field"));if(string.IsNullOrWhiteSpace(ddlPostingDetailPlace.SelectedValue)){ddlPostingDetailPlace.SelectedValue="HQ";hq=true;}}
    divPostingDepartment.Visible=hq;divAreaBoardZone.Visible=field;divCircle.Visible=field;divDivision.Visible=field;divSubdivision.Visible=field;divSection.Visible=field;
}
protected void ddlCompany_SelectedIndexChanged(object sender,System.EventArgs e){BindPostingPlaceOptions("");BindDepartment("");BindZone("");ClearList(ddlCircle,"Select Circle");ClearList(ddlDivision,"Select Division");ClearList(ddlSubdivision,"Select Subdivision");ClearList(ddlSection,"Select Section");SetMode();LoadSelect2();}
protected void ddlPostingDetailPlace_SelectedIndexChanged(object sender,System.EventArgs e){SetMode();LoadSelect2();}
protected void ddlAreaBoardZone_SelectedIndexChanged(object sender,System.EventArgs e){BindCircle("");ClearList(ddlDivision,"Select Division");ClearList(ddlSubdivision,"Select Subdivision");ClearList(ddlSection,"Select Section");LoadSelect2();}
protected void ddlCircle_SelectedIndexChanged(object sender,System.EventArgs e){BindDivision("");ClearList(ddlSubdivision,"Select Subdivision");ClearList(ddlSection,"Select Section");LoadSelect2();}
protected void ddlDivision_SelectedIndexChanged(object sender,System.EventArgs e){BindSubdivision("");ClearList(ddlSection,"Select Section");LoadSelect2();}
protected void ddlSubdivision_SelectedIndexChanged(object sender,System.EventArgs e){BindSection("");LoadSelect2();}
private void LoadSelect2(){ClientScript.RegisterStartupScript(GetType(),"LoadSearchableDropdown","LoadSearchableDropdown();",true);}
protected void btnUpdate_Click(object sender,System.EventArgs e)
{
    if(string.IsNullOrWhiteSpace(hfEmpID.Value)){lblMessage.Text="Invalid Employee ID.";return;}
    if(string.IsNullOrWhiteSpace(txtMobile.Text)||!System.Text.RegularExpressions.Regex.IsMatch(txtMobile.Text.Trim(),"^[0-9]{10}$")){lblMessage.Text="Enter valid 10 digit mobile number.";return;}
    if(string.IsNullOrWhiteSpace(ddlPostingDetailPlace.SelectedValue)){lblMessage.Text="Select Posting Details Posting Place.";LoadSelect2();return;}
    if(HqOnly()&&ddlPostingDetailPlace.SelectedValue!="HQ"){lblMessage.Text="BSPHCL and BSPGCL allow HQ posting only.";return;}
    if(ddlPostingDetailPlace.SelectedValue=="HQ"&&string.IsNullOrWhiteSpace(ddlPostingDepartment.SelectedValue)){lblMessage.Text="Select Department / Office / Cell.";LoadSelect2();return;}
    if(ddlPostingDetailPlace.SelectedValue=="Field"&&(string.IsNullOrWhiteSpace(ddlAreaBoardZone.SelectedValue)||string.IsNullOrWhiteSpace(ddlCircle.SelectedValue)||string.IsNullOrWhiteSpace(ddlDivision.SelectedValue)||string.IsNullOrWhiteSpace(ddlSubdivision.SelectedValue)||string.IsNullOrWhiteSpace(ddlSection.SelectedValue))){lblMessage.Text="Select complete Field Office hierarchy.";LoadSelect2();return;}
    clsDataAccess db=DB();
    try
    {
        db.BeginTransaction();
        db.ExecuteSql("UPDATE EmpBasicMaster SET MobileNo=@MobileNo,EmailId=@EmailId,EmpCompany=@EmpCompany,EmpDesignation=@EmpDesignation,EmpPostingPlace=@EmpPostingPlace WHERE EmpID=@EmpID",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@MobileNo",txtMobile.Text.Trim()),new System.Data.SqlClient.SqlParameter("@EmailId",txtEmail.Text.Trim()),new System.Data.SqlClient.SqlParameter("@EmpCompany",ddlCompany.SelectedValue),new System.Data.SqlClient.SqlParameter("@EmpDesignation",ddlDesignation.SelectedValue),new System.Data.SqlClient.SqlParameter("@EmpPostingPlace",ddlPostingPlace.SelectedValue),new System.Data.SqlClient.SqlParameter("@EmpID",hfEmpID.Value)},db.Transaction);
        string dept=ddlPostingDetailPlace.SelectedValue=="HQ"?ddlPostingDepartment.SelectedValue:"";string zone=ddlPostingDetailPlace.SelectedValue=="Field"?ddlAreaBoardZone.SelectedItem.Text:"";string circle=ddlPostingDetailPlace.SelectedValue=="Field"?ddlCircle.SelectedItem.Text:"";string division=ddlPostingDetailPlace.SelectedValue=="Field"?ddlDivision.SelectedItem.Text:"";string subdivision=ddlPostingDetailPlace.SelectedValue=="Field"?ddlSubdivision.SelectedItem.Text:"";string section=ddlPostingDetailPlace.SelectedValue=="Field"?ddlSection.SelectedItem.Text:"";
        int count=System.Convert.ToInt32(db.ExecuteScalar("SELECT COUNT(*) FROM EmpPostingDetails WHERE EmpID=@EmpID",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@EmpID",hfEmpID.Value)},db.Transaction));
        if(count>0)
            db.ExecuteSql("UPDATE EmpPostingDetails SET EmpPostingPlace=@Place,EmpPostingDepartment=@Dept,AreaBoardZone=@Zone,Circle=@Circle,Division=@Division,Subdivision=@Subdivision,Section=@Section,CreatedOn=GETDATE(),CreatedBy=@CreatedBy WHERE ID=(SELECT TOP 1 ID FROM EmpPostingDetails WHERE EmpID=@EmpID ORDER BY ID DESC)",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@Place",ddlPostingDetailPlace.SelectedValue),new System.Data.SqlClient.SqlParameter("@Dept",dept),new System.Data.SqlClient.SqlParameter("@Zone",zone),new System.Data.SqlClient.SqlParameter("@Circle",circle),new System.Data.SqlClient.SqlParameter("@Division",division),new System.Data.SqlClient.SqlParameter("@Subdivision",subdivision),new System.Data.SqlClient.SqlParameter("@Section",section),new System.Data.SqlClient.SqlParameter("@CreatedBy","Admin"),new System.Data.SqlClient.SqlParameter("@EmpID",hfEmpID.Value)},db.Transaction);
        else
            db.ExecuteSql("INSERT INTO EmpPostingDetails(EmpID,EmpPostingPlace,EmpPostingDepartment,AreaBoardZone,Circle,Division,Subdivision,Section,EmpOnDeputation,CreatedOn,CreatedBy,AssessmentYear) VALUES(@EmpID,@Place,@Dept,@Zone,@Circle,@Division,@Subdivision,@Section,@Deputation,GETDATE(),@CreatedBy,@AssessmentYear)",new System.Data.SqlClient.SqlParameter[]{new System.Data.SqlClient.SqlParameter("@EmpID",hfEmpID.Value),new System.Data.SqlClient.SqlParameter("@Place",ddlPostingDetailPlace.SelectedValue),new System.Data.SqlClient.SqlParameter("@Dept",dept),new System.Data.SqlClient.SqlParameter("@Zone",zone),new System.Data.SqlClient.SqlParameter("@Circle",circle),new System.Data.SqlClient.SqlParameter("@Division",division),new System.Data.SqlClient.SqlParameter("@Subdivision",subdivision),new System.Data.SqlClient.SqlParameter("@Section",section),new System.Data.SqlClient.SqlParameter("@Deputation","NO"),new System.Data.SqlClient.SqlParameter("@CreatedBy","Admin"),new System.Data.SqlClient.SqlParameter("@AssessmentYear",AssessmentYear())},db.Transaction);
        db.Commit();lblMessage.ForeColor=System.Drawing.Color.Green;lblMessage.Text="Employee and Posting Details updated successfully.";LoadSelect2();
    }
    catch(System.Exception ex){try{db.Rollback();}catch{}lblMessage.ForeColor=System.Drawing.Color.Red;lblMessage.Text=ex.Message;}
}
private string AssessmentYear(){System.DateTime n=System.DateTime.Now;int y=n.Month>=4?n.Year:n.Year-1;return y+"-"+(y+1).ToString().Substring(2);}
</script>