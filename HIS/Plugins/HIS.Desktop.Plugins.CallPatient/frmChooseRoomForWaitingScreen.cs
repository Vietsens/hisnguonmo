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
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatient
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen1 aFrmWaitingScreen = null;
        frmWaitingScreen_QY1 aFrmWaitingScreenQy = null;
        //frmWaitingExam aFrmWaitingExam = null;// TODO
        const string frmWaitingScreenStr = "frmWaitingScreen1";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_QY1";
        const string frmWaitingExam = "frmWaitingWaitingExam1";
        Inventec.Desktop.Common.Modules.Module currentModule;


        int positionHandleControl;
        long roomId = 0;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq = null;
        public frmChooseRoomForWaitingScreen()
        {
            InitializeComponent();
        }

        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
		:base(module)
        {
            try
            {
                FormCollection fc = Application.OpenForms;
                foreach (Form frm in fc)
                {
                    if (frm.Name == frmWaitingScreenQyStr || frm.Name == frmWaitingScreenStr || frm.Name == frmWaitingExam)
                    {
                        this.Close();
                        return;
                    }
                }
                InitializeComponent();
                this.currentModule = module;
                this.roomId = module.RoomId;
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatient.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatient.frmChooseRoomForWaitingScreen).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbcRoom.Text = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.lbcRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChooseRoomForWaitingScreen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChooseRoomForWaitingScreen_Load(object sender, EventArgs e)
        {

            try
            {
                SetIcon();
                //ChooseRoomForWaitingScreenProcess.LoadDataToComboRoom(roomId);
                ChooseRoomForWaitingScreenProcess.LoadDataToExamServiceReqSttGridControl(this);
                room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                lbcRoom.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName().ToUpper();
                if (room != null)
                {
                    lbcRoom.Text = (room.ROOM_NAME + " (" + room.DEPARTMENT_NAME + ")").ToUpper();
                }
                else
                {
                    lbcRoom.Text = "";
                }
                //CommonParam param = new CommonParam();
                //MOS.Filter.HisRoomTypeFilter roomfilter = new MOS.Filter.HisRoomTypeFilter();

                //Validate_Room();

                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr || f.Name == frmWaitingScreenQyStr || f.Name == frmWaitingExam)
                        {
                            //dxValidationProviderControl.RemoveControlError(txtRoomCode);
                            tgExtendMonitor.IsOn = true;
                            if (GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN != 0 && BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>() != null && BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Count > 0)
                            {
                                //cboRoom.Enabled = false;
                                //txtRoomCode.Enabled = false;
                            }
                        }
                    }
                }
                //cboRoom.EditValue = GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN;
                //txtRoomCode.Text = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN).ROOM_CODE;
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
                //cboRoom.Enabled = true;
                //txtRoomCode.Enabled = true;
                this.positionHandleControl = -1;
                if (!dxValidationProviderControl.Validate())
                    return;

                //if (GlobalVariables.ChonManHinhGoiBenhNhan == 1)
                //{
                //    aFrmWaitingScreen = new frmWaitingScreen1(HisServiceReq, serviceReqStts);
                //    if (roomId != 0 )
                //    {
                //        MOS.EFMODEL.DataModels.V_HIS_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                //        if (data != null)
                //        {
                //            aFrmWaitingScreen.room = data;
                //        }
                //    }
                //    if (aFrmWaitingScreen != null && tgExtendMonitor.IsOn)
                //    {
                //        HIS.Desktop.Plugins.CallPatient.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(((frmWaitingScreen1)aFrmWaitingScreen));
                //        this.Close();
                //    }
                //    else
                //    {
                //        HIS.Desktop.Plugins.CallPatient.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(((frmWaitingScreen1)aFrmWaitingScreen));
                //        GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = roomId;
                //        //GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = GlobalVariables.CurrentModule.RoomId;
                //    }
                //}
                //else
                //{
                //    aFrmWaitingScreenQy = new frmWaitingScreen_QY1(HisServiceReq, serviceReqStts);
                //    if (roomId != 0)
                //    {
                //        MOS.EFMODEL.DataModels.V_HIS_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                //        if (data != null)
                //        {
                //            aFrmWaitingScreenQy.room = data;
                //        }
                //    }
                //    if (aFrmWaitingScreenQy != null && tgExtendMonitor.IsOn)
                //    {
                //        HIS.Desktop.Plugins.CallPatient.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy);
                //        this.Close();
                //    }
                //    else
                //    {
                //        HIS.Desktop.Plugins.CallPatient.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy);
                //        //GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = GlobalVariables.CurrentModule.RoomId;
                //        GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = roomId;
                //    }
                //}
                aFrmWaitingScreenQy = new frmWaitingScreen_QY1(currentModule, HisServiceReq, serviceReqStts);
                if (roomId != 0)
                {
                    MOS.EFMODEL.DataModels.V_HIS_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                    if (data != null)
                    {
                        aFrmWaitingScreenQy.room = data;
                    }
                }
                if (aFrmWaitingScreenQy != null && tgExtendMonitor.IsOn)
                {
                    HIS.Desktop.Plugins.CallPatient.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy);
                    this.Close();
                }
                else
                {
                    HIS.Desktop.Plugins.CallPatient.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy);
                    //GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = GlobalVariables.CurrentModule.RoomId;
                    GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = roomId;
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
