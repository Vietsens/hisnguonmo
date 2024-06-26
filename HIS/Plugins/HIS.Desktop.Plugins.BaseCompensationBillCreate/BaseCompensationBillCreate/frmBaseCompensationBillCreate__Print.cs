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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ValidationRules;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.Print;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;

namespace HIS.Desktop.Plugins.BaseCompensationBillCreate
{
    public partial class frmBaseCompensationBillCreate : HIS.Desktop.Utility.FormBase
    {
        Dictionary<string, object> dicParam;
        decimal? SumMoney = 0, SumMoneyDisplay = 0, SumMoneyForChms = 0, SumMoneyForChmsDisplay = 0;
        List<System.Collections.IList> listInput;
        string urlForPrint = "";
        string ReceiverUserName = "";

        List<V_HIS_EXP_MEST_MEDICINE> lstHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstGayNghien = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstGopGayNgienHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
        List<V_HIS_EXP_MEST_MEDICINE> lstThuong = new List<V_HIS_EXP_MEST_MEDICINE>();
        V_HIS_CHMS_EXP_MEST chmsExpMest = new V_HIS_CHMS_EXP_MEST();
        string Req_Department_Name = "";
        string Req_Room_Name = "";
        string Exp_Department_Name = "";
        long roomIdByMediStockIdPrint = 0;
        long keyPhieuTra = 0;

        private void onClickInPhieuXuatChuyenKho(object sender, EventArgs e)
        {
            try
            {
                if (this.dataResult == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086, delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086:
                            InPhieuXuatChuyenKho(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089:
                            InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090:
                            InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InPhieuXuatChuyenKho(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                chmsFilter.ID = this.dataResult.ChmsExpMest.ID;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                var chmsExpMest = listChmsExpMest.First();
                CommonParam param = new CommonParam();
                Req_Department_Name = "";
                Req_Room_Name = "";
                Exp_Department_Name = "";
                keyPhieuTra = 0;
                var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == this.dataResult.ExpMest.REQ_DEPARTMENT_ID).ToList();
                if (Req_Department != null && Req_Department.Count > 0)
                {
                    Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                }

                var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.dataResult.ExpMest.REQ_ROOM_ID).ToList();
                if (Req_Room != null && Req_Room.Count > 0)
                {
                    Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                }
                var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == this.dataResult.ExpMest.MEDI_STOCK_ID).ToList();
                if (Exp_Department != null && Exp_Department.Count > 0)
                {
                    Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                }
                // tách riêng gây nghiện hướng thần thành một bản in
                lstHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
                lstGayNghien = new List<V_HIS_EXP_MEST_MEDICINE>();
                lstGopGayNgienHuongThan = new List<V_HIS_EXP_MEST_MEDICINE>();
                lstThuong = new List<V_HIS_EXP_MEST_MEDICINE>();

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));

                roomIdByMediStockIdPrint = 0;
                roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.chmsExpMest.MEDI_STOCK_ID).ROOM_ID;

                if (this.dataResult != null && this.dataResult.ExpMedicines != null && this.dataResult.ExpMedicines.Count > 0)
                {
                    foreach (var item in this.dataResult.ExpMedicines)
                    {
                        if (item.IS_NEUROLOGICAL == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_NEUROLOGICAL__TRUE && item.IS_ADDICTIVE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_ADDICTIVE__TRUE)
                        {
                            lstHuongThan.Add(item);
                            lstGayNghien.Add(item);
                            lstGopGayNgienHuongThan.Add(item);
                        }
                        else if (item.IS_NEUROLOGICAL == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_NEUROLOGICAL__TRUE)
                        {
                            lstHuongThan.Add(item);
                            lstGopGayNgienHuongThan.Add(item);
                        }
                        else if (item.IS_ADDICTIVE == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_ADDICTIVE__TRUE)
                        {
                            lstGayNghien.Add(item);
                            lstGopGayNgienHuongThan.Add(item);
                        }
                        else
                        {
                            lstThuong.Add(item);
                        }
                    }
                }

                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__CHE_DO_PHIEU_LINH_THUOC_GAY_NGHIEN_HUONG_TAM_THAN);

                if (keyPrintType == 1)
                {
                    if (this.dataResult.ExpMedicines != null && this.dataResult.ExpMedicines.Count > 0)
                    {
                        string keyName = "PHIẾU LĨNH TỔNG HỢP";
                        if (roomIdByMediStockIdPrint >0)
                        {
                            if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                            {
                                keyPhieuTra = 1;
                                keyName = "PHIẾU TRẢ TỔNG HỢP";
                            }
                            else
                            {
                                keyName = "PHIẾU LĨNH TỔNG HỢP";
                            }
                        }
                        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                    (
                     chmsExpMest,
                     this.dataResult.ExpMedicines,
                     this.dataResult.ExpMaterials,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                     keyName,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     configKeyMert,
                     keyPhieuTra
                      );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    else
                    {
                        string keyName = "PHIẾU LĨNH TỔNG HỢP";
                        if (roomIdByMediStockIdPrint > 0)
                        {
                            if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                            {
                                keyPhieuTra = 1;
                                keyName = "PHIẾU TRẢ TỔNG HỢP";
                            }
                            else
                            {
                                keyName = "PHIẾU LĨNH TỔNG HỢP";
                            }
                        }
                        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                   (
                    chmsExpMest,
                    null,
                    this.dataResult.ExpMaterials,
                    Req_Department_Name,
                    Req_Room_Name,
                    Exp_Department_Name,
                    HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                    keyName,
                    BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                    configKeyMert,
                    keyPhieuTra
                     );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    richEditorMain.RunPrintTemplate("Mps000048", DelegateRunMps);

                    if (lstThuong != null && lstThuong.Count > 0)
                    {
                        string keyNameAggr = "PHIẾU LĨNH THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint > 0)
                        {
                            if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                            {
                                keyPhieuTra = 1;
                                keyNameAggr = "PHIẾU TRẢ THUỐC THƯỜNG";
                            }
                            else
                            {
                                keyNameAggr = "PHIẾU LĨNH THUỐC THƯỜNG";
                            }
                        }
                        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                     (
                      chmsExpMest,
                      lstThuong,
                      null,
                      Req_Department_Name,
                      Req_Room_Name,
                      Exp_Department_Name,
                      HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                      keyNameAggr,
                      BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                      configKeyMert,
                      keyPhieuTra
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    if (this.dataResult.ExpMaterials != null && this.dataResult.ExpMaterials.Count > 0)
                    {
                        var listHoaChat = this.dataResult.ExpMaterials.Where(p => p.IS_CHEMICAL_SUBSTANCE != null).ToList();
                        var listVatTu = this.dataResult.ExpMaterials.Where(p => p.IS_CHEMICAL_SUBSTANCE == null).ToList();
                        if (listHoaChat != null && listHoaChat.Count > 0)
                        {
                            string keyNameAggrHc = "PHIẾU LĨNH HÓA CHẤT";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggrHc = "PHIẾU TRẢ HÓA CHẤT";
                                }
                                else
                                {
                                    keyNameAggrHc = "PHIẾU LĨNH HÓA CHẤT";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                       (
                        chmsExpMest,
                        null,
                        listHoaChat,
                        Req_Department_Name,
                        Req_Room_Name,
                        Exp_Department_Name,
                        HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                        keyNameAggrHc,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        configKeyMert,
                        keyPhieuTra
                         );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        if (listVatTu != null && listVatTu.Count > 0)
                        {
                            string keyNameAggr = "PHIẾU LĨNH VẬT TƯ";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggr = "PHIẾU TRẢ VẬT TƯ";
                                }
                                else
                                {
                                    keyNameAggr = "PHIẾU LĨNH VẬT TƯ";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                       (
                        chmsExpMest,
                        null,
                        listVatTu,
                        Req_Department_Name,
                        Req_Room_Name,
                        Exp_Department_Name,
                        HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                        keyNameAggr,
                        BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                        configKeyMert,
                        keyPhieuTra
                         );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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
                    case "Mps000048":
                        Mps000048(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000048(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                roomIdByMediStockIdPrint = 0;
                roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.chmsExpMest.MEDI_STOCK_ID).ROOM_ID;
                if (lstGayNghien != null && lstGayNghien.Count > 0)
                {
                    string keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                    if (roomIdByMediStockIdPrint > 0)
                    {
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN";
                        }
                        else
                        {
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                        }
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                  lstGayNghien,
                  null,
                  Req_Department_Name,
                  Req_Room_Name,
                  Exp_Department_Name,
                  HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                  keyAddictive,
                  BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                  configKeyMert,
                  keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }

                if (lstHuongThan != null && lstHuongThan.Count > 0)
                {
                    string keyNeurological = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                    if (roomIdByMediStockIdPrint > 0)
                    {
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNeurological = "PHIẾU TRẢ THUỐC HƯỚNG THẦN";
                        }
                        else
                        {
                            keyNeurological = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                        }
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                  lstHuongThan,
                  null,
                  Req_Department_Name,
                  Req_Room_Name,
                  Exp_Department_Name,
                  HisExpMestSttCFG.HisExpMestSttId__Draft,
                 HisExpMestSttCFG.HisExpMestSttId__Request,
                 HisExpMestSttCFG.HisExpMestSttId__Rejected,
                 HisExpMestSttCFG.HisExpMestSttId__Approved,
                 HisExpMestSttCFG.HisExpMestSttId__Exported,
                  keyNeurological,
                  BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                  configKeyMert,
                  keyPhieuTra
                   );
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                chmsFilter.ID = this.dataResult.ChmsExpMest.ID;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                var chmsExpMest = listChmsExpMest.First();
                MPS.Core.Mps000089.Mps000089RDO rdo = new MPS.Core.Mps000089.Mps000089RDO(chmsExpMest, this.dataResult.ExpMedicines);
                WaitingManager.Hide();
                result = MPS.Printer.Run(printTypeCode, fileName, rdo);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                chmsFilter.ID = this.dataResult.ChmsExpMest.ID;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                var chmsExpMest = listChmsExpMest.First();
                MPS.Core.Mps000090.Mps000090RDO rdo = new MPS.Core.Mps000090.Mps000090RDO(chmsExpMest, this.dataResult.ExpMedicines, this.dataResult.ExpMaterials);
                WaitingManager.Hide();
                result = MPS.Printer.Run(printTypeCode, fileName, rdo);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
