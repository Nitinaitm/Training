using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class PostTrainingExam : System.Web.UI.Page
    {
        private clsDataAccess objDB =
            new clsDataAccess();

        protected void Page_Load(
    object sender,
    EventArgs e)
        {
            Response.Cache.SetCacheability(
                HttpCacheability.NoCache);

            Response.Cache.SetNoStore();

            Response.Cache.SetExpires(
                DateTime.UtcNow.AddMinutes(-1));

            if (!IsPostBack)
            {
                if
            (
                  Session["EmpID"] == null

            )
                {
                    Response.Redirect(
                        "~/Default.aspx");

                    return;
                }

                if
                (
                     Session["TrainingID"] == null
                    ||
                    Session["SessionID"] == null
                )
                {
                    Response.Redirect(
                        "MyTrainings.aspx");

                    return;
                }

                ViewState["EmpID"] =
                    Session["EmpID"]
                    .ToString().ToUpperInvariant();

                ViewState["TrainingID"] =
                    Session["TrainingID"]
                    .ToString();

                ViewState["SessionID"] =
                    Session["SessionID"]
                    .ToString();

                ViewState["QuestionIndex"] =
                    0;

                SessionSummary1.LoadSession(ViewState["TrainingID"].ToString(), ViewState["SessionID"].ToString(), ViewState["EmpID"].ToString());

                

                CheckAssignedTraining();

                CheckPublishedTest();

                LoadTest();

                if
                (
                    ResumeAttempt()
                )
                {
                    LoadAttemptQuestions();

                    BindPalette();

                    int currentQuestion =
                        Convert.ToInt32(
                            ViewState["CurrentQuestionNo"]);

                    LoadQuestion(
                        currentQuestion - 1);

                    SetRemainingTime();

                    divExam.Visible =
                        true;

                    btnStart.Visible =
                        false;

                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "timer",
                        "StartExamTimer();",
                        true);
                }
                else
                {
                    divExam.Visible =
                        false;

                    btnStart.Visible =
                        true;
                }
            }
        }
        private bool ResumeAttempt()
        {
            string sql =
                "SELECT " +
                "AttemptID," +
                "AttemptNo," +
                "CurrentQuestionNo," +
                "StartTime " +
                "FROM TestAttempt " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID " +
                "AND Submitted=0";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return false;
            }

            ViewState["AttemptID"] =
                dt.Rows[0]["AttemptID"]
                .ToString();

            ViewState["AttemptNo"] =
                Convert.ToInt32(
                    dt.Rows[0]["AttemptNo"]);

            ViewState["CurrentQuestionNo"] =
                Convert.ToInt32(
                    dt.Rows[0]["CurrentQuestionNo"]);

            return true;
        }
        private void CheckAssignedTraining()
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TrainingID",
            ViewState["TrainingID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
    };

            int count =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            if
            (
                count
                ==
                0
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('You are not assigned to this training.');window.location='Default.aspx';",
                    true);

                return;
            }
        }

        private void SetRemainingTime()
        {
            string sql =
                "SELECT " +
                "TM.Duration," +
                "TA.StartTime " +
                "FROM TestAttempt TA " +
                "INNER JOIN TestMaster TM " +
                "ON TA.TestID=TM.TestID " +
                "WHERE TA.AttemptID=@AttemptID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])

    };

            DataTable dt =
                objDB.GetDataTable(
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

            int durationMinute =
                Convert.ToInt32(
                    dt.Rows[0]["Duration"]);

            DateTime startTime =
                Convert.ToDateTime(
                    dt.Rows[0]["StartTime"]);

            int totalSecond =
                durationMinute
                *
                60;

            int usedSecond =
                Convert.ToInt32(
                    (
                        DateTime.Now
                        -
                        startTime
                    ).TotalSeconds);

            int remainingSecond =
                totalSecond
                -
                usedSecond;

            if
            (
                remainingSecond
                <
                0
            )
            {
                remainingSecond =
                    0;
            }

            hfRemainingSecond.Value =
                remainingSecond
                .ToString();

            TimeSpan ts =
                TimeSpan.FromSeconds(
                    remainingSecond);

            lblTimer.Text =
                ts.ToString(
                    @"mm\:ss");
        }

        private void CheckPublishedTest()
        {
            string sql =
                "SELECT " +
                "TestID " +
                "FROM TestMaster " +
                "WHERE SessionID=@SessionID " +
                "AND TestType='Post' " +
                "AND IsPublished=1";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@SessionID",
            ViewState["SessionID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Post Training Test is not available for this session.');window.location='MySessions.aspx';",
                    true);

                return;
            }

            ViewState["TestID"] =
                dt.Rows[0]["TestID"]
                .ToString();

            hfTestID.Value =
                ViewState["TestID"]
                .ToString();

            string sql2 =
    "SELECT COUNT(*) " +
    "FROM TestCandidateQuestion " +
    "WHERE TestID=@TestID " +
    "AND EmpID=@EmpID";

            SqlParameter[] parameter2 =
            {
    new SqlParameter(
        "@TestID",
        ViewState["TestID"]),

    new SqlParameter(
        "@EmpID",
        ViewState["EmpID"])
};

            int count =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql2,
                        parameter2));

            if
            (
                count
                ==
                0
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Question paper has not been generated for you. Please contact the administrator.');window.location='MySessions.aspx';",
                    true);

                return;
            }
        }
        private void LoadTest()
        {
            string sql =
                "SELECT * " +
                "FROM TestMaster " +
                "WHERE TestID=@TestID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Test not found.');window.location='Default.aspx';",
                    true);

                return;
            }

            lblTestTitle.Text =
                dt.Rows[0]["TestTitle"]
                .ToString();

            lblTotalQuestions.Text =
                dt.Rows[0]["TotalQuestions"]
                .ToString();

            lblTotalMarks.Text =
                dt.Rows[0]["TotalMarks"]
                .ToString();

            lblPassing.Text =
                dt.Rows[0]["PassingPercentage"]
                .ToString()
                +
                " %";

            lblTimer.Text =
                Convert.ToInt32(
                    dt.Rows[0]["Duration"])
                .ToString("00")
                +
                ":00";

            hfTestID.Value =
                dt.Rows[0]["TestID"]
                .ToString();

            hfTotalQuestion.Value =
                dt.Rows[0]["TotalQuestions"]
                .ToString();

            hfRemainingSecond.Value =
                (
                    Convert.ToInt32(
                        dt.Rows[0]["Duration"])
                    *
                    60
                )
                .ToString();
        }

        private void LoadCandidateQuestions()
        {
            string sql =
                "SELECT " +
                "TCQ.TestCandidateQuestionID," +
                "TCQ.QuestionID," +
                "TCQ.QuestionOrder," +
                "TCQ.SelectedOption," +
                "QB.Question," +
                "QB.OptionA," +
                "QB.OptionB," +
                "QB.OptionC," +
                "QB.OptionD " +
                "FROM TestCandidateQuestion TCQ " +
                "INNER JOIN QuestionBank QB " +
                "ON TCQ.QuestionID=QB.QuestionID " +
                "WHERE TCQ.TestID=@TestID " +
                "AND TCQ.EmpID=@EmpID " +
                "ORDER BY TCQ.QuestionOrder";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('No Questions Available.');window.location='Default.aspx';",
                    true);

                return;
            }

            ViewState["QuestionTable"] =
                dt;
        }
        private void BindPalette()
        {
            DataTable dt =
                (DataTable)
                ViewState["QuestionTable"];

            rptPalette.DataSource =
                dt;

            rptPalette.DataBind();
        }

        protected string GetPaletteClass(
    int index)
        {
            DataTable dt =
                (DataTable)
                ViewState["QuestionTable"];

            if
            (
                dt
                ==
                null
            )
            {
                return
                    "btn btn-outline-secondary question-palette";
            }

            if
            (
                index
                ==
                Convert.ToInt32(
                    ViewState["QuestionIndex"])
            )
            {
                return
                    "btn btn-warning question-palette";
            }

            string answer =
                dt.Rows[index]["SelectedOption"]
                .ToString();

            if
            (
                !string.IsNullOrWhiteSpace(
                    answer)
            )
            {
                return
                    "btn btn-success question-palette";
            }

            return
                "btn btn-outline-secondary question-palette";
        }
        //private void BindPalette()
        //{
        //    DataTable dtQuestion =
        //        (DataTable)
        //        ViewState["QuestionTable"];

        //    DataTable dtPalette =
        //        new DataTable();

        //    dtPalette.Columns.Add(
        //        "QuestionNo");

        //    foreach
        //    (
        //        DataRow row
        //        in
        //        dtQuestion.Rows
        //    )
        //    {
        //        dtPalette.Rows.Add(
        //            row["QuestionOrder"]
        //            .ToString());
        //    }

        //    rptPalette.DataSource =
        //        dtPalette;

        //    rptPalette.DataBind();
        //}

        private void LoadQuestion(
      int index)
        {


            DataTable dt =
                (DataTable)
                ViewState["QuestionTable"];

            if
            (
                dt
                ==
                null
            )
            {
                return;
            }

            if
            (
                index
                <
                0
            )
            {
                index = 0;
            }

            if
            (
                index
                >=
                dt.Rows.Count
            )
            {
                index =
                    dt.Rows.Count
                    -
                    1;
            }

            ViewState["QuestionIndex"] =
                index;

            hfCurrentQuestion.Value =
                (
                    index
                    +
                    1
                )
                .ToString();

            ViewState["CurrentQuestionNo"] =
    index + 1;

            lblQuestionNo.Text =
                (
                    index
                    +
                    1
                )
                +
                " / "
                +
                dt.Rows.Count;

            lblQuestion.Text =
                dt.Rows[index]["Question"]
                .ToString();
            if
(
    dt.Columns.Contains(
        "ImagePath")
)
            {
                string image =
                    dt.Rows[index]["ImagePath"]
                    .ToString();

                if
 (
     !string.IsNullOrWhiteSpace(
         image)
 )
                {
                    imgQuestion.ImageUrl =
                        "~/QuestionImage/"
                        +
                        image;

                    imgQuestion.Visible =
                        true;
                }
                else
                {
                    imgQuestion.Visible =
                        false;
                }
            }
            rblOption.Items.Clear();

            rblOption.Items.Add(
                new ListItem(
                    dt.Rows[index]["OptionA"]
                    .ToString(),
                    "A"));

            rblOption.Items.Add(
                new ListItem(
                    dt.Rows[index]["OptionB"]
                    .ToString(),
                    "B"));

            rblOption.Items.Add(
                new ListItem(
                    dt.Rows[index]["OptionC"]
                    .ToString(),
                    "C"));

            rblOption.Items.Add(
                new ListItem(
                    dt.Rows[index]["OptionD"]
                    .ToString(),
                    "D"));

            rblOption.ClearSelection();

            string selectedOption =
                dt.Rows[index]["SelectedOption"]
                .ToString();

            if
            (
                selectedOption
                !=
                ""
            )
            {
                ListItem item =
                    rblOption.Items.FindByValue(
                        selectedOption);

                if
                (
                    item
                    !=
                    null
                )
                {
                    item.Selected =
                        true;
                }
            }

            btnPrevious.Visible =
     (
         index
         >
         0
     );

            btnNext.Visible =
                (
                    index
                    <
                    dt.Rows.Count
                    -
                    1
                );

            //btnSubmit.Visible =
            //    (
            //        index
            //        ==
            //        dt.Rows.Count
            //        -
            //        1
            //    );
            btnFinish.Visible =
(
    index
    ==
    dt.Rows.Count
    -
    1
);
            string sql =
    "UPDATE TestAttempt " +
    "SET CurrentQuestionNo=@CurrentQuestionNo " +
    "WHERE AttemptID=@AttemptID";

            SqlParameter[] parameter =
            {
    new SqlParameter(
        "@CurrentQuestionNo",
        index + 1),

    new SqlParameter(
        "@AttemptID",
        ViewState["AttemptID"])
};

            objDB.ExecuteSql(
                sql,
                parameter);
            BindPalette();
        }
        private void SaveCurrentAnswer()
        {
            if
            (
                ViewState["QuestionTable"]
                ==
                null
            )
            {
                return;
            }

            DataTable dt =
                (DataTable)
                ViewState["QuestionTable"];

            int index =
                Convert.ToInt32(
                    ViewState["QuestionIndex"]);

            string selectedOption =
                "";

            if
            (
                rblOption.SelectedItem
                !=
                null
            )
            {
                selectedOption =
                    rblOption.SelectedValue;
            }

            dt.Rows[index]["SelectedOption"] =
                selectedOption;

            string sql =
                "UPDATE TestAttemptAnswer " +
                "SET SelectedOption=@SelectedOption " +
                "WHERE AttemptAnswerID=@AttemptAnswerID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@SelectedOption",
            selectedOption),

        new SqlParameter(
            "@AttemptAnswerID",
            dt.Rows[index]["AttemptAnswerID"])
    };

            objDB.ExecuteSql(
                sql,
                parameter);

            ViewState["QuestionTable"] =
                dt;
        }

        private bool IsExamTimeOver()
        {
            string sql =
                "SELECT " +
                "TM.Duration," +
                "TA.StartTime " +
                "FROM TestAttempt TA " +
                "INNER JOIN TestMaster TM " +
                "ON TM.TestID=TA.TestID " +
                "WHERE TA.AttemptID=@AttemptID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return true;
            }

            int duration =
                Convert.ToInt32(
                    dt.Rows[0]["Duration"]);

            DateTime startTime =
                Convert.ToDateTime(
                    dt.Rows[0]["StartTime"]);

            return
                DateTime.Now >
                startTime.AddMinutes(duration);
        }

        protected void btnNext_Click(
     object sender,
     EventArgs e)
        {
            if
            (
                IsExamTimeOver()
            )
            {
                btnSubmit_Click(
                    null,
                    null);

                return;
            }

            SaveCurrentAnswer();

            int index =
                Convert.ToInt32(
                    ViewState["QuestionIndex"]);

            LoadQuestion(
                index + 1);
        }

        protected void btnPrevious_Click(
     object sender,
     EventArgs e)
        {
            if
            (
                IsExamTimeOver()
            )
            {
                btnSubmit_Click(
                    null,
                    null);

                return;
            }

            SaveCurrentAnswer();

            int index =
                Convert.ToInt32(
                    ViewState["QuestionIndex"]);

            LoadQuestion(
                index - 1);
        }

        protected void btnQuestion_Command(
    object sender,
    CommandEventArgs e)
        {
            if
            (
                IsExamTimeOver()
            )
            {
                btnSubmit_Click(
                    null,
                    null);

                return;
            }

            SaveCurrentAnswer();

            LoadQuestion(
                Convert.ToInt32(
                    e.CommandArgument) - 1);
        }

        protected void btnStart_Click(
    object sender,
    EventArgs e)
        {
            if
            (
                ResumeAttempt()
            )
            {
                LoadAttemptQuestions();

                BindPalette();

                LoadQuestion(
                    Convert.ToInt32(
                        ViewState["CurrentQuestionNo"])
                    - 1);

                SetRemainingTime();

                divExam.Visible =
                    true;

                btnStart.Visible =
                    false;

                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "timer",
                    "StartExamTimer();",
                    true);

                return;
            }

            if
            (
                !CanStartNewAttempt()
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Maximum attempt limit reached.');",
                    true);

                return;
            }

            CreateAttempt();

            CopyQuestionsToAttempt();

            LoadAttemptQuestions();

            BindPalette();

            LoadQuestion(0);

            SetRemainingTime();

            divExam.Visible =
                true;

            btnStart.Visible =
                false;

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "timer",
                "StartExamTimer();",
                true);
        }

        private void CreateAttempt()
        {
            string sql =
                "SELECT " +
                "AttemptID," +
                "AttemptNo " +
                "FROM TestAttempt " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID " +
                "AND Submitted=0";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count
                >
                0
            )
            {
                ViewState["AttemptID"] =
                    dt.Rows[0]["AttemptID"]
                    .ToString();

                ViewState["AttemptNo"] =
                    Convert.ToInt32(
                        dt.Rows[0]["AttemptNo"]);

                return;
            }

            int nextAttempt =
                GetNextAttemptNo();

            string attemptID =
                GenerateAttemptID();

            sql =
                "INSERT INTO TestAttempt " +
                "(" +
                "AttemptID," +
                "TestID," +
                "EmpID," +
                "AttemptNo," +
                "StartTime," +
                "Submitted," +
                "CurrentQuestionNo," +
                "CreatedOn" +
                ") " +
                "VALUES " +
                "(" +
                "@AttemptID," +
                "@TestID," +
                "@EmpID," +
                "@AttemptNo," +
                "GETDATE()," +
                "0," +
                "1," +
                "GETDATE()" +
                ")";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@AttemptID",
            attemptID),

        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"]),

        new SqlParameter(
            "@AttemptNo",
            nextAttempt)
            };

            objDB.ExecuteSql(
                sql,
                parameter);

            ViewState["AttemptID"] =
                attemptID;

            ViewState["AttemptNo"] =
                nextAttempt;
        }

        private string GenerateAttemptAnswerID()
        {
            return
                "ATA"
                +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff")
                +
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 4)
                    .ToUpper();
        }

        private void CopyQuestionsToAttempt()
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TestAttemptAnswer " +
                "WHERE AttemptID=@AttemptID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
    };

            int count =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            if
            (
                count > 0
            )
            {
                return;
            }

            sql =
                "SELECT " +
                "TCQ.QuestionID," +
                "TCQ.QuestionOrder," +
                "TCQ.CorrectOption," +
                "TCQ.Marks " +
                "FROM TestCandidateQuestion TCQ " +
                "WHERE TCQ.TestID=@TestID " +
                "AND TCQ.EmpID=@EmpID " +
                "ORDER BY TCQ.QuestionOrder";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count == 0
            )
            {
                return;
            }

            //--------------------------------------------------
            // Shuffle Questions
            //--------------------------------------------------

            DataView dv =
                dt.DefaultView;

            DataTable shuffleTable =
                dv.ToTable();

            Random rnd =
                new Random();

            for
            (
                int i = shuffleTable.Rows.Count - 1;
                i > 0;
                i--
            )
            {
                int j =
                    rnd.Next(
                        i + 1);

                object[] temp =
                    shuffleTable.Rows[i].ItemArray;

                shuffleTable.Rows[i].ItemArray =
                    shuffleTable.Rows[j].ItemArray;

                shuffleTable.Rows[j].ItemArray =
                    temp;
            }

            //--------------------------------------------------
            // Insert Snapshot
            //--------------------------------------------------

            int displayOrder =
                1;

            foreach
            (
                DataRow row
                in
                shuffleTable.Rows
            )
            {
                string attemptAnswerID =
                    GenerateAttemptAnswerID();

                sql =
                    "INSERT INTO TestAttemptAnswer " +
                    "(" +
                    "AttemptAnswerID," +
                    "AttemptID," +
                    "TestID," +
                    "EmpID," +
                    "QuestionID," +
                    "QuestionOrder," +
                    "DisplayOrder," +
                    "SelectedOption," +
                    "CorrectOption," +
                    "IsCorrect," +
                    "Marks," +
                    "ObtainedMarks," +
                    "CreatedOn" +
                    ") " +
                    "VALUES " +
                    "(" +
                    "@AttemptAnswerID," +
                    "@AttemptID," +
                    "@TestID," +
                    "@EmpID," +
                    "@QuestionID," +
                    "@QuestionOrder," +
                    "@DisplayOrder," +
                    "NULL," +
                    "@CorrectOption," +
                    "NULL," +
                    "@Marks," +
                    "0," +
                    "GETDATE()" +
                    ")";

                parameter =
                new SqlParameter[]
                {
            new SqlParameter(
                "@AttemptAnswerID",
                attemptAnswerID),

            new SqlParameter(
                "@AttemptID",
                ViewState["AttemptID"]),

            new SqlParameter(
                "@TestID",
                ViewState["TestID"]),

            new SqlParameter(
                "@EmpID",
                ViewState["EmpID"]),

            new SqlParameter(
                "@QuestionID",
                row["QuestionID"]),

            new SqlParameter(
                "@QuestionOrder",
                row["QuestionOrder"]),

            new SqlParameter(
                "@DisplayOrder",
                displayOrder),

            new SqlParameter(
                "@CorrectOption",
                row["CorrectOption"]),

            new SqlParameter(
                "@Marks",
                row["Marks"])
                };

                objDB.ExecuteSql(
                    sql,
                    parameter);

                displayOrder++;
            }
        }

        private int GetNextAttemptNo()
        {
            string sql =
                "SELECT " +
                "ISNULL(MAX(AttemptNo),0)+1 " +
                "FROM TestAttempt " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
    };

            object obj =
                objDB.ExecuteScalar(
                    sql,
                    parameter);

            return
                Convert.ToInt32(
                    obj);
        }

        private string GenerateAttemptID()
        {
            return
                "ATT"
                +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff");
        }

        private bool CanStartNewAttempt()
        {
            string sql =
                "SELECT " +
                "MaxAttempt," +
                "AllowRetest " +
                "FROM TestMaster " +
                "WHERE TestID=@TestID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count
                ==
                0
            )
            {
                return false;
            }

            bool allowRetest =
                Convert.ToBoolean(
                    dt.Rows[0]["AllowRetest"]);

            if
            (
                !allowRetest
            )
            {
                return
                    GetNextAttemptNo()
                    ==
                    1;
            }

            int maxAttempt =
                Convert.ToInt32(
                    dt.Rows[0]["MaxAttempt"]);

            return
                GetNextAttemptNo()
                <=
                maxAttempt;
        }

        private void LoadAttemptQuestions()
        {
            if
            (
                ViewState["AttemptID"] == null
            )
            {
                return;
            }

            string sql =
                "SELECT " +
                "TAA.AttemptAnswerID," +
                "TAA.QuestionID," +
                "TAA.QuestionOrder," +
                "TAA.DisplayOrder," +
                "TAA.SelectedOption," +
                "TAA.CorrectOption," +
                "TAA.Marks," +
                "TAA.ObtainedMarks," +
                "QB.Question," +
                "QB.ImagePath," +
                "QB.OptionA," +
                "QB.OptionB," +
                "QB.OptionC," +
                "QB.OptionD " +
                "FROM TestAttemptAnswer TAA " +
                "INNER JOIN QuestionBank QB " +
                "ON QB.QuestionID=TAA.QuestionID " +
                "WHERE TAA.AttemptID=@AttemptID " +
                "ORDER BY TAA.DisplayOrder";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            if
            (
                dt.Rows.Count == 0
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Question paper not found.');window.location='MySessions.aspx';",
                    true);

                return;
            }

            ViewState["QuestionTable"] =
                dt;

            hfTotalQuestion.Value =
                dt.Rows.Count
                .ToString();
        }


        private void EvaluateResult()
        {
            string sql =
                "SELECT NegativeMarking," +
                "PassingPercentage," +
                "TotalMarks " +
                "FROM TestMaster " +
                "WHERE TestID=@TestID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"])
    };

            DataTable dtTest =
                objDB.GetDataTable(
                    sql,
                    parameter);

            decimal negativeMarking = 0;

            if
            (
                dtTest.Rows[0]["NegativeMarking"]
                !=
                DBNull.Value
            )
            {
                negativeMarking =
                    Convert.ToDecimal(
                        dtTest.Rows[0]["NegativeMarking"]);
            }

            decimal totalMarks =
                Convert.ToDecimal(
                    dtTest.Rows[0]["TotalMarks"]);

            decimal passingPercentage =
                Convert.ToDecimal(
                    dtTest.Rows[0]["PassingPercentage"]);

            sql =
                "SELECT * " +
                "FROM TestAttemptAnswer " +
                "WHERE AttemptID=@AttemptID " +
                "ORDER BY DisplayOrder";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
            };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);

            int totalQuestion =
                dt.Rows.Count;

            int attemptedQuestion =
                0;

            int correctAnswer =
                0;

            int wrongAnswer =
                0;

            decimal obtainedMarks =
                0;

            foreach
            (
                DataRow row
                in
                dt.Rows
            )
            {
                string selectedOption =
                    row["SelectedOption"]
                    .ToString();

                string correctOption =
                    row["CorrectOption"]
                    .ToString();

                decimal questionMarks =
                    Convert.ToDecimal(
                        row["Marks"]);

                decimal awardedMarks =
                    0;

                bool isCorrect =
                    false;

                if
                (
                    !string.IsNullOrEmpty(
                        selectedOption)
                )
                {
                    attemptedQuestion++;

                    if
                    (
                        selectedOption
                        ==
                        correctOption
                    )
                    {
                        correctAnswer++;

                        isCorrect =
                            true;

                        awardedMarks =
                            questionMarks;

                        obtainedMarks +=
                            questionMarks;
                    }
                    else
                    {
                        wrongAnswer++;

                        awardedMarks =
                            0
                            -
                            negativeMarking;

                        obtainedMarks -=
                            negativeMarking;
                    }
                }

                sql =
                    "UPDATE TestAttemptAnswer " +
                    "SET IsCorrect=@IsCorrect," +
                    "ObtainedMarks=@ObtainedMarks " +
                    "WHERE AttemptAnswerID=@AttemptAnswerID";

                parameter =
                new SqlParameter[]
                {
            new SqlParameter(
                "@IsCorrect",
                isCorrect),

            new SqlParameter(
                "@ObtainedMarks",
                awardedMarks),

            new SqlParameter(
                "@AttemptAnswerID",
                row["AttemptAnswerID"])
                };

                objDB.ExecuteSql(
                    sql,
                    parameter);
            }

            if
            (
                obtainedMarks
                <
                0
            )
            {
                obtainedMarks =
                    0;
            }

            decimal percentage =
                0;

            if
            (
                totalMarks
                >
                0
            )
            {
                percentage =
 Math.Round(
 (
 obtainedMarks
 *
 100
 )
 /
 totalMarks,
 2);
            }

            bool isPass =
                (
                    percentage
                    >=
                    passingPercentage
                );

            ViewState["TotalQuestion"] =
                totalQuestion;

            ViewState["AttemptedQuestion"] =
                attemptedQuestion;

            ViewState["CorrectAnswer"] =
                correctAnswer;

            ViewState["WrongAnswer"] =
                wrongAnswer;

            ViewState["ObtainedMarks"] =
                obtainedMarks;

            ViewState["TotalMarks"] =
                totalMarks;

            ViewState["Percentage"] =
                percentage;

            ViewState["Result"] =
                isPass
                ?
                "PASS"
                :
                "FAIL";
        }
        private void UpdateTestAttempt()
        {
            string sql =
                "SELECT StartTime " +
                "FROM TestAttempt " +
                "WHERE AttemptID=@AttemptID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
    };

            DataTable dt =
                objDB.GetDataTable(
                    sql,
                    parameter);
            if (dt.Rows.Count == 0)
            {
                return;
            }
            DateTime startTime =
                Convert.ToDateTime(
                    dt.Rows[0]["StartTime"]);

            DateTime endTime =
                DateTime.Now;

            int timeTaken =
                Convert.ToInt32(
                    (
                        endTime
                        -
                        startTime
                    ).TotalSeconds);

            sql =
                "UPDATE TestAttempt " +
                "SET EndTime=@EndTime," +
                "TotalQuestions=@TotalQuestions," +
                "CorrectAnswers=@CorrectAnswers," +
                "WrongAnswers=@WrongAnswers," +
                "ObtainedMarks=@ObtainedMarks," +
                "TotalMarks=@TotalMarks," +
                "Percentage=@Percentage," +
                "Result=@Result," +
                "Submitted=1 " +
                "WHERE AttemptID=@AttemptID";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@EndTime",
            endTime),

        new SqlParameter(
            "@TotalQuestions",
            ViewState["TotalQuestion"]),

        new SqlParameter(
            "@CorrectAnswers",
            ViewState["CorrectAnswer"]),

        new SqlParameter(
            "@WrongAnswers",
            ViewState["WrongAnswer"]),

        new SqlParameter(
            "@ObtainedMarks",
            ViewState["ObtainedMarks"]),

        new SqlParameter(
            "@TotalMarks",
            ViewState["TotalMarks"]),

        new SqlParameter(
            "@Percentage",
            ViewState["Percentage"]),

        new SqlParameter(
            "@Result",
            ViewState["Result"]),

        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
            };

            objDB.ExecuteSql(
                sql,
                parameter);

            ViewState["TimeTaken"] =
                timeTaken;

            ViewState["SubmittedOn"] =
                endTime;
        }
        private void SaveTestResult()
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TestResult " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID " +
                "AND AttemptNo=@AttemptNo";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"]),

        new SqlParameter(
            "@AttemptNo",
            ViewState["AttemptNo"])
    };

            int count =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            if
            (
                count
                >
                0
            )
            {
                return;
            }

            sql =
                "UPDATE TestResult " +
                "SET IsFinalAttempt=0 " +
                "WHERE TestID=@TestID " +
                "AND EmpID=@EmpID";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
            };

            objDB.ExecuteSql(
                sql,
                parameter);

            string resultID =
                GenerateResultID();

            sql =
                "INSERT INTO TestResult " +
                "(" +
                "ResultID," +
                "TestID," +
                "EmpID," +
                "TotalQuestions," +
                "AttemptedQuestions," +
                "CorrectAnswers," +
                "WrongAnswers," +
                "TotalMarks," +
                "ObtainedMarks," +
                "Percentage," +
                "ResultStatus," +
                "CreatedOn," +
                "CreatedBy," +
                "AttemptNo," +
                "TimeTaken," +
                "SubmittedOn," +
                "IsFinalAttempt," +
                "RankNo" +
                ") " +
                "VALUES " +
                "(" +
                "@ResultID," +
                "@TestID," +
                "@EmpID," +
                "@TotalQuestions," +
                "@AttemptedQuestions," +
                "@CorrectAnswers," +
                "@WrongAnswers," +
                "@TotalMarks," +
                "@ObtainedMarks," +
                "@Percentage," +
                "@ResultStatus," +
                "GETDATE()," +
                "@CreatedBy," +
                "@AttemptNo," +
                "@TimeTaken," +
                "@SubmittedOn," +
                "1," +
                "0" +
                ")";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@ResultID",
            resultID),

        new SqlParameter(
            "@TestID",
            ViewState["TestID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"]),

        new SqlParameter(
            "@TotalQuestions",
            ViewState["TotalQuestion"]),

        new SqlParameter(
            "@AttemptedQuestions",
            ViewState["AttemptedQuestion"]),

        new SqlParameter(
            "@CorrectAnswers",
            ViewState["CorrectAnswer"]),

        new SqlParameter(
            "@WrongAnswers",
            ViewState["WrongAnswer"]),

        new SqlParameter(
            "@TotalMarks",
            ViewState["TotalMarks"]),

        new SqlParameter(
            "@ObtainedMarks",
            ViewState["ObtainedMarks"]),

        new SqlParameter(
            "@Percentage",
            ViewState["Percentage"]),

        new SqlParameter(
            "@ResultStatus",
            ViewState["Result"]),

        new SqlParameter(
            "@CreatedBy",
            ViewState["EmpID"]),

        new SqlParameter(
            "@AttemptNo",
            ViewState["AttemptNo"]),

        new SqlParameter(
            "@TimeTaken",
            ViewState["TimeTaken"]),

        new SqlParameter(
            "@SubmittedOn",
            ViewState["SubmittedOn"])
            };

            objDB.ExecuteSql(
                sql,
                parameter);
        }

        private string GenerateResultID()
        {
            return
                "RES"
                +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff");
        }

        protected void btnSubmit_Click(
    object sender,
    EventArgs e)
        {
            if
            (
                ViewState["AttemptID"] == null
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('Attempt not found.');window.location='MySessions.aspx';",
                    true);

                return;
            }

            string sql =
                "SELECT Submitted " +
                "FROM TestAttempt " +
                "WHERE AttemptID=@AttemptID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@AttemptID",
            ViewState["AttemptID"])
    };

            object result =
                objDB.ExecuteScalar(
                    sql,
                    parameter);

            if
            (
                result != null
                &&
                Convert.ToBoolean(result)
            )
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "msg",
                    "alert('This examination has already been submitted.');window.location='MySessions.aspx';",
                    true);

                return;
            }

            SetRemainingTime();

            SaveCurrentAnswer();

            EvaluateResult();

            UpdateTestAttempt();

            SaveTestResult();

            UpdateRank();

            UpdatePostTrainingWorkflow();

            ViewState["AttemptID"] =
                null;

            ViewState["QuestionTable"] =
                null;

            ViewState["QuestionIndex"] =
                null;

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "msg",
                "alert('Post Training Examination submitted successfully.');window.location='MySessions.aspx';",
                true);
        }

        private void UpdateRank()
        {
            string sql =
                "WITH R AS " +
                "(" +
                "SELECT " +
                "ResultID," +
                "DENSE_RANK() OVER " +
                "(" +
                "ORDER BY " +
                "ObtainedMarks DESC," +
                "TimeTaken ASC" +
                ") RankNo " +
                "FROM TestResult " +
                "WHERE TestID=@TestID " +
                "AND IsFinalAttempt=1" +
                ") " +
                "UPDATE TR " +
                "SET RankNo=R.RankNo " +
                "FROM TestResult TR " +
                "INNER JOIN R " +
                "ON R.ResultID=TR.ResultID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TestID",
            ViewState["TestID"])
    };

            objDB.ExecuteSql(
                sql,
                parameter);
        }

        private void UpdatePostTrainingWorkflow()
        {
            if
            (
                !IsAllSessionPostSubmitted()
            )
            {
                return;
            }

            string sql =
                "UPDATE TrainingProgress " +
                "SET PostExamCompleted=1," +
                "WorkflowStatus='H'," +
                "UpdatedOn=GETDATE()," +
                "UpdatedBy=@UpdatedBy " +
                "WHERE TrainingID=@TrainingID " +
                "AND EmpID=@EmpID";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@UpdatedBy",
            ViewState["EmpID"]),

        new SqlParameter(
            "@TrainingID",
            ViewState["TrainingID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
    };

            objDB.ExecuteSql(
                sql,
                parameter);

            sql =
                "SELECT COUNT(*) " +
                "FROM TrainingAssignment " +
                "WHERE TrainingID=@TrainingID " +
                "AND AssignmentStatus='Assigned'";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@TrainingID",
            ViewState["TrainingID"])
            };

            int totalAssigned =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            sql =
                "SELECT COUNT(*) " +
                "FROM TrainingProgress " +
                "WHERE TrainingID=@TrainingID " +
                "AND PostExamCompleted=1";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@TrainingID",
            ViewState["TrainingID"])
            };

            int completed =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            if
            (
                totalAssigned
                ==
                completed
            )
            {
                sql =
                    "UPDATE TrainingDetails " +
                    "SET TrainingStatus='Post Exam Completed'," +
                    "WorkflowStatus='H'," +
                    "UpdatedOn=GETDATE()," +
                    "UpdatedBy=@UpdatedBy " +
                    "WHERE TrainingID=@TrainingID";

                parameter =
                new SqlParameter[]
                {
            new SqlParameter(
                "@UpdatedBy",
                ViewState["EmpID"]),

            new SqlParameter(
                "@TrainingID",
                ViewState["TrainingID"])
                };

                objDB.ExecuteSql(
                    sql,
                    parameter);
            }
        }
        private bool IsAllSessionPostSubmitted()
        {
            string sql =
                "SELECT COUNT(*) " +
                "FROM TestMaster " +
                "WHERE TrainingID=@TrainingID " +
                "AND TestType='Post' " +
                "AND IsPublished=1";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@TrainingID",
            ViewState["TrainingID"])
    };

            int totalSession =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            sql =
                "SELECT COUNT(DISTINCT TR.TestID) " +
                "FROM TestResult TR " +
                "INNER JOIN TestMaster TM " +
                "ON TR.TestID=TM.TestID " +
                "WHERE TM.TrainingID=@TrainingID " +
                "AND TM.TestType='Post' " +
                "AND TR.EmpID=@EmpID";

            parameter =
            new SqlParameter[]
            {
        new SqlParameter(
            "@TrainingID",
            ViewState["TrainingID"]),

        new SqlParameter(
            "@EmpID",
            ViewState["EmpID"])
            };

            int completedSession =
                Convert.ToInt32(
                    objDB.ExecuteScalar(
                        sql,
                        parameter));

            return
                totalSession
                ==
                completedSession;
        }
    }
}