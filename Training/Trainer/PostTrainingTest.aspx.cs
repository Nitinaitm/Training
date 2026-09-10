using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class PostTrainingTest : System.Web.UI.Page
    {
        private clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
                if (Session["TrainingID"] == null || Session["SessionID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }
                ViewState["SessionID"] = Session["SessionID"].ToString();
                SessionSummary1.LoadSession(Session["SessionID"].ToString());
                LoadSessionDetails();
                if (!CheckPostTrainingRequired()) return;
                LoadQuestionPool();
                CheckAttendance();
                CheckExistingTest();
            }
        }

        private void LoadSessionDetails()
        {
            string sql = "SELECT SM.SessionID,SM.SessionName,SM.SessionDate,SM.TopicID,TM.TopicName,SM.TrainerID,ISNULL(EBM.EmpName,TMR.NameExternal) AS TrainerName,TD.TrainingID,TD.TrainingType,TD.BatchStrength FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID INNER JOIN TopicMaster TM ON SM.TopicID=TM.TopicID LEFT JOIN EmpBasicMaster EBM ON SM.TrainerID=EBM.EmpID LEFT JOIN TrainerMaster TMR ON SM.TrainerID=TMR.TrainerID WHERE SM.SessionID=@SessionID";
            DataTable dt = objDB.GetDataTable(sql, new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]) });
            if (dt.Rows.Count == 0) { Response.Redirect("Default.aspx"); return; }
            ViewState["TopicID"] = dt.Rows[0]["TopicID"].ToString();
            ViewState["TrainerID"] = dt.Rows[0]["TrainerID"].ToString();
            ViewState["TrainingID"] = dt.Rows[0]["TrainingID"].ToString();
            lblSession.Text = dt.Rows[0]["SessionName"].ToString();
        }

        private bool CheckPostTrainingRequired()
        {
            object result = objDB.ExecuteScalar("SELECT FinalAssessmentRequired,ISNULL((SELECT PostAssessmentSkipped FROM SessionMaster WHERE SessionID=@SessionID),0) AS PostAssessmentSkipped FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", ViewState["TrainingID"]), new SqlParameter("@SessionID", ViewState["SessionID"]) });
            if (result == null || result == DBNull.Value) return false;
            bool required = Convert.ToBoolean(result);
            object skipped = objDB.ExecuteScalar("SELECT ISNULL(PostAssessmentSkipped,0) FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]) });
            if (!required)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PostTrainingRequired", "alert('Post-Training Assessment is not required for this training.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
                return false;
            }
            if (skipped != null && skipped != DBNull.Value && Convert.ToBoolean(skipped))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PostTrainingSkipped", "alert('Post-Training Assessment has been skipped for this session.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
                return false;
            }
            return true;
        }

        private void CheckAttendance()
        {
            object skipped = objDB.ExecuteScalar("SELECT ISNULL(AttendanceSkipped,0) FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]) });
            if (skipped != null && skipped != DBNull.Value && Convert.ToBoolean(skipped)) return;
            object status = objDB.ExecuteScalar("SELECT AttendanceStatus FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]) });
            if (status != null && status.ToString() != "Completed")
                ScriptManager.RegisterStartupScript(this, GetType(), "Attendance", "alert('Attendance is not completed for this session.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
        }

        private void CheckExistingTest()
        {
            DataTable dt = objDB.GetDataTable("SELECT * FROM TestMaster WHERE SessionID=@SessionID AND TestType='Post'", new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]) });
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
            DataTable dt = objDB.GetDataTable("SELECT TQ.QuestionID,QB.Question,QB.DifficultyLevel,TQ.Marks,QB.QuestionOwnerType FROM TestQuestion TQ INNER JOIN QuestionBank QB ON TQ.QuestionID=QB.QuestionID WHERE TQ.TestID=@TestID ORDER BY TQ.QuestionOrder", new SqlParameter[] { new SqlParameter("@TestID", ViewState["TestID"]) });
            ViewState["SelectedQuestions"] = dt;
            gvQuestion.DataSource = dt;
            gvQuestion.DataBind();
        }

        private void LoadQuestionPool()
        {
            if (ViewState["TopicID"] == null || ViewState["TrainerID"] == null) { lblPool.Text = "0 questions"; return; }
            DataTable dt = objDB.GetDataTable("SELECT DifficultyLevel,COUNT(*) QuestionCount FROM QuestionBank WHERE TopicID=@TopicID AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID)) GROUP BY DifficultyLevel", new SqlParameter[] { new SqlParameter("@TopicID", ViewState["TopicID"]), new SqlParameter("@TrainerID", ViewState["TrainerID"]) });
            int easy=0,medium=0,hard=0;
            foreach(DataRow r in dt.Rows){int n=Convert.ToInt32(r["QuestionCount"]);switch(r["DifficultyLevel"].ToString()){case "Easy":easy=n;break;case "Medium":medium=n;break;case "Hard":hard=n;break;}}
            lblPool.Text="Total: "+(easy+medium+hard)+" | Easy: "+easy+" | Medium: "+medium+" | Hard: "+hard;
        }

        private void LoadTest()
        {
            DataTable dt=objDB.GetDataTable("SELECT * FROM TestMaster WHERE TestID=@TestID",new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"])});
            if(dt.Rows.Count==0)return;
            txtTestTitle.Text=dt.Rows[0]["TestTitle"].ToString(); txtDuration.Text=dt.Rows[0]["Duration"].ToString(); txtTotalQuestions.Text=dt.Rows[0]["TotalQuestions"].ToString(); txtPassing.Text=dt.Rows[0]["PassingPercentage"].ToString(); chkRandom.Checked=Convert.ToBoolean(dt.Rows[0]["RandomQuestion"]); chkShuffle.Checked=Convert.ToBoolean(dt.Rows[0]["ShuffleOption"]); chkAllowRetest.Checked=Convert.ToBoolean(dt.Rows[0]["AllowRetest"]); txtAttempt.Text=dt.Rows[0]["MaxAttempt"].ToString();
            decimal totalMarks=Convert.ToDecimal(dt.Rows[0]["TotalMarks"]); int totalQuestion=Convert.ToInt32(dt.Rows[0]["TotalQuestions"]); if(totalQuestion>0)txtMarks.Text=(totalMarks/totalQuestion).ToString("0.##");
            if(Convert.ToBoolean(dt.Rows[0]["IsPublished"])){btnPublish.Enabled=false;btnPublish.Text="Published";btnGenerateQuestions.Enabled=false;btnSaveDraft.Enabled=false;}
        }

        private void SetDefaultValues(){txtTestTitle.Text=lblSession.Text+" Post Training Test";txtDuration.Text="30";txtTotalQuestions.Text="20";txtMarks.Text="1";txtPassing.Text="40";txtAttempt.Text="1";txtEasy.Text="5";txtMedium.Text="10";txtHard.Text="5";chkRandom.Checked=true;chkShuffle.Checked=true;chkAllowRetest.Checked=false;}

        protected void btnGenerateQuestions_Click(object sender, EventArgs e)
        {
            if(!ValidateQuestionDistribution() || !ValidateQuestionPool()) return;
            if(chkRandom.Checked) GenerateRandomQuestions(); else LoadManualQuestions();
            LoadQuestionPool();
            ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Questions Generated Successfully.');",true);
        }

        protected void chkRandom_CheckedChanged(object sender, EventArgs e){gvQuestion.DataSource=null;gvQuestion.DataBind();}

        private void LoadManualQuestions()
        {
            DataTable dt=objDB.GetDataTable("SELECT QuestionID,Question,DifficultyLevel,Marks,QuestionOwnerType FROM QuestionBank WHERE TopicID=@TopicID AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID)) ORDER BY CASE DifficultyLevel WHEN 'Easy' THEN 1 WHEN 'Medium' THEN 2 WHEN 'Hard' THEN 3 END,QuestionID",new SqlParameter[]{new SqlParameter("@TopicID",ViewState["TopicID"]),new SqlParameter("@TrainerID",ViewState["TrainerID"])});
            gvQuestion.DataSource=dt;gvQuestion.DataBind();
            foreach(GridViewRow row in gvQuestion.Rows){CheckBox chk=(CheckBox)row.FindControl("chkSelect");if(chk!=null)chk.Checked=false;}
        }

        private bool ValidateQuestionDistribution(){int total=Convert.ToInt32(txtTotalQuestions.Text);int easy=Convert.ToInt32(txtEasy.Text);int medium=Convert.ToInt32(txtMedium.Text);int hard=Convert.ToInt32(txtHard.Text);if(easy+medium+hard!=total){ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Easy + Medium + Hard should be equal to Total Questions.');",true);return false;}return true;}

        private bool ValidateQuestionPool(){return CheckDifficultyCount("Easy",Convert.ToInt32(txtEasy.Text))&&CheckDifficultyCount("Medium",Convert.ToInt32(txtMedium.Text))&&CheckDifficultyCount("Hard",Convert.ToInt32(txtHard.Text));}

        private bool CheckDifficultyCount(string difficulty,int requiredCount)
        {
            object value=objDB.ExecuteScalar("SELECT COUNT(*) FROM QuestionBank WHERE TopicID=@TopicID AND DifficultyLevel=@DifficultyLevel AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID))",new SqlParameter[]{new SqlParameter("@TopicID",ViewState["TopicID"]),new SqlParameter("@DifficultyLevel",difficulty),new SqlParameter("@TrainerID",ViewState["TrainerID"])});
            int available=Convert.ToInt32(value);if(available<requiredCount){ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Only "+available+" "+difficulty+" questions are available.');",true);return false;}return true;
        }

        private DataTable CreateQuestionTable(){DataTable dt=new DataTable();dt.Columns.Add("QuestionID");dt.Columns.Add("Question");dt.Columns.Add("DifficultyLevel");dt.Columns.Add("Marks",typeof(decimal));dt.Columns.Add("QuestionOwnerType");return dt;}

        private void GenerateRandomQuestions()
        {
            DataTable dt=CreateQuestionTable();GetRandomQuestions(dt,"Easy",Convert.ToInt32(txtEasy.Text));GetRandomQuestions(dt,"Medium",Convert.ToInt32(txtMedium.Text));GetRandomQuestions(dt,"Hard",Convert.ToInt32(txtHard.Text));ViewState["SelectedQuestions"]=dt;BindSelectedQuestions();
        }

        private void GetRandomQuestions(DataTable target,string difficulty,int count)
        {
            DataTable dt=objDB.GetDataTable("SELECT TOP "+count+" QuestionID,Question,DifficultyLevel,Marks,QuestionOwnerType FROM QuestionBank WHERE TopicID=@TopicID AND DifficultyLevel=@DifficultyLevel AND IsActive=1 AND ((QuestionOwnerType='Admin') OR (QuestionOwnerType='Trainer' AND ApprovalStatus='Approved') OR (QuestionOwnerType='Trainer' AND OwnerID=@TrainerID)) ORDER BY NEWID()",new SqlParameter[]{new SqlParameter("@TopicID",ViewState["TopicID"]),new SqlParameter("@DifficultyLevel",difficulty),new SqlParameter("@TrainerID",ViewState["TrainerID"])});
            foreach(DataRow r in dt.Rows)target.Rows.Add(r["QuestionID"],r["Question"],r["DifficultyLevel"],r["Marks"],r["QuestionOwnerType"]);
        }

        private void BindSelectedQuestions(){gvQuestion.DataSource=ViewState["SelectedQuestions"] as DataTable;gvQuestion.DataBind();}

        protected void btnSaveDraft_Click(object sender, EventArgs e)
        {
            if(!chkRandom.Checked && !CreateManualQuestionTable())return;
            DataTable dt=ViewState["SelectedQuestions"] as DataTable;if(dt==null||dt.Rows.Count==0){ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Please Generate Questions First.');",true);return;}
            if(ViewState["TestID"]==null){ViewState["TestID"]="TST"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+new Random().Next(100,999);InsertTestMaster();}else UpdateTestMaster();
            SaveTestQuestions();ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Draft Saved Successfully.');",true);
        }

        private bool CreateManualQuestionTable()
        {
            DataTable dt=CreateQuestionTable();int easy=0,medium=0,hard=0;
            foreach(GridViewRow row in gvQuestion.Rows){CheckBox chk=(CheckBox)row.FindControl("chkSelect");if(chk==null||!chk.Checked)continue;DataKey key=gvQuestion.DataKeys[row.RowIndex];string difficulty=key.Values["DifficultyLevel"].ToString();dt.Rows.Add(key.Values["QuestionID"],key.Values["Question"],difficulty,Convert.ToDecimal(key.Values["Marks"]),key.Values["QuestionOwnerType"]);if(difficulty=="Easy")easy++;else if(difficulty=="Medium")medium++;else if(difficulty=="Hard")hard++;}
            if(easy!=Convert.ToInt32(txtEasy.Text)||medium!=Convert.ToInt32(txtMedium.Text)||hard!=Convert.ToInt32(txtHard.Text)){ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Please select the required Easy, Medium and Hard questions.');",true);return false;}ViewState["SelectedQuestions"]=dt;return true;
        }

        protected void chkAll_CheckedChanged(object sender, EventArgs e){CheckBox all=(CheckBox)sender;foreach(GridViewRow row in gvQuestion.Rows){CheckBox chk=(CheckBox)row.FindControl("chkSelect");if(chk!=null)chk.Checked=all.Checked;}}

        private void SaveTestQuestions()
        {
            objDB.ExecuteSql("DELETE FROM TestQuestion WHERE TestID=@TestID",new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"])});
            DataTable dt=ViewState["SelectedQuestions"] as DataTable;if(dt==null)return;int order=1;foreach(DataRow r in dt.Rows){string id="TQ"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+new Random().Next(100,999);objDB.ExecuteSql("INSERT INTO TestQuestion (TestQuestionID,TestID,QuestionID,QuestionOrder,Marks,CreatedOn) VALUES (@TestQuestionID,@TestID,@QuestionID,@QuestionOrder,@Marks,GETDATE())",new SqlParameter[]{new SqlParameter("@TestQuestionID",id),new SqlParameter("@TestID",ViewState["TestID"]),new SqlParameter("@QuestionID",r["QuestionID"]),new SqlParameter("@QuestionOrder",order++),new SqlParameter("@Marks",r["Marks"])});}
        }

        private bool ValidatePublish(){DataTable dt=ViewState["SelectedQuestions"] as DataTable;if(ViewState["TestID"]==null){ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Please Save Draft First.');",true);return false;}if(dt==null||dt.Rows.Count!=Convert.ToInt32(txtTotalQuestions.Text)){ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Question Count Mismatch.');",true);return false;}return true;}

        private void InsertTestMaster()
        {
            decimal totalMarks=Convert.ToDecimal(txtMarks.Text)*Convert.ToInt32(txtTotalQuestions.Text);
            objDB.ExecuteSql("INSERT INTO TestMaster (TestID,TrainingID,SessionID,TopicID,TrainerID,TestType,TestTitle,Duration,TotalQuestions,TotalMarks,PassingPercentage,RandomQuestion,ShuffleOption,AllowRetest,MaxAttempt,IsPublished,CreatedOn) VALUES (@TestID,@TrainingID,@SessionID,@TopicID,@TrainerID,'Post',@TestTitle,@Duration,@TotalQuestions,@TotalMarks,@PassingPercentage,@RandomQuestion,@ShuffleOption,@AllowRetest,@MaxAttempt,0,GETDATE())",new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"]),new SqlParameter("@TrainingID",ViewState["TrainingID"]),new SqlParameter("@SessionID",ViewState["SessionID"]),new SqlParameter("@TopicID",ViewState["TopicID"]),new SqlParameter("@TrainerID",ViewState["TrainerID"]),new SqlParameter("@TestTitle",txtTestTitle.Text),new SqlParameter("@Duration",txtDuration.Text),new SqlParameter("@TotalQuestions",txtTotalQuestions.Text),new SqlParameter("@TotalMarks",totalMarks),new SqlParameter("@PassingPercentage",txtPassing.Text),new SqlParameter("@RandomQuestion",chkRandom.Checked),new SqlParameter("@ShuffleOption",chkShuffle.Checked),new SqlParameter("@AllowRetest",chkAllowRetest.Checked),new SqlParameter("@MaxAttempt",txtAttempt.Text)});
        }

        private void UpdateTestMaster(){decimal totalMarks=Convert.ToDecimal(txtMarks.Text)*Convert.ToInt32(txtTotalQuestions.Text);objDB.ExecuteSql("UPDATE TestMaster SET TestTitle=@TestTitle,Duration=@Duration,TotalQuestions=@TotalQuestions,TotalMarks=@TotalMarks,PassingPercentage=@PassingPercentage,RandomQuestion=@RandomQuestion,ShuffleOption=@ShuffleOption,AllowRetest=@AllowRetest,MaxAttempt=@MaxAttempt,ModifiedOn=GETDATE() WHERE TestID=@TestID",new SqlParameter[]{new SqlParameter("@TestTitle",txtTestTitle.Text),new SqlParameter("@Duration",txtDuration.Text),new SqlParameter("@TotalQuestions",txtTotalQuestions.Text),new SqlParameter("@TotalMarks",totalMarks),new SqlParameter("@PassingPercentage",txtPassing.Text),new SqlParameter("@RandomQuestion",chkRandom.Checked),new SqlParameter("@ShuffleOption",chkShuffle.Checked),new SqlParameter("@AllowRetest",chkAllowRetest.Checked),new SqlParameter("@MaxAttempt",txtAttempt.Text),new SqlParameter("@TestID",ViewState["TestID"])});}

        protected void btnPublish_Click(object sender, EventArgs e)
        {
            if(!ValidatePublish())return;objDB.ExecuteSql("UPDATE TestMaster SET IsPublished=1,TestStatus='Published',ModifiedOn=GETDATE() WHERE TestID=@TestID",new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"])});GenerateCandidateQuestions();btnPublish.Enabled=false;btnPublish.Text="Published";btnGenerateQuestions.Enabled=false;btnSaveDraft.Enabled=false;ScriptManager.RegisterStartupScript(this,GetType(),"msg","alert('Test Published Successfully.');",true);
        }

        private void GenerateCandidateQuestions()
        {
            DataTable employees=objDB.GetDataTable("SELECT EmpID FROM TrainingAssignment WHERE TrainingID=@TrainingID AND ISNULL(AssignmentStatus,'Assigned')='Assigned'",new SqlParameter[]{new SqlParameter("@TrainingID",ViewState["TrainingID"])});
            DataTable questions=objDB.GetDataTable("SELECT TQ.QuestionID,TQ.QuestionOrder,TQ.Marks,QB.CorrectOption FROM TestQuestion TQ INNER JOIN QuestionBank QB ON TQ.QuestionID=QB.QuestionID WHERE TQ.TestID=@TestID ORDER BY TQ.QuestionOrder",new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"])});
            foreach(DataRow emp in employees.Rows)foreach(DataRow q in questions.Rows){string id="TCQ"+DateTime.Now.ToString("yyyyMMddHHmmssfff")+new Random().Next(100,999);objDB.ExecuteSql("INSERT INTO TestCandidateQuestion (TestCandidateQuestionID,TestID,EmpID,QuestionID,QuestionOrder,Marks,SelectedOption,CorrectOption,IsCorrect,CreatedOn) VALUES (@ID,@TestID,@EmpID,@QuestionID,@QuestionOrder,@Marks,NULL,@CorrectOption,NULL,GETDATE())",new SqlParameter[]{new SqlParameter("@ID",id),new SqlParameter("@TestID",ViewState["TestID"]),new SqlParameter("@EmpID",emp["EmpID"]),new SqlParameter("@QuestionID",q["QuestionID"]),new SqlParameter("@QuestionOrder",q["QuestionOrder"]),new SqlParameter("@Marks",q["Marks"]),new SqlParameter("@CorrectOption",q["CorrectOption"])});}
        }

        protected void btnBack_Click(object sender, EventArgs e){Response.Redirect("SessionDetails.aspx?SessionID="+Server.UrlEncode(Convert.ToString(ViewState["SessionID"])));}

        protected void gvQuestion_RowDataBound(object sender, GridViewRowEventArgs e){if(e.Row.RowType==DataControlRowType.DataRow){Label lbl=e.Row.FindControl("lblSlNo") as Label;if(lbl!=null)lbl.Text=(e.Row.RowIndex+1).ToString();}}
    }
}