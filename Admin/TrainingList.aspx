<%@ Page Title=""
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="TrainingList.aspx.cs"
    Inherits="Training.Admin.TrainingList" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

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
            color: #198754;
            margin-bottom: 20px;
        }

        .search-panel {
            background: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 10px;
            padding: 15px;
            margin-bottom: 20px;
        }

        .gridview th {
            background: #198754;
            color: white;
            text-align: center;
            vertical-align: middle;
        }

        .gridview td {
            vertical-align: middle;
        }

        .status-draft {
            color: #fd7e14;
            font-weight: bold;
        }

        .status-trainee {
            color: #198754;
            font-weight: bold;
        }

        .status-completed {
            color: #6f42c1;
            font-weight: bold;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Training Management
            </div>

            <div class="search-panel">

                <div class="row">

                    <div class="col-md-3">

                        <label>
                            Training ID
                        </label>

                        <asp:TextBox
                            ID="txtTrainingID"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>
                            Course Name
                        </label>

                        <asp:TextBox
                            ID="txtCourse"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>
                            Status
                        </label>

                        <asp:DropDownList
                            ID="ddlStatus"
                            runat="server"
                            CssClass="form-control">

                            <asp:ListItem Value="">
        All
                            </asp:ListItem>

                            <asp:ListItem Value="Draft">
        Draft
                            </asp:ListItem>

                            <asp:ListItem Value="SessionAssigned">
        Session Assigned
                            </asp:ListItem>

                            <asp:ListItem Value="TraineeAssigned">
        Trainee Assigned
                            </asp:ListItem>

                            <asp:ListItem Value="Completed">
        Completed
                            </asp:ListItem>

                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3">

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

            <asp:GridView
                ID="gvTraining"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-striped gridview"
                OnRowCommand="gvTraining_RowCommand">

                <Columns>

                    <asp:TemplateField HeaderText="Sl No">

                        <ItemTemplate>

                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>

                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="TrainingID"
                        HeaderText="Training ID" />

                    <asp:BoundField
                        DataField="CourseName"
                        HeaderText="Course" />

                    <asp:BoundField
                        DataField="TrainingCategory"
                        HeaderText="Category" />

                    <asp:BoundField
                        DataField="TrainingType"
                        HeaderText="Type" />

                    <asp:BoundField
                        DataField="TrainingLocation"
                        HeaderText="Location" />

                    <asp:BoundField
                        DataField="Batch"
                        HeaderText="Batch" />

                    <asp:BoundField
                        DataField="DateFrom"
                        HeaderText="Date From"
                        DataFormatString="{0:dd-MM-yyyy}" />

                    <asp:BoundField
                        DataField="DateTo"
                        HeaderText="Date To"
                        DataFormatString="{0:dd-MM-yyyy}" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>

                            <asp:Label
                                ID="lblStatus"
                                runat="server"
                                Text='<%# Eval("TrainingStatus") %>'
                                CssClass='<%#
                Eval("TrainingStatus").ToString()=="Draft" ? "badge bg-secondary" :
                Eval("TrainingStatus").ToString()=="SessionAssigned" ? "badge bg-warning text-dark" :
                Eval("TrainingStatus").ToString()=="TraineeAssigned" ? "badge bg-info text-dark" :
                "badge bg-success"
            %>'>
                            </asp:Label>

                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkManage"
                                runat="server"
                                Text="Manage"
                                CssClass="btn btn-success btn-sm"
                                CommandName="Manage"
                                CommandArgument='<%# Eval("TrainingID") %>'>
                            </asp:LinkButton>

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
