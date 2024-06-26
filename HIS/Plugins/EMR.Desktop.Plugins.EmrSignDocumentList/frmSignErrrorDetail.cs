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
using EMR.Desktop.Plugins.EmrSignDocumentList.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMR.Desktop.Plugins.EmrSignDocumentList
{
    public partial class frmSignErrrorDetail : Form
    {
        List<SignErrorADO> signErrorADOs;
        public frmSignErrrorDetail(List<SignErrorADO> _signErrorADOs)
        {
            InitializeComponent();
            this.signErrorADOs = _signErrorADOs;
        }

        private void frmSignErrrorDetail_Load(object sender, EventArgs e)
        {
            ShowErrorToControl();
        }

        internal void ShowErrorToControl()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => signErrorADOs), signErrorADOs));
                if (signErrorADOs != null && signErrorADOs.Count > 0)
                {
                    foreach (var item in signErrorADOs)
                    {
                        txtErrorList.Text += item.NAME + "\r\n";
                    }
                }
                else
                {
                    txtErrorList.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
