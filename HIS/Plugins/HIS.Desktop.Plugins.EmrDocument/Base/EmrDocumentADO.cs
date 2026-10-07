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
using EMR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EmrDocument.Base
{
    public class EmrDocumentADO : V_EMR_DOCUMENT
    {
        public string DOCUMENT_DISPLAY { get; set; }
        public long? CUSTOM_NUM_ORDER { get; set; }
        public string CUSTOM_BY_GROUP_NUM_ORDER { get; set; }
        public string PARENT_KEY { get; set; }
        public string CHILD_KEY { get; set; }
        public bool IsChecked { get; set; }

        //Văn bản trùng: cùng hồ sơ + loại + HIS_CODE, có chung người đã ký, đã có bản tạo sau
        public bool IsOlderDuplicate { get; set; }
        public long DuplicateAnchorId { get; set; }//ID bản mới nhất của bộ trùng
        public string NewestDocumentCode { get; set; }
        public long? NewestCreateTime { get; set; }
        //Khóa sắp xếp: bản cũ mượn khóa của bản mới nhất để đứng liền ngay dưới
        public long? SortDocTime { get; set; }
        public long? SortCreateTime { get; set; }
        public int DupRank { get; set; }

      
    }
}
