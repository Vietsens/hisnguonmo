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
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;

namespace HIS.Desktop.Plugins.CallPatientV3
{
    public class ChooseRoomForWaitingScreenProcess
    {
        const string frmWaitingScreen5 = "frmWaitingScreen5";
        const string frmWaitingScreenQy = "frmWaitingScreen_QY5";
        internal static void LoadDataToComboRoom(DevExpress.XtraEditors.GridLookUpEdit cboRoom)
        {
            try
            {
                List<long> roomTypeIds = new List<long>();
                roomTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL);
                roomTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG);

                cboRoom.Properties.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => roomTypeIds.Contains(o.ROOM_TYPE_ID) && o.ROOM_TYPE_CODE == "XL").ToList();
                cboRoom.Properties.DisplayMember = "ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";

                cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboRoom.Properties.ImmediatePopup = true;
                cboRoom.ForceInitialize();
                cboRoom.Properties.View.Columns.Clear();

                GridColumn aColumnCode = cboRoom.Properties.View.Columns.AddField("ROOM_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cboRoom.Properties.View.Columns.AddField("ROOM_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ShowFormInExtendMonitor(frmWaitingScreen5 control)
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
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        internal static void TurnOffExtendMonitor(frmWaitingScreen5 control)
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
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqSttFilter filter = new MOS.Filter.HisServiceReqSttFilter();
                var HisServiceReqStts = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_STT>>(HisRequestUriStore.HIS_SERVICE_REQ_STT_GET, ApiConsumers.MosConsumer, filter, param);
                //List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> HisServiceReqStts = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
                List<ServiceReqSttSDO> serviceReqSttSdos = new List<ServiceReqSttSDO>();
                foreach (var item in HisServiceReqStts)
                {
                    ServiceReqSttSDO serviceReqSttSdo = new ServiceReqSttSDO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>();
                    serviceReqSttSdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>(item);
                    if (item.ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        serviceReqSttSdo.checkStt = true;
                    else
                        serviceReqSttSdo.checkStt = false;
                    serviceReqSttSdos.Add(serviceReqSttSdo);
                }
                control.gridControlExecuteStatus.DataSource = serviceReqSttSdos;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
    //public class ChooseRoomForWaitingScreenProcess
    //{
    //    const string frmWaitingScreen5 = "frmWaitingScreen5";
    //    const string frmWaitingScreenQy = "frmWaitingScreen_QY5";
    //    internal static void LoadDataToComboRoom(DevExpress.XtraEditors.GridLookUpEdit cboRoom)
    //    {
    //        try
    //        {
    //            List<long> roomTypeIds = new List<long>();
    //            roomTypeIds.Add(HisRoomTypeCFG.HIS_ROOM_TYPE_ID__DV);
    //            roomTypeIds.Add(HisRoomTypeCFG.HIS_ROOM_TYPE_ID__GI);
    //            cboRoom.Properties.DataSource = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => roomTypeIds.Contains(o.ROOM_TYPE_ID)).ToList();
    //            cboRoom.Properties.DisplayMember = "ROOM_NAME";
    //            cboRoom.Properties.ValueMember = "ID";

    //            cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
    //            cboRoom.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
    //            cboRoom.Properties.ImmediatePopup = true;
    //            cboRoom.ForceInitialize();
    //            cboRoom.Properties.View.Columns.Clear();

    //            GridColumn aColumnCode = cboRoom.Properties.View.Columns.AddField("ROOM_CODE");
    //            aColumnCode.Caption = "Mã";
    //            aColumnCode.Visible = true;
    //            aColumnCode.VisibleIndex = 1;
    //            aColumnCode.Width = 100;

    //            GridColumn aColumnName = cboRoom.Properties.View.Columns.AddField("ROOM_NAME");
    //            aColumnName.Caption = "Tên";
    //            aColumnName.Visible = true;
    //            aColumnName.VisibleIndex = 2;
    //            aColumnName.Width = 300;
    //        }
    //        catch (Exception ex)
    //        {
    //            Inventec.Common.Logging.LogSystem.Warn(ex);
    //        }
    //    }

    //    internal static void LoadRoomCombo(string searchCode, bool isExpand, frmChooseRoomForWaitingScreen control)
    //    {
    //        try
    //        {
    //            if (String.IsNullOrEmpty(searchCode))
    //            {
    //                control.cboRoom.Properties.Buttons[1].Visible = false;
    //                control.cboRoom.EditValue = null;
    //                control.cboRoom.Focus();
    //                control.cboRoom.ShowPopup();
    //                //PopupProcess.SelectFirstRowPopup(control.cboRoom);
    //            }
    //            else
    //            {
    //                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.ROOM_CODE.Contains(searchCode)).ToList();
    //                if (data != null)
    //                {
    //                    if (data.Count == 1)
    //                    {
    //                        control.cboRoom.Properties.Buttons[1].Visible = true;
    //                        control.cboRoom.EditValue = data[0].ID;
    //                        control.txtRoomCode.Text = data[0].ROOM_CODE;
    //                        control.txtRoomCode.Focus();
    //                        control.txtRoomCode.SelectAll();
    //                    }
    //                    else
    //                    {
    //                        var search = data.FirstOrDefault(m => m.ROOM_CODE == searchCode);
    //                        if (search != null)
    //                        {
    //                            control.cboRoom.Properties.Buttons[1].Visible = true;
    //                            control.cboRoom.EditValue = search.ID;
    //                            control.txtRoomCode.Text = search.ROOM_CODE;
    //                            control.txtRoomCode.Focus();
    //                            control.txtRoomCode.SelectAll();
    //                        }
    //                        else
    //                        {
    //                            control.cboRoom.Properties.Buttons[1].Visible = false;
    //                            control.cboRoom.EditValue = null;
    //                            control.cboRoom.Focus();
    //                            control.cboRoom.ShowPopup();
    //                            //PopupProcess.SelectFirstRowPopup(control.cboRoom);
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Inventec.Common.Logging.LogSystem.Warn(ex);
    //        }
    //    }

    //    internal static void ShowFormInExtendMonitor(frmCallPatient control)
    //    {
    //        try
    //        {
    //            Screen[] sc;
    //            sc = Screen.AllScreens;
    //            if (sc.Length <= 1)
    //            {
    //                DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
    //                control.Show();
    //            }
    //            else
    //            {
    //                control.FormBorderStyle = FormBorderStyle.None;
    //                control.Left = sc[1].Bounds.Width;
    //                control.Top = sc[1].Bounds.Height;
    //                control.StartPosition = FormStartPosition.Manual;
    //                control.Location = sc[1].Bounds.Location;
    //                Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
    //                control.Location = p;
    //                control.WindowState = FormWindowState.Maximized;
    //                control.Show();
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            LogSystem.Error(ex);
    //        }
    //    }

    //    //internal static void ShowFormInExtendMonitor(frmWaitingScreen_QY5 control)
    //    //{
    //    //    try
    //    //    {
    //    //        Screen[] sc;
    //    //        sc = Screen.AllScreens;
    //    //        if (sc.Length <= 1)
    //    //        {
    //    //            DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
    //    //            control.Show();
    //    //        }
    //    //        else
    //    //        {
    //    //            control.FormBorderStyle = FormBorderStyle.None;
    //    //            control.Left = sc[1].Bounds.Width;
    //    //            control.Top = sc[1].Bounds.Height;
    //    //            control.StartPosition = FormStartPosition.Manual;
    //    //            control.Location = sc[1].Bounds.Location;
    //    //            Point p = new Point(sc[1].Bounds.Location.X, sc[1].Bounds.Location.Y);
    //    //            control.Location = p;
    //    //            control.WindowState = FormWindowState.Maximized;
    //    //            control.Show();
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        LogSystem.Error(ex);
    //    //    }
    //    //}
    //    internal static void TurnOffExtendMonitor(frmCallPatient control)
    //    {
    //        try
    //        {
    //            if (control != null)
    //            {
    //                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
    //                {
    //                    for (int i = 0; i < Application.OpenForms.Count; i++)
    //                    {
    //                        Form f = Application.OpenForms[i];
    //                        if (f.Name == frmWaitingScreen5)
    //                        {
    //                            f.Close();
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            LogSystem.Error(ex);
    //        }
    //    }

    //    //internal static void TurnOffExtendMonitor(frmWaitingScreen_QY5 control)
    //    //{
    //    //    try
    //    //    {
    //    //        if (control != null)
    //    //        {
    //    //            if (Application.OpenForms != null && Application.OpenForms.Count > 0)
    //    //            {
    //    //                for (int i = 0; i < Application.OpenForms.Count; i++)
    //    //                {
    //    //                    Form f = Application.OpenForms[i];
    //    //                    if (f.Name == frmWaitingScreenQy)
    //    //                    {
    //    //                        f.Close();
    //    //                    }
    //    //                }
    //    //            }
    //    //        }
    //    //    }
    //    //    catch (Exception ex)
    //    //    {
    //    //        LogSystem.Error(ex);
    //    //    }
    //    //}

    //    internal static void LoadDataToExamServiceReqSttGridControl(frmChooseRoomForWaitingScreen control)
    //    {
    //        try
    //        {
    //            List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> HisServiceReqStts = new List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT>();
    //            List<ServiceReqSttSDO> serviceReqSttSdos = new List<ServiceReqSttSDO>();
    //            foreach (var item in HisServiceReqStts)
    //            {
    //                ServiceReqSttSDO serviceReqSttSdo = new ServiceReqSttSDO();
    //                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>();
    //                serviceReqSttSdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT, ServiceReqSttSDO>(item);
    //                if (item.ID == HisServiceReqSttCFG.SERVICE_REQ_STT_ID__NEW)
    //                    serviceReqSttSdo.checkStt = true;
    //                else
    //                    serviceReqSttSdo.checkStt = false;
    //                serviceReqSttSdos.Add(serviceReqSttSdo);
    //            }
    //            control.gridControlExecuteStatus.DataSource = serviceReqSttSdos;
    //        }
    //        catch (Exception ex)
    //        {
    //            LogSystem.Error(ex);
    //        }
    //    }
    //}
}
