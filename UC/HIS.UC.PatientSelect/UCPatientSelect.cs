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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.Data;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PatientSelect;
using HIS.UC.PatientSelect.ADO;
using HIS.UC.PatientSelect.GetFocusRow;
using HIS.UC.PatientSelect.Run;
using HIS.UC.PatientSelect.Reload;
using MOS.SDO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraExport.Helpers;
using static DevExpress.XtraPrinting.Native.PageSizeInfo;

namespace HIS.UC.PatientSelect.Run
{
    public partial class UCPatientSelect : UserControl
    {
        #region Declare

        LanguageInputADO languageInputADO;
        private long TreatmentId { get; set; }
        private long CurrentTreatmentId { get; set; }
        private V_HIS_TREATMENT_BED_ROOM RowCellClickBedRoom { get; set; }
        private List<V_HIS_TREATMENT_BED_ROOM> ListTreatmentBedRooms { get; set; }
        Dictionary<long, bool> dicPescriotionPerious = new Dictionary<long, bool>();
        CheckChangeSelectedPatientWhileHasPrescription checkChangeSelectedPatientWhileHasPrescription;
        gridviewHandler selectSingleRow;
        int rowIndexSelected = -1;
        int rowCount;
        bool? isAutoWidth { get; set; }
        bool isInitForm { get; set; }
        long RoomId { get; set; }
        int width { get; set; }
        int height { get; set; }
        bool IsShowBedRoomName { get; set; }
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        PatientSelectInitADO data;
        MOS.Filter.HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilterInput;
        List<long> treatmentId = new List<long>();
        #endregion

        #region Contructor
        public UCPatientSelect()
            : this(null)
        {
        }

        public UCPatientSelect(PatientSelectInitADO data)
        {
            InitializeComponent();
            try
            {
                if (data != null)
                {
                    this.data = data;
                    this.TreatmentId = data.TreatmentId;
                    this.CurrentTreatmentId = data.TreatmentId;
                    this.checkChangeSelectedPatientWhileHasPrescription = data.CheckChangeSelectedPatientWhileHasPrescription;
                    this.selectSingleRow = data.SelectedSingleRow;
                    this.languageInputADO = data.LanguageInputADO;
                    this.isAutoWidth = data.IsAutoWidth;
                    this.isInitForm = data.IsInitForm;
                    this.RoomId = data.RoomId;
                    this.IsShowBedRoomName = data.IsShowColumnBedRoomName;
                }
                this.SetCaptionByLanguageKey();
                this.gridControlTreatmentBedRoom.ToolTipController = this.toolTipController1;
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
                grdColSTT.Caption = this.languageInputADO.gridControlTreatmentBedRoom__grdColSTT__Caption;
                grdColPatientName.Caption = this.languageInputADO.gridControlTreatmentBedRoom__grdColPatientName__Caption;
                grdColDob.Caption = this.languageInputADO.gridControlTreatmentBedRoom__grdColDob__Caption;
                grdColTreatmentCode.Caption = this.languageInputADO.gridControlTreatmentBedRoom__grdColTreatmentCode__Caption;
                grdColBedName.Caption = this.languageInputADO.gridControlTreatmentBedRoom__grdColBedName__Caption;
                if (!String.IsNullOrEmpty(this.languageInputADO.txtKeyWord__NullValuePromp))
                {
                    txtKeyWord.Properties.NullValuePromptShowForEmptyValue = true;
                    txtKeyWord.Properties.NullValuePrompt = this.languageInputADO.txtKeyWord__NullValuePromp;
                }
                grdBedRoomName.Caption = this.languageInputADO.gridControlTreatmentBedRoom__grdColBedRoomName__Caption;
                if (!this.IsShowBedRoomName)
				{
                    grdBedRoomName.Visible = false;
                }
                grdColClassifyName.Caption = !string.IsNullOrEmpty(this.languageInputADO.gridControlTreatmentBedRoom__grdColClassifyName__Caption) ? this.languageInputADO.gridControlTreatmentBedRoom__grdColClassifyName__Caption : grdColClassifyName.Caption;
                //grdColClassifyName.VisibleIndex = -1;
                this.txtKeyWord.Properties.NullValuePrompt = this.languageInputADO.SearchNull_Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
        private void gridViewTreatmentBedRoomList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TREATMENT_BED_ROOM treatmentBedRoom = (V_HIS_TREATMENT_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        if (treatmentBedRoom != null && treatmentBedRoom.TDL_PATIENT_DOB.ToString().Length >= 4)
                        {
                            e.Value = treatmentBedRoom.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.RowHandle), e.RowHandle)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.Column.FieldName), e.Column.FieldName)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.Column.VisibleIndex), e.Column.VisibleIndex)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => e.Column.AbsoluteIndex), e.Column.AbsoluteIndex));

                this.ProcessChangeSelectedPatientRowInTreatmentBedRoom(e.Column.FieldName != "DX$CheckboxSelectorColumn");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.ProcessChangeSelectedPatientRowInTreatmentBedRoom();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                var index = this.gridViewTreatmentBedRoom.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0) return;

                var listDatas = this.gridControlTreatmentBedRoom.DataSource as List<V_HIS_TREATMENT_BED_ROOM>;
                var dataRow = listDatas[index];
                if (dataRow != null && dicPescriotionPerious != null)
                {
                    if (dicPescriotionPerious.ContainsKey(dataRow.PATIENT_ID))
                    {
                        e.Appearance.ForeColor = System.Drawing.Color.Red;
                        var bedRoom = (V_HIS_TREATMENT_BED_ROOM)this.gridViewTreatmentBedRoom.GetFocusedRow();
                        if (bedRoom.PATIENT_ID == dataRow.PATIENT_ID)
                        {
                            e.Appearance.BorderColor = System.Drawing.Color.Red;
                        }
                        //e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == this.gridControlTreatmentBedRoom)
                {
                    string text = "";
                    DevExpress.XtraGrid.Views.Grid.GridView view = this.gridControlTreatmentBedRoom.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (this.lastRowHandle != info.RowHandle || this.lastColumn != info.Column)
                        {
                            this.lastColumn = info.Column;
                            this.lastRowHandle = info.RowHandle;

                            long patientId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(this.lastRowHandle, "PATIENT_ID") ?? "0").ToString());

                            if (patientId > 0 && dicPescriotionPerious != null && dicPescriotionPerious.ContainsKey(patientId))
                            {
                                text = this.languageInputADO.CanhBaoBenhNhanDaKeThuocTrongNgay;
                            }

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

        private void UCTreatmentBedRoomList_Load(object sender, EventArgs e)
        {
            try
            {
                if (this.isAutoWidth.HasValue)
                {
                    this.gridViewTreatmentBedRoom.OptionsView.ColumnAutoWidth = this.isAutoWidth.Value;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treatmentId.AddRange(GetSelectedRows().Select(o => o.TREATMENT_ID).ToList());
                    treatmentId = treatmentId.Distinct().ToList();
                    FillDataToGridTreatment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method
        private void ProcessChangeSelectedPatientRowInTreatmentBedRoom(bool isSetSelectRowChanged = false)
        {
            try
            {
                bool isLoad = true;
                if (this.checkChangeSelectedPatientWhileHasPrescription != null)
                    isLoad = this.checkChangeSelectedPatientWhileHasPrescription();
                Inventec.Common.Logging.LogSystem.Debug("ProcessChangeSelectedPatientRowInTreatmentBedRoom____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isLoad), isLoad));
                if (isLoad)
                {
                    WaitingManager.Show();
                    this.RowCellClickBedRoom = (V_HIS_TREATMENT_BED_ROOM)this.gridViewTreatmentBedRoom.GetFocusedRow();
                    if (this.RowCellClickBedRoom != null && this.selectSingleRow != null)
                    {
                        this.TreatmentId = this.RowCellClickBedRoom.TREATMENT_ID;
                        this.selectSingleRow(this.RowCellClickBedRoom);
                        if (isSetSelectRowChanged)
                        {
                            int curentFocusedRowHandle = this.gridViewTreatmentBedRoom.FocusedRowHandle;

                            gridViewTreatmentBedRoom.ClearSelection();
                            gridViewTreatmentBedRoom.SelectRow(curentFocusedRowHandle);
                        }
                    }
                    WaitingManager.Hide();
                }
                else
                {
                    this.gridViewTreatmentBedRoom.FocusedRowHandle = rowIndexSelected;
                    if (isSetSelectRowChanged)
                    {
                        gridViewTreatmentBedRoom.ClearSelection();
                        gridViewTreatmentBedRoom.SelectRow(rowIndexSelected);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                WaitingManager.Show();
                ListTreatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
                this.gridControlTreatmentBedRoom.DataSource = null;
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisTreatmentBedRoomViewFilter treatFilter = new MOS.Filter.HisTreatmentBedRoomViewFilter();
                treatFilter.ORDER_DIRECTION = "ASC";
                treatFilter.ORDER_FIELD = "TDL_PATIENT_FIRST_NAME";

                if (this.treatmentBedRoomLViewFilterInput != null)
                {
                    treatFilter.IS_IN_ROOM = this.treatmentBedRoomLViewFilterInput.IS_IN_ROOM;
                }
                else
                {
                    treatFilter.IS_IN_ROOM = true;
                }
                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                    treatFilter.KEYWORD__PATIENT_NAME__TREATMENT_CODE__BED_NAME__PATIENT_CODE = txtKeyWord.Text;
                long bedRoomId = 0;
                MOS.EFMODEL.DataModels.V_HIS_BED_ROOM data = BackendDataWorker.Get<V_HIS_BED_ROOM>().SingleOrDefault(o => o.ROOM_ID == this.RoomId);
                if (data != null)
                    bedRoomId = data.ID;
                treatFilter.BED_ROOM_ID = bedRoomId;
                pageSize = 0;
                var rs = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatFilter, paramCommon);
                List<int> indexRow = new List<int>();
                if (rs != null && rs.Count > 0)
                {
                    ListTreatmentBedRooms = (List<V_HIS_TREATMENT_BED_ROOM>)rs.OrderBy(p => p.TDL_PATIENT_FIRST_NAME).ToList();
                    rowCount = (ListTreatmentBedRooms == null ? 0 : ListTreatmentBedRooms.Count);
                    if (this.isInitForm)
                    {
                        this.rowIndexSelected = -1;
                        for (int i = 0; i < rowCount; i++)
                        {
                            if (ListTreatmentBedRooms[i].TREATMENT_ID == TreatmentId)
                            {
                                this.rowIndexSelected = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < rowCount; i++)
                        {
                            if (treatmentId.Exists(o => o == ListTreatmentBedRooms[i].TREATMENT_ID))
                            {
                                indexRow.Add(i);
                            }
                        }
                        if(indexRow == null || indexRow.Count == 0)
						{
                            for (int i = 0; i < rowCount; i++)
                            {
                                if (ListTreatmentBedRooms[i].TREATMENT_ID == TreatmentId)
                                {
                                    indexRow.Add(i);
                                }
                            }
                        }                            
                        //this.rowIndexSelected = 0;
                    }
                }

                this.GetPrescriptionPerious(true);

                gridControlTreatmentBedRoom.BeginUpdate();
                gridControlTreatmentBedRoom.DataSource = ListTreatmentBedRooms;
                gridControlTreatmentBedRoom.EndUpdate();
                if(indexRow!=null && indexRow.Count > 0)
				{
					for (int i = 0; i < indexRow.Count; i++)
					{
                        gridViewTreatmentBedRoom.SelectRow(indexRow[i]);
                    }
                    gridViewTreatmentBedRoom.FocusedRowHandle = indexRow.LastOrDefault();
                    gridViewTreatmentBedRoom.TopRowIndex = rowIndexSelected;
                    this.RowCellClickBedRoom = ListTreatmentBedRooms[indexRow.LastOrDefault()];
                }
                else if (this.rowIndexSelected >= 0)
                {
                    //TH danh sách bệnh nhân trong buồng có bệnh nhân đang chọn hoặc đang tìm kiếm để chọn 1 bệnh nhân
                    gridViewTreatmentBedRoom.FocusedRowHandle = rowIndexSelected;              
                    gridViewTreatmentBedRoom.SelectRow(rowIndexSelected);
                    gridViewTreatmentBedRoom.TopRowIndex = rowIndexSelected;
                    this.RowCellClickBedRoom = (rowCount > 0 ? ListTreatmentBedRooms[rowIndexSelected] : null);
                }
                else if (this.isInitForm)
                {
                    //TH danh sách bệnh nhân trong buồng không còn bệnh nhân đang chọn/sửa đơn từ ds y lệnh
                    //==>Tự tạo ra đối tượng để chứa hồ sơ bệnh nhân đang chọn/đơn đang sửa để hiển thị thông tin trên form kê đơn

                    CommonParam paramCommon1 = new CommonParam();
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                    treatmentFilter.ID = this.TreatmentId;

                    var rs1 = new Inventec.Common.Adapter.BackendAdapter(paramCommon1).Get<List<HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, paramCommon1);
                    if (rs1 != null && rs1.Count > 0)
                    {
                        HIS_TREATMENT treatment = rs1.First();
                        this.RowCellClickBedRoom = new V_HIS_TREATMENT_BED_ROOM();
                        this.RowCellClickBedRoom.TREATMENT_CODE = treatment.TREATMENT_CODE;
                        this.RowCellClickBedRoom.TREATMENT_ID = treatment.ID;
                        this.RowCellClickBedRoom.TDL_PATIENT_DOB = treatment.TDL_PATIENT_DOB;
                        this.RowCellClickBedRoom.TDL_PATIENT_NAME = treatment.TDL_PATIENT_NAME;
                        this.RowCellClickBedRoom.TDL_PATIENT_GENDER_NAME = treatment.TDL_PATIENT_GENDER_NAME;
                    }
                    Inventec.Common.Logging.LogSystem.Info("TH danh sách bệnh nhân trong buồng không còn bệnh nhân đang chọn/sửa đơn từ ds y lệnh ==> Tự tạo ra đối tượng để chứa hồ sơ bệnh nhân đang chọn/đơn đang sửa để hiển thị thông tin trên form kê đơn"
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TreatmentId), TreatmentId)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isInitForm), isInitForm)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => RowCellClickBedRoom), RowCellClickBedRoom));
                }
                if (this.selectSingleRow != null && this.RowCellClickBedRoom != null)
                {
                    this.TreatmentId = this.RowCellClickBedRoom.TREATMENT_ID;
                    this.selectSingleRow(this.RowCellClickBedRoom);
                }
                WaitingManager.Hide();
                this.isInitForm = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FLoad()
        {
            try
            {
                this.isInitForm = true;
                FillDataToGridTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FLoad(MOS.Filter.HisTreatmentBedRoomLViewFilter treatmentBedRoomLViewFilter)
        {
            try
            {
                this.isInitForm = true;
                this.treatmentBedRoomLViewFilterInput = treatmentBedRoomLViewFilter;
                FillDataToGridTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<V_HIS_TREATMENT_BED_ROOM> listtreatmentbedrooms)
        {
            try
            {
                this.ListTreatmentBedRooms = listtreatmentbedrooms;
                WaitingManager.Show();
                this.pageSize = 0;
                this.GetPrescriptionPerious(false);
                gridViewTreatmentBedRoom.GridControl.BeginUpdate();
                gridViewTreatmentBedRoom.GridControl.DataSource = this.ListTreatmentBedRooms;
                gridViewTreatmentBedRoom.GridControl.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        int pageSize = 0;
        private void GetPrescriptionPerious(bool? ScrollLoad)
        {
            try
            {
                List<HIS_EXP_MEST> lst = new List<HIS_EXP_MEST>();
                int skip = 0;
                while (this.ListTreatmentBedRooms.Count - pageSize - skip > 0)
                {
                    var listIds = this.ListTreatmentBedRooms.Skip(skip + pageSize).Take(100).ToList();
                    skip += 100;
                    pageSize += 100;
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestFilter expMestFilter = new MOS.Filter.HisExpMestFilter();
                    expMestFilter.TDL_INTRUCTION_DATE_FROM = Inventec.Common.DateTime.Get.StartDay();
                    expMestFilter.TDL_INTRUCTION_DATE_TO = Inventec.Common.DateTime.Get.EndDay();
                    expMestFilter.TDL_PATIENT_IDs = listIds.Where(o => this.dicPescriotionPerious != null && !this.dicPescriotionPerious.ContainsKey(o.PATIENT_ID)).Select(o=>o.PATIENT_ID).ToList();
                    if (expMestFilter.TDL_PATIENT_IDs != null && expMestFilter.TDL_PATIENT_IDs.Count > 0)
                    {
                        var listExpMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST__GET, ApiConsumers.MosConsumer, expMestFilter, param);
                        if (listExpMests != null && listExpMests.Count > 0)
                            lst.AddRange(listExpMests);
                    }
                    if (ScrollLoad == true)
                        break;
                }
                if (lst != null && lst.Count > 0)
                {
                    var dicExpMest = lst.GroupBy(o => o.TDL_PATIENT_ID ?? 0).ToDictionary(o => o.Key, o => o.ToList().Count > 0);
                    foreach (var item in dicExpMest)
                    {
                        if (!dicPescriotionPerious.ContainsKey(item.Key))
                            dicPescriotionPerious[item.Key] = item.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadStatePrescriptionPerious()
        {
            try
            {
                this.pageSize = 0;
                this.GetPrescriptionPerious(false);

                int[] selectRows = gridViewTreatmentBedRoom.GetSelectedRows();

                gridViewTreatmentBedRoom.GridControl.BeginUpdate();
                gridViewTreatmentBedRoom.GridControl.DataSource = this.ListTreatmentBedRooms;
                for (int i = 0; i < selectRows.Length; i++)
                {
                    gridViewTreatmentBedRoom.SelectRow(selectRows[i]);
                }
                gridViewTreatmentBedRoom.GridControl.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public List<V_HIS_TREATMENT_BED_ROOM> listTemp = new List<V_HIS_TREATMENT_BED_ROOM>();
        public void SetOnlyOneRow(bool IsEnableOnly)
        {
            try
            {
                if (IsEnableOnly)
                {
                    listTemp = GetSelectedRows();
                    for (int i = 0; i < ListTreatmentBedRooms.Count; i++)
                    {
                        if (ListTreatmentBedRooms[i].TREATMENT_ID == CurrentTreatmentId)
                        {
                            gridViewTreatmentBedRoom.ClearSelection();
                            gridViewTreatmentBedRoom.OptionsSelection.MultiSelect = false;
                            gridViewTreatmentBedRoom.SelectRow(i);
                            TreatmentId = CurrentTreatmentId;
                            RowCellClickBedRoom = ListTreatmentBedRooms[i];
                            this.selectSingleRow(this.RowCellClickBedRoom);
                            break;
                        }
                    }
                }
                else
                {
                    gridViewTreatmentBedRoom.ClearSelection();
                    gridViewTreatmentBedRoom.OptionsSelection.MultiSelect = true;
                    int maxIndex = 0;
                    for (int i = 0; i < ListTreatmentBedRooms.Count; i++)
                    {
                        if (listTemp!= null && listTemp.Exists(o=>o.TREATMENT_ID == ListTreatmentBedRooms[i].TREATMENT_ID))
                        {
                            gridViewTreatmentBedRoom.SelectRow(i);
                            maxIndex = i;
                        }
                    }
                    gridViewTreatmentBedRoom.FocusedRowHandle = maxIndex;
                    RowCellClickBedRoom = ListTreatmentBedRooms[maxIndex];

                    if (this.selectSingleRow != null && this.RowCellClickBedRoom != null)
                    {
                        this.TreatmentId = this.RowCellClickBedRoom.TREATMENT_ID;
                        this.selectSingleRow(this.RowCellClickBedRoom);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public V_HIS_TREATMENT_BED_ROOM GetFocusRow()
        {
            V_HIS_TREATMENT_BED_ROOM result = new V_HIS_TREATMENT_BED_ROOM();
            try
            {
                result = (V_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetFocusedRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new V_HIS_TREATMENT_BED_ROOM();
            }
            return result;
        }

        public List<V_HIS_TREATMENT_BED_ROOM> GetSelectedRows()
        {
            List<V_HIS_TREATMENT_BED_ROOM> result = new List<V_HIS_TREATMENT_BED_ROOM>();
            try
            {
                int[] selectRows = gridViewTreatmentBedRoom.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var mediMatyTypeADO = (V_HIS_TREATMENT_BED_ROOM)gridViewTreatmentBedRoom.GetRow(selectRows[i]);
                        result.Add(mediMatyTypeADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        public void FocusSearchTextbox()
        {
            try
            {
                this.txtKeyWord.Focus();
                this.txtKeyWord.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        private int GetLastVisibleRowHandle(GridView view)
        {
            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
            return viewInfo.RowsInfo.Last().RowHandle;
        }
        private void gridViewTreatmentBedRoom_TopRowChanged(object sender, EventArgs e)
        {
            try
            {
                int rowCount = GetLastVisibleRowHandle(gridViewTreatmentBedRoom);
                this.BeginInvoke(new MethodInvoker(delegate { IsBottom(rowCount); }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        void IsBottom(int rowCount)
        {
            //if (gridViewTreatmentBedRoom.IsRowVisible(0) == RowVisibleState.Visible) LoadPresciptionPerious(false);
            if (gridViewTreatmentBedRoom.IsRowVisible(rowCount - 1) == RowVisibleState.Visible) LoadPresciptionPerious(true);
        }


        void LoadPresciptionPerious(bool bottom)
        {
            if (bottom)
            {
                this.GetPrescriptionPerious(true);
                gridViewTreatmentBedRoom.GridControl.RefreshDataSource();
            }
        }
    }
}
