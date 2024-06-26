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
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
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

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.MaternityLeave
{
    public partial class frmMaternityLeave : FormBase
    {
        private bool Check()
        {
            bool result = true;
            try
            {
                List<MaternityLeaveData> maternityLeaveDatas = gridControlMaternityLeave.DataSource as List<MaternityLeaveData>;
                if (maternityLeaveDatas == null && maternityLeaveDatas.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy thông tin con!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                MaternityLeaveData maternityLeaveData = maternityLeaveDatas.FirstOrDefault(o => 
                    !o.BornTimeDt.HasValue 
                    && String.IsNullOrEmpty(o.FatherName) 
                    && !o.GenderId.HasValue 
                    && !o.Weight.HasValue);
                if (maternityLeaveData != null)
                {
                    MessageBox.Show("Tồn tại dòng dữ liệu trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
    }
}
