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

namespace HIS.UC.ExamTreatmentFinish.ADO
{
    /// <summary>
    /// DTO gui len API "api/HisTreatment/SetChronic" de danh dau / bo danh dau dot dieu tri man tinh.
    ///
    /// HANH VI PHIA BACKEND:
    ///   IsChronic = true  → backend SINH ngay dong tuong ung tren dong thoi gian
    ///   IsChronic = false → backend tu XOA dong da sinh
    /// FE chi gui co, khong tu sinh/xoa dong.
    ///
    /// TAI SAO KHONG DUNG TRUC TIEP MOS.SDO.HisTreatmentSetChronicSDO:
    ///   Project nay tham chieu ..\..\..\lib\MOS\MOS.SDO.dll — ban pre-built dung chung toan solution,
    ///   chua co class HisTreatmentSetChronicSDO. TUYET DOI KHONG ghi de DLL do.
    ///   Vi vay dung ADO local nay lam DTO trung gian.
    ///
    /// RANG BUOC: Ten field phai TRUNG KHIT ten property cua SDO ben backend
    /// (TreatmentId / IsChronic / RequestRoomId) de JSON bind dung — doi ten se lam BE nhan null.
    ///
    /// KHI NAO XOA FILE NAY: sau khi MOS.SDO.dll duoc build lai co HisTreatmentSetChronicSDO,
    /// thay class nay bang SDO that va xoa khai bao Compile trong csproj.
    /// </summary>
    public class SetChronicADO
    {
        /// <summary>ID dot dieu tri can danh dau (HIS_TREATMENT.ID)</summary>
        public long TreatmentId { get; set; }

        /// <summary>true = danh dau man tinh, false = bo danh dau</summary>
        public bool IsChronic { get; set; }

        /// <summary>ID phong thuc hien thao tac — BE dung de ghi nhan nguoi/noi thay doi</summary>
        public long RequestRoomId { get; set; }

        /// <summary>
        /// Thoi diem cua dong dien dieu tri "Dieu tri ngoai tru" (yyyyMMddHHmmss).
        ///
        /// PHAI gui gia tri o "Thoi gian ra" tren form, KHONG de BE lay gio may:
        /// LOG_TIME cua dong nay tro thanh HIS_TREATMENT.CLINICAL_IN_TIME, ma BE chan khi
        /// CLINICAL_IN_TIME > OUT_TIME ("Thoi gian ra vien khong duoc nho hon thoi gian nhap vien").
        /// O "Thoi gian ra" nap luc mo man hinh nen luon nho hon gio may khi tich checkbox
        /// --> lay gio may se lam khong luu duoc ket thuc dieu tri ngay.
        ///
        /// De trong --> BE tu lay thoi diem he thong.
        /// </summary>
        public long? LogTime { get; set; }
    }
}
