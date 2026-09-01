<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="PreTrainingExam.aspx.cs" 
    Inherits="Training.Trainee.PreTrainingExam"
    MasterPageFile="~/TraineeMaster.Master" %>

<%@ Register
    Src="~/Trainee/SessionSummary.ascx"
    TagPrefix="uc1"
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

        .question-palette {
            width: 40px;
            height: 40px;
            margin: 4px;
            border-radius: 50%;
            font-weight: bold;
        }

        .timer {
            font-size: 28px;
            font-weight: bold;
            color: #dc3545;
        }

        .question-box {
            min-height: 120px;
            font-size: 18px;
            font-weight: 600;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">
         <uc1:SessionSummary
    ID="SessionSummary1"
    runat="server" />
        <div class="card">

            <div class="card-header bg-primary text-white">
                Pre Training Examination

            </div>

            <div class="card-body">

                <div class="row">

                    <div class="col-md-6">

                        <table class="table table-bordered">

                            <tr>

                                <th width="180">Test Title

                                </th>

                                <td>

                                    <asp:Label
                                        ID="lblTestTitle"
                                        runat="server" />

                                </td>

                            </tr>

                            <tr>

                                <th>Total Questions

                                </th>

                                <td>

                                    <asp:Label
                                        ID="lblTotalQuestions"
                                        runat="server" />

                                </td>

                            </tr>

                            <tr>

                                <th>Total Marks

                                </th>

                                <td>

                                    <asp:Label
                                        ID="lblTotalMarks"
                                        runat="server" />

                                </td>

                            </tr>

                            <tr>

                                <th>Passing %

                                </th>

                                <td>

                                    <asp:Label
                                        ID="lblPassing"
                                        runat="server" />

                                </td>

                            </tr>

                        </table>

                    </div>

                    <div class="col-md-3 text-center">

                        <div class="timer">

                            <asp:Label
                                ID="lblTimer"
                                runat="server"
                                ClientIDMode="Static"
                                Text="30:00" />


                        </div>

                    </div>

                    <div class="col-md-3 text-end">

                        <asp:Button
                            ID="btnStart"
                            runat="server"
                            Text="Start Test"
                            CssClass="btn btn-success btn-lg"
                            OnClick="btnStart_Click" />

                    </div>

                </div>

            </div>

        </div>

        <div
            id="divExam"
            runat="server"
            visible="false">

            <div class="row">

                <div class="col-md-9">

                    <div class="card">

                        <div class="card-header bg-info text-white">
                            Question

                        </div>

                        <div class="card-body">

                            <div class="mb-3">
                                Question No :

                                <asp:Label
                                    ID="lblQuestionNo"
                                    runat="server" />

                            </div>

                            <div
                                class="question-box">

                                <asp:Label
                                    ID="lblQuestion"
                                    runat="server" />
                                <asp:Image
                                    ID="imgQuestion"
                                    runat="server"
                                    CssClass="img-fluid mb-3"
                                    Visible="false" />
                            </div>

                            <hr />

                            <asp:RadioButtonList
                                ID="rblOption"
                                runat="server"
                                RepeatDirection="Vertical">
                            </asp:RadioButtonList>

                        </div>

                    </div>

                    <div class="text-center">

                        <asp:Button
                            ID="btnPrevious"
                            runat="server"
                            Text="Previous"
                            CssClass="btn btn-secondary"
                            OnClick="btnPrevious_Click" />

                        <asp:Button
                            ID="btnNext"
                            runat="server"
                            Text="Next"
                            CssClass="btn btn-primary"
                            OnClick="btnNext_Click" />
                        <asp:Button
                            ID="btnFinish"
                            runat="server"
                            Text="Finish Test"
                            CssClass="btn btn-success"
                            CausesValidation="false"
                            Visible="false"
                            OnClientClick="return FinishExam();" />
                        <asp:Button
                            ID="btnSubmit"
                            runat="server"
                            Style="display: none;"
                            ClientIDMode="Static"
                            OnClick="btnSubmit_Click" />
                    </div>

                </div>

                <div class="col-md-3">

                    <div class="card">

                        <div class="card-header bg-warning">
                            Question Palette

                        </div>

                        <div class="card-body">

                            <asp:Repeater
                                ID="rptPalette"
                                runat="server">

                                <ItemTemplate>

                                    <asp:Button
    ID="btnQuestion"
    runat="server"
    Text='<%# Eval("DisplayOrder") %>'
    CommandArgument='<%# Eval("DisplayOrder") %>'
    CssClass='<%# GetPaletteClass(Container.ItemIndex) %>'
    OnCommand="btnQuestion_Command" />

                                </ItemTemplate>

                            </asp:Repeater>

                        </div>

                    </div>

                </div>

            </div>

        </div>

        <asp:HiddenField
            ID="hfTestID"
            runat="server" />

        <asp:HiddenField
            ID="hfCurrentQuestion"
            runat="server"
            Value="1" />

        <asp:HiddenField
            ID="hfTotalQuestion"
            runat="server" />

        <asp:HiddenField
            ID="hfRemainingSecond"
            runat="server"
            ClientIDMode="Static" />

    </div>

    <script>

        var timer = null;

        function StartExamTimer() {
            if (timer != null) {
                clearInterval(timer);
            }

            var second =
                parseInt(
                    document.getElementById(
                        "hfRemainingSecond").value);

            timer =
                setInterval(function () {
                    second--;

                    if (second < 0) {
                        clearInterval(timer);

                        alert(
                            "Time is over. Test will be submitted automatically.");

                        document
                            .getElementById(
                                "btnSubmit")
                            .click();

                        return;
                    }

                    document
                        .getElementById(
                            "hfRemainingSecond")
                        .value =
                        second;

                    var minute =
                        Math.floor(
                            second / 60);

                    var sec =
                        second % 60;

                    document
                        .getElementById(
                            "lblTimer")
                        .innerHTML =
                        ("0" + minute)
                            .slice(-2)
                        +
                        ":"
                        +
                        ("0" + sec)
                            .slice(-2);

                }, 1000);
        }

        function FinishExam() {
            if
            (
                confirm(
                    "Are you sure you want to submit the examination?")
            ) {
                document
                    .getElementById(
                        "btnSubmit")
                    .click();
            }

            return false;
        }

    </script>

</asp:Content>
