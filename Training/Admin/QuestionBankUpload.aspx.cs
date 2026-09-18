using OfficeOpenXml;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class QuestionBankUpload : System.Web.UI.Page
    {
        clsDataAccess objData =
            new clsDataAccess();

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            //if
            //(
            //    Session["Corresponding_id"] == null
            //)
            //{
            //    Response.Redirect(
            //        "~/Default.aspx");

            //    return;
            //}

            if
            (
                !IsPostBack
            )
            {
                lblMessage.Text =
                    "";

                gvResult.DataSource =
                    null;

                gvResult.DataBind();
            }
        }

        protected void btnDownloadSample_Click(
            object sender,
            EventArgs e)
        {
            string filePath =
                Server.MapPath(
                "~/SampleFormat/QuestionBankSample.xlsx");

            if
            (
                !File.Exists(
                    filePath)
            )
            {
                lblMessage.ForeColor =
                    Color.Red;

                lblMessage.Text =
                    "Sample file not found.";

                return;
            }

            Response.Clear();

            Response.ContentType =
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            Response.AddHeader(
                "content-disposition",
                "attachment; filename=QuestionBankSample.xlsx");

            Response.TransmitFile(
                filePath);

            Response.Flush();

            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        protected void btnUpload_Click(
            object sender,
            EventArgs e)
        {
            lblMessage.Text =
                "";

            lblMessage.ForeColor =
                Color.Red;

            if
            (
                !fuExcel.HasFile
            )
            {
                lblMessage.Text =
                    "Please select an Excel file.";

                return;
            }

            string extension =
                Path.GetExtension(
                fuExcel.FileName)
                .ToLower();

            if
            (
                extension != ".xlsx"
            )
            {
                lblMessage.Text =
                    "Only .xlsx file is allowed.";

                return;
            }

            if
            (
                fuExcel.PostedFile.ContentLength
                >
                10
                *
                1024
                *
                1024
            )
            {
                lblMessage.Text =
                    "Maximum file size allowed is 10 MB.";

                return;
            }

            try
            {
                ProcessExcel();
            }
            catch
            (
                Exception ex
            )
            {
                lblMessage.ForeColor =
                    Color.Red;

                lblMessage.Text =
                    ex.Message;
            }
        }
        private void ProcessExcel()
        {
            ExcelPackage.LicenseContext =
                LicenseContext.NonCommercial;

            DataTable dtResult =
                CreateResultTable();

            using
            (
                ExcelPackage package =
                new ExcelPackage(
                    fuExcel.PostedFile.InputStream)
            )
            {
                ExcelWorksheet worksheet =
                    package.Workbook.Worksheets[0];

                if
                (
                    worksheet == null
                )
                {
                    throw new Exception(
                        "Worksheet not found.");
                }

                if
                (
                    worksheet.Dimension == null
                )
                {
                    throw new Exception(
                        "Excel file is empty.");
                }

                ValidateHeader(
                    worksheet);

                int totalRows =
                    worksheet.Dimension.End.Row;

                for
                (
                    int row = 2;
                    row <= totalRows;
                    row++
                )
                {
                    string course =
                        GetCellValue(
                            worksheet,
                            row,
                            1);

                    string topic =
                        GetCellValue(
                            worksheet,
                            row,
                            2);

                    string question =
                        GetCellValue(
                            worksheet,
                            row,
                            3);

                    string optionA =
                        GetCellValue(
                            worksheet,
                            row,
                            4);

                    string optionB =
                        GetCellValue(
                            worksheet,
                            row,
                            5);

                    string optionC =
                        GetCellValue(
                            worksheet,
                            row,
                            6);

                    string optionD =
                        GetCellValue(
                            worksheet,
                            row,
                            7);

                    string correctOption =
                        GetCellValue(
                            worksheet,
                            row,
                            8);

                    string difficulty =
                        GetCellValue(
                            worksheet,
                            row,
                            9);

                    string marks =
                        GetCellValue(
                            worksheet,
                            row,
                            10);

                    string explanation =
                        GetCellValue(
                            worksheet,
                            row,
                            11);

                    string questionType =
                        GetCellValue(
                            worksheet,
                            row,
                            12);

                    string negativeMarks =
                        GetCellValue(
                            worksheet,
                            row,
                            13);

                    string language =
                        GetCellValue(
                            worksheet,
                            row,
                            14);

                    SaveExcelRow(
                        dtResult,
                        row,
                        course,
                        topic,
                        question,
                        optionA,
                        optionB,
                        optionC,
                        optionD,
                        correctOption,
                        difficulty,
                        marks,
                        explanation,
                        questionType,
                        negativeMarks,
                        language);
                }
            }

            gvResult.DataSource =
                dtResult;

            gvResult.DataBind();

            lblMessage.ForeColor =
                Color.Green;

            lblMessage.Text =
                "Upload completed.";
        }

        private void ValidateHeader(
            ExcelWorksheet worksheet)
        {
            if
            (
                worksheet.Cells[1, 1].Text.Trim() != "Course"
                ||
                worksheet.Cells[1, 2].Text.Trim() != "Topic"
                ||
                worksheet.Cells[1, 3].Text.Trim() != "Question"
                ||
                worksheet.Cells[1, 4].Text.Trim() != "OptionA"
                ||
                worksheet.Cells[1, 5].Text.Trim() != "OptionB"
                ||
                worksheet.Cells[1, 6].Text.Trim() != "OptionC"
                ||
                worksheet.Cells[1, 7].Text.Trim() != "OptionD"
                ||
                worksheet.Cells[1, 8].Text.Trim() != "CorrectOption"
                ||
                worksheet.Cells[1, 9].Text.Trim() != "Difficulty"
                ||
                worksheet.Cells[1, 10].Text.Trim() != "Marks"
                ||
                worksheet.Cells[1, 11].Text.Trim() != "Explanation"
                ||
                worksheet.Cells[1, 12].Text.Trim() != "QuestionType"
                ||
                worksheet.Cells[1, 13].Text.Trim() != "NegativeMarks"
                ||
                worksheet.Cells[1, 14].Text.Trim() != "Language"
            )
            {
                throw new Exception(
                    "Invalid Excel format. Please download the latest sample file.");
            }
        }

        private string GetCellValue(
            ExcelWorksheet worksheet,
            int row,
            int column)
        {
            if
            (
                worksheet.Cells[row, column].Value
                ==
                null
            )
            {
                return
                    "";
            }

            return
                worksheet.Cells[row, column]
                .Text
                .Trim();
        }

        private DataTable CreateResultTable()
        {
            DataTable dt =
                new DataTable();

            dt.Columns.Add(
                "RowNo");

            dt.Columns.Add(
                "Course");

            dt.Columns.Add(
                "Topic");

            dt.Columns.Add(
                "Question");

            dt.Columns.Add(
                "Status");

            dt.Columns.Add(
                "Message");

            return
                dt;
        }
        private void SaveExcelRow(
    DataTable dtResult,
    int rowNo,
    string course,
    string topic,
    string question,
    string optionA,
    string optionB,
    string optionC,
    string optionD,
    string correctOption,
    string difficulty,
    string marks,
    string explanation,
    string questionType,
    string negativeMarks,
    string language)
        {
            string message =
                ValidateRow(
                course,
                topic,
                question,
                optionA,
                optionB,
                optionC,
                optionD,
                correctOption,
                difficulty,
                marks,
                questionType,
                negativeMarks,
                language);

            DataRow dr =
                dtResult.NewRow();

            dr["RowNo"] =
                rowNo;

            dr["Course"] =
                course;

            dr["Topic"] =
                topic;

            dr["Question"] =
                question;

            if
            (
                message == ""
            )
            {
                BulkInsertQuestion(
                    course,
                    topic,
                    question,
                    optionA,
                    optionB,
                    optionC,
                    optionD,
                    correctOption,
                    difficulty,
                    marks,
                    explanation,
                    questionType,
                    negativeMarks,
                    language);

                dr["Status"] =
                    "Success";

                dr["Message"] =
                    "Inserted Successfully";
            }
            else
            {
                dr["Status"] =
                    "Failed";

                dr["Message"] =
                    message;
            }

            dtResult.Rows.Add(
                dr);
        }

        private string ValidateRow(
            string course,
            string topic,
            string question,
            string optionA,
            string optionB,
            string optionC,
            string optionD,
            string correctOption,
            string difficulty,
            string marks,
            string questionType,
            string negativeMarks,
            string language)
        {
            if
            (
                course == ""
            )
            {
                return
                    "Course is required.";
            }

            if
            (
                topic == ""
            )
            {
                return
                    "Topic is required.";
            }

            if
            (
                question == ""
            )
            {
                return
                    "Question is required.";
            }

            if
            (
                optionA == ""
                ||
                optionB == ""
                ||
                optionC == ""
                ||
                optionD == ""
            )
            {
                return
                    "All options are required.";
            }

            if
            (
                correctOption != "A"
                &&
                correctOption != "B"
                &&
                correctOption != "C"
                &&
                correctOption != "D"
            )
            {
                return
                    "Correct Option should be A/B/C/D.";
            }

            if
            (
                difficulty != "Easy"
                &&
                difficulty != "Medium"
                &&
                difficulty != "Hard"
            )
            {
                return
                    "Invalid Difficulty.";
            }

            decimal decimalMarks;

            if
            (
                !Decimal.TryParse(
                    marks,
                    out decimalMarks)
            )
            {
                return
                    "Invalid Marks.";
            }

            decimal decimalNegativeMarks;

            if
            (
                !Decimal.TryParse(
                    negativeMarks,
                    out decimalNegativeMarks)
            )
            {
                return
                    "Invalid Negative Marks.";
            }

            if
            (
                questionType == ""
            )
            {
                return
                    "Question Type is required.";
            }

            if
            (
                language == ""
            )
            {
                return
                    "Language is required.";
            }

            if
            (
                !CourseExists(
                    course)
            )
            {
                return
                    "Course not found.";
            }

            if
            (
                !TopicExists(
                    course,
                    topic)
            )
            {
                return
                    "Topic not found for selected Course.";
            }

            if
            (
                IsDuplicateQuestion(
                    course,
                    topic,
                    question)
            )
            {
                return
                    "Duplicate Question.";
            }

            return
                "";
        }

        private bool CourseExists(
            string courseName)
        {
            string sql =
                "SELECT COUNT(*) FROM CourseMaster WHERE CourseName=@CourseName";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseName",
            courseName)
    };

            object result =
                objData.ExecuteScalar(
                sql,
                parameter);

            return
                Convert.ToInt32(
                result)
                >
                0;
        }

        private bool TopicExists(
            string courseName,
            string topicName)
        {
            string sql =
                "SELECT COUNT(*) FROM TrainingDetails TD INNER JOIN SessionMaster SM ON TD.TrainingID=SM.TrainingID INNER JOIN TopicMaster TM ON SM.TopicID=TM.TopicID INNER JOIN CourseMaster CM ON TD.CourseID=CM.CourseID WHERE CM.CourseName=@CourseName AND TM.TopicName=@TopicName";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseName",
            courseName),

        new SqlParameter(
            "@TopicName",
            topicName)
    };

            object result =
                objData.ExecuteScalar(
                sql,
                parameter);

            return
                Convert.ToInt32(
                result)
                >
                0;
        }

        private bool IsDuplicateQuestion(
            string course,
            string topic,
            string question)
        {
            string sql =
                "SELECT COUNT(*) FROM QuestionBank QB INNER JOIN CourseMaster CM ON QB.CourseID=CM.CourseID INNER JOIN TopicMaster TM ON QB.TopicID=TM.TopicID WHERE CM.CourseName=@CourseName AND TM.TopicName=@TopicName AND QB.Question=@Question AND QB.QuestionOwnerType='Admin'";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseName",
            course),

        new SqlParameter(
            "@TopicName",
            topic),

        new SqlParameter(
            "@Question",
            question)
    };

            object result =
                objData.ExecuteScalar(
                sql,
                parameter);

            return
                Convert.ToInt32(
                result)
                >
                0;
        }
        private string GetCourseID(
    string courseName)
        {
            string sql =
                "SELECT CourseID FROM CourseMaster WHERE CourseName=@CourseName";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseName",
            courseName)
    };

            object result =
                objData.ExecuteScalar(
                sql,
                parameter);

            if
            (
                result == null
            )
            {
                return
                    "";
            }

            return
                result.ToString();
        }
        private string GetTopicID(
    string courseID,
    string topicName)
        {
            string sql =
                "SELECT TOP 1 TM.TopicID " +
                "FROM TrainingDetails TD " +
                "INNER JOIN SessionMaster SM " +
                "ON TD.TrainingID=SM.TrainingID " +
                "INNER JOIN TopicMaster TM " +
                "ON SM.TopicID=TM.TopicID " +
                "WHERE TD.CourseID=@CourseID " +
                "AND TM.TopicName=@TopicName";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@CourseID",
            courseID),

        new SqlParameter(
            "@TopicName",
            topicName)
    };

            object result =
                objData.ExecuteScalar(
                sql,
                parameter);

            if
            (
                result == null
            )
            {
                return
                    "";
            }

            return
                result.ToString();
        }
        private string GenerateQuestionID()
        {
            string sql =
                "SELECT ISNULL(MAX(CAST(RIGHT(QuestionID,5) AS INT)),0)+1 FROM QuestionBank";

            object result =
                objData.ExecuteScalar(
                sql,
                null);

            int id =
                Convert.ToInt32(
                result);

            return
                "QST"
                +
                id.ToString(
                "00000");
        }
        private void BulkInsertQuestion(
    string course,
    string topic,
    string question,
    string optionA,
    string optionB,
    string optionC,
    string optionD,
    string correctOption,
    string difficulty,
    string marks,
    string explanation,
    string questionType,
    string negativeMarks,
    string language)
        {
            string courseID =
                GetCourseID(
                course);

            string topicID =
                GetTopicID(
                courseID,
                topic);

            string sql =
                "INSERT INTO QuestionBank(" +
                "QuestionID," +
                "QuestionOwnerType," +
                "OwnerID," +
                "CourseID," +
                "TopicID," +
                "Question," +
                "OptionA," +
                "OptionB," +
                "OptionC," +
                "OptionD," +
                "CorrectOption," +
                "DifficultyLevel," +
                "Marks," +
                "Explanation," +
                "IsActive," +
                "CreatedOn," +
                "CreatedBy," +
                "QuestionType," +
                "NegativeMarks," +
                "Language" +
                ")" +
                " VALUES(" +
                "@QuestionID," +
                "'Admin'," +
                "'ADMIN'," +
                "@CourseID," +
                "@TopicID," +
                "@Question," +
                "@OptionA," +
                "@OptionB," +
                "@OptionC," +
                "@OptionD," +
                "@CorrectOption," +
                "@DifficultyLevel," +
                "@Marks," +
                "@Explanation," +
                "1," +
                "GETDATE()," +
                "@CreatedBy," +
                "@QuestionType," +
                "@NegativeMarks," +
                "@Language" +
                ")";

            SqlParameter[] parameter =
            {
        new SqlParameter(
            "@QuestionID",
            GenerateQuestionID()),

        new SqlParameter(
            "@CourseID",
            courseID),

        new SqlParameter(
            "@TopicID",
            topicID),

        new SqlParameter(
            "@Question",
            question),

        new SqlParameter(
            "@OptionA",
            optionA),

        new SqlParameter(
            "@OptionB",
            optionB),

        new SqlParameter(
            "@OptionC",
            optionC),

        new SqlParameter(
            "@OptionD",
            optionD),

        new SqlParameter(
            "@CorrectOption",
            correctOption),

        new SqlParameter(
            "@DifficultyLevel",
            difficulty),

        new SqlParameter(
            "@Marks",
            Convert.ToDecimal(
            marks)),

        new SqlParameter(
            "@Explanation",
            explanation),

        new SqlParameter(
            "@CreatedBy",
            Session["UserID"]?.ToString()),

        new SqlParameter(
            "@QuestionType",
            questionType),

        new SqlParameter(
            "@NegativeMarks",
            Convert.ToDecimal(
            negativeMarks)),

        new SqlParameter(
            "@Language",
            language)
    };

            objData.ExecuteSql(
                sql,
                parameter);
        }
    }
}