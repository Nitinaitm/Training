using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using Training.Business.Certificate;

namespace Training.Trainee
{
    public partial class TraineeFeedback :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            if (Session["EmpID"] == null
                || string.IsNullOrWhiteSpace(
                    Session["EmpID"].ToString()))
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if (Session["TrainingID"] == null
                || string.IsNullOrWhiteSpace(
                    Session["TrainingID"].ToString()))
            {
                Response.Redirect(
                    "MyTrainings.aspx");

                return;
            }

            if (!IsPostBack)
            {
                ViewState["EmpID"] =
                    Session["EmpID"].ToString().ToUpperInvariant();

                ViewState["TrainingID"] =
                    Session["TrainingID"].ToString();

                string trainingID =
                    Session["TrainingID"].ToString();

                string empID =
                    Session["EmpID"].ToString().ToUpperInvariant();

                TraineeTrainingSummary1.LoadTraining(
                    trainingID,
                    empID);

                if (String.Equals(Request.QueryString["mode"], "view", StringComparison.OrdinalIgnoreCase))
                {
                    BuildFeedback();
                    LoadExistingFeedback();
                    SetFeedbackReadOnly();
                    btnSubmit.Visible = false;
                    lblMessage.Text = "Feedback submitted. You are viewing it in read-only mode.";
                    lblMessage.ForeColor = System.Drawing.Color.Green;
                    return;
                }

                if (!CanSubmitFeedback())
                {
                    lblMessage.Text =
                        "Feedback is not available. Please complete all required training activities first.";

                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    btnSubmit.Enabled =
                        false;

                    phFeedback.Visible =
                        false;

                    return;
                }

                if (IsFeedbackSubmitted())
                {
                    lblMessage.Text =
                        "Feedback already submitted.";

                    lblMessage.ForeColor =
                        System.Drawing.Color.Green;

                    btnSubmit.Enabled =
                        false;

                    phFeedback.Visible =
                        false;

                    return;
                }
            }

            BuildFeedback();
        }

        private void LoadExistingFeedback()
        {
            string sql = "SELECT FD.QuestionID,FD.SessionID,FD.TrainerID,FD.Rating,FD.Answer FROM Feedback F INNER JOIN FeedbackDetail FD ON FD.FeedbackID=F.FeedbackID WHERE F.TrainingID=@TrainingID AND F.EmpID=@EmpID AND F.Submitted=1";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TrainingID", Session["TrainingID"].ToString()), new SqlParameter("@EmpID", Session["EmpID"].ToString().ToUpperInvariant()) });
            foreach (DataRow row in dt.Rows)
            {
                string questionID = Convert.ToString(row["QuestionID"]);
                string sessionID = row["SessionID"] == DBNull.Value ? "" : Convert.ToString(row["SessionID"]);
                string trainerID = row["TrainerID"] == DBNull.Value ? "" : Convert.ToString(row["TrainerID"]);
                string controlID = "ANS_" + questionID + "_" + sessionID + "_" + trainerID;
                Control answer = phFeedback.FindControl(controlID);
                if (answer == null) continue;
                if (answer is RadioButtonList)
                {
                    RadioButtonList rbl = (RadioButtonList)answer;
                    string value = "";
                    if (row["Rating"] != DBNull.Value && Convert.ToInt32(row["Rating"]) > 0) value = Convert.ToString(row["Rating"]);
                    if (String.IsNullOrWhiteSpace(value)) value = row["Answer"] == DBNull.Value ? "" : Convert.ToString(row["Answer"]);
                    if (!String.IsNullOrWhiteSpace(value) && rbl.Items.FindByValue(value) != null) rbl.SelectedValue = value;
                }
                else if (answer is TextBox)
                {
                    ((TextBox)answer).Text = row["Answer"] == DBNull.Value ? "" : Convert.ToString(row["Answer"]);
                }
            }
        }

        private void SetFeedbackReadOnly()
        {
            foreach (Control ctrl in phFeedback.Controls)
            {
                Panel panel = ctrl as Panel;
                if (panel == null) continue;
                panel.Enabled = false;
                panel.CssClass = "question-row feedback-readonly";
            }
        }

        private bool CanSubmitFeedback()
        {
            string query =
                "SELECT " +
                "TD.AttendanceRequired," +
                "TD.InitialAssessmentRequired," +
                "TD.FinalAssessmentRequired," +
                "TD.FeedbackRequired," +
                "ISNULL(TD.FeedbackSkipped,0) AS FeedbackSkipped " +
                "FROM TrainingDetails TD " +
                "WHERE TD.TrainingID=@TrainingID";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    Session["TrainingID"].ToString())
            };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            DataRow row = dt.Rows[0];

            bool attendanceRequired =
                Convert.ToBoolean(
                    row["AttendanceRequired"]);

            bool preRequired =
                Convert.ToBoolean(
                    row["InitialAssessmentRequired"]);

            bool postRequired =
                Convert.ToBoolean(
                    row["FinalAssessmentRequired"]);

            bool feedbackRequired =
                Convert.ToBoolean(
                    row["FeedbackRequired"]);

            bool feedbackSkipped =
                Convert.ToBoolean(
                    row["FeedbackSkipped"]);

            if (!feedbackRequired || feedbackSkipped)
            {
                return false;
            }

            query =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID " +
                "AND AssignmentStatus='Assigned'";

            param =
            new SqlParameter[]
            {
                new SqlParameter(
                    "@TrainingID",
                    Session["TrainingID"].ToString()),

                new SqlParameter(
                    "@EmpID",
                    Session["EmpID"].ToString().ToUpperInvariant())
            };

            if (Convert.ToInt32(
                objDB.ExecuteScalar(
                    query,
                    param)) == 0)
            {
                return false;
            }

            if (attendanceRequired)
            {
                query =
                    "SELECT COUNT(*) " +
                    "FROM SessionMaster " +
                    "WHERE TrainingID=@TrainingID " +
                    "AND ISNULL(AttendanceSkipped,0)=0 " +
                    "AND ISNULL(AttendanceStatus,'')<>'Completed'";

                param =
                new SqlParameter[]
                {
                    new SqlParameter(
                        "@TrainingID",
                        Session["TrainingID"].ToString())
                };

                if (Convert.ToInt32(
                    objDB.ExecuteScalar(
                        query,
                        param)) > 0)
                {
                    return false;
                }
            }

            if (preRequired)
            {
                query =
                    "SELECT COUNT(*) " +
                    "FROM SessionMaster SM " +
                    "WHERE SM.TrainingID=@TrainingID " +
                    "AND ISNULL(SM.PreAssessmentSkipped,0)=0 " +
                    "AND (" +
                    "NOT EXISTS (" +
                    "SELECT 1 FROM TestMaster TM " +
                    "WHERE TM.SessionID=SM.SessionID " +
                    "AND TM.TestType='Pre' " +
                    "AND TM.IsPublished=1" +
                    ") " +
                    "OR NOT EXISTS (" +
                    "SELECT 1 FROM TestMaster TM " +
                    "INNER JOIN TestResult TR " +
                    "ON TR.TestID=TM.TestID " +
                    "AND TR.EmpID=@EmpID " +
                    "WHERE TM.SessionID=SM.SessionID " +
                    "AND TM.TestType='Pre' " +
                    "AND TM.IsPublished=1 " +
                    "AND TR.IsFinalAttempt=1" +
                    ")" +
                    ")";

                param =
                new SqlParameter[]
                {
                    new SqlParameter(
                        "@TrainingID",
                        Session["TrainingID"].ToString()),

                    new SqlParameter(
                        "@EmpID",
                        Session["EmpID"].ToString().ToUpperInvariant())
                };

                if (Convert.ToInt32(
                    objDB.ExecuteScalar(
                        query,
                        param)) > 0)
                {
                    return false;
                }
            }

            if (postRequired)
            {
                query =
                    "SELECT COUNT(*) " +
                    "FROM SessionMaster SM " +
                    "WHERE SM.TrainingID=@TrainingID " +
                    "AND ISNULL(SM.PostAssessmentSkipped,0)=0 " +
                    "AND (" +
                    "NOT EXISTS (" +
                    "SELECT 1 FROM TestMaster TM " +
                    "WHERE TM.SessionID=SM.SessionID " +
                    "AND TM.TestType='Post' " +
                    "AND TM.IsPublished=1" +
                    ") " +
                    "OR NOT EXISTS (" +
                    "SELECT 1 FROM TestMaster TM " +
                    "INNER JOIN TestResult TR " +
                    "ON TR.TestID=TM.TestID " +
                    "AND TR.EmpID=@EmpID " +
                    "WHERE TM.SessionID=SM.SessionID " +
                    "AND TM.TestType='Post' " +
                    "AND TM.IsPublished=1 " +
                    "AND TR.IsFinalAttempt=1" +
                    ")";

                if (preRequired)
                {
                    query +=
                        " OR (" +
                        "ISNULL(SM.PreAssessmentSkipped,0)=0 " +
                        "AND (" +
                        "NOT EXISTS (" +
                        "SELECT 1 FROM TestMaster TP " +
                        "WHERE TP.SessionID=SM.SessionID " +
                        "AND TP.TestType='Pre' " +
                        "AND TP.IsPublished=1" +
                        ") " +
                        "OR NOT EXISTS (" +
                        "SELECT 1 FROM TestMaster TP " +
                        "INNER JOIN TestResult RP " +
                        "ON RP.TestID=TP.TestID " +
                        "AND RP.EmpID=@EmpID " +
                        "WHERE TP.SessionID=SM.SessionID " +
                        "AND TP.TestType='Pre' " +
                        "AND TP.IsPublished=1 " +
                        "AND RP.IsFinalAttempt=1" +
                        ")" +
                        ")" +
                        ")";
                }

                query +=
                    ")";

                param =
                new SqlParameter[]
                {
                    new SqlParameter(
                        "@TrainingID",
                        Session["TrainingID"].ToString()),

                    new SqlParameter(
                        "@EmpID",
                        Session["EmpID"].ToString().ToUpperInvariant())
                };

                if (Convert.ToInt32(
                    objDB.ExecuteScalar(
                        query,
                        param)) > 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void BuildFeedback()
        {
            phFeedback.Controls.Clear();

            DataTable dtCategory =
                GetCategories();

            foreach (DataRow drCategory in dtCategory.Rows)
            {
                BuildCategory(
                    drCategory);
            }
        }

        private DataTable GetCategories()
        {
            string query =
@"
SELECT
FCM.CategoryID,
FCM.CategoryName
FROM
TrainingFeedbackCategory TFC
INNER JOIN
FeedbackCategoryMaster FCM
ON
TFC.CategoryID=FCM.CategoryID
WHERE
TFC.TrainingID=@TrainingID
ORDER BY
FCM.DisplayOrder
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                     Session["TrainingID"]?.ToString())
            };

            return
                objDB.GetDataTable(
                query,
                param);
        }

        private void BuildCategory(
            DataRow drCategory)
        {
            string categoryID =
                drCategory["CategoryID"]
                .ToString();

            string categoryName =
                drCategory["CategoryName"]
                .ToString();

            Literal title =
                new Literal();

            title.Text =
                "<div class='card'>" +
                "<div class='card-header bg-primary text-white'>" +
                "<b>" +
                categoryName +
                "</b>" +
                "</div>" +
                "<div class='card-body'>";

            phFeedback.Controls.Add(
                title);

            if (categoryName.ToUpper() == "TRAINER")
            {
                BuildTrainerCategory(
                    categoryID);
            }
            else
            {
                BuildNormalCategory(
                    categoryID);
            }

            Literal footer =
                new Literal();

            footer.Text =
                "</div></div>";

            phFeedback.Controls.Add(
                footer);
        }

        private void BuildNormalCategory(
    string categoryID)
        {
            DataTable dtQuestion =
                GetQuestions(
                categoryID);

            foreach (DataRow drQuestion in dtQuestion.Rows)
            {
                BuildQuestion(
                    drQuestion,
                    "",
                    "",
                    "",
                    "");
            }
        }

        private void BuildTrainerCategory(
     string categoryID)
        {
            DataTable dtSessionTrainer =
                GetSessionTrainerList();

            foreach (DataRow drSession in dtSessionTrainer.Rows)
            {
                Literal sessionTitle =
                    new Literal();

                sessionTitle.Text =
                    "<div class='session-title'>Session: " +
                    drSession["SessionName"].ToString() +
                    "</div>";

                phFeedback.Controls.Add(
                    sessionTitle);

                Literal trainerTitle =
                    new Literal();

                trainerTitle.Text =
                    "<div class='trainer-title'>Trainer: " +
                    drSession["TrainerName"].ToString() +
                    "</div>";

                phFeedback.Controls.Add(
                    trainerTitle);

                DataTable dtQuestion =
                    GetQuestions(
                    categoryID);

                foreach (DataRow drQuestion in dtQuestion.Rows)
                {
                    BuildQuestion(
                        drQuestion,
                        drSession["SessionID"].ToString(),
                        drSession["SessionName"].ToString(),
                        drSession["TrainerID"].ToString(),
                        drSession["TrainerType"].ToString());
                }
            }
        }

        private DataTable GetQuestions(
            string categoryID)
        {
            string query =
        @"
SELECT
CategoryID,
QuestionID,
QuestionText,
AnswerType,
Mandatory,
DisplayOrder
FROM
FeedbackQuestionMaster
WHERE
CategoryID=@CategoryID
AND
Active=1
ORDER BY
DisplayOrder,
QuestionText
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryID",
            categoryID)
    };

            return
                objDB.GetDataTable(
                query,
                param);
        }

        private DataTable GetSessionTrainerList()
        {
            string query =
                "SELECT " +
                "SM.SessionID," +
                "SM.SessionName," +
                "TR.TrainerID," +
                "TR.TrainerType," +
                "CASE " +
                "WHEN TR.TrainerType='Internal' " +
                "THEN ISNULL(EB.EmpName,'') " +
                "ELSE ISNULL(TR.NameExternal,'') " +
                "END AS TrainerName " +
                "FROM SessionMaster SM " +
                "INNER JOIN TrainerMaster TR " +
                "ON TR.TrainerID=SM.TrainerID " +
                "LEFT JOIN EmpBasicMaster EB " +
                "ON EB.EmpID=TR.EmpID " +
                "WHERE SM.TrainingID=@TrainingID " +
                "AND ISNULL(SM.TrainerID,'')<>'' " +
                "ORDER BY SM.SessionDate, SM.StartTime, SM.SessionID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"].ToString())
    };

            return
                objDB.GetDataTable(
                    query,
                    param);
        }

        private void BuildQuestion(
     DataRow drQuestion,
     string sessionID,
     string sessionName,
     string trainerID,
     string trainerType)
        {
            string questionID =
                drQuestion["QuestionID"]
                .ToString();

            string answerType =
                drQuestion["AnswerType"]
                .ToString();

            string question =
                drQuestion["QuestionText"]
                .ToString();

            bool mandatory =
                Convert.ToBoolean(
                drQuestion["Mandatory"]);

            string categoryID =
    drQuestion["CategoryID"]
    .ToString();

            Panel pnl =
                new Panel();

            pnl.Attributes["Mandatory"] =
    mandatory.ToString();

            pnl.Attributes["CategoryID"] =
    categoryID;

            pnl.Attributes["QuestionID"] =
    questionID;

            pnl.Attributes["SessionID"] =
                sessionID;

            pnl.Attributes["SessionName"] =
                sessionName;

            pnl.Attributes["TrainerID"] =
                trainerID;

            pnl.Attributes["TrainerType"] =
                trainerType;

            pnl.Attributes["AnswerType"] =
                answerType;

            pnl.CssClass =
                "question-row";

            HiddenField hfQuestion =
                new HiddenField();

            hfQuestion.ID =
                "HFQ_" +
                questionID +
                "_" +
                sessionID +
                "_" +
                trainerID;

            hfQuestion.Value =
                questionID;

            pnl.Controls.Add(
                hfQuestion);

            HiddenField hfTrainer =
                new HiddenField();

            hfTrainer.ID =
                "HFT_" +
                questionID +
                "_" +
                sessionID +
                "_" +
                trainerID;

            hfTrainer.Value =
                trainerID;

            pnl.Controls.Add(
                hfTrainer);

            HiddenField hfTrainerType =
                new HiddenField();

            hfTrainerType.ID =
                "HFTYPE_" +
                questionID +
                "_" +
                sessionID +
                "_" +
                trainerID;

            hfTrainerType.Value =
                trainerType;

            pnl.Controls.Add(
                hfTrainerType);

            Literal lbl =
     new Literal();

            lbl.Text =
"<div class='question-label'>" +
question +
(mandatory
? "<span style='color:red;'> *</span>"
: "")
+
"</div>";

            Control answerControl =
    null;

            pnl.Controls.Add(
                lbl);

            if (answerType == "Rating")
            {
                RadioButtonList rbl =
                    new RadioButtonList();

                rbl.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID;

                rbl.RepeatDirection =
    RepeatDirection.Horizontal;

                rbl.RepeatLayout =
                    RepeatLayout.Flow;

                rbl.CssClass =
                    "star-rating";
                rbl.Items.Add(
     new ListItem("★", "1"));

                rbl.Items.Add(
                    new ListItem("★", "2"));

                rbl.Items.Add(
                    new ListItem("★", "3"));

                rbl.Items.Add(
                    new ListItem("★", "4"));

                rbl.Items.Add(
                    new ListItem("★", "5"));

                answerControl =
    rbl;
            }
            else if (answerType == "YesNo")
            {
                RadioButtonList rbl =
                    new RadioButtonList();

                rbl.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID;

                rbl.RepeatDirection =
                    RepeatDirection.Horizontal;

                rbl.Items.Add(
                    new ListItem(
                    "Yes",
                    "Yes"));

                rbl.Items.Add(
                    new ListItem(
                    "No",
                    "No"));

                answerControl =
    rbl;
            }
            else if (answerType == "Text")
            {
                TextBox txt =
                    new TextBox();

                txt.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID;

                txt.CssClass =
                    "form-control";

                txt.MaxLength =
                    200;

                answerControl =
     txt;
            }
            else if (answerType == "TextArea")
            {
                TextBox txt =
                    new TextBox();

                txt.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID;

                txt.CssClass =
                    "form-control";

                txt.TextMode =
                    TextBoxMode.MultiLine;

                txt.Rows =
                    4;

                answerControl =
     txt;
            }
            else if (answerType == "Number")
            {
                TextBox txt =
                    new TextBox();

                txt.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID;

                txt.CssClass =
                    "form-control";

                txt.Attributes["type"] =
"number";

                answerControl =
    txt;
            }

            if (answerControl != null)
            {
                pnl.Controls.Add(
                    answerControl);
            }

            phFeedback.Controls.Add(
                pnl);
        }

        private bool ValidateFeedback()
        {
            foreach (Control ctrl in phFeedback.Controls)
            {
                Panel pnl =
                    ctrl as Panel;

                if (pnl == null)
                {
                    continue;
                }

                if (pnl.Attributes["QuestionID"] == null)
                {
                    continue;
                }

                bool mandatory =
                    Convert.ToBoolean(
                    pnl.Attributes["Mandatory"]);

                if (!mandatory)
                {
                    continue;
                }

                string questionID =
                    pnl.Attributes["QuestionID"];

                string trainerID =
                    pnl.Attributes["TrainerID"];

                string answerType =
                    pnl.Attributes["AnswerType"];

                string sessionID =
    pnl.Attributes["SessionID"];

                Control ans =
                    pnl.FindControl(
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID);
                if (ans == null)
                {
                    continue;
                }

                if (answerType == "Rating")
                {
                    RadioButtonList rbl =
                        (RadioButtonList)ans;

                    if (rbl.SelectedIndex < 0)
                    {
                        lblMessage.Text =
                            "Please answer all mandatory questions.";

                        lblMessage.ForeColor =
                            System.Drawing.Color.Red;

                        return false;
                    }
                }
                else if (answerType == "YesNo")
                {
                    RadioButtonList rbl =
                        (RadioButtonList)ans;

                    if (String.IsNullOrEmpty(
                        rbl.SelectedValue))
                    {
                        lblMessage.Text =
                            "Please answer all mandatory questions.";

                        lblMessage.ForeColor =
                            System.Drawing.Color.Red;

                        return false;
                    }
                }
                else
                {
                    TextBox txt =
                        (TextBox)ans;

                    if (String.IsNullOrWhiteSpace(
                        txt.Text))
                    {
                        lblMessage.Text =
                            "Please answer all mandatory questions.";

                        lblMessage.ForeColor =
                            System.Drawing.Color.Red;

                        return false;
                    }
                }
            }

            return true;
        }

        private string GenerateFeedbackID()
        {
            return Guid.NewGuid()
.ToString("N")
.ToUpper();
        }
        private string GenerateFeedbackDetailID()
        {
            return Guid.NewGuid()
.ToString("N")
.ToUpper();
        }

        protected void btnSubmit_Click(
   object sender,
   EventArgs e)
        {
            try
            {
                if (!CanSubmitFeedback())
                {
                    lblMessage.Text =
                        "Feedback cannot be submitted until all required training activities are completed.";

                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    return;
                }
                if (IsFeedbackSubmitted())
                {
                    lblMessage.Text =
                        "Feedback already submitted.";

                    lblMessage.ForeColor =
                        System.Drawing.Color.Red;

                    return;
                }

                if (!ValidateFeedback())
                {
                    return;
                }

                string trainingID =
                    Session["TrainingID"]
                    .ToString();

                string empID =
                    Session["EmpID"]
                    .ToString().ToUpperInvariant();

                string feedbackID =
                    GenerateFeedbackID();

                SaveFeedback(
                    feedbackID);

                SaveFeedbackDetails(
                    feedbackID);

                UpdateTrainingProgress();

                btnSubmit.Enabled =
                    false;

                bool certificateGenerated =
                    TryGenerateCertificate(
                    trainingID,
                    empID);

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                if (certificateGenerated)
                {
                    Session["CertificateFromTraining"] = "1";
                    Response.Redirect("MyCertificate.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                else
                {
                    lblMessage.Text =
                        "Feedback submitted successfully. Certificate generation is pending.";
                }
            }
            catch (Exception ex)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Unable to submit feedback. "
                    +
                    ex.Message;
            }
        }

        private bool TryGenerateCertificate(
    string trainingID,
    string empID)
        {
            try
            {
                CertificateGenerator generator =
                    new CertificateGenerator();

                return
                    generator.GenerateCertificate(
                    trainingID,
                    empID);
            }
            catch (Exception ex)
            {
                LogCertificateError(
                    trainingID,
                    empID,
                    ex);

                return false;
            }
        }

        private void LogCertificateError(
    string trainingID,
    string empID,
    Exception ex)
        {
            try
            {
                string query =
        @"
INSERT INTO CertificateGenerationLog
(
TrainingID,
EmpID,
ErrorMessage,
ErrorDetails,
CreatedOn
)
VALUES
(
@TrainingID,
@EmpID,
@ErrorMessage,
@ErrorDetails,
GETDATE()
)
";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@TrainingID",
                trainingID),

            new SqlParameter(
                "@EmpID",
                empID),

            new SqlParameter(
                "@ErrorMessage",
                ex.Message),

            new SqlParameter(
                "@ErrorDetails",
                ex.ToString())
        };

                objDB.ExecuteSql(
                    query,
                    param);
            }
            catch
            {
            }
        }

        private bool IsFeedbackSubmitted()
        {
            string query =
@"
SELECT
COUNT(*)
FROM
Feedback
WHERE
TrainingID=@TrainingID
AND
EmpID=@EmpID
AND
Submitted=1
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@TrainingID",
                    Session["TrainingID"]),

                new SqlParameter(
                    "@EmpID",
                    Session["EmpID"])
            };

            return
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param))
                > 0;
        }

        private void SaveFeedback(
    string feedbackID)
        {
            string query =
        @"
INSERT INTO
Feedback
(
FeedbackID,
TrainingID,
EmpID,
Submitted,
SubmittedOn,
CreatedOn,
CreatedBy
)
VALUES
(
@FeedbackID,
@TrainingID,
@EmpID,
1,
GETDATE(),
GETDATE(),
@EmpID
)
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@FeedbackID",
            feedbackID),

        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"]?.ToString()),

        new SqlParameter(
            "@EmpID",
            Session["EmpID"]?.ToString().ToUpperInvariant())
    };

            objDB.ExecuteSql(
                query,
                param);
        }

        private void UpdateTrainingProgress()
        {
            string query =
                "UPDATE TrainingProgress " +
                "SET " +
                "BatchFeedbackCompleted=1," +
                "UpdatedOn=GETDATE()," +
                "UpdatedBy=@EmpID " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"].ToString()),

        new SqlParameter(
            "@EmpID",
            Session["EmpID"].ToString().ToUpperInvariant())
    };

            objDB.ExecuteSql(
                query,
                param);
        }

        private void SaveFeedbackDetails(
    string feedbackID)
        {
            foreach (Control ctrl in phFeedback.Controls)
            {
                if (!(ctrl is Panel))
                {
                    continue;
                }

                Panel pnl =
                    (Panel)ctrl;

                if (String.IsNullOrEmpty(
                    pnl.Attributes["QuestionID"]))
                {
                    continue;
                }

                string questionID =
                    pnl.Attributes["QuestionID"];

                string categoryID =
                    pnl.Attributes["CategoryID"];

                string sessionID =
                    pnl.Attributes["SessionID"];

                string trainerID =
                    pnl.Attributes["TrainerID"];

                string trainerType =
                    pnl.Attributes["TrainerType"];

                string answerType =
                    pnl.Attributes["AnswerType"];

                int rating =
                    0;

                string answer =
                    "";

                Control ans =
                    pnl.FindControl(
                    "ANS_" +
                    questionID +
                    "_" +
                    sessionID +
                    "_" +
                    trainerID);

                if (ans == null)
                {
                    continue;
                }

                if (answerType == "Rating")
                {
                    RadioButtonList rbl =
                        (RadioButtonList)ans;

                    if (rbl.SelectedIndex >= 0)
                    {
                        rating =
                            Convert.ToInt32(
                            rbl.SelectedValue);
                    }
                }
                else if (answerType == "YesNo")
                {
                    RadioButtonList rbl =
                        (RadioButtonList)ans;

                    answer =
                        rbl.SelectedValue;
                }
                else
                {
                    TextBox txt =
                        (TextBox)ans;

                    answer =
                        txt.Text.Trim();
                }

                string query =
        @"
INSERT INTO
FeedbackDetail
(
FeedbackDetailID,
FeedbackID,
TrainingID,
EmpID,
CategoryID,
QuestionID,
SessionID,
TrainerID,
TrainerType,
AnswerType,
Rating,
Answer,
CreatedOn
)
VALUES
(
@FeedbackDetailID,
@FeedbackID,
@TrainingID,
@EmpID,
@CategoryID,
@QuestionID,
@SessionID,
@TrainerID,
@TrainerType,
@AnswerType,
@Rating,
@Answer,
GETDATE()
)
";

                SqlParameter[] param =
                {
            new SqlParameter(
                "@FeedbackDetailID",
                GenerateFeedbackDetailID()),

            new SqlParameter(
                "@FeedbackID",
                feedbackID),

            new SqlParameter(
                "@TrainingID",
                Session["TrainingID"]?.ToString()),

            new SqlParameter(
                "@EmpID",
                Session["EmpID"]?.ToString().ToUpperInvariant()),

            new SqlParameter(
                "@CategoryID",
                categoryID),

            new SqlParameter(
                "@QuestionID",
                questionID),

            new SqlParameter(
                "@SessionID",
                String.IsNullOrWhiteSpace(sessionID)
                ? (object)DBNull.Value
                : sessionID),

            new SqlParameter(
                "@TrainerID",
                String.IsNullOrWhiteSpace(trainerID)
                ? (object)DBNull.Value
                : trainerID),

            new SqlParameter(
                "@TrainerType",
                String.IsNullOrWhiteSpace(trainerType)
                ? (object)DBNull.Value
                : trainerType),

            new SqlParameter(
                "@AnswerType",
                answerType),

            new SqlParameter(
                "@Rating",
                rating == 0
                ? (object)DBNull.Value
                : rating),

            new SqlParameter(
                "@Answer",
                String.IsNullOrWhiteSpace(answer)
                ? (object)DBNull.Value
                : answer)
        };

                objDB.ExecuteSql(
                    query,
                    param);
            }
        }
    }
}
