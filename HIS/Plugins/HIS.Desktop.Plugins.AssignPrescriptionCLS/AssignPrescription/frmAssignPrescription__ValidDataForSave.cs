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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        //private bool Valid(List<MediMatyTypeADO> serviceCheckeds__Send)
        //{
        //    CommonParam param = new CommonParam();
        //    bool valid = true;
        //    try
        //    {
        //        this.positionHandleControl = -1;
        //        valid = (this.dxValidationProviderControl.Validate());
        //        valid = valid && this.CheckValidDataInGridService(param, serviceCheckeds__Send);
        //        valid = valid && this.CheckValidHeinServicePrice(param, serviceCheckeds__Send);

        //        if (!valid && param.Messages != null && param.Messages.Count > 0)
        //        {
        //            MessageManager.Show(param, null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        valid = false;
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }

        //    return valid;
        //}

        //private bool CheckValidDataInGridService(CommonParam param, List<MediMatyTypeADO> serviceCheckeds__Send)
        //{
        //    bool valid = true;
        //    try
        //    {
        //        if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
        //        {
        //            foreach (var item in serviceCheckeds__Send)
        //            {
        //                string messageErr = "";
        //                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
        //                {
        //                    messageErr = String.Format(ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
        //                }
        //                else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
        //                {
        //                    messageErr = String.Format(ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
        //                }

        //                if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && item.PATIENT_TYPE_ID <= 0)
        //                {
        //                    valid = false;
        //                    messageErr += ResourceMessage.KhongCoDoiTuongThanhToan;
        //                    Inventec.Common.Logging.LogSystem.Warn("Thuoc/Vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " khong co doi tuong thanh toan.");
        //                }
        //                if (item.AMOUNT <= 0)
        //                {
        //                    valid = false;
        //                    messageErr += ResourceMessage.KhongNhapSoLuong;
        //                    Inventec.Common.Logging.LogSystem.Warn("Thuoc/vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " khong co so luong.");
        //                }
        //                if ((item.AmountAlert ?? 0) > 0)
        //                {
        //                    valid = false;
        //                    messageErr += (" " + ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho);
        //                    Inventec.Common.Logging.LogSystem.Warn("Thuoc/vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " co so luong lon hon so luong kha dung trong kho.");
        //                }
        //                if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
        //                {
        //                    if (item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.MEDICINE_USE_FORM_ID ?? 0) == 0)
        //                    {
        //                        messageErr += ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
        //                        valid = false;
        //                        Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dung cua thuoc.");
        //                    }
        //                    if (String.IsNullOrEmpty(item.TUTORIAL.Trim()))
        //                    {
        //                        messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.DoiTuongBHYTBatBuocPhaiNhapHDSD, System.Drawing.Color.Maroon));
        //                        valid = false;
        //                        Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dan su dung cua thuoc.");
        //                    }
        //                }

        //                if (!String.IsNullOrEmpty(item.TUTORIAL.Trim()) && Encoding.UTF8.GetByteCount(item.TUTORIAL) > 1000)
        //                {
        //                    messageErr += (" " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(ResourceMessage.HDSDVuotQuaKyTu, System.Drawing.Color.Maroon));
        //                    valid = false;
        //                }
        //                if (!valid)
        //                {
        //                    param.Messages.Add(messageErr + ";");
        //                }
        //            }

        //            var existsMameType = serviceCheckeds__Send.Any(o => o.MAME_ID == null && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
        //            var existsMame = serviceCheckeds__Send.Any(o => (o.MAME_ID ?? 0) > 0 && (o.MEDI_STOCK_ID ?? 0) > 0 && (o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT));
        //            if (existsMameType && existsMame)
        //            {
        //                param.Messages.Add(ResourceMessage.Trong1LanKeDonChiDuocKeTheoLoHoacTheoLoai);
        //                valid = false;
        //            }
        //        }
        //        else
        //        {
        //            Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetParam(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return valid;
        //}

        ///// <summary>
        ///// Hàm kiểm tra số tiền đã kê có vượt quá cấu hình trần bảo hiểm không
        ///// Trường hợp trần được cấu hình trong hồ sơ điều trị thì lấy từ hsdt
        ///// Trường hợp hsdt không cấu hình trần thì lấy trần từ cấu hình ccc ra
        ///// Chỉ check trần số tiền bhyt khi có cấu hình trần
        ///// </summary>
        ///// <param name="param"></param>
        ///// <param name="serviceCheckeds__Send"></param>
        ///// <returns></returns>
        //private bool CheckValidHeinServicePrice(CommonParam param, List<MediMatyTypeADO> serviceCheckeds__Send)
        //{
        //    bool valid = true;
        //    decimal tongtienThuocPhatSinh = 0;
        //    string messageErr = "";
        //    try
        //    {
        //        if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
        //        {
        //            decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
        //            decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

        //            //Kiểm tra, nếu có key cấu hình "check giới hạn thuốc", thì khi người dùng nhấn nút "Lưu" mới lấy thông tin hồ sơ điều trị để check
        //            if (limitHeinMedicinePrice__RightMediOrg > 0
        //                || limitHeinMedicinePrice__NotRightMediOrg > 0)
        //            {
        //                //Load lại thông tin hồ sơ điều trị
        //                this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());

        //                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType.TREATMENT_CODE), this.currentTreatmentWithPatientType.TREATMENT_CODE) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP), this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE), this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE));

        //                //Nếu hồ sơ điều trị cấu hình trường IS_NOT_CHECK_LHMP = 1 thì bỏ qua không check, return true
        //                //Hoặc đối tượng điều tị là điều trị nội/ngoại trú thì bỏ qua không check
        //                if ((this.currentTreatmentWithPatientType != null
        //                && this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP.HasValue
        //                && (this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP ?? 0) == GlobalVariables.CommonNumberTrue)
        //                || this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatIn
        //                || this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatOut
        //                    )
        //                {
        //                    return true;
        //                }

        //                this.limitHeinMedicinePrice = this.IsLimitHeinMedicinePrice(treatmentId);

        //                var bhyt__Exists = serviceCheckeds__Send
        //                    .Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
        //                        && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
        //                        && !o.IsExpend).ToList();
        //                //Kiểm tra tiền bhyt đã kê vượt mức giới hạn chưa
        //                if (limitHeinMedicinePrice == true)
        //                {
        //                    valid = false;
        //                }
        //                else if (bhyt__Exists != null
        //                    && bhyt__Exists.Count > 0
        //                    && this.currentHisPatientTypeAlter != null
        //                    && (limitHeinMedicinePrice__RightMediOrg > 0 || limitHeinMedicinePrice__NotRightMediOrg > 0)
        //                    )
        //                {
        //                    foreach (var item in bhyt__Exists)
        //                    {
        //                        tongtienThuocPhatSinh += (item.TotalPrice);
        //                    }

        //                    //Đối với bệnh nhân đúng tuyến KCB
        //                    if (limitHeinMedicinePrice__RightMediOrg > 0
        //                        && this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE == HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT)
        //                    {
        //                        if (tongtienThuocPhatSinh + totalPriceBHYT > limitHeinMedicinePrice__RightMediOrg)
        //                        {
        //                            messageErr = String.Format(ResourceMessage.SoTienDaKeChoBHYTDaVuotquaMucGioiHan, Inventec.Common.Number.Convert.NumberToString(tongtienThuocPhatSinh + totalPriceBHYT, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToString(HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg));
        //                            valid = false;
        //                        }
        //                    }

        //                    //Đối với bệnh nhân chuyển tuyến
        //                    if (limitHeinMedicinePrice__NotRightMediOrg > 0
        //                        && this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
        //                    {
        //                        if (tongtienThuocPhatSinh + totalPriceBHYT > limitHeinMedicinePrice__NotRightMediOrg)
        //                        {
        //                            messageErr = String.Format(ResourceMessage.SoTienDaKeChoBHYTDaVuotquaMucGioiHan, Inventec.Common.Number.Convert.NumberToString(tongtienThuocPhatSinh + totalPriceBHYT), Inventec.Common.Number.Convert.NumberToString(HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg, ConfigApplications.NumberSeperator));
        //                            valid = false;
        //                        }
        //                    }
        //                }

        //                if (!valid)
        //                {
        //                    param.Messages.Add(messageErr);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        valid = false;
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //    return valid;
        //}

        /// <summary>
        /// HIS.HIS_ICD_SERVICE.HAS_CHECK. Giá trị:
        ///- 0 (hoặc ko khai báo): Không thay đổi gì, giữ nguyên nghiệp vụ như hiện tại
        ///- 1: Có kiểm tra thuốc/vật tư đã kê có nằm trong danh sách đã được cấu hình (dữ liệu trong HIS_ICD_SERVICE) tương ứng với ICD (căn cứ theo ICD_CODE và ICD_SUB_CODE)của bệnh nhân hay không. Nếu tồn tại dịch vụ không được cấu hình thì hiển thị thông báo và không cho lưu.
        ///- 2: Có kiểm tra, nhưng chỉ hiển thị cảnh báo, và hỏi "Bạn có muốn tiếp tục không". Nếu người dùng chọn "OK" thì vẫn cho phép lưu
        /// </summary>
        /// <returns></returns>
        private bool CheckICDService()
        {
            bool result = true;
            try
            {
                string icdCode = "";
                
                var icdValue = UcIcdGetValue() as IcdInputADO;
                if (icdValue != null)
                {
                    icdCode = icdValue.ICD_CODE;
                }

                //var subIcd = UcIcdCauseGetValue() as SecondaryIcdDataADO;
                //if (subIcd != null)
                //{
                //    icdCode += ";" + subIcd.ICD_SUB_CODE;
                //}

                if (!String.IsNullOrEmpty(icdCode))
                {
                    string[] icdCodeArr = icdCode.Split(';');

                    if (HisConfigCFG.icdServiceHasCheck != 0)
                    {
                        var serviceADOs = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).ToList();
                        List<long> serviceNotConfigIds = GetServiceIdNotIcdService(icdCodeArr.Distinct().ToList(), serviceADOs);

                        if (serviceNotConfigIds != null && serviceNotConfigIds.Count > 0)
                        {
                            List<MediMatyTypeADO> serviceNotConfigs = this.mediMatyTypeADOs.Where(o => serviceNotConfigIds.Contains(o.SERVICE_ID)).ToList();
                            string medicineTypeNameStr = "";
                            foreach (var item in serviceNotConfigs)
                            {
                                medicineTypeNameStr += item.MEDICINE_TYPE_NAME + ";";
                            }

                            if (HisConfigCFG.icdServiceHasCheck == 1)
                            {
                                MessageBox.Show(String.Format(ResourceMessage.DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICD, medicineTypeNameStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                            else if (HisConfigCFG.icdServiceHasCheck == 2)
                            {
                                DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICDBanCoMuonTiepTuc, medicineTypeNameStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (myResult != DialogResult.Yes)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    else
                    {
                        // Lấy danh sách thuốc được cấu hình giới hạn
                        string medicineCodeLimit = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignPrescription.MedicineLimitAssign");
                        if (!String.IsNullOrEmpty(medicineCodeLimit))
                        {
                            string[] medicineCodeLimitArr = medicineCodeLimit.Split(',');
                            if (medicineCodeLimitArr != null && medicineCodeLimitArr.Length > 0)
                            {
                                List<long> serviceIdCFGs = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
                                    .Where(o => medicineCodeLimitArr.Contains(o.SERVICE_CODE)).Select(p => p.ID).ToList();
                                var serviceADOs = this.mediMatyTypeADOs.Where(o => serviceIdCFGs.Contains(o.SERVICE_ID)).ToList();

                                if (serviceADOs != null && serviceADOs.Count > 0)
                                {
                                    List<long> serviceNotConfigIds = GetServiceIdNotIcdService(icdCodeArr.ToList(), serviceADOs);

                                    if (serviceNotConfigIds != null && serviceNotConfigIds.Count > 0)
                                    {
                                        List<MediMatyTypeADO> serviceNotConfigs = this.mediMatyTypeADOs.Where(o => serviceNotConfigIds.Contains(o.SERVICE_ID)).ToList();
                                        string medicineTypeNameStr = "";
                                        foreach (var item in serviceNotConfigs)
                                        {
                                            medicineTypeNameStr += item.MEDICINE_TYPE_NAME + ";";
                                        }

                                        DialogResult myResult = MessageBox.Show(String.Format(ResourceMessage.DichVuTrongCauHinhThuocCHuaDuocCauHinhChanDoanICDBanCoMuonTiepTuc, medicineTypeNameStr), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                        if (myResult != DialogResult.Yes)
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Thuốc cho phép trong "phác đồ" là thuốc thỏa mãn 1 trong 2 điều kiện: loại thuốc (service_id) hoặc hoạt chất (active_ingradient_id) được khai báo tương ứng với ICD
        /// </summary>
        /// <param name="icdCodes"></param>
        /// <param name="serviceIds"></param>
        /// <returns></returns>
        private List<long> GetServiceIdNotIcdService(List<string> icdCodes, List<MediMatyTypeADO> mediADOs)
        {
            List<long> serviceIdNotIcdServices = new List<long>();
            try
            {
                List<long> serviceIds = mediADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).Distinct().ToList();

                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.ICD_CODE__EXACTs = icdCodes;
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.icdServiceHasRequireCheck), HisConfigCFG.icdServiceHasRequireCheck) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServices), icdServices));
                if (HisConfigCFG.icdServiceHasRequireCheck || (!HisConfigCFG.icdServiceHasRequireCheck && icdServices != null && icdServices.Count > 0))
                {
                    if (icdServices == null || icdServices.Count == 0)
                    {
                        return serviceIds;
                    }

                    bool checkByService = icdServices.Any(o => o.SERVICE_ID > 0);
                    bool checkByActiveIngredient = icdServices.Any(o => o.ACTIVE_INGREDIENT_ID > 0);
                    foreach (var item in mediADOs)
                    {
                        bool valid = true;
                        if (checkByService)
                        {
                            valid = icdServices.Any(o => o.SERVICE_ID > 0 && o.SERVICE_ID == item.SERVICE_ID);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("GetServiceIdNotIcdService:1." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM) && checkByActiveIngredient && valid)
                        {
                            valid = ValidMediAcin(icdServices, item);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("GetServiceIdNotIcdService:2." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                        if (!valid)
                        {
                            serviceIdNotIcdServices.Add(item.SERVICE_ID);
                        }
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.icdServiceHasRequireCheck), HisConfigCFG.icdServiceHasRequireCheck) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdServices), icdServices) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceIdNotIcdServices), serviceIdNotIcdServices) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => icdCodes), icdCodes));
            }
            catch (Exception ex)
            {
                serviceIdNotIcdServices = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serviceIdNotIcdServices;
        }

        private bool ValidMediAcin(List<HIS_ICD_SERVICE> icdServices1, MediMatyTypeADO mediADO)
        {
            bool isValid = false;
            try
            {
                if (this.currentMedicineTypeAcins == null)
                {
                    this.currentMedicineTypeAcins = BackendDataWorker.Get<HIS_MEDICINE_TYPE_ACIN>();
                }
                if (this.currentMedicineTypeAcins != null && this.currentMedicineTypeAcins.Count > 0 && mediADO != null)
                {
                    var medicineTypeAcinF1s = this.currentMedicineTypeAcins.Where(o => mediADO.ID == o.MEDICINE_TYPE_ID).ToList();
                    if (medicineTypeAcinF1s != null && medicineTypeAcinF1s.Count > 0)
                    {
                        var acinIgrIds = medicineTypeAcinF1s.Select(o => o.ACTIVE_INGREDIENT_ID).ToList();
                        isValid = icdServices1.Any(o => o.ACTIVE_INGREDIENT_ID > 0 && acinIgrIds.Contains(o.ACTIVE_INGREDIENT_ID ?? 0));
                        if (!isValid)
                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay HIS_ICD_SERVICE theo medicineTypeId = " + mediADO.ID + " & cac hoat chat cua loai thuoc do (" + String.Join(",", acinIgrIds) + ")");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ValidMediAcin: Khong co du lieu MedicineTypeAcins theo medicineTypeId = " + mediADO.ID);
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("ValidMediAcin: Khong co du lieu danh muc MedicineTypeAcins");
                }
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return isValid;
        }

        //private bool CheckSunSatAppointment()
        //{
        //    bool result = true;
        //    try
        //    {
        //        if (treatmentFinishProcessor != null)
        //        {
        //            var treatDT = treatmentFinishProcessor.GetDataOutput(ucTreatmentFinish);
        //            if (treatDT != null)
        //            {
        //                if (treatDT.IsAutoTreatmentFinish && treatDT.dtAppointmentTime != null && treatDT.dtAppointmentTime != DateTime.MinValue)
        //                {
        //                    long appointmentTime = Inventec.Common.TypeConvert.Parse.ToInt64((treatDT.dtAppointmentTime.ToString("yyyyMMddHHmm") + "00").ToString());
        //                    if (!this.CheckSunSatAppointmentTime(appointmentTime))
        //                    {
        //                        result = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private bool CheckWarringIntructionUseDayNum()
        {
            bool result = true;
            try
            {
                if (GlobalStore.IsTreatmentIn && !GlobalStore.IsCabinet && HisConfigCFG.WarringIntructionUseDayNum.HasValue)
                {
                    var instructionTimeMedis = GetInstructionTimeMedi();
                    DateTime dtAdd = DateTime.Now.AddDays(HisConfigCFG.WarringIntructionUseDayNum.Value);
                    long intructionTimeTemp = instructionTimeMedis.OrderByDescending(o => o).First();
                    DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeTemp).Value;
                    if (DateTime.Compare(dtIntructionTime.Date, dtAdd.Date) > 0)
                    {
                        MessageBox.Show(String.Format("Thời gian y lệnh lớn hơn thời gian giới hạn kê thuốc. ({0} ngày)", HisConfigCFG.WarringIntructionUseDayNum.Value));
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckUseDayAndExpTimeBHYT()
        {
            bool result = true;
            try
            {
                //Check key cau hinh
                if (HisConfigCFG.IsWarringUseDayAndExpTimeBHYT && !GlobalStore.IsTreatmentIn)
                {
                    if (this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                    {
                        //Thoi gian dung
                        List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                        if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                        {
                            var instructionTimeMedis = GetInstructionTimeMedi();
                            long? maxUseTimeTo = mediMatyTypeADOs.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).Max(o => o.UseTimeTo);
                            if (maxUseTimeTo > 0
                                && this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME.HasValue
                                && maxUseTimeTo / 1000000 > this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME.Value / 1000000)
                            {
                                long intructionTimeLong = instructionTimeMedis != null ? instructionTimeMedis
                                    .OrderBy(o => o).FirstOrDefault() : 0;
                                if (intructionTimeLong > 0)
                                {
                                    DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(intructionTimeLong).Value;
                                    DateTime dtHeinCardToTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0).Value;
                                    TimeSpan ts = (TimeSpan)(dtHeinCardToTime.Date - dtIntructionTime.Date);

                                    DialogResult myResult;
                                    myResult = MessageBox.Show(String.Format(ResourceMessage.BenhNhanSapHetHanTheBHYT_SoNgayThuocToiDa_BanCoMuonTiepTuc, ts.TotalDays + 1), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                    if (myResult != DialogResult.Yes)
                                    {
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        //private bool CheckTreatmentFinish()
        //{
        //    bool result = true;
        //    try
        //    {
        //        if (treatmentFinishProcessor != null)
        //        {
        //            var treatDT = treatmentFinishProcessor.GetDataOutput(ucTreatmentFinish);
        //            if (treatDT.IsAutoTreatmentFinish)
        //            {
        //                if (HisConfigCFG.CheckSameHein == 1)
        //                {
        //                    bool checkSameHein = false;
        //                    CommonParam param = new CommonParam();

        //                    HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
        //                    patientTypeAlterFilter.TREATMENT_ID = treatmentId;

        //                    var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

        //                    if (patientTypeAlter != null && patientTypeAlter.Count >= 2)
        //                    {
        //                        foreach (var item in patientTypeAlter)
        //                        {
        //                            var sameHein = patientTypeAlter.Where(o => o.HEIN_CARD_NUMBER == item.HEIN_CARD_NUMBER).ToList();
        //                            if (sameHein != null && sameHein.Count >= 2)
        //                            {
        //                                var checkHeinOrg = sameHein.Select(o => o.HEIN_MEDI_ORG_CODE).Distinct().ToList();
        //                                if (checkHeinOrg.Count > 1)
        //                                {
        //                                    //Mã cskcb khác nhau
        //                                    checkSameHein = true;
        //                                    break;
        //                                }
        //                                else
        //                                {
        //                                    var checkRightRoute = sameHein.Select(o => o.RIGHT_ROUTE_CODE).Distinct().ToList();
        //                                    if (checkRightRoute.Count == 1)
        //                                    {
        //                                        //Đúng tuyến và lý do đúng tuyến khác nhau
        //                                        if (checkRightRoute.FirstOrDefault() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
        //                                        {
        //                                            var checkRightRouteType = sameHein.Select(o => o.RIGHT_ROUTE_TYPE_CODE).Distinct().ToList();
        //                                            if (checkRightRouteType.Count > 1)
        //                                            {
        //                                                checkSameHein = true;
        //                                                break;
        //                                            }
        //                                        }
        //                                    }
        //                                    else
        //                                    {
        //                                        //Trái tuyến
        //                                        checkSameHein = true;
        //                                        break;
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }

        //                    if (checkSameHein)
        //                    {
        //                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoSaiThongTinTheBHYT, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
        //                        result = false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        //private bool CheckSunSatAppointmentTime(long appointmentTime)
        //{
        //    bool result = true;
        //    try
        //    {
        //        if (appointmentTime > 0)
        //        {
        //            DateTime dtAppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(appointmentTime) ?? DateTime.Now;
        //            if (dtAppointmentTime.DayOfWeek == DayOfWeek.Sunday)
        //            {
        //                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaChuNhat,
        //                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
        //                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
        //                    return false;
        //            }
        //            else if (dtAppointmentTime.DayOfWeek == DayOfWeek.Saturday)
        //            {
        //                if (DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoNgayHenLaThuBay,
        //                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
        //                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return false;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = false;
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return result;
        //}

        private bool CheckMaxExpend()
        {
            bool result = true;
            try
            {
                List<MediMatyTypeADO> mediMatyTypeADOs = gridControlServiceProcess.DataSource as List<MediMatyTypeADO>;
                if (mediMatyTypeADOs != null && mediMatyTypeADOs.Count > 0)
                {
                    //Get list expend
                    List<MediMatyTypeADO> mediMatyIsExpends = mediMatyTypeADOs.Where(o => o.IsExpend).ToList();
                    if (mediMatyIsExpends != null && mediMatyIsExpends.Count > 0)
                    {
                        decimal totalPrice = mediMatyIsExpends.Sum(o => o.TotalPrice);
                        V_HIS_SERE_SERV sereServParent = currentSereServInEkip != null ? currentSereServInEkip : currentSereServ;
                        if (sereServParent != null)
                        {
                            V_HIS_SERVICE service = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>()
                       .FirstOrDefault(o => o.ID == sereServParent.SERVICE_ID && o.MAX_EXPEND.HasValue);
                            if (service != null && totalPrice > service.MAX_EXPEND)
                            {
                                MessageBox.Show(String.Format(ResourceMessage.TongTienDichVuHaoPhiVuotQuaMucGiaTriDuocCauHinh, service.MAX_EXPEND), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                result = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Nếu trong ngày có 2 thuốc cùng thuộc nhóm kháng sinh
        //thì khi lưu đơn đưa ra cảnh báo. Trong ngày đã có thuốc kháng sinh: ABC. bạn có muốn kê (có => lưu, không => ko lưu để người dùng sửa)
        //Kiểm tra theo ngày y lệnh (intruction_date)
        //Hoặc nếu trong 1 đơn có 2 thuốc cùng thuộc nhóm kháng sinh thì cảnh báo khi lưu.
        //Đơn thuốc có 2 thuốc thuộc nhóm kháng sinh: ABC, DEF. Bạn có muốn kê ?(có => lưu, không => ko lưu để người dùng sửa)
        /// </summary>
        /// <returns></returns>
        private bool CheckThuocKhangSinhTrongNgay()
        {
            bool result = true;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckThuocKhangSinhTrongNgay. 1");
                List<V_HIS_MEDICINE_TYPE> medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                var instructionTimeMedis = GetInstructionTimeMedi();
                List<V_HIS_MEDICINE_TYPE> medicineTypeInTreatments = new List<V_HIS_MEDICINE_TYPE>();
                //Khang sinh theo trong ngay
                if (sereServWithTreatment != null && sereServWithTreatment.Count > 0)
                {
                    List<string> intructionDateStrs = instructionTimeMedis != null && instructionTimeMedis.Count > 0
                        ? (from r in instructionTimeMedis select r.ToString().Substring(0, 8)).ToList() : null;
                    if (intructionDateStrs != null && intructionDateStrs.Count > 0)
                    {
                        List<HIS_SERE_SERV> sereServTodays = sereServWithTreatment.Where(o => intructionDateStrs.Contains(o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8))).ToList();
                        if (sereServTodays != null && sereServTodays.Count > 0)
                        {
                            List<long> sereServWithTreatmentServiceIds = sereServTodays.Select(o => o.SERVICE_ID).ToList();
                            medicineTypeInTreatments = medicineTypes.Where(
                                o => sereServWithTreatmentServiceIds.Contains(o.SERVICE_ID)
                                && o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS
                                ).ToList();
                        }
                    }
                }
                string mssThongBao = LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao);
                //Khang sinh trong 1 lan ke don
                List<MediMatyTypeADO> mediMatyTypeADONotEdits = this.mediMatyTypeADOs.Where(o => o.IsEdit == false).ToList();
                List<long> mediMatyTypeADONotEditServiceIds = mediMatyTypeADONotEdits.Select(o => o.SERVICE_ID).ToList();
                List<V_HIS_MEDICINE_TYPE> medicineTypeAssigns = medicineTypes.Where(
                o => mediMatyTypeADONotEditServiceIds.Contains(o.SERVICE_ID)
                && o.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS).ToList();
                if (medicineTypeAssigns != null && medicineTypeAssigns.Count > 0)
                {
                    if (medicineTypeAssigns.Count > 1)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CheckThuocKhangSinhTrongNgay. 1.1");
                        string medicineTypeNames = medicineTypeAssigns.Aggregate((i, j) =>
                            new V_HIS_MEDICINE_TYPE { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME;
                        string messageError = String.Format(ResourceMessage.DonThuocCoCacThuocThuocNhomKhangSinhBanCOMuonTiepTuc, medicineTypeNames);

                        DialogResult myResult;
                        myResult = DevExpress.XtraEditors.XtraMessageBox.Show(messageError, mssThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                            result = false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => mssThongBao), mssThongBao) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageError), messageError));
                    }
                    else if (medicineTypeInTreatments != null && medicineTypeInTreatments.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("CheckThuocKhangSinhTrongNgay. 1.2");
                        string medicineTypeNames = medicineTypeInTreatments.Aggregate((i, j) =>
                            new V_HIS_MEDICINE_TYPE { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME;
                        string messageError = String.Format(ResourceMessage.TrongNgayDaCoThuocKhangSinhBanCoMuonTiepTuc, medicineTypeNames);
                        DialogResult myResult;
                        myResult = DevExpress.XtraEditors.XtraMessageBox.Show(messageError, mssThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                            result = false;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageError), messageError));
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("CheckThuocKhangSinhTrongNgay. 2");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        /// <summary>
        /// Khi kê đơn thuốc (kiểm tra thuốc trong đơn)
        //Nếu có 2 thuốc trùng hoạt chất thì đưa ra cảnh báo (kiểm tra theo hoạt chất bhyt)
        //Đơn thuốc có 2 thuốc trùng hoạt chất: Abc, DEF, bạn có muốn tiếp tục? (có => lưu, không => ko lưu để người dùng sửa)
        //issue: 13452
        /// </summary>
        /// <returns></returns>
        private bool CheckCungHoatChat()
        {
            bool result = true;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0)
                {
                    string medicineTypeNames = "";
                    var mediMatyTypeADOGroups = this.mediMatyTypeADOs.Where(o => o.ACTIVE_INGR_BHYT_CODE != null).GroupBy(o => o.ACTIVE_INGR_BHYT_CODE.ToUpper());
                    foreach (var g in mediMatyTypeADOGroups)
                    {
                        if (g.Count() > 1)
                        {
                            medicineTypeNames += (g.Aggregate((i, j) => new MediMatyTypeADO { MEDICINE_TYPE_NAME = i.MEDICINE_TYPE_NAME + ";" + j.MEDICINE_TYPE_NAME }).MEDICINE_TYPE_NAME) + "\r\n";
                        }
                    }

                    if (!String.IsNullOrEmpty(medicineTypeNames))
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show(String.Format(ResourceMessage.CacThuocCoCUngHoatChat_BanCoMuonTiepTuc, medicineTypeNames), Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (myResult != System.Windows.Forms.DialogResult.Yes)
                            result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckMediStockWhenEditPrescription()
        {
            bool result = true;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && isMediMatyIsOutStock)
                {
                    var groups = this.mediMatyTypeADOs.Where(o => o.MEDI_STOCK_ID > 0).GroupBy(o => new { o.MEDI_STOCK_ID });
                    if (groups.Count() > 1)
                    {
                        MessageBox.Show(ResourceMessage.DanhSachThuocVatTuTonTai2KhoXuat, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
                    }
                }
            }

            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckPrescriptionSplitOutMediStock()
        {
            bool result = true;
            try
            {
                if (this.mediMatyTypeADOs != null && this.mediMatyTypeADOs.Count > 0 && this.actionType == GlobalVariables.ActionEdit && HisConfigCFG.isPrescriptionSplitOutMediStock)
                {
                    MediMatyTypeADO mediMatyTypeADOInStock = this.mediMatyTypeADOs.FirstOrDefault(o => o.MEDI_STOCK_ID > 0);
                    MediMatyTypeADO mediMatyTypeADOOutStock = this.mediMatyTypeADOs.FirstOrDefault(o => o.MEDI_STOCK_ID == null || o.MEDI_STOCK_ID == 0);
                    if (mediMatyTypeADOInStock != null && mediMatyTypeADOOutStock != null)
                    {
                        MessageBox.Show(ResourceMessage.SuaDonThuocKhongChoPhepTachDonThuocTrongKhoVaNgoaiKho, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Có số lượng không khả dụng trong kho
        /// </summary>
        /// <returns></returns>
        private bool CheckAmoutWarringInStock()
        {
            bool result = true;
            try
            {
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => ((o.AmountAlert ?? 0) > 0 || o.ErrorTypeMediMatyBean == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning) && (o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_TSD)).ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }
                    message += ResourceMessage._KhongDuKhaDungTrongKho;
                }
                if (!String.IsNullOrEmpty(message))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        ///#17940
        ///Trong trường hợp kê đơn nhiều ngày thì chỉ cho phép kê với những ngày y lệnh nhỏ hơn hoặc bằng ngày hết hạn 
        ///sử dụng. Các ngày y lệnh lớn hơn ngày hết hạn thì chặn lại, không cho lưu. Khi lưu sẽ có cảnh báo, lấy ra 
        ///ngày hết hạn của thuốc. Xử lý thất bại.Thuốc/ vật tư  (Mã thuốc. vật tư - tên thuốc, vật tư) có hạn sử dụng < thời gian y lệnh. Ngày hết hạn:..... --- Ngày y lệnh:......
        private bool CheckMediMatyDontPresExpiredTime()
        {
            bool result = true;
            try
            {
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    string message = "";
                    List<MediMatyTypeADO> mediMatyTypeWarnings =
                        this.mediMatyTypeADOs.Where(o => ((o.EXPIRED_DATE ?? 0) > 0)).ToList();
                    if (mediMatyTypeWarnings != null && mediMatyTypeWarnings.Count > 0)
                    {
                        foreach (var item in mediMatyTypeWarnings)
                        {
                            message += String.Format(ResourceMessage.WarnThuocVatTuCoHanSuDungNhoHonThoiGianYLenh,
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_CODE, System.Drawing.Color.Maroon),
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Maroon),
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0), System.Drawing.Color.Maroon),
                                Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.DateTime.Convert.TimeNumberToDateString(InstructimeForWarnPresExpiredTime(item)), System.Drawing.Color.Maroon) + " \n");
                        }
                    }
                    if (!String.IsNullOrEmpty(message))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        long InstructimeForWarnPresExpiredTime(MediMatyTypeADO item)
        {
            try
            {
                return (item.IntructionTimeSelecteds != null && item.IntructionTimeSelecteds.Count > 0 ? item.IntructionTimeSelecteds.First() : this.intructionTimeSelecteds.First());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private bool CheckAmoutWarringNumber()
        {
            bool result = true;
            try
            {
                string message = "";
                var mediMatyTypeInStockWarnings = this.mediMatyTypeADOs.Where(o => (o.AMOUNT ?? 0) == 0 || o.ErrorTypeAmount == DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning).ToList();
                if (mediMatyTypeInStockWarnings != null && mediMatyTypeInStockWarnings.Count > 0)
                {
                    foreach (var item in mediMatyTypeInStockWarnings)
                    {
                        message += (Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(item.MEDICINE_TYPE_NAME, System.Drawing.Color.Red)
                            + " : " + Inventec.Desktop.Common.HtmlString.ProcessorString.InsertColor(Inventec.Common.Number.Convert.NumberToString((item.AmountAlert ?? 0), ConfigApplications.NumberSeperator), System.Drawing.Color.Maroon) + "; ");
                    }
                    message += ResourceMessage._KhongNhapSoLuongKe;
                }
                if (!String.IsNullOrEmpty(message))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True);

                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

    }
}
