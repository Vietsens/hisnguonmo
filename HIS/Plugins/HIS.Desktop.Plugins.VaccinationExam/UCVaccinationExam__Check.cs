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
using DevExpress.Data;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Base;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.VaccinationExam.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Utility;

namespace HIS.Desktop.Plugins.VaccinationExam
{
    public partial class UCVaccinationExam : UserControlBase
    {
        private bool CheckAssignee()
        {
            bool result = true;
            try
            {
                result = result && CheckExistDataVaccinationMety();
                result = result && CheckNotNullVacciantionMety();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckNotNullVacciantionMety()
        {
            bool result = true;
            try
            {
                List<ExpMestMedicineADO> vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                List<ExpMestMedicineADO> vaccinationMetyADODatas = vaccinationMetyADOs != null ?
                    vaccinationMetyADOs.Where(o => o.AMOUNT > 0 && o.TDL_MEDICINE_TYPE_ID > 0).ToList() : null;
                if (vaccinationMetyADODatas == null || vaccinationMetyADODatas.Count == 0 || vaccinationMetyADOs.Count != vaccinationMetyADODatas.Count)
                {
                    MessageBox.Show("Không tìm thấy thông tin mũi tiêm hoặc sai dữ liệu mũi tiêm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistDataVaccinationMety()
        {
            bool result = true;
            try
            {
                List<ExpMestMedicineADO> vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                List<ExpMestMedicineADO> vaccinationMetyADODatas = vaccinationMetyADOs != null ?
                    vaccinationMetyADOs.Where(o => o.AMOUNT > 0 && o.TDL_MEDICINE_TYPE_ID > 0).ToList() : null;
                if (vaccinationMetyADODatas != null && vaccinationMetyADODatas.Count > 0)
                {
                    var vaccinationGroup = vaccinationMetyADODatas.GroupBy(o => new { o.TDL_MEDICINE_TYPE_ID });
                    foreach (var g in vaccinationGroup)
                    {
                        if (g.Count() > 1)
                        {
                            MessageBox.Show("Tồn tại dữ liệu trùng nhau", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckAppointment()
        {
            bool result = true;
            try
            {
                result = result && CheckNotNullVaccAppointment();
                result = result && CheckExistDataVaccAppointment();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool CheckExistDataVaccAppointment()
        {
            bool result = true;
            try
            {
                List<AppointmentVaccineADO> appointmentVaccineADOs = gridControlAppointmentVacc.DataSource as List<AppointmentVaccineADO>;
                List<AppointmentVaccineADO> vaccineADODatas = appointmentVaccineADOs != null ? appointmentVaccineADOs.Where(o => (o.VACCINE_TYPE_ID ?? 0) > 0).ToList() : null;
                if (vaccineADODatas != null && vaccineADODatas.Count > 0)
                {
                    var vaccinationGroup = vaccineADODatas.GroupBy(o => o.VACCINE_TYPE_ID);
                    List<string> lstVaccine = new List<string>();
                    foreach (var g in vaccinationGroup)
                    {
                        if (g.Count() > 1)
                        {
                            var vacc = this.VaccineTypes.FirstOrDefault(o => o.ID == g.Key);
                            if (vacc != null)
                            {
                                lstVaccine.Add(vacc.VACCINE_TYPE_NAME);
                            }
                        }
                    }

                    if (lstVaccine != null && lstVaccine.Count > 0)
                    {
                        MessageBox.Show(string.Format("Vắc xin {0} bị trùng", string.Join(",", lstVaccine)), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                List<AppointmentVaccineADO> vaccineADODataNullTurns = appointmentVaccineADOs != null ? appointmentVaccineADOs.Where(o => !o.VACCINE_TURN.HasValue).ToList() : null;
                if (vaccineADODataNullTurns != null && vaccineADODataNullTurns.Count > 0)
                {
                    MessageBox.Show("Thiếu thông tin lần tiêm của mũi tiêm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private bool CheckNotNullVaccAppointment()
        {
            bool result = true;
            try
            {
                List<AppointmentVaccineADO> appointmentVaccineADOs = gridControlAppointmentVacc.DataSource as List<AppointmentVaccineADO>;
                List<AppointmentVaccineADO> VaccineADODatas = appointmentVaccineADOs != null ? appointmentVaccineADOs.Where(o => (o.VACCINE_TYPE_ID ?? 0) > 0).ToList() : null;
                if (appointmentVaccineADOs == null || VaccineADODatas == null || VaccineADODatas.Count == 0 || appointmentVaccineADOs.Count != VaccineADODatas.Count)
                {
                    MessageBox.Show("Không tìm thấy thông tin mũi tiêm hoặc sai dữ liệu mũi tiêm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    result = false;
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
