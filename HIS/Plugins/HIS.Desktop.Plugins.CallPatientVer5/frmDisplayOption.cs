using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CallPatientVer5.Class;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
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
    public partial class frmDisplayOption : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen9 aFrmWaitingScreen = null;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isInit = true;
        //frmWaitingExam9 aFrmWaitingExam = null;// TODO
        const string frmWaitingScreenStr = "frmWaitingScreen9";
        const string frmWaitingScreenQyStr = "frmWaitingScreen_QY9";
        const string frmWaitingExam9 = "frmWaitingWaitingExam9";
        const string frmWaitingScreenQyNewStr = "frmWaitingScreen_QY_New";
        string ModuleLinkName = "HIS.Desktop.Plugins.CallPatientVer5";

        int positionHandleControl;
        internal long roomId = 0;
        internal bool checkStt = false;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        internal MOS.EFMODEL.DataModels.HIS_SERVICE_REQ HisServiceReq = null;
        internal Inventec.Desktop.Common.Modules.Module _module = null;

        public frmDisplayOption()
        {
            InitializeComponent();
        }

      
        public frmDisplayOption(Inventec.Desktop.Common.Modules.Module module, bool _checkStt)
            : base(module)
        {
            try
            {
                FormCollection fc = Application.OpenForms;
                foreach (Form frm in fc)
                {
                    if (frm.Name == frmWaitingScreenQyStr || frm.Name == frmWaitingScreenStr || frm.Name == frmWaitingScreenQyNewStr)
                    {
                        this.Close();
                        return;
                    }
                }
                InitializeComponent();
                this._module = module;
                this.roomId = module.RoomId;
                this.checkStt = _checkStt;
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmDisplayOption
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.CallPatientVer5.Resources.Lang", typeof(frmDisplayOption).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmDisplayOption.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmDisplayOption.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsShowCol.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.chkIsShowCol.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
               


                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbcRoom.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.lbcRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTitleSTTNext.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.chkTitleSTTNext.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBackground.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.cboBackground.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtColorSTTNext.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.txtColorSTTNext.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.LayoutFont.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.LayoutFont.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmDisplayOption.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDisplayOption_Load(object sender, EventArgs e)
        {

            try
            {
                SetIcon();
                InitControlState();
                //DisplayTbStatus();
                cboSizeTitle.EditValue = null;
                cboColorSTTNext.EditValue = null;
                cboSizeTitleSTT.EditValue = null;
                cboSizeSTT.EditValue = null;
                cboSizeContentSTT.EditValue = null;
                cboSizeList.EditValue = null;

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
                InitControlSate();
                ShowWaitingScreen();
                //CommonParam param = new CommonParam();
                //MOS.Filter.HisRoomTypeFilter roomfilter = new MOS.Filter.HisRoomTypeFilter();

                //Validate_Room();




                //cboRoom.EditValue = GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN;
                //txtRoomCode.Text = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN).ROOM_CODE;
                this.isInit = false;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ShowWaitingScreen()
        {
            try
            {
                if (checkStt)
                {
                    tgExtendMonitor.IsOn = true;
                }
                else
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == frmWaitingScreenStr || f.Name == frmWaitingScreenQyStr || f.Name == frmWaitingExam9 || f.Name == frmWaitingScreenQyNewStr)
                            {
                                //dxValidationProviderControl.RemoveControlError(txtRoomCode);
                                //if (!checkStt)

                                if (GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN != 0 && BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>() != null && BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Count > 0)
                                {
                                    //cboRoom.Enabled = false;
                                    //txtRoomCode.Enabled = false;
                                }
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
        private void LoadDefaultByConfigKey()
        {

            try
            {
                cboSizeTitle.EditValue = WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN;
                if (WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES != null && WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES.Count >= 2)
                {
                    cboColorTittle.Color = Color.FromArgb(WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES[0], WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES[1], WaitingScreenCFG.USER_NAME_FORCE_COLOR_CODES[2]);
                }
                if (WaitingScreenCFG.PARENT_BACK_COLOR_CODES != null && WaitingScreenCFG.PARENT_BACK_COLOR_CODES.Count == 3)
                {
                    cboColorBackround.Color = Color.FromArgb(WaitingScreenCFG.PARENT_BACK_COLOR_CODES[0], WaitingScreenCFG.PARENT_BACK_COLOR_CODES[1], WaitingScreenCFG.PARENT_BACK_COLOR_CODES[2]);
                }
                cboSizeTitleSTT.EditValue = WaitingScreenCFG.FONT_SIZE__LABEL_SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI;
                cboSizeSTT.EditValue = WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI;
                if (WaitingScreenCFG.PARENT_BACK_COLOR_CODES != null && WaitingScreenCFG.PARENT_BACK_COLOR_CODES.Count == 3)
                {
                    cboColorSTT.Color = Color.FromArgb(WaitingScreenCFG.PARENT_BACK_COLOR_CODES[0], WaitingScreenCFG.PARENT_BACK_COLOR_CODES[1], WaitingScreenCFG.PARENT_BACK_COLOR_CODES[2]);
                }
                cboSizeList.EditValue = WaitingScreenCFG.FONT_SIZE__NOI_DUNG_DS_BENH_NHAN;
                if (WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES != null && WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES.Count == 3)
                {
                    cboColorList.Color = Color.FromArgb(WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES[0], WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES[1], WaitingScreenCFG.GRID_PATIENTS_BODY_FORCE_COLOR_CODES[2]);
                }
                cboSizeContentSTT.EditValue = WaitingScreenCFG.FONT_SIZE__SO_THU_TU_BENH_NHAN_DANG_DUOC_GOI;
                chkIsShowCol.Checked = true;
                chkTitleSTTNext.Checked = false;
                txtTitleSTTNext.ReadOnly = true;
                cboColorSTTNext.ReadOnly = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
        //public class ServiceReqSttInfo
        //{
        //    //  public long SERVICE_REQ_STT_ID { get; set; }
        //    public bool IsChecked { get; set; }
        //    public string SERVICE_REQ_STT_CODE { get; set; }
        //    public string SERVICE_REQ_STT_NAME { get; set; }

        //}
        //private void DisplayTbStatus()
        //{
        //    List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().ToList();


        //    List<ServiceReqSttInfo> infoList = serviceReqStts
        //      .Select(x => new ServiceReqSttInfo
        //      {

        //            //SERVICE_REQ_STT_ID = x.ID,
        //            IsChecked = false,
        //          SERVICE_REQ_STT_CODE = x.SERVICE_REQ_STT_CODE,
        //          SERVICE_REQ_STT_NAME = x.SERVICE_REQ_STT_NAME

        //      })
        //      .ToList();


        //    gridControl1.DataSource = infoList;
          
          
        //    //gridView1.PopulateColumns();

        //}
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
                        if (item.KEY == chkIsShowCol.Name)
                        {
                            chkIsShowCol.Checked = item.VALUE == "1";
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
              //  List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_STT> serviceReqStts = BackendDataWorker.Get<HIS_SERVICE_REQ_STT>().ToList();
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
                        if (frm.Name == frmWaitingScreenQyStr || frm.Name == frmWaitingScreenStr || frm.Name == frmWaitingScreenQyNewStr)
                        {
                            frm.Close();
                            tgExtendMonitor.IsOn = true;
                            break;
                        }
                    }
                }
                DisplayOptionADO ado = new DisplayOptionADO();
                if (cboSizeTitle.Value > 0)
                {
                    ado.SizeTitle = cboSizeTitle.Value;
                }
                if (cboColorTittle.EditValue != null)
                    ado.ColorTittle = cboColorTittle.Color;
                if (cboColorBackround.EditValue != null)
                    ado.ColorBackround = cboColorBackround.Color;
                if (!string.IsNullOrEmpty(txtTitle.Text))
                {
                    ado.Title = txtTitle.Text.Trim();
                }
                if (cboSizeTitleSTT.Value > 0)
                {
                    ado.SizeTitleSTT = cboSizeTitleSTT.Value;
                }
                if (cboSizeSTT.Value > 0)
                {
                    ado.SizeSTT = cboSizeSTT.Value;
                }
                if (cboColorSTT.EditValue != null)
                    ado.ColorSTT = cboColorSTT.Color;
                if (cboSizeContentSTT.Value > 0)
                {
                    ado.SizeContentSTT = cboSizeContentSTT.Value;
                }
                if (!string.IsNullOrEmpty(txtTitleSTTNext.Text))
                    ado.TitleSTTNext = txtTitleSTTNext.Text.Trim();
                if (cboColorSTTNext.EditValue != null)
                    ado.ColorSTTNext = cboColorSTTNext.Color;
                if (!string.IsNullOrEmpty(txtContent.Text))
                    ado.Content = txtContent.Text.Trim();
                if (cboSizeList.Value > 0)
                    ado.SizeList = cboSizeList.Value;
                if (cboColorList.EditValue != null)
                    ado.ColorList = cboColorList.Color;
                if (cboColorPriority.EditValue != null)
                    ado.ColorPriority = cboColorPriority.Color;
                ado.IsShowCol = chkIsShowCol.Checked;
                ado.IsNotInDebt = chkIsNotInDebt.Checked;
                string dataSave = Newtonsoft.Json.JsonConvert.SerializeObject(ado);
                SaveConfigToRam(dataSave);
                if (chkTitleSTTNext.Checked)
                {
                    var aFrmNew = new frmWaitingScreen_QY_New(HisServiceReq, serviceReqStts, ado, _module);
                    if (roomId != 0)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_ROOM data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                        if (data != null)
                        {
                            aFrmNew.room = data;
                        }
                    }
                    if (aFrmNew != null && tgExtendMonitor.IsOn)
                    {
                        HIS.Desktop.Plugins.CallPatientVer5.ChooseRoomForWaitingScreenProcess.ShowFormInExtendMonitor(aFrmNew);
                        this.Close();
                    }

                    else
                    {
                        HIS.Desktop.Plugins.CallPatientVer5.ChooseRoomForWaitingScreenProcess.TurnOffExtendMonitor(aFrmNew);
                        //GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = GlobalVariables.CurrentModule.RoomId;
                        GlobalVariables.ROOM_ID_FOR_WAITING_SCREEN = roomId;
                    }
                }
                else
                {
                    var aFrmWaitingScreenQy = new frmWaitingScreen_QY9(HisServiceReq, serviceReqStts, ado, _module);
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

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveConfigToRam(string dataSave)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdatePrintDocumentSigned = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "DisplayConfig" && o.MODULE_LINK == this.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdatePrintDocumentSigned != null)
                {
                    csAddOrUpdatePrintDocumentSigned.VALUE = dataSave;
                }
                else
                {
                    csAddOrUpdatePrintDocumentSigned = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdatePrintDocumentSigned.KEY = "DisplayConfig";
                    csAddOrUpdatePrintDocumentSigned.VALUE = dataSave;
                    csAddOrUpdatePrintDocumentSigned.MODULE_LINK = this.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdatePrintDocumentSigned);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitControlSate()
        {
            try
            {
                DisplayOptionADO ado = null;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "DisplayConfig")
                        {
                            ado = Newtonsoft.Json.JsonConvert.DeserializeObject<DisplayOptionADO>(item.VALUE);
                            cboSizeTitle.EditValue = ado.SizeTitle;
                            cboColorTittle.Color = ado.ColorTittle;
                            cboColorBackround.Color = ado.ColorBackround;
                            txtTitle.Text = ado.Title;
                            cboSizeTitleSTT.EditValue = ado.SizeTitleSTT;
                            cboSizeSTT.EditValue = ado.SizeSTT;
                            cboSizeContentSTT.EditValue = ado.SizeContentSTT;
                            txtTitleSTTNext.Text = ado.TitleSTTNext;
                            cboColorSTT.EditValue = ado.ColorSTT;
                            cboColorSTTNext.EditValue = ado.ColorSTTNext;
                            txtContent.Text = ado.Content;
                            cboSizeList.EditValue = ado.SizeList;
                            cboColorList.Color = ado.ColorList;
                            cboColorPriority.Color = ado.ColorPriority;
                            chkIsShowCol.Checked = ado.IsShowCol;
                            chkIsNotInDebt.Checked = ado.IsNotInDebt;
                            chkTitleSTTNext.Checked = !string.IsNullOrEmpty(txtTitleSTTNext.Text.Trim());
                            if (!chkTitleSTTNext.Checked)
                            {
                                txtTitleSTTNext.ReadOnly = true;
                                cboColorSTTNext.ReadOnly = true;
                            }
                        }
                    }
                }
                if (ado == null)
                {
                    LoadDefaultByConfigKey();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void chkTitleSTTNext_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTitleSTTNext.Checked == true)
                {
                    txtTitleSTTNext.ReadOnly = false;
                    cboColorSTTNext.ReadOnly = false;
                }
                else
                {
                    txtTitleSTTNext.Text = null;
                    cboColorSTTNext.EditValue = null;
                    txtTitleSTTNext.ReadOnly = true;
                    cboColorSTTNext.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

        }

        private void gridControlExecuteStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
