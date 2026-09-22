using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class ManagerMaster : Page
    {
        private readonly clsDataAccess objDB = new clsDataAccess();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindMapForLocation();
                BindTrainingLocation();
                BindGrid();
            }
        }

        private void BindMapForLocation()
        {
            DataTable dt = objDB.GetDataTable("SELECT DISTINCT LTRIM(RTRIM(EmpPostingPlace)) AS EmpPostingPlace FROM EmpBasicMaster WHERE EmpPostingPlace IS NOT NULL AND LTRIM(RTRIM(EmpPostingPlace)) <> '' ORDER BY LTRIM(RTRIM(EmpPostingPlace))");
            ddlMapForLocation.DataSource = dt;
            ddlMapForLocation.DataTextField = "EmpPostingPlace";
            ddlMapForLocation.DataValueField = "EmpPostingPlace";
            ddlMapForLocation.DataBind();
            ddlMapForLocation.Items.Insert(0, new ListItem("Select", ""));
        }

        private void BindTrainingLocation()
        {
            DataTable dt = objDB.GetDataTable("SELECT TrainingLocationID, TrainingLocation FROM TrainingLocationMaster ORDER BY TrainingLocation");
            ddlTrainingLocation.DataSource = dt;
            ddlTrainingLocation.DataTextField = "TrainingLocation";
            ddlTrainingLocation.DataValueField = "TrainingLocationID";
            ddlTrainingLocation.DataBind();
            ddlTrainingLocation.Items.Insert(0, new ListItem("Select", ""));
        }

        private void BindGrid()
        {
            DataTable dt = objDB.GetDataTable("SELECT M.ID,M.ManagerID,M.EmpID,E.EmpName,E.EmpDesignation AS Designation,E.EmpPostingPlace AS PlaceOfPosting,M.MapForLocation,L.TrainingLocation,M.CreatedOn FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID LEFT JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID WHERE ISNULL(M.ActiveStatus,'Y')='Y' ORDER BY M.ID DESC");
            gvManager.DataSource = dt;
            gvManager.DataBind();
        }

        private void BindSearchGrid()
        {
            string search = txtSearch.Text.Trim();
            DataTable dt = objDB.GetDataTable("SELECT M.ID,M.EmpID,E.EmpName,E.EmpDesignation AS Designation,E.EmpPostingPlace AS PlaceOfPosting,M.MapForLocation,L.TrainingLocation,M.CreatedOn FROM ManagerMaster M INNER JOIN EmpBasicMaster E ON M.EmpID=E.EmpID LEFT JOIN TrainingLocationMaster L ON M.TrainingLocationID=L.TrainingLocationID WHERE ISNULL(M.ActiveStatus,'Y')='Y' AND (M.EmpID LIKE @Search OR E.EmpName LIKE @Search OR E.EmpDesignation LIKE @Search OR E.EmpPostingPlace LIKE @Search OR M.MapForLocation LIKE @Search OR L.TrainingLocation LIKE @Search OR M.ManagerID LIKE @Search) ORDER BY M.ID DESC", new SqlParameter[] { new SqlParameter("@Search", "%" + search + "%") });
            gvManager.DataSource = dt;
            gvManager.DataBind();
        }

        protected void txtEmpID_TextChanged(object sender, EventArgs e)
        {
            LoadEmployeeDetails();
        }

        private void LoadEmployeeDetails()
        {
            string empID = txtEmpID.Text.Trim();
            ClearEmployeeDetails();

            if (empID == "")
            {
                return;
            }

            DataTable dt = objDB.GetDataTable("SELECT EmpID,EmpName,DOB,DOJ,MobileNo,EmailId,EmpDesignation,EmpPostingPlace FROM EmpBasicMaster WHERE EmpID=@EmpID", new SqlParameter[] { new SqlParameter("@EmpID", empID) });

            if (dt.Rows.Count == 0)
            {
                ShowMessage("EmpID not found.", System.Drawing.Color.Red);
                return;
            }

            DataRow dr = dt.Rows[0];
            txtEmpID.Text = dr["EmpID"].ToString();
            txtEmpName.Text = dr["EmpName"].ToString();
            txtDOB.Text = dr["DOB"].ToString();
            txtDOJ.Text = dr["DOJ"].ToString();
            txtMobileNo.Text = dr["MobileNo"].ToString();
            txtEmailID.Text = dr["EmailId"].ToString();
            txtPlaceOfPosting.Text = dr["EmpPostingPlace"].ToString();
            txtDesignation.Text = dr["EmpDesignation"].ToString();

            string postingPlace = dr["EmpPostingPlace"].ToString().Trim();
            if (ddlMapForLocation.Items.FindByValue(postingPlace) != null)
            {
                ddlMapForLocation.SelectedValue = postingPlace;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate("SaveGroup");

            if (!Page.IsValid)
            {
                return;
            }

            string empID = txtEmpID.Text.Trim();

            DataTable dtEmp = objDB.GetDataTable("SELECT EmpID FROM EmpBasicMaster WHERE EmpID=@EmpID", new SqlParameter[] { new SqlParameter("@EmpID", empID) });

            if (dtEmp.Rows.Count == 0)
            {
                ShowMessage("EmpID not found.", System.Drawing.Color.Red);
                return;
            }

            int result = objDB.ExecuteSql("INSERT INTO ManagerMaster (ManagerID,EmpID,MapForLocation,TrainingLocationID,CreatedBy,ActiveStatus) SELECT 'MGR' + RIGHT('0000' + CAST(ISNULL(MAX(ID),0) + 1 AS VARCHAR(20)),4),@EmpID,@MapForLocation,@TrainingLocationID,@CreatedBy,'Y' FROM ManagerMaster", new SqlParameter[]
            {
                new SqlParameter("@EmpID", empID),
                new SqlParameter("@MapForLocation", ddlMapForLocation.SelectedValue),
                new SqlParameter("@TrainingLocationID", ddlTrainingLocation.SelectedValue),
                new SqlParameter("@CreatedBy", "Admin")
            });

            if (result > 0)
            {
                CreateManagerLogin(empID);
                ShowMessage("Manager mapping saved successfully.", System.Drawing.Color.Green);
                ClearForm();
                BindGrid();
            }
            else
            {
                ShowMessage("Manager mapping could not be saved.", System.Drawing.Color.Red);
            }
        }

        private void CreateManagerLogin(string empID)
        {
            DataTable dtManager = objDB.GetDataTable("SELECT TOP 1 ManagerID FROM ManagerMaster WHERE EmpID=@EmpID AND ISNULL(ActiveStatus,'Y')='Y' ORDER BY ID DESC", new SqlParameter[] { new SqlParameter("@EmpID", empID) });
            if (dtManager.Rows.Count == 0) return;
            string managerID = dtManager.Rows[0]["ManagerID"].ToString().Trim();
            if (string.IsNullOrWhiteSpace(managerID)) return;
            DataTable dtLogin = objDB.GetDataTable("SELECT LoginIDUserID FROM Login WHERE LoginIDUserID=@LoginIDUserID", new SqlParameter[] { new SqlParameter("@LoginIDUserID", managerID) });
            if (dtLogin.Rows.Count > 0) return;
            Encryptor2 encryptor = new Encryptor2();
            string password = encryptor.Encrypt("Bsphcl*123");
            string firstLogin = encryptor.Encrypt("Y");
            objDB.ExecuteSql("INSERT INTO Login (LoginIDUserID,Password,Role,CorrespondingEmpID,Active,re) VALUES (@LoginIDUserID,@Password,'Manager',@CorrespondingEmpID,'Y',@FirstLogin)", new SqlParameter[] { new SqlParameter("@LoginIDUserID", managerID), new SqlParameter("@Password", password), new SqlParameter("@CorrespondingEmpID", empID), new SqlParameter("@FirstLogin", firstLogin) });
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            txtEmpID.Text = "";
            ClearEmployeeDetails();
            if (ddlMapForLocation.Items.Count > 0) ddlMapForLocation.SelectedIndex = 0;
            if (ddlTrainingLocation.Items.Count > 0) ddlTrainingLocation.SelectedIndex = 0;
            lblMessage.Text = "";
            btnSave.Text = "Save Manager";
        }

        private void ClearEmployeeDetails()
        {
            txtEmpName.Text = "";
            txtDOB.Text = "";
            txtDOJ.Text = "";
            txtMobileNo.Text = "";
            txtEmailID.Text = "";
            txtPlaceOfPosting.Text = "";
            txtDesignation.Text = "";
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindSearchGrid();
        }

        protected void gvManager_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvManager.EditIndex = e.NewEditIndex;
            BindGrid();
        }

        protected void gvManager_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvManager.EditIndex = -1;
            BindGrid();
        }

        protected void gvManager_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int id = Convert.ToInt32(gvManager.DataKeys[e.RowIndex].Value);
            TextBox txtMapForLocation = (TextBox)gvManager.Rows[e.RowIndex].Cells[5].Controls[0];

            int result = objDB.ExecuteSql("UPDATE ManagerMaster SET MapForLocation=@MapForLocation WHERE ID=@ID", new SqlParameter[] { new SqlParameter("@MapForLocation", txtMapForLocation.Text.Trim()), new SqlParameter("@ID", id) });

            gvManager.EditIndex = -1;
            BindGrid();

            if (result > 0)
            {
                ShowMessage("Manager mapping updated successfully.", System.Drawing.Color.Green);
            }
        }

        protected void gvManager_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvManager.DataKeys[e.RowIndex].Value);
            int result = objDB.ExecuteSql("UPDATE ManagerMaster SET ActiveStatus='N' WHERE ID=@ID", new SqlParameter[] { new SqlParameter("@ID", id) });

            if (result > 0)
            {
                ShowMessage("Manager mapping deactivated successfully.", System.Drawing.Color.Green);
            }

            BindGrid();
        }

        private void ShowMessage(string message, System.Drawing.Color color)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = color;
        }
    }
}