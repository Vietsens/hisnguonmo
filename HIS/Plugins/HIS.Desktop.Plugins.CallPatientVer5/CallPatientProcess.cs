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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CallPatientVer5;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatient
{
    internal class CallPatientProcess
    {
        internal static string[] FileName, FilePath;
        //internal static void FillDataToGridTop5Patient(frmCallPatient control)
        //{
        //    try
        //    {
        //        if (control != null)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisServiceReqViewFilter searchMVC = new MOS.Filter.HisServiceReqViewFilter();

        //            try
        //            {
        //                if (WorkPlace.GetRoomId() > 0)
        //                {
        //                    searchMVC.EXECUTE_ROOM_ID = WorkPlace.GetRoomId();
        //                }
        //                searchMVC.IS_SHOW = true;
        //            }
        //            catch (Exception ex)
        //            {
        //                LogSystem.Warn(ex);
        //            }
        //            var lstData = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEWUSINGORDER, ApiConsumers.MosConsumer, searchMVC, param);
        //            control.gridControlTop5Patient.BeginUpdate();
        //            control.gridControlTop5Patient.DataSource = lstData.Take(6);
        //            control.gridControlTop5Patient.EndUpdate();
        //            #region Process has exception
        //            SessionManager.ProcessTokenLost(param);
        //            #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Debug(ex);
        //    }
        //}
        //internal static void FillDataToGridWaitingPatient(frmCallPatient control)
        //{
        //    try
        //    {
        //        if (control != null)
        //        {
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.HisServiceReqViewFilter searchMVC = new MOS.Filter.HisServiceReqViewFilter();
        //            try
        //            {
        //                if (WorkPlace.GetRoomId() > 0)
        //                {
        //                    searchMVC.EXECUTE_ROOM_ID = WorkPlace.GetRoomId();
        //                }
        //                searchMVC.IS_SHOW = true;
        //            }
        //            catch (Exception ex)
        //            {
        //                LogSystem.Warn(ex);
        //            }
        //            var lstData = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEWUSINGORDER, ApiConsumers.MosConsumer, searchMVC, param);
        //            if (lstData != null)
        //            {
        //                control.gridControlWaitingPatient.BeginUpdate();
        //                control.gridControlWaitingPatient.DataSource = lstData;
        //                control.gridControlWaitingPatient.EndUpdate();
        //            }
        //            else
        //            {
        //                control.gridControlWaitingPatient.BeginUpdate();
        //                control.gridControlWaitingPatient.DataSource = null;
        //                control.gridControlWaitingPatient.EndUpdate();
        //            }
        //            #region Process has exception
        //            SessionManager.ProcessTokenLost(param);
        //            #endregion
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }
        //}

        //public static void playfile(frmCallPatient control, int playlistindex)
        //{
        //    try
        //    {
        //        if (playlistindex < 0)
        //        {
        //            return;
        //        }
        //        control.WindowsMediaPlayerQC.settings.autoStart = true;
        //        control.WindowsMediaPlayerQC.URL = FilePath[playlistindex];
        //        control.WindowsMediaPlayerQC.Ctlcontrols.next();
        //        control.WindowsMediaPlayerQC.Ctlcontrols.play();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        public static void GetFilePath()
        {
            try
            {

                //FilePath = Directory.GetFiles(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
                FilePath = Directory.GetFiles(ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__DUONG_DAN_CHAY_FILE_VIDEO));
         
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
