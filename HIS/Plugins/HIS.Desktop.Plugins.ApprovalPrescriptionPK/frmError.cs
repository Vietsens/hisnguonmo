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
using HIS.Desktop.Plugins.ApprovalPrescriptionPK.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalPrescriptionPK
{
    public partial class frmError : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<ErrorADO> lstErrorADO = new List<ErrorADO>();

        public frmError(Inventec.Desktop.Common.Modules.Module module, List<ErrorADO> _lstErrorADO)
            : base(module)
        {
            InitializeComponent();

            try
            {
                this.currentModule = module;
                this.lstErrorADO = _lstErrorADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void frmError_Load(object sender, EventArgs e)
        {
            string Error = "";
            if (this.lstErrorADO != null && lstErrorADO.Count > 0)
            {
                foreach (var item in lstErrorADO)
                {
                    Error += item.ErrorCode + " - " + item.ErrorReason + "\r\n";
                }
            }

            txtError.Text = Error;
        }
    }
}
