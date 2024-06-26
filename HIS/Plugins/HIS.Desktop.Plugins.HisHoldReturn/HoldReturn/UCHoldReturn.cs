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
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisHoldReturn.Config;
using HIS.Desktop.Plugins.HisHoldReturn.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Core;
using System.Threading;
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.PopupLoader;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.HisHoldReturn.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HisHoldReturn.HoldReturn
{
    public partial class UCHoldReturn : UserControlBase
    {
        #region Variables
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_ROOM requestRoom;
        HIS_DEPARTMENT currentDepartment = null;

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        int pageSize = 0;
        int positionHandleControl = -1;
        int actionType = 0;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        const string START_TIME = "000000";
        const string END_TIME = "235959";
        HoldReturnDataADO currentHoldReturn;
        List<HIS_DOC_HOLD_TYPE> currentDocHoldTypeSelecteds;
        HIS_TREATMENT currentTreatment;
        long currentTreatmentId;
        long currentPatientId;
        HoldReturnADO currentHoldReturnADO;
        #endregion

        #region Construct - OnLoad
        //Bổ sung thêm cho truyền vào thông tịn khởi tạo để hiển thị luôn khi mở từ chức năng khác(tiếp đón)
        public UCHoldReturn(Inventec.Desktop.Common.Modules.Module currentModule, HoldReturnADO holdReturnADO)
            : base(currentModule)
        {
            try
            {
                InitializeComponent();
                this.currentModule = currentModule;
                this.currentHoldReturnADO = holdReturnADO;
                this.actionType = GlobalVariables.ActionAdd;
                this.gridControlHoldReturn.ToolTipController = this.tooltipServiceRequest;
                if (this.currentHoldReturnADO != null)
                {
                    this.currentTreatment = this.currentHoldReturnADO.Treatment;
                    this.currentDocHoldTypeSelecteds = this.currentHoldReturnADO.DocHoldType;
                    this.currentTreatmentId = this.currentTreatment.ID;
                    this.currentPatientId = this.currentTreatment.PATIENT_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCKidneyShift_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                this.SetDefaultData();
                this.requestRoom = GetRequestRoom(this.currentModule.RoomId);
                this.FillDataToControlsForm();
                this.FillDataToGridHoldReturn();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Private method
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisHoldReturn.Resources.Lang", typeof(HIS.Desktop.Plugins.HisHoldReturn.HoldReturn.UCHoldReturn).Assembly);


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultData()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisHoldReturn.Resources.Lang", typeof(HIS.Desktop.Plugins.HisHoldReturn.HoldReturn.UCHoldReturn).Assembly);
                HisConfigCFG.LoadConfig();
                this.actionType = GlobalVariables.ActionAdd;
                ButtonEdit_DelDisable.Enabled = false;
                ButtonEdit_DelDisable.ReadOnly = true;
                this.dateHoldTime.EditValue = null;
                this.dateHoldTime.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private V_HIS_ROOM GetRequestRoom(long requestRoomId)
        {
            V_HIS_ROOM result = new V_HIS_ROOM();
            try
            {
                if (requestRoomId > 0)
                {
                    result = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == requestRoomId);
                    this.currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == result.DEPARTMENT_ID);
                }
            }
            catch (Exception ex)
            {
                result = new V_HIS_ROOM();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #region Button clicks
        private void btnSearchForHoldReturn_Click(object sender, EventArgs e)
        {
            this.FillDataToGridHoldReturn();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            ProcessAddClick();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.EnableButtonByData(true);
                this.ResetStateControlForm();
                this.txtIFTreatmentCode.Focus();
                this.txtIFTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void tooltipServiceRequest_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlHoldReturn)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlHoldReturn.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "IconHandover")
                            {
                                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                                short sIS_HANDOVERING = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewHoldReturn.GetRowCellValue(lastRowHandle, "IS_HANDOVERING") ?? "0").ToString());
                                long lHOLD_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewHoldReturn.GetRowCellValue(lastRowHandle, "HOLD_ROOM_ID") ?? "0").ToString());
                                long lRESPONSIBLE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewHoldReturn.GetRowCellValue(lastRowHandle, "RESPONSIBLE_ROOM_ID") ?? "0").ToString());
                                long lRETURN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewHoldReturn.GetRowCellValue(lastRowHandle, "RETURN_TIME") ?? "0").ToString());
                                string cREATOR = (gridViewHoldReturn.GetRowCellValue(lastRowHandle, "CREATOR") ?? "").ToString();
                                if (lRETURN_TIME > 0)
                                {
                                    text = "Đã trả";
                                }
                                else
                                {
                                    text = sIS_HANDOVERING == GlobalVariables.CommonNumberTrue ? "Đã bàn giao" : "Chưa bàn giao";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlHoldReturn_Click(object sender, EventArgs e)
        {
            try
            {
                this.HoldReturnRowClick();

                this.gridViewDocType.Focus();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHoldReturn_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource) != null && ((IList)((BaseView)sender).DataSource).Count > 0)
                    {
                        HoldReturnDataADO oneServiceSDO = (HoldReturnDataADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (oneServiceSDO != null)
                        {
                            if (e.Column.FieldName == "STT")
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (((ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.CurrentPage) - 1) * (ucPaging1.pagingGrid == null ? 0 : ucPaging1.pagingGrid.PageSize));
                            }
                            if (e.Column.FieldName == "TDL_PATIENT_DOB_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.TDL_PATIENT_DOB);
                            }
                            if (e.Column.FieldName == "HOLD_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.HOLD_TIME);
                            }
                            if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.CREATE_TIME ?? 0);
                            }
                            if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(oneServiceSDO.MODIFY_TIME ?? 0);
                            }
                           
                            if (e.Column.FieldName == "HOLD_USERNAME_DISPLAY")
                            {
                                e.Value = String.Format("{0}{1}", oneServiceSDO.HOLD_LOGINNAME, String.IsNullOrEmpty(oneServiceSDO.HOLD_USERNAME) ? "" : " - " + oneServiceSDO.HOLD_USERNAME);
                            }
                            if (e.Column.FieldName == "IconHandover")
                            {
                                //Chua ban giao: mau trang = 0
                                //Da ban giao: mau den  = 4
                                //Chua tra: yellow = 1
                                //Da tra: orange = 2

                                if (oneServiceSDO.RETURN_TIME == null)
                                {
                                    if (oneServiceSDO.IS_HANDOVERING == GlobalVariables.CommonNumberTrue)
                                    {
                                        e.Value = imageListIcon.Images[0];
                                    }
                                    else
                                    {
                                        e.Value = imageListIcon.Images[4];
                                    }
                                }
                                else
                                {
                                    e.Value = imageListIcon.Images[2];
                                }
                            }
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHoldReturn_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "BtnDelete")
                    {
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        short sIS_HANDOVERING = Inventec.Common.TypeConvert.Parse.ToInt16((gridViewHoldReturn.GetRowCellValue(e.RowHandle, "IS_HANDOVERING") ?? "0").ToString());
                        long lHOLD_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewHoldReturn.GetRowCellValue(e.RowHandle, "HOLD_ROOM_ID") ?? "0").ToString());
                        long lRESPONSIBLE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewHoldReturn.GetRowCellValue(e.RowHandle, "RESPONSIBLE_ROOM_ID") ?? "0").ToString());
                        string cREATOR = (gridViewHoldReturn.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                        if ((sIS_HANDOVERING == 0 || sIS_HANDOVERING != 1) && lHOLD_ROOM_ID == currentModule.RoomId && lRESPONSIBLE_ROOM_ID == currentModule.RoomId && cREATOR == loginName)
                            e.RepositoryItem = this.ButtonEdit_DelEnable;
                        else
                        {
                            e.RepositoryItem = this.repositoryItemTextEditReadOnly;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHoldReturn_ShowingEditor(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                HoldReturnDataADO data = view.GetFocusedRow() as HoldReturnDataADO;
                if (data == null) return;

                if (view.FocusedColumn.FieldName == "BtnDelete")
                {
                    if ((data.IS_HANDOVERING == null || data.IS_HANDOVERING != GlobalVariables.CommonNumberTrue) && data.HOLD_ROOM_ID == currentModule.RoomId && data.RESPONSIBLE_ROOM_ID == currentModule.RoomId && data.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                    {

                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDocType_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                this.currentDocHoldTypeSelecteds = new List<HIS_DOC_HOLD_TYPE>();
                int[] rows = gridViewDocType.GetSelectedRows();
                for (int i = 0; i < rows.Length; i++)
                {
                    this.currentDocHoldTypeSelecteds.Add((MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE)gridViewDocType.GetRow(rows[i]));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_DelEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ButtonEdit_DelEnable_ButtonClick.1");
                var serviceReqADO = (HoldReturnDataADO)this.gridViewHoldReturn.GetFocusedRow();
                if (serviceReqADO != null)
                {
                    if (MessageBox.Show(ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.CanhBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        if ((serviceReqADO.IS_HANDOVERING == null || serviceReqADO.IS_HANDOVERING != GlobalVariables.CommonNumberTrue) && serviceReqADO.HOLD_ROOM_ID == currentModule.RoomId && serviceReqADO.RESPONSIBLE_ROOM_ID == currentModule.RoomId && serviceReqADO.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                        {
                            WaitingManager.Show();
                            success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(RequestUriStore.HIS_HOLD_RETURN_DELETE, ApiConsumers.MosConsumer, serviceReqADO.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            WaitingManager.Hide();
                            if (success)
                            {
                                this.FillDataToGridHoldReturn();
                            }

                            #region Show message
                            MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                        else
                        {
                            MessageManager.Show(ResourceMessage.HeThongThongBaoBanKhongCoQuyenHuyDuLieuNay);
                            Inventec.Common.Logging.LogSystem.Warn("Ban ghi giu giay to khong o trang thai chua xu ly hoac tai khoan thao tac khong phai tai khoan tao du lieu, khong the huy bo yeu cau");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeywordForSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.FillDataToGridHoldReturn();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCodeForAdd_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (!String.IsNullOrEmpty(strValue))
                    {
                        if (strValue.Length > 10 && strValue.Contains("|"))
                        {
                            //Quet qrcode hoac nhap qrcode va nhan enter
                            this.ProcessForGetDataQrCodeHeinCard(strValue);
                        }
                        else
                        {
                            //Nhap ma BN va nhan enter
                            WaitingManager.Show();
                            string str = string.Format("{0:0000000000}", Convert.ToInt64(strValue));
                            this.txtPatientCodeForAdd.Text = str;

                            HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                            treatmentFilter.ORDER_DIRECTION = "DESC";
                            treatmentFilter.ORDER_FIELD = "IN_TIME";
                            treatmentFilter.TDL_PATIENT_CODE__EXACT = this.txtPatientCodeForAdd.Text;
                            CommonParam paramCommon = new CommonParam();
                            paramCommon.Limit = 1;
                            paramCommon.Start = 0;
                            this.currentTreatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, paramCommon).FirstOrDefault();

                            if (this.currentTreatment != null)
                            {
                                this.currentTreatmentId = this.currentTreatment.ID;
                                this.currentPatientId = this.currentTreatment.PATIENT_ID;
                                this.txtIFTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE;
                                this.txtPatientCodeForAdd.Text = this.currentTreatment.TDL_PATIENT_CODE;
                                this.lblPatientName.Text = this.currentTreatment.TDL_PATIENT_NAME;
                                this.lblGenderName.Text = this.currentTreatment.TDL_PATIENT_GENDER_NAME;
                                this.lblPatientAddress.Text = this.currentTreatment.TDL_PATIENT_ADDRESS;
                                this.lblHeinCardNumber.Text = this.currentTreatment.TDL_HEIN_CARD_NUMBER;
                                this.lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.TDL_PATIENT_DOB);


                                HisHoldReturnViewFilter holdReturnFilter = new HisHoldReturnViewFilter();
                                holdReturnFilter.PATIENT_ID = this.currentPatientId;
                                //holdReturnFilter.HOLD_ROOM_ID = this.currentModule.RoomId;
                                //holdReturnFilter.RESPONSIBLE_ROOM_ID = this.currentModule.RoomId;
                                holdReturnFilter.IS_HANDOVERING = false;//TODO
                                paramCommon = new CommonParam();
                                var holdReturns = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_HOLD_RETURN>>(RequestUriStore.HIS_HOLD_RETURN_GETVIEW, ApiConsumers.MosConsumer, holdReturnFilter, paramCommon);

                                if (holdReturns != null && holdReturns.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => holdReturns), holdReturns));
                                    var roomHold = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == holdReturns[0].HOLD_ROOM_ID).FirstOrDefault();
                                    this.lblHandoverRoom.Text = roomHold != null ? roomHold.ROOM_NAME : "";
                                   
                                    if (holdReturns[0].RETURN_TIME != null)
                                    {
                                        this.btnSave.Enabled = true;
                                    }
                                    else
                                    {
                                        this.btnSave.Enabled = false;
                                    }
                                }
                                else
                                {
                                    if (holdReturns[0].RETURN_TIME != null)
                                    {
                                        this.btnSave.Enabled = true;
                                    }
                                    else
                                    {
                                        this.btnSave.Enabled = false;
                                    }
                                   
                                    this.gridViewDocType.Focus();
                                }
                            }
                            else
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Không tồn tại dữ liệu", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            }
                            WaitingManager.Hide();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocHoldTypeForSearch_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboDocHoldTypeForSearch.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboDocHoldTypeForSearch.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            //this.txtDocHoldTypeForSearch.Text = data.DOC_HOLD_TYPE_CODE;
                            cboDocHoldTypeForSearch.Properties.Buttons[1].Visible = true;
                        }
                    }
                    cboHandoverForSearch.Focus();
                    cboHandoverForSearch.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocHoldTypeForSearch_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboDocHoldTypeForSearch.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE>().Where(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((this.cboDocHoldTypeForSearch.EditValue ?? "0").ToString())).FirstOrDefault();
                        if (data != null)
                        {
                            // this.txtDocHoldTypeForSearch.Text = data.DOC_HOLD_TYPE_CODE;
                            cboDocHoldTypeForSearch.Properties.Buttons[1].Visible = true;

                            cboHandoverForSearch.Focus();
                            cboHandoverForSearch.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDocHoldTypeForSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //if (e.KeyCode == Keys.Enter)
                //{
                //    bool showCombo = true;
                // //   string searchCode = txtDocHoldTypeForSearch.Text;
                //    if (!String.IsNullOrEmpty(searchCode))
                //    {
                //        var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DOC_HOLD_TYPE>().Where(o => o.DOC_HOLD_TYPE_CODE.ToLower().Contains(searchCode.ToLower())).ToList();
                //        var result = data != null ? (data.Count > 1 ? data.Where(o => o.DOC_HOLD_TYPE_CODE.ToLower() == searchCode.ToLower()).ToList() : data) : null;
                //        if (result != null && result.Count > 0)
                //        {
                //            showCombo = false;
                //            cboDocHoldTypeForSearch.Properties.Buttons[1].Visible = true;
                //            cboDocHoldTypeForSearch.EditValue = result.First().ID;
                //          //  txtDocHoldTypeForSearch.Text = result.First().DOC_HOLD_TYPE_CODE;

                //            cboHandoverForSearch.Focus();
                //            cboHandoverForSearch.SelectAll();
                //            e.Handled = true;
                //        }
                //    }
                //    if (showCombo)
                //    {
                //        cboDocHoldTypeForSearch.Properties.Buttons[1].Visible = false;
                //        cboDocHoldTypeForSearch.EditValue = null;
                //        cboDocHoldTypeForSearch.Focus();
                //        cboDocHoldTypeForSearch.ShowPopup();
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocHoldTypeForSearch_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDocHoldTypeForSearch.Properties.Buttons[1].Visible = false;
                    cboDocHoldTypeForSearch.EditValue = null;
                    //txtDocHoldTypeForSearch.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientCodeForSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.FillDataToGridHoldReturn();
            }
        }

        private void EnableButtonByData(bool isEnabled)
        {
            try
            {
                btnSave.Enabled = isEnabled;
                txtIFTreatmentCode.Enabled = isEnabled;
                txtPatientCodeForAdd.Enabled = isEnabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(GridLookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusShowpopup(LookUpEdit cboEditor, bool isSelectFirstRow)
        {
            try
            {
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRow)
                    PopupLoader.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public Method
        public void SaveHoldReturnShortcut()
        {
            if (btnSave.Enabled)
                ProcessAddClick();
        }

        public void SearchHoldReturnShortcut()
        {
            if (btnSearch.Enabled)
                this.FillDataToGridHoldReturn();
        }

        public void NewHoldReturnShortcut()
        {
            if (btnNew.Enabled)
                this.btnNew_Click(null, null);
        }
        #endregion

        private void txtMaDieuTri_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                
                if (e.KeyCode == Keys.Enter)
                {
                    
                     //   this.FillDataToGridHoldReturn();
                        //string codeTreatment = txtTreatmentCode.Text.Trim();
                        //if (codeTreatment.Length < 12)
                        //{
                        //    codeTreatment = string.Format("{0:000000000000}", Convert.ToInt64(codeTreatment));
                        //    txtTreatmentCode.Text = codeTreatment;
                        //}
                        btnSearchForHoldReturn_Click(null, null);
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtIFTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    if (!String.IsNullOrEmpty(strValue))
                    {
                        if (strValue.Length > 10 && strValue.Contains("|"))
                        {
                            //Quet qrcode hoac nhap qrcode va nhan enter
                            this.ProcessForGetDataQrCodeHeinCard(strValue);
                        }
                        else
                        {
                            //Nhap ma ĐT va nhan enter
                            WaitingManager.Show();

                            string str = string.Format("{0:000000000000}", Convert.ToInt64(strValue));
                            this.txtIFTreatmentCode.Text = str;

                            HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                            treatmentFilter.ORDER_DIRECTION = "DESC";
                            treatmentFilter.ORDER_FIELD = "CREATE_TIME";
                           // treatmentFilter.TDL_PATIENT_CODE__EXACT = this.txtPatientCodeForAdd.Text;
                            treatmentFilter.TREATMENT_CODE__EXACT = txtIFTreatmentCode.Text;
                            CommonParam paramCommon = new CommonParam();
                            this.currentTreatment = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_TREATMENT>>(RequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, paramCommon).FirstOrDefault();

                            if (this.currentTreatment != null)
                            {
                                this.currentTreatmentId = this.currentTreatment.ID;
                                this.currentPatientId = this.currentTreatment.PATIENT_ID;
                                this.txtPatientCodeForAdd.Text = this.currentTreatment.TDL_PATIENT_CODE;
                                this.lblPatientName.Text = this.currentTreatment.TDL_PATIENT_NAME;
                                this.lblGenderName.Text = this.currentTreatment.TDL_PATIENT_GENDER_NAME;
                                this.lblPatientAddress.Text = this.currentTreatment.TDL_PATIENT_ADDRESS;
                                this.lblHeinCardNumber.Text = this.currentTreatment.TDL_HEIN_CARD_NUMBER;
                                this.lblPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentTreatment.TDL_PATIENT_DOB);


                                HisHoldReturnViewFilter holdReturnFilter = new HisHoldReturnViewFilter();
                                holdReturnFilter.PATIENT_ID = this.currentPatientId;
                                //holdReturnFilter.HOLD_ROOM_ID = this.currentModule.RoomId;
                                //holdReturnFilter.RESPONSIBLE_ROOM_ID = this.currentModule.RoomId;
                                holdReturnFilter.IS_HANDOVERING = false;//TODO
                                paramCommon = new CommonParam();
                                var holdReturns = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_HOLD_RETURN>>(RequestUriStore.HIS_HOLD_RETURN_GETVIEW, ApiConsumers.MosConsumer, holdReturnFilter, paramCommon);

                                if (holdReturns != null && holdReturns.Count > 0)
                                {
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => holdReturns), holdReturns));
                                    var roomHold = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == holdReturns[0].HOLD_ROOM_ID).FirstOrDefault();
                                    this.lblHandoverRoom.Text = roomHold != null ? roomHold.ROOM_NAME : "";
                                    this.btnSave.Enabled = true;
                                    if (holdReturns[0].RETURN_TIME != null)
                                    {
                                        this.btnSave.Enabled = true;
                                    }
                                    else
                                    {
                                        this.btnSave.Enabled = false;
                                    }
                                }
                                else
                                {
                                    if (holdReturns[0].RETURN_TIME != null)
                                    {
                                        this.btnSave.Enabled = true;
                                    }
                                    else
                                    {
                                        this.btnSave.Enabled = false;
                                    }
                                    this.gridViewDocType.Focus();
                                }
                            }
                            else
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Không tồn tại dữ liệu", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Warning);
                            }
                            WaitingManager.Hide();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
