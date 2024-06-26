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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using MPS.Processor.Mps000078.PDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AggrImpMestPrintFilter
{
    internal partial class frmAggregateImpMestPrintFilter : HIS.Desktop.Utility.FormBase
    {
        List<HIS_PATIENT> _Patients = new List<HIS_PATIENT>();

        List<HIS_IMP_MEST> _ImpMest100s = new List<HIS_IMP_MEST>();

        List<V_HIS_TREATMENT_BED_ROOM> _TreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();

        List<V_HIS_PATIENT_TYPE_ALTER> _PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();

        internal List<long> _ServiceUnitIds = new List<long>();
        internal List<long> _UseFormIds = new List<long>();
        internal List<long> _ReqRoomIds = new List<long>();

        List<MPS.Processor.Mps000093.PDO.Mps000093ADO> listMps000093ADO = new List<MPS.Processor.Mps000093.PDO.Mps000093ADO>();

        private void ProcessPrint()
        {
            try
            {
                if (!chkMedicine.Checked && !chkMaterial.Checked)
                {
                    MessageBox.Show("Bạn chưa chọn điều kiện lọc");
                    return;
                }

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                switch (this.printType)
                {
                    case 1:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA__TRA_DOI__MPS000093, DelegateRunPrinter);
                        break;
                    case 2:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC__TONG_HOP__MPS000078, DelegateRunPrinter);
                        break;
                    case 3:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOSC_GAY_NGHIEN_HUONG_TAM_THAN__MPS000101, DelegateRunPrinter);
                        break;
                    case 4:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100, DelegateRunPrinter);
                        break;
                    case 6:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC__TONG_HOP__MPS000078, DelegateRunPrinter);
                        break;
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
                    case "Mps000101":
                        Mps000101(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000240":
                        Mps000240(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000241":
                        Mps000241(printTypeCode, fileName, ref result);
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
                long departmentId = (aggrImpMest.REQ_DEPARTMENT_ID ?? 0);
                if (departmentId <= 0)
                {
                    departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                }
                this.department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == departmentId);

                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA__TRA_DOI__MPS000093:
                        LoadPhieuTraTraDoi(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC__TONG_HOP__MPS000078:
                        LoadPhieuTraTongHop(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOSC_GAY_NGHIEN_HUONG_TAM_THAN__MPS000101:
                        LoadPhieuTraGayNghienHuongThan(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_THUOC_CHI_TIET__MPS000100:
                        LoadPhieuTraThuocVatTu(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadPhieuTraTongHop(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._ReqRoomIds = new List<long>();
                if (this.RoomDTO3s != null && this.RoomDTO3s.Count > 0)
                {
                    this._ReqRoomIds = RoomDTO3s.Select(o => o.ID).ToList();
                }

                this._ServiceUnitIds = new List<long>();
                if (this.ServiceUnits != null && this.ServiceUnits.Count > 0)
                {
                    this._ServiceUnitIds = ServiceUnits.Select(o => o.ID).ToList();
                }

                this._UseFormIds = new List<long>();
                if (this.MedicineUseForms != null && this.MedicineUseForms.Count > 0)
                {
                    this._UseFormIds = MedicineUseForms.Select(o => o.ID).ToList();
                }

                WaitingManager.Hide();
                //if (keyPrintType == 1)
                //{
                //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                //    // string keyNameAggr = "TỔNG HỢP";
                //    MPS.Processor.Mps000078.PDO.Mps000078PDO mps000078RDO = new MPS.Processor.Mps000078.PDO.Mps000078PDO(
                //        this._ImpMestMedicines,
                //        this._ImpMestMaterials,
                //        this.aggrImpMest,
                //        this.department,
                //        this._ServiceUnitIds,
                //        this._UseFormIds,
                //        this._ReqRoomIds,
                //        chkMedicine.Checked,
                //        chkMaterial.Checked,
                //        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                //        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                //        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //        IsTittle.TongHop,
                //        this._MobaImpMests
                //    );

                //    WaitingManager.Hide();
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000078RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000078RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                //    }
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                //else
                {
                    ExecuteMediFunc();

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    if (chkMedicine.Checked && this._ImpMestMedi_GN_HTs != null && this._ImpMestMedi_GN_HTs.Count > 0)
                    {
                        richEditorMain.RunPrintTemplate("Mps000101", DelegateRunMps);
                    }

                    if (this._ImpMestMedi_TDs != null && this._ImpMestMedi_TDs.Count > 0)
                    {
                        richEditorMain.RunPrintTemplate("Mps000241", DelegateRunMps);
                    }

                    if (this._ImpMestMedi_PXs != null && this._ImpMestMedi_PXs.Count > 0)
                    {
                        richEditorMain.RunPrintTemplate("Mps000240", DelegateRunMps);
                    }

                    if (chkMaterial.Checked && this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000078.PDO.Mps000078PDO mps000078RDO = new MPS.Processor.Mps000078.PDO.Mps000078PDO(
                            null,
                        this._ImpMestMaterials,
                        this.aggrImpMest,
                        this.department,
                        this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                        chkMedicine.Checked,
                        chkMaterial.Checked,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        IsTittle.VatTu,
                        this._MobaImpMests,
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

                    if (chkMedicine.Checked && this._ImpMestMedi_Ts != null && this._ImpMestMedi_Ts.Count > 0)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000078.PDO.Mps000078PDO mps000078RDO = new MPS.Processor.Mps000078.PDO.Mps000078PDO(
                            this._ImpMestMedi_Ts,
                        null,
                        this.aggrImpMest,
                        this.department,
                        this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                        chkMedicine.Checked,
                        chkMaterial.Checked,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        IsTittle.ThuocThuong,
                        this._MobaImpMests,
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

                    #region thuoc nhom khac
                    if (chkMedicine.Checked && this._ImpMestMedi_Others != null && this._ImpMestMedi_Others.Count > 0)
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
                            this.department,
                            this._ServiceUnitIds,
                            this._UseFormIds,
                            this._ReqRoomIds,
                            chkMedicine.Checked,
                            chkMaterial.Checked,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            title,
                            this._MobaImpMests
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
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ExecuteMediFunc()
        {
            try
            {
                this._ImpMestMedi_GN_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMedi_HTs = new List<V_HIS_IMP_MEST_MEDICINE>();//Huong than
                this._ImpMestMedi_GNs = new List<V_HIS_IMP_MEST_MEDICINE>();//Gay nghiện
                this._ImpMestMedi_Ts = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMedi_TDs = new List<V_HIS_IMP_MEST_MEDICINE>();
                this._ImpMestMedi_Others = new List<V_HIS_IMP_MEST_MEDICINE>();

                if (this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                {
                    var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                    var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                    bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                    bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                    bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                    bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);

                    this._ImpMestMedi_GN_HTs = this._ImpMestMedicines.Where(p =>
                        (p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                        || (p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)).ToList();
                    this._ImpMestMedi_HTs = this._ImpMestMedicines.Where(p =>
                        p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht).ToList();
                    this._ImpMestMedi_GNs = this._ImpMestMedicines.Where(p =>
                        p.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn).ToList();
                    this._ImpMestMedi_Ts = this._ImpMestMedicines.Where(p => !mediTs.Select(s => s.ID).Contains(p.MEDICINE_GROUP_ID ?? 0)).ToList();
                    //p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC
                    //&& p.MEDICINE_GROUP_ID != IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX).ToList();
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

        private void LoadPhieuTraTraDoi(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                _TreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                _PatientTypeAlters = new List<V_HIS_PATIENT_TYPE_ALTER>();
                _Patients = new List<HIS_PATIENT>();
                listMps000093ADO = new List<MPS.Processor.Mps000093.PDO.Mps000093ADO>();
                CommonParam param = new CommonParam();
                reqRoomIds = new List<long>();
                reqRoomIds = RoomDTO3s.Select(o => o.ID).ToList();

                serviceUnitIds = new List<long>();
                if (ServiceUnits != null && ServiceUnits.Count > 0)
                    serviceUnitIds = ServiceUnits.Select(o => o.ID).Distinct().ToList();

                _useFormIds = new List<long>();
                if (MedicineUseForms != null && MedicineUseForms.Count > 0)
                    _useFormIds = MedicineUseForms.Select(o => o.ID).Distinct().ToList();

                int start = 0;
                int count = this._MobaImpMests.Count;
                while (count > 0)
                {
                    int limit = (count <= 100) ? count : 100;
                    _ImpMest100s = new List<HIS_IMP_MEST>();
                    _ImpMest100s = this._MobaImpMests.Skip(start).Take(limit).ToList();

                    CreateThread100();

                    start += 100;
                    count -= 100;
                }

                CreateThreadMediMate093();

                listMps000093ADO = listMps000093ADO.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                var query = this.listMps000093ADO.ToList();
                if (this.reqRoomIds != null && this.reqRoomIds.Count > 0 && query != null && query.Count > 0)
                {
                    query = query.Where(p => this.reqRoomIds.Contains(p.REQ_ROOM_ID ?? 0)).ToList();
                }
                query = query.Where(p => Check(p)).ToList();

                var expMestMedicineGroups = query.GroupBy(o => new { o.MEDICINE_TYPE_ID, o.IS_MEDICINE }).ToList();

                int countTemp = 0;
                int start2 = 0;
                int pageCount = 0;
                Dictionary<long, List<MPS.Processor.Mps000093.PDO.Mps000093ADO>> dicMedi = new Dictionary<long, List<MPS.Processor.Mps000093.PDO.Mps000093ADO>>();

                while (countTemp < query.Count)
                {
                    pageCount += 1;
                    var expMestMedicineGroups__sub1 = expMestMedicineGroups.Skip(start2).Take(44).Select(o => new { MEDICINE_TYPE_ID = o.ToList().First().MEDICINE_TYPE_ID, IS_MEDICINE = o.ToList().First().IS_MEDICINE }).ToList();
                    if (expMestMedicineGroups__sub1 != null && expMestMedicineGroups__sub1.Count > 0)
                    {
                        var expMestMedicinePrintAdoSplits =
                            (from m in query
                             from n in expMestMedicineGroups__sub1
                             where m.MEDICINE_TYPE_ID == n.MEDICINE_TYPE_ID
                             && m.IS_MEDICINE == n.IS_MEDICINE
                             select m).ToList();

                        dicMedi.Add(pageCount, expMestMedicinePrintAdoSplits);
                        start2 += 44;
                        countTemp += expMestMedicinePrintAdoSplits.Count();
                    }
                }
                dicMedi = dicMedi.OrderByDescending(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
                foreach (var item in dicMedi)
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    MPS.Processor.Mps000093.PDO.Mps000093PDO mps000093RDO = new MPS.Processor.Mps000093.PDO.Mps000093PDO(
                    item.Value,
                        this.aggrImpMest,
                        this.department,
                        serviceUnitIds,
                        _useFormIds,
                        reqRoomIds,
                        chkMedicine.Checked,
                        chkMaterial.Checked,
                        BackendDataWorker.Get<V_HIS_ROOM>(),
                        _TreatmentBedRooms
                    );

                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000093RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000093RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        bool Check(MPS.Processor.Mps000093.PDO.Mps000093ADO _impMestReq)
        {
            bool result = false;
            try
            {
                long SERVICE_UNIT_ID = 0;
                long MEDICINE_USE_FORM_ID = 0;
                if (_impMestReq.IS_MEDICINE)
                {
                    var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == _impMestReq.MEDICINE_TYPE_ID);
                    SERVICE_UNIT_ID = data != null ? data.SERVICE_UNIT_ID : 0;
                    MEDICINE_USE_FORM_ID = data != null ? (data.MEDICINE_USE_FORM_ID ?? 0) : 0;
                }
                else
                {
                    var data = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == _impMestReq.MEDICINE_TYPE_ID);
                    SERVICE_UNIT_ID = data != null ? data.SERVICE_UNIT_ID : 0;
                }
                if (_impMestReq.IS_MEDICINE)
                {
                    if (this.serviceUnitIds != null
                        && this.serviceUnitIds.Count > 0)
                    {
                        if (this.serviceUnitIds.Contains(SERVICE_UNIT_ID))
                            result = true;
                    }
                    if (MEDICINE_USE_FORM_ID > 0)
                    {
                        if (this._useFormIds != null
                    && this._useFormIds.Count > 0 && this._useFormIds.Contains(MEDICINE_USE_FORM_ID))
                        {
                            result = result && true;
                        }
                    }
                }
                else
                {
                    if (this.serviceUnitIds != null
                        && this.serviceUnitIds.Count > 0)
                    {
                        if (this.serviceUnitIds.Contains(SERVICE_UNIT_ID))
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

        private void LoadPhieuTraGayNghienHuongThan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ExecuteMediFunc();

                if (this._ImpMestMedi_GN_HTs.Count == 0 && this._ImpMestMedi_GN_HTs.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu rỗng", "Thông báo");
                    this.Close();
                    return;
                }

                this._ReqRoomIds = new List<long>();
                if (this.RoomDTO3s != null && this.RoomDTO3s.Count > 0)
                {
                    this._ReqRoomIds = RoomDTO3s.Select(o => o.ID).ToList();
                }

                this._ServiceUnitIds = new List<long>();
                if (this.ServiceUnits != null && this.ServiceUnits.Count > 0)
                {
                    this._ServiceUnitIds = ServiceUnits.Select(o => o.ID).ToList();
                }

                this._UseFormIds = new List<long>();
                if (this.MedicineUseForms != null && this.MedicineUseForms.Count > 0)
                {
                    this._UseFormIds = MedicineUseForms.Select(o => o.ID).ToList();
                }

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate("Mps000101", DelegateRunMps);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
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
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000101.PDO.Mps000101PDO mps000101RDO = new MPS.Processor.Mps000101.PDO.Mps000101PDO(
                            this._ImpMestMedi_GN_HTs,
                            this.aggrImpMest,
                            this.department,
                            this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                            MPS.Processor.Mps000101.PDO.IsTittle101.GayNghienTamThan,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                              IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                             BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         this._MobaImpMests,
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
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000101.PDO.Mps000101PDO mps000101RDO = new MPS.Processor.Mps000101.PDO.Mps000101PDO(
                            this._ImpMestMedi_HTs,
                        this.aggrImpMest,
                        this.department,
                        this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                        MPS.Processor.Mps000101.PDO.IsTittle101.TamThan,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                          IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         this._MobaImpMests,
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
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                        MPS.Processor.Mps000101.PDO.Mps000101PDO mps000101RDO = new MPS.Processor.Mps000101.PDO.Mps000101PDO(
                            this._ImpMestMedi_GNs,
                        this.aggrImpMest,
                        this.department,
                        this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                        MPS.Processor.Mps000101.PDO.IsTittle101.GayNghien,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                          IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         this._MobaImpMests,
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
                    this.department,
                    this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
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

        private void Mps000241(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (_ImpMestMedi_TDs != null && _ImpMestMedi_TDs.Count > 0)
                {
                    WaitingManager.Show();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.aggrImpMest != null ? this.aggrImpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
                    MPS.Processor.Mps000241.PDO.Mps000241PDO mps000241RDO = new MPS.Processor.Mps000241.PDO.Mps000241PDO(
                        this._ImpMestMedi_TDs,
                    this.aggrImpMest,
                    this.department,
                    this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
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

        private void LoadPhieuTraThuocVatTu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._ReqRoomIds = new List<long>();
                
                if (this.RoomDTO3s != null && this.RoomDTO3s.Count > 0)
                {
                    this._ReqRoomIds = RoomDTO3s.Select(o => o.ID).ToList();
                }

                this._ServiceUnitIds = new List<long>();
                if (this.ServiceUnits != null && this.ServiceUnits.Count > 0)
                {
                    this._ServiceUnitIds = ServiceUnits.Select(o => o.ID).ToList();
                }

                this._UseFormIds = new List<long>();
                if (this.MedicineUseForms != null && this.MedicineUseForms.Count > 0)
                {
                    this._UseFormIds = MedicineUseForms.Select(o => o.ID).ToList();
                }

                //if (keyPrintType == 1)
                //{
                //    WaitingManager.Show();
                //    MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                //        this._ImpMestMedicines,
                //        this._ImpMestMaterials,
                //        this.aggrImpMest,
                //        this.department,
                //        this._ServiceUnitIds,
                //        this._UseFormIds,
                //        this._ReqRoomIds,
                //        chkMedicine.Checked,
                //        chkMaterial.Checked,
                //        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                //        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                //        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //        MPS.Processor.Mps000100.PDO.IsTittle.TongHop
                //    );

                //    WaitingManager.Hide();
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //    }
                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                //else
                {
                    ExecuteMediFunc();

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    if (chkMedicine.Checked)
                    {
                        if (this._ImpMestMedi_GN_HTs != null && this._ImpMestMedi_GN_HTs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000101", DelegateRunMps);
                        }

                        if (this._ImpMestMedi_TDs != null && this._ImpMestMedi_TDs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000241", DelegateRunMps);
                        }
                        if (this._ImpMestMedi_PXs != null && this._ImpMestMedi_PXs.Count > 0)
                        {
                            richEditorMain.RunPrintTemplate("Mps000240", DelegateRunMps);
                        }
                    }
                    //this._MobaExpMests = new List<HIS_EXP_MEST>();
                    //if (this._MobaImpMests != null && this._MobaImpMests.Count > 0)
                    //{
                    //    int start = 0;
                    //    int count = this._MobaImpMests.Count;
                    //    while (count > 0)
                    //    {
                    //        int limit = (count <= 100) ? count : 100;
                    //        var listSub = this._MobaImpMests.Skip(start).Take(limit).ToList();
                    //        List<long> _impMestIds = new List<long>();
                    //        _impMestIds = listSub.Select(p => p.ID).Distinct().ToList();

                    //        List<long> _MobaExpMestIds = listSub.Select(p => p.MOBA_EXP_MEST_ID ?? 0).Distinct().ToList();
                    //        CommonParam param = new CommonParam();
                    //        MOS.Filter.HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    //        expMestFilter.IDs = _MobaExpMestIds;
                    //        var dataExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, param);
                    //        if (dataExpMests != null && dataExpMests.Count > 0)
                    //        {
                    //            this._MobaExpMests.AddRange(dataExpMests);
                    //        }
                    //        CreateThread(_impMestIds);

                    //        start += 100;
                    //        count -= 100;
                    //    }
                    //}
                    if (chkMaterial.Checked && this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                            null,
                        this._ImpMestMaterials,
                        this.aggrImpMest,
                        this.department,
                        this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                        chkMedicine.Checked,
                        chkMaterial.Checked,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000100.PDO.IsTittle.VatTu,
                        BackendDataWorker.Get<V_HIS_ROOM>(),
                        this._MobaExpMests,
                      AppConfigKeys.ProcessOderOption
                        );

                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    //Review
                    if (chkMedicine.Checked && this._ImpMestMedi_Ts != null && this._ImpMestMedi_Ts.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                            this._ImpMestMedi_Ts,
                        null,
                        this.aggrImpMest,
                        this.department,
                        this._ServiceUnitIds,
                        this._UseFormIds,
                        this._ReqRoomIds,
                        chkMedicine.Checked,
                        chkMaterial.Checked,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                        IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                        BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000100.PDO.IsTittle.ThuocThuong,
                        BackendDataWorker.Get<V_HIS_ROOM>(),
                        this._MobaExpMests,
                          AppConfigKeys.ProcessOderOption
                        );

                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #region nhom thuoc khac
                    if (chkMedicine.Checked && this._ImpMestMedi_Others != null && this._ImpMestMedi_Others.Count > 0)
                    {
                        var groups = _ImpMestMedi_Others.GroupBy(o => o.MEDICINE_GROUP_ID).ToList();
                        foreach (var gr in groups)
                        {
                            MPS.Processor.Mps000100.PDO.IsTittle titleMps000100 = MPS.Processor.Mps000100.PDO.IsTittle.ThuocThuong;
                            if (gr.First().MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC)
                            {
                                titleMps000100 = MPS.Processor.Mps000100.PDO.IsTittle.TienChat;
                            }
                            MPS.Processor.Mps000100.PDO.Mps000100PDO mps000100RDO = new MPS.Processor.Mps000100.PDO.Mps000100PDO(
                                gr.ToList(),
                            null,
                            this.aggrImpMest,
                            this.department,
                            this._ServiceUnitIds,
                            this._UseFormIds,
                            this._ReqRoomIds,
                            chkMedicine.Checked,
                            chkMaterial.Checked,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT,
                            IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL,
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                            titleMps000100,
                            BackendDataWorker.Get<V_HIS_ROOM>(),
                            this._MobaExpMests,
                             AppConfigKeys.ProcessOderOption
                            );

                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000100RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThread100()
        {
            Thread thread = new Thread(LoadPatient);
            Thread thread1 = new Thread(LoadTreatmentBedRoom);
            Thread thread2 = new Thread(LoadPatientTypeAlter);

            thread1.Priority = ThreadPriority.Normal;
            thread2.Priority = ThreadPriority.Normal;
            try
            {
                thread.Start();
                thread1.Start();
                thread2.Start();


                thread.Join();
                thread1.Join();
                thread2.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
                thread1.Abort();
                thread2.Abort();
            }
        }

        private void LoadPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> patientIds = this._ImpMest100s.Select(p => p.TDL_PATIENT_ID ?? 0).ToList();
                HisPatientFilter patientViewFilter = new HisPatientFilter();
                patientViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                patientViewFilter.IDs = patientIds;
                var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GET, ApiConsumers.MosConsumer, patientViewFilter, param);
                _Patients.AddRange(patients);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentBedRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> treatmentIds = this._ImpMest100s.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                HisTreatmentBedRoomViewFilter treatmentBedRoomViewFilter = new HisTreatmentBedRoomViewFilter();
                treatmentBedRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                treatmentBedRoomViewFilter.TREATMENT_IDs = treatmentIds;
                var vHisTreatmentBedRooms = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomViewFilter, param);
                _TreatmentBedRooms.AddRange(vHisTreatmentBedRooms);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientTypeAlter()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> treatmentIds = this._ImpMest100s.Select(p => p.TDL_TREATMENT_ID ?? 0).ToList();
                HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                patientTypeAlterFilter.TREATMENT_IDs = treatmentIds;
                var listPatientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("/api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);
                _PatientTypeAlters.AddRange(listPatientTypeAlter);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadMediMate093()
        {
            Thread thread = new Thread(LoadMedicine093);
            Thread thread1 = new Thread(LoadMaterial093);

            thread1.Priority = ThreadPriority.Normal;
            try
            {
                thread.Start();
                thread1.Start();

                thread.Join();
                thread1.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                thread.Abort();
                thread1.Abort();
            }
        }

        private void LoadMedicine093()
        {
            try
            {
                if (this._ImpMestMedicines != null && this._ImpMestMedicines.Count > 0)
                {
                    var dataGroups = this._ImpMestMedicines.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.IMP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        MPS.Processor.Mps000093.PDO.Mps000093ADO ado = new MPS.Processor.Mps000093.PDO.Mps000093ADO(item);
                        //Review
                        var mobaImpMestPatient = this._MobaImpMests.FirstOrDefault(o => o.ID == item[0].IMP_MEST_ID);
                        if (mobaImpMestPatient != null)
                        {
                            ado.TREATMENT_CODE = mobaImpMestPatient.TDL_TREATMENT_CODE;
                            ado.Patient = _Patients.FirstOrDefault(o => o.ID == mobaImpMestPatient.TDL_PATIENT_ID);
                            ado.TreatmentId = mobaImpMestPatient.TDL_TREATMENT_ID ?? 0;

                            var patyAlter = _PatientTypeAlters.Where(p => p.TREATMENT_ID == ado.TreatmentId).OrderByDescending(p => p.LOG_TIME);

                            if (patyAlter != null && patyAlter.Count() > 0 && patyAlter.FirstOrDefault() != null && patyAlter.FirstOrDefault().PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                ado.IS_BHYT = "X";
                            }
                        }
                        listMps000093ADO.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterial093()
        {
            try
            {
                if (this._ImpMestMaterials != null && this._ImpMestMaterials.Count > 0)
                {
                    var dataGroups = this._ImpMestMaterials.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.IMP_MEST_ID }).Select(p => p.ToList()).ToList();
                    foreach (var item in dataGroups)
                    {
                        MPS.Processor.Mps000093.PDO.Mps000093ADO ado = new MPS.Processor.Mps000093.PDO.Mps000093ADO(item);
                        //Review
                        var mobaImpMestPatient = this._MobaImpMests.FirstOrDefault(o => o.ID == item[0].IMP_MEST_ID);
                        if (mobaImpMestPatient != null)
                        {
                            ado.TREATMENT_CODE = mobaImpMestPatient.TDL_TREATMENT_CODE;
                            ado.Patient = _Patients.FirstOrDefault(o => o.ID == mobaImpMestPatient.TDL_PATIENT_ID);
                            ado.TreatmentId = mobaImpMestPatient.TDL_TREATMENT_ID ?? 0;

                            var patyAlter = _PatientTypeAlters.Where(p => p.TREATMENT_ID == ado.TreatmentId).OrderByDescending(p => p.LOG_TIME);

                            if (patyAlter != null && patyAlter.Count() > 0 && patyAlter.FirstOrDefault()!=null && patyAlter.FirstOrDefault().PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                            {
                                ado.IS_BHYT = "X";
                            }
                        }
                        listMps000093ADO.Add(ado);
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
