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
using HIS.Desktop.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportMestMedicine
{
    public partial class frmHisImportMestMedicine : HIS.Desktop.Utility.FormBase
    {
        long roomId = 0;
        long roomTypeId = 0;
        long impMestTypeId = 0;
        MobaImpMestListADO mobaImpMestListADO = null;

        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmHisImportMestMedicine()
        {
            InitializeComponent();
        }

        public frmHisImportMestMedicine(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
                this.currentModule = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public frmHisImportMestMedicine(Inventec.Desktop.Common.Modules.Module _module, long impMestTypeId, MobaImpMestListADO mobaImpMestListADO)
        {
            InitializeComponent();
            try
            {
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
                this.currentModule = _module;

                this.impMestTypeId = impMestTypeId;
                this.mobaImpMestListADO = mobaImpMestListADO;
                UCHisImportMestMedicine uCHisImportMestMedicine = new UCHisImportMestMedicine(this.currentModule, this.impMestTypeId, this.mobaImpMestListADO);
                this.panelControl1.Controls.Add(uCHisImportMestMedicine);
                uCHisImportMestMedicine.Dock = DockStyle.Fill;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisImportMestMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
