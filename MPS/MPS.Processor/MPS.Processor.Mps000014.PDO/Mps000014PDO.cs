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

namespace MPS.Processor.Mps000014.PDO
{
    public class Mps000014PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER _PatyAlterBhyt { get; set; }
        public HIS_TREATMENT _Treatment { get; set; }
        public V_HIS_SERVICE_REQ _ServiceReq { get; set; }
        public List<V_HIS_TEST_INDEX_RANGE> _TestIndexRangeAll { get; set; }
        public List<V_HIS_SERE_SERV_TEIN> _SereServTeins { get; set; }
        public decimal ratio_text { get; set; }
        public List<SereServNumOder> _SereServNumOder { get; set; }
        public long GenderId { get; set; }
        public List<V_HIS_SERVICE> ProcessCode { get; set; }
        public HIS_DHST hisdhst;
        public MLCTADO mLCTADO { get; set; }
        public Mps000014PDO() { }

        public Mps000014PDO(object[] rdo, List<SereServNumOder> _sereServNumOder, List<V_HIS_SERE_SERV_TEIN> lstSereServTein, decimal ratio_text, List<V_HIS_TEST_INDEX_RANGE> testIndexRange, long genderId)
        {
            this._SereServNumOder = _sereServNumOder;
            this._SereServTeins = lstSereServTein;
            this.ratio_text = ratio_text;
            this._TestIndexRangeAll = testIndexRange;
            this.GenderId = genderId;

            foreach (var item in rdo)
            {
                if (item.GetType() == typeof(V_HIS_PATIENT_TYPE_ALTER))
                {
                    this._PatyAlterBhyt = (V_HIS_PATIENT_TYPE_ALTER)item;
                }
                else if (item.GetType() == typeof(HIS_TREATMENT))
                {
                    this._Treatment = (HIS_TREATMENT)item;
                }
                else if (item.GetType() == typeof(V_HIS_SERVICE_REQ))
                {
                    this._ServiceReq = (V_HIS_SERVICE_REQ)item;
                }
            }
        }

        public Mps000014PDO(object[] rdo, List<SereServNumOder> _sereServNumOder, List<V_HIS_SERE_SERV_TEIN> lstSereServTein, decimal ratio_text, List<V_HIS_TEST_INDEX_RANGE> testIndexRange, long genderId, List<V_HIS_SERVICE> processCode)
        {
            this._SereServNumOder = _sereServNumOder;
            this._SereServTeins = lstSereServTein;
            this.ratio_text = ratio_text;
            this._TestIndexRangeAll = testIndexRange;
            this.GenderId = genderId;

            this.ProcessCode = processCode;

            foreach (var item in rdo)
            {
                if (item.GetType() == typeof(V_HIS_PATIENT_TYPE_ALTER))
                {
                    this._PatyAlterBhyt = (V_HIS_PATIENT_TYPE_ALTER)item;
                }
                else if (item.GetType() == typeof(HIS_TREATMENT))
                {
                    this._Treatment = (HIS_TREATMENT)item;
                }
                else if (item.GetType() == typeof(V_HIS_SERVICE_REQ))
                {
                    this._ServiceReq = (V_HIS_SERVICE_REQ)item;
                }
            }
        }


        public Mps000014PDO(object[] rdo, List<SereServNumOder> _sereServNumOder, List<V_HIS_SERE_SERV_TEIN> lstSereServTein, decimal ratio_text, List<V_HIS_TEST_INDEX_RANGE> testIndexRange, long genderId, List<V_HIS_SERVICE> processCode, HIS_DHST hisdhst_)
        {
            this._SereServNumOder = _sereServNumOder;
            this._SereServTeins = lstSereServTein;
            this.ratio_text = ratio_text;
            this._TestIndexRangeAll = testIndexRange;
            this.GenderId = genderId;
            this.hisdhst = hisdhst_;
            this.ProcessCode = processCode;

            foreach (var item in rdo)
            {
                if (item.GetType() == typeof(V_HIS_PATIENT_TYPE_ALTER))
                {
                    this._PatyAlterBhyt = (V_HIS_PATIENT_TYPE_ALTER)item;
                }
                else if (item.GetType() == typeof(HIS_TREATMENT))
                {
                    this._Treatment = (HIS_TREATMENT)item;
                }
                else if (item.GetType() == typeof(V_HIS_SERVICE_REQ))
                {
                    this._ServiceReq = (V_HIS_SERVICE_REQ)item;
                }
            }
        }

        public class TestSereServADO
        {
            public string SERVICE_NAME { get; set; }
            public string TEST_INDEX_UNIT_NAME { get; set; }
            public long TEST_INDEX_ID { get; set; }
            public string TEST_INDEX_RANGE { get; set; }
            public string VALUE { get; set; }
            public string VALUE_RANGE { get; set; }
            public string RESULT_CODE { get; set; }
            public string IS_IMPORTANT { get; set; }

            public decimal? VALUE_NUM { get; set; }
            public decimal? MIN_VALUE { get; set; }
            public decimal? MAX_VALUE { get; set; }
            public string HIGH_OR_LOW { get; set; }
            public string VALUE_HL { get; set; }

            public string RelationshipId { get; set; }
            public long PRINT_NUM_ORDER { get; set; }

            public decimal? WARNING_MIN_VALUE { get; set; }
            public decimal? WARNING_MAX_VALUE { get; set; }
            public string WARNING_DESCRIPTION { get; set; }
            public string WARNING_NOTE { get; set; }
            public long? TEST_INDEX_GROUP_ID { get; set; }
            public string TEST_INDEX_GROUP_CODE { get; set; }
            public string TEST_INDEX_GROUP_NAME { get; set; }
            public string MACHINE_CODE { get; set; }
            public string MACHINE_NAME { get; set; }
            public string MACHINE_GROUP_CODE { get; set; }

            public string PROCESS_CODE { get; set; }

            public string NOTE { get; set; }

            public string LEAVEN { get; set; }
        }
    }
    public class SereServNumOder : HIS_SERE_SERV
    {
        public long SERVICE_NUM_ODER { get; set; }
        public long? HisService_MIN_PROCESS_TIME { get; set; }
        public long? HisService_MAX_PROCESS_TIME { get; set; }
        public long? HisService_MAX_TOTAL_PROCESS_TIME { get; set; }
        public long? ServiceParentId { get; set; }
        public long ServiceParentOrder { get; set; }
        public string ServiceParentCode { get; set; }
        public string ServiceParentName { get; set; }
        public long? GrandParentID { get; set; }

        public SereServNumOder() { }

        public SereServNumOder(HIS_SERE_SERV data, List<HIS_SERVICE> _Services)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServNumOder>(this, data);
                    HIS_SERVICE numOder = _Services.FirstOrDefault(p => p.ID == data.SERVICE_ID);
                    this.SERVICE_NUM_ODER = numOder != null ? (numOder.NUM_ORDER ?? 0) : 99999;

                    if (numOder != null && numOder.PARENT_ID.HasValue)
                    {
                        HIS_SERVICE parent = _Services.FirstOrDefault(p => p.ID == numOder.PARENT_ID.Value);
                        if (parent != numOder)
                        {
                            this.ServiceParentId = parent.ID;
                            this.ServiceParentCode = parent.SERVICE_CODE;
                            this.ServiceParentName = parent.SERVICE_NAME;
                            this.ServiceParentOrder = parent.NUM_ORDER ?? 0;
                            this.GrandParentID = parent.PARENT_ID;
                        }
                        else
                        {
                            ServiceParentOrder = 99999;
                        }
                    }
                    else
                    {
                        ServiceParentOrder = 99999;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public SereServNumOder(HIS_SERE_SERV data, List<V_HIS_SERVICE> _Services)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SereServNumOder>(this, data);
                    V_HIS_SERVICE numOder = _Services.FirstOrDefault(p => p.ID == data.SERVICE_ID);
                    this.SERVICE_NUM_ODER = numOder != null ? (numOder.NUM_ORDER ?? 0) : 99999;
                    this.HisService_MIN_PROCESS_TIME = numOder != null ? (numOder.MIN_PROCESS_TIME) : null;
                    this.HisService_MAX_PROCESS_TIME = numOder != null ? (numOder.MAX_PROCESS_TIME) : null;
                    this.HisService_MAX_TOTAL_PROCESS_TIME = numOder != null ? (numOder.MAX_TOTAL_PROCESS_TIME) : null;

                    if (numOder != null && numOder.PARENT_ID.HasValue)
                    {
                        V_HIS_SERVICE parent = _Services.FirstOrDefault(p => p.ID == numOder.PARENT_ID.Value);
                        if (parent != numOder)
                        {
                            this.ServiceParentId = parent.ID;
                            this.ServiceParentCode = parent.SERVICE_CODE;
                            this.ServiceParentName = parent.SERVICE_NAME;
                            this.ServiceParentOrder = parent.NUM_ORDER ?? 0;
                            this.GrandParentID = parent.PARENT_ID;
                        }
                        else
                        {
                            ServiceParentOrder = 99999;
                        }
                    }
                    else
                    {
                        ServiceParentOrder = 99999;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
    public class MLCTADO
    {
        public string EGFR { get; set; }
        public string CRCL { get; set; }
        public string UACR { get; set; }
        public string UPCR { get; set; }
    }
}
