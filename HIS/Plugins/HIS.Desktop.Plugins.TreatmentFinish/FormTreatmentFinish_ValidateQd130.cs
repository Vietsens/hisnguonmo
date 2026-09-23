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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.TreatmentFinish.Config;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    /// <summary>
    /// Kiem tra lai day du thong tin kham benh ngoai tru theo QD130 truoc khi ket thuc dieu tri.
    /// Chi kich hoat khi key HIS.DESKTOP.EXAM.REQUIRED_FIELDS_QD130 = 1
    /// VA ho so co loai dieu tri Kham. Khac di thi thoat som, khong phat sinh loi goi lay du lieu.
    /// </summary>
    public partial class FormTreatmentFinish : HIS.Desktop.Utility.FormBase
    {
        private bool IsApplyQd130()
        {
            try
            {
                return ConfigKey.IsRequiredFieldsQd130
                    && this.currentHisTreatment != null
                    && this.currentHisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// To do nhan cac truong bat buoc co mat tren man nay.
        /// </summary>
        private void ApplyQd130Appearance()
        {
            try
            {
                if (!IsApplyQd130()) return;

                SetQd130CaptionRequired(lciClinical);
                SetQd130CaptionRequired(lciMethod);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetQd130CaptionRequired(DevExpress.XtraLayout.LayoutControlItem item)
        {
            try
            {
                if (item == null) return;
                item.AppearanceItemCaption.ForeColor = Color.Maroon;
                item.AppearanceItemCaption.Options.UseForeColor = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Thieu thi gom tat ca vao MOT thong bao va chan ket thuc dieu tri.
        /// Cac truong khong hien thi tren man nay duoc kiem tra tren du lieu da luu cua phieu kham.
        /// </summary>
        private bool CheckQd130RequiredFields()
        {
            try
            {
                if (!IsApplyQd130()) return true;

                List<string> missing = new List<string>();

                // Hai truong co o nhap ngay tren man nay
                if (String.IsNullOrWhiteSpace(txtDauHieuLamSang.Text))
                {
                    missing.Add("Quá trình bệnh lý");
                }
                if (String.IsNullOrWhiteSpace(txtMethod.Text))
                {
                    missing.Add("Phương pháp điều trị");
                }
                if (cboTreatmentEndType.EditValue == null)
                {
                    missing.Add("Loại ra viện");
                }
                if (cboResult.EditValue == null)
                {
                    missing.Add("Kết quả điều trị");
                }

                // Sau nhom con lai kiem tra tren du lieu da luu cua phieu kham
                missing.AddRange(GetQd130MissingFromExamServiceReqs());
                missing.AddRange(GetQd130MissingFromDhst());

                if (missing.Count == 0) return true;

                XtraMessageBox.Show(
                    "Hồ sơ khám chưa đầy đủ thông tin bắt buộc: " + String.Join(", ", missing)
                        + ". Vui lòng vào màn hình Xử lý khám để bổ sung trước khi kết thúc điều trị.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return true;
            }
        }

        /// <summary>
        /// Kiem tra TOAN BO phieu kham cua dot dieu tri. Chi can mot phieu con thieu la bao.
        /// Khi dot dieu tri co tu hai phieu kham tro len thi neu kem ma phieu.
        /// </summary>
        private List<string> GetQd130MissingFromExamServiceReqs()
        {
            List<string> missing = new List<string>();
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqFilter srFilter = new HisServiceReqFilter();
                srFilter.TREATMENT_ID = this.treatmentId;
                srFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                srFilter.ORDER_DIRECTION = "ASC";
                srFilter.ORDER_FIELD = "INTRUCTION_TIME";
                List<HIS_SERVICE_REQ> examServiceReqs = new BackendAdapter(param)
                    .Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, param);

                if (examServiceReqs == null || examServiceReqs.Count == 0)
                {
                    return missing;
                }

                bool isManyExam = examServiceReqs.Count > 1;
                foreach (HIS_SERVICE_REQ item in examServiceReqs)
                {
                    string suffix = isManyExam ? String.Format(" (phiếu {0})", item.SERVICE_REQ_CODE) : "";

                    if (String.IsNullOrWhiteSpace(item.HOSPITALIZATION_REASON))
                        missing.Add("Lý do khám" + suffix);
                    if (String.IsNullOrWhiteSpace(item.PATHOLOGICAL_HISTORY))
                        missing.Add("Tiền sử bệnh" + suffix);
                    if (String.IsNullOrWhiteSpace(item.FULL_EXAM))
                        missing.Add("Khám toàn thân" + suffix);
                    if (String.IsNullOrWhiteSpace(item.PART_EXAM))
                        missing.Add("Khám bộ phận" + suffix);
                    if (String.IsNullOrWhiteSpace(item.ICD_CODE))
                        missing.Add("Chẩn đoán chính" + suffix);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return missing;
        }

        /// <summary>
        /// Dau hieu sinh ton co ban - 8 chi so thuoc tab Co ban cua man Xu ly kham.
        /// Du dieu kien khi ton tai it nhat mot ban ghi dau hieu sinh ton cua dot dieu tri
        /// co day du ca 8 chi so.
        /// </summary>
        private List<string> GetQd130MissingFromDhst()
        {
            List<string> missing = new List<string>();
            try
            {
                CommonParam param = new CommonParam();
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = this.treatmentId;
                List<HIS_DHST> dhsts = new BackendAdapter(param)
                    .Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, dhstFilter, param);

                if (dhsts != null && dhsts.Any(o => IsQd130DhstFull(o)))
                {
                    return missing;
                }

                missing.Add("Dấu hiệu sinh tồn cơ bản");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return missing;
        }

        private bool IsQd130DhstFull(HIS_DHST dhst)
        {
            if (dhst == null) return false;
            return dhst.EXECUTE_TIME.HasValue
                && dhst.PULSE.HasValue
                && dhst.BREATH_RATE.HasValue
                && dhst.BLOOD_PRESSURE_MAX.HasValue
                && dhst.BLOOD_PRESSURE_MIN.HasValue
                && dhst.TEMPERATURE.HasValue
                && dhst.HEIGHT.HasValue
                && dhst.WEIGHT.HasValue;
        }
    }
}
