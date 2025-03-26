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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.PlusInfo.ADO;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCWorkPlace : UserControlBase
    {
        #region Declare

        UserControl ucWorkPlace;
        internal WorkPlaceProcessor workPlaceProcessor;
        internal WorkPlaceProcessor.Template workPlaceTemplate;
        WorkPlaceInitADO workPlaceInitADO;
        Inventec.Desktop.Common.Modules.Module currentModule;
        DelegateFocusMoveout dlgFocusNextUserControl;

        #endregion

        #region Constructor - Load

        public UCWorkPlace(Inventec.Desktop.Common.Modules.Module module, DelegateFocusMoveout __dlgFocusNextUserControl)
            : base("UCPlusInfo", "UCWorkPlace")
        {
            try
            {
                InitializeComponent();
                this.currentModule = module;
                dlgFocusNextUserControl = __dlgFocusNextUserControl;
                InitWorkPlaceControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCWorkPlace_Load(object sender, EventArgs e)
        {
            try
            {
                //
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region private Process

        public async Task InitWorkPlaceControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitWorkPlaceControl Start!");
                List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE> datas = null;
                if (BackendDataWorker.IsExistsKey<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>())
                {
                    datas = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                }
                else
                {
                    Inventec.Core.CommonParam paramCommon = new Inventec.Core.CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    filter.IS_ACTIVE = 1;
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>>("api/HisWorkPlace/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_WORK_PLACE), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.workPlaceProcessor = new WorkPlaceProcessor(new Inventec.Core.CommonParam());
                workPlaceInitADO = new WorkPlaceInitADO();
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.CheDoHienThiNoiLamViecManHinhDangKyTiepDon == 1)
                {
                    workPlaceInitADO.Template = WorkPlaceProcessor.Template.Textbox1;
                    this.workPlaceTemplate = WorkPlaceProcessor.Template.Textbox1;
                }
                else
                {
                    workPlaceInitADO.Template = WorkPlaceProcessor.Template.Combo1;
                    this.workPlaceTemplate = WorkPlaceProcessor.Template.Combo1;
                }
                workPlaceInitADO.FocusMoveout = this.dlgFocusNextUserControl;
                workPlaceInitADO.PlusClick = WorkPlacePlusClick;
                workPlaceInitADO.WorlPlaces = datas.Where(p => p.IS_ACTIVE == 1).ToList();
                this.ucWorkPlace = (await this.workPlaceProcessor.Generate(workPlaceInitADO).ConfigureAwait(false)) as UserControl;
                if (this.ucWorkPlace != null)
                {
                    this.ucWorkPlace.Dock = DockStyle.Fill;
                    this.Controls.Add(this.ucWorkPlace);
                }
                this.workPlaceProcessor.FocusNextUserControl(workPlaceTemplate, this.dlgFocusNextUserControl);
                Inventec.Common.Logging.LogSystem.Debug("InitWorkPlaceControl Finished!");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void WorkPlacePlusClick()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisWorkPlace").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisWorkPlace'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisWorkPlace' is not plugins");

                List<object> listArgs = new List<object>();
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, (this.currentModule != null ? this.currentModule.RoomId : 0), (this.currentModule != null ? this.currentModule.RoomTypeId : 0)), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                ((Form)extenceInstance).ShowDialog();

                BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>();
                this.workPlaceProcessor.Reload(WorkPlaceProcessor.Template.Combo1, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_WORK_PLACE>().Where(p => p.IS_ACTIVE == 1).ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void SetCurrentModuleAgain(Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                if (module != null)
                    this.currentModule = module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Data

        internal object GetValue(object uc)
        {
            object result = null;
            WorkPlaceProcessor.Template template = ((UCWorkPlace)uc).workPlaceTemplate;
            try
            {
                switch (template)
                {
                    case WorkPlaceProcessor.Template.Combo:
                        result = ((UCWorkPlaceCombo)ucWorkPlace).GetValue();
                        break;
                    case WorkPlaceProcessor.Template.Combo1:
                        result = ((UCWorkPlaceCombo1)ucWorkPlace).GetValue();
                        break;
                    case WorkPlaceProcessor.Template.Textbox:
                        result = ((UCWorkPlaceTextbox)ucWorkPlace).GetValue();
                        break;
                    case WorkPlaceProcessor.Template.Textbox1:
                        result = ((UCWorkPlaceTextbox1)ucWorkPlace).GetValue();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        internal void SetValue(object uc, object value)
        {
            try
            {
                if (uc != null)
                {
                    WorkPlaceProcessor.Template _template = ((UCWorkPlace)uc).workPlaceTemplate;
                    if (_template == WorkPlaceProcessor.Template.Combo)
                    {
                        ((UCWorkPlaceCombo)ucWorkPlace).SetValue(value);
                    }
                    else if (_template == WorkPlaceProcessor.Template.Textbox)
                    {
                        ((UCWorkPlaceTextbox)ucWorkPlace).SetValue(value);
                    }
                    else if (_template == WorkPlaceProcessor.Template.Combo1)
                    {
                        ((UCWorkPlaceCombo1)ucWorkPlace).SetValue(value);
                    }
                    else if (_template == WorkPlaceProcessor.Template.Textbox1)
                    {
                        ((UCWorkPlaceTextbox1)ucWorkPlace).SetValue(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void ReFreshUserControl(WorkPlaceProcessor.Template template, object data)
        {
            try
            {
                switch (template)
                {
                    case WorkPlaceProcessor.Template.Combo:
                        ((UCWorkPlaceCombo)ucWorkPlace).ReloadData(data);
                        break;
                    case WorkPlaceProcessor.Template.Combo1:
                        ((UCWorkPlaceCombo1)ucWorkPlace).ReloadData(data);
                        break;
                    case WorkPlaceProcessor.Template.Textbox:
                        ((UCWorkPlaceCombo)ucWorkPlace).ReloadData(data);
                        break;
                    case WorkPlaceProcessor.Template.Textbox1:
                        ((UCWorkPlaceCombo1)ucWorkPlace).ReloadData(data);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Focus

        internal void FocusNextControl()
        {
            try
            {
                this.dlgFocusNextUserControl = this.SendTab;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SendTab()
        {
            try
            {
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusControl(WorkPlaceProcessor.Template data)
        {
            try
            {
                switch (data)
                {
                    case WorkPlaceProcessor.Template.Combo:
                        ((UCWorkPlaceCombo)ucWorkPlace).FocusControl();
                        break;
                    case WorkPlaceProcessor.Template.Textbox:
                        ((UCWorkPlaceTextbox)ucWorkPlace).FocusControl();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            try
            {
                switch (data)
                {
                    case WorkPlaceProcessor.Template.Combo1:
                        ((UCWorkPlaceCombo1)ucWorkPlace).FocusControl();
                        break;
                    case WorkPlaceProcessor.Template.Textbox1:
                        ((UCWorkPlaceTextbox1)ucWorkPlace).FocusControl();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        internal void DisposeControl()
        {
            try
            {
                dlgFocusNextUserControl = null;
                currentModule = null;
                workPlaceInitADO = null;
                workPlaceProcessor = null;
                ucWorkPlace = null;
                this.Load -= new System.EventHandler(this.UCWorkPlace_Load);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
