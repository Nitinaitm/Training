<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="CertificatePreview.aspx.cs"
    Inherits="Training.Admin.CertificatePreview"
    MasterPageFile="~/AdminMaster.Master" %>

<asp:Content ID="Content1" contentplaceholderid="head" runat="server">

    <style>
        body {
            background: #f5f5f5;
        }

        .toolbar {
            text-align: center;
            margin-bottom: 20px;
        }

        .certificate {
            position: relative;
            width: 1123px;
            height: 794px;
            margin: auto;
            background: #ffffff;
            background-repeat: no-repeat;
            background-position: center;
            background-size: 100% 100%;
            border: 1px solid #cccccc;
            box-shadow: 0 0 10px #999999;
            overflow: hidden;
        }

        .logo {
            position: absolute;
            top: 35px;
            left: 50%;
            transform: translateX(-50%);
            width: 90px;
            height: 90px;
            object-fit: contain;
        }

        .header {
            position: absolute;
            top: 130px;
            left: 80px;
            right: 80px;
            text-align: center;
            font-weight: bold;
        }

        .title {
            position: absolute;
            top: 220px;
            left: 60px;
            right: 60px;
            text-align: center;
            font-weight: bold;
        }

        .body {
            position: absolute;
            top: 300px;
            left: 90px;
            right: 90px;
            text-align: center;
            line-height: 35px;
        }

        .footer {
            position: absolute;
            bottom: 20px;
            left: 60px;
            right: 60px;
            text-align: center;
        }

        .signature {
            position: absolute;
            bottom: 110px;
            width: 220px;
            text-align: center;
        }

            .signature img {
                width: 180px;
                height: 70px;
                object-fit: contain;
            }

        .left {
            left: 80px;
        }

        .right {
            right: 80px;
        }

        .sign-name {
            font-weight: bold;
            margin-top: 5px;
        }

        .sign-designation {
            font-size: 15px;
        }

        .trainee-name {
            font-size: 36px;
            font-weight: bold;
            display: block;
            margin-top: 15px;
            margin-bottom: 15px;
        }

        .course-title {
            font-size: 24px;
            font-weight: bold;
        }

        @media print {
            body {
                background: #ffffff;
            }

            .toolbar {
                display: none;
            }

            .certificate {
                border: none;
                box-shadow: none;
                margin: 0;
            }
        }
    </style>

</asp:Content>

<asp:Content    ID="Content2"    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">

    <div class="container-fluid">

        <div class="toolbar">

            <asp:button
                id="btnBack"
                runat="server"
                text="Back"
                cssclass="btn btn-secondary"
                onclick="btnBack_Click" />

        </div>

        <div
            id="divCertificate"
            runat="server"
            class="certificate">

            <asp:image
                id="imgLogo"
                runat="server"
                cssclass="logo" />

            <asp:label
                id="lblHeader"
                runat="server"
                cssclass="header">
            </asp:label>

            <asp:label
                id="lblTitle"
                runat="server"
                cssclass="title">
            </asp:label>

            <div class="body">
                This Certificate is proudly presented to

                <br />
                <br />

                <asp:label
                    id="lblEmployee"
                    runat="server"
                    cssclass="trainee-name">
                </asp:label>

                for successfully completing

                <br />
                <br />

                <asp:label
                    id="lblCourse"
                    runat="server"
                    cssclass="course-title">
                </asp:label>

                <br />
                <br />

                conducted from

                <asp:label
                    id="lblDuration"
                    runat="server">
                </asp:label>

            </div>

            <div
                class="signature left">

                <asp:image
                    id="imgLeftSignature"
                    runat="server" />

                <div class="sign-name">

                    <asp:label
                        id="lblLeftName"
                        runat="server">
                    </asp:label>

                </div>

                <div class="sign-designation">

                    <asp:label
                        id="lblLeftDesignation"
                        runat="server">
                    </asp:label>

                </div>

            </div>

            <div
                class="signature right">

                <asp:image
                    id="imgRightSignature"
                    runat="server" />

                <div class="sign-name">

                    <asp:label
                        id="lblRightName"
                        runat="server">
                    </asp:label>

                </div>

                <div class="sign-designation">

                    <asp:label
                        id="lblRightDesignation"
                        runat="server">
                    </asp:label>

                </div>

            </div>

            <asp:label
                id="lblFooter"
                runat="server"
                cssclass="footer">
            </asp:label>

        </div>

    </div>

</asp:Content>
