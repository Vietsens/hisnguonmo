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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.ADO
{
    /// <summary>
    /// Mot van ban EMR (V_EMR_DOCUMENT) tra ve tu "api/EmrDocument/GetView".
    /// Chi khai bao cac truong duoc su dung; cac truong con lai bi bo qua khi deserialize.
    /// </summary>
    public class EmrDocumentTreatmentADO
    {
        /// <summary>Ma dieu tri cua ho so, dung de biet luot kham nao trong luoi lich su kham co van ban.</summary>
        public string TREATMENT_CODE { get; set; }

        /// <summary>
        /// Co xoa mem. Tren V_EMR_DOCUMENT day la NUMBER nullable (short?), KHONG phai boolean:
        /// van ban binh thuong de NULL chu khong phai 0, nen "chua xoa" = khac 1.
        /// </summary>
        public short? IS_DELETE { get; set; }
    }
}
