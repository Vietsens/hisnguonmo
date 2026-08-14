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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    public partial class FormHisTreatmentRecordChecking : FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long? treatmentId;
        private List<long> listTreatmentId;
        private List<V_HIS_TREATMENT> ListTreatment;
        private List<EmrDocumentTypeADO> ListDocumentType;
        private EmrDocumentTypeADO CurrentType;
        private List<V_EMR_DOCUMENT> ListDocument;
        private HisTreatmentForRecordCheckingSDO CurrentTreatment;
        private List<InfoRecordADO> ListDataInfoRecord;
        private List<InfoRecordADO> CurrentDataInfoRecord = new List<InfoRecordADO>();
        private List<InfoRecordADO> CurrentInfoRecord = new List<InfoRecordADO>();
        private List<long> ListTypeId = new List<long>()
        {
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__PRESCRIPTION,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__INFUSION,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__CARE,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__MEDI_REACT,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRANSFUSION,
        };

        private List<long> ReqTypeId = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
        };

        private int lastRowHandle = -1;
        private DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        private string lastGrid = "";
        private DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        /// <summary>Worker reads/writes control state to local SQLite. Instance scoped - not shared between form instances.</summary>
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        /// <summary>Control states of this module.</summary>
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool IsLoadFirstForm = true;

        List<V_EMR_SIGN> lstVEmrSign = new List<V_EMR_SIGN>();
        Dictionary<long, V_EMR_SIGN> dicVEmrSign = new Dictionary<long, V_EMR_SIGN>();

        /// <summary>Config MOS.HIS_TREATMENT.IS_AUTO_APPROVAL_STORE == "1" -> tự động duyệt hồ sơ khi Đạt.</summary>
        private bool isAutoApprovalStore = false;
        /// <summary>Có quyền HIS000054 - Duyệt.</summary>
        private bool hasPermissionApprove = false;
        /// <summary>Có quyền HIS000055 - Hủy duyệt.</summary>
        private bool hasPermissionUnapprove = false;

        /// <summary>Mã control ACS nút Duyệt.</summary>
        private const string CONTROL_CODE__DUYET = "HIS000056";
        /// <summary>Mã control ACS nút Hủy duyệt.</summary>
        private const string CONTROL_CODE__HUY_DUYET = "HIS000055";
        /// <summary>
        /// Trạng thái APPROVAL_STORE_STT_ID = 3 (Đạt). Mapping: 1 = Duyệt, 2 = Chưa đạt, 3 = Đạt.
        /// TODO: thay bằng IMSys.DbConfig.HIS_RS.HIS_TREATMENT.APPROVAL_STORE_STT_ID__DAT khi backend bổ sung constant.
        /// </summary>
        private const long APPROVAL_STORE_STT_ID__DAT = 3;

        /// <summary>Plugin id, also the key used to store control states locally.</summary>
        internal const string MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking";

        /// <summary>True when the user picked a row in the treatment grid (bold highlight).</summary>
        private bool hasSelectedTreatment = false;
        /// <summary>ACS controls granted to the current account - drives the approval buttons.</summary>
        private List<ACS_CONTROL> controlAcs;
        #endregion

        public FormHisTreatmentRecordChecking()
            : base()
        {
            InitializeComponent();
        }

        public FormHisTreatmentRecordChecking(Inventec.Desktop.Common.Modules.Module moduleData, long? treatmentId)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                lciGC_Treatment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //
                this.moduleData = moduleData;
                this.treatmentId = treatmentId;
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormHisTreatmentRecordChecking(Inventec.Desktop.Common.Modules.Module moduleData, long? treatmentId, List<long> listTreatmentId)
            : this(moduleData, treatmentId)
        {
            try
            {
                this.listTreatmentId = listTreatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormHisTreatmentRecordChecking_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                GetControlAcs();
                InitConfigAndPermission();
                SetCaptionByLanguageKey();
                if (this.listTreatmentId != null)
                {
                    lciGC_Treatment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    FillDataToGridTreatment(this.listTreatmentId);
                }
                InitComboRequestDoctor();
                InitComboTreatmentStatus();
                InitGridEmrDocumentType();
                SetDefaultValueControl();
                SetDefaultFilterValue();
                ApplyModeUI();
                ProcessCaptionGridInfoRecord();
                InitControlState();
                FillDataToGrid();
                SetDefaultProperties();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultProperties()
        {
            try
            {
                //var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                //if (screenWidth >= 1600)
                //{
                //    Gv_Treatment.OptionsView.ColumnAutoWidth = true;
                //}
                SetCustomSizeForGridView(ref Gv_Treatment);
                SetCustomSizeForGridView(ref Gv_EmrDocument);
                SetCustomSizeForGridView(ref Gv_InfoRecord);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(List<long> listId)
        {
            try
            {
                if (listId != null && listId.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.IDs = listId;
                    this.ListTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                }
                else
                {
                    this.ListTreatment = null;
                }
                Gc_Treatment.BeginUpdate();
                Gc_Treatment.DataSource = this.ListTreatment;
                Gc_Treatment.EndUpdate();
                if (this.ListTreatment != null && this.ListTreatment.Count == 1)
                {
                    this.hasSelectedTreatment = true;
                    Gv_Treatment.FocusedRowHandle = 0;
                    TxtTreatmentCode.Text = this.ListTreatment.First().TREATMENT_CODE;
                    // Do NOT call FillDataToGrid() here: this method runs before InitGridEmrDocumentType(),
                    // so ListDocumentType is still null and FillDataToGrid would exit at its guard.
                    // The Load event calls FillDataToGrid() after the document types are loaded.
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                IsLoadFirstForm = true;
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(MODULE_LINK);
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == chkUuTien.Name)
                        {
                            chkUuTien.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkIncludeCancelDoc.Name)
                        {
                            chkIncludeCancelDoc.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkNoDocument.Name)
                        {
                            chkNoDocument.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkNotFullySigned.Name)
                        {
                            chkNotFullySigned.Checked = item.VALUE == "1";
                        }
                    }
                }
                IsLoadFirstForm = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataTreatment()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisTreatmentForRecordCheckingFilter filter = new HisTreatmentForRecordCheckingFilter();
                if (!String.IsNullOrWhiteSpace(TxtTreatmentCode.Text))
                {
                    string code = TxtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        TxtTreatmentCode.Text = code;
                    }

                    filter.TREATMENT_CODE__EXACT = TxtTreatmentCode.Text;
                }
                else if (treatmentId.HasValue)
                {
                    filter.TREATMENT_ID = treatmentId;
                }
                else
                {
                    filter.TREATMENT_ID = -1;
                }

                CurrentTreatment = new BackendAdapter(paramCommon).Get<HisTreatmentForRecordCheckingSDO>("api/HisTreatment/GetInfoForRecordChecking", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, paramCommon);
                if (CurrentTreatment != null)
                {
                    TxtTreatmentCode.Text = CurrentTreatment.Treatment.TREATMENT_CODE;

                    FillDataToControl(CurrentTreatment.Treatment);
                    ProcessDataADO();
                }
                else
                {
                    SetDefaultValueControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Builds an id-keyed department lookup from the local cache.
        /// Call once outside loops - O(1) per lookup afterwards.
        /// </summary>
        private Dictionary<long, HIS_DEPARTMENT> BuildDepartmentLookup()
        {
            Dictionary<long, HIS_DEPARTMENT> result = new Dictionary<long, HIS_DEPARTMENT>();
            try
            {
                List<HIS_DEPARTMENT> departments = BackendDataWorker.Get<HIS_DEPARTMENT>();
                if (departments != null)
                {
                    foreach (var item in departments)
                    {
                        if (!result.ContainsKey(item.ID)) result.Add(item.ID, item);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Dictionary<long, HIS_DEPARTMENT>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Builds an id-keyed service request type lookup from the local cache.
        /// </summary>
        private Dictionary<long, HIS_SERVICE_REQ_TYPE> BuildServiceReqTypeLookup()
        {
            Dictionary<long, HIS_SERVICE_REQ_TYPE> result = new Dictionary<long, HIS_SERVICE_REQ_TYPE>();
            try
            {
                List<HIS_SERVICE_REQ_TYPE> types = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>();
                if (types != null)
                {
                    foreach (var item in types)
                    {
                        if (!result.ContainsKey(item.ID)) result.Add(item.ID, item);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new Dictionary<long, HIS_SERVICE_REQ_TYPE>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Returns the department for the given id, or null when the id is null / unknown.
        /// </summary>
        private HIS_DEPARTMENT GetDepartment(Dictionary<long, HIS_DEPARTMENT> dicDepartment, long? departmentId)
        {
            HIS_DEPARTMENT result = null;
            try
            {
                if (dicDepartment != null && departmentId.HasValue)
                {
                    dicDepartment.TryGetValue(departmentId.Value, out result);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void GvEmrDocumentType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    CurrentType = (EmrDocumentTypeADO)GvEmrDocumentType.GetFocusedRow();
                    ProcessCaptionGridInfoRecord();
                    ProcessFillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_InfoRecord_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                // Both modes use the same handler: ProcessDataGridDocument() resolves the
                // documents of the focused order through GetDocumentByInfoRecod(), which works
                // across records because SEARCH_CODE is unique.
                ProcessDataGridDocument();

                var row = (InfoRecordADO)Gv_InfoRecord.GetFocusedRow();
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATOR" && !string.IsNullOrEmpty(row.CREATOR))
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfoUser").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfoUser'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.InfoUser' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.CREATOR);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitConfigAndPermission()
        {
            try
            {
                string configValue = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(
                    "MOS.HIS_TREATMENT.IS_AUTO_APPROVAL_STORE");
                isAutoApprovalStore = configValue == "1";

                hasPermissionApprove = controlAcs != null
                    && controlAcs.Any(o => o.CONTROL_CODE == CONTROL_CODE__DUYET);
                hasPermissionUnapprove = controlAcs != null
                    && controlAcs.Any(o => o.CONTROL_CODE == CONTROL_CODE__HUY_DUYET);

                // Nút Duyệt: config=1 (tự động duyệt) -> ẨN hẳn; config≠1 -> hiển thị (enable theo quyền + trạng thái).
                layoutControlItem13.Visibility = isAutoApprovalStore
                    ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never
                    : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                // Nút Hủy duyệt: luôn hiển thị.
                layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                Inventec.Common.Logging.LogSystem.Debug("InitConfigAndPermission____"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => isAutoApprovalStore), isAutoApprovalStore)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => hasPermissionApprove), hasPermissionApprove)
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => hasPermissionUnapprove), hasPermissionUnapprove));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
