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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo;
using HIS.Desktop.Utilities.Extentions;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AppointmentService
{
    public partial class frmAppointmentService : FormBase
    {

        private void BindTree()
        {
            try
            {
                dicService = new Dictionary<SERVICE_ENUM,List<ServiceADO>>();
                dicService[SERVICE_ENUM.ALL] = new List<ServiceADO>();
                dicService[SERVICE_ENUM.PARENT] =new List<ServiceADO>();
                dicService[SERVICE_ENUM.PARENT_FOR_GRID_SERVCIE] = new List<ServiceADO>();

                ServiceComboADO serviceComboADO = null;
                if (ServiceComboDataWorker.DicServiceCombo == null)
                    ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, ServiceComboADO>();
                if (ServiceComboDataWorker.DicServiceCombo.ContainsKey(this.PatientTypeAlter.PATIENT_TYPE_ID))
                {
                    ServiceComboDataWorker.DicServiceCombo.TryGetValue(this.PatientTypeAlter.PATIENT_TYPE_ID, out serviceComboADO);
                }
                else
                {
                    serviceComboADO = ServiceComboDataWorker.GetByPatientType(PatientTypeAlter.PATIENT_TYPE_ID, BranchDataWorker.DicServicePatyInBranch);

                    ServiceComboDataWorker.DicServiceCombo.Add(this.PatientTypeAlter.PATIENT_TYPE_ID, serviceComboADO);
                }

                if (serviceComboADO != null)
                {
                    ServiceIsleafADOs = serviceComboADO.ServiceIsleafADOs;
                    dicService[SERVICE_ENUM.ALL] = serviceComboADO.ServiceAllADOs;
                    dicService[SERVICE_ENUM.PARENT] = serviceComboADO.ServiceParentADOs;
                    dicService[SERVICE_ENUM.PARENT_FOR_GRID_SERVCIE] = serviceComboADO.ServiceParentADOs;

                    foreach (var item in ServiceIsleafADOs)
                    {
                        item.IsChecked = false;
                        item.ShareCount = null;
                        item.AMOUNT = 1;
                        item.PATIENT_TYPE_ID = 0;
                        item.PRICE = 0;
                        item.TDL_EXECUTE_ROOM_ID = 0;
                        item.IsExpend = false;
                        item.IsOutKtcFee = ((item.IS_OUT_PARENT_FEE ?? -1) == 1);
                        item.IsKHBHYT = false;
                        item.SERVICE_GROUP_ID_SELECTEDs = null;
                        item.IsNoDifference = false;
                        item.ErrorMessageAmount = "";
                        item.ErrorMessageIsAssignDay = "";
                        item.ErrorMessagePatientTypeId = "";
                        item.ErrorTypeAmount = ErrorType.None;
                        item.ErrorTypeIsAssignDay = ErrorType.None;
                        item.ErrorTypePatientTypeId = ErrorType.None;
                    }
                }
                //SereServADO tt = ServiceIsleafADOs.FirstOrDefault(o => o.TDL_SERVICE_CODE == "TT");

                records = new BindingList<ServiceADO>(dicService[SERVICE_ENUM.PARENT]);
                this.treeService.DataSource = records;
                this.treeService.KeyFieldName = "CONCRETE_ID__IN_SETY";
                this.treeService.ParentFieldName = "PARENT_ID__IN_SETY";
                this.treeService.CollapseAll();
                this.hideCheckBoxHelper__Service = new HideCheckBoxHelper(this.treeService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
