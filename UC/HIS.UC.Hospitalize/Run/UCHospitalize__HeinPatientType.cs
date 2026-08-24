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

                // Nguoi dung da tu sua tay -> ton trong gia tri ho nhap, khong ghi de
                if (this.isUserEditedHeinPatientTypeCode) return;

                LoadPatientTypeAlterForHein();
                var pta = this.currentPatientTypeAlterForHein;

                // Khong phai BHYT (khong co tuyen) -> khong co ma doi tuong KCB
                if (pta == null || string.IsNullOrWhiteSpace(pta.RIGHT_ROUTE_CODE))
                {
                    SetHeinPatientTypeCodeText(null);
                    return;
                }

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
                SetHeinPatientTypeCodeText(matched != null ? matched.HEIN_PATIENT_TYPE_CODE : null);

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
    }
}
