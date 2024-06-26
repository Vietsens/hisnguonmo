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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.ADO.Bordereau
{
    public class SereServADO : V_HIS_SERE_SERV
    {
        public int ROW_POS { get; set; }

        public long? DEPARTMENT__HEIN_SERVICE_TYPE_ID { get; set; }
        public long? SERE_SERV__HEIN_SERVICE_TYPE_ID { get; set; }
        public long? DEPARTMENT__SERE_SERV { get; set; }

        public decimal PRICE_BHYT { get; set; }
        public decimal PRICE_FEE { get; set; }
        public decimal TOTAL_PRICE_FEE { get; set; }

        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal RADIO_SERIVCE { get; set; }

        public decimal? TOTAL_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_HEIN_PRICE_DEPARTMENT { get; set; }
        public decimal? TOTAL_PRICE_ROOM { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_ROOM { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ROOM { get; set; }

        public decimal? TOTAL_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }

        public decimal? TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }

        public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; } //bhyt chi tra cho so luong 1

        public decimal? TOTAL_PRICE_KTC_FEE_GROUP { get; set; }
        public decimal? TOTAL_HEIN_PRICE_FEE_GROUP { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_FEE_GROUP { get; set; }

        public decimal? TOTAL_PRICE_BHYT_FEE_GROUP { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_BHYT_FEE_GROUP { get; set; }

        public decimal? TOTAL_PRICE_EXECUTE_GROUP { get; set; }
        public decimal? TOTAL_HEIN_PRICE_EXECUTE_GROUP { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_EXECUTE_GROUP { get; set; }

        public decimal? TOTAL_PRICE_BHYT_EXECUTE_GROUP { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_BHYT_EXECUTE_GROUP { get; set; }

        public decimal? PRICE_POLICY { get; set; }

        //Tong nhom dich vu ky thuat cao
        public decimal TOTAL_PRICE_HIGHTECH { get; set; }
        public decimal TOTAL_HEIN_PRICE_HIGHTECH { get; set; }
        public decimal TOTAL_PATIENT_PRICE_HIGHTECH { get; set; }
        public decimal TOTAL_PRICE_HIGHTECH_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_HEIN_PRICE_HIGHTECH_HEIN_SERVICE_TYPE { get; set; }
        public decimal TOTAL_PATIENT_PRICE_HIGHTECH_HEIN_SERVICE_TYPE { get; set; }
        public decimal? PRICE_CO_PAYMENT { get; set; } //dong chi tra
        public string KTC_FEE_GROUP_NAME { get; set; }
        public string EXECUTE_GROUP_NAME { get; set; }

        public decimal? PRICE_DIFFERENCES_EXPEND { get; set; }
        public decimal? PRICE_DIFFERENCES_SERVICE_PATY { get; set; } //Chenh lenh gia với chính sách giá dịch vụ

        public long? ROOM_TYPE_ID { get; set; }
        public bool IS_EXAM { get; set; }

        public SereServADO()
        {

        }

        public SereServADO(SereServADO data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SereServADO>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                this.PRICE_BHYT = PriceBHYTProcess(data, patyAlter);
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                //Check lech dinh muc hao phi
                //if (this.MAX_EXPEND.HasValue)
                //{
                //    this.PRICE_DIFFERENCES_EXPEND = this.MAX_EXPEND.Value - this.VIR_TOTAL_PRICE;
                //}

                //Dong chi tra
                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE)
                {
                    this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, HIS_PATIENT_TYPE_ALTER patyAlter, List<V_HIS_ROOM> rooms)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                this.PRICE_BHYT = PriceBHYTProcess(data, patyAlter);
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }
                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                //Dong chi tra
                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE && this.HEIN_LIMIT_PRICE.Value > 0)
                {
                    this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value;
                }

                if (rooms != null && rooms.Count > 0)
                {
                    V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == this.TDL_REQUEST_ROOM_ID);
                    this.ROOM_TYPE_ID = room != null ? room.ROOM_TYPE_ID : 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (this.HEIN_LIMIT_PRICE != null && this.HEIN_LIMIT_PRICE > 0)
                {
                    this.PRICE_BHYT = (this.HEIN_LIMIT_PRICE ?? 0);
                    this.PRICE_FEE = (this.HEIN_LIMIT_PRICE ?? 0);
                }
                else
                {
                    this.PRICE_BHYT = this.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    this.PRICE_FEE = this.VIR_PRICE_NO_ADD_PRICE ?? 0;
                }

                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;
                this.TOTAL_PRICE_FEE = this.PRICE_FEE * this.AMOUNT;

                //Dong chi tra
                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE)
                {
                    this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, List<V_HIS_ROOM> rooms)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (this.HEIN_LIMIT_PRICE != null && this.HEIN_LIMIT_PRICE > 0)
                {
                    this.PRICE_BHYT = (this.HEIN_LIMIT_PRICE ?? 0);
                    this.PRICE_FEE = (this.HEIN_LIMIT_PRICE ?? 0);
                }
                else
                {
                    this.PRICE_BHYT = this.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    this.PRICE_FEE = this.VIR_PRICE_NO_ADD_PRICE ?? 0;
                }

                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;
                this.TOTAL_PRICE_FEE = this.PRICE_FEE * this.AMOUNT;

                //Dong chi tra
                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE)
                {
                    this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                if (rooms != null && rooms.Count > 0)
                {
                    V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == this.TDL_REQUEST_ROOM_ID);
                    this.ROOM_TYPE_ID = room != null ? room.ROOM_TYPE_ID : 0;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, List<V_HIS_SERVICE_PATY_PRPO> servicePatyPrpos)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (data.PRICE_POLICY_ID.HasValue)
                {
                    var servicePatyPrpo = servicePatyPrpos.Where(o => o.SERVICE_ID == data.SERVICE_ID && o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID && o.PRICE_POLICY_ID == data.PRICE_POLICY_ID).ToList();
                    if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                    {
                        this.PRICE_POLICY = servicePatyPrpo.FirstOrDefault().PRICE;
                    }
                }
                else
                {
                    this.PRICE_POLICY = 0;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(V_HIS_SERE_SERV data, List<V_HIS_SERVICE_PATY_PRPO> servicePatyPrpos, HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                this.PRICE_BHYT = PriceBHYTProcess(data, patyAlter);
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;

                if (this.VIR_TOTAL_HEIN_PRICE.HasValue && this.AMOUNT > 0)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                if (data.PRICE_POLICY_ID.HasValue)
                {
                    var servicePatyPrpo = servicePatyPrpos.Where(o => o.SERVICE_ID == data.SERVICE_ID && o.PATIENT_TYPE_ID == data.PATIENT_TYPE_ID && o.PRICE_POLICY_ID == data.PRICE_POLICY_ID).ToList();
                    if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                    {
                        this.PRICE_POLICY = servicePatyPrpo.FirstOrDefault().PRICE;
                    }
                }
                else
                {
                    this.PRICE_POLICY = 0;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public decimal PriceBHYTProcess(V_HIS_SERE_SERV sereServ, HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            decimal priceBHYT = 0;
            try
            {
                if (patyAlter != null && !String.IsNullOrEmpty(patyAlter.HEIN_CARD_NUMBER))
                {
                    if (sereServ.VIR_TOTAL_HEIN_PRICE == null || sereServ.VIR_TOTAL_HEIN_PRICE == 0)
                        priceBHYT = 0;
                    else
                    {
                        if (sereServ.HEIN_LIMIT_PRICE != null)
                            priceBHYT = sereServ.HEIN_LIMIT_PRICE.Value;
                        else
                            priceBHYT = sereServ.VIR_PRICE_NO_ADD_PRICE.Value;
                    }
                }
                else
                {
                    priceBHYT = 0;
                }
            }
            catch (Exception ex)
            {
                priceBHYT = 0;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return priceBHYT;
        }


    }
}
