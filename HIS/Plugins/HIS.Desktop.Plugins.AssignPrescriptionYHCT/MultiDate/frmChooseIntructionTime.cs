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
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.AssignPrescriptionPK.MultiDate;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK
{
    public partial class frmChooseIntructionTime : Form
    {
        List<long> selectedIntructionTimes;
        DelegateSelectData delegateSelectData;
        List<DateTime?> oldDatas;
        public frmChooseIntructionTime()
        {
            InitializeComponent();
        }
        public frmChooseIntructionTime(List<DateTime?> datas, DelegateSelectData selectData)
        {
            try
            {
                InitializeComponent();
                this.delegateSelectData = selectData;
                this.oldDatas = datas;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseIntructionTime_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.oldDatas != null && this.oldDatas.Count > 0)
                {
                    //Add empty datepciker
                    int emptyCount = 4 - this.oldDatas.Count;
                    if (emptyCount > 0)
                    {
                        for (int i = 0; i < emptyCount; i++)
                        {
                            UCMultiDate ucMultiDate = new MultiDate.UCMultiDate();
                            ucMultiDate.Dock = DockStyle.Top;
                            ucMultiDate.AutoSize = false;
                            pnlMultiDate.Controls.Add(ucMultiDate);
                        }
                    }

                    //Add datepcker with data
                    foreach (var item in this.oldDatas)
                    {
                        UCMultiDate ucMultiDate = new MultiDate.UCMultiDate(item);
                        ucMultiDate.Dock = DockStyle.Top;
                        ucMultiDate.AutoSize = false;
                        pnlMultiDate.Controls.Add(ucMultiDate);
                    }
                }
                else
                {
                    //Add empty datepciker
                    for (int i = 0; i < 4; i++)
                    {
                        UCMultiDate ucMultiDate = new MultiDate.UCMultiDate();
                        ucMultiDate.Dock = DockStyle.Top;
                        ucMultiDate.AutoSize = false;
                        pnlMultiDate.Controls.Add(ucMultiDate);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                UCMultiDate ucMultiDate = new MultiDate.UCMultiDate();
                ucMultiDate.Dock = DockStyle.Top;
                ucMultiDate.AutoSize = false;
                pnlMultiDate.Controls.Add(ucMultiDate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                bool isSelected = false;
                List<DateTime?> listSelected = new List<DateTime?>();
                foreach (Control item in pnlMultiDate.Controls)
                {
                    if (item != null && (item is UCMultiDate))
                    {
                        var dt = ((UCMultiDate)item).GetValue();
                        if (dt != null)
                        {
                            isSelected = true;
                            listSelected.Add(dt);
                        }
                    }
                }
                if (isSelected)
                {
                    delegateSelectData(listSelected);
                    this.Close();
                }
                else
                {
                    MessageManager.Show(ResourceMessage.ChuaChonNgayChiDinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
