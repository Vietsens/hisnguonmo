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

namespace HIS.Desktop.Plugins.HisPrepareApprove.ADO
{
    public class MetyMatyADO : MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY
    {
        public bool IsMedicine { get; set; }
        public decimal ReqAmount { get; set; }
        public decimal? ApprovalAmount { get; set; }

        public MetyMatyADO(MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY _data)
        {
            try
            {
                if (_data != null)
                {

                    System.Reflection.PropertyInfo[] pi = Inventec.Common.Repository.Properties.Get<MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY>();

                    foreach (var item in pi)
                    {
                        item.SetValue(this, (item.GetValue(_data)));
                    }
                    this.IsMedicine = true;
                    this.ApprovalAmount = _data.APPROVAL_AMOUNT;
                    this.ReqAmount = _data.REQ_AMOUNT;
                }

            }

            catch (Exception)
            {

            }
        }

        public MetyMatyADO(MOS.EFMODEL.DataModels.V_HIS_PREPARE_MATY _data)
        {
            try
            {
                if (_data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MetyMatyADO>(this, _data);
                    this.IsMedicine = false;
                    this.MEDICINE_TYPE_NAME = _data.MATERIAL_TYPE_NAME;
                    this.MEDICINE_TYPE_CODE = _data.MATERIAL_TYPE_CODE;
                    this.ApprovalAmount = _data.APPROVAL_AMOUNT;
                    this.ReqAmount = _data.REQ_AMOUNT;
                    this.SERVICE_UNIT_NAME = _data.SERVICE_UNIT_NAME;
                    this.SERVICE_UNIT_CODE = _data.SERVICE_UNIT_CODE;
                    this.NATIONAL_NAME = _data.NATIONAL_NAME;
                    this.MANUFACTURER_CODE = _data.MANUFACTURER_CODE;
                    this.MANUFACTURER_NAME = _data.MANUFACTURER_NAME;
                    this.CONCENTRA = _data.CONCENTRA;
                }

            }

            catch (Exception)
            {

            }
        }
    }
}
