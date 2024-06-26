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
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.XMLViewer.XML.XML2
{
    public class XML2DetailData
    {
        public string MA_LK { get; set; }

        public string STT { get; set; }

        public string MA_THUOC { get; set; }

        public string MA_NHOM { get; set; }

        public string TEN_THUOC { get; set; }

        public string DON_VI_TINH { get; set; }

        public string HAM_LUONG { get; set; }

        public string DUONG_DUNG { get; set; }

        public string LIEU_DUNG { get; set; }

        public string SO_DANG_KY { get; set; }

        public string TT_THAU { get; set; }

        public string PHAM_VI { get; set; }

        public string TYLE_TT { get; set; }

        public decimal SO_LUONG { get; set; }

        public decimal DON_GIA { get; set; }

        public decimal THANH_TIEN { get; set; }

        public string MUC_HUONG { get; set; }

        public decimal T_NGUONKHAC { get; set; }

        public decimal T_BNTT { get; set; }

        public decimal T_BHTT { get; set; }

        public decimal T_BNCCT { get; set; }

        public decimal T_NGOAIDS { get; set; }

        public string MA_KHOA { get; set; }

        public string MA_BAC_SI { get; set; }

        public string MA_BENH { get; set; }

        public string NGAY_YL { get; set; }

        public string MA_PTTT { get; set; }
    }
}
