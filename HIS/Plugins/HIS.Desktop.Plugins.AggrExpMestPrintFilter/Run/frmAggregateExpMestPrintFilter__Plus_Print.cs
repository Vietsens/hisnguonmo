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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter
{
    internal partial class frmAggregateExpMestPrintFilter : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Ts = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_GNs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_HTs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_TDs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_PXs = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_HCGN = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_HCHT = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Others = new List<V_HIS_EXP_MEST_MEDICINE>();
        long configKeyMERGER_DATA = 0;
        long configKeyOderOption = 0;
        bool IsPrintMps169 = false;
        private void InitControl()
        {
            try
            {
                chkMedicine.Checked = true;
                chkMaterial.Checked = true;
                chkIsChemicalSustance.Checked = true;
                dtIntructionTimeFrom.EditValue = null;
                dtIntructionTimeTo.EditValue = null;

                switch (this.printType)
                {
                    case 1://tra doi
                        this.Text = "Điều kiện lọc - Tra đổi phiếu lĩnh";// Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_AGG_EXP_MEST_FILTER__TRA_DOI", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                        lciChkIsChemicalSubstance.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case 2://tong hop
                        this.Text = "Điều kiện lọc - Tổng hợp phiếu lĩnh";// Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_AGG_EXP_MEST_FILTER__SUM", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                        lciChkIsChemicalSubstance.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case 3://linh thuoc
                        this.Text = "Điều kiện lọc - Phiếu lĩnh";// Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_AGG_EXP_MEST_FILTER__LINH_THUOC_VAT_TU", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                        lciChkIsChemicalSubstance.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        break;

                    case 6://tra doi bù cơ số
                        this.Text = "Điều kiện lọc - Tra đổi bù cơ số";// Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_AGG_EXP_MEST_FILTER__TRA_DOI", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                        lciChkIsChemicalSubstance.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;

                    case 5: //Tra đối tổng hợp
                        this.Text = "Điều kiện lọc - Tra đối tổng hợp";

                        layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;
                    case 7:
                        this.Text = "Điều kiện lọc - Tra đổi phiếu lĩnh";// Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_FRM_AGG_EXP_MEST_FILTER__TRA_DOI", Base.ResourceLangManager.LanguageUCExpMestAggregate, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                        lciChkIsChemicalSubstance.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        lciChkMaterial.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciChkMedicine.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        layoutControlItem3.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        break;

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        Inventec.Common.RichEditor.RichEditorStore richEditorMain;
        private List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> GroupStreamPrint;
        private int TotalMediMatePrint;
        private int CountMediMatePrinted;
        private bool CancelPrint;
        private const int TIME_OUT_PRINT_MERGE = 1200;
        Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo adodata = null;
        Inventec.Common.Print.FlexCelPrintProcessor printProcess = null;
        HIS.Desktop.Plugins.AggrExpMestPrintFilter.Run.PrintNow print = null;
        private void ProcessPrint()
        {
            try
            {
                if (!chkMedicine.Checked && !chkMaterial.Checked && !chkIsChemicalSustance.Checked)
                {
                    MessageBox.Show("Bạn chưa chọn điều kiện lọc");
                    return;
                }

                if (AppConfigKeys.IsmergePrint)
                {
                    Inventec.Common.Logging.LogSystem.Info("PrintServiceReq IsmergePrint");
                    this.GroupStreamPrint = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                    if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                    {
                        this.TotalMediMatePrint += this._ExpMestMedicines.Count;
                    }
                    if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                    {
                        this.TotalMediMatePrint += this._ExpMestMaterials.Count;
                    }
                }

                long departmentId = (aggrExpMest.REQ_DEPARTMENT_ID);
                if (departmentId <= 0)
                {
                    departmentId = this.departmentId;
                }

                this.department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);
                this.richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                this.richEditorMain.SetActionCancelChooseTemplate(CancelChooseTemplate);
                switch (this.printType)
                {
                    case 1:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TRA_DOI__MPS000047, DelegateRunPrinter);
                        break;
                    case 2:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TONG_HOP__MPS000046, DelegateRunPrinter);
                        break;
                    case 4:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__THUOC_VAT_TU__MPS000049, DelegateRunPrinter);
                        break;
                    case 3:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__THUOC_VAT_TU__MPS000049, DelegateRunPrinter);
                        break;
                    case 6:
                        richEditorMain.RunPrintTemplate(PrintTypeCode.PRINT_TYPE_CODE__MPS000372, DelegateRunPrinter);
                        break;
                    case 5:
                        richEditorMain.RunPrintTemplate("Mps000247", DelegateRunPrinter);
                        break;
                    case 7:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TRA_DOI__MPS000047, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

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

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TONG_HOP__MPS000046:
                        LoadPhieuLinhTongHop(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__TRA_DOI__MPS000047:
                        LoadPhieuLinhTraDoi(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_LINH__THUOC_VAT_TU__MPS000049:
                        LoadPhieuLinhThuocGNHT(printTypeCode, fileName, ref result);
                        break;
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
                    case PrintTypeCode.PRINT_TYPE_CODE__MPS000372:
                        LoadPhieuTraDoiBuCoSo(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000247":
                        long chooseTimeType = 1;
                        if (cboChooseTime.EditValue != null && cboChooseTime.EditValue is long)
                        {
                            chooseTimeType = (long)cboChooseTime.EditValue;
                        }

                        print = new Run.PrintNow(this.currrentModule, chooseTimeType);
                        print._AggrExpMests = this._AggrExpMests.OrderBy(o => o.EXP_MEST_CODE).ToList();
                        print.printNow = this.chkPrintNow.Checked;

                        long? IntructionTimeFrom = null, IntructionTimeTo = null;

                        if (dtIntructionTimeFrom != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                        {
                            //IntructionTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTimeFrom.DateTime) ?? null;
                            IntructionTimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                        }
                        if (dtIntructionTimeTo != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                        {
                            //IntructionTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTimeTo.DateTime) ?? null;
                            IntructionTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                        }

                        this.serviceUnitIds = new List<long>();
                        if (ServiceUnits != null && ServiceUnits.Count > 0)
                            this.serviceUnitIds = ServiceUnits.Select(o => o.ID).Distinct().ToList();

                        this.useFormIds = new List<long>();
                        if (MedicineUseForms != null && MedicineUseForms.Count > 0)
                            this.useFormIds = MedicineUseForms.Select(o => o.ID).Distinct().ToList();

                        this.reqRoomIds = new List<long>();
                        if (RoomDTO3s != null && RoomDTO3s.Count > 0)
                            this.reqRoomIds = RoomDTO3s.Select(p => p.ID).ToList();

                        print.InTraDoiTongHop6282(printTypeCode, fileName, ref result, true, this.serviceUnitIds, this.useFormIds, this.reqRoomIds, IntructionTimeFrom, IntructionTimeTo, chkMedicine.Checked, chkMaterial.Checked, chkIsChemicalSustance.Checked, this.department, chooseTimeType);
                        this.CountMediMatePrinted = this.TotalMediMatePrint;
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
                Inventec.Common.Logging.LogSystem.Debug("Mps000175 nambg");
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

                    if (chkIsChemicalSustance.Checked && _ExpMestMate_HCs != null && _ExpMestMate_HCs.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                        MPS.Processor.Mps000175.PDO.Mps000175PDO mps000175PDO = new MPS.Processor.Mps000175.PDO.Mps000175PDO
                   (
                        _ExpMestMate_HCs,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        this.serviceUnitIds,
                        this.reqRoomIds,
                        MPS.Processor.Mps000175.PDO.keyTitles.phieuLinhHoaChat,
                        mpsConfig75,
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>()
                     );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000175PDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, _ExpMestMate_HCs.Count, SetDataGroup);
                    }
                    if (chkMaterial.Checked && _ExpMestMate_VTs != null && _ExpMestMate_VTs.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                        MPS.Processor.Mps000175.PDO.Mps000175PDO mps000175PDO = new MPS.Processor.Mps000175.PDO.Mps000175PDO
                   (
                     _ExpMestMate_VTs,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        this.serviceUnitIds,
                        this.reqRoomIds,
                        MPS.Processor.Mps000175.PDO.keyTitles.phieuLinhVatTu,
                        mpsConfig75,
                        BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>()
                     );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000175PDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, _ExpMestMate_VTs.Count, SetDataGroup);
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
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    if ((this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0) || (this._ExpMestMedi_HTs != null && this._ExpMestMedi_HTs.Count > 0))
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> DataGroups = new List<V_HIS_EXP_MEST_MEDICINE>();
                        DataGroups.AddRange(this._ExpMestMedi_GNs);
                        DataGroups.AddRange(this._ExpMestMedi_HTs);
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                        DataGroups,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        this.serviceUnitIds,
                        this.useFormIds,
                        this.reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinh_GN_HT,
                        mpsConfig169
                    );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000169RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, DataGroups.Count, SetDataGroup);
                    }

                    #region In Tat Ca HCGN,HCHT
                    if (_ExpMestMedi_HCGN.Count > 0 || _ExpMestMedi_HCHT.Count() > 0)
                    {
                        List<V_HIS_EXP_MEST_MEDICINE> mediTotalPrint = new List<V_HIS_EXP_MEST_MEDICINE>();
                        mediTotalPrint.AddRange(_ExpMestMedi_HCGN);
                        mediTotalPrint.AddRange(_ExpMestMedi_HCHT);
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO_HCGNHT = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                        mediTotalPrint,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        serviceUnitIds,
                        useFormIds,
                        reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinhHCGN_HCHT,
                        mpsConfig169
                    );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000169RDO_HCGNHT, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, mediTotalPrint.Count, SetDataGroup);
                    }
                    #endregion
                }
                else
                {
                    if ((this._ExpMestMedi_GNs != null && this._ExpMestMedi_GNs.Count > 0) || (this._ExpMestMedi_HCGN != null && this._ExpMestMedi_HCGN.Count > 0))
                    {
                        richEditorMain.RunPrintTemplate("Mps000325", DelegateRunPrinter);
                    }

                    if (this._ExpMestMedi_HTs != null && this._ExpMestMedi_HTs.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                        this._ExpMestMedi_HTs,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        this.serviceUnitIds,
                        this.useFormIds,
                        this.reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinhHT,
                        mpsConfig169
                    );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000169RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, this._ExpMestMedi_HTs.Count, SetDataGroup);
                    }
                    if (_ExpMestMedi_HCHT != null && _ExpMestMedi_HCHT.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                        MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                         _ExpMestMedi_HCHT,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        serviceUnitIds,
                        useFormIds,
                        reqRoomIds,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000169.PDO.keyTitles.phieuLinhHCHT,
                        mpsConfig169
                    );
                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000169RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, _ExpMestMedi_HCHT.Count, SetDataGroup);

                    }
                }

                if (_ExpMestMedi_Others != null && _ExpMestMedi_Others.Count > 0)
                {
                    var groups = _ExpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                    foreach (var gr in groups)
                    {
                        MPS.Processor.Mps000169.PDO.keyTitles title = new MPS.Processor.Mps000169.PDO.keyTitles();
                        if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                        {
                            title = MPS.Processor.Mps000169.PDO.keyTitles.TienChat;
                            IsPrintMps169 = true;

                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                            MPS.Processor.Mps000169.PDO.Mps000169PDO mps000169RDO = new MPS.Processor.Mps000169.PDO.Mps000169PDO(
                            gr.ToList(),
                            this.aggrExpMest,
                            this._ExpMests_Print,
                            this.department,
                            this.serviceUnitIds,
                            this.useFormIds,
                            this.reqRoomIds,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            title,
                            mpsConfig169
                        );

                            WaitingManager.Hide();
                            Run.Print.PrintData(printTypeCode, fileName, mps000169RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, this._ExpMestMedi_HTs.Count, SetDataGroup);
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
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);

                    MPS.Processor.Mps000325.PDO.Mps000325PDO mps000325RDO = new MPS.Processor.Mps000325.PDO.Mps000325PDO(
                    this._ExpMestMedi_GNs,
                    this.aggrExpMest,
                    this._ExpMests_Print,
                    this.department,
                    this.serviceUnitIds,
                    this.useFormIds,
                    this.reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    MPS.Processor.Mps000325.PDO.keyTitles.phieuLinhGN,
                    mpsConfig325
                );
                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, mps000325RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, this._ExpMestMedi_GNs.Count, SetDataGroup);
                }
                if (_ExpMestMedi_HCGN != null && _ExpMestMedi_HCGN.Count > 0)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    MPS.Processor.Mps000325.PDO.Mps000325PDO Mps000325PDO = new MPS.Processor.Mps000325.PDO.Mps000325PDO(
                    _ExpMestMedi_HCGN,
                    this.aggrExpMest,
                    this._ExpMests_Print,
                    this.department,
                    serviceUnitIds,
                    useFormIds,
                    reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    MPS.Processor.Mps000325.PDO.keyTitles.phieuLinhHCGN,
                    mpsConfig325
                );
                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, Mps000325PDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, _ExpMestMedi_HCGN.Count, SetDataGroup);
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
                    WaitingManager.Show();
                    MPS.Processor.Mps000236.PDO.Mps000236Config mpsConfig236 = new MPS.Processor.Mps000236.PDO.Mps000236Config();
                    mpsConfig236._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                    mpsConfig236._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    mpsConfig236._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                    mpsConfig236.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    MPS.Processor.Mps000236.PDO.Mps000236PDO mps000236RDO = new MPS.Processor.Mps000236.PDO.Mps000236PDO(
                    this._ExpMestMedi_TDs,
                    this.aggrExpMest,
                    this._ExpMests_Print,
                    this.department,
                    this.serviceUnitIds,
                    this.useFormIds,
                    this.reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    mpsConfig236
                );
                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, mps000236RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, this._ExpMestMedi_TDs.Count, SetDataGroup);
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
                    WaitingManager.Show();
                    MPS.Processor.Mps000239.PDO.Mps000239Config mpsConfig239 = new MPS.Processor.Mps000239.PDO.Mps000239Config();
                    mpsConfig239._ConfigKeyMERGER_DATA = configKeyMERGER_DATA;
                    mpsConfig239._ExpMestSttId__Approved = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE;
                    mpsConfig239._ExpMestSttId__Exported = IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE;
                    mpsConfig239.PatientTypeId__BHYT = AppConfigKeys.PatientTypeId__BHYT;

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    MPS.Processor.Mps000239.PDO.Mps000239PDO mps000239RDO = new MPS.Processor.Mps000239.PDO.Mps000239PDO(
                    this._ExpMestMedi_PXs,
                    this.aggrExpMest,
                    this._ExpMests_Print,
                    this.department,
                    this.serviceUnitIds,
                    this.useFormIds,
                    this.reqRoomIds,
                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    mpsConfig239
                );
                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, mps000239RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, this._ExpMestMedi_PXs.Count, SetDataGroup);
                }
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPhieuLinhTongHop(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                this.configKeyOderOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"));
                this.reqRoomIds = new List<long>();
                if (RoomDTO3s != null && RoomDTO3s.Count > 0)
                    this.reqRoomIds = RoomDTO3s.Select(o => o.ID).ToList();

                this.serviceUnitIds = new List<long>();
                if (ServiceUnits != null && ServiceUnits.Count > 0)
                    this.serviceUnitIds = ServiceUnits.Select(o => o.ID).Distinct().ToList();

                this.useFormIds = new List<long>();
                if (MedicineUseForms != null && MedicineUseForms.Count > 0)
                    this.useFormIds = MedicineUseForms.Select(o => o.ID).Distinct().ToList();

                //if (keyPrintType == 1)
                //{
                //    #region In Tong Hop
                //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                //    MPS.Processor.Mps000046.PDO.Mps000046PDO mps000046RDO = new MPS.Processor.Mps000046.PDO.Mps000046PDO(
                //        this._ExpMestMedicines,
                //        this._ExpMestMaterials,
                //        this.aggrExpMest,
                //        this._ExpMests_Print,
                //        this.department,
                //        serviceUnitIds,
                //        useFormIds,
                //        reqRoomIds,
                //        chkMedicine.Checked,
                //        chkMaterial.Checked,
                //        chkIsChemicalSustance.Checked,
                //        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                //        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                //        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //         MPS.Processor.Mps000046.PDO.keyTitles.phieuLinhTongHop,
                //         this.configKeyMERGER_DATA,
                //         AppConfigKeys.PatientTypeId__BHYT
                //    );
                //    // WaitingManager.Hide();
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000046RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000046RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                //    }
                //    result = MPS.MpsPrinter.Run(PrintData);
                //    #endregion
                //}
                //else
                {
                    #region Tach Thuoc GN - HT - TT
                    ProcessorMedicneGNHT();
                    #endregion

                    if ((chkMaterial.Checked || chkIsChemicalSustance.Checked) && this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                    {
                        richEditorMain.RunPrintTemplate("Mps000175", DelegateRunPrinter);
                    }
                    else if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                    {
                        this.CountMediMatePrinted += this._ExpMestMaterials.Count;
                    }

                    if (chkMedicine.Checked)
                    {
                        IsPrintMps169 = false;
                        richEditorMain.RunPrintTemplate("Mps000169", DelegateRunPrinter);

                        richEditorMain.RunPrintTemplate("Mps000236", DelegateRunPrinter);

                        richEditorMain.RunPrintTemplate("Mps000239", DelegateRunPrinter);

                        if (_ExpMestMedi_Others != null && _ExpMestMedi_Others.Count > 0)
                        {
                            var groups = _ExpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            if (IsPrintMps169)
                                groups = _ExpMestMedi_Others.Where(o => o.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                            foreach (var gr in groups)
                            {
                                MPS.Processor.Mps000046.PDO.keyTitles title = new MPS.Processor.Mps000046.PDO.keyTitles();
                                if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO)
                                {
                                    title = MPS.Processor.Mps000046.PDO.keyTitles.Corticoid;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN)
                                {
                                    title = MPS.Processor.Mps000046.PDO.keyTitles.DichTruyen;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                {
                                    title = MPS.Processor.Mps000046.PDO.keyTitles.KhangSinh;
                                }
                                else if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO)
                                {
                                    title = MPS.Processor.Mps000046.PDO.keyTitles.Lao;
                                }
                                if (!IsPrintMps169 && gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                                {
                                    title = MPS.Processor.Mps000046.PDO.keyTitles.TienChat;
                                }
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                                MPS.Processor.Mps000046.PDO.Mps000046PDO mps000046RDO = new MPS.Processor.Mps000046.PDO.Mps000046PDO(
                                gr.ToList(),
                                null,
                                this.aggrExpMest,
                                this._ExpMests_Print,
                                this.department,
                                serviceUnitIds,
                                useFormIds,
                                reqRoomIds,
                                chkMedicine.Checked,
                                false,
                                false,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                 title,
                                 this.configKeyMERGER_DATA,
                                 AppConfigKeys.PatientTypeId__BHYT,
                                 this.configKeyOderOption
                            );

                                WaitingManager.Hide();
                                Run.Print.PrintData(printTypeCode, fileName, mps000046RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, gr.ToList().Count, SetDataGroup);
                            }
                        }
                    }
                    else if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                    {
                        this.CountMediMatePrinted += this._ExpMestMedicines.Count;
                    }

                    #region In Thuoc Thuong
                    List<V_HIS_EXP_MEST_MEDICINE> dtMedicine = _ExpMestMedi_Ts;
                    if (_ExpMestMedi_Ts != null && _ExpMestMedi_Ts.Count > 0 && chkMedicine.Checked)
                    {
                        if (IsPrintMps169)
                            dtMedicine = _ExpMestMedi_Ts.Where(o => o.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).ToList();

                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("dtMedicine: ", dtMedicine.Select(o => o.MEDICINE_TYPE_ID).Distinct().ToList()));
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                        MPS.Processor.Mps000046.PDO.Mps000046PDO mps000046RDO = new MPS.Processor.Mps000046.PDO.Mps000046PDO(
                        dtMedicine,
                        null,
                        this.aggrExpMest,
                        this._ExpMests_Print,
                        this.department,
                        serviceUnitIds,
                        useFormIds,
                        reqRoomIds,
                        chkMedicine.Checked,
                        false,
                        false,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                        IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         MPS.Processor.Mps000046.PDO.keyTitles.phieuLinhThuocThuong,
                         this.configKeyMERGER_DATA,
                         AppConfigKeys.PatientTypeId__BHYT,
                         this.configKeyOderOption
                    );

                        WaitingManager.Hide();
                        Run.Print.PrintData(printTypeCode, fileName, mps000046RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, dtMedicine.Count, SetDataGroup);
                    }
                    #endregion
                }
                WaitingManager.Hide();
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
                _ExpMestMedi_HCGN = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_HCHT = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMedi_Others = new List<V_HIS_EXP_MEST_MEDICINE>();

                //ninhdd #32837
                //danh sách thuốc thường sẽ là nhóm thuốc không được chọn in tách
                if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                {
                    //this._ExpMestMedi_Ts = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID
                    //     == null || p.MEDICINE_GROUP_ID <= 0).ToList();
                    var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                    var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                    bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                    bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                    bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                    bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                    bool dcgn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                    bool dcht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);

                    this._ExpMestMedi_Ts = this._ExpMestMedicines.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                    //p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
                    this._ExpMestMedi_GNs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                    this._ExpMestMedi_HTs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                    this._ExpMestMedi_TDs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc).ToList();
                    this._ExpMestMedi_PXs = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px).ToList();
                    this._ExpMestMedi_HCGN = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn).ToList();
                    this._ExpMestMedi_HCHT = this._ExpMestMedicines.Where(p => p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht).ToList();
                    this._ExpMestMedi_Others = this._ExpMestMedicines.Where(p => mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0) &&
                      (p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN
                    && p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT)
                    ).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPhieuLinhTraDoi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                reqRoomIds = new List<long>();
                if (RoomDTO3s != null && RoomDTO3s.Count > 0)
                    reqRoomIds = RoomDTO3s.Select(p => p.ID).ToList();

                serviceUnitIds = new List<long>();
                if (ServiceUnits != null && ServiceUnits.Count > 0)
                    serviceUnitIds = ServiceUnits.Select(o => o.ID).Distinct().ToList();

                useFormIds = new List<long>();
                if (MedicineUseForms != null && MedicineUseForms.Count > 0)
                    useFormIds = MedicineUseForms.Select(o => o.ID).Distinct().ToList();

                List<V_HIS_PATIENT> patients = new List<V_HIS_PATIENT>();
                List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                List<V_HIS_BED_LOG> BedLogList = new List<V_HIS_BED_LOG>();

                long? IntructionTimeFrom = null, IntructionTimeTo = null;

                if (dtIntructionTimeFrom != null && dtIntructionTimeFrom.DateTime != DateTime.MinValue)
                {
                    //IntructionTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTimeFrom.DateTime) ?? null;
                    IntructionTimeFrom = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtIntructionTimeTo != null && dtIntructionTimeTo.DateTime != DateTime.MinValue)
                {
                    //IntructionTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtIntructionTimeTo.DateTime) ?? null;
                    IntructionTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtIntructionTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                long chooseTimeType = 1;
                if (cboChooseTime.EditValue != null && cboChooseTime.EditValue is long)
                {
                    chooseTimeType = (long)cboChooseTime.EditValue;
                }
                List<HIS_EXP_MEST> _ExpMests_Print_Temp = _ExpMests_Print;

                if (chooseTimeType == 1)
                {
                    if (IntructionTimeFrom != null)
                    {
                        _ExpMests_Print_Temp = _ExpMests_Print_Temp.Where(o => IntructionTimeFrom <= o.TDL_INTRUCTION_TIME).ToList();
                    }
                    if (IntructionTimeTo != null)
                    {
                        _ExpMests_Print_Temp = _ExpMests_Print_Temp.Where(o => o.TDL_INTRUCTION_TIME <= IntructionTimeTo).ToList();
                    }
                }
                else
                {
                    if (IntructionTimeFrom != null)
                    {
                        _ExpMests_Print_Temp = _ExpMests_Print_Temp.Where(o => IntructionTimeFrom <= o.TDL_USE_TIME).ToList();
                    }
                    if (IntructionTimeTo != null)
                    {
                        _ExpMests_Print_Temp = _ExpMests_Print_Temp.Where(o => o.TDL_USE_TIME <= IntructionTimeTo).ToList();
                    }
                }
                if (_ExpMests_Print_Temp != null && _ExpMests_Print_Temp.Count > 0)
                {
                    int count = 0;
                    var PatientList = _ExpMests_Print_Temp.Select(p => p.TDL_PATIENT_ID ?? 0).Distinct().ToList();
                    int total = PatientList.Count;
                    while (count < total)
                    {
                        var lst = PatientList.Skip(count).Take(100).ToList();

                        HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                        patientViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        patientViewFilter.IDs = lst;
                        var patienttmp = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (patienttmp != null && patienttmp.Count > 0)
                            patients.AddRange(patienttmp);
                        count += 100;
                    }

                    count = 0;
                    var TreatmentList = _ExpMests_Print_Temp.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                    total = TreatmentList.Count;
                    while (count < total)
                    {
                        var lst = TreatmentList.Skip(count).Take(100).ToList();

                        HisTreatmentBedRoomViewFilter treatmentBedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                        treatmentBedRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        treatmentBedRoomViewFilter.TREATMENT_IDs = lst;
                        var vHisTreatmentBedRoomstmp = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (vHisTreatmentBedRoomstmp != null && vHisTreatmentBedRoomstmp.Count > 0)
                            vHisTreatmentBedRooms.AddRange(vHisTreatmentBedRoomstmp);
                        count += 100;
                    }

                    //int countTempBed = 0;
                    //int startBed = 0;

                    //while (countTempBed < vHisTreatmentBedRooms.Count)
                    //{
                    //    var treatmentBedRoomTempIds = vHisTreatmentBedRooms.Skip(startBed).Take(100).Select(o => o.ID).ToList();
                    //    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    //    bedLogFilter.TREATMENT_IDs = this._ExpMests_Print.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();
                    //    bedLogFilter.TREATMENT_BED_ROOM_IDs = treatmentBedRoomTempIds;
                    //    var BedLogListTemp = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, param);
                    //    if (BedLogListTemp != null && BedLogListTemp.Count > 0)
                    //        BedLogList.AddRange(BedLogListTemp);

                    //    startBed += 100;
                    //    countTempBed += BedLogListTemp.Count();
                    //}

                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_IDs = _ExpMests_Print_Temp.Select(p => p.TDL_TREATMENT_ID ?? 0).Distinct().ToList();

                    bedLogFilter.TREATMENT_BED_ROOM_IDs = vHisTreatmentBedRooms != null && vHisTreatmentBedRooms.Count > 0
                        ? vHisTreatmentBedRooms.Select(o => o.ID).Distinct().ToList()
                        : null;

                    BedLogList = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, param);

                }
                List<MPS.Processor.Mps000047.PDO.Mps000047ADO> listMps000047ADO = new List<MPS.Processor.Mps000047.PDO.Mps000047ADO>();

                #region ------T-------------------------
                if (chkMedicine.Checked && this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                {
                    var query = this._ExpMestMedicines.ToList();
                    if (this.reqRoomIds != null && this.reqRoomIds.Count > 0 && _ExpMests_Print_Temp != null && _ExpMests_Print_Temp.Count > 0)
                    {
                        var expMests = _ExpMests_Print_Temp.Where(p => this.reqRoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                        if (expMests != null && expMests.Count > 0)
                        {
                            query = query.Where(o => expMests.Select(p => p.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        }
                    }

                    query = query.Where(p => Check(p)).ToList();
                    var Groups = query.GroupBy(g => new
                    {
                        g.MEDICINE_TYPE_ID,
                        g.EXP_MEST_ID,
                        g.REQ_ROOM_ID
                    }).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in Groups)
                    {
                        MPS.Processor.Mps000047.PDO.Mps000047ADO ado = new MPS.Processor.Mps000047.PDO.Mps000047ADO();

                        ado.TYPE_ID = 1;
                        if (itemGr[0].IS_EXPEND == 1)
                        {
                            ado.IS_EXPEND_DISPLAY = "X";
                        }
                        ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                        var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemGr[0].MEDICINE_TYPE_ID);
                        if (data != null)
                        {
                            ado.MEDICINE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                            ado.MEDI_MATE_TYPE_ID = data.ID;
                            ado.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                            ado.REGISTER_NUMBER = data.REGISTER_NUMBER;
                            ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                            ado.SERVICE_ID = data.SERVICE_ID;
                            ado.CONCENTRA = data.CONCENTRA;
                            ado.MEDICINE_USE_FORM_NAME = data.MEDICINE_USE_FORM_NAME;
                        }
                        ado.REQ_ROOM_ID = itemGr[0].REQ_ROOM_ID;
                        ado.REQ_ROOM_NAME = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == ado.REQ_ROOM_ID).ROOM_NAME;
                        if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            || this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            List<V_HIS_EXP_MEST_MEDICINE> listMedicines = (
                                this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                                ? this._ExpMestMedicines.Where(p =>
                                    p.MEDICINE_TYPE_ID == itemGr[0].MEDICINE_TYPE_ID).ToList()
                                    : null;
                            if (listMedicines != null && listMedicines.Count > 0)
                            {
                                ado.AMOUNT_EXCUTE = listMedicines.Sum(p => p.AMOUNT);
                                ado.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                                ado.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                                ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                                ado.PRICE = listMedicines.First().PRICE;
                                ado.IMP_PRICE = listMedicines.First().IMP_PRICE;
                                ado.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO * 100;
                                ado.DESCRIPTION = listMedicines.First().DESCRIPTION;
                                ado.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                                ado.NUM_ORDER = listMedicines.First().NUM_ORDER;
                            }
                            if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                            {
                                ado.AMOUNT_EXPORTED = ado.AMOUNT_EXCUTE;
                            }
                        }
                        ado.AMOUNT = itemGr.Sum(o => o.AMOUNT);
                        ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT)));
                        ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE)));
                        ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED)));

                        var expMest = _ExpMests_Print_Temp.FirstOrDefault(o => o.ID == itemGr[0].EXP_MEST_ID);
                        if (expMest != null)
                        {
                            ado.TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                            ado.Patient = patients.SingleOrDefault(o => o.ID == expMest.TDL_PATIENT_ID);
                            ado.TreatmentId = expMest.TDL_TREATMENT_ID ?? 0;
                        }
                        listMps000047ADO.Add(ado);
                    }
                }
                #endregion

                #region  -----VT---------------
                if (chkMaterial.Checked && this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    var query = this._ExpMestMaterials.ToList();
                    if (this.reqRoomIds != null && this.reqRoomIds.Count > 0 && _ExpMests_Print_Temp != null && _ExpMests_Print_Temp.Count > 0)
                    {
                        var expMests = _ExpMests_Print_Temp.Where(p => this.reqRoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                        if (expMests != null && expMests.Count > 0)
                        {
                            query = query.Where(o => expMests.Select(p => p.ID).Contains(o.EXP_MEST_ID ?? 0)).ToList();
                        }
                    }
                    query = query.Where(p => Check(p)).ToList();

                    var Groups = query.GroupBy(g => new
                    {
                        g.MATERIAL_TYPE_ID,
                        g.EXP_MEST_ID,
                        g.REQ_ROOM_ID
                    }).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in Groups)
                    {
                        MPS.Processor.Mps000047.PDO.Mps000047ADO ado = new MPS.Processor.Mps000047.PDO.Mps000047ADO();

                        ado.TYPE_ID = 1;
                        if (itemGr[0].IS_EXPEND == 1)
                        {
                            ado.IS_EXPEND_DISPLAY = "X";
                        }
                        ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                        var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == itemGr[0].MATERIAL_TYPE_ID);
                        if (data != null)
                        {
                            ado.MEDICINE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                            ado.MEDI_MATE_TYPE_ID = data.ID;
                            ado.MEDICINE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                            ado.SERVICE_ID = data.SERVICE_ID;
                        }

                        ado.REQ_ROOM_ID = itemGr[0].REQ_ROOM_ID;
                        ado.REQ_ROOM_NAME = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == ado.REQ_ROOM_ID).ROOM_NAME;
                        if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            || this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            List<V_HIS_EXP_MEST_MEDICINE> listMedicines = (
                                this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                                ? this._ExpMestMedicines.Where(p =>
                                    p.MEDICINE_TYPE_ID == itemGr[0].MATERIAL_TYPE_ID).ToList()
                                    : null;
                            if (listMedicines != null && listMedicines.Count > 0)
                            {
                                ado.AMOUNT_EXCUTE = listMedicines.Sum(p => p.AMOUNT);
                                ado.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                                ado.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                                ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                                ado.PRICE = listMedicines.First().PRICE;
                                ado.IMP_PRICE = listMedicines.First().IMP_PRICE;
                                ado.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO * 100;
                                ado.DESCRIPTION = listMedicines.First().DESCRIPTION;
                                ado.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                                ado.NUM_ORDER = listMedicines.First().NUM_ORDER;
                            }
                            if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                            {
                                ado.AMOUNT_EXPORTED = ado.AMOUNT_EXCUTE;
                            }
                        }
                        ado.AMOUNT = itemGr.Sum(o => o.AMOUNT);
                        ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT)));
                        ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE)));
                        ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED)));

                        var expMest = _ExpMests_Print_Temp.FirstOrDefault(o => o.ID == itemGr[0].EXP_MEST_ID);
                        if (expMest != null)
                        {
                            ado.TREATMENT_CODE = expMest.TDL_TREATMENT_CODE;
                            ado.Patient = patients.SingleOrDefault(o => o.ID == expMest.TDL_PATIENT_ID);
                            ado.TreatmentId = expMest.TDL_TREATMENT_ID ?? 0;
                        }
                        listMps000047ADO.Add(ado);
                    }
                }
                #endregion

                long keyColumnSize = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_PHIEU_TRA_DOI_THUOC_COLUMN_SIZE));

                listMps000047ADO = listMps000047ADO.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                var expMestMedicineGroups = listMps000047ADO.GroupBy(o => new { o.MEDI_MATE_TYPE_ID, o.TYPE_ID }).ToList();

                int countTemp = 0;
                int start = 0;
                int pageCount = 0;
                Dictionary<long, List<MPS.Processor.Mps000047.PDO.Mps000047ADO>> dicMedi = new Dictionary<long, List<MPS.Processor.Mps000047.PDO.Mps000047ADO>>();

                while (countTemp < listMps000047ADO.Count)
                {
                    pageCount += 1;
                    var expMestMedicineGroups__sub1 = expMestMedicineGroups.Skip(start).Take((int)keyColumnSize).Select(o => new { MEDI_MATE_TYPE_ID = o.ToList().First().MEDI_MATE_TYPE_ID, TYPE_ID = o.ToList().First().TYPE_ID }).ToList();
                    if (expMestMedicineGroups__sub1 != null && expMestMedicineGroups__sub1.Count > 0)
                    {
                        var expMestMedicinePrintAdoSplits =
                            (
                            from m in listMps000047ADO
                            from n in expMestMedicineGroups__sub1
                            where m.MEDI_MATE_TYPE_ID == n.MEDI_MATE_TYPE_ID
                            && m.TYPE_ID == n.TYPE_ID
                            select m
                             ).ToList();

                        dicMedi.Add(pageCount, expMestMedicinePrintAdoSplits);
                        //start += 44;
                        start += (int)keyColumnSize;
                        countTemp += expMestMedicinePrintAdoSplits.Count();
                    }
                }
                dicMedi = dicMedi.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
                if (dicMedi.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show("Phiếu lĩnh không có thuốc, vật tư thỏa mãn điều kiện lọc.");
                    return;
                }
                foreach (var item in dicMedi)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    MPS.Processor.Mps000047.PDO.Mps000047PDO mps000047RDO = new MPS.Processor.Mps000047.PDO.Mps000047PDO(
                      item.Value,
                     this.aggrExpMest,
                     _ExpMests_Print_Temp,
                     this.department,
                     vHisTreatmentBedRooms,
                     BedLogList,
                     keyColumnSize,
                     chooseTimeType
                 );
                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, mps000047RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, 1, SetDataGroup);
                }

                this.CountMediMatePrinted = this.TotalMediMatePrint;
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPhieuTraDoiBuCoSo(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                reqRoomIds = new List<long>();
                if (RoomDTO3s != null && RoomDTO3s.Count > 0)
                    reqRoomIds = RoomDTO3s.Select(p => p.ID).ToList();

                serviceUnitIds = new List<long>();
                if (ServiceUnits != null && ServiceUnits.Count > 0)
                    serviceUnitIds = ServiceUnits.Select(o => o.ID).Distinct().ToList();

                useFormIds = new List<long>();
                if (MedicineUseForms != null && MedicineUseForms.Count > 0)
                    useFormIds = MedicineUseForms.Select(o => o.ID).Distinct().ToList();

                List<long> treatmentIdList = new List<long>();
                if (this._ExpMestMetyReqList != null && this._ExpMestMetyReqList.Count > 0)
                {
                    treatmentIdList.AddRange(this._ExpMestMetyReqList
                        .Where(p => p.TREATMENT_ID.HasValue && p.TREATMENT_ID.Value > 0)
                        .Select(o => o.TREATMENT_ID.Value)
                        .Distinct().ToList());
                }
                if (this._ExpMestMatyReqList != null && this._ExpMestMatyReqList.Count > 0)
                {
                    treatmentIdList.AddRange(this._ExpMestMatyReqList
                        .Where(p => p.TREATMENT_ID.HasValue && p.TREATMENT_ID.Value > 0)
                        .Select(o => o.TREATMENT_ID.Value)
                        .Distinct().ToList());
                }

                List<HIS_TREATMENT> treatmentList = new List<HIS_TREATMENT>();
                if (treatmentIdList != null && treatmentIdList.Count > 0)
                {
                    treatmentIdList = treatmentIdList.Distinct().ToList();
                    int skip = 0;
                    while (treatmentIdList.Count - skip > 0)
                    {
                        var listIds = treatmentIdList.Skip(skip).Take(100).ToList();
                        skip += 100;
                        CommonParam param_ = new CommonParam();
                        MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                        treatmentFilter.IDs = listIds;
                        var treatmentList_ = new BackendAdapter(param_).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param_);
                        if (treatmentList_ != null && treatmentList_.Count > 0)
                        {
                            treatmentList.AddRange(treatmentList_);
                        }
                    }
                }


                List<V_HIS_PATIENT> patients = new List<V_HIS_PATIENT>();
                List<V_HIS_TREATMENT_BED_ROOM> vHisTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                List<V_HIS_BED_LOG> BedLogList = new List<V_HIS_BED_LOG>();
                if (treatmentList != null && treatmentList.Count > 0)
                {
                    int skip = 0;
                    while (treatmentList.Count - skip > 0)
                    {
                        var listIds = treatmentList.Skip(skip).Take(100).ToList();
                        skip += 100;

                        HisTreatmentBedRoomViewFilter treatmentBedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                        treatmentBedRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        treatmentBedRoomViewFilter.TREATMENT_IDs = listIds.Select(p => p.ID).ToList();
                        var vHisTreatmentBedRooms_ = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                        if (vHisTreatmentBedRooms_ != null && vHisTreatmentBedRooms_.Count > 0)
                        {
                            vHisTreatmentBedRooms.AddRange(vHisTreatmentBedRooms_);
                            HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                            bedLogFilter.TREATMENT_IDs = listIds.Select(o => o.ID).ToList();
                            bedLogFilter.TREATMENT_BED_ROOM_IDs = vHisTreatmentBedRooms_ != null && vHisTreatmentBedRooms_.Count > 0
                                ? vHisTreatmentBedRooms_.Select(o => o.ID).Distinct().ToList()
                                : null;

                            var BedLogList_ = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogFilter, param);
                            if (BedLogList_ != null && BedLogList_.Count > 0)
                            {
                                BedLogList.AddRange(BedLogList_);
                            }
                        }
                    }

                    skip = 0;
                    List<long> patientIds = treatmentList.Select(s => s.PATIENT_ID).Distinct().ToList();
                    while (patientIds.Count - skip > 0)
                    {
                        var listIds = patientIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        HisPatientViewFilter patientViewFilter = new HisPatientViewFilter();
                        patientViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        patientViewFilter.IDs = listIds;
                        var patients_ = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientViewFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (patients_ != null && patients_.Count > 0)
                        {
                            patients.AddRange(patients_);
                        }
                    }
                }

                List<MPS.Processor.Mps000372.PDO.Mps000372ADO> listMps000372ADO = new List<MPS.Processor.Mps000372.PDO.Mps000372ADO>();

                #region ------T-------------------------
                if (chkMedicine.Checked && this._ExpMestMetyReqList != null && this._ExpMestMetyReqList.Count > 0)
                {
                    var query = this._ExpMestMetyReqList.ToList();
                    if (this.reqRoomIds != null && this.reqRoomIds.Count > 0 && this._ExpMests_Print != null && this._ExpMests_Print.Count > 0)
                    {
                        var expMests = this._ExpMests_Print.Where(p => this.reqRoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                        if (expMests != null && expMests.Count > 0)
                        {
                            query = query.Where(o => expMests.Select(p => p.ID).Contains(o.EXP_MEST_ID)).ToList();
                        }

                    }
                    query = query.Where(p => Check(p)).ToList();
                    var Groups = query.GroupBy(g => new
                    {
                        g.MEDICINE_TYPE_ID,
                        g.TREATMENT_ID
                    }).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in Groups)
                    {
                        MPS.Processor.Mps000372.PDO.Mps000372ADO ado = new MPS.Processor.Mps000372.PDO.Mps000372ADO();

                        ado.TYPE_ID = 1;
                        //if (itemGr[0].IS_EXPEND == 1)
                        //{
                        //    ado.IS_EXPEND_DISPLAY = "X";
                        //}
                        ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                        var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemGr[0].MEDICINE_TYPE_ID);
                        if (data != null)
                        {
                            ado.MEDICINE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                            ado.MEDI_MATE_TYPE_ID = data.ID;
                            ado.MEDICINE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                            ado.REGISTER_NUMBER = data.REGISTER_NUMBER;
                            ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                            ado.SERVICE_ID = data.SERVICE_ID;
                        }

                        if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            || this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            List<HIS_EXP_MEST_METY_REQ> listMedicines = (
                                this._ExpMestMetyReqList != null && this._ExpMestMetyReqList.Count > 0)
                                ? this._ExpMestMetyReqList.Where(p =>
                                    p.MEDICINE_TYPE_ID == itemGr[0].MEDICINE_TYPE_ID).ToList()
                                    : null;
                            if (listMedicines != null && listMedicines.Count > 0)
                            {
                                ado.AMOUNT_EXCUTE = listMedicines.Sum(p => p.AMOUNT);
                                //ado.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                                //ado.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                                //ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                                //ado.PRICE = listMedicines.First().PRICE;
                                //ado.IMP_PRICE = listMedicines.First().IMP_PRICE;
                                //ado.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO * 100;
                                ado.DESCRIPTION = listMedicines.First().DESCRIPTION;
                                //ado.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                                ado.NUM_ORDER = listMedicines.First().NUM_ORDER;
                            }
                            if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                            {
                                ado.AMOUNT_EXPORTED = ado.AMOUNT_EXCUTE;
                            }
                        }
                        ado.AMOUNT = itemGr.Sum(o => o.AMOUNT);
                        ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT)));
                        ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE)));
                        ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED)));
                        HIS_TREATMENT treatment = null;
                        if (treatmentList != null && treatmentList.Count > 0)
                        {
                            var itemFist = itemGr.FirstOrDefault(p => p.TREATMENT_ID.HasValue);
                            treatment = itemFist != null ? treatmentList.FirstOrDefault(o => o.ID == itemFist.TREATMENT_ID) : null;
                        }
                        if (treatment != null)
                        {
                            ado.TREATMENT_CODE = treatment.TREATMENT_CODE;
                            ado.TreatmentId = treatment.ID;
                            ado.Patient = patients != null && patients.Count > 0 ? patients.FirstOrDefault(o => o.ID == treatment.PATIENT_ID) : null;
                        }
                        listMps000372ADO.Add(ado);
                    }
                }
                #endregion

                #region  -----VT---------------
                if (chkMaterial.Checked && this._ExpMestMatyReqList != null && this._ExpMestMatyReqList.Count > 0)
                {
                    var query = this._ExpMestMatyReqList.ToList();
                    if (this.reqRoomIds != null && this.reqRoomIds.Count > 0 && this._ExpMests_Print != null && this._ExpMests_Print.Count > 0)
                    {
                        var expMests = this._ExpMests_Print.Where(p => this.reqRoomIds.Contains(p.REQ_ROOM_ID)).ToList();
                        if (expMests != null && expMests.Count > 0)
                        {
                            query = query.Where(o => expMests.Select(p => p.ID).Contains(o.EXP_MEST_ID)).ToList();
                        }

                    }
                    query = query.Where(p => Check(p)).ToList();
                    var Groups = query.GroupBy(g => new
                    {
                        g.MATERIAL_TYPE_ID,
                        g.TREATMENT_ID
                    }).Select(p => p.ToList()).ToList();
                    foreach (var itemGr in Groups)
                    {
                        MPS.Processor.Mps000372.PDO.Mps000372ADO ado = new MPS.Processor.Mps000372.PDO.Mps000372ADO();

                        ado.TYPE_ID = 1;
                        //if (itemGr[0].IS_EXPEND == 1)
                        //{
                        //    ado.IS_EXPEND_DISPLAY = "X";
                        //}
                        ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                        var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == itemGr[0].MATERIAL_TYPE_ID);
                        if (data != null)
                        {
                            ado.MEDICINE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                            ado.MEDI_MATE_TYPE_ID = data.ID;
                            ado.MEDICINE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                            ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                            ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                            ado.SERVICE_ID = data.SERVICE_ID;
                        }

                        if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                            || this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            List<HIS_EXP_MEST_MATY_REQ> listMedicines = (
                                this._ExpMestMatyReqList != null && this._ExpMestMatyReqList.Count > 0)
                                ? this._ExpMestMatyReqList.Where(p =>
                                    p.MATERIAL_TYPE_ID == itemGr[0].MATERIAL_TYPE_ID).ToList()
                                    : null;
                            if (listMedicines != null && listMedicines.Count > 0)
                            {
                                ado.AMOUNT_EXCUTE = listMedicines.Sum(p => p.AMOUNT);
                                //ado.PACKAGE_NUMBER = listMedicines.First().PACKAGE_NUMBER;
                                //ado.SUPPLIER_NAME = listMedicines.First().SUPPLIER_NAME;
                                //ado.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(listMedicines.First().EXPIRED_DATE ?? 0);
                                //ado.PRICE = listMedicines.First().PRICE;
                                //ado.IMP_PRICE = listMedicines.First().IMP_PRICE;
                                //ado.IMP_VAT_RATIO = listMedicines.First().IMP_VAT_RATIO * 100;
                                ado.DESCRIPTION = listMedicines.First().DESCRIPTION;
                                //ado.MEDI_MATE_NUM_ORDER = listMedicines.First().MEDICINE_NUM_ORDER ?? 0;
                                ado.NUM_ORDER = listMedicines.First().NUM_ORDER;
                            }
                            if (this.aggrExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                            {
                                ado.AMOUNT_EXPORTED = ado.AMOUNT_EXCUTE;
                            }
                        }
                        ado.AMOUNT = itemGr.Sum(o => o.AMOUNT);
                        ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT)));
                        ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXCUTE)));
                        ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.AMOUNT_EXPORTED)));

                        //var expMest = this._ExpMests_Print.FirstOrDefault(o => o.ID == itemGr[0].EXP_MEST_ID);
                        //if (expMest != null)
                        //{
                        HIS_TREATMENT treatment = null;
                        if (treatmentList != null && treatmentList.Count > 0)
                        {
                            var itemFist = itemGr.FirstOrDefault(p => p.TREATMENT_ID.HasValue);
                            treatment = itemFist != null ? treatmentList.FirstOrDefault(o => o.ID == itemFist.TREATMENT_ID) : null;
                        }

                        if (treatment != null)
                        {
                            ado.TREATMENT_CODE = treatment.TREATMENT_CODE;
                            ado.TreatmentId = treatment.ID;
                            ado.Patient = patients != null && patients.Count > 0 ? patients.FirstOrDefault(o => o.ID == treatment.PATIENT_ID) : null;
                        }

                        listMps000372ADO.Add(ado);
                    }
                }
                #endregion

                listMps000372ADO = listMps000372ADO.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                var expMestMedicineGroups = listMps000372ADO.GroupBy(o => new { o.MEDI_MATE_TYPE_ID, o.TYPE_ID }).ToList();

                int countTemp = 0;
                int start = 0;
                int pageCount = 0;
                Dictionary<long, List<MPS.Processor.Mps000372.PDO.Mps000372ADO>> dicMedi = new Dictionary<long, List<MPS.Processor.Mps000372.PDO.Mps000372ADO>>();

                while (countTemp < listMps000372ADO.Count)
                {
                    pageCount += 1;
                    var expMestMedicineGroups__sub1 = expMestMedicineGroups.Skip(start).Take(44).Select(o => new { MEDI_MATE_TYPE_ID = o.ToList().First().MEDI_MATE_TYPE_ID, TYPE_ID = o.ToList().First().TYPE_ID }).ToList();
                    if (expMestMedicineGroups__sub1 != null && expMestMedicineGroups__sub1.Count > 0)
                    {
                        var expMestMedicinePrintAdoSplits =
                            (
                            from m in listMps000372ADO
                            from n in expMestMedicineGroups__sub1
                            where m.MEDI_MATE_TYPE_ID == n.MEDI_MATE_TYPE_ID
                            && m.TYPE_ID == n.TYPE_ID
                            select m
                             ).ToList();

                        dicMedi.Add(pageCount, expMestMedicinePrintAdoSplits);
                        start += 44;
                        countTemp += expMestMedicinePrintAdoSplits.Count();
                    }
                }
                dicMedi = dicMedi.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
                foreach (var item in dicMedi)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    MPS.Processor.Mps000372.PDO.Mps000372PDO mps000047RDO = new MPS.Processor.Mps000372.PDO.Mps000372PDO(
                      item.Value,
                     this.aggrExpMest,
                     this._ExpMests_Print,
                     this.department,
                     vHisTreatmentBedRooms,
                     BedLogList
                 );

                    WaitingManager.Hide();
                    Run.Print.PrintData(printTypeCode, fileName, mps000047RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, 0, SetDataGroup);
                }

                this.CountMediMatePrinted = this.TotalMediMatePrint;
            }
            catch (Exception ex)
            {
                CancelChooseTemplate(printTypeCode);
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPhieuLinhThuocGNHT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this.configKeyMERGER_DATA = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                this.configKeyOderOption = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMest.OderOption"));
                WaitingManager.Show();
                reqRoomIds = new List<long>();
                if (RoomDTO3s != null && RoomDTO3s.Count > 0)
                    reqRoomIds = RoomDTO3s.Select(o => o.ID).ToList();

                serviceUnitIds = new List<long>();
                if (ServiceUnits != null && ServiceUnits.Count > 0)
                    serviceUnitIds = ServiceUnits.Select(o => o.ID).Distinct().ToList();

                useFormIds = new List<long>();
                if (MedicineUseForms != null && MedicineUseForms.Count > 0)
                    useFormIds = MedicineUseForms.Select(o => o.ID).Distinct().ToList();

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
                                var expMestMedicineTypeNotChild = this._ExpMestMedicines.Where(o => lstExpMetyNotChild.Select(s => s.ID).Contains(o.MEDICINE_TYPE_ID)).ToList();
                                var expMestP = this._ExpMests_Print.Where(o => expMestMedicineTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                ado._ExpMests_Print.AddRange(expMestP);

                                if (ado._ExpMestMedicines == null) ado._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                                ado._ExpMestMedicines.AddRange(expMestMedicineTypeNotChild);

                                DicDataPrint[" "] = ado;
                            }

                            if (lstExpMetyChild != null && lstExpMetyChild.Count > 0)
                            {
                                foreach (var item in listParentMedicine)
                                {
                                    var groupParent = lstExpMetyChild.Where(o => o.PARENT_ID == item.ID).ToList();
                                    if (groupParent != null && groupParent.Count > 0)
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
                    else if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                    {
                        var expMestP = this._ExpMests_Print.Where(o => this._ExpMestMedicines.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                        if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                        if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                        ado._ExpMests_Print.AddRange(expMestP);

                        if (ado._ExpMestMedicines == null) ado._ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                        ado._ExpMestMedicines.AddRange(this._ExpMestMedicines);

                        DicDataPrint[" "] = ado;
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
                                var expMestMaterialTypeNotChild = this._ExpMestMaterials.Where(o => lstExpMatyNotChild.Select(s => s.ID).Contains(o.MATERIAL_TYPE_ID)).ToList();
                                var expMestP = this._ExpMests_Print.Where(o => expMestMaterialTypeNotChild.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                                ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                                if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                                if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                                ado._ExpMests_Print.AddRange(expMestP);

                                if (ado._ExpMestMaterials == null) ado._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                                ado._ExpMestMaterials.AddRange(expMestMaterialTypeNotChild);

                                DicDataPrint[" "] = ado;
                            }

                            if (lstExpMatyChild != null && lstExpMatyChild.Count > 0)
                            {
                                foreach (var item in listParentMaterial)
                                {
                                    var groupParent = lstExpMatyChild.Where(o => o.PARENT_ID == item.ID).ToList();
                                    if (groupParent != null && groupParent.Count > 0)
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
                    else if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                    {
                        var expMestP = this._ExpMests_Print.Where(o => this._ExpMestMaterials.Select(s => s.EXP_MEST_ID ?? 0).Distinct().Contains(o.ID)).ToList();

                        ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                        if (DicDataPrint.ContainsKey(" ")) ado = DicDataPrint[" "];

                        if (ado._ExpMests_Print == null) ado._ExpMests_Print = new List<HIS_EXP_MEST>();
                        ado._ExpMests_Print.AddRange(expMestP);

                        if (ado._ExpMestMaterials == null) ado._ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                        ado._ExpMestMaterials.AddRange(this._ExpMestMaterials);

                        DicDataPrint[" "] = ado;
                    }
                }
                else
                {
                    DicDataPrint.Clear();
                    ADO.MediMatePrintADO ado = new ADO.MediMatePrintADO();
                    ado._ExpMestMaterials = this._ExpMestMaterials;
                    ado._ExpMestMedicines = this._ExpMestMedicines;
                    ado._ExpMests_Print = this._ExpMests_Print;
                    DicDataPrint[" "] = ado;
                }

                this.TotalMediMatePrint = 0;
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

                    //if (keyPrintType == 1)
                    //{
                    //    #region In Tong Hop
                    //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                    //    MPS.Processor.Mps000049.PDO.Mps000049PDO mps000046RDO = new MPS.Processor.Mps000049.PDO.Mps000049PDO(
                    //        this._ExpMestMedicines,
                    //        this._ExpMestMaterials,
                    //        this.aggrExpMest,
                    //        this._ExpMests_Print,
                    //        this.department,
                    //        serviceUnitIds,
                    //        useFormIds,
                    //        reqRoomIds,
                    //        chkMedicine.Checked,
                    //        chkMaterial.Checked,
                    //        chkIsChemicalSustance.Checked,
                    //        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                    //        MPS.Processor.Mps000049.PDO.keyTitles.phieuLinhTongHop,
                    //        mpsConfig49
                    //    );

                    //    WaitingManager.Hide();
                    //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //    {
                    //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000046RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    //    }
                    //    else
                    //    {
                    //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000046RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    //    }
                    //    result = MPS.MpsPrinter.Run(PrintData);
                    //    #endregion
                    //}
                    //else
                    {
                        #region Tach Thuoc GN - HT - TT
                        ProcessorMedicneGNHT();
                        #endregion

                        WaitingManager.Hide();
                        if ((chkMaterial.Checked || chkIsChemicalSustance.Checked) && this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000175", DelegateRunPrinter);
                        }
                        else if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                        {
                            this.CountMediMatePrinted += this._ExpMestMaterials.Count;
                        }

                        if (chkMedicine.Checked)
                        {
                            IsPrintMps169 = false;
                            richEditorMain.RunPrintTemplate("Mps000169", DelegateRunPrinter);

                            richEditorMain.RunPrintTemplate("Mps000236", DelegateRunPrinter);

                            richEditorMain.RunPrintTemplate("Mps000239", DelegateRunPrinter);

                            if (_ExpMestMedi_Others != null && _ExpMestMedi_Others.Count > 0 && chkMedicine.Checked)
                            {
                                var groups = _ExpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                                if (IsPrintMps169)
                                    groups = _ExpMestMedi_Others.Where(o => o.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
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
                                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                                    MPS.Processor.Mps000049.PDO.Mps000049PDO mps000049RDO = new MPS.Processor.Mps000049.PDO.Mps000049PDO(
                                     gr.ToList(),
                                    null,
                                    this.aggrExpMest,
                                    this._ExpMests_Print,
                                    this.department,
                                    serviceUnitIds,
                                    useFormIds,
                                    reqRoomIds,
                                    chkMedicine.Checked,
                                    false,
                                    false,
                                    BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                                    title,
                                    mpsConfig49
                                );
                                    WaitingManager.Hide();
                                    Run.Print.PrintData(printTypeCode, fileName, mps000049RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, gr.ToList().Count, SetDataGroup);
                                }
                            }
                        }
                        else if (this._ExpMestMedicines != null && this._ExpMestMedicines.Count > 0)
                        {
                            this.CountMediMatePrinted += this._ExpMestMedicines.Count;
                        }

                        #region In Thuoc Thuong
                        List<V_HIS_EXP_MEST_MEDICINE> dtMedicine = _ExpMestMedi_Ts;
                        if (_ExpMestMedi_Ts != null && _ExpMestMedi_Ts.Count > 0 && chkMedicine.Checked)
                        {
                            if (IsPrintMps169)
                                dtMedicine = _ExpMestMedi_Ts.Where(o => o.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC).ToList();
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrExpMest != null ? this.aggrExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currrentModule.RoomId);
                            MPS.Processor.Mps000049.PDO.Mps000049PDO mps000049RDO = new MPS.Processor.Mps000049.PDO.Mps000049PDO(
                             this._ExpMestMedi_Ts,
                            null,
                            this.aggrExpMest,
                            this._ExpMests_Print,
                            this.department,
                            serviceUnitIds,
                            useFormIds,
                            reqRoomIds,
                            chkMedicine.Checked,
                            false,
                            false,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            MPS.Processor.Mps000049.PDO.keyTitles.phieuLinhThuocThuong,
                            mpsConfig49
                        );
                            WaitingManager.Hide();
                            Run.Print.PrintData(printTypeCode, fileName, mps000049RDO, this.chkPrintNow.Checked, inputADO, ref result, this.currrentModule.RoomId, false, false, dtMedicine.Count, SetDataGroup);
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

        bool Check(HIS_EXP_MEST_MATY_REQ _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == _expMestMedicine.MATERIAL_TYPE_ID);
                if (data != null)
                {
                    if (this.serviceUnitIds != null
                        && this.serviceUnitIds.Count > 0)
                    {
                        if (this.serviceUnitIds.Contains(data.SERVICE_UNIT_ID))
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

        bool Check(HIS_EXP_MEST_METY_REQ _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (this.serviceUnitIds != null
                        && this.serviceUnitIds.Count > 0)
                    {
                        if (this.serviceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (this.useFormIds != null
                    && this.useFormIds.Count > 0 && this.useFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
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

        bool Check(V_HIS_EXP_MEST_MEDICINE _expMestMedicine)
        {
            bool result = false;
            try
            {
                var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == _expMestMedicine.MEDICINE_TYPE_ID);
                if (data != null)
                {
                    if (this.serviceUnitIds != null
                        && this.serviceUnitIds.Count > 0)
                    {
                        if (this.serviceUnitIds.Contains(data.SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (data.MEDICINE_USE_FORM_ID > 0)
                    {
                        if (this.useFormIds != null
                    && this.useFormIds.Count > 0 && this.useFormIds.Contains(data.MEDICINE_USE_FORM_ID ?? 0))
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

        bool Check(V_HIS_EXP_MEST_MATERIAL _expMestMaterial)
        {
            bool result = false;
            try
            {
                if (_expMestMaterial != null)
                {
                    if (this.serviceUnitIds != null && this.serviceUnitIds.Count > 0 && (chkMaterial.Checked || chkIsChemicalSustance.Checked))
                    {
                        if (this.serviceUnitIds.Contains(_expMestMaterial.SERVICE_UNIT_ID))
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

        public object mpsConfig169 { get; set; }
    }
}
