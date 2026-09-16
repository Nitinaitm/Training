using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for ValidateInput
/// </summary>
public class ValidateInput
{
	public ValidateInput()
	{
		//
		// TODO: Add constructor logic here
		//
	}
    public static Boolean CheckSqlInjection(string str)
    {
        if (str.Contains("'") || str.Contains(";")) return false;
        else return true;

    }
}