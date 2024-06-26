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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisPosition.HisPosition
{
    partial class frmHisPosition
    {
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_POSITION updateDTO = new MOS.EFMODEL.DataModels.HIS_POSITION();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_POSITION>(HisRequestUriStore.MOS_HIS_POSITION_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_POSITION>(HisRequestUriStore.MOS_HIS_POSITION_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_POSITION>();
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                if (!success && param.BugCodes.Contains("MOS000") && this.ActionType == GlobalVariables.ActionAdd)
                {
                    MessageManager.Show(string.Format("Xử lý thất bại. Mã {0} đã tồn tại trên hệ thống", txtPositionCode.Text.Trim()));
                }
                else
                {
                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion
                }

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_POSITION currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPositionFilter filter = new MOS.Filter.HisPositionFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_POSITION>>(HisRequestUriStore.MOS_HIS_POSITION_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_POSITION currentDTO)
        {
            try
            {
                currentDTO.POSITION_CODE = txtPositionCode.Text.Trim();
                currentDTO.POSITION_NAME = txtPositionName.Text.Trim();
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteProcess()
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_POSITION)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.Filter.HisPositionFilter filter = new MOS.Filter.HisPositionFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_POSITION>>(HisRequestUriStore.MOS_HIS_POSITION_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOS_HIS_POSITION_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_POSITION>();
                            FillDataToGridControl();
                            currentData = ((List<MOS.EFMODEL.DataModels.HIS_POSITION>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                    ResetFormData();
                    this.ActionType = GlobalVariables.ActionAdd;
                    EnableControlChanged(this.ActionType);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LockProcess()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_POSITION data = (HIS_POSITION)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_POSITION>(HisRequestUriStore.MOS_HIS_POSITION_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_POSITION>();
                        FillDataToGridControl();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnlockProcess()
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HIS_POSITION data = (HIS_POSITION)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_POSITION>(HisRequestUriStore.MOS_HIS_POSITION_CHANGELOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (result != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<HIS_POSITION>();
                        FillDataToGridControl();
                    }

                    #region Hien thi message thong bao
                    MessageManager.Show(this, param, success);
                    #endregion

                    #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
