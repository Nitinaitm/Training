<%@ Page Title="" Language="C#"
    MasterPageFile="~/SuperAdminMaster.Master"
    AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="TrainingReAssign.aspx.cs"
    Inherits="Training.SuperAdmin.TrainingReAssign" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

<style>

.page-card{
    background:#fff;
    padding:20px;
    border-radius:10px;
    box-shadow:0 2px 10px rgba(0,0,0,.1);
    margin-top:20px;
}

.page-title{
    font-size:28px;
    font-weight:600;
    color:#198754;
    margin-bottom:20px;
}

.gridview th{
    background:#198754;
    color:white;
    text-align:center;
}

.gridview td{
    vertical-align:middle;
}

.btn-reassign{
    background:#fd7e14;
    color:white;
    border:none;
    padding:5px 12px;
    border-radius:5px;
    text-decoration:none;
}

.btn-reassign:hover{
    color:white;
    background:#e76b00;
}

.reassign-panel{
    margin-top:20px;
    border:1px solid #ddd;
    border-radius:10px;
    overflow:hidden;
}

.panel-header{
    background:#ffc107;
    color:#000;
    padding:12px;
    font-weight:bold;
    font-size:18px;
}

.panel-body{
    padding:20px;
}

</style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="container-fluid">

    <div class="page-card">

        <div class="page-title">
            Training ReAssign
        </div>

        <div class="row">

            <div class="col-md-3">

                <label>Employee ID</label>

                <asp:TextBox
                    ID="txtEmpID"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

            <div class="col-md-2">

                <br />

                <asp:Button
                    ID="btnSearch"
                    runat="server"
                    Text="Search"
                    CssClass="btn btn-success"
                    OnClick="btnSearch_Click" />

            </div>

        </div>

        <br />

        <asp:GridView
            ID="gvTraining"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="table table-bordered table-striped gridview"
            OnRowCommand="gvTraining_RowCommand">

            <Columns>

                <asp:BoundField
                    DataField="TrainingID"
                    HeaderText="Training ID" />

                <asp:BoundField
                    DataField="TrainingType"
                    HeaderText="Training Type" />

                <asp:BoundField
                    DataField="TrainingOrganizer"
                    HeaderText="Organizer" />

                <asp:BoundField
                    DataField="TrainingLocation"
                    HeaderText="Location" />

                <asp:BoundField
                    DataField="Batch"
                    HeaderText="Batch" />

                <asp:BoundField
                    DataField="DateFrom"
                    HeaderText="Date From" />

                <asp:BoundField
                    DataField="DateTo"
                    HeaderText="Date To" />

                <asp:TemplateField HeaderText="Action">

                    <ItemTemplate>

                        <asp:LinkButton
                            ID="lnkReAssign"
                            runat="server"
                            Text="ReAssign"
                            CssClass="btn-reassign"
                            CommandName="ReAssign"
                            CommandArgument='<%# Eval("ID") %>'>
                        </asp:LinkButton>
                        
                    </ItemTemplate>

                </asp:TemplateField>

            </Columns>

        </asp:GridView>

    </div>

    <asp:Panel
        ID="pnlReAssign"
        runat="server"
        Visible="false"
        CssClass="reassign-panel">

        <div class="panel-header">
            ReAssign Training
        </div>

        <div class="panel-body">

            <asp:HiddenField
                ID="hfAssignmentID"
                runat="server" />

            <div class="row">

                <div class="col-md-12">

                    <label>New Training ID</label>

                    <asp:TextBox
                        ID="txtNewTrainingID"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>

                    <small>
                        If Training ID is known, enter directly.
                    </small>

                </div>

            </div>

            <hr />

            <h5>OR Search Training</h5>

            <div class="row">

                <div class="col-md-3">

                    <label>Training Type</label>

                    <asp:DropDownList
                        ID="ddlTrainingType"
                        runat="server"
                        CssClass="form-control" 
                        AutoPostBack="true"
    OnSelectedIndexChanged="ddlTrainingType_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3">

                    <label>Organizer</label>

                    <asp:DropDownList
                        ID="ddlOrganizer"
                        runat="server"
                        CssClass="form-control" AutoPostBack="true"
    OnSelectedIndexChanged="ddlOrganizer_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3">

                    <label>Location</label>

                    <asp:DropDownList
                        ID="ddlLocation"
                        runat="server"
                        CssClass="form-control">
                    </asp:DropDownList>

                </div>
              <div class="col-md-6">

    <label>
        Designation
    </label>

    <input type="text"
        id="txtSearchDesignation"
        class="form-control"
        placeholder="Search Designation..."
        onkeyup="filterDesignation();" />

    <br />

    <div id="designationContainer"
         style="
            border:1px solid #ced4da;
            height:180px;
            overflow-y:auto;
            padding:8px;
            border-radius:5px;
            background:white;">

        <asp:CheckBoxList
            ID="chkDesignation"
            runat="server"
            RepeatLayout="Table"
            RepeatColumns="1"
            Width="100%">
        </asp:CheckBoxList>

    </div>

</div>
                <div class="col-md-3">

                    <label>Batch</label>

                    <asp:TextBox
                        ID="txtBatch"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

            </div>

            <br />

            <div class="row">

                <div class="col-md-3">

                    <label>Date From</label>

                    <asp:TextBox
                        ID="txtDateFrom"
                        runat="server"
                        TextMode="Date"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-3">

                    <label>Date To</label>

                    <asp:TextBox
                        ID="txtDateTo"
                        runat="server"
                        TextMode="Date"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-3">

                    <br />

                    <asp:Button
                        ID="btnSearchTraining"
                        runat="server"
                        Text="Search Training"
                        CssClass="btn btn-info"
                        OnClick="btnSearchTraining_Click" />

                </div>

            </div>

            <br />

            <asp:RadioButtonList
                ID="rblTrainingID"
                runat="server"
                RepeatDirection="Vertical">
            </asp:RadioButtonList>

            <br />

            <asp:Button
                ID="btnSaveReAssign"
                runat="server"
                Text="ReAssign"
                CssClass="btn btn-success"
                OnClick="btnSaveReAssign_Click" />

        </div>

    </asp:Panel>

</div>
   <script type="text/javascript">

function filterDesignation()
{
    var filter =
        document.getElementById(
        "txtSearchDesignation")
        .value.toUpperCase();

    var container =
        document.getElementById(
        "designationContainer");

    var rows =
        container.getElementsByTagName(
        "tr");

    for(var i=0;i<rows.length;i++)
    {
        var txt =
            rows[i].innerText ||
            rows[i].textContent;

        if(txt.toUpperCase()
            .indexOf(filter) > -1)
        {
            rows[i].style.display = "";
        }
        else
        {
            rows[i].style.display = "none";
        }
    }
}

</script>
</asp:Content>