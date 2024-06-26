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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestDetailBCS.ADO;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestDetailBCS.ExpMestViewDetail
{
    public partial class frmExpMestViewDetail : HIS.Desktop.Utility.FormBase
    {
        List<V_HIS_EXP_MEST_MEDICINE> lstHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstGayNghien = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstGopGayNgienHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstThuong = new List<V_HIS_EXP_MEST_MEDICINE>();

        string Req_Department_Name = "";
        string Req_Room_Name = "";
        string Exp_Department_Name = "";
        long roomIdByMediStockIdPrint = 0;
        long keyPhieuTra = 0;
        internal enum PrintType
        {
            MPS000215_XUAT_BU_CO_SO_TU_TRUC,
            MPS000216_PHIEU_XUAT_BU_THUOC_LE,
            MPS000346,
            MPS000347,
            MPS000372
        }

        string printerName = "";
        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new Inventec.Common.SignLibrary.ADO.InputADO();
        private void ProcessPrint(String printTypeCode)
        {
            try
            {
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this._CurrentExpMest != null ? this._CurrentExpMest.TDL_TREATMENT_CODE : ""), printTypeCode, this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void InitMenuToButtonPrint()
        {
            try
            {
                WaitingManager.Show();
                DXPopupMenu menu = new DXPopupMenu();

                if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCS)
                {
                    DXMenuItem itemXuatBuCoSoTuTruc = new DXMenuItem("Phiếu xuất bù cơ số tủ trực", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatBuCoSoTuTruc.Tag = PrintType.MPS000215_XUAT_BU_CO_SO_TU_TRUC;
                    menu.Items.Add(itemXuatBuCoSoTuTruc);

                    DXMenuItem itemXuatTraDoiBCS = new DXMenuItem("Phiếu tra đối bù cơ số", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatTraDoiBCS.Tag = PrintType.MPS000372;
                    menu.Items.Add(itemXuatTraDoiBCS);
                }
                else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BL)
                {
                    DXMenuItem itemXuatBuThuocLe = new DXMenuItem("Phiếu xuất bù thuốc lẻ", new EventHandler(OnClickInPhieuXuatKho));
                    itemXuatBuThuocLe.Tag = PrintType.MPS000216_PHIEU_XUAT_BU_THUOC_LE;
                    menu.Items.Add(itemXuatBuThuocLe);
                }
                else if (true)
                {

                }
                //else if (this._CurrentExpMest.EXP_MEST_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__BCT)
                //{
                //    DXMenuItem itemXuatBaoChe = new DXMenuItem("Phiếu xuất bào chế thuốc", new EventHandler(OnClickInPhieuXuatKho));
                //    itemXuatBaoChe.Tag = PrintType.MPS000244_PHIEU_XUAT_BAO_CHE_THUOC;
                //    menu.Items.Add(itemXuatBaoChe);
                //}
                else
                {
                    cboPrint.Enabled = false;
                    Inventec.Common.Logging.LogSystem.Info("khong tim thay PrintTypeCode");
                }

                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuXuatKho(object sender, EventArgs e)
        {
            try
            {
                LoadSpecificExpMest();
                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.MPS000215_XUAT_BU_CO_SO_TU_TRUC:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_CO_SO_TU_TRUC__MPS000215, DelegateRunPrinter);
                        break;
                    case PrintType.MPS000372:
                        richEditorMain.RunPrintTemplate(RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_DOI_BCS__MPS000372, DelegateRunPrinter);
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

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092:
                        InPhieuXuatBan(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_CO_SO_TU_TRUC__MPS000215:
                        InPhieuXuatBuCoSoTuTruc(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_XUAT_BU_THUOC_LE__MPS000216:
                        InPhieuXuatBuThuocLe(printTypeCode, fileName, ref result);
                        break;
                    case RequestUri.PRINT_TYPE_CODE__BIEUMAU__PHIEU_TRA_DOI_BCS__MPS000372:
                        ShowFormFilter(Convert.ToInt64(6));
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

        private void InPhieuXuatBaoCheThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                List<V_HIS_IMP_MEST_MEDICINE> impMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                V_HIS_IMP_MEST impMest = new V_HIS_IMP_MEST();
                CommonParam param = new CommonParam();
                WaitingManager.Show();

                if (this._CurrentExpMest != null)
                {
                    MOS.Filter.HisImpMestViewFilter impMestViewFilter = new HisImpMestViewFilter();
                    //impMestViewFilter.PREPARATION_EXP_MEST_ID = this._CurrentExpMest.ID;
                    impMest = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestViewFilter, param).FirstOrDefault();
                }
                if (impMest != null && impMest.ID > 0)
                {
                    MOS.Filter.HisImpMestMedicineViewFilter impMestMedicineViewFilter = new HisImpMestMedicineViewFilter();
                    impMestMedicineViewFilter.IMP_MEST_ID = impMest.ID;
                    impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>("api/HisImpMestMedicine/GetView", ApiConsumer.ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
                }

                if (this._ExpMestMaterials != null && this._ExpMestMaterials.Count > 0)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_EXP_MEST_MATERIAL_1, V_HIS_EXP_MEST_MATERIAL>();
                    expMestMaterials = AutoMapper.Mapper.Map<List<V_HIS_EXP_MEST_MATERIAL>>(this._ExpMestMaterials);
                }

                //MPS.Processor.Mps000244.PDO.Mps000244PDO rdo = new MPS.Processor.Mps000244.PDO.Mps000244PDO(
                //      this._CurrentExpMest,
                //      impMest,
                //      impMestMedicines,
                //      expMestMaterials);

                //MPS.ProcessorBase.Core.PrintData PrintData = null;
                //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                //}
                //WaitingManager.Hide();
                //result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBuThuocLe(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MOS.Filter.HisExpMestViewFilter e1 = new HisExpMestViewFilter();

                ProcessPrint(printTypeCode);

                MPS.Processor.Mps000216.PDO.Mps000216PDO rdo = new MPS.Processor.Mps000216.PDO.Mps000216PDO(
                    this._CurrentExpMest, this._ExpMestMedicines_Print, this._ExpMestMaterials_Print);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicinesBCS { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterialsBCS { get; set; }
        List<HIS_TREATMENT> ListTreatment { get; set; }
        HisExpMestBcsMoreInfoSDO MoreInfo { get; set; }

        private void ShowFormFilter(long printType)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    //Review
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this._CurrentExpMest);
                    listArgs.Add(printType);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    if (extenceInstance.GetType() == typeof(bool))
                    {
                        return;
                    }
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBuCoSoTuTruc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long keyOrder = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__ODER_OPTION));
                #region TT Chung
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 1");
                ProcessPrint(printTypeCode);
                Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 2");
                _ExpMestMedicinesBCS = new List<V_HIS_EXP_MEST_MEDICINE>();
                _ExpMestMaterialsBCS = new List<V_HIS_EXP_MEST_MATERIAL>();
                if (this._ExpMestMedicines_Print != null && this._ExpMestMedicines_Print.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 3");
                    _ExpMestMedicinesBCS = this._ExpMestMedicines_Print.Where(o => (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)).ToList();
                }
                if (this._ExpMestMaterials_Print != null && this._ExpMestMaterials_Print.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 3a");
                    _ExpMestMaterialsBCS = this._ExpMestMaterials_Print.Where(o => (o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                || o.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)).ToList();
                }
                #endregion

                List<long> treatmentIds = new List<long>();
                if (_ExpMestMatyReqs_Print != null && _ExpMestMatyReqs_Print.Count > 0)
                {
                    treatmentIds.AddRange(_ExpMestMatyReqs_Print.Select(s => s.TREATMENT_ID ?? 0).ToList());
                }

                if (_ExpMestMetyReqs_Print != null && _ExpMestMetyReqs_Print.Count > 0)
                {
                    treatmentIds.AddRange(_ExpMestMetyReqs_Print.Select(s => s.TREATMENT_ID ?? 0).ToList());
                }

                ListTreatment = new List<HIS_TREATMENT>();
                treatmentIds = treatmentIds.Distinct().ToList();

                if (treatmentIds != null && treatmentIds.Count > 0)
                {
                    int skip = 0;
                    while (treatmentIds.Count - skip > 0)
                    {
                        var listIds = treatmentIds.Skip(skip).Take(100).ToList();
                        skip += 100;

                        MOS.Filter.HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                        treatFilter.IDs = listIds;
                        var treat = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                        if (treat != null && treat.Count > 0)
                        {
                            ListTreatment.AddRange(treat);
                        }
                    }
                }


                if (this._CurrentExpMest != null &&
                    (this._CurrentExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES
                    || this._CurrentExpMest.BCS_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST.BCS_TYPE__ID__PRES_DETAIL))
                {
                    HisExpMestBcsMoreInfoFilter moreFilter = new HisExpMestBcsMoreInfoFilter();
                    moreFilter.BCS_EXP_MEST_ID = this._CurrentExpMest.ID;
                    MoreInfo = new BackendAdapter(new CommonParam()).Get<HisExpMestBcsMoreInfoSDO>("api/HisExpMest/GetBcsMoreInfo", ApiConsumers.MosConsumer, moreFilter, null);
                }
                Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 6");

                {
                    Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 8");
                    _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCHTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DCGNs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DC_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                    _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();

                    List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();

                    #region --- Xu Ly Tach GN_HT -----
                    if (_ExpMestMetyReqs_Print != null && _ExpMestMetyReqs_Print.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 8.1");
                        var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                        var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                        bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                        bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                        var IsSeparatePrintingGN = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN).IS_SEPARATE_PRINTING ?? 0;
                        var IsSeparatePrintingHT = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT).IS_SEPARATE_PRINTING ?? 0;
                        bool dcgn = IsSeparatePrintingGN == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN);
                        bool dcht = IsSeparatePrintingHT == 1 && mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT);
                        bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                        bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                        bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                        bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                        bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                        bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);
                        bool tc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC);


                        var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var item in _ExpMestMetyReqs_Print)
                        {
                            var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                            if (dataMedi != null)
                            {
                                if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_GNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                {
                                    _ExpMestMetyReq_GN_HTs.Add(item);
                                    _ExpMestMetyReq_HTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCGN && dcgn)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCGNs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HCHT && dcht)
                                {
                                    _ExpMestMetyReq_DC_GN_HTs.Add(item);
                                    _ExpMestMetyReq_DCHTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                {
                                    _ExpMestMetyReq_TDs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                {
                                    _ExpMestMetyReq_PXs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                {
                                    _ExpMestMetyReq_COs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                {
                                    _ExpMestMetyReq_DTs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                {
                                    _ExpMestMetyReq_KSs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                {
                                    _ExpMestMetyReq_LAOs.Add(item);
                                }
                                else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__TC && tc)
                                {
                                    _ExpMestMetyReq_TCs.Add(item);
                                }
                                else
                                {
                                    _ExpMestMetyReq_Ts.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    WaitingManager.Hide();
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("Mps000254", DelegateRunMps);

                    #region ----VatTu----
                    if (_ExpMestMatyReqs != null && _ExpMestMatyReqs.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 8.2");
                        WaitingManager.Show();
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                 this._CurrentExpMest,
                 null,
                 _ExpMestMaterialsBCS,
                 null,
                 _ExpMestMatyReqs_Print,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                 MPS.Processor.Mps000215.PDO.keyTitles.vattu,
                 ListTreatment,
                 MoreInfo,
                 keyOrder
                  );
                        WaitingManager.Hide();

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            //PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    
                    #endregion

                    #region ----- Thuong ----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 8.3");
                        WaitingManager.Show();
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                (
                  this._CurrentExpMest,
                 _ExpMestMedicinesBCS,
                 null,
                 _ExpMestMetyReq_Ts,
                 null,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                 MPS.Processor.Mps000215.PDO.keyTitles.thuong,
                 ListTreatment,
                 MoreInfo,
                 keyOrder
                  );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ----- Tien Chat ----
                    if (_ExpMestMetyReq_TCs != null && _ExpMestMetyReq_TCs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000215.PDO.Mps000215PDO mps000215PDO = new MPS.Processor.Mps000215.PDO.Mps000215PDO
                        (
                          this._CurrentExpMest,
                         _ExpMestMedicinesBCS,
                         null,
                         _ExpMestMetyReq_TCs,
                         null,
                         BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                         BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                         BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                         MPS.Processor.Mps000215.PDO.keyTitles.tienchat,
                         ListTreatment,
                         MoreInfo,
                         keyOrder
                         );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000215PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }
                Inventec.Common.Logging.LogSystem.Debug("InPhieuXuatBuCoSoTuTruc. 9");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DC_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCGNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DCHTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TCs = new List<HIS_EXP_MEST_METY_REQ>();

        bool DelegateRunMps(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000048":
                        Mps000048(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000198":
                        Mps000198(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000254":
                        MPS000254(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void MPS000254(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ProcessPrint(printTypeCode);

                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- GOP GN HT -----
                    if (_ExpMestMetyReq_GN_HTs != null && _ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                 this._CurrentExpMest,
                _ExpMestMedicinesBCS,
                _ExpMestMetyReq_GN_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.tonghop,
                 ListTreatment,
                 MoreInfo
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- GOP DC GN HT -----
                    if (_ExpMestMetyReq_DC_GN_HTs != null && _ExpMestMetyReq_DC_GN_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDODC = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                      (
                        this._CurrentExpMest,
                       _ExpMestMedicinesBCS,
                       _ExpMestMetyReq_DC_GN_HTs,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                       IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                       BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                       BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                        MPS.Processor.Mps000254.PDO.keyTitles.tonghopHc,
                        ListTreatment,
                    MoreInfo
                        );

                        MPS.ProcessorBase.Core.PrintData PrintDataDC = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintDataDC = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDODC, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintDataDC);
                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (_ExpMestMetyReq_GNs != null && _ExpMestMetyReq_GNs.Count > 0)
                    {
                        WaitingManager.Show();
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                 this._CurrentExpMest,
                _ExpMestMedicinesBCS,
                _ExpMestMetyReq_GNs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.gaynghien,
                 ListTreatment,
                 MoreInfo
                 );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- HT -----
                    if (_ExpMestMetyReq_HTs != null && _ExpMestMetyReq_HTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
               (
                 this._CurrentExpMest,
                _ExpMestMedicinesBCS,
                _ExpMestMetyReq_HTs,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 MPS.Processor.Mps000254.PDO.keyTitles.huongthan,
                 ListTreatment,
                 MoreInfo
                 );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCGN ----
                    if (_ExpMestMetyReq_DCGNs != null && _ExpMestMetyReq_DCGNs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_DCGNs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcGayNghien,
                             ListTreatment,
                         MoreInfo
                             );

                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion

                    #region ---- DCHT -----
                    if (_ExpMestMetyReq_DCHTs != null && _ExpMestMetyReq_DCHTs.Count > 0)
                    {
                        MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
                           (
                             this._CurrentExpMest,
                            _ExpMestMedicinesBCS,
                            _ExpMestMetyReq_DCHTs.OrderBy(o => o.NUM_ORDER).ToList(),
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                             MPS.Processor.Mps000254.PDO.keyTitles.DcHuongThan,
                             ListTreatment,
                         MoreInfo
                             );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                        }

                        WaitingManager.Hide();
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    #endregion
                }

                #region ----- TD -----
                if (_ExpMestMetyReq_TDs != null && _ExpMestMetyReq_TDs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
             this._CurrentExpMest,
            _ExpMestMedicinesBCS,
            _ExpMestMetyReq_TDs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocdoc,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- PX -----
                if (_ExpMestMetyReq_PXs != null && _ExpMestMetyReq_PXs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
             this._CurrentExpMest,
            _ExpMestMedicinesBCS,
            _ExpMestMetyReq_PXs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.thuocphongxa,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- CO -----
                if (_ExpMestMetyReq_COs != null && _ExpMestMetyReq_COs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
             this._CurrentExpMest,
            _ExpMestMedicinesBCS,
            _ExpMestMetyReq_COs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Corticoid,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- DT -----
                if (_ExpMestMetyReq_DTs != null && _ExpMestMetyReq_DTs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
             this._CurrentExpMest,
            _ExpMestMedicinesBCS,
            _ExpMestMetyReq_DTs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.DichTruyen,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- KS -----
                if (_ExpMestMetyReq_KSs != null && _ExpMestMetyReq_KSs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
             this._CurrentExpMest,
            _ExpMestMedicinesBCS,
            _ExpMestMetyReq_KSs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.KhangSinh,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion

                #region ---- LAO -----
                if (_ExpMestMetyReq_LAOs != null && _ExpMestMetyReq_LAOs.Count > 0)
                {
                    MPS.Processor.Mps000254.PDO.Mps000254PDO mps000254PDO = new MPS.Processor.Mps000254.PDO.Mps000254PDO
           (
             this._CurrentExpMest,
            _ExpMestMedicinesBCS,
            _ExpMestMetyReq_LAOs,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
            IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
            BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
            BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
             MPS.Processor.Mps000254.PDO.keyTitles.Lao,
             ListTreatment,
                 MoreInfo
             );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000254PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000048(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                ProcessPrint(printTypeCode);
                WaitingManager.Show();
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                if (this._ExpMestMetyReq_GNs != null && this._ExpMestMetyReq_GNs.Count > 0)
                {
                    string keyAddictive = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_GNs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyAddictive,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }

                if (this._ExpMestMetyReq_HTs != null && this._ExpMestMetyReq_HTs.Count > 0)
                {
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC HƯỚNG THẦN";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_HTs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }

                if (this._ExpMestMetyReq_TDs != null && this._ExpMestMetyReq_TDs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC ĐỘC";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC ĐỘC";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_TDs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }

                if (this._ExpMestMetyReq_PXs != null && this._ExpMestMetyReq_PXs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC PHÓNG XẠ";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC PHÓNG XẠ";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  this._CurrentExpMest,
                 this._ExpMestMedicines_Print,
                 null,
                 null,
                 this._ExpMestMetyReq_PXs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000198(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                WaitingManager.Show();
                ProcessPrint(printTypeCode);
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                if (this._ExpMestBltyReqs_Print != null && this._ExpMestBltyReqs_Print.Count > 0)
                {
                    string keyAddictive = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this._CurrentExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyAddictive = "PHIẾU TRẢ MÁU";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyAddictive = "PHIẾU LĨNH MÁU";
                    }
                    var expMestBltyReqs = ConvertExpMestBltyViewToTable();
                    MPS.Processor.Mps000198.PDO.Mps000198PDO mps000198PDO = new MPS.Processor.Mps000198.PDO.Mps000198PDO
                 (
                  this._CurrentExpMest,
                 expMestBltyReqs,
                 this._ExpMestBloods_Print,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                 BackendDataWorker.Get<HIS_BLOOD_RH>(),
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 keyAddictive,
                 configKeyMert,
                 keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                    }
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatBan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();

                WaitingManager.Show();
                ProcessPrint(printTypeCode);

                V_HIS_TRANSACTION transaction = null;
                if (_CurrentExpMest.BILL_ID.HasValue)
                {
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.ID = _CurrentExpMest.BILL_ID;
                    var lstTran = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetVIew", ApiConsumers.MosConsumer, tranFilter, param);
                    if (lstTran != null && lstTran.Count > 0)
                    {
                        transaction = lstTran.FirstOrDefault();
                    }
                }
                List<V_HIS_EXP_MEST> expMestList = new List<V_HIS_EXP_MEST>();
                expMestList.Add(this._CurrentExpMest);
                MPS.Processor.Mps000092.PDO.Mps000092PDO pdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(
                 expMestList,
                 this._ExpMestMedicines_Print,
                 this._ExpMestMaterials_Print,
                 transaction
                 );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO };
                }
                WaitingManager.Hide();
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
