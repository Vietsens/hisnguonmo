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

namespace HIS.Desktop.Plugins.ImportBlood.ADO
{
    public class VHisBloodADO : V_HIS_BLOOD
    {
        public decimal ImpVatRatio { get; set; }

        public long SERVICE_TYPE_ID { get; set; }
        public bool IsEmptyRow { get; set; }
        public bool IsBloodDonation { get; set; }
        public string BloodDonationCode { get; set; }
        public string BloodDonationStr_ForDisplayGroup { get; set; }
        public string DOB_ForDisplay { get; set; }
        public string GENDER_ForDisplay { get; set; }
        public List<string> ErrorDescriptions = new List<string>();
        public string EXPIRED_DATE_excel { get; set; }
        public string PACKING_TIME_excel { get; set; }
        public VHisBloodADO() { }

        public VHisBloodADO(V_HIS_BLOOD_TYPE bloodType)
        {
            try
            {
                if (bloodType != null)
                {
                    this.BLOOD_TYPE_ID = bloodType.ID;
                    this.BLOOD_TYPE_CODE = bloodType.BLOOD_TYPE_CODE;
                    this.BLOOD_TYPE_NAME = bloodType.BLOOD_TYPE_NAME;
                    this.IMP_PRICE = bloodType.IMP_PRICE ?? 0;
                    this.IMP_VAT_RATIO = bloodType.IMP_VAT_RATIO ?? 0;
                    this.ImpVatRatio = this.IMP_VAT_RATIO * 100;
                    this.ELEMENT = bloodType.ELEMENT;
                    this.SERVICE_TYPE_ID = bloodType.SERVICE_TYPE_ID;
                    this.SERVICE_ID = bloodType.SERVICE_ID;
                    this.VOLUME = bloodType.VOLUME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public VHisBloodADO(V_HIS_BLOOD blood)
        {
            try
            {
                if (blood != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<VHisBloodADO>(this, blood);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetValueByHisBlood(HIS_BLOOD blood)
        {
            try
            {
                if (blood != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<VHisBloodADO>(this, blood);
                    //System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<HIS_BLOOD>();
                    //foreach (var item in pi)
                    //{
                    //    item.SetValue(this, item.GetValue(blood));
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
