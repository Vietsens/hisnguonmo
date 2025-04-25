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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using Inventec.Common.Logging;
using MOS.LibraryHein.Bhyt.HeinTreatmentType;
using System.Reflection;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentFinish.ADO;
using static MPS.ProcessorBase.PrintConfig;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        HIS.Desktop.Plugins.Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor process;
        bool isLoad = false;

        private bool PrintNow;
        private const string Mps000478 = "Mps000478";

        public enum ModuleTypePrint
        {
            IN_GIAY_CHUYEN_VIEN,
            IN_GIAY_RA_VIEN,
            HEN_KHAM_LAI,
            BANG_KE_THANH_TOAN,
            BIEU_MAU_KHAC,
            _IN_GIAY_CHUNG_NHAN_PTTT,
            _IN_GIAY_CHUNG_NHAN_TT,
            GIAY_BAO_TU,
            GIAY_NGHI_OM,
            TOM_TAT_BENH_AN,
            Mps000382,
            Mps000399,
            Mps000476,
            Mps000485,
            Mps000478_TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC,
            Mps000222_TOM_TAT_KET_QUA_CLS
        }

        internal void PrintCloseTreatment_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Hide();
                var btnIn = sender as DevExpress.Utils.Menu.DXMenuItem;
                ModuleTypePrint printType = (ModuleTypePrint)btnIn.Tag;
                if(AppointmentPrintOptionsStorageADO.InPhieuHenKham || AppointmentPrintOptionsStorageADO.KyPhieuHenKham || AppointmentPrintOptionsStorageADO.XemTruocKhiIn)
                {
                    if (printType == ModuleTypePrint.HEN_KHAM_LAI)
                    {
                        var inPhieu = AppointmentPrintOptionsStorageADO.InPhieuHenKham;
                        var kyPhieu = AppointmentPrintOptionsStorageADO.KyPhieuHenKham;
                        var xemPhieu = AppointmentPrintOptionsStorageADO.XemTruocKhiIn;
                        MPS.ProcessorBase.PrintConfig.PreviewType previewType = PreviewType.ShowDialog;
                        if (inPhieu && !kyPhieu && !xemPhieu)
                        {
                            previewType = PreviewType.PrintNow;
                        }
                        else if (!inPhieu && !kyPhieu && xemPhieu)
                        {
                            previewType = PreviewType.Show;
                        }
                        else if (!inPhieu && kyPhieu && !xemPhieu)
                        {
                            previewType = PreviewType.EmrSignNow;
                        }
                        else if (inPhieu && kyPhieu && !xemPhieu)
                        {
                            previewType = PreviewType.EmrSignAndPrintNow;
                        }
                        else if (!inPhieu && kyPhieu && xemPhieu)
                        {
                            previewType = PreviewType.EmrSignAndPrintPreview;
                        }
                        if (!isLoad)
                        {
                            process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0, previewType);
                            isLoad = true;
                        }

                        DelegateRunPrinter(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode);
                    }
                }
                else
                {
                    switch (printType)
                    {
                        case ModuleTypePrint.GIAY_NGHI_OM:
                            PrintTreatmentEndTypeExtProcessor printTreatmentEndTypeExtProcessor = new PrintTreatmentEndTypeExtProcessor(this.treatmentId, ReloadMenuTreatmentEndTypeExt, CreateMenu.TYPE.DYNAMIC, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);

                            printTreatmentEndTypeExtProcessor.Print(HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM, PrintTreatmentEndTypeExtProcessor.OPTION.PRINT);
                            break;
                        case ModuleTypePrint.IN_GIAY_RA_VIEN:
                            if (!isLoad)
                            {
                                process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);
                                isLoad = true;
                            }
                            DelegateRunPrinter(MPS.Processor.Mps000008.PDO.Mps000008PDO.printTypeCode);
                            break;
                        case ModuleTypePrint.HEN_KHAM_LAI:
                            if (!isLoad)
                            {
                                process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);
                                isLoad = true;
                            }
                            DelegateRunPrinter(MPS.Processor.Mps000010.PDO.Mps000010PDO.printTypeCode);
                            break;
                        case ModuleTypePrint.IN_GIAY_CHUYEN_VIEN:
                            if (!isLoad)
                            {
                                process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, LoadServiceReq(hisTreatmentResult.ID), this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);
                                isLoad = true;
                            }
                            DelegateRunPrinter(MPS.Processor.Mps000011.PDO.Mps000011PDO.printTypeCode);
                            break;
                        case ModuleTypePrint.Mps000382:
                            if (!isLoad)
                            {
                                process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);
                                isLoad = true;
                            }
                            DelegateRunPrinter(MPS.Processor.Mps000382.PDO.Mps000382PDO.printTypeCode);
                            break;
                        case ModuleTypePrint.GIAY_BAO_TU:
                            if (!isLoad)
                            {
                                HIS_BRANCH branch = new HIS_BRANCH();
                                if (this.module != null)
                                {
                                    var currentRoom = this.hisRooms.FirstOrDefault(o => o.ID == this.module.RoomId && o.ROOM_TYPE_ID == this.module.RoomTypeId);
                                    branch = currentRoom != null ? this.hisBranchs.FirstOrDefault(o => o.ID == currentRoom.BRANCH_ID) : null;
                                }

                                process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, branch, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);
                                isLoad = true;
                            }
                            DelegateRunPrinter(MPS.Processor.Mps000268.PDO.Mps000268PDO.printTypeCode);
                            break;
                        case ModuleTypePrint.Mps000476:
                            ProcessMps000476();
                            break;
                        case ModuleTypePrint.BANG_KE_THANH_TOAN:
                            BangKeThanhToanClick();
                            break;
                        case ModuleTypePrint.BIEU_MAU_KHAC:
                            TaoBieuMauKhac(currentHisTreatment);
                            break;
                        case ModuleTypePrint._IN_GIAY_CHUNG_NHAN_PTTT:
                            ShowFormPTTT();
                            break;
                        case ModuleTypePrint._IN_GIAY_CHUNG_NHAN_TT:
                            ShowFormTT();
                            break;
                        case ModuleTypePrint.TOM_TAT_BENH_AN:
                            clickItemTomTatBenhAn();
                            break;
                        case ModuleTypePrint.Mps000399:
                            if (!isLoad)
                            {
                                process = new Library.PrintTreatmentFinish.PrintTreatmentFinishProcessor(hisTreatmentResult, this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0);
                                isLoad = true;
                            }
                            DelegateRunPrinter("Mps000399");
                            break;
                        case ModuleTypePrint.Mps000478_TOM_TAT_Y_LENH_PTTT_VA_DON_THUOC:
                            PrintMps000478(false);
                            break;
                        case ModuleTypePrint.Mps000222_TOM_TAT_KET_QUA_CLS:
                            PrintMps000222();
                            break;
                        case ModuleTypePrint.Mps000485:
                            PrintMps000485();
                            break;
                        default:
                            break;
                    }
                }                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private HIS_SERVICE_REQ LoadServiceReq(long id)
        {
            HIS_SERVICE_REQ serviceReqExamEndType = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter filter = new HisServiceReqFilter();
                filter.IS_ACTIVE = 1;
                filter.TREATMENT_ID = id;
                var dt = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                if (dt != null && dt.Count > 0 && dt.FirstOrDefault(o => o.EXAM_END_TYPE == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__KET_THUC_DT) != null)
                    serviceReqExamEndType = dt.FirstOrDefault(o => o.EXAM_END_TYPE == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ.EXAM_END_TYPE__KET_THUC_DT);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return serviceReqExamEndType;
        }
        private void PrintMps000485()
        {
            try
            {
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate("Mps000485", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintMps000478(bool printNow)
        {
            try
            {
                this.PrintNow = printNow;
                var richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.ROOT_PATH);
                richEditorMain.RunPrintTemplate(Mps000478, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintMps000222()
        {
            try
            {
                V_HIS_TREATMENT Treatment = GetTreatment_ByID(this.treatmentId);
                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(this.module.RoomId, Treatment);
                if (printTest != null)
                {
                    printTest.Print();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case Mps000478:
                        ProcessPrintMps000478(printCode, fileName, ref result);
                        break;
                    case "Mps000485":
                        ProcessPrintMps000485(printCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void ProcessPrintMps000485(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000484");
                if (this.currentHisTreatment.ID > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = this.currentHisTreatment.ID;
                    HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                        .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                    HIS_SEVERE_ILLNESS_INFO SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    List<HIS_EVENTS_CAUSES_DEATH> lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    HIS_DEPARTMENT_TRAN departmentTran = new HIS_DEPARTMENT_TRAN();
                    HIS_DEPARTMENT department = new HIS_DEPARTMENT();
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    CommonParam param = new CommonParam();
                    HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                    filter.TREATMENT_ID = this.currentHisTreatment.ID;
                    var dtSevere = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>("api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);
                    if (dtSevere != null && dtSevere.Count > 0)
                    {
                        SevereIllnessInfo = dtSevere.FirstOrDefault(o => o.IS_DEATH == 1);
                        if (SevereIllnessInfo != null)
                        {
                            MOS.Filter.HisDepartmentTranFilter filterDepartmentTran = new HisDepartmentTranFilter();
                            filterDepartmentTran.TREATMENT_ID = this.currentHisTreatment.ID;
                            filterDepartmentTran.DEPARTMENT_ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datas = new BackendAdapter(null).Get<List<HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/Get", ApiConsumers.MosConsumer, filterDepartmentTran, null);
                            if (datas != null && datas.Count > 0)
                                departmentTran = datas.Last();
                            MOS.Filter.HisDepartmentFilter filterDepartment = new HisDepartmentFilter();
                            filterDepartment.ID = SevereIllnessInfo.DEPARTMENT_ID;
                            var datasDepatment = new BackendAdapter(null).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filterDepartment, null);
                            if (datasDepatment != null && datasDepatment.Count > 0)
                                department = datasDepatment.First();
                            HisEventsCausesDeathFilter filterChild = new HisEventsCausesDeathFilter();
                            filterChild.SEVERE_ILLNESS_INFO_ID = SevereIllnessInfo.ID;
                            lstEvents = new BackendAdapter(param).Get<List<HIS_EVENTS_CAUSES_DEATH>>("api/HisEventsCausesDeath/Get", ApiConsumers.MosConsumer, filterChild, param);
                        }
                    }
                    HisPatientTypeAlterFilter patientTypeAlterFilter = new HisPatientTypeAlterFilter();
                    patientTypeAlterFilter.TREATMENT_ID = this.currentHisTreatment.ID;
                    var patientTypeAlterData = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/Get", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                    if (patientTypeAlterData != null && patientTypeAlterData.Count > 0)
                    {
                        patientTypeAlter = patientTypeAlterData.OrderByDescending(o=>o.CREATE_TIME).ToList()[0];
                    }
                    HIS_PATIENT patient = GetPatientByID(treatment.PATIENT_ID);
                    HIS_BRANCH branch = this.hisBranchs.FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    if (SevereIllnessInfo == null)
                        SevereIllnessInfo = new HIS_SEVERE_ILLNESS_INFO();
                    if (lstEvents == null)
                        lstEvents = new List<HIS_EVENTS_CAUSES_DEATH>();
                    if (patientTypeAlter == null)
                        patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    if (departmentTran == null)
                        departmentTran = new HIS_DEPARTMENT_TRAN();
                    if (department == null)
                        department = new HIS_DEPARTMENT();

                    WaitingManager.Hide();
                    MPS.Processor.Mps000485.PDO.Mps000485PDO pdo = new MPS.Processor.Mps000485.PDO.Mps000485PDO
                        (
                        SevereIllnessInfo,
                        lstEvents,
                        treatment,
                        patientTypeAlter,
                        patient,
                        departmentTran,
                        department,
                        branch,
                        BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>().ToList(),
                        BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToList()
                        );

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : ""), printTypeCode, module.RoomId);
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PATIENT GetPatientByID(long patientId)
        {
            HIS_PATIENT result = new HIS_PATIENT();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                filter.ID = patientId;
                var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (patients != null && patients.Count() > 0)
                {
                    result = patients.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }

        private void ProcessPrintMps000478(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("__________ProcessPrintMps000478");
                if (this.treatmentId > 0)
                {
                    WaitingManager.Show();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    V_HIS_TREATMENT Treatment = GetTreatment_ByID(this.treatmentId);
                    var listExpMestMedicine_ByTreatment = GetListExpMestMedicine_ByTreatmentID(this.treatmentId);
                    List<V_HIS_EXP_MEST_MEDICINE> listExpMestMedicine = new List<V_HIS_EXP_MEST_MEDICINE>();
                    if (listExpMestMedicine_ByTreatment != null)
                    {
                        listExpMestMedicine = listExpMestMedicine_ByTreatment.Where(o => o.IS_EXPEND != 1).ToList();
                    }
                    var listSereServ_ByTreatment = GetListSereServ_ByTreatmentID(this.treatmentId);
                    List<V_HIS_SERE_SERV> listSereServ = new List<V_HIS_SERE_SERV>();
                    if (listSereServ_ByTreatment != null)
                    {
                        listSereServ = listSereServ_ByTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT
                                                                        || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT
                                                                        || o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM).ToList();
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Treatment", Treatment));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("listExpMestMedicine", listExpMestMedicine));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("lstSereServ", listSereServ));
                    WaitingManager.Hide();
                    MPS.Processor.Mps000478.PDO.Mps000478PDO pdo = new MPS.Processor.Mps000478.PDO.Mps000478PDO
                        (
                        Treatment,
                        listSereServ,
                        listExpMestMedicine
                        );

                    if (this.PrintNow
                        || HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private V_HIS_TREATMENT GetTreatment_ByID(long treatmentId)
        {
            V_HIS_TREATMENT result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_EXP_MEST_MEDICINE> GetListExpMestMedicine_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_EXP_MEST_MEDICINE> result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisExpMestMedicineViewFilter filter = new HisExpMestMedicineViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV> GetListSereServ_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_SERE_SERV> result = null;
            try
            {
                if (treatmentId <= 0)
                    return null;
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param)
                    .Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessMps000476()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    var ado = new HIS.Desktop.Plugins.Library.PrintOtherForm.Base.PrintOtherInputADO();
                    ado.TreatmentId = treatmentId;
                    var printProcess = new HIS.Desktop.Plugins.Library.PrintOtherForm.PrintOtherFormProcessor(ado, Library.PrintOtherForm.Base.UpdateType.TYPE.OPEN_OTHER_ASS_TREATMENT);
                    printProcess.Print(Library.PrintOtherForm.Base.PrintType.TYPE.MPS000476_CHAN_DOAN_NGUYEN_NHAN_TU_VONG);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Event Button Print
        private void BangKeThanhToanClick()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(this.treatmentId);
                if (this.module == null)
                {
                    CallModule.Run(CallModule.Bordereau, 0, 0, listArgs);
                }
                else
                {
                    CallModule.Run(CallModule.Bordereau, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DelegateRunPrinter(string printTypeCode)
        {
            try
            {
                process.Print(printTypeCode, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowFormPTTT()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(currentHisTreatment.ID);
                listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT.ToString());
                if (this.module == null)
                {
                    CallModule.Run(CallModule.ListSurgMisuByTreatment, 0, 0, listArgs);
                }
                else
                {
                    CallModule.Run(CallModule.ListSurgMisuByTreatment, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ShowFormTT()
        {
            try
            {
                List<object> listArgs = new List<object>();
                listArgs.Add(currentHisTreatment.ID);
                listArgs.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT.ToString());
                if (this.module == null)
                {
                    CallModule.Run(CallModule.ListSurgMisuByTreatment, 0, 0, listArgs);
                }
                else
                {
                    CallModule.Run(CallModule.ListSurgMisuByTreatment, this.module.RoomId, this.module.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TaoBieuMauKhac(HIS_TREATMENT data)
        {
            try
            {
                WaitingManager.Hide();
                if (data != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.ID);

                    if (this.module == null)
                    {
                        CallModule.Run(CallModule.OtherForms, 0, 0, listArgs);
                    }
                    else
                    {
                        CallModule.Run(CallModule.OtherForms, this.module.RoomId, this.module.RoomTypeId, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void clickItemTomTatBenhAn()
        {
            try
            {
                var printTest = new HIS.Desktop.Plugins.Library.PrintTestTotal.PrintTestTotalProcessor(0, treatmentId);
                if (printTest != null)
                {
                    printTest.Print("Mps000316");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
