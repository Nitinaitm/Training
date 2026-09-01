using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for clsReport
/// </summary>
public class clsReport
{
    public clsReport()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public string getWhereClause(string tabname, string colname, string value, string strWhere)
    {
        if (value == "0")
            return string.Empty;
        else
            if (strWhere == string.Empty)
            {
                strWhere = " where " + "" + tabname + "" + "." + " " + colname + "='" + value + "'";
            }
            else
            {
                strWhere = " and  " + "" + tabname + "" + "." + " " + colname + " = '" + value + "'";
            }
        return strWhere;
    }

    public string GETDistrictCode(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "='" + ddlvalue + "'";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = '" + ddlvalue + "'";
            }
            return str;
        }

    }

    public string GETSubdivisionCode(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "='" + ddlvalue + "'";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = '" + ddlvalue + "'";
            }
            return str;
        }

    }

    public string GETBlockCode(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "='" + ddlvalue + "'";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = '" + ddlvalue + "'";
            }
            return str;
        }

    }

    public string GETPanchayatCode(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "='" + ddlvalue + "'";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = '" + ddlvalue + "'";
            }
            return str;
        }

    }

    public string GETStatusCode(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = " + ddlvalue + "";
            }
            return str;
        }

    }

    public string GETRegNo(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = " + ddlvalue + "";
            }
            return str;
        }

    }

    public string GETACCode(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = " + ddlvalue + "";
            }
            return str;
        }

    }
    public string GETLAEOID(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = " + ddlvalue + "";
            }
            return str;
        }

    }
    public string GETMemberId(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = " + ddlvalue + "";
            }
            return str;
        }

    }
    public string GETMemberTypeId(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "='" + ddlvalue + "'";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " = '" + ddlvalue + "'";
            }
            return str;
        }

    }
    public string GETSchemeTypeId(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " =" + ddlvalue + "";
            }
            return str;
        }

    }
    public string GetDateRange(string tabname, string colName, string datefrom, string dateto, string str)
    {
        if (datefrom == "" || dateto == "")
            return string.Empty;
        if (datefrom == "" && dateto == "")
            return string.Empty;

        if (str == string.Empty)
            str = " where " + "" + tabname + "" + "." + " " + colName + " " + "BETWEEN '" + datefrom.ToString() + "' AND  '" + dateto.ToString() + "' ";

        else
            str = " AND " + "" + tabname + "" + "." + " " + colName + " " + "BETWEEN '" + datefrom.ToString() + "' AND '" + dateto.ToString() + "' ";
        return str;
    }
    public string GETFYearId(string tabname, string colname, string ddlvalue, string str)
    {
        if (ddlvalue == "0")
        {
            return string.Empty;
        }
        else
        {
            if (str == string.Empty)
            {
                str = " where " + "" + tabname + "" + "." + " " + colname + "=" + ddlvalue + "";
            }
            else
            {
                str = " and  " + "" + tabname + "" + "." + " " + colname + " =" + ddlvalue + "";
            }
            return str;
        }

    }
}