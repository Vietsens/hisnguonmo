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
using DevExpress.XtraGrid.Views.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace HIS.Desktop.Plugins.XMLViewer130
{
    public partial class UCXml130 : UserControl
    {
        public List<object> dataSourceXMLGrid = new List<object>();
        public UCXml130()
        {
            InitializeComponent();
        }

        public void LoadList(List<object> listObj)
        {
            try
            {
                dataSourceXMLGrid.AddRange(listObj);
                gridControllXml.DataSource = dataSourceXMLGrid;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewXml_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            try
            {
                if (e.Column.ColumnType.Name == typeof(XmlCDataSection).Name)
                {
                    e.DisplayText = (e.Value as XmlCDataSection).Value;
                    e.Column.OptionsColumn.AllowEdit = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
