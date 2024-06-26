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
using MOS.Filter;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.RegisterReqNumOrder
{
    internal class ChooseRoomForWaitingScreenProcess
    {
        const string frmWaitingScreen = "frmWaitingScreen";
        const string frmWaitingScreenQy = "frmWaitingScreen_QY";
        const string except = " _";

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

        internal static void TurnOffExtendMonitor(frmWaitingScreen control)
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

        internal static void LoadDataToSttGridControl(frmChooseRoomForWaitingScreen control)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisRegisterGateFilter filter = new HisRegisterGateFilter();
                filter.IS_ACTIVE = 1;
                var hisRegisterGates = new BackendAdapter(param).Get<List<HIS_REGISTER_GATE>>("api/HisRegisterGate/Get", ApiConsumers.MosConsumer, filter, param);
                List<HisRegisterGateSDO> registerGateSdos = new List<HisRegisterGateSDO>();

                foreach (var item in hisRegisterGates)
                {
                    HisRegisterGateSDO registerGateSdo = new HisRegisterGateSDO();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE, HisRegisterGateSDO>();
                    registerGateSdo = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.HIS_REGISTER_GATE, HisRegisterGateSDO>(item);

                    if (control.currentAdo != null && !string.IsNullOrEmpty(control.currentAdo.registerGate))
                    {
                        var lstRegisterGate = control.currentAdo.registerGate.Split(',').ToList();
                        registerGateSdo.checkStt = lstRegisterGate.Contains(registerGateSdo.ID.ToString());

                    }
                    if (control.currentAdo != null && !string.IsNullOrEmpty(control.currentAdo.Display_Screen))
                    {
                        var lstRegisterGateDisplayScreen = control.currentAdo.Display_Screen.Split(',').ToList();
                        var display = lstRegisterGateDisplayScreen.Where(o => o.Split('/').ToList()[0].Equals(registerGateSdo.ID.ToString()) && !string.IsNullOrEmpty(o.Split('/').ToList()[1])).ToList();
                        if (display != null && display.Count() > 0)
                        {
                            registerGateSdo.DISPLAY_SCREEN = display.FirstOrDefault().Split('/').ToList()[1];
                        }
                    }
                    registerGateSdos.Add(registerGateSdo);
                }
                control.gridControlStatus.DataSource = registerGateSdos;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
