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
using MOS.EFMODEL.DataModels;
using System;

namespace HIS.Desktop.Plugins.DrugUsageAnalysis.ADO
{
    public class TrackingDrugUseAnalysisADO : V_HIS_TRACKING
    {
        public string KeyFieldName { get; set; }
        public string ParentFieldName { get; set; }
        public string ServiceName { get; set; }
        public decimal Amount { get; set; }
        public string ServiceUnitName { get; set; }
        public bool IsParent { get; set; }
        public int NOrder { get; set; }
        public bool IsColorDeepSkyBlue { get; set; }
        public TrackingDrugUseAnalysisADO()
        {

        }
        public TrackingDrugUseAnalysisADO(V_HIS_TRACKING data, bool isParent = true, int stt = 0)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TRACKING>(this, data);
                    this.IsParent = isParent;
                    this.KeyFieldName = isParent ? data.ID.ToString() : Guid.NewGuid().ToString();
                    if (!isParent)
                    {
                        this.ParentFieldName = data.ID.ToString();
                        this.NOrder = stt;
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
