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
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Desktop.Common.Message;
using Inventec.Common.RichEditor.Base;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class Form_RequestDeposit : HIS.Desktop.Utility.FormBase
    {
        private void depositReqGrid__CustomUnboundColumnData(V_HIS_DEPOSIT_REQ data, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        try
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "AMOUNT_DISPLAY_TRANSACTION")
                    {
                        try
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.TRANSACTION_AMOUNT ?? 0);

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(V_HIS_DEPOSIT_REQ data, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellStyle(V_HIS_DEPOSIT_REQ data, RowCellStyleEventArgs e)
        {
            try
            {
                if (data != null && data.DEPOSIT_ID == null)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else
                {
                    e.Appearance.ForeColor = Color.Blue;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveProcess()
        {
            try
            {
                //bool isObligatory = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.RequestDeposit.ReasonMustBeEnteredByCategory") == "1";
                if (this.isObligatory && string.IsNullOrEmpty(txtGhiChu.Text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa nhập lý do tạm ứng", "Thông báo");
                    return;
                }

                CommonParam param = new CommonParam();
                bool success = false;
                try
                {
                    this.positionHandleControl = -1;
                    if (!dxValidationProvider1.Validate())
                        return;
                    var depositReq = new HIS_DEPOSIT_REQ();
                    if (this.action == GlobalVariables.ActionEdit)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPOSIT_REQ>(depositReq, this.currentdepositReq);
                    }

                    depositReq.AMOUNT = spinEditPrice.Value;
                    depositReq.DESCRIPTION = txtGhiChu.Text;
                    depositReq.REQUEST_ROOM_ID = currentModule.RoomId;
                    depositReq.TREATMENT_ID = this.treatmentID;
                    if (this.action == GlobalVariables.ActionEdit)
                    {
                        //depositReq.TREATMENT_ID = treatmentID;
                        //depositReq.ID = depositReqView.ID;
                        var dataResult = new BackendAdapter(param).Post<V_HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, depositReq, null);
                        if (dataResult != null)
                        {
                            //this.depositReq = dataResult;
                            success = true;
                            //depositReqProcessor.Reload(ucDepositRq, listdepositReqView);
                            getDataDepositReq(this.treatmentID);
                            FillDataToGridDepositReq();
                            this.action = GlobalVariables.ActionEdit;
                            EnableControlChanged(action);
                        }
                    }
                    else if (this.action == GlobalVariables.ActionAdd)
                    {
                        //depositReq.TREATMENT_ID = treatmentID;
                        //depositReq.ID = depositReqView.ID;
                        var dataResult = new BackendAdapter(param).Post<V_HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_CREATE, ApiConsumer.ApiConsumers.MosConsumer, depositReq, null);
                        if (dataResult != null)
                        {
                            //this.depositReq = dataResult;
                            success = true;
                            //depositReqProcessor.Reload(ucDepositRq, listdepositReqView);
                            getDataDepositReq(this.treatmentID);
                            FillDataToGridDepositReq();
                            this.action = GlobalVariables.ActionAdd;
                            EnableControlChanged(action);
                        }
                    }
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion

                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
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

        private void txtAmount_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGhiChu.Focus();
                    txtGhiChu.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGhiChu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtGhiChu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (action == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled != false)
            {
                btnEdit_Click(null, null);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled != false)
            {
                btnAdd_Click(null, null);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRefresh.Enabled != false)
            {
                btnRefresh_Click(null, null);
            }
        }
    }
}
