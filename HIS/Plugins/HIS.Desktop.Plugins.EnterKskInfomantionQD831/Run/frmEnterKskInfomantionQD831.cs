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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.EnterKskInfomantionQD831.Config;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EnterKskInfomantionQD831.Run
{
    /// <summary>
    /// Form nhập thông tin khám sức khỏe theo QĐ 831 (khung dùng chung).
    /// Giữ lại: thông tin hành chính bệnh nhân + panel "Danh sách y lệnh khám" + nhóm nút Lưu/In/Kết thúc.
    /// 4 tab (Tiền sử / Tiêm chủng / Khám lâm sàng / Khám cận lâm sàng) để TRẮNG — tự bổ sung control sau.
    /// </summary>
    public partial class frmEnterKskInfomantionQD831 : HIS.Desktop.Utility.FormBase
    {
        #region ----ObjCurrent-----
        private V_HIS_SERVICE_REQ currentServiceReq { get; set; }
        private bool isLoginAdmin = false;
        private string currentLoginName = null;
        /// <summary>Cờ ký EMR (khi bấm "Lưu Ký"). Dùng khi bổ sung logic lưu/ký sau.</summary>
        public bool IsSignEmr { get; set; }
        #endregion

        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmEnterKskInfomantionQD831(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_SERVICE_REQ hisServiceReq)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentServiceReq = hisServiceReq;
                this.currentModule = moduleData;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmEnterKskInfomantionQD831_Load(object sender, System.EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                this.currentLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.isLoginAdmin = IsLoginAdmin(this.currentLoginName);

                // Nạp dữ liệu nền + thông tin hành chính bệnh nhân.
                PrefetchFormData();
                ShowInformationPatient();
                InitAvatarContextMenu();
                InitAutoClsSettingButtonIcon();
                InitBmiAutoCalc();
                // "Tự động lấy kết quả CLS": nối sự kiện + set enable theo tab (chỉ bật ở tab Khám cận lâm sàng).
                this.chkAutoTestIndex.CheckedChanged += new System.EventHandler(this.chkAutoTestIndex_CheckedChanged);
                UpdateAutoTestIndexEnableByTab();

                // Panel trái "Danh sách y lệnh khám".
                InitYlenhData();
                // "Tự động kết thúc" + nút "Kết thúc khám".
                InitFinishFeature();

                // Tab "B. Tiền sử" (tab mặc định) mục 3: Dị ứng (grid, type 50) + Bệnh tật (control động, type 49).
                InitDiseaseHistoryTienSu();
                // Mục 2 (Yếu tố nguy cơ): radio "Có" mới enable các control con cùng nhóm.
                InitMuc2RiskFactorLogic();
                // Vừa mở: XÓA TRẮNG toàn bộ thông tin. Chọn y lệnh bên panel mới nạp dữ liệu theo y lệnh đó.
                ClearAllKskControls();
                // Tab "C. Tiêm chủng" và "D. Khám cận lâm sàng" LAZY-LOAD khi mở tab (xtraTabControl1_SelectedPageChanged)
                // -> mở form nhanh hơn (không nạp HIS_ICD/HIS_VACCINE_TYPE/combo ngay lúc mở).

                WaitingManager.Hide();

                // Hoãn (sau khi form hiển thị) 2 tác vụ quét toàn bộ control: in đậm tiêu đề mục + thêm nút clear.
                this.BeginInvoke((System.Windows.Forms.MethodInvoker)(() =>
                {
                    try
                    {
                        BoldAllSectionHeaders(this);
                        InitClearButtonForGridLookUpEdits(this);
                    }
                    catch (System.Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                }));
            }
            catch (System.Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Xác định login hiện tại có quyền admin không (HIS_EMPLOYEE.IS_ADMIN = 1).
        /// </summary>
        private bool IsLoginAdmin(string loginName)
        {
            bool result = false;
            try
            {
                if (!string.IsNullOrEmpty(loginName))
                {
                    var employee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>()
                        .FirstOrDefault(p => p.LOGINNAME == loginName.Trim());
                    if (employee != null && employee.IS_ADMIN == (short)1)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>Đổ thông tin hành chính bệnh nhân (mã, tên, giới, ngày sinh, hợp đồng, lý do khám, avatar).</summary>
        private void ShowInformationPatient()
        {
            try
            {
                if (currentServiceReq != null)
                {
                    if (this.btnPrint != null) this.btnPrint.Enabled = true; // có y lệnh -> cho In/Lưu ký (Mps000519)
                    txtServiceCode.Text = currentServiceReq.SERVICE_REQ_CODE;
                    txtTreatmentCode.Text = currentServiceReq.TDL_TREATMENT_CODE;
                    txtPatientCode.Text = currentServiceReq.TDL_PATIENT_CODE;
                    lblPatientCccd.Text = currentServiceReq.TDL_PATIENT_CCCD_NUMBER;
                    txtPatientName.Text = currentServiceReq.TDL_PATIENT_NAME;
                    txtGender.Text = currentServiceReq.TDL_PATIENT_GENDER_NAME;

                    txtPatientDob.Text = currentServiceReq.TDL_PATIENT_IS_HAS_NOT_DAY_DOB != (short?)1 ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentServiceReq.TDL_PATIENT_DOB) : currentServiceReq.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    txtInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentServiceReq.INTRUCTION_TIME);
                    txtLyDoKham.Text = currentServiceReq.HOSPITALIZATION_REASON; // Lý do khám (từ y lệnh)
                    if (currentServiceReq.TDL_KSK_CONTRACT_ID != null && currentServiceReq.TDL_KSK_CONTRACT_ID > 0)
                    {
                        CommonParam param = new CommonParam();
                        HisKskContractFilter filter = new HisKskContractFilter();
                        filter.ID = currentServiceReq.TDL_KSK_CONTRACT_ID;
                        var dataKskContract = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>>("api/HisKskContract/Get", ApiConsumers.MosConsumer, filter, param).SingleOrDefault();
                        if (dataKskContract != null)
                            txtKskContract.Text = dataKskContract.KSK_CONTRACT_CODE;
                    }

                    // Avatar: ưu tiên TDL_PATIENT_AVATAR_URL; nếu trống lấy AVATAR_URL của HIS_PATIENT; vẫn trống → ảnh mặc định.
                    // Tải nền (không chặn UI khi FSS chậm): hiện ảnh mặc định trước, có ảnh thì thay sau.
                    string pathLocal = GetPathDefault();
                    if (!string.IsNullOrEmpty(pathLocal) && System.IO.File.Exists(pathLocal))
                        pictureEdit1.Image = Image.FromFile(pathLocal);
                    string avatarUrlHint = currentServiceReq.TDL_PATIENT_AVATAR_URL;
                    long avatarPatientId = currentServiceReq.TDL_PATIENT_ID;
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            // Lấy HIS_PATIENT 1 lần: dùng cho Nhóm máu (BLOOD_ABO_CODE + BLOOD_RH_CODE) + avatar (khi hint trống).
                            MOS.EFMODEL.DataModels.HIS_PATIENT patient = GetPatient(avatarPatientId);
                            if (patient != null && this.IsHandleCreated && !this.IsDisposed)
                            {
                                string blood = ((patient.BLOOD_ABO_CODE ?? "") + (patient.BLOOD_RH_CODE ?? "")).Trim();
                                this.BeginInvoke((System.Windows.Forms.MethodInvoker)(() => { lblBlood.Text = blood; }));
                            }
                            string avatarUrl = avatarUrlHint;
                            if (String.IsNullOrEmpty(avatarUrl))
                            {
                                avatarUrl = (patient != null) ? patient.AVATAR_URL : null;
                            }
                            if (String.IsNullOrEmpty(avatarUrl)) return;
                            System.IO.MemoryStream stream = Inventec.Fss.Client.FileDownload.GetFile(avatarUrl);
                            if (stream == null) return;
                            Image img = Image.FromStream(stream);
                            img.Tag = avatarUrl;
                            if (this.IsHandleCreated && !this.IsDisposed)
                                this.BeginInvoke((System.Windows.Forms.MethodInvoker)(() =>
                                {
                                    pictureEdit1.Image = img;
                                }));
                        }
                        catch (System.Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
                    });
                }
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetPathDefault()
        {
            string imageDefaultPath = string.Empty;
            try
            {
                string localPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                imageDefaultPath = localPath + "\\Img\\ImageStorage\\notImage.jpg";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return imageDefaultPath;
        }

        /// <summary>Lấy HIS_PATIENT theo patientId (model đầy đủ như EnterKskV2) — dùng cho Nhóm máu + AVATAR_URL.</summary>
        private MOS.EFMODEL.DataModels.HIS_PATIENT GetPatient(long patientId)
        {
            try
            {
                if (patientId <= 0) return null;
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientFilter filter = new MOS.Filter.HisPatientFilter();
                filter.ID = patientId;
                var patients = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, null);
                if (patients != null && patients.Count > 0)
                    return patients[0];
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>Gán icon setting (⚙) cho nút cấu hình cạnh "Tự động lấy kết quả CLS".</summary>
        private void InitAutoClsSettingButtonIcon()
        {
            try
            {
                btnAutoClsSetting.Text = "⚙";
                btnAutoClsSetting.Appearance.Font = new System.Drawing.Font("Segoe UI Symbol", 9F);
                btnAutoClsSetting.Appearance.Options.UseFont = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // ================= In đậm tiêu đề mục (I. II. 1. 2.1 ...) =================

        private void BoldAllSectionHeaders(Control parent)
        {
            try
            {
                foreach (Control c in parent.Controls)
                {
                    LabelControl dxl = c as LabelControl;
                    if (dxl != null && IsSectionHeaderText(dxl.Text))
                    {
                        dxl.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                    System.Windows.Forms.Label wl = c as System.Windows.Forms.Label;
                    if (wl != null && IsSectionHeaderText(wl.Text))
                    {
                        wl.Font = new System.Drawing.Font(wl.Font, wl.Font.Style | System.Drawing.FontStyle.Bold);
                    }
                    GroupControl gc = c as GroupControl;
                    if (gc != null && IsSectionHeaderText(gc.Text))
                    {
                        gc.AppearanceCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                    DevExpress.XtraLayout.LayoutControl lcx = c as DevExpress.XtraLayout.LayoutControl;
                    if (lcx != null && lcx.Root != null)
                    {
                        BoldLayoutGroupItems(lcx.Root);
                    }
                    if (c.Controls.Count > 0) BoldAllSectionHeaders(c);
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>In đậm caption của group + item layout (LayoutControlItem.Text) khớp mẫu tiêu đề. Đệ quy.</summary>
        private void BoldLayoutGroupItems(DevExpress.XtraLayout.LayoutControlGroup g)
        {
            try
            {
                if (g == null) return;
                if (IsSectionHeaderText(g.Text))
                {
                    g.AppearanceGroup.FontStyleDelta = System.Drawing.FontStyle.Bold;
                }
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in g.Items)
                {
                    DevExpress.XtraLayout.LayoutControlGroup sub = item as DevExpress.XtraLayout.LayoutControlGroup;
                    if (sub != null) { BoldLayoutGroupItems(sub); continue; }
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && IsSectionHeaderText(lci.Text))
                    {
                        lci.AppearanceItemCaption.FontStyleDelta = System.Drawing.FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        /// <summary>True nếu text bắt đầu bằng số La Mã + "." (I. II.) hoặc số/đa cấp + "." (1. 2.1 2.1.) rồi tới chữ.</summary>
        private bool IsSectionHeaderText(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string t = text.Trim();
            if (t.Length == 0) return false;
            return System.Text.RegularExpressions.Regex.IsMatch(t, @"^(?:[IVXLCDM]{1,6}\.|\d{1,2}\.(?:\d{1,2}\.?)*)\s+\S");
        }

        // ================= Nhóm nút Lưu / In / Kết thúc =================

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        /// <summary>
        /// Lưu — khung QĐ831: POST phần khám (B+D+C) qua /api/HisKskProfile/SaveExam.
        /// (Phần A hồ sơ HIS_KSK_PROFILE lưu qua API hồ sơ riêng — chưa nối ở đây.)
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                var exam = CollectExamSdo();
                bool empty = exam.HisKskGeneral == null && exam.HisDhst == null
                    && (exam.HisDiseaseDetailResults == null || exam.HisDiseaseDetailResults.Count == 0)
                    && (exam.HisHealthVaccinations == null || exam.HisHealthVaccinations.Count == 0)
                    && (exam.HisKskRelations == null || exam.HisKskRelations.Count == 0)
                    && !ProfileHasData(exam.HisKskProfile);
                if (empty)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa nhập dữ liệu khám để lưu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                CommonParam param = new CommonParam();
                WaitingManager.Show();
                bool success = false;
                var saved = SaveExamToApi(exam, param); // POST SaveExam
                if (saved != null) success = true;
                WaitingManager.Hide();

                // Message chuẩn theo thư viện (giống EnterKskV2).
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>In phiếu Hồ sơ QĐ831 -> Mps000519 (dữ liệu theo y lệnh đang xử lý, đã lưu).</summary>
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintMps000519();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndSign_Click(object sender, EventArgs e)
        {
            try
            {
                IsSignEmr = true;
                btnSave_Click(null, null);
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Nút cấu hình dịch vụ tự động lấy kết quả CLS.</summary>
        private void btnAutoClsSetting_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new frmAutoClsSetting())
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool _tiemChungLoaded = false;
        private bool _khamClsLoaded = false;

        /// <summary>Lazy-load nội dung nặng theo tab khi mở lần đầu (tối ưu tốc độ mở form).</summary>
        private void xtraTabControl1_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            try
            {
                if (e.Page == this.xtraTabPageTiemChung && !_tiemChungLoaded)
                {
                    _tiemChungLoaded = true;
                    WaitingManager.Show();
                    try { LoadTiemChungData(); PopulateLazyTab(e.Page); } finally { WaitingManager.Hide(); }
                }
                else if (e.Page == this.xtraTabPageKhamCanLamSang && !_khamClsLoaded)
                {
                    _khamClsLoaded = true;
                    WaitingManager.Show();
                    try { InitKhamClsTab(); PopulateLazyTab(e.Page); } finally { WaitingManager.Hide(); }
                }
                // Cập nhật enable "Tự động lấy kết quả CLS" theo tab (bỏ tích khi rời tab).
                UpdateAutoTestIndexEnableByTab();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        /// <summary>
        /// Nút (ảnh người) cạnh thông tin bệnh nhân — mở chức năng Cập nhật thông tin bệnh nhân (PatientUpdate).
        /// Khi tắt form PatientUpdate → load lại thông tin HIS_PATIENT (làm mới phần hành chính đang hiển thị).
        /// </summary>
        private void btnPatientUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentServiceReq == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == "HIS.Desktop.Plugins.PatientUpdate").FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.PatientUpdate");
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(currentServiceReq.TDL_PATIENT_ID);
                listArgs.Add(currentServiceReq.TREATMENT_ID);
                listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ReloadPatientData);

                var instance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(
                    HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                var frm = instance as System.Windows.Forms.Form;
                if (frm != null)
                {
                    frm.ShowDialog(this);
                    // Khi tắt form PatientUpdate → load lại HIS_PATIENT.
                    ReloadPatientData(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.HIS_KSK_RELATION> familyRelations;

        /// <summary>Nút "Thông tin quan hệ gia đình" — mở popup grid nhập quan hệ gia đình.</summary>
        private void btnFamilyRelation_Click(object sender, EventArgs e)
        {
            try
            {
                using (var frm = new frmFamilyRelation(familyRelations))
                {
                    if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        familyRelations = frm.Result; // giữ lại để mở lần sau + lưu về sau
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        /// <summary>
        /// Load lại thông tin bệnh nhân (HIS_PATIENT) sau khi cập nhật: refetch V_HIS_SERVICE_REQ (view pull lại
        /// các trường TDL_PATIENT_* mới nhất từ HIS_PATIENT) rồi hiển thị lại phần hành chính + avatar.
        /// </summary>
        private void ReloadPatientData(object data)
        {
            try
            {
                if (currentServiceReq == null) return;
                var param = new CommonParam();
                var filter = new HisServiceReqViewFilter();
                filter.ID = currentServiceReq.ID;
                var list = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>(
                    "api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
                var sreq = (list != null) ? list.FirstOrDefault() : null;
                if (sreq != null)
                {
                    this.currentServiceReq = sreq;
                }
                ShowInformationPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearData_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var cbo = sender as GridLookUpEdit;
                    if (cbo != null)
                        cbo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
