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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Data;
using HIS.Desktop.Utility;
using HIS.UC.WorkPlace;
using IMSys.DbConfig.HIS_RS;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.SickLeave
{
    public partial class frmSickLeave : FormBase
    {
        private bool Check()
        {
            bool result = true;
            try
            {
                if (spinSickLeaveDay.EditValue == null || spinSickLeaveDay.Value <= 0)
                {

                    if (type == FormEnum.TYPE.NGHI_DUONG_THAI)
                    {
                        MessageBox.Show("Thông tin nghỉ dưỡng thai thiếu số ngày nghỉ. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                        //if (type == FormEnum.TYPE.NGHI_VIEC_HUONG_BHXH)
                        {
                            MessageBox.Show("Thông tin nghỉ hưởng BHXH thiếu số ngày nghỉ. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        //else
                        //{
                        //    MessageBox.Show("Số ngày nghỉ phải lớn hơn 0. Vui lòng kiểm tra lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //}
                    return false;
                }

                if (dtSickLeaveFromTime.EditValue != null && dtSickLeaveToTime.EditValue != null
                    && dtSickLeaveFromTime.DateTime.Date > dtSickLeaveToTime.DateTime.Date)
                {
                    MessageBox.Show("Số ngày nghỉ từ lớn hơn ngày đến", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                //if (type == FormEnum.TYPE.NGHI_DUONG_THAI)
                //{
                //    List<BabyADO> babyADOs = gridControlMaternityLeave.DataSource as List<BabyADO>;
                //    if (babyADOs == null && babyADOs.Count == 0)
                //    {
                //        MessageBox.Show("Không tìm thấy thông tin con!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return false;
                //    }

                //    BabyADO babyADO = babyADOs.FirstOrDefault(o =>
                //        !o.BornTimeDt.HasValue
                //        && String.IsNullOrEmpty(o.FatherName)
                //        && !o.GenderId.HasValue
                //        && !o.Weight.HasValue);
                //    if (babyADO != null)
                //    {
                //        MessageBox.Show("Tồn tại dòng dữ liệu trống", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //        return false;
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
