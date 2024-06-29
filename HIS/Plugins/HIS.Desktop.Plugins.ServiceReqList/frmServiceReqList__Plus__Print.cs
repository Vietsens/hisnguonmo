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
using AutoMapper;
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using HIS.Desktop.Plugins.ServiceReqList.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HIS.Desktop.ADO;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using EMR.Filter;
using EMR.EFMODEL.DataModels;
using System.IO;
using Inventec.Common.SignLibrary;
using System.Drawing;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using EMR.SDO;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.ServiceReqList
{
    public partial class frmServiceReqList : HIS.Desktop.Utility.FormBase
    {
        private const string MPS000167 = "Mps000167";
        private const string MPS000033 = "Mps000033";
        private const string MPS000035 = "Mps000035";
        private const string MPS000037 = "Mps000037";
        private const string MPS000097 = "Mps000097";
        private const string MPS000063 = "Mps000063";
        private const string MPS000234 = "Mps000234";
        private const string MPS000204 = "Mps000204";
        private const string MPS000433 = "Mps000433";
        private const string PRINT_TYPE_CODE__PHIEU_THU_KIEM_YC_KHAM__MPS000420 = "Mps000420";
        private const int Thuoc = 1;
        private const int VatTu = 2;
        private const int ThuocNgoaiKho = 3;
        private const int VatTuNgoaiKho = 4;
        private const int TuTuc = 5;
        internal long printChangeServiceId = 0;
        int SetDefaultDepositPrice = Inventec.Common.TypeConvert.Parse.ToInt32(HisConfigs.Get<string>(Base.ConfigKey.HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT));
        long patientTypeId_Bhyt = Inventec.Common.TypeConvert.Parse.ToInt32(HisConfigs.Get<string>(Base.ConfigKey.PATIENT_TYPE_ID__BHYT));
        List<ListMedicineADO> lstSereServSelected = new List<ListMedicineADO>();

        #region Xử lý
        private void PrintMedicine_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem /*&& this.prescriptionPrint != null*/)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);
                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType.donThuoc:
                            InDonThuocVatTu();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.thuocTongHop:
                            InPhieuYeuCauThuocVatTuTongHop();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.donThuocYHCT:
                            InPhieuYeuCauDonThuocYHocCoTruyen();
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
        }

        private void PrintExam_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType.chuyenKhoa:
                            InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__YEU_CAU_KHAM_CHUYEN_KHOA__MPS000071);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.PhieuThuKiemPhieuYcKham:
                            InPhieuThuKiemYcKham_Click();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.kham:
                            InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.TheBenhNhan:
                            InTheBenhNhan_Click();
                            break;
                        case PrintPopupMenuProcessor.ModuleType.HenKhamLai:
                            InHenKhamLai_Click();
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
        }

        private void PrintPttt_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType.Mps000033:
                            richEditorMain.RunPrintTemplate(MPS000033, DelegateRunPrinter);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Mps000035:
                            richEditorMain.RunPrintTemplate(MPS000035, DelegateRunPrinter);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Mps000097:
                            richEditorMain.RunPrintTemplate(MPS000097, DelegateRunPrinter);
                            break;
                        case PrintPopupMenuProcessor.ModuleType.Mps000204:
                            richEditorMain.RunPrintTemplate(MPS000204, DelegateRunPrinter);
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

        private void PrintTest_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintPopupMenuProcessor.ModuleType type = (PrintPopupMenuProcessor.ModuleType)(e.Item.Tag);

                    switch (type)
                    {
                        case PrintPopupMenuProcessor.ModuleType._testPhieuYeuCau:
                            InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026);
                            break;
                        case PrintPopupMenuProcessor.ModuleType._testDomSoi:
                            onClickInKetQuaKhacPlus(null, null);
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
        }

        private void ProcessingPrint()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)
                //Siêu âm
                {
                    InPhieuYeuCauDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030);
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
                    InPhieuYeuCauDichVu(MPS000167);
                }
                else if (this.currentServiceReqPrint.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)//Suất ăn
                {
                    richEditorMain.RunPrintTemplate("Mps000275", DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrintResult()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                if (this.sereServPrint.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN)
                //Phục hồi chức năng
                {
                    richEditorMain.RunPrintTemplate(MPS000063, DelegateRunPrinter);
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
                    case MPS000033:
                        LoadBieuMauPhieuThuThuatPhauThuat(printTypeCode, fileName, ref result);
                        break;
                    case MPS000035:
                        LoadBieuMauPhieuYCInGiayCamDoan(printTypeCode, fileName, ref result);
                        break;
                    case MPS000063:
                        InPhieuKetQuaPHCN(printTypeCode, fileName, ref result);
                        break;
                    case MPS000097:
                        LoadBieuMauCachThucPhauThuat(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000108:
                        InPhieuYeuCauChiDinhMau(printTypeCode, fileName, ref result);
                        break;
                    case MPS000204:
                        LoadGiayChungNhanPTTT(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000275":
                        InSuatAn(printTypeCode, fileName, null, ref result);
                        break;
                    case MPS000433:
                        InGiayDeNghiDoiTraDichVu(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000494":
                        PrintMps494(printTypeCode, fileName, ref result);
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

        private void PrintMps494(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    var data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data != null && data.ID > 0)
                    {
                        HisExpMestMaterialFilter filter = new HisExpMestMaterialFilter();
                        filter.TDL_SERVICE_REQ_ID = data.ID;

                        var lstMatePrintMps494 = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, filter, null);
                        if (lstMatePrintMps494 == null || lstMatePrintMps494.Count == 0)
                        {
                            XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                            return;
                        }
                        var expmestMate = lstMatePrintMps494.Where(o => !string.IsNullOrEmpty(o.SERIAL_NUMBER) && o.REMAIN_REUSE_COUNT != null).ToList();
                        if (expmestMate == null || expmestMate.Count == 0)
                        {
                            XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                            return;
                        }
                        List<HIS_MATERIAL> mate = new List<HIS_MATERIAL>();
                        var materialIdList = expmestMate.Select(p => p.MATERIAL_ID ?? 0).ToList();
                        if (materialIdList != null && materialIdList.Count > 0)
                        {
                            MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                            materialFilter.IDs = materialIdList;
                            mate = new BackendAdapter(new CommonParam()).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                        }
                        List<MPS.Processor.Mps000494.PDO.SerialADO> lstSend = new List<MPS.Processor.Mps000494.PDO.SerialADO>();
                        foreach (var m in expmestMate)
                        {
                            MPS.Processor.Mps000494.PDO.SerialADO ado = new MPS.Processor.Mps000494.PDO.SerialADO();
                            var te = mate.FirstOrDefault(o => o.ID == m.MATERIAL_ID);
                            if (te != null)
                            {
                                ado.NEXT_REUSABLE_NUMBER = ((te.MAX_REUSE_COUNT ?? 0) - (m.REMAIN_REUSE_COUNT ?? 0) + 2).ToString();
                                ado.SIZE = te.MATERIAL_SIZE;
                                if (Int32.Parse(ado.NEXT_REUSABLE_NUMBER) > (te.MAX_REUSE_COUNT ?? 0))
                                    continue;
                            }
                            else
                                continue;
                            ado.SERIAL_NUMBER = m.SERIAL_NUMBER;
                            lstSend.Add(ado);
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstSend), lstSend));
                        if (lstSend == null || lstSend.Count == 0)
                        {
                            XtraMessageBox.Show("Không có vật tư tái sử dụng hoặc vật tư tái sử dụng đã hết số lần tái sử dụng.");
                            return;
                        }
                        MPS.Processor.Mps000494.PDO.Mps000494PDO rdo = new MPS.Processor.Mps000494.PDO.Mps000494PDO(lstSend);

                        RunPrint(printTypeCode, fileName, rdo, (Inventec.Common.FlexCelPrint.DelegateEventLog)EventLogPrint, result, currentModule.RoomId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void EventLogPrint()
        {
            try
            {
                var row = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                string message = "In tem vật tư tái sử dụng. Mã in : Mps000494" + "  TREATMENT_CODE: " + row.TDL_TREATMENT_CODE + "  Thời gian in: " + Inventec.Common.DateTime.Convert.SystemDateTimeToTimeSeparateString(DateTime.Now) + "  Người in: " + Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.EventLog.Logger.Log(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private HIS_TREATMENT getTreatment(long treatmentId)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get HIS_TREATMENT");
            CommonParam param = new CommonParam();
            HIS_TREATMENT currentHisTreatment = new HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentFilter();
                hisTreatmentFilter.ID = treatmentId;
                var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, hisTreatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get HIS_TREATMENT");
            return currentHisTreatment;
        }

        private V_HIS_PATIENT_TYPE_ALTER getPatientTypeAlter(long treatmentId, long instructTime)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get HispatientTypeAlter");
            CommonParam param = new CommonParam();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER currentHispatientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                MOS.Filter.HisPatientTypeAlterViewAppliedFilter hisPTAlterFilter = new HisPatientTypeAlterViewAppliedFilter();
                hisPTAlterFilter.TreatmentId = treatmentId;
                if (instructTime > 0)
                    hisPTAlterFilter.InstructionTime = instructTime;
                else
                    hisPTAlterFilter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                currentHispatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, hisPTAlterFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
            }
            catch (Exception ex)
            {
                currentHispatientTypeAlter = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get HispatientTypeAlter");
            return currentHispatientTypeAlter;
        }

        private List<V_HIS_SERE_SERV> GetSereServByServiceReqId(long serviceReqId)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get List<V_HIS_SERE_SERV>");
            List<V_HIS_SERE_SERV> result = new List<V_HIS_SERE_SERV>();
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.SERVICE_REQ_ID = serviceReqId;
                var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV>>(Base.GlobalStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, sereServFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    result = apiResult.Where(o => o.IS_NO_EXECUTE != Base.GlobalStore.IS_TRUE).ToList();
                    //foreach (var item in apiResult)
                    //{
                    //    V_HIS_SERE_SERV ss11 = item;

                    //    var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ss11.SERVICE_ID);
                    //    if (service != null)
                    //    {
                    //        ss11.TDL_SERVICE_CODE = service.SERVICE_CODE;
                    //        ss11.TDL_SERVICE_NAME = service.SERVICE_NAME;
                    //        ss11.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                    //        ss11.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                    //        ss11.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                    //        ss11.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                    //        ss11.HEIN_SERVICE_TYPE_CODE = service.HEIN_SERVICE_TYPE_CODE;
                    //        ss11.HEIN_SERVICE_TYPE_NAME = service.HEIN_SERVICE_TYPE_NAME;
                    //        ss11.HEIN_SERVICE_TYPE_NUM_ORDER = service.HEIN_SERVICE_TYPE_NUM_ORDER;
                    //    }

                    //    var executeRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                    //    if (executeRoom != null)
                    //    {
                    //        ss11.EXECUTE_ROOM_CODE = executeRoom.ROOM_CODE;
                    //        ss11.EXECUTE_ROOM_NAME = executeRoom.ROOM_NAME;
                    //        ss11.EXECUTE_DEPARTMENT_CODE = executeRoom.DEPARTMENT_CODE;
                    //        ss11.EXECUTE_DEPARTMENT_NAME = executeRoom.DEPARTMENT_NAME;
                    //    }

                    //    var reqRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_REQUEST_ROOM_ID);
                    //    if (reqRoom != null)
                    //    {
                    //        ss11.REQUEST_DEPARTMENT_CODE = reqRoom.DEPARTMENT_CODE;
                    //        ss11.REQUEST_DEPARTMENT_NAME = reqRoom.DEPARTMENT_NAME;
                    //        ss11.REQUEST_ROOM_CODE = reqRoom.ROOM_CODE;
                    //        ss11.REQUEST_ROOM_NAME = reqRoom.ROOM_NAME;
                    //    }

                    //    var patientTpye = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                    //    if (patientTpye != null)
                    //    {
                    //        ss11.PATIENT_TYPE_CODE = patientTpye.PATIENT_TYPE_CODE;
                    //        ss11.PATIENT_TYPE_NAME = patientTpye.PATIENT_TYPE_NAME;
                    //    }
                    //    result.Add(ss11);
                    //}
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_SERE_SERV>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get List<V_HIS_SERE_SERV>");
            return result;
        }

        private void PrintData(string printTypeCode, string fileName, object data, ref bool result)
        {
            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                }
                else
                {
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.treatmentCode, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, data, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Chức năng in của từng loại yêu cầu dịch vụ
        #region in chỉ định
        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                if (serviceReqPrintRaw != null)
                {
                    ThreadChiDinhDichVuADO data = new ThreadChiDinhDichVuADO(this.currentServiceReqPrint);
                    CreateThreadLoadDataForService(data);

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                    LoadCurrentPatientTypeAlter(this.currentServiceReqPrint.TREATMENT_ID, ref patientTypeAlter);
                    data.vHisPatientTypeAlter = patientTypeAlter;

                    HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                    HisServiceReqSDO.SereServs = data.listVHisSereServ;
                    HisServiceReqSDO.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { this.serviceReqPrintRaw };
                    HisServiceReqSDO.SereServBills = data.ListSereServBill;
                    HisServiceReqSDO.SereServDeposits = data.ListSereServDeposit;

                    List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_ID = serviceReqPrintRaw.TREATMENT_ID;
                    var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                    if (resultBedlog != null)
                    {
                        listBedLogs = resultBedlog;
                    }

                    HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, data.hisTreatment);

                    if (data.vHisPatientTypeAlter != null)
                    {
                        HisTreatment.PATIENT_TYPE_CODE = data.vHisPatientTypeAlter.PATIENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_FROM_TIME = data.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        HisTreatment.HEIN_CARD_NUMBER = data.vHisPatientTypeAlter.HEIN_CARD_NUMBER;
                        HisTreatment.HEIN_CARD_TO_TIME = data.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        HisTreatment.HEIN_MEDI_ORG_CODE = data.vHisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        HisTreatment.LEVEL_CODE = data.vHisPatientTypeAlter.LEVEL_CODE;
                        HisTreatment.RIGHT_ROUTE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_CODE;
                        HisTreatment.RIGHT_ROUTE_TYPE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        HisTreatment.TREATMENT_TYPE_CODE = data.vHisPatientTypeAlter.TREATMENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_ADDRESS = data.vHisPatientTypeAlter.ADDRESS;
                    }

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InPhieuThuKiemYcKham_Click()
        {
            try
            {
                if (serviceReqPrintRaw != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate(PRINT_TYPE_CODE__PHIEU_THU_KIEM_YC_KHAM__MPS000420, InPhieuThuKiemYcKham);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InTheBenhNhan_Click()
        {
            try
            {
                if (serviceReqPrintRaw != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("Mps000178", InTheBenhNhan);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void InHenKhamLai_Click()
        {
            try
            {
                if (serviceReqPrintRaw != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richEditorMain.RunPrintTemplate("Mps000010", InHenKhamLai);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private bool InPhieuThuKiemYcKham(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (serviceReqPrintRaw == null)
                {
                    return false;
                }
                WaitingManager.Show();

                string printerName = "";

                MOS.Filter.HisSereServViewFilter sereServFilter = new HisSereServViewFilter();
                sereServFilter.SERVICE_REQ_ID = serviceReqPrintRaw.ID;

                var sereServByServiceReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, sereServFilter, null);

                if (sereServByServiceReqs != null && sereServByServiceReqs.Count > 0)
                {
                    HisSereServDepositFilter ssDepositFilter = new HisSereServDepositFilter();
                    ssDepositFilter.SERE_SERV_IDs = sereServByServiceReqs.Select(o => o.ID).ToList();
                    List<HIS_SERE_SERV_DEPOSIT> hisSSDeposits = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, ssDepositFilter, null);
                    if (hisSSDeposits != null && hisSSDeposits.Count > 0)
                    {
                        HisTransactionViewFilter transactionFilter = new HisTransactionViewFilter();
                        transactionFilter.IDs = hisSSDeposits.Select(o => o.DEPOSIT_ID).ToList();
                        var transactionList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, transactionFilter, null);
                        if (transactionList != null && transactionList.Count > 0)
                        {
                            foreach (var transaction in transactionList)
                            {
                                string treatmentCode = this.serviceReqPrintRaw.TREATMENT_CODE;
                                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                                MPS.Processor.Mps000420.PDO.Mps000420PDO pdo = new MPS.Processor.Mps000420.PDO.Mps000420PDO(
                                    transaction,
                                    sereServByServiceReqs,
                                    serviceReqPrintRaw
                                    );

                                WaitingManager.Hide();
                                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                                {
                                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                                }

                                MPS.ProcessorBase.Core.PrintData PrintData = null;
                                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                                }
                                else
                                {
                                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName);
                                }
                                PrintData.EmrInputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.serviceReqPrintRaw.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                                result = MPS.MpsPrinter.Run(PrintData);
                            }
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có giao dịch thanh toán", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Chưa có giao dịch thanh toán", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool InTheBenhNhan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (serviceReqPrintRaw == null)
                {
                    return false;
                }
                WaitingManager.Show();
                V_HIS_TREATMENT_4 treatment4 = null;
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;

                HisTreatmentView4Filter tFilter = new HisTreatmentView4Filter();
                tFilter.ID = serviceReqPrintRaw.TREATMENT_ID;
                List<V_HIS_TREATMENT_4> treatments = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, tFilter, null);
                treatment4 = treatments != null ? treatments.FirstOrDefault() : null;

                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                if (treatment4 != null && instructionTime < treatment4.IN_TIME)
                {
                    instructionTime = treatment4.IN_TIME;
                }
                PrintGlobalStore.LoadCurrentPatientTypeAlter(serviceReqPrintRaw.TREATMENT_ID, instructionTime, ref patientTypeAlter);
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = serviceReqPrintRaw.TDL_PATIENT_ID;
                var currentPatient = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
                WaitingManager.Hide();

                #region bo sung V_HIS_DEPARTMENT_TRAN
                V_HIS_DEPARTMENT_TRAN departmentDTO = new V_HIS_DEPARTMENT_TRAN();
                HisDepartmentTranViewFilter deFilter = new HisDepartmentTranViewFilter();
                deFilter.TREATMENT_ID = treatment4.ID;
                deFilter.ORDER_DIRECTION = "DESC";
                deFilter.ORDER_FIELD = "DEPARTMENT_IN_TIME";
                var dataAPI = new BackendAdapter(param).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, deFilter, param);

                if (dataAPI != null)
                {
                    var sortedList = dataAPI
                                    .OrderByDescending(o => o.DEPARTMENT_IN_TIME.HasValue)  // NULL lên đầu
                                    .ThenByDescending(o => o.DEPARTMENT_IN_TIME)            // Giảm dần theo DEPARTMENT_IN_TIME
                                    .ThenByDescending(o => o.ID)                            // Giảm dần theo ID
                                    .ToList();
                    departmentDTO = dataAPI.First();
                }
                #endregion

                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                   currentPatient,
                   patientTypeAlter,
                   treatment4,
                   departmentDTO
                   );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment4 != null ? treatment4.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
                result = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }


        private bool InHenKhamLai(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (serviceReqPrintRaw == null)
                {
                    return false;
                }

                WaitingManager.Show();

                HIS_MEDI_RECORD MediRecode = new HIS_MEDI_RECORD();

                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = serviceReqPrintRaw.TDL_PATIENT_ID;
                var currentPatient = new BackendAdapter(param)
                          .Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.serviceReqPrintRaw.TREATMENT_ID;
                HIS_TREATMENT HisTreatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();

                HIS_SERVICE_REQ ServiceReq = new HIS_SERVICE_REQ();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(ServiceReq, this.serviceReqPrintRaw);

                MPS.Processor.Mps000010.PDO.PatientADO PatientADO = new MPS.Processor.Mps000010.PDO.PatientADO(currentPatient);

                MPS.Processor.Mps000010.PDO.Mps000010ADO ado = new MPS.Processor.Mps000010.PDO.Mps000010ADO();
                if (HisTreatment.DEATH_CAUSE_ID != null)
                {
                    var deathCause = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_CAUSE>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_CAUSE_ID.Value);
                    if (deathCause != null)
                    {
                        ado.DEATH_CAUSE_CODE = deathCause.DEATH_CAUSE_CODE;
                        ado.DEATH_CAUSE_NAME = deathCause.DEATH_CAUSE_NAME;
                    }
                }
                if (HisTreatment.DEATH_WITHIN_ID != null)
                {
                    var deathWithin = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEATH_WITHIN>().FirstOrDefault(o => o.ID == HisTreatment.DEATH_WITHIN_ID.Value);
                    if (deathWithin != null)
                    {
                        ado.DEATH_WITHIN_CODE = deathWithin.DEATH_WITHIN_CODE;
                        ado.DEATH_WITHIN_NAME = deathWithin.DEATH_WITHIN_NAME;
                    }
                }
                if (HisTreatment.TRAN_PATI_FORM_ID.HasValue)
                {
                    var tranPatiForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_FORM_ID.Value);
                    if (tranPatiForm != null)
                    {
                        ado.TRAN_PATI_FORM_CODE = tranPatiForm.TRAN_PATI_FORM_CODE;
                        ado.TRAN_PATI_FORM_NAME = tranPatiForm.TRAN_PATI_FORM_NAME;
                    }
                }
                if (HisTreatment.TREATMENT_RESULT_ID.HasValue)
                {
                    var treatmentResult = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_RESULT>().FirstOrDefault(o => o.ID == HisTreatment.TREATMENT_RESULT_ID.Value);
                    if (treatmentResult != null)
                    {
                        ado.TREATMENT_RESULT_CODE = treatmentResult.TREATMENT_RESULT_CODE;
                        ado.TREATMENT_RESULT_NAME = treatmentResult.TREATMENT_RESULT_NAME;
                    }
                }
                if (HisTreatment.TRAN_PATI_REASON_ID.HasValue)
                {
                    var tranPatiReason = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().FirstOrDefault(o => o.ID == HisTreatment.TRAN_PATI_REASON_ID.Value);
                    if (tranPatiReason != null)
                    {
                        ado.TRAN_PATI_REASON_CODE = tranPatiReason.TRAN_PATI_REASON_CODE;
                        ado.TRAN_PATI_REASON_NAME = tranPatiReason.TRAN_PATI_REASON_NAME;
                    }
                }

                if (HisTreatment != null && HisTreatment.MEDI_RECORD_ID.HasValue)
                {
                    MOS.Filter.HisMediRecordFilter mediRecordFilter = new HisMediRecordFilter();
                    mediRecordFilter.ID = HisTreatment.MEDI_RECORD_ID.Value;
                    var MediRecodes = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_RECORD>>("api/HisMediRecord/Get", ApiConsumer.ApiConsumers.MosConsumer, mediRecordFilter, null);
                    MediRecode = MediRecodes != null && MediRecodes.Count > 0 ? MediRecodes.FirstOrDefault() : null;
                }

                if (MediRecode != null)
                {
                    ado.MEDI_RECORD_STORE_CODE = MediRecode.STORE_CODE;
                }
                if (HisTreatment.END_ROOM_ID.HasValue)
                {
                    var endRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.END_ROOM_ID.Value);
                    if (endRoom != null)
                    {
                        ado.END_DEPARTMENT_CODE = endRoom.DEPARTMENT_CODE;
                        ado.END_DEPARTMENT_NAME = endRoom.DEPARTMENT_NAME;
                        ado.END_ROOM_CODE = endRoom.ROOM_CODE;
                        ado.END_ROOM_NAME = endRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.FEE_LOCK_ROOM_ID.HasValue)
                {
                    var feelockRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.FEE_LOCK_ROOM_ID.Value);
                    if (feelockRoom != null)
                    {
                        ado.FEE_LOCK_DEPARTMENT_CODE = feelockRoom.DEPARTMENT_CODE;
                        ado.FEE_LOCK_DEPARTMENT_NAME = feelockRoom.DEPARTMENT_NAME;
                        ado.FEE_LOCK_ROOM_CODE = feelockRoom.ROOM_CODE;
                        ado.FEE_LOCK_ROOM_NAME = feelockRoom.ROOM_NAME;
                    }
                }
                if (HisTreatment.IN_ROOM_ID.HasValue)
                {
                    var inRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == HisTreatment.IN_ROOM_ID.Value);
                    if (inRoom != null)
                    {
                        ado.IN_DEPARTMENT_CODE = inRoom.DEPARTMENT_CODE;
                        ado.IN_DEPARTMENT_NAME = inRoom.DEPARTMENT_NAME;
                        ado.IN_ROOM_CODE = inRoom.ROOM_CODE;
                        ado.IN_ROOM_NAME = inRoom.ROOM_NAME;
                    }
                }
                if (ServiceReq.APPOINTMENT_EXAM_ROOM_ID != null)
                {
                    ado.APPOINTMENT_EXAM_ROOM_IDS = ServiceReq.APPOINTMENT_EXAM_ROOM_ID.ToString();
                    var executeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == ServiceReq.APPOINTMENT_EXAM_ROOM_ID);
                    if (executeRoom != null)
                    {
                        ado.APPOINTMENT_EXAM_ROOM_NAMES = executeRoom.EXECUTE_ROOM_NAME;
                        ado.APPOINTMENT_EXAM_ROOM_CODE_NAMES = executeRoom.EXECUTE_ROOM_CODE + " - " + executeRoom.EXECUTE_ROOM_NAME;
                    }
                }
                if (ServiceReq.APPOINTMENT_EXAM_SERVICE_ID != null)
                {
                    var service = BackendDataWorker.Get<HIS_SERVICE>().FirstOrDefault(o => o.ID == ServiceReq.APPOINTMENT_EXAM_SERVICE_ID);
                    if (service != null)
                    {
                        ado.APPOINTMENT_SERVICE_CODES = service.SERVICE_CODE;
                        ado.APPOINTMENT_SERVICE_NAMES = service.SERVICE_NAME;
                    }
                }
                //if (!string.IsNullOrEmpty(HisTreatment.APPOINTMENT_EXAM_ROOM_IDS))
                //{
                //    var _RoomExamADOs = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.IS_EXAM == 1).ToList();
                //    string[] ids = HisTreatment.APPOINTMENT_EXAM_ROOM_IDS.Split(',');
                //    List<string> _roomName = new List<string>();
                //    List<string> _roomCodeName = new List<string>();
                //    foreach (var item in _RoomExamADOs)
                //    {
                //        var dataCheck = ids.FirstOrDefault(p => p.Trim() == item.ROOM_ID.ToString().Trim());
                //        if (!string.IsNullOrEmpty(dataCheck))
                //        {
                //            _roomName.Add(item.EXECUTE_ROOM_NAME);
                //            _roomCodeName.Add(item.EXECUTE_ROOM_CODE + " - " + item.EXECUTE_ROOM_NAME);
                //        }
                //    }
                //    if (_roomName != null && _roomName.Count > 0)
                //        ado.APPOINTMENT_EXAM_ROOM_NAMES = string.Join(",", _roomName);
                //    if (_roomCodeName != null && _roomCodeName.Count > 0)
                //        ado.APPOINTMENT_EXAM_ROOM_CODE_NAMES = string.Join(",", _roomCodeName);
                //}

                MOS.Filter.HisAppointmentServViewFilter _ssFilter = new MOS.Filter.HisAppointmentServViewFilter();
                _ssFilter.TREATMENT_ID = serviceReqPrintRaw.TREATMENT_ID;
                var dataApps = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_APPOINTMENT_SERV>>("api/HisAppointmentServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, _ssFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);


                var appointmentPeriods = BackendDataWorker.Get<HIS_APPOINTMENT_PERIOD>();

                V_HIS_PATIENT_TYPE_ALTER VHisPatientTypeAlter = null;
                LoadCurrentPatientTypeAlter(this.serviceReqPrintRaw.TREATMENT_ID, ref VHisPatientTypeAlter);

                WaitingManager.Hide();

                MPS.Processor.Mps000010.PDO.Mps000010PDO mps000010RDO = new MPS.Processor.Mps000010.PDO.Mps000010PDO(
                  PatientADO,
                  VHisPatientTypeAlter,
                  HisTreatment,
                  ado,
                  dataApps,
                  appointmentPeriods,
                  ServiceReq
                  );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((HisTreatment != null ? HisTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000010RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                }
                result = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }


        public void LoadCurrentPatientTypeAlter(long treatmentId, ref MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter)
        {
            try
            {
                CommonParam param = new CommonParam();
                hisPatientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InPhieuYeuCauChiDinhTongHop(string printTypeCode)
        {
            try
            {
                if (this.listServiceReq != null && this.listServiceReq.Count > 0)
                {
                    ThreadChiDinhDichVuADO data = new ThreadChiDinhDichVuADO(this.listServiceReq.First());
                    CreateThreadLoadDataForChiDinhTongHop(data);

                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.IDs = listServiceReq.Select(o => o.ID).ToList();
                    serviceReqFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    List<V_HIS_SERVICE_REQ> lstSR = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);

                    HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                    HisServiceReqSDO.SereServs = data.listVHisSereServ;
                    HisServiceReqSDO.ServiceReqs = lstSR;

                    List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();

                    HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                    bedLogFilter.TREATMENT_ID = data.hisTreatment.ID;
                    var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                    if (resultBedlog != null)
                    {
                        listBedLogs = resultBedlog;
                    }

                    HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, data.hisTreatment);
                    if (data.vHisPatientTypeAlter != null)
                    {
                        HisTreatment.PATIENT_TYPE_CODE = data.vHisPatientTypeAlter.PATIENT_TYPE_CODE;
                        HisTreatment.HEIN_CARD_FROM_TIME = data.vHisPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        HisTreatment.HEIN_CARD_NUMBER = data.vHisPatientTypeAlter.HEIN_CARD_NUMBER;
                        HisTreatment.HEIN_CARD_TO_TIME = data.vHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        HisTreatment.HEIN_MEDI_ORG_CODE = data.vHisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        HisTreatment.LEVEL_CODE = data.vHisPatientTypeAlter.LEVEL_CODE;
                        HisTreatment.RIGHT_ROUTE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_CODE;
                        HisTreatment.RIGHT_ROUTE_TYPE_CODE = data.vHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        HisTreatment.TREATMENT_TYPE_CODE = data.vHisPatientTypeAlter.TREATMENT_TYPE_CODE;
                    }

                    var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                    PrintServiceReqProcessor.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InDonThuocTongHop(string printTypeCode)
        {
            try
            {
                var IsNotShow = lstConfig.Exists(o => o.IsChecked && o.ID == (int)ConfigADO.RowConfigID.KhongHienThiDonKhongLayODonThuocTH);
                if (this.listServiceReq != null && this.listServiceReq.Count > 0)
                {
                    List<MOS.SDO.OutPatientPresResultSDO> listOutPatientPresResultSDO = new List<OutPatientPresResultSDO>();

                    MOS.SDO.OutPatientPresResultSDO outPatientPresResultSDO = new OutPatientPresResultSDO();

                    AutoMapper.Mapper.CreateMap<ServiceReqADO, HIS_SERVICE_REQ>();
                    var listSeviceReqPrint = AutoMapper.Mapper.Map<List<HIS_SERVICE_REQ>>(this.listServiceReq);
                    outPatientPresResultSDO.ServiceReqs = listSeviceReqPrint;

                    CommonParam param = new CommonParam();

                    //Get ServiceReqMety
                    HisServiceReqMetyFilter hisServiceReqMetyFilter = new HisServiceReqMetyFilter();
                    hisServiceReqMetyFilter.SERVICE_REQ_IDs = this.listServiceReq.Select(o => o.ID).ToList();
                    var listHisServiceReqMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, hisServiceReqMetyFilter, param);
                    outPatientPresResultSDO.ServiceReqMeties = listHisServiceReqMety;

                    //Get ServiceReqMaty
                    HisServiceReqMatyFilter hisServiceReqMatyFilter = new HisServiceReqMatyFilter();
                    hisServiceReqMatyFilter.SERVICE_REQ_IDs = this.listServiceReq.Select(o => o.ID).ToList();
                    var listHisServiceReqMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, hisServiceReqMatyFilter, param);
                    outPatientPresResultSDO.ServiceReqMaties = listHisServiceReqMaty;

                    //Get ExpMest
                    HisExpMestFilter hisExpMestFilter = new HisExpMestFilter();
                    hisExpMestFilter.SERVICE_REQ_IDs = this.listServiceReq.Select(o => o.ID).ToList();
                    var listHisExpMest = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, hisExpMestFilter, param);
                    outPatientPresResultSDO.ExpMests = listHisExpMest;

                    //Get ExpMestMedicine
                    HisExpMestMedicineFilter hisExpMestMedicineFilter = new HisExpMestMedicineFilter();
                    hisExpMestMedicineFilter.TDL_SERVICE_REQ_IDs = this.listServiceReq.Select(o => o.ID).ToList();
                    var listHisExpMestMedicine = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/Get", ApiConsumers.MosConsumer, hisExpMestMedicineFilter, param);
                    outPatientPresResultSDO.Medicines = listHisExpMestMedicine;

                    //Get ExpMestMaterial
                    HisExpMestMaterialFilter hisExpMestMaterialFilter = new HisExpMestMaterialFilter();
                    hisExpMestMaterialFilter.TDL_SERVICE_REQ_IDs = this.listServiceReq.Select(o => o.ID).ToList();
                    var listHisExpMestMaterial = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/Get", ApiConsumers.MosConsumer, hisExpMestMaterialFilter, param);
                    outPatientPresResultSDO.Materials = listHisExpMestMaterial;

                    listOutPatientPresResultSDO.Add(outPatientPresResultSDO);

                    var PrintPresProcessor = new Library.PrintPrescription.PrintPrescriptionProcessor(listOutPatientPresResultSDO, IsNotShow, this.currentModule);
                    PrintPresProcessor.Print(printTypeCode, false);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InSuatAn(string printTypeCode, string fileName, List<ListMedicineADO> lstSereServSelected, ref bool result)
        {
            try
            {
                if (this.serviceReqPrintRaw != null)
                {
                    WaitingManager.Show();
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(serviceReqPrintRaw.TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);

                    var lstServiceReq = new List<V_HIS_SERVICE_REQ>();
                    lstServiceReq.Add(serviceReqPrintRaw);

                    HisSereServRationFilter rationFilter = new HisSereServRationFilter();
                    rationFilter.SERVICE_REQ_ID = serviceReqPrintRaw.ID;
                    rationFilter.ORDER_DIRECTION = "DESC";
                    rationFilter.ORDER_FIELD = "ID";
                    var paramCommon = new CommonParam();
                    var listRation = new BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_RATION>>("api/HisSereServRation/Get", ApiConsumers.MosConsumer, rationFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                    HisSereServViewFilter ssfilter = new HisSereServViewFilter();
                    ssfilter.SERVICE_REQ_ID = serviceReqPrintRaw.ID;
                    ssfilter.ORDER_DIRECTION = "DESC";
                    ssfilter.ORDER_FIELD = "ID";
                    paramCommon = new CommonParam();
                    var listSereServ = new BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, ssfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                    if (listSereServ != null && listSereServ.Count > 0 && lstSereServSelected != null && lstSereServSelected.Count > 0)
                    {
                        listSereServ = listSereServ.Where(o => lstSereServSelected.Select(s => s.ID).Contains(o.ID)).ToList();
                    }

                    var listSereServExt = new List<HIS_SERE_SERV_EXT>();
                    if (listSereServ != null && listSereServ.Count > 0)
                    {
                        HisSereServExtFilter extFilter = new HisSereServExtFilter();
                        extFilter.SERE_SERV_IDs = listSereServ.Select(s => s.ID).ToList();
                        extFilter.ORDER_DIRECTION = "DESC";
                        extFilter.ORDER_FIELD = "ID";
                        paramCommon = new CommonParam();
                        listSereServExt = new BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, extFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    }

                    MPS.Processor.Mps000275.PDO.Mps000275PDO mps000275PDO = new MPS.Processor.Mps000275.PDO.Mps000275PDO(lstServiceReq, listSereServ, listRation, listSereServExt, BackendDataWorker.Get<HIS_PATIENT_TYPE>());
                    WaitingManager.Hide();

                    PrintData(printTypeCode, fileName, mps000275PDO, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InGiayDeNghiDoiTraDichVu(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (printChangeServiceId > 0)
                {
                    V_HIS_SERVICE_REQ serviceReqChangePrint = GetServiceReqForPrint(printChangeServiceId);
                    WaitingManager.Show();
                    List<HIS_SERE_SERV> dataSereServ = new List<HIS_SERE_SERV>();
                    V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();

                    HisSereServFilter ssfilter = new HisSereServFilter();
                    ssfilter.SERVICE_REQ_ID = serviceReqChangePrint.ID;
                    ssfilter.ORDER_DIRECTION = "DESC";
                    var listSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (listSereServ != null && listSereServ.Count > 0)
                        dataSereServ = listSereServ.Where(o => o.IS_ACCEPTING_NO_EXECUTE == 1).ToList();

                    HisTreatmentViewFilter tmfilter = new HisTreatmentViewFilter();
                    tmfilter.ID = serviceReqChangePrint.TREATMENT_ID;
                    treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, tmfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null).FirstOrDefault();

                    V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                    LoadCurrentPatientTypeAlter(serviceReqChangePrint.TREATMENT_ID, ref patientTypeAlter);

                    MPS.Processor.Mps000433.PDO.Mps000433PDO mps000433PDO = new MPS.Processor.Mps000433.PDO.Mps000433PDO(serviceReqChangePrint, listSereServ, treatment, patientTypeAlter);
                    WaitingManager.Hide();

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.treatmentCode, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000433PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000433PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void PrintServiceReqBySelectedService()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    lstSereServSelected = new List<ListMedicineADO>();
                    this.serviceReqPrintRaw = GetServiceReqForPrint(currentServiceReq.ID);
                    var index = grdViewSereServServiceReq.GetSelectedRows();
                    foreach (var rowHandle in index)
                    {
                        var row = (ListMedicineADO)grdViewSereServServiceReq.GetRow(rowHandle);
                        if (row != null)
                        {
                            lstSereServSelected.Add(row);
                        }
                    }

                    if (lstSereServSelected != null && lstSereServSelected.Count > 0)
                    {
                        if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL
                            && this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)
                        {
                            ThreadChiDinhDichVuADO data = new ThreadChiDinhDichVuADO(this.currentServiceReq);
                            CreateThreadLoadDataForService(data);

                            V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
                            LoadCurrentPatientTypeAlter(this.serviceReqPrintRaw.TREATMENT_ID, ref patientTypeAlter);

                            HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                            HisServiceReqSDO.SereServs = data.listVHisSereServ.Where(o => lstSereServSelected.Select(s => s.ID).Contains(o.ID)).ToList();
                            HisServiceReqSDO.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { this.serviceReqPrintRaw };
                            HisServiceReqSDO.SereServBills = data.ListSereServBill;
                            HisServiceReqSDO.SereServDeposits = data.ListSereServDeposit;

                            List<V_HIS_BED_LOG> listBedLogs = new List<V_HIS_BED_LOG>();
                            HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                            bedLogFilter.TREATMENT_ID = serviceReqPrintRaw.TREATMENT_ID;
                            var resultBedlog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
                            if (resultBedlog != null)
                            {
                                listBedLogs = resultBedlog;
                            }

                            HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, data.hisTreatment);

                            if (patientTypeAlter != null)
                            {
                                HisTreatment.PATIENT_TYPE_CODE = patientTypeAlter.PATIENT_TYPE_CODE;
                                HisTreatment.HEIN_CARD_FROM_TIME = patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                                HisTreatment.HEIN_CARD_NUMBER = patientTypeAlter.HEIN_CARD_NUMBER;
                                HisTreatment.HEIN_CARD_TO_TIME = patientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                                HisTreatment.HEIN_MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                                HisTreatment.LEVEL_CODE = patientTypeAlter.LEVEL_CODE;
                                HisTreatment.RIGHT_ROUTE_CODE = patientTypeAlter.RIGHT_ROUTE_CODE;
                                HisTreatment.RIGHT_ROUTE_TYPE_CODE = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                                HisTreatment.TREATMENT_TYPE_CODE = patientTypeAlter.TREATMENT_TYPE_CODE;
                                HisTreatment.HEIN_CARD_ADDRESS = patientTypeAlter.ADDRESS;
                            }

                            var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, currentModule != null ? currentModule.RoomId : 0);

                            if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH)//Khám
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA)//Siêu âm
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_SIEU_AM__MPS000030, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN)//Xét nghiệm
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_XET_NGHIEM__MPS000026, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS)//Nội soi
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_NOI_SOI__MPS000029, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN)//Thăm dò chức năng
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THAM_DO_CHUC_NANG__MPS000038, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT)//Thủ thuật
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_THU_THUAT__MPS000031, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT)//Phẫu thuật
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHAU_THUAT__MPS000036, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA)//Chẩn đoán hình ảnh
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_CHUAN_DOAN_HINH_ANH__MPS000028, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN)//Phục hồi chức năng
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG__MPS000053, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC)//Khác
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_DICH_VU_KHAC__MPS000040, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G)//Giường
                            {
                                PrintServiceReqProcessor.Print(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_GIUONG__MPS000042, false);
                            }
                            else if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL)//giải phẫu bệnh lý
                            {
                                PrintServiceReqProcessor.Print(MPS000167);
                            }
                        }
                        else
                        {
                            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                            if (this.serviceReqPrintRaw.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN)//Suất ăn
                            {
                                richEditorMain.RunPrintTemplate("Mps000275", DelegateRunPrinterWithSelectedService);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintKetQuaHeThongBenhAnhDienTu()
        {

            try
            {
                WaitingManager.Show();
                if (gridViewServiceReq.FocusedRowHandle >= 0)
                {
                    CommonParam paramEmr = new CommonParam();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    ADO.ServiceReqADO data = (ADO.ServiceReqADO)gridViewServiceReq.GetFocusedRow();
                    if (data == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Data thuc hien huy yeu cau dich vu null: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        return;
                    }

                    List<ADO.ServiceReqADO> lstSerSelected = new List<ADO.ServiceReqADO>();
                    var listData = (List<ADO.ServiceReqADO>)gridControlServiceReq.DataSource;
                    if (listData.Count > 0)
                    {
                        foreach (var item in listData)
                        {
                            if (item.isCheck)
                            {
                                lstSerSelected.Add(item);
                            }
                        }
                    }

                    var check = lstSerSelected.Where(o => o.ID == data.ID);
                    if (lstSerSelected.Count == 0) //lstSerSelected == null && 
                    {
                        lstSerSelected.Add(data);
                    }
                    else
                    {
                        if (check == null && check.Count() == 0)
                        {
                            lstSerSelected.Add(data);
                        }
                    }

                    if (lstSerSelected != null && lstSerSelected.Count > 0)
                    {
                        List<EMR_DOCUMENT> resultEmrDocument = new List<EMR_DOCUMENT>();
                        List<EMR_DOCUMENT> resultEmrDocument_ = new List<EMR_DOCUMENT>();
                        string output = Utils.GenerateTempFileWithin();
                        foreach (var item in lstSerSelected)
                        {
                            EmrDocumentFilter filter = new EmrDocumentFilter();
                            Inventec.Common.Logging.LogSystem.Debug("TDL_TREATMENT_CODE_______________________________________" + data.TDL_TREATMENT_CODE);
                            filter.TREATMENT_CODE__EXACT = item.TDL_TREATMENT_CODE;

                            filter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                            resultEmrDocument_ = new BackendAdapter(paramEmr).Get<List<EMR_DOCUMENT>>("api/EmrDocument/Get", ApiConsumers.EmrConsumer, filter, paramEmr);
                            resultEmrDocument_ = resultEmrDocument_.Where(o => o.IS_DELETE == 0 && o.HIS_CODE != null).ToList();
                            if (resultEmrDocument_ != null && resultEmrDocument_.Count() > 0)
                            {
                                string servicereq = "SERVICE_REQ_CODE:" + item.SERVICE_REQ_CODE;
                                foreach (var item_ in resultEmrDocument_)
                                {
                                    if (item_.HIS_CODE.Contains(servicereq))
                                    {
                                        resultEmrDocument.Add(item_);
                                    }
                                }
                            }
                        }

                        if (resultEmrDocument != null && resultEmrDocument.Count() > 0)
                        {
                            var documents = GetEmrDocumentFile(null, resultEmrDocument.Select(o => o.ID).ToList(), true, true, false);
                            if (documents != null && documents.Count > 0)
                            {
                                List<string> joinStreams = new List<string>();

                                List<MemoryStream> documentData = new List<MemoryStream>();
                                foreach (var item in documents)
                                {
                                    if (item.Extension.ToLower().Equals("pdf"))
                                    {
                                        string pdfAddFile = Utils.GenerateTempFileWithin();
                                        Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                                        joinStreams.Add(pdfAddFile);
                                    }
                                }

                                Stream currentStream = File.Open(output, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                                var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                                var pages = new List<int>();
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                                foreach (var file in joinStreams)
                                {
                                    iTextSharp.text.pdf.PdfReader pdfReader = null;
                                    pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                                    pages = new List<int>();
                                    for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                                    {
                                        pages.Add(i);
                                    }
                                    pdfReader.SelectPages(pages);
                                    pdfConcat.AddPages(pdfReader);
                                    pdfReader.Close();
                                }
                                try
                                {
                                    pdfConcat.Close();
                                }
                                catch { }

                                foreach (var file in joinStreams)
                                {
                                    try
                                    {
                                        File.Delete(file);
                                    }
                                    catch { }
                                }


                                Inventec.Common.Logging.LogSystem.Warn("output: " + output);

                                Inventec.Common.DocumentViewer.Template.frmPdfViewer DocumentView = new Inventec.Common.DocumentViewer.Template.frmPdfViewer(output);

                                DocumentView.Text = "In phiếu kết quả trên hệ thống Bệnh án điện tử";
                                DocumentView.Icon = new Icon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                                DocumentView.WindowState = 0;
                                WaitingManager.Hide();
                                DocumentView.ShowDialog();
                            }
                        }
                        else
                        {
                            if (lstSerSelected != null && lstSerSelected.Count > 0)
                            {
                                WaitingManager.Hide();
                                string tb = "Không tìm thấy phiếu kết quả trên hệ thống Bệnh án điện tử ứng với y lệnh: " + string.Join(", ", lstSerSelected.Select(o => o.SERVICE_REQ_CODE));
                                DevExpress.XtraEditors.XtraMessageBox.Show(tb, "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            }
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

        private List<EmrDocumentFileSDO> GetEmrDocumentFile(V_EMR_DOCUMENT document, List<long> docIds, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark)
        {
            CommonParam paramCommon = new CommonParam();
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            if (document != null)
                emrFilter.ID = document.ID;
            else
                emrFilter.IDs = docIds;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.RoomCode = currentRoom != null ? currentRoom.ROOM_CODE : null;
            sdo.DepartmentCode = currentRoom != null ? currentRoom.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }

        internal static void InsertPage1(Stream sourceStream, List<string> fileListJoin, string desFileJoined)
        {
            List<string> joinStreams = new List<string>();

            if (fileListJoin != null && fileListJoin.Count > 0)
            {
                iTextSharp.text.pdf.PdfReader reader1 = null;

                if (sourceStream != null)
                {
                    reader1 = new iTextSharp.text.pdf.PdfReader(sourceStream);
                }
                int pageCount = reader1.NumberOfPages;
                iTextSharp.text.Rectangle pageSize = reader1.GetPageSizeWithRotation(reader1.NumberOfPages);
                iTextSharp.text.Rectangle pageSize1 = new iTextSharp.text.Rectangle(pageSize.Left, pageSize.Bottom, pageSize.Right, (pageSize.Bottom + pageSize.Height), pageSize.Rotation);

                if (fileListJoin != null && fileListJoin.Count > 0)
                {
                    fileListJoin.Remove(fileListJoin.FirstOrDefault());
                }
                foreach (var item in fileListJoin)
                {

                    int lIndex1 = item.LastIndexOf(".");
                    string EXTENSION = item.Substring(lIndex1 > 0 ? lIndex1 + 1 : lIndex1);
                    if (EXTENSION != "pdf")
                    {
                        var stream = Inventec.Fss.Client.FileDownload.GetFile(item);
                        stream.Position = 0;
                        string convertTpPdf = Utils.GenerateTempFileWithin();

                        Stream streamConvert = new FileStream(convertTpPdf, FileMode.Create, FileAccess.Write);
                        iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                        iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                        iTextdocument.Open();
                        writer.Open();

                        iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(stream);
                        if (img.Height > img.Width)
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Height / img.Height;
                            img.ScalePercent(percentage * 100);
                        }
                        else
                        {
                            float percentage = 0.0f;
                            percentage = pageSize.Width / img.Width;
                            img.ScalePercent(percentage * 100);
                        }
                        iTextdocument.Add(img);
                        iTextdocument.Close();
                        writer.Close();

                        joinStreams.Add(convertTpPdf);
                    }
                    else
                    {

                        //string joinFileResult = Utils.GenerateTempFileWithin();
                        //var streamSource = FssFileDownload.GetFile(item);
                        //streamSource.Position = 0;
                        //Stream streamConvert = new FileStream(joinFileResult, FileMode.Create, FileAccess.Write);
                        //iTextSharp.text.Document iTextdocument = new iTextSharp.text.Document(pageSize1, 0, 0, 0, 0);
                        //iTextSharp.text.pdf.PdfWriter writer = iTextSharp.text.pdf.PdfWriter.GetInstance(iTextdocument, streamConvert);
                        var stream = Inventec.Fss.Client.FileDownload.GetFile(item);

                        if (stream != null && stream.Length > 0)
                        {
                            stream.Position = 0;
                            string pdfAddFile = Utils.GenerateTempFileWithin();
                            Utils.ByteToFile(Utils.StreamToByte(stream), pdfAddFile);
                            joinStreams.Add(pdfAddFile);
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Loi convert va luu tam file pdf tu server fss ve may tram____item=" + item);
                        }
                    }
                }

                Stream currentStream = File.Open(desFileJoined, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                var pages = new List<int>();
                for (int i = 0; i <= reader1.NumberOfPages; i++)
                {
                    pages.Add(i);
                }
                reader1.SelectPages(pages);
                pdfConcat.AddPages(reader1);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                foreach (var file in joinStreams)
                {
                    iTextSharp.text.pdf.PdfReader pdfReader = null;
                    pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                    pages = new List<int>();
                    for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                    {
                        pages.Add(i);
                    }
                    pdfReader.SelectPages(pages);
                    pdfConcat.AddPages(pdfReader);
                    pdfReader.Close();
                }

                try
                {
                    reader1.Close();
                }
                catch { }

                try
                {
                    if (sourceStream != null)
                        sourceStream.Close();
                }
                catch { }

                try
                {
                    pdfConcat.Close();
                }
                catch { }

                foreach (var file in joinStreams)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }
        }

        private bool DelegateRunPrinterWithSelectedService(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000275":
                        InSuatAn(printTypeCode, fileName, lstSereServSelected, ref result);
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
        #endregion

        #region In kết quả
        #region in phẫu thuật thủ thuật
        private void LoadBieuMauPhieuThuThuatPhauThuat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.sereServPrint != null)
                {
                    WaitingManager.Show();

                    ThreadPtttADO data = new ThreadPtttADO(this.sereServPrint);
                    CreateThreadLoadDataForPttt(data);

                    MPS.Processor.Mps000033.PDO.PatientADO currentPatient = new MPS.Processor.Mps000033.PDO.PatientADO(data.patient);

                    HIS_SERE_SERV_EXT SereServExt = null;
                    HisSereServExtFilter ssExtFilter = new HisSereServExtFilter();
                    ssExtFilter.SERE_SERV_ID = sereServPrint.ID;
                    var SereServExts = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer, ssExtFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (SereServExts != null && SereServExts.Count > 0)
                    {
                        SereServExt = SereServExts.FirstOrDefault();
                    }

                    V_HIS_BED_LOG currentBedLog = new V_HIS_BED_LOG();
                    V_HIS_BED_LOG lastBedLog = new V_HIS_BED_LOG();
                    if (SereServExt != null)
                    {
                        HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                        bedLogFilter.TREATMENT_ID = data.vhisTreatment.ID;
                        var bedLogs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/Get", ApiConsumers.MosConsumer, bedLogFilter, param);
                        if (bedLogs != null & bedLogs.Count > 0)
                        {
                            currentBedLog = bedLogs.Where(o => o.START_TIME <= SereServExt.BEGIN_TIME
                                && (!o.FINISH_TIME.HasValue || o.FINISH_TIME.Value > SereServExt.BEGIN_TIME))
                                .OrderByDescending(o => o.START_TIME).FirstOrDefault();
                            lastBedLog = bedLogs.Where(o => o.START_TIME <= SereServExt.BEGIN_TIME)
                                .OrderByDescending(o => o.START_TIME).FirstOrDefault();
                        }
                    }

                    HIS_SKIN_SURGERY_DESC skinDesc = null;
                    if (data.sereServPttts != null && data.sereServPttts.SKIN_SURGERY_DESC_ID.HasValue)
                    {
                        HisSkinSurgeryDescFilter skinFilter = new HisSkinSurgeryDescFilter();
                        skinFilter.ID = data.sereServPttts.SKIN_SURGERY_DESC_ID;
                        var skins = new BackendAdapter(param).Get<List<HIS_SKIN_SURGERY_DESC>>("api/HisSkinSurgeryDesc/Get", ApiConsumers.MosConsumer, skinFilter, param);
                        if (skins != null && skins.Count > 0)
                        {
                            skinDesc = skins.FirstOrDefault();
                        }
                    }
                    List<V_HIS_SESE_PTTT_METHOD> sesePtttMethod = null;
                    if (data.sereServPrint != null)
                    {
                        HisSesePtttMethodViewFilter filter = new HisSesePtttMethodViewFilter();
                        filter.TDL_SERE_SERV_ID = data.sereServPrint.ID;
                        sesePtttMethod = new BackendAdapter(param).Get<List<V_HIS_SESE_PTTT_METHOD>>("api/HisSesePtttMethod/GetView", ApiConsumers.MosConsumer, filter, param);
                    }
                    MPS.Processor.Mps000033.PDO.HisExecuteRoleCFGPrint hisExecuteRoleCFGPrint = new MPS.Processor.Mps000033.PDO.HisExecuteRoleCFGPrint();

                    WaitingManager.Hide();
                    MPS.Processor.Mps000033.PDO.Mps000033PDO rdo = new MPS.Processor.Mps000033.PDO.Mps000033PDO(currentPatient, data.departmentTran, data.serviceReq, data.sereServ5Print, SereServExt, data.sereServPttts, data.vhisTreatment, data.ekipUsers, hisExecuteRoleCFGPrint, currentBedLog, lastBedLog, null, skinDesc, data.sereServFile, sesePtttMethod);

                    PrintData(printTypeCode, fileName, rdo, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadBieuMauPhieuYCInGiayCamDoan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.sereServPrint != null)
                {
                    WaitingManager.Show();
                    ThreadPtttADO data = new ThreadPtttADO(this.sereServPrint);
                    CreateThreadLoadDataForPttt(data);

                    MPS.Processor.Mps000035.PDO.Mps000035PDO rdo = new MPS.Processor.Mps000035.PDO.Mps000035PDO(data.patient, data.departmentTran, data.serviceReq, data.vhisTreatment);

                    WaitingManager.Hide();

                    PrintData(printTypeCode, fileName, rdo, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadBieuMauCachThucPhauThuat(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.sereServPrint != null)
                {
                    WaitingManager.Show();

                    ThreadPtttADO data = new ThreadPtttADO(this.sereServPrint);
                    CreateThreadLoadDataForPttt(data);

                    MPS.Processor.Mps000097.PDO.Mps000097PDO rdo = new MPS.Processor.Mps000097.PDO.Mps000097PDO(data.patient, data.sereServPttts, data.ekipUsers, data.vhisTreatment);

                    WaitingManager.Hide();

                    PrintData(printTypeCode, fileName, rdo, ref result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadGiayChungNhanPTTT(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.sereServPrint != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_1 sereServ1 = new V_HIS_SERE_SERV_1();
                    MOS.Filter.HisSereServView1Filter sereServFilter = new MOS.Filter.HisSereServView1Filter();
                    sereServFilter.TREATMENT_ID = this.sereServPrint.ID;

                    var sereServData = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>("/api/HisSereServ/GetView1", ApiConsumers.MosConsumer, sereServFilter, param);
                    if (sereServData != null && sereServData.Count > 0)
                    {
                        sereServ1 = sereServData.FirstOrDefault();
                    }

                    MOS.EFMODEL.DataModels.V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                    MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                    hisTreatmentFilter.ID = this.sereServPrint.TDL_TREATMENT_ID;
                    var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                    if (treatments != null && treatments.Count > 0)
                    {
                        treatment = treatments.FirstOrDefault();
                    }

                    MOS.Filter.HisSereServPtttViewFilter hisSereServPtttFilter = new MOS.Filter.HisSereServPtttViewFilter();
                    hisSereServPtttFilter.SERE_SERV_ID = this.sereServPrint.ID;

                    var hisSereServPttt = new BackendAdapter(param)
                     .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param).FirstOrDefault();

                    MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt = new HIS_SERE_SERV_EXT();
                    MOS.Filter.HisSereServExtFilter hisSereServExtFilter = new HisSereServExtFilter();
                    hisSereServExtFilter.SERE_SERV_ID = this.sereServPrint.ID;
                    var sereServExts = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServExtFilter, null);
                    if (sereServExts != null && sereServExts.Count > 0)
                    {
                        sereServExt = sereServExts.FirstOrDefault();
                    }

                    List<V_HIS_EKIP_USER> listEkipUser = new List<V_HIS_EKIP_USER>();
                    if (this.sereServPrint.EKIP_ID.HasValue)
                    {
                        MOS.Filter.HisEkipUserViewFilter hisEkipUserFilter = new MOS.Filter.HisEkipUserViewFilter();
                        hisEkipUserFilter.EKIP_ID = this.sereServPrint.EKIP_ID;
                        hisEkipUserFilter.ORDER_FIELD = "EXECUTE_ROLE_ID";
                        hisEkipUserFilter.ORDER_DIRECTION = "ASC";
                        listEkipUser = new BackendAdapter(param)
                .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);
                    }

                    MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                    serviceReqFilter.ID = this.sereServPrint.SERVICE_REQ_ID;

                    var currentServiceReq = new BackendAdapter(param)
             .Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                    List<HIS_EXECUTE_ROLE> HisExecuteRoles = new List<HIS_EXECUTE_ROLE>();
                    HisExecuteRoles = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                    MPS.Processor.Mps000204.PDO.Mps000204PDO rdo = new MPS.Processor.Mps000204.PDO.Mps000204PDO(sereServ1, treatment, hisSereServPttt, listEkipUser, currentServiceReq, HisExecuteRoles);

                    WaitingManager.Hide();

                    PrintData(printTypeCode, fileName, rdo, ref result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void InPhieuKetQuaPHCN(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (sereServPrint != null)
                {
                    WaitingManager.Show();
                    var patient = PrintGlobalStore.GetPatientById(sereServPrint.TDL_PATIENT_ID ?? 0);

                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == sereServPrint.TDL_EXECUTE_ROOM_ID);
                    string bedRoom = room != null ? room.ROOM_NAME : "";

                    //Lấy thông tin chuyển khoa
                    var departmentTran = PrintGlobalStore.getDepartmentTran(sereServPrint.TDL_TREATMENT_ID ?? 0);
                    List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran = new List<V_HIS_DEPARTMENT_TRAN>();
                    lstDepartmentTran.Add(departmentTran);

                    HIS_REHA_SUM hisRehaSum = new HIS_REHA_SUM();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_REHA_SUM>(hisRehaSum, currentServiceReq);

                    HisSereServRehaFilter sereServRehaFiler = new HisSereServRehaFilter();
                    sereServRehaFiler.SERE_SERV_ID = sereServPrint.ID;
                    var sereServRehas = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_REHA>>(RequestUriStore.HIS_SERE_SERV_REHA_GET, ApiConsumers.MosConsumer, sereServRehaFiler, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    HisRehaTrainViewFilter rehaTrainFilter = new HisRehaTrainViewFilter();
                    rehaTrainFilter.SERE_SERV_REHA_IDs = sereServRehas != null ? sereServRehas.Select(o => o.ID).ToList() : new List<long>();
                    List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN> lstSereServRehas = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN>>(HisRequestUriStore.HIS_REHA_TRAIN_GETVIEW, ApiConsumers.MosConsumer, rehaTrainFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    List<MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO> hisSereServRehaADOs = new List<MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO>();
                    if (lstSereServRehas != null && lstSereServRehas.Count > 0)
                    {
                        var sereServRehaGroupBy = lstSereServRehas.Select(x => new { SereServReha = x, SERVICE_ID = x.REHA_TRAIN_TYPE_ID, SERVICE_CODE = x.REHA_TRAIN_TYPE_CODE, SERVICE_NAME = x.REHA_TRAIN_TYPE_NAME, REHA_TRAIN_UNIT_CODE = x.REHA_TRAIN_UNIT_CODE, REHA_TRAIN_UNIT_NAME = x.REHA_TRAIN_UNIT_NAME }).GroupBy(s => (s.SERVICE_ID), (g, exps) => new { SERVICE_ID = g, SereServRehas = exps, AMOUNT_REHA_SUM = exps.Sum(o => o.SereServReha.AMOUNT) });

                        if (sereServRehaGroupBy != null)
                        {
                            foreach (var itemGroup in sereServRehaGroupBy)
                            {
                                MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO sereServRehaSDO = new MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO();

                                sereServRehaSDO.AMOUNT = itemGroup.AMOUNT_REHA_SUM;
                                var dataReha = itemGroup.SereServRehas.FirstOrDefault(o => o.SERVICE_ID == itemGroup.SERVICE_ID);
                                if (dataReha != null)
                                {
                                    sereServRehaSDO.REHA_TRAIN_TYPE_CODE = dataReha.SERVICE_CODE;
                                    sereServRehaSDO.REHA_TRAIN_TYPE_NAME = dataReha.SERVICE_NAME;
                                    sereServRehaSDO.REHA_TRAIN_UNIT_CODE = dataReha.REHA_TRAIN_UNIT_CODE;
                                    sereServRehaSDO.REHA_TRAIN_UNIT_NAME = dataReha.REHA_TRAIN_UNIT_NAME;
                                }

                                hisSereServRehaADOs.Add(sereServRehaSDO);
                            }
                        }
                    }

                    MPS.Processor.Mps000063.PDO.Mps000063PDO rdo = new MPS.Processor.Mps000063.PDO.Mps000063PDO(patient, lstDepartmentTran, hisRehaSum, hisSereServRehaADOs, bedRoom, serviceReqPrintRaw);

                    WaitingManager.Hide();

                    PrintData(printTypeCode, fileName, rdo, ref result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region in thuoc
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

                List<V_HIS_TREATMENT_BED_ROOM> listTreatmentBedRoom = null;
                if (treatment != null)
                {
                    HisTreatmentBedRoomViewFilter treatmentBedRoomFilter = new HisTreatmentBedRoomViewFilter();
                    treatmentBedRoomFilter.TREATMENT_ID = treatment.ID;
                    treatmentBedRoomFilter.IS_IN_ROOM = true;
                    listTreatmentBedRoom = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentBedRoomFilter, paramCommon);
                }
                List<V_HIS_SERE_SERV_1> listSereServ1 = null;
                if (examServiceReq != null)
                {
                    HisSereServView1Filter sereServ1Filter = new HisSereServView1Filter();
                    sereServ1Filter.SERVICE_REQ_PARENT_ID = examServiceReq.ID;
                    listSereServ1 = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV_1>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_1, ApiConsumers.MosConsumer, sereServ1Filter, paramCommon);
                }

                List<V_HIS_EXP_MEST_BLTY_REQ_1> expMestMeties = null;
                List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods_Print = null;
                if (expMest != null)
                {
                    MOS.Filter.HisExpMestBltyReqView1Filter expMestMetyFilter = new MOS.Filter.HisExpMestBltyReqView1Filter();
                    expMestMetyFilter.EXP_MEST_ID = expMest.ID;
                    expMestMeties = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_EXP_MEST_BLTY_REQ_1>>(RequestUriStore.HIS_EXP_MEST_BLTY_REQ_GETVIEW_1, ApiConsumers.MosConsumer, expMestMetyFilter, paramCommon);

                    MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                    bloodFilter.EXP_MEST_ID = expMest.ID;
                    _ExpMestBloods_Print = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                }

                string treatmentCode = (treatment != null ? treatment.TREATMENT_CODE : "");

                MPS.Processor.Mps000108.PDO.Mps000108PDO mps000108RDO = new MPS.Processor.Mps000108.PDO.Mps000108PDO(
                    expMest,
                    expMestMeties,
                    treatment,
                    examServiceReq,
                    _ExpMestBloods_Print,
                    listTreatmentBedRoom,
                    listSereServ1
               );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplicationWorker.Get<string>(Base.AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__IS_PRINT_NOW) == "1")
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

        private void InPhieuYeuCauDonThuocYHocCoTruyen()
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
                        currentServiceReqPrint = new ServiceReqADO();
                    }

                    MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                    sdo.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>() { prescriptionPrint };
                    sdo.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>() { currentServiceReqPrint };

                    Library.PrintPrescription.PrintPrescriptionProcessor processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModule);

                    processPress.Print(MPS.Processor.Mps000050.PDO.Mps000050PDO.PrintTypeCode, false);

                    if (isNull)
                    {
                        prescriptionPrint = null;
                        currentServiceReqPrint = null;
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                        currentServiceReqPrint = new ServiceReqADO();
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

        private void InPhieuYeuCauThuocVatTuTongHop()
        {
            try
            {
                //Thong tin thuoc / vat tu
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
                        currentServiceReqPrint = new ServiceReqADO();
                    }

                    MOS.SDO.OutPatientPresResultSDO sdo = new MOS.SDO.OutPatientPresResultSDO();
                    sdo.ExpMests = new List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>() { prescriptionPrint };
                    sdo.ServiceReqs = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>() { currentServiceReqPrint };
                    var processPress = new Library.PrintPrescription.PrintPrescriptionProcessor(new List<MOS.SDO.OutPatientPresResultSDO>() { sdo }, this.currentModule);

                    processPress.Print(MPS.Processor.Mps000118.PDO.Mps000118PDO.PrintTypeCode, false);

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
        #endregion
        #endregion

        #region thread
        #region dịch vụ/ thông tin chung
        private void CreateThreadLoadDataForService(ThreadChiDinhDichVuADO data)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataTreatment));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadDataSereServ));
            Thread threadSereServBill = new Thread(new ParameterizedThreadStart(LoadDataSereServBill));
            Thread threadSereServDeposit = new Thread(new ParameterizedThreadStart(LoadDataSereServDeposit));

            try
            {
                threadTreatment.Start(data);
                threadSereServ.Start(data);
                threadSereServBill.Start(data);
                threadSereServDeposit.Start(data);

                threadTreatment.Join();
                threadSereServ.Join();
                threadSereServBill.Join();
                threadSereServDeposit.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                threadSereServBill.Abort();
                threadSereServDeposit.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServDeposit(object data)
        {
            try
            {
                LoadThreadDataSereServDeposit((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServDeposit(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    List<HIS_SERE_SERV_DEPOSIT> ssDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                    List<HIS_SESE_DEPO_REPAY> ssRepay = new List<HIS_SESE_DEPO_REPAY>();

                    CommonParam paramCommon = new CommonParam();
                    HisSereServDepositFilter ssDepositFilter = new HisSereServDepositFilter();
                    ssDepositFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssDepositFilter.IS_CANCEL = false;
                    var apiDepositResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, ssDepositFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiDepositResult != null && apiDepositResult.Count > 0)
                    {
                        ssDeposit = apiDepositResult;
                    }

                    HisSeseDepoRepayFilter ssRepayFilter = new HisSeseDepoRepayFilter();
                    ssRepayFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssRepayFilter.IS_CANCEL = false;
                    var apiRepayResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumers.MosConsumer, ssRepayFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiRepayResult != null && apiRepayResult.Count > 0)
                    {
                        ssRepay = apiRepayResult;
                    }

                    data.ListSereServDeposit = ssDeposit.Where(o => !ssRepay.Exists(e => e.SERE_SERV_DEPOSIT_ID == o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServBill(object data)
        {
            try
            {
                LoadThreadDataSereServBill((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServBill(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    CommonParam paramCommon = new CommonParam();
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = data.vHisServiceReq2Print.TREATMENT_ID;
                    ssBillFilter.IS_NOT_CANCEL = true;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        data.ListSereServBill = apiResult;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatment(object data)
        {
            try
            {
                LoadThreadDataTreatment((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatment(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    data.hisTreatment = getTreatment(data.vHisServiceReq2Print.TREATMENT_ID);

                    data.vHisPatientTypeAlter = getPatientTypeAlter(data.vHisServiceReq2Print.TREATMENT_ID, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataSereServ(object data)
        {
            try
            {
                LoadThreadDataSereServ((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataSereServ(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    data.listVHisSereServ = GetSereServByServiceReqId(data.vHisServiceReq2Print.ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ChiDinhTongHop
        private void CreateThreadLoadDataForChiDinhTongHop(ThreadChiDinhDichVuADO data)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataServiceReq));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadListDataSereServ));
            threadTreatment.Priority = ThreadPriority.Normal;


            try
            {
                threadTreatment.Start(data);
                threadSereServ.Start(data);

                threadTreatment.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataServiceReq(object data)
        {
            try
            {
                LoadThreadDataServiceReq((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataServiceReq(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null)
                {
                    data.hisTreatment = getTreatment(data.vHisServiceReq2Print.TREATMENT_ID);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListDataSereServ(object data)
        {
            try
            {
                LoadThreadListDataSereServ((ThreadChiDinhDichVuADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadListDataSereServ(ThreadChiDinhDichVuADO data)
        {
            try
            {
                if (data != null && data.vHisServiceReq2Print != null && this.listServiceReq != null && this.listServiceReq.Count > 0)
                {
                    data.vHisPatientTypeAlter = getPatientTypeAlter(data.vHisServiceReq2Print.TREATMENT_ID, 0);

                    data.listVHisSereServ = new List<V_HIS_SERE_SERV>();
                    foreach (var item in this.listServiceReq)
                    {
                        data.listVHisSereServ.AddRange(GetSereServByServiceReqId(item.ID));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region PTTT
        private void CreateThreadLoadDataForPttt(ThreadPtttADO data)
        {
            Thread threadTreatment = new Thread(new ParameterizedThreadStart(LoadDataTreatmentPttt));
            Thread threadSereServ = new Thread(new ParameterizedThreadStart(LoadListDataSereServPttt));
            threadTreatment.Priority = ThreadPriority.Normal;

            try
            {
                threadTreatment.Start(data);
                threadSereServ.Start(data);

                threadTreatment.Join();
                threadSereServ.Join();
            }
            catch (Exception ex)
            {
                threadTreatment.Abort();
                threadSereServ.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataTreatmentPttt(object data)
        {
            try
            {
                LoadThreadDataTreatmentPttt((ThreadPtttADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadDataTreatmentPttt(ThreadPtttADO data)
        {
            try
            {
                if (data != null && data.sereServPrint != null)
                {
                    data.patient = PrintGlobalStore.GetPatientById(data.sereServPrint.TDL_PATIENT_ID ?? 0);
                    data.vhisTreatment = PrintGlobalStore.getTreatment(data.sereServPrint.TDL_TREATMENT_ID ?? 0);
                    data.departmentTran = PrintGlobalStore.getDepartmentTran(data.sereServPrint.TDL_TREATMENT_ID ?? 0);

                    HisSereServView5Filter view5Filter = new HisSereServView5Filter();
                    view5Filter.ID = data.sereServPrint.ID;
                    data.sereServ5Print = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_5, ApiConsumers.MosConsumer, view5Filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadListDataSereServPttt(object data)
        {
            try
            {
                LoadThreadListDataSereServPttt((ThreadPtttADO)data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadThreadListDataSereServPttt(ThreadPtttADO data)
        {
            try
            {
                if (data != null && data.sereServPrint != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("Begin get serviceReq, sereServPttt, ekipUser");
                    CommonParam param = new CommonParam();
                    HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                    serviceReqFilter.ID = data.sereServPrint.SERVICE_REQ_ID;
                    var serviceReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (serviceReq != null && serviceReq.Count == 1)
                    {
                        data.serviceReq = serviceReq.First();
                    }

                    HisSereServPtttViewFilter sereServPtttfilter = new HisSereServPtttViewFilter();
                    sereServPtttfilter.SERE_SERV_ID = data.sereServPrint.ID;
                    var sereServPttts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, sereServPtttfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (sereServPttts != null && sereServPttts.Count > 0)
                    {
                        data.sereServPttts = sereServPttts.FirstOrDefault();
                    }

                    if (data.sereServPrint.EKIP_ID.HasValue)
                    {
                        HisEkipUserViewFilter ekipUserFilter = new HisEkipUserViewFilter();
                        ekipUserFilter.EKIP_ID = data.sereServPrint.EKIP_ID;
                        data.ekipUsers = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, ekipUserFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    }
                    else
                    {
                        data.ekipUsers = new List<V_HIS_EKIP_USER>();
                    }

                    HisSereServFileFilter ssffilter = new MOS.Filter.HisSereServFileFilter();
                    ssffilter.SERE_SERV_ID = data.sereServPrint.ID;
                    data.sereServFile = new BackendAdapter(param).Get<List<HIS_SERE_SERV_FILE>>("api/HisSereServFile/Get", ApiConsumers.MosConsumer, ssffilter, param);
                    Inventec.Common.Logging.LogSystem.Info("End get serviceReq, sereServPttt, ekipUser");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion
    }
}

