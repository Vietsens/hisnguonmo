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
using HIS.UC.MedicineTypeInStock;
using HIS.UC.MaterialTypeInStock;
using HIS.UC.ExpMestMedicineGrid;
using HIS.UC.ExpMestMaterialGrid;
using HIS.UC.ExpMestMedicineGrid.ADO;
using HIS.UC.ExpMestMaterialGrid.ADO;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.SDO;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.ExpMestSaleCreate.ADO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Validation;
using DevExpress.Utils.Menu;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpMestSaleCreate.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate
{
    public partial class UCExpMestSaleCreate : UserControlBase
    {
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__GNs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__HTs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__TPCNs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedi_Sale__Ts { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicine_Sale_Prints { get; set; }
        List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterial_Sale_Prints { get; set; }
        List<V_HIS_EXP_MEST> _ExpMest_Sale_Prints { get; set; }
        List<V_HIS_EXP_MEST> _ExpMest_Sale_Print__One { get; set; }
        V_HIS_TRANSACTION _Transaction_Sale_Print { get; set; }
        HIS_EXP_MEST prescriptionPrint;
        HIS_SERVICE_REQ currentServiceReqPrint;
        Inventec.Desktop.Common.Modules.Module currentModule;
        private void InitMenuPrint(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST expMest, bool? isListResult = null)
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                if (HIS.Desktop.Plugins.ExpMestSaleCreate.Config.HisConfigCFG.IS_MUST_BE_FINISHED_BEFORED_PRINTING)
                {
                    if (expMest != null && expMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                    {
                        if (isListResult == null)
                        {
                            menu.Items.Add(new DXMenuItem("In phiếu xuất bán", new EventHandler(onClickInPhieuXuatBan)));
                        }
                        else if (isListResult == true)
                        {
                            menu.Items.Add(new DXMenuItem("In phiếu xuất bán", new EventHandler(onClickInPhieuXuatBan)));
                        }
                    }
                }
                else
                {
                    if (isListResult == null)
                    {
                        menu.Items.Add(new DXMenuItem("In phiếu xuất bán", new EventHandler(onClickInPhieuXuatBan)));
                    }
                    else if (isListResult == true)
                    {
                        menu.Items.Add(new DXMenuItem("In phiếu xuất bán", new EventHandler(onClickInPhieuXuatBan)));
                    }
                }

                if (chkCreateBill.Checked)
                {
                    if (isListResult == null)
                    {
                        menu.Items.Add(new DXMenuItem("In Hóa đơn/biên lai xuất bán", new EventHandler(onClickInHoaDonBienLaiXuatBan)));
                    }
                    else if (isListResult == true)
                    {
                        menu.Items.Add(new DXMenuItem("In Hóa đơn/biên lai xuất bán", new EventHandler(onClickInHoaDonBienLaiXuatBan)));
                    }
                }

                menu.Items.Add(new DXMenuItem("Hướng dẫn sử dụng thuốc", new EventHandler(onClickInHuongDanSuDung)));
                menu.Items.Add(new DXMenuItem("In đơn thuốc", new EventHandler(onClickInDonThuoc)));
                menu.Items.Add(new DXMenuItem("In đơn thuốc tổng hợp", new EventHandler(onClickInDonThuocTongHop)));

                ddBtnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClickInDonThuocTongHop(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_EXP_MEST> listExpMest = new List<V_HIS_EXP_MEST>();
                List<HIS_SERVICE_REQ> serviceReqPrints = new List<HIS_SERVICE_REQ>();
                List<HIS_EXP_MEST> expMestPrints = new List<HIS_EXP_MEST>();
                if (resultSDO != null && resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                {
                    listExpMest = resultSDO.ExpMestSdos.Select(s => s.ExpMest).ToList();

                    foreach (var item in listExpMest)
                    {
                        HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(expMest, item);
                        expMestPrints.Add(expMest);
                    }

                    //lấy đơn thuốc tư phiếu xuất

                    List<long> serviceReqId = listExpMest.Select(s => s.SERVICE_REQ_ID ?? 0).ToList();
                    if (serviceReqId != null && serviceReqId.Count() > 0)
                    {
                        CommonParam parama = new CommonParam();
                        HisServiceReqFilter HisServiceReq = new HisServiceReqFilter();
                        HisServiceReq.IDs = serviceReqId;
                        serviceReqPrints = new BackendAdapter(parama).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, HisServiceReq, parama);
                    }
                    //số lượng đơn thuốc với số lượng phiếu xuất khác nhau sẽ không in được
                    if (serviceReqPrints.Count < expMestPrints.Count)
                    {
                        int dem = 0;
                        foreach (var item in expMestPrints)
                        {
                            //không có thông tin đơn thuốc thì tạo thông tin đơn thuốc.
                            if (!item.SERVICE_REQ_ID.HasValue)
                            {
                                dem--;

                                HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, item);
                                item.SERVICE_REQ_ID = dem;
                                req.ID = item.SERVICE_REQ_ID ?? 0;
                                req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                                req.PARENT_ID = item.TDL_PATIENT_ID;
                                serviceReqPrints.Add(req);
                            }
                            else if (!serviceReqPrints.Exists(o => o.ID == item.SERVICE_REQ_ID))
                            {
                                //nếu có service_req_id mà không get được thông tin thì cũng tạo đơn giả để in
                                HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                                req.ID = item.SERVICE_REQ_ID ?? 0;
                                req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                                req.PARENT_ID = item.TDL_PATIENT_ID;
                                serviceReqPrints.Add(req);
                            }
                        }
                    }
                }

                if (listExpMest.Count <= 0)
                {
                    serviceReqPrints = dataServiceReqs;
                }
                
                MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                sdo.ExpMests = expMestPrints;
                sdo.ServiceReqs = serviceReqPrints;
                var PrintServiceReqProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModule);
                PrintServiceReqProcessor.Print(MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode, false);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuXuatBan(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("begin onClickInPhieuXuatBan");
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);


                WaitingManager.Show();


                if (isTwoPatient && ListResultSDO != null && ListResultSDO.Count > 0)
                {
                    foreach (var item in ListResultSDO)
                    {
                        this.resultSDO = new HisExpMestSaleListResultSDO();
                        this.resultSDO = item;
                        if (resultSDO != null)
                        {
                            InXuatBan(this.resultSDO, store);
                        }
                    }
                }
                else if (!isTwoPatient)
                {
                    InXuatBan(this.resultSDO, store);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InXuatBan(HisExpMestSaleListResultSDO resultSDO, Inventec.Common.RichEditor.RichEditorStore store)
        {
            try
            {
                List<long> expMestIdTemps = new List<long>();
                if (resultSDO != null && resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                {
                    expMestIdTemps = resultSDO.ExpMestSdos.Select(p => p.ExpMest).Select(p => p.ID).Distinct().ToList();
                }

                if (expMestIdTemps.Count == 0)
                {
                    return;
                }
                _Transaction_Sale_Print = new V_HIS_TRANSACTION();
                _ExpMest_Sale_Prints = new List<V_HIS_EXP_MEST>();

                _ExpMest_Sale_Prints = this.resultSDO.ExpMestSdos.Select(o => o.ExpMest).ToList();

                foreach (var expMestSale in this.resultSDO.ExpMestSdos)
                {
                    _ExpMest_Sale_Print__One = new List<V_HIS_EXP_MEST>();
                    _ExpMest_Sale_Print__One.Add(expMestSale.ExpMest);
                    if (HIS.Desktop.Plugins.ExpMestSaleCreate.Config.HisConfigCFG.IS_MUST_BE_FINISHED_BEFORED_PRINTING)
                    {
                        if (expMestSale != null && expMestSale.ExpMest.EXP_MEST_STT_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            WaitingManager.Hide();
                            MessageBox.Show("Phiếu chưa thực xuất");
                            return;
                        }
                    }
                    _ExpMestMedicine_Sale_Prints = expMestSale.ExpMedicines != null && expMestSale.ExpMedicines.Count > 0 ? expMestSale.ExpMedicines.ToList() : new List<V_HIS_EXP_MEST_MEDICINE>();

                    _ExpMestMaterial_Sale_Prints = expMestSale.ExpMaterials != null && expMestSale.ExpMaterials.Count > 0 ? expMestSale.ExpMaterials.ToList() : new List<V_HIS_EXP_MEST_MATERIAL>();
                    CommonParam param = new CommonParam();
                    if (expMestSale.ExpMest.BILL_ID.HasValue)
                    {
                        HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                        tranFilter.ID = expMestSale.ExpMest.BILL_ID.Value;
                        tranFilter.ORDER_DIRECTION = "DESC";
                        tranFilter.ORDER_FIELD = "MODIFY_TIME";
                        _Transaction_Sale_Print = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param).FirstOrDefault();
                    }

                    string key = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.ExpMestSaleCreate.PrintSplitOption");//#24245
                    WaitingManager.Hide();

                    if (key.Trim() == "1")
                    {
                        _ExpMestMedi_Sale__GNs = new List<V_HIS_EXP_MEST_MEDICINE>();
                        _ExpMestMedi_Sale__HTs = new List<V_HIS_EXP_MEST_MEDICINE>();
                        _ExpMestMedi_Sale__TPCNs = new List<V_HIS_EXP_MEST_MEDICINE>();
                        _ExpMestMedi_Sale__Ts = new List<V_HIS_EXP_MEST_MEDICINE>();

                        foreach (var item in _ExpMestMedicine_Sale_Prints)
                        {
                            if (item.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)
                            {
                                _ExpMestMedi_Sale__GNs.Add(item);
                            }
                            else if (item.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                            {
                                _ExpMestMedi_Sale__HTs.Add(item);
                            }
                            else if (item.IS_FUNCTIONAL_FOOD == 1)
                            {
                                _ExpMestMedi_Sale__TPCNs.Add(item);
                            }
                            else
                            {
                                _ExpMestMedi_Sale__Ts.Add(item);
                            }
                        }
                        if (_ExpMestMedi_Sale__GNs != null && _ExpMestMedi_Sale__GNs.Count > 0)
                        {
                            store.RunPrintTemplate("Mps000349", deletePrintTemplate);
                        }
                        if (_ExpMestMedi_Sale__HTs != null && _ExpMestMedi_Sale__HTs.Count > 0)
                        {
                            store.RunPrintTemplate("Mps000350", deletePrintTemplate);
                        }
                        if ((_ExpMestMedi_Sale__TPCNs != null && _ExpMestMedi_Sale__TPCNs.Count > 0) || (_ExpMestMaterial_Sale_Prints != null && _ExpMestMaterial_Sale_Prints.Count > 0))
                        {
                            store.RunPrintTemplate("Mps000351", deletePrintTemplate);
                        }
                        if (_ExpMestMedi_Sale__Ts != null && _ExpMestMedi_Sale__Ts.Count > 0)
                        {
                            store.RunPrintTemplate("Mps000352", deletePrintTemplate);
                        }
                    }
                    else
                    {
                        store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, deletePrintTemplate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHoaDonBienLaiXuatBan(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("begin onClickInHoaDonBienLaiXuatBan");
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                store.RunPrintTemplate("Mps000339", deletePrintTemplate);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHuongDanSuDung(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInDonThuoc(object sender, EventArgs e)
        {
            try
            {
                InMps000044();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeWorker.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092:
                            InPhieuXuatBan(printTypeCode, fileName);
                            break;
                        case PrintTypeCodeWorker.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099:
                            InHuongDanSuDungThuoc(printTypeCode, fileName);
                            break;
                        case "Mps000349":
                            Mps000349(printTypeCode, fileName);
                            break;
                        case "Mps000350":
                            Mps000350(printTypeCode, fileName);
                            break;
                        case "Mps000351":
                            Mps000351(printTypeCode, fileName);
                            break;
                        case "Mps000352":
                            Mps000352(printTypeCode, fileName);
                            break;
                        case "Mps000339":
                            Mps000339(printTypeCode, fileName);
                            break;

                        default:
                            break;
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

        private void InPhieuXuatBan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("_ExpMest_Sale_Print__One " + Inventec.Common.Logging.LogUtil.TraceData("", _ExpMest_Sale_Print__One));
                var hisCashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                CommonParam param = new CommonParam();
                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                if (_ExpMest_Sale_Print__One.FirstOrDefault().TDL_TREATMENT_ID.HasValue)
                {
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = _ExpMest_Sale_Print__One.FirstOrDefault().TDL_TREATMENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                        treatment = listTreatment.FirstOrDefault();
                }

                if ((treatment == null || treatment.ID == 0) && _ExpMest_Sale_Print__One.FirstOrDefault().TDL_PATIENT_ID.HasValue)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisTreatmentViewFilter filterView = new HisTreatmentViewFilter();
                    filterView.PATIENT_ID = _ExpMest_Sale_Print__One.FirstOrDefault().TDL_PATIENT_ID;
                    var listTreatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filterView, paramCommon);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        treatment = listTreatment.OrderByDescending(o => o.TREATMENT_CODE).FirstOrDefault();
                    }
                }

                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(_ExpMest_Sale_Print__One, _ExpMestMedicine_Sale_Prints, _ExpMestMaterial_Sale_Prints, _Transaction_Sale_Print, hisCashierRoom, treatment);
                Inventec.Common.Logging.LogSystem.Debug("End onClickInPhieuXuatBan (before ShowDialog)");
                if (this.savePrint)
                {
                    if (chkPrintNow.CheckState == CheckState.Checked)
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug("End onClickInPhieuXuatBan (after ShowDialog)");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000349(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000349.PDO.Mps000349PDO rdo = new MPS.Processor.Mps000349.PDO.Mps000349PDO(_ExpMest_Sale_Print__One, _ExpMestMedi_Sale__GNs, _Transaction_Sale_Print);
                if (this.savePrint)
                {
                    if (chkPrintNow.CheckState == CheckState.Checked)
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //-B1: gán giá trị vào List<HIS_EXP_MEST> và List<HIS_SERVICE_REQ> trong MOS.SDO.OutPatientPresResultSDO
        //-B2: Rồi truyền vào Library.PrintPrescription.PrintPrescriptionProcessor List<MOS.SDO.OutPatientPresResultSDO> (giá trị ở B1 chuyển về dạng list) và Inventec.Desktop.Common.Modules.Module
        //-B3 từ hàm ở B2 gọi vào hàm Print để in
        //(Tham khảo chức năng danh sách y lệnh (HIS.Desktop.Plugins.ServiceReqList))
        private void Mps000044(HisExpMestSaleListResultSDO resultSDO)
        {
            try
            {
                List<V_HIS_EXP_MEST> listExpMest = new List<V_HIS_EXP_MEST>();
                List<HIS_SERVICE_REQ> serviceReqPrints = new List<HIS_SERVICE_REQ>();
                List<HIS_EXP_MEST> expMestPrints = new List<HIS_EXP_MEST>();
                if (resultSDO != null && resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                {
                    listExpMest = resultSDO.ExpMestSdos.Select(s => s.ExpMest).ToList();

                    foreach (var item in listExpMest)
                    {
                        HIS_EXP_MEST expMest = new HIS_EXP_MEST();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(expMest, item);
                        expMestPrints.Add(expMest);
                    }

                    //lấy đơn thuốc tư phiếu xuất

                    List<string> serviceReqCode = listExpMest.Select(s => s.TDL_SERVICE_REQ_CODE).ToList();
                    if (serviceReqCode != null && serviceReqCode.Count() > 0)
                    {
                        CommonParam parama = new CommonParam();
                        HisServiceReqFilter HisServiceReq = new HisServiceReqFilter();
                        HisServiceReq.SERVICE_REQ_CODES = serviceReqCode;
                        serviceReqPrints = new BackendAdapter(parama).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, HisServiceReq, parama);
                    }
                    if (serviceReqPrints != null && serviceReqPrints.Count() > 0)
                    {

                    }
                    //số lượng đơn thuốc với số lượng phiếu xuất khác nhau sẽ không in được
                    if (serviceReqPrints.Count < expMestPrints.Count)
                    {
                        int dem = 0;
                        foreach (var item in expMestPrints)
                        {
                            //không có thông tin đơn thuốc thì tạo thông tin đơn thuốc.
                            if (!item.SERVICE_REQ_ID.HasValue)
                            {
                                dem--;

                                HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(req, item);
                                item.SERVICE_REQ_ID = dem;
                                req.ID = item.SERVICE_REQ_ID ?? 0;
                                req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                                req.PARENT_ID = item.TDL_PATIENT_ID;
                                serviceReqPrints.Add(req);
                            }
                            else if (!serviceReqPrints.Exists(o => o.ID == item.SERVICE_REQ_ID))
                            {
                                //nếu có service_req_id mà không get được thông tin thì cũng tạo đơn giả để in
                                HIS_SERVICE_REQ req = new HIS_SERVICE_REQ();
                                req.ID = item.SERVICE_REQ_ID ?? 0;
                                req.TREATMENT_ID = item.TDL_TREATMENT_ID ?? 0;
                                req.PARENT_ID = item.TDL_PATIENT_ID;
                                serviceReqPrints.Add(req);
                            }
                        }
                    }
                }

                if (listExpMest.Count <= 0)
                {
                    serviceReqPrints = dataServiceReqs;
                }



                MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                sdo.ExpMests = expMestPrints;
                sdo.ServiceReqs = serviceReqPrints;

                //  Library.PrintPrescription.PrintPrescriptionProcessor processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModule);
                // processPress.Print(MPS.Processor.Mps000044.PDO.Mps000044PDO.PrintTypeCode, false);
                var PrintServiceReqProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModule);
                // .Print(printTypeCode, false);
                PrintServiceReqProcessor.Print(MPS.Processor.Mps000044.PDO.Mps000044PDO.PrintTypeCode, false);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Mps000350(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000350.PDO.Mps000350PDO rdo = new MPS.Processor.Mps000350.PDO.Mps000350PDO(_ExpMest_Sale_Print__One, _ExpMestMedi_Sale__HTs, _Transaction_Sale_Print);
                if (this.savePrint)
                {
                    if (chkPrintNow.CheckState == CheckState.Checked)
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000351(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000351.PDO.Mps000351PDO rdo = new MPS.Processor.Mps000351.PDO.Mps000351PDO(_ExpMest_Sale_Print__One, _ExpMestMedi_Sale__TPCNs, _ExpMestMaterial_Sale_Prints, _Transaction_Sale_Print);
                if (this.savePrint)
                {
                    if (chkPrintNow.CheckState == CheckState.Checked)
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000352(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                MPS.Processor.Mps000352.PDO.Mps000352PDO rdo = new MPS.Processor.Mps000352.PDO.Mps000352PDO(_ExpMest_Sale_Print__One, _ExpMestMedi_Sale__Ts, _Transaction_Sale_Print);

                if (this.savePrint)
                {
                    if (chkPrintNow.CheckState == CheckState.Checked)
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    else
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000339(string printTypeCode, string fileName)
        {
            try
            {
                if (isTwoPatient && ListResultSDO != null && ListResultSDO.Count > 0)
                {
                    foreach (var item in ListResultSDO)
                    {
                        this.resultSDO = new HisExpMestSaleListResultSDO();
                        this.resultSDO = item;
                        if (this.resultSDO != null)
                        {
                            InMps000339(printTypeCode, fileName, this.resultSDO);
                        }
                    }
                }
                else if (!isTwoPatient)
                {
                    InMps000339(printTypeCode, fileName, this.resultSDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InHuongDanSuDungThuoc(string printTypeCode, string fileName)
        {
            try
            {
                if (isTwoPatient && ListResultSDO != null && ListResultSDO.Count > 0)
                {
                    foreach (var item in ListResultSDO)
                    {
                        this.resultSDO = new HisExpMestSaleListResultSDO();
                        this.resultSDO = item;
                        if (this.resultSDO != null)
                        {
                            InHDSD(printTypeCode, fileName, this.resultSDO);
                        }
                    }
                }
                else if (!isTwoPatient)
                {
                    InHDSD(printTypeCode, fileName, this.resultSDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InHDSD(string printTypeCode, string fileName, HisExpMestSaleListResultSDO resultSDO)
        {
            bool result = false;
            try
            {
                List<long> expMestIdTemps = new List<long>();
                if (resultSDO == null || resultSDO.ExpMestSdos != null && resultSDO.ExpMestSdos.Count > 0)
                {
                    expMestIdTemps = resultSDO.ExpMestSdos.Select(p => p.ExpMest).Select(p => p.ID).Distinct().ToList();
                }

                if (expMestIdTemps.Count == 0)
                {
                    return;
                }
                CommonParam param = new CommonParam();
                HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                expMestFilter.IDs = expMestIdTemps;
                V_HIS_EXP_MEST expMests = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param).FirstOrDefault();

                HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                expMestMedicineFilter.EXP_MEST_IDs = expMestIdTemps;
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                MPS.Processor.Mps000099.PDO.Mps000099PDO rdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(expMests, expMestMedicines);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InMps000339(string printTypeCode, string fileName, HisExpMestSaleListResultSDO resultSDO)
        {
            bool result = false;
            try
            {
                if (resultSDO == null || resultSDO.ExpMestSdos == null || resultSDO.ExpMestSdos.Count <= 0)
                {
                    LogSystem.Debug("Result Sdo is empty: \n" + LogUtil.TraceData("resultSDO", resultSDO));
                    return;
                }
                List<string> expMestCodes = this.resultSDO.ExpMestSdos.Where(o => !o.ExpMest.BILL_ID.HasValue).Select(s => s.ExpMest.EXP_MEST_CODE).ToList();
                if (expMestCodes.Count > 0)
                {
                    XtraMessageBox.Show(String.Format("Phiếu xuất chưa tạo giao dịch: {0}", String.Join(",", expMestCodes)), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DevExpress.Utils.DefaultBoolean.True);
                    return;
                }

                CommonParam param = new CommonParam();

                HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                tranFilter.IDs = this.resultSDO.ExpMestSdos.Select(s => s.ExpMest.BILL_ID.Value).Distinct().ToList();
                List<V_HIS_TRANSACTION> lstTransaction = new BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);

                HisBillGoodsFilter billGroodsFilter = new HisBillGoodsFilter();
                billGroodsFilter.BILL_IDs = this.resultSDO.ExpMestSdos.Select(s => s.ExpMest.BILL_ID.Value).Distinct().ToList();
                List<HIS_BILL_GOODS> lstGroods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGroodsFilter, param);

                foreach (V_HIS_TRANSACTION tran in lstTransaction)
                {
                    List<V_HIS_EXP_MEST_MEDICINE> medicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                    List<V_HIS_EXP_MEST_MATERIAL> materials = new List<V_HIS_EXP_MEST_MATERIAL>();
                    List<V_HIS_EXP_MEST> listExpMest = new List<V_HIS_EXP_MEST>();

                    List<HisExpMestSaleResultSDO> expMests = this.resultSDO.ExpMestSdos.Where(o => o.ExpMest.BILL_ID == tran.ID).ToList();

                    foreach (HisExpMestSaleResultSDO item in expMests)
                    {
                        if (item.ExpMedicines != null)
                        {
                            medicines.AddRange(item.ExpMedicines);
                        }

                        if (item.ExpMaterials != null)
                        {
                            materials.AddRange(item.ExpMaterials);
                        }

                        if (item.ExpMest != null)
                        {
                            listExpMest.Add(item.ExpMest);
                        }
                    }

                    MPS.Processor.Mps000339.PDO.Mps000339PDO rdo = new MPS.Processor.Mps000339.PDO.Mps000339PDO(tran, lstGroods.Where(o => o.BILL_ID == tran.ID).ToList(), medicines, materials, listExpMest);

                    if (this.savePrint)
                    {
                        if (chkPrintNow.CheckState == CheckState.Checked)
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
                        else
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                        }
                        else
                        {
                            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, ""));
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

        private void InMps000044()
        {
            try
            {
                if (isTwoPatient && ListResultSDO != null && ListResultSDO.Count > 0)
                {
                    foreach (var item in ListResultSDO)
                    {
                        this.resultSDO = new HisExpMestSaleListResultSDO();
                        this.resultSDO = item;
                        if (this.resultSDO != null)
                        {
                            Mps000044(this.resultSDO);
                        }
                    }
                }
                else if (!isTwoPatient)
                {
                    Mps000044(this.resultSDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
