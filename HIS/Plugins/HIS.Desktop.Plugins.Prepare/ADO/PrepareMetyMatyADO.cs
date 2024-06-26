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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Prepare.ADO
{
    public class PrepareMetyMatyADO
    {
        public long? ID { get; set; }
        public long METY_MATY_TYPE_ID { get; set; }
        public string METY_MATY_TYPE_CODE { get; set; }
        public string METY_MATY_TYPE_NAME { get; set; }
        public decimal? APPROVAL_AMOUNT { get; set; }
        public decimal? REQ_AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; } //Hoat chat
        public string CONCENTRA { get; set; } //Ham luong
        public string MANUFACTURER_NAME { get; set; } //Noi san xuat
        public string NATIONAL_NAME { get; set; } //Quoc gia
        public string REQ_LOGINNAME { get; set; }
        public string REQ_USERNAME { get; set; }
        public string APPROVAL_LOGINNAME { get; set; }
        public string APPROVAL_USERNAME { get; set; }
        public int ACTION { get; set; }

        public string METY_MATY_TYPE_NAME__UNSIGN { get; set; }
        public string ACTIVE_INGR_BHYT_NAME__UNSIGN { get; set; }
        public string NATIONAL_NAME__UNSIGN { get; set; }
        public string MANUFACTURER_NAME__UNSIGN { get; set; }
        public int TYPE { get; set; }
    }
}
