using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class QuestionApproval : System.Web.UI.Page
    {
        clsDataAccess objData = new clsDataAccess();
        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            //if
            //(
            //    Session["UserID"] == null
            //)
            //{
            //    Response.Redirect(
            //        "~/Default.aspx");

            //    return;
            //}

            if
            (
                !IsPostBack
            )
            {
                BindCourse();

                BindTrainer();

                BindGrid();

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
                "$('#ddlTrainer').select2({width:'100%'});",

                true);
        }
        private void BindCourse()
        {
            string sql =
                "SELECT CourseID,CourseName " +
                "FROM CourseMaster " +
                "ORDER BY CourseName";

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
                    "All Courses",
                    ""));
        }
        private void BindTopic(
    string courseID)
        {
            ddlTopic.Items.Clear();

            if
            (
                courseID == ""
            )
            {
                ddlTopic.Items.Insert(
                    0,
                    new ListItem(
                        "All Topics",
                        ""));

                return;
            }

            string sql =
                "SELECT DISTINCT " +
                "TM.TopicID," +
                "TM.TopicName " +
                "FROM TrainingDetails TD " +
                "INNER JOIN SessionMaster SM " +
                "ON TD.TrainingID=SM.TrainingID " +
                "INNER JOIN TopicMaster TM " +
                "ON SM.TopicID=TM.TopicID " +
                "WHERE TD.CourseID=@CourseID " +
                "ORDER BY TM.TopicName";

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
                    "All Topics",
                    ""));
        }
        private void BindTrainer()
        {
            string sql =
                "SELECT DISTINCT " +
                "OwnerID," +
                "OwnerID " +
                "FROM QuestionBank " +
                "WHERE QuestionOwnerType='Trainer' " +
                "ORDER BY OwnerID";

            DataTable dt =
                objData.GetDataTable(
                sql);

            ddlTrainer.DataSource =
                dt;

            ddlTrainer.DataTextField =
                "OwnerID";

            ddlTrainer.DataValueField =
                "OwnerID";

            ddlTrainer.DataBind();

            ddlTrainer.Items.Insert(
                0,
                new ListItem(
                    "All Trainers",
                    ""));
        }

        private void BindGrid()
        {
            string sql =
                "SELECT " +
                "QB.QuestionID," +
                "CM.CourseName," +
                "TM.TopicName," +
                "QB.OwnerID AS TrainerName," +
                "QB.Question," +
                "QB.DifficultyLevel," +
                "QB.ApprovalStatus," +
                "QB.CreatedOn " +
                "FROM QuestionBank QB " +
                "INNER JOIN CourseMaster CM " +
                "ON QB.CourseID=CM.CourseID " +
                "INNER JOIN TopicMaster TM " +
                "ON QB.TopicID=TM.TopicID " +
                "WHERE QB.QuestionOwnerType='Trainer'";

            List<SqlParameter> parameter =
                new List<SqlParameter>();

            if
            (
                ddlCourse.SelectedValue != ""
            )
            {
                sql +=
                    " AND QB.CourseID=@CourseID";

                parameter.Add(
                    new SqlParameter(
                        "@CourseID",
                        ddlCourse.SelectedValue));
            }

            if
            (
                ddlTopic.SelectedValue != ""
            )
            {
                sql +=
                    " AND QB.TopicID=@TopicID";

                parameter.Add(
                    new SqlParameter(
                        "@TopicID",
                        ddlTopic.SelectedValue));
            }

            if
            (
                ddlTrainer.SelectedValue != ""
            )
            {
                sql +=
                    " AND QB.OwnerID=@OwnerID";

                parameter.Add(
                    new SqlParameter(
                        "@OwnerID",
                        ddlTrainer.SelectedValue));
            }

            if
            (
                ddlStatus.SelectedValue != ""
            )
            {
                sql +=
                    " AND QB.ApprovalStatus=@ApprovalStatus";

                parameter.Add(
                    new SqlParameter(
                        "@ApprovalStatus",
                        ddlStatus.SelectedValue));
            }

            sql +=
                " ORDER BY QB.CreatedOn DESC";

            gvQuestion.DataSource =
                objData.GetDataTable(
                sql,
                parameter.ToArray());

            gvQuestion.DataBind();
        }
        protected void ddlCourse_SelectedIndexChanged(
    object sender,
    EventArgs e)
        {
            BindTopic(
                ddlCourse.SelectedValue);
        }
        protected void btnSearch_Click(
    object sender,
    EventArgs e)
        {
            gvQuestion.PageIndex = 0;

            BindGrid();
        }
        protected void btnReset_Click(
    object sender,
    EventArgs e)
        {
            ddlCourse.SelectedIndex =
                0;

            ddlTopic.Items.Clear();

            ddlTopic.Items.Insert(
                0,
                new ListItem(
                    "All Topics",
                    ""));

            ddlTrainer.SelectedIndex =
                0;

            ddlStatus.SelectedIndex =
                0;

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
            if
            (
                e.CommandName
                ==
                "ViewQuestion"
            )
            {
                ViewQuestion(
                    e.CommandArgument.ToString());
            }

            if
            (
                e.CommandName
                ==
                "ApproveQuestion"
            )
            {
                ApproveQuestion(
                    e.CommandArgument.ToString());

                BindGrid();
            }

            if
            (
                e.CommandName
                ==
                "RejectQuestion"
            )
            {
                RejectQuestion(
                    e.CommandArgument.ToString());

                BindGrid();
            }
        }
        private void ApproveQuestion(
      string questionID)
        {
            string sql =
                "UPDATE QuestionBank " +
                "SET ApprovalStatus='Approved'," +
                "ApprovedBy=@ApprovedBy," +
                "ApprovedOn=GETDATE()," +
                "RejectionReason=NULL " +
                "WHERE QuestionID=@QuestionID " +
                "AND QuestionOwnerType='Trainer'";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@ApprovedBy",
            Session["UserID"].ToString()),

        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            objData.ExecuteSql(
                sql,
                parameter);
        }
        private void RejectQuestion(
    string questionID)
        {
            string sql =
                "UPDATE QuestionBank " +
                "SET ApprovalStatus='Rejected'," +
                "ApprovedBy=NULL," +
                "ApprovedOn=NULL," +
                "RejectionReason=@RejectionReason " +
                "WHERE QuestionID=@QuestionID " +
                "AND QuestionOwnerType='Trainer'";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@RejectionReason",
            txtRejectReason.Text.Trim()),

        new SqlParameter(
            "@QuestionID",
            questionID)
    };

            objData.ExecuteSql(
                sql,
                parameter);
        }
        private void ViewQuestion(
    string questionID)
        {
            hfQuestionID.Value =
                questionID;

            string sql =
                "SELECT * " +
                "FROM QuestionBank " +
                "WHERE QuestionID=@QuestionID";

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

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                return;
            }

            DataRow dr =
                dt.Rows[0];

            lblQuestion.Text =
                dr["Question"].ToString();

            lblA.Text =
                dr["OptionA"].ToString();

            lblB.Text =
                dr["OptionB"].ToString();

            lblC.Text =
                dr["OptionC"].ToString();

            lblD.Text =
                dr["OptionD"].ToString();

            lblAnswer.Text =
                dr["CorrectOption"].ToString();

            lblExplanation.Text =
                dr["Explanation"].ToString();
            imgQuestion.ImageUrl =
    dr["QuestionImage"]?.ToString();

            imgExplanation.ImageUrl =
                dr["ExplanationImage"]?.ToString();
            imgQuestion.Visible =
    !string.IsNullOrWhiteSpace(
        imgQuestion.ImageUrl);

            imgExplanation.Visible =
                !string.IsNullOrWhiteSpace(
                    imgExplanation.ImageUrl);

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "popup",
                "$('#questionModal').modal('show');",
                true);
        }
        protected void btnApprove_Click(
     object sender,
     EventArgs e)
        {
            ApproveQuestion(
                hfQuestionID.Value);

            BindGrid();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "closeModal",
                "$('#questionModal').modal('hide');alert('Question Approved Successfully.');",
                true);
        }

        protected void gvQuestion_RowDataBound(
     object sender,
     GridViewRowEventArgs e)
        {
            if
            (
                e.Row.RowType
                !=
                DataControlRowType.DataRow
            )
            {
                return;
            }

            string status =
                DataBinder.Eval(
                    e.Row.DataItem,
                    "ApprovalStatus")
                .ToString();

            LinkButton btnApprove =
                (LinkButton)
                e.Row.FindControl(
                    "lnkApprove");

            LinkButton btnReject =
                (LinkButton)
                e.Row.FindControl(
                    "lnkReject");

            if
            (
                status
                ==
                "Pending"
            )
            {
                e.Row.BackColor =
                    System.Drawing.Color.LightYellow;
            }

            if
            (
                status
                ==
                "Approved"
            )
            {
                e.Row.BackColor =
                    System.Drawing.Color.Honeydew;

                if
                (
                    btnApprove != null
                )
                {
                    btnApprove.Enabled =
                        false;

                    btnApprove.CssClass =
                        "btn btn-secondary btn-sm";
                }
            }

            if
            (
                status
                ==
                "Rejected"
            )
            {
                e.Row.BackColor =
                    System.Drawing.Color.MistyRose;

                if
                (
                    btnReject != null
                )
                {
                    btnReject.Enabled =
                        false;

                    btnReject.CssClass =
                        "btn btn-secondary btn-sm";
                }
            }
            Label lblStatus =
    (Label)e.Row.FindControl(
        "lblStatus");

            if (lblStatus != null)
            {
                switch (lblStatus.Text)
                {
                    case "Pending":

                        lblStatus.CssClass =
                            "badge bg-warning text-dark";
                        break;

                    case "Approved":

                        lblStatus.CssClass =
                            "badge bg-success";
                        break;

                    case "Rejected":

                        lblStatus.CssClass =
                            "badge bg-danger";
                        break;
                }
            }
        }
        protected void btnReject_Click(
     object sender,
     EventArgs e)
        {
            if
            (
                txtRejectReason.Text.Trim()
                ==
                ""
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Please enter rejection reason.');",
                    true);

                return;
            }

            RejectQuestion(
                hfQuestionID.Value);

            txtRejectReason.Text =
                "";

            BindGrid();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "closeModal",
                "$('#questionModal').modal('hide');alert('Question Rejected Successfully.');",
                true);
        }
        protected void btnApproveSelected_Click(
     object sender,
     EventArgs e)
        {
            int count =
                0;

            foreach
            (
                GridViewRow row
                in
                gvQuestion.Rows
            )
            {
                CheckBox chk =
                    (CheckBox)
                    row.FindControl(
                        "chkSelect");

                if
                (
                    chk != null
                    &&
                    chk.Checked
                )
                {
                    //string status =
                    //    row.Cells[6].Text.Trim();
                    string status =
    gvQuestion.DataKeys[row.RowIndex]
    .Values["ApprovalStatus"]
    .ToString();
                    if
                    (
                        status
                        ==
                        "Approved"
                    )
                    {
                        continue;
                    }

                    string questionID =
                        gvQuestion.DataKeys[
                            row.RowIndex]
                        .Value
                        .ToString();

                    string sql =
                        "UPDATE QuestionBank " +
                        "SET ApprovalStatus='Approved'," +
                        "ApprovedBy=@ApprovedBy," +
                        "ApprovedOn=GETDATE()," +
                        "RejectionReason=NULL " +
                        "WHERE QuestionID=@QuestionID " +
                        "AND QuestionOwnerType='Trainer'";

                    SqlParameter[] parameter =
                    {
                new SqlParameter(
                    "@ApprovedBy",
                    Session["UserID"].ToString()),

                new SqlParameter(
                    "@QuestionID",
                    questionID)
            };

                    objData.ExecuteSql(
                        sql,
                        parameter);

                    count++;
                }
            }

            BindGrid();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('" + count + " Question(s) Approved Successfully.');",
                true);
        }

        protected void btnRejectSelected_Click(
            object sender,
            EventArgs e)
        {
            int count =
                0;

            foreach
            (
                GridViewRow row
                in
                gvQuestion.Rows
            )
            {
                CheckBox chk =
                    (CheckBox)
                    row.FindControl(
                        "chkSelect");

                if
                (
                    chk != null
                    &&
                    chk.Checked
                )
                {
                    //string status =
                    //    row.Cells[6].Text.Trim();
                    string status =
    gvQuestion.DataKeys[row.RowIndex]
    .Values["ApprovalStatus"]
    .ToString();

                    if
                    (
                        status
                        ==
                        "Rejected"
                    )
                    {
                        continue;
                    }

                    string questionID =
                        gvQuestion.DataKeys[
                            row.RowIndex]
                        .Value
                        .ToString();

                    string sql =
                        "UPDATE QuestionBank " +
                        "SET ApprovalStatus='Rejected'," +
                        "ApprovedBy=NULL," +
                        "ApprovedOn=NULL," +
                        "RejectionReason='Rejected By Admin' " +
                        "WHERE QuestionID=@QuestionID " +
                        "AND QuestionOwnerType='Trainer'";

                    SqlParameter[] parameter =
                    {
                new SqlParameter(
                    "@QuestionID",
                    questionID)
            };

                    objData.ExecuteSql(
                        sql,
                        parameter);

                    count++;
                }
            }

            BindGrid();

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('" + count + " Question(s) Rejected Successfully.');",
                true);
        }
    }
}