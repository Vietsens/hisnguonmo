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

namespace MPS.ADO
{
    public class HisTrackingGroupADOs : MOS.EFMODEL.DataModels.HIS_TRACKING
    {
        public string TRACKING_TIME_STR { get; set; }

        //Xét nghiệm
        public string TEST_NAME { get; set; }
        public string TEST_DETAIL { get; set; }

        //Chuẩn đoán hình ảnh
        public string DIIM_NAME { get; set; }
        public string DIIM_DETAIL { get; set; }

        //Thủ thuật
        public string MISU_NAME { get; set; }
        public string MISU_DETAIL { get; set; }

        //Phẫu thuật
        public string SURG_NAME { get; set; }
        public string SURG_DETAIL { get; set; }

        //Thăm dò chức năng
        public string FUEX_NAME { get; set; }
        public string FUEX_DETAIL { get; set; }

        //Nội soi
        public string ENDO_NAME { get; set; }
        public string ENDO_DETAIL { get; set; }

        //Siêu âm
        public string SUIM_NAME { get; set; }
        public string SUIM_DETAIL { get; set; }

        //Thuốc vật tư
        public string PRES_NAME { get; set; }
        public string PRES_DETAIL { get; set; }

        //Phục hồi chức năng
        public string REHA_NAME { get; set; }
        public string REHA_DETAIL { get; set; }

        //Giường
        public string BED_NAME { get; set; }
        public string BED_DETAIL { get; set; }

        //Khác
        public string OTHER_NAME { get; set; }
        public string OTHER_DETAIL { get; set; }

        //ICD
        public string ICD_NAME_TRACKING { get; set; }
    }
}
