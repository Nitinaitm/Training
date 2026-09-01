using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class TrainingAttendance : System.Web.UI.Page
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
                    Response.Redirect("TrainingList.aspx");
                    return;
                }

                TrainingSummary1.LoadTraining(
                    Session["TrainingID"].ToString());
                using (SqlConnection con =
    new SqlConnection(constr))
                {
                    SqlCommand cmd =
                    new SqlCommand(@"

SELECT WorkflowStatus

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                    cmd.Parameters.AddWithValue(
                        "@TrainingID",
                        Session["TrainingID"]);

                    con.Open();

                    string workflow =
                        Convert.ToString(
                        cmd.ExecuteScalar());

                    con.Close();

                    if (String.IsNullOrWhiteSpace(workflow) ||
                        !workflow.Contains("E"))
                    {
                        lblMessage.ForeColor =
                            Color.Red;

                        lblMessage.Text =
                            "Training has not started yet. Attendance cannot be marked.";

                        gvSession.Visible = false;

                        btnFinalizeAttendance.Visible = false;

                        return;
                    }
                }
                BindSummary();

                BindGrid();
            }
        }

        //--------------------------------------------------------
        // Summary
        //--------------------------------------------------------

        private void BindSummary()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

COUNT(*) TotalSession,

SUM(CASE
WHEN AttendanceStatus='Completed'
THEN 1
ELSE 0
END) Completed

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    int total =
                        Convert.ToInt32(
                        dr["TotalSession"]);

                    int completed =
                        dr["Completed"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(
                        dr["Completed"]);

                    int pending =
                        total - completed;

                    lblTotalSession.Text =
                        total.ToString();

                    lblCompleted.Text =
                        completed.ToString();

                    lblPending.Text =
                        pending.ToString();

                    if (total == 0)
                    {
                        lblProgress.Text = "0 %";
                    }
                    else
                    {
                        lblProgress.Text =
                            Math.Round(
                            ((decimal)completed /
                            total) * 100, 2)
                            + " %";
                    }

                    //----------------------------------

                    btnFinalizeAttendance.Visible =
                        (total > 0 &&
                         completed == total);
                }

                dr.Close();

                con.Close();
            }
        }

        //--------------------------------------------------------
        // Grid
        //--------------------------------------------------------

        private void BindGrid()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlDataAdapter da =
                new SqlDataAdapter(@"

SELECT

SM.SessionID,

SM.SessionNo,

SM.SessionName,

SM.SessionDate,

SM.StartTime,

SM.EndTime,

SM.TotalHours,

SM.AttendanceStatus,

TM.TopicName,

CASE

WHEN TR.TrainerType='Internal'

THEN E.EmpName

ELSE TR.NameExternal

END TrainerName

FROM SessionMaster SM

LEFT JOIN TopicMaster TM

ON TM.TopicID=SM.TopicID

LEFT JOIN TrainerMaster TR

ON TR.TrainerID=SM.TrainerID

LEFT JOIN EmpBasicMaster E

ON E.EmpID=TR.EmpID

WHERE SM.TrainingID=@TrainingID

ORDER BY CAST(SM.SessionNo AS INT)

", con);

                da.SelectCommand.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                DataTable dt =
                    new DataTable();

                da.Fill(dt);

                gvSession.DataSource =
                    dt;

                gvSession.DataBind();
            }
        }

        //--------------------------------------------------------
        // Grid Command
        //--------------------------------------------------------

        protected void gvSession_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Attendance")
            {
                Session["SessionID"] =
                    e.CommandArgument.ToString();

                Response.Redirect(
                    "SessionAttendance.aspx");
            }
        }

        //--------------------------------------------------------
        // Final Attendance
        //--------------------------------------------------------

        protected void btnFinalizeAttendance_Click(
            object sender,
            EventArgs e)
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                con.Open();

                //----------------------------------

                SqlCommand chk =
                new SqlCommand(@"

SELECT COUNT(*)

FROM SessionMaster

WHERE

TrainingID=@TrainingID

AND

AttendanceStatus='Pending'

", con);

                chk.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                int pending =
                    Convert.ToInt32(
                    chk.ExecuteScalar());

                if (pending > 0)
                {
                    lblMessage.ForeColor =
                        Color.Red;

                    lblMessage.Text =
                        "Attendance of all Sessions is not completed.";

                    return;
                }

                //----------------------------------

                clsWorkflow.UpdateWorkflow(

                    Session["TrainingID"].ToString(),

                    "AttendanceCompleted",

                    "F");

                //----------------------------------

                lblMessage.ForeColor =
                    Color.Green;

                lblMessage.Text =
                    "Attendance Finalized Successfully.";

                btnFinalizeAttendance.Visible =
                    false;
            }
        }

    }
}