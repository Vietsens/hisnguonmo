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
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using LIS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Print;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.SampleRoomPartialKXN.Properties;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.SampleRoomPartialKXN
{
    public partial class UCSampleRoomPartialKXN : UserControl
    {
        internal Inventec.Core.ApiResultObject<List<V_HIS_TEST_SERVICE_REQ>> apiResult;
        internal LIS_SAMPLE currentLisSample;
        int start = 0;
        int limit = 0;
        int rowCount = 0;
        int dataTotal = 0;
        Inventec.Desktop.Common.Modules.Module moduleData;

        public UCSampleRoomPartialKXN()
        {
            InitializeComponent();
        }
        public UCSampleRoomPartialKXN(long roomId, long roomTypeId)
        {
            InitializeComponent();
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnCreateSample()
        {
            try
            {
                btnCreateSample_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void UCSampleRoomPartialKXN_Load(object sender, EventArgs e)
        {
            MeShow();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                gridControlTestServiceReq.DataSource=null;
                gridControlBarCode.DataSource = null;
                gridControlSereServ.DataSource = null;
                gridControlSimple.DataSource = null;
                FillDataToGridControl();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestServiceReq_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_TEST_SERVICE_REQ data = (MOS.EFMODEL.DataModels.V_HIS_TEST_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + ((ucPaging1.pagingGrid.CurrentPage - 1) * ucPaging1.pagingGrid.PageSize);
                        }

                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlTestServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                currentLisSample = new LIS_SAMPLE();
                var row = (V_HIS_TEST_SERVICE_REQ)gridViewTestServiceReq.GetFocusedRow();
                currentLisSample.SAMPLE_ROOM_CODE = row.SAMPLE_ROOM_CODE;
                currentLisSample.SAMPLE_ROOM_NAME = row.SAMPLE_ROOM_NAME;
                currentLisSample.SERVICE_REQ_CODE = row.SERVICE_REQ_CODE;
                currentLisSample.EXECUTE_ROOM_CODE = row.EXECUTE_ROOM_CODE;
                currentLisSample.EXECUTE_ROOM_NAME = row.EXECUTE_ROOM_NAME;
                currentLisSample.PATIENT_CODE = row.PATIENT_CODE;
                if (row != null)
                {
                    LoadDataToGridSereServ(row);
                    LoadDataGridSample();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERE_SERV data = (V_HIS_SERE_SERV)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSimple_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
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
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "TRANGTHAI_IMG")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == 1)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == 2)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == 3)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[3];
                            }
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSimple_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSimple.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID")).ToString());
                    if (e.Column.FieldName == "SAMPLE_STT")
                    {
                        if (sampleStt == 1)
                        {
                            e.RepositoryItem = repositoryItemButtonSample_Disable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonSample_Enable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCreateSample_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                if (gridViewSereServ.RowCount > 0)
                {
                    List<V_HIS_SERE_SERV> lstSereServChecks = new List<V_HIS_SERE_SERV>();
                    for (int i = 0; i < gridViewSereServ.SelectedRowsCount; i++)
                    {
                        if (gridViewSereServ.GetSelectedRows()[i] >= 0)
                        {
                            lstSereServChecks.Add((V_HIS_SERE_SERV)gridViewSereServ.GetRow(gridViewSereServ.GetSelectedRows()[i]));
                        }
                    }
                    if (lstSereServChecks != null && lstSereServChecks.Count > 0)
                    {
                        List<LIS_SAMPLE_SERVICE> lstSampleService = new List<LIS_SAMPLE_SERVICE>();
                        foreach (var item in lstSereServChecks)
                        {
                            LIS_SAMPLE_SERVICE sampleService = new LIS_SAMPLE_SERVICE();
                            sampleService.SERVICE_CODE = item.SERVICE_CODE;
                            sampleService.SERVICE_NAME = item.SERVICE_NAME;
                            lstSampleService.Add(sampleService);
                        }
                        currentLisSample.LIS_SAMPLE_SERVICE = lstSampleService;
                        currentLisSample.LAST_NAME = lstSereServChecks[0].LAST_NAME;
                        currentLisSample.FIRST_NAME = lstSereServChecks[0].FIRST_NAME;
                        currentLisSample.DOB = lstSereServChecks[0].DOB;
                        currentLisSample.GENDER_CODE = lstSereServChecks[0].GENDER_CODE;
                        var currentSample = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>(
                            "/api/LisSample/Create", ApiConsumers.LisConsumer, currentLisSample, param);
                        if (currentSample != null)
                        {
                            LoadDataGridSample();
                            success = true;
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Chưa chọn dịch vụ nào", "Thông báo");
                        return;
                    }
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonSample_Disable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                UpdateStt(LisSampleSttCFG.SAMPLE_STT_ID__SPECIMEN);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonSample_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                UpdateStt(LisSampleSttCFG.SAMPLE_STT_ID__UNSPECIMEN);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateStt(long sampleId)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                LIS.SDO.UpdateSampleSttSDO updateStt = new LIS.SDO.UpdateSampleSttSDO();
                var row = (LIS.EFMODEL.DataModels.LIS_SAMPLE)gridViewSimple.GetFocusedRow();
                //sampleId = row.SAMPLE_STT_ID;
                if (row != null)
                {
                    updateStt.Id = row.ID;
                    updateStt.SampleSttId = sampleId;
                    var curentSTT = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>(
                        "/api/LisSample/UpdateStt", ApiConsumers.LisConsumer, updateStt, param);
                    if (curentSTT != null)
                    {
                        LoadDataGridSample();
                        success = true;
                    }
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Delete_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (LIS.EFMODEL.DataModels.LIS_SAMPLE)gridViewSimple.GetFocusedRow();
                if (row != null)
                {
                    result = new BackendAdapter(param).Post<bool>("/api/LisSample/Delete",
                        ApiConsumers.LisConsumer,row.ID,param);
                    WaitingManager.Hide();
                    if (result == true)
                    {
                        gridControlSimple.BeginUpdate();
                        gridViewSimple.DeleteRow(gridViewSimple.FocusedRowHandle);
                        gridViewSimple.RefreshData();
                        gridControlSimple.RefreshDataSource();
                        gridControlSimple.EndUpdate();
                        LoadDataGridSample();
                    }
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, result);
                    #endregion

                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButton__Print__BarCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            onClickBtnPrintBarCode();
        }

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

        private void SetCaptionByLanguageKey()
        {
          try
          {
            if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
            {
              this.Text = this.moduleData.text;
            }
            ////Khoi tao doi tuong resource
            Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SampleRoomPartialKXN.Resources.Lang", typeof(HIS.Desktop.Plugins.SampleRoomPartialKXN.UCSampleRoomPartialKXN).Assembly);

            ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
            this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn35.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn35.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn36.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn36.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn21.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn22.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn23.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn23.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn24.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn24.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn25.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn25.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn26.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn26.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn27.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn27.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn28.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn28.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn29.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn29.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn30.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn30.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn31.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn31.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn32.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn32.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn33.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn33.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn34.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn34.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnCreateSample.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.btnCreateSample.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.cboFind.Properties.NullText = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.cboFind.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("UCSampleRoomPartialKXN.gridColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
          }
          catch (Exception ex)
          {
            Inventec.Common.Logging.LogSystem.Warn(ex);
          }
        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(
                    HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer,
                    ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    GlobalVariables.TemnplatePathFolder);

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
                var currentBarCode = (LIS_SAMPLE)gridViewBarCode.GetFocusedRow();
                MPS.Core.Mps000077.Mps000077RDO mps000077RDO = new MPS.Core.Mps000077.Mps000077RDO(
                    currentBarCode
                    );
                WaitingManager.Hide();
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
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

        private void cboFind_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboFind.Properties.Buttons[1].Visible = false;
                    cboFind.EditValue = null;
                    //txtFind.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboFind_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    
                    btnFind.Focus();
                    cboFind.Properties.Buttons[1].Visible = true;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
