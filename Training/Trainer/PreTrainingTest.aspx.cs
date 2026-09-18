using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class PreTrainingTest : System.Web.UI.Page
    {
        private clsDataAccess objDB = new clsDataAccess();

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (Session["TrainingID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }
            if (Session["SessionID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SessionID"] = Session["SessionID"]?.ToString();
                SessionSummary1.LoadSession(Session["SessionID"].ToString());
                LoadSessionDetails();

                if (!CheckPreTrainingRequired()) return;

                LoadQuestionPool();
                CheckExistingTest();
            }
        }

        private void LoadSessionDetails()
        {
            string sql = "SELECT SM.SessionID,SM.SessionName,SM.SessionDate,SM.TopicID,TM.TopicName,SM.TrainerID,ISNULL(EBM.EmpName,TMR.NameExternal) AS TrainerName,TD.TrainingID,TD.TrainingType,TD.BatchStrength FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TopicMaster TM ON SM.TopicID=TM.TopicID LEFT JOIN EmpBasicMaster EBM ON SM.TrainerID=EBM.EmpID LEFT JOIN TrainerMaster TMR ON SM.TrainerID=TMR.TrainerID WHERE SM.SessionID=@SessionID AND SM.TrainerID=@TrainerID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]), new SqlParameter("@TrainerID", Session["TrainerID"]) });
            if (dt.Rows.Count == 0) { Response.Redirect("Default.aspx"); return; }

            lblSession.Text = dt.Rows[0]["SessionName"].ToString();
            ViewState["TopicID"] = dt.Rows[0]["TopicID"].ToString();
            ViewState["TrainerID"] = dt.Rows[0]["TrainerID"].ToString();
            ViewState["TrainingID"] = dt.Rows[0]["TrainingID"].ToString();
        }

        private bool CheckPreTrainingRequired()
        {
            object requiredResult = objDB.ExecuteScalar(
                "SELECT InitialAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", ViewState["TrainingID"]) });

            if (requiredResult == null || requiredResult == DBNull.Value || !Convert.ToBoolean(requiredResult))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PreTrainingRequired",
                    "alert('Pre-Training Assessment is not required for this training.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
                return false;
            }

            object skipped = objDB.ExecuteScalar(
                "SELECT ISNULL(PreAssessmentSkipped,0) FROM SessionMaster WHERE SessionID=@SessionID",
                new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]) });

            if (skipped != null && skipped != DBNull.Value && Convert.ToBoolean(skipped))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PreTrainingSkipped",
                    "alert('Pre-Training Assessment has been skipped for this session.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
                return false;
            }

            return true;
        }

        private void CheckExistingTest()
        {
            DataTable dt = objDB.GetDataTable("SELECT * FROM TestMaster WHERE SessionID=@SessionID AND TestType=@TestType",
                new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]), new SqlParameter("@TestType", "Pre") });

            if (dt.Rows.Count > 0)
            {
                ViewState["TestID"] = dt.Rows[0]["TestID"].ToString();
                LoadTest();
                LoadTestQuestions();
                return;
            }

            SetDefaultValues();
        }

        private void LoadTestQuestions()
        {
            DataTable dt = objDB.GetDataTable(
                "SELECT TQ.QuestionID,QB.Question,QB.DifficultyLevel,TQ.Marks,QB.QuestionOwnerType FROM TestQuestion TQ INNER JOIN QuestionBank QB ON TQ.QuestionID=QB.QuestionID WHERE TQ.TestID=@TestID ORDER BY TQ.QuestionOrder",
                new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]) });

            ViewState["SelectedQuestions"] = dt;
            gvQuestion.DataSource = dt;
            gvQuestion.DataBind();
        }

        private void LoadTest()
        {
            DataTable dt = objDB.GetDataTable("SELECT * FROM TestMaster WHERE TestID=@TestID AND TrainerID=@TrainerID",
                new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]), new SqlParameter("@TrainerID", Session["TrainerID"]) });
            if (dt.Rows.Count == 0) return;

            txtTestTitle.Text = dt.Rows[0]["TestTitle"].ToString();
            txtDuration.Text = dt.Rows[0]["Duration"].ToString();
            txtTotalQuestions.Text = dt.Rows[0]["TotalQuestions"].ToString();
            txtPassing.Text = dt.Rows[0]["PassingPercentage"].ToString();
            chkRandom.Checked = Convert.ToBoolean(dt.Rows[0]["RandomQuestion"]);
            chkShuffle.Checked = Convert.ToBoolean(dt.Rows[0]["ShuffleOption"]);
            chkAllowRetest.Checked = Convert.ToBoolean(dt.Rows[0]["AllowRetest"]);
            txtAttempt.Text = dt.Rows[0]["MaxAttempt"].ToString();

            decimal totalMarks = Convert.ToDecimal(dt.Rows[0]["TotalMarks"]);
            int totalQuestion = Convert.ToInt32(dt.Rows[0]["TotalQuestions"]);
            if (totalQuestion > 0) txtMarks.Text = (totalMarks / totalQuestion).ToString("0.##");

            if (Convert.ToBoolean(dt.Rows[0]["IsPublished"]))
            {
                btnPublish.Enabled = false;
                btnPublish.Text = "Published";
                btnGenerateQuestions.Enabled = false;
                btnSaveDraft.Enabled = false;
            }
        }

        private void SetDefaultValues()
        {
            txtTestTitle.Text = lblSession.Text + " Pre Training Test";
            txtDuration.Text = "30";
            txtTotalQuestions.Text = "20";
            txtMarks.Text = "1";
            txtPassing.Text = "40";
            txtAttempt.Text = "1";
            txtEasy.Text = "5";
            txtMedium.Text = "10";
            txtHard.Text = "5";
            chkRandom.Checked = true;
            chkShuffle.Checked = true;
            chkAllowRetest.Checked = false;
        }

        protected void btnGenerateQuestions_Click(object sender, EventArgs e)
        {
            if (!ValidateQuestionDistribution()) return;
            if (!ValidateQuestionPool()) return;

            if (chkRandom.Checked)
            {
                GenerateRandomQuestions();
                BindSelectedQuestions();
            }
            else
            {
                LoadManualQuestions();
            }

            LoadQuestionPool();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Questions Generated Successfully.');", true);
        }

        protected void chkRandom_CheckedChanged(object sender, EventArgs e)
        {
            gvQuestion.DataSource = null;
            gvQuestion.DataBind();
        }

        private void LoadManualQuestions()
        {
            string sql = "SELECT QuestionID,Question,DifficultyLevel,Marks,QuestionOwnerType FROM QuestionBank WHERE TopicID=@TopicID AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID)) ORDER BY CASE DifficultyLevel WHEN 'Easy' THEN 1 WHEN 'Medium' THEN 2 WHEN 'Hard' THEN 3 END,QuestionID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] {
                        new SqlParameter("@TopicID", ViewState["TopicID"]),
                new SqlParameter("@TrainerID", ViewState["TrainerID"])
            });

            gvQuestion.DataSource = dt;
            gvQuestion.DataBind();

            foreach (GridViewRow row in gvQuestion.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                if (chk != null) chk.Checked = false;
            }
        }

        private bool CreateManualQuestionTable()
        {
            DataTable dt = CreateQuestionTable();
            int easy = 0, medium = 0, hard = 0;

            foreach (GridViewRow row in gvQuestion.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                if (chk == null || !chk.Checked) continue;

                DataKey key = gvQuestion.DataKeys[row.RowIndex];
                string difficulty = key.Values["DifficultyLevel"].ToString();
                dt.Rows.Add(key.Values["QuestionID"], key.Values["Question"], difficulty,
                    Convert.ToDecimal(key.Values["Marks"]), key.Values["QuestionOwnerType"]);

                if (difficulty == "Easy") easy++;
                else if (difficulty == "Medium") medium++;
                else if (difficulty == "Hard") hard++;
            }

            if (easy != Convert.ToInt32(txtEasy.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select exactly " + txtEasy.Text + " Easy Questions.');", true);
                return false;
            }
            if (medium != Convert.ToInt32(txtMedium.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select exactly " + txtMedium.Text + " Medium Questions.');", true);
                return false;
            }
            if (hard != Convert.ToInt32(txtHard.Text))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please select exactly " + txtHard.Text + " Hard Questions.');", true);
                return false;
            }

            ViewState["SelectedQuestions"] = dt;
            return true;
        }

        private bool ValidateQuestionDistribution()
        {
            int totalQuestions;
            int easy;
            int medium;
            int hard;

            if (!int.TryParse(txtTotalQuestions.Text.Trim(), out totalQuestions) ||
                !int.TryParse(txtEasy.Text.Trim(), out easy) ||
                !int.TryParse(txtMedium.Text.Trim(), out medium) ||
                !int.TryParse(txtHard.Text.Trim(), out hard) ||
                totalQuestions < 0 || easy < 0 || medium < 0 || hard < 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please enter valid non-negative numbers for question distribution.');", true);
                return false;
            }

            if (easy + medium + hard != totalQuestions)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Easy + Medium + Hard should be equal to Total Questions.');", true);
                return false;
            }
            return true;
        }

        private bool ValidateQuestionPool()
        {
            return CheckDifficultyCount("Easy", Convert.ToInt32(txtEasy.Text))
                && CheckDifficultyCount("Medium", Convert.ToInt32(txtMedium.Text))
                && CheckDifficultyCount("Hard", Convert.ToInt32(txtHard.Text));
        }

        private bool CheckDifficultyCount(string difficulty, int requiredCount)
        {
            string sql = "SELECT COUNT(*) FROM QuestionBank WHERE TopicID=@TopicID AND DifficultyLevel=@DifficultyLevel AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID))";
            object obj = objDB.ExecuteScalar(sql, new SqlParameter[] {
                new SqlParameter("@TopicID", ViewState["TopicID"]),
                new SqlParameter("@DifficultyLevel", difficulty),
                new SqlParameter("@TrainerID", ViewState["TrainerID"])
            });

            int availableCount = Convert.ToInt32(obj);
            if (availableCount < requiredCount)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Only " + availableCount + " " + difficulty + " questions are available.');", true);
                return false;
            }
            return true;
        }

        private DataTable CreateQuestionTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("QuestionID");
            dt.Columns.Add("Question");
            dt.Columns.Add("DifficultyLevel");
            dt.Columns.Add("Marks", typeof(decimal));
            dt.Columns.Add("QuestionOwnerType");
            return dt;
        }

        private void GenerateRandomQuestions()
        {
            DataTable dtQuestion = CreateQuestionTable();
            GetRandomQuestions(dtQuestion, "Easy", Convert.ToInt32(txtEasy.Text));
            GetRandomQuestions(dtQuestion, "Medium", Convert.ToInt32(txtMedium.Text));
            GetRandomQuestions(dtQuestion, "Hard", Convert.ToInt32(txtHard.Text));
            ViewState["SelectedQuestions"] = dtQuestion;
            BindSelectedQuestions();
        }

        private void GetRandomQuestions(DataTable dtQuestion, string difficulty, int count)
        {
            string sql = "SELECT TOP (@QuestionCount) QuestionID,Question,DifficultyLevel,Marks,QuestionOwnerType FROM QuestionBank WHERE TopicID=@TopicID AND DifficultyLevel=@DifficultyLevel AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID)) ORDER BY NEWID()";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] {
                new SqlParameter("@TopicID", ViewState["TopicID"]),
                new SqlParameter("@DifficultyLevel", difficulty),
                new SqlParameter("@TrainerID", ViewState["TrainerID"])
            });

            foreach (DataRow row in dt.Rows)
            {
                dtQuestion.Rows.Add(row["QuestionID"], row["Question"], row["DifficultyLevel"], row["Marks"], row["QuestionOwnerType"]);
            }
        }

        private void BindSelectedQuestions()
        {
            if (ViewState["SelectedQuestions"] == null)
            {
                gvQuestion.DataSource = null;
                gvQuestion.DataBind();
                return;
            }
            gvQuestion.DataSource = (DataTable)ViewState["SelectedQuestions"];
            gvQuestion.DataBind();
        }

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            if (!chkRandom.Checked && !CreateManualQuestionTable()) return;

            DataTable dt = ViewState["SelectedQuestions"] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please Generate Questions First.');", true);
                return;
            }

            if (ViewState["TestID"] == null)
            {
                ViewState["TestID"] = "TST" + Guid.NewGuid().ToString("N").Substring(0, 16);
                InsertTestMaster();
            }
            else
            {
                UpdateTestMaster();
            }

            SaveTestQuestions();
            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Draft Saved Successfully.');", true);
        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkAll = (CheckBox)sender;
            foreach (GridViewRow row in gvQuestion.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");
                if (chk != null) chk.Checked = chkAll.Checked;
            }
        }

        private void SaveTestQuestions()
        {
            try
            {
                objDB.ExecuteSql("DELETE FROM TestQuestion WHERE TestID=@TestID",
                    new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]) });

                DataTable dt = (DataTable)ViewState["SelectedQuestions"];
                int order = 1;

                foreach (DataRow row in dt.Rows)
                {
                    string questionID = "TQ" + Guid.NewGuid().ToString("N").Substring(0, 16);
                    string sql = "INSERT INTO TestQuestion (TestQuestionID,TestID,QuestionID,QuestionOrder,Marks,CreatedOn) VALUES (@TestQuestionID,@TestID,@QuestionID,@QuestionOrder,@Marks,GETDATE())";
                    objDB.ExecuteSql(sql, new SqlParameter[] {
                        new SqlParameter("@TestQuestionID", questionID),
                        new SqlParameter("@TestID", ViewState["TestID"]),
                        new SqlParameter("@QuestionID", row["QuestionID"]),
                        new SqlParameter("@QuestionOrder", order),
                        new SqlParameter("@Marks", row["Marks"])
                    });
                    order++;
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + ex.Message.Replace("'", "") + "');", true);
            }
        }

        private bool ValidatePublish()
        {
            if (ViewState["TestID"] == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please Save Draft First.');", true);
                return false;
            }

            DataTable dt = ViewState["SelectedQuestions"] as DataTable;
            if (dt == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('No Questions Found.');", true);
                return false;
            }

            int totalQuestions;
            if (!int.TryParse(txtTotalQuestions.Text.Trim(), out totalQuestions) || totalQuestions < 0)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Please enter a valid Total Questions value.');", true);
                return false;
            }

            if (dt.Rows.Count != totalQuestions)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Question Count Mismatch.');", true);
                return false;
            }
            return true;
        }

        private void InsertTestMaster()
        {
            try
            {
                decimal totalMarks = Convert.ToDecimal(txtMarks.Text) * Convert.ToInt32(txtTotalQuestions.Text);
                string sql = "INSERT INTO TestMaster (TestID,TrainingID,SessionID,TopicID,TrainerID,TestType,TestTitle,Duration,TotalQuestions,TotalMarks,PassingPercentage,RandomQuestion,ShuffleOption,AllowRetest,MaxAttempt,IsPublished,CreatedOn) VALUES (@TestID,@TrainingID,@SessionID,@TopicID,@TrainerID,'Pre',@TestTitle,@Duration,@TotalQuestions,@TotalMarks,@PassingPercentage,@RandomQuestion,@ShuffleOption,@AllowRetest,@MaxAttempt,0,GETDATE())";
                objDB.ExecuteSql(sql, new SqlParameter[] {
                    new SqlParameter("@TestID",ViewState["TestID"]),
                    new SqlParameter("@TrainingID",ViewState["TrainingID"]),
                    new SqlParameter("@SessionID",ViewState["SessionID"]),
                    new SqlParameter("@TopicID",ViewState["TopicID"]),
                    new SqlParameter("@TrainerID",ViewState["TrainerID"]),
                    new SqlParameter("@TestTitle",txtTestTitle.Text),
                    new SqlParameter("@Duration",txtDuration.Text),
                    new SqlParameter("@TotalQuestions",txtTotalQuestions.Text),
                    new SqlParameter("@TotalMarks",totalMarks),
                    new SqlParameter("@PassingPercentage",txtPassing.Text),
                    new SqlParameter("@RandomQuestion",chkRandom.Checked),
                    new SqlParameter("@ShuffleOption",chkShuffle.Checked),
                    new SqlParameter("@AllowRetest",chkAllowRetest.Checked),
                    new SqlParameter("@MaxAttempt",txtAttempt.Text)
                });
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('" + ex.Message.Replace("'", "") + "');", true);
            }
        }

        private void UpdateTestMaster()
        {
            decimal totalMarks = Convert.ToDecimal(txtMarks.Text) * Convert.ToInt32(txtTotalQuestions.Text);
            string sql = "UPDATE TestMaster SET TestTitle=@TestTitle,Duration=@Duration,TotalQuestions=@TotalQuestions,TotalMarks=@TotalMarks,PassingPercentage=@PassingPercentage,RandomQuestion=@RandomQuestion,ShuffleOption=@ShuffleOption,AllowRetest=@AllowRetest,MaxAttempt=@MaxAttempt,ModifiedOn=GETDATE() WHERE TestID=@TestID AND TrainerID=@TrainerID";
            objDB.ExecuteSql(sql, new SqlParameter[] {
                new SqlParameter("@TestTitle",txtTestTitle.Text),
                new SqlParameter("@Duration",txtDuration.Text),
                new SqlParameter("@TotalQuestions",txtTotalQuestions.Text),
                new SqlParameter("@TotalMarks",totalMarks),
                new SqlParameter("@PassingPercentage",txtPassing.Text),
                new SqlParameter("@RandomQuestion",chkRandom.Checked),
                new SqlParameter("@ShuffleOption",chkShuffle.Checked),
                new SqlParameter("@AllowRetest",chkAllowRetest.Checked),
                new SqlParameter("@MaxAttempt",txtAttempt.Text),
                new SqlParameter("@TestID",ViewState["TestID"]),
                new SqlParameter("@TrainerID",Session["TrainerID"])
            });
        }

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            if (!ValidatePublish()) return;

            PublishTest();

            if (!CandidateQuestionsExist())
                GenerateCandidateQuestions();

            ScriptManager.RegisterStartupScript(this, GetType(), "msg", "alert('Test Published Successfully.');", true);
        }

        private bool CandidateQuestionsExist()
        {
            int count = Convert.ToInt32(objDB.ExecuteScalar(
                "SELECT COUNT(*) FROM TestCandidateQuestion WHERE TestID=@TestID",
                new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]) }));
            return count > 0;
        }

        private void PublishTest()
        {
            string sql = "UPDATE TestMaster SET IsPublished=1,TestStatus='Published',ModifiedOn=GETDATE() WHERE TestID=@TestID AND TrainerID=@TrainerID";
            objDB.ExecuteSql(sql, new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]), new SqlParameter("@TrainerID", Session["TrainerID"]) });
            btnPublish.Enabled = false;
            btnPublish.Text = "Published";
            btnGenerateQuestions.Enabled = false;
            btnSaveDraft.Enabled = false;
        }

        private void GenerateCandidateQuestions()
        {
            DataTable dtEmployee = objDB.GetDataTable(
                "SELECT EmpID FROM TrainingAssignment WHERE TrainingID=@TrainingID",
                new SqlParameter[] { new SqlParameter("@TrainingID", ViewState["TrainingID"]) });

            foreach (DataRow empRow in dtEmployee.Rows)
                SaveCandidateQuestions(empRow["EmpID"].ToString().ToUpperInvariant());
        }

        private void SaveCandidateQuestions(string empID)
        {
            string sql = "SELECT TQ.QuestionID,TQ.QuestionOrder,TQ.Marks,QB.CorrectOption FROM TestQuestion TQ INNER JOIN QuestionBank QB ON TQ.QuestionID=QB.QuestionID WHERE TQ.TestID=@TestID ORDER BY TQ.QuestionOrder";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]) });
            foreach (DataRow row in dt.Rows) InsertCandidateQuestion(empID, row);
        }

        private void InsertCandidateQuestion(string empID, DataRow row)
        {
            string candidateQuestionID = "TCQ" + Guid.NewGuid().ToString("N").Substring(0, 16);
            string sql = "INSERT INTO TestCandidateQuestion (TestCandidateQuestionID,TestID,EmpID,QuestionID,QuestionOrder,Marks,SelectedOption,CorrectOption,IsCorrect,CreatedOn) VALUES (@TestCandidateQuestionID,@TestID,@EmpID,@QuestionID,@QuestionOrder,@Marks,NULL,@CorrectOption,NULL,GETDATE())";
            objDB.ExecuteSql(sql, new SqlParameter[] {
                new SqlParameter("@TestCandidateQuestionID",candidateQuestionID),
                new SqlParameter("@TestID",ViewState["TestID"]),
                new SqlParameter("@EmpID",empID),
                new SqlParameter("@QuestionID",row["QuestionID"]),
                new SqlParameter("@QuestionOrder",row["QuestionOrder"]),
                new SqlParameter("@Marks",row["Marks"]),
                new SqlParameter("@CorrectOption",row["CorrectOption"])
            });
        }

        private void LoadQuestionPool()
        {
            if (ViewState["TopicID"] == null) { lblPool.Text = "No Topic Selected"; return; }
            string sql = "SELECT DifficultyLevel,COUNT(*) Total FROM QuestionBank WHERE TopicID=@TopicID AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID)) GROUP BY DifficultyLevel";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] {
                new SqlParameter("@TopicID",ViewState["TopicID"]),
                new SqlParameter("@TrainerID",ViewState["TrainerID"])
            });

            int easy=0, medium=0, hard=0;
            foreach (DataRow row in dt.Rows)
            {
                switch (row["DifficultyLevel"].ToString())
                {
                    case "Easy": easy=Convert.ToInt32(row["Total"]); break;
                    case "Medium": medium=Convert.ToInt32(row["Total"]); break;
                    case "Hard": hard=Convert.ToInt32(row["Total"]); break;
                }
            }
            lblPool.Text = "Easy : " + easy + "<br/>Medium : " + medium + "<br/>Hard : " + hard + "<br/><b>Total : " + (easy+medium+hard) + "</b>";
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("SessionDetails.aspx");
        }

        protected void gvQuestion_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lbl = (Label)e.Row.FindControl("lblSlNo");
                if (lbl != null) lbl.Text = (e.Row.RowIndex + 1).ToString();
            }
        }
    }
}