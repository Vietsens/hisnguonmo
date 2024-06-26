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
using LIS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using HIS.Desktop.Print;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.ExecuteRoomPartialKXN
{
    public partial class UCExecuteRoomPartialKXN : UserControl
    {
        #region Declare

        Inventec.Desktop.Common.Modules.Module currentModule;
        internal Inventec.Core.ApiResultObject<List<LIS_SAMPLE>> apiResult;
        internal List<LIS_SAMPLE> currentSample { get; set; }
        internal LIS_SAMPLE rowSample { get; set; }
        internal LIS_SAMPLE samplePrint { get; set; }
        int lastRowHandle = -1;
        internal V_LIS_RESULT currentLisResult { get; set; }
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;

        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;

        #endregion

        #region Contructor

        public UCExecuteRoomPartialKXN()
        {
            InitializeComponent();
        }
        public UCExecuteRoomPartialKXN(Inventec.Desktop.Common.Modules.Module currentModule)
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
        private void UCExecuteRoomPartialKXN_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadDataToCombo();
                LoadDefaultData();
                this.gridControlExecute.ToolTipController = this.toolTipController1;
                FillDataToGridControl();

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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExecuteRoomPartialKXN.Resources.Lang", typeof(HIS.Desktop.Plugins.ExecuteRoomPartialKXN.UCExecuteRoomPartialKXN).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFind.Properties.NullText = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.cboFind.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barbtnSearch.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.barbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Method

        private void LoadDataToCombo()
        {
            try
            {
                List<HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO> status = new List<HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO>();

                status.Add(new HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO(0, Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.TatCa", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                status.Add(new HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO(StatusSampleCFG.SAMPLE_STT_ID__UNSPECIMEN, Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.ChuaLayMau", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                status.Add(new HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO(StatusSampleCFG.SAMPLE_STT_ID__SPECIMEN, Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.DaLayMau", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                status.Add(new HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO(StatusSampleCFG.SAMPLE_STT_ID__RESULT, Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.CoKetQua", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                status.Add(new HIS.Desktop.Plugins.ExecuteRoomPartialKXN.ComboADO(StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT, Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.TraKetQua", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.TrangThai", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()), 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboFind, status, controlEditorADO);

                cboFind.EditValue = status[0].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDefaultData()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
                dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                //cboFind.EditValue = null;
                gridControlExecute.DataSource = null;
                gridControlLisResult.DataSource = null;
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
                SetDefaultData();
                FillDataToGridControl();
                gridControlLisResult.DataSource = null;
                gridControlExecute.DataSource = null;
                gridControlExecuteService.DataSource = null;
                //todo
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetDefaultData()
        {
            try
            {
                dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                txtSearchKey.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FillDataToGridControl()
        {
            FillDataToGridSample(new CommonParam(0, (int)ConfigApplications.NumPageSize));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridSample, param);
        }

        internal void FillDataToGridSample(object param)
        {
            try
            {
                //transitionManager1.StartTransition(layoutControl2);
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                gridControlLisResult.DataSource = null;
                gridControlExecuteService.DataSource = null;

                LIS.Filter.LisSampleFilter lisSampleFilter = new LIS.Filter.LisSampleFilter();

                var roomCode = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == this.currentModule.RoomId);
                if (roomCode != null)
                {
                    lisSampleFilter.EXECUTE_ROOM_CODE__EXACT = roomCode.ROOM_CODE;
                }
                lisSampleFilter.KEY_WORD = txtSearchKey.Text.Trim();
                if (dtCreatefrom != null && dtCreatefrom.DateTime != DateTime.MinValue)
                    lisSampleFilter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreatefrom.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (dtCreateTo != null && dtCreateTo.DateTime != DateTime.MinValue)
                    lisSampleFilter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMddHHmm") + "59");
                lisSampleFilter.ORDER_FIELD = "CREATE_TIME";
                lisSampleFilter.ORDER_DIRECTION = "DESC";

                List<long> lstTestServiceReqSTT = new List<long>();

                //Tất cả 0
                //Chưa lấy mẫu 1
                //Đã lấy mẫu 2
                //Đã có kết quả
                //Đã trả kết quả
                //Filter yeu cau chua lấy mẫu
                if (cboFind.EditValue != null)
                {
                    //Chưa lấy mẫu
                    if ((long)cboFind.EditValue == 1)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__UNSPECIMEN;
                    }
                    //Đã lấy mẫu
                    else if ((long)cboFind.EditValue == 2)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__SPECIMEN;
                    }
                    //đã có kết quả
                    else if ((long)cboFind.EditValue == 3)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__RESULT;
                    }//đã trả kết quả
                    else if ((long)cboFind.EditValue == 4)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT;
                    }
                    //Tất cả
                    else
                    {
                        lisSampleFilter.SAMPLE_STT_ID = null;
                    }
                }
                apiResult = new ApiResultObject<List<LIS_SAMPLE>>();

                apiResult = new BackendAdapter(paramCommon).GetRO<List<LIS_SAMPLE>>("api/LisSample/Get", ApiConsumer.ApiConsumers.LisConsumer, lisSampleFilter, paramCommon);
                gridControlExecute.DataSource = null;

                if (apiResult != null)
                {
                    WaitingManager.Hide();
                    var data = (List<LIS_SAMPLE>)apiResult.Data;
                    if (data != null)
                    {
                        gridControlExecute.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDataToGridSampleService(LIS_SAMPLE row)
        {
            try
            {
                CommonParam param = new CommonParam();
                LIS.Filter.LisSampleServiceFilter sampleServiceFilter = new LIS.Filter.LisSampleServiceFilter();
                if (rowSample != null)
                {
                    sampleServiceFilter.SAMPLE_ID = rowSample.ID;

                    var currentLisSampleService = new BackendAdapter(param).Get<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/Get", ApiConsumer.ApiConsumers.LisConsumer, sampleServiceFilter, param);
                    if (currentLisSampleService != null)
                    {
                        gridControlExecuteService.DataSource = currentLisSampleService;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void btnFind_Click(object sender, EventArgs e)
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

        private void gridViewSample_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LIS.EFMODEL.DataModels.LIS_SAMPLE data = (LIS.EFMODEL.DataModels.LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "SAMPLE_STT")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == StatusSampleCFG.SAMPLE_STT_ID__UNSPECIMEN)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == StatusSampleCFG.SAMPLE_STT_ID__SPECIMEN)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == StatusSampleCFG.SAMPLE_STT_ID__RESULT)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else if (statusId == StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT)
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //CommonParam param= new CommonParam();

                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    //LIS.Filter.LisResultViewFilter lisResultFilter = new LIS.Filter.LisResultViewFilter();
                    //lisResultFilter.SAMPLE_ID= ((LIS_SAMPLE)gridViewExecute.GetRow(e.RowHandle)).ID;
                    var sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewExecute.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID")).ToString());
                    //var hasResult = new BackendAdapter(param).Get<List<LIS_RESULT>>("api/LisResult/Get", ApiConsumer.ApiConsumers.LisConsumer, lisResultFilter, param).Count;
                    if (e.Column.FieldName == "HUY_KQ")
                    {
                        if (sampleStt == 1 || sampleStt == 2)
                        {
                            e.RepositoryItem = HuyKetQuaD;
                        }
                        else if (sampleStt == 3)
                        {
                            e.RepositoryItem = HuyKetQuaE;
                        }
                        else
                        {
                            e.RepositoryItem = HuyKetQuaE;
                        }
                    }
                    if (e.Column.FieldName == "TRA_KQ")
                    {
                        if (sampleStt == 1 || sampleStt == 2)
                        {
                            e.RepositoryItem = TraKetQuaD;
                        }
                        else if (sampleStt == 3)
                        {
                            e.RepositoryItem = TraKetQuaE;
                        }
                        else if (sampleStt == 4)
                        {
                            e.RepositoryItem = HuyTraE;
                        }
                    }

                    if (e.Column.FieldName == "DUYET")
                    {
                        if (sampleStt == 1)
                        {
                            e.RepositoryItem = DuyetE;
                        }
                        else
                        {
                            if (sampleStt == 3 || sampleStt == 4)
                                e.RepositoryItem = HuyDuyetD;
                            else
                                e.RepositoryItem = HuyDuyetE;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewExecute_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                
                rowSample = new LIS.EFMODEL.DataModels.LIS_SAMPLE();
                rowSample = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                if (rowSample != null)
                {
                    LoadDataToGridSampleService(rowSample);
                    //
                    //
                    //
                    LIS.Filter.LisResultViewFilter lisResultFilter = new LIS.Filter.LisResultViewFilter();
                    lisResultFilter.SAMPLE_ID = rowSample.ID;
                    var currentLisResult = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("/api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, lisResultFilter, param);
                    if (currentLisResult != null)
                    {
                        gridControlLisResult.DataSource = currentLisResult;
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__DeleteKQ_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                try
                {
                    //WaitingManager.Show();
                    bool result = false;
                    CommonParam param = new CommonParam();
                    var row = (LIS.EFMODEL.DataModels.V_LIS_RESULT)gridViewLisResult.GetFocusedRow();
                    if (row != null)
                    {
                        result = new BackendAdapter(param).Post<bool>("api/LisResult/Delete", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
                        if (result == true)
                        {
                            gridControlLisResult.BeginUpdate();
                            gridViewLisResult.DeleteRow(gridViewLisResult.FocusedRowHandle);
                            gridViewLisResult.RefreshData();
                            gridControlLisResult.RefreshDataSource();
                            gridControlLisResult.EndUpdate();
                            FillDataToGridControl();
                            // WaitingManager.Hide();
                        }
                    }
                    //WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, result);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
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

        #region UpdateStt
        private void UpdateStt(long sampleSTT)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                LIS.SDO.UpdateSampleSttSDO updateStt = new LIS.SDO.UpdateSampleSttSDO();
                var row = (LIS.EFMODEL.DataModels.LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                if (row != null)
                {
                    updateStt.Id = row.ID;
                    updateStt.SampleSttId = sampleSTT;

                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/UpdateStt", ApiConsumer.ApiConsumers.LisConsumer, updateStt, param);
                    if (curentSTT != null)
                    {
                        FillDataToGridControl();
                        result = true;
                        FillDataToGridControl();
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void repositoryItemButton__HuyKQ_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                if (row != null)
                {
                    result = new BackendAdapter(param).Post<bool>("api/LisSample/Delete", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
                    if (result == true)
                    {
                        gridControlExecute.BeginUpdate();
                        gridViewExecute.DeleteRow(gridViewExecute.FocusedRowHandle);
                        gridViewExecute.RefreshData();
                        gridControlExecute.RefreshDataSource();
                        gridControlExecute.EndUpdate();
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__TraKQ_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                row.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT;
                if (row != null)
                {
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/ReturnResult", ApiConsumer.ApiConsumers.LisConsumer, row.ID, param);
                    if (curentSTT != null)
                    {
                        WaitingManager.Hide();
                        FillDataToGridControl();
                        result = true;
                        //gridViewExecute.RefreshData();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void repositoryItemButton__HuyTraKQ_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                UpdateStt(StatusSampleCFG.SAMPLE_STT_ID__RESULT);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlExecute)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlExecute.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "SAMPLE_STT")
                            {
                                //text = (view.GetRowCellValue(lastRowHandle, "SAMPLE_STT_NAME") ?? "").ToString();
                                var busyCount = ((LIS_SAMPLE)view.GetRow(lastRowHandle)).SAMPLE_STT_ID;
                                if (busyCount == StatusSampleCFG.SAMPLE_STT_ID__UNSPECIMEN)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.ChuaLayMau", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (busyCount == StatusSampleCFG.SAMPLE_STT_ID__SPECIMEN)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.DaLayMau", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (busyCount == StatusSampleCFG.SAMPLE_STT_ID__RESULT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.CoKetQua", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                }
                                else if (busyCount == StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCExecuteRoomPartialKXN.TraKetQua", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void repositoryItemTextEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if ((e.KeyCode == Keys.Return))
                {
                    WaitingManager.Show();
                    bool result = false;
                    CommonParam param = new CommonParam();
                    var currentListResultNew = (V_LIS_RESULT)gridViewLisResult.GetFocusedRow();
                    currentListResultNew.VALUE = (sender as TextEdit).Text;

                    var curent = new BackendAdapter(param).Post<LIS_RESULT>("api/LisResult/Update", ApiConsumer.ApiConsumers.LisConsumer, currentListResultNew, param);
                    if (curent != null)
                    {
                        result = true;
                        gridViewLisResult.RefreshData();
                        rowSample.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__RESULT;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, rowSample, param);
                        if (curentSTT != null)
                        {
                            FillDataToGridControl();
                        }
                        WaitingManager.Hide();
                    }
                    // MessageBox.Show((sender as TextEdit).Text);

                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, result);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
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

        private void repositoryItemButton__Print_Click_1(object sender, EventArgs e)
        {
            try
            {
                samplePrint = new LIS.EFMODEL.DataModels.LIS_SAMPLE();
                samplePrint = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                if (samplePrint != null)
                {
                    PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSampleService_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    LIS_SAMPLE_SERVICE data = (LIS_SAMPLE_SERVICE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCreatefrom_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dtCreatefrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

        }

        private void dtCreateTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

        }

        private void dtCreateTo_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void txtSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                row.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__SPECIMEN;
                if (row != null)
                {
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, row, param);
                    if (curentSTT != null)
                    {
                        WaitingManager.Hide();
                        FillDataToGridControl();
                        result = true;
                        gridViewExecute.RefreshData();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void HuyDuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                row.SAMPLE_STT_ID = StatusSampleCFG.SAMPLE_STT_ID__UNSPECIMEN;
                if (row != null)
                {
                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumer.ApiConsumers.LisConsumer, row, param);
                    if (curentSTT != null)
                    {
                        WaitingManager.Hide();
                        FillDataToGridControl();
                        result = true;
                        gridViewExecute.RefreshData();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void gridViewLisResult_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                 GridView View = sender as GridView;
                 if (e.RowHandle >= 0)
                 {
                     if (e.Column.FieldName == "Delete")
                     {
                         if (rowSample.SAMPLE_STT_ID == StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT)
                         {
                             e.RepositoryItem = DeleteTestIndexD;
                         }
                     }
                     else if (e.Column.FieldName == "VALUE")
                     {
                         if (rowSample.SAMPLE_STT_ID == StatusSampleCFG.SAMPLE_STT_ID__RETURN_RESULT)
                         {
                             e.RepositoryItem = repositoryItemTextEdit1D;
                         }
                     }
                 }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Print

        internal enum PrintTypeKXN
        {
            IN_KET_QUA_XET_NGHIEM,
        }

        void PrintProcess(PrintTypeKXN printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeKXN.IN_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_KET_QUA_XET_NGHIEM__MPS000096, DelegateRunPrinterKXN);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterKXN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__IN_KET_QUA_XET_NGHIEM__MPS000096:
                        LoadBieuMauInKetQuaXetNghiem(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauInKetQuaXetNghiem(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LIS.Filter.LisResultViewFilter lisResultFilter = new LIS.Filter.LisResultViewFilter();
                lisResultFilter.SAMPLE_ID = samplePrint.ID;

                var currentLisResult = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, lisResultFilter, param);

                List<V_HIS_SERVICE_REQ> currentServiceReqs = new List<V_HIS_SERVICE_REQ>();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();
                serviceReqFilter.SERVICE_REQ_CODE = samplePrint.SERVICE_REQ_CODE;

                var currentServiceReq = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                //MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                //patientFilter.PATIENT_CODE = samplePrint.PATIENT_CODE;
                //KXN.LOGIC.HisPatient.HisPatientLogic patientLogic = new LOGIC.HisPatient.HisPatientLogic();
                //var currentPatient = patientLogic.Get<List<V_HIS_PATIENT>>(patientFilter).FirstOrDefault();

                MOS.Filter.HisPatientTypeAlterViewFilter hisPTAlterFilter = new HisPatientTypeAlterViewFilter();
                hisPTAlterFilter.TREATMENT_ID = currentServiceReq.TREATMENT_ID;
                var hisPatientTypeAlter = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumer.ApiConsumers.MosConsumer, hisPTAlterFilter, param).FirstOrDefault();

                HisPatyAlterBhytViewFilter patyAlterBhytFilter = new HisPatyAlterBhytViewFilter();
                if (hisPatientTypeAlter != null)
                    patyAlterBhytFilter.PATIENT_TYPE_ALTER_ID = hisPatientTypeAlter.ID;

                var hisPatyAlterBhyt = new BackendAdapter(param).Get<List<V_HIS_PATY_ALTER_BHYT>>("api/HisPatyAlterBhyt/GetView", ApiConsumer.ApiConsumers.MosConsumer, patyAlterBhytFilter, param).FirstOrDefault();

                MPS.Core.Mps000096.Mps000096RDO mps000096RDO = new MPS.Core.Mps000096.Mps000096RDO(
                    hisPatientTypeAlter,
                    hisPatyAlterBhyt,
                    currentServiceReq,
                samplePrint,
                currentLisResult
                    );
                WaitingManager.Hide();

                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000096RDO, MPS.Printer.PreviewType.PrintNow);
                }
                else
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000096RDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        #endregion

        #region Print barcode

        private void onClickBtnPrintBarCode()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.IN_BARCODE;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal enum PrintType
        {
            IN_BARCODE,
        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BARCODE:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_BAR_CODE__MPS000077, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_BAR_CODE__MPS000077:
                        LoadBieuMauPhieuYCInBarCode(printTypeCode, fileName, ref result);
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

        internal void LoadBieuMauPhieuYCInBarCode(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                var currentBarCode = (LIS_SAMPLE)gridViewExecute.GetFocusedRow();
                MPS.Core.Mps000077.Mps000077RDO mps000077RDO = new MPS.Core.Mps000077.Mps000077RDO(
                    currentBarCode
                    );
                WaitingManager.Hide();
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000077RDO, MPS.Printer.PreviewType.PrintNow);
                }
                else
                {
                    result = MPS.Printer.Run(printTypeCode, fileName, mps000077RDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        #endregion


    }
}
