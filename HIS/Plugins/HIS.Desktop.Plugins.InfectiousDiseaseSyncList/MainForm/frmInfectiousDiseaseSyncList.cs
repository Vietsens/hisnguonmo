/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Form danh sách đồng bộ ca bệnh truyền nhiễm lên cổng ECDS.
 * Trái: tìm kiếm + grid (V_HIS_TREATMENT) + phân trang + đồng bộ hàng loạt.
 * Bấm Xem/Sửa (hoặc double-click) -> mở plugin chi tiết InfectiousDiseaseReport qua inter-plugin.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm
{
    public partial class frmInfectiousDiseaseSyncList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;

        private EcdsApiWorker apiWorker;
        private EcdsCatalogCache catalogCache;
        private DiseaseCaseMapper mapper;

        // Tìm kiếm
        private PanelControl pnlSearch;
        private TextEdit txtSearchTreatmentCode, txtSearchPatientName;
        private DateEdit dteSearchFrom, dteSearchTo;
        private SimpleButton btnSearch;

        // Grid + phân trang
        private GridControl grdList;
        private GridView gvList;
        private Inventec.UC.Paging.UcPaging ucPaging;
        private int listRowCount, listDataTotal, listStartPage;
        private List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT> listData;

        // Đồng bộ + footer
        private PanelControl pnlSyncBar, pnlFooter;
        private SimpleButton btnSyncList, btnEdit, btnReconcile, btnClose;
        private int currentPageSize = 50;

        // Tự động đẩy (Timer)
        private CheckEdit chkAutoPush;
        private SpinEdit spnAutoInterval;
        private LabelControl lblAutoStatus;
        private System.Windows.Forms.Timer autoPushTimer;
        /// <summary>ID điều trị đã auto-đẩy trong phiên (mỗi ca auto tối đa 1 lần → tránh trùng/spam).</summary>
        private readonly HashSet<long> autoAttemptedIds = new HashSet<long>();
        /// <summary>Chặn tick chồng lấn / đẩy tay đang chạy.</summary>
        private bool isSyncing = false;

        // ControlState — nhớ trạng thái tự động đẩy giữa các phiên
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private bool isNotLoadWhileChangeControlStateInFirst = false;
        private readonly string moduleLink = "HIS.Desktop.Plugins.InfectiousDiseaseSyncList";
        #endregion

        #region Constructor
        public frmInfectiousDiseaseSyncList()
        {
            InitializeComponent();
            BuildUi();
        }

        public frmInfectiousDiseaseSyncList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                BuildUi();
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(
                    HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath,
                    System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void frmInfectiousDiseaseSyncList_Load(object sender, EventArgs e)
        {
            try
            {
                Config.EcdsConfigCFG.LoadConfig();
                apiWorker = new EcdsApiWorker();
                catalogCache = new EcdsCatalogCache(apiWorker);
                mapper = new DiseaseCaseMapper(catalogCache);
                SearchList();
                InitAutoPush();   // khởi tạo Timer + khôi phục trạng thái tự động đẩy
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dừng & giải phóng Timer khi đóng form (FormBase gọi sau khi đóng).</summary>
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                StopAutoPushTimer();
                if (autoPushTimer != null)
                {
                    autoPushTimer.Dispose();
                    autoPushTimer = null;
                }
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        #region Events
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try { OpenDetailForFocusedRow(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try { this.Close(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion
    }
}
