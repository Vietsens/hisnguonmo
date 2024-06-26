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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.GenerateRegisterOrder.ADO;

namespace HIS.Desktop.Plugins.GenerateRegisterOrder
{
    public partial class UCItem : UserControl
    {
        public long SizeTitle { get; set; }
        public long SizeStt { get; set; }
        public string TextStt { get; set; }
        public string TextTitle { get; set; }
        private List<HIS_REGISTER_GATE> lstGate { get; set; }
        private List<HisRegisterGateADO> lstSend { get; set; }
        public event EventHandler _Click;
        public UCItem(List<HIS_REGISTER_GATE> lst, List<HisRegisterGateADO> lstSend)
        {
            InitializeComponent();
            try
            {
                lstGate = lst;
                lstSend = lstSend;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCItem_Load(object sender, EventArgs e)
        {
            try
            {
                this.lblName.Font = new System.Drawing.Font("Microsoft Sans Serif", SizeTitle, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
                this.lblTitleNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", SizeStt, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                this.lblNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", SizeStt, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
                this.lblName.Text = TextTitle;
                this.lblNumber.Text = TextStt;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void lblName_Click(object sender, EventArgs e)
        {
            if (_Click != null)
            {
                lblNumber_Click(lblNumber, e);
            }
        }

        private void lblTitleNumber_Click(object sender, EventArgs e)
        {
            if (_Click != null)
            {
                lblNumber_Click(lblNumber, e);
            }
        }

        private void lblNumber_Click(object sender, EventArgs e)
        {
            if (_Click != null)
            {
                Label lb = sender as Label;
                lb.Tag = this.Tag;
                _Click.Invoke(lb, e);
            }
        }

     

    }
}
