using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class AssignSession
    {
        private string GetTrainerIDDisplay(object displayID, object trainerType)
        {
            string id = Convert.ToString(displayID);
            string type = Convert.ToString(trainerType);
            if (!string.Equals(type, "Internal", StringComparison.OrdinalIgnoreCase)) return "Trainer ID: " + id;
            DataTable dt = new clsDataAccess().GetDataTable(
                "SELECT TOP 1 TrainerID,EmpID FROM TrainerMaster WHERE EmpID=@EmpID",
                new SqlParameter[] { new SqlParameter("@EmpID", id) });
            if (dt.Rows.Count == 0) return "Emp ID: " + id;
            return "Emp ID: " + dt.Rows[0]["EmpID"] + " | Trainer ID: " + dt.Rows[0]["TrainerID"];
        }

        private string GetTrainerMobile(object displayID, object trainerType)
        {
            string id = Convert.ToString(displayID);
            string type = Convert.ToString(trainerType);
            string sql = string.Equals(type, "Internal", StringComparison.OrdinalIgnoreCase)
                ? "SELECT TOP 1 E.MobileNo FROM TrainerMaster T LEFT JOIN EmpBasicMaster E ON E.EmpID=T.EmpID WHERE T.EmpID=@ID"
                : "SELECT TOP 1 MobileNo FROM TrainerMaster WHERE TrainerID=@ID";
            object v = new clsDataAccess().ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@ID", id) });
            return v == null || v == DBNull.Value ? "-" : v.ToString();
        }

        private string GetTrainerEmail(object displayID, object trainerType)
        {
            string id = Convert.ToString(displayID);
            string type = Convert.ToString(trainerType);
            string sql = string.Equals(type, "Internal", StringComparison.OrdinalIgnoreCase)
                ? "SELECT TOP 1 E.EmailId FROM TrainerMaster T LEFT JOIN EmpBasicMaster E ON E.EmpID=T.EmpID WHERE T.EmpID=@ID"
                : "SELECT TOP 1 EmailID FROM TrainerMaster WHERE TrainerID=@ID";
            object v = new clsDataAccess().ExecuteScalar(sql, new SqlParameter[] { new SqlParameter("@ID", id) });
            return v == null || v == DBNull.Value ? "-" : v.ToString();
        }
    }
}
