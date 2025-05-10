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
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CallPatientVer5
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen9 aFrmWaitingScreen = null;
        frmWaitingScreen_QY9 aFrmWaitingScreenQy = null;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isInit = true;
        //frmWaitingExam9 aFrmWaitingExam = null;// TODO
        const string frmWaitingScreenStr = "frmWaitingScreen9";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_QY9";
        const string frmWaitingExam9 = "frmWaitingWaitingExam9";

        string ModuleLinkName = "HIS.Desktop.Plugins.CallPatientVer5";

        int positionHandleControl;
        internal long roomId = 0;
        internal bool checkStt;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq = null;
        internal Inventec.Desktop.Common.Modules.Module _module = null;
        public frmChooseRoomForWaitingScreen()
        {
            InitializeComponent();
        }
        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module, bool _checkStt)
            : base(module)
        {
            this._module = module;
            this.checkStt = _checkStt;
            InitializeComponent();
        }
        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
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
                this._module = module;
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientVer5.Resources.Lang", typeof(HIS.Desktop.Plugins.CallPatientVer5.frmChooseRoomForWaitingScreen).Assembly);

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
                InitControlState();
                //ChooseRoomForWaitingScreenProcess.LoadDataToComboRoom(roomId);
            //    ChooseRoomForWaitingScreenProcess.LoadDataToExamServiceReqSttGridControl(this); 
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
                        if (f.Name == frmWaitingScreenStr || f.Name == frmWaitingScreenQyStr || f.Name == frmWaitingExam9)
                        {
                            //dxValidationProviderControl.RemoveControlError(txtRoomCode);
                            if (!checkStt)
                            {
                                tgExtendMonitor.IsOn = true;
                            }
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
                this.isInit = false;


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

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(ModuleLinkName);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkIsNotInDebt.Name)
                        {
                            chkIsNotInDebt.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void tgExtendMonitor_Toggled(object sender, EventArgs e)
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

                if (checkStt)
                {
                    FormCollection fc = Application.OpenForms;
                    foreach (Form frm in fc)
                    {
                        if (frm.Name == frmWaitingScreenQyStr || frm.Name == frmWaitingScreenStr)
                        {
                            frm.Close();
                            tgExtendMonitor.IsOn = true;
                            break;
                        }
                    }
                }

                aFrmWaitingScreenQy = new frmWaitingScreen_QY9(HisServiceReq, serviceReqStts, new Class.DisplayOptionADO(), _module);
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
                    HIS.Desktop.Plugins.CallPatientVer5.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmWaitingScreenQy);
                    this.Close();
                }

                else
                {
                    HIS.Desktop.Plugins.CallPatientVer5.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmWaitingScreenQy);
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

        private void chkIsNotInDebt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInit)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkIsNotInDebt.Name && o.MODULE_LINK == this.ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsNotInDebt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIsNotInDebt.Name;
                    csAddOrUpdate.VALUE = (chkIsNotInDebt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckEditStt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var currentServiceReqStt = (ServiceReqSttSDO)gridViewExecuteStatus.GetFocusedRow();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == currentServiceReqStt.SERVICE_REQ_STT_CODE && o.MODULE_LINK == this.ModuleLinkName).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (currentServiceReqStt.checkStt ? "" : "1");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = currentServiceReqStt.SERVICE_REQ_STT_CODE;
                    csAddOrUpdate.VALUE = currentServiceReqStt.checkStt ? "" : "1";
                    csAddOrUpdate.MODULE_LINK = this.ModuleLinkName;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
