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

namespace MPS.Processor.Mps000312.ADO
{
    class SereServADO : HIS_SERE_SERV
    {
        public long SERVICE_TYPE_ID { get; set; }
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public long? HEIN_SERVICE_TYPE_ID { get; set; }
        public string HEIN_SERVICE_TYPE_NAME { get; set; }
        public string HEIN_SERVICE_TYPE_CODE { get; set; }
        public long? HEIN_SERVICE_TYPE_NUM_ORDER { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string REQUEST_DEPARTMENT_CODE { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string HEIN_SERVICE_BHYT_CODE { get; set; }
        public string HEIN_SERVICE_BHYT_NAME { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }

        public int ROW_POS { get; set; }
        public decimal PRICE_BHYT { get; set; }
        public decimal TOTAL_PRICE_BHYT { get; set; }
        public decimal RADIO_SERIVCE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_ONE_AMOUNT { get; set; }
        public decimal? PRICE_CO_PAYMENT { get; set; }
        public decimal? PACKAGE_PRICE { get; set; }
        public long? HEIN_SERVICE_TYPE_CHILD_NUM_ORDER { get; set; }

        public short? IS_STENT { get; set; }

        public SereServADO(HIS_SERE_SERV data, List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes, List<V_HIS_SERVICE> services, List<V_HIS_ROOM> rooms, HIS_PATIENT_TYPE_ALTER patyAlter, List<HIS_DEPARTMENT> departments, List<HIS_MATERIAL_TYPE> materialTypes, List<HIS_SERVICE_UNIT> hisServiceUnit)
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
                                this.HEIN_SERVICE_TYPE_NAME = Inventec.Common.String.Convert.FirstCharToUpper((heinServiceTypeTT.HEIN_SERVICE_TYPE_NAME + ", " + heinServiceTypePTTT.HEIN_SERVICE_TYPE_NAME).ToLower());
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
                                this.HEIN_SERVICE_TYPE_NAME = Inventec.Common.String.Convert.FirstCharToUpper((heinServiceTypeMAU.HEIN_SERVICE_TYPE_NAME + ", " + heinServiceTypeCPM.HEIN_SERVICE_TYPE_NAME).ToLower());
                            }
                            else
                            {
                                this.HEIN_SERVICE_TYPE_ID = heinServiceType.ID;
                                this.HEIN_SERVICE_TYPE_NUM_ORDER = heinServiceType.NUM_ORDER;
                                this.HEIN_SERVICE_TYPE_CODE = heinServiceType.HEIN_SERVICE_TYPE_CODE;
                                this.HEIN_SERVICE_TYPE_NAME = heinServiceType.HEIN_SERVICE_TYPE_NAME;
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

                if (departments != null && departments.Count > 0)
                {
                    HIS_DEPARTMENT requestDepartment = departments.FirstOrDefault(o => o.ID == data.TDL_REQUEST_DEPARTMENT_ID);
                    if (requestDepartment != null)
                    {
                        this.REQUEST_DEPARTMENT_CODE = requestDepartment.DEPARTMENT_CODE;
                        this.REQUEST_DEPARTMENT_NAME = requestDepartment.DEPARTMENT_NAME;
                    }
                    HIS_DEPARTMENT executeDepartment = departments.FirstOrDefault(o => o.ID == data.TDL_EXECUTE_DEPARTMENT_ID);
                    if (executeDepartment != null)
                    {
                        this.EXECUTE_DEPARTMENT_CODE = executeDepartment.DEPARTMENT_CODE;
                        this.EXECUTE_DEPARTMENT_NAME = executeDepartment.DEPARTMENT_NAME;
                    }
                }

                if (materialTypes != null && materialTypes.Count > 0)
                {
                    HIS_MATERIAL_TYPE materialType = materialTypes.FirstOrDefault(o => o.SERVICE_ID == data.SERVICE_ID);
                    if (materialType != null && materialType.IS_STENT.HasValue)
                    {
                        this.IS_STENT = materialType.IS_STENT;
                    }
                }

                //dịch vụ khám, pttt, ptc gom theo khoa thực hiện
                if (this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH
                    || this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__PTTT
                    || this.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC)
                {
                    this.TDL_REQUEST_DEPARTMENT_ID = this.TDL_EXECUTE_DEPARTMENT_ID;
                    this.REQUEST_DEPARTMENT_NAME = this.EXECUTE_DEPARTMENT_NAME;
                }
                #endregion

                this.PRICE_BHYT = PriceBHYTProcess(data, this.IS_STENT, patyAlter);
                this.TOTAL_PRICE_BHYT = this.PRICE_BHYT * this.AMOUNT;
                if (this.VIR_TOTAL_HEIN_PRICE.HasValue)
                {
                    this.TOTAL_HEIN_PRICE_ONE_AMOUNT = this.VIR_TOTAL_HEIN_PRICE.Value / this.AMOUNT;
                }
                this.OTHER_SOURCE_PRICE = (this.OTHER_SOURCE_PRICE ?? 0) * this.AMOUNT;

                //nếu dịch vụ là gói thì cột này thể hiện số tiền gói - tiền thuốc, vật tư, dịch vụ trong gói - tiền bh thanh toán
                if (this.PACKAGE_ID.HasValue)
                {
                    this.VIR_PRICE = this.ADD_PRICE - PRICE_BHYT - (this.OTHER_SOURCE_PRICE ?? 0);
                    this.VIR_TOTAL_PRICE = this.VIR_PRICE * this.AMOUNT;
                }

                this.RADIO_SERIVCE = this.ORIGINAL_PRICE > 0 ? (this.HEIN_LIMIT_PRICE.HasValue ? (this.HEIN_LIMIT_PRICE.Value / this.ORIGINAL_PRICE) * 100 : (this.PRICE / this.ORIGINAL_PRICE) * 100) : 0;

                if (this.HEIN_LIMIT_PRICE.HasValue && this.HEIN_LIMIT_PRICE < this.VIR_PRICE)
                {
                    this.PRICE_CO_PAYMENT = this.VIR_PRICE - this.HEIN_LIMIT_PRICE.Value - (this.OTHER_SOURCE_PRICE ?? 0);
                }

                //có đơn vị quy đổi thì gán lại số lượng giá, đơn vị
                //Nếu trường này khác 1 thì xử lý như hiện tại, tức là: nếu có đơn vị chuyển đổi thì hiển thị theo đơn vị chuyển đổi, nếu ko có đơn vị chuyển đổi thì hiển thị theo đơn vị tính gốc
                var svUnit = hisServiceUnit.FirstOrDefault(o => o.ID == this.TDL_SERVICE_UNIT_ID);
                if (svUnit != null && svUnit.CONVERT_RATIO.HasValue && this.USE_ORIGINAL_UNIT_FOR_PRES != 1)
                {
                    var convertUnit = hisServiceUnit.FirstOrDefault(o => o.ID == svUnit.CONVERT_ID);
                    if (convertUnit != null)
                    {
                        this.SERVICE_UNIT_CODE = convertUnit.SERVICE_UNIT_CODE;
                        this.SERVICE_UNIT_NAME = convertUnit.SERVICE_UNIT_NAME;
                    }

                    this.AMOUNT = this.AMOUNT * svUnit.CONVERT_RATIO.Value;
                    this.PRICE = PRICE / svUnit.CONVERT_RATIO.Value;
                    this.PRIMARY_PRICE = (PRIMARY_PRICE ?? 0) / svUnit.CONVERT_RATIO.Value;
                    this.PRICE_BHYT = PRICE_BHYT / svUnit.CONVERT_RATIO.Value;
                    this.VIR_PRICE = VIR_PRICE / svUnit.CONVERT_RATIO.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this), this));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public SereServADO(SereServADO s)
        {
            try
            {
                System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<SereServADO>();
                foreach (var item in pi)
                {
                    item.SetValue(this, (item.GetValue(s)));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public decimal PriceBHYTProcess(HIS_SERE_SERV sereServ, short? IS_STENT, HIS_PATIENT_TYPE_ALTER patyAlter)
        {
            decimal priceBHYT = 0;
            try
            {
                if (patyAlter != null && !String.IsNullOrEmpty(sereServ.HEIN_CARD_NUMBER))
                {
                    if ((IS_STENT.HasValue && IS_STENT.Value == 1) || sereServ.HEIN_LIMIT_PRICE != null)
                        priceBHYT = sereServ.ORIGINAL_PRICE * (1 + sereServ.VAT_RATIO);
                    else
                    {

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
