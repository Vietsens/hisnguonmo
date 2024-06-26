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

namespace HIS.Desktop.Plugins.CareSlipList
{
    public delegate void SelectRowOnLoadHanler(long treatmentId, long patientId, long intructionTime, long finishTime);
    public delegate void SelectImageHanler(List<byte[]> arrImages);
    public delegate void UpdatePatientInfo(MOS.EFMODEL.DataModels.V_HIS_PATIENT patient);
    public delegate bool UpdatePatientType(MOS.EFMODEL.DataModels.HIS_SERE_SERV hisSereServ);
    public delegate bool UpdateSereServ(MOS.EFMODEL.DataModels.HIS_SERE_SERV hisSereServ);
    public delegate void UpdatePatientStatus(MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ servicereq);
    public delegate void DelegateRefeshData();
    public delegate void DelegateRefeshRowData(int rowHandle);
    public delegate void DelegateFocusMoveout();
    public delegate void DelegateRefeshTreatmentPartialData(long treatmentId);
    public delegate void DelegateRefeshDataIcd(MOS.EFMODEL.DataModels.HIS_ICD icd);
    public delegate void DelegateRefeshDataInfusion(MOS.EFMODEL.DataModels.V_HIS_INFUSION infusion);
    public delegate void DelegateRefeshDataTreatmentBedRoom(MOS.SDO.HisTreatmentCommonInfoSDO treatmentCommon);
    public delegate void DelegateRefeshDataServiceReqView(MOS.SDO.HisServiceReqViewSDO serviceReqView);
    public delegate void DelegateRefeshIcdChandoanphu(string icds);
    public delegate void DelegateRefPatientHouseHold(HIS_PATIENT_HOUSEHOLD patientHouseHold);
    public delegate void PatientInfoResult(object o);
    public delegate void DelegateSereServResult(object sereServs);
    public delegate void DelegateSwapService(object sereServ);

}
