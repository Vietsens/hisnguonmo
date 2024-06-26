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
using Inventec.Core;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using TYT.Desktop.Plugins.TYTTreatment;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Logging;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Common.Adapter;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.ConfigSystem;
using System.IO;
using DevExpress.XtraEditors.Controls;
using TYT.Filter;
using TYT.EFMODEL.DataModels;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;

namespace TYT.Desktop.Plugins.TYTTreatment
{
    public partial class UCListTYTTreatment : HIS.Desktop.Utility.UserControlBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int pageSize;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<TYT_UNINFECT_ICD> listTytUninfectIcd;
        List<HIS_TREATMENT_TYPE> treatmentType;
        List<TYT_UNINFECT_ICD> _IcdSelecteds;

        public UCListTYTTreatment(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.currentModule = module;
        }

        private void UCListTYTTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                GetTytUninfectIcd();
                InitComboGroup();
                InitCheck(cboICD, SelectionGrid__Status);
                InitCombo(cboICD, listTytUninfectIcd, "UNINFECT_ICD_NAME", "ID");
                setDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);

                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                cbo.Properties.DataSource = listTytUninfectIcd;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectCombo(GridLookUpEdit cbo, object data)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                _IcdSelecteds = new List<TYT_UNINFECT_ICD>();
                foreach (TYT_UNINFECT_ICD rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _IcdSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboICD_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string icdName = "";
                if (_IcdSelecteds != null && _IcdSelecteds.Count > 0)
                {
                    foreach (var item in _IcdSelecteds)
                    {
                        icdName += item.UNINFECT_ICD_NAME + ", ";
                    }
                }

                e.DisplayText = icdName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void GetTytUninfectIcd()
        {
            try
            {
                TytUninfectIcdFilter filter = new TytUninfectIcdFilter();
                filter.IS_ACTIVE = 1;
                CommonParam param = new CommonParam();
                listTytUninfectIcd = new BackendAdapter(param).Get<List<TYT_UNINFECT_ICD>>("api/TytUninfectIcd/Get", ApiConsumers.TytConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboGroup()
        {
            try
            {
                TytUninfectIcdGroupFilter filter = new TytUninfectIcdGroupFilter();
                CommonParam param = new CommonParam();
                List<TYT_UNINFECT_ICD_GROUP> data = new BackendAdapter(param).Get<List<TYT_UNINFECT_ICD_GROUP>>("api/TytUninfectIcdGroup/Get", ApiConsumers.TytConsumer, filter, param);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("UNINFECT_ICD_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("UNINFECT_ICD_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("UNINFECT_ICD_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGroup, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboIcd(List<TYT_UNINFECT_ICD> data)
        {
            try
            {
                cboICD.Properties.BeginUpdate();
                cboICD.Properties.DataSource = data;
                cboICD.Properties.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridTYTTreatmentList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    var data = (V_HIS_TREATMENT_9)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TDL_TREATMENT_TYPE_NAME")
                    {
                        if (treatmentType != null)
                        {
                            var tt = treatmentType.FirstOrDefault(o => o.ID == data.TDL_TREATMENT_TYPE_ID);
                            if (tt != null)
                                e.Value = tt.TREATMENT_TYPE_NAME;
                        }
                    }

                    gridViewTYTTreatment.RefreshData();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditHeinCardInfo_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setDefaultControl()
        {
            try
            {
                ResetCombo(cboICD);
                cboICD.Enabled = false;
                cboICD.Enabled = true;
                cboGroup.EditValue = null;
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                txtKeyWord.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                GridPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pageSize, gridControlTYTTreatment);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                ApiResultObject<List<V_HIS_TREATMENT_9>> apiResult = null;
                HisTreatmentView9Filter filter = new HisTreatmentView9Filter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                SetFilter(ref filter);

                gridViewTYTTreatment.BeginUpdate();

                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<V_HIS_TREATMENT_9>>
                    ("api/HisTreatment/GetView9", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlTYTTreatment.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlTYTTreatment.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewTYTTreatment.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                setDefaultControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                Search();
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilter(ref HisTreatmentView9Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                if (listTytUninfectIcd != null && listTytUninfectIcd.Count > 0)
                {
                    filter.ICD_CODE_OR_ICD_SUB_CODEs = listTytUninfectIcd.Select(o => o.UNINFECT_ICD_CODE).ToList();
                }

                if (_IcdSelecteds != null && _IcdSelecteds.Count > 0)
                {
                    filter.ICD_CODE_OR_ICD_SUB_CODEs = _IcdSelecteds.Select(o => o.UNINFECT_ICD_CODE).ToList();
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Search()
        {
            if (btnSearch.Enabled)
            {
                btnSearch.Focus();
                btnSearch_Click(null, null);
            }
        }

        public void Refesh()
        {
            try
            {
                if (btnRefesh.Enabled)
                {
                    btnRefesh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (txtKeyWord.Text != "")
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        btnSearch_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTYTTreatment_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    //    string creator = (gridViewTYTTreatment.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    //    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    //    if (e.Column.FieldName == "DELETE")
                    //    {
                    //        if (creator == loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName))
                    //            e.RepositoryItem = repositoryItemButton__Delete;
                    //        else
                    //            e.RepositoryItem = repositoryItemButton__Delete_D;
                    //    }
                    //    else if (e.Column.FieldName == "EDIT")
                    //    {
                    //        if (creator == loginName || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName))
                    //            e.RepositoryItem = btnGTYTTreatmentEdit;
                    //        else
                    //            e.RepositoryItem = repositoryItemButtonEdit__D;
                    //    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Bạn có muốn hủy dữ liệu không",
                    "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (V_HIS_TREATMENT_9)gridViewTYTTreatment.GetFocusedRow();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("/api/TYTTuberculosis/Delete", ApiConsumers.TytConsumer, row.ID, param);
                        if (apiresul)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();

                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void cboGroup_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboGroup.Properties.Buttons[1].Visible = (cboGroup.EditValue != null);
                if (cboGroup.EditValue != null)
                {
                    var data = listTytUninfectIcd.Where(o => o.UNINFECT_ICD_GROUP_ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboGroup.EditValue.ToString())).ToList();
                    ResetCombo(cboICD);
                    InitComboIcd(data);
                    SelectCombo(cboICD, data);
                }
                else
                {
                    ResetCombo(cboICD);
                    InitComboIcd(listTytUninfectIcd);
                }
                cboICD.Enabled = false;
                cboICD.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGroup_Closed(object sender, ClosedEventArgs e)
        {
        }

        private void cboGroup_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboGroup.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboICD_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //if (e.Button.Kind == ButtonPredefines.Delete)
                //    cboICD.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboICD_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboICD_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboICD.Enabled = false;
                cboICD.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
