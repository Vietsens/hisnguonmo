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
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;


namespace HIS.Desktop.Plugins.TreatmentList.ADO
{
    class ExcellDataADO : V_HIS_TREATMENT_4
    {
        public string WORK_PLACE_NAME { get; set; }
        public string EXAM_STOMATOLOGY { get; set; }
        public string EXAM_ENT { get; set; }
        public string DISEASES { get; set; }
        public string EXAM_MUSCLE_BONE { get; set; }
        public string EXAM_DERMATOLOGY { get; set; }
        public string EXAM_RESPIRATORY { get; set; }
        public string TREATMENT_INSTRUCTION { get; set; }
        public string NOTE_SUPERSONIC { get; set; }
        public string NOTE_XRAY { get; set; }
        public string EXAM_EYE { get; set; }
        public string EXAM_SURGERY { get; set; }
        public string NOTE_BLOOD { get; set; }
        public string NOTE_BIOCHEMICAL { get; set; }
        public string EXAM_OEND { get; set; }
        public string NOTE_PROSTASE { get; set; }
        public string EXAM_MENTAL { get; set; }
        public string EXAM_NEUROLOGICAL { get; set; }
        public string EXAM_KIDNEY_UROLOGY { get; set; }
        public string EXAM_DIGESTION { get; set; }
        public string EXAM_CIRCULATION { get; set; }
        public string TDL_PATIENT_DOB_MEN { get; set; }
        public string TDL_PATIENT_DOB_WOM { get; set; }
        public string HEIGH_RANK_NAME { get; set; }
        public string EXAM_CONCLUSION { get; set; }
        public string CONCLUSION { get; set; }
        public decimal? HEIGHT { get; set; }
        public decimal? WEIGHT { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? PULSE { get; set; }
        public decimal? BLOOD_PRESSURE_MAX { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public string TDL_PATIENT_POSITION_NAME { get; set; }
        public ExcellDataADO(V_HIS_TREATMENT_4 data)
        {
            CommonParam paramCommon = new CommonParam();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_4>(this, data);
            if (data.TDL_PATIENT_GENDER_ID == 1)
            {
                TDL_PATIENT_DOB_WOM = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
            }
            else
            {
                TDL_PATIENT_DOB_MEN = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
            }
            if (data.TDL_PATIENT_POSITION_ID != null)
            {
                TDL_PATIENT_POSITION_NAME = BackendDataWorker.Get<HIS_POSITION>().FirstOrDefault(o => o.ID == data.TDL_PATIENT_POSITION_ID).POSITION_NAME;
            }
            if (data.TDL_KSK_CONTRACT_ID != null)
            {
                HisKskContractViewFilter filter = new HisKskContractViewFilter();
                filter.ID = data.TDL_KSK_CONTRACT_ID;
                var dataKskContract = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_KSK_CONTRACT>>("api/HisKskContract/GetView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (dataKskContract != null && dataKskContract.Count > 0)
                {
                    WORK_PLACE_NAME = dataKskContract.FirstOrDefault().WORK_PLACE_NAME;
                }
            }

            var dataSr = GetServiceReq(data.ID);
            if (dataSr != null && dataSr.Count > 0)
            {
                HisKskGeneralFilter GFilter = new HisKskGeneralFilter();
                GFilter.SERVICE_REQ_ID = dataSr.OrderBy(o => o.INTRUCTION_TIME).FirstOrDefault().ID;
                var dataKskGenaral = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_KSK_GENERAL>>("api/HisKskGeneral/Get", ApiConsumers.MosConsumer, GFilter, paramCommon);
                if (dataKskGenaral != null && dataKskGenaral.Count > 0)
                {
                    HIS_KSK_GENERAL currentGenaral = dataKskGenaral.FirstOrDefault();
                    EXAM_STOMATOLOGY = currentGenaral.EXAM_STOMATOLOGY;
                    EXAM_ENT = currentGenaral.EXAM_ENT;
                    DISEASES = currentGenaral.DISEASES;
                    EXAM_MUSCLE_BONE = currentGenaral.EXAM_MUSCLE_BONE;
                    EXAM_DERMATOLOGY = currentGenaral.EXAM_DERMATOLOGY;
                    EXAM_RESPIRATORY = currentGenaral.EXAM_RESPIRATORY;
                    TREATMENT_INSTRUCTION = currentGenaral.TREATMENT_INSTRUCTION;
                    NOTE_SUPERSONIC = currentGenaral.NOTE_SUPERSONIC;
                    NOTE_XRAY = currentGenaral.NOTE_SUPERSONIC;
                    EXAM_EYE = currentGenaral.EXAM_EYE;
                    EXAM_SURGERY = currentGenaral.EXAM_EYE;
                    NOTE_BLOOD = currentGenaral.NOTE_BLOOD;
                    NOTE_BIOCHEMICAL = currentGenaral.NOTE_BIOCHEMICAL;
                    EXAM_OEND = currentGenaral.EXAM_OEND;
                    NOTE_PROSTASE = currentGenaral.NOTE_PROSTASE;
                    EXAM_MENTAL = currentGenaral.EXAM_MENTAL;
                    EXAM_NEUROLOGICAL = currentGenaral.EXAM_NEUROLOGICAL;
                    EXAM_KIDNEY_UROLOGY = currentGenaral.EXAM_KIDNEY_UROLOGY;
                    EXAM_DIGESTION = currentGenaral.EXAM_DIGESTION;
                    EXAM_CIRCULATION = currentGenaral.EXAM_CIRCULATION;
                    if (currentGenaral.HEALTH_EXAM_RANK_ID != null)
                    {
                        var heighRank = BackendDataWorker.Get<HIS_HEALTH_EXAM_RANK>().FirstOrDefault(o => o.ID == currentGenaral.HEALTH_EXAM_RANK_ID);
                        HEIGH_RANK_NAME = heighRank.HEALTH_EXAM_RANK_NAME;
                    }
                    if (currentGenaral.DHST_ID != null)
                    {
                        HisDhstFilter DFilter = new HisDhstFilter();
                        DFilter.ID = currentGenaral.DHST_ID;
                        var dataDhst = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_DHST>>("api/HisDhst/Get", ApiConsumers.MosConsumer, DFilter, paramCommon);
                        if (dataDhst != null && dataDhst.Count > 0)
                        {
                            HIS_DHST currentDhst = dataDhst.FirstOrDefault();
                            HEIGHT = currentDhst.HEIGHT;
                            WEIGHT = currentDhst.WEIGHT;
                            VIR_BMI = currentDhst.VIR_BMI;
                            PULSE = currentDhst.PULSE;
                            BLOOD_PRESSURE_MAX = currentDhst.BLOOD_PRESSURE_MAX;
                            TEMPERATURE = currentDhst.TEMPERATURE;
                            BREATH_RATE = currentDhst.BREATH_RATE;
                        }

                    }
                }
                CONCLUSION = dataSr.OrderByDescending(o => o.INTRUCTION_TIME).FirstOrDefault().CONCLUSION;
                EXAM_CONCLUSION = dataSr.OrderByDescending(o => o.INTRUCTION_TIME).FirstOrDefault().EXAM_CONCLUSION;
            }
        }
        private List<V_HIS_SERVICE_REQ> GetServiceReq(long treatmentId)
        {
            List<V_HIS_SERVICE_REQ> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                filter.HAS_EXECUTE = true;
                filter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }


    }
}
