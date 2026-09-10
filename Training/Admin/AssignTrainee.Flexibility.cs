using System;
using System.Data;
using System.Data.SqlClient;

namespace Training.Admin
{
    public partial class AssignTrainee
    {
        private string GetEmployeeMobile(object empID)
        {
            object v = new clsDataAccess().ExecuteScalar(
                "SELECT TOP 1 MobileNo FROM EmpBasicMaster WHERE EmpID=@EmpID",
                new SqlParameter[] { new SqlParameter("@EmpID", Convert.ToString(empID)) });
            return v == null || v == DBNull.Value ? "-" : v.ToString();
        }

        private string GetEmployeeEmail(object empID)
        {
            object v = new clsDataAccess().ExecuteScalar(
                "SELECT TOP 1 EmailId FROM EmpBasicMaster WHERE EmpID=@EmpID",
                new SqlParameter[] { new SqlParameter("@EmpID", Convert.ToString(empID)) });
            return v == null || v == DBNull.Value ? "-" : v.ToString();
        }
    }
}
