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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ImportBlood.ADO
{
    public class HisBloodGiverADO : HIS_BLOOD_GIVER
    {
        public string DOB_ForDisplay { get; set; }
        public string GENDER_ForDisplay { get; set; }
        public string VirAddress_ID_RAW { get; set; }

        //prop for Import
        public string DOB_str { get; set; }
        public string GENDER_NAME { get; set; }
        public string WORK_PLACE_CODE { get; set; }
        public string CAREER_CODE { get; set; }
        public string EXAM_TIME_str { get; set; }
        public string CMND_CCCD_NUMBER { get; set; }
        public string CMND_CCCD_DATE { get; set; }
        public long? CMND_CCCD_DATE_number { get; set; }
        public string CMND_CCCD_PLACE { get; set; }
        public string IS_WORK_PLACE { get; set; }
        public string IS_VIR_ADDRESS { get; set; }
        public decimal? VOLUME { get; set; }
        public string EXECUTE_TIME_str { get; set; }
        public string BLOOD_ABO_CODE { get; set; }
        public string BLOOD_RH_CODE { get; set; }
        

        public List<string> ErrorDescriptions = new List<string>();
        public string ErrorDesc { get; set; }

        public HisBloodGiverADO()
        {
        }

        public HisBloodGiverADO(HIS_BLOOD_GIVER data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisBloodGiverADO>(this, data);
                    this.DOB_ForDisplay = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                    this.GENDER_ForDisplay = BackendDataWorker.Get<HIS_GENDER>().Where(o => o.ID == data.GENDER_ID).Select(o => o.GENDER_NAME).FirstOrDefault();
                    this.VirAddress_ID_RAW = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().Where(o => o.RENDERER_PDC_NAME != null && o.RENDERER_PDC_NAME == data.VIR_ADDRESS).Select(o=>o.ID_RAW).SingleOrDefault();
                    if (String.IsNullOrEmpty(this.VirAddress_ID_RAW))
                    {
                        var province = BackendDataWorker.Get<SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE != null && o.PROVINCE_CODE.ToUpper() == (data.PROVINCE_CODE ?? "").ToUpper());
                        var district = BackendDataWorker.Get<SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE != null && o.DISTRICT_CODE.ToUpper() == (data.DISTRICT_CODE ?? "").ToUpper());
                        var commune = BackendDataWorker.Get<SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE != null && o.COMMUNE_CODE.ToUpper() == (data.COMMUNE_CODE ?? "").ToUpper());
                        if (commune == null)
                        {
                            var dataNoCommunes = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().Where(o => o.ID < 0).ToList();
                            if (district != null && province != null)
                            {
                                var dataNoCommunes_Result = dataNoCommunes.Where(o => o.DISTRICT_CODE != null && o.DISTRICT_CODE == district.DISTRICT_CODE
                                                                                    && district.PROVINCE_ID == province.ID).FirstOrDefault();
                                if (dataNoCommunes_Result != null)
                                {
                                    this.VirAddress_ID_RAW = dataNoCommunes_Result.ID_RAW;
                                }
                            }
                        }
                        else
                        {
                            var communeADO = BackendDataWorker.Get<HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO>().SingleOrDefault(o => o.COMMUNE_CODE != null && o.COMMUNE_CODE == commune.COMMUNE_CODE);
                            if (communeADO != null)
                            {
                                this.VirAddress_ID_RAW = communeADO.ID_RAW;
                            }
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
