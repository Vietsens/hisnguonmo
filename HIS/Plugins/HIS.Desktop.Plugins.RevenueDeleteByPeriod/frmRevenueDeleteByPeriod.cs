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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.RevenueDeleteByPeriod.Validation;
using HTC.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RevenueDeleteByPeriod
{
    public partial class frmRevenueDeleteByPeriod : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        List<HTC_PERIOD> listPeriod = new List<HTC_PERIOD>();

        int positionHandleControl = -1;

        public frmRevenueDeleteByPeriod(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmRevenueDeleteByPeriod_Load(object sender, EventArgs e)
        {
            try
            {
                ValidControl();
                LoadDataToComboPeriod();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.RevenueDeleteByPeriod.Resources.Lang", typeof(HIS.Desktop.Plugins.RevenueDeleteByPeriod.frmRevenueDeleteByPeriod).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPeriod.Properties.NullText = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.cboPeriod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDelete.Text = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.btnDelete.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutPeriod.Text = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.layoutPeriod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCDelete.Caption = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.bbtnRCDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmRevenueDeleteByPeriod.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboPeriod()
        {
            try
            {
                listPeriod = BackendDataWorker.Get<HTC_PERIOD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HTC_RS.COMMON.IS_ACTIVE__TRUE).OrderByDescending(o => o.CREATE_TIME).ToList();
                cboPeriod.Properties.DataSource = listPeriod;
                cboPeriod.Properties.DisplayMember = "PERIOD_NAME";
                cboPeriod.Properties.ValueMember = "ID";
                cboPeriod.Properties.ForceInitialize();
                cboPeriod.Properties.Columns.Clear();
                cboPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_CODE", "Mã", 60));
                cboPeriod.Properties.Columns.Add(new LookUpColumnInfo("PERIOD_NAME", "Tên", 160));
                cboPeriod.Properties.ShowHeader = true;
                cboPeriod.Properties.ImmediatePopup = true;
                cboPeriod.Properties.DropDownRows = 10;
                cboPeriod.Properties.PopupWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlPeriod();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPeriod()
        {
            try
            {
                PeriodValidationRule periodRule = new PeriodValidationRule();
                periodRule.txtPeriodCode = txtPeriodCode;
                periodRule.cboPeriod = cboPeriod;
                dxValidationProvider1.SetValidationRule(txtPeriodCode, periodRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPeriodCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPeriodCode.Text))
                    {
                        string key = txtPeriodCode.Text.ToLower().Trim();
                        var listData = listPeriod.Where(o => o.PERIOD_CODE.ToLower().Contains(key) || o.PERIOD_NAME.ToLower().Contains(key)).ToList();
                        if (listPeriod != null && listPeriod.Count == 1)
                        {
                            valid = true;
                            cboPeriod.EditValue = null;
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (!valid)
                    {
                        cboPeriod.Focus();
                        cboPeriod.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPeriod_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPeriod_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                txtPeriodCode.Text = "";
                if (cboPeriod.EditValue != null)
                {
                    var period = listPeriod.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                    if (period != null)
                    {
                        txtPeriodCode.Text = period.PERIOD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnDelete.Enabled || !dxValidationProvider1.Validate())
                    return;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                {
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                var period = listPeriod.FirstOrDefault(o => o.ID == Convert.ToInt64(cboPeriod.EditValue));
                if (period != null)
                {
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HtcRevenue/DeleteByPeriod", ApiConsumers.HtcConsumer, period, param);
                }

                WaitingManager.Hide();
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCDelete_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnDelete_Click(null, null);
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
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
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
