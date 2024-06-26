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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InfusionCreate.ADO
{
    public class ComboSelectMedicineADO
    {
        public int Action { get; set; }

        public long? MEDICINE_ID { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public decimal? VOLUME { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public decimal? AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public long INFUSION_ID { get; set; }
        public long isDisableVolume { get; set; }
        public long isDisableAmount { get; set; }
        public long isDisableUnitName { get; set; }

        public ComboSelectMedicineADO() { }

        public ComboSelectMedicineADO(V_HIS_EXP_MEST_MEDICINE_6 _ExpMestMedicine6)
        {
            try
            {

                if (_ExpMestMedicine6 != null)
                {
                    this.MEDICINE_ID = _ExpMestMedicine6.MEDICINE_ID;
                    this.MEDICINE_TYPE_ID = _ExpMestMedicine6.MEDICINE_TYPE_ID;
                    this.PACKAGE_NUMBER = _ExpMestMedicine6.PACKAGE_NUMBER;
                    //this.VOLUME = _ExpMestMedicine6.AMOUNT;
                    this.MEDICINE_TYPE_NAME = _ExpMestMedicine6.MEDICINE_TYPE_NAME;
                    this.MEDICINE_TYPE_CODE = _ExpMestMedicine6.MEDICINE_TYPE_CODE;
                    this.AMOUNT = _ExpMestMedicine6.AMOUNT;
                    this.SERVICE_UNIT_NAME = _ExpMestMedicine6.SERVICE_UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public ComboSelectMedicineADO(V_HIS_SERVICE_REQ_METY _ServiceReqMety)
        {
            try
            {

                if (_ServiceReqMety != null)
                {
                    this.MEDICINE_ID = null;
                    this.MEDICINE_TYPE_ID = _ServiceReqMety.MEDICINE_TYPE_ID;
                    this.PACKAGE_NUMBER = null;
                    //this.VOLUME = _ServiceReqMety.AMOUNT;
                    this.MEDICINE_TYPE_NAME = _ServiceReqMety.MEDICINE_TYPE_NAME;
                    var medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().SingleOrDefault(o => o.ID == _ServiceReqMety.MEDICINE_TYPE_ID);
                    this.MEDICINE_TYPE_CODE = medicineType != null ? medicineType.MEDICINE_TYPE_CODE : null;
                    this.AMOUNT = _ServiceReqMety.AMOUNT;
                    this.SERVICE_UNIT_NAME = _ServiceReqMety.UNIT_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

    }
}
