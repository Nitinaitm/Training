<%@ Page Title="Employee Search"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Training.Admin.Default" %>


<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">


    <!-- =========================================================
         SELECT2
    ========================================================== -->

     <link rel="stylesheet"
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" />

    <link rel="stylesheet"
        href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/1.1.2/css/bootstrap-multiselect.min.css" />

    <script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/1.1.2/js/bootstrap-multiselect.min.js"></script>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>



    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>


    <style type="text/css">

        /* =====================================================
           GENERAL
        ===================================================== */

        * {
            box-sizing: border-box;
        }

        body {
            overflow-x: hidden;
        }

        .main-container {
            width: 100%;
            padding: 20px;
            min-height: 700px;
        }


        /* =====================================================
           CARDS
        ===================================================== */

        .search-card,
        .grid-card {
            width: 100%;
            background: #ffffff;
            border-radius: 12px;
            padding: 25px;
            margin-bottom: 25px;
            box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
        }


        /* =====================================================
           PAGE TITLE
        ===================================================== */

        .page-title {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 25px;
            color: #1e293b;
        }


        /* =====================================================
           SEARCH GRID
        ===================================================== */

        .search-grid {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 18px;
            align-items: start;
        }

        .form-group {
            min-width: 0;
            width: 100%;
            position: relative;
            display: flex;
            flex-direction: column;
        }

        .form-group label {
            display: block;
            margin-bottom: 7px;
            font-weight: 600;
            color: #334155;
            font-size: 14px;
        }


        /* =====================================================
           TEXTBOX
        ===================================================== */

        .textbox {
            display: block;
            width: 100%;
            height: 40px;
            padding: 8px 11px;
            border: 1px solid #cbd5e1;
            border-radius: 6px;
            background: #ffffff;
            color: #334155;
            font-size: 14px;
            outline: none;
        }

        .textbox:focus {
            border-color: #2563eb;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.10);
        }


        /* =====================================================
           SELECT2
        ===================================================== */

        .select2-container {
            width: 100% !important;
        }

        .select2-container--default
        .select2-selection--multiple {
            width: 100% !important;
            min-height: 40px !important;
            border: 1px solid #cbd5e1 !important;
            border-radius: 6px !important;
            background: #ffffff !important;
            padding: 2px 5px !important;
        }

        .select2-container--default.select2-container--focus
        .select2-selection--multiple {
            border-color: #2563eb !important;
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.10);
        }

        .select2-container--default
        .select2-selection--multiple
        .select2-selection__rendered {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            padding: 0 !important;
            margin: 0 !important;
        }

        .select2-container--default
        .select2-selection--multiple
        .select2-selection__choice {
            margin-top: 4px !important;
            margin-right: 5px !important;
            padding: 2px 7px 2px 20px !important;
            border: 1px solid #bfdbfe !important;
            border-radius: 4px !important;
            background: #eff6ff !important;
            color: #1e40af !important;
            font-size: 12px !important;
        }

        .select2-container--default
        .select2-selection--multiple
        .select2-selection__choice__remove {
            border-right: 1px solid #bfdbfe !important;
            color: #1e40af !important;
        }

        .select2-container--default
        .select2-search--inline
        .select2-search__field {
            height: 27px !important;
            margin-top: 4px !important;
            font-size: 13px !important;
            min-width: 100px !important;
        }

        .select2-dropdown {
            border: 1px solid #cbd5e1 !important;
            border-radius: 6px !important;
            box-shadow: 0 5px 15px rgba(0,0,0,.12);
            z-index: 99999 !important;
        }

        .select2-search--dropdown {
            padding: 8px !important;
        }

        .select2-search--dropdown
        .select2-search__field {
            width: 100% !important;
            height: 36px !important;
            padding: 6px 9px !important;
            border: 1px solid #cbd5e1 !important;
            border-radius: 5px !important;
            outline: none;
        }

        .select2-results__option {
            padding: 7px 10px !important;
            font-size: 13px !important;
        }


        /* =====================================================
           BUTTONS
        ===================================================== */

        .button-container {
            margin-top: 25px;
            display: flex;
            align-items: center;
            gap: 12px;
            flex-wrap: wrap;
        }

        .custom-btn {
            min-width: 110px;
            padding: 9px 22px;
            border: none;
            border-radius: 6px;
            color: #ffffff;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
        }

        .btn-search {
            background: #2563eb;
        }

        .btn-search:hover {
            background: #1d4ed8;
        }

        .btn-reset {
            background: #64748b;
        }

        .btn-reset:hover {
            background: #475569;
        }


        /* =====================================================
           GRID
        ===================================================== */

        .grid-card {
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
            margin: 0;
        }

        .gridview th {
            padding: 12px;
            background: #2563eb;
            color: #ffffff;
            text-align: left;
            font-size: 13px;
            font-weight: 600;
            white-space: nowrap;
        }

        .gridview td {
            padding: 11px 12px;
            border-bottom: 1px solid #e2e8f0;
            color: #334155;
            font-size: 13px;
            white-space: nowrap;
        }

        .gridview tr:nth-child(even) {
            background: #f8fafc;
        }

        .gridview tr:hover {
            background: #eef4ff;
        }


        /* =====================================================
           LARGE / MEDIUM SCREEN
        ===================================================== */

        @media screen and (max-width: 1200px) {

            .search-grid {
                grid-template-columns: repeat(3, 1fr);
            }

        }


        /* =====================================================
           TABLET
        ===================================================== */

        @media screen and (max-width: 991px) {

            .main-container {
                padding: 15px;
            }

            .search-grid {
                grid-template-columns: repeat(2, 1fr);
            }

            .search-card,
            .grid-card {
                padding: 20px;
            }

        }


        /* =====================================================
           MOBILE
        ===================================================== */

        @media screen and (max-width: 576px) {

            .main-container {
                padding: 10px;
            }

            .search-card,
            .grid-card {
                padding: 15px;
                border-radius: 8px;
            }

            .page-title {
                font-size: 21px;
                margin-bottom: 18px;
            }

            .search-grid {
                grid-template-columns: 1fr;
                gap: 14px;
            }

            .button-container {
                flex-direction: column;
                width: 100%;
            }

            .custom-btn {
                width: 100%;
            }

            .gridview {
                min-width: 950px;
            }

        }

    </style>

</asp:Content>



<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">


    <div class="main-container">


        <!-- =====================================================
             SEARCH CARD
        ====================================================== -->

        <div class="search-card">


            <div class="page-title">

                <i class="fa fa-search"></i>

                Employee Search

            </div>


            <div class="search-grid">


                <!-- =================================================
                     EMPLOYEE ID
                ================================================== -->

                <div class="form-group">

                    <label>
                        Employee ID
                    </label>

                    <asp:TextBox
                        ID="txtEmpID"
                        runat="server"
                        CssClass="textbox"
                        placeholder="Enter Employee ID">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     EMPLOYEE NAME
                ================================================== -->

                <div class="form-group">

                    <label>
                        Employee Name
                    </label>

                    <asp:TextBox
                        ID="txtEmpName"
                        runat="server"
                        CssClass="textbox"
                        placeholder="Enter Employee Name">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     MOBILE
                ================================================== -->

                <div class="form-group">

                    <label>
                        Mobile No
                    </label>

                    <asp:TextBox
                        ID="txtMobile"
                        runat="server"
                        CssClass="textbox"
                        MaxLength="10"
                        placeholder="Enter Mobile No">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     EMAIL
                ================================================== -->

                <div class="form-group">

                    <label>
                        Email ID
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="textbox"
                        placeholder="Enter Email ID">
                    </asp:TextBox>

                </div>


                <!-- =================================================
                     COMPANY
                ================================================== -->

                <div class="form-group">

                    <label>
                        Company
                    </label>

                    <asp:ListBox
                        ID="lstCompany"
                        runat="server"
                        SelectionMode="Multiple"
                        CssClass="form-control"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="lstCompany_SelectedIndexChanged">
                    </asp:ListBox>

                </div>


                <!-- =================================================
                     DESIGNATION
                ================================================== -->

                <div class="form-group">

                    <label>
                        Designation
                    </label>

                    <asp:ListBox
                        ID="lstDesignation"
                        runat="server"
                        SelectionMode="Multiple"
                        CssClass="form-control">
                    </asp:ListBox>

                </div>


                <!-- =================================================
                     POSTING PLACE
                ================================================== -->

                <div class="form-group">

                    <label>
                        Posting Place
                    </label>

                    <asp:ListBox
                        ID="lstPostingPlace"
                        runat="server"
                        SelectionMode="Multiple"
                        CssClass="form-control">
                    </asp:ListBox>

                </div>


            </div>


            <!-- =====================================================
                 BUTTONS
            ====================================================== -->

            <div class="button-container">


                <asp:Button
                    ID="btnSearch"
                    runat="server"
                    Text="Search"
                    CssClass="custom-btn btn-search"
                    OnClick="btnSearch_Click" />


                <asp:Button
                    ID="btnReset"
                    runat="server"
                    Text="Reset"
                    CssClass="custom-btn btn-reset"
                    OnClick="btnReset_Click" />


            </div>


        </div>



        <!-- =====================================================
             EMPLOYEE GRID
        ====================================================== -->

        <div class="grid-card">


            <asp:GridView
                ID="gvEmployee"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="gridview"
                EmptyDataText="No Record Found"
                GridLines="None">


                <Columns>


              

                    <asp:TemplateField HeaderText="Sl No">

                        <ItemTemplate>

                            <%# Container.DataItemIndex + 1 %>

                        </ItemTemplate>

                    </asp:TemplateField>


                    

                    <asp:BoundField
                        DataField="EmpID"
                        HeaderText="Emp ID" />


                  

                    <asp:BoundField
                        DataField="EmpName"
                        HeaderText="Employee Name" />


                  

                    <asp:BoundField
                        DataField="MobileNo"
                        HeaderText="Mobile No" />


                 

                    <asp:BoundField
                        DataField="EmailId"
                        HeaderText="Email ID" />


                  

                    <asp:BoundField
                        DataField="EmpCompany"
                        HeaderText="Company" />


                 

                    <asp:BoundField
                        DataField="EmpDesignation"
                        HeaderText="Designation" />


                  

                    <asp:BoundField
                        DataField="EmpPostingPlace"
                        HeaderText="Posting Place" />


                </Columns>


                <EmptyDataTemplate>

                    <div style="
                        padding:25px;
                        text-align:center;
                        color:#64748b;
                        font-weight:600;">

                        No Employee Record Found

                    </div>

                </EmptyDataTemplate>


            </asp:GridView>


        </div>


    </div>


</asp:Content>