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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.HisPermissionCancelTrans
{
    public partial class frmHisPermissionCancelTrans : FormBase
    {
        private int rowCount = 0;
        private int dataTotal = 0;
        private int limit = 0;
        private int start = 0;

        private int positionHandle = -1;

        private HIS_PERMISSION currentData = null;

        private List<ACS_USER> listUser = null;

        public frmHisPermissionCancelTrans(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void frmHisPermissionCancelTrans_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControl();
                this.InitComboUser();
                this.SetDataToControl();
                this.FillDataToGridControl();
                this.InitComboPermissionType();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                this.ValidateGridLookupWithTextEdit(cboCashier, txtCashierLoginname, dxValidationProvider1);
                this.ValidationSingleControl(dtEffectiveDate, dxValidationProvider1);
                this.ValidCboPermissionType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitComboUser()
        {
            try
            {
                List<ACS_USER> datas = null;
                if (BackendDataWorker.IsExistsKey<ACS_USER>())
                {
                    datas = BackendDataWorker.Get<ACS_USER>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    datas = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<ACS_USER>>("api/AcsUser/Get", HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, filter, paramCommon);

                    if (datas != null) BackendDataWorker.UpdateToRam(typeof(ACS_USER), datas, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                HisUserRoomViewFilter uRfilter = new HisUserRoomViewFilter();
                uRfilter.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN;
                uRfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                List<V_HIS_USER_ROOM> userRooms = new BackendAdapter(new CommonParam()).Get<List<V_HIS_USER_ROOM>>("api/HisUserRoom/GetView", ApiConsumers.MosConsumer, uRfilter, null);
                this.listUser = datas != null ? datas.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (userRooms != null && userRooms.Any(a => a.LOGINNAME == o.LOGINNAME))).ToList() : null;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboCashier, this.listUser, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidCboPermissionType() {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = cboPermissionType;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cboPermissionType, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboPermissionType()
        {
            try
            {
                var listPermissionType = new List<ADO.PermissionTypeADO>();
                listPermissionType.Add(new ADO.PermissionTypeADO() { ID = 1, NAME = "Hủy/hoàn hủy giao dịch" });
                listPermissionType.Add(new ADO.PermissionTypeADO(){ID = 2, NAME = "Sửa thông tin giao dịch khác ngày"});

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 350, 1));

                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboPermissionType, listPermissionType, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void FillDataToGridControl()
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControlPermission);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                List<HIS_PERMISSION> listData = new List<HIS_PERMISSION>();
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisPermissionFilter filter = new HisPermissionFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //filter.PERMISSION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_PERMISSION_TYPE.ID__CANCEL_TRAN;

                if (!String.IsNullOrWhiteSpace(this.txtKeyword.Text))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }

                var rs = new BackendAdapter(paramCommon).GetRO<List<HIS_PERMISSION>>("api/HisPermission/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null)
                {
                    listData = rs.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }
                gridControlPermission.BeginUpdate();
                gridControlPermission.DataSource = listData;
                gridControlPermission.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToControl()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                if (this.currentData != null)
                {
                    btnAdd.Enabled = false;
                    btnEdit.Enabled = true;
                    cboCashier.EditValue = this.currentData.LOGINNAME;
                    txtCashierLoginname.Text = this.currentData.LOGINNAME;
                    dtEffectiveDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.currentData.EFFECTIVE_DATE);
                    cboPermissionType.EditValue = this.currentData.PERMISSION_TYPE_ID;
                }
                else
                {
                    btnAdd.Enabled = true;
                    btnEdit.Enabled = false;
                    cboCashier.EditValue = null;
                    txtCashierLoginname.Text = "";
                    dtEffectiveDate.EditValue = null;
                    cboPermissionType.EditValue = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void gridViewPermission_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    HIS_PERMISSION pData = (HIS_PERMISSION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "EFFECTIVE_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.EFFECTIVE_DATE);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPermission_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {

                    HIS_PERMISSION data = (HIS_PERMISSION)gridViewPermission.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "CHANGE_LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? repositoryItemBtn_Unlock : repositoryItemBtn_Lock);

                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            e.RepositoryItem = repositoryItemBtn_Delete_Enable;
                        else
                            e.RepositoryItem = repositoryItemBtn_Delete_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPermission_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (HIS_PERMISSION)gridViewPermission.GetFocusedRow();
                this.SetDataToControl();
                if (this.currentData != null)
                {
                    txtCashierLoginname.Focus();
                    txtCashierLoginname.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPermission_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.currentData = (HIS_PERMISSION)gridViewPermission.GetFocusedRow();
                    this.SetDataToControl();
                    if (this.currentData != null)
                    {
                        txtCashierLoginname.Focus();
                        txtCashierLoginname.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCashierLoginname_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtCashierLoginname.Text))
                    {
                        string key = txtCashierLoginname.Text.ToLower().Trim();
                        List<ACS_USER> lstData = this.listUser.Where(o => o.LOGINNAME.ToLower().Contains(key)).ToList();
                        if (lstData != null && lstData.Count == 1)
                        {
                            cboCashier.EditValue = lstData[0].LOGINNAME;
                        }
                        else
                        {
                            cboCashier.Focus();
                            cboCashier.ShowPopup();
                        }
                    }
                    else
                    {
                        cboCashier.Focus();
                        cboCashier.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashier_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //txtCashierLoginname.Text = "";
                //if (cboCashier.EditValue != null)
                //{
                //    ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboCashier.EditValue.ToString());
                //    if (user != null)
                //    {
                //        txtCashierLoginname.Text = user.LOGINNAME;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashier_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboCashier.EditValue = null;
                    txtCashierLoginname.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashier_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtCashierLoginname.Text = "";
                    if (cboCashier.EditValue != null)
                    {
                        ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboCashier.EditValue.ToString());
                        if (user != null)
                        {
                            txtCashierLoginname.Text = user.LOGINNAME;
                        }
                    }
                    dtEffectiveDate.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtEffectiveDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtEffectiveDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtn_Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_PERMISSION row = (HIS_PERMISSION)gridViewPermission.GetFocusedRow();
                if (row == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HIS_PERMISSION rs = new BackendAdapter(param).Post<HIS_PERMISSION>("api/HisPermission/ChangeLock", ApiConsumers.MosConsumer, row.ID, param);
                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void repositoryItemBtn_Unlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_PERMISSION row = (HIS_PERMISSION)gridViewPermission.GetFocusedRow();
                if (row == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;

                HIS_PERMISSION rs = new BackendAdapter(param).Post<HIS_PERMISSION>("api/HisPermission/ChangeLock", ApiConsumers.MosConsumer, row.ID, param);
                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void repositoryItemBtn_Delete_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HIS_PERMISSION row = (HIS_PERMISSION)gridViewPermission.GetFocusedRow();
                if (row == null)
                {
                    return;
                }
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("/api/HisPermission/Delete", ApiConsumers.MosConsumer, row.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                        btnRefresh_Click(null, null);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateControlToObject(HIS_PERMISSION data)
        {
            try
            {
                ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboCashier.EditValue.ToString());
                data.LOGINNAME = user != null ? user.LOGINNAME : null;
                data.USERNAME = user != null ? user.USERNAME : null;
                //data.PERMISSION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_PERMISSION_TYPE.ID__CANCEL_TRAN;
                data.EFFECTIVE_DATE = Convert.ToInt64(dtEffectiveDate.DateTime.ToString("yyyyMMdd") + "000000");
                data.PERMISSION_TYPE_ID = Convert.ToInt64(cboPermissionType.EditValue ?? 1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            } 

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnEdit.Enabled || !dxValidationProvider1.Validate() || this.currentData == null)
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                Mapper.CreateMap<HIS_PERMISSION, HIS_PERMISSION>();
                HIS_PERMISSION data = Mapper.Map<HIS_PERMISSION>(this.currentData);
                this.UpdateControlToObject(data);
                if (!CoincideCheck(data))
                {
                    return; 
                }

                HIS_PERMISSION rs = new BackendAdapter(param).Post<HIS_PERMISSION>("api/HisPermission/Update", ApiConsumers.MosConsumer, data, param);

                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled || !dxValidationProvider1.Validate())
                {
                    return;
                }

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_PERMISSION data = new HIS_PERMISSION();
                this.UpdateControlToObject(data);
                if (!CoincideCheck(data))
                {
                    return;
                }

                HIS_PERMISSION rs = new BackendAdapter(param).Post<HIS_PERMISSION>("api/HisPermission/Create", ApiConsumers.MosConsumer, data, param);
                
                if (rs != null)
                {
                    success = true;
                    FillDataToGridControl();
                    btnRefresh_Click(null, null);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled) return;
                WaitingManager.Show();
                this.currentData = null;
                this.SetDataToControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                this.currentData = null;
                this.SetDataToControl();
                this.FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashier_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCashier.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME.ToLower() == cboCashier.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtCashierLoginname.Text = data.LOGINNAME;
                            cboCashier.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
                else 
                {
                    cboCashier.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CoincideCheck(HIS_PERMISSION data)
        {
            try
            {
                List<HIS_PERMISSION> listData = new List<HIS_PERMISSION>();
                CommonParam paramCommon = new CommonParam();

                listData = new BackendAdapter(paramCommon).Get<List<HIS_PERMISSION>>("api/HisPermission/Get", ApiConsumers.MosConsumer, new HisPermissionFilter(), paramCommon);
                string type1 = "Hủy/hoàn hủy giao dịch";
                string type2 = "Sửa thông tin giao dịch khác ngày";

                if (listData != null)
                {
                    foreach (var item in listData)
                    {
                        if (data.LOGINNAME == item.LOGINNAME && data.EFFECTIVE_DATE == item.EFFECTIVE_DATE && data.PERMISSION_TYPE_ID == item.PERMISSION_TYPE_ID)
                        {
                            WaitingManager.Hide();
                            XtraMessageBox.Show(String.Format("Tài khoản {0} đã được thiết lập ngày {1} với loại {2}", data.LOGINNAME, ConvertLongToDateString(data.EFFECTIVE_DATE), data.PERMISSION_TYPE_ID==1? type1 : type2),"Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }
        public static string ConvertLongToDateString(long rawDateTime)
        {
            try
            {
                string rawString = rawDateTime.ToString(); 
                DateTime date = DateTime.ParseExact(rawString, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
                return date.ToString("dd/MM/yyyy");
            }
            catch
            {
                return string.Empty; 
            }
        }

    }
}
