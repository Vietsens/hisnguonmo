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
    public partial class UCFilterByWeek : UserControl
    {
        public List<Week> listWeek { get; set; }

        public UCFilterByWeek()
        {
            InitializeComponent();
        }

        public bool ValidateUC()
        {
            bool result = true;
            try
            {
                if (cboWeekFrom.EditValue == null || cboWeekFrom.EditValue == null || cboNam.EditValue == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (cboWeekFrom.SelectedIndex > cboWeekTo.SelectedIndex)
                {
                    MessageBox.Show("Tuần từ lớn hơn tuần đến", "Thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            if (cboNam.EditValue != null)
            {
                int nam = Inventec.Common.TypeConvert.Parse.ToInt32(cboNam.EditValue.ToString());
                listWeek = GetListWeekByYear(nam);
                FilterToComboWeek();
            }
        }

        private List<Week> GetListWeekByYear(int year)
        {
            List<Week> result = new List<Week>();
            try
            {
                var jan1 = new DateTime(year, 1, 1);
                //beware different cultures, see other answers
                var startOfFirstWeek = jan1.AddDays(1 - (int)(jan1.DayOfWeek));
                result =
                    Enumerable
                        .Range(0, 54)
                        .Select(i => new
                        {
                            weekStart = startOfFirstWeek.AddDays(i * 7)
                        })
                        .TakeWhile(x => x.weekStart.Year <= jan1.Year)
                        .Select(x => new
                        {
                            x.weekStart,
                            weekFinish = x.weekStart.AddDays(4)
                        })
                        .SkipWhile(x => x.weekFinish < jan1.AddDays(1))
                        .Select((x, i) => new Week
                        {
                            WeekStart = x.weekStart,
                            WeekEnd = x.weekFinish,
                            WeekNum = i + 1
                        }).ToList();
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return result;
        }

        private void FilterToComboWeek()
        {
            try
            {
                cboWeekFrom.Properties.Items.Clear();
                cboWeekTo.Properties.Items.Clear();
                if (listWeek != null && listWeek.Count > 0)
                {
                    for (int i = 0; i < listWeek.Count; i++)
                    {
                        cboWeekFrom.Properties.Items.Add(i + 1);
                        cboWeekTo.Properties.Items.Add(i + 1);
                    }

                    cboWeekFrom.SelectedIndex = 0;
                    cboWeekTo.SelectedIndex = cboWeekTo.Properties.Items.Count - 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCFilterByWeek_Load(object sender, EventArgs e)
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
