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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTrackingList.ADO
{
    public class TrackingListADO
    {
        public TrackingListADO() { }
        public List<V_HIS_TRACKING> Trackings { get; set; }
    }
    public class V_HIS_TRACKING_EXT
    {
        public MOS.EFMODEL.DataModels.V_HIS_TRACKING Tracking { get; set; }
        public HIS_DRUG_USE_ANALYSIS DrugUseAnalysis { get; set; }
        public bool HasDrugUseAnalysis => DrugUseAnalysis != null && DrugUseAnalysis.ID > 0;
    }
}
