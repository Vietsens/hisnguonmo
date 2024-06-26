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

namespace MPS.Core.Mps000108
{
    public class Mps000108RDO : RDOBase
    {
        internal HIS_EXP_MEST ExpMest { get; set; }
        internal List<V_HIS_EXP_MEST_BLTY> HisExpMestBltys { get; set; }
        //internal PatientADO PatientADO { get; set; }
        internal V_HIS_TREATMENT Treatment { get; set; }
        internal V_HIS_SERVICE_REQ ServiceReq { get; set; }

        long expMesttSttId__Draft = 1;// trạng thái nháp
        long expMesttSttId__Request = 2;// trạng thái yêu cầu
        long expMesttSttId__Reject = 3;// không duyệt
        long expMesttSttId__Approval = 4; // duyệt
        long expMesttSttId__Export = 5;// đã xuất

        public Mps000108RDO(HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_BLTY> hisExpMestBltys, V_HIS_TREATMENT treatment, V_HIS_SERVICE_REQ serviceReq)
        {
            try
            {
                this.HisExpMestBltys = hisExpMestBltys;
                this.Treatment = treatment;
                this.ExpMest = expMest;
                this.ServiceReq = serviceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000108RDO(HIS_EXP_MEST expMest, List<V_HIS_EXP_MEST_BLTY> hisExpMestBltys, V_HIS_TREATMENT treatment, V_HIS_SERVICE_REQ serviceReq, long _expMesttSttId__Draft, long _expMesttSttId__Request, long _expMesttSttId__Reject, long _expMesttSttId__Approval, long _expMesttSttId__Export)
        {
            try
            {
                this.HisExpMestBltys = hisExpMestBltys;
                this.Treatment = treatment;
                this.ExpMest = expMest;
                this.ServiceReq = serviceReq;
                this.expMesttSttId__Draft = _expMesttSttId__Draft;
                this.expMesttSttId__Request = _expMesttSttId__Request;
                this.expMesttSttId__Reject = _expMesttSttId__Reject;
                this.expMesttSttId__Approval = _expMesttSttId__Approval;
                this.expMesttSttId__Export = _expMesttSttId__Export;
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
                string icdText = "";
                if (this.ServiceReq != null)
                {
                    if (!String.IsNullOrEmpty(this.ServiceReq.ICD_MAIN_TEXT))
                    {
                        icdText = this.ServiceReq.ICD_MAIN_TEXT;
                    }
                    else
                    {
                        icdText = this.ServiceReq.ICD_NAME;
                    }
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.ICD_MAIN_TEXT, icdText));
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(ServiceReq.DOB)));
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.DOB_YEAR_STR, ServiceReq.DOB.ToString().Substring(0, 4)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_SERVICE_REQ>(ServiceReq, keyValues, false);
                }
                else if (this.Treatment != null)
                {
                    if (!String.IsNullOrEmpty(this.Treatment.ICD_MAIN_TEXT))
                    {
                        icdText = this.Treatment.ICD_MAIN_TEXT;
                    }
                    else
                    {
                        icdText = this.Treatment.ICD_NAME;
                    }
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.ICD_MAIN_TEXT, icdText));
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.AGE_STR, AgeUtil.CalculateFullAge(Treatment.DOB)));
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.DOB_YEAR_STR, Treatment.DOB.ToString().Substring(0, 4)));
                    GlobalQuery.AddObjectKeyIntoListkey<V_HIS_TREATMENT>(Treatment, keyValues, false);
                }

                if (ExpMest != null)
                {
                    keyValues.Add(new KeyValue(Mps000108ExtendSingleKey.EXP_MEST_CODE, ExpMest.EXP_MEST_CODE));

                    if (ExpMest.EXP_MEST_STT_ID == expMesttSttId__Approval || ExpMest.EXP_MEST_STT_ID == expMesttSttId__Export)
                    {
                        HisExpMestBltys = HisExpMestBltys.Where(o => (o.IN_EXECUTE == null && o.IN_REQUEST == null) || o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                    }
                    else if (ExpMest.EXP_MEST_STT_ID == expMesttSttId__Draft || ExpMest.EXP_MEST_STT_ID == expMesttSttId__Request || ExpMest.EXP_MEST_STT_ID == expMesttSttId__Reject)
                    {
                        HisExpMestBltys = HisExpMestBltys.Where(o => (o.IN_EXECUTE == null && o.IN_REQUEST == null) || o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
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
