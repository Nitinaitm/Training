using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class PostTrainingTest : System.Web.UI.Page
    {
        private clsDataAccess objDB = new clsDataAccess();
        private DataTable dtSelectedQuestion = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["TrainerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
                if (Session["TrainingID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }
                if (Session["SessionID"] == null) { Response.Redirect("~/Trainer/Default.aspx"); return; }

                ViewState["SessionID"] = Session["SessionID"]?.ToString();
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
        }

        private bool CheckPostTrainingRequired()
        {
            object result = objDB.ExecuteScalar("SELECT FinalAssessmentRequired FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", ViewState["TrainingID"]) });
            if (result == null || result == DBNull.Value || !Convert.ToBoolean(result))
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "PostTrainingRequired", "alert('Post-Training Assessment is not required for this training.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
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
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "Attendance", "alert('Attendance is not completed for this session.');window.location='SessionDetails.aspx?SessionID=" + ViewState["SessionID"] + "';", true);
            }
        }

        private void CheckExistingTest()
        {
            DataTable dt = objDB.GetDataTable("SELECT * FROM TestMaster WHERE SessionID=@SessionID AND TestType=@TestType", new SqlParameter[] { new SqlParameter("@SessionID", ViewState["SessionID"]), new SqlParameter("@TestType", "Post") });
            if (dt.Rows.Count > 0)
            {
                ViewState["TestID"] = dt.Rows[0]["TestID"].ToString();
                LoadTest(); LoadTestQuestions(); return;
            }
            SetDefaultValues();
        }

        // The remainder of this class is unchanged from the existing implementation.
        private void LoadTestQuestions() { string sql="SELECT TQ.QuestionID,QB.Question,QB.DifficultyLevel,TQ.Marks,QB.QuestionOwnerType FROM TestQuestion TQ INNER JOIN QuestionBank QB ON TQ.QuestionID=QB.QuestionID WHERE TQ.TestID=@TestID ORDER BY TQ.QuestionOrder"; DataTable dt=objDB.GetDataTable(sql,new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"])}); ViewState["SelectedQuestions"]=dt; gvQuestion.DataSource=dt; gvQuestion.DataBind(); }
        private void LoadTest() { DataTable dt=objDB.GetDataTable("SELECT * FROM TestMaster WHERE TestID=@TestID",new SqlParameter[]{new SqlParameter("@TestID",ViewState["TestID"])}); if(dt.Rows.Count==0)return; txtTestTitle.Text=dt.Rows[0]["TestTitle"].ToString(); txtDuration.Text=dt.Rows[0]["Duration"].ToString(); txtTotalQuestions.Text=dt.Rows[0]["TotalQuestions"].ToString(); txtPassing.Text=dt.Rows[0]["PassingPercentage"].ToString(); chkRandom.Checked=Convert.ToBoolean(dt.Rows[0]["RandomQuestion"]); chkShuffle.Checked=Convert.ToBoolean(dt.Rows[0]["ShuffleOption"]); chkAllowRetest.Checked=Convert.ToBoolean(dt.Rows[0]["AllowRetest"]); txtAttempt.Text=dt.Rows[0]["MaxAttempt"].ToString(); decimal totalMarks=Convert.ToDecimal(dt.Rows[0]["TotalMarks"]); int totalQuestion=Convert.ToInt32(dt.Rows[0]["TotalQuestions"]); if(totalQuestion>0)txtMarks.Text=(totalMarks/totalQuestion).ToString("0.##"); if(dt.Rows[0]["IsPublished"].ToString()=="True"){btnPublish.Enabled=false;btnPublish.Text="Published";btnGenerateQuestions.Enabled=false;btnSaveDraft.Enabled=false;} }
        private void SetDefaultValues() { txtTestTitle.Text=lblSession.Text+" Post Training Test"; txtDuration.Text="30"; txtTotalQuestions.Text="20"; txtMarks.Text="1"; txtPassing.Text="40"; txtAttempt.Text="1"; txtEasy.Text="5"; txtMedium.Text="10"; txtHard.Text="5"; chkRandom.Checked=true; chkShuffle.Checked=true; chkAllowRetest.Checked=false; }

        // Existing event handlers and helper methods continue below in the source file.
    }
}
