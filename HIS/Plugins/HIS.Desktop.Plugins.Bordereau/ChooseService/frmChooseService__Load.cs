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
using HIS.Desktop.Utility;
using Inventec.Common.Controls.EditorLoader;
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

namespace HIS.Desktop.Plugins.Bordereau.ChooseService
{
    public partial class frmChooseService : FormBase
    {
        private void LoadServicePackage()
        {
            try
            {
                string packageCode3Day7Day = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKeys.POLICY_CODE_3DAY7DAY);
                List<HIS_PACKAGE> packages = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PACKAGE>(false, true);
                if (packages == null || packages.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Error("Khong tim thay thong tin goi nao!");
                }
                HIS_PACKAGE package37 = packages != null ? packages.FirstOrDefault(o => o.PACKAGE_CODE == packageCode3Day7Day) : null;
                this.SereServADOPackages = this.SereServADOs.Where(o =>
                    o.IS_NO_EXECUTE != 1
                    && (o.PACKAGE_ID.HasValue && package37 != null && o.PACKAGE_ID == package37.ID)
                    || ((o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                        || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                    || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA))
                        && (currentDepartmentId == o.TDL_EXECUTE_DEPARTMENT_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCboService()
        {
            try
            {
                if (this.SereServADOPackages != null && this.SereServADOPackages.Count > 0)
                {
                    if (this.sereServADOSelecteds != null && this.sereServADOSelecteds.Count > 0)
                    {
                        List<long> parentIds = this.sereServADOSelecteds.Where(o=>o.PARENT_ID.HasValue)
                            .Select(o => o.PARENT_ID.Value).ToList();
                        if (parentIds != null && parentIds.Count > 0)
                        {
                            cboService.EditValue = parentIds[0];
                        }
                    }

                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("INSTRUCTION_TIME___SERVICE_REQ_CODE", "", 200, 1));
                    columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 300, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_REQ_CODE___SERVICE_NAME", "ID", columnInfos, false, 300);
                    ControlEditorLoader.Load(cboService, this.SereServADOPackages, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
