using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Training
{
    public class clsWorkflow
    {
        public static void UpdateWorkflow(
    string trainingID,
    string status,
    string stage)
        {
            string constr =
                ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(constr))
            {
                con.Open();

                SqlCommand cmd =
                    new SqlCommand(@"

SELECT WorkflowStatus

FROM TrainingDetails

WHERE TrainingID=@TrainingID

", con);

                cmd.Parameters.AddWithValue(
                    "@TrainingID",
                    trainingID);
                string workflow =
    Convert.ToString(cmd.ExecuteScalar());

                if (string.IsNullOrWhiteSpace(workflow))
                {
                    workflow = stage;
                }
                else if (!workflow.Contains(stage))
                {
                    workflow += stage;
                }
                
                if (!workflow.Contains(stage))
                {
                    workflow += stage;
                }

                string newWorkflow = workflow;
                SqlCommand upd =
                    new SqlCommand(@"

UPDATE TrainingDetails

SET

TrainingStatus=@Status,

WorkflowStatus=@Workflow

WHERE TrainingID=@TrainingID

", con);

                upd.Parameters.AddWithValue(
                    "@Status",
                    status);

                upd.Parameters.AddWithValue(
                    "@Workflow",
                    newWorkflow);

                upd.Parameters.AddWithValue(
                    "@TrainingID",
                    trainingID);

                upd.ExecuteNonQuery();
            }
        }

    }

}