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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.SdaConfigKey;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DepositRequest.DepositRequest
{
    public static class DepositServicePrintProcess
    {
        static int iPatientTypeIsNotBHYT = -1;
        //public static List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> HisSereServ_Bordereaus;

        //internal List<V_HIS_SERE_SERV> ProcessGetSereServ(long patientTypeId, bool? isExpend, long treatmentId, List<long> sereServIds)
        //{
        //    List<V_HIS_SERE_SERV> SereServAlls = new List<V_HIS_SERE_SERV>();
        //    CommonParam param = new CommonParam();
        //    try
        //    {
        //        var sereServ_Bordereaus = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
        //        List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> HisSereServ_Bordereaus = new List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
        //        //Lay tat ca du lieu can xu ly
        //        MOS.Filter.HisSereServViewFilter hisSereServFilter = new HisSereServViewFilter();
        //        hisSereServFilter.TREATMENT_ID = treatmentId;
        //        hisSereServFilter.IDs = sereServIds;
        //        if (patientTypeId != iPatientTypeIsNotBHYT && patientTypeId != 0)
        //        {
        //            hisSereServFilter.PATIENT_TYPE_ID = patientTypeId;
        //        }

        //        sereServ_Bordereaus = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW, ApiConsumers.MosConsumer, hisSereServFilter, param);

        //        if (sereServ_Bordereaus != null && sereServ_Bordereaus.Count > 0)
        //        {
        //            //Kiem tra va xu ly du lieu ton tai
        //            ProcessExistsData(sereServ_Bordereaus, patientTypeId, ref HisSereServ_Bordereaus);
        //            var query = HisSereServ_Bordereaus.AsQueryable();
        //            if (patientTypeId == 0)//Loc du lieu theo doi tuong
        //            {
        //                query = query.Where(o => o.AMOUNT > 0);
        //            }
        //            else
        //            {
        //                var patientTypeCFG = Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(ExtensionConfigKey.SDA_CONFIG__PATIENT_TYPE_CODE__BHYT);
        //                var patientTypeBhyt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == patientTypeCFG);
        //                query = query.Where(o => o.AMOUNT > 0
        //                     && (patientTypeId != iPatientTypeIsNotBHYT || o.PATIENT_TYPE_ID != patientTypeBhyt.ID));
        //            }

        //            //Check hao phí
        //            if (isExpend == false)
        //            {
        //                query = query.Where(o => o.IS_EXPEND == null);
        //            }

        //            HisSereServ_Bordereaus = query.ToList();

        //            SereServAlls.AddRange(HisSereServ_Bordereaus);
        //        }
        //        return SereServAlls;
        //    }
        //    catch (Exception)
        //    {
        //        return new List<V_HIS_SERE_SERV>();
        //        throw;
        //    }
        //}

        private void ProcessExistsData(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServ_Bordereaus, long patientTypeId, ref List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV> sereServ_BordereausResult)
        {
            try
            {
                //HisSereServ_Bordereaus = new List<V_HIS_SERE_SERV>();


                var sereServGroups = sereServ_Bordereaus.GroupBy(o => new { o.SERVICE_ID, o.VIR_PRICE, o.PARENT_ID, o.IS_OUT_PARENT_FEE, o.PATIENT_TYPE_ID, o.IS_EXPEND, o.REQUEST_DEPARTMENT_ID });
                foreach (var sereServGroup in sereServGroups)
                {
                    V_HIS_SERE_SERV sereServ = sereServGroup.FirstOrDefault();
                    sereServ.VIR_TOTAL_PRICE = sereServGroup.Sum(o => o.VIR_TOTAL_PRICE);
                    sereServ.AMOUNT = sereServGroup.Sum(o => o.AMOUNT);
                    sereServ_BordereausResult.Add(sereServ);
                }


                //foreach (var item in sereServ_Bordereaus)
                //{
                //    var sereServExist = HisSereServ_Bordereaus.SingleOrDefault(o =>
                //                                o.SERVICE_ID == item.SERVICE_ID
                //        && o.VIR_PRICE == item.VIR_PRICE
                //        && o.PARENT_ID == item.PARENT_ID
                //        && o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                //        && o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                //        && o.IS_EXPEND == item.IS_EXPEND
                //        && o.REQUEST_DEPARTMENT_ID == item.REQUEST_DEPARTMENT_ID);// nếu gom nhóm theo khoa thì cộng dồn cac dv cùng khoa
                //    if (sereServExist != null && sereServExist.ID > 0)
                //    {
                //        sereServExist.AMOUNT += item.AMOUNT;

                //        sereServExist.VIR_TOTAL_PRICE = sereServExist.VIR_TOTAL_PRICE ?? 0;
                //        sereServExist.VIR_TOTAL_HEIN_PRICE = sereServExist.VIR_TOTAL_HEIN_PRICE ?? 0;
                //        sereServExist.VIR_TOTAL_PRICE_NO_EXPEND = sereServExist.VIR_TOTAL_PRICE_NO_EXPEND ?? 0;
                //        sereServExist.VIR_TOTAL_PATIENT_PRICE = sereServExist.VIR_TOTAL_PATIENT_PRICE ?? 0;
                //        sereServExist.VIR_TOTAL_PRICE_NO_ADD_PRICE += sereServExist.VIR_TOTAL_PRICE_NO_ADD_PRICE ?? 0;
                //        sereServExist.VIR_TOTAL_PRICE += item.VIR_TOTAL_PRICE ?? 0;
                //        sereServExist.VIR_TOTAL_HEIN_PRICE += item.VIR_TOTAL_HEIN_PRICE ?? 0;
                //        sereServExist.VIR_TOTAL_PRICE_NO_EXPEND += item.VIR_TOTAL_PRICE_NO_EXPEND ?? 0;
                //        sereServExist.VIR_TOTAL_PATIENT_PRICE += item.VIR_TOTAL_PATIENT_PRICE ?? 0;
                //    }
                //    else
                //    {
                //        sereServ_BordereausResult.Add(item);
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<SereServGroupPlusADO> sereServADOs = new List<SereServGroupPlusADO>();

            try
            {
                foreach (var item in sereServs)
                {

                    SereServGroupPlusADO sereServADO = new SereServGroupPlusADO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>();
                    sereServADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV, SereServGroupPlusADO>(item);
                    var patientTypeCFG = Inventec.Common.LocalStorage.SdaConfig.SdaConfigs.Get<string>(ExtensionConfigKey.SDA_CONFIG__PATIENT_TYPE_CODE__BHYT);
                    if (sereServADO.PATIENT_TYPE_CODE != patientTypeCFG)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

    }
}
