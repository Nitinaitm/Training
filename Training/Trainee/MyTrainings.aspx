<%@ Page title="My Trainings" language="C#" masterpagefile="~/TraineeMaster.Master" autoeventwireup="true" codebehind="MyTrainings.aspx.cs" inherits="Training.Trainee.MyTrainings" %>
<%@ Register src="~/Trainee/TraineeTrainingSummary.ascx" tagprefix="uc1" tagname="TraineeTrainingSummary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<style>
.search-card,.grid-card{border:0;border-radius:10px;box-shadow:0 2px 10px rgba(0,0,0,.08)}
.gridview th{background:#198754;color:white;text-align:center;vertical-align:middle}.gridview td,.table td,.table th{vertical-align:middle!important}.badge-status{font-size:13px;padding:6px 10px;min-width:110px;display:inline-block;text-align:center}.page-title{font-size:24px;font-weight:600}.btn-group .btn{margin-right:4px}.btn-group{display:flex;flex-wrap:wrap;gap:4px}.progress{min-width:120px;height:20px}.btn[disabled],.btn.disabled{cursor:not-allowed;opacity:.55}
</style>
</asp:Content>
<asp:content id="Content2" contentplaceholderid="ContentPlaceHolder1" runat="server">
<div class="container-fluid">
<div class="row mb-3"><div class="col-md-12"><h3 class="page-title">My Trainings</h3></div></div>
<div class="card search-card mb-3"><div class="card-body"><div class="row">
<div class="col-md-3 mb-2"><label>Training ID</label><asp:TextBox ID="txtTrainingID" runat="server" CssClass="form-control" placeholder="Training ID"></asp:TextBox></div>
<div class="col-md-3 mb-2"><label>Course</label><asp:DropDownList ID="ddlCourse" runat="server" CssClass="form-control"></asp:DropDownList></div>
<div class="col-md-3 mb-2"><label>Status</label><asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control"><asp:ListItem Text="All" Value=""></asp:ListItem><asp:ListItem Text="Pending" Value="P"></asp:ListItem><asp:ListItem Text="In Progress" Value="I"></asp:ListItem><asp:ListItem Text="Completed" Value="C"></asp:ListItem></asp:DropDownList></div>
<div class="col-md-3"><label>&nbsp;</label><div><asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary" OnClick="btnSearch_Click" /><asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-secondary" OnClick="btnReset_Click" /></div></div>
</div></div></div>
<div class="card grid-card"><div class="card-body table-responsive">
<asp:GridView ID="gvTraining" runat="server" CssClass="table table-bordered table-hover gridview" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" PageSize="10" DataKeyNames="TrainingID" EmptyDataText="No Training Assigned." OnPageIndexChanging="gvTraining_PageIndexChanging" OnSorting="gvTraining_Sorting" OnRowCommand="gvTraining_RowCommand" OnRowDataBound="gvTraining_RowDataBound" OnDataBound="gvTraining_DataBound">
<Columns>
<asp:BoundField HeaderText="Training ID" DataField="TrainingID" /><asp:BoundField HeaderText="Course" DataField="CourseName" /><asp:BoundField HeaderText="Training Type" DataField="TrainingType" /><asp:BoundField HeaderText="Organizer" DataField="TrainingOrganizer" /><asp:BoundField HeaderText="Batch" DataField="Batch" /><asp:BoundField HeaderText="From" DataField="DateFrom" DataFormatString="{0:dd-MMM-yyyy}" /><asp:BoundField HeaderText="To" DataField="DateTo" DataFormatString="{0:dd-MMM-yyyy}" />
<asp:TemplateField HeaderText="Status"><ItemTemplate><asp:Label ID="lblStatus" runat="server" Text='<%# Eval("StatusText") %>' CssClass='<%# Eval("StatusClass") %>'></asp:Label></ItemTemplate></asp:TemplateField>
<asp:TemplateField HeaderText="Action"><ItemTemplate><div class="btn-group">
<asp:LinkButton ID="lnkView" runat="server" CssClass="btn btn-success btn-sm" CommandName="ViewTraining" CommandArgument='<%# Eval("TrainingID") %>'><i class="fa fa-eye"></i> View</asp:LinkButton>
<asp:LinkButton ID="lnkAttendance" runat="server" CssClass="btn btn-primary btn-sm" CommandName="Attendance" CommandArgument='<%# Eval("TrainingID") %>' Visible='<%# Convert.ToBoolean(Eval("AttendanceRequired")) %>' Enabled="true"><i class="fa fa-calendar-check-o"></i> Attendance</asp:LinkButton>
<asp:LinkButton ID="lnkFeedback" runat="server" CssClass="btn btn-warning btn-sm" CommandName="BatchFeedback" CommandArgument='<%# Eval("TrainingID") %>' Visible='<%# Convert.ToBoolean(Eval("FeedbackRequired")) && !Convert.ToBoolean(Eval("FeedbackSkipped")) %>' Enabled="false"><i class="fa fa-comments"></i> Feedback</asp:LinkButton>
<asp:LinkButton ID="lnkCertificate" runat="server" CssClass="btn btn-info btn-sm" CommandName="Certificate" CommandArgument='<%# Eval("TrainingID") %>' Visible='<%# Convert.ToBoolean(Eval("CertificateRequired")) && !Convert.ToBoolean(Eval("CertificateSkipped")) %>' Enabled='<%# Convert.ToBoolean(Eval("CanCertificate")) %>'><i class="fa fa-certificate"></i> Certificate</asp:LinkButton>
</div></ItemTemplate></asp:TemplateField>
</Columns><PagerStyle CssClass="pagination-ys" /></asp:GridView>
</div></div></div>
<script runat="server">
protected void gvTraining_DataBound(object sender, EventArgs e)
{
    string empID = Session["EmpID"] == null ? "" : Session["EmpID"].ToString().ToUpperInvariant();
    if (string.IsNullOrWhiteSpace(empID)) return;
    foreach (GridViewRow row in gvTraining.Rows)
    {
        LinkButton feedback = row.FindControl("lnkFeedback") as LinkButton;
        if (feedback == null) continue;
        string trainingID = gvTraining.DataKeys[row.RowIndex].Value.ToString();
        DataTable dt = objDB.GetDataTable(@"SELECT TD.FeedbackRequired,ISNULL(TD.FeedbackSkipped,0) FeedbackSkipped,
CASE WHEN TD.AttendanceRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID AND ISNULL(SM.AttendanceSkipped,0)=0 AND NOT EXISTS (SELECT 1 FROM SessionAttendance SA WHERE SA.SessionID=SM.SessionID AND SA.EmpID=@EmpID AND SA.AttendanceStatus IN ('Present','Completed'))) THEN 1 ELSE 0 END AttendanceBlocked,
CASE WHEN TD.InitialAssessmentRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID AND ISNULL(SM.PreAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Pre' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1))) THEN 1 ELSE 0 END PreBlocked,
CASE WHEN TD.FinalAssessmentRequired=1 AND EXISTS (SELECT 1 FROM SessionMaster SM WHERE SM.TrainingID=TD.TrainingID AND ISNULL(SM.PostAssessmentSkipped,0)=0 AND (NOT EXISTS (SELECT 1 FROM TestMaster TM WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1) OR NOT EXISTS (SELECT 1 FROM TestMaster TM INNER JOIN TestAttempt TA ON TA.TestID=TM.TestID WHERE TM.SessionID=SM.SessionID AND TM.TestType='Post' AND TM.IsPublished=1 AND TA.EmpID=@EmpID AND TA.Submitted=1))) THEN 1 ELSE 0 END PostBlocked,
CASE WHEN EXISTS (SELECT 1 FROM Feedback F WHERE F.TrainingID=TD.TrainingID AND F.EmpID=@EmpID AND F.Submitted=1) THEN 1 ELSE 0 END FeedbackSubmitted
FROM TrainingDetails TD WHERE TD.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID), new SqlParameter("@EmpID", empID) });
        if (dt.Rows.Count == 0) continue;
        DataRow r = dt.Rows[0];
        if (!Convert.ToBoolean(r["FeedbackRequired"]) || Convert.ToBoolean(r["FeedbackSkipped"])) { feedback.Visible=false; continue; }
        if (Convert.ToBoolean(r["FeedbackSubmitted"])) { feedback.Text="Feedback Submitted"; feedback.Enabled=false; feedback.CssClass="btn btn-success btn-sm disabled"; feedback.ToolTip="Feedback has already been submitted."; continue; }
        if (Convert.ToBoolean(r["AttendanceBlocked"])) { feedback.Enabled=false; feedback.ToolTip="Complete your required attendance first."; }
        else if (Convert.ToBoolean(r["PreBlocked"])) { feedback.Enabled=false; feedback.ToolTip="Complete all required Pre-Training Tests first. Required tests must be published."; }
        else if (Convert.ToBoolean(r["PostBlocked"])) { feedback.Enabled=false; feedback.ToolTip="Complete all required Post-Training Tests first. Required tests must be published."; }
        else { feedback.Enabled=true; feedback.ToolTip="You can submit feedback now."; }
    }
}
</script>
</asp:content>