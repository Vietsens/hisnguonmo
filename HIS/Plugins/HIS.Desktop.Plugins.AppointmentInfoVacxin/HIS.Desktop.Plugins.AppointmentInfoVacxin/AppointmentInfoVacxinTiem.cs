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
using Inventec.Aup.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Desktop.Common.Modules;
using Inventec.Aup.Utility;
using System.Configuration;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.AppointmentInfoVacxin
{
    public partial class AppointmentInfoVacxinTiem : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        Module currentModule;
        #endregion
        private bool editdtTimeAppointments = false;
        private bool editcboTimeFrame = false;

        public AppointmentInfoVacxinTiem(Module _Module): base(_Module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _Module;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeAppointments_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!editcboTimeFrame)
                {
                    editdtTimeAppointments = true;
                    DateEdit editor = sender as DateEdit;
                    if (editor != null)
                    {
                        this.CalculateDayNum();

                        if (editor.OldEditValue != null)
                        {
                            DateTime oldValue = (DateTime)editor.OldEditValue;
                            if (oldValue != DateTime.MinValue && (editor.DateTime.Day != oldValue.Day || editor.DateTime.Month != oldValue.Month || editor.DateTime.Year != oldValue.Year))
                            {
                                cboTimeFrame.EditValue = null;
                                cboTimeFrame.Properties.DataSource = null;
                                ProcessLoadAppointmentCount();
                                cboTimeFrame.Properties.DataSource = ListTime;
                                cboTimeFrame.EditValue = ProcessGetFromTime(editor.DateTime);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                editdtTimeAppointments = false;
            }
        }
        private void CalculateDayNum()
        {
            try
            {
                if (dtTimeAppointments.EditValue != null)
                {
                    TimeSpan ts = (TimeSpan)(dtTimeAppointments.DateTime.Date - DateTime.Now.Date);
                   // spDay.Value = ts.Days;
                   // cboTimeFrame.EditValue = ProcessGetFromTime(dtTimeAppointments.DateTime);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateTo()
        {
            try
            {
                if (dtTimeAppointments.EditValue != null)
                {
                    DateTime appoint = DateTime.Now.AddDays((double)(spDay.Value));
                    dtTimeAppointments.DateTime = new DateTime(appoint.Year, appoint.Month, appoint.Day, dtTimeAppointments.DateTime.Hour, dtTimeAppointments.DateTime.Minute, dtTimeAppointments.DateTime.Second);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

