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
using ACS.SDO;
using DevExpress.XtraBars.Ribbon;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        int timeTimeoutCFG = 0;
        int timeCompareCFG = 0;
        private void RunCheckTokenTimeout()
        {
            try
            {
                string checktimeTimeoutCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.KEY__HIS_DESKTOP_TIME_APPLICATION_CHECK_TOKEN_TIMEOUT);

                if (!String.IsNullOrEmpty(checktimeTimeoutCFG))
                {
                    var spTimes = checktimeTimeoutCFG.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (spTimes != null && spTimes.Count() > 0)
                    {
                        this.timeTimeoutCFG = Convert.ToInt32(spTimes[0]);
                        this.timeCompareCFG = Convert.ToInt32(spTimes[1]);
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checktimeTimeoutCFG), checktimeTimeoutCFG) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.timeTimeoutCFG), this.timeTimeoutCFG) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.timeCompareCFG), this.timeCompareCFG));
                if (timeTimeoutCFG > 0)
                {
                    this.timeCompareCFG = (this.timeCompareCFG == 0 ? 5 : this.timeCompareCFG);
                    Inventec.Common.Logging.LogSystem.Debug("timeCompareCFG sau khi kiem tra va tinh lai theo don vị phut" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.timeCompareCFG), this.timeCompareCFG));

                    System.Windows.Forms.Timer timerNotify = new System.Windows.Forms.Timer();
                    timerNotify.Interval = (this.timeTimeoutCFG * 1000 * 60);
                    timerNotify.Enabled = true;
                    timerNotify.Tick += TimerApplicationTimeout_Tick;
                    timerNotify.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimerApplicationTimeout_Tick(object sender, EventArgs e)
        {
            try
            {
                var token = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                long expireTime = 0;
                if (token != null)
                {
                    expireTime = Inventec.Common.TypeConvert.Parse.ToInt64(token.ExpireTime.ToString("yyyyMMddHHmm"));
                }

                long localTime = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMddHHmm"));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => expireTime), expireTime) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => localTime), localTime));
                if (expireTime - localTime > 0 && expireTime - localTime <= this.timeCompareCFG)
                {
                    this.lblTokenTimeout.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Red;
                    this.lblTokenTimeout.ItemAppearance.Normal.Font = new Font(this.lblTokenTimeout.ItemAppearance.Normal.Font, FontStyle.Bold);
                }
                else if (expireTime - localTime <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("TimerApplicationTimeout_Tick.1");
                    this.lblTokenTimeout.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Red;
                    this.lblTokenTimeout.ItemAppearance.Normal.Font = new Font(this.lblTokenTimeout.ItemAppearance.Normal.Font, FontStyle.Bold);

                    var w = new Form() { Size = new Size(0, 0) };
                    System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10))
                        .ContinueWith((t) => w.Close(), System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
                    Inventec.Common.Logging.LogSystem.Info("TimerApplicationTimeout_Tick.2");
                    if (MessageBox.Show(w, String.Format(HIS.Desktop.Resources.ResourceCommon.PhienLamViecConHieuLucDen, token.ExpireTime.ToString("dd/MM/yyyy HH:mm")) + " " + HIS.Desktop.Resources.ResourceCommon.ThongBaoBanCoMuonThoatPhanMem, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Inventec.Common.Logging.LogSystem.Info("TimerApplicationTimeout_Tick.3");
                        Inventec.Common.Logging.LogSystem.Info(String.Format(HIS.Desktop.Resources.ResourceCommon.PhienLamViecConHieuLucDen, token.ExpireTime.ToString("dd/MM/yyyy HH:mm")) + " " + HIS.Desktop.Resources.ResourceCommon.ThongBaoBanCoMuonThoatPhanMem + "___nguoi dung cho co");
                        //GlobalVariables.IsLostToken = true;
                        //this.Close();
                        this.LogoutAndResetToDefault();
                    }
                    Inventec.Common.Logging.LogSystem.Info("TimerApplicationTimeout_Tick.4");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
