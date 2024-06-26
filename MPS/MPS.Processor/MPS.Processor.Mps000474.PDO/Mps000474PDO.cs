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

namespace MPS.Processor.Mps000474.PDO
{
    public class Mps000474PDO : RDOBase
    {
        public List<HIS_VACC_EXAM_RESULT> HisVaccExamResult { get; set; }

        public List<V_HIS_EXP_MEST_MEDICINE> HisExpMest { get; set; }
        public V_HIS_VACCINATION_EXAM HisVactionExam { get; set; }
        public HIS_VAEX_VAER HisVaexVaer { get; set; }
        public HIS_DHST HisDHST { get; set; }

        public string VACC_TYPE { get; set; }
        public Mps000474PDO(
            List<HIS_VACC_EXAM_RESULT> HisVaccExamResult_,
           V_HIS_VACCINATION_EXAM HisVactionExam_,
          List<V_HIS_EXP_MEST_MEDICINE> HisExpMest_,
            HIS_DHST HisDHST_
            )
        {
            try
            {
                this.HisVaccExamResult = HisVaccExamResult_;
                this.HisVactionExam = HisVactionExam_;
                this.HisExpMest = HisExpMest_;
                this.HisDHST = HisDHST_; 
                // this.HisVaexVaer = HisVaexVaer_;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
