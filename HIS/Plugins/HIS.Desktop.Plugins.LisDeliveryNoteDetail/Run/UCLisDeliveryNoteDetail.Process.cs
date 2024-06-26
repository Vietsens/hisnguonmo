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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.LisDeliveryNoteDetail.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.LisDeliveryNoteDetail
{
    public partial class UCLisDeliveryNoteDetail
    {
        private void ProcessApprove(SampleADO sample)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LIS_SAMPLE apiresult = null;

                LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                sdo.SampleId = sample.ID;
                sdo.WorkingRoomId = this.moduleData.RoomId;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<LIS_SAMPLE>(RequestUriStore.LIS_SAMPLE_APPROVE, ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));

                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    FillDataToGridControl_Sample();
                    if (chkAutoFocusTo_txtKeyword.Checked)
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCancelApprove(SampleADO sample)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LIS_SAMPLE apiresult = null;

                LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                sdo.SampleId = sample.ID;
                sdo.WorkingRoomId = this.moduleData.RoomId;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<LIS_SAMPLE>(RequestUriStore.LIS_SAMPLE_UNAPPROVE, ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));

                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    FillDataToGridControl_Sample();
                    if (chkAutoFocusTo_txtKeyword.Checked)
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessRejectApprove(SampleADO sample, string rejectReason)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LIS_SAMPLE apiresult = null;

                LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                sdo.SampleId = sample.ID;
                sdo.WorkingRoomId = this.moduleData.RoomId;
                sdo.RejectReason = rejectReason;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<LIS_SAMPLE>(RequestUriStore.LIS_SAMPLE_REJECT, ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));

                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    FillDataToGridControl_Sample();
                    if (chkAutoFocusTo_txtKeyword.Checked)
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessCancelRejectApprove(SampleADO sample)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LIS_SAMPLE apiresult = null;

                LisSampleApproveSDO sdo = new LisSampleApproveSDO();
                sdo.SampleId = sample.ID;
                sdo.WorkingRoomId = this.moduleData.RoomId;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<LIS_SAMPLE>(RequestUriStore.LIS_SAMPLE_UNREJECT, ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));

                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    FillDataToGridControl_Sample();
                    if (chkAutoFocusTo_txtKeyword.Checked)
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessApproveList(List<long> listSampleID)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                List<LIS_SAMPLE> apiresult = null;

                LisSampleApproveListSDO sdo = new LisSampleApproveListSDO();
                sdo.SampleIds = listSampleID;
                sdo.WorkingRoomId = this.moduleData.RoomId;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<List<LIS_SAMPLE>>(RequestUriStore.LIS_SAMPLE_APPROVE_LIST, ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null && apiresult.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));

                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    FillDataToGridControl_Sample();
                    if (chkAutoFocusTo_txtKeyword.Checked)
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessRejectList(List<long> listSampleID, string rejectReason)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                List<LIS_SAMPLE> apiresult = null;

                LisSampleApproveListSDO sdo = new LisSampleApproveListSDO();
                sdo.SampleIds = listSampleID;
                sdo.WorkingRoomId = this.moduleData.RoomId;
                sdo.RejectReason = rejectReason;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                apiresult = new BackendAdapter(param).Post<List<LIS_SAMPLE>>(RequestUriStore.LIS_SAMPLE_REJECT_LIST, ApiConsumer.ApiConsumers.LisConsumer, sdo, param);

                WaitingManager.Hide();
                if (apiresult != null && apiresult.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));

                    success = true;
                }

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion

                if (success)
                {
                    FillDataToGridControl_Sample();
                    if (chkAutoFocusTo_txtKeyword.Checked)
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
