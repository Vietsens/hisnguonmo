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
using Inventec.Aup.Client.AutoUpdater;
using Inventec.Common.Logging;
using System;
using System.Net;
using System.Windows.Forms;
using System.Xml;

namespace Inventec.Aup.Client
{
    public class VersionV2_0
    {
        public VersionV2_0() { }

        public void Update(string cfgAupUri, string cfgServerConfigUrl = "", string preThreadName = "", string exePath = "", string preCmd = "")
        {

            #region check and download new version program
            bool bHasError = false;
            IAutoUpdater autoUpdater = new AutoUpdater.AutoUpdater(cfgAupUri, cfgServerConfigUrl, preThreadName, exePath, preCmd);
            try
            {
                autoUpdater.Update();
            }
            catch (WebException exp)
            {
                MessageBox.Show("Kết nối máy chủ thất bại");
                LogSystem.Debug(exp.Message);
                bHasError = true;
            }
            catch (XmlException exp)
            {
                bHasError = true;
                LogSystem.Debug(exp.Message);
                MessageBox.Show("Lỗi khi tải xuống tệp cập nhật");
            }
            catch (NotSupportedException exp)
            {
                bHasError = true;
                LogSystem.Debug(exp.Message);
                MessageBox.Show("Nâng cấp tệp cấu hình thất bại");
            }
            catch (ArgumentException exp)
            {
                bHasError = true;
                LogSystem.Debug(exp.Message);
                MessageBox.Show("Lỗi khi tải xuống tệp nâng cấp");
            }
            catch (Exception exp)
            {
                bHasError = true;
                LogSystem.Debug(exp.Message);
                MessageBox.Show("Đã xảy ra lỗi trong quá trình cập nhật");
            }
            finally
            {
                if (bHasError == true)
                {
                    try
                    {
                        autoUpdater.RollBack();
                    }
                    catch (Exception)
                    {
                        //Log the message to your file or database
                    }
                }
                OperProcess op = new OperProcess();
                //Bắt đầu quá trình
                op.StartProcess();
                //  this.Close();
            }
            #endregion
        }
    }
}
