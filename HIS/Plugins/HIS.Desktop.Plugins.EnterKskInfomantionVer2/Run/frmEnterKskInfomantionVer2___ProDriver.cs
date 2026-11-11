/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * Tab "Ksk lái xe (Mẫu 5)" — Giấy khám sức khỏe của người lái xe, người điều khiển xe máy chuyên dùng,
 * Mẫu 5 Thông tư 36/2024/TT-BYT (BV Đa khoa Xuyên Á).
 *
 * Lưu / nạp:
 *  - Dữ liệu khám -> HIS_KSK_PRO_DRIVER qua dịch vụ CRUD riêng api/HisKskProDriver/Get|Create|Update
 *    (cùng cách bảng mẫu M4 HIS_KSK_SYT_HCM), gọi NGAY SAU KskExecuteV2.
 *  - Thể lực -> HIS_DHST (api/HisDhst/Create|Update), khóa DHST_ID trên bản ghi.
 *  - Người kết luận + ngày kết luận -> HIS_KSK_GENERAL của lượt khám qua KskExecuteV2 (như tab Ksk tâm thần).
 *  - 22 dòng tiền sử bản thân -> HIS_PERIOD_DRIVER_DITY theo KSK_PRO_DRIVER_ID: CHỈ bật khi MOS.EFMODEL
 *    có cột này (BE đang bổ sung); chưa có thì bảng vẫn nhập/in được nhưng chưa lưu.
 * Bản ghi dùng lớp tạm KskProDriverADO (đúng tên cột DB) vì MOS.EFMODEL chưa có HIS_KSK_PRO_DRIVER.
 * In: Mps000524 (LoadBieuMauPhieuMps000524) — cần SAR_PRINT_TYPE 'Mps000524' + mẫu trong Tmp\Mps\Mps000524\.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO;
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

        private const string API__PRO_DRIVER_GET = "api/HisKskProDriver/Get";
        private const string API__PRO_DRIVER_CREATE = "api/HisKskProDriver/Create";
        private const string API__PRO_DRIVER_UPDATE = "api/HisKskProDriver/Update";
        private const string API__PERIOD_DRIVER_DITY_GET = "api/HisPeriodDriverDity/Get";
        private const string API__PERIOD_DRIVER_DITY_CREATE = "api/HisPeriodDriverDity/Create";
        private const string API__PERIOD_DRIVER_DITY_UPDATE = "api/HisPeriodDriverDity/Update";
        /// <summary>Cột liên kết 22 dòng tiền sử với bản ghi Mẫu 5 (BE đang bổ sung vào HIS_PERIOD_DRIVER_DITY).</summary>
        private const string DITY_LINK_COLUMN = "KSK_PRO_DRIVER_ID";

        private bool isProDriverTabInited = false;
        /// <summary>Bản ghi Mẫu 5 của y lệnh đang mở (null = chưa có).</summary>
        private KskProDriverADO currentKskProDriver;
        /// <summary>Sinh hiệu của bản ghi — giữ ID để lần Lưu sau cập nhật, không tạo mới.</summary>
        private HIS_DHST dhstProDriver;
        /// <summary>22 dòng tiền sử đã lưu (theo KSK_PRO_DRIVER_ID).</summary>
        private List<HIS_PERIOD_DRIVER_DITY> proDriverDitys;

        /// <summary>Lọc theo y lệnh — lớp tạm vì MOS.Filter chưa có HisKskProDriverFilter.</summary>
        private class KskProDriverFilterADO
        {
            public long? SERVICE_REQ_ID { get; set; }
            public bool? IS_ACTIVE { get; set; }
        }

        private class ProDriverDityFilterADO
        {
            public long? KSK_PRO_DRIVER_ID { get; set; }
        }

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
                long? ct = currentKskProDriver != null ? currentKskProDriver.CONCLUSION_TIME : null;
                if ((!ct.HasValue || ct <= 0) && currentKskGeneral != null) ct = currentKskGeneral.CONCLUSION_TIME;
                if (ct.HasValue && ct.Value > 0)
                    conclusionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ct.Value) ?? DateTime.Now;
                this.ucKskProDriver.DteConclusionTime.DateTime = conclusionTime;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Bản ghi Mẫu 5 của y lệnh: api/HisKskProDriver/Get theo SERVICE_REQ_ID.</summary>
        private void LoadKskProDriverRecord()
        {
            try
            {
                currentKskProDriver = null;
                proDriverDitys = null;
                if (currentServiceReq == null) return;
                CommonParam param = new CommonParam();
                var filter = new KskProDriverFilterADO { SERVICE_REQ_ID = currentServiceReq.ID };
                var list = new BackendAdapter(param).Get<List<KskProDriverADO>>(API__PRO_DRIVER_GET, ApiConsumers.MosConsumer, filter, param);
                // Phòng khi dịch vụ chưa hỗ trợ lọc: chỉ nhận bản ghi đúng y lệnh.
                currentKskProDriver = list != null ? list.FirstOrDefault(o => o != null && o.SERVICE_REQ_ID == currentServiceReq.ID) : null;
                if (currentKskProDriver != null && IsDityLinkSupported())
                {
                    var dFilter = new ProDriverDityFilterADO { KSK_PRO_DRIVER_ID = currentKskProDriver.ID };
                    var ds = new BackendAdapter(param).Get<List<HIS_PERIOD_DRIVER_DITY>>(API__PERIOD_DRIVER_DITY_GET, ApiConsumers.MosConsumer, dFilter, param);
                    proDriverDitys = ds != null ? ds.Where(o => DityLinkValue(o) == currentKskProDriver.ID).ToList() : null;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Đổ bản ghi lên tab. Chưa có bản ghi: để trống, thể lực lấy sẵn từ sinh hiệu của y lệnh
        /// (chỉ lấy giá trị, không lấy ID: lưu lần đầu tạo sinh hiệu riêng, không sửa bản ghi của màn khác).
        /// </summary>
        private void FillDataProDriver()
        {
            try
            {
                dhstProDriver = null;
                HIS_DHST dhstShow = null;
                if (currentKskProDriver != null && currentKskProDriver.DHST_ID.HasValue && currentKskProDriver.DHST_ID > 0)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentKskProDriver.DHST_ID);
                    dhstProDriver = (ds != null && ds.Count > 0) ? ds[0] : null;
                    dhstShow = dhstProDriver;
                }
                else if (currentServiceReq != null)
                {
                    List<HIS_DHST> ds = PreGetDhst(currentServiceReq.DHST_ID);
                    dhstShow = (ds != null && ds.Count > 0) ? ds[0] : null;
                }

                this.ucKskProDriver.FillData(currentKskProDriver, dhstShow);
                this.ucKskProDriver.CboLicenseClass.EditValue = currentKskProDriver != null ? (object)currentKskProDriver.LICENSE_CLASS_ID : null;
                SetObjectValueExt(this.ucKskProDriver.CboObject, currentKskProDriver != null ? currentKskProDriver.KSK_PATIENT_TYPES : null);
                this.ucKskProDriver.CboPaymentSource.EditValue = (currentKskProDriver != null && currentKskProDriver.KSK_PAY_SOURCE != null)
                    ? (object)currentKskProDriver.KSK_PAY_SOURCE : null;
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
                foreach (var t in types.OrderBy(o => CodeOrder(o.DISEASE_TYPE_CODE)).ThenBy(o => o.DISEASE_TYPE_CODE))
                {
                    var row = new ProDriverDityRow { ID = t.ID, DISEASE_TYPE_CODE = t.DISEASE_TYPE_CODE, DISEASE_TYPE_NAME = t.DISEASE_TYPE_NAME };
                    var saved = proDriverDitys != null ? proDriverDitys.FirstOrDefault(o => o.DISEASE_TYPE_ID == t.ID) : null;
                    if (saved != null)
                    {
                        row.DITY_ID = saved.ID;
                        row.IS_YES = saved.IS_YES_NO == "1";
                        row.IS_NO = saved.IS_YES_NO == "0";
                    }
                    else if (currentKskProDriver == null)
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

        /// <summary>Gom các ô của tab thành bản ghi sắp lưu — bắt đầu từ bản đã lưu (dịch vụ cập nhật thay cả dòng).</summary>
        private KskProDriverADO BuildKskProDriverFromControls()
        {
            KskProDriverADO d = new KskProDriverADO();
            try
            {
                if (currentKskProDriver != null)
                {
                    foreach (PropertyInfo pi in typeof(KskProDriverADO).GetProperties())
                        if (pi.CanRead && pi.CanWrite) pi.SetValue(d, pi.GetValue(currentKskProDriver, null), null);
                }
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

        /// <summary>
        /// Lưu bản ghi Mẫu 5 (+ sinh hiệu, + 22 dòng tiền sử nếu BE đã có cột liên kết).
        /// Gọi NGAY SAU KskExecuteV2 thành công. Lỗi thì báo rõ — im lặng thì người dùng tưởng đã lưu đủ.
        /// </summary>
        private void SaveKskProDriver()
        {
            try
            {
                if (xtraTabControl1.SelectedTabPageIndex != TAB_PRO_DRIVER || currentServiceReq == null) return;
                CommonParam param = new CommonParam();
                KskProDriverADO data = BuildKskProDriverFromControls();

                // 1. Sinh hiệu
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
                if (HasDhstData(dhst))
                {
                    dhst.TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                    if (!dhst.EXECUTE_TIME.HasValue || dhst.EXECUTE_TIME <= 0)
                        dhst.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    if (!dhst.EXECUTE_ROOM_ID.HasValue && currentModule != null) dhst.EXECUTE_ROOM_ID = currentModule.RoomId;
                    HIS_DHST savedDhst = dhst.ID > 0
                        ? new BackendAdapter(param).Post<HIS_DHST>("api/HisDhst/Update", ApiConsumers.MosConsumer, dhst, param)
                        : new BackendAdapter(param).Post<HIS_DHST>("api/HisDhst/Create", ApiConsumers.MosConsumer, dhst, param);
                    if (savedDhst != null)
                    {
                        dhstProDriver = savedDhst;
                        data.DHST_ID = savedDhst.ID;
                    }
                }

                // 2. Bản ghi Mẫu 5
                KskProDriverADO saved = data.ID > 0
                    ? new BackendAdapter(param).Post<KskProDriverADO>(API__PRO_DRIVER_UPDATE, ApiConsumers.MosConsumer, data, param)
                    : new BackendAdapter(param).Post<KskProDriverADO>(API__PRO_DRIVER_CREATE, ApiConsumers.MosConsumer, data, param);
                if (saved == null)
                {
                    LogSystem.Warn("KskProDriver: LUU THAT BAI " + LogUtil.TraceData("param", param));
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        "Đã lưu người kết luận, nhưng dữ liệu Giấy khám sức khỏe lái xe (Mẫu 5) chưa lưu được."
                        + "\r\nVui lòng bấm Lưu lại; nếu vẫn lỗi, báo bộ phận kỹ thuật.",
                        "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                    return;
                }
                currentKskProDriver = saved;

                // 3. 22 dòng tiền sử
                SaveProDriverDitys(saved.ID);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void SaveProDriverDitys(long proDriverId)
        {
            try
            {
                if (!IsDityLinkSupported())
                {
                    LogSystem.Warn("KskProDriver: MOS.EFMODEL chua co HIS_PERIOD_DRIVER_DITY." + DITY_LINK_COLUMN + " -> chua luu 22 dong tien su");
                    return;
                }
                PropertyInfo link = typeof(HIS_PERIOD_DRIVER_DITY).GetProperty(DITY_LINK_COLUMN);
                CommonParam param = new CommonParam();
                var saved = new List<HIS_PERIOD_DRIVER_DITY>();
                foreach (ProDriverDityRow row in this.ucKskProDriver.GetDityRows())
                {
                    HIS_PERIOD_DRIVER_DITY d = new HIS_PERIOD_DRIVER_DITY();
                    d.ID = row.DITY_ID;
                    d.DISEASE_TYPE_ID = row.ID;
                    d.IS_YES_NO = row.IS_YES ? "1" : (row.IS_NO ? "0" : null);
                    link.SetValue(d, (long?)proDriverId, null);
                    HIS_PERIOD_DRIVER_DITY r = d.ID > 0
                        ? new BackendAdapter(param).Post<HIS_PERIOD_DRIVER_DITY>(API__PERIOD_DRIVER_DITY_UPDATE, ApiConsumers.MosConsumer, d, param)
                        : new BackendAdapter(param).Post<HIS_PERIOD_DRIVER_DITY>(API__PERIOD_DRIVER_DITY_CREATE, ApiConsumers.MosConsumer, d, param);
                    if (r != null) { row.DITY_ID = r.ID; saved.Add(r); }
                }
                proDriverDitys = saved;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>MOS.EFMODEL đang chạy đã có cột liên kết 22 dòng tiền sử với bản ghi Mẫu 5 chưa.</summary>
        private static bool IsDityLinkSupported()
        {
            return typeof(HIS_PERIOD_DRIVER_DITY).GetProperty(DITY_LINK_COLUMN) != null;
        }

        private static long? DityLinkValue(HIS_PERIOD_DRIVER_DITY d)
        {
            PropertyInfo pi = typeof(HIS_PERIOD_DRIVER_DITY).GetProperty(DITY_LINK_COLUMN);
            object v = pi != null ? pi.GetValue(d, null) : null;
            return v != null ? (long?)Convert.ToInt64(v) : null;
        }

        #endregion

        #region In Mps000524

        /// <summary>
        /// Dựng dữ liệu in Mẫu 5 (Mps000524): bản ghi theo các ô đang hiển thị (đã lưu thì giữ ID/mã),
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

                KskProDriverADO record = BuildKskProDriverFromControls();
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
                var logins = typeof(KskProDriverADO).GetProperties()
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
