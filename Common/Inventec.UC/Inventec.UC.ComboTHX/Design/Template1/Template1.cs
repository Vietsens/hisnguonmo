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
using Inventec.UC.ComboTHX.Data;

namespace Inventec.UC.ComboTHX.Design.Template1
{
    public partial class Template1 : UserControl
    {

        private LoadComboDistrict _LoadHuyen;
        private LoadComboCommune _LoadPhuongXa;
        private SetValueComboProvince _SetValueTinh;
        private SetValueComboDistrict _SetValueHuyen;
        private SetValueComboCommune _SetValuePhuongXa;
        private FocusComboProvince _FocusTinh;
        private FocusNextControl _FocusNext;

        private List<ViewSdaCommuneModel> listData = new List<ViewSdaCommuneModel>();
        private List<ViewSdaCommuneModel> listDataTHX = new List<ViewSdaCommuneModel>();

        private List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listDistrict = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
        private List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listCommune = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();

        public Template1(DataInitTHX Data)
        {
            InitializeComponent();
            this.listCommune = Data.ListCommune;
            this.listDistrict = Data.ListDistrict;

            LoadDataCommuneFromDbToLocal();
            LoadDataToComboTHX();
        }

    }
}
