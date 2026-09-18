<%@ Page Title="" Language="C#" MasterPageFile="~/TrainerMaster.Master" AutoEventWireup="true" CodeBehind="TraineeProgress.aspx.cs" Inherits="Training.Trainer.TraineeProgress" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        .page-heading {
            font-size: 28px;
            font-weight: bold;
            color: #198754;
            margin-bottom: 20px
        }

        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 0 10px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px
        }

        .progress-bar-custom {
            height: 20px;
            border-radius: 10px
        }

        .gridview th {
            background: #198754;
            color: white;
            text-align: center;
            vertical-align: middle
        }

        .gridview td {
            vertical-align: middle
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <div class="page-heading">Trainee Progress</div>
        <div class="dashboard-card">
            <div class="row">
                <div class="col-md-4">
                    <label>Training ID</label><asp:DropDownList ID="ddlTraining" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged" />
                </div>
                <div class="col-md-3">
                    <label>Search</label><asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Search by Name/EmpID" />
                </div>
                <div class="col-md-5">
                    <br />
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary ms-1" OnClick="btnReset_Click" />
                </div>
            </div>
            <asp:GridView ID="gvProgress" runat="server" AutoGenerateColumns="False" CssClass="table table-bordered table-hover gridview" EmptyDataText="No Trainees Found">
                <Columns>
                    <asp:TemplateField HeaderText="Sl No">
                        <ItemTemplate><%# Container.DataItemIndex + 1 %></ItemTemplate>
                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="EmpID" HeaderText="Employee ID" />
                    <asp:BoundField DataField="EmpName" HeaderText="Employee Name" />
                    <asp:BoundField DataField="TotalSessions" HeaderText="Total Sessions" />
                    <asp:BoundField DataField="Attended" HeaderText="Attended" />
                    <asp:BoundField DataField="Percentage" HeaderText="Attendance %" />
                    <asp:TemplateField HeaderText="Progress">
                        <ItemTemplate>
                            <div class="progress" style="height: 20px; border-radius: 10px;">
                                <div class='progress-bar <%# Convert.ToInt32(Eval("Percentage")) >= 80 ? "bg-success" : Convert.ToInt32(Eval("Percentage")) >= 50 ? "bg-warning" : "bg-danger" %>'
                                    style='width: <%# Eval("Percentage") + "%" %>;'>
                                    <%# Eval("Percentage") %>% 
                                </div>
                            </div>
                        </ItemTemplate>
                        <ItemStyle Width="200px" />
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
