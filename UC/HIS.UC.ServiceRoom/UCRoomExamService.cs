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
using DevExpress.XtraGrid.Views.Grid;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using HIS.Desktop.ADO;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraBars;
using System.IO;

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
        internal List<V_HIS_ROOM> hisRooms { get; set; }
        public RemoveRoomExamService dlgRemoveUC { get; set; }
        public DelegateFocusNextUserControl dlgFocusNextUserControl;     
        Action registerPatientWithRightRouteBHYT;
        Action changeRoomNotEmergency;
        Action<long> changeServiceProcessPrimaryPatientType;
        Action<bool> changeDisablePrimaryPatientType;
        DelegateGetIntructionTime GetIntructionTime;

        Dictionary<long, HIS_PATIENT_TYPE> dicPatientType = new Dictionary<long, HIS_PATIENT_TYPE>();
        Dictionary<long, List<V_HIS_SERVICE_ROOM>> dicRoomService = new Dictionary<long, List<V_HIS_SERVICE_ROOM>>();
        Dictionary<long, RoomExtADO> dicExecuteRoom = new Dictionary<long, RoomExtADO>();
        CultureInfo currentCulture;
        List<V_HIS_SERVICE_ROOM> currentServiceRooms = new List<V_HIS_SERVICE_ROOM>();
        Dictionary<long, ResultChooseNumOrderBlockADO> dicNumOrderBlock = new Dictionary<long, ResultChooseNumOrderBlockADO>();
        
        MOS.SDO.HisPatientSDO PatientSDO { get; set; }
        Dictionary<long, string> dicBlockByAppointment = new Dictionary<long, string>();
        long? PatientClassifyId { get; set; }
        string ucName;
        long? serviceId = null;
        int popupHeight = 400;
        bool statecheckColumn;
        bool isShowContainerMediMaty = false;
        bool isShowContainerTutorial = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;
        string col1 = "";
        string col2 = "";
        string col3 = "";
        string col4 = "";
        string col5 = "";
        string col6 = "";
        string col7 = "";
        string col8 = "";
        string col9 = "";
        string col10 = "";
        string col11 = "";
        string col12 = "";
        string col13 = "";// thêm
        string col14 = "";
        string col15 = "";
        string col16 = "";
        string col17 = "";
        string tol1 = "";
        string tol2 = "";
        string tol3 = "";     
        string tol4 = "";
        string tol5 = "";
        string tol6 = "";
        string tol7 = "";
        string tol12 = "";// thêm
        string tol8 = "";
        string tol9 = "";
        string tol10 = "";
        string tol11 = "";
        
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
                SetCaptionByLanguageKeyNew();
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
                this.beditRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomExamService.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciCboRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciCboRoom.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciExamService.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciExamService.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
                this.lciRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciRoom.Text", Resources.ResourceLanguageManager.LanguageResource, this.currentCulture);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCRoomExamService
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.ServiceRoom.Resources.Lang", typeof(UCRoomExamService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExamService.Properties.NullText = Inventec.Common.Resource.Get.Value("UCRoomExamService.cboExamService.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCboRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciCboRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRoom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciRoom.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRoom.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamService.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciExamService.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamService.Text = Inventec.Common.Resource.Get.Value("UCRoomExamService.lciExamService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col1 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col2 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col3 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col4 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col5 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col6 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col7 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col8 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col9 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col10 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col11 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col12 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col13 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col14 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col15 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col16 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                col17 = Inventec.Common.Resource.Get.Value("UCRoomExamService.col17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol1 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol2 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol3 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol4 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol5 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol6 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol7 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol8 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol9 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol10 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol11 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                tol12 = Inventec.Common.Resource.Get.Value("UCRoomExamService.tol12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        var arrSearchCodes = this.txtRoomCode.Text.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrSearchCodes != null && arrSearchCodes.Count() > 0)
                        {
                            foreach (var itemCode in arrSearchCodes)
                            {
                                string code = itemCode.Trim().ToLower();
                                var listData = this.roomExts.Where(o => o.EXECUTE_ROOM_CODE.ToLower().Contains(code)).ToList();
                                var searchResult = (listData != null && listData.Count > 0) ? (listData.Count == 1 ? listData : listData.Where(o => o.EXECUTE_ROOM_CODE.ToUpper() == code.ToUpper()).ToList()) : null;
                                if (searchResult != null && searchResult.Count == 1)
                                {
                                    searchResult.ForEach(o => o.IsChecked = true);
                                }
                            }
                        }
                        var currentRoomExts = this.roomExts.Where(o => o.IsChecked == true).ToList();
                        if (currentRoomExts != null && currentRoomExts.Count > 0)
                        {
                            RoomEditValueChanged();

                            this.txtExamServiceCode.Focus();
                            this.txtExamServiceCode.SelectAll();
                        }
                        else
                        {
                            ProcessShowpopupControlContainerRoom((Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.FocusExecuteRoomOption != "1"));
                        }
                    }
                    else
                    {
                        ProcessShowpopupControlContainerRoom((Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.FocusExecuteRoomOption != "1"));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void beditRoom_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
                {
                    ProcessShowpopupControlContainerRoom(true);
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    this.txtRoomCode.Text = "";
                    this.beditRoom.Text = "";
                    this.roomExts.ForEach(o => o.IsChecked = false);
                    this.beditRoom.Properties.Buttons[1].Visible = false;
                    DevExpress.XtraGrid.Columns.GridColumn gridColumnCheck = gridViewContainerRoom.Columns["IsChecked"];
                    if (gridColumnCheck != null)
                    {
                        gridColumnCheck.ImageAlignment = StringAlignment.Center;
                        gridColumnCheck.Image = this.imageCollection1.Images[0];
                    }
                    this.cboExamService.Properties.Buttons[1].Visible = false;
                    this.cboExamService.EditValue = null;
                    this.txtExamServiceCode.Text = "";
                    FocusTotxtExamServiceCode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Rectangle GetClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                if (control.Parent != null && !(control is UserControl))
                {
                    heightPlus += bounds.Y;
                    return GetClientRectangle(control.Parent, ref  heightPlus);
                }
            }
            return bounds;
        }

        private Rectangle GetAllClientRectangle(Control control, ref int heightPlus)
        {
            Rectangle bounds = default(Rectangle);
            if (control != null)
            {
                bounds = control.Bounds;
                if (control.Parent != null)
                {
                    heightPlus += bounds.Y;
                    return GetAllClientRectangle(control.Parent, ref  heightPlus);
                }
            }
            return bounds;
        }

        private void beditRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    ProcessShowpopupControlContainerRoom(true);
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    beditRoom.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void beditRoom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(beditRoom.Text))
                {
                    beditRoom.Refresh();
                    //if (beditRoom.Text.Contains(",") || txtRoomCode.Text.Contains(","))
                    //{
                    //    isShowContainerMediMatyForChoose = true;
                    //}
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewContainerRoom.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewContainerRoom.ActiveFilterString = String.Format("[EXECUTE_ROOM_NAME] Like '%{0}%' OR [EXECUTE_ROOM_CODE] Like '%{0}%' OR [RESPONSIBLE_USERNAME] Like '%{0}%' OR [RESPONSIBLE_LOGINNAME] Like '%{0}%' OR [EXECUTE_ROOM_NAME__UNSIGN] Like '%{0}%'", beditRoom.Text);
                        gridViewContainerRoom.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewContainerRoom.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewContainerRoom.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewContainerRoom.FocusedRowHandle = 0;
                        gridViewContainerRoom.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewContainerRoom.OptionsFind.HighlightFindResults = true;

                        if (isShow)
                        {
                            ProcessShowpopupControlContainerRoom();
                            isShow = false;
                        }

                        //beditRoom.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewContainerRoom.ActiveFilter.Clear();
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerRoom.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                ColumnView View = (ColumnView)gridControlContainerRoom.FocusedView;
                if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewContainerRoom.IsEditing)
                        this.gridViewContainerRoom.CloseEditor();

                    if (this.gridViewContainerRoom.FocusedRowModified)
                        this.gridViewContainerRoom.UpdateCurrentRow();

                    int rowHandle = gridViewContainerRoom.FocusedRowHandle;

                    var rawRoom = (rowHandle >= 0) ? (RoomExtADO)this.gridViewContainerRoom.GetRow(rowHandle) : null;

                    if (rawRoom != null && rawRoom.IsChecked)
                    {
                        rawRoom.IsChecked = false;
                    }
                    else if (rawRoom != null)
                    {
                        rawRoom.IsChecked = true;
                    }
                    gridControlContainerRoom.RefreshDataSource();
                    RoomEditValueChanged();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    int rowHandle = gridViewContainerRoom.FocusedRowHandle;

                    var rawRoom = (rowHandle >= 0) ? (RoomExtADO)this.gridViewContainerRoom.GetRow(rowHandle) : null;
                    if ((roomExts != null && !roomExts.Any(o => o.IsChecked)) && rawRoom != null)
                    {
                        rawRoom.IsChecked = true;
                        gridControlContainerRoom.RefreshDataSource();
                        RoomEditValueChanged();
                    }
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;
                    popupControlContainerRoom.HidePopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerRoom_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {     
                    var row = View.GetRow(e.RowHandle) as RoomExtADO;

                    long isWarn = Inventec.Common.TypeConvert.Parse.ToInt64((View.GetRowCellValue(e.RowHandle, "IS_WARN") ?? "-1").ToString());
                    if (isWarn == 1)
                    {
                        e.Appearance.ForeColor = Color.Red;
                        e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                        e.HighPriority = true;
                    }
                    else      
                    {
                        if (row != null && row.IS_PAUSE_ENCLITIC == 1)
                        {
                            e.Appearance.ForeColor = Color.Gray;
                        }
                        else
                        {
                            long ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(View.GetRowCellValue(e.RowHandle, "ROOM_ID").ToString());
                            long? RESPONSIBLE_TIME = View.GetRowCellValue(e.RowHandle, "RESPONSIBLE_TIME") != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(View.GetRowCellValue(e.RowHandle, "RESPONSIBLE_TIME").ToString()) : null;
                            //+ Với các dòng dữ liệu không bị bôi đỏ và có RESPONSIBLE_TIME trong HIS_ROOM nằm trong khoảng: 
                            //  tg hiện tại - 5p <= RESPONSIBLE_TIME <= tg hiện tại + 10p thì bôi màu vàng.                    
                            if (RESPONSIBLE_TIME.HasValue && RESPONSIBLE_TIME.Value > 0)
                            {
                                DateTime now = DateTime.Now;
                                DateTime nowPre5P = now.AddMinutes(-5);
                                DateTime nowNexr10P = now.AddMinutes(10);
                                DateTime resTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(RESPONSIBLE_TIME.Value).Value;
                                if (resTime != DateTime.MinValue && (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(nowPre5P) ?? 0) <= (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(resTime) ?? 0) && (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(nowNexr10P) ?? 0) >= (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(resTime) ?? 0))
                                {
                                    e.Appearance.ForeColor = Color.Blue;
                                }
                                else
                                    e.Appearance.ForeColor = Color.Black;
                            }
                            else
                                e.Appearance.ForeColor = Color.Black;
                        }
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerRoom_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);

                    if (hi.InColumn && e.Button == MouseButtons.Left)
                    {
                        gridViewContainerRoom.ClearSelection();
                        gridViewContainerRoom.FocusedRowHandle = GridControl.InvalidRowHandle;
                        (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        return;
                    }

                    if (e.Button == MouseButtons.Right && ModifierKeys == Keys.None)
                    {
                        if (view != null)
                        {
                            view.ColumnsCustomization();
                            Rectangle screenBounds = Screen.GetBounds(view.GridControl);

                            int x = screenBounds.Right - view.CustomizationForm.Width;
                            int y = screenBounds.Bottom - view.CustomizationForm.Height;

                            view.CustomizationForm.Location = new Point(x, y);
                        }
                    }

                    if (hi.Column.FieldName == "IsChecked" && hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();
                            }

                            gridControlContainerRoom.RefreshDataSource();
                            RoomEditValueChanged();

                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    else if (hi.Column.FieldName == "IsChecked" && hi.InColumnPanel)
                    {

                        statecheckColumn = !statecheckColumn;
                        this.SetCheckAllColumn(statecheckColumn);

                        int rowHandle = gridViewContainerRoom.FocusedRowHandle;

                        var rawRoom = (rowHandle >= 0) ? (RoomExtADO)this.gridViewContainerRoom.GetRow(rowHandle) : null;

                        long roomIdFocus = rawRoom != null ? rawRoom.ROOM_ID : 0;
                        this.roomExts.ForEach(o => o.IsChecked = statecheckColumn);
                        var roomFocus = this.roomExts.FirstOrDefault(o => o.ROOM_ID == roomIdFocus);
                        if (roomFocus != null)
                        {
                            roomFocus.IsChecked = !roomFocus.IsChecked;
                        }
                        gridControlContainerRoom.RefreshDataSource();
                        RoomEditValueChanged();
                    }
                    else if (hi.Column.FieldName == "NumOrderBlock" && hi.InRowCell)
                    {
                        view.FocusedRowHandle = hi.RowHandle;
                        view.FocusedColumn = hi.Column;
                        var rawRoom = (RoomExtADO)this.gridViewContainerRoom.GetRow(hi.RowHandle);
                        
                        CallModuleNumOrderBlockChooser(rawRoom);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GenerateHour(string hour)
        {
            if (!String.IsNullOrWhiteSpace(hour))
                return new StringBuilder().Append(hour.Substring(0, 2)).Append(":").Append(hour.Substring(2, 2)).ToString();
            else
                return "";
        }

        private void gridControlContainerRoom_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                GridControl grid = sender as GridControl;
                GridView view = grid.FocusedView as GridView;
                if (e.KeyCode == Keys.Tab || e.KeyCode == Keys.Enter)
                {
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        //grid.SelectNextControl(btnChoice, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    if (this.gridViewContainerRoom.IsEditing)
                        this.gridViewContainerRoom.CloseEditor();

                    if (this.gridViewContainerRoom.FocusedRowModified)
                        this.gridViewContainerRoom.UpdateCurrentRow();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewContainerRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] != null && ((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex] is RoomExtADO)
                    {
                        RoomExtADO data = (RoomExtADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                        if (e.Column.FieldName == "RESPONSIBLE_USERNAME_DISPLAY" && data != null)
                        {
                            e.Value = (!String.IsNullOrEmpty(data.WORKING_USERNAME) ? String.Format("{0};{1}", data.RESPONSIBLE_USERNAME, data.WORKING_USERNAME) : data.RESPONSIBLE_USERNAME);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlContainerRoom_Click(object sender, EventArgs e)
        {
            try
            {
                int rowHandle = gridViewContainerRoom.FocusedRowHandle;

                var rawRoom = (rowHandle >= 0) ? (RoomExtADO)this.gridViewContainerRoom.GetRow(rowHandle) : null;
                if (rawRoom != null)
                {
                    rawRoom.IsChecked = !rawRoom.IsChecked;
                    gridControlContainerRoom.RefreshDataSource();
                    isShowContainerMediMaty = true;
                    RoomEditValueChanged();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainerRoom_CloseUp(object sender, EventArgs e)
        {
            try
            {
                bool isExistsCheckeds = this.roomExts.Any(o => o.IsChecked);
                this.beditRoom.Properties.Buttons[1].Visible = isExistsCheckeds;
                isShow = true;
                if (isExistsCheckeds)
                {
                    this.SetDataSourceCboExamService();
                    this.FocusTotxtExamServiceCode();
                }
                else
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
					else
					{
						if (this.changeServiceProcessPrimaryPatientType != null)
						{
							this.changeServiceProcessPrimaryPatientType(0);
						}
						if (this.changeDisablePrimaryPatientType != null)
						{
							this.changeDisablePrimaryPatientType(false);
						}
					}
				}
			}
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsContainString(string arrStr,string str)
		{
            bool result = false;
			try
			{
                if(arrStr.Contains(","))
				{
                    var arr = arrStr.Split(',');
					for (int i = 0; i < arr.Length; i++)
					{
                        result = arr[i] == str;
                        if (result) break;
					}
				}
				else
				{
                    result = arrStr == str;
				}                    
			}
			catch (Exception ex)
			{
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
		}

        private void ProcessPrimaryPatientTypeByService(long serviceId)
        {
            try
            {
				
				if (this.changeDisablePrimaryPatientType != null)
				{
					this.changeDisablePrimaryPatientType(true);
				}
				var service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.ID == serviceId).FirstOrDefault();
                if (service != null && service.BILL_PATIENT_TYPE_ID.HasValue && this.changeServiceProcessPrimaryPatientType != null
                    && ((string.IsNullOrEmpty(service.APPLIED_PATIENT_TYPE_IDS) || ( service.APPLIED_PATIENT_TYPE_IDS != null && currentPatientTypeAlter.PATIENT_TYPE_ID > 0 && IsContainString(service.APPLIED_PATIENT_TYPE_IDS,currentPatientTypeAlter.PATIENT_TYPE_ID.ToString()))) && ((string.IsNullOrEmpty(service.APPLIED_PATIENT_CLASSIFY_IDS) || (service.APPLIED_PATIENT_CLASSIFY_IDS != null && PatientClassifyId != null && IsContainString(service.APPLIED_PATIENT_CLASSIFY_IDS, PatientClassifyId != null ? PatientClassifyId.ToString() : "-1"))))))
                {
                    this.changeServiceProcessPrimaryPatientType(service.BILL_PATIENT_TYPE_ID.Value);
					if (service.IS_NOT_CHANGE_BILL_PATY == (short?)1 && this.changeDisablePrimaryPatientType != null)
					{
						this.changeDisablePrimaryPatientType(false);
					}
					else
					{
						this.changeDisablePrimaryPatientType(true);
					}
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

        private void gridViewContainerRoom_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string numOrderBlock = (gridViewContainerRoom.GetRowCellValue(e.RowHandle, "NumOrderBlock") ?? "").ToString().Trim();
                    string IsBlockNumOrder = (gridViewContainerRoom.GetRowCellValue(e.RowHandle, "IsBlockNumOrder") ?? "0").ToString().Trim();

                    if (e.Column.FieldName == "NumOrderBlock" && IsBlockNumOrder == "1")
                    {
                        if (String.IsNullOrWhiteSpace(numOrderBlock))
                        {
                            e.RepositoryItem = repositoryItemBtnChooseHide;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnChoose;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnChooseHide_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                int rowHandle = gridViewContainerRoom.FocusedRowHandle;

                var rawRoom = (rowHandle >= 0) ? (RoomExtADO)this.gridViewContainerRoom.GetRow(rowHandle) : null;
                CallModuleNumOrderBlockChooser(rawRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewContainerRoom_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void gridViewContainerRoom_ColumnPositionChanged(object sender, EventArgs e)
        {
            SaveLayoutForCurrentUser(gridViewContainerRoom);
        }

        private void gridViewContainerRoom_EndSorting(object sender, EventArgs e)
        {
            SaveLayoutForCurrentUser(gridViewContainerRoom);
        }

        private void gridViewContainerRoom_ShowingEditor(object sender, CancelEventArgs e)
        {
            SaveLayoutForCurrentUser(gridViewContainerRoom);
        }
    }
}
