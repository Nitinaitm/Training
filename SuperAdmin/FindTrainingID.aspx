<%@ Page Title="" Language="C#"
    MasterPageFile="~/SuperAdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="FindTrainingID.aspx.cs"
    Inherits="Training.SuperAdmin.FindTrainingID" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

<style>

.page-card {
    background: #fff;
    padding: 20px;
    border-radius: 10px;
    box-shadow: 0 2px 10px rgba(0,0,0,.1);
    margin-top: 20px;
}

.page-title {
    font-size: 28px;
    font-weight: 600;
    color: #0d6efd;
    margin-bottom: 20px;
}

.gridview th {
    background: #0d6efd;
    color: white;
    text-align: center;
}

.gridview td {
    vertical-align: middle;
}

.checkbox-container {
    border: 1px solid #ddd;
    height: 220px;
    overflow-y: auto;
    padding: 10px;
    border-radius: 5px;
    background: #fff;
}

</style>

<script type="text/javascript">

function filterDesignation()
{
    var filter =
        document.getElementById(
        "txtSearchDesignation")
        .value.toUpperCase();

    var container =
        document.getElementById(
        "designationContainer");

    var rows =
        container.getElementsByTagName(
        "tr");

    for (var i = 0; i < rows.length; i++)
    {
        var txt =
            rows[i].innerText ||
            rows[i].textContent;

        if (txt.toUpperCase()
            .indexOf(filter) > -1)
        {
            rows[i].style.display = "";
        }
        else
        {
            rows[i].style.display = "none";
        }
    }
}

</script>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="container-fluid">

    <div class="page-card">

        <div class="page-title">
            Find Training ID
        </div>

        <div class="row">

            <div class="col-md-3">

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

            <div class="col-md-3">

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

            <div class="col-md-3">

                <label>
                    Location
                </label>

                <asp:DropDownList
                    ID="ddlLocation"
                    runat="server"
                    CssClass="form-control">
                </asp:DropDownList>

            </div>

            <div class="col-md-3">

                <label>
                    Batch
                </label>

                <asp:TextBox
                    ID="txtBatch"
                    runat="server"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

        </div>

        <br />

        <div class="row">

                        <div class="col-md-3">

                <label>
                    Date From
                </label>

                <asp:TextBox
                    ID="txtDateFrom"
                    runat="server"
                    TextMode="Date"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

            <div class="col-md-3">

                <label>
                    Date To
                </label>

                <asp:TextBox
                    ID="txtDateTo"
                    runat="server"
                    TextMode="Date"
                    CssClass="form-control">
                </asp:TextBox>

            </div>

            <div class="col-md-6">

                <label>
                    Designation
                </label>

                <input type="text"
                    id="txtSearchDesignation"
                    class="form-control"
                    placeholder="Search Designation..."
                    onkeyup="filterDesignation();" />

                <br />

                <div id="designationContainer"
                    class="checkbox-container">

                    <asp:CheckBoxList
                        ID="chkDesignation"
                        runat="server"
                        RepeatLayout="Table"
                        RepeatColumns="1"
                        Width="100%">
                    </asp:CheckBoxList>

                </div>

            </div>

        </div>

        <br />

        <asp:Button
            ID="btnSearch"
            runat="server"
            Text="Search"
            CssClass="btn btn-primary"
            OnClick="btnSearch_Click" />

        <br />
        <br />

        <div class="table-responsive">

            <asp:GridView
                ID="gvTraining"
                runat="server"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-striped gridview">

                <Columns>

                    <asp:BoundField
                        DataField="TrainingID"
                        HeaderText="Training ID" />

                    <asp:BoundField
                        DataField="TrainingType"
                        HeaderText="Training Type" />

                    <asp:BoundField
                        DataField="TrainingOrganizer"
                        HeaderText="Organizer" />

                    <asp:BoundField
                        DataField="TrainingLocation"
                        HeaderText="Location" />

                    <asp:BoundField
                        DataField="Batch"
                        HeaderText="Batch" />
                                        <asp:BoundField
                        DataField="DateFrom"
                        HeaderText="Date From" />

                    <asp:BoundField
                        DataField="DateTo"
                        HeaderText="Date To" />

                    <asp:BoundField
    DataField="Designation"
    HeaderText="Designation">
    <ItemStyle Width="300px" />
</asp:BoundField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

</div>

</asp:Content>