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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignServiceTest.ADO;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignServiceTest.AssignService
{
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private void BindTree()
        {
            try
            {
                var listGroup = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o =>
                    o.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN).ToList().OrderByDescending(o => o.NUM_ORDER).ThenBy(m => m.SERVICE_TYPE_NAME).ToList();

                ServiceIsleafADOs = new List<SereServADO>();
                ServiceAllADOs = (
                    from m in BackendDataWorker.Get<V_HIS_SERVICE>()
                    where
                     m.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                     && m.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    select new ServiceADO(m)
                    ).Distinct()
                    .OrderByDescending(o => o.NUM_ORDER)
                    .ThenBy(o => o.SERVICE_NAME)
                    .ToList();

                ServiceParentADOs = ServiceAllADOs.Where(m => (m.IS_LEAF ?? 0) != 1)
                    .OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_NAME)
                    .ToList();

                // ChoosePatientTypeDefaultlService(currentHisPatientTypeAlter.PATIENT_TYPE_ID, m.ID)

                var patientTypeIdAllows = this.currentPatientTypeWithPatientTypeAlter.Select(o => o.ID).Distinct().ToList();
                var serviceHasServicePatyIds = BackendDataWorker.Get<V_HIS_SERVICE_PATY>()
                    .Where(o => patientTypeIdAllows.Contains(o.PATIENT_TYPE_ID))
                    .Select(o => o.SERVICE_ID).Distinct().ToList();

                var serviceHasServiceRoomIds = BackendDataWorker.Get<V_HIS_SERVICE_ROOM>()
                    .Where(o =>
                        (o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL
                        || o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                        && o.BRANCH_ID == WorkPlace.GetBranchId()
                    )
                    .Select(o => o.SERVICE_ID).Distinct().ToList();

                ServiceIsleafADOs = (
                    from m in ServiceAllADOs
                    where
                     m.IS_LEAF == 1
                    && serviceHasServicePatyIds != null && serviceHasServicePatyIds.Count > 0 && serviceHasServicePatyIds.Contains(m.ID)
                    && serviceHasServiceRoomIds != null && serviceHasServiceRoomIds.Count > 0 && serviceHasServiceRoomIds.Contains(m.ID)
                    select new SereServADO(m, null, IsAssignDay(m.ID))
                    ).Distinct()
                    .OrderByDescending(o => o.SERVICE_NUM_ORDER)
                    .ThenBy(o => o.TDL_SERVICE_NAME)
                    .ToList();

                foreach (var gr in listGroup)
                {
                    ServiceADO sety1 = new ServiceADO();
                    sety1.CONCRETE_ID__IN_SETY = gr.ID + ".";
                    sety1.NUM_ORDER = gr.NUM_ORDER;
                    sety1.SERVICE_CODE = gr.SERVICE_TYPE_CODE;
                    sety1.SERVICE_NAME = gr.SERVICE_TYPE_NAME.ToUpper();
                    sety1.SERVICE_TYPE_CODE = gr.SERVICE_TYPE_CODE;
                    sety1.SERVICE_TYPE_NAME = gr.SERVICE_TYPE_NAME.ToUpper();
                    sety1.SERVICE_TYPE_ID = gr.ID;
                    ServiceParentADOs.Add(sety1);
                }
                ServiceParentADOs = ServiceParentADOs.OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.SERVICE_NAME).ToList();
                records = new BindingList<ServiceADO>(ServiceParentADOs);
                treeService.DataSource = records;
                treeService.KeyFieldName = "CONCRETE_ID__IN_SETY";
                treeService.ParentFieldName = "PARENT_ID__IN_SETY";
                treeService.CollapseAll();
                treeService.ExpandToLevel(0);
                hideCheckBoxHelper__Service = new HideCheckBoxHelper(treeService);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
