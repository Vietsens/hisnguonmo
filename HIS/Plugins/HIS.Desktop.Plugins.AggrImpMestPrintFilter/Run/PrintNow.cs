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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AggrImpMestPrintFilter.Run
{
    public class PrintNow
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        public PrintNow(Inventec.Desktop.Common.Modules.Module _currentModule)
        {
            this.currentModule = _currentModule;
        }
        List<HIS_IMP_MEST> _ImpMests_Print = new List<HIS_IMP_MEST>();
        List<HIS_EXP_MEST> _MobaExpMests = new List<HIS_EXP_MEST>();

        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_GN_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_PXs = new List<V_HIS_IMP_MEST_MEDICINE>();
        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

        List<V_HIS_IMP_MEST_MEDICINE> _ImpMestMedicines { get; set; }
        List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMaterials { get; set; }

        List<V_HIS_IMP_MEST_MATERIAL> _ImpMestMedi_VTs = new List<V_HIS_IMP_MEST_MATERIAL>();

        internal List<long> serviceUnitIds = new List<long>();
        internal List<long> reqRoomIds = new List<long>();
        internal List<long> _useFormIds = new List<long>();

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        V_HIS_IMP_MEST aggrImpMest;
        List<V_HIS_IMP_MEST_2> listAggrImpMest;
        List<V_HIS_IMP_MEST> listAggrImpMest1;
        internal HIS_DEPARTMENT _Department;
        internal long _DepartmentId = 0;
        long configKeyMERGER_DATA = 0;

        public void RunPrintNow(long _roomId, V_HIS_IMP_MEST _aggrImpMest, long type)
        {
            try
            {
                this.aggrImpMest = _aggrImpMest;
                this._DepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == _roomId).DepartmentId;
                ProcessPrint(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void RunPrintNow(long _roomId, List<V_HIS_IMP_MEST_2> _listAggrImpMest, long type)
        {
            try
            {
                this.listAggrImpMest = _listAggrImpMest;
                this._DepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == _roomId).DepartmentId;
                ProcessPrint(_roomId, _listAggrImpMest, type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void RunPrintNow(long _roomId, List<V_HIS_IMP_MEST> _listAggrImpMest1, long type)
        {
            try
            {
                this.listAggrImpMest1 = _listAggrImpMest1;
                this._DepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == _roomId).DepartmentId;
                ProcessPrint(_roomId, _listAggrImpMest1, type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint(long type)
        {
            try
            {
                this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                switch (type)
                {
                    case 5:
                        richEditorMain.RunPrintTemplate("Mps000245", DelegateRunPrinter);
                        break;
                    case 6:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100, DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrint(long _roomId, List<V_HIS_IMP_MEST_2> _aggrExpMests, long type)
        {
            try
            {
                this._DepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == _roomId).DepartmentId;
                this.listAggrImpMest = _aggrExpMests.OrderBy(o => o.IMP_MEST_CODE).ToList();
                this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                switch (type)
                {
                    case 6:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100, DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrint(long _roomId, List<V_HIS_IMP_MEST> _aggrExpMests1, long type)
        {
            try
            {
                this._DepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == _roomId).DepartmentId;
                this.listAggrImpMest1 = _aggrExpMests1.OrderBy(o => o.IMP_MEST_CODE).ToList();
                this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100, DelegateRunPrinter1);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter1(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                long departmentId = (this.listAggrImpMest1.First().REQ_DEPARTMENT_ID ?? 0);
                if (departmentId <= 0)
                {
                    departmentId = this._DepartmentId;
                }
                this._Department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);
                switch (printCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100:
                        LoadPhieuTraDSThuocGNHT(printCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                long departmentId = 0;
                if (this.listAggrImpMest != null)
                {
                    departmentId = (this.listAggrImpMest.First().REQ_DEPARTMENT_ID ?? 0);
                }

                if (departmentId <= 0)
                {
                    departmentId = this._DepartmentId;
                }
                this._Department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);
                switch (printTypeCode)
                {
                    case "Mps000245":
                        InTheoBenhNhan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100:
                        LoadPhieuTraThuocGNHT(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void InTheoBenhNhan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                WaitingManager.Show();

                LoadDataMedicineAndMaterial(this.aggrImpMest);

                if (this._ImpMests_Print != null && this._ImpMests_Print.Count > 0)
                {
                    this._ImpMests_Print = this._ImpMests_Print.Where(p => p.IMP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_TYPE.ID__BL).ToList();
                    MOS.Filter.HisPatientTypeAlterFilter patientTypeFilter = new HisPatientTypeAlterFilter();
                    patientTypeFilter.TREATMENT_IDs = this._ImpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                    var _DataPatientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ALTER>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET, ApiConsumers.MosConsumer, patientTypeFilter, null);

                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.IDs = this._ImpMests_Print.Select(p => p.TDL_PATIENT_ID ?? 0).ToList();
                    var _DataPatients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                    if (_DataPatients == null)
                    {
                        Inventec.Common.Logging.LogSystem.Error("Danh sach benh nhan rong ");
                        return;
                    }

                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.IDs = this._ImpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                    var _Datatreatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, null);

                    var GroupTreatments = this._ImpMests_Print.GroupBy(p => p.TDL_TREATMENT_ID).ToList();

                    long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_TRA);

                    if (keyPrintType == 1)
                    {
                        foreach (var itemGr in GroupTreatments)
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                            var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                            var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                            MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                        this._ImpMestMedicines,
                        this._ImpMestMaterials,
                        this.aggrImpMest,
                        this._ImpMests_Print,
                        this._Department,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000245.PDO.keyTitles.phieuLinhTongHop,
                         this.configKeyMERGER_DATA,
                         patient,
                         _DataPatientTypeAlters,
                         this._MobaExpMests,
                         treatment
                    );
                            WaitingManager.Hide();
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    else
                    {
                        #region Tach Thuoc GN - HT - TT
                        ProcessorMedicneGNHT();
                        #endregion

                        #region In Thuoc Thuong
                        if (_ImpMestMedi_Ts != null && _ImpMestMedi_Ts.Count > 0)
                        {
                            List<long> ExpMestIds = _ImpMestMedi_Ts.Select(p => p.IMP_MEST_ID).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                if (itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                {
                                    continue;
                                }
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                            this._ImpMestMedi_Ts,
                            null,
                            this.aggrImpMest,
                            this._ImpMests_Print,
                            this._Department,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000245.PDO.keyTitles.phieuLinhThuocThuong,
                             this.configKeyMERGER_DATA,
                             patient,
                             _DataPatientTypeAlters,
                             this._MobaExpMests,
                             treatment
                        );
                                WaitingManager.Hide();
                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                }
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
                        #endregion

                        #region Vat Tu
                        if (this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                        {
                            List<long> ImpMestIds = _ImpMestMaterials.Select(p => p.IMP_MEST_ID).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                if (itemGr.Where(p => ImpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                {
                                    continue;
                                }
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                            null,
                            _ImpMestMaterials,
                            this.aggrImpMest,
                            this._ImpMests_Print,
                            this._Department,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000245.PDO.keyTitles.VatTu,
                             this.configKeyMERGER_DATA,
                             patient,
                             _DataPatientTypeAlters,
                             this._MobaExpMests,
                             treatment
                        );
                                WaitingManager.Hide();
                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                }
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
                        #endregion

                        #region Gay Nghien Huong Than
                        if ((this._ImpMestMedi_GNs != null && this._ImpMestMedi_GNs.Count > 0) || (this._ImpMestMedi_HTs != null && this._ImpMestMedi_HTs.Count > 0))
                        {
                            List<V_HIS_IMP_MEST_MEDICINE> DataGroups = new List<V_HIS_IMP_MEST_MEDICINE>();
                            DataGroups.AddRange(this._ImpMestMedi_GNs);
                            DataGroups.AddRange(this._ImpMestMedi_HTs);
                            long keyPrintTypeHTGN = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                            if (keyPrintTypeHTGN == 1)
                            {
                                List<long> ImpMestIds = DataGroups.Select(p => p.IMP_MEST_ID).ToList();
                                foreach (var itemGr in GroupTreatments)
                                {
                                    if (itemGr.Where(p => ImpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                    {
                                        continue;
                                    }
                                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                    var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                    var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                    MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                                DataGroups,
                                null,
                                this.aggrImpMest,
                                this._ImpMests_Print,
                                this._Department,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                 MPS.Processor.Mps000245.PDO.keyTitles.GayNghienHuongThan,
                                 this.configKeyMERGER_DATA,
                                 patient,
                                 _DataPatientTypeAlters,
                                 this._MobaExpMests,
                                 treatment
                            );
                                    WaitingManager.Hide();
                                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                    {
                                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                    }
                                    else
                                    {
                                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                    }
                                    result = MPS.MpsPrinter.Run(PrintData);
                                }
                            }
                            else
                            {
                                if (this._ImpMestMedi_GNs != null && this._ImpMestMedi_GNs.Count > 0)
                                {
                                    List<long> ExpMestIds = _ImpMestMedi_GNs.Select(p => p.IMP_MEST_ID).ToList();
                                    foreach (var itemGr in GroupTreatments)
                                    {
                                        if (itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                        {
                                            continue;
                                        }
                                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                        var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                        var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                        MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                                    _ImpMestMedi_GNs,
                                    null,
                                    this.aggrImpMest,
                                    this._ImpMests_Print,
                                    this._Department,
                                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                     MPS.Processor.Mps000245.PDO.keyTitles.GayNghien,
                                     this.configKeyMERGER_DATA,
                                     patient,
                                     _DataPatientTypeAlters,
                                     this._MobaExpMests,
                                     treatment
                                );
                                        WaitingManager.Hide();
                                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                        {
                                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                        }
                                        else
                                        {
                                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                        }
                                        result = MPS.MpsPrinter.Run(PrintData);
                                    }
                                }
                                if (this._ImpMestMedi_HTs != null && this._ImpMestMedi_HTs.Count > 0)
                                {
                                    List<long> ImpMestIds = _ImpMestMedi_HTs.Select(p => p.IMP_MEST_ID).ToList();
                                    foreach (var itemGr in GroupTreatments)
                                    {
                                        if (itemGr.Where(p => ImpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                        {
                                            continue;
                                        }
                                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                        var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                        var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                        MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                                    _ImpMestMedi_HTs,
                                    null,
                                    this.aggrImpMest,
                                    this._ImpMests_Print,
                                    this._Department,
                                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                     MPS.Processor.Mps000245.PDO.keyTitles.HuongThan,
                                     this.configKeyMERGER_DATA,
                                     patient,
                                     _DataPatientTypeAlters,
                                     this._MobaExpMests,
                                     treatment
                                );
                                        WaitingManager.Hide();
                                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                        {
                                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                        }
                                        else
                                        {
                                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                        }
                                        result = MPS.MpsPrinter.Run(PrintData);
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Thuoc Doc
                        if (this._ImpMestMedi_TDs != null && this._ImpMestMedi_TDs.Count > 0)
                        {
                            List<long> ImpMestIds = _ImpMestMedi_TDs.Select(p => p.IMP_MEST_ID).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                if (itemGr.Where(p => ImpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                {
                                    continue;
                                }
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                            _ImpMestMedi_TDs,
                            null,
                            this.aggrImpMest,
                            this._ImpMests_Print,
                            this._Department,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000245.PDO.keyTitles.ThuocDoc,
                             this.configKeyMERGER_DATA,
                             patient,
                             _DataPatientTypeAlters,
                             this._MobaExpMests,
                             treatment
                        );
                                WaitingManager.Hide();
                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                }
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
                        #endregion

                        #region Thuoc Phong Xa
                        if (this._ImpMestMedi_PXs != null && this._ImpMestMedi_PXs.Count > 0)
                        {
                            List<long> ImpMestIds = _ImpMestMedi_PXs.Select(p => p.IMP_MEST_ID).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                if (itemGr.Where(p => ImpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                {
                                    continue;
                                }
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                            _ImpMestMedi_PXs,
                            null,
                            this.aggrImpMest,
                            this._ImpMests_Print,
                            this._Department,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000245.PDO.keyTitles.PhongXa,
                             this.configKeyMERGER_DATA,
                             patient,
                             _DataPatientTypeAlters,
                             this._MobaExpMests,
                             treatment
                        );
                                WaitingManager.Hide();
                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                }
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
                        #endregion

                        #region nhom khac
                        if (_ImpMestMedi_Others != null && _ImpMestMedi_Others.Count > 0)
                        {
                            var groups = _ImpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            foreach (var gr in groups)
                            {
                                MPS.Processor.Mps000245.PDO.keyTitles title = new MPS.Processor.Mps000245.PDO.keyTitles();
                                if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)
                                {
                                    title = MPS.Processor.Mps000245.PDO.keyTitles.Corticoid;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN)
                                {
                                    title = MPS.Processor.Mps000245.PDO.keyTitles.DichTruyen;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                {
                                    title = MPS.Processor.Mps000245.PDO.keyTitles.KhangSinh;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    title = MPS.Processor.Mps000245.PDO.keyTitles.Lao;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                                {
                                    title = MPS.Processor.Mps000245.PDO.keyTitles.TienChat;
                                }

                                var grname = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == gr.First().MEDICINE_GROUP_ID) ?? new HIS_MEDICINE_GROUP();
                                List<long> ExpMestIds = gr.Select(p => p.IMP_MEST_ID).ToList();
                                foreach (var itemGr in GroupTreatments)
                                {
                                    if (itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList().Count <= 0)
                                    {
                                        continue;
                                    }
                                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                    var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                    var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;
                                    MPS.Processor.Mps000245.PDO.Mps000245PDO mps000245RDO = new MPS.Processor.Mps000245.PDO.Mps000245PDO(
                                gr.ToList(),
                                null,
                                this.aggrImpMest,
                                this._ImpMests_Print,
                                this._Department,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                 title,
                                 this.configKeyMERGER_DATA,
                                 patient,
                                 _DataPatientTypeAlters,
                                 this._MobaExpMests,
                                 treatment
                            );
                                    WaitingManager.Hide();
                                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                    {
                                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                    }
                                    else
                                    {
                                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000245RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                    }
                                    result = MPS.MpsPrinter.Run(PrintData);
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPhieuTraThuocGNHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                WaitingManager.Show();

                LoadDataMedicineAndMaterial2(this.listAggrImpMest);

                Dictionary<string, ADO.MediMatePrintADO> DicDataPrint = new Dictionary<string, ADO.MediMatePrintADO>();

                if (AppConfigKeys.ListParentMedicine != null && AppConfigKeys.ListParentMedicine.Count > 0)
                {
                    List<V_HIS_MEDICINE_TYPE> listParentMedicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => AppConfigKeys.ListParentMedicine.Contains(o.MEDICINE_TYPE_CODE)).ToList();
                    List<V_HIS_MATERIAL_TYPE> listParentMaterial = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => AppConfigKeys.ListParentMedicine.Contains(o.MATERIAL_TYPE_CODE)).ToList();

                    if (listParentMedicine != null && listParentMedicine.Count > 0 && this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                    {
                        var listExpMetyIds = this._ImpMestMedicines.Select(s => s.MEDICINE_TYPE_ID).Distinct().ToList();
                        List<V_HIS_MEDICINE_TYPE> listExpMety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => listExpMetyIds.Contains(o.ID)).ToList();
                        if (listExpMety != null && listExpMety.Count > 0)
                        {
                            var lstExpMetyChild = listExpMety.Where(o => listParentMedicine.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            var lstExpMetyNotChild = listExpMety.Where(o => !listParentMedicine.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            if (lstExpMetyNotChild != null && lstExpMetyNotChild.Count > 0)
                            {
                                if (this.listAggrImpMest.Count > 1)
                                {
                                    #region stock
                                    var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                    foreach (var stock in mediStockGroup)
                                    {
                                        var expMestMedicineTypeNotChild = this._ImpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                        var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                        if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                        if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                        ado._ImpMests_Print.AddRange(expMestP);

                                        if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                                        ado._ImpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                        DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region old
                                    var expMestMedicineTypeNotChild = this._ImpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                    var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                    if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                    if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                    ado._ImpMests_Print.AddRange(expMestP);

                                    if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                                    ado._ImpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                    DicDataPrint[" "] = ado;
                                    #endregion
                                }
                            }

                            if (lstExpMetyChild != null && lstExpMetyChild.Count > 0)
                            {
                                foreach (var item in listParentMedicine)
                                {
                                    var groupParent = lstExpMetyChild.Where(o => o.PARENT_ID == item.ID).ToList();
                                    if (groupParent != null && groupParent.Count > 0)
                                    {
                                        if (this.listAggrImpMest.Count > 1)
                                        {
                                            #region stock
                                            var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                            foreach (var stock in mediStockGroup)
                                            {
                                                var expMestMedicineTypeChild = this._ImpMestMedicines.Where(o => groupParent.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                                var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                                ado._ImpMestMedicines = expMestMedicineTypeChild;
                                                ado._ImpMests_Print = expMestP;

                                                DicDataPrint[stock.First().MEDI_STOCK_CODE + "_" + item.MEDICINE_TYPE_CODE] = ado;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            var expMestMedicineTypeChild = this._ImpMestMedicines.Where(o => groupParent.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                            var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                            ado._ImpMestMedicines = expMestMedicineTypeChild;
                                            ado._ImpMests_Print = expMestP;
                                            DicDataPrint[item.MEDICINE_TYPE_CODE ?? " "] = ado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                    {
                        if (this.listAggrImpMest.Count > 1)
                        {
                            #region stock
                            var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                            foreach (var stock in mediStockGroup)
                            {
                                var expMestMedicineTypeNotChild = this._ImpMestMedicines.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                ado._ImpMests_Print.AddRange(expMestP);

                                if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                                ado._ImpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                            }
                            #endregion
                        }
                        else
                        {
                            #region old
                            var expMestP = this._ImpMests_Print.Where(o => this._ImpMestMedicines.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                            if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                            ado._ImpMests_Print.AddRange(expMestP);

                            if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                            ado._ImpMestMedicines.AddRange(_ImpMestMedicines);

                            DicDataPrint[" "] = ado;
                            #endregion
                        }
                    }

                    if (listParentMaterial != null && listParentMaterial.Count > 0 && this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                    {
                        var listExpMatyIds = this._ImpMestMaterials.Select(s => s.MATERIAL_TYPE_ID).Distinct().ToList();
                        List<V_HIS_MATERIAL_TYPE> listExpMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => listExpMatyIds.Contains(o.ID)).ToList();
                        if (listExpMaty != null && listExpMaty.Count > 0)
                        {
                            var lstExpMatyChild = listExpMaty.Where(o => listParentMaterial.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            var lstExpMatyNotChild = listExpMaty.Where(o => !listParentMaterial.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            if (lstExpMatyNotChild != null && lstExpMatyNotChild.Count > 0)
                            {
                                if (this.listAggrImpMest.Count > 1)
                                {
                                    #region stock
                                    var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                    foreach (var stock in mediStockGroup)
                                    {
                                        var expMestMaterialTypeNotChild = this._ImpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                        var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                        if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                        if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                        ado._ImpMests_Print.AddRange(expMestP);

                                        if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                                        ado._ImpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                        DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region old
                                    var expMestMaterialTypeNotChild = this._ImpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                    var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                    if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                    if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                    ado._ImpMests_Print.AddRange(expMestP);

                                    if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                                    ado._ImpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                    DicDataPrint[" "] = ado;
                                    #endregion
                                }
                            }

                            if (lstExpMatyChild != null && lstExpMatyChild.Count > 0)
                            {
                                foreach (var item in listParentMaterial)
                                {
                                    var groupParent = lstExpMatyChild.Where(o => o.PARENT_ID == item.ID).ToList();
                                    if (groupParent != null && groupParent.Count > 0)
                                    {
                                        if (this.listAggrImpMest.Count > 1)
                                        {
                                            #region stock
                                            var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                            foreach (var stock in mediStockGroup)
                                            {
                                                var expMestMaterialTypeChild = this._ImpMestMaterials.Where(o => groupParent.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                                var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                                ado._ImpMestMaterials = expMestMaterialTypeChild;
                                                ado._ImpMests_Print = expMestP;

                                                DicDataPrint[stock.First().MEDI_STOCK_CODE + "_" + item.MATERIAL_TYPE_CODE] = ado;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            var expMestMaterialTypeChild = this._ImpMestMaterials.Where(o => groupParent.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                            var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                            ado._ImpMestMaterials = expMestMaterialTypeChild;
                                            ado._ImpMests_Print = expMestP;
                                            DicDataPrint[item.MATERIAL_TYPE_CODE ?? " "] = ado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                    {
                        if (this.listAggrImpMest.Count > 1)
                        {
                            #region stock
                            var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                            foreach (var stock in mediStockGroup)
                            {
                                var expMestMaterialTypeNotChild = this._ImpMestMaterials.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                ado._ImpMests_Print.AddRange(expMestP);

                                if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                                ado._ImpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                            }
                            #endregion
                        }
                        else
                        {
                            #region old
                            var expMestP = this._ImpMests_Print.Where(o => this._ImpMestMaterials.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                            if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                            ado._ImpMests_Print.AddRange(expMestP);

                            if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                            ado._ImpMestMaterials.AddRange(this._ImpMestMaterials);

                            DicDataPrint[" "] = ado;
                            #endregion
                        }
                    }
                }
                else
                {
                    DicDataPrint.Clear();
                    if (this.listAggrImpMest.Count > 1)
                    {
                        var mediStockGroup = this.listAggrImpMest.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                        foreach (var stock in mediStockGroup)
                        {
                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            ado._ImpMests_Print = this._ImpMests_Print.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                            if (ado._ImpMests_Print == null || ado._ImpMests_Print.Count <= 0)
                            {
                                continue;
                            }

                            ado._ImpMestMaterials = this._ImpMestMaterials.Where(o => ado._ImpMests_Print.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                            ado._ImpMestMedicines = this._ImpMestMedicines.Where(o => ado._ImpMests_Print.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                            DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                        }
                    }
                    else
                    {
                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                        ado._ImpMestMaterials = this._ImpMestMaterials;
                        ado._ImpMestMedicines = this._ImpMestMedicines;
                        ado._ImpMests_Print = this._ImpMests_Print;
                        DicDataPrint[" "] = ado;
                    }
                }

                foreach (var item in DicDataPrint)
                {
                    this._ImpMests_Print = item.Value._ImpMests_Print.Distinct().ToList();
                    this._ImpMestMedicines = item.Value._ImpMestMedicines;
                    this._ImpMestMaterials = item.Value._ImpMestMaterials;

                    //mpsConfig49.PARENT_TYPE_CODE = item.Key;

                    //if (keyPrintType == 1)
                    //{
                    //    #region In Tong Hop
                    //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    //    MPS.Processor.Mps000049.PDO.Mps000049PDO mps000049RDO = new MPS.Processor.Mps000049.PDO.Mps000049PDO(
                    //        this._ExpMestMedicines,
                    //        this._ExpMestMaterials,
                    //        this._AggrExpMests.FirstOrDefault(),
                    //        this._ExpMests_Print,
                    //        this._Department,
                    //        serviceUnitIds,
                    //        useFormIds,
                    //        reqRoomIds,
                    //        true,
                    //        true,
                    //        true,
                    //        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    //        MPS.Processor.Mps000049.PDO.keyTitles.phieuLinhTongHop,
                    //        mpsConfig49
                    //    );

                    //    WaitingManager.Hide();
                    //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //    {
                    //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000049RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    //    }
                    //    else
                    //    {
                    //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000049RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    //    }
                    //    result = MPS.MpsPrinter.Run(PrintData);
                    //    #endregion
                    //}
                    //else
                    {
                        #region Tach Thuoc GN - HT - TT
                        ProcessorMedicneGNHT();
                        #endregion

                        #region In gây nghiện
                        if ((this._ImpMestMedi_GNs != null && this._ImpMestMedi_GNs.Count > 0) || (this._ImpMestMedi_HTs != null && this._ImpMestMedi_HTs.Count > 0))
                        {
                            richEditorMain.RunPrintTemplate("Mps000101", DelegateRunMps);
                        }
                        #endregion

                        #region In thuốc độc
                        if (this._ImpMestMedi_TDs != null && this._ImpMestMedi_TDs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000241", DelegateRunMps);
                        }
                        #endregion

                        #region In phóng xạ
                        if (this._ImpMestMedi_PXs != null && this._ImpMestMedi_PXs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000240", DelegateRunMps);
                        }
                        #endregion

                        #region In vật tư

                        if (this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000100", DelegateRunMps);
                        }
                        #endregion

                        #region In Thuoc Thuong
                        if (_ImpMestMedi_Ts != null && _ImpMestMedi_Ts.Count > 0)
                        {
                            var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMedi_Ts.First().AGGR_IMP_MEST_ID);
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                            aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);

                            List<V_HIS_IMP_MEST> listAggrImpMests = new List<V_HIS_IMP_MEST>();
                            foreach (var AggrImpMest in this.listAggrImpMest)
                            {
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                listAggrImpMests.Add(AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(AggrImpMest));
                            }

                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.listAggrImpMest != null ? this.listAggrImpMest.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                            MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                                 _ImpMestMedi_Ts,
                        null,
                        aggrImpMest,
                        this._Department,
                        this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                        true,
                        true,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000100.PDO.IsTittle.ThuocThuong,
                        BackendDataWorker.Get<V_HIS_ROOM>(),
                        this._MobaExpMests,
                        AppConfigKeys.ProcessOderOption,
                        listAggrImpMests
                        );

                            WaitingManager.Hide();
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion

                        #region thuoc nhom khac
                        if (this._ImpMestMedi_Others != null && this._ImpMestMedi_Others.Count > 0)
                        {
                            var groups = _ImpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            foreach (var gr in groups)
                            {
                                MPS.Processor.Mps000078.PDO.IsTittle title = new MPS.Processor.Mps000078.PDO.IsTittle();
                                if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.Corticoid;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.DichTruyen;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.KhangSinh;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.Lao;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.TienChat;
                                }

                                var grname = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == gr.First().MEDICINE_GROUP_ID) ?? new HIS_MEDICINE_GROUP();

                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                MPS.Processor.Mps000078.PDO.Mps000078PDO mps000078RDO = new MPS.Processor.Mps000078.PDO.Mps000078PDO(
                                    gr.ToList(),
                                null,
                                this.aggrImpMest,
                                this._Department,
                                this.serviceUnitIds,
                                this._useFormIds,
                                this.reqRoomIds,
                                true,
                                true,
                                IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                                IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                title,
                                this._ImpMests_Print,
                                this._MobaExpMests
                                );

                                WaitingManager.Hide();
                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000078RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000078RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                }
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
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

        private void LoadPhieuTraDSThuocGNHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                WaitingManager.Show();

                LoadDataMedicineAndMaterial1(this.listAggrImpMest1);

                Dictionary<string, ADO.MediMatePrintADO> DicDataPrint = new Dictionary<string, ADO.MediMatePrintADO>();

                #region parent
                if (AppConfigKeys.ListParentMedicine != null && AppConfigKeys.ListParentMedicine.Count > 0)
                {
                    List<V_HIS_MEDICINE_TYPE> listParentMedicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => AppConfigKeys.ListParentMedicine.Contains(o.MEDICINE_TYPE_CODE)).ToList();
                    List<V_HIS_MATERIAL_TYPE> listParentMaterial = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => AppConfigKeys.ListParentMedicine.Contains(o.MATERIAL_TYPE_CODE)).ToList();

                    if (listParentMedicine != null && listParentMedicine.Count > 0 && this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                    {
                        var listExpMetyIds = this._ImpMestMedicines.Select(s => s.MEDICINE_TYPE_ID).Distinct().ToList();
                        List<V_HIS_MEDICINE_TYPE> listExpMety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => listExpMetyIds.Contains(o.ID)).ToList();
                        if (listExpMety != null && listExpMety.Count > 0)
                        {
                            var lstExpMetyChild = listExpMety.Where(o => listParentMedicine.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            var lstExpMetyNotChild = listExpMety.Where(o => !listParentMedicine.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            if (lstExpMetyNotChild != null && lstExpMetyNotChild.Count > 0)
                            {
                                if (this.listAggrImpMest1.Count > 1)
                                {
                                    #region stock
                                    var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                    foreach (var stock in mediStockGroup)
                                    {
                                        var expMestMedicineTypeNotChild = this._ImpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                        var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                        if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                        if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                        ado._ImpMests_Print.AddRange(expMestP);

                                        if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                                        ado._ImpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                        DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region old
                                    var expMestMedicineTypeNotChild = this._ImpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                    var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                    if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                    if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                    ado._ImpMests_Print.AddRange(expMestP);

                                    if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                                    ado._ImpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                    DicDataPrint[" "] = ado;
                                    #endregion
                                }
                            }

                            if (lstExpMetyChild != null && lstExpMetyChild.Count > 0)
                            {
                                foreach (var item in listParentMedicine)
                                {
                                    var groupParent = lstExpMetyChild.Where(o => o.PARENT_ID == item.ID).ToList();
                                    if (groupParent != null && groupParent.Count > 0)
                                    {
                                        if (this.listAggrImpMest1.Count > 1)
                                        {
                                            #region stock
                                            var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                            foreach (var stock in mediStockGroup)
                                            {
                                                var expMestMedicineTypeChild = this._ImpMestMedicines.Where(o => groupParent.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                                var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                                ado._ImpMestMedicines = expMestMedicineTypeChild;
                                                ado._ImpMests_Print = expMestP;

                                                DicDataPrint[stock.First().MEDI_STOCK_CODE + "_" + item.MEDICINE_TYPE_CODE] = ado;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            var expMestMedicineTypeChild = this._ImpMestMedicines.Where(o => groupParent.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                            var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                            ado._ImpMestMedicines = expMestMedicineTypeChild;
                                            ado._ImpMests_Print = expMestP;
                                            DicDataPrint[item.MEDICINE_TYPE_CODE ?? " "] = ado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                    {
                        if (this.listAggrImpMest1.Count > 1)
                        {
                            #region stock
                            var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                            foreach (var stock in mediStockGroup)
                            {
                                var expMestMedicineTypeNotChild = this._ImpMestMedicines.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                var expMestP = this._ImpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                ado._ImpMests_Print.AddRange(expMestP);

                                if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                                ado._ImpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                            }
                            #endregion
                        }
                        else
                        {
                            #region old
                            var expMestP = this._ImpMests_Print.Where(o => this._ImpMestMedicines.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                            if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                            ado._ImpMests_Print.AddRange(expMestP);

                            if (ado._ImpMestMedicines == null) ado._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                            ado._ImpMestMedicines.AddRange(_ImpMestMedicines);

                            DicDataPrint[" "] = ado;
                            #endregion
                        }
                    }

                    if (listParentMaterial != null && listParentMaterial.Count > 0 && this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                    {
                        var listExpMatyIds = this._ImpMestMaterials.Select(s => s.MATERIAL_TYPE_ID).Distinct().ToList();
                        List<V_HIS_MATERIAL_TYPE> listExpMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => listExpMatyIds.Contains(o.ID)).ToList();
                        if (listExpMaty != null && listExpMaty.Count > 0)
                        {
                            var lstExpMatyChild = listExpMaty.Where(o => listParentMaterial.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            var lstExpMatyNotChild = listExpMaty.Where(o => !listParentMaterial.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            if (lstExpMatyNotChild != null && lstExpMatyNotChild.Count > 0)
                            {
                                if (this.listAggrImpMest1.Count > 1)
                                {
                                    #region stock
                                    var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                    foreach (var stock in mediStockGroup)
                                    {
                                        var expMestMaterialTypeNotChild = this._ImpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                        var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                        if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                        if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                        ado._ImpMests_Print.AddRange(expMestP);

                                        if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                                        ado._ImpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                        DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region old
                                    var expMestMaterialTypeNotChild = this._ImpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                    var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                    if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                    if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                    ado._ImpMests_Print.AddRange(expMestP);

                                    if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                                    ado._ImpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                    DicDataPrint[" "] = ado;
                                    #endregion
                                }
                            }

                            if (lstExpMatyChild != null && lstExpMatyChild.Count > 0)
                            {
                                foreach (var item in listParentMaterial)
                                {
                                    var groupParent = lstExpMatyChild.Where(o => o.PARENT_ID == item.ID).ToList();
                                    if (groupParent != null && groupParent.Count > 0)
                                    {
                                        if (this.listAggrImpMest1.Count > 1)
                                        {
                                            #region stock
                                            var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                            foreach (var stock in mediStockGroup)
                                            {
                                                var expMestMaterialTypeChild = this._ImpMestMaterials.Where(o => groupParent.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                                var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                                ado._ImpMestMaterials = expMestMaterialTypeChild;
                                                ado._ImpMests_Print = expMestP;

                                                DicDataPrint[stock.First().MEDI_STOCK_CODE + "_" + item.MATERIAL_TYPE_CODE] = ado;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            var expMestMaterialTypeChild = this._ImpMestMaterials.Where(o => groupParent.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                            var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                            ado._ImpMestMaterials = expMestMaterialTypeChild;
                                            ado._ImpMests_Print = expMestP;
                                            DicDataPrint[item.MATERIAL_TYPE_CODE ?? " "] = ado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                    {
                        if (this.listAggrImpMest1.Count > 1)
                        {
                            #region stock
                            var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                            foreach (var stock in mediStockGroup)
                            {
                                var expMestMaterialTypeNotChild = this._ImpMestMaterials.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                                var expMestP = this._ImpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                                ado._ImpMests_Print.AddRange(expMestP);

                                if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                                ado._ImpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                            }
                            #endregion
                        }
                        else
                        {
                            #region old
                            var expMestP = this._ImpMests_Print.Where(o => this._ImpMestMaterials.Select(s => s.IMP_MEST_ID).Distinct().Contains(o.ID)).ToList();

                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                            if (ado._ImpMests_Print == null) ado._ImpMests_Print = new List<HIS_IMP_MEST>();
                            ado._ImpMests_Print.AddRange(expMestP);

                            if (ado._ImpMestMaterials == null) ado._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                            ado._ImpMestMaterials.AddRange(this._ImpMestMaterials);

                            DicDataPrint[" "] = ado;
                            #endregion
                        }
                    }
                }
                else
                {
                    DicDataPrint.Clear();
                    if (this.listAggrImpMest1.Count > 1)
                    {
                        var mediStockGroup = this.listAggrImpMest1.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                        foreach (var stock in mediStockGroup)
                        {
                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            ado._ImpMests_Print = this._ImpMests_Print.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_IMP_MEST_ID ?? 0)).ToList();
                            if (ado._ImpMests_Print == null || ado._ImpMests_Print.Count <= 0)
                            {
                                continue;
                            }

                            ado._ImpMestMaterials = this._ImpMestMaterials.Where(o => ado._ImpMests_Print.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                            ado._ImpMestMedicines = this._ImpMestMedicines.Where(o => ado._ImpMests_Print.Select(s => s.ID).Contains(o.IMP_MEST_ID)).ToList();
                            DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                        }
                    }
                    else
                    {
                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                        ado._ImpMestMaterials = this._ImpMestMaterials;
                        ado._ImpMestMedicines = this._ImpMestMedicines;
                        ado._ImpMests_Print = this._ImpMests_Print;
                        DicDataPrint[" "] = ado;
                    }
                }
                #endregion
                foreach (var item in DicDataPrint)
                {
                    this._ImpMests_Print = item.Value._ImpMests_Print.Distinct().ToList();
                    this._ImpMestMedicines = item.Value._ImpMestMedicines;
                    this._ImpMestMaterials = item.Value._ImpMestMaterials;
                    {
                        #region Tach Thuoc GN - HT - TT
                        ProcessorMedicneGNHT();
                        #endregion

                        #region In Thuoc gay nghien
                        if ((this._ImpMestMedi_GNs != null && this._ImpMestMedi_GNs.Count > 0) || (this._ImpMestMedi_HTs != null && this._ImpMestMedi_HTs.Count > 0))
                        {
                            richEditorMain.RunPrintTemplate("Mps000101", DelegateRunMps);
                        }
                        #endregion

                        #region In Thuốc Độc
                        if (this._ImpMestMedi_TDs != null && this._ImpMestMedi_TDs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000241", DelegateRunMps);
                        }
                        #endregion

                        #region In Phóng xạ
                        if (this._ImpMestMedi_PXs != null && this._ImpMestMedi_PXs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000240", DelegateRunMps);
                        }
                        #endregion

                        #region In Vat tu
                        if (this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000100", DelegateRunMps);
                        }
                        #endregion

                        #region In Thuoc Thuong
                        if (_ImpMestMedi_Ts != null && _ImpMestMedi_Ts.Count > 0)
                        {
                            var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMedi_Ts.First().AGGR_IMP_MEST_ID);

                            aggrImpMest = aggrImpMest2;

                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.listAggrImpMest != null ? this.listAggrImpMest.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                            MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                                 _ImpMestMedi_Ts,
                        null,
                        aggrImpMest,
                        this._Department,
                        this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                        true,
                        true,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000100.PDO.IsTittle.ThuocThuong,
                        BackendDataWorker.Get<V_HIS_ROOM>(),this._MobaExpMests,
                         AppConfigKeys.ProcessOderOption,
                         this.listAggrImpMest1
                        );

                            WaitingManager.Hide();
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        #endregion

                        #region thuoc nhom khac
                        if (this._ImpMestMedi_Others != null && this._ImpMestMedi_Others.Count > 0)
                        {
                            var groups = _ImpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            foreach (var gr in groups)
                            {
                                MPS.Processor.Mps000078.PDO.IsTittle title = new MPS.Processor.Mps000078.PDO.IsTittle();
                                if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.Corticoid;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.DichTruyen;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.KhangSinh;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.Lao;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                                {
                                    title = MPS.Processor.Mps000078.PDO.IsTittle.TienChat;
                                }

                                var grname = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == gr.First().MEDICINE_GROUP_ID) ?? new HIS_MEDICINE_GROUP();

                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                MPS.Processor.Mps000078.PDO.Mps000078PDO mps000078RDO = new MPS.Processor.Mps000078.PDO.Mps000078PDO(
                                    gr.ToList(),
                                null,
                                this.aggrImpMest,
                                this._Department,
                                this.serviceUnitIds,
                                this._useFormIds,
                                this.reqRoomIds,
                                true,
                                true,
                                IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                                IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                title,
                                this._ImpMests_Print,
                                this._MobaExpMests
                                );

                                WaitingManager.Hide();
                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000078RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000078RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                                }
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
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

        private void LoadDataMedicineAndMaterial2(List<V_HIS_IMP_MEST_2> currentAggImpMest)
        {
            try
            {
                if (currentAggImpMest == null)
                    throw new Exception("Du lieu rong currentAggImpMest");
                CommonParam param = new CommonParam();
                //
                this._ImpMests_Print = new List<HIS_IMP_MEST>();
                this._MobaExpMests = new List<HIS_EXP_MEST>();
                this._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.AGGR_IMP_MEST_IDs = currentAggImpMest.Select(p => p.ID).ToList();
                this._ImpMests_Print = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, impMestFilter, param);
                if (this._ImpMests_Print != null && this._ImpMests_Print.Count > 0)
                {
                    int start = 0;
                    int count = this._ImpMests_Print.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = this._ImpMests_Print.Skip(start).Take(limit).ToList();
                        List<long> _impMestIds = new List<long>();
                        _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                        List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                        MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                        expMestFilter.IDs = _MobaExpMestIds;
                        var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                        if (dataExpMests != null && dataExpMests.Count > 0)
                        {
                            this._MobaExpMests.AddRange(dataExpMests);
                        }
                        CreateThread(_impMestIds);

                        start += 100;
                        count -= 100;
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineAndMaterial1(List<V_HIS_IMP_MEST> currentAggImpMest)
        {
            try
            {
                if (currentAggImpMest == null)
                    throw new Exception("Du lieu rong currentAggImpMest");
                CommonParam param = new CommonParam();
                //
                this._ImpMests_Print = new List<HIS_IMP_MEST>();
                this._MobaExpMests = new List<HIS_EXP_MEST>();
                this._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.AGGR_IMP_MEST_IDs = currentAggImpMest.Select(p => p.ID).ToList();
                this._ImpMests_Print = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, impMestFilter, param);
                if (this._ImpMests_Print != null && this._ImpMests_Print.Count > 0)
                {
                    int start = 0;
                    int count = this._ImpMests_Print.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = this._ImpMests_Print.Skip(start).Take(limit).ToList();
                        List<long> _impMestIds = new List<long>();
                        _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                        List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                        MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                        expMestFilter.IDs = _MobaExpMestIds;
                        var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                        if (dataExpMests != null && dataExpMests.Count > 0)
                        {
                            this._MobaExpMests.AddRange(dataExpMests);
                        }
                        CreateThread(_impMestIds);

                        start += 100;
                        count -= 100;
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunMps(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000100":
                        Mps000100(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000241":
                        Mps000241(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000101":
                        Mps000101(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000240":
                        Mps000240(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000241(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (_ImpMestMedi_TDs != null && _ImpMestMedi_TDs.Count > 0)
                {
                    WaitingManager.Show();
                    if (this.listAggrImpMest != null)
                    {
                        if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                        {
                            var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                            aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                        }
                        else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                        {
                            var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                            aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                        }
                    }
                    if (this.listAggrImpMest1 != null)
                    {
                        if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                        {
                            var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                            aggrImpMest = aggrImpMest2;
                        }
                        else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                        {
                            var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                            aggrImpMest = aggrImpMest2;
                        }
                    }
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.listAggrImpMest != null ? this.listAggrImpMest.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    MPS.Processor.Mps000241.PDO.Mps000241PDO mps000241RDO = new MPS.Processor.Mps000241.PDO.Mps000241PDO(
                     _ImpMestMedi_TDs,
                    aggrImpMest,
                    this._Department,
                    this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                      IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     this._MobaExpMests
                    );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000241RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000241RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000240(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (_ImpMestMedi_PXs != null && _ImpMestMedi_PXs.Count > 0)
                {
                    WaitingManager.Show();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    MPS.Processor.Mps000240.PDO.Mps000240PDO mps000240RDO = new MPS.Processor.Mps000240.PDO.Mps000240PDO(
                        this._ImpMestMedi_PXs,
                    this.aggrImpMest,
                    this._Department,
                    this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                    IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                      IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     this._MobaExpMests
                    );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000240RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000240RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000101(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);

                if (keyPrintType == 1)
                {
                    if (this._ImpMestMedi_GN_HTs != null && this._ImpMestMedi_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        if (this.listAggrImpMest != null)
                        {
                            if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                            }
                            else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                            }
                        }

                        if (this.listAggrImpMest1 != null)
                        {
                            if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                                aggrImpMest = aggrImpMest2;
                            }
                            else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                                aggrImpMest = aggrImpMest2;
                            }
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.listAggrImpMest != null ? this.listAggrImpMest.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                        MPS.Processor.Mps000101.PDO.Mps000101PDO mps000101RDO = new MPS.Processor.Mps000101.PDO.Mps000101PDO(
                            _ImpMestMedi_GN_HTs,
                            aggrImpMest,
                            this._Department,
                            this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                            MPS.Processor.Mps000101.PDO.IsTittle101.GayNghienTamThan,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                              IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                             BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             this._ImpMests_Print,
                             this._MobaExpMests
                        );

                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000101RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000101RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                else
                {
                    if (_ImpMestMedi_HTs != null && _ImpMestMedi_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        if (this.listAggrImpMest != null)
                        {
                            if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                            }
                            else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                            }
                        }
                        if (this.listAggrImpMest1 != null)
                        {
                            if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                                aggrImpMest = aggrImpMest2;
                            }
                            else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                                aggrImpMest = aggrImpMest2;
                            }
                        }
                        this._MobaExpMests = new List<HIS_EXP_MEST>();
                        if (this._ImpMests_Print != null && this._ImpMests_Print.Count > 0)
                        {
                            int start = 0;
                            int count = this._ImpMests_Print.Count;
                            while (count > 0)
                            {
                                int limit = (count <= 100) ? count : 100;
                                var listSub = this._ImpMests_Print.Skip(start).Take(limit).ToList();
                                List<long> _impMestIds = new List<long>();
                                _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                                List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                                CommonParam param = new CommonParam();
                                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                                expMestFilter.IDs = _MobaExpMestIds;
                                var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                                if (dataExpMests != null && dataExpMests.Count > 0)
                                {
                                    this._MobaExpMests.AddRange(dataExpMests);
                                }
                                CreateThread(_impMestIds);

                                start += 100;
                                count -= 100;
                            }
                        }

                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000101.PDO.Mps000101PDO mps000101RDO = new MPS.Processor.Mps000101.PDO.Mps000101PDO(
                            this._ImpMestMedi_HTs,
                        aggrImpMest,
                        this._Department,
                        this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                        MPS.Processor.Mps000101.PDO.IsTittle101.TamThan,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                          IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         this._ImpMests_Print,
                         this._MobaExpMests
                        );

                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000101RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000101RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    if (_ImpMestMedi_GNs != null && _ImpMestMedi_GNs.Count > 0)
                    {
                        WaitingManager.Show();
                        if (this.listAggrImpMest != null)
                        {
                            if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                            }
                            else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                                aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                            }
                        }
                        if (this.listAggrImpMest1 != null)
                        {
                            if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                                aggrImpMest = aggrImpMest2;
                            }
                            else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                            {
                                var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                                aggrImpMest = aggrImpMest2;
                            }
                        }
                        this._MobaExpMests = new List<HIS_EXP_MEST>();
                        if (this._ImpMests_Print != null && this._ImpMests_Print.Count > 0)
                        {
                            int start = 0;
                            int count = this._ImpMests_Print.Count;
                            while (count > 0)
                            {
                                int limit = (count <= 100) ? count : 100;
                                var listSub = this._ImpMests_Print.Skip(start).Take(limit).ToList();
                                List<long> _impMestIds = new List<long>();
                                _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                                List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                                CommonParam param = new CommonParam();
                                MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                                expMestFilter.IDs = _MobaExpMestIds;
                                var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                                if (dataExpMests != null && dataExpMests.Count > 0)
                                {
                                    this._MobaExpMests.AddRange(dataExpMests);
                                }
                                CreateThread(_impMestIds);

                                start += 100;
                                count -= 100;
                            }
                        }


                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000101.PDO.Mps000101PDO mps000101RDO = new MPS.Processor.Mps000101.PDO.Mps000101PDO(
                            this._ImpMestMedi_GNs,
                        aggrImpMest,
                        this._Department,
                        this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                        MPS.Processor.Mps000101.PDO.IsTittle101.GayNghien,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                          IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         this._ImpMests_Print,
                         this._MobaExpMests
                        );

                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000101RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000101RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000100(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.listAggrImpMest != null)
                {
                    if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                    {
                        var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                        aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                    }
                    else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                    {
                        var aggrImpMest2 = this.listAggrImpMest.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                        AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>();
                        aggrImpMest = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST>(aggrImpMest2);
                    }
                }

                if (this.listAggrImpMest1 != null)
                {
                    if (_ImpMestMaterials != null && _ImpMestMaterials.Count > 0)
                    {
                        var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMaterials.First().AGGR_IMP_MEST_ID);
                        aggrImpMest = aggrImpMest2;
                    }
                    else if (_ImpMestMedicines != null && _ImpMestMedicines.Count > 0)
                    {
                        var aggrImpMest2 = this.listAggrImpMest1.FirstOrDefault(o => o.ID == _ImpMestMedicines.First().AGGR_IMP_MEST_ID);
                        aggrImpMest = aggrImpMest2;
                    }
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.listAggrImpMest != null ? this.listAggrImpMest.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                
                MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                        null,
                        _ImpMestMaterials,
                        aggrImpMest,
                        this._Department,
                        this.serviceUnitIds,
                        this._useFormIds,
                        this.reqRoomIds,
                        true,
                        true,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000100.PDO.IsTittle.VatTu,
                        BackendDataWorker.Get<V_HIS_ROOM>(),
                        this._MobaExpMests,
                         AppConfigKeys.ProcessOderOption,
                         this.listAggrImpMest1
                        );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineAndMaterial(V_HIS_IMP_MEST currentAggImpMest)
        {
            try
            {
                if (currentAggImpMest == null)
                    throw new Exception("Du lieu rong currentAggImpMest");
                CommonParam param = new CommonParam();
                //
                this._ImpMests_Print = new List<HIS_IMP_MEST>();
                this._MobaExpMests = new List<HIS_EXP_MEST>();
                this._ImpMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMaterials = new List<V_HIS_IMP_MEST_MATERIAL>();
                MOS.Filter.HisImpMestFilter impMestFilter = new HisImpMestFilter();
                impMestFilter.AGGR_IMP_MEST_ID = currentAggImpMest.ID;
                this._ImpMests_Print = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>(HisRequestUriStore.HIS_IMP_MEST_GET, ApiConsumers.MosConsumer, impMestFilter, param);
                if (this._ImpMests_Print != null && this._ImpMests_Print.Count > 0)
                {
                    int start = 0;
                    int count = this._ImpMests_Print.Count;
                    while (count > 0)
                    {
                        int limit = (count <= 100) ? count : 100;
                        var listSub = this._ImpMests_Print.Skip(start).Take(limit).ToList();
                        List<long> _impMestIds = new List<long>();
                        _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                        List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                        MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                        expMestFilter.IDs = _MobaExpMestIds;
                        var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                        if (dataExpMests != null && dataExpMests.Count > 0)
                        {
                            this._MobaExpMests.AddRange(dataExpMests);
                        }
                        CreateThread(_impMestIds);

                        start += 100;
                        count -= 100;
                    }
                }
                else
                    return;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThread(object param)
        {
            Thread threadMedi = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMate = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));

            try
            {
                threadMate.Start(param);
                threadMedi.Start(param);

                threadMedi.Join();
                threadMate.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedi.Abort();
                threadMate.Abort();
            }
        }

        private void LoadDataMaterialNewThread(object obj)
        {
            try
            {
                LoadDataMaterial((List<long>)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineNewThread(object obj)
        {
            try
            {
                LoadDataMedicine((List<long>)obj);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(List<long> impMestIds)
        {
            try
            {
                //Thuoc
                CommonParam param = new CommonParam();
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.IMP_MEST_IDs = impMestIds;
                var datas = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);

                if (datas != null && datas.Count > 0)
                {
                    this._useFormIds.AddRange(BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(p => datas.Select(o => o.MEDICINE_TYPE_ID).Contains(p.ID)).Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                    this.serviceUnitIds.AddRange(datas.Select(p => p.SERVICE_UNIT_ID).ToList());
                    this.reqRoomIds.AddRange(datas.Select(p => p.REQ_ROOM_ID ?? 0).ToList());
                    this._ImpMestMedicines.AddRange(datas);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(List<long> impMestIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                //Vat tu
                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_IDs = impMestIds;
                var datas = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);

                if (datas != null && datas.Count > 0)
                {
                    serviceUnitIds.AddRange(datas.Select(p => p.SERVICE_UNIT_ID).ToList());
                    reqRoomIds.AddRange(datas.Select(p => p.REQ_ROOM_ID ?? 0).ToList());
                    _ImpMestMaterials.AddRange(datas);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessorMedicneGNHT()
        {
            try
            {
                _ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMedi_GN_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMedi_PXs = new List<V_HIS_IMP_MEST_MEDICINE>();
                _ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

                if (this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                {
                    var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                    var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                    bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                    bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                    bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                    bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);

                    this._ImpMestMedi_Ts = this._ImpMestMedicines.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                    //p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                    this._ImpMestMedi_GN_HTs = this._ImpMestMedicines.Where(p =>
                       (p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                       || (p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)).ToList();
                    this._ImpMestMedi_GNs = this._ImpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                    this._ImpMestMedi_HTs = this._ImpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                    this._ImpMestMedi_TDs = this._ImpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc).ToList();
                    this._ImpMestMedi_PXs = this._ImpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px).ToList();

                    this._ImpMestMedi_Others = this._ImpMestMedicines.Where(p => mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0) &&
                        p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
