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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.SereServTree;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using Inventec.Common.LocalStorage.SdaConfig;
using HIS.Desktop.Utility;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System.IO;
using HIS.Desktop.Utilities.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using Inventec.Common.Adapter;
using HIS.Desktop.Plugins.ExpXml4210ObOtherBHYT.Config;
using HIS.Desktop.Plugins.ExpXml4210ObOtherBHYT.Base;
using His.ExportXml.Base;
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.ExpXml4210ObOtherBHYT.ADO;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.ExpXml4210ObOtherBHYT
{
    public partial class UCExportXml : HIS.Desktop.Utility.UserControlBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;

        List<V_HIS_TREATMENT_1> listTreatment1 = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_TREATMENT_1> listSelection = new List<V_HIS_TREATMENT_1>();
        List<V_HIS_TREATMENT_1> listSelectionExported = new List<V_HIS_TREATMENT_1>();

        V_HIS_TREATMENT_1 currentTreatment = null;

        HIS_BRANCH _Branch = null;

        CommonParam param = new CommonParam();
        List<V_HIS_SERE_SERV_TEIN> hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_SERE_SERV_PTTT> hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();
        List<HIS_DHST> listDhst = new List<HIS_DHST>();
        List<HIS_TRACKING> hisTrackings = new List<HIS_TRACKING>();
        List<V_HIS_TREATMENT_3> hisTreatments = new List<V_HIS_TREATMENT_3>();
        List<V_HIS_SERE_SERV_2> ListSereServ = new List<V_HIS_SERE_SERV_2>();
        List<HIS_EKIP_USER> ListEkipUser = new List<HIS_EKIP_USER>();
        List<V_HIS_BED_LOG> ListBedlog = new List<V_HIS_BED_LOG>();
        List<V_HIS_PATIENT_TYPE_ALTER> listPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();
        List<HIS_PATIENT_TYPE> patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
        List<HIS_PATIENT_TYPE> listPatientType;
        List<TreatmentImportADO> listTreatmentImport;

        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;
        string FilePath = "";
        private string typeCodeFind;

        private string[] icdSeparators = new string[] { ";" };

        public UCExportXml(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.currentModule = moduleData;
                this.InitSereServTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem item1 = new DXMenuItem("Thời gian khóa viện phí từ:", new EventHandler(btnDropTime_Click));
                item1.Tag = "islock";
                menu.Items.Add(item1);

                DXMenuItem item2 = new DXMenuItem("Thời gian kết thúc điều trị từ:", new EventHandler(btnDropTime_Click));
                item2.Tag = "finish";
                menu.Items.Add(item2);

                DXMenuItem item3 = new DXMenuItem("Thời gian vào viện từ:", new EventHandler(btnDropTime_Click));
                item3.Tag = "intime";
                menu.Items.Add(item3);

                btnDropTime.DropDownControl = menu;
                btnDropTime.MenuManager = barManager1;
                btnDropTime.Text = item1.Caption;
                this.typeCodeFind = item1.Tag.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitSereServTree()
        {
            try
            {
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsCreateParentNodeWithSereServExpend = false;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn("Tên dịch vụ", "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn("SL", "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn("Đơn giá", "VIR_PRICE", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn("Thành tiền", "VIR_TOTAL_PRICE", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn("Đồng chi trả", "VIR_TOTAL_HEIN_PRICE", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn("Bệnh nhân trả", "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn("Chiết khấu", "DISCOUNT", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn("Hao phí", "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn("Vat %", "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn("Mã dịch vụ", "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn("Mã yêu cầu", "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlSereServTree.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCExportXml_Load(object sender, EventArgs e)
        {
            try
            {
                this.InitTypeFind();
                this.InitComboPatientType();
                this.SetDefaultValueControl();
                this.FillDataToGridTreatment();
                this.LoadDataEmployeestoXML();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboPatientType()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboPatientType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__cboPatientType);
                cboPatientType.Properties.Tag = gridCheck;
                cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                listPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.IS_RATION != 1).ToList();

                cboPatientType.Properties.DataSource = listPatientType;
                cboPatientType.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientType.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                col2.VisibleIndex = 1;
                col2.Width = 100;
                col2.Caption = "";
                DevExpress.XtraGrid.Columns.GridColumn col3 = cboPatientType.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                col3.VisibleIndex = 2;
                col3.Width = 200;
                col3.Caption = "";

                cboPatientType.Properties.PopupFormWidth = 200;
                cboPatientType.Properties.View.OptionsView.ShowColumnHeaders = false;
                cboPatientType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboPatientType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboPatientType.Properties.View);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__cboPatientType(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                GridCheckMarksSelection gridCheckMark = sender as GridCheckMarksSelection;
                patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                if (gridCheckMark != null)
                {
                    List<HIS_PATIENT_TYPE> erSelectedNews = new List<HIS_PATIENT_TYPE>();
                    foreach (HIS_PATIENT_TYPE er in (sender as GridCheckMarksSelection).Selection)
                    {
                        if (er != null)
                        {
                            if (sb.ToString().Length > 0) { sb.Append(", "); }
                            sb.Append(er.PATIENT_TYPE_NAME);
                            erSelectedNews.Add(er);
                        }

                    }
                    patientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                    patientTypeSelecteds.AddRange(erSelectedNews);
                }
                cboPatientType.Text = sb.ToString();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetComboPatientType(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                    cbo.EditValue = null;
                    cbo.Focus();
                    this.patientTypeSelecteds.AddRange(listPatientType.Where(o => o.ID != Config.HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList());
                    gridCheckMark.SelectAll(this.patientTypeSelecteds);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void SetDefaultValueControl()
        {
            try
            {
                dtTimeFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.MinValue;
                dtTimeTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.MinValue;
                txtTreatCodeOrHeinCard.Text = "";
                this._Branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                this.btnXmlViewer.Enabled = false;
                this.btnDropTime.Text = "Thời gian khóa viện phí từ";
                ResetComboPatientType(cboPatientType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                FillDataToGridTreatment(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, (int)ConfigApplications.NumPageSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(object param)
        {
            try
            {
                listTreatment1 = new List<V_HIS_TREATMENT_1>();
                listSelection = new List<V_HIS_TREATMENT_1>();
                gridControlTreatment.DataSource = null;
                listTreatmentImport = null;
                btnExportXml.Enabled = false;
                btnXmlViewer.Enabled = false;
                btnExportXmlGroup.Enabled = false;
                currentTreatment = null;
                FillDataToSereServTreeByTreatment();

                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);

                HisTreatmentView1Filter filter = new HisTreatmentView1Filter();
                filter.ORDER_DIRECTION = "ACS";
                filter.ORDER_FIELD = "FEE_LOCK_TIME";
                filter.IS_ACTIVE = 0;
                if (patientTypeSelecteds != null && patientTypeSelecteds.Count() > 0)
                    filter.TDL_PATIENT_TYPE_IDs = patientTypeSelecteds.Select(o => o.ID).ToList();

                if (!String.IsNullOrEmpty(txtKeyword.Text.Trim()))
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                }
                else
                {
                    if (!String.IsNullOrEmpty(txtTreatCodeOrHeinCard.Text.Trim()))
                    {
                        string code = txtTreatCodeOrHeinCard.Text.Trim();
                        if (Char.IsDigit(code.FirstOrDefault()))
                        {
                            if (code.Length < 12)
                            {
                                try
                                {
                                    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                                    txtTreatCodeOrHeinCard.Text = code;
                                    filter.TREATMENT_CODE__EXACT = code;
                                }
                                catch (Exception ex)
                                {
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                }
                            }
                            else
                            {
                                filter.TREATMENT_CODE__EXACT = code;
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(filter.TREATMENT_CODE__EXACT))
                    {
                        switch (typeCodeFind)
                        {
                            case "islock":
                                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                                {
                                    filter.FEE_LOCK_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                                }
                                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                                {
                                    filter.FEE_LOCK_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                                }
                                break;
                            case "finish":
                                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                                {
                                    filter.OUT_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                                }
                                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                                {
                                    filter.OUT_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                                }
                                break;
                            case "intime":
                                if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                                {
                                    filter.REQUEST_HOSPITALIZE_TIME_FROM = Convert.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                                }
                                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                                {
                                    filter.REQUEST_HOSPITALIZE_TIME_TO = Convert.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                                }
                                break;
                            default:
                                break;
                        }

                    }
                }

                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumers.MosConsumer, filter, paramCommon);
                if (result != null)
                {
                    listTreatment1 = (List<V_HIS_TREATMENT_1>)result.Data;
                    rowCount = (listTreatment1 == null ? 0 : listTreatment1.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                gridControlTreatment.BeginUpdate();
                gridControlTreatment.DataSource = listTreatment1;
                gridControlTreatment.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToSereServTreeByTreatment()
        {
            try
            {

                var listSereServ = new List<V_HIS_SERE_SERV_5>();
                if (this.currentTreatment != null)
                {
                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.TDL_TREATMENT_ID = this.currentTreatment.ID;
                    listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                    if (listSereServ != null)
                    {
                        listSereServ = listSereServ.Where(o => o.PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT).ToList();
                    }
                }
                this.ssTreeProcessor.Reload(ucSereServTree, listSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataEmployeestoXML()
        {
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new HisEmployeeFilter();
                His.ExportXml.Base.GlobalConfigStore.ListEmployees = new Inventec.Common.Adapter.BackendAdapter(paramGet).Get<List<HIS_EMPLOYEE>>("/api/HisEmployee/Get", ApiConsumers.MosConsumer, filter, paramGet);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtTimeTo.Focus();
                    dtTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    btnFind.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatCodeOrHeinCard_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtTreatCodeOrHeinCard.Text))
                    {
                        dtTimeFrom.Focus();
                        dtTimeFrom.SelectAll();
                    }
                    else
                    {
                        this.btnFind_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex < 0 || !e.IsGetData || e.Column.UnboundType == DevExpress.Data.UnboundColumnType.Bound)
                    return;
                var data = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(e.ListSourceRowIndex);
                if (data != null)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + (ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize;
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    }
                    else if (e.Column.FieldName == "IN_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                    }
                    else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR" && data.CLINICAL_IN_TIME.HasValue)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME.Value);
                    }
                    else if (e.Column.FieldName == "OUT_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "FEE_LOCK_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FEE_LOCK_TIME ?? 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0)
                    return;
                WaitingManager.Show();
                this.currentTreatment = (V_HIS_TREATMENT_1)gridViewTreatment.GetFocusedRow();
                FillDataToSereServTreeByTreatment();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                listSelection = new List<V_HIS_TREATMENT_1>();
                var listIndex = gridViewTreatment.GetSelectedRows();
                foreach (var index in listIndex)
                {
                    var treatment = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(index);
                    if (treatment != null)
                    {
                        listSelection.Add(treatment);
                    }
                }

                if (listSelection.Count > 0)
                {
                    btnExportXml.Enabled = btnExportAll.Enabled = btnExportXmlGroup.Enabled = true;
                }
                else
                {
                    btnExportXml.Enabled = btnExportAll.Enabled = btnExportXmlGroup.Enabled = false;
                }

                gridViewTreatment.BeginDataUpdate();
                gridViewTreatment.EndDataUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (data != null && !e.Node.HasChildren)
                {
                    if (data.VIR_TOTAL_PATIENT_PRICE.HasValue && data.VIR_TOTAL_PATIENT_PRICE.Value > 0)
                    {
                    }
                    else
                    {
                        e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled)
                    return;
                WaitingManager.Show();
                FillDataToGridTreatment();
                if (listTreatment1 != null && listTreatment1.Count == 1)
                {
                    this.currentTreatment = listTreatment1.First();
                    FillDataToSereServTreeByTreatment();
                }
                gridControlTreatment.Focus();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXml_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                bool success = false;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.FilePath = fbd.SelectedPath;
                }
                WaitingManager.Show();
                success = this.GenerateXml(ref param, listSelection, this.FilePath);
                WaitingManager.Hide();
                if (success && param.Messages.Count == 0)
                {
                    listSelectionExported = new List<V_HIS_TREATMENT_1>();
                    listSelectionExported.AddRange(listSelection);
                    MessageManager.Show(this.ParentForm, param, success);
                    this.gridViewTreatment.BeginDataUpdate();
                    this.gridViewTreatment.EndDataUpdate();
                }
                else if (success && param.Messages.Count > 0)
                {
                    success = false;
                    listSelectionExported = new List<V_HIS_TREATMENT_1>();
                    listSelectionExported.AddRange(listSelection);
                    MessageManager.Show(this.ParentForm, param, success);
                    this.gridViewTreatment.BeginDataUpdate();
                    this.gridViewTreatment.EndDataUpdate();
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool GenerateXml(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection, string pathSave, bool IsGroup = false)
        {
            bool result = false;
            try
            {
                string message = "";
                if (listSelection.Count > 0)
                {
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        pathSave = ConfigStore.GetFolderSaveXml + "\\ExportXmlPlus\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                        var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                        if (dicInfo == null)
                        {
                            paramExport.Messages.Add("Không tạo được Folder lưu Xml");
                            return result;
                        }
                    }
                    if (!GlobalConfigStore.IsInit)
                        if (!this.SetDataToLocalXml(pathSave))
                        {
                            paramExport.Messages.Add("Không thiết lập được cấu hình dữ liệu xuất Xml");
                            return result;
                        }
                    GlobalConfigStore.PathSaveXml = pathSave;

                    param = new CommonParam();

                    int skip = 0;
                    while (listSelection.Count - skip > 0)
                    {
                        var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                        skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                        CreateThreadGetData(limit);
                    }
                    if (param.HasException) throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh lay du lieu xuat xml");

                    if (IsGroup)
                        message = ProcessExportXmlDetailGroup(ref result, hisTreatments, listPatientTypeAlter, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog, listSelection);
                    else
                        message = ProcessExportXmlDetail(ref result, hisTreatments, listPatientTypeAlter, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog, listSelection);

                    if (!String.IsNullOrEmpty(message))
                    {
                        //paramExport.Messages.Add(String.Format(Base.ResourceMessageLang.CacMaDieuTriKhongXuatDuocXml, message));
                        paramExport.Messages.Add(message);
                    }
                    if (!String.IsNullOrWhiteSpace(FilePath))
                    {
                        btnXmlViewer.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        string ProcessExportXmlDetail(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_PATIENT_TYPE_ALTER> hisPatientTypeAlters, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein, List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser, List<V_HIS_BED_LOG> ListBedlog, List<V_HIS_TREATMENT_1> _listUpdate)
        {
            string result = "";
            this.FilePath = "";
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> dicPatientTypeAlter = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                Dictionary<long, HIS_DHST> dicDhst = new Dictionary<long, HIS_DHST>();
                Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();

                if (hisPatientTypeAlters != null && hisPatientTypeAlters.Count > 0)
                {
                    foreach (var item in hisPatientTypeAlters)
                    {
                        if (!dicPatientTypeAlter.ContainsKey(item.TREATMENT_ID))
                            dicPatientTypeAlter[item.TREATMENT_ID] = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        dicPatientTypeAlter[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    foreach (var sereServ in ListSereServ)
                    {
                        if (sereServ.PATIENT_TYPE_ID != HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT
                            && sereServ.AMOUNT > 0
                            && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                            dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                        }

                        if (sereServ.EKIP_ID.HasValue && ListEkipUser != null && ListEkipUser.Count > 0 && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            var ekips = ListEkipUser.Where(o => o.EKIP_ID == sereServ.EKIP_ID).ToList();
                            if (ekips != null && ekips.Count > 0)
                            {
                                foreach (var item in ekips)
                                {
                                    if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                        dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();

                                    dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].Add(item);
                                }
                            }
                        }
                    }
                }

                if (listSereServTein != null && listSereServTein.Count > 0)
                {
                    foreach (var ssTein in listSereServTein)
                    {
                        if (!ssTein.TDL_TREATMENT_ID.HasValue) continue;

                        if (!dicSereServTein.ContainsKey(ssTein.TDL_TREATMENT_ID.Value))
                            dicSereServTein[ssTein.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_TEIN>();

                        dicSereServTein[ssTein.TDL_TREATMENT_ID.Value].Add(ssTein);
                    }
                }

                if (hisTrackings != null && hisTrackings.Count > 0)
                {
                    foreach (var tracking in hisTrackings)
                    {
                        if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                            dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();

                        dicTracking[tracking.TREATMENT_ID].Add(tracking);
                    }
                }

                if (hisSereServPttts != null && hisSereServPttts.Count > 0)
                {
                    foreach (var ssPttt in hisSereServPttts)
                    {
                        if (!ssPttt.TDL_TREATMENT_ID.HasValue) continue;

                        if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value))
                            dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();

                        dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                    }
                }

                if (listDhst != null && listDhst.Count > 0)
                {
                    //sap xep thoi gian tang dan de trong th co nhieu dhst se lay cai co thoi gian thuc hien lon nhat
                    //lay dhst cuoi cung co can nang
                    listDhst = listDhst.OrderBy(o => o.EXECUTE_TIME).ToList();
                    foreach (var item in listDhst)
                    {
                        if (dicDhst.ContainsKey(item.TREATMENT_ID))
                        {
                            if (item.WEIGHT.HasValue) dicDhst[item.TREATMENT_ID] = item;
                            else if (!dicDhst[item.TREATMENT_ID].WEIGHT.HasValue)
                                dicDhst[item.TREATMENT_ID] = item;
                        }
                        else
                            dicDhst[item.TREATMENT_ID] = item;
                    }
                }

                if (ListBedlog != null && ListBedlog.Count > 0)
                {
                    foreach (var bed in ListBedlog)
                    {
                        if (!dicBedLog.ContainsKey(bed.TREATMENT_ID))
                            dicBedLog[bed.TREATMENT_ID] = new List<V_HIS_BED_LOG>();

                        dicBedLog[bed.TREATMENT_ID].Add(bed);
                    }
                }

                foreach (var treatment in hisTreatments)
                {
                    InputADO ado = new InputADO();
                    ado.Treatment = treatment;
                    V_HIS_PATIENT_TYPE_ALTER _alter = new V_HIS_PATIENT_TYPE_ALTER();
                    if (dicPatientTypeAlter.ContainsKey(treatment.ID))
                    {
                        _alter = dicPatientTypeAlter[treatment.ID].FirstOrDefault(p => p.PATIENT_TYPE_ID == treatment.TDL_PATIENT_TYPE_ID);
                    }
                    ado.LastPatientTypeAlter = _alter;
                    ado.ListSereServ = dicSereServ.ContainsKey(treatment.ID) ? dicSereServ[treatment.ID] : null;
                    ado.Branch = BranchDataWorker.Branch;
                    if (dicDhst.ContainsKey(treatment.ID))
                    {
                        ado.Dhst = dicDhst[treatment.ID];
                    }

                    if (dicSereServTein.ContainsKey(treatment.ID))
                    {
                        ado.SereServTeins = dicSereServTein[treatment.ID];
                    }

                    if (dicTracking.ContainsKey(treatment.ID))
                    {
                        ado.Trackings = dicTracking[treatment.ID];
                    }

                    if (dicSereServPttt.ContainsKey(treatment.ID))
                    {
                        ado.SereServPttts = dicSereServPttt[treatment.ID];
                    }

                    if (dicBedLog.ContainsKey(treatment.ID))
                    {
                        ado.BedLogs = dicBedLog[treatment.ID];
                    }

                    if (dicEkipUser.ContainsKey(treatment.ID))
                    {
                        ado.EkipUsers = dicEkipUser[treatment.ID].Distinct().ToList();
                    }

                    ado.MaterialPackageOption = HisConfigCFG.GetValue(HisConfigCFG.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION);
                    ado.MaterialPriceOriginalOption = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_PRICE_OPTION);

                    His.ExportXml.CreateXmlMain xmlMain = new His.ExportXml.CreateXmlMain(ado);

                    string errorMess = "";
                    string fullFileName = xmlMain.Run4210Path(ref errorMess);
                    if (String.IsNullOrWhiteSpace(fullFileName))
                    {
                        _listUpdate.Remove(_listUpdate.FirstOrDefault(p => p.ID == treatment.ID));

                        Inventec.Common.Logging.LogSystem.Info("Khong tao duoc XML4210 cho Ho so duyet TreatmentCode: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatment.TREATMENT_CODE), treatment.TREATMENT_CODE));
                        if (!DicErrorMess.ContainsKey(errorMess))
                        {
                            DicErrorMess[errorMess] = new List<string>();
                        }

                        DicErrorMess[errorMess].Add(treatment.TREATMENT_CODE);
                    }
                    else
                    {
                        this.FilePath += fullFileName + ";";
                        isSuccess = true;
                    }
                }

                if (DicErrorMess.Count > 0)
                {
                    foreach (var item in DicErrorMess)
                    {
                        result += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
        string ProcessExportXmlDetailGroup(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_PATIENT_TYPE_ALTER> hisPatientTypeAlters, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein, List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser, List<V_HIS_BED_LOG> ListBedlog, List<V_HIS_TREATMENT_1> _listUpdate)
        {
            string result = "";
            this.FilePath = "";
            try
            {
                InputGroupADO ado = new InputGroupADO();
                ado.Treatments = hisTreatments;
                V_HIS_PATIENT_TYPE_ALTER _alter = new V_HIS_PATIENT_TYPE_ALTER();
                ado.LastPatientTypeAlters = hisPatientTypeAlters;
                ado.ListSereServ = ListSereServ;
                ado.Branch = BranchDataWorker.Branch;
                ado.Dhsts = listDhst;
                ado.SereServTeins = listSereServTein;
                ado.Trackings = hisTrackings;
                ado.SereServPttts = hisSereServPttts;
                ado.BedLogs = ListBedlog;
                ado.EkipUsers = ListEkipUser;

                ado.MaterialPackageOption = HisConfigCFG.GetValue(HisConfigCFG.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION);
                ado.MaterialPriceOriginalOption = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_PRICE_OPTION);

                His.ExportXml.CreateXmlMain xmlMain = new His.ExportXml.CreateXmlMain(ado);

                string fullFileName = xmlMain.Run4210GroupPath(ref result);
                if (!String.IsNullOrWhiteSpace(fullFileName))
                {
                    this.FilePath = fullFileName;
                    isSuccess = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }
        //Lay branch theo cashier_room_id chu ko lay tu token (tranh truong hop chay thread bi mat thong tin token)
        private HIS_BRANCH GetByCashierRoomId(long cashierRoomId)
        {
            try
            {
                V_HIS_CASHIER_ROOM cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>() != null ? BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == cashierRoomId) : null;
                if (cashierRoom != null)
                {
                    return (BackendDataWorker.Get<HIS_BRANCH>() != null ? BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == cashierRoom.BRANCH_ID) : null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        bool SetDataToLocalXml(string path)
        {
            bool result = false;
            try
            {
                if (this._Branch == null)
                {
                    return result;
                }

                GlobalConfigStore.Branch = this._Branch;

                GlobalConfigStore.ListIcdCode_Nds = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__OTHER);
                GlobalConfigStore.ListIcdCode_Nds_Te = HisConfigCFG.GetListValue(HisConfigCFG.MRS_HIS_REPORT_BHYT_NDS_ICD_CODE__TE);

                GlobalConfigStore.IsInit = true;
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        public void BtnFind()
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

        public void BtnExportXml()
        {
            try
            {
                btnExportXml_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusTreatmentCode()
        {
            try
            {
                txtTreatCodeOrHeinCard.Focus();
                txtTreatCodeOrHeinCard.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Thread
        private void CreateThreadGetData(List<V_HIS_TREATMENT_1> listSelection)
        {
            System.Threading.Thread HeinApproval = new System.Threading.Thread(ThreadGetPatientType);
            System.Threading.Thread SereServ2 = new System.Threading.Thread(ThreadGetSereServ2);
            System.Threading.Thread Treatment3 = new System.Threading.Thread(ThreadGetTreatment3);
            System.Threading.Thread Dhst_Tracking = new System.Threading.Thread(ThreadGetDhst_Tracking);
            System.Threading.Thread SereServTein_PTTT = new System.Threading.Thread(ThreadGetSereServTein_PTTT);
            try
            {
                HeinApproval.Start(listSelection);
                SereServ2.Start(listSelection);
                Treatment3.Start(listSelection);
                Dhst_Tracking.Start(listSelection);
                SereServTein_PTTT.Start(listSelection);

                HeinApproval.Join();
                SereServ2.Join();
                Treatment3.Join();
                Dhst_Tracking.Join();
                SereServTein_PTTT.Join();
            }
            catch (Exception ex)
            {
                HeinApproval.Abort();
                SereServ2.Abort();
                Treatment3.Abort();
                Dhst_Tracking.Abort();
                SereServTein_PTTT.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetSereServTein_PTTT(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                hisSereServTeins = new List<V_HIS_SERE_SERV_TEIN>();
                hisSereServPttts = new List<V_HIS_SERE_SERV_PTTT>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisSereServTeinViewFilter ssTeinFilter = new HisSereServTeinViewFilter();
                    ssTeinFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    var resulTein = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_TEIN>>("api/HisSereServTein/GetView", ApiConsumers.MosConsumer, ssTeinFilter, param);
                    if (resulTein != null && resulTein.Count > 0)
                    {
                        hisSereServTeins.AddRange(resulTein);
                    }

                    HisSereServPtttViewFilter ssPtttFilter = new HisSereServPtttViewFilter();
                    ssPtttFilter.TDL_TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    ssPtttFilter.ORDER_DIRECTION = "DESC";
                    ssPtttFilter.ORDER_FIELD = "ID";
                    var resultPttt = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_PTTT>>("api/HisSereServPttt/GetView", ApiConsumers.MosConsumer, ssPtttFilter, param);
                    if (resultPttt != null && resultPttt.Count > 0)
                    {
                        hisSereServPttts.AddRange(resultPttt);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetDhst_Tracking(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                listDhst = new List<HIS_DHST>();
                hisTrackings = new List<HIS_TRACKING>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisDhstFilter dhstFilter = new HisDhstFilter();
                    dhstFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultDhst = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_DHST>>(HisRequestUriStore.HIS_DHST_GET, ApiConsumers.MosConsumer, dhstFilter, param);
                    if (resultDhst != null && resultDhst.Count > 0)
                    {
                        listDhst.AddRange(resultDhst);
                    }

                    HisTrackingFilter trackingFilter = new HisTrackingFilter();
                    trackingFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultTracking = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TRACKING>>("api/HisTracking/Get", ApiConsumers.MosConsumer, trackingFilter, param);
                    if (resultTracking != null && resultTracking.Count > 0)
                    {
                        hisTrackings.AddRange(resultTracking);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetTreatment3(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                hisTreatments = new List<V_HIS_TREATMENT_3>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisTreatmentView3Filter treatmentFilter = new HisTreatmentView3Filter();
                    treatmentFilter.IDs = limit.Select(o => o.ID).ToList();
                    var resultTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_3>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_3, ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (resultTreatment != null && resultTreatment.Count > 0)
                    {
                        hisTreatments.AddRange(resultTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetSereServ2(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                ListSereServ = new List<V_HIS_SERE_SERV_2>();
                ListEkipUser = new List<HIS_EKIP_USER>();
                ListBedlog = new List<V_HIS_BED_LOG>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisSereServView2Filter ssFilter = new HisSereServView2Filter();
                    ssFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultSS = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_SERE_SERV_2>>(HisRequestUriStore.HIS_SERE_SERV_GETVIEW_2, ApiConsumers.MosConsumer, ssFilter, param);
                    if (resultSS != null && resultSS.Count > 0)
                    {
                        ListSereServ.AddRange(resultSS);

                        var ekipIds = resultSS.Select(o => o.EKIP_ID ?? 0).Distinct().ToList();
                        if (ekipIds != null && ekipIds.Count > 1)//null sẽ có 1 id bằng 0
                        {
                            HisEkipUserFilter ekipFilter = new HisEkipUserFilter();
                            ekipFilter.EKIP_IDs = ekipIds;
                            var resultEkip = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EKIP_USER>>("api/HisEkipUser/Get", ApiConsumers.MosConsumer, ekipFilter, param);
                            if (resultEkip != null && resultEkip.Count > 0)
                            {
                                ListEkipUser.AddRange(resultEkip);
                            }
                        }
                    }

                    HisBedLogViewFilter bedFilter = new HisBedLogViewFilter();
                    bedFilter.TREATMENT_IDs = limit.Select(o => o.ID).ToList();
                    var resultBed = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedFilter, param);
                    if (resultBed != null && resultBed.Count > 0)
                    {
                        ListBedlog.AddRange(resultBed);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ThreadGetPatientType(object obj)
        {
            try
            {
                if (obj == null) return;
                List<V_HIS_TREATMENT_1> listSelection = (List<V_HIS_TREATMENT_1>)obj;
                listPatientTypeAlter = new List<V_HIS_PATIENT_TYPE_ALTER>();

                var skip = 0;
                while (listSelection.Count - skip > 0)
                {
                    var limit = listSelection.Skip(skip).Take(GlobalVariables.MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip = skip + GlobalVariables.MAX_REQUEST_LENGTH_PARAM;

                    HisPatientTypeAlterViewFilter filter = new HisPatientTypeAlterViewFilter();
                    filter.TREATMENT_IDs = limit.Select(s => s.ID).ToList();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "LOG_TIME";
                    filter.PATIENT_TYPE_IDs = new List<long>();
                    if (patientTypeSelecteds != null && patientTypeSelecteds.Count() > 0)
                        filter.PATIENT_TYPE_IDs = patientTypeSelecteds.Select(o => o.ID).ToList();

                    var resultPatientTypeAlters = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, filter, param);
                    if (resultPatientTypeAlters != null && resultPatientTypeAlters.Count > 0)
                    {
                        listPatientTypeAlter.AddRange(resultPatientTypeAlters);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnXmlViewer_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(FilePath))
                {
                    string[] filePaths = FilePath.Split(';');
                    if (filePaths == null || filePaths.Count() == 0)
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                        {
                            moduleData.RoomId = this.currentModule.RoomId;
                            moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                            List<object> listArgs = new List<object>();
                            listArgs.Add(moduleData);
                            var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                            if (extenceInstance == null)
                            {
                                throw new ArgumentNullException("moduleData is null");
                            }

                            ((Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                        }
                    }
                    else
                    {
                        foreach (var path in filePaths)
                        {
                            if (!String.IsNullOrWhiteSpace(path))
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    moduleData.RoomId = this.currentModule.RoomId;
                                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                    List<object> listArgs = new List<object>();
                                    listArgs.Add(path);
                                    listArgs.Add(moduleData);
                                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                    if (extenceInstance == null)
                                    {
                                        throw new ArgumentNullException("moduleData is null");
                                    }

                                    ((Form)extenceInstance).Show();
                                }
                                else
                                {
                                    MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                                }
                            }
                        }
                    }
                    string firstFilePath = filePaths.FirstOrDefault(o => !String.IsNullOrWhiteSpace(o));
                }
                else
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        moduleData.RoomId = this.currentModule.RoomId;
                        moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                        List<object> listArgs = new List<object>();
                        listArgs.Add(moduleData);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null)
                        {
                            throw new ArgumentNullException("moduleData is null");
                        }

                        ((Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TREATMENT_1)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {

                        if (e.Column.FieldName == "ViewXML")
                        {
                            try
                            {
                                if (this.listSelectionExported != null && this.listSelectionExported.Count > 0 && this.listSelectionExported.Select(p => p.TREATMENT_CODE).Contains(data.TREATMENT_CODE) && !String.IsNullOrWhiteSpace(this.FilePath))
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_XMLViewerEnable;
                                }
                                else
                                {
                                    e.RepositoryItem = repositoryItemButtonEdit_XMLViewerDisable;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void repositoryItemButtonEdit_XMLViewerEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var treatment1 = (V_HIS_TREATMENT_1)gridViewTreatment.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "ViewXML")
                        {
                            if (this.listSelectionExported != null && this.listSelectionExported.Count() > 0 && this.listSelectionExported.Select(o => o.TREATMENT_CODE).Contains(treatment1.TREATMENT_CODE))
                            {
                                if (!String.IsNullOrWhiteSpace(FilePath))
                                {
                                    string[] filePaths = FilePath.Split(';');
                                    if (filePaths == null || filePaths.Count() == 0)
                                    {
                                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                        {
                                            moduleData.RoomId = this.currentModule.RoomId;
                                            moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                            List<object> listArgs = new List<object>();
                                            listArgs.Add(moduleData);
                                            var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                            if (extenceInstance == null)
                                            {
                                                throw new ArgumentNullException("moduleData is null");
                                            }

                                            ((Form)extenceInstance).ShowDialog();
                                        }
                                        else
                                        {
                                            MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                                        }
                                    }
                                    else
                                    {
                                        var path = filePaths.FirstOrDefault(o => o.Contains(treatment1.TREATMENT_CODE));
                                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.XMLViewer").FirstOrDefault();
                                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.XMLViewer'");
                                        if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                        {
                                            moduleData.RoomId = this.currentModule.RoomId;
                                            moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                            List<object> listArgs = new List<object>();
                                            listArgs.Add(path);
                                            listArgs.Add(moduleData);
                                            var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                            if (extenceInstance == null)
                                            {
                                                throw new ArgumentNullException("moduleData is null");
                                            }

                                            ((Form)extenceInstance).ShowDialog();
                                        }
                                        else
                                        {
                                            MessageManager.Show("Chức năng chưa hỗ trợ phiên bản hiện tại");
                                        }
                                    }
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Info("FilePath is null or empty");
                                }
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

        private void cboPatientType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string paytientType = "";
                if (this.patientTypeSelecteds != null && this.patientTypeSelecteds.Count > 0 && this.patientTypeSelecteds.Count < this.listPatientType.Count)
                {
                    foreach (var item in this.patientTypeSelecteds)
                    {

                        paytientType += item.PATIENT_TYPE_NAME;

                        if (!(item == patientTypeSelecteds.Last()))
                        {
                            paytientType += ", ";
                        }

                    }
                }
                else if (this.patientTypeSelecteds.Count == this.listPatientType.Count)
                {
                    paytientType = "Tất cả";
                }
                e.DisplayText = paytientType;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);

            }
        }

        private void btnExportAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXml.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                bool success = false;
                FolderBrowserDialog fbd = new FolderBrowserDialog();

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.FilePath = fbd.SelectedPath;
                }
                WaitingManager.Show();

                success = this.GenerateXmlAll(ref param, listSelection, this.FilePath);
                WaitingManager.Hide();
                if (success && param.Messages.Count == 0)
                {
                    listSelectionExported = new List<V_HIS_TREATMENT_1>();
                    listSelectionExported.AddRange(listSelection);
                    MessageManager.Show(this.ParentForm, param, success);
                    this.gridViewTreatment.BeginDataUpdate();
                    this.gridViewTreatment.EndDataUpdate();
                }
                else if (success && param.Messages.Count > 0)
                {
                    success = false;
                    listSelectionExported = new List<V_HIS_TREATMENT_1>();
                    listSelectionExported.AddRange(listSelection);
                    MessageManager.Show(this.ParentForm, param, success);
                    this.gridViewTreatment.BeginDataUpdate();
                    this.gridViewTreatment.EndDataUpdate();
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool GenerateXmlAll(ref CommonParam paramExport, List<V_HIS_TREATMENT_1> listSelection, string pathSave)
        {
            bool result = false;
            try
            {
                string message = "";
                if (listSelection.Count > 0)
                {
                    if (String.IsNullOrEmpty(pathSave))
                    {
                        pathSave = ConfigStore.GetFolderSaveXml + "\\ExportXmlPlus\\Xml" + DateTime.Now.ToString("ddMMyyyy");
                        var dicInfo = System.IO.Directory.CreateDirectory(pathSave);
                        if (dicInfo == null)
                        {
                            paramExport.Messages.Add("Không tạo được Folder lưu Xml");
                            return result;
                        }
                    }
                    if (!GlobalConfigStore.IsInit)
                        if (!this.SetDataToLocalXml(pathSave))
                        {
                            paramExport.Messages.Add("Không thiết lập được cấu hình dữ liệu xuất Xml");
                            return result;
                        }
                    GlobalConfigStore.PathSaveXml = pathSave;

                    param = new CommonParam();

                    CreateThreadGetData(listSelection);

                    if (param.HasException) throw new DataMisalignedException("Co exception xay ra tai DAOGET trong qua trinh lay du lieu xuat xml");

                    message = ProcessExportXmlAll(ref result, hisTreatments, listPatientTypeAlter, ListSereServ, listDhst, hisSereServTeins, hisTrackings, hisSereServPttts, ListEkipUser, ListBedlog, listSelection);

                    if (!String.IsNullOrEmpty(message))
                    {
                        //paramExport.Messages.Add(String.Format(Base.ResourceMessageLang.CacMaDieuTriKhongXuatDuocXml, message));
                        paramExport.Messages.Add(message);
                    }
                    if (!String.IsNullOrWhiteSpace(FilePath))
                    {
                        btnXmlViewer.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
        string ProcessExportXmlAll(ref bool isSuccess, List<V_HIS_TREATMENT_3> hisTreatments, List<V_HIS_PATIENT_TYPE_ALTER> hisPatientTypeAlters, List<V_HIS_SERE_SERV_2> ListSereServ, List<HIS_DHST> listDhst, List<V_HIS_SERE_SERV_TEIN> listSereServTein, List<HIS_TRACKING> hisTrackings, List<V_HIS_SERE_SERV_PTTT> hisSereServPttts, List<HIS_EKIP_USER> ListEkipUser, List<V_HIS_BED_LOG> ListBedlog, List<V_HIS_TREATMENT_1> _listUpdate)
        {
            string result = "";
            this.FilePath = "";
            Dictionary<string, List<string>> DicErrorMess = new Dictionary<string, List<string>>();
            try
            {
                Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>> dicPatientTypeAlter = new Dictionary<long, List<V_HIS_PATIENT_TYPE_ALTER>>();
                Dictionary<long, List<V_HIS_SERE_SERV_2>> dicSereServ = new Dictionary<long, List<V_HIS_SERE_SERV_2>>();
                Dictionary<long, List<V_HIS_SERE_SERV_TEIN>> dicSereServTein = new Dictionary<long, List<V_HIS_SERE_SERV_TEIN>>();
                Dictionary<long, HIS_DHST> dicDhst = new Dictionary<long, HIS_DHST>();
                Dictionary<long, List<HIS_TRACKING>> dicTracking = new Dictionary<long, List<HIS_TRACKING>>();
                Dictionary<long, List<V_HIS_SERE_SERV_PTTT>> dicSereServPttt = new Dictionary<long, List<V_HIS_SERE_SERV_PTTT>>();
                Dictionary<long, List<HIS_EKIP_USER>> dicEkipUser = new Dictionary<long, List<HIS_EKIP_USER>>();
                Dictionary<long, List<V_HIS_BED_LOG>> dicBedLog = new Dictionary<long, List<V_HIS_BED_LOG>>();

                if (hisPatientTypeAlters != null && hisPatientTypeAlters.Count > 0)
                {
                    foreach (var item in hisPatientTypeAlters)
                    {
                        if (!dicPatientTypeAlter.ContainsKey(item.TREATMENT_ID))
                            dicPatientTypeAlter[item.TREATMENT_ID] = new List<V_HIS_PATIENT_TYPE_ALTER>();
                        dicPatientTypeAlter[item.TREATMENT_ID].Add(item);
                    }
                }

                if (ListSereServ != null && ListSereServ.Count > 0)
                {
                    foreach (var sereServ in ListSereServ)
                    {
                        if (sereServ.AMOUNT > 0
                            && sereServ.IS_NO_EXECUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            if (!dicSereServ.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                dicSereServ[sereServ.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_2>();
                            dicSereServ[sereServ.TDL_TREATMENT_ID.Value].Add(sereServ);
                        }

                        if (sereServ.EKIP_ID.HasValue && ListEkipUser != null && ListEkipUser.Count > 0 && sereServ.TDL_TREATMENT_ID.HasValue)
                        {
                            var ekips = ListEkipUser.Where(o => o.EKIP_ID == sereServ.EKIP_ID).ToList();
                            if (ekips != null && ekips.Count > 0)
                            {
                                foreach (var item in ekips)
                                {
                                    if (!dicEkipUser.ContainsKey(sereServ.TDL_TREATMENT_ID.Value))
                                        dicEkipUser[sereServ.TDL_TREATMENT_ID.Value] = new List<HIS_EKIP_USER>();

                                    dicEkipUser[sereServ.TDL_TREATMENT_ID.Value].Add(item);
                                }
                            }
                        }
                    }
                }

                if (listSereServTein != null && listSereServTein.Count > 0)
                {
                    foreach (var ssTein in listSereServTein)
                    {
                        if (!ssTein.TDL_TREATMENT_ID.HasValue) continue;

                        if (!dicSereServTein.ContainsKey(ssTein.TDL_TREATMENT_ID.Value))
                            dicSereServTein[ssTein.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_TEIN>();

                        dicSereServTein[ssTein.TDL_TREATMENT_ID.Value].Add(ssTein);
                    }
                }

                if (hisTrackings != null && hisTrackings.Count > 0)
                {
                    foreach (var tracking in hisTrackings)
                    {
                        if (!dicTracking.ContainsKey(tracking.TREATMENT_ID))
                            dicTracking[tracking.TREATMENT_ID] = new List<HIS_TRACKING>();

                        dicTracking[tracking.TREATMENT_ID].Add(tracking);
                    }
                }

                if (hisSereServPttts != null && hisSereServPttts.Count > 0)
                {
                    foreach (var ssPttt in hisSereServPttts)
                    {
                        if (!ssPttt.TDL_TREATMENT_ID.HasValue) continue;

                        if (!dicSereServPttt.ContainsKey(ssPttt.TDL_TREATMENT_ID.Value))
                            dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value] = new List<V_HIS_SERE_SERV_PTTT>();

                        dicSereServPttt[ssPttt.TDL_TREATMENT_ID.Value].Add(ssPttt);
                    }
                }

                if (listDhst != null && listDhst.Count > 0)
                {
                    //sap xep thoi gian tang dan de trong th co nhieu dhst se lay cai co thoi gian thuc hien lon nhat
                    //lay dhst cuoi cung co can nang
                    listDhst = listDhst.OrderBy(o => o.EXECUTE_TIME).ToList();
                    foreach (var item in listDhst)
                    {
                        if (dicDhst.ContainsKey(item.TREATMENT_ID))
                        {
                            if (item.WEIGHT.HasValue) dicDhst[item.TREATMENT_ID] = item;
                            else if (!dicDhst[item.TREATMENT_ID].WEIGHT.HasValue)
                                dicDhst[item.TREATMENT_ID] = item;
                        }
                        else
                            dicDhst[item.TREATMENT_ID] = item;
                    }
                }

                if (ListBedlog != null && ListBedlog.Count > 0)
                {
                    foreach (var bed in ListBedlog)
                    {
                        if (!dicBedLog.ContainsKey(bed.TREATMENT_ID))
                            dicBedLog[bed.TREATMENT_ID] = new List<V_HIS_BED_LOG>();

                        dicBedLog[bed.TREATMENT_ID].Add(bed);
                    }
                }

                foreach (var treatment in hisTreatments)
                {
                    InputADO ado = new InputADO();
                    ado.Treatment = treatment;
                    V_HIS_PATIENT_TYPE_ALTER _alter = new V_HIS_PATIENT_TYPE_ALTER();
                    if (dicPatientTypeAlter.ContainsKey(treatment.ID))
                    {
                        _alter = dicPatientTypeAlter[treatment.ID].FirstOrDefault(p => p.PATIENT_TYPE_ID == treatment.TDL_PATIENT_TYPE_ID);
                    }
                    ado.LastPatientTypeAlter = _alter;
                    ado.ListSereServ = dicSereServ.ContainsKey(treatment.ID) ? dicSereServ[treatment.ID] : null;
                    ado.Branch = BranchDataWorker.Branch;
                    if (dicDhst.ContainsKey(treatment.ID))
                    {
                        ado.Dhst = dicDhst[treatment.ID];
                    }

                    if (dicSereServTein.ContainsKey(treatment.ID))
                    {
                        ado.SereServTeins = dicSereServTein[treatment.ID];
                    }

                    if (dicTracking.ContainsKey(treatment.ID))
                    {
                        ado.Trackings = dicTracking[treatment.ID];
                    }

                    if (dicSereServPttt.ContainsKey(treatment.ID))
                    {
                        ado.SereServPttts = dicSereServPttt[treatment.ID];
                    }

                    if (dicBedLog.ContainsKey(treatment.ID))
                    {
                        ado.BedLogs = dicBedLog[treatment.ID];
                    }

                    if (dicEkipUser.ContainsKey(treatment.ID))
                    {
                        ado.EkipUsers = dicEkipUser[treatment.ID].Distinct().ToList();
                    }

                    ado.MaterialPackageOption = HisConfigCFG.GetValue(HisConfigCFG.MOS__BHYT__CALC_MATERIAL_PACKAGE_PRICE_OPTION);
                    ado.MaterialPriceOriginalOption = HisConfigCFG.GetValue(HisConfigCFG.XML__4210__MATERIAL_PRICE_OPTION);
                    ado.ListDhsts = listDhst;

                    His.ExportXml.CreateXmlMain xmlMain = new His.ExportXml.CreateXmlMain(ado);

                    string errorMess = "";
                    var fullFileName = xmlMain.Run4210FullPath(ref errorMess);

                    if (String.IsNullOrWhiteSpace(fullFileName))
                    {
                        _listUpdate.Remove(_listUpdate.FirstOrDefault(p => p.ID == treatment.ID));

                        if (!DicErrorMess.ContainsKey(errorMess))
                        {
                            DicErrorMess[errorMess] = new List<string>();
                        }

                        DicErrorMess[errorMess].Add(treatment.TREATMENT_CODE);
                    }
                    else
                    {
                        this.FilePath += fullFileName + ";";
                        isSuccess = true;
                    }
                }

                if (DicErrorMess.Count > 0)
                {
                    foreach (var item in DicErrorMess)
                    {
                        result += String.Format("{0}:{1}. ", item.Key, String.Join(",", item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void btnDropTime_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnDropTime.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind = btnMenuCodeFind.Tag.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportXmlGroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExportXmlGroup.Enabled || listSelection == null || listSelection.Count == 0) return;
                CommonParam param = new CommonParam();
                bool success = false;
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    this.FilePath = fbd.SelectedPath;
                    WaitingManager.Show();
                    success = this.GenerateXml(ref param, listSelection, this.FilePath, IsGroup: true);
                    WaitingManager.Hide();
                    if (success && param.Messages.Count == 0)
                    {
                        listSelectionExported = new List<V_HIS_TREATMENT_1>();
                        listSelectionExported.AddRange(listSelection);
                        MessageManager.Show(this.ParentForm, param, success);
                        this.gridViewTreatment.BeginDataUpdate();
                        this.gridViewTreatment.EndDataUpdate();
                    }
                    else if (success && param.Messages.Count > 0)
                    {
                        success = false;
                        listSelectionExported = new List<V_HIS_TREATMENT_1>();
                        listSelectionExported.AddRange(listSelection);
                        MessageManager.Show(this.ParentForm, param, success);
                        this.gridViewTreatment.BeginDataUpdate();
                        this.gridViewTreatment.EndDataUpdate();
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDownTemp_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath
                + "/Tmp/Imp", "IMPORT_TREATMENT_XML_OTHER_BHYT.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_TREATMENT_XML_OTHER_BHYT";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file về máy trạm thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file về máy trạm thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private List<HisTreatmentView1ImportFilter.TreatmentImportFilter> ProcessDataImport(List<TreatmentImportADO> treatmentImport, ref string error)
        {
            List<HisTreatmentView1ImportFilter.TreatmentImportFilter> result = new List<HisTreatmentView1ImportFilter.TreatmentImportFilter>();
            try
            {
                Inventec.Common.Logging.LogSystem.Info("begin time format");
                string cultureName = "en";
                string timeMax = "";
                if (treatmentImport.Exists(o => !string.IsNullOrEmpty(o.IN_TIME_STR)))
                {
                    var in_time = treatmentImport.Where(o => !string.IsNullOrEmpty(o.IN_TIME_STR)).ToList();
                    if (in_time != null && in_time.Count() > 0)
                    {
                        timeMax = in_time.OrderByDescending(o => o.IN_TIME_STR.Length).ThenByDescending(o => o.IN_TIME_STR).First().IN_TIME_STR;
                    }
                }
                else if (treatmentImport.Exists(o => !string.IsNullOrEmpty(o.OUT_TIME_STR)))
                {
                    var out_time = treatmentImport.Where(o => !string.IsNullOrEmpty(o.OUT_TIME_STR)).ToList();
                    if (out_time != null && out_time.Count() > 0)
                    {
                        timeMax = out_time.OrderByDescending(o => o.IN_TIME_STR.Length).ThenByDescending(o => o.IN_TIME_STR).First().OUT_TIME_STR;
                    }
                }

                if (!String.IsNullOrEmpty(timeMax))
                {
                    try
                    {
                        var dateTime = Convert.ToDateTime(timeMax);
                        if (dateTime != null)
                        {
                            cultureName = "vi";
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        cultureName = "en";
                    }
                }

                CustomProvider provider = new CustomProvider(cultureName);
                Inventec.Common.Logging.LogSystem.Info("end time format");
                foreach (var item in treatmentImport)
                {
                    if (item == null)
                        continue;

                    if (string.IsNullOrEmpty(item.IN_TIME_STR.Trim())
                        && string.IsNullOrEmpty(item.OUT_TIME_STR.Trim())
                        && string.IsNullOrEmpty(item.TDL_PATIENT_CODE.Trim())
                        && string.IsNullOrEmpty(item.TDL_PATIENT_NAME.Trim())
                        && string.IsNullOrEmpty(item.TREATMENT_CODE.Trim())
                        && string.IsNullOrEmpty(item.ICD_CODEs.Trim())) continue;

                    HisTreatmentView1ImportFilter.TreatmentImportFilter filter = new HisTreatmentView1ImportFilter.TreatmentImportFilter();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentView1ImportFilter.TreatmentImportFilter>(filter, item);
                    if (!string.IsNullOrEmpty(item.ICD_CODEs.Trim()) && (string.IsNullOrEmpty(item.IN_TIME_STR) || string.IsNullOrEmpty(item.OUT_TIME_STR)))
                    {
                        error += string.Format("Bạn chưa nhập đủ thông tin  “Ngày vào” hoặc “Ngày ra”.");
                    }
                    if (!string.IsNullOrEmpty(item.IN_TIME_STR))
                    {
                        try
                        {
                            var dateTime = Convert.ToDateTime(item.IN_TIME_STR, provider);
                            filter.IN_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTime);
                            item.IN_TIME = filter.IN_TIME;
                        }
                        catch (Exception)
                        {
                            error += string.Format("Ngày vào {0} không hợp lệ|", item.IN_TIME_STR);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.OUT_TIME_STR))
                    {
                        try
                        {
                            var dateTime = Convert.ToDateTime(item.OUT_TIME_STR, provider);
                            filter.OUT_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateTime);
                            item.OUT_TIME = filter.OUT_TIME;
                        }
                        catch (Exception)
                        {
                            error += string.Format("Ngày ra {0} không hợp lệ|", item.OUT_TIME_STR);
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TDL_PATIENT_CODE))
                    {
                        if (item.TDL_PATIENT_CODE.Length < 10 && checkDigit(item.TDL_PATIENT_CODE))
                        {
                            filter.TDL_PATIENT_CODE = string.Format("{0:0000000000}", Convert.ToInt64(item.TDL_PATIENT_CODE));
                            item.TDL_PATIENT_CODE = string.Format("{0:0000000000}", Convert.ToInt64(item.TDL_PATIENT_CODE));
                        }
                        else
                        {
                            filter.TDL_PATIENT_CODE = item.TDL_PATIENT_CODE;
                        }
                    }

                    if (!string.IsNullOrEmpty(item.TREATMENT_CODE))
                    {
                        if (item.TREATMENT_CODE.Length < 12 && checkDigit(item.TREATMENT_CODE))
                        {
                            filter.TREATMENT_CODE = string.Format("{0:000000000000}", Convert.ToInt64(item.TREATMENT_CODE));
                            item.TREATMENT_CODE = string.Format("{0:000000000000}", Convert.ToInt64(item.TREATMENT_CODE));
                        }
                        else
                        {
                            filter.TREATMENT_CODE = item.TREATMENT_CODE;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.ICD_CODEs))
                    {
                        filter.ICD_CODEs = item.ICD_CODEs.Trim().Split(this.icdSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }

                    result.Add(filter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            if (result.Count == 0)
                return null;
            return result;
        }
        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();

                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        this.listTreatmentImport = import.GetWithCheck<TreatmentImportADO>(0);
                        if (this.listTreatmentImport != null && this.listTreatmentImport.Count > 0)
                        {
                            string error = "";
                            List<HisTreatmentView1ImportFilter.TreatmentImportFilter> processImport = ProcessDataImport(this.listTreatmentImport, ref error);

                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => processImport), processImport));
                            List<V_HIS_TREATMENT_1> listTreatment = new List<V_HIS_TREATMENT_1>();

                            if (!string.IsNullOrEmpty(error))
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show(error, MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return;
                            }
                            else if (processImport == null)
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Lỗi khi lấy dữ liệu lọc", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                                return;
                            }
                            else
                            {
                                var skip = 0;
                                while (processImport.Count - skip >= 0)
                                {
                                    var imports = processImport.Skip(skip).Take(20).ToList();
                                    skip += 20;
                                    CommonParam param = new CommonParam();
                                    HisTreatmentView1ImportFilter filter = new HisTreatmentView1ImportFilter();
                                    filter.TreatmentImportFilters = imports;
                                    filter.ORDER_DIRECTION = "DESC";
                                    filter.ORDER_FIELD = "TREATMENT_CODE";

                                    var rsApi = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_1>>("api/HisTreatment/GetByImportView1", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                                    if (rsApi != null)
                                    {
                                        listTreatment.AddRange(rsApi);
                                    }
                                }

                                if (listTreatment != null && listTreatment.Count > 0)//lọc lại danh sách
                                {
                                    listTreatment = listTreatment.GroupBy(o => o.ID).Select(s => s.First()).ToList();
                                }

                                if (listTreatment != null && listTreatment.Count > 0 && ucPaging1 != null && ucPaging1.pagingGrid != null)
                                {
                                    ucPaging1.pagingGrid.CurrentPage = 1;
                                    ucPaging1.pagingGrid.PageCount = 1;
                                    ucPaging1.pagingGrid.MaxRec = listTreatment.Count;
                                    ucPaging1.pagingGrid.DataCount = listTreatment.Count;
                                    ucPaging1.pagingGrid.LoadPage();
                                }

                                gridControlTreatment.BeginUpdate();
                                gridControlTreatment.DataSource = listTreatment;
                                gridControlTreatment.EndUpdate();

                                WaitingManager.Hide();
                            }
                        }
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
    }
}
