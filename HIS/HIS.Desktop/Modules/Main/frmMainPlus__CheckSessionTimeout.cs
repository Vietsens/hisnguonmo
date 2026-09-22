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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        #region Tu dong dang xuat khi treo may

        /// <summary>So phut treo may toi da, doc tu cau hinh HIS.Desktop.SessionTimeout.</summary>
        int timerSessionTimeoutCFG = 0;

        /// <summary>Chan tick lap khi hop thoai canh bao dang hien (modal van bom message loop).</summary>
        bool isSessionTimeoutHandling = false;

        System.Windows.Forms.Timer timerSessionTimeout;

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        /// <summary>
        /// So phut nguoi dung khong dung chuot / ban phim, tinh tren toan may.
        /// GetLastInputInfo tra ve moc dwTime cua lan nhap lieu cuoi, cung goc thoi gian
        /// voi Environment.TickCount.
        /// </summary>
        private int GetIdleMinutes()
        {
            try
            {
                LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
                lastInputInfo.cbSize = (uint)Marshal.SizeOf(typeof(LASTINPUTINFO));
                if (!GetLastInputInfo(ref lastInputInfo)) return 0;

                //Ep ve uint truoc khi tru: ca TickCount lan dwTime deu quay vong sau ~49,7 ngay,
                //phep tru tren uint van ra dung khoang cach khi vua quay vong.
                uint idleMilliseconds = (uint)Environment.TickCount - lastInputInfo.dwTime;
                return (int)(idleMilliseconds / 60000);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return 0;
            }
        }

        /// <summary>
        /// Bat theo doi thoi gian treo may.
        /// Cau hinh HIS.Desktop.SessionTimeout la so phut; rong / 0 / khong phai so -> bo qua. 
        /// </summary>
        private void RunCheckSessionTimeout()
        {
            try
            {
                string sessionTimeoutCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(
                    HisConfigKeys.CONFIG_KEY__HIS_DESKTOP__SESSION_TIMEOUT);

                if (!String.IsNullOrEmpty(sessionTimeoutCFG))
                {
                    int cfg = 0;
                    if (int.TryParse(sessionTimeoutCFG.Trim(), out cfg))
                    {
                        this.timerSessionTimeoutCFG = cfg;
                    }
                }

                LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => sessionTimeoutCFG), sessionTimeoutCFG)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => this.timerSessionTimeoutCFG), this.timerSessionTimeoutCFG));

                if (this.timerSessionTimeoutCFG > 0)
                {
                    //Tick moi phut chu khong phai moi timerSessionTimeoutCFG phut:
                    //dat bang nguong thi thoi diem phat hien co the tre toi gap doi nguong.
                    timerSessionTimeout = new System.Windows.Forms.Timer();
                    timerSessionTimeout.Interval = 60 * 1000;
                    timerSessionTimeout.Enabled = true;
                    timerSessionTimeout.Tick += TimerSessionTimeout_Tick;
                    timerSessionTimeout.Start();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void TimerSessionTimeout_Tick(object sender, EventArgs e)
        {
            try
            {
                if (isSessionTimeoutHandling) return;

                int idleMinutes = GetIdleMinutes();
                if (idleMinutes < this.timerSessionTimeoutCFG) return;

                isSessionTimeoutHandling = true;
                if (timerSessionTimeout != null) timerSessionTimeout.Stop();

                LogSystem.Warn(String.Format(
                    "Tu dong dang xuat do treo may. LOGINNAME={0}, idleMinutes={1}, cauHinh={2} phut",
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    idleMinutes, this.timerSessionTimeoutCFG));

                WaitingManager.Hide();

                XtraMessageBox.Show(
                    String.Format(
                        "Bạn đã không thao tác trong {0} phút." + Environment.NewLine
                        + "Phần mềm sẽ đăng xuất và quay về màn hình đăng nhập.", idleMinutes),
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                        LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);

                //Dang xuat va khoi dong lai phan mem ve man dang nhap
                LogoutAndResetToDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion
    }
}
