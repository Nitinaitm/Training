<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master"
AutoEventWireup="true"
CodeBehind="TrainingAttendance1.aspx.cs"
Inherits="Training.Admin.TrainingAttendance1"
ClientIDMode="Static" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet"/>
<script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

<style>

.main-card{
background:#fff;
padding:25px;
border-radius:12px;
box-shadow:0 0 10px #ddd;
margin-top:20px;
margin-bottom:20px;
}

.page-heading{
font-size:28px;
font-weight:bold;
color:darkcyan;
margin-bottom:20px;
}

.select2-container{
width:100%!important;
}

.form-select{
height:38px!important;
}

.btn-switch{
min-width:160px;
margin-bottom:10px;
}

.validation{
color:red;
font-size:13px;
}

</style>

</asp:Content>



<asp:Content ID="Content2"
ContentPlaceHolderID="ContentPlaceHolder1"
runat="server">

<div class="container-fluid">

<div class="main-card">

<div class="page-heading">

Training Attendance

</div>


<div class="mb-4">

<asp:Button ID="btnManualTab"
runat="server"
Text="Manual Update"
CssClass="btn btn-primary btn-switch"
OnClick="btnManualTab_Click"/>

<asp:Button ID="btnBulkTab"
runat="server"
Text="Bulk Update"
CssClass="btn btn-secondary btn-switch ms-md-2"
OnClick="btnBulkTab_Click"/>

</div>



<asp:MultiView ID="mvAttendance"
runat="server"
ActiveViewIndex="0">

<!-- MANUAL VIEW -->

<asp:View ID="vwManual"
runat="server">

<div class="row">

<div class="col-12 col-md-6 col-lg-6 mb-3">

<label class="form-label">

Training

</label>

<asp:DropDownList ID="ddlTraining"
runat="server"
CssClass="form-select"
AutoPostBack="true"
OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged">
</asp:DropDownList>

</div>
<div class="col-12 col-md-6 col-lg-6 mb-3">

    <label class="form-label">Search Trainee</label>

    <div class="input-group">

        <asp:TextBox ID="txtSearch"
            runat="server"
            CssClass="form-control"
            placeholder="EmpID / Name / Designation">
        </asp:TextBox>

        <asp:Button ID="btnSearch"
            runat="server"
            Text="Search"
            CssClass="btn btn-primary"
            OnClick="btnSearch_Click" />

    </div>

</div>

<div class="col-12 mt-3">

<div class="table-responsive">

<asp:GridView ID="gvAttendance"
runat="server"
AutoGenerateColumns="false"
CssClass="table table-bordered table-hover"
DataKeyNames="AssignmentID"
OnRowDataBound="gvAttendance_RowDataBound">

<Columns>
     <asp:TemplateField>
        <ItemTemplate>
             <%#Container.DataItemIndex+1 %>
        </ItemTemplate>
    </asp:TemplateField>
<asp:BoundField DataField="EmpID"
HeaderText="Emp ID"/>

<asp:BoundField DataField="EmpName"
HeaderText="Name"/>

<asp:BoundField DataField="EmpDesignation"
HeaderText="Designation"/>


<asp:TemplateField HeaderText="Attendance">

<ItemTemplate>

<asp:DropDownList ID="ddlStatus"
runat="server"
CssClass="form-select">

<asp:ListItem Value="">
Pending
</asp:ListItem>

<asp:ListItem Value="Yes">
Yes
</asp:ListItem>

<asp:ListItem Value="No">
No
</asp:ListItem>

</asp:DropDownList>

</ItemTemplate>

</asp:TemplateField>

</Columns>

</asp:GridView>

</div>

</div>


<div class="col-12 mt-3">

<asp:Button ID="btnSave"
runat="server"
Text="Update Attendance"
CssClass="btn btn-success"
OnClick="btnSave_Click"/>

</div>


<div class="col-12 mt-3">

<asp:Label ID="lblMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>


</div>

</asp:View>




<!-- BULK VIEW -->

<asp:View ID="vwBulk"
runat="server">

<div class="row">

<div class="col-12 col-md-6 col-lg-4 mb-3">

<label class="form-label">

Training

</label>

<asp:DropDownList ID="ddlBulkTraining"
runat="server"
CssClass="form-select"
AutoPostBack="true"
OnSelectedIndexChanged="ddlBulkTraining_SelectedIndexChanged">
</asp:DropDownList>

</div>



<div class="col-12 col-md-6 col-lg-4 mb-3">

<label class="form-label">

Excel File

</label>

<asp:FileUpload ID="fuAttendance"
runat="server"
CssClass="form-control"/>

</div>



<div class="col-12 col-md-6 col-lg-4 mb-3 d-flex align-items-end">

<asp:Button ID="btnBulkAttendance"
runat="server"
Text="Upload Attendance"
CssClass="btn btn-primary w-100"
OnClick="btnBulkAttendance_Click"/>

</div>



<div class="col-12">

<div class="alert alert-info">

<b>Excel Format</b>

<br/><br/>

EmpID | TrainingAttended

<br/>

E14811 | Yes

<br/>

E14812 | No

<br/><br/>

Header row mandatory

</div>

</div>


<div class="col-12">

<asp:Label ID="lblBulkMessage"
runat="server"
Font-Bold="true">
</asp:Label>

</div>

</div>

</asp:View>

</asp:MultiView>

</div>

</div>


<script>

$(document).ready(function(){

$('#ddlTraining').select2({
width:'100%'
});

$('#ddlBulkTraining').select2({
width:'100%'
});

});

</script>

</asp:Content>