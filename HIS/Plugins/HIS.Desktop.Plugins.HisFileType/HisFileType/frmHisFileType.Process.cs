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

namespace HIS.Desktop.Plugins.HisFileType.HisFileType
{
    public partial class frmHisFileType
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
                MOS.EFMODEL.DataModels.HIS_FILE_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_FILE_TYPE();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>(HisRequestUriStore.MOS_HIS_FILE_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridFileType();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>(HisRequestUriStore.MOS_HIS_FILE_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridFileType();
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>();
                    //SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_FILE_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisFileTypeFilter filter = new MOS.Filter.HisFileTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>>(HisRequestUriStore.MOS_HIS_FILE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_FILE_TYPE currentDTO)
        {
            try
            {
                currentDTO.FILE_TYPE_CODE = txtCode.Text.Trim();
                currentDTO.FILE_TYPE_NAME = txtName.Text.Trim();

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
                var rowData = (MOS.EFMODEL.DataModels.HIS_FILE_TYPE)gridViewFileType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.Filter.HisFileTypeFilter filter = new MOS.Filter.HisFileTypeFilter();
                    filter.ID = rowData.ID;
                    var data = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>>(HisRequestUriStore.MOS_HIS_FILE_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOS_HIS_FILE_TYPE_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>();
                            FillDataToGridFileType();
                            currentData = ((List<MOS.EFMODEL.DataModels.HIS_FILE_TYPE>)gridControlFileType.DataSource).FirstOrDefault();
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
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
            HIS_FILE_TYPE success = new HIS_FILE_TYPE();
            try
            {
                HIS_FILE_TYPE data = (HIS_FILE_TYPE)gridViewFileType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_FILE_TYPE>(HisRequestUriStore.MOS_HIS_FILE_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_FILE_TYPE>();
                        FillDataToGridFileType();
                    }
                    MessageManager.Show(this, param, success != null);
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
            HIS_FILE_TYPE success = new HIS_FILE_TYPE();
            try
            {
                HIS_FILE_TYPE data = (HIS_FILE_TYPE)gridViewFileType.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_FILE_TYPE>(HisRequestUriStore.MOS_HIS_FILE_TYPE_CHANGE_LOCK, ApiConsumers.MosConsumer, data.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_FILE_TYPE>();
                        FillDataToGridFileType();
                    }
                    MessageManager.Show(this, param, success != null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
