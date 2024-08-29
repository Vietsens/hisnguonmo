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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using MOS.SDO;
namespace MPS.Processor.Mps000452.PDO
{
    public partial class Mps000452PDO : RDOBase
    {
        public HIS_KSK_OVER_EIGHTEEN HisKskOverEighteen { get; set; }
        public V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        public HIS_DHST HisDhst { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> examRank { get; set; }
        public List<HIS_DISEASE_TYPE> DiseaseType { get; set; }
        public List<HIS_PERIOD_DRIVER_DITY> PeriodDriverDity { get; set; }

        public Mps000452PDO(
            HIS_KSK_OVER_EIGHTEEN HisKskOverEighteen,
           V_HIS_SERVICE_REQ HisServiceReq,
           HIS_DHST HisDhst
            , List<HIS_HEALTH_EXAM_RANK> examRank
            )
        {
            try
            {
                this.HisKskOverEighteen = HisKskOverEighteen;
                this.HisServiceReq = HisServiceReq;
                this.HisDhst = HisDhst;
                this.examRank = examRank;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000452PDO(
           HIS_KSK_OVER_EIGHTEEN HisKskOverEighteen,
          V_HIS_SERVICE_REQ HisServiceReq,
          HIS_DHST HisDhst
           , List<HIS_HEALTH_EXAM_RANK> examRank,
          List<HIS_DISEASE_TYPE> DiseaseType,
          List<HIS_PERIOD_DRIVER_DITY> PeriodDriverDity
           )
        {
            try
            {
                this.HisKskOverEighteen = HisKskOverEighteen;
                this.HisServiceReq = HisServiceReq;
                this.HisDhst = HisDhst;
                this.examRank = examRank;
                this.DiseaseType = DiseaseType;
                this.PeriodDriverDity = PeriodDriverDity;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
