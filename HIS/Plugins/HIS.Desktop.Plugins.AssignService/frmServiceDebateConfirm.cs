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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService
{
    public partial class frmServiceDebateConfirm : Form
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<MOS.EFMODEL.DataModels.HIS_DEBATE> lstHisDebate { get; set; }
        List<V_HIS_SERVICE> lstService { get; set; }
        string message = "";
        long treatmentId = 0;

        public frmServiceDebateConfirm(Inventec.Desktop.Common.Modules.Module _Module, List<V_HIS_SERVICE> _lstService, List<MOS.EFMODEL.DataModels.HIS_DEBATE> _lstHisDebate, string _message, long _treatmentId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _Module;
                this.lstService = _lstService;
                this.lstHisDebate = _lstHisDebate;
                this.message = _message;
                this.treatmentId = _treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void frmMedicineDebateConfirm_Load(object sender, EventArgs e)
        {
            txtMessage.Text = this.message;
            btnCreateDebate.Focus();
            SetCaptionByLanguageKey();
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmServiceDebateConfirm
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmServiceDebateConfirm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmServiceDebateConfirm.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmServiceDebateConfirm.btnClose.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateDebate.Text = Inventec.Common.Resource.Get.Value("frmServiceDebateConfirm.btnCreateDebate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmServiceDebateConfirm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnCreateDebate_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var item in lstService)
                {
                    List<object> listArgs = new List<object>();
                    
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.DebateDiagnostic").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.DebateDiagnostic");

                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        HIS_SERVICE service = new HIS_SERVICE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(service, item);
                        listArgs.Add(service);
                        listArgs.Add(lstHisDebate);
                        listArgs.Add(this.treatmentId);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
