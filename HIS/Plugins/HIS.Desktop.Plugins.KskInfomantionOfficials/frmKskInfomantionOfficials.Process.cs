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
using HIS.Desktop.Plugins.KskInfomantionOfficials.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    partial class frmKskInfomantionOfficials
    {
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnSave.Enabled)
                    return;
                  
                positionHandle = -1;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                if (this.currentData == null || this.currentData.ID <= 0)
                    return;

                WaitingManager.Show();
                MOS.SDO.HisServiceReqKskOfficialsSDO updateDTO = new MOS.SDO.HisServiceReqKskOfficialsSDO();
                LoadCurrent(this.currentData, ref updateDTO);
                UpdateOfficialsDTOFromDataForm(ref updateDTO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("updateDTO__:", updateDTO));
                var resultData = new BackendAdapter(param).Post<MOS.SDO.KskOfficialsResultSDO>(HisRequestUriStore.MOS_HIS_SERVICE_REQ_KSK_OFFICIALS, ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    FillDataToGridControl();
                    SyncCurrentDataAfterSave(resultData);
                }

                if (success)
                {
                    EnableControlChanged(this.currentServiceReqSTT);
                    SetFocusEditor();
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

        /// <summary>
        /// Sau khi Luu, grid da load lai (listData la object moi) nhung currentData van la ADO cu:
        /// KSK_GENERAL.ID = 0 / HIS_DHST.ID = 0 (lan Luu dau) → Luu tiep se tao them DHST moi moi lan,
        /// trang thai y lenh (Chua xu ly → Dang xu ly/Hoan thanh) khong cap nhat, focus grid lech sang dong khac.
        /// Dong bo lai tu ket qua API, khong goi them API.
        /// </summary>
        private void SyncCurrentDataAfterSave(MOS.SDO.KskOfficialsResultSDO resultData)
        {
            try
            {
                if (resultData == null || this.currentData == null) return;

                long serviceReqId = this.currentData.ID;
                HIS_KSK_GENERAL general = resultData.HisKskGeneral ?? this.currentData.KSK_GENERAL;
                if (general != null && resultData.HisDhst != null)
                    general.HIS_DHST = resultData.HisDhst;

                List<HIS_DISEASE_DETAIL_RESULT> results = (general != null && general.ID > 0 && resultData.DiseaseDetailResults != null)
                    ? DedupDiseaseResults(resultData.DiseaseDetailResults, general.ID)
                    : this.currentData.DISEASE_DETAIL_RESULTS;

                ADO.ServiceReqADO newRow = this.listData != null ? this.listData.FirstOrDefault(o => o.ID == serviceReqId) : null;
                ADO.ServiceReqADO target = newRow ?? this.currentData;
                target.KSK_GENERAL = general;
                target.KSK_OCCUPATIONAL = this.currentData.KSK_OCCUPATIONAL;
                target.DISEASE_DETAIL_RESULTS = results;

                this.currentData = target;
                this.listChooseFromGrid = new List<ADO.ServiceReqADO>() { target };
                this.diseaseResults = results ?? new List<HIS_DISEASE_DETAIL_RESULT>();
                SetCurrentServiceReqSTT(target);

                if (newRow != null)
                {
                    int rowHandle = gridViewServiceReq.LocateByValue("ID", serviceReqId);
                    if (rowHandle >= 0)
                        gridViewServiceReq.FocusedRowHandle = rowHandle;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(ADO.ServiceReqADO currentData, ref MOS.SDO.HisServiceReqKskOfficialsSDO currentDTO)
        {
            try
            {
                currentDTO.ServiceReqId = currentData.ID;
                currentDTO.isFinish = chkIsFinish.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FinishProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnFinish.Enabled)
                    return;

                positionHandle = -1;

                if (this.currentData == null
                    || this.currentServiceReqSTT == ServiceReqStatus.HoanThanh
                    || this.currentServiceReqSTT == ServiceReqStatus.Default)
                {
                    EnableControlChanged(this.currentServiceReqSTT);
                    return;
                }

                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>(
                    HisRequestUriStore.MOS_HIS_SERVICE_REQ_FINISH,
                    ApiConsumers.MosConsumer, this.currentData.ID, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

                if (success)
                {
                    ResetPatientInfoDisplayed();
                    ResetFormData();
                    this.currentData = null;
                    EnableControlChanged(this.currentServiceReqSTT);
                    SetFocusEditor();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnfinishProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnUnfinish.Enabled)
                    return;

                positionHandle = -1;

                if (this.currentData == null
                    || this.currentServiceReqSTT != ServiceReqStatus.HoanThanh)
                {
                    EnableControlChanged(this.currentServiceReqSTT);
                    return;
                }

                WaitingManager.Show();
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE_REQ>(
                    HisRequestUriStore.MOS_HIS_SERVICE_REQ_UNFINISH,
                    ApiConsumers.MosConsumer, this.currentData.ID, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                }

                if (success)
                {
                    ResetPatientInfoDisplayed();
                    ResetFormData();
                    this.currentData = null;
                    EnableControlChanged(this.currentServiceReqSTT);
                    SetFocusEditor();
                }

                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
