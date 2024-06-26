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
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.Plugins.HisExpMestTemplate.ADO;
using HIS.Desktop.Plugins.HisExpMestTemplate.Validate;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.HisExpMestTemplate.Resources;
using System.Threading;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;

namespace HIS.Desktop.Plugins.HisExpMestTemplate
{
    public partial class frmHisExpMestTemplate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        int positionHandleAdd = -1;
        int actionBosung = 0;
        MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE currentData;

        bool isShowContainerMediMaty = false;
        bool isShowContainerMediMatyForChoose = false;
        bool isShow = true;

        List<V_HIS_MEDICINE_TYPE> ListMedicineType = new List<V_HIS_MEDICINE_TYPE>();
        List<V_HIS_MATERIAL_TYPE> ListMaterialType = new List<V_HIS_MATERIAL_TYPE>();
        List<MediMatyTypeADO> listMediMatyTypeADO = new List<MediMatyTypeADO>();
        List<HIS_MEDICINE_USE_FORM> listMedicineUseForm = new List<HIS_MEDICINE_USE_FORM>();
        List<HIS_HTU> listHisHtu = new List<HIS_HTU>();
        MediMatyTypeADO MediMatyTypeADO = new MediMatyTypeADO();
        List<MediMatyTypeADO> listMediMatyTypeADOchoose = new List<MediMatyTypeADO>();

        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        string loggingName;
        int numberDisplaySeperateFormatAmount = 0;

        #endregion

        #region Construct
        public frmHisExpMestTemplate(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
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
        private void frmHisExpMestTemplate_Load(object sender, EventArgs e)
        {
            try
            {
                loggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                layoutControlItem9.Size = new Size(111, 26);
                layoutControlItem6.Size = new Size(111, 26);

                if (HisConfigCFG.AmountDecimalNumber > 0)
                {
                    numberDisplaySeperateFormatAmount = HisConfigCFG.AmountDecimalNumber;
                }
                else
                {
                    numberDisplaySeperateFormatAmount = 2;
                }
                this.actionBosung = GlobalVariables.ActionAdd;
                int wight = lciAmount.Size.Width + layoutControlItem51.Size.Width;
                this.emptySpaceItem1.Size = new System.Drawing.Size(wight, emptySpaceItem1.Size.Height);

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

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExpMestTemplate.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExpMestTemplate.frmHisExpMestTemplate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
                
                this.grdColCode.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit1.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.checkEdit1.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnResset.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.btnResset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciBedTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBedTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciBedTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPrescriptionType.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciPrescriptionType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.chkTanDuoc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.chkTanDuoc.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkYHCT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.chkYHCT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSoThang.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciSoThang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkInStock.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.chkInStock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkOutStock.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.chkOutStock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThuocVatTu.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciThuocVatTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSang.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciSang.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTrua.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciTrua.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciChieu.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciChieu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciToi.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciToi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHtu.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciHtu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUseForm.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciUseForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTutorial.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.lciTutorial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn37.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn37.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn38.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn39.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn39.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn39.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn40.Caption = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn40.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn40.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.gridColumn40.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }
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
                ResetFormData();
                txtKeyword.Text = "";
                chkInStock.CheckState = CheckState.Checked;
                btnSave.Enabled = false;
                chkTanDuoc.CheckState = CheckState.Checked;
                chkYHCT.CheckState = CheckState.Unchecked;
                ResetMediMaty();
                
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
                dicOrderTabIndexControl.Add("txtExpMestTemplateCode", 0);
                dicOrderTabIndexControl.Add("txtExpMestTemplateName", 1);

                //dicOrderTabIndexControl.Add("spMaxCapacity", 2);


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

                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo

        #endregion

        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControl()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));

                //CommonParam param = new CommonParam();
                //param.Limit = rowCount;
                //param.Count = dataTotal;
                //ucPaging.Init(LoadPaging, param, pageSize, this.gridControlFormList);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                //Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>> apiResult = null;
                HisExpMestTemplateFilter filter = new HisExpMestTemplateFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //filter.CREATOR = this.loggingName;
                //filter.IS_PUBLIC = 1;
                gridviewFormList.BeginUpdate();
                var apiResult = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    //var data = (List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>)apiResult.Data.Where(o => o.CREATOR == this.loggingName || o.IS_PUBLIC == 1).ToList()
                    var data = (List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>)apiResult.Where(o => o.CREATOR == this.loggingName || o.IS_PUBLIC == 1).ToList();
                    if (data != null)
                    {
                        gridviewFormList.GridControl.DataSource = data;
                        //rowCount = (data == null ? 0 : data.Count);
                        //dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
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

        private void SetFilterNavBar(ref HisExpMestTemplateFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE pData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        try
                        {
                            if (status == 1)
                                e.Value = "Hoạt động";
                            else
                                e.Value = "Tạm khóa";
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
                    else if (e.Column.FieldName == "Check")
                    {
                        try
                        {
                            e.Value = pData.IS_PUBLIC == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "Check2")
                    {
                        try
                        {
                            e.Value = pData.IS_KIDNEY == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "PRESCRIPTION_TYPE_STR")
                    {
                        try
                        {
                            e.Value = pData.PRESCRIPTION_TYPE_ID == 1 ? "Tân được" : "YHCT";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }

                gridControlFormList.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
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

      

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data)
        {
            try
            {
                if (data != null)
                {
                    if (data.CREATOR == this.loggingName)
                    {
                        btnSave.Enabled = true;
                    }
                    else 
                    {
                        btnSave.Enabled = false;
                    }

                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    //btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && this.currentData.CREATOR == this.loggingName);
                    positionHandle = -1;
                    this.positionHandleAdd = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderAdd, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data)
        {
            try
            {
                if (data != null)
                {
                    this.listMediMatyTypeADOchoose = new List<ADO.MediMatyTypeADO>();

                    txtExpMestTemplateCode.Text = data.EXP_MEST_TEMPLATE_CODE;
                    txtExpMestTemplateName.Text = data.EXP_MEST_TEMPLATE_NAME;
                    txtDescription.Text = data.DESCRIPTION;
                    checkEdit1.Checked = data.IS_PUBLIC == 1 ? true : false;
                    checkEdit2.Checked = data.IS_KIDNEY == 1 ? true : false;

                    this.chkTanDuoc.Checked = data.PRESCRIPTION_TYPE_ID == 1 ? true : false;
                    this.chkYHCT.Checked = data.PRESCRIPTION_TYPE_ID == 2 ? true : false;

                    if (this.lciSoThang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.txtRemedyCount.Text = data.REMEDY_COUNT.ToString();
                    }

                    //spMaxCapacity.EditValue = data.MAX_CAPACITY;

                    CommonParam param = new CommonParam();

                    HisEmteMaterialTypeViewFilter matyFilter = new HisEmteMaterialTypeViewFilter();
                    matyFilter.EXP_MEST_TEMPLATE_ID = data.ID;

                    var listEmteMaty = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MATERIAL_TYPE>>("api/HisEmteMaterialType/GetView", ApiConsumers.MosConsumer, matyFilter, param);

                    HisEmteMedicineTypeViewFilter metyFilter = new HisEmteMedicineTypeViewFilter();
                    metyFilter.EXP_MEST_TEMPLATE_ID = data.ID;

                    var listEmteMety = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MEDICINE_TYPE>>("api/HisEmteMedicineType/GetView", ApiConsumers.MosConsumer, metyFilter, param);

                    if (listEmteMaty != null && listEmteMaty.Count > 0)
                    {
                        foreach (var item in listEmteMaty)
                        {
                            MediMatyTypeADO ado = new MediMatyTypeADO(item);
                            this.listMediMatyTypeADOchoose.Add(ado);
                        }
                    }

                    if (listEmteMety != null && listEmteMety.Count > 0)
                    {
                        foreach (var item in listEmteMety)
                        {
                            MediMatyTypeADO ado = new MediMatyTypeADO(item);
                            this.listMediMatyTypeADOchoose.Add(ado);
                        }
                    }

                    gridViewAdd.BeginUpdate();
                    gridViewAdd.GridControl.DataSource = this.listMediMatyTypeADOchoose;
                    gridViewAdd.EndUpdate();

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
                //TODO

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
                //if (!lcEditorInfo.IsInitialized) return;
                //lcEditorInfo.BeginUpdate();

                this.txtExpMestTemplateCode.Text = "";
                this.txtExpMestTemplateCode.EditValue = null;

                this.txtExpMestTemplateName.Text = "";
                this.txtExpMestTemplateName.EditValue = null;

                this.txtRemedyCount.Text = "";
                this.txtRemedyCount.EditValue = null;

                this.txtDescription.Text = "";
                this.txtDescription.EditValue = null;

                this.checkEdit1.CheckState = CheckState.Unchecked;
                this.checkEdit2.CheckState = CheckState.Unchecked;

                //try
                //{
                //    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                //    {
                //        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                //        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                //        {
                //            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                //            fomatFrm.ResetText();
                //            fomatFrm.EditValue = null;

                //            txtExpMestTemplateCode.Focus();
                //        }
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Inventec.Common.Logging.LogSystem.Warn(ex);
                //}
                //finally
                //{
                //    lcEditorInfo.EndUpdate();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExpMestTemplateFilter filter = new HisExpMestTemplateFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                WaitingManager.Show();
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetDefaultValue();
                //FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnGDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HisExpMestTemplateFilter filter = new HisExpMestTemplateFilter();
                    filter.ID = rowData.ID;
                    WaitingManager.Show();
                    var data = new BackendAdapter(param).Get<List<HIS_EXP_MEST_TEMPLATE>>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();

                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_DELETE, ApiConsumers.MosConsumer, data.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_EXP_MEST_TEMPLATE>();
                            FillDataToGridControl();
                            currentData = ((List<HIS_EXP_MEST_TEMPLATE>)gridControlFormList.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
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


        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE currentDTO, ref MOS.SDO.HisExpMestTemplateSDO sdo)
        {
            try
            {
                currentDTO.EXP_MEST_TEMPLATE_CODE = txtExpMestTemplateCode.Text.Trim();
                currentDTO.EXP_MEST_TEMPLATE_NAME = txtExpMestTemplateName.Text.Trim();
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();
                currentDTO.IS_PUBLIC = checkEdit1.Checked ? (short)1 : (short)0;
                currentDTO.IS_KIDNEY = checkEdit2.Checked ? (short)1 : (short)0;
                if (this.chkTanDuoc.Checked)
                {
                    currentDTO.PRESCRIPTION_TYPE_ID = 1;
                }
                else if (this.chkYHCT.Checked)
                {
                    currentDTO.PRESCRIPTION_TYPE_ID = 2;
                }

                if (this.lciSoThang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && !String.IsNullOrEmpty(this.txtRemedyCount.Text.Trim()))
                {
                    currentDTO.REMEDY_COUNT = long.Parse(this.txtRemedyCount.Text.Trim());
                }

                //currentDTO.MAX_CAPACITY = (long)spMaxCapacity.Value;

                sdo.ExpMestTemplate = currentDTO;

                var lstMety = this.listMediMatyTypeADOchoose.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC).ToList();
                var lstMaty = this.listMediMatyTypeADOchoose.Where(o => o.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU).ToList();

                sdo.EmteMedicineTypes = new List<HIS_EMTE_MEDICINE_TYPE>();

                if (lstMety != null && lstMety.Count > 0)
                {
                    foreach (var item in lstMety)
                    {
                        HIS_EMTE_MEDICINE_TYPE mediType = new HIS_EMTE_MEDICINE_TYPE();
                        mediType.MEDICINE_TYPE_ID = item.ID;
                        mediType.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        mediType.AFTERNOON = item.Trua;
                        mediType.AMOUNT = item.AMOUNT ?? 0;
                        mediType.EVENING = item.Toi;
                        mediType.HTU_ID = item.HTU_ID;
                        mediType.IS_EXPEND = item.IS_AUTO_EXPEND;
                        mediType.IS_OUT_MEDI_STOCK = item.IsOutMediStock;
                        mediType.MORNING = item.Sang;
                        mediType.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        mediType.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        mediType.TUTORIAL = item.TUTORIAL;
                        mediType.NOON = item.Chieu;

                        if (currentDTO != null && currentDTO.ID > 0)
                        {
                            mediType.EXP_MEST_TEMPLATE_ID = currentDTO.ID;
                        }

                        sdo.EmteMedicineTypes.Add(mediType);
                    }
                }

                sdo.EmteMaterialTypes = new List<HIS_EMTE_MATERIAL_TYPE>();

                if (lstMaty != null && lstMaty.Count > 0)
                {
                    foreach (var item in lstMaty)
                    {
                        HIS_EMTE_MATERIAL_TYPE maty = new HIS_EMTE_MATERIAL_TYPE();

                        maty.AMOUNT = item.AMOUNT ?? 0;
                        maty.IS_EXPEND = item.IS_AUTO_EXPEND;
                        maty.IS_OUT_MEDI_STOCK = item.IsOutMediStock;
                        maty.MATERIAL_TYPE_ID = item.ID;
                        maty.MATERIAL_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                        maty.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        maty.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;

                        if (currentDTO != null && currentDTO.ID > 0)
                        {
                            maty.EXP_MEST_TEMPLATE_ID = currentDTO.ID;
                        }

                        sdo.EmteMaterialTypes.Add(maty);
                    }
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
                ValidationSingleControl(txtExpMestTemplateCode,10, true);
                ValidationSingleControl(txtExpMestTemplateName,100, true);
                //ValidationSingleControl1();
                validationRemedyCount();
                ValidationSingleControl(this.txtAmount, this.dxValidationProviderAdd);
                

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl1()
        {
            try
            {
                //ValidatespMax validate = new ValidatespMax();
                //validate.spMax = spMaxCapacity;
                //validate.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                //validate.ErrorType = ErrorType.Warning;
                //this.dxValidationProviderEditorInfo.SetValidationRule(spMaxCapacity, validate);
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

        private void ValidationSingleControl(BaseEdit control, int? maxLength, bool IsRequest = false)
        {
            try
            {
                ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxLength;
                validRule.IsRequired = IsRequest;
                //validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validationRemedyCount()
        {
            try
            {
                RemedyCountValidationRule validRule = new RemedyCountValidationRule();
                validRule.lciSoThang = this.lciSoThang;
                validRule.txtRemedyCount = this.txtRemedyCount;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(this.txtRemedyCount, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void validationTutorial()
        {
            try
            {
                TutorialValidationRule validRule = new TutorialValidationRule();
                
                validRule.txtTutorial = this.txtTutorial;
                validRule.DataType = this.MediMatyTypeADO != null ? this.MediMatyTypeADO.DataType : (int?)null;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderAdd.SetValidationRule(this.txtTutorial, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
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
                WaitingManager.Show();
                //Gan gia tri mac dinh
                SetDefaultValue();

                //InitComboMedimaty();

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                //FillDataToGridControl();

                CreateThread();

                //Load ngon ngu label control
                //SetCaptionByLanguageKey();

                this.VisibleButton(this.actionBosung);

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void loadcombobox()
        {
            try
            {
                InitComboMedicineUseForm();
                InitComboHtu();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void CreateThread() 
        {
            Thread threadComboMedimaty = new System.Threading.Thread(InitComboMedimaty);
            Thread threadloadCbo = new System.Threading.Thread(loadcombobox);
            Thread threadFillData = new System.Threading.Thread(FillDataToGridControl);

            
            threadFillData.Priority = ThreadPriority.Normal;
            threadloadCbo.Priority = ThreadPriority.Normal;
            try
            {
                threadFillData.Start();
                threadComboMedimaty.Start();
                threadloadCbo.Start();

                threadComboMedimaty.Join();
                threadFillData.Join();
                threadloadCbo.Join();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadComboMedimaty.Abort();
                threadFillData.Abort();
                threadloadCbo.Abort();
            }
        }

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

        private void btnGLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_EXP_MEST_TEMPLATE success = new HIS_EXP_MEST_TEMPLATE();
            //bool notHandler = false;
            try
            {

                HIS_EXP_MEST_TEMPLATE data = (HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_EXP_MEST_TEMPLATE data1 = new HIS_EXP_MEST_TEMPLATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST_TEMPLATE>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
                    
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_EXP_MEST_TEMPLATE>();
                        FillDataToGridControl();
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnGunLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            
            CommonParam param = new CommonParam();
            HIS_EXP_MEST_TEMPLATE success = new HIS_EXP_MEST_TEMPLATE();
            //bool notHandler = false;
            try
            {

                HIS_EXP_MEST_TEMPLATE data = (HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_EXP_MEST_TEMPLATE data1 = new HIS_EXP_MEST_TEMPLATE();
                    data1.ID = data.ID;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_EXP_MEST_TEMPLATE>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_CHANGE_LOCK, ApiConsumers.MosConsumer, data1, param);
                    if (success != null)
                    {
                        BackendDataWorker.Reset<HIS_EXP_MEST_TEMPLATE>();
                        FillDataToGridControl();
                    }
                } 
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {  //var da = gridControl1.
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridviewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                   HIS_EXP_MEST_TEMPLATE data = (HIS_EXP_MEST_TEMPLATE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                       
                      //  e.RepositoryItem = ((isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&  data.CREATOR == loggingName) ? btnGEdit : repositoryItemButtonEdit1);
                        e.RepositoryItem = ((isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&  data.CREATOR == loggingName) ? btnGEdit : repositoryItemButtonEdit1);
                    }
                    if (e.Column.FieldName == "Check")
                    {
                        e.RepositoryItem = checkIsPublic;

                    }
                    if (e.Column.FieldName == "Check2")
                    {
                        e.RepositoryItem = checkiskidney;
                    }

                    if (e.Column.FieldName == "Edit")
                    {
                        e.RepositoryItem = ((isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&  data.CREATOR == loggingName) ? btnGEdit_Enable : btnGEdit_Disable);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spMaxCapacity_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridviewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());

                HIS_EXP_MEST_TEMPLATE data = (HIS_EXP_MEST_TEMPLATE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        private void txtExpMestTemplateCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExpMestTemplateName.Focus();
                    txtExpMestTemplateName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtExpMestTemplateName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkEdit1.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtDescription_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    chkInStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void checkEdit1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkEdit2.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkEdit2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkTanDuoc.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// combobox thuốc vật tư
        /// </summary>

        private void InitComboMedimaty() 
        {
            try
            {
                this.ListMedicineType = new List<V_HIS_MEDICINE_TYPE>();
                this.ListMaterialType = new List<V_HIS_MATERIAL_TYPE>();
                this.listMediMatyTypeADO = new List<MediMatyTypeADO>();

                this.ListMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                if (chkTanDuoc.Checked)
                {
                    this.ListMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__TTD && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                else if (chkYHCT.Checked)
                {
                    this.ListMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.MEDICINE_LINE_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_LINE.ID__VT_YHCT && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                else
                {
                    this.ListMedicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }


                if (this.ListMedicineType != null && this.ListMedicineType.Count > 0)
                {
                    if (chkInStock.Checked)
                    {
                        this.ListMedicineType = this.ListMedicineType.Where(o => o.IS_BUSINESS != 1).ToList();

                        if (this.ListMaterialType != null && this.ListMaterialType.Count > 0)
                        {
                            this.ListMaterialType = this.ListMaterialType.Where(o => o.IS_BUSINESS != 1).ToList();
                        }

                    }
                    else if (chkOutStock.Checked)
                    {
                        this.ListMedicineType = this.ListMedicineType.Where(o => o.IS_BUSINESS == 1).ToList();

                        if (this.ListMaterialType != null && this.ListMaterialType.Count > 0)
                        {
                            this.ListMaterialType = this.ListMaterialType.Where(o => o.IS_BUSINESS == 1).ToList();
                        }
                    }
                }

                if (this.ListMedicineType != null && this.ListMedicineType.Count > 0)
                {
                    foreach (var Medi in this.ListMedicineType)
                    {
                        MediMatyTypeADO ado = new MediMatyTypeADO(Medi);
                        this.listMediMatyTypeADO.Add(ado);
                    }
                }

                if (this.ListMaterialType != null && this.ListMaterialType.Count > 0)
                {
                    foreach (var Mater in this.ListMaterialType)
                    {
                        MediMatyTypeADO ado = new MediMatyTypeADO(Mater);
                        this.listMediMatyTypeADO.Add(ado);
                    }
                }

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã", 250, 1));
                //columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên", 350, 2));
                //columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "Đơn vị", 150, 3));
                //columnInfos.Add(new ColumnInfo("CONCENTRA", "Hàm lượng", 150, 4));
                //columnInfos.Add(new ColumnInfo("ACTIVE_INGR_BHYT_NAME", "Hoạt chất", 100, 5));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, true, 1000);
                //ControlEditorLoader.Load(cboChooseMediMate, this.listMediMatyTypeADO, controlEditorADO);

                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.GridControl.DataSource = this.listMediMatyTypeADO;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Load combobox đường dùng
        /// </summary>
        private void InitComboMedicineUseForm() 
        {
            try
            {
                this.listMedicineUseForm = new List<HIS_MEDICINE_USE_FORM>();
                this.listMedicineUseForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_USE_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_USE_FORM_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_USE_FORM_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboMedicineUseForm, this.listMedicineUseForm, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        /// <summary>
        /// Load combobox cách dùng
        /// </summary>
        private void InitComboHtu()
        {
            try
            {
                CommonParam param = new CommonParam();
                this.listHisHtu = new List<HIS_HTU>();
                //this.listHisHtu = BackendDataWorker.Get<HIS_HTU>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                HisHtuFilter filter = new HisHtuFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                this.listHisHtu = new BackendAdapter(param).Get<List<HIS_HTU>>("api/HisHtu/Get", ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HTU_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HTU_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboHtuName, this.listHisHtu, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTanDuoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboMedimaty();
                ResetMediMaty();
                gridViewAdd.BeginUpdate();
                gridViewAdd.GridControl.DataSource = null;
                gridViewAdd.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkYHCT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkYHCT.Checked)
                {
                    lciSoThang.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else 
                {
                    lciSoThang.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                
                InitComboMedimaty();
                ResetMediMaty();
                gridViewAdd.BeginUpdate();
                gridViewAdd.GridControl.DataSource = null;
                gridViewAdd.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInStock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboMedimaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void chkOutStock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboMedimaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSang_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTrua_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChieu_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtToi_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSang_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTrua_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChieu_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtToi_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSang_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTrua_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChieu_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtToi_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSang_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTrua_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChieu_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtToi_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnAmount_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnadd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled) return;

                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                this.positionHandleAdd = -1;

                if (!btnAdd.Enabled)
                    return;

                if (!dxValidationProviderAdd.Validate())
                    return;
                WaitingManager.Show();
                MediMatyTypeADO ado = new MediMatyTypeADO();

                UpdateADOFromDataForm(ref ado);

                if (ado == null || ado.ID < 0) return;
                
                if (this.actionBosung == GlobalVariables.ActionAdd)
                {
                    this.listMediMatyTypeADOchoose.Add(ado);
                }
                else 
                {
                    if (this.listMediMatyTypeADOchoose != null && this.listMediMatyTypeADOchoose.Count > 0)
                    {
                        this.listMediMatyTypeADOchoose.Remove(this.MediMatyTypeADO);
                    }
                    this.listMediMatyTypeADOchoose.Add(ado);
                }

                gridViewAdd.BeginUpdate();
                gridViewAdd.GridControl.DataSource = this.listMediMatyTypeADOchoose;
                gridViewAdd.EndUpdate();

                if (this.listMediMatyTypeADOchoose != null && this.listMediMatyTypeADOchoose.Count > 0)
                {
                    btnSave.Enabled = true;
                }
                else 
                {
                    btnSave.Enabled = false;
                }

                ResetMediMaty();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void ResetMediMaty()
        {
            try
            {
                this.actionBosung = GlobalVariables.ActionAdd;
                cboChooseMediMate1.Text = "";
                txtSang.Text = "";
                txtTrua.Text = "";
                txtChieu.Text = "";
                txtToi.Text = "";
                cboHtuName.EditValue = null;
                cboMedicineUseForm.EditValue = null;
                txtAmount.Text = "";
                txtTutorial.Text = "";
                this.VisibleButton(this.actionBosung);
                cboChooseMediMate1.Focus();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void UpdateADOFromDataForm(ref ADO.MediMatyTypeADO data)
        {
            try
            {
                data = this.MediMatyTypeADO;
                data.MEDICINE_USE_FORM_ID = (long?)cboMedicineUseForm.EditValue;
                data.HTU_ID = (long?)cboHtuName.EditValue;
                data.Sang = txtSang.Text.Trim();
                data.Trua = txtTrua.Text.Trim();
                data.Chieu = txtChieu.Text.Trim();
                data.Toi = txtToi.Text.Trim();
                data.AMOUNT = (!String.IsNullOrEmpty(txtAmount.Text.Trim())) ? decimal.Parse(txtAmount.Text.Trim()) : (decimal?)null;
                data.TUTORIAL = txtTutorial.Text.Trim();
                data.IsOutMediStock = chkOutStock.Checked ? (short?)1 : (short?)0;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtAmount_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                e.ErrorText = ResourceMessage.TruongSoLuongNhapSaiDinhDang;
                e.ExceptionMode = ExceptionMode.DisplayError;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                this.SpinAmountKeyPress(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAmount_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                this.SpinValidating(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAdd_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    this.MediMatyTypeADO = (MediMatyTypeADO)this.gridViewAdd.GetFocusedRow();
            //    if (this.MediMatyTypeADO != null )
            //    {
            //        this.actionBosung = GlobalVariables.ActionEdit;

            //        if (this.MediMatyTypeADO.IS_BUSINESS != 1)
            //        {
            //            chkInStock.CheckState = CheckState.Checked;
            //        }
            //        else
            //        {
            //            chkOutStock.CheckState = CheckState.Checked;
            //        }

            //        cboChooseMediMate.EditValue = this.MediMatyTypeADO.ID;

            //        if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Sang))
            //            this.txtSang.EditValue = this.MediMatyTypeADO.Sang;
            //        else
            //            this.txtSang.EditValue = null;
            //        if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Trua))
            //            this.txtTrua.EditValue = this.MediMatyTypeADO.Trua;
            //        else
            //            this.txtTrua.EditValue = null;
            //        if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Chieu))
            //            this.txtChieu.EditValue = this.MediMatyTypeADO.Chieu;
            //        else
            //            this.txtChieu.EditValue = null;
            //        if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Toi))
            //            this.txtToi.EditValue = this.MediMatyTypeADO.Toi;
            //        else
            //            this.txtToi.EditValue = null;
            //        //Tự động hiển thi số lượng là phân số nếu AMOUNT là số thập phân
            //        //Vd: AMOUNT = 0.25 --> spinAmount.Text = 1/4
            //        //Ngược lại nếu là số nguyên thì hiển thị giữ nguyên giá trị     
            //        this.txtAmount.EditValue = ConvertNumber.ConvertDecToFracByConfig((double)(this.MediMatyTypeADO.AMOUNT ?? 0));
            //        this.txtTutorial.Text = this.MediMatyTypeADO.TUTORIAL;
            //        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderAdd, this.dxErrorProvider);
            //        this.VisibleButton(this.actionBosung);

            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);                
            //}
        }

        private void VisibleButton(int action)
        {
            try
            {
                if (action == GlobalVariables.ActionAdd)
                    this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                else
                    this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExpMestTemplate.btnAdd.Text.Update", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderAdd_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleAdd == -1)
                {
                    positionHandleAdd = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandleAdd > edit.TabIndex)
                {
                    positionHandleAdd = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAdd_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MediMatyTypeADO data_ManuMedicineADO = (MediMatyTypeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data_ManuMedicineADO != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IS_AUTO_EXPEND_STR")
                        {
                            e.Value = data_ManuMedicineADO.IS_AUTO_EXPEND == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;

                CommonParam param = new CommonParam();
                try
                {
                    this.positionHandle = -1;

                    bool success = false;

                    if (!dxValidationProviderEditorInfo.Validate())
                    return;

                    WaitingManager.Show();
                    MOS.SDO.HisExpMestTemplateSDO sdo = new MOS.SDO.HisExpMestTemplateSDO();
                    
                    MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE updateDTO = new MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE();

                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        LoadCurrent(this.currentData.ID, ref updateDTO);
                    }
                    UpdateDTOFromDataForm(updateDTO, ref sdo);
                    
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        sdo.ExpMestTemplate.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_CREATE, ApiConsumers.MosConsumer, sdo, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            ResetFormData();
                            ResetMediMaty();
                            gridViewAdd.BeginUpdate();
                            gridViewAdd.GridControl.DataSource = null;
                            gridViewAdd.EndUpdate();
                        }
                    }
                    else
                    {
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE>(HisRequestUriStore.MOSHIS_EXP_MEST_TEMPLATE_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            ResetFormData();
                            ResetMediMaty();
                            gridViewAdd.BeginUpdate();
                            gridViewAdd.GridControl.DataSource = null;
                            gridViewAdd.EndUpdate();
                        }
                    }

                    if (success)
                    {
                        
                        this.ActionType = GlobalVariables.ActionAdd;
                        BackendDataWorker.Reset<HIS_EXP_MEST_TEMPLATE>();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnGEdit_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXP_MEST_TEMPLATE)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    WaitingManager.Show();
                    currentData = rowData;
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRemedyCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTanDuoc_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkYHCT.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void chkYHCT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciSoThang.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        txtRemedyCount.Focus();
                        txtRemedyCount.SelectAll();
                    }
                    else 
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRemedyCount_KeyUp(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkInStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkOutStock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkOutStock_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboChooseMediMate1.Focus();
                    cboChooseMediMate1.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChooseMediMate_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    txtSang.Focus();
                    txtSang.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtSang_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTrua.Focus();
                    txtTrua.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTrua_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtChieu.Focus();
                    txtChieu.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtChieu_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtToi.Focus();
                    txtToi.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtToi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHtuName.Focus();
                    cboHtuName.SelectAll();
                    cboHtuName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHtuName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicineUseForm.Enabled)
                    {
                        cboMedicineUseForm.Focus();
                        cboMedicineUseForm.SelectAll();
                        cboMedicineUseForm.ShowPopup();
                    }
                    else 
                    {
                        txtAmount.Focus();
                        txtAmount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMedicineUseForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmount.Focus();
                    txtAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAmount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTutorial_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHtuName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.cboHtuName.EditValue = null;
                    this.cboHtuName.Properties.Buttons[1].Visible = false;
                    this.SetHuongDanFromSoLuongNgay();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void cboHtuName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (this.cboHtuName.EditValue != null)
                        this.cboHtuName.Properties.Buttons[1].Visible = true;
                    this.SetHuongDanFromSoLuongNgay();

                    if (this.cboMedicineUseForm.Enabled )
                    {
                        this.cboMedicineUseForm.Focus();
                        this.cboMedicineUseForm.SelectAll();
                    }
                    else
                    {
                        this.txtAmount.Focus();
                        this.txtAmount.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHtuName_Leave(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(this.cboHtuName.Text) && this.cboHtuName.EditValue != null)
                {
                    this.cboHtuName.EditValue = null;
                    this.cboHtuName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEdit10_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MediMatyTypeADO)gridViewAdd.GetFocusedRow();
                if (rowData != null)
                {
                    if (this.listMediMatyTypeADOchoose != null && this.listMediMatyTypeADOchoose.Count > 0)
                    {
                        this.listMediMatyTypeADOchoose.Remove(rowData);
                    }

                    gridViewAdd.BeginUpdate();
                    gridViewAdd.GridControl.DataSource = this.listMediMatyTypeADOchoose;
                    gridViewAdd.EndUpdate();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnResset_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.listMediMatyTypeADOchoose = new List<ADO.MediMatyTypeADO>();
                positionHandle = -1;

                //Gan gia tri mac dinh
                SetDefaultValue();
                
                SetFocusEditor();

                gridViewAdd.BeginUpdate();
                gridViewAdd.GridControl.DataSource = this.listMediMatyTypeADOchoose;
                gridViewAdd.EndUpdate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnResset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void txtSang_Leave(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTrua_Leave(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChieu_Leave(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtToi_Leave(object sender, EventArgs e)
        {
            try
            {
                this.CalculateAmount();
                this.SetHuongDanFromSoLuongNgay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                btnSave.Focus();
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void gridViewAdd_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        this.MediMatyTypeADO = (MediMatyTypeADO)view.GetRow(hi.RowHandle);

                        if (this.MediMatyTypeADO != null)
                        {
                            isShow = false;
                            this.actionBosung = GlobalVariables.ActionEdit;

                            validationTutorial();


                            if (this.MediMatyTypeADO != null && this.MediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                            {
                                lciTutorial.AppearanceItemCaption.ForeColor = Color.Brown;
                            }
                            else
                            {
                                lciTutorial.AppearanceItemCaption.ForeColor = new Color();
                            }

                            if (this.MediMatyTypeADO.IS_BUSINESS != 1)
                            {
                                chkInStock.CheckState = CheckState.Checked;
                            }
                            else
                            {
                                chkOutStock.CheckState = CheckState.Checked;
                            }

                            cboChooseMediMate1.Text = this.MediMatyTypeADO.MEDICINE_TYPE_NAME;

                            if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Sang))
                                this.txtSang.EditValue = this.MediMatyTypeADO.Sang;
                            else
                                this.txtSang.EditValue = null;
                            if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Trua))
                                this.txtTrua.EditValue = this.MediMatyTypeADO.Trua;
                            else
                                this.txtTrua.EditValue = null;
                            if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Chieu))
                                this.txtChieu.EditValue = this.MediMatyTypeADO.Chieu;
                            else
                                this.txtChieu.EditValue = null;
                            if (!String.IsNullOrEmpty(this.MediMatyTypeADO.Toi))
                                this.txtToi.EditValue = this.MediMatyTypeADO.Toi;
                            else
                                this.txtToi.EditValue = null;
                            //Tự động hiển thi số lượng là phân số nếu AMOUNT là số thập phân
                            //Vd: AMOUNT = 0.25 --> spinAmount.Text = 1/4
                            //Ngược lại nếu là số nguyên thì hiển thị giữ nguyên giá trị     
                            this.txtAmount.EditValue = ConvertNumber.ConvertDecToFracByConfig((double)(this.MediMatyTypeADO.AMOUNT ?? 0));
                            this.txtTutorial.Text = this.MediMatyTypeADO.TUTORIAL;

                            Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderAdd, this.dxErrorProvider);
                            this.VisibleButton(this.actionBosung);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnResset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnResset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void cboChooseMediMate1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.MediMatyTypeADO = new MediMatyTypeADO();
                    this.MediMatyTypeADO = (MediMatyTypeADO)this.gridViewMediMaty.GetFocusedRow();
                    if (this.MediMatyTypeADO != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();

                        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderAdd, dxErrorProvider);

                        validationTutorial();

                        if (this.MediMatyTypeADO != null && this.MediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {
                            lciTutorial.AppearanceItemCaption.ForeColor = Color.Brown;
                        }
                        else
                        {
                            lciTutorial.AppearanceItemCaption.ForeColor = new Color();
                        }

                        this.cboChooseMediMate1.Text = this.MediMatyTypeADO.MEDICINE_TYPE_NAME;

                        cboMedicineUseForm.EditValue = MediMatyTypeADO.MEDICINE_USE_FORM_ID ?? 0;
                        if (!string.IsNullOrEmpty(this.MediMatyTypeADO.TUTORIAL))
                        {
                            txtTutorial.Text = this.MediMatyTypeADO.TUTORIAL;
                        }
                        else
                        {
                            this.SetHuongDanFromSoLuongNgay();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewMediMaty.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(cboChooseMediMate1.Bounds.X, cboChooseMediMate1.Bounds.Y, cboChooseMediMate1.Bounds.Width, cboChooseMediMate1.Bounds.Height);
                    popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                    gridViewMediMaty.Focus();
                    gridViewMediMaty.FocusedRowHandle = rowHandlerNext;
                    System.Threading.Thread.Sleep(100);
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    cboChooseMediMate1.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChooseMediMate1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainerMediMaty = !isShowContainerMediMaty;
                    if (isShowContainerMediMaty)
                    {
                        Rectangle buttonBounds = new Rectangle(cboChooseMediMate1.Bounds.X, cboChooseMediMate1.Bounds.Y, cboChooseMediMate1.Bounds.Width, cboChooseMediMate1.Bounds.Height);
                        popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));

                        if (this.MediMatyTypeADO != null)
                        {
                            int rowIndex = 0;
                            var listDatas = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MediMatyTypeADO>>(Newtonsoft.Json.JsonConvert.SerializeObject(this.gridControlMediMaty.DataSource));
                            for (int i = 0; i < listDatas.Count; i++)
                            {
                                if (listDatas[i].SERVICE_ID == this.MediMatyTypeADO.SERVICE_ID)
                                {
                                    rowIndex = i;
                                    break;
                                }
                            }
                            gridViewMediMaty.FocusedRowHandle = rowIndex;
                        }
                    }
                    else
                    {
                        //popupControlContainerMediMaty.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboChooseMediMate1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(cboChooseMediMate1.Text))
                {
                    cboChooseMediMate1.Refresh();
                    if (isShowContainerMediMatyForChoose)
                    {
                        gridViewMediMaty.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainerMediMaty)
                        {
                            isShowContainerMediMaty = true;
                        }

                        //Filter data
                        gridViewMediMaty.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [SERVICE_UNIT_NAME] Like '%{0}%' OR [CONCENTRA] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%'", cboChooseMediMate1.Text);
                        //+ " OR [CONCENTRA] Like '%" + txtMediMatyForPrescription.Text + "%'"
                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewMediMaty.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewMediMaty.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewMediMaty.FocusedRowHandle = 0;
                        gridViewMediMaty.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewMediMaty.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(cboChooseMediMate1.Bounds.X, cboChooseMediMate1.Bounds.Y, cboChooseMediMate1.Bounds.Width, cboChooseMediMate1.Bounds.Height);
                        if (isShow)
                        {
                            popupControlContainerMediMaty.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 25));
                            isShow = false;
                        }

                        cboChooseMediMate1.Focus();
                    }
                    isShowContainerMediMatyForChoose = false;
                }
                else
                {
                    gridViewMediMaty.ActiveFilter.Clear();
                    this.MediMatyTypeADO = null;
                    if (!isShowContainerMediMaty)
                    {
                        popupControlContainerMediMaty.HidePopup();
                    }
                }
                this.ValidateForm();

                validationTutorial();

                if (this.MediMatyTypeADO != null && this.MediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                {
                    lciTutorial.AppearanceItemCaption.ForeColor = Color.Brown;
                }
                else
                {
                    lciTutorial.AppearanceItemCaption.ForeColor = new Color();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region HightLight result while search medi maty
        private void OnCustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view.OptionsFind.HighlightFindResults && !cboChooseMediMate1.Text.Equals(string.Empty))
            {
                CriteriaOperator op = ConvertFindPanelTextToCriteriaOperator(cboChooseMediMate1.Text, view, false);
                if (op is GroupOperator)
                {
                    string findText = cboChooseMediMate1.Text;
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }
                else if (op is FunctionOperator)
                {
                    FunctionOperator func = op as FunctionOperator;
                    CriteriaOperator colNameOperator = func.Operands[0];
                    string colName = colNameOperator.LegacyToString().Replace("[", string.Empty).Replace("]", string.Empty);
                    if (!e.Column.FieldName.StartsWith(colName)) return;

                    CriteriaOperator valueOperator = func.Operands[1];
                    string findText = valueOperator.LegacyToString().ToLower().Replace("'", "");
                    if (HiglightSubString(e, findText))
                        e.Handled = true;
                }

            }
        }

        public static CriteriaOperator ConvertFindPanelTextToCriteriaOperator(string findPanelText, GridView view, bool applyPrefixes)
        {
            if (!string.IsNullOrEmpty(findPanelText))
            {
                FindSearchParserResults parseResult = new FindSearchParser().Parse(findPanelText, GetFindToColumnsCollection(view));
                if (applyPrefixes)
                    parseResult.AppendColumnFieldPrefixes();

                return DxFtsContainsHelperAlt.Create(parseResult, FilterCondition.Contains, false);
            }
            return null;
        }

        private static ICollection GetFindToColumnsCollection(GridView view)
        {
            System.Reflection.MethodInfo mi = typeof(ColumnView).GetMethod("GetFindToColumnsCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return mi.Invoke(view, null) as ICollection;
        }

        private bool HiglightSubString(DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e, string findText)
        {
            int index = FindSubStringStartPosition(e.DisplayText, findText);
            if (index == -1)
            {
                return false;
            }

            e.Appearance.FillRectangle(e.Cache, e.Bounds);
            e.Cache.Paint.DrawMultiColorString(e.Cache, e.Bounds, e.DisplayText, GetStringWithoutQuotes(findText),
                e.Appearance, Color.Indigo, Color.Gold, true, index);
            return true;
        }

        private int FindSubStringStartPosition(string dispalyText, string findText)
        {
            string stringWithoutQuotes = GetStringWithoutQuotes(findText);
            int index = dispalyText.ToLower().IndexOf(stringWithoutQuotes);
            return index;
        }

        private string GetStringWithoutQuotes(string findText)
        {
            string stringWithoutQuotes = findText.ToLower().Replace("\"", string.Empty);
            return stringWithoutQuotes;
        }
        #endregion

        private void gridViewMediMaty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_KeyDown.1");
                    this.MediMatyTypeADO = new MediMatyTypeADO();
                    this.MediMatyTypeADO = (MediMatyTypeADO)this.gridViewMediMaty.GetFocusedRow();
                    if (this.MediMatyTypeADO != null)
                    {
                        isShowContainerMediMaty = false;
                        isShowContainerMediMatyForChoose = true;
                        popupControlContainerMediMaty.HidePopup();

                        Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderAdd, dxErrorProvider);

                        validationTutorial();

                        if (this.MediMatyTypeADO != null && this.MediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                        {
                            lciTutorial.AppearanceItemCaption.ForeColor = Color.Brown;
                        }
                        else
                        {
                            lciTutorial.AppearanceItemCaption.ForeColor = new Color();
                        }

                        this.cboChooseMediMate1.Text = this.MediMatyTypeADO.MEDICINE_TYPE_NAME;

                        cboMedicineUseForm.EditValue = MediMatyTypeADO.MEDICINE_USE_FORM_ID ?? 0;
                        if (!string.IsNullOrEmpty(this.MediMatyTypeADO.TUTORIAL))
                        {
                            txtTutorial.Text = this.MediMatyTypeADO.TUTORIAL;
                        }
                        else
                        {
                            this.SetHuongDanFromSoLuongNgay();
                        }

                    }
                    Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_KeyDown.4");
                }
                else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    this.gridViewMediMaty.Focus();
                    this.gridViewMediMaty.FocusedRowHandle = this.gridViewMediMaty.FocusedRowHandle;
                    int rowHandle = gridViewMediMaty.GetVisibleRowHandle(this.gridViewMediMaty.FocusedRowHandle);
                    if (e.KeyCode == Keys.Down)
                    {
                        rowHandle += 1;
                    }
                    else if (e.KeyCode == Keys.Up)
                    {
                        rowHandle -= 1;
                    }
                    var medicineTypeADOForEdit = gridViewMediMaty.GetRow(rowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediMaty_RowClick(object sender, RowClickEventArgs e)
        {
            try
            {
                this.MediMatyTypeADO = new ADO.MediMatyTypeADO();
                Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.1");
                this.MediMatyTypeADO = (MediMatyTypeADO)this.gridViewMediMaty.GetFocusedRow();
                if (this.MediMatyTypeADO != null)
                {
                    popupControlContainerMediMaty.HidePopup();
                    isShowContainerMediMaty = false;
                    isShowContainerMediMatyForChoose = true;

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderAdd, dxErrorProvider);

                    validationTutorial();

                    if (this.MediMatyTypeADO != null && this.MediMatyTypeADO.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC)
                    {
                        lciTutorial.AppearanceItemCaption.ForeColor = Color.Brown;
                    }
                    else
                    {
                        lciTutorial.AppearanceItemCaption.ForeColor = new Color();
                    }

                    this.cboChooseMediMate1.Text = this.MediMatyTypeADO.MEDICINE_TYPE_NAME;

                    cboMedicineUseForm.EditValue = this.MediMatyTypeADO.MEDICINE_USE_FORM_ID ?? 0;
                    if (!string.IsNullOrEmpty(this.MediMatyTypeADO.TUTORIAL))
                    {
                        txtTutorial.Text = this.MediMatyTypeADO.TUTORIAL;
                    }
                    else
                    {
                        this.SetHuongDanFromSoLuongNgay();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("gridViewMediMaty_RowClick.7");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerMediMaty_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }



    }
}
