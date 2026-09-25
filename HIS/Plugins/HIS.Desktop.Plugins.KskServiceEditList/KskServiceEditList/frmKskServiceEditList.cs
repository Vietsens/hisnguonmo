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

namespace HIS.Desktop.Plugins.KskServiceEditList
{
    /// <summary>
    /// 58013 - Sửa dịch vụ cho nhiều bệnh nhân khám sức khỏe hợp đồng.
    /// Luồng hiển thị và xử lý tương tự "Sửa chỉ định dịch vụ" (AssignServiceEdit):
    /// lưới dịch vụ có ô chọn — dịch vụ đang có được tick sẵn; tick thêm = thêm dịch vụ,
    /// bỏ tick = xóa dịch vụ, đổi "Phòng thực hiện" = đổi phòng. Lưu gọi API api/HisKskContract/ServiceEdit.
    /// </summary>
    public partial class frmKskServiceEditList : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private List<V_HIS_TREATMENT_4> treatments;
        private V_HIS_KSK_CONTRACT kskContract;
        private RefeshReference refeshReference;

        /// <summary>Toàn bộ dòng dịch vụ (dịch vụ đang có + dịch vụ trong nhóm dịch vụ KSK của hợp đồng)</summary>
        private List<ServiceRowADO> allServiceRows = new List<ServiceRowADO>();
        /// <summary>Phòng thực hiện được theo dịch vụ (V_HIS_SERVICE_ROOM)</summary>
        private ILookup<long, RoomADO> roomsByService;
        /// <summary>Tên phòng theo ROOM_ID (hiển thị cột "Phòng thực hiện")</summary>
        private Dictionary<long, string> roomNameDic;
        /// <summary>Combo "Phòng thực hiện" theo từng dịch vụ (tạo 1 lần, dùng lại khi vẽ lại grid)</summary>
        private Dictionary<long, RepositoryItemGridLookUpEdit> repositoryRoomDic = new Dictionary<long, RepositoryItemGridLookUpEdit>();
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
                this.SetCaptionByLanguageKey();
                this.SetDefaultValue();
                this.LoadServiceRows();
                this.InitComboRoom();
                this.FillDataToGrid();
                this.GridViewService.Focus();
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
                this.lciIntructionTime.Text = GetLang("frmKskServiceEditList.lciIntructionTime.Text");
                this.lciRoom.Text = GetLang("frmKskServiceEditList.lciRoom.Text");
                this.lciRoom.OptionsToolTip.ToolTip = GetLang("frmKskServiceEditList.lciRoom.ToolTip");
                this.lciLogin.Text = GetLang("frmKskServiceEditList.lciLogin.Text");
                this.toggleSwitch.Properties.OffText = GetLang("frmKskServiceEditList.toggleSwitch.OffText");
                this.toggleSwitch.Properties.OnText = GetLang("frmKskServiceEditList.toggleSwitch.OnText");
                this.btnSave.Text = GetLang("frmKskServiceEditList.btnSave.Text");

                this.gcServiceCode.Caption = GetLang("frmKskServiceEditList.gcServiceCode.Caption");
                this.gcServiceName.Caption = GetLang("frmKskServiceEditList.gcServiceName.Caption");
                this.gcRoom.Caption = GetLang("frmKskServiceEditList.gcRoom.Caption");
                this.gcRoom.ToolTip = GetLang("frmKskServiceEditList.gcRoom.ToolTip");
                this.gcPatientCount.Caption = GetLang("frmKskServiceEditList.gcPatientCount.Caption");
                this.gcPatientCount.ToolTip = GetLang("frmKskServiceEditList.gcPatientCount.ToolTip");
                this.gcExecutedCount.Caption = GetLang("frmKskServiceEditList.gcExecutedCount.Caption");
                this.gcExecutedCount.ToolTip = GetLang("frmKskServiceEditList.gcExecutedCount.ToolTip");
                this.gcAmount.Caption = GetLang("frmKskServiceEditList.gcAmount.Caption");
                this.gcPrice.Caption = GetLang("frmKskServiceEditList.gcPrice.Caption");
                this.gcPrice.ToolTip = GetLang("frmKskServiceEditList.gcPrice.ToolTip");
                this.gcServiceType.Caption = GetLang("frmKskServiceEditList.gcServiceType.Caption");
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

        private string GetTitleMessage()
        {
            return HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
        }
    }
}
