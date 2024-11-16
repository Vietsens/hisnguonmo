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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.UC.Icd;
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
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private bool Valid(List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            CommonParam param = new CommonParam();
            bool valid = true;
            try
            {
                this.positionHandleControl = -1;
                valid = (this.dxValidationProviderControl.Validate());
                valid = valid && this.CheckValidDataInGridService(param, serviceCheckeds__Send);
                valid = valid && this.CheckValidHeinServicePrice(param, serviceCheckeds__Send);

                if (!valid && param.Messages != null && param.Messages.Count > 0)
                {
                    MessageManager.Show(param, null);
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        private bool CheckValidDataInGridService(CommonParam param, List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    foreach (var item in serviceCheckeds__Send)
                    {
                        string messageErr = "";
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {
                            messageErr = String.Format(ResourceMessage.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU)
                        {
                            messageErr = String.Format(ResourceMessage.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                        }

                        if ((item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU) && item.PATIENT_TYPE_ID <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongCoDoiTuongThanhToan;
                            Inventec.Common.Logging.LogSystem.Warn("Thuoc/Vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " khong co doi tuong thanh toan.");
                        }
                        if (item.AMOUNT <= 0)
                        {
                            valid = false;
                            messageErr += ResourceMessage.KhongNhapSoLuong;
                            Inventec.Common.Logging.LogSystem.Warn("Thuoc/vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " khong co so luong.");
                        }
                        if ((item.AmountAlert ?? 0) > 0)
                        {
                            valid = false;
                            messageErr += (" " + ResourceMessage.SoLuongXuatLonHonSpoLuongKhadungTrongKho);
                            Inventec.Common.Logging.LogSystem.Warn("Thuoc/vat tu (" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " co so luong lon hon so luong kha dung trong kho.");
                        }
                        if (item.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC && item.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (item.MEDICINE_USE_FORM_ID ?? 0) == 0 && (item.DO_NOT_REQUIRED_USE_FORM ?? -1) != RequiredUseFormCFG.DO_NOT_REQUIRED)
                        {
                            messageErr += ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung;
                            valid = false;
                            Inventec.Common.Logging.LogSystem.Warn("Doi tuong thanh toan bhyt bat buoc phai nhap thong tin duong dung cua thuoc.");
                        }

                        if (!valid)
                        {
                            param.Messages.Add(messageErr + ";");
                        }
                    }
                }
                else
                {
                    Inventec.Desktop.Common.LibraryMessage.MessageUtil.SetParam(param, Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        /// <summary>
        /// Hàm kiểm tra số tiền đã kê có vượt quá cấu hình trần bảo hiểm không
        /// Trường hợp trần được cấu hình trong hồ sơ điều trị thì lấy từ hsdt
        /// Trường hợp hsdt không cấu hình trần thì lấy trần từ cấu hình ccc ra
        /// Chỉ check trần số tiền bhyt khi có cấu hình trần
        /// </summary>
        /// <param name="param"></param>
        /// <param name="serviceCheckeds__Send"></param>
        /// <returns></returns>
        private bool CheckValidHeinServicePrice(CommonParam param, List<MediMatyTypeADO> serviceCheckeds__Send)
        {
            bool valid = true;
            decimal tongtienThuocPhatSinh = 0;
            string messageErr = "";
            try
            {
                if (serviceCheckeds__Send != null && serviceCheckeds__Send.Count > 0)
                {
                    decimal limitHeinMedicinePrice__RightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg;
                    decimal limitHeinMedicinePrice__NotRightMediOrg = HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg;

                    //Kiểm tra, nếu có key cấu hình "check giới hạn thuốc", thì khi người dùng nhấn nút "Lưu" mới lấy thông tin hồ sơ điều trị để check
                    if (limitHeinMedicinePrice__RightMediOrg > 0
                        || limitHeinMedicinePrice__NotRightMediOrg > 0)
                    {
                        //Load lại thông tin hồ sơ điều trị
                        this.currentTreatmentWithPatientType = this.LoadDataToCurrentTreatmentData(treatmentId, this.intructionTimeSelecteds.OrderByDescending(o => o).First());

                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType.TREATMENT_CODE), this.currentTreatmentWithPatientType.TREATMENT_CODE) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP), this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE), this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE));

                        //Nếu hồ sơ điều trị cấu hình trường IS_NOT_CHECK_LHMP = 1 thì bỏ qua không check, return true
                        //Hoặc đối tượng điều tị là điều trị nội/ngoại trú thì bỏ qua không check
                        if ((this.currentTreatmentWithPatientType != null
                        && this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP.HasValue
                        && (this.currentTreatmentWithPatientType.IS_NOT_CHECK_LHMP ?? 0) == GlobalVariables.CommonNumberTrue)
                        || this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatIn
                        || this.currentHisPatientTypeAlter.TREATMENT_TYPE_CODE == HisConfigCFG.TreatmentTypeCode__TreatOut
                            )
                        {
                            return true;
                        }

                        this.limitHeinMedicinePrice = this.IsLimitHeinMedicinePrice(treatmentId);

                        var bhyt__Exists = serviceCheckeds__Send
                            .Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC
                                && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                                && !o.IsExpend).ToList();
                        //Kiểm tra tiền bhyt đã kê vượt mức giới hạn chưa
                        if (limitHeinMedicinePrice == true)
                        {
                            valid = false;
                        }
                        else if (bhyt__Exists != null
                            && bhyt__Exists.Count > 0
                            && this.currentHisPatientTypeAlter != null
                            && (limitHeinMedicinePrice__RightMediOrg > 0 || limitHeinMedicinePrice__NotRightMediOrg > 0)
                            )
                        {
                            foreach (var item in bhyt__Exists)
                            {
                                tongtienThuocPhatSinh += (item.TotalPrice);
                            }

                            //Đối với bệnh nhân đúng tuyến KCB
                            if (limitHeinMedicinePrice__RightMediOrg > 0
                                && this.currentHisPatientTypeAlter.HEIN_MEDI_ORG_CODE == HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT)
                            {
                                if (tongtienThuocPhatSinh + totalPriceBHYT > limitHeinMedicinePrice__RightMediOrg)
                                {
                                    messageErr = String.Format(ResourceMessage.SoTienDaKeChoBHYTDaVuotquaMucGioiHan, Inventec.Common.Number.Convert.NumberToString(tongtienThuocPhatSinh + totalPriceBHYT, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToString(HisConfigCFG.LimitHeinMedicinePrice__RightMediOrg));
                                    valid = false;
                                }
                            }

                            //Đối với bệnh nhân chuyển tuyến
                            if (limitHeinMedicinePrice__NotRightMediOrg > 0
                                && this.currentHisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                            {
                                if (tongtienThuocPhatSinh + totalPriceBHYT > limitHeinMedicinePrice__NotRightMediOrg)
                                {
                                    messageErr = String.Format(ResourceMessage.SoTienDaKeChoBHYTDaVuotquaMucGioiHan, Inventec.Common.Number.Convert.NumberToString(tongtienThuocPhatSinh + totalPriceBHYT), Inventec.Common.Number.Convert.NumberToString(HisConfigCFG.LimitHeinMedicinePrice__NotRightMediOrg, ConfigApplications.NumberSeperator));
                                    valid = false;
                                }
                            }
                        }

                        if (!valid)
                        {
                            param.Messages.Add(messageErr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

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
                var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                if (icdValue != null && icdValue is IcdInputADO)
                {
                    icdCode = ((IcdInputADO)icdValue).ICD_CODE;
                }
                if (this.ucSecondaryIcd != null)
                {
                    var subIcd = this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        icdCode += ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                    }
                }

                if (!String.IsNullOrEmpty(icdCode))
                {
                    string[] icdCodeArr = icdCode.Split(';');

                    if (HisConfigCFG.icdServiceHasCheck != 0)
                    {
                        List<long> serviceIds = this.mediMatyTypeADOs.Where(o => o.SERVICE_ID > 0).Select(p => p.SERVICE_ID).ToList();
                        List<long> serviceNotConfigIds = GetServiceIdNotIcdService(icdCodeArr.ToList(), serviceIds);

                        if (serviceNotConfigIds != null && serviceNotConfigIds.Count > 0)
                        {
                            if (HisConfigCFG.icdServiceHasCheck == 1)
                            {
                                MessageBox.Show("Tồn tại dịch vụ chưa được cấu hình chẩn đoán (ICD).", "Thông báo",
                                    MessageBoxButtons.OK, MessageBoxIcon.Question);
                                return false;
                            }
                            else if (HisConfigCFG.icdServiceHasCheck == 2)
                            {
                                DialogResult myResult = MessageBox.Show("Tồn tại dịch vụ chưa được cấu hình chẩn đoán (ICD). Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                if (myResult != DialogResult.OK)
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
                                List<long> serviceIds = this.mediMatyTypeADOs.Where(o => serviceIdCFGs.Contains(o.SERVICE_ID)).Select(p => p.SERVICE_ID).ToList();

                                if (serviceIds != null && serviceIds.Count > 0)
                                {
                                    List<long> serviceNotConfigIds = GetServiceIdNotIcdService(icdCodeArr.ToList(), serviceIds);

                                    if (serviceNotConfigIds != null && serviceNotConfigIds.Count > 0)
                                    {
                                        List<MediMatyTypeADO> serviceNotConfigs = this.mediMatyTypeADOs.Where(o => serviceNotConfigIds.Contains(o.SERVICE_ID)).ToList();
                                        string medicineTypeNameStr = "";
                                        foreach (var item in serviceNotConfigs)
                                        {
                                            medicineTypeNameStr += item.MEDICINE_TYPE_NAME + ";";
                                        }

                                        DialogResult myResult = MessageBox.Show(String.Format("{0} \nDịch vụ trong cấu hình thuốc chưa được cấu hình chẩn đoán (ICD). Bạn có muốn tiếp tục không?", medicineTypeNameStr), "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                        if (myResult != DialogResult.OK)
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

        private List<long> GetServiceIdNotIcdService(List<string> icdCodes, List<long> serviceIds)
        {
            List<long> serviceIdNotIcdServices = new List<long>();
            try
            {
                CommonParam param = new CommonParam();
                HisIcdServiceFilter icdServiceFilter = new HisIcdServiceFilter();
                icdServiceFilter.SERVICE_IDs = serviceIds;
                icdServiceFilter.ICD_CODE__EXACTs = icdCodes;
                List<HIS_ICD_SERVICE> icdServices = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.HIS_ICD_SERVICE>>("api/HisIcdService/Get", ApiConsumers.MosConsumer, icdServiceFilter, param);

                List<long> icdServiceIds = icdServices.Select(o => o.SERVICE_ID ?? 0).Distinct().ToList();
                foreach (var item in serviceIds)
                {
                    if (!icdServiceIds.Contains(item))
                    {
                        serviceIdNotIcdServices.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                serviceIdNotIcdServices = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return serviceIdNotIcdServices;
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

                            long? maxUseTimeTo = mediMatyTypeADOs.Where(o => o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT).Max(o => o.UseTimeTo);
                            if (maxUseTimeTo > 0)
                            {
                                if (maxUseTimeTo / 1000000 > this.currentHisPatientTypeAlter.HEIN_CARD_TO_TIME.Value / 1000000)
                                {
                                    DialogResult myResult;
                                    myResult = MessageBox.Show("Bệnh nhân sắp hết hạn thẻ BHYT. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                                    if (myResult != DialogResult.OK)
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

        private bool CheckTreatmentFinish()
        {
            bool result = true;
            try
            {
                if (treatmentFinishProcessor != null)
                {
                    var treatDT = treatmentFinishProcessor.GetDataOutput(ucTreatmentFinish);
                    if (treatDT.IsAutoTreatmentFinish)
                    {
                        var subIcd = (SecondaryIcdDataADO)this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                        if (subIcd != null && !string.IsNullOrEmpty(subIcd.ICD_SUB_CODE))
                        {
                            var subIcdList = subIcd.ICD_SUB_CODE.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            if (subIcdList != null && subIcdList.Count > 12)
                            {
                                if ((HisConfigCFG.IsCheckSubIcdExceedLimit == "1" && DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Vui lòng kiểm tra lại",
HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
MessageBoxButtons.OK) == DialogResult.OK) || (HisConfigCFG.IsCheckSubIcdExceedLimit == "2" && DevExpress.XtraEditors.XtraMessageBox.Show("Chẩn đoán phụ nhập quá 12 mã bệnh. Bạn có muốn tiếp tục?",
HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
MessageBoxButtons.YesNo) == DialogResult.No))
                                {
                                    return result = false;
                                }
                            }
                        }
                        if (HisConfigCFG.CheckSameHein == 1)
                        {
                            bool checkSameHein = false;
                            CommonParam param = new CommonParam();

                            HisPatientTypeAlterViewFilter patientTypeAlterFilter = new HisPatientTypeAlterViewFilter();
                            patientTypeAlterFilter.TREATMENT_ID = treatmentId;

                            var patientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, patientTypeAlterFilter, param);

                            if (patientTypeAlter != null && patientTypeAlter.Count >= 2)
                            {
                                foreach (var item in patientTypeAlter)
                                {
                                    var sameHein = patientTypeAlter.Where(o => o.HEIN_CARD_NUMBER == item.HEIN_CARD_NUMBER).ToList();
                                    if (sameHein != null && sameHein.Count >= 2)
                                    {
                                        var checkHeinOrg = sameHein.Select(o => o.HEIN_MEDI_ORG_CODE).Distinct().ToList();
                                        if (checkHeinOrg.Count > 1)
                                        {
                                            //Mã cskcb khác nhau
                                            checkSameHein = true;
                                            break;
                                        }
                                        else
                                        {
                                            var checkRightRoute = sameHein.Select(o => o.RIGHT_ROUTE_CODE).Distinct().ToList();
                                            if (checkRightRoute.Count == 1)
                                            {
                                                //Đúng tuyến và lý do đúng tuyến khác nhau
                                                if (checkRightRoute.FirstOrDefault() == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                                                {
                                                    var checkRightRouteType = sameHein.Select(o => o.RIGHT_ROUTE_TYPE_CODE).Distinct().ToList();
                                                    if (checkRightRouteType.Count > 1)
                                                    {
                                                        checkSameHein = true;
                                                        break;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //Trái tuyến
                                                checkSameHein = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            if (checkSameHein)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.CanhBaoSaiThongTinTheBHYT, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                result = false;
                            }
                        }
                        if (string.IsNullOrEmpty(Histreatment.TREATMENT_METHOD) && ((HisConfigCFG.RequiredTreatmentMethodOption == "1" && Histreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU && (treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV)) || (HisConfigCFG.RequiredTreatmentMethodOption == "2" && (treatDT.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM || ((Histreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU || Histreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || Histreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTBANNGAY) && (treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN || treatDT.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CTCV))))))
                        {
                            frmTreatmentMethod frm = new frmTreatmentMethod(TreatmentMethod, treatmentMethod =>
                            {
                                TreatmentMethod = treatmentMethod;
                            });
                            frm.ShowDialog();
                            if (string.IsNullOrEmpty(TreatmentMethod))
                                result = false;
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
    }
}
