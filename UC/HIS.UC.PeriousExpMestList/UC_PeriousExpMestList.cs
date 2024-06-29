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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.UC.PeriousExpMestList.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Columns;
using System.ComponentModel;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.UC.PeriousExpMestList.Run
{
    public partial class UC_PeriousExpMestList : UserControl
    {
        #region Declare

        LanguageInputADO languageInputADO;
        PeriousExpMestInitADO DepositRequestInitADO = new PeriousExpMestInitADO();
        private HisTreatmentWithPatientTypeInfoSDO Treatment { get; set; }
        private PreServiceReqADO ExpMest { get; set; }
        private List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> ListServiceReqs { get; set; }
        private List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7> ListServiceReqsAll { get; set; }
        private List<HIS.UC.PeriousExpMestList.ADO.PreServiceReqADO> PreServiceReqADOs = null;
        gridviewHandler grv_btnView_Click;
        gridviewHandlerList grv_btnSelected_Click;
        gridviewHandler updateSingleRow;
        long patientId { get; set; }
        long? expMestTypeId { get; set; }
        bool? isAutoWidth { get; set; }
        int width { get; set; }
        int height { get; set; }
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        List<FilterADO> filterADOs;
        MOS.Filter.HisServiceReqView7Filter currentPrescriptionFilter;
        MOS.Filter.HisServiceReqView7Filter serviceReqView7Filter;
        bool notProcessWhileNotShowExpMestTypeDTTCheckedChanged = false;
        Action<bool> actNotShowExpMestTypeDTTCheckedChanged;

        string vlShowingOldPrescriptionOption;
        bool isPresPK;
        bool IsOptionTime { get; set; }
        long? InFrom { get; set; }
        long? InTo { get; set; }
        #endregion

        #region Contructor - Load
        public UC_PeriousExpMestList()
            : this(null)
        {
        }

        public UC_PeriousExpMestList(PeriousExpMestInitADO data)
        {
            InitializeComponent();
            try
            {
                if (data != null)
                {
                    this.Treatment = data.Treatment;
                    this.grv_btnView_Click = data.btnView_Click;
                    this.grv_btnSelected_Click = data.btnSelected_Click;
                    this.updateSingleRow = data.UpdateSingleRow;
                    this.languageInputADO = data.LanguageInputADO;
                    this.isAutoWidth = data.IsAutoWidth;
                    this.expMestTypeId = data.ExpMestTypeId;
                    this.isPresPK = (data.IsPresPK.HasValue && data.IsPresPK.Value);
                    this.currentPrescriptionFilter = data.ServiceReqView7Filter;
                    this.notProcessWhileNotShowExpMestTypeDTTCheckedChanged = true;
                    if (data.IsNotShowExpMestTypeDTT.HasValue)
                    {
                        chkNotShowExpMestTypeDTT.Checked = data.IsNotShowExpMestTypeDTT.Value;
                    }
                    else
                        chkNotShowExpMestTypeDTT.Checked = true;
                    this.actNotShowExpMestTypeDTTCheckedChanged = data.ActNotShowExpMestTypeDTTCheckedChanged;
                    this.notProcessWhileNotShowExpMestTypeDTTCheckedChanged = false;
                }
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
                //gcolShowPrescriptionPrevious.ToolTip = this.languageInputADO.btnShowPrescriptionPrevious__ToolTip;
                //gcolSelectPrescriptionPrevious.ToolTip = this.languageInputADO.btnSelectPrescriptionPrevious__ToolTip;
                //gcolIntructionTime.Caption = this.languageInputADO.gridControlPreviousprescription__gcolIntructionTime__Caption;
                //gcolIntructionUser.Caption = this.languageInputADO.gridControlPreviousprescription__gcolIntructionUser__Caption;

                lblPresTitle.Text = this.languageInputADO.lciPrePrescription__Text;
                layoutControlItem4.Text = this.languageInputADO.lciCheckBox__Text;
                treeListColumnShow.ToolTip = this.languageInputADO.btnShowPrescriptionPrevious__ToolTip;
                treeListColumnSelect.ToolTip = this.languageInputADO.btnSelectPrescriptionPrevious__ToolTip;
                treeListColumnShow.Caption = " ";
                treeListColumnSelect.Caption = " ";
                treeListColumnInstructionTime.Caption = this.languageInputADO.gridControlPreviousprescription__gcolIntructionTime__Caption;
                treeListColumnUser.Caption = this.languageInputADO.gridControlPreviousprescription__gcolIntructionUser__Caption;
                treeListColumn4.Caption = this.languageInputADO.gridControlPreviousprescription__gcol4__Caption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_DepositRequestList_Load(object sender, EventArgs e)
        {
            try
            {
                this.vlShowingOldPrescriptionOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AssignPrescriptionPK.ShowingOldPrescriptionOption");

                this.treeListServiceReq.OptionsView.AutoWidth = false;
                this.treeListServiceReq.ToolTipController = this.toolTipController1;
                this.treeListServiceReq.KeyFieldName = "ID_NODE";
                this.treeListServiceReq.ParentFieldName = "PARENT_ID_NODE";

                this.InitComboFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private async Task LoadDataToGridControl()
        {
            try
            {
                if (cboFilter.EditValue != null && (string)cboFilter.EditValue == "0")
                    IsOptionTime = true;
                GetServiceReq7List(new CommonParam(), true);
                int prescriptionOldLimit = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__OLD_PRECRIPTIONS_DISPLAY_LIMIT");
                if (prescriptionOldLimit > 0 && !(this.vlShowingOldPrescriptionOption == "1" && this.isPresPK))
                {
                    if (currentPrescriptionFilter.PRESCRIPTION_TYPE_ID == 1)
                        ListServiceReqs = ListServiceReqsAll.Where(o => o.PRESCRIPTION_TYPE_ID == 1).Take(prescriptionOldLimit).ToList();
                    else if (currentPrescriptionFilter.PRESCRIPTION_TYPE_ID == 2)
                        ListServiceReqs = ListServiceReqsAll.Where(o => o.PRESCRIPTION_TYPE_ID == 2).Take(prescriptionOldLimit).ToList();
                }
                else
                {
                    if (currentPrescriptionFilter.PRESCRIPTION_TYPE_ID == 1)
                        ListServiceReqs = ListServiceReqsAll.Where(o => o.PRESCRIPTION_TYPE_ID == 1).ToList();
                    else if (currentPrescriptionFilter.PRESCRIPTION_TYPE_ID == 2)
                        ListServiceReqs = ListServiceReqsAll.Where(o => o.PRESCRIPTION_TYPE_ID == 2).ToList();
                }
                Reload(this.ListServiceReqs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboFilter()
        {
            try
            {
                this.filterADOs = new List<FilterADO>();
                this.filterADOs.Add(new FilterADO() { CODE = "1", NAME = this.languageInputADO.cboItem1_Text });
                this.filterADOs.Add(new FilterADO() { CODE = "2", NAME = this.languageInputADO.cboItem2_Text });
                this.filterADOs.Add(new FilterADO() { CODE = "3", NAME = this.languageInputADO.cboItem3_Text });
                this.filterADOs.Add(new FilterADO() { CODE = "0", NAME = this.languageInputADO.cboItem4_Text });

                List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                columnInfos.Add(new Inventec.Common.Controls.EditorLoader.ColumnInfo("NAME", "", 150, 1));
                Inventec.Common.Controls.EditorLoader.ControlEditorADO controlEditorADO = new Inventec.Common.Controls.EditorLoader.ControlEditorADO("NAME", "CODE", columnInfos, false, 150);
                Inventec.Common.Controls.EditorLoader.ControlEditorLoader.Load(this.cboFilter, this.filterADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event
        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl is DevExpress.XtraTreeList.TreeList)
                {
                    string toolTip = "";
                    TreeList tree = (TreeList)e.SelectedControl;
                    TreeListHitInfo hit = tree.CalcHitInfo(e.ControlMousePosition);
                    if (hit.HitInfoType == HitInfoType.Cell && (hit.Column.FieldName == "RENDERER_EXP_TIME" || hit.Column.FieldName == "CREATOR_DISPLAY" || hit.Column.FieldName == "MEDI_STOCK_CODE"))
                    {
                        string format = "{0}: {1} - {2}.\r\n{3}: {4} - {5}";
                        string format1 = "{0}\r\n{1} {2}";
                        object cellInfo = new TreeListCellToolTipInfo(hit.Node, hit.Column, null);
                        var data = tree.GetDataRecordByNode(hit.Node) as PreServiceReqADO;
                        if (data != null)
                        {
                            if (String.IsNullOrEmpty(data.PARENT_ID_NODE))
                            {
                                toolTip = string.Format(format, this.languageInputADO.BenhChinh__Text, data.ICD_CODE, data.ICD_NAME, this.languageInputADO.BenhPhu__Text, data.ICD_SUB_CODE, data.ICD_TEXT);
                            }
                            else
                            {
                                toolTip = string.Format(format1, data.NAME, data.AMOUNT, data.SERVICE_UNIT_NAME);
                            }
                        }
                        e.Info = new DevExpress.Utils.ToolTipControlInfo(cellInfo, toolTip);
                    }
                }
                else if (e.Info == null && e.SelectedControl is Inventec.Desktop.CustomControl.MyGridControl)
                {
                    string text = "";
                    Inventec.Desktop.CustomControl.MyGridControl myGridControl = (Inventec.Desktop.CustomControl.MyGridControl)e.SelectedControl;
                    DevExpress.XtraGrid.Views.Grid.GridView view = myGridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)
                        {
                            this.lastColumn = info.Column;
                            this.lastRowHandle = info.RowHandle;

                            string format = "{0}: {1} - {2}.\r\n{3}: {4} - {5}";
                            text = String.Format(format, this.languageInputADO.BenhChinh__Text, (view.GetRowCellValue(this.lastRowHandle, "ICD_CODE") ?? "").ToString(), (view.GetRowCellValue(this.lastRowHandle, "ICD_NAME") ?? "").ToString(), this.languageInputADO.BenhPhu__Text, (view.GetRowCellValue(this.lastRowHandle, "ICD_SUB_CODE") ?? "").ToString(), (view.GetRowCellValue(this.lastRowHandle, "ICD_TEXT") ?? "").ToString());
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Tree
        private void treeListServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Row != null)
                {
                    PreServiceReqADO currentRow = e.Row as PreServiceReqADO;
                    if (currentRow != null)
                    {
                        if (e.Column.FieldName == "RENDERER_EXP_TIME")
                        {
                            if (String.IsNullOrEmpty(currentRow.PARENT_ID_NODE))
                            {
                                e.Value = String.Format("{0} {1} - {2} - {3}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(currentRow.INTRUCTION_TIME), currentRow.REQUEST_ROOM_NAME, currentRow.REQUEST_LOGINNAME, currentRow.REQUEST_USERNAME);
                            }
                            else if (currentRow.IS_PARENT_1)
                            {
                                e.Value = String.Format("{0}", currentRow.SERVICE_REQ_CODE);
                            }
                            else
                            {
                                e.Value = String.Format("{0} - {1} {2}", currentRow.NAME, currentRow.AMOUNT, currentRow.SERVICE_UNIT_NAME);
                            }
                        }
                        else if (e.Column.FieldName == "CREATOR_DISPLAY")
                        {
                            e.Value = currentRow.REQUEST_LOGINNAME + " - " + currentRow.REQUEST_USERNAME;
                        }
                    }
                    else
                    {
                        e.Value = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeListServiceReq_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeListServiceReq.GetDataRecordByNode(e.Node) as PreServiceReqADO;

                if (data != null && e.Column.FieldName == "SELECT")
                {
                    if (String.IsNullOrEmpty(data.PARENT_ID_NODE))
                    {
                        e.RepositoryItem = repositoryItemButtonEditselect;
                    }
                    else if (data.IS_PARENT_1)
                    {
                        e.RepositoryItem = repositoryItemButtonEditselect;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemTextEditReadOnly;
                    }
                    //e.RepositoryItem = (String.IsNullOrEmpty(data.PARENT_ID_NODE) || data.IS_PARENT_1) ? repositoryItemButtonEditselect : repositoryItemTextEditReadOnly;
                }
                else if (data != null && e.Column.FieldName == "SHOW")
                {
                    e.RepositoryItem = (data.IS_PARENT_1) ? repositoryItemButtonEditView : repositoryItemTextEditReadOnly;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditselect_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = treeListServiceReq.GetDataRecordByNode(treeListServiceReq.FocusedNode) as PreServiceReqADO;
                if (data != null)
                {
                    List<V_HIS_SERVICE_REQ_7> lstServiceReq = new List<V_HIS_SERVICE_REQ_7>();
                    if (!String.IsNullOrEmpty(data.PARENT_ID_NODE))
                    {
                        V_HIS_SERVICE_REQ_7 serviceReq = new V_HIS_SERVICE_REQ_7();
                        AutoMapper.Mapper.CreateMap<PreServiceReqADO, V_HIS_SERVICE_REQ_7>();
                        serviceReq = AutoMapper.Mapper.Map<PreServiceReqADO, V_HIS_SERVICE_REQ_7>(data);

                        lstServiceReq.Add(serviceReq);
                    }
                    else
                    {
                        lstServiceReq = this.ListServiceReqs.Where(o => o.INTRUCTION_DATE == data.INTRUCTION_DATE).ToList();
                    }
                    grv_btnSelected_Click(lstServiceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditView_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var data = treeListServiceReq.GetDataRecordByNode(treeListServiceReq.FocusedNode) as PreServiceReqADO;
                if (data != null)
                {
                    V_HIS_SERVICE_REQ_7 serviceReq = new V_HIS_SERVICE_REQ_7();
                    AutoMapper.Mapper.CreateMap<PreServiceReqADO, V_HIS_SERVICE_REQ_7>();
                    serviceReq = AutoMapper.Mapper.Map<PreServiceReqADO, V_HIS_SERVICE_REQ_7>(data);

                    grv_btnView_Click(serviceReq);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void cboFilter_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboFilter.EditValue = null;
                    LoadDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFilter_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboFilter.Properties.Buttons[1].Visible = cboFilter.EditValue != null;
                if (cboFilter.EditValue != null)
                {
                    WaitingManager.Show();
                    IsOptionTime = false;
                    InFrom = null;
                    InTo = null;
                    GetServiceReq7List(new CommonParam());
                    Reload(this.ListServiceReqs);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetServiceReq7List(CommonParam param, bool IsCheckNotShowTT = false)
        {
            try
            {
                this.serviceReqView7Filter = new MOS.Filter.HisServiceReqView7Filter();
                if (IsOptionTime && InFrom.HasValue && InTo.HasValue)
                {
                    this.serviceReqView7Filter.INTRUCTION_TIME_FROM = InFrom;
                    this.serviceReqView7Filter.INTRUCTION_TIME_TO = InTo;
                }
                else if (cboFilter.EditValue != null)
                {
                    if ((string)cboFilter.EditValue == "1")
                    {
                        this.serviceReqView7Filter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.AddMonths(-1));
                        this.serviceReqView7Filter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else if ((string)cboFilter.EditValue == "2")
                    {
                        this.serviceReqView7Filter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.AddMonths(-2));
                        this.serviceReqView7Filter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else if ((string)cboFilter.EditValue == "3")
                    {
                        this.serviceReqView7Filter.INTRUCTION_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.AddMonths(-3));
                        this.serviceReqView7Filter.INTRUCTION_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else if ((string)cboFilter.EditValue == "0")
                    {
                        WaitingManager.Hide();
                        frmChooseDate frmChooseDate = new frmChooseDate(ProcessSelectDate);
                        frmChooseDate.ShowDialog();
                    }
                }
                this.serviceReqView7Filter.TDL_PATIENT_ID = (this.currentPrescriptionFilter != null && this.currentPrescriptionFilter.TDL_PATIENT_ID.HasValue) ? this.currentPrescriptionFilter.TDL_PATIENT_ID.Value : this.patientId;
                this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs = this.currentPrescriptionFilter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs;
                if (IsCheckNotShowTT)
                {
                    if (this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs == null || this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs.Count == 0)
                        this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs = new List<long>();
                    if (chkNotShowExpMestTypeDTT.Checked)
                        this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs.Add(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT);
                    else if(this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs.Exists(o=> o == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT))
                        this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs.Remove(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT);
                }
                else if (this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs.Exists(o => o == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT))
                    this.serviceReqView7Filter.NULL_OR_NOT_IN_EXP_MEST_TYPE_IDs.Remove(IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT);
                this.serviceReqView7Filter.ORDER_DIRECTION = this.currentPrescriptionFilter.ORDER_DIRECTION;
                this.serviceReqView7Filter.ORDER_FIELD = this.currentPrescriptionFilter.ORDER_FIELD;
                this.ListServiceReqs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_7>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW7, ApiConsumers.MosConsumer, this.serviceReqView7Filter, param);

                if (this.ListServiceReqs != null && this.ListServiceReqs.Count > 0)
                    ListServiceReqs = ListServiceReqsAll = this.ListServiceReqs.Where(o => o.ID > 0).OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.ID).ToList();
                else
                    ListServiceReqs = ListServiceReqsAll = new List<V_HIS_SERVICE_REQ_7>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSelectDate(DateTime dateTimeFrom, DateTime dateTimeTo)
        {
            this.serviceReqView7Filter.INTRUCTION_TIME_FROM = InFrom = (dateTimeFrom != null && dateTimeFrom != DateTime.MinValue) ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(dateTimeFrom.ToString("yyyyMMdd") + "000000") : null;
            this.serviceReqView7Filter.INTRUCTION_TIME_TO = InTo = (dateTimeTo != null && dateTimeTo != DateTime.MinValue) ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(dateTimeTo.ToString("yyyyMMdd") + "235959") : null;
            IsOptionTime = true;
        }
        #endregion

        #region Method

        public void FLoad()
        {
            try
            {
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public PreServiceReqADO GetFocusRow()
        {
            PreServiceReqADO result = new PreServiceReqADO();
            try
            {
                result = treeListServiceReq.GetDataRecordByNode(treeListServiceReq.FocusedNode) as PreServiceReqADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new PreServiceReqADO();
            }
            return result;
        }

        public List<V_HIS_SERVICE_REQ_7> GetServiceReqData()
        {
            return this.ListServiceReqs;
        }
        public List<V_HIS_SERVICE_REQ_7> GetServiceReqDataAll()
        {
            return this.ListServiceReqsAll;
        }
        public List<PreServiceReqADO> GetPreServiceReqADOData()
        {
            return this.PreServiceReqADOs;
        }

        public IntructionTimeADO GetIntructionTime()
        {
            IntructionTimeADO ado = new IntructionTimeADO();
            try
            {
                if (IsOptionTime && InFrom.HasValue && InTo.HasValue)
                {
                    ado.IntructionTimeFrom = InFrom;
                    ado.IntructionTimeTo = InTo;
                }
                else if (cboFilter.EditValue != null)
                {
                    if ((string)cboFilter.EditValue == "1")
                    {
                        ado.IntructionTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.AddMonths(-1));
                        ado.IntructionTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else if ((string)cboFilter.EditValue == "2")
                    {
                        ado.IntructionTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.AddMonths(-2));
                        ado.IntructionTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                    else if ((string)cboFilter.EditValue == "3")
                    {
                        ado.IntructionTimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.AddMonths(-3));
                        ado.IntructionTimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);
                    }
                }
            }
            catch (Exception ex)
            {
                ado = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return ado;
        }
        public void SetListAll(List<V_HIS_SERVICE_REQ_7> serviceReqs)
        {
            try
            {
                this.ListServiceReqsAll = serviceReqs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void Reload(List<V_HIS_SERVICE_REQ_7> serviceReqs)
        {
            try
            {
                this.PreServiceReqADOs = new List<PreServiceReqADO>();
                Inventec.Common.Logging.LogSystem.Debug("Reload.1");
                this.ListServiceReqs = serviceReqs;
                if (chkNotShowExpMestTypeDTT.Checked && this.ListServiceReqs != null && this.ListServiceReqs.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Reload.2");
                    this.ListServiceReqs = this.ListServiceReqs.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT).OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.ID).ToList();
                }

                if (this.vlShowingOldPrescriptionOption == "1" && this.isPresPK)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Reload.3");
                    this.ListServiceReqs = this.ListServiceReqs.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK || (o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && o.IS_HOME_PRES.HasValue && o.IS_HOME_PRES == 1)).OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.ID).ToList();

                    int prescriptionOldLimit = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__OLD_PRECRIPTIONS_DISPLAY_LIMIT");
                    if (prescriptionOldLimit > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Reload.4");
                        this.ListServiceReqs = this.ListServiceReqs.Skip(0).Take(prescriptionOldLimit).ToList();
                    }
                    Inventec.Common.Logging.LogSystem.Debug("Reload.5");
                }

                //var ListServiceReqGExpMest = this.ListServiceReqs.GroupBy(o => o.EXP_MEST_ID);
                var ListServiceReqGExpMest = this.ListServiceReqs.GroupBy(o => o.INTRUCTION_DATE);

                foreach (var item in ListServiceReqGExpMest)
                {
                    PreServiceReqADO ado = new PreServiceReqADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<PreServiceReqADO>(ado, item.First());
                    ado.ID = -1;
                    //ado.ID_NODE = ado.EXP_MEST_CODE + "__";
                    ado.ID_NODE = ado.INTRUCTION_DATE + "__";
                    ado.IS_PARENT_1 = false;
                    ado.MEDI_STOCK_ID = null;
                    ado.MEDI_STOCK_CODE = "";
                    ado.MEDI_STOCK_NAME = "";
                    this.PreServiceReqADOs.Add(ado);

                    var ListOrder = item.OrderByDescending(o => o.INTRUCTION_TIME).ToList();
                    foreach (var item1 in ListOrder)
                    {
                        PreServiceReqADO ado1 = new PreServiceReqADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<PreServiceReqADO>(ado1, item1);
                        ado1.ID_NODE = item1.ID + ".";
                        //ado1.PARENT_ID_NODE = ado.EXP_MEST_CODE + "__";
                        ado1.PARENT_ID_NODE = ado.INTRUCTION_DATE + "__";
                        ado1.IS_PARENT_1 = true;
                        this.PreServiceReqADOs.Add(ado1);
                    }
                }


                //AutoMapper.Mapper.CreateMap<V_HIS_SERVICE_REQ_7, HIS.UC.PeriousExpMestList.ADO.PreServiceReqADO>();
                //this.PreServiceReqADOs = AutoMapper.Mapper.Map<List<HIS.UC.PeriousExpMestList.ADO.PreServiceReqADO>>(this.ListServiceReqs);

                this.patientId = (this.ListServiceReqs != null && this.ListServiceReqs.Count > 0) ? this.ListServiceReqs[0].TDL_PATIENT_ID : 0;
                this.ProcessGetDataMetyMaty();
                Inventec.Common.Logging.LogSystem.Debug("Reload.6");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void DisposeData()
        {
            try
            {
                this.chkNotShowExpMestTypeDTT.CheckedChanged -= new System.EventHandler(this.chkNotShowPresTT_CheckedChanged);
                this.cboFilter.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboFilter_ButtonClick);
                this.cboFilter.EditValueChanged -= new System.EventHandler(this.cboFilter_EditValueChanged);
                this.treeListServiceReq.CustomNodeCellEdit -= new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.treeListServiceReq_CustomNodeCellEdit);
                this.treeListServiceReq.CustomUnboundColumnData -= new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.treeListServiceReq_CustomUnboundColumnData);
                this.repositoryItemButtonEditselect.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditselect_ButtonClick);
                this.repositoryItemButtonEditView.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEditView_ButtonClick);
                this.toolTipController1.GetActiveObjectInfo -= new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(this.toolTipController1_GetActiveObjectInfo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ProcessGetDataMetyMaty()
        {
            BindingList<PreServiceReqADO> listResult = null;

            if (this.PreServiceReqADOs != null && this.PreServiceReqADOs.Count > 0)
            {
                //this.PreServiceReqADOs.ForEach(o => o.ID_NODE = o.ID + ".");

                List<long> expMestIds = this.ListServiceReqs.Where(o => o.EXP_MEST_ID.HasValue && o.EXP_MEST_TYPE_ID.HasValue).Select(o => (o.EXP_MEST_ID ?? 0)).Distinct().ToList();
                if (expMestIds != null && expMestIds.Count > 0)
                {
                    await GetDataMedicineAll(expMestIds, PreServiceReqADOs);
                    await GetDataMaterialAll(expMestIds, PreServiceReqADOs);
                }

                var rooms = BackendDataWorker.Get<V_HIS_ROOM>();

                await ProcessGetServiceReqMety(PreServiceReqADOs);
                await ProcessGetServiceReqMaty(PreServiceReqADOs);

                this.PreServiceReqADOs.ForEach(o => SetRoomName(o, rooms));
            }

            listResult = new BindingList<PreServiceReqADO>(this.PreServiceReqADOs);
            treeListServiceReq.DataSource = listResult;
            treeListServiceReq.ExpandAll();
        }

        private void SetRoomName(PreServiceReqADO data, List<V_HIS_ROOM> rooms)
        {
            try
            {
                var room = rooms.Where(o => o.ID == data.REQUEST_ROOM_ID).FirstOrDefault();
                data.REQUEST_ROOM_NAME = room != null ? room.ROOM_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task ProcessGetServiceReqMety(List<PreServiceReqADO> preServiceReqADOs)
        {
            try
            {
                PreServiceReqADO preServiceReqADOCreate = new PreServiceReqADO();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter expMestMetyFilter = new MOS.Filter.HisServiceReqMetyFilter();
                expMestMetyFilter.SERVICE_REQ_IDs = this.ListServiceReqs.Select(o => o.ID).ToList();
                var lstExpMestMety = await new BackendAdapter(param).GetAsync<List<HIS_SERVICE_REQ_METY>>(HisRequestUriStore.HIS_SERVICE_REQ_METY__GET, ApiConsumers.MosConsumer, expMestMetyFilter, param);

                if (lstExpMestMety != null)
                {
                    var q1 = (from m in lstExpMestMety
                              select preServiceReqADOCreate.CreatePreServiceReqADO(m, this.ListServiceReqs)).ToList();
                    if (q1 != null && q1.Count > 0)
                        preServiceReqADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //vat tu trong danh muc
        private async Task ProcessGetServiceReqMaty(List<PreServiceReqADO> preServiceReqADOs)
        {
            try
            {
                PreServiceReqADO preServiceReqADOCreate = new PreServiceReqADO();
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter expMestMatyFilter = new MOS.Filter.HisServiceReqMatyFilter();
                expMestMatyFilter.SERVICE_REQ_IDs = this.ListServiceReqs.Select(o => o.ID).ToList();
                var lstExpMestMaty = await new BackendAdapter(param).GetAsync<List<HIS_SERVICE_REQ_MATY>>(HisRequestUriStore.HIS_SERVICE_REQ_MATY__GET, ApiConsumers.MosConsumer, expMestMatyFilter, param);

                if (lstExpMestMaty != null)
                {
                    var q1 = (from m in lstExpMestMaty
                              select preServiceReqADOCreate.CreatePreServiceReqADO(m, this.ListServiceReqs)).ToList();
                    if (q1 != null && q1.Count > 0)
                        preServiceReqADOs.AddRange(q1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetDataMedicineAll(List<long> ExpMestIds, List<PreServiceReqADO> preServiceReqADOs)
        {
            try
            {
                PreServiceReqADO preServiceReqADOCreate = new PreServiceReqADO();
                CommonParam param = new CommonParam();
                MOS.Filter.HisExpMestMedicineViewFilter filter = new MOS.Filter.HisExpMestMedicineViewFilter();
                filter.EXP_MEST_IDs = ExpMestIds;
                var expMestMedicineAlls = await new Inventec.Common.Adapter.BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (expMestMedicineAlls != null && expMestMedicineAlls.Count > 0)
                {
                    foreach (var item in expMestMedicineAlls)
                    {
                        if ((item.IS_NOT_PRES ?? 0) != 1)
                        {
                            var itemNoPres = expMestMedicineAlls.FirstOrDefault(o =>
                                o.IS_NOT_PRES.HasValue && o.IS_NOT_PRES == 1
                                && o.MEDICINE_TYPE_ID == item.MEDICINE_TYPE_ID
                                && ((o.MEDICINE_ID == null && item.MEDICINE_ID == null) || (o.MEDICINE_ID == item.MEDICINE_ID))
                                && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                //&& o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                //&& o.IS_EXPEND == item.IS_EXPEND
                                //&& o.EXPEND_TYPE_ID == item.EXPEND_TYPE_ID
                                //&& o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                                //&& o.SERE_SERV_PARENT_ID == item.SERE_SERV_PARENT_ID
                                //&& o.TUTORIAL == item.TUTORIAL
                                );

                            if (itemNoPres != null)
                            {
                                item.AMOUNT += itemNoPres.AMOUNT;
                                item.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)item.AMOUNT, 0);
                            }
                            PreServiceReqADO preServiceReqADOAdd = preServiceReqADOCreate.CreatePreServiceReqADO(item, this.ListServiceReqs);
                            preServiceReqADOs.Add(preServiceReqADOAdd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task GetDataMaterialAll(List<long> ExpMestIds, List<PreServiceReqADO> preServiceReqADOs)
        {
            try
            {
                CommonParam param = new CommonParam();
                PreServiceReqADO preServiceReqADOCreate = new PreServiceReqADO();
                MOS.Filter.HisExpMestMaterialViewFilter filter = new MOS.Filter.HisExpMestMaterialViewFilter();
                filter.EXP_MEST_IDs = ExpMestIds;
                var expMestMaterialAlls = await new Inventec.Common.Adapter.BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MATERIAL>>(HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (expMestMaterialAlls != null && expMestMaterialAlls.Count > 0)
                {
                    foreach (var item in expMestMaterialAlls)
                    {
                        if ((item.IS_NOT_PRES ?? 0) != 1)
                        {
                            var itemNoPres = expMestMaterialAlls.FirstOrDefault(o =>
                                o.IS_NOT_PRES.HasValue && o.IS_NOT_PRES == 1
                                && o.MATERIAL_TYPE_ID == item.MATERIAL_TYPE_ID
                                && ((o.MATERIAL_ID == null && item.MATERIAL_ID == null) || (o.MATERIAL_ID == item.MATERIAL_ID))
                                && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID
                                //&& o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID
                                //&& o.IS_EXPEND == item.IS_EXPEND
                                //&& o.EXPEND_TYPE_ID == item.EXPEND_TYPE_ID
                                //&& o.IS_OUT_PARENT_FEE == item.IS_OUT_PARENT_FEE
                                );

                            if (itemNoPres != null)
                            {
                                item.AMOUNT += itemNoPres.AMOUNT;
                                item.AMOUNT = (decimal)Inventec.Common.Number.Convert.RoundUpValue((double)item.AMOUNT, 0);
                            }
                            PreServiceReqADO preServiceReqADOAdd = preServiceReqADOCreate.CreatePreServiceReqADO(item, this.ListServiceReqs);
                            preServiceReqADOs.Add(preServiceReqADOAdd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void chkNotShowPresTT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.notProcessWhileNotShowExpMestTypeDTTCheckedChanged)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => notProcessWhileNotShowExpMestTypeDTTCheckedChanged), notProcessWhileNotShowExpMestTypeDTTCheckedChanged));
                    this.notProcessWhileNotShowExpMestTypeDTTCheckedChanged = false;
                    return;
                }

                CommonParam param = new CommonParam();
                int prescriptionOldLimit = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__OLD_PRECRIPTIONS_DISPLAY_LIMIT");
                List<V_HIS_SERVICE_REQ_7> LisShow = ListServiceReqsAll;
                if (prescriptionOldLimit > 0 && !(this.vlShowingOldPrescriptionOption == "1" && this.isPresPK))
                {
                    if (chkNotShowExpMestTypeDTT.Checked && this.ListServiceReqsAll != null && this.ListServiceReqsAll.Count > 0)
                    {
                        LisShow = this.ListServiceReqsAll.Where(o => o.EXP_MEST_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_TYPE.ID__DTT).OrderByDescending(o => o.INTRUCTION_TIME).ThenByDescending(o => o.ID).Take(prescriptionOldLimit).ToList();
                    }
                    else
                    {
                        LisShow = ListServiceReqsAll.Take(prescriptionOldLimit).ToList();
                    }
                }
                Reload(LisShow);
                if (this.actNotShowExpMestTypeDTTCheckedChanged != null)
                    this.actNotShowExpMestTypeDTTCheckedChanged(chkNotShowExpMestTypeDTT.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
