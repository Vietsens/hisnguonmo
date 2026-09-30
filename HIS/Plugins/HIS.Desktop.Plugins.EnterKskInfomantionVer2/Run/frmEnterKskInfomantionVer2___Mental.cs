/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Ksk tâm thần" — Giấy khám sức khỏe tâm thần, Mẫu số 04 (Phụ lục XXIV, TT 25/2026/TT-BYT).
 * Xem _Software-Specs/02_Analysis_Design/PTTK_TBD_Kham_Suc_Khoe_Tam_Than_Mau_04_TT25.md.
 *
 * Lưu / nạp:
 *  - Dữ liệu khám (mục II–V) -> HIS_KSK_MENTAL, thể lực -> HIS_DHST: gửi qua nhánh KskMental của
 *    api/HisServiceReq/KskExecuteV2; nạp lại từ api/HisKskSync/GetKskData (SDO.HisKskMentals).
 *  - Bác sĩ khám + ngày kết luận -> HIS_KSK_GENERAL của lượt khám (như các tab khác).
 *  - Lý do khám dùng ô chung của form.
 * In: mẫu Mps000520 "Giấy khám sức khỏe tâm thần" (LoadBieuMauPhieuMps000520) — cần bản ghi
 *  SAR_PRINT_TYPE 'Mps000520' + file mẫu trong Tmp\Mps\Mps000520\.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Chỉ số tab "Ksk tâm thần" (tab cuối, sau "Trẻ em dưới 6 tuổi").</summary>
        private const int TAB_MENTAL = 8;

        private bool isMentalTabInited = false;

        /// <summary>Hồ sơ khám tâm thần tải sẵn theo y lệnh (api/HisKskSync/GetKskData).</summary>
        private List<HIS_KSK_MENTAL> preKskMentals;
        /// <summary>Hồ sơ khám tâm thần đang hiển thị / vừa lưu (null = chưa có).</summary>
        private HIS_KSK_MENTAL currentKskMentalEf;
        /// <summary>Sinh hiệu (mục III.1) của hồ sơ đang hiển thị — giữ ID để lần Lưu sau UPDATE, không tạo mới.</summary>
        private HIS_DHST dhstMental;

        /// <summary>
        /// Dựng dữ liệu in Giấy khám sức khỏe tâm thần (Mps000520). Ưu tiên bản ĐÃ LƯU (giống Mps000516);
        /// chưa lưu thì dựng từ các ô trên tab. Bác sĩ khám + ngày kết luận lấy theo ô đang hiển thị.
        /// </summary>
        private void LoadBieuMauPhieuMps000520(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
                Inventec.Desktop.Common.Message.WaitingManager.Show();

                // Hồ sơ điều trị (thông tin hành chính mục I)
                V_HIS_TREATMENT_4 treatment = null;
                if (currentServiceReq != null && currentServiceReq.TREATMENT_ID > 0)
                {
                    var tFilter = new MOS.Filter.HisTreatmentView4Filter { ID = currentServiceReq.TREATMENT_ID };
                    var ts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>(
                        "api/HisTreatment/GetView4", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, tFilter, null);
                    treatment = ts != null ? ts.FirstOrDefault() : null;
                }

                MOS.SDO.KskMental2SDO fromForm = BuildKskMentalSdo();
                HIS_KSK_MENTAL kskMental = currentKskMentalEf ?? fromForm.HisKskMental;

                // Thể lực: bản đã lưu theo DHST_ID, chưa có thì lấy các ô trên form
                HIS_DHST dhst = null;
                if (kskMental.DHST_ID != null && kskMental.DHST_ID > 0)
                {
                    List<HIS_DHST> ds = PreGetDhst(kskMental.DHST_ID);
                    dhst = (ds != null && ds.Count > 0) ? ds[0] : null;
                    if (dhst == null)
                    {
                        var dFilter = new MOS.Filter.HisDhstFilter { ID = kskMental.DHST_ID };
                        var dl = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(
                            "api/HisDhst/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, dFilter, param);
                        dhst = dl != null ? dl.FirstOrDefault() : null;
                    }
                }
                if (dhst == null) dhst = fromForm.HisDhst;

                // Bác sĩ khám + ngày kết luận theo ô đang hiển thị (bản sao, không chạm currentKskGeneral)
                HIS_KSK_GENERAL general = BuildKskGeneralForMental();
                object login = this.ucKskMental.CboConcluder.EditValue;
                general.CONCLUDER_LOGINNAME = login != null && !string.IsNullOrWhiteSpace(login.ToString()) ? login.ToString() : null;
                general.CONCLUDER_USERNAME = !string.IsNullOrEmpty(general.CONCLUDER_LOGINNAME)
                    ? HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                        .Where(o => o.LOGINNAME == general.CONCLUDER_LOGINNAME).Select(o => o.TDL_USERNAME).FirstOrDefault()
                    : null;
                if (this.ucKskMental.DteConclusionTime.EditValue != null)
                    general.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.ucKskMental.DteConclusionTime.DateTime);

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                MPS.Processor.Mps000520.PDO.Mps000520PDO rdo = new MPS.Processor.Mps000520.PDO.Mps000520PDO(
                    kskMental, currentServiceReq, dhst, general, treatment);
                rdo.KskServiceReq = printKskServiceReq;
                rdo.KskPatient = printKskPatient;
                rdo.KskPatientTypesName = MapKskNames(BuildKskObjectList(), kskMental.KSK_PATIENT_TYPES);
                rdo.KskPaySourceName = MapKskNames(BuildKskPaymentSourceList(),
                    kskMental.KSK_PAY_SOURCE.HasValue ? kskMental.KSK_PAY_SOURCE.Value.ToString() : null);
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        /// <summary>Đổi chuỗi mã ("1;3;13") sang tên, ngăn bởi dấu phẩy.</summary>
        private static string MapKskNames(List<KskCodeNameADO> list, string codes)
        {
            if (list == null || string.IsNullOrWhiteSpace(codes)) return "";
            var names = codes.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(c => list.FirstOrDefault(o => o.ID.ToString() == c.Trim()))
                .Where(o => o != null).Select(o => o.NAME).ToArray();
            return string.Join(", ", names);
        }

        /// <summary>Dựng 1 lần: danh mục Đối tượng/Nguồn chi trả, Bác sĩ khám, danh mục ICD cho V.1.</summary>
        private void InitTabMental()
        {
            if (isMentalTabInited) return;
            isMentalTabInited = true;
            try
            {
                // V.1 chọn ICD-10 bằng cụm chọn bệnh (popup chọn nhiều) — cùng danh mục ICD với cụm tiền sử
                // của các tab khác (nạp ở Load — InitKskHistoryIcdForTabs). Không dùng khối "Kết luận theo
                // bệnh" 3 lựa chọn của các tab khác: Mẫu 04 hỏi Có/Không, "Có" thì ghi mã bệnh ICD-10.
                this.ucKskMental.InitUc(this.historyIcdDataSource, this.historyIcdPageSize);
                InitAdminCombosExt(this.ucKskMental.CboObject, this.ucKskMental.CboPaymentSource);
                // Bác sĩ khám = người kết luận của lượt khám (HIS_KSK_GENERAL) — đồng bộ với các tab khác.
                RegisterConcluderCombo(TAB_MENTAL, this.ucKskMental.CboConcluder);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nạp tab khi mở lần đầu / đổi y lệnh — gọi từ EnsureTabLoaded.</summary>
        private void LoadTabMental()
        {
            try
            {
                InitTabMental();
                ClearTabInputEditors(TAB_MENTAL);
                // Combo chọn nhiều không bị ClearTabInputEditors xóa — xóa riêng.
                SetObjectValueExt(this.ucKskMental.CboObject, null);
                this.ucKskMental.CboPaymentSource.EditValue = null;
                this.ucKskMental.RefreshState();
                LoadAdministrativeInfoMental();
                FillDataMental();
                LoadConcluderComboExt();
                DateTime conclusionTime = DateTime.Now;
                if (currentKskGeneral != null && currentKskGeneral.CONCLUSION_TIME != null && currentKskGeneral.CONCLUSION_TIME > 0)
                    conclusionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.CONCLUSION_TIME.Value) ?? DateTime.Now;
                this.ucKskMental.DteConclusionTime.DateTime = conclusionTime;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ hồ sơ khám tâm thần của y lệnh lên tab. Nguồn: bản vừa lưu (current) trước, bản tải sẵn
        /// (pre) sau. Chưa có hồ sơ -> để trống, thể lực lấy sẵn từ sinh hiệu của y lệnh (chỉ lấy giá trị,
        /// không lấy ID: lưu lần đầu tạo sinh hiệu riêng cho hồ sơ, không sửa bản ghi của màn khác).
        /// </summary>
        private void FillDataMental()
        {
            try
            {
                if (currentKskMentalEf == null && preKskMentals != null && preKskMentals.Count > 0)
                    currentKskMentalEf = preKskMentals[0];

                dhstMental = null;
                HIS_DHST dhstShow = null;
                if (currentKskMentalEf != null)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentKskMentalEf.DHST_ID);
                    dhstMental = (ds != null && ds.Count > 0) ? ds[0] : null;
                    dhstShow = dhstMental;
                }
                else if (currentServiceReq != null)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentServiceReq.DHST_ID);
                    dhstShow = (ds != null && ds.Count > 0) ? ds[0] : null;
                }

                this.ucKskMental.FillData(currentKskMentalEf, dhstShow);
                SetObjectValueExt(this.ucKskMental.CboObject, currentKskMentalEf != null ? currentKskMentalEf.KSK_PATIENT_TYPES : null);
                this.ucKskMental.CboPaymentSource.EditValue = (currentKskMentalEf != null && currentKskMentalEf.KSK_PAY_SOURCE != null)
                    ? (object)currentKskMentalEf.KSK_PAY_SOURCE : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nhánh KskMental của SDO lưu hợp nhất: hồ sơ HIS_KSK_MENTAL + sinh hiệu (nếu có nhập).</summary>
        private MOS.SDO.KskMental2SDO BuildKskMentalSdo()
        {
            MOS.SDO.KskMental2SDO result = new MOS.SDO.KskMental2SDO();
            try
            {
                HIS_KSK_MENTAL obj = new HIS_KSK_MENTAL();
                if (currentKskMentalEf != null)
                {
                    obj.ID = currentKskMentalEf.ID;
                    obj.KSK_MENTAL_CODE = currentKskMentalEf.KSK_MENTAL_CODE;
                    obj.DHST_ID = currentKskMentalEf.DHST_ID;
                }
                if (currentServiceReq != null) obj.SERVICE_REQ_ID = currentServiceReq.ID;
                this.ucKskMental.GetData(obj);
                string types = GetObjectValueExt(this.ucKskMental.CboObject);
                obj.KSK_PATIENT_TYPES = !string.IsNullOrEmpty(types) ? types : null;
                obj.KSK_PAY_SOURCE = this.ucKskMental.CboPaymentSource.EditValue != null
                    ? (short?)Convert.ToInt16(this.ucKskMental.CboPaymentSource.EditValue) : null;
                result.HisKskMental = obj;

                HIS_DHST dhst = new HIS_DHST();
                if (dhstMental != null) dhst.ID = dhstMental.ID;
                else if (currentKskMentalEf != null && currentKskMentalEf.DHST_ID != null && currentKskMentalEf.DHST_ID > 0)
                    dhst.ID = currentKskMentalEf.DHST_ID.Value;
                this.ucKskMental.GetDhst(dhst);
                // Người đo = bác sĩ khám (giống tab trẻ dưới 6 tuổi)
                object login = this.ucKskMental.CboConcluder.EditValue;
                dhst.EXECUTE_LOGINNAME = login != null ? login.ToString() : null;
                if (!string.IsNullOrEmpty(dhst.EXECUTE_LOGINNAME))
                {
                    var emp = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                        .FirstOrDefault(o => o.LOGINNAME == dhst.EXECUTE_LOGINNAME);
                    dhst.EXECUTE_USERNAME = emp != null ? emp.TDL_USERNAME : null;
                }
                result.HisDhst = HasDhstData(dhst) ? dhst : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return result;
        }

        /// <summary>
        /// Bản ghi HIS_KSK_GENERAL gửi kèm khi lưu tab này — để lưu Bác sĩ khám + Ngày kết luận.
        /// Backend cập nhật CẢ DÒNG nên gửi BẢN SAO ĐẦY ĐỦ bản ghi hiện có (không gửi bản ghi rỗng chỉ có
        /// ID — sẽ xóa trắng kết luận các tab khác đã lưu). Người kết luận / ngày kết luận được gán sau
        /// ở luồng chung (FillConcluderExtToGeneral, ApplyConclusionTimeToKskGeneralSdo).
        /// </summary>
        private HIS_KSK_GENERAL BuildKskGeneralForMental()
        {
            HIS_KSK_GENERAL result = new HIS_KSK_GENERAL();
            try
            {
                if (currentKskGeneral != null)
                {
                    foreach (var pi in typeof(HIS_KSK_GENERAL).GetProperties())
                    {
                        if (!pi.CanRead || !pi.CanWrite) continue;
                        if (pi.PropertyType.IsClass && pi.PropertyType != typeof(string)) continue; // bỏ navigation
                        pi.SetValue(result, pi.GetValue(currentKskGeneral, null), null);
                    }
                }
                if (currentServiceReq != null) result.SERVICE_REQ_ID = currentServiceReq.ID;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return result;
        }

        /// <summary>
        /// Mục I (phần chưa có ở khối Thông tin bệnh nhân): ngày cấp, nơi cấp giấy tờ, dân tộc, nơi ở, SĐT.
        /// Giấy tờ ưu tiên CCCD → Hộ chiếu → CMND; ngày/nơi cấp lấy theo đúng loại giấy tờ đó.
        /// Dữ liệu đã prefetch sẵn (V_HIS_SERVICE_REQ + HIS_TREATMENT) — không gọi thêm API.
        /// </summary>
        private void LoadAdministrativeInfoMental()
        {
            try
            {
                if (currentServiceReq == null)
                {
                    this.ucKskMental.SetAdministrativeInfo(null, null, null, null, null);
                    return;
                }
                HIS_TREATMENT t = (preTreatments != null && preTreatments.Count > 0) ? preTreatments[0] : null;
                long? idDate = null;
                string idPlace = null;
                if (t != null)
                {
                    if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_CCCD_NUMBER)) { idDate = t.TDL_PATIENT_CCCD_DATE; idPlace = t.TDL_PATIENT_CCCD_PLACE; }
                    else if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_PASSPORT_NUMBER)) { idDate = t.TDL_PATIENT_PASSPORT_DATE; idPlace = t.TDL_PATIENT_PASSPORT_PLACE; }
                    else if (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_CMND_NUMBER)) { idDate = t.TDL_PATIENT_CMND_DATE; idPlace = t.TDL_PATIENT_CMND_PLACE; }
                }
                string idDateText = (idDate.HasValue && idDate.Value > 0)
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(idDate.Value) : null;
                string residence = BuildResidence(currentServiceReq);
                if (string.IsNullOrWhiteSpace(residence) && t != null) residence = t.TDL_PATIENT_ADDRESS;
                string phone = t != null
                    ? (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_MOBILE) ? t.TDL_PATIENT_MOBILE : t.TDL_PATIENT_PHONE)
                    : null;
                this.ucKskMental.SetAdministrativeInfo(idDateText, idPlace, t != null ? t.TDL_PATIENT_ETHNIC_NAME : null, residence, phone);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
    }
}
