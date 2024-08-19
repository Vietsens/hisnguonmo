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
using Inventec.Common.QrCodeBHYT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.CheckHeinGOV
{
    public class ResultDataADO
    {
        public ResultDataADO() { }

        public ResultHistoryLDO ResultHistoryLDO { get; set; }
        public HeinCardData HeinCardData { get; set; }
        public bool IsShowQuestionWhileChangeHeinTime__Choose { get; set; }
        public bool IsToDate { get; set; }
        public bool IsAddress { get; set; }
        public bool SuccessWithoutMessage { get; set; }
        public bool IsThongTinNguoiDungThayDoiSoVoiCong__Choose { get; set; }
        public bool IsUsedNewCard { get; set; }
        public ChiTietKCBLDO ChiTietKCBLDO { get; set; }
    }
}
