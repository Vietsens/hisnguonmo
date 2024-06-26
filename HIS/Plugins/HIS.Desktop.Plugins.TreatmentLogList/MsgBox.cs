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
using Inventec.Common.Logging;
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.TreatmentLogList
{
    public class MsgBox
    {
        public enum CaptionEnum
        {
            ThongBao,
            CanhBao,
            Loi,
        }

        private static string GetCaption(CaptionEnum captionEnum)
        {
            string result = "";
            try
            {
                switch (captionEnum)
                {
                    case CaptionEnum.ThongBao:
                        result = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao);
                        break;
                    case CaptionEnum.CanhBao:
                        result = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao);
                        break;
                    case CaptionEnum.Loi:
                        result = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaLoi);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                result = "";
                LogSystem.Error(ex);
            }
            return result;
        }

        public static System.Windows.Forms.DialogResult Show(string text, CaptionEnum captionEnum)
        {
            return Show(text, GetCaption(captionEnum), MessageBoxButtons.OK);
        }

        public static System.Windows.Forms.DialogResult Show(string text, CaptionEnum captionEnum, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon)
        {
            return Show(text, GetCaption(captionEnum), buttons, icon);
        }

        public static System.Windows.Forms.DialogResult Show(string text)
        {
            return Show(text, string.Empty);
        }

        public static System.Windows.Forms.DialogResult Show(string text, string caption)
        {
            return Show(text, caption, MessageBoxButtons.OK);
        }

        public static System.Windows.Forms.DialogResult Show(string text, string caption, System.Windows.Forms.MessageBoxButtons buttons)
        {

            return Show(text, caption, buttons, MessageBoxIcon.None);
        }

        public static System.Windows.Forms.DialogResult Show(string text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon)
        {

            return Show(text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static System.Windows.Forms.DialogResult Show(string text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon, System.Windows.Forms.MessageBoxDefaultButton defaultButton)
        {

            return Show(text, caption, buttons, icon, defaultButton, (MessageBoxOptions)default(System.Windows.Forms.MessageBoxOptions));
        }

        public static System.Windows.Forms.DialogResult Show(string text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon, System.Windows.Forms.MessageBoxDefaultButton defaultButton, System.Windows.Forms.MessageBoxOptions options)
        {

            return Show(null, text, caption, buttons, icon, defaultButton, options);
        }

        public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner, string text)
        {

            return Show(null, text, string.Empty);
        }

        public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption)
        {

            return Show(owner, text, caption, MessageBoxButtons.OK);
        }

        public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption, System.Windows.Forms.MessageBoxButtons buttons)
        {

            return Show(owner, text, caption, buttons, MessageBoxIcon.None);
        }

        public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon)
        {

            return Show(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1);
        }

        public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon, System.Windows.Forms.MessageBoxDefaultButton defaultButton)
        {

            return Show(owner, text, caption, buttons, icon, defaultButton, (MessageBoxOptions)default(System.Windows.Forms.MessageBoxOptions));
        }

        public static System.Windows.Forms.DialogResult Show(System.Windows.Forms.IWin32Window owner, string text, string caption, System.Windows.Forms.MessageBoxButtons buttons, System.Windows.Forms.MessageBoxIcon icon, System.Windows.Forms.MessageBoxDefaultButton defaultButton, System.Windows.Forms.MessageBoxOptions options)
        {

            CSysMsgBoxHook cysymsgbox = new CSysMsgBoxHook();

            cysymsgbox.HookDialog();
            DialogResult res = default(DialogResult);

            res = MessageBox.Show(owner, text, caption, buttons, icon, defaultButton, options);
            //res = CType(MsgBox(text, MsgBoxStyle.OkCancel, caption), DialogResult)

            cysymsgbox.UnHookDialog();
            cysymsgbox = null;

            return res;
        }

        public static void Show(Form owner, DevExpress.XtraBars.Alerter.AlertControl alertControl, CaptionEnum captionEnum, string text, int? timeout)
        {
            if (timeout.HasValue)
            {
                alertControl.AutoFormDelay = timeout.Value;
            }
            else
                alertControl.AutoFormDelay = 5000;
            alertControl.Show(owner, GetCaption(captionEnum), text);
        }

        public static void Show(Form owner, DevExpress.XtraBars.Alerter.AlertControl alertControl, CaptionEnum captionEnum, string text)
        {
            Show(owner, alertControl, captionEnum, text, null);
        }
    }

    #region  Class CSysMsgBoxHook

    public class CSysMsgBoxHook
    {

        #region  API Declarations

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SetDlgItemTextW(IntPtr hWnd, int nIDDlgItem, IntPtr lpString);

        [DllImport("gdi32.dll", EntryPoint = "CreateFontA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        // VBConversions Note: Parameter W renamed to W1. Parameter names must be unique to avoid a C# compiler error CS0100.
        public static extern int CreateFont(int H, int W, int E, int O, int W1, int I, int u, int S, int C, int OP, int CP, int Q, int PAF, string F);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowTextW", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int SetWindowText(IntPtr hwnd, IntPtr lpString);

        [DllImport("user32.dll", EntryPoint = "FindWindowExA", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        public static extern int FindWindowEx(IntPtr hWnd1, int hWnd2, string lpsz1, string lpsz2);


        #endregion

        #region  Delegates

        private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        #endregion

        #region  Private Constants

        #region  System

        private const long WH_CBT = 0x5;
        private const long HCBT_ACTIVATE = 0x5;

        #endregion

        #region  Buttons

        private const int ID_BUT_OK = 0x1;
        private const int ID_BUT_CANCEL = 0x2;
        private const int ID_BUT_ABORT = 0x3;
        private const int ID_BUT_RETRY = 0x4;
        private const int ID_BUT_IGNORE = 0x5;
        private const int ID_BUT_YES = 0x6;
        private const int ID_BUT_NO = 0x7;

        private const string BUT_OK = "Đồng ý";
        private const string BUT_CANCLE = "Hủy";
        private const string BUT_ABORT = "Kết thúc";
        private const string BUT_RETRY = "Làm lại";
        private const string BUT_IGNORE = "Bỏ qua";
        private const string BUT_YES = "Có";
        private const string BUT_NO = "Không";

        #endregion

        #endregion

        #region  Private variables

        private IntPtr _hook;

        #endregion

        #region  Public methods

        public void HookDialog()
        {

            this.UnHookDialog();

            _hook = SetWindowsHookEx((int)WH_CBT, new HookProc(DialogHookProc), IntPtr.Zero, AppDomain.GetCurrentThreadId());
        }

        public void UnHookDialog()
        {
            if (!_hook.Equals(IntPtr.Zero))
            {
                UnhookWindowsHookEx(_hook);
            }
        }

        #endregion

        #region  Private functions

        private IntPtr DialogHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode < 0)
            {
                return CallNextHookEx(_hook, nCode, wParam, lParam);
            }

            if (nCode == HCBT_ACTIVATE)
            {

                SetDlgItemTextW(wParam, ID_BUT_OK, Marshal.StringToHGlobalUni(BUT_OK));
                SetDlgItemTextW(wParam, ID_BUT_CANCEL, Marshal.StringToHGlobalUni(BUT_CANCLE));
                SetDlgItemTextW(wParam, ID_BUT_ABORT, Marshal.StringToHGlobalUni(BUT_ABORT));
                SetDlgItemTextW(wParam, ID_BUT_RETRY, Marshal.StringToHGlobalUni(BUT_RETRY));
                SetDlgItemTextW(wParam, ID_BUT_IGNORE, Marshal.StringToHGlobalUni(BUT_IGNORE));
                SetDlgItemTextW(wParam, ID_BUT_YES, Marshal.StringToHGlobalUni(BUT_YES));
                SetDlgItemTextW(wParam, ID_BUT_NO, Marshal.StringToHGlobalUni(BUT_NO));

                UnhookWindowsHookEx(_hook);
            }

            return CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        #endregion

    }

    #endregion
}
