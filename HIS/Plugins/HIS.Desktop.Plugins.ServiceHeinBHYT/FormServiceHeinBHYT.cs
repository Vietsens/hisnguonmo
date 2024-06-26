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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
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
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ServiceHeinBHYT
{
    public partial class FormHeinServiceBHYT : HIS.Desktop.Utility.FormBase
    {

        #region Declare

        Inventec.Desktop.Common.Modules.Module currentModule;
        int start;
        int limit;
        int rowCount;
        int dataTotal;
        int ActionType;
        long currentId;
        DelegateSelectData delegateSelect;
        V_HIS_HEIN_SERVICE_BHYT selectBHYT = new V_HIS_HEIN_SERVICE_BHYT();
        HIS_HEIN_SERVICE_BHYT heinServiceBhyt;
        int positionHandleControl = -1;
        List<HIS_PATIENT_TYPE> patientType = new List<HIS_PATIENT_TYPE>();

        #endregion

        #region Constructor

        public FormHeinServiceBHYT()
        {
            InitializeComponent();
        }

        public FormHeinServiceBHYT(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData _delegateSelect)
		:base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                ValidateForm();
                if (_delegateSelect != null)
                {
                    this.delegateSelect = _delegateSelect;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        public FormHeinServiceBHYT(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void FormHeinServiceBHYT_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                ActionType = GlobalVariables.ActionAdd;
                layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                txtSearch.Focus();
                GetDataToCombo();
                LoadComboDT(cboDTTT, patientType);
                this.cboDTTT.EditValue = patientType[0].ID;

                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ServiceHeinBHYT.Resources.Lang", typeof(HIS.Desktop.Plugins.ServiceHeinBHYT.FormHeinServiceBHYT).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.txtSearch.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDTTT.Properties.NullText = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.cboDTTT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.barButtonItem3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormServiceHeinBHYT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Method

        private void Refresh()
        {
            try
            {
                this.txtCode.Text = "";
                this.txtName.Text = "";
                this.txtSTT.Text = "";
                this.cboDTTT.EditValue = patientType[0].ID;
                this.txtSearch.Text = "";
                this.txtCode.Focus();
                layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGrid()
        {

            try
            {
                int numPageSize;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }

                FillDataToGridHeinServiceBHYT(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridHeinServiceBHYT, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void FillDataToGridHeinServiceBHYT(object param)
        {
            try
            {
                List<V_HIS_HEIN_SERVICE_BHYT> listData = new List<V_HIS_HEIN_SERVICE_BHYT>();
                gridControlHeinServiceBHYT.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisHeinServiceBhytViewFilter filter = new HisHeinServiceBhytViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtSearch.Text.Trim();
                //if (filter.KEY_WORD == "") paramCommon = new CommonParam(0, 100);
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_HEIN_SERVICE_BHYT>>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listData = (List<V_HIS_HEIN_SERVICE_BHYT>)result.Data;
                    rowCount = (listData == null ? 0 : listData.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);

                }

                gridControlHeinServiceBHYT.BeginUpdate();
                gridControlHeinServiceBHYT.DataSource = listData;
                gridControlHeinServiceBHYT.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                patientType = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>(HisRequestUriStore.HIS_PATIENT_TYPE_GET, ApiConsumers.MosConsumer, filter, null).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        void RefeshDataAfterSave()
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(heinServiceBhyt);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void unLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_HEIN_SERVICE_BHYT success = new HIS_HEIN_SERVICE_BHYT();
            bool notHandler = false;
            try
            {

                V_HIS_HEIN_SERVICE_BHYT data = (V_HIS_HEIN_SERVICE_BHYT)gridViewHeinServiceBHYT.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_HEIN_SERVICE_BHYT data1 = new HIS_HEIN_SERVICE_BHYT();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_HEIN_SERVICE_BHYT>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGrid();
                        FillDataToGrid();
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>();
                    }
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, param, notHandler);
                param = new CommonParam();
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_HEIN_SERVICE_BHYT success = new HIS_HEIN_SERVICE_BHYT();
            bool notHandler = false;
            try
            {

                V_HIS_HEIN_SERVICE_BHYT data = (V_HIS_HEIN_SERVICE_BHYT)gridViewHeinServiceBHYT.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_HEIN_SERVICE_BHYT data1 = new HIS_HEIN_SERVICE_BHYT();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_HEIN_SERVICE_BHYT>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        notHandler = true;
                        FillDataToGrid();
                        //FillDataToGrid();
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>();
                    }
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, param, notHandler);
                param = new CommonParam();
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_HEIN_SERVICE_BHYT success = new HIS_HEIN_SERVICE_BHYT();
            bool result = false;
            //bool notHandler = false;
            try
            {

                V_HIS_HEIN_SERVICE_BHYT data = (V_HIS_HEIN_SERVICE_BHYT)gridViewHeinServiceBHYT.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_HEIN_SERVICE_BHYT data1 = new HIS_HEIN_SERVICE_BHYT();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_DELETE, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (result)
                    {
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>();
                        FillDataToGrid();
                        FillDataToGrid();
                    }
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, param, result);
                param = new CommonParam();
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtName.Focus();
                    txtName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (layoutControlItem5.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        cboDTTT.Focus();
                        cboDTTT.ShowPopup();
                    }
                    else
                    {
                        txtSTT.Focus();
                        txtSTT.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboDTTT_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtSTT.Focus();
                    txtSTT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboDTTT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDTTT.EditValue != null)
                    {
                        txtSTT.Focus();
                        txtSTT.SelectAll();
                    }
                }
                //e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtSTT_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBedFilter filter = new HisBedFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT currentDTO)
        {
            try
            {
                //currentDTO.SAMPLE_ROOM_CODE = txtBedCode.Text.Trim();
                currentDTO.HEIN_SERVICE_BHYT_CODE = txtCode.Text.Trim();
                currentDTO.HEIN_SERVICE_BHYT_NAME = txtName.Text.Trim();
                currentDTO.HEIN_ORDER = txtSTT.Text.Trim();

                //if (lkBedTypeId.EditValue != null) currentDTO.ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkBedTypeId.EditValue ?? "0").ToString());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            CommonParam param = new CommonParam();
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                //var mapCurrentBHYT = new HIS_HEIN_SERVICE_BHYT();
                heinServiceBhyt = new HIS_HEIN_SERVICE_BHYT();
                MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT updateDTO = new MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT();
                var heinService = new HIS_HEIN_SERVICE();

                if (this.selectBHYT != null && this.selectBHYT.ID > 0)
                {
                    LoadCurrent(this.selectBHYT.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);

                heinService.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDTTT.EditValue).ToString());

                updateDTO.HIS_HEIN_SERVICE = heinService;

                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var result = new BackendAdapter(param).Post<HIS_HEIN_SERVICE_BHYT>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_CREATE, ApiConsumers.MosConsumer, updateDTO, null);
                    if (result != null)
                    {
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>();
                        heinServiceBhyt = result;
                        btnRefresh_Click(null, null);
                        success = true;
                        this.delegateSelect(result);
                    }
                }

                if (this.ActionType == GlobalVariables.ActionEdit)
                {
                    updateDTO.HIS_HEIN_SERVICE.ID = selectBHYT.HEIN_SERVICE_ID;
                    var result = new BackendAdapter(param).Post<HIS_HEIN_SERVICE_BHYT>(HisRequestUriStore.HIS_HEIN_SERVICE_BHYT_UPDATE, ApiConsumers.MosConsumer, updateDTO, null);
                    if (result != null)
                    {
                        success = true;
                        BackendDataWorker.Reset<MOS.EFMODEL.DataModels.HIS_HEIN_SERVICE_BHYT>();
                        heinServiceBhyt = result;
                        this.ActionType = GlobalVariables.ActionAdd;
                        RemoveErrorControl();
                        FillDataToGrid();
                        this.delegateSelect(result);
                    }
                }

                

                WaitingManager.Hide();
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.ShowAlert(this, param, success);
                param = new CommonParam();
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void RemoveErrorControl()
        {
            try
            {

                dxValidationProvider1.RemoveControlError(txtCode);
                dxValidationProvider1.RemoveControlError(txtName);
                dxValidationProvider1.RemoveControlError(cboDTTT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
			
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                Refresh();
                dxValidationProvider1.RemoveControlError(txtCode);
                dxValidationProvider1.RemoveControlError(txtName);
                dxValidationProvider1.RemoveControlError(cboDTTT);
                FillDataToGrid();
                btnSave.Enabled = true;
                ActionType = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewHeinServiceBHYT_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_HEIN_SERVICE_BHYT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((data.IS_ACTIVE ?? -1).ToString());
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * (ucPaging1.pagingGrid.PageSize);
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
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.CREATE_TIME);
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
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString((long)data.MODIFY_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            try
                            {
                                if (status == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                    e.Value = "Hoạt động";
                                else
                                    e.Value = "Tạm khóa";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewHeinServiceBHYT_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_HEIN_SERVICE_BHYT data = (V_HIS_HEIN_SERVICE_BHYT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? Lock : unLock);
                    }

                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? DeleteD : Delete);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridViewHeinServiceBHYT_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var data = (V_HIS_HEIN_SERVICE_BHYT)gridViewHeinServiceBHYT.GetFocusedRow();
                if (data != null)
                {
                    selectBHYT = null;
                    txtCode.Text = data.HEIN_SERVICE_BHYT_CODE;
                    txtName.Text = data.HEIN_SERVICE_BHYT_NAME;
                    txtSTT.Text = data.HEIN_ORDER;
                    cboDTTT.EditValue = data.PATIENT_TYPE_ID;
                    currentId = data.ID;
                    selectBHYT = data;
                    layoutControlItem5.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ActionType = GlobalVariables.ActionEdit;
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                    {
                        btnSave.Enabled = false;
                    }
                    else
                        btnSave.Enabled = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                V_HIS_HEIN_SERVICE_BHYT data = (V_HIS_HEIN_SERVICE_BHYT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        #endregion

        #region Load Combo

        private void LoadComboDT(DevExpress.XtraEditors.LookUpEdit cboDT, List<HIS_PATIENT_TYPE> data)
        {
            try
            {
                cboDT.Properties.DataSource = data;
                cboDT.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboDT.Properties.ValueMember = "ID";
                cboDT.Properties.ForceInitialize();
                cboDT.Properties.Columns.Clear();
                cboDT.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_CODE", "", 50));
                cboDT.Properties.Columns.Add(new LookUpColumnInfo("PATIENT_TYPE_NAME", "", 100));
                cboDT.Properties.ShowHeader = false;
                cboDT.Properties.ImmediatePopup = true;
                cboDT.Properties.DropDownRows = 10;
                cboDT.Properties.PopupWidth = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
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
                ValidationSingleControl(cboDTTT, dxValidationProvider1);
                ValidationSingleControl(txtCode, dxValidationProvider1);
                ValidationSingleControl(txtName, dxValidationProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

    }
}
