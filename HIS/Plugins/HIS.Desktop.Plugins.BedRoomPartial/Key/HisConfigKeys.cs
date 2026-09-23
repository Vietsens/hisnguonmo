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

namespace HIS.Desktop.Plugins.BedRoomPartial.Key
{
    internal class HisConfigKeys
    {
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        internal const string HIS_CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        internal const string HIS_CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";
        internal const string HIS_CONFIG_KEY__MaxTimeFilter__Option = "HIS.Desktop.Plugins.MaxTimeFilter.Option";
        /// <summary>
        /// Hien thi don thuoc du tru theo ngay du tru tren man Buong benh (QT-11).
        /// "1" = bat, rong/khac = tat (mac dinh).
        /// </summary>
        internal const string HIS_CONFIG_KEY__ShowAnticipatePresByUseDate = "HIS.Desktop.Plugins.BedRoomPartial.ShowAnticipatePresByUseDate";
        /// <summary>
        /// So chu so thap phan hien thi cua cot so luong. Khong khai bao = giu nguyen hien thi cu.
        /// </summary>
        internal const string HIS_CONFIG_KEY__AmountDecimalNumber = "HIS.Desktop.AmountDecimalNumber";
        /// <summary>
        /// Canh bao Loai van ban bat buoc phai hoan thanh khi benh nhan vao khoa.
        ///
        /// Gia tri la SO PHUT ke tu khi nhap vien vao khoa thi bat dau kiem tra — vi du "30".
        /// Rong / khong phai so / nho hon hoac bang 0 = KHONG kiem tra (mac dinh, an toan cho
        /// vien chua bat).
        ///
        /// Muc rang buoc (canh bao / chan) da bo — phan mem chi CANH BAO, khong chan thao tac nao.
        ///
        /// Lan cap nhat gan nhat cua chinh dong cau hinh nay la moc "thoi diem khai bao cau hinh"
        /// cua quy tac khong hoi to: benh nhan vao khoa truoc moc do thi khong canh bao.
        /// </summary>
        internal const string HIS_CONFIG_KEY__RequiredDocument = "HIS.Desktop.Plugins.BedRoomPartial.RequiredDocument";
    }
}
