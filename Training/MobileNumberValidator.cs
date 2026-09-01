using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace BSPHCL_ACR
{
    public class MobileNumberValidator
    {
        public static bool ValidateMobileNumber(string input)
        {
            // Remove any whitespace or special characters from the input
            string cleanedInput = Regex.Replace(input, @"[\s-]+", "");

            // Check if the cleaned input matches the pattern for a 10-digit Indian mobile number
            return Regex.IsMatch(cleanedInput, @"^[6-9]\d{9}$");
        }
    }
}