/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Ksk lái xe (Mẫu 5)" — Giấy khám sức khỏe của người lái xe, người điều khiển xe máy chuyên dùng,
 * Mẫu 5 Thông tư 36/2024/TT-BYT (BV Đa khoa Xuyên Á).
 *
 * Dùng chung bảng HIS_KSK_PRO_DRIVER với Mẫu 6 (KSK định kỳ người hành nghề lái xe ô tô, việc 61143):
 * cột KSK_FORM_TYPE = 5 là Mẫu 5; 6 hoặc NULL là Mẫu 6. Mỗi y lệnh tối đa 1 bản ghi cho mỗi mẫu.
 *
 * Lưu: 1 lệnh api/HisServiceReq/KskExecuteV2 —
 *  - nhánh KskProDriver: bản ghi Mẫu 5 + thể lực (HIS_DHST) + 22 dòng tiền sử (HIS_PERIOD_DRIVER_DITY theo KSK_PRO_DRIVER_ID);
 *    BE tự sinh số giấy KSK_PRO_DRIVER_CODE (dãy PRO_DRIVER chung với Mẫu 6), sửa lại thì giữ số cũ;
 *  - nhánh KskGeneral: người kết luận + ngày kết luận (như tab Ksk tâm thần).
 * Nạp: api/HisKskSync/GetKskData (PrefetchFormData) — HisKskProDrivers + HisPeriodDriverDitys + HisDhsts.
 * Chưa có bản ghi Mẫu 5 mà hồ sơ đã có Mẫu 6: tự điền sẵn phần khám chung từ Mẫu 6, bác sĩ sửa rồi Lưu.
 * In: Mps000524 (LoadBieuMauPhieuMps000524) — cần SAR_PRINT_TYPE 'Mps000524' + mẫu trong Tmp\Mps\Mps000524\.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Run
{
    public partial class frmEnterKskInfomantionVer2
    {
        /// <summary>Chỉ số tab "Ksk lái xe (Mẫu 5)" — tab cuối, sau "Ksk tâm thần".</summary>
        private const int TAB_PRO_DRIVER = 9;

        /// <summary>HIS_KSK_PRO_DRIVER.KSK_FORM_TYPE của Mẫu 5 (Mẫu 6 = 6 hoặc NULL).</summary>
        private const short KSK_FORM_TYPE__MAU5 = 5;

        /// <summary>
        /// Cột không chép khi điền sẵn từ Mẫu 6: khóa/số giấy/loại mẫu/sinh hiệu/người tạo-sửa
        /// và phần đề nghị - kết luận (mỗi giấy kết luận riêng).
        /// </summary>
        private static readonly HashSet<string> PRO_DRIVER_PREFILL_SKIP = new HashSet<string>
        {
            "ID", "CREATE_TIME", "MODIFY_TIME", "CREATOR", "MODIFIER", "APP_CREATOR", "APP_MODIFIER",
            "IS_ACTIVE", "IS_DELETE", "GROUP_CODE", "SERVICE_REQ_ID", "TDL_TREATMENT_ID", "TDL_PATIENT_ID",
            "DHST_ID", "KSK_PRO_DRIVER_CODE", "KSK_FORM_TYPE",
            "DRIVER_CONCLUSION_TYPE", "REEXAM_NOTE", "SUGGEST_TYPE", "SUGGEST_NOTE"
        };

        private bool isProDriverTabInited = false;
        /// <summary>Bản ghi HIS_KSK_PRO_DRIVER (mọi mẫu, mọi y lệnh của hồ sơ) tải sẵn từ GetKskData.</summary>
        private List<HIS_KSK_PRO_DRIVER> preKskProDrivers;
        /// <summary>Tiền sử gắn với các bản ghi HIS_KSK_PRO_DRIVER (KSK_PRO_DRIVER_ID != null) tải sẵn từ GetKskData.</summary>
        private List<HIS_PERIOD_DRIVER_DITY> preDitysProDriver;
        /// <summary>Bản ghi Mẫu 5 đã lưu của y lệnh đang mở (null = chưa có).</summary>
        private HIS_KSK_PRO_DRIVER currentKskProDriver;
        /// <summary>Bản điền sẵn từ Mẫu 6 khi chưa có Mẫu 5 (chưa lưu, ID = 0).</summary>
        private HIS_KSK_PRO_DRIVER prefillKskProDriver;
        /// <summary>Sinh hiệu đã lưu của bản ghi Mẫu 5 — giữ ID để lần Lưu sau cập nhật, không tạo mới.</summary>
        private HIS_DHST dhstProDriver;
        /// <summary>Câu trả lời tiền sử để đổ lên lưới (của Mẫu 5 đã lưu, hoặc của Mẫu 6 khi điền sẵn).</summary>
        private List<HIS_PERIOD_DRIVER_DITY> proDriverDitys;

        #region Ẩn/hiện tab theo key

        /// <summary>
        /// Key HIS.Desktop.Plugins.EnterKskInfomantionVer2.IsUseKskDriverForm5 = "1": ẩn tab "Ksk lái xe" cũ, hiện tab Mẫu 5;
        /// khác "1": hiện tab cũ, ẩn tab Mẫu 5. Gọi 1 lần khi mở form (sau HisConfigCFG.LoadConfig).
        /// </summary>
        private void ApplyDriverTabVisibility()
        {
            try
            {
                bool useForm5 = HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config.HisConfigCFG.IsUseKskDriverForm5;
                this.xtraTabPage4.PageVisible = !useForm5;
                this.xtraTabPage18.PageVisible = useForm5;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Tab mặc định rơi vào tab lái xe đang bị ẩn thì chuyển sang tab lái xe đang hiện
        /// (tab 3 = "Ksk lái xe" cũ, HIS_KSK_PERIOD_DRIVER).
        /// </summary>
        private int MapToVisibleDriverTab(int tab)
        {
            bool useForm5 = HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config.HisConfigCFG.IsUseKskDriverForm5;
            if (useForm5 && tab == 3) return TAB_PRO_DRIVER;
            if (!useForm5 && tab == TAB_PRO_DRIVER) return 3;
            return tab;
        }

        #endregion

        #region Khởi tạo / nạp tab

        private void InitTabProDriver()
        {
            if (isProDriverTabInited) return;
            isProDriverTabInited = true;
            try
            {
                this.ucKskProDriver.InitUc(this.historyIcdDataSource, this.historyIcdPageSize);
                InitAdminCombosExt(this.ucKskProDriver.CboObject, this.ucKskProDriver.CboPaymentSource);
                RegisterConcluderCombo(TAB_PRO_DRIVER, this.ucKskProDriver.CboConcluder);
                foreach (var cbo in this.ucKskProDriver.DoctorCombos)
                {
                    SetDataCboExamLoginName(cbo);
                    cbo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.ClearData_ButtonClick);
                }
                var licenseClasses = BackendDataWorker.Get<HIS_LICENSE_CLASS>(false, true)
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var columns = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columns.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("LICENSE_CLASS_NAME", "", 250, 0));
                var ado = new Inventec.Common.Controls.EditorLoader.ControlEditorADO("LICENSE_CLASS_NAME", "ID", columns, false, 250);
                Inventec.Common.Controls.EditorLoader.ControlEditorLoader.Load(this.ucKskProDriver.CboLicenseClass, licenseClasses, ado);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Nạp tab khi mở lần đầu / đổi y lệnh — gọi từ EnsureTabLoaded.</summary>
        private void LoadTabProDriver()
        {
            try
            {
                InitTabProDriver();
                ClearTabInputEditors(TAB_PRO_DRIVER);
                SetObjectValueExt(this.ucKskProDriver.CboObject, null);
                this.ucKskProDriver.CboPaymentSource.EditValue = null;
                LoadAdministrativeInfoProDriver();
                LoadKskProDriverRecord();
                FillDataProDriver();
                LoadConcluderComboExt();
                DateTime conclusionTime = DateTime.Now;
                if (currentKskGeneral != null && currentKskGeneral.CONCLUSION_TIME.HasValue && currentKskGeneral.CONCLUSION_TIME > 0)
                    conclusionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentKskGeneral.CONCLUSION_TIME.Value) ?? DateTime.Now;
                this.ucKskProDriver.DteConclusionTime.DateTime = conclusionTime;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private static bool IsMau5(HIS_KSK_PRO_DRIVER d)
        {
            return d != null && d.KSK_FORM_TYPE == KSK_FORM_TYPE__MAU5;
        }

        /// <summary>
        /// Bản ghi Mẫu 5 của y lệnh (từ dữ liệu tải sẵn). Chưa có thì lấy Mẫu 6 để điền sẵn:
        /// ưu tiên cùng y lệnh, sau đó bản sửa gần nhất trong cùng hồ sơ.
        /// </summary>
        private void LoadKskProDriverRecord()
        {
            try
            {
                currentKskProDriver = null;
                prefillKskProDriver = null;
                proDriverDitys = null;
                if (currentServiceReq == null || preKskProDrivers == null) return;
                long srId = currentServiceReq.ID;

                currentKskProDriver = preKskProDrivers.FirstOrDefault(o => o != null && o.SERVICE_REQ_ID == srId && IsMau5(o));
                if (currentKskProDriver != null)
                {
                    proDriverDitys = DitysOfProDriver(currentKskProDriver.ID);
                    return;
                }

                HIS_KSK_PRO_DRIVER mau6 = preKskProDrivers.Where(o => o != null && !IsMau5(o))
                    .OrderByDescending(o => o.SERVICE_REQ_ID == srId)
                    .ThenByDescending(o => o.MODIFY_TIME ?? o.CREATE_TIME ?? 0)
                    .FirstOrDefault();
                if (mau6 != null)
                {
                    prefillKskProDriver = CopyProDriverForPrefill(mau6);
                    proDriverDitys = DitysOfProDriver(mau6.ID);
                    LogSystem.Info("KskProDriver: chua co Mau 5 -> dien san tu Mau 6 ID=" + mau6.ID + " (SERVICE_REQ_ID=" + mau6.SERVICE_REQ_ID + ")");
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private List<HIS_PERIOD_DRIVER_DITY> DitysOfProDriver(long proDriverId)
        {
            return preDitysProDriver != null
                ? preDitysProDriver.Where(o => o != null && o.KSK_PRO_DRIVER_ID == proDriverId).ToList()
                : null;
        }

        /// <summary>Bản sao Mẫu 6 để điền sẵn Mẫu 5 — bỏ khóa, số giấy, sinh hiệu, đề nghị/kết luận.</summary>
        private static HIS_KSK_PRO_DRIVER CopyProDriverForPrefill(HIS_KSK_PRO_DRIVER src)
        {
            HIS_KSK_PRO_DRIVER d = new HIS_KSK_PRO_DRIVER();
            foreach (PropertyInfo pi in typeof(HIS_KSK_PRO_DRIVER).GetProperties())
            {
                if (!pi.CanRead || !pi.CanWrite || PRO_DRIVER_PREFILL_SKIP.Contains(pi.Name)) continue;
                Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal))
                    pi.SetValue(d, pi.GetValue(src, null), null);
            }
            return d;
        }

        /// <summary>
        /// Đổ bản ghi lên tab. Thể lực: bản đã lưu theo DHST_ID; chưa có thì lấy giá trị (không lấy ID) từ
        /// Mẫu 6 đang điền sẵn, rồi tới sinh hiệu của y lệnh — lưu lần đầu tạo sinh hiệu riêng cho Mẫu 5.
        /// </summary>
        private void FillDataProDriver()
        {
            try
            {
                dhstProDriver = null;
                HIS_DHST dhstShow = null;
                HIS_KSK_PRO_DRIVER show = currentKskProDriver ?? prefillKskProDriver;
                if (currentKskProDriver != null && currentKskProDriver.DHST_ID.HasValue && currentKskProDriver.DHST_ID > 0)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentKskProDriver.DHST_ID);
                    dhstProDriver = (ds != null && ds.Count > 0) ? ds[0] : null;
                    dhstShow = dhstProDriver;
                }
                if (dhstShow == null && prefillKskProDriver != null)
                {
                    HIS_KSK_PRO_DRIVER mau6 = preKskProDrivers != null
                        ? preKskProDrivers.FirstOrDefault(o => o != null && !IsMau5(o) && o.DHST_ID.HasValue && o.DHST_ID > 0)
                        : null;
                    List<HIS_DHST> ds = mau6 != null ? PreGetDhst(mau6.DHST_ID) : null;
                    dhstShow = (ds != null && ds.Count > 0) ? ds[0] : null;
                }
                if (dhstShow == null && currentServiceReq != null)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentServiceReq.DHST_ID);
                    dhstShow = (ds != null && ds.Count > 0) ? ds[0] : null;
                }

                this.ucKskProDriver.FillData(show, dhstShow);
                this.ucKskProDriver.CboLicenseClass.EditValue = show != null ? (object)show.LICENSE_CLASS_ID : null;
                SetObjectValueExt(this.ucKskProDriver.CboObject, show != null ? show.KSK_PATIENT_TYPES : null);
                this.ucKskProDriver.CboPaymentSource.EditValue = (show != null && show.KSK_PAY_SOURCE != null)
                    ? (object)show.KSK_PAY_SOURCE : null;
                this.ucKskProDriver.SetDityRows(BuildProDriverDityRows());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>22 dòng theo danh mục HIS_DISEASE_TYPE (IS_KSK_PERIOD_DRIVER), sắp theo mã SỐ (mã "10" sau "9").</summary>
        private List<ProDriverDityRow> BuildProDriverDityRows()
        {
            var rows = new List<ProDriverDityRow>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDiseaseTypeFilter filter = new MOS.Filter.HisDiseaseTypeFilter();
                filter.IS_ACTIVE = 1;
                filter.IS_KSK_PERIOD_DRIVER = 1;
                var types = new BackendAdapter(param).Get<List<HIS_DISEASE_TYPE>>("api/HisDiseaseType/Get", ApiConsumers.MosConsumer, filter, param);
                if (types == null) return rows;
                bool hasSaved = proDriverDitys != null && proDriverDitys.Count > 0;
                foreach (var t in types.OrderBy(o => CodeOrder(o.DISEASE_TYPE_CODE)).ThenBy(o => o.DISEASE_TYPE_CODE))
                {
                    var row = new ProDriverDityRow { ID = t.ID, DISEASE_TYPE_CODE = t.DISEASE_TYPE_CODE, DISEASE_TYPE_NAME = t.DISEASE_TYPE_NAME };
                    var saved = hasSaved ? proDriverDitys.FirstOrDefault(o => o.DISEASE_TYPE_ID == t.ID) : null;
                    if (saved != null)
                    {
                        row.IS_YES = saved.IS_YES_NO == "1";
                        row.IS_NO = saved.IS_YES_NO == "0";
                    }
                    else if (!hasSaved)
                    {
                        row.IS_NO = true; // hồ sơ mới: mặc định "Không" như tab Ksk lái xe cũ
                    }
                    rows.Add(row);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return rows;
        }

        private static long CodeOrder(string code)
        {
            long n;
            return long.TryParse((code ?? "").Trim(), out n) ? n : long.MaxValue;
        }

        /// <summary>Hành chính chỉ đọc: ngày/nơi cấp giấy tờ (CCCD → hộ chiếu → CMND), dân tộc, nơi ở, SĐT.</summary>
        private void LoadAdministrativeInfoProDriver()
        {
            try
            {
                if (currentServiceReq == null)
                {
                    this.ucKskProDriver.SetAdministrativeInfo(null, null, null, null, null);
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
                string idDateText = (idDate.HasValue && idDate.Value > 0) ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(idDate.Value) : null;
                string residence = BuildResidence(currentServiceReq);
                if (string.IsNullOrWhiteSpace(residence) && t != null) residence = t.TDL_PATIENT_ADDRESS;
                string phone = t != null ? (!string.IsNullOrWhiteSpace(t.TDL_PATIENT_MOBILE) ? t.TDL_PATIENT_MOBILE : t.TDL_PATIENT_PHONE) : null;
                this.ucKskProDriver.SetAdministrativeInfo(idDateText, idPlace, t != null ? t.TDL_PATIENT_ETHNIC_NAME : null, residence, phone);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region Lưu

        /// <summary>
        /// Gom các ô của tab thành bản ghi sắp lưu — bắt đầu từ bản đã lưu (hoặc bản điền sẵn từ Mẫu 6)
        /// vì BE cập nhật thay cả dòng: cột tab không hiển thị vẫn giữ nguyên.
        /// </summary>
        private HIS_KSK_PRO_DRIVER BuildKskProDriverFromControls()
        {
            HIS_KSK_PRO_DRIVER d = new HIS_KSK_PRO_DRIVER();
            try
            {
                HIS_KSK_PRO_DRIVER src = currentKskProDriver ?? prefillKskProDriver;
                if (src != null)
                {
                    foreach (PropertyInfo pi in typeof(HIS_KSK_PRO_DRIVER).GetProperties())
                    {
                        if (!pi.CanRead || !pi.CanWrite) continue;
                        Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                        if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal))
                            pi.SetValue(d, pi.GetValue(src, null), null);
                    }
                }
                d.KSK_FORM_TYPE = KSK_FORM_TYPE__MAU5;
                if (currentServiceReq != null)
                {
                    d.SERVICE_REQ_ID = currentServiceReq.ID;
                    d.TDL_TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    d.TDL_PATIENT_ID = currentServiceReq.TDL_PATIENT_ID;
                }
                this.ucKskProDriver.GetData(d);
                object lc = this.ucKskProDriver.CboLicenseClass.EditValue;
                d.LICENSE_CLASS_ID = lc != null ? (long?)Convert.ToInt64(lc) : null;
                string types = GetObjectValueExt(this.ucKskProDriver.CboObject);
                d.KSK_PATIENT_TYPES = !string.IsNullOrEmpty(types) ? types : null;
                d.KSK_PAY_SOURCE = this.ucKskProDriver.CboPaymentSource.EditValue != null
                    ? (short?)Convert.ToInt16(this.ucKskProDriver.CboPaymentSource.EditValue) : null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return d;
        }

        /// <summary>Thể lực sắp lưu: sửa trên bản đã lưu (giữ ID), chưa có thì tạo mới. Không nhập gì → null.</summary>
        private HIS_DHST BuildDhstProDriver()
        {
            HIS_DHST dhst = new HIS_DHST();
            if (dhstProDriver != null)
            {
                foreach (PropertyInfo pi in typeof(HIS_DHST).GetProperties())
                {
                    if (!pi.CanRead || !pi.CanWrite) continue;
                    Type t = Nullable.GetUnderlyingType(pi.PropertyType) ?? pi.PropertyType;
                    if (t.IsPrimitive || t == typeof(string) || t == typeof(decimal)) pi.SetValue(dhst, pi.GetValue(dhstProDriver, null), null);
                }
            }
            this.ucKskProDriver.GetDhst(dhst);
            if (!HasDhstData(dhst)) return null;
            if (!dhst.EXECUTE_TIME.HasValue || dhst.EXECUTE_TIME <= 0)
                dhst.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
            return dhst;
        }

        /// <summary>22 câu trả lời tiền sử theo lưới — BE xóa câu cũ của giấy rồi tạo lại, gắn KSK_PRO_DRIVER_ID.</summary>
        private List<HIS_PERIOD_DRIVER_DITY> BuildProDriverDitys()
        {
            var list = new List<HIS_PERIOD_DRIVER_DITY>();
            foreach (ProDriverDityRow row in this.ucKskProDriver.GetDityRows())
            {
                HIS_PERIOD_DRIVER_DITY d = new HIS_PERIOD_DRIVER_DITY();
                d.DISEASE_TYPE_ID = row.ID;
                d.IS_YES_NO = row.IS_YES ? "1" : (row.IS_NO ? "0" : null);
                list.Add(d);
            }
            return list;
        }

        /// <summary>Nhánh KskProDriver của KskExecuteV2 (bản ghi Mẫu 5 + thể lực + 22 dòng tiền sử).</summary>
        private MOS.SDO.KskProDriver2SDO BuildKskProDriverSdo()
        {
            MOS.SDO.KskProDriver2SDO sdo = new MOS.SDO.KskProDriver2SDO();
            sdo.HisKskProDriver = BuildKskProDriverFromControls();
            sdo.HisDhst = BuildDhstProDriver();
            sdo.HisPeriodDriverDitys = BuildProDriverDitys();
            return sdo;
        }

        /// <summary>Kiểm tra trước khi Lưu tab Mẫu 5 (cùng điều kiện BE chặn, báo sớm cho rõ).</summary>
        private bool ValidateProDriverBeforeSave()
        {
            if (!this.ucKskProDriver.ValidateReexamNote())
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Kết luận \"Đạt tiêu chuẩn sức khỏe nhưng yêu cầu khám lại\": chưa nhập thời gian khám lại.",
                    "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>Sau khi KskExecuteV2 thành công: giữ bản ghi vừa lưu + cập nhật dữ liệu tải sẵn để mở lại tab đúng.</summary>
        private void ApplyKskProDriverSaved(HIS_KSK_PRO_DRIVER saved, MOS.SDO.KskProDriver2SDO sent)
        {
            try
            {
                if (saved == null) return;
                currentKskProDriver = saved;
                prefillKskProDriver = null;
                // BE không trả HIS_DHST: lấy ID từ bản ghi vừa lưu để lần Lưu sau cập nhật đúng sinh hiệu, không tạo mới
                if (sent != null && sent.HisDhst != null && saved.DHST_ID.HasValue && saved.DHST_ID > 0)
                {
                    sent.HisDhst.ID = saved.DHST_ID.Value;
                    dhstProDriver = sent.HisDhst;
                }

                if (preKskProDrivers == null) preKskProDrivers = new List<HIS_KSK_PRO_DRIVER>();
                preKskProDrivers.RemoveAll(o => o != null && o.ID == saved.ID);
                preKskProDrivers.Add(saved);

                var ditys = sent != null && sent.HisPeriodDriverDitys != null ? sent.HisPeriodDriverDitys : new List<HIS_PERIOD_DRIVER_DITY>();
                ditys.ForEach(o => o.KSK_PRO_DRIVER_ID = saved.ID);
                if (preDitysProDriver == null) preDitysProDriver = new List<HIS_PERIOD_DRIVER_DITY>();
                preDitysProDriver.RemoveAll(o => o != null && o.KSK_PRO_DRIVER_ID == saved.ID);
                preDitysProDriver.AddRange(ditys);
                proDriverDitys = ditys;

                if (sent != null && sent.HisDhst != null && sent.HisDhst.ID > 0)
                {
                    if (preDhstById == null) preDhstById = new Dictionary<long, HIS_DHST>();
                    preDhstById[sent.HisDhst.ID] = sent.HisDhst;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        #endregion

        #region In Mps000524

        /// <summary>
        /// Dựng dữ liệu in Mẫu 5 (Mps000524): bản ghi theo các ô đang hiển thị (đã lưu thì giữ ID/số giấy),
        /// thể lực theo các ô, 22 dòng tiền sử theo lưới, người/ngày kết luận theo ô của tab.
        /// </summary>
        private void LoadBieuMauPhieuMps000524(string printTypeCode, string fileName, ref bool result)
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

                HIS_KSK_PRO_DRIVER record = BuildKskProDriverFromControls();
                if (currentKskProDriver == null) record.KSK_PRO_DRIVER_CODE = null; // điền sẵn từ Mẫu 6: chưa có số giấy Mẫu 5
                HIS_DHST dhst = new HIS_DHST();
                this.ucKskProDriver.GetDhst(dhst);

                HIS_KSK_GENERAL general = BuildKskGeneralForMental();
                object login = this.ucKskProDriver.CboConcluder.EditValue;
                general.CONCLUDER_LOGINNAME = login != null && !string.IsNullOrWhiteSpace(login.ToString()) ? login.ToString() : null;
                general.CONCLUDER_USERNAME = !string.IsNullOrEmpty(general.CONCLUDER_LOGINNAME)
                    ? BackendDataWorker.Get<V_HIS_EMPLOYEE>().Where(o => o.LOGINNAME == general.CONCLUDER_LOGINNAME).Select(o => o.TDL_USERNAME).FirstOrDefault()
                    : null;
                if (this.ucKskProDriver.DteConclusionTime.EditValue != null)
                    general.CONCLUSION_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.ucKskProDriver.DteConclusionTime.DateTime);

                var histories = this.ucKskProDriver.GetDityRows().Select(o => new MPS.Processor.Mps000524.PDO.Mps000524HistoryADO
                {
                    DISEASE_TYPE_CODE = o.DISEASE_TYPE_CODE,
                    DISEASE_TYPE_NAME = o.DISEASE_TYPE_NAME,
                    IS_YES_NO = o.IS_YES ? "1" : (o.IS_NO ? "0" : null)
                }).ToList();

                // Họ tên mọi bác sĩ có trong hồ sơ (cột *_LOGINNAME)
                var logins = typeof(HIS_KSK_PRO_DRIVER).GetProperties()
                    .Where(p => p.Name.EndsWith("_LOGINNAME") && p.PropertyType == typeof(string))
                    .Select(p => (string)p.GetValue(record, null)).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
                var employeeNames = BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                    .Where(o => o.LOGINNAME != null && logins.Contains(o.LOGINNAME))
                    .GroupBy(o => o.LOGINNAME).ToDictionary(g => g.Key, g => g.First().TDL_USERNAME);

                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                var rdo = new MPS.Processor.Mps000524.PDO.Mps000524PDO(currentServiceReq, record, general, dhst, treatment,
                    BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>());
                rdo.KskServiceReq = printKskServiceReq;
                rdo.KskPatient = printKskPatient;
                rdo.PersonalHistories = histories;
                rdo.EmployeeNames = employeeNames;
                rdo.LicenseClassName = this.ucKskProDriver.CboLicenseClass.EditValue != null ? this.ucKskProDriver.CboLicenseClass.Text : null;
                PrintData(printTypeCode, fileName, rdo, ref result);
            }
            catch (Exception ex)
            {
                Inventec.Desktop.Common.Message.WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
