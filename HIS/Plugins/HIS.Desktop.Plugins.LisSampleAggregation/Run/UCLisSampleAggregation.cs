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
using Inventec.Desktop.Common.Modules;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utility;
using LIS.Filter;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Adapter;
using DevExpress.XtraGrid.Columns;
using System.Dynamic;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.LisSampleAggregation.ADO;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.LocalStorage.ConfigApplication;
using LIS.SDO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraBars;

namespace HIS.Desktop.Plugins.LisSampleAggregation.Run
{
    public partial class UCLisSampleAggregation : UserControlBase
    {
        private Module currentModule;
        private List<LIS_SAMPLE> ListDataCreateAggr = new List<LIS_SAMPLE>();
        private List<LIS_SAMPLE> ListDataSample2 = null;
        internal PopupMenu menu;
        public enum ProcessType
        {
            GOP_MAU,
        }

        private int MaxColumn = 0;
        private int MaxRow = 0;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.LisSampleAggregation";

        List<ExpandoObject> ListDataSource = new List<ExpandoObject>();

        List<ExpandoObject> ListSampleAggrPrint = new List<ExpandoObject>();

        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private int limit = 0;

        private ExpandoObject EditData;
        long? trayNumber;
        DateTime trayTime;
        MPS.ProcessorBase.PrintConfig.PreviewType? workingPreviewType;

        public UCLisSampleAggregation(Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
        }

        private void UCLisSampleAggregation_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                SetDefaultControlsProperties();
                LoadDataToComboControls();
                ResetControl();
                FillDataToGridControl();
                txtSample.Focus();
                txtBarCode.Focus();
                
                InitControlState();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.chkBarCode)
                        {
                            chkBarCode.Checked = item.VALUE == "1";
                        }
                    }
                }
                if (chkBarCode.Checked == true)
                {

                    txtBarCode.Focus();
                    txtBarCode.SelectAll();

                }
                else
                {
                    txtSample.Focus();
                    txtSample.SelectAll();
                }

                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstan.toggleSwitch1)
                        {
                            toggleSwitch1.IsOn = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControlsProperties()
        {
            try
            {
                if (toggleSwitch1.IsOn)
                {
                    lciToggleSwitch1.Text = "Chọn mẫu theo mã vạch";
                    navigationFrame1.SelectedPage = navigationPage2;
                }
                else
                {
                    lciToggleSwitch1.Text = "Chọn mẫu từ phiếu giao nhận";
                    navigationFrame1.SelectedPage = navigationPage1;
                }
                
                navigationFrame1.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region private method
        private void LoadDataComboFilter()
        {
            try
            {
                List<ComboADO> status = new List<ComboADO>();
                status.Add(new ComboADO(0, "Tất cả"));
                status.Add(new ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM, "Chưa lấy mẫu"));
                status.Add(new ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM, "Đã lấy mẫu"));
                status.Add(new ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ, "Có kết quả"));
                status.Add(new ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ, "Trả kết quả"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboSampleStt, status, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControl()
        {
            try
            {
                txtSample.Text = "";
                dtTimeFrom.EditValue = DateTime.Now;
                dtTimeTo.EditValue = DateTime.Now;
                txtBarcodeSearch.Text = "";
                txtKeyword.Text = "";
                cboSampleStt.EditValue = 0;

                spinTrayFrom.EditValue = null;
                spinTrayTo.EditValue = null;

                //xóa text barcode sửa
                lblAggrBarcodeEdit.Text = " ";
                lciAggrBarcode.Text = " ";
                lciCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                //Reset tab 'Chọn mẫu từ phiếu giao nhận'
                cboDeliveryNote.EditValue = null;
                cboSampleRoom.EditValue = null;
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
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSample.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.txtSample.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAddSample.ToolTip = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.btnAddSample.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAggrSample.Text = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.btnAggrSample.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_DeleteSample.Caption = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.gc_DeleteSample.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Barcode.Caption = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.gc_Barcode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientName.Caption = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.gc_PatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_DeleteAggrSample.Caption = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.gc_DeleteAggrSample.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_AggregateBarcode.Caption = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.gc_AggregateBarcode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup2.Text = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.layoutControlGroup2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                int pageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = (int)ConfigApplications.NumPageSize;
                }
                FillDataToGridSample(new CommonParam(0, pageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridSample, param, pageSize, gridControlSampleAggregation);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridSample(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                LisSampleAggregationViewFilter lisSampleFilter = new LisSampleAggregationViewFilter();
                SetFilter(ref lisSampleFilter);
                var apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE_AGGREGATION>>("api/LisSample/GetAggregationView", ApiConsumer.ApiConsumers.LisConsumer, lisSampleFilter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_LIS_SAMPLE_AGGREGATION>)apiResult.Data;
                    ProcessDataGrid(data);
                    rowCount = (data == null ? 0 : data.Count);
                    dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    WaitingManager.Hide();
                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref LisSampleAggregationViewFilter lisSampleFilter)
        {
            try
            {
                lisSampleFilter.ORDER_FIELD = "CREATE_TIME";
                lisSampleFilter.ORDER_DIRECTION = "DESC";

                if (!String.IsNullOrEmpty(txtBarcodeSearch.Text))
                {
                    lisSampleFilter.BARCODE__EXACT = txtBarcodeSearch.Text.Trim();
                }
                else
                {
                    if (!String.IsNullOrEmpty(txtKeyword.Text))
                    {
                        lisSampleFilter.KEY_WORD = txtKeyword.Text.Trim();
                    }

                    if (dtTimeFrom != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        lisSampleFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    if (dtTimeTo != null && dtTimeTo.DateTime != DateTime.MinValue)
                        lisSampleFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                    if (spinTrayFrom.Value > 0)
                        lisSampleFilter.TRAY_NUMBER_FROM = (long)spinTrayFrom.Value;
                    if (spinTrayTo.Value > 0)
                        lisSampleFilter.TRAY_NUMBER_TO = (long)spinTrayTo.Value;


                    //Tất cả 0
                    //Chưa lấy mẫu 1
                    //Đã lấy mẫu 2
                    //Đã có kết quả
                    //Đã trả kết quả
                    //Filter yeu cau chua lấy mẫu
                    if (cboSampleStt.EditValue != null)
                    {
                        //Chưa lấy mẫu
                        if (Inventec.Common.TypeConvert.Parse.ToInt64(cboSampleStt.EditValue.ToString()) == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                        {
                            lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                            };
                        }
                        //Đã lấy mẫu
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(cboSampleStt.EditValue.ToString()) == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                        {
                            lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            };
                        }
                        //đã có kết quả
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(cboSampleStt.EditValue.ToString()) == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                        {
                            lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            };
                        }//đã trả kết quả
                        else if (Inventec.Common.TypeConvert.Parse.ToInt64(cboSampleStt.EditValue.ToString()) == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAddSampleByBarcode(string barcode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(barcode))
                {
                    CommonParam param = new CommonParam();
                    LisSampleFilter filter = new LisSampleFilter();
                    filter.BARCODE__EXACT = barcode;
                    var apiResult = new BackendAdapter(param).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumer.ApiConsumers.LisConsumer, filter, SessionManager.ActionLostToken, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        if (apiResult.Count > 1)
                        {
                            FormSelectSample form = new FormSelectSample(this.currentModule, apiResult, SelectedSample);
                            form.ShowDialog();
                        }
                        else if (apiResult.Exists(o => o.AGGREGATE_SAMPLE_ID.HasValue))
                        {
                            XtraMessageBox.Show(Resources.ResourceLanguageManager.MauDaGop, Resources.ResourceLanguageManager.ThongBao);
                        }
                        else
                        {
                            var notExist = apiResult.Where(o => !this.ListDataCreateAggr.Exists(e => e.ID == o.ID)).ToList();
                            if (notExist != null && notExist.Count > 0)
                            {
                                this.ListDataCreateAggr.AddRange(notExist);
                            }
                        }

                        gridControlSamples.DataSource = this.ListDataCreateAggr;
                        gridControlSamples.RefreshDataSource();
                    }
                    else
                    {
                        XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongCoMauBenhPhamUngVoiBarcode);
                    }

                    txtSample.Focus();
                    txtSample.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectedSample(List<LIS_SAMPLE> data)
        {
            try
            {
                if (data != null && data.Count > 0)
                {
                    var notExist = data.Where(o => !this.ListDataCreateAggr.Exists(e => e.ID == o.ID)).ToList();
                    if (notExist != null && notExist.Count > 0)
                    {
                        this.ListDataCreateAggr.AddRange(notExist);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenColumn(int numCol)
        {
            try
            {
                if (numCol > 0)
                {
                    for (int i = 0; i < numCol; i++)
                    {
                        GridColumn colKho = new GridColumn();
                        colKho.Caption = Resources.ResourceLanguageManager.Mau + (MaxColumn + i + 1);
                        colKho.FieldName = "BARCODE_" + (MaxColumn + i);
                        colKho.VisibleIndex = 6 + MaxColumn + i;
                        colKho.Width = 100;
                        colKho.OptionsColumn.AllowEdit = false;
                        gridViewSampleAggregation.Columns.Add(colKho);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGrid(List<V_LIS_SAMPLE_AGGREGATION> data)
        {
            try
            {
                ListDataSource = new List<ExpandoObject>();
                if (data != null && data.Count > 0)
                {
                    //gom nhóm để tạo ra cột động bằng số phần tử lớn nhất của 1 mã gộp 
                    int maxGroup = GetMaxChild(data);
                    if (maxGroup > MaxColumn)
                    {
                        GenColumn(maxGroup - MaxColumn);
                        MaxColumn = maxGroup;
                    }

                    foreach (var item in data)
                    {
                        ExpandoObject dt = new ExpandoObject();
                        AddProperty(dt, "ID", item.ID);
                        AddProperty(dt, "AGGREGATE_BARCODE", item.BARCODE);
                        AddProperty(dt, "RowNum", MaxRow);
                        AddProperty(dt, "SAMPLE_STT_ID", item.SAMPLE_STT_ID);
                        AddProperty(dt, "TRAY_NUMBER", item.TRAY_NUMBER);
                        AddProperty(dt, "TRAY_TIME", item.TRAY_TIME);
                        AddProperty(dt, "TRAY_ID", item.TRAY_ID);
                        AddProperty(dt, "VIR_TRAY_DATE", item.VIR_TRAY_DATE);
                        AddProperty(dt, "CHILD_BARCODE", item.CHILD_BARCODE);

                        int count = 0;
                        if (!String.IsNullOrWhiteSpace(item.CHILD_BARCODE))
                        {
                            string[] barcodes = item.CHILD_BARCODE.Split('|');
                            foreach (var sample in barcodes)
                            {
                                AddProperty(dt, "BARCODE_" + count, sample);
                                count++;
                            }
                        }

                        //tạo cột trống để hiển thị được đủ theo số cột lớn nhất
                        if (count < MaxColumn)
                        {
                            for (int i = 0; i < MaxColumn - count; i++)
                            {
                                AddProperty(dt, "BARCODE_" + (count + i), "");
                            }
                        }

                        MaxRow++;
                        ListDataSource.Add(dt);
                    }
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDataSource), ListDataSource));
                gridControlSampleAggregation.DataSource = null;
                gridControlSampleAggregation.DataSource = ListDataSource;
                gridControlSampleAggregation.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private int GetMaxChild(List<V_LIS_SAMPLE_AGGREGATION> data)
        {
            int result = 0;
            try
            {
                if (data != null && data.Count > 0)
                {
                    result = (int)data.Where(o => o.COUNT_BARCODE.HasValue).Max(o => o.COUNT_BARCODE.Value);
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            try
            {
                var expandoDict = expando as IDictionary<string, object>;
                expandoDict[propertyName] = propertyValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EditAggrSampe(long ID)
        {
            try
            {
                if (ID > 0)
                {
                    ExpandoObject data = ListDataSource.FirstOrDefault(x => Inventec.Common.TypeConvert.Parse.ToInt64(((IDictionary<string, object>)x)["ID"].ToString()) == ID);
                    if (data != null)
                    {
                        this.EditData = data;

                        CommonParam param = new CommonParam();
                        LisSampleFilter filter = new LisSampleFilter();
                        filter.AGGREGATE_SAMPLE_ID = ID;
                        this.ListDataCreateAggr = new BackendAdapter(param).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumer.ApiConsumers.LisConsumer, filter, param);

                        if (this.ListDataCreateAggr != null && this.ListDataCreateAggr.Count > 0)
                        {
                            this.ListDataCreateAggr = this.ListDataCreateAggr.OrderBy(o => o.BARCODE).ToList();
                        }

                        gridControlSamples.DataSource = this.ListDataCreateAggr;
                        gridControlSamples.RefreshDataSource();

                        var expandoDict = data as IDictionary<string, object>;
                        if (expandoDict.ContainsKey("AGGREGATE_BARCODE"))
                        {
                            lblAggrBarcodeEdit.Text = (expandoDict["AGGREGATE_BARCODE"] ?? "").ToString();
                            lciAggrBarcode.Text = Inventec.Common.Resource.Get.Value("UCLisSampleAggregation.gc_AggregateBarcode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()) + ":";
                        }

                        lciCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DeleteAggrSampe(long ID)
        {
            try
            {
                if (ID > 0)
                {
                    if (MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        var apiresult = new BackendAdapter(param).Post<bool>("api/LisSample/Delete", ApiConsumer.ApiConsumers.LisConsumer, ID, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGridControl();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                        SessionManager.ProcessTokenLost(param);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DeleteSetTray(long id)
        {
            try
            {
                if (id > 0)
                {
                    DialogResult myResult;
                    myResult = MessageBox.Show("Bạn có chắc muốn xóa thông tin khay của mẫu gộp không", MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (myResult == DialogResult.Yes)
                    {
                        //TODO
                        bool success = false;
                        RemoveTraySDO removeTraySDO = new RemoveTraySDO();
                        removeTraySDO.AggSampleIds = new List<long>();
                        removeTraySDO.AggSampleIds.Add(id);

                        CommonParam param = new CommonParam();
                        success = new BackendAdapter(param).Post<bool>("api/LisSample/RemoveTray", ApiConsumers.LisConsumer, removeTraySDO, 0, SessionManager.ActionLostToken, param);
                        if (success)
                        {
                            FillDataToGridControl();
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadListSample()
        {
            try
            {
                FillDataToGridControl();
                ListDataCreateAggr = new List<LIS_SAMPLE>();
                gridControlSamples.DataSource = this.ListDataCreateAggr;
                gridControlSamples.RefreshDataSource();

                this.EditData = null;

                lciCancel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lblAggrBarcodeEdit.Text = " ";
                lciAggrBarcode.Text = " ";

                txtSample.Focus();
                txtSample.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = true;
            try
            {
                WaitingManager.Show();
                if (ListSampleAggrPrint != null && ListSampleAggrPrint.Count > 0)
                {
                    MPS.Processor.Mps000435.PDO.Mps000435PDO mps000435pdo = new MPS.Processor.Mps000435.PDO.Mps000435PDO(ListSampleAggrPrint);
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;

                    string printerName = "";
                    WaitingManager.Hide();
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }
                    MPS.ProcessorBase.PrintConfig.PreviewType previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                    if (this.workingPreviewType != null)
                    {
                        previewType = workingPreviewType.Value;
                    }
                    else if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                    }
                    else
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                    }
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000435pdo, previewType, printerName);

                    result = MPS.MpsPrinter.Run(PrintData);
                    Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => PrintData), PrintData));
                }
            }
            catch (Exception ex)
            {
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private long? GetMaxTray()
        {
            long? max = 0;
            try
            {
                CommonParam param = new CommonParam();
                dynamic filter = new System.Dynamic.ExpandoObject();
                filter.IS_ACTIVE = 1;
                filter.VIR_TRAY_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now);//TODO

                LisTrayMaxTrayNumberFilter trayMaxTrayNumberFilter = new LisTrayMaxTrayNumberFilter();
                trayMaxTrayNumberFilter.TRAY_DATE = Inventec.Common.TypeConvert.Parse.ToInt64(DateTime.Now.ToString("yyyyMMdd000000"));

                var data = new BackendAdapter(param).Get<MaxTrayNumberSDO>("api/LisTray/GetMaxTrayNumber", ApiConsumers.LisConsumer, trayMaxTrayNumberFilter, param);
                if (data != null && data.MaxTrayNumber.HasValue)
                {
                    max = data.MaxTrayNumber.Value + 1;
                }
                else
                {
                    max = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return max;
        }

        private void DelegateRunPrinterWithType(MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                PrintMps(previewType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<LIS_SAMPLE> SetListDataCreateAggrByFormData()
        {
            var ListDataCreateAggrData = new List<LIS_SAMPLE>();
            var rowHandles = gridViewSampleAggregation.GetSelectedRows();
            if (rowHandles != null && rowHandles.Count() > 0)
            {
                foreach (var i in rowHandles)
                {
                    var row = (ExpandoObject)gridViewSampleAggregation.GetRow(i);
                    if (row != null)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                        LIS_SAMPLE sample = new LIS_SAMPLE();
                        var expandoDict = row as IDictionary<string, object>;
                        if (expandoDict != null)
                        {
                            sample.ID = Inventec.Common.TypeConvert.Parse.ToInt64((expandoDict["ID"] ?? "").ToString());
                            sample.BARCODE = (expandoDict["CHILD_BARCODE"] ?? "").ToString();
                            sample.TDL_AGGREGATE_BARCODE = (expandoDict["AGGREGATE_BARCODE"] ?? "").ToString();
                            sample.SAMPLE_STT_ID = Inventec.Common.TypeConvert.Parse.ToInt64((expandoDict["SAMPLE_STT_ID"] ?? "").ToString());
                            sample.TRAY_ID = Inventec.Common.TypeConvert.Parse.ToInt64((expandoDict["TRAY_ID"] ?? "").ToString());
                        }

                        ListDataCreateAggrData.Add(sample);
                    }
                }
            }
            return ListDataCreateAggrData;
        }

        List<ExpandoObject> SetListDataExpandoObjectByFormData()
        {
            var ListDataCreateAggrData = new List<ExpandoObject>();
            var rowHandles = gridViewSampleAggregation.GetSelectedRows();
            if (rowHandles != null && rowHandles.Count() > 0)
            {
                foreach (var i in rowHandles)
                {
                    var row = (ExpandoObject)gridViewSampleAggregation.GetRow(i);
                    if (row != null)
                    {
                        ListDataCreateAggrData.Add(row);
                    }
                }
            }
            return ListDataCreateAggrData;
        }

        private bool ActSetTrayFromPopup(long? traynumber, DateTime trayTime, bool isPrint)
        {
            bool success = false;
            try
            {
                this.trayNumber = traynumber;
                this.trayTime = trayTime;

                CommonParam param = new CommonParam();
                PutTraySDO putTraySDO = new PutTraySDO();
                putTraySDO.TrayNumber = traynumber ?? 0;
                putTraySDO.TrayTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(trayTime) ?? 0;
                putTraySDO.AggSampleIds = ListDataCreateAggr.Select(o => o.ID).Distinct().ToList();

                var rs = new BackendAdapter(param).Post<List<LIS_SAMPLE>>("api/LisSample/PutTray", ApiConsumers.LisConsumer, putTraySDO, param);
                Inventec.Common.Logging.LogSystem.Debug("api/LisSample/PutTray____Input:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => putTraySDO), putTraySDO)
                    + "____Result:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => putTraySDO), putTraySDO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isPrint), isPrint));
                if (rs != null && rs.Count > 0)
                {
                    success = true;
                    FillDataToGridControl();
                    int rowIndex = 0;
                    var listAggrTrayIds = rs.Select(k => k.TRAY_ID ?? 0).Distinct().ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAggrTrayIds), listAggrTrayIds));
                    foreach (var item in ListDataSource)
                    {
                        var expandoDict = item as IDictionary<string, object>;
                        if (listAggrTrayIds.Contains(Inventec.Common.TypeConvert.Parse.ToInt64((expandoDict["TRAY_ID"] ?? "").ToString())))
                        {
                            gridViewSampleAggregation.SelectRow(rowIndex);
                            Inventec.Common.Logging.LogSystem.Debug("gridViewSampleAggregation.SelectRow:" + rowIndex);
                        }

                        rowIndex++;
                    }
                    if (isPrint)
                    {
                        PrintMps(MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow);
                    }
                }
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        #endregion

        #region public method
        public void CreateAggrSample()
        {
            try
            {
                btnAggrSample_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SearchData()
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

        public void SetTrayShortcut()
        {
            try
            {
                if (btnSetTray.Enabled)
                {
                    btnSetTray_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Event
        private void btnSetTray_Click(object sender, EventArgs e)
        {
            try
            {
                this.ListDataCreateAggr = this.SetListDataCreateAggrByFormData();

                if (this.ListDataCreateAggr == null || this.ListDataCreateAggr.Count == 0)
                {
                    MessageManager.Show("Bạn chưa chọn mẫu gộp");
                    Inventec.Common.Logging.LogSystem.Debug("Bạn chưa chọn mẫu gộp" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDataCreateAggr), ListDataCreateAggr));
                    return;
                }
                var dataAggrByTray = this.ListDataCreateAggr.Where(o => o.TRAY_ID != null && o.TRAY_ID.Value > 0).ToList();
                if (dataAggrByTray != null && dataAggrByTray.Count > 0)
                {
                    string messageError = "";
                    //foreach (var item in dataAggrByTray)
                    //{
                    messageError += String.Format("Mẫu {0} đã được đặt vào khay", String.Join(", ", dataAggrByTray.Select(t => t.TDL_AGGREGATE_BARCODE)).Replace("|", ", "));
                    //}
                    MessageManager.Show(messageError);
                    Inventec.Common.Logging.LogSystem.Debug(messageError + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDataCreateAggr), ListDataCreateAggr));
                    return;
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDataCreateAggr), ListDataCreateAggr));

                long? maxTrayNumber = this.GetMaxTray();
                frmSetTray frmSetTray = new frmSetTray(maxTrayNumber, DateTime.Now, ActSetTrayFromPopup, DelegateRunPrinterWithType);
                frmSetTray.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteSample_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (LIS_SAMPLE)gridViewSamples.GetFocusedRow();
                if (row != null)
                {
                    bool remove = false;
                    if (this.EditData == null || MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        remove = true;
                    }

                    if (remove)
                    {
                        this.ListDataCreateAggr.Remove(row);
                        gridControlSamples.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSamples_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == gc_PatientName.FieldName)
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSample_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter && !String.IsNullOrWhiteSpace(txtSample.Text))
                {
                    ProcessAddSampleByBarcode(txtSample.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBarcodeSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtBarcodeSearch.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtKeyword.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        cboSampleStt.Focus();
                        cboSampleStt.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSampleAggregation_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        long ID = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(hi.RowHandle, "ID") ?? "").ToString());
                        long sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(hi.RowHandle, "SAMPLE_STT_ID") ?? "").ToString());
                        if (ID > 0)
                        {
                            if (hi.Column.FieldName == gc_EditAggrSample.FieldName)
                            {
                                #region ----- Sửa -----
                                if (sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                                    && sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                                    && sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                                {
                                    EditAggrSampe(ID);
                                }
                                #endregion
                            }
                            if (hi.Column.FieldName == gc_DeleteAggrSample.FieldName)
                            {
                                #region ----- Xóa -----
                                if (sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                                    && sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                                    && sampleStt != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                                {
                                    DeleteAggrSampe(ID);
                                }
                                #endregion
                            }

                            if (hi.Column.FieldName == gridColumnDeleteTray.FieldName)
                            {
                                #region ----- Xóa đặt khay -----
                                long TRAY_NUMBER = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(hi.RowHandle, "TRAY_NUMBER") ?? "").ToString());
                                if (TRAY_NUMBER > 0 && ID > 0)
                                {
                                    DeleteSetTray(ID);
                                }
                                #endregion
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

        private void gridViewSampleAggregation_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSampleAggregation.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID") ?? "").ToString());
                    if (e.Column.FieldName == gc_EditAggrSample.FieldName)
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            e.RepositoryItem = repositoryItemBtnEditAggSampleDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnEditAggSample;
                        }
                    }
                    if (e.Column.FieldName == gc_DeleteAggrSample.FieldName)
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteAggSampleDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDeleteAggSample;
                        }
                    }

                    if (e.Column.FieldName == gridColumnDeleteTray.FieldName)
                    {
                        long TRAY_NUMBER = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSampleAggregation.GetRowCellValue(e.RowHandle, "TRAY_NUMBER") ?? "").ToString());
                        if (TRAY_NUMBER > 0)
                        {
                            e.RepositoryItem = repositoryItembtnDeleteTray;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextEditReadOnly;
                        }
                    }
                }
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
                PrintMps(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintMps(MPS.ProcessorBase.PrintConfig.PreviewType? previewType)
        {
            try
            {
                this.workingPreviewType = previewType;
                ListSampleAggrPrint = this.SetListDataExpandoObjectByFormData();

                if (ListSampleAggrPrint != null && ListSampleAggrPrint.Count > 0)
                {
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                    richEditorMain.RunPrintTemplate("Mps000435", DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                ReloadListSample();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAggrSample_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtBarCode.Text.Length > 15)
                {
                    //string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon
                    MessageBox.Show("Độ dài Barcode mẫu gộp lớn hơn 15","Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (this.ListDataCreateAggr.Count > 0)
                    {
                        WaitingManager.Show();
                        var listSampleId = this.ListDataCreateAggr.Select(s => s.ID).ToList();
                        CommonParam param = new CommonParam();
                        bool success = false;

                        List<LIS_SAMPLE> apiresult = null;
                        if (this.EditData != null)
                        {
                            var data = this.EditData as IDictionary<string, object>;
                            if (data.ContainsKey("ID") && data["ID"] != null)
                            {
                                UpdateSampleAggregateSDO sdo = new UpdateSampleAggregateSDO();
                                //AggregateSDO sdo = new AggregateSDO();
                                 sdo.AggregateSampleId = Inventec.Common.TypeConvert.Parse.ToInt64(data["ID"].ToString());
                                sdo.SampleIds = listSampleId;
                               // sdo.Barcode = txtBarCode.Text;
                                
                                apiresult = new BackendAdapter(param).Post<List<LIS_SAMPLE>>("api/LisSample/UpdateAggregate", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                            }
                        }
                        else
                        {
                            AggregateSDO sdo = new AggregateSDO();
                            // sdo.AggregateSampleId = Inventec.Common.TypeConvert.Parse.ToInt64(data["ID"].ToString());
                            sdo.SampleIds = listSampleId;
                            sdo.Barcode = txtBarCode.Text;
                            apiresult = new BackendAdapter(param).Post<List<LIS_SAMPLE>>("api/LisSample/Aggregate", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        }

                        if (apiresult != null && apiresult.Count > 0)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiresult), apiresult));
                            ReloadListSample();

                            success = true;

                           
                        }
                        if (chkBarCode.Checked == true)
                        {
                            if (success == true)
                            {
                                txtBarCode.Text = null;
                                txtSample.Text = null;
                                gridControlSamples.BeginUpdate();
                                gridControlSamples.DataSource = null;
                                gridControlSamples.EndUpdate();
                                gridControlSamples.Refresh();
                                txtBarCode.Focus();
                                txtBarCode.SelectAll();
                            }
                        }
                        else
                        {
                            if (success == true)
                            {
                                txtBarCode.Text = null;
                                txtSample.Text = null;
                                gridControlSamples.BeginUpdate();
                                gridControlSamples.DataSource = null;
                                gridControlSamples.EndUpdate();
                                gridControlSamples.Refresh();
                                txtSample.Focus();
                                txtSample.SelectAll();
                            }
                        }

                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                        SessionManager.ProcessTokenLost(param);
                    }
                    else
                    {
                        XtraMessageBox.Show(Resources.ResourceLanguageManager.KhongCoDanhSachMau, Resources.ResourceLanguageManager.ThongBao);
                        txtSample.Focus();
                        txtSample.SelectAll();
                    }
                  
                }
                
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAddSample_Click(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtSample.Text))
                {
                    ProcessAddSampleByBarcode(txtSample.Text);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridViewSampleAggregation_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (ExpandoObject)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        var expandoDict = data as IDictionary<string, object>;
                        if (e.Column.FieldName == "TRAY_DATE_DISPLAY" && expandoDict != null && expandoDict.ContainsKey("VIR_TRAY_DATE"))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("(long)VIR_TRAY_DATE=" + (long)Inventec.Common.TypeConvert.Parse.ToDecimal((expandoDict["VIR_TRAY_DATE"] ?? "").ToString()));
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)Inventec.Common.TypeConvert.Parse.ToDecimal((expandoDict["VIR_TRAY_DATE"] ?? "").ToString()));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItembtnDeleteTray_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTrayFrom_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTrayTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBarCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSample.Focus();
                    txtSample.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBarCode_CheckedChanged(object sender, EventArgs e)
        {
            WaitingManager.Show();
            //if (chkSign.Checked == false)
            //{
            //    chkPrintDocumentSigned.Checked = false;
            //}

            HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.chkBarCode && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
            if (csAddOrUpdate != null)
            {
                csAddOrUpdate.VALUE = (chkBarCode.Checked ? "1" : "");
            }
            else
            {
                csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                csAddOrUpdate.KEY = ControlStateConstan.chkBarCode;
                csAddOrUpdate.VALUE = (chkBarCode.Checked ? "1" : "");
                csAddOrUpdate.MODULE_LINK = moduleLink;
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                this.currentControlStateRDO.Add(csAddOrUpdate);
            }
            this.controlStateWorker.SetData(this.currentControlStateRDO);
            WaitingManager.Hide();
        }

        // Chế độ chọn mẫu từ phiếu giao nhận
        #region Chế độ chọn mẫu từ phiếu giao nhận

        private void LoadDataToComboControls()
        {
            try
            {
                LoadDataComboFilter();
                InitComboSampleRoom();
                InitComboDeliveryNote();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboSampleRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<HIS_SAMPLE_ROOM> data = BackendDataWorker.Get<HIS_SAMPLE_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("SAMPLE_ROOM_CODE", "Mã", 50, 1));
                columnInfos.Add(new ColumnInfo("SAMPLE_ROOM_NAME", "Tên", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("SAMPLE_ROOM_NAME", "SAMPLE_ROOM_CODE", columnInfos, true, 200);
                ControlEditorLoader.Load(cboSampleRoom, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDeliveryNote()
        {
            try
            {
                List<DeliveryNoteADO> data = null;
                cboDeliveryNote.EditValue = null;
                if (cboSampleRoom.EditValue != null)
                {
                    CommonParam param = new CommonParam();
                    List<long> listDeliveryNoteStatus = new List<long>();
                    listDeliveryNoteStatus.Add(3);
                    LisDeliveryNoteFilter filter = new LisDeliveryNoteFilter();
                    filter.SEND_ROOM_CODE__EXACT = (cboSampleRoom.EditValue ?? "").ToString();
                    filter.DELIVERY_NOTE_STATUSES = listDeliveryNoteStatus;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("__filter ", filter));
                    var apiResult = new BackendAdapter(param).GetRO<List<LIS_DELIVERY_NOTE>>("api/LisDeliveryNote/Get", ApiConsumer.ApiConsumers.LisConsumer, filter, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("__apiResult ", apiResult));
                    if (apiResult != null)
                    {
                         var dataResult = (List<LIS_DELIVERY_NOTE>)apiResult.Data;
                         if (dataResult != null)
                         {
                             data = new List<DeliveryNoteADO>();
                             foreach (var item in dataResult)
                             {
                                 data.Add(new DeliveryNoteADO(item));
                             }
                         }
                    }
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DELIVERY_NOTE_CODE", "Mã phiếu", 40, 1));
                columnInfos.Add(new ColumnInfo("NOTE", "Ghi chú", 150, 2));
                columnInfos.Add(new ColumnInfo("RECEIVING_TIME_ForDisplay", "Thời gian nhận", 260, 3));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DELIVERY_NOTE_CODE", "ID", columnInfos, true, 450);
                ControlEditorLoader.Load(cboDeliveryNote, data, controlEditorADO);

                FillDataToGridControl_Samples2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl_Samples2()
        {
            try
            {
                WaitingManager.Show();
                this.ListDataSample2 = null;
                if (cboDeliveryNote.EditValue != null)
                {
                    CommonParam param = new CommonParam();
                    List<long> listSampleSttId = new List<long>();
                    listSampleSttId.Add(2);
                    listSampleSttId.Add(5);
                    LisSampleFilter filter = new LisSampleFilter();
                    filter.SAMPLE_STT_IDs = listSampleSttId;
                    filter.HAS_AGGREGATE_SAMPLE = false;
                    filter.DELIVERY_NOTE_ID = (long)(cboDeliveryNote.EditValue ?? 0);

                    this.ListDataSample2 = new BackendAdapter(param).Get<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumer.ApiConsumers.LisConsumer, filter, SessionManager.ActionLostToken, param);

                }
                gridControlSamples2.DataSource = null;
                gridControlSamples2.DataSource = this.ListDataSample2;
                gridControlSamples2.RefreshDataSource();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMenu()
        {
            try
            {
                if (menu == null)
                    menu = new PopupMenu(barManager1);
                // Add item and show
                menu.ItemLinks.Clear();

                BarButtonItem itemSetParent = new BarButtonItem(barManager1, "Gộp mẫu", 1);
                itemSetParent.Tag = ProcessType.GOP_MAU;
                itemSetParent.ItemClick += new ItemClickEventHandler(setExecuteProcessMenu);
                menu.AddItems(new BarButtonItem[] { itemSetParent });

                menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setExecuteProcessMenu(object sender, ItemClickEventArgs e)
        {
            try
            {
                List<LIS_SAMPLE> sampleSelecteds = GetSampleSelected_Samples2();
                if (sampleSelecteds == null || sampleSelecteds.Count == 0)
                    throw new Exception("Khong co mau nao duoc chon!");

                var btn = e.Item as BarButtonItem;
                ProcessType processType = (ProcessType)btn.Tag;
                ProcessInitDataAndExecuteMenu(sampleSelecteds, processType);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<LIS_SAMPLE> GetSampleSelected_Samples2()
        {
            List<LIS_SAMPLE> result = new List<LIS_SAMPLE>();
            try
            {
                int[] selectRows = gridViewSamples2.GetSelectedRows();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    for (int i = 0; i < selectRows.Count(); i++)
                    {
                        var sample = (LIS_SAMPLE)gridViewSamples2.GetRow(selectRows[i]);
                        result.Add(sample);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void ProcessInitDataAndExecuteMenu(List<LIS_SAMPLE> sampleSelecteds, ProcessType processType)
        {
            try
            {
                if (processType == ProcessType.GOP_MAU)
                {
                    Process_GOP_MAU(sampleSelecteds);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void Process_GOP_MAU(List<LIS_SAMPLE> sampleSelecteds)
        {
            try
            {
                frmBarcodeSampleAggregation frmBarcodeSampleAggregation = new frmBarcodeSampleAggregation(sampleSelecteds, ReLoadProcess_GOP_MAU);
                frmBarcodeSampleAggregation.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReLoadProcess_GOP_MAU()
        {
            try
            {
                FillDataToGridControl_Samples2();
                FillDataToGridControl();

                gridViewSamples2.Focus();
                gridViewSamples2.FocusedRowHandle = 0;
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSamples2_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hitInfo = e.HitInfo;
                if (hitInfo.InRowCell)
                {
                    int visibleRowHandle = this.gridViewSamples2.GetVisibleRowHandle(hitInfo.RowHandle);
                    int[] selectedRows = this.gridViewSamples2.GetSelectedRows();
                    if (selectedRows != null && selectedRows.Length > 0 && selectedRows.Contains(visibleRowHandle))
                    {

                        InitMenu();

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            try
            {
                if (toggleSwitch1.IsOn)
                {
                    lciToggleSwitch1.Text = "Chọn mẫu theo mã vạch";
                    navigationFrame1.SelectedPage = navigationPage2;
                }
                else
                {
                    lciToggleSwitch1.Text = "Chọn mẫu từ phiếu giao nhận";
                    navigationFrame1.SelectedPage = navigationPage1;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstan.toggleSwitch1 && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (toggleSwitch1.IsOn ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstan.toggleSwitch1;
                    csAddOrUpdate.VALUE = (toggleSwitch1.IsOn ? "1" : "");
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
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampleRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                InitComboDeliveryNote();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDeliveryNote_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridControl_Samples2();
                gridViewSamples2.Focus();
                gridViewSamples2.FocusedRowHandle = 0;
                SendKeys.Send("{TAB}");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSamples2_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    List<LIS_SAMPLE> sampleSelecteds = GetSampleSelected_Samples2();
                    if (sampleSelecteds != null && sampleSelecteds.Count > 0)
                    {
                        Process_GOP_MAU(sampleSelecteds);
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    int focusedRowHandle = gridViewSamples2.FocusedRowHandle;

                    if (gridViewSamples2.IsRowSelected(focusedRowHandle))
                        gridViewSamples2.UnselectRow(focusedRowHandle);
                    else
                        gridViewSamples2.SelectRow(focusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSamples2_GotFocus(object sender, EventArgs e)
        {
            try
            {
                gridViewSamples2.FocusedRowHandle = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
