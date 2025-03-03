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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using MOS.Filter;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.HisMachine.Properties;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.CustomControl;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Plugins.HisMachine.Validation;
using System.IO;
using HIS.Desktop.Plugins.HisMachine.XML;
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
        MOS.EFMODEL.DataModels.HIS_MACHINE currentData;
        MOS.EFMODEL.DataModels.HIS_MACHINE resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
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
                ValidationMaxLength(txtCirculationNumber, 22);


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
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
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
                    MOS.EFMODEL.DataModels.HIS_MACHINE pData = (MOS.EFMODEL.DataModels.HIS_MACHINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                MOS.EFMODEL.DataModels.HIS_MACHINE data = null;
                if (e.RowHandle > -1)
                {
                    data = (MOS.EFMODEL.DataModels.HIS_MACHINE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_MACHINE)gridView1.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_MACHINE data)
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

                MOS.EFMODEL.DataModels.HIS_MACHINE data = (MOS.EFMODEL.DataModels.HIS_MACHINE)gridView1.GetFocusedRow();
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

                MOS.EFMODEL.DataModels.HIS_MACHINE data = (MOS.EFMODEL.DataModels.HIS_MACHINE)gridView1.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_MACHINE)gridView1.GetFocusedRow();
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
                            currentData = ((List<MOS.EFMODEL.DataModels.HIS_MACHINE>)gridControl1.DataSource).FirstOrDefault();
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
                HIS_MACHINE data = (HIS_MACHINE)gridView1.GetRow(e.RowHandle);
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_MACHINE)gridView1.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisQcNormation").FirstOrDefault();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleData), moduleData));
                //if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                //{
                List<object> listArgs = new List<object>();
                listArgs.Add(rowData);
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnExportXml_Click(null, null);
        }
    }
}
