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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    public partial class frmAssignServiceHistory : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_SERE_SERV> LstHisSereServ;
        SereServADO SereServADOs = new SereServADO();
        HisTreatmentWithPatientTypeInfoSDO currentHisTreatment;
        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmAssignServiceHistory(SereServADO _SereServADOs, HisTreatmentWithPatientTypeInfoSDO _currentHisTreatment, Inventec.Desktop.Common.Modules.Module _currentModule)
            : base(_currentModule)
        {
            InitializeComponent();
            
            try
            {
                this.SereServADOs = _SereServADOs;
                this.currentHisTreatment = _currentHisTreatment;
                this.currentModule = _currentModule;
                this.IsUseApplyFormClosingOption = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void frmAssignServiceHistory_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                FillDataToControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmAssignServiceHistory
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(frmAssignServiceHistory).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grc_History_STT.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grc_History_ServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_ServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grc_History_IntructionTime.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_IntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grc_History_RequestRoom.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_RequestRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grc_History_Amount.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillDataToControl()
        {
            try
            {
                CommonParam param = new CommonParam();
                LstHisSereServ = new List<V_HIS_SERE_SERV>();
                HisSereServViewFilter SereServViewFilter = new HisSereServViewFilter();

                SereServViewFilter.SERVICE_ID = SereServADOs.ID;
                SereServViewFilter.TREATMENT_ID = currentHisTreatment.ID;

                var SereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, SereServViewFilter, param);

                if (SereServs != null && SereServs.Count > 0)
                {
                    LstHisSereServ = SereServs.Where(o => o.IS_NO_EXECUTE == null && o.IS_DELETE == 0).ToList();
                }

                gridViewHistory.BeginUpdate();
                gridViewHistory.GridControl.DataSource = LstHisSereServ;
                gridViewHistory.EndUpdate();
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
                Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory = new ResourceManager("HIS.Desktop.Plugins.AssignService.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignService.AssignService.frmAssignServiceHistory).Assembly);

                this.grc_History_STT.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_STT.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory, LanguageManager.GetCulture());
                this.grc_History_ServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_ServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory, LanguageManager.GetCulture());
                this.grc_History_IntructionTime.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_IntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory, LanguageManager.GetCulture());
                this.grc_History_RequestRoom.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_RequestRoom.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory, LanguageManager.GetCulture());
                this.grc_History_Amount.Caption = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.grc_History_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmAssignServiceHistory.Text", Resources.ResourceLanguageManager.LanguageResource__frmAssignServiceHistory, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewHistory_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV dataRow = (V_HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.TDL_INTRUCTION_TIME);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }
    }
}
