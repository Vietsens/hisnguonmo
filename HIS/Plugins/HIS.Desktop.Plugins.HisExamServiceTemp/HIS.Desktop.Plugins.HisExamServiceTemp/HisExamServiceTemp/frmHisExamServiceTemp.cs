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
using HIS.Desktop.ADO;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.IsAdmin;
using DevExpress.XtraEditors.Controls;

namespace HIS.Desktop.Plugins.HisExamServiceTemp.HisExamServiceTemp
{
    public partial class frmHisExamServiceTemp : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomID = 0;
        long departmentID = 0;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int positionHandle = -1;
        MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP currentData;
        List<string> arrControlEnableNotChange = new List<string>();
        Dictionary<string, int> dicOrderTabIndexControl = new Dictionary<string, int>();
        Inventec.Desktop.Common.Modules.Module moduleData;
        MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP data;
        DelegateSelectData selectData;
        DelegateSelectObjectData selectObjectData;
        bool? isCreatorOrPublic;
        private string LoggingName = "";
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.HisExamServiceTemp.HisExamServiceTemp";
        #endregion

        #region Construct
        public frmHisExamServiceTemp(Inventec.Desktop.Common.Modules.Module moduleData, ExamServiceTempADO examServiceTempADO, MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP data_)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.roomID = moduleData.RoomId;
                
                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                this.selectData = examServiceTempADO.DelegateSelectData;
                this.selectObjectData = examServiceTempADO.DelegateSelectObjectData;
                this.data = data_;
                this.isCreatorOrPublic = examServiceTempADO.IsCreatorOrPublic;
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
        private void frmHisExamServiceTemp_Load(object sender, EventArgs e)
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
        //private void frmHisExamServiceTemp_KeyDown(object sender, KeyEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Control && e.KeyCode == Keys.N)
        //        {
        //            btnAdd.PerformClick();
        //        }
        //        if (e.Control && e.KeyCode == Keys.F)
        //        {
        //            btnSearch.PerformClick();
        //        }
        //        if (e.Control && e.KeyCode == Keys.S)
        //        {
        //            btnEdit.PerformClick();
        //        }
        //        if (e.Control && e.KeyCode == Keys.R)
        //        {
        //            btnCancel.PerformClick();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogSystem.Debug(ex);
        //    }
        //}
        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = this.moduleData.text;
                }

                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExamServiceTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExamServiceTemp.HisExamServiceTemp.frmHisExamServiceTemp).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.gridColumnDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcolGSelect.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.gcolGSelect.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempCode.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempName.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamCirculation.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamCirculation.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamCirculation.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamCirculation.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamRespiratory.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamRespiratory.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamRespiratory.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamRespiratory.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamDigestion.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamDigestion.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamDigestion.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamDigestion.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamKidneyUrology.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamKidneyUrology.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamKidneyUrology.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamKidneyUrology.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNeurological.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNeurological.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNeurological.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNeurological.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMuscleBone.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMuscleBone.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMuscleBone.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMuscleBone.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEnt.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEnt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEnt.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEnt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamStomatology.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamStomatology.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamStomatology.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamStomatology.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEye.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEye.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEye.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEye.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamOend.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamOend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamOend.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamOend.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMental.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMental.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMental.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMental.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamObstetric.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamObstetric.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamObstetric.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamObstetric.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNutrition.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNutrition.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNutrition.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNutrition.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMotion.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMotion.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMotion.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMotion.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHospitalizationReason.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColHospitalizationReason.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHospitalizationReason.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColHospitalizationReason.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalProcess.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalProcess.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalProcess.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalProcess.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistory.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistory.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistory.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistory.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistoryFamily.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistoryFamily.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistoryFamily.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistoryFamily.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFullExam.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColFullExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFullExam.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColFullExam.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExam.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExam.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExam.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSelect.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnSelect.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__QuaTrinhBenhLy.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__QuaTrinhBenhLy.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__TienSuBenhBenhNhan.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__TienSuBenhBenhNhan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__TienSuBenhGiaDinh.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__TienSuBenhGiaDinh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage7.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage8.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage9.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage10.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage11.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage12.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage13.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage14.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage15.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage16.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage17.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage18.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage19.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__TomTatCLS.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__TomTatCLS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__PhuongPhapDieuTri.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__PhuongPhapDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__HuongDieuTri.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__HuongDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamServiceTempCode.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lciExamServiceTempCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamServiceTempName.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lciExamServiceTempName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHospitalizationReason.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lciHospitalizationReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

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
                dicOrderTabIndexControl.Add("txtExamServiceTempCode", 0);
                dicOrderTabIndexControl.Add("txtExamServiceTempName", 1);
                dicOrderTabIndexControl.Add("txtHospitalizationReason", 2);
                dicOrderTabIndexControl.Add("txtPartExamCirculation", 3);
                dicOrderTabIndexControl.Add("txtPartExamRespiratory", 4);
                dicOrderTabIndexControl.Add("txtPartExamDigestion", 5);
                dicOrderTabIndexControl.Add("txtPartExamKidneyUrology", 6);
                dicOrderTabIndexControl.Add("txtPartExamNeurological", 7);
                dicOrderTabIndexControl.Add("txtPartExamMuscleBone", 8);
                dicOrderTabIndexControl.Add("txtPartExamEnt", 9);
                dicOrderTabIndexControl.Add("txtPartExamStomatology", 10);
                dicOrderTabIndexControl.Add("txtPartExamEye", 11);
                dicOrderTabIndexControl.Add("txtPartExamOend", 12);
                dicOrderTabIndexControl.Add("txtPartExamMental", 13);
                dicOrderTabIndexControl.Add("txtPartExamObstetric", 14);
                dicOrderTabIndexControl.Add("txtPartExamNutrition", 15);
                dicOrderTabIndexControl.Add("txtPartExamMotion", 16);
                dicOrderTabIndexControl.Add("txtPathologicalProcess", 17);
                dicOrderTabIndexControl.Add("txtPathologicalHistory", 18);
                dicOrderTabIndexControl.Add("txtPathologicalHistoryFamily", 19);
                dicOrderTabIndexControl.Add("txtFullExam", 20);
                dicOrderTabIndexControl.Add("txtPartExam", 21);


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
                Inventec.Core.ApiResultObject<List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>> apiResult = null;
                HisExamServiceTempFilter filter = new HisExamServiceTempFilter();
                filter.ORDER_DIRECTION = "MODIFY_TIME";
                filter.ORDER_FIELD = "ACS";
                SetFilterNavBar(ref filter);
                dnNavigation.DataSource = null;
                gridviewFormList.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>>(RequestUriStore.HIS_EXAM_SERVICE_TEMP_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>)apiResult.Data;
                    if (data != null)
                    {
                        dnNavigation.DataSource = data;
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

        private void SetFilterNavBar(ref HisExamServiceTempFilter filter)
        {
            try
            {
                filter.KEY_WORD__CODE__NAME = txtKeyword.Text.Trim();
                if (this.isCreatorOrPublic.HasValue && this.isCreatorOrPublic.Value)
                {
                    filter.PUBLIC_DEPARTMENT_ID = this.departmentID;
                }
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
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
                    var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
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
                    MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP pData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "IS_PUBLIC_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.IS_PUBLIC == 1 ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi cho phep nhap chi dinh lon hon 1 IS_LEAF_STR", ex);
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
                    else if (e.Column.FieldName == "PUBLIC_DEPARTMENT_STR")
                    {
                        try
                        {
                            e.Value = pData != null && pData.PUBLIC_DEPARTMENT_ID != null ? true : false;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
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
                this.currentData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
                if (this.currentData != null)
                {
                    ChangedDataRow(this.currentData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null &&
                    (this.currentData == null || (this.currentData != null && rowData.ID == this.currentData.ID)))
                {
                    this.currentData = rowData;
                    ChangedDataRow(this.currentData);

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
                    this.currentData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
                    if (this.currentData != null)
                    {
                        ChangedDataRow(this.currentData);

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

        private void dnNavigation_PositionChanged(object sender, EventArgs e)
        {
            try
            {
                this.currentData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)(gridControlFormList.DataSource as List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>)[dnNavigation.Position];
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

        private void ChangedDataRow(MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    ResetFormData();
                    FillDataToEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);

                    
                    if (data.CREATOR == this.LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                    {
                        btnEdit.ResetForeColor();
                        btnEdit.Enabled = true;
                        
                    }
                    else
                    {
                        btnEdit.ForeColor = Color.Gray;
                        btnEdit.Appearance.Options.UseForeColor = true; // Cho phép sử dụng màu chữ được đặt
                        btnEdit.Appearance.BackColor = System.Drawing.Color.LightGray; // Đặt màu nền là màu xám nhạt
                        btnEdit.Appearance.BackColor2 = System.Drawing.Color.LightGray;
                        btnEdit.Appearance.BorderColor = System.Drawing.Color.FromArgb(100, 100, 100); // Đặt màu đường viền là màu xám với độ trong suốt
                        //btnEdit.Appearance.Options.UseBorderColor = true;
                        //btnEdit.Refresh();

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

        private void FillDataToEditorControl(MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    txtExamServiceTempCode.Text = data.EXAM_SERVICE_TEMP_CODE;
                    txtExamServiceTempName.Text = data.EXAM_SERVICE_TEMP_NAME;
                    txtPartExamCirculation.Text = data.PART_EXAM_CIRCULATION;
                    txtPartExamRespiratory.Text = data.PART_EXAM_RESPIRATORY;
                    txtPartExamDigestion.Text = data.PART_EXAM_DIGESTION;
                    txtPART_EXAM_KIDNEY_UROLOGY.Text = data.PART_EXAM_KIDNEY_UROLOGY;
                    txtPART_EXAM_NEUROLOGICAL.Text = data.PART_EXAM_NEUROLOGICAL;
                    txtPART_EXAM_MUSCLE_BONE.Text = data.PART_EXAM_MUSCLE_BONE;
                    txtPART_EXAM_ENT.Text = data.PART_EXAM_ENT;
                    txtPART_EXAM_STOMATOLOGY.Text = data.PART_EXAM_STOMATOLOGY;
                    txtPART_EXAM_EYE.Text = data.PART_EXAM_EYE;
                    txtPART_EXAM_OEND.Text = data.PART_EXAM_OEND;
                    txtPART_EXAM_MENTAL.Text = data.PART_EXAM_MENTAL;
                    txtPART_EXAM_OBSTETRIC.Text = data.PART_EXAM_OBSTETRIC;
                    txtPART_EXAM_NUTRITION.Text = data.PART_EXAM_NUTRITION;
                    txtPART_EXAM_MOTION.Text = data.PART_EXAM_MOTION;
                    txtHospitalizationReason.Text = data.HOSPITALIZATION_REASON;
                    txtPathologicalProcess.Text = data.PATHOLOGICAL_PROCESS;
                    txtPathologicalHistory.Text = data.PATHOLOGICAL_HISTORY;
                    txtPathologicalHistoryFamily.Text = data.PATHOLOGICAL_HISTORY_FAMILY;
                    txtFullExam.Text = data.FULL_EXAM;
                    txtPartExam.Text = data.PART_EXAM;
                    txtDescription.Text = data.DESCRIPTION;
                    txtConclude.Text = data.CONCLUDE;
                    txtResultNote.Text = data.NOTE;
                    txtExamServiceTempCode.Text = data.EXAM_SERVICE_TEMP_CODE;
                    txtExamServiceTempName.Text = data.EXAM_SERVICE_TEMP_NAME;
                    chkPublic.Checked = (data.IS_PUBLIC == 1 ? true : false);
                    chkPublicDepartment.Checked = (data.PUBLIC_DEPARTMENT_ID != null ? true : false);
                    //
                    txtTai.Text = data.PART_EXAM_EAR;
                    txtHong.Text = data.PART_EXAM_THROAT;
                    txtMui.Text = data.PART_EXAM_NOSE;
                    txtPART_EXAM_EAR_RIGHT_NORMAL.Text = data.PART_EXAM_EAR_RIGHT_NORMAL;
                    txtPART_EXAM_EAR_RIGHT_WHISPER.Text = data.PART_EXAM_EAR_RIGHT_WHISPER;
                    txtPART_EXAM_EAR_LEFT_NORMAL.Text = data.PART_EXAM_EAR_LEFT_NORMAL;
                    txtPART_EXAM_EAR_LEFT_WHISPER.Text = data.PART_EXAM_EAR_LEFT_WHISPER;
                    //
                    txtPART_EXAM_UPPER_JAW.Text = data.PART_EXAM_UPPER_JAW;
                    txtPART_EXAM_LOWER_JAW.Text = data.PART_EXAM_LOWER_JAW;
                    txtRHM.Text = data.PART_EXAM_STOMATOLOGY;
                    //
                    txtMat.Text = data.PART_EXAM_EYE;
                    txtNhanApPhai.Text = data.PART_EXAM_EYE_TENSION_RIGHT;
                    txtNhanApTrai.Text = data.PART_EXAM_EYE_TENSION_LEFT;
                    txtThiLucKhongKinhPhai.Text = data.PART_EXAM_EYESIGHT_RIGHT;
                    txtThiLucKhongKinhTrai.Text = data.PART_EXAM_EYESIGHT_LEFT;
                    txtKinhLoPhai.Text = data.PART_EXAM_HOLE_GLASS_RIGHT;
                    txtKinhLoTrai.Text = data.PART_EXAM_HOLE_GLASS_LEFT;
                    ////
                    //if (data.PART_EXAM_DERMATOLOGY != null)
                    //{
                        txtDaLieu.Text = data.PART_EXAM_DERMATOLOGY;
                    //}

                    //
                    if (data.PART_EXAM_HORIZONTAL_SIGHT == 1)
                        chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = true;
                    else if (data.PART_EXAM_HORIZONTAL_SIGHT == 2)
                        chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = true;
                    else
                    {
                        chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = false;
                        chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = false;
                    }
                    if (data.PART_EXAM_VERTICAL_SIGHT == 1)
                        chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = true;
                    else if (data.PART_EXAM_VERTICAL_SIGHT == 2)
                        chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = true;
                    else
                    {
                        chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = false;
                        chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = false;
                    }

                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = (data.PART_EXAM_EYE_BLIND_COLOR == 1);

                    chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = (data.PART_EXAM_EYE_BLIND_COLOR == 2);


                    chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = (data.PART_EXAM_EYE_BLIND_COLOR == 3 || data.PART_EXAM_EYE_BLIND_COLOR == 6 || data.PART_EXAM_EYE_BLIND_COLOR == 7 || data.PART_EXAM_EYE_BLIND_COLOR == 9);

                    chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = (data.PART_EXAM_EYE_BLIND_COLOR == 4 || data.PART_EXAM_EYE_BLIND_COLOR == 6 || data.PART_EXAM_EYE_BLIND_COLOR == 8 || data.PART_EXAM_EYE_BLIND_COLOR == 9);

                    chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = (data.PART_EXAM_EYE_BLIND_COLOR == 5 || data.PART_EXAM_EYE_BLIND_COLOR == 7 || data.PART_EXAM_EYE_BLIND_COLOR == 8 || data.PART_EXAM_EYE_BLIND_COLOR == 9);

                    chkPartExamEyeStPlus.Checked = data.PART_EXAM_EYE_ST_PLUS == 1;
                    chkPartExamEyeStMinus.Checked = data.PART_EXAM_EYE_ST_MINUS == 1;
                    cboPartExamEyeTension.EditValue = data.PART_EXAM_EYE_TENSION;
                    txtPartExamEyeCountFinger.Text = data.PART_EXAM_EYE_COUNT_FINGER != null ? data.PART_EXAM_EYE_COUNT_FINGER.ToString() : null;

                    txtPartEyeGlassOldSphRight.Text = data.PART_EYE_GLASS_OLD_SPH_RIGHT != null ? data.PART_EYE_GLASS_OLD_SPH_RIGHT.ToString() : null;
                    txtPartEyeGlassOldCylRight.Text = data.PART_EYE_GLASS_OLD_CYL_RIGHT != null ? data.PART_EYE_GLASS_OLD_CYL_RIGHT.ToString() : null;
                    txtPartEyeGlassOldAxeRight.Text = data.PART_EYE_GLASS_OLD_AXE_RIGHT != null ? data.PART_EYE_GLASS_OLD_AXE_RIGHT.ToString() : null;
                    txtPartEyesightGlassOldRight.Text = data.PART_EYESIGHT_GLASS_OLD_RIGHT != null ? data.PART_EYESIGHT_GLASS_OLD_RIGHT.ToString() : null;
                    txtPartEyeGlassOldKcdtRight.Text = data.PART_EYE_GLASS_OLD_KCDT_RIGHT != null ? data.PART_EYE_GLASS_OLD_KCDT_RIGHT.ToString() : null;
                    txtPartEyeGlassOldAddRight.Text = data.PART_EYE_GLASS_OLD_ADD_RIGHT != null ? data.PART_EYE_GLASS_OLD_ADD_RIGHT.ToString() : null;

                    txtPartEyeGlassOldSphLeft.Text = data.PART_EYE_GLASS_OLD_SPH_LEFT != null ? data.PART_EYE_GLASS_OLD_SPH_LEFT.ToString() : null;
                    txtPartEyeGlassOldCylLeft.Text = data.PART_EYE_GLASS_OLD_CYL_LEFT != null ? data.PART_EYE_GLASS_OLD_CYL_LEFT.ToString() : null;
                    txtPartEyeGlassOldAxeLeft.Text = data.PART_EYE_GLASS_OLD_AXE_LEFT != null ? data.PART_EYE_GLASS_OLD_AXE_LEFT.ToString() : null;
                    txtPartEyesightGlassOldLeft.Text = data.PART_EYESIGHT_GLASS_OLD_LEFT != null ? data.PART_EYESIGHT_GLASS_OLD_LEFT.ToString() : null;
                    txtPartEyeGlassOldKcdtLeft.Text = data.PART_EYE_GLASS_OLD_KCDT_LEFT != null ? data.PART_EYE_GLASS_OLD_KCDT_LEFT.ToString() : null;
                    txtPartEyeGlassOldAddLeft.Text = data.PART_EYE_GLASS_OLD_ADD_LEFT != null ? data.PART_EYE_GLASS_OLD_ADD_LEFT.ToString() : null;

                    txtPartEyeGlassSphRight.Text = data.PART_EYE_GLASS_SPH_RIGHT != null ? data.PART_EYE_GLASS_SPH_RIGHT.ToString() : null;
                    txtPartEyeGlassCylRight.Text = data.PART_EYE_GLASS_CYL_RIGHT != null ? data.PART_EYE_GLASS_CYL_RIGHT.ToString() : null;
                    txtPartEyeGlassAxeRight.Text = data.PART_EYE_GLASS_AXE_RIGHT != null ? data.PART_EYE_GLASS_AXE_RIGHT.ToString() : null;
                    txtPartExamEyeSightGlassRight.Text = data.PART_EXAM_EYESIGHT_GLASS_RIGHT != null ? data.PART_EXAM_EYESIGHT_GLASS_RIGHT.ToString() : null;
                    txtPartEyeGlassKcdtRight.Text = data.PART_EYE_GLASS_KCDT_RIGHT != null ? data.PART_EYE_GLASS_KCDT_RIGHT.ToString() : null;
                    txtPartEyeGlassAddRight.Text = data.PART_EYE_GLASS_ADD_RIGHT != null ? data.PART_EYE_GLASS_ADD_RIGHT.ToString() : null;

                    txtPartEyeGlassSphLeft.Text = data.PART_EYE_GLASS_SPH_LEFT != null ? data.PART_EYE_GLASS_SPH_LEFT.ToString() : null;
                    txtPartEyeGlassCylLeft.Text = data.PART_EYE_GLASS_CYL_LEFT != null ? data.PART_EYE_GLASS_CYL_LEFT.ToString() : null;
                    txtPartEyeGlassAxeLeft.Text = data.PART_EYE_GLASS_AXE_LEFT != null ? data.PART_EYE_GLASS_AXE_LEFT.ToString() : null;
                    txtPartExamEyeSightGlassLeft.Text = data.PART_EXAM_EYESIGHT_GLASS_LEFT != null ? data.PART_EXAM_EYESIGHT_GLASS_LEFT.ToString() : null;
                    txtPartEyeGlassKcdtLeft.Text = data.PART_EYE_GLASS_KCDT_LEFT != null ? data.PART_EYE_GLASS_KCDT_LEFT.ToString() : null;
                    txtPartEyeGlassAddLeft.Text = data.PART_EYE_GLASS_ADD_LEFT != null ? data.PART_EYE_GLASS_ADD_LEFT.ToString() : null;
                    
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
                txtExamServiceTempCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void ResetControlText()
        {
            try
            {
                txtExamServiceTempCode.Text = "";
                txtExamServiceTempName.Text = "";
                txtPartExamCirculation.Text = "";
                txtPartExamRespiratory.Text = "";
                txtPartExamDigestion.Text = "";
                txtPART_EXAM_KIDNEY_UROLOGY.Text = "";
                txtPART_EXAM_NEUROLOGICAL.Text = "";
                txtPART_EXAM_MUSCLE_BONE.Text = "";
                txtPART_EXAM_ENT.Text = "";
                txtPART_EXAM_STOMATOLOGY.Text = "";
                txtPART_EXAM_EYE.Text = "";
                txtPART_EXAM_OEND.Text = "";
                txtPART_EXAM_MENTAL.Text = "";
                txtPART_EXAM_OBSTETRIC.Text = "";
                txtPART_EXAM_NUTRITION.Text = "";
                txtPART_EXAM_MOTION.Text = "";
                txtHospitalizationReason.Text = "";
                txtPathologicalProcess.Text = "";
                txtPathologicalHistory.Text = "";
                txtPathologicalHistoryFamily.Text = "";
                txtFullExam.Text = "";
                txtPartExam.Text = "";
                txtDescription.Text = "";
                txtConclude.Text = "";
                txtResultNote.Text = "";
                txtExamServiceTempCode.Text = "";
                txtExamServiceTempName.Text = "";
                //
                //
                txtTai.Text = "";
                txtHong.Text = "";
                txtMui.Text = "";
                txtPART_EXAM_EAR_RIGHT_NORMAL.Text = "";
                txtPART_EXAM_EAR_RIGHT_WHISPER.Text = "";
                txtPART_EXAM_EAR_LEFT_NORMAL.Text = "";
                txtPART_EXAM_EAR_LEFT_WHISPER.Text = "";
                //
                txtPART_EXAM_UPPER_JAW.Text = "";
                txtPART_EXAM_LOWER_JAW.Text = "";
                txtRHM.Text = "";
                //
                txtMat.Text = "";
                //
                txtDaLieu.Text = "";
                //
                chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = false;
                chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = false;
                chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
                chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = false;
                chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = false;
                chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = false;
                chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = false;
                chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = false;
                chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = false;
                chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = false;
                chkPublicDepartment.Checked = false;
                //
                txtNhanApPhai.Text = "";
                txtNhanApTrai.Text = "";
                txtThiLucKhongKinhPhai.Text = "";
                txtThiLucKhongKinhTrai.Text = "";
                txtKinhLoPhai.Text = "";
                txtKinhLoTrai.Text = "";

                ResetEye();
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
                if (!lcEditorInfo.IsInitialized) return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit && lci.Control.Name != chkLockInfor.Name)
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

        private void LoadCurrent(long currentId, ref MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExamServiceTempFilter filter = new HisExamServiceTempFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>>(RequestUriStore.HIS_EXAM_SERVICE_TEMP_GET, ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
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

        private void btnGSelect_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    ProcessSlected(rowData);
                    this.Close();
                }
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
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_EXAM_SERVICE_TEMP_DELETE, ApiConsumers.MosConsumer, rowData.ID, param);
                    if (success)
                    {
                        this.currentData = null;
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                positionHandle = -1;
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProviderEditorInfo, dxErrorProvider);
                ResetFormData();
                ResetControlText();
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
                if (btnEdit.ForeColor == Color.Gray) return;
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

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                UpdateDTOFromDataForm(ref currentData);
                ProcessSlected(this.currentData);
                WaitingManager.Hide();
                this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessSlected(MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP data)
        {
            try
            {
                if (this.selectData != null)
                {
                    this.selectData(data);
                }
                if (this.selectObjectData != null)
                {
                    this.selectObjectData(data, chkLockInfor.Checked);
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

                WaitingManager.Show();
                MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP updateDTO = new MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP();

                if (this.currentData != null && this.currentData.ID > 0)
                {
                    LoadCurrent(this.currentData.ID, ref updateDTO);
                }
                UpdateDTOFromDataForm(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>(RequestUriStore.HIS_EXAM_SERVICE_TEMP_CREATE, ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        success = true;
                        FillDataToGridControl();
                        ResetControlText();
                        ResetFormData();
                    }
                }
                else
                {
                    var resultData = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>(RequestUriStore.HIS_EXAM_SERVICE_TEMP_UPDATE, ApiConsumers.MosConsumer, updateDTO, param);
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

        private void UpdateRowDataAfterEdit(MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("data(MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP) is null");
                var rowData = (MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetFocusedRow();
                if (rowData != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP>(rowData, data);
                    gridviewFormList.RefreshRow(gridviewFormList.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataForm(MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP currentDTO)
        {
            try
            {

                txtExamServiceTempCode.Text = currentDTO.EXAM_SERVICE_TEMP_CODE;
                txtExamServiceTempName.Text = currentDTO.EXAM_SERVICE_TEMP_NAME;
                txtPartExamCirculation.Text = currentDTO.PART_EXAM_CIRCULATION;
                txtPartExamRespiratory.Text = currentDTO.PART_EXAM_RESPIRATORY;
                txtPartExamDigestion.Text = currentDTO.PART_EXAM_DIGESTION;
                txtPART_EXAM_KIDNEY_UROLOGY.Text = currentDTO.PART_EXAM_KIDNEY_UROLOGY;
                txtPART_EXAM_NEUROLOGICAL.Text = currentDTO.PART_EXAM_NEUROLOGICAL;
                txtPART_EXAM_MUSCLE_BONE.Text = currentDTO.PART_EXAM_MUSCLE_BONE;
                txtPART_EXAM_ENT.Text = currentDTO.PART_EXAM_ENT;
                txtPART_EXAM_STOMATOLOGY.Text = currentDTO.PART_EXAM_STOMATOLOGY;
                txtPART_EXAM_EYE.Text = currentDTO.PART_EXAM_EYE;
                txtPART_EXAM_OEND.Text = currentDTO.PART_EXAM_OEND;
                txtPART_EXAM_MENTAL.Text = currentDTO.PART_EXAM_MENTAL;
                txtPART_EXAM_OBSTETRIC.Text = currentDTO.PART_EXAM_OBSTETRIC;
                txtPART_EXAM_NUTRITION.Text = currentDTO.PART_EXAM_NUTRITION;
                txtPART_EXAM_MOTION.Text = currentDTO.PART_EXAM_MOTION;
                txtHospitalizationReason.Text = currentDTO.HOSPITALIZATION_REASON;
                txtPathologicalProcess.Text = currentDTO.PATHOLOGICAL_PROCESS;
                txtPathologicalHistory.Text = currentDTO.PATHOLOGICAL_HISTORY;
                txtPathologicalHistoryFamily.Text = currentDTO.PATHOLOGICAL_HISTORY_FAMILY;
                txtFullExam.Text = currentDTO.FULL_EXAM;
                txtPartExam.Text = currentDTO.PART_EXAM;
                txtDescription.Text = currentDTO.DESCRIPTION;
                txtConclude.Text = currentDTO.CONCLUDE;
                txtResultNote.Text = currentDTO.NOTE;
                txtExamServiceTempCode.Text = currentDTO.EXAM_SERVICE_TEMP_CODE;
                txtExamServiceTempName.Text = currentDTO.EXAM_SERVICE_TEMP_NAME;
                if (currentDTO.IS_PUBLIC == 1)
                {
                    chkPublic.Checked = true;
                }
                else
                {
                    chkPublic.Checked = false;
                }
                //currentDTO.IS_PUBLIC = (short)(chkPublic.Checked ? 1 : 0);
                //  
                txtTai.Text = currentDTO.PART_EXAM_EAR;
                txtMui.Text = currentDTO.PART_EXAM_NOSE;
                txtHong.Text = currentDTO.PART_EXAM_THROAT;
                txtPART_EXAM_EAR_RIGHT_NORMAL.Text = currentDTO.PART_EXAM_EAR_RIGHT_NORMAL;
                txtPART_EXAM_EAR_RIGHT_WHISPER.Text = currentDTO.PART_EXAM_EAR_RIGHT_WHISPER;
                txtPART_EXAM_EAR_LEFT_NORMAL.Text = currentDTO.PART_EXAM_EAR_LEFT_NORMAL;
                txtPART_EXAM_EAR_LEFT_WHISPER.Text = currentDTO.PART_EXAM_EAR_LEFT_WHISPER;
                //
                txtPART_EXAM_UPPER_JAW.Text = currentDTO.PART_EXAM_UPPER_JAW;
                txtPART_EXAM_LOWER_JAW.Text = currentDTO.PART_EXAM_LOWER_JAW;
                txtRHM.Text = currentDTO.PART_EXAM_STOMATOLOGY;
                //
                txtMat.Text = currentDTO.PART_EXAM_EYE;
                txtNhanApPhai.Text = currentDTO.PART_EXAM_EYE_TENSION_RIGHT;
                txtNhanApTrai.Text = currentDTO.PART_EXAM_EYE_TENSION_LEFT;
                txtThiLucKhongKinhPhai.Text = currentDTO.PART_EXAM_EYESIGHT_RIGHT;
                txtThiLucKhongKinhTrai.Text = currentDTO.PART_EXAM_EYESIGHT_LEFT;
                txtKinhLoPhai.Text = currentDTO.PART_EXAM_HOLE_GLASS_RIGHT;
                txtKinhLoTrai.Text = currentDTO.PART_EXAM_HOLE_GLASS_LEFT;
                //
                txtDaLieu.Text = currentDTO.PART_EXAM_DERMATOLOGY;
                //
                if (currentDTO.PART_EXAM_HORIZONTAL_SIGHT == 1)
                {
                    chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked = true;
                }
                if (currentDTO.PART_EXAM_HORIZONTAL_SIGHT == 2)
                {
                    chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked = true;
                }


                if (currentDTO.PART_EXAM_VERTICAL_SIGHT == 1)
                {
                    chkPART_EXAM_VERTICAL_SIGHT__BT.Checked = true;
                }
                if (currentDTO.PART_EXAM_VERTICAL_SIGHT == 2)
                {
                    chkPART_EXAM_VERTICAL_SIGHT__HC.Checked = true;
                }

                //if (chkPART_EXAM_VERTICAL_SIGHT__BT.Checked)
                //    currentDTO.PART_EXAM_VERTICAL_SIGHT = 1;
                //else if (chkPART_EXAM_VERTICAL_SIGHT__HC.Checked)
                //    currentDTO.PART_EXAM_VERTICAL_SIGHT = 2;
                //else
                //{
                //    currentDTO.PART_EXAM_VERTICAL_SIGHT = 0;
                //}

                //
                if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 1)
                {
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = true;
                }
                else

                    if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 2)
                    {
                        chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = true;
                    }
                    else
                    {
                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 9)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = true;
                            chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = true;
                            chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = true;
                        }

                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 8)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = true;
                            chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = true;
                        }
                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 7)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = true;
                            chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = true;
                        }
                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 6)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = true;
                            chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = true;
                        }
                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 5)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = true;
                        }
                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 4)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = true;
                        }
                        if (currentDTO.PART_EXAM_EYE_BLIND_COLOR == 3)
                        {
                            chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = true;
                        }
                        
                    }

                //if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked)
                //    currentDTO.PART_EXAM_EYE_BLIND_COLOR = 1;
                //else if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked)
                //    currentDTO.PART_EXAM_EYE_BLIND_COLOR = 2;
                //else
                //{
                //    if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 9;
                //    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 8;
                //    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 7;
                //    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 6;
                //    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 5;
                //    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 4;
                //    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked)
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 3;
                //    else
                //        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 0;
                //}


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref MOS.EFMODEL.DataModels.HIS_EXAM_SERVICE_TEMP currentDTO)
        {
            try
            {
                currentDTO.EXAM_SERVICE_TEMP_CODE = txtExamServiceTempCode.Text.Trim();
                currentDTO.EXAM_SERVICE_TEMP_NAME = txtExamServiceTempName.Text.Trim();
                currentDTO.PART_EXAM_CIRCULATION = txtPartExamCirculation.Text.Trim();
                currentDTO.PART_EXAM_RESPIRATORY = txtPartExamRespiratory.Text.Trim();
                currentDTO.PART_EXAM_DIGESTION = txtPartExamDigestion.Text.Trim();
                currentDTO.PART_EXAM_KIDNEY_UROLOGY = txtPART_EXAM_KIDNEY_UROLOGY.Text.Trim();
                currentDTO.PART_EXAM_NEUROLOGICAL = txtPART_EXAM_NEUROLOGICAL.Text.Trim();
                currentDTO.PART_EXAM_MUSCLE_BONE = txtPART_EXAM_MUSCLE_BONE.Text.Trim();
                currentDTO.PART_EXAM_ENT = txtPART_EXAM_ENT.Text.Trim();
                currentDTO.PART_EXAM_STOMATOLOGY = txtPART_EXAM_STOMATOLOGY.Text.Trim();
                currentDTO.PART_EXAM_EYE = txtPART_EXAM_EYE.Text.Trim();
                currentDTO.PART_EXAM_OEND = txtPART_EXAM_OEND.Text.Trim();
                currentDTO.PART_EXAM_MENTAL = txtPART_EXAM_MENTAL.Text.Trim();
                currentDTO.PART_EXAM_OBSTETRIC = txtPART_EXAM_OBSTETRIC.Text.Trim();
                currentDTO.PART_EXAM_NUTRITION = txtPART_EXAM_NUTRITION.Text.Trim();
                currentDTO.PART_EXAM_MOTION = txtPART_EXAM_MOTION.Text.Trim();
                currentDTO.HOSPITALIZATION_REASON = txtHospitalizationReason.Text.Trim();
                currentDTO.PATHOLOGICAL_PROCESS = txtPathologicalProcess.Text.Trim();
                currentDTO.PATHOLOGICAL_HISTORY = txtPathologicalHistory.Text.Trim();
                currentDTO.PATHOLOGICAL_HISTORY_FAMILY = txtPathologicalHistoryFamily.Text.Trim();
                currentDTO.FULL_EXAM = txtFullExam.Text.Trim();
                currentDTO.PART_EXAM = txtPartExam.Text.Trim();
                currentDTO.DESCRIPTION = txtDescription.Text.Trim();
                currentDTO.CONCLUDE = txtConclude.Text.Trim();
                currentDTO.NOTE = txtResultNote.Text.Trim();
                currentDTO.EXAM_SERVICE_TEMP_CODE = txtExamServiceTempCode.Text;
                currentDTO.EXAM_SERVICE_TEMP_NAME = txtExamServiceTempName.Text.Trim();
                currentDTO.IS_PUBLIC = (short)(chkPublic.Checked ? 1 : 0);
                //  
                currentDTO.PART_EXAM_EAR = txtTai.Text;
                currentDTO.PART_EXAM_NOSE = txtMui.Text;
                currentDTO.PART_EXAM_THROAT = txtHong.Text;
                currentDTO.PART_EXAM_EAR_RIGHT_NORMAL = txtPART_EXAM_EAR_RIGHT_NORMAL.Text;
                currentDTO.PART_EXAM_EAR_RIGHT_WHISPER = txtPART_EXAM_EAR_RIGHT_WHISPER.Text;
                currentDTO.PART_EXAM_EAR_LEFT_NORMAL = txtPART_EXAM_EAR_LEFT_NORMAL.Text;
                currentDTO.PART_EXAM_EAR_LEFT_WHISPER = txtPART_EXAM_EAR_LEFT_WHISPER.Text;
                //
                currentDTO.PART_EXAM_UPPER_JAW = txtPART_EXAM_UPPER_JAW.Text;
                currentDTO.PART_EXAM_LOWER_JAW = txtPART_EXAM_LOWER_JAW.Text;
                currentDTO.PART_EXAM_STOMATOLOGY = txtRHM.Text;
                //
                currentDTO.PART_EXAM_EYE = txtMat.Text;
                currentDTO.PART_EXAM_EYE_TENSION_RIGHT = txtNhanApPhai.Text;
                currentDTO.PART_EXAM_EYE_TENSION_LEFT = txtNhanApTrai.Text;
                currentDTO.PART_EXAM_EYESIGHT_RIGHT = txtThiLucKhongKinhPhai.Text;
                currentDTO.PART_EXAM_EYESIGHT_LEFT = txtThiLucKhongKinhTrai.Text;

                currentDTO.PART_EXAM_HOLE_GLASS_RIGHT = txtKinhLoPhai.Text;
                currentDTO.PART_EXAM_HOLE_GLASS_LEFT = txtKinhLoTrai.Text;
                //
                currentDTO.PART_EXAM_DERMATOLOGY = txtDaLieu.Text;
                //
                if (chkPART_EXAM_HORIZONTAL_SIGHT__BT.Checked)
                    currentDTO.PART_EXAM_HORIZONTAL_SIGHT = 1;
                else if (chkPART_EXAM_HORIZONTAL_SIGHT__HC.Checked)
                    currentDTO.PART_EXAM_HORIZONTAL_SIGHT = 2;
                else
                {
                    currentDTO.PART_EXAM_HORIZONTAL_SIGHT = 0;
                }

                if (chkPART_EXAM_VERTICAL_SIGHT__BT.Checked)
                    currentDTO.PART_EXAM_VERTICAL_SIGHT = 1;
                else if (chkPART_EXAM_VERTICAL_SIGHT__HC.Checked)
                    currentDTO.PART_EXAM_VERTICAL_SIGHT = 2;
                else
                {
                    currentDTO.PART_EXAM_VERTICAL_SIGHT = 0;
                }

                //
                if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked)
                    currentDTO.PART_EXAM_EYE_BLIND_COLOR = 1;
                else if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked)
                    currentDTO.PART_EXAM_EYE_BLIND_COLOR = 2;
                else
                {
                    if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 9;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 8;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 7;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked && chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 6;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 5;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 4;
                    else if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked)
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 3;
                    else
                        currentDTO.PART_EXAM_EYE_BLIND_COLOR = 0;
                }

                currentDTO.PART_EXAM_EYE_ST_PLUS = chkPartExamEyeStPlus.Checked ? (short?)1 : 0;
                currentDTO.PART_EXAM_EYE_ST_MINUS = chkPartExamEyeStMinus.Checked ? (short?)1 : 0;
                currentDTO.PART_EXAM_EYE_TENSION = cboPartExamEyeTension.SelectedIndex != -1 ? cboPartExamEyeTension.EditValue.ToString() : null;
                if (!string.IsNullOrEmpty(txtPartExamEyeCountFinger.Text.Trim()))
                    currentDTO.PART_EXAM_EYE_COUNT_FINGER = txtPartExamEyeCountFinger.Text.Trim();

                currentDTO.PART_EYE_GLASS_OLD_SPH_RIGHT = txtPartEyeGlassOldSphRight.EditValue != null ? txtPartEyeGlassOldSphRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_CYL_RIGHT = txtPartEyeGlassOldCylRight.EditValue != null ? txtPartEyeGlassOldCylRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_AXE_RIGHT = txtPartEyeGlassOldAxeRight.EditValue != null ? txtPartEyeGlassOldAxeRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYESIGHT_GLASS_OLD_RIGHT = txtPartEyesightGlassOldRight.EditValue != null ? txtPartEyesightGlassOldRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_KCDT_RIGHT = txtPartEyeGlassOldKcdtRight.EditValue != null ? txtPartEyeGlassOldKcdtRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_ADD_RIGHT = txtPartEyeGlassOldAddRight.EditValue != null ? txtPartEyeGlassOldAddRight.Text.Trim().ToString() : null;

                currentDTO.PART_EYE_GLASS_OLD_SPH_LEFT = txtPartEyeGlassOldSphLeft.EditValue != null ? txtPartEyeGlassOldSphLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_CYL_LEFT = txtPartEyeGlassOldCylLeft.EditValue != null ? txtPartEyeGlassOldCylLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_AXE_LEFT = txtPartEyeGlassOldAxeLeft.EditValue != null ? txtPartEyeGlassOldAxeLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYESIGHT_GLASS_OLD_LEFT = txtPartEyesightGlassOldLeft.EditValue != null ? txtPartEyesightGlassOldLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_KCDT_LEFT = txtPartEyeGlassOldKcdtLeft.EditValue != null ? txtPartEyeGlassOldKcdtLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_OLD_ADD_LEFT = txtPartEyeGlassOldAddLeft.EditValue != null ? txtPartEyeGlassOldAddLeft.Text.Trim().ToString() : null;

                currentDTO.PART_EYE_GLASS_SPH_RIGHT = txtPartEyeGlassSphRight.EditValue != null ? txtPartEyeGlassSphRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_CYL_RIGHT = txtPartEyeGlassCylRight.EditValue != null ? txtPartEyeGlassCylRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_AXE_RIGHT = txtPartEyeGlassAxeRight.EditValue != null ? txtPartEyeGlassAxeRight.Text.Trim().ToString() : null;
                currentDTO.PART_EXAM_EYESIGHT_GLASS_RIGHT = txtPartExamEyeSightGlassRight.EditValue != null ? txtPartExamEyeSightGlassRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_KCDT_RIGHT = txtPartEyeGlassKcdtRight.EditValue != null ? txtPartEyeGlassKcdtRight.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_ADD_RIGHT = txtPartEyeGlassAddRight.EditValue != null ? txtPartEyeGlassAddRight.Text.Trim().ToString() : null;

                currentDTO.PART_EYE_GLASS_SPH_LEFT = txtPartEyeGlassSphLeft.EditValue != null ? txtPartEyeGlassSphLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_CYL_LEFT = txtPartEyeGlassCylLeft.EditValue != null ? txtPartEyeGlassCylLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_AXE_LEFT = txtPartEyeGlassAxeLeft.EditValue != null ? txtPartEyeGlassAxeLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EXAM_EYESIGHT_GLASS_LEFT = txtPartExamEyeSightGlassLeft.EditValue != null ? txtPartExamEyeSightGlassLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_KCDT_LEFT = txtPartEyeGlassKcdtLeft.EditValue != null ? txtPartEyeGlassKcdtLeft.Text.Trim().ToString() : null;
                currentDTO.PART_EYE_GLASS_ADD_LEFT = txtPartEyeGlassAddLeft.EditValue != null ? txtPartEyeGlassAddLeft.Text.Trim().ToString() : null;
                if (chkPublicDepartment.Checked == true)
                {
                    if(this.departmentID != 0)
                        currentDTO.PUBLIC_DEPARTMENT_ID = this.departmentID;
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
                ValidationSingleControl(txtExamServiceTempCode);
                ValidationSingleControl(txtExamServiceTempName);
                ValidationControlMaxLength(txtPartExamCirculation, 500);
                ValidationControlMaxLength(txtPartExamRespiratory, 500);
                ValidationControlMaxLength(txtPART_EXAM_KIDNEY_UROLOGY, 500);
                ValidationControlMaxLength(txtPART_EXAM_NEUROLOGICAL, 500);
                ValidationControlMaxLength(txtPART_EXAM_MUSCLE_BONE, 500);
                ValidationControlMaxLength(txtPART_EXAM_ENT, 500);
                ValidationControlMaxLength(txtPART_EXAM_STOMATOLOGY, 500);
                ValidationControlMaxLength(txtPART_EXAM_EYE, 500);
                ValidationControlMaxLength(txtPART_EXAM_OEND, 500);
                ValidationControlMaxLength(txtPART_EXAM_MENTAL, 500);
                ValidationControlMaxLength(txtPART_EXAM_OBSTETRIC, 500);
                ValidationControlMaxLength(txtPART_EXAM_NUTRITION, 500);
                ValidationControlMaxLength(txtPART_EXAM_MOTION, 500);
                ValidationControlMaxLength(txtFullExam, 4000);
                ValidationControlMaxLength(txtPathologicalProcess, 4000);
                ValidationControlMaxLength(txtPathologicalHistory, 500);
                ValidationControlMaxLength(txtPathologicalHistoryFamily, 500);
                ValidationControlMaxLength(txtDescription, 1000);
                ValidationControlMaxLength(txtConclude, 1000);
                ValidationControlMaxLength(txtResultNote, 500);
                //ValidationSingleControl(txtPartExamCirculation);
                //ValidationSingleControl(txtPartExamRespiratory);
                //ValidationSingleControl(txtPartExamDigestion);
                //ValidationSingleControl(txtPART_EXAM_KIDNEY_UROLOGY);
                //ValidationSingleControl(txtPART_EXAM_NEUROLOGICAL);
                //ValidationSingleControl(txtPART_EXAM_MUSCLE_BONE);
                //ValidationSingleControl(txtPART_EXAM_ENT);
                //ValidationSingleControl(txtPART_EXAM_STOMATOLOGY);
                //ValidationSingleControl(txtPART_EXAM_EYE);
                //ValidationSingleControl(txtPART_EXAM_OEND);
                //ValidationSingleControl(txtPART_EXAM_MENTAL);
                //ValidationSingleControl(txtPART_EXAM_OBSTETRIC);
                //ValidationSingleControl(txtPART_EXAM_NUTRITION);
                //ValidationSingleControl(txtPART_EXAM_MOTION);
                //ValidationSingleControl(txtHospitalizationReason);
                //ValidationSingleControl(txtPathologicalProcess);
                //ValidationSingleControl(txtPathologicalHistory);
                //ValidationSingleControl(txtPathologicalHistoryFamily);
                //ValidationSingleControl(txtFullExam);
                //ValidationSingleControl(txtPartExam);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.ErrorText = "Trường nhập quá dài";
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProviderEditorInfo.SetValidationRule(control, validate);
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

        void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkLockInfor.Name)
                        {
                            chkLockInfor.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                //load department
                LoadDepartment();
                //Gan gia tri mac dinh
                SetDefaultValue();
                InitControlState();
                if (this.selectData != null || this.selectObjectData != null)
                {
                    gcolGSelect.Visible = true;
                    btnSelect.Enabled = true;
                }
                else
                {
                    gcolGSelect.Visible = false;
                    btnSelect.Enabled = false;
                }
                //Set enable control default
                EnableControlChanged(this.ActionType);

                //Fill data into datasource combo
                FillDataToControlsForm();

                //Load du lieu
                FillDataToGridControl();

                //Load ngon ngu label control
                SetCaptionByLanguageKey();

                //Set tabindex control
                InitTabIndex();

                //Set validate rule
                ValidateForm();

                //Focus default
                SetDefaultFocus();

                
                if (data != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    FillDataForm(data);
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDepartment()
        {
            try
            {
                chkPublicDepartment.Enabled = false;
                if (roomID != 0)
                {
                    var currentRoom = BackendDataWorker.Get<HIS_ROOM>().Where(s => s.ID == roomID).FirstOrDefault();
                    if (currentRoom != null)
                    {
                        chkPublicDepartment.Enabled = true;
                        this.departmentID = currentRoom.DEPARTMENT_ID;
                    }
                }
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

        private void bbtnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSelect_Click(null, null);
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
                    var data = (HIS_EXAM_SERVICE_TEMP)gridviewFormList.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "DELETE_ITEM")
                    {
                        if (data.CREATOR == this.LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                        {
                            e.RepositoryItem = btnGDelete_enable;
                        }
                        else
                        {
                            e.RepositoryItem = btnGDelete_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void txtPART_EXAM_ENT_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void SpinKeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
       (e.KeyChar != '.') && (e.KeyChar != ','))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPART_EXAM_EAR_RIGHT_NORMAL_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_LEFT_NORMAL_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_RIGHT_WHISPER_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void txtPART_EXAM_EAR_LEFT_WHISPER_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtPART_EXAM_EAR_LEFT_WHISPER_KeyPress(object sender, KeyPressEventArgs e)
        {
            SpinKeyPress(sender, e);
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__BT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked)
                {
                    chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = false;
                    chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = !chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked;
                }
                Inventec.Common.Logging.LogSystem.Debug("chkPART_EXAM_EYE_BLIND_COLOR__BT_CheckedChanged: chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked + "____chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMTB_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked)
                {
                    chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked = false;
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = !chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked;
                }
                Inventec.Common.Logging.LogSystem.Debug("chkPART_EXAM_EYE_BLIND_COLOR__MMTB_CheckedChanged: chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked + "____chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked=" + chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMD.Checked)
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMXLC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Checked)
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkPART_EXAM_EYE_BLIND_COLOR__MMV_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPART_EXAM_EYE_BLIND_COLOR__MMV.Checked)
                    chkPART_EXAM_EYE_BLIND_COLOR__BT.Checked = chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPartEyeGlassOldSphRight_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ',' && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != (char)Keys.Back)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ResetEye()
        {
            try
            {
                cboPartExamEyeTension.SelectedIndex = -1;
                chkPartExamEyeStMinus.Checked = false;
                chkPartExamEyeStPlus.Checked = false;
                txtPartExamEyeCountFinger.Text = null;

                txtPartEyeGlassOldSphRight.Text = null;
                txtPartEyeGlassOldCylRight.Text = null;
                txtPartEyeGlassOldAxeRight.Text = null;
                txtPartEyesightGlassOldRight.Text = null;
                txtPartEyeGlassOldKcdtRight.Text = null;
                txtPartEyeGlassOldAddRight.Text = null;

                txtPartEyeGlassOldSphLeft.Text = null;
                txtPartEyeGlassOldCylLeft.Text = null;
                txtPartEyeGlassOldAxeLeft.Text = null;
                txtPartEyesightGlassOldLeft.Text = null;
                txtPartEyeGlassOldKcdtLeft.Text = null;
                txtPartEyeGlassOldAddLeft.Text = null;

                txtPartEyeGlassSphRight.Text = null;
                txtPartEyeGlassCylRight.Text = null;
                txtPartEyeGlassAxeRight.Text = null;
                txtPartExamEyeSightGlassRight.Text = null;
                txtPartEyeGlassKcdtRight.Text = null;
                txtPartEyeGlassAddRight.Text = null;

                txtPartEyeGlassSphLeft.Text = null;
                txtPartEyeGlassCylLeft.Text = null;
                txtPartEyeGlassAxeLeft.Text = null;
                txtPartExamEyeSightGlassLeft.Text = null;
                txtPartEyeGlassKcdtLeft.Text = null;
                txtPartEyeGlassAddLeft.Text = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPartExamEyeStPlus_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPartExamEyeStPlus.Checked)
                chkPartExamEyeStMinus.Checked = false;
        }

        private void chkPartExamEyeStMinus_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPartExamEyeStMinus.Checked)
                chkPartExamEyeStPlus.Checked = false;
        }

        private void txtPartExamEyeCountFinger_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblCountFinger.Text = "";
                if (!String.IsNullOrWhiteSpace(txtPartExamEyeCountFinger.Text.Trim()))
                {
                    bool isNum = true;
                    foreach (var item in txtPartExamEyeCountFinger.Text.Trim())
                    {
                        if (!char.IsNumber(item))
                        {
                            isNum = false;
                            break;
                        }
                    }

                    if (isNum)
                    {
                        lblCountFinger.Text = "mét";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPartExamEyeTension_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                cboPartExamEyeTension.Properties.Buttons[1].Visible = cboPartExamEyeTension.SelectedIndex != -1 ? true : false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPartExamEyeTension_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboPartExamEyeTension.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkLockInfor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkLockInfor.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkLockInfor.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkLockInfor.Name;
                    csAddOrUpdate.VALUE = (chkLockInfor.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl == btnEdit && btnEdit.ForeColor == Color.Gray)
                {
                    e.Info = new ToolTipControlInfo(btnEdit, "Chỉ cho phép đối với người tạo hoặc tài khoản quản trị");
                }
                
            }
            catch (Exception ex)
            {
                
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmHisExamServiceTemp_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var control = GetChildAtPoint(PointToClient(Cursor.Position));
                var button = control as SimpleButton;
                if (button != null && button == btnEdit && !button.Enabled)
                {
                    toolTipController1.ShowHint("Chỉ cho phép đối với người tạo hoặc tài khoản quản trị", MousePosition);
                }
                else
                {
                    toolTipController1.HideHint();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        
    }
}
