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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public class ServiceReq1ADOWorker
    {
        private static ServiceReq1ADO serviceReq1ADO;

        public static ServiceReq1ADO ServiceReq1ADO
        {
            get
            {
                if (serviceReq1ADO == null)
                {
                    serviceReq1ADO = new ServiceReq1ADO();
                }
                //lock (serviceReq1ADO) ;
                return serviceReq1ADO;
            }
            set
            {
                //lock (serviceReq1ADO) ;
                serviceReq1ADO = value;
            }
        }
    }
}
