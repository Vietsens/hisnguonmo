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
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SwapService
{
    public partial class frmSwapService : HIS.Desktop.Utility.FormBase
    {
        private void LoadDataToPatientType(DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit repositoryItemcboPatientType, object data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(repositoryItemcboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGridSereServ()
        {
            try
            {

                if (serviceReq != null && currentSereServ != null)
                {

                    List<V_HIS_SERVICE_ROOM> serviceRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_ROOM>().Where(o => o.ROOM_ID == this.currentModule.RoomId).ToList();
                    List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_ID == currentSereServ.TDL_SERVICE_TYPE_ID && o.ID != currentSereServ.SERVICE_ID).ToList();

                    sereServADOs = new List<HisSereServADO>();
                    foreach (var serviceRoom in serviceRooms)
                    {
                        var serviceAlow = services.FirstOrDefault(o => o.ID == serviceRoom.SERVICE_ID);
                        if (serviceAlow != null)
                        {
                            HisSereServADO model = new HisSereServADO();

                            AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERVICE, HisSereServADO>();
                            model = AutoMapper.Mapper.Map<V_HIS_SERVICE, HisSereServADO>(serviceAlow);
                            model.AMOUNT = 1;

                            int statusParentFee = currentSereServ.IS_OUT_PARENT_FEE ?? 0;
                            if (statusParentFee == 1)
                            {
                                model.IsOutKtcFee = true;
                            }
                            else
                            {
                                model.IsOutKtcFee = false;
                            }

                            int statusExpend = currentSereServ.IS_EXPEND ?? 0;
                            if (statusExpend == 1)
                            {
                                model.IsExpend = true;
                            }

                            else
                            {
                                model.IsExpend = false;
                            }
                            model.IsExpend = false;
                            model.SERVICE_ID = serviceAlow.ID;
                            model.TDL_SERVICE_CODE = serviceAlow.SERVICE_CODE;
                            model.TDL_SERVICE_NAME = serviceAlow.SERVICE_NAME;
                            model.TDL_SERVICE_TYPE_ID = serviceAlow.SERVICE_TYPE_ID;
                            model.PATIENT_TYPE_ID = currentSereServ.PATIENT_TYPE_ID;
                            sereServADOs.Add(model);
                        }
                    }

                    if (sereServADOs != null && sereServADOs.Count > 0)
                    {
                        gridControlSwapService.DataSource = null;
                        List<HisSereServADO> sereServADOsData = sereServADOs.Where(o => o.IS_ACTIVE == 1).ToList();
                        gridControlSwapService.DataSource = sereServADOsData;
                        gridControlSwapService.RefreshDataSource();
                        // gridControlSwapService.EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void LoadCurrentPatientTypeAlter(long treatmentId, long intructionTime)
        {
            try
            {
                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatmentId;
                if (intructionTime > 0)
                    filter.InstructionTime = intructionTime;
                else
                    filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;

                hisPatientTypeAlter = new BackendAdapter(new CommonParam())
                    .Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, new CommonParam());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PatientTypeWithPatientTypeAlter()
        {
            try
            {
                var vPatientTypeAllows = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_PATIENT_TYPE_ALLOW>();
                if (vPatientTypeAllows != null && vPatientTypeAllows.Count > 0)
                {
                    if (hisPatientTypeAlter != null)
                    {
                        var patientTypeAllow = vPatientTypeAllows.Where(o => o.PATIENT_TYPE_ID == hisPatientTypeAlter.PATIENT_TYPE_ID).Select(m => m.PATIENT_TYPE_ALLOW_ID).Distinct().ToList();
                        if (patientTypeAllow != null && patientTypeAllow.Count > 0)
                        {
                            currentPatientTypeWithPatientTypeAlter = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => patientTypeAllow.Contains(o.ID)).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
