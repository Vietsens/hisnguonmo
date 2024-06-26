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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public class CallPatientDataWorker
    {
        private static Dictionary<long, List<ServiceReq1ADO>> dicCallPatient;

        public static Dictionary<long, List<ServiceReq1ADO>> DicCallPatient
        {
            get
            {
                if (dicCallPatient == null)
                {
                    dicCallPatient = new Dictionary<long, List<ServiceReq1ADO>>();
                }
                lock (dicCallPatient) ;
                return dicCallPatient;
            }
            set
            {
                lock (dicCallPatient) ;
                dicCallPatient = value;
            }
        }

        private static Dictionary<long, DelegateSelectData> dicDelegateCallingPatient;

        public static Dictionary<long, DelegateSelectData> DicDelegateCallingPatient
        {
            get
            {
                if (dicDelegateCallingPatient == null)
                {
                    dicDelegateCallingPatient = new Dictionary<long, DelegateSelectData>();
                }
                lock (dicDelegateCallingPatient) ;
                return dicDelegateCallingPatient;
            }
            set
            {
                lock (dicDelegateCallingPatient) ;
                dicDelegateCallingPatient = value;
            }
        }
    }
}
