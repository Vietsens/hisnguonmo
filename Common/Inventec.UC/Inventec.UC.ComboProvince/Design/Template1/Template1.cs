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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.UC.ComboProvince.Data;

namespace Inventec.UC.ComboProvince.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private LoadComboDistrictFromProvince _LoadHuyenFromTinh;
        private SetValueCboDistrictAndCboCommune _SetValue;
        private SetFocusCboDistrict _FocusCboHuyen;

        private DataInitProcinve Data;
        private List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listData;

        public Template1(DataInitProcinve Data)
        {
            InitializeComponent();
            this.Data = Data;
            this.listData = Data.listProvince;
            LoadDataToCombo();
        }
    }
}
