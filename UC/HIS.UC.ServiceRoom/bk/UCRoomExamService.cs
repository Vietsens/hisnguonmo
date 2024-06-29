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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ServiceRoom.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using MOS.SDO;
using System.Resources;
using System.Globalization;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using HIS.UC.ServiceRoom.CustomControl;

namespace HIS.UC.ServiceRoom
{
    public partial class UCRoomExamService : UserControl
    {
        #region Daclare

        public V_HIS_PATIENT_TYPE_ALTER currentPatientTypeAlter { get; set; }
        public List<HIS_PATIENT_TYPE> currentPatientTypes { get; set; }
        public List<V_HIS_SERVICE_ROOM> hisServiceRooms { get; set; }
        RoomExamServiceInitADO roomExamServiceInitADO;
        public List<RoomExtADO> roomExts { get; set; }
        public string layoutRoomName { get; set; }
        public string layoutExamServiceName { get; set; }
        public bool isInit { get; set; }
        public bool isFocusCombo { get; set; }
        public string userControlItemName { get; set; }
        public V_HIS_SERE_SERV sereServExam { get; set; }

        public RemoveRoomExamService dlgRemoveUC { get; set; }
        public DelegateFocusNextUserControl dlgFocusNextUserControl;
        Action registerPatientWithRightRouteBHYT;
        Action changeRoomNotEmergency;
        Action<long> changeServiceProcessPrimaryPatientType;

        Dictionary<long, HIS_PATIENT_TYPE> dicPatientType = new Dictionary<long, HIS_PATIENT_TYPE>();
        Dictionary<long, List<V_HIS_SERVICE_ROOM>> dicRoomService = new Dictionary<long, List<V_HIS_SERVICE_ROOM>>();
        Dictionary<long, RoomExtADO> dicExecuteRoom = new Dictionary<long, RoomExtADO>();
        CultureInfo currentCulture;
        List<V_HIS_SERVICE_ROOM> currentServiceRooms = new List<V_HIS_SERVICE_ROOM>();

        List<RoomExtADO> currentRoomExts = new List<RoomExtADO>();
        string ucName;
        long? serviceId = null;

        #endregion

        #region Constructor - Load

        public UCRoomExamService()
        {
            InitializeComponent();
        }

        public UCRoomExamService(RoomExamServiceInitADO ado)
        {
            InitializeComponent();
            try
            {
                SetDataInit(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRoomExamService_Load(object sender, EventArgs e)
        {
            try
            {
                InitRoomComboByConfig();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ServiceRoom.Resources.Lang", typeof(HIS.UC.ServiceRoom.UCRoomExamService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.cboExamService.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomExamService.cboExamService.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.cboRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomExamService.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciCboRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciCboRoom.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciExamService.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciExamService.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciRoom.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Event Control

        private void txtRoomCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(this.txtRoomCode.Text))
                    {
                        this.currentRoomExts = new List<RoomExtADO>();
                        var arrSearchCodes = this.txtRoomCode.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrSearchCodes != null && arrSearchCodes.Count() > 0)
                        {
                            foreach (var itemCode in arrSearchCodes)
                            {
                                string key = itemCode.ToLower();
                                var listData = this.roomExts.Where(o => o.EXECUTE_ROOM_CODE.ToLower().Contains(key)).ToList();
                                var searchResult = (listData != null && listData.Count > 0) ? (listData.Count == 1 ? listData : listData.Where(o => o.EXECUTE_ROOM_CODE.ToUpper() == txtRoomCode.Text.ToUpper()).ToList()) : null;
                                if (searchResult != null && searchResult.Count == 1)
                                {
                                    this.currentRoomExts.Add(searchResult.First());
                                }
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("this.txtRoomCode.Text", this.txtRoomCode.Text) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentRoomExts), currentRoomExts));
                        if (this.currentRoomExts != null && this.currentRoomExts.Count > 0)
                        {
                            RoomEditValueChanged();

                            this.txtExamServiceCode.Focus();
                            this.txtExamServiceCode.SelectAll();
                        }
                        else
                        {
                            this.cboRoom.Focus();
                            this.cboRoom.ShowPopup();
                        }
                    }
                    else
                    {
                        this.cboRoom.Focus();
                        this.cboRoom.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboRoom.CustomSetAuto(false);
                Inventec.Common.Logging.LogSystem.Debug("cboRoom_Closed.e.CloseMode=" + e.CloseMode.ToString());
                if ((e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate) && this.currentRoomExts != null && this.currentRoomExts.Count > 0)
                {
                    this.FocusTotxtExamServiceCode();
                }
                else if ((e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal || e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Immediate) && (this.currentRoomExts == null || this.currentRoomExts.Count == 0))
                {
                    try
                    {
                        if (this.dlgFocusNextUserControl != null)
                        {
                            this.dlgFocusNextUserControl();
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //this.cboExamService.EditValue = null;
                //this.txtExamServiceCode.Text = "";
                //this.txtRoomCode.Text = "";
                //this.currentServiceRooms = new List<V_HIS_SERVICE_ROOM>();
                //this.cboRoom.Properties.Buttons[1].Visible = false;
                //if (this.cboRoom.EditValue != null)
                //{
                //    this.cboRoom.Properties.Buttons[1].Visible = true;
                //    var room = this.roomExts.FirstOrDefault(o => o.ROOM_ID == Convert.ToInt64(this.cboRoom.EditValue));
                //    if (room != null)
                //    {
                //        RoomEditValueChanged(room);
                //    }
                //    if (this.dicRoomService.ContainsKey(Convert.ToInt64(this.cboRoom.EditValue)))
                //        this.currentServiceRooms = dicRoomService[Convert.ToInt64(this.cboRoom.EditValue)];
                //}
                //this.SetDataSourceCboExamService();
                //if (this.serviceId.HasValue)
                //{
                //    this.cboExamService.EditValue = this.serviceId.Value;
                //    this.serviceId = null;
                //}
                //else if (this.currentServiceRooms != null && this.currentServiceRooms.Count == 1)
                //{
                //    this.cboExamService.EditValue = this.currentServiceRooms.FirstOrDefault().SERVICE_ID;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboRoom.Properties.Buttons[1].Visible = false;
                    this.cboRoom.EditValue = null;
                    this.txtRoomCode.Text = "";
                    GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboRoom.Properties.View);
                    }
                    this.currentRoomExts = new List<RoomExtADO>();
                    FocusTotxtExamServiceCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //long roomId = 0;
              
                //if (e.KeyCode == Keys.Enter && gridLookUpEdit1View.FocusedRowHandle >= 0 && (this.currentRoomExts == null || this.currentRoomExts.Count == 0))
                //{
                //    roomId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpEdit1View.GetRowCellValue(gridLookUpEdit1View.FocusedRowHandle, "ROOM_ID").ToString());

                //    //    this.FocusTotxtExamServiceCode();
                //}
                //Inventec.Common.Logging.LogSystem.Debug("cboRoom_KeyDown" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "gridLookUpEdit1View.FocusedRowHandle=" + gridLookUpEdit1View.FocusedRowHandle + ",this.currentRoomExts.Count=" + ((this.currentRoomExts == null || this.currentRoomExts.Count == 0) ? 0 + "" : this.currentRoomExts.Count + ""));

                ////else if (e.KeyCode == Keys.Enter && this.cboExamService.EditValue == null)
                ////{
                ////    try
                ////    {
                ////        if (this.dlgFocusNextUserControl != null)
                ////        {
                ////            this.dlgFocusNextUserControl();
                ////        }
                ////    }
                ////    catch (Exception ex)
                ////    {
                ////        Inventec.Common.Logging.LogSystem.Warn(ex);
                ////        SendKeys.Send("{TAB}");
                ////    }
                ////}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //long roomId = 0;
                
                //if (e.KeyCode == Keys.Enter && gridLookUpEdit1View.FocusedRowHandle >= 0 && (this.currentRoomExts == null || this.currentRoomExts.Count == 0))
                //{
                //    roomId = Inventec.Common.TypeConvert.Parse.ToInt64(gridLookUpEdit1View.GetRowCellValue(gridLookUpEdit1View.FocusedRowHandle, "ROOM_ID").ToString());

                //}
                //Inventec.Common.Logging.LogSystem.Debug("cboRoom_KeyUp" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "gridLookUpEdit1View.FocusedRowHandle=" + gridLookUpEdit1View.FocusedRowHandle + ",this.currentRoomExts.Count=" + ((this.currentRoomExts == null || this.currentRoomExts.Count == 0) ? 0 + "" : this.currentRoomExts.Count + ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridRoomView_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {

                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long isWarn = Inventec.Common.TypeConvert.Parse.ToInt64(View.GetRowCellValue(e.RowHandle, "IS_WARN").ToString());
                    if (isWarn == 1)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.HighPriority = true;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                }

                //string sdfdf = this.gridLookUpEdit1View.GetRowCellValue(e.RowHandle, "IS_WARN").ToString();
                //if (Inventec.Common.TypeConvert.Parse.ToInt64(this.gridLookUpEdit1View.GetRowCellValue(e.RowHandle, "IS_WARN").ToString() ?? "") == 1)
                //{
                //    e.Appearance.ForeColor = Color.Red;
                //    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                //}
                //else
                //    e.Appearance.ForeColor = Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridRoomView_ColumnFilterChanged(object sender, EventArgs e)
        {
            try
            {
                int count = gridLookUpEdit1View.RowCount;
                if (count == 1)
                {
                    cboRoom.CustomSetAuto(true);
                    Inventec.Common.Controls.PopupLoader.PopupLoader.SelectFirstRowPopup(cboRoom);
                }
                else
                {
                    cboRoom.CustomSetAuto(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridRoomView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                long roomId = 0;
                var view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.KeyCode == Keys.Enter && view.FocusedRowHandle >= 0 && (this.currentRoomExts == null || this.currentRoomExts.Count == 0))
                {
                    roomId = Inventec.Common.TypeConvert.Parse.ToInt64(view.GetRowCellValue(view.FocusedRowHandle, "ROOM_ID").ToString());

                }
                Inventec.Common.Logging.LogSystem.Debug("gridRoomView_KeyDown" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId) + "gridLookUpEdit1View.FocusedRowHandle=" + view.FocusedRowHandle + ",this.currentRoomExts.Count=" + ((this.currentRoomExts == null || this.currentRoomExts.Count == 0) ? 0 + "" : this.currentRoomExts.Count + ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {

                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookupEditCustom ? (sender as GridLookupEditCustom).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (RoomExtADO rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.EXECUTE_ROOM_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom__SelectionChange(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("cboRoom__SelectionChange.1");
                //WaitingManager.Show();
                StringBuilder sbText = new StringBuilder();
                StringBuilder sbCode = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = this.cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<RoomExtADO> mestRoomSelectedNews = new List<RoomExtADO>();
                    foreach (RoomExtADO rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sbText.ToString().Length > 0) { sbText.Append(", "); }
                            sbText.Append(rv.EXECUTE_ROOM_NAME);
                            if (sbCode.ToString().Length > 0) { sbCode.Append(", "); }
                            sbCode.Append(rv.EXECUTE_ROOM_CODE);
                            mestRoomSelectedNews.Add(rv);
                        }
                    }
                    this.currentRoomExts = new List<RoomExtADO>();
                    this.currentRoomExts.AddRange(mestRoomSelectedNews);
                }

                this.cboRoom.Text = sbText.ToString();
                this.txtRoomCode.Text = sbCode.ToString();
                //WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("cboRoom__SelectionChange.2 this.cboRoom.Text=" + sbText.ToString());
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExamServiceCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(this.txtExamServiceCode.Text) && this.currentServiceRooms != null && this.currentServiceRooms.Count > 0)
                    {
                        string key = this.txtExamServiceCode.Text.ToLower();
                        var listData = this.currentServiceRooms.Where(o => o.SERVICE_CODE.ToLower().Contains(key)).ToList();
                        var searchResult = (listData != null && listData.Count > 0) ? (listData.Count == 1 ? listData : listData.Where(o => o.SERVICE_CODE.ToUpper() == key.ToUpper()).ToList()) : null;
                        if (searchResult != null && searchResult.Count == 1)
                        {
                            valid = true;
                            this.cboExamService.EditValue = searchResult.First().SERVICE_ID;

                            if (this.dlgFocusNextUserControl != null)
                            {
                                this.dlgFocusNextUserControl();
                            }
                        }
                        else
                        {
                            this.txtExamServiceCode.Text = "";
                            this.cboExamService.EditValue = null;
                        }
                    }
                    if (!valid)
                    {
                        this.FocusTocboExamService();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExamServiceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void cboExamService_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (this.cboExamService.EditValue != null)
                    {
                        if (this.dlgFocusNextUserControl != null)
                        {
                            this.dlgFocusNextUserControl();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.cboExamService.EditValue != null)
                    {
                        if (this.dlgFocusNextUserControl != null)
                        {
                            this.dlgFocusNextUserControl();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.txtExamServiceCode.Text = "";
                this.cboExamService.Properties.Buttons[1].Visible = false;
                if (this.cboExamService.EditValue != null)
                {
                    var service = this.currentServiceRooms.FirstOrDefault(o => o.SERVICE_ID == Convert.ToInt64(this.cboExamService.EditValue));
                    if (service != null)
                    {
                        this.txtExamServiceCode.Text = service.SERVICE_CODE;
                        this.cboExamService.Properties.Buttons[1].Visible = true;
                        this.ProcessPrimaryPatientTypeByService(service.SERVICE_ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrimaryPatientTypeByService(long serviceId)
        {
            try
            {
                var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault();
                if (service != null && service.BILL_PATIENT_TYPE_ID.HasValue && this.changeServiceProcessPrimaryPatientType != null)
                {
                    this.changeServiceProcessPrimaryPatientType(service.BILL_PATIENT_TYPE_ID.Value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExamService_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.cboExamService.Properties.Buttons[1].Visible = false;
                    this.cboExamService.EditValue = null;
                    this.txtExamServiceCode.Text = "";
                    this.FocusTotxtExamServiceCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (!this.btnDelete.Enabled)
                    return;
                if (this.dlgRemoveUC != null)
                    this.dlgRemoveUC(this);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

    }
}
