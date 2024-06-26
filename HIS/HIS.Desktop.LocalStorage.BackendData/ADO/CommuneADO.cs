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

namespace HIS.Desktop.LocalStorage.BackendData.ADO
{
    public class CommuneADO : SDA.EFMODEL.DataModels.V_SDA_COMMUNE
    {
        public string RENDERER_PDC_NAME { get; set; }
        public string RENDERER_PDC_NAME_UNSIGNED { get; set; }
        public string PROVINCE_CODE { get; set; }
        public long PROVINCE_ID { get; set; }
        public string PROVINCE_NAME { get; set; }
        public string SEARCH_CODE_COMMUNE { get; set; }
        public string ID_RAW { get; set; }

        public CommuneADO()
        {

        }

        //public string CODE_PATH { get; set; }
        //public string COMMUNE_CODE { get; set; }
        //public string COMMUNE_NAME { get; set; }
        //public long? CREATE_TIME { get; set; }
        //public string CREATOR { get; set; }
        //public string DISTRICT_CODE { get; set; }
        //public long DISTRICT_ID { get; set; }
        //public string DISTRICT_INITIAL_NAME { get; set; }
        //public string DISTRICT_NAME { get; set; }
        //public string GROUP_CODE { get; set; }
        //public long ID { get; set; }
        //public string ID_PATH { get; set; }
        //public string INITIAL_NAME { get; set; }
        //public short? IS_ACTIVE { get; set; }
        //public short? IS_DELETE { get; set; }
        //public string MODIFIER { get; set; }
        //public long? MODIFY_TIME { get; set; }
        //public string SEARCH_CODE { get; set; }


        public CommuneADO(SDA.EFMODEL.DataModels.V_SDA_COMMUNE data, List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> districts, List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> provinces)
        {
            try
            {
                //Inventec.Common.Mapper.DataObjectMapper.Map<CommuneADO>(this, data);

                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.ID = data.ID;
                this.ID_RAW = "C" + data.ID;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.CODE_PATH = data.CODE_PATH;
                this.COMMUNE_CODE = data.COMMUNE_CODE;
                this.COMMUNE_NAME = data.COMMUNE_NAME;
                this.DISTRICT_CODE = data.DISTRICT_CODE;
                this.DISTRICT_ID = data.DISTRICT_ID;
                this.DISTRICT_INITIAL_NAME = data.DISTRICT_INITIAL_NAME;
                this.DISTRICT_NAME = data.DISTRICT_NAME;
                this.ID_PATH = data.ID_PATH;
                this.INITIAL_NAME = data.INITIAL_NAME;
                this.SEARCH_CODE = data.SEARCH_CODE;

                this.SEARCH_CODE_COMMUNE += data.SEARCH_CODE;
                var districtOne = districts.FirstOrDefault(o => o.ID == data.DISTRICT_ID);
                if (districtOne != null)
                {
                    this.PROVINCE_ID = districtOne.PROVINCE_ID;
                    this.PROVINCE_CODE = districtOne.PROVINCE_CODE;
                    this.PROVINCE_NAME = districtOne.PROVINCE_NAME;
                    this.SEARCH_CODE_COMMUNE += districtOne.SEARCH_CODE;
                    var provinceOne = provinces.FirstOrDefault(o => o.ID == districtOne.PROVINCE_ID);
                    if (provinceOne != null)
                    {
                        this.SEARCH_CODE_COMMUNE += provinceOne.SEARCH_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public CommuneADO(List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> provinces, List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> districts, SDA.EFMODEL.DataModels.V_SDA_COMMUNE data)
        {
            try
            {
                this.CREATE_TIME = data.CREATE_TIME;
                this.CREATOR = data.CREATOR;
                this.GROUP_CODE = data.GROUP_CODE;
                this.ID = data.ID;
                this.ID_RAW = "C" + data.ID;
                this.IS_ACTIVE = data.IS_ACTIVE;
                this.IS_DELETE = data.IS_DELETE;
                this.CODE_PATH = data.CODE_PATH;
                this.COMMUNE_CODE = data.COMMUNE_CODE;
                this.COMMUNE_NAME = data.COMMUNE_NAME;
                this.DISTRICT_CODE = data.DISTRICT_CODE;
                this.DISTRICT_ID = data.DISTRICT_ID;
                this.DISTRICT_INITIAL_NAME = data.DISTRICT_INITIAL_NAME;
                this.DISTRICT_NAME = data.DISTRICT_NAME;
                this.ID_PATH = data.ID_PATH;
                this.INITIAL_NAME = data.INITIAL_NAME;
                this.SEARCH_CODE = data.SEARCH_CODE;

                var districtOne = districts.FirstOrDefault(o => o.ID == data.DISTRICT_ID);
                if (districtOne != null)
                {
                    this.PROVINCE_ID = districtOne.PROVINCE_ID;
                    this.PROVINCE_CODE = districtOne.PROVINCE_CODE;
                    this.PROVINCE_NAME = districtOne.PROVINCE_NAME;

                    var provinceOne = provinces.FirstOrDefault(o => o.ID == districtOne.PROVINCE_ID);
                    if (provinceOne != null)
                    {
                        this.SEARCH_CODE_COMMUNE += provinceOne.SEARCH_CODE;
                    }
                    this.SEARCH_CODE_COMMUNE += districtOne.SEARCH_CODE;
                }
                this.SEARCH_CODE_COMMUNE += data.SEARCH_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
