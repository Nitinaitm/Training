<%@ Page Title="Contact Us"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Contact.aspx.cs"
    Inherits="Training.Contact" %>


<asp:Content
    ID="Content1"
    ContentPlaceHolderID="head"
    runat="server">

    <style type="text/css">

        /* ============================================
           CONTACT PAGE
        ============================================ */

        .contact-page {
            padding: 55px 15px 65px 15px;
            background: #f7f9fc;
            min-height: 500px;
        }

        .contact-container {
            max-width: 1150px;
            margin: 0 auto;
        }


        /* ============================================
           PAGE HEADING
        ============================================ */

        .contact-heading {
            text-align: center;
            margin-bottom: 38px;
        }

        .contact-small-title {
            display: inline-block;
            color: #198754;
            font-size: 14px;
            font-weight: 800;
            text-transform: uppercase;
            letter-spacing: 1.2px;
            margin-bottom: 8px;
        }

        .contact-heading h1 {
            margin: 0 0 10px 0;
            color: #173b67;
            font-size: 32px;
            font-weight: 800;
        }

        .contact-heading p {
            max-width: 650px;
            margin: 0 auto;
            color: #6c757d;
            font-size: 15px;
            line-height: 24px;
        }


        /* ============================================
           MAIN CONTACT BOX
        ============================================ */

        .contact-main-box {
            background: #ffffff;
            border-radius: 14px;
            box-shadow: 0 5px 25px rgba(0, 0, 0, .08);
            overflow: hidden;
        }


        /* ============================================
           LEFT INFORMATION PANEL
        ============================================ */

        .contact-info-panel {
            height: 100%;
            padding: 40px 35px;
            background: #173b67;
            color: #ffffff;
        }

        .contact-info-panel h2 {
            color: #ffffff;
            font-size: 23px;
            font-weight: 800;
            margin-bottom: 12px;
        }

        .contact-info-panel > p {
            color: #dbe6f3;
            font-size: 14px;
            line-height: 23px;
            margin-bottom: 30px;
        }


        /* ============================================
           CONTACT ITEM
        ============================================ */

        .contact-item {
            display: flex;
            align-items: flex-start;
            margin-bottom: 28px;
        }

        .contact-item:last-child {
            margin-bottom: 0;
        }

        .contact-icon {
            width: 48px;
            height: 48px;
            min-width: 48px;
            border-radius: 50%;
            background: rgba(255, 255, 255, .13);
            display: flex;
            align-items: center;
            justify-content: center;
            margin-right: 16px;
            font-size: 19px;
            color: #ffd54f;
        }

        .contact-item-content {
            min-width: 0;
        }

        .contact-item-content h5 {
            color: #ffffff;
            font-size: 15px;
            font-weight: 700;
            margin: 0 0 5px 0;
        }

        .contact-item-content p {
            color: #e3eaf3;
            font-size: 14px;
            line-height: 22px;
            margin: 0;
            word-break: break-word;
        }

        .contact-item-content a {
            color: #ffffff;
            text-decoration: none;
            word-break: break-word;
        }

        .contact-item-content a:hover {
            color: #ffd54f;
            text-decoration: none;
        }


        /* ============================================
           RIGHT PANEL
        ============================================ */

        .contact-right-panel {
            height: 100%;
            padding: 40px 38px;
        }

        .contact-right-panel h3 {
            color: #173b67;
            font-size: 22px;
            font-weight: 800;
            margin-bottom: 10px;
        }

        .contact-right-panel > p {
            color: #6c757d;
            font-size: 14px;
            line-height: 23px;
            margin-bottom: 25px;
        }


        /* ============================================
           OFFICE CARD
        ============================================ */

        .office-card {
            border: 1px solid #e6ebf1;
            border-radius: 10px;
            padding: 20px;
            margin-bottom: 18px;
            background: #fafcff;
            display: flex;
            align-items: flex-start;
        }

        .office-card-icon {
            width: 44px;
            height: 44px;
            min-width: 44px;
            border-radius: 8px;
            background: #e9f2ff;
            color: #173b67;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            margin-right: 15px;
        }

        .office-card h5 {
            color: #173b67;
            font-size: 15px;
            font-weight: 800;
            margin: 0 0 5px 0;
        }

        .office-card p {
            color: #606c7a;
            font-size: 14px;
            line-height: 21px;
            margin: 0;
        }


        /* ============================================
           HELP MESSAGE
        ============================================ */

        .help-box {
            margin-top: 25px;
            padding: 17px 18px;
            border-left: 4px solid #198754;
            border-radius: 5px;
            background: #eef9f3;
            color: #495057;
            font-size: 14px;
            line-height: 22px;
        }

        .help-box i {
            color: #198754;
            margin-right: 7px;
        }


        /* ============================================
           TABLET
        ============================================ */

        @media (max-width: 991.98px) {

            .contact-page {
                padding: 40px 15px 50px 15px;
            }

            .contact-heading {
                margin-bottom: 30px;
            }

            .contact-heading h1 {
                font-size: 27px;
            }

            .contact-info-panel {
                padding: 32px 28px;
            }

            .contact-right-panel {
                padding: 32px 28px;
            }
        }


        /* ============================================
           MOBILE
        ============================================ */

        @media (max-width: 767.98px) {

            .contact-page {
                padding: 30px 10px 40px 10px;
            }

            .contact-heading {
                margin-bottom: 24px;
            }

            .contact-small-title {
                font-size: 12px;
            }

            .contact-heading h1 {
                font-size: 23px;
                line-height: 29px;
            }

            .contact-heading p {
                font-size: 14px;
                line-height: 21px;
            }

            .contact-main-box {
                border-radius: 10px;
            }

            .contact-info-panel {
                padding: 28px 20px;
            }

            .contact-info-panel h2 {
                font-size: 20px;
            }

            .contact-right-panel {
                padding: 28px 20px;
            }

            .contact-right-panel h3 {
                font-size: 20px;
            }

            .contact-item {
                margin-bottom: 23px;
            }

            .contact-icon {
                width: 43px;
                height: 43px;
                min-width: 43px;
                font-size: 17px;
                margin-right: 12px;
            }

            .office-card {
                padding: 16px;
            }

            .office-card-icon {
                width: 40px;
                height: 40px;
                min-width: 40px;
                margin-right: 12px;
            }
        }


        /* ============================================
           SMALL MOBILE
        ============================================ */

        @media (max-width: 400px) {

            .contact-page {
                padding-left: 7px;
                padding-right: 7px;
            }

            .contact-info-panel {
                padding: 24px 16px;
            }

            .contact-right-panel {
                padding: 24px 16px;
            }

            .contact-heading h1 {
                font-size: 21px;
            }

            .contact-item-content p,
            .contact-item-content a {
                font-size: 13px;
            }

        }

    </style>

</asp:Content>


<asp:Content
    ID="Content2"
    ContentPlaceHolderID="ContentPlaceHolder1"
    runat="server">


    <section class="contact-page">

        <div class="contact-container">


            <!-- ========================================= -->
            <!-- HEADING                                   -->
            <!-- ========================================= -->

            <div class="contact-heading">

                <span class="contact-small-title">
                    Get In Touch
                </span>

                <h1>
                    Contact Training Cell
                </h1>

                <p>
                    For any query or assistance related to the Training
                    Management System, please contact the Training Cell,
                    Bihar State Power Holding Company Limited.
                </p>

            </div>


            <!-- ========================================= -->
            <!-- CONTACT BOX                               -->
            <!-- ========================================= -->

            <div class="contact-main-box">

                <div class="row no-gutters">


                    <!-- ================================= -->
                    <!-- LEFT SIDE                         -->
                    <!-- ================================= -->

                    <div class="col-lg-5">

                        <div class="contact-info-panel">

                            <h2>
                                Contact Information
                            </h2>

                            <p>
                                For training related queries, technical
                                assistance or other information, you may
                                contact us using the details below.
                            </p>


                            <!-- ADDRESS -->

                            <div class="contact-item">

                                <div class="contact-icon">

                                    <i class="fas fa-map-marker-alt"></i>

                                </div>

                                <div class="contact-item-content">

                                    <h5>
                                        Office Address
                                    </h5>

                                    <p>
                                        Training Cell<br />
                                        Bihar State Power Holding Company Limited<br />
                                        Vidyut Bhawan, Patna, Bihar
                                    </p>

                                </div>

                            </div>


                            <!-- EMAIL -->

                            <div class="contact-item">

                                <div class="contact-icon">

                                    <i class="fas fa-envelope"></i>

                                </div>

                                <div class="contact-item-content">

                                    <h5>
                                        Email Address
                                    </h5>

                                    <p>

                                        <a href="mailto:trainingcell.bsphcl@gmail.com">

                                            trainingcell.bsphcl@gmail.com

                                        </a>

                                    </p>

                                </div>

                            </div>


                            <!-- WORKING HOURS -->

                            <div class="contact-item">

                                <div class="contact-icon">

                                    <i class="far fa-clock"></i>

                                </div>

                                <div class="contact-item-content">

                                    <h5>
                                        Office Hours
                                    </h5>

                                    <p>
                                        Monday - Friday
                                        <br />
                                        09:30 AM - 06:00 PM
                                    </p>

                                </div>

                            </div>

                        </div>

                    </div>


                    <!-- ================================= -->
                    <!-- RIGHT SIDE                        -->
                    <!-- ================================= -->

                    <div class="col-lg-7">

                        <div class="contact-right-panel">

                            <h3>
                                Training Cell, BSPHCL
                            </h3>

                            <p>
                                The Training Cell facilitates training
                                programmes, trainee management, examinations,
                                feedback and certification through the
                                Training Management System.
                            </p>


                            <!-- ORGANIZATION -->

                            <div class="office-card">

                                <div class="office-card-icon">

                                    <i class="fas fa-building"></i>

                                </div>

                                <div>

                                    <h5>
                                        Organization
                                    </h5>

                                    <p>
                                        Bihar State Power Holding
                                        Company Limited
                                    </p>

                                </div>

                            </div>


                            <!-- DEPARTMENT -->

                            <div class="office-card">

                                <div class="office-card-icon">

                                    <i class="fas fa-graduation-cap"></i>

                                </div>

                                <div>

                                    <h5>
                                        Department
                                    </h5>

                                    <p>
                                        Training Cell, BSPHCL
                                    </p>

                                </div>

                            </div>


                            <!-- LOCATION -->

                            <div class="office-card">

                                <div class="office-card-icon">

                                    <i class="fas fa-map-marked-alt"></i>

                                </div>

                                <div>

                                    <h5>
                                        Location
                                    </h5>

                                    <p>
                                        Vidyut Bhawan, Patna, Bihar
                                    </p>

                                </div>

                            </div>


                            <!-- HELP -->

                            <div class="help-box">

                                <i class="fas fa-info-circle"></i>

                                For issues related to login, training
                                assignment, examination, feedback or
                                certificate, please mention your
                                <strong>Employee/Trainee ID</strong>
                                and
                                <strong>Training ID</strong>
                                while contacting the Training Cell.

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </section>

</asp:Content>