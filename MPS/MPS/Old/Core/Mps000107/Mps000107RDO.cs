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
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Core.Mps000107
{
    public class Mps000107RDO : RDOBase
    {
        internal V_HIS_SERVICE_REQ HisServiceReq { get; set; }
        internal List<V_HIS_EXP_MEST_BLOOD> HisExpMestBloods { get; set; }
        internal List<V_HIS_EXP_MEST_BLTY> HisExpMestBltys { get; set; }
        internal List<ExpMestBloodADO> ListBloodADO = new List<ExpMestBloodADO>();

        public Mps000107RDO(V_HIS_SERVICE_REQ serviceReq, List<V_HIS_EXP_MEST_BLTY> expMestBltys, List<V_HIS_EXP_MEST_BLOOD> expMestBloods)
        {
            try
            {
                this.HisServiceReq = serviceReq;
                this.HisExpMestBloods = expMestBloods;
                this.HisExpMestBltys = expMestBltys;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal override void SetSingleKey()
        {
            try
            {
                if (HisServiceReq != null)
                {
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.PATIENT_CODE, HisServiceReq.PATIENT_CODE));
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.VIR_PATIENT_NAME, HisServiceReq.VIR_PATIENT_NAME));
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(HisServiceReq.DOB)));
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.DOB_YEAR_STR, HisServiceReq.DOB.ToString().Substring(0, 4)));
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.GENDER_NAME, HisServiceReq.GENDER_NAME));
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.VIR_ADDRESS, HisServiceReq.VIR_ADDRESS));
                    string icdText = null;
                    if (!String.IsNullOrEmpty(HisServiceReq.ICD_MAIN_TEXT))
                    {
                        icdText = HisServiceReq.ICD_MAIN_TEXT;
                    }
                    else
                    {
                        icdText = HisServiceReq.ICD_NAME;
                    }
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.ICD_MAIN_TEXT, icdText));
                }

                if (HisExpMestBloods != null && HisExpMestBloods.Count > 0)
                {
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.VIR_TOTAL_PRICE, HisExpMestBloods.Sum(s => s.PRICE ?? 0)));
                    keyValues.Add(new KeyValue(Mps000107ExtendSingleKey.VIR_TOTAL_PRICE_OTHER, 0));

                    this.ListBloodADO = (from r in this.HisExpMestBloods select new ExpMestBloodADO(r)).ToList();
                    // sắp xếp theo thứ tự tăng dần id
                    this.ListBloodADO = this.ListBloodADO.OrderBy(p => p.ID).ToList();
                }

                if (this.HisExpMestBltys == null)
                {
                    this.HisExpMestBltys = new List<V_HIS_EXP_MEST_BLTY>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
