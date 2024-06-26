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
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using MOS.EFMODEL.DataModels;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using Inventec.UC.Paging;
using DevExpress.XtraEditors.ViewInfo;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;

namespace HIS.Desktop.Plugins.PatientTypeAllow.PatientTypeAllowForm
{
    public partial class PatientTypeAllowForm : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int ActionType = -1;
        PagingGrid pagingGrid;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW currentData;
        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW resultData;
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        int positionHandle = -1;

        #endregion

        public PatientTypeAllowForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                currentModule = module;
                this.delegateSelect = delegateData;
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

        public PatientTypeAllowForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {

            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                currentModule = module;
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

        #region loadform
        private void PatientTypeAllowForm_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void MeShow()
        {
            try
            {
                LoadComboPatient();
                //Gán mặc định
                setDefaultValue();
                //Gán lại mặc định nút
                enableControlChanged(this.ActionType);

                //load dữ liệu vào datagctFormList
                LoadDatagctFormList();
                //----load ngôn ngữ
                SetCaptionByLanguageKey();

                //Set tabindex control//gán tabindex cho control
                InitTabIndex();
                //gán quy định bắt buộc
                ValidateForm();
                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;//Mới mở form cho phép thêm actionType=1
                // ResetFormData();               
                txtKey.Text = "";

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void enableControlChanged(int action)
        {
            try
            {
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);//action=actionEdit->hiện btnEdit
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);//...

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadDatagctFormList()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControl1);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>> apiResult = null;
                HisPatientTypeAllowFilter filter = new HisPatientTypeAllowFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                gridView1.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>)apiResult.Data;
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
                WaitingManager.Hide();
            }
        }
        
        //private void LoadPaging(object param)
        //{
        //    try
        //    {
        //        CommonParam paramCommon = new CommonParam();
        //        Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>> apiResult = null;
        //        HisPatientTypeAllowFilter filter = new HisPatientTypeAllowFilter();
        //        SetFilterNavBar(ref filter);
        //        filter.ORDER_DIRECTION = "DESC";
        //        filter.ORDER_FIELD = "MODIFY_TIME";

        //        gridView1.BeginUpdate();
        //        apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_GET, ApiConsumers.MosConsumer, filter, paramCommon);
        //        if (apiResult != null)
        //        {
        //            var data = (List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>)apiResult.Data;
        //            if (data != null)
        //            {

        //                gridView1.GridControl.DataSource = data;
        //                rowCount = (data == null ? 0 : data.Count);
        //                dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
        //            }
        //        }
        //        gridView1.EndUpdate();

        //        #region Process has exception
        //        SessionManager.ProcessTokenLost(paramCommon);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Error(ex);
        //    }


        //}

        private void SetFilterNavBar(ref HisPatientTypeAllowFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKey.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
       
        
        private void LoadComboPatient()
        {
            List<ColumnInfo> columnInfos = new List<ColumnInfo>();
            columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
            columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
            ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
            ControlEditorLoader.Load(cbbPatientType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
            ControlEditorLoader.Load(cbbPatientTypeAllow, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
        }
        
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientTypeAllow.Resource.Lang", typeof(HIS.Desktop.Plugins.PatientTypeAllow.PatientTypeAllowForm.PatientTypeAllowForm).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControl1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControl2.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControl4.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.btnReset.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.btnEdit.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.btnAdd.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbPatientTypeAllow.Properties.NullText = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.cbbPatientTypeAllow.Properties.NullText", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.cbbPatientType.Properties.NullText", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.gridColumn2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclPatientType.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.gclPatientType.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclPatientTypeAllowName.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.grclPatientTypeAllowName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControlItem3.OptionsToolTip.ToolTip", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControlItem3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControlItem6.OptionsToolTip.ToolTip", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControlItem6.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.layoutControl3.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.btnFind.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grlSTT.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.grlSTT.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclCode.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.gclCode.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gclName.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.gclName.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IS_BASE.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.IS_BASE.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.bar1.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.barButtonItem1.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.barButtonItem2.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem3.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.barButtonItem3.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFind.Caption = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.bbtnFind.Caption", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("PatientTypeAllowForm.Text", Resource.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("txtKey", 0);
                dicOrderTabIndexControl.Add("cbbStock", 1);
                dicOrderTabIndexControl.Add("cbbPatient", 2);
                //dicOrderTabIndexControl.Add("comboBox1", 3);

                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, layoutControl1);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool SetTabIndexToControl(KeyValuePair<string, int> itemOrderTab, DevExpress.XtraLayout.LayoutControl layoutControlEditor)
        {
            bool success = false;
            try
            {
                if (!layoutControlEditor.IsInitialized) return success;
                layoutControlEditor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlEditor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null)
                        {
                            BaseEdit be = lci.Control as BaseEdit;
                            if (be != null)
                            {
                                //Cac control dac biet can fix khong co thay doi thuoc tinh enable
                                if (itemOrderTab.Key.Contains(be.Name))
                                {
                                    be.TabIndex = itemOrderTab.Value;
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

            return success;
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControlCombo(cbbPatientTypeAllow);
                ValidationSingleControlCombo(cbbPatientType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControlCombo(GridLookUpEdit control)
        {
            try
            {
                ValidateCombo validRule = new ValidateCombo();
                validRule.grid = control;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocus()
        {
            try
            {
                txtKey.Focus();
                txtKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                V_HIS_PATIENT_TYPE_ALLOW data = null;
                if (e.RowHandle > -1)
                {
                    data = (V_HIS_PATIENT_TYPE_ALLOW)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDeleteEnable : btnDeleteDisable);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW pData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage;
                    }


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region event
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

        private bool? Existed()
        {
            try
            {
                long patient = Inventec.Common.TypeConvert.Parse.ToInt64(cbbPatientType.EditValue.ToString());
                long patientTypeAllow = Inventec.Common.TypeConvert.Parse.ToInt64(cbbPatientTypeAllow.EditValue.ToString());
                CommonParam paramCommon = new CommonParam();
                HisPatientTypeAllowFilter filter = new HisPatientTypeAllowFilter();
                filter.PATIENT_TYPE_ID = patient;
                var apiResult = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW>>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_GET, ApiConsumers.MosConsumer, filter, paramCommon).ToList();

                if (apiResult != null && apiResult.Exists(o => o.PATIENT_TYPE_ALLOW_ID == patientTypeAllow))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
           
	{
		 
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

                WaitingManager.Show();
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    cbbPatientType.EditValue = null;
                    cbbPatientTypeAllow.EditValue = null;
                    return;
                }
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW updateDTO = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW();
                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                   
                }
                //UpdateDTOFromDataForm(ref updateDTO);


                if (ActionType == GlobalVariables.ActionAdd)
                {

                    bool? check = Existed();
                    if (check!=null)
                    {
                        if (check==true)
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show("Dữ liệu đã tồn tại trên hệ thống", "Thông báo");
                            return;
                        }

                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Lỗi khi kiểm tra dữ liệu. Vui lòng thử lại sau.", "Thông báo");
                        return;
                    }
                    //updateDTO.MEDI_STOCK_ID = IMSys.DbConfig.HIS_RS.His.ID__XL; 
                    updateDTO.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cbbPatientType.EditValue.ToString());
                    updateDTO.PATIENT_TYPE_ALLOW_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cbbPatientTypeAllow.EditValue.ToString());

                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {

                        success = true;
                        LoadDatagctFormList();
                        cbbPatientTypeAllow.EditValue = null;
                        cbbPatientType.EditValue = null;
                        RefeshDataAfterSave(resultData);
                        ResetFormData();

                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        LoadDatagctFormList();
                        RefeshDataAfterSave(resultData);
                    }
                }

                WaitingManager.Hide();
                if (success)
                {
                    SetDefaultFocus();
                    BackendDataWorker.Reset<HIS_PATIENT_TYPE_ALLOW>();
                    BackendDataWorker.Reset<V_HIS_PATIENT_TYPE_ALLOW>();
                }


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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAllowFilter filter = new HisPatientTypeAllowFilter();
                filter.ID = currentId;
                
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW>>("api/HisPatientTypeAllow/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                currentDTO.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cbbPatientType.EditValue.ToString());
                currentDTO.PATIENT_TYPE_ALLOW_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cbbPatientTypeAllow.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
       
        void RefeshDataAfterSave(MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW data)
        {
            try
            {
                if (this.delegateSelect != null)
                {
                    this.delegateSelect(data);
                }

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
                if (!layoutControl1.IsInitialized) return;
                layoutControl1.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControl1.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtKey.Focus();

                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    layoutControl1.EndUpdate();


                }
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
                enableControlChanged(this.ActionType);
                positionHandle = -1;
                txtKey.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
               // ValidateForm();
                ResetFormData();
                LoadDatagctFormList();
                cbbPatientType.EditValue = null;
                cbbPatientTypeAllow.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }      
       
        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDatagctFormList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    enableControlChanged(this.ActionType);

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW data)
        {
            try
            {
                if (data != null)
                {
                    cbbPatientTypeAllow.EditValue = data.PATIENT_TYPE_ALLOW_ID;
                    cbbPatientType.EditValue = data.PATIENT_TYPE_ID;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void gridControl1_Click(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW)gridView1.GetFocusedRow();
                if (this.currentData != null)
                {

                    ChangedDataRow(this.currentData);
                    SetDefaultFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeleteEnable_Click(object sender, EventArgs e)
        {

            try
            {
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALLOW)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        HisPatientTypeAllowFilter filter = new HisPatientTypeAllowFilter(); 
                        CommonParam param = new CommonParam();
                        //var apiResult = new BackendAdapter(param).GetRO<List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALLOW>>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_GET, ApiConsumers.MosConsumer, filter, param);
                        success = new BackendAdapter(param).Post<bool>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_DELETE, ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            this.ActionType = 1;
                            cbbPatientType.Text = "";
                            cbbPatientTypeAllow.Text = "";
                            enableControlChanged(this.ActionType);
                            LoadDatagctFormList();
                            currentData = ((List<V_HIS_PATIENT_TYPE_ALLOW>)gridControl1.DataSource).FirstOrDefault();
                            BackendDataWorker.Reset<HIS_PATIENT_TYPE_ALLOW>();
                            BackendDataWorker.Reset<V_HIS_PATIENT_TYPE_ALLOW>();
                        }
                        MessageManager.Show(this, param, success);
                        btnReset_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_PATIENT_TYPE_ALLOW hisDepertments = new HIS_PATIENT_TYPE_ALLOW();
            bool notHandler = false;
            try
            {
                V_HIS_PATIENT_TYPE_ALLOW dataDepartment = (V_HIS_PATIENT_TYPE_ALLOW)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_PATIENT_TYPE_ALLOW data1 = new HIS_PATIENT_TYPE_ALLOW();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<HIS_PATIENT_TYPE_ALLOW>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) LoadDatagctFormList();
                    btnEdit.Enabled = false;
                    BackendDataWorker.Reset<HIS_PATIENT_TYPE_ALLOW>();
                    BackendDataWorker.Reset<V_HIS_PATIENT_TYPE_ALLOW>();
                    notHandler = true;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_PATIENT_TYPE_ALLOW hisDepertments = new HIS_PATIENT_TYPE_ALLOW();
            bool notHandler = false;
            try
            {
                V_HIS_PATIENT_TYPE_ALLOW dataDepartment = (V_HIS_PATIENT_TYPE_ALLOW)gridView1.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_PATIENT_TYPE_ALLOW data1 = new HIS_PATIENT_TYPE_ALLOW();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<HIS_PATIENT_TYPE_ALLOW>(HISRequestUriStore.MOSHIS_PATIENT_TYPE_ALLOW_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) LoadDatagctFormList();
                    btnEdit.Enabled = true;
                    BackendDataWorker.Reset<HIS_PATIENT_TYPE_ALLOW>();
                    BackendDataWorker.Reset<V_HIS_PATIENT_TYPE_ALLOW>();
                    notHandler = true;
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDeleteDisable_Click(object sender, EventArgs e)
        {

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
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKey_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode==Keys.Enter)
            {
                btnFind_Click(null,null);
                gridView1.SelectAll();
                gridView1.Focus();
            }
            if (e.KeyCode==Keys.Down)
            { 
                gridView1.SelectAll();
                gridView1.Focus();
                
            }
        }

        private void gridControl1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {

                    var rowData = (V_HIS_PATIENT_TYPE_ALLOW)gridView1.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);
                    }
                }
                if (e.KeyCode == Keys.Up)
                {

                    var rowData = (V_HIS_PATIENT_TYPE_ALLOW)gridView1.GetFocusedRow();
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
        #endregion

        #region shortcut
        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnEdit.Enabled)
            {
                btnEdit_Click(null, null);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnReset.Enabled)
            {
                btnReset_Click(null, null);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnFind.Enabled)
            {
                btnFind_Click(null, null);
            }
        }
        #endregion

        private void cbbStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnAdd.Focus();
            }
        }

        private void cbbPatient_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cbbPatientTypeAllow.ShowPopup();
            }
        }
    }
}
