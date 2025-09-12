using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisHeinPatientType.Validtion;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
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


namespace HIS.Desktop.Plugins.HisHeinPatientType
{
    public partial class frmHisHeinPatientType : HIS.Desktop.Utility.FormBase
    {
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE currentData;
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> lstTreatmentType;
        public frmHisHeinPatientType(Inventec.Desktop.Common.Modules.Module moduleData) : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                pagingGrid = new PagingGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisHeinPatientType_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Show()
        {
            try
            {
                FillDataToControl();
                SetSpinEditDefaultNull(sprinNumOrder);
                LoadRightRouteType(cboRightRouteTypeCode);
                InitCheck(cboTreatmentType, SelectionGrid__Status);

                FillTreatmentType();

                InitCombo(cboTreatmentType, lstTreatmentType, "TREATMENT_TYPE_NAME", "ID");
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidateForm()
        {
            try
            {
                ValidationControlTextHeinPatientTypeCode();
                ValidationControlTextDescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogTime.Warn(ex);
            }
        }

        private void ValidationControlTextHeinPatientTypeCode()
        {
            try
            {
                ValidMaxlengthtxtHeinPatientType validRule = new ValidMaxlengthtxtHeinPatientType();
                validRule.txtHeinPatientType = txtHeinPatientTypeCode;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtHeinPatientTypeCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextDescription()
        {
            try
            {
                ValidMaxlengthtxtDescription validRule = new ValidMaxlengthtxtDescription();
                validRule.txtDescription = txtDescription;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtDescription, validRule);       
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
                    pageSize = (int)ConfigApplications.NumPageSize;
                }

                LoadPaging(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pageSize, this.gridControlHeinPatientType);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>> apiResult = null;
                HisHeinPatientTypeFilter filter = new HisHeinPatientTypeFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewHeinPatientType.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>)apiResult.Data;
                    if (data != null)
                    {
                        gridViewHeinPatientType.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewHeinPatientType.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisHeinPatientTypeFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogTime.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                txtDescription.Text = null;
                txtHeinPatientTypeCode.Text = null;
                chkDT.Checked = false;
                chkTT.Checked = false;
                txtRightRouteTypeCode.Text = null;
                cboRightRouteTypeCode.EditValue = null;
                sprinNumOrder.EditValue = null;
                cboTreatmentType.EditValue = null;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError
                (dxValidationProvider1, dxErrorProvider1);               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE updateDTO = new MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE();


                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        txtHeinPatientTypeCode.Focus();
                    }
                }     
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;

                        FillDataToControl();
                    }
                }

                if (success)
                {
                    btnReset_Click(null, null);
                    SetFocusEditor();
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

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE updateDTO)
        {
            try        
            {
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;

                var selectedItems = gridCheckMark.Selection.OfType<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().ToList();

                if (selectedItems.Any())
                {
                    updateDTO.TREATMENT_TYPE_IDS = string.Join(",", selectedItems.Select(x => x.ID));
                }

                updateDTO.HEIN_PATIENT_TYPE_CODE = txtHeinPatientTypeCode.Text.Trim();
                updateDTO.DESCRIPTION = txtDescription.Text.Trim();
                if (chkDT.Checked)
                {
                    updateDTO.RIGHT_ROUTE_CODE = "DT";
                }
                else if (chkTT.Checked)
                {
                    updateDTO.RIGHT_ROUTE_CODE = "TT";
                }

                if (!string.IsNullOrEmpty(txtRightRouteTypeCode.Text))
                {
                    updateDTO.RIGHT_ROUTE_TYPE_CODE = txtRightRouteTypeCode.Text;
                }

                updateDTO.NUM_ORDER = Convert.ToInt64(sprinNumOrder.EditValue ?? 0);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinPatientTypeFilter filter = new HisHeinPatientTypeFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlHeinPatientType_DoubleClick(object sender, EventArgs e)
        {
            
        }
        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);

                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data)
        {
            try
            {
                if (data != null)
                {
                    txtHeinPatientTypeCode.Text = data.HEIN_PATIENT_TYPE_CODE;
                    txtDescription.Text = data.DESCRIPTION;
                    if (data.RIGHT_ROUTE_CODE == "TT")
                    {
                        chkTT.Checked = true;
                    }
                    else if (data.RIGHT_ROUTE_CODE == "DT")
                    {
                        chkDT.Checked = true;
                    }
                    sprinNumOrder.EditValue = data.NUM_ORDER;
                    if (!string.IsNullOrEmpty(data.RIGHT_ROUTE_TYPE_CODE))
                    {
                        cboRightRouteTypeCode.EditValue = data.RIGHT_ROUTE_TYPE_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewHeinPatientType_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGUnLock);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGDelete : btnGEnable);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewHeinPatientType_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE pData = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
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

                gridControlHeinPatientType.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE success = new MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE();
            bool notHandler = false;
            try
            {

                MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)gridViewHeinPatientType.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data1 = new MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGUnLock_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE success = new MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE();
            bool notHandler = false;
            try
            {
                MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)gridViewHeinPatientType.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE data1 = new MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_LOCK, ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToControl();
                    }
                    MessageManager.Show(this, param, notHandler);
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnGDelete_ButtonPressed(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)gridViewHeinPatientType.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {


                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_HEIN_PATIENT_TYPE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE>)gridControlHeinPatientType.DataSource).FirstOrDefault();


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

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewHeinPatientType.Focus();
                    gridViewHeinPatientType.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)gridViewHeinPatientType.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinPatientTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    if (this.ActionType == GlobalVariables.ActionEdit)
                        btnEdit.Focus();
                }

                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlHeinPatientType_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_HEIN_PATIENT_TYPE)gridViewHeinPatientType.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkTT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTT.Checked)
                {
                    chkDT.Checked = false;
                    txtRightRouteTypeCode.Enabled = false;
                    txtRightRouteTypeCode.Text = "";
                    cboRightRouteTypeCode.Enabled = false;
                    cboRightRouteTypeCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkDT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDT.Checked)
                {
                    chkTT.Checked = false;
                    txtRightRouteTypeCode.Enabled = true;
                    cboRightRouteTypeCode.Enabled = true;
                    LoadRightRouteType(cboRightRouteTypeCode);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadRightRouteType(GridLookUpEdit cbo)
        {
            var data = new List<dynamic>
            {
                new { Code = "CC", Name = "Cấp cứu" },
                new { Code = "GT", Name = "Giới thiệu" },
                new { Code = "HK", Name = "Hẹn khám" }
            };

            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("Code", "", 150, 1));
            columnInfos.Add(new ColumnInfo("Name", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Code", columnInfos, false, 250);
            ControlEditorLoader.Load(cbo, data, controlEditorADO);
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
                    gridCheckMark.ClearSelection(cbo.Properties.View);
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

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                cboTreatmentType.RefreshEditValue();
                lstTreatmentType = new List<HIS_TREATMENT_TYPE>();
                foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        lstTreatmentType.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillTreatmentType()
        {
            //lstTreatmentType = new List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentTypeFilter filter = new HisTreatmentTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                lstTreatmentType = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>>(HisRequestUriStore.HIS_TREATMENT_TYPE_GET, ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRightRouteTypeCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboRightRouteTypeCode.EditValue != null)
                {
                    txtRightRouteTypeCode.Text = cboRightRouteTypeCode.EditValue.ToString();
                }
                else
                {
                    txtRightRouteTypeCode.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                if (cbo == null) return;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;

                var selectedItems = gridCheckMark.Selection.OfType<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>().ToList();

                if (selectedItems.Count > 0)
                {
                    string statusName = string.Join(", ", selectedItems.Select(x => x.TREATMENT_TYPE_NAME));
                    e.DisplayText = statusName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetSpinEditDefaultNull(DevExpress.XtraEditors.SpinEdit spinEdit)
        {
            spinEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            spinEdit.Properties.NullText = string.Empty;
            spinEdit.Properties.MinValue = 0;
            spinEdit.EditValue = null;
            spinEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
            spinEdit.Properties.Mask.EditMask = "n0"; // số nguyên
            spinEdit.Properties.Mask.UseMaskAsDisplayFormat = true;
        }

        private void sprinNumOrder_Spin(object sender, DevExpress.XtraEditors.Controls.SpinEventArgs e)
        {
            if (!e.IsSpinUp && sprinNumOrder.Value <= sprinNumOrder.Properties.MinValue)
            {
                e.Handled = true;
            }
        }

        private void sprinNumOrder_Validating(object sender, CancelEventArgs e)
        {
        }

        private void sprinNumOrder_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '-' || e.KeyChar == '+')
            {
                e.Handled = true;
            }
        }
    }
}
