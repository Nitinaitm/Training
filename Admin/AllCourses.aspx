<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master" AutoEventWireup="true" CodeBehind="AllCourses.aspx.cs" Inherits="Training.Admin.AllCourses" %>
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
      <div class="main-card">

            <div class="row">

                <div class="col-lg-4 mb-3">

                    <label class="form-label">Search Course</label>

                    <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" AutoPostBack="true" OnTextChanged="txtSearch_TextChanged" placeholder="Enter Course Name"></asp:TextBox>

                </div>

                <div class="col-lg-8 text-end mt-4">

                    <asp:Button ID="btnExportExcel" runat="server" Text="Export Excel" CssClass="btn btn-success" CausesValidation="false" OnClick="btnExportExcel_Click" />

                </div>

            </div>

            <div class="table-responsive">

                <asp:GridView ID="gvCourse" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover table-striped" Width="100%" DataKeyNames="CourseID" AllowPaging="true" PageSize="20" AllowSorting="true" >

                    <HeaderStyle CssClass="table-dark" />

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">

                            <ItemTemplate>

                                <%# Container.DataItemIndex + 1 %>
                            </ItemTemplate>

                            <ItemStyle Width="70px" HorizontalAlign="Center" />

                        </asp:TemplateField>

                        <asp:BoundField DataField="CourseName" HeaderText="Course Name" SortExpression="CourseName" />

                        <asp:BoundField DataField="CourseCategory" HeaderText="Category" SortExpression="CourseCategory" />

                        <asp:BoundField DataField="PassingPercentage" HeaderText="Passing %" />

                        <asp:BoundField DataField="AttendancePercentage" HeaderText="Attendance %" />

                        <asp:BoundField DataField="CreatedOn" HeaderText="Created On" DataFormatString="{0:dd-MM-yyyy}" />

                        

                    </Columns>

                    <EmptyDataTemplate>

                        <div class="text-center p-3">
                            No Course Found.

                        </div>

                    </EmptyDataTemplate>

                    <PagerStyle CssClass="table-secondary" HorizontalAlign="Center" />

                </asp:GridView>

            </div>

        </div>
</asp:Content>
