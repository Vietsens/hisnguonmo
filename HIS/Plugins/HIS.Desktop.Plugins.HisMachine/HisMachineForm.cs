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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using EMR.WCF.DCO;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisMachine.ADO;
using HIS.Desktop.Plugins.HisMachine.Properties;
using HIS.Desktop.Plugins.HisMachine.Validation;
using HIS.Desktop.Plugins.HisMachine.XML;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.SettingSignInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.SignLibrary.ServiceSign;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisMachine
{
    public partial class HisMachineForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        MachineADO currentData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        int positionHandle = -1;
        List<V_HIS_ROOM> listRoom;
        List<V_HIS_ROOM> listRoomSelecteds;
        string[] roomNew;
        #endregion

        public HisMachineForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {

            InitializeComponent();
            currentModule = module;
            this.delegateSelect = delegateData;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public HisMachineForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                //pagingGrid = new PagingGrid();
                currentModule = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Loadform
        private void HisMachineForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void MeShow()
        {
            InitComboDepartment();
            InitControlState();
            InitComboConfigTimeConflict();
            InitCheck(cboRoom, SelectionGrid__ROOM_NAME);
            InitComboRoom(cboRoom, BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList(), "ROOM_NAME", "ID");
            SetDefaultValue();

            EnableControlChanged(this.ActionType);

            FillDatagctFormList();

            SetCaptionByLanguageKey();

            InitTabIndex();

            ValidateForm();

            SetDefaultFocus();
        }
        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkSign.Name)
                        {
                            SettingSignADO = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingSignADO>(item.VALUE);
                            chkSign.Checked = SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber);
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                chkSign.Checked = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboDepartment()
        {
            try
            {
                var listDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã khoa", 80, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 270, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDepartment, listDepartment, controlEditorADO);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboConfigTimeConflict()
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.Add("ID", typeof(short));
                dt.Columns.Add("NAME", typeof(string));

                //dt.Rows.Add((short)0, "Không kiểm tra");
                dt.Rows.Add((short)1, "Cảnh báo");
                dt.Rows.Add((short)2, "Chặn");

                cboConfigTimeConflict.Properties.DataSource = dt;
                cboConfigTimeConflict.Properties.ValueMember = "ID";      // lưu 0/1/2
                cboConfigTimeConflict.Properties.DisplayMember = "NAME";  // hiển thị tên
                cboConfigTimeConflict.Properties.NullText = "";
                cboConfigTimeConflict.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

                cboConfigTimeConflict.ForceInitialize();
                var view = cboConfigTimeConflict.Properties.View;
                view.Columns.Clear();

                view.OptionsView.ShowColumnHeaders = false;   // nếu muốn có header
                view.OptionsView.ShowIndicator = false;

                var colId = view.Columns.AddField("ID");
                colId.Caption = "ID";
                colId.Visible = true;
                colId.VisibleIndex = 0;
                colId.Width = 40;

                var colName = view.Columns.AddField("NAME");
                colName.Caption = "Tên";
                colName.Visible = true;
                colName.VisibleIndex = 1;
                colName.Width = 160;

                cboConfigTimeConflict.Properties.PopupFormWidth = 220;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__ROOM_NAME(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<V_HIS_ROOM> sgSelectedNews = new List<V_HIS_ROOM>();
                    foreach (V_HIS_ROOM rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0)
                            {
                                sb.Append(",");
                            }
                            sb.Append(rv.ROOM_NAME.ToString());
                            sgSelectedNews.Add(rv);

                        }

                    }
                    this.listRoomSelecteds = new List<V_HIS_ROOM>();
                    this.listRoomSelecteds.AddRange(sgSelectedNews);

                }
                this.cboRoom.Text = sb.ToString();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void SetDefaultFocus()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;

                txtFind.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtName, 200);
                ValidationSingleControl(txtCode, 100);

                ValidationMaxLength(txtSymbol, 500);
                ValidationMaxLength(txtManufacturerName, 500);
                ValidationMaxLength(txtNationalName, 500);
                ValidationMaxLength(txtManufacturedYear, 4);
                ValidationMaxLength(txtUsedYear, 4);
                ValidationMaxLength(txtCirculationNumber, 50);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseControl control, int maxLength)
        {
            try
            {
                ValidateMaxLengthAndRequired validRule = new ValidateMaxLengthAndRequired();
                validRule.textEdit = control;
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationMaxLength(BaseControl control, int maxLength)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.textEdit = control;
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtName", 1);
                dicOrderTabIndexControl.Add("txtCode", 0);


                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {    ////Khoi tao doi tuong resource

                HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMachine.Resource.Lang", typeof(HIS.Desktop.Plugins.HisMachine.HisMachineForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControl1.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.bar2.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.bbtnAdd.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.barButtonItem2.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.barButtonItem3.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem4.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.barButtonItem4.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControl3.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControl4.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.grlSTT.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.gclCode.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.gclName.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.gridColumn1.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.gridColumn2.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("HisMachineForm.gridColumn3.Caption", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.btnEdit.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.btnReset.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.btnAdd.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControl2.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.btnFind.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControlItem2.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControlItem3.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControlItem13.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControlItem14.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.layoutControlItem15.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.bar1.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("HisMachineForm.Text", HIS.Desktop.Plugins.RoomGroup.Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void FillDatagctFormList()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {

                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_MACHINE>> apiResult = null;
                HisMachineFilter filter = new HisMachineFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_MACHINE>>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_MACHINE>)apiResult.Data;
                    if (data != null)
                    {
                        AutoMapper.Mapper.CreateMap<HIS_MACHINE, MachineADO>();
                        var machines = AutoMapper.Mapper.Map<List<MachineADO>>(data);
                        gridView1.GridControl.DataSource = machines;
                        rowCount = (machines == null ? 0 : machines.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisMachineFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtFind.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void SetDefaultValue()
        //{
        //    try
        //    {
        //        this.ActionType = GlobalVariables.ActionAdd;

        //        txtFind.Text = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        #endregion

        #region event
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();


            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (IsRequiredSourceName)
                {
                    if (string.IsNullOrWhiteSpace(txtSourceName.Text))
                    {
                        dxErrorProvider1.SetError(txtSourceName, "Trường dữ liệu bắt buộc", ErrorType.Warning);
                    }
                    else
                    {
                        dxErrorProvider1.SetError(txtSourceName, "", ErrorType.None);
                    }
                }
                else dxErrorProvider1.SetError(txtSourceName, "", ErrorType.None);
                if (dxErrorProvider1.HasErrors) return;
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                WaitingManager.Show();
                HIS_MACHINE updateDTO = new HIS_MACHINE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);

                    string roomString = currentData.ROOM_IDS;
                    if (!String.IsNullOrWhiteSpace(roomString) && roomString.Length > 0)
                    {
                        roomNew = roomString.Split(',');
                        for (int i = 0; i < roomNew.Count(); i++)
                        {
                            long m = Inventec.Common.TypeConvert.Parse.ToInt32(roomNew[i]);
                            listRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == m).ToList();
                        }
                    }
                }

                UpdateDTOFromDataForm(ref updateDTO);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MACHINE>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        RefeshDataAfterSave(resultData);
                        FillDatagctFormList();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MACHINE>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDatagctFormList();
                        //RefeshDataAfterSave(resultData);
                    }
                }

                if (success)
                {
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                txtCode.Focus();
                txtCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }
        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                ResetFormData();
                EnableControlChanged(this.ActionType);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
                listRoomSelecteds = new List<V_HIS_ROOM>();
                cboRoom.Text = "";
                SetValueRoom(this.cboRoom, this.listRoomSelecteds, BackendDataWorker.Get<V_HIS_ROOM>().OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.ROOM_NAME).Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList());
                txtSeri.Text = "";
                txtMachineGroupCode.Text = "";
                txtIntegrateAddress.Text = "";
                txtCode.Text = "";
                txtName.Text = "";
                txtFind.Text = "";
                txtServiceOnDay.Text = "";
                chkIsKidney.CheckState = CheckState.Unchecked;
                cboSource.SelectedIndex = -1;
                txtSymbol.Text = "";
                txtManufacturerName.Text = "";
                txtNationalName.Text = "";
                txtManufacturedYear.Text = "";
                txtUsedYear.Text = "";
                txtCirculationNumber.Text = "";
                cboDepartment.EditValue = null;
                dteContractTo.EditValue = null;
                dteFromTime.EditValue = null;
                dteToTime.EditValue = null;
                dteContractFrom.EditValue = null;
                txtSourceName.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetValueRoom(GridLookUpEdit grdLookUpEdit, List<V_HIS_ROOM> listSelect, List<V_HIS_ROOM> listAll)
        {
            try
            {
                if (listSelect != null)
                {
                    //EmrBusinessFilter filter = new EmrBusinessFilter();
                    //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;


                    grdLookUpEdit.Properties.DataSource = listAll;
                    var selectFilter = listAll.Where(o => listSelect.Exists(p => o.ID == p.ID)).OrderByDescending(o => o.MODIFY_TIME).ToList();
                    GridCheckMarksSelection gridCheckMark = grdLookUpEdit.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMark.Selection.Clear();
                    gridCheckMark.Selection.AddRange(selectFilter);

                }
                grdLookUpEdit.Text = null;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void filldatatocboRoom(HIS_MACHINE data)
        {
            try
            {
                if (data.ROOM_IDS != null)
                {

                    listRoomSelecteds = new List<V_HIS_ROOM>();
                    cboRoom.Text = "";
                    SetValueRoom(this.cboRoom, this.listRoomSelecteds, BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList());
                    string roomstring = data.ROOM_IDS;
                    roomNew = roomstring.Split(',');
                    if (roomNew.Count() == 1)
                    {
                        long idRoom = Inventec.Common.TypeConvert.Parse.ToInt32(roomNew.First());
                        listRoomSelecteds = BackendDataWorker.Get<V_HIS_ROOM>().OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.ROOM_NAME).Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(data.ROOM_IDS)).ToList();
                        cboRoom.Text = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(data.ROOM_IDS)).ROOM_NAME;
                    }
                    else
                    {
                        string cboRoomText = "";
                        for (int i = 0; i < roomNew.Count(); i++)
                        {
                            //int m = int.Parse(roomNew[i]);
                            long m = Inventec.Common.TypeConvert.Parse.ToInt32(roomNew[i]);
                            List<V_HIS_ROOM> RoomLoad = new List<V_HIS_ROOM>();
                            RoomLoad = BackendDataWorker.Get<V_HIS_ROOM>().OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.ROOM_NAME).Where(o => o.ID == m).ToList();
                            if (cboRoomText.Length > 0)
                                cboRoomText = cboRoomText + "," + BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt32(data.ROOM_IDS)).ROOM_NAME;
                            foreach (V_HIS_ROOM a in RoomLoad)
                            {
                                listRoomSelecteds.Add(a);
                            }
                        }

                        cboRoom.Text = cboRoomText;
                    }
                }
                else
                {
                    listRoomSelecteds = new List<V_HIS_ROOM>();
                    cboRoom.Text = "";
                    SetValueRoom(this.cboRoom, this.listRoomSelecteds, BackendDataWorker.Get<V_HIS_ROOM>().OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.ROOM_NAME).Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList());
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }


        private void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_MACHINE data)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(data);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_MACHINE currentDTO)
        {
            try
            {

                currentDTO.MACHINE_CODE = txtCode.Text.Trim();
                currentDTO.MACHINE_NAME = txtName.Text.Trim();
                currentDTO.SERIAL_NUMBER = txtSeri.Text.Trim();
                currentDTO.MACHINE_GROUP_CODE = txtMachineGroupCode.Text.Trim();
                currentDTO.INTEGRATE_ADDRESS = txtIntegrateAddress.Text.Trim();
                currentDTO.SYMBOL = txtSymbol.Text.Trim();
                currentDTO.MANUFACTURER_NAME = txtManufacturerName.Text.Trim();
                currentDTO.NATIONAL_NAME = txtNationalName.Text.Trim();
                currentDTO.CIRCULATION_NUMBER = txtCirculationNumber.Text.Trim();
                currentDTO.SOURCE_NAME = txtSourceName.Text;
                currentDTO.CONFIG_TIME_CONFLICT = cboConfigTimeConflict.EditValue != null
                ? (short?)Convert.ToInt16(cboConfigTimeConflict.EditValue)
                : null;
                if (cboDepartment.EditValue != null)
                    currentDTO.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                else
                    currentDTO.DEPARTMENT_ID = null;
                if (String.IsNullOrEmpty(txtManufacturedYear.Text.Trim()))
                {
                    currentDTO.MANUFACTURED_YEAR = null;
                }
                else
                {
                    currentDTO.MANUFACTURED_YEAR = short.Parse(txtManufacturedYear.Text.Trim());
                }
                if (String.IsNullOrEmpty(txtUsedYear.Text.Trim()))
                {
                    currentDTO.USED_YEAR = null;
                }
                else
                {
                    currentDTO.USED_YEAR = short.Parse(txtUsedYear.Text.Trim());
                }

                if (String.IsNullOrEmpty(txtServiceOnDay.Text.Trim()))
                {
                    currentDTO.MAX_SERVICE_PER_DAY = null;
                }
                else
                {
                    currentDTO.MAX_SERVICE_PER_DAY = long.Parse(txtServiceOnDay.Text.Trim());
                }
                if (chkIsKidney.CheckState == CheckState.Checked)
                {
                    currentDTO.IS_KIDNEY = 1;
                }
                else
                {
                    currentDTO.IS_KIDNEY = null;
                }
                if (cboSource.SelectedIndex == 0 && !String.IsNullOrEmpty(cboSource.Text))
                {
                    currentDTO.SOURCE_CODE = "1";
                }
                else if (cboSource.SelectedIndex == 1 && !String.IsNullOrEmpty(cboSource.Text))
                {
                    currentDTO.SOURCE_CODE = "2";
                }
                else if (cboSource.SelectedIndex == 2 && !String.IsNullOrEmpty(cboSource.Text))
                {
                    currentDTO.SOURCE_CODE = "3";
                }
                else
                {
                    currentDTO.SOURCE_CODE = null;
                }
                List<long> Rooms = listRoomSelecteds.Select(o => o.ID).ToList();
                currentDTO.ROOM_IDS = string.Join(",", Rooms);

                if (dteContractFrom != null && dteContractFrom.DateTime != DateTime.MinValue)
                    currentDTO.CONTRACT_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteContractFrom.DateTime);
                if (dteContractTo != null && dteContractTo.DateTime != DateTime.MinValue)
                    currentDTO.CONTRACT_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteContractTo.DateTime);
                if (dteFromTime != null && dteFromTime.DateTime != DateTime.MinValue)
                    currentDTO.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteFromTime.DateTime);
                if (dteToTime != null && dteToTime.DateTime != DateTime.MinValue)
                    currentDTO.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteToTime.DateTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref HIS_MACHINE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMachineFilter filter = new HisMachineFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<HIS_MACHINE>>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDatagctFormList();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MachineADO pData = (MachineADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "SOURCE")
                    {
                        if (pData.SOURCE_CODE == "1")
                        {
                            e.Value = "Ngân sách";
                        }
                        else if (pData.SOURCE_CODE == "2")
                        {
                            e.Value = "Xã hội hóa";
                        }
                        else if (pData.SOURCE_CODE == "3")
                        {
                            e.Value = "Khác";
                        }
                    }
                    else if (e.Column.FieldName == "ROOM_CODES")
                    {

                        if (!String.IsNullOrWhiteSpace(pData.ROOM_IDS))
                        {
                            List<V_HIS_ROOM> listRoom = BackendDataWorker.Get<V_HIS_ROOM>();
                            string[] listRoomIds = pData.ROOM_IDS.Split(',');
                            if (listRoomIds != null)
                            {
                                List<string> roomCodes = new List<string>();
                                for (int i = 0; i < listRoomIds.Count(); i++)
                                {
                                    long m = Inventec.Common.TypeConvert.Parse.ToInt32(listRoomIds[i]);
                                    V_HIS_ROOM ado = listRoom.FirstOrDefault(o => o.ID == m);
                                    if (ado != null)
                                        roomCodes.Add(ado.ROOM_CODE);
                                }
                                if (roomCodes != null && roomCodes.Count > 0)
                                    e.Value = String.Join(", ", roomCodes);
                            }
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {

                            LogSystem.Error(ex);
                        }
                    }
                }
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
                MachineADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (MachineADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDeleteEnable : btnDeleteDisable);
                    }
                    if (e.Column.FieldName == "IsKidney")
                    {
                        e.RepositoryItem = data.IS_KIDNEY == 1 ? ButtonEditIsKidney : null;
                    }
                    if (e.Column.FieldName == "IsQcNormation")
                    {
                        e.RepositoryItem = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? ButtonEditQcNormation : ButtonEditDisableQcNormation;
                    }
                    if (e.Column.FieldName == "IsMachine")
                    {
                        var listMachineInspection = BackendDataWorker.Get<HIS_MACHINE_INSPECTION>().Where(o => o.MACHINE_ID == data.ID).ToList();
                        e.RepositoryItem = (listMachineInspection?.Count > 0) ? ButtonEditIsMachine : ButtonEditNonMachine;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MachineADO)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ResetFormData();
                    ChangedDataRow(rowData);
                    if (cboDepartment.EditValue != null)
                    {
                        long departmentId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                        SetValueRoom(this.cboRoom, this.listRoomSelecteds, BackendDataWorker.Get<V_HIS_ROOM>().OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.ROOM_NAME).Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.DEPARTMENT_ID == departmentId && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList());

                    }
                    else
                        SetValueRoom(this.cboRoom, this.listRoomSelecteds, BackendDataWorker.Get<V_HIS_ROOM>().OrderByDescending(o => o.MODIFY_TIME).ThenBy(o => o.ROOM_NAME).Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList());
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MachineADO data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRoom(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {

            try
            {


                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = " Tất cả ";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);

                    ////
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        //private void InitComboRoomCode(CustomGridLookUpEditWithFilterMultiColumn cbo)
        //{
        //    try
        //    {
        //        listRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();


        //        cbo.Properties.DataSource = listRoom;
        //        cbo.Properties.DisplayMember = "ROOM_NAME";
        //        cbo.Properties.ValueMember = "ID";
        //        cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
        //        cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
        //        cbo.Properties.ImmediatePopup = true;
        //        cbo.ForceInitialize();
        //        cbo.Properties.View.Columns.Clear();
        //        cbo.Properties.PopupFormSize = new Size(400, 250);

        //        var aColumnCode = cbo.Properties.View.Columns.AddField("ROOM_CODE");
        //        aColumnCode.Caption = "Mã phòng";
        //        aColumnCode.Visible = true;
        //        aColumnCode.VisibleIndex = 1;
        //        aColumnCode.Width = 100;

        //        var aColumnName = cbo.Properties.View.Columns.AddField("ROOM_NAME");
        //        aColumnName.Caption = "Tên phòng";
        //        aColumnName.Visible = true;
        //        aColumnName.VisibleIndex = 2;
        //        aColumnName.Width = 300;
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        private void cboRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (V_HIS_ROOM rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0)
                    {
                        sb.Append(" , ");
                    }
                    sb.Append(rv.ROOM_NAME.ToString());

                }
                e.DisplayText = sb.ToString();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }
        //private void InitRoomCheck()
        //{
        //    try
        //    {
        //        GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboRoom.Properties);
        //        gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__ServiceReqType);
        //        cboRoom.Properties.Tag = gridCheck;
        //        cboRoom.Properties.View.OptionsSelection.MultiSelect = true;
        //        GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
        //        if (gridCheckMark != null)
        //        {
        //            gridCheckMark.ClearSelection(cboRoom.Properties.View);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        //private void SelectionGrid__ServiceReqType(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        listRoom = new List<V_HIS_ROOM>();
        //        foreach (MOS.EFMODEL.DataModels.V_HIS_ROOM rv in (sender as GridCheckMarksSelection).Selection)
        //        {
        //            if (rv != null)
        //                listRoom.Add(rv);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_MACHINE data)
        {
            try
            {
                if (data != null)
                {
                    txtIntegrateAddress.Text = data.INTEGRATE_ADDRESS;
                    txtCode.Text = data.MACHINE_CODE;
                    txtName.Text = data.MACHINE_NAME;
                    txtSeri.Text = data.SERIAL_NUMBER;
                    txtMachineGroupCode.Text = data.MACHINE_GROUP_CODE;
                    txtServiceOnDay.Text = data.MAX_SERVICE_PER_DAY != null ? data.MAX_SERVICE_PER_DAY.ToString() : "";
                    if (String.IsNullOrEmpty(data.SOURCE_CODE))
                    {
                        cboSource.EditValue = null;
                    }
                    else if (data.SOURCE_CODE == "1")
                    {
                        cboSource.SelectedIndex = 0;
                    }
                    else if (data.SOURCE_CODE == "2")
                    {
                        cboSource.SelectedIndex = 1;
                    }
                    else if (data.SOURCE_CODE == "3")
                    {
                        cboSource.SelectedIndex = 2;
                    }
                    filldatatocboRoom(data);
                    if (data.IS_KIDNEY == 1)
                    {
                        chkIsKidney.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        chkIsKidney.CheckState = CheckState.Unchecked;
                    }
                    txtSymbol.Text = data.SYMBOL;
                    txtManufacturerName.Text = data.MANUFACTURER_NAME;
                    txtNationalName.Text = data.NATIONAL_NAME;
                    txtManufacturedYear.Text = data.MANUFACTURED_YEAR != null ? data.MANUFACTURED_YEAR.ToString() : "";
                    txtUsedYear.Text = data.USED_YEAR != null ? data.USED_YEAR.ToString() : null;
                    txtCirculationNumber.Text = data.CIRCULATION_NUMBER;
                    txtSourceName.Text = data.SOURCE_NAME;
                    cboDepartment.EditValue = data.DEPARTMENT_ID;
                    cboConfigTimeConflict.EditValue = (short?)(data.CONFIG_TIME_CONFLICT);
                    if (data.TO_TIME.HasValue)
                        dteToTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TO_TIME ?? 0) ?? DateTime.Now;
                    if (data.FROM_TIME.HasValue)
                        dteFromTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FROM_TIME ?? 0) ?? DateTime.Now;
                    if (data.CONTRACT_TO.HasValue)
                        dteContractTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.CONTRACT_TO ?? 0) ?? DateTime.Now;
                    if (data.CONTRACT_FROM.HasValue)
                        dteContractFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.CONTRACT_FROM ?? 0) ?? DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            MOS.EFMODEL.DataModels.HIS_MACHINE success = new MOS.EFMODEL.DataModels.HIS_MACHINE();
            //bool notHandler = false;
            try
            {

                MachineADO data = (MachineADO)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.HIS_MACHINE data1 = new MOS.EFMODEL.DataModels.HIS_MACHINE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MACHINE>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_MACHINE>();
                        rs = true;
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    btnReset_Click(null, null);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool rs = false;
            MOS.EFMODEL.DataModels.HIS_MACHINE success = new MOS.EFMODEL.DataModels.HIS_MACHINE();
            //bool notHandler = false;

            try
            {

                MachineADO data = (MachineADO)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_MACHINE>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_GROUP_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_MACHINE>();
                        rs = true;
                        FillDatagctFormList();
                    }
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, rs);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                    btnReset_Click(null, null);
                }

            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteEnable_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MachineADO)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {

                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            txtName.Text = "";
                            txtCode.Text = "";
                            EnableControlChanged(this.ActionType);
                            FillDatagctFormList();
                            currentData = ((List<MachineADO>)gridControl1.DataSource).FirstOrDefault();
                            BackendDataWorker.Reset<HIS_MACHINE>();
                        }
                        MessageManager.Show(this, param, success);
                        btnReset_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeleteDisable_Click(object sender, EventArgs e)
        {

        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtName.Focus();
            }
        }

        private void txtName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSeri.Focus();
            }
        }

        private void txtFind_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnFind_Click(null, null);
                gridView1.Focus();
            }
        }
        #endregion

        #region ShortCut
        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled)
            {
                btnEdit_Click(null, null);
            }

        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnReset.Enabled)
            {
                btnReset_Click(null, null);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        #endregion

        private void txtSeri_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboSource.Focus();
                cboSource.ShowPopup();
            }
        }

        private void txtMachineGroupCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtSymbol.Focus();
                txtSymbol.SelectAll();
            }
        }

        private void cboSource_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtMachineGroupCode.Focus();
            }
        }

        private void txtIntegrateAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRoom.Focus();
                    cboRoom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSource_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSource.EditValue != null)
                {
                    cboSource.Properties.Buttons[1].Visible = true;

                }
                else
                {
                    cboSource.Properties.Buttons[1].Visible = false;
                }
                SetValidateSourceName(Convert.ToInt64(cboSource.SelectedIndex));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private bool IsRequiredSourceName = false;
        private void SetValidateSourceName(long value)
        {
            try
            {
                this.layoutControlItemSourceName.AppearanceItemCaption.ForeColor = value == 2 ? Color.Maroon : Color.Black;
                IsRequiredSourceName = value == 2;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSource_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSource.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                MachineADO data = (MachineADO)gridView1.GetRow(e.RowHandle);
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void gridView1_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column != null && e.Column.FieldName == "IsChecked")
                {
                    e.Info.InnerElements.Clear();
                    e.Painter.DrawObject(e.Info);
                    DrawCheckBox(e.Graphics, e.Bounds, isCheckAll);
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DrawCheckBox(Graphics graphics, Rectangle bounds, bool isChecked)
        {
            int checkBoxSize = 14;
            int x = bounds.X + (bounds.Width - checkBoxSize) / 2;
            int y = bounds.Y + (bounds.Height - checkBoxSize) / 2;
            Rectangle checkBoxRect = new Rectangle(x, y, checkBoxSize, checkBoxSize);
            ControlPaint.DrawCheckBox(graphics, checkBoxRect, isChecked ? ButtonState.Checked | ButtonState.Flat : ButtonState.Normal | ButtonState.Flat);
        }

        private void gridView1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridView1.CalcHitInfo(e.Location);
                if (hitInfo.InColumnPanel && hitInfo.Column != null && hitInfo.Column.FieldName == "IsChecked")
                {
                    isCheckAll = !isCheckAll;
                    var dataSource = gridControl1.DataSource as List<MachineADO>;
                    if (dataSource != null)
                    {
                        foreach (var item in dataSource)
                        {
                            item.IsChecked = isCheckAll;
                        }
                    }
                    gridView1.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceOnDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    cboDepartment.Focus();
                    cboDepartment.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditQcNormation_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MachineADO)gridView1.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisQcNormation").FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
                //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //{
                List<object> listArgs = new List<object>();
                HIS_MACHINE data = new HIS_MACHINE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MACHINE>(data, rowData);
                listArgs.Add(data);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule), currentModule));
                //var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                //if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                //((Form)extenceInstance).ShowDialog();
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisQcNormation", 0, 0, listArgs);

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDepartment.Properties.Buttons[1].Visible = cboDepartment.EditValue != null;
                if (cboDepartment.EditValue != null)
                {
                    long departmentId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                    cboRoom.Properties.DataSource = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.DEPARTMENT_ID == departmentId && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
                }
                else
                    cboRoom.Properties.DataSource = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.IS_ACTIVE == 1 && o.BRANCH_ID == BranchDataWorker.GetCurrentBranchId() && o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtManufacturedYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    txtUsedYear.Focus();
                    txtUsedYear.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtUsedYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    txtCirculationNumber.Focus();
                    txtCirculationNumber.SelectAll();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSymbol_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtManufacturerName.Focus();
                txtManufacturerName.SelectAll();
            }
        }

        private void txtManufacturerName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtNationalName.Focus();
                txtNationalName.SelectAll();
            }
        }

        private void txtNationalName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtManufacturedYear.Focus();
                txtManufacturedYear.SelectAll();
            }
        }

        private void txtManufacturedYear_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtUsedYear.Focus();
                txtUsedYear.SelectAll();
            }
        }

        private void txtUsedYear_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtCirculationNumber.Focus();
                txtCirculationNumber.SelectAll();
            }
        }

        private void txtCirculationNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtIntegrateAddress.Focus();
                txtIntegrateAddress.SelectAll();
            }
        }

        private void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                string savePath = "";
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    savePath = fbd.SelectedPath;
                }
                if (String.IsNullOrEmpty(savePath))
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisMachineFilter filter = new HisMachineFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var listMachines = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MACHINE>>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_GET, ApiConsumers.MosConsumer, filter, param);
                var selectedMachines = ((List<MachineADO>)gridControl1.DataSource);
                listMachines = listMachines.Where(o => selectedMachines.Any(s => s.ID == o.ID && s.IsChecked)).ToList();
                if (listMachines == null || listMachines.Count == 0)
                {
                    WaitingManager.Hide();
                    return;
                }

                string fullFileName = String.Format("MayCLS_{0}.xml", DateTime.Now.ToString("ddMMyyyy_HHmmss"));
                string saveFilePath = String.Format("{0}/{1}", savePath, fullFileName);
                List<CLSAdo> listXmlAdos = new List<CLSAdo>();
                List<XMLCLSDetailData> listXmlDetails = new List<XMLCLSDetailData>();
                listXmlAdos = GenerateXmlAdo(listMachines);
                MapADOToXml(listXmlAdos, ref listXmlDetails);
                XMLCLSData xmlData = new XMLCLSData();
                xmlData.MayCls = listXmlDetails;
                var rs = CreatedXmlFilePlus(xmlData);
                if (rs != null)
                {
                    FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                    rs.WriteTo(file);
                    file.Close();
                    rs.Close();
                    success = true;
                }
                if (chkSign.Checked)
                {
                    SignFile(fullFileName, saveFilePath);
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private List<CLSAdo> GenerateXmlAdo(List<HIS_MACHINE> listMachines)
        {
            List<CLSAdo> result = new List<CLSAdo>();
            try
            {
                int count = 1;
                foreach (var machine in listMachines)
                {
                    CLSAdo xmlCLS = new CLSAdo();
                    xmlCLS.Stt = count;
                    string maCSKCB = "";
                    if (!String.IsNullOrEmpty(machine.ROOM_IDS))
                    {
                        var roomId = machine.ROOM_IDS.Split(',')[0];
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(roomId));
                        if (room != null)
                        {
                            var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == room.BRANCH_ID);
                            if (branch != null)
                                maCSKCB = branch.HEIN_MEDI_ORG_CODE;
                        }
                    }
                    xmlCLS.MaCoSoKCB = maCSKCB;
                    xmlCLS.TenThietBi = machine.MACHINE_NAME;
                    xmlCLS.KyHieu = !String.IsNullOrEmpty(machine.SYMBOL) ? machine.SYMBOL : "";
                    xmlCLS.CongTySX = !String.IsNullOrEmpty(machine.MANUFACTURER_NAME) ? machine.MANUFACTURER_NAME : "";
                    xmlCLS.NuocSX = !String.IsNullOrEmpty(machine.NATIONAL_NAME) ? machine.NATIONAL_NAME : "";
                    xmlCLS.NamSX = machine.MANUFACTURED_YEAR;
                    xmlCLS.NamSD = machine.USED_YEAR;
                    xmlCLS.SoLuuHanh = !String.IsNullOrEmpty(machine.CIRCULATION_NUMBER) ? machine.CIRCULATION_NUMBER : "";
                    xmlCLS.MaMay = String.Format("{0}.{1}.{2}.{3}", machine.MACHINE_GROUP_CODE, machine.SOURCE_CODE, maCSKCB, machine.SERIAL_NUMBER);

                    var listMachineInspection = BackendDataWorker.Get<HIS_MACHINE_INSPECTION>().Where(o => o.MACHINE_ID == machine.ID).ToList();

                    if (listMachineInspection != null && listMachineInspection.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("listMachineInspection1: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listMachineInspection), listMachineInspection));

                        long? maxFromTime = listMachineInspection.Max(o => o.FROM_TIME);
                        long? maxToTime = listMachineInspection.Max(o => o.TO_TIME);

                        if (maxFromTime.HasValue)
                        {
                            xmlCLS.TuNgay = (int)(maxFromTime.Value / 1000000);
                        }

                        if (maxToTime.HasValue)
                        {
                            xmlCLS.DenNgay = (int)(maxToTime.Value / 1000000);
                        }
                    }
                    result.Add(xmlCLS);
                    count++;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<XmlTT12Ado> GenerateXmlTT12Ado(List<HIS_MACHINE> listMachines)
        {
            List<XmlTT12Ado> result = new List<XmlTT12Ado>();
            try
            {
                int count = 1;
                foreach (var machine in listMachines)
                {
                    XmlTT12Ado xmlCLS = new XmlTT12Ado();
                    xmlCLS.Stt = count;
                    string maCSKCB = "";
                    if (!String.IsNullOrEmpty(machine.ROOM_IDS))
                    {
                        var roomId = machine.ROOM_IDS.Split(',')[0];
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(roomId));
                        if (room != null)
                        {
                            var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == room.BRANCH_ID);
                            if (branch != null)
                                maCSKCB = branch.HEIN_MEDI_ORG_CODE;
                        }
                    }
                    xmlCLS.MaCoSoKCB = maCSKCB;
                    xmlCLS.TenThietBi = machine.MACHINE_NAME;
                    xmlCLS.KyHieu = !String.IsNullOrEmpty(machine.SYMBOL) ? machine.SYMBOL : "";
                    xmlCLS.CongTySX = !String.IsNullOrEmpty(machine.MANUFACTURER_NAME) ? machine.MANUFACTURER_NAME : "";
                    xmlCLS.NuocSX = !String.IsNullOrEmpty(machine.NATIONAL_NAME) ? machine.NATIONAL_NAME : "";
                    xmlCLS.NamSX = machine.MANUFACTURED_YEAR;
                    xmlCLS.NamSD = machine.USED_YEAR;
                    xmlCLS.SoLuuHanh = !String.IsNullOrEmpty(machine.CIRCULATION_NUMBER) ? machine.CIRCULATION_NUMBER : "";
                    xmlCLS.MaMay = String.Format("{0}.{1}.{2}.{3}", machine.MACHINE_GROUP_CODE, machine.SOURCE_CODE, maCSKCB, machine.SERIAL_NUMBER);

                    var listMachineInspection = BackendDataWorker.Get<HIS_MACHINE_INSPECTION>().Where(o => o.MACHINE_ID == machine.ID).ToList();

                    if (listMachineInspection != null && listMachineInspection.Count > 0)
                    {
                        long? maxFromTime = listMachineInspection.Max(o => o.FROM_TIME);
                        long? maxToTime = listMachineInspection.Max(o => o.TO_TIME);

                        if (maxFromTime.HasValue)
                        {
                            xmlCLS.TuNgay = (int)(maxFromTime.Value / 1000000);
                        }

                        if (maxToTime.HasValue)
                        {
                            xmlCLS.DenNgay = (int)(maxToTime.Value / 1000000);
                        }
                    }
                    else
                    {
                        xmlCLS.TuNgay = (int)((machine.FROM_TIME ?? 0) / 1000000);
                        xmlCLS.DenNgay = (int)((machine.TO_TIME ?? 0) / 1000000);
                    }
                    xmlCLS.TuNgay = (int)((machine.CONTRACT_FROM ?? 0) / 1000000);
                    xmlCLS.DenNgay = (int)((machine.CONTRACT_TO ?? 0) / 1000000);
                    result.Add(xmlCLS);
                    count++;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        public void MapADOToXml(List<CLSAdo> listAdo, ref List<XMLCLSDetailData> datas)
        {
            try
            {
                if (datas == null)
                    datas = new List<XMLCLSDetailData>();
                if (listAdo != null || listAdo.Count > 0)
                {
                    foreach (var ado in listAdo)
                    {
                        XMLCLSDetailData detail = new XMLCLSDetailData();
                        detail.STT = ado.Stt;
                        detail.MA_CSKCB = ado.MaCoSoKCB;
                        detail.TEN_TB = this.ConvertStringToXmlDocument(ado.TenThietBi);
                        detail.KY_HIEU = ado.KyHieu;
                        detail.CONGTY_SX = this.ConvertStringToXmlDocument(ado.CongTySX);
                        detail.NUOC_SX = this.ConvertStringToXmlDocument(ado.NuocSX);
                        detail.NAM_SX = ado.NamSX != null ? ado.NamSX.ToString() : "";
                        detail.NAM_SD = ado.NamSD != null ? ado.NamSD.ToString() : "";
                        detail.MA_MAY = ado.MaMay;
                        detail.SO_LUU_HANH = ado.SoLuuHanh;
                        detail.TU_NGAY = ado.TuNgay != null ? ado.TuNgay.ToString() : "";
                        detail.DEN_NGAY = ado.DenNgay != null ? ado.DenNgay.ToString() : "";
                        datas.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void MapADOToXmlTT12(List<XmlTT12Ado> listAdo, ref List<XMLTT12DetailData> datas)
        {
            try
            {
                if (datas == null)
                    datas = new List<XMLTT12DetailData>();
                if (listAdo != null || listAdo.Count > 0)
                {
                    foreach (var ado in listAdo)
                    {
                        XMLTT12DetailData detail = new XMLTT12DetailData();
                        detail.STT = ado.Stt;
                        detail.MA_CSKCB = ado.MaCoSoKCB;
                        detail.TEN_TB = this.ConvertStringToXmlDocument(ado.TenThietBi);
                        detail.KY_HIEU = ado.KyHieu;
                        detail.CONGTY_SX = this.ConvertStringToXmlDocument(ado.CongTySX);
                        detail.NUOC_SX = this.ConvertStringToXmlDocument(ado.NuocSX);
                        detail.NAM_SX = ado.NamSX != null ? ado.NamSX.ToString() : "";
                        detail.NAM_SD = ado.NamSD != null ? ado.NamSD.ToString() : "";
                        detail.MA_MAY = ado.MaMay;
                        detail.SO_LUU_HANH = ado.SoLuuHanh;
                        detail.TU_NGAY = ado.TuNgay != null ? ado.TuNgay.ToString() : "";
                        detail.DEN_NGAY = ado.DenNgay != null ? ado.DenNgay.ToString() : "";
                        detail.HD_TU = ado.HdTu != null ? ado.HdTu.ToString() : "";
                        detail.HD_DEN = ado.HdDen != null ? ado.HdDen.ToString() : "";
                        datas.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal XmlCDataSection ConvertStringToXmlDocument(string data)
        {
            XmlCDataSection result;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<book genre='novel' ISBN='1-861001-57-5'>" + "<title>Pride And Prejudice</title>" + "</book>");
            result = doc.CreateCDataSection(RemoveXmlCharError(data));
            return result;
        }
        internal string RemoveXmlCharError(string data)
        {
            string result = "";
            try
            {
                StringBuilder s = new StringBuilder();
                if (!String.IsNullOrWhiteSpace(data))
                {
                    foreach (char c in data)
                    {
                        if (!System.Xml.XmlConvert.IsXmlChar(c)) continue;
                        s.Append(c);
                    }
                }

                result = s.ToString();
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private static MemoryStream CreatedXmlFilePlus<XMLCLSData>(XMLCLSData input)
        {
            MemoryStream stream = null;
            try
            {
                var enc = Encoding.UTF8;
                stream = new MemoryStream();
                var xmlNamespaces = new XmlSerializerNamespaces();
                xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");


                var xmlWriterSettings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = enc,
                    OmitXmlDeclaration = false,
                    Indent = true
                };
                using (var xw = XmlWriter.Create(stream, xmlWriterSettings))
                {
                    var s = new XmlSerializer(typeof(XMLCLSData));
                    s.Serialize(xw, input, xmlNamespaces);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                stream = null;
            }
            return stream;
        }
        private static MemoryStream CreatedXmlFileTT12Plus<XMLTT12Data>(XMLTT12Data input)
        {
            MemoryStream stream = null;
            try
            {
                var enc = Encoding.UTF8;
                stream = new MemoryStream();
                var xmlNamespaces = new XmlSerializerNamespaces();
                xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");


                var xmlWriterSettings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = enc,
                    OmitXmlDeclaration = false,
                    Indent = true
                };
                using (var xw = XmlWriter.Create(stream, xmlWriterSettings))
                {
                    var s = new XmlSerializer(typeof(XMLTT12Data));
                    s.Serialize(xw, input, xmlNamespaces);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                stream = null;
            }
            return stream;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnExportXml_Click(null, null);
        }

        private void cboConfigTimeConflict_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboConfigTimeConflict.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboConfigTimeConflict_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboConfigTimeConflict.EditValue != null)
                {
                    cboConfigTimeConflict.Properties.Buttons[1].Visible = true;

                }
                else
                {
                    cboConfigTimeConflict.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditIsMachine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {

                var rowData = (MachineADO)gridView1.GetFocusedRow();
                if (rowData == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMachineInspection").FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
                List<object> listArgs = new List<object>();

                HIS_MACHINE data = new HIS_MACHINE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MACHINE>(data, rowData);
                listArgs.Add(data);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule), currentModule));
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisMachineInspection", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditNonMachine_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MachineADO)gridView1.GetFocusedRow();
                if (rowData == null) return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisMachineInspection").FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
                List<object> listArgs = new List<object>();

                HIS_MACHINE data = new HIS_MACHINE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MACHINE>(data, rowData);
                listArgs.Add(data);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentModule), currentModule));
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisMachineInspection", 0, 0, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkSign_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                    return;

                isChkSignFileCertUtil();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        SettingSignADO SettingSignADO;
        private bool isNotLoadWhileChangeControlStateInFirst;
        private bool isCheckAll = false;

        private void isChkSignFileCertUtil()
        {
            try
            {
                if (chkSign.Checked == true)
                {
                    frmSetting frm = new frmSetting(SettingSignADO, (result) =>
                    {
                        SettingSignADO = (SettingSignADO)result;
                    });
                    frm.ShowDialog();
                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                        chkSign.Checked = false;
                }
                else
                {
                    SettingSignADO = null;
                }
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkSign.Name && o.MODULE_LINK == this.currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkSign.Name;
                    csAddOrUpdate.VALUE = Newtonsoft.Json.JsonConvert.SerializeObject(SettingSignADO);
                    csAddOrUpdate.MODULE_LINK = this.currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXmlTT12_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                string savePath = "";
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    savePath = fbd.SelectedPath;
                }
                if (String.IsNullOrEmpty(savePath))
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                HisMachineFilter filter = new HisMachineFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var listMachines = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_MACHINE>>(HIS.Desktop.Plugins.HisMachine.HisRequestUriStore.MOSHIS_HIS_MACHINE_GET, ApiConsumers.MosConsumer, filter, param);
                var selectedMachines = ((List<MachineADO>)gridControl1.DataSource);
                listMachines = listMachines.Where(o => selectedMachines.Any(s => s.ID == o.ID && s.IsChecked)).ToList();
                if (listMachines == null || listMachines.Count == 0)
                {
                    WaitingManager.Hide();
                    return;
                }

                string fullFileName = String.Format("MayCLSTT12_{0}.xml", DateTime.Now.ToString("ddMMyyyy_HHmmss"));
                string saveFilePath = String.Format("{0}/{1}", savePath, fullFileName);
                List<XmlTT12Ado> listXmlAdos = new List<XmlTT12Ado>();
                List<XMLTT12DetailData> listXmlDetails = new List<XMLTT12DetailData>();
                listXmlAdos = GenerateXmlTT12Ado(listMachines);
                MapADOToXmlTT12(listXmlAdos, ref listXmlDetails);
                XMLTT12Data xmlData = new XMLTT12Data();
                xmlData.DanhMuc = listXmlDetails;
                xmlData.ChuKyDonVi = "";
                var rs = CreatedXmlFileTT12Plus(xmlData);
                if (rs != null)
                {
                    FileStream file = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
                    rs.WriteTo(file);
                    file.Close();
                    rs.Close();
                    success = true;
                }
                if (chkSign.Checked)
                {
                    SignFile(fullFileName, saveFilePath);
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool SignFile(string fullFileName,string saveFilePath)
        {
            try
            {
                if (SettingSignADO == null || (SettingSignADO != null && string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                {
                    MessageBox.Show("Không có thông tin Usb Token ký số");
                    return false;
                }
                else
                {
                    string currentDirectory = Directory.GetCurrentDirectory();
                    string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                    Directory.CreateDirectory(tempFolderPath);
                    string tempFilePath = Path.Combine(tempFolderPath, fullFileName);
                    File.Create(tempFilePath).Close();
                    string pathAfterFileSign = null;
                    WcfSignDCO wcfSignDCO = null;
                    if (SettingSignADO.IsHsm)
                    {
                        var xmlBase64 = SourceFileSignApi(ReadFileContent(saveFilePath));
                        if (string.IsNullOrEmpty(xmlBase64))
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Ký HSM thất bại");
                            return false;
                        }
                        var xmlBytes = Convert.FromBase64String(xmlBase64);
                        File.WriteAllBytes(tempFilePath, xmlBytes);
                        pathAfterFileSign = tempFilePath;
                    }
                    else
                    {
                        wcfSignDCO = new WcfSignDCO
                        {
                            SerialNumber = SettingSignADO.SerialNumber,
                            OutputFile = tempFilePath,
                            PIN = "",
                            SourceFile = saveFilePath,
                            fieldSigned = "CHUKYDONVI"
                        };
                        string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                        SignProcessorClient signProcessorClient = new SignProcessorClient();
                        if (!VerifyServiceSignProcessorIsRunning())
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Service ký số không chạy");
                        }
                        var wcfSignResultDCO = signProcessorClient.SignXml130(jsonData);
                        if (wcfSignResultDCO == null || !wcfSignResultDCO.Success)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Ký file thất bại: " + (wcfSignResultDCO != null ? wcfSignResultDCO.Message : ""));
                            return false;
                        }
                        pathAfterFileSign = wcfSignResultDCO.OutputFile;
                    }
                    if (!string.IsNullOrEmpty(pathAfterFileSign) && File.Exists(pathAfterFileSign))
                    {
                        File.Copy(pathAfterFileSign, saveFilePath, true);
                    }
                    if (File.Exists(tempFilePath))
                    {
                        File.Delete(tempFilePath);
                    }
                    if (Directory.Exists(tempFolderPath) && Directory.GetFiles(tempFolderPath).Length == 0 && Directory.GetDirectories(tempFolderPath).Length == 0)
                    {
                        Directory.Delete(tempFolderPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }

        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {

                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        private string SourceFileSignApi(string xmlBase64Source)
        {
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO signXmlBhytSDO = new EMR.SDO.SignXmlBhytSDO();
                signXmlBhytSDO.XmlBase64 = xmlBase64Source;
                signXmlBhytSDO.TagStoreSignatureValue = "CHUKYDONVI";
                signXmlBhytSDO.ConfigData = new EMR.SDO.XmlConfigDataSDO() { HsmSerialNumber = SettingSignADO.SerialNumber, HsmType = SettingSignADO.Id, HsmUserCode = SettingSignADO.Name, Password = SettingSignADO.Password, SecretKey = SettingSignADO.SercetKey, IdentityNumber = SettingSignADO.CccdNumber };
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumer.ApiConsumers.EmrConsumer, signXmlBhytSDO, SessionManager.ActionLostToken, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, param.Messages);
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Inventec.Common.Logging.LogSystem.Warn(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private string ReadFileContent(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(filePath);
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.LoadXml(RemoveByteOrderMark(Encoding.UTF8.GetString(File.ReadAllBytes(filePath))));
                        return Convert.ToBase64String(StringToBytes(RemoveByteOrderMark(Encoding.UTF8.GetString(fileBytes))));
                    }
                    catch (Exception)
                    {
                        xmlDocument.LoadXml(Encoding.UTF8.GetString(File.ReadAllBytes(filePath)));
                        return Convert.ToBase64String(StringToBytes(Encoding.UTF8.GetString(fileBytes)));
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        private string RemoveByteOrderMark(string XML)
        {
            string byteOrderMark = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());
            if (XML.StartsWith(byteOrderMark))
            {
                XML = XML.Remove(0, byteOrderMark.Length);
            }
            return XML;
        }
        public byte[] StringToBytes(string input)
        {
            if (input == null) return null;
            return Encoding.UTF8.GetBytes(input);
        }
    }
}
