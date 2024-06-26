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
using EMR.EFMODEL.DataModels;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MPS.ProcessorBase.EmrBusiness
{
    public partial class frmChooseBusiness : Form
    {
        Action<EMR_BUSINESS> actChoose;
        List<EMR_BUSINESS> emrBusiness;

        public frmChooseBusiness(Action<EMR_BUSINESS> _actChoose, List<EMR_BUSINESS> _emrBusiness)
        {
            InitializeComponent();
            this.actChoose = _actChoose;
            this.emrBusiness = _emrBusiness;
        }

        private void frmChooseBusiness_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl1.DataSource = this.emrBusiness;
                gridView1.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridView1.IsEditing)
                        this.gridView1.CloseEditor();

                    if (this.gridView1.FocusedRowModified)
                        this.gridView1.UpdateCurrentRow();

                    btnChoose_Click(null, null);
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnChoose_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridControl1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                btnChoose_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnChoose_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (EMR_BUSINESS)this.gridView1.GetFocusedRow();
                if (data != null)
                {
                    this.actChoose(data);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

    }
}
