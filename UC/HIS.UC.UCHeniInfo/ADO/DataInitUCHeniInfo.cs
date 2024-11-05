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
using HIS.UC.UCHeniInfo;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.UCHeniInfo.Data
{
    public class DataInitUCHeniInfo
    {
        public DataInitUCHeniInfo() { }

        public long PATIENT_TYPE_ID__BHYT { get; set; }
        public bool IsChild { get; set; }
        public long isVisibleControl { get; set; }
        public string IsShowCheckKhongKTHSD { get; set; }
        public bool IsDefaultRightRouteType { get; set; }
        public bool IsEdit { get; set; }
        public bool IsTempQN { get; set; }
        public bool IsCheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong { get; set; }
        public long AlertExpriedTimeHeinCardBhyt { get; set; }
        public UpdateTranPatiDataByPatientOld UpdateTranPatiDataByPatientOld { get; set; }
        public FillDataPatientSDOToRegisterForm dlgFillDataPatientSDOToRegisterForm { get; set; }
        public DelegateFocusNextUserControl dlgFocusNextUserControl { get; set; }
        public ProcessFillDataCareerUnder6AgeByHeinCardNumber dlgProcessFillDataCareerUnder6AgeByHeinCardNumber { get; set; }
        public DelegateAutoCheckCC dlgAutoCheckCC { get; set; }
        public CheckExamHistoryByHeinCardNumber dlgCheckExamHistory { get; set; }
        public GetIsChild dlgGetIsChild { get; set; }
        public DelegateValidationUserControl dlgValidationControl { get; set; }
        public DelegateCheckSS dlgCheckSS { get; set; }
    }
}
