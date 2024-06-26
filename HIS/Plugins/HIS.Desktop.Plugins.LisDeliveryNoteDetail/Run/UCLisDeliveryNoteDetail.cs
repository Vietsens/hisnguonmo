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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LIS.EFMODEL.DataModels;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.ADO;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.Config;

namespace HIS.Desktop.Plugins.LisDeliveryNoteDetail
{
    public partial class UCLisDeliveryNoteDetail : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.LisDeliveryNoteDetail";

        V_LIS_DELIVERY_NOTE deliveryNote;
        RefeshReference refeshReference;
        SampleADO currentSample = null;
        List<SampleADO> currentListSample = new List<SampleADO>();
        List<SampleADO> rootListSample = new List<SampleADO>();
        private Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_ROOM currentRoom = null;
        string roomCode;
        int positionHandle = -1;
        StateCheckBox statecheckColumn = StateCheckBox.Unchecked;
        enum StateCheckBox
        {
            Checked,
            Unchecked,
            Selected
        }
        #endregion

        #region Constructor
        public UCLisDeliveryNoteDetail(Inventec.Desktop.Common.Modules.Module _moduleData, V_LIS_DELIVERY_NOTE _deliveryNote, RefeshReference _refeshReference)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                this.deliveryNote = _deliveryNote;
                this.refeshReference = _refeshReference;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void UCLisDeliveryNoteDetail_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Public method
        public void MeShow()
        {
            try
            {
                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                InitControlState();

                //Gan gia tri mac dinh
                SetDefaultValue();
                SetDefaultProperties();

                //Load du lieu
                FillDataToGridControl_Sample();
                FillDataToLableControls_Info();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

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
                        if (item.KEY == ControlStateConstant.chkAutoFocusTo_txtKeyword)
                        {
                            chkAutoFocusTo_txtKeyword.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToLableControls_Info()
        {
            try
            {
                if (this.deliveryNote == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay du lieu phieu giao nhan!");
                    return;
                }

                lblDeliveryNoteCode.Text = this.deliveryNote.DELIVERY_NOTE_CODE;
                lblTotalSample.Text = this.currentListSample != null ? this.currentListSample.Count().ToString() : "0";
                lblSendRoomName.Text = this.deliveryNote.SEND_ROOM_NAME;
                lblReceiveRoomName.Text = this.deliveryNote.RECEIVE_ROOM_NAME;
                lblDeliverName.Text = this.deliveryNote.DELIVER_NAME;
                lblReceiverName.Text = this.deliveryNote.RECEIVER_NAME;
                lblNote.Text = this.deliveryNote.NOTE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gán giá trị mặc định
        /// </summary>
        private void SetDefaultValue()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultProperties()
        {
            try
            {
                gridColumn_Select.Visible = false;
                gridColumn_Approve.Visible = false;
                gridColumn_RejectApprove.Visible = false;

                this.currentRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                if (currentRoom != null)
                    this.roomCode = this.currentRoom.ROOM_CODE;
                if (this.deliveryNote.RECEIVE_ROOM_CODE == roomCode)
                {
                    gridColumn_Select.VisibleIndex = 1;
                    gridColumn_Approve.VisibleIndex = 2;
                    gridColumn_RejectApprove.VisibleIndex = 3;
                    gridColumn_Select.AbsoluteIndex = 1;
                    gridColumn_Approve.AbsoluteIndex = 2;
                    gridColumn_RejectApprove.AbsoluteIndex = 3;
                }
                this.statecheckColumn = StateCheckBox.Unchecked;
                SetGridColumn_Select_Image(statecheckColumn);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (SampleADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    SampleADO data = (SampleADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Approve_Sample")
                    {
                        if (this.deliveryNote.RECEIVE_ROOM_CODE == roomCode && data.SAMPLE_STT_ID == 2)
                            e.RepositoryItem = repositoryItemButtonEdit_Approve;
                        else if (this.deliveryNote.RECEIVE_ROOM_CODE == roomCode && data.SAMPLE_STT_ID == 5)
                            e.RepositoryItem = repositoryItemButtonEdit_CancelApprove;
                        else
                            e.RepositoryItem = repositoryItemButtonEdit_Approve_Disable;

                    }

                    if (e.Column.FieldName == "RejectApprove_Sample")
                    {
                        if (this.deliveryNote.RECEIVE_ROOM_CODE == roomCode && data.SAMPLE_STT_ID == 2)
                            e.RepositoryItem = repositoryItemButtonEdit_RejectApprove;
                        else if (this.deliveryNote.RECEIVE_ROOM_CODE == roomCode && data.SAMPLE_STT_ID == 6)
                            e.RepositoryItem = repositoryItemButtonEdit_CancelRejectApprove;
                        else
                            e.RepositoryItem = repositoryItemButtonEdit_RejectApprove_Disable;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_Approve_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    ProcessApprove(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_CancelApprove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    ProcessCancelApprove(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_RejectApprove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    frmRejectReason frmRejectReason = new frmRejectReason(moduleData ,ProcessRejectApprove, row);
                    frmRejectReason.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit_CancelRejectApprove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (SampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    ProcessCancelRejectApprove(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEditChoose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                //if (this.listSample == null || this.statecheckColumn == null)
                //    return;
                //var listChecked = this.listSample.Where(o => o.IsChecked).ToList();
                //if (listChecked.Count() == 0)
                //{
                //    this.statecheckColumn = StateCheckBox.Unchecked;
                //}
                //else if (listChecked.Count() < this.listSample.Count())
                //{
                //    this.statecheckColumn = StateCheckBox.Selected;
                //}
                //else if (listChecked.Count() == this.listSample.Count())
                //{
                //    this.statecheckColumn = StateCheckBox.Selected;
                //}

                //SetGridColumn_Select_Image(statecheckColumn);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region CheckBox Mutiselect
        private void gridViewSample_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InColumnPanel)
                    {
                        if (hi.Column.FieldName == "IsChecked")
                        {
                            if (statecheckColumn == StateCheckBox.Unchecked)
                            {
                                statecheckColumn = StateCheckBox.Checked;
                            }
                            else if (statecheckColumn == StateCheckBox.Checked)
                            {
                                statecheckColumn = StateCheckBox.Unchecked;
                            }
                            else if (statecheckColumn == StateCheckBox.Selected)
                            {
                                statecheckColumn = StateCheckBox.Checked;
                            }

                            SetGridColumn_Select_Image(statecheckColumn);

                            if (statecheckColumn == StateCheckBox.Checked)
                                GridCheckChange(true);
                            else if (statecheckColumn == StateCheckBox.Unchecked)
                                GridCheckChange(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetGridColumn_Select_Image(StateCheckBox state)
        {
            try
            {
                this.gridColumn_Select.ImageAlignment = StringAlignment.Center;
                switch (state)
                {
                    case StateCheckBox.Checked:
                        this.gridColumn_Select.Image = this.imageListCheck.Images[3];
                        break;
                    case StateCheckBox.Unchecked:
                        this.gridColumn_Select.Image = this.imageListCheck.Images[4];
                        break;
                    case StateCheckBox.Selected:
                        this.gridColumn_Select.Image = this.imageListCheck.Images[5];
                        break;
                    default:
                        this.gridColumn_Select.Image = null;
                        break;
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.currentListSample)
                {
                    item.IsChecked = checkedAll;
                }
                gridControlSample.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FilldataToGridControl_Sample_Searching();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.gridViewSample.FocusedRowHandle = 0;
                    this.gridViewSample.Focus();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewSample.FocusedRowHandle = 0;
                    this.gridViewSample.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridView view = (GridView)sender;
                GridHitInfo hitInfo = e.HitInfo;
                if (hitInfo.InRowCell)
                {
                    int visibleRowHandle = this.gridViewSample.GetVisibleRowHandle(hitInfo.RowHandle);
                    
                    var selectedSample = GetSampleSelected();
                    if (selectedSample == null || selectedSample.Count() == 0)
                        return;

                    int[] selectedRows = new int[selectedSample.Count()];
                    for (int i = 0; i < selectedSample.Count(); i++)
                    {
                        selectedRows[i] = FindRowHandleByRowObject(view, selectedSample[i]);
                    }
                    if (selectedRows != null && selectedRows.Length > 0 && selectedRows.Contains(visibleRowHandle))
                    {

                        InitMenu();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private int FindRowHandleByRowObject(DevExpress.XtraGrid.Views.Grid.GridView view, object row)
        {

            if (row != null)

                for (int i = 0; i < view.DataRowCount; i++)

                    if (row.Equals(view.GetRow(i)))

                        return i;

            return DevExpress.XtraGrid.GridControl.InvalidRowHandle;

        }

        private void chkAutoFocusTo_txtKeyword_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkAutoFocusTo_txtKeyword && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoFocusTo_txtKeyword.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkAutoFocusTo_txtKeyword;
                    csAddOrUpdate.VALUE = (chkAutoFocusTo_txtKeyword.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnApprove_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("bbtnApprove_ItemClick");
                var row = (SampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    ProcessApprove(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReject_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("bbtnReject_ItemClick");
                var row = (SampleADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    frmRejectReason frmRejectReason = new frmRejectReason(moduleData, ProcessRejectApprove, row);
                    frmRejectReason.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
