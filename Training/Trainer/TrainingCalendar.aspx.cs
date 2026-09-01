using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class TrainingCalendar : System.Web.UI.Page
    {
        clsDataAccess obj = new clsDataAccess();
        private DateTime currentDate = DateTime.Now;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["TrainerID"] == null) Response.Redirect("~/TrainerLogin.aspx");
            if (!IsPostBack) { currentDate = DateTime.Now; GenerateCalendar(); }
        }

        private string TrainerID => Session["TrainerID"].ToString();

        protected void lnkPrev_Click(object sender, EventArgs e)
        { currentDate = currentDate.AddMonths(-1); GenerateCalendar(); }

        protected void lnkNext_Click(object sender, EventArgs e)
        { currentDate = currentDate.AddMonths(1); GenerateCalendar(); }

        protected void btnToday_Click(object sender, EventArgs e)
        { currentDate = DateTime.Now; GenerateCalendar(); }

        private void GenerateCalendar()
        {
            lblMonthYear.Text = currentDate.ToString("MMMM yyyy");
            tblCalendar.Rows.Clear();

            // Headers
            TableRow headerRow = new TableRow();
            string[] days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            foreach (string day in days)
            {
                TableCell cell = new TableCell();
                cell.Text = day;
                cell.HorizontalAlign = HorizontalAlign.Center;
                cell.Font.Bold = true;
                headerRow.Cells.Add(cell);
            }
            tblCalendar.Rows.Add(headerRow);

            // Get sessions for the month
            string query = @"SELECT SessionDate, SessionName, AttendanceStatus FROM SessionMaster WHERE TrainerID=@TrainerID AND TRY_CONVERT(date,SessionDate,105) >= @StartDate AND TRY_CONVERT(date,SessionDate,105) <= @EndDate";
            SqlParameter[] param = new SqlParameter[] {
                new SqlParameter("@TrainerID", TrainerID),
                new SqlParameter("@StartDate", new DateTime(currentDate.Year, currentDate.Month, 1)),
                new SqlParameter("@EndDate", new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)))
            };
            DataTable dt = obj.GetDataTable(query, param);

            DateTime firstDay = new DateTime(currentDate.Year, currentDate.Month, 1);
            int startDay = (int)firstDay.DayOfWeek;
            int daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            TableRow row = new TableRow();
            for (int i = 0; i < startDay; i++)
            {
                TableCell cell = new TableCell();
                row.Cells.Add(cell);
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                TableCell cell = new TableCell();
                DateTime currentDay = new DateTime(currentDate.Year, currentDate.Month, day);

                // Day number
                cell.Text = $"<div class='day-number'>{day}</div>";

                // Check if today
                if (currentDay.Date == DateTime.Now.Date)
                    cell.CssClass = "today";

                // Add sessions for this day
                string dayStr = currentDay.ToString("dd-MM-yyyy");
                DataRow[] rows = dt.Select($"SessionDate = '{dayStr}'");
                foreach (DataRow dr in rows)
                {
                    string status = dr["AttendanceStatus"].ToString();
                    string bgColor = status == "Completed" ? "bg-success" : status == "Pending" ? "bg-warning" : "bg-info";
                    cell.Text += $"<div class='event {bgColor}'>{dr["SessionName"]}</div>";
                }

                row.Cells.Add(cell);

                if ((startDay + day) % 7 == 0 || day == daysInMonth)
                {
                    tblCalendar.Rows.Add(row);
                    row = new TableRow();
                }
            }
        }
    }
}