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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientCashier.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientCashier
{
    public partial class frmWaitingScreenCashier : HIS.Desktop.Utility.FormBase
    {
        int countTimer = 0;
        V_HIS_ROOM currentRoom;

        public frmWaitingScreenCashier(Inventec.Desktop.Common.Modules.Module moduleData, V_HIS_ROOM room)
            : base(moduleData)
        {
            InitializeComponent();
            this.currentRoom = room;
            this.Name = "CALL_PATIENT_CASHIER_" + this.currentRoom.ROOM_CODE;
            SetIcon();
            HisConfigCFG.LoadConfigs();
            this.SetBackground();
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

        private void frmWaitingScreenCashier_Load(object sender, EventArgs e)
        {
            try
            {
                LoadValueToControl();
                //RegisterTimer(ModuleLink, "timer", 1000, ChangeColor);
                //StartTimer(ModuleLink, "timer");
                timer.Interval = 1000;
                timer.Enabled = true;
                timer.Start();
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                this.TopMost = true;
                this.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadValueToControl()
        {
            try
            {
                lblOrganizationName.Text = BranchDataWorker.Branch.BRANCH_NAME.ToUpper();
                
                var organ_colors = HisConfigCFG.ORGANIZATION_NAME_COLOR_CODES;
                if (organ_colors != null && organ_colors.Count == 3)
                {
                    lblOrganizationName.ForeColor = System.Drawing.Color.FromArgb(organ_colors[0], organ_colors[1], organ_colors[2]);
                }
                var room_colors = HisConfigCFG.ROOM_NAM_COLOR_CODES;
                if (room_colors != null && room_colors.Count == 3)
                {
                    lblRegisterRoom.ForeColor = System.Drawing.Color.FromArgb(room_colors[0], room_colors[1], room_colors[2]);
                }
                if (HisConfigCFG.TITLE_SIZE > 0)
                {
                    lblOrganizationName.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), HisConfigCFG.TITLE_SIZE, FontStyle.Bold);
                    lblInvitingTitle.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), HisConfigCFG.TITLE_SIZE, FontStyle.Bold);
                    lblRegisterRoom.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), HisConfigCFG.TITLE_SIZE, FontStyle.Bold);
                    int size = (int)(HisConfigCFG.TITLE_SIZE * 0.6);
                    lblProcessExam.Appearance.Font = new System.Drawing.Font(new FontFamily("Arial"), size, FontStyle.Bold);
                }

                if (HisConfigCFG.NUM_ORDER_SIZE > 0)
                {
                    lblNumOrder.Appearance.Font = new Font(new FontFamily("Arial"), HisConfigCFG.NUM_ORDER_SIZE, FontStyle.Bold);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBackground()
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(HisConfigCFG.BACKGROUND_IMAGE))
                {
                    this.layoutControlGroup1.BackgroundImage = Image.FromFile(ApplicationStoreLocation.ApplicationDirectory + HisConfigCFG.BACKGROUND_IMAGE);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SetNumOrder(int num, int counters)
        {
            try
            {
                if (num >= 0)
                {
                    lblNumOrder.Invoke(new MethodInvoker(delegate
                    {
                        lblNumOrder.Text = num + "";
                        timer_Tick(null, null);
                    }));
                }

                lblRegisterRoom.Text = ("VÀO QUẦY THU NGÂN SỐ " + counters).ToUpper();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadChangeColor);
            }
            catch (Exception ex)
            {
                
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ExecuteThreadChangeColor()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { ChangeColor(); }));
                }
                else
                {
                    ChangeColor();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeColor()
        {
            try
            {
                countTimer++;
                if (countTimer == 1 || countTimer == 3 || countTimer == 5)
                {
                    lblNumOrder.ForeColor = System.Drawing.Color.FromArgb(40, 255, 40);
                    timer.Start();
                }
                if (countTimer == 0 || countTimer == 2 || countTimer == 4 || countTimer == 6)
                {
                    lblNumOrder.ForeColor = Color.Red;
                    timer.Start();
                }
                if (countTimer > 6)
                {
                    //StopTimer(ModuleLink, "timer");
                    timer.Stop();
                    countTimer = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
