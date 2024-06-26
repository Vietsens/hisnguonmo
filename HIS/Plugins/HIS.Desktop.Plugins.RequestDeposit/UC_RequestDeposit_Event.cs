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

namespace HIS.Desktop.Plugins.RequestDeposit
{
    public partial class UC_RequestDeposit : UserControl
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
                            e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(data.AMOUNT);
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
                ////long careSumId = 0;
                ////long careSumId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewCare.GetRowCellValue(e.RowHandle, "CARE_SUM_ID") ?? 0).ToString());
                ////var creator = data.CREATOR;
                ////var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //if (data != null)
                //{
                //    if (e.Column.FieldName == "DELETE")
                //    {
                //        //if (loginName.Equals(creator))
                //        //{
                //        //    e.RepositoryItem = btnDeleteD;
                //        //}
                //        //else
                //        //{
                //        e.RepositoryItem = btnDeleteE;
                //        //}
                //    }
                //    if (e.Column.FieldName == "PRINT")
                //    {
                //        //if (loginName.Equals(creator))
                //        //{
                //        e.RepositoryItem = btnPrintE;
                //        //}
                //        //else
                //        //{
                //        //    e.RepositoryItem = btnPrintD;
                //        //}
                //    }

                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                //this.depositReq = new HIS_DEPOSIT_REQ();
                var depositReqs = currentdepositReq;
                depositReqs.AMOUNT = Convert.ToDecimal(txtAmount.Text);
                depositReqs.DESCRIPTION = txtGhiChu.Text;
                depositReqs.REQUEST_ROOM_ID = GlobalVariables.CurrentModule.RoomId;
                if (this.action == GlobalVariables.ActionEdit)
                {
                    //depositReq.TREATMENT_ID = treatmentID;
                    //depositReq.ID = depositReqView.ID;
                    var dataResult = new BackendAdapter(param).Post<V_HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_UPDATE, ApiConsumer.ApiConsumers.MosConsumer, depositReqs, null);
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
                WaitingManager.Hide();

                #region Show message
                ResultManager.ShowMessage(param, success);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                this.positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                var depositReqs = new V_HIS_DEPOSIT_REQ();
                depositReqs.AMOUNT = Convert.ToDecimal(txtAmount.Text);
                depositReqs.DESCRIPTION = txtGhiChu.Text;
                depositReqs.REQUEST_ROOM_ID = GlobalVariables.CurrentModule.RoomId;
                //depositReqs.REQUEST_DEPARTMENT_ID = WorkPlace.GetDepartmentId();
                if (this.action == GlobalVariables.ActionAdd)
                {
                    depositReqs.TREATMENT_ID = treatmentID;
                    var dataResult = new BackendAdapter(param).Post<V_HIS_DEPOSIT_REQ>(HisRequestUriStore.HIS_DEPOSIT_REQ_CREATE, ApiConsumer.ApiConsumers.MosConsumer, depositReqs, null);
                    if (dataResult != null)
                    {
                        //this.depositReq = dataResult;
                        success = true;
                        this.action = GlobalVariables.ActionEdit;
                        EnableControlChanged(action);
                        getDataDepositReq(this.treatmentID);
                        FillDataToGridDepositReq();
                        txtAmount.Text = null;
                        txtGhiChu.Text = null;
                        txtAmount.Focus();
                    }
                }

                WaitingManager.Hide();

                #region Show message
                ResultManager.ShowMessage(param, success);
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
            btnEdit_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnAdd_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefresh_Click(null, null);
        }

    }
}
