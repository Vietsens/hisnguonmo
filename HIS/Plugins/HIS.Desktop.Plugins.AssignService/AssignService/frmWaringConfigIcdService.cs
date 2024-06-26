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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmWaringConfigIcdService : Form
    {
        List<HIS_ICD> icdCodeFromUc;
        List<SereServADO> serviceCheckeds__Send;
        Inventec.Desktop.Common.Modules.Module currentModule;
        DelegateSelectData delegateSelectData;
        bool isYes = false;
        public System.Drawing.Size OldSize { get; set; }
        public frmWaringConfigIcdService(List<HIS_ICD> _icdCodeFromUc, List<SereServADO> _serviceCheckeds__Send, Inventec.Desktop.Common.Modules.Module _currentModule, DelegateSelectData _delegateSelectData)
        {
            InitializeComponent();
            this.icdCodeFromUc = _icdCodeFromUc;
            this.serviceCheckeds__Send = _serviceCheckeds__Send;
            this.currentModule = _currentModule;
            this.delegateSelectData = _delegateSelectData;
            try
            {
                this.Text = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmWaringConfigIcdService_Load(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.IcdServiceAllowUpdate == "1")
                {
                    btnServiceICD.Enabled = true;
                }
                else
                {
                    btnServiceICD.Enabled = false;
                }
                if (this.serviceCheckeds__Send != null && this.serviceCheckeds__Send.Count > 0)
                {
                    string serviceNames = "";
                    foreach (var item in this.serviceCheckeds__Send)
                    {
                        serviceNames += item.TDL_SERVICE_CODE + " - "+item.TDL_SERVICE_NAME+"; ";
                    }
                    txtWarning.Text = string.Format(ResourceMessage.DichVuChuaDuocCauHinhICDDichVu, serviceNames) + " " + ResourceMessage.BanCoMuonTiepTuc;
                }
                btnYes.Focus();
                btnYes.Select();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmWaringConfigIcdService
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmWaringConfigIcdService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmWaringConfigIcdService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnYes.Text = Inventec.Common.Resource.Get.Value("frmWaringConfigIcdService.btnYes.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNo.Text = Inventec.Common.Resource.Get.Value("frmWaringConfigIcdService.btnNo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceICD.Text = Inventec.Common.Resource.Get.Value("frmWaringConfigIcdService.btnServiceICD.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.txtWarning.Text = Inventec.Common.Resource.Get.Value("frmWaringConfigIcdService.txtWarning.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmWaringConfigIcdService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void btnServiceICD_Click(object sender, EventArgs e)
        {
            try
            {
                List<long> serviceNotConfigIds = new List<long>();
                if (this.icdCodeFromUc != null && this.icdCodeFromUc.Count > 0 && this.serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    this.Close();
                    this.isYes = false;
                    if (this.delegateSelectData != null)
                    {
                        this.delegateSelectData(this.isYes);
                    }
                    serviceNotConfigIds = this.serviceCheckeds__Send.Select(o => o.SERVICE_ID).Distinct().ToList();
                    List<object> listObj = new List<object>();
                    listObj.Add(this.icdCodeFromUc);
                    listObj.Add(serviceNotConfigIds);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.ServiceIcd", this.currentModule.RoomId, this.currentModule.RoomTypeId, listObj);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("icdCodeFromUc hoac serviceCheckeds__Send is null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.icdCodeFromUc), this.icdCodeFromUc) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceNotConfigIds), serviceNotConfigIds));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            try
            {
                this.isYes = false;
                if (this.delegateSelectData != null)
                {
                    this.delegateSelectData(this.isYes);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            try
            {
                this.isYes = true;
                if (this.delegateSelectData != null)
                {
                    this.delegateSelectData(this.isYes);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
