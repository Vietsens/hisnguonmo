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
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisPrepareApprove.ADO;
using HIS.Desktop.Plugins.HisPrepareApprove.Validtion;
using TYT.EFMODEL.DataModels;
using TYT.Filter;
using MOS.Filter;
using MOS.SDO;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPrepareApprove
{
    public partial class frmHisPrepareApprove : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        long prepareId;
        int positionHandleControl = -1;
        V_HIS_PREPARE currentPrepare = null;
        List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY> currentPrepareMetyList = new List<V_HIS_PREPARE_METY>();
        List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_MATY> currentPrepareMatyList = new List<V_HIS_PREPARE_MATY>();

        ActionType actionType = ActionType.VIEW;
        HIS.Desktop.Common.RefeshReference refeshReference;

        enum ActionType
        {
            VIEW = 0,
            CREATE = 1,
            UPDATE = 2
        }

        public frmHisPrepareApprove()
        {
            InitializeComponent();
        }

        public frmHisPrepareApprove(Inventec.Desktop.Common.Modules.Module _currentModule, long _prepareId, HIS.Desktop.Common.RefeshReference refeshReference)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.refeshReference = refeshReference;
            this.prepareId = _prepareId;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            this.actionType = ActionType.UPDATE;
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

        private void frmHisPrepareApprove_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefault();

                SetCaptionByLanguageKey();

                GetPrepare();

                FillDataToGridControlProcess();

                FillDataToGridControlView();

                EnableControlChanged(this.actionType);

                ValidateControl();

                gridViewProcess.SelectAll();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPrepare()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPrepareViewFilter filter = new HisPrepareViewFilter();
                filter.ID = this.prepareId;
                var results = new BackendAdapter(param).Get<List<V_HIS_PREPARE>>("api/HisPrepare/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (results != null && results.Count() > 0)
                {
                    currentPrepare = results.FirstOrDefault();
                    lblApprovalLogginName.Text = currentPrepare.CREATOR;
                    lblDescription.Text = currentPrepare.DESCRIPTION;
                    lblUseFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentPrepare.FROM_TIME ?? 0);
                    lblUseTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(currentPrepare.TO_TIME ?? 0);
                }
                else
                {
                    lblUseTo.Text = "";
                    lblUseFrom.Text = "";
                    lblDescription.Text = "";
                    lblApprovalLogginName.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FillDataToGridControlProcess()
        {
            try
            {
                WaitingManager.Show();

                List<MetyMatyADO> metyMatyADOList = new List<MetyMatyADO>();
                CommonParam paramCommon = new CommonParam();

                // get mety
                MOS.Filter.HisPrepareMetyViewFilter filterMety = new HisPrepareMetyViewFilter();
                filterMety.PREPARE_ID = this.prepareId;
                filterMety.ORDER_DIRECTION = "ASC";
                filterMety.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                currentPrepareMetyList = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY>>("api/HisPrepareMety/GetView", ApiConsumers.MosConsumer, filterMety, paramCommon);

                if (currentPrepareMetyList != null && currentPrepareMetyList.Count() > 0)
                {
                    foreach (var item in currentPrepareMetyList)
                    {
                        MetyMatyADO MetyMatyADO = new MetyMatyADO(item);
                        metyMatyADOList.Add(MetyMatyADO);
                    }
                }

                // get maty
                MOS.Filter.HisPrepareMatyViewFilter filterMaty = new HisPrepareMatyViewFilter();
                filterMaty.PREPARE_ID = this.prepareId;
                filterMaty.ORDER_DIRECTION = "ASC";
                filterMaty.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                currentPrepareMatyList = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_MATY>>("api/HisPrepareMaty/GetView", ApiConsumers.MosConsumer, filterMaty, paramCommon);

                if (currentPrepareMatyList != null && currentPrepareMatyList.Count() > 0)
                {
                    foreach (var item in currentPrepareMatyList)
                    {
                        MetyMatyADO MetyMatyADO = new MetyMatyADO(item);
                        MetyMatyADO.ID = item.ID;
                        metyMatyADOList.Add(MetyMatyADO);
                    }
                }

                gridViewProcess.BeginUpdate();
                if (metyMatyADOList != null)
                {
                    gridViewProcess.GridControl.DataSource = metyMatyADOList;
                }
                gridViewProcess.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

                CommonParam param = new CommonParam();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        public void FillDataToGridControlView()
        {
            try
            {
                WaitingManager.Show();

                List<MetyMatyADO> metyMatyADOList = new List<MetyMatyADO>();
                CommonParam paramCommon = new CommonParam();

                // get mety
                MOS.Filter.HisPrepareMetyViewFilter filterMety = new HisPrepareMetyViewFilter();
                filterMety.TDL_TREATMENT_ID = this.currentPrepare.TREATMENT_ID;
                filterMety.ORDER_DIRECTION = "ASC";
                filterMety.ORDER_FIELD = "MEDICINE_TYPE_NAME";
                var prepareMetyList = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_METY>>("api/HisPrepareMety/GetView", ApiConsumers.MosConsumer, filterMety, paramCommon);

                if (prepareMetyList != null && prepareMetyList.Count() > 0)
                {
                    // remove current Mety
                    if (this.currentPrepareMetyList != null && this.currentPrepareMetyList.Count() > 0)
                        prepareMetyList = prepareMetyList.Where(o => !this.currentPrepareMetyList.Select(p => p.ID).Contains(o.ID)).ToList();

                    foreach (var item in prepareMetyList)
                    {
                        MetyMatyADO MetyMatyADO = new MetyMatyADO(item);
                        metyMatyADOList.Add(MetyMatyADO);
                    }
                }

                // get maty
                MOS.Filter.HisPrepareMatyViewFilter filterMaty = new HisPrepareMatyViewFilter();
                filterMaty.TDL_TREATMENT_ID = this.currentPrepare.TREATMENT_ID;
                filterMaty.ORDER_DIRECTION = "ASC";
                filterMaty.ORDER_FIELD = "MATERIAL_TYPE_NAME";
                var prepareMatyList = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PREPARE_MATY>>("api/HisPrepareMaty/GetView", ApiConsumers.MosConsumer, filterMaty, paramCommon);

                if (prepareMatyList != null && prepareMatyList.Count() > 0)
                {
                    // remove currrent maty
                    if (this.currentPrepareMatyList != null && this.currentPrepareMatyList.Count() > 0)
                        prepareMatyList = prepareMatyList.Where(o => !this.currentPrepareMatyList.Select(p => p.ID).Contains(o.ID)).ToList();

                    foreach (var item in prepareMatyList)
                    {
                        MetyMatyADO MetyMatyADO = new MetyMatyADO(item);
                        metyMatyADOList.Add(MetyMatyADO);
                    }
                }

                gridViewView.BeginUpdate();
                if (metyMatyADOList != null)
                {
                    gridViewView.GridControl.DataSource = metyMatyADOList;
                }
                gridViewView.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetDefault()
        {
            try
            {
                if (this.actionType == ActionType.UPDATE)
                {
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangedDataRow()
        {
            prepareId = (long)gridViewProcess.GetFocusedRow();

            if (prepareId > 0)
            {
                this.actionType = ActionType.UPDATE;
                EnableControlChanged(this.actionType);

                //btnUpdate.Enabled = (this.prepareId.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

            }
        }

        private void EnableControlChanged(ActionType action)
        {

        }

        private void btnTuChoiDuyet_Click(object sender, EventArgs e)
        {
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                this.actionType = ActionType.CREATE;
                this.SetDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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
                if (this.actionType == ActionType.UPDATE && this.prepareId > 0)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    WaitingManager.Show();
                    success = new BackendAdapter(param).Post<bool>("api/TYTTuberculosis/Delete", ApiConsumers.MosConsumer, this.prepareId, param);
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);

                    if (success)
                    {
                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidateControl()
        {
            ValidMaxlengthTxtTick();
            ValidMaxlengthTxtGhiChu();
        }

        void ValidMaxlengthTxtTick()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.maxLength = 20;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtGhiChu()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.maxLength = 100;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void gridLookUpEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewProcess_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    MetyMatyADO data = (MetyMatyADO)gridViewProcess.GetRow(e.RowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewProcess_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MetyMatyADO pData = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFIER_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.MODIFY_TIME ?? 0);
                    }

                    gridControlProcess.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;
                gridViewProcess.PostEditor();
                gridViewProcess.UpdateCurrentRow();

                var DataSoure = (List<MetyMatyADO>)gridControlProcess.DataSource;
                if (DataSoure == null || DataSoure.Count() == 0)
                {
                    return;
                }

                string error = "";

                var errorDataSource = DataSoure.Where(o => o.ApprovalAmount == null || o.ApprovalAmount < 0).ToList();
                if (errorDataSource != null && errorDataSource.Count() > 0)
                {
                    error += String.Join(", ", errorDataSource.Select(o => o.MEDICINE_TYPE_NAME).ToList()) + " có số lượng duyệt không hợp lệ (số lượng duyệt lớn hơn hoặc bằng 0)\n";
                }

                var errorDataSourceWithReq = DataSoure.Where(o => o.ApprovalAmount != null && o.ApprovalAmount > o.ReqAmount).ToList();
                if (errorDataSourceWithReq != null && errorDataSourceWithReq.Count() > 0)
                {
                    error += String.Join(", ", errorDataSourceWithReq.Select(o => o.MEDICINE_TYPE_NAME).ToList()) + " có số lượng duyệt lớn hơn số lượng yêu cầu";
                }

                if (!String.IsNullOrWhiteSpace(error))
                {
                    MessageManager.Show(error);
                    return;
                }

                WaitingManager.Show();

                List<HisPrepareMatySDO> matySDOList = new List<HisPrepareMatySDO>();

                List<HisPrepareMetySDO> metySDOList = new List<HisPrepareMetySDO>();

                foreach (var item in DataSoure)
                {
                    if (item.IsMedicine)
                    {
                        HisPrepareMetySDO sdo = new HisPrepareMetySDO();
                        sdo.ApproveAmount = (item.ApprovalAmount ?? 0);
                        sdo.PrepareMetyId = item.ID;
                        metySDOList.Add(sdo);
                    }
                    else
                    {
                        HisPrepareMatySDO sdo = new HisPrepareMatySDO();
                        sdo.ApproveAmount = (item.ApprovalAmount ?? 0);
                        sdo.PrepareMatyId = item.ID;
                        matySDOList.Add(sdo);
                    }
                }

                HisPrepareApproveSDO inputADO = new HisPrepareApproveSDO();
                inputADO.Id = this.prepareId;
                inputADO.PrepareMatys = matySDOList;
                inputADO.PrepareMetys = metySDOList;
                inputADO.ReqRoomId = this.currentModule.RoomId;

                HisPrepareResultSDO _result = new HisPrepareResultSDO();

                Inventec.Common.Logging.LogSystem.Info("Du lieu dau vao (api/HisPrepare/Approve) HisPrepareApproveSDO:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO), inputADO));

                _result = new BackendAdapter(param).Post<HisPrepareResultSDO>("api/HisPrepare/Approve", ApiConsumers.MosConsumer, inputADO, param);

                if (_result != null)
                {
                    success = true;
                    this.actionType = ActionType.UPDATE;
                    btnSave.Enabled = false;
                    this.refeshReference();
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFormData()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_Click(object sender, EventArgs e)
        {
            try
            {
                var focus = (MetyMatyADO)gridViewProcess.GetFocusedRow();
                if (focus != null)
                {
                    //this.actionType = ActionType.UPDATE;
                    //EnableControlChanged(this.actionType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void gridViewProcess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var dataSource = (List<MetyMatyADO>)gridControlProcess.DataSource;
                // reset
                Parallel.ForEach(dataSource, l => l.ApprovalAmount = 0);

                var selected = gridViewProcess.GetSelectedRows();
                foreach (var item in selected)
                {
                    var selectRow = (MetyMatyADO)gridViewProcess.GetRow(item);
                    if (selectRow == null)
                        continue;

                    Parallel.ForEach(dataSource.Where(f => f.ID == selectRow.ID), l => l.ApprovalAmount = l.ReqAmount);
                }

                gridViewProcess.BeginDataUpdate();
                gridViewProcess.GridControl.DataSource = dataSource;
                foreach (var item in selected)
                {
                    gridViewProcess.SelectRow(item);
                }
                gridViewProcess.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MetyMatyADO pData = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "REQ_NAME_STR")
                    {
                        e.Value = pData.REQ_LOGINNAME + " - " + pData.REQ_USERNAME;
                    }
                    else if (e.Column.FieldName == "APPROVAL_NAME_STR")
                    {
                        e.Value = pData.APPROVAL_LOGINNAME + " - " + pData.APPROVAL_USERNAME;
                    }
                    gridControlView.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewProcess_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsMedicine)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (MetyMatyADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IsMedicine)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
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
