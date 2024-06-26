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

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate
{
    public partial class UCMultiDate : UserControl
    {
        DateTime? data;
        public UCMultiDate()
        {
            InitializeComponent();
        }
        public UCMultiDate(DateTime? oldValue)
        {
            try
            {
                InitializeComponent();
                this.data = oldValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void UCMultiDate_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.data != null && this.data.Value != DateTime.MinValue)
                {
                    dtIntructionTime.EditValue = this.data;
                }
                else
                {
                    dtIntructionTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal DateTime? GetValue()
        {
            try
            {
                if (dtIntructionTime.DateTime != DateTime.MinValue)
                {
                    return dtIntructionTime.DateTime;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}
