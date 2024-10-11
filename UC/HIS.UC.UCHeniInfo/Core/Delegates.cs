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
using Inventec.Common.QrCodeBHYT;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.UCHeniInfo
{
    public delegate bool FillDataPatientSDOToRegisterForm(HisPatientSDO patientSDO);
    public delegate void ProcessFillDataCareerUnder6AgeByHeinCardNumber(HeinCardData heinCard, bool isSearchHeinCardNumber);
    public delegate void CheckExamHistoryByHeinCardNumber(HeinCardData heinCardNumber);
    public delegate void DelegateAutoCheckCC(bool value);
    public delegate bool GetIsChild();
    public delegate void UpdateTranPatiDataByPatientOld(HIS_PATIENT_TYPE_ALTER patientTypeAlter);
    //public delegate long DelegateGetPatientTypeId();
    public delegate void DelegateCheckSS(bool isCheck);
}
