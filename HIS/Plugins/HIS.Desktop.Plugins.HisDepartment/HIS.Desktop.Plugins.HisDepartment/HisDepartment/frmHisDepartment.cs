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
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Plugins.HisDepartment.Entity;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Utility;
using HIS.Desktop.Utilities.Extensions;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.HisDepartment.Validation;
using DevExpress.XtraEditors.Repository;
using HIS.UC.SettingSignInfo;
using HIS.Desktop.ADO;
using Newtonsoft.Json;
using HIS.Desktop.Plugins.HisDepartment.ADO;
using EMR.WCF.DCO;
using EMR.SDO;
using Inventec.Common.SignLibrary.ServiceSign;
using System.Threading;
using System.Diagnostics;

namespace HIS.Desktop.Plugins.HisDepartment.HisDepartment
{
    public partial class frmHisDepartment : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        //qtcode
        private bool isHeaderChecked = false;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_DEPARTMENT currentData;
        List<HIS_DEPARTMENT> listKhoa = new List<HIS_DEPARTMENT>();
        List<ACS_USER> listHead = new List<ACS_USER>();
        internal List<ADO.SubsDirectorADO> listSubsDirectors { get; set; } // QTCODE
        internal List<ADO.HeadADO> listHeads { get; set; }
        List<string> arrControlEnableNotChange = new List<string>();

        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        long departmentId;
        // qtcode
        bool isNotLoadWhileChangeControlStateInFirst;
        SettingSignADO SettingSignADO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        #endregion

        #region Construct
        public frmHisDepartment(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisDepartment_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisDepartment.Resources.Lang", typeof(HIS.Desktop.Plugins.HisDepartment.HisDepartment.frmHisDepartment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDepartmentCode.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColDepartmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDepartmentCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColDepartmentCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDepartmentName.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDepartmentName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColDepartmentName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColGCode.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColGCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColGCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColGCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBhytCode.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColBhytCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBhytCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColBhytCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsAutoReceivePatient.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColIsAutoReceivePatient.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsAutoReceivePatient.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColIsAutoReceivePatient.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOrder.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColNumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNumOrder.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColNumOrder.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsClinical.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColIsClinical.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsClinical.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColIsClinical.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamDepartment.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColExamDepartment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamDepartment.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColExamDepartment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTheoryPatientCount.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColTheoryPatientCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTheoryPatientCount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColTheoryPatientCount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRealityPatientCount.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColRealityPatientCount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRealityPatientCount.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColRealityPatientCount.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHead.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColHead.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHead.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColHead.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColSubsDirector.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColSubsDirector.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSubsDirector.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColSubsDirector.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem6.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem8.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisDepartment.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkBranchId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDepartment.lkBranchId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAutoReceivePatient.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.chkIsAutoReceivePatient.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsClinical.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.chkIsClinical.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtGCode.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisDepartment.txtGCode.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartmentCode.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciDepartmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartmentName.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciDepartmentName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGCode.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciGCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBhytCode.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciBhytCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBranchId.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciBranchId.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsAutoReceivePatient.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciIsAutoReceivePatient.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsClinical.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciIsClinical.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTheoryPatientCount.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.lciTheoryPatientCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisDepartment.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisDepartment.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue() // set các giá trị mặc định
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtKeyword.Text = "";
                cboTreatmentType.EditValue = null;
                cboIcdCode.EditValue = null;
                spNumOrder.EditValue = null;
                spTheoryPatientCount.EditValue = null;
                spRealityPatientCount.EditValue = null;
                spExamDeskCount.EditValue = null;
                spIcuBedCount.EditValue = null;
                spErResusBedCount.EditValue = null;
                chkWarningWhenIsNoSurg.Checked = false;
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


        //private void InitTabIndex()
        //{
        //    try
        //    {
        //        dicOrderTabIndexControl.Add("txtDepartmentCode", 0);
        //        dicOrderTabIndexControl.Add("txtDepartmentName", 1);
        //        dicOrderTabIndexControl.Add("txtGCode", 3);
        //        dicOrderTabIndexControl.Add("txtBhytCode", 4);
        //        dicOrderTabIndexControl.Add("lkBranchId", 5);
        //        dicOrderTabIndexControl.Add("cboDefaultInstrPatientType", 6);
        //        dicOrderTabIndexControl.Add("chkIsAutoReceivePatient", 7);
        //        dicOrderTabIndexControl.Add("spNumOrder", 8);
        //        dicOrderTabIndexControl.Add("chkIsClinical", 9);
        //        dicOrderTabIndexControl.Add("spTheoryPatientCount", 10);
        //        dicOrderTabIndexControl.Add("spRealityPatientCount", 11);
        //        dicOrderTabIndexControl.Add("cboReqSurgTreatmentType", 12);
        //        dicOrderTabIndexControl.Add("TxtPhone", 13);
        //        dicOrderTabIndexControl.Add("chkAllowAssignSurgeryPrice", 14);
        //        dicOrderTabIndexControl.Add("chkAllowAssignPackagePrice", 15);

        //        if (dicOrderTabIndexControl != null)
        //        {
        //            foreach (KeyValuePair<string, int> itemOrderTab in dicOrderTabIndexControl)
        //            {
        //                SetTabIndexToControl(itemOrderTab, lcEditorInfo);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

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
                InitComboBranchId();
                InitComboDefaultInstrPatientType();
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboBranchId()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BRANCH_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BRANCH_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BRANCH_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkBranchId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BRANCH>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDefaultInstrPatientType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDefaultInstrPatientType, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        private void InitCombo_(GridLookUpEdit cbo, object data, string DisplayValue1, string DisplayValue2, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue2;
                cbo.Properties.ValueMember = ValueMember;
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue1);
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";

                DevExpress.XtraGrid.Columns.GridColumn col3 = cbo.Properties.View.Columns.AddField(DisplayValue2);
                col3.VisibleIndex = 2;
                col3.Width = 300;
                col3.Caption = "";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = false;
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

        private void InitComboReqSurgTreatmentType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_TYPE_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboReqSurgTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboIcdCode()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ICD_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 400);
                ControlEditorLoader.Load(this.cboIcdCode, BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);

                DevExpress.XtraGrid.Columns.GridColumn col2 = cboIcdCode.Properties.View.Columns.AddField("ICD_NAME");
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cboIcdCode.Properties.PopupFormWidth = 200;
                cboIcdCode.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboIcdCode.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cboIcdCode.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboIcdCode.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboHead()
        {
            try
            {
                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                listHeads = new List<ADO.HeadADO>();

                foreach (var item in acsUser)
                {
                    listHeads.Add(new ADO.HeadADO(item));
                }
                Base.GlobalStore.LoadDataGridLookUpEdit(cboHeadUserName, "LOGINNAME", "USERNAME", "LOGINNAME", listHeads);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                ucPaging.Init(LoadPaging, param, numPageSize, this.gridControlFormList);
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>> apiResult = null;
                HisDepartmentFilter filter = new HisDepartmentFilter();
                SetFilterNavBar(ref filter);
                //dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>>(HisRequestUriStore.MOSHIS_DEPARTMENT_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>)apiResult.Data;
                    var listADO = data.Select(o => new HisDepartmentADO(o))
                     .ToList();
                    //dnNavigation.DataSource = data;
                    gridviewFormList.GridControl.DataSource = listADO;
                    rowCount = (listADO == null ? 0 : listADO.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
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

        private void SetFilterNavBar(ref HisDepartmentFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
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

                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound) //Đảm bảo rằng cột đang xử lý là một cột unbound((không ràng buộc với dữ liệu trực tiếp từ DataSource).
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.HIS_DEPARTMENT pData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage + ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "HeadSTR")
                    {
                        try
                        {
                            if (pData.HEAD_LOGINNAME != null && pData.HEAD_USERNAME != null)
                            {
                                e.Value = pData.HEAD_LOGINNAME + " - " + pData.HEAD_USERNAME;
                            }
                            else if (pData.HEAD_LOGINNAME != null && pData.HEAD_USERNAME == null)
                            {
                                e.Value = pData.HEAD_LOGINNAME;
                            }
                            else if (pData.HEAD_LOGINNAME == null && pData.HEAD_USERNAME != null)
                            {
                                e.Value = pData.HEAD_USERNAME;
                            }
                            else e.Value = null;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Lỗi xét giá trị cho cột trưởng phòng", ex);
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
                    else if (e.Column.FieldName == "IS_CLINICAL_STR")
                    {
                        try
                        {
                            ////string IS_CLINICAL = (view.GetRowCellValue(e.ListSourceRowIndex, "IS_CLINICAL") ?? "").ToString();
                            //e.Value = ;
                            e.Value = pData != null && pData.IS_CLINICAL == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la khoa lam sang IS_CLINICAL_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_EXAM_STR")
                    {
                        try
                        {
                            ////string IS_CLINICAL = (view.GetRowCellValue(e.ListSourceRowIndex, "IS_CLINICAL") ?? "").ToString();
                            //e.Value = ;
                            e.Value = pData != null && pData.IS_EXAM == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la khoa lam sang IS_CLINICAL_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_EMEGENCY_OBJ")
                    {
                        try
                        {
                            ////string IS_CLINICAL = (view.GetRowCellValue(e.ListSourceRowIndex, "IS_CLINICAL") ?? "").ToString();
                            //e.Value = ;
                            e.Value = pData != null && pData.IS_EMERGENCY == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot la khoa lam sang IS_CLINICAL_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_AUTO_RECEIVE_PATIENT_STR")
                    {
                        try
                        {
                            //string IS_CLINICAL = (view.GetRowCellValue(e.ListSourceRowIndex, "IS_AUTO_RECEIVE_PATIENT") ?? "").ToString();
                            e.Value = pData != null && pData.IS_AUTO_RECEIVE_PATIENT == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot tu dong nhan benh nhan vao khoa IS_AUTO_RECEIVE_PATIENT_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ALLOW_ASSIGN_SURGERY_PRICE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.ALLOW_ASSIGN_SURGERY_PRICE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot cho phep chi dinh gia phau thuat ALLOW_ASSIGN_SURGERY_PRICE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ALLOW_ASSIGN_PACKAGE_PRICE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.ALLOW_ASSIGN_PACKAGE_PRICE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot cho phep chi dinh gia goi ALLOW_ASSIGN_PACKAGE_PRICE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "SubsDirectorSTR")
                    {
                        if (pData.HOSP_SUBS_DIRECTOR_LOGINNAME != null && pData.HOSP_SUBS_DIRECTOR_USERNAME != null)
                        {
                            e.Value = pData.HOSP_SUBS_DIRECTOR_LOGINNAME + " - " + pData.HOSP_SUBS_DIRECTOR_USERNAME;
                        }
                        else if (pData.HOSP_SUBS_DIRECTOR_LOGINNAME != null)
                        {
                            e.Value = pData.HOSP_SUBS_DIRECTOR_LOGINNAME;
                        }
                        else if (pData.HOSP_SUBS_DIRECTOR_USERNAME != null)
                        {
                            e.Value = pData.HOSP_SUBS_DIRECTOR_USERNAME;
                        }
                        else
                        {
                            e.Value = null;
                        }
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
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
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

        //private void dnNavigation_PositionChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        this.currentData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>)[dnNavigation.Position];
        //        if (this.currentData != null)
        //        {
        //            ChangedDataRow(this.currentData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_DEPARTMENT data) // ĐƯỢC GỌI TRONG CLICK 1 ROW, SERACH, KEYDOWN CỦA FORM LIST
        {
            try
            {
                if (data != null)
                {

                    FillDataToEditorControl(data); // lấy được dữ liệu của input
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    currentData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_DEPARTMENT data) // GỌI TRONG CHANGEDATAROW
        {
            try
            {
                if (data != null)
                {
                    // qtcode
                    spExamDeskCount.EditValue = data.EXAM_DESK_COUNT;
                    spIcuBedCount.EditValue = data.ICU_BED_COUNT;
                    spErResusBedCount.EditValue = data.ER_RESUS_BED_COUNT;
                    // Thời gian hiệu lực đến
                    if (data.FROM_TIME.HasValue && data.FROM_TIME.Value > 0)
                    {
                        deTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.FROM_TIME.Value);
                    }
                    else
                    {
                        deTimeFrom.EditValue = null;
                    }

                    if (data.TO_TIME.HasValue && data.TO_TIME.Value > 0)
                    {
                        deTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.TO_TIME.Value);
                    }
                    else
                    {
                        deTimeTo.EditValue = null;
                    }

                    // Đồng bộ disable
                    deTimeFrom_EditValueChanged(null, null);
                    deTimeTo_EditValueChanged(null, null);

                    // qtcode
                    departmentId = data.ID;
                    txtDepartmentCode.Text = data.DEPARTMENT_CODE;
                    txtDepartmentName.Text = data.DEPARTMENT_NAME;
                    txtGCode.EditValue = data.G_CODE;
                    txtBhytCode.Text = data.BHYT_CODE;
                    lkBranchId.EditValue = data.BRANCH_ID;
                    cboDefaultInstrPatientType.EditValue = data.DEFAULT_INSTR_PATIENT_TYPE_ID;
                    chkIsAutoReceivePatient.Checked = (data.IS_AUTO_RECEIVE_PATIENT == 1 ? true : false);
                    chkIsEmegency.Checked = (data.IS_EMERGENCY == 1 ? true : false);
                    spNumOrder.EditValue = data.NUM_ORDER;
                    chkIsClinical.Checked = (data.IS_CLINICAL == 1 ? true : false);
                    chkIsExamDepartment.Checked = (data.IS_EXAM == 1 ? true : false);
                    chkAllowAssignPackagePrice.Checked = (data.ALLOW_ASSIGN_PACKAGE_PRICE == 1 ? true : false);
                    chkAllowAssignSurgeryPrice.Checked = (data.ALLOW_ASSIGN_SURGERY_PRICE == 1 ? true : false);
                    chkIsInDepStockMoba.Checked = (data.IS_IN_DEP_STOCK_MOBA == 1 ? true : false);
                    chkWarningWhenIsNoSurg.Checked = (data.WARNING_WHEN_IS_NO_SURG == 1 ? true : false);
                    chkAUTO_BED_ASSIGN_OPTION.Checked = (data.AUTO_BED_ASSIGN_OPTION == 1 ? true : false);
                    spTheoryPatientCount.EditValue = data.THEORY_PATIENT_COUNT;
                    cboReqSurgTreatmentType.EditValue = data.REQ_SURG_TREATMENT_TYPE_ID;
                    chkIsTraditional.Checked = (data.IS_TRADITIONAL == 1 ? true : false);

                    GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                    }
                    if (!String.IsNullOrWhiteSpace(data.ALLOW_TREATMENT_TYPE_IDS) && gridCheckMark != null)
                    {
                        ProcessSelectTreatmentType(data, gridCheckMark);
                    }

                    GridCheckMarksSelection gridCheckMark_ = cboIcdCode.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark_ != null)
                    {
                        gridCheckMark_.ClearSelection(cboIcdCode.Properties.View);
                    }
                    if (!String.IsNullOrWhiteSpace(data.ACCEPTED_ICD_CODES) && gridCheckMark_ != null)
                    {
                        ProcessSelectIcdCode(data, gridCheckMark_);
                    }
                    spRealityPatientCount.EditValue = data.REALITY_PATIENT_COUNT;
                    TxtPhone.Text = data.PHONE;


                    txtHeadLoginName.Text = data.HEAD_LOGINNAME;
                    cboHeadUserName.EditValue = data.HEAD_LOGINNAME;

                    // QTCODE
                    txtSubsDirectorLoginName.Text = data.HOSP_SUBS_DIRECTOR_LOGINNAME;
                    cboSubsDirectorUserName.EditValue = data.HOSP_SUBS_DIRECTOR_LOGINNAME;

                    // QTCODE

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectTreatmentType(HIS_DEPARTMENT data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_TREATMENT_TYPE> ds = cboTreatmentType.Properties.DataSource as List<HIS_TREATMENT_TYPE>;
                string[] arrays = data.ALLOW_TREATMENT_TYPE_IDS.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_TREATMENT_TYPE> selects = new List<HIS_TREATMENT_TYPE>();
                    List<long> ids = new List<long>();
                    foreach (var item in arrays)
                    {
                        long id = Convert.ToInt64(item ?? "0");
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID == id) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectIcdCode(HIS_DEPARTMENT data, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_ICD> ds = cboIcdCode.Properties.DataSource as List<HIS_ICD> ?? new List<HIS_ICD>();
                string[] arrays = data.ACCEPTED_ICD_CODES.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_ICD> selects = new List<HIS_ICD>();
                    List<string> ids = new List<string>();
                    foreach (var item in arrays)
                    {
                        string id = item;
                        var row = ds != null ? ds.FirstOrDefault(o => o.ICD_CODE == id) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    List<HIS_ICD> notSelects = ds.Where(o => !selects.Contains(o)).ToList() ?? new List<HIS_ICD>();
                    List<HIS_ICD> newDatasource = new List<HIS_ICD>();
                    newDatasource.AddRange(selects);
                    newDatasource.AddRange(notSelects);
                    cboIcdCode.Properties.DataSource = newDatasource;
                    gridCheckMark.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                        }
                        if (lci != null && lci.Control != null && lci.Control is GridLookUpEdit)
                        {
                            DevExpress.XtraEditors.GridLookUpEdit cbo = lci.Control as DevExpress.XtraEditors.GridLookUpEdit;
                            if (cbo.Properties.View.OptionsSelection.MultiSelect == true)
                            {
                                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                                if (gridCheckMark != null)
                                {
                                    gridCheckMark.ClearSelection(cbo.Properties.View);
                                }
                            }
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_DEPARTMENT currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>>(HisRequestUriStore.MOSHIS_DEPARTMENT_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                txtDepartmentCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
                lkBranchId.ReadOnly = !(action == GlobalVariables.ActionAdd);
                //cboDefaultInstrPatientType.ReadOnly = !(action == GlobalVariables.ActionAdd);
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_DEPARTMENT_DELETE, ApiConsumers.MosConsumer, rowData, param);
                    if (success)
                    {
                        BackendDataWorker.Reset<HIS_DEPARTMENT>();
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
                cboTreatmentType.ResetText();
                cboIcdCode.ResetText();
                ResetFormData();
                SetFocusEditor();
                txtDepartmentCode.Focus();
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
                MOS.EFMODEL.DataModels.HIS_DEPARTMENT updateDTO = new MOS.EFMODEL.DataModels.HIS_DEPARTMENT();

                //if (this.currentData != null && this.currentData.ID > 0)
                //{
                //    LoadCurrent(this.currentData.ID, ref updateDTO);
                //}
                UpdateDTOFromDataForm(ref updateDTO);
                // sau hàm này sẽ lấy được dữ liệu cho updateDTO
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(HisRequestUriStore.MOSHIS_DEPARTMENT_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<HIS_DEPARTMENT>();
                        success = true;
                        FillDataToGridControl();
                        ResetFormData();
                    }
                }
                else
                {
                    if (departmentId > 0)
                    {
                        updateDTO.ID = departmentId;
                        var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(HisRequestUriStore.MOSHIS_DEPARTMENT_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
                        if (resultData != null)
                        {
                            BackendDataWorker.Reset<HIS_DEPARTMENT>();
                            success = true;
                            FillDataToGridControl();
                            //ResetFormData();
                            //UpdateRowDataAfterEdit(resultData);
                        }
                    }
                }

                //if (success)
                //{
                //    SetFocusEditor();
                //}

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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_DEPARTMENT data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_DEPARTMENT) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_DEPARTMENT currentDTO)
        {
            try
            {
                currentDTO.DEPARTMENT_CODE = txtDepartmentCode.Text.Trim();
                currentDTO.DEPARTMENT_NAME = txtDepartmentName.Text.Trim();
                currentDTO.G_CODE = txtGCode.EditValue.ToString();
                currentDTO.BHYT_CODE = txtBhytCode.Text.Trim();
                if (lkBranchId.EditValue != null) currentDTO.BRANCH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkBranchId.EditValue ?? "0").ToString());
                if (cboDefaultInstrPatientType.EditValue != null)
                {
                    currentDTO.DEFAULT_INSTR_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDefaultInstrPatientType.EditValue ?? "0").ToString());
                }
                currentDTO.IS_AUTO_RECEIVE_PATIENT = (short)(chkIsAutoReceivePatient.Checked ? 1 : 0);
                currentDTO.IS_EMERGENCY = (short)(chkIsEmegency.Checked ? 1 : 0);
                if (spNumOrder.EditValue != null)
                {
                    if (spNumOrder.Value == 0)
                    {
                        spNumOrder.EditValue = null;
                    }
                    else
                    {
                        currentDTO.NUM_ORDER = (long)spNumOrder.Value;
                    }

                }

                currentDTO.IS_EXAM = (short)(chkIsExamDepartment.Checked ? 1 : 0);
                currentDTO.IS_CLINICAL = (short)(chkIsClinical.Checked ? 1 : 0);
                currentDTO.ALLOW_ASSIGN_SURGERY_PRICE = (short)(chkAllowAssignSurgeryPrice.Checked ? 1 : 0);
                currentDTO.ALLOW_ASSIGN_PACKAGE_PRICE = (short)(chkAllowAssignPackagePrice.Checked ? 1 : 0);
                if (chkIsInDepStockMoba.Checked)
                {
                    currentDTO.IS_IN_DEP_STOCK_MOBA = 1;
                }
                else
                {
                    currentDTO.IS_IN_DEP_STOCK_MOBA = null;
                }

                if (chkWarningWhenIsNoSurg.Checked)
                {
                    currentDTO.WARNING_WHEN_IS_NO_SURG = 1;
                }
                else
                {
                    currentDTO.WARNING_WHEN_IS_NO_SURG = null;
                }
                if (chkAUTO_BED_ASSIGN_OPTION.Checked)
                {
                    currentDTO.AUTO_BED_ASSIGN_OPTION = 1;
                }
                else
                {
                    currentDTO.AUTO_BED_ASSIGN_OPTION = null;
                }
                if (chkIsTraditional.Checked)
                {
                    currentDTO.IS_TRADITIONAL = 1;
                }
                else
                {
                    currentDTO.IS_TRADITIONAL = null;
                }


                if (spTheoryPatientCount.EditValue != null)
                {
                    if (spTheoryPatientCount.Value == 0)
                    {
                        spTheoryPatientCount.EditValue = null;
                    }
                    else
                    {
                        currentDTO.THEORY_PATIENT_COUNT = (long)spTheoryPatientCount.Value;
                    }

                }
                if (spRealityPatientCount.EditValue != null)
                {
                    if (spRealityPatientCount.Value == 0)
                    {
                        spRealityPatientCount.EditValue = null;
                    }
                    else
                    {
                        currentDTO.REALITY_PATIENT_COUNT = Convert.ToInt64(spRealityPatientCount.Value);
                    }

                }
                currentDTO.ALLOW_TREATMENT_TYPE_IDS = null;
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    List<long> ids = new List<long>();
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv != null && !ids.Contains(rv.ID))
                            ids.Add(rv.ID);
                    }
                    currentDTO.ALLOW_TREATMENT_TYPE_IDS = String.Join(",", ids);
                }
                cboTreatmentType.ResetText();
                if (cboReqSurgTreatmentType.EditValue != null)
                    currentDTO.REQ_SURG_TREATMENT_TYPE_ID = Convert.ToInt64(cboReqSurgTreatmentType.EditValue);
                else
                    currentDTO.REQ_SURG_TREATMENT_TYPE_ID = null;

                currentDTO.PHONE = TxtPhone.Text;

                currentDTO.HEAD_LOGINNAME = txtHeadLoginName.Text;
                currentDTO.HEAD_USERNAME = cboHeadUserName.Text;

                // QTCODE
                currentDTO.HOSP_SUBS_DIRECTOR_LOGINNAME = txtSubsDirectorLoginName.Text;
                currentDTO.HOSP_SUBS_DIRECTOR_USERNAME = cboSubsDirectorUserName.Text;

                // END QTCODE
                if (spExamDeskCount.EditValue != null && spExamDeskCount.Value >= 0)
                {
                    currentDTO.EXAM_DESK_COUNT = (long)spExamDeskCount.Value;
                }
                else
                {
                    currentDTO.EXAM_DESK_COUNT = null;
                }

                // Số giường HSTC
                if (spIcuBedCount.EditValue != null && spIcuBedCount.Value >= 0)
                {
                    currentDTO.ICU_BED_COUNT = (long)spIcuBedCount.Value;
                }
                else
                {
                    currentDTO.ICU_BED_COUNT = null;
                }

                // Số giường HSCC
                if (spErResusBedCount.EditValue != null && spErResusBedCount.Value >= 0)
                {
                    currentDTO.ER_RESUS_BED_COUNT = (long)spErResusBedCount.Value;
                }
                else
                {
                    currentDTO.ER_RESUS_BED_COUNT = null;
                }

                // Thời gian hiệu lực đến
                currentDTO.FROM_TIME = null;
                currentDTO.TO_TIME = null;

                if (deTimeFrom.EditValue != null && deTimeFrom.DateTime != DateTime.MinValue)
                {
                    currentDTO.FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(deTimeFrom.DateTime);
                }
                else if (deTimeTo.EditValue != null && deTimeTo.DateTime != DateTime.MinValue)
                {
                    currentDTO.TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(deTimeTo.DateTime);
                }

                GridCheckMarksSelection gridCheckMark_ = cboIcdCode.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark_ != null && gridCheckMark_.SelectedCount > 0)
                {
                    List<string> ids = new List<string>();
                    foreach (HIS_ICD rv in gridCheckMark_.Selection)
                    {
                        if (rv != null && !ids.Contains(rv.ICD_CODE))
                            ids.Add(rv.ICD_CODE);
                    }
                    currentDTO.ACCEPTED_ICD_CODES = String.Join(",", ids);
                }
                cboIcdCode.ResetText();

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
                validatetxtDepartmentCode();
                ValidationSingleControl(txtDepartmentName);
                ValidationSingleControl(txtGCode);
                ValidationSingleControl(lkBranchId);
                ValidComboTreatmentType();

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

        private void validatetxtDepartmentCode()
        {
            try
            {
                ValidMaxlengthtxtDepartmentCode validRule = new ValidMaxlengthtxtDepartmentCode();
                validRule.txtDepartmentCode = this.txtDepartmentCode;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(txtDepartmentCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }


        private void ValidComboTreatmentType()
        {
            try
            {
                TreatmentTypeValidationRule rule = new TreatmentTypeValidationRule();
                rule.chkIsClinical = chkIsClinical;
                rule.cboTreatmentType = cboTreatmentType;
                dxValidationProviderEditorInfo.SetValidationRule(cboTreatmentType, rule);
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
                //Gan gia tri mac dinh
                SetDefaultValue();

                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load combo Don Vi
                LoadComboDonVi();

                //Load combo Truong phong
                LoadComboHead();

                // Load combo Người ký thay
                LoadComboSubsDirector();

                //Init combo TreatmentType
                InitCheck(cboTreatmentType, SelectionGrid__TreatmentType);
                InitCombo(cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), "TREATMENT_TYPE_NAME", "ID");

                InitCheck(cboIcdCode, SelectionGrid__IcdCode);
                InitCombo_(cboIcdCode, BackendDataWorker.Get<HIS_ICD>().Where(o => o.IS_ACTIVE == 1).Distinct().ToList(), "ICD_CODE", "ICD_NAME", "ICD_CODE");
                // InitComboIcdCode();
                //Init combo ReqSurgTreatmentType
                InitComboReqSurgTreatmentType();

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

                InitControlStateSign();
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

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_DEPARTMENT hisDepertments = new HIS_DEPARTMENT();
            bool notHandler = false;
            try
            {
                HIS_DEPARTMENT dataDepartment = (HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_DEPARTMENT data1 = new HIS_DEPARTMENT();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<HIS_DEPARTMENT>("api/HisDepartment/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<HIS_DEPARTMENT>();
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            HIS_DEPARTMENT hisDepertments = new HIS_DEPARTMENT();
            bool notHandler = false;
            try
            {
                HIS_DEPARTMENT dataDepartment = (HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    HIS_DEPARTMENT data1 = new HIS_DEPARTMENT();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<HIS_DEPARTMENT>("api/HisDepartment/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<HIS_DEPARTMENT>();
                MessageManager.Show(this.ParentForm, param, notHandler);
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

                    HIS_DEPARTMENT data = (HIS_DEPARTMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                        {
                            e.RepositoryItem = btnGEdit;
                        }
                        else
                            e.RepositoryItem = btnGEdit_Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboDonVi()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("G_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GROUP_NAME", "G_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(txtGCode, BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_GROUP>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtGCode.EditValue == null)
                    {
                        txtGCode.Focus();
                        txtGCode.ShowPopup();
                    }
                    else
                    {
                        txtBhytCode.Focus();
                        txtBhytCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGCode_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (txtGCode.EditValue != null && txtGCode.EditValue != txtGCode.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.SDA_GROUP gt = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_GROUP>().SingleOrDefault(o => o.G_CODE == txtGCode.EditValue.ToString());
                        if (gt != null)
                        {
                            txtBhytCode.Focus();
                            txtBhytCode.SelectAll();
                        }
                    }
                    else
                    {
                        txtGCode.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkBranchId_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lkBranchId.EditValue == null)
                    {
                        lkBranchId.Focus();
                        lkBranchId.ShowPopup();
                    }
                    else
                    {
                        cboDefaultInstrPatientType.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultInstrPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDefaultInstrPatientType.EditValue == null)
                    {
                        cboDefaultInstrPatientType.Focus();
                        cboDefaultInstrPatientType.ShowPopup();
                    }
                    else
                    {
                        spNumOrder.Focus();
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkBranchId_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (lkBranchId.EditValue != null && lkBranchId.EditValue != lkBranchId.OldEditValue)
                    {
                        //MOS.EFMODEL.DataModels.HIS_ROOM_TYPE gt = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ROOM_TYPE>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(lkBranchId.EditValue.ToString()));
                        //if (gt != null)
                        //{
                        //    chkIsAutoReceivePatient.Properties.FullFocusRect = true;
                        //    chkIsAutoReceivePatient.Focus();
                        //}
                    }
                    else
                    {
                        lkBranchId.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtDepartmentName.Focus();
                txtDepartmentName.SelectAll();
            }
        }

        private void txtDepartmentName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtGCode.Focus();
                txtGCode.SelectAll();
            }
        }

        private void txtBhytCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lkBranchId.Focus();
                lkBranchId.SelectAll();
            }
        }

        private void chkIsAutoReceivePatient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkAllowAssignSurgeryPrice.Focus();
            }
        }

        private void spNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkIsClinical.Properties.FullFocusRect = true;
                cboTreatmentType.Focus();
            }
        }



        private void chkIsClinical_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                chkAllowAssignPackagePrice.Focus();
            }
        }

        private void spTheoryPatientCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spRealityPatientCount.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    chkIsAutoReceivePatient.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        //private void cboChuyenKhoa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {

        //            chkIsAutoReceivePatient.Properties.FullFocusRect = true;
        //            chkIsAutoReceivePatient.Focus();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ImportDepartment").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ImportDepartment");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    listArgs.Add((RefeshReference)ReLoadDataDepartment);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadDataDepartment()
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

        private void cboDefaultInstrPatientType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDefaultInstrPatientType.EditValue = null;
                    cboDefaultInstrPatientType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultInstrPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDefaultInstrPatientType.EditValue ?? 0).ToString()));
                if (data != null)
                {
                    cboDefaultInstrPatientType.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                string typeName = "";
                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.TREATMENT_TYPE_NAME + ",";
                    }
                }
                e.DisplayText = typeName;
                cboTreatmentType.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__TreatmentType(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_TREATMENT_TYPE rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.TREATMENT_TYPE_NAME + ",";
                    }
                }
                cboTreatmentType.Text = typeName;
                cboTreatmentType.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__IcdCode(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                var gridCheckMark = (sender as GridCheckMarksSelection);
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_ICD rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.ICD_NAME + ",";
                    }
                }
                cboIcdCode.Text = typeName;
                cboIcdCode.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboReqSurgTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    TxtPhone.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqSurgTreatmentType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboReqSurgTreatmentType.EditValue != null)
                {
                    cboReqSurgTreatmentType.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboReqSurgTreatmentType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqSurgTreatmentType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode != Keys.Enter)
                {
                    cboReqSurgTreatmentType.ShowPopup();
                    SelectFirstRowPopup(cboReqSurgTreatmentType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqSurgTreatmentType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboReqSurgTreatmentType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SelectFirstRowPopup(GridLookUpEdit cbo)
        {
            try
            {
                if (cbo != null && cbo.IsPopupOpen)
                {
                    DevExpress.Utils.Win.IPopupControl popupEdit = cbo as DevExpress.Utils.Win.IPopupControl;
                    DevExpress.XtraEditors.Popup.PopupLookUpEditForm popupWindow = popupEdit.PopupWindow as DevExpress.XtraEditors.Popup.PopupLookUpEditForm;
                    if (popupWindow != null)
                    {
                        popupWindow.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void cboTreatmentType_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                spTheoryPatientCount.Focus();
                spTheoryPatientCount.SelectAll();
            }
        }

        //private void TxtPhone_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            chkIsExamDepartment.Properties.FullFocusRect = true;
        //            chkIsExamDepartment.Focus();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}

        private void spRealityPatientCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboReqSurgTreatmentType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spRealityPatientCount_TextChanged(object sender, EventArgs e)
        {
            if (spRealityPatientCount.Value < 0)
                spRealityPatientCount.Value = 0;
        }

        private void cboTreatmentType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spTheoryPatientCount.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsEmegency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAutoReceivePatient.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAllowAssignPackagePrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsEmegency.Properties.FullFocusRect = true;
                    chkIsEmegency.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // CLICK 1 ROW 
        private void gridviewFormList_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_DEPARTMENT)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TxtPhone_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboHeadUserName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsExamDepartment_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkIsClinical.Focus();
            }
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (e.KeyChar == '\r')
                {
                    cboHeadUserName.Focus();
                }
                else if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAllowAssignSurgeryPrice_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAUTO_BED_ASSIGN_OPTION.Properties.FullFocusRect = true;
                    chkAUTO_BED_ASSIGN_OPTION.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHeadUserName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHeadUserName.EditValue != null)
                    {
                        var dataheads = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                        var data = dataheads.FirstOrDefault(o => o.LOGINNAME == cboHeadUserName.EditValue.ToString());
                        if (data != null)
                        {
                            txtHeadLoginName.Text = data.LOGINNAME;
                            cboHeadUserName.Properties.Buttons[1].Visible = true;
                        }
                        txtSubsDirectorLoginName.Focus();
                        txtSubsDirectorLoginName.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHeadUserName_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void cboHeadUserName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtHeadLoginName.Text = "";
                    cboHeadUserName.EditValue = null;
                    cboHeadUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHeadLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtHeadLoginName.Text.Trim()))
                    {
                        string code = txtHeadLoginName.Text.Trim().ToLower();
                        var listData = this.listHeads.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtHeadLoginName.Text = result.First().LOGINNAME;
                            cboHeadUserName.EditValue = result.First().LOGINNAME;
                            cboHeadUserName.Properties.Buttons[1].Visible = true;
                            txtSubsDirectorLoginName.Focus();
                            txtSubsDirectorLoginName.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboHeadUserName.Focus();
                        cboHeadUserName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboHeadUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboIcdCode.Focus();
                    cboIcdCode.SelectAll();
                }
                //else if (e.KeyCode == Keys.Down)
                //{
                //    cboHeadUserName.ShowPopup();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsInDepStockMoba_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAUTO_BED_ASSIGN_OPTION_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsInDepStockMoba.Properties.FullFocusRect = true;
                    chkIsInDepStockMoba.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboHeadUserName.EditValue != null)
                    {
                        var data = this.listHeads.FirstOrDefault(o => o.LOGINNAME.ToLower() == cboHeadUserName.EditValue.ToString().ToLower());
                        if (data != null)
                        {
                            txtHeadLoginName.Text = data.LOGINNAME;
                            cboHeadUserName.Properties.Buttons[1].Visible = true;
                        }
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    cboHeadUserName.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCode_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                string typeName = "";
                GridCheckMarksSelection gridCheckMark = cboIcdCode.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (HIS_ICD rv in gridCheckMark.Selection)
                    {
                        if (rv == null)
                            continue;
                        typeName += rv.ICD_NAME + ",";
                    }
                }
                e.DisplayText = typeName;
                cboIcdCode.ToolTip = typeName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void LoadComboSubsDirector()
        {
            try
            {
                //truy vấn dữ liệu trực tiếp từ cơ sở dữ liệu thông qua BackendDataWorker.Get
                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>()
                    .Where(o => o.IS_ACTIVE == 1).ToList();
                listSubsDirectors = new List<ADO.SubsDirectorADO>();
                // listSubsDirectors là biến thành viên (field) của lớp, nên sau khi ra khỏi hàm sẽ k bị mất dữ liệu, 
                //nếu khai báo listHeads trong hàm thì khi ra khỏi hàm sẽ mất dữ liệu

                // Chuyển đổi dữ liệu sang định dạng ADO
                foreach (var item in acsUser)
                {
                    listSubsDirectors.Add(new ADO.SubsDirectorADO(item));
                }
                // Gán dữ liệu cho combobox
                //Base.GlobalStore.LoadDataGridLookUpEdit(Control control, string valueMember, string displayMember, 
                //string searchMember, object dataSource);
                Base.GlobalStore.LoadDataGridLookUpEdit(cboSubsDirectorUserName, "LOGINNAME", "USERNAME", "LOGINNAME", listSubsDirectors);
                //"LOGINNAME"	Giá trị thực sự của ComboBox (valueMember). Khi chọn một mục, ComboBox sẽ trả về giá trị của cột này.
                // "USERNAME"	Dữ liệu hiển thị trong ComboBox (displayMember). Đây là cột mà người dùng nhìn thấy khi chọn giá trị.
                //"LOGINNAME"	Cột dùng để tìm kiếm khi người dùng nhập vào ô tìm kiếm trong ComboBox.
                // listSubsDirectors	Danh sách dữ liệu được gán vào ComboBox, lấy từ danh sách listSubsDirectors.
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtSubsDirectorLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtSubsDirectorLoginName.Text.Trim()))
                    {
                        string code = txtSubsDirectorLoginName.Text.Trim().ToLower(); // lấy của thằng login
                        var listData = listSubsDirectors.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList(); // lấy 1 list các SubsDirectors sao cho thằng loginname chứa thằng mới điền vào ô text 
                        var result = listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData; // nếu có giống hết thì lấy luôn thằng đó nếu k thì là 1 list 
                        if (result != null && result.Count > 0)
                        {
                            showCbo = false;
                            txtSubsDirectorLoginName.Text = result.First().LOGINNAME;
                            cboSubsDirectorUserName.EditValue = result.First().LOGINNAME;
                            cboSubsDirectorUserName.Properties.Buttons[1].Visible = true;
                            // Chuyển focus sang control tiếp theo
                            chkIsExamDepartment.Focus();
                            chkIsExamDepartment.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboSubsDirectorUserName.Focus();
                        cboSubsDirectorUserName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSubsDirectorUserName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal && cboSubsDirectorUserName.EditValue != null)
                {
                    var data = listSubsDirectors.FirstOrDefault(o => o.LOGINNAME == cboSubsDirectorUserName.EditValue.ToString());
                    if (data != null)
                    {
                        txtSubsDirectorLoginName.Text = data.LOGINNAME;
                        cboSubsDirectorUserName.Properties.Buttons[1].Visible = true;
                        // Chuyển focus sang control tiếp theo
                    }
                    chkIsExamDepartment.Focus();
                    chkIsExamDepartment.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void cboSubsDirectorUserName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtSubsDirectorLoginName.Text = "";
                    cboSubsDirectorUserName.EditValue = null;
                    cboSubsDirectorUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridviewFormList_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "IS_CHECK_DEPT")
            {
                // Xóa các element mặc định nếu cần (để vẽ sạch)
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);

                RepositoryItemCheckEdit checkEdit = e.Column.ColumnEdit as RepositoryItemCheckEdit;
                if (checkEdit != null)
                {
                    int size = 16;  // Kích thước checkbox
                    int x = e.Bounds.X + (e.Bounds.Width - size) / 2;
                    int y = e.Bounds.Y + (e.Bounds.Height - size) / 2;
                    Rectangle rect = new Rectangle(x, y, size, size);

                    var info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.CreateViewInfo();
                    var painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)checkEdit.CreatePainter();

                    // Trạng thái checkbox header dựa trên biến
                    info.EditValue = isHeaderChecked;

                    info.Bounds = rect;
                    info.CalcViewInfo(e.Graphics);

                    using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                    {
                        painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, rect));
                    }
                }

                e.Handled = true;
            }
        }

        private void chkSignXml_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("isNotLoadWhileChangeControlStateInFirst: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", isNotLoadWhileChangeControlStateInFirst));
                Inventec.Common.Logging.LogSystem.Debug("SettingSignADO: " + Inventec.Common.Logging.LogUtil.TraceData("DataA", SettingSignADO));


                if (isNotLoadWhileChangeControlStateInFirst) return;

                //if (chkSignXml.Checked && (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber)))
                if (chkSignXml.Checked)
                {
                    // Mở form thiết lập ký (giống popup XML130)
                    frmSetting frm = new frmSetting(SettingSignADO, (result) =>
                    {
                        SettingSignADO = result as SettingSignADO;
                    });
                    frm.ShowDialog();

                    if (SettingSignADO == null || string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                    {
                        chkSignXml.Checked = false;
                    }
                }

                // Lưu trạng thái
                SaveControlState("chkSignXml", chkSignXml.Checked ? "1" : "0");
                SaveControlState("SignXml_Config", SettingSignADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveControlState(string key, object value)
        {
            string json = null;

            if (value != null)
            {
                if (value is string)
                {
                    json = value.ToString();   // không serialize
                }
                else
                {
                    json = JsonConvert.SerializeObject(value);
                }
            }
            var cs = currentControlStateRDO?.FirstOrDefault(o => o.KEY == key && o.MODULE_LINK == this.currentModuleBase.ModuleLink);

            if (cs != null)
            {
                cs.VALUE = json;
            }
            else
            {
                cs = new HIS.Desktop.Library.CacheClient.ControlStateRDO
                {
                    KEY = key,
                    VALUE = json,
                    MODULE_LINK = this.currentModuleBase.ModuleLink
                };
                if (currentControlStateRDO == null) currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                currentControlStateRDO.Add(cs);
            }

            controlStateWorker.SetData(currentControlStateRDO);
        }

        private void InitControlStateSign()
        {
            try
            {
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData(this.currentModuleBase.ModuleLink);

                isNotLoadWhileChangeControlStateInFirst = true;

                var csSign = currentControlStateRDO?.FirstOrDefault(o => o.KEY == "chkSignXml" && o.MODULE_LINK == this.currentModuleBase.ModuleLink);
                chkSignXml.Checked = csSign != null && csSign.VALUE == "1";

                var csDetail = currentControlStateRDO?.FirstOrDefault(o => o.KEY == "SignXml_Config" && o.MODULE_LINK == this.currentModuleBase.ModuleLink);
                if (csDetail != null && !string.IsNullOrEmpty(csDetail.VALUE))
                {
                    SettingSignADO = JsonConvert.DeserializeObject<SettingSignADO>(csDetail.VALUE);
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXmlTT12_Click(object sender, EventArgs e)
        {
            try
            {
                var listAll = gridviewFormList.GridControl.DataSource as List<HisDepartmentADO>;

                // Lọc ra các khoa đã checkbox (IS_CHECK_DEPT = true)
                List<HisDepartmentADO> selectedDepts = listAll.Where(o => o.IS_CHECK_DEPT).ToList();

                if (selectedDepts.Count == 0)
                {
                    XtraMessageBox.Show("Vui lòng checkbox ít nhất một khoa để xuất XML TT12", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (selectedDepts.Count == 0)
                {
                    XtraMessageBox.Show("Không có khoa nào được chọn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Mở dialog chọn thư mục lưu file
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() != DialogResult.OK)
                    return;

                string savePath = fbd.SelectedPath;
                if (String.IsNullOrEmpty(savePath))
                    return;

                WaitingManager.Show();

                // Tạo file XML
                string fullFileName = String.Format("DepartmentTT12_{0}.xml", DateTime.Now.ToString("ddMMyyyy_HHmmss"));
                string saveFilePath = System.IO.Path.Combine(savePath, fullFileName);

                // Tạo và xuất XML  
                var xmlStream = SerializeToXml(selectedDepts);

                if (xmlStream != null)
                {
                    // Ký số nếu được chọn
                    if (chkSignXml.Checked && SettingSignADO != null && !string.IsNullOrEmpty(SettingSignADO.SerialNumber))
                    {
                        xmlStream = SignXmlTT12(xmlStream, SettingSignADO, fullFileName, saveFilePath);
                        if (xmlStream == null)
                        {
                            WaitingManager.Hide();
                            XtraMessageBox.Show("Ký số thất bại", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Ghi file
                    if (xmlStream != null)
                    {
                        using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write))
                        {
                            xmlStream.WriteTo(fileStream);
                        }
                        xmlStream.Close();
                    }

                    WaitingManager.Hide();
                    //MessageBox.Show(String.Format("Xuất XML thành công!\nĐường dẫn: {0}", saveFilePath), "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    WaitingManager.Hide();
                    MessageBox.Show("Lỗi khi tạo file XML", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddXmlElement(XmlDocument doc, XmlElement parent, string elementName, string elementValue)
        {
            try
            {
                if (elementValue != null)
                {
                    XmlElement element = doc.CreateElement(elementName);
                    element.InnerText = elementValue;
                    parent.AppendChild(element);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private MemoryStream SerializeToXml(List<HisDepartmentADO> input)
        {
            try
            {
                
                XmlDocument doc = new XmlDocument();
                // Tạo phần tử gốc
                XmlElement rootElement = doc.CreateElement("DanhSachKhoa");
                rootElement.SetAttribute("ThoiGian", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                doc.AppendChild(rootElement);

                // Thêm thông tin từng khoa
                int stt = 1;
                foreach (var dept in input)
                {
                    XmlElement departmentElement = doc.CreateElement("Khoa");

                    // STT - Số thứ tự tự tăng dần từ 1
                    AddXmlElement(doc, departmentElement, "STT", stt.ToString());

                    // MA_KHOA - Mã khám bệnh/mã khoa (dùng BHYT_CODE)
                    AddXmlElement(doc, departmentElement, "MA_KHOA", dept.BHYT_CODE ?? "");

                    // TEN_KHOA - Tên khoa
                    AddXmlElement(doc, departmentElement, "TEN_KHOA", dept.DEPARTMENT_NAME ?? "");

                    // BAN_KHAM - Số lượng bàn khám (EXAM_DESK_COUNT nếu có)
                    AddXmlElement(doc, departmentElement, "BAN_KHAM",
                dept.EXAM_DESK_COUNT.HasValue ? dept.EXAM_DESK_COUNT.Value.ToString() : "");

                    // GIUONG_PD - Số giường phê duyệt (kế hoạch)
                    AddXmlElement(doc, departmentElement, "GIUONG_PD",
                dept.THEORY_PATIENT_COUNT.HasValue ? dept.THEORY_PATIENT_COUNT.Value.ToString() : "");
                    // GIUONG_TK - Số giường thực tế
                    AddXmlElement(doc, departmentElement, "GIUONG_TK",
                dept.REALITY_PATIENT_COUNT.HasValue ? dept.REALITY_PATIENT_COUNT.Value.ToString() : "");

                    // GIUONG_HSTC - luôn xuất hiện (theo code gốc bạn ghi "0" mặc định)
                    string hstcValue = "";
                    if (dept.ICU_BED_COUNT != null)
                    {
                        hstcValue = dept.ICU_BED_COUNT.Value.ToString();
                    }
                    AddXmlElement(doc, departmentElement, "GIUONG_HSTC", hstcValue);

                    // GIUONG_HSCC
                    AddXmlElement(doc, departmentElement, "GIUONG_HSCC",
                        dept.ER_RESUS_BED_COUNT != null
                            ? dept.ER_RESUS_BED_COUNT.Value.ToString()
                            : "");

                    // TU_NGAY - Thời gian hiệu lực từ (định dạng YYYYMMDD từ FROM_TIME)
                    string tuNgay = "";
                    if (dept.FROM_TIME != null)
                    {
                        tuNgay = FormatTimeToYYYYMMDD(dept.FROM_TIME.Value) ?? "";
                    }
                    AddXmlElement(doc, departmentElement, "TU_NGAY", tuNgay);

                    // DEN_NGAY - chỉ có khi khoa ngừng hoạt động, còn lại để rỗng
                    string denNgay = "";
                    if (dept.TO_TIME != null)
                    {
                        denNgay = FormatTimeToYYYYMMDD(dept.TO_TIME.Value) ?? "";
                    }
                    AddXmlElement(doc, departmentElement, "DEN_NGAY", denNgay);

                    // MA_CSKCB
                    string maCSKCB = "";
                    if (dept.BRANCH_ID > 0)
                    {
                        var branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == dept.BRANCH_ID);
                        if (branch != null && !string.IsNullOrEmpty(branch.HEIN_MEDI_ORG_CODE))
                        {
                            maCSKCB = branch.HEIN_MEDI_ORG_CODE;
                        }
                    }
                    AddXmlElement(doc, departmentElement, "MA_CSKCB", maCSKCB);

                    rootElement.AppendChild(departmentElement);
                    stt++;
                }

                MemoryStream stream = new MemoryStream();
                var xmlSettings = new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    OmitXmlDeclaration = false
                };

                using (XmlWriter writer = XmlWriter.Create(stream, xmlSettings))
                {
                    doc.WriteTo(writer);
                }
                stream.Position = 0;
                return stream;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private string FormatTimeToYYYYMMDD(long timeValue)
        {
            try
            {
                // timeValue được lưu ở dạng số (ví dụ: 20260312000000)
                string timeStr = timeValue.ToString("D14").PadLeft(14, '0');
                if (timeStr.Length >= 8)
                {
                    // Lấy 4 ký tự năm + 2 ký tự tháng + 2 ký tự ngày
                    return timeStr.Substring(0, 8);
                }
                return "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Lỗi khi format thời gian: " + ex.Message);
                return "";
            }
        }


        private MemoryStream SignXmlDocument(MemoryStream xmlStream, SettingSignADO signInfo)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Ký số XML với Serial: " + signInfo.SerialNumber);

                // Tạm thời return nguyên file (chưa ký)
                return xmlStream;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Lỗi khi ký XML: " + ex.Message);
                return xmlStream;
            }
        }

        private void deTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (deTimeFrom.EditValue != null && deTimeFrom.DateTime != DateTime.MinValue)
                {
                    deTimeTo.Enabled = false;
                    deTimeTo.EditValue = null;
                }
                else
                {
                    deTimeTo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void deTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (deTimeTo.EditValue != null && deTimeTo.DateTime != DateTime.MinValue)
                {
                    deTimeFrom.Enabled = false;
                    deTimeFrom.EditValue = null;
                }
                else
                {
                    deTimeFrom.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridviewFormList_MouseDown(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = view.CalcHitInfo(e.Location);

            if (hitInfo.InColumn && hitInfo.Column.FieldName == "IS_CHECK_DEPT")
            {
                isHeaderChecked = !isHeaderChecked;

                List<HisDepartmentADO> list = gridviewFormList.DataSource as List<HisDepartmentADO>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (isHeaderChecked)
                        {
                            if (item.IS_ACTIVE == 1)
                            {
                                item.IS_CHECK_DEPT = true;
                            }
                        }
                        else
                        {
                            item.IS_CHECK_DEPT = false;
                        }
                    }
                    gridviewFormList.RefreshData();
                }
                view.InvalidateColumnHeader(hitInfo.Column); // vẽ lại checkbox header
            }
        }




        private void gridviewFormList_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "IS_CHECK_DEPT")
            {
                bool allActiveChecked = true;

                for (int i = 0; i < gridviewFormList.RowCount; i++)
                {
                    var row = gridviewFormList.GetRow(i) as HisDepartmentADO;

                    if (row != null && row.IS_ACTIVE == 1)
                    {
                        bool isChecked;

                        if (i == e.RowHandle)
                        {
                            // dùng giá trị mới
                            isChecked = Convert.ToBoolean(e.Value);
                        }
                        else
                        {
                            // dùng giá trị hiện tại
                            isChecked = Convert.ToBoolean(gridviewFormList.GetRowCellValue(i, "IS_CHECK_DEPT"));
                        }

                        if (!isChecked)
                        {
                            allActiveChecked = false;
                            break;
                        }
                    }
                }

                isHeaderChecked = allActiveChecked;

                gridviewFormList.InvalidateColumnHeader(gridviewFormList.Columns["IS_CHECK_DEPT"]);
            }
        }
        private MemoryStream SignXmlTT12(MemoryStream xmlStream, SettingSignADO signInfo, string fullFileName, string saveFilePath)
        {
            try
            {
                if (signInfo == null || string.IsNullOrEmpty(signInfo.SerialNumber))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Không có thông tin USB Token để ký");
                    return xmlStream; // trả về file gốc nếu không ký được
                }

                string currentDirectory = Directory.GetCurrentDirectory();
                string tempFolderPath = Path.Combine(currentDirectory, "Temp");
                Directory.CreateDirectory(tempFolderPath);
                string tempFilePath = Path.Combine(tempFolderPath, fullFileName);

                // Lưu tạm file XML gốc để ký
                using (FileStream tempFile = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write))
                {
                    xmlStream.WriteTo(tempFile);
                }
                xmlStream.Position = 0;

                string pathAfterSign = null;
                WcfSignDCO wcfSignDCO = null;

                if (signInfo.IsHsm)
                {
                    // Ký HSM (nếu dùng HSM)
                    var xmlBase64 = SourceFileSignApi(ConvertStreamToBase64(xmlStream));
                    if (string.IsNullOrEmpty(xmlBase64))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký HSM thất bại");
                        return xmlStream;
                    }

                    var xmlBytes = Convert.FromBase64String(xmlBase64);
                    File.WriteAllBytes(tempFilePath, xmlBytes);
                    pathAfterSign = tempFilePath;
                }
                else
                {
                    // Ký USB Token qua service
                    wcfSignDCO = new WcfSignDCO
                    {
                        SerialNumber = signInfo.SerialNumber,
                        OutputFile = tempFilePath,
                        PIN = "", // nếu cần PIN thì truyền vào đây
                        SourceFile = tempFilePath,
                        fieldSigned = "CHUKYDONVI" // giống XML130
                    };

                    string jsonData = JsonConvert.SerializeObject(wcfSignDCO);
                    SignProcessorClient signProcessorClient = new SignProcessorClient();

                    if (!VerifyServiceSignProcessorIsRunning())
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Service ký số không chạy");
                        return xmlStream;
                    }

                    var wcfSignResult = signProcessorClient.SignXml130(jsonData);
                    if (wcfSignResult == null || !wcfSignResult.Success)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("Ký file thất bại: " + (wcfSignResult?.Message ?? ""));
                        return xmlStream;
                    }

                    pathAfterSign = wcfSignResult.OutputFile;
                }

                // Đọc file đã ký và trả về stream
                if (!string.IsNullOrEmpty(pathAfterSign) && File.Exists(pathAfterSign))
                {
                    byte[] signedBytes = File.ReadAllBytes(pathAfterSign);
                    MemoryStream signedStream = new MemoryStream(signedBytes);
                    signedStream.Position = 0;
                    return signedStream;
                }

                // Xóa file temp
                if (File.Exists(tempFilePath)) File.Delete(tempFilePath);
                if (Directory.Exists(tempFolderPath) && Directory.GetFiles(tempFolderPath).Length == 0)
                    Directory.Delete(tempFolderPath);

                return xmlStream; // nếu ký thất bại, trả về file gốc
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return xmlStream;
            }
        }
        private string SourceFileSignApi(string xmlBase64Source)
        {
            string result = null;
            try
            {
                CommonParam param = new CommonParam();
                EMR.SDO.SignXmlBhytSDO signXmlBhytSDO = new EMR.SDO.SignXmlBhytSDO();
                signXmlBhytSDO.XmlBase64 = xmlBase64Source;
                signXmlBhytSDO.TagStoreSignatureValue = "CHUKYDONVI";
                signXmlBhytSDO.ConfigData = new EMR.SDO.XmlConfigDataSDO() { HsmSerialNumber = SettingSignADO.SerialNumber, HsmType = SettingSignADO.Id, HsmUserCode = SettingSignADO.Name, Password = SettingSignADO.Password, SecretKey = SettingSignADO.SercetKey, IdentityNumber = SettingSignADO.CccdNumber };
                result = new Inventec.Common.Adapter.BackendAdapter(param).Post<string>("api/EmrSign/SignXmlBhyt", ApiConsumer.ApiConsumers.EmrConsumer, signXmlBhytSDO, SessionManager.ActionLostToken, param);
                if (param != null && param.Messages != null && param.Messages.Count > 0)
                {
                    string message = string.Join(Environment.NewLine, param.Messages);
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Inventec.Common.Logging.LogSystem.Warn(message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        internal bool VerifyServiceSignProcessorIsRunning()
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.1");
                string exeSignPath = AppFilePathSignService();
                if (File.Exists(exeSignPath))
                {
                    if (IsProcessOpen("EMR.SignProcessor"))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.2");
                        valid = true;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.3");
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => exeSignPath), exeSignPath));
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.FileName = exeSignPath;
                        try
                        {

                            Process.Start(startInfo);
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.4");
                            Thread.Sleep(500);
                            valid = true;
                            Inventec.Common.Logging.LogSystem.Debug("GetSerialNumber.5");
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
        public string AppFilePathSignService()
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(Path.Combine(Application.StartupPath, "Integrate"), "EMR.SignProcessor"), "EMR.SignProcessor.exe");
                return pathFolderTemp;
            }
            catch (IOException exception)
            {
                Inventec.Common.Logging.LogSystem.Warn("Error create temp file: " + exception.Message);
                return "";
            }
        }
        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        // Hàm hỗ trợ convert stream sang base64 (dùng cho HSM)
        private string ConvertStreamToBase64(MemoryStream stream)
        {
            stream.Position = 0;
            byte[] buffer = stream.ToArray();
            return Convert.ToBase64String(buffer);
        }
    }
}


