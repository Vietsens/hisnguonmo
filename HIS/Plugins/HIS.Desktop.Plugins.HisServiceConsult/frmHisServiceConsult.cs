/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisServiceConsult.ADO;
using HIS.Desktop.Plugins.HisServiceConsult.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceConsult
{
    public partial class frmHisServiceConsult : FormBase
    {
        #region Declare

        Inventec.Desktop.Common.Modules.Module currentModule;
        HIS.Desktop.Common.DelegateSelectData delegateSelect;

        long treatmentId;

        long? currentConsultId;
        List<MOS.EFMODEL.DataModels.HIS_CONSULT_RESULT_TYPE> resultTypes;
        List<ACS.EFMODEL.DataModels.ACS_USER> acsUsers;
        List<PackageGridADO> packageRows;

        HisServiceConsultSDO originalSDO;

        #endregion

        #region Constructor

        public frmHisServiceConsult(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId, HIS.Desktop.Common.DelegateSelectData delegateSelect)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.currentModule = moduleData;
                this.treatmentId = treatmentId;
                this.delegateSelect = delegateSelect;
                SetIcon();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public frmHisServiceConsult(Inventec.Desktop.Common.Modules.Module moduleData, long treatmentId)
            : this(moduleData, treatmentId, null)
        {
        }

        #endregion

        #region SetIcon

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
                LogSystem.Error(ex);
            }
        }

        #endregion

        #region Load

        private void frmHisServiceConsult_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                InitConsultantCombo();
                InitResultTypeCombo();
                InitPackageGrid();
                SetDefaultValue();
                LoadDataByTreatment();
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
                ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisServiceConsult.Resources.Lang",
                    typeof(frmHisServiceConsult).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());

                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.txtKeyword.NullText",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());

                this.lciConsultantLoginname.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.lciConsultant.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.lciResultType.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.lciResultType.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.lciConsultTime.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.lciConsultTime.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.lciReason.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.lciReason.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.lciDescription.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());

                this.btnReset.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.btnReset.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.btnSave.Text",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());

                this.gcChk.Caption = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.gcChk.Caption",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.gcStt.Caption = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.gcStt.Caption",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.gcPackageCode.Caption = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.gcPackageCode.Caption",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
                this.gcPackageName.Caption = Inventec.Common.Resource.Get.Value(
                    "frmHisServiceConsult.gcPackageName.Caption",
                    ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitConsultantCombo()
        {
            try
            {
                this.acsUsers = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .ToList();

                this.cboConsultantUser.Properties.DataSource = this.acsUsers;
                this.cboConsultantUser.Properties.DisplayMember = "USERNAME";
                this.cboConsultantUser.Properties.ValueMember = "LOGINNAME";
                this.cboConsultantUser.Properties.TextEditStyle = TextEditStyles.Standard;
                this.cboConsultantUser.Properties.PopupFilterMode = PopupFilterMode.Contains;
                this.cboConsultantUser.Properties.ImmediatePopup = true;
                this.cboConsultantUser.ForceInitialize();
                this.cboConsultantUser.Properties.PopupFormWidth = 320;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitResultTypeCombo()
        {
            CommonParam param = new CommonParam();
            try
            {
                HisConsultResultTypeFilter filter = new HisConsultResultTypeFilter();
                filter.IS_ACTIVE = (short)IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => filter), filter));

                this.resultTypes = new BackendAdapter(param).Get<List<HIS_CONSULT_RESULT_TYPE>>(
                    HisRequestUriStore.MOSHIS_HIS_CONSULT_RESULT_TYPE_GET,
                    ApiConsumers.MosConsumer, filter, param);

                if (this.resultTypes == null) this.resultTypes = new List<HIS_CONSULT_RESULT_TYPE>();
                this.resultTypes = this.resultTypes
                    .OrderBy(o => o.NUM_ORDER ?? long.MaxValue)
                    .ThenBy(o => o.ID)
                    .ToList();

                this.cboResultType.Properties.DataSource = this.resultTypes;
                this.cboResultType.Properties.DisplayMember = "CONSULT_RESULT_TYPE_NAME";
                this.cboResultType.Properties.ValueMember = "ID";
                this.cboResultType.Properties.TextEditStyle = TextEditStyles.Standard;
                this.cboResultType.Properties.PopupFilterMode = PopupFilterMode.Contains;
                this.cboResultType.Properties.ImmediatePopup = true;
                this.cboResultType.ForceInitialize();
                this.cboResultType.Properties.PopupFormWidth = 320;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitPackageGrid()
        {
            try
            {
                this.gridViewPackage.BeginUpdate();
                try
                {
                    var packages = BackendDataWorker.Get<HIS_PACKAGE>() ?? new List<HIS_PACKAGE>();
                    var actives = packages
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .OrderBy(o => o.PACKAGE_CODE)
                        .ToList();

                    this.packageRows = actives.Select(o => new PackageGridADO(o) { IS_CHECKED = false }).ToList();
                    this.gridControlPackage.DataSource = this.packageRows;
                }
                finally
                {
                    this.gridViewPackage.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.dteConsultTime.EditValue = DateTime.Now;

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var data = (this.acsUsers ?? new List<ACS.EFMODEL.DataModels.ACS_USER>())
                    .FirstOrDefault(o => (o.LOGINNAME ?? "").ToUpper() == (loginName ?? "").ToUpper());
                if (data != null)
                {
                    this.cboConsultantUser.EditValue = data.LOGINNAME;
                    this.txtConsultantLoginname.Text = data.LOGINNAME;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadDataByTreatment()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                long treatmentIdInput = this.treatmentId;
                LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => treatmentIdInput), treatmentIdInput));

                this.originalSDO = new BackendAdapter(param).Post<HisServiceConsultSDO>(
                    HisRequestUriStore.MOSHIS_HIS_SERVICE_CONSULT_GETBYTREATMENT,
                    ApiConsumers.MosConsumer, treatmentIdInput, param);

                if (this.originalSDO != null && this.originalSDO.Consult != null)
                {
                    FillDataToForm(this.originalSDO);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private void FillDataToForm(HisServiceConsultSDO sdo)
        {
            try
            {
                if (sdo == null || sdo.Consult == null) return;

                this.currentConsultId = sdo.Consult.ID;
                this.cboConsultantUser.EditValue = sdo.Consult.CONSULTANT_LOGINNAME;
                this.txtConsultantLoginname.Text = sdo.Consult.CONSULTANT_LOGINNAME;
                this.cboResultType.EditValue = sdo.Consult.CONSULT_RESULT_TYPE_ID;
                this.txtReason.Text = sdo.Consult.REASON ?? "";
                this.txtDescription.Text = sdo.Consult.DESCRIPTION ?? "";

                if (sdo.Consult.CONSULT_TIME.HasValue && sdo.Consult.CONSULT_TIME.Value > 0)
                {
                    var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(sdo.Consult.CONSULT_TIME.Value);
                    if (dt.HasValue) this.dteConsultTime.EditValue = dt.Value;
                }

                var existingLinks = (sdo.Packages ?? new List<HIS_CONSULT_PACKAGE>())
                    .Where(o => o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                    .GroupBy(o => o.PACKAGE_ID)
                    .ToDictionary(g => g.Key, g => g.First().ID);

                if (this.packageRows != null)
                {
                    foreach (var row in this.packageRows)
                    {
                        long existingConsultPackageId;
                        if (existingLinks.TryGetValue(row.ID, out existingConsultPackageId))
                        {
                            row.IS_CHECKED = true;
                            row.CONSULT_PACKAGE_ID = existingConsultPackageId;
                        }
                        else
                        {
                            row.IS_CHECKED = false;
                            row.CONSULT_PACKAGE_ID = 0;
                        }
                    }
                    SortAndReloadPackages();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SortAndReloadPackages()
        {
            try
            {
                this.gridViewPackage.BeginUpdate();
                try
                {
                    this.packageRows = this.packageRows
                        .OrderByDescending(o => o.IS_CHECKED)
                        .ThenBy(o => o.PACKAGE_CODE)
                        .ToList();
                    this.gridControlPackage.DataSource = this.packageRows;
                    ApplyKeywordFilter();
                }
                finally
                {
                    this.gridViewPackage.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Filter

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                ApplyKeywordFilter();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ApplyKeywordFilter()
        {
            try
            {
                string keyword = (this.txtKeyword.Text ?? "").Trim();
                if (string.IsNullOrEmpty(keyword))
                {
                    this.gridViewPackage.ActiveFilterString = "";
                    return;
                }
                string escaped = keyword.Replace("'", "''");
                this.gridViewPackage.ActiveFilterString =
                    string.Format("Contains(Upper([PACKAGE_CODE]), '{0}') OR Contains(Upper([PACKAGE_NAME]), '{0}')",
                        escaped.ToUpper());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Combo events

        private void cboConsultantUser_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && this.cboConsultantUser.EditValue != null)
                {
                    this.txtConsultantLoginname.Text = (this.cboConsultantUser.EditValue ?? "").ToString();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboConsultantUser_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && this.cboConsultantUser.EditValue != null)
                {
                    this.txtConsultantLoginname.Text = (this.cboConsultantUser.EditValue ?? "").ToString();
                }
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
                if (e.Column.FieldName == "STT" && e.IsGetData)
                {
                    e.Value = e.ListSourceRowIndex + 1;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

        private bool ValidateForm()
        {
            bool ok = true;
            try
            {
                this.dxErrorProvider1.ClearErrors();

                if (this.cboConsultantUser.EditValue == null
                    || string.IsNullOrWhiteSpace((this.cboConsultantUser.EditValue ?? "").ToString()))
                {
                    this.dxErrorProvider1.SetError(this.cboConsultantUser,
                        ResourceMessage.TruongDuLieuBatBuoc, ErrorType.Warning);
                    ok = false;
                }

                if (this.cboResultType.EditValue == null
                    || string.IsNullOrWhiteSpace((this.cboResultType.EditValue ?? "").ToString()))
                {
                    this.dxErrorProvider1.SetError(this.cboResultType,
                        ResourceMessage.TruongDuLieuBatBuoc, ErrorType.Warning);
                    ok = false;
                }

                if (!string.IsNullOrEmpty(this.txtReason.Text) && this.txtReason.Text.Length > 2000)
                {
                    this.dxErrorProvider1.SetError(this.txtReason,
                        ResourceMessage.LyDoKhongDuocVuotQua2000KyTu, ErrorType.Warning);
                    ok = false;
                }

                if (!string.IsNullOrEmpty(this.txtDescription.Text) && this.txtDescription.Text.Length > 2000)
                {
                    this.dxErrorProvider1.SetError(this.txtDescription,
                        ResourceMessage.MoTaKhongDuocVuotQua2000KyTu, ErrorType.Warning);
                    ok = false;
                }

                var checkedRows = GetCheckedPackageRows();
                if (checkedRows == null || checkedRows.Count == 0)
                {
                    XtraMessageBox.Show(ResourceMessage.VuiLongChonGoiDichVu,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                ok = false;
            }
            return ok;
        }

        private List<PackageGridADO> GetCheckedPackageRows()
        {
            try
            {
                this.gridViewPackage.CloseEditor();
                this.gridViewPackage.UpdateCurrentRow();
                if (this.packageRows == null) return new List<PackageGridADO>();
                return this.packageRows
                    .Where(o => o.IS_CHECKED)
                    .GroupBy(o => o.ID)
                    .Select(g => g.First())
                    .ToList();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return new List<PackageGridADO>();
            }
        }

        #endregion

        #region Save

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            if (!ValidateForm()) return;

            string loginName = (this.cboConsultantUser.EditValue ?? "").ToString();
            var consultant = (this.acsUsers ?? new List<ACS.EFMODEL.DataModels.ACS_USER>())
                .FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                                  && o.LOGINNAME == loginName);
            if (consultant == null)
            {
                this.dxErrorProvider1.SetError(this.cboConsultantUser,
                    ResourceMessage.TruongDuLieuBatBuoc, ErrorType.Warning);
                return;
            }

            long? consultTime = null;
            if (this.dteConsultTime.EditValue is DateTime)
            {
                DateTime dt = (DateTime)this.dteConsultTime.EditValue;
                consultTime = long.Parse(dt.ToString("yyyyMMddHHmm00"));
            }

            long resultTypeId = Convert.ToInt64(this.cboResultType.EditValue);
            var checkedPackages = GetCheckedPackageRows();
            string reason = this.txtReason.Text;
            string description = this.txtDescription.Text;

            if (this.currentConsultId.HasValue && this.currentConsultId.Value > 0)
            {
                UpdateConsult(consultant, consultTime, resultTypeId, checkedPackages, reason, description);
            }
            else
            {
                CreateConsult(consultant, consultTime, resultTypeId, checkedPackages, reason, description);
            }
        }

        private void CreateConsult(ACS.EFMODEL.DataModels.ACS_USER consultant, long? consultTime,
            long resultTypeId, List<PackageGridADO> checkedPackages, string reason, string description)
        {
            CommonParam param = new CommonParam();
            HIS_SERVICE_CONSULT result = null;
            try
            {
                WaitingManager.Show();
                var data = BuildConsultEntity(this.treatmentId, consultant, consultTime, resultTypeId, checkedPackages, reason, description);
                LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => data), data));

                result = new BackendAdapter(param).Post<HIS_SERVICE_CONSULT>(
                    HisRequestUriStore.MOSHIS_HIS_SERVICE_CONSULT_CREATE,
                    ApiConsumers.MosConsumer, data, param);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error("CreateConsult thất bại.", ex);
            }
            finally
            {
                WaitingManager.Hide();
            }

            bool success = result != null && result.ID > 0;
            if (success)
            {
                this.originalSDO = ToSDO(result);
                FillDataToForm(this.originalSDO);
                XtraMessageBox.Show(ResourceMessage.XuLyThanhCong,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogUtil.LogActionSuccess("frmHisServiceConsult", "Create", consultant.LOGINNAME);
                if (this.delegateSelect != null) this.delegateSelect(this.originalSDO);
            }
            else
            {
                XtraMessageBox.Show(ResourceMessage.XuLyThatBai,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogUtil.LogActionFail("frmHisServiceConsult", "Create", consultant.LOGINNAME);
            }
        }

        private void UpdateConsult(ACS.EFMODEL.DataModels.ACS_USER consultant, long? consultTime,
            long resultTypeId, List<PackageGridADO> checkedPackages, string reason, string description)
        {
            CommonParam param = new CommonParam();
            HIS_SERVICE_CONSULT result = null;
            try
            {
                WaitingManager.Show();
                var data = BuildConsultEntity(this.treatmentId, consultant, consultTime, resultTypeId, checkedPackages, reason, description);
                data.ID = this.currentConsultId ?? 0;
                if (data.HIS_CONSULT_PACKAGE != null)
                {
                    foreach (var pkg in data.HIS_CONSULT_PACKAGE)
                    {
                        pkg.SERVICE_CONSULT_ID = data.ID;
                    }
                }
                LogSystem.Debug(LogUtil.TraceData(LogUtil.GetMemberName(() => data), data));

                result = new BackendAdapter(param).Post<HIS_SERVICE_CONSULT>(
                    HisRequestUriStore.MOSHIS_HIS_SERVICE_CONSULT_UPDATE,
                    ApiConsumers.MosConsumer, data, param);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                LogSystem.Error("UpdateConsult thất bại.", ex);
            }
            finally
            {
                WaitingManager.Hide();
            }

            bool success = result != null && result.ID > 0;
            if (success)
            {
                this.originalSDO = ToSDO(result);
                FillDataToForm(this.originalSDO);
                XtraMessageBox.Show(ResourceMessage.XuLyThanhCong,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LogUtil.LogActionSuccess("frmHisServiceConsult", "Update", consultant.LOGINNAME);
                if (this.delegateSelect != null) this.delegateSelect(this.originalSDO);
            }
            else
            {
                XtraMessageBox.Show(ResourceMessage.XuLyThatBai,
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogUtil.LogActionFail("frmHisServiceConsult", "Update", consultant.LOGINNAME);
            }
        }

        private static HIS_SERVICE_CONSULT BuildConsultEntity(long treatmentId,
            ACS.EFMODEL.DataModels.ACS_USER consultant, long? consultTime, long resultTypeId,
            List<PackageGridADO> checkedPackages, string reason, string description)
        {
            return new HIS_SERVICE_CONSULT
            {
                TREATMENT_ID = treatmentId,
                CONSULTANT_LOGINNAME = consultant.LOGINNAME,
                CONSULTANT_USERNAME = consultant.USERNAME,
                CONSULT_RESULT_TYPE_ID = resultTypeId,
                REASON = reason,
                DESCRIPTION = description,
                CONSULT_TIME = consultTime,
                IS_ACTIVE = (short)IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE,
                IS_DELETE = 0,
                HIS_CONSULT_PACKAGE = (checkedPackages ?? new List<PackageGridADO>())
                    .Select(row => new HIS_CONSULT_PACKAGE
                    {
                        ID = row.CONSULT_PACKAGE_ID,    // 0 nếu mới, > 0 nếu đã link từ Mode Edit
                        PACKAGE_ID = row.ID,
                        IS_ACTIVE = (short)IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE,
                        IS_DELETE = 0
                    })
                    .ToList()
            };
        }

        private static HisServiceConsultSDO ToSDO(HIS_SERVICE_CONSULT entity)
        {
            if (entity == null) return null;
            var sdo = new HisServiceConsultSDO { Consult = entity };
            sdo.Packages = (entity.HIS_CONSULT_PACKAGE ?? new List<HIS_CONSULT_PACKAGE>()).ToList();
            return sdo;
        }

        #endregion

        #region Reset

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                ResetForm();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ResetForm()
        {
            try
            {
                this.dxErrorProvider1.ClearErrors();
                this.txtKeyword.Text = "";
                ApplyKeywordFilter();

                if (this.originalSDO != null && this.originalSDO.Consult != null)
                {
                    UncheckAllPackages();
                    FillDataToForm(this.originalSDO);
                }
                else
                {
                    this.currentConsultId = null;
                    this.cboConsultantUser.EditValue = null;
                    this.txtConsultantLoginname.Text = "";
                    this.cboResultType.EditValue = null;
                    this.txtReason.Text = "";
                    this.txtDescription.Text = "";
                    this.dteConsultTime.EditValue = DateTime.Now;

                    UncheckAllPackages();
                    SortAndReloadPackages();
                    SetDefaultValue();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UncheckAllPackages()
        {
            try
            {
                if (this.packageRows == null) return;
                foreach (var row in this.packageRows) row.IS_CHECKED = false;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Shortcuts

        private void frmHisServiceConsult_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.S)
                {
                    e.Handled = true;
                    SaveProcess();
                }
                else if (e.Control && e.KeyCode == Keys.R)
                {
                    e.Handled = true;
                    ResetForm();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        #endregion
    }

    internal static class EnumerableExtensions
    {
        internal static HashSet<long> ToHashSet_Safe(this IEnumerable<long> source)
        {
            return source == null ? new HashSet<long>() : new HashSet<long>(source);
        }
    }
}
