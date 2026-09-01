<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="CourseCategoryMaster.aspx.cs" Inherits="Training.Admin.CourseCategoryMaster" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style>
        body {
            background: #f5f5f5;
        }

        .main-card {
            background: #ffffff;
            padding: 25px;
            border-radius: 12px;
            box-shadow: 0px 0px 10px #d9d9d9;
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: darkcyan;
            margin-bottom: 20px;
        }

        .validation {
            color: red;
            font-size: 13px;
        }

        .btn-save {
            background: darkcyan;
            color: white;
            border: none;
        }

            .btn-save:hover {
                background: teal;
                color: white;
            }

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default
        .select2-selection--single {
            height: 38px !important;
            border: 1px solid #ced4da !important;
        }

        .select2-selection__rendered {
            line-height: 36px !important;
        }

        .select2-selection__arrow {
            height: 36px !important;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">

        <div class="main-card">

            <div class="page-heading">
                Category Entry

            </div>

            <div class="row">

                <!-- Category Name -->

                <div class="col-lg-6 mb-3">

                    <label class="form-label">
                        Category Name *

                    </label>

                    <asp:TextBox
                        ID="txtCategoryName"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="300">
                    </asp:TextBox>

                    <asp:RequiredFieldValidator
                        ID="rfvCategoryName"
                        runat="server"
                        ControlToValidate="txtCategoryName"
                        ValidationGroup="SaveGroup"
                        CssClass="validation"
                        ErrorMessage="Enter Category Name">
                    </asp:RequiredFieldValidator>

                </div>

               
                <!-- Remarks -->

                <div class="col-lg-6 mb-3">

                    <label class="form-label">
                        Remarks

                    </label>

                    <asp:TextBox
                        ID="txtRemarks"
                        runat="server"
                        CssClass="form-control"
                        TextMode="MultiLine"
                        Rows="2">
                    </asp:TextBox>

                </div>

                <!-- Buttons -->

                <div class="col-lg-12 mt-3">

                    <asp:Button
                        ID="btnSave"
                        runat="server"
                        Text="Save"
                        CssClass="btn btn-success"
                        Width="120px"
                        ValidationGroup="SaveGroup"
                        OnClick="btnSave_Click" />

                    &nbsp;

                    <asp:Button
                        ID="btnUpdate"
                        runat="server"
                        Text="Update"
                        CssClass="btn btn-warning"
                        Width="120px"
                        Visible="false"
                        ValidationGroup="SaveGroup"
                        OnClick="btnUpdate_Click" />

                   

                    &nbsp;

                    <asp:Button
                        ID="btnClear"
                        runat="server"
                        Text="Clear"
                        CssClass="btn btn-secondary"
                        Width="120px"
                        CausesValidation="false"
                        OnClick="btnClear_Click" />

                </div>

                <!-- Message -->

                <div class="col-lg-12 mt-3">

                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        Font-Bold="true"
                        Font-Size="14px">
                    </asp:Label>

                </div>

            </div>

        </div>

        <div class="main-card">

            <div class="row">

                <div class="col-lg-4 mb-3">

                    <label class="form-label">Search Category</label>

                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged" placeholder="Enter Category Name"></asp:TextBox>

                </div>

                <div class="col-lg-8 text-end mt-4">

                    <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success" CausesValidation="false" OnClick="btnExportExcel_Click" />

                </div>

            </div>

            <div class="table-responsive">

                <asp:GridView ID="gvCategory" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover table-striped" Width="100%" DataKeyNames="CategoryID" AllowPaging="true" PageSize="20" AllowSorting="true" OnPageIndexChanging="gvCategory_PageIndexChanging" OnSorting="gvCategory_Sorting" OnRowCommand="gvCategory_RowCommand">

                    <HeaderStyle CssClass="table-dark" />

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">

                            <ItemTemplate>

                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>

                            <ItemStyle Width="70px" HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:BoundField DataField="CategoryName" HeaderText="Category Name" SortExpression="CategoryName" />
                        <asp:BoundField DataField="Remarks" HeaderText="Remarks" SortExpression="Remarks" />

                     

                        <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />

                        <asp:TemplateField HeaderText="Edit">

                            <ItemStyle Width="70px" HorizontalAlign="Center" />

                            <ItemTemplate>

                                <asp:LinkButton ID="lnkEdit" runat="server" CssClass="btn btn-primary btn-sm" Text="Edit" CommandName="EditRecord" CommandArgument='<%# Eval("CategoryID") %>' CausesValidation="false"></asp:LinkButton>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Delete">

                            <ItemStyle Width="80px" HorizontalAlign="Center" />

                            <ItemTemplate>

                                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-sm" Text="Delete" CommandName="DeleteRecord" CommandArgument='<%# Eval("CategoryID") %>' CausesValidation="false" OnClientClick="return confirm('Are you sure you want to delete this Category?');"></asp:LinkButton>

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                    <EmptyDataTemplate>

                        <div class="text-center p-3">
                            No Category Found.

                        </div>

                    </EmptyDataTemplate>

                    <PagerStyle CssClass="table-secondary" HorizontalAlign="Center" />

                </asp:GridView>

            </div>

        </div>

    </div>
</asp:Content>
