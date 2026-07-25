/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    /// <summary>
    /// Tab "D. Khám cận lâm sàng" (UI dựng trong Designer): 4 memo lab + nút "+" mở ContentSubclinical,
    /// kết luận ICD-10 (nhúng UcKskConclusionIcd — mã + tên, nhiều chẩn đoán), Tư vấn, Bác sĩ khám.
    /// </summary>
    public partial class frmEnterKskInfomantionQD831
    {
        private MemoEdit _clsTargetMemo;
        private List<V_HIS_EMPLOYEE> cachedEmpListFull;
        private List<V_HIS_EMPLOYEE> cachedEmpListOnlyLogin;
        private SecondaryIcdProcessor subIcdProcessorCls;
        private UserControl ucClsSecondaryIcd;
        private static List<HIS_ICD> _icdOrderedCache;

        /// <summary>Khởi tạo tab Khám cận lâm sàng: nhúng UC ICD, nối nút "+", nạp combo Bác sĩ khám.</summary>
        private void InitKhamClsTab()
        {
            try
            {
                // Chẩn đoán ICD-10: nhúng UC SecondaryIcd (F1 chọn bệnh, hiện mã + tên) giống "mã ICD bản thân" bên KSK >18t.
                // HIS_ICD rất lớn -> sắp xếp 1 lần rồi cache tĩnh (tránh OrderBy/ToList mỗi lần mở form).
                if (_icdOrderedCache == null)
                {
                    var allIcd = BackendDataWorker.Get<HIS_ICD>();
                    _icdOrderedCache = allIcd != null ? allIcd.OrderBy(o => o.ICD_CODE).ToList() : new List<HIS_ICD>();
                }
                subIcdProcessorCls = new SecondaryIcdProcessor(new CommonParam(), _icdOrderedCache);
                SecondaryIcdInitADO icdAdo = new SecondaryIcdInitADO();
                icdAdo.Width = (this.pnlClsIcd.Width > 0) ? this.pnlClsIcd.Width : 400;
                icdAdo.Height = 24;
                icdAdo.TextLblIcd = "CĐ:";
                icdAdo.TextSize = 30;
                icdAdo.TextNullValue = "Nhấn F1 để chọn bệnh";
                icdAdo.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucClsSecondaryIcd = (UserControl)subIcdProcessorCls.Run(icdAdo);
                if (ucClsSecondaryIcd != null)
                {
                    this.pnlClsIcd.Controls.Add(ucClsSecondaryIcd);
                    ucClsSecondaryIcd.Dock = DockStyle.Fill;
                }
                this.btnClsIcdChoose.Click += BtnClsIcdChoose_Click;

                // 4 nút "+" — mở chức năng chọn kết quả CLS (ContentSubclinical), điền về memo tương ứng.
                this.btnClsHuyetHoc.Tag = this.memoClsHuyetHoc;
                this.btnClsSinhHoaMau.Tag = this.memoClsSinhHoaMau;
                this.btnClsSinhHoaNuocTieu.Tag = this.memoClsSinhHoaNuocTieu;
                this.btnClsSieuAmOB.Tag = this.memoClsSieuAmOB;
                this.btnClsHuyetHoc.Click += ClsPickResult_Click;
                this.btnClsSinhHoaMau.Click += ClsPickResult_Click;
                this.btnClsSinhHoaNuocTieu.Click += ClsPickResult_Click;
                this.btnClsSieuAmOB.Click += ClsPickResult_Click;

                // Bác sĩ khám — nạp danh sách nhân viên (giống combo "Người khám" bên EnterKskV2).
                SetDataCboExamLoginName(this.cboClsBacSiKham);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void ClsPickResult_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = sender as SimpleButton;
                _clsTargetMemo = btn != null ? btn.Tag as MemoEdit : null;
                if (_clsTargetMemo == null) return;
                OpenContentSubclinical();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Nút "..." — mở popup chọn NHIỀU chẩn đoán ICD-10 (frmSubIcd), đổ về ô CĐ.</summary>
        private void BtnClsIcdChoose_Click(object sender, EventArgs e)
        {
            try
            {
                string subCode = "", text = "";
                var cur = (subIcdProcessorCls != null && ucClsSecondaryIcd != null)
                    ? subIcdProcessorCls.GetValue(ucClsSecondaryIcd) as SecondaryIcdDataADO : null;
                if (cur != null) { subCode = cur.ICD_SUB_CODE ?? ""; text = cur.ICD_TEXT ?? ""; }
                int pageSize = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                frmSubIcd frm = new frmSubIcd(
                    new DelegateRefeshIcdChandoanphu(ClsDlgChooseIcd),
                    subCode, text, pageSize, new List<HIS_ICD>());
                frm.ShowDialog();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private void ClsDlgChooseIcd(string icdCodes, string icdNames)
        {
            try
            {
                if (subIcdProcessorCls == null || ucClsSecondaryIcd == null) return;
                SecondaryIcdDataADO data = new SecondaryIcdDataADO();
                data.ICD_SUB_CODE = icdCodes;
                data.ICD_TEXT = icdNames;
                subIcdProcessorCls.Reload(ucClsSecondaryIcd, data);
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Mở plugin HIS.Desktop.Plugins.ContentSubclinical để chọn kết quả CLS (trả về chuỗi).</summary>
        private void OpenContentSubclinical()
        {
            try
            {
                if (currentServiceReq == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null)
                {
                    LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                    return;
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentServiceReq.TREATMENT_ID);
                    listArgs.Add(false); // ReturnObject = false -> plugin trả về String
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinicalCls);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                        HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Callback nhận kết quả CLS đã chọn, điền vào memo đang chờ (đã set qua nút "+").</summary>
        private void DelegateSelectDataContentSubclinicalCls(object data)
        {
            try
            {
                if (data != null && data is String && _clsTargetMemo != null)
                {
                    _clsTargetMemo.Text = JoinResultBySemicolon(data.ToString());
                }
                _clsTargetMemo = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>Gộp các dòng kết quả (phân cách xuống dòng) thành 1 chuỗi ngăn bởi "; ".</summary>
        private string JoinResultBySemicolon(string raw)
        {
            try
            {
                if (string.IsNullOrEmpty(raw)) return raw;
                var lines = raw.Replace("\r\n", "\n").Split('\n');
                var parts = new List<string>();
                foreach (var l in lines)
                {
                    var s = (l ?? "").Trim();
                    if (s.Length > 0) parts.Add(s);
                }
                return string.Join("; ", parts.ToArray());
            }
            catch { return raw; }
        }

        /// <summary>Nạp combo Bác sĩ khám (GridLookUpEdit) từ V_HIS_EMPLOYEE — DisplayMember TDL_USERNAME, ValueMember LOGINNAME.</summary>
        private void SetDataCboExamLoginName(DevExpress.XtraEditors.GridLookUpEdit cbo, bool onlyCurrentLogin = false)
        {
            try
            {
                if (cbo == null) return;
                var loginName = this.currentLoginName ?? Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (cachedEmpListFull == null)
                {
                    cachedEmpListFull = BackendDataWorker.Get<V_HIS_EMPLOYEE>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .OrderByDescending(o => o.LOGINNAME == loginName)
                        .ToList();
                    cachedEmpListOnlyLogin = cachedEmpListFull.Where(o => o.LOGINNAME == loginName).ToList();
                }
                var data = onlyCurrentLogin ? cachedEmpListOnlyLogin : cachedEmpListFull;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "Tên đăng nhập", 100, 1));
                columnInfos.Add(new ColumnInfo("TDL_USERNAME", "Họ tên", 150, 2));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Khoa", 150, 3));

                ControlEditorADO controlEditorADO = new ControlEditorADO("LOGINNAME", "TDL_USERNAME", columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

                cbo.Properties.ValueMember = "LOGINNAME";
                cbo.Properties.DisplayMember = "TDL_USERNAME";
                cbo.Properties.NullText = "";
                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        // ================= Tự động lấy kết quả CLS (chkAutoTestIndex + btnAutoClsSetting) =================
        // Chỉ bật ở tab "Khám cận lâm sàng". Tích checkbox -> lấy kết quả theo dịch vụ đã cấu hình
        // (frmAutoClsSetting) qua plugin ContentSubclinical (chạy ẩn), điền vào 4 memo tương ứng.

        private bool isSuppressAutoTestIndexEvent = false;

        /// <summary>Bật/tắt chkAutoTestIndex + nút cấu hình theo tab hiện tại (chỉ bật ở "Khám cận lâm sàng").</summary>
        private void UpdateAutoTestIndexEnableByTab()
        {
            try
            {
                bool allow = (this.xtraTabControl1.SelectedTabPage == this.xtraTabPageKhamCanLamSang);
                isSuppressAutoTestIndexEvent = true;
                this.chkAutoTestIndex.Checked = false; // mỗi lần đổi tab -> bỏ tích (không tự chạy)
                isSuppressAutoTestIndexEvent = false;
                this.chkAutoTestIndex.Enabled = allow;
                this.btnAutoClsSetting.Enabled = allow;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void chkAutoTestIndex_CheckedChanged(object sender, EventArgs e)
        {
            if (isSuppressAutoTestIndexEvent) return;
            try
            {
                if (this.chkAutoTestIndex.Checked) AutoGetTestIndexByGroup();
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        /// <summary>Lấy kết quả CLS theo dịch vụ đã cấu hình cho 4 memo (huyết học/sinh hóa máu/nước tiểu/siêu âm).</summary>
        private void AutoGetTestIndexByGroup()
        {
            try
            {
                if (currentServiceReq == null) return;
                var csw = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var states = csw.GetData(frmAutoClsSetting.MODULE_LINK)
                    ?? new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                OpenClsByServiceIds(GetAutoClsStateValue(states, frmAutoClsSetting.KEY_HUYET_HOC), this.memoClsHuyetHoc);
                OpenClsByServiceIds(GetAutoClsStateValue(states, frmAutoClsSetting.KEY_SINH_HOA_MAU), this.memoClsSinhHoaMau);
                OpenClsByServiceIds(GetAutoClsStateValue(states, frmAutoClsSetting.KEY_SINH_HOA_NUOC_TIEU), this.memoClsSinhHoaNuocTieu);
                OpenClsByServiceIds(GetAutoClsStateValue(states, frmAutoClsSetting.KEY_SIEU_AM_OB), this.memoClsSieuAmOB);
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }

        private string GetAutoClsStateValue(List<HIS.Desktop.Library.CacheClient.ControlStateRDO> states, string key)
        {
            var item = states.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == frmAutoClsSetting.MODULE_LINK);
            return item != null ? item.VALUE : null;
        }

        /// <summary>Mở ContentSubclinical CHẠY ẨN theo danh sách serviceIds — kết quả trả về điền vào memo đích.</summary>
        private void OpenClsByServiceIds(string serviceIds, DevExpress.XtraEditors.BaseEdit target)
        {
            try
            {
                if (string.IsNullOrEmpty(serviceIds) || target == null || currentServiceReq == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null)
                {
                    LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                    return;
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentServiceReq.TREATMENT_ID);
                    HIS.Desktop.Common.DelegateSelectData del = (data) =>
                    {
                        if (data != null && data is string && target.Enabled)
                        {
                            string val = JoinResultBySemicolon(data.ToString());
                            if (!string.IsNullOrEmpty(val)) target.Text = val;
                        }
                    };
                    listArgs.Add(del);
                    listArgs.Add(serviceIds); // có serviceIds -> plugin chạy ẩn (không show form)
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                        HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    var contentForm = extenceInstance as System.Windows.Forms.Form;
                    if (contentForm != null) contentForm.Dispose(); // dựng & lấy dữ liệu, không hiển thị
                }
            }
            catch (Exception ex) { LogSystem.Error(ex); }
        }
    }
}
