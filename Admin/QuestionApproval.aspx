<%@ Page
    Title="Question Approval"
    Language="C#"
    MasterPageFile="~/AdminMaster.Master"
    AutoEventWireup="true"
    CodeBehind="QuestionApproval.aspx.cs"
    Inherits="Training.Admin.QuestionApproval"
    ClientIDMode="Static"
    MaintainScrollPositionOnPostback="true" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css"
        rel="stylesheet" />

    <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>

    <style>
        .card-box {
            background: #ffffff;
            border-radius: 12px;
            box-shadow: 0 0 8px #d9d9d9;
            padding: 20px;
            margin-bottom: 20px;
        }

        .page-title {
            font-size: 28px;
            font-weight: bold;
            color: darkcyan;
            margin-bottom: 20px;
        }

        .section-title {
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 15px;
            color: #444;
        }

        .select2-container {
            width: 100% !important;
        }
    </style>

</asp:Content>

<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="page-title">
            Question Approval

        </div>

        <div class="card-box">

            <div class="section-title">
                Search

            </div>

            <div class="row">

                <div class="col-md-3 mb-3">

                    <label>
                        Course

                    </label>

                    <asp:DropDownList
                        ID="ddlCourse"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlCourse_SelectedIndexChanged">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3 mb-3">

                    <label>
                        Topic

                    </label>

                    <asp:DropDownList
                        ID="ddlTopic"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3 mb-3">

                    <label>
                        Trainer

                    </label>

                    <asp:DropDownList
                        ID="ddlTrainer"
                        runat="server"
                        CssClass="form-select">
                    </asp:DropDownList>

                </div>

                <div class="col-md-3 mb-3">

                    <label>
                        Status

                    </label>

                    <asp:DropDownList
                        ID="ddlStatus"
                        runat="server"
                        CssClass="form-select">

                        <asp:ListItem Value="">
All
                        </asp:ListItem>

                        <asp:ListItem>
Pending
                        </asp:ListItem>

                        <asp:ListItem>
Approved
                        </asp:ListItem>

                        <asp:ListItem>
Rejected
                        </asp:ListItem>

                    </asp:DropDownList>

                </div>

            </div>

            <div class="row">

                <div class="col-md-12 text-end">

                    <asp:Button
                        ID="btnSearch"
                        runat="server"
                        Text="Search"
                        CssClass="btn btn-primary"
                        OnClick="btnSearch_Click" />

                    <asp:Button
                        ID="btnReset"
                        runat="server"
                        Text="Reset"
                        CssClass="btn btn-secondary"
                        OnClick="btnReset_Click" />

                </div>

            </div>

        </div>

        <div class="card-box">

            <div class="section-title">
                Question List

            </div>

            <div class="mb-3">

                <asp:Button
                    ID="btnApproveSelected"
                    runat="server"
                    Text="Approve Selected"
                    CssClass="btn btn-success"
                    OnClick="btnApproveSelected_Click" />

                <asp:Button
                    ID="btnRejectSelected"
                    runat="server"
                    Text="Reject Selected"
                    CssClass="btn btn-danger"
                    OnClick="btnRejectSelected_Click" />

            </div>
            <asp:HiddenField
                ID="hfQuestionID"
                runat="server" />
            <div
                class="modal fade"
                id="questionModal"
                tabindex="-1">

                <div
                    class="modal-dialog modal-xl">

                    <div
                        class="modal-content">

                        <div
                            class="modal-header">

                            <h5>Question Details

                            </h5>

                            <button
                                type="button"
                                class="btn-close"
                                data-bs-dismiss="modal">
                            </button>

                        </div>

                        <div
                            class="modal-body">

                            <b>Question</b>

                            <hr />

                            <asp:Label
                                ID="lblQuestion"
                                runat="server" />

                            <hr />

                            <b>Options</b>

                            <table
                                class="table table-bordered">

                                <tr>

                                    <td>A

                                    </td>

                                    <td>

                                        <asp:Label
                                            ID="lblA"
                                            runat="server" />

                                    </td>

                                </tr>

                                <tr>

                                    <td>B

                                    </td>

                                    <td>

                                        <asp:Label
                                            ID="lblB"
                                            runat="server" />

                                    </td>

                                </tr>

                                <tr>

                                    <td>C

                                    </td>

                                    <td>

                                        <asp:Label
                                            ID="lblC"
                                            runat="server" />

                                    </td>

                                </tr>

                                <tr>

                                    <td>D

                                    </td>

                                    <td>

                                        <asp:Label
                                            ID="lblD"
                                            runat="server" />

                                    </td>

                                </tr>

                            </table>

                            <b>Correct Answer

                            </b>

                            <br />

                            <asp:Label
                                ID="lblAnswer"
                                runat="server"
                                CssClass="text-success fw-bold" />

                            <hr />

                            <b>Explanation

                            </b>

                            <br />

                            <asp:Label
                                ID="lblExplanation"
                                runat="server" />

                            <hr />

                            <b>Question Image

                            </b>

                            <br />

                            <asp:Image
                                ID="imgQuestion"
                                runat="server"
                                Width="300px" />

                            <hr />

                            <b>Explanation Image

                            </b>

                            <br />

                            <asp:Image
                                ID="imgExplanation"
                                runat="server"
                                Width="300px" />

                        </div>

                        <div
                            class="modal-footer">

                            <asp:Button
                                ID="btnApprove"
                                runat="server"
                                Text="Approve"
                                CssClass="btn btn-success"
                                OnClick="btnApprove_Click" OnClientClick="return confirm('Approve this question?');" />
                            <div class="mb-3">

                                <label class="form-label">
                                    Reject Reason

                                </label>

                                <asp:TextBox
                                    ID="txtRejectReason"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="MultiLine"
                                    Rows="3">
                                </asp:TextBox>

                            </div>
                            <asp:Button
                                ID="btnReject"
                                runat="server"
                                Text="Reject"
                                CssClass="btn btn-danger"
                                OnClick="btnReject_Click" OnClientClick="return confirm('Reject this question?');" />

                            <button
                                class="btn btn-secondary"
                                data-bs-dismiss="modal">
                                Close

                            </button>

                        </div>

                    </div>

                </div>

            </div>
            <asp:GridView
                ID="gvQuestion"
                runat="server"
                CssClass="table table-bordered table-hover"
                AutoGenerateColumns="False"
                AllowPaging="true"
                PageSize="20" EmptyDataText="No questions found."
                DataKeyNames="QuestionID,ApprovalStatus" OnPageIndexChanging="gvQuestion_PageIndexChanging"
                OnRowCommand="gvQuestion_RowCommand" OnRowDataBound="gvQuestion_RowDataBound">

                <Columns>

                    <asp:TemplateField>

                        <HeaderTemplate>

                            <asp:CheckBox
                                ID="chkAll"
                                runat="server"
                                onclick="ToggleAll(this);" />

                        </HeaderTemplate>

                        <ItemTemplate>

                            <asp:CheckBox
                                ID="chkSelect"
                                runat="server" />

                        </ItemTemplate>

                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="#">
                        <ItemTemplate>

                            <%# Container.DataItemIndex + 1 %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField
                        DataField="QuestionID"
                        HeaderText="Question ID" />

                    <asp:BoundField
                        DataField="CourseName"
                        HeaderText="Course" />

                    <asp:BoundField
                        DataField="TopicName"
                        HeaderText="Topic" />

                    <asp:BoundField
                        DataField="TrainerName"
                        HeaderText="Trainer" />

                    <asp:BoundField
                        DataField="DifficultyLevel"
                        HeaderText="Difficulty" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>

                            <asp:Label
                                ID="lblStatus"
                                runat="server"
                                Text='<%# Eval("ApprovalStatus") %>'>
                            </asp:Label>

                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField
                        DataField="CreatedOn"
                        HeaderText="Created On"
                        DataFormatString="{0:dd-MM-yyyy}" />

                    <asp:TemplateField HeaderText="Action">

                        <ItemTemplate>

                            <asp:LinkButton
                                ID="lnkView"
                                runat="server"
                                CommandName="ViewQuestion"
                                CommandArgument='<%# Eval("QuestionID") %>'
                                CssClass="btn btn-info btn-sm">

View

                            </asp:LinkButton>

                            <asp:LinkButton
                                ID="lnkApprove"
                                runat="server"
                                CommandName="ApproveQuestion"
                                CommandArgument='<%# Eval("QuestionID") %>'
                                CssClass="btn btn-success btn-sm">

Approve

                            </asp:LinkButton>

                            <asp:LinkButton
                                ID="lnkReject"
                                runat="server"
                                CommandName="RejectQuestion"
                                CommandArgument='<%# Eval("QuestionID") %>'
                                CssClass="btn btn-danger btn-sm">

Reject

                            </asp:LinkButton>

                        </ItemTemplate>

                    </asp:TemplateField>

                </Columns>

            </asp:GridView>

        </div>

    </div>

    <script>

        $(function () {

            $('#ddlCourse').select2();

            $('#ddlTopic').select2();

            $('#ddlTrainer').select2();

        });

    </script>
    <script>

        function ToggleAll(source) {
            var checkboxes =
                document.querySelectorAll(
                    "[id*=chkSelect]");

            for
            (
                var i = 0;
                i < checkboxes.length;
                i++
            ) {
                checkboxes[i].checked =
                    source.checked;
            }
        }

    </script>
</asp:Content>
