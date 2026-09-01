<%@ Page Title="" Language="C#" MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true" MaintainScrollPositionOnPostback="true"
    CodeBehind="TrainingDetailsReport.aspx.cs"
    Inherits="Training.Admin.TrainingDetailsReport" EnableEventValidation="false" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js">
    </script>
    <link rel="stylesheet"
href="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/1.1.2/css/bootstrap-multiselect.min.css" />

<script src="https://cdnjs.cloudflare.com/ajax/libs/bootstrap-multiselect/1.1.2/js/bootstrap-multiselect.min.js"></script>
    <style>
        .main-container {
            padding: 20px;
        }

        .search-card,
        .grid-card {
            background: #fff;
            padding: 25px;
            border-radius: 12px;
            margin-bottom: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
        }

        .page-title {
            font-size: 28px;
            font-weight: 600;
            margin-bottom: 25px;
            color: darkcyan;
        }

        .search-grid {
            display: grid;
            grid-template-columns:repeat(5,1fr);
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

        .btn-export {
            background: #198754;
            color: white;
        }

        .gridview {
            width: 100%;
            min-width: 1200px;
        }

            .gridview th {
                background: #0d6efd;
                color: white;
                padding: 12px;
            }

            .gridview td {
                padding: 10px;
            }

        @media(max-width:992px) {
            .search-grid {
                grid-template-columns: repeat(2,1fr);
            }
        }

        @media(max-width:576px) {
            .search-grid {
                grid-template-columns: 1fr;
            }
        }

        .multiselect{
    width:100% !important;
    text-align:left !important;
}

.multiselect-container{
    max-height:300px;
    overflow-y:auto;
}
    </style>

</asp:Content>


<asp:Content ID="Content2"  ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="main-container">

        <div class="search-card">

            <div class="page-title">
                Training Report

            </div>


            <div class="search-grid">

                <div class="form-group">
                    <label>Training ID</label>
                    <asp:textbox id="txtTrainingID"
                        runat="server"
                        cssclass="textbox" />
                </div>


                <div class="form-group">
                    <label>Batch</label>
                    <asp:textbox id="txtBatch"
                        runat="server"
                        cssclass="textbox" />
                </div>


                <div class="form-group">
                    <label>Date From</label>
                    <asp:textbox id="txtDateFrom"
                        runat="server"
                        textmode="Date"
                        cssclass="textbox" />
                </div>


                <div class="form-group">
                    <label>Date To</label>
                    <asp:textbox id="txtDateTo"
                        runat="server"
                        textmode="Date"
                        cssclass="textbox" />
                </div>


                <div class="form-group">
                    <label>Training Type</label>

                    <asp:dropdownlist id="ddlType"
                        runat="server"
                        cssclass="textbox"
                        autopostback="true"
                        onselectedindexchanged="ddlType_SelectedIndexChanged">
</asp:dropdownlist>

                </div>


                <div class="form-group">

                    <label>Organizer</label>

                    <asp:dropdownlist id="ddlOrganizer"
                        runat="server"
                        cssclass="textbox"
                        autopostback="true"
                        onselectedindexchanged="ddlOrganizer_SelectedIndexChanged">
</asp:dropdownlist>

                </div>



                <div class="form-group">

                    <label>Location</label>

                    <asp:dropdownlist id="ddlLocation"
                        runat="server"
                        cssclass="textbox">
</asp:dropdownlist>

                </div>
                <div class="form-group">

    <label>Company</label>

    <div style="height:150px;
                overflow-y:auto;
                border:1px solid #ccc;
                border-radius:8px;
                padding:10px;
                background:#fff;">

        <asp:CheckBoxList
            ID="chkCompany"
            runat="server"
            RepeatDirection="Vertical">
        </asp:CheckBoxList>

    </div>

</div>
<div class="form-group">

<label>

Designation

</label>

<asp:TextBox
ID="txtDesignationSearch"
runat="server"
CssClass="textbox"
placeholder="Search Designation..."
onkeyup="filterDesignation()">
</asp:TextBox>

<div id="designationContainer"
style="height:220px;overflow-y:auto;border:1px solid #ccc;border-radius:8px;padding:10px;background:#fff;">

<asp:CheckBoxList
ID="chkDesignation"
runat="server"
RepeatDirection="Vertical">
</asp:CheckBoxList>

</div>

</div>
            </div>


            <div class="button-container">

                <asp:button id="btnSearch"
                    runat="server"
                    text="Search"
                    cssclass="btn btn-search"
                    onclick="btnSearch_Click" />

                <asp:button id="btnReset"
                    runat="server"
                    text="Reset"
                    cssclass="btn btn-reset"
                    onclick="btnReset_Click" />

                <asp:button id="btnExport"
                    runat="server"
                    text="Export Excel"
                    cssclass="btn btn-export"
                    onclick="btnExport_Click" />

            </div>

        </div>

      <asp:Label
ID="lblTotalEmployee"
runat="server"
Font-Bold="true"
Font-Size="Large"
ForeColor="DarkGreen">
</asp:Label>

        <div class="grid-card">

            <div style="overflow: auto">

                <asp:gridview id="gvTraining"
                    runat="server"
                    autogeneratecolumns="false"
                    cssclass="table table-bordered gridview"
                    onrowcommand="gvTraining_RowCommand">

<Columns>

<asp:TemplateField HeaderText="Sl No">
<ItemTemplate>
<%# Container.DataItemIndex+1 %>
</ItemTemplate>
</asp:TemplateField>


<asp:BoundField DataField="TrainingID"
HeaderText="Training ID"/>

<asp:BoundField DataField="TrainingType"
HeaderText="Training Type"/>

<asp:BoundField DataField="TrainingOrganizer"
HeaderText="Organizer"/>

<asp:BoundField DataField="TrainingLocation"
HeaderText="Location"/>

<asp:BoundField DataField="Batch"
HeaderText="Batch"/>

<asp:BoundField DataField="DateFrom"
HeaderText="Date From"/>

<asp:BoundField DataField="DateTo"
HeaderText="Date To"/>


<%--<asp:TemplateField HeaderText="Assigned">

<ItemTemplate>

<asp:LinkButton ID="lnkAssigned"
runat="server"
Text='<%# Eval("TotalAssigned") %>'
CommandName="Assigned"
CommandArgument='<%# Eval("TrainingID") %>'/>

</ItemTemplate>

</asp:TemplateField>--%>
    <asp:TemplateField HeaderText="Filtered Assigned">

<ItemTemplate>

<asp:LinkButton
ID="lnkFilteredAssigned"
runat="server"
Text='<%# Eval("TotalAssigned") %>'
CommandName="FilteredAssigned"
CommandArgument='<%# Eval("TrainingID") %>' />

</ItemTemplate>

</asp:TemplateField>

<asp:TemplateField HeaderText="Total Assigned">

<ItemTemplate>

<asp:LinkButton
ID="lnkTotalAssigned"
runat="server"
Text='<%# Eval("TotalBatchStrength") %>'
CommandName="TotalAssigned"
CommandArgument='<%# Eval("TrainingID") %>' />

</ItemTemplate>

</asp:TemplateField>


<asp:TemplateField HeaderText="Attended">

<ItemTemplate>

<asp:LinkButton ID="lnkAttended"
runat="server"
Text='<%# Eval("TotalAttended") %>'
CommandName="Attended"
CommandArgument='<%# Eval("TrainingID") %>'/>

</ItemTemplate>

</asp:TemplateField>

</Columns>

</asp:gridview>

            </div>

        </div>

    </div>


    <div class="modal fade"
        id="empModal">

        <div class="modal-dialog modal-xl">

            <div class="modal-content">

                <div class="modal-header">

                    <h5>Employee Details
                    </h5>

                    <button type="button"
                        class="btn-close"
                        data-bs-dismiss="modal">
                    </button>

                </div>


                <div class="modal-body">

                    <div class="mb-2 text-end">

                        <asp:button id="btnPopupExport"
                            runat="server"
                            text="Export Excel"
                            cssclass="btn btn-success"
                            onclick="btnPopupExport_Click" />

                    </div>


                    <div style="overflow: auto">

                        <asp:gridview id="gvEmployeeDetails"
                            runat="server"
                            cssclass="table table-bordered">
</asp:gridview>

                    </div>

                </div>

            </div>

        </div>

    </div>

   <script>

function filterDesignation() {

    var input = document.getElementById('<%= txtDesignationSearch.ClientID %>');
    var filter = input.value.toUpperCase();

    var container = document.getElementById('designationContainer');
    var labels = container.getElementsByTagName('label');

    for (var i = 0; i < labels.length; i++) {

        var txt = labels[i].innerText || labels[i].textContent;
        var row = labels[i].parentElement;

        if (txt.toUpperCase().indexOf(filter) > -1)
            row.style.display = "";
        else
            row.style.display = "none";
    }
}

</script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</asp:Content>
