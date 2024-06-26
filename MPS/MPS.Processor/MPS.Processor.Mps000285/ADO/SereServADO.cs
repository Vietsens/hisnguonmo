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
using MPS.Processor.Mps000285.PDO.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000285.ADO
{
    public class SereServADO : MPS.Processor.Mps000285.PDO.SereServKey
    {
        public long? HEIN_SERVICE_TYPE_CHILD_NUM_ORDER { get; set; }

        public SereServADO(HIS_SERE_SERV data, List<HIS_SERE_SERV_EXT> sereServExts, List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes, List<V_HIS_SERVICE> services, List<V_HIS_ROOM> rooms, List<HIS_MEDICINE_TYPE> medicineTypes, List<HIS_MEDICINE_LINE> medicineLines, List<HIS_MATERIAL_TYPE> materialTypes, PatientTypeCFG patientTypeCFG)
        {
            try
            {

                #region Base
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.HIS_SERE_SERV>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(data)));
                }

                if (heinServiceTypes != null && heinServiceTypes.Count > 0 && services != null && services.Count > 0)
                {
                    V_HIS_SERVICE service = services.FirstOrDefault(o => o.ID == data.SERVICE_ID);
                    if (service != null)
                    {
                        HIS_HEIN_SERVICE_TYPE heinServiceType = heinServiceTypes.FirstOrDefault(o => o.ID == service.HEIN_SERVICE_TYPE_ID);
                        this.SERVICE_TYPE_ID = service.SERVICE_TYPE_ID;
                        this.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                        this.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                        this.SERVICE_NAME = service.SERVICE_NAME;
                        this.SERVICE_CODE = service.SERVICE_CODE;

                        this.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        this.ACTIVE_INGR_BHYT_CODE = service.ACTIVE_INGR_BHYT_CODE;
                        this.ACTIVE_INGR_BHYT_NAME = service.ACTIVE_INGR_BHYT_NAME;
                        this.HEIN_SERVICE_BHYT_CODE = service.HEIN_SERVICE_BHYT_CODE;
                        this.HEIN_SERVICE_BHYT_NAME = service.HEIN_SERVICE_BHYT_NAME;
                        this.CONCENTRA = service.CONCENTRA;

                        //Set tong so film
                        HIS_SERE_SERV_EXT sereServExt = sereServExts != null ? sereServExts.Where(o => o.SERE_SERV_ID == data.ID).OrderByDescending(o => o.CREATE_TIME).FirstOrDefault() : null;
                        if (sereServExt != null && (sereServExt.NUMBER_OF_FILM ?? 0) > 0)
                            this.NUMBER_OF_FILM = sereServExt.NUMBER_OF_FILM;
                        else
                            this.NUMBER_OF_FILM = service.NUMBER_OF_FILM;

                        if (heinServiceType != null)
                        {
                            if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC
                                || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT
                                || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypePTTT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT);
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeTT = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TT);
                                this.HEIN_SERVICE_TYPE_ID = heinServiceTypePTTT.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypePTTT.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypePTTT.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceTypePTTT.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceTypeTT.HEIN_SERVICE_TYPE_NAME.First().ToString().ToUpper() + heinServiceTypeTT.HEIN_SERVICE_TYPE_NAME.ToLower().Substring(1) + ", " + heinServiceTypePTTT.HEIN_SERVICE_TYPE_NAME.ToLower();
                            }
                            else if (service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU
                                    || service.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM)
                            {
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeMAU = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__MAU);
                                HIS_HEIN_SERVICE_TYPE heinServiceTypeCPM = heinServiceTypes.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CPM);
                                this.HEIN_SERVICE_TYPE_ID = heinServiceTypeMAU.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceTypeMAU.VIR_PARENT_NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CHILD_NUM_ORDER = heinServiceTypeMAU.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceTypeMAU.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceTypeMAU.HEIN_SERVICE_TYPE_NAME.First().ToString().ToUpper() + heinServiceTypeMAU.HEIN_SERVICE_TYPE_NAME.ToLower().Substring(1) + ", " + heinServiceTypeCPM.HEIN_SERVICE_TYPE_NAME.ToLower();
                            }
                            else
                            {

                                this.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceType.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceType.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_NAME;
                            }
                        }

                        if (medicineTypes != null && medicineTypes.Count > 0 && medicineLines != null && medicineLines.Count > 0)
                        {
                            HIS_MEDICINE_TYPE medicineType = medicineTypes.FirstOrDefault(o => o.SERVICE_ID == this.SERVICE_ID);
                            if (medicineType != null && medicineType.MEDICINE_LINE_ID.HasValue)
                            {
                                HIS_MEDICINE_LINE medicineLine = medicineLines.FirstOrDefault(o => o.ID == medicineType.MEDICINE_LINE_ID);
                                if (medicineLine != null)
                                {
                                    this.MEDICINE_LINE_ID = medicineLine.ID;
                                    this.MEDICINE_LINE_CODE = medicineLine.MEDICINE_LINE_CODE;
                                    this.MEDICINE_LINE_NAME = medicineLine.MEDICINE_LINE_NAME;
                                }

                            }
                        }
                    }
                }

                if (rooms != null && rooms.Count > 0)
                {
                    V_HIS_ROOM room = rooms.FirstOrDefault(o => o.ID == data.TDL_EXECUTE_ROOM_ID);
                    if (room != null)
                    {
                        this.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                        this.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                    }
                }
                #endregion

                string keyPaty = "";
                this.PatientTypeAlter = PatientTypeAlterProcessor.GetPatientTypeAlter(data, patientTypeCFG, ref keyPaty);
                this.KEY_PATY_ALTER = keyPaty;

                this.PRICE_BHYT = 0;
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;

                if (this.ID == 923851)
                { 
                
                }
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE)
                {
                    this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value;
                }

                //Ty le thanh toan dich vu
                this.SERVICE_PAY_RATE = Math.Round(this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / (this.ORIGINAL_PRICE * (1 + this.VAT_RATIO))) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0, 0);
                //ty le thanh toan bao hiem

                this.BHYT_PAY_RATE = 0;
                this.VIR_TOTAL_PATIENT_PRICE_BHYT = 0;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal? GetBHYTPayRate(SereServADO s)
        {
            decimal? result = null;
            try
            {
                if (!s.HEIN_LIMIT_PRICE.HasValue || s.ORIGINAL_PRICE > s.HEIN_LIMIT_PRICE)
                {
                    result = 100;
                }
                else
                {
                    result = Math.Round((s.ORIGINAL_PRICE / s.HEIN_LIMIT_PRICE.Value) * 100, 0);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public decimal PriceBHYTProcess(SereServADO ss, List<HIS_MATERIAL_TYPE> materialTypes)
        {
            decimal priceBHYT = 0;
            try
            {
                //Get PATIENT_TYPE_ALTER

                if (!String.IsNullOrEmpty(ss.HEIN_CARD_NUMBER))
                {
                    if (ss.PatientTypeAlter != null)
                    {
                        if (this.VIR_TOTAL_HEIN_PRICE > 0)
                        {
                            if (ss.HEIN_LIMIT_PRICE.HasValue)
                                priceBHYT = ss.HEIN_LIMIT_PRICE.Value;
                            else
                            {
                                HIS_MATERIAL_TYPE materialType = materialTypes.FirstOrDefault(o =>
                                    o.SERVICE_ID == ss.SERVICE_ID
                                    && o.IS_STENT == 1);
                                priceBHYT = materialType != null ?
                                    ss.ORIGINAL_PRICE * (1 + ss.VAT_RATIO)
                                    : ss.VIR_PRICE_NO_ADD_PRICE.Value;
                            }
                        }
                        else
                        {
                            priceBHYT = 0;
                        }
                    }
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
