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
using Inventec.UC.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.TreatmentLatchApproveStore.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.TreatmentLatchApproveStore.TreatmentLatchApproveStore
{
    public partial class frmTreatmentLatchApproveStore : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.L_HIS_TREATMENT_3 currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<V_HIS_ROOM> hisRoomList;

        V_HIS_ROOM currentRoom;
        List<HIS_DEPARTMENT> _EndDepartmentSelecteds;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.Plugins.TreatmentLatchApproveStore";
        #endregion

        #region Construct
        public frmTreatmentLatchApproveStore(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method

        private void frmTreatmentLatchApproveStore_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Show()
        {
            chkDepartment.Checked = false;
            SetDefaultValue();
            //Focus default
            SetDefaultFocus();
            currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == moduleData.RoomId).FirstOrDefault();


            InitCheck();
            InitCombo();

            InitControlState();

            FillDataToControl();

            LoadCboStatus();
            LoadCboTDLTreatmentTypeID();
            LoadCboTDLPatientTypeID();

            //set ngon ngu
            SetCaptionByLanguagekey();

        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkDepartment.Name)
                        {
                            chkDepartment.Checked = item.VALUE == "1";
                        }
                    }
                }

                GridCheckMarksSelection gridCheckMarkFinishDepartment = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkFinishDepartment.ClearSelection(cboFinishDepartment.Properties.View);
                GridCheckMarksSelection gridCheckMark = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                ProcessSelectBusiness(currentRoom.DEPARTMENT_ID, gridCheckMark);
                cboFinishDepartment.Enabled = chkDepartment.Checked;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBusiness(long departmentId, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_DEPARTMENT> ds = cboFinishDepartment.Properties.DataSource as List<HIS_DEPARTMENT>;
                var row = ds != null ? ds.FirstOrDefault(o => o.ID == departmentId) : null;
                List<HIS_DEPARTMENT> dt = new List<HIS_DEPARTMENT>();
                if (row != null)
                {
                    dt.Add(row);
                    cboFinishDepartment.Text = row.DEPARTMENT_NAME;
                }
                gridCheckMark.SelectAll(dt);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCombo()
        {
            try
            {
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;
                var data = new BackendAdapter(new CommonParam()).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, null);

                cboFinishDepartment.Properties.DataSource = data;
                cboFinishDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboFinishDepartment.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboFinishDepartment.Properties.View.Columns.AddField("DEPARTMENT_NAME");

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cboFinishDepartment.Properties.PopupFormWidth = 200;
                cboFinishDepartment.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboFinishDepartment.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cboFinishDepartment.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboFinishDepartment.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__FinishDepartment);
                cboFinishDepartment.Properties.Tag = gridCheck;
                cboFinishDepartment.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboFinishDepartment.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__FinishDepartment(object sender, EventArgs e)
        {
            try
            {
                _EndDepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _EndDepartmentSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboTDLPatientTypeID()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboTDLPatientTypeID, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboTDLTreatmentTypeID()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "TREATMENT_TYPE_CODE", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboTDLTreatmentTypeID, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboStatus()
        {
            try
            {
                List<FilterADO> _filterAdos = new List<FilterADO>();
                _filterAdos.Add(new FilterADO(1, "Null||02", "Chưa đạt"));
                _filterAdos.Add(new FilterADO(2, "Null", "Chưa chốt"));
                _filterAdos.Add(new FilterADO(3, "1", "Đã chốt"));
                _filterAdos.Add(new FilterADO(4, "2", "Bị từ chối"));
                _filterAdos.Add(new FilterADO(5, "", "tất cả"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TYPE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TYPE_NAME", "ID", columnInfos, false, 100);
                ControlEditorLoader.Load(cboApprovalStoreSttID, _filterAdos, controlEditorADO);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguagekey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentLatchApproveStore.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentLatchApproveStore.TreatmentLatchApproveStore.frmTreatmentLatchApproveStore).Assembly);
                ////Gan gia tri cho cac control editor co Text/Caption/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDuyetDDK.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.btnDuyetDDK.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnDuyetDDK.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.bbtnDuyetDDK.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnBarF2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.bbtnBarF2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnBarF3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.bbtnBarF3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.bar2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTdPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.txtTdPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentLatchApproveStore.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();

                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_3>> apiResult = null;
                HisTreatmentLView3Filter filter = new HisTreatmentLView3Filter();
                SetFilterNavBar(ref filter);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("filter_____:", filter));
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //filter.IS_PAUSE = true;
                filter.IS_STORED = false;
                


                if (dateEditFrom.EditValue != null && dateEditFrom.DateTime != DateTime.MinValue)
                {
                    filter.OUT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dateEditFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }

                if (dateEditTo.EditValue != null && dateEditTo.DateTime != DateTime.MinValue)
                {
                    filter.OUT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dateEditTo.EditValue).ToString("yyyyMMdd") + "000000");
                }

                if (this.cboApprovalStoreSttID.EditValue != null)
                {
                    if ((long)this.cboApprovalStoreSttID.EditValue == 1)
                    {
                        filter.APPROVAL_STORE_STT_ID__NULL_OR_EQUAL = 2;
                    }
                    else if ((long)this.cboApprovalStoreSttID.EditValue == 2)
                    {
                        filter.HAS_APPROVAL_STORE_STT_ID = false;
                    }
                    else if ((long)this.cboApprovalStoreSttID.EditValue == 5)
                    {
                        filter.HAS_APPROVAL_STORE_STT_ID = null;
                    }
                    else if ((long)this.cboApprovalStoreSttID.EditValue == 4)
                    {
                        filter.APPROVAL_STORE_STT_ID = 2;
                    }
                    else if ((long)this.cboApprovalStoreSttID.EditValue == 3)
                    {
                        filter.APPROVAL_STORE_STT_ID = 1;
                    }
                }

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<L_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_3>)apiResult.Data.Where(s=>s.OUT_DATE != null).ToList();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("data__:", data));
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisTreatmentLView3Filter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtTdPatientCode.Text))
                {
                    filter.PATIENT_CODE__EXACT = txtTdPatientCode.Text.Trim();
                }
                if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    filter.TREATMENT_CODE__EXACT = txtTreatmentCode.Text.Trim();
                }


                if (cboTDLTreatmentTypeID.EditValue != null)
                {
                    filter.TDL_TREATMENT_TYPE_ID = long.Parse(cboTDLTreatmentTypeID.EditValue.ToString());
                }

                if (cboTDLPatientTypeID.EditValue != null)
                {
                    filter.TDL_PATIENT_TYPE_ID = long.Parse(cboTDLPatientTypeID.EditValue.ToString());
                }
                if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                {
                    filter.END_DEPARTMENT_IDs = _EndDepartmentSelecteds.Select(o => o.ID).ToList();
                }
                GridCheckMarksSelection gridCheckMarkBusiness = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                {
                    List<long> _Ids = new List<long>();
                    foreach (HIS_DEPARTMENT rv in gridCheckMarkBusiness.Selection)
                    {
                        if (rv != null && !_Ids.Contains(rv.ID))
                            _Ids.Add(rv.ID);
                    }

                    filter.END_DEPARTMENT_IDs = _Ids;
                }
                else
                {
                    filter.END_DEPARTMENT_ID = hisRoomList.FirstOrDefault().DEPARTMENT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                hisRoomList = new List<V_HIS_ROOM>();
                CommonParam param = new CommonParam();
                HisRoomFilter filter = new HisRoomFilter();
                filter.ID = moduleData.RoomId;
                hisRoomList = new BackendAdapter(param).Get<List<V_HIS_ROOM>>(HisRequestUriStore.HIS_ROM_GETVIEW, ApiConsumers.MosConsumer, filter, param);

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("Dữ liệu hisRoomList: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => hisRoomList), hisRoomList));

                txtSearch.Text = "";
                txtTdPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                dateEditFrom.DateTime = DateTime.Now;
                dateEditTo.DateTime = DateTime.Now;
                cboTDLPatientTypeID.EditValue = null;
                cboTDLPatientTypeID.Properties.Buttons[1].Visible = false;
                cboTDLTreatmentTypeID.EditValue = null;
                cboTDLTreatmentTypeID.Properties.Buttons[1].Visible = false;

                cboApprovalStoreSttID.EditValue = (long)1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region event

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        txtTdPatientCode.Text = "";
                        txtSearch.Text = "";
                        var TreatmentCode = txtTreatmentCode.Text.Trim();
                        if (TreatmentCode.Length < 12 && checkDigit(TreatmentCode))
                        {
                            TreatmentCode = string.Format("{0:000000000000}", Convert.ToInt64(TreatmentCode));
                            txtTreatmentCode.Text = TreatmentCode;
                        }
                        FillDataToControl();
                        txtTreatmentCode.SelectAll();
                    }
                    else
                    {
                        FillDataToControl();
                        txtTdPatientCode.Focus();
                        txtTdPatientCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTdPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTdPatientCode.Text))
                    {
                        txtTreatmentCode.Text = "";
                        txtSearch.Text = "";
                        var TdPatientCode = txtTdPatientCode.Text.Trim();
                        if (TdPatientCode.Length < 10 && checkDigit(TdPatientCode))
                        {
                            TdPatientCode = string.Format("{0:0000000000}", Convert.ToInt64(TdPatientCode));
                            txtTdPatientCode.Text = TdPatientCode;
                        }
                        FillDataToControl();
                        txtTdPatientCode.SelectAll();
                    }
                    else
                    {
                        txtSearch.Focus();
                        txtSearch.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnBarF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCode.Focus();
                txtTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnBarF3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTdPatientCode.Focus();
                txtTdPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboTDLPatientTypeID_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTDLPatientTypeID.EditValue = null;
                    cboTDLPatientTypeID.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTDLTreatmentTypeID_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTDLTreatmentTypeID.EditValue = null;
                    cboTDLTreatmentTypeID.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApprovalStoreSttID_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboApprovalStoreSttID.EditValue = null;
                    cboApprovalStoreSttID.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnDuyetDDK_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                List<L_HIS_TREATMENT_3> _dataChecks = new List<L_HIS_TREATMENT_3>();
                if (gridView1.RowCount > 0)
                {
                    for (int i = 0; i < gridView1.SelectedRowsCount; i++)
                    {
                        if (gridView1.GetSelectedRows()[i] >= 0)
                        {
                            L_HIS_TREATMENT_3 data = new L_HIS_TREATMENT_3();
                            data = (L_HIS_TREATMENT_3)gridView1.GetRow(gridView1.GetSelectedRows()[i]);
                            if (data.APPROVAL_STORE_STT_ID == null || data.APPROVAL_STORE_STT_ID == 2)
                            {
                                _dataChecks.Add(data);
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData("Dữ liệu _dataChecks: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => _dataChecks), _dataChecks));
                if (_dataChecks != null && _dataChecks.Count > 0)
                {
                    var listID = _dataChecks.Select(o => o.ID).ToList();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Dữ liệu listID: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => listID), listID));

                    CommonParam paramCommon = new CommonParam();
                    var apiData = new BackendAdapter(paramCommon).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_APPROVALSTORE, ApiConsumers.MosConsumer, listID, paramCommon);
                    if (apiData != null)
                    {
                        success = true;
                        FillDataToControl();
                    }


                    #region Hien thi message thong bao
                    MessageManager.Show(this, paramCommon, success);
                    #endregion
                }
                else
                {
                    MessageBox.Show("Bạn chưa chọn hồ sơ cần duyệt đạt điều kiện lưu hồ sơ bệnh án. Vui lòng kiểm tra lại.");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnDuyetDDK_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDuyetDDK_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    L_HIS_TREATMENT_3 data = (L_HIS_TREATMENT_3)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "APPROVAL_STORE_STT_ID_STR")
                    {
                        if (data.APPROVAL_STORE_STT_ID == 1)
                        {
                            e.RepositoryItem = btnHuyChot;
                        }
                        else
                        {
                            e.RepositoryItem = btnChot;
                        }

                    }
                    if (e.Column.FieldName == "EDIT")
                    {
                        if (true)
                        {
                            e.RepositoryItem = repositoryItemButtonEditE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditD;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTDLPatientTypeID_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboTDLPatientTypeID.Text))
                {
                    cboTDLPatientTypeID.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTDLTreatmentTypeID_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboTDLTreatmentTypeID.Text))
                {
                    cboTDLTreatmentTypeID.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApprovalStoreSttID_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(cboApprovalStoreSttID.Text))
                {
                    cboApprovalStoreSttID.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #endregion

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    L_HIS_TREATMENT_3 data = (L_HIS_TREATMENT_3)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "EXP_MEST_STT_ICON")
                        {
                            if (data.APPROVAL_STORE_STT_ID == null)
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.APPROVAL_STORE_STT_ID == 2)
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.APPROVAL_STORE_STT_ID == 1)
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_NAME_DISPLAY")
                        {
                            e.Value = String.Format("{0} - {1}", data.APPROVAL_LOGINNAME, data.APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot thoi gian chot duyet ho so benh an APPROVAL_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "UNAPPROVAL_NAME_DISPLAY")
                        {
                            e.Value = String.Format("{0} - {1}", data.UNAPPROVAL_LOGINNAME, data.UNAPPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "UNAPPROVAL_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.UNAPPROVAL_TIME ?? 0);

                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot thoi gian huy chot duyet ho so benh an UNAPPROVAL_TIME", ex);
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var row = (L_HIS_TREATMENT_3)gridView1.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "EDIT")
                        {

                            try
                            {
                                edit(row);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }


                        }
                        if (hi.Column.FieldName == "DETAIL_DATA_DISPLAY")
                        {
                            //WaitingManager.Show();
                            //// popup yêu cầu xem
                            //List<object> _listObj = new List<object>();
                            //_listObj.Add(row.ID);
                            //WaitingManager.Hide();
                            //HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisTreatmentRecordChecking", this.moduleData.RoomId, this.moduleData.RoomTypeId, _listObj);

                            Inventec.Desktop.Common.Modules.Module currentModule = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisTreatmentRecordChecking").FirstOrDefault();
                            if (currentModule == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisTreatmentRecordChecking'");
                            if (currentModule.IsPlugin && currentModule.ExtensionInfo != null)
                            {
                                currentModule.RoomId = this.moduleData.RoomId;
                                currentModule.RoomTypeId = this.moduleData.RoomTypeId;
                                List<object> listArgs = new List<object>();
                                listArgs.Add(row.ID);
                                listArgs.Add(currentModule);
                                var extenceInstance = PluginInstance.GetPluginInstance(currentModule, listArgs);
                                if (extenceInstance == null)
                                {
                                    throw new ArgumentNullException("currentModule is null");
                                }

                                ((Form)extenceInstance).ShowDialog();
                            }
                            else
                            {
                                MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                            }
                        }
                        if (hi.Column.FieldName == "APPROVAL_STORE_STT_ID_STR")
                        {
                            if (row.APPROVAL_STORE_STT_ID == 1)
                            {
                                try
                                {
                                    CommonParam param = new CommonParam();
                                    if (MessageBox.Show("Bạn có muốn hủy chốt duyệt hồ sơ bệnh án", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        bool success = false;
                                        List<long> listID = new List<long>();
                                        listID.Add(row.ID);
                                        var apidata = new BackendAdapter(param).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_UNAPPROVALSTORE, ApiConsumers.MosConsumer, listID, param);
                                        if (apidata != null)
                                        {
                                            success = true;
                                            FillDataToControl();
                                            currentData = ((List<L_HIS_TREATMENT_3>)gridView1.DataSource).FirstOrDefault();
                                        }
                                        MessageManager.Show(this, param, success);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }
                            }
                            else
                            {
                                try
                                {
                                    CommonParam param = new CommonParam();
                                    if (MessageBox.Show("Bạn có muốn chốt duyệt hồ sơ bệnh án", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        bool success = false;
                                        List<long> listID = new List<long>();
                                        listID.Add(row.ID);
                                        var apidata = new BackendAdapter(param).Post<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_APPROVALSTORE, ApiConsumers.MosConsumer, listID, param);
                                        if (apidata != null)
                                        {
                                            success = true;
                                            FillDataToControl();
                                            currentData = ((List<L_HIS_TREATMENT_3>)gridView1.DataSource).FirstOrDefault();
                                        }
                                        MessageManager.Show(this, param, success);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Warn(ex);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                //edit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void edit(L_HIS_TREATMENT_3 row)
        {

            try
            {
                Inventec.Desktop.Common.Modules.Module currentModule_ = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MRSummaryList").FirstOrDefault();
                if (currentModule_ == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.MRSummaryList'");
                if (currentModule_.IsPlugin && currentModule_.ExtensionInfo != null)
                {

                    MRSummaryDetailADO ado = new MRSummaryDetailADO();
                    List<HIS_MR_CHECK_SUMMARY> CheckSummary_ = new List<HIS_MR_CHECK_SUMMARY>();
                    CommonParam param = new CommonParam();
                    HisMrCheckSummaryFilter filter = new HisMrCheckSummaryFilter();
                    filter.TREATMENT_ID = row.ID;
                    CheckSummary_ = new BackendAdapter(param).Get<List<HIS_MR_CHECK_SUMMARY>>("/api/HisMrCheckSummary/Get", ApiConsumers.MosConsumer, filter, param);
                    if (CheckSummary_ != null && CheckSummary_.Count > 0)
                    {
                        ado.CheckSummary = new HIS_MR_CHECK_SUMMARY();
                        ado.CheckSummary = CheckSummary_.FirstOrDefault();
                    }
                    ado.processType = MRSummaryDetailADO.OpenFrom.TreatmentLatchApproveStore;
                    ado.TreatmentId = row.ID;

                    currentModule_.RoomId = this.moduleData.RoomId;
                    currentModule_.RoomTypeId = this.moduleData.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);
                    listArgs.Add(row.ID);
                    listArgs.Add(row);

                    listArgs.Add(currentModule_);
                    var extenceInstance = PluginInstance.GetPluginInstance(currentModule_, listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("currentModule is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
                else
                {
                    MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboFinishDepartment_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string endDepartment = "";
                if (_EndDepartmentSelecteds != null && _EndDepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _EndDepartmentSelecteds)
                    {
                        endDepartment += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = endDepartment;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkDepartment_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkDepartment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkDepartment.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkDepartment.Name;
                    csAddOrUpdate.VALUE = (chkDepartment.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                GridCheckMarksSelection gridCheckMarkFinishDepartment = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkFinishDepartment.ClearSelection(cboFinishDepartment.Properties.View);
                GridCheckMarksSelection gridCheckMark = cboFinishDepartment.Properties.Tag as GridCheckMarksSelection;
                ProcessSelectBusiness(currentRoom.DEPARTMENT_ID, gridCheckMark);
                cboFinishDepartment.Enabled = chkDepartment.Checked;


                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            ChangeAllEditColumn(true);
        }

        private void ChangeAllEditColumn(bool IsAllowEdit)
        {
            try
            {
                if (gridView1.FocusedRowHandle > -1)
                {
                    var columnFocus = gridView1.FocusedColumn;
                    if (columnFocus.FieldName == "TREATMENT_CODE" || columnFocus.FieldName == "TDL_PATIENT_CODE" || columnFocus.FieldName == "REJECT_STORE_REASON")
                    {
                        foreach (var item in gridView1.Columns.ToList())
                        {
                            if (item.FieldName == columnFocus.FieldName)
                            {
                                item.OptionsColumn.AllowEdit = IsAllowEdit;
                                item.OptionsColumn.ReadOnly = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {
                ChangeAllEditColumn(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
