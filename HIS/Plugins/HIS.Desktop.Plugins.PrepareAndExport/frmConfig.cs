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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.PrepareAndExport.Run;
using HIS.Desktop.Plugins.PrepareAndExport.Validate;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PrepareAndExport
{
	public partial class frmConfig : FormBase
	{
		private int positionHandle;
        Action<bool> isOpen;
        Action<string> gateCode;
        Action<string> Ip;
        public frmConfig(Action<bool> isOpen, Action<string> gateCode, Action<string> Ip)
		{
			InitializeComponent();
			try
			{
                this.gateCode = gateCode;
                this.Ip = Ip;
                this.isOpen = isOpen;
				string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
				this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
                bool success = false;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate1 = (frmPrepareAndExport.currentControlStateRDO != null && frmPrepareAndExport.currentControlStateRDO.Count > 0) ? frmPrepareAndExport.currentControlStateRDO.Where(o => o.KEY == "AddressIPCPA" && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExport").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate1), csAddOrUpdate1));
                if (csAddOrUpdate1 != null)
                {
                    csAddOrUpdate1.VALUE = txtIP.Text.Trim();
                }
                else
                {
                    csAddOrUpdate1 = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate1.KEY = "AddressIPCPA";
                    csAddOrUpdate1.VALUE = txtIP.Text.Trim();
                    csAddOrUpdate1.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExport";
                    if (frmPrepareAndExport.currentControlStateRDO == null)
                        frmPrepareAndExport.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    frmPrepareAndExport.currentControlStateRDO.Add(csAddOrUpdate1);
                }
                frmPrepareAndExport.controlStateWorker.SetData(frmPrepareAndExport.currentControlStateRDO);
                WaitingManager.Hide();


                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (frmPrepareAndExport.currentControlStateRDO != null && frmPrepareAndExport.currentControlStateRDO.Count > 0) ? frmPrepareAndExport.currentControlStateRDO.Where(o => o.KEY == "txtGateCodeString" && o.MODULE_LINK == "HIS.Desktop.Plugins.PrepareAndExport").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtGate.Text.Trim();
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "txtGateCodeString";
                    csAddOrUpdate.VALUE = txtGate.Text.Trim();
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.PrepareAndExport";
                    if (frmPrepareAndExport.currentControlStateRDO == null)
                        frmPrepareAndExport.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    frmPrepareAndExport.currentControlStateRDO.Add(csAddOrUpdate);
                }
                frmPrepareAndExport.controlStateWorker.SetData(frmPrepareAndExport.currentControlStateRDO);
                WaitingManager.Hide();
                isOpen(true);
                gateCode(txtGate.Text.Trim());
                Ip(txtIP.Text.Trim());
                this.Close();
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
          
        }

        private void SetValidateText1()
        {
            try
            {
                ValidNull valid = new ValidNull();
                valid.txt = txtGate;
                dxValidationProvider1.SetValidationRule(txtGate, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidateText2()
        {
            try
            {
                ValidNull valid = new ValidNull();
                valid.txt = txtIP;
                dxValidationProvider1.SetValidationRule(txtIP, valid);
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

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

		private void frmConfig_Load(object sender, EventArgs e)
		{
			try
			{
                SetValidateText1();
                SetValidateText2();
                txtGate.Text = frmPrepareAndExport.txtGateCodeString;
                txtIP.Text= frmPrepareAndExport.txtIpCPA;
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
	}
}
