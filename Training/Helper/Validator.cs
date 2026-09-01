using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
namespace Training.Helper
{

    public class Validator
    {
        public static bool Required(string value)
        {
            return
                !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNumeric(string value)
        {
            decimal number;

            return
                decimal.TryParse(
                value,
                out number);
        }

        public static bool IsInteger(string value)
        {
            int number;

            return
                int.TryParse(
                value,
                out number);
        }

        public static bool IsDate(string value)
        {
            DateTime dt;

            return
                DateTime.TryParse(
                value,
                out dt);
        }

        public static bool IsFutureDate(DateTime date)
        {
            return
                date.Date >
                DateTime.Today;
        }

        public static bool IsPastDate(DateTime date)
        {
            return
                date.Date <
                DateTime.Today;
        }

        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return
                Regex.IsMatch(
                email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return false;

            return
                Regex.IsMatch(
                mobile,
                @"^[6-9][0-9]{9}$");
        }

        public static bool IsPAN(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan))
                return false;

            return
                Regex.IsMatch(
                pan.ToUpper(),
                @"^[A-Z]{5}[0-9]{4}[A-Z]{1}$");
        }

        public static bool IsAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar))
                return false;

            return
                Regex.IsMatch(
                aadhaar,
                @"^[0-9]{12}$");
        }

        public static bool IsPDF(HttpPostedFile file)
        {
            if (file == null)
                return false;

            string extension =
                Path.GetExtension(
                file.FileName)
                .ToLower();

            return
                extension == ".pdf";
        }

        public static bool IsExcel(HttpPostedFile file)
        {
            if (file == null)
                return false;

            string extension =
                Path.GetExtension(
                file.FileName)
                .ToLower();

            return
                extension == ".xls"
                ||
                extension == ".xlsx";
        }

        public static bool IsImage(HttpPostedFile file)
        {
            if (file == null)
                return false;

            string extension =
                Path.GetExtension(
                file.FileName)
                .ToLower();

            return
                extension == ".jpg"
                ||
                extension == ".jpeg"
                ||
                extension == ".png";
        }

        public static bool IsWord(HttpPostedFile file)
        {
            if (file == null)
                return false;

            string extension =
                Path.GetExtension(
                file.FileName)
                .ToLower();

            return
                extension == ".doc"
                ||
                extension == ".docx";
        }

        public static bool MaxFileSize(
            HttpPostedFile file,
            int sizeInMB)
        {
            if (file == null)
                return false;

            int maxSize =
                sizeInMB *
                1024 *
                1024;

            return
                file.ContentLength <=
                maxSize;
        }

        public static bool IsValidFileName(
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            char[] invalidChars =
                Path.GetInvalidFileNameChars();

            return
                fileName.IndexOfAny(
                invalidChars) < 0;
        }

        public static bool IsPositiveNumber(
            decimal value)
        {
            return
                value > 0;
        }

        public static bool IsPositiveInteger(
            int value)
        {
            return
                value > 0;
        }

        public static bool Between(
            int value,
            int min,
            int max)
        {
            return
                value >= min &&
                value <= max;
        }

        public static bool Between(
            decimal value,
            decimal min,
            decimal max)
        {
            return
                value >= min &&
                value <= max;
        }

        public static bool IsNull(object obj)
        {
            return
                obj == null
                ||
                obj == DBNull.Value;
        }
    }
}