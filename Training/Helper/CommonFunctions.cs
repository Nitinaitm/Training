using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;


namespace Training.Helper
{

    public class CommonFunctions
    {
        clsDataAccess objDB =
            new clsDataAccess();

        public string GetServerDate()
        {
            string sql =
                "SELECT CONVERT(VARCHAR(10),GETDATE(),105)";

            object obj =
                objDB.ExecuteScalar(
                sql,
                null);

            if (obj == null)
                return "";

            return obj.ToString();
        }

        public DateTime GetServerDateTime()
        {
            string sql =
                "SELECT GETDATE()";

            object obj =
                objDB.ExecuteScalar(
                sql,
                null);

            if (obj == null)
                return DateTime.Now;

            return Convert.ToDateTime(
                obj);
        }

        public bool RecordExists(
            string sql,
            SqlParameter[] param)
        {
            DataTable dt =
                objDB.GetDataTable(
                sql,
                param);

            return
                dt.Rows.Count > 0;
        }

        public int GetCount(
            string sql,
            SqlParameter[] param)
        {
            object obj =
                objDB.ExecuteScalar(
                sql,
                param);

            if (
                obj == null ||
                obj == DBNull.Value)
            {
                return 0;
            }

            return
                Convert.ToInt32(
                obj);
        }

        public decimal GetDecimal(
            string sql,
            SqlParameter[] param)
        {
            object obj =
                objDB.ExecuteScalar(
                sql,
                param);

            if (
                obj == null ||
                obj == DBNull.Value)
            {
                return 0;
            }

            return
                Convert.ToDecimal(
                obj);
        }

        public string GetString(
            string sql,
            SqlParameter[] param)
        {
            object obj =
                objDB.ExecuteScalar(
                sql,
                param);

            if (
                obj == null ||
                obj == DBNull.Value)
            {
                return "";
            }

            return
                obj.ToString();
        }

        public DateTime? GetDate(
            string sql,
            SqlParameter[] param)
        {
            object obj =
                objDB.ExecuteScalar(
                sql,
                param);

            if (
                obj == null ||
                obj == DBNull.Value)
            {
                return null;
            }

            return
                Convert.ToDateTime(
                obj);
        }

        public bool Execute(
            string sql,
            SqlParameter[] param)
        {
            int i =
                objDB.ExecuteSql(
                sql,
                param);

            return
                i > 0;
        }
    }
}