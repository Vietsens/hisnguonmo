/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * UserControl danh sách đồng bộ ca bệnh truyền nhiễm lên cổng ECDS (mô hình KskSyncListQD831).
 * Tìm kiếm + grid (V_HIS_TREATMENT) + cột trạng thái đẩy (đối soát HIS_ECDS_DISEASE_CASE) + phân trang
 * + đồng bộ hàng loạt + tự động đẩy (Timer). Bấm Xem/Sửa (double-click) hoặc cột "Xem" -> mở plugin
 * chi tiết InfectiousDiseaseReport qua inter-plugin; cột "Đẩy" -> đẩy riêng 1 ca.
 */
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO;
using HIS.Desktop.Plugins.InfectiousDiseaseSyncList.Worker;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.MainForm
{
    public partial class UCInfectiousDiseaseSyncList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        // Control UI khai báo ở UCInfectiousDiseaseSyncList.Designer.cs (dựng trong InitializeComponent).
        private Inventec.Desktop.Common.Modules.Module moduleData;

        private EcdsApiWorker apiWorker;
        private EcdsCatalogCache catalogCache;
        private DiseaseCaseMapper mapper;

        // Phân trang / dữ liệu grid
        private int listRowCount, listDataTotal, listStartPage;
        private List<EcdsSyncGridRowADO> listData;
        /// <summary>Map TREATMENT_ID → ID bản ghi HIS_ECDS_DISEASE_CASE (đối soát) — để cập nhật kết quả đẩy.</summary>
        private readonly Dictionary<long, long> caseIdByTreatment = new Dictionary<long, long>();

        private int currentPageSize = 50;

        // Tự động đẩy (Timer tạo ở __AutoPush.cs)
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
        public UCInfectiousDiseaseSyncList()
        {
            InitializeComponent();
        }

        public UCInfectiousDiseaseSyncList(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void UCInfectiousDiseaseSyncList_Load(object sender, EventArgs e)
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

        #region Events
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try { OpenDetailForFocusedRow(); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }
        #endregion
    }
}
