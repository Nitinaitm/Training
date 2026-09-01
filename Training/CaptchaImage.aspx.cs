using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Training
{
    public partial class CaptchaImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string captchaText = GenerateCaptchaText(5);
            Session["CaptchaCode"] = captchaText; // Store in session for validation

            // Create an image with a white background
            Bitmap bmp = new Bitmap(200, 120);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            // Set font and brush
            Font font = new Font("Arial", 28, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);

            // Random rotation angle (-20 to 20 degrees)
            Random rand = new Random();
            int angle = rand.Next(-20, 20);

            // Translate graphics to center for proper rotation
            g.TranslateTransform(50, 30);
            g.RotateTransform(angle);
            g.DrawString(captchaText, font, brush, new PointF(0, 0));
            g.RotateTransform(-angle); // Reset rotation
            g.TranslateTransform(-50, -30);

            // Add noise (random dots)
            for (int i = 0; i < 50; i++)
            {
                int x = rand.Next(bmp.Width);
                int y = rand.Next(bmp.Height);
                bmp.SetPixel(x, y, Color.Gray);
            }

            // Save and send image response
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            Response.Clear();
            Response.ContentType = "image/png";
            Response.BinaryWrite(ms.ToArray());

            // Clean up
            g.Dispose();
            bmp.Dispose();
        }

        private string GenerateCaptchaText(int length)
        {
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            Random rand = new Random();
            char[] captcha = new char[length];

            for (int i = 0; i < length; i++)
            {
                captcha[i] = chars[rand.Next(chars.Length)];
            }

            return new string(captcha);
        }
    }
}