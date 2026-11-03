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
using System.Linq;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

namespace HIS.UC.Hospitalize.Run
{
    /// <summary>
    /// O ma doi tuong KCB (BHYT) tren man hinh nhap vien.
    /// Tu tinh lai theo dien dieu tri dang chon; nguoi dung duoc chon lai trong danh muc.
    /// Gia tri cuoi cung duoc gui len backend de luu vao HIS_TREATMENT.HEIN_PATIENT_TYPE_CODE.
    /// </summary>
    public partial class UCHospitalize
    {
        private V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlterForHein = null;
        private bool isLoadedPatientTypeAlterForHein = false;

        /// <summary>Dang gan gia tri bang code -> khong tinh la nguoi dung sua tay.</summary>
        private bool isSettingHeinPatientTypeCodeByCode = false;

        /// <summary>Nguoi dung da tu chon ma doi tuong KCB -> khong tu dong ghi de nua.</summary>
        private bool isUserEditedHeinPatientTypeCode = false;

        /// <summary>
        /// Nguoi dung tu chon ma doi tuong KCB trong danh muc.
        /// Tu thoi diem nay khong tu dong tinh lai de de len lua chon cua ho nua
        /// (giong cach man Tiep don / Doi tuong dieu tri ton trong ma nguoi dung tu chon).
        /// </summary>
        private void cboHeinPatientTypeCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isSettingHeinPatientTypeCodeByCode) return;
                this.isUserEditedHeinPatientTypeCode = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lay ban ghi doi tuong thanh toan moi nhat cua ho so de biet tuyen (DT/TT),
        /// truong hop (CC/GT/HK/TH) va noi DKKCB ban dau tren the.
        /// Chi goi API 1 lan cho moi lan mo man hinh.
        /// </summary>
        private void LoadPatientTypeAlterForHein()
        {
            try
            {
                if (this.isLoadedPatientTypeAlterForHein) return;
                this.isLoadedPatientTypeAlterForHein = true;

                if (this.hospitalizeInitADO == null || !this.hospitalizeInitADO.TreatmentId.HasValue
                    || this.hospitalizeInitADO.TreatmentId.Value <= 0)
                    return;

                CommonParam param = new CommonParam();
                this.currentPatientTypeAlterForHein = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>(
                    HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_LAST_BY_TREATMENTID,
                    ApiConsumers.MosConsumer,
                    this.hospitalizeInitADO.TreatmentId.Value,
                    param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ban ghi CO liet ke ma co so KCB ban dau cu the (khac rong va khac "*")
        /// => chi ap dung cho dung nhung ma da liet ke.
        /// De trong (NULL) hoac "*" => ap dung MOI co so, giong quy uoc cua truong Dien dieu tri.
        /// </summary>
        private bool IsMediOrgCodeListSpecific(string codes)
        {
            return !string.IsNullOrWhiteSpace(codes) && codes.Trim() != "*";
        }

        /// <summary>
        /// Chon ban ghi doi tuong KCB, co xet them tieu chi ma co so KCB ban dau (noi DKKCB tren the):
        /// 1. Ban ghi liet ke ma co so cu the va chua ma DKKCB cua the -> uu tien cao nhat
        /// 2. Ban ghi de trong hoac "*" (ap dung moi co so)
        /// Cung muc -> NUM_ORDER nho nhat (null xep cuoi) -> ID lon nhat.
        /// Logic giong het man Tiep don (UCHeinInfo) va Doi tuong dieu tri (TemplateHeinBHYT1).
        /// </summary>
        private HIS_HEIN_PATIENT_TYPE GetHeinPatientTypeByMediOrgPriority(List<HIS_HEIN_PATIENT_TYPE> heinList, string dkbdCode)
        {
            try
            {
                var specific = new List<HIS_HEIN_PATIENT_TYPE>();
                if (!string.IsNullOrEmpty(dkbdCode))
                {
                    specific = heinList.Where(o => IsMediOrgCodeListSpecific(o.HEIN_MEDI_ORG_CODES)
                        && o.HEIN_MEDI_ORG_CODES.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Any(c => c.Trim() == dkbdCode)).ToList();
                }
                var applyAll = heinList.Where(o => !IsMediOrgCodeListSpecific(o.HEIN_MEDI_ORG_CODES)).ToList();

                var matchedList = specific.Count > 0 ? specific : applyAll;
                if (matchedList.Count == 0) return null;

                return matchedList.OrderBy(o => o.NUM_ORDER == null ? 1 : 0)
                                  .ThenBy(o => o.NUM_ORDER)
                                  .ThenByDescending(o => o.ID)
                                  .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Gan gia tri vao o ma doi tuong KCB bang code, co dat co de
        /// EditValueChanged khong hieu nham la nguoi dung tu chon.
        /// </summary>
        private void SetHeinPatientTypeCodeText(string code)
        {
            try
            {
                this.isSettingHeinPatientTypeCodeByCode = true;
                cboHeinPatientTypeCode.EditValue = code;
            }
            finally
            {
                this.isSettingHeinPatientTypeCodeByCode = false;
            }
        }

        /// <summary>
        /// Gan ma doi tuong KCB vao SDO gui len backend.
        /// Dung reflection de van chay duoc voi ban MOS.SDO cu chua co truong nay
        /// (khi do bo qua, backend giu nguyen ma cu - dung hanh vi truoc nang cap).
        /// </summary>
        private void SetHeinPatientTypeCodeIfSupported(MOS.SDO.HisDepartmentTranHospitalizeSDO sdo, string code)
        {
            try
            {
                if (sdo == null) return;
                var property = sdo.GetType().GetProperty("HeinPatientTypeCode");
                if (property != null && property.CanWrite)
                {
                    property.SetValue(sdo, code, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nap danh muc Doi tuong KCB dang hoat dong vao combo de nguoi dung chon lai.
        /// Chi cho chon ma co that trong danh muc -> khong the go bua ma sai vao du lieu BHYT.
        /// </summary>
        private void LoadDataToComboHeinPatientTypeCode()
        {
            try
            {
                var heinData = BackendDataWorker.Get<HIS_HEIN_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.HEIN_PATIENT_TYPE_CODE)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_PATIENT_TYPE_CODE", "", 60, 1));
                columnInfos.Add(new ColumnInfo("DESCRIPTION", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_PATIENT_TYPE_CODE", "HEIN_PATIENT_TYPE_CODE", columnInfos, false, 360);
                ControlEditorLoader.Load(cboHeinPatientTypeCode, heinData, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Ma doi tuong KCB nguoi dung xac nhan tren man hinh, gui len de luu vao ho so.
        /// </summary>
        private string GetHeinPatientTypeCodeValue()
        {
            try
            {
                if (cboHeinPatientTypeCode == null || cboHeinPatientTypeCode.EditValue == null) return null;
                string code = cboHeinPatientTypeCode.EditValue.ToString().Trim();
                return string.IsNullOrEmpty(code) ? null : code;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        /// <summary>
        /// Tinh lai ma doi tuong KCB theo dien dieu tri dang chon tren man hinh va do len o nhap.
        /// Duoc goi khi load man hinh va moi khi nguoi dung doi Dien dieu tri.
        /// </summary>
        private void RefreshHeinPatientTypeCode()
        {
            try
            {
                if (cboHeinPatientTypeCode == null) return;

                // Nguoi dung da tu sua tay -> ton trong gia tri ho nhap, khong ghi de.
                // Rieng key = 1: van tra lai va HOI (chon Khong la giu ma chon tay) nen khong can chan.
                if (this.isUserEditedHeinPatientTypeCode && !Config.HisConfig.IsConfirmChangeHeinPatientTypeCode) return;

                LoadPatientTypeAlterForHein();
                var pta = this.currentPatientTypeAlterForHein;

                if (Config.HisConfig.IsHeinPatientTypeCodeLookup)
                {
                    RefreshHeinPatientTypeCodeByLookupOption(pta);
                    return;
                }

                // Key khac 1, 2: tra kieu cu (tuyen + truong hop cua ho so trung khit).
                // Khong phai BHYT (khong co tuyen) -> khong tra; giu nguyen ma dang hien (khong xoa trang o)
                if (pta == null || string.IsNullOrWhiteSpace(pta.RIGHT_ROUTE_CODE))
                    return;

                long treatmentTypeId = 0;
                if (cboTreatmentType.EditValue != null)
                    treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString());

                var heinList = BackendDataWorker.Get<HIS_HEIN_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Where(o => o.RIGHT_ROUTE_CODE == pta.RIGHT_ROUTE_CODE)
                    .Where(o => o.RIGHT_ROUTE_TYPE_CODE == pta.RIGHT_ROUTE_TYPE_CODE)
                    .ToList();

                if (treatmentTypeId > 0)
                {
                    heinList = heinList.Where(o =>
                        string.IsNullOrEmpty(o.TREATMENT_TYPE_IDS) ||
                        o.TREATMENT_TYPE_IDS.Split(',').Select(s => s.Trim()).Any(id => id == treatmentTypeId.ToString())
                    ).ToList();
                }

                var matched = GetHeinPatientTypeByMediOrgPriority(heinList, (pta.HEIN_MEDI_ORG_CODE ?? "").Trim());
                // Khong khop dong nao => giu nguyen ma dang hien tren o (VD ma lay tu ho so luc mo form), khong xoa trang
                if (matched != null)
                    SetHeinPatientTypeCodeText(matched.HEIN_PATIENT_TYPE_CODE);

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "HienThiMaDoiTuongKCB_NhapVien: treatmentId={0}___tuyen={1}___truongHop={2}___dienDieuTri={3}___maDKKCBBanDau={4}___soBanGhiSauLoc={5}___maHienThi={6}",
                    this.hospitalizeInitADO != null && this.hospitalizeInitADO.TreatmentId.HasValue ? this.hospitalizeInitADO.TreatmentId.Value : 0,
                    pta.RIGHT_ROUTE_CODE, pta.RIGHT_ROUTE_TYPE_CODE, treatmentTypeId,
                    pta.HEIN_MEDI_ORG_CODE, heinList.Count, GetHeinPatientTypeCodeValue()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Key HIS.Desktop.Plugins.Hospitalize.HeinPatientTypeCodeLookupOption = 1 hoac 2:
        /// tra danh muc chi theo Cap cuu + Dien dieu tri + ma CSKCB ban dau, bo qua Tuyen.
        /// Cap cuu lay tu database (Truong hop = Cap cuu tren dong doi tuong moi nhat cua ho so), khong xet o Cap cuu tren man hinh.
        /// </summary>
        private void RefreshHeinPatientTypeCodeByLookupOption(V_HIS_PATIENT_TYPE_ALTER pta)
        {
            try
            {
                // Khong phai doi tuong BHYT -> khong tra; giu nguyen ma dang hien (khong xoa trang o)
                if (pta == null || !IsHeinPatientTypeAlter(pta))
                    return;

                long treatmentTypeId = 0;
                if (cboTreatmentType.EditValue != null)
                    treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString());

                bool isEmergency = IsEmergencyPatientTypeAlter(pta);
                var matched = FindHeinPatientTypeStandard(pta, treatmentTypeId, isEmergency);
                string newCode = matched != null ? matched.HEIN_PATIENT_TYPE_CODE : null;
                if (Config.HisConfig.IsConfirmChangeHeinPatientTypeCode)
                    // Key = 1: ma doi thi hoi nguoi dung ngay luc tu dien
                    ApplyHeinPatientTypeCodeWithConfirm(newCode, treatmentTypeId);
                else if (!string.IsNullOrEmpty(newCode))
                    // Key = 2: tu dien, khong hoi. Khong khop dong nao => giu nguyen ma dang hien (khong xoa trang o)
                    SetHeinPatientTypeCodeText(newCode);

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "HienThiMaDoiTuongKCB_NhapVien_TheoKey: treatmentId={0}___dienDieuTri={1}___maDKKCBBanDau={2}___truongHop={3}___capCuu={4}___maHienThi={5}",
                    this.hospitalizeInitADO != null && this.hospitalizeInitADO.TreatmentId.HasValue ? this.hospitalizeInitADO.TreatmentId.Value : 0,
                    treatmentTypeId, pta.HEIN_MEDI_ORG_CODE, pta.RIGHT_ROUTE_TYPE_CODE, isEmergency, GetHeinPatientTypeCodeValue()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Ma dang cho nguoi dung xac nhan (tranh hoi chong khi doi dien lien tuc).</summary>
        private string pendingConfirmHeinPatientTypeCode = null;

        /// <summary>
        /// Key = 1: ma tu tra ra khac ma hien tai thi hoi "Ban co muon chuyen ma DT KCB tu X sang Y khong?" ngay luc tu dien.
        /// Ma hien tai = gia tri dang o o ma; o ma trong (moi mo form) thi lay ma dang luu tren ho so.
        /// Co => doi sang ma moi. Khong => giu ma cu (o ma trong thi dien ma cua ho so).
        /// Moi lan doi Dien ma ma khac deu hoi lai, ke ca da chon Khong truoc do.
        /// Chua chon Dien dieu tri / khong co dong danh muc phu hop => khong doi, khong hoi.
        /// </summary>
        private void ApplyHeinPatientTypeCodeWithConfirm(string newCode, long treatmentTypeId)
        {
            try
            {
                if (treatmentTypeId <= 0) return;
                if (string.IsNullOrEmpty(newCode)) return;

                string current = GetHeinPatientTypeCodeValue();
                string oldCode = !string.IsNullOrEmpty(current) ? current : GetTreatmentHeinPatientTypeCode();
                if (newCode == oldCode)
                {
                    SetHeinPatientTypeCodeText(newCode);
                    return;
                }

                this.pendingConfirmHeinPatientTypeCode = newCode;
                // BeginInvoke: hoi sau khi su kien doi dien chay xong, tranh mo hop thoai giua su kien cua control
                if (this.IsHandleCreated)
                    this.BeginInvoke(new Action(() => AskChangeHeinPatientTypeCode(oldCode, newCode)));
                else
                    AskChangeHeinPatientTypeCode(oldCode, newCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AskChangeHeinPatientTypeCode(string oldCode, string newCode)
        {
            try
            {
                // Da co lan tra moi hon (nguoi dung doi dien tiep) => bo lan hoi nay
                if (this.pendingConfirmHeinPatientTypeCode != newCode) return;
                this.pendingConfirmHeinPatientTypeCode = null;

                string message = string.IsNullOrEmpty(oldCode)
                    ? string.Format("Bạn có muốn chuyển mã ĐT KCB sang {0} không?", newCode)
                    : string.Format("Bạn có muốn chuyển mã ĐT KCB từ {0} sang {1} không?", oldCode, newCode);
                bool isYes = DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo",
                    System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes;

                if (isYes)
                {
                    SetHeinPatientTypeCodeText(newCode);
                }
                else
                {
                    if (string.IsNullOrEmpty(GetHeinPatientTypeCodeValue()) && !string.IsNullOrEmpty(oldCode))
                        SetHeinPatientTypeCodeText(oldCode);
                }

                Inventec.Common.Logging.LogSystem.Info(string.Format(
                    "XacNhanDoiMaDoiTuongKCB_NhapVien: treatmentId={0}___maCu={1}___maMoi={2}___dongY={3}",
                    this.hospitalizeInitADO != null && this.hospitalizeInitADO.TreatmentId.HasValue ? this.hospitalizeInitADO.TreatmentId.Value : 0,
                    oldCode, newCode, isYes));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mo form nhap vien (moi gia tri key): o ma lay san ma dang luu tren ho so dieu tri.
        /// Chi tu tra lai danh muc khi Dien dieu tri da duoc chon san; con lai cho nguoi dung doi Dien.
        /// </summary>
        private void InitHeinPatientTypeCodeOnLoad()
        {
            try
            {
                if (cboHeinPatientTypeCode == null) return;
                string treatmentCode = GetTreatmentHeinPatientTypeCode();
                SetHeinPatientTypeCodeText(string.IsNullOrEmpty(treatmentCode) ? null : treatmentCode);

                long treatmentTypeId = 0;
                if (cboTreatmentType.EditValue != null)
                    treatmentTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((cboTreatmentType.EditValue ?? "").ToString());
                if (treatmentTypeId > 0)
                    RefreshHeinPatientTypeCode();

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "MaDoiTuongKCB_NhapVien_MoForm: treatmentId={0}___maHoSo={1}___dienDieuTri={2}___maHienThi={3}",
                    this.hospitalizeInitADO != null && this.hospitalizeInitADO.TreatmentId.HasValue ? this.hospitalizeInitADO.TreatmentId.Value : 0,
                    treatmentCode, treatmentTypeId, GetHeinPatientTypeCodeValue()));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetTreatmentHeinPatientTypeCode()
        {
            return (this.hospitalizeInitADO != null && this.hospitalizeInitADO.Treatment != null)
                ? (this.hospitalizeInitADO.Treatment.HEIN_PATIENT_TYPE_CODE ?? "").Trim()
                : "";
        }

        /// <summary>Ma truong hop Cap cuu (RIGHT_ROUTE_TYPE_CODE) trong danh muc Doi tuong KCB.</summary>
        private const string RIGHT_ROUTE_TYPE_CODE__EMERGENCY = "CC";

        /// <summary>
        /// Cap cuu theo database: Truong hop (RIGHT_ROUTE_TYPE_CODE) cua dong doi tuong moi nhat tren Dong thoi gian = Cap cuu.
        /// Khong xet o Cap cuu tren man hinh nhap vien.
        /// </summary>
        private bool IsEmergencyPatientTypeAlter(V_HIS_PATIENT_TYPE_ALTER pta)
        {
            return pta != null && (pta.RIGHT_ROUTE_TYPE_CODE ?? "").Trim() == RIGHT_ROUTE_TYPE_CODE__EMERGENCY;
        }

        /// <summary>
        /// Tra theo danh muc Doi tuong KCB khi key HIS.Desktop.Plugins.Hospitalize.HeinPatientTypeCodeLookupOption = 1 hoac 2.
        /// CHI xet 3 thu: Cap cuu (theo database) + Dien dieu tri + ma CSKCB ban dau. BO QUA Tuyen
        /// (cot Tuyen tren danh muc cung khong xet).
        /// Nguyen tac: cot de trong tren dong danh muc = ap dung moi gia tri; khai cu the thi phai khop.
        ///  - Truong hop: ho so Cap cuu => dong Truong hop = Cap cuu; khong => chi dong de trong Truong hop.
        ///  - Dien dieu tri: dong trong, hoac chua dien dang chon.
        ///  - Ma CSKCB ban dau: dong trong / "*", hoac liet ke dung ma DKKCB cua the.
        /// Nhieu dong cung khop => dong khai cu the hon thang, thu tu: Truong hop -> ma CSKCB -> Dien dieu tri
        ///  -> NUM_ORDER nho nhat (null xep cuoi) -> ID lon nhat.
        ///  (Truong hop xep dau de dong Cap cuu thang dong ghi dich danh ma CSKCB: cap cuu => ma cap cuu voi moi the.)
        /// Moi ma deu lay tu danh muc, khong co ma nao co dinh trong code.
        /// </summary>
        private HIS_HEIN_PATIENT_TYPE FindHeinPatientTypeStandard(V_HIS_PATIENT_TYPE_ALTER pta, long treatmentTypeId, bool isEmergency)
        {
            try
            {
                string rightRouteTypeCode = isEmergency ? RIGHT_ROUTE_TYPE_CODE__EMERGENCY : "";
                string dkbdCode = (pta.HEIN_MEDI_ORG_CODE ?? "").Trim();
                string treatmentTypeIdStr = treatmentTypeId.ToString();

                Func<HIS_HEIN_PATIENT_TYPE, bool> isRouteTypeSpecific = o => !string.IsNullOrWhiteSpace(o.RIGHT_ROUTE_TYPE_CODE);
                Func<HIS_HEIN_PATIENT_TYPE, bool> isTreatmentTypeSpecific = o => !string.IsNullOrWhiteSpace(o.TREATMENT_TYPE_IDS);
                Func<HIS_HEIN_PATIENT_TYPE, bool> isMediOrgSpecific = o => IsMediOrgCodeListSpecific(o.HEIN_MEDI_ORG_CODES);

                var matchedList = BackendDataWorker.Get<HIS_HEIN_PATIENT_TYPE>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Where(o => !isRouteTypeSpecific(o) || o.RIGHT_ROUTE_TYPE_CODE.Trim() == rightRouteTypeCode)
                    .Where(o => !isTreatmentTypeSpecific(o) || treatmentTypeId <= 0
                        || o.TREATMENT_TYPE_IDS.Split(',').Select(s => s.Trim()).Any(id => id == treatmentTypeIdStr))
                    .Where(o => !isMediOrgSpecific(o)
                        || (!string.IsNullOrEmpty(dkbdCode)
                            && o.HEIN_MEDI_ORG_CODES.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Any(c => c.Trim() == dkbdCode)))
                    .ToList();

                if (matchedList.Count == 0) return null;

                return matchedList
                    .OrderByDescending(o => isRouteTypeSpecific(o) ? 1 : 0)
                    .ThenByDescending(o => isMediOrgSpecific(o) ? 1 : 0)
                    .ThenByDescending(o => isTreatmentTypeSpecific(o) ? 1 : 0)
                    .ThenBy(o => o.NUM_ORDER == null ? 1 : 0)
                    .ThenBy(o => o.NUM_ORDER)
                    .ThenByDescending(o => o.ID)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private long patientTypeIdBhyt = 0;

        /// <summary>
        /// Dong doi tuong gan nhat cua ho so co phai doi tuong BHYT khong
        /// (ma doi tuong thanh toan BHYT lay tu key MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT, giong UC Ket thuc dieu tri).
        /// </summary>
        private bool IsHeinPatientTypeAlter(V_HIS_PATIENT_TYPE_ALTER pta)
        {
            try
            {
                if (this.patientTypeIdBhyt == 0)
                {
                    string code = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                    if (patientType != null) this.patientTypeIdBhyt = patientType.ID;
                }
                // Khong doc duoc cau hinh -> fallback theo so the de khong mat chuc nang
                if (this.patientTypeIdBhyt == 0)
                    return !string.IsNullOrWhiteSpace(pta.HEIN_CARD_NUMBER);
                return pta.PATIENT_TYPE_ID == this.patientTypeIdBhyt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return !string.IsNullOrWhiteSpace(pta.HEIN_CARD_NUMBER);
            }
        }


        /// <summary>
        /// Truoc day: hoi xac nhan doi ma luc Luu. Nay key = 1 hoi ngay luc tu dien ma (ApplyHeinPatientTypeCodeWithConfirm)
        /// nen luc Luu khong hoi nua. Giu ham de plugin Xu ly kham goi khong loi.
        /// </summary>
        public bool ConfirmChangeHeinPatientTypeCode()
        {
            return true;
        }

        /// <summary>Dua con tro ve o ma doi tuong KCB (sau khi nguoi dung chon Khong o thong bao xac nhan).</summary>
        public void FocusHeinPatientTypeCode()
        {
            try
            {
                if (cboHeinPatientTypeCode == null) return;
                // BeginInvoke: doi luong luu ket thuc roi moi focus, tranh bi control khac lay lai focus
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        cboHeinPatientTypeCode.Focus();
                        cboHeinPatientTypeCode.SelectAll();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
