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
using HIS.Desktop.Plugins.CallPatientV4.Config;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.CallPatientV4
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen_V4 aFrmWaitingScreenQy = null;
        frmWaitingScreen_V4_SeparateScreen aFrmWaitingScreenQy_SeparateScreen = null;
        const string frmWaitingScreenStr = "frmWaitingScreen_V4";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_TP6";
        int positionHandleControl;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq = null;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        long roomId = 0;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        Inventec.Desktop.Common.Modules.Module currentModule;


        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
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

                // SetControlState
                InitControlState();

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

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru)
                        {
                            chkSeparatePatientNoiTruNgoaiTru.Checked = item.VALUE == "1";
                        }
                    }
                }
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
                RoomWaitingScreenValidation roomRule = new RoomWaitingScreenValidation();
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
                if (chkSeparatePatientNoiTruNgoaiTru.Checked)
                {
                    aFrmWaitingScreenQy_SeparateScreen = new frmWaitingScreen_V4_SeparateScreen(this.currentModule,HisServiceReq, serviceReqStts);
                    if (this.room != null)
                    {
                        aFrmWaitingScreenQy_SeparateScreen.room = this.room;
                    }
                    if (aFrmWaitingScreenQy_SeparateScreen != null && tgExtendMonitor.IsOn)
                    {
                        HIS.Desktop.Plugins.CallPatientV4.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy_SeparateScreen);
                        this.Close();
                    }
                    else
                    {
                        HIS.Desktop.Plugins.CallPatientV4.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy_SeparateScreen);
                    }
                }
                else
                {
                    aFrmWaitingScreenQy = new frmWaitingScreen_V4(this.currentModule,HisServiceReq, serviceReqStts);
                    if (this.room != null)
                    {
                        aFrmWaitingScreenQy.room = this.room;
                    }
                    if (aFrmWaitingScreenQy != null && tgExtendMonitor.IsOn)
                    {
                        HIS.Desktop.Plugins.CallPatientV4.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy);
                        this.Close();
                    }
                    else
                    {
                        HIS.Desktop.Plugins.CallPatientV4.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy);
                    }
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

        private void chkSeparatePatientNoiTruNgoaiTru_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru && o.MODULE_LINK == ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkSeparatePatientNoiTruNgoaiTru.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkSeparatePatientNoiTruNgoaiTru;
                    csAddOrUpdate.VALUE = (chkSeparatePatientNoiTruNgoaiTru.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
