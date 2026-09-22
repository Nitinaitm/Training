using System;
using System.Data.SqlClient;
using System.Web.UI;

namespace Training.Manager
{
    public partial class TrainingRequirements
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (Session["Role"] == null || Session["Role"].ToString() != "Manager" || Session["ManagerID"] == null)
            {
                Response.Redirect("~/Default.aspx", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(TrainingID) || !HasManagerAccess())
            {
                Response.Redirect("~/Manager/Default.aspx", true);
                return;
            }

            if (IsTrainingCompleted())
            {
                Response.Redirect("~/Manager/Default.aspx", true);
            }
        }

        private bool HasManagerAccess()
        {
            object value = db.ExecuteScalar("SELECT COUNT(*) FROM ManagerMaster M INNER JOIN TrainingDetails TD ON M.MapForLocation=TD.TrainingLocation WHERE M.ManagerID=@ManagerID AND ISNULL(M.ActiveStatus,'Y')='Y' AND TD.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@ManagerID", Session["ManagerID"].ToString()), new SqlParameter("@TrainingID", TrainingID) });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) > 0;
        }

        private bool IsTrainingCompleted()
        {
            object value = db.ExecuteScalar("SELECT CASE WHEN ISNULL(TrainingStatus,'') IN ('Completed','TrainingCompleted') OR ISNULL(WorkflowStatus,'')='ABCDEFGHIJ' THEN 1 ELSE 0 END FROM TrainingDetails WHERE TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", TrainingID) });
            return value != null && value != DBNull.Value && Convert.ToInt32(value) == 1;
        }
    }
}
