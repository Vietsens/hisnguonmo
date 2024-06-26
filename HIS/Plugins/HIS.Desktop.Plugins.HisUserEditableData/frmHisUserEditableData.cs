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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors.Controls;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
namespace HIS.Desktop.Plugins.HisUserEditableData
{
    public partial class frmHisUserEditableData : FormBase
    {
        long IdRole = 0;
        Grantable grantTable;
        #region global list
        string allowLogginames = "";
        ACS.EFMODEL.DataModels.ACS_USER currentDataUser = null;
        ACS_USER currentData;
        List<ACS_USER> listUser;
        Inventec.Desktop.Common.Modules.Module moduleData;
        int rowCount;
        int dataTotal;
        int positionHandle = -1;
        int ActionType = -1;
        int startPage;
        int limit;
        List<ACS_USER> ListSelectUser;
        bool isAll = true;
        #endregion

        public frmHisUserEditableData(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            SetIcon();
        }

        public frmHisUserEditableData(Inventec.Desktop.Common.Modules.Module moduleData, long idRole, Grantable roleCode, string allowLogginame)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.IdRole = idRole;
            this.grantTable = roleCode;
            this.allowLogginames = allowLogginame;
            SetIcon();
        }

        #region function
        private void FillDataToGridControl()
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
            ucPaging.Init(LoadPaging, param, numPageSize, gridControlFormList);

            WaitingManager.Hide();
        }

        private void SaveProcess()
        {
            if (!btnSave.Enabled)
                return;
            positionHandle = -1;
            if (!dxValidationProvider1.Validate())
                return;
            //ValidateForm();
            CommonParam param = new CommonParam();
            bool success = false;

            if (this.ListSelectUser == null || this.ListSelectUser.Count == 0)
            {
                MessageBox.Show("Chưa chọn tài khoản", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            WaitingManager.Show();
            try
            {
                GrantPermissionSDO grantPermissionSDO = new GrantPermissionSDO();
                grantPermissionSDO.DataId = this.IdRole;
                grantPermissionSDO.Table = this.grantTable;
                grantPermissionSDO.LoginNames = this.ListSelectUser.Select(o => o.LOGINNAME).Distinct().ToList();

                success = new BackendAdapter(param).Post<bool>
                  (HisRequestUriStore.GRANT_PERMISSION_UPDATE, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, grantPermissionSDO, param);
                if (success)
                {
                    FillDataToGridControl();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void EnableControlChanged(int action)
        {
            btnSave.Enabled = (action == GlobalVariables.ActionAdd);
        }

        private void SetDefaultValue()
        {
            txtSearch.Text = "";
            FillDataToGridControl();
        }

        #endregion

        #region validate
        private void ValidateForm()
        {
            try
            {
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object param)
        {
            startPage = ((CommonParam)param).Start ?? 0;
            limit = ((CommonParam)param).Limit ?? 0;

            CommonParam paramCommon = new CommonParam(startPage, limit);
            //ACS_USER data
            ACS.Filter.AcsUserFilter filterUserSearch = new ACS.Filter.AcsUserFilter();
            filterUserSearch.KEY_WORD = txtSearch.Text.Trim();
            filterUserSearch.ORDER_FIELD = "MODIFY_TIME";
            filterUserSearch.ORDER_DIRECTION = "DESC";

            Inventec.Core.ApiResultObject<List<ACS.EFMODEL.DataModels.ACS_USER>> apiResultUser = null;
            apiResultUser = new BackendAdapter(paramCommon).GetRO<List<ACS.EFMODEL.DataModels.ACS_USER>>
                (HisRequestUriStore.ACS_USER_GET, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filterUserSearch, paramCommon);

            //HIS_EMPLOYEE data

            if (apiResultUser != null)
            {
                var dataUser = (List<ACS.EFMODEL.DataModels.ACS_USER>)apiResultUser.Data;
                this.listUser = dataUser;
                //gan du lieu len gridview
                gridViewFormList.GridControl.DataSource = dataUser;
                foreach (var user in ListSelectUser)
                {
                    int index = 0;
                    foreach (var item in dataUser)
                    {
                        if (user.LOGINNAME == item.LOGINNAME)
                        {
                            gridViewFormList.SelectRow(index);
                            break;
                        }
                        index++;
                    }

                }
                rowCount = (dataUser == null ? 0 : dataUser.Count);
                dataTotal = (apiResultUser.Param == null ? 0 : apiResultUser.Param.Count ?? 0);
            }
        }

        private void ValidationEmail(TextEdit control)
        {
            if (control.Text != null || control.Text.Equals(""))
            {
                ValidateEmail validRule = new ValidateEmail();
                validRule.txt = control;
                validRule.ErrorText = "E-mail sai định dạng";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
        }

        private void ValidationGreatThanZeroControl(SpinEdit control)
        {
            ControlGreatThanZeroValidationRule validRule = new ControlGreatThanZeroValidationRule();
            validRule.spin = control;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(control, validRule);
        }
        #endregion

        #region init data,icon,Language

        private void SetCaptionByLanguageKey()
        {
            try
            {
                HIS.Desktop.Plugins.HisUserEditableData.Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisUserEditableData.Resources.Lang", typeof(HIS.Desktop.Plugins.HisUserEditableData.frmHisUserEditableData).Assembly);
                //Inventec.Common.Resource.Get getValue;

                this.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumnLOGINNAME.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnLOGINNAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnUSERNAME.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.gridColumnUSERNAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmEmpUser.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemSearch.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemEdit.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemAdd.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.itemRedo.Caption = Inventec.Common.Resource.Get.Value("frmEmpUser.barButtonItem4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon
                  (System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        private void frmEmpUser_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                toggleSwitch1.IsOn = true;
                this.ActionType = GlobalVariables.ActionAdd;
                this.ListSelectUser = new List<ACS_USER>();
                if (!String.IsNullOrWhiteSpace(this.allowLogginames))
                {
                    string[] loginnameAllows = this.allowLogginames.Split(',');
                    if (loginnameAllows != null && loginnameAllows.Count() > 0)
                    {
                        foreach (var item in loginnameAllows)
                        {
                            var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == item);
                            if (user != null)
                            {
                                this.ListSelectUser.Add(user);
                            }
                        }
                    }
                }

                this.ListSelectUser = this.ListSelectUser != null && this.ListSelectUser.Count() > 0
                    ? this.ListSelectUser.Distinct().ToList()
                    : this.ListSelectUser;

                EnableControlChanged(this.ActionType);
                SetCaptionByLanguageKey();
                ValidateForm();
                SetDefaultValue();
                WaitingManager.Hide();

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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ActionType = GlobalVariables.ActionAdd;
                this.currentData = new ACS_USER();
                positionHandle = -1;
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    ACS_USER data = (ACS_USER)gridViewFormList.GetRow(e.RowHandle);

                    var checkExist = this.ListSelectUser != null && this.ListSelectUser.Count > 0
             ? this.ListSelectUser.FirstOrDefault(o => o.ID == data.ID)
             : null;
                    if (checkExist != null)
                    {
                        gridViewFormList.SelectRow(e.RowHandle);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                ACS_USER data = (ACS_USER)gridViewFormList.GetRow(e.ListSourceRowIndex);
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.Column.FieldName == "STT")
                    e.Value = e.ListSourceRowIndex + 1 + startPage;
                if (e.Column.FieldName == "STATUS")
                    e.Value = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "Hoạt động" : "Tạm khóa";

                if (e.Column.FieldName == "CREATE_TIME_STR")
                {
                    string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(createTime));
                }
                if (e.Column.FieldName == "MODIFY_TIME_STR")
                {
                    string mobdifyTime = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString
                      (Inventec.Common.TypeConvert.Parse.ToInt64(mobdifyTime));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                if (backgroundWorker1.IsBusy)
                {
                    WaitingManager.Show();
                }
                else
                {
                    var rowData = (ACS_USER)gridViewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        currentData = rowData;
                        //ChangedDataRow(rowData);
                    }
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
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                ACS_USER data = (ACS_USER)gridViewFormList.GetRow(e.RowHandle);
                if (e.Column.FieldName == "STATUS")
                    e.Appearance.ForeColor = data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void itemEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void itemRedo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    //FillDataToGridControl();
                    //ResetFormData();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
        }


        #endregion

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {

                //List<object> args = new List<object>();
                //HIS.Desktop.Plugins.AcsUser.CallModule call = new HIS.Desktop.Plugins.AcsUser.CallModule(HIS.Desktop.Plugins.AcsUser.CallModule.HisImprotEmpUser, 0, 0, args);
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add((RefeshReference)FillDataToGridControl);
                if (this.moduleData != null)
                {
                    HIS.Desktop.Plugins.AcsUser.CallModule callModule = new HIS.Desktop.Plugins.AcsUser.CallModule(HIS.Desktop.Plugins.AcsUser.CallModule.HisImprotEmpUser, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    HIS.Desktop.Plugins.AcsUser.CallModule callModule = new HIS.Desktop.Plugins.AcsUser.CallModule(HIS.Desktop.Plugins.AcsUser.CallModule.HisImprotEmpUser, 0, 0, listArgs);
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                //btnImport.Focus();
                WaitingManager.Show();
                btnImport_Click(null, null);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewFormList_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == CollectionChangeAction.Add)// remove or add
                {
                    var focus = (ACS_USER)gridViewFormList.GetRow(e.ControllerRow);
                    if (focus != null)
                    {
                        var checkExist = this.ListSelectUser != null && this.ListSelectUser.Count > 0
                            ? this.ListSelectUser.FirstOrDefault(o => o.ID == focus.ID)
                            : null;
                        if (checkExist == null || checkExist.ID == 0)
                        {
                            this.ListSelectUser.Add(focus);
                        }
                    }
                }
                else if (e.Action == CollectionChangeAction.Remove)
                {
                    var focus = (ACS_USER)gridViewFormList.GetRow(e.ControllerRow);
                    if (focus != null && this.ListSelectUser != null && this.ListSelectUser.Count > 0)
                    {
                        this.ListSelectUser.RemoveAll(o => o.ID == focus.ID);
                    }
                }
                else
                {
                    if (!isAll)
                    {
                        return;
                    }

                    if (this.listUser != null && this.ListSelectUser != null && this.listUser.Count() <= this.ListSelectUser.Count())
                    {
                        bool checkRemove = false;
                        foreach (var item in this.ListSelectUser)
                        {
                            var check = listUser.FirstOrDefault(o => o.ID == item.ID);
                            if (check == null)
                            {
                                checkRemove = true;
                            }
                        }

                        if (checkRemove)
                        {
                            this.ListSelectUser.RemoveAll(o => this.listUser.Select(p => p.ID).Contains(o.ID));
                        }
                    }
                    else
                    {
                        this.ListSelectUser.AddRange(this.listUser);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            try
            {
                var obj = sender as ToggleSwitch;
                if (obj != null)
                {
                    gridViewFormList.BeginDataUpdate();
                    gridControlFormList.DataSource = null;
                    if (!obj.IsOn)
                    {
                        isAll = false;
                        gridControlFormList.DataSource = this.ListSelectUser != null && this.ListSelectUser.Count > 0
                            ? this.ListSelectUser.Distinct().ToList()
                            : null;

                        gridViewFormList.SelectAll();
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    }
                    else
                    {
                        isAll = true;
                        gridControlFormList.DataSource = this.listUser;
                        layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }

                    gridViewFormList.EndDataUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlFormList_Click(object sender, EventArgs e)
        {

        }

    }
}
