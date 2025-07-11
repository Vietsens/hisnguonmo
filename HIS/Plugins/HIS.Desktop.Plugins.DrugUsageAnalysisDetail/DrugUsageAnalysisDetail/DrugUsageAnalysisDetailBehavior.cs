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
using Inventec.Core;
using System;
using System.Linq;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using HIS.Desktop.Plugins.DrugUsageAnalysisDetail;
using DevExpress.XtraEditors.Controls;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.DrugUsageAnalysisDetail.DrugUsageAnalysisDetail
{
    public sealed class DrugUsageAnalysisDetailBehavior : Tool<IDesktopToolContext>, IDrugUsageAnalysisDetail
    {
        object[] entity;


        public DrugUsageAnalysisDetailBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IDrugUsageAnalysisDetail.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                V_HIS_TRACKING trackingData = null;
                Tuple<bool, bool> isAlowEditPharmacistOrDoctor = null;                 
                HIS.Desktop.Common.DelegateSelectData delegateSelectData = null;

                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module drugModule)
                        {
                            moduleData = drugModule;
                        }
                        else if (item is V_HIS_TRACKING tracking)
                        {
                            trackingData = tracking;
                        }
                        else if (item is Tuple<bool, bool> isAEP)
                        {
                            isAlowEditPharmacistOrDoctor = isAEP;
                        }
                        else if (item is HIS.Desktop.Common.DelegateSelectData deleData)
                        {
                            delegateSelectData = deleData;
                        }
                    }
                }
                if (moduleData != null)
                {
                    return new frmDrugUsageAnalysisDetail(moduleData, trackingData, isAlowEditPharmacistOrDoctor, delegateSelectData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }

    }
}
