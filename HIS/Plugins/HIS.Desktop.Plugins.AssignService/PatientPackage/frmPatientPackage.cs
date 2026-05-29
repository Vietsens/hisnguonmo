using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.PatientPackage
{
    public partial class frmPatientPackage : FormBase
    {
        #region Declare

        private readonly long patientId;
        private readonly Action<List<PatientPackageDtADO>> actSelected;
        private Inventec.Desktop.Common.Modules.Module currentModule;

        private List<HIS_PATIENT_PACKAGE> allPackages = new List<HIS_PATIENT_PACKAGE>();
        private List<PatientPackageDtADO> currentDts = new List<PatientPackageDtADO>();

        #endregion

        #region Constructor

        public frmPatientPackage(long patientId, Action<List<PatientPackageDtADO>> actSelected, Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.patientId = patientId;
            this.actSelected = actSelected;
            this.currentModule = currentModule;
        }

        #endregion

        #region Load

        private void frmPatientPackage_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.SetCaptionByLanguageKey();
                this.WireEvents();
                this.LoadPackages();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void WireEvents()
        {
            try
            {
                this.gridViewPackage.FocusedRowChanged += new FocusedRowChangedEventHandler(this.gridViewPackage_FocusedRowChanged);
                this.gridViewPackage.CustomUnboundColumnData += new CustomColumnDataEventHandler(this.gridViewPackage_CustomUnboundColumnData);
                this.txtSearchPackage.EditValueChanged += new EventHandler(this.txtSearchPackage_EditValueChanged);
                this.txtSearchDt.EditValueChanged += new EventHandler(this.txtSearchDt_EditValueChanged);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.AssignService.Resources.Lang",
                    typeof(frmPatientPackage).Assembly);

                this.Text = GetLangValue("frmPatientPackage.Text");
                this.lblPackageList.Text = GetLangValue("frmPatientPackage.lblPackageList.Text");
                this.lblDtList.Text = GetLangValue("frmPatientPackage.lblDtList.Text");
                this.btnSelect.Text = GetLangValue("frmPatientPackage.btnSelect.Text");
                this.btnCancel.Text = GetLangValue("frmPatientPackage.btnCancel.Text");

                this.gColPkgName.Caption = GetLangValue("frmPatientPackage.gColPkgName.Caption");
                this.gColPkgRegisterDate.Caption = GetLangValue("frmPatientPackage.gColPkgRegisterDate.Caption");
                this.gColPkgNote.Caption = GetLangValue("frmPatientPackage.gColPkgNote.Caption");
                this.gColPkgCreateTime.Caption = GetLangValue("frmPatientPackage.gColPkgCreateTime.Caption");
                this.gColPkgCreator.Caption = GetLangValue("frmPatientPackage.gColPkgCreator.Caption");
                this.gColPkgModifyTime.Caption = GetLangValue("frmPatientPackage.gColPkgModifyTime.Caption");
                this.gColPkgModifier.Caption = GetLangValue("frmPatientPackage.gColPkgModifier.Caption");

                this.gColDtCheck.Caption = "";
                this.gColDtServiceCode.Caption = GetLangValue("frmPatientPackage.gColDtServiceCode.Caption");
                this.gColDtServiceName.Caption = GetLangValue("frmPatientPackage.gColDtServiceName.Caption");
                this.gColDtServiceTypeName.Caption = GetLangValue("frmPatientPackage.gColDtServiceTypeName.Caption");
                this.gColDtAmount.Caption = GetLangValue("frmPatientPackage.gColDtAmount.Caption");
                this.gColDtAmountUsed.Caption = GetLangValue("frmPatientPackage.gColDtAmountUsed.Caption");
                this.gColDtAmountThisTime.Caption = GetLangValue("frmPatientPackage.gColDtAmountThisTime.Caption");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private string GetLangValue(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
        }

        #endregion

        #region Data

        private void LoadPackages()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                HisPatientPackageFilter filter = new HisPatientPackageFilter();
                filter.PATIENT_ID = this.patientId;
                filter.IS_ACTIVE = (short)IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_FIELD = "REGISTER_DATE";
                filter.ORDER_DIRECTION = "DESC";

                LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => filter), filter));

                var result = new BackendAdapter(param).Get<List<HIS_PATIENT_PACKAGE>>(
                    RequestUriStore.HIS_PATIENT_PACKAGE_GET, ApiConsumers.MosConsumer, filter, param);

                this.allPackages = (result ?? new List<HIS_PATIENT_PACKAGE>())
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                WaitingManager.Hide();
                SessionManager.ProcessTokenLost(param);

                this.BindPackages(this.allPackages);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void BindPackages(List<HIS_PATIENT_PACKAGE> data)
        {
            try
            {
                this.gridViewPackage.BeginUpdate();
                try
                {
                    this.gridControlPackage.DataSource = data;
                }
                finally
                {
                    this.gridViewPackage.EndUpdate();
                }
                if (data != null && data.Count > 0)
                    this.gridViewPackage.FocusedRowHandle = 0;
                else
                    this.BindDts(new List<PatientPackageDtADO>());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadPackageDt(HIS_PATIENT_PACKAGE package)
        {
            CommonParam param = new CommonParam();
            try
            {
                if (package == null)
                {
                    this.BindDts(new List<PatientPackageDtADO>());
                    return;
                }

                WaitingManager.Show();
                HisPatientPackageDtViewFilter filter = new HisPatientPackageDtViewFilter();
                filter.PATIENT_PACKAGE_ID = package.ID;
                filter.IS_ACTIVE = (short)IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_FIELD = "SV_SERVICE_CODE";
                filter.ORDER_DIRECTION = "ASC";

                LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => filter), filter));

                var result = new BackendAdapter(param).Get<List<PatientPackageDtADO>>(
                    RequestUriStore.HIS_PATIENT_PACKAGE_DT_GETVIEW, ApiConsumers.MosConsumer, filter, param);

                WaitingManager.Hide();
                SessionManager.ProcessTokenLost(param);

                this.currentDts = (result ?? new List<PatientPackageDtADO>())
                    .Where(o => o.SV_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                             && o.SV_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                             && o.SV_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                             && o.SV_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                    .ToList();

                foreach (var dt in this.currentDts)
                {
                    dt.IsChecked = false;
                    dt.AmountThisTime = 1;
                    dt.PATIENT_PACKAGE_NAME = package.PACKAGE_NAME;
                }

                this.BindDts(this.currentDts);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void BindDts(List<PatientPackageDtADO> data)
        {
            try
            {
                this.gridViewDt.BeginUpdate();
                try
                {
                    this.gridControlDt.DataSource = data;
                }
                finally
                {
                    this.gridViewDt.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Events

        private void gridViewPackage_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                this.gridViewDt.CloseEditor();
                var package = this.gridViewPackage.GetFocusedRow() as HIS_PATIENT_PACKAGE;
                this.LoadPackageDt(package);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewPackage_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (!e.IsGetData) return;
                var source = this.gridControlPackage.DataSource as List<HIS_PATIENT_PACKAGE>;
                if (source == null || e.ListSourceRowIndex < 0 || e.ListSourceRowIndex >= source.Count) return;
                var data = source[e.ListSourceRowIndex];
                if (data == null) return;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1;
                else if (e.Column.FieldName == "CREATE_TIME_STR")
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSearchPackage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string key = (this.txtSearchPackage.Text ?? "").Trim().ToLower();
                List<HIS_PATIENT_PACKAGE> filtered = string.IsNullOrEmpty(key)
                    ? this.allPackages
                    : this.allPackages.Where(o => (o.PACKAGE_NAME ?? "").ToLower().Contains(key)
                                                || (o.NOTE ?? "").ToLower().Contains(key)).ToList();
                this.BindPackages(filtered);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSearchDt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string key = (this.txtSearchDt.Text ?? "").Trim();
                if (string.IsNullOrEmpty(key))
                    this.gridViewDt.ActiveFilterString = "";
                else
                {
                    string safe = key.Replace("'", "''");
                    this.gridViewDt.ActiveFilterString = string.Format(
                        "[SV_SERVICE_CODE] Like '%{0}%' OR [SERVICE_NAME] Like '%{0}%'", safe);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewDt.CloseEditor();
                this.gridViewDt.UpdateCurrentRow();

                var selected = (this.currentDts ?? new List<PatientPackageDtADO>())
                    .Where(o => o.IsChecked).ToList();

                if (!selected.Any())
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonDichVuTrongGoi,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (selected.Any(o => o.AmountThisTime <= 0))
                {
                    XtraMessageBox.Show(
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.Common__SoLuongPhaiLonHonKhong),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (this.actSelected != null)
                    this.actSelected(selected);

                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
