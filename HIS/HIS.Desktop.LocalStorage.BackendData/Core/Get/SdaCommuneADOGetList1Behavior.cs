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
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using SDA.EFMODEL.DataModels;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData.Get
{
    class SdaCommuneADOGetList1Behavior : BusinessBase, IGetDataT
    {
        internal SdaCommuneADOGetList1Behavior(CommonParam param)
            : base(param)
        {

        }

        object IGetDataT.Execute<T>()
        {
            try
            {
                List<ADO.CommuneADO> result = new List<ADO.CommuneADO>();
                var communes = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var districts = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var provinces = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (communes != null && communes.Count > 0
                    && districts != null && districts.Count > 0
                    && provinces != null && provinces.Count > 0
                    )
                {
                    var queryCommunes = (from c in communes
                                         select new ADO.CommuneADO(c, districts, provinces)).ToList();

                    result.AddRange(queryCommunes);
                    foreach (var dis in districts)
                    {
                        var province = provinces.FirstOrDefault(o => o.ID == dis.PROVINCE_ID);

                        ADO.CommuneADO comEmpty = new ADO.CommuneADO();
                        comEmpty.ID_RAW = "D" + -dis.ID;
                        comEmpty.ID = -dis.ID;
                        comEmpty.SEARCH_CODE_COMMUNE = dis.SEARCH_CODE + (province != null ? province.SEARCH_CODE : "");
                        comEmpty.DISTRICT_ID = dis.ID;
                        comEmpty.DISTRICT_CODE = dis.DISTRICT_CODE;
                        comEmpty.DISTRICT_NAME = dis.DISTRICT_NAME;
                        comEmpty.PROVINCE_ID = dis.PROVINCE_ID;
                        comEmpty.PROVINCE_CODE = dis.PROVINCE_CODE;
                        comEmpty.PROVINCE_NAME = dis.PROVINCE_NAME;
                        comEmpty.IS_ACTIVE = dis.IS_ACTIVE;
                        comEmpty.IS_DELETE = dis.IS_DELETE;

                        result.Add(comEmpty);
                    }

                    foreach (var item in result)
                    {
                        string x1 = (String.IsNullOrEmpty(item.COMMUNE_NAME) ? "" : "" + item.INITIAL_NAME + " " + item.COMMUNE_NAME);
                        string h1 = (String.IsNullOrEmpty(item.DISTRICT_INITIAL_NAME) ? "" : (String.IsNullOrEmpty(x1) ? "" : " - ") + item.DISTRICT_INITIAL_NAME) + (String.IsNullOrEmpty(item.DISTRICT_NAME) ? "" : " " + item.DISTRICT_NAME);
                        string t1 = (String.IsNullOrEmpty(item.PROVINCE_NAME) ? "" : " - " + item.PROVINCE_NAME);
                        item.RENDERER_PDC_NAME = string.Format("{0}{1}{2}", x1, h1, t1);
                        item.RENDERER_PDC_NAME_UNSIGNED = Inventec.Common.String.Convert.UnSignVNese2(item.RENDERER_PDC_NAME);
                    }
                }

                return result.Where(o=> (string.IsNullOrEmpty(o.COMMUNE_CODE) || communes.Exists(p=>p.COMMUNE_CODE.Equals(o.COMMUNE_CODE))) && (string.IsNullOrEmpty(o.DISTRICT_CODE) || districts.Exists(p=>p.DISTRICT_CODE == o.DISTRICT_CODE)) && (string.IsNullOrEmpty(o.PROVINCE_CODE) || provinces.Exists(p=>p.PROVINCE_CODE == o.PROVINCE_CODE))).OrderBy(o => o.SEARCH_CODE_COMMUNE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
