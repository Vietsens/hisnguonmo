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

namespace MPS.ProcessorBase.Core
{
    internal class EmrColumnMappingADO
    {
        internal EmrColumnMappingADO() { }
        public string EmrColumn { get; set; }
        public string Key { get; set; }

        /// <summary>
        /// Cach gan gia tri vao cot EMR khi cot do la chuoi:
        ///   null / khong khai bao / "REPLACE" -> GHI DE (hanh vi mac dinh, giong truoc day)
        ///   "APPEND"                          -> NOI vao CUOI gia tri dang co
        ///   "PREPEND"                         -> NOI vao DAU gia tri dang co
        ///
        /// Dat tren TUNG DONG anh xa cua TUNG bieu in (man "Anh xa du lieu EMR"), nen chi
        /// bieu in nao khai bao moi doi hanh vi - cac MPS khac khong bi anh huong.
        ///
        /// Dung cho truong hop can bo sung du lieu vao mot cot ma KHONG duoc mat gia tri cu,
        /// vi du HIS_CODE cua van ban ngoai dot dieu tri: thu vien ky dung chuoi nay de nhan
        /// dien "van ban da ky chua" (so khop BANG), ghi de se lam mat nhan dien va tao ban trung.
        /// </summary>
        public string Mode { get; set; }

        /// <summary>
        /// Ky tu ngan cach khi Mode = APPEND / PREPEND. Khong khai bao -> khong chen gi.
        /// Vi du "|" hoac " ".
        /// </summary>
        public string Separator { get; set; }
    }
}
