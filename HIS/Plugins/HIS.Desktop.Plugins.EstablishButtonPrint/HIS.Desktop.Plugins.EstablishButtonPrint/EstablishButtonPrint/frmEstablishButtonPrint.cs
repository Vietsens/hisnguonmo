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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using SAR.EFMODEL.DataModels;
using SAR.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using System.Linq;
using HIS.Desktop.Plugins.EstablishButtonPrint.EstablishButtonPrint.ADO;

namespace HIS.Desktop.Plugins.EstablishButtonPrint
{
    public partial class frmEstablishButtonPrint : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int flag = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        PrintTypeCFGADO currentData;
        List<PrintTypeIDCaptionADO> glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
        List<SAR_PRINT_TYPE> glstPrintType = new List<SAR_PRINT_TYPE>();
        List<PrintTypeADO> glstPrintTypeAD0;
        List<SAR_PRINT_TYPE_CFG> glstPrintTypeCfg = new List<SAR_PRINT_TYPE_CFG>();
        List<PrintTypeCFGADO> glstPrintTypeCFGADO = new List<PrintTypeCFGADO>();
        List<ACS_MODULE> glstModule = new List<ACS_MODULE>();
        List<ACS_APPLICATION> glstApplication = new List<ACS_APPLICATION>();
        List<HIS_BRANCH> glstBranch = new List<HIS_BRANCH>();
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        string appCode, branchCode, moduleLink;
        List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG> resultInsert;
        List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG> resultUpdate;
        bool resultDelete;
        #endregion

        #region Construct
        public frmEstablishButtonPrint(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                grdPrintTypeCFG.ToolTipController = toolTipControllerGrid;

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

        private void frmEstablishButtonPrint_Load(object sender, EventArgs e)
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

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.EstablishButtonPrint.Resources.Lang", typeof(HIS.Desktop.Plugins.EstablishButtonPrint.frmEstablishButtonPrint).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModuleName.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColModuleName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModuleName.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColModuleName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colModulLink.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.colModulLink.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.colModulLink.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.colModulLink.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdControlCode.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdControlCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdControlCode.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdControlCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPrintTypeNames.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColPrintTypeNames.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPrintTypeNames.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColPrintTypeNames.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.cboApplication.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.cboApplication.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChinhanh.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.cboChinhanh.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.cboApplication1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.cboApplication1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChucnang.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.cboChinhanh1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChinhanh1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.cboChucnang.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label1.Text = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.label1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdPrintTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdPrintTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdPrintTypeCode.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdPrintTypeCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdPrintTypeName.Caption = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdPrintTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdPrintTypeName.ToolTip = Inventec.Common.Resource.Get.Value("frmEstablishButtonPrint.grdPrintTypeName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                EnableControlChanged(this.ActionType);

                cboApplication.EditValue = null;
                cboChinhanh.EditValue = null;
                cboApplication1.EditValue = null;
                cboChucnang.EditValue = null;
                cboChinhanh1.EditValue = null;
                txtButtonName.EditValue = null;
                txtPath.EditValue = null;


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
                txtApplicationCode.Focus();
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
                //dicOrderTabIndexControl.Add("txtApplicationCode", 0);
                //dicOrderTabIndexControl.Add("cboApplication1", 1);
                //dicOrderTabIndexControl.Add("txtBranchCode", 2);
                //dicOrderTabIndexControl.Add("cboChinhanh1", 3);
                //dicOrderTabIndexControl.Add("txtModuleCode", 4);
                //dicOrderTabIndexControl.Add("cboChucnang", 5);
                //dicOrderTabIndexControl.Add("txtButtonName", 6);
                //dicOrderTabIndexControl.Add("txtPath", 7);
                //dicOrderTabIndexControl.Add("txtCaption", 8);


                //if (dicOrderTabIndexControl != null)
                //{
                //    foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
                //    {
                //        SetTabIndexToControl(itemOrderTab, lcEditorInfo);
                //    }
                //}
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


        /// <summary>
        /// Ham lay du lieu theo dieu kien tim kiem va gan du lieu vao danh sach
        /// </summary>
        public void FillDataToGridControlPrintTypeCFG()
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.grdPrintTypeCFG);
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
                SarPrintTypeCfgFilter filter = new SarPrintTypeCfgFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";

                if (cboApplication.EditValue != null)
                    filter.APP_CODE__EXACT = glstApplication.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboApplication.EditValue.ToString())).APPLICATION_CODE;
                if (cboChinhanh.EditValue != null)
                    filter.BRANCH_CODE__EXACT = glstBranch.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboChinhanh.EditValue.ToString())).BRANCH_CODE; ;

                dnNavigation.DataSource = null;
                gridviewPrintTypeCFG.BeginUpdate();
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_GET, ApiConsumers.SarConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    glstPrintTypeCfg = (List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>)apiResult.Data;
                    glstPrintTypeCFGADO = ConvertData(glstPrintTypeCfg);
                    if (glstPrintTypeCFGADO != null)
                    {
                        dnNavigation.DataSource = glstPrintTypeCFGADO;
                        gridviewPrintTypeCFG.GridControl.DataSource = glstPrintTypeCFGADO;
                        //grdPrintTypeCFG.DataSource = glstPrintTypeCFGADO;

                        rowCount = (glstPrintTypeCfg == null ? 0 : glstPrintTypeCFGADO.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridviewPrintTypeCFG.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        //Group by PrintTypeCFG
        public List<PrintTypeCFGADO> ConvertData(List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG> data)
        {
            List<PrintTypeCFGADO> glstPrintTypeCFGConvert = new List<PrintTypeCFGADO>();
            List<PrintTypeIDCaptionADO> glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
            List<PrintTypeCFGADO> glstPrintTypeCFGConvertCaption = new List<PrintTypeCFGADO>();
            try
            {
                try
                {
                    foreach (SAR_PRINT_TYPE_CFG pt in data)
                    {
                        PrintTypeCFGADO ptcfg = new PrintTypeCFGADO();
                        ptcfg.APP_CODE = pt.APP_CODE;
                        ptcfg.MODULE_LINK = pt.MODULE_LINK;
                        ptcfg.CONTROL_PATH = pt.CONTROL_PATH;
                        ptcfg.CONTROL_CODE = pt.CONTROL_CODE;
                        ptcfg.BRANCH_CODE = pt.BRANCH_CODE;
                        PrintTypeIDCaptionADO ptca = new PrintTypeIDCaptionADO();
                        ptca.PRINT_TYPE_ID = pt.PRINT_TYPE_ID;
                        ptca.CAPTION = pt.CAPTION;
                        ptcfg.PRINT_TYPE_CAPTION = ptca;
                        ptcfg.IS_ACTIVE = pt.IS_ACTIVE;
                        ptcfg.CREATOR = pt.CREATOR;
                        ptcfg.CREATE_TIME = pt.CREATE_TIME;
                        ptcfg.MODIFIER = pt.MODIFIER;
                        ptcfg.MODIFY_TIME = pt.MODIFY_TIME;
                        glstPrintTypeCFGConvertCaption.Add(ptcfg);
                    }
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                }

                glstPrintTypeCFGConvert = (from m in glstPrintTypeCFGConvertCaption
                                           group m by new
                                           {
                                               m.APP_CODE,
                                               m.CONTROL_PATH,
                                               m.MODULE_LINK,
                                               m.CONTROL_CODE,
                                               m.BRANCH_CODE,
                                               m.IS_ACTIVE
                                           } into g
                                           select new PrintTypeCFGADO
                                           {
                                               APP_CODE = g.Key.APP_CODE,
                                               MODULE_LINK = g.Key.MODULE_LINK,
                                               CONTROL_PATH = g.Key.CONTROL_PATH,
                                               CONTROL_CODE = g.Key.CONTROL_CODE,
                                               BRANCH_CODE = g.Key.BRANCH_CODE,
                                               PRINT_TYPE_CAPTIONs = g.Select(pp => pp.PRINT_TYPE_CAPTION).ToList(),
                                               IS_ACTIVE = g.Key.IS_ACTIVE,
                                               CREATOR = g.Max(sp => sp.CREATOR),
                                               CREATE_TIME = g.Max(sp => sp.CREATE_TIME),
                                               MODIFIER = g.Max(sp => sp.MODIFIER),
                                               MODIFY_TIME = g.Max(sp => sp.MODIFY_TIME)

                                           }).ToList<PrintTypeCFGADO>();

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                if (MessageBox.Show("Có sự sai lệch thông tin nút in biểu in. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    glstPrintTypeCFGConvert = (from m in glstPrintTypeCFGConvertCaption
                                               group m by new
                                               {
                                                   m.APP_CODE,
                                                   m.CONTROL_PATH,
                                                   m.MODULE_LINK,
                                                   m.CONTROL_CODE,
                                                   m.BRANCH_CODE
                                               } into g
                                               select new PrintTypeCFGADO
                                               {
                                                   APP_CODE = g.Key.APP_CODE,
                                                   MODULE_LINK = g.Key.MODULE_LINK,
                                                   CONTROL_PATH = g.Key.CONTROL_PATH,
                                                   CONTROL_CODE = g.Key.CONTROL_CODE,
                                                   BRANCH_CODE = g.Key.BRANCH_CODE,
                                                   PRINT_TYPE_CAPTIONs = g.Select(pp => pp.PRINT_TYPE_CAPTION).ToList(),
                                                   IS_ACTIVE = data[0].IS_ACTIVE,
                                                   CREATOR = g.Max(sp => sp.CREATOR),
                                                   CREATE_TIME = g.Max(sp => sp.CREATE_TIME),
                                                   MODIFIER = g.Max(sp => sp.MODIFIER),
                                                   MODIFY_TIME = g.Max(sp => sp.MODIFY_TIME)


                                               }).ToList<PrintTypeCFGADO>();
                }


            }
            return glstPrintTypeCFGConvert;
        }

        

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (PrintTypeCFGADO)(grdPrintTypeCFG.DataSource as List<PrintTypeCFGADO>)[dnNavigation.Position];
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(PrintTypeCFGADO data)
        {
            try
            {
                if (data != null)
                {
                    ResetFormData();
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

        private void FillDataToEditorControl(PrintTypeCFGADO data)
        {
            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
            try
            {
                if (data != null)
                {
                    if (data.APP_CODE != null)
                    {
                        ACS_APPLICATION app = glstApplication.Where(pp => pp.APPLICATION_CODE.Equals(data.APP_CODE)).FirstOrDefault();
                        cboApplication1.EditValue = app.ID;
                        txtApplicationCode.EditValue = app.APPLICATION_CODE;
                    }
                    else
                        cboApplication1.EditValue = null;
                    if (data.BRANCH_CODE != null)
                    {
                        HIS_BRANCH br = glstBranch.Where(app => app.BRANCH_CODE.Equals(data.BRANCH_CODE)).FirstOrDefault();
                        cboChinhanh1.EditValue = br.ID;
                        txtBranchCode.EditValue = br.BRANCH_CODE;
                    }
                    else
                        cboChinhanh1.EditValue = null;
                    if (data.MODULE_LINK != null)
                    {
                        ACS_MODULE mo = glstModule.Where(app => app.MODULE_LINK.Equals(data.MODULE_LINK)).FirstOrDefault();
                        cboChucnang.EditValue = mo.ID;
                        txtModuleCode.EditValue = mo.MODULE_NAME;
                    }
                    else
                        cboChucnang.EditValue = null;
                    txtButtonName.Text = data.CONTROL_CODE;
                    txtPath.Text = data.CONTROL_PATH;

                    FillDataToGridControlPrintType(data.PRINT_TYPE_CAPTIONs);
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
                            txtButtonName.EditValue = null;
                            txtApplicationCode.EditValue = null;
                            txtBranchCode.EditValue = null;
                            txtModuleCode.EditValue = null;
                            txtPath.EditValue = null;
                            cboChucnang.EditValue = null;
                            cboChinhanh1.EditValue = null;
                            cboApplication1.EditValue = null;
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

        private void LoadCurrent(string moduleLink, string controlCode, ref List<SAR_PRINT_TYPE_CFG> currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                SarPrintTypeCfgFilter filter = new SarPrintTypeCfgFilter();
                filter.MODULE_LINK__EXACT = moduleLink;
                filter.CONTROL_CODE__EXACT = controlCode;
                currentDTO = new BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_GET, ApiConsumers.SarConsumer, filter, param);

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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
               
                //FillDataToGridControlPrintTypeCFG();
                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
               
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
                SetFocusEditor();
                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
               
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
                PrintTypeCFGADO rowData = (PrintTypeCFGADO)gridviewPrintTypeCFG.GetFocusedRow();
                List<long> glstPrintTypeCFG = new List<long>();
                List<long> glstPrintTypeCfgID = new List<long>();
                foreach (PrintTypeIDCaptionADO ptIDCaption in rowData.PRINT_TYPE_CAPTIONs)
                {
                    SAR_PRINT_TYPE_CFG printTypeCfg = glstPrintTypeCfg.Where(pp => pp.MODULE_LINK.Equals(rowData.MODULE_LINK) && pp.CONTROL_CODE.Equals(rowData.CONTROL_CODE) && pp.PRINT_TYPE_ID == ptIDCaption.PRINT_TYPE_ID).FirstOrDefault();
                    glstPrintTypeCFG.Add(printTypeCfg.ID);
                }
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_DELETE, ApiConsumers.SarConsumer, glstPrintTypeCFG, param);
                        if (success)
                        {
                            FillDataToGridControlPrintTypeCFG();
                            currentData = ((List<PrintTypeCFGADO>)grdPrintTypeCFG.DataSource).FirstOrDefault();

                            BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                            BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();


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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                btnEdit.Focus();
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
                btnAdd.Focus();
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
                List<SAR_PRINT_TYPE_CFG> updatetDTO = new List<SAR_PRINT_TYPE_CFG>();


                if (ActionType == GlobalVariables.ActionAdd)
                { 
                    UpdateDTOFromDataForm(ref updatetDTO);
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updatetDTO), updatetDTO));

                    if (flag != 1)
                    {
                        var resultData = new BackendAdapter(param).Post<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_CREATE, ApiConsumers.SarConsumer, updatetDTO, param);
                        if (resultData != null)
                        {
                            success = true;
                            FillDataToGridControlPrintTypeCFG();
                            glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                            FillDataToGridControlPrintType(glstPrintTypeIDCaption);
                            BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                            BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();
                            ResetFormData();
                        }

                    }
                   
                }
                else
                {
                    if (this.currentData != null && this.currentData.MODULE_LINK != null && this.currentData.CONTROL_CODE != null)
                    {
                        LoadCurrent(this.currentData.MODULE_LINK, this.currentData.CONTROL_CODE, ref updatetDTO);
                    }
                    // Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updatetDTO), updatetDTO));
                    List<SAR_PRINT_TYPE_CFG> glstPrintTypeNewUpdate = new List<SAR_PRINT_TYPE_CFG>();
                    UpdateDTOFromDataForm(ref glstPrintTypeNewUpdate);

                    if (flag != 1)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => glstPrintTypeNewUpdate), glstPrintTypeNewUpdate));

                        List<SAR_PRINT_TYPE_CFG> glstPrintTypeCfgInsert = new List<SAR_PRINT_TYPE_CFG>();
                        List<SAR_PRINT_TYPE_CFG> glstPrintTypeCfgUpdate = new List<SAR_PRINT_TYPE_CFG>();
                        List<long> glstPrintTypeCfgDelete = new List<long>();
                        foreach (SAR_PRINT_TYPE_CFG ptCfgOld in updatetDTO)
                        {
                            int kq = glstPrintTypeNewUpdate.FindIndex(pt => pt.PRINT_TYPE_ID == ptCfgOld.PRINT_TYPE_ID);
                            if (kq < 0)
                                glstPrintTypeCfgDelete.Add(ptCfgOld.ID);
                        }
                        foreach (SAR_PRINT_TYPE_CFG ptCfgNew in glstPrintTypeNewUpdate)
                        {
                            int kq = updatetDTO.FindIndex(pt => pt.PRINT_TYPE_ID == ptCfgNew.PRINT_TYPE_ID);
                            if (kq < 0)
                                glstPrintTypeCfgInsert.Add(ptCfgNew);
                            else
                            {
                                ptCfgNew.ID = updatetDTO[kq].ID;
                                glstPrintTypeCfgUpdate.Add(ptCfgNew);
                            }

                        }

                        if (glstPrintTypeCfgInsert.Count > 0)
                        {
                            resultInsert = new BackendAdapter(param).Post<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_CREATE, ApiConsumers.SarConsumer, glstPrintTypeCfgInsert, param);
                        } if (glstPrintTypeCfgUpdate.Count > 0)
                            resultUpdate = new BackendAdapter(param).Post<List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_UPDATE, ApiConsumers.SarConsumer, glstPrintTypeCfgUpdate, param);
                        if (glstPrintTypeCfgDelete.Count > 0)
                            resultDelete = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_DELETE, ApiConsumers.SarConsumer, glstPrintTypeCfgDelete, param);

                        if (glstPrintTypeCfgInsert.Count > 0 && glstPrintTypeCfgUpdate.Count > 0 && glstPrintTypeCfgDelete.Count > 0)
                        {
                            if (resultInsert != null && resultUpdate != null && resultDelete)
                            {
                                success = true;
                                FillDataToGridControlPrintTypeCFG();
                                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
                                ResetFormData();
                                BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                                BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();
                            }
                        }
                        if (glstPrintTypeCfgInsert.Count > 0 && glstPrintTypeCfgUpdate.Count > 0)
                        {
                            if (resultInsert != null && resultUpdate != null)
                            {
                                success = true;
                                FillDataToGridControlPrintTypeCFG();
                                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
                                ResetFormData();
                                BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                                BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();
                            }
                        }
                        if (glstPrintTypeCfgInsert.Count > 0 && glstPrintTypeCfgDelete.Count > 0)
                        {
                            if (resultInsert != null && resultDelete)
                            {
                                success = true;
                                FillDataToGridControlPrintTypeCFG();
                                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
                                ResetFormData();
                                BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                                BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();
                            }
                        }
                        if (glstPrintTypeCfgUpdate.Count > 0 && glstPrintTypeCfgDelete.Count > 0)
                        {
                            if (resultUpdate != null && resultDelete)
                            {
                                success = true;
                                FillDataToGridControlPrintTypeCFG();
                                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
                                ResetFormData();
                                BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                                BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();
                            }
                        }
                        if (glstPrintTypeCfgUpdate.Count > 0)
                        {
                            if (resultUpdate != null)
                            {
                                success = true;
                                FillDataToGridControlPrintTypeCFG();
                                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                                FillDataToGridControlPrintType(glstPrintTypeIDCaption);
                                ResetFormData();
                                BackendDataWorker.Reset<SAR_PRINT_TYPE_CFG>();
                                BackendDataWorker.Get<SAR_PRINT_TYPE_CFG>();
                            }
                        }

                    }
                    

                }

                if (success)
                {
                    SetFocusEditor();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                if (flag != 1)
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

        private void UpdateRowDataAfterEdit(SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG) is null");
                var rowData = (SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG)gridviewPrintTypeCFG.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE_CFG>(rowData, data);
                    gridviewPrintTypeCFG.RefreshRow(gridviewPrintTypeCFG.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref List<SAR_PRINT_TYPE_CFG> currentDTO)
        {
            try
            {
                //Lấy thông tin từ List<PRINT_TYPE_ID>
                glstPrintTypeIDCaption = new List<PrintTypeIDCaptionADO>();
                if (this.glstPrintTypeAD0 != null && this.glstPrintTypeAD0.Count > 0)
                {
                    List<PrintTypeIDCaptionADO> glstPTCA = (from m in this.glstPrintTypeAD0.Where(p => p.IsCheck == true)
                                                            select new PrintTypeIDCaptionADO
                                                            {
                                                                PRINT_TYPE_ID = m.ID,
                                                                CAPTION = m.CAPTION
                                                            }).ToList();
                    this.glstPrintTypeIDCaption.AddRange(glstPTCA);
                }
                if (this.glstPrintTypeIDCaption.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn cần chọn ít nhất 1 biểu in để thiết lập!", "Thông báo");
                    flag = 1;
                    return;
                }
                else
                    flag = 0;
                if (cboApplication1.EditValue != null)
                    appCode = glstApplication.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboApplication1.EditValue.ToString())).APPLICATION_CODE;

                if (cboChinhanh1.EditValue != null)
                    branchCode = glstBranch.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboChinhanh1.EditValue.ToString())).BRANCH_CODE;
                else
                    branchCode = "";
                if (cboChucnang.EditValue != null)
                    moduleLink = cboChucnang.Text;
               

                //Add list
                if (glstPrintTypeIDCaption != null && glstPrintTypeIDCaption.Count > 0)
                {
                    foreach (PrintTypeIDCaptionADO ptIDCaption in glstPrintTypeIDCaption)
                    {
                        SAR_PRINT_TYPE_CFG ptCFG = new SAR_PRINT_TYPE_CFG();
                        ptCFG.APP_CODE = appCode;
                        ptCFG.BRANCH_CODE = branchCode;
                        ptCFG.MODULE_LINK = moduleLink;
                        ptCFG.CONTROL_CODE = txtButtonName.Text;
                        ptCFG.CONTROL_PATH = txtPath.Text;
                        ptCFG.PRINT_TYPE_ID = ptIDCaption.PRINT_TYPE_ID;
                        ptCFG.CAPTION = ptIDCaption.CAPTION;
                        ptCFG.IS_ACTIVE = 1;
                        currentDTO.Add(ptCFG);
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
                ValidationControlMaxLength(txtButtonName, 50, true);
                ValidationControlMaxLength(txtPath, 4000, true);
                ValidationControlMaxLength(txtApplicationCode, 3, true);
                ValidationControlMaxLength(txtModuleCode, 100, true);
                ValidationControlMaxLength(txtBranchCode, 20, false);
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

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequired)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequired;
                validate.ErrorText = "Nhập quá kí tự cho phép";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validate);
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

        public void MeShow()
        {
            try
            {
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                InitCombo();


                //Load du lieu Print Type CFG
                FillDataToGridControlPrintTypeCFG();

                //Load du lieu Print Type
                FillDataToGridControlPrintType(glstPrintTypeIDCaption);

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

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

        private void FillDataToGridControlPrintType(List<PrintTypeIDCaptionADO> printTypeIDsCaptions)
        {
            glstPrintTypeAD0 = new List<PrintTypeADO>();
            glstPrintType = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().ToList();
            this.glstPrintTypeAD0.AddRange((from r in glstPrintType select new PrintTypeADO(r)).ToList());
            if (printTypeIDsCaptions != null && printTypeIDsCaptions.Count > 0)
            {
                foreach (PrintTypeIDCaptionADO ptCaption in printTypeIDsCaptions)
                    glstPrintTypeAD0.ForEach(ptADO => { if (ptADO.ID == ptCaption.PRINT_TYPE_ID) { ptADO.IsCheck = true; ptADO.CAPTION = ptCaption.CAPTION; } });

            }
            if (glstPrintTypeAD0 != null && glstPrintTypeAD0.Count > 0)
                grdPrintType.DataSource = glstPrintTypeAD0;
        }

        #region<Load Combo>
        private void InitCombo()
        {
            var dataApplication = BackendDataWorker.Get<ACS_APPLICATION>().Where(dt => dt.IS_ACTIVE == 1).ToList();
            //CommonParam param = new CommonParam();
            //AcsApplicationFilter filter = new AcsApplicationFilter();
            //filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
            //var data = new BackendAdapter(param).Get<List<ACS_APPLICATION>>("api/AcsApplication/Get", ApiConsumers.AcsConsumer, filter, null);

            glstApplication = new List<ACS_APPLICATION>();
            glstApplication.AddRange(dataApplication);
            InitComboApplication(dataApplication, cboApplication);
            InitComboApplication(dataApplication, cboApplication1);

            var dataBranch = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>().Where(dt => dt.IS_ACTIVE == 1).ToList();
            glstBranch = new List<HIS_BRANCH>();
            glstBranch.AddRange(dataBranch);
            InitComboBranch(dataBranch, cboChinhanh);
            InitComboBranch(dataBranch, cboChinhanh1);
            InitComboModule();
        }

        private void InitComboApplication(List<ACS_APPLICATION> data, GridLookUpEdit cbo)
        {
            try
            {
                //var data = BackendDataWorker.Get<ACS_APPLICATION>().Where(dt => dt.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("APPLICATION_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("APPLICATION_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("APPLICATION_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitComboBranch(List<HIS_BRANCH> data, GridLookUpEdit cbo)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void InitComboModule()
        {
            try
            {
                // List<ACS.EFMODEL.DataModels.ACS_MODULE> data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_MODULE>().Where(dt => dt.IS_ACTIVE == 1).ToList();
                CommonParam param = new CommonParam();
                AcsModuleFilter filter = new AcsModuleFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var data = new BackendAdapter(param).Get<List<ACS_MODULE>>("api/AcsModule/Get", ApiConsumers.AcsConsumer, filter, null).ToList();

                glstModule = new List<ACS_MODULE>();
                glstModule.AddRange(data);
                //foreach (var item in data)
                //{
                //    glstModule.Add(item);
                //}
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MODULE_NAME", "", 200, 1));
                columnInfos.Add(new ColumnInfo("MODULE_LINK", "", 350, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_LINK", "ID", columnInfos, false, 550);
                ControlEditorLoader.Load(cboChucnang, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        #region Shortcut

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
                cboApplication.Focus();
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
            SAR.SDO.ChangeLockByControlSDO success = new SAR.SDO.ChangeLockByControlSDO();
            //bool notHandler = false;
            try
            {

                PrintTypeCFGADO data = (PrintTypeCFGADO)gridviewPrintTypeCFG.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR.SDO.ChangeLockByControlSDO data1 = new SAR.SDO.ChangeLockByControlSDO();
                    data1.AppCode = data.APP_CODE;
                    data1.BranckCode = data.BRANCH_CODE;
                    data1.ControlPath = data.CONTROL_PATH;
                    data1.ModuleLink = data.MODULE_LINK;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.SDO.ChangeLockByControlSDO>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControlPrintTypeCFG();
                    }
                }
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
            SAR.SDO.ChangeLockByControlSDO success = new SAR.SDO.ChangeLockByControlSDO();
            //bool notHandler = false;
            try
            {

                PrintTypeCFGADO data = (PrintTypeCFGADO)gridviewPrintTypeCFG.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SAR.SDO.ChangeLockByControlSDO data1 = new SAR.SDO.ChangeLockByControlSDO();
                    data1.AppCode = data.APP_CODE;
                    data1.BranckCode = data.BRANCH_CODE;
                    data1.ControlPath = data.CONTROL_PATH;
                    data1.ModuleLink = data.MODULE_LINK;
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<SAR.SDO.ChangeLockByControlSDO>(HisRequestUriStore.MOSSAR_PRINT_TYPE_CFG_CHANGE_LOCK, ApiConsumers.SarConsumer, data1, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        FillDataToGridControlPrintTypeCFG();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                PrintTypeCFGADO data = (PrintTypeCFGADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                if (e.Column.FieldName == "IS_ACTIVE_STR")
                {
                    if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        e.Appearance.ForeColor = Color.Red;
                    else
                        e.Appearance.ForeColor = Color.Green;
                }
            }
        }

        #region<Event KeyDown>
        private void txtApplicationCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtApplicationCode.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtApplicationCode.Text.ToLower().Trim());
                        var data = glstApplication.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.APPLICATION_CODE.ToLower()).Equals(key)).ToList();

                        List<ACS_APPLICATION> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.APPLICATION_CODE.ToLower() == txtApplicationCode.Text).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboApplication1.EditValue = result[0].ID;
                            txtApplicationCode.Text = result[0].APPLICATION_CODE;
                            cboApplication1.Focus();
                            txtBranchCode.Focus();
                        }
                        else
                        {
                            cboApplication1.EditValue = null;
                            cboApplication1.Focus();
                            cboApplication1.ShowPopup();
                        }
                    }
                    else
                    {
                        cboApplication1.EditValue = null;
                        cboApplication1.Focus();
                        cboApplication1.ShowPopup();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void txtModuleCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtModuleCode.Text))
                    {
                        string key = Inventec.Common.String.Convert.UnSignVNese(this.txtModuleCode.Text.ToLower().Trim());
                        var data = glstModule.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.MODULE_NAME.ToLower()).StartsWith(key)).ToList();

                        List<ACS_MODULE> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.MODULE_NAME.ToLower() == txtModuleCode.Text).ToList()) : null);
                        if (result != null && result.Count == 1)
                        {
                            cboChucnang.EditValue = result[0].ID;
                            txtModuleCode.Text = result[0].MODULE_NAME;
                            cboChucnang.Focus();
                            txtButtonName.Focus();
                        }
                        else
                        {
                            cboChucnang.EditValue = null;
                            cboChucnang.Focus();
                            cboChucnang.ShowPopup();
                        }
                    }
                    else
                    {
                        cboChucnang.EditValue = null;
                        cboChucnang.Focus();
                        cboChucnang.ShowPopup();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboApplication1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboApplication1.EditValue != null && cboApplication1.EditValue != cboApplication1.OldEditValue)
                {
                    ACS_APPLICATION gt = glstApplication.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboApplication1.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtApplicationCode.Text = gt.APPLICATION_CODE;
                        txtBranchCode.Focus();
                    }
                }
                else
                {
                    txtBranchCode.Focus();
                }
            }
        }

        private void cboChinhanh1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboChinhanh1.EditValue != null && cboChinhanh1.EditValue != cboChinhanh1.OldEditValue)
                {
                    HIS_BRANCH gt = glstBranch.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboChinhanh1.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtBranchCode.Text = gt.BRANCH_CODE;
                        txtModuleCode.Focus();
                    }
                }
                else
                {
                    txtModuleCode.Focus();
                }
            }
        }

        private void cboChucnang_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (cboChucnang.EditValue != null && cboChucnang.EditValue != cboChucnang.OldEditValue)
                {
                    ACS_MODULE gt = glstModule.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboChucnang.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtModuleCode.Text = gt.MODULE_NAME;
                        txtButtonName.Focus();
                    }
                }
                else
                {
                    txtButtonName.Focus();
                }
            }
        }
        private void txtButtonName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPath.SelectAll();
                    txtPath.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        private void gridviewPrintTypeCFG_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    PrintTypeCFGADO data = (PrintTypeCFGADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "isLock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE ? btnGLock : btnGunLock);
                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnGEdit : repositoryItemButtonEdit1);

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void gridviewPrintTypeCFG_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (PrintTypeCFGADO)gridviewPrintTypeCFG.GetFocusedRow();
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

        private void gridviewPrintTypeCFG_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    PrintTypeCFGADO pData = (PrintTypeCFGADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }

                    else if (e.Column.FieldName == "MODULE_NAME_STR")
                    {
                        try
                        {
                            e.Value = BackendDataWorker.Get<ACS_MODULE>().Where(dt => dt.MODULE_LINK == pData.MODULE_LINK).FirstOrDefault().MODULE_NAME;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "PRINT_TYPE_NAMES_STR")
                    {
                        try
                        {
                            string printTypeNames = "";
                            foreach (PrintTypeIDCaptionADO printTypeIdCaption in pData.PRINT_TYPE_CAPTIONs)
                            {
                                string printTypeName = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().Where(sp => sp.ID == printTypeIdCaption.PRINT_TYPE_ID).FirstOrDefault().PRINT_TYPE_NAME;
                                if (printTypeName != null)
                                    printTypeNames = printTypeNames + printTypeName + "; \n\r";
                            }
                            e.Value = printTypeNames;
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }


                }

                grdPrintTypeCFG.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewPrintTypeCFG_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (PrintTypeCFGADO)gridviewPrintTypeCFG.GetFocusedRow();
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

        #region<Event Close Combo>
        private void cboApplication1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboApplication1.EditValue != null && cboApplication1.EditValue != cboApplication1.OldEditValue)
                    {
                        ACS_APPLICATION gt = glstApplication.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboApplication1.EditValue.ToString()));
                        if (gt != null)
                        {
                            txtApplicationCode.Text = gt.APPLICATION_CODE;
                            txtBranchCode.Focus();
                        }
                    }
                    else
                    {
                        txtBranchCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChinhanh1_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                if (cboChinhanh1.EditValue != null && cboChinhanh1.EditValue != cboChinhanh1.OldEditValue)
                {
                    HIS_BRANCH gt = glstBranch.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboChinhanh1.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtBranchCode.Text = gt.BRANCH_CODE;
                        txtModuleCode.Focus();
                    }
                }
                else
                {
                    txtModuleCode.Focus();
                }
            }
        }

        private void cboChucnang_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                if (cboChucnang.EditValue != null && cboChucnang.EditValue != cboChucnang.OldEditValue)
                {
                    ACS_MODULE gt = glstModule.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboChucnang.EditValue.ToString()));
                    if (gt != null)
                    {
                        txtModuleCode.Text = gt.MODULE_NAME;
                        txtButtonName.Focus();
                    }
                }
                else
                {
                    txtButtonName.Focus();
                }
            }
        }

        private void cboApplication_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                FillDataToGridControlPrintTypeCFG();

            }
        }

        private void cboChinhanh_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            if (e.CloseMode == PopupCloseMode.Normal)
            {
                FillDataToGridControlPrintTypeCFG();
            }
        }

        #endregion

        private void gridViewPrintType_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void cboApplication_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboApplication.Properties.Buttons[1].Visible = true;
                    cboApplication.EditValue = null;
                    FillDataToGridControlPrintTypeCFG();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChinhanh_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboChinhanh.Properties.Buttons[1].Visible = true;
                    cboChinhanh.EditValue = null;
                    FillDataToGridControlPrintTypeCFG();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChinhanh1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboChinhanh1.EditValue = null;
                    txtBranchCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void txtBranchCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(txtBranchCode.Text))
                {
                    string key = Inventec.Common.String.Convert.UnSignVNese(this.txtBranchCode.Text.ToLower().Trim());
                    var data = glstBranch.Where(o => Inventec.Common.String.Convert.UnSignVNese(o.BRANCH_CODE.ToLower()).Equals(key)).ToList();

                    List<HIS_BRANCH> result = (data != null ? ((data.Count == 1) ? data : data.Where(o => o.BRANCH_CODE.ToLower() == txtBranchCode.Text).ToList()) : null);
                    if (result != null && result.Count == 1)
                    {
                        cboChucnang.EditValue = result[0].ID;
                        txtBranchCode.Text = result[0].BRANCH_CODE;
                        cboChinhanh1.Focus();
                        txtModuleCode.Focus();
                    }
                    else
                    {
                        cboChinhanh1.EditValue = null;
                        cboChinhanh1.Focus();
                        cboChinhanh1.ShowPopup();
                    }
                }
                else
                {
                    cboChinhanh1.EditValue = null;
                    cboChinhanh1.Focus();
                    cboChinhanh1.ShowPopup();

                }
            }
        }


    }
}
