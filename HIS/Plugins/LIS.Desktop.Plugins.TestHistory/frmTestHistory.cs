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
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.Desktop.Plugins.TestHistory.ADO;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LIS.Desktop.Plugins.TestHistory
{
    public partial class frmTestHistory : FormBase
    {
        private int start = 0;
        private int limit = 0;
        private int rowCount = 0;
        private int totalData = 0;

        private string PatientCode;

        private int lastRowHandle = -1;
        private GridColumn lastColumn = null;
        private ToolTipControlInfo lastInfo = null;
        private NumberStyles style = NumberStyles.Any;

        private List<V_HIS_TEST_INDEX_RANGE> testIndexRangeAll = null;

        private V_LIS_SAMPLE currentSample = null;

        public frmTestHistory(Inventec.Desktop.Common.Modules.Module module, string patientCode)
            : base(module)
        {
            InitializeComponent();
            this.PatientCode = patientCode;
        }

        private void frmTestHistory_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                this.SetDefaultValue();
                this.PatientCode = "";
                gridControlSample.ToolTipController = this.toolTipController1;
                this.FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtPatientCode.Text = this.PatientCode ?? "";
                txtTreatmentCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            int numPageSize;
            if (ucPaging1.pagingGrid != null)
            {
                numPageSize = ucPaging1.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            LoadPaging(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = this.rowCount;
            param.Count = this.totalData;
            ucPaging1.Init(LoadPaging, param, numPageSize);
        }

        private void LoadPaging(object param)
        {
            try
            {
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                this.rowCount = 0;
                this.totalData = 0;
                CommonParam paramCommon = new CommonParam(this.start, this.limit);
                List<V_LIS_SAMPLE> listData = new List<V_LIS_SAMPLE>();

                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text) || !String.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    LisSampleViewFilter filter = new LisSampleViewFilter();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "INTRUCTION_TIME";
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12)
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        }
                        filter.TREATMENT_CODE__EXACT = code;
                        txtTreatmentCode.Text = code;
                    }
                    else if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                    {
                        string code = txtPatientCode.Text.Trim();
                        if (code.Length < 10)
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        }
                        filter.PATIENT_CODE__EXACT = code;
                        txtPatientCode.Text = code;
                    }
                    var rs = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, filter, paramCommon);
                    if (rs != null)
                    {
                        if (rs.Data != null)
                        {
                            listData = rs.Data;
                        }
                        this.rowCount = (listData == null ? 0 : listData.Count);
                        this.totalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    }
                }

                gridControlSample.BeginUpdate();
                gridControlSample.DataSource = listData;
                gridControlSample.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
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
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                this.FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefresh.Enabled) return;
                WaitingManager.Show();
                this.SetDefaultValue();
                this.FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_LIS_SAMPLE data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.start;
                        }
                        else if (e.Column.FieldName == "IMAGE_STATUS")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            {
                                e.Value = imageListIcon.Images[5];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                            {
                                e.Value = imageListIcon.Images[11];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                            {
                                e.Value = imageListIcon.Images[10];
                            }
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                        }
                        else if (e.Column.FieldName == "GENDER_NAME")
                        {
                            e.Value = data.GENDER_CODE == "01" ? "Nữ" : "Nam";
                        }
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
                if (e.Info == null && e.SelectedControl == gridControlSample)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSample.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "IMAGE_STATUS")
                            {
                                //text = (view.GetRowCellValue(lastRowHandle, "SAMPLE_STT_NAME") ?? "").ToString();
                                var busyCount = ((V_LIS_SAMPLE)view.GetRow(lastRowHandle)).SAMPLE_STT_ID;
                                if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                                {
                                    text = "Chưa lấy mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                                {
                                    text = "Đã lấy mẫu";
                                }

                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                                {
                                    text = "Có kết quả";
                                }

                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                                {
                                    text = "Đã trả kết quả";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                                {
                                    text = "Từ chối mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                                {
                                    text = "Chấp nhận mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                                {
                                    text = "Duyệt kết quả";
                                }
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentSample = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                this.LoadDataToGridTestResult();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewResult_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {

        }

        private void gridViewResult_RowStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowStyleEventArgs e)
        {
            try
            {
                Decimal valueRange;
                SampleLisResultADO data = (SampleLisResultADO)gridViewResult.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IS_NO_EXECUTE != null)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                    }

                    if (Decimal.TryParse(data.VALUE_RANGE, out valueRange))
                    {
                        if (data.MIN_VALUE != null && valueRange < data.MIN_VALUE)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                            e.HighPriority = true;
                        }
                        else if (data.MAX_VALUE != null && valueRange > data.MAX_VALUE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.HighPriority = true;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewResult_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                SampleLisResultADO data = (SampleLisResultADO)gridViewResult.GetRow(e.RowHandle);
                if (data != null)
                {

                    if (data.IS_PARENT == 1 || data.HAS_ONE_CHILD == 1)
                        e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridTestResult()
        {
            try
            {
                CommonParam param = new CommonParam();
                List<V_LIS_RESULT> listResult = new List<V_LIS_RESULT>();

                List<SampleLisResultADO> lstHisSereServTeinSDO = new List<SampleLisResultADO>();
                if (this.currentSample != null)
                {
                    LisResultViewFilter rsFilter = new LisResultViewFilter();
                    rsFilter.SAMPLE_ID = this.currentSample.ID;
                    listResult = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumers.LisConsumer, rsFilter, param);
                    List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = null;
                    List<V_HIS_TEST_INDEX> currentTestIndexs = null;
                    var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    if (listResult != null && listResult.Count > 0)
                    {
                        listResult = listResult.OrderBy(o => o.SERVICE_NUM_ORDER).ToList();

                        currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                        var serviceCodes = listResult.Select(o => o.SERVICE_CODE).Distinct().ToList();
                        currentTestIndexs = testIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                        if (currentTestIndexs != null && currentTestIndexs.Count > 0 && testIndex != null && testIndex.Count > 0)
                        {
                            var testIndexCodes = currentTestIndexs.Select(o => o.TEST_INDEX_CODE).Distinct().ToList();
                            testIndexRanges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>().Where(o => testIndexCodes.Contains(o.TEST_INDEX_CODE)).ToList();
                        }
                    }
                    long genderId = LoadGenderId(this.currentSample);
                    if (currentTestIndexs != null && currentTestIndexs.Count > 0)
                    {
                        var groupListResult = listResult.GroupBy(o => o.SERVICE_CODE).ToList();

                        foreach (var group in groupListResult)
                        {
                            SampleLisResultADO hisSereServTeinSDO = new SampleLisResultADO();
                            var fistGroup = group.FirstOrDefault();
                            hisSereServTeinSDO.IS_PARENT = 1;
                            hisSereServTeinSDO.TEST_INDEX_CODE = fistGroup != null ? fistGroup.SERVICE_CODE : "";
                            hisSereServTeinSDO.TEST_INDEX_NAME = fistGroup != null ? fistGroup.SERVICE_NAME : "";
                            hisSereServTeinSDO.SERVICE_CODE = fistGroup != null ? fistGroup.SERVICE_CODE : "";
                            hisSereServTeinSDO.SERVICE_NAME = fistGroup != null ? fistGroup.SERVICE_NAME : "";
                            hisSereServTeinSDO.ID = fistGroup.ID;
                            hisSereServTeinSDO.IS_NO_EXECUTE = fistGroup.IS_NO_EXECUTE;
                            hisSereServTeinSDO.PARENT_ID = ".";
                            hisSereServTeinSDO.MODIFIER = "";
                            hisSereServTeinSDO.CHILD_ID = fistGroup.ID + ".";
                            hisSereServTeinSDO.SERVICE_NUM_ORDER = fistGroup.SERVICE_NUM_ORDER;
                            hisSereServTeinSDO.NUM_ORDER = 999999;
                            //Lay machine_id
                            var lstResultItem = group.ToList();
                            lstResultItem = lstResultItem.OrderBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                            if (listResult != null
                                && listResult.Count > 0
                                && lstResultItem != null
                                && lstResultItem.Count > 0)
                            {
                                var machineByLisResult = listResult.FirstOrDefault(p => p.SERVICE_CODE == hisSereServTeinSDO.SERVICE_CODE);
                                if (machineByLisResult != null)
                                {
                                    hisSereServTeinSDO.MACHINE_ID = machineByLisResult.MACHINE_ID;
                                    hisSereServTeinSDO.MACHINE_ID_OLD = machineByLisResult.MACHINE_ID;
                                }
                            }
                            var testIndFist = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == lstResultItem[0].TEST_INDEX_CODE);

                            if (lstResultItem != null
                                && lstResultItem.Count == 1
                                && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE == 1)
                            {

                                hisSereServTeinSDO.HAS_ONE_CHILD = 1;
                                hisSereServTeinSDO.CHILD_ID = lstResultItem[0].ID + "." + lstResultItem[0].ID;
                                hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                                hisSereServTeinSDO.TEST_INDEX_CODE = "       " + lstResultItem[0].TEST_INDEX_CODE;
                                hisSereServTeinSDO.TEST_INDEX_NAME = lstResultItem[0].TEST_INDEX_NAME;
                                hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = testIndFist.TEST_INDEX_UNIT_NAME;
                                hisSereServTeinSDO.IS_IMPORTANT = testIndFist.IS_IMPORTANT;
                                hisSereServTeinSDO.SAMPLE_SERVICE_ID = lstResultItem[0].SAMPLE_SERVICE_ID;
                                hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = lstResultItem[0].SAMPLE_SERVICE_STT_ID;
                                hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                                hisSereServTeinSDO.VALUE_RANGE = lstResultItem[0].VALUE;
                                hisSereServTeinSDO.LIS_RESULT_ID = lstResultItem[0].ID;
                                hisSereServTeinSDO.ID = lstResultItem[0].ID;
                                hisSereServTeinSDO.SAMPLE_ID = lstResultItem[0].SAMPLE_ID;
                                hisSereServTeinSDO.SAMPLE_SERVICE_ID = lstResultItem[0].SAMPLE_SERVICE_ID;
                                hisSereServTeinSDO.SAMPLE_SERVICE_STT_CODE = lstResultItem[0].SAMPLE_SERVICE_STT_CODE;
                                hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = lstResultItem[0].SAMPLE_SERVICE_STT_ID;
                                hisSereServTeinSDO.SAMPLE_SERVICE_STT_NAME = lstResultItem[0].SAMPLE_SERVICE_STT_NAME;
                                hisSereServTeinSDO.MACHINE_ID_OLD = lstResultItem[0].MACHINE_ID;
                                hisSereServTeinSDO.MACHINE_ID = lstResultItem[0].MACHINE_ID;
                                hisSereServTeinSDO.NOTE = lstResultItem[0].DESCRIPTION;
                                hisSereServTeinSDO.SERVICE_NUM_ORDER = lstResultItem[0].SERVICE_NUM_ORDER;
                                hisSereServTeinSDO.OLD_VALUE = lstResultItem[0].OLD_VALUE;
                            }
                            lstHisSereServTeinSDO.Add(hisSereServTeinSDO);

                            if (lstResultItem != null
                                && (lstResultItem.Count > 1
                                || (lstResultItem.Count == 1
                                && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE != 1))
                                )
                            {
                                foreach (var ssTein in lstResultItem)
                                {
                                    var testIndChild = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == ssTein.TEST_INDEX_CODE);
                                    SampleLisResultADO hisSereServTein = new SampleLisResultADO();
                                    hisSereServTein.HAS_ONE_CHILD = 0;
                                    Inventec.Common.Mapper.DataObjectMapper.Map<SampleLisResultADO>(hisSereServTein, ssTein);
                                    hisSereServTein.IS_PARENT = 0;

                                    if (testIndChild != null)
                                    {
                                        hisSereServTein.IS_IMPORTANT = testIndChild.IS_IMPORTANT;
                                        hisSereServTein.TEST_INDEX_UNIT_NAME = testIndChild.TEST_INDEX_UNIT_NAME;
                                        hisSereServTein.NUM_ORDER = testIndChild.NUM_ORDER;
                                    }
                                    else
                                    {
                                        hisSereServTein.NUM_ORDER = null;
                                    }
                                    hisSereServTein.CHILD_ID = ssTein.ID + "." + ssTein.ID;
                                    hisSereServTein.ID = ssTein.ID;
                                    hisSereServTein.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                                    hisSereServTein.TEST_INDEX_CODE = "       " + ssTein.TEST_INDEX_CODE;
                                    hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                    hisSereServTein.MODIFIER = "";
                                    hisSereServTeinSDO.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                    hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                    hisSereServTein.MODIFIER = ssTein.MODIFIER;
                                    hisSereServTein.VALUE_RANGE = ssTein.VALUE;
                                    hisSereServTein.LIS_RESULT_ID = ssTein.ID;
                                    hisSereServTein.MACHINE_ID = ssTein.MACHINE_ID;
                                    hisSereServTein.MACHINE_ID_OLD = ssTein.MACHINE_ID;
                                    hisSereServTein.SAMPLE_ID = ssTein.SAMPLE_ID;
                                    hisSereServTein.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                    hisSereServTein.SAMPLE_SERVICE_STT_CODE = ssTein.SAMPLE_SERVICE_STT_CODE;
                                    hisSereServTein.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                    hisSereServTein.SAMPLE_SERVICE_STT_NAME = ssTein.SAMPLE_SERVICE_STT_NAME;
                                    hisSereServTein.SERVICE_NUM_ORDER = ssTein.SERVICE_NUM_ORDER;
                                    hisSereServTein.OLD_VALUE = ssTein.OLD_VALUE;
                                    hisSereServTein.DESCRIPTION = "";

                                    hisSereServTein.NOTE = ssTein.DESCRIPTION;
                                    lstHisSereServTeinSDO.Add(hisSereServTein);
                                }
                            }
                        }
                    }
                    // gán test index range
                    if (lstHisSereServTeinSDO != null && lstHisSereServTeinSDO.Count > 0)
                    {

                        foreach (var hisSereServTeinSDO in lstHisSereServTeinSDO)
                        {
                            V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                            testIndexRange = GetTestIndexRange(this.currentSample.DOB ?? 0, genderId, hisSereServTeinSDO.TEST_INDEX_CODE.Trim(), ref this.testIndexRangeAll);
                            if (testIndexRange != null)
                            {
                                ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                            }
                        }
                    }

                    if (lstHisSereServTeinSDO != null && lstHisSereServTeinSDO.Count() > 0)
                    {
                        lstHisSereServTeinSDO = lstHisSereServTeinSDO.OrderBy(o => o.SERVICE_NUM_ORDER)
                        .ThenByDescending(p => p.NUM_ORDER).ToList();
                    }
                }

                gridControlResult.BeginUpdate();
                gridControlResult.DataSource = lstHisSereServTeinSDO;
                gridControlResult.EndUpdate();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long LoadGenderId(V_LIS_SAMPLE sample)
        {
            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (sample != null && !String.IsNullOrWhiteSpace(sample.GENDER_CODE))
                {
                    genderId = sample.GENDER_CODE == "01" ? 1 : 2;
                }
                else if (sample != null && !String.IsNullOrWhiteSpace(sample.PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = sample.PATIENT_CODE;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        genderId = patients.FirstOrDefault().GENDER_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                genderId = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return genderId;
        }

        private V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long genderId, string testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    double age = 0;
                    List<V_HIS_TEST_INDEX_RANGE> query = new List<V_HIS_TEST_INDEX_RANGE>();
                    var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob);
                    foreach (var item in testIndexRanges)
                    {
                        if (item.TEST_INDEX_CODE == testIndexId)
                        {
                            if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__YEAR) // Năm
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 365;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__MONTH) // Tháng
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays / 30;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__DAY) // Ngày
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalDays;
                            }
                            else if (item.AGE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_AGE_TYPE.ID__HOUR) // Giờ
                            {
                                age = (DateTime.Now - (ageDate ?? DateTime.Now)).TotalHours;
                            }
                            Inventec.Common.Logging.LogSystem.Debug(age + "______" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dob), dob));
                        
                            if (((item.AGE_FROM.HasValue && item.AGE_FROM.Value <= age) || !item.AGE_FROM.HasValue)
                                 && ((item.AGE_TO.HasValue && item.AGE_TO.Value >= age) || !item.AGE_TO.HasValue))
                            {
                                query.Add(item);
                            }
                        }
                    }
                    HIS_GENDER gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == genderId);
                    if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1).ToList();
                    }
                    else if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1).ToList();
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        private void ProcessMaxMixValue(SampleLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0;
                if (ti != null && testIndexRange != null)
                {
                    ti.DESCRIPTION = "";
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE))
                    {
                        if (Decimal.TryParse((ti.VALUE_RANGE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                        {
                            ti.VALUE = value;
                        }
                        else
                        {
                            ti.VALUE = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }
                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
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

        private void gridViewResult_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    SampleLisResultADO data = (SampleLisResultADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "VALUE_RANGE" && data != null)
                        {

                            if ((data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0) || data.IS_NO_EXECUTE == 1)
                            {
                                e.RepositoryItem = repositoryItemText__NumBody;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemText_SimpleBody;
                            }
                        }
                        else if (e.Column.FieldName == "OLD_VALUE" && data != null)
                        {

                            if ((data.IS_PARENT == 1 && data.HAS_ONE_CHILD == 0) || data.IS_NO_EXECUTE == 1)
                            {
                                e.RepositoryItem = repositoryItemText__NumBody;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemText_SimpleBody;
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

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void gridViewSample_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                V_LIS_SAMPLE row = (V_LIS_SAMPLE)gridViewSample.GetRow(e.RowHandle);
                if (row != null && e.Column.FieldName == "PRINT")
                {
                    if (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                    {
                        e.RepositoryItem = repositoryItemButtonPrint_E;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemButtonPrint_D;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonPrint_E_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, DelegateRunPrinterKXN);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterKXN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                V_LIS_SAMPLE sample = (V_LIS_SAMPLE)gridViewSample.GetFocusedRow();
                if (sample == null)
                {
                    return false;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                LisResultViewFilter rsFilter = new LisResultViewFilter();
                rsFilter.SAMPLE_ID = sample.ID;
                List<V_LIS_RESULT> listResult = new BackendAdapter(new CommonParam()).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumers.LisConsumer, rsFilter, null);

                HisServiceReqFilter srFilter = new HisServiceReqFilter();
                srFilter.SERVICE_REQ_CODE__EXACT = sample.SERVICE_REQ_CODE;
                HIS_SERVICE_REQ serviceReq = null;
                List<HIS_SERVICE_REQ> sreqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, srFilter, param);
                serviceReq = sreqs != null ? sreqs.FirstOrDefault() : null;
                if (serviceReq == null)
                {
                    LogSystem.Warn("Khong lay duoc ServiceReq" + LogUtil.TraceData("sample", sample));
                    return false;
                }

                HisTreatmentFilter tFilter = new HisTreatmentFilter();
                tFilter.ID = serviceReq.TREATMENT_ID;
                List<HIS_TREATMENT> treats = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, tFilter, param);
                HIS_TREATMENT treatment = treats != null ? treats.FirstOrDefault() : null;
                var currentPatientTypeAlter = new BackendAdapter(param).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, serviceReq.TREATMENT_ID, param);

                List<V_HIS_TEST_INDEX> hisTestIndexs = null;
                if (listResult != null && listResult.Count > 0)
                {
                    List<string> serviceCodes = listResult.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    hisTestIndexs = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                }
                WaitingManager.Hide();

                long genderId = LoadGenderId(sample);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((treatment != null ? treatment.TREATMENT_CODE : serviceReq.TDL_TREATMENT_CODE), printTypeCode, this.currentModuleBase != null ? currentModuleBase.RoomId : 0);

                MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                           currentPatientTypeAlter,
                           treatment,
                           sample,
                           serviceReq,
                           hisTestIndexs,
                           listResult,
                           BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                           genderId,
                           BackendDataWorker.Get<V_HIS_SERVICE>());

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }
                //PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

    }
}
