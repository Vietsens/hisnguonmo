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
using MOS.SDO;
using MPS.ProcessorBase;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000132.PDO
{
    public partial class Mps000132PDO : RDOBase
    {
        public V_HIS_MEDI_STOCK_PERIOD mediStock { get; set; }
        public List<V_HIS_MEST_PERIOD_METY> lstMestPeriodMety { get; set; }
        public List<V_HIS_MEST_PERIOD_MATY> lstMestPeriodMaty { get; set; }
        public List<V_HIS_MEST_PERIOD_MEDI> lstMestPeriodMedi { get; set; }
        public List<V_HIS_MEST_PERIOD_MATE> lstMestPeriodMate { get; set; }
        public List<V_HIS_MEST_PERIOD_BLTY> lstMestPeriodBlty { get; set; }
        public List<HIS_MEDICINE_GROUP> lstMedicinGroup { get; set; }
        public List<Mps000132ADO> listMrs000132ADO;
        public List<V_HIS_MEST_INVE_USER> listMestInveUser { get; set; }
        public List<HIS_MEDI_STOCK_METY> ListMediStockMety { get; set; }
        public List<HIS_MEDI_STOCK_MATY> ListMediStockMaty { get; set; }
        public List<long> medistockIdList { get; set; }
    }

    public class Mps000132ADO
    {
        public string MEDI_MATE_TYPE_NAME { get; set; }
        public string MEDI_MATE_TYPE_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public decimal? IMP_PRICE { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string RECORDING_TRANSACTION { get; set; }
        public string CONCENTRA { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal OUT_AMOUNT { get; set; }
        public decimal? VIR_END_AMOUNT { get; set; }
        public decimal INVENTORY_AMOUNT { get; set; }
        public decimal BEGIN_AMOUNT { get; set; }
        public decimal? VOLUME { get; set; }
        public long TYPE { get; set; }
        public long? PARENT_ID { get; set; }
        public long MEDI_MATE_ID { get; set; }
        public decimal? SL_THUA { get; set; }
        public decimal? SL_THIEU { get; set; }
        public decimal? TIEN_THUA { get; set; }
        public decimal? TIEN_THIEU { get; set; }
        public decimal? ALERT_MAX_IN_STOCK { get; set; }
        public decimal IMP_VAT_RATIO { get; set; }
        public decimal? IN_AMOUNT { get; set; }
        public long METY_MATY_ID { get; set; }


        public Mps000132ADO() { }

        public Mps000132ADO(V_HIS_MEST_PERIOD_MEDI data)
        {
            try
            {
                if (data != null)
                {
                    this.TYPE = 1;
                    this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = data.NATIONAL_NAME;
                    //this.CONCENTRA = data.CONCENTRA;
                    this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.VIR_END_AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.INVENTORY_AMOUNT = data.INVENTORY_AMOUNT ?? 0;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.IMP_VAT_RATIO = data.IMP_VAT_RATIO;
                    this.METY_MATY_ID = data.MEDICINE_TYPE_ID;
                    this.IN_AMOUNT = data.IN_AMOUNT;
                    this.EXPIRED_DATE_STR = data.EXPIRED_DATE.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0) : "";
                    this.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                    this.PARENT_ID = data.PARENT_ID;
                    this.MEDI_MATE_ID = data.MEDICINE_ID;
                    if ((data.INVENTORY_AMOUNT ?? 0) >= (data.VIR_END_AMOUNT ?? 0))
                    {
                        this.SL_THIEU = (data.INVENTORY_AMOUNT ?? 0) - (data.VIR_END_AMOUNT ?? 0);
                        this.TIEN_THIEU = this.SL_THIEU * data.IMP_PRICE;
                    }
                    else
                    {
                        this.SL_THUA = (data.VIR_END_AMOUNT ?? 0) - (data.INVENTORY_AMOUNT ?? 0);
                        this.TIEN_THUA = this.SL_THUA * data.IMP_PRICE;
                    }
                    var medi = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == data.MEDICINE_TYPE_ID);
                    if (medi != null)
                    {
                        this.RECORDING_TRANSACTION = medi.RECORDING_TRANSACTION;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000132ADO(V_HIS_MEST_PERIOD_MATE data)
        {
            try
            {
                if (data != null)
                {
                    this.TYPE = 2;
                    this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = data.NATIONAL_NAME;
                    //this.CONCENTRA = data.CONCENTRA;
                    this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.VIR_END_AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.INVENTORY_AMOUNT = data.INVENTORY_AMOUNT ?? 0;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.IMP_VAT_RATIO = data.IMP_VAT_RATIO;
                    this.IN_AMOUNT = data.IN_AMOUNT;
                    this.METY_MATY_ID = data.MATERIAL_TYPE_ID;
                    this.EXPIRED_DATE_STR = data.EXPIRED_DATE.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0) : "";
                    this.PACKAGE_NUMBER = data.PACKAGE_NUMBER;
                    this.PARENT_ID = data.PARENT_ID;
                    this.MEDI_MATE_ID = data.MATERIAL_ID;
                    if ((data.INVENTORY_AMOUNT ?? 0) >= (data.VIR_END_AMOUNT ?? 0))
                    {
                        this.SL_THIEU = (data.INVENTORY_AMOUNT ?? 0) - (data.VIR_END_AMOUNT ?? 0);
                        this.TIEN_THIEU = this.SL_THIEU * data.IMP_PRICE;
                    }
                    else
                    {
                        this.SL_THUA = (data.VIR_END_AMOUNT ?? 0) - (data.INVENTORY_AMOUNT ?? 0);
                        this.TIEN_THUA = this.SL_THUA * data.IMP_PRICE;
                    }
                    var medi = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == data.MATERIAL_TYPE_ID);
                    if (medi != null)
                    {
                        this.RECORDING_TRANSACTION = medi.RECORDING_TRANSACTION;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000132ADO(V_HIS_MEST_PERIOD_METY data)
        {
            try
            {
                if (data != null)
                {
                    this.TYPE = 1;
                    this.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = data.NATIONAL_NAME;
                    this.CONCENTRA = data.CONCENTRA;
                    this.REGISTER_NUMBER = data.REGISTER_NUMBER;
                    this.AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.VIR_END_AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.IN_AMOUNT = data.IN_AMOUNT;
                    this.OUT_AMOUNT = data.OUT_AMOUNT;
                    this.BEGIN_AMOUNT = data.BEGIN_AMOUNT;
                    this.INVENTORY_AMOUNT = data.INVENTORY_AMOUNT ?? 0;
                    this.EXPIRED_DATE_STR = data.ALERT_EXPIRED_DATE.ToString(); ;
                    this.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                    this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000132ADO(V_HIS_MEST_PERIOD_MATY data)
        {
            try
            {
                if (data != null)
                {
                    this.TYPE = 2;
                    this.MEDI_MATE_TYPE_CODE = data.MATERIAL_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.MATERIAL_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.NATIONAL_NAME = data.NATIONAL_NAME;
                    this.AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.VIR_END_AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.IN_AMOUNT = data.IN_AMOUNT;
                    this.OUT_AMOUNT = data.OUT_AMOUNT;
                    this.BEGIN_AMOUNT = data.BEGIN_AMOUNT;
                    this.INVENTORY_AMOUNT = data.INVENTORY_AMOUNT ?? 0;
                    this.EXPIRED_DATE_STR = data.ALERT_EXPIRED_DATE.ToString();
                    this.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                    this.CONCENTRA = data.CONCENTRA;
                    this.IMP_PRICE = data.IMP_PRICE;
                    this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000132ADO(V_HIS_MEST_PERIOD_BLTY data)
        {
            try
            {
                if (data != null)
                {
                    this.TYPE = 3;
                    this.MEDI_MATE_TYPE_CODE = data.BLOOD_TYPE_CODE;
                    this.MEDI_MATE_TYPE_NAME = data.BLOOD_TYPE_NAME;
                    this.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                    this.AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.VIR_END_AMOUNT = data.VIR_END_AMOUNT ?? 0;
                    this.IN_AMOUNT = data.IN_AMOUNT;
                    this.OUT_AMOUNT = data.OUT_AMOUNT;
                    this.BEGIN_AMOUNT = data.BEGIN_AMOUNT;
                    this.INVENTORY_AMOUNT = data.INVENTORY_AMOUNT ?? 0;
                    this.VOLUME = data.VOLUME;
                    this.PACKING_TYPE_NAME = data.PACKING_TYPE_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class RoleADO
    {
        public string Role1 { get; set; }
        public string Role2 { get; set; }
        public string Role3 { get; set; }
        public string Role4 { get; set; }
        public string Role5 { get; set; }
        public string Role6 { get; set; }
        public string Role7 { get; set; }
        public string Role8 { get; set; }
        public string Role9 { get; set; }
        public string Role10 { get; set; }
        public string User1 { get; set; }
        public string User2 { get; set; }
        public string User3 { get; set; }
        public string User4 { get; set; }
        public string User5 { get; set; }
        public string User6 { get; set; }
        public string User7 { get; set; }
        public string User8 { get; set; }
        public string User9 { get; set; }
        public string User10 { get; set; }
    }
}
