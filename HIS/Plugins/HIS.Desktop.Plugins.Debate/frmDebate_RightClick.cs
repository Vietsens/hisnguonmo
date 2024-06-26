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
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Debate.Processors;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Debate
{
    public partial class frmDebate
    {
        bool isProcessingTMP = false;
        V_HIS_DEBATE debate_ForProcess;
        List<V_HIS_SERVICE_REQ> listServiceReq_ForProcess;
        List<HIS_SERE_SERV> listSereServ_ForProcess;
        List<V_HIS_SERE_SERV_TEIN> listSereServTein_ForProcess;
        List<V_HIS_TREATMENT_BED_ROOM> listTreatmentBedRoom_ForProcess;
        V_HIS_TREATMENT treatment_ForProcess;
        V_HIS_PATIENT patient_ForProcess;

        void Debate_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.debate_ForProcess != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.TMP:
                            bbtnTMP();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnTMP()
        {
            try
            {
                if (isProcessingTMP == true)
                {
                    return;
                }
                else
                {
                    ProcessTMP(this.debate_ForProcess);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ProcessTMP(V_HIS_DEBATE debate)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("ProcessTMP()_Begin!");
                this.isProcessingTMP = true;
                if (String.IsNullOrEmpty(Config.HisConfigCFG.Telemedicine_ConnectionInfo))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chưa khai báo cấu hình kết nối. Vui lòng điền thông tin vào cấu hình hệ thống HIS.Desktop.Plugins.Library.Telemedicine.ConnectionInfo.");
                    return;
                }
                string[] listConfig = Config.HisConfigCFG.Telemedicine_ConnectionInfo.Split('|');
                if (listConfig.Count() != 3)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Cấu hình hệ thống HIS.Desktop.Plugins.Library.Telemedicine.ConnectionInfo khai báo không đúng vui lòng kiểm tra lại.");
                    return;
                }
                string uri = listConfig[0] ?? "";
                string loginame = listConfig[1] ?? "";
                string password = listConfig[2] ?? "";

                if (debate != null)
                {
                    WaitingManager.Show();
                    Task taskGetDataBeforeTMP = LoadAsyncDataBeforeTMP(debate);
                    this.treatment_ForProcess = GetTreatment_ByID(debate.TREATMENT_ID);
                    this.patient_ForProcess = GetPatient_ByID(treatment_ForProcess.PATIENT_ID);
                    this.debate_ForProcess = GetDebate_ByID(debate.ID);

                    await taskGetDataBeforeTMP;


                    HIS.Library.Telemedicine.TelemedicineProcessor processor = new HIS.Library.Telemedicine.TelemedicineProcessor(uri, loginame, password);
                    HIS.Library.Telemedicine.TelemedicineResultADO resultTMP = processor.SendToTmp(patient_ForProcess, treatment_ForProcess, debate_ForProcess, listServiceReq_ForProcess, listSereServ_ForProcess, listSereServTein_ForProcess, listTreatmentBedRoom_ForProcess, this.HisBranch);
                    if (resultTMP!=null)
                    {
                        if (resultTMP.Success)
                        {
                            bool success = false;
                            WaitingManager.Show();
                            CommonParam param = new CommonParam();
                            MOS.SDO.DebateTelemedicineSDO requestSDO = new MOS.SDO.DebateTelemedicineSDO();
                            requestSDO.TmpId = resultTMP.TmpId;
                            requestSDO.DebateId = debate.ID;
                            var apiResult = new BackendAdapter(param).Post<bool>("api/HisDebate/UpdateTelemedicineInfo", ApiConsumers.MosConsumer, requestSDO, param);
                            if (apiResult == true)
                            {
                                success = true;
                            }
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                        }
                        else
                        {
                            WaitingManager.Hide();
                            MessageManager.Show(resultTMP.Message);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("HIS.Library.Telemedicine.TelemedicineResultADO resultTMP = null!");
                    }
                }
                this.isProcessingTMP = false;
                Inventec.Common.Logging.LogSystem.Debug("ProcessTMP()_Ended!");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            WaitingManager.Hide();
        }

        private async Task LoadAsyncDataBeforeTMP(V_HIS_DEBATE debate)
        {
            try
            {
                Task task1 = LoadAsyncListServiceReq(debate.TREATMENT_ID);
                Task task2 = LoadAsyncListSereServ(debate.TREATMENT_ID);
                Task task3 = LoadAsyncListSereServTein(debate.TREATMENT_ID);
                Task task4 = LoadAsyncListTreatmentBedRoom(debate.TREATMENT_ID);

                await task1;
                await task2;
                await task3;
                await task4;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadAsyncListServiceReq(long treatmentId)
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        this.listServiceReq_ForProcess = GetListServiceReq_ByTreatmentID(treatmentId);
                    }
                );
                t.Start();
                await t;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_SERVICE_REQ> GetListServiceReq_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_SERVICE_REQ> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter filter = new MOS.Filter.HisServiceReqViewFilter();
                filter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task LoadAsyncListSereServ(long treatmentId)
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        this.listSereServ_ForProcess = GetListSereServ_ByTreatmentID(treatmentId);
                    }
                );
                t.Start();
                await t;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<HIS_SERE_SERV> GetListSereServ_ByTreatmentID(long treatmentId)
        {
            List<HIS_SERE_SERV> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServFilter filter = new MOS.Filter.HisSereServFilter();
                filter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task LoadAsyncListSereServTein(long treatmentId)
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        this.listSereServTein_ForProcess = GetListSereServTein_ByTreatmentID(treatmentId);
                    }
                );
                t.Start();
                await t;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_SERE_SERV_TEIN> GetListSereServTein_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_SERE_SERV_TEIN> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServTeinViewFilter filter = new MOS.Filter.HisSereServTeinViewFilter();
                filter.TDL_TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task LoadAsyncListTreatmentBedRoom(long treatmentId)
        {
            try
            {
                Task t = new Task(
                    () =>
                    {
                        this.listTreatmentBedRoom_ForProcess = GetListTreatmentBedRoom_ByTreatmentID(treatmentId);
                    }
                );
                t.Start();
                await t;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_TREATMENT_BED_ROOM> GetListTreatmentBedRoom_ByTreatmentID(long treatmentId)
        {
            List<V_HIS_TREATMENT_BED_ROOM> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentBedRoomViewFilter filter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                filter.TREATMENT_ID = treatmentId;
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private V_HIS_TREATMENT GetTreatment_ByID(long id)
        {
            V_HIS_TREATMENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private V_HIS_PATIENT GetPatient_ByID(long id)
        {
            V_HIS_PATIENT result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private V_HIS_DEBATE GetDebate_ByID(long id)
        {
            V_HIS_DEBATE result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisDebateViewFilter filter = new MOS.Filter.HisDebateViewFilter();
                filter.ID = id;
                result = new BackendAdapter(param).Get<List<V_HIS_DEBATE>>("api/HisDebate/GetView", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
