using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainee
{
    public partial class MyTrainings : System.Web.UI.Page
    {
        clsDataAccess objDB = new clsDataAccess();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["EmpID"] == null) { Response.Redirect("~/Default.aspx"); return; }
            if (!IsPostBack) { LoadCourse(); ViewState["SortExpression"]="TrainingID"; ViewState["SortDirection"]="DESC"; ddlStatus.SelectedIndex=0; LoadTraining(); }
        }
        private string SortColumn(){string s=Convert.ToString(ViewState["SortExpression"]);switch(s){case "TrainingID":case "CourseName":case "TrainingType":case "TrainingOrganizer":case "Batch":case "DateFrom":case "DateTo":return s;default:return "TrainingID";}}
        private string SortDirection(){return Convert.ToString(ViewState["SortDirection"])=="ASC"?"ASC":"DESC";}
        private void LoadCourse(){DataTable dt=objDB.GetDataTable("SELECT DISTINCT CM.CourseID,CM.CourseName FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TA.EmpID=@EmpID ORDER BY CM.CourseName",new SqlParameter[] { new SqlParameter("@EmpID",Session["EmpID"].ToString().ToUpperInvariant()) });ddlCourse.DataSource=dt;ddlCourse.DataTextField="CourseName";ddlCourse.DataValueField="CourseID";ddlCourse.DataBind();ddlCourse.Items.Insert(0,new ListItem("All",""));}
        protected void btnSearch_Click(object sender,EventArgs e){gvTraining.PageIndex=0;LoadTraining();}
        protected void btnReset_Click(object sender,EventArgs e){txtTrainingID.Text="";ddlCourse.SelectedIndex=0;ddlStatus.SelectedIndex=0;ViewState["SortExpression"]="TrainingID";ViewState["SortDirection"]="DESC";gvTraining.PageIndex=0;LoadTraining();}
        protected void gvTraining_PageIndexChanging(object sender,GridViewPageEventArgs e){gvTraining.PageIndex=e.NewPageIndex;LoadTraining();}
        protected void gvTraining_Sorting(object sender,GridViewSortEventArgs e){if(Convert.ToString(ViewState["SortExpression"])==e.SortExpression)ViewState["SortDirection"]=Convert.ToString(ViewState["SortDirection"])=="ASC"?"DESC":"ASC";else{ViewState["SortExpression"]=e.SortExpression;ViewState["SortDirection"]="ASC";}LoadTraining();}
        protected void gvTraining_RowDataBound(object sender,GridViewRowEventArgs e){if(e.Row.RowType!=DataControlRowType.DataRow)return;LinkButton f=(LinkButton)e.Row.FindControl("lnkFeedback");LinkButton c=(LinkButton)e.Row.FindControl("lnkCertificate");LinkButton a=(LinkButton)e.Row.FindControl("lnkAttendance");if(f!=null&&!f.Enabled)f.CssClass="btn btn-warning btn-sm disabled";if(c!=null&&!c.Enabled)c.CssClass="btn btn-info btn-sm disabled";if(a!=null&&!a.Enabled)a.CssClass="btn btn-primary btn-sm disabled";}
        protected void gvTraining_RowCommand(object sender,GridViewCommandEventArgs e){string trainingID=e.CommandArgument.ToString();Session["TrainingID"]=trainingID;if(e.CommandName=="ViewTraining"){Response.Redirect("TrainingDetails.aspx",false);return;}if(e.CommandName=="Attendance"){Response.Redirect("Attendance.aspx",false);return;}if(e.CommandName=="BatchFeedback"){Response.Redirect("TraineeFeedback.aspx",false);return;}if(e.CommandName=="Certificate"){Session["CertificateFromTraining"]=true;Session["SessionID"]="CERTIFICATE";Response.Redirect("MyCertificate.aspx",false);}}
        private void LoadTraining()
        {
            string sql=@"SELECT TA.TrainingID,CM.CourseName,TD.TrainingType,TD.TrainingOrganizer,TD.Batch,TRY_CONVERT(date,TD.DateFrom,105) DateFrom,TRY_CONVERT(date,TD.DateTo,105) DateTo,TD.AttendanceRequired,TD.InitialAssessmentRequired,TD.FinalAssessmentRequired,TD.FeedbackRequired,TD.CertificateRequired,CASE WHEN TD.AttendanceRequired=1 AND EXISTS(SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TA.TrainingID AND NOT EXISTS(SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=TA.EmpID AND SA.AttendanceStatus='Completed')) THEN 0 ELSE 1 END AS AttendanceDone,CASE WHEN TD.InitialAssessmentRequired=1 AND EXISTS(SELECT 1 FROM TestMaster T INNER JOIN SessionMaster SM ON SM.SessionID=T.SessionID WHERE SM.TrainingID=TA.TrainingID AND T.TestType='Pre' AND T.IsPublished=1 AND NOT EXISTS(SELECT 1 FROM TestAttempt A WHERE A.TestID=T.TestID AND A.EmpID=TA.EmpID AND A.Submitted=1)) THEN 0 ELSE 1 END AS PreDone,CASE WHEN TD.FinalAssessmentRequired=1 AND EXISTS(SELECT 1 FROM TestMaster T INNER JOIN SessionMaster SM ON SM.SessionID=T.SessionID WHERE SM.TrainingID=TA.TrainingID AND T.TestType='Post' AND T.IsPublished=1 AND NOT EXISTS(SELECT 1 FROM TestAttempt A WHERE A.TestID=T.TestID AND A.EmpID=TA.EmpID AND A.Submitted=1)) THEN 0 ELSE 1 END AS PostDone,CASE WHEN TD.FeedbackRequired=1 AND EXISTS(SELECT 1 FROM Feedback F WHERE F.TrainingID=TA.TrainingID AND F.EmpID=TA.EmpID) THEN 1 WHEN TD.FeedbackRequired=0 THEN 1 ELSE 0 END AS FeedbackDone FROM TrainingAssignment TA INNER JOIN TrainingDetails TD ON TA.TrainingID=TD.TrainingID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE TA.EmpID=@EmpID";
            string filter=txtTrainingID.Text.Trim(); if(!string.IsNullOrEmpty(filter))sql+=" AND TA.TrainingID LIKE @TrainingID"; if(ddlCourse.SelectedValue!="")sql+=" AND TD.CourseID=@CourseID"; string status=ddlStatus.SelectedValue; if(status=="Pending")sql+=" AND TD.TrainingStatus='Pending'"; else if(status=="In Progress")sql+=" AND TD.TrainingStatus='In Progress'"; else if(status=="Completed")sql+=" AND TD.TrainingStatus='Completed'"; sql+=" ORDER BY "+SortColumn()+" "+SortDirection();
            SqlParameter[] p={new SqlParameter("@EmpID",Session["EmpID"].ToString().ToUpperInvariant()),new SqlParameter("@TrainingID",filter+"%"),new SqlParameter("@CourseID",ddlCourse.SelectedValue)}; DataTable dt=objDB.GetDataTable(sql,p); gvTraining.DataSource=dt;gvTraining.DataBind();
        }
    }
}