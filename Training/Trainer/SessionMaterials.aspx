<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="SessionMaterials.aspx.cs" Inherits="Training.Trainer.SessionMaterials" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px
        }

        .gridview th {
            background: #198754;
            color: white;
            text-align: center;
            vertical-align: middle
        }

        .gridview td {
            vertical-align: middle
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Session Materials</div>
        <div class="dashboard-card">
            <div class="card-header bg-success text-white">
                <h5 class="mb-0"><i class="fa fa-upload"></i>Upload Material</h5>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-4">
                        <label>Session *</label><asp:DropDownList ID="ddlSession" runat="server" CssClass="form-select" /></div>
                    <div class="col-md-4">
                        <label>Title *</label><asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" /></div>
                    <div class="col-md-4">
                        <label>File *</label><asp:FileUpload ID="fuMaterial" runat="server" CssClass="form-control" /></div>
                </div>
                <div class="mt-3">
                    <asp:Button ID="btnUpload" runat="server" Text="Upload" CssClass="btn btn-success" OnClick="btnUpload_Click" /><asp:Label ID="lblMessage" runat="server" Font-Bold="true" CssClass="ms-3" /></div>
            </div>
        </div>
        <div class="dashboard-card">
            <div class="card-header bg-primary text-white">
                <h5 class="mb-0"><i class="fa fa-list"></i>Session Materials</h5>
            </div>
            <div class="card-body">
                <asp:GridView ID="gvMaterials" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Materials">
                    <Columns>
                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                            <ItemStyle Width="50px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="Title" HeaderText="Title" />
                        <asp:BoundField DataField="SessionName" HeaderText="Session" />
                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                        <asp:BoundField DataField="CreatedOn" HeaderText="Uploaded On" DataFormatString="{0:dd-MM-yyyy}" />
                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkDownload" runat="server" Text="Download" CssClass="btn btn-info btn-sm" CommandName="Download" CommandArgument='<%# Eval("MaterialID") %>' /><asp:LinkButton ID="lnkDelete" runat="server" Text="Delete" CssClass="btn btn-danger btn-sm ms-1" CommandName="Delete" CommandArgument='<%# Eval("MaterialID") %>' OnClientClick="return confirm('Delete?');" /></ItemTemplate>
                            <ItemStyle Width="150px" HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
