using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Trainee
{
    public partial class MySessions
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (btnPreTest != null) { btnPreTest.Click -= btnPreTest_Click; btnPreTest.Click += FlexiblePreTest_Click; }
            if (btnPostTest != null) { btnPostTest.Click -= btnPostTest_Click; btnPostTest.Click += FlexiblePostTest_Click; }
        }

        private void FlexiblePreTest_Click(object sender, EventArgs e)
        {
            if (!PreRequired) return;
            if (AttendanceBlocksTests()) { ClientScript.RegisterStartupScript(GetType(), "msg", "alert('Please complete attendance for this session first.');", true); return; }
            if (btnPreTest.CommandArgument == "Result") { Session["ResultTestType"] = "Pre"; Response.Redirect("MyExamResult.aspx"); return; }
            Response.Redirect("PreTrainingExam.aspx");
        }

        private void FlexiblePostTest_Click(object sender, EventArgs e)
        {
            if (!PostRequired) return;
            if (AttendanceBlocksTests()) { ClientScript.RegisterStartupScript(GetType(), "msg", "alert('Please complete attendance for this session first.');", true); return; }
            if (btnPostTest.CommandArgument == "Result") { Session["ResultTestType"] = "Post"; Response.Redirect("MyExamResult.aspx"); return; }
            Response.Redirect("PostTrainingExam.aspx");
        }

        private bool AttendanceBlocksTests()
        {
            if (!AttendanceRequired) return false;
            object value = objDB.ExecuteScalar("SELECT ISNULL(AttendanceSkipped,0) FROM SessionMaster WHERE SessionID=@SessionID", new SqlParameter[] { new SqlParameter("@SessionID", Session["SessionID"].ToString()) });
            if (value != null && value != DBNull.Value && Convert.ToInt32(value) == 1) return false;
            return !SessionAttendanceDone();
        }
    }
}
