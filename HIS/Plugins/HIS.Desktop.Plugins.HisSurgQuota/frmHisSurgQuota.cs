using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LibraryMessage;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisSurgQuota
{
    public partial class frmHisSurgQuota : XtraForm
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<HIS_EXECUTE_ROLE> listExecuteRole = new List<HIS_EXECUTE_ROLE>();
        List<HIS_SERVICE_TYPE> listServiceType = new List<HIS_SERVICE_TYPE>();
        List<HIS_PTTT_GROUP> listPtttGroup = new List<HIS_PTTT_GROUP>();
        V_HIS_SURG_QUOTA currentSurgQuota = null;
        List<HIS_SURG_QUOTA_DETAIL> listDetail = new List<HIS_SURG_QUOTA_DETAIL>();
        List<V_HIS_SURG_QUOTA> listQuota = new List<V_HIS_SURG_QUOTA>();
        bool isSelectedNoActive = false;
        private int oldDataCount = 0;
        public frmHisSurgQuota(Inventec.Desktop.Common.Modules.Module module)
        {
            this.currentModule = module;
            InitializeComponent();
        }

        private void frmHisSurgQuota_Load(object sender, EventArgs e)
        {
            try
            {
                InitData();
                LoadRequiredControl();
                SetDefaultControl();
                SetCaptionByLanguageKey();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisSurgQuota.Resources.Lang", typeof(frmHisSurgQuota).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControlItem4.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroupPttt.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.cboGroupPttt.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboVaitro.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.cboVaitro.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.Text = Inventec.Common.Resource.Get.Value("frmHisSurgQuota.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #region init data
        private void SetDefaultControl()
        {
            try
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
                btnEdit.Enabled = false;
                cboType.EditValue = null;
                cboGroupPttt.EditValue = null;
                cboVaitro.EditValue = null;
                txtChuyenKhoa.Text = txtSearchValue.Text = txtVaitro.Text = null;
                spinQuota.EditValue = null;
                gridControlSurgQuotaDetail.DataSource = null;
                dxErrorProvider1.ClearErrors();
                this.currentSurgQuota = null;
                if(this.listQuota != null)
                {
                    LoadAllDetail(this.listQuota.Select(s => s.ID).ToList());
                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void EnableControlChange()
        {
            try
            {
                btnSave.Enabled = false;
                btnAdd.Enabled = btnEdit.Enabled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void LoadRequiredControl()
        {
            try
            {
                SetRequiredControl(cboGroupPttt);
                SetRequiredControl(txtChuyenKhoa);
                SetRequiredControl(cboType);
                SetRequiredControl2(txtVaitro);
                SetRequiredControl2(spinQuota);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetRequiredControl(BaseEdit control)
        {
            try
            {
                if (control == null) return;


                //validate
                ValidationSingleControl(control);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void SetRequiredControl2(BaseEdit control)
        {
            try
            {
                if (control == null) return;
                control.ForeColor = Color.Maroon;

                //validate
                ValidationSingleControl2(control);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationSingleControl2(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider2.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitData()
        {
            try
            {
                LoadDataToGrid();

                LoadDataTocboType();
                LoadDataTocboGroup();
                LoadDataTocboRole();


            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void LoadAllDetail(List<long> listID)
        {
            try
            {

                HisSurgQuotaDetailFilter filter = new HisSurgQuotaDetailFilter();
                filter.SURG_QUOTA_IDs = listID;
                var rs = new BackendAdapter(new CommonParam()).Get<List<HIS_SURG_QUOTA_DETAIL>>("api/HisSurgQuotaDetail/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                if (rs != null)
                {
                    listDetail = rs;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void LoadDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SURG_QUOTA>> apiResult = null;
                HisSurgQuotaViewFilter filter = new MOS.Filter.HisSurgQuotaViewFilter();

                UpdateFilter(ref filter);
                gridControlSurgQuota.BeginUpdate();
                apiResult = new BackendAdapter(param).GetRO<List<V_HIS_SURG_QUOTA>>("api/HisSurgQuota/GetView", ApiConsumers.MosConsumer, filter, param);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null)
                    {
                        gridControlSurgQuota.DataSource = data;
                        gridControlSurgQuota.EndUpdate();
                        listQuota = data.ToList(); ;
                        LoadAllDetail(data.Select(s => s.ID).ToList());
                    }
                }
                WaitingManager.Hide();
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateFilter(ref HisSurgQuotaViewFilter filter)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtSearchValue.Text))
                {
                    filter.KEY_WORD = txtSearchValue.Text.Trim();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridDetail(long ID)
        {
            try
            {
                if (ID > 0)
                {

                    gridControlSurgQuotaDetail.BeginUpdate();
                    var data = listDetail.Where(s => s.SURG_QUOTA_ID == ID);
                    if (data != null)
                    {
                        gridControlSurgQuotaDetail.DataSource = data;

                    }
                    gridControlSurgQuotaDetail.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>();
                if (data != null)
                {
                    listServiceType = data.ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 40, 1));
                    columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 190, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 200);
                    ControlEditorLoader.Load(cboType, data, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboGroup()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_PTTT_GROUP>();
                if (data != null)
                {
                    listPtttGroup = data.ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 40, 1));
                    columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 190, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 200);
                    ControlEditorLoader.Load(cboGroupPttt, data, controlEditorADO);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboRole()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_EXECUTE_ROLE>();
                if (data != null)
                {
                    data = data.Where(s => s.IS_ACTIVE == 1 && (s.IS_DISABLE_IN_EKIP != 1 || s.IS_DISABLE_IN_EKIP == null)).ToList();
                    listExecuteRole = data.ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_CODE", "", 40, 1));
                    columnInfos.Add(new ColumnInfo("EXECUTE_ROLE_NAME", "", 190, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROLE_NAME", "ID", columnInfos, false, 200);
                    ControlEditorLoader.Load(cboVaitro, data, controlEditorADO);
                    ControlEditorLoader.Load(grdCboRole, data, controlEditorADO);
                    grdCboRole.DisplayMember = "EXECUTE_ROLE_NAME";
                    grdCboRole.ValueMember = "ID";
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion
        #region grid
        private void gridViewSurgQuota_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SURG_QUOTA pData = (V_HIS_SURG_QUOTA)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (pData.IS_ACTIVE == 1)
                        {
                            e.Value = "Hoạt động";
                        }
                        else
                            e.Value = "Tạm khóa";
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "SERVICE_TYPE_NAME")
                    {
                        var data = listServiceType.Where(s => s.ID == pData.SERVICE_TYPE_ID).FirstOrDefault();
                        if (data != null)
                        {
                            e.Value = data.SERVICE_TYPE_NAME;
                        }
                    }
                    else if (e.Column.FieldName == "PTTT_GROUP_NAME")
                    {
                        var data = listPtttGroup.Where(s => s.ID == pData.PTTT_GROUP_ID).FirstOrDefault();
                        if (data != null)
                        {
                            e.Value = data.PTTT_GROUP_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void gridViewSurgQuota_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_SURG_QUOTA data = (V_HIS_SURG_QUOTA)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == 0 ? btnLockE : btnUnlockE);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {

                        e.RepositoryItem = (data.IS_ACTIVE == 0 ? btnDeleteD : btnDeleteE);

                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }
        private void gridViewSurgQuota_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                V_HIS_SURG_QUOTA data = (V_HIS_SURG_QUOTA)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == 0)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }

            }
        }

        private void gridViewSurgQuota_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_SURG_QUOTA)gridViewSurgQuota.GetFocusedRow();
                if (rowData != null)
                {
                    currentSurgQuota = rowData;
                    FillDataToEditControl(rowData);
                    LoadDataToGridDetail(rowData.ID);

                    if (rowData.IS_ACTIVE == 1)
                    {
                        EnableControlChange();
                        isSelectedNoActive = false;
                    }
                    else
                    {
                        btnAdd.Enabled = btnSave.Enabled = btnEdit.Enabled = false;
                        isSelectedNoActive = true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void FillDataToEditControl(V_HIS_SURG_QUOTA rowData)
        {
            try
            {
                if(rowData != null)
                {
                    cboType.EditValue = rowData.SERVICE_TYPE_ID;
                    cboGroupPttt.EditValue = rowData.PTTT_GROUP_ID;
                    txtChuyenKhoa.Text = rowData.SPECIALITY_KEY;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnLockE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_SURG_QUOTA)gridViewSurgQuota.GetFocusedRow();
                if (rowData != null)
                {

                    bool success = false;
                    CommonParam param = new CommonParam();
                    ChangeLock(param, ref success);
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void ChangeLock(CommonParam param, ref bool success)
        {
            try
            {
                var rowData = (V_HIS_SURG_QUOTA)gridViewSurgQuota.GetFocusedRow();
                if (rowData != null)
                {
                    LogSystem.Debug("_________Goi api cap nhat du lieu(change lock). du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA", rowData));
                    var rs = new BackendAdapter(param).Post<HIS_SURG_QUOTA>("api/HisSurgQuota/ChangeLock", ApiConsumers.MosConsumer, rowData.ID, param);
                    if (rs != null)
                    {
                        success = true;
                        LoadDataToGrid();
                        SetDefaultControl();

                    }

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void btnDeleteE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_SURG_QUOTA)gridViewSurgQuota.GetFocusedRow();
                if (rowData != null)
                {
                    CommonParam param = new CommonParam();
                    if (MessageBox.Show(this, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        var exitsDetail = listDetail.Where(s => s.SURG_QUOTA_ID == rowData.ID).FirstOrDefault();
                        if (exitsDetail != null)
                        {
                            MessageBox.Show(this, "Tồn tại danh sách chi tiết ứng với dữ liệu đang chọn.", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
                        LogSystem.Debug("_________Goi api cap nhat du lieu (xoa). du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA", rowData));
                        //xoa du lieu

                        bool rs = new BackendAdapter(param).Post<bool>("api/HisSurgQuota/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                        MessageManager.Show(this, param, rs);
                        if (rs)
                        {
                            LoadDataToGrid();
                            SetDefaultControl();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnUnlockE_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (V_HIS_SURG_QUOTA)gridViewSurgQuota.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    ChangeLock(param, ref success);
                    MessageManager.Show(this, param, success);

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region grid detail
        private void gridViewSurgQuotaDetail_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    HIS_SURG_QUOTA_DETAIL pData = (HIS_SURG_QUOTA_DETAIL)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "EXECUTE_ROLE_NAME")
                    {
                        var role = this.listExecuteRole.FirstOrDefault(s => s.ID == pData.EXECUTE_ROLE_ID);
                        if (role != null)
                        {
                            e.Value = role.EXECUTE_ROLE_NAME;

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void gridViewSurgQuotaDetail_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewSurgQuotaDetail_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    HIS_SURG_QUOTA_DETAIL data = (HIS_SURG_QUOTA_DETAIL)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "EDIT")
                    {
                        e.RepositoryItem = ((data.IS_ACTIVE == 0 || isSelectedNoActive) ? btnEditDetailD : btnEditDetail);

                    }

                    if (e.Column.FieldName == "DELETE")
                    {

                        e.RepositoryItem = ((data.IS_ACTIVE == 0 || isSelectedNoActive) ? btnDeleteDetailD : btnDeleteDetail);

                    }
                    

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void btnEditDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_SURG_QUOTA_DETAIL rowData = (HIS_SURG_QUOTA_DETAIL)gridViewSurgQuotaDetail.GetFocusedRow();
                if (rowData != null)
                {
                    if (dxErrorProvider2.HasErrors)
                    {

                        MessageBox.Show(this, "Đã tồn tại vai trò tương tự.", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                    if (dxErrorProvider3.HasErrors)
                    {
                        MessageBox.Show(this, "Số lượng không hợp lệ.", "Thông báo", MessageBoxButtons.OK);
                        return;
                    }
                    
                    bool success = false;
                    LogSystem.Debug("_________Goi api cap nhat du lieu(Update). du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA_DETAIL", rowData));
                    var res = new BackendAdapter(param).Post<HIS_SURG_QUOTA_DETAIL>("api/HisSurgQuotaDetail/Update", ApiConsumers.MosConsumer, rowData, param);
                    if (res != null)
                    {
                        success = true;
                        LoadAllDetail(this.listQuota.Select(s => s.ID).ToList());
                        LoadDataToGridDetail(rowData.SURG_QUOTA_ID);
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnDeleteDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                HIS_SURG_QUOTA_DETAIL rowData = (HIS_SURG_QUOTA_DETAIL)gridViewSurgQuotaDetail.GetFocusedRow();
                if (rowData != null)
                {
                    if (MessageBox.Show(this, MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        return;
                    long current_id = rowData.SURG_QUOTA_ID;
                    bool success = false;
                    LogSystem.Debug("_________Goi api cap nhat du lieu(DELETE). du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA_DETAIL", rowData));
                    var res = new BackendAdapter(param).Post<bool>("api/HisSurgQuotaDetail/Delete", ApiConsumers.MosConsumer, rowData.ID, param);
                    if (res)
                    {
                        success = true;
                        LoadAllDetail(this.listQuota.Select(s => s.ID).ToList());
                        LoadDataToGridDetail(current_id);
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }




        private void gridSpinDM_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SpinEdit edit = sender as SpinEdit;
                if (edit != null)
                {
                    var data = (HIS_SURG_QUOTA_DETAIL)gridViewSurgQuotaDetail.GetFocusedRow();
                    // Set the old value if not yet initialized
                    if (edit.OldEditValue != null)
                    {
                        oldDataCount = Convert.ToInt32(edit.OldEditValue);
                    }

                    // Check if the new value is different from the old value
                    if ((edit.EditValue ?? 0).ToString() != oldDataCount.ToString())
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = gridViewSurgQuotaDetail as DevExpress.XtraGrid.Views.Grid.GridView;
                        

                        // Check if the new value is invalid (negative)
                        if (Convert.ToInt64(edit.EditValue) < 0)
                        {
                            dxErrorProvider3.SetError(edit, "Số lượng không hợp lệ.", ErrorType.Warning);

                            // Revert to the old valid value
                            data.QUOTA_COUNT = oldDataCount;
                        }
                        else
                        {
                            // Clear any error and update the data object with the new value
                            dxErrorProvider3.SetError(edit, "", ErrorType.None);
                            data.QUOTA_COUNT = Convert.ToInt64(edit.EditValue);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



        private void grdCboVaitro_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            
        }
        #endregion
        private void txtVaitro_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtVaitro.Text))
                {
                    var rs = listExecuteRole.Where(s => s.EXECUTE_ROLE_CODE == txtVaitro.Text).FirstOrDefault();
                    if (rs != null)
                    {
                        txtVaitro.Text = rs.EXECUTE_ROLE_CODE;
                        cboVaitro.EditValue = rs.ID;
                        btnAdd.Focus();
                    }
                    else
                    {
                        txtVaitro.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtVaitro_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtVaitro_TextChanged(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboVaitro_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboVaitro.EditValue != null)
                {
                    var rs = listExecuteRole.Where(s => s.ID == Convert.ToInt64(cboVaitro.EditValue)).FirstOrDefault();
                    if (rs != null)
                    {
                        txtVaitro.Text = rs.EXECUTE_ROLE_CODE;
                        cboVaitro.EditValue = rs.ID;
                        btnAdd.Focus();
                    }
                    else
                    {
                        cboVaitro.ShowPopup();
                    }
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
                LoadDataToGrid();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                ProccessSave(GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ProccessSave(GlobalVariables.ActionAdd);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }

        }
        private void ProccessSave(int action)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!dxValidationProvider1.Validate()) return;
                HIS_SURG_QUOTA data = null;
                if (this.currentSurgQuota != null)
                {
                    data = new HIS_SURG_QUOTA();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SURG_QUOTA>(data, this.currentSurgQuota);

                }
                else
                    data = new HIS_SURG_QUOTA();
                data.SERVICE_TYPE_ID = Convert.ToInt64(cboType.EditValue);
                data.PTTT_GROUP_ID = Convert.ToInt64(cboGroupPttt.EditValue);
                data.SPECIALITY_KEY = txtChuyenKhoa.Text;
                bool success = false;
                WaitingManager.Show();
                if (LocalStorage.LocalData.GlobalVariables.ActionAdd == action)
                {
                    // them moi
                    LogSystem.Debug("_________Goi api tao moi du lieu. du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA", data));
                    var rs = new BackendAdapter(param).Post<HIS_SURG_QUOTA>("api/HisSurgQuota/Create", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        LoadDataToGrid();
                        SetDefaultControl();
                    }
                }
                else
                {
                    //update
                    LogSystem.Debug("_________Goi api cap nhat du lieu. du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA", data));
                    var rs = new BackendAdapter(param).Post<HIS_SURG_QUOTA>("api/HisSurgQuota/Update", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        LoadDataToGrid();
                        SetDefaultControl();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControl();

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!dxValidationProvider2.Validate()) return;
                if (dxErrorProvider1.HasErrors) return;
                HIS_SURG_QUOTA_DETAIL data = new HIS_SURG_QUOTA_DETAIL();
                data.SURG_QUOTA_ID = this.currentSurgQuota.ID;
                data.EXECUTE_ROLE_ID = Convert.ToInt64(cboVaitro.EditValue);
                data.QUOTA_COUNT = Convert.ToInt64(spinQuota.EditValue);
                WaitingManager.Show();
                LogSystem.Debug("_________Goi api tao moi  du lieu. du lieu dau vao: " + LogUtil.TraceData("HIS_SURG_QUOTA_DETAIL", data));
                var rs = new BackendAdapter(param).Post<HIS_SURG_QUOTA_DETAIL>("api/HisSurgQuotaDetail/Create", ApiConsumers.MosConsumer, data, param);
                bool success = false;
                if (rs != null)
                {
                    success = true;
                    LoadAllDetail(this.listQuota.Select(s => s.ID).ToList());
                    LoadDataToGridDetail(rs.SURG_QUOTA_ID);
                    cboVaitro.EditValue = null;
                    txtVaitro.Text = null;
                    spinQuota.EditValue = null;

                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void frmHisSurgQuota_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.F)
                {
                    btnFind.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.N)
                {
                    btnSave.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.S)
                {
                    btnEdit.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.A)
                {
                    btnAdd.PerformClick();
                }
                if (e.Control && e.KeyCode == Keys.R)
                {
                    btnReset.PerformClick();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void spinQuota_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (Convert.ToInt64(spinQuota.EditValue) < 0)
                {
                    dxErrorProvider1.SetError(spinQuota, "Số lượng không hợp lệ.", ErrorType.Warning);

                }
                else
                {
                    dxErrorProvider1.SetError(spinQuota, "", ErrorType.None);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdCboRole_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                GridLookUpEdit edit = sender as GridLookUpEdit;
                if (edit == null) return;
                if (edit.EditValue != null)
                {

                    if ((edit.EditValue ?? 0).ToString() != (edit.OldEditValue ?? 0).ToString())
                    {
                        dxErrorProvider2.SetError(edit, "", ErrorType.None);
                        var role = Convert.ToInt64(edit.EditValue);
                        DevExpress.XtraGrid.Views.Grid.GridView view = gridViewSurgQuotaDetail as DevExpress.XtraGrid.Views.Grid.GridView;
                        var rowData = (HIS_SURG_QUOTA_DETAIL)gridViewSurgQuotaDetail.GetFocusedRow();
                        var exitRole = listDetail.FirstOrDefault(s => s.EXECUTE_ROLE_ID == role && s.SURG_QUOTA_ID == rowData.SURG_QUOTA_ID && s.ID != rowData.ID);
                        if (exitRole != null)
                        {
                            rowData.EXECUTE_ROLE_ID = Convert.ToInt64(edit.OldEditValue);
                            dxErrorProvider2.SetError(edit, "Đã tồn tại vai trò tương tự.", ErrorType.Warning);
                            return;
                        }
                        

                        rowData.EXECUTE_ROLE_ID = Convert.ToInt64(edit.EditValue);
                        view.SetRowCellValue(view.FocusedRowHandle, view.Columns["EXECUTE_ROLE_NAME"], rowData.EXECUTE_ROLE_ID);
                        edit.EditValue = rowData.EXECUTE_ROLE_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboType.EditValue = null;
                    cboType.Text = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboGroupPttt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboGroupPttt.EditValue = null;
                    cboGroupPttt.Text = null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void cboVaitro_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboVaitro.EditValue = null;
                    cboVaitro.Text = null;
                }
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
                if(e.KeyCode == Keys.Enter)
                {
                    btnFind.PerformClick();
                }
                if(e.Control && e.KeyCode == Keys.F)
                {
                    btnFind.PerformClick();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        
        private void gridViewSurgQuotaDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                // Check if the changed column is QUOTA_COUNT
                if (e.Column.FieldName == "QUOTA_COUNT")
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    decimal newQuotaCount = Convert.ToDecimal(view.GetRowCellValue(e.RowHandle, e.Column));

                    // If the new value is negative, revert to the previous value
                    if (newQuotaCount < 0)
                    {
                        // Display an error message
                        dxErrorProvider3.SetError(view.GridControl, "Số lượng không hợp lệ", ErrorType.Warning);

                        // Reset the QUOTA_COUNT to the previous value
                        view.SetRowCellValue(e.RowHandle, e.Column, oldDataCount);
                    }
                    else
                    {
                        // Clear any existing error message
                        dxErrorProvider3.SetError(view.GridControl, "");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }
    }
}
