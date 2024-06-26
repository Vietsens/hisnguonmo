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
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using HIS.Desktop.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;

namespace HIS.Desktop.Plugins.CallPatientVer5
{
    public class ChooseRoomForWaitingScreenProcess
    {
        const string frmWaitingScreen9 = "frmWaitingScreen9";
        const string frmWaitingScreenQy = "frmWaitingScreen_QY9";
        const string frmWaitingExam9 = "frmWaitingExam9";


        internal static void ShowFormInExtendMonitor(frmWaitingScreen9 control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    Screen secondScreen = sc.FirstOrDefault(o => o != Screen.PrimaryScreen);
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = secondScreen.Bounds.Width;
                    control.Top = secondScreen.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = secondScreen.Bounds.Location;
                    Point p = new Point(secondScreen.Bounds.Location.X, secondScreen.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();

                    //control.FormBorderStyle = FormBorderStyle.None;
                    //control.Left = sc[1].Bounds.Width;
                    //control.Top = sc[1].Bounds.Height;
                    //control.StartPosition = FormStartPosition.Manual;
                    //control.Location = sc[1].Bounds.Location;
                    //Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    //control.Location = p;
                    //control.WindowState = FormWindowState.Maximized;
                    //control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void ShowFormInExtendMonitor(frmWaitingScreen_QY9 control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    Screen secondScreen = sc.FirstOrDefault(o => o != Screen.PrimaryScreen);
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = secondScreen.Bounds.Width;
                    control.Top = secondScreen.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = secondScreen.Bounds.Location;
                    Point p = new Point(secondScreen.Bounds.Location.X, secondScreen.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();


                    //control.FormBorderStyle = FormBorderStyle.None;
                    //control.Left = sc[1].Bounds.Width;
                    //control.Top = sc[1].Bounds.Height;
                    //control.StartPosition = FormStartPosition.Manual;
                    //control.Location = sc[1].Bounds.Location;
                    //Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
                    //control.Location = p;
                    //control.WindowState = FormWindowState.Maximized;
                    //control.Show();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        internal static void TurnOffExtendMonitor(frmWaitingScreen9 control)
        {
            try
            {
                if (control != null)
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreen9)
                            {
                                f.Close();
                            }
                            else if (f.Name == frmWaitingExam9)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void TurnOffExtendMonitor(frmWaitingScreen_QY9 control)
        {
            try
            {
                if (control != null)
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreenQy)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        internal static void LoadDataToExamServiceReqSttGridControl(frmChooseRoomForWaitingScreen control)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
                List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
                string ModuleLinkName = "HIS.Desktop.Plugins.CallPatientVer5";

                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                var HisServiceReqStts = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_STT>>(HisRequestUriStore.HIS_SERVICE_REQ_STT_GET, ApiConsumers.MosConsumer, filter, param);
                //List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> HisServiceReqStts = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                List<ServiceReqSttSDO> serviceReqSttSdos = new List<ServiceReqSttSDO>();
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);


                foreach (var item in HisServiceReqStts)
                {
                    ServiceReqSttSDO serviceReqSttSdo = new ServiceReqSttSDO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>();
                    serviceReqSttSdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>(item);

                    if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                    {
                        foreach (var i in currentControlStateRDO)
                        {
                            if (i.KEY == item.SERVICE_REQ_STT_CODE)
                            {


                                serviceReqSttSdo.checkStt = i.VALUE == "1";
                            }
                        }
                    }
                    serviceReqSttSdos.Add(serviceReqSttSdo);
                }

                control.gridControlExecuteStatus.DataSource = serviceReqSttSdos;

                List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                foreach (var item in serviceReqSttSdos)
                {
                    if (item.checkStt)
                    {
                        MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT serviceReqStt = new MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT();
                        AutoMapper.Mapper.CreateMap<ServiceReqSttSDO, MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                        serviceReqStt = AutoMapper.Mapper.Map<ServiceReqSttSDO, MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>(item);
                        serviceReqStts.Add(serviceReqStt);
                    }
                }
                if (serviceReqStts.Count() > 0)
                {
                    if (control.checkStt == true)
                    {
                        control.tgExtendMonitor.IsOn = false;
                    }
                    else
                    {
                        control.tgExtendMonitor.IsOn = true;
                    }
                }
                else
                {
                    control.tgExtendMonitor.IsOn = false;
                }


            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
