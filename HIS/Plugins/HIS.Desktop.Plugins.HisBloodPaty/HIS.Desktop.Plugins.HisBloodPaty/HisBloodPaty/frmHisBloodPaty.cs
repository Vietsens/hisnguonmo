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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.HisBloodPaty.HisBloodPaty
{
    public partial class frmHisBloodPaty : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<MOS.EFMODEL.DataModels.V_HIS_BLOOD> bloods;
        #endregion

        #region Construct
        public frmHisBloodPaty(Inventec.Desktop.Common.Modules.Module moduleData)
		:base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                gridControlFormList.ToolTipController = toolTipControllerGrid;

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

        #region Private method
        private void frmHisBloodPaty_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBloodPaty.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBloodPaty.HisBloodPaty.frmHisBloodPaty).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnLock.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.gridColumnLock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.gridColumnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBloodId.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColBloodId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBloodId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColBloodId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeId.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColPatientTypeId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientTypeId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColPatientTypeId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPrice.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColExpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPrice.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColExpPrice.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColExpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpVatRatio.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColExpVatRatio.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPriority.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColPriority.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPriority.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColPriority.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColFromTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColFromTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColToTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColToTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentFromTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColTreatmentFromTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentFromTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColTreatmentFromTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentToTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColTreatmentToTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentToTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColTreatmentToTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkBloodType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lkBloodType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lkBloodId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lkBloodId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkPatientTypeId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lkPatientTypeId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodId.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciBloodId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeId.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciPatientTypeId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciExpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpVatRatio.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciExpVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentFromTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciTreatmentFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentToTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciTreatmentToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodType.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciBloodType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPriority.Text = Inventec.Common.Resource.Get.Value("frmHisBloodPaty.lciPriority.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetDefaultFocus()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitTabIndex()
        {
            try
            {
                dicOrderTabIndexControl.Add("lkBloodId", 0);
                dicOrderTabIndexControl.Add("lkPatientTypeId", 1);
                dicOrderTabIndexControl.Add("spExpPrice", 2);
                dicOrderTabIndexControl.Add("spExpVatRatio", 3);
                dicOrderTabIndexControl.Add("spPriority", 4);
                dicOrderTabIndexControl.Add("dtFromTime", 5);
                dicOrderTabIndexControl.Add("dtToTime", 6);
                dicOrderTabIndexControl.Add("dtTreatmentFromTime", 7);
                dicOrderTabIndexControl.Add("dtTreatmentToTime", 8);


                if (dicOrderTabIndexControl != null)
                {
                    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                    {
                        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
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

        private void FillDataToControlsForm()
        {
            try
            {
                InitComboBloodId();
                InitComboBloodType();
                InitComboPatientTypeId();

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // khóa, bỏ khóa
        private void LockUnlockBloodPaty()
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var updateDTO = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
                HIS_BLOOD_PATY bloodPaty = new HIS_BLOOD_PATY();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BLOOD_PATY>(bloodPaty, updateDTO);
                var result = new BackendAdapter(param).Post<HIS_BLOOD_PATY>(HisRequestUriStore.MOSHIS_BLOOD_PATY_CHANGE_LOCK, ApiConsumers.MosConsumer, bloodPaty.ID, param);
                if (result != null)
                {
                    success = true;
                    FillDataToGridControl();
                    ResetFormData();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboBloodId()
        {
            try
            {
                WaitingManager.Show();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 80, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_RH_CODE", "", 80, 2));
                columnInfos.Add(new ColumnInfo("BLOOD_CODE", "", 100, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfos, false, 350);
                MOS.Filter.HisBloodViewFilter bloodFilter = new HisBloodViewFilter();
                //bloodFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                if (lkBloodType.EditValue != null)
                {
                    bloodFilter.BLOOD_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(lkBloodType.EditValue.ToString());
                }
                bloods = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD>>("api/HisBlood/GetView", ApiConsumer.ApiConsumers.MosConsumer, bloodFilter, new CommonParam());
                ControlEditorLoader.Load(lkBloodId, bloods, controlEditorADO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBloodType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BLOOD_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BLOOD_TYPE_NAME", "ID", columnInfos, false, 350);
                MOS.Filter.HisBloodTypeFilter bloodTypeFilter = new HisBloodTypeFilter();
                bloodTypeFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var bloodTypes = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_TYPE>>("api/HisBloodType/Get", ApiConsumer.ApiConsumers.MosConsumer, bloodTypeFilter, new CommonParam());
                ControlEditorLoader.Load(lkBloodType, bloodTypes, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientTypeId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkPatientTypeId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
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
        public void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPaging, param, numPageSize);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY>> apiResult = null;
                HisBloodPatyViewFilter filter = new HisBloodPatyViewFilter();
                SetFilterNavBar(ref filter);
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY>>(HisRequestUriStore.MOSHIS_BLOOD_PATY_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY>)apiResult.Data;
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

        private void SetFilterNavBar(ref HisBloodPatyViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
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

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridviewFormList.Focus();
                    gridviewFormList.FocusedRowHandle = 0;
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY pData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "FROM_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.FROM_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TO_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.TO_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TREATMENT_FROM_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.TREATMENT_FROM_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TREATMENT_TO_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(pData.TREATMENT_TO_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "EXP_VAT_RATIO_STR")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToStringRoundMax4(pData.EXP_VAT_RATIO * 100);
                    }

                    gridControlFormList.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {

        }

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        ChangedDataRow(rowData);

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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY data)
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY data)
        {
            try
            {
                if (data != null)
                {
                    lkBloodType.EditValue = data.BLOOD_TYPE_ID;
                    lkBloodId.EditValue = data.BLOOD_ID;
                    lkPatientTypeId.EditValue = data.PATIENT_TYPE_ID;
                    spExpPrice.EditValue = data.EXP_PRICE;
                    spExpVatRatio.EditValue = data.EXP_VAT_RATIO * 100;
                    spPriority.EditValue = data.PRIORITY;
                    dtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FROM_TIME ?? 0);
                    dtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TO_TIME ?? 0);
                    dtTreatmentFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TREATMENT_FROM_TIME ?? 0);
                    dtTreatmentToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TREATMENT_TO_TIME ?? 0);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Gan focus vao control mac dinh
        /// </summary>
        private void SetFocusEditor()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
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

                            fomatFrm.ResetText();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_BLOOD_PATY currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBloodPatyFilter filter = new HisBloodPatyFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BLOOD_PATY>>(HisRequestUriStore.MOSHIS_BLOOD_PATY_GETVIEW, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
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

        #region Button handler
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_BLOOD_PATY_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (success)
                    {
                        FillDataToGridControl();
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                this.currentData = null;
                SetFocusEditor();
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
                MOS.EFMODEL.DataModels.HIS_BLOOD_PATY updateDTO = new MOS.EFMODEL.DataModels.HIS_BLOOD_PATY();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_PATY>(HisRequestUriStore.MOSHIS_BLOOD_PATY_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BLOOD_PATY>(HisRequestUriStore.MOSHIS_BLOOD_PATY_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        UpdateRowDataAfterEdit(resultData);
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_BLOOD_PATY data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_BLOOD_PATY) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    MOS.Filter.HisBloodPatyViewFilter bloodPatyViewFilter = new HisBloodPatyViewFilter();
                    bloodPatyViewFilter.ID = rowData.ID;
                    var bloodPaty = new BackendAdapter(new CommonParam()).Get<List<V_HIS_BLOOD_PATY>>("api/HisBloodPaty/GetView", ApiConsumer.ApiConsumers.MosConsumer, bloodPatyViewFilter, new CommonParam()).FirstOrDefault();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY>(rowData, bloodPaty);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_BLOOD_PATY currentDTO)
        {
            try
            {
                if (lkBloodId.EditValue != null) currentDTO.BLOOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkBloodId.EditValue ?? "0").ToString());
                if (lkPatientTypeId.EditValue != null) currentDTO.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkPatientTypeId.EditValue ?? "0").ToString());
                currentDTO.EXP_PRICE = (decimal)spExpPrice.Value;
                currentDTO.EXP_VAT_RATIO = (decimal)spExpVatRatio.Value / 100;
                currentDTO.PRIORITY = (long)spPriority.Value;
                if (dtFromTime.DateTime != null && dtFromTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtFromTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                if (dtToTime.DateTime != null && dtToTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtToTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                if (dtTreatmentFromTime.DateTime != null && dtTreatmentFromTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.TREATMENT_FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTreatmentFromTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                if (dtTreatmentToTime.DateTime != null && dtTreatmentToTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.TREATMENT_TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtTreatmentToTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(lkBloodId);
                ValidationSingleControl(lkPatientTypeId);
                ValidationSingleControl(spExpPrice);
                ValidationSingleControl(spExpVatRatio);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
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
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Tooltip
        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
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
        #endregion

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                //InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
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
                btnCancel_Click(null, null);
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
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    short isActive = short.Parse((gridviewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    if (e.Column.FieldName == "LOCK_ITEM")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = ButtonEditUnlock;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditLock;
                        }
                    }
                    else if (e.Column.FieldName == "gridColumnEdit")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = buttonEditDeleteEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditDeleteDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                LockUnlockBloodPaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEditUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                LockUnlockBloodPaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_BLOOD_PATY)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    var searchOne = (lkBloodId.Properties.DataSource as List<V_HIS_BLOOD>).FirstOrDefault(o => o.ID == 3075);

                    this.currentData = rowData;
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

        private void lkBloodType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (lkBloodType.EditValue != null && lkBloodType.EditValue != lkBloodType.OldEditValue)
                    {
                        InitComboBloodId();
                        lkBloodId.Focus();
                        lkBloodId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkBloodId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    lkPatientTypeId.Focus();
                    lkPatientTypeId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkPatientTypeId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    spExpPrice.Focus();
                    spExpPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkPatientTypeId.Focus();
                    lkPatientTypeId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spExpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtFromTime.Focus();
                    dtFromTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtFromTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtToTime.Focus();
                    dtToTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtTreatmentFromTime.Focus();
                    dtTreatmentFromTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTreatmentFromTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtTreatmentToTime.Focus();
                    dtTreatmentToTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTreatmentToTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    spPriority.Focus();
                    spPriority.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spPriority_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else if (btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkBloodType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkBloodType.EditValue != null)
                    {
                        InitComboBloodId();
                        lkBloodId.Focus();
                        lkBloodId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkBloodId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkBloodId.EditValue != null)
                    {
                        lkPatientTypeId.Focus();
                        lkPatientTypeId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
