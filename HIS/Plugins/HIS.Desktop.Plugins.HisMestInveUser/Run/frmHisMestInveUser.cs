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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMestInveUser.Run
{
    public partial class frmHisMestInveUser : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<HisMestInveUserAdo> listRoleUserAdo = new List<HisMestInveUserAdo>();

        internal List<HIS_EXECUTE_ROLE> listExecuteRole = new List<HIS_EXECUTE_ROLE>();
        internal List<HIS_EXECUTE_ROLE_USER> listExecuteRoleUser = new List<HIS_EXECUTE_ROLE_USER>();

        internal V_HIS_MEDI_STOCK_PERIOD _MediStockPeriod;
        internal List<V_HIS_MEST_PERIOD_METY> ListMestPeriodMety { get; set; }
        internal List<V_HIS_MEST_PERIOD_MATY> ListMestPeriodMaty { get; set; }
        internal List<V_HIS_MEST_PERIOD_BLTY> ListMestPeriodBlty { get; set; }
        internal List<HIS_USER_GROUP_TEMP> ListUserGroupTemp { get; set; }

        HisExpMestResultSDO _ExpMest { get; set; }

        public frmHisMestInveUser()
        {
            InitializeComponent();
        }

        public frmHisMestInveUser(HisExpMestResultSDO _expMest)
        {
            InitializeComponent();
            try
            {
                this._ExpMest = _expMest;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmHisMestInveUser(Inventec.Desktop.Common.Modules.Module currentModule,
            V_HIS_MEDI_STOCK_PERIOD HisMediStockPeriod
            )
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this._MediStockPeriod = HisMediStockPeriod;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisMestInveUser_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                SetIcon();
                this.ListUserGroupTemp = GetUserGroupTemp();
                FillDataCboUserGroup(cboUserGroupTemp, this.ListUserGroupTemp);
                LoadDataToCombo();
                InitData();
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

        private List<HIS_USER_GROUP_TEMP> GetUserGroupTemp()
        {
            List<HIS_USER_GROUP_TEMP> result = null;
            try
            {
                MOS.Filter.HisUserGroupTempFilter filter = new HisUserGroupTempFilter();
                filter.IS_ACTIVE = 1;
                result = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_GROUP_TEMP>>("api/HisUserGroupTemp/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                if (result != null)
                {
                    result = result.Where(o => o.USER_GROUP_TEMP_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_USER_GROUP_TEMP_TYPE.ID_HoiDongKiemKe).OrderBy(o => o.USER_GROUP_TEMP_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FillDataCboUserGroup(DevExpress.XtraEditors.GridLookUpEdit cboMediStock, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("USER_GROUP_TEMP_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USER_GROUP_TEMP_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(cboMediStock, data, controlEditorADO);
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
                else
                    btnPrintV2.Enabled = true;
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
                ComboGrdAcsUser();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ComboGrdAcsUser()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp__ExecuteRoleUser, BackendDataWorker.Get<ACS_USER>(), controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo, List<ACS.EFMODEL.DataModels.ACS_USER> data)
        //{
        //    try
        //    {
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
        //        columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
        //        ControlEditorLoader.Load(cbo, data, controlEditorADO);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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

        private void gridViewRoleUser_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisMestInveUserAdo data = null;
                //if (e.RowHandle > -1)
                //{
                //    data = (RoleUserAdo)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                //}
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

        private void repositoryItemButton__Add_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

        private void repositoryItemButton__Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

        private void repositoryItemLookUp__ExecuteRoleName_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    //gridViewRoleUser.PostEditor();
                    //if (gridViewRoleUser.EditingValue != null && gridViewRoleUser.EditingValue != null)
                    //{
                    //    var data = listExecuteRole.FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(gridViewRoleUser.EditingValue.ToString()));
                    //    if (data != null)
                    //    {
                    //        List<string> listLoginName = listExecuteRoleUser.Where(p => p.EXECUTE_ROLE_ID == data.ID).Select(p => p.LOGINNAME).ToList();
                    //        ComboAcsUser(this.repositoryItemLookUp__ExecuteRoleUser, BackendDataWorker.Get<ACS_USER>().Where(p => listLoginName.Contains(p.LOGINNAME)).ToList());
                    //    }
                    //}
                    this.repositoryItemLookUp__ExecuteRoleUser.AllowFocused = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewRoleUser.HasColumnErrors)
                    return;
                gridViewRoleUser.PostEditor();
                if (gridViewRoleUser.IsEditing)
                    gridViewRoleUser.CloseEditor();

                if (gridViewRoleUser.FocusedRowModified)
                    gridViewRoleUser.UpdateCurrentRow();
                if (this.listRoleUserAdo != null)
                {
                    var listData = listRoleUserAdo.Where(o => o.EXECUTE_ROLE_ID > 0).ToList();

                    bool success = false;
                    CommonParam param = new CommonParam();
                    List<HIS_MEST_INVE_USER> ImpMestUsers = new List<HIS_MEST_INVE_USER>();
                    MOS.SDO.HisMestInventorySDO inputAdo = new MOS.SDO.HisMestInventorySDO();
                    foreach (var item in listData)
                    {
                        HIS_MEST_INVE_USER ado = new HIS_MEST_INVE_USER();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEST_INVE_USER>(ado, item);
                        ado.USERNAME = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(p => p.LOGINNAME
                             == item.LOGINNAME).USERNAME;
                        ImpMestUsers.Add(ado);
                    }
                    inputAdo.MestInveUsers = ImpMestUsers;
                    inputAdo.MediStockPeriodId = this._MediStockPeriod.ID;
                    Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("api/HisMestInventory/Create ,, Gui Len : -- ", inputAdo));
                    var rsOutPut = new BackendAdapter(param).Post<HisMestInventoryResultSDO>("api/HisMestInventory/Create", ApiConsumers.MosConsumer, inputAdo, param);
                    if (rsOutPut != null)
                    {
                        success = true;
                        btnSave.Enabled = false;
                        btnPrintV2.Enabled = true;
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                //PrintProcess(PrintType.BIEN_BAN_KIEM_NHAP_TU_NCC);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                gridViewRoleUser.PostEditor();
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Print_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnPrintV2.Enabled)
                {
                    btnPrint_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoleUser_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                gridViewRoleUser.ClearColumnErrors();
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HisMestInveUserAdo data = view.GetFocusedRow() as HisMestInveUserAdo;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

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

        public void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();

                List<ACS.EFMODEL.DataModels.ACS_USER> acsUserAlows = new List<ACS.EFMODEL.DataModels.ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = data.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Họ tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;

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

        private void btnPrintV2_Click(object sender, EventArgs e)
        {
            try
            {
               

                PrintProcess(PrintType.IN_BIEN_BAN_KIEM_KE_T_VT_M);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintType
        {
            IN_BIEN_BAN_KIEM_KE_T_VT_M,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BIEN_BAN_KIEM_KE_T_VT_M:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEN_BAN_KIEM_KE_THUOC_VT_MAU__MPS000132, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEN_BAN_KIEM_KE_THUOC_VT_MAU__MPS000132:
                        LoadDataPrint(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadDataPrint(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
               

                List<V_HIS_MEST_INVE_USER> listUser = new List<V_HIS_MEST_INVE_USER>();
                List<V_HIS_MEST_PERIOD_MATE> mestPeriodMateList = new List<V_HIS_MEST_PERIOD_MATE>();
                List<V_HIS_MEST_PERIOD_MEDI> mestPeriodMetiList = new List<V_HIS_MEST_PERIOD_MEDI>();
                List<long> medistockIds = new List<long>();
                if (this._MediStockPeriod != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisMestPeriodMediViewFilter mediFilter = new MOS.Filter.HisMestPeriodMediViewFilter();
                    mediFilter.MEDI_STOCK_PERIOD_ID = this._MediStockPeriod.ID;
                    mestPeriodMetiList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEST_PERIOD_MEDI>>("api/HisMestPeriodMedi/GetView", ApiConsumer.ApiConsumers.MosConsumer, mediFilter, null);
                    // mest_period_mate
                    MOS.Filter.HisMestPeriodMateViewFilter mateFilter = new MOS.Filter.HisMestPeriodMateViewFilter();
                    mateFilter.MEDI_STOCK_PERIOD_ID = this._MediStockPeriod.ID;
                    mestPeriodMateList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_MEST_PERIOD_MATE>>("api/HisMestPeriodMate/GetView", ApiConsumer.ApiConsumers.MosConsumer, mateFilter, null);


                    if (mestPeriodMetiList != null && mestPeriodMetiList.Count() > 0)
                    {
                        medistockIds.AddRange(mestPeriodMetiList.Where(p => p.MEDI_STOCK_ID.HasValue).Select(o => o.MEDI_STOCK_ID ?? 0).ToList());
                    }
                    if (mestPeriodMateList != null && mestPeriodMateList.Count() > 0)
                    {
                        medistockIds.AddRange(mestPeriodMateList.Where(p => p.MEDI_STOCK_ID.HasValue).Select(o => o.MEDI_STOCK_ID ?? 0).ToList());
                    }

                    medistockIds = medistockIds != null ? medistockIds.Distinct().ToList() : medistockIds;

                    MOS.Filter.HisMestInventoryFilter mestInventoryFilter = new HisMestInventoryFilter();
                    mestInventoryFilter.MEDI_STOCK_PERIOD_ID = this._MediStockPeriod.ID;
                    var rsMestInventory = new BackendAdapter(param).Get<List<HIS_MEST_INVENTORY>>("api/HisMestInventory/Get", ApiConsumers.MosConsumer, mestInventoryFilter, param);

                    if (rsMestInventory != null && rsMestInventory.Count > 0)
                    {
                        MOS.Filter.HisMestInveUserViewFilter userFilter = new MOS.Filter.HisMestInveUserViewFilter();
                        userFilter.MEST_INVENTORY_ID = rsMestInventory.FirstOrDefault().ID;
                        listUser = new BackendAdapter(param).Get<List<V_HIS_MEST_INVE_USER>>("api/HisMestInveUser/GetView", ApiConsumers.MosConsumer, userFilter, param);
                    }

                    MOS.Filter.HisMestPeriodMetyViewFilter mestPeriodMetyFilter = new MOS.Filter.HisMestPeriodMetyViewFilter();
                    mestPeriodMetyFilter.MEDI_STOCK_PERIOD_ID = this._MediStockPeriod.ID;
                    ListMestPeriodMety = new List<V_HIS_MEST_PERIOD_METY>();
                    ListMestPeriodMety = new BackendAdapter(param).Get<List<V_HIS_MEST_PERIOD_METY>>(HisRequestUriStore.HIS_MEST_PERIOD_METY_GETVIEW, ApiConsumers.MosConsumer, mestPeriodMetyFilter, param);

                    MOS.Filter.HisMestPeriodMatyViewFilter mestPeriodMatyFilter = new MOS.Filter.HisMestPeriodMatyViewFilter();
                    mestPeriodMatyFilter.MEDI_STOCK_PERIOD_ID = this._MediStockPeriod.ID;
                    ListMestPeriodMaty = new List<V_HIS_MEST_PERIOD_MATY>();
                    ListMestPeriodMaty = new BackendAdapter(param).Get<List<V_HIS_MEST_PERIOD_MATY>>(HisRequestUriStore.HIS_MEST_PERIOD_MATY_GETVIEW, ApiConsumers.MosConsumer, mestPeriodMatyFilter, param);

                }
                List<HIS_MEDI_STOCK_METY> mediStockMetyList = new List<HIS_MEDI_STOCK_METY>();
                if (ListMestPeriodMety != null && ListMestPeriodMety.Count() > 0)
                {
                    MOS.Filter.HisMediStockMetyFilter medistockMetyFilter = new MOS.Filter.HisMediStockMetyFilter();
                    medistockMetyFilter.MEDICINE_TYPE_IDs = ListMestPeriodMety.Select(o => o.MEDICINE_TYPE_ID).ToList();
                    mediStockMetyList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_METY>>("api/HisMediStockMety/Get", ApiConsumer.ApiConsumers.MosConsumer, medistockMetyFilter, null);
                }

                List<HIS_MEDI_STOCK_MATY> mediStockMatyList = new List<HIS_MEDI_STOCK_MATY>();
                if (ListMestPeriodMaty != null && ListMestPeriodMaty.Count() > 0)
                {
                    MOS.Filter.HisMediStockMatyFilter medistockMetyFilter = new MOS.Filter.HisMediStockMatyFilter();
                    medistockMetyFilter.MATERIAL_TYPE_IDs = ListMestPeriodMaty.Select(o => o.MATERIAL_TYPE_ID).ToList();
                    mediStockMatyList = new BackendAdapter(new CommonParam()).Get<List<HIS_MEDI_STOCK_MATY>>("api/HisMediStockMaty/Get", ApiConsumer.ApiConsumers.MosConsumer, medistockMetyFilter, null);
                }

                List<HIS_MEDICINE_GROUP> medicineGroups = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_GROUP>();

                WaitingManager.Hide();
                MPS.Processor.Mps000132.PDO.Mps000132PDO mps000132RDO = new MPS.Processor.Mps000132.PDO.Mps000132PDO(
                   this._MediStockPeriod,
                   listUser,
                   mestPeriodMetiList,
                   mestPeriodMateList,
                   medicineGroups,
                   medistockIds,
                   mediStockMetyList,
                   mediStockMatyList
                );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000132RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000132RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //chỉ hiển thị khi còn 1 dòng có dữ liệu
        private void repositoryItemLookUp__ExecuteRoleUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.listRoleUserAdo = new List<HisMestInveUserAdo>();
                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                    RoleUserAdo.Action = GlobalVariables.ActionAdd;
                    this.listRoleUserAdo.Add(RoleUserAdo);
                    gridControlRoleUser.BeginUpdate();
                    gridControlRoleUser.DataSource = null;
                    gridControlRoleUser.DataSource = listRoleUserAdo;
                    gridControlRoleUser.EndUpdate();
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoleUser_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                EnableButtonDelete();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlRoleUser_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                EnableButtonDelete();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableButtonDelete()
        {
            try
            {
                if (listRoleUserAdo != null && listRoleUserAdo.Count == 1 && !String.IsNullOrWhiteSpace(listRoleUserAdo.First().LOGINNAME))
                {
                    repositoryItemLookUp__ExecuteRoleUser.Buttons[1].Visible = true;
                }
                else
                {
                    repositoryItemLookUp__ExecuteRoleUser.Buttons[1].Visible = false;
                }

                if (listRoleUserAdo != null && listRoleUserAdo.Exists(o => !String.IsNullOrWhiteSpace(o.LOGINNAME)))
                {
                    btnSave.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboUserGroupTemp_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboUserGroupTemp.EditValue != null)
                    {
                        var rs = this.ListUserGroupTemp.Where(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboUserGroupTemp.EditValue.ToString())).FirstOrDefault();
                        if (rs != null)
                        {
                            this.listRoleUserAdo = new List<HisMestInveUserAdo>();
                            MOS.Filter.HisUserGroupTempDtFilter filter = new HisUserGroupTempDtFilter();
                            filter.USER_GROUP_TEMP_ID = rs.ID;
                            var userGroupDtList = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_GROUP_TEMP_DT>>("api/HisUserGroupTempDt/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                            if (userGroupDtList != null && userGroupDtList.Count > 0)
                            {
                                foreach (var item in userGroupDtList)
                                {
                                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo(item);
                                    this.listRoleUserAdo.Add(RoleUserAdo);
                                }
                            }
                            if (this.listRoleUserAdo == null || this.listRoleUserAdo.Count == 0)
                            {
                                HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                                RoleUserAdo.Action = GlobalVariables.ActionAdd;
                                this.listRoleUserAdo.Add(RoleUserAdo);
                            }
                            else
                                btnPrintV2.Enabled = true;
                            gridControlRoleUser.DataSource = null;
                            gridControlRoleUser.DataSource = this.listRoleUserAdo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveUserGroupTemp_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewRoleUser.HasColumnErrors)
                    return;
                if (this.listRoleUserAdo != null)
                {
                    var listData = listRoleUserAdo.Where(o => o.EXECUTE_ROLE_ID > 0).ToList();

                    frmUserGroupTempCreate frm = new frmUserGroupTempCreate(listData);
                    frm.ShowDialog();

                    this.ListUserGroupTemp = GetUserGroupTemp();
                    FillDataCboUserGroup(cboUserGroupTemp, this.ListUserGroupTemp);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemLookUp__ExecuteRoleUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LookUpEdit lookUp = sender as LookUpEdit;
                if (lookUp != null && lookUp.EditValue != null)
                {
                    if (gridViewRoleUser.IsEditing)
                        gridViewRoleUser.CloseEditor();

                    if (gridViewRoleUser.FocusedRowModified)
                        gridViewRoleUser.UpdateCurrentRow();
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRoleUser_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.HitTest == GridHitTest.RowCell)
                    {
                        if (hi.Column.FieldName == "LOGINNAME")
                        {
                            var row = (HisMestInveUserAdo)gridViewRoleUser.GetRow(hi.RowHandle);
                            if (row != null)
                            {

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

        private void repositoryItemGridLookUp__ExecuteRoleUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.listRoleUserAdo = new List<HisMestInveUserAdo>();
                    HisMestInveUserAdo RoleUserAdo = new HisMestInveUserAdo();
                    RoleUserAdo.Action = GlobalVariables.ActionAdd;
                    this.listRoleUserAdo.Add(RoleUserAdo);
                    gridControlRoleUser.BeginUpdate();
                    gridControlRoleUser.DataSource = null;
                    gridControlRoleUser.DataSource = listRoleUserAdo;
                    gridControlRoleUser.EndUpdate();
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemGridLookUp__ExecuteRoleUser_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                LookUpEdit lookUp = sender as LookUpEdit;
                if (lookUp != null && lookUp.EditValue != null)
                {
                    if (gridViewRoleUser.IsEditing)
                        gridViewRoleUser.CloseEditor();

                    if (gridViewRoleUser.FocusedRowModified)
                        gridViewRoleUser.UpdateCurrentRow();
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
