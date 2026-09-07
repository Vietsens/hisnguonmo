/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Form chi tiết: đẩy 1 ca bệnh truyền nhiễm của 1 điều trị lên cổng ECDS.
 * Tham khảo kiến trúc HIS.Desktop.Plugins.MchTreatmentExamService.
 * UI (design) nằm trong frmInfectiousDiseaseReport.Designer.cs (InitializeComponent) — KHÔNG dựng ở runtime.
 * File này chỉ giữ context/constructor/helper/event. Thứ tự Load -> InitCombo -> FillDataFromHis.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseReport.Worker;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.MainForm
{
    public partial class frmInfectiousDiseaseReport : HIS.Desktop.Utility.FormBase
    {
        #region Declare — context
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private HIS_TREATMENT treatment;
        private HIS.Desktop.Common.RefeshReference dlgRefresh;

        private EcdsApiWorker apiWorker;
        private EcdsCatalogCache catalogCache;
        private DiseaseCaseMapper mapper;

        // Đối soát ca bệnh đã đẩy
        private string ecdsCaseId;
        private string ecdsCaseCode;

        // Bệnh nhân (V_HIS_PATIENT) — nạp khi Load
        private V_HIS_PATIENT patient;

        // ControlState — nhớ Người báo cáo (tên/SĐT/email) qua các lần dùng, giữa các bệnh nhân/phiên.
        private HIS.Desktop.Library.CacheClient.ControlStateWorker reporterStateWorker;
        private System.Collections.Generic.List<HIS.Desktop.Library.CacheClient.ControlStateRDO> reporterStateRDO;
        private const string ReporterModuleLink = "HIS.Desktop.Plugins.InfectiousDiseaseReport";
        #endregion

        // Toàn bộ control + code dựng UI nằm ở frmInfectiousDiseaseReport.Designer.cs (InitializeComponent).

        #region Constructor
        public frmInfectiousDiseaseReport()
        {
            InitializeComponent();
        }

        public frmInfectiousDiseaseReport(Inventec.Desktop.Common.Modules.Module moduleData,
            HIS_TREATMENT treatment, HIS.Desktop.Common.RefeshReference dlgRefresh)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.treatment = treatment;
                this.dlgRefresh = dlgRefresh;
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Helpers
        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bind GridLookUpEdit từ danh mục — GIỐNG combo giới tính của HisIcd:
        /// gõ thẳng vào ô (không cần ô tìm riêng) -> popup mở ngay (ImmediatePopup) và nhảy tới dòng khớp.
        /// displayMember="MaTen" nên ô hiển thị "Mã - Tên" và gõ MÃ hoặc TÊN đều định vị được.
        /// </summary>
        private void SetupLookup(GridLookUpEdit cbo, System.Collections.IList data, string valueMember, string displayMember)
        {
            try
            {
                // GridLookUpEdit tạo runtime -> đảm bảo có GridView cho popup.
                var view = cbo.Properties.View;
                if (view == null)
                {
                    view = new DevExpress.XtraGrid.Views.Grid.GridView();
                    cbo.Properties.View = view;
                }
                view.OptionsBehavior.AutoPopulateColumns = false;   // KHÔNG tự sinh mọi cột (id/ma/ten/...)

                cbo.Properties.DataSource = data;
                cbo.Properties.ValueMember = valueMember;
                cbo.Properties.DisplayMember = displayMember;
                cbo.Properties.NullText = "";

                // Gõ trực tiếp vào ô -> mở popup ngay và LỌC theo Contains (như HisIcd).
                // Contains trên cột "Mã - Tên" => gõ MÃ hoặc TÊN (ở giữa chuỗi) đều lọc được.
                cbo.Properties.ImmediatePopup = true;
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;

                // Popup 1 cột gọn (hiển thị "Mã - Tên").
                view.Columns.Clear();
                var col = view.Columns.AddVisible(displayMember);
                col.Caption = "Nội dung";
                view.OptionsView.ShowColumnHeaders = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Bind GridLookUpEdit từ danh sách enum (KeyValueADO) — hiển thị + tìm theo Tên.</summary>
        private void BindEnumCombo(GridLookUpEdit cbo, List<KeyValueADO> items)
        {
            SetupLookup(cbo, items, "Value", "Text");
        }

        /// <summary>Lấy long yyyyMMddHHmmss từ DateEdit (null nếu trống).</summary>
        private long? GetDateLong(DateEdit dte)
        {
            try
            {
                if (dte == null || dte.EditValue == null) return null;
                return Int64.Parse(dte.DateTime.ToString("yyyyMMdd000000"));
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        /// <summary>Đặt DateEdit từ long yyyyMMddHHmmss.</summary>
        private void SetDateLong(DateEdit dte, long? value)
        {
            try
            {
                if (value.HasValue && value.Value > 0)
                {
                    DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(value.Value);
                    dte.EditValue = dt;
                }
                else dte.EditValue = null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private long? GetLookupLong(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo == null || cbo.EditValue == null) return null;
                long v;
                if (Int64.TryParse(cbo.EditValue.ToString(), out v)) return v;
                return null;
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }

        private int GetLookupInt(GridLookUpEdit cbo, int defaultValue)
        {
            long? v = GetLookupLong(cbo);
            return v.HasValue ? (int)v.Value : defaultValue;
        }

        /// <summary>Lấy MÃ (chuỗi) đang chọn của combo có ValueMember là mã (SDA). Null nếu trống.</summary>
        private string GetLookupString(GridLookUpEdit cbo)
        {
            try { return (cbo != null && cbo.EditValue != null) ? cbo.EditValue.ToString() : null; }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); return null; }
        }
        #endregion

        #region Events
        private void btnSave_Click(object sender, EventArgs e)
        {
            try { SaveToHisProcess(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnPush_Click(object sender, EventArgs e)
        {
            try { PushProcess(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.ecdsCaseId = null;
                this.ecdsCaseCode = null;
                ClearInputControls();
                FillDataFromHis();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion
    }
}
