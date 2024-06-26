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
        public List<V_HIS_TREATMENT_4> Treatments { get; set; }
        public List<V_HIS_SERVICE_REQ> ServiceReqs { get; set; }
        public List<HIS_KSK_GENERAL> ksk_Generals { get; set; }
        public List<V_HIS_SERE_SERV> SereServs { get; set; }
        public List<HIS_SERVICE> HisServices { get; set; }
        public List<HIS_SERE_SERV_EXT> SereSErvExts { get; set; }
        public List<V_HIS_TEST_INDEX> TestIndexs { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> SereServTeins { get; set; }
        public List<HIS_HEALTH_EXAM_RANK> HealthExamRanks { get; set; }
        public List<HIS_POSITION> HisPositions { get; set; }
    }

    public class SereServADO : V_HIS_SERE_SERV
    {
        public string CONCLUDE { get; set; }
        public long? NUM_ORDER { get; set; }
        public long? FUEX_TYPE_ID { get; set; }
        public long? DIIM_TYPE_ID { get; set; }

        public long? TEST_TYPE_ID { get; set; }

        public SereServADO (){}
        public SereServADO(V_HIS_SERE_SERV data, HIS_SERVICE Service, HIS_SERE_SERV_EXT SereSErvExt)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);

                    if (Service != null)
                    {
                        this.NUM_ORDER = Service.NUM_ORDER;
                        this.FUEX_TYPE_ID = Service.FUEX_TYPE_ID;
                        this.DIIM_TYPE_ID = Service.DIIM_TYPE_ID;
                        this.TEST_TYPE_ID = Service.TEST_TYPE_ID;
                    }

                    if (SereSErvExt != null)
                    {
                        this.CONCLUDE = SereSErvExt.CONCLUDE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, HIS_SERVICE Service)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServADO>(this, data);

                    if (Service != null)
                    {
                        this.NUM_ORDER = Service.NUM_ORDER;
                        this.FUEX_TYPE_ID = Service.FUEX_TYPE_ID;
                        this.DIIM_TYPE_ID = Service.DIIM_TYPE_ID;
                        this.TEST_TYPE_ID = Service.TEST_TYPE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class SereServTeinADO : V_HIS_SERE_SERV_TEIN
    {
        public short? IS_IMPORTANT { get; set; }

        public SereServTeinADO(V_HIS_SERE_SERV_TEIN data, V_HIS_TEST_INDEX TestIndex)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServTeinADO>(this, data);

                    if (TestIndex != null)
                    {
                        this.IS_IMPORTANT = TestIndex.IS_IMPORTANT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class kskGeneralADO
    {
        public long? TREATMENT_ID { get; set; }
        public string DISEASES { get; set; }
        public string TREATMENT_INSTRUCTION { get; set; }
        public long? HEALTH_EXAM_RANK_ID { get; set; }
        public string HEALTH_EXAM_RANK_NAME { get; set; }
        public string HEALTH_EXAM_RANK_CODE { get; set; }

        public kskGeneralADO() { }

        public kskGeneralADO(HIS_KSK_GENERAL data, List<V_HIS_SERVICE_REQ> ServiceReqs, List<HIS_HEALTH_EXAM_RANK> HealthExamRanks)
        {
            try
            {
                if (data != null)
                {
                    this.DISEASES = data.DISEASES;
                    this.TREATMENT_INSTRUCTION = data.TREATMENT_INSTRUCTION;
                    this.HEALTH_EXAM_RANK_ID = data.HEALTH_EXAM_RANK_ID;

                    if (ServiceReqs != null && ServiceReqs.Count > 0)
                    {
                        var ServiceReq = ServiceReqs.FirstOrDefault(o => o.ID == data.SERVICE_REQ_ID);

                        TREATMENT_ID = ServiceReq.TREATMENT_ID;
                    }

                    if (HealthExamRanks != null && HealthExamRanks.Count > 0)
                    {
                        var HealthExamRank = HealthExamRanks.FirstOrDefault(o => o.ID == data.HEALTH_EXAM_RANK_ID);

                        if (HealthExamRank != null)
                        {
                            this.HEALTH_EXAM_RANK_CODE = HealthExamRank.HEALTH_EXAM_RANK_CODE;
                            this.HEALTH_EXAM_RANK_NAME = HealthExamRank.HEALTH_EXAM_RANK_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }

    public class TreatmentlADO : V_HIS_TREATMENT_4
    {
        public string PATIENT_POSITION_CODE { get; set; }
        public string PATIENT_POSITION_NAME { get; set; }

        public string DISEASES { get; set; }
        public string TREATMENT_INSTRUCTION { get; set; }
        public string HEALTH_EXAM_RANK_NAME { get; set; }
        public string HEALTH_EXAM_RANK_CODE { get; set; }

        public TreatmentlADO() { }
        public TreatmentlADO(V_HIS_TREATMENT_4 data, List<HIS_POSITION> HisPositions, List<kskGeneralADO> kskGeneralADOs)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<TreatmentlADO>(this, data);

                    if (HisPositions != null && HisPositions.Count > 0)
                    {
                        var HisPosition = HisPositions.FirstOrDefault(o => o.ID == data.TDL_PATIENT_POSITION_ID);
                        if (HisPosition != null)
                        {
                            this.PATIENT_POSITION_CODE = HisPosition.POSITION_CODE;
                            this.PATIENT_POSITION_NAME = HisPosition.POSITION_NAME;
                        }
                    }

                    if (kskGeneralADOs != null && kskGeneralADOs.Count > 0)
                    {
                        var lstkskGeneralADO = kskGeneralADOs.FirstOrDefault(o => o.TREATMENT_ID == data.ID);
                        if (lstkskGeneralADO != null)
                        {
                            this.DISEASES = lstkskGeneralADO.DISEASES;
                            this.TREATMENT_INSTRUCTION = lstkskGeneralADO.TREATMENT_INSTRUCTION;
                            this.HEALTH_EXAM_RANK_NAME = lstkskGeneralADO.HEALTH_EXAM_RANK_NAME;
                            this.HEALTH_EXAM_RANK_CODE = lstkskGeneralADO.HEALTH_EXAM_RANK_CODE;
                        }
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
