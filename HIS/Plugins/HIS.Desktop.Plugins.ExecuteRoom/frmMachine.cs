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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    public partial class frmMachine : Form
    {
        private int positionHandleControlLeft;
        List<long> serviceReqId { get; set; }
        List<long> sereServId { get; set; }
        Action<bool> Result { get; set; }
        public frmMachine(Action<bool> ResultMachine, List<long> serviceReqId, List<long> sereServId)
        {
            try
            {
                InitializeComponent();
                SetIconFrm();
                this.Result = ResultMachine;
                this.sereServId = sereServId;
                this.serviceReqId = serviceReqId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void frmMachine_Load(object sender, EventArgs e)
        {
            try
            {
                MachineValidationRule rule = new MachineValidationRule();
                rule.cbo = cboMachine;
                dxValidationProvider1.SetValidationRule(cboMachine,rule);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE","Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "Tên", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_CODE", "ID", columnInfos, true, 300);
                ControlEditorLoader.Load(cboMachine, BackendDataWorker.Get<HIS_MACHINE>().Where(o=>o.IS_ACTIVE == 1 && WorkPlace.GetRoomIds().Exists(p => ("," + o.ROOM_IDS + ",").Contains("," + p +","))).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate()) return;
                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                List<MOS.SDO.HisSereServUpdateMachineSDO> lstData = new List<HisSereServUpdateMachineSDO>();
                if (sereServId != null && sereServId.Count > 0)
                {
                    foreach (var item in sereServId)
                    {
                        HisSereServUpdateMachineSDO sdo = new HisSereServUpdateMachineSDO();
                        sdo.MachineId = Int64.Parse(cboMachine.EditValue.ToString());
                        sdo.ServiceReqID = serviceReqId[0];
                        sdo.SereServID = item;
                        lstData.Add(sdo);
                    }
                }
                else
                {
                    foreach (var item in serviceReqId)
                    {
                        lstData.Add(new HisSereServUpdateMachineSDO() { MachineId = Int64.Parse(cboMachine.EditValue.ToString()), ServiceReqID = item });
                    }
                }
                success = new BackendAdapter(param)
                    .Post<bool>("api/HisSereServ/UpdateMachine", ApiConsumers.MosConsumer, lstData, param);
                WaitingManager.Hide();
                if (success)
                {
                    Result(true);
                    this.Close();
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
