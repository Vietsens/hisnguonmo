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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestDetailBCS.ADO
{
    public class ExpMestMatyMetyReqSDODetail : V_HIS_EXP_MEST_MEDICINE
    {
        public bool IS_MEDICINE { get; set; }
        public long MEDI_MATE_TYPE_ID { get; set; }
        public long MEDI_MATE_ID { get; set; }

        public ExpMestMatyMetyReqSDODetail()
        {

        }

        public ExpMestMatyMetyReqSDODetail(ExpMestMatyMetyReqSDODetail medicine)
        {
            if (medicine != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ExpMestMatyMetyReqSDODetail>(this, medicine);
            }
        }

        public ExpMestMatyMetyReqSDODetail(V_HIS_EXP_MEST_MEDICINE _data)
        {
            try
            {
                if (_data != null)
                {
                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<V_HIS_EXP_MEST_MEDICINE>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }
                    this.IS_MEDICINE = true;
                    this.MEDI_MATE_TYPE_ID = _data.MEDICINE_TYPE_ID;
                    this.MEDI_MATE_ID = _data.MEDICINE_ID ?? 0;
                }
            }

            catch (Exception)
            {

            }
        }

        public ExpMestMatyMetyReqSDODetail(V_HIS_EXP_MEST_MATERIAL _data)
        {
            try
            {
                if (_data != null)
                {
                    this.AGGR_EXP_MEST_ID = _data.AGGR_EXP_MEST_ID;
                    this.AMOUNT = _data.AMOUNT;
                    this.APP_CREATOR = _data.APP_CREATOR;
                    this.APP_MODIFIER = _data.APP_MODIFIER;
                    this.APPROVAL_DATE = _data.APPROVAL_DATE;
                    this.APPROVAL_LOGINNAME = _data.APPROVAL_LOGINNAME;
                    this.APPROVAL_TIME = _data.APPROVAL_TIME;
                    this.APPROVAL_USERNAME = _data.APPROVAL_USERNAME;
                    this.BID_ID = _data.BID_ID;
                    this.BID_NAME = _data.BID_NAME;
                    this.BID_NUMBER = _data.BID_NUMBER;
                    this.CK_IMP_MEST_MEDICINE_ID = _data.CK_IMP_MEST_MATERIAL_ID;
                    this.CREATE_TIME = _data.CREATE_TIME;
                    this.CREATOR = _data.CREATOR;
                    this.DESCRIPTION = _data.DESCRIPTION;
                    this.DISCOUNT = _data.DISCOUNT;
                    this.EXP_DATE = _data.EXP_DATE;
                    this.EXP_LOGINNAME = _data.EXP_LOGINNAME;
                    this.EXP_MEST_CODE = _data.EXP_MEST_CODE;
                    this.EXP_MEST_ID = _data.EXP_MEST_ID;
                    this.EXP_MEST_METY_REQ_ID = _data.EXP_MEST_MATY_REQ_ID;
                    this.EXP_MEST_STT_ID = _data.EXP_MEST_STT_ID;
                    this.EXP_MEST_TYPE_ID = _data.EXP_MEST_TYPE_ID;
                    this.EXP_TIME = _data.EXP_TIME;
                    this.EXP_USERNAME = _data.EXP_USERNAME;
                    this.EXPIRED_DATE = _data.EXPIRED_DATE;
                    this.GROUP_CODE = _data.GROUP_CODE;
                    this.ID = _data.ID;
                    this.IMP_PRICE = _data.IMP_PRICE;
                    this.IMP_TIME = _data.IMP_TIME;
                    this.IMP_VAT_RATIO = _data.IMP_VAT_RATIO;
                    this.INTERNAL_PRICE = _data.INTERNAL_PRICE;
                    this.IS_ACTIVE = _data.IS_ACTIVE;
                    this.IS_DELETE = _data.IS_DELETE;
                    this.IS_EXPEND = _data.IS_EXPEND;
                    this.IS_EXPORT = _data.IS_EXPORT;
                    this.IS_OUT_PARENT_FEE = _data.IS_OUT_PARENT_FEE;
                    this.MANUFACTURER_CODE = _data.MANUFACTURER_CODE;
                    this.MANUFACTURER_ID = _data.MANUFACTURER_ID;
                    this.MANUFACTURER_NAME = _data.MANUFACTURER_NAME;
                    this.MATERIAL_NUM_ORDER = _data.MATERIAL_NUM_ORDER;
                    this.MEDICINE_TYPE_CODE = _data.MATERIAL_TYPE_CODE;
                    this.MEDICINE_TYPE_NAME = _data.MATERIAL_TYPE_NAME;
                    this.MEDICINE_TYPE_NUM_ORDER = _data.MATERIAL_TYPE_NUM_ORDER;
                    this.MEDI_STOCK_ID = _data.MEDI_STOCK_ID;
                    this.MEDI_STOCK_PERIOD_ID = _data.MEDI_STOCK_PERIOD_ID;
                    this.MEDICINE_NUM_ORDER = _data.MEDICINE_NUM_ORDER;
                    this.MEMA_GROUP_ID = _data.MEMA_GROUP_ID;
                    this.MODIFIER = _data.MODIFIER;
                    this.MODIFY_TIME = _data.MODIFY_TIME;
                    this.NATIONAL_NAME = _data.NATIONAL_NAME;
                    this.NUM_ORDER = _data.NUM_ORDER;
                    this.PACKAGE_NUMBER = _data.PACKAGE_NUMBER;
                    this.PATIENT_TYPE_CODE = _data.PATIENT_TYPE_CODE;
                    this.PATIENT_TYPE_ID = _data.PATIENT_TYPE_ID;
                    this.PATIENT_TYPE_NAME = _data.PATIENT_TYPE_NAME;
                    this.PRICE = _data.PRICE;
                    this.REQ_DEPARTMENT_ID = _data.REQ_DEPARTMENT_ID;
                    this.REQ_ROOM_ID = _data.REQ_ROOM_ID;
                    this.SERE_SERV_PARENT_ID = _data.SERE_SERV_PARENT_ID;
                    this.SERVICE_ID = _data.SERVICE_ID;
                    this.SERVICE_UNIT_CODE = _data.SERVICE_UNIT_CODE;
                    this.SERVICE_UNIT_ID = _data.SERVICE_UNIT_ID;
                    this.SERVICE_UNIT_NAME = _data.SERVICE_UNIT_NAME;
                    this.SUPPLIER_CODE = _data.SUPPLIER_CODE;
                    this.SUPPLIER_ID = _data.SUPPLIER_ID;
                    this.SUPPLIER_NAME = _data.SUPPLIER_NAME;
                    this.TDL_AGGR_EXP_MEST_ID = _data.TDL_AGGR_EXP_MEST_ID;
                    this.TDL_MEDICINE_TYPE_ID = _data.TDL_MATERIAL_TYPE_ID;
                    this.TDL_MEDI_STOCK_ID = _data.TDL_MEDI_STOCK_ID;
                    this.TH_AMOUNT = _data.TH_AMOUNT;
                    this.VAT_RATIO = _data.VAT_RATIO;
                    this.IS_MEDICINE = false;
                    this.MEDI_MATE_TYPE_ID = _data.MATERIAL_TYPE_ID;
                    this.MEDI_MATE_ID = _data.MATERIAL_ID ?? 0;
                    this.CONVERT_RATIO = _data.CONVERT_RATIO;
                    this.CONVERT_UNIT_CODE = _data.CONVERT_UNIT_CODE;
                    this.CONVERT_UNIT_NAME = _data.CONVERT_UNIT_NAME;
                }
            }

            catch (Exception)
            {

            }
        }
    }
}
