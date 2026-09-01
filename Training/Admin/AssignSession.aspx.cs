using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;

using System.Data;

using System.Data.SqlClient;

using System.Drawing;

using System.Globalization;
using System.Web.Services;

namespace Training.Admin
{

    public partial class AssignSession : System.Web.UI.Page
    {

        string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;

            if (Session["TrainingID"] == null)
            {
                Response.Redirect("~/Admin/TrainingList.aspx");
                return;
            }
            TrainingSummary1.LoadTraining(Session["TrainingID"].ToString());
            lblTrainingID.Text = Session["TrainingID"].ToString();

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT Hours FROM TrainingDetails WHERE TrainingID=@TrainingID",
                    con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                object obj = cmd.ExecuteScalar();

                lblTrainingHours.Text =
                    obj == null ? "0" : obj.ToString();

                con.Close();
            }
            if (!IsPostBack)
            {
                BindStartTime();

                BindTopic();

                BindTrainer();


                GenerateSessionNo();

                GenerateSessionID();

                BindGrid();

                BindSummary();

                ScriptManager.RegisterStartupScript(
                       this,
                       GetType(),
                       "Init",
                       "initControls();calculateHours();",
                       true);
            }
        }

        private void LoadTrainingDates(
      out DateTime batchFrom,
      out DateTime batchTo)
        {
            batchFrom = DateTime.MinValue;

            batchTo = DateTime.MinValue;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

DateFrom,
DateTo

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    lblTrainingID.Text.Trim());

                con.Open();

                SqlDataReader dr =
                    cmd.ExecuteReader();

                if (dr.Read())
                {
                    if (!DateTime.TryParseExact(
                            dr["DateFrom"].ToString().Trim(),
                            "dd-MM-yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out batchFrom))
                    {
                        throw new Exception(
                            "Invalid Batch From Date in TrainingDetails.");
                    }

                    if (!DateTime.TryParseExact(
                            dr["DateTo"].ToString().Trim(),
                            "dd-MM-yyyy",
                            CultureInfo.InvariantCulture,
                            DateTimeStyles.None,
                            out batchTo))
                    {
                        throw new Exception(
                            "Invalid Batch To Date in TrainingDetails.");
                    }
                }
                else
                {
                    throw new Exception(
                        "Training details not found.");
                }

                dr.Close();
            }
        }
        private void BindStartTime()
        {
            ddlStartTime.Items.Clear();
            ddlEndTime.Items.Clear();

            ddlStartTime.Items.Add(new ListItem("-- Select Time --", ""));
            ddlEndTime.Items.Add(new ListItem("-- Select Time --", ""));

            DateTime dt = DateTime.Today.AddHours(6);
            DateTime end = DateTime.Today.AddHours(22);

            while (dt <= end)
            {
                string tm = dt.ToString("hh:mm tt");

                ddlStartTime.Items.Add(new ListItem(tm, tm));
                ddlEndTime.Items.Add(new ListItem(tm, tm));

                dt = dt.AddMinutes(15);
            }
        }
        private void BindTopic()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"
SELECT
TopicID,
TopicName
FROM TopicMaster
ORDER BY TopicName", con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlTopic.DataSource = dt;
                ddlTopic.DataTextField = "TopicName";
                ddlTopic.DataValueField = "TopicID";
                ddlTopic.DataBind();

                ddlTopic.Items.Insert(0, new ListItem("-- Select Topic --", ""));
            }
        }
        private void BindTrainer()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"

SELECT

T.TrainerID,

CASE

WHEN T.TrainerType='Internal'


THEN

ISNULL(E.EmpID,'')
+ ' | '
+ ISNULL(E.EmpName,'')

+ CASE
WHEN ISNULL(E.EmpDesignation,'')=''
THEN ''
ELSE ' | ' + E.EmpDesignation
END

+ ' | Internal'


ELSE

T.TrainerID
+ ' | '
+ ISNULL(T.NameExternal,'')

+ CASE
WHEN ISNULL(T.DesignationExternal,'')=''
THEN ''
ELSE ' | ' + T.DesignationExternal
END

+ ' | External'

END AS TrainerDisplay

FROM TrainerMaster T

LEFT JOIN EmpBasicMaster E
ON T.EmpID=E.EmpID

ORDER BY
T.TrainerType,
T.TrainerID

", con);

                DataTable dt = new DataTable();

                da.Fill(dt);

                ddlTrainer.DataSource = dt;

                ddlTrainer.DataTextField = "TrainerDisplay";

                ddlTrainer.DataValueField = "TrainerID";

                ddlTrainer.DataBind();

                ddlTrainer.Items.Insert(0,
                    new ListItem("-- Select Trainer --", ""));
            }
        }
        private void GenerateSessionNo()
        {
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT
ISNULL(MAX(CAST(SessionNo AS INT)),0)+1

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                txtSessionNo.Text =
                    cmd.ExecuteScalar().ToString();

                con.Close();
            }
        }
        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlDataAdapter da = new SqlDataAdapter(@"

SELECT

S.SessionID,
S.SessionNo,
S.SessionName,
S.SessionDate,
S.StartTime,
S.EndTime,
S.TotalHours,

TM.TopicName,

TR.TrainerID,
CASE

WHEN TR.TrainerType='Internal'

THEN E.EmpID

ELSE TR.TrainerID

END AS DisplayTrainerID,
CASE
WHEN TR.TrainerType='Internal'
THEN E.EmpName
ELSE TR.NameExternal
END AS TrainerName,

CASE
WHEN TR.TrainerType='Internal'
THEN E.EmpDesignation
ELSE TR.DesignationExternal
END AS Designation,

TR.TrainerType

FROM SessionMaster S

LEFT JOIN TopicMaster TM
ON TM.TopicID = S.TopicID

LEFT JOIN TrainerMaster TR
ON TR.TrainerID = S.TrainerID

LEFT JOIN EmpBasicMaster E
ON E.EmpID = TR.EmpID

WHERE S.TrainingID=@TrainingID

ORDER BY CAST(S.SessionNo AS INT)

", con);

                da.SelectCommand.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                DataTable dt = new DataTable();

                da.Fill(dt);

                gvSession.DataSource = dt;

                gvSession.DataBind();
            }
        }
        private void BindSummary()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {

                SqlCommand cmd = new SqlCommand(@"

SELECT

COUNT(*) AS TotalSession,

ISNULL(SUM(CAST(TotalHours AS decimal(10,2))),0) AS UsedHours

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lblTotalSessions.Text = dr["TotalSession"].ToString();

                    decimal used = Convert.ToDecimal(dr["UsedHours"]);

                    lblUsedHours.Text = used.ToString("0.00");

                    decimal planned = 0;

                    decimal.TryParse(lblTrainingHours.Text, out planned);

                    decimal remaining = planned - used;

                    if (remaining < 0)
                        remaining = 0;

                    lblRemainingHours.Text = remaining.ToString("0.00");
                }

                dr.Close();

                con.Close();

            }
        }
        [WebMethod]
        public static string GetTrainerExpertise(string trainerID)
        {
            string constr =
            ConfigurationManager
            .ConnectionStrings["constr"]
            .ConnectionString;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

ISNULL(A.ExpertiseName,'')

FROM TrainerMaster T

LEFT JOIN AreaOfExpertiseMaster A

ON T.AreaOfExpertiseID=A.ExpertiseID

WHERE T.TrainerID=@TrainerID

", con);

                cmd.Parameters.AddWithValue(
                "@TrainerID",
                trainerID);

                con.Open();

                object obj =
                cmd.ExecuteScalar();

                if (obj == null)
                    return "";

                return obj.ToString();
            }
        }
        protected void ddlTrainer_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            lblTrainerExpertise.Text = "";

            if (ddlTrainer.SelectedIndex == 0)
                return;

            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT

ISNULL(A.ExpertiseName,'')

FROM TrainerMaster T

LEFT JOIN AreaOfExpertiseMaster A
ON T.AreaOfExpertiseID=A.ExpertiseID

WHERE T.TrainerID=@TrainerID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainerID",
                    ddlTrainer.SelectedValue);

                con.Open();

                object obj =
                    cmd.ExecuteScalar();

                if (obj != null)
                    lblTrainerExpertise.Text =
                        obj.ToString();

                con.Close();
            }
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {

            lblMessage.Text = "";

            if (ddlTopic.SelectedIndex == 0)
            {
                ShowMessage("Please select Topic.", Color.Red);
                return;
            }

            if (ddlTrainer.SelectedIndex == 0)
            {
                ShowMessage("Please select Trainer.", Color.Red);
                return;
            }

            if (!Page.IsValid)
                return;


            DateTime batchFrom;

            DateTime batchTo;

            LoadTrainingDates(
                out batchFrom,
                out batchTo);


            DateTime sessionDate;

            if (!DateTime.TryParseExact(
                    txtSessionDate.Text.Trim(),
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out sessionDate))
            {
                lblMessage.Text = "Please select a valid Session Date.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            if (sessionDate < batchFrom || sessionDate > batchTo)
            {
                lblMessage.Text = "Session Date must be within Training Duration.";
                lblMessage.ForeColor = Color.Red;
                return;
            }


            decimal sessionHours;

            if (!decimal.TryParse(
                    hfTotalHours.Value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out sessionHours))
            {
                ShowMessage("Invalid Session Hours.", Color.Red);
                return;
            }

            if (sessionHours <= 0)
            {
                lblMessage.Text = "Session Hours should be greater than zero.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            //decimal plannedHours = Convert.ToDecimal(lblTrainingHours.Text);
            decimal plannedHours = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"

SELECT Hours

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    lblTrainingID.Text);

                con.Open();

                decimal.TryParse(
                    Convert.ToString(cmd.ExecuteScalar()),
                    out plannedHours);

                con.Close();
            }
            decimal usedHours = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"
SELECT ISNULL(SUM(CAST(TotalHours AS DECIMAL(10,2))),0)
FROM SessionMaster
WHERE TrainingID=@TrainingID", con);

                cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                con.Open();

                usedHours = Convert.ToDecimal(cmd.ExecuteScalar());

                con.Close();
            }

            if ((usedHours + sessionHours) > plannedHours)
            {
                lblMessage.Text = "Total Session Hours cannot exceed Planned Training Hours.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            if (IsDuplicateSession())
            {
                ShowMessage("Session timing overlaps with an existing session.", Color.Red);
                return;
            }

            if (IsTrainerBusy())
            {
                ShowMessage("Selected trainer is already assigned to another session during this time.", Color.Red);
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand chk = new SqlCommand(@"
SELECT COUNT(*)
FROM SessionMaster
WHERE TrainingID=@TrainingID
AND SessionNo=@SessionNo", con, tran);

                    chk.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);
                    chk.Parameters.AddWithValue("@SessionNo", txtSessionNo.Text);

                    int cnt = Convert.ToInt32(chk.ExecuteScalar());

                    if (cnt > 0)
                    {
                        lblMessage.Text = "Session No already exists.";
                        lblMessage.ForeColor = Color.Red;

                        tran.Rollback();
                        return;
                    }

                    // INSERT COMMAND STARTS HERE...
                    // Continue in Part 2A-2
                    SqlCommand cmd = new SqlCommand(@"

INSERT INTO SessionMaster
(
    SessionID,
    TrainingID,
    SessionNo,
    SessionName,
    SessionDate,
    StartTime,
    EndTime,
    TotalHours,
    TopicID,
    TrainerID,
    SessionStatus,
    Remarks,
    CreatedOn,
    CreatedBy
)

VALUES
(
    @SessionID,
    @TrainingID,
    @SessionNo,
    @SessionName,
    @SessionDate,
    @StartTime,
    @EndTime,
    @TotalHours,
    @TopicID,
    @TrainerID,
    'Draft',
    @Remarks,
    GETDATE(),
    @CreatedBy
)

", con, tran);

                    cmd.Parameters.AddWithValue("@SessionID", txtSessionID.Text.Trim());

                    cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text.Trim());

                    cmd.Parameters.AddWithValue("@SessionNo", Convert.ToInt32(txtSessionNo.Text));

                    cmd.Parameters.AddWithValue("@SessionName", txtSessionName.Text.Trim());

                    cmd.Parameters.AddWithValue("@SessionDate", sessionDate.ToString("dd-MM-yyyy"));

                    cmd.Parameters.AddWithValue("@StartTime", ddlStartTime.SelectedValue);

                    cmd.Parameters.AddWithValue("@EndTime", ddlEndTime.SelectedValue);

                    cmd.Parameters.AddWithValue("@TotalHours", sessionHours);

                    cmd.Parameters.AddWithValue("@TopicID", ddlTopic.SelectedValue);

                    cmd.Parameters.AddWithValue("@TrainerID", ddlTrainer.SelectedValue);

                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

                    cmd.Parameters.AddWithValue("@CreatedBy", Session["UserID"] == null
                                                            ? "Admin"
                                                            : Session["UserID"].ToString());

                    cmd.ExecuteNonQuery();

                    tran.Commit();

                    //clsWorkflow.UpdateWorkflow(lblTrainingID.Text.Trim(), "SessionCreated", 23);
                    clsWorkflow.UpdateWorkflow(Session["TrainingID"].ToString(), "SessionCreated", "BC");
                    lblMessage.Text =
                    "Session created successfully";

                    lblMessage.ForeColor = Color.Green;

                    ClearControls();

                    GenerateSessionNo();

                    GenerateSessionID();

                    BindGrid();

                    BindSummary();

                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.Text = ex.Message;

                    lblMessage.ForeColor = Color.Red;
                }
                finally
                {
                    con.Close();
                }

            }
        }
        private void ClearControls()
        {
            txtSessionName.Text = "";

            txtSessionDate.Text = "";

            ddlStartTime.SelectedIndex = 0;

            ddlEndTime.SelectedIndex = 0;

            ddlTopic.SelectedIndex = 0;

            ddlTrainer.SelectedIndex = 0;

            txtTotalHours.Text = "";

            txtRemarks.Text = "";

            ViewState["SessionID"] = null;

            btnSave.Visible = true;

            btnUpdate.Visible = false;

            btnDelete.Visible = false;

            txtSessionName.Focus();
        }
        private void LoadSession(string SessionID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"

SELECT
    *
FROM SessionMaster
WHERE SessionID=@SessionID

", con);

                cmd.Parameters.AddWithValue("@SessionID", SessionID);

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    ViewState["SessionID"] = dr["SessionID"].ToString();

                    txtSessionID.Text = dr["SessionID"].ToString();

                    txtSessionNo.Text = dr["SessionNo"].ToString();

                    txtSessionName.Text = dr["SessionName"].ToString();

                    txtSessionDate.Text = dr["SessionDate"].ToString();

                    if (ddlStartTime.Items.FindByValue(dr["StartTime"].ToString()) != null)
                        ddlStartTime.SelectedValue = dr["StartTime"].ToString();

                    if (ddlEndTime.Items.FindByValue(dr["EndTime"].ToString()) != null)
                        ddlEndTime.SelectedValue = dr["EndTime"].ToString();

                    txtTotalHours.Text = dr["TotalHours"].ToString();

                    if (ddlTopic.Items.FindByValue(dr["TopicID"].ToString()) != null)
                        ddlTopic.SelectedValue = dr["TopicID"].ToString();

                    if (ddlTrainer.Items.FindByValue(dr["TrainerID"].ToString()) != null)
                        ddlTrainer.SelectedValue = dr["TrainerID"].ToString();

                    txtRemarks.Text = dr["Remarks"].ToString();

                    btnSave.Visible = false;

                    btnUpdate.Visible = true;

                    btnDelete.Visible = true;

                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "calcHours",
                        "calculateHours();",
                        true);
                }

                dr.Close();

                con.Close();
            }
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            if (!Page.IsValid)
                return;

            if (ViewState["SessionID"] == null)
            {
                lblMessage.Text = "Please select a Session.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            DateTime batchFrom;

            DateTime batchTo;

            LoadTrainingDates(
                out batchFrom,
                out batchTo);

            DateTime sessionDate;

            if (!DateTime.TryParseExact(txtSessionDate.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out sessionDate))
            {
                lblMessage.Text = "Invalid Session Date.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            if (sessionDate < batchFrom || sessionDate > batchTo)
            {
                lblMessage.Text = "Session Date should be within Training Duration.";
                lblMessage.ForeColor = Color.Red;
                return;
            }
            decimal sessionHours;

            if (!decimal.TryParse(
                    hfTotalHours.Value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out sessionHours))
            {
                ShowMessage("Invalid Session Hours.", Color.Red);
                return;
            }


            if (sessionHours <= 0)
            {
                lblMessage.Text = "Session Hours should be greater than zero.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            if (IsDuplicateSession())
            {
                ShowMessage("Session timing overlaps with an existing session.", Color.Red);
                return;
            }

            if (IsTrainerBusy())
            {
                ShowMessage("Selected trainer is already assigned to another session during this time.", Color.Red);
                return;
            }

            decimal plannedHours = Convert.ToDecimal(lblTrainingHours.Text);
            decimal usedHours = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand hrs = new SqlCommand(@"

SELECT ISNULL(SUM(CAST(TotalHours AS DECIMAL(10,2))),0)

FROM SessionMaster

WHERE TrainingID=@TrainingID

AND SessionID<>@SessionID

", con);

                hrs.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);
                hrs.Parameters.AddWithValue("@SessionID", ViewState["SessionID"].ToString());

                con.Open();

                usedHours = Convert.ToDecimal(hrs.ExecuteScalar());

                con.Close();
            }

            if ((usedHours + sessionHours) > plannedHours)
            {
                lblMessage.Text = "Total Session Hours cannot exceed Planned Training Hours.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(@"

UPDATE SessionMaster

SET

SessionName=@SessionName,
SessionDate=@SessionDate,
StartTime=@StartTime,
EndTime=@EndTime,
TotalHours=@TotalHours,
TopicID=@TopicID,
TrainerID=@TrainerID,
Remarks=@Remarks,
UpdatedOn=GETDATE(),
UpdatedBy=@UpdatedBy

WHERE SessionID=@SessionID

", con, tran);

                    cmd.Parameters.AddWithValue("@SessionID", ViewState["SessionID"].ToString());

                    cmd.Parameters.AddWithValue("@SessionName", txtSessionName.Text.Trim());

                    cmd.Parameters.AddWithValue("@SessionDate", sessionDate.ToString("dd-MM-yyyy"));

                    cmd.Parameters.AddWithValue("@StartTime", ddlStartTime.SelectedValue);

                    cmd.Parameters.AddWithValue("@EndTime", ddlEndTime.SelectedValue);

                    cmd.Parameters.AddWithValue("@TotalHours", sessionHours);

                    cmd.Parameters.AddWithValue("@TopicID", ddlTopic.SelectedValue);

                    cmd.Parameters.AddWithValue("@TrainerID", ddlTrainer.SelectedValue);

                    cmd.Parameters.AddWithValue("@Remarks", txtRemarks.Text.Trim());

                    cmd.Parameters.AddWithValue("@UpdatedBy",
                        Session["UserID"] == null
                        ? "Admin"
                        : Session["UserID"].ToString());

                    cmd.ExecuteNonQuery();

                    tran.Commit();

                    lblMessage.Text = "Session updated successfully.";

                    lblMessage.ForeColor = Color.Green;

                    ClearControls();

                    GenerateSessionNo();

                    GenerateSessionID();

                    BindGrid();

                    BindSummary();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.Text = ex.Message;

                    lblMessage.ForeColor = Color.Red;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        protected void btnFinishSession_Click(
     object sender,
     EventArgs e)
        {
            // Optional Validation
            using (SqlConnection con =
                new SqlConnection(constr))
            {
                SqlCommand cmd =
                new SqlCommand(@"

SELECT COUNT(*)

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    Session["TrainingID"]);

                con.Open();

                int sessionCount =
                    Convert.ToInt32(
                    cmd.ExecuteScalar());

                con.Close();

                if (sessionCount == 0)
                {
                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    lblMessage.Text =
                        "Please create at least one Session before proceeding.";

                    return;
                }
            }

            Response.Redirect(
                "ManageTraining.aspx");
        }
        protected void btnUpdateBatch_Click(
object sender,
EventArgs e)
        {
            Response.Redirect("CreateBatch.aspx?mode=edit");

        }

        protected void btnUpdateTrainee_Click(
object sender,
EventArgs e)
        {
            Response.Redirect("AssignTrainee.aspx");

        }


        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (ViewState["SessionID"] == null)
            {
                lblMessage.Text = "Please select a Session.";
                lblMessage.ForeColor = Color.Red;
                return;
            }

            DeleteSession(ViewState["SessionID"].ToString());
            RenumberSessions();
        }
        private void RenumberSessions()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(@"

SELECT
SessionID

FROM SessionMaster

WHERE TrainingID=@TrainingID

ORDER BY CAST(SessionNo AS INT)

", con, tran);

                    cmd.Parameters.AddWithValue(
                        "@TrainingID",
                        Session["TrainingID"].ToString());

                    SqlDataReader dr = cmd.ExecuteReader();

                    List<string> ids = new List<string>();

                    while (dr.Read())
                    {
                        ids.Add(dr["SessionID"].ToString());
                    }

                    dr.Close();

                    int no = 1;

                    foreach (string id in ids)
                    {
                        SqlCommand upd = new SqlCommand(@"

UPDATE SessionMaster

SET SessionNo=@SessionNo

WHERE SessionID=@SessionID

", con, tran);

                        upd.Parameters.AddWithValue("@SessionNo", no);

                        upd.Parameters.AddWithValue("@SessionID", id);

                        upd.ExecuteNonQuery();

                        no++;
                    }

                    tran.Commit();
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }
            }
        }
        private void DeleteSession(string sessionID)
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlTransaction tran = con.BeginTransaction();

                try
                {
                    SqlCommand cmd = new SqlCommand(@"

DELETE FROM SessionMaster

WHERE SessionID=@SessionID

", con, tran);

                    cmd.Parameters.AddWithValue("@SessionID", sessionID);

                    int rows = cmd.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        tran.Rollback();

                        lblMessage.Text = "Session not found.";

                        lblMessage.ForeColor = Color.Red;

                        return;
                    }

                    tran.Commit();

                    lblMessage.Text = "Session deleted successfully.";

                    lblMessage.ForeColor = Color.Green;

                    ClearControls();



                    RenumberSessions();

                    GenerateSessionNo();

                    GenerateSessionID();


                    BindGrid();

                    BindSummary();
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    lblMessage.Text = ex.Message;

                    lblMessage.ForeColor = Color.Red;
                }
                finally
                {
                    con.Close();
                }
            }
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ResetForm();
        }
        private void ResetForm()
        {
            ViewState["SessionID"] = null;

            ClearControls();

            GenerateSessionNo();

            GenerateSessionID();

            BindGrid();

            BindSummary();

            lblMessage.Text = "";

            btnSave.Visible = true;

            btnUpdate.Visible = false;

            btnDelete.Visible = false;
        }
        protected void gvSession_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRecord")
            {
                LoadSession(e.CommandArgument.ToString());
            }

            else if (e.CommandName == "DeleteRecord")
            {
                DeleteSession(e.CommandArgument.ToString());
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            foreach (GridViewRow row in gvSession.Rows)
            {
                Page.ClientScript.RegisterForEventValidation(
                    gvSession.UniqueID,
                    "EditRecord$" + row.RowIndex);

                Page.ClientScript.RegisterForEventValidation(
                    gvSession.UniqueID,
                    "DeleteRecord$" + row.RowIndex);
            }

            base.Render(writer);
        }
        private bool IsDuplicateSession()
        {
            DateTime newStart = DateTime.ParseExact(
                ddlStartTime.SelectedValue,
                "hh:mm tt",
                CultureInfo.InvariantCulture);

            DateTime newEnd = DateTime.ParseExact(
                ddlEndTime.SelectedValue,
                "hh:mm tt",
                CultureInfo.InvariantCulture);

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"

SELECT
SessionID,
StartTime,
EndTime

FROM SessionMaster

WHERE TrainingID=@TrainingID
AND SessionDate=@SessionDate
AND SessionID<>@SessionID

", con);

                cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                cmd.Parameters.AddWithValue("@SessionDate", txtSessionDate.Text.Trim());

                if (ViewState["SessionID"] == null)
                    cmd.Parameters.AddWithValue("@SessionID", "");
                else
                    cmd.Parameters.AddWithValue("@SessionID", ViewState["SessionID"].ToString());

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    DateTime oldStart = DateTime.ParseExact(
                        dr["StartTime"].ToString(),
                        "hh:mm tt",
                        CultureInfo.InvariantCulture);

                    DateTime oldEnd = DateTime.ParseExact(
                        dr["EndTime"].ToString(),
                        "hh:mm tt",
                        CultureInfo.InvariantCulture);

                    if (newStart < oldEnd && newEnd > oldStart)
                    {
                        dr.Close();
                        con.Close();
                        return true;
                    }
                }

                dr.Close();

                con.Close();
            }

            return false;
        }
        private bool IsTrainerBusy()
        {
            DateTime newStart = DateTime.ParseExact(
                ddlStartTime.SelectedValue,
                "hh:mm tt",
                CultureInfo.InvariantCulture);

            DateTime newEnd = DateTime.ParseExact(
                ddlEndTime.SelectedValue,
                "hh:mm tt",
                CultureInfo.InvariantCulture);

            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"

SELECT
SessionID,
StartTime,
EndTime,
TrainingID

FROM SessionMaster

WHERE TrainerID=@TrainerID
AND SessionDate=@SessionDate
AND SessionID<>@SessionID

", con);

                cmd.Parameters.AddWithValue("@TrainerID", ddlTrainer.SelectedValue);

                cmd.Parameters.AddWithValue("@SessionDate", txtSessionDate.Text.Trim());

                if (ViewState["SessionID"] == null)
                    cmd.Parameters.AddWithValue("@SessionID", "");
                else
                    cmd.Parameters.AddWithValue("@SessionID", ViewState["SessionID"].ToString());

                con.Open();

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    DateTime oldStart = DateTime.ParseExact(
                        dr["StartTime"].ToString(),
                        "hh:mm tt",
                        CultureInfo.InvariantCulture);

                    DateTime oldEnd = DateTime.ParseExact(
                        dr["EndTime"].ToString(),
                        "hh:mm tt",
                        CultureInfo.InvariantCulture);

                    if (newStart < oldEnd && newEnd > oldStart)
                    {
                        dr.Close();
                        con.Close();
                        return true;
                    }
                }

                dr.Close();

                con.Close();
            }

            return false;
        }
        private bool IsSessionDateValid()
        {
            DateTime sessionDate;

            if (!DateTime.TryParseExact(
                txtSessionDate.Text.Trim(),
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out sessionDate))
                return false;

            DateTime batchFrom;

            DateTime batchTo;

            LoadTrainingDates(
                out batchFrom,
                out batchTo);

            return sessionDate >= batchFrom &&
                   sessionDate <= batchTo;
        }
        private decimal GetUsedHours()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd;

                if (ViewState["SessionID"] == null)
                {
                    cmd = new SqlCommand(@"

SELECT
ISNULL(SUM(CAST(TotalHours AS decimal(10,2))),0)

FROM SessionMaster

WHERE TrainingID=@TrainingID

", con);

                    cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);
                }
                else
                {
                    cmd = new SqlCommand(@"

SELECT
ISNULL(SUM(CAST(TotalHours AS decimal(10,2))),0)

FROM SessionMaster

WHERE TrainingID=@TrainingID

AND SessionID<>@SessionID

", con);

                    cmd.Parameters.AddWithValue("@TrainingID", lblTrainingID.Text);

                    cmd.Parameters.AddWithValue("@SessionID", ViewState["SessionID"].ToString());
                }

                con.Open();

                decimal hrs = Convert.ToDecimal(cmd.ExecuteScalar());

                con.Close();

                return hrs;
            }
        }
        private bool ValidateTotalHours(decimal sessionHours)
        {
            decimal plannedHours =
                Convert.ToDecimal(lblTrainingHours.Text);

            decimal usedHours = GetUsedHours();

            return (usedHours + sessionHours) <= plannedHours;
        }
        private void ShowMessage(string message, Color color)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = color;
        }
        private void GenerateSessionID()
        {
            using (SqlConnection con = new SqlConnection(constr))
            {
                SqlCommand cmd = new SqlCommand(@"

SELECT TOP 1 SessionID

FROM SessionMaster

ORDER BY ID DESC

", con);

                con.Open();

                object obj = cmd.ExecuteScalar();

                int next = 1;

                if (obj != null)
                {
                    string lastID = obj.ToString().Replace("SES", "");

                    int.TryParse(lastID, out next);

                    next++;
                }

                txtSessionID.Text = "SES" + next.ToString("000000");

                con.Close();
            }
        }
    }
}