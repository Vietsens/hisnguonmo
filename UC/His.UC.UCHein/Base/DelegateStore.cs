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
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using Inventec.Common.QrCodeBHYT;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein
{
    public delegate void SetFocusMoveOut();
    public delegate void SetRendererServiceReqControl();
    public delegate void HeinCardNumberKeyDownByUc(List<MOS.EFMODEL.DataModels.V_HIS_PATIENT> Patients);
    public delegate void ProcessFillDataCareerUnder6AgeByHeinCardNumber(HeinCardData heinCard, bool isSearchHeinCardNumber);
    public delegate void SetShortcutKeyDown(System.Windows.Forms.Keys key);
    public delegate void DelegateRefeshDataIcd(MOS.EFMODEL.DataModels.HIS_ICD icd);
    public delegate void DelegateRefeshIcdChandoanphu(string icds);
    public delegate void DelegateAutoCheckCC(bool value);
    public delegate void CheckExamHistoryByHeinCardNumber(HeinCardData heinCardNumber);
    public delegate bool FillDataPatientSDOToRegisterForm(HisPatientSDO patientSDO);
    public delegate void DelegateSetRelativeAddress(bool value);
}
