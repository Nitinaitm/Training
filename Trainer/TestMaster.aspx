<%@ Page Language="C#"
    AutoEventWireup="true"
    MasterPageFile="~/TrainerMaster.Master"
    CodeBehind="TestMaster.aspx.cs"
    Inherits="Training.Trainer.TestMaster" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>
        .page-title {
            font-size: 28px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 20px;
        }

        .card-box {
            background: #ffffff;
            border-radius: 10px;
            box-shadow: 0 0 8px #d8d8d8;
            padding: 20px;
            margin-bottom: 20px;
        }

        .section-title {
            font-size: 18px;
            font-weight: bold;
            color: #0d6efd;
            margin-bottom: 15px;
        }
    </style>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="page-title">
            Test Master

        </div>

        <div class="card-box">

            <div class="section-title">
                Create Test

            </div>

            <div class="row">

                <div class="col-md-3">

                    <label>Training *</label>

                    <asp:DropDownList
                        ID="ddlTraining"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlTraining_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3">

                    <label>Session *</label>

                    <asp:DropDownList
                        ID="ddlSession"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlSession_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3">

                    <label>Topic *</label>

                    <asp:DropDownList
                        ID="ddlTopic"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3">

                    <label>Test Type *</label>

                    <asp:DropDownList
                        ID="ddlTestType"
                        runat="server"
                        CssClass="form-select">

                        <asp:ListItem Value="Pre-Test">
Pre-Test
                        </asp:ListItem>

                        <asp:ListItem Value="Post-Test">
Post-Test
                        </asp:ListItem>

                        <asp:ListItem Value="Practice">
Practice
                        </asp:ListItem>

                    </asp:DropDownList>

                </div>

            </div>

            <br />

            <div class="row">

                <div class="col-md-6">

                    <label>Test Title *</label>

                    <asp:TextBox
                        ID="txtTestTitle"
                        runat="server"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <label>Duration (Min)</label>

                    <asp:TextBox
                        ID="txtDuration"
                        runat="server"
                        Text="30"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <label>Total Questions</label>

                    <asp:TextBox
                        ID="txtTotalQuestion"
                        runat="server"
                        Text="20"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <label>Total Marks</label>

                    <asp:TextBox
                        ID="txtTotalMarks"
                        runat="server"
                        Text="20"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

            </div>

            <br />

            <div class="row">

                <div class="col-md-2">

                    <label>Passing %</label>

                    <asp:TextBox
                        ID="txtPassingPercentage"
                        runat="server"
                        Text="40"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <label>Passing Marks</label>

                    <asp:TextBox
                        ID="txtPassingMarks"
                        runat="server"
                        Text="8"
                        CssClass="form-control">
                    </asp:TextBox>

                </div>

                <div class="col-md-2">

                    <br />

                    <asp:CheckBox
                        ID="chkRandomQuestion"
                        runat="server"
                        Text="Random Questions" />

                </div>

                <div class="col-md-2">

                    <br />

                    <asp:CheckBox
                        ID="chkShuffleOption"
                        runat="server"
                        Text="Shuffle Options" />

                </div>

                <div class="col-md-2">

                    <br />

                    <asp:CheckBox
                        ID="chkPublished"
                        runat="server"
                        Text="Published" />

                </div>

                <div class="col-md-2">

                    <br />

                    <asp:CheckBox
                        ID="chkShowResult"
                        runat="server"
                        Text="Show Result" />

                </div>

            </div>

            <br />

            <asp:HiddenField
                ID="hfTestID"
                runat="server" />

            <asp:Button
                ID="btnSave"
                runat="server"
                Text="Save"
                CssClass="btn btn-success"
                OnClick="btnSave_Click" />

            <asp:Button
                ID="btnClear"
                runat="server"
                Text="Clear"
                CssClass="btn btn-warning"
                OnClick="btnClear_Click" />

            <asp:Button
                ID="btnAssignQuestion"
                runat="server"
                Text="Assign Questions"
                CssClass="btn btn-primary"
                Enabled="false" />

            <br />
            <br />

            <asp:Label
                ID="lblMessage"
                runat="server"
                Font-Bold="true">
            </asp:Label>

        </div>

        <div class="card-box">

            <div class="section-title">
                Test List

            </div>

            <asp:GridView
                ID="gvTest"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                AllowPaging="true"
                PageSize="20"
                DataKeyNames="TestID"
                OnRowCommand="gvTest_RowCommand"
                OnPageIndexChanging="gvTest_PageIndexChanging">

                <Columns>

                    <asp:BoundField
                        DataField="TestID"
                        HeaderText="Test ID" />

                    <asp:BoundField
                        DataField="TrainingName"
                        HeaderText="Training" />

                    <asp:BoundField
                        DataField="SessionName"
                        HeaderText="Session" />

                    <asp:BoundField
                        DataField="TopicName"
                        HeaderText="Topic" />

                    <asp:BoundField
                        DataField="TestType"
                        HeaderText="Type" />

                    <asp:BoundField
                        DataField="TestTitle"
                        HeaderText="Title" />

                    <asp:BoundField
                        DataField="Duration"
                        HeaderText="Minutes" />

                    <asp:BoundField
                        DataField="TotalQuestions"
                        HeaderText="Questions" />

                    <asp:BoundField
                        DataField="PassingPercentage"
                        HeaderText="%" />

                    <asp:TemplateField>

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkEdit"
                                runat="server"
                                Text="Edit"
                                CommandName="EditTest"
                                CommandArgument='<%# Eval("TestID") %>' />

                            &nbsp;

                            <asp:LinkButton
                                ID="lnkDelete"
                                runat="server"
                                Text="Delete"
                                CommandName="DeleteTest"
                                CommandArgument='<%# Eval("TestID") %>'
                                OnClientClick="return confirm('Delete this Test ?');" />

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

</asp:Content>
