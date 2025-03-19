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
using DevExpress.Utils;
using DevExpress.XtraBars.Alerter;
using Inventec.Core;
using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Inventec.Desktop.Common.Message
{
    public class MessageManager
    {
        public static int AutoFormDelay = 1000;
        public const int DefaultFontSize = 12;

        public static void Show(CommonParam param, bool? success)
        {
            try
            {
                if (success.HasValue)
                {
                    MessageUtil.SetResultParam(param, success.Value);
                }
                string message = MessageUtil.GetMessageAlert(param);
                if (!String.IsNullOrEmpty(message))
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Show(string message)
        {
            try
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Show(System.Windows.Forms.Form owner, CommonParam param, bool? success)
        {
            try
            {
                bool showAlert = false;
                if (success.HasValue)
                {
                    if (success.Value && (param.Messages == null || param.Messages.Count == 0))
                    {
                        showAlert = true;
                    }
                    MessageUtil.SetResultParam(param, success.Value);
                }

                string message = MessageUtil.GetMessageAlert(param);
                if (showAlert)
                {
                    if (!String.IsNullOrEmpty(message))
                        ShowAlert(owner, "", message);
                }
                else
                {
                    if (!String.IsNullOrEmpty(message))
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void Show(System.Windows.Forms.Form owner, CommonParam param, bool? success, AlertFormLocation formLocation)
        {
            try
            {
                bool showAlert = false;
                if (success.HasValue)
                {
                    if (success.Value && (param.Messages == null || param.Messages.Count == 0))
                    {
                        showAlert = true;
                    }
                    MessageUtil.SetResultParam(param, success.Value);
                }

                string message = MessageUtil.GetMessageAlert(param);
                if (showAlert)
                {
                    if (!String.IsNullOrEmpty(message))
                        ShowAlert(owner, "", message, AutoFormDelay, formLocation);
                }
                else
                {
                    if (!String.IsNullOrEmpty(message))
                        DevExpress.XtraEditors.XtraMessageBox.Show(message, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), DefaultBoolean.True);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowAlert(System.Windows.Forms.Form owner, CommonParam param, bool? success)
        {
            try
            {
                if (success.HasValue)
                    MessageUtil.SetResultParam(param, success.Value);
                string message = MessageUtil.GetMessageAlert(param);
                if (!String.IsNullOrEmpty(message))
                {
                    ShowAlert(owner, "", message, AutoFormDelay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowAlert(System.Windows.Forms.Form owner, string caption, string message)
        {
            try
            {
                ShowAlert(owner, caption, message, AutoFormDelay);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowAlert(System.Windows.Forms.Form owner, CommonParam param, bool? success, int autoFormDelay)
        {
            try
            {
                if (success.HasValue)
                    MessageUtil.SetResultParam(param, success.Value);
                string message = MessageUtil.GetMessageAlert(param);
                if (!String.IsNullOrEmpty(message))
                {
                    AlertControl alert = new AlertControl();
                    alert.ShowPinButton = true;
                    alert.ShowCloseButton = true;
                    alert.AppearanceCaption.TextOptions.HAlignment = HorzAlignment.Center;
                    alert.AppearanceText.TextOptions.HAlignment = HorzAlignment.Center;
                    alert.AppearanceText.TextOptions.WordWrap = WordWrap.Wrap;
                    alert.AutoFormDelay = autoFormDelay;
                    alert.FormLocation = AlertFormLocation.BottomLeft;
                    alert.AppearanceCaption.ForeColor = Color.Green;
                    alert.AppearanceCaption.Font = new Font(alert.AppearanceCaption.Font.FontFamily, 12, FontStyle.Bold);
                    alert.Show(owner, message, "");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowAlert(System.Windows.Forms.Form owner, string caption, string message, int autoFormDelay)
        {
            try
            {
                ShowAlert(owner, caption, message, autoFormDelay, AlertFormLocation.TopRight);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowAlert(System.Windows.Forms.Form owner, string caption, string message, int autoFormDelay, AlertFormLocation formLocation)
        {
            bool isShowCenter = true;
            try
            {
                ShowAlert(owner, caption, message, autoFormDelay, formLocation, DefaultFontSize, Color.Green, FontStyle.Bold, isShowCenter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void ShowAlert(System.Windows.Forms.Form owner, string caption, string message, int autoFormDelay, AlertFormLocation formLocation, int fontSize, Color color, FontStyle fontStyle, bool ? isShowScreenCenter)
        {
            try
            {
                AlertControl alert = new AlertControl();
                alert.ShowPinButton = true;
                alert.ShowCloseButton = true;
                alert.AppearanceCaption.TextOptions.HAlignment = HorzAlignment.Center;
                alert.AppearanceText.TextOptions.HAlignment = HorzAlignment.Center;
                alert.AppearanceText.TextOptions.WordWrap = WordWrap.Wrap;
                alert.AppearanceCaption.ForeColor = color;
                alert.AppearanceCaption.Font = new Font(alert.AppearanceCaption.Font.FontFamily, fontSize, fontStyle);
                alert.AutoFormDelay = autoFormDelay;
                if (isShowScreenCenter.Value)
                    alert.BeforeFormShow += beforeShowAlert;
                else
                {
                    if (formLocation != null)
                        alert.FormLocation = formLocation;
                } 
                alert.Show(owner, message, "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void ShowAlert(System.Windows.Forms.Form owner, string caption, string message, int autoFormDelay, AlertFormControlBoxPosition formPosition, int fontSize, Color color, FontStyle fontStyle)
        {
            try
            {
                AlertControl alert = new AlertControl();
                alert.ShowPinButton = true;
                alert.ShowCloseButton = true;
                alert.AppearanceCaption.TextOptions.HAlignment = HorzAlignment.Center;
                alert.AppearanceText.TextOptions.HAlignment = HorzAlignment.Center;
                alert.AppearanceText.TextOptions.WordWrap = WordWrap.Wrap;
                alert.AppearanceCaption.ForeColor = color;
                alert.AppearanceCaption.Font = new Font(alert.AppearanceCaption.Font.FontFamily, fontSize, fontStyle);
                alert.AutoFormDelay = autoFormDelay;
                alert.ControlBoxPosition = formPosition;
                alert.Show(owner, message, "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        // set point before show
        private static void beforeShowAlert(object sender, DevExpress.XtraBars.Alerter.AlertFormEventArgs e)
        {
            AlertControl control = sender as AlertControl;
            Point point = e.Location;
            Rectangle rect = Screen.GetWorkingArea(Screen.PrimaryScreen.Bounds);
            point.X = (rect.Width - e.AlertForm.Width) / 2;
            point.Y = (rect.Height - e.AlertForm.Height) / 2;
            e.Location = point;

        }


        //public static void ShowCenter(System.Windows.Forms.Form owner, string caption, string message, int autoFormDelay)
        //{
        //    try
        //    {
        //        AlertControl alert = new AlertControl();
        //        alert.ShowPinButton = true;
        //        alert.ShowCloseButton = true;
        //        alert.AppearanceCaption.TextOptions.HAlignment = HorzAlignment.Center;
        //        alert.AppearanceText.TextOptions.HAlignment = HorzAlignment.Center;
        //        alert.AppearanceText.TextOptions.WordWrap = WordWrap.Wrap;
        //        alert.AutoFormDelay = autoFormDelay;
        //        alert.BeforeFormShow += beforeShowAlert;
        //        alert.Show(owner, message, "");
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
