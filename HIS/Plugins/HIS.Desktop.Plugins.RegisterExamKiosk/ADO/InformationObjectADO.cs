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
using His.Bhyt.InsuranceExpertise.LDO;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.ADO
{
    public class InformationObjectADO
    {
        //lấy thông tin BHYT (trái tuyến, giới thiệu)
        public HisExamRegisterKioskSDO ExamRegisterKiosk { get; set; }

        public ResultHistoryLDO HeinInfo { get; set; }
        
        //có dữ liệu khi đã từng đăng ký trên HIS
        public HisPatientForKioskSDO PatientForKiosk { get; set; }

        //có khi quẹt thẻ. kể cả chưa từng đăng ký trên his.
        public HisCardSDO CardInfo { get; set; }
        public string ServiceCode { get; set; }

        public InformationObjectADO(string serviceCode = null)
        {
            this.ServiceCode = serviceCode;
        }
    }
}
