using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

/// <summary>
/// Summary description for clsTransaction
/// </summary>
public class clsTransaction
{
    public SqlCommand SqlCmd = new SqlCommand();
    public SqlConnection SqlConn = new SqlConnection(ConfigurationManager.ConnectionStrings["BICConnectionString"].ConnectionString);

    SqlTransaction Trans;
    public clsTransaction()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public DataTable getDataTable(string strSql)
    {

        DataTable dt = new DataTable();
        SqlCmd.CommandType = CommandType.Text;
        SqlCmd.CommandText = strSql;
        SqlCmd.Connection = SqlConn;
        SqlDataAdapter da = new SqlDataAdapter();
        da.SelectCommand = SqlCmd;
        da.Fill(dt);
        da.Dispose();
        return dt;
    }

    public int ExecuteSql(string SqlStr)
    {
        try
        {
            SqlCmd.CommandType = CommandType.Text;
            SqlCmd.CommandText = SqlStr;
            return SqlCmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return 0;
        }
    }


    public int ExecuteSqlPro(string ProcName, ref SqlParameter[] param)
    {
        try
        {
            SqlCmd.CommandType = CommandType.StoredProcedure;
            SqlCmd.CommandText = ProcName;

            SqlCmd.Parameters.AddRange(param);

            return SqlCmd.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            return 0;
        }
    }




    public string UpdateDataUnicode(string TableName, string[] ColNames, string[] ColValues, string[] ColTypes, string[] WhColNames, string[] WhColValues, string[] WhColTypes)
    {
        string SQL;
        string SQL1, SQL2;
        SQL1 = "";
        SQL2 = "";
        SqlCmd.Parameters.Clear();
        try
        {
            SQL1 = "";
            SQL2 = "";
            for (int i = 0; i < ColNames.Length; i++)
            {
                if (SQL1.ToString() == "")
                {

                    //SQL1 = ColNames[i].ToString() + " = " + " '" + ColValues[i].ToString() + "'";
                    SQL1 = ColNames[i].ToString() + " = " + " @" + "" + ColNames[i].ToString() + "";
                    // SQL2 = "@" + ColNames[i].ToString();

                }
                else
                {

                    SQL1 = SQL1 + "," + ColNames[i].ToString() + " = " + " @" + "" + ColNames[i].ToString() + "";


                }
                if (ColValues[i] == "NULL")
                    SqlCmd.Parameters.AddWithValue("@" + ColNames[i], DBNull.Value); // (@param name, value)
                else
                    SqlCmd.Parameters.AddWithValue("@" + ColNames[i], ColValues[i]); // (@param name, value)



            }
            for (int j = 0; j < WhColNames.Length; j++)
            {
                if (SQL2.ToString() == "")
                {
                    SQL2 = WhColNames[j].ToString() + " = " + " @" + "" + WhColNames[j].ToString() + "";

                }
                else
                {
                    SQL2 = SQL2 + " " + "And " + WhColNames[j].ToString() + " = " + " @" + "" + WhColNames[j].ToString() + "";
                }
                if (WhColTypes[j] == "1") // for unicode
                {
                    SqlCmd.Parameters.AddWithValue("@" + WhColNames[j], WhColValues[j]); // (@param name, value)
                }
                else
                {
                    SqlCmd.Parameters.AddWithValue("@" + WhColNames[j], WhColValues[j]); // (@param name, value)
                }
            }

            string strCommand = "Update " + TableName + " " + " SET  " + SQL1 + " " + " Where " + SQL2 + " ";

            SqlCmd.CommandText = strCommand;
            SqlCmd.CommandType = CommandType.Text;
            SqlCmd.ExecuteNonQuery();

            return "1";
        }

        catch (Exception e)
        {
            return "0";
        }
    }

    public string SaveDataSimple(string TableName, string[] ColNames, string[] ColValues)
    {

        string SQL1, SQL2;
        SqlCmd.Parameters.Clear();
        try
        {

            SQL1 = "";
            SQL2 = "";
            for (int i = 0; i < ColNames.Length; i++)
            {
                if (SQL1.ToString() == "")
                {
                    SQL1 = ColNames[i].ToString();
                    SQL2 = "@" + ColNames[i].ToString();

                }
                else
                {
                    SQL1 = SQL1 + ", " + ColNames[i].ToString();
                    SQL2 = SQL2 + ", " + "@" + ColNames[i].ToString();
                }
                // add parameter value
                if (ColValues[i].ToString() != "NULL")
                    SqlCmd.Parameters.AddWithValue("@" + ColNames[i], ColValues[i]); // (@param name, value)
                else
                    SqlCmd.Parameters.AddWithValue("@" + ColNames[i], DBNull.Value); // (@param name, value)

            }

            string strCommand = "Insert into " + TableName + " ( " + SQL1 + " ) " + " VALUES ( " + SQL2 + " )";
            SqlCmd.CommandText = strCommand;
            SqlCmd.CommandType = CommandType.Text;
            SqlCmd.ExecuteNonQuery();
            return "1";
        }
        catch (Exception ex)
        {
            return "0";

        }



    }

    public string SaveDataPro(string TableName, string[] ColNames, string[] ColValues, string[] ColTypes, string App_PrifixID)
    {
        string SQL;
        string SQL1, SQL2;

        try
        {

            SQL1 = "";
            SQL2 = "";
            for (int i = 0; i < ColNames.Length; i++)
            {
                if (SQL1.ToString() == "")
                {
                    SQL1 = ColNames[i].ToString();
                    SQL2 = "'" + ColValues[i].ToString() + "'";
                }
                else
                {
                    SQL1 = SQL1 + ", " + ColNames[i].ToString();
                    if (ColTypes[i] != "1") // for unicode
                    {

                        if (ColValues[i].ToString() != "NULL")
                            SQL2 = SQL2 + ", N'" + ColValues[i].ToString() + "'";
                        else
                            SQL2 = SQL2 + ", " + ColValues[i].ToString() + "";
                    }
                    else
                    {
                        if (ColValues[i].ToString() != "NULL")
                            SQL2 = SQL2 + ", '" + ColValues[i].ToString() + "'";
                        else
                            SQL2 = SQL2 + ", " + ColValues[i].ToString() + "";
                    }
                }

            }
            string strCommand = "Insert into " + TableName + " ( " + SQL1 + " ) " + " VALUES ( " + SQL2 + " )";


            SqlCmd.CommandText = "Application_Insert";
            SqlCmd.CommandType = CommandType.StoredProcedure;

            SqlCmd.Parameters.Clear();
            SqlParameter Apl_Cer_No = new SqlParameter("@Apl_Cer_No", SqlDbType.VarChar, 25);
            Apl_Cer_No.Direction = ParameterDirection.Output;
            SqlCmd.Parameters.Add(Apl_Cer_No);
            SqlCmd.Parameters.AddWithValue("@ID_Prefix", App_PrifixID);
            SqlCmd.Parameters.AddWithValue("@TableName", TableName);
            SqlCmd.Parameters.AddWithValue("@Query", strCommand);
            SqlCmd.ExecuteNonQuery();
            return Apl_Cer_No.Value.ToString();
        }
        catch (Exception ex)
        {
            return "0";


        }



    }


    public void OpenConnection()
    {
        SqlConn.Open();
        SqlCmd.Connection = SqlConn;
    }
    public void CloseConnection()
    {
        SqlConn.Close();
    }

    public void BeginTransaction()
    {
        Trans = SqlConn.BeginTransaction(IsolationLevel.Serializable);
        SqlCmd.Transaction = Trans;

    }
    public void BeginTransaction(IsolationLevel level)
    {
        Trans = SqlConn.BeginTransaction(level);
        SqlCmd.Transaction = Trans;

    }

    public void Commit()
    {
        Trans.Commit();

    }
    public void Rollback()
    {
        Trans.Rollback();

    }
}
