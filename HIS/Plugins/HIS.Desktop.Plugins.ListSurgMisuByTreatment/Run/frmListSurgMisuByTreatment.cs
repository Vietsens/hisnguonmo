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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.Plugins.ListSurgMisuByTreatment.ADO;

namespace HIS.Desktop.Plugins.ListSurgMisuByTreatment.Run
{
    
    public partial class frmListSurgMisuByTreatment : HIS.Desktop.Utility.FormBase
    {
        //QTCODE
        private bool isHeaderChecked = false;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId;
        long _patientTypeId;
        HIS.Desktop.Common.DelegateLoadPTTT loadPTTT;
        V_HIS_SERE_SERV_1 currentRow = new V_HIS_SERE_SERV_1();
        List<HIS_SERE_SERV_EXT> lstSereServExts = new List<HIS_SERE_SERV_EXT>();
        bool isPTTT = false;
        private bool _handlingCtrlS = false;

        public frmListSurgMisuByTreatment()
        {
            InitializeComponent();
        }

        public frmListSurgMisuByTreatment(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmListSurgMisuByTreatment(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId,long patientTypeId)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this._patientTypeId = patientTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmListSurgMisuByTreatment(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, HIS.Desktop.Common.DelegateLoadPTTT loadPTTT)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                this.treatmentId = treatmentId;
                this.loadPTTT = loadPTTT;
                this.isPTTT = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmListSurgMisuByTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                this.KeyPreview = true;
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                LoadDataByTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataByTreatment()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisSereServView1Filter sereServFilter = new MOS.Filter.HisSereServView1Filter();
                    sereServFilter.TREATMENT_ID = this.treatmentId;
                    sereServFilter.ORDER_FIELD = "SERVICE_NUM_ORDER";
                    sereServFilter.ORDER_DIRECTION = "ASC";
                    List<long> serviceTypeIds = new List<long>();
                    Inventec.Common.Logging.LogSystem.Debug("this._patientTypeId" + this._patientTypeId);
                    Inventec.Common.Logging.LogSystem.Debug("IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT" + IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                    Inventec.Common.Logging.LogSystem.Debug("IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT" + IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT);
                    if (this._patientTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                    {
                        serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                    }
                    else if (this._patientTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT) 
                    { 
                        serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT); 
                    }

                    if (isPTTT) 
                    {
                        serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT);
                        serviceTypeIds.Add(IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT);
                    }
                    sereServFilter.SERVICE_TYPE_IDs = serviceTypeIds;
                    var sereServData = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_1>>("/api/HisSereServ/GetView1", ApiConsumers.MosConsumer, sereServFilter, param);

                    MOS.Filter.HisSereServExtFilter hisSereServExtFilter = new HisSereServExtFilter();
                    if (sereServData != null && sereServData.Count > 0)
                    {
                        List<long> lstID = sereServData.Select(o => o.ID).ToList();
                        hisSereServExtFilter.SERE_SERV_IDs = lstID;
                        lstSereServExts = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServExtFilter, null);
                    }
                    //gridControl.DataSource = null;
                    //gridControl.DataSource = sereServData;
                    //qtcode
                    List<SurgMisuADO> adoList = sereServData != null ? sereServData.Select(s => new SurgMisuADO(s)).ToList() : new List<SurgMisuADO>();
                    gridControl.DataSource = null;
                    gridControl.DataSource = adoList;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItem__Print_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                currentRow = new V_HIS_SERE_SERV_1();
                currentRow = (V_HIS_SERE_SERV_1)gridView.GetFocusedRow();
                if (currentRow != null)
                {
                    PrintProcess(PrintType._IN_GIAY_CHUNG_NHAN_PTTT);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal enum PrintType
        {
            _IN_GIAY_CHUNG_NHAN_PTTT
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType._IN_GIAY_CHUNG_NHAN_PTTT:
                        richEditorMain.RunPrintTemplate("Mps000204",null, DelegateRunPrinter, true);
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
                    case "Mps000204":
                        MPS000204(printTypeCode, fileName, ref result);
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

        private void MPS000204(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this.currentRow.TDL_TREATMENT_ID;
                var currentTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                MOS.Filter.HisSereServPtttViewFilter hisSereServPtttFilter = new MOS.Filter.HisSereServPtttViewFilter();
                hisSereServPtttFilter.SERE_SERV_ID = this.currentRow.ID;

                var hisSereServPttt = new BackendAdapter(param)
                 .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_PTTT>>(HisRequestUriStore.HIS_SERE_SERV_PTTT_GETVIEW, ApiConsumers.MosConsumer, hisSereServPtttFilter, param).FirstOrDefault();

                MOS.EFMODEL.DataModels.HIS_SERE_SERV_EXT sereServExt = new HIS_SERE_SERV_EXT();
                MOS.Filter.HisSereServExtFilter hisSereServExtFilter = new HisSereServExtFilter();
                hisSereServExtFilter.SERE_SERV_ID = this.currentRow.ID;
                var sereServExts = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_EXT>>("api/HisSereServExt/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServExtFilter, null);
                if (sereServExts != null && sereServExts.Count > 0)
                {
                    sereServExt = sereServExts.FirstOrDefault();
                }

                List<V_HIS_EKIP_USER> listEkipUser = new List<V_HIS_EKIP_USER>();
                if (this.currentRow.EKIP_ID.HasValue)
                {
                    MOS.Filter.HisEkipUserViewFilter hisEkipUserFilter = new MOS.Filter.HisEkipUserViewFilter();
                    hisEkipUserFilter.EKIP_ID = this.currentRow.EKIP_ID;
                    hisEkipUserFilter.ORDER_FIELD = "EXECUTE_ROLE_ID";
                    hisEkipUserFilter.ORDER_DIRECTION = "ASC";
                    listEkipUser = new BackendAdapter(param)
            .Get<List<V_HIS_EKIP_USER>>(HisRequestUriStore.HIS_EKIP_USER_GETVIEW, ApiConsumers.MosConsumer, hisEkipUserFilter, param);
                }

                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ID = this.currentRow.SERVICE_REQ_ID;

                var currentServiceReq = new BackendAdapter(param)
         .Get<List<HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, serviceReqFilter, param).FirstOrDefault();

                List<HIS_EXECUTE_ROLE> HisExecuteRoles = new List<HIS_EXECUTE_ROLE>();
                HisExecuteRoles = BackendDataWorker.Get<HIS_EXECUTE_ROLE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                string printerName = "";
                string treatmentCode = currentTreatment.TREATMENT_CODE;
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(treatmentCode, printTypeCode, currentModule != null ? currentModule.RoomId : 0);
                
                MPS.Processor.Mps000204.PDO.Mps000204PDO mps000204RDO = new MPS.Processor.Mps000204.PDO.Mps000204PDO(
                    this.currentRow,
                    currentTreatment,
                    hisSereServPttt,
                    listEkipUser,
                    currentServiceReq,
                    sereServExt,
                    HisExecuteRoles
                    );

                MPS.ProcessorBase.Core.PrintData PrintData = null;

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000204RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "",false);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000204RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "", false);
                }
                WaitingManager.Hide();
                PrintData.EmrInputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentTreatment.TREATMENT_CODE, printTypeCode, currentModule.RoomId);
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    short ServiceReqSttID = Inventec.Common.TypeConvert.Parse.ToInt16((gridView.GetRowCellValue(e.RowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                    if (e.Column.FieldName == "ServiceReqSttID")
                    {
                        if (ServiceReqSttID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                        {
                            e.RepositoryItem = repositoryItem__Print;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItem__PrintDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    //DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    //var data = (V_HIS_SERE_SERV_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    var data = (SurgMisuADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "INSTRUCTION_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TDL_INTRUCTION_TIME);
                        }
                        if (e.Column.FieldName == "SERVICE_TYPE_NAME")
                        {
                            if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT)
                                e.Value = "Thủ thuật";
                            else if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT)
                                e.Value = "Phẫu thuật";
                            else
                                e.Value = null;
                        }

                        if (e.Column.FieldName == "SERVICE_REQ_STT_NAME")
                        {
                            if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                e.Value = "Chưa xử lý";
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                e.Value = "Đang xử lý";
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                e.Value = "Hoàn thành";
                            }
                            else
                            {
                                e.Value = null;
                            }
                        }

                        if (e.Column.FieldName == "EXECUTE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                        if (e.Column.FieldName == "BEGIN_TIME_DISPLAY")
                        {
                            if (lstSereServExts != null && lstSereServExts.Count > 0)
                            {
                                var curSereServExt = lstSereServExts.Where(o => o.SERE_SERV_ID == data.ID).FirstOrDefault();
                                if (curSereServExt != null)
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(curSereServExt.BEGIN_TIME ?? 0);
                                }
                            }
                        }
                        if (e.Column.FieldName == "END_TIME_DISPLAY")
                        {
                            if (lstSereServExts != null && lstSereServExts.Count > 0)
                            {
                                var curSereServExt = lstSereServExts.Where(o => o.SERE_SERV_ID == data.ID).FirstOrDefault();
                                if (curSereServExt != null)
                                {
                                    e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(curSereServExt.END_TIME ?? 0);
                                }
                            }
                        }
                        //qtcode
                        if (e.Column.FieldName == "IS_SELECTED_STR")
                        {
                            e.Value = data.IS_SELECTED;
                        }
                    }
                }
                else if (e.IsSetData)
                {
                    var ado = (SurgMisuADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (ado != null)
                    {
                        if (e.Column.FieldName == "IS_SELECTED_STR")
                        {
                            ado.IS_SELECTED = (bool)e.Value; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            //try
            //{
            //    if (gridView.GetSelectedRows().Length > 0)
            //    {
            //        int selectedRowIndex = gridView.GetSelectedRows()[0];
            //        var dataRow = gridView.GetRow(selectedRowIndex);
            //        if (dataRow != null && gridView.Columns.Count > 0)
            //        {
            //            GridColumn colNamePTTT = gridView.Columns[2];
            //            GridColumn colBeginTime = gridView.Columns[7];
            //            GridColumn colEndTime = gridView.Columns[8];
            //            string namePTTT = (string)gridView.GetRowCellValue(selectedRowIndex, colNamePTTT);
            //            DateTime? dtBeginTime = null;
            //            object cellValueBeginTime = gridView.GetRowCellValue(selectedRowIndex, colBeginTime);
            //            if (cellValueBeginTime != null)
            //            {
            //                dtBeginTime = DateTime.Parse(cellValueBeginTime.ToString());
            //            }
            //            DateTime? dtEndTime = null;
            //            object cellValuedtEndTime = gridView.GetRowCellValue(selectedRowIndex, colEndTime);
            //            if (cellValuedtEndTime != null)
            //            {
            //                dtEndTime = DateTime.Parse(cellValuedtEndTime.ToString());
            //            }
            //            loadPTTT(namePTTT, dtBeginTime, dtEndTime);
            //            this.Close();
            //        }
            //    }
            //    else
            //    {
            //        // Không có dòng nào được chọn
            //        MessageBox.Show("Vui lòng chọn một dòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void gridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (gridView.GetSelectedRows().Length > 0)
                    {
                        int selectedRowIndex = gridView.GetSelectedRows()[0];
                        var dataRow = gridView.GetRow(selectedRowIndex);
                        if (dataRow != null && gridView.Columns.Count > 0)
                        {
                            GridColumn colNamePTTT = gridView.Columns[2];
                            GridColumn colBeginTime = gridView.Columns[7];
                            GridColumn colEndTime = gridView.Columns[8];
                            string namePTTT = (string)gridView.GetRowCellValue(selectedRowIndex, colNamePTTT);
                            DateTime? dtBeginTime = null;
                            object cellValueBeginTime = gridView.GetRowCellValue(selectedRowIndex, colBeginTime);
                            if (cellValueBeginTime != null)
                            {
                                dtBeginTime = DateTime.Parse(cellValueBeginTime.ToString());
                            }
                            DateTime? dtEndTime = null;
                            object cellValuedtEndTime = gridView.GetRowCellValue(selectedRowIndex, colEndTime);
                            if (cellValuedtEndTime != null)
                            {
                                dtEndTime = DateTime.Parse(cellValuedtEndTime.ToString());
                            }
                            loadPTTT(namePTTT, dtBeginTime, dtEndTime);
                            this.Close();
                        }
                    }
                    else
                    {
                        // Không có dòng nào được chọn
                        MessageBox.Show("Vui lòng chọn một dòng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            try
            {
                // Thu thập các dịch vụ được chọn cùng với rowHandle tương ứng
                List<SurgMisuADO> selectedAdos = new List<SurgMisuADO>();
                List<int> selectedRowHandles = new List<int>();
                int firstSelectedHandle = -1;

                for (int i = 0; i < gridView.DataRowCount; i++)
                {
                    var ado = (SurgMisuADO)gridView.GetRow(i);
                    bool isSelected = (bool?)gridView.GetRowCellValue(i, "IS_SELECTED_STR") ?? false;

                    if (isSelected && ado != null)
                    {
                        selectedAdos.Add(ado);
                        selectedRowHandles.Add(i);
                        if (firstSelectedHandle < 0) firstSelectedHandle = i;
                    }
                }

                if (selectedAdos.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một dịch vụ.", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Trường hợp chỉ chọn 1 dịch vụ: giữ nguyên hành vi cũ
                if (selectedAdos.Count == 1)
                {
                    int rowHandle = firstSelectedHandle;
                    GridColumn colBeginTime = gridView.Columns[8];
                    GridColumn colEndTime = gridView.Columns[9];

                    DateTime? dtBeginTime = null, dtEndTime = null;

                    var cellBegin = gridView.GetRowCellValue(rowHandle, colBeginTime);
                    if (cellBegin != null && !string.IsNullOrWhiteSpace(cellBegin.ToString()))
                    {
                        if (DateTime.TryParse(cellBegin.ToString(), out var tmpBegin))
                            dtBeginTime = tmpBegin;
                    }

                    var cellEnd = gridView.GetRowCellValue(rowHandle, colEndTime);
                    if (cellEnd != null && !string.IsNullOrWhiteSpace(cellEnd.ToString()))
                    {
                        if (DateTime.TryParse(cellEnd.ToString(), out var tmpEnd))
                            dtEndTime = tmpEnd;
                    }

                    string surgeryName = selectedAdos[0].TDL_SERVICE_NAME;
                    if (loadPTTT != null)
                        loadPTTT(surgeryName, dtBeginTime, dtEndTime);

                    this.Close();
                    return;
                }

                // Trường hợp chọn nhiều dịch vụ
                List<string> surgeryInfoList = new List<string>();
                GridColumn colBeginMulti = gridView.Columns[8];
                GridColumn colEndMulti = gridView.Columns[9];

                for (int index = 0; index < selectedAdos.Count; index++)
                {
                    var ado = selectedAdos[index];
                    int rowHandle = selectedRowHandles[index];

                    DateTime? dtBeginTime = null, dtEndTime = null;

                    var cellBegin = gridView.GetRowCellValue(rowHandle, colBeginMulti);
                    if (cellBegin != null && !string.IsNullOrWhiteSpace(cellBegin.ToString()))
                    {
                        if (DateTime.TryParse(cellBegin.ToString(), out var tmpBegin))
                            dtBeginTime = tmpBegin;
                    }

                    var cellEnd = gridView.GetRowCellValue(rowHandle, colEndMulti);
                    if (cellEnd != null && !string.IsNullOrWhiteSpace(cellEnd.ToString()))
                    {
                        if (DateTime.TryParse(cellEnd.ToString(), out var tmpEnd))
                            dtEndTime = tmpEnd;
                    }

                    string name = ado.TDL_SERVICE_NAME;

                    // Format lại dữ liệu theo yêu cầu trên thiết kế
                    List<string> timeInfo = new List<string>();

                    if (dtBeginTime.HasValue)
                    {
                        timeInfo.Add(string.Format("Bắt đầu {0} giờ {1} phút, ngày {2} tháng {3} năm {4}",
                            dtBeginTime.Value.Hour.ToString("D2"),
                            dtBeginTime.Value.Minute.ToString("D2"),
                            dtBeginTime.Value.Day.ToString("D2"),
                            dtBeginTime.Value.Month.ToString("D2"),
                            dtBeginTime.Value.Year));
                    }

                    if (dtEndTime.HasValue)
                    {
                        timeInfo.Add(string.Format("Kết thúc: {0} giờ {1} phút, ngày {2} tháng {3} năm {4}",
                            dtEndTime.Value.Hour.ToString("D2"),
                            dtEndTime.Value.Minute.ToString("D2"),
                            dtEndTime.Value.Day.ToString("D2"),
                            dtEndTime.Value.Month.ToString("D2"),
                            dtEndTime.Value.Year));
                    }

                    if (timeInfo.Count > 0)
                    {
                        surgeryInfoList.Add(name + ":" + Environment.NewLine + string.Join("; ", timeInfo) + ";");
                    }
                    else
                    {
                        surgeryInfoList.Add(name + ":;");
                    }
                }

                string surgeryNamesWithTime = string.Join(Environment.NewLine, surgeryInfoList);

                if (loadPTTT != null)
                {
                    loadPTTT(surgeryNamesWithTime, null, null);
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                MessageBox.Show("Đã xảy ra lỗi khi xử lý.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmListSurgMisuByTreatment_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control && e.KeyCode == Keys.S)
                {
                    if (_handlingCtrlS) return;        
                    _handlingCtrlS = true;

                    e.Handled = true;                 
                    e.SuppressKeyPress = true;

                    btnSelect_Click(sender, e);
                    _handlingCtrlS = false;
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomDrawColumnHeader(object sender, DevExpress.XtraGrid.Views.Grid.ColumnHeaderCustomDrawEventArgs e)
        {
            if (e.Column != null && e.Column.FieldName == "IS_SELECTED_STR")
            {
                e.Info.InnerElements.Clear();
                e.Painter.DrawObject(e.Info);

                RepositoryItemCheckEdit checkEdit = e.Column.ColumnEdit as RepositoryItemCheckEdit;
                if (checkEdit != null)
                {
                    int size = 16;
                    int x = e.Bounds.X + (e.Bounds.Width - size) / 2;
                    int y = e.Bounds.Y + (e.Bounds.Height - size) / 2;
                    Rectangle rect = new Rectangle(x, y, size, size);

                    var info = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.CreateViewInfo();
                    var painter = (DevExpress.XtraEditors.Drawing.CheckEditPainter)checkEdit.CreatePainter();
                    info.EditValue = isHeaderChecked;
                    info.Bounds = rect;
                    info.CalcViewInfo(e.Graphics);

                    using (DevExpress.Utils.Drawing.GraphicsCache cache = new DevExpress.Utils.Drawing.GraphicsCache(e.Graphics))
                    {
                        painter.Draw(new DevExpress.XtraEditors.Drawing.ControlGraphicsInfoArgs(info, cache, rect));
                    }
                }
                e.Handled = true;
            }
        }

        private void gridView_MouseDown(object sender, MouseEventArgs e)
        {
            GridView view = sender as GridView;
            GridHitInfo info = view.CalcHitInfo(e.Location);
            if (info.InColumn && info.Column.FieldName == "IS_SELECTED_STR")
            {
                isHeaderChecked = !isHeaderChecked;
                for (int i = 0; i < view.RowCount; i++)
                {
                    view.SetRowCellValue(i, view.Columns["IS_SELECTED_STR"], isHeaderChecked);
                }
                view.InvalidateColumnHeader(view.Columns["IS_SELECTED_STR"]);
            }
        }

        private void gridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "IS_SELECTED_STR")
                {
                    var service = (V_HIS_SERE_SERV_1)gridView.GetRow(e.RowHandle);
                    if (service != null)
                    {
                        bool newValue = !(bool?)gridView.GetRowCellValue(e.RowHandle, "IS_SELECTED_STR") ?? true;
                        gridView.SetRowCellValue(e.RowHandle, "IS_SELECTED_STR", newValue);

                        List<V_HIS_SERE_SERV_1> selectedServices = new List<V_HIS_SERE_SERV_1>();
                        for (int i = 0; i < gridView.DataRowCount; i++)
                        {
                            var row = (V_HIS_SERE_SERV_1)gridView.GetRow(i);
                            bool isSelected = (bool?)gridView.GetRowCellValue(i, "IS_SELECTED_STR") ?? false;
                            if (isSelected)
                            {
                                selectedServices.Add(row);
                            }
                        }

                        //gridControl.RefreshDataSource();
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
