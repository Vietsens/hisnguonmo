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
using HIS.Desktop.Plugins.Dashboard.ADO;

namespace HIS.Desktop.Plugins.Dashboard.UCFilter
{
    public partial class UCFilterByMonth : UserControl
    {

        public List<Month> listMonth { get; set; }
        public UCFilterByMonth()
        {
            InitializeComponent();
        }

        public bool ValidateUC()
        {
            bool result = true;
            try
            {
                if (cboMonthFrom.EditValue == null || cboMonthFrom.EditValue == null || cboNam.EditValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (cboMonthFrom.SelectedIndex > cboMonthTo.SelectedIndex)
                {
                    MessageBox.Show("Tháng từ lớn hơn tháng đến", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void cboNam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboMonthFrom.SelectedIndex = 0;
                cboMonthTo.SelectedIndex = cboMonthTo.Properties.Items.Count - 1;
                listMonth = new List<Month>();
                if (cboNam.EditValue != null)
                {
                    int nam = Inventec.Common.TypeConvert.Parse.ToInt32(cboNam.EditValue.ToString());
                    for (int i = 1; i <= 12; i++)
                    {
                        Month month = new Month();
                        month.MonthStart = new DateTime(nam, i, 1);
                        month.MonthEnd = new DateTime(nam, i, 1).AddMonths(1).AddDays(-1);
                        month.MonthNum = i;
                        listMonth.Add(month);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCFilterByMonth_Load(object sender, EventArgs e)
        {
            try
            {
                int year = DateTime.Now.Year;
                var items = cboNam.Properties.Items;
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[0].ToString() == year.ToString())
                    {
                        cboNam.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
