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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.RegisterReqNumOrder.Popup;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterReqNumOrder
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen aFrmWaitingScreenQy = null;
        const string frmWaitingScreenStr = "frmWaitingScreen";
        int positionHandleControl;
        MOS.EFMODEL.DataModels.V_HIS_ROOM room;
        long roomId = 0;
        string moduleLink = "HIS.Desktop.Plugins.RegisterReqNumOrder";
        internal RegisterReqNumOderADO currentAdo;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<ScreenADO> screens;
        string except = " _";

        Inventec.Desktop.Common.Modules.Module _module;

        public frmChooseRoomForWaitingScreen(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            FormCollection fc = Application.OpenForms;
            foreach (Form frm in fc)
            {
                if (frm.Name == frmWaitingScreenStr)
                {
                    this.Close();
                    return;
                }
            }
            InitializeComponent();
            this.roomId = module.RoomId;
            this._module = module;
        }

        private void frmChooseRoomForWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                GetAllScreen();
                InitComboDisplay(screens);

                InitControlState();
                ChooseRoomForWaitingScreenProcess.LoadDataToSttGridControl(this);
                room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                //ValidateControl();
                ToogleExtendMonitor();
                SetDataTolblControl();
                if (currentAdo != null && currentAdo.isAutoOpenWaitingScreen)
                {
                    chkAutoOpenWaitingScreen.Checked = true;
                    tgExtendMonitor.IsOn = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShowChooseRoomForWaitingScreen()
        {
            try
            {
                this.Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboDisplay(List<ScreenADO> screens)
        {
            try
            {
                if (screens.Count > 0)
                {
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("Device_Name", "Màn hình", 150, 1));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("Device_Name", "Device_Name", columnInfos, true, 200);
                    controlEditorADO.ImmediatePopup = true;
                    ControlEditorLoader.Load(repositoryItemGridLookUpEdit1, screens, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetAllScreen()
        {
            try
            {
                this.screens = new List<ScreenADO>();
                foreach (var item in Screen.AllScreens.OrderBy(o => o.Bounds.Location.X))
                {
                    ScreenADO screen = new ScreenADO();
                    screen.Device_Name = Regex.Replace(item.DeviceName, @"[^a-zA-Z0-9" + except + "]+", string.Empty);
                    screen.Bounds = item.Bounds;
                    screens.Add(screen);
                }
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
                this.currentControlStateRDO = controlStateWorker.GetData(currentModuleBase.ModuleLink);
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    this.currentAdo = new RegisterReqNumOderADO();
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "RegisterReqNumOderADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                currentAdo = JsonConvert.DeserializeObject<RegisterReqNumOderADO>(item.VALUE);
                            }
                        }
                    }
                    if (currentAdo != null)
                    {
                        currentAdo.registerGate = currentAdo.registerGate;
                        spnMaxLine.EditValue = currentAdo.maxLine;
                        spnReloadTime.EditValue = currentAdo.reloadTime;
                        spnFlickerTime.EditValue = currentAdo.flickerTime;
                        txtNote.Text = currentAdo.note;
                        spSizeCellColumn.EditValue = currentAdo.CellSize;
                        spSizeHeaderColumn.EditValue = currentAdo.HeaderSize;
                        spnFooterSize.EditValue = currentAdo.footerSize;

                        if (!string.IsNullOrEmpty(currentAdo.urlVoice)) txtSound.Text = currentAdo.urlVoice;
                        txtConfigNotify.Text = currentAdo.configNotify;

                        if (!string.IsNullOrEmpty(currentAdo.backgroundColorTitle))
                        {
                            List<int> colorTittle = new List<int>();
                            colorTittle = GetColorValues(currentAdo.backgroundColorTitle);
                            cboColorTittle.Color = Color.FromArgb(colorTittle[0], colorTittle[1], colorTittle[2]);
                        }
                        if (!string.IsNullOrEmpty(currentAdo.backgroundColorSTT))
                        {
                            List<int> backgroundColorSTT = new List<int>();
                            backgroundColorSTT = GetColorValues(currentAdo.backgroundColorSTT);
                            cboBackgroundColorSTT.Color = Color.FromArgb(backgroundColorSTT[0], backgroundColorSTT[1], backgroundColorSTT[2]);
                        }
                        if (!string.IsNullOrEmpty(currentAdo.backgroundColorStall))
                        {
                            List<int> backgroundColorStall = new List<int>();
                            backgroundColorStall = GetColorValues(currentAdo.backgroundColorStall);
                            cboBackgroundColorStall.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static List<int> GetColorValues(string code)
        {
            List<int> result = new List<int>();
            try
            {
                //string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
                //string pattern = ",";
                //Regex myRegex = new Regex(pattern);
                //string[] Codes = myRegex.Split(value);

                string[] Codes = code.Split(',');

                //if (String.IsNullOrEmpty(value)) throw new ArgumentNullException(code);

                if (!(Codes != null) || Codes.Length <= 0) throw new ArgumentNullException(code + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                foreach (var item in Codes) ///
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt32(item));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
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
        void ToogleExtendMonitor()
        {
            try
            {
                if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                {
                    for (int i = 0; i < Application.OpenForms.Count; i++)
                    {
                        Form f = Application.OpenForms[i];
                        if (f.Name == frmWaitingScreenStr)
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

        private void ValidateControl()
        {
            try
            {
                ValidationSingleControl(spnMaxLine);
                ValidationSingleControl(spnReloadTime);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(SpinEdit control)
        {
            try
            {
                RoomWaitingScreenValidation roomRule = new RoomWaitingScreenValidation();
                roomRule.spn = control;
                roomRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                roomRule.ErrorType = ErrorType.Warning;
                dxValidationProviderControl.SetValidationRule(control, roomRule);
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

        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (!tgExtendMonitor.IsOn) return;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderControl, dxErrorProvider1);
                ValidateControl();
                if (aFrmWaitingScreenQy != null) aFrmWaitingScreenQy.Close();

                List<HisRegisterGateSDO> hisRegisterGates = new List<HisRegisterGateSDO>();
                if (gridControlStatus.DataSource != null)
                {
                    hisRegisterGates = (List<HisRegisterGateSDO>)gridControlStatus.DataSource;
                }
                if (hisRegisterGates.Count() > 0)
                {
                    var displayScreen = hisRegisterGates.Where(o => o.checkStt);
                    this.positionHandleControl = -1;
                    if (!dxValidationProviderControl.Validate())
                    {
                        tgExtendMonitor.IsOn = false;
                        return;
                    }
                    if (displayScreen == null || displayScreen.Count() == 0)
                    {
                        if (tgExtendMonitor.IsOn)
                        {
                            tgExtendMonitor.IsOn = false;
                            return;
                        }
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn ít nhất 1 dãy tiếp đón", "Thông báo");
                        return;
                    }

                    SavePin(hisRegisterGates);
                    ScreenADO sc = ThisScreen();
                    if (!string.IsNullOrEmpty(currentAdo.Display_Screen))
                    {
                        var displayScreenGroup = displayScreen.GroupBy(o => o.DISPLAY_SCREEN);
                        bool isHasNullScreen = displayScreenGroup.Any(o => string.IsNullOrEmpty(o.FirstOrDefault().DISPLAY_SCREEN));

                        foreach (var item in displayScreenGroup)
                        {
                            var screen = screens.FirstOrDefault(o => o.Device_Name == item.FirstOrDefault().DISPLAY_SCREEN);

                            if (isHasNullScreen)
                            {
                                if (string.IsNullOrEmpty(item.First().DISPLAY_SCREEN))
                                {
                                    var dataScreen = displayScreen.Where(o => (o.DISPLAY_SCREEN == sc.Device_Name) || string.IsNullOrEmpty(o.DISPLAY_SCREEN)).ToList();
                                    aFrmWaitingScreenQy = new frmWaitingScreen(dataScreen.ToList(), currentAdo, this._module);
                                    if (this.room != null) aFrmWaitingScreenQy.room = this.room;
                                    ShowFormInExtendMonitor(aFrmWaitingScreenQy, sc);
                                }
                                else if (!string.IsNullOrEmpty(screen.Device_Name) && screen.Device_Name != sc.Device_Name)
                                {
                                    var dataScreen = displayScreen.Where(o => o.DISPLAY_SCREEN == item.FirstOrDefault().DISPLAY_SCREEN).ToList();
                                    aFrmWaitingScreenQy = new frmWaitingScreen(dataScreen.ToList(), currentAdo, this._module);
                                    if (this.room != null) aFrmWaitingScreenQy.room = this.room;
                                    ShowFormInExtendMonitor(aFrmWaitingScreenQy, screen);
                                }
                            }
                            else
                            {
                                var dataScreen = displayScreen.Where(o => o.DISPLAY_SCREEN == item.FirstOrDefault().DISPLAY_SCREEN).ToList();
                                aFrmWaitingScreenQy = new frmWaitingScreen(dataScreen.ToList(), currentAdo, this._module);
                                if (this.room != null) aFrmWaitingScreenQy.room = this.room;
                                ShowFormInExtendMonitor(aFrmWaitingScreenQy, screen);
                            }
                        }
                    }
                    else
                    {
                        aFrmWaitingScreenQy = new frmWaitingScreen(displayScreen.ToList(), currentAdo, this._module);
                        if (this.room != null)
                        {
                            aFrmWaitingScreenQy.room = this.room;
                        }
                        ShowFormInExtendMonitor(aFrmWaitingScreenQy, sc);
                    }
                    tgExtendMonitor.IsOn = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private ScreenADO ThisScreen()
        {
            try
            {
                for (int i = 0; i < this.screens.Count(); i++)
                {
                    if (i < this.screens.Count() - 1)
                    {
                        if (screens[i].Bounds.Location.X < this.Location.X && this.Location.X < screens[i + 1].Bounds.Location.X)
                            return screens[i];
                    }
                    else
                    {
                        return screens[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void ShowFormInExtendMonitor(frmWaitingScreen control, ScreenADO screen)
        {
            try
            {
                if (screen != null)
                {
                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = screen.Bounds.Width;
                    control.Top = screen.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = screen.Bounds.Location;
                    Point p = new Point(screen.Bounds.Location.X, screen.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                    control.Activate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void gridViewStatus_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HisRegisterGateSDO dataRow = (HisRegisterGateSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (dataRow != null)
                    {
                        //if (e.Column.FieldName == "CheckStt")
                        //{
                        //    try
                        //    {
                        //        e.Value = dataRow.checkStt;
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        //    }
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SavePin(List<HisRegisterGateSDO> lstRegisterGate)
        {
            try
            {

                RegisterReqNumOderADO ado = new RegisterReqNumOderADO();
                ado.registerGate = string.Join(",", lstRegisterGate.Where(p => p.checkStt).Select(o => o.ID).ToList());
                ado.maxLine = (decimal)spnMaxLine.EditValue;
                ado.reloadTime = (decimal)spnReloadTime.EditValue;
                ado.flickerTime = (decimal?)spnFlickerTime.EditValue;
                ado.note = txtNote.Text;
                ado.CellSize = spSizeCellColumn.Value;
                ado.HeaderSize = spSizeHeaderColumn.Value;
                ado.isAutoOpenWaitingScreen = chkAutoOpenWaitingScreen.Checked;
                ado.Display_Screen = string.Join(",", lstRegisterGate.Where(p => !string.IsNullOrEmpty(p.DISPLAY_SCREEN)).Select(o => string.Format("{0}/{1}", o.ID, o.DISPLAY_SCREEN)).ToList());
                ado.footerSize = (decimal?)spnFooterSize.EditValue;
                ado.backgroundColorTitle = cboColorTittle.Text;
                ado.backgroundColorSTT = cboBackgroundColorSTT.Text;
                ado.backgroundColorStall = cboBackgroundColorStall.Text;
                ado.urlVoice = txtSound.Text.Trim();
                ado.configNotify = txtConfigNotify.Text.Trim();

                this.currentAdo = ado;
                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ado__send__:", ado));
                string textJson = JsonConvert.SerializeObject(ado);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "RegisterReqNumOderADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = textJson;
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "RegisterReqNumOderADO";
                    csAddOrUpdateValue.VALUE = textJson;
                    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewStatus_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                ScreenADO data = view.GetFocusedRow() as ScreenADO;
                if (view.FocusedColumn.FieldName == "DISPLAY_SCREEN" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    if (data != null)
                    {
                        editor.EditValue = data.Device_Name;
                    }
                    gridViewStatus.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemGridLookUpEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    gridViewStatus.SetFocusedRowCellValue("DISPLAY_SCREEN", null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSound_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(openFileDialog.FileName)) txtSound.Text = openFileDialog.FileName;
                }
                tgExtendMonitor.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtConfigNotify_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {
                    frmConfigNotify frm = new frmConfigNotify();
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
