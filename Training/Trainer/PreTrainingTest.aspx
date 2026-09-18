<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PreTrainingTest.aspx.cs"
    Inherits="Training.Trainer.PreTrainingTest"
    MasterPageFile="~/TrainerMaster.Master" %>

<%@ Register Src="~/Trainer/SessionSummary.ascx"
    TagPrefix="uc2"
    TagName="SessionSummary" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style>
        .card {
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,.15);
            margin-bottom: 20px;
        }

        .card-header {
            font-weight: bold;
            font-size: 18px;
        }

        .form-label {
            font-weight: 600;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">
           <div class="card-header bg-success text-white">
       Pre Training Test
          

       </div>

       

       

        <uc2:sessionsummary id="SessionSummary1" runat="server" />
        
        <br />
      <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Session

                        </label>

                        <asp:Label
                            ID="lblSession"
                            runat="server"
                            CssClass="form-control" Visible="false">
                        </asp:Label>

                    </div>
        <!-- Configuration -->

        <div class="card">

            <div class="card-header bg-success text-white">
                Test Configuration

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-4 mb-3">

                        <label class="form-label">
                            Test Title

                        </label>

                        <asp:TextBox
                            ID="txtTestTitle"
                            runat="server"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label class="form-label">
                            Duration

                        </label>

                        <asp:TextBox
                            ID="txtDuration"
                            runat="server"
                            Text="30"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label class="form-label">
                            Questions

                        </label>

                        <asp:TextBox
                            ID="txtTotalQuestions"
                            runat="server"
                            Text="20"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label class="form-label">
                            Marks

                        </label>

                        <asp:TextBox
                            ID="txtMarks"
                            runat="server"
                            Text="1"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2 mb-3">

                        <label class="form-label">
                            Passing %

                        </label>

                        <asp:TextBox
                            ID="txtPassing"
                            runat="server"
                            Text="40"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>

                <div class="row">

                    <div class="col-md-3">

                        <asp:CheckBox
                            ID="chkRandom"
                            runat="server"
                            Checked="true"
                            Text=" Random Questions"  AutoPostBack="true"
    OnCheckedChanged="chkRandom_CheckedChanged" />

                    </div>

                    <div class="col-md-3">

                        <asp:CheckBox
                            ID="chkShuffle"
                            runat="server"
                            Checked="true"
                            Text=" Shuffle Options" />

                    </div>

                    <div class="col-md-3">

                        <asp:CheckBox
                            ID="chkAllowRetest"
                            runat="server"
                            Text=" Allow Retest" />

                    </div>

                    <div class="col-md-3">

                        <label>
                            Max Attempt

                        </label>

                        <asp:TextBox
                            ID="txtAttempt"
                            runat="server"
                            Text="1"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                </div>

            </div>

        </div>

        <!-- Question Distribution -->

        <div class="card">

            <div class="card-header bg-warning">
                Random Question Distribution

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-2">

                        <label>
                            Easy

                        </label>

                        <asp:TextBox
                            ID="txtEasy"
                            runat="server"
                            Text="5"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2">

                        <label>
                            Medium

                        </label>

                        <asp:TextBox
                            ID="txtMedium"
                            runat="server"
                            Text="10"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-2">

                        <label>
                            Hard

                        </label>

                        <asp:TextBox
                            ID="txtHard"
                            runat="server"
                            Text="5"
                            CssClass="form-control">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label>
                            Question Pool

                        </label>

                        <asp:Label
                            ID="lblPool"
                            runat="server"
                            CssClass="form-control">
                        </asp:Label>

                    </div>

                    <div class="col-md-3">

                        <br />

                        <asp:Button
                            ID="btnGenerateQuestions"
                            runat="server"
                            Text="Generate Questions"
                            CssClass="btn btn-warning w-100" OnClick="btnGenerateQuestions_Click" />

                    </div>

                </div>

            </div>

        </div>

        <!-- Grid -->

        <div class="card">

            <div class="card-header bg-info text-white">
                Selected Questions

            </div>

            <div class="card-body">

                <asp:GridView
                    ID="gvQuestion"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="table table-bordered"
                    DataKeyNames="QuestionID,Question,DifficultyLevel,Marks,QuestionOwnerType"
                    OnRowDataBound="gvQuestion_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="Select">

                            <HeaderTemplate>

                                <asp:CheckBox
                                    ID="chkAll"
                                    runat="server"
                                    AutoPostBack="true"
                                    OnCheckedChanged="chkAll_CheckedChanged" />

                            </HeaderTemplate>

                            <ItemTemplate>

                                <asp:CheckBox
                                    ID="chkSelect"
                                    runat="server" />

                                <asp:HiddenField
                                    ID="hfQuestionID"
                                    runat="server"
                                    Value='<%# Eval("QuestionID") %>' />

                            </ItemTemplate>

                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="#">
                            <ItemTemplate>
                                <asp:Label
                                    ID="lblSlNo"
                                    runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField
                            DataField="Question"
                            HeaderText="Question" />

                        <asp:BoundField
                            DataField="DifficultyLevel"
                            HeaderText="Difficulty" />

                        <asp:BoundField
                            DataField="Marks"
                            HeaderText="Marks" />

                        <asp:BoundField
                            DataField="QuestionOwnerType"
                            HeaderText="Owner" />



                    </Columns>

                </asp:GridView>

            </div>

        </div>

        <div class="text-end mb-5">

            <asp:Button
                ID="btnSaveDraft"
                runat="server"
                Text="Save Draft"
                CssClass="btn btn-success" OnClick="btnSaveDraft_Click" />

            <asp:Button
                ID="btnPublish"
                runat="server"
                Text="Publish Test"
                CssClass="btn btn-primary" OnClick="btnPublish_Click" />

            <asp:Button
                ID="btnBack"
                runat="server"
                Text="Back"
                CssClass="btn btn-primary" OnClick="btnBack_Click" />

        </div>

    </div>

</asp:Content>
