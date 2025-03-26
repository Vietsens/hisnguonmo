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
using Inventec.UC.ComboDistrict.Data;

namespace Inventec.UC.ComboDistrict.Desgin.Template1
{
    public partial class Template1 : UserControl
    {
        private LoadComboCommuneFromDistrict _LoadComboCommune;
        private SetValueComboCommune _SetValueCommune;
        private SetFocusComboCommune _FocusComboCommune;
        private GetValueComboProvince _GetValueProvince;

        private DataInitDistrict DataDistrict;
        private List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listData;

        public Template1(DataInitDistrict Data)
        {
            InitializeComponent();
            this.DataDistrict = Data;
            this.listData = Data.listDistrict;
        }

    }
}
