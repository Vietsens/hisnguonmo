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
using System.Collections;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.UC.TreeSereServ7;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.RichEditor.Base;
using DevExpress.XtraBars;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using Inventec.Desktop.Plugins.ExecuteRoom;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.PayClinicalResult.Base;
using Inventec.Desktop.Common.Modules;

namespace HIS.Desktop.Plugins.PayClinicalResult
{
    public partial class UCExecuteRoom : HIS.Desktop.Utility.UserControlBase
    {
        void ExecuteRoomMouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.serviceReqRightClick != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    ExecuteRoomPopupMenuProcessor.ModuleType type = (ExecuteRoomPopupMenuProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case ExecuteRoomPopupMenuProcessor.ModuleType.SummaryInforTreatmentRecords:
                            SummaryInforTreatmentRecordsClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.AggrHospitalFees:
                            AggrHospitalFeesClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.TreatmentHistory:
                            TreatmentHistoryClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.TreatmentHistory2:
                            TreatmentHistory2Click(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.RoomTran:
                            RoomTranClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.DepositReq:
                            DepositReqClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.Bordereau:
                            BordereauClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.Execute:
                            ExecuteClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.UnStart:
                            UnStartClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.UnExecute:
                            CancelFinish(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.OtherForms:
                            OtherFormClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.ServiceReqList:
                            ServiceReqListClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.BenhAnNgoaiTru:
                            InBenhAnNgoaiTru(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.Debate:
                            DebateClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.SuaYeuCauKham:
                            SuaYeuCauKham(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.AssignPaan:
                            AssignPaanClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.TreatmentList:
                            TreatmentListClick(this.serviceReqRightClick);
                            break;
                        case ExecuteRoomPopupMenuProcessor.ModuleType.AllergyCard:
                            AllergyCardClick(this.serviceReqRightClick);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AllergyCardClick(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AllergyCard").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AllergyCard");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AssignPaanClick(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPaan").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPaan");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq);
                    
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    listArgs.Add(serviceReq.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TreatmentListClick(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentList").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentList");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.TDL_TREATMENT_CODE);
                    Module currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId);
                    listArgs.Add(serviceReq);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(currentModule, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), currentModule.ExtensionInfo.Code + serviceReq.SERVICE_REQ_CODE + serviceReq.TDL_TREATMENT_CODE, serviceReq.TDL_TREATMENT_CODE + " - " + serviceReq.TDL_PATIENT_NAME, (System.Windows.Forms.UserControl)extenceInstance, currentModule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SuaYeuCauKham(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.UpdateExamServiceReq").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.UpdateExamServiceReq");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReq.ID);
                    listArgs.Add(true);//La phong kham
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SummaryInforTreatmentRecordsClick(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SummaryInforTreatmentRecords").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.SummaryInforTreatmentRecords");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AggrHospitalFeesClick(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrHospitalFees").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrHospitalFees");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(serviceReq.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TreatmentHistoryClick(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                btnTreatmentHistory_Click(null, null);
                //Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                //if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //{
                //    List<object> listArgs = new List<object>();
                //    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                //    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                //    treatmentFilter.ID = serviceReq.TREATMENT_ID;
                //    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                //    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                //    if (treatment != null)
                //    {
                //        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                //        treatmentHistory.treatmentId = treatment.ID;
                //        treatmentHistory.treatment_code = treatment.TREATMENT_CODE;
                //        listArgs.Add(treatmentHistory);
                //        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                //        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                //        ((Form)extenceInstance).Show();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void TreatmentHistory2Click(HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TreatmentHistory").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.TreatmentHistory");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();

                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = serviceReq.TREATMENT_ID;
                    V_HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    if (treatment != null)
                    {
                        TreatmentHistoryADO treatmentHistory = new TreatmentHistoryADO();
                        treatmentHistory.treatmentId = treatment.ID;
                        treatmentHistory.treatment_code = treatment.TREATMENT_CODE;
                        listArgs.Add(treatmentHistory);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RoomTranClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChangeExamRoomProcess").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ChangeExamRoomProcess");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>();
                    V_HIS_SERVICE_REQ serviceReq = AutoMapper.Mapper.Map<HIS_SERVICE_REQ, V_HIS_SERVICE_REQ>(serviceReqInput);
                    listArgs.Add(serviceReq);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DepositReqClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RequestDeposit").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.RequestDeposit");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add(serviceReqInput.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BordereauClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Bordereau").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Bordereau");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    moduleData.RoomId = roomId;
                    moduleData.RoomTypeId = roomTypeId;
                    listArgs.Add(serviceReqInput.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OtherFormClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.OtherForms").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.OtherForms");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(serviceReqInput.TREATMENT_ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null"); ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExecuteClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                LoadModuleExecuteService(serviceReqInput);
                InitEnableControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnStartClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    WaitingManager.Show();
                    var serviceReq = new BackendAdapter(param)
                        .Post<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>(HisRequestUriStore.HIS_SERVICE_REQ_UNSTART, ApiConsumers.MosConsumer, serviceReqInput.ID, param);
                    if (serviceReq != null && serviceReq.ID > 0)
                    {
                        LoadServiceReqCount();
                        success = true;
                        btnUnStart.Enabled = true;
                        //Reload data
                        if (serviceReqInput.ID == currentHisServiceReq.ID)
                            currentHisServiceReq.SERVICE_REQ_STT_ID = serviceReq.SERVICE_REQ_STT_ID;
                        gridControlServiceReq.RefreshDataSource();
                    }

                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ServiceReqMatyClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisServiceReqMaty").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisServiceReqMaty");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(serviceReqInput.ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InBenhAnNgoaiTru(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174, DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DebateClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Debate").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.Debate");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(serviceReqInput.TREATMENT_ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ServiceReqListClick(HIS_SERVICE_REQ serviceReqInput)
        {
            try
            {
                if (serviceReqInput != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ServiceReqList").FirstOrDefault();
                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ServiceReqList");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        HIS_TREATMENT treatment = new HIS_TREATMENT();
                        treatment.ID = serviceReqInput.TREATMENT_ID;
                        listArgs.Add(treatment);
                        listArgs.Add(serviceReqInput.TREATMENT_ID);
                        var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, roomId, roomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                LoadBieuMauPhieuYCBenhAnNgoaiTru(printTypeCode, fileName, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
