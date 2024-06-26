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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.MediStockPeriod.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Plugins.MediStockPeriod;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MediStockPeriod.CreatePeriod
{
    public partial class frmCreatePeriodNew : HIS.Desktop.Utility.FormBase
    {
        int positionHandleControlBedInfo = -1;
        RefeshData refeshData;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_MEDI_STOCK_PERIOD _MediStockPeriod = new V_HIS_MEDI_STOCK_PERIOD();
        int Action = 0;
        List<HIS_EXECUTE_ROLE> listExecuteRole = new List<HIS_EXECUTE_ROLE>();
        List<HIS_EXECUTE_ROLE_USER> listExecuteRoleUser = new List<HIS_EXECUTE_ROLE_USER>();
        List<HisMestInveUserAdo> listRoleUserAdo = new List<HisMestInveUserAdo>();

        public frmCreatePeriodNew()
            : base()
        {
            InitializeComponent();
        }

        public frmCreatePeriodNew(Inventec.Desktop.Common.Modules.Module _module, RefeshData refeshData)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this.refeshData = refeshData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmCreatePeriodNew(Inventec.Desktop.Common.Modules.Module _module, V_HIS_MEDI_STOCK_PERIOD dataEdit, RefeshData refeshData)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this.refeshData = refeshData;
                this._MediStockPeriod = dataEdit;
                this.Action = GlobalVariables.ActionEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmCreatePeriod_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                ValidateBedForm();
                LoadDataToComboMedistock();
                if (this._MediStockPeriod != null && this._MediStockPeriod.ID > 0)
                {
                    txtMediStockPeriodName.Text = this._MediStockPeriod.MEDI_STOCK_PERIOD_NAME;
                    cboMediStock.EditValue = this._MediStockPeriod.MEDI_STOCK_ID;
                    txtMediStockCode.Text = this._MediStockPeriod.MEDI_STOCK_CODE;
                    cboMediStock.Enabled = false;
                    txtMediStockCode.Enabled = false;

                    dtTimePeriod.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this._MediStockPeriod.TO_TIME ?? 0) ?? DateTime.Now;
                    dtTimePeriod.Enabled = false;
                }
                LoadDataToCombo();
                InitData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboMedistock()
        {
            CommonParam param = new CommonParam();
            try
            {
                List<V_HIS_MEDI_STOCK> mediStockImp = new List<V_HIS_MEDI_STOCK>();
                var _WorkPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);
                if (_WorkPlace != null)
                {
                    var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim() && _WorkPlace.BranchId == p.BRANCH_ID).ToList();
                    if (datas != null && datas.Count > 0)
                    {
                        List<long> roomIds = datas.Select(p => p.ROOM_ID).ToList();
                        mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => p.DEPARTMENT_ID == _WorkPlace.DepartmentId && roomIds.Contains(p.ROOM_ID) && p.IS_ACTIVE == 1).OrderBy(p => p.MEDI_STOCK_NAME).ToList();
                    }
                }

                cboMediStock.Properties.DataSource = mediStockImp;
                cboMediStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                cboMediStock.Properties.ValueMember = "ID";
                cboMediStock.Properties.ForceInitialize();
                cboMediStock.Properties.Columns.Clear();
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 100));
                cboMediStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 200));
                cboMediStock.Properties.ShowHeader = false;
                cboMediStock.Properties.ImmediatePopup = true;
                cboMediStock.Properties.PopupWidth = 300;
                if (mediStockImp != null && mediStockImp.Count > 0)
                {
                    var data = mediStockImp.FirstOrDefault(p => p.ROOM_ID == this.currentModule.RoomId);
                    if (data != null)
                    {
                        this.cboMediStock.EditValue = data.ID;
                        this.txtMediStockCode.Text = data.MEDI_STOCK_CODE;
                    }
                }

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitData()
        {
            try
            {
                this.listRoleUserAdo = new List<HisMestInveUserAdo>();
                if (this._MediStockPeriod != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMestInventoryFilter mestInventoryFilter = new HisMestInventoryFilter();
                    mestInventoryFilter.MEDI_STOCK_PERIOD_ID = this._MediStockPeriod.ID;
                    var rsMestInventory = new BackendAdapter(param).Get<List<HIS_MEST_INVENTORY>>("api/HisMestInventory/Get", ApiConsumers.MosConsumer, mestInventoryFilter, param);

                    if (rsMestInventory != null && rsMestInventory.Count > 0)
                    {
                        MOS.Filter.HisMestInveUserFilter userFilter = new MOS.Filter.HisMestInveUserFilter();
                        userFilter.MEST_INVENTORY_ID = rsMestInventory.FirstOrDefault().ID;
                        var rsImpMestUser = new BackendAdapter(param).Get<List<HIS_MEST_INVE_USER>>("api/HisMestInveUser/Get", ApiConsumers.MosConsumer, userFilter, param);
                        if (rsImpMestUser != null && rsImpMestUser.Count > 0)
                        {
                            foreach (var item in rsImpMestUser)
                            {
                                HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo(item);
                                this.listRoleUserAdo.Add(RoleUserAdo);
                            }
                        }
                    }
                }
                if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                {
                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                    RoleUserAdo.Action = GlobalVariables.ActionAdd;
                    this.listRoleUserAdo.Add(RoleUserAdo);
                }
                //else
                //    btnPrintV2.Enabled = true;
                gridControlRoleUser.DataSource = null;
                gridControlRoleUser.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExecuteRoleFilter roleFilter = new MOS.Filter.HisExecuteRoleFilter();
                listExecuteRole = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE>>(HisRequestUriStore.HIS_EXECUTE_ROLE_GET, ApiConsumers.MosConsumer, roleFilter, param);
                InitComboLookUp(this.repositoryItemLookUp__ExecuteRoleName, listExecuteRole);

                MOS.Filter.HisExecuteRoleUserFilter roleUserFilter = new MOS.Filter.HisExecuteRoleUserFilter();
                listExecuteRoleUser = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumers.MosConsumer, roleUserFilter, param);
                ComboAcsUser(this.repositoryItemLookUp__ExecuteRoleUser, BackendDataWorker.Get<ACS_USER>());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo, List<ACS.EFMODEL.DataModels.ACS_USER> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboLookUp(RepositoryItemLookUpEdit cbo, List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(LookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, acsUserAlows, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMediStockCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    if (String.IsNullOrEmpty(strValue))
                    {
                        cboMediStock.EditValue = null;
                        cboMediStock.Focus();
                        cboMediStock.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediStock.EditValue != null)
                    {
                        var mediStockImp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>()
                            .FirstOrDefault(o =>
                                o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStock.EditValue ?? 0).ToString()));
                        if (mediStockImp != null)
                        {
                            txtMediStockCode.Text = mediStockImp.MEDI_STOCK_CODE;
                        }
                    }
                    dtTimePeriod.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboMediStock.EditValue != null)
                {
                    var mediStockImp = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>()
                            .FirstOrDefault(o => o.ID == (long)cboMediStock.EditValue);
                    if (mediStockImp != null)
                    {
                        txtMediStockCode.Text = mediStockImp.MEDI_STOCK_CODE;
                        dtTimePeriod.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlBedInfo == -1)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlBedInfo > edit.TabIndex)
                {
                    positionHandleControlBedInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateBedForm()
        {
            ValidateWithTextEdit(txtMediStockPeriodName);
            ValidateComboWithTextEdit(cboMediStock, txtMediStockCode);

        }

        private void ValidateWithTextEdit(TextEdit txtTextEdit)
        {
            try
            {
                TextEditValidationRule validRule = new TextEditValidationRule();
                validRule.txtTextEdit = txtTextEdit;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTextEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateComboWithTextEdit(LookUpEdit cbo, TextEdit txtTextEdit)
        {
            try
            {
                //LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                //validRule.txtTextEdit = txtTextEdit;
                //validRule.cbo = cbo;
                //validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                //validRule.ErrorType = ErrorType.Warning;
                //dxValidationProvider1.SetValidationRule(txtTextEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControlBedInfo = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HIS_MEDI_STOCK_PERIOD dataCreate = new HIS_MEDI_STOCK_PERIOD();
                dataCreate.MEDI_STOCK_PERIOD_NAME = txtMediStockPeriodName.Text;
                dataCreate.MEDI_STOCK_ID = (long)cboMediStock.EditValue;
                if (dtTimePeriod.EditValue != null)
                    dataCreate.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimePeriod.DateTime);
                else
                    dataCreate.TO_TIME = null;

                List<HIS_MEST_INVE_USER> impMestUsers = GetImpMestUserForSave();
                //TODO

                HIS_MEDI_STOCK_PERIOD result = new HIS_MEDI_STOCK_PERIOD();
                if (this.Action == GlobalVariables.ActionEdit && this._MediStockPeriod != null && this._MediStockPeriod.ID > 0)
                {
                    dataCreate.ID = this._MediStockPeriod.ID;
                    result = new BackendAdapter(param).Post<HIS_MEDI_STOCK_PERIOD>(RequestUriStore.HIS_MEST_STOCK_PERIOD_UPDATE, ApiConsumers.MosConsumer, dataCreate, param);
                }
                else
                    result = new BackendAdapter(param).Post<HIS_MEDI_STOCK_PERIOD>(RequestUriStore.HIS_MEST_STOCK_PERIOD_CREATE, ApiConsumers.MosConsumer, dataCreate, param);
                if (result != null)
                {
                    this.refeshData();
                    this.Close();
                    success = true;
                }
                WaitingManager.Hide();
                #region ShowMessager
                MessageManager.Show(this.ParentForm, param, success);
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<HIS_MEST_INVE_USER> GetImpMestUserForSave()
        {
            List<HIS_MEST_INVE_USER> impMestUsers = new List<HIS_MEST_INVE_USER>();
            try
            {
                if (this.listRoleUserAdo != null && this.listRoleUserAdo.Count > 0)
                {
                    var listData = this.listRoleUserAdo.Where(o => o.EXECUTE_ROLE_ID > 0).ToList();
                    MOS.SDO.HisMestInventorySDO inputAdo = new MOS.SDO.HisMestInventorySDO();
                    foreach (var item in listData)
                    {
                        HIS_MEST_INVE_USER ado = new HIS_MEST_INVE_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEST_INVE_USER>(ado, item);
                        ado.USERNAME = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                             == item.LOGINNAME).USERNAME;
                        impMestUsers.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            
            return impMestUsers;
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimePeriod_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtTimePeriod.EditValue != null)
                    {
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimePeriod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTimePeriod.EditValue != null)
                    {
                        btnSave.Focus();
                    }
                    else
                        dtTimePeriod.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRoleUser_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisMestInveUserAdo data = null;
                if (e.RowHandle > 0)
                {
                    data = (HisMestInveUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "ADD")
                    {
                        if (data.Action == GlobalVariables.ActionAdd)
                        {
                            e.RepositoryItem = repositoryItemButton__Add;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRoleUser_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                gridViewRoleUser.ClearColumnErrors();
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisMestInveUserAdo data = view.GetFocusedRow() as HisMestInveUserAdo;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is LookUpEdit)
                {
                    LookUpEdit editor = view.ActiveEditor as LookUpEdit;

                    List<string> loginNames = new List<string>();
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (data.LOGINNAME != null)
                            editor.EditValue = data.LOGINNAME;
                        var rs = listExecuteRoleUser.Where(p => p.EXECUTE_ROLE_ID == data.EXECUTE_ROLE_ID).ToList();
                        if (rs != null && rs.Count > 0)
                        {
                            loginNames = rs.Select(o => o.LOGINNAME).Distinct().ToList();
                        }
                    }

                    ComboAcsUser(editor, loginNames);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRoleUser_ValidateRow(object sender, ValidateRowEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    GridView view = sender as GridView;
                    GridColumn onOrderCol = view.Columns["LOGINNAME"];
                    var data = (HisMestInveUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null && data.EXECUTE_ROLE_ID > 0)
                    {
                        if (String.IsNullOrEmpty(data.LOGINNAME.Trim()))
                        {
                            e.Valid = false;
                            view.SetColumnError(onOrderCol, "Trường dữ liệu bắt buộc");
                        }
                        else
                        {
                            var ktra = this.listRoleUserAdo.Where(p => p.LOGINNAME == data.LOGINNAME).ToList();
                            if (ktra != null && ktra.Count > 1)
                            {
                                e.Valid = false;
                                view.SetColumnError(onOrderCol, "'" + BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                             == data.LOGINNAME).USERNAME + "'" + " đã được gán vai trò");
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

        private void gridViewRoleUser_InvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            try
            {
                //Suppress displaying the error message box
                e.ExceptionMode = ExceptionMode.NoAction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Add_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                RoleUserAdo.Action = GlobalVariables.ActionEdit;
                this.listRoleUserAdo.Add(RoleUserAdo);
                gridControlRoleUser.DataSource = null;
                gridControlRoleUser.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var row = (HisMestInveUserAdo)gridViewRoleUser.GetFocusedRow();
                if (row != null)
                {
                    this.listRoleUserAdo.Remove(row);
                }
                if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                {
                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                    RoleUserAdo.Action = GlobalVariables.ActionAdd;
                    this.listRoleUserAdo.Add(RoleUserAdo);
                }
                gridControlRoleUser.DataSource = null;
                gridControlRoleUser.DataSource = this.listRoleUserAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemLookUp__ExecuteRoleName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    this.repositoryItemLookUp__ExecuteRoleUser.AllowFocused = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
