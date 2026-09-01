<%@ Page Title="" Language="C#" MasterPageFile="~/SuperAdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingSearch.aspx.cs"
    Inherits="Training.SuperAdmin.TrainingSearch" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>
        * {
            box-sizing: border-box;
        }

        .main-container {
            padding: 20px;
            /*max-width: 100%;
            overflow: hidden;*/
        }

        .search-card, .grid-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            margin-bottom: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
            width: 100%;
        }

        .page-title {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 25px;
            color: #1e293b;
        }

        .search-grid {
            display: grid;
            grid-template-columns: repeat(7,1fr);
            gap: 15px;
        }

        .form-group {
            display: flex;
            flex-direction: column;
        }

            .form-group label {
                font-weight: 600;
                margin-bottom: 8px;
            }

        .textbox {
            padding: 10px;
            border: 1px solid #ccc;
            border-radius: 8px;
            width: 100%;
        }

        .multiselect-search {
            width: 100%;
            padding: 8px;
            border: 1px solid #ccc;
            border-radius: 5px;
            margin-bottom: 8px;
        }

        .button-container {
            display: flex;
            gap: 10px;
            margin-top: 20px;
            flex-wrap: wrap;
        }

        .btn-search {
            background: #0d6efd;
            color: white;
        }

        .btn-reset {
            background: #6c757d;
            color: white;
        }

        .table-responsive {
            width: 100%;
            overflow-x: auto;
            -webkit-overflow-scrolling: touch;
        }

        .gridview {
            width: 100%;
            border-collapse: collapse;
            table-layout: auto;
        }

            .gridview th {
                background: #0d6efd;
                color: white;
                padding: 10px;
                white-space: nowrap;
                font-size: 13px;
            }

            .gridview td {
                padding: 8px;
                border: 1px solid #ddd;
                white-space: nowrap;
                font-size: 13px;
            }

        .action-btn {
            padding: 6px 12px;
            border-radius: 6px;
            text-decoration: none;
            display: inline-block;
            color: white;
            font-size: 12px;
        }

        .detail-btn {
            background: #0f766e;
        }

        .feedback-btn {
            background: #7c3aed;
        }

        .training-btn {
            background: #ea580c;
        }

        .multiselect-container {
            position: relative;
        }

        .multiselect-header {
            border: 1px solid #ccc;
            padding: 10px;
            border-radius: 8px;
            cursor: pointer;
            background: white;
        }

        .multiselect-content {
            display: none;
            position: absolute;
            background: white;
            width: 100%;
            border: 1px solid #ccc;
           max-height: 350px;
    overflow-y: auto;
    overflow-x: hidden;

    z-index: 99999;
    padding: 10px;

    box-shadow: 0 4px 10px rgba(0,0,0,.15);
}
        .multiselect-content table {
    width: 100%;
    margin-bottom: 0 !important;
}


        .multiselect-container.active .multiselect-content {
            display: block;
        }

        @media(max-width:1200px) {
            .search-grid {
                grid-template-columns: repeat(4,1fr);
            }
        }

        @media(max-width:768px) {
            .search-grid {
                grid-template-columns: repeat(2,1fr);
            }
        }

        @media(max-width:576px) {
            .search-grid {
                grid-template-columns: 1fr;
            }
        }
    </style>

</asp:Content>



<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="main-container">

        <div class="search-card">

            <div class="page-title">
                Training Search Report
            </div>

            <div class="search-grid">

                <div class="form-group">
                    <label>Emp ID</label>
                    <asp:TextBox ID="txtEmpID" runat="server" CssClass="textbox" />
                </div>

                <div class="form-group">
                    <label>Employee Name</label>
                    <asp:TextBox ID="txtEmpName" runat="server" CssClass="textbox" />
                </div>

                <div class="form-group">
                    <label>Mobile</label>
                    <asp:TextBox ID="txtMobile" runat="server" CssClass="textbox" />
                </div>

                <div class="form-group">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="textbox" />
                </div>


                <div class="form-group">

                    <label>Designation</label>

                    <div class="multiselect-container" id="designationBox">

                        <div class="multiselect-header"
                            onclick="toggleMultiSelect('designationBox')">
                            Select Designation

                        </div>

                        <div class="multiselect-content">

                            <input type="text"
                                class="multiselect-search"
                                placeholder="Search Designation"
                                onkeyup="filterCheckbox('designationBox',this.value)" />

                            <asp:CheckBoxList
                                ID="chkDesignation"
                                runat="server">
                            </asp:CheckBoxList>

                        </div>

                    </div>

                </div>


                <div class="form-group">

                    <label>Company</label>

                    <div class="multiselect-container"
                        id="companyBox">

                        <div class="multiselect-header"
                            onclick="toggleMultiSelect('companyBox')">
                            Select Company

                        </div>

                        <div class="multiselect-content">

                            <input type="text"
                                class="multiselect-search"
                                placeholder="Search Company"
                                onkeyup="filterCheckbox('companyBox',this.value)" />

                            <asp:CheckBoxList
                                ID="chkCompany"
                                runat="server">
                            </asp:CheckBoxList>

                        </div>

                    </div>

                </div>


                <div class="form-group">

                    <label>Posting Place</label>

                    <div class="multiselect-container"
                        id="postingBox">

                        <div class="multiselect-header"
                            onclick="toggleMultiSelect('postingBox')">
                            Select Posting Place

                        </div>

                        <div class="multiselect-content">

                            <input type="text"
                                class="multiselect-search"
                                placeholder="Search Posting Place"
                                onkeyup="filterCheckbox('postingBox',this.value)" />

                            <asp:CheckBoxList
                                ID="chkPostingPlace"
                                runat="server">
                            </asp:CheckBoxList>

                        </div>

                    </div>

                </div>

            </div>


            <div class="button-container">

                <asp:Button
    ID="btnSearch"
    runat="server"
    Text="Search"
    CssClass="btn btn-search"
    OnClick="btnSearch_Click" />

<asp:Button
    ID="btnShowAll"
    runat="server"
    Text="Show All"
    CssClass="btn btn-warning"
    OnClick="btnShowAll_Click" />

<asp:Button
    ID="btnReset"
    runat="server"
    Text="Reset"
    CssClass="btn btn-reset"
    OnClick="btnReset_Click" />

            </div>

        </div>


        <div class="grid-card">

            <div class="table-responsive">

                <asp:GridView
                    ID="gvTraining"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="gridview table table-bordered">

                    <Columns>

                        <asp:TemplateField HeaderText="Sl No">
                            <ItemTemplate>
                                <%# Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="EmpID" HeaderText="Emp ID" />
                        <asp:BoundField DataField="EmpName" HeaderText="Employee Name" />
                        <asp:BoundField DataField="TrainingType" HeaderText="Training Type" />
                        <asp:BoundField DataField="TrainingOrganizer" HeaderText="Organizer" />
                        <asp:BoundField DataField="Batch" HeaderText="Batch" />
                        <asp:BoundField DataField="Attendance" HeaderText="Attendance" />
                        <asp:BoundField DataField="DateFrom" HeaderText="Date From" />
                        <asp:BoundField DataField="DateTo" HeaderText="Date To" />
                        <asp:BoundField DataField="LocationOfInduction" HeaderText="Location" />

                        <asp:TemplateField HeaderText="Employee">
                            <ItemTemplate>

                                <a href="javascript:void(0)"
                                    class="action-btn detail-btn"
                                    onclick='showDetails(
"<%# Eval("MobileNo") %>",
"<%# Eval("EmailId") %>",
"<%# Eval("EmpCompany") %>",
"<%# Eval("EmpDesignation") %>",
"<%# Eval("EmpPostingPlace") %>")'>View

                                </a>

                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Topic Feedback">
                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="lnkTopicFeedback"
                                    runat="server"
                                    Text="View"
                                    CssClass="action-btn feedback-btn"
                                    CommandArgument='<%# Eval("EmpID")+"|"+Eval("TrainingID") %>'
                                    OnClick="lnkTopicFeedback_Click" />

                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Training Related Feedback">

                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="lnkTrainingFeedback"
                                    runat="server"
                                    Text="View"
                                    CssClass="action-btn training-btn"
                                    CommandArgument='<%# Eval("EmpID")+"|"+Eval("TrainingID") %>'
                                    OnClick="lnkTrainingFeedback_Click" />

                            </ItemTemplate>

                        </asp:TemplateField>


                             <asp:TemplateField HeaderText="Overall Feedback">

                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="lnkOverallResponse"
                                    runat="server"
                                    Text="View"
                                    CssClass="action-btn training-btn"
                                    CommandArgument='<%# Eval("EmpID")+"|"+Eval("TrainingID") %>'
                                    OnClick="lnkOverallResponse_Click" />

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </div>

    </div>


    <div class="modal fade" id="feedbackModal">

        <div class="modal-dialog modal-xl">

            <div class="modal-content">

                <div class="modal-header">

                    <h5>Feedback Details</h5>

                    <button type="button"
                        class="btn-close"
                        data-bs-dismiss="modal">
                    </button>

                </div>

                <div class="modal-body">

                    <div class="mb-2 text-end">

                        <asp:Button
                            ID="btnFeedbackExport"
                            runat="server"
                            Text="Export Excel"
                            CssClass="btn btn-success"
                            OnClick="btnFeedbackExport_Click" />

                    </div>

                    <div style="overflow: auto">

                        <asp:GridView
                            ID="gvFeedback"
                            runat="server"
                            CssClass="table table-bordered">
                        </asp:GridView>

                    </div>

                </div>

            </div>

        </div>

    </div>


    <div class="modal fade"
        id="empModal">

        <div class="modal-dialog">

            <div class="modal-content">

                <div class="modal-header">

                    <h5>Employee Details</h5>

                    <button type="button"                        class="btn-close"                        data-bs-dismiss="modal">                    </button>

                </div>

                <div class="modal-body">

                    <p>
                        <b>Mobile:</b>
                        <span id="lblMobile"></span>
                    </p>

                    <p>
                        <b>Email:</b>
                        <span id="lblEmail"></span>
                    </p>

                    <p>
                        <b>Company:</b>
                        <span id="lblCompany"></span>
                    </p>

                    <p>
                        <b>Designation:</b>
                        <span id="lblDesignation"></span>
                    </p>

                    <p>
                        <b>Posting Place:</b>
                        <span id="lblPosting"></span>
                    </p>

                </div>

            </div>

        </div>

    </div>
    

     <div class="modal fade" id="overallResponseModal">

        <div class="modal-dialog modal-xl">

            <div class="modal-content">

                <div class="modal-header">

                    <h5>Overall Response</h5>

                    <button type="button"
                        class="btn-close"
                        data-bs-dismiss="modal">
                    </button>

                </div>

                <div class="modal-body">

                    <div class="mb-2 text-end">

                        <asp:Button
                            ID="btnOverallResponse"
                            runat="server"
                            Text="Export Excel"
                            CssClass="btn btn-success"
                            OnClick="btnOverallResponse_Click" />

                    </div>

                    <div style="overflow: auto">

                        <asp:GridView
                            ID="gvOverallResponse"
                            runat="server"
                            CssClass="table table-bordered">
                        </asp:GridView>

                    </div>

                </div>

            </div>

        </div>

    </div>

    <script>

function toggleMultiSelect(id)
             {
                 document.getElementById(id)
                     .classList.toggle("active");
             }

             document.addEventListener(
                 "click",
                 function (e) {

                     let x =
                         document.getElementsByClassName(
                             "multiselect-container");

                     for (let i = 0; i < x.length; i++) {
                         if (!x[i].contains(e.target)) {
                             x[i].classList.remove("active");
                         }
                     }

                 });

             function filterCheckbox(
                 containerId,
                 text) {
                 text = text.toLowerCase();

                 var container =
                     document.getElementById(
                         containerId);

                 var labels =
                     container.querySelectorAll(
                         "label");

                 for (let i = 0; i < labels.length; i++) {
                     var val =
                         labels[i].innerText.toLowerCase();

                     labels[i]
                         .parentElement
                         .style.display =
                         val.indexOf(text) > -1
                             ? ""
                             : "none";
                 }
             }

             function showDetails(
                 mobile,
                 email,
                 company,
                 designation,
                 posting) {
                 document.getElementById("lblMobile").innerHTML = mobile;
                 document.getElementById("lblEmail").innerHTML = email;
                 document.getElementById("lblCompany").innerHTML = company;
                 document.getElementById("lblDesignation").innerHTML = designation;
                 document.getElementById("lblPosting").innerHTML = posting;

                 var m =
                     new bootstrap.Modal(
                         document.getElementById('empModal'));

                 m.show();
             }

    </script>

    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</asp:Content>
