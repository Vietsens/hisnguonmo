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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.WaitingScreenPttt.ADO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.WaitingScreenPttt
{
    public partial class frmWaitingScreenPttt : HIS.Desktop.Utility.FormBase
    {
        long departmentId;
        List<long> bedRoomIds;
        int reloadSeconds;
        bool isReloading;

        public frmWaitingScreenPttt(Inventec.Desktop.Common.Modules.Module module, long departmentId, List<long> bedRoomIds, int reloadSeconds)
            : base(module)
        {
            InitializeComponent();
            this.departmentId = departmentId;
            this.bedRoomIds = bedRoomIds;
            this.reloadSeconds = reloadSeconds;
        }

        private void frmWaitingScreenPttt_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                ReloadData();

                timerReload.Interval = reloadSeconds * 1000;
                timerReload.Start();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerReload_Tick(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            if (isReloading)
                return;
            isReloading = true;

            Task.Factory.StartNew(() => WaitingPatientLoader.Load(departmentId, bedRoomIds))
                .ContinueWith(task =>
                {
                    try
                    {
                        if (task.IsFaulted)
                        {
                            Inventec.Common.Logging.LogSystem.Error(task.Exception);
                            return;
                        }
                        if (this.IsDisposed || !this.IsHandleCreated)
                            return;

                        this.BeginInvoke(new MethodInvoker(() =>
                        {
                            gridControlPatient.BeginUpdate();
                            gridControlPatient.DataSource = task.Result;
                            gridControlPatient.EndUpdate();
                        }));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    finally
                    {
                        isReloading = false;
                    }
                });
        }

        private void frmWaitingScreenPttt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void frmWaitingScreenPttt_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerReload.Stop();
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
