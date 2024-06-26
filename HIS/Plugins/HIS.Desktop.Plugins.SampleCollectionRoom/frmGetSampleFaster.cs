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
using AutoMapper;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.SampleCollectionRoom.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using LIS.SDO;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    public partial class frmGetSampleFaster : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        V_LIS_SAMPLE rowSample;
        V_HIS_ROOM room = null;
        DelegateSelectData refreshDataAfterSuccess;
        public frmGetSampleFaster(Inventec.Desktop.Common.Modules.Module module, V_HIS_ROOM vRoom, DelegateSelectData _refreshDataAfterSuccess)
            : base(module)
        {
            try
            {
                InitializeComponent();
                this.refreshDataAfterSuccess = _refreshDataAfterSuccess;
                this.currentModule = module;
                this.room = vRoom;
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

        private void frmGetSampleFaster_Load(object sender, EventArgs e)
        {
            try
            {
                txtBarcode.Focus();
                txtBarcode.SelectAll();
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void executeAction()
        {
            try
            {
                if (!string.IsNullOrEmpty(txtBarcode.Text))
                {
                    GetSampleByBarCode();
                    onClickBtnPrintBarCode();
                    onClickBtnPrintPhieuHen();
                    UpdateSampleStt();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGetSampleFaster_Click(object sender, EventArgs e)
        {
            executeAction();
        }

        internal enum PrintType
        {
            IN_BARCODE,
            IN_PHIEU_HEN
        }

        private void GetSampleByBarCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                LisSampleViewFilter sampleFilter = new LisSampleViewFilter();
                sampleFilter.SERVICE_REQ_CODE__EXACT = this.txtBarcode.Text.Trim();
                var LisSamples = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, sampleFilter, param);
                if (LisSamples != null && LisSamples.Count == 1)
                {
                    this.rowSample = LisSamples.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region in barcode

        private void onClickBtnPrintBarCode()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.IN_BARCODE;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BARCODE:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077, DelegateRunPrinter);
                        break;
                    case PrintType.IN_PHIEU_HEN:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000233.PDO.Mps000233PDO.PrintTypeCode.Mps000233, DelegateRunPrinter);
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

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077:
                        LoadBieuMauPhieuYCInBarCode(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000233.PDO.Mps000233PDO.PrintTypeCode.Mps000233:
                        LoadBieuMauPhieuHen(printTypeCode, fileName, ref result);
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

        internal void LoadBieuMauPhieuYCInBarCode(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (rowSample == null)
                    return;
                //bool refresh = false;
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        /*Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        Mapper.CreateMap<V_LIS_SAMPLE, LIS_SAMPLE>();
                        LIS_SAMPLE data = Mapper.Map<LIS_SAMPLE>(rowSample);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        //listArgs.Add(this.currentModule);
                        //var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(this.currentModule, listArgs);
                        //if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        //((Form)extenceInstance).ShowDialog();
                        */
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                        }
                    }
                }

                MOS.Filter.HisServiceReqViewFilter ServiceReqViewFilter = new HisServiceReqViewFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE = rowSample.SERVICE_REQ_CODE;
                var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();

                MPS.Processor.Mps000077.PDO.Mps000077PDO mps000077RDO = new MPS.Processor.Mps000077.PDO.Mps000077PDO(
                           rowSample,
                           rs
                           );
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                result = MPS.MpsPrinter.Run(PrintData);

                //FillDataToGridControl();
                //gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        #endregion

        #region lấy mẫu
        private void UpdateSampleStt()
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();

                if (this.rowSample != null &&
                    (this.rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || this.rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                       /* Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        Mapper.CreateMap<V_LIS_SAMPLE, LIS_SAMPLE>();
                        LIS_SAMPLE data = Mapper.Map<LIS_SAMPLE>(rowSample);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add(this.currentModule);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(this.currentModule, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        ((Form)extenceInstance).ShowDialog();*/
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        WaitingManager.Show();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion
                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        #endregion

        #region in phiếu hẹn
        private void onClickBtnPrintPhieuHen()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.IN_PHIEU_HEN;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal void LoadBieuMauPhieuHen(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                if (rowSample == null)
                    return;
                //bool refresh = false;
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                       /* Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        Mapper.CreateMap<V_LIS_SAMPLE, LIS_SAMPLE>();
                        LIS_SAMPLE data = Mapper.Map<LIS_SAMPLE>(rowSample);
                        List<object> listArgs = new List<object>();
                        listArgs.Add(data);
                        listArgs.Add(this.currentModule);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(this.currentModule, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        ((Form)extenceInstance).ShowDialog();*/
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(rowSample);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        WaitingManager.Show();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                        }
                    }
                }

                MOS.Filter.HisServiceReqViewFilter ServiceReqViewFilter = new HisServiceReqViewFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE = rowSample.SERVICE_REQ_CODE;
                var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();

                // get sereServs
                MOS.Filter.HisSereServView6Filter hisSereServView6Filter = new MOS.Filter.HisSereServView6Filter();
                hisSereServView6Filter.SERVICE_REQ_ID = rs.ID;
                hisSereServView6Filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_6>>("api/HisSereServ/GetView6", ApiConsumer.ApiConsumers.MosConsumer, hisSereServView6Filter, param);
                List<V_HIS_SERVICE> serviceParents = new List<V_HIS_SERVICE>();
                if (sereServs != null && sereServs.Count > 0)
                {
                    var services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => sereServs.Select(o => o.SERVICE_ID).Distinct().Contains(p.ID)).ToList();
                    if (services != null && services.Count > 0)
                    {
                        serviceParents = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => services.Select(p => p.PARENT_ID).Distinct().Contains(o.ID)).ToList();
                    }
                }

                MPS.Processor.Mps000233.PDO.Mps000233PDO mps000233RDO = new MPS.Processor.Mps000233.PDO.Mps000233PDO(
                           rowSample,
                           rs,
                           serviceParents,
                           sereServs
                           );
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000233RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                result = MPS.MpsPrinter.Run(PrintData);

                //FillDataToGridControl();
                //gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        #endregion

        private void txtBarcode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    executeAction();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnGetSampleFaster_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnGetSampleFaster_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
