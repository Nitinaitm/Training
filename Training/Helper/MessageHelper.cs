using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Web.UI.WebControls;
using Training.Common;

namespace Training.Helper
{


    public class MessageHelper
    {
        public static void Success(
            Label lblMessage,
            string message)
        {
            if (lblMessage == null)
                return;

            lblMessage.Text =
                message;

            lblMessage.ForeColor =
                Color.Green;

            lblMessage.Visible =
                true;
        }

        public static void Error(
            Label lblMessage,
            string message)
        {
            if (lblMessage == null)
                return;

            lblMessage.Text =
                message;

            lblMessage.ForeColor =
                Color.Red;

            lblMessage.Visible =
                true;
        }

        public static void Warning(
            Label lblMessage,
            string message)
        {
            if (lblMessage == null)
                return;

            lblMessage.Text =
                message;

            lblMessage.ForeColor =
                Color.DarkOrange;

            lblMessage.Visible =
                true;
        }

        public static void Info(
            Label lblMessage,
            string message)
        {
            if (lblMessage == null)
                return;

            lblMessage.Text =
                message;

            lblMessage.ForeColor =
                Color.Blue;

            lblMessage.Visible =
                true;
        }

        public static void Clear(
            Label lblMessage)
        {
            if (lblMessage == null)
                return;

            lblMessage.Text =
                "";

            lblMessage.Visible =
                false;
        }

        public static void SetMessage(
            Label lblMessage,
            string message,
            Color color)
        {
            if (lblMessage == null)
                return;

            lblMessage.Text =
                message;

            lblMessage.ForeColor =
                color;

            lblMessage.Visible =
                true;
        }
        public static void Exception(
        Label lblMessage,
        Exception ex)
        {
            Error(
                lblMessage,
                ex.Message);

            // Future:
            // ErrorLog table me save karenge.
        }
        public static void SqlException(
        Label lblMessage,
        Exception ex,
        string sql)
        {
            Error(
                lblMessage,
                Messages.DatabaseError);

            // Future:
            // SQL + Exception ErrorLog table me save hoga.
        }
    }
}