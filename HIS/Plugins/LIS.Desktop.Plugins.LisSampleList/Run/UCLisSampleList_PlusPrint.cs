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
using DevExpress.XtraEditors;
using EMR.Filter;
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.Desktop.Plugins.LisSampleList.Config;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.SignLibrary;

namespace LIS.Desktop.Plugins.LisSampleList.Run
{
    public partial class UCLisSampleList : UserControlBase
    {
        V_LIS_SAMPLE_1 SamplePrint;

        private void ProcessPrintRow(V_LIS_SAMPLE_1 row)
        {
            try
            {
                if (row != null)
                {
                    if (!String.IsNullOrWhiteSpace(row.EMR_RESULT_DOCUMENT_URL))
                    {
                        ProcessShowEmrByUrl(row);
                    }
                    else
                    {
                        PrintMps96(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void PrintMps96(V_LIS_SAMPLE_1 row)
        {
            try
            {
                this.SamplePrint = row;

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
            bool result = true;
            try
            {
                if (this.SamplePrint != null)
                {
                    if (String.IsNullOrWhiteSpace(this.SamplePrint.SERVICE_REQ_CODE))
                    {
                        this.InKetQuaXNKhongCoServiceReq(printTypeCode, fileName, ref result);
                    }
                    else
                    {
                        this.InKetQuaXNCoServiceReq(printTypeCode, fileName, ref result);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private long LoadGenderId(V_LIS_SAMPLE_1 samplePrint)
        {
            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (samplePrint != null && !String.IsNullOrWhiteSpace(samplePrint.GENDER_CODE))
                {
                    genderId = samplePrint.GENDER_CODE == "01" ? 1 : 2;
                }
                else if (samplePrint != null && !String.IsNullOrWhiteSpace(samplePrint.PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = samplePrint.PATIENT_CODE;
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

        private void InKetQuaXNCoServiceReq(string printTypeCode, string fileName, ref bool result)
        {
            WaitingManager.Show();
            CommonParam param = new CommonParam();
            HisServiceReqFilter ServiceReqViewFilter = new HisServiceReqFilter();
            ServiceReqViewFilter.SERVICE_REQ_CODE__EXACT = this.SamplePrint.SERVICE_REQ_CODE;
            var currentServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();
            if (currentServiceReq == null)
            {
                Inventec.Common.Logging.LogSystem.Info("Khong lay duoc ServiceReq" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.SamplePrint), this.SamplePrint));
                return;
            }

            MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
            treatmentFilter.ID = currentServiceReq.TREATMENT_ID;
            var curentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

            MOS.Filter.HisPatientFilter patientFilter = new HisPatientFilter();
            patientFilter.ID = currentServiceReq.TDL_PATIENT_ID;
            var lstPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, patientFilter, param);

            HIS_PATIENT patient = lstPatient != null ? lstPatient.FirstOrDefault() : null;
            var currentPatientTypeAlter = new BackendAdapter(param).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, currentServiceReq.TREATMENT_ID, param);

            LisResultViewFilter resultFilter = new LisResultViewFilter();
            resultFilter.SAMPLE_ID = this.SamplePrint.ID;
            List<V_LIS_RESULT> lstResultPrint = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumers.LisConsumer, resultFilter, param);

            WaitingManager.Hide();

            long genderId = LoadGenderId(this.SamplePrint);
            string printerName = "";
            if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
            {
                printerName = GlobalVariables.dicPrinter[printTypeCode];
            }

            V_LIS_SAMPLE sample = new V_LIS_SAMPLE();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(sample, this.SamplePrint);

            Inventec.Common.Logging.LogSystem.Debug("LoadBieuMauInKetQuaXetNghiemV2 SamplePrint.PATIENT_CODE: " + this.SamplePrint.PATIENT_CODE);

            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((curentTreatment != null ? curentTreatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

            MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                       currentPatientTypeAlter,
                       curentTreatment,
                       sample,
                       currentServiceReq,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX>(),
                       lstResultPrint,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>(),
                       genderId,
                       BackendDataWorker.Get<V_HIS_SERVICE>(),
                       patient, null);

            MPS.ProcessorBase.Core.PrintData PrintData = null;

            if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
            {
                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
            }
            else
            {
                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
            }

            PrintData.EmrInputADO = inputADO;
            result = MPS.MpsPrinter.Run(PrintData);
            if (result && (PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow))
            {
                MessageManager.Show(this.ParentForm, new CommonParam(), true);
            }
        }

        private void InKetQuaXNKhongCoServiceReq(string printTypeCode, string fileName, ref bool result)
        {
            WaitingManager.Show();
            CommonParam param = new CommonParam();
            LisResultViewFilter resultFilter = new LisResultViewFilter();
            resultFilter.SAMPLE_ID = this.SamplePrint.ID;
            List<V_LIS_RESULT> lstResultPrint = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumers.LisConsumer, resultFilter, param);
            WaitingManager.Hide();

            long genderId = LoadGenderId(this.SamplePrint);
            string printerName = "";
            if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
            {
                printerName = GlobalVariables.dicPrinter[printTypeCode];
            }

            V_LIS_SAMPLE sample = new V_LIS_SAMPLE();
            Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(sample, this.SamplePrint);

            MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                       null,
                       null,
                       sample,
                       null,
                       BackendDataWorker.Get<V_HIS_TEST_INDEX>(),
                       lstResultPrint,
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

            result = MPS.MpsPrinter.Run(PrintData);
            if (result && (PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow))
            {
                MessageManager.Show(this.ParentForm, new CommonParam(), true);
            }
        }

        private void ProcessShowEmrByUrl(V_LIS_SAMPLE_1 rowData)
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentDownloadFileSDO filter = new EmrDocumentDownloadFileSDO();
                filter.EmrDocumentViewFilter = new EmrDocumentViewFilter();
                filter.EmrDocumentViewFilter.DOCUMENT_CODE__EXACT = rowData.EMR_RESULT_DOCUMENT_CODE;
                filter.IsMerge = false;
                filter.IsShowPatientSign = false;
                filter.IsShowWatermark = false;

                if (room != null)
                {
                    filter.RoomCode = room.ROOM_CODE;
                    filter.DepartmentCode = room.DEPARTMENT_CODE;
                }

                filter.IsView = null;

                List<EmrDocumentFileSDO> apiResult = new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, filter, paramCommon);
                if (apiResult != null && apiResult.Count > 0)
                {
                    string fileName = Path.GetTempFileName();
                    fileName = fileName.Replace("tmp", "pdf");

                    Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(apiResult.FirstOrDefault().Base64Data))), fileName);

                    Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(Inventec.Common.DocumentViewer.ViewType.ENUM.Pdf);
                    Inventec.Common.DocumentViewer.InputADO ado = new Inventec.Common.DocumentViewer.InputADO();
                    ado.DeleteWhenClose = true;
                    ado.URL = fileName;
                    Inventec.Common.DocumentViewer.ViewType.Platform type = Inventec.Common.DocumentViewer.ViewType.Platform.Telerik;
                    if (HisConfigCFG.PlatformOption > 0)
                    {
                        type = (Inventec.Common.DocumentViewer.ViewType.Platform)(HisConfigCFG.PlatformOption - 1);
                    }

                    viewManager.Run(ado, type);
                }
                else
                {
                    #region Hien thi message thong bao
                    MessageManager.Show(paramCommon, false);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessExportExcel(List<V_LIS_SAMPLE_1> listSelected)
        {
            try
            {
                if (listSelected != null && listSelected.Count > 0)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.DefaultExt = "xlsx";
                    saveFileDialog.Filter = "Excel files|*.xlsx";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp\\", "KetQuaXetNghiem.xlsx");
                        if (!File.Exists(fileName))
                        {
                            XtraMessageBox.Show("Không tìm thấy file template theo đường dẫn " + fileName);
                            return;
                        }

                        WaitingManager.Show();
                        bool success = false;
                        Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                        Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                        Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                        store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                        store.SetCommonFunctions();

                        objectTag.AddObjectData(store, "ExportResult", listSelected);

                        WaitingManager.Hide();
                        success = store.OutFile(saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, null, success);
                        if (!success)
                            return;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
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
