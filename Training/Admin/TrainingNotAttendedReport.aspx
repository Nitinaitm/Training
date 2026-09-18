<%@ Page Title="" Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingNotAttendedReport.aspx.cs"
    Inherits="Training.Admin.TrainingNotAttendedReport" %>

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
    color: #1e293b;
}

.search-grid {
    display: grid;
    grid-template-columns: repeat(4,1fr);
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

/*.button-container {
    display: flex;
    gap: 10px;
    margin-top: 20px;
    flex-wrap: wrap;
}*/
.button-container {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 15px;
    margin-top: 20px;
    margin-bottom: 20px;
    flex-wrap: wrap;
}

.button-container .btn,
.button-container input[type=submit] {
    min-width: 180px;
    height: 50px;
    font-size: 16px;
    font-weight: 600;
    border-radius: 8px;
}
.btn-search {
    background: #0d6efd;
    color: white;
}

.btn-reset {
    background: #6c757d;
    color: white;
}
.button-summary-row {
    display: flex;
    width: 100%;
    gap: 15px;
    margin-bottom: 20px;
}

.button-summary-row .summary-btn {
    flex: 1;
    min-height: 90px;
    border-radius: 12px;
    font-size: 18px;
    font-weight: 700;
    text-align: center;
    white-space: normal;
    padding: 10px;
    transition: all 0.3s ease;
}

.button-summary-row .summary-btn:hover {
    transform: translateY(-3px);
}
.summary-box {
    display: flex;
    gap: 15px;
    flex-wrap: wrap;
    margin-bottom: 20px;
}

.summary-card {
    min-width: 220px;
    background: #f8fafc;
    border-left: 5px solid #0d6efd;
    padding: 15px;
    border-radius: 8px;
}

.summary-card h4 {
    margin: 0;
    color: #0d6efd;
}

.summary-card span {
    font-size: 22px;
    font-weight: bold;
}

.table-responsive {
    width: 100%;
    overflow-x: auto;
}
.grid-responsive {
    width: 100%;
    max-height: 600px;
    overflow: auto;
    border: 1px solid #ddd;
    border-radius: 8px;
}

.gridview {
    min-width: 1400px;
    width: max-content;
}


.gridview th {
    white-space: nowrap;
    text-align: center;
}

/*.gridview {
    width: 100%;
    border-collapse: collapse;
}*/

.gridview th {
    background: #0d6efd;
    color: white;
    padding: 10px;
    white-space: nowrap;
    text-align: center;
}

.gridview td {
    padding: 8px;
    border: 1px solid #ddd;
    white-space: nowrap;
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
    width: 350px;
    border: 1px solid #ccc;
    max-height: 350px;
    overflow-y: auto;
    overflow-x: auto;
    z-index: 99999;
    padding: 10px;
}

.multiselect-container.active .multiselect-content {
    display: block;
}

.multiselect-search {
    width: 100%;
    padding: 8px;
    border: 1px solid #ccc;
    border-radius: 5px;
    margin-bottom: 8px;
}

.multiselect-content table {
    width: 100%;
}

.multiselect-content td {
    white-space: nowrap;
}
@media(max-width:768px)
{
    .button-container {
        flex-direction: column;
    }

    .button-container .btn,
    .button-container input[type=submit] {
        width: 100%;
        max-width: 300px;
    }
}
@media(max-width:1200px)
{
    .search-grid {
        grid-template-columns: repeat(3,1fr);
    }
}

@media(max-width:768px)
{
    .search-grid {
        grid-template-columns: repeat(2,1fr);
    }
}

@media(max-width:576px)
{
    .search-grid {
        grid-template-columns: 1fr;
    }
}

</style>

<script>

function toggleMultiSelect(id)
{
    document.getElementById(id)
    .classList.toggle("active");
}

document.addEventListener(
"click",
function(e)
{
    let x =
    document.getElementsByClassName(
    "multiselect-container");

    for(let i=0;i<x.length;i++)
    {
        if(!x[i].contains(e.target))
        {
            x[i].classList.remove("active");
        }
    }
});

function filterCheckbox(
containerId,
text)
{
    text =
    text.toLowerCase();

    var container =
    document.getElementById(
    containerId);

    var rows =
    container.querySelectorAll(
    "table tr");

    for(let i=0;i<rows.length;i++)
    {
        var val =
        rows[i].textContent
        .toLowerCase();

        rows[i].style.display =
        val.indexOf(text) > -1
        ? ""
        : "none";
    }
}

</script>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="main-container">

<div class="search-card">

<div class="page-title">
Employees Not Attended Training Report
</div>
    <div class="search-grid">

    <div class="form-group">

        <label>
            Batch
        </label>

        <asp:TextBox
            ID="txtBatch"
            runat="server"
            CssClass="textbox">
        </asp:TextBox>

    </div>

    <div class="form-group">

        <label>
            Date From
        </label>

        <asp:TextBox
            ID="txtDateFrom"
            runat="server"
            TextMode="Date"
            CssClass="textbox">
        </asp:TextBox>

    </div>

    <div class="form-group">

        <label>
            Date To
        </label>

        <asp:TextBox
            ID="txtDateTo"
            runat="server"
            TextMode="Date"
            CssClass="textbox">
        </asp:TextBox>

    </div>

    <div class="form-group">

        <label>
            Training Type
        </label>

        <asp:DropDownList
            ID="ddlTrainingType"
            runat="server"
            CssClass="form-control"
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlTrainingType_SelectedIndexChanged">
        </asp:DropDownList>

    </div>

    <div class="form-group">

        <label>
            Organizer
        </label>

        <asp:DropDownList
            ID="ddlOrganizer"
            runat="server"
            CssClass="form-control"
            AutoPostBack="true"
            OnSelectedIndexChanged="ddlOrganizer_SelectedIndexChanged">
        </asp:DropDownList>

    </div>

    <div class="form-group">

        <label>
            Location
        </label>

        <asp:DropDownList
            ID="ddlLocation"
            runat="server"
            CssClass="form-control">
        </asp:DropDownList>

    </div>


    <div class="form-group">

        <label>
            Company
        </label>

        <div class="multiselect-container"
            id="companyBox">

            <div class="multiselect-header"
                onclick="toggleMultiSelect('companyBox')">

                Select Company

            </div>

            <div class="multiselect-content">

                <asp:CheckBoxList
                    ID="chkCompany"
                    runat="server">
                </asp:CheckBoxList>

            </div>

        </div>

    </div>


    <div class="form-group">

        <label>
            Designation
        </label>

        <div class="multiselect-container"
            id="designationBox">

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


   

</div>


<div class="button-container">

    <asp:Button
        ID="btnSearch"
        runat="server"
        Text="Search"
        CssClass="btn btn-search"
        OnClick="btnSearch_Click" />

    <asp:Button
        ID="btnExport"
        runat="server"
        Text="Export Excel"
        CssClass="btn btn-success"
        OnClick="btnExport_Click" />

    <asp:Button
        ID="btnReset"
        runat="server"
        Text="Reset"
        CssClass="btn btn-reset"
        OnClick="btnReset_Click" />

</div>

</div>
    <div class="grid-card">
<div class="button-summary-row">

    <asp:Button
        ID="btnTotalEmployees"
        runat="server"
        Text="Total Employees (0)"
        CssClass="btn btn-primary summary-btn"
        OnClick="btnTotalEmployees_Click" />

    <asp:Button
        ID="btnNeverAttendedEver"
        runat="server"
        Text="Never Attended Ever (0)"
        CssClass="btn btn-danger summary-btn"
        OnClick="btnNeverAttendedEver_Click" />

    <asp:Button
        ID="btnNeverAttendedSelected"
        runat="server"
        Text="Never Attended Selected Training (0)"
        CssClass="btn btn-warning summary-btn"
        OnClick="btnNeverAttendedSelected_Click" />

    <asp:Button
        ID="btnAttendedSelected"
        runat="server"
        Text="Attended Selected Training (0)"
        CssClass="btn btn-success summary-btn"
        OnClick="btnAttendedSelected_Click" />

</div>

<div style="margin-bottom:10px;">

    <asp:Label
        ID="lblCurrentView"
        runat="server"
        Font-Bold="true"
        Font-Size="Large"
        ForeColor="#003366">
    </asp:Label>

    <br />

    <asp:Label
        ID="Label1"
        runat="server"
        Font-Bold="true"
        ForeColor="Green">
    </asp:Label>

</div>

        <div style="margin-bottom:10px;">

    <asp:Label
        ID="lblRecordCount"
        runat="server"
        Font-Bold="true"
        ForeColor="Green">
    </asp:Label>

</div>
    <div class="table-responsive">
<div class="grid-responsive">
        <asp:GridView
            ID="gvReport"
            runat="server"
            AutoGenerateColumns="False"
            CssClass="gridview table table-bordered">

            <Columns>

                <asp:TemplateField
                    HeaderText="Sl No">

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
                    DataField="EmpCompany"
                    HeaderText="Company" />

                <asp:BoundField
                    DataField="EmpDesignation"
                    HeaderText="Designation" />

                <asp:BoundField
                    DataField="EmpPostingPlace"
                    HeaderText="Posting Place" />

                <asp:BoundField
                    DataField="MobileNo"
                    HeaderText="Mobile" />

                <asp:BoundField
    DataField="EmailId"
    HeaderText="Email" />

                <asp:BoundField
                    DataField="Status"
                    HeaderText="Status" />

                <asp:BoundField
                    DataField="TotalTrainingsAttended"
                    HeaderText="Total Training Attended" />

               

            </Columns>

        </asp:GridView>
    </div>
    </div>

</div>

</div>
    
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

</asp:Content>