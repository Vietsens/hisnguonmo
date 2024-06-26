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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.HisMedicalAssessment.ADO;
using ACS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utilities.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using Telerik.WinForms.Documents;
using HIS.Desktop.Plugins.HisMedicalAssessment.Resources;
using DevExpress.Utils.Menu;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using EMR.TDO;
using DevExpress.Office.Utils;
using DevExpress.XtraGrid.Columns;
using System.Web.UI.WebControls;
using MOS.SDO;
using DevExpress.XtraGrid;
using System.Globalization;

namespace HIS.Desktop.Plugins.HisMedicalAssessment
{
    public partial class frmHisMedicalAssessment : FormBase
    {
        public static int ADD = 1;

        public static int DELETE = 2;
        List<HIS_ASSESSMENT_MEMBER> listAssMem { get; set; }
        HIS_MEDICAL_ASSESSMENT currentMedicalAss { get; set; }
        V_HIS_MEDICAL_ASSESSMENT vMedicalAss { get; set; }
        internal MOS.EFMODEL.DataModels.HIS_TREATMENT currentTreatment { get; set; }
        internal long treatmentId { get; set; }
        Inventec.Desktop.Common.Modules.Module currentModule;
        private int positionHandle;

        public List<ACS_USER> listAcsUser { get; set; }
        public List<HIS_ASSESSMENT_OBJECT> listAssObj { get; set; }
        public List<HIS_WELFARE_TYPE> listWelfareType { get; set; }
        public List<GridADO> listDataTable1 { get; set; }
        public List<GridADO> listDataTable2 { get; set; }
        public List<GridADO> listDataTable3 { get; set; }
        public frmHisMedicalAssessment(Inventec.Desktop.Common.Modules.Module _Module, long treatmentId)
             : base(_Module)
        {
            try
            {
                InitializeComponent();
                SetIcon();
                this.currentModule = _Module;
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmHisMedicalAssessment
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMedicalAssessment.Resources.Lang", typeof(frmHisMedicalAssessment).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCan.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.btnCan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOk.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.btnOk.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDel.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.btnDel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox2.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.groupBox2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label3.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.label3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label2.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.label2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDisabilityStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboDisabilityStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDisabilityType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboDisabilityType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtLegalGroundDocuments.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.txtLegalGroundDocuments.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtLegalGroundDocuments.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.txtLegalGroundDocuments.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtLegalGroundNumbers.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.txtLegalGroundNumbers.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtLegalGroundNumbers.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem18.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem24.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem27.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.groupBox1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.label1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.label1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboWelfareType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboWelfareType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAssObj.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboAssObj.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtRequestOrgName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.txtRequestOrgName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtRequestOrgCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.txtRequestOrgCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAssType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboAssType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPresident.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboPresident.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSecretary.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.cboSecretary.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem1.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem4.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem7.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem9.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem10.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem12.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem13.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem31.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem32.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem33.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisMedicalAssessment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void frmHisMedicalAssessment_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                TaskValidte();
                CreateThreadLoadData();
                TaskLoadCombo();
                InitTypeFind();
                LoadDataControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task TaskLoadCombo()
        {
            try
            {
                FillDataToGridLookupEdit(cboAssType, ComboADO.listAssType());
                FillDataToGridLookupEdit(cboPresident, listAcsUser, "LOGINNAME", "USERNAME", "LOGINNAME");
                FillDataToGridLookupEdit(cboSecretary, listAcsUser, "LOGINNAME", "USERNAME", "LOGINNAME");
                FillDataToGridLookupEdit(cboAssObj, listAssObj, "ID", "ASSESSMENT_OBJECT_NAME", "ASSESSMENT_OBJECT_CODE");
                FillDataToGridLookupEdit(cboDisabilityType, ComboADO.listDisabilityType());
                FillDataToGridLookupEdit(cboDisabilityStatus, ComboADO.listDisabilityStatus());
                FillDataToComboSelect();
                FillDefaultTable();
                FillDataToRepGridLookupEdit(repLoginTab1, listAcsUser);
                FillDataToRepGridLookupEdit(repLoginTab2, listAcsUser);
                FillDataToRepGridLookupEdit(repLoginTab3, listAcsUser);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task FillDefaultTable()
        {
            try
            {
                listDataTable1 = new List<GridADO>();
                listDataTable1.Add(new GridADO());
                gridControl1.DataSource = null;
                gridControl1.DataSource = listDataTable1;

                listDataTable2 = new List<GridADO>();
                listDataTable2.Add(new GridADO());
                gridControl2.DataSource = null;
                gridControl2.DataSource = listDataTable2;

                listDataTable3 = new List<GridADO>();
                listDataTable3.Add(new GridADO());
                gridControl3.DataSource = null;
                gridControl3.DataSource = listDataTable3;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillDataToRepGridLookupEdit(Inventec.Desktop.CustomControl.RepositoryItemCustomGridLookUpEdit cboEditor, object datasource, string value = null, string displayName = null)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = "LOGINNAME";
                    displayName = "USERNAME";
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(value, "", 100, 1));
                columnInfos.Add(new ColumnInfo(displayName, "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayName, value, columnInfos, false, 400);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private async Task FillDataToGridLookupEdit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, object datasource, string value = null, string displayName = null, string displayCode = null)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                {
                    displayCode = value = "Value";
                    displayName = "Name";
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayCode, "", 100, 1));
                columnInfos.Add(new ColumnInfo(displayName, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayName, value, columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private async Task FillDataToComboSelect()
        {
            try
            {
                var lstWelfareType = BackendDataWorker.Get<HIS_WELFARE_TYPE>();
                if (lstWelfareType != null && lstWelfareType.Count > 0)
                {
                    InitCheck(cboWelfareType, SelectionGrid__cboWelfareType);
                    InitCombo(cboWelfareType, lstWelfareType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), "WELFARE_TYPE_NAME", "WELFARE_TYPE_CODE", "ID");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string CodeValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col1 = cbo.Properties.View.Columns.AddField(CodeValue);
                col1.VisibleIndex = 1;
                col1.Width = 100;
                col1.Caption = "Mã";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 2;
                col2.Width = 200;
                col2.Caption = "Tên";
                cbo.Properties.PopupFormWidth = 300;
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
        private void SelectionGrid__cboWelfareType(object sender, EventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_WELFARE_TYPE> sgSelectedNews = new List<HIS_WELFARE_TYPE>();
                    foreach (HIS_WELFARE_TYPE rv in (gridCheckMark).Selection)
                    {
                        if (rv != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append("; "); }
                            sb.Append(rv.WELFARE_TYPE_NAME.ToString());
                            sgSelectedNews.Add(rv);
                        }
                    }
                    this.listWelfareType = new List<HIS_WELFARE_TYPE>();
                    this.listWelfareType.AddRange(sgSelectedNews);

                }
                this.cboWelfareType.Text = sb.ToString();
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

        private async Task TaskValidte()
        {
            try
            {
                ValidMaxLength(txtAssName, 200, true);
                ValidRequired(cboAssType);
                ValidMaxLength(txtReportNumber, 20);
                ValidRequired(dteAssTo);
                ValidRequired(dteAssFrom);
                ValidRequired(cboPresident);
                ValidRequired(cboSecretary);
                ValidMaxLength(txtRequestOrgCode, 20);
                ValidMaxLength(txtRequestOrgName, 1000);
                ValidMaxLength(txtReferalCode, 20);
                ValidRequired(cboAssObj);
                ValidMaxLength(memAssPurpose, 1000);
                ValidMaxLength(memAssRequestContent, 1000, true);
                ValidMaxLength(memPathologicalHistory, 1000, false);
                ValidMaxLength(memExaminationResult, 1000, false);
                ValidMaxLength(memDiscussion, 1000, false);
                ValidMaxLength(txtLegalGroundNumbers, 200);
                ValidMaxLength(txtLegalGroundDocuments, 1000);
                ValidMaxLength(txtConClusion, 1000);
                ValidMaxLength(txtRequestAfterAss, 1000);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task ValidMaxLength(BaseEdit baseEdit, int maxlength, bool IsRequired = false, BaseEdit baseEditShowValid = null)
        {
            try
            {
                ValidMaxlength valid = new ValidMaxlength();
                valid.textEdit = baseEdit;
                valid.IsRequired = IsRequired;
                valid.maxLength = maxlength;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                if (baseEditShowValid == null)
                    baseEditShowValid = baseEdit;
                dxValidationProvider1.SetValidationRule(baseEditShowValid, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private async Task ValidRequired(BaseEdit baseEdit)
        {
            try
            {
                ValidRequired valid = new ValidRequired();
                valid.be = baseEdit;
                valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(baseEdit, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataControl()
        {
            try
            {
                if (currentMedicalAss == null)
                {
                    DefaultForm();
                    return;
                }
                btnPrint.Enabled = true;
                txtAssName.Text = currentMedicalAss.ASSESSMENT_BOARD_NAME;
                cboAssType.EditValue = currentMedicalAss.ASSESSMENT_TYPE_ID;
                txtReportNumber.Text = currentMedicalAss.REPORT_NUMBER;
                dteAssFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentMedicalAss.ASSESSMENT_TIME_FROM) ?? DateTime.Now;
                if (currentMedicalAss.ASSESSMENT_TIME_TO != null)
                    dteAssTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentMedicalAss.ASSESSMENT_TIME_TO ?? 0) ?? DateTime.Now;
                if (listAssMem != null && listAssMem.Count > 0)
                {
                    CreateThreadLoadGrid();
                    var present = listAssMem.Where(o => o.IS_PRESIDENT == 1).ToList();
                    if (present != null)
                        cboPresident.EditValue = present.OrderByDescending(o => o.ID).First().LOGINNAME;
                    var secretary = listAssMem.Where(o => o.IS_SECRETARY == 1).ToList();
                    if (secretary != null)
                        cboSecretary.EditValue = secretary.OrderByDescending(o => o.ID).First().LOGINNAME;
                }
                txtRequestOrgCode.Text = currentMedicalAss.REQUEST_ORG_CODE;
                txtRequestOrgName.Text = currentMedicalAss.REQUEST_ORG_NAME;
                txtReferalCode.Text = currentMedicalAss.REFERRAL_CODE;
                if (currentMedicalAss.REQUEST_TIME != null)
                    dteRequestTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(currentMedicalAss.REQUEST_TIME ?? 0) ?? DateTime.Now;
                memAssPurpose.Text = currentMedicalAss.ASSESSMENT_PURPOSE;
                cboAssObj.EditValue = currentMedicalAss.ASSESSMENT_OBJECT_ID;
                memAssRequestContent.Text = currentMedicalAss.ASSESSMENT_REQUEST_CONTENT;
                if (currentMedicalAss.PREVIOUS_INJURY_RATE != null)
                    txtPreviousInJuryRate.Text = ((currentMedicalAss.PREVIOUS_INJURY_RATE ?? 0) * 100).ToString();
                else
                    txtPreviousInJuryRate.Text = null;
                ProcessSelectWelf(cboWelfareType, currentMedicalAss.WELFARE_TYPE_IDS);
                memPathologicalHistory.Text = currentMedicalAss.PATHOLOGICAL_HISTORY;
                memExaminationResult.Text = currentMedicalAss.EXAMINATION_RESULT;
                memDiscussion.Text = currentMedicalAss.DISCUSSION;
                txtLegalGroundNumbers.Text = currentMedicalAss.LEGAL_GROUND_NUMBERS;
                txtLegalGroundDocuments.Text = currentMedicalAss.LEGAL_GROUND_DOCUMENTS;
                txtConClusion.Text = currentMedicalAss.CONCLUSION;
                cboDisabilityType.EditValue = currentMedicalAss.DISABILITY_TYPE_ID;
                cboDisabilityStatus.EditValue = currentMedicalAss.DISABILITY_STATUS_ID;
                if (currentMedicalAss.INJURY_RATE != null)
                    txtInjuryRate.Text = ((currentMedicalAss.INJURY_RATE ?? 0) * 100).ToString();
                else
                    txtInjuryRate.Text = null;
                if (currentMedicalAss.INJURY_RATE_TOTAL != null)
                    txtInJuryRateTotal.Text = ((currentMedicalAss.INJURY_RATE_TOTAL ?? 0) * 100).ToString();
                else
                    txtInJuryRateTotal.Text = null;
                txtRequestAfterAss.Text = currentMedicalAss.REQUEST_AFTER_ASSESSMENT;
                gridControl1.DataSource = null;
                gridControl1.DataSource = listDataTable1;
                gridControl2.DataSource = null;
                gridControl2.DataSource = listDataTable2;
                gridControl3.DataSource = null;
                gridControl3.DataSource = listDataTable3;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessSelectWelf(GridLookUpEdit cbo, string p)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                gridCheckMark.ClearSelection(cbo.Properties.View);
                if (!String.IsNullOrWhiteSpace(p) && cbo.Properties.Tag != null)
                {
                    List<HIS_WELFARE_TYPE> ds = cbo.Properties.DataSource as List<HIS_WELFARE_TYPE>;
                    string[] arrays = p.Split(';');
                    if (arrays != null && arrays.Length > 0)
                    {
                        List<HIS_WELFARE_TYPE> selects = new List<HIS_WELFARE_TYPE>();
                        foreach (var item in arrays)
                        {
                            var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                            if (row != null)
                            {
                                selects.Add(row);
                            }
                        }
                        gridCheckMark.SelectAll(selects);
                    }
                }
                else
                {
                    cbo.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cbo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cbo.Properties.View);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadLoadGrid()
        {
            try
            {
                Thread threadLoadGrid1 = new Thread(new ThreadStart(LoadGrid1));
                Thread threadLoadGrid2 = new Thread(new ThreadStart(LoadGrid2));
                Thread threadLoadGrid3 = new Thread(new ThreadStart(LoadGrid3));
                try
                {
                    threadLoadGrid1.Start();
                    threadLoadGrid2.Start();
                    threadLoadGrid3.Start();
                    threadLoadGrid1.Join();
                    threadLoadGrid2.Join();
                    threadLoadGrid3.Join();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    threadLoadGrid1.Abort();
                    threadLoadGrid2.Abort();
                    threadLoadGrid3.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGrid3()
        {
            try
            {
                var lstAssMemTb3 = listAssMem.Where(o => o.IS_GUEST == 1).ToList();
                int count = 0;
                listDataTable3 = new List<GridADO>();
                foreach (var item in lstAssMemTb3)
                {
                    GridADO ado = new GridADO();
                    ado.LOGINNAME = item.LOGINNAME;
                    ado.USERNAME = item.USERNAME;
                    if (count == 0)
                        ado.ActionType = ADD;
                    else
                        ado.ActionType = DELETE;
                    count++;
                    listDataTable3.Add(ado);
                }
                if (listDataTable3 == null || listDataTable3.Count == 0)
                {
                    listDataTable3 = new List<GridADO>();
                    listDataTable3.Add(new GridADO());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGrid2()
        {
            try
            {
                var lstAssMemTb2 = listAssMem.Where(o => o.IS_DISAGREED == 1).ToList();
                int count = 0;
                listDataTable2 = new List<GridADO>();
                foreach (var item in lstAssMemTb2)
                {
                    GridADO ado = new GridADO();
                    ado.LOGINNAME = item.LOGINNAME;
                    ado.USERNAME = item.USERNAME;
                    ado.DISAGREE_REASON = item.DISAGREE_REASON;
                    if (count == 0)
                        ado.ActionType = ADD;
                    else
                        ado.ActionType = DELETE;
                    count++;
                    listDataTable2.Add(ado);
                }
                if (listDataTable2 == null || listDataTable2.Count == 0)
                {
                    listDataTable2 = new List<GridADO>();
                    listDataTable2.Add(new GridADO());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadGrid1()
        {
            try
            {
                var lstAssMemTab1 = listAssMem.Where(o => o.IS_SECRETARY != 1 && o.IS_PRESIDENT != 1 && o.IS_GUEST != 1).ToList();
                int count = 0;
                listDataTable1 = new List<GridADO>();
                foreach (var item in lstAssMemTab1)
                {
                    GridADO ado = new GridADO();
                    ado.LOGINNAME = item.LOGINNAME;
                    ado.USERNAME = item.USERNAME;
                    ado.IS_ABS = item.IS_ABSENT == 1 ? true : false;
                    ado.IS_ON_BE = item.ON_BEHALF_TO_SIGN == 1 ? true : false;
                    if (count == 0)
                        ado.ActionType = ADD;
                    else
                        ado.ActionType = DELETE;
                    count++;
                    listDataTable1.Add(ado);
                }
                if (listDataTable1 == null || listDataTable1.Count == 0)
                {
                    listDataTable1 = new List<GridADO>();
                    listDataTable1.Add(new GridADO());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateThreadLoadData()
        {
            try
            {
                Thread threadLoadTreatment = new Thread(new ThreadStart(LoadTreatment));
                Thread threadLoadMedicalAssessment = new Thread(new ThreadStart(LoadMedicalAssessment));
                Thread threadLoadAcsUser = new Thread(new ThreadStart(LoadAcsUser));
                Thread threadLoadAssObj = new Thread(new ThreadStart(LoadAssObj));
                try
                {
                    threadLoadAssObj.Start();
                    threadLoadMedicalAssessment.Start();
                    threadLoadTreatment.Start();
                    threadLoadAcsUser.Start();
                    threadLoadAssObj.Join();
                    threadLoadMedicalAssessment.Join();
                    threadLoadTreatment.Join();
                    threadLoadAcsUser.Join();
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    threadLoadTreatment.Abort();
                    threadLoadAcsUser.Abort();
                    threadLoadAssObj.Abort();
                    threadLoadMedicalAssessment.Abort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAssObj()
        {
            try
            {
                var lstAssmentObj = BackendDataWorker.Get<HIS_ASSESSMENT_OBJECT>();
                if (lstAssmentObj != null && lstAssmentObj.Count > 01)
                    listAssObj = lstAssmentObj.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAcsUser()
        {
            try
            {
                listAcsUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicalAssessment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicalAssessmentFilter filter = new HisMedicalAssessmentFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.IS_DELETE = 0;
                var data = new BackendAdapter(param)
                    .Get<List<HIS_MEDICAL_ASSESSMENT>>("api/HisMedicalAssessment/Get", ApiConsumers.MosConsumer, filter, param);
                if (data != null && data.Count > 0)
                {
                    currentMedicalAss = data.OrderByDescending(o => o.ID).First();
                    LoadAssessmentMember();
                    LoadViewMedicalAssessment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadViewMedicalAssessment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicalAssessmentViewFilter filter = new HisMedicalAssessmentViewFilter();
                filter.ID = currentMedicalAss.ID;
                var data = new BackendAdapter(param)
                    .Get<List<V_HIS_MEDICAL_ASSESSMENT>>("api/HisMedicalAssessment/GetView", ApiConsumers.MosConsumer, filter, param);
                if (data != null && data.Count > 0)
                {
                    vMedicalAss = data.OrderByDescending(o => o.ID).First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                filter.ID = treatmentId;
                var data = new BackendAdapter(param)
                    .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                if (data != null && data.Count > 0)
                {
                    currentTreatment = data.First();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadAssessmentMember()
        {
            try
            {
                if (currentMedicalAss == null)
                    return;
                CommonParam param = new CommonParam();
                MOS.Filter.HisAssessmentMemberFilter filter = new HisAssessmentMemberFilter();
                filter.MEDICAL_ASSESSMENT_ID = currentMedicalAss.ID;
                listAssMem = new BackendAdapter(param)
                    .Get<List<HIS_ASSESSMENT_MEMBER>>("api/HisAssessmentMember/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repbtnAddTab1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                listDataTable1.Add(new GridADO());
                RefreshDataGrid(gridControl1, listDataTable1);
                gridView1.FocusedRowHandle = gridView1.RowCount - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repbtnMinusTab1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (GridADO)gridView1.GetFocusedRow();
                listDataTable1.Remove(data);
                RefreshDataGrid(gridControl1, listDataTable1);
                gridView1.FocusedRowHandle = gridView1.RowCount - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void RefreshDataGrid(DevExpress.XtraGrid.GridControl grid, List<GridADO> lstData)
        {
            try
            {
                int count = 0;
                lstData.ForEach(o =>
                {
                    if (count == 0)
                        o.ActionType = ADD;
                    else
                        o.ActionType = DELETE;
                    count++;
                });
                grid.DataSource = null;
                grid.DataSource = lstData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repbtnAddTab3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                listDataTable3.Add(new GridADO());
                RefreshDataGrid(gridControl3, listDataTable3);
                gridView3.FocusedRowHandle = gridView3.RowCount - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repbtnMinusTab3_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (GridADO)gridView3.GetFocusedRow();
                listDataTable3.Remove(data);
                RefreshDataGrid(gridControl3, listDataTable3);
                gridView3.FocusedRowHandle = gridView3.RowCount - 1;
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
                bool success = false;
                btnSave.Focus();
                if (!btnSave.Enabled)
                    return;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteAssTo.DateTime) < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteAssFrom.DateTime))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Chỉ cho phép nhập \"Giám định đến\" lớn hơn hoặc bằng \"Giám định từ\".", ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    return;
                }    
                var lstTab1Save = listDataTable1.Where(o => !string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                if (lstTab1Save == null || lstTab1Save.Count() == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapThanhVien, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    gridView1.FocusedColumn = gridColumn1;
                    gridView1.FocusedRowHandle = 0;
                    gridView1.ShowEditor();
                    return;
                }
                else
                {
                    var lstLoginName1 = lstTab1Save.Select(o => o.LOGINNAME).Distinct().ToList();
                    if(lstLoginName1.Exists(o=> cboPresident.EditValue.ToString().Equals(o) || cboSecretary.EditValue.ToString().Equals(o)))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThanhVienChuTriHoacThuKy, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }
                    if (lstLoginName1.Count != lstTab1Save.Count)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThanhVienKhongTrungNhau, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }
                    if (lstTab1Save.FirstOrDefault(o=>o.IS_ABS && o.IS_ON_BE) != null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Người được khai báo \"Vắng mặt\" không cho phép \"Ký thay\"", ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }
                }
                var lstTab3Save = listDataTable3.Where(o => !string.IsNullOrEmpty(o.LOGINNAME)).ToList();
                if (lstTab3Save != null && lstTab3Save.Count > 0)
                {
                    var lstLoginName3 = lstTab3Save.Select(o => o.LOGINNAME).Distinct().ToList();
                    if (lstLoginName3.Exists(o => cboPresident.EditValue.ToString().Equals(o) || cboSecretary.EditValue.ToString().Equals(o)))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThanhPhanDuocMoiChuTriHoacThuKy, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }
                    if (lstLoginName3.Count != lstTab3Save.Count)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThanhVienDuocMoiKhongTrungNhau, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }
                    if(lstTab1Save.Where(o=>o.IS_ON_BE).ToList().Count > 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThanhVienKyThay, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }    
                    if (lstTab1Save.Exists(o => lstTab3Save.Exists(p => p.LOGINNAME.Equals(o.LOGINNAME))))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ThanhVienDuocMoiTrungThanhVien, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        return;
                    }
                }
                listDataTable2 = listDataTable2.Where(o => !string.IsNullOrEmpty(o.LOGINNAME) || !string.IsNullOrEmpty(o.DISAGREE_REASON)).ToList();
                var lstTab2NameSave = listDataTable2.Where(o => !string.IsNullOrEmpty(o.LOGINNAME) && string.IsNullOrEmpty(o.DISAGREE_REASON)).ToList();
                if (lstTab2NameSave != null && lstTab2NameSave.Count() > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapYKien, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    gridView2.FocusedColumn = gridColumn6;
                    gridView2.FocusedRowHandle = listDataTable2.IndexOf(lstTab2NameSave.First());
                    gridView2.ShowEditor();
                    return;
                }
                var lstTab2ReasonSave = listDataTable2.Where(o => string.IsNullOrEmpty(o.LOGINNAME) && !string.IsNullOrEmpty(o.DISAGREE_REASON)).ToList();
                if (lstTab2ReasonSave != null && lstTab2ReasonSave.Count() > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.ChuaNhapHoTen, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    gridView2.FocusedColumn = gridColumn5;
                    gridView2.FocusedRowHandle = listDataTable2.IndexOf(lstTab2ReasonSave.First());
                    gridView2.ShowEditor();
                    return;
                }
                if (listDataTable2.Select(o => o.LOGINNAME).Distinct().ToList().Count != listDataTable2.Count)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.KhongNhatTriKhongTrungNhau, ResourceMessage.ThongBao, MessageBoxButtons.OK);
                    return;
                }
                MOS.SDO.HisMedicalAssessmentSDO sdo = new MOS.SDO.HisMedicalAssessmentSDO();
                sdo.TreatmentId = treatmentId;
                sdo.HisMedicalAssessement = GetObj();
                sdo.HisAssessmentMembers = GetListObj(lstTab1Save, listDataTable2, lstTab3Save);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var rs = new BackendAdapter(param).Post<HisMedicalAssessmentResultSDO>("api/HisMedicalAssessment/Save", ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                vMedicalAss = null;
                if (rs != null)
                {
                    success = true;
                    vMedicalAss = rs.vHisMedicalAssessment;
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_MEDICAL_ASSESSMENT>(currentMedicalAss, vMedicalAss);
                    listAssMem = rs.HisAssessmentMember;
                    btnPrint.Enabled = true;
                }

                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HIS_ASSESSMENT_MEMBER> GetListObj(List<GridADO> lstGrid1, List<GridADO> lstGrid2, List<GridADO> lstGrid3)
        {
            List<HIS_ASSESSMENT_MEMBER> lstObj = new List<HIS_ASSESSMENT_MEMBER>();
            try
            {

                List<HIS_ASSESSMENT_MEMBER> lstTmp = new List<HIS_ASSESSMENT_MEMBER>();
                if (cboPresident.EditValue != null)
                {
                    lstTmp.Add(new HIS_ASSESSMENT_MEMBER() { IS_PRESIDENT = 1, LOGINNAME = cboPresident.EditValue.ToString(), USERNAME = cboPresident.Text.Trim() });
                }
                if (cboSecretary.EditValue != null)
                {
                    if (lstTmp[0].LOGINNAME == cboSecretary.EditValue.ToString())
                        lstTmp[0].IS_SECRETARY = 1;
                    else
                        lstTmp.Add(new HIS_ASSESSMENT_MEMBER() { IS_SECRETARY = 1, LOGINNAME = cboSecretary.EditValue.ToString(), USERNAME = cboSecretary.Text.Trim() });
                }
                foreach (var item in lstGrid1)
                {
                    lstTmp.Add(new HIS_ASSESSMENT_MEMBER() { IS_SECRETARY = null, IS_PRESIDENT = null, LOGINNAME = item.LOGINNAME, IS_GUEST = null, ON_BEHALF_TO_SIGN = item.IS_ON_BE ? (short?)1 : null, IS_ABSENT = item.IS_ABS ? (short?)1 : null, USERNAME = item.USERNAME });
                }
                var lstNotExist = lstGrid2.Where(o => !lstTmp.Exists(p => p.LOGINNAME.Equals(o.LOGINNAME)));
                foreach (var item in lstNotExist)
                {
                    lstObj.Add(new HIS_ASSESSMENT_MEMBER() { LOGINNAME = item.LOGINNAME, IS_DISAGREED = 1, USERNAME = item.USERNAME, DISAGREE_REASON = item.DISAGREE_REASON });
                }
                foreach (var item in lstTmp)
                {
                    var data = lstGrid2.FirstOrDefault(o => o.LOGINNAME == item.LOGINNAME);
                    if (data != null)
                    {
                        item.IS_DISAGREED = 1;
                        item.DISAGREE_REASON = data.DISAGREE_REASON;
                    }
                }
                lstObj.AddRange(lstTmp);              
                foreach (var item in lstGrid3)
                {
                    lstObj.Add(new HIS_ASSESSMENT_MEMBER() { LOGINNAME = item.LOGINNAME, IS_GUEST = 1, USERNAME = item.USERNAME });
                }
            }
            catch (Exception ex)
            {
                lstObj = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return lstObj;
        }

        private HIS_MEDICAL_ASSESSMENT GetObj()
        {
            HIS_MEDICAL_ASSESSMENT obj = new HIS_MEDICAL_ASSESSMENT();
            try
            {
                if (currentMedicalAss != null)
                    obj.ID = currentMedicalAss.ID;
                obj.ASSESSMENT_BOARD_NAME = txtAssName.Text.Trim();
                if (cboAssType.EditValue != null)
                    obj.ASSESSMENT_TYPE_ID = Int64.Parse(cboAssType.EditValue.ToString());
                else
                    obj.ASSESSMENT_TYPE_ID = null;
                obj.ASSESSMENT_TIME_FROM = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteAssFrom.DateTime) ?? 0;
                if (dteAssTo.EditValue != null && dteAssTo.DateTime != DateTime.MinValue)
                    obj.ASSESSMENT_TIME_TO = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteAssTo.DateTime) ?? 0;
                else
                    obj.ASSESSMENT_TIME_TO = null;
                if (dteRequestTime.EditValue != null && dteRequestTime.DateTime != DateTime.MinValue)
                    obj.REQUEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dteRequestTime.DateTime) ?? 0;
                else
                    obj.REQUEST_TIME = null;
                obj.REPORT_NUMBER = txtReportNumber.Text.Trim();
                obj.TREATMENT_ID = treatmentId;
                obj.REQUEST_ORG_CODE = txtRequestOrgCode.Text.Trim();
                obj.REQUEST_ORG_NAME = txtRequestOrgName.Text.Trim();
                obj.REFERRAL_CODE = txtReferalCode.Text.Trim();
                obj.ASSESSMENT_PURPOSE = memAssPurpose.Text.Trim();
                if (cboAssObj.EditValue != null)
                    obj.ASSESSMENT_OBJECT_ID = Int64.Parse(cboAssObj.EditValue.ToString());
                obj.ASSESSMENT_REQUEST_CONTENT = memAssRequestContent.Text.Trim();
                if (!string.IsNullOrEmpty(txtPreviousInJuryRate.Text.Trim()))
                    obj.PREVIOUS_INJURY_RATE = ConvertDecimal(txtPreviousInJuryRate.Text.Trim()) / 100;
                else
                    obj.PREVIOUS_INJURY_RATE = null;
                obj.PATHOLOGICAL_HISTORY = memPathologicalHistory.Text.Trim();
                obj.EXAMINATION_RESULT = memExaminationResult.Text.Trim();
                obj.DISCUSSION = memDiscussion.Text.Trim();
                obj.LEGAL_GROUND_NUMBERS = txtLegalGroundNumbers.Text.Trim();
                obj.LEGAL_GROUND_DOCUMENTS = txtLegalGroundDocuments.Text.Trim();
                obj.CONCLUSION = txtConClusion.Text.Trim();
                if (listWelfareType != null && listWelfareType.Count > 0)
                    obj.WELFARE_TYPE_IDS = string.Join(";", listWelfareType.Select(o => o.ID).ToList());
                else
                    obj.WELFARE_TYPE_IDS = null;
                if (cboDisabilityType.EditValue != null)
                    obj.DISABILITY_TYPE_ID = Int64.Parse(cboDisabilityType.EditValue.ToString());
                else
                    obj.DISABILITY_TYPE_ID = null;
                if (cboDisabilityStatus.EditValue != null)
                    obj.DISABILITY_STATUS_ID = Int64.Parse(cboDisabilityStatus.EditValue.ToString());
                else
                    obj.DISABILITY_STATUS_ID = null;
                if (!string.IsNullOrEmpty(txtInjuryRate.Text.Trim()))
                    obj.INJURY_RATE = ConvertDecimal(txtInjuryRate.Text.Trim()) / 100;
                else
                    obj.INJURY_RATE = null;
                if (!string.IsNullOrEmpty(txtInJuryRateTotal.Text.Trim()))
                    obj.INJURY_RATE_TOTAL = ConvertDecimal(txtInJuryRateTotal.Text.Trim()) / 100;
                else
                    obj.INJURY_RATE_TOTAL = null;
                obj.REQUEST_AFTER_ASSESSMENT = txtRequestAfterAss.Text.Trim();
            }
            catch (Exception ex)
            {
                obj = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return obj;
        }

        private decimal ConvertDecimal(string txt)
        {
            decimal rs = 0;
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                if (txt.Contains(","))
                    culture = new CultureInfo("fr-FR");
                rs = Convert.ToDecimal(txt, culture);
            }
            catch (Exception ex)
            {
                rs = 0;
                Inventec.Common.Logging.LogSystem.Error(txt + "________" + ex);
            }
            return rs;
        }

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

        private void btnDel_Click(object sender, EventArgs e)
        {
            try
            {
                if (vMedicalAss != null)
                {
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var success = new BackendAdapter(param).Post<bool>("api/HisMedicalAssessment/Delete", ApiConsumers.MosConsumer, treatmentId, param);
                    if (success)
                    {
                        currentMedicalAss = null;
                        vMedicalAss = null;
                        listAssMem = null;
                        DefaultForm();
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    DefaultForm();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DefaultForm()
        {
            try
            {
                txtAssName.Text = null;
                cboAssType.EditValue = null;
                txtReportNumber.Text = null;
                dteAssFrom.DateTime = DateTime.Now;
                dteAssTo.EditValue = null;
                cboPresident.EditValue = null;
                cboSecretary.EditValue = null;
                txtRequestOrgCode.Text = null;
                txtRequestOrgName.Text = null;
                txtReferalCode.Text = null;
                dteRequestTime.EditValue = null;
                memAssPurpose.Text = null;
                cboAssObj.EditValue = null;
                memAssRequestContent.Text = null;
                txtPreviousInJuryRate.EditValue = null;
                memPathologicalHistory.Text = null;
                memExaminationResult.Text = null;
                memDiscussion.Text = null;
                txtLegalGroundNumbers.Text = null;
                txtLegalGroundDocuments.Text = null;
                txtConClusion.Text = null;
                cboDisabilityType.EditValue = null;
                cboDisabilityStatus.EditValue = null;
                txtInjuryRate.EditValue = null;
                txtInJuryRateTotal.EditValue = null;
                txtRequestAfterAss.Text = null;
                listWelfareType = new List<HIS_WELFARE_TYPE>();
                GridCheckMarksSelection gridCheckMark = cboWelfareType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboWelfareType.Properties.View);
                }
                cboWelfareType.Focus();
                txtAssName.Focus();
                FillDefaultTable();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPatientCode = new DXMenuItem(ResourceMessage.GiamDinhYKhoa, new EventHandler(btnPrint_Click));
                itemPatientCode.Tag = "Mps000490";
                menu.Items.Add(itemPatientCode);

                DXMenuItem itemProgramCode = new DXMenuItem(ResourceMessage.HopGiamDinhYKhoa, new EventHandler(btnPrint_Click));
                itemProgramCode.Tag = "Mps000491";
                menu.Items.Add(itemProgramCode);

                btnPrint.DropDownControl = menu;
                btnPrint.MenuManager = barManager2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var code = sender as DXMenuItem;
                Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richStore.RunPrintTemplate(code.Tag.ToString(), this.DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000490":
                        LoadBieuMauMps490(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000491":
                        LoadBieuMauMps491(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauMps490(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MPS.Processor.Mps000490.PDO.Mps000490PDO pdo = new MPS.Processor.Mps000490.PDO.Mps000490PDO(vMedicalAss, listAssMem);
                WaitingManager.Hide();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentTreatment.TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                string printerName = "";


                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadBieuMauMps491(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MPS.Processor.Mps000491.PDO.Mps000491PDO pdo = new MPS.Processor.Mps000491.PDO.Mps000491PDO(vMedicalAss, listAssMem);
                WaitingManager.Hide();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentTreatment.TREATMENT_CODE, printTypeCode, this.currentModule.RoomId);

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                string printerName = "";


                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repReasonTab2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis)
                {
                    ButtonEdit btn = sender as ButtonEdit;
                    var data = (GridADO)gridView2.GetFocusedRow();
                    memReason.Text = btn.Text;
                    ButtonEdit editor = sender as ButtonEdit;
                    Rectangle buttonPosition = new Rectangle(editor.Bounds.X, editor.Bounds.Y, editor.Bounds.Width, editor.Bounds.Height);
                    popupContainerControl1.ShowPopup(new Point(buttonPosition.X + 800, buttonPosition.Bottom + 650));
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (GridADO)gridView2.GetFocusedRow();
                gridView2.SetFocusedValue(memReason.Text.Trim());
                gridControl2.RefreshDataSource();
                popupContainerControl1.HidePopup();
                gridView2.FocusedColumn = gridColumn7;
                memReason.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCan_Click(object sender, EventArgs e)
        {
            try
            {
                memReason.Text = null;
                popupContainerControl1.HidePopup();
                gridView2.FocusedColumn = gridColumn7;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repbtnAddTab2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                listDataTable2.Add(new GridADO());
                RefreshDataGrid(gridControl2, listDataTable2);
                gridView2.FocusedRowHandle = gridView2.RowCount - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repbtnMinusTab2_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (GridADO)gridView2.GetFocusedRow();
                listDataTable2.Remove(data);
                RefreshDataGrid(gridControl2, listDataTable2);
                gridView2.FocusedRowHandle = gridView2.RowCount - 1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView1.GetRowCellValue(e.RowHandle, "ActionType") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repbtnAddTab1;
                    }
                    else
                    {
                        e.RepositoryItem = repbtnMinusTab1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView3.GetRowCellValue(e.RowHandle, "ActionType") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repbtnAddTab3;
                    }
                    else
                    {
                        e.RepositoryItem = repbtnMinusTab3;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

            try
            {
                if (e.Column.FieldName == "BtnDelete")
                {
                    int rowSelected = Convert.ToInt32(e.RowHandle);
                    int action = Inventec.Common.TypeConvert.Parse.ToInt32((gridView2.GetRowCellValue(e.RowHandle, "ActionType") ?? "").ToString());
                    if (action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                    {
                        e.RepositoryItem = repbtnAddTab2;
                    }
                    else
                    {
                        e.RepositoryItem = repbtnMinusTab2;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWelfareType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string roomName = "";
                if (this.listWelfareType != null && this.listWelfareType.Count > 0)
                {
                    foreach (var item in this.listWelfareType)
                    {
                        roomName += item.WELFARE_TYPE_NAME + "; ";

                    }
                }
                e.DisplayText = roomName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void gridView2_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridADO data = view.GetFocusedRow() as GridADO;
                if (view.FocusedColumn.FieldName == "LOGINNAME" && view.ActiveEditor is GridLookUpEdit)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    List<string> lstLoginName = new List<string>();
                    ReloadUserGridControl2(ref lstLoginName);
                    ComboAcsUser(editor, lstLoginName);
                    gridView2.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadUserGridControl2(ref List<string> lstLoginName)
        {
            try
            {
                if (listDataTable1 != null && listDataTable1.Count > 0)
                {
                    lstLoginName.AddRange(listDataTable1.Where(o => !string.IsNullOrEmpty(o.LOGINNAME)).Select(o => o.LOGINNAME).ToList());
                }
                if (cboPresident.EditValue != null)
                    lstLoginName.Add(cboPresident.EditValue.ToString());
                if (cboSecretary.EditValue != null)
                    lstLoginName.Add(cboSecretary.EditValue.ToString());
                lstLoginName = lstLoginName.Distinct().ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ComboAcsUser(GridLookUpEdit cbo, List<string> loginNames)
        {
            try
            {
                List<ACS_USER> acsUserAlows = new List<ACS_USER>();
                if (loginNames != null && loginNames.Count > 0)
                {
                    acsUserAlows = this.listAcsUser.Where(o => loginNames.Contains(o.LOGINNAME)).ToList();
                }

                cbo.Properties.DataSource = acsUserAlows;
                cbo.Properties.DisplayMember = "USERNAME";
                cbo.Properties.ValueMember = "LOGINNAME";

                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("LOGINNAME");
                aColumnCode.Caption = "Tài khoản";
                aColumnCode.OptionsColumn.ShowCaption = false;
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 100;

                GridColumn aColumnName = cbo.Properties.View.Columns.AddField("USERNAME");
                aColumnName.Caption = "Họ tên";
                aColumnName.OptionsColumn.ShowCaption = false;
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 200;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    GridADO pData = (GridADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        pData.ID = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            try
            {
                btnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var data = (GridADO)gridView1.GetFocusedRow();

                if (e.Column.FieldName == "LOGINNAME")
                {
                    if (!string.IsNullOrEmpty(data.LOGINNAME))
                        data.USERNAME = listAcsUser.FirstOrDefault(o => o.LOGINNAME.Equals(data.LOGINNAME)).USERNAME;
                    else
                        data.USERNAME = null;
                    this.gridControl1.RefreshDataSource();
                }
                if (e.Column.FieldName == "LOGINNAME" || e.Column.FieldName == "BtnDelete")
                {
                    List<string> lstLoginName = new List<string>();
                    ReloadUserGridControl2(ref lstLoginName);
                    listDataTable2 = listDataTable2.Where(o => lstLoginName.Exists(p => p.Equals(o.LOGINNAME))).ToList();
                    if (listDataTable2 == null || listDataTable2.Count == 0)
                        listDataTable2 = new List<GridADO>() { new GridADO() };
                    RefreshDataGrid(gridControl2, listDataTable2);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var data = (GridADO)gridView3.GetFocusedRow();

                if (e.Column.FieldName == "LOGINNAME")
                {
                    if (!string.IsNullOrEmpty(data.LOGINNAME))
                        data.USERNAME = listAcsUser.FirstOrDefault(o => o.LOGINNAME.Equals(data.LOGINNAME)).USERNAME;
                    else
                        data.USERNAME = null;
                    this.gridControl3.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var data = (GridADO)gridView2.GetFocusedRow();

                if (e.Column.FieldName == "LOGINNAME")
                {
                    if (!string.IsNullOrEmpty(data.LOGINNAME))
                        data.USERNAME = listAcsUser.FirstOrDefault(o => o.LOGINNAME.Equals(data.LOGINNAME)).USERNAME;
                    else
                        data.USERNAME = null;
                    this.gridControl2.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPresident_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                List<string> lstLoginName = new List<string>();
                ReloadUserGridControl2(ref lstLoginName);
                listDataTable2 = listDataTable2.Where(o => lstLoginName.Exists(p => p.Equals(o.LOGINNAME))).ToList();
                if (listDataTable2 == null || listDataTable2.Count == 0)
                    listDataTable2 = new List<GridADO>() { new GridADO() };
                RefreshDataGrid(gridControl2, listDataTable2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSecretary_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                List<string> lstLoginName = new List<string>();
                ReloadUserGridControl2(ref lstLoginName);
                listDataTable2 = listDataTable2.Where(o => lstLoginName.Exists(p => p.Equals(o.LOGINNAME))).ToList();
                if (listDataTable2 == null || listDataTable2.Count == 0)
                    listDataTable2 = new List<GridADO>() { new GridADO() };
                RefreshDataGrid(gridControl2, listDataTable2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPreviousInJuryRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !e.KeyChar.Equals(',') && !e.KeyChar.Equals('.'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInjuryRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !e.KeyChar.Equals(',') && !e.KeyChar.Equals('.'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtInJuryRateTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !e.KeyChar.Equals(',') && !e.KeyChar.Equals('.'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
