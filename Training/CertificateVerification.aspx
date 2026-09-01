<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="CertificateVerification.aspx.cs"
    Inherits="Training.CertificateVerification" %>

<!DOCTYPE html>

<html>
<head runat="server">

    <title>Certificate Verification</title>

    <meta name="viewport"
        content="width=device-width, initial-scale=1" />

    <link
        href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
        rel="stylesheet" />

    <style>

        body {
            background: #f5f7fa;
        }

        .verification-wrapper {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 30px 15px;
        }

        .verification-card {
            width: 100%;
            max-width: 650px;
            border: none;
            border-radius: 12px;
            box-shadow: 0 4px 20px rgba(0,0,0,.12);
        }

        .verification-title {
            font-size: 28px;
            font-weight: 600;
        }

        .verification-subtitle {
            color: #6c757d;
            font-size: 14px;
        }

        .result-card {
            border-radius: 10px;
            margin-top: 25px;
        }

        .valid-status {
            color: #198754;
            font-weight: 700;
            font-size: 20px;
        }

        .invalid-status {
            color: #dc3545;
            font-weight: 700;
            font-size: 20px;
        }

        .detail-label {
            font-weight: 600;
            color: #555;
        }

        .detail-value {
            color: #222;
        }

    </style>

</head>

<body>

<form
    id="form1"
    runat="server">

    <div class="verification-wrapper">

        <div class="card verification-card">

            <!-- ============================================= -->
            <!-- HEADER -->
            <!-- ============================================= -->

            <div class="card-header text-center bg-primary text-white p-4">

                <div class="verification-title">
                    Certificate Verification
                </div>

                <div class="mt-2">
                    Verify the authenticity of a training certificate
                </div>

            </div>


            <!-- ============================================= -->
            <!-- BODY -->
            <!-- ============================================= -->

            <div class="card-body p-4">

                <div class="verification-subtitle mb-4 text-center">

                    Enter the Certificate Number and Verification Code
                    printed on the certificate.

                </div>


                <!-- Certificate Number -->

                <div class="mb-3">

                    <label
                        class="form-label">

                        Certificate Number

                        <span class="text-danger">
                            *
                        </span>

                    </label>

                    <asp:TextBox
                        ID="txtCertificateNo"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100">
                    </asp:TextBox>

                </div>


                <!-- Verification Code -->

                <div class="mb-3">

                    <label
                        class="form-label">

                        Verification Code

                        <span class="text-danger">
                            *
                        </span>

                    </label>

                    <asp:TextBox
                        ID="txtVerificationCode"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="50">
                    </asp:TextBox>

                </div>


                <!-- Verify -->

                <div class="d-grid mt-4">

                    <asp:Button
                        ID="btnVerify"
                        runat="server"
                        Text="Verify Certificate"
                        CssClass="btn btn-primary"
                        OnClick="btnVerify_Click" />

                </div>


                <!-- Message -->

                <div class="text-center mt-3">

                    <asp:Label
                        ID="lblMessage"
                        runat="server">
                    </asp:Label>

                </div>


                <!-- ============================================= -->
                <!-- RESULT -->
                <!-- ============================================= -->

                <asp:Panel
                    ID="pnlResult"
                    runat="server"
                    Visible="false"
                    CssClass="card result-card">

                    <div class="card-body">


                        <div class="text-center mb-4">

                            <asp:Label
                                ID="lblVerificationStatus"
                                runat="server">
                            </asp:Label>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Certificate No.
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblCertificateNo"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Employee Name
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblEmpName"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Training ID
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblTrainingID"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Course
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblCourseTitle"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Training Period
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblTrainingPeriod"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Certificate Date
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblGeneratedOn"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <div class="row mb-2">

                            <div class="col-md-5 detail-label">
                                Status
                            </div>

                            <div class="col-md-7 detail-value">

                                <asp:Label
                                    ID="lblStatus"
                                    runat="server">
                                </asp:Label>

                            </div>

                        </div>


                        <!-- PDF -->

                        <div class="text-center mt-4">

                            <asp:HyperLink
                                ID="lnkCertificate"
                                runat="server"
                                Target="_blank"
                                CssClass="btn btn-success">

                                View Certificate

                            </asp:HyperLink>

                        </div>

                    </div>

                </asp:Panel>


            </div>

        </div>

    </div>

</form>

</body>
</html>