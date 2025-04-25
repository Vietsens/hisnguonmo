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
using DevExpress.XtraBars.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisExecuteRoom.ADO;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Markup;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

namespace HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom
{
    public partial class frmHisExecuteRoom : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        private const short IS_ACTIVE_TRUE = 1;
        private const short IS_ACTIVE_FALSE = 0;
        MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        internal long roomId;
        internal long executeRoomId;
        List<HIS_AREA> listArea;
        List<ACS_MODULE> listAcsModule;
        List<ACS_USER> listDirector = new List<ACS_USER>();
        internal List<ADO.DirectorADO> listDirectors { get; set; }
        HIS_DEPARTMENT department = new HIS_DEPARTMENT();
        List<HIS_MEDI_STOCK> defaultDrugSelecteds;
        List<HIS_PATIENT_TYPE> listPatientTypeIds = new List<HIS_PATIENT_TYPE>();
        #endregion

        #region Construct
        public frmHisExecuteRoom(Inventec.Desktop.Common.Modules.Module moduleData)
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
        private void frmHisExecuteRoom_Load(object sender, EventArgs e)
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExecuteRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExecuteRoom.HisExecuteRoom.frmHisExecuteRoom).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnEdit.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumnEdit.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomCode.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColExecuteRoomCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColExecuteRoomCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomName.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColExecuteRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExecuteRoomName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColExecuteRoomName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCoRoomGroup.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdCoRoomGroup.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCoRoomGroup.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdCoRoomGroup.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomId.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColRoomId.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRoomId.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColRoomId.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsEmergency.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsEmergency.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsEmergency.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsEmergency.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsSpeciality.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsSpeciality.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsSpeciality.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsSpeciality.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsSurgery.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsSurgery.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsSurgery.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsSurgery.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclKiosk.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grclKiosk.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsExam.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsExam.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsExam.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsPauseEnclitic.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColIsPauseEnclitic.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxRequestByDay.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColMaxRequestByDay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxRequestByDay.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColMaxRequestByDay.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.gridColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grclOrderIssueCode.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grclOrderIssueCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());


                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkVitaminA.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.checkVitaminA.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkVaccine2.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkVaccine2.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkKhongCanChonDV.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkKhongCanChonDV.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsKidney.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsKidney.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsPauseEnclitic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsPauseEnclitic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsPauseEnclitic.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsPauseEnclitic.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboChuyenKhoa.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.cboChuyenKhoa.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkRestrictExecuteRoom.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkRestrictExecuteRoom.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkRestrictExecuteRoom.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkRestrictExecuteRoom.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkRestrictMedicineType.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkRestrictMedicineType.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkRestrictMedicineType.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkRestrictMedicineType.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkRestrictTime.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkRestrictTime.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkRestrictTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkRestrictTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEdit1.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.checkEdit1.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cbbRoomGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.cbbRoomGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lkRoomId.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lkRoomId.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSpeciality.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsSpeciality.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsPause.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsPause.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsEmergency.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsEmergency.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSurgery.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsSurgery.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsExam.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.chkIsExam.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExecuteRoomCode.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciExecuteRoomCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExecuteRoomName.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciExecuteRoomName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsEmergency.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsEmergency.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsSpeciality.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsSpeciality.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem11.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem13.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem14.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsSurgery.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsSurgery.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsExam.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsExam.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsKidney.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsKidney.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciKidneyCount.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciKidneyCount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsPause.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsPause.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem12.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaxRequestByDay.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciMaxRequestByDay.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem15.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem21.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem21.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsPauseEnclitic.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsPauseEnclitic.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsPauseEnclitic.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsPauseEnclitic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem23.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem25.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem28.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem28.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.layoutControlItem28.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcArea.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lcArea.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColArea.Caption = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColArea.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColArea.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.grdColArea.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcTestTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lcTestTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcTestTypeCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("lcTestTypeCode.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsBlockNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsBlockNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsBlockNumOrder.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExecuteRoom.lciIsBlockNumOrder.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                txtKeyword.Text = "";
                spMaxRequestByDay.EditValue = null;
                spSTT.EditValue = null;
                spHoldOrder.EditValue = null;
                spMaxReqBhytByDay.EditValue = null;
                chkIsKidney.CheckState = CheckState.Unchecked;
                spinKidneyCount.EditValue = null;
                spinKidneyCount.Enabled = false;
                lkRoomId.EditValue = null;
                cboWaitingScreen.EditValue = null;
                cboAccountBook.EditValue = null;
                txtJsonQr.Text = null;
                if (cboDefaultDrug.EditValue != null)
                {
                    cboDefaultDrug.EditValue = null;
                    GridCheckMarksSelection gridCheckMarkPart = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPart.ClearSelection(cboDefaultDrug.Properties.View);
                    cboDefaultDrug.Text = "";

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
                dicOrderTabIndexControl.Add("txtExecuteRoomCode", 0);
                dicOrderTabIndexControl.Add("txtExecuteRoomName", 1);
                dicOrderTabIndexControl.Add("lkRoomId", 2);
                dicOrderTabIndexControl.Add("chkIsEmergency", 3);
                dicOrderTabIndexControl.Add("chkIsSpeciality", 4);
                dicOrderTabIndexControl.Add("chkIsSurgery", 5);
                dicOrderTabIndexControl.Add("spMaxRequestByDay", 6);
                dicOrderTabIndexControl.Add("chkIsExam", 7);
                dicOrderTabIndexControl.Add("checkEdit1", 8);
                dicOrderTabIndexControl.Add("cbbRoomGroup", 9);
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
                InitComboDepartment();
                InitComboRoomGroup();
                InitComboSpeciality();
                InitComboUser();
                InitComboDefaultDrug();
                InitComboCashRoom();
                InitComboArea();
                InitComboWaitingScreen();
                //
                InitComboDepositBook();
                InitComboAccountBook();
                InitComboAccountBookQr();
                //
                InitComboDefaultService();
                InitComboDefaultsCLS();
                //
                LoadResQrInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboAccountBookQr()
        {
            try
            {
                cboAccountQr.EditValue = null;
                CommonParam param = new CommonParam();
                HisAccountBookFilter filter = new HisAccountBookFilter();
                filter.IS_ACTIVE = 1;
                filter.FOR_DEPOSIT = true;
                filter.FOR_BILL = true;
                var data = new BackendAdapter(param).Get<List<HIS_ACCOUNT_BOOK>>("api/HisAccountBook/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                if (data != null && data.Count > 0)
                {
                    data = data.Where(o => o.IS_NOT_GEN_TRANSACTION_ORDER != 1).ToList();
                    List<ColumnInfo> colum = new List<ColumnInfo>();
                    colum.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                    colum.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 200, 2));
                    ControlEditorADO controlADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", colum, false, 300);
                    ControlEditorLoader.Load(cboAccountQr, data, controlADO);
                    cboAccountQr.Properties.ImmediatePopup = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public void LoadDirectorData()
        {
            //cboDirectorUserName.Properties.DataSource = listDirectors;
        }
        private void LoadResQrInfo()
        {

            try
            {
                var listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode")).ToList();
                List<ConfigADO> data = new List<ConfigADO>();
                foreach (var item in listConfig)
                {
                    string key = "";
                    string value = item.KEY;
                    int index = value.IndexOf("Info");
                    if (index > 0)
                    {
                        var shotkey = value.Substring(0, index);
                        string[] parts = shotkey.Split('.');
                        if (parts.Length > 0)
                        {
                            key = parts[parts.Length - 1];
                        }
                    }
                    else
                    {
                        key = item.KEY;
                    }
                    data.Add(new ConfigADO() { ID_CONFIG = key });
                }
                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("ID_CONFIG", "", 100, 1));
                ControlEditorADO controlADO = new ControlEditorADO("ID_CONFIG", "ID_CONFIG", colum, false, 100);
                ControlEditorLoader.Load(repConfig, data, controlADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        #region Init combo
        private void InitComboDefaultDrug()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDefaultDrug.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboDefaultDrug);
                cboDefaultDrug.Properties.Tag = gridCheck;
                cboDefaultDrug.Properties.View.OptionsSelection.MultiSelect = true;

                CommonParam param = new CommonParam();
                HisRoomGroupFilter filter = new HisRoomGroupFilter();
                //List<HIS_MEDI_STOCK> data = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumers.MosConsumer, filter, param);
                var data = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>();
                if (data == null)
                    return;
                data = data.Where(me => me.IS_BUSINESS == 1).ToList();
                if (data != null && data.Count > 0)
                {
                    cboDefaultDrug.Properties.DataSource = data;
                    cboDefaultDrug.Properties.DisplayMember = "MEDI_STOCK_NAME";
                    cboDefaultDrug.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboDefaultDrug.Properties.View.Columns.AddField("MEDI_STOCK_CODE");

                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "Mã";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboDefaultDrug.Properties.View.Columns.AddField("MEDI_STOCK_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "Tên";

                    cboDefaultDrug.Properties.PopupFormWidth = 200;
                    cboDefaultDrug.Properties.View.OptionsView.ShowColumnHeaders = true;
                    cboDefaultDrug.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDefaultDrug.Properties.View);
                    }
                }

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("MEDI_STOCK_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("MEDI_STOCK_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboDefaultDrug, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboDefaultDrug(object sender, EventArgs e)
        {
            try
            {
                string typeName = "";
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                defaultDrugSelecteds = new List<HIS_MEDI_STOCK>();
                if (gridCheckMark != null)
                {
                    List<HIS_MEDI_STOCK> erSelectedNews = new List<HIS_MEDI_STOCK>();
                    foreach (HIS_MEDI_STOCK er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er == null)
                            continue;
                        typeName += er.MEDI_STOCK_NAME + ",";
                    }
                    cboDefaultDrug.Text = typeName;
                    cboDefaultDrug.ToolTip = typeName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCashRoom()
        {
            try
            {
                List<V_HIS_CASHIER_ROOM> data = null;
                if (department != null)
                {
                    CommonParam param = new CommonParam();
                    HisCashierRoomViewFilter filter = new HisCashierRoomViewFilter();
                    filter.IS_ACTIVE = 1;
                    filter.BRANCH_ID = department.BRANCH_ID;

                    data = new BackendAdapter(param).Get<List<V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, param);
                }
                //data = data.Where(me => me.IS_BUSINESS == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("CASHIER_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("CASHIER_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCashRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(lkRoomId, BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDefaultsCLS()
        {
            try
            {
                HIS_PATIENT_TYPE currentDefaultsCLS = new HIS_PATIENT_TYPE();

                CommonParam param = new CommonParam();
                HisPatientFilter filter = new HisPatientFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                listPatientTypeIds = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE>>("api/HisPatientType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                var filteredList = listPatientTypeIds.Select(item => new
                {
                    ID = item.ID,
                    PATIENT_TYPE_CODE = item.PATIENT_TYPE_CODE,
                    PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME
                }).ToList();
                //var filteredList = listPatientTypeIds.Select(item => item.PATIENT_TYPE_NAME).ToList();

                if (filteredList != null)
                {

                    cboDefaultsCLS.Properties.DataSource = filteredList;
                    cboDefaultsCLS.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                    cboDefaultsCLS.Properties.ValueMember = "ID";

                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboDefaultsCLS.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                    col2.VisibleIndex = 1;
                    col2.Width = 100;
                    col2.Caption = "";
                    DevExpress.XtraGrid.Columns.GridColumn col3 = cboDefaultsCLS.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                    col3.VisibleIndex = 2;
                    col3.Width = 200;
                    col3.Caption = "";
                }
                cboDefaultsCLS.Properties.View.OptionsView.ShowColumnHeaders = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }


        private void InitComboRoomGroup()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisRoomGroupFilter filter = new HisRoomGroupFilter();
                List<HIS_ROOM_GROUP> data = new BackendAdapter(param).Get<List<HIS_ROOM_GROUP>>("api/HisRoomGroup/Get", ApiConsumers.MosConsumer, filter, param);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_GROUP_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_GROUP_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_GROUP_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cbbRoomGroup, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSpeciality()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSpecialityFilter filter = new HisSpecialityFilter();
                filter.IS_ACTIVE = 1;
                List<HIS_SPECIALITY> data = new BackendAdapter(param).Get<List<HIS_SPECIALITY>>("api/HisSpeciality/Get", ApiConsumers.MosConsumer, filter, param);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SPECIALITY_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("SPECIALITY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SPECIALITY_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboChuyenKhoa, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboUser()
        {
            try
            {
                List<ACS.EFMODEL.DataModels.ACS_USER> data = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
                if (data == null) return;

                data = data.Where(o => o.IS_ACTIVE == 1).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 350);
                ControlEditorLoader.Load(CboResponsible, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboArea()
        {
            try
            {
                cboArea.EditValue = null;
                cboArea.Properties.Buttons[1].Visible = false;
                List<HIS_AREA> data_area = new List<HIS_AREA>();
                if (lkRoomId.EditValue != null)
                {
                    data_area = this.listArea.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && (o.DEPARTMENT_ID == Inventec.Common.TypeConvert.Parse.ToInt64((lkRoomId.EditValue ?? 0).ToString()) || o.DEPARTMENT_ID == null)).ToList();
                }

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("AREA_CODE", "", 100, 1));
                colum.Add(new ColumnInfo("AREA_NAME", "", 200, 2));
                ControlEditorADO controlADO = new ControlEditorADO("AREA_NAME", "ID", colum, false, 300);
                ControlEditorLoader.Load(cboArea, data_area, controlADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void InitComboWaitingScreen()
        {
            try
            {
                cboWaitingScreen.EditValue = null;
                cboWaitingScreen.Properties.Buttons[1].Visible = false;
                List<ACS_MODULE> data_module = new List<ACS_MODULE>();

                CommonParam param = new CommonParam();
                AcsModuleGroupFilter filter = new AcsModuleGroupFilter();
                var resultData = new BackendAdapter(param).Get<List<ACS_MODULE_GROUP>>("api/AcsModuleGroup/Get", ApiConsumers.AcsConsumer, filter, param);
                var MHCId = resultData.Where(o => o.MODULE_GROUP_CODE == "MHC").FirstOrDefault().ID;
                Inventec.Common.Logging.LogSystem.Debug("MHCId" + MHCId);
                AcsModuleFilter filterModule = new AcsModuleFilter();
                listAcsModule = new BackendAdapter(param).Get<List<ACS_MODULE>>("api/AcsModule/Get", ApiConsumers.AcsConsumer, filterModule, null).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAcsModule), listAcsModule));
                if (listAcsModule != null && listAcsModule.Count > 0)
                {
                    data_module = this.listAcsModule.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.MODULE_GROUP_ID == MHCId).ToList();
                }
                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("MODULE_NAME", "", 300, 1));
                ControlEditorADO controlADO = new ControlEditorADO("MODULE_NAME", "MODULE_LINK", colum, false, 300);
                ControlEditorLoader.Load(cboWaitingScreen, data_module, controlADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
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
                txtHein_card_number.Text = null;
                txtHein_card_number.Clear();
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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>> apiResult = null;
                HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                SetFilterNavBar(ref filter);
                dnNavigation.DataSource = null;
                gridControlFormList.DataSource = null;
                //gridviewFormList.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>(HisRequestUriStore.MOSV_HIS_EXECUTE_ROOM_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
                        gridviewFormList.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridControlFormList.RefreshDataSource();
                //gridviewFormList.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisExecuteRoomFilter filter)
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

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    EnDefautService(this.ActionType);
                    //Disable nút sửa nếu dữ liệu đã bị khóa
                    if (currentData != null)
                    {
                        btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    }

                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data)
        {
            try
            {

                if (data != null)
                {
                    cboAccountQr.EditValue = null;
                    txtJsonQr.Text = null;
                    roomId = data.ROOM_ID;
                    InitComboDefaultService();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId));
                    executeRoomId = data.ID;
                    txtExecuteRoomCode.Text = data.EXECUTE_ROOM_CODE;
                    txtExecuteRoomName.Text = data.EXECUTE_ROOM_NAME;
                    lkRoomId.EditValue = data.DEPARTMENT_ID;
                    InitComboArea();
                    if (data.AREA_ID != null)
                    {
                        cboArea.EditValue = data.AREA_ID;
                        cboArea.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboArea.EditValue = null;
                        cboArea.Properties.Buttons[1].Visible = false;
                    }
                    GridCheckMarksSelection gridCheckMarkPart = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkPart.ClearSelection(cboDefaultDrug.Properties.View);

                    chkIsEmergency.Checked = (data.IS_EMERGENCY == 1 ? true : false);
                    chkIsSpeciality.Checked = (data.IS_SPECIALITY == 1 ? true : false);
                    chkIsSurgery.Checked = (data.IS_SURGERY == 1 ? true : false);
                    chkIsPause.Checked = (data.IS_PAUSE == 1 ? true : false);
                    chkRestrictTime.Checked = (data.IS_RESTRICT_TIME == 1 ? true : false);
                    chkIsPauseEnclitic.Checked = (data.IS_PAUSE_ENCLITIC == 1 ? true : false);
                    chkRestrictMedicineType.Checked = (data.IS_RESTRICT_MEDICINE_TYPE == 1 ? true : false);
                    chkRestrictPatientType.Checked = (data.IS_RESTRICT_PATIENT_TYPE == 1 ? true : false);
                    c.Checked = (data.IS_RESTRICT_REQ_SERVICE == 1 ? true : false);
                    chkKhongCanChonDV.Checked = (data.ALLOW_NOT_CHOOSE_SERVICE == 1 ? true : false);
                    chkRestrictExecuteRoom.Checked = (data.IS_RESTRICT_EXECUTE_ROOM == 1 ? true : false);
                    spMaxRequestByDay.EditValue = data.MAX_REQUEST_BY_DAY;
                    spinMaxAppointment.EditValue = data.MAX_APPOINTMENT_BY_DAY;
                    txtTestTypeCode.Text = data.TEST_TYPE_CODE;
                    spHoldOrder.EditValue = data.HOLD_ORDER;
                    chkIsExam.Checked = (data.IS_EXAM == 1 ? true : false);
                    chkIsExamPlus.Checked = (data.IS_AUTO_EXPEND_ADD_EXAM == 1 ? true : false);
                    chkVaccine2.Checked = (data.IS_VACCINE == 1 ? true : false);
                    chkIsAllowNoICD.Checked = (data.IS_ALLOW_NO_ICD == 1 ? true : false);
                    if (data.IS_KIDNEY.HasValue && data.IS_KIDNEY.Value == 1)
                    {
                        spinKidneyCount.Enabled = true;
                        spinKidneyCount.EditValue = data.KIDNEY_SHIFT_COUNT;
                        chkIsKidney.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        spinKidneyCount.Enabled = false;
                        spinKidneyCount.EditValue = null;
                        chkIsKidney.CheckState = CheckState.Unchecked;
                    }
                    checkVitaminA.Checked = (data.IS_VITAMIN_A == 1 ? true : false);
                    //chkIsPause.Checked = (data.IS_PAUSE == 1 ? true : false);IS_USE_KIOSK
                    spSTT.EditValue = data.NUM_ORDER;
                    cbbRoomGroup.EditValue = data.ROOM_GROUP_ID;
                    cboChuyenKhoa.EditValue = data.SPECIALITY_ID;
                    txtAddress.Text = data.ADDRESS;
                    checkEdit1.Checked = (data.IS_USE_KIOSK == 1 ? true : false);
                    txtOrderIssueCode.Text = data.ORDER_ISSUE_CODE;
                    spMaxReqBhytByDay.EditValue = data.MAX_REQ_BHYT_BY_DAY;
                    spAVERAGE_ETA.EditValue = data.AVERAGE_ETA;
                    spMaxPatientByDay.EditValue = data.MAX_PATIENT_BY_DAY;
                    //cboDefaultDrug.EditValue = data.DEFAULT_DRUG_STORE_ID;
                    GridCheckMarksSelection gridCheckMark = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    if (!String.IsNullOrWhiteSpace(data.DEFAULT_DRUG_STORE_IDS) && cboDefaultDrug.Properties.Tag != null)
                    {
                        ProcessSelectBusiness(data.DEFAULT_DRUG_STORE_IDS, gridCheckMark);
                    }

                    var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == data.ROOM_ID);
                    if (room != null)
                    {
                        chkIsBlockNumOrder.Enabled = (data.IS_EXAM == 1 ? true : false);
                        chkIsBlockNumOrder.Checked = (room.IS_BLOCK_NUM_ORDER == 1 ? true : false);
                        chkIsExam.Checked = (data.IS_EXAM == 1 ? true : false);

                        CboResponsible.EditValue = room.RESPONSIBLE_LOGINNAME;
                        cboWaitingScreen.EditValue = room.SCREEN_SAVER_MODULE_LINK;
                        cboWaitingScreen.Properties.Buttons[1].Visible = true;
                        cboCashRoom.EditValue = room.DEFAULT_CASHIER_ROOM_ID;
                        cboDepositBook.EditValue = room.DEPOSIT_ACCOUNT_BOOK_ID;
                        cboDepositBook.Properties.Buttons[1].Visible = room.DEPOSIT_ACCOUNT_BOOK_ID.HasValue;
                        cboAccountBook.EditValue = room.BILL_ACCOUNT_BOOK_ID;
                        cboAccountBook.Properties.Buttons[1].Visible = room.BILL_ACCOUNT_BOOK_ID.HasValue;
                        cboDefaultService.EditValue = room.DEFAULT_SERVICE_ID;
                        cboDefaultsCLS.EditValue = room.DEFAULT_INSTR_PATIENT_TYPE_ID;
                        cboAccountQr.EditValue = room.QR_ACCOUNT_BOOK_ID;
                        txtJsonQr.Text = room.QR_CONFIG_JSON;
                        var bhyt = BackendDataWorker.Get<HIS_ROOM>().FirstOrDefault(p => p.ID == room.ID);
                        if (bhyt != null)
                        {
                            txtHein_card_number.Clear();
                            txtHein_card_number.Refresh();
                            if (bhyt.BHYT_CODE != null)
                            {
                                txtHein_card_number.Text = bhyt.BHYT_CODE.ToString();
                            }
                            else
                            {
                                txtHein_card_number.Text = "";
                            }
                        }
                        else
                        {
                            txtHein_card_number.Clear();
                            txtHein_card_number.Refresh();
                            txtHein_card_number.Text = "";
                        }
                    }
                    else
                    {
                        cboWaitingScreen.Properties.Buttons[1].Visible = true;
                    }
                    txtDirectorLoginName.Text = data.HOSP_SUBS_DIRECTOR_LOGINNAME;
                    cboDirectorUserName.EditValue = data.HOSP_SUBS_DIRECTOR_LOGINNAME;
                }
                else
                {
                    roomId = 0;
                    executeRoomId = 0;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void ProcessSelectBusiness(string p, GridCheckMarksSelection gridCheckMark)
        {
            try
            {
                List<HIS_MEDI_STOCK> ds = cboDefaultDrug.Properties.DataSource as List<HIS_MEDI_STOCK>;
                string[] arrays = p.Split(',');
                if (arrays != null && arrays.Length > 0)
                {
                    List<HIS_MEDI_STOCK> selects = new List<HIS_MEDI_STOCK>();
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoomFilter filter = new HisExecuteRoomFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>>(HisRequestUriStore.MOSV_HIS_EXECUTE_ROOM_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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
                txtExecuteRoomCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
                lkRoomId.ReadOnly = !(action == GlobalVariables.ActionAdd);
                EnDefautService(action);
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
                if (chkIsKidney.Checked && spinKidneyCount.EditValue == null)
                {
                    dxErrorProvider.SetError(spinKidneyCount, "Chưa nhập trường dữ liệu bắt buộc", ErrorType.Warning);
                    //txtPackagePrice.Validating += txtPackagePrice_Validating;
                    return;
                }
                else if (chkIsKidney.Checked && spinKidneyCount.EditValue != null)
                {
                    dxErrorProvider.ClearErrors();
                }
                WaitingManager.Show();
                MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM updateDTO = new MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM();
                //UpdateDTOFromDataForm(ref updateDTO);
                MOS.SDO.HisExecuteRoomSDO executeRoomSDO = new MOS.SDO.HisExecuteRoomSDO();
                MOS.SDO.HisExecuteRoomSDO executeRoomResultSDO = new MOS.SDO.HisExecuteRoomSDO();

                executeRoomSDO.HisRoom = SetDataRoom();

                executeRoomSDO.HisExecuteRoom = SetDataExecuteRoom();
                executeRoomSDO.HisExecuteRoom.HOSP_SUBS_DIRECTOR_LOGINNAME = txtDirectorLoginName.Text;
                executeRoomSDO.HisExecuteRoom.HOSP_SUBS_DIRECTOR_USERNAME = cboDirectorUserName.Text;
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    executeRoomResultSDO = new BackendAdapter(param)
                        .Post<MOS.SDO.HisExecuteRoomSDO>("api/HisExecuteRoom/Create", ApiConsumers.MosConsumer, executeRoomSDO, param);
                }
                else
                {
                    if (roomId > 0 && executeRoomId > 0)
                    {
                        executeRoomSDO.HisRoom.ID = roomId;
                        executeRoomSDO.HisExecuteRoom.ID = executeRoomId;
                        executeRoomResultSDO = new BackendAdapter(param)
                        .Post<HisExecuteRoomSDO>("api/HisExecuteRoom/Update", ApiConsumers.MosConsumer, executeRoomSDO, param);
                    }
                }
                if (executeRoomResultSDO != null && executeRoomResultSDO.HisExecuteRoom != null)
                {
                    checkEdit1.CheckState = CheckState.Unchecked;
                    checkEdit1.Properties.FullFocusRect = false;
                    success = true;
                    FillDataToGridControl();
                    btnCancel_Click(null, null);
                }
                if (success)
                {
                    BackendDataWorker.Reset<V_HIS_EXECUTE_ROOM>();
                    BackendDataWorker.Reset<HIS_EXECUTE_ROOM>();
                    BackendDataWorker.Reset<V_HIS_ROOM>();
                    BackendDataWorker.Reset<HIS_ROOM>();
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM) is null");
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM currentDTO)
        {
            try
            {
                currentDTO.EXECUTE_ROOM_CODE = txtExecuteRoomCode.Text.Trim();
                currentDTO.EXECUTE_ROOM_NAME = txtExecuteRoomName.Text.Trim();
                if (lkRoomId.EditValue != null) currentDTO.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((lkRoomId.EditValue ?? "0").ToString());
                currentDTO.ROOM_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL;
                currentDTO.HOSP_SUBS_DIRECTOR_LOGINNAME = txtDirectorLoginName.Text;
                currentDTO.HOSP_SUBS_DIRECTOR_USERNAME = cboDirectorUserName.Text;
                currentDTO.IS_EMERGENCY = (short)(chkIsEmergency.Checked ? 1 : 0);
                currentDTO.IS_PAUSE_ENCLITIC = (short)(chkIsPauseEnclitic.Checked ? 1 : 0);
                currentDTO.IS_SPECIALITY = (short)(chkIsSpeciality.Checked ? 1 : 0);
                currentDTO.IS_SURGERY = (short)(chkIsSurgery.Checked ? 1 : 0);
                currentDTO.IS_PAUSE = (short)(chkIsPause.Checked ? 1 : 0);
                currentDTO.TEST_TYPE_CODE = txtTestTypeCode.Text.Trim();
                if (spMaxReqBhytByDay.EditValue != null)
                {
                    currentDTO.MAX_REQ_BHYT_BY_DAY = (long)spMaxReqBhytByDay.EditValue;
                }
                else
                {
                    currentDTO.MAX_REQ_BHYT_BY_DAY = null;
                }
                if (spMaxRequestByDay.EditValue != null)
                {
                    currentDTO.MAX_REQUEST_BY_DAY = (long)spMaxRequestByDay.EditValue;
                }
                else
                {
                    currentDTO.MAX_REQUEST_BY_DAY = null;
                }

                if (cboDefaultDrug.EditValue != null)
                {
                    GridCheckMarksSelection gridCheckMarkBusiness = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMarkBusiness != null && gridCheckMarkBusiness.SelectedCount > 0)
                    {
                        List<string> codes = new List<string>();
                        foreach (HIS_MEDI_STOCK rv in gridCheckMarkBusiness.Selection)
                        {
                            if (rv != null && !codes.Contains(rv.ID.ToString()))
                                codes.Add(rv.ID.ToString());
                        }

                        currentDTO.DEFAULT_DRUG_STORE_IDS = String.Join(",", codes);
                    }
                }
                else
                {
                    currentDTO.DEFAULT_DRUG_STORE_IDS = null;
                }

                if (spinMaxAppointment.EditValue != null)
                {
                    currentDTO.MAX_APPOINTMENT_BY_DAY = (long)spinMaxAppointment.EditValue;
                }
                else
                {
                    currentDTO.MAX_APPOINTMENT_BY_DAY = null;
                }

                currentDTO.IS_EXAM = (short)(chkIsExam.Checked ? 1 : 0);
                if (spinKidneyCount.EditValue != null)
                {
                    currentDTO.KIDNEY_SHIFT_COUNT = (long)spinKidneyCount.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ---Even Gridcontrol ---
        private void gridviewFormList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM pData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPage; //+ ((pagingGrid.CurrentPage - 1) * pagingGrid.PageSize);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {
                        try
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);

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
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME ?? 0);

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao MODIFY_TIME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_PAUSE_ENCLITIC_DISPLAY")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PAUSE_ENCLITIC == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot IS_PAUSE_ENCLITIC_DISPLAY", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_PAUSE_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PAUSE == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot IS_PAUSE_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ALLOW_NO_ICD_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_ALLOW_NO_ICD == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot IS_ALLOW_NO_ICD_STR", ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_ST")
                    {
                        try
                        {
                            if (pData.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.Value = "Hoạt động";
                            else e.Value = "Tạm khóa";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "RESPONSIBLE_NAME")
                    {
                        try
                        {
                            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == pData.ROOM_ID);
                            if (room != null) e.Value = string.Format("{0}({1})", room.RESPONSIBLE_USERNAME, room.RESPONSIBLE_LOGINNAME);
                            else e.Value = null;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "SIGN_FOR_DIRECTOR")
                    {
                        try
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
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    //gridControlFormList.RefreshDataSource();
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
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();

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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
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

        private void gridviewFormList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_EXECUTE_ROOM data = (V_HIS_EXECUTE_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "Lock")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnlock : btnLock);

                    }
                    else if (e.Column.FieldName == "DELETE")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnGEdit;
                            else
                                e.RepositoryItem = btnGEdit_Disable;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "RestrictTime")
                    {
                        try
                        {
                            //if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_TIME == 1)
                            //    e.RepositoryItem = Btn_RestrictTime_Enable;
                            //else
                            //    e.RepositoryItem = Btn_RestrictTime_Disable;
                            e.RepositoryItem = Btn_RestrictTime_Enable;
                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "RestrictMedicineType")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_MEDICINE_TYPE == 1)
                                e.RepositoryItem = Btn_RestrictMedicineType_Enable;
                            else
                                e.RepositoryItem = Btn_RestrictMedicineType_Disable;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "RestrictExecuteRoom")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_EXECUTE_ROOM == 1)
                                e.RepositoryItem = Btn_RestrictExecuteRoom_Enable;
                            else
                                e.RepositoryItem = Btn_RestrictExecuteRoom_Disable;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_VITAMIN_A_Str")
                    {
                        try
                        {
                            if (data.IS_VITAMIN_A == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnIVitaminA;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_VACCINE_Str")
                    {
                        try
                        {
                            if (data.IS_VACCINE == IS_ACTIVE_TRUE)
                                e.RepositoryItem = btnIVaccine;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_EMERGENCY_STR")
                    {
                        try
                        {
                            if (data.IS_EMERGENCY == IS_ACTIVE_TRUE)
                                e.RepositoryItem = this.btnIPhongCapCuu;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }

                    else if (e.Column.FieldName == "IS_SURGERY_STR")
                    {
                        try
                        {
                            if (data.IS_SURGERY == IS_ACTIVE_TRUE)
                                e.RepositoryItem = this.btnIMo;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_USE_KIOSK_STR")
                    {
                        try
                        {
                            if (data.IS_USE_KIOSK == IS_ACTIVE_TRUE)
                                e.RepositoryItem = this.btnIKiosk;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_EXAM_STR")
                    {
                        try
                        {
                            if (data.IS_EXAM == IS_ACTIVE_TRUE)
                                e.RepositoryItem = this.btnIKham;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_SPECIALITY_STR")
                    {
                        try
                        {
                            if (data.IS_SPECIALITY == IS_ACTIVE_TRUE)
                                e.RepositoryItem = this.btnIChuyenKhoa;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "PatientTypeRoom")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_PATIENT_TYPE == 1)
                                e.RepositoryItem = Btn_PatientTypeRoom_Enable;
                            else
                                e.RepositoryItem = Btn_PatientTypeRoom_Disable;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "ServiceRoom")
                    {
                        try
                        {
                            if (data.IS_ACTIVE == IS_ACTIVE_TRUE && data.IS_RESTRICT_REQ_SERVICE == 1)
                                e.RepositoryItem = Btn_ServiceRoom_Enable;
                            else
                                e.RepositoryItem = Btn_ServiceRoom_Disable;

                        }
                        catch (Exception ex)
                        {

                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_KIDNEY")
                    {
                        try
                        {
                            if (data.IS_KIDNEY == IS_ACTIVE_TRUE)
                                e.RepositoryItem = this.ButtonEditIsKidney;

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

        private void gridviewFormList_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_EXECUTE_ROOM data = (V_HIS_EXECUTE_ROOM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "IS_ACTIVE_ST")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
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
        #endregion

        #region ---KeyUp---
        private void txtTestTypeCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spMaxRequestByDay.Focus();
                    spMaxRequestByDay.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
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

        private void txtExecuteRoomCode_KeyUp_1(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExecuteRoomName.Focus();
                    txtExecuteRoomName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtExecuteRoomName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    lkRoomId.Focus();
                    lkRoomId.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void lkRoomId_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cbbRoomGroup.Focus();
                    cbbRoomGroup.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cbbRoomGroup_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtOrderIssueCode.Focus();
                    txtOrderIssueCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtOrderIssueCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spSTT.Focus();
                    spSTT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spSTT_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestTypeCode.Focus();
                    txtTestTypeCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spMaxRequestByDay_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spHoldOrder.Focus();
                    spHoldOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spHoldOrder_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboChuyenKhoa.Focus();
                    cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChuyenKhoa_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAddress.Focus();
                    txtAddress.SelectAll();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsEmergency_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void chkIsSurgery_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsExam.Focus();
                    //chkIsSpeciality.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsSpeciality_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsAllowNoICD.Focus();
                    //chkIsExam.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsExam_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsExamPlus.Focus();
                    //checkEdit1.Focus();
                    //cboChuyenKhoa.ShowPopup();
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
                    chkIsPause.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPause_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRestrictExecuteRoom.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRestrictExecuteRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRestrictMedicineType.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRestrictMedicineType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRestrictTime.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRestrictTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsPauseEnclitic.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAddress_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spMaxReqBhytByDay.Focus();
                    spMaxReqBhytByDay.SelectAll();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spMaxReqBhytByDay_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spMaxPatientByDay.Focus();
                    spMaxPatientByDay.SelectAll();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPauseEnclitic_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkVaccine2.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spAVERAGE_ETA_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboResponsible.Focus();
                    //chkIsEmergency.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkVaccine2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    checkVitaminA.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void checkVitaminA_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    c.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRestrictPatientType_KeyUp(object sender, KeyEventArgs e)
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

        private void CboResponsible_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDefaultsCLS.Focus();
                    //chkIsEmergency.Focus();
                    cboDefaultsCLS.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRestrictReqService_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkRestrictPatientType.Focus();
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
                    if (spinKidneyCount.Enabled)
                    {
                        spinKidneyCount.Focus();
                        spinKidneyCount.SelectAll();
                    }
                    else if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinKidneyCount_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDefaultService.Enabled == true)
                    {
                        cboDefaultService.Focus();
                        cboDefaultService.ShowPopup();
                    }
                    else
                    {
                        if (this.ActionType == GlobalVariables.ActionAdd)
                            btnAdd.Focus();
                        else
                            btnEdit.Focus();
                        //cboChuyenKhoa.ShowPopup();
                        e.Handled = true;
                        //chkIsAllowNoICD.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsAllowNoICD_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
                    //cboChuyenKhoa.ShowPopup();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultDrug_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboCashRoom.Focus();
                    cboCashRoom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboArea_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //cboWaitingScreen.Focus();
                    if (cboArea.EditValue != null)
                    {
                        cboArea.Properties.Buttons[1].Visible = true;
                        cboWaitingScreen.Focus();
                    }
                    else
                    {
                        cboArea.Properties.Buttons[1].Visible = false;
                        cboArea.ShowPopup();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region ---EditValueChange---
        private void lkRoomId_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (lkRoomId.EditValue != null)
                {
                    InitComboArea();
                    department = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>().Where(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(lkRoomId.EditValue.ToString())).FirstOrDefault();
                    InitComboCashRoom();
                }
                else
                {
                    cboArea.EditValue = null;
                    cboArea.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void chkIsKidney_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsKidney.CheckState == CheckState.Checked)
                {
                    spinKidneyCount.Enabled = true;
                }
                else
                {
                    spinKidneyCount.Enabled = false;
                    spinKidneyCount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsKidney_EditValueChanging(object sender, ChangingEventArgs e)
        {
            try
            {
                if (chkIsKidney.Checked)
                {

                    spinKidneyCount.Enabled = false;
                    spinKidneyCount.EditValue = null;
                    lciKidneyCount.AppearanceItemCaption.ForeColor = Color.Transparent;
                    dxValidationProviderEditorInfo.RemoveControlError(spinKidneyCount);

                }
                else
                {
                    spinKidneyCount.Enabled = true;
                    lciKidneyCount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //ValidationSingleControl(txtPackagePrice);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        #endregion

        #region Button handler
        #region ---Click---
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
                if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                    if (rowData != null)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.MOSV_HIS_EXECUTE_ROOM_DELETE, ApiConsumers.MosConsumer, rowData, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<V_HIS_EXECUTE_ROOM>();
                            BackendDataWorker.Reset<HIS_EXECUTE_ROOM>();
                            FillDataToGridControl();
                            btnRefesh_Click(null, null);
                        }
                        MessageManager.Show(this, param, success);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                txtKeyword.Text = "";
                txtExecuteRoomCode.Text = "";
                txtExecuteRoomName.Text = "";
                txtTestTypeCode.Text = "";
                lkRoomId.EditValue = null;
                cboArea.EditValue = null;
                cboArea.Properties.Buttons[1].Visible = false;
                spMaxRequestByDay.EditValue = null;
                chkIsEmergency.CheckState = CheckState.Unchecked;
                chkIsExamPlus.CheckState = CheckState.Unchecked;
                chkIsExam.CheckState = CheckState.Unchecked;
                chkIsPause.CheckState = CheckState.Unchecked;
                chkIsSpeciality.CheckState = CheckState.Unchecked;
                chkIsSurgery.CheckState = CheckState.Unchecked;
                chkIsExamPlus.Properties.FullFocusRect = false;
                chkIsEmergency.Properties.FullFocusRect = false;
                chkIsExam.Properties.FullFocusRect = false;
                chkIsPause.Properties.FullFocusRect = false;
                chkIsSpeciality.Properties.FullFocusRect = false;
                chkIsSurgery.Properties.FullFocusRect = false;
                checkEdit1.CheckState = CheckState.Unchecked;
                checkEdit1.Properties.FullFocusRect = false;
                chkRestrictExecuteRoom.Checked = false;
                chkRestrictMedicineType.Checked = false;
                chkRestrictPatientType.Checked = false;
                chkIsKidney.Checked = false;
                c.Checked = false;
                chkKhongCanChonDV.Checked = false;
                chkRestrictTime.Checked = false;
                chkVaccine2.Checked = false;
                chkIsKidney.Checked = false;
                spinKidneyCount.EditValue = null;
                spinKidneyCount.Enabled = false;
                checkVitaminA.Checked = false;
                chkIsPauseEnclitic.CheckState = CheckState.Unchecked;
                chkIsAllowNoICD.Checked = false;
                txtOrderIssueCode.Text = "";
                txtAddress.Text = "";
                //SetFocusEditor();
                txtExecuteRoomCode.Focus();
                cbbRoomGroup.EditValue = null;
                spMaxReqBhytByDay.EditValue = null;
                chkIsBlockNumOrder.Enabled = false;
                chkIsBlockNumOrder.Checked = false;

                cboDefaultDrug.EditValue = null;

                GridCheckMarksSelection gridCheckMarkPart = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                gridCheckMarkPart.ClearSelection(cboDefaultDrug.Properties.View);
                cboDefaultDrug.Text = "";

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

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            //cbbRoomGroup.Text = null;
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                List<object> listArgs = new List<object>();
                CallModule callModule = new CallModule(CallModule.HisImportExecuteRoom, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---ButtonClick--
        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_EXECUTE_ROOM hisDepertments = new V_HIS_EXECUTE_ROOM();
            bool notHandler = false;
            try
            {
                V_HIS_EXECUTE_ROOM dataDepartment = (V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_EXECUTE_ROOM data1 = new V_HIS_EXECUTE_ROOM();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<V_HIS_EXECUTE_ROOM>("api/HisExecuteRoom/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                    btnEdit.Enabled = false;
                }
                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
                BackendDataWorker.Reset<V_HIS_EXECUTE_ROOM>();
                BackendDataWorker.Reset<HIS_EXECUTE_ROOM>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_EXECUTE_ROOM hisDepertments = new V_HIS_EXECUTE_ROOM();
            bool notHandler = false;
            try
            {
                V_HIS_EXECUTE_ROOM dataDepartment = (V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    V_HIS_EXECUTE_ROOM data1 = new V_HIS_EXECUTE_ROOM();
                    data1.ID = dataDepartment.ID;
                    WaitingManager.Show();
                    hisDepertments = new BackendAdapter(param).Post<V_HIS_EXECUTE_ROOM>("api/HisExecuteRoom/ChangeLock", ApiConsumers.MosConsumer, data1, param);
                    WaitingManager.Hide();
                    if (hisDepertments != null) FillDataToGridControl();
                    btnEdit.Enabled = true;
                }
                notHandler = true;
                MessageManager.Show(this.ParentForm, param, notHandler);
                BackendDataWorker.Reset<V_HIS_EXECUTE_ROOM>();
                BackendDataWorker.Reset<HIS_EXECUTE_ROOM>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultDrug_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMarkBusinessCodes = cboDefaultDrug.Properties.Tag as GridCheckMarksSelection;
                    gridCheckMarkBusinessCodes.ClearSelection(cboDefaultDrug.Properties.View);
                    cboDefaultDrug.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboArea_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboArea.EditValue = null;
                    cboArea.Properties.Buttons[1].Visible = false;
                    cboWaitingScreen.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cbbRoomGroup_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cbbRoomGroup.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_RestrictTime_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.HisRoomTime, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                    //this.Hide();
                    //this.ParentForm.WindowState = FormWindowState.Normal;

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_RestrictMedicineType_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.MedicineTypeRoom, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    this.Hide();
                    this.ParentForm.WindowState = FormWindowState.Normal;

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_RestrictExecuteRoom_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.ExroRoom, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                    this.Hide();

                    this.ParentForm.WindowState = FormWindowState.Normal;

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboChuyenKhoa_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboChuyenKhoa.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_PatientTypeRoom_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.PatientTypeRoom, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                    this.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Btn_ServiceRoom_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                //WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();

                    V_HIS_ROOM room = new V_HIS_ROOM();
                    room.ID = row.ROOM_ID;
                    room.ROOM_CODE = row.EXECUTE_ROOM_CODE;
                    room.ROOM_NAME = row.EXECUTE_ROOM_NAME;
                    room.ROOM_TYPE_ID = row.ROOM_TYPE_ID;
                    room.ROOM_TYPE_CODE = row.ROOM_TYPE_CODE;
                    listArgs.Add(room);
                    CallModule callModule = new CallModule(CallModule.ServiceRoom, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                    WaitingManager.Hide();
                    this.Hide();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboResponsible_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboResponsible.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        #endregion

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidationtxtExecuteRoomCode();
                ValidationSingleControl(txtExecuteRoomName);
                ValidationSingleControl(lkRoomId);
                ValidationLonHon0(spHoldOrder);


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

        private void ValidationLonHon0(SpinEdit control)
        {
            try
            {
                ValidateSpin2 validRule = new ValidateSpin2();
                validRule.spin = control;
                //validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditorInfo.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationtxtExecuteRoomCode()
        {
            ValidMaxlengthtxtExecuteRoomCode validRule = new ValidMaxlengthtxtExecuteRoomCode();
            validRule.txtExecuteRoomCode = this.txtExecuteRoomCode;
            validRule.ErrorType = ErrorType.Warning;
            dxValidationProviderEditorInfo.SetValidationRule(txtExecuteRoomCode, validRule);
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

        #region ---PreviewKeyDown---
        private void chkIsEmergency_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSurgery.Focus();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTestTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

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
                    var rowData = (MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM)gridviewFormList.GetFocusedRow();
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

        #endregion

        #region Public method
        public void MeShow()
        {
            try
            {
                loadDataArea();
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

                // load combobox ky thay bac sy
                LoadComboDirector();

                LoadDirectorData();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void loadDataArea()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisAreaFilter filer = new HisAreaFilter();
                this.listArea = new BackendAdapter(param).Get<List<HIS_AREA>>("api/HisArea/Get", ApiConsumers.MosConsumer, filer, param);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
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
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnImport_Click(null, null);
        }
        #endregion

        private void btnAdd_Enter(object sender, EventArgs e)
        {
            //SaveProcess();
        }

        private void cboArea_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboArea.EditValue != null)
                    {
                        cboArea.Properties.Buttons[1].Visible = true;
                        cboWaitingScreen.Focus();
                    }
                    else
                    {
                        cboArea.Properties.Buttons[1].Visible = false;
                        cboArea.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWaitingScreen_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboWaitingScreen.EditValue = null;
                    cboWaitingScreen.Properties.Buttons[1].Visible = false;
                    chkIsEmergency.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWaitingScreen_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboWaitingScreen.EditValue != null)
                    {
                        cboWaitingScreen.Properties.Buttons[1].Visible = true;
                        chkIsEmergency.Focus();
                    }
                    else
                    {
                        cboWaitingScreen.Properties.Buttons[1].Visible = false;
                        cboArea.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWaitingScreen_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsEmergency.Focus();
                    //if (cboWaitingScreen.EditValue != null)
                    //{
                    //    cboWaitingScreen.Properties.Buttons[1].Visible = true;
                    //    chkIsEmergency.Focus();
                    //}
                    //else
                    //{
                    //    cboWaitingScreen.Properties.Buttons[1].Visible = false;
                    //    cboWaitingScreen.ShowPopup();
                    //}
                }
                //e.Handled = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashRoom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCashRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCashRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboArea.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spMaxPatientByDay_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                e.Handled = true;

        }

        private void spMaxPatientByDay_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spAVERAGE_ETA.Focus();
                    spAVERAGE_ETA.SelectAll();
                    //cboChuyenKhoa.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsExamPlus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsSpeciality.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkRestrictReqService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkKhongCanChonDV.Focus();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDepositBook()
        {
            try
            {
                cboDepositBook.EditValue = null;
                cboDepositBook.Properties.Buttons[1].Visible = false;


                CommonParam param = new CommonParam();
                HisAccountBookFilter filter = new HisAccountBookFilter();
                filter.IS_ACTIVE = 1;
                filter.FOR_DEPOSIT = true;
                var data = new BackendAdapter(param).Get<List<HIS_ACCOUNT_BOOK>>("api/HisAccountBook/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                if (data != null && data.Count > 0)
                {
                    List<ColumnInfo> colum = new List<ColumnInfo>();
                    colum.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                    colum.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 200, 2));
                    ControlEditorADO controlADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", colum, false, 300);
                    ControlEditorLoader.Load(cboDepositBook, data, controlADO);
                    cboDepositBook.Properties.ImmediatePopup = true;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void InitComboAccountBook()
        {
            try
            {
                cboAccountBook.EditValue = null;
                cboAccountBook.Properties.Buttons[1].Visible = false;


                CommonParam param = new CommonParam();
                HisAccountBookFilter filter = new HisAccountBookFilter();
                filter.IS_ACTIVE = 1;
                filter.FOR_BILL = true;
                var data = new BackendAdapter(param).Get<List<HIS_ACCOUNT_BOOK>>("api/HisAccountBook/Get", ApiConsumers.MosConsumer, filter, param).ToList();
                if (data != null && data.Count > 0)
                {
                    List<ColumnInfo> colum = new List<ColumnInfo>();
                    colum.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                    colum.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 200, 2));
                    ControlEditorADO controlADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", colum, false, 300);
                    ControlEditorLoader.Load(cboAccountBook, data, controlADO);
                    cboAccountBook.Properties.ImmediatePopup = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitComboDefaultService()
        {
            try
            {
                cboDefaultService.EditValue = null;

                CommonParam param = new CommonParam();
                HisServiceRoomViewFilter filter = new HisServiceRoomViewFilter();
                filter.IS_ACTIVE = 1;
                filter.ROOM_ID = roomId;
                var data = new BackendAdapter(param).Get<List<V_HIS_SERVICE_ROOM>>("api/HisServiceRoom/GetView", ApiConsumers.MosConsumer, filter, param).ToList();
                if (data != null && data.Count > 0)
                {
                    List<ColumnInfo> colum = new List<ColumnInfo>();
                    colum.Add(new ColumnInfo("SERVICE_CODE", "", 100, 1));
                    colum.Add(new ColumnInfo("SERVICE_NAME", "", 200, 2));
                    ControlEditorADO controlADO = new ControlEditorADO("SERVICE_NAME", "SERVICE_ID", colum, false, 300);
                    ControlEditorLoader.Load(cboDefaultService, data, controlADO);
                    cboDefaultService.Properties.ImmediatePopup = true;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        private void cboDepositBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepositBook.EditValue = null;
                    cboDepositBook.Properties.Buttons[1].Visible = false;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHein_card_number_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepositBook.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAccountBook.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsExam.CheckState == CheckState.Checked)
                {
                    chkIsBlockNumOrder.Enabled = true;
                }
                else
                {
                    chkIsBlockNumOrder.Enabled = false;
                    chkIsBlockNumOrder.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDefaultDrug_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;
                foreach (HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void EnDefautService(int edit)
        {

            try
            {
                if (edit == GlobalVariables.ActionEdit && chkIsExam.Checked == true)
                {
                    cboDefaultService.Enabled = true;
                }
                else
                {
                    cboDefaultService.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboDefaultService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDefaultService.EditValue = null;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDefaultService_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                        btnAdd.Focus();
                    else
                        btnEdit.Focus();
                    //cboChuyenKhoa.ShowPopup();
                    e.Handled = true;
                    //chkIsAllowNoICD.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboAccountBook.EditValue = null;
                    cboAccountBook.Properties.Buttons[1].Visible = false;

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsEmergency.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDefaultsCLS_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDefaultsCLS.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDefaultsCLS_KeyUp(object sender, KeyEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDefaultDrug.Focus();
                    //chkIsEmergency.Focus();
                    cboDefaultDrug.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<ConfigSettingsADO> lstConfigQrSetting = new List<ConfigSettingsADO>();
        private void btnOkQr_Click(object sender, EventArgs e)
        {
            try
            {
                BankInfo bankif = new BankInfo();
                if (!string.IsNullOrEmpty(lstConfigQrSetting.First().ID_CONFIG))
                    bankif.BANK = lstConfigQrSetting.First().ID_CONFIG;
                if (!string.IsNullOrEmpty(lstConfigQrSetting.Last().VALUE_CONFIG))
                    bankif.VALUE = lstConfigQrSetting.Last().VALUE_CONFIG;
                if (!string.IsNullOrEmpty(bankif.BANK) || !string.IsNullOrEmpty(bankif.VALUE))
                {
                    string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(bankif);
                    txtJsonQr.Text = jsonString;
                }
                else
                {
                    txtJsonQr.Text = null;
                }
                PopupContainerBarControl control = popupControlContainer1.Parent as PopupContainerBarControl;
                control.ClosePopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView19_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {

            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    ConfigSettingsADO pData = (ConfigSettingsADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "VALUE")
                    {
                        if (pData.IS_VALUE)
                            e.Value = pData.VALUE_CONFIG;
                        else
                            e.Value = pData.ID_CONFIG;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void gridView19_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {

                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    ConfigSettingsADO data = (ConfigSettingsADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "VALUE")
                    {
                        if (!data.IS_VALUE)
                            e.RepositoryItem = repConfig;
                        else
                            e.RepositoryItem = repValue;

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnJsonQr_Click(object sender, EventArgs e)
        {

            try
            {
                lstConfigQrSetting = new List<ConfigSettingsADO>();
                if (!String.IsNullOrEmpty(txtJsonQr.Text))
                {
                    BankInfo ados = Newtonsoft.Json.JsonConvert.DeserializeObject<BankInfo>(txtJsonQr.Text);
                    lstConfigQrSetting.Add(new ConfigSettingsADO() { NAME = "Ngân hàng", ID_CONFIG = ados.BANK });
                    lstConfigQrSetting.Add(new ConfigSettingsADO() { NAME = "Giá trị cấu hình", IS_VALUE = true, VALUE_CONFIG = ados.VALUE });
                }
                else
                {
                    lstConfigQrSetting.Add(new ConfigSettingsADO() { NAME = "Ngân hàng" });
                    lstConfigQrSetting.Add(new ConfigSettingsADO() { NAME = "Giá trị cấu hình", IS_VALUE = true });
                }
                gridControl2.DataSource = lstConfigQrSetting;
                Rectangle buttonPosition = new Rectangle(btnJsonQr.Bounds.X, btnJsonQr.Bounds.Y, btnJsonQr.Bounds.Width, btnJsonQr.Bounds.Height);
                popupControlContainer1.ShowPopup(new Point(buttonPosition.X + 480, buttonPosition.Bottom + btnJsonQr.Bounds.Height - 10));
                //popupControlContainer1.ShowPopup(new Point(Cursor.Position.X - 150, Cursor.Position.Y + 200));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repConfig_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                var data = (ConfigSettingsADO)gridView19.GetFocusedRow();
                if (data != null && !data.IS_VALUE) { 
                    GridLookUpEdit cbo = sender as GridLookUpEdit;
                    data.ID_CONFIG = cbo.EditValue.ToString();
                    gridControl2.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void repValue_Leave(object sender, EventArgs e)
        {
            try
            {
                var data = (ConfigSettingsADO)gridView19.GetFocusedRow();
                if (data != null && data.IS_VALUE)
                {
                    TextEdit txt = sender as TextEdit;
                    data.VALUE_CONFIG = txt.Text.Trim();
                    gridControl2.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repConfig_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    var data = (ConfigSettingsADO)gridView19.GetFocusedRow();
                    if (data != null && !data.IS_VALUE)
                    {
                        data.ID_CONFIG = null;
                        gridControl2.DataSource = null;
                        gridControl2.DataSource = lstConfigQrSetting;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountQr_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccountQr.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtJsonQr_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                txtJsonQr.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }    
        private void txtDirectorLoginName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {
            //        bool showCbo = true;
            //        if (!String.IsNullOrEmpty(txtDirectorLoginName.Text.Trim()))
            //        {
            //            string code = txtDirectorLoginName.Text.Trim().ToLower();
            //            var listData = this.listDirectors.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
            //            var result = listData != null ? (listData.Count > 1 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
            //            if (result != null && result.Count > 0)
            //            {
            //                showCbo = false;
            //                txtDirectorLoginName.Text = result.First().LOGINNAME;
            //                cboDirectorUserName.EditValue = result.First().LOGINNAME;
            //                cboDirectorUserName.Properties.Buttons[1].Visible = true;
            //                chkIsEmergency.Focus();
            //                chkIsEmergency.SelectAll();
            //            }
            //        }
            //        if (showCbo)
            //        {
            //            cboDirectorUserName.Focus();
            //            cboDirectorUserName.ShowPopup();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    string code = txtDirectorLoginName.Text.Trim().ToLower();

                    if (!string.IsNullOrEmpty(code) && listDirectors != null)
                    {
                        var listData = listDirectors.Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = (listData.Count > 1)
                            ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList()
                            : listData;

                        if (result.Count > 0)
                        {
                            showCbo = false;
                            txtDirectorLoginName.Text = result.First().LOGINNAME;
                            cboDirectorUserName.EditValue = result.First().LOGINNAME;
                            if (cboDirectorUserName.Properties.Buttons.Count > 1)
                            {
                                cboDirectorUserName.Properties.Buttons[1].Visible = true;
                            }

                            chkIsEmergency.Focus();
                            chkIsEmergency.SelectAll();
                        }
                    }

                    if (showCbo)
                    {
                        cboDirectorUserName.Focus();
                        cboDirectorUserName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void LoadComboDirector()
        {
            try
            {
                var acsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                if(acsUser == null || acsUser.Count == 0)
                {
                    listDirectors = new List<ADO.DirectorADO>();
                }
                else
                {
                    listDirectors = acsUser.Select(item => new ADO.DirectorADO(item)).ToList();
                }
                //foreach(var item in acsUser)
                //{
                //    listDirectors.Add(new ADO.DirectorADO(item));
                //}
                Base.GlobalStore.LoadDataGridLookUpEdit(cboDirectorUserName, "LOGINNAME", "USERNAME", "LOGINNAME", listDirectors);
            }
            catch(Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtJsonQr_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDirectorUserName.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void cboDirectorUserName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    txtDirectorLoginName.Text = "";
                    cboDirectorUserName.EditValue = null;
                    cboDirectorUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDirectorUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsEmergency.Focus();
                    chkIsEmergency.SelectAll();
                }               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDirectorUserName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboDirectorUserName.EditValue != null)
                    {
                        var dataheads = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.IS_ACTIVE == 1).ToList();
                        var data = dataheads.FirstOrDefault(o => o.LOGINNAME == cboDirectorUserName.EditValue.ToString());
                        if (data != null)
                        {
                            txtDirectorLoginName.Text = data.LOGINNAME;
                            cboDirectorUserName.Properties.Buttons[1].Visible = true;
                        }
                        chkIsEmergency.Focus();
                        chkIsEmergency.SelectAll();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //private void frmHisExecuteRoom_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Control && e.KeyCode == Keys.F)
        //        {
        //            FillDataToGridControl();
        //        }
        //        else if (e.Control && e.KeyCode == Keys.N || e.Control && e.KeyCode == Keys.S)
        //        {
        //            SaveProcess();
        //        }
        //        else
        //            if (e.Control && e.KeyCode == Keys.R)
        //            {
        //                btnCancel_Click(null, null);
        //            }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }

        //}
    }
}
