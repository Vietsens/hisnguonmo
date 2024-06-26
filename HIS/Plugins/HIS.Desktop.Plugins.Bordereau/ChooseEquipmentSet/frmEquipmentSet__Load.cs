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
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChooseEquipmentSet
{
    public partial class frmEquipmentSet : Form
    {
        private void LoadComboEquipment()
        {
            try
            {
                List<HIS_EQUIPMENT_SET> listEquipment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_EQUIPMENT_SET>(false, true);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EQUIPMENT_SET_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EQUIPMENT_SET_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboEquipmentSet, listEquipment, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataDefault()
        {
            try
            {
                cboEquipmentSet.EditValue = equipmentId.HasValue ? equipmentId : null;
                spinNumOrder.Value = (decimal) (numOrder.HasValue ? numOrder : 1);
                if (controlType == CONTROL_TYPE.HIDE_NUMORDER)
                {
                    //spinNumOrder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }
    }
}
