using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class FeedbackMaster :
        System.Web.UI.Page
    {
        clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                BindCategory();

                ClearControls();

                BindGrid();
            }
        }

        //------------------------------------------------------
        // Bind Category
        //------------------------------------------------------

        private void BindCategory()
        {
            string query =
@"
SELECT
CategoryID,
CategoryName
FROM
FeedbackCategoryMaster
WHERE
Active=1
ORDER BY
DisplayOrder,
CategoryName
";

            DataTable dt =
                objDB.GetDataTable(
                query);

            ddlCategory.DataSource =
                dt;

            ddlCategory.DataTextField =
                "CategoryName";

            ddlCategory.DataValueField =
                "CategoryID";

            ddlCategory.DataBind();

            ddlCategory.Items.Insert(
                0,
                new System.Web.UI.WebControls.ListItem(
                    "-- Select Category --",
                    ""));
        }

        //------------------------------------------------------
        // Bind Grid
        //------------------------------------------------------

        private void BindGrid()
        {
            string query =
@"
SELECT
FQM.QuestionID,
FCM.CategoryName,
FQM.QuestionText,
FQM.AnswerType,
FQM.DisplayOrder,
FQM.Mandatory,
FQM.Active,
FQM.CreatedOn
FROM
FeedbackQuestionMaster FQM
INNER JOIN
FeedbackCategoryMaster FCM
ON
FQM.CategoryID=FCM.CategoryID
WHERE
FQM.QuestionText LIKE @Search
ORDER BY
FCM.DisplayOrder,
FQM.DisplayOrder,
FQM.QuestionText
";

            SqlParameter[] param =
            {
                new SqlParameter(
                    "@Search",
                    "%" +
                    txtSearch.Text.Trim() +
                    "%")
            };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            gvQuestion.DataSource =
                dt;

            gvQuestion.DataBind();
        }

        //------------------------------------------------------
        // Generate Question ID
        //------------------------------------------------------

        private string GenerateQuestionID()
        {
            string query =
@"
SELECT
ISNULL(MAX(ID),0)+1
FROM
FeedbackQuestionMaster
";

            int id =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query));

            return
                "FQ" +
                id.ToString("0000");
        }

        //------------------------------------------------------
        // Clear Controls
        //------------------------------------------------------

        private void ClearControls()
        {
            hfQuestionID.Value =
                "";

            ddlCategory.SelectedIndex =
                0;

            txtQuestion.Text =
                "";

            ddlAnswerType.SelectedValue =
    "Rating";

            txtDisplayOrder.Text =
                "1";

            chkMandatory.Checked =
                false;

            chkActive.Checked =
                true;

            btnSave.Text =
                "Save";

            lblMessage.Text =
                "";
        }

        //------------------------------------------------------
        // Search
        //------------------------------------------------------

        protected void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            BindGrid();
        }

        //------------------------------------------------------
        // Clear Button
        //------------------------------------------------------

        protected void btnClear_Click(
            object sender,
            EventArgs e)
        {
            ClearControls();

            BindGrid();
        }
        //------------------------------------------------------
        // Save
        //------------------------------------------------------

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            if (ddlCategory.SelectedIndex == 0)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Please select Feedback Category.";

                return;
            }

            if (txtQuestion.Text.Trim() == "")
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Please enter Question.";

                return;
            }

            int order = 1;

            if (!Int32.TryParse(
                txtDisplayOrder.Text.Trim(),
                out order))
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Invalid Display Order.";

                return;
            }

            if (IsDuplicateQuestion())
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Question already exists in selected category.";

                return;
            }

            if (String.IsNullOrWhiteSpace(
                hfQuestionID.Value))
            {
                InsertQuestion();
            }
            else
            {
                UpdateQuestion();
            }

            ClearControls();

            BindGrid();
        }

        //------------------------------------------------------
        // Duplicate Validation
        //------------------------------------------------------

        private bool IsDuplicateQuestion()
        {
            string query =
        @"
SELECT
COUNT(*)
FROM
FeedbackQuestionMaster
WHERE
CategoryID=@CategoryID
AND
QuestionText=@QuestionText
AND
QuestionID<>@QuestionID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryID",
            ddlCategory.SelectedValue),

        new SqlParameter(
            "@QuestionText",
            txtQuestion.Text.Trim()),

        new SqlParameter(
            "@QuestionID",
            hfQuestionID.Value)
    };

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                query,
                param));

            return
                count > 0;
        }

        //------------------------------------------------------
        // Insert
        //------------------------------------------------------

        private void InsertQuestion()
        {
            string query =
        @"
INSERT INTO
FeedbackQuestionMaster
(
QuestionID,
CategoryID,
FeedbackType,
QuestionText,
AnswerType,
DisplayOrder,
Mandatory,
Active,
CreatedOn
)
VALUES
(
@QuestionID,
@CategoryID,
@FeedbackType,
@QuestionText,
@AnswerType,
@DisplayOrder,
@Mandatory,
@Active,
GETDATE()
)
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@QuestionID",
            GenerateQuestionID()),

        new SqlParameter(
            "@CategoryID",
            ddlCategory.SelectedValue),

        new SqlParameter(
            "@FeedbackType",
            "Feedback"),

        new SqlParameter(
            "@QuestionText",
            txtQuestion.Text.Trim()),

        new SqlParameter(
    "@AnswerType",
    ddlAnswerType.SelectedValue),

        new SqlParameter(
            "@DisplayOrder",
            Convert.ToInt32(
            txtDisplayOrder.Text.Trim())),

        new SqlParameter(
            "@Mandatory",
            chkMandatory.Checked),

        new SqlParameter(
            "@Active",
            chkActive.Checked)
    };

            objDB.ExecuteSql(
                query,
                param);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Question saved successfully.";
        }

        //------------------------------------------------------
        // Update
        //------------------------------------------------------

        private void UpdateQuestion()
        {
            string query =
        @"
UPDATE
FeedbackQuestionMaster
SET
CategoryID=@CategoryID,
QuestionText=@QuestionText,
AnswerType=@AnswerType,
DisplayOrder=@DisplayOrder,
Mandatory=@Mandatory,
Active=@Active
WHERE
QuestionID=@QuestionID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@CategoryID",
            ddlCategory.SelectedValue),

        new SqlParameter(
            "@QuestionText",
            txtQuestion.Text.Trim()),

        new SqlParameter(
    "@AnswerType",
    ddlAnswerType.SelectedValue),

        new SqlParameter(
            "@DisplayOrder",
            Convert.ToInt32(
            txtDisplayOrder.Text.Trim())),

        new SqlParameter(
            "@Mandatory",
            chkMandatory.Checked),

        new SqlParameter(
            "@Active",
            chkActive.Checked),

        new SqlParameter(
            "@QuestionID",
            hfQuestionID.Value)
    };

            objDB.ExecuteSql(
                query,
                param);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Question updated successfully.";
        }
        //------------------------------------------------------
        // Load Question
        //------------------------------------------------------

        private void LoadQuestion(
            string questionID)
        {
            string query =
        @"
SELECT
QuestionID,
CategoryID,
QuestionText,
AnswerType,
DisplayOrder,
Mandatory,
Active
FROM
FeedbackQuestionMaster
WHERE
QuestionID=@QuestionID
";

            SqlParameter[] param =
            {
        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            DataTable dt =
                objDB.GetDataTable(
                query,
                param);

            if (dt.Rows.Count == 0)
            {
                return;
            }

            hfQuestionID.Value =
                dt.Rows[0]["QuestionID"]
                .ToString();

            ddlCategory.SelectedValue =
                dt.Rows[0]["CategoryID"]
                .ToString();

            txtQuestion.Text =
                dt.Rows[0]["QuestionText"]
                .ToString();
            ddlAnswerType.SelectedValue =
    dt.Rows[0]["AnswerType"]
    .ToString();

            txtDisplayOrder.Text =
                dt.Rows[0]["DisplayOrder"]
                .ToString();

            chkMandatory.Checked =
                Convert.ToBoolean(
                dt.Rows[0]["Mandatory"]);

            chkActive.Checked =
                Convert.ToBoolean(
                dt.Rows[0]["Active"]);

            btnSave.Text =
                "Update";
        }

        //------------------------------------------------------
        // Grid Row Command
        //------------------------------------------------------

        protected void gvQuestion_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditRow")
            {
                LoadQuestion(
                    e.CommandArgument
                    .ToString());

                return;
            }

            if (e.CommandName == "DeleteRow")
            {
                DeleteQuestion(
                    e.CommandArgument
                    .ToString());

                ClearControls();

                BindGrid();

                return;
            }
        }

        //------------------------------------------------------
        // Delete Question
        //------------------------------------------------------

        private void DeleteQuestion(
            string questionID)
        {
            string checkQuery =
        @"
SELECT
COUNT(*)
FROM
FeedbackDetail
WHERE
QuestionID=@QuestionID
";

            SqlParameter[] checkParam =
            {
        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            int count =
                Convert.ToInt32(
                objDB.ExecuteScalar(
                checkQuery,
                checkParam));

            if (count > 0)
            {
                lblMessage.ForeColor =
                    System.Drawing.Color.Red;

                lblMessage.Text =
                    "Question cannot be deleted because it has already been used in Feedback.";

                return;
            }

            string deleteQuery =
        @"
DELETE
FROM
FeedbackQuestionMaster
WHERE
QuestionID=@QuestionID
";

            SqlParameter[] deleteParam =
            {
        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            objDB.ExecuteSql(
                deleteQuery,
                deleteParam);

            lblMessage.ForeColor =
                System.Drawing.Color.Green;

            lblMessage.Text =
                "Question deleted successfully.";
        }

    }
}