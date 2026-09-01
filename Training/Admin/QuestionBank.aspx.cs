using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class QuestionBank : System.Web.UI.Page
    {
        private clsDataAccess objData = new clsDataAccess();

        //private string ConnectionString =            ConfigurationManager.ConnectionStrings["MyDbConnection"].ConnectionString;

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (
                Session["UserID"] == null
                ||
                Session["UserID"]?.ToString() == ""
               )
            {
                Response.Redirect(
                    "~/Default.aspx",
                    false);

                Context.ApplicationInstance.CompleteRequest();

                return;
            }

            if (
                !IsPostBack
               )
            {
                BindCourse();

                BindSearchCourse();

                BindDifficulty();

                BindLanguage();

                BindGrid();

                txtMarks.Text =
                    "1";

                txtNegativeMarks.Text =
                    "0";

                ddlStatus.SelectedValue =
                    "1";

                ddlQuestionType.SelectedIndex =
                    0;

                ddlLanguage.SelectedIndex =
                    0;
                LoadPlugins();


            }
        }

        private void LoadPlugins()
        {
            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                Guid.NewGuid().ToString(),

                "$('#ddlCourse').select2({width:'100%'});" +
                "$('#ddlTopic').select2({width:'100%'});" +
                "$('#ddlSearchCourse').select2({width:'100%'});" +
                "$('#ddlSearchTopic').select2({width:'100%'});",


                true);
        }


        private void BindCourse()
        {
            string sql =
                "SELECT CourseID,CourseName FROM CourseMaster ORDER BY CourseName";

            DataTable dt =
                objData.GetDataTable(
                sql);

            ddlCourse.DataSource =
                dt;

            ddlCourse.DataTextField =
                "CourseName";

            ddlCourse.DataValueField =
                "CourseID";

            ddlCourse.DataBind();

            ddlCourse.Items.Insert(
                0,
                new ListItem(
                    "-- Select Course --",
                    ""));
        }

        private void BindSearchCourse()
        {
            string sql =
                "SELECT CourseID,CourseName FROM CourseMaster ORDER BY CourseName";

            DataTable dt =
                objData.GetDataTable(
                sql);

            ddlSearchCourse.DataSource =
                dt;

            ddlSearchCourse.DataTextField =
                "CourseName";

            ddlSearchCourse.DataValueField =
                "CourseID";

            ddlSearchCourse.DataBind();

            ddlSearchCourse.Items.Insert(
                0,
                new ListItem(
                    "All Courses",
                    ""));
        }
        private void BindTopic(
    string courseID)
        {
            ddlTopic.Items.Clear();

            if (
                string.IsNullOrWhiteSpace(
                    courseID)
               )
            {
                ddlTopic.Items.Insert(
                    0,
                    new ListItem(
                        "-- Select Topic --",
                        ""));

                return;
            }

            string sql =
                "SELECT DISTINCT TP.TopicID, TP.TopicName FROM TrainingDetails TD INNER JOIN SessionMaster SM ON TD.TrainingID = SM.TrainingID INNER JOIN TopicMaster TP ON SM.TopicID = TP.TopicID WHERE TD.CourseID = @CourseID ORDER BY TP.TopicName";
            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseID",
            courseID)
    };

            DataTable dt =
                objData.GetDataTable(
                sql,
                parameter);

            ddlTopic.DataSource =
                dt;

            ddlTopic.DataTextField =
                "TopicName";

            ddlTopic.DataValueField =
                "TopicID";

            ddlTopic.DataBind();

            ddlTopic.Items.Insert(
                0,
                new ListItem(
                    "-- Select Topic --",
                    ""));
        }

        private void BindSearchTopic(
            string courseID)
        {
            ddlSearchTopic.Items.Clear();

            if (
                string.IsNullOrWhiteSpace(
                    courseID)
               )
            {
                ddlSearchTopic.Items.Insert(
                    0,
                    new ListItem(
                        "All Topics",
                        ""));

                return;
            }

            string sql =
                 "SELECT DISTINCT TP.TopicID, TP.TopicName FROM TrainingDetails TD INNER JOIN SessionMaster SM ON TD.TrainingID = SM.TrainingID INNER JOIN TopicMaster TP ON SM.TopicID = TP.TopicID WHERE TD.CourseID = @CourseID ORDER BY TP.TopicName";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseID",
            courseID)
    };

            DataTable dt =
                objData.GetDataTable(
                sql,
                parameter);

            ddlSearchTopic.DataSource =
                dt;

            ddlSearchTopic.DataTextField =
                "TopicName";

            ddlSearchTopic.DataValueField =
                "TopicID";

            ddlSearchTopic.DataBind();

            ddlSearchTopic.Items.Insert(
                0,
                new ListItem(
                    "All Topics",
                    ""));
        }
        private void BindDifficulty()
        {
            ddlDifficulty.Items.Clear();

            ddlDifficulty.Items.Add(
                new ListItem(
                    "Easy",
                    "Easy"));

            ddlDifficulty.Items.Add(
                new ListItem(
                    "Medium",
                    "Medium"));

            ddlDifficulty.Items.Add(
                new ListItem(
                    "Hard",
                    "Hard"));

            ddlSearchDifficulty.Items.Clear();

            ddlSearchDifficulty.Items.Add(
                new ListItem(
                    "All",
                    ""));

            ddlSearchDifficulty.Items.Add(
                new ListItem(
                    "Easy",
                    "Easy"));

            ddlSearchDifficulty.Items.Add(
                new ListItem(
                    "Medium",
                    "Medium"));

            ddlSearchDifficulty.Items.Add(
                new ListItem(
                    "Hard",
                    "Hard"));
        }

        private void BindLanguage()
        {
            ddlLanguage.Items.Clear();

            ddlLanguage.Items.Add(
                new ListItem(
                    "English",
                    "English"));

            ddlLanguage.Items.Add(
                new ListItem(
                    "Hindi",
                    "Hindi"));
        }

        private void BindGrid()
        {
            string sql =
                "SELECT QB.QuestionID,QB.CourseID,CM.CourseName,QB.TopicID,TM.TopicName,QB.Question,QB.DifficultyLevel,QB.Marks,QB.QuestionType,QB.CreatedOn FROM QuestionBank QB INNER JOIN CourseMaster CM ON QB.CourseID=CM.CourseID INNER JOIN TopicMaster TM ON QB.TopicID=TM.TopicID";

            SqlParameter[] parameter =
            {
    };

            if (
                ddlSearchCourse.SelectedValue != ""
               )
            {
                sql +=
                    " AND QB.CourseID=@CourseID";

                Array.Resize(
                    ref parameter,
                    parameter.Length + 1);

                parameter[
                    parameter.Length - 1] =
                    new SqlParameter(
                        "@CourseID",
                        ddlSearchCourse.SelectedValue);
            }

            if (
                ddlSearchTopic.SelectedValue != ""
               )
            {
                sql +=
                    " AND QB.TopicID=@TopicID";

                Array.Resize(
                    ref parameter,
                    parameter.Length + 1);

                parameter[
                    parameter.Length - 1] =
                    new SqlParameter(
                        "@TopicID",
                        ddlSearchTopic.SelectedValue);
            }

            if (
                ddlSearchDifficulty.SelectedValue != ""
               )
            {
                sql +=
                    " AND QB.DifficultyLevel=@DifficultyLevel";

                Array.Resize(
                    ref parameter,
                    parameter.Length + 1);

                parameter[
                    parameter.Length - 1] =
                    new SqlParameter(
                        "@DifficultyLevel",
                        ddlSearchDifficulty.SelectedValue);
            }

            if (
                txtSearchQuestion.Text.Trim() != ""
               )
            {
                sql +=
                    " AND QB.Question LIKE @Question";

                Array.Resize(
                    ref parameter,
                    parameter.Length + 1);

                parameter[
                    parameter.Length - 1] =
                    new SqlParameter(
                        "@Question",
                        "%" +
                        txtSearchQuestion.Text.Trim() +
                        "%");
            }

            sql +=
                " ORDER BY QB.CreatedOn DESC";

            DataTable dt =
                objData.GetDataTable(
                sql,
                parameter);

            gvQuestion.DataSource =
                dt;

            gvQuestion.DataBind();
        }
        protected void ddlCourse_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            BindTopic(
                ddlCourse.SelectedValue);

            ddlTopic.Focus();
        }

        protected void ddlSearchCourse_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            BindSearchTopic(
                ddlSearchCourse.SelectedValue);

            BindGrid();
        }
        private bool ValidateForm()
        {
            if (
                ddlCourse.SelectedValue == ""
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please select Course.');",
                    true);

                ddlCourse.Focus();

                return false;
            }

            if (
                ddlTopic.SelectedValue == ""
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please select Topic.');",
                    true);

                ddlTopic.Focus();

                return false;
            }

            if (
                txtQuestion.Text.Trim() == ""
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please enter Question.');",
                    true);

                txtQuestion.Focus();

                return false;
            }

            if (
                txtOptionA.Text.Trim() == ""
                ||
                txtOptionB.Text.Trim() == ""
                ||
                txtOptionC.Text.Trim() == ""
                ||
                txtOptionD.Text.Trim() == ""
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please enter all four options.');",
                    true);

                return false;
            }

            decimal marks;

            if (
                !decimal.TryParse(
                    txtMarks.Text.Trim(),
                    out marks)
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Invalid Marks.');",
                    true);

                txtMarks.Focus();

                return false;
            }

            decimal negativeMarks;

            if (
                !decimal.TryParse(
                    txtNegativeMarks.Text.Trim(),
                    out negativeMarks)
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Invalid Negative Marks.');",
                    true);

                txtNegativeMarks.Focus();

                return false;
            }

            if (
                CheckDuplicateQuestion()
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Question already exists for selected Course and Topic.');",
                    true);

                return false;
            }

            return true;
        }

        private string GenerateQuestionID()
        {
            string sql =
                "SELECT ISNULL(MAX(ID),0)+1 FROM QuestionBank";

            object result =
                objData.ExecuteScalar(
                sql);

            int nextID =
                Convert.ToInt32(
                result);

            return
                "QUE" +
                nextID.ToString("000000");
        }

        private bool CheckDuplicateQuestion()
        {
            string sql =
                "SELECT COUNT(*) FROM QuestionBank WHERE CourseID=@CourseID AND TopicID=@TopicID AND Question=@Question";

            if (
                hfQuestionID.Value != ""
               )
            {
                sql +=
                    " AND QuestionID<>@QuestionID";
            }

            SqlParameter[] param =
{
    new SqlParameter("@CourseID", ddlCourse.SelectedValue),
    new SqlParameter("@TopicID", ddlTopic.SelectedValue),
    new SqlParameter("@Question", txtQuestion.Text.Trim())
};

            if
            (
                hfQuestionID.Value != ""
            )
            {
                Array.Resize(
                    ref param,
                    4);

                param[3] =
                    new SqlParameter(
                        "@QuestionID",
                        hfQuestionID.Value);
            }

            object result =
                objData.ExecuteScalar(
                sql,
                param);

            return
                Convert.ToInt32(
                result) > 0;
        }
        protected void btnSave_Click(
    object sender,
    EventArgs e)
        {
            if (
                !ValidateForm()
               )
            {
                return;
            }

            if (
                hfQuestionID.Value == ""
               )
            {
                SaveQuestion();
            }
            else
            {
                UpdateQuestion();
            }

            BindGrid();

            ClearForm();
        }

        private void SaveQuestion()
        {
            string questionID =
                GenerateQuestionID();

            string questionImage =
                UploadQuestionImage();

            string explanationImage =
                UploadExplanationImage();

            string sql =
                "INSERT INTO QuestionBank(" +
                "QuestionID," +
                "QuestionOwnerType," +
                "OwnerID," +
                "CourseID," +
                "TopicID," +
                "Question," +
                "OptionA," +
                "OptionB," +
                "OptionC," +
                "OptionD," +
                "CorrectOption," +
                "DifficultyLevel," +
                "Marks," +
                "Explanation," +
                "IsActive," +
                "CreatedOn," +
                "CreatedBy," +
                "QuestionType," +
                "NegativeMarks," +
                "Language," +
                "ImagePath," +
                "ExplanationImage" +
                ")" +
                " VALUES(" +
                "@QuestionID," +
                "'Admin'," +
                "'ADMIN'," +
                "@CourseID," +
                "@TopicID," +
                "@Question," +
                "@OptionA," +
                "@OptionB," +
                "@OptionC," +
                "@OptionD," +
                "@CorrectOption," +
                "@DifficultyLevel," +
                "@Marks," +
                "@Explanation," +
                "@IsActive," +
                "GETDATE()," +
                "@CreatedBy," +
                "@QuestionType," +
                "@NegativeMarks," +
                "@Language," +
                "@ImagePath," +
                "@ExplanationImage" +
                ")";

            SqlParameter[] parameter =
            {
        new SqlParameter("@QuestionID",questionID),
        new SqlParameter("@CourseID",ddlCourse.SelectedValue),
        new SqlParameter("@TopicID",ddlTopic.SelectedValue),
        new SqlParameter("@Question",txtQuestion.Text.Trim()),
        new SqlParameter("@OptionA",txtOptionA.Text.Trim()),
        new SqlParameter("@OptionB",txtOptionB.Text.Trim()),
        new SqlParameter("@OptionC",txtOptionC.Text.Trim()),
        new SqlParameter("@OptionD",txtOptionD.Text.Trim()),
        new SqlParameter("@CorrectOption",ddlCorrectOption.SelectedValue),
        new SqlParameter("@DifficultyLevel",ddlDifficulty.SelectedValue),
        new SqlParameter("@Marks",Convert.ToDecimal(txtMarks.Text)),
        new SqlParameter("@Explanation",txtExplanation.Text.Trim()),
        new SqlParameter("@IsActive",ddlStatus.SelectedValue),
        new SqlParameter("@CreatedBy",Session["UserID"]?.ToString()),
        new SqlParameter("@QuestionType",ddlQuestionType.SelectedValue),
        new SqlParameter("@NegativeMarks",Convert.ToDecimal(txtNegativeMarks.Text)),
        new SqlParameter("@Language",ddlLanguage.SelectedValue),
        new SqlParameter("@ImagePath",questionImage),
        new SqlParameter("@ExplanationImage",explanationImage)
    };

            int result =
                objData.ExecuteSql(
                sql,
                parameter);

            if (
                result > 0
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Question saved successfully.');",
                    true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Unable to save question.');",
                    true);
            }
        }
        private void UpdateQuestion()
        {
            string questionImage =
                UploadQuestionImage();

            string explanationImage =
                UploadExplanationImage();

            string sql =
                "UPDATE QuestionBank SET " +
                "CourseID=@CourseID," +
                "TopicID=@TopicID," +
                "Question=@Question," +
                "OptionA=@OptionA," +
                "OptionB=@OptionB," +
                "OptionC=@OptionC," +
                "OptionD=@OptionD," +
                "CorrectOption=@CorrectOption," +
                "DifficultyLevel=@DifficultyLevel," +
                "Marks=@Marks," +
                "Explanation=@Explanation," +
                "IsActive=@IsActive," +
                "ModifiedOn=GETDATE()," +
                "ModifiedBy=@ModifiedBy," +
                "QuestionType=@QuestionType," +
                "NegativeMarks=@NegativeMarks," +
                "Language=@Language";

            if (
                questionImage != ""
               )
            {
                sql +=
                    ",ImagePath=@ImagePath";
            }

            if (
                explanationImage != ""
               )
            {
                sql +=
                    ",ExplanationImage=@ExplanationImage";
            }

            sql +=
                " WHERE QuestionID=@QuestionID";

            List<SqlParameter> parameter =
                new List<SqlParameter>();

            parameter.Add(
                new SqlParameter(
                    "@CourseID",
                    ddlCourse.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@TopicID",
                    ddlTopic.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@Question",
                    txtQuestion.Text.Trim()));

            parameter.Add(
                new SqlParameter(
                    "@OptionA",
                    txtOptionA.Text.Trim()));

            parameter.Add(
                new SqlParameter(
                    "@OptionB",
                    txtOptionB.Text.Trim()));

            parameter.Add(
                new SqlParameter(
                    "@OptionC",
                    txtOptionC.Text.Trim()));

            parameter.Add(
                new SqlParameter(
                    "@OptionD",
                    txtOptionD.Text.Trim()));

            parameter.Add(
                new SqlParameter(
                    "@CorrectOption",
                    ddlCorrectOption.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@DifficultyLevel",
                    ddlDifficulty.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@Marks",
                    Convert.ToDecimal(
                        txtMarks.Text)));

            parameter.Add(
                new SqlParameter(
                    "@Explanation",
                    txtExplanation.Text.Trim()));

            parameter.Add(
                new SqlParameter(
                    "@IsActive",
                    ddlStatus.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@ModifiedBy",
                    Session["UserID"]?.ToString()));

            parameter.Add(
                new SqlParameter(
                    "@QuestionType",
                    ddlQuestionType.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@NegativeMarks",
                    Convert.ToDecimal(
                        txtNegativeMarks.Text)));

            parameter.Add(
                new SqlParameter(
                    "@Language",
                    ddlLanguage.SelectedValue));

            parameter.Add(
                new SqlParameter(
                    "@QuestionID",
                    hfQuestionID.Value));

            if (
                questionImage != ""
               )
            {
                parameter.Add(
                    new SqlParameter(
                        "@ImagePath",
                        questionImage));
            }

            if (
                explanationImage != ""
               )
            {
                parameter.Add(
                    new SqlParameter(
                        "@ExplanationImage",
                        explanationImage));
            }

            int result =
                objData.ExecuteSql(
                    sql,
                    parameter.ToArray());

            if (
                result > 0
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Question updated successfully.');",
                    true);
            }
        }

        private string UploadQuestionImage()
        {
            if (
                !fuQuestionImage.HasFile
               )
            {
                return "";
            }

            string extension =
                Path.GetExtension(
                    fuQuestionImage.FileName)
                .ToLower();

            if (
                extension != ".jpg"
                &&
                extension != ".jpeg"
                &&
                extension != ".png"
               )
            {
                return "";
            }

            string fileName =
                Guid.NewGuid().ToString()
                +
                extension;

            string folder =
                Server.MapPath(
                    "~/Uploads/QuestionImage/");

            if (
                !Directory.Exists(
                    folder)
               )
            {
                Directory.CreateDirectory(
                    folder);
            }

            fuQuestionImage.SaveAs(
                Path.Combine(
                    folder,
                    fileName));

            return
                "~/Uploads/QuestionImage/"
                +
                fileName;
        }
        private string UploadExplanationImage()
        {
            if (
                !fuExplanationImage.HasFile
               )
            {
                return "";
            }

            string extension =
                Path.GetExtension(
                    fuExplanationImage.FileName)
                .ToLower();

            if (
                extension != ".jpg"
                &&
                extension != ".jpeg"
                &&
                extension != ".png"
               )
            {
                return "";
            }

            string fileName =
                Guid.NewGuid().ToString()
                +
                extension;

            string folder =
                Server.MapPath(
                    "~/Uploads/ExplanationImage/");

            if (
                !Directory.Exists(
                    folder)
               )
            {
                Directory.CreateDirectory(
                    folder);
            }

            fuExplanationImage.SaveAs(
                Path.Combine(
                    folder,
                    fileName));

            return
                "~/Uploads/ExplanationImage/"
                +
                fileName;
        }

        private void ClearForm()
        {
            hfQuestionID.Value =
                "";

            ddlCourse.SelectedIndex =
                0;

            ddlTopic.Items.Clear();

            ddlTopic.Items.Insert(
                0,
                new ListItem(
                    "--Select--",
                    ""));

            ddlDifficulty.SelectedIndex =
                0;

            ddlQuestionType.SelectedIndex =
                0;

            ddlLanguage.SelectedIndex =
                0;

            ddlCorrectOption.SelectedIndex =
                0;

            ddlStatus.SelectedValue =
                "1";

            txtQuestion.Text =
                "";

            txtOptionA.Text =
                "";

            txtOptionB.Text =
                "";

            txtOptionC.Text =
                "";

            txtOptionD.Text =
                "";

            txtMarks.Text =
                "1";

            txtNegativeMarks.Text =
                "0";

            txtExplanation.Text =
                "";

            btnSave.Text =
                "Save";
        }

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            ClearForm();
        }

        protected void btnSearch_Click(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }

        protected void btnReset_Click(
            object sender,
            EventArgs e)
        {
            ddlSearchCourse.SelectedIndex =
                0;

            ddlSearchTopic.Items.Clear();

            ddlSearchTopic.Items.Insert(
                0,
                new ListItem(
                    "All",
                    ""));

            ddlSearchDifficulty.SelectedIndex =
                0;

            txtSearchQuestion.Text =
                "";

            BindGrid();
        }
        protected void gvQuestion_PageIndexChanging(
    object sender,
    GridViewPageEventArgs e)
        {
            gvQuestion.PageIndex =
                e.NewPageIndex;

            BindGrid();
        }

        protected void gvQuestion_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (
                e.CommandName == "EditRecord"
               )
            {
                LoadQuestion(
                    e.CommandArgument.ToString());
            }

            if (
                e.CommandName == "DeleteRecord"
               )
            {
                DeleteQuestion(
                    e.CommandArgument.ToString());

                BindGrid();
            }
        }
        private void DeleteQuestion(
    string questionID)
        {
            string sql =
                "DELETE FROM QuestionBank WHERE QuestionID=@QuestionID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            int result =
                objData.ExecuteSql(
                    sql,
                    parameter);

            if (
                result > 0
               )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Question deleted successfully.');",
                    true);
            }
        }
        private void LoadQuestion(
    string questionID)
        {
            string sql =
                "SELECT * FROM QuestionBank WHERE QuestionID=@QuestionID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            DataTable dt =
                objData.GetDataTable(
                    sql,
                    parameter);

            if (
                dt.Rows.Count == 0
               )
            {
                return;
            }

            DataRow dr =
                dt.Rows[0];

            hfQuestionID.Value =
                dr["QuestionID"].ToString();

            ddlCourse.SelectedValue =
                dr["CourseID"].ToString();

            BindTopic(
                ddlCourse.SelectedValue);

            ddlTopic.SelectedValue =
                dr["TopicID"].ToString();

            ddlDifficulty.SelectedValue =
                dr["DifficultyLevel"].ToString();

            ddlQuestionType.SelectedValue =
                dr["QuestionType"].ToString();

            ddlLanguage.SelectedValue =
                dr["Language"].ToString();

            ddlCorrectOption.SelectedValue =
                dr["CorrectOption"].ToString();

            ddlStatus.SelectedValue =
                dr["IsActive"].ToString();

            txtQuestion.Text =
                dr["Question"].ToString();

            txtOptionA.Text =
                dr["OptionA"].ToString();

            txtOptionB.Text =
                dr["OptionB"].ToString();

            txtOptionC.Text =
                dr["OptionC"].ToString();

            txtOptionD.Text =
                dr["OptionD"].ToString();

            txtMarks.Text =
                dr["Marks"].ToString();

            txtNegativeMarks.Text =
                dr["NegativeMarks"].ToString();

            txtExplanation.Text =
                dr["Explanation"].ToString();

            btnSave.Text =
                "Update";
        }
    }
}