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
                DataTable dt = new clsDataAccess().GetDataTable(@"SELECT S.SessionID,TR.TrainerID FROM SessionMaster S LEFT JOIN TrainerMaster TR ON TR.TrainerID=S.TrainerID WHERE S.TrainingID=@TrainingID", new SqlParameter[] { new SqlParameter("@TrainingID", trainingID) });
                Dictionary<string, string> trainerMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (DataRow row in dt.Rows) trainerMap[Convert.ToString(row["SessionID"])] = Convert.ToString(row["TrainerID"]);

                for (int i = 0; i < gvSession.Rows.Count; i++)
                {
                    string sessionID = Convert.ToString(gvSession.DataKeys[i].Value);
                    string trainerID;
                    if (!trainerMap.TryGetValue(sessionID, out trainerID)) continue;
                    if (gvSession.Rows[i].Cells.Count > 6)
                    {
                        gvSession.Rows[i].Cells[6].Controls.Clear();
                        gvSession.Rows[i].Cells[6].Controls.Add(new LiteralControl("<div style='font-weight:bold;color:#0d6efd;'>" + Server.HtmlEncode(trainerID) + "</div>"));
                    }
                }
            }
            base.OnPreRender(e);
        }
    }
}
