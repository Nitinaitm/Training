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
            if (Session["EmpID"] == null)
            {
                Response.Redirect(
                    "~/Default.aspx");

                return;
            }

            if (Session["TrainingID"] == null)
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

                TraineeTrainingSummary1.LoadTraining(
                    Session["TrainingID"].ToString(),
                    Session["EmpID"].ToString());

                string trainingID =
        Session["TrainingID"].ToString();

                string empID =
                    Session["EmpID"].ToString().ToUpperInvariant();

                TraineeTrainingSummary1.LoadTraining(
                    trainingID,
                    empID);


                if (!CanSubmitFeedback())
                {
                    lblMessage.Text =
                        "Feedback is not available. Please complete all required published tests first.";

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

        private bool CanSubmitFeedback()
        {
            string query =
                "SELECT " +
                "COUNT(*) AS PublishedTests," +
                "COUNT(TA.TestID) AS CompletedTests " +
                "FROM TestMaster TM " +
                "INNER JOIN SessionMaster SM " +
                "ON SM.SessionID=TM.SessionID " +
                "LEFT JOIN " +
                "(" +
                "SELECT DISTINCT TestID " +
                "FROM TestAttempt " +
                "WHERE EmpID=@EmpID " +
                "AND Submitted=1" +
                ") TA " +
                "ON TA.TestID=TM.TestID " +
                "WHERE SM.TrainingID=@TrainingID " +
                "AND TM.IsPublished=1";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@TrainingID",
            Session["TrainingID"].ToString()),

        new SqlParameter(
            "@EmpID",
            Session["EmpID"].ToString().ToUpperInvariant())
    };

            DataTable dt =
                objDB.GetDataTable(
                    query,
                    param);

            if (dt.Rows.Count == 0)
            {
                return false;
            }

            int publishedTests =
                Convert.ToInt32(
                    dt.Rows[0]["PublishedTests"]);

            int completedTests =
                Convert.ToInt32(
                    dt.Rows[0]["CompletedTests"]);

            if (publishedTests == 0)
            {
                return false;
            }

            if
            (
                publishedTests
                !=
                completedTests
            )
            {
                return false;
            }

            return true;
        }
        //-----------------------------------------------------
        // Build Feedback
        //-----------------------------------------------------

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

        //-----------------------------------------------------
        // Get Categories
        //-----------------------------------------------------

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

        //-----------------------------------------------------
        // Build Category
        //-----------------------------------------------------

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
        //-----------------------------------------------------
        // Build Normal Category
        //-----------------------------------------------------

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
                    "");
            }
        }

        //-----------------------------------------------------
        // Build Trainer Category
        //-----------------------------------------------------

        private void BuildTrainerCategory(
     string categoryID)
        {
            DataTable dtTrainer =
                GetTrainerList();

            foreach (DataRow drTrainer in dtTrainer.Rows)
            {
                Literal trainerTitle =
                    new Literal();

                trainerTitle.Text =
                    "<div class='trainer-title'>" +
                    drTrainer["TrainerName"].ToString() +
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
                        drTrainer["TrainerID"].ToString(),
                        drTrainer["TrainerType"].ToString());
                }
            }
        }

        //-----------------------------------------------------
        // Get Questions
        //-----------------------------------------------------

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

        //-----------------------------------------------------
        // Get Trainer List
        //-----------------------------------------------------

        private DataTable GetTrainerList()
        {
            string query =
                "SELECT DISTINCT " +
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
                "ORDER BY TrainerName";

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
        //-----------------------------------------------------
        // Build Question
        //-----------------------------------------------------

        private void BuildQuestion(
     DataRow drQuestion,
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
                trainerID;

            hfTrainerType.Value =
                trainerType;

            pnl.Controls.Add(
                hfTrainerType);

            Literal lbl =
     new Literal();

            //lbl.ID =
            //    "LBL_" +
            //    questionID +
            //    "_" +
            //    trainerID;

            //lbl.Text =
            //    question +
            //    (mandatory
            //    ? " <span style='color:red;'>*</span>"
            //    : "");
            lbl.Text =
"<div class='question-label'>" +
question +
(mandatory
? "<span style='color:red;'> *</span>"
: "")
+
"</div>";

            //lbl.CssClass =
            //    "question-label";

            Control answerControl =
    null;

            pnl.Controls.Add(
                lbl);

            //-------------------------------------------------
            // Rating
            //-------------------------------------------------

            if (answerType == "Rating")
            {
                RadioButtonList rbl =
                    new RadioButtonList();

                rbl.ID =
                    "ANS_" +
                    questionID +
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

            //-------------------------------------------------
            // Yes No
            //-------------------------------------------------

            else if (answerType == "YesNo")
            {
                RadioButtonList rbl =
                    new RadioButtonList();

                rbl.ID =
                    "ANS_" +
                    questionID +
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

            //-------------------------------------------------
            // Text
            //-------------------------------------------------

            else if (answerType == "Text")
            {
                TextBox txt =
                    new TextBox();

                txt.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    trainerID;

                txt.CssClass =
                    "form-control";

                txt.MaxLength =
                    200;

                answerControl =
     txt;
            }

            //-------------------------------------------------
            // Text Area
            //-------------------------------------------------

            else if (answerType == "TextArea")
            {
                TextBox txt =
                    new TextBox();

                txt.ID =
                    "ANS_" +
                    questionID +
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

            //-------------------------------------------------
            // Number
            //-------------------------------------------------

            else if (answerType == "Number")
            {
                TextBox txt =
                    new TextBox();

                txt.ID =
                    "ANS_" +
                    questionID +
                    "_" +
                    trainerID;

                txt.CssClass =
                    "form-control";

                //txt.TextMode =
                //    TextBoxMode.Number;
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

                Control ans =
                    pnl.FindControl(
                    "ANS_" +
                    questionID +
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
            //Random rnd =
            //    new Random();

            //return
            //    "FDB" +
            //    DateTime.Now.ToString("yyyyMMddHHmmssfff") +
            //    rnd.Next(1000, 9999).ToString();
            return Guid.NewGuid()
.ToString("N")
.ToUpper();
        }
        private string GenerateFeedbackDetailID()
        {
            //Random rnd =
            //    new Random();

            //return
            //    "FDD" +
            //    DateTime.Now.ToString("yyyyMMddHHmmssfff") +
            //    rnd.Next(1000, 9999).ToString();
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
                        "Feedback cannot be submitted until all required published tests are completed.";

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

                //btnCancel.Enabled =
                //    false;

                bool certificateGenerated =
                    TryGenerateCertificate(
                    trainingID,
                    empID);

                lblMessage.ForeColor =
                    System.Drawing.Color.Green;

                if (certificateGenerated)
                {
                    lblMessage.Text =
                        "Feedback submitted successfully. Your certificate has been generated.";
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
                // Logging failure must not affect feedback submission.
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
                "@TrainerID",
                trainerID),

            new SqlParameter(
                "@TrainerType",
                trainerType),

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