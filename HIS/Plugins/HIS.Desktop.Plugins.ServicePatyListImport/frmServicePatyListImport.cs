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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.ServicePatyListImport
{
    public partial class frmServicePatyListImport : HIS.Desktop.Utility.FormBase
    {
        internal List<V_HIS_SERVICE_PATY> ListServicePatyprocess;
        internal Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_SERVICE_PATY> hisServicePaty;
        PagingGrid pagingGrid;
        internal long treatmentId = 0;
        int ActionType = -1;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;

        public frmServicePatyListImport()
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        public frmServicePatyListImport(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            InitializeComponent();
            try
            {
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                SetCaptionByLanguageKey();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    this.ActionType = GlobalVariables.ActionAdd;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public frmServicePatyListImport(List<V_HIS_SERVICE_PATY> servicePaty, Inventec.Desktop.Common.Modules.Module module)
            : this()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.Text = module.text;
                //this.treatment_id = exam.TREATMENT_ID;
                this.hisServicePaty = servicePaty;
                this.moduleData = module;
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
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServicePatyListImport.Resources.Lang", typeof(HIS.Desktop.Plugins.ServicePatyListImport.frmServicePatyListImport).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("frmServicePatyListImport.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmServicePatyListImport_Load(object sender, EventArgs e)
        {
            if (this.hisServicePaty != null)
            {
                LoadDataToGridServicePaty(hisServicePaty);
            }
            //FillDataToGridImport();
            //FillDataToGridControl();
        }

        private void LoadDataToGridServicePaty(List<V_HIS_SERVICE_PATY> hisServicePaty)
        {
            try
            {
                WaitingManager.Show();
                gridControlImport.DataSource = hisServicePaty;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int numPageSize;
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridImport()
        {
            try
            {
                WaitingManager.Show();
                try
                {
                    //OpenFileDialog ofd = new OpenFileDialog();
                    //ofd.Multiselect = false;
                    //if (ofd.ShowDialog() == DialogResult.OK)
                    //{
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    //if (import.ReadFileExcel(ofd.FileName))
                    //{
                    var ImpServicePatyProcessor = import.Get<ADO.ServicePatyAdo>(0);
                    if (ImpServicePatyProcessor != null && ImpServicePatyProcessor.Count > 0)
                    {
                        this.ListServicePatyprocess = new List<V_HIS_SERVICE_PATY>();
                        addServicePatyToProcessList(ImpServicePatyProcessor);

                        gridControlImport.BeginUpdate();
                        gridControlImport.DataSource = null;
                        gridControlImport.DataSource = this.ListServicePatyprocess;
                        gridControlImport.EndUpdate();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceMessage.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void addServicePatyToProcessList(List<ADO.ServicePatyAdo> servicePatyImports)
        {
            try
            {
                if (servicePatyImports == null || servicePatyImports.Count == 0)
                    return;
                foreach (var item in servicePatyImports)
                {
                    var ado = new ADO.ServicePatyAdo();
                    var serviceParty = new V_HIS_SERVICE_PATY();

                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ServicePatyAdo>(ado, item);
                    if (ado.SERVICE_CODE != null)
                    {
                        var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                        if (service != null)
                        {
                            serviceParty.SERVICE_ID = service.ID;
                            serviceParty.SERVICE_CODE = service.SERVICE_CODE;
                            serviceParty.SERVICE_NAME = service.SERVICE_NAME;
                            serviceParty.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        }
                        else
                        {
                            serviceParty.SERVICE_CODE = ado.SERVICE_CODE;
                        }
                    }
                    if (ado.PATIENT_TYPE_CODE != null)
                    {
                        var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                        if (patientType != null)
                        {
                            serviceParty.PATIENT_TYPE_ID = patientType.ID;
                            serviceParty.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }
                        else
                        {
                            serviceParty.PATIENT_TYPE_CODE = ado.PATIENT_TYPE_CODE;
                        }
                    }
                    if (ado.BRANCH_CODE != null)
                    {
                        var branchCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.BRANCH_CODE == item.BRANCH_CODE);
                        if (branchCode != null)
                        {
                            serviceParty.BRANCH_ID = branchCode.ID;
                            serviceParty.BRANCH_CODE = branchCode.BRANCH_CODE;
                            serviceParty.BRANCH_NAME = branchCode.BRANCH_NAME;
                        }
                        else
                        {
                            serviceParty.BRANCH_CODE = ado.BRANCH_CODE;
                        }
                    }
                    if (ado.EXECUTE_ROOM_CODES != null)
                    {
                        var excuteRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => ado.EXECUTE_ROOM_CODES.Contains(p.ROOM_CODE)).ToList();
                        if (excuteRoom != null)
                        {
                            foreach (var itemExcute in excuteRoom)
                            {
                                serviceParty.EXECUTE_ROOM_IDS += itemExcute.ID.ToString() + ",";
                            }
                        }
                    }
                    if (ado.REQUEST_DEPARMENT_CODES != null)
                    {
                        var deparmentCodes = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(
                            p => ado.REQUEST_DEPARMENT_CODES.Contains(p.DEPARTMENT_CODE)).ToList();
                        if (deparmentCodes != null)
                        {
                            foreach (var itemDeparment in deparmentCodes)
                            {
                                serviceParty.REQUEST_DEPARMENT_IDS += itemDeparment.ID.ToString() + ",";
                            }
                        }
                    }
                    if (ado.REQUEST_ROOM_CODES != null)
                    {
                        var requestRooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(
                            p => ado.REQUEST_ROOM_CODES.Contains(p.ROOM_CODE)).ToList();
                        if (requestRooms != null)
                        {
                            foreach (var itemRequestRooms in requestRooms)
                            {
                                serviceParty.REQUEST_ROOM_IDS += itemRequestRooms.ID.ToString() + ",";
                            }
                        }
                    }

                    serviceParty.PRICE = ado.PRICE;
                    serviceParty.PRIORITY = ado.PRIORITY;
                    serviceParty.VAT_RATIO = ado.VAT_RATIO;
                    serviceParty.INTRUCTION_NUMBER_FROM = ado.INTRUCTION_NUMBER_FROM;
                    serviceParty.INTRUCTION_NUMBER_TO = ado.INTRUCTION_NUMBER_TO;
                    serviceParty.FROM_TIME = ado.FROM_TIME;
                    serviceParty.TO_TIME = ado.TO_TIME;
                    serviceParty.TREATMENT_FROM_TIME = ado.TREATMENT_FROM_TIME;
                    serviceParty.TREATMENT_TO_TIME = ado.TREATMENT_TO_TIME;
                    serviceParty.DAY_FROM = ado.DAY_FROM;
                    serviceParty.DAY_TO = ado.DAY_TO;
                    serviceParty.HOUR_FROM = ado.HOUR_FROM;
                    serviceParty.HOUR_TO = ado.HOUR_TO;
                    serviceParty.IS_ACTIVE = 1;
                    this.ListServicePatyprocess.Add(serviceParty);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImport_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewImport_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (this.hisServicePaty != null)
                    {
                        foreach (var item in hisServicePaty)
                        {
                            var requestRooms = BackendDataWorker.Get<V_HIS_ROOM>().Where(
                                p => (item.REQUEST_ROOM_IDS != null ? item.REQUEST_ROOM_IDS : "").Contains(p.ROOM_CODE)).ToList();

                            var deparmentCodes = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(
                                p => (item.REQUEST_DEPARMENT_IDS != null ? item.REQUEST_DEPARMENT_IDS : "").Contains(p.DEPARTMENT_CODE)).ToList();

                            var excuteRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(p => (item.EXECUTE_ROOM_IDS!=null?item.EXECUTE_ROOM_IDS:"").Contains(p.ROOM_CODE)).ToList();

                            var branchCode = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.BRANCH_CODE == item.BRANCH_CODE);
                            var patientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == item.PATIENT_TYPE_CODE);
                            var service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.SERVICE_CODE == item.SERVICE_CODE);
                            if (service == null)
                            {
                                if (e.Column.FieldName == "SERVICE_CODE")
                                    e.Appearance.ForeColor = Color.Red;
                            }
                            if (requestRooms == null || requestRooms.Count==0)
                            {
                                if (e.Column.FieldName == "REQUEST_ROOM_IDS")
                                    e.Appearance.ForeColor = Color.Red;
                            }
                            if (deparmentCodes == null || deparmentCodes.Count == 0)
                            {
                                if (e.Column.FieldName == "REQUEST_DEPARMENT_IDS")
                                    e.Appearance.ForeColor = Color.Red;
                            }
                            if (excuteRoom == null || excuteRoom.Count == 0)
                            {
                                if (e.Column.FieldName == "EXECUTE_ROOM_IDS")
                                    e.Appearance.ForeColor = Color.Red;
                            }
                            if (branchCode == null)
                            {
                                if (e.Column.FieldName == "BRANCH_CODE")
                                    e.Appearance.ForeColor = Color.Red;
                            }
                            if (patientType == null)
                            {
                                if (e.Column.FieldName == "PATIENT_TYPE_CODE")
                                    e.Appearance.ForeColor = Color.Red;
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
