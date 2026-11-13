/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Ksk định kỳ lái xe ô tô" — Giấy khám sức khỏe định kỳ đối với người hành nghề lái xe ô tô,
 * Mẫu 06 (Thông tư 25/2026/TT-BYT). BV Đa khoa Xuyên Á.
 * Xem _Software-Specs/02_Analysis_Design/PTTK_XXXXX_BV_DK_Xuyen_A_KSK_Dinh_Ky_Lai_Xe_O_To_Mau_06_TT25.md.
 *
 * Làm theo khuôn tab "Ksk tâm thần" (Mẫu 04):
 *  - Dữ liệu giấy -> HIS_KSK_PRO_DRIVER, thể lực -> HIS_DHST, 21 câu tiền sử -> HIS_PERIOD_DRIVER_DITY
 *    (theo KSK_PRO_DRIVER_ID): gửi qua nhánh KskProDriver của api/HisServiceReq/KskExecuteV2;
 *    nạp lại từ api/HisKskSync/GetKskData (SDO.HisKskProDrivers + HisPeriodDriverDitys).
 *  - Người kết luận + ngày kết luận -> HIS_KSK_GENERAL của lượt khám (như các tab khác).
 *  - Tab thêm lúc chạy, đứng sau "Ksk lái xe (Mẫu 5)" (index 10), chỉ hiện khi viện bật cấu hình
 *    HIS.Desktop.Plugins.EnterKskInfomantionVer2.ShowKskProDriverTab = 1.
 *  - Bảng HIS_KSK_PRO_DRIVER dùng chung với tab "Ksk lái xe (Mẫu 5)": Mẫu 06 ghi KSK_FORM_TYPE = 6
 *    (BE coi 6 hoặc NULL là Mẫu 6), mỗi y lệnh tối đa 1 bản ghi mỗi mẫu. Danh sách tải sẵn
 *    preKskProDrivers / preDitysProDriver khai báo ở partial Mẫu 5 (mọi mẫu, mọi y lệnh của hồ sơ) —
 *    ở đây chỉ lấy bản ghi KHÔNG phải Mẫu 5 của y lệnh đang mở.
 *  - Kết luận từng chuyên khoa: ô chữ + Thư viện văn bản ("PL:Lx" tự chọn phân loại) như tab trên 18 tuổi.
 *  - "Tự động lấy kết quả xét nghiệm": ma túy theo nhóm chỉ số có sẵn; chỉ số máu / nước tiểu / XN nữ /
 *    CĐHA theo bảng nối chỉ số (khuôn bảng nối M3 của màn Đồng bộ KSK, lưu ControlState theo máy).
 * In: Mps000525 — cần bản ghi SAR_PRINT_TYPE 'Mps000525' + file mẫu trong Tmp\Mps\Mps000525\.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Chỉ số tab "Ksk định kỳ lái xe ô tô" (Mẫu 06) — tab thêm lúc chạy, sau "Ksk lái xe (Mẫu 5)".</summary>
        private const int TAB_PRO_DRIVER_M6 = 10;
        /// <summary>HIS_KSK_PRO_DRIVER.KSK_FORM_TYPE ghi cho giấy Mẫu 06 (Mẫu 5 = 5).</summary>
        private const short KSK_FORM_TYPE__MAU6 = 6;
        /// <summary>Nhãn phân loại trong Thư viện văn bản cho câu kết luận từng chuyên khoa của Mẫu 06.</summary>
        private const string TEXT_LIB_HASHTAG__PRO_DRIVER_M6 = "KetLuanKskLaiXe";

        private DevExpress.XtraTab.XtraTabPage tabProDriverM6;
        private UcKskProDriverM6 ucKskProDriverM6;
        private bool isProDriverM6TabInited = false;
        /// <summary>Giấy Mẫu 06 đang hiển thị / vừa lưu (null = chưa có).</summary>
        private HIS_KSK_PRO_DRIVER currentKskProDriverM6;
        /// <summary>Sinh hiệu của giấy đang hiển thị — giữ ID để lần Lưu sau UPDATE.</summary>
        private HIS_DHST dhstProDriverM6;
        /// <summary>Danh mục 21 câu tiền sử (bệnh "KSK lái xe định kỳ") — tải 1 lần.</summary>
        private List<HIS_DISEASE_TYPE> proDriverM6DiseaseTypes;

        #region ===== Dựng tab =====

        /// <summary>
        /// Thêm trang tab Mẫu 06 vào dãy tab chính. Gọi đầu Load, trước khi chọn tab mặc định.
        /// Chỉ thêm khi trang rơi đúng chỉ số TAB_PRO_DRIVER_M6 — Designer đổi số tab thì bỏ qua (ghi nhật ký)
        /// để không lệch các nhánh xử lý theo chỉ số tab.
        /// </summary>
        private void InitTabProDriverM6Page()
        {
            try
            {
                if (tabProDriverM6 != null || this.xtraTabControl1 == null) return;
                // An toàn đa viện: chỉ hiện tab khi viện bật cấu hình (mặc định tắt).
                if (!Config.HisConfigCFG.IsShowKskProDriverM6Tab) return;
                if (this.xtraTabControl1.TabPages.Count != TAB_PRO_DRIVER_M6)
                {
                    LogSystem.Error("KskProDriver: day tab co " + this.xtraTabControl1.TabPages.Count
                        + " trang, khong phai " + TAB_PRO_DRIVER_M6 + " -> KHONG them tab Mau 06");
                    return;
                }
                tabProDriverM6 = new DevExpress.XtraTab.XtraTabPage();
                tabProDriverM6.Name = "tabKskProDriver";
                tabProDriverM6.Text = "Ksk định kỳ lái xe ô tô";
                tabProDriverM6.Tooltip = "Giấy khám sức khỏe định kỳ đối với người hành nghề lái xe ô tô (Mẫu 06 - Thông tư 25/2026/TT-BYT)";
                ucKskProDriverM6 = new UcKskProDriverM6();
                ucKskProDriverM6.Dock = DockStyle.Fill;
                tabProDriverM6.Controls.Add(ucKskProDriverM6);
                this.xtraTabControl1.TabPages.Add(tabProDriverM6);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Dựng 1 lần: danh mục ICD, Đối tượng / Nguồn chi trả, Phân loại, Người khám, Hạng GPLX, Người kết luận.</summary>
        private void InitTabProDriverM6()
        {
            if (isProDriverM6TabInited || ucKskProDriverM6 == null) return;
            isProDriverM6TabInited = true;
            try
            {
                ucKskProDriverM6.InitUc(this.historyIcdDataSource, this.historyIcdPageSize);
                InitAdminCombosExt(ucKskProDriverM6.CboObject, ucKskProDriverM6.CboPaymentSource);
                foreach (var cbo in ucKskProDriverM6.RankCombos) SetDataCboRank(cbo);
                // Nút Xóa của các combo do InitClearButtonForGridLookUpEdits (lúc Load) gắn chung cho cả form.
                foreach (var cbo in ucKskProDriverM6.LoginCombos) SetDataCboExamLoginName(cbo);
                LoadLicenseClassComboM6(ucKskProDriverM6.CboLicenseClass);
                // Người kết luận của lượt khám (HIS_KSK_GENERAL) — đồng bộ với các tab khác.
                RegisterConcluderCombo(TAB_PRO_DRIVER_M6, ucKskProDriverM6.CboConcluder);
                ucKskProDriverM6.ConclusionTextLibRequested += (txt, rank) => OpenTextLibExamResult(txt, TEXT_LIB_HASHTAG__PRO_DRIVER_M6, rank);
                ucKskProDriverM6.ClsMapRequested += (s, e) => OpenProDriverM6ClsMap();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void LoadLicenseClassComboM6(GridLookUpEdit cbo)
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_LICENSE_CLASS>(false, true)
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("LICENSE_CLASS_NAME", "", 250, 0));
                Inventec.Common.Controls.EditorLoader.ControlEditorADO ado = new Inventec.Common.Controls.EditorLoader.ControlEditorADO("LICENSE_CLASS_NAME", "ID", columnInfos, false, 250);
                Inventec.Common.Controls.EditorLoader.ControlEditorLoader.Load(cbo, data, ado);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Nạp =====

        /// <summary>Nạp tab khi mở lần đầu / đổi y lệnh — gọi từ EnsureTabLoaded.</summary>
        private void LoadTabProDriverM6()
        {
            try
            {
                if (ucKskProDriverM6 == null) return;
                InitTabProDriverM6();
                ClearTabInputEditors(TAB_PRO_DRIVER_M6);
                // Combo chọn nhiều không bị ClearTabInputEditors xóa — xóa riêng.
                SetObjectValueExt(ucKskProDriverM6.CboObject, null);
                ucKskProDriverM6.CboPaymentSource.EditValue = null;
                ucKskProDriverM6.SetDiseaseCatalog(GetProDriverM6DiseaseTypes());
                ucKskProDriverM6.SetFemale(currentServiceReq == null
                    || currentServiceReq.TDL_PATIENT_GENDER_ID != IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE);
                LoadAdministrativeInfoProDriverM6();
                FillDataProDriverM6();
                LoadConcluderComboExt();
                DateTime conclusionTime = DateTime.Now;
                if (currentKskGeneral != null && currentKskGeneral.CONCLUSION_TIME != null && currentKskGeneral.CONCLUSION_TIME > 0)
                    conclusionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.CONCLUSION_TIME.Value) ?? DateTime.Now;
                ucKskProDriverM6.DteConclusionTime.DateTime = conclusionTime;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// 21 câu tiền sử bản thân của giấy Mẫu 06 = danh mục bệnh mã 01–21 đang hoạt động, sắp theo số câu.
        /// KHÔNG lọc theo cờ "KSK lái xe định kỳ": cờ này phục vụ tab Ksk lái xe cũ và có viện đã bỏ cờ ở mã 11
        /// "Nghiện rượu, bia" (vẫn là câu 11 trên giấy Mẫu 06) — lọc theo cờ sẽ thiếu câu và lệch số câu khi in.
        /// </summary>
        private List<HIS_DISEASE_TYPE> GetProDriverM6DiseaseTypes()
        {
            try
            {
                if (proDriverM6DiseaseTypes != null) return proDriverM6DiseaseTypes;
                CommonParam param = new CommonParam();
                MOS.Filter.HisDiseaseTypeFilter filter = new MOS.Filter.HisDiseaseTypeFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(param).Get<List<HIS_DISEASE_TYPE>>("api/HisDiseaseType/Get", ApiConsumers.MosConsumer, filter, param);
                proDriverM6DiseaseTypes = (data ?? new List<HIS_DISEASE_TYPE>())
                    .Where(o => UcKskProDriverM6.QuestionNo(o) > 0)
                    .GroupBy(o => UcKskProDriverM6.QuestionNo(o)).Select(g => g.OrderBy(o => o.ID).First())
                    .OrderBy(o => UcKskProDriverM6.QuestionNo(o)).ToList();
                if (proDriverM6DiseaseTypes.Count != 21)
                    LogSystem.Warn("KskProDriverM6: danh muc benh ma 01-21 dang hoat dong co " + proDriverM6DiseaseTypes.Count + " muc (giay Mau 06 can 21)");
            }
            catch (Exception ex) { LogSystem.Warn(ex); proDriverM6DiseaseTypes = new List<HIS_DISEASE_TYPE>(); }
            return proDriverM6DiseaseTypes;
        }

        /// <summary>
        /// Đổ giấy Mẫu 06 của y lệnh lên tab. Nguồn: bản vừa lưu (current) trước, bản tải sẵn (pre) sau.
        /// Chưa có giấy -> thể lực lấy sẵn từ sinh hiệu của y lệnh (chỉ lấy giá trị, không lấy ID), tiền sử /
        /// thị lực / thính lực lấy từ y lệnh khám (như tab Ksk lái xe), 21 câu tiền sử mặc định "Không".
        /// </summary>
        private void FillDataProDriverM6()
        {
            try
            {
                if (currentKskProDriverM6 == null)
                    currentKskProDriverM6 = FindPreKskProDriverM6();

                dhstProDriverM6 = null;
                HIS_DHST dhstShow = null;
                if (currentKskProDriverM6 != null)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentKskProDriverM6.DHST_ID);
                    dhstProDriverM6 = (ds != null && ds.Count > 0) ? ds[0] : null;
                    dhstShow = dhstProDriverM6;
                }
                else if (currentServiceReq != null)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentServiceReq.DHST_ID);
                    dhstShow = (ds != null && ds.Count > 0) ? ds[0] : null;
                }

                ucKskProDriverM6.FillData(currentKskProDriverM6, dhstShow);
                if (currentKskProDriverM6 == null) FillProDriverM6DefaultsFromServiceReq();
                SetObjectValueExt(ucKskProDriverM6.CboObject, currentKskProDriverM6 != null ? currentKskProDriverM6.KSK_PATIENT_TYPES : null);
                ucKskProDriverM6.CboPaymentSource.EditValue = (currentKskProDriverM6 != null && currentKskProDriverM6.KSK_PAY_SOURCE != null)
                    ? (object)currentKskProDriverM6.KSK_PAY_SOURCE : null;

                List<HIS_PERIOD_DRIVER_DITY> ditys = null;
                if (currentKskProDriverM6 != null && preDitysProDriver != null)
                    ditys = preDitysProDriver.Where(o => o != null && o.KSK_PRO_DRIVER_ID == currentKskProDriverM6.ID).ToList();
                ucKskProDriverM6.SetDiseaseAnswers(ditys, currentKskProDriverM6 == null);
                ucKskProDriverM6.UpdateConclusionLicenseLabel();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Giấy Mẫu 06 đã lưu của y lệnh đang mở trong dữ liệu tải sẵn (danh sách chung mọi mẫu, mọi y lệnh
        /// của hồ sơ): cùng y lệnh và không phải Mẫu 5. Không có thì null (giấy mới).
        /// </summary>
        private HIS_KSK_PRO_DRIVER FindPreKskProDriverM6()
        {
            if (currentServiceReq == null || preKskProDrivers == null) return null;
            long srId = currentServiceReq.ID;
            return preKskProDrivers.FirstOrDefault(o => o != null && o.SERVICE_REQ_ID == srId && !IsMau5(o));
        }

        /// <summary>
        /// Sau khi Lưu tab Mẫu 06: giữ giấy vừa lưu (có số giấy, DHST_ID) và cập nhật danh sách tải sẵn dùng chung
        /// (tab Mẫu 5 điền sẵn từ Mẫu 6 đọc danh sách này).
        /// </summary>
        private void ApplyKskProDriverM6Saved(HIS_KSK_PRO_DRIVER saved, MOS.SDO.KskProDriver2SDO sent)
        {
            try
            {
                if (saved == null) return;
                currentKskProDriverM6 = saved;
                // BE không trả HIS_DHST: lấy ID theo giấy vừa lưu để lần Lưu sau cập nhật đúng sinh hiệu
                if (sent != null && sent.HisDhst != null && saved.DHST_ID.HasValue && saved.DHST_ID > 0)
                {
                    sent.HisDhst.ID = saved.DHST_ID.Value;
                    dhstProDriverM6 = sent.HisDhst;
                    if (preDhstById == null) preDhstById = new Dictionary<long, HIS_DHST>();
                    preDhstById[sent.HisDhst.ID] = sent.HisDhst;
                }

                if (preKskProDrivers == null) preKskProDrivers = new List<HIS_KSK_PRO_DRIVER>();
                preKskProDrivers.RemoveAll(o => o != null && o.ID == saved.ID);
                preKskProDrivers.Add(saved);

                var ditys = sent != null && sent.HisPeriodDriverDitys != null ? sent.HisPeriodDriverDitys : new List<HIS_PERIOD_DRIVER_DITY>();
                ditys.ForEach(o => o.KSK_PRO_DRIVER_ID = saved.ID);
                if (preDitysProDriver == null) preDitysProDriver = new List<HIS_PERIOD_DRIVER_DITY>();
                preDitysProDriver.RemoveAll(o => o != null && o.KSK_PRO_DRIVER_ID == saved.ID);
                preDitysProDriver.AddRange(ditys);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Hồ sơ mới: lấy sẵn tiền sử gia đình, thị lực, thính lực đã nhập ở y lệnh khám.</summary>
        private void FillProDriverM6DefaultsFromServiceReq()
        {
            try
            {
                var sr = currentServiceReq;
                if (sr == null) return;
                if (!string.IsNullOrWhiteSpace(sr.PATHOLOGICAL_HISTORY_FAMILY))
                {
                    SetProDriverM6Field("IS_FAMILY_HISTORY", 1);
                    SetProDriverM6Field("FAMILY_HISTORY", sr.PATHOLOGICAL_HISTORY_FAMILY);
                }
                SetProDriverM6Field("EXAM_EYESIGHT_RIGHT", sr.PART_EXAM_EYESIGHT_RIGHT);
                SetProDriverM6Field("EXAM_EYESIGHT_LEFT", sr.PART_EXAM_EYESIGHT_LEFT);
                SetProDriverM6Field("EXAM_EYESIGHT_GLASS_RIGHT", sr.PART_EXAM_EYESIGHT_GLASS_RIGHT);
                SetProDriverM6Field("EXAM_EYESIGHT_GLASS_LEFT", sr.PART_EXAM_EYESIGHT_GLASS_LEFT);
                SetProDriverM6Field("EXAM_ENT_LEFT_NORMAL", sr.PART_EXAM_EAR_LEFT_NORMAL);
                SetProDriverM6Field("EXAM_ENT_LEFT_WHISPER", sr.PART_EXAM_EAR_LEFT_WHISPER);
                SetProDriverM6Field("EXAM_ENT_RIGHT_NORMAL", sr.PART_EXAM_EAR_RIGHT_NORMAL);
                SetProDriverM6Field("EXAM_ENT_RIGHT_WHISPER", sr.PART_EXAM_EAR_RIGHT_WHISPER);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void SetProDriverM6Field(string column, object value)
        {
            if (value == null || (value is string && string.IsNullOrWhiteSpace((string)value))) return;
            BaseEdit be = ucKskProDriverM6.GetFieldEditor(column);
            if (be != null) be.EditValue = value;
        }

        /// <summary>Phần hành chính chỉ đọc — dữ liệu đã prefetch sẵn (V_HIS_SERVICE_REQ + HIS_TREATMENT), không gọi thêm API.</summary>
        private void LoadAdministrativeInfoProDriverM6()
        {
            try
            {
                if (currentServiceReq == null)
                {
                    ucKskProDriverM6.SetAdministrativeInfo(null, null, null, null, null, null, null);
                    return;
                }
                HIS_TREATMENT t = (preTreatments != null && preTreatments.Count > 0) ? preTreatments[0] : null;
                string residence = BuildResidence(currentServiceReq);
                if (string.IsNullOrWhiteSpace(residence) && t != null) residence = t.TDL_PATIENT_ADDRESS;
                string blood = t != null
                    ? string.Join(" ", new[] { t.TDL_PATIENT_BLOOD_ABO_CODE, t.TDL_PATIENT_BLOOD_RH_CODE }.Where(o => !string.IsNullOrWhiteSpace(o)).ToArray())
                    : null;
                string phone = t != null
                    ? (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_MOBILE) ? t.TDL_PATIENT_MOBILE : t.TDL_PATIENT_PHONE)
                    : null;
                string workPlace = t != null
                    ? (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_WORK_PLACE_NAME) ? t.TDL_PATIENT_WORK_PLACE_NAME : t.TDL_PATIENT_WORK_PLACE)
                    : null;
                ucKskProDriverM6.SetAdministrativeInfo(
                    t != null ? t.TDL_PATIENT_ETHNIC_NAME : null,
                    blood,
                    t != null ? t.TDL_HEIN_CARD_NUMBER : null,
                    residence,
                    t != null ? t.TDL_PATIENT_CAREER_NAME : null,
                    workPlace,
                    phone);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== Lưu =====

        /// <summary>Nhánh KskProDriver của SDO lưu hợp nhất: giấy Mẫu 06 + sinh hiệu (nếu có nhập) + 21 câu tiền sử.</summary>
        private MOS.SDO.KskProDriver2SDO BuildKskProDriverM6Sdo()
        {
            MOS.SDO.KskProDriver2SDO result = new MOS.SDO.KskProDriver2SDO();
            try
            {
                HIS_KSK_PRO_DRIVER obj = new HIS_KSK_PRO_DRIVER();
                if (currentKskProDriverM6 != null)
                {
                    obj.ID = currentKskProDriverM6.ID;
                    obj.KSK_PRO_DRIVER_CODE = currentKskProDriverM6.KSK_PRO_DRIVER_CODE;
                    obj.DHST_ID = currentKskProDriverM6.DHST_ID;
                }
                if (currentServiceReq != null) obj.SERVICE_REQ_ID = currentServiceReq.ID;
                ucKskProDriverM6.GetData(obj);
                // Bảng dùng chung với Mẫu 5 — ghi rõ loại mẫu (BE tìm bản ghi cũ của y lệnh theo loại mẫu).
                obj.KSK_FORM_TYPE = KSK_FORM_TYPE__MAU6;
                string types = GetObjectValueExt(ucKskProDriverM6.CboObject);
                obj.KSK_PATIENT_TYPES = !string.IsNullOrEmpty(types) ? types : null;
                obj.KSK_PAY_SOURCE = ucKskProDriverM6.CboPaymentSource.EditValue != null
                    ? (short?)Convert.ToInt16(ucKskProDriverM6.CboPaymentSource.EditValue) : null;
                result.HisKskProDriver = obj;
                result.HisPeriodDriverDitys = ucKskProDriverM6.GetDiseaseAnswers();

                HIS_DHST dhst = new HIS_DHST();
                if (dhstProDriverM6 != null) dhst.ID = dhstProDriverM6.ID;
                else if (currentKskProDriverM6 != null && currentKskProDriverM6.DHST_ID != null && currentKskProDriverM6.DHST_ID > 0)
                    dhst.ID = currentKskProDriverM6.DHST_ID.Value;
                ucKskProDriverM6.GetDhst(dhst);
                // Người đo = người kết luận (giống tab Ksk tâm thần)
                object login = ucKskProDriverM6.CboConcluder.EditValue;
                dhst.EXECUTE_LOGINNAME = login != null ? login.ToString() : null;
                if (!string.IsNullOrEmpty(dhst.EXECUTE_LOGINNAME))
                {
                    var emp = BackendDataWorker.Get<V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == dhst.EXECUTE_LOGINNAME);
                    dhst.EXECUTE_USERNAME = emp != null ? emp.TDL_USERNAME : null;
                }
                result.HisDhst = HasDhstData(dhst) ? dhst : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return result;
        }

        /// <summary>Kết luận "Đạt tiêu chuẩn … nhưng yêu cầu khám lại" bắt buộc ghi thời gian khám lại (QT07).</summary>
        private void ValidateProDriverM6Conclusion(List<string> messages)
        {
            try
            {
                if (ucKskProDriverM6 == null) return;
                object v = ucKskProDriverM6.RdoConclusion.EditValue;
                if (v != null && v != DBNull.Value && Convert.ToInt32(v) == 3 && string.IsNullOrWhiteSpace(ucKskProDriverM6.TxtReexam.Text))
                    AddRequiredError(ucKskProDriverM6.TxtReexam, "Kết luận \"yêu cầu khám lại\": chưa nhập thời gian khám lại.", messages);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region ===== In Mps000525 =====

        /// <summary>
        /// Dựng dữ liệu in Giấy KSK định kỳ lái xe ô tô (Mps000525). Ưu tiên bản ĐÃ LƯU; chưa lưu thì dựng từ
        /// các ô trên tab. Người kết luận + ngày kết luận lấy theo ô đang hiển thị.
        /// </summary>
        private void LoadBieuMauPhieuMps000525(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                Inventec.Desktop.Common.Message.WaitingManager.Show();

                V_HIS_TREATMENT_4 treatment = null;
                if (currentServiceReq != null && currentServiceReq.TREATMENT_ID > 0)
                {
                    var tFilter = new MOS.Filter.HisTreatmentView4Filter { ID = currentServiceReq.TREATMENT_ID };
                    var ts = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumers.MosConsumer, tFilter, null);
                    treatment = ts != null ? ts.FirstOrDefault() : null;
                }

                MOS.SDO.KskProDriver2SDO fromForm = BuildKskProDriverM6Sdo();
                HIS_KSK_PRO_DRIVER k = currentKskProDriverM6 ?? fromForm.HisKskProDriver;

                HIS_DHST dhst = null;
                if (k.DHST_ID != null && k.DHST_ID > 0)
                {
                    List<HIS_DHST> ds = PreGetDhst(k.DHST_ID);
                    dhst = (ds != null && ds.Count > 0) ? ds[0] : null;
                    if (dhst == null)
                    {
                        var dl = new BackendAdapter(param).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer,
                            new MOS.Filter.HisDhstFilter { ID = k.DHST_ID }, param);
                        dhst = dl != null ? dl.FirstOrDefault() : null;
                    }
                }
                if (dhst == null) dhst = fromForm.HisDhst;

                HIS_KSK_GENERAL general = BuildKskGeneralForMental();
                object login = ucKskProDriverM6.CboConcluder.EditValue;
                general.CONCLUDER_LOGINNAME = login != null && !string.IsNullOrWhiteSpace(login.ToString()) ? login.ToString() : null;
                var employees = BackendDataWorker.Get<V_HIS_EMPLOYEE>() ?? new List<V_HIS_EMPLOYEE>();
                general.CONCLUDER_USERNAME = !string.IsNullOrEmpty(general.CONCLUDER_LOGINNAME)
                    ? employees.Where(o => o.LOGINNAME == general.CONCLUDER_LOGINNAME).Select(o => o.TDL_USERNAME).FirstOrDefault()
                    : null;
                if (ucKskProDriverM6.DteConclusionTime.EditValue != null)
                    general.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ucKskProDriverM6.DteConclusionTime.DateTime);

                // Tên bác sĩ từng mục / người thực hiện CLS (mọi cột *_LOGINNAME của giấy)
                var logins = typeof(HIS_KSK_PRO_DRIVER).GetProperties()
                    .Where(p => p.Name.EndsWith("_LOGINNAME") && p.PropertyType == typeof(string))
                    .Select(p => p.GetValue(k, null) as string)
                    .Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                Dictionary<string, string> names = new Dictionary<string, string>();
                foreach (var e in employees.Where(o => o != null && logins.Contains(o.LOGINNAME)))
                    if (!names.ContainsKey(e.LOGINNAME)) names[e.LOGINNAME] = e.TDL_USERNAME;

                // 21 câu tiền sử: theo câu trả lời đang hiển thị trên tab (= bản đã lưu nếu chưa sửa)
                List<HIS_PERIOD_DRIVER_DITY> answers = fromForm.HisPeriodDriverDitys ?? new List<HIS_PERIOD_DRIVER_DITY>();
                List<MPS.Processor.Mps000525.PDO.Mps000525DiseaseADO> diseases = new List<MPS.Processor.Mps000525.PDO.Mps000525DiseaseADO>();
                List<HIS_DISEASE_TYPE> catalog = GetProDriverM6DiseaseTypes();
                for (int i = 0; i < catalog.Count; i++)
                {
                    var a = answers.FirstOrDefault(o => o.DISEASE_TYPE_ID == catalog[i].ID);
                    int no = UcKskProDriverM6.QuestionNo(catalog[i]);
                    diseases.Add(new MPS.Processor.Mps000525.PDO.Mps000525DiseaseADO
                    {
                        STT = no > 0 ? no : i + 1, // số câu theo mã danh mục — thiếu 1 mã thì các câu sau không bị đẩy lên
                        DISEASE_TYPE_NAME = catalog[i].DISEASE_TYPE_NAME,
                        IS_YES_NO = a != null ? a.IS_YES_NO : null
                    });
                }

                string licenseName = null;
                if (k.LICENSE_CLASS_ID.HasValue)
                {
                    var lc = BackendDataWorker.Get<HIS_LICENSE_CLASS>().FirstOrDefault(o => o.ID == k.LICENSE_CLASS_ID.Value);
                    licenseName = lc != null ? lc.LICENSE_CLASS_NAME : null;
                }

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                MPS.Processor.Mps000525.PDO.Mps000525PDO rdo = new MPS.Processor.Mps000525.PDO.Mps000525PDO(
                    k, currentServiceReq, dhst, general, treatment);
                rdo.KskServiceReq = printKskServiceReq;
                rdo.KskPatient = printKskPatient;
                rdo.KskPatientTypesName = MapKskNames(BuildKskObjectList(), k.KSK_PATIENT_TYPES);
                rdo.KskPaySourceName = MapKskNames(BuildKskPaymentSourceList(),
                    k.KSK_PAY_SOURCE.HasValue ? k.KSK_PAY_SOURCE.Value.ToString() : null);
                rdo.LicenseClassName = licenseName;
                rdo.Diseases = diseases;
                rdo.HealthExamRanks = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                rdo.EmployeeNames = names;
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region ===== Nối chỉ số CLS + tự động lấy kết quả =====

        /// <summary>Điền kết quả ma túy theo nhóm chỉ số vào ô của Mẫu 06 (chỉ khi ô đang mở và còn trống).</summary>
        private void FillProDriverM6Test(string column, string value)
        {
            if (ucKskProDriverM6 != null) ucKskProDriverM6.SetFieldTextIfEmpty(column, value);
        }

        /// <summary>Bảng nối chỉ số Mẫu 06 đã lưu tại máy (ControlState cùng module với "Thiết lập tự động lấy CLS").</summary>
        private string GetProDriverM6ClsMapJson()
        {
            try
            {
                var csw = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var states = csw.GetData(frmAutoClsSetting.MODULE_LINK) ?? new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                var it = states.FirstOrDefault(o => o.KEY == KskProDriverM6ClsFields.STATE_KEY && o.MODULE_LINK == frmAutoClsSetting.MODULE_LINK);
                return it != null ? it.VALUE : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); return null; }
        }

        /// <summary>
        /// Mở hộp thoại nối chỉ số. Lưu: đọc ĐỦ danh sách khóa của module rồi chỉ thay khóa của Mẫu 06 —
        /// ControlStateWorker.SetData ghi đè cả danh sách, làm khác là mất cấu hình "Tự động lấy CLS".
        /// </summary>
        private void OpenProDriverM6ClsMap()
        {
            try
            {
                using (var frm = new frmKskProDriverM6ClsMap(GetProDriverM6ClsMapJson()))
                {
                    if (frm.ShowDialog(this) != DialogResult.OK) return;
                    var csw = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                    var states = csw.GetData(frmAutoClsSetting.MODULE_LINK) ?? new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    var it = states.FirstOrDefault(o => o.KEY == KskProDriverM6ClsFields.STATE_KEY && o.MODULE_LINK == frmAutoClsSetting.MODULE_LINK);
                    if (it != null) it.VALUE = frm.ResultJson;
                    else states.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO()
                    {
                        KEY = KskProDriverM6ClsFields.STATE_KEY,
                        VALUE = frm.ResultJson,
                        MODULE_LINK = frmAutoClsSetting.MODULE_LINK
                    });
                    csw.SetData(states);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tích "Tự động lấy kết quả xét nghiệm" ở tab Mẫu 06 (QT16):
        ///  1. Ma túy theo nhóm chỉ số có sẵn (Morphin, Heroin, Codein, Amphetamin, Marijuana) — FillTestIndexByGroupCode.
        ///  2. Chỉ số máu / nước tiểu / XN nữ / CĐHA theo bảng nối chỉ số Mẫu 06.
        /// Chỉ điền ô đang trống; người thực hiện lấy theo người thực hiện y lệnh.
        /// KHÔNG chạy phần "theo dịch vụ" của tab trên 18 tuổi (ô đích nằm ở tab đó).
        /// </summary>
        private void AutoGetTestIndexProDriverM6()
        {
            try
            {
                if (currentServiceReq == null || ucKskProDriverM6 == null) return;
                OpenTestIndexGroupHeadless();

                var map = KskProDriverM6ClsFields.Parse(GetProDriverM6ClsMapJson());
                if (map.Count == 0)
                {
                    this.BeginInvoke(new Action(() => XtraMessageBox.Show(this,
                        "Chưa nối chỉ số xét nghiệm cho Mẫu 06 — mới lấy được kết quả ma túy.\r\n"
                        + "Vào tab con \"Cận lâm sàng\" > \"Nối chỉ số CLS…\" để nối chỉ số máu / nước tiểu / CĐHA.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<long> trIds = new List<long>() { currentServiceReq.TREATMENT_ID };
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>("api/HisSereServ/GetView2", ApiConsumers.MosConsumer,
                    new MOS.Filter.HisSereServView2Filter { TREATMENT_IDs = trIds, HAS_EXECUTE = true }, param) ?? new List<V_HIS_SERE_SERV_2>();
                List<long> ssIds = sereServs.Select(o => o.ID).Distinct().ToList();
                List<V_HIS_SERE_SERV_TEIN> teins = ssIds.Count > 0
                    ? (new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer,
                        new MOS.Filter.HisSereServTeinViewFilter { SERE_SERV_IDs = ssIds, IS_ACTIVE = 1 }, param) ?? new List<V_HIS_SERE_SERV_TEIN>())
                    : new List<V_HIS_SERE_SERV_TEIN>();
                List<HIS_SERE_SERV_EXT> exts = new BackendAdapter(param).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumers.MosConsumer,
                    new MOS.Filter.HisSereServExtFilter { TDL_TREATMENT_IDs = trIds }, param) ?? new List<HIS_SERE_SERV_EXT>();
                WaitingManager.Hide();

                Dictionary<long, V_HIS_SERE_SERV_2> ssById = new Dictionary<long, V_HIS_SERE_SERV_2>();
                foreach (var ss in sereServs) if (!ssById.ContainsKey(ss.ID)) ssById[ss.ID] = ss;

                // Mã chỉ số -> (giá trị, người thực hiện); giữ giá trị KHÔNG RỖNG ĐẦU TIÊN (như bảng nối M3).
                var byIndex = new Dictionary<string, KeyValuePair<string, string>>();
                foreach (var t in teins)
                {
                    if (t == null || string.IsNullOrWhiteSpace(t.TEST_INDEX_CODE) || string.IsNullOrWhiteSpace(t.VALUE)) continue;
                    string key = t.TEST_INDEX_CODE.Trim().ToUpperInvariant();
                    if (byIndex.ContainsKey(key)) continue;
                    V_HIS_SERE_SERV_2 ss;
                    byIndex[key] = new KeyValuePair<string, string>(t.VALUE.Trim(), ssById.TryGetValue(t.SERE_SERV_ID, out ss) ? ss.EXECUTE_LOGINNAME : null);
                }
                // Mã dịch vụ -> (kết luận, trống thì mô tả; người thực hiện)
                var extBySs = new Dictionary<long, HIS_SERE_SERV_EXT>();
                foreach (var e in exts) if (e != null && !extBySs.ContainsKey(e.SERE_SERV_ID)) extBySs[e.SERE_SERV_ID] = e;
                var byService = new Dictionary<string, KeyValuePair<string, string>>();
                foreach (var ss in sereServs)
                {
                    HIS_SERE_SERV_EXT e;
                    if (ss == null || string.IsNullOrWhiteSpace(ss.TDL_SERVICE_CODE) || !extBySs.TryGetValue(ss.ID, out e)) continue;
                    string v = !string.IsNullOrWhiteSpace(e.CONCLUDE) ? e.CONCLUDE : e.DESCRIPTION;
                    if (string.IsNullOrWhiteSpace(v)) continue;
                    string key = ss.TDL_SERVICE_CODE.Trim().ToUpperInvariant();
                    if (!byService.ContainsKey(key)) byService[key] = new KeyValuePair<string, string>(v.Trim(), ss.EXECUTE_LOGINNAME);
                }

                // Gom theo ô đích (2 chỉ tiêu nữ dồn chung 1 ô của Mẫu 06)
                var valuesByColumn = new Dictionary<string, List<string>>();
                var performerByColumn = new Dictionary<string, string>();
                int found = 0;
                foreach (var f in KskProDriverM6ClsFields.ALL)
                {
                    KskProDriverM6ClsFields.MapItem it;
                    if (!map.TryGetValue(f.Code, out it)) continue;
                    string code = it.TestIndexCode.Trim().ToUpperInvariant();
                    KeyValuePair<string, string> hit;
                    bool ok = it.SourceKind == KskProDriverM6ClsFields.KIND_SERVICE ? byService.TryGetValue(code, out hit) : byIndex.TryGetValue(code, out hit);
                    if (!ok) continue;
                    found++;
                    if (!valuesByColumn.ContainsKey(f.Column)) valuesByColumn[f.Column] = new List<string>();
                    bool shared = KskProDriverM6ClsFields.ALL.Count(o => o.Column == f.Column) > 1;
                    valuesByColumn[f.Column].Add(shared ? f.Name + ": " + hit.Key : hit.Key);
                    if (!performerByColumn.ContainsKey(f.PerformerColumn) && !string.IsNullOrWhiteSpace(hit.Value))
                        performerByColumn[f.PerformerColumn] = hit.Value;
                }
                var filledPerformers = new HashSet<string>();
                foreach (var kv in valuesByColumn)
                {
                    if (!ucKskProDriverM6.SetFieldTextIfEmpty(kv.Key, string.Join("; ", kv.Value.ToArray()))) continue;
                    string pfCol = KskProDriverM6ClsFields.ALL.First(o => o.Column == kv.Key).PerformerColumn;
                    string pf;
                    if (filledPerformers.Contains(pfCol) || !performerByColumn.TryGetValue(pfCol, out pf)) continue;
                    BaseEdit cbo = ucKskProDriverM6.GetFieldEditor(pfCol);
                    if (cbo != null && cbo.Enabled && (cbo.EditValue == null || string.IsNullOrWhiteSpace(cbo.EditValue.ToString())))
                    {
                        cbo.EditValue = pf;
                        filledPerformers.Add(pfCol);
                    }
                }
                LogSystem.Info("KskProDriver/CLS: noi " + map.Count + " chi tieu, co ket qua " + found
                    + ", ket qua XN " + teins.Count + ", dich vu co ket qua " + byService.Count);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
