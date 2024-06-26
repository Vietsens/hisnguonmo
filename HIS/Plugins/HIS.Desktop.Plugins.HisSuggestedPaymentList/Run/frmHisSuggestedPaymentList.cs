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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisSuggestedPaymentList.Run
{
    public partial class frmHisSuggestedPaymentList : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;

        
        private V_HIS_IMP_MEST_PROPOSE HisImpMestPropose;
        List<HIS_SUPPLIER> listSupplier = new List<HIS_SUPPLIER>();
        public frmHisSuggestedPaymentList()
        {
            InitializeComponent();
        }

        public frmHisSuggestedPaymentList(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisSuggestedPaymentList_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                dtFromTime.EditValue = null;
                dtToTime.EditValue = DateTime.Now;
                cboCtyHoaDonXuat.EditValue = null;
                cboNhaCungCap.EditValue = null;
                txtCtyXuatHoaDon.Text = null;
                txtKeyword.Text = null;
                txtProposeCode.Text = null;

                GetSupplier();
                LoadDataToCboCtyHoaDonXuat();
                LoadDataToCboNhaCungCap();
                LoadDataList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataList()
        {
            try
            {
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                ucPagingData(new CommonParam(0, (int)pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(ucPagingData, param, (int)pageSize, gridControls);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ucPagingData(object param)
        {
            try
            {
                WaitingManager.Show();
                gridControls.DataSource = null;

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                MOS.Filter.HisImpMestProposeViewFilter _Filter = new MOS.Filter.HisImpMestProposeViewFilter();
                if (this.currentModule != null && this.currentModule.RoomId > 0)
                {
                    _Filter.PROPOSE_ROOM_ID = this.currentModule.RoomId;
                }
                else
                {
                    return;
                }
                _Filter.ORDER_FIELD = "CREATE_TIME";
                _Filter.ORDER_DIRECTION = "DESC";
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    _Filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    _Filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (!string.IsNullOrEmpty(txtProposeCode.Text))
                {
                    string code = txtProposeCode.Text.Trim();
                    if (code.Length < 8)
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                        txtProposeCode.Text = code;
                    }
                    _Filter.IMP_MEST_PROPOSE_CODE__EXACT = code;
                }

                if (cboNhaCungCap.EditValue != null)
                {
                    _Filter.SUPPLIER_ID = (long)cboNhaCungCap.EditValue;
                }
                if (cboCtyHoaDonXuat.EditValue != null)
                {
                   _Filter.DOCUMENT_SUPPLIER_ID = (long)cboCtyHoaDonXuat.EditValue;
                }
                _Filter.KEY_WORD = txtKeyword.Text != "" ? txtKeyword.Text : null;
                List<V_HIS_IMP_MEST_PROPOSE> vHisSuggestedPaymentList = new List<V_HIS_IMP_MEST_PROPOSE>();
                var result = new BackendAdapter(paramCommon).GetRO<List<V_HIS_IMP_MEST_PROPOSE>>("api/HisImpMestPropose/GetView", ApiConsumers.MosConsumer, _Filter, paramCommon);
                if (result != null)
                {
                    vHisSuggestedPaymentList = (List<V_HIS_IMP_MEST_PROPOSE>)result.Data;
                    rowCount = (vHisSuggestedPaymentList == null ? 0 : vHisSuggestedPaymentList.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }

                gridControls.BeginUpdate();
                gridControls.DataSource = vHisSuggestedPaymentList;
                gridControls.EndUpdate();
                gridViews.OptionsSelection.EnableAppearanceFocusedCell = false;
                gridViews.OptionsSelection.EnableAppearanceFocusedRow = false;
                //gridViewTrackings.BestFitColumns();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViews_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_IMP_MEST_PROPOSE data = (V_HIS_IMP_MEST_PROPOSE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data == null)
                        return;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME.Value);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME.Value);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViews_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string creator = (gridViews.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    //long DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViews.GetRowCellValue(e.RowHandle, "DEPARTMENT_ID") ?? "0").ToString());
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (loginName == creator || CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Delete_Disable;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")
                    {
                        if (loginName == creator || CheckLoginAdmin.IsAdmin(loginName))
                        {
                            e.RepositoryItem = repositoryItemButton__Edit;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButton__Edit_D;
                        }
                    }
                    else if (e.Column.FieldName == "PRINT_DISPLAY")
                    {
                        e.RepositoryItem = repositoryItembtnEdit_Print;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViews_MouseDown(object sender, MouseEventArgs e)
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
                        long departmentId = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentId;
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        if (hi.Column.FieldName == "EDIT")
                        {
                            #region EDIT
                            var row = (V_HIS_IMP_MEST_PROPOSE)gridViews.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                if (creator.Trim() == row.CREATOR.Trim() || CheckLoginAdmin.IsAdmin(loginName))
                                {
                                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisSuggestedPayment").FirstOrDefault();
                                    if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisSuggestedPayment");
                                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                    {
                                        List<object> listArgs = new List<object>();
                                        listArgs.Add(row);
                                        listArgs.Add((int)2);
                                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                        ((Form)extenceInstance).ShowDialog();

                                    }
                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "VIEW")
                        {
                            #region View
                            var row = (V_HIS_IMP_MEST_PROPOSE)gridViews.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisSuggestedPayment").FirstOrDefault();
                                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisSuggestedPayment");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(row);
                                    listArgs.Add((int)3);
                                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                                    ((Form)extenceInstance).ShowDialog();

                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "DELETE")
                        {
                            #region DELETE
                            var row = (V_HIS_IMP_MEST_PROPOSE)gridViews.GetRow(hi.RowHandle);
                            if (row != null)
                            {
                                var creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                if (creator.Trim() == row.CREATOR.Trim() || CheckLoginAdmin.IsAdmin(loginName))
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                                    MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        bool success = false;
                                        CommonParam param = new CommonParam();
                                        HisImpMestProposeDeleteSDO sdo = new HisImpMestProposeDeleteSDO();
                                        sdo.Id = row.ID;
                                        sdo.WorkingRoomId = this.currentModule.RoomId;
                                        var rs = new BackendAdapter(param).Post<bool>("api/HisImpMestPropose/Delete", ApiConsumers.MosConsumer, sdo, param);
                                        if (rs)
                                        {
                                            success = true;
                                            //Load lại tracking
                                            LoadDataList();
                                        }
                                        MessageManager.Show(this.ParentForm, param, success);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisSuggestedPayment").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.HisSuggestedPayment");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add((DelegateRefreshData)ReLoad);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReLoad()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__Search_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButton__New_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtProposeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNhaCungCap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtNhaCungCap.Text.Trim()))
                    {
                        string code = txtNhaCungCap.Text.Trim().ToLower();
                        if (this.listSupplier != null && this.listSupplier.Count > 0)
                        {
                            var listData = this.listSupplier.Where(o => o.SUPPLIER_CODE.ToLower().Contains(code) && o.IS_ACTIVE == 1).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SUPPLIER_CODE.ToLower() == code).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                txtNhaCungCap.Text = result.First().SUPPLIER_CODE;
                                cboNhaCungCap.EditValue = result.First().ID;
                                SendKeys.Send("{TAB}");
                                SendKeys.Send("{TAB}");
                            }
                        }
                    }
                    if (showCbo)
                    {
                        cboNhaCungCap.Focus();
                        cboNhaCungCap.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNhaCungCap_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboNhaCungCap.EditValue != null)
                    {
                        if (this.listSupplier != null && this.listSupplier.Count > 0)
                        {
                            var data = this.listSupplier.FirstOrDefault(o => o.SUPPLIER_CODE.ToLower() == cboNhaCungCap.EditValue.ToString().ToLower() && o.IS_ACTIVE == 1);
                            if (data != null)
                            {
                                txtNhaCungCap.Text = data.SUPPLIER_CODE;
                                cboNhaCungCap.Properties.Buttons[1].Visible = true;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboNhaCungCap.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboNhaCungCap_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboNhaCungCap.EditValue != null)
                    {
                        if (this.listSupplier != null && this.listSupplier.Count > 0)
                        {
                            var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get
                                <MOS.EFMODEL.DataModels.HIS_SUPPLIER>().FirstOrDefault
                                (o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboNhaCungCap.EditValue ?? 0).ToString()) && o.IS_ACTIVE == 1);
                            if (data != null)
                            {
                                txtNhaCungCap.Text = data.SUPPLIER_CODE;
                                cboNhaCungCap.Properties.Buttons[1].Visible = true;
                                SendKeys.Send("{TAB}");
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

        private void cboNhaCungCap_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtNhaCungCap.Text = null;
                    cboNhaCungCap.EditValue = null;
                    txtNhaCungCap.Focus();
                    cboNhaCungCap.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCtyXuatHoaDon_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtCtyXuatHoaDon.Text.Trim()))
                    {
                        string code = txtCtyXuatHoaDon.Text.Trim().ToLower();
                        if (this.listSupplier != null && this.listSupplier.Count > 0)
                        {
                            var listData = this.listSupplier.Where(o => o.SUPPLIER_CODE.ToLower().Contains(code)).ToList();
                            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.SUPPLIER_CODE.ToLower() == code).ToList() : listData) : null;
                            if (result != null && result.Count > 0)
                            {
                                showCbo = false;
                                txtCtyXuatHoaDon.Text = result.First().SUPPLIER_CODE;
                                cboCtyHoaDonXuat.EditValue = result.First().ID;
                                SendKeys.Send("{TAB}");
                                SendKeys.Send("{TAB}");
                            }
                        }
                    }
                    if (showCbo)
                    {
                        cboCtyHoaDonXuat.Focus();
                        cboCtyHoaDonXuat.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCtyHoaDonXuat_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCtyHoaDonXuat.EditValue != null)
                    {
                        if (this.listSupplier != null && this.listSupplier.Count > 0)
                        {
                            var data = this.listSupplier.FirstOrDefault(o => o.SUPPLIER_CODE.ToLower() == cboCtyHoaDonXuat.EditValue.ToString().ToLower());
                            if (data != null)
                            {
                                txtCtyXuatHoaDon.Text = data.SUPPLIER_CODE;
                                cboCtyHoaDonXuat.Properties.Buttons[1].Visible = true;
                            }
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboCtyHoaDonXuat.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCtyHoaDonXuat_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboCtyHoaDonXuat.EditValue != null)
                    {
                        if (this.listSupplier != null && this.listSupplier.Count > 0)
                        {
                            var data = this.listSupplier.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCtyHoaDonXuat.EditValue ?? 0).ToString()));
                            if (data != null)
                            {
                                txtCtyXuatHoaDon.Text = data.SUPPLIER_CODE;
                                cboCtyHoaDonXuat.Properties.Buttons[1].Visible = true;
                                SendKeys.Send("{TAB}");
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

        private void cboCtyHoaDonXuat_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    txtCtyXuatHoaDon.Text = null;
                    cboCtyHoaDonXuat.EditValue = null;
                    txtCtyXuatHoaDon.Focus();
                    cboCtyHoaDonXuat.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCboCtyHoaDonXuat()
        {
            try
            {
                cboCtyHoaDonXuat.Properties.DataSource = listSupplier;
                cboCtyHoaDonXuat.Properties.DisplayMember = "SUPPLIER_NAME";
                cboCtyHoaDonXuat.Properties.ValueMember = "ID";
                cboCtyHoaDonXuat.Properties.TextEditStyle = TextEditStyles.Standard;
                cboCtyHoaDonXuat.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboCtyHoaDonXuat.Properties.ImmediatePopup = true;
                cboCtyHoaDonXuat.ForceInitialize();
                cboCtyHoaDonXuat.Properties.View.Columns.Clear();
                cboCtyHoaDonXuat.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboCtyHoaDonXuat.Properties.View.Columns.AddField("SUPPLIER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboCtyHoaDonXuat.Properties.View.Columns.AddField("SUPPLIER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCboNhaCungCap()
        {
            try
            {
                cboNhaCungCap.Properties.DataSource = listSupplier;
                cboNhaCungCap.Properties.DisplayMember = "SUPPLIER_NAME";
                cboNhaCungCap.Properties.ValueMember = "ID";
                cboNhaCungCap.Properties.TextEditStyle = TextEditStyles.Standard;
                cboNhaCungCap.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboNhaCungCap.Properties.ImmediatePopup = true;
                cboNhaCungCap.ForceInitialize();
                cboNhaCungCap.Properties.View.Columns.Clear();
                cboNhaCungCap.Properties.PopupFormSize = new Size(400, 250);

                GridColumn aColumnCode = cboNhaCungCap.Properties.View.Columns.AddField("SUPPLIER_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                GridColumn aColumnName = cboNhaCungCap.Properties.View.Columns.AddField("SUPPLIER_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SUPPLIER> GetSupplier()
        {
            try
            {
                listSupplier = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == 1).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            return listSupplier;
        }
        private void repositoryItembtnEdit_Print_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                HisImpMestPropose = new V_HIS_IMP_MEST_PROPOSE();
                HisImpMestPropose = (V_HIS_IMP_MEST_PROPOSE)gridViews.GetFocusedRow();
                PrintTypeCare type = new PrintTypeCare();
                type = PrintTypeCare.IN_PHIEU_DE_NGHI_THANH_TOAN_NCC;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintTypeCare
        {
            IN_PHIEU_DE_NGHI_THANH_TOAN_NCC,
        }

        void PrintProcess(PrintTypeCare printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeCare.IN_PHIEU_DE_NGHI_THANH_TOAN_NCC:
                        richEditorMain.RunPrintTemplate("Mps000327", DelegateRunPrinterCare);
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

        bool DelegateRunPrinterCare(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000327":
                        LoadInPhieuDeNghiThanhToanNcc(printTypeCode, fileName, ref result);
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

        private void LoadInPhieuDeNghiThanhToanNcc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                if (HisImpMestPropose != null)
                {
                    HisImpMestViewFilter initImpMestFilter = new HisImpMestViewFilter();
                    initImpMestFilter.IMP_MEST_PROPOSE_ID = HisImpMestPropose.ID;
                    var hisInitImpMests = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetView", ApiConsumers.MosConsumer, initImpMestFilter, null);

                    HisImpMestPayViewFilter initImpMestPayFilter = new HisImpMestPayViewFilter();
                    initImpMestPayFilter.IMP_MEST_PROPOSE_ID = HisImpMestPropose.ID;
                    var hisInitImpMestPays = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_PAY>>("api/HisImpMestPay/GetView", ApiConsumers.MosConsumer, initImpMestFilter, null);

                    MPS.Processor.Mps000327.PDO.Mps000327PDO mps000327PDO = new MPS.Processor.Mps000327.PDO.Mps000327PDO(HisImpMestPropose, hisInitImpMests, hisInitImpMestPays);
                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]);
                        }
                    }
                    else
                    {
                        if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000327PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
