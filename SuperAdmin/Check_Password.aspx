<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="Check_Password.aspx.cs" Inherits="Training.SuperAdmin.Check_Password" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="../script.js" type="text/javascript"></script>
    <script type="text/javascript">
        function DisableBackButton() {
            window.history.forward()
        }
        DisableBackButton();
        window.onload = DisableBackButton;
        window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
        window.onunload = function () { void (0) }
    </script>
    <script>
        function preventBack() { window.history.forward(); }
        setTimeout("preventBack()", 0);
        window.onunload = function () { null };
    </script>
    <script type="text/javascript">
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
    </script>
    <link rel="stylesheet" href="css/StyleSheet3.css" />
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.0/css/select2.min.css" />


    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.0/js/select2.min.js"></script>



    <script>
        jQuery(document).ready(function ($) {
            $('.js-select2').select2();
        });

    </script>
    <style>
        .textbox {
            height: 40px;
            padding: 0 0px;
            border: none;
            border-radius: 25px;
            text-indent: 10px;
            color: darkcyan;
            font-size: 20px;
            width: 100px;
        }

            .textbox:focus {
                outline: none;
            }

        label {
            color: darkcyan;
        }
    </style>
    <style>
        .btn-search {
            height: 40px;
            padding: 0 0px;
            border: solid 2px;
            border-radius: 25px;
            font-size: 20px;
            width: 90px;
        }
    </style>
    <style>
        .submit-button-container {
            text-align: center;
        }

        .submit-button {
            width: 100%;
            /*max-width: 300px;*/
            padding: 10px;
            font-size: 16px;
            background-color: #4285f4;
            color: white;
            border: none;
            cursor: pointer;
        }
    </style>
    <style>
        /* Style the GridView container */
        .gridview-style {
            border-collapse: collapse;
            width: 100%;
        }

            /* Style the table header */
            .gridview-style th {
                background-color: cadetblue;
                border: 1px solid #dddddd;
                text-align: center;
                padding: 8px;
                font-weight: bold;
            }

            /* Style the table rows */
            .gridview-style tr {
                border: 1px solid #dddddd;
            }

                /* Style alternating rows for better readability */
                .gridview-style tr:nth-child(even) {
                    background-color: #f2f2f2;
                }

            /* Style the table cells */
            .gridview-style td {
                border: 1px solid #dddddd;
                text-align: center;
                padding: 8px;
            }

            /* Style the "Fill ACR" button */
            .gridview-style .btnAction {
                background-color: #4CAF50;
                color: white;
                border: none;
                padding: 6px 12px;
                text-align: center;
                text-decoration: none;
                display: inline-block;
                font-size: 12px;
                margin: 2px;
                cursor: pointer;
                border-radius: 4px;
            }
    </style>
    <style>

.row{
margin-bottom:15px;
}

.form-control,
.js-select2{
border-radius:8px!important;
min-height:40px!important;
}

.select2-container{
width:100%!important;
}

.select2-selection--single{
height:40px!important;
border-radius:8px!important;
border:1px solid #ced4da!important;
}

.select2-selection__rendered{
line-height:38px!important;
}

.btn{
border-radius:8px!important;
padding:8px 18px!important;
font-weight:600;
}

.gridview-style{
box-shadow:0px 2px 8px rgba(0,0,0,0.08);
border-radius:8px;
overflow:hidden;
}

.gridview-style th{
background:#0d6efd!important;
color:white!important;
}

.gridview-style tr:hover{
background:#f2f7ff;
}

</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <br />
    <br />
    <h3 style="text-align:center;
color:#0d6efd;
margin-bottom:20px;
font-weight:600;">

User Search Panel

</h3>

<div class="container-fluid px-3 px-md-4 px-lg-5">

<div style="
background:#fff;
padding:20px;
border-radius:12px;
box-shadow:0px 2px 10px rgba(0,0,0,.08);
">
    <div class="row">
        <div class="col-md-2">
            <label for="txtName" class="form-label">Emp ID</label>
            <asp:TextBox ID="txtEmpID" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="col-md-3">
            <label for="txtEmail" class="form-label">Emp Name</label>
            <asp:TextBox ID="txtName" runat="server" CssClass="form-control"></asp:TextBox>
        </div>
        <div class="col-md-3">
            <label for="ddlDesignation" class="form-label d-block">Designation</label>
            <asp:DropDownList ID="ddlDesignation" runat="server" AppendDataBoundItems="true" CssClass="js-select2">
            </asp:DropDownList>

        </div>
        <div class="col-md-2">
            <label for="ddlCompany" class="form-label d-block">Company</label>
            <asp:DropDownList ID="ddlCompany" runat="server" CssClass="js-select2">
                <asp:ListItem Text="Select a Company" Value=""></asp:ListItem>
                <asp:ListItem Text="BSPGCL" Value="BSPGCL"></asp:ListItem>
                <asp:ListItem Text="BSPHCL" Value="BSPHCL"></asp:ListItem>
                <asp:ListItem Text="SBPDCL" Value="SBPDCL"></asp:ListItem>
                <asp:ListItem Text="NBPDCL" Value="NBPDCL"></asp:ListItem>
                <asp:ListItem Text="BSPTCL" Value="BSPTCL"></asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col-md-2">
            <label for="ddlRole" class="form-label d-block">Role</label>
            <asp:DropDownList ID="ddlRole" runat="server" CssClass="js-select2">
                <asp:ListItem Text="Select Role" Value=""></asp:ListItem>
                <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>
                <asp:ListItem Text="Nodal" Value="Nodal"></asp:ListItem>
                <asp:ListItem Text="Custodian" Value="Custodian"></asp:ListItem>
                <asp:ListItem Text="Representation" Value="Representation"></asp:ListItem>
                <asp:ListItem Text="Verifier" Value="Verifier"></asp:ListItem>
                <asp:ListItem Text="Officer" Value="Officer"></asp:ListItem>
                <asp:ListItem Text="Verifier" Value="Verifier"></asp:ListItem>
                <asp:ListItem Text="Cust" Value="Cust"></asp:ListItem>
            </asp:DropDownList>
        </div>
    </div>

    <div class="row mb-3">
        <div class="col-md-12 text-center">
            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary me-2" OnClick="btnSearch_Click" />
            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-secondary" OnClick="btnClear_Click" />
        </div>
    </div>



    <asp:Panel ID="P1" runat="server" Visible="false">
        <div style="text-align: right">
            <asp:Button ID="btnExportToExcel" runat="server" Text="Export" ValidationGroup="ver"
                CssClass="btn-search" ForeColor="White" OnClick="btnExportToExcel_Click" CausesValidation="True" BackColor="Red" BorderStyle="Solid" />

        </div>
        <hr />
        <asp:GridView ID="gridView" runat="server" CssClass="gridview-style" BorderStyle="Dashed" Font-Size="Small" ForeColor="Black" HeaderStyle-HorizontalAlign="center" ShowHeader="True" GridLines="Both" EmptyDataText="No Record Found" AutoGenerateColumns="false" OnSelectedIndexChanged="gridView_SelectedIndexChanged">

            <Columns>
                <asp:TemplateField HeaderText="SL. No" ItemStyle-Width="2%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="2%" />
                </asp:TemplateField>

                <asp:BoundField HeaderText="Login ID" DataField="LoginIDUserID" />
                <asp:BoundField HeaderText="Name" DataField="EmpName" />
                <asp:BoundField HeaderText="Role" DataField="Role" />
                <asp:BoundField HeaderText="Corresponding Employee ID" DataField="CorrespondingEmpID" />
                <asp:BoundField HeaderText="Password" DataField="Password" />
                <asp:BoundField HeaderText="Status" DataField="re" />
                <asp:BoundField HeaderText="Mobile No" DataField="MobileNo" />
                <asp:BoundField HeaderText="EmailID" DataField="EmailId" />
                <asp:BoundField HeaderText="Designation" DataField="EmpDesignation" />


            </Columns>
        </asp:GridView>
    </asp:Panel>

    </div>
    </div>
</asp:Content>
