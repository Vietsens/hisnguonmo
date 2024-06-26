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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ChooseICD
{
    public partial class frmMissingIcd : HIS.Desktop.Utility.FormBase
    {
        //bool isLoad = false;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<MissingIcdADO> missingIcdADOList = null;
        List<MissingIcdADO> RemoveIcdADOList = null;
        List<HIS_ICD> icdFromUcs;
        List<MediMatyTypeADO> mediMatyTypeADOs;
        DelegateSelectDatas delegateSelectData;
        List<HIS_ICD_SERVICE> icdServices;
        bool IsVisibleButtonSkip = false;
        Action<bool> getSkip;
        /// <summary>
        /// </summary>
        public frmMissingIcd(List<HIS_ICD> _icdCodeFromUc, List<MediMatyTypeADO> _mediMatyTypeADOs, Inventec.Desktop.Common.Modules.Module _currentModule, List<HIS_ICD_SERVICE> _icdServices, DelegateSelectDatas _delegateSelectData, bool IsVisibleButtonSkip = false,Action<bool> getSkip = default(Action<bool>))
            : base(_currentModule)
        {
            InitializeComponent();
            this.icdFromUcs = _icdCodeFromUc;
            this.mediMatyTypeADOs = _mediMatyTypeADOs;
            this.currentModule = _currentModule;
            this.delegateSelectData = _delegateSelectData;
            this.icdServices = _icdServices;
            this.IsVisibleButtonSkip = IsVisibleButtonSkip;
            this.getSkip = getSkip;
        }

        private void frmDetail_Load(object sender, EventArgs e)
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                SetCaptionByLanguageKey();
                SetDataToGridControl();
                SearchByKeyword();
                SearchByFind();
                if (IsVisibleButtonSkip)
                    lciSkip.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                try
                {
                    ////Khoi tao doi tuong resource
                    Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMissingIcd).Assembly);

                    ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                    this.gridColumnServiceName.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumnServiceName.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumnIcdName.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumnIcdName.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumnIsMainIcd.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumnIsMainIcd.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumnIsMainIcd.ToolTip = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumnIsMainIcd.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumnIsCauseIcd.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumnIsCauseIcd.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumnIsCauseIcd.ToolTip = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumnIsCauseIcd.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.labelControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.btnAddIcd.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.btnAddIcd.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.bar1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.bbtnPrint.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.bbtnPrint.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());

                    this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmMissingIcd.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.btnFind.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.labelControl2.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.bbtnFind.Caption = Inventec.Common.Resource.Get.Value("frmMissingIcd.bbtnFind.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                    this.btnAddIcd.ToolTip = Inventec.Common.Resource.Get.Value("frmMissingIcd.btnAddIcd.ToolTip", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());

                    this.Text = Inventec.Common.Resource.Get.Value("frmMissingIcd.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMissingIcd, LanguageManager.GetCulture());
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMissingIcd_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MissingIcdADO dataRow = (MissingIcdADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    //else if (e.Column.FieldName == "PRIORIRY_DISPLAY")
                    //{
                    //    long priority = (dataRow.PRIORITY ?? 0);
                    //    if ((priority == 1))
                    //    {
                    //        e.Value = imageListPriority.Images[0];
                    //    }
                    //}
                    else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                    {
                        //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(dataRow.INTRUCTION_TIME);
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMissingIcd_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                MissingIcdADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (MissingIcdADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "PRINT_DOM_SOI")
                    {
                        //if (data.SERVICE_REQ_TYPE_ID == (serviceReqType__Test != null ? serviceReqType__Test.ID : 0))
                        //{
                        //    e.RepositoryItem = ButtonEditPrintDomSoi;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMissingIcd_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    var dataSources = (List<MissingIcdADO>)gridControlMissingIcd.DataSource;

                    if (hi.InRowCell && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                    {
                        view.FocusedRowHandle = hi.RowHandle;
                        view.FocusedColumn = hi.Column;

                        int rowHandle = gridViewMissingIcd.GetVisibleRowHandle(hi.RowHandle);
                        var dataRow = (MissingIcdADO)gridViewMissingIcd.GetRow(rowHandle);
                        if (dataRow != null)
                        {
                            //if (hi.Column.FieldName == "Check" && ((dataRow.VIR_TOTAL_PATIENT_PRICE ?? 0) - (dataRow.TOTAL_DEBT_PRICE ?? 0) <= 0))
                            //{
                            //    (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                            //    return;
                            //}
                        }

                        view.ShowEditor();
                        CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                        if (checkEdit == null)
                            return;

                        DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                        Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                        GridViewInfo viewInfo1 = view.GetViewInfo() as GridViewInfo;
                        Rectangle gridGlyphRect =
                            new Rectangle(viewInfo1.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                             viewInfo1.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
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
                            checkEdit.Checked = !checkEdit.Checked;
                            view.CloseEditor();
                        }
                        (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, ItemClickEventArgs e)
        {
            btnSearch_Click(null, null);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchByKeyword();
        }

        private void SetDataToGridControl()
        {
            try
            {
                if (mediMatyTypeADOs == null || mediMatyTypeADOs.Count == 0)
                    return;

                this.missingIcdADOList = new List<MissingIcdADO>();
                this.RemoveIcdADOList = new List<MissingIcdADO>();
                long dem = 0;
                foreach (var item in icdServices)
                {
                    if (item.IS_CONTRAINDICATION != 1)
                    {
                        MissingIcdADO missingIcdADO = new MissingIcdADO();
                        missingIcdADO.ID = dem;
                        missingIcdADO.ICD_NAME = item.ICD_CODE + " - " + item.ICD_NAME;
                        missingIcdADO.ICD_CODE = item.ICD_CODE;
                        var checkService = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (checkService != null)
                        {
                            missingIcdADO.SERVICE_ID = checkService.SERVICE_ID;
                            missingIcdADO.SERVICE_NAME = checkService.MEDICINE_TYPE_NAME;
                        }
                        this.missingIcdADOList.Add(missingIcdADO);
                    }
                    else 
                    {
                        MissingIcdADO RemoveIcdADO = new MissingIcdADO();
                        RemoveIcdADO.ID = dem;
                        RemoveIcdADO.ICD_NAME = item.ICD_CODE + " - " + item.ICD_NAME;
                        RemoveIcdADO.ICD_CODE = item.ICD_CODE;
                        var checkService = this.mediMatyTypeADOs.FirstOrDefault(o => o.SERVICE_ID == item.SERVICE_ID);
                        if (checkService != null)
                        {
                            RemoveIcdADO.SERVICE_ID = checkService.SERVICE_ID;
                            RemoveIcdADO.SERVICE_NAME = checkService.MEDICINE_TYPE_NAME;
                        }
                        this.RemoveIcdADOList.Add(RemoveIcdADO);
                    }
                    dem++;
                }


                gridControlMissingIcd.BeginUpdate();
                gridControlMissingIcd.DataSource = null;
                gridControlMissingIcd.DataSource = this.missingIcdADOList;
                gridControlMissingIcd.EndUpdate();

                if (this.missingIcdADOList == null || this.missingIcdADOList.Count <= 0)
                {
                    this.layoutControlItem7.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    var Height = this.Size.Height;
                    var Width = this.Size.Width;
                    this.Size = new Size(Width, Height - this.layoutControlItem7.Size.Height);
                }

                gridControlRemoveIcd.BeginUpdate();
                gridControlRemoveIcd.DataSource = null;
                gridControlRemoveIcd.DataSource = this.RemoveIcdADOList;
                gridControlRemoveIcd.EndUpdate();

                if (this.RemoveIcdADOList == null || this.RemoveIcdADOList.Count <= 0)
                {
                    this.layoutControlItem8.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    var Height = this.Size.Height;
                    var Width = this.Size.Width;
                    this.Size = new Size(Width, Height - this.layoutControlItem8.Size.Height);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchByKeyword()
        {
            try
            {
                List<MissingIcdADO> dataSource = this.missingIcdADOList;
                if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                {
                    string keyword = txtKeyword.Text.Trim().ToLower();
                    dataSource = this.missingIcdADOList
                         .Where(o => (!String.IsNullOrWhiteSpace(o.ICD_NAME) && o.ICD_NAME.ToLower().Contains(keyword))
                         || (!String.IsNullOrWhiteSpace(o.SERVICE_NAME) && o.SERVICE_NAME.ToLower().Contains(keyword))
                         ).ToList();
                }

                gridControlMissingIcd.BeginUpdate();
                gridControlMissingIcd.DataSource = null;
                gridControlMissingIcd.DataSource = dataSource;
                gridControlMissingIcd.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchByFind()
        {
            try
            {
                List<MissingIcdADO> dataSource = this.RemoveIcdADOList;
                if (!String.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string keyword = txtSearch.Text.Trim().ToLower();
                    dataSource = this.RemoveIcdADOList
                         .Where(o => (!String.IsNullOrWhiteSpace(o.ICD_NAME) && o.ICD_NAME.ToLower().Contains(keyword))
                         || (!String.IsNullOrWhiteSpace(o.SERVICE_NAME) && o.SERVICE_NAME.ToLower().Contains(keyword))
                         ).ToList();
                }

                gridControlRemoveIcd.BeginUpdate();
                gridControlRemoveIcd.DataSource = null;
                gridControlRemoveIcd.DataSource = dataSource;
                gridControlRemoveIcd.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddIcd_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.delegateSelectData != null && this.missingIcdADOList != null)
                {
                    List<MissingIcdADO> selectDatas = this.missingIcdADOList.Where(o => o.ICD_CAUSE_CHECK || o.ICD_MAIN_CHECK).ToList();
                    List<MissingIcdADO> selectDataRemoves = this.RemoveIcdADOList.Where(o => o.ICD_CAUSE_CHECK || o.ICD_MAIN_CHECK).ToList();
                    this.delegateSelectData(selectDatas, selectDataRemoves);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMissingIcd_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var dataSources = (List<MissingIcdADO>)gridControlMissingIcd.DataSource;
                var dataRow = (MissingIcdADO)gridViewMissingIcd.GetFocusedRow();
                bool isCheck = dataRow.ICD_MAIN_CHECK;
                if (dataRow != null && e.Column.FieldName == "ICD_MAIN_CHECK")
                {
                    if (dataRow.ICD_MAIN_CHECK && dataRow.ICD_CAUSE_CHECK)
                    {
                        MessageBox.Show("Chỉ được chọn chẩn đoán chính hoặc chẩn đoán phụ");
                        isCheck = false;
                        dataRow.ICD_MAIN_CHECK = false;
                    }
                    foreach (var item in dataSources)
                    {
                        item.ICD_MAIN_CHECK = false;
                        if (item.ID == dataRow.ID)
                        {
                            item.ICD_MAIN_CHECK = isCheck;
                        }
                    }

                    gridControlMissingIcd.BeginUpdate();
                    gridControlMissingIcd.DataSource = dataSources;
                    gridControlMissingIcd.EndUpdate();
                }
                else if (dataRow != null && e.Column.FieldName == "ICD_CAUSE_CHECK")
                {
                    if (dataRow.ICD_MAIN_CHECK && dataRow.ICD_CAUSE_CHECK)
                    {
                        MessageBox.Show("Chỉ được chọn chẩn đoán chính hoặc chẩn đoán phụ");
                        dataRow.ICD_CAUSE_CHECK = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                SearchByFind();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnFind_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRemoveIcd_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MissingIcdADO dataRow = (MissingIcdADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
                else
                {
                    e.Value = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRemoveIcd_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                var dataSources = (List<MissingIcdADO>)gridControlRemoveIcd.DataSource;
                var dataRow = (MissingIcdADO)gridViewRemoveIcd.GetFocusedRow();
                bool isCheck = dataRow.ICD_MAIN_CHECK;
                if (dataRow != null && e.Column.FieldName == "ICD_MAIN_CHECK")
                {
                    if (dataRow.ICD_MAIN_CHECK && dataRow.ICD_CAUSE_CHECK)
                    {
                        MessageBox.Show("Chỉ được chọn chẩn đoán chính hoặc chẩn đoán phụ");
                        isCheck = false;
                        dataRow.ICD_MAIN_CHECK = false;
                    }
                    foreach (var item in dataSources)
                    {
                        item.ICD_MAIN_CHECK = false;
                        if (item.ID == dataRow.ID)
                        {
                            item.ICD_MAIN_CHECK = isCheck;
                        }
                    }

                    gridControlRemoveIcd.BeginUpdate();
                    gridControlRemoveIcd.DataSource = dataSources;
                    gridControlRemoveIcd.EndUpdate();
                }
                else if (dataRow != null && e.Column.FieldName == "ICD_CAUSE_CHECK")
                {
                    if (dataRow.ICD_MAIN_CHECK && dataRow.ICD_CAUSE_CHECK)
                    {
                        MessageBox.Show("Chỉ được chọn chẩn đoán chính hoặc chẩn đoán phụ");
                        dataRow.ICD_CAUSE_CHECK = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRemoveIcd_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    var dataSources = (List<MissingIcdADO>)gridControlRemoveIcd.DataSource;

                    if (hi.InRowCell && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                    {
                        view.FocusedRowHandle = hi.RowHandle;
                        view.FocusedColumn = hi.Column;

                        int rowHandle = gridViewRemoveIcd.GetVisibleRowHandle(hi.RowHandle);
                        
                        view.ShowEditor();
                        CheckEdit checkEdit = view.ActiveEditor as CheckEdit;
                        if (checkEdit == null)
                            return;

                        DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                        Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                        GridViewInfo viewInfo1 = view.GetViewInfo() as GridViewInfo;
                        Rectangle gridGlyphRect =
                            new Rectangle(viewInfo1.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                             viewInfo1.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
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
                            checkEdit.Checked = !checkEdit.Checked;
                            view.CloseEditor();
                        }
                        (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                    }

                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            try
            {
                if (getSkip != null)
                    getSkip(true);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
