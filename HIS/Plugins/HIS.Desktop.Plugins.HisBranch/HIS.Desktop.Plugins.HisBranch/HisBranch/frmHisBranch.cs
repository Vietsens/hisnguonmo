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
using HIS.Desktop.Utility;
using System.IO;
using Inventec.Fss.Client;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LibraryMessage;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisBranch.ADO;
using System.Xml;
using System.Text;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisBranch.HisBranch
{
    public partial class frmHisBranch : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_BRANCH currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        List<ManagerADO> listManager = new List<ManagerADO>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<ManagerADO> ManagerADOs = new List<ManagerADO>();
        byte[] byteImgs;
        #endregion

        #region Construct
        public frmHisBranch(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisBranch_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBranch.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBranch.HisBranch.frmHisBranch).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBranchCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColBranchCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBranchCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColBranchCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBranchName.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColBranchName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBranchName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColBranchName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinMediOrgCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColHeinMediOrgCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinMediOrgCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColHeinMediOrgCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAcceptHeinMediOrgCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAcceptHeinMediOrgCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAcceptHeinMediOrgCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAcceptHeinMediOrgCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAddress.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAddress.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAddress.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAddress.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColParentOrganizationName.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColParentOrganizationName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColParentOrganizationName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColParentOrganizationName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinProvinceCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColHeinProvinceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinProvinceCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColHeinProvinceCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinLevelCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColHeinLevelCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHeinLevelCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColHeinLevelCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColNoBHYT.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColNoBHYT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNoBHYT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColNoBHYT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColTaxCode.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColTaxCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTaxCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColTaxCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAccountNumber.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAccountNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAccountNumber.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAccountNumber.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPhone.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColPhone.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPhone.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColPhone.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBranch.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.btnEdit.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lcRepresentative.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lcRepresentative.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcPosition.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lcPosition.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcAuthLetterIssueDate.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lcAuthLetterIssueDate.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcAuthLetterNum.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lcAuthLetterNum.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBankInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lcBankInfo.Text",
Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.lciBranchCode.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciBranchCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBranchName.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciBranchName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinMediOrgCode.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciHeinMediOrgCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciAcceptHeinMediOrgCode.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciAcceptHeinMediOrgCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciAddress.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciParentOrganizationName.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciParentOrganizationName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinProvinceCode.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciHeinProvinceCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciHeinLevelCode.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciHeinLevelCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciTaxCode.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciTaxCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciAccountNumber.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciAccountNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lciPhone.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.lciPhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBranch.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColRepresentative.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColRepresentative.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRepresentative.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColRepresentative.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPosition.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColPosition.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPosition.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColPosition.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAuthLetterIssueDate.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAuthLetterIssueDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAuthLetterIssueDate.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAuthLetterIssueDate.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAuthLetterNum.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAuthLetterNum.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAuthLetterNum.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColAuthLetterNum.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBankInfo.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColBankInfo.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBankInfo.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.grdColBankInfo.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisBranch.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem16.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.layoutControlItem16.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTheBrankCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.txtTheBrankCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtNoBHYT.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBranch.txtNoBHYT.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // khóa, bỏ khóa
        private void LockUnlockBranch()
        {

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
                dicOrderTabIndexControl.Add("txtBranchCode", 0);
                dicOrderTabIndexControl.Add("txtBranchName", 1);
                dicOrderTabIndexControl.Add("txtHeinMediOrgCode", 2);
                dicOrderTabIndexControl.Add("txtAcceptHeinMediOrgCode", 3);
                dicOrderTabIndexControl.Add("txtSYS_MEDI_ORG_CODE", 4);
                dicOrderTabIndexControl.Add("txtProvince", 5);
                dicOrderTabIndexControl.Add("cboProvince", 6);
                dicOrderTabIndexControl.Add("txtDistricts", 7);
                dicOrderTabIndexControl.Add("cboDistricts", 8);
                dicOrderTabIndexControl.Add("txtCommune", 9);
                dicOrderTabIndexControl.Add("cboCommune", 10);
                dicOrderTabIndexControl.Add("txtAddress", 11);
                dicOrderTabIndexControl.Add("txtParentOrganizationName", 12);
                dicOrderTabIndexControl.Add("txtHeinProvinceCode", 13);
                dicOrderTabIndexControl.Add("txtHeinLevelCode", 14);
                dicOrderTabIndexControl.Add("txtNoBHYT", 15);
                dicOrderTabIndexControl.Add("txtTaxCode", 16);
                dicOrderTabIndexControl.Add("txtAccountNumber", 17);
                dicOrderTabIndexControl.Add("txtPhone", 18);
                dicOrderTabIndexControl.Add("txtPhone", 19);
                dicOrderTabIndexControl.Add("txtPhone", 20);
                dicOrderTabIndexControl.Add("txtPhone", 21);
                dicOrderTabIndexControl.Add("txtPhone", 22);
                dicOrderTabIndexControl.Add("txtPhone", 23);
                dicOrderTabIndexControl.Add("chkUseBranchTime", 24);
                dicOrderTabIndexControl.Add("chkPay", 24);



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
        private void LoadDataToCombo()
        {
            try
            {

                List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResultProvince = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                FillDataToLookupedit(this.cboProvince, "PROVINCE_NAME", "PROVINCE_CODE", "PROVINCE_CODE", BackendDataWorker.Get<V_SDA_PROVINCE>());

                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResultDistricts = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                FillDataToLookupedit(this.cboDistricts, "DISTRICT_NAME", "DISTRICT_CODE", "DISTRICT_CODE", BackendDataWorker.Get<V_SDA_DISTRICT>());

                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResultCommune = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                FillDataToLookupedit(this.cboCommune, "COMMUNE_NAME", "COMMUNE_CODE", "COMMUNE_CODE", BackendDataWorker.Get<V_SDA_COMMUNE>());



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDatacboManager()
        {
            try
            {
                var Employee = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(o => o.IS_ACTIVE == 1).ToList();
                var acsUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();

                if (Employee != null && Employee.Count() > 0)
                {
                    List<ACS_USER> source = acsUser.Where(o => Employee.Exists(e => e.LOGINNAME == o.LOGINNAME)).ToList();
                    if (source != null && source.Count() > 0)
                    {
                        ManagerADOs.AddRange((from n in source select new ManagerADO(n)).ToList());
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 150, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 400);
                ControlEditorLoader.Load(cboManager, ManagerADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToControlsForm()
        {
            try
            {
                LoadDataToCombo();
                LoadDatacboManager();
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
            //cboManager.EditValue = emp.LOGINNAME;
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_BRANCH>> apiResult = null;
                HisBranchFilter filter = new HisBranchFilter();
                SetFilterNavBar(ref filter);
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_BRANCH>>(HisRequestUriStore.MOSHIS_BRANCH_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_BRANCH>)apiResult.Data;
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

        private void SetFilterNavBar(ref HisBranchFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.HIS_BRANCH pData = (MOS.EFMODEL.DataModels.HIS_BRANCH)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "AUTH_LETTER_ISSUE_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.AUTH_LETTER_ISSUE_DATE ?? 0);
                    }
                    else if (e.Column.FieldName == "SYS_MEDI_ORG_CODE")
                    {
                        e.Value = pData.SYS_MEDI_ORG_CODE;
                    }
                    gridControlFormList.RefreshDataSource();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_BRANCH data)
        {
            try
            {
                if (data != null)
                {
                    byteImgs = null;
                    pBLogo.Image = null;
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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_BRANCH data)
        {
            try
            {
                if (data != null)
                {

                    txtBranchCode.Text = data.BRANCH_CODE;
                    txtBranchName.Text = data.BRANCH_NAME;
                    txtHeinMediOrgCode.Text = data.HEIN_MEDI_ORG_CODE;
                    txtAcceptHeinMediOrgCode.Text = data.ACCEPT_HEIN_MEDI_ORG_CODE;
                    txtSYS_MEDI_ORG_CODE.Text = data.SYS_MEDI_ORG_CODE;
                    txtProvince.Text = data.PROVINCE_CODE;
                    cboProvince.EditValue = data.PROVINCE_CODE;
                    txtDistricts.Text = data.DISTRICT_CODE;
                    cboDistricts.EditValue = data.DISTRICT_CODE;
                    txtCommune.Text = data.COMMUNE_CODE;
                    cboCommune.EditValue = data.COMMUNE_CODE;
                    txtAddress.Text = data.ADDRESS;
                    txtParentOrganizationName.Text = data.PARENT_ORGANIZATION_NAME;
                    txtHeinProvinceCode.Text = data.HEIN_PROVINCE_CODE;
                    txtHeinLevelCode.Text = data.HEIN_LEVEL_CODE;
                    txtNoBHYT.Text = data.DO_NOT_ALLOW_HEIN_LEVEL_CODE;
                    txtTaxCode.Text = data.TAX_CODE;
                    txtAccountNumber.Text = data.ACCOUNT_NUMBER;
                    txtPhone.Text = data.PHONE;
                    txtRepresentative.Text = data.REPRESENTATIVE;
                    txtPosition.Text = data.POSITION;
                    txtAuthLetterNum.Text = data.AUTH_LETTER_NUM;
                    txtAuthLetterIssueDate.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.AUTH_LETTER_ISSUE_DATE ?? 0);
                    txtBankInfo.Text = data.BANK_INFO;
                    txtTheBrankCode.Text = data.THE_BRANCH_CODE;
                    cboManager.EditValue = data.DIRECTOR_LOGINNAME;

                    if (data.IS_WARNING_WRONG_ROUTE_FEE == 1)
                    {
                        chkPay.Checked = true;
                    }
                    else
                    {
                        chkPay.Checked = false;
                    }

                    if (data.IS_USE_BRANCH_TIME == 1)
                    {
                        chkUseBranchTime.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        chkUseBranchTime.CheckState = CheckState.Unchecked;
                    }

                    if (!string.IsNullOrEmpty(data.LOGO_URL))
                    {
                        MemoryStream ms = Inventec.Fss.Client.FileDownload.GetFile(data.LOGO_URL);
                        if (ms != null)
                        {
                            pBLogo.Image = Image.FromStream(ms);
                        }
                    }
                    txtRepresentativeHeinCode.Text = data.REPRESENTATIVE_HEIN_CODE;
                    cboType.SelectedIndex = data.TYPE != null ? (int)data.TYPE - 1 : -1;
                    cboVenture.SelectedIndex = data.VENTURE != null ? (int)data.VENTURE - 1 : -1;
                    cboForm.SelectedIndex = data.FORM != null ? (int)data.FORM - 1 : -1;
                    spnBedActual.EditValue = data.BED_ACTUAL;
                    spnBedApproved.EditValue = data.BED_APPROVED;
                    spnBedResuscitation.EditValue = data.BED_RESUSCITATION;
                    spnBedResuscitationEmg.EditValue = data.BED_RESUSCITATION_EMG;
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("data:____", data));
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
                txtBranchCode.Focus();
                txtBranchCode.SelectAll();
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
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit )
                        {
                            if (lci.Control is DevExpress.XtraEditors.CheckEdit)
                                continue;

                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;

                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                    chkUseBranchTime.CheckState = CheckState.Unchecked;
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_BRANCH currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBranchFilter filter = new HisBranchFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_BRANCH>>(HisRequestUriStore.MOSHIS_BRANCH_GET, ApiConsumers.MosConsumer, filter, null).ToList().First();//param , FirstOrDefault
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
                txtBranchCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
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
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_BRANCH_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_BRANCH>();
                            FillDataToGridControl();
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                btnExportXML.Enabled = false;
                ResetFormData();
                this.currentData = null;
                byteImgs = null;
                pBLogo.Image = null;
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
        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_BRANCH currentDTO)
        {
            try
            {
                currentDTO.BRANCH_CODE = txtBranchCode.Text.Trim();
                currentDTO.BRANCH_NAME = txtBranchName.Text.Trim();
                currentDTO.HEIN_MEDI_ORG_CODE = txtHeinMediOrgCode.Text != null ? txtHeinMediOrgCode.Text.Trim() : null;
                currentDTO.ACCEPT_HEIN_MEDI_ORG_CODE = txtAcceptHeinMediOrgCode.Text != null ? txtAcceptHeinMediOrgCode.Text.Trim() : null;
                currentDTO.SYS_MEDI_ORG_CODE = txtSYS_MEDI_ORG_CODE.Text != null ? txtSYS_MEDI_ORG_CODE.Text : null;
                currentDTO.PROVINCE_CODE = txtProvince.Text != null ? txtProvince.Text : null;
                currentDTO.PROVINCE_NAME = GetProvincesByCombo(cboProvince);
                currentDTO.DISTRICT_CODE = txtDistricts.Text != null ? txtDistricts.Text : null;
                currentDTO.DISTRICT_NAME = GetDistrictsByCombo(cboDistricts);
                currentDTO.COMMUNE_CODE = txtCommune.Text != null ? txtCommune.Text : null;
                currentDTO.COMMUNE_NAME = GetCommuneByCombo(cboCommune);
                if (ManagerADOs != null && ManagerADOs.Count > 0 && cboManager.EditValue != null)
                {
                    var user = ManagerADOs.FirstOrDefault(o => o.LOGINNAME.ToUpper() == cboManager.EditValue.ToString().ToUpper());
                    currentDTO.DIRECTOR_LOGINNAME = user != null ? user.LOGINNAME : "";
                    currentDTO.DIRECTOR_USERNAME = user != null ? user.USERNAME : "";
                }
                currentDTO.ADDRESS = txtAddress.Text != null ? txtAddress.Text : null;
                currentDTO.PARENT_ORGANIZATION_NAME = txtParentOrganizationName.Text != null ? txtParentOrganizationName.Text.Trim() : null;
                currentDTO.HEIN_PROVINCE_CODE = txtHeinProvinceCode.Text != null ? txtHeinProvinceCode.Text.Trim() : null;
                currentDTO.HEIN_LEVEL_CODE = txtHeinLevelCode.Text != null ? txtHeinLevelCode.Text.Trim() : null;
                currentDTO.DO_NOT_ALLOW_HEIN_LEVEL_CODE = txtNoBHYT.Text != null ? txtNoBHYT.Text.Trim() : null;
                currentDTO.TAX_CODE = txtTaxCode.Text != null ? txtTaxCode.Text.Trim() : null;
                currentDTO.ACCOUNT_NUMBER = txtAccountNumber.Text != null ? txtAccountNumber.Text.Trim() : null;
                currentDTO.PHONE = txtPhone.Text != null ? txtPhone.Text.Trim() : null;
                currentDTO.REPRESENTATIVE = txtRepresentative.Text != null ? txtRepresentative.Text.Trim() : null;
                currentDTO.POSITION = txtPosition.Text != null ? txtPosition.Text.Trim() : null;
                currentDTO.AUTH_LETTER_NUM = txtAuthLetterNum.Text != null ? txtAuthLetterNum.Text.Trim() : null;
                if (txtAuthLetterIssueDate.EditValue != null)
                {
                    currentDTO.AUTH_LETTER_ISSUE_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(txtAuthLetterIssueDate.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    currentDTO.AUTH_LETTER_ISSUE_DATE = null;
                }
                currentDTO.BANK_INFO = txtBankInfo.Text != null ? txtBankInfo.Text.Trim() : null;
                currentDTO.THE_BRANCH_CODE = txtTheBrankCode.Text != null ? txtTheBrankCode.Text.Trim() : null;

                if (chkPay.Checked)
                {
                    currentDTO.IS_WARNING_WRONG_ROUTE_FEE = 1;
                }
                else
                {
                    currentDTO.IS_WARNING_WRONG_ROUTE_FEE = null;
                }

                if (chkUseBranchTime.Checked)
                {
                    currentDTO.IS_USE_BRANCH_TIME = 1;
                }
                else
                {
                    currentDTO.IS_USE_BRANCH_TIME = null;
                }
                currentDTO.REPRESENTATIVE_HEIN_CODE = !string.IsNullOrEmpty(txtRepresentativeHeinCode.Text) ? txtRepresentativeHeinCode.Text : null;
                currentDTO.VENTURE = cboVenture.SelectedIndex != -1 ? (short?)(cboVenture.SelectedIndex + 1) : null;
                currentDTO.TYPE = cboType.SelectedIndex != -1 ? (short?)(cboType.SelectedIndex + 1) : null;
                currentDTO.FORM = cboForm.SelectedIndex != -1 ? (short?)(cboForm.SelectedIndex + 1) : null;
                currentDTO.BED_APPROVED = spnBedApproved.EditValue != null ? (int?)spnBedApproved.Value : null;
                currentDTO.BED_ACTUAL = spnBedActual.EditValue != null ? (int?)spnBedActual.Value : null;
                currentDTO.BED_RESUSCITATION = spnBedResuscitation.EditValue != null ? (int?)spnBedResuscitation.Value : null;
                currentDTO.BED_RESUSCITATION_EMG = spnBedResuscitationEmg.EditValue != null ? (int?)spnBedResuscitationEmg.Value : null;
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
                HisBranchSDO sdo = new HisBranchSDO();
                MOS.EFMODEL.DataModels.HIS_BRANCH updateDTO = new MOS.EFMODEL.DataModels.HIS_BRANCH();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                sdo.Branch = updateDTO;
                if (byteImgs != null && byteImgs.Count() > 0)
                {
                    sdo.ImageData = byteImgs;
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sdo:____", sdo));
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BRANCH>(HisRequestUriStore.MOSHIS_BRANCH_CREATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_BRANCH>(HisRequestUriStore.MOSHIS_BRANCH_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                    if (resultData != null)
                    {
                        success = true;
                        //UpdateRowDataAfterEdit(resultData);
                        FillDataToGridControl();
                        currentData = resultData;
                    }
                }

                if (success)
                {
                    BackendDataWorker.Reset<HIS_BRANCH>();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_BRANCH data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_BRANCH) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BRANCH>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetProvincesByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    var provinces = BackendDataWorker.Get<V_SDA_PROVINCE>();
                    if (provinces != null)
                    {
                        edit = (provinces.FirstOrDefault(o => o.PROVINCE_CODE == cbo.EditValue.ToString()) ?? new V_SDA_PROVINCE()).PROVINCE_NAME;
                    }
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }
        private string GetDistrictsByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    var districts = BackendDataWorker.Get<V_SDA_DISTRICT>();
                    if (districts != null)
                    {
                        var selectedDistrict = districts.FirstOrDefault(o => o.DISTRICT_CODE == cbo.EditValue.ToString()) ?? new V_SDA_DISTRICT();
                        edit = selectedDistrict.INITIAL_NAME + " " + selectedDistrict.DISTRICT_NAME;
                    }
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }
        private string GetCommuneByCombo(LookUpEdit cbo)
        {
            string edit = "";
            try
            {
                if (cbo.EditValue != null)
                {
                    var commune = BackendDataWorker.Get<V_SDA_COMMUNE>();
                    if (commune != null)
                    {
                        var selectedCommune = commune.FirstOrDefault(o => o.COMMUNE_CODE == cbo.EditValue.ToString()) ?? new V_SDA_COMMUNE();
                        edit = selectedCommune.INITIAL_NAME + " " + selectedCommune.COMMUNE_NAME;
                    }
                }
                else
                {
                    edit = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }

            return edit;
        }

        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationControlMaxLength(txtBranchCode, 6, true);
                ValidationControlMaxLength(txtBranchName, 100, true);
                ValidationControlMaxLength(txtHeinMediOrgCode, 6, false);
                ValidationControlMaxLength(txtAcceptHeinMediOrgCode, 4000, false);
                ValidationControlMaxLength(txtSYS_MEDI_ORG_CODE, 4000, false);
                ValidationControlMaxLength(txtAddress, 500, false);
                ValidationControlMaxLength(txtParentOrganizationName, 100, false);
                ValidationControlMaxLength(txtHeinProvinceCode, 2, false);
                ValidationControlMaxLength(txtHeinLevelCode, 1, false);
                ValidationControlMaxLength(txtTaxCode, 20, false);
                ValidationControlMaxLength(txtAccountNumber, 50, false);
                ValidationControlMaxLength(txtPhone, 20, false);
                ValidationControlMaxLength(txtRepresentative, 200, false);
                ValidationControlMaxLength(txtPosition, 100, false);
                ValidationControlMaxLength(txtAuthLetterIssueDate, 14, false);
                ValidationControlMaxLength(txtAuthLetterNum, 50, false);
                ValidationControlMaxLength(txtBankInfo, 300, false);
                ValidationControlMaxLength(txtTheBrankCode, 20, false);
                ValidationRepresentativeHeinCode();
                ValidationControlWarnigProvince();
                ValidationControlWarnigDistricts();
                ValidationControlWarnigCommune();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlWarnigCommune()
        {
            ValidationWarningText validation = new ValidationWarningText();
            validation.textEdit = txtCommune;
            validation.cbo = cboCommune;
            dxValidationProviderEditorInfo.SetValidationRule(txtCommune, validation);
        }

        private void ValidationControlWarnigProvince()
        {
            ValidationWarningText validation = new ValidationWarningText();
            validation.textEdit = txtProvince;
            validation.cbo = cboProvince;
            dxValidationProviderEditorInfo.SetValidationRule(txtProvince, validation);
        }

        private void ValidationControlWarnigDistricts()
        {
            ValidationWarningText validation = new ValidationWarningText();
            validation.textEdit = txtDistricts;
            validation.cbo = cboDistricts;
            dxValidationProviderEditorInfo.SetValidationRule(txtDistricts, validation);
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationRepresentativeHeinCode()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.editor = txtRepresentativeHeinCode;
                validRule.maxLength = 20;
                dxValidationProviderEditorInfo.SetValidationRule(txtRepresentativeHeinCode, validRule);
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
                btnExportXML.Enabled = false;
                //Gan gia tri mac dinh
                SetDefaultValue();
                ResetFormData();
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

                LoadComboManager();

                //Focus default
                SetDefaultFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboManager()
        {

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

        private void LoadProvinceCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboDistricts.Properties.DataSource = null;
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboProvince.EditValue = null;
                    FocusShowPopup(cboProvince);
                    //PopupLoader.SelectFirstRowPopup(cboProvince);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboProvince.EditValue = listResult[0].PROVINCE_CODE;
                        txtProvince.Text = listResult[0].PROVINCE_CODE;
                        LoadDistrictsCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtDistricts);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        cboDistricts.Properties.DataSource = null;
                        cboDistricts.EditValue = null;
                        txtDistricts.Text = "";
                        cboProvince.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboProvince);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadDistrictsCombo(string searchCode, string provinceCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                listResult = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.DISTRICT_CODE.Contains(searchCode) && (provinceCode == "" || o.PROVINCE_CODE == provinceCode)).ToList();

                cboDistricts.Properties.DataSource = listResult;
                cboDistricts.Properties.DisplayMember = "RENDERER_DISTRICT_NAME";
                cboDistricts.Properties.ValueMember = "DISTRICT_CODE";
                cboDistricts.Properties.ForceInitialize();

                cboDistricts.Properties.Columns.Clear();
                cboDistricts.Properties.Columns.Add(new LookUpColumnInfo("DISTRICT_CODE", "", 100));
                cboDistricts.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_DISTRICT_NAME", "", 200));

                cboDistricts.Properties.ShowHeader = false;
                cboDistricts.Properties.ImmediatePopup = true;
                cboDistricts.Properties.DropDownRows = 20;
                cboDistricts.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(provinceCode) && listResult.Count > 0)
                {
                    cboCommune.Properties.DataSource = null;
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    txtDistricts.Text = "";
                    cboDistricts.EditValue = null;
                    FocusShowPopup(cboDistricts);
                    //PopupProcess.SelectFirstRowPopup(cboDistricts);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboDistricts.EditValue = listResult[0].DISTRICT_CODE;
                        txtDistricts.Text = listResult[0].DISTRICT_CODE;
                        LoadCommuneCombo("", listResult[0].DISTRICT_CODE, false);
                        if (isExpand)
                        {
                            FocusMoveText(txtCommune);
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        cboCommune.Properties.DataSource = null;
                        cboCommune.EditValue = null;
                        txtCommune.Text = "";
                        txtDistricts.Text = "";
                        cboDistricts.EditValue = null;
                        if (isExpand)
                        {
                            FocusShowPopup(cboDistricts);
                            //PopupProcess.SelectFirstRowPopup(cboDistricts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadCommuneCombo(string searchCode, string districtCode, bool isExpand)
        {
            try
            {
                List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                listResult = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.COMMUNE_CODE.Contains(searchCode) && (districtCode == "" || o.DISTRICT_CODE == districtCode)).ToList();
                cboCommune.Properties.DataSource = listResult;
                cboCommune.Properties.DisplayMember = "RENDERER_COMMUNE_NAME";
                cboCommune.Properties.ValueMember = "COMMUNE_CODE";
                cboCommune.Properties.ForceInitialize();

                cboCommune.Properties.Columns.Clear();
                cboCommune.Properties.Columns.Add(new LookUpColumnInfo("COMMUNE_CODE", "", 100));
                cboCommune.Properties.Columns.Add(new LookUpColumnInfo("RENDERER_COMMUNE_NAME", "", 200));

                cboCommune.Properties.ShowHeader = false;
                cboCommune.Properties.ImmediatePopup = true;
                cboCommune.Properties.DropDownRows = 20;
                cboCommune.Properties.PopupWidth = 300;

                if (String.IsNullOrEmpty(searchCode) && String.IsNullOrEmpty(districtCode) && listResult.Count > 0)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    FocusShowPopup(cboCommune);
                    //PopupProcess.SelectFirstRowPopup(cboCommune);
                }
                else
                {
                    if (listResult.Count == 1)
                    {
                        cboCommune.EditValue = listResult[0].COMMUNE_CODE;
                        txtCommune.Text = listResult[0].COMMUNE_CODE;

                    }
                    else if (isExpand && listResult.Count > 1)
                    {
                        cboCommune.EditValue = null;
                        FocusShowPopup(cboCommune);
                        // PopupProcess.SelectFirstRowPopup(cboCommune);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void cboProvince_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (!cboProvince.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            cboProvince.EditValue = null;
        //            txtProvince.Text = "";
        //            cboProvince.Properties.Buttons[1].Visible = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void cboDistricts_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (!cboDistricts.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            cboDistricts.EditValue = null;
        //            txtDistricts.Text = "";
        //            cboDistricts.Properties.Buttons[1].Visible = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        //private void cboCommune_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        //{
        //    try
        //    {
        //        if (!cboCommune.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
        //        {
        //            cboCommune.EditValue = null;
        //            txtCommune.Text = "";
        //            cboCommune.Properties.Buttons[1].Visible = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
        private void txtProvince_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadProvinceCombo(strValue.ToUpper(), true);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtDistricts_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string provinceCode = "";
                    if (cboProvince.EditValue != null)
                    {
                        provinceCode = cboProvince.EditValue.ToString();
                    }
                    LoadDistrictsCombo(strValue.ToUpper(), provinceCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCommune_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string districtCode = "";
                    if (cboDistricts.EditValue != null)
                    {
                        districtCode = cboDistricts.EditValue.ToString();
                    }
                    LoadCommuneCombo(strValue.ToUpper(), districtCode, true);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboProvince.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboProvince.EditValue = null;
                    txtProvince.Text = "";
                    cboProvince.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboDistricts.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDistricts.EditValue = null;
                    txtDistricts.Text = "";
                    cboDistricts.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboCommune_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (!cboCommune.ReadOnly && e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCommune.EditValue = null;
                    txtCommune.Text = "";
                    cboCommune.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboProvince_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboProvince.EditValue != null && cboProvince.EditValue != cboProvince.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.PROVINCE_CODE;
                        }
                    }
                    txtDistricts.Text = "";
                    txtDistricts.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDistricts.EditValue != null && cboDistricts.EditValue != cboDistricts.OldEditValue)
                    {
                        string str = cboDistricts.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>()
                            .SingleOrDefault(o => o.DISTRICT_CODE == cboDistricts.EditValue.ToString()
                                && (String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()) || o.PROVINCE_CODE == (cboProvince.EditValue ?? "").ToString()));
                        if (district != null)
                        {
                            txtDistricts.Text = district.DISTRICT_CODE;
                            if (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                cboDistricts.EditValue = district.PROVINCE_CODE;
                                txtProvince.Text = district.PROVINCE_CODE;
                            }
                            LoadCommuneCombo("", district.DISTRICT_CODE, false);
                            txtDistricts.Text = district.DISTRICT_CODE;
                            cboCommune.EditValue = null;
                            txtCommune.Text = "";
                        }
                    }
                    FocusMoveText(this.txtCommune);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCommune_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboCommune.EditValue != null && cboCommune.EditValue != cboCommune.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>()
                            .SingleOrDefault(o =>
                                o.COMMUNE_CODE == cboCommune.EditValue.ToString()
                                    && (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()) || o.DISTRICT_CODE == (cboDistricts.EditValue ?? "").ToString())
                                );
                        if (commune != null)
                        {
                            txtCommune.Text = commune.COMMUNE_CODE;
                            if (String.IsNullOrEmpty((cboDistricts.EditValue ?? "").ToString()))
                            {
                                txtDistricts.Text = commune.DISTRICT_CODE;
                                cboDistricts.EditValue = commune.DISTRICT_CODE;
                                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.ID == commune.DISTRICT_ID).FirstOrDefault();
                                if (district != null && String.IsNullOrEmpty((cboProvince.EditValue ?? "").ToString()))
                                {

                                    txtProvince.Text = district.PROVINCE_CODE;
                                    cboCommune.EditValue = district.PROVINCE_CODE;
                                }
                            }
                        }
                    }
                    FocusMoveText(this.txtAddress);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void cboProvince_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboProvince.EditValue != null)
                    {
                        string str = cboProvince.EditValue.ToString();
                        SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().SingleOrDefault(o => o.PROVINCE_CODE == cboProvince.EditValue.ToString());
                        if (province != null)
                        {
                            LoadDistrictsCombo("", province.PROVINCE_CODE, false);
                            txtProvince.Text = province.SEARCH_CODE;
                            txtDistricts.Text = "";
                            txtDistricts.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDistricts_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboDistricts);
        }

        private void cboCommune_GetNotInListValue(object sender, GetNotInListValueEventArgs e)
        {
            GetNotInListValue(sender, e, cboCommune);
        }

        void GetNotInListValue(object sender, GetNotInListValueEventArgs e, LookUpEdit cbo)
        {
            try
            {
                if (e.FieldName == "RENDERER_DISTRICT_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.DISTRICT_NAME);
                }

                if (e.FieldName == "RENDERER_COMMUNE_NAME")
                {
                    var item = ((List<V_SDA_COMMUNE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", item.INITIAL_NAME, item.COMMUNE_NAME);
                }

                if (e.FieldName == "RENDERER_PROVINCE_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} {1}", "", item.PROVINCE_NAME);
                }

                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>)cbo.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} - {1} {2} - {3}", item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboProvince_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboProvince.Properties.Buttons[1].Visible = false;
                if (cboProvince.EditValue != null)
                {
                    cboProvince.Properties.Buttons[1].Visible = true;
                    LoadDistrictsCombo("", cboProvince.EditValue.ToString(), false);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboDistricts_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboDistricts.Properties.Buttons[1].Visible = false;
                if (cboDistricts.EditValue != null)
                {
                    cboDistricts.Properties.Buttons[1].Visible = true;
                    LoadCommuneCombo("", cboDistricts.EditValue.ToString(), false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboCommune_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboCommune.Properties.Buttons[1].Visible = false;
                if (cboCommune.EditValue != null)
                {
                    cboCommune.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void txtBranchCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBranchName.Focus();
                    txtBranchName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBranchName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinMediOrgCode.Focus();
                    txtHeinMediOrgCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinMediOrgCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAcceptHeinMediOrgCode.Focus();
                    txtAcceptHeinMediOrgCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAcceptHeinMediOrgCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSYS_MEDI_ORG_CODE.Focus();
                    txtSYS_MEDI_ORG_CODE.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtParentOrganizationName.Focus();
                    txtParentOrganizationName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtParentOrganizationName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinProvinceCode.Focus();
                    txtHeinProvinceCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinProvinceCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinLevelCode.Focus();
                    txtHeinLevelCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinLevelCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNoBHYT.Focus();
                    txtNoBHYT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtNoBHYT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTaxCode.Focus();
                    txtTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtTaxCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountNumber.Focus();
                    txtAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccountNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPhone.Focus();
                    txtPhone.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtRepresentative.Focus();
                    txtRepresentative.SelectAll();

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
                var rowData = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    this.currentData = rowData;
                    btnExportXML.Enabled = true;
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

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    short isActive = short.Parse((gridviewFormList.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    int? isUseBranchTime = ToNullableInt((gridviewFormList.GetRowCellValue(e.RowHandle, "IS_USE_BRANCH_TIME") ?? "").ToString());
                    if (e.Column.FieldName == "LOCK_BRANCH")
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
                    else if (e.Column.FieldName == "ButtonEditDeleteEnable")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = ButtonEditDeleteEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditDeleteDisable;
                        }
                    }
                    else if (e.Column.FieldName == "BRANCH_TIME_SETUP")
                    {
                        if (isUseBranchTime != null && isUseBranchTime == 1)
                        {
                            e.RepositoryItem = ButtonEdit__SetupBranchTimeEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEdit_SetupBranchTimeDisable;
                        }
                    }
                    else if (e.Column.FieldName == "UseBranchTime")
                    {
                        if (isUseBranchTime == 1)
                        {
                            e.RepositoryItem = ButtonEdit_BranchTime_View;
                        }
                        else
                        {
                            e.RepositoryItem = null;
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
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var updateDTO = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
                    var result = new BackendAdapter(param).Post<HIS_BRANCH>(HisRequestUriStore.MOSHIS_BRANCH_CHANGE_LOCK, ApiConsumers.MosConsumer, updateDTO.ID, param);
                    if (result != null)
                    {
                        BackendDataWorker.Reset<HIS_BRANCH>();
                        success = true;
                        FillDataToGridControl();
                        btnCancel_Click(null, null);
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
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    bool success = false;
                    CommonParam param = new CommonParam();
                    var updateDTO = (MOS.EFMODEL.DataModels.HIS_BRANCH)gridviewFormList.GetFocusedRow();
                    var result = new BackendAdapter(param).Post<HIS_BRANCH>(HisRequestUriStore.MOSHIS_BRANCH_CHANGE_LOCK, ApiConsumers.MosConsumer, updateDTO.ID, param);
                    if (result != null)
                    {
                        BackendDataWorker.Reset<HIS_BRANCH>();
                        success = true;
                        FillDataToGridControl();
                        btnCancel_Click(null, null);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUseBranchTime_KeyDown(object sender, KeyEventArgs e)
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

        private void ButtonEdit__SetupBranchTimeEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.currentData = (HIS_BRANCH)gridviewFormList.GetFocusedRow();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisBranchTime").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisBranchTime'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    //FillDataToControlBySelectTreatment(true);
                    //txtFindTreatmentCode.Focus();
                    //txtFindTreatmentCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlFormList_Click(object sender, EventArgs e)
        {

        }

        private void btnUploadLogo_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|Bitmaps|*.bmp|PNG files|*.png|JPEG files|*.jpg|GIF files|*.gif|TIFF files|*.tif|All files|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(openFileDialog1.FileName))
                    {
                        byteImgs = converImgToByte(openFileDialog1.FileName);
                        MemoryStream ms = new MemoryStream(byteImgs, 0, byteImgs.Length);
                        Image image = Image.FromStream(ms);
                        pBLogo.Image = image;
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải logo thành công", "Thông báo");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private byte[] converImgToByte(string url)
        {
            byte[] picbyte = null;
            try
            {
                FileStream fs;
                fs = new FileStream(url, FileMode.Open, FileAccess.Read);
                picbyte = new byte[fs.Length];
                fs.Read(picbyte, 0, System.Convert.ToInt32(fs.Length));
                fs.Close();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return picbyte;
        }


        void FillDataToLookupedit(LookUpEdit cboEditor, string displayMember, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayMember, "", 100));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 20;
                cboEditor.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void FillDataToLookupedit1(LookUpEdit cboEditor, string valueMember, string displayCodeMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayCodeMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 10;
                cboEditor.Properties.PopupWidth = 50;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        void FillDataToLookupedit(DevExpress.XtraEditors.LookUpEdit cboEditor, string displayMember, string valueMember, object datasource)
        {
            try
            {
                cboEditor.Properties.DataSource = datasource;
                cboEditor.Properties.DisplayMember = displayMember;
                cboEditor.Properties.ValueMember = valueMember;
                cboEditor.Properties.ForceInitialize();
                cboEditor.Properties.Columns.Clear();
                //cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayCodeMember, "", 50));
                cboEditor.Properties.Columns.Add(new LookUpColumnInfo(displayMember, "", 100));
                cboEditor.Properties.ShowHeader = false;
                cboEditor.Properties.ImmediatePopup = true;
                cboEditor.Properties.DropDownRows = 20;
                cboEditor.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FocusShowPopup(DateEdit cboEditor)
        {
            cboEditor.Focus();
            cboEditor.ShowPopup();
        }

        private void FocusShowPopup(LookUpEdit cboEditor)
        {
            cboEditor.Focus();
            cboEditor.ShowPopup();
        }
        private void FocusMoveText(TextEdit txt)
        {
            try
            {
                txt.Focus();
                txt.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRepresentative_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPosition.Focus();
                    txtPosition.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtPosition_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAuthLetterIssueDate.Focus();
                    txtAuthLetterIssueDate.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtAuthLetterIssueDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAuthLetterNum.Focus();
                    txtAuthLetterNum.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtAuthLetterNum_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBankInfo.Focus();
                    txtBankInfo.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtBankInfo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTheBrankCode.Focus();
                    txtTheBrankCode.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

        private void txtTheBrankCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkUseBranchTime.Focus();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtSYS_MEDI_ORG_CODE_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProvince.Focus();
                    txtProvince.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboManager_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboManager_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboManager_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboManager.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnExportXML_Click(null, null);
        }

        private void btnExportXML_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentData == null) return;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                string folderPath = null;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    folderPath = fbd.SelectedPath;
                }
                if (folderPath == null) return;
                XML xml = new XML();
                xml.STT = gridviewFormList.FocusedRowHandle + 1;
                xml.MA_CSKCB = currentData.HEIN_MEDI_ORG_CODE ?? "";
                xml.TEN_CSKCB = this.ConvertStringToXmlDocument(currentData.BRANCH_NAME ?? "");
                xml.LOAI_HINH = currentData.TYPE != null ? this.ConvertStringToXmlDocument(cboType.Properties.Items[(currentData.TYPE ?? 0) - 1].ToString()) : this.ConvertStringToXmlDocument("");
                xml.PHAN_TUYEN = currentData.HEIN_LEVEL_CODE;
                xml.HINH_THUC_TC = currentData.FORM != null ? this.ConvertStringToXmlDocument(cboForm.Properties.Items[(currentData.FORM ?? 0) - 1].ToString()) : this.ConvertStringToXmlDocument("");
                xml.DANH_MUC_KHOA = String.Join(";", BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && !string.IsNullOrEmpty(o.BHYT_CODE) && o.BRANCH_ID == currentData.ID).Select(o => o.BHYT_CODE).Distinct().ToList());
                xml.GIUONG_PD = currentData.BED_APPROVED != null ? currentData.BED_APPROVED.ToString() : "";
                xml.GIUONG_TK = currentData.BED_ACTUAL != null ? currentData.BED_ACTUAL.ToString() : "";
                xml.GIUONG_HSTC = currentData.BED_RESUSCITATION != null ? currentData.BED_RESUSCITATION.ToString() : "";
                xml.GIUONG_HSCC = currentData.BED_RESUSCITATION_EMG != null ? currentData.BED_RESUSCITATION_EMG.ToString() : "";
                xml.LDLK = currentData.VENTURE != null ? this.ConvertStringToXmlDocument(cboVenture.Properties.Items[(currentData.VENTURE ?? 0) - 1].ToString()) : this.ConvertStringToXmlDocument("");
                xml.MA_TINH = currentData.PROVINCE_CODE ?? "";
                xml.MA_HUYEN = currentData.DISTRICT_CODE ?? "";
                xml.MA_XA = currentData.COMMUNE_CODE ?? "";
                xml.DIEN_THOAI = currentData.PHONE ?? "";

                var fileName = string.Format("XML_{0}___{1}", new string[] { DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss"), xml.MA_CSKCB });
                var path = string.Format("{0}/{1}.xml", folderPath, fileName);
                bool Sucess = CreatedXmlFile(xml, displayNamspacess: false, saveFile: true, path);
                if (Sucess)
                {
                    XtraMessageBox.Show("Lưu file xml thành công", "Thông báo");
                    if (XtraMessageBox.Show("Bạn có muốn mở file?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        System.Diagnostics.Process.Start(path);
                }
                else
                {
                    XtraMessageBox.Show("Lưu file xml thất bại", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool CreatedXmlFile<T>(T input, bool displayNamspacess, bool saveFile, string path)
        {
            bool rs = false;
            string xmlFile = null;
            try
            {
                var enc = Encoding.UTF8;
                using (var ms = new MemoryStream())
                {
                    var xmlNamespaces = new XmlSerializerNamespaces();
                    if (displayNamspacess)
                    {
                        xmlNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                        xmlNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    }
                    else
                        xmlNamespaces.Add("", "");

                    var xmlWriterSettings = new XmlWriterSettings
                    {
                        CloseOutput = false,
                        Encoding = enc,
                        OmitXmlDeclaration = false,
                        Indent = true
                    };
                    using (var xw = XmlWriter.Create(ms, xmlWriterSettings))
                    {
                        var s = new XmlSerializer(typeof(T));
                        s.Serialize(xw, input, xmlNamespaces);
                    }
                    xmlFile = enc.GetString(ms.ToArray());
                }

                if (saveFile)
                {
                    using (var file = new StreamWriter(path))
                    {
                        file.Write(xmlFile);
                    }
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                rs = false;
            }
            return rs;
        }
        private XmlCDataSection ConvertStringToXmlDocument(string data)
        {
            XmlCDataSection result;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<book genre='novel' ISBN='1-861001-57-5'>" + "<title>Pride And Prejudice</title>" + "</book>");
            result = doc.CreateCDataSection(RemoveXmlCharError(data));
            return result;
        }
        private string RemoveXmlCharError(string data)
        {
            string result = "";
            try
            {
                StringBuilder s = new StringBuilder();
                if (!String.IsNullOrWhiteSpace(data))
                {
                    foreach (char c in data)
                    {
                        if (!System.Xml.XmlConvert.IsXmlChar(c)) continue;
                        s.Append(c);
                    }
                }

                result = s.ToString();
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void cboVenture_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboVenture.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboType.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboForm.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtHeinLevelCode_TextChanged(object sender, EventArgs e)     
        {
            if (txtHeinLevelCode.Text == "1" || txtHeinLevelCode.Text == "2")
            {
                chkPay.Enabled = true;
            }
            else
            {
                chkPay.Enabled = false;
                chkPay.Checked = false;
            }
        }
    }
}
