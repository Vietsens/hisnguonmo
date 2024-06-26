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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000481.PDO
{
    public partial class Mps000481PDO : RDOBase
    {
         public Mps000481PDO(){}
         public Mps000481PDO(List<V_HIS_TREATMENT_4> _Treatment,
             List<V_HIS_SERVICE_REQ> _ServiceReqs,
             List<HIS_KSK_GENERAL> _ksk_Generals,
             List<V_HIS_SERE_SERV> _SereServs,
             List<HIS_SERVICE> _HisServices,
             List<HIS_SERE_SERV_EXT> _SereSErvExts,
             List<V_HIS_TEST_INDEX> _TestIndexs,
             List<V_HIS_SERE_SERV_TEIN> _SereServTeins,
             List<HIS_HEALTH_EXAM_RANK> _HealthExamRanks,
             List<HIS_POSITION> _HisPosition)
         {
             try
             {
                 this.Treatments = _Treatment;
                 this.ServiceReqs = _ServiceReqs;
                 this.ksk_Generals = _ksk_Generals;
                 this.SereServs = _SereServs;
                 this.HisServices = _HisServices;
                 this.SereSErvExts = _SereSErvExts;
                 this.TestIndexs = _TestIndexs;
                 this.SereServTeins = _SereServTeins;
                 this.HealthExamRanks = _HealthExamRanks;
                 this.HisPositions = _HisPosition;
             }
             catch (Exception ex)
             {
                 Inventec.Common.Logging.LogSystem.Error(ex);
             }
         }

    }
}
