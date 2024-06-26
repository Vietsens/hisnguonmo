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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Resources;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.HisAppointmentPeriod.Validate;
using HIS.Desktop.Plugins.HisAppointmentPeriod.Validation;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisAppointmentPeriod.HisAppointmentPeriod
{
    public partial class frmHisAppointmentPeriod : HIS.Desktop.Utility.FormBase
    {

        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD currentData;
        List<HIS_APPOINTMENT_PERIOD> ListHisAppointtmentPeriod = new List<HIS_APPOINTMENT_PERIOD>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        List<HIS_BRANCH> ListBrank = new List<HIS_BRANCH>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        long PeriodID;
        #endregion

        #region Construct
        public frmHisAppointmentPeriod(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region public method
        public void Show()
        {
            //load du lieu
            FillDataToControl();

            //Load Combo Co so
            ComboboxCoSo();


            this.ActionType = GlobalVariables.ActionAdd;
            //Set enable control default
            EnableControlChanged(this.ActionType);

            //Value default
            SetDefaultValue();

            //Load ngon ngu label control
            SetReroucesByLanguageKey();

            //Set tabindex control
            InitTabIndex();

            //Set validate
            SetVali();
        }
        #region Setvali
        private void SetVali()
        {
            try
            {
                ValidationSingleControl(cboCoSo, dxValidationProviderEditorInfo);
                ValidaSpinFrom(this.spinGioTu, this.spinGioDen);
                ValidaSpinTo(this.spinGioTu, this.spinGioDen);
                ValidaSpinMinuteFrom(this.spinGioDen, this.spinPhutDen);
                ValidaSpinMinuteFrom(this.spinGioTu, this.spinPhutTu);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidaSpinFrom(SpinEdit control1, SpinEdit control2)
        {
            ValidaSpinHours validRule = new ValidaSpinHours();
            validRule.spinFrom = control1;
            validRule.spinTo = control2;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProviderEditorInfo.SetValidationRule(control1, validRule);
        }

        private void ValidaSpinTo(SpinEdit control1, SpinEdit control2)
        {
            ValidaSpinHours validRule = new ValidaSpinHours();
            validRule.spinFrom = control1;
            validRule.spinTo = control2;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProviderEditorInfo.SetValidationRule(control2, validRule);
        }

        private void ValidaSpinMinuteFrom(SpinEdit controlHours, SpinEdit controlMinute)
        {
            ValidaSpinMinute validRule = new ValidaSpinMinute();
            validRule.spinHours = controlHours;
            validRule.spinMinute = controlMinute;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProviderEditorInfo.SetValidationRule(controlMinute, validRule);
        }
        #endregion
        #endregion

        #region Private method

        private void gridControlFormList_Load(object sender, EventArgs e)
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


        #region Load combobox Cơ sở
        private void ComboboxCoSo()
        {
            try
            {
                ListBrank = BackendDataWorker.Get<HIS_BRANCH>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));

                var CoSo = ListBrank.Where(s => s.IS_ACTIVE == 1).ToList();

                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboCoSo, CoSo, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        private void FillDataToControl()
        {

            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ham goi api lay du lieu phan trang
        /// </summary>
        /// <param name="param"></param>
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD>> apiResult = null;
                HisAppointmentPeriodFilter filter = new HisAppointmentPeriodFilter();
               

                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD>>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD>)apiResult.Data;
                    if (data != null)
                    {
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
    

        private void SetDefaultValue()
        {
            try
            {
                cboCoSo.EditValue = null;
                spinGioTu.EditValue = null;
                spinGioTu.Text = null;
                spinGioDen.EditValue = null;
                spinGioDen.Text = null;
                spinPhutTu.EditValue = null;
                spinPhutDen.EditValue = null;
                spinToiDa.EditValue = null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> ItemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexControl(ItemOrderTab, lcEditorInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetTabIndexControl(KeyValuePair<string, int> ItemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            try
            {
                bool success = false;
                if (!layoutControlEditor.IsInitialized) return;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit ba = lci.Control as BaseEdit;
                            if (ba != null)
                            {
                                if (ItemOrderTab.Key.Contains(ba.Name))
                                {
                                    ba.TabIndex = ItemOrderTab.Value;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    layoutControlEditor.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return;
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD data)
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
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(HIS_APPOINTMENT_PERIOD data)
        {
            try
            {
                if (data != null)
                {
                    PeriodID = data.ID;
                    cboCoSo.EditValue = data.BRANCH_ID;
                    spinGioTu.EditValue = data.FROM_HOUR;
                    spinPhutTu.EditValue = data.FROM_MINUTE;
                    spinGioDen.EditValue = data.TO_HOUR;
                    spinPhutDen.EditValue = data.TO_MINUTE;
                    spinToiDa.EditValue = data.MAXIMUM;
                }
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Loadcurrent(long currentid, ref HIS_APPOINTMENT_PERIOD currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisAppointmentPeriodFilter filter = new HisAppointmentPeriodFilter();
                filter.ID = currentid;
                currentDTO = new BackendAdapter(param).Get<List<HIS_APPOINTMENT_PERIOD>>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_GET,
                    ApiConsumers.SdaConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD currentDTO)
        {
            try
            {
                currentDTO.BRANCH_ID = (long)cboCoSo.EditValue;
                if (spinGioTu.Text == "")
                {
                    currentDTO.FROM_HOUR = null;
                }
                else
                {
                    currentDTO.FROM_HOUR = long.Parse(spinGioTu.Text);
                }
                if (spinPhutTu.Text == "")
                {
                    currentDTO.FROM_MINUTE = null;
                }
                else
                {
                    currentDTO.FROM_MINUTE = long.Parse(spinPhutTu.Text);
                }
                if (spinGioDen.Text == "")
                {
                    currentDTO.TO_HOUR = null;
                }
                else
                {
                    currentDTO.TO_HOUR = long.Parse(spinGioDen.Text);
                }
                if (spinPhutDen.Text == "")
                {
                    currentDTO.TO_MINUTE = null;
                }
                else
                {
                    currentDTO.TO_MINUTE = long.Parse(spinPhutDen.Text);
                }
                if (spinToiDa.Text == "")
                {
                    currentDTO.MAXIMUM = null;
                }
                else
                {
                    currentDTO.MAXIMUM = long.Parse(spinToiDa.Text);
                }

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
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD updateDTO = new MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    Loadcurrent(this.currentData.ID, ref updateDTO);
                }

                UpdateDTOFromDataForm(ref updateDTO);

                if (ActionType == GlobalVariables.ActionAdd)
                {
                    updateDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_CREATE,
                        ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToControl();
                        ResetFormData();
                    }
                }
                else
                {
                    if (PeriodID > 0)
                    {
                        updateDTO.ID = PeriodID;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_UPDATE,
                            ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToControl();
                        }
                    }
                }
                if (success)
                {
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

        //private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD data)
        //{
        //    try
        //    {
        //        if (data == null)
        //            throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD) is null");
        //        var rowData = (MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
        //        if (rowData != null)
        //        {
        //            Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD>(rowData, data);
        //            gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}


        private void SetFocusEditor()
        {
            try
            {
                //TODO
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
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.Text=null;
                            fomatFrm.EditValue = null;

                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

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

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                cboCoSo.EditValue = null;
                spinGioTu.Text = null;
                spinGioDen.Text = null;
                spinGioTu.EditValue = null;
                spinGioDen.EditValue = null;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                SetFocusEditor();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            btnEdit.Enabled = false;
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                    HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_DELETE,
                            ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<HIS_APPOINTMENT_PERIOD>)gridControlFormList.DataSource).FirstOrDefault();
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

        private void bbtnThem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
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
                gridviewFormList.Focus();
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFocusDefault_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboCoSo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboCoSo.EditValue == null)
                    {
                        cboCoSo.Focus();
                        cboCoSo.ShowPopup();
                    }
                    else
                    {
                        cboCoSo.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.currentData = (MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
                    if (this.currentData != null)
                    {
                        ChangedDataRow(this.currentData);

                        //Set focus vào control editor đầu tiên
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var Rowdata = (MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
                if (Rowdata != null)
                {
                    currentData = Rowdata;
                    ChangedDataRow(Rowdata);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD pData = (MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    if (e.Column.FieldName == "BRANCH_STR")
                    {
                        try
                        {
                            e.Value = ListBrank.Where(o => o.ID == pData.BRANCH_ID).ToList().Count > 0 ? ListBrank.Where(o => o.ID == pData.BRANCH_ID).First().BRANCH_NAME : "";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    if (e.Column.FieldName == "FROMTIME")
                    {
                        try
                        {
                            if (pData.FROM_HOUR != null)
                            {
                                e.Value = pData.FROM_HOUR + ":" + pData.FROM_MINUTE;
                            }                               
                            else if (pData.FROM_HOUR == null && pData.FROM_MINUTE != null)
                            {
                                e.Value = 0 + ":" + pData.FROM_MINUTE;
                            }
                            else
                            {
                                e.Value = null;
                            }                              
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot thoi gian tu", ex);
                        }
                    }
                    if (e.Column.FieldName == "TOTIME")
                    {
                        try
                        {
                            if (pData.TO_HOUR != null)
                            {
                                e.Value = pData.TO_HOUR + ":" + pData.TO_MINUTE;
                            }
                            else if (pData.TO_HOUR == null && pData.TO_MINUTE != null)
                            {
                                e.Value = 0 + ":" + pData.TO_MINUTE;
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot thoi gian den", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                e.Value = "Hoạt động";
                            }
                            else
                            {
                                e.Value = "Tạm khóa";
                            }                             
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            string createTime = (view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(createTime));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        try
                        {
                            string MODIFY_TIME = (view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString();
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(MODIFY_TIME));

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {         
            try
            {             
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        var isActive = (short?)view.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var isActive = (short?)view.GetRowCellValue(e.RowHandle, "IS_ACTIVE");
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGUnLock : btnGLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEnable : btnGDelete); 
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var Rowdata = (MOS.EFMODEL.DataModels.HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
                if (Rowdata != null)
                {
                    currentData = Rowdata;
                    ChangedDataRow(Rowdata);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_APPOINTMENT_PERIOD success = new HIS_APPOINTMENT_PERIOD();
            //bool notHandler = false;
            try
            {
                HIS_APPOINTMENT_PERIOD data = (HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                    HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_APPOINTMENT_PERIOD data1 = new HIS_APPOINTMENT_PERIOD();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_APPOINTMENT_PERIOD>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_LOCK,
                        ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        //notHandler = true;
                        BackendDataWorker.Reset<HIS_APPOINTMENT_PERIOD>();
                        FillDataToControl();
                    }
                    //MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGUnLock_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_APPOINTMENT_PERIOD success = new HIS_APPOINTMENT_PERIOD();
            //bool notHandler = false;
            try
            {
                HIS_APPOINTMENT_PERIOD data = (HIS_APPOINTMENT_PERIOD)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(
                    HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_APPOINTMENT_PERIOD data1 = new HIS_APPOINTMENT_PERIOD();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_APPOINTMENT_PERIOD>(HisRequestUriStore.MOSHIS_APPOINTMENT_PERIOD_LOCK,
                        ApiConsumers.MosConsumer, data1.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        //notHandler = true;
                        BackendDataWorker.Reset<HIS_APPOINTMENT_PERIOD>();
                        FillDataToControl();

                        //var datasource = gridControlFormList.DataSource as List<HIS_APPOINTMENT_PERIOD>;
                        //var dataUpdate = datasource.FirstOrDefault(o => o.ID == data.ID);
                        //if (dataUpdate != null)
                        //{
                        //    dataUpdate.IS_ACTIVE = success.IS_ACTIVE;
                        //}
                        //gridControlFormList.RefreshDataSource();
                        //gridviewFormList.BeginUpdate();
                        //gridviewFormList.GridControl.DataSource = datasource;
                        //gridviewFormList.EndUpdate();
                    }
                    //MessageManager.Show(this, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetReroucesByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAppointmentPeriod.Resources.Lang",
                    typeof(HIS.Desktop.Plugins.HisAppointmentPeriod.HisAppointmentPeriod.frmHisAppointmentPeriod).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.layoutControl1.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.layoutControl4.Text",
                     Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.lcEditorInfo.Text",
                      Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.btnAdd.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.btnEdit.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.btnReset.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.bbtnEdit.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.bbtnAdd.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.bbtnReset.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.bbtnFocusDefault.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.bar2.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn1.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn2.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn3.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn4.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn5.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn6.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn7.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn8.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn9.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn10.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn11.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.gridColumn12.Caption",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.spinGioTu.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.spinGioTu.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spinGioDen.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.spinGioDen.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spinPhutTu.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.spinPhutTu.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spinPhutDen.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.spinPhutDen.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spinToiDa.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.spinToiDa.Properties.NullValuePrompt",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisAppointmentPeriod.layoutControlItem6.Text",
                    Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void spinGioTu_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(spinGioTu.Text))
                {
                    spinPhutTu.EditValue = 0;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void spinGioDen_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(spinGioDen.Text))
                {
                    spinPhutDen.EditValue = 0;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void cboCoSo_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCoSo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinGioTu_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    spinGioTu.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPhutTu_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    spinPhutTu.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinGioDen_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    spinGioDen.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPhutDen_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    spinPhutDen.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinToiDa_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    spinToiDa.EditValue = null;
                }
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












    }
}
