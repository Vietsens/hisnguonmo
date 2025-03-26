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
using HIS.Desktop.LocalStorage.LocalData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.Loading.Design.Template1
{
    internal partial class Template1
    {
        internal void SetDescription(string description)
        {
            try
            {
                progressLoading.Description = description;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal void ReportProgress(int i)
        {
            try
            {
                worker.ReportProgress(i);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                worker = sender as BackgroundWorker;
                if (_DoWorker != null) _DoWorker();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if ((e.Cancelled == true))
                {
                    progressLoading.Visible = false;
                    progressLoading.Description = "Hủy bỏ!";

                }
                else if (!(e.Error == null))
                {
                    progressLoading.Visible = false;
                    progressLoading.Description = "Lỗi: " + e.Error.Message;
                }
                else
                {
                    progressLoading.Visible = false;
                    progressLoading.Description = "Thành công!";

                    if (_RunCompleted != null) _RunCompleted();
                    //Hide form login
                    //this.Close();

                    //try
                    //{
                    //    System.Threading.Thread.CurrentThread.CurrentCulture = ICA.APP.Base.LanguageManager.GetCulture();
                    //    System.Globalization.CultureInfo.DefaultThreadCurrentCulture = ICA.APP.Base.LanguageManager.GetCulture();
                    //}
                    //catch (Exception ex)
                    //{
                    //    Inventec.Common.Logging.LogSystem.Warn("Set CurrentCulture loi: " + ex);
                    //}

                    //if (ICA.MANAGER.Config.Loader.dictionaryConfig == null || ICA.MANAGER.Config.Loader.dictionaryConfig.Count == 0)
                    //{
                    //    ICA.MANAGER.Config.Loader.RefreshConfig();
                    //}

                    //Vao trang chu
                    //frmMain fMain = new frmMain();
                    //fMain.ShowDialog();

                    //Show form login
                    //this.Show();
                    //LoginMain.SetLoadFocus(UCLogin);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                //LoginMain.SetButtonEnable(UCLogin, true);
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                int percent = e.ProgressPercentage;
                string content = null;
                switch (GlobalVariables.LoadType)
                {
                    case GlobalVariables.Function.KetNoiHeThongMayChu:
                        content = ResourceMessage.DangKetNoiMayChuHeThong;
                        break;
                    case GlobalVariables.Function.TaiDuLieuCauHinh:
                        content = String.Format(ResourceMessage.DangTaiDuLieu, String.Format("{0}%", percent));
                        break;
                    case GlobalVariables.Function.ThietLapMenu:
                        content = ResourceMessage.DangThietLapThamSoHeThong;
                        break;
                    case GlobalVariables.Function.NapModule:
                        content = String.Format(ResourceMessage.DangNapChucNang, GlobalVariables.CurrentNumberModule, GlobalVariables.NumTotalModule);
                        break;
                    case GlobalVariables.Function.NapMps:
                        content = String.Format(ResourceMessage.DangNapMauIn,GlobalVariables.CurrentNumberMps,GlobalVariables.NumTotalMps);
                        break;
                    default:
                        break;
                }
                progressLoading.Description = content;
                //LoginMain.SetProgressDescription(UCLogin, Description, Text);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
