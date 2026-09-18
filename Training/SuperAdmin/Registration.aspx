<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdminMaster.Master" AutoEventWireup="true" CodeBehind="Registration.aspx.cs" Inherits="Training.SuperAdmin.Registration" %>

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
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.0/css/select2.min.css" />


    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.0/js/select2.min.js"></script>



    <script>
        jQuery(document).ready(function ($) {
            $('.js-select2').select2();
        });

    </script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" />
    <script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
    <script>
        window.addEventListener('DOMContentLoaded', function () {
            var inputElements = document.querySelectorAll('.date-input');
            inputElements.forEach(function (inputElement) {
                var selectedDate = inputElement.value;

                // Make an asynchronous request to fetch the current server date
                fetch('/GetServerDate.aspx') // Replace with the actual URL of your ASPX page
                    .then(function (response) {
                        if (response.ok) {
                            return response.json();
                        }
                        throw new Error('Error fetching server date');
                    })
                    .then(function (data) {
                        var currentDate = new Date(data.serverDate);

                        var minDate = new Date('01-01-1940');

                        flatpickr(inputElement, {
                            minDate: minDate,

                            defaultDate: selectedDate || currentDate,
                            dateFormat: "d-m-Y"
                        });
                    })
                    .catch(function (error) {
                        console.error(error);
                    });
            });
        });
    </script>
    <script>
        function validateMobileInput() {
            var input = document.getElementById("phone");
            var sanitizedValue = input.value.replace(/\D/g, "");
            input.value = sanitizedValue;
        }
    </script>
    <script>
        function isNumber(evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;

            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        }
    </script>
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
    <asp:ScriptManager ID="ScriptManager2" runat="server" EnableCdn="true"></asp:ScriptManager>
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

    <div id="searchUsername" runat="server">
        <div class="row">
            <div class="col-md-2 ">
                <label>Enter Username</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtUsername" runat="server" BorderColor="DarkCyan" BorderStyle="Solid" MaxLength="19" ForeColor="DarkCyan"></asp:TextBox>
            </div>
            <%--  <div class="col-md-2" id="nameVisible" runat="server" visible="false">
                <label>Name</label>
            </div>
            <div class="col-md-3" id="nametextboxVisible" runat="server" visible="false">
                <asp:TextBox ID="txtName" runat="server" Enabled="false" BorderColor="DarkCyan" BorderStyle="Solid" MaxLength="49" ForeColor="DarkCyan"></asp:TextBox>
            </div>--%>


            <div class="col-md-2">
                <asp:Button ID="btnSearch" runat="server" Text="Search" ValidationGroup="ver"
                    CssClass="btn-search" ForeColor="White" OnClick="btnSearch_Click" CausesValidation="True" BackColor="#4285F4" BorderStyle="Solid" />
            </div>
        </div>

    </div>

    <asp:Panel ID="P1" runat="server" Visible="false">
        <hr />
        <asp:GridView ID="gridView" runat="server" CssClass="gridview-style" BorderStyle="Dashed" Font-Size="Small" ForeColor="Black" HeaderStyle-HorizontalAlign="center" ShowHeader="True" GridLines="Both" EmptyDataText="No Record Found" AutoGenerateColumns="false" OnSelectedIndexChanged="gridView_SelectedIndexChanged">

            <Columns>
                <asp:TemplateField HeaderText="SL. No" ItemStyle-Width="2%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                </asp:TemplateField>

                <asp:BoundField HeaderText="Login ID" DataField="LoginIDUserID" />
                <asp:BoundField HeaderText="Role" DataField="Role" />
                <asp:BoundField HeaderText="Active" DataField="Active" />
                <asp:BoundField HeaderText="Corresponding Employee ID" DataField="CorrespondingEmpID" />


            </Columns>
        </asp:GridView>
    </asp:Panel>

    <asp:Panel ID="P2" runat="server" Visible="false">

        <asp:GridView ID="gridView1" runat="server" CssClass="gridview-style" BorderStyle="Dashed" Font-Size="Small" ForeColor="Black" HeaderStyle-HorizontalAlign="center" ShowHeader="True" GridLines="Both" EmptyDataText="No Record Found" AutoGenerateColumns="false" OnSelectedIndexChanged="gridView1_SelectedIndexChanged">

            <Columns>
                <asp:TemplateField HeaderText="SL. No" ItemStyle-Width="2%" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                </asp:TemplateField>

                <asp:BoundField HeaderText="Employee ID" DataField="EmpID" />
                <asp:BoundField HeaderText="Name" DataField="EmpName" />
                <asp:BoundField HeaderText="DOB" DataField="DOB" />
                <asp:BoundField HeaderText="DOJ" DataField="DOJ" />
                <asp:BoundField HeaderText="Mobile No" DataField="MobileNo" />
                <asp:BoundField HeaderText="Email Id" DataField="EmailId" />
                <asp:BoundField HeaderText="Company" DataField="EmpCompany" />
                <asp:BoundField HeaderText="Designation" DataField="EmpDesignation" />


            </Columns>
        </asp:GridView>
    </asp:Panel>
    <%--<asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
      <%-- </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>--%>
            <div id="login_not_found" runat="server" visible="false">
                <hr />
                <div class="row">
                    <div class="col-md-2">
                        <label for="fname">Enter Username</label>
                    </div>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtUsernameLogin" runat="server" BorderColor="DarkCyan" BorderStyle="Solid" MaxLength="19" ForeColor="DarkCyan" Enabled="false"></asp:TextBox>
                    </div>
                    <div class="col-md-2">
                        <label for="fname">Role</label>
                    </div>
                    <div class="col-md-4">
                        <asp:DropDownList ID="ddlRole" runat="server"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged">

                            <asp:ListItem Text="Select Role" Value=""></asp:ListItem>
                            <asp:ListItem Text="Officer" Value="Officer"></asp:ListItem>
                            <asp:ListItem Text="Custodian" Value="Custodian"></asp:ListItem>
							<asp:ListItem Text="Verifier" Value="Verifier"></asp:ListItem>
                            <asp:ListItem Text="Representation" Value="Representation"></asp:ListItem>
                            <asp:ListItem Text="Admin" Value="Admin"></asp:ListItem>

                        </asp:DropDownList>
                    </div>

                </div>
                <hr />
                <div class="row">
                    <div class="col-md-2">
                        <label for="fname">Corresponding ID</label>
                    </div>
                    <div class="col-md-3">
                        <asp:TextBox ID="txtCorrespondingLogin" runat="server" BorderColor="DarkCyan" BorderStyle="Solid" MaxLength="6" ForeColor="DarkCyan" AutoPostBack="true" OnTextChanged="txtCorrespondingLogin_TextChanged"></asp:TextBox>
                    </div>
                    <%-- <div class="col-md-2">
                        <label for="fname">Active</label>
                    </div>
                    <div class="col-md-4">
                        <asp:DropDownList ID="ddlActive" runat="server">

                            <asp:ListItem Text="Y" Value="Y"></asp:ListItem>
                            <asp:ListItem Text="N" Value="N"></asp:ListItem>


                        </asp:DropDownList>
                    </div>--%>
                </div>

            </div>
     
    <div id="update_Mobile" runat="server" visible="false">
        <hr />
        <div class="row">
            <div class="col-md-2">
                <label for="fname">Mobile No.:</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtMobile" runat="server" onkeypress="return isNumber(event)" oninput="validateMobileInput()" MaxLength="10"></asp:TextBox>
            </div>

            <div class="col-md-2">
                <label for="fname">Email ID :</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtMail" runat="server" MaxLength="49"></asp:TextBox>
            </div>
        </div>
        <hr />
        <div style="text-align: center">
            <asp:Label ID="Label1" Font-Bold="true" runat="server" ForeColor="Red"></asp:Label>
            <asp:Button ID="btnUpdate" runat="server" class="submit-button" Text="Update" ValidationGroup="ver" CausesValidation="True" OnClick="btnUpdate_Click" />
        </div>
    </div>
    <div id="update_basic_master" runat="server" visible="false">
        <hr />
        <div class="row">
            <div class="col-md-2">
                <label for="fname">Emp ID:</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtEmp_basic" runat="server" MaxLength="6" Enabled="false"></asp:TextBox>
            </div>

            <div class="col-md-2">
                <label for="fname">Name:</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtName_basic" runat="server" MaxLength="54"></asp:TextBox>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-md-2">
                <label for="fname">DOB:</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="from_period" runat="server" class="date-input"></asp:TextBox>

                <%-- <asp:TextBox ID="txtDOB_basic" runat="server"></asp:TextBox>--%>
            </div>

            <div class="col-md-2">
                <label for="fname">DOJ :</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="to_period" runat="server" class="date-input"></asp:TextBox>

                <%--                <asp:TextBox ID="txtDOJ_basic" runat="server"></asp:TextBox>--%>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-md-2">
                <label for="fname">Mobile No.:</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtMobile_basic" runat="server" onkeypress="return isNumber(event)" oninput="validateMobileInput()" MaxLength="10"></asp:TextBox>
            </div>

            <div class="col-md-2">
                <label for="fname">Email ID :</label>
            </div>
            <div class="col-md-3">
                <asp:TextBox ID="txtEmail_basic" runat="server" MaxLength="49"></asp:TextBox>
            </div>
        </div>
        <hr />
        <div class="row">
            <div class="col-md-2">
                <label for="fname">Company:</label>
            </div>
            <div class="col-md-3">
                <asp:DropDownList ID="ddlCompany" runat="server" AppendDataBoundItems="true" Width="190px" CssClass="js-select2">
                </asp:DropDownList>
            </div>

            <div class="col-md-2">
                <label for="fname">Designation :</label>
            </div>
            <div class="col-md-4">
                <asp:DropDownList ID="ddlDesignation_Selected" runat="server" AppendDataBoundItems="true" CssClass="js-select2">
                </asp:DropDownList>
            </div>
        </div>
        <hr />
        
        <div style="text-align: center">
            <asp:Label ID="Label2" Font-Bold="true" runat="server" ForeColor="Red"></asp:Label>
            <asp:Button ID="btnInsert_basic" runat="server" class="submit-button" Text="Save" ValidationGroup="ver" CausesValidation="True" OnClick="btnInsert_basic_Click" />
        </div>
    </div>



    <%--  <div id="login_not_found_basic_not_found" runat="server" visible="false">

    </div>--%>

    <div style="text-align: center" id="final_submit_all_data" runat="server" visible="false">
        <hr />
        <%--        <asp:Label ID="lblMsg1" Font-Bold="true" runat="server" ForeColor="Red"></asp:Label>--%>

        <asp:Button ID="BtnFinalSubmit" runat="server" class="submit-button" Text="Submit" ValidationGroup="ver" CausesValidation="True" OnClick="BtnFinalSubmit_Click" />

    </div>
    </div>
    </div>
    
</asp:Content>
