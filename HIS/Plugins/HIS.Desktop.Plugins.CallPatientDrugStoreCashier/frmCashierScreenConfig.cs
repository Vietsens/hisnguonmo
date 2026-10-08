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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.CallPatientDrugStoreCashier.ADO;
using HIS.Desktop.Plugins.CallPatientDrugStoreCashier.Base;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreCashier
{
    public partial class frmCashierScreenConfig : FormBase
    {
        internal const string MODULE_LINK = "HIS.Desktop.Plugins.CallPatientDrugStoreCashier";
        const string CONTROL_STATE_KEY = "CashierScreenConfigADO";

        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_MEDI_STOCK mediStock;
        CashierScreenConfigADO currentConfig;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        public frmCashierScreenConfig(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void frmCashierScreenConfig_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId);
                }
                if (this.mediStock != null)
                {
                    lblMediStock.Text = (this.mediStock.MEDI_STOCK_NAME + " (" + this.mediStock.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lblMediStock.Text = Resources.ResourceLanguageManager.GetValue("frmCashierScreenConfig.lblMediStock.NotMediStock");
                    lblMediStock.Appearance.ForeColor = Color.Red;
                    btnOpen.Enabled = false;
                }

                LoadConfig();
                SetConfigToControl(this.currentConfig);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadConfig()
        {
            try
            {
                this.currentConfig = null;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(MODULE_LINK);
                var state = this.currentControlStateRDO != null ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == CONTROL_STATE_KEY) : null;
                if (state != null && !String.IsNullOrEmpty(state.VALUE))
                {
                    this.currentConfig = JsonConvert.DeserializeObject<CashierScreenConfigADO>(state.VALUE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            if (this.currentConfig == null)
            {
                this.currentConfig = CashierScreenConfigADO.CreateDefault();
            }
            this.currentConfig.FillDefault();
        }

        private void SetConfigToControl(CashierScreenConfigADO config)
        {
            try
            {
                radioBillFilter.SelectedIndex = config.BillFilter;
                chkHideAmount.Checked = config.IsHideAmount;
                spnTitleSize.EditValue = config.TitleFontSize;
                cboTitleColor.Color = WaitingScreenDrawHelper.ParseColor(config.TitleColor, Color.Black);
                spnListSize.EditValue = config.ListFontSize;
                cboListColor.Color = WaitingScreenDrawHelper.ParseColor(config.ListColor, Color.Black);
                spnCallingSize.EditValue = config.CallingFontSize;
                cboCallingColor.Color = WaitingScreenDrawHelper.ParseColor(config.CallingColor, Color.White);
                cboCallingBackColor.Color = WaitingScreenDrawHelper.ParseColor(config.CallingBackColor, Color.OrangeRed);
                txtInviteText.Text = config.InviteText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private CashierScreenConfigADO GetConfigFromControl()
        {
            CashierScreenConfigADO config = new CashierScreenConfigADO();
            config.BillFilter = radioBillFilter.SelectedIndex;
            config.IsHideAmount = chkHideAmount.Checked;
            config.TitleFontSize = (int)spnTitleSize.Value;
            config.TitleColor = WaitingScreenDrawHelper.ToHtml(cboTitleColor.Color);
            config.ListFontSize = (int)spnListSize.Value;
            config.ListColor = WaitingScreenDrawHelper.ToHtml(cboListColor.Color);
            config.CallingFontSize = (int)spnCallingSize.Value;
            config.CallingColor = WaitingScreenDrawHelper.ToHtml(cboCallingColor.Color);
            config.CallingBackColor = WaitingScreenDrawHelper.ToHtml(cboCallingBackColor.Color);
            config.InviteText = txtInviteText.Text.Trim();
            config.FillDefault();
            return config;
        }

        private void SaveConfig(CashierScreenConfigADO config)
        {
            try
            {
                if (this.controlStateWorker == null) return;
                if (this.currentControlStateRDO == null)
                {
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                }
                // SetData removes every KEY of the module that is not passed in, so keep the full list
                var state = this.currentControlStateRDO.FirstOrDefault(o => o.KEY == CONTROL_STATE_KEY);
                if (state == null)
                {
                    state = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    state.KEY = CONTROL_STATE_KEY;
                    state.MODULE_LINK = MODULE_LINK;
                    this.currentControlStateRDO.Add(state);
                }
                state.VALUE = JsonConvert.SerializeObject(config);
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.mediStock == null) return;
                this.currentConfig = GetConfigFromControl();
                SaveConfig(this.currentConfig);

                // Only one cashier waiting screen per client: close the old one before opening again
                List<Form> openedForms = new List<Form>();
                foreach (Form f in Application.OpenForms)
                {
                    if (f is frmCashierWaitingScreen) openedForms.Add(f);
                }
                foreach (Form f in openedForms)
                {
                    f.Close();
                }

                frmCashierWaitingScreen frm = new frmCashierWaitingScreen(this.currentConfig, this.mediStock);
                WaitingScreenDrawHelper.ShowInExtendMonitor(frm);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
