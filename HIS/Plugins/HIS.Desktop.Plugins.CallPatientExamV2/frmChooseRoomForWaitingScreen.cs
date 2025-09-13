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
using HIS.Desktop.Plugins.CallPatientExamV2.Popup;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.CallPatientExamV2
{
    public partial class frmChooseRoomForWaitingScreen : HIS.Desktop.Utility.FormBase
    {
        frmWaitingScreen aFrmWaitingScreenQy = null;
        const string frmWaitingScreenStr = "frmWaitingScreen";
        int positionHandleControl;
        List<RoomGateSDO> RoomGates;
        long roomId = 0;
        string moduleLink = "HIS.Desktop.Plugins.CallPatientExamV2";
        internal ServiceReqGateADO currentAdo;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
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
                var Rooms = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().Where(o=>o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                foreach (var item in Rooms)
                {
                    RoomGateSDO sdo = new RoomGateSDO();
                    sdo.ROOM_CODE = item.EXECUTE_ROOM_CODE;
                    sdo.ROOM_NAME = item.EXECUTE_ROOM_NAME;
                    sdo.ID = item.ID;
                    RoomGates.Add(sdo);
                }
                InitControlState();
                gridControlRoom.DataSource = RoomGates;
                //ValidateControl();
                ToogleExtendMonitor();
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

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currentModuleBase.ModuleLink);
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    this.currentAdo = new ServiceReqGateADO();
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "ServiceReqGateADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                currentAdo = JsonConvert.DeserializeObject<ServiceReqGateADO>(item.VALUE);
                            }
                        }
                    }
                    if (currentAdo != null)
                    {
                        spnTimeReload.EditValue = currentAdo.timeReload;
                        spnSizeTitle.EditValue = currentAdo.sizeTitle;
                        if (!string.IsNullOrEmpty(currentAdo.colorTitle))
                        {
                            List<int> backgroundColorStall = new List<int>();
                            backgroundColorStall = GetColorValues(currentAdo.colorTitle);
                            cboColorTitle.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                        }

                        if (!string.IsNullOrEmpty(currentAdo.bgColorTitle))
                        {
                            List<int> backgroundColorStall = new List<int>();
                            backgroundColorStall = GetColorValues(currentAdo.bgColorTitle);
                            cboBgColorTitle.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                        }

                        spnSizeContentNumber.EditValue = currentAdo.sizeContentNumber;
                        if (!string.IsNullOrEmpty(currentAdo.colorContent))
                        {
                            List<int> backgroundColorStall = new List<int>();
                            backgroundColorStall = GetColorValues(currentAdo.colorContent);
                            cboColorContent.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                        }

                        spnSizeEndTitle.EditValue = currentAdo.sizeEndTitle;
                        if (!string.IsNullOrEmpty(currentAdo.colorEnd))
                        {
                            List<int> backgroundColorStall = new List<int>();
                            backgroundColorStall = GetColorValues(currentAdo.colorEnd);
                            cboColorEnd.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                        }

                        if (!string.IsNullOrEmpty(currentAdo.bgColorEnd))
                        {
                            List<int> backgroundColorStall = new List<int>();
                            backgroundColorStall = GetColorValues(currentAdo.bgColorEnd);
                            cboBgColorEnd.Color = Color.FromArgb(backgroundColorStall[0], backgroundColorStall[1], backgroundColorStall[2]);
                        }

                        chkAutoOpenWaitingScreen.Checked = currentAdo.isAutoOpenWaitingScreen;

                        txtConfigNotify.Text = currentAdo.configNotify;
                        spnChoKham.EditValue = currentAdo.sizeChoKham;
                        spnDangKham.EditValue = currentAdo.sizeDangKham;
                        foreach (var item in RoomGates)
                        {
                            if (currentAdo.roomGateSDOs.Exists(o => o.ID == item.ID))
                                item.checkStt = true;
                        }
                        RoomGates = RoomGates.OrderByDescending(o => o.checkStt). ThenBy(o=>o.POSITION).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultState()
        {

            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                ValidationSingleControl(spnTimeReload);

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


        private void tgExtendMonitor_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (!tgExtendMonitor.IsOn) return;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderControl, dxErrorProvider1);
                ValidateControl();
                if (aFrmWaitingScreenQy != null) aFrmWaitingScreenQy.Close();

                List<RoomGateSDO> hisRegisterGates = new List<RoomGateSDO>();
                if (gridControlRoom.DataSource != null)
                {
                    hisRegisterGates = (List<RoomGateSDO>)gridControlRoom.DataSource;
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
                        DevExpress.XtraEditors.XtraMessageBox.Show("Vui lòng chọn ít nhất 1 phòng khám", "Thông báo");
                        return;
                    }

                    SavePin(hisRegisterGates);
                    frmWaitingScreen frm = new frmWaitingScreen(currentAdo);
                    ShowFormInExtendMonitor(frm);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal static void ShowFormInExtendMonitor(Form control)
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
                    var sc2 = sc.FirstOrDefault(o => !o.Primary);
                    if (sc2 == null) sc2 = sc[1];

                    control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = sc2.Bounds.Width;
                    control.Top = sc2.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = sc2.Bounds.Location;
                    Point p = new Point(sc2.Bounds.Location.X, sc2.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
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

        private void gridViewStatus_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    RoomGateSDO dataRow = (RoomGateSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void SavePin(List<RoomGateSDO> lstRegisterGate)
        {
            try
            {

                ServiceReqGateADO ado = new ServiceReqGateADO();
                ado.roomGateSDOs = lstRegisterGate;
                ado.timeReload = (long)spnTimeReload.Value;
                ado.sizeTitle= (long)spnSizeTitle.Value;
                ado.colorTitle = cboColorTitle.Text;
                ado.bgColorTitle = cboBgColorTitle.Text; 
                ado.sizeContentNumber = (long)spnSizeContentNumber.Value;
                ado.sizeChoKham = (long)spnChoKham.Value;
                ado.sizeDangKham = (long)spnDangKham.Value;
                ado.colorContent = cboColorContent.Text;
                ado.sizeEndTitle = (long)spnSizeEndTitle.Value;
                ado.colorEnd = cboColorEnd.Text;
                ado.bgColorEnd = cboBgColorEnd.Text;
                ado.isAutoOpenWaitingScreen = chkAutoOpenWaitingScreen.Checked;
                ado.configNotify = txtConfigNotify.Text.Trim();
                this.currentAdo = ado;
                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("ado__send__:", ado));
                string textJson = JsonConvert.SerializeObject(ado);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "ServiceReqGateADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = textJson;
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "ServiceReqGateADO";
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
                    gridViewRoom.SetFocusedRowCellValue("DISPLAY_SCREEN", null);

                }
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
