<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="AllTrainers.aspx.cs"
    Inherits="Training.Admin.AllTrainers" %>

<asp:Content ID="Content1"    ContentPlaceHolderID="head"    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>
        .main-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0 0 10px #d9d9d9;
            margin-top: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 20px;
        }

        .search-panel {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
        }

        .gridview th {
            background: #0d6efd;
            color: white;
            text-align: center;
        }

        .gridview td {
            vertical-align: middle;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                All Trainers
            </div>

            <div class="search-panel">

                <div class="row">

                    <div class="col-md-2">

                        <label>Trainer Type</label>

                        <asp:DropDownList
                            ID="ddlTrainerType"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem Text="All" Value=""></asp:ListItem>
                            <asp:ListItem Text="Internal" Value="Internal"></asp:ListItem>
                            <asp:ListItem Text="External" Value="External"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-md-2">

                        <label>Employee ID</label>

                        <asp:TextBox
                            ID="txtEmpID"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>Trainer Name</label>

                        <asp:TextBox
                            ID="txtTrainerName"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>Organization</label>

                        <asp:TextBox
                            ID="txtOrganization"
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

                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Reset"
                            CssClass="btn btn-secondary"
                            OnClick="btnReset_Click" />

                    </div>

                </div>

            </div>

            <div class="mb-2">

                <asp:Label
                    ID="lblCount"
                    runat="server"
                    Font-Bold="true">
                </asp:Label>

            </div>

            <asp:GridView
                ID="gvTrainer"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-striped gridview">

                <Columns>

                    <asp:TemplateField HeaderText="Sl No">
                        <ItemTemplate>
                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="TrainerID"
                        HeaderText="Trainer ID" />

                    <asp:BoundField
                        DataField="TrainerType"
                        HeaderText="Trainer Type" />

                    <asp:BoundField
                        DataField="EmpID"
                        HeaderText="Employee ID" />

                    <asp:BoundField
                        DataField="TrainerName"
                        HeaderText="Trainer Name" />

                    <asp:BoundField
                        DataField="Designation"
                        HeaderText="Designation" />

                    <asp:BoundField
                        DataField="Organization"
                        HeaderText="Organization" />

                    <asp:BoundField
                        DataField="Remarks"
                        HeaderText="Remarks" />

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
