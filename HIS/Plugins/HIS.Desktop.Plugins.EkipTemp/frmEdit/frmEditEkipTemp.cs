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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.EkipTemp.Ado;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.Mapper;
using Inventec.Common.TypeConvert;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EkipTemp.frmEdit
{
    public partial class frmEditEkipTemp : FormBase
    {
        private int actionType;
        private HIS_EKIP_TEMP_USER currentData;
        private HIS_EKIP_TEMP currentData2;
        private int dataTotal;
        private HIS_EKIP_TEMP ekipTemp;
        internal List<EkipTempUserADO> ekipUserAdoTemps;
        private int positionHandle;
        private int rowCount;
        Inventec.Desktop.Common.Modules.Module currentModule;
        private long DepartmentID;
        List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE> Execute;

        private int startPage;
        // Methods
        public frmEditEkipTemp(DelegateRefreshData _refeshData, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.ekipUserAdoTemps = new List<EkipTempUserADO>();
            this.ekipTemp = new HIS_EKIP_TEMP();
            this.rowCount = 0;
            this.dataTotal = 0;
            this.startPage = 0;
            this.positionHandle = -1;
            this.components = null;
            this.currentModule = module;
            this.InitializeComponent();
            this.refeshData = _refeshData;
            try
            {
                this.actionType = 1;
                string filePath = Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                base.Icon = Icon.ExtractAssociatedIcon(filePath);
            }
            catch (Exception exception)
            {
                LogSystem.Warn(exception);
            }
        }

        public frmEditEkipTemp(HIS_EKIP_TEMP _data, DelegateRefreshData _refeshData, Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            this.ekipUserAdoTemps = new List<EkipTempUserADO>();
            this.ekipTemp = new HIS_EKIP_TEMP();
            this.rowCount = 0;
            this.dataTotal = 0;
            this.startPage = 0;
            this.positionHandle = -1;
            this.components = null;
            this.currentModule = module;
            this.InitializeComponent();
            this.refeshData = _refeshData;
            this.ekipTemp = _data;
            this.actionType = 2;
            try
            {
                string filePath = Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                base.Icon = Icon.ExtractAssociatedIcon(filePath);
            }
            catch (Exception exception)
            {
                LogSystem.Warn(exception);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                List<EkipTempUserADO> dataSource = this.grdControlInformationSurg.DataSource as List<EkipTempUserADO>;
                if ((dataSource == null) || (dataSource.Count < 1))
                {
                    EkipTempUserADO item = new EkipTempUserADO
                    {
                        Action = 2
                    };
                    this.ekipUserAdoTemps.Add(item);
                    this.grdControlInformationSurg.DataSource = null;
                    this.grdControlInformationSurg.DataSource = this.ekipUserAdoTemps;
                }
                else
                {
                    EkipTempUserADO rado2 = new EkipTempUserADO
                    {
                        Action = 2
                    };
                    dataSource.Add(rado2);
                    this.grdControlInformationSurg.DataSource = null;
                    this.grdControlInformationSurg.DataSource = dataSource;
                }
            }
            catch (Exception exception)
            {
                LogSystem.Warn(exception);
            }
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
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

        private void ComboAcsUser(DevExpress.XtraEditors.Repository.RepositoryItemSearchLookUpEdit cbo)
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                cbo.DataSource = data;
                cbo.DisplayMember = "USERNAME";
                cbo.ValueMember = "LOGINNAME";
                cbo.TextEditStyle = TextEditStyles.Standard;
                cbo.PopupFilterMode = PopupFilterMode.Contains;
                cbo.ImmediatePopup = true;
                cbo.View.Columns.Clear();
                GridColumn column = cbo.View.Columns.AddField("LOGINNAME");
                column.Caption = "Mã";
                column.Visible = true;
                column.VisibleIndex = 1;
                column.Width = 100;
                GridColumn column2 = cbo.View.Columns.AddField("USERNAME");
                column2.Caption = "Tên";
                column2.Visible = true;
                column2.VisibleIndex = 2;
                column2.Width = 200;

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ComboExecuteRole(DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit cbo)
        {
            try
            {
                Execute = new List<HIS_EXECUTE_ROLE>();
                Execute = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE>();
                Execute = Execute.Where(dt => dt.IS_ACTIVE == 1).ToList();
                
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, Execute, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<EkipTempUserADO> dataSource = this.grdControlInformationSurg.DataSource as List<EkipTempUserADO>;
                EkipTempUserADO focusedRow = (EkipTempUserADO)this.grdViewInformationSurg.GetFocusedRow();
                if ((focusedRow != null) && (dataSource.Count > 0))
                {
                    dataSource.Remove(focusedRow);
                    this.grdControlInformationSurg.DataSource = null;
                    this.grdControlInformationSurg.DataSource = dataSource;
                }
            }
            catch (Exception exception)
            {
                LogSystem.Warn(exception);
            }
        }


        private void FillDatatoEkipTempUser(HIS_EKIP_TEMP data)
        {
            try
            {
                List<EkipTempUserADO> list = new List<EkipTempUserADO>();
                CommonParam commonParam = new CommonParam();
                MOS.Filter.HisEkipTempUserViewFilter filter = new MOS.Filter.HisEkipTempUserViewFilter();
                filter.EKIP_TEMP_ID = data.ID;
                List<V_HIS_EKIP_TEMP_USER> list2 = new BackendAdapter(commonParam).Get<List<V_HIS_EKIP_TEMP_USER>>("api/HisEkipTempUser/GetView", ApiConsumers.MosConsumer, filter, commonParam);
                if (list2.Count > 0)
                {
                    foreach (V_HIS_EKIP_TEMP_USER v_his_ekip_temp_user in list2)
                    {
                        EkipTempUserADO objDestination = new EkipTempUserADO();
                        DataObjectMapper.Map<EkipTempUserADO>(objDestination, v_his_ekip_temp_user);
                        if (v_his_ekip_temp_user != list2[0])
                        {
                            objDestination.Action = 2;
                        }
                        else
                        {
                            objDestination.Action = 1;
                        }
                        list.Add(objDestination);

                    }
                    this.grdControlInformationSurg.DataSource = null;
                    this.grdControlInformationSurg.DataSource = list;



                }
            }
            catch (Exception exception)
            {
                LogSystem.Error(exception);
                WaitingManager.Hide();
            }
        }

        private void frmEditEkipTemp_Load(object sender, EventArgs e)
        {
            if (this.currentModule.RoomId > 0)
            {
                DepartmentID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId).DepartmentId;

                this.checkPublicDepartment.Enabled = true;
            }
            else 
            {
                this.checkPublicDepartment.Checked = false;
                this.checkPublicDepartment.Enabled = false;
                this.checkPublicDepartment.ToolTip = " Để sử dụng tính năng này, bạn cần mở chức năng \"kíp mẫu\" từ phòng làm việc";
            }

            this.LoadDataToCombo();
            this.FillDatatoEkipTempUser(this.ekipTemp);
            if (this.actionType == 2)
            {
                this.textEdit1.Text = this.ekipTemp.EKIP_TEMP_NAME;
                if (this.ekipTemp.IS_PUBLIC == 1)
                {
                    this.checkPublic.Checked = true;
                }
                if (this.ekipTemp.IS_PUBLIC_IN_DEPARTMENT == 1)
                {
                    this.checkPublicDepartment.Checked = true;
                }
            }
            if (this.grdControlInformationSurg.DataSource == null)
            {
                this.InitGrid();
            }
            this.ValidationSingleControl(this.textEdit1);
        }

        private void grdViewInformationSurg_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int num = Convert.ToInt32(e.RowHandle);
                    switch (Parse.ToInt32((this.grdViewInformationSurg.GetRowCellValue(e.RowHandle, "Action") ?? "").ToString()))
                    {
                        case 1:
                            e.RepositoryItem = this.btnAdd;
                            break;

                        case 2:
                            e.RepositoryItem = this.btnDelete;
                            break;
                    }
                }
            }
            catch (Exception exception)
            {
                LogSystem.Error(exception);
            }
        }

        private void InitGrid()
        {
            try
            {
                List<EkipTempUserADO> list = new List<EkipTempUserADO>();
                EkipTempUserADO item = new EkipTempUserADO
                {
                    Action = 1
                };
                list.Add(item);
                this.grdControlInformationSurg.DataSource = list;
            }
            catch (Exception)
            {
                throw;
            }
        }


        private void LoadDataToCombo()
        {
            try
            {
                this.ComboAcsUser(repositoryItemSearchLookUpEdit1);
                this.ComboExecuteRole(cboPosition);
            }
            catch (Exception exception)
            {
                LogSystem.Error(exception);
            }
        }


        private void simpleButton1_Click(object sender, EventArgs e)
        {
            bool flag = true;
            bool flag2 = false;
            try
            {
                this.positionHandle = -1;
                if (this.dxValidationProvider1.Validate() && flag)
                {
                    List<EkipTempUserADO> dataSource;
                    List<HIS_EKIP_TEMP_USER> list2;
                    HIS_EKIP_TEMP_USER his_ekip_temp_user;
                    string mess = "";

                    WaitingManager.Show();
                    CommonParam commonParam = new CommonParam();
                    this.ekipTemp.EKIP_TEMP_NAME = this.textEdit1.Text;
                    if (this.checkPublic.Checked)
                    {
                        this.ekipTemp.IS_PUBLIC = 1;
                    }
                    else
                    {
                        this.ekipTemp.IS_PUBLIC = null;
                    }

                    if (this.checkPublicDepartment.Checked)
                    {
                        this.ekipTemp.IS_PUBLIC_IN_DEPARTMENT = 1;
                    }
                    else 
                    {
                        this.ekipTemp.IS_PUBLIC_IN_DEPARTMENT = null;
                    }

                    if (this.currentModule.RoomId > 0)
                    {
                        
                        this.ekipTemp.DEPARTMENT_ID = DepartmentID;
                    }
                    else 
                    {
                        this.ekipTemp.DEPARTMENT_ID = null;
                    }

                    HIS_EKIP_TEMP his_ekip_temp = new HIS_EKIP_TEMP();
                    if (this.actionType == 2)
                    {
                        dataSource = this.grdControlInformationSurg.DataSource as List<EkipTempUserADO>;
                        //if (CS$<>9__CachedAnonymousMethodDelegate1 == null)
                        //{
                        //    CS$<>9__CachedAnonymousMethodDelegate1 = new Func<EkipTempUserADO, bool>(null, (IntPtr) <simpleButton1_Click>b__0);
                        //}
                        //bool flag3 = Enumerable.Where<EkipTempUserADO>(dataSource, CS$<>9__CachedAnonymousMethodDelegate1).FirstOrDefault<EkipTempUserADO>() != null;
                        list2 = new List<HIS_EKIP_TEMP_USER>();
                        if ((dataSource != null) && (dataSource.Count > 0))
                        {
                            string messError = "";
                            foreach (EkipTempUserADO rado in dataSource)
                            {
                                if (!string.IsNullOrEmpty(rado.LOGINNAME))
                                {
                                    his_ekip_temp_user = new HIS_EKIP_TEMP_USER
                                    {
                                        EXECUTE_ROLE_ID = rado.EXECUTE_ROLE_ID,
                                        USERNAME = rado.USERNAME,
                                        LOGINNAME = rado.LOGINNAME
                                    };
                                    list2.Add(his_ekip_temp_user);
                                }
                                else
                                {
                                    messError += String.Format("vai trò {0} không có tên tài khoản.\n", Execute.FirstOrDefault(o => o.ID == rado.EXECUTE_ROLE_ID).EXECUTE_ROLE_NAME);
                                }
                            }

                            if (!string.IsNullOrEmpty(messError))
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(messError, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            this.ekipTemp.HIS_EKIP_TEMP_USER = list2;
                        }

                        if (this.ekipTemp.HIS_EKIP_TEMP_USER != null && this.ekipTemp.HIS_EKIP_TEMP_USER.Count > 0)
                        {
                            var lstLoginName = this.ekipTemp.HIS_EKIP_TEMP_USER.Select(o => o.LOGINNAME).Distinct().ToList();
                            foreach (var Login in lstLoginName)
                            {
                                var check = this.ekipTemp.HIS_EKIP_TEMP_USER.Where(o => o.LOGINNAME == Login).ToList();
                                if (check != null && check.Count > 1)
                                {
                                    List<string> lstExecuteRoleName = new List<string>();
                                    foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                    {
                                        lstExecuteRoleName.Add(Execute.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                    }
                                    string messLogin = String.Join(",", lstExecuteRoleName);

                                    mess += String.Format("Tài khoản {0} được thiết lập với các vai trò {1}.\n", check.FirstOrDefault().LOGINNAME, messLogin);
                                }
                            }

                            if (!string.IsNullOrEmpty(mess))
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        his_ekip_temp = new BackendAdapter(commonParam).Post<HIS_EKIP_TEMP>("api/HisEkipTemp/Update", ApiConsumers.MosConsumer, this.ekipTemp, commonParam);
                    }
                    else
                    {
                        dataSource = this.grdControlInformationSurg.DataSource as List<EkipTempUserADO>;
                        list2 = new List<HIS_EKIP_TEMP_USER>();
                        if ((dataSource != null) && (dataSource.Count > 0))
                        {
                            string messError = "";
                            foreach (EkipTempUserADO rado in dataSource)
                            {
                                if (!string.IsNullOrEmpty(rado.LOGINNAME))
                                {
                                    his_ekip_temp_user = new HIS_EKIP_TEMP_USER
                                    {
                                        EXECUTE_ROLE_ID = rado.EXECUTE_ROLE_ID,
                                        USERNAME = rado.USERNAME,
                                        LOGINNAME = rado.LOGINNAME
                                    };
                                    list2.Add(his_ekip_temp_user);
                                }
                                else
                                {
                                    messError += String.Format("vai trò {0} không có tên tài khoản.\n", Execute.FirstOrDefault(o => o.ID == rado.EXECUTE_ROLE_ID).EXECUTE_ROLE_NAME);
                                }
                            }
                            if (!string.IsNullOrEmpty(messError))
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(messError, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            this.ekipTemp.HIS_EKIP_TEMP_USER = list2;
                        }

                        if (this.ekipTemp.HIS_EKIP_TEMP_USER != null && this.ekipTemp.HIS_EKIP_TEMP_USER.Count > 0)
                        {
                            var lstLoginName = this.ekipTemp.HIS_EKIP_TEMP_USER.Select(o => o.LOGINNAME).Distinct().ToList();
                            foreach (var Login in lstLoginName)
                            {
                                var check = this.ekipTemp.HIS_EKIP_TEMP_USER.Where(o => o.LOGINNAME == Login).ToList();
                                if (check != null && check.Count > 1)
                                {
                                    List<string> lstExecuteRoleName = new List<string>();
                                    foreach (var item in check.Select(o => o.EXECUTE_ROLE_ID).ToList())
                                    {
                                        lstExecuteRoleName.Add(Execute.FirstOrDefault(o => o.ID == item).EXECUTE_ROLE_NAME);
                                    }
                                    string messLogin = String.Join(",", lstExecuteRoleName);

                                    mess += String.Format("Tài khoản {0} được thiết lập với các vai trò {1}.\n", check.FirstOrDefault().LOGINNAME, messLogin);
                                }
                            }

                            if (!string.IsNullOrEmpty(mess))
                            {
                                WaitingManager.Hide();
                                MessageBox.Show(mess, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                        }

                        his_ekip_temp = new BackendAdapter(commonParam).Post<HIS_EKIP_TEMP>("api/HisEkipTemp/Create", ApiConsumers.MosConsumer, this.ekipTemp, commonParam);
                    }
                    WaitingManager.Hide();
                    if (his_ekip_temp != null)
                    {
                        if (this.refeshData != null)
                        {
                            this.refeshData();
                        }
                        flag2 = true;
                        base.Close();
                    }
                    MessageManager.Show(this, commonParam, new bool?(flag2));
                }
            }
            catch (Exception exception)
            {
                WaitingManager.Hide();
                LogSystem.Warn(exception);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void grdViewInformationSurg_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                //EkipTempUserADO data = view.GetFocusedRow() as EkipTempUserADO;
                //if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                //{
                //    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;

                //    List<string> loginNames = new List<string>();
                //    if (data != null && data.EXECUTE_ROLE_ID > 0)
                //    {
                //        if (data.LOGINNAME != null)
                //            editor.EditValue = data.LOGINNAME;
                //        MOS.Filter.HisExecuteRoleUserFilter filter = new MOS.Filter.HisExecuteRoleUserFilter();
                //        filter.EXECUTE_ROLE_ID = data.EXECUTE_ROLE_ID;
                //        List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER> executeRoleUsers = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROLE_USER>>("api/HisExecuteRoleUser/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, new CommonParam());

                //        if (executeRoleUsers != null && executeRoleUsers.Count > 0)
                //        {
                //            loginNames = executeRoleUsers.Select(o => o.LOGINNAME).Distinct().ToList();
                //        }
                //    }
                //    ComboAcsUser(editor, loginNames);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "USERNAME")
                    {
                        try
                        {
                            string status = (view.GetRowCellValue(e.ListSourceRowIndex, "USERNAME") ?? "").ToString();
                            ACS.EFMODEL.DataModels.ACS_USER data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().SingleOrDefault(o => o.LOGINNAME == status);
                            e.Value = data.USERNAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi hien thi gia tri cot USERNAME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void grdViewInformationSurg_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            try
            {
                if (e.FocusedColumn.FieldName == "USERNAME")
                {
                    grdViewInformationSurg.ShowEditor();
                    ((GridLookUpEdit)grdViewInformationSurg.ActiveEditor).ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
