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
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;

namespace HIS.Desktop.Plugins.ContactDeclaration.ADO
{
    class ContactPointADO : V_HIS_CONTACT_POINT
    {
        public string CODE_STR { get; set; }
        public long CONTACT_TIME { get; set; }
        public long CONTACT_ID { get; set; }
        public string FULL_NAME { get; set; }

        public ContactPointADO() { }

        public ContactPointADO(V_HIS_CONTACT_POINT data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ContactPointADO>(this, data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
