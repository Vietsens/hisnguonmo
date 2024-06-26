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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisBloodTypePreparations.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBloodTypePreparations.Run
{
    public partial class frmHisBloodTypePreparations : FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        HIS_BLTY_PREPARATIONS currentData;
        public frmHisBloodTypePreparations(Inventec.Desktop.Common.Modules.Module module) : base(module)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboFind_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo != null && e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) { cbo.EditValue = null; }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        List<HIS_BLOOD_ABO> lstBloodAbo = new List<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>();
        List<HIS_BLOOD_RH> lstBloodRh = new List<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>();
        List<HIS_PREPARATIONS_BLOOD> lstPreparationsBlood = new List<MOS.EFMODEL.DataModels.HIS_PREPARATIONS_BLOOD>();
        private void frmHisBloodTypePreparations_Load(object sender, EventArgs e)
        {

            try
            {
                FillDataToControl();
                lstBloodAbo = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_ABO>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                lstBloodRh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BLOOD_RH>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                lstPreparationsBlood = BackendDataWorker.Get<HIS_PREPARATIONS_BLOOD>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                InitCbo(cboProvideBloodAbo, lstBloodAbo, "", "BLOOD_ABO_CODE", "BLOOD_ABO_CODE");
                InitCbo(cboProvideRh, lstBloodRh, "", "BLOOD_RH_CODE", "BLOOD_RH_CODE");
                InitCbo(cboPreparationBlood, lstPreparationsBlood, "PREPARATIONS_BLOOD_CODE", "PREPARATIONS_BLOOD_NAME", "ID");
                InitCbo(cboFind, lstPreparationsBlood, "PREPARATIONS_BLOOD_CODE", "PREPARATIONS_BLOOD_NAME", "ID");
                InitCbo(cboReceiveBloodAbo, lstBloodAbo, "", "BLOOD_ABO_CODE", "BLOOD_ABO_CODE");
                InitCbo(cboReciveRh, lstBloodRh, "", "BLOOD_RH_CODE", "BLOOD_RH_CODE");

                ValidationGridControls(cboProvideBloodAbo);
                ValidationGridControls(cboProvideRh);
                ValidationGridControls(cboPreparationBlood);
                ValidationGridControls(cboReceiveBloodAbo);
                ValidationGridControls(cboReciveRh);
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationGridControls(GridLookUpEdit cbo)
        {
            try
            {
                ValidationGridControls valid = new ValidationGridControls();
                valid.cbo = cbo;
                valid.ErrorText = "Trường dữ liệu bắt buộc";
                valid.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(cbo, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCbo(GridLookUpEdit cbo, object data, string DisplayMemberCode, string DisplayMemberName, string ValueMember)
        {
            try
            {

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!string.IsNullOrEmpty(DisplayMemberCode))
                    columnInfos.Add(new ColumnInfo(DisplayMemberCode, "Mã", 150, 1));
                columnInfos.Add(new ColumnInfo(DisplayMemberName, "Tên", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(DisplayMemberName, ValueMember, columnInfos, true, 400);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
                cbo.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();


                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControl1);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<HIS_BLTY_PREPARATIONS>> apiResult = null;
                HisBltyPreparationsFilter filter = new HisBltyPreparationsFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<HIS_BLTY_PREPARATIONS>>("api/HisBltyPreparations/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_BLTY_PREPARATIONS>)apiResult.Data;
                    if (data != null)
                    {
                        gridView1.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView1.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisBltyPreparationsFilter filter)
        {
            try
            {
                if (cboFind.EditValue != null)
                    filter.PREPARATIONS_BLOOD_ID = Int64.Parse(cboFind.EditValue.ToString());
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {

            try
            {
                currentData = null;
                cboProvideBloodAbo.EditValue = null;
                cboProvideRh.EditValue = null;
                cboPreparationBlood.EditValue = null;
                cboReceiveBloodAbo.EditValue = null;
                cboReciveRh.EditValue = null;
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void EnableControlChanged(int action)
        {
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_BLTY_PREPARATIONS pData = (HIS_BLTY_PREPARATIONS)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }
                    else if (e.Column.FieldName == "PREPARATIONS_BLOOD_STR")
                    {
                        e.Value = lstPreparationsBlood.FirstOrDefault(o => o.ID == pData.PREPARATIONS_BLOOD_ID).PREPARATIONS_BLOOD_NAME;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {

                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.CREATE_TIME);

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)pData.MODIFY_TIME);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {



        }

        private void repDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_BLTY_PREPARATIONS)gridView1.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>("api/HisBltyPreparations/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                        }

                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            FillDataToControl();
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }
        private void SaveProcess()
        {
            CommonParam param = new CommonParam();

            try
            {
                bool success = false;
                if (!btnEdit.Enabled && !btnAdd.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_BLTY_PREPARATIONS obj = new HIS_BLTY_PREPARATIONS();
                if (currentData != null)
                    obj.ID = currentData.ID;
                obj.PROVIDE_BLOOD_ABO_CODE = (string)cboProvideBloodAbo.EditValue;
                obj.PROVIDE_BLOOD_RH_CODE = (string)cboProvideRh.EditValue;
                obj.PREPARATIONS_BLOOD_ID = (long)cboPreparationBlood.EditValue;
                obj.RECEIVE_BLOOD_ABO_CODE = (string)cboReceiveBloodAbo.EditValue;
                obj.RECEIVE_BLOOD_RH_CODE = (string)cboReciveRh.EditValue;

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    obj.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<HIS_BLTY_PREPARATIONS>("api/HisBltyPreparations/Create", ApiConsumers.MosConsumer, obj, param);
                    if (resultData != null)
                    {
                        success = true;
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<HIS_BLTY_PREPARATIONS>("api/HisBltyPreparations/Update", ApiConsumers.MosConsumer, obj, param);
                    if (resultData != null)
                    {
                        success = true;

                    }
                }
                if (success)
                {
                    FillDataToControl();
                    btnRefresh_Click(null, null);
                }


                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SaveProcess();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnFind_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled)
                btnEdit_Click(null, null);
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
                btnAdd_Click(null, null);
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnRefresh.Enabled)
                btnRefresh_Click(null, null);
        }

        private void gridView1_Click(object sender, EventArgs e)
        {
            try
            {

                var dt = (HIS_BLTY_PREPARATIONS)gridView1.GetFocusedRow();
                if (dt != null)
                {
                    currentData = dt;
                    cboProvideBloodAbo.EditValue = dt.PROVIDE_BLOOD_ABO_CODE;
                    cboProvideRh.EditValue = dt.PROVIDE_BLOOD_RH_CODE;
                    cboPreparationBlood.EditValue = dt.PREPARATIONS_BLOOD_ID;
                    cboReceiveBloodAbo.EditValue = dt.RECEIVE_BLOOD_ABO_CODE;
                    cboReciveRh.EditValue = dt.RECEIVE_BLOOD_RH_CODE;
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    btnEdit.Enabled = (dt.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
