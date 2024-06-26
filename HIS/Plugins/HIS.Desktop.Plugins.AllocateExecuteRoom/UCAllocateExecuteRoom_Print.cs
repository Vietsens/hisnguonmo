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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Logging;
using HIS.Desktop.Controls.Session;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.HisConfig;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.AllocateExecuteRoom.ADO;
using HIS.Desktop.Plugins.AllocateExecuteRoom.Base;
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using HIS.Desktop.Utility;
using Inventec.Common.ThreadCustom;
using HIS.Desktop.Print;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraBars;
namespace HIS.Desktop.Plugins.AllocateExecuteRoom
{
    public partial class UCAllocateExecuteRoom : UserControlBase
    {
        DevExpress.XtraBars.BarManager barManager = new DevExpress.XtraBars.BarManager();
        PopupMenu menu;
        private enum PrintType
        {
            InPhieuChiDinh,
            InSTT
        }
        private void InPhieuChiDinh(HIS_SERVICE_REQ _ServiceReq)
        {

            try
            {
                this.currentServiceReqPrint = new HIS_SERVICE_REQ();
                if (_ServiceReq != null)
                {
                    this.currentServiceReqPrint = _ServiceReq;
                    WaitingManager.Show();
                    if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK ||
                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT ||
                        currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT)
                    {
                        this.prescriptionPrint = null;
                        HisExpMestFilter expfilter = new HisExpMestFilter();
                        expfilter.SERVICE_REQ_ID = currentServiceReqPrint.ID;
                        var expMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, expfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                        if (expMest != null && expMest.Count > 0)
                        {
                            this.prescriptionPrint = expMest.FirstOrDefault();
                            if (this.prescriptionPrint.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DM)
                            {
                                InDonThuocVatTu();
                            }
                        }
                        else
                        {
                            InDonThuocVatTu();
                        }
                    }
                    else if (currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM)
                    {
                        //Đơn máu
                        PrintBlood();
                    }
                    else
                        ProcessingPrintV2();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InSTT(V_HIS_SERVICE_REQ _ServiceReq)
        {
            try
            {
                if (_ServiceReq != null)
                {
                    WaitingManager.Show();
                    Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richStore.RunPrintTemplate("Mps000445", this.DelegateRunPrinter);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {

                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108:
                        InPhieuYeuCauChiDinhMau(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000445":
                        LoadBieuMauSTT(printTypeCode, fileName, ref result);
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

        private void InDonThuocVatTu()
        {
            try
            {
                if (prescriptionPrint != null || currentServiceReqPrint != null)
                {
                    bool isNull = false;
                    if (prescriptionPrint == null)
                    {
                        isNull = true;
                        prescriptionPrint = new HIS_EXP_MEST();
                    }

                    if (currentServiceReqPrint == null)
                    {
                        isNull = true;
                        currentServiceReqPrint = new HIS_SERVICE_REQ();
                    }

                    MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                    sdo.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>() { prescriptionPrint };
                    sdo.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>() { currentServiceReqPrint };

                    Library.PrintPrescription.PrintPrescriptionProcessor processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModule);

                    processPress.Print(MPS.Processor.Mps000044.PDO.Mps000044PDO.PrintTypeCode, false);

                    if (isNull)
                    {
                        prescriptionPrint = null;
                        currentServiceReqPrint = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintBlood()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InPhieuYeuCauChiDinhMau(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                //Lay thong tin dich vu kham
                HisServiceReqViewFilter serviceFilter = new HisServiceReqViewFilter();
                serviceFilter.ID = this.currentServiceReqPrint.ID;
                V_HIS_SERVICE_REQ examServiceReq = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon).FirstOrDefault();

                HisExpMestFilter expMestFilter = new HisExpMestFilter();
                expMestFilter.SERVICE_REQ_ID = examServiceReq.ID;
                var expMest = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GET, ApiConsumers.MosConsumer, expMestFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon).FirstOrDefault();

                HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = examServiceReq.TREATMENT_ID;
                var treatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon).FirstOrDefault();

                List<V_HIS_EXP_MEST_BLTY_REQ_1> expMestMeties = null;
                List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods_Print = null;
                if (expMest != null)
                {
                    MOS.Filter.HisExpMestBltyReqView1Filter expMestMetyFilter = new MOS.Filter.HisExpMestBltyReqView1Filter();
                    expMestMetyFilter.EXP_MEST_ID = expMest.ID;
                    expMestMeties = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>("api/HisExpMestBltyReq/GetView1", ApiConsumers.MosConsumer, expMestMetyFilter, paramCommon);

                    MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                    bloodFilter.EXP_MEST_ID = expMest.ID;
                    _ExpMestBloods_Print = new BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, paramCommon);
                }

                string treatmentCode = (treatment != null ? treatment.TREATMENT_CODE : "");

                MPS.Processor.Mps000108.PDO.Mps000108PDO mps000108RDO = new MPS.Processor.Mps000108.PDO.Mps000108PDO(
                    expMest,
                    expMestMeties,
                    treatment,
                    examServiceReq,
                    _ExpMestBloods_Print
               );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW") == "1")
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000108RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000108RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000108RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessingPrintV2()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)//Khám
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)
                //Siêu âm
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)//Xét nghiệm
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS)
                //Nội soi
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)
                //Thăm dò chức năng
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)
                //Thủ thuật
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)
                //Phẫu thuật
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)
                //Chẩn đoán hình ảnh
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028);
                }
                //else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisServiceReqTypeCFG.SERVICE_REQ_TYPE_ID__PRES)//Thuốc vật tư
                //{
                //    richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_DON_THUOC_TONG_HOP__MPS000118, DelegateRunPrinter);
                //}
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN)
                //Phục hồi chức năng
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC)
                //Khác
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G)
                //Giường
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)
                //giải phẫu bệnh lý
                {
                    InPhieuYeuCauDichVu("Mps000167");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadBieuMauSTT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                MPS.Processor.Mps000445.PDO.Mps000445PDO pdo = new MPS.Processor.Mps000445.PDO.Mps000445PDO(serviceReqPrintRaw);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(serviceReqPrintRaw.TDL_TREATMENT_CODE, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplicationWorker.Get<string>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW") == "1")
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InitMenuPrint()
        {
            try
            {
                this.barManager.Form = this;
                if (this.menu == null)
                    this.menu = new PopupMenu(this.barManager);
                this.menu.ItemLinks.Clear();

                BarButtonItem itemInDvKham = new BarButtonItem(barManager, "In phiếu chỉ định", 1);
                itemInDvKham.Tag = PrintType.InPhieuChiDinh;
                itemInDvKham.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemInDvKham);

                BarButtonItem itemPrintDangKyKham = new BarButtonItem(barManager, "In số thứ tự", 1);
                itemPrintDangKyKham.Tag = PrintType.InSTT;
                itemPrintDangKyKham.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemPrintDangKyKham);



                this.menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClick__Pluss(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintType type = (PrintType)(e.Item.Tag);

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    CommonParam param = new CommonParam();
                    HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.ID = resultPrint.ID;
                    var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                      "api/HisServiceReq/GetView",
                      HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                      filter,
                      param);
                    if (data != null && data.Count > 0)
                    {
                        serviceReqPrintRaw = data.FirstOrDefault();
                    }
                    switch (type)
                    {
                        case PrintType.InPhieuChiDinh:
                            InPhieuChiDinh(resultPrint);
                            break;

                        case PrintType.InSTT:
                            InSTT(serviceReqPrintRaw);
                            break;
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
