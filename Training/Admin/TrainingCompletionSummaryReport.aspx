<%@ Page Title="" Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TrainingCompletionSummaryReport.aspx.cs"
    Inherits="Training.Admin.TrainingCompletionSummaryReport" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">
      <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
      rel="stylesheet" />
<style>

.main-container{
    padding:20px;
}

.report-card{
    background:#fff;
    padding:20px;
    border-radius:10px;
    box-shadow:0 2px 10px rgba(0,0,0,.08);
    margin-bottom:20px;
}

.page-title{
    font-size:28px;
    font-weight:600;
    margin-bottom:20px;
    color:#003366;
}

.search-grid{
    display:grid;
    grid-template-columns:repeat(4,1fr);
    gap:15px;
}

.form-group{
    display:flex;
    flex-direction:column;
}

.form-group label{
    font-weight:600;
    margin-bottom:6px;
}

.textbox,
select{
    width:100%;
    padding:10px;
    border:1px solid #ccc;
    border-radius:6px;
}

.button-container{
    display:flex;
    justify-content:center;
    gap:15px;
    margin-top:20px;
    flex-wrap:wrap;
}

.button-container input[type=submit]{
    min-width:180px;
    height:50px;
    font-weight:bold;
}

.grid-container{
    width:100%;
    overflow:auto;
    max-height:650px;
    border:1px solid #ddd;
}

.gridview{
    width:100%;
    border-collapse:collapse;
}

.gridview th{
    background:#0d6efd;
    color:#fff;
    padding:10px;
    white-space:nowrap;
    position:sticky;
    top:0;
    z-index:10;
}

.gridview td{
    padding:8px;
    border:1px solid #ddd;
    white-space:nowrap;
}

@media(max-width:992px)
{
    .search-grid{
        grid-template-columns:repeat(2,1fr);
    }
}

@media(max-width:576px)
{
    .search-grid{
        grid-template-columns:1fr;
    }

    .button-container{
        flex-direction:column;
        align-items:center;
    }
}

</style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

<div class="main-container">

<div class="report-card">

<div class="page-title">
Training Completion Summary Report
</div>

<div class="search-grid">

    <div class="form-group">
        <label>Company</label>

        <asp:DropDownList
            ID="ddlCompany"
            runat="server"
            CssClass="textbox">
        </asp:DropDownList>
    </div>

    <div class="form-group">
        <label>Training Type</label>

        <asp:DropDownList
            ID="ddlTrainingType"
            runat="server"
            CssClass="textbox">
        </asp:DropDownList>
    </div>

    <div class="form-group">
        <label>Date From</label>

        <asp:TextBox
            ID="txtDateFrom"
            runat="server"
            TextMode="Date"
            CssClass="textbox">
        </asp:TextBox>
    </div>

    <div class="form-group">
        <label>Date To</label>

        <asp:TextBox
            ID="txtDateTo"
            runat="server"
            TextMode="Date"
            CssClass="textbox">
        </asp:TextBox>
    </div>

</div>

<div class="button-container">

    <asp:Button
        ID="btnShowReport"
        runat="server"
        Text="Show Report"
        CssClass="btn btn-primary"
        OnClick="btnShowReport_Click" />

    <asp:Button
        ID="btnExportExcel"
        runat="server"
        Text="Export Excel"
        CssClass="btn btn-success"
        OnClick="btnExportExcel_Click" />

    <asp:Button
        ID="btnDownloadPDF"
        runat="server"
        Text="Download PDF"
        CssClass="btn btn-danger"
        OnClick="btnDownloadPDF_Click" />

</div>

</div>

<div class="report-card">

    <asp:Label
        ID="lblTotalDesignation"
        runat="server"
        Font-Bold="true"
        Font-Size="Large">
    </asp:Label>

    <br /><br />

    <div class="grid-container">

        <asp:GridView
            ID="gvReport"
            runat="server"
            AutoGenerateColumns="true"
            CssClass="gridview">
        </asp:GridView>

    </div>

</div>

</div>

</asp:Content>