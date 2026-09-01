using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Trainer
{
    public partial class TestMaster : System.Web.UI.Page
    {
        clsDataAccess obj =
            new clsDataAccess();

        private string TrainerID
        {
            get
            {
                return
                    Session["TrainerID"].ToString();
            }
        }

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if
            (
                Session["TrainerID"] == null
            )
            {
                Response.Redirect(
                    "~/TrainerLogin.aspx");

                return;
            }

            if
            (
                !IsPostBack
            )
            {
                BindTraining();

                //  BindSession();

                //  BindTopic();

                // BindGrid();

                txtDuration.Text =
                    "30";

                txtTotalQuestion.Text =
                    "20";

                txtTotalMarks.Text =
                    "20";

                txtPassingPercentage.Text =
                    "40";

                txtPassingMarks.Text =
                    "8";

                chkRandomQuestion.Checked =
                    true;

                chkShuffleOption.Checked =
                    true;

                chkPublished.Checked =
                    false;

                chkShowResult.Checked =
                    true;
            }
        }

        private void BindTraining()
        {
            string query =
                @"SELECT
                    TD.TrainingID,
                    TD.TrainingTitle
                  FROM
                    TrainingDetails TD
                  INNER JOIN
                    TrainerTraining TT
                  ON
                    TD.TrainingID = TT.TrainingID
                  WHERE
                    TT.TrainerID = @TrainerID
                  AND
                    TD.TrainingStatus = 'Active'
                  ORDER BY
                    TD.TrainingTitle";

            SqlParameter[] parameter =
            {
                new SqlParameter(
                    "@TrainerID",
                    TrainerID)
            };

            DataTable dt =
                obj.GetDataTable(
                query,
                parameter);

            ddlTraining.DataSource =
                dt;

            ddlTraining.DataTextField =
                "TrainingTitle";

            ddlTraining.DataValueField =
                "TrainingID";

            ddlTraining.DataBind();

            ddlTraining.Items.Insert(
                0,
                new ListItem(
                    "-- Select Training --",
                    ""));
        }

        protected void ddlTraining_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            //BindSession();

            // BindTopic();
        }
    }
}