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

namespace HIS.Desktop.Common
{
    public delegate void RefeshReference();
    public delegate void DelegateRefeshTreatmentPartialData(long treatmentId);
    public delegate void DelegateRefeshDataIcd(MOS.EFMODEL.DataModels.HIS_ICD icd);
    public delegate void DelegateRefeshIcdChandoanphu(string icdCodes, string icdNames);
    public delegate void DelegateRefreshAcsUser(string loginNames);
    public delegate void DelegateDataTextLib(MOS.EFMODEL.DataModels.HIS_TEXT_LIB textLib);
    public delegate void DelegateSelectData(object data);
    public delegate void DelegateSelectObjectData(object data, bool status);
    public delegate void DelegateRefreshData();
    public delegate void DelegateRefresh(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ serviceReq);
    public delegate void DelegateReturnSuccess(bool success);
    public delegate void DelegateReturnMutilObject(object[] args);
    public delegate void DelegateCloseForm_Uc(object data);
    public delegate void DelegateSelectDatas(object data1, object data2);
    public delegate void DelegateImpTime(long? impTime);
    public delegate void DelegateLoadPTTT(string namePTTT, DateTime? startTime, DateTime? finishTime);
}
