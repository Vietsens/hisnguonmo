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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.UC.PatientPackagePicker;
using HIS.UC.PatientPackagePicker.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription
    {
        #region Patient package

        /// <summary>Cache V_HIS_SERVICE theo ID — build trước khi gọi Pick, dùng trong filter/mapping. Tránh phụ thuộc property SV_* trên V_HIS_PATIENT_PACKAGE_DT (có thể không tồn tại nếu runtime dùng MOS.EFMODEL bản cũ).</summary>
        private Dictionary<long, V_HIS_SERVICE> packagePickerServiceDict;

        /// <summary>Set property bằng reflection — bỏ qua nếu property không có (chống MissingMethodException khi DLL runtime khác DLL biên dịch).</summary>
        private static void TrySetProperty(object target, string propertyName, object value)
        {
            try
            {
                if (target == null) return;
                var prop = target.GetType().GetProperty(propertyName);
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(target, value, null);
                }
            }
            catch { /* swallow — property absent in this DLL version */ }
        }

        /// <summary>Read decimal property bằng reflection — return 0 nếu không có/null. Chống MissingMethodException.</summary>
        private static decimal TryGetDecimalProperty(object target, string propertyName)
        {
            try
            {
                if (target == null) return 0m;
                var prop = target.GetType().GetProperty(propertyName);
                if (prop == null) return 0m;
                var v = prop.GetValue(target, null);
                if (v == null) return 0m;
                return Convert.ToDecimal(v);
            }
            catch { return 0m; }
        }

        /// <summary>Read long property bằng reflection — return 0 nếu không có/null. Chống MissingMethodException.</summary>
        private static long TryGetLongProperty(object target, string propertyName)
        {
            try
            {
                if (target == null) return 0L;
                var prop = target.GetType().GetProperty(propertyName);
                if (prop == null) return 0L;
                var v = prop.GetValue(target, null);
                if (v == null) return 0L;
                return Convert.ToInt64(v);
            }
            catch { return 0L; }
        }


        /// <summary>
        /// Khởi tạo cột "Gói bệnh nhân" và caption đa ngôn ngữ cho nút (nút đã có trên Designer).
        /// Gọi trong Load event của form chính.
        /// </summary>
        internal void InitPatientPackageFeature()
        {
            try
            {
                InitPatientPackageColumn();
                InitPatientPackageCaption();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Đặt cột "Gói bệnh nhân" ngay sau cột "Nguồn khác" (gridColumn11) trên lưới đơn thuốc.</summary>
        private void InitPatientPackageColumn()
        {
            try
            {
                if (gcPatientPackageName == null || gridColumn11 == null) return;
                gcPatientPackageName.Caption = Inventec.Common.Resource.Get.Value(
                    "frmAssignPrescription.gcPatientPackageName.Caption",
                    ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                gcPatientPackageName.OptionsColumn.AllowEdit = false;
                gcPatientPackageName.Visible = true;
                // Chèn ngay sau cột "Nguồn khác" — DevExpress tự dịch các cột phía sau.
                gcPatientPackageName.VisibleIndex = gridColumn11.VisibleIndex + 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Set caption đa ngôn ngữ cho nút "Gói bệnh nhân" (nút do Designer tạo).</summary>
        private void InitPatientPackageCaption()
        {
            try
            {
                if (btnPatientPackage == null) return;
                btnPatientPackage.Text = Inventec.Common.Resource.Get.Value(
                    "frmAssignPrescription.btnPatientPackage.Text",
                    ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Enable/disable nút theo trạng thái đã chọn kho xuất. Gọi trong cboMediStockExport_EditValueChanged và khi xóa kho.</summary>
        internal void SetEnablePatientPackageButton()
        {
            try
            {
                if (btnPatientPackage == null) return;
                bool hasMediStock = this.currentMediStock != null && this.currentMediStock.Count > 0;
                btnPatientPackage.Enabled = hasMediStock && this.actionType != GlobalVariables.ActionView;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPatientPackage_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("btnPatientPackage_Click => START");

                if (this.currentMediStock == null || this.currentMediStock.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        ResourceMessage.BanChuaChonKhoXuat,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                long patientId = GetPatientPackagePatientId();
                Inventec.Common.Logging.LogSystem.Debug("btnPatientPackage_Click => patientId=" + patientId);
                if (patientId <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Không xác định được bệnh nhân.",
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    return;
                }

                // 1. Load danh sách gói bệnh nhân đang hoạt động
                List<HIS_PATIENT_PACKAGE> activePackages = LoadActivePatientPackages(patientId);
                Inventec.Common.Logging.LogSystem.Debug("btnPatientPackage_Click => activePackages.Count=" + (activePackages == null ? 0 : activePackages.Count));
                if (activePackages == null || activePackages.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        ResourceMessage.VuiLongChonGoiBenhNhan,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }

                // 2. Build cache V_HIS_SERVICE — dùng cho filter (thuốc/vật tư) + map output. Tránh đọc property SV_* trực tiếp.
                this.packagePickerServiceDict = new Dictionary<long, V_HIS_SERVICE>();
                var serviceList = BackendDataWorker.Get<V_HIS_SERVICE>();
                if (serviceList != null)
                {
                    foreach (var sv in serviceList)
                    {
                        if (!this.packagePickerServiceDict.ContainsKey(sv.ID))
                            this.packagePickerServiceDict.Add(sv.ID, sv);
                    }
                }

                // 3. Gọi UC HIS.UC.PatientPackagePicker (Processor là static class)
                Inventec.Common.Logging.LogSystem.Debug("btnPatientPackage_Click => calling PatientPackagePickerProcessor.Pick");
                List<SelectedPatientPackageServiceADO> selected = null;
                try
                {
                    selected = PatientPackagePickerProcessor.Pick(
                        activePackages,
                        new frmPatientPackagePicker.LoadDetailDelegate(LoadPatientPackageDetailForPicker),
                        new frmPatientPackagePicker.DetailFilterDelegate(FilterMedicineOrMaterialDetail));
                }
                catch (Exception exPick)
                {
                    Inventec.Common.Logging.LogSystem.Error("PatientPackagePickerProcessor.Pick THREW:", exPick);
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Lỗi khi mở chọn gói bệnh nhân: " + exPick.Message,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi));
                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug("btnPatientPackage_Click => Pick returned. selected.Count=" + (selected == null ? 0 : selected.Count));

                if (selected == null || selected.Count == 0)
                    return;

                // 3. Map output UC sang schema nội bộ rồi đẩy vào lưới + chiếm tồn
                List<PatientPackageServiceADO> output = new List<PatientPackageServiceADO>();
                foreach (var s in selected)
                {
                    if (s == null || s.PatientPackage == null || s.PatientPackageDetail == null) continue;
                    long serviceIdVal = s.PatientPackageDetail.SERVICE_ID ?? 0;
                    long serviceTypeId = 0;
                    string serviceCode = null;
                    string serviceName = s.PatientPackageDetail.SERVICE_NAME;
                    V_HIS_SERVICE svcInfo;
                    if (serviceIdVal > 0 && this.packagePickerServiceDict != null
                        && this.packagePickerServiceDict.TryGetValue(serviceIdVal, out svcInfo) && svcInfo != null)
                    {
                        serviceTypeId = svcInfo.SERVICE_TYPE_ID;
                        serviceCode = svcInfo.SERVICE_CODE;
                        if (string.IsNullOrEmpty(serviceName)) serviceName = svcInfo.SERVICE_NAME;
                    }
                    // ĐTTT + Đơn giá lấy trực tiếp từ gói — đọc qua reflection để chống MissingMethodException nếu DLL runtime cũ thiếu property.
                    long packagePatientTypeId = TryGetLongProperty(s.PatientPackage, "PATIENT_TYPE_ID");
                    decimal unitPrice = TryGetDecimalProperty(s.PatientPackageDetail, "UNIT_PRICE");
                    Inventec.Common.Logging.LogSystem.Debug(
                        "btnPatientPackage_Click => map output: serviceId=" + serviceIdVal
                        + ", patientTypeId=" + packagePatientTypeId
                        + ", unitPrice=" + unitPrice
                        + ", amountThisTime=" + s.AmountThisTime);

                    output.Add(new PatientPackageServiceADO
                    {
                        ServiceId = serviceIdVal,
                        ServiceTypeId = serviceTypeId,
                        Amount = s.AmountThisTime,
                        PatientPackageId = s.PatientPackage.ID,
                        PatientPackageName = s.PatientPackage.PACKAGE_NAME,
                        ServiceCode = serviceCode,
                        ServiceName = serviceName,
                        PatientTypeId = packagePatientTypeId,
                        UnitPrice = unitPrice
                    });
                }

                if (output.Count > 0)
                    OnPatientPackageSelected(output);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("btnPatientPackage_Click outer:", ex);
                try
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Lỗi xử lý Gói bệnh nhân: " + ex.Message,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi));
                }
                catch { /* ignore secondary */ }
            }
        }

        /// <summary>Load danh sách gói bệnh nhân đang hoạt động — truyền vào UC PatientPackagePicker.</summary>
        private List<HIS_PATIENT_PACKAGE> LoadActivePatientPackages(long patientId)
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                MOS.Filter.HisPatientPackageFilter filter = new MOS.Filter.HisPatientPackageFilter();
                filter.PATIENT_ID = patientId;
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var result = new BackendAdapter(param).Get<List<HIS_PATIENT_PACKAGE>>(
                    RequestUriStore.HIS_PATIENT_PACKAGE__GET, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
                // Order theo CREATE_TIME (audit field — luôn có ở mọi phiên bản EFMODEL).
                // KHÔNG dùng REGISTER_DATE — property này có thể không tồn tại ở MOS.EFMODEL bản cũ → MissingMethodException tại JIT.
                return (result ?? new List<HIS_PATIENT_PACKAGE>())
                    .OrderByDescending(o => o.CREATE_TIME ?? 0).ToList();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<HIS_PATIENT_PACKAGE>();
            }
        }

        /// <summary>
        /// Callback UC truyền vào để load chi tiết của 1 gói. Lấy bảng gốc HIS_PATIENT_PACKAGE_DT
        /// (view filter không hỗ trợ lọc theo PATIENT_PACKAGE_ID) rồi map sang V_HIS_PATIENT_PACKAGE_DT,
        /// bổ sung SERVICE_CODE / SERVICE_TYPE_* từ cache V_HIS_SERVICE.
        /// </summary>
        private List<V_HIS_PATIENT_PACKAGE_DT> LoadPatientPackageDetailForPicker(long patientPackageId)
        {
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisPatientPackageDtFilter filter = new MOS.Filter.HisPatientPackageDtFilter();
                filter.PATIENT_PACKAGE_ID = patientPackageId;
                var bases = new BackendAdapter(param).Get<List<HIS_PATIENT_PACKAGE_DT>>(
                    RequestUriStore.HIS_PATIENT_PACKAGE_DT__GET, ApiConsumers.MosConsumer, filter, param);

                // Đảm bảo cache V_HIS_SERVICE đã được build (btnPatientPackage_Click build trước khi gọi Pick)
                Dictionary<long, V_HIS_SERVICE> serviceDict = this.packagePickerServiceDict ?? new Dictionary<long, V_HIS_SERVICE>();

                List<V_HIS_PATIENT_PACKAGE_DT> result = new List<V_HIS_PATIENT_PACKAGE_DT>();
                if (bases != null)
                {
                    foreach (var b in bases)
                    {
                        V_HIS_PATIENT_PACKAGE_DT v = new V_HIS_PATIENT_PACKAGE_DT();
                        v.ID = b.ID;
                        v.PATIENT_PACKAGE_ID = b.PATIENT_PACKAGE_ID;
                        v.SERVICE_ID = b.SERVICE_ID;
                        v.AMOUNT = b.AMOUNT;
                        TrySetProperty(v, "AMOUNT_USED", b.AMOUNT_USED);
                        v.SERVICE_NAME = b.SERVICE_NAME;
                        v.IS_ACTIVE = b.IS_ACTIVE;
                        // Copy UNIT_PRICE từ bảng gốc sang view (qua reflection — chống MissingMethodException).
                        // Đây là field "Đơn giá" của gói, dùng override "Thành tiền" trên lưới đơn thuốc.
                        decimal unitPriceFromBase = TryGetDecimalProperty(b, "UNIT_PRICE");
                        TrySetProperty(v, "UNIT_PRICE", unitPriceFromBase);
                        if (b.SERVICE_ID.HasValue)
                        {
                            V_HIS_SERVICE svc;
                            if (serviceDict.TryGetValue(b.SERVICE_ID.Value, out svc) && svc != null)
                            {
                                if (string.IsNullOrEmpty(v.SERVICE_NAME)) v.SERVICE_NAME = svc.SERVICE_NAME;
                                v.SERVICE_TYPE_CODE = svc.SERVICE_TYPE_CODE;
                                v.SERVICE_TYPE_NAME = svc.SERVICE_TYPE_NAME;
                                // Set theo cả 2 tên property (DLL cũ: SERVICE_CODE/SERVICE_TYPE_ID; DLL mới: SV_*).
                                // Reflection sẽ no-op trên tên không tồn tại — code chạy đúng với cả 2 phiên bản EFMODEL.
                                TrySetProperty(v, "SV_SERVICE_CODE", svc.SERVICE_CODE);
                                TrySetProperty(v, "SERVICE_CODE", svc.SERVICE_CODE);
                                TrySetProperty(v, "SV_SERVICE_NAME", svc.SERVICE_NAME);
                                TrySetProperty(v, "SV_SERVICE_TYPE_ID", svc.SERVICE_TYPE_ID);
                                TrySetProperty(v, "SERVICE_TYPE_ID", svc.SERVICE_TYPE_ID);
                            }
                        }
                        result.Add(v);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return new List<V_HIS_PATIENT_PACKAGE_DT>();
            }
        }

        /// <summary>Filter cho UC: chỉ cho phép tick chọn dịch vụ là Thuốc / Vật tư.</summary>
        private bool FilterMedicineOrMaterialDetail(V_HIS_PATIENT_PACKAGE_DT detail)
        {
            if (detail == null || !detail.SERVICE_ID.HasValue) return false;
            if (this.packagePickerServiceDict == null) return false;
            V_HIS_SERVICE svc;
            if (!this.packagePickerServiceDict.TryGetValue(detail.SERVICE_ID.Value, out svc) || svc == null) return false;
            return svc.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                || svc.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
        }

        private long GetPatientPackagePatientId()
        {
            try
            {
                if (this.currentTreatmentWithPatientType != null && this.currentTreatmentWithPatientType.PATIENT_ID > 0)
                    return this.currentTreatmentWithPatientType.PATIENT_ID;
                if (this.VHistreatment != null)
                    return this.VHistreatment.PATIENT_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return 0;
        }

        private string BuildPatientPackagePatientInfo()
        {
            try
            {
                if (this.VHistreatment != null)
                {
                    string name = this.VHistreatment.TDL_PATIENT_NAME ?? "";
                    string code = this.VHistreatment.TDL_PATIENT_CODE ?? "";
                    return string.IsNullOrEmpty(code) ? name : (name + " - " + code);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>
        /// Callback từ popup: đẩy danh sách dịch vụ (thuốc/vật tư) trong gói vào lưới đơn thuốc,
        /// kiểm tra và chiếm tồn theo cơ chế dùng chung với "Đơn mẫu / Đơn cũ".
        /// </summary>
        private void OnPatientPackageSelected(object data)
        {
            try
            {
                List<PatientPackageServiceADO> services = data as List<PatientPackageServiceADO>;
                if (services == null || services.Count == 0) return;
                if (this.actionType == GlobalVariables.ActionView)
                {
                    Inventec.Common.Logging.LogSystem.Debug("OnPatientPackageSelected => thao tac khong hop le. actionType = " + this.actionType);
                    return;
                }

                this.lstOutPatientPres = new List<OutPatientPresADO>();
                // Release các thuốc/vật tư đã take bean trước đó nhưng chưa lưu
                this.ReleaseAllMediByUser();
                if (chkShowLo.Checked)
                    chkShowLo.Checked = false;

                // Dựng emte tương đương để tái dùng đúng luồng đơn mẫu
                List<V_HIS_EMTE_MEDICINE_TYPE> emteMedicines = new List<V_HIS_EMTE_MEDICINE_TYPE>();
                List<V_HIS_EMTE_MATERIAL_TYPE> emteMaterials = new List<V_HIS_EMTE_MATERIAL_TYPE>();

                foreach (var service in services)
                {
                    if (service.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var mety = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == service.ServiceId);
                        if (mety == null)
                        {
                            WarningServiceNotInStock(service.ServiceName);
                            continue;
                        }
                        emteMedicines.Add(new V_HIS_EMTE_MEDICINE_TYPE
                        {
                            MEDICINE_TYPE_ID = mety.ID,
                            SERVICE_ID = service.ServiceId,
                            SERVICE_UNIT_ID = mety.SERVICE_UNIT_ID,
                            SERVICE_UNIT_CODE = mety.SERVICE_UNIT_CODE,
                            SERVICE_UNIT_NAME = mety.SERVICE_UNIT_NAME,
                            AMOUNT = service.Amount
                        });
                    }
                    else if (service.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        var maty = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.SERVICE_ID == service.ServiceId);
                        if (maty == null)
                        {
                            WarningServiceNotInStock(service.ServiceName);
                            continue;
                        }
                        emteMaterials.Add(new V_HIS_EMTE_MATERIAL_TYPE
                        {
                            MATERIAL_TYPE_ID = maty.ID,
                            MATERIAL_TYPE_CODE = maty.MATERIAL_TYPE_CODE,
                            MATERIAL_TYPE_NAME = maty.MATERIAL_TYPE_NAME,
                            SERVICE_ID = service.ServiceId,
                            SERVICE_UNIT_ID = maty.SERVICE_UNIT_ID,
                            SERVICE_UNIT_CODE = maty.SERVICE_UNIT_CODE,
                            SERVICE_UNIT_NAME = maty.SERVICE_UNIT_NAME,
                            AMOUNT = service.Amount
                        });
                    }
                }

                if (emteMedicines.Count == 0 && emteMaterials.Count == 0)
                    return;

                int beforeCount = this.mediMatyTypeADOs.Count;
                this.ProcessGetEmteMedcineType(emteMedicines, false);
                this.ProcessGetEmteMaterialType(emteMaterials, false);

                // Gắn thông tin gói bệnh nhân cho các dòng vừa thêm (phục vụ cột hiển thị + lưu liên kết)
                Dictionary<long, PatientPackageServiceADO> serviceDict = new Dictionary<long, PatientPackageServiceADO>();
                foreach (var s in services)
                {
                    if (!serviceDict.ContainsKey(s.ServiceId))
                        serviceDict.Add(s.ServiceId, s);
                }
                // Cache HIS_PATIENT_TYPE để override CODE/NAME khi đổi ĐTTT theo gói.
                Dictionary<long, HIS_PATIENT_TYPE> patientTypeDict = new Dictionary<long, HIS_PATIENT_TYPE>();
                var ptList = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (ptList != null)
                {
                    foreach (var pt in ptList)
                    {
                        if (!patientTypeDict.ContainsKey(pt.ID))
                            patientTypeDict.Add(pt.ID, pt);
                    }
                }

                for (int i = beforeCount; i < this.mediMatyTypeADOs.Count; i++)
                {
                    var ado = this.mediMatyTypeADOs[i];
                    PatientPackageServiceADO match;
                    if (serviceDict.TryGetValue(ado.SERVICE_ID, out match))
                    {
                        // Tag thông tin gói (phục vụ cột hiển thị + lưu liên kết)
                        ado.PatientPackageId = match.PatientPackageId;
                        ado.PatientPackageName = match.PatientPackageName;

                        // Override ĐTTT theo gói (thay vì mặc định BHYT/Thu phí)
                        if (match.PatientTypeId > 0)
                        {
                            ado.PATIENT_TYPE_ID = match.PatientTypeId;
                            HIS_PATIENT_TYPE ptObj;
                            if (patientTypeDict.TryGetValue(match.PatientTypeId, out ptObj) && ptObj != null)
                            {
                                ado.PATIENT_TYPE_CODE = ptObj.PATIENT_TYPE_CODE;
                                ado.PATIENT_TYPE_NAME = ptObj.PATIENT_TYPE_NAME;
                            }
                        }

                        // Override giá theo gói (UNIT_PRICE × SL)
                        if (match.UnitPrice > 0)
                        {
                            ado.PRICE = match.UnitPrice;
                            ado.TotalPrice = (ado.AMOUNT ?? 0) * match.UnitPrice;
                        }
                    }
                }

                if (ProcessCheckOutMediStock(true))
                    return;

                var dataSourceTmp = this.mediMatyTypeADOs;
                this.mediMatyTypeADOs = new List<MediMatyTypeADO>();
                if (this.currentTreatment != null && !string.IsNullOrEmpty(this.currentTreatment.GUARANTEE_CODE))
                    dataSourceTmp.ForEach(o => o.IsGuarantee = this.GetIsPatientHasGuarantee());

                this.ProcessDataMediStock(dataSourceTmp);

                if (!CheckMaterialReusableOrIdentityManager() || !CheckValidMaterial(true) || !CheckMedicineGroupTuberCulosis(true))
                    return;

                this.ProcessInstructionTimeMediForEdit();
                if (this.ProcessCheckAllergenicByPatientAfterChoose()
                    && this.ProcessCheckContraindicaterWarningOptionAfterChoose())
                {
                    this.ProcessMergeDuplicateRowForListProcessing();
                    this.ProcessAddListRowDataIntoGridWithTakeBean();
                }

                // Override LẦN CUỐI sau khi mọi bước xử lý xong (ProcessDataMediStock có copy constructor; take bean có thể recompute giá).
                // Lookup theo SERVICE_ID — chỉ những dòng có trong selection mới override.
                OverridePatientPackageRowsFinal(serviceDict, patientTypeDict);
                this.RefeshResourceGridMedicine();

                // Bật lại nút Lưu sau khi nạp dịch vụ từ gói bệnh nhân (giống RowAdd/Đơn cũ).
                // ActionAdd: btnSave.Enabled = (mediMatyTypeADOs.Count > 0).
                this.SetEnableButtonControl(this.actionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Override ĐTTT + đơn giá lần cuối trên danh sách lưới đơn (sau khi đã chiếm tồn).</summary>
        private void OverridePatientPackageRowsFinal(Dictionary<long, PatientPackageServiceADO> serviceDict, Dictionary<long, HIS_PATIENT_TYPE> patientTypeDict)
        {
            try
            {
                if (this.mediMatyTypeADOs == null || serviceDict == null) return;
                foreach (var ado in this.mediMatyTypeADOs)
                {
                    PatientPackageServiceADO match;
                    if (!serviceDict.TryGetValue(ado.SERVICE_ID, out match)) continue;

                    // Re-tag (đề phòng copy constructor không copy hết)
                    ado.PatientPackageId = match.PatientPackageId;
                    ado.PatientPackageName = match.PatientPackageName;

                    if (match.PatientTypeId > 0)
                    {
                        ado.PATIENT_TYPE_ID = match.PatientTypeId;
                        HIS_PATIENT_TYPE ptObj;
                        if (patientTypeDict != null && patientTypeDict.TryGetValue(match.PatientTypeId, out ptObj) && ptObj != null)
                        {
                            ado.PATIENT_TYPE_CODE = ptObj.PATIENT_TYPE_CODE;
                            ado.PATIENT_TYPE_NAME = ptObj.PATIENT_TYPE_NAME;
                        }
                    }
                    if (match.UnitPrice > 0)
                    {
                        ado.PRICE = match.UnitPrice;
                        ado.TotalPrice = (ado.AMOUNT ?? 0) * match.UnitPrice;
                        Inventec.Common.Logging.LogSystem.Debug(
                            "OverridePatientPackageRowsFinal => serviceId=" + ado.SERVICE_ID
                            + ", AMOUNT=" + ado.AMOUNT
                            + ", PRICE=" + ado.PRICE
                            + ", TotalPrice=" + ado.TotalPrice);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void WarningServiceNotInStock(string serviceName)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn(string.Format(ResourceMessage.DichVuKhongCoTrongKhoDaChon, serviceName));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
