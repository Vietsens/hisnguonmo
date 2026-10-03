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
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.TrackingCreate.ADO
{
    /// <summary>
    /// Dong che do an tren man hinh chon che do an (F2).
    /// </summary>
    public class DietADO
    {
        public long ID { get; set; }
        public string DIET_CODE { get; set; }
        public string NUTRITION_INFO { get; set; }
        public string PROCESSING_FORM { get; set; }
        public string TREATMENT_APPLY { get; set; }

        public bool IsChecked { get; set; }

        /// <summary>
        /// Thu tu bac si tick (1, 2, 3...). Noi dung chen vao to dieu tri xep theo thu tu nay,
        /// khong theo thu tu tren luoi. Bo tick thi ve 0.
        /// </summary>
        public int CheckOrder { get; set; }

        public DietADO(HIS_DIET data)
        {
            this.ID = data.ID;
            this.DIET_CODE = data.DIET_CODE;
            this.NUTRITION_INFO = data.NUTRITION_INFO;
            this.PROCESSING_FORM = data.PROCESSING_FORM;
            this.TREATMENT_APPLY = data.TREATMENT_APPLY;
        }
    }
}
