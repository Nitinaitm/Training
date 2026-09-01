
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class BulkExcelUpload : System.Web.UI.Page
    {
        string constr = ConfigurationManager
                         .ConnectionStrings["constr"]
                         .ConnectionString;

        Dictionary<string, List<string>> tableFields =
            new Dictionary<string, List<string>>()
        {
            {
                "EmpBasicMaster",
                new List<string>
                {
                    "MobileNo",
                    "EmailId",
                    "EmpCompany",
                    "EmpDesignation",
                    "EmpPostingPlace"
                }
            },

            {
                "TrainingDetails",
                new List<string>
                {
                    "TrainingType",
                    "Batch",
                    "Attendance",
                    "DateFrom",
                    "DateTo",
                    "LocationOfInduction",
                    "EmpOverallFeedback"
                }
            },

            {
                "FeedbackReport",
                new List<string>
                {
                    "Topic",
                    "Report"
                }
            },

            {
                "FeedbackTrainingRelated",
                new List<string>
                {
                    "TrainingRelatedAspects",
                    "OrganizedBy",
                    "Remarks",
                    "Grading"
                }
            }
        };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (
                Session["InternalRedirect_Admin"] == null)
                {
                    Response.Redirect(
                    "~/Default.aspx");
                }
                
            }
        }

        protected void ddlTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkFields.Items.Clear();

            if (ddlTable.SelectedValue != "")
            {
                List<string> fields =
                    tableFields[ddlTable.SelectedValue];

                foreach (string field in fields)
                {
                    chkFields.Items.Add(field);
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (!fuExcel.HasFile)
            {
                lblSummary.Text = "Please upload excel file.";
                return;
            }

            if (ddlTable.SelectedValue == "")
            {
                lblSummary.Text = "Please select table.";
                return;
            }

            List<string> selectedFields =
                chkFields.Items.Cast<ListItem>()
                .Where(x => x.Selected)
                .Select(x => x.Value)
                .ToList();

            if (selectedFields.Count == 0)
            {
                lblSummary.Text = "Please select fields.";
                return;
            }

            DataTable dtExcel = ReadExcel();

            if (!dtExcel.Columns.Contains("EmpID"))
            {
                lblSummary.Text = "EmpID column mandatory in excel.";
                return;
            }

            gvPreview.DataSource = dtExcel;
            gvPreview.DataBind();

            int inserted = 0;
            int updated = 0;
            int skipped = 0;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                foreach (DataRow row in dtExcel.Rows)
                {
                    string empid = row["EmpID"].ToString().Trim().ToUpperInvariant();

                    if (empid == "")
                    {
                        skipped++;
                        continue;
                    }

                    bool exists = false;

                    if (ddlTable.SelectedValue == "EmpBasicMaster")
                    {
                        SqlCommand checkCmd = new SqlCommand(
                            "SELECT COUNT(*) FROM EmpBasicMaster WHERE EmpID=@EmpID",
                            con);

                        checkCmd.Parameters.AddWithValue("@EmpID", empid);

                        exists = Convert.ToInt32(checkCmd.ExecuteScalar()) > 0;
                    }

                    // ==========================
                    // UPDATE
                    // ==========================

                    if (rblOperation.SelectedValue == "Update")
                    {
                        if (ddlTable.SelectedValue == "EmpBasicMaster" && !exists)
                        {
                            skipped++;
                            continue;
                        }

                        string updateQuery = "UPDATE " + ddlTable.SelectedValue +
                                             " SET ";

                        foreach (string field in selectedFields)
                        {
                            updateQuery += field + "=@" + field + ",";
                        }

                        updateQuery = updateQuery.TrimEnd(',');

                        updateQuery += " WHERE EmpID=@EmpID";

                        SqlCommand updateCmd =
                            new SqlCommand(updateQuery, con);

                        updateCmd.Parameters.AddWithValue("@EmpID", empid);

                        foreach (string field in selectedFields)
                        {
                            object value = DBNull.Value;

                            if (dtExcel.Columns.Contains(field))
                            {
                                value = row[field].ToString();
                            }

                            updateCmd.Parameters.AddWithValue(
                                "@" + field,
                                value);
                        }

                        updateCmd.ExecuteNonQuery();

                        updated++;
                    }

                    // ==========================
                    // INSERT
                    // ==========================

                    else
                    {
                        if (ddlTable.SelectedValue == "EmpBasicMaster" && exists)
                        {
                            skipped++;
                            continue;
                        }

                        string insertColumns = "EmpID,";
                        string insertValues = "@EmpID,";

                        foreach (string field in selectedFields)
                        {
                            insertColumns += field + ",";
                            insertValues += "@" + field + ",";
                        }

                        insertColumns = insertColumns.TrimEnd(',');
                        insertValues = insertValues.TrimEnd(',');

                        string insertQuery =
                            "INSERT INTO " + ddlTable.SelectedValue +
                            " (" + insertColumns + ") VALUES (" + insertValues + ")";

                        SqlCommand insertCmd =
                            new SqlCommand(insertQuery, con);

                        insertCmd.Parameters.AddWithValue("@EmpID", empid);

                        foreach (string field in selectedFields)
                        {
                            object value = DBNull.Value;

                            if (dtExcel.Columns.Contains(field))
                            {
                                value = row[field].ToString();
                            }

                            insertCmd.Parameters.AddWithValue(
                                "@" + field,
                                value);
                        }

                        insertCmd.ExecuteNonQuery();

                        inserted++;
                    }
                }

                con.Close();
            }

            lblSummary.Text =
                "Total Rows : " + dtExcel.Rows.Count +
                "<br/>Inserted : " + inserted +
                "<br/>Updated : " + updated +
                "<br/>Skipped : " + skipped;
        }

        private DataTable ReadExcel()
        {
            DataTable dt = new DataTable();

            using (XLWorkbook workbook =
                new XLWorkbook(fuExcel.FileContent))
            {
                IXLWorksheet worksheet =
                    workbook.Worksheet(1);

                bool firstRow = true;

                foreach (IXLRow row in worksheet.RowsUsed())
                {
                    if (firstRow)
                    {
                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Columns.Add(cell.Value.ToString());
                        }

                        firstRow = false;
                    }
                    else
                    {
                        dt.Rows.Add();

                        int i = 0;

                        foreach (IXLCell cell in row.Cells())
                        {
                            dt.Rows[dt.Rows.Count - 1][i] =
                                cell.Value.ToString();

                            i++;
                        }
                    }
                }
            }

            return dt;
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            ddlTable.SelectedIndex = 0;

            chkFields.Items.Clear();

            gvPreview.DataSource = null;
            gvPreview.DataBind();

            lblSummary.Text = "";
        }
    }
}
