using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class MySessions : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) BindGrid();
        }
        private string TrainerID { get { return Session["TrainerID"].ToString(); } }
        private void BindGrid()
        {
            string query=@"SELECT SM.SessionID,SM.TrainingID,TD.Batch,SM.SessionNo,SM.SessionName,TM.TopicName,SM.SessionDate,SM.StartTime+' - '+SM.EndTime AS SessionTime,
TD.AttendanceRequired,TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,
CASE WHEN TD.AttendanceRequired=0 THEN '-' ELSE ISNULL((SELECT TOP 1 SA.AttendanceStatus FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID),'Pending') END AS AttendanceStatus
FROM SessionMaster SM INNER JOIN TrainingDetails TD ON SM.TrainingID=TD.TrainingID LEFT JOIN TopicMaster TM ON TM.TopicID=SM.TopicID
WHERE SM.TrainerID=@TrainerID ORDER BY TRY_CONVERT(date,SM.SessionDate,105),TRY_CONVERT(INT,SM.SessionNo),SM.SessionNo";
            DataTable dt=obj.GetDataTable(query,new SqlParameter("@TrainerID",TrainerID)); gvSessions.DataSource=dt;gvSessions.DataBind();
        }
        protected void gvSessions_RowCommand(object sender,GridViewCommandEventArgs e)
        {
            if(e.CommandName!="ViewSession")return;
            Session["SessionID"]=e.CommandArgument.ToString();
            Response.Redirect("~/Trainer/SessionDetails.aspx");
        }
        protected void gvSessions_RowDataBound(object sender,GridViewRowEventArgs e)
        {
            if(e.Row.RowType!=DataControlRowType.DataRow)return;
            Button btn=(Button)e.Row.FindControl("btnAction"); Label lbl=(Label)e.Row.FindControl("lblAttendance");
            bool attendanceRequired=Convert.ToBoolean(DataBinder.Eval(e.Row.DataItem,"AttendanceRequired"));
            if(btn!=null){btn.Visible=attendanceRequired;btn.Text=lbl!=null&&lbl.Text=="Completed"?"View":"Take Attendance";}
            if(lbl!=null)lbl.Visible=attendanceRequired;
        }
    }
}