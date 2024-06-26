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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
using HIS.Desktop.Common;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using System.Threading;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Plugins.ImpMestAggregate.ADO;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors.Repository;
using System.Globalization;

namespace HIS.Desktop.Plugins.ImpMestAggregate
{
    public partial class UCImpMestAggregate : HIS.Desktop.Utility.UserControlBase
    {
        int lastRowHandle = -1;
        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        CommonParam param = new CommonParam();
        int rowCount = 0;
        int dataTotal = 0;
        long aggrImpMestId = 0;
        int rowCountImpM = 0;
        int dataTotalImpM = 0;
        internal List<V_HIS_IMP_MEST_2> currentAggrImpMest { get; set; }
        internal List<V_HIS_IMP_MEST_2> _ImpMestChecks { get; set; }
        internal List<V_HIS_MEST_ROOM> lstMestRoom { get; set; }
        internal V_HIS_IMP_MEST_2 currentImpMest { get; set; }

        internal List<ImpMestADO> _ImpMestADOs = new List<ImpMestADO>();
        bool isCheckAll = true;

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> impMestManuMetyMatys = new List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> impMestMedicines = new List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> impMestMaterialDs = new List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();

        public UCImpMestAggregate()
        {
            InitializeComponent();
        }

        public UCImpMestAggregate(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCImpMestAggregate_Load(object sender, EventArgs e)
        {
            try
            {
                //SetDefaultValue();
                HisConfigCFG.LoadConfig();
                SetCaptionByLanguageKey();

                this.gridControlMobaImpMest.ToolTipController = this.toolTipController1;
                this.gridControlAggrImpMest.ToolTipController = this.toolTipController1;
                this.gridControlImpMestReq.ToolTipController = this.toolTipController1;

                //LoadDataToComboMediStock();
                InitMediStockCheck();

                InitComboMediStock();
                SetDataDefaultUC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ImpMestAggregate.Resources.Lang", typeof(HIS.Desktop.Plugins.ImpMestAggregate.UCImpMestAggregate).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAggrOddImpMest.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.btnAggrOddImpMest.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColllStatus.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColllStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpImportCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpImportCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpImportRoom.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpImportRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPatientName.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpUseTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpUseTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpReqUserName.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollStatus.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdCollStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdCollStatus.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColDelete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColDelete.ToolTip = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSumPrint.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColSumPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSumPrint.ToolTip = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColSumPrint.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVotePayCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColVotePayCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColRepositoryImport.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColRepositoryImport.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColImportTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColImportTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreatorImport.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColCreatorImport.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColServiceUnit.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColServiceUnit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColAmount.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBidNumber.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColBidNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColStatus.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColImportCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColImportCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColImportRoom.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColImportRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientName.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColUseTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColUseTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColReqUserName.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColReqUserName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIntructionTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColIntructionTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColImpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColImpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIntructionDate.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.grdColIntructionDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControl1.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.navBarControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.navCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciFromTime.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.lciFromTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciToTime.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.lciToTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkNotSynthetic.Properties.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.chkNotSynthetic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSynthesized.Properties.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.chkSynthesized.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl14.Text = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.layoutControl14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupMediStock.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.navBarGroupMediStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navStatus.Caption = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.navStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeywordProcess.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCImpMestAggregate.txtKeywordProcess.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                txtKeywordProcess.Text = "";
                dtFromTime.EditValue = DateTime.Now;
                dtFromTime.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, DateTime.Now.Second);
                dtToTime.EditValue = DateTime.Now;
                dtToTime.EditValue = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, DateTime.Now.Second);
                chkNotSynthetic.Checked = true;
                chkSynthesized.Checked = false;
                gridControlMobaImpMest.DataSource = null;
                gridControlDetailImpMest.DataSource = null;
                gridControlAggrImpMest.DataSource = null;
                gridControlImpMestReq.DataSource = null;

            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataDefaultUC()
        {
            try
            {
                SetDefaultValue();
                LoadDataMobaImpMest();
                LoadDataAggrImpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMobaImpMest_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 data = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IMP_MEST_STT_DISPLAY")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        long status = data.IMP_MEST_STT_ID;
                        //Status
                        //trang: dang gui YC : màu vàng
                        if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu đỏ
                        else if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet mau den
                        else if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                    }
                    else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlMobaImpMest)
                {
                    DetailToolTipControl(gridControlMobaImpMest, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlAggrImpMest)
                {
                    DetailToolTipControl(gridControlAggrImpMest, e);
                }
                else if (e.Info == null && e.SelectedControl == gridControlImpMestReq)
                {
                    DetailToolTipControl(gridControlImpMestReq, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DetailToolTipControl(DevExpress.XtraGrid.GridControl gridControl, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                if (info.InRowCell)
                {
                    if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                    {
                        lastColumn = info.Column;
                        lastRowHandle = info.RowHandle;

                        string text = "";
                        if (info.Column.FieldName == "IMP_MEST_STT_DISPLAY")
                        {
                            text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                        }

                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDetailImpMest_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrImpMest_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 data = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "IMP_MEST_STT_DISPLAY")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        long status = data.IMP_MEST_STT_ID;
                        //Status
                        //trang: dang gui YC : màu vàng
                        if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        //Trang thai: da duyet màu xanh
                        else if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__APPROVAL)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        //trang thai: da hoan thanh-da xuat: màu do
                        else if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__IMPORT)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        //trang thai: tu choi duyet : mau den
                        else if (status == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                    }

                    else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrImpMest_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                    var roomId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).RoomId;
                    long impMestSttId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewAggrImpMest.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    long reqRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewAggrImpMest.GetRowCellValue(e.RowHandle, "REQ_ROOM_ID") ?? "").ToString());
                    string creator = (gridViewAggrImpMest.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var data = (V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE_DISPLAY")
                        {
                            if (data.REQ_DEPARTMENT_ID == departmentId
                                && (impMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT
                                || impMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT
                                || impMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST))
                            {
                                if (HisConfigCFG.isEnableDeleteAggregate)
                                {
                                    if ((CheckLoginAdmin.IsAdmin(loginName) || creator.Equals(loginName)) && reqRoomId == roomId)
                                    {
                                        e.RepositoryItem = repositoryItemButtonEdit_Delete;
                                    }
                                    else
                                    {
                                        e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                                    }
                                }
                                else
                                    e.RepositoryItem = repositoryItemButtonEdit_Delete;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemButtonEdit_DeleteDisable;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridControlAggrImpMest_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetFocusedRow();
                if (row != null)
                {
                    this.aggrImpMestId = row.ID;
                    LoadDataDetailAggrImpMest();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Delete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2 HisAggrImpMestRow = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetFocusedRow();
                if (HisAggrImpMestRow != null && DevExpress.XtraEditors.XtraMessageBox.Show(
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong),
                    MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MOS.EFMODEL.DataModels.HIS_IMP_MEST impMest = new HIS_IMP_MEST();
                    impMest.ID = HisAggrImpMestRow.ID;
                    if (impMest != null)
                    {
                        var result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_IMP_MEST_DELETE, ApiConsumers.MosConsumer, impMest, param);
                        if (result == true)
                        {
                            success = true;
                            btnRefresh_Click(null, null);
                        }
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void buttonEditPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var currentAggrImpMest = (V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetFocusedRow();
                if (currentAggrImpMest != null)
                {
                    PrintAggregateImpMest(currentAggrImpMest);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAggrOddImpMest_Click(object sender, EventArgs e)
        {
            try
            {
                string messages = "";
                if (gridViewMobaImpMest.RowCount > 0)
                {
                    _ImpMestChecks = new List<V_HIS_IMP_MEST_2>();
                    if (this._ImpMestADOs != null && this._ImpMestADOs.Count > 0)
                    {
                        _ImpMestChecks.AddRange(this._ImpMestADOs.Where(p => p.IsCheck == true && p.AGGR_IMP_MEST_ID == null && p.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST).ToList());
                    }

                    if (_ImpMestChecks != null && _ImpMestChecks.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.AggrMobaImpMests").FirstOrDefault();
                        if (moduleData == null)
                        {
                            Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.AggrMobaImpMests");
                            return;
                        }
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            List<object> listArgs = new List<object>();
                            listArgs.Add(_ImpMestChecks);
                            listArgs.Add((HIS.Desktop.Common.RefeshReference)SetDataDefaultUC);
                            listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                            if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                            ((Form)extenceInstance).ShowDialog();
                        }
                        WaitingManager.Hide();
                    }
                    else
                        messages = "Chưa chọn phiếu cần tổng hợp";
                }
                else
                    messages = "Dữ liệu rỗng";
                if (!String.IsNullOrEmpty(messages))
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(messages, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataMobaImpMest();
                LoadDataAggrImpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDataDefaultUC();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch123()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh123()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void keyF2FocusedKeyWord()
        {
            try
            {
                txtKeywordProcess.Focus();
                txtKeywordProcess.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnAggrOddImpMest123()
        {
            try
            {
                btnAggrOddImpMest_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMobaImpMest_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "DX$CheckboxSelectorColumn")
                    return;
                if (chkThuocVTTheoPhieuNhap.Checked)
                {
                    gridControlDetailImpMest.DataSource = null;
                    var row = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_2)gridViewMobaImpMest.GetFocusedRow();
                    if (row != null)
                    {
                        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> lstMedicinGroup = new List<V_HIS_IMP_MEST_MEDICINE>();
                        var medicines = LoadDetailImpMestMatyMetyByImpMestId(row.ID);
                        var mediciGroup = medicines.GroupBy(p => p.MEDICINE_ID).Select(p => p.ToList()).ToList();
                        foreach (var item in mediciGroup)
                        {
                            V_HIS_IMP_MEST_MEDICINE group = new V_HIS_IMP_MEST_MEDICINE();
                            group = item[0];
                            group.AMOUNT = item.Sum(p => p.AMOUNT);
                            lstMedicinGroup.Add(group);
                        }
                        gridControlDetailImpMest.DataSource = lstMedicinGroup;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal || e.CloseMode == PopupCloseMode.Immediate)
                {
                    dtFromTime.Focus();
                    dtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeywordProcess_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMediStock.Focus();
                    cboMediStock.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMobaImpMest_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_IMP_MEST_2)gridViewMobaImpMest.GetRow(e.RowHandle);
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IsCheck")
                        {
                            if (data.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST && (data.AGGR_IMP_MEST_ID == null || data.AGGR_IMP_MEST_ID <= 0))
                            {
                                e.RepositoryItem = repositoryItemCheck_E;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemCheck_D;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMobaImpMest_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {
                                var dataRow = (V_HIS_IMP_MEST_2)gridViewMobaImpMest.GetRow(hi.RowHandle);
                                if (dataRow != null && dataRow.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST && (dataRow.AGGR_IMP_MEST_ID == null || dataRow.AGGR_IMP_MEST_ID <= 0))
                                {
                                    checkEdit.Checked = !checkEdit.Checked;
                                    view.CloseEditor();
                                    if (this._ImpMestADOs != null && this._ImpMestADOs.Count > 0)
                                    {
                                        var dataChecks = this._ImpMestADOs.Where(p => p.IsCheck != true && p.AGGR_IMP_MEST_ID == null && p.IMP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST).ToList();
                                        if (dataChecks != null && dataChecks.Count > 0)
                                        {
                                            gridColumnCheck.Image = imageListIcon.Images[6];
                                            isCheckAll = false;
                                        }
                                        else
                                        {
                                            gridColumnCheck.Image = imageListIcon.Images[5];
                                            isCheckAll = true;
                                        }
                                    }
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewMobaImpMest.BeginUpdate();
                            if (this._ImpMestADOs == null)
                                this._ImpMestADOs = new List<ImpMestADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._ImpMestADOs)
                                {
                                    item.IsCheck = true;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._ImpMestADOs)
                                {
                                    item.IsCheck = false;
                                }
                                isCheckAll = true;
                            }
                            gridViewMobaImpMest.EndUpdate();
                            this.LoadImpMestDetail();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboMediStock_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender is GridLookUpEdit ? (sender as GridLookUpEdit).Properties.Tag as GridCheckMarksSelection : (sender as RepositoryItemGridLookUpEdit).Tag as GridCheckMarksSelection;
                if (gridCheckMark == null) return;


                foreach (MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK rv in gridCheckMark.Selection)
                {
                    if (sb.ToString().Length > 0) { sb.Append(", "); }
                    sb.Append(rv.MEDI_STOCK_NAME.ToString());
                }
                e.DisplayText = sb.ToString();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAggrImpMest_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRowCell)
                {
                    var listData = gridControlAggrImpMest.DataSource as List<V_HIS_IMP_MEST_2>;
                    var data = (V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetRow(hi.RowHandle);
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        DevExpress.XtraBars.BarManager barManager1 = new DevExpress.XtraBars.BarManager();
                        barManager1.Form = this;
                        this.currentImpMest = data;

                        List<V_HIS_IMP_MEST_2> _ImpMestChecks = new List<V_HIS_IMP_MEST_2>();
                        if (gridViewAggrImpMest.RowCount > 0)
                        {
                            for (int i = 0; i < gridViewAggrImpMest.SelectedRowsCount; i++)
                            {
                                if (gridViewAggrImpMest.GetSelectedRows()[i] >= 0)
                                {
                                    _ImpMestChecks.Add((V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetRow(gridViewAggrImpMest.GetSelectedRows()[i]));
                                }
                            }
                        }

                        ImpMestAggregateListPopupMenuProcessor processor = new ImpMestAggregateListPopupMenuProcessor(this.currentImpMest, _ImpMestChecks, ImpMestAggregateMouseRightClick, barManager1);
                        processor.InitMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAggrImpMest_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                    var roomId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).RoomId;
                    long impMestSttId = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewAggrImpMest.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    string creator = (gridViewAggrImpMest.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    var data = (V_HIS_IMP_MEST_2)gridViewAggrImpMest.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "DELETE_DISPLAY")
                        {
                            if (data.REQ_DEPARTMENT_ID == departmentId
                                && (impMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__DRAFT
                                || impMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REJECT
                                || impMestSttId == IMSys.DbConfig.HIS_RS.HIS_IMP_MEST_STT.ID__REQUEST)
                                && (!HisConfigCFG.isEnableDeleteAggregate || ((CheckLoginAdmin.IsAdmin(loginName) || creator.Equals(loginName)) && data.REQ_ROOM_ID == roomId)))
                            {
                                repositoryItemButtonEdit_Delete_ButtonClick(null, null);
                            }
                        }
                        if (e.Column.FieldName == "EDIT_PRINT")
                        {
                            buttonEditPrint_ButtonClick(null, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkThuocVTTheoPhieuNhap_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkThuocVTTheoPhieuNhap.Checked)
                {
                    LoadImpMestDetail();
                }
                else
                {
                    gridControlDetailImpMest.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemCheck_E_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                gridViewMobaImpMest.PostEditor();
                V_HIS_IMP_MEST_2 row = (V_HIS_IMP_MEST_2)gridViewMobaImpMest.GetFocusedRow();
                if (row != null)
                {
                    this.LoadImpMestDetail();
                }
                gridControlMobaImpMest.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadImpMestDetail()
        {
            try
            {
                if (chkThuocVTTheoPhieuNhap.Checked)
                    return;
                WaitingManager.Show();
                List<ImpMestADO> dataChecks = new List<ImpMestADO>();
                if (this._ImpMestADOs != null && this._ImpMestADOs.Count > 0)
                {
                    dataChecks = this._ImpMestADOs.Where(p => p.IsCheck == true).ToList();
                }
                impMestManuMetyMatys = new List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();
                List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE> lstMedicinGroup = new List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();

                if (dataChecks != null)
                {
                    List<long> listExpMestIds = dataChecks.Select(o => o.ID).ToList();
                    CreateThreadLoadDataDetailImpMest(listExpMestIds);
                    if (impMestMedicines != null && impMestMedicines.Count > 0)
                        impMestManuMetyMatys.AddRange(impMestMedicines);
                    if (impMestMaterialDs != null && impMestMaterialDs.Count > 0)
                        impMestManuMetyMatys.AddRange(impMestMaterialDs);
                    if (impMestManuMetyMatys != null && impMestManuMetyMatys.Count > 0)
                    {
                        var mediciGroup = impMestManuMetyMatys.GroupBy(p => p.MEDICINE_ID).Select(p => p.ToList()).ToList();
                        foreach (var item in mediciGroup)
                        {
                            V_HIS_IMP_MEST_MEDICINE group = new V_HIS_IMP_MEST_MEDICINE();
                            group = item[0];
                            group.AMOUNT = item.Sum(p => p.AMOUNT);
                            lstMedicinGroup.Add(group);
                        }
                    }
                }
                gridControlDetailImpMest.DataSource = lstMedicinGroup;
                gridControlDetailImpMest.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateThreadLoadDataDetailImpMest(List<long> _listImpMestIDs)
        {
            Thread threadMetyReq = new System.Threading.Thread(new ParameterizedThreadStart(LoadDataMediDetailImpMest));
            Thread threadMatyReq = new System.Threading.Thread(new ParameterizedThreadStart(LoadDataMateDetailImpMest));

            threadMetyReq.Priority = ThreadPriority.Normal;
            threadMatyReq.Priority = ThreadPriority.Normal;
            try
            {
                threadMetyReq.Start(_listImpMestIDs);
                threadMatyReq.Start(_listImpMestIDs);

                threadMetyReq.Join();
                threadMatyReq.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMetyReq.Abort();
                threadMatyReq.Abort();
            }
        }
        void LoadDataMediDetailImpMest(object _listImpMestIDs)
        {
            try
            {
                impMestMedicines = new List<V_HIS_IMP_MEST_MEDICINE>();
                HisImpMestMedicineViewFilter impMestMedicineViewFilter = new MOS.Filter.HisImpMestMedicineViewFilter();
                impMestMedicineViewFilter.IMP_MEST_IDs = _listImpMestIDs as List<long>;
                impMestMedicines = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MEDICINE>>(HisRequestUriStore.HIS_IMP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, impMestMedicineViewFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void LoadDataMateDetailImpMest(object _listImpMestIDs)
        {
            try
            {
                impMestMaterialDs = new List<V_HIS_IMP_MEST_MEDICINE>();
                HisImpMestMaterialViewFilter impMestMaterialViewFilter = new MOS.Filter.HisImpMestMaterialViewFilter();
                impMestMaterialViewFilter.IMP_MEST_IDs = _listImpMestIDs as List<long>;
                var impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL>>(HisRequestUriStore.HIS_IMP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, impMestMaterialViewFilter, param);
                if (impMestMaterials != null)
                {
                    foreach (var item_impMaterial in impMestMaterials)
                    {
                        MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE impMestMaterial = new MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE();

                        Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>();
                        impMestMaterial = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MATERIAL, MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MEDICINE>(item_impMaterial);

                        impMestMaterial.MEDICINE_TYPE_ID = item_impMaterial.MATERIAL_TYPE_ID;
                        impMestMaterial.MEDICINE_TYPE_CODE = item_impMaterial.MATERIAL_TYPE_CODE;
                        impMestMaterial.MEDICINE_TYPE_NAME = item_impMaterial.MATERIAL_TYPE_NAME;
                        impMestMaterial.MEDICINE_ID = item_impMaterial.MATERIAL_ID;

                        impMestMaterialDs.Add(impMestMaterial);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
