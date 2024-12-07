using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.CustomControl.CustomGrid;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignNutritionRefectory
{
    public partial class frmAssign : Form
    {
        List<HIS_DEPARTMENT> listSelectedDeaprtment = new List<HIS_DEPARTMENT>();
        List<V_HIS_BED_ROOM> listSelectedRoom = new List<V_HIS_BED_ROOM>();
        public long ADD_TIME_FROM { get; set; }
        public long INTRUCTION_TIME_FROM { get; set; }
        public long ADD_TIME_TO { get; set; }
        public long INTRUCTION_TIME_TO { get; set; }
        public long TreatmentID { get; set; }

        Inventec.Desktop.Common.Modules.Module moduleData;
        public frmAssign(Inventec.Desktop.Common.Modules.Module modules)
        {
            InitializeComponent();
            this.moduleData = modules;
        }

        private void frmAssign_Load(object sender, EventArgs e)
        {
            try
            {
                LogSystem.Debug("frmAssign___Load.START");
                InitDataControl();
                SetCaptionByLanguageKey();
                SetDefaultValue();
                SetDefaultValueServiceReq();
                FillDataToControlGridTreatment();
                //FillDataToControlGridServiceReq();
                CheckISAdmin();
                this.gridControlServiceReq.ToolTipController = this.toolTipController1;
                #region ---- Icon ----
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                    if (moduleData != null)
                    {
                        this.Text = moduleData.text;
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
                #endregion
                LogSystem.Debug("frmAssign___Load.END");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmAssign
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.AssignNutritionRefectory.Resources.Lang", typeof(frmAssign).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset1.Text = Inventec.Common.Resource.Get.Value("frmAssign.btnReset1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind1.Text = Inventec.Common.Resource.Get.Value("frmAssign.btnFind1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceReqCode.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssign.txtServiceReqCode.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssign.txtServiceReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRation.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssign.cboRation.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmAssign.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmAssign.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssign.txtSearchValue.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchValue.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmAssign.txtSearchValue.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("frmAssign.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDepartment.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssign.cboDepartment.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssign.cboRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientIn.Properties.NullText = Inventec.Common.Resource.Get.Value("frmAssign.cboPatientIn.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmAssign.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmAssign.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.moduleData != null)
                {
                    this.Text = this.moduleData.text.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetDefaultValue()
        {
            try
            {
                SetDefaultControl();
                SetDefaultValueTime();
                LoadDataToDepartment(false);
                LoadDataToBedRoom(false);
                LoadDataToPatient();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetDefaultValueServiceReq()
        {
            try
            {
                txtServiceReqCode.Text = "";
                cboRation.EditValue = null;
                dtTimeFrom.EditValue = null;
                dtTimeTo.EditValue = null;
                this.INTRUCTION_TIME_FROM = 0;
                this.INTRUCTION_TIME_TO = 0;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetDefaultControl()
        {
            try
            {
                txtSearchValue.Text = "";
                cboTimeTo.Enabled = cboTimeFrom.Enabled = false;
                cboTimeFrom.EditValue = cboTimeTo.EditValue = null;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueTime()
        {
            try
            {
                this.ADD_TIME_FROM = this.ADD_TIME_TO = this.INTRUCTION_TIME_FROM = this.INTRUCTION_TIME_TO = 0;

                dtTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now() ?? 0)) ?? DateTime.Now;
                dtTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now() ?? 0)) ?? DateTime.Now;
                this.INTRUCTION_TIME_FROM = (long)Inventec.Common.DateTime.Get.Now() / 1000000 * 1000000;
                this.INTRUCTION_TIME_TO = (long)Inventec.Common.DateTime.Get.Now() / 1000000 * 1000000 + 235959;

                if (cboTimeFrom.Enabled == false) return;
                cboTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now() ?? 0)) ?? DateTime.Now;
                cboTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((Inventec.Common.DateTime.Get.Now() ?? 0)) ?? DateTime.Now;
                this.ADD_TIME_FROM = (long)Inventec.Common.DateTime.Get.Now() / 1000000 * 1000000;
                this.ADD_TIME_TO = (long)Inventec.Common.DateTime.Get.Now() / 1000000 * 1000000 + 235959;


            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #region init data
        private void InitDataControl()
        {
            try
            {
                LoadDataToDepartment(true);
                LoadDataToBedRoom(true);
                LoadDataToRation();
                LoadDataToPatient();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void InitComboDepartmentCheck(bool isFirstLoad)
        {
            try
            {

                if (isFirstLoad)
                {
                    GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDepartment.Properties);
                    gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check);
                    cboDepartment.Properties.Tag = gridCheck;
                    cboDepartment.Properties.View.OptionsSelection.MultiSelect = true;
                }
                GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDepartment.Properties.View);
                    gridCheckMark.SelectAll(this.listSelectedDeaprtment);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_Check(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<HIS_DEPARTMENT> listOldSelected = null;
                if (listSelectedDeaprtment != null && listSelectedDeaprtment.Count > 0)
                {
                    listOldSelected = listSelectedDeaprtment.Where(s => string.IsNullOrEmpty(s.DEPARTMENT_CODE)).Distinct().ToList();
                }
                listSelectedDeaprtment.Clear();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_DEPARTMENT> erSelectedNews = new List<HIS_DEPARTMENT>();
                    foreach (HIS_DEPARTMENT er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            erSelectedNews.Add(er);
                        }
                    }

                    if (this.listSelectedDeaprtment == null) this.listSelectedDeaprtment = new List<HIS_DEPARTMENT>();
                    this.listSelectedDeaprtment.AddRange(erSelectedNews);
                    LoadDataToBedRoom(false);

                }
                cboDepartment.Text = string.Join("; ", listSelectedDeaprtment.Select(s => s.DEPARTMENT_NAME));
                if (cboDepartment.Properties.Buttons.Count > 1)
                {
                    cboDepartment.Properties.Buttons[1].Visible = !(listSelectedDeaprtment == null || listSelectedDeaprtment.Count == 0);
                }
                //WaitingManager.Show();

                //var customEventArgs = new DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs(cboDepartment, cboDepartment.Text);

                //cboDepartment_CustomDisplayText(cboDepartment, customEventArgs);


                WaitingManager.Hide();
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSelectedDeaprtment.Count), listSelectedDeaprtment.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = string.Join("; ", listSelectedRoom.Select(s => s.BED_ROOM_NAME));
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = string.Join("; ", listSelectedDeaprtment.Select(s => s.DEPARTMENT_NAME));

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void InitComboDepartmentName(List<HIS_DEPARTMENT> data, bool isFirstLoad)
        {
            try
            {
                cboDepartment.Properties.DataSource = data;
                if (!isFirstLoad) return;
                cboDepartment.Properties.DisplayMember = "DEPARTMENT_NAME";
                cboDepartment.Properties.ValueMember = "ID";

                cboDepartment.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cboDepartment.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                cboDepartment.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboDepartment.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                cboDepartment.Properties.View.OptionsView.ShowDetailButtons = false;
                cboDepartment.Properties.View.OptionsView.ShowGroupPanel = false;
                cboDepartment.Properties.View.OptionsView.ShowIndicator = false;


                DevExpress.XtraGrid.Columns.GridColumn column = cboDepartment.Properties.View.Columns.AddField("DEPARTMENT_CODE");
                column.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                column.VisibleIndex = 1;
                column.Width = 50;
                column.Caption = "Mã";

                DevExpress.XtraGrid.Columns.GridColumn column1 = cboDepartment.Properties.View.Columns.AddField("DEPARTMENT_NAME");
                column1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                column1.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                column1.VisibleIndex = 2;
                column1.Width = 250;
                column1.Caption = "Tên";
                cboDepartment.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboDepartment.Properties.View.OptionsSelection.MultiSelect = true;
                cboDepartment.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataToDepartment(bool isFirstLoad)
        {
            try
            {
                LogSystem.Debug("LoadDataToDepartment___Start");
                var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(s => s.IS_ACTIVE == 1).ToList();
                listSelectedDeaprtment.AddRange(data);
                InitComboDepartmentName(data, isFirstLoad);
                InitComboDepartmentCheck(isFirstLoad);
                GridCheckMarksSelection gridCheckMark = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(data);
                }


            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void InitComboBedRoomCheck(bool isFirstLoad)
        {
            try
            {


                if (isFirstLoad)
                {
                    GridCheckMarksSelection gridCheckMarkRoom = new GridCheckMarksSelection(cboRoom.Properties);
                    gridCheckMarkRoom.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_CheckRoom);
                    cboRoom.Properties.Tag = gridCheckMarkRoom;
                    cboRoom.Properties.View.OptionsSelection.MultiSelect = true;
                }
                GridCheckMarksSelection gridCheckMarkSelectionRoom = cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMarkSelectionRoom != null)
                {
                    gridCheckMarkSelectionRoom.ClearSelection(cboRoom.Properties.View);
                    gridCheckMarkSelectionRoom.SelectAll(this.listSelectedRoom);

                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Event_CheckRoom(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_BED_ROOM> listOldSelected = null;
                if (listSelectedRoom != null && listSelectedRoom.Count > 0)
                {
                    listOldSelected = listSelectedRoom.Where(s => string.IsNullOrEmpty(s.BED_ROOM_NAME)).Distinct().ToList();
                }
                listSelectedRoom.Clear();
                GridCheckMarksSelection gridCheckMarkSelectionRoom = sender as GridCheckMarksSelection;
                if (gridCheckMarkSelectionRoom != null)
                {
                    List<V_HIS_BED_ROOM> erSelectedNews = new List<V_HIS_BED_ROOM>();
                    foreach (V_HIS_BED_ROOM er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            erSelectedNews.Add(er);
                        }
                    }
                    this.listSelectedRoom = new List<V_HIS_BED_ROOM>();
                    this.listSelectedRoom.AddRange(erSelectedNews);

                }
                if (cboRoom.Properties.Buttons.Count > 1)
                {
                    cboRoom.Properties.Buttons[1].Visible = !(listSelectedRoom == null || listSelectedRoom.Count == 0);
                }
                cboRoom.Text = string.Join("; ", listSelectedRoom.Select(s => s.BED_ROOM_NAME));
                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listSelectedRoom.Count), listSelectedRoom.Count));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataToBedRoom(bool isFirstLoad)
        {
            try
            {
                LogSystem.Debug("LoadDataToBedRoom___Start");
                var data = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(s => s.IS_ACTIVE == 1 && listSelectedDeaprtment.Select(d => d.ID).ToList().Contains(s.DEPARTMENT_ID)).ToList();
                listSelectedRoom.AddRange(data);
                InitComboBedRoom(data, isFirstLoad);
                InitComboBedRoomCheck(isFirstLoad);
                GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(data);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void InitComboBedRoom(List<V_HIS_BED_ROOM> data, bool isFirstLoad)
        {
            try
            {

                cboRoom.Properties.DataSource = data;
                if (!isFirstLoad) return;
                cboRoom.Properties.DisplayMember = "BED_ROOM_NAME";
                cboRoom.Properties.ValueMember = "ID";

                cboRoom.Properties.View.OptionsView.GroupDrawMode = DevExpress.XtraGrid.Views.Grid.GroupDrawMode.Office;
                cboRoom.Properties.View.OptionsView.HeaderFilterButtonShowMode = DevExpress.XtraEditors.Controls.FilterButtonShowMode.SmartTag;
                cboRoom.Properties.View.OptionsView.ShowAutoFilterRow = true;
                cboRoom.Properties.View.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
                cboRoom.Properties.View.OptionsView.ShowDetailButtons = false;
                cboRoom.Properties.View.OptionsView.ShowGroupPanel = false;
                cboRoom.Properties.View.OptionsView.ShowIndicator = false;


                DevExpress.XtraGrid.Columns.GridColumn column = cboRoom.Properties.View.Columns.AddField("BED_ROOM_CODE");
                column.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                column.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                column.VisibleIndex = 1;
                column.Width = 50;
                column.Caption = "Mã";

                DevExpress.XtraGrid.Columns.GridColumn column1 = cboRoom.Properties.View.Columns.AddField("BED_ROOM_NAME");
                column1.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                column1.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.True;
                column1.VisibleIndex = 2;
                column1.Width = 250;
                column1.Caption = "Tên";
                cboRoom.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboRoom.Properties.View.OptionsSelection.MultiSelect = true;
                cboRoom.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDataToRation()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_RATION_TIME>().Where(s => s.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columInfos = new List<ColumnInfo>();
                columInfos.Add(new ColumnInfo("RATION_TIME_CODE", "", 100, 1));
                columInfos.Add(new ColumnInfo("RATION_TIME_NAME", "", 200, 2));
                ControlEditorADO controlEditorDAO = new ControlEditorADO("RATION_TIME_NAME", "ID", columInfos, false, 300);
                ControlEditorLoader.Load(cboRation, data, controlEditorDAO);

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        class FILTER_PATIENT
        {
            public long ID { get; set; }
            public string VALUE { get; set; }
        }
        private void LoadDataToPatient()
        {
            try
            {
                var data = new List<FILTER_PATIENT>();
                data.Add(new FILTER_PATIENT { ID = 1, VALUE = "BN đang ở trong buồng" });
                data.Add(new FILTER_PATIENT { ID = 2, VALUE = "BN vào buồng trong khoảng" });
                List<ColumnInfo> columInfos = new List<ColumnInfo>();
                columInfos.Add(new ColumnInfo("VALUE", "", 200, 2));
                ControlEditorADO controlEditorDAO = new ControlEditorADO("VALUE", "ID", columInfos, false, 200);
                ControlEditorLoader.Load(cboPatientIn, data, controlEditorDAO);
                cboPatientIn.EditValue = 1;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        #region LOAD DATA TO GRID 1 TREATMENT
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private void FillDataToControlGridTreatment()
        {
            try
            {
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPagingTreatment.pagingGrid != null)
                {
                    pageSize = ucPagingTreatment.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPagingTreatment.Init(LoadPaging, param, pageSize, this.gridControlTreatment);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        MOS.Filter.HisTreatmentBedRoomLViewFilter filter = null;
        private void LoadPaging(object param)
        {
            try
            {
                LogSystem.Debug("FillDataToControlGridTreatment___Start");
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                gridControlTreatment.BeginUpdate();

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM>> apiResult = null;
                filter = new HisTreatmentBedRoomLViewFilter();
                UpdateDataFilterTreatment(ref filter);
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                apiResult = new BackendAdapter(paramCommon).GetRO<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {

                        gridViewTreatment.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);


                    }
                    else gridControlTreatment.DataSource = null;
                }
                gridControlTreatment.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void UpdateDataFilterTreatment(ref HisTreatmentBedRoomLViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtSearchValue.Text))
                {
                    filter.KEYWORD__PATIENT_NAME__TREATMENT_CODE__BED_NAME__PATIENT_CODE = txtSearchValue.Text;
                }
                if (listSelectedRoom != null && listSelectedRoom.Count > 0)
                {
                    filter.BED_ROOM_IDs = listSelectedRoom.Select(s => s.ID).ToList();
                }
                if (cboPatientIn.EditValue != null && Convert.ToInt64(cboPatientIn.EditValue) == 1)
                {
                    filter.IS_IN_ROOM = true;
                }

                if (this.ADD_TIME_FROM > 0 && string.IsNullOrWhiteSpace(txtSearchValue.Text))
                {
                    filter.ADD_TIME_FROM = this.ADD_TIME_FROM;
                }
                else
                {
                    filter.ADD_TIME_FROM = null;
                }
                if (this.ADD_TIME_TO > 0 && string.IsNullOrWhiteSpace(txtSearchValue.Text))
                {
                    filter.ADD_TIME_TO = this.ADD_TIME_TO;
                }
                else filter.ADD_TIME_TO = null;

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        #region LOAD DATA TO GRID 2 SERVICE_REQ
        int rowCountSe = 0;
        int dataTotalSe = 0;
        int startPageSe = 0;
        private void FillDataToControlGridServiceReq()
        {
            try
            {
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPagingServiceReq.pagingGrid != null)
                {
                    pageSize = ucPagingServiceReq.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPagingSe(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCountSe;
                param.Count = dataTotalSe;
                ucPagingServiceReq.Init(LoadPagingSe, param, pageSize, this.gridControlServiceReq);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPagingSe(object param)
        {
            try
            {
                LogSystem.Debug("FillDataToControlGridServiceReq___Start");
                startPageSe = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPageSe, limit);
                gridControlServiceReq.BeginUpdate();

                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10>> apiResult = null;
                MOS.Filter.HisServiceReqView10Filter filter = new MOS.Filter.HisServiceReqView10Filter();
                UpdateDataFilterServiceReq(ref filter);
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN;
                filter.REQUEST_ROOM_ID = this.moduleData.RoomId;
                filter.TREATMENT_ID = this.TreatmentID;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SERVICE_REQ_10>>("api/HisServiceReq/GetView10", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {

                        gridViewServiceReq.GridControl.DataSource = data;
                        rowCountSe = (data == null ? 0 : data.Count);
                        dataTotalSe = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);


                    }
                    else gridControlServiceReq.DataSource = null;
                }
                gridControlServiceReq.EndUpdate();
                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void UpdateDataFilterServiceReq(ref HisServiceReqView10Filter filter)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                {
                    filter.SERVICE_REQ_CODE = txtServiceReqCode.Text;
                }
                if (cboRation.EditValue != null)
                {
                    filter.RATION_TIME_ID = Convert.ToInt64(cboRation.EditValue);
                }
                if (this.INTRUCTION_TIME_FROM > 0)
                {
                    filter.INTRUCTION_TIME_FROM = this.INTRUCTION_TIME_FROM;
                }
                if (this.INTRUCTION_TIME_TO > 0) { filter.INTRUCTION_TIME_TO = this.INTRUCTION_TIME_TO; }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    L_HIS_TREATMENT_BED_ROOM pData = (L_HIS_TREATMENT_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Calculation.Age(pData.TDL_PATIENT_DOB);
                    }
                    else if (e.Column.FieldName == "TDL_HEIN_CARD_FROM_TIME_STR")
                    {
                        e.Value = !string.IsNullOrEmpty(pData.TDL_HEIN_CARD_NUMBER) ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_HEIN_CARD_FROM_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_HEIN_CARD_TO_TIME ?? 0) : "";
                    }
                    else if (e.Column.FieldName == "DAY_COUNT")
                    {
                        if (pData.CLINICAL_IN_TIME == null)
                            e.Value = "";
                        else
                        {
                            TimeSpan? durationTime = new TimeSpan(0, 0, 0, 0);
                            durationTime = DateTime.Now - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(pData.CLINICAL_IN_TIME ?? 0);
                            e.Value = durationTime.Value.Days + 1;
                        }

                    }
                    else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CLINICAL_IN_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "NOTE_STR")
                    {
                        e.Value = !string.IsNullOrEmpty(pData.APPROVE_FINISH_NOTE) ? pData.APPROVE_FINISH_NOTE + " - " + pData.NOTE : pData.NOTE;
                    }
                    else if (e.Column.FieldName == "ADD_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.ADD_TIME);
                    }


                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ_10 pData = (V_HIS_SERVICE_REQ_10)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }

                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.INTRUCTION_TIME);
                    }
                    else if (e.Column.FieldName == "STATUS")
                    {
                        //Chưa xử lý: màu trắng
                        //Đã xử lý: màu vàng
                        //Hoàn thành: màu đỏ

                        long statusId = pData.SERVICE_REQ_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                        {
                            e.Value = imageList1.Images[2];

                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                        {
                            e.Value = imageList1.Images[1];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.Value = imageList1.Images[0];
                        }

                    }


                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void gridViewServiceReq_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_SERVICE_REQ_10 data = (V_HIS_SERVICE_REQ_10)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EDIT")
                        {
                            if (!(data.IS_NO_EXECUTE == 1) && (this.LoginName == data.REQUEST_LOGINNAME || this.IsAdmin) && (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL || ConfigCFG.ALLOW_MODIFYING_OF_STARTED == "1"))
                            {
                                e.RepositoryItem = repoBtnEditE;
                            }
                            else
                            {
                                e.RepositoryItem = null;
                            }
                        }
                        if (e.Column.FieldName == "DELETE")
                        {
                            if (( this.LoginName == data.REQUEST_LOGINNAME || this.IsAdmin) && data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                e.RepositoryItem = repoBtnDeleteE;
                            }
                            else e.RepositoryItem = repoBtnDeleteD;
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        public string LoginName { get; set; }
        public bool IsAdmin { get; set; }
        private bool CheckISAdmin()
        {
            bool isAdmin = false;
            try
            {
                this.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim();
                this.IsAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(LoginName);
                LogSystem.Debug("LoginName: " + LoginName + " , IS_ADMIN: " + IsAdmin);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
            return isAdmin;
        }

        private void gridViewTreatment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var rowData = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatment.GetFocusedRow();
                if (rowData != null)
                {
                    this.TreatmentID = rowData.TREATMENT_ID;
                    FillDataToControlGridServiceReq();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void repoBtnAssign_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                L_HIS_TREATMENT_BED_ROOM data = (L_HIS_TREATMENT_BED_ROOM)gridViewTreatment.GetFocusedRow();
                if (data != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.TREATMENT_ID);
                    filter.TREATMENT_ID = data.TREATMENT_ID;
                    listArgs.Add(filter ?? new HisTreatmentBedRoomLViewFilter() { TREATMENT_ID = data.TREATMENT_ID });
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignNutrition", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    Inventec.Common.Logging.LogSystem.Debug("Call module : HIS.Desktop.Plugins.AssignNutrition" + LogUtil.TraceData("listArgs", listArgs));
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControlGridTreatment();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    listSelectedRoom.Clear();
                    GridCheckMarksSelection gridCheck = cboRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheck != null)
                    {
                        gridCheck.ClearSelection(cboRoom.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    listSelectedDeaprtment.Clear();
                    GridCheckMarksSelection gridCheck = cboDepartment.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheck != null)
                    {
                        gridCheck.ClearSelection(cboDepartment.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #region EDIT VALUE CHANGE
        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboRoom.Properties.Buttons[1].Visible = cboRoom.EditValue != null;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTimeFrom.EditValue != null && cboTimeFrom.DateTime != null && cboTimeFrom.DateTime != DateTime.MinValue)
                {
                    long time_from = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(cboTimeFrom.DateTime) ?? 0;
                    this.ADD_TIME_FROM = time_from > 0 ? time_from / 1000000 * 1000000 : 0;
                }
                else this.ADD_TIME_FROM = 0;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboTimeTo.EditValue != null && cboTimeTo.DateTime != null && cboTimeTo.DateTime != DateTime.MinValue)
                {
                    long time_to = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(cboTimeTo.DateTime) ?? 0;
                    this.ADD_TIME_TO = time_to > 0 ? time_to / 1000000 * 1000000 + 235959 : 0;
                }
                else this.ADD_TIME_TO = 0;

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void dtTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != null && dtTimeFrom.DateTime != DateTime.MinValue)
                {
                    long time_from = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeFrom.DateTime) ?? 0;
                    this.INTRUCTION_TIME_FROM = time_from > 0 ? time_from / 1000000 * 1000000 : 0;
                }
                else this.INTRUCTION_TIME_FROM = 0;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void dtTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != null && dtTimeTo.DateTime != DateTime.MinValue)
                {
                    long time_from = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeTo.DateTime) ?? 0;
                    this.INTRUCTION_TIME_TO = time_from > 0 ? time_from / 1000000 * 1000000 + 235959 : 0;
                }
                else
                {
                    this.INTRUCTION_TIME_TO = 0;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboPatientIn_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientIn.EditValue != null)
                {
                    if (Convert.ToInt64(cboPatientIn.EditValue) == 1)
                    {
                        this.ADD_TIME_FROM = this.ADD_TIME_TO = 0;
                        cboTimeFrom.Enabled = cboTimeTo.Enabled = false;
                    }
                    else
                    {
                        cboTimeTo.Enabled = cboTimeFrom.Enabled = true;
                        SetDefaultValueTime();
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion
        #region BUTTON CLICK
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtSearchValue_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToControlGridTreatment();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void frmAssign_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    btnFind.PerformClick();
                }
                else if (e.Control && e.KeyCode == Keys.R)
                {
                    btnReset.PerformClick();
                }
                else if (e.KeyCode == Keys.F2)
                {
                    txtSearchValue.Focus();
                    txtSearchValue.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnFind1_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControlGridServiceReq();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnReset1_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueServiceReq();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion
        #region REPO BUTTON CLICK
        private void repoBtnEditE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                V_HIS_SERVICE_REQ_10 data = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetFocusedRow();
                if (data != null)
                {
                    List<object> listArgs = new List<object>();

                    AssignServiceEditADO ado = new AssignServiceEditADO(data.ID, data.INTRUCTION_TIME, RefreshClick);
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AssignNutritionEdit", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    Inventec.Common.Logging.LogSystem.Debug("Call module : HIS.Desktop.Plugins.AssignNutritionEdit" + LogUtil.TraceData("listArgs", listArgs));
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        public void RefreshClick()
        {
            try
            {
                WaitingManager.Show();
                FillDataToControlGridServiceReq();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void repoBtnDeleteE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_SERVICE_REQ_10 data = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetFocusedRow();
                if (data != null)
                {
                    if (MessageBox.Show(this, "Bạn có chắc muốn hủy dữ liệu?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        Inventec.Common.Logging.LogSystem.Debug("Call api xoa du lieu" + LogUtil.TraceData("V_HIS_SERVICE_REQ_10", data));
                        bool rs = new BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, data, param);
                        MessageManager.Show(this, param, rs);
                    }
                }
            }

            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        V_HIS_SERVICE_REQ_10 currentServiceReq { get; set; }
        private void repoBtnPrintE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_SERVICE_REQ_10 data = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    currentServiceReq = data;
                    ProcessingPrintV2("Mps000275");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        #region PROCESS PRINT
        private void ProcessingPrintV2(string printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                switch (printType)
                {
                    case "Mps000275":
                        richEditorMain.RunPrintTemplate("Mps000275", DelegateRunPrinter);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000275":
                        Mps000275(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000275(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                if (this.currentServiceReq != null)
                {
                    List<V_HIS_SERE_SERV> currnentSS = new List<V_HIS_SERE_SERV>();
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, new HisSereServViewFilter() { SERVICE_REQ_ID = this.currentServiceReq.ID }, null);
                    if (rs != null)
                    {
                        currnentSS = rs;
                    }
                    List<V_HIS_SERVICE_REQ> lsitServiceReq = new List<V_HIS_SERVICE_REQ>();
                    V_HIS_SERVICE_REQ sv = new V_HIS_SERVICE_REQ();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERVICE_REQ>(sv, this.currentServiceReq);
                    lsitServiceReq.Add(sv);
                    List<HIS_SERE_SERV_RATION> listRation = new List<HIS_SERE_SERV_RATION>();
                    var ration = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_RATION>>("api/HisSereServRation/Get", ApiConsumers.MosConsumer, new HisSereServRationFilter() { SERVICE_REQ_ID = this.currentServiceReq.ID }, null);
                    if (ration != null) listRation = ration;
                    MPS.Processor.Mps000275.PDO.Mps000275PDO mps000275PDO = new MPS.Processor.Mps000275.PDO.Mps000275PDO
                    (
                        lsitServiceReq, currnentSS, listRation, null
                    );
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Debug("In mps00275: " + LogUtil.TraceData("mps000275PDO", mps000275PDO));
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000275PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000275PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    result = MPS.MpsPrinter.Run(PrintData);

                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl != gridControlServiceReq)
                    return;

                GridView view = gridControlServiceReq.FocusedView as GridView;
                if (view == null) return;

                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);

                if (info.InRowCell && info.Column.FieldName == "STATUS")
                {
                    if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                    {
                        lastColumn = info.Column;
                        lastRowHandle = info.RowHandle;
                        // Lấy dữ liệu của hàng hiện tại
                        var data = (V_HIS_SERVICE_REQ_10)view.GetRow(lastRowHandle);
                        if (data != null)
                        {
                            // Xác định nội dung tooltip
                            string tooltipText = "";
                            long statusId = data.SERVICE_REQ_STT_ID;

                            if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                tooltipText = "Chưa xử lý";
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                tooltipText = "Đã xử lý";
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                tooltipText = "Hoàn thành";
                            }

                            lastInfo = new ToolTipControlInfo(
                                new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")),
                                tooltipText);

                        }
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRation_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRation.Properties.Buttons.Count > 0) cboRation.Properties.Buttons[1].Visible = cboRation.EditValue != null;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboRation_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRation.EditValue = null;
                    cboRation.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtServiceReqCode.Text))
                {
                    txtServiceReqCode.Text = txtServiceReqCode.Text.PadLeft(12, '0');
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtSearchValue_EditValueChanged(object sender, EventArgs e)
        {

        }
    }
}
