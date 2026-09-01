using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class ManageTraining :
        System.Web.UI.Page
    {
        string constr =
        ConfigurationManager
        .ConnectionStrings["constr"]
        .ConnectionString;

        protected void Page_Load(
        object sender,
        EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainingID"] == null)
                {
                    Response.Redirect(
                    "TrainingList.aspx");

                    return;
                }
                TrainingSummary1.LoadTraining(Session["TrainingID"].ToString());
                LoadWorkflow();
            }
        }

        private void LoadWorkflow()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT
HostelRequiredTrainee,
TrainingStatus,
WorkflowStatus

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (!dr.Read())
                    return;

                lblStatus.Text =
                    dr["TrainingStatus"].ToString();

                string workflow =
                    dr["WorkflowStatus"].ToString();

                string assignHostel =
                    dr["HostelRequiredTrainee"].ToString();

                

                //---------------------------------------
                // Default
                //---------------------------------------

                btnUpdateTraining.Visible = true;

                btnAssignSession.Visible = true;

                btnAssignTrainee.Visible = true;

                btnStartTraining.Visible = false;

                btnAttendance.Visible = false;

                btnAssignHostel.Visible = false;

                btnCertificateTemplate.Visible =
    true;

                btnCertificateTemplate.Enabled =
                    false;

                bool traineeAssigned =
    IsTraineeAssigned();

                bool certificateConfigured =
                    IsCertificateTemplateConfigured();

                if
                (
                    traineeAssigned
                    &&
                    !workflow.Contains("E")
                )
                {
                    btnCertificateTemplate.Enabled =
                        true;

                    if (certificateConfigured)
                    {
                        btnCertificateTemplate.Text =
                            "Certificate Template ✓";
                    }
                    else
                    {
                        btnCertificateTemplate.Text =
                            "Certificate Template";
                    }
                }
                // btnCertificate.Visible = false;

                //---------------------------------------
                // Ready To Start
                //---------------------------------------

                bool canStart =

                    workflow.Contains("A") &&

                    workflow.Contains("B") &&

                    workflow.Contains("C") &&

                    workflow.Contains("D") &&

                    !workflow.Contains("E");

                if (canStart)
                {
                    btnStartTraining.Visible = true;
                    if (assignHostel == "Yes")
                    {
                        btnAssignHostel.Visible = true;
                    }
                    else
                    {
                        btnAssignHostel.Visible = false;
                    }
                }

                //---------------------------------------
                // Training Started
                //---------------------------------------

                if (workflow.Contains("E"))
                {
                    btnUpdateTraining.Visible =
                        false;

                    btnAssignSession.Visible =
                        false;

                    btnAssignTrainee.Visible =
                        false;

                    btnStartTraining.Visible =
                        false;

                    btnAssignHostel.Visible =
                        false;

                    btnCertificateTemplate.Enabled =
                        false;

                    btnAttendance.Visible =
                        true;
                }
                //if (workflow.Contains("E") && !workflow.Contains("J"))
                //{
                //    btnUpdateTraining.Visible = false;

                //    btnAssignSession.Visible = false;

                //    btnAssignTrainee.Visible = false;

                //    btnStartTraining.Visible = false;

                //    Label1.ForeColor = System.Drawing.Color.Green;

                //    Label1.Text = "Training Has Started Successfully.";

                //    action.Visible = false;
                //    //  btnAttendance.Visible = true;
                //}
                //---------------------------------------
                // Attendance Completed
                //---------------------------------------

                //if (workflow.Contains("F"))
                //{
                //    btnCertificate.Visible = true;
                //}

                dr.Close();
            }
        }

        //-------------------------------------------------------
        // Certificate Template
        //-------------------------------------------------------

        protected void btnCertificateTemplate_Click(
            object sender,
            EventArgs e)
        {
            if
            (
                Session["TrainingID"]
                ==
                null
            )
            {
                Response.Redirect(
                    "TrainingList.aspx");

                return;
            }

            if
            (
                !IsTraineeAssigned()
            )
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Please assign trainee before configuring certificate template.";

                return;
            }

            Response.Redirect(
                "CertificateTemplate.aspx");
        }

        //-------------------------------------------------------
        // Check Trainee Assigned
        //-------------------------------------------------------

        private bool IsTraineeAssigned()
        {
            string query =
        @"
SELECT
COUNT(*)
FROM
TrainingAssignment
WHERE
TrainingID=@TrainingID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"]
            .ToString())
    };

            clsDataAccess objDB =
                new clsDataAccess();

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param));

            return
                count > 0;
        }

        //-------------------------------------------------------
        // Check Certificate Template Configured
        //-------------------------------------------------------

        private bool IsCertificateTemplateConfigured()
        {
            string query =
        @"
SELECT
COUNT(*)
FROM
TrainingCertificateTemplate
WHERE
TrainingID=@TrainingID
AND
ISNULL(TemplateID,'')<>''
AND
ISNULL(CourseTitle,'')<>''
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"]
            .ToString())
    };

            clsDataAccess objDB =
                new clsDataAccess();

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param));

            return
                count > 0;
        }

        protected void btnStartTraining_Click(
     object sender,
     EventArgs e)
        {
            if
            (
                Session["TrainingID"]
                ==
                null
            )
            {
                Response.Redirect(
                    "TrainingList.aspx");

                return;
            }

            if
            (
                !IsTraineeAssigned()
            )
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Please assign trainee before starting training.";

                return;
            }

            if
            (
                !IsCertificateTemplateConfigured()
            )
            {
                Session["ReturnAfterCertificate"] =
                    "StartTraining";

                Response.Redirect(
                    "CertificateTemplate.aspx");

                return;
            }

            pnlHostelConfirmation.Visible =
                true;

            lblMessage.ForeColor =
                System.Drawing.Color.Blue;

            lblMessage.Text =
                "Please confirm whether hostel accommodation is required.";
        }
       

        protected void btnHostelNo_Click(
    object sender,
    EventArgs e)
        {
            UpdateHostelRequirement(
                "No");

            StartTraining();
        }

        protected void btnHostelYes_Click(
    object sender,
    EventArgs e)
        {
            UpdateHostelRequirement(
                "Yes");

            pnlHostelConfirmation.Visible =
                false;

            Response.Redirect(
                "AssignHostel.aspx");
        }
        private void UpdateHostelRequirement(
       string hostelRequired)
        {
            string query =
                "UPDATE TrainingDetails SET HostelRequiredTrainee=@HostelRequiredTrainee, UpdatedOn=GETDATE(), UpdatedBy=@UpdatedBy WHERE TrainingID=@TrainingID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@HostelRequiredTrainee",
            hostelRequired),

        new SqlParameter(
            "@UpdatedBy",
            Session["AdminID"] == null
            ? "Admin"
            : Session["AdminID"].ToString()),

        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"].ToString())
    };

            clsDataAccess objDB =
                new clsDataAccess();

            objDB.ExecuteSql(
                query,
                param);
        }

        private void StartTraining()
        {
            if
            (
                !IsCertificateTemplateConfigured()
            )
            {
                pnlHostelConfirmation.Visible =
                    false;

                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Certificate template configuration is mandatory before starting training.";

                return;
            }

            clsWorkflow.UpdateWorkflow(
                Session["TrainingID"].ToString(),
                "InProgress",
                "E");

            pnlHostelConfirmation.Visible =
                false;

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Training has started successfully.";

            LoadWorkflow();
        }

        protected void btnAttendance_Click(
   object sender,
   EventArgs e)
        {
            Response.Redirect(
                "TrainingAttendance.aspx");
        }

        //        protected void btnCertificate_Click(
        //object sender,
        //EventArgs e)
        //        {
        //            Response.Redirect(
        //            "TrainingCertificate.aspx");
        //        }
        //private void ApplyStatusRules(string status)
        //{
        //    // Default
        //    btnUpdateTraining.Enabled = true;

        //    btnAssignSession.Enabled = true;

        //    btnAssignTrainee.Enabled = true;

        //    btnAttendance.Visible = false;

        //    btnCertificate.Visible = false;

        //    switch (status)
        //    {
        //        case "Draft":

        //            lblStatus.CssClass =
        //            "badge bg-secondary status-badge";

        //            break;

        //        case "SessionAssigned":

        //            lblStatus.CssClass =
        //            "badge bg-warning text-dark status-badge";

        //            btnUpdateTraining.Enabled = false;

        //            break;

        //        case "TraineeAssigned":

        //            lblStatus.CssClass =
        //            "badge bg-info text-dark status-badge";

        //            btnUpdateTraining.Enabled = false;

        //            btnAttendance.Visible = true;

        //            break;

        //        case "Completed":

        //            lblStatus.CssClass =
        //            "badge bg-success status-badge";

        //            btnUpdateTraining.Enabled = false;

        //            btnAssignSession.Enabled = false;

        //            btnAssignTrainee.Enabled = false;

        //            btnAttendance.Visible = true;

        //            btnCertificate.Visible = true;

        //            break;
        //    }
        //}

        protected void btnUpdateTraining_Click(
        object sender,
        EventArgs e)
        {
            //Response.Redirect(
            //"CreateTraining.aspx");
            Response.Redirect("CreateBatch.aspx?mode=edit");
        }


        protected void btnAssignSession_Click(
        object sender,
        EventArgs e)
        {
            Response.Redirect(
            "AssignSession.aspx");
        }

        protected void btnAssignHostel_Click(
       object sender,
       EventArgs e)
        {
            Response.Redirect(
            "AssignHostel.aspx");
        }

        protected void btnAssignTrainee_Click(
        object sender,
        EventArgs e)
        {
            Response.Redirect(
            "AssignTrainee.aspx");
        }
    }
}