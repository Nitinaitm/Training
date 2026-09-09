using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Training.Admin
{
    public partial class AssignSession
    {
        protected override void OnPreRender(EventArgs e)
        {
            string trainingID = Convert.ToString(Session["TrainingID"]);
            if (!string.IsNullOrWhiteSpace(trainingID) && gvSession != null)
            {
                DataTable dt = new clsDataAccess().GetDataTable(@"SELECT S.SessionID,TR.TrainerID,ISNULL(TR.EmpID,'') EmpID FROM SessionMaster S LEFT JOIN TrainerMaster TR ON TR.TrainerID=S.TrainerID WHERE S.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
                Dictionary<string, Tuple<string,string>> trainerMap = new Dictionary<string, Tuple<string,string>>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow row in dt.Rows)
                    trainerMap[Convert.ToString(row["SessionID"])] = Tuple.Create(Convert.ToString(row["TrainerID"]), Convert.ToString(row["EmpID"]));

                for (int i = 0; i < gvSession.Rows.Count; i++)
                {
                    string sessionID = Convert.ToString(gvSession.DataKeys[i].Value);
                    Tuple<string,string> trainer;
                    if (!trainerMap.TryGetValue(sessionID, out trainer)) continue;
                    if (gvSession.Rows[i].Cells.Count <= 6) continue;

                    foreach (Control control in gvSession.Rows[i].Cells[6].Controls)
                    {
                        LiteralControl literal = control as LiteralControl;
                        if (literal == null) continue;
                        if (!string.IsNullOrWhiteSpace(trainer.Item2)) literal.Text = literal.Text.Replace(trainer.Item2, trainer.Item1);
                    }
                }
            }
            base.OnPreRender(e);
        }
    }
}
