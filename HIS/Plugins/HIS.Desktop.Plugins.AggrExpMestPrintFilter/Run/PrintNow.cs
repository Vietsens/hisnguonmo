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
using HIS.Desktop.LocalStorage.HisConfig;
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

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter.Run
{
    public class PrintNow
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        long TimeFilterOption;
        public PrintNow(Inventec.Desktop.Common.Modules.Module _currentModule, long? chooseTimeType = null)
        {
            this.currentModule = _currentModule;
            TimeFilterOption = chooseTimeType.HasValue ? chooseTimeType.Value : 1;
        }

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;

        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Ts = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_GNs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_HTs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_TDs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_PXs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Other = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMediHCHT = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMediHCGN = new List<V_HIS_EXP_MEST_MEDICINE>();
        internal List<long> serviceUnitIds = new List<long>();
        internal List<long> useFormIds = new List<long>();
        internal List<long> reqRoomIds = new List<long>();

        internal List<V_HIS_EXP_MEST> _AggrExpMests;
        internal HIS_DEPARTMENT _Department;
        internal long _DepartmentId = 0;
        long configKeyMERGER_DATA = 0;
        long configKeyOderOption = 0;
        internal bool printNow;
        bool EmrSign;
        bool EmrSignAndPrint;
        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;
        private int TotalMediMatePrint;
        private int CountMediMatePrinted;
        private bool CancelPrint;
        private const int TIME_OUT_PRINT_MERGE = 1200;
        internal Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata;
        internal Inventec.Common.Print.FlexCelPrintProcessor printProcess;
        bool IsPrintMps169;
        public void DisposePrintNow()
        {
            try
            {
                richEditorMain = null;
                _ExpMestMedi_Ts = null;
                _ExpMestMedi_GNs = null;
                _ExpMestMedi_HTs = null;
                _ExpMestMedi_TDs = null;
                _ExpMestMedi_PXs = null;
                _ExpMestMedi_Other = null;
                _AggrExpMests = null;
                GroupStreamPrint = null;
                adodata = null;
                printProcess = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void RunPrintNow(long _roomId, List<V_HIS_EXP_MEST> _aggrExpMests, long type, Desktop.ADO.AggrExpMestPrintSDO printSdo)
        {
            try
            {
                if (printSdo != null)
                {
                    this.printNow = printSdo.PrintNow ?? false;
                    this.EmrSign = printSdo.EmrSignNow ?? false;
                    this.EmrSignAndPrint = printSdo.EmrSignAndPrintNow ?? false;
                }

                if (AppConfigKeys.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                }

                ProcessPrint(_roomId, _aggrExpMests, type);

                if (AppConfigKeys.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    int countTimeOut = 0;
                    while (this.TotalMediMatePrint != this.CountMediMatePrinted && countTimeOut < TIME_OUT_PRINT_MERGE && !CancelPrint)
                    {
                        Thread.Sleep(50);
                        countTimeOut++;
                    }

                    if (countTimeOut > TIME_OUT_PRINT_MERGE)
                    {
                        throw new Exception("TimeOut");
                    }
                    if (CancelPrint)
                    {
                        throw new Exception("Cancel Print");
                    }

                    adodata = this.GroupStreamPrint.First();
                    Inventec.Common.Logging.LogSystem.Debug("List MPS Group: " + string.Join("; ", this.GroupStreamPrint.Select(s => s.printTypeCode).Distinct()));
                    Inventec.Common.Logging.LogSystem.Debug("List Group count: " + this.GroupStreamPrint.Count);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.GroupStreamPrint), this.GroupStreamPrint));
                    printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(adodata.saveMemoryStream, adodata.printerName, adodata.fileName, adodata.numCopy, true,
                        adodata.isAllowExport, adodata.TemplateKey, adodata.eventLog, adodata.eventPrint, adodata.EmrInputADO, adodata.PrintLog, adodata.ShowPrintLog, adodata.IsAllowEditTemplateFile, adodata.IsSingleCopy);
                    printProcess.SetPartialFile(this.GroupStreamPrint);
                    printProcess.PrintPreviewShow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataGroup(int count, Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo data)
        {
            try
            {
                this.CountMediMatePrinted += count;
                this.GroupStreamPrint.Add(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint(long _roomId, List<V_HIS_EXP_MEST> _aggrExpMests, long type)
        {
            try
            {
                this._DepartmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == _roomId).DepartmentId;
                this._AggrExpMests = _aggrExpMests.OrderBy(o => o.EXP_MEST_CODE).ToList();

                this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                this.richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                switch (type)
                {
                    case 3:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__THUOC_VAT_TU__MPS000049, DelegateRunPrinter);
                        break;
                    case 4:
                        richEditorMain.RunPrintTemplate("Mps000235", DelegateRunPrinter);
                        break;
                    case 5:
                        richEditorMain.RunPrintTemplate("Mps000247", DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CancelChooseTemplate(string printTypeCode)
        {
            try
            {
                this.CancelPrint = true;
                Inventec.Common.Logging.LogSystem.Info("CancelPrint: " + printTypeCode);
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
                long departmentId = (_AggrExpMests.FirstOrDefault().REQ_DEPARTMENT_ID);
                if (departmentId <= 0)
                {
                    departmentId = this._DepartmentId;
                }
                this._Department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__THUOC_VAT_TU__MPS000049:
                        LoadPhieuLinhThuocGNHT(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000235":
                        InTheoBenhNhan2497(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000247":
                        InTraDoiTongHop6282(printTypeCode, fileName, ref result, false, null, null, null, TimeFilterOption);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadPhieuLinhThuocGNHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                this.configKeyOderOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"));
                WaitingManager.Show();
                MPS.Processor.Mps000049.PDO.Mps000049Config mpsConfig49 = new MPS.Processor.Mps000049.PDO.Mps000049Config();
                mpsConfig49._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                mpsConfig49._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                mpsConfig49._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                mpsConfig49.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;
                mpsConfig49._ConfigKeyOderOption = this.configKeyOderOption;

                MPS.Processor.Mps000169.PDO.Mps000169Config mpsConfig169 = new MPS.Processor.Mps000169.PDO.Mps000169Config();
                mpsConfig169._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                mpsConfig169._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                mpsConfig169._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                mpsConfig169.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;
                mpsConfig169._ConfigKeyOderOption = this.configKeyOderOption;

                MPS.Processor.Mps000325.PDO.Mps000325Config mpsConfig325 = new MPS.Processor.Mps000325.PDO.Mps000325Config();
                mpsConfig169._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                mpsConfig169._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                mpsConfig169._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                mpsConfig169.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;
                mpsConfig169._ConfigKeyOderOption = this.configKeyOderOption;


                LoadDataMedicineAndMaterial(this._AggrExpMests, TimeFilterOption);

                Dictionary<string, ADO.MediMatePrintADO> DicDataPrint = new Dictionary<string, ADO.MediMatePrintADO>();

                if (AppConfigKeys.ListParentMedicine != null && AppConfigKeys.ListParentMedicine.Count > 0)
                {
                    List<V_HIS_MEDICINE_TYPE> listParentMedicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => AppConfigKeys.ListParentMedicine.Contains(o.MEDICINE_TYPE_CODE)).ToList();
                    List<V_HIS_MATERIAL_TYPE> listParentMaterial = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => AppConfigKeys.ListParentMedicine.Contains(o.MATERIAL_TYPE_CODE)).ToList();

                    if (listParentMedicine != null && listParentMedicine.Count > 0 && this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                    {
                        var listExpMetyIds = this._ExpMestMedicines.Select(s => s.MEDICINE_TYPE_ID).Distinct().ToList();
                        List<V_HIS_MEDICINE_TYPE> listExpMety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => listExpMetyIds.Contains(o.ID)).ToList();
                        if (listExpMety != null && listExpMety.Count > 0)
                        {
                            var lstExpMetyChild = listExpMety.Where(o => listParentMedicine.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            var lstExpMetyNotChild = listExpMety.Where(o => !listParentMedicine.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            if (lstExpMetyNotChild != null && lstExpMetyNotChild.Count > 0)
                            {
                                if (this._AggrExpMests.Count > 1)
                                {
                                    #region stock
                                    var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                    foreach (var stock in mediStockGroup)
                                    {
                                        var expMestMedicineTypeNotChild = this._ExpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                                        var expMestP = this._ExpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();

                                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                        if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                        if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                        ado._ExpMests_Print.AddRange(expMestP);

                                        if (ado._ExpMestMedicines == null) ado._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                                        ado._ExpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                        DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region old
                                    var expMestMedicineTypeNotChild = this._ExpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                    var expMestP = this._ExpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                    if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                    if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                    ado._ExpMests_Print.AddRange(expMestP);

                                    if (ado._ExpMestMedicines == null) ado._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                                    ado._ExpMestMedicines.AddRange(expMestMedicineTypeNotChild);

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
                                        if (this._AggrExpMests.Count > 1)
                                        {
                                            #region stock
                                            var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                            foreach (var stock in mediStockGroup)
                                            {
                                                var expMestMedicineTypeChild = this._ExpMestMedicines.Where(o => groupParent.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                                                var expMestP = this._ExpMests_Print.Where(o => expMestMedicineTypeChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();

                                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                                ado._ExpMestMedicines = expMestMedicineTypeChild;
                                                ado._ExpMests_Print = expMestP;

                                                DicDataPrint[stock.First().MEDI_STOCK_CODE + "_" + item.MEDICINE_TYPE_CODE] = ado;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            var expMestMedicineTypeChild = this._ExpMestMedicines.Where(o => groupParent.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                            var expMestP = this._ExpMests_Print.Where(o => expMestMedicineTypeChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                            ado._ExpMestMedicines = expMestMedicineTypeChild;
                                            ado._ExpMests_Print = expMestP;
                                            DicDataPrint[item.MEDICINE_TYPE_CODE ?? " "] = ado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                    {
                        if (this._AggrExpMests.Count > 1)
                        {
                            #region stock
                            var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                            foreach (var stock in mediStockGroup)
                            {
                                var expMestMedicineTypeNotChild = this._ExpMestMedicines.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                                var expMestP = this._ExpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                ado._ExpMests_Print.AddRange(expMestP);

                                if (ado._ExpMestMedicines == null) ado._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                                ado._ExpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                            }
                            #endregion
                        }
                        else
                        {
                            #region old
                            var expMestP = this._ExpMests_Print.Where(o => this._ExpMestMedicines.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                            if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                            ado._ExpMests_Print.AddRange(expMestP);

                            if (ado._ExpMestMedicines == null) ado._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                            ado._ExpMestMedicines.AddRange(_ExpMestMedicines);

                            DicDataPrint[" "] = ado;
                            #endregion
                        }
                    }

                    if (listParentMaterial != null && listParentMaterial.Count > 0 && this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                    {
                        var listExpMatyIds = this._ExpMestMaterials.Select(s => s.MATERIAL_TYPE_ID).Distinct().ToList();
                        List<V_HIS_MATERIAL_TYPE> listExpMaty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => listExpMatyIds.Contains(o.ID)).ToList();
                        if (listExpMaty != null && listExpMaty.Count > 0)
                        {
                            var lstExpMatyChild = listExpMaty.Where(o => listParentMaterial.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            var lstExpMatyNotChild = listExpMaty.Where(o => !listParentMaterial.Select(s => s.ID).Contains(o.PARENT_ID ?? 0)).ToList();
                            if (lstExpMatyNotChild != null && lstExpMatyNotChild.Count > 0)
                            {
                                if (this._AggrExpMests.Count > 1)
                                {
                                    #region stock
                                    var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                    foreach (var stock in mediStockGroup)
                                    {
                                        var expMestMaterialTypeNotChild = this._ExpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                                        var expMestP = this._ExpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();

                                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                        if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                        if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                        ado._ExpMests_Print.AddRange(expMestP);

                                        if (ado._ExpMestMaterials == null) ado._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                                        ado._ExpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                        DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region old
                                    var expMestMaterialTypeNotChild = this._ExpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                    var expMestP = this._ExpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                    if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                    if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                    ado._ExpMests_Print.AddRange(expMestP);

                                    if (ado._ExpMestMaterials == null) ado._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                                    ado._ExpMestMaterials.AddRange(expMestMaterialTypeNotChild);

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
                                        if (this._AggrExpMests.Count > 1)
                                        {
                                            #region stock
                                            var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                                            foreach (var stock in mediStockGroup)
                                            {
                                                var expMestMaterialTypeChild = this._ExpMestMaterials.Where(o => groupParent.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                                                var expMestP = this._ExpMests_Print.Where(o => expMestMaterialTypeChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();

                                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                                ado._ExpMestMaterials = expMestMaterialTypeChild;
                                                ado._ExpMests_Print = expMestP;

                                                DicDataPrint[stock.First().MEDI_STOCK_CODE + "_" + item.MATERIAL_TYPE_CODE] = ado;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            var expMestMaterialTypeChild = this._ExpMestMaterials.Where(o => groupParent.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                            var expMestP = this._ExpMests_Print.Where(o => expMestMaterialTypeChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();

                                            ado._ExpMestMaterials = expMestMaterialTypeChild;
                                            ado._ExpMests_Print = expMestP;
                                            DicDataPrint[item.MATERIAL_TYPE_CODE ?? " "] = ado;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                    {
                        if (this._AggrExpMests.Count > 1)
                        {
                            #region stock
                            var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                            foreach (var stock in mediStockGroup)
                            {
                                var expMestMaterialTypeNotChild = this._ExpMestMaterials.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                                var expMestP = this._ExpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID) && stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(stock.First().MEDI_STOCK_CODE)) ado = DicDataPrint[stock.First().MEDI_STOCK_CODE];

                                if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                ado._ExpMests_Print.AddRange(expMestP);

                                if (ado._ExpMestMaterials == null) ado._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                                ado._ExpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                            }
                            #endregion
                        }
                        else
                        {
                            #region old
                            var expMestP = this._ExpMests_Print.Where(o => this._ExpMestMaterials.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                            if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                            ado._ExpMests_Print.AddRange(expMestP);

                            if (ado._ExpMestMaterials == null) ado._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                            ado._ExpMestMaterials.AddRange(this._ExpMestMaterials);

                            DicDataPrint[" "] = ado;
                            #endregion
                        }
                    }
                }
                else
                {
                    DicDataPrint.Clear();
                    if (this._AggrExpMests.Count > 1)
                    {
                        var mediStockGroup = this._AggrExpMests.GroupBy(o => o.MEDI_STOCK_ID).ToList();
                        foreach (var stock in mediStockGroup)
                        {
                            ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                            ado._ExpMests_Print = this._ExpMests_Print.Where(o => stock.Select(s => s.ID).Contains(o.AGGR_EXP_MEST_ID ?? 0)).ToList();
                            if (ado._ExpMests_Print == null || ado._ExpMests_Print.Count <= 0)
                            {
                                continue;
                            }

                            ado._ExpMestMaterials = this._ExpMestMaterials.Where(o => ado._ExpMests_Print.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                            ado._ExpMestMedicines = this._ExpMestMedicines.Where(o => ado._ExpMests_Print.Select(s => s.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                            DicDataPrint[stock.First().MEDI_STOCK_CODE] = ado;
                        }
                    }
                    else
                    {
                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                        ado._ExpMestMaterials = this._ExpMestMaterials;
                        ado._ExpMestMedicines = this._ExpMestMedicines;
                        ado._ExpMests_Print = this._ExpMests_Print;
                        DicDataPrint[" "] = ado;
                    }
                }

                foreach (var item in DicDataPrint)
                {
                    if (item.Value != null && item.Value._ExpMestMaterials != null)
                    {
                        this.TotalMediMatePrint += item.Value._ExpMestMaterials.Count;
                    }

                    if (item.Value != null && item.Value._ExpMestMedicines != null)
                    {
                        this.TotalMediMatePrint += item.Value._ExpMestMedicines.Count;
                    }
                }

                foreach (var item in DicDataPrint)
                {
                    this._ExpMests_Print = item.Value._ExpMests_Print.Distinct().ToList();
                    this._ExpMestMedicines = item.Value._ExpMestMedicines;
                    this._ExpMestMaterials = item.Value._ExpMestMaterials;

                    mpsConfig49.PARENT_TYPE_CODE = item.Key;


                    {
                        #region Tach Thuoc GN - HT - TT
                        ProcessorMedicneGNHT();
                        #endregion

                        if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000175", DelegateRunMps);
                        }
                        IsPrintMps169 = false;
                        if ((this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0) || (this._ExpMestMedi_HTs != null && this._ExpMestMedi_HTs.Count > 0) || (this._ExpMestMediHCGN.Count > 0 || this._ExpMestMediHCHT.Count > 0))
                        {
                            richEditorMain.RunPrintTemplate("Mps000169", DelegateRunMps);
                        }

                        if (this._ExpMestMedi_TDs != null && this._ExpMestMedi_TDs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000236", DelegateRunMps);
                        }

                        if (this._ExpMestMedi_PXs != null && this._ExpMestMedi_PXs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000239", DelegateRunMps);
                        }

                        if (this._ExpMestMedi_Other != null && this._ExpMestMedi_Other.Count > 0)
                        {
                            var groups = _ExpMestMedi_Other.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            if (IsPrintMps169)
                                groups = _ExpMestMedi_Other.Where(o => o.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).GroupBy(o => o.MEDICINE_GROUP_ID).ToList();


                            foreach (var gr in groups)
                            {
                                MPS.Processor.Mps000049.PDO.keyTitles title = new MPS.Processor.Mps000049.PDO.keyTitles();
                                if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)
                                {
                                    title = MPS.Processor.Mps000049.PDO.keyTitles.Corticoid;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN)
                                {
                                    title = MPS.Processor.Mps000049.PDO.keyTitles.DichTruyen;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                {
                                    title = MPS.Processor.Mps000049.PDO.keyTitles.KhangSinh;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    title = MPS.Processor.Mps000049.PDO.keyTitles.Lao;
                                }
                                if (!IsPrintMps169 && gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                                {
                                    title = MPS.Processor.Mps000049.PDO.keyTitles.TienChat;
                                }

                                var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == gr.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                                MPS.Processor.Mps000049.PDO.Mps000049PDO mps000049RDO = new MPS.Processor.Mps000049.PDO.Mps000049PDO(
                                 gr.ToList(),
                                null,
                                aggrExpMests,
                                this._ExpMests_Print,
                                this._Department,
                                serviceUnitIds,
                                useFormIds,
                                reqRoomIds,
                                true,
                                false,
                                false,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                title,
                                mpsConfig49,
                                this._AggrExpMests
                            );

                                WaitingManager.Hide();
                                Print.PrintData(printTypeCode, fileName, mps000049RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, gr.ToList().Count, SetDataGroup);
                            }
                        }


                        #region In Thuoc Thuong
                        List<V_HIS_EXP_MEST_MEDICINE> dtMedicine = _ExpMestMedi_Ts;
                        if (_ExpMestMedi_Ts != null && _ExpMestMedi_Ts.Count > 0)
                        {
                            if (IsPrintMps169)
                                dtMedicine = _ExpMestMedi_Ts.Where(o => o.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).ToList();
                            var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMedi_Ts.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                            MPS.Processor.Mps000049.PDO.Mps000049PDO mps000049RDO = new MPS.Processor.Mps000049.PDO.Mps000049PDO(
                             dtMedicine,
                            null,
                            aggrExpMests,
                            this._ExpMests_Print,
                            this._Department,
                            serviceUnitIds,
                            useFormIds,
                            reqRoomIds,
                            true,
                            false,
                            false,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            MPS.Processor.Mps000049.PDO.keyTitles.phieuLinhThuocThuong,
                            mpsConfig49,
                            this._AggrExpMests
                        );

                            WaitingManager.Hide();
                            Print.PrintData(printTypeCode, fileName, mps000049RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, dtMedicine.Count, SetDataGroup);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void ProcessorMedicneGNHT()
        {
            try
            {
                _ExpMestMedi_Ts = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_GNs = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_HTs = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_TDs = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_PXs = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_Other = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMediHCGN = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMediHCHT = new List<V_HIS_EXP_MEST_MEDICINE>();

                //ninhdd #32837
                //danh sách thuốc thường sẽ là nhóm thuốc không được chọn in tách
                if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                {
                    // this._ExpMestMedi_Ts = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                    //   == null || p.MEDICINE_GROUP_ID <= 0).ToList();
                    var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                    var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                    bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                    bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                    bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                    bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                    bool hcgn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                    bool hcht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);

                    this._ExpMestMedi_Ts = this._ExpMestMedicines.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                    //p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                    this._ExpMestMedi_GNs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                    this._ExpMestMedi_HTs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                    this._ExpMestMedi_TDs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc).ToList();
                    this._ExpMestMedi_PXs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px).ToList();

                    this._ExpMestMediHCGN = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && hcgn).ToList();
                    this._ExpMestMediHCHT = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                         == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && hcht).ToList();

                    this._ExpMestMedi_Other = this._ExpMestMedicines.Where(p => mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0) &&
                        p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunMps(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000175":
                        Mps000175(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000169":
                        Mps000169(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000236":
                        Mps000236(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000239":
                        Mps000239(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000325":
                        Mps000325(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000175(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    WaitingManager.Show();
                    long configKey = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                    List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMate_VTs = new List<V_HIS_EXP_MEST_MATERIAL>();
                    List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMate_HCs = new List<V_HIS_EXP_MEST_MATERIAL>();
                    foreach (var item in this._ExpMestMaterials)
                    {
                        if (item.IS_CHEMICAL_SUBSTANCE == 1)
                        {
                            _ExpMestMate_HCs.Add(item);
                        }
                        else
                            _ExpMestMate_VTs.Add(item);
                    }

                    MPS.Processor.Mps000175.PDO.Mps000175Config mpsConfig75 = new MPS.Processor.Mps000175.PDO.Mps000175Config();
                    mpsConfig75._ConfigKeyMERGER_DATA = configKey;
                    mpsConfig75._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    mpsConfig75._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                    mpsConfig75.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;

                    if (_ExpMestMate_HCs != null && _ExpMestMate_HCs.Count > 0)
                    {
                        var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMate_HCs.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000175.PDO.Mps000175PDO mps000175PDO = new MPS.Processor.Mps000175.PDO.Mps000175PDO
                   (
                        _ExpMestMate_HCs,
                        aggrExpMests,
                        this._ExpMests_Print,
                        this._Department,
                        this.serviceUnitIds,
                        this.reqRoomIds,
                        MPS.Processor.Mps000175.PDO.keyTitles.phieuLinhHoaChat,
                        mpsConfig75,
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        this._AggrExpMests
                     );
                        WaitingManager.Hide();
                        Print.PrintData(printTypeCode, fileName, mps000175PDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMate_HCs.Count, SetDataGroup);
                    }
                    if (_ExpMestMate_VTs != null && _ExpMestMate_VTs.Count > 0)
                    {
                        var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMate_VTs.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000175.PDO.Mps000175PDO mps000175PDO = new MPS.Processor.Mps000175.PDO.Mps000175PDO
                   (
                     _ExpMestMate_VTs,
                        aggrExpMests,
                        this._ExpMests_Print,
                        this._Department,
                        this.serviceUnitIds,
                        this.reqRoomIds,
                        MPS.Processor.Mps000175.PDO.keyTitles.phieuLinhVatTu,
                        mpsConfig75,
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                        this._AggrExpMests
                     );
                        WaitingManager.Hide();
                        Print.PrintData(printTypeCode, fileName, mps000175PDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMate_VTs.Count, SetDataGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000169(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                this.configKeyOderOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"));

                MPS.Processor.Mps000169.PDO.Mps000169Config mpsConfig169 = new MPS.Processor.Mps000169.PDO.Mps000169Config();
                mpsConfig169._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                mpsConfig169._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                mpsConfig169._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                mpsConfig169.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;
                mpsConfig169._ConfigKeyOderOption = this.configKeyOderOption;

                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region In Tat Ca GN,HT
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    List<V_HIS_EXP_MEST_MEDICINE> DataGroups = new List<V_HIS_EXP_MEST_MEDICINE>();

                    if (_ExpMestMedi_GNs.Count > 0 || _ExpMestMedi_HTs.Count() > 0)
                    {
                        DataGroups.AddRange(this._ExpMestMedi_GNs);
                        DataGroups.AddRange(this._ExpMestMedi_HTs);
                    }
                    var aggrExpMests = (this._AggrExpMests != null && _AggrExpMests.Count > 0 && DataGroups.Count > 0) ? (this._AggrExpMests.FirstOrDefault(o => o.ID == DataGroups.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First()) : null;
                    if (aggrExpMests != null)
                    {
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                        DataGroups,
                        aggrExpMests,
                        this._ExpMests_Print,
                        this._Department,
                        this.serviceUnitIds,
                        this.useFormIds,
                        this.reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinh_GN_HT,
                        mpsConfig169,
                        this._AggrExpMests
                    );
                        WaitingManager.Hide();

                        Print.PrintData(printTypeCode, fileName, mps000169RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, DataGroups.Count, SetDataGroup);
                        #endregion
                    }

                    #region In Tat Ca HCGN,HCHT
                    if (_ExpMestMediHCGN.Count > 0 || _ExpMestMediHCHT.Count() > 0)
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> mediTotalPrint = new List<V_HIS_EXP_MEST_MEDICINE>();
                        mediTotalPrint.AddRange(_ExpMestMediHCGN);
                        mediTotalPrint.AddRange(_ExpMestMediHCHT);
                        var aggrExpMestsHT = (this._AggrExpMests != null && _AggrExpMests.Count > 0 && mediTotalPrint.Count > 0) ? (this._AggrExpMests.FirstOrDefault(o => o.ID == mediTotalPrint.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First()) : null;
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO_HCGNHT = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                        mediTotalPrint,
                        aggrExpMestsHT,
                        this._ExpMests_Print,
                        this._Department,
                        serviceUnitIds,
                        useFormIds,
                        reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinhHCGN_HCHT,
                        mpsConfig169
                    );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000169RDO_HCGNHT, this.printNow, inputADO, ref result, this.currentModule.RoomId, false, false, mediTotalPrint.Count, SetDataGroup);
                    }
                    #endregion
                }
                else
                {
                    if ((this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0) || (this._ExpMestMediHCGN != null && this._ExpMestMediHCGN.Count > 0))
                    {
                        richEditorMain.RunPrintTemplate("Mps000325", DelegateRunMps);
                    }

                    if (this._ExpMestMedi_HTs != null && this._ExpMestMedi_HTs.Count > 0)
                    {
                        var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMedi_HTs.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                        this._ExpMestMedi_HTs,
                        aggrExpMests,
                        this._ExpMests_Print,
                        this._Department,
                        this.serviceUnitIds,
                        this.useFormIds,
                        this.reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinhHT,
                        mpsConfig169,
                        this._AggrExpMests
                    );
                        WaitingManager.Hide();
                        Print.PrintData(printTypeCode, fileName, mps000169RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMedi_HTs.Count, SetDataGroup);
                    }

                    if (_ExpMestMediHCHT != null && _ExpMestMediHCHT.Count > 0)
                    {
                        var aggrExpMests = (this._AggrExpMests != null && _AggrExpMests.Count > 0) ? (this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMediHCHT.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First()) : null;
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                         _ExpMestMediHCHT,
                        aggrExpMests,
                        this._ExpMests_Print,
                        this._Department,
                        serviceUnitIds,
                        useFormIds,
                        reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinhHCHT,
                        mpsConfig169
                    );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000169RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, false, false, _ExpMestMediHCHT.Count, SetDataGroup);

                    }

                }

                if (this._ExpMestMedi_Other != null && _ExpMestMedi_Other.Count > 0)
                {
                    var groups = _ExpMestMedi_Other.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                    foreach (var gr in groups)
                    {
                        MPS.Processor.Mps000169.PDO.keyTitles title = new MPS.Processor.Mps000169.PDO.keyTitles();
                        if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                        {
                            title = MPS.Processor.Mps000169.PDO.keyTitles.TienChat;
                            IsPrintMps169 = true;

                            var aggrExpMests = (this._AggrExpMests != null && _AggrExpMests.Count > 0) ? (this._AggrExpMests.FirstOrDefault(o => o.ID == gr.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First()) : null;
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                            MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                            gr.ToList(),
                            aggrExpMests,
                            this._ExpMests_Print,
                            this._Department,
                            this.serviceUnitIds,
                            this.useFormIds,
                            this.reqRoomIds,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            title,
                            mpsConfig169
                        );

                            WaitingManager.Hide();
                            Print.PrintData(printTypeCode, fileName, mps000169RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMedi_HTs.Count, SetDataGroup);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000325(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MPS.Processor.Mps000325.PDO.Mps000325Config mpsConfig325 = new MPS.Processor.Mps000325.PDO.Mps000325Config();
                mpsConfig325._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                mpsConfig325._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                mpsConfig325._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                mpsConfig325.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;

                if (this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0)
                {
                    var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMedi_GNs.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    MPS.Processor.Mps000325.PDO.Mps000325PDO mps000325RDO = new MPS.Processor.Mps000325.PDO.Mps000325PDO(
                    this._ExpMestMedi_GNs,
                    aggrExpMests,
                    this._ExpMests_Print,
                    this._Department,
                    this.serviceUnitIds,
                    this.useFormIds,
                    this.reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    MPS.Processor.Mps000325.PDO.keyTitles.phieuLinhGN,
                    mpsConfig325
                );
                    WaitingManager.Hide();
                    Print.PrintData(printTypeCode, fileName, mps000325RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMedi_GNs.Count, SetDataGroup);
                }

                if (_ExpMestMediHCGN != null && _ExpMestMediHCGN.Count > 0)
                {
                    var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMediHCGN.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    MPS.Processor.Mps000325.PDO.Mps000325PDO Mps000325PDO = new MPS.Processor.Mps000325.PDO.Mps000325PDO(
                    _ExpMestMediHCGN,
                    aggrExpMests,
                    this._ExpMests_Print,
                    this._Department,
                    serviceUnitIds,
                    useFormIds,
                    reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    MPS.Processor.Mps000325.PDO.keyTitles.phieuLinhHCGN,
                    mpsConfig325
                );
                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, Mps000325PDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, false, false, _ExpMestMediHCGN.Count, SetDataGroup);
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000236(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this._ExpMestMedi_TDs != null && this._ExpMestMedi_TDs.Count > 0)
                {
                    var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMedi_TDs.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                    MPS.Processor.Mps000236.PDO.Mps000236Config mpsConfig236 = new MPS.Processor.Mps000236.PDO.Mps000236Config();
                    mpsConfig236._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                    mpsConfig236._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    mpsConfig236._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                    mpsConfig236.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;

                    WaitingManager.Show();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    MPS.Processor.Mps000236.PDO.Mps000236PDO mps000236RDO = new MPS.Processor.Mps000236.PDO.Mps000236PDO(
                    this._ExpMestMedi_TDs,
                    aggrExpMests,
                    this._ExpMests_Print,
                    this._Department,
                    this.serviceUnitIds,
                    this.useFormIds,
                    this.reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    mpsConfig236,
                    this._AggrExpMests
                );
                    WaitingManager.Hide();
                    Print.PrintData(printTypeCode, fileName, mps000236RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMedi_TDs.Count, SetDataGroup);
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000239(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this._ExpMestMedi_PXs != null && this._ExpMestMedi_PXs.Count > 0)
                {
                    var aggrExpMests = this._AggrExpMests.FirstOrDefault(o => o.ID == _ExpMestMedi_PXs.First().AGGR_EXP_MEST_ID) ?? this._AggrExpMests.First();
                    MPS.Processor.Mps000239.PDO.Mps000239Config mpsConfig239 = new MPS.Processor.Mps000239.PDO.Mps000239Config();
                    mpsConfig239._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                    mpsConfig239._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    mpsConfig239._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                    mpsConfig239.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;

                    WaitingManager.Show();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    MPS.Processor.Mps000239.PDO.Mps000239PDO mps000239RDO = new MPS.Processor.Mps000239.PDO.Mps000239PDO(
                    this._ExpMestMedi_PXs,
                    aggrExpMests,
                    this._ExpMests_Print,
                    this._Department,
                    this.serviceUnitIds,
                    this.useFormIds,
                    this.reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    mpsConfig239
                );
                    WaitingManager.Hide();
                    Print.PrintData(printTypeCode, fileName, mps000239RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, _ExpMestMedi_PXs.Count, SetDataGroup);
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InTheoBenhNhan2497(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                WaitingManager.Show();
                //bổ sung thêm danh sách nhiều phiếu tổng hợp
                LoadDataMedicineAndMaterial(this._AggrExpMests, TimeFilterOption);

                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_LINH);

                if (this._ExpMests_Print != null && this._ExpMests_Print.Count > 0)
                {
                    this._ExpMests_Print = this._ExpMests_Print.Where(p => p.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL).ToList();
                    List<HIS_PATIENT_TYPE_ALTER> _DataPatientTypeAlters = new List<HIS_PATIENT_TYPE_ALTER>();
                    List<HIS_TREATMENT> _Datatreatments = new List<HIS_TREATMENT>();
                    Dictionary<long, List<V_HIS_TREATMENT_BED_ROOM>> dictreatmentBedrooms = new Dictionary<long, List<V_HIS_TREATMENT_BED_ROOM>>();
                    List<HIS_SERVICE_REQ> _DataServiceReq = new List<HIS_SERVICE_REQ>();
                    List<V_HIS_PATIENT> _DataPatients = new List<V_HIS_PATIENT>();

                    List<long> listTreatmentIds = this._ExpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                    int skip = 0;
                    while (listTreatmentIds.Count - skip > 0)
                    {
                        var listIds = listTreatmentIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        MOS.Filter.HisPatientTypeAlterFilter patientTypeFilter = new HisPatientTypeAlterFilter();
                        patientTypeFilter.TREATMENT_IDs = listIds;
                        var patientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ALTER>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET, ApiConsumers.MosConsumer, patientTypeFilter, null);
                        if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                        {
                            _DataPatientTypeAlters.AddRange(patientTypeAlters);
                        }

                        MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        treatmentFilter.IDs = listIds;
                        var treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, null);
                        if (treatments != null && treatments.Count > 0)
                        {
                            _Datatreatments.AddRange(treatments);
                        }

                        MOS.Filter.HisTreatmentBedRoomViewFilter treatmentBedRoomFilter = new HisTreatmentBedRoomViewFilter();
                        treatmentBedRoomFilter.TREATMENT_IDs = this._ExpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                        var treatmentBedrooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomFilter, null);
                        if (treatmentBedrooms != null && treatmentBedrooms.Count > 0)
                        {
                            foreach (var item in treatmentBedrooms)
                            {
                                if (!dictreatmentBedrooms.ContainsKey(item.TREATMENT_ID))
                                    dictreatmentBedrooms[item.TREATMENT_ID] = new List<V_HIS_TREATMENT_BED_ROOM>();

                                dictreatmentBedrooms[item.TREATMENT_ID].Add(item);
                                dictreatmentBedrooms[item.TREATMENT_ID] = dictreatmentBedrooms[item.TREATMENT_ID].OrderByDescending(o => o.CREATE_TIME).ToList();
                            }
                        }
                    }

                    List<long> listServiceReqIds = this._ExpMests_Print.Select(p => p.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                    skip = 0;
                    while (listServiceReqIds.Count - skip > 0)
                    {
                        var listIds = listServiceReqIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        MOS.Filter.HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                        reqFilter.IDs = listIds;
                        var serviceReq = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, reqFilter, null);
                        if (serviceReq != null && serviceReq.Count > 0)
                        {
                            _DataServiceReq.AddRange(serviceReq);
                        }
                    }

                    List<long> listPatientIds = this._ExpMests_Print.Select(p => p.TDL_PATIENT_ID ?? 0).Distinct().ToList();
                    skip = 0;
                    while (listPatientIds.Count - skip > 0)
                    {
                        var listIds = listPatientIds.Skip(skip).Take(100).ToList();
                        skip += 100;
                        MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.IDs = listIds;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                        if (patients != null && patients.Count > 0)
                        {
                            _DataPatients.AddRange(patients);
                        }
                    }

                    if (this._ExpMestMaterials != null)
                    {
                        this.TotalMediMatePrint += this._ExpMestMaterials.Count;
                    }

                    if (this._ExpMestMedicines != null)
                    {
                        this.TotalMediMatePrint += this._ExpMestMedicines.Count;
                    }

                    var GroupTreatments = this._ExpMests_Print.GroupBy(p => p.TDL_TREATMENT_ID).ToList();
                    if (keyPrintType == 1)
                    {
                        #region In Tong Hop
                        foreach (var itemGr in GroupTreatments)
                        {
                            var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                            //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                            //xếp theo có thông tin giường hay ko. true lên trước false
                            var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                            var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                            var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => itemGr.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                            var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                            foreach (var grstock in groupMediStockParent)
                            {
                                if (grstock.Key.HasValue)
                                {
                                    #region in theo kho cha
                                    var expMestPrints = itemGr.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                    if (expMestPrints == null || expMestPrints.Count <= 0)
                                    {
                                        continue;
                                    }

                                    var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                    PrintMps000235(printTypeCode, fileName,
                                        this._ExpMestMedicines,
                                        this._ExpMestMaterials,
                                        aggrExpMest,
                                        expMestPrints,
                                        this._Department,
                                        MPS.Processor.Mps000235.PDO.keyTitles.phieuLinhTongHop,
                                        patient,
                                        _DataPatientTypeAlters,
                                        _DataServiceReq,
                                        treatment,
                                        treatmentBedRoom,
                                        inputADO, ref result);
                                    #endregion
                                }
                                else
                                {
                                    foreach (var stock in grstock)
                                    {
                                        #region in theo kho con
                                        var expMestPrints = itemGr.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                        if (expMestPrints == null || expMestPrints.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                        PrintMps000235(printTypeCode, fileName,
                                            this._ExpMestMedicines,
                                            this._ExpMestMaterials,
                                            aggrExpMest,
                                            expMestPrints,
                                            this._Department,
                                            MPS.Processor.Mps000235.PDO.keyTitles.phieuLinhTongHop,
                                            patient,
                                            _DataPatientTypeAlters,
                                            _DataServiceReq,
                                            treatment,
                                            treatmentBedRoom,
                                            inputADO, ref result);
                                        #endregion
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region Tach Thuoc GN - HT - TT
                        ProcessorMedicneGNHT();
                        #endregion

                        #region In Thuoc Thuong
                        if (_ExpMestMedi_Ts != null && _ExpMestMedi_Ts.Count > 0)
                        {
                            List<long> ExpMestIds = _ExpMestMedi_Ts.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                if (expMestCheck.Count <= 0)
                                {
                                    continue;
                                }

                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                //xếp theo có thông tin giường hay ko. true lên trước false
                                var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                foreach (var grstock in groupMediStockParent)
                                {
                                    if (grstock.Key.HasValue)
                                    {
                                        #region in theo kho cha
                                        var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                        if (expMestPrints == null || expMestPrints.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                        PrintMps000235(printTypeCode, fileName,
                                            _ExpMestMedi_Ts,
                                            null,
                                            aggrExpMest,
                                            expMestPrints,
                                            this._Department,
                                            MPS.Processor.Mps000235.PDO.keyTitles.phieuLinhThuocThuong,
                                            patient,
                                            _DataPatientTypeAlters,
                                            _DataServiceReq,
                                            treatment,
                                            treatmentBedRoom,
                                            inputADO, ref result);
                                        #endregion
                                    }
                                    else
                                    {
                                        foreach (var stock in grstock)
                                        {
                                            #region in theo kho con
                                            var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                            if (expMestPrints == null || expMestPrints.Count <= 0)
                                            {
                                                continue;
                                            }

                                            var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                            PrintMps000235(printTypeCode, fileName,
                                                _ExpMestMedi_Ts,
                                                null,
                                                aggrExpMest,
                                                expMestPrints,
                                                this._Department,
                                                MPS.Processor.Mps000235.PDO.keyTitles.phieuLinhThuocThuong,
                                                patient,
                                                _DataPatientTypeAlters,
                                                _DataServiceReq,
                                                treatment,
                                                treatmentBedRoom,
                                                inputADO, ref result);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Vat Tu
                        if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                        {
                            List<long> ExpMestIds = _ExpMestMaterials.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                if (expMestCheck.Count <= 0)
                                {
                                    continue;
                                }

                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                //xếp theo có thông tin giường hay ko. true lên trước false
                                var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                foreach (var grstock in groupMediStockParent)
                                {
                                    if (grstock.Key.HasValue)
                                    {
                                        #region in theo kho cha
                                        var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                        if (expMestPrints == null || expMestPrints.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                        PrintMps000235(printTypeCode, fileName,
                                            null,
                                            _ExpMestMaterials,
                                            aggrExpMest,
                                            expMestPrints,
                                            this._Department,
                                            MPS.Processor.Mps000235.PDO.keyTitles.VatTu,
                                            patient,
                                            _DataPatientTypeAlters,
                                            _DataServiceReq,
                                            treatment,
                                            treatmentBedRoom,
                                            inputADO, ref result);
                                        #endregion
                                    }
                                    else
                                    {
                                        foreach (var stock in grstock)
                                        {
                                            #region in theo kho con
                                            var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                            if (expMestPrints == null || expMestPrints.Count <= 0)
                                            {
                                                continue;
                                            }

                                            var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                            PrintMps000235(printTypeCode, fileName,
                                                null,
                                                _ExpMestMaterials,
                                                aggrExpMest,
                                                expMestPrints,
                                                this._Department,
                                                MPS.Processor.Mps000235.PDO.keyTitles.VatTu,
                                                patient,
                                                _DataPatientTypeAlters,
                                                _DataServiceReq,
                                                treatment,
                                                treatmentBedRoom,
                                                inputADO, ref result);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Gay Nghien Huong Than
                        if ((this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0) || (this._ExpMestMedi_HTs != null && this._ExpMestMedi_HTs.Count > 0))
                        {
                            List<V_HIS_EXP_MEST_MEDICINE> DataGroups = new List<V_HIS_EXP_MEST_MEDICINE>();
                            DataGroups.AddRange(this._ExpMestMedi_GNs);
                            DataGroups.AddRange(this._ExpMestMedi_HTs);
                            long keyPrintTypeHTGN = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                            if (keyPrintTypeHTGN == 1)
                            {
                                List<long> ExpMestIds = DataGroups.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                                foreach (var itemGr in GroupTreatments)
                                {
                                    var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                    if (expMestCheck.Count <= 0)
                                    {
                                        continue;
                                    }

                                    var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                    //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                    //xếp theo có thông tin giường hay ko. true lên trước false
                                    var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                    var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                    var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                    var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                    foreach (var grstock in groupMediStockParent)
                                    {
                                        if (grstock.Key.HasValue)
                                        {
                                            #region in theo kho cha
                                            var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                            if (expMestPrints == null || expMestPrints.Count <= 0)
                                            {
                                                continue;
                                            }

                                            var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                            PrintMps000235(printTypeCode, fileName,
                                                DataGroups,
                                                null,
                                                aggrExpMest,
                                                expMestPrints,
                                                this._Department,
                                                MPS.Processor.Mps000235.PDO.keyTitles.GayNghienHuongThan,
                                                patient,
                                                _DataPatientTypeAlters,
                                                _DataServiceReq,
                                                treatment,
                                                treatmentBedRoom,
                                                inputADO, ref result);
                                            #endregion
                                        }
                                        else
                                        {
                                            foreach (var stock in grstock)
                                            {
                                                #region in theo kho con
                                                var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                                if (expMestPrints == null || expMestPrints.Count <= 0)
                                                {
                                                    continue;
                                                }

                                                var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                                PrintMps000235(printTypeCode, fileName,
                                                    DataGroups,
                                                    null,
                                                    aggrExpMest,
                                                    expMestPrints,
                                                    this._Department,
                                                    MPS.Processor.Mps000235.PDO.keyTitles.GayNghienHuongThan,
                                                    patient,
                                                    _DataPatientTypeAlters,
                                                    _DataServiceReq,
                                                    treatment,
                                                    treatmentBedRoom,
                                                    inputADO, ref result);
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0)
                                {
                                    List<long> ExpMestIds = _ExpMestMedi_GNs.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                                    foreach (var itemGr in GroupTreatments)
                                    {
                                        var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                        if (expMestCheck.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                        //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                        //xếp theo có thông tin giường hay ko. true lên trước false
                                        var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                        var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                        var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                        var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                        foreach (var grstock in groupMediStockParent)
                                        {
                                            if (grstock.Key.HasValue)
                                            {
                                                #region in theo kho cha
                                                var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                                if (expMestPrints == null || expMestPrints.Count <= 0)
                                                {
                                                    continue;
                                                }

                                                var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                                PrintMps000235(printTypeCode, fileName,
                                                    _ExpMestMedi_GNs,
                                                    null,
                                                    aggrExpMest,
                                                    expMestPrints,
                                                    this._Department,
                                                    MPS.Processor.Mps000235.PDO.keyTitles.GayNghien,
                                                    patient,
                                                    _DataPatientTypeAlters,
                                                    _DataServiceReq,
                                                    treatment,
                                                    treatmentBedRoom,
                                                    inputADO, ref result);
                                                #endregion
                                            }
                                            else
                                            {
                                                foreach (var stock in grstock)
                                                {
                                                    #region in theo kho con
                                                    var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                                    if (expMestPrints == null || expMestPrints.Count <= 0)
                                                    {
                                                        continue;
                                                    }

                                                    var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                                    PrintMps000235(printTypeCode, fileName,
                                                        _ExpMestMedi_GNs,
                                                        null,
                                                        aggrExpMest,
                                                        expMestPrints,
                                                        this._Department,
                                                        MPS.Processor.Mps000235.PDO.keyTitles.GayNghien,
                                                        patient,
                                                        _DataPatientTypeAlters,
                                                        _DataServiceReq,
                                                        treatment,
                                                        treatmentBedRoom,
                                                        inputADO, ref result);
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                }

                                if (this._ExpMestMedi_HTs != null && this._ExpMestMedi_HTs.Count > 0)
                                {
                                    List<long> ExpMestIds = _ExpMestMedi_HTs.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                                    foreach (var itemGr in GroupTreatments)
                                    {
                                        var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                        if (expMestCheck.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                        //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                        //xếp theo có thông tin giường hay ko. true lên trước false
                                        var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                        var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                        var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                        var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                        foreach (var grstock in groupMediStockParent)
                                        {
                                            if (grstock.Key.HasValue)
                                            {
                                                #region in theo kho cha
                                                var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                                if (expMestPrints == null || expMestPrints.Count <= 0)
                                                {
                                                    continue;
                                                }

                                                var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                                PrintMps000235(printTypeCode, fileName,
                                                    _ExpMestMedi_HTs,
                                                    null,
                                                    aggrExpMest,
                                                    expMestPrints,
                                                    this._Department,
                                                    MPS.Processor.Mps000235.PDO.keyTitles.HuongThan,
                                                    patient,
                                                    _DataPatientTypeAlters,
                                                    _DataServiceReq,
                                                    treatment,
                                                    treatmentBedRoom,
                                                    inputADO, ref result);
                                                #endregion
                                            }
                                            else
                                            {
                                                foreach (var stock in grstock)
                                                {
                                                    #region in theo kho con
                                                    var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                                    if (expMestPrints == null || expMestPrints.Count <= 0)
                                                    {
                                                        continue;
                                                    }

                                                    var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                                    PrintMps000235(printTypeCode, fileName,
                                                        _ExpMestMedi_HTs,
                                                        null,
                                                        aggrExpMest,
                                                        expMestPrints,
                                                        this._Department,
                                                        MPS.Processor.Mps000235.PDO.keyTitles.HuongThan,
                                                        patient,
                                                        _DataPatientTypeAlters,
                                                        _DataServiceReq,
                                                        treatment,
                                                        treatmentBedRoom,
                                                        inputADO, ref result);
                                                    #endregion
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Thuoc Doc
                        if (this._ExpMestMedi_TDs != null && this._ExpMestMedi_TDs.Count > 0)
                        {
                            List<long> ExpMestIds = _ExpMestMedi_TDs.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                if (expMestCheck.Count <= 0)
                                {
                                    continue;
                                }

                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                //xếp theo có thông tin giường hay ko. true lên trước false
                                var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                foreach (var grstock in groupMediStockParent)
                                {
                                    if (grstock.Key.HasValue)
                                    {
                                        #region in theo kho cha
                                        var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                        if (expMestPrints == null || expMestPrints.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                        PrintMps000235(printTypeCode, fileName,
                                            _ExpMestMedi_TDs,
                                            null,
                                            aggrExpMest,
                                            expMestPrints,
                                            this._Department,
                                            MPS.Processor.Mps000235.PDO.keyTitles.ThuocDoc,
                                            patient,
                                            _DataPatientTypeAlters,
                                            _DataServiceReq,
                                            treatment,
                                            treatmentBedRoom,
                                            inputADO, ref result);
                                        #endregion
                                    }
                                    else
                                    {
                                        foreach (var stock in grstock)
                                        {
                                            #region in theo kho con
                                            var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                            if (expMestPrints == null || expMestPrints.Count <= 0)
                                            {
                                                continue;
                                            }

                                            var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                            PrintMps000235(printTypeCode, fileName,
                                                _ExpMestMedi_TDs,
                                                null,
                                                aggrExpMest,
                                                expMestPrints,
                                                this._Department,
                                                MPS.Processor.Mps000235.PDO.keyTitles.ThuocDoc,
                                                patient,
                                                _DataPatientTypeAlters,
                                                _DataServiceReq,
                                                treatment,
                                                treatmentBedRoom,
                                                inputADO, ref result);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region Thuoc Phong Xa
                        if (this._ExpMestMedi_PXs != null && this._ExpMestMedi_PXs.Count > 0)
                        {
                            List<long> ExpMestIds = _ExpMestMedi_PXs.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                            foreach (var itemGr in GroupTreatments)
                            {
                                var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                if (expMestCheck.Count <= 0)
                                {
                                    continue;
                                }

                                var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                //xếp theo có thông tin giường hay ko. true lên trước false
                                var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                foreach (var grstock in groupMediStockParent)
                                {
                                    if (grstock.Key.HasValue)
                                    {
                                        #region in theo kho cha
                                        var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                        if (expMestPrints == null || expMestPrints.Count <= 0)
                                        {
                                            continue;
                                        }

                                        var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                        PrintMps000235(printTypeCode, fileName,
                                            _ExpMestMedi_PXs,
                                            null,
                                            aggrExpMest,
                                            expMestPrints,
                                            this._Department,
                                            MPS.Processor.Mps000235.PDO.keyTitles.PhongXa,
                                            patient,
                                            _DataPatientTypeAlters,
                                            _DataServiceReq,
                                            treatment,
                                            treatmentBedRoom,
                                            inputADO, ref result);
                                        #endregion
                                    }
                                    else
                                    {
                                        foreach (var stock in grstock)
                                        {
                                            #region in theo kho con
                                            var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                            if (expMestPrints == null || expMestPrints.Count <= 0)
                                            {
                                                continue;
                                            }

                                            var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                            PrintMps000235(printTypeCode, fileName,
                                                _ExpMestMedi_PXs,
                                                null,
                                                aggrExpMest,
                                                expMestPrints,
                                                this._Department,
                                                MPS.Processor.Mps000235.PDO.keyTitles.PhongXa,
                                                patient,
                                                _DataPatientTypeAlters,
                                                _DataServiceReq,
                                                treatment,
                                                treatmentBedRoom,
                                                inputADO, ref result);
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region khac
                        if (this._ExpMestMedi_Other != null && this._ExpMestMedi_Other.Count > 0)
                        {
                            var groups = _ExpMestMedi_Other.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            foreach (var gr in groups)
                            {
                                MPS.Processor.Mps000235.PDO.keyTitles title = new MPS.Processor.Mps000235.PDO.keyTitles();
                                if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)
                                {
                                    title = MPS.Processor.Mps000235.PDO.keyTitles.Corticoid;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN)
                                {
                                    title = MPS.Processor.Mps000235.PDO.keyTitles.DichTruyen;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                {
                                    title = MPS.Processor.Mps000235.PDO.keyTitles.KhangSinh;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    title = MPS.Processor.Mps000235.PDO.keyTitles.Lao;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                                {
                                    title = MPS.Processor.Mps000235.PDO.keyTitles.TienChat;
                                }

                                List<long> ExpMestIds = gr.Select(p => p.EXP_MEST_ID ?? 0).ToList();
                                foreach (var itemGr in GroupTreatments)
                                {
                                    var expMestCheck = itemGr.Where(p => ExpMestIds.Contains(p.ID)).ToList();
                                    if (expMestCheck.Count <= 0)
                                    {
                                        continue;
                                    }

                                    var treatment = _Datatreatments.FirstOrDefault(p => p.ID == itemGr.Key);
                                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : itemGr.First().TDL_TREATMENT_CODE), printTypeCode, this.currentModule.RoomId);

                                    //xếp theo thời gian ra buồng. chưa ra thì là 0 bé nhất.
                                    //xếp theo có thông tin giường hay ko. true lên trước false
                                    var treatmentBedRoom = dictreatmentBedrooms.ContainsKey(treatment.ID) ? dictreatmentBedrooms[treatment.ID].OrderBy(o => o.OUT_TIME ?? 0).ThenByDescending(o => o.BED_ID.HasValue).FirstOrDefault() : null;
                                    var patient = treatment != null ? _DataPatients.FirstOrDefault(p => p.ID == treatment.PATIENT_ID) : null;

                                    var listMediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => expMestCheck.Select(s => s.MEDI_STOCK_ID).Contains(o.ID)).ToList();
                                    var groupMediStockParent = listMediStock.GroupBy(o => o.PARENT_ID).ToList();
                                    foreach (var grstock in groupMediStockParent)
                                    {
                                        if (grstock.Key.HasValue)
                                        {
                                            #region in theo kho cha
                                            var expMestPrints = expMestCheck.Where(o => grstock.Select(s => s.ID).Contains(o.MEDI_STOCK_ID)).ToList();
                                            if (expMestPrints == null || expMestPrints.Count <= 0)
                                            {
                                                continue;
                                            }

                                            var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                            PrintMps000235(printTypeCode, fileName,
                                                gr.ToList(),
                                                null,
                                                aggrExpMest,
                                                expMestPrints,
                                                this._Department,
                                                title,
                                                patient,
                                                _DataPatientTypeAlters,
                                                _DataServiceReq,
                                                treatment,
                                                treatmentBedRoom,
                                                inputADO, ref result);
                                            #endregion
                                        }
                                        else
                                        {
                                            foreach (var stock in grstock)
                                            {
                                                #region in theo kho con
                                                var expMestPrints = expMestCheck.Where(o => stock.ID == o.MEDI_STOCK_ID).ToList();
                                                if (expMestPrints == null || expMestPrints.Count <= 0)
                                                {
                                                    continue;
                                                }

                                                var aggrExpMest = this._AggrExpMests.FirstOrDefault(o => o.ID == expMestPrints.First().AGGR_EXP_MEST_ID);
                                                PrintMps000235(printTypeCode, fileName,
                                                    gr.ToList(),
                                                    null,
                                                    aggrExpMest,
                                                    expMestPrints,
                                                    this._Department,
                                                    title,
                                                    patient,
                                                    _DataPatientTypeAlters,
                                                    _DataServiceReq,
                                                    treatment,
                                                    treatmentBedRoom,
                                                    inputADO, ref result);
                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintMps000235(string printTypeCode, string fileName,
            List<V_HIS_EXP_MEST_MEDICINE> _expMestMedicines,
            List<V_HIS_EXP_MEST_MATERIAL> _expMestMaterials,
            V_HIS_EXP_MEST aggrExpMest,
            List<HIS_EXP_MEST> _expMests_Print,
            HIS_DEPARTMENT department,
            MPS.Processor.Mps000235.PDO.keyTitles _key,
            V_HIS_PATIENT _patient,
            List<HIS_PATIENT_TYPE_ALTER> _patientTYpeAlters,
            List<HIS_SERVICE_REQ> _ServiceReq,
            HIS_TREATMENT treatment,
            V_HIS_TREATMENT_BED_ROOM bedRoom,
            Inventec.Common.SignLibrary.ADO.InputADO inputADO,
            ref bool result
                )
        {
            try
            {
                MPS.Processor.Mps000235.PDO.Mps000235PDO mps000235RDO = new MPS.Processor.Mps000235.PDO.Mps000235PDO(
                    _expMestMedicines,
                    _expMestMaterials,
                    aggrExpMest,
                    _expMests_Print,
                    department,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    _key,
                    this.configKeyMERGER_DATA,
                    _patient,
                    _patientTYpeAlters,
                    _ServiceReq,
                    treatment,
                    bedRoom);

                mps000235RDO.ListMediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>();
                WaitingManager.Hide();
                int count = 0;

                if (_expMestMedicines != null && _expMestMedicines.Count > 0)
                {
                    count += _expMestMedicines.Where(o => _expMests_Print.Exists(e => e.ID == o.EXP_MEST_ID)).ToList().Count;
                }

                if (_expMestMaterials != null && _expMestMaterials.Count > 0)
                {
                    count += _expMestMaterials.Where(o => _expMests_Print.Exists(e => e.ID == o.EXP_MEST_ID)).ToList().Count;
                }

                Print.PrintData(printTypeCode, fileName, mps000235RDO, this.printNow, inputADO, ref result, this.currentModule.RoomId, this.EmrSign, this.EmrSignAndPrint, count, SetDataGroup);
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InTraDoiTongHop6282(string printTypeCode, string fileName, ref bool result, bool conditional, List<long> serviceUnitIds, List<long> useFormIds, List<long> lstreqRoomId, long? IntructionTimeFrom = null, long? IntructionTimeTo = null, bool Medicine = false, bool Material = false, bool IsChemicalSustance = false, HIS_DEPARTMENT hisDepartment = null, long? chooseTimeType = null)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                WaitingManager.Show();

                LoadDataMedicineAndMaterial(this._AggrExpMests, chooseTimeType, IntructionTimeFrom, IntructionTimeTo);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._AggrExpMests != null ? this._AggrExpMests.FirstOrDefault().TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                MPS.Processor.Mps000247.PDO.Mps000247PDO mps000247RDO;

                List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                List<V_HIS_BED_LOG> BedLogList = new List<V_HIS_BED_LOG>();

                CommonParam param = new CommonParam();

                if (hisDepartment != null)
                {
                    this._Department = hisDepartment;
                }

                if (conditional)
                {
                    _ExpMestMedicines = _ExpMestMedicines.Where(p => Check(p, serviceUnitIds, useFormIds)).ToList();
                    _ExpMestMaterials = _ExpMestMaterials.Where(p => Check(p, serviceUnitIds)).ToList();

                    if (!Medicine)
                    {
                        this._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    }

                    if (!Material)
                    {
                        if (IsChemicalSustance)
                        {
                            if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                            {
                                this._ExpMestMaterials = this._ExpMestMaterials.Where(o => o.IS_CHEMICAL_SUBSTANCE == 1).ToList();
                            }
                        }
                        else
                        {
                            this._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                        }
                    }

                    if (lstreqRoomId != null && lstreqRoomId.Count > 0)
                    {
                        if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                        {
                            this._ExpMestMaterials = this._ExpMestMaterials.Where(p => lstreqRoomId.Contains(p.REQ_ROOM_ID)).ToList();
                        }

                        if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                        {
                            this._ExpMestMedicines = this._ExpMestMedicines.Where(p => lstreqRoomId.Contains(p.REQ_ROOM_ID)).ToList();
                        }

                        if (this._ExpMests_Print != null && _ExpMests_Print.Count > 0)
                        {
                            this._ExpMests_Print = this._ExpMests_Print.Where(p => lstreqRoomId.Contains(p.REQ_ROOM_ID)).ToList();
                        }

                        if (this._ViewExpMests_Print != null && _ViewExpMests_Print.Count > 0)
                        {
                            this._ViewExpMests_Print = this._ViewExpMests_Print.Where(p => lstreqRoomId.Contains(p.REQ_ROOM_ID)).ToList();
                        }
                    }

                    if (this._ExpMests_Print != null && _ExpMests_Print.Count > 0)
                    {
                        HisTreatmentBedRoomViewFilter treatmentBedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                        treatmentBedRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        treatmentBedRoomViewFilter.TREATMENT_IDs = this._ExpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                        vHisTreatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                        bedLogFilter.TREATMENT_IDs = this._ExpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();

                        bedLogFilter.TREATMENT_BED_ROOM_IDs = vHisTreatmentBedRooms != null && vHisTreatmentBedRooms.Count > 0
                            ? vHisTreatmentBedRooms.Select(o => o.ID).Distinct().ToList()
                            : null;

                        BedLogList = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, param);
                    }

                    //Inventec.Common.Logging.LogSystem.Info("_ExpMestMaterials: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _ExpMestMaterials), _ExpMestMaterials));

                    mps000247RDO = new MPS.Processor.Mps000247.PDO.Mps000247PDO(
                        this._ExpMestMedicines,
                        this._ExpMestMaterials,
                        this._ViewExpMests_Print,
                        this._Department,
                        this.configKeyMERGER_DATA,
                        vHisTreatmentBedRooms,
                        BedLogList,
                        TimeFilterOption
                    );
                }
                else
                {
                    mps000247RDO = new MPS.Processor.Mps000247.PDO.Mps000247PDO(
                        this._ExpMestMedicines,
                        this._ExpMestMaterials,
                        this._ViewExpMests_Print,
                        this._Department,
                        this.configKeyMERGER_DATA,
                        vHisTreatmentBedRooms,
                        BedLogList,
                        TimeFilterOption
                    );
                }

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (this.EmrSign)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000247RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, printerName) { EmrInputADO = inputADO });
                }
                else if (this.EmrSignAndPrint)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000247RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, printerName) { EmrInputADO = inputADO });
                }
                else if (this.printNow || GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000247RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000247RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }

                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool Check(V_HIS_EXP_MEST_MATERIAL _expMestMaterial, List<long> serviceUnitIds)
        {
            bool result = false;
            try
            {
                if (_expMestMaterial != null)
                {
                    if (serviceUnitIds != null && serviceUnitIds.Count > 0)
                    {
                        if (serviceUnitIds.Contains(_expMestMaterial.SERVICE_UNIT_ID))
                            result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        bool Check(V_HIS_EXP_MEST_MEDICINE _expMestMedicine, List<long> serviceUnitIds, List<long> useFormIds)
        {
            bool result = false;
            try
            {
                var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (serviceUnitIds != null
                        && serviceUnitIds.Count > 0)
                    {
                        if (serviceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (useFormIds != null
                    && useFormIds.Count > 0 && useFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
                        {
                            result = result && true;
                        }
                        else
                            result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        List<HIS_EXP_MEST> _ExpMests_Print { get; set; }
        List<V_HIS_EXP_MEST> _ViewExpMests_Print { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }

        private void LoadDataMedicineAndMaterial(List<V_HIS_EXP_MEST> currentAggExpMest, long? chooseTimeType = null, long? IntructionTimeFrom = null, long? IntructionTimeTo = null)
        {
            try
            {
                if (currentAggExpMest == null)
                    throw new Exception("Du lieu rong currentAggExpMest");

                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestViewFilter expMestViewFilter = new HisExpMestViewFilter();
                expMestViewFilter.AGGR_EXP_MEST_IDs = currentAggExpMest.Select(p => p.ID).ToList();

                if (chooseTimeType == 1)
                {
                    if (IntructionTimeFrom != null)
                    {
                        expMestViewFilter.TDL_INTRUCTION_TIME_FROM = IntructionTimeFrom;
                    }
                    if (IntructionTimeTo != null)
                    {
                        expMestViewFilter.TDL_INTRUCTION_TIME_TO = IntructionTimeTo;
                    }
                }
                else
                {
                    if (IntructionTimeFrom != null)
                    {
                        expMestViewFilter.TDL_USE_TIME_FROM = IntructionTimeFrom;
                    }
                    if (IntructionTimeTo != null)
                    {
                        expMestViewFilter.TDL_USE_TIME_TO = IntructionTimeTo;
                    }
                }

                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ViewExpMests_Print = new List<V_HIS_EXP_MEST>();
                _ViewExpMests_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, expMestViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                _ExpMests_Print = new List<HIS_EXP_MEST>();
                AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST, HIS_EXP_MEST>();
                _ExpMests_Print = AutoMapper.Mapper.Map<List<HIS_EXP_MEST>>(this._ViewExpMests_Print);

                LoadDataExpMestMetyReq(IntructionTimeFrom, IntructionTimeTo, chooseTimeType);
                LoadDataExpMestMatyReq(IntructionTimeFrom, IntructionTimeTo, chooseTimeType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData(object param)
        {
            Thread threadMetyReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataExpMestMetyReqNewThread));
            //Thread threadMatyReq = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataExpMestMatyReqNewThread));

            //threadMetyReq.Priority = ThreadPriority.Normal;
            //threadMatyReq.Priority = ThreadPriority.Normal;
            try
            {
                threadMetyReq.Start(param);
                //threadMatyReq.Start(param);

                threadMetyReq.Join();
                //threadMatyReq.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                //threadMatyReq.Abort();
            }
        }

        private void LoadDataExpMestMetyReqNewThread(object param)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { LoadDataMaterial((long)param); }));
                //}
                //else
                //{
                LoadDataExpMestMetyReq();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Thuoc Yeu Cau
        /// </summary>
        /// <param name="_expMestIds"></param>
        private void LoadDataExpMestMetyReq(long? IntructionTimeFrom = null, long? IntructionTimeTo = null, long? chooseTimeType = null)
        {
            try
            {
                if (this._AggrExpMests != null && this._AggrExpMests.Count > 0)
                {
                    this._AggrExpMests = this._AggrExpMests.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                    Inventec.Common.Logging.LogSystem.Info("LoadDataExpMestMatyReq count:" + _AggrExpMests.Count + " : " + string.Join(",", _AggrExpMests.Select(s => s.ID)));

                    foreach (var item in this._AggrExpMests)
                    {
                        CommonParam param = new CommonParam();
                        HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                        medicineFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID = item.ID;

                        if (chooseTimeType == 1)
                        {
                            if (IntructionTimeFrom != null)
                            {
                                medicineFilter.TDL_INTRUCTION_TIME_FROM = IntructionTimeFrom;
                            }
                            if (IntructionTimeTo != null)
                            {
                                medicineFilter.TDL_INTRUCTION_TIME_TO = IntructionTimeTo;
                            }
                        }
                        else
                        {
                            //if (IntructionTimeFrom != null)
                            //{
                            //    medicineFilter.USE_TIME_TO_FROM = IntructionTimeFrom;
                            //}
                            //if (IntructionTimeTo != null)
                            //{
                            //    medicineFilter.USE_TIME_TO_TO = IntructionTimeTo;
                            //}
                        }
                       


                        var dataMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (dataMedicines != null && dataMedicines.Count > 0)
                        {
                            _ExpMestMedicines.AddRange(dataMedicines);
                        }
                    }
                }

                if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                {
                    _ExpMestMedicines = _ExpMestMedicines.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                    serviceUnitIds.AddRange(_ExpMestMedicines.Select(p => p.SERVICE_UNIT_ID).ToList());
                    useFormIds.AddRange(_ExpMestMedicines.Select(p => p.MEDICINE_USE_FORM_ID ?? 0).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataExpMestMatyReqNewThread(object param)
        {
            try
            {
                LoadDataExpMestMatyReq();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Vat Tu Yeu Cau
        /// </summary>
        /// <param name="_expMestIds"></param>
        private void LoadDataExpMestMatyReq(long? IntructionTimeFrom = null, long? IntructionTimeTo = null, long? medicineFilter = null, long? chooseTimeType = null)
        {
            try
            {
                if (this._AggrExpMests != null && this._AggrExpMests.Count > 0)
                {
                    this._AggrExpMests = this._AggrExpMests.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                    Inventec.Common.Logging.LogSystem.Info("LoadDataExpMestMatyReq count:" + _AggrExpMests.Count + " : " + string.Join(",", _AggrExpMests.Select(s => s.ID)));

                    foreach (var item in this._AggrExpMests)
                    {
                        CommonParam param = new CommonParam();
                        HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                        materialFilter.TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID = item.ID;

                        if (chooseTimeType == 1)
                        {
                            if (IntructionTimeFrom != null)
                            {
                                materialFilter.TDL_INTRUCTION_TIME_FROM = IntructionTimeFrom;
                            }
                            if (IntructionTimeTo != null)
                            {
                                materialFilter.TDL_INTRUCTION_TIME_TO = IntructionTimeTo;
                            }
                        }
                        else
                        {
                            //if (IntructionTimeFrom != null)
                            //{
                            //    materialFilter.TDL = IntructionTimeFrom;
                            //}
                            //if (IntructionTimeTo != null) 
                            //{
                            //    materialFilter.USE_TIME_TO_TO = IntructionTimeTo;
                            //}
                        }
                        

                        var dataMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (dataMaterials != null && dataMaterials.Count > 0)
                        {
                            _ExpMestMaterials.AddRange(dataMaterials);
                        }
                    }
                }

                if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                {
                    _ExpMestMaterials = _ExpMestMaterials.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                    serviceUnitIds.AddRange(_ExpMestMaterials.Select(p => p.SERVICE_UNIT_ID).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
