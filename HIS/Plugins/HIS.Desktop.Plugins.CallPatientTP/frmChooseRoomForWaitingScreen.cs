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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientTP
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen_TP4 aFrmWaitingScreenQy = null;
        const string frmWaitingScreenStr = "frmWaitingScreen4";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_TP4";
        int positionHandleControl;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq = null;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        long roomId = 0;
        Inventec.Desktop.Common.Modules.Module currentModule;


        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
        : base(module)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == frmWaitingScreenQyStr || frm.Name == frmWaitingScreenStr)
                {
                    this.Close();
                    return;
                }
            }
            InitializeComponent();
            this.roomId = module.RoomId;
            this.currentModule = module;
        }

        private void frmChooseRoomForWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                ChooseRoomForWaitingScreenProcess.LoadDataToExamServiceReqSttGridControl(this);
                room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                ValidateRoom();
                ToogleExtendMonitor();
                SetDataTolblControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ToogleExtendMonitor()
        {
            try
            {
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr || f.Name == frmWaitingScreenQyStr)
                        {
                            tgExtendMonitor.IsOn = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDataTolblControl()
        {

            try
            {
                if (room != null)
                {
                    lblRoom.Text = (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lblRoom.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateRoom()
        {
            try
            {
                roomWaitingScreenValidation roomRule = new roomWaitingScreenValidation();
                roomRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                roomRule.ErrorType = ErrorType.Warning;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                List<ServiceReqSttSDO> serviceReqSttSdos = new List<ServiceReqSttSDO>();
                if (gridControlExecuteStatus.DataSource != null)
                {
                    serviceReqSttSdos = (List<ServiceReqSttSDO>)gridControlExecuteStatus.DataSource;
                }
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
                this.positionHandleControl = -1;
                if (!dxValidationProviderControl.Validate())
                    return;
                aFrmWaitingScreenQy = new frmWaitingScreen_TP4(this.currentModule, HisServiceReq, serviceReqStts);
                if (this.room != null)
                {
                    aFrmWaitingScreenQy.room = this.room;
                }
                if (aFrmWaitingScreenQy != null && tgExtendMonitor.IsOn)
                {
                    HIS.Desktop.Plugins.CallPatientTP.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy);
                    this.Close();
                }
                else
                {
                    HIS.Desktop.Plugins.CallPatientTP.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExecuteStatus_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ServiceReqSttSDO dataRow = (ServiceReqSttSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderControl_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
