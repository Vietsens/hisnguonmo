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
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisService.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisService
{
    public partial class UC_HisService : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_ICD_CM> listIcdCm;
        List<HIS_PTTT_GROUP> listPtttGroup;
        List<HIS_RATION_GROUP> listRatioGroup;
        List<HIS_FILM_SIZE> listFILM_SIZE;
        List<HIS_PTTT_METHOD> listPtttMethod;
        List<HIS_PACKAGE> listPackage;
        List<HIS_PATIENT_TYPE> listPatientType;
        List<HIS_SERVICE_TYPE> listServiceType;
        List<HIS_SERVICE_UNIT> listServiceUnit;
        List<HIS_HEIN_SERVICE_TYPE> listHeinServiceType;
        List<HIS_BODY_PART> listBodyPart;
        List<HIS_RATION_TIME> RationTimeSelecteds;
        List<HIS_RATION_TIME> DataRationTime;
        List<HIS_RATION_TIME> RationTimeSelectedsEdit;
        List<HIS_EXE_SERVICE_MODULE> listExeServiceModules;
        List<HIS_SUIM_INDEX> listHisSuimIndex;
        List<HIS_SUIM_INDEX> listHisSuimIndexChoose = new List<HIS_SUIM_INDEX>();
        List<HIS_SERVICE> hisServiceRef;
        V_HIS_SERVICE currentRightClick;
        List<V_HIS_SERVICE> listVServiceExport = new List<V_HIS_SERVICE>();
        List<V_HIS_SERVICE_PATY> listVServicePatyExport = new List<V_HIS_SERVICE_PATY>();
        List<V_HIS_SERVICE> listParentService;
        List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE> ListPrintType;
        private System.Windows.Forms.SaveFileDialog SaveFileExportExcel = new SaveFileDialog();
        #endregion

        #region Construct
        public UC_HisService(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            try
            {
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private List<long> typeIdNotShowPtttInfos = new List<long>()
            {
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT,
                IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
            };

        #region Private method
        private void UC_HisService_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefaultControlsProperties();
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisService.Resources.Lang", typeof(HIS.Desktop.Plugins.HisService.UC_HisService).Assembly);
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisService.Resources.Lang", typeof(HIS.Desktop.Plugins.HisService.UC_HisService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCapacity.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcCapacity.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem69.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem69.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbCapacity.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lbCapacity.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbCapacity.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lbCapacity.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSearchType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboSearchType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn34.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn33.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn32.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn28.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn31.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn31.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn31.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn30.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn30.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn30.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn29.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn29.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn29.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn20.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn27.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn26.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn25.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn24.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn23.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn16.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn17.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.treeListColumn41.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn41.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn41.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn41.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_HisService.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPackage.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboPackage.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChiDinh.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkChiDinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsKidney.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsKidney.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAntibioticResistance.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsAntibioticResistance.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowExpend.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkAllowExpend.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateOnly.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkUpdateOnly.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateAll.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkUpdateAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcdCm.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboIcdCm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPNgoaiGoi.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkCPNgoaiGoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBillOption.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboBillOption.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRationGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboRationGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTaxRateType.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciTaxRateType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("UC_HisService.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeCode.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spCogss.Text = Inventec.Common.Resource.Get.Value("UC_HisService.spCogss.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcFuexType.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcFuexType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcFuexType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lcFuexType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spEstimateDurations.Text = Inventec.Common.Resource.Get.Value("UC_HisService.spEstimateDurations.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeName.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLock.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLock.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lciLock.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem61.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem61.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboTestType.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.CboTestType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.Text = Inventec.Common.Resource.Get.Value("frmHisService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.dtInTime.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.dtInTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsOtherSourcePaid.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsOtherSourcePaid.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBlockDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsBlockDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.chkTachdichvu.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkTachdichvu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcDiimType.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcDiimType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.lcDiimType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lcDiimType.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem55.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem55.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtNotice.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.txtNotice.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                this.layoutControlItem51.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem51.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtProcessCode.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.txtProcessCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                //Chạy tool sinh resource key language sinh tự động đa ngôn ngữ -> copy vào đây
                //TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UC_HisService
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisService.Resources.Lang", typeof(UC_HisService).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkLock.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkLock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.BtnExportExcel.Text = Inventec.Common.Resource.Get.Value("UC_HisService.BtnExportExcel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboSearchType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboSearchType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn34.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn33.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn32.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn38.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn38.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn28.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn28.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn31.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn31.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn31.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn30.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn30.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn30.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn29.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn29.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn29.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn20.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn27.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn27.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn26.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn26.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn25.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn25.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn24.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn24.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn23.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn23.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn16.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn42.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn42.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn42.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn42.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn17.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn40.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn40.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn39.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn39.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn41.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn41.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn41.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn41.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn35.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn36.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn37.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn37.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn43.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.treeListColumn43.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_HisService.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBlockDepartment.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsBlockDepartment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBlockDepartment.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsBlockDepartment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowSimultaneity.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkAllowSimultaneity.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowSimultaneity.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkAllowSimultaneity.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIS_AUTO_EXPEND.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIS_AUTO_EXPEND.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIS_AUTO_EXPEND.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkIS_AUTO_EXPEND.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFILM_SIZE.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboFILM_SIZE.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFILM_SIZE.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.cboFILM_SIZE.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIS_NOT_SHOW_TRACKING.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIS_NOT_SHOW_TRACKING.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsDisAllowanceNoExecute.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsDisAllowanceNoExecute.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsDisAllowanceNoExecute.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsDisAllowanceNoExecute.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAttachAssignPrintTypeCode.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboAttachAssignPrintTypeCode.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChiSo.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboChiSo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIS_OUT_OF_MANAGEMENT.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIS_OUT_OF_MANAGEMENT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMUST_BE_CONSULTED.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkMUST_BE_CONSULTED.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNgoaiDinhSuat.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkNgoaiDinhSuat.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboOTHER_PAY_SOURCE.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboOTHER_PAY_SOURCE.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAntibioticResistance.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsAntibioticResistance.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsOtherSourcePaid.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsOtherSourcePaid.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTachdichvu.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkTachdichvu.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkTachdichvu.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkTachdichvu.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotChangePaty.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkNotChangePaty.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotChangePaty.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkNotChangePaty.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.CboTestType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.CboTestType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkForPrice.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkForPrice.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkForPrice.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkForPrice.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSplitTreatment.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkSplitTreatment.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSplitTreatment.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.chkSplitTreatment.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboTaxRateType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboTaxRateType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRationGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboRationGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbCapacity.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lbCapacity.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lbCapacity.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lbCapacity.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsKidney.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsKidney.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEditServiceType.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.btnEditServiceType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAllowExpend.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkAllowExpend.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGender.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboGender.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExeServiceModule.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboExeServiceModule.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnMayCuaToi.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnMayCuaToi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.simpleButton2.Text = Inventec.Common.Resource.Get.Value("UC_HisService.simpleButton2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPackage.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboPackage.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkChiDinh.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkChiDinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateOnly.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkUpdateOnly.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateAll.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkUpdateAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcdCm.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboIcdCm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dtInTime.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.dtInTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPNgoaiGoi.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.chkCPNgoaiGoi.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMethod.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboMethod.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBillOption.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboBillOption.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("UC_HisService.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("UC_HisService.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDiimType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboDiimType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFuexType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboFuexType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPart.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboPart.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboAppliedPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboAppliedPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientClassify.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboPatientClassify.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDefaultPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_HisService.cboDefaultPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeCode.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem17.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem19.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem24.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem32.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem32.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem33.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem1.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem18.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem35.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTypeName.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem37.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem37.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem55.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem55.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spCogss.Text = Inventec.Common.Resource.Get.Value("UC_HisService.spCogss.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem39.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem39.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem49.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem49.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem26.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem29.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem48.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem48.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem24.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTaxRateType.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciTaxRateType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spEstimateDurations.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.spEstimateDurations.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.spEstimateDurations.Text = Inventec.Common.Resource.Get.Value("UC_HisService.spEstimateDurations.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem5.Text = Inventec.Common.Resource.Get.Value("UC_HisService.emptySpaceItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem50.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem50.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem47.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem47.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem54.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem54.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem30.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem46.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem46.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem2.Text = Inventec.Common.Resource.Get.Value("UC_HisService.emptySpaceItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem51.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem51.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem61.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem61.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem61.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem61.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem44.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem44.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAntibioticResistanc_Lay.Text = Inventec.Common.Resource.Get.Value("UC_HisService.chkIsAntibioticResistanc_Lay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem59.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem59.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem59.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem59.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem64.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem64.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem65.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem65.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem66.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem66.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem66.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem66.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem68.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem68.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAttachAssignPrintTypeCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lciAttachAssignPrintTypeCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAttachAssignPrintTypeCode.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciAttachAssignPrintTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem45.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem45.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem60.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem60.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem60.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem60.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFILM_SIZE.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lcFILM_SIZE.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFILM_SIZE.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcFILM_SIZE.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSpMinProcess.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lciSpMinProcess.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSpMinProcess.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciSpMinProcess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem1.Text = Inventec.Common.Resource.Get.Value("UC_HisService.emptySpaceItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem71.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem71.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem71.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem71.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem4.Text = Inventec.Common.Resource.Get.Value("UC_HisService.emptySpaceItem4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem38.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem38.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItem6.Text = Inventec.Common.Resource.Get.Value("UC_HisService.emptySpaceItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcCapacity.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lcCapacity.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem69.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem69.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem63.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem63.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem52.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem52.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem73.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem73.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem67.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem67.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem67.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem67.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem74.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem74.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem74.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem74.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem75.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem75.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem75.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem75.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem76.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem76.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem76.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem76.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem77.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem77.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem78.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem78.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem78.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem78.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem56.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem56.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem58.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem58.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem58.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem42.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem79.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem79.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem27.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem27.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("UC_HisService.layoutControlItem31.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLock.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lciLock.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLock.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaxTotalProcessTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.lciMaxTotalProcessTime.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaxTotalProcessTime.Text = Inventec.Common.Resource.Get.Value("UC_HisService.lciMaxTotalProcessTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.emptySpaceItemMaxTotalProcessTime.Text = Inventec.Common.Resource.Get.Value("UC_HisService.emptySpaceItemMaxTotalProcessTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                LockT.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                unLockT.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                DeleteD.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                DeleteE.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_ChinhSachGia_Enable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_ChinhSachGia_Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_BaoCao_Enable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_BaoCao_Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_PhongXuLy_Enable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_PhongXuLy_Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_ServiceRaty_Enable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                btn_ServiceRaty_Disable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("UC_HisService.tol12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                currentData = null;
                this.ActionType = GlobalVariables.ActionAdd;
                txtNotice.EditValue = null;
                txtDescription.EditValue = null;
                txtMisuSerTypeCode.EditValue = null;
                txtMisuSerTypeName.EditValue = null;
                txtRatioSymbol.EditValue = null;
                txtMisuSerTypeCode.Focus();
                txtTestingTechnique.EditValue = null;
                spCogs.EditValue = null;
                spEstimateDuration.EditValue = null;
                spNumOrder.EditValue = null;
                spMaxExpend.EditValue = null;
                spMinProcess.EditValue = null;
                spnMaxProcessTime.EditValue = null;
                spMaxTotalProcessTime.EditValue = null;
                cboDVT.EditValue = null;
                cboType.EditValue = null;
                cboRationGroup.EditValue = null;
                cboTaxRateType.EditValue = null;
                cboParent.EditValue = null;
                cboBillOption.EditValue = null;
                cboBillOption.EditValue = null;
                cboPatientType.EditValue = null;
                cboDefaultPatientType.EditValue = null;
                cboPart.EditValue = null;
                cboChiSo.EditValue = null;
                listHisSuimIndexChoose = new List<HIS_SUIM_INDEX>();
                cboAppliedPatientType.EditValue = null;
                cboPatientClassify.EditValue = null;
                cboPatientClassify.Enabled = false;
                cboDTTT1.EditValue = null;
                cboDTTT2.EditValue = null;
                cboDTTT3.EditValue = null;
                cboSampleType.EditValue = null;
                chkNotChangePaty.Checked = false;
                cboGroup.EditValue = null;
                cboIcdCm.EditValue = null;
                txtIcdCm.Text = "";
                cboMethod.EditValue = null;
                cboServiceType.EditValue = null;
                txtBHYTCode.EditValue = null;
                txtBHYTName.EditValue = null;
                txtBHYTStt.EditValue = null;
                txtPacsType.EditValue = null;
                spMinDuration.EditValue = null;
                spinWARNING.EditValue = null;
                spHeinNew.EditValue = null;
                spHeinOld.EditValue = null;
                dtInTime.EditValue = null;
                chkIsDisAllowanceNoExecute.EditValue = null;
                chkCPNgoaiGoi.EditValue = null;
                chkChiDinh.EditValue = null;
                chkIsKidney.EditValue = null;
                spinMaxAmount.EditValue = null;
                chkIsAntibioticResistance.EditValue = null;
                chkIsOtherSourcePaid.EditValue = null;
                chkIsBlockDepartment.EditValue = null;
                chkAllowSimultaneity.EditValue = null;
                chkTachdichvu.EditValue = null;
                chkAllowExpend.EditValue = true;
                chkMUST_BE_CONSULTED.EditValue = null;
                chkNOT_REQUIRED_COMPLETE.EditValue = null;
                chkNOT_REQUIRED_COMPLETE.Checked = false;
                chkIS_OUT_OF_MANAGEMENT.EditValue = null;
                chkNotUseBHYT.EditValue = null;
                chkNotUseBHYT.Checked = false;
                chkIS_NOT_SHOW_TRACKING.EditValue = null;
                chkIS_NOT_SHOW_TRACKING.Checked = false;
                chkUpdateAll.EditValue = null;
                chkUpdateOnly.EditValue = null;
                chkAllowSendPacs.EditValue = null;
                chkAllowSendPacs.Checked = false;
                SetEnablePTTT(true, true);
                txtSpecialityCode.EditValue = null;
                txtSpecialityCode.Enabled = true;
                cboPackage.EditValue = null;
                cboPackage.Enabled = true;
                txtPackagePrice.EditValue = null;
                txtPackagePrice.Enabled = false;
                txtPackagePrice.Validating += null;
                cboGender.EditValue = null;
                cboExeServiceModule.EditValue = null;
                spAgeFrom.EditValue = null;
                txtCapacity.EditValue = null;
                spAgeTo.EditValue = null;
                chkLock.CheckState = CheckState.Checked;
                dxErrorProvider.ClearErrors();
                layoutControlItem35.AppearanceItemCaption.ForeColor = Color.Transparent;
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                EnableControlChanged(this.ActionType);
                cboDiimType.EditValue = null;
                cboDiimType.Enabled = false;
                cboFuexType.Enabled = false;
                cboFuexType.EditValue = null;
                chkSplitTreatment.EditValue = null;
                chkSplitTreatment.Checked = false;
                chkIS_AUTO_EXPEND.EditValue = null;
                chkIS_AUTO_EXPEND.Checked = false;
                chkIS_AUTO_EXPEND.Enabled = false;
                chkTachdichvu.Enabled = false;
                chkTachdichvu.EditValue = null;
                CboTestType.EditValue = null;

                CboTestType.Enabled = false;
                txtProcessCode.EditValue = null;
                cboOTHER_PAY_SOURCE.EditValue = null;
                txtOTHER_PAY_SOURCE.Enabled = false;
                txtOTHER_PAY_SOURCE.Text = "";
                chkNgoaiDinhSuat.EditValue = null;
                GridCheckMarksSelection gridCheckChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                gridCheckChiSo.ClearSelection(cboChiSo.Properties.View);
                cboChiSo.Text = "";

                GridCheckMarksSelection gridCheckMarkPart = cboPart.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkPart.ClearSelection(cboPart.Properties.View);
                cboPart.Text = "";

                GridCheckMarksSelection gridCheckMarkPatientType = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkPatientType.ClearSelection(cboAppliedPatientType.Properties.View);
                cboAppliedPatientType.Text = "";

                GridCheckMarksSelection gridCheckMarkPatientClassify = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkPatientClassify.ClearSelection(cboPatientClassify.Properties.View);
                cboPatientClassify.Text = "";

                GridCheckMarksSelection gridCheckDTTT1 = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                gridCheckDTTT1.ClearSelection(cboDTTT1.Properties.View);
                cboDTTT1.Text = "";

                GridCheckMarksSelection gridCheckDTTT2 = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                gridCheckDTTT2.ClearSelection(cboDTTT2.Properties.View);
                cboDTTT2.Text = "";

                GridCheckMarksSelection gridCheckDTTT3 = cboDTTT3.Properties.Tag as GridCheckMarksSelection;
                gridCheckDTTT3.ClearSelection(cboDTTT3.Properties.View);
                cboDTTT3.Text = "";
                GridCheckMarksSelection gridCheckPhieuBA = cboPhieuBA.Properties.Tag as GridCheckMarksSelection;
                gridCheckPhieuBA.ClearSelection(cboPhieuBA.Properties.View);
                cboPhieuBA.Text = "";

                cboAttachAssignPrintTypeCode.EditValue = null;
                cboFILM_SIZE.EditValue = null;
                cboFILM_SIZE.Enabled = false;
                cboAppliedPatientType.Enabled = false;
                cboPatientClassify.Enabled = false;
                //cboDTTT1.Enabled = false;
                //cboDTTT2.Enabled = false;
                //cboDTTT3.Enabled = false;
                cboSampleType.Enabled = false;
                cboPetroleum.EditValue = null;
                cboPetroleum.Enabled = false;



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
                cboSearchType.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void SetDefaultControlsProperties()
        {
            try
            {
                spMinProcess.Properties.MaxValue = Decimal.MaxValue;
                spnMaxProcessTime.Properties.MaxValue = Decimal.MaxValue;
                spMaxTotalProcessTime.Properties.MaxValue = Decimal.MaxValue;
                spinWARNING.Properties.MaxValue = Decimal.MaxValue;
                spinMaxAmount.Properties.MaxValue = Decimal.MaxValue;
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
                InitComboHeinServiceTypeId();
                InitComboSearchServiceType();
                InitComboServiceUnit();
                InitComboIcdCm();
                InitComboServiceType();
                InitComboPtttGroup();
                InitComboRatioGroup();
                InitComboFILM_SIZE();
                InitComboTaxRateType();
                LoadComboStatus();
                LoadComboPatientType();
                LoadComboPatientClassify();
                LoadComboDefaultPatientType();
                InitComboGender();
                InitComboMethod();
                InitComboPackage();
                InitComboExeServiceModule();
                InitComboParentID();
                InitComboDiimType();
                InitComboFuexType();
                InitComboTestType();
                InitComboSampleType();
                InitComboOtherPay();
                InitComboPart();
                InitComboPatientType();
                InitComboChiSo();
                InitComboPrintType();
                InitComboDTTT1();
                InitComboDTTT2();
                InitComboDTTT3();
                InitComboPetroleum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void InitComboSampleType()
        {
            try
            {

                string lisIntegrationVersion = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.LIS.INTEGRATION_VERSION");
                string lisIntegrationOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.LIS.INTEGRATE_OPTION");
                string lisIntegrationType = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.LIS.INTEGRATION_TYPE");
                if ((lisIntegrationVersion == "1" && lisIntegrationOption == "1") || (lisIntegrationVersion == "2" && lisIntegrationType == "1"))
                {

                    List<ColumnInfo> column = new List<ColumnInfo>();
                    column.Add(new ColumnInfo("SAMPLE_TYPE_CODE", "", 100, 1));
                    column.Add(new ColumnInfo("SAMPLE_TYPE_NAME", "", 250, 2));
                    ControlEditorADO controlEditADO = new ControlEditorADO("SAMPLE_TYPE_CODE", "SAMPLE_TYPE_CODE", column, false, 350);
                    ControlEditorLoader.Load(cboSampleType, BackendDataWorker.Get<LIS_SAMPLE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditADO);
                }
                else if ((lisIntegrationVersion != "1" || lisIntegrationOption != "1") && (lisIntegrationVersion != "2" || lisIntegrationType != "1"))
                {
                    List<ColumnInfo> column = new List<ColumnInfo>();
                    column.Add(new ColumnInfo("TEST_SAMPLE_TYPE_CODE", "", 100, 1));
                    column.Add(new ColumnInfo("TEST_SAMPLE_TYPE_NAME", "", 250, 2));
                    ControlEditorADO controlEditADO = new ControlEditorADO("TEST_SAMPLE_TYPE_CODE", "TEST_SAMPLE_TYPE_CODE", column, false, 350);
                    ControlEditorLoader.Load(cboSampleType, BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region Init combo
        private void InitComboHeinServiceTypeId()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisHeinServiceTypeFilter filter = new HisHeinServiceTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_HEIN_SERVICE_TYPE>>("api/HisHeinServiceType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                if (data != null && data.Count > 0)
                {
                    //11471
                    data = data.Where(o =>
                        o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_NDM
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TDM
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_TL
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TH_UT
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_NDM
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TDM
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TL
                        && o.ID != IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT
                        ).ToList();
                }
                listHeinServiceType = new List<HIS_HEIN_SERVICE_TYPE>();
                listHeinServiceType = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("HEIN_SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("HEIN_SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPart()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPart.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboPart);
                cboPart.Properties.Tag = gridCheck;
                cboPart.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisBodyPartFilter filter = new HisBodyPartFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_BODY_PART>>("api/HisBodyPart/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listBodyPart = new List<HIS_BODY_PART>();
                listBodyPart = data;
                if (listBodyPart != null)
                {
                    cboPart.Properties.DataSource = listBodyPart;
                    cboPart.Properties.DisplayMember = "BODY_PART_NAME";
                    cboPart.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboPart.Properties.View.Columns.AddField("BODY_PART_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboPart.Properties.View.Columns.AddField("BODY_PART_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "";

                    cboPart.Properties.PopupFormWidth = 200;
                    cboPart.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboPart.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboPart.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboPart.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboPart(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_BODY_PART> erSelectedNews = new List<HIS_BODY_PART>();
                    foreach (HIS_BODY_PART er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.BODY_PART_NAME + ",";
                    }
                    cboPart.Text = typeName;
                    cboPart.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPatientType()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboAppliedPatientType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboAppliedPatientType);
                cboAppliedPatientType.Properties.Tag = gridCheck;
                cboAppliedPatientType.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                cboAppliedPatientType.Properties.DataSource = data;
                cboAppliedPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboAppliedPatientType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboAppliedPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboAppliedPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboAppliedPatientType.Properties.PopupFormWidth = 200;
                cboAppliedPatientType.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboAppliedPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboAppliedPatientType.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadComboPatientClassify()
        {
            try
            {
                GridCheckMarksSelection gridCheckMarkClassify = new GridCheckMarksSelection(cboPatientClassify.Properties);
                gridCheckMarkClassify.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboPatientClassify);
                cboPatientClassify.Properties.Tag = gridCheckMarkClassify;
                cboPatientClassify.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisPatientClassifyFilter filter = new HisPatientClassifyFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_CLASSIFY>>("api/HisPatientClassify/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                cboPatientClassify.Properties.DataSource = data;
                cboPatientClassify.Properties.DisplayMember = "PATIENT_CLASSIFY_NAME";
                cboPatientClassify.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboPatientClassify.Properties.View.Columns.AddField("PATIENT_CLASSIFY_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboPatientClassify.Properties.View.Columns.AddField("PATIENT_CLASSIFY_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboPatientClassify.Properties.PopupFormWidth = 200;
                cboPatientClassify.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboPatientClassify.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPatientClassify.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void Event_Check(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> erSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_TYPE_NAME);
                        }
                    }
                }
                this.cboDTTT1.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check1(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> erSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_TYPE_NAME);
                        }
                    }
                }
                this.cboDTTT2.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Event_Check2(object sender, EventArgs e)
        {

            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> erSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_TYPE_NAME);
                        }
                    }
                }
                this.cboAppliedPatientType.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDTTT1()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDTTT1.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboDTTT1);
                cboDTTT1.Properties.Tag = gridCheck;
                cboDTTT1.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                cboDTTT1.Properties.DataSource = data;
                cboDTTT1.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboDTTT1.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboDTTT1.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboDTTT1.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboDTTT1.Properties.PopupFormWidth = 200;
                cboDTTT1.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboDTTT1.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDTTT1.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDTTT2()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDTTT2.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboDTTT2);
                cboDTTT2.Properties.Tag = gridCheck;
                cboDTTT2.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                cboDTTT2.Properties.DataSource = data;
                cboDTTT2.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboDTTT2.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboDTTT2.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboDTTT2.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboDTTT2.Properties.PopupFormWidth = 200;
                cboDTTT2.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboDTTT2.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDTTT2.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDTTT3()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDTTT3.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboDTTT3);
                cboDTTT3.Properties.Tag = gridCheck;
                cboDTTT3.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisPatientTypeFilter filter = new HisPatientTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                cboDTTT3.Properties.DataSource = data;
                cboDTTT3.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboDTTT3.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboDTTT3.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboDTTT3.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboDTTT3.Properties.PopupFormWidth = 200;
                cboDTTT3.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboDTTT3.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDTTT3.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDTTT3.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboAppliedPatientType(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.PATIENT_TYPE_NAME + ",";
                    }
                    cboAppliedPatientType.Text = typeName;
                    cboAppliedPatientType.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboPatientClassify(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_PATIENT_CLASSIFY er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.PATIENT_CLASSIFY_NAME + ",";
                    }
                    cboPatientClassify.Text = typeName;
                    cboPatientClassify.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboDTTT1(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.PATIENT_TYPE_NAME + ",";
                    }
                    cboDTTT1.Text = typeName;
                    cboDTTT1.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboDTTT2(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.PATIENT_TYPE_NAME + ",";
                    }
                    cboDTTT2.Text = typeName;
                    cboDTTT2.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboDTTT3(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.PATIENT_TYPE_NAME + ",";
                    }
                    cboDTTT3.Text = typeName;
                    cboDTTT3.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboServiceType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).OrderByDescending(o => o.MODIFY_TIME).ToList();
                listServiceType = new List<HIS_SERVICE_TYPE>();
                listServiceType = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboServiceType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTaxRateType()
        {
            try
            {
                List<TaxRateTypeADO> taxRateTypeList = new List<TaxRateTypeADO>();
                TaxRateTypeADO tax1 = new TaxRateTypeADO(1, "0%");
                taxRateTypeList.Add(tax1);
                TaxRateTypeADO tax2 = new TaxRateTypeADO(2, "5%");
                taxRateTypeList.Add(tax2);
                TaxRateTypeADO tax3 = new TaxRateTypeADO(3, "10%");
                taxRateTypeList.Add(tax3);
                TaxRateTypeADO tax4 = new TaxRateTypeADO(4, "Không chịu thuế");
                taxRateTypeList.Add(tax4);
                TaxRateTypeADO tax5 = new TaxRateTypeADO(5, "Không kê khai thuế");
                taxRateTypeList.Add(tax5);
                TaxRateTypeADO tax6 = new TaxRateTypeADO(6, "Khác");
                taxRateTypeList.Add(tax6);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTaxRateType, taxRateTypeList, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSearchServiceType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_TYPE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboSearchType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboIcdCm()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_ICD_CM>().Where(o => o.IS_ACTIVE == 1).ToList();
                listIcdCm = new List<HIS_ICD_CM>();
                listIcdCm = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ICD_CM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ICD_CM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_CM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboIcdCm, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPtttGroup()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_PTTT_GROUP>().Where(o => o.IS_ACTIVE == 1).ToList();
                listPtttGroup = new List<HIS_PTTT_GROUP>();
                listPtttGroup = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PTTT_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboGroup, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRatioGroup()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_RATION_GROUP>().Where(o => o.IS_ACTIVE == 1).ToList();
                listRatioGroup = new List<HIS_RATION_GROUP>();
                listRatioGroup = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("RATION_GROUP_CODE", "", 2, 1));
                columnInfos.Add(new ColumnInfo("RATION_GROUP_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RATION_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRationGroup, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboFILM_SIZE()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_FILM_SIZE>().Where(o => o.IS_ACTIVE == 1).ToList();
                listFILM_SIZE = new List<HIS_FILM_SIZE>();
                listFILM_SIZE = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("FILM_SIZE_CODE", "", 2, 1));
                columnInfos.Add(new ColumnInfo("FILM_SIZE_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("FILM_SIZE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboFILM_SIZE, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboServiceUnit()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceUnitFilter filter = new HisServiceUnitFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_UNIT>>("api/HisServiceUnit/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listServiceUnit = new List<HIS_SERVICE_UNIT>();
                listServiceUnit = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_UNIT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_UNIT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDVT, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboExeServiceModule()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExeServiceModuleFilter filter = new HisExeServiceModuleFilter();
                filter.IS_ACTIVE = 1;
                listExeServiceModules = new BackendAdapter(param).Get<List<HIS_EXE_SERVICE_MODULE>>("api/HisExeServiceModule/Get", ApiConsumers.MosConsumer, filter, null).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXE_SERVICE_MODULE_NAME", "", 250, 1, true));
                columnInfos.Add(new ColumnInfo("MODULE_LINK", "", 250, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MODULE_LINK", "ID", columnInfos, false, 500);
                ControlEditorLoader.Load(cboExeServiceModule, listExeServiceModules, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboChiSo()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboChiSo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboChiSo);
                cboChiSo.Properties.Tag = gridCheck;
                cboChiSo.Properties.View.OptionsSelection.MultiSelect = true;

                listHisSuimIndex = new List<HIS_SUIM_INDEX>();

                CommonParam param = new CommonParam();
                //HisSuimSetySuinFilter SetyFilter = new HisSuimSetySuinFilter();
                //var listHisSuimSety = new BackendAdapter(param).Get<List<HIS_SUIM_SETY_SUIN>>("api/HisSuimSetySuin/Get", ApiConsumers.MosConsumer, SetyFilter, null).ToList();
                //if (listHisSuimSety != null && listHisSuimSety.Count > 0)
                //{
                HisSuimIndexFilter filter = new HisSuimIndexFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //filter.IDs = listHisSuimSety.Select(o => o.SUIM_INDEX_ID).ToList();
                listHisSuimIndex = new BackendAdapter(param).Get<List<HIS_SUIM_INDEX>>("api/HisSuimIndex/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                //}
                cboChiSo.Properties.DataSource = listHisSuimIndex;
                cboChiSo.Properties.DisplayMember = "SUIM_INDEX_NAME";
                cboChiSo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboChiSo.Properties.View.Columns.AddField("SUIM_INDEX_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "Mã";

                DevExpress.XtraGrid.Columns.GridColumn col3 = cboChiSo.Properties.View.Columns.AddField("SUIM_INDEX_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 300;
                col3.Caption = "Tên";

                cboChiSo.Properties.PopupFormWidth = 400;
                cboChiSo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cboChiSo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboChiSo.Properties.View);
                }
                //}
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("SUIM_INDEX_CODE", "", 150, 1, true));
                //columnInfos.Add(new ColumnInfo("SUIM_INDEX_NAME", "", 250, 2, true));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("SUIM_INDEX_NAME", "ID", columnInfos, false, 400);
                //ControlEditorLoader.Load(cboChiSo, listHisSuimIndex, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SelectionGrid__cboChiSo(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    List<HIS_SUIM_INDEX> erSelectedNews = new List<HIS_SUIM_INDEX>();
                    foreach (HIS_SUIM_INDEX er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.SUIM_INDEX_NAME + ",";
                    }
                    cboChiSo.Text = typeName;
                    cboChiSo.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitComboMethod()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPtttMethodFilter filter = new HisPtttMethodFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_PTTT_METHOD>>("api/HisPtttMethod/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listPtttMethod = new List<HIS_PTTT_METHOD>();
                listPtttMethod = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PTTT_METHOD_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PTTT_METHOD_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboMethod, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPackage()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisPackageFilter filter = new HisPackageFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.IS_NOT_FIXED_SERVICE = true;
                var data = new BackendAdapter(param).Get<List<HIS_PACKAGE>>("api/HisPackage/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                listPackage = new List<HIS_PACKAGE>();
                listPackage = data;

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PACKAGE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PACKAGE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PACKAGE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPackage, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBillOption()
        {
            try
            {
                List<BillOption> billOption = new List<BillOption>();
                billOption.Add(new BillOption(1, "Hóa đơn thường"));
                billOption.Add(new BillOption(2, "Hóa đơn dịch vụ"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("billName", "", 235, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("billName", "id", columnInfos, false, 235);
                ControlEditorLoader.Load(cboBillOption, billOption, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadComboPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == (short)1 && o.IS_ADDITION == (short)1).ToList();
                listPatientType = new List<HIS_PATIENT_TYPE>();
                listPatientType = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 230, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboDefaultPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                listPatientType = new List<HIS_PATIENT_TYPE>();
                listPatientType = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 230, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboDefaultPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboStatus()
        {
            try
            {
                List<Status> status = new List<Status>();
                status.Add(new Status(1, "Hóa đơn thường"));
                status.Add(new Status(2, "Tách chênh lệch vào hóa đơn dịch vụ"));
                status.Add(new Status(3, "Hóa đơn dịch vụ"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBillOption, status, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDiimType()
        {
            try
            {
                BackendDataWorker.Reset<HIS_DIIM_TYPE>();
                var lsDiimType = BackendDataWorker.Get<HIS_DIIM_TYPE>().ToList();
                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("DIIM_TYPE_CODE", "", 100, 1));
                colum.Add(new ColumnInfo("DIIM_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditADO = new ControlEditorADO("DIIM_TYPE_NAME", "ID", colum, false, 350);
                ControlEditorLoader.Load(cboDiimType, lsDiimType, controlEditADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboTestType()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisTestTypeFilter filter = new HisTestTypeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var lsTestType = new BackendAdapter(param).Get<List<HIS_TEST_TYPE>>("api/HisTestType/Get", ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("TEST_TYPE_CODE", "", 100, 1));
                colum.Add(new ColumnInfo("TEST_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditADO = new ControlEditorADO("TEST_TYPE_NAME", "ID", colum, false, 350);
                ControlEditorLoader.Load(CboTestType, lsTestType, controlEditADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboOtherPay()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisOtherPaySourceFilter filter = new HisOtherPaySourceFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var lsTestType = new BackendAdapter(param).Get<List<HIS_OTHER_PAY_SOURCE>>("api/HisOtherPaySource/Get", ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("OTHER_PAY_SOURCE_CODE", Inventec.Common.Resource.Get.Value("UC_HisService.ColumnCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 100, 1));
                colum.Add(new ColumnInfo("OTHER_PAY_SOURCE_NAME", Inventec.Common.Resource.Get.Value("UC_HisService.ColumnName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 250, 2));
                ControlEditorADO controlEditADO = new ControlEditorADO("OTHER_PAY_SOURCE_NAME", "ID", colum, true, 350);
                ControlEditorLoader.Load(cboOTHER_PAY_SOURCE, lsTestType, controlEditADO);
                cboOTHER_PAY_SOURCE.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboParentID()
        {
            try
            {
                if (listParentService == null || listParentService.Count == 0)
                {
                    listParentService = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o =>
                        o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                        && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        .OrderByDescending(o => o.MODIFY_TIME).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SERVICE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SERVICE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboParent, listParentService, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboGender()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboGender, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPrintType()
        {
            try
            {
                var data = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>().Where(o => o.IS_ACTIVE == 1 && o.IS_ALLOW_ATTACH_PRINT == 1).OrderBy(o => o.PRINT_TYPE_CODE).ToList();
                ListPrintType = data;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PRINT_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PRINT_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PRINT_TYPE_NAME", "PRINT_TYPE_CODE", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboAttachAssignPrintTypeCode, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPetroleum()
        {
            try
            {
                HisPetroleumFilter filter = new HisPetroleumFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(new CommonParam()).Get<List<HIS_PETROLEUM>>("api/HisPetroleum/Get", ApiConsumers.MosConsumer, filter, null);

                //var data = BackendDataWorker.Get<HIS_PETROLEUM>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PETROLEUM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PETROLEUM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PETROLEUM_NAME", "PETROLEUM_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboPetroleum, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void SetFilterNavBar(ref HisServiceViewFilter filter)
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {
                if (data != null)
                {
                    this.ActionType = GlobalVariables.ActionEdit;
                    SetEnableDD(true);
                    FillDataToEditorControl(data);

                    EnableControlChanged(this.ActionType);

                    dtInTime.Enabled = true;
                    setEnableSpinHein(true);
                    chkUpdateOnly.Checked = true;

                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                    positionHandle = -1;

                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                    {
                        SetEnableDD(false);
                    }
                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                    {
                        CboTestType.Enabled = true;
                        cboSampleType.Enabled = true;

                    }
                    else
                    {
                        cboSampleType.EditValue = null;
                        cboSampleType.Enabled = false;
                    }
                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    {
                        cboFILM_SIZE.Enabled = true;
                    }
                    else
                    {
                        cboFILM_SIZE.Enabled = false;
                    }
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);

                    if (data.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA || data.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TDCN)
                    {
                        cboChiSo.Enabled = true;
                    }
                    else
                    {
                        cboChiSo.Enabled = false;
                        cboChiSo.EditValue = null;
                    }

                    if (data.HEIN_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VC)
                    {
                        cboPetroleum.Enabled = true;
                    }
                    else
                    {
                        cboPetroleum.Enabled = false;
                        cboPetroleum.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {
                if (data != null)
                {
                    if (data.ICD_CM_ID != null)
                        txtIcdCm.Text = BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ID == data.ICD_CM_ID).ICD_CM_CODE;
                    else
                        txtIcdCm.Text = "";
                    GridCheckMarksSelection gridCheckMarkChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkChiSo.ClearSelection(cboChiSo.Properties.View);

                    GridCheckMarksSelection gridCheckMarkPart = cboPart.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPart.ClearSelection(cboPart.Properties.View);

                    GridCheckMarksSelection gridCheckMarkPatientType = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPatientType.ClearSelection(cboAppliedPatientType.Properties.View);

                    GridCheckMarksSelection gridCheckMarkPatientClassify = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPatientClassify.ClearSelection(cboPatientClassify.Properties.View);

                    GridCheckMarksSelection gridCheckDTTT1 = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                    gridCheckDTTT1.ClearSelection(cboDTTT1.Properties.View);

                    GridCheckMarksSelection gridCheckDTTT2 = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                    gridCheckDTTT2.ClearSelection(cboDTTT2.Properties.View);

                    GridCheckMarksSelection gridCheckDTTT3 = cboDTTT3.Properties.Tag as GridCheckMarksSelection;
                    gridCheckDTTT3.ClearSelection(cboDTTT3.Properties.View);

                    txtNotice.Text = data.NOTICE;
                    txtDescription.Text = data.DESCRIPTION;
                    txtMisuSerTypeCode.Text = data.SERVICE_CODE;
                    txtMisuSerTypeName.Text = data.SERVICE_NAME;
                    txtRatioSymbol.Text = data.RATION_SYMBOL;
                    txtTestingTechnique.Text = data.TESTING_TECHNIQUE;
                    spNumOrder.EditValue = data.NUM_ORDER;
                    spCogs.EditValue = data.COGS;
                    spEstimateDuration.EditValue = data.ESTIMATE_DURATION;
                    cboParent.EditValue = data.PARENT_ID;
                    cboDVT.EditValue = data.SERVICE_UNIT_ID;
                    cboServiceType.EditValue = data.SERVICE_TYPE_ID;
                    txtBHYTCode.Text = data.HEIN_SERVICE_BHYT_CODE;
                    txtBHYTName.Text = data.HEIN_SERVICE_BHYT_NAME;
                    txtBHYTStt.Text = data.HEIN_ORDER;
                    txtPacsType.Text = data.PACS_TYPE_CODE;

                    txtProcessCode.Text = data.PROCESS_CODE;
                    GridCheckMarksSelection gridCheckMark = cboPart.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.BODY_PART_IDS) && cboPart.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(data.BODY_PART_IDS, gridCheckMark);
                    }

                    GridCheckMarksSelection gridCheckMarkPatient = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.APPLIED_PATIENT_TYPE_IDS) && cboAppliedPatientType.Properties.Tag != null)
                    {
                        ProcessSelectBusinessTreatmentType(data.APPLIED_PATIENT_TYPE_IDS, gridCheckMarkPatient, cboAppliedPatientType);
                    }
                    else
                    {
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboAppliedPatientType.Properties.View);
                        cboAppliedPatientType.EditValue = null;
                    }

                    GridCheckMarksSelection gridCheckMarkClassify = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.APPLIED_PATIENT_CLASSIFY_IDS) && cboPatientClassify.Properties.Tag != null)
                    {
                        ProcessSelectBusinessTreatmentClassify(data.APPLIED_PATIENT_CLASSIFY_IDS, gridCheckMarkClassify, cboPatientClassify);
                    }
                    else
                    {
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboPatientClassify.Properties.View);
                        cboPatientClassify.EditValue = null;
                    }

                    GridCheckMarksSelection gridCheckDttt1 = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.MIN_PROC_TIME_EXCEPT_PATY_IDS) && cboDTTT1.Properties.Tag != null)
                    {
                        ProcessSelectBusinessTreatmentType(data.MIN_PROC_TIME_EXCEPT_PATY_IDS, gridCheckDttt1, cboDTTT1);
                    }
                    else
                    {
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboDTTT1.Properties.View);
                        cboDTTT1.EditValue = null;
                    }

                    GridCheckMarksSelection gridCheckDttt2 = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.MAX_PROC_TIME_EXCEPT_PATY_IDS) && cboDTTT2.Properties.Tag != null)
                    {
                        ProcessSelectBusinessTreatmentType(data.MAX_PROC_TIME_EXCEPT_PATY_IDS, gridCheckDttt2, cboDTTT2);
                    }
                    else
                    {
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboDTTT2.Properties.View);
                        cboDTTT2.EditValue = null;
                    }

                    GridCheckMarksSelection gridCheckDttt3 = cboDTTT3.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.TOTAL_TIME_EXCEPT_PATY_IDS) && cboDTTT3.Properties.Tag != null)
                    {
                        ProcessSelectBusinessTreatmentType(data.TOTAL_TIME_EXCEPT_PATY_IDS, gridCheckDttt3, cboDTTT3);
                    }
                    else
                    {
                        GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDTTT3.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkBusinessCodes.ClearSelection(cboDTTT3.Properties.View);
                        cboDTTT3.EditValue = null;
                    }
                    listHisSuimIndexDefault = new List<HIS_SUIM_INDEX>();
                    GridCheckMarksSelection gridCheckChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                    if (data.SUIM_INDEX_ID != null && cboChiSo.Properties.Tag != null)
                    {
                        ProcessSelectChiSo(data.SUIM_INDEX_ID.ToString(), gridCheckChiSo);
                    }
                    //else
                    //{
                    CommonParam param = new CommonParam();
                    HisSuimSetySuinFilter SetyFilter = new HisSuimSetySuinFilter();
                    SetyFilter.SUIM_SERVICE_TYPE_ID = data.ID;
                    var listHisSuimSety = new BackendAdapter(param).Get<List<HIS_SUIM_SETY_SUIN>>("api/HisSuimSetySuin/Get", ApiConsumers.MosConsumer, SetyFilter, null).ToList();

                    if (listHisSuimSety != null && listHisSuimSety.Count > 0)
                    {
                        ProcessSelectChiSo(string.Join(",", listHisSuimSety.Select(o => o.SUIM_INDEX_ID).ToList()), gridCheckChiSo);
                    }
                    else if (data.SUIM_INDEX_ID == null)
                    {
                        gridCheckChiSo.ClearSelection(cboChiSo.Properties.View);
                        cboChiSo.EditValue = null;
                    }
                    //}

                    CheckPTTT(data);
                    spHeinNew.EditValue = data.HEIN_LIMIT_PRICE;
                    spHeinOld.EditValue = data.HEIN_LIMIT_PRICE_OLD;
                    dtInTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.HEIN_LIMIT_PRICE_IN_TIME ?? 0);
                    cboType.EditValue = data.HEIN_SERVICE_TYPE_ID;
                    cboTaxRateType.EditValue = data.TAX_RATE_TYPE;
                    cboRationGroup.EditValue = data.RATION_GROUP_ID;
                    spNumberfilm.EditValue = data.NUMBER_OF_FILM;
                    spMaxExpend.EditValue = data.MAX_EXPEND;
                    spMinProcess.EditValue = data.MIN_PROCESS_TIME;
                    spnMaxProcessTime.EditValue = data.MAX_PROCESS_TIME;
                    spMaxTotalProcessTime.EditValue = data.MAX_TOTAL_PROCESS_TIME;
                    if (data.BILL_OPTION == null)
                    {
                        cboBillOption.EditValue = 1;
                    }
                    else if (data.BILL_OPTION == 1)
                    {
                        cboBillOption.EditValue = 2;
                    }
                    else if (data.BILL_OPTION == 2)
                    {
                        cboBillOption.EditValue = 3;
                    }
                    cboPackage.EditValue = data.PACKAGE_ID;
                    txtPackagePrice.EditValue = data.PACKAGE_PRICE;
                    txtSpecialityCode.EditValue = data.SPECIALITY_CODE;
                    cboPatientType.EditValue = data.BILL_PATIENT_TYPE_ID;
                    cboDefaultPatientType.EditValue = data.DEFAULT_PATIENT_TYPE_ID;
                    chkNotChangePaty.Checked = (data.IS_NOT_CHANGE_BILL_PATY == (short)1);
                    chkNotChangePaty.Checked = data.IS_NOT_CHANGE_BILL_PATY == (short)1;
                    chkCPNgoaiGoi.Checked = data.IS_OUT_PARENT_FEE == 1 ? true : false;
                    chkChiDinh.Checked = data.IS_MULTI_REQUEST == 1 ? true : false;
                    chkTachdichvu.Checked = data.IS_SPLIT_SERVICE == 1 ? true : false;
                    chkIsKidney.Checked = data.IS_KIDNEY == 1 ? true : false;
                    chkAllowExpend.Checked = data.IS_ALLOW_EXPEND == 1 ? true : false;
                    chkIsAntibioticResistance.Checked = data.IS_ANTIBIOTIC_RESISTANCE == 1 ? true : false;
                    chkSplitTreatment.Checked = data.IS_SPLIT_SERVICE_REQ == 1 ? true : false;
                    chkIS_AUTO_EXPEND.Checked = data.IS_AUTO_EXPEND == 1 ? true : false;
                    chkForPrice.Checked = data.IS_ENABLE_ASSIGN_PRICE == 1 ? true : false;
                    chkNgoaiDinhSuat.Checked = data.IS_OUT_OF_DRG == 1 ? true : false;
                    chkIsOtherSourcePaid.Checked = data.IS_OTHER_SOURCE_PAID == 1 ? true : false;
                    chkIsBlockDepartment.Checked = data.IS_BLOCK_DEPARTMENT_TRAN == 1 ? true : false;
                    chkNotUseBHYT.Checked = data.DO_NOT_USE_BHYT == 1 ? true : false;
                    chkAllowSimultaneity.Checked = data.ALLOW_SIMULTANEITY == 1 ? true : false;
                    chkMUST_BE_CONSULTED.Checked = data.MUST_BE_CONSULTED == 1 ? true : false;
                    chkIS_OUT_OF_MANAGEMENT.Checked = data.IS_OUT_OF_MANAGEMENT == 1 ? true : false;
                    chkIS_NOT_SHOW_TRACKING.Checked = data.IS_NOT_SHOW_TRACKING == 1 ? true : false;
                    chkNOT_REQUIRED_COMPLETE.Checked = data.IS_NOT_REQUIRED_COMPLETE == 1 ? true : false;
                    chkIsDisAllowanceNoExecute.Checked = data.IS_DISALLOWANCE_NO_EXECUTE == 1 ? true : false;
                    chkAllowSendPacs.Checked = data.ALLOW_SEND_PACS == 1 ? true : false;
                    cboExeServiceModule.EditValue = data.EXE_SERVICE_MODULE_ID;
                    cboDiimType.EditValue = data.DIIM_TYPE_ID;
                    if (data.DIIM_TYPE_ID.HasValue)
                        cboDiimType.Properties.Buttons[1].Visible = true;
                    cboFuexType.EditValue = data.FUEX_TYPE_ID;
                    if (data.FUEX_TYPE_ID.HasValue)
                        cboFuexType.Properties.Buttons[1].Visible = true;
                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    {
                        spNumberfilm.Enabled = true;
                        cboDiimType.Enabled = true;
                    }
                    else
                    {
                        spNumberfilm.Enabled = false;
                        cboDiimType.Enabled = false;
                        cboDiimType.EditValue = null;
                        spNumberfilm.EditValue = null;
                    }
                    if (data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                    {
                        cboFuexType.Enabled = true;
                    }
                    else
                    {
                        cboFuexType.Enabled = false;
                        cboFuexType.EditValue = null;
                    }

                    spMinDuration.EditValue = data.MIN_DURATION;
                    spinWARNING.EditValue = data.WARNING_SAMPLING_TIME;
                    spinMaxAmount.EditValue = data.MAX_AMOUNT;
                    spAgeFrom.EditValue = data.AGE_FROM;
                    txtCapacity.EditValue = data.CAPACITY;
                    spAgeTo.EditValue = data.AGE_TO;
                    if (data.GENDER_ID != null)
                    {
                        var gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == data.GENDER_ID);
                        if (gender != null)
                        {
                            cboGender.EditValue = gender.ID;
                        }
                    }
                    else
                    {
                        cboGender.EditValue = null;
                    }

                    CboTestType.EditValue = data.TEST_TYPE_ID;
                    cboOTHER_PAY_SOURCE.EditValue = data.OTHER_PAY_SOURCE_ID;
                    txtOTHER_PAY_SOURCE.Text = data.OTHER_PAY_SOURCE_ICDS;
                    cboAttachAssignPrintTypeCode.EditValue = data.ATTACH_ASSIGN_PRINT_TYPE_CODE;
                    cboFILM_SIZE.EditValue = data.FILM_SIZE_ID;
                    cboSampleType.EditValue = data.SAMPLE_TYPE_CODE;
                    cboPetroleum.EditValue = data.PETROLEUM_CODE;
                    //load phieu benh an
                    //thinhdt2
                    if (!string.IsNullOrEmpty(data.EMR_FORM_CODES))
                    {
                        var codes = data.EMR_FORM_CODES.Split(',');
                        var dataEmr = this.listEmrForm.Where(s => codes.Contains(s.EMR_FORM_CODE)).ToList();
                        GridCheckMarksSelection gridCheckPhieuBA = cboPhieuBA.Properties.Tag as GridCheckMarksSelection;
                        gridCheckPhieuBA.ClearSelection(cboPhieuBA.Properties.View);
                        gridCheckPhieuBA.SelectAll(dataEmr);
                    }
                    else
                    {
                        GridCheckMarksSelection gridCheckPhieuBA = cboPhieuBA.Properties.Tag as GridCheckMarksSelection;
                        gridCheckPhieuBA.ClearSelection(cboPhieuBA.Properties.View);
                        this.EMR_FORM_CODES = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessSelectBusinessTreatmentClassify(string p, GridCheckMarksSelection gridCheckMark, GridLookUpEdit cbo)
        {
            try
            {
                List<HIS_PATIENT_CLASSIFY> ds = cbo.Properties.DataSource as List<HIS_PATIENT_CLASSIFY>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_PATIENT_CLASSIFY> selects = new List<HIS_PATIENT_CLASSIFY>();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_BODY_PART> ds = cboPart.Properties.DataSource as List<HIS_BODY_PART>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_BODY_PART> selects = new List<HIS_BODY_PART>();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_SUIM_INDEX> listHisSuimIndexDefault = new List<HIS_SUIM_INDEX>();
        private void ProcessSelectChiSo(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_SUIM_INDEX> ds = cboChiSo.Properties.DataSource as List<HIS_SUIM_INDEX>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_SUIM_INDEX> selects = new List<HIS_SUIM_INDEX>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                            listHisSuimIndexDefault.Add(row);
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

        private void ProcessSelectBusinessTreatmentType(string p, GridCheckMarksSelection gridCheckMark, DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<HIS_PATIENT_TYPE> ds = cbo.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_PATIENT_TYPE> selects = new List<HIS_PATIENT_TYPE>();
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusinessTreatmentType1(string p, GridCheckMarksSelection gridCheckDTTT1)
        {
            try
            {
                List<HIS_PATIENT_TYPE> ds = cboDTTT1.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                string[] arrays = p.Split('=');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_PATIENT_TYPE> selects = new List<HIS_PATIENT_TYPE>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckDTTT1.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSelectBusinessTreatmentType2(string p, GridCheckMarksSelection gridCheckDTTT2)
        {
            try
            {
                List<HIS_PATIENT_TYPE> ds = cboDTTT2.Properties.DataSource as List<HIS_PATIENT_TYPE>;
                string[] arrays = p.Split('=');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_PATIENT_TYPE> selects = new List<HIS_PATIENT_TYPE>();
                    foreach (var item in arrays)
                    {
                        var row = ds != null ? ds.FirstOrDefault(o => o.ID.ToString() == item) : null;
                        if (row != null)
                        {
                            selects.Add(row);
                        }
                    }
                    gridCheckDTTT2.SelectAll(selects);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckPTTT(MOS.EFMODEL.DataModels.V_HIS_SERVICE data)
        {
            try
            {
                if (!typeIdNotShowPtttInfos.Contains(data.SERVICE_TYPE_ID))
                {
                    SetEnablePTTT(true, data.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                    cboGroup.EditValue = data.PTTT_GROUP_ID;
                    cboIcdCm.EditValue = data.ICD_CM_ID;
                    cboMethod.EditValue = data.PTTT_METHOD_ID;
                    cboRationGroup.EditValue = data.RATION_GROUP_ID;
                }
                else
                {
                    cboGroup.EditValue = null;
                    cboIcdCm.EditValue = null;
                    cboMethod.EditValue = null;
                    cboRationGroup.EditValue = null;
                    SetEnablePTTT(false, false);
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
                            if (fomatFrm != lci.Control as DevExpress.XtraEditors.CheckEdit)
                            {
                                fomatFrm.Text = "";
                                fomatFrm.EditValue = null;
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_SERVICE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceFilter filter = new HisServiceFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE>>(HisRequestUriStore.MOSHIS_SERVICE_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                chkUpdateAll.Enabled = (action == GlobalVariables.ActionEdit);
                chkUpdateOnly.Enabled = (action == GlobalVariables.ActionEdit);
                cboServiceType.Enabled = (action == GlobalVariables.ActionAdd);
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
                LoaddataToTreeList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void unLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            HIS_SERVICE success = new HIS_SERVICE();
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_CHANGE_LOCK, ApiConsumers.MosConsumer, rowData.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        result = true;
                        LoaddataToTreeList();
                    }
                    MessageManager.Show(this.ParentForm, param, result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Lock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool result = false;
            HIS_SERVICE success = new HIS_SERVICE();
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    success = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_CHANGE_LOCK, ApiConsumers.MosConsumer, rowData.ID, param);
                    WaitingManager.Hide();
                    if (success != null)
                    {
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        result = true;
                        LoaddataToTreeList();
                    }
                    MessageManager.Show(this.ParentForm, param, result);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                dtInTime.Enabled = true;
                cboServiceType.Enabled = true;
                txtRatioSymbol.Enabled = false;
                txtRatioSymbol.EditValue = null;
                cboRationGroup.Enabled = false;
                cboRationGroup.EditValue = null;
                cboFILM_SIZE.Enabled = false;
                cboFILM_SIZE.EditValue = null;
                layoutControlItem35.Enabled = false;
                setEnableSpinHein(true);
                cboChiSo.Enabled = false;
                if (chkAllowExpend.Checked)
                {
                    chkIS_AUTO_EXPEND.Enabled = true;
                }
                else
                {
                    chkIS_AUTO_EXPEND.Enabled = false;
                }
                chkIsBlockDepartment.Checked = false;
                chkAllowSimultaneity.Checked = false;
                chkNotUseBHYT.Checked = false;
                this.EMR_FORM_CODES = null;
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
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSHIS_SERVICE_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<V_HIS_SERVICE>();
                            LoaddataToTreeList();
                            currentData = ((List<V_HIS_SERVICE>)treeList1.DataSource).FirstOrDefault();

                        }
                        MessageManager.Show(this.ParentForm, param, success);
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
            btnRefesh_Click(null, null);
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

        private bool CheckServiceTypeParent()
        {
            bool result = false;
            try
            {
                if (cboServiceType.EditValue != null && cboParent.EditValue != null)
                {
                    var serviceTypeParent = BackendDataWorker.Get<HIS_SERVICE_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0"));
                    var serviceParent = BackendDataWorker.Get<V_HIS_SERVICE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboParent.EditValue.ToString() ?? "0"));
                    if (serviceTypeParent != null && serviceParent != null && serviceTypeParent.ID == serviceParent.SERVICE_TYPE_ID)
                    {
                        result = true;
                    }
                }
                if (cboParent.EditValue == null)
                    result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
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

                if (cboPackage.EditValue != null && txtPackagePrice.EditValue == null)
                {
                    dxErrorProvider.SetError(txtPackagePrice, "Chưa nhập trường dữ liệu bắt buộc", ErrorType.Warning);
                    return;
                }
                else if (cboPackage.EditValue != null && txtPackagePrice.EditValue != null)
                {
                    dxErrorProvider.ClearErrors();
                }

                if (!CheckServiceTypeParent())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại. Loại dịch vụ của cha phải giống loại dịch vụ của con", "Thông báo");
                    return;
                }

                if (string.IsNullOrEmpty(txtBHYTCode.Text) && string.IsNullOrEmpty(txtBHYTName.Text))
                {

                }
                else if (string.IsNullOrEmpty(txtBHYTCode.Text) || string.IsNullOrEmpty(txtBHYTName.Text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Phải nhập đầy đủ mã và tên DV BHYT", "Thông báo");
                    return;
                }

                ValidationSingleControl(txtMisuSerTypeCode);
                ValidationControlMaxLength(txtMisuSerTypeCode, 25, true);
                ValidationControlMaxLength(txtTestingTechnique, 500, false);
                ValidationSingleControl(txtMisuSerTypeName);
                ValidationControlMaxLength(txtMisuSerTypeName, 1500, true);
                ValidationControlMaxLength(txtBHYTName, 1500, false);
                if (txtRatioSymbol.Enabled)
                    ValidationControlMaxLength(txtRatioSymbol, 10, false);
                if (txtOTHER_PAY_SOURCE.Enabled)
                    ValidationControlMaxLength(txtOTHER_PAY_SOURCE, 200, false);
                ValidationControlMaxLength(txtProcessCode, 50, false);

                MOS.EFMODEL.DataModels.HIS_SERVICE updateDTO = new MOS.EFMODEL.DataModels.HIS_SERVICE();

                if (ActionType == GlobalVariables.ActionEdit)
                {
                    if (this.currentData != null && this.currentData.ID > 0)
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE>(updateDTO, this.currentData);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO), updateDTO));
                    }
                }

                UpdateDTOFromDataForm(ref updateDTO);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => updateDTO.IS_AUTO_EXPEND), updateDTO.IS_AUTO_EXPEND));
                WaitingManager.Show();
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Api Add => Begin" + Inventec.Common.Logging.LogUtil.TraceData("", updateDTO));
                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    Inventec.Common.Logging.LogSystem.Info("Api Add => End");
                    if (resultData != null)
                    {
                        Inventec.Common.Logging.LogSystem.Info("LoaddataToTreeList Add => Begin");
                        success = true;
                        LoaddataToTreeList();
                        Inventec.Common.Logging.LogSystem.Info("LoaddataToTreeList Add => End");
                        btnRefesh_Click(null, null);
                        Inventec.Common.Logging.LogSystem.Info("Reset Add => Begin");
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                        Inventec.Common.Logging.LogSystem.Info("Reset Add => End");
                    }
                }
                else
                {
                    HisServiceSDO sdo = new HisServiceSDO();
                    sdo.HisService = updateDTO;

                    if (chkUpdateAll.Checked)
                    {
                        sdo.UpdateSereServ = true;
                    }
                    else if (chkUpdateOnly.Checked)
                    {
                        sdo.UpdateSereServ = false;
                    }
                    else if (!chkUpdateAll.Checked && !chkUpdateOnly.Checked)
                    {
                        sdo.UpdateSereServ = null;
                    }
                    if (listHisSuimIndexChoose == null)
                        listHisSuimIndexChoose = new List<HIS_SUIM_INDEX>();
                    if (listHisSuimIndexDefault == null)
                        listHisSuimIndexDefault = new List<HIS_SUIM_INDEX>();
                    bool allElementsExistChoose = !listHisSuimIndexChoose.Except(listHisSuimIndexDefault).Any();
                    bool allElementsExistDefault = !listHisSuimIndexDefault.Except(listHisSuimIndexChoose).Any();

                    if (!(allElementsExistChoose && allElementsExistDefault))
                        sdo.HisSuimIndexs = listHisSuimIndexChoose;

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(" ---------sdo_______     " + Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                    Inventec.Common.Logging.LogSystem.Debug("Api EDIT => Begin" + Inventec.Common.Logging.LogUtil.TraceData("", sdo));
                    var resultData = new BackendAdapter(param).Post<HIS_SERVICE>(HisRequestUriStore.MOSHIS_SERVICE_UPDATE, ApiConsumers.MosConsumer, sdo, param);
                    Inventec.Common.Logging.LogSystem.Info("Api Edit => End");
                    if (resultData != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug("resultData" + Inventec.Common.Logging.LogUtil.TraceData("", resultData));
                        success = true;
                        btnRefesh_Click(null, null);
                        LoaddataToTreeList();
                        BackendDataWorker.Reset<V_HIS_SERVICE>();
                    }
                }

                if (success)
                {
                    treeList1.RefreshDataSource();
                    LoaddataToTreeList();
                    SetFocusEditor();
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
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

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_SERVICE currentDTO)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtMisuSerTypeCode.Text))
                    currentDTO.SERVICE_CODE = txtMisuSerTypeCode.Text.Trim();
                if (!String.IsNullOrEmpty(txtMisuSerTypeName.Text))
                    currentDTO.SERVICE_NAME = txtMisuSerTypeName.Text.Trim();

                currentDTO.RATION_SYMBOL = !String.IsNullOrEmpty(txtRatioSymbol.Text.Trim()) ? txtRatioSymbol.Text.Trim() : null;
                currentDTO.TESTING_TECHNIQUE = !String.IsNullOrEmpty(txtTestingTechnique.Text.Trim()) ? txtTestingTechnique.Text.Trim() : null;
                currentDTO.EMR_FORM_CODES = this.EMR_FORM_CODES;
                if (chkIS_AUTO_EXPEND.Checked == true)
                {
                    currentDTO.IS_AUTO_EXPEND = 1;
                }
                else
                {
                    currentDTO.IS_AUTO_EXPEND = null;
                }
                if (spNumOrder.EditValue != null) currentDTO.NUM_ORDER = (long)spNumOrder.Value;
                else
                    currentDTO.NUM_ORDER = null;
                if (cboParent.EditValue != null)
                {
                    currentDTO.PARENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboParent.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.PARENT_ID = null;
                }

                if (ActionType == GlobalVariables.ActionAdd && cboChiSo.EditValue != null)
                {
                    currentDTO.HIS_SUIM_SETY_SUIN = new List<HIS_SUIM_SETY_SUIN>();
                    foreach (HIS_SUIM_INDEX rv in listHisSuimIndexChoose)
                    {
                        currentDTO.HIS_SUIM_SETY_SUIN.Add(new HIS_SUIM_SETY_SUIN() { SUIM_INDEX_ID = rv.ID });
                    }
                }
                if (chkNOT_REQUIRED_COMPLETE.Checked == true)
                {
                    currentDTO.IS_NOT_REQUIRED_COMPLETE = 1;
                }
                else
                {
                    currentDTO.IS_NOT_REQUIRED_COMPLETE = null;
                }

                if (cboPart.EditValue != null)
                {
                    GridCheckMarksSelection gridCheckMarkBusiness = cboPart.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                    {
                        List<string> codes = new List<string>();
                        foreach (HIS_BODY_PART rv in gridCheckMarkBusiness.Selection)
                        {
                            if (rv != null && !codes.Contains(rv.ID.ToString()))
                                codes.Add(rv.ID.ToString());
                        }

                        currentDTO.BODY_PART_IDS = String.Join(",", codes);
                    }
                }
                else
                {
                    currentDTO.BODY_PART_IDS = null;
                }

                GridCheckMarksSelection gridCheckMarkPatientType = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMarkPatientType != null && gridCheckMarkPatientType.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_PATIENT_TYPE rv in gridCheckMarkPatientType.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ID.ToString()))
                            codes.Add(rv.ID.ToString());
                    }

                    currentDTO.APPLIED_PATIENT_TYPE_IDS = String.Join(",", codes);
                }
                else
                    currentDTO.APPLIED_PATIENT_TYPE_IDS = null;

                GridCheckMarksSelection gridCheckMarkPatientClassify = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckMarkPatientClassify != null && gridCheckMarkPatientClassify.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_PATIENT_CLASSIFY rv in gridCheckMarkPatientClassify.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ID.ToString()))
                            codes.Add(rv.ID.ToString());
                    }

                    currentDTO.APPLIED_PATIENT_CLASSIFY_IDS = String.Join(",", codes);
                }
                else
                    currentDTO.APPLIED_PATIENT_CLASSIFY_IDS = null;

                GridCheckMarksSelection gridCheckDTTT1 = cboDTTT1.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckDTTT1 != null && gridCheckDTTT1.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_PATIENT_TYPE rv in gridCheckDTTT1.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ID.ToString()))
                            codes.Add(rv.ID.ToString());
                        cboDTTT1.Text = null;
                        cboDTTT1.EditValue = null;
                    }

                    currentDTO.MIN_PROC_TIME_EXCEPT_PATY_IDS = String.Join(",", codes);
                }
                else
                    currentDTO.MIN_PROC_TIME_EXCEPT_PATY_IDS = null;

                GridCheckMarksSelection gridCheckDTTT2 = cboDTTT2.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckDTTT2 != null && gridCheckDTTT2.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_PATIENT_TYPE rv in gridCheckDTTT2.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ID.ToString()))
                            codes.Add(rv.ID.ToString());
                        cboDTTT2.Text = null;
                        cboDTTT2.EditValue = null;
                    }

                    currentDTO.MAX_PROC_TIME_EXCEPT_PATY_IDS = String.Join(",", codes);
                }
                else
                    currentDTO.MAX_PROC_TIME_EXCEPT_PATY_IDS = null;

                GridCheckMarksSelection gridCheckDTTT3 = cboDTTT3.Properties.Tag as GridCheckMarksSelection;

                if (gridCheckDTTT3 != null && gridCheckDTTT3.SelectedCount > 0)
                {
                    List<string> codes = new List<string>();
                    foreach (HIS_PATIENT_TYPE rv in gridCheckDTTT3.Selection)
                    {
                        if (rv != null && !codes.Contains(rv.ID.ToString()))
                            codes.Add(rv.ID.ToString());
                        cboDTTT3.Text = null;
                        cboDTTT3.EditValue = null;
                    }

                    currentDTO.TOTAL_TIME_EXCEPT_PATY_IDS = String.Join(",", codes);
                }
                else
                    currentDTO.TOTAL_TIME_EXCEPT_PATY_IDS = null;


                if (cboMethod.EditValue != null) currentDTO.PTTT_METHOD_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMethod.EditValue ?? "0").ToString());
                else
                    currentDTO.PTTT_METHOD_ID = null;

                if (cboType.EditValue != null) currentDTO.HEIN_SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboType.EditValue ?? "0").ToString());
                else currentDTO.HEIN_SERVICE_TYPE_ID = null;

                if (cboTaxRateType.EditValue != null) currentDTO.TAX_RATE_TYPE = Inventec.Common.TypeConvert.Parse.ToInt64((cboTaxRateType.EditValue ?? "0").ToString());
                else currentDTO.TAX_RATE_TYPE = null;

                if (cboRationGroup.EditValue != null) currentDTO.RATION_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboRationGroup.EditValue ?? "0").ToString());
                else currentDTO.RATION_GROUP_ID = null;

                if (cboDVT.EditValue != null) currentDTO.SERVICE_UNIT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboDVT.EditValue ?? "0").ToString());

                if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? "0").ToString()) == 1) currentDTO.BILL_OPTION = null;
                else if (cboBillOption.EditValue == null)
                    currentDTO.BILL_OPTION = null;
                else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? "0").ToString()) == 2)
                    currentDTO.BILL_OPTION = 1;
                else if (Inventec.Common.TypeConvert.Parse.ToInt64((cboBillOption.EditValue ?? "0").ToString()) == 3)
                    currentDTO.BILL_OPTION = 2;

                currentDTO.DEFAULT_PATIENT_TYPE_ID = cboDefaultPatientType.EditValue != null ? (long?)Convert.ToInt64(cboDefaultPatientType.EditValue) : null;

                if (cboPatientType.EditValue != null)
                {
                    currentDTO.BILL_PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPatientType.EditValue ?? "0").ToString());
                    currentDTO.IS_NOT_CHANGE_BILL_PATY = chkNotChangePaty.Checked ? (short?)1 : null;
                }
                else
                {
                    currentDTO.BILL_PATIENT_TYPE_ID = null;
                    currentDTO.IS_NOT_CHANGE_BILL_PATY = null;
                }
                if (spnMaxProcessTime.EditValue != null)
                {
                    currentDTO.MAX_PROCESS_TIME = Convert.ToInt64(spnMaxProcessTime.Value);
                }
                else
                {
                    currentDTO.MAX_PROCESS_TIME = null;
                }
                if (spMaxTotalProcessTime.EditValue != null)
                {
                    currentDTO.MAX_TOTAL_PROCESS_TIME = Convert.ToInt64(spMaxTotalProcessTime.Value);
                }
                else
                {
                    currentDTO.MAX_TOTAL_PROCESS_TIME = null;
                }
                if (spCogs.EditValue != null) currentDTO.COGS = (decimal)spCogs.Value;
                else
                    currentDTO.COGS = null;

                if (spEstimateDuration.EditValue != null) currentDTO.ESTIMATE_DURATION = (decimal)spEstimateDuration.Value;
                else
                    currentDTO.ESTIMATE_DURATION = null;

                currentDTO.IS_OUT_PARENT_FEE = (short)(chkCPNgoaiGoi.Checked ? 1 : 0);
                currentDTO.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                if (cboGroup.EditValue != null) currentDTO.PTTT_GROUP_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboGroup.EditValue ?? "0").ToString());
                else currentDTO.PTTT_GROUP_ID = null;

                if (cboIcdCm.EditValue != null) currentDTO.ICD_CM_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboIcdCm.EditValue ?? "0").ToString());
                else currentDTO.ICD_CM_ID = null;

                if (cboServiceType.EditValue != null) currentDTO.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue ?? "0").ToString());

                if (spMaxExpend.EditValue != null) currentDTO.MAX_EXPEND = (decimal)spMaxExpend.Value;
                else
                    currentDTO.MAX_EXPEND = null;
                if (spMinProcess.EditValue != null) currentDTO.MIN_PROCESS_TIME = (long)spMinProcess.Value;
                else
                    currentDTO.MIN_PROCESS_TIME = null;
                if (spNumberfilm.EditValue != null)
                {
                    currentDTO.NUMBER_OF_FILM = (long)spNumberfilm.Value;
                }
                else
                {
                    currentDTO.NUMBER_OF_FILM = null;
                }
                currentDTO.HEIN_SERVICE_BHYT_CODE = txtBHYTCode.Text;
                currentDTO.HEIN_SERVICE_BHYT_NAME = txtBHYTName.Text;
                currentDTO.HEIN_ORDER = txtBHYTStt.Text;
                currentDTO.PACS_TYPE_CODE = txtPacsType.Text.Trim();
                if (cboDiimType.EditValue != null)
                {
                    currentDTO.DIIM_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDiimType.EditValue.ToString());
                }
                else
                    currentDTO.DIIM_TYPE_ID = null;

                if (cboFuexType.EditValue != null)
                {
                    currentDTO.FUEX_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboFuexType.EditValue.ToString());
                }
                else currentDTO.FUEX_TYPE_ID = null;
                currentDTO.NOTICE = txtNotice.Text.Trim();
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();
                if (spMinDuration.EditValue != null) currentDTO.MIN_DURATION = (long)spMinDuration.Value;
                else
                    currentDTO.MIN_DURATION = null;

                if (spinWARNING.EditValue != null)
                {
                    currentDTO.WARNING_SAMPLING_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(spinWARNING.EditValue.ToString());
                }
                else
                {
                    currentDTO.WARNING_SAMPLING_TIME = null;
                }
                if (spinMaxAmount.EditValue != null)
                {
                    currentDTO.MAX_AMOUNT = Inventec.Common.TypeConvert.Parse.ToInt64(spinMaxAmount.EditValue.ToString());
                }
                else
                {
                    currentDTO.MAX_AMOUNT = null;
                }
                if (spHeinNew.Enabled)
                {
                    if (spHeinNew.EditValue != null) currentDTO.HEIN_LIMIT_PRICE = (decimal)spHeinNew.Value;
                    else
                        currentDTO.HEIN_LIMIT_PRICE = null;
                }
                else
                {
                    currentDTO.HEIN_LIMIT_PRICE = null;
                }

                if (spHeinOld.Enabled)
                {
                    if (spHeinOld.EditValue != null) currentDTO.HEIN_LIMIT_PRICE_OLD = (decimal)spHeinOld.Value;
                    else
                        currentDTO.HEIN_LIMIT_PRICE_OLD = null;
                }
                else
                {
                    currentDTO.HEIN_LIMIT_PRICE_OLD = null;
                }

                if (dtInTime.DateTime != null && dtInTime.DateTime != DateTime.MinValue)
                {
                    currentDTO.HEIN_LIMIT_PRICE_IN_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtInTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                else
                {
                    currentDTO.HEIN_LIMIT_PRICE_IN_TIME = null;
                }

                if (txtPackagePrice.Enabled)
                {
                    if (txtPackagePrice.EditValue != null) currentDTO.PACKAGE_PRICE = (decimal)txtPackagePrice.Value;
                    else
                        currentDTO.PACKAGE_PRICE = null;
                }
                else
                {
                    currentDTO.PACKAGE_PRICE = null;
                }

                if (txtSpecialityCode.Enabled)
                {
                    if (txtSpecialityCode.EditValue != null) currentDTO.SPECIALITY_CODE = txtSpecialityCode.Text;
                }
                else
                    currentDTO.SPECIALITY_CODE = null;

                if (cboPackage.EditValue != null) currentDTO.PACKAGE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboPackage.EditValue ?? "0").ToString());
                else
                    currentDTO.PACKAGE_ID = null;

                if (cboExeServiceModule.EditValue != null) currentDTO.EXE_SERVICE_MODULE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboExeServiceModule.EditValue ?? "0").ToString());
                else
                    currentDTO.EXE_SERVICE_MODULE_ID = null;

                currentDTO.IS_MULTI_REQUEST = (short)(chkChiDinh.Checked ? 1 : 0);
                currentDTO.IS_ALLOW_EXPEND = (short)(chkAllowExpend.Checked ? 1 : 0);
                currentDTO.IS_KIDNEY = (short)(chkIsKidney.Checked ? 1 : 0);
                currentDTO.IS_ANTIBIOTIC_RESISTANCE = (short)(chkIsAntibioticResistance.Checked ? 1 : 0);
                currentDTO.IS_OTHER_SOURCE_PAID = (short)(chkIsOtherSourcePaid.Checked ? 1 : 0);
                currentDTO.MUST_BE_CONSULTED = (short)(chkMUST_BE_CONSULTED.Checked ? 1 : 0);
                currentDTO.IS_OUT_OF_MANAGEMENT = (short)(chkIS_OUT_OF_MANAGEMENT.Checked ? 1 : 0);
                currentDTO.IS_BLOCK_DEPARTMENT_TRAN = (short)(chkIsBlockDepartment.Checked ? 1 : 0);
                currentDTO.ALLOW_SIMULTANEITY = (short)(chkAllowSimultaneity.Checked ? 1 : 0);
                if (chkNotUseBHYT.Checked)
                {
                    currentDTO.DO_NOT_USE_BHYT = 1;
                }
                else
                {
                    currentDTO.DO_NOT_USE_BHYT = null;
                }
                if (chkIS_NOT_SHOW_TRACKING.Checked)
                {
                    currentDTO.IS_NOT_SHOW_TRACKING = 1;
                }
                else
                {
                    currentDTO.IS_NOT_SHOW_TRACKING = null;
                }
                if (chkNOT_REQUIRED_COMPLETE.Checked)
                {
                    currentDTO.IS_NOT_REQUIRED_COMPLETE = 1;
                }
                else
                {
                    currentDTO.IS_NOT_REQUIRED_COMPLETE = null;
                }
                if (chkAllowSendPacs.Checked)
                {
                    currentDTO.ALLOW_SEND_PACS = 1;
                }
                else
                {
                    currentDTO.ALLOW_SEND_PACS = null;
                }

                //  Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentDTO.IS_AUTO_EXPEND), currentDTO.IS_AUTO_EXPEND));
                // currentDTO.IS_NOT_SHOW_TRACKING = (short)(chkIS_NOT_SHOW_TRACKING.Checked ? 1 : null ); 
                currentDTO.IS_SPLIT_SERVICE = (short)(chkTachdichvu.Checked ? 1 : 0);
                currentDTO.IS_SPLIT_SERVICE_REQ = (short)(chkSplitTreatment.Checked ? 1 : 0);
                currentDTO.IS_ENABLE_ASSIGN_PRICE = (short)(chkForPrice.Checked ? 1 : 0);
                currentDTO.IS_OUT_OF_DRG = (short)(chkNgoaiDinhSuat.Checked ? 1 : 0);
                currentDTO.IS_DISALLOWANCE_NO_EXECUTE = (short)(chkIsDisAllowanceNoExecute.Checked ? 1 : 0);

                if (cboGender.EditValue != null)
                {
                    currentDTO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                }
                else
                {
                    currentDTO.GENDER_ID = null;
                }

                if (spAgeFrom.EditValue != null)
                {
                    currentDTO.AGE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(spAgeFrom.Value.ToString());
                }
                else
                {
                    currentDTO.AGE_FROM = null;
                }

                if (txtCapacity.EditValue != null)
                {
                    currentDTO.CAPACITY = Inventec.Common.TypeConvert.Parse.ToInt64(txtCapacity.Value.ToString());
                }
                else
                {
                    currentDTO.CAPACITY = null;
                }
                if (spAgeTo.EditValue != null)
                {
                    currentDTO.AGE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(spAgeTo.Value.ToString());
                }
                else
                {
                    currentDTO.AGE_TO = null;
                }

                currentDTO.PROCESS_CODE = txtProcessCode.Text.Trim();

                if (CboTestType.Enabled)
                {
                    if (CboTestType.EditValue != null)
                    {
                        currentDTO.TEST_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((CboTestType.EditValue ?? "0").ToString());
                    }
                    else
                        currentDTO.TEST_TYPE_ID = null;
                }

                if (cboOTHER_PAY_SOURCE.EditValue != null)
                {
                    currentDTO.OTHER_PAY_SOURCE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboOTHER_PAY_SOURCE.EditValue ?? "0").ToString());
                }
                else
                {
                    currentDTO.OTHER_PAY_SOURCE_ID = null;
                }
                if (txtOTHER_PAY_SOURCE.Enabled)
                {
                    currentDTO.OTHER_PAY_SOURCE_ICDS = txtOTHER_PAY_SOURCE.Text;
                }
                else
                    currentDTO.OTHER_PAY_SOURCE_ICDS = null;

                if (cboAttachAssignPrintTypeCode.EditValue != null)
                {
                    currentDTO.ATTACH_ASSIGN_PRINT_TYPE_CODE = cboAttachAssignPrintTypeCode.EditValue.ToString();
                }
                else
                {
                    currentDTO.ATTACH_ASSIGN_PRINT_TYPE_CODE = null;
                }

                if (cboFILM_SIZE.EditValue != null)
                {
                    currentDTO.FILM_SIZE_ID = (long)cboFILM_SIZE.EditValue;
                }
                else
                {
                    currentDTO.FILM_SIZE_ID = null;
                }

                currentDTO.SAMPLE_TYPE_CODE = cboSampleType.EditValue != null ? cboSampleType.EditValue.ToString() : null;
                if (cboPetroleum.EditValue != null)
                {
                    currentDTO.PETROLEUM_CODE = cboPetroleum.EditValue.ToString();
                    currentDTO.PETROLEUM_NAME = cboPetroleum.Text;
                }
                else
                {
                    currentDTO.PETROLEUM_CODE = null;
                    currentDTO.PETROLEUM_NAME = null;
                }
            }
            catch (Exception ex)
            {
                currentDTO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(txtMisuSerTypeCode);
                ValidationControlMaxLength(txtMisuSerTypeCode, 25, true);
                ValidationSingleControl(txtMisuSerTypeName);
                ValidationControlMaxLength(txtMisuSerTypeName, 1500, true);
                ValidationControlMaxLength(txtBHYTName, 1500, false);
                ValidationControlMaxLength(txtTestingTechnique, 500, false);
                ValidationControlMaxLength(txtProcessCode, 50, false);

                if (txtRatioSymbol.Enabled)
                    ValidationControlMaxLength(txtRatioSymbol, 10, false);
                if (txtOTHER_PAY_SOURCE.Enabled)
                    ValidationControlMaxLength(txtOTHER_PAY_SOURCE, 200, false);
                ValidationSingleControl(cboServiceType);
                ValidationSingleControl(cboDVT);
                ValidationSingleControl1(spNumOrder);
                ValidationSingleControl1(spCogs);
                ValidationSingleControl1(spEstimateDuration);
                ValidationControlAgeMonth(spAgeFrom, spAgeTo);
                ValidationSingleControl1(spHeinNew);
                ValidationSingleControl1(txtPackagePrice);
                ValidationSingleControl1(spHeinOld);
                ValidationSingleControl1(spMinDuration);
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

        private void ValidationSingleControl1(SpinEdit control)
        {
            try
            {
                ValidateSpin2 validate = new ValidateSpin2();
                validate.spin = control;
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl3(SpinEdit control)
        {
            try
            {
                ValidateSpin validate = new ValidateSpin();
                validate.spin = control;
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl2(SpinEdit control)
        {
            try
            {
                ValidateSpin1 validate = new ValidateSpin1();
                validate.spin = control;
                validate.ErrorType = ErrorType.Warning;
                this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlAgeMonth(SpinEdit controlFrom, SpinEdit controlTo)
        {
            try
            {
                ValidationAgeMonth validRule = new ValidationAgeMonth();
                validRule.spinAgeFrom = controlFrom;
                validRule.spinAgeTo = controlTo;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(controlFrom, validRule);
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
                btnRefesh_Click(null, null);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                LoaddataToTreeList();

                //Load ngon ngu label control
                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                //Set tabindex control
                //InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();

                if (chkAllowExpend.Checked)
                {
                    chkIS_AUTO_EXPEND.Enabled = true;
                }
                else
                {
                    chkIS_AUTO_EXPEND.Enabled = false;
                }
                LoadDataCboPhieuBA();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Shortcut
        public void BtnSearch()
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnEdit()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("BtnEdit()");
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    Inventec.Common.Logging.LogSystem.Info("BtnEdit() this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled");
                    btnEdit.Focus();
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void BtnAdd()
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

        public void BtnRefresh()
        {
            try
            {
                if (btnRefresh.Enabled)
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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_SERVICE data = (V_HIS_SERVICE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                                e.Appearance.ForeColor = Color.Red;
                            else
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

        private void txtCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMisuSerTypeName.Focus();
                    txtMisuSerTypeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void txtName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDVT.Focus();
                    if (cboDVT.EditValue == null)
                    {
                        cboDVT.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spEstimateDuration_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spMinDuration.Focus();
                    spMinDuration.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spNumOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPacsType.Focus();
                    txtPacsType.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void spCogs_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboRationGroup.Enabled)
                    {
                        cboRationGroup.Focus();
                        if (cboRationGroup.EditValue == null)
                        {
                            cboRationGroup.ShowPopup();
                        }
                    }
                    else
                    {
                        spNumOrder.Focus();
                        spNumOrder.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboBhyt_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTCode.Focus();
                    txtBHYTCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboParent_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPackage.Focus();
                    if (cboPackage.EditValue == null)
                    {
                        cboPackage.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDVT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtSpecialityCode.Enabled)
                    {
                        txtSpecialityCode.Focus();
                        txtSpecialityCode.SelectAll();
                    }
                    else
                    {
                        cboType.Focus();
                        if (cboType.EditValue == null)
                        {
                            cboType.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTCode.Focus();
                    txtBHYTCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void treeList1_Click(object sender, EventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                if (rowData != null)
                {
                    currentData = rowData;
                    ChangedDataRow(rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_SERVICE)
                {
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "IS_LOCK")
                    {
                        e.RepositoryItem = (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? unLockT : LockT);
                    }
                    else if (e.Column.FieldName == "Delete")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = DeleteE;
                            else
                                e.RepositoryItem = DeleteD;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "ChinhSachGia")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btn_ChinhSachGia_Enable;
                            else
                                e.RepositoryItem = btn_ChinhSachGia_Disable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "PhongXuLy")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btn_PhongXuLy_Enable;
                            else
                                e.RepositoryItem = btn_PhongXuLy_Disable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "BaoCao")
                    {
                        try
                        {
                            if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                                e.RepositoryItem = btn_BaoCao_Enable;
                            else
                                e.RepositoryItem = btn_BaoCao_Disable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == " Service_Rati")
                    {
                        try
                        {
                            if (rowData.SERVICE_TYPE_ID == 16)
                                e.RepositoryItem = btn_ServiceRaty_Enable;
                            else
                                e.RepositoryItem = btn_ServiceRaty_Disable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    V_HIS_SERVICE pData = (V_HIS_SERVICE)e.Row;
                    short status = Inventec.Common.TypeConvert.Parse.ToInt16((pData.IS_ACTIVE ?? -1).ToString());
                    if (pData == null || this.treeList1 == null) return;

                    if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.CREATE_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao CREATE_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_IN_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_IN_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_IN_TIME_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PACKAGE_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PACKAGE>().FirstOrDefault(o => o.ID == pData.PACKAGE_ID);
                            if (data != null)
                            {
                                e.Value = data.PACKAGE_NAME;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_IN_TIME_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_PRICE_INTR_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.HEIN_LIMIT_PRICE_INTR_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao HEIN_LIMIT_PRICE_INTR_TIME_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PTTT_GROUP_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PTTT_GROUP>().FirstOrDefault(o => o.ID == pData.PTTT_GROUP_ID);
                            if (data != null)
                            {
                                e.Value = data.PTTT_GROUP_NAME;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PTTT_GROUP_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PTTT_METHOD_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_PTTT_METHOD>().FirstOrDefault(o => o.ID == pData.PTTT_METHOD_ID);
                            if (data != null)
                            {
                                e.Value = data.PTTT_METHOD_NAME;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua PTTT_METHOD_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "ICD_CM_NAME_STR")
                    {
                        try
                        {
                            var data = BackendDataWorker.Get<HIS_ICD_CM>().FirstOrDefault(o => o.ID == pData.ICD_CM_ID);
                            if (data != null)
                            {
                                e.Value = data.ICD_CM_NAME;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua ICD_CM_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO * 100;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "HEIN_LIMIT_RATIO_OLD_STR")
                    {
                        try
                        {
                            e.Value = pData.HEIN_LIMIT_RATIO_OLD * 100;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "CPNG")
                    {
                        try
                        {
                            e.Value = pData.IS_OUT_PARENT_FEE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua CPNG", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_NOT_CHANGE_PATY_STR")
                    {
                        try
                        {
                            e.Value = (pData.IS_NOT_CHANGE_BILL_PATY == (short)1);
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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(pData.MODIFY_TIME.ToString()));
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BILL_OPTION_STR")
                    {
                        try
                        {
                            if (pData.BILL_OPTION == null)
                                e.Value = "Hóa đơn thường";
                            else if (pData.BILL_OPTION == 1)
                                e.Value = "Tách chênh lệch vào hóa đơn dịch vụ";
                            else if (pData.BILL_OPTION == 2)
                                e.Value = "Hóa đơn dịch vụ";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }
                    else if (e.Column.FieldName == "PATIENT_TYPE_NAME")
                    {
                        try
                        {
                            if (pData.BILL_PATIENT_TYPE_ID.HasValue)
                            {
                                var paty = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == pData.BILL_PATIENT_TYPE_ID.Value);
                                e.Value = paty != null ? paty.PATIENT_TYPE_NAME : "";
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }
                    else if (e.Column.FieldName == "SUIM_INDEX_NAME_STR")
                    {
                        try
                        {
                            if (pData.SUIM_INDEX_ID.HasValue)
                            {
                                var paty = BackendDataWorker.Get<HIS_SUIM_INDEX>().FirstOrDefault(o => o.ID == pData.SUIM_INDEX_ID.Value);
                                e.Value = paty != null ? paty.SUIM_INDEX_NAME : "";
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay sua BILL_OPTION", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoaddataToTreeList()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceViewFilter filter = new HisServiceViewFilter();
                if (cboSearchType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboSearchType.EditValue.ToString() ?? ""));
                }
                else if (cboSearchType.EditValue == null && String.IsNullOrWhiteSpace(txtKeyword.Text))
                {
                    treeList1.DataSource = null;
                    return;
                }
                if (chkLock.CheckState == CheckState.Checked)
                {
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                }

                filter.KEY_WORD = txtKeyword.Text.Trim();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                WaitingManager.Show();
                var currentDataStore = new BackendAdapter(param).Get<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (currentDataStore != null && currentDataStore.Count > 0)
                {
                    currentDataStore = currentDataStore.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();
                    listParentService = currentDataStore.Where(o => o.PARENT_ID == null || currentDataStore.Exists(p => p.PARENT_ID == o.ID)).ToList();
                    InitComboParentID();
                }
                listVServiceExport = new List<V_HIS_SERVICE>();
                listVServiceExport.AddRange(currentDataStore);

                treeList1.KeyFieldName = "ID";
                treeList1.ParentFieldName = "PARENT_ID";
                treeList1.DataSource = currentDataStore;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeList1_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(e.Node);
                if (data != null && data is V_HIS_SERVICE)
                {
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData == null) return;

                    else if (e.Column.FieldName == "ACTIVE_ITEM")
                    {
                        if (rowData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                    V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                    if (rowData != null)
                    {
                        currentData = rowData;
                        ChangedDataRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBillOption_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientType.Focus();
                    if (cboPatientType.EditValue == null)
                    {
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboServiceType.EditValue != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToInt64((cboServiceType.EditValue).ToString()) == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT || Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0") == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                        {
                            cboMethod.Focus();
                            if (cboMethod.EditValue == null)
                            {
                                cboMethod.ShowPopup();
                            }
                        }
                        else
                        {
                            spHeinOld.Focus();
                            spHeinOld.SelectAll();
                        }
                    }
                    else
                    {
                        cboMethod.Focus();
                        if (cboMethod.EditValue == null)
                        {
                            cboMethod.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboParent_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboParent.Properties.Buttons[1].Visible = true;
                    cboParent.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboType.Properties.Buttons[1].Visible = true;
                    cboType.EditValue = null;
                    cboChiSo.Enabled = false;
                    cboChiSo.EditValue = null;
                    cboPetroleum.Enabled = false;
                    cboPetroleum.EditValue = null;
                    cboChiSo.Properties.Buttons[2].Visible = false;
                    GridCheckMarksSelection gridCheckMarkChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkChiSo.ClearSelection(cboChiSo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBillOption_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBillOption.Properties.Buttons[1].Visible = true;
                    cboBillOption.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPatientType.Properties.Buttons[1].Visible = true;
                    cboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMethod_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPatientClassify.Focus();
                    if (cboPatientClassify.EditValue == null)
                    {
                        cboPatientClassify.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboMethod_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboMethod.Properties.Buttons[1].Visible = true;
                    cboMethod.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCPNgoaiGoi_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAllowExpend.Focus();
                }
                if (e.KeyCode == Keys.Space)
                {
                    chkAllowExpend.Checked = !chkAllowExpend.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBHYTCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTName.Focus();
                    txtBHYTName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBHYTName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBHYTStt.Focus();
                    txtBHYTStt.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBHYTStt_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboParent.Focus();
                    if (cboParent.EditValue == null)
                    {
                        cboParent.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spHeinOld.Focus();
                    spHeinOld.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdCm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtInTime.Focus();
                    dtInTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spHeinOld_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtIcdCm.Focus();
                    txtIcdCm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spHeinNew_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtInTime.Focus();
                    if (dtInTime.EditValue == null)
                    {
                        dtInTime.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtRatioSymbol.Enabled)
                    {
                        spHeinNew.Focus();
                        spHeinNew.SelectAll();
                    }
                    else
                    {
                        spCogs.Focus();
                        spCogs.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && txtMisuSerTypeCode.Enabled)
                {
                    txtMisuSerTypeCode.Focus();
                    txtMisuSerTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGroup.Properties.Buttons[1].Visible = true;
                    cboGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIcdCm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboIcdCm.Properties.Buttons[1].Visible = true;
                    cboIcdCm.EditValue = null;
                    txtIcdCm.Text = "";
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
                    chkUpdateOnly.Focus();
                }
                if (e.KeyCode == Keys.Space)
                {
                    chkUpdateAll.Checked = !chkUpdateAll.Checked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkEdit2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                        btnEdit.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAllowExpend_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkForPrice.Focus();
                }

                if (e.KeyCode == Keys.Space)
                {
                    chkUpdateOnly.Checked = !chkUpdateOnly.Checked;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboServiceType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboServiceType.EditValue != null)
                {
                    SetEnableDD(true);
                    var servicceType = Inventec.Common.TypeConvert.Parse.ToInt64(cboServiceType.EditValue.ToString() ?? "0");
                    if (!typeIdNotShowPtttInfos.Contains(servicceType))
                    {
                        SetEnablePTTT(true, servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                    }
                    else
                    {
                        SetEnablePTTT(false, false);
                    }

                    if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                    {
                        txtSpecialityCode.Enabled = true;
                        CboTestType.EditValue = null;
                        CboTestType.Enabled = false;
                    }
                    else if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    {
                        txtSpecialityCode.Enabled = false;
                        txtPackagePrice.EditValue = null;
                        spNumberfilm.Enabled = true;
                        cboDiimType.Enabled = true;
                        CboTestType.EditValue = null;
                        CboTestType.Enabled = false;
                        // cboFILM_SIZE.Enabled = true;
                    }
                    else if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN)
                    {
                        cboFuexType.Enabled = true;
                        CboTestType.EditValue = null;
                        CboTestType.Enabled = false;
                    }
                    else if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                    {
                        SetEnableDD(false);
                        txtRatioSymbol.Enabled = true;
                        cboRationGroup.Enabled = true;
                        layoutControlItem35.Enabled = true;
                        layoutControlItem58.Enabled = true;
                        CboTestType.EditValue = null;
                        CboTestType.Enabled = false;
                    }
                    else if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                    {
                        CboTestType.Enabled = true;
                    }
                    else
                    {
                        txtRatioSymbol.Enabled = false;
                        txtRatioSymbol.EditValue = null;
                        cboRationGroup.Enabled = false;
                        cboRationGroup.EditValue = null;
                        layoutControlItem35.Enabled = false;
                        txtSpecialityCode.Enabled = false;
                        txtPackagePrice.EditValue = null;
                        spNumberfilm.Enabled = false;
                        cboDiimType.Enabled = false;
                        cboFuexType.EditValue = null;
                        cboFuexType.Enabled = false;
                        cboDiimType.EditValue = null;
                        CboTestType.EditValue = null;
                        CboTestType.Enabled = false;
                    }
                    if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                    {
                        cboFILM_SIZE.Enabled = true;
                    }
                    else
                    {
                        cboFILM_SIZE.Enabled = false;
                    }
                    if (ActionType == GlobalVariables.ActionAdd)
                    {
                        if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                            || servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                            || servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                            || servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA)
                        {
                            chkAllowSendPacs.Checked = true;
                        }
                        else
                            chkAllowSendPacs.Checked = false;
                    }
                    if (servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH
                            || servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__G
                            || servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN
                            || servicceType == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN)
                    {
                        chkAllowSendPacs.Enabled = false;
                    }
                    else
                        chkAllowSendPacs.Enabled = true;
                }
                else
                {
                    SetEnablePTTT(true, true);
                    txtSpecialityCode.Enabled = true;
                    chkAllowSendPacs.Checked = false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableDD(bool set)
        {
            try
            {
                spHeinOld.Enabled = set;
                spHeinNew.Enabled = set;
                txtPacsType.Enabled = set;
                cboMethod.Enabled = set;
                cboGroup.Enabled = set;
                txtSpecialityCode.Enabled = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnablePTTT(bool set, bool pt)
        {
            try
            {
                spNumberfilm.Enabled = false;
                cboMethod.Enabled = set;
                cboGroup.Enabled = set;
                cboIcdCm.Enabled = set;
                txtIcdCm.Enabled = set;
                spMaxExpend.Enabled = pt;
                cboFuexType.Enabled = false;
                cboDiimType.EditValue = null;
                cboFuexType.EditValue = null;
                CboTestType.EditValue = null;
                cboDiimType.Enabled = false;
                chkForPrice.Enabled = true;
                CboTestType.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkChiDinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkSplitTreatment.Enabled == true)
                    {
                        chkSplitTreatment.Focus();
                    }
                    else
                    {
                        chkCPNgoaiGoi.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUpdateAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkUpdateAll.Checked)
                {
                    chkUpdateOnly.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUpdateOnly_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkUpdateOnly.Checked)
                {
                    chkUpdateAll.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setEnableSpinHein1(bool set, SpinEdit sp, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    spHeinOld.EditValue = null;
                    spHeinNew.EditValue = null;
                    spHeinNew.Enabled = set;
                    spHeinOld.Enabled = set;
                }
                else
                {
                    spHeinNew.Enabled = !set;
                    spHeinOld.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setEnableSpinHein(bool set)
        {
            try
            {
                spHeinNew.Enabled = set;
                spHeinOld.Enabled = set;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSearchType.Properties.Buttons[1].Visible = true;
                    cboSearchType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setEnableDtIn(bool set, ChangingEventArgs e)
        {
            try
            {
                if (e.NewValue != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
                {
                    dtInTime.EditValue = null;
                    dtInTime.Enabled = set;
                }
                else
                {
                    dtInTime.Enabled = !set;
                    dtInTime.Enabled = !set;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSpecialityCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboType.Focus();
                    if (cboType.EditValue == null)
                    {
                        cboType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPackagePrice.Enabled)
                    {
                        txtPackagePrice.Focus();
                        txtPackagePrice.SelectAll();
                    }
                    else
                    {
                        cboBillOption.Focus();
                        if (cboBillOption.EditValue == null)
                        {
                            cboBillOption.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPackage_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboPackage.EditValue != null)
                {
                    txtPackagePrice.Enabled = true;
                    layoutControlItem35.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    txtPackagePrice.Enabled = false;
                    txtPackagePrice.EditValue = null;
                    layoutControlItem35.AppearanceItemCaption.ForeColor = Color.Transparent;
                    dxValidationProviderEditorInfo.RemoveControlError(txtPackagePrice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackagePrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBillOption.Focus();
                    if (cboBillOption.EditValue == null)
                    {
                        cboBillOption.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick_1(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPackage.Properties.Buttons[1].Visible = true;
                    cboPackage.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btn_ChinhSachGia_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE row = data as V_HIS_SERVICE;
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);

                    if (this.moduleData != null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisServicePatyList, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisServicePatyList, 0, 0, listArgs);
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btn_BaoCao_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE row = data as V_HIS_SERVICE;
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    if (this.moduleData != null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisServiceRetyCat, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisServiceRetyCat, 0, 0, listArgs);
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btn_PhongXuLy_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE row = data as V_HIS_SERVICE;
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    if (this.moduleData != null)
                    {
                        CallModule callModule = new CallModule(CallModule.RoomService, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.RoomService, 0, 0, listArgs);
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<object> listArgs = new List<object>();
                listArgs.Add(this.moduleData);
                listArgs.Add((RefeshReference)LoaddataToTreeList);

                if (this.moduleData != null)
                {
                    CallModule callModule = new CallModule(CallModule.HisImportService, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                }
                else
                {
                    CallModule callModule = new CallModule(CallModule.HisImportService, 0, 0, listArgs);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spNumberfilm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboFILM_SIZE.Enabled)
                    {
                        cboFILM_SIZE.Focus();
                        cboFILM_SIZE.SelectAll();
                    }
                    else if (cboPetroleum.Enabled)
                    {
                        cboPetroleum.Focus();
                        cboPetroleum.SelectAll();
                    }
                    else
                    {
                        spMinProcess.Focus();
                        spMinProcess.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spMinDuration_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinWARNING.Focus();
                    spinWARNING.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_PATY)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(V_HIS_SERVICE_ROOM)).ToString(), false);
                BackendDataWorker.CacheMonitorSyncExecute((typeof(HIS_SERVICE)).ToString(), false);

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, new CommonParam(), true);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMayCuaToi_Click(object sender, EventArgs e)
        {
            try
            {
                BackendDataWorker.Reset<V_HIS_SERVICE>();
                BackendDataWorker.Reset<HIS_SERVICE>();
                BackendDataWorker.Reset<V_HIS_SERVICE_PATY>();
                BackendDataWorker.Reset<V_HIS_SERVICE_ROOM>();
                HIS.Desktop.LocalStorage.BackendData.Core.ServiceCombo.ServiceComboDataWorker.DicServiceCombo = new Dictionary<long, LocalStorage.BackendData.ADO.ServiceComboADO>();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, new CommonParam(), true);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIcdCm_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboIcdCm.EditValue != null)
                    {
                        var data = BackendDataWorker.Get<HIS_ICD_CM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboIcdCm.EditValue ?? "").ToString()));
                        if (data != null)
                        {
                            txtIcdCm.Text = data.ICD_CM_CODE;
                            txtIcdCm.Focus();
                            txtIcdCm.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIcdCm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (string.IsNullOrEmpty(txtIcdCm.Text))
                    {
                        cboIcdCm.EditValue = null;
                        cboIcdCm.Focus();
                        cboIcdCm.ShowPopup();
                    }
                    else
                    {
                        List<HIS_ICD_CM> searchs = null;
                        var listData1 = BackendDataWorker.Get<HIS_ICD_CM>().Where(o => o.ICD_CM_CODE.ToUpper().Contains(txtIcdCm.Text.ToUpper())).ToList();
                        if (listData1 != null && listData1.Count > 0)
                        {
                            searchs = (listData1.Count == 1) ? listData1 : (listData1.Where(o => o.ICD_CM_CODE.ToUpper() == txtIcdCm.Text.ToUpper()).ToList());
                        }
                        if (searchs != null && searchs.Count == 1)
                        {
                            txtIcdCm.Text = searchs[0].ICD_CM_CODE;
                            cboIcdCm.EditValue = searchs[0].ID;
                            spHeinOld.Focus();
                            spHeinOld.SelectAll();
                        }
                        else
                        {
                            cboIcdCm.Focus();
                            cboIcdCm.ShowPopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore
                  (HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                  Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                  HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(MPS.Processor.Mps000256.PDO.Mps000256PDO.printTypeCode, DelegateRunPrinterExportExcel);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterExportExcel(string printCode, string fileName)
        {
            bool result = true;
            try
            {
                string savePath = null;
                if (SaveFileExportExcel.ShowDialog() == DialogResult.OK)
                {
                    savePath = SaveFileExportExcel.FileName;
                }
                else
                {
                    return false;
                }

                WaitingManager.Show();
                if (!string.IsNullOrWhiteSpace(savePath))
                {
                    MPS.Processor.Mps000256.PDO.Mps000256ADO mps000256ADO = new MPS.Processor.Mps000256.PDO.Mps000256ADO();
                    mps000256ADO.PATIENT_TYPE_ID__BHYT = Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                    mps000256ADO.PATIENT_TYPE_ID__VP = Config.HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                    ThreadGetDataService();

                    MPS.Processor.Mps000256.PDO.Mps000256PDO mps000256PDO = new MPS.Processor.Mps000256.PDO.Mps000256PDO(listVServiceExport, listVServicePatyExport, this.listPackage, this.listPtttMethod, this.listPtttGroup, mps000256ADO);

                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(MPS.Processor.Mps000256.PDO.Mps000256PDO.printTypeCode, fileName, mps000256PDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, savePath + ".xlsx");
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printCode, moduleData != null ? (long?)moduleData.RoomId : null);

                    PrintData.EmrInputADO = inputADO;

                    result = MPS.MpsPrinter.Run(PrintData);
                    WaitingManager.Hide();
                    if (result)
                        MessageManager.Show(Resources.ResourceMessage.HisService__ExportExcel__ThanhCong);
                    else
                        MessageManager.Show(Resources.ResourceMessage.HisService__ExportExcel__ThatBai);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void ThreadGetDataService()
        {
            Thread service = new Thread(GetServiceDVKT);
            Thread servicePaty = new Thread(GetServicePatyDVKT);
            try
            {
                service.Start();
                servicePaty.Start();

                service.Join();
                servicePaty.Join();
            }
            catch (Exception ex)
            {
                service.Abort();
                servicePaty.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServiceDVKT()
        {
            try
            {
                var param = new CommonParam();
                HisServiceViewFilter filter = new HisServiceViewFilter();

                if (cboSearchType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboSearchType.EditValue.ToString() ?? ""));
                }
                filter.IS_ACTIVE = 1;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var apiResult = new BackendAdapter(param).GetStrong<List<V_HIS_SERVICE>>("api/HisService/GetView", ApiConsumer.ApiConsumers.MosConsumer, param, filter, 0, null);
                if (apiResult != null && apiResult.Count > 0)
                {
                    listVServiceExport = apiResult.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetServicePatyDVKT()
        {
            try
            {
                var param = new CommonParam();
                HisServicePatyViewFilter filter = new HisServicePatyViewFilter();

                if (cboSearchType.EditValue != null)
                {
                    filter.SERVICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboSearchType.EditValue.ToString() ?? ""));
                }
                filter.IS_ACTIVE = 1;
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                var apiResult = new BackendAdapter(param).GetStrong<List<V_HIS_SERVICE_PATY>>("api/HisServicePaty/GetView", ApiConsumer.ApiConsumers.MosConsumer, param, filter, 0, null);
                if (apiResult != null && apiResult.Count > 0)
                {
                    listVServicePatyExport = apiResult.Where(o => o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spMaxExpend_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Enter)
                {
                    cboFILM_SIZE.Focus();
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNutritionTime_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (MOS.EFMODEL.DataModels.HIS_RATION_TIME rv in gridCheckMark.Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.RATION_TIME_NAME.ToString());
                    }
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btn_ServiceRaty_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                var data = treeList1.GetDataRecordByNode(treeList1.FocusedNode);
                V_HIS_SERVICE rowData = data as V_HIS_SERVICE;
                HIS.Desktop.Plugins.HisRefectory.frmServiceRati.frmServiceRati frm = new HIS.Desktop.Plugins.HisRefectory.frmServiceRati.frmServiceRati(rowData.ID);

                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick_2(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboExeServiceModule.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExeServiceModule_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGender.Focus();
                }
            }
            catch (Exception ex)
            {
                LogSession.Warn(ex);
            }
        }

        private void cboGender_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtCapacity.Focus();
                    txtCapacity.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboGender.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboGender.EditValue != null)
                {
                    cboGender.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboGender.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeList1_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                if (tree != null)
                {
                    Point pt = tree.PointToClient(MousePosition);
                    TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Point);
                    if (hitInfo != null && (hitInfo.HitInfoType == HitInfoType.Row
                        || hitInfo.HitInfoType == HitInfoType.Cell))
                    {
                        e.Menu.Items.Clear();
                        tree.FocusedNode = hitInfo.Node;
                        var data = treeList1.GetDataRecordByNode(hitInfo.Node);
                        if (data != null && data is V_HIS_SERVICE)
                        {
                            this.currentRightClick = (V_HIS_SERVICE)data;
                            if (this.currentRightClick.IS_ACTIVE == 1)
                            {
                                DXMenuItem item = new DXMenuItem();
                                item.Caption = Inventec.Common.Resource.Get.Value("UC_HisService.tolPopup.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                item.Click += OnRightClick;
                                e.Menu.Items.Add(item);
                                e.Menu.Show(pt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnRightClick(object sender, EventArgs e)
        {

        }

        private void spAgeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spAgeTo.Focus();
                    spAgeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spAgeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboExeServiceModule.Focus();
                    if (cboExeServiceModule.EditValue == null)
                    {
                        cboExeServiceModule.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSearchType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    chkLock.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEditServiceType_Click(object sender, EventArgs e)
        {
            try
            {
                cboServiceType.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNotice_KeyUp(object sender, KeyEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsKidney_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAntibioticResistance.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSearchType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboSearchType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCapacity_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsKidney.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboType.EditValue != null)
                {
                    cboChiSo.Properties.Buttons[2].Visible = false;
                    if ((long)cboType.EditValue == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__CDHA || (long)cboType.EditValue == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__TDCN)
                    {
                        cboChiSo.Enabled = true;
                        if (currentData != null && currentData.ID > 0)
                            cboChiSo.Properties.Buttons[2].Visible = true;
                    }
                    else
                    {
                        cboChiSo.Enabled = false;
                        cboChiSo.EditValue = null;
                        cboChiSo.Properties.Buttons[2].Visible = false;
                        GridCheckMarksSelection gridCheckMarkChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkChiSo.ClearSelection(cboChiSo.Properties.View);
                    }

                    if ((long)cboType.EditValue == IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VC)
                    {
                        cboPetroleum.Enabled = true;
                    }
                    else
                    {
                        cboPetroleum.Enabled = false;
                        cboPetroleum.EditValue = null;
                    }
                }
                if (cboType.EditValue == null)
                {
                    cboChiSo.Enabled = false;
                    cboChiSo.EditValue = null;
                    cboPetroleum.Enabled = false;
                    cboPetroleum.EditValue = null;
                    cboChiSo.Properties.Buttons[2].Visible = false;
                    GridCheckMarksSelection gridCheckMarkChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkChiSo.ClearSelection(cboChiSo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRationGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRationGroup.Properties.Buttons[1].Visible = true;
                    cboRationGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRatioSymbol_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spCogs.Focus();
                    spCogs.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRationGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spNumOrder.Focus();
                    spNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAntibioticResistance_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsOtherSourcePaid.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboGender_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPart.Focus();
                    cboPart.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ---Combobox Diim Type
        private void cboDiimType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDiimType.EditValue = null;
                    cboDiimType.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDiimType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDiimType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDiimType.EditValue != null && cboDiimType.EditValue != cboDiimType.OldEditValue)
                    {
                        var data = BackendDataWorker.Get<HIS_DIIM_TYPE>().FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDiimType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboDiimType.Properties.Buttons[1].Visible = true;
                            cboFuexType.Focus();
                            cboFuexType.SelectAll();
                        }
                    }
                    else
                    {
                        cboFuexType.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Combobox Fuex type
        private void cboFuexType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboFuexType.EditValue = null;
                    cboFuexType.Properties.Buttons[1].Visible = false;
                    if (spNumberfilm.Enabled)
                    {
                        spNumberfilm.Focus();
                        spNumberfilm.SelectAll();
                    }
                    else
                    {
                        spEstimateDuration.Focus();
                        spEstimateDuration.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFuexType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboFuexType.EditValue != null && cboFuexType.EditValue != cboFuexType.OldEditValue)
                    {
                        var data = BackendDataWorker.Get<HIS_FUEX_TYPE>().FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboFuexType.EditValue.ToString()));
                        if (data != null)
                        {
                            cboFuexType.Properties.Buttons[1].Visible = true;
                            if (CboTestType.Enabled)
                            {
                                CboTestType.Focus();
                                CboTestType.ShowPopup();
                            }
                            else if (spNumberfilm.Enabled)
                            {
                                spNumberfilm.Focus();
                                spNumberfilm.SelectAll();
                            }
                            else
                            {
                                spEstimateDuration.Focus();
                                spEstimateDuration.SelectAll();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFuexType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (CboTestType.Enabled)
                    {
                        CboTestType.Focus();
                        CboTestType.ShowPopup();
                    }
                    else if (spNumberfilm.Enabled)
                    {
                        spNumberfilm.Focus();
                        spNumberfilm.SelectAll();
                    }
                    else
                    {
                        spEstimateDuration.Focus();
                        spEstimateDuration.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboFuexType()
        {
            try
            {
                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("FUEX_TYPE_CODE", "", 100, 1));
                colum.Add(new ColumnInfo("FUEX_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditoADO = new ControlEditorADO("FUEX_TYPE_NAME", "ID", colum, false, 350);
                ControlEditorLoader.Load(cboFuexType, BackendDataWorker.Get<HIS_FUEX_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList(), controlEditoADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboTaxRateType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtProcessCode.Focus();
                    txtProcessCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTaxRateType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboTaxRateType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkChiDinh_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkChiDinh.Checked)
                {
                    chkTachdichvu.Enabled = true;
                }
                else
                {
                    chkTachdichvu.Enabled = false;
                    chkSplitTreatment.EditValue = null;
                    chkTachdichvu.EditValue = null;
                    chkTachdichvu.Checked = false;
                    chkSplitTreatment.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboExeServiceModule_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                cboGender.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkForPrice_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNotice.Focus();
                    txtNotice.SelectAll();
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
                if (e.KeyCode == Keys.Enter)
                {
                    cboTaxRateType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtProcessCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkUpdateAll.Enabled)
                    {
                        chkUpdateAll.Focus();
                    }
                    else
                    {
                        if (e.KeyCode == Keys.Enter)
                        {
                            if (this.ActionType == GlobalVariables.ActionAdd)
                            {
                                btnAdd.Focus();
                            }
                            else
                                btnEdit.Focus();
                        }
                        if (e.KeyCode == Keys.Space)
                        {
                            if (chkUpdateOnly.Enabled)
                                chkUpdateOnly.Checked = !chkUpdateOnly.Checked;
                        }
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboTestType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboTestType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTestType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (CboTestType.EditValue != null)
                {
                    CboTestType.Properties.Buttons[1].Visible = true;
                }
                else
                    CboTestType.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTestType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (spMaxExpend.Enabled)
                    {
                        spMaxExpend.Focus();
                        spMaxExpend.SelectAll();
                    }
                    else
                    {
                        spEstimateDuration.Focus();
                        spEstimateDuration.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboTestType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spMaxExpend.Enabled == true)
                    {
                        spMaxExpend.Focus();
                        spMaxExpend.SelectAll();
                    }
                    else
                    {
                        cboExeServiceModule.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spMinProcess_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnMaxProcessTime.Focus();
                    spnMaxProcessTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                chkNotChangePaty.Enabled = cboPatientType.EditValue != null;
                chkNotChangePaty.Checked = cboPatientType.EditValue != null ? chkNotChangePaty.Checked : false;
                if (cboPatientType.EditValue == null)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboAppliedPatientType.Properties.View);
                    cboAppliedPatientType.EditValue = null;
                }
                cboAppliedPatientType.Enabled = cboPatientType.EditValue != null;

                if (cboPatientType.EditValue == null)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboPatientClassify.Properties.View);
                    cboPatientClassify.EditValue = null;
                }
                cboPatientClassify.Enabled = cboPatientType.EditValue != null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                chkNotChangePaty.Enabled = cboPatientType.EditValue != null;
                chkNotChangePaty.Checked = cboPatientType.EditValue != null ? chkNotChangePaty.Checked : false;
                if (cboPatientType.EditValue == null)
                {
                    GridCheckMarksSelection gridcheckDttt1 = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                    gridcheckDttt1.ClearSelection(cboDTTT1.Properties.View);
                    cboDTTT1.EditValue = null;
                }
                cboDTTT1.Enabled = cboPatientType.EditValue != null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT2_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                chkNotChangePaty.Enabled = cboPatientType.EditValue != null;
                chkNotChangePaty.Checked = cboPatientType.EditValue != null ? chkNotChangePaty.Checked : false;
                if (cboPatientType.EditValue == null)
                {
                    GridCheckMarksSelection gridcheckDttt2 = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                    gridcheckDttt2.ClearSelection(cboDTTT2.Properties.View);
                    cboDTTT2.EditValue = null;
                }
                cboDTTT2.Enabled = cboPatientType.EditValue != null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsKidney_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkChiDinh.Enabled == true)
                    {
                        chkChiDinh.Focus();
                    }
                    else
                    {
                        chkCPNgoaiGoi.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAntibioticResistance_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkSplitTreatment.Enabled == true)
                    {
                        chkSplitTreatment.Focus();
                    }
                    else
                    {
                        chkAllowExpend.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboOTHER_PAY_SOURCE_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboOTHER_PAY_SOURCE.EditValue != null)
                {
                    cboOTHER_PAY_SOURCE.Properties.Buttons[1].Visible = true;
                    txtOTHER_PAY_SOURCE.Enabled = true;
                }
                else
                {
                    cboOTHER_PAY_SOURCE.Properties.Buttons[1].Visible = false;
                    txtOTHER_PAY_SOURCE.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOTHER_PAY_SOURCE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboOTHER_PAY_SOURCE.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboOTHER_PAY_SOURCE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboOTHER_PAY_SOURCE.EditValue != null)
                    {
                        txtOTHER_PAY_SOURCE.Focus();
                        txtOTHER_PAY_SOURCE.SelectAll();
                    }
                    else
                    {
                        txtNotice.Focus();
                        txtNotice.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtOTHER_PAY_SOURCE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAttachAssignPrintTypeCode.Focus();
                    cboAttachAssignPrintTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPart_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPart.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboPart.Properties.View);
                    cboPart.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPart_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCapacity.Focus();
                    txtCapacity.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPart_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_BODY_PART rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.BODY_PART_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTestingTechnique_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDefaultPatientType.Focus();
                    if (cboDefaultPatientType.EditValue == null)
                    {
                        cboDefaultPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboChiSo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboChiSo.Properties.View);
                    cboChiSo.EditValue = null;
                    listHisSuimIndexChoose = null;
                }
                else if (e.Button.Kind == ButtonPredefines.Ellipsis)
                {
                    frmServiceIndex frm = new frmServiceIndex(listData =>
                    {
                        GridCheckMarksSelection gridCheckMarkChiSo = cboChiSo.Properties.Tag as GridCheckMarksSelection;
                        gridCheckMarkChiSo.ClearSelection(cboChiSo.Properties.View);
                        listHisSuimIndexChoose = new List<HIS_SUIM_INDEX>();
                        foreach (var item in listData)
                        {
                            HIS_SUIM_INDEX updateDTO = new HIS_SUIM_INDEX();
                            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SUIM_INDEX>(updateDTO, item);
                            listHisSuimIndexChoose.Add(updateDTO);
                        }
                        if (listHisSuimIndexChoose != null && listHisSuimIndexChoose.Count > 0)
                        {
                            List<HIS_SUIM_INDEX> selects = new List<HIS_SUIM_INDEX>();
                            foreach (var item in listHisSuimIndexChoose)
                            {
                                var row = listHisSuimIndex != null ? listHisSuimIndex.FirstOrDefault(o => o.ID == item.ID) : null;
                                if (row != null)
                                {
                                    selects.Add(row);
                                }
                            }
                            gridCheckMarkChiSo.SelectAll(selects);
                        }
                        else
                        {
                            gridCheckMarkChiSo.ClearSelection(cboChiSo.Properties.View);
                            cboChiSo.EditValue = null;
                        }
                    }, listHisSuimIndexChoose, txtMisuSerTypeCode.Text.Trim(), txtMisuSerTypeName.Text.Trim());
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAttachAssignPrintTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNotice.Focus();
                    txtNotice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAttachAssignPrintTypeCode_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtNotice.Focus();
                    txtNotice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAttachAssignPrintTypeCode_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAttachAssignPrintTypeCode.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFILM_SIZE_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboFILM_SIZE.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFILM_SIZE_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPetroleum.Enabled)
                    {
                        cboPetroleum.Focus();
                        cboPetroleum.SelectAll();
                    }
                    else
                    {
                        spMinProcess.Focus();
                        spMinProcess.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAllowExpend_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkAllowExpend.Checked)
                {
                    chkIS_AUTO_EXPEND.Enabled = true;
                }
                else
                {
                    chkIS_AUTO_EXPEND.Enabled = false;
                    chkIS_AUTO_EXPEND.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void spinWARNING_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spAgeFrom.Focus();
                    spAgeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAppliedPatientType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboPatientClassify_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_PATIENT_CLASSIFY rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_CLASSIFY_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboAppliedPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboAppliedPatientType.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboAppliedPatientType.Properties.View);
                    //cboAppliedPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboPatientClassify_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboPatientClassify.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboPatientClassify.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboDefaultPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDefaultPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDTTT1_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDTTT1.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboDTTT1.Properties.View);
                    cboDTTT1.EditValue = null;
                    cboDTTT1.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT2_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDTTT2.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboDTTT2.Properties.View);
                    cboDTTT2.EditValue = null;
                    cboDTTT2.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT3_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDTTT3.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboDTTT3.Properties.View);
                    cboDTTT3.EditValue = null;
                    cboDTTT3.Text = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT1_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT2_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDTTT3_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_PATIENT_TYPE rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.PATIENT_TYPE_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void cboDTTT1_Closed(object sender, ClosedEventArgs e)
        {
            cboDTTT1.Focus();
        }

        private void cboDTTT2_Closed(object sender, ClosedEventArgs e)
        {
            cboDTTT2.Focus();
        }

        private void cboDTTT3_Closed(object sender, ClosedEventArgs e)
        {
            cboDTTT3.Focus();
        }
        private void spinMaxAmount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spMinDuration.Focus();
                    spMinDuration.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spMaxTotalProcessTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinMaxAmount.Focus();
                    spinMaxAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPetroleum_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spMinProcess.Enabled == true)
                    {
                        spMinProcess.Focus();
                        spMinProcess.SelectAll();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPetroleum_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboPetroleum.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboSampleType.Properties.Buttons[1].Visible = cboSampleType.EditValue != null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleType_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboSampleType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientClassify_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAppliedPatientType.Focus();
                    if (cboAppliedPatientType.EditValue == null)
                    {
                        cboAppliedPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboAppliedPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestingTechnique.Focus();
                    txtTestingTechnique.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboDefaultPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboGroup.Focus();
                    if (cboGroup.EditValue == null)
                    {
                        cboGroup.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void cboChiSo_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                listHisSuimIndexChoose = new List<HIS_SUIM_INDEX>();
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_SUIM_INDEX rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.SUIM_INDEX_NAME.ToString());
                    listHisSuimIndexChoose.Add(rv);
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<HIS_EMR_FORM> listEmrForm = new List<HIS_EMR_FORM>();
        GridCheckMarksSelection selection = new GridCheckMarksSelection();
        List<HIS_EMR_FORM> lstSelectedEmr = new List<HIS_EMR_FORM>();
        public string EMR_FORM_CODES { get; set; }
        private void LoadDataCboPhieuBA()
        {
            try
            {
                listEmrForm = BackendDataWorker.Get<HIS_EMR_FORM>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EMR_FORM_CODE", "Mã", 100, 1));
                columnInfos.Add(new ColumnInfo("EMR_FORM_NAME", "Tên", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EMR_FORM_NAME", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(cboPhieuBA, listEmrForm, controlEditorADO);
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPhieuBA.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(Event_Check_PhieuBA);
                cboPhieuBA.Properties.Tag = gridCheck;
                cboPhieuBA.Properties.View.OptionsSelection.MultiSelect = true;
                cboPhieuBA.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void Event_Check_PhieuBA(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    this.lstSelectedEmr.Clear();
                    foreach (HIS_EMR_FORM er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.EMR_FORM_NAME);
                            lstSelectedEmr.Add(er);
                        }
                    }
                    EMR_FORM_CODES = string.Join(",", lstSelectedEmr.Select(s => s.EMR_FORM_CODE).ToList());
                    cboPhieuBA.EditValue = lstSelectedEmr;
                }
                this.cboPhieuBA.Text = sb.ToString();
                

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        

        private void cboPhieuBA_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
                {

                    cboPhieuBA.ShowPopup();
                }
                else if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {

                    cboPhieuBA.EditValue = null;
                    this.EMR_FORM_CODES = null;
                    GridCheckMarksSelection gridCheckMark = cboPhieuBA.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboPhieuBA.Properties.View);
                    }
                    this.cboPhieuBA.Focus();


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuBA_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = string.Join(",", lstSelectedEmr.Select(s => s.EMR_FORM_NAME).ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhieuBA_Leave(object sender, EventArgs e)
        {
            btnAdd.Focus();
        }

        private void cboPhieuBA_Closed(object sender, ClosedEventArgs e)
        {
            btnAdd.Focus();
        }
    }
}


