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

namespace HIS.Desktop.Plugins.ServiceExecute
{
    public partial class UcWord : UserControl
    {
        Action<decimal> actChangeZoom;
        public UcWord(Action<decimal> actChangeZoom)
        {
            InitializeComponent();
            this.actChangeZoom = actChangeZoom;
        }

        private void txtDescription_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                //WordProcess.zoomFactor(txtDescription);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_ZoomChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.actChangeZoom != null)
                {
                    this.actChangeZoom((decimal)txtDescription.ActiveView.ZoomFactor);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
