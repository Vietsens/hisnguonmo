using HIS.Desktop.Plugins.Optometrist.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.DAL;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Optometrist.UC
{
    public partial class UCOptometrist : UserControlBase
    {
        public void OptometristEndReq()
        {
            btnEndReq_Click(null, null);
        }
        private void btnEndReq_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEndReq.Enabled) return;
                if (currentSR == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("currentSR is null");
                    return;
                }

                bool success = false;
                CommonParam param = new CommonParam();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>("api/HisServiceReq/Finish", ApiConsumer.ApiConsumers.MosConsumer, currentSR.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (result != null)
                {
                    success = true;
                    btnEndReq.Enabled = false;
                    btnSave.Enabled = false;
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}