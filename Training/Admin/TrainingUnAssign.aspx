<%@ Page Title="" Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingUnAssign.aspx.cs"
    Inherits="Training.Admin.TrainingUnAssign" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
     <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
      rel="stylesheet" />
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
    color:#dc3545;
    margin-bottom:20px;
}

.gridview th{
    background:#dc3545;
    color:white;
    text-align:center;
}

.gridview td{
    vertical-align:middle;
}

.btn-unassign{
    background:#dc3545;
    color:white;
    border:none;
    padding:5px 12px;
    border-radius:5px;
    text-decoration:none;
}

.btn-unassign:hover{
    color:white;
    background:#bb2d3b;
}

</style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="container-fluid">

    <div class="page-card">

        <div class="page-title">
            Training UnAssign
        </div>

        <div class="row">

            <div class="col-md-3">

                <label>
                    Employee ID
                </label>

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
                    CssClass="btn btn-primary"
                    OnClick="btnSearch_Click" />

            </div>

        </div>

        <br />

        <div class="table-responsive">

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

                    <asp:TemplateField
                        HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkUnAssign"
                                runat="server"
                                Text="UnAssign"
                                CssClass="btn-unassign"
                                CommandName="UnAssign"
                                CommandArgument='<%# Eval("ID") %>'
                               >
                            </asp:LinkButton>

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>

</asp:Content>