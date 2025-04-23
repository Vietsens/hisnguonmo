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
using AutoMapper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestAggregate.ADO;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestAggregate
{
    public partial class UCExpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {
        #region Variable
        List<long> list_exp_mest_id;
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int rowCount = 0;
        int dataTotal = 0;
        long aggrExpMestId;
        int rowCountExpM = 0;
        int dataTotalExpM = 0;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_EXP_MEST currentAggrExpMest { get; set; }
        List<V_HIS_BED> listBed;
        List<V_HIS_BED> bedSelecteds;
        List<long> _lstCurrentBedId = new List<long>();
        List<long> lstRoomSelectedId = new List<long>();
        List<long> lstBedRoomIds = new List<long>();

        bool isCheckAll = true;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        string moduleLink = "HIS.Desktop.Plugins.AggrExpMestDetail";
        private bool isNotLoadWhileChangeControlStateInFirst;
        public List<SereServADO> listSereServeAll; //luu tat ca thuoc va vat tu
        public List<SereServADO> listSereServeMedi; //luu tat thuoc
        public List<SereServADO> listSereServeMate; //luu tat vat tu
        #endregion

        public UCExpMestAggregate()
        {
            InitializeComponent();
        }

        public UCExpMestAggregate(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExpMestAggregate_Load(object sender, EventArgs e)
        {
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                HisConfigCFG.LoadConfig();
                isNotLoadWhileChangeControlStateInFirst = true;
                SetCaptionByLanguageKey();

                this.gridControlAggrExpMest.ToolTipController = this.toolTipController1;
                this.gridControlAggregateRequest.ToolTipController = this.toolTipController1;
                this.gridControlExpMestReq.ToolTipController = this.toolTipController1;
                chkNotSynthetic.Checked = true;
                chkSynthesized.Checked = false;

                InitControlState();
                InitMediStockCheck();
                InitComboMediStock();
                InitcboBed();
                InitCboPatientType();
                SetDataDefault();
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCboPatientType()
        {
            try
            {
                List<HIS_PATIENT_TYPE> patientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>()
                  .Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientType, patientTypes, controlEditorADO);
                cboPatientType.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitcboBed()
        {
            try
            {
                cboBed.Properties.View.Columns.Clear();
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboBed.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboBed);
                cboBed.Properties.Tag = gridCheck;
                cboBed.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisBedViewFilter filter = new HisBedViewFilter();
                if (lstRoomSelectedId != null && lstRoomSelectedId.Count() > 0)
                {
                    filter.BED_ROOM_IDs = lstBedRoomIds.Distinct().ToList();
                }
                else
                {
                    filter.DEPARTMENT_ID = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                }

                var data = new BackendAdapter(param).Get<List<V_HIS_BED>>("api/HisBed/GetView", ApiConsumers.MosConsumer, filter, param);
                listBed = new List<V_HIS_BED>();
                listBed = data;
                if (listBed != null)
                {

                    cboBed.Properties.DataSource = listBed;
                    cboBed.Properties.DisplayMember = "BED_NAME";
                    cboBed.Properties.ValueMember = "ID";

                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboBed.Properties.View.Columns.AddField("BED_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 50;
                    col2.Caption = " ";

                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboBed.Properties.View.Columns.AddField("BED_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = Resources.ResourceMessage.TatCa;

                    cboBed.Properties.PopupFormWidth = 400;
                    cboBed.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboBed.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboBed.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboBed.Properties.View);
                    }
                }
                layoutControlItem28.Control.Controls.Add(cboBed);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboBed(object sender, EventArgs e)
        {
            try
            {
                _lstCurrentBedId = new List<long>();
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                bedSelecteds = new List<V_HIS_BED>();
                if (gridCheckMark != null)
                {
                    List<V_HIS_BED> erSelectedNews = new List<V_HIS_BED>();
                    foreach (V_HIS_BED er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.BED_NAME + ",";
                        _lstCurrentBedId.Add(er.ID);

                    }
                    cboBed.Text = typeName;
                    cboBed.ToolTip = typeName;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Du lieu mac dinh
        /// </summary>
        private void SetDataDefault()
        {
            try
            {

                SetDefaultValue();

                FillDataNavListRoom();
                CreateThreadLoadData();
                FillDataToGridControlExpMestReq();
                LoadDataAggrExpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData()
        {
            Thread threadMetyReq = new System.Threading.Thread(LoadDataExpMestThuocNT);
            Thread threadMatyReq = new System.Threading.Thread(PagingAggrExpMest);

            threadMetyReq.Priority = ThreadPriority.Normal;
            threadMatyReq.Priority = ThreadPriority.Normal;
            try
            {
                threadMetyReq.Start();
                threadMatyReq.Start();

                threadMetyReq.Join();
                threadMatyReq.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                threadMatyReq.Abort();
            }
        }

        private void LoadDataExpMestThuocNTNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataExpMestThuocNT(); }));
                }
                else
                {
                    LoadDataExpMestThuocNT();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataAggrExpMestNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { LoadDataAggrExpMest(); }));
                }
                else
                {
                    LoadDataAggrExpMest();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtKeywordProcess.Text = "";
                // Load thoi gian mac dinh len control datetime
                dtFromIntructionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtToIntructionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;

                checkPresNormal.Checked = true;
                checkIHomePres.Checked = true;
                checkPresKidney.Checked = false;
                chkNotSynthetic.Checked = true;
                chkSynthesized.Checked = false;
                gridControlAggrExpMest.DataSource = null;
                gridControlDetail.DataSource = null;
                gridControlExpMestReq.DataSource = null;
                gridControlAggregateRequest.DataSource = null;
                cboBed.EditValue = null;

                GridCheckMarksSelection gridCheckMarkPart = cboBed.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkPart.ClearSelection(cboBed.Properties.View);
                cboBed.Text = "";

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExpMestAggregate.Resources.Lang", typeof(UCExpMestAggregate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkThuocVTTheoPhieuXuat.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.chkThuocVTTheoPhieuXuat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnInTraDoiTongHop.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.btnInTraDoiTongHop.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqRoomName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqUserTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqUserTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqUserName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpReqPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPrint.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.chkPrint.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpDelete.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpDelete.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPrint.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPrint.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpPrint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpStockName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpCreator.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpTimeDisplay.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpTimeDisplay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpUserName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediSTT.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediTypeCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediTypeName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediUnitName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediAmount.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediBidNumber.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColMediBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColEditPres.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.gridColEditPres.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColDelete.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.gridColDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExecuteRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReqUserName.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAggrExpMest.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.btnAggrExpMest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarFilterProcess.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navBarFilterProcess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFromIntructionTime.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciFromIntructionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciToIntructionTime.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciToIntructionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotSynthetic.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.chkNotSynthetic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSynthesized.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.chkSynthesized.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckSynthesized.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciCheckSynthesized.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckNotSynthetic.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.lciCheckNotSynthetic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl14.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkIHomePres.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.checkIHomePres.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkPresKidney.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.checkPresKidney.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkPresNormal.Properties.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.checkPresNormal.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl16.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBed.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.cboBed.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl17.Text = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.layoutControl17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupMediStock.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navBarGroupMediStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroup1.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navBarGroup1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navStatus.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navRoom.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupTypePress.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navBarGroupTypePress.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBed.Caption = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.navBed.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeywordProcess.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExpMestAggregate.txtKeywordProcess.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_EXP_MEST data = (V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();

                        //Status
                        long expMestSttId = data.EXP_MEST_STT_ID;
                        //trang: dang gui YC : màu vàng
                        if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu xanh
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet mau den
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else if (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestReq_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (chkThuocVTTheoPhieuXuat.Checked)
                {
                    gridControlDetail.DataSource = null;
                    if (e.Column.FieldName == "DX$CheckboxSelectorColumn" || e.Column.FieldName == "EDIT")
                    {
                        return;
                    }
                    var row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridViewExpMestReq.GetFocusedRow();
                    if (row != null)
                    {
                        List<long> listExpMestIds = new List<long> { row.ID };
                        CreateThreadLoadDataDetailExpMest(listExpMestIds);
                        listSereServeAll = new List<SereServADO>();
                        listSereServeAll.AddRange(listSereServeMedi);
                        listSereServeAll.AddRange(listSereServeMate);

                        gridControlDetail.DataSource = listSereServeAll;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tổng hợp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAggrExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                SelectCheckExpMestAggr();
                ExecuteAggrByListPres();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        List<V_HIS_EXP_MEST> _ExpMestChecks { get; set; }
        /// <summary>
        /// Select Check 
        /// </summary>
        private void SelectCheckExpMestAggr()
        {
            try
            {
                _ExpMestChecks = new List<V_HIS_EXP_MEST>();
                if (this._ExpMestADOs != null && this._ExpMestADOs.Count > 0)
                {
                    _ExpMestChecks.AddRange(this._ExpMestADOs.Where(p => p.IsCheck == true && p.AGGR_EXP_MEST_ID == null && p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tong hop
        /// </summary>
        private void ExecuteAggrByListPres()
        {
            try
            {
                this.list_exp_mest_id = new List<long>();
                //Review
                if (this._ExpMestChecks != null && this._ExpMestChecks.Count > 0)
                {
                    this.list_exp_mest_id = this._ExpMestChecks.Select(p => p.ID).ToList();
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.BanChuaChonDichVu, Resources.ResourceMessage.ThongBao);
                    return;
                }
                if (CheckOld())
                    DelegateReturnSuccess(true);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Start Tong Hop
        /// </summary>
        /// <param name="data"></param>
        public void DelegateReturnSuccess(object data)
        {
            try
            {
                //Review
                bool success = false;
                CommonParam param = new CommonParam();
                if (data is long)
                    this.list_exp_mest_id.Add((long)data);
                MOS.SDO.HisExpMestAggrSDO expMestSdo = new MOS.SDO.HisExpMestAggrSDO();
                expMestSdo.ExpMestIds = this.list_exp_mest_id;
                expMestSdo.ReqRoomId = this.currentModule.RoomId;
                var hisAggrExpMestCreate = new BackendAdapter(param).Post<List<HIS_EXP_MEST>>("api/HisExpMest/AggrCreate", ApiConsumers.MosConsumer, expMestSdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (hisAggrExpMestCreate != null)
                {
                    success = true;
                    CreateThreadLoadData();
                    FillDataToGridControlExpMestReq();
                    LoadDataAggrExpMest();
                }
                else
                    success = false;
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckOld()
        {
            bool success = true;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter _medicineFilter = new HisExpMestMedicineViewFilter();
                _medicineFilter.EXP_MEST_IDs = this.list_exp_mest_id;
                var dataMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/getView", ApiConsumers.MosConsumer, _medicineFilter, param);

                List<V_HIS_EXP_MEST_MEDICINE> _MedicineOld = new List<V_HIS_EXP_MEST_MEDICINE>();

                if (dataMedicines != null && dataMedicines.Count > 0)
                {
                    var dataMediGroups = dataMedicines.GroupBy(p => new { p.EXP_MEST_ID, p.MEDICINE_TYPE_ID }).Select(p => p.ToList()).ToList();
                    //Ktra xem có lẻ hay k?
                    foreach (var item in dataMediGroups)
                    {
                        V_HIS_EXP_MEST_MEDICINE ado = new V_HIS_EXP_MEST_MEDICINE();
                        ado = item.FirstOrDefault();
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        decimal x = Math.Abs(Math.Round(ado.AMOUNT, 3) - Math.Floor(ado.AMOUNT));
                        if (x > 0)
                        {
                            //x : sl lẻ 1 -x
                            _MedicineOld.Add(ado);
                        }
                    }
                }
                if (_MedicineOld != null && _MedicineOld.Count > 0)
                {
                    var dataGroups = _MedicineOld.GroupBy(p => p.EXP_MEST_ID).Select(p => p.ToList()).ToList();
                    string pressMessages = "";
                    if (dataGroups.Count == 1)
                    {
                        pressMessages += string.Format(Resources.ResourceMessage.PhieuCoCacThuocSauCoSoLuongLe, dataGroups[0][0].EXP_MEST_CODE);
                        foreach (var item in dataGroups[0])
                        {
                            pressMessages += " - " + item.MEDICINE_TYPE_NAME + "(" + item.AMOUNT + ")" + "\r\n";
                        }
                    }
                    else
                    {
                        pressMessages += Resources.ResourceMessage.CacThuocSauCoSoLuongLe;
                        foreach (var itemGr in dataGroups)
                        {
                            List<string> _mess = new List<string>();

                            for (int i = 0; i < itemGr.Count; i++)
                            {
                                _mess.Add(itemGr[i].MEDICINE_TYPE_NAME + "(" + itemGr[i].AMOUNT + ")");
                            }
                            pressMessages += string.Format(Resources.ResourceMessage.Phieu, itemGr[0].EXP_MEST_CODE, string.Join(",", _mess));
                        }
                    }

                    if (DevExpress.XtraEditors.XtraMessageBox.Show(pressMessages + Resources.ResourceMessage.BanCoMuonTiepTuc, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        success = false;
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private void gridControlAggrExpMest_Click(object sender, EventArgs e)
        {
            try
            {
                //Review
                gridControlAggregateRequest.DataSource = null;
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)gridViewAggrExpMest.GetFocusedRow();
                this.aggrExpMestId = row.ID;
                if (this.aggrExpMestId > 0)
                {
                    LoadDetailAggrExpMestByAggrExpMestId();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tim Kiem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                CreateThreadLoadData();
                FillDataToGridControlExpMestReq();
                LoadDataAggrExpMest();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Lam Lai
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDataDefault(); // Du lieu Grid yeu cau
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewDetail_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.Column == grdColMediSTT)
                {
                    e.DisplayText = Convert.ToString(e.RowHandle + 1);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExpMestReq)
                {
                    ToolTipDetail(gridControlExpMestReq, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlAggrExpMest)
                {
                    ToolTipDetail(gridControlAggrExpMest, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlAggregateRequest)
                {
                    ToolTipDetail(gridControlAggregateRequest, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ToolTipDetail(DevExpress.XtraGrid.GridControl gridControl, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandle != info.RowHandle)
                    {
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        if (info.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                        {
                            text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                        }
                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordProcess_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    LoadDataExpMestThuocNTNewThread();
                    FillDataToGridControlExpMestReq();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewAggrExpMest_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                //Review
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXP_MEST_STT_DISPLAY")
                    {
                        //Status
                        //trang: dang gui YC : màu vàng
                        if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu đỏ
                        else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet : mau den
                        else if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                    }

                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrExpMest_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //Review
                    var data = (V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(e.RowHandle);
                    var departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                    var roomId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).RoomId;
                    long expMestSttId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewAggrExpMest.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long reqRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewAggrExpMest.GetRowCellValue(e.RowHandle, "REQ_ROOM_ID") ?? "").ToString());
                    string creator = (gridViewAggrExpMest.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "DELETE_DISPLAY")
                    {
                        if (data.REQ_DEPARTMENT_ID == departmentId
                            && (expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT
                            || expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT
                            || expMestSttId == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST))
                        {
                            if (HisConfigCFG.isEnableDeleteAggregate)
                            {
                                if ((CheckLoginAdmin.IsAdmin(loginName) || creator.Equals(loginName)) && reqRoomId == roomId)
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_Delete;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                                }
                            }
                            else
                                e.RepositoryItem = repositoryItemButtonEdit_Delete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch12345()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh123456()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnAggrExpMest()
        {
            try
            {
                btnAggrExpMest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void f2KeyWordFocused()
        {
            try
            {
                txtKeywordProcess.Focus();
                txtKeywordProcess.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterSave(object prescription)
        {
            try
            {
                if (prescription != null)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestReq_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(e.RowHandle);
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (data != null)
                    {
                        if (e.Column.FieldName == "EDIT")
                        {
                            if (data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = repositoryItemButtonEdit__Pres;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit__Disable;
                            }
                        }
                        else if (e.Column.FieldName == "DELETE")
                        {
                            if (data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                e.RepositoryItem = repositoryItemButton__Delete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButton__Delete_Disable;
                            }
                        }
                        else if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (data.AGGR_EXP_MEST_ID == null || data.AGGR_EXP_MEST_ID <= 0))
                            {
                                e.RepositoryItem = repositoryItemCheck_E;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck_D;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewExpMestReq_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
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
                                var dataRow = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(hi.RowHandle);
                                if (dataRow != null && dataRow.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST && (dataRow.AGGR_EXP_MEST_ID == null || dataRow.AGGR_EXP_MEST_ID <= 0))
                                {
                                    checkEdit.Checked = !checkEdit.Checked;
                                    view.CloseEditor();
                                    if (this._ExpMestADOs != null && this._ExpMestADOs.Count > 0)
                                    {
                                        var dataChecks = this._ExpMestADOs.Where(p => p.IsCheck != true && p.AGGR_EXP_MEST_ID == null && p.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST).ToList();
                                        if (dataChecks != null && dataChecks.Count > 0)
                                        {
                                            gridColumnCheck.Image = imageListIcon.Images[6];
                                            isCheckAll = false;
                                        }
                                        else
                                        {
                                            gridColumnCheck.Image = imageListIcon.Images[5];
                                            isCheckAll = true;
                                        }
                                    }
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                        else if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit))
                        {
                            var data = (V_HIS_EXP_MEST)gridViewExpMestReq.GetRow(hi.RowHandle);
                            string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                            if (data != null && hi.HitTest == GridHitTest.RowCell && data.CREATOR == loginName && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                if (hi.Column.FieldName == "DELETE")
                                {
                                    DeleteServiceReq(data);
                                }
                                else if (hi.Column.FieldName == "EDIT")
                                {
                                    ShowAssignPrescription(data);
                                }
                            }
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewExpMestReq.BeginUpdate();
                            if (this._ExpMestADOs == null)
                                this._ExpMestADOs = new List<ExpMestADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._ExpMestADOs)
                                {
                                    item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._ExpMestADOs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewExpMestReq.EndUpdate();
                            this.LoadExpMestDetail();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Sua Yeu Cau Don Thuoc
        /// </summary>
        /// <param name="data"></param>
        private void ShowAssignPrescription(V_HIS_EXP_MEST data)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AssignPrescriptionPK").FirstOrDefault();
                if (moduleData == null)
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AssignPrescriptionPK");
                else if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    HIS.Desktop.ADO.AssignPrescriptionADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionADO(data.TDL_TREATMENT_ID ?? 0, 0, 0);
                    assignServiceADO.PatientDob = data.TDL_PATIENT_DOB ?? 0;
                    assignServiceADO.PatientName = data.TDL_PATIENT_NAME;
                    assignServiceADO.GenderName = data.TDL_PATIENT_GENDER_NAME;
                    assignServiceADO.TreatmentCode = data.TDL_TREATMENT_CODE;
                    assignServiceADO.TreatmentId = data.TDL_TREATMENT_ID ?? 0;

                    MOS.Filter.HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.ID = data.SERVICE_REQ_ID;
                    var resultServiceReqs = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumer.ApiConsumers.MosConsumer, reqFilter, null);
                    HIS_SERVICE_REQ _serviceReq = new HIS_SERVICE_REQ();
                    if (resultServiceReqs != null && resultServiceReqs.Count > 0)
                    {
                        _serviceReq = resultServiceReqs.FirstOrDefault();
                    }
                    MOS.EFMODEL.DataModels.HIS_EXP_MEST _expMes = new HIS_EXP_MEST();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST>(_expMes, data);
                    AssignPrescriptionEditADO assignEditADO = assignEditADO = new AssignPrescriptionEditADO(_serviceReq, _expMes, FillDataApterSave);

                    assignServiceADO.AssignPrescriptionEditADO = assignEditADO;

                    listArgs.Add(assignServiceADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, currentModule.RoomId, currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xoa Yeu Cau Don Thuoc
        /// </summary>
        private void DeleteServiceReq(V_HIS_EXP_MEST data)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                //Review
                if (MessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    MOS.SDO.HisServiceReqSDO sdo = new MOS.SDO.HisServiceReqSDO();
                    sdo.Id = data.SERVICE_REQ_ID;
                    sdo.RequestRoomId = this.currentModule.RoomId;
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisServiceReq/Delete", ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (success)
                    {
                        btnSearch_Click(null, null);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    dtFromIntructionTime.Focus();
                    dtFromIntructionTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExpMestReq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewExpMestReq.RowCount > 0 && gridViewExpMestReq.SelectedRowsCount > 0)
                {
                    btnAggrExpMest.Enabled = true;
                }
                else
                {
                    btnAggrExpMest.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrExpMest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (gridViewAggrExpMest.RowCount > 0 && gridViewAggrExpMest.SelectedRowsCount > 0)
                {
                    btnInTraDoiTongHop.Enabled = true;
                }
                else
                {
                    btnInTraDoiTongHop.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnInTraDoiTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_EXP_MEST> _ExpMestTraDoiChecks = new List<V_HIS_EXP_MEST>();
                if (gridViewAggrExpMest.RowCount > 0)
                {
                    for (int i = 0; i < gridViewAggrExpMest.SelectedRowsCount; i++)
                    {
                        if (gridViewAggrExpMest.GetSelectedRows()[i] >= 0)
                        {
                            _ExpMestTraDoiChecks.Add((V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(gridViewAggrExpMest.GetSelectedRows()[i]));
                        }
                    }
                }
                if (_ExpMestTraDoiChecks != null && _ExpMestTraDoiChecks.Count > 0)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrExpMestPrintFilter").FirstOrDefault();
                    if (moduleData == null)
                        Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrExpMestPrintFilter");
                    else if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(_ExpMestTraDoiChecks);
                        listArgs.Add((long)5);
                        int[] selectRows = gridViewAggrExpMest.GetSelectedRows();
                        if (selectRows != null && selectRows.Count() > 0)
                        {
                            if (chkPrint.Checked)
                            {
                                HIS.Desktop.ADO.AggrExpMestPrintSDO sdo = new HIS.Desktop.ADO.AggrExpMestPrintSDO();
                                sdo.PrintNow = true;
                                listArgs.Add(sdo);
                            }
                        }
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        if (extenceInstance.GetType() == typeof(bool))
                        {
                            return;
                        }
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrExpMest_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        var data = (V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(hi.RowHandle);
                        string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                        if (hi.Column.FieldName == "DELETE_DISPLAY")
                        {
                            #region ----- DELETE_DISPLAY -----
                            if (data != null
                        && hi.HitTest == GridHitTest.RowCell
                        && data.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST)
                            {
                                try
                                {
                                    CommonParam param = new CommonParam();
                                    bool success = false;

                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    {
                                        var result = new BackendAdapter(param).Post<bool>("/api/HisExpMest/AggrDelete", ApiConsumers.MosConsumer, data.ID, param);
                                        if (result)
                                        {
                                            success = true;
                                            gridControlDetail.DataSource = null;
                                            gridControlAggregateRequest.DataSource = null;
                                            CreateThreadLoadData();
                                            FillDataToGridControlExpMestReq();
                                            LoadDataAggrExpMest();
                                        }
                                        WaitingManager.Hide();
                                        MessageManager.Show(this.ParentForm, param, success);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WaitingManager.Hide();
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            #endregion
                        }
                        else if (hi.Column.FieldName == "PRINT_DISPLAY")
                        {
                            PrintAggregateExpMest(data);
                        }
                        else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                        {
                            DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                            barManager1.Form = this;
                            this.currentAggrExpMest = data;

                            List<V_HIS_EXP_MEST> _ExpMestChecks = new List<V_HIS_EXP_MEST>();
                            if (gridViewAggrExpMest.RowCount > 0)
                            {
                                for (int i = 0; i < gridViewAggrExpMest.SelectedRowsCount; i++)
                                {
                                    if (gridViewAggrExpMest.GetSelectedRows()[i] >= 0)
                                    {
                                        _ExpMestChecks.Add((V_HIS_EXP_MEST)gridViewAggrExpMest.GetRow(gridViewAggrExpMest.GetSelectedRows()[i]));
                                    }
                                }
                            }

                            ExpMestAggregateListPopupMenuProcessor processor = new ExpMestAggregateListPopupMenuProcessor(this.currentAggrExpMest, _ExpMestChecks, ExpMestAggregateMouseRightClick, barManager1);
                            processor.InitMenu();
                        }

                        // }
                    }
                }
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
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPrint.Name)
                        {
                            chkPrint.Checked = item.VALUE == "1";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrint_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrint.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrint.Name;
                    csAddOrUpdate.VALUE = (chkPrint.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
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

        private void cboBed_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (V_HIS_BED hb in gridCheckMark.Selection)
                {
                    if (hb != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(hb.BED_NAME.ToString());
                    }
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkPresNormal_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void repositoryItemCheck_E_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                gridViewExpMestReq.PostEditor();
                ExpMestADO row = (ExpMestADO)gridViewExpMestReq.GetFocusedRow();
                if (row != null)
                {
                    this.LoadExpMestDetail();
                }
                gridControlExpMestReq.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExpMestDetail()
        {
            try
            {
                WaitingManager.Show();
                LogSystem.Info("LoadExpMestDetail. Begin Process");
                List<ExpMestADO> dataChecks = new List<ExpMestADO>();
                if (this._ExpMestADOs != null && this._ExpMestADOs.Count > 0)
                {
                    dataChecks = this._ExpMestADOs.Where(p => p.IsCheck == true).ToList();
                }
                List<V_HIS_EXP_MEST> sellectedrows = new List<V_HIS_EXP_MEST>();
                List<SereServADO> listDetail = new List<SereServADO>();

                if (dataChecks != null)
                {
                    List<long> listExpMestIds = dataChecks.Select(o => o.ID).ToList();
                    CreateThreadLoadDataDetailExpMest(listExpMestIds);
                    if (listSereServeMedi != null && listSereServeMedi.Count > 0)
                        listDetail.AddRange(listSereServeMedi);
                    if (listSereServeMate != null && listSereServeMate.Count > 0)
                        listDetail.AddRange(listSereServeMate);
                }
                gridControlDetail.DataSource = listDetail;
                gridControlDetail.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboPatientType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue != null)
                    cboPatientType.Properties.Buttons[1].Visible = true;
                else
                    cboPatientType.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThuocVTTheoPhieuXuat_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkThuocVTTheoPhieuXuat.Checked)
                {
                    LoadExpMestDetail();
                }
                else
                {
                    gridControlDetail.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadLoadDataDetailExpMest(List<long> _listExpMestIDs)
        {
            Thread threadMetyReq = new System.Threading.Thread(new ParameterizedThreadStart(LoadDataMediDetailExpMest));
            Thread threadMatyReq = new System.Threading.Thread(new ParameterizedThreadStart(LoadDataMateDetailExpMest));

            threadMetyReq.Priority = ThreadPriority.Normal;
            threadMatyReq.Priority = ThreadPriority.Normal;
            try
            {
                threadMetyReq.Start(_listExpMestIDs);
                threadMatyReq.Start(_listExpMestIDs);

                threadMetyReq.Join();
                threadMatyReq.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                threadMatyReq.Abort();
            }
        }


        void LoadDataMediDetailExpMest(object _listExpMestIDs)
        {
           
            try
            {
                listSereServeMedi = new List<SereServADO>();
                CommonParam param = new CommonParam();
                List<V_HIS_EXP_MEST_MEDICINE> _expMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = _listExpMestIDs as List<long>;
                _expMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, param);
                if (_expMedicines != null && _expMedicines.Count > 0)
                {
                    var dataGroups = _expMedicines.GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    var _meidicneTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                    foreach (var item in dataGroups)
                    {
                        var dataType = _meidicneTypes.FirstOrDefault(p => p.ID == item[0].MEDICINE_TYPE_ID);
                        if (dataType == null)
                        {
                            continue;
                        }

                        SereServADO ado = new SereServADO();
                        foreach (var itemNumber in item)
                        {
                            ado.PACKAGE_NUMBER += (!String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? "; " + itemNumber.PACKAGE_NUMBER : "");
                        }

                        ado.AMOUNT = item.Sum(p => p.AMOUNT);

                        ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = dataType.MEDICINE_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = dataType.MEDICINE_TYPE_NAME;
                        ado.CONCENTRA = dataType.CONCENTRA;
                        listSereServeMedi.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void LoadDataMateDetailExpMest(object _listExpMestIDs)
        {
            try
            {
                listSereServeMate = new List<SereServADO>();
                CommonParam param = new CommonParam();
                List<V_HIS_EXP_MEST_MATERIAL> _expMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                MOS.Filter.HisExpMestMaterialViewFilter materialFilter = new HisExpMestMaterialViewFilter();
                materialFilter.EXP_MEST_IDs = _listExpMestIDs as List<long>;
                _expMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, materialFilter, param);
                if (_expMaterials != null && _expMaterials.Count > 0)
                {
                    var dataGroups = _expMaterials.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                    var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    foreach (var item in dataGroups)
                    {
                        var dataType = _materialTypes.FirstOrDefault(p => p.ID == item[0].MATERIAL_TYPE_ID);
                        if (dataType == null)
                        {
                            continue;
                        }
                        SereServADO ado = new SereServADO();
                        foreach (var itemNumber in item)
                        {
                            ado.PACKAGE_NUMBER += !String.IsNullOrEmpty(itemNumber.PACKAGE_NUMBER) ? "; " + itemNumber.PACKAGE_NUMBER : "";
                        }
                        ado.AMOUNT = item.Sum(p => p.AMOUNT);
                        ado.PACKAGE_NUMBER = "";
                        ado.SERVICE_UNIT_NAME = dataType.SERVICE_UNIT_NAME;
                        ado.TDL_SERVICE_CODE = dataType.MATERIAL_TYPE_CODE;
                        ado.TDL_SERVICE_NAME = dataType.MATERIAL_TYPE_NAME;
                        
                        listSereServeMate.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
