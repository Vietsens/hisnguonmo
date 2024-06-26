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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.PeriousExpMestList
{
    public partial class frmChooseDate : Form
    {
        Action<DateTime, DateTime> actSelectDate;
        public frmChooseDate(Action<DateTime, DateTime> _actSelectDate)
        {
            InitializeComponent();
            this.actSelectDate = _actSelectDate;
        }

        private void frmChooseDate_Load(object sender, EventArgs e)
        {

        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                if (dateTimeFrom.EditValue == null && dateTimeTo.EditValue == null)
                {
                    Inventec.Desktop.Common.Message.MessageManager.Show("Chưa chọn ngày");
                    return;
                }

                if (this.actSelectDate != null)
                {
                    this.actSelectDate(dateTimeFrom.DateTime, dateTimeTo.DateTime);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
