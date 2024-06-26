/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class frmMessage : Form
    {
        ADO.ServiceADO clickServiceADO { get; set; }
        Action<HIS_SERE_SERV> IsSuccess { get; set; }
        Inventec.Desktop.Common.Modules.Module moduleData { get; set; }
        public frmMessage(ADO.ServiceADO clickServiceADO, Action<HIS_SERE_SERV> IsSuccess, Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                this.clickServiceADO = clickServiceADO;
                this.IsSuccess = IsSuccess;
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(memMessage.Text.Trim()))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bắt buộc nhập lý do", "Thông báo", MessageBoxButtons.OK);
                    memMessage.Focus();
                    return;
                }
                if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(memMessage.Text.Trim(), 200))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ được phép nhập tối đa 200 kí tự", "Thông báo", MessageBoxButtons.OK);
                    memMessage.Focus();
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                if (clickServiceADO == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("clickServiceADO is null");
                    return;
                }
                HisSereServConfirmNoExcuteSDO sdo = new HisSereServConfirmNoExcuteSDO();
                sdo.SereServId = clickServiceADO.ID;
                sdo.WorkingRoomId = moduleData.RoomId;
                sdo.ConfirmNoExcuteReason = memMessage.Text.Trim();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERE_SERV>("api/HisSereServ/ConfirmNoExcute", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                IsSuccess(result);
                if (result != null)
                {
                    success = true;
                    this.Close();
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

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmMessage_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
