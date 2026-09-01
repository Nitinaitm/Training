<%@ Page Title="Training Feedback"
    Language="C#"
    MasterPageFile="~/TraineeMaster.Master"
    AutoEventWireup="true"
    CodeBehind="TraineeFeedback.aspx.cs"
    Inherits="Training.Trainee.TraineeFeedback" %>

<%@ Register
    Src="~/Trainee/TraineeTrainingSummary.ascx"
    TagPrefix="uc1"
    TagName="TraineeTrainingSummary" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .page-title {
            font-size: 24px;
            font-weight: 600;
            color: #0d6efd;
        }

        .card {
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,.10);
            margin-bottom: 20px;
        }

        .category-title {
            font-size: 20px;
            font-weight: bold;
            color: #0d6efd;
        }

        .question-row {
            padding: 15px 12px;
            border-bottom: 1px solid #ececec;
        }

            .question-row:last-child {
                border-bottom: none;
            }

        .question-label {
            font-weight: 600;
            margin-bottom: 8px;
            display: block;
        }

        .trainer-title {
            font-size: 17px;
            font-weight: bold;
            color: #198754;
            margin-top: 20px;
            margin-bottom: 10px;
        }

        /* Rating Stars */

        .star-rating {
            display: inline-block;
        }

            .star-rating input[type="radio"] {
                position: absolute;
                opacity: 0;
                width: 1px;
                height: 1px;
            }

            .star-rating label {
                font-size: 30px;
                color: #cfcfcf;
                cursor: pointer;
                margin-right: 4px;
                line-height: 1;
            }

        /* Yes / No */

        .question-row input[type="radio"] {
            margin-right: 5px;
        }

        .question-row label {
            margin-right: 15px;
        }

        /* Input */

        .question-row .form-control {
            max-width: 100%;
        }

        /* Mobile */

        @media (max-width: 576px) {

            .page-title {
                font-size: 20px;
            }

            .star-rating label {
                font-size: 26px;
            }

            .question-row {
                padding: 12px 5px;
            }
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid">

        <div class="row mb-3">

            <div class="col-md-12">

                <span class="page-title">Training Feedback

                </span>

            </div>

        </div>

        <asp:Label
            ID="lblMessage"
            runat="server"
            Font-Bold="true">
        </asp:Label>

        <div class="card">

            <div class="card-header bg-primary text-white">

                <b>Training Details</b>

            </div>

            <div class="card-body">

               <uc1:TraineeTrainingSummary
    ID="TraineeTrainingSummary1"
    runat="server" />
            </div>

        </div>

        <div class="card">

            <div class="card-header bg-success text-white">

                <b>Feedback Questionnaire</b>

            </div>

            <div class="card-body">

                <asp:ValidationSummary
                    ID="ValidationSummary1"
                    runat="server"
                    CssClass="alert alert-danger" />

                <!-- Dynamic Questions -->

                <asp:PlaceHolder
                    ID="phFeedback"
                    runat="server"></asp:PlaceHolder>

            </div>

        </div>

        <div class="text-center mb-4">
             <asp:Button
                            ID="btnBack"
                            runat="server"
                            Text="Back"
                            CssClass="btn btn-secondary btn-lg"
                            CausesValidation="false"
                            PostBackUrl="~/Trainee/MyTrainings.aspx" />
            <asp:Button
                ID="btnSubmit"
                runat="server"
                Text="Submit Feedback"
                CssClass="btn btn-success btn-lg"
                OnClick="btnSubmit_Click" />

            &nbsp;

      <%--  <asp:Button
            ID="btnCancel"
            runat="server"
            Text="Cancel"
            CssClass="btn btn-secondary btn-lg"
            CausesValidation="false"
            OnClick="btnCancel_Click" />--%>

        </div>

    </div>

</asp:Content>
