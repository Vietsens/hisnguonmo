using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.KskServiceEditList.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    /// <summary>
    /// 58013 - Sửa dịch vụ (thêm, xóa, đổi phòng thực hiện) cho nhiều bệnh nhân khám sức khỏe hợp đồng.
    /// Bố cục tương tự "Sửa chỉ định dịch vụ", nhưng dữ liệu gộp theo dịch vụ trên toàn bộ hồ sơ đã chọn.
    /// </summary>
    public partial class frmKskServiceEditList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private List<V_HIS_TREATMENT_4> treatments;
        private V_HIS_KSK_CONTRACT kskContract;
        private RefeshReference refeshReference;

        private List<ExistServiceADO> existServices = new List<ExistServiceADO>();
        private List<AddServiceADO> addServices = new List<AddServiceADO>();
        /// <summary>Phòng thực hiện được theo dịch vụ (V_HIS_SERVICE_ROOM)</summary>
        private ILookup<long, RoomADO> roomsByService;
        /// <summary>Combo "Phòng mới" theo từng dịch vụ (tạo 1 lần, dùng lại khi vẽ lại grid)</summary>
        private Dictionary<long, RepositoryItemGridLookUpEdit> repositoryNewRoomDic = new Dictionary<long, RepositoryItemGridLookUpEdit>();
        private List<KskServiceADO> currentKskServices = new List<KskServiceADO>();
        private bool isSaving = false;
        #endregion

        public frmKskServiceEditList(Inventec.Desktop.Common.Modules.Module module, List<V_HIS_TREATMENT_4> treatments, V_HIS_KSK_CONTRACT kskContract, RefeshReference refeshReference)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.treatments = treatments;
                this.kskContract = kskContract;
                this.refeshReference = refeshReference;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmKskServiceEditList_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.InitComboLogin();
                this.InitComboKsk();
                this.SetCaptionByLanguageKey();
                this.SetDefaultValue();
                this.LoadExistServices();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.KskServiceEditList.Resources.Lang", typeof(frmKskServiceEditList).Assembly);
                this.Text = GetLang("frmKskServiceEditList.Text");
                this.lciContract.Text = GetLang("frmKskServiceEditList.lciContract.Text");
                this.lciPatientCount.Text = GetLang("frmKskServiceEditList.lciPatientCount.Text");
                this.lciLogin.Text = GetLang("frmKskServiceEditList.lciLogin.Text");
                this.lciIntructionTime.Text = GetLang("frmKskServiceEditList.lciIntructionTime.Text");
                this.lblExistTitle.Text = GetLang("frmKskServiceEditList.lcgExist.Text");
                this.lblAddTitle.Text = GetLang("frmKskServiceEditList.lcgAdd.Text");
                this.lciKsk.Text = GetLang("frmKskServiceEditList.lciKsk.Text");
                this.lciKsk.OptionsToolTip.ToolTip = GetLang("frmKskServiceEditList.lciKsk.ToolTip");
                this.lciKskService.Text = GetLang("frmKskServiceEditList.lciKskService.Text");
                this.lciAddRoom.Text = GetLang("frmKskServiceEditList.lciAddRoom.Text");
                this.btnAdd.Text = GetLang("frmKskServiceEditList.btnAdd.Text");
                this.btnSave.Text = GetLang("frmKskServiceEditList.btnSave.Text");
                this.bbtnSave.Caption = this.btnSave.Text;

                this.gcExistStt.Caption = GetLang("frmKskServiceEditList.gcExistStt.Caption");
                this.gcExistServiceCode.Caption = GetLang("frmKskServiceEditList.gcExistServiceCode.Caption");
                this.gcExistServiceName.Caption = GetLang("frmKskServiceEditList.gcExistServiceName.Caption");
                this.gcExistServiceType.Caption = GetLang("frmKskServiceEditList.gcExistServiceType.Caption");
                this.gcExistPatientCount.Caption = GetLang("frmKskServiceEditList.gcExistPatientCount.Caption");
                this.gcExistPatientCount.ToolTip = GetLang("frmKskServiceEditList.gcExistPatientCount.ToolTip");
                this.gcExistExecutedCount.Caption = GetLang("frmKskServiceEditList.gcExistExecutedCount.Caption");
                this.gcExistExecutedCount.ToolTip = GetLang("frmKskServiceEditList.gcExistExecutedCount.ToolTip");
                this.gcExistCurrentRoom.Caption = GetLang("frmKskServiceEditList.gcExistCurrentRoom.Caption");
                this.gcExistIsDelete.Caption = GetLang("frmKskServiceEditList.gcExistIsDelete.Caption");
                this.gcExistIsDelete.ToolTip = GetLang("frmKskServiceEditList.gcExistIsDelete.ToolTip");
                this.gcExistNewRoom.Caption = GetLang("frmKskServiceEditList.gcExistNewRoom.Caption");
                this.gcExistNewRoom.ToolTip = GetLang("frmKskServiceEditList.gcExistNewRoom.ToolTip");

                this.gcAddStt.Caption = GetLang("frmKskServiceEditList.gcAddStt.Caption");
                this.gcAddServiceCode.Caption = GetLang("frmKskServiceEditList.gcAddServiceCode.Caption");
                this.gcAddServiceName.Caption = GetLang("frmKskServiceEditList.gcAddServiceName.Caption");
                this.gcAddKskName.Caption = GetLang("frmKskServiceEditList.gcAddKskName.Caption");
                this.gcAddRoomName.Caption = GetLang("frmKskServiceEditList.gcAddRoomName.Caption");
                this.gcAddAmount.Caption = GetLang("frmKskServiceEditList.gcAddAmount.Caption");
                this.gcAddPrice.Caption = GetLang("frmKskServiceEditList.gcAddPrice.Caption");
                this.gcAddPrice.ToolTip = GetLang("frmKskServiceEditList.gcAddPrice.ToolTip");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.btnSave.Enabled)
                {
                    this.btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
