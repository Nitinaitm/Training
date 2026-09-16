<%@ Page Title="Verify Certificate"
    Language="C#"
    AutoEventWireup="true"
    CodeBehind="VerifyCertificate.aspx.cs"
    Inherits="Training.VerifyCertificate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">

    <title>Verify Certificate</title>

    <meta
        name="viewport"
        content="width=device-width, initial-scale=1" />

    <style type="text/css">

        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 0;
            font-family: Arial, Helvetica, sans-serif;
            background-color: #f5f7fa;
            color: #212529;
        }

        .page-wrapper {
            min-height: 100vh;
            padding: 30px 15px;
        }

        .verify-container {
            max-width: 850px;
            margin: 0 auto;
        }

        .page-heading {
            text-align: center;
            margin-bottom: 25px;
        }

        .page-heading h1 {
            margin: 0 0 8px 0;
            font-size: 28px;
            color: #0d6efd;
        }

        .page-heading p {
            margin: 0;
            color: #6c757d;
            font-size: 15px;
        }

        .card {
            background-color: #ffffff;
            border: 1px solid #e5e5e5;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,.08);
            margin-bottom: 20px;
            overflow: hidden;
        }

        .card-header {
            padding: 14px 20px;
            background-color: #0d6efd;
            color: #ffffff;
            font-size: 17px;
            font-weight: bold;
        }

        .card-body {
            padding: 22px;
        }

        .form-row {
            display: flex;
            flex-wrap: wrap;
            margin-left: -8px;
            margin-right: -8px;
        }

        .form-column {
            width: 50%;
            padding: 0 8px;
            margin-bottom: 16px;
        }

        .form-label {
            display: block;
            font-weight: 600;
            margin-bottom: 6px;
        }

        .required {
            color: #dc3545;
        }

        .form-control {
            display: block;
            width: 100%;
            padding: 10px 12px;
            font-size: 14px;
            border: 1px solid #ced4da;
            border-radius: 5px;
            outline: none;
        }

        .form-control:focus {
            border-color: #86b7fe;
            box-shadow: 0 0 0 2px rgba(13,110,253,.15);
        }

        .button-area {
            text-align: center;
            margin-top: 5px;
        }

        .btn {
            display: inline-block;
            border: none;
            border-radius: 5px;
            padding: 10px 24px;
            font-size: 15px;
            cursor: pointer;
            text-decoration: none;
        }

        .btn-primary {
            background-color: #0d6efd;
            color: #ffffff;
        }

        .btn-secondary {
            background-color: #6c757d;
            color: #ffffff;
        }

        .message {
            display: block;
            margin-top: 15px;
            text-align: center;
            font-weight: bold;
        }

        .valid-box {
            border: 2px solid #198754;
        }

        .valid-header {
            background-color: #198754;
            color: #ffffff;
            padding: 15px 20px;
            font-size: 19px;
            font-weight: bold;
            text-align: center;
        }

        .detail-table {
            width: 100%;
            border-collapse: collapse;
        }

        .detail-table td {
            padding: 11px 10px;
            border-bottom: 1px solid #eeeeee;
            vertical-align: top;
        }

        .detail-label {
            width: 35%;
            font-weight: 600;
            color: #495057;
        }

        .detail-value {
            color: #212529;
        }

        .certificate-number {
            color: #0d6efd;
            font-weight: bold;
        }

        .verification-code {
            font-family: Consolas, monospace;
            font-weight: bold;
        }

        .status-valid {
            display: inline-block;
            padding: 5px 12px;
            background-color: #198754;
            color: #ffffff;
            border-radius: 15px;
            font-size: 13px;
            font-weight: bold;
        }

        .invalid-box {
            padding: 18px;
            border: 1px solid #dc3545;
            border-radius: 6px;
            background-color: #fff5f5;
            color: #dc3545;
            text-align: center;
            font-weight: bold;
        }

        @media (max-width: 650px) {

            .form-column {
                width: 100%;
            }

            .detail-label {
                width: 42%;
            }

            .page-heading h1 {
                font-size: 23px;
            }

            .card-body {
                padding: 16px;
            }
        }

    </style>

</head>


<body>

    <form
        id="form1"
        runat="server">

        <div class="page-wrapper">

            <div class="verify-container">

                <!-- Heading -->

                <div class="page-heading">

                    <h1>
                        Certificate Verification
                    </h1>

                    <p>
                        Verify the authenticity of a training certificate.
                    </p>

                </div>


                <!-- Search Card -->

                <div class="card">

                    <div class="card-header">

                        Verify Certificate

                    </div>

                    <div class="card-body">

                        <div class="form-row">

                            <div class="form-column">

                                <label class="form-label">

                                    Certificate Number

                                    <span class="required">
                                        *
                                    </span>

                                </label>

                                <asp:TextBox
                                    ID="txtCertificateNo"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="50"
                                    autocomplete="off"
                                    placeholder="Enter Certificate Number">
                                </asp:TextBox>

                            </div>


                            <div class="form-column">

                                <label class="form-label">

                                    Verification Code

                                    <span class="required">
                                        *
                                    </span>

                                </label>

                                <asp:TextBox
                                    ID="txtVerificationCode"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="50"
                                    autocomplete="off"
                                    placeholder="Enter Verification Code">
                                </asp:TextBox>

                            </div>

                        </div>


                        <div class="button-area">

                            <asp:Button
                                ID="btnVerify"
                                runat="server"
                                Text="Verify Certificate"
                                CssClass="btn btn-primary"
                                OnClick="btnVerify_Click" />

                            &nbsp;

                            <asp:Button
                                ID="btnReset"
                                runat="server"
                                Text="Reset"
                                CssClass="btn btn-secondary"
                                CausesValidation="false"
                                OnClick="btnReset_Click" />

                        </div>


                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            CssClass="message">
                        </asp:Label>

                    </div>

                </div>


                <!-- Valid Certificate Result -->

                <asp:Panel
                    ID="pnlCertificate"
                    runat="server"
                    Visible="false"
                    CssClass="card valid-box">

                    <div class="valid-header">

                        Certificate Verified Successfully

                    </div>


                    <div class="card-body">

                        <table class="detail-table">

                            <tr>

                                <td class="detail-label">
                                    Certificate Number
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblCertificateNo"
                                        runat="server"
                                        CssClass="certificate-number">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Trainee Name
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblTraineeName"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Employee / Trainee ID
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblEmpID"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Training ID
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblTrainingID"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Course
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblCourseName"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Course Title
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblCourseTitle"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Training Duration
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblTrainingDuration"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Generated On
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblGeneratedOn"
                                        runat="server">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Verification Code
                                </td>

                                <td class="detail-value">

                                    <asp:Label
                                        ID="lblVerificationCode"
                                        runat="server"
                                        CssClass="verification-code">
                                    </asp:Label>

                                </td>

                            </tr>


                            <tr>

                                <td class="detail-label">
                                    Certificate Status
                                </td>

                                <td class="detail-value">

                                    <span class="status-valid">
                                        VALID
                                    </span>

                                </td>

                            </tr>

                        </table>

                    </div>

                </asp:Panel>


                <!-- Invalid Result -->

                <asp:Panel
                    ID="pnlInvalid"
                    runat="server"
                    Visible="false"
                    CssClass="invalid-box">

                    Certificate could not be verified.

                    <br />

                    Please check the Certificate Number and
                    Verification Code and try again.

                </asp:Panel>

            </div>

        </div>

    </form>

</body>

</html>