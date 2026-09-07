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
using System.Collections.Generic;

public class clsDataAccess
{
    private DataTable _dt; private int _vic; private Page _page; private string _id;
    SqlConnection con = new SqlConnection(); SqlTransaction Trans;
    public SqlConnection Connection { get { return con; } }
    public SqlTransaction Transaction { get { return Trans; } }
    public clsDataAccess() { string connectionString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString; con = new SqlConnection(connectionString); }
    public DataTable GetDataTable(string query)
    {
        DataTable dt = new DataTable(); try { con.Open(); SqlCommand cmd = new SqlCommand(); cmd.CommandText=query; SqlDataAdapter adap=new SqlDataAdapter(); cmd.Connection=con; adap.SelectCommand=cmd; adap.Fill(dt); return dt; } catch(Exception) { return dt; } finally { con.Close(); }
    }
    public DataTable GetDataTable(string query, SqlParameter[] param)
    {
        DataTable dt=new DataTable(); try { con.Open(); SqlCommand cmd=new SqlCommand(); cmd.CommandText=query; if(param!=null) foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); SqlDataAdapter adap1=new SqlDataAdapter(); cmd.Connection=con; adap1.SelectCommand=cmd; adap1.Fill(dt); return dt; } catch(Exception) { return dt; } finally { con.Close(); }
    }
    public int ExecuteSql(string Query)
    {
        try { con.Open(); SqlCommand cmd=new SqlCommand(); cmd.CommandText=Query; cmd.Connection=con; return cmd.ExecuteNonQuery(); } catch(Exception) { return 0; } finally { con.Close(); }
    }
    public int ExecuteSql(string Query, SqlParameter[] param)
    {
        try { con.Open(); SqlCommand cmd=new SqlCommand(); cmd.CommandText=Query; if(param!=null) foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); cmd.Connection=con; return cmd.ExecuteNonQuery(); } catch(Exception ex) { HttpContext.Current.Response.Write(ex.Message); return 0; } finally { con.Close(); }
    }
    public int ExecuteSql(string Query, List<SqlParameter> param, Label lblMsg)
    {
        try { con.Open(); SqlCommand cmd=new SqlCommand(); cmd.CommandText=Query; foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); cmd.Connection=con; return cmd.ExecuteNonQuery(); } catch(Exception ex) { lblMsg.Text=ex.Message; return 0; } finally { con.Close(); }
    }
    public string ExecuteScalar(string strSql)
    {
        SqlCommand cmd=new SqlCommand(); try { cmd.CommandType=CommandType.Text; cmd.CommandText=strSql; cmd.Connection=con; cmd.Connection.Open(); return cmd.ExecuteScalar().ToString(); } catch(Exception) { cmd.Connection.Close(); return ""; } finally { cmd.Connection.Close(); }
    }
    public void OpenConnection() { SqlCommand cmd=new SqlCommand(); if(con.State==ConnectionState.Closed) con.Open(); cmd.Connection=con; }
    public void CloseConnection() { con.Close(); }
    public void BeginTransaction() { if(con.State==ConnectionState.Closed) con.Open(); Trans=con.BeginTransaction(IsolationLevel.Serializable); }
    public void BeginTransaction(IsolationLevel level) { if(con.State==ConnectionState.Closed) con.Open(); SqlCommand cmd=new SqlCommand(); Trans=con.BeginTransaction(level); cmd.Transaction=Trans; }
    public void Commit() { if(Trans!=null){Trans.Commit();Trans.Dispose();Trans=null;} if(con.State==ConnectionState.Open) con.Close(); }
    public void Rollback() { if(Trans!=null){Trans.Rollback();Trans.Dispose();Trans=null;} if(con.State==ConnectionState.Open) con.Close(); }
    public object ExecuteScalar(string Query, SqlParameter[] param)
    {
        SqlCommand cmd=new SqlCommand(); try { cmd.CommandType=CommandType.Text; cmd.CommandText=Query; cmd.Connection=con; cmd.Connection.Open(); if(param!=null) foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); object objRet=cmd.ExecuteScalar(); if(cmd.Connection.State==ConnectionState.Open) cmd.Connection.Close(); return objRet; } catch(Exception) { if(cmd.Connection.State==ConnectionState.Open) cmd.Connection.Close(); return null; } finally { if(cmd.Connection.State==ConnectionState.Open) cmd.Connection.Close(); }
    }
    public int ExecuteSql(string query, SqlParameter[] param, SqlTransaction trans)
    {
        try { SqlCommand cmd=new SqlCommand(); cmd.CommandText=query; cmd.Connection=trans.Connection; cmd.Transaction=trans; if(param!=null) foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); return cmd.ExecuteNonQuery(); } catch { throw; }
    }
    public object ExecuteScalar(string query, SqlParameter[] param, SqlTransaction trans)
    {
        SqlCommand cmd=new SqlCommand(); cmd.CommandText=query; cmd.Connection=trans.Connection; cmd.Transaction=trans; if(param!=null) foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); return cmd.ExecuteScalar();
    }
    public DataTable GetDataTable(string query, SqlParameter[] param, SqlTransaction trans)
    {
        DataTable dt=new DataTable(); SqlCommand cmd=new SqlCommand(); cmd.CommandText=query; cmd.Connection=trans.Connection; cmd.Transaction=trans; if(param!=null) foreach(SqlParameter prm in param) cmd.Parameters.Add(prm); SqlDataAdapter da=new SqlDataAdapter(cmd); da.Fill(dt); return dt;
    }
    public SqlCommand CreateCommand()
    {
        SqlCommand cmd=new SqlCommand(); cmd.Connection=con; if(Trans!=null) cmd.Transaction=Trans; return cmd;
    }
}