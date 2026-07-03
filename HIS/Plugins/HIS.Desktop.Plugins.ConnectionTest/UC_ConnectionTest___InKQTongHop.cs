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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using HIS.Desktop.Plugins.ConnectionTest.Config;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class UC_ConnectionTest : HIS.Desktop.Utility.UserControlBase
    {
        #region In KQ tong hop

        /// <summary>PDO đã dựng, truyền sang callback in của RichEditorStore.</summary>
        private MPS.Processor.Mps000517.PDO.Mps000517PDO _mps517PdoToPrint;

        private void btnInKetQuaTongHop_Click(object sender, EventArgs e)
        {
            try
            {
                // QT1: phải chọn ít nhất 1 mẫu.
                // Đọc từ lstSampleAll (tập đầy đủ) để không bỏ sót mẫu đã tích nhưng đang bị ẩn bởi filter Nhóm XN.
                List<LisSampleADO> source = lstSampleAll ?? (gridControlSample.DataSource as List<LisSampleADO>);
                List<LisSampleADO> checkedSamples = source != null
                    ? source.Where(o => o.IsCheck).ToList()
                    : new List<LisSampleADO>();
                if (checkedSamples.Count == 0)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.ChuaChonMauXetNghiem,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    return;
                }

                // QT2: các mẫu phải cùng một bệnh nhân (theo PATIENT_CODE).
                List<string> distinctPatients = checkedSamples
                    .Select(o => o.PATIENT_CODE).Distinct().ToList();
                if (distinctPatients.Count > 1)
                {
                    XtraMessageBox.Show(
                        Resources.ResourceMessage.MauKhongCungMotBenhNhan,
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    return;
                }

                WaitingManager.Show();
                bool hasError = false;
                _mps517PdoToPrint = BuildMps000517PDO(checkedSamples, ref hasError);
                WaitingManager.Hide();
                if (hasError || _mps517PdoToPrint == null)
                {
                    return;
                }

                // QT4: gọi biểu in Mps000517.
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(
                    HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer,
                    HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
                    HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate(
                    MPS.Processor.Mps000517.PDO.PrintTypeCode.Mps000517, DelegateRunPrinterTongHop);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterTongHop(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (_mps517PdoToPrint == null)
                {
                    return false;
                }
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                WaitingManager.Hide();
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printTypeCode, fileName, _mps517PdoToPrint,
                        MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(
                        printTypeCode, fileName, _mps517PdoToPrint,
                        MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Gom dữ liệu cho nhiều mẫu (cùng 1 bệnh nhân) và dựng Mps000517PDO.
        /// QT3: nếu có mẫu chưa có kết quả (không có V_LIS_RESULT) -> cảnh báo và dừng.
        /// </summary>
        private MPS.Processor.Mps000517.PDO.Mps000517PDO BuildMps000517PDO(List<LisSampleADO> checkedSamples, ref bool hasError)
        {
            try
            {
                // QT3: nạp kết quả từng mẫu, kiểm tra mẫu chưa có kết quả.
                List<V_LIS_RESULT> allResults = new List<V_LIS_RESULT>();
                List<LisSampleADO> samplesNoResult = new List<LisSampleADO>();
                foreach (var s in checkedSamples)
                {
                    CommonParam pr = new CommonParam();
                    LisResultViewFilter rf = new LisResultViewFilter();
                    rf.SAMPLE_ID = s.ID;
                    List<V_LIS_RESULT> rs = new BackendAdapter(pr).Get<List<V_LIS_RESULT>>(
                        "api/LisResult/GetView", ApiConsumers.LisConsumer, rf, pr);
                    if (rs == null || rs.Count == 0)
                    {
                        samplesNoResult.Add(s);
                    }
                    else
                    {
                        allResults.AddRange(rs);
                    }
                }
                if (samplesNoResult.Count > 0)
                {
                    WaitingManager.Hide();
                    string codes = string.Join(", ", samplesNoResult.Select(o => o.BARCODE));
                    XtraMessageBox.Show(
                        string.Format(Resources.ResourceMessage.CoMauChuaCoKetQua, codes),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                    hasError = true;
                    return null;
                }

                // Yêu cầu dịch vụ theo từng mã (distinct).
                List<HIS_SERVICE_REQ> serviceReqs = new List<HIS_SERVICE_REQ>();
                List<string> serviceReqCodes = checkedSamples
                    .Select(o => o.SERVICE_REQ_CODE)
                    .Where(c => !string.IsNullOrEmpty(c)).Distinct().ToList();
                foreach (var code in serviceReqCodes)
                {
                    CommonParam ps = new CommonParam();
                    HisServiceReqFilter srf = new HisServiceReqFilter();
                    srf.SERVICE_REQ_CODE__EXACT = code;
                    var sr = new BackendAdapter(ps).Get<List<HIS_SERVICE_REQ>>(
                        "api/HisServiceReq/Get", ApiConsumers.MosConsumer, srf, ps);
                    if (sr != null && sr.Count > 0)
                    {
                        serviceReqs.Add(sr.FirstOrDefault());
                    }
                }
                HIS_SERVICE_REQ firstSr = serviceReqs.FirstOrDefault();
                if (firstSr == null)
                {
                    hasError = true;
                    return null;
                }

                // Điều trị, bệnh nhân, đối tượng BHYT (cùng 1 bệnh nhân).
                CommonParam pt = new CommonParam();
                HisTreatmentFilter tf = new HisTreatmentFilter();
                tf.ID = firstSr.TREATMENT_ID;
                HIS_TREATMENT treatment = new BackendAdapter(pt).Get<List<HIS_TREATMENT>>(
                    "api/HisTreatment/Get", ApiConsumers.MosConsumer, tf, pt).FirstOrDefault();

                CommonParam pp = new CommonParam();
                HisPatientFilter pf = new HisPatientFilter();
                pf.ID = firstSr.TDL_PATIENT_ID;
                var lstP = new BackendAdapter(pp).Get<List<HIS_PATIENT>>(
                    "api/HisPatient/Get", ApiConsumers.MosConsumer, pf, pp);
                HIS_PATIENT patient = lstP != null ? lstP.FirstOrDefault() : null;

                CommonParam pa = new CommonParam();
                HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(pa).Get<HIS_PATIENT_TYPE_ALTER>(
                    "api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, firstSr.TREATMENT_ID, pa);

                // genderId: ưu tiên GENDER_CODE của mẫu, fallback theo bệnh nhân.
                long genderIdLocal = 0;
                LisSampleADO firstSample = checkedSamples.FirstOrDefault();
                if (firstSample != null && !string.IsNullOrEmpty(firstSample.GENDER_CODE))
                {
                    genderIdLocal = firstSample.GENDER_CODE == "01" ? 1 : 2;
                }
                else if (patient != null)
                {
                    genderIdLocal = patient.GENDER_ID;
                }

                // Chỉ số XN, dải tham chiếu, danh mục dịch vụ (cache RAM).
                List<string> resultServiceCodes = allResults.Select(o => o.SERVICE_CODE).Distinct().ToList();
                List<V_HIS_TEST_INDEX> testIndexs = BackendDataWorker.Get<V_HIS_TEST_INDEX>()
                    .Where(o => resultServiceCodes.Contains(o.SERVICE_CODE)).ToList();
                List<V_HIS_TEST_INDEX_RANGE> ranges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                List<V_HIS_SERVICE> listService = BackendDataWorker.Get<V_HIS_SERVICE>();

                // Nhóm cha (header) - lấy nhóm đầu tiên có PARENT_ID.
                V_HIS_SERVICE serviceParent = null;
                var groupByParent = listService
                    .Where(o => resultServiceCodes.Contains(o.SERVICE_CODE) && o.PARENT_ID != null)
                    .GroupBy(o => o.PARENT_ID).FirstOrDefault();
                if (groupByParent != null && groupByParent.Key != null)
                {
                    serviceParent = listService.FirstOrDefault(x => x.ID == groupByParent.Key);
                }

                // Giường - phòng theo ngữ cảnh yêu cầu đầu tiên.
                V_HIS_TREATMENT_BED_ROOM treatBedRoom = null;
                CommonParam pb = new CommonParam();
                HisTreatmentBedRoomViewFilter bf = new HisTreatmentBedRoomViewFilter();
                bf.TREATMENT_ID = firstSr.TREATMENT_ID;
                var bedRooms = new BackendAdapter(pb).Get<List<V_HIS_TREATMENT_BED_ROOM>>(
                    "api/HisTreatmentBedRoom/GetView", ApiConsumers.MosConsumer, bf, pb);
                if (bedRooms != null && bedRooms.Count > 0)
                {
                    treatBedRoom = bedRooms
                        .Where(o => o.ADD_TIME <= firstSr.INTRUCTION_TIME && o.ROOM_ID == firstSr.REQUEST_ROOM_ID)
                        .FirstOrDefault() ?? bedRooms.FirstOrDefault();
                }

                // Dịch vụ thực hiện theo từng mã yêu cầu.
                List<HIS_SERE_SERV> sereServList = new List<HIS_SERE_SERV>();
                foreach (var code in serviceReqCodes)
                {
                    CommonParam pss = new CommonParam();
                    HisSereServFilter ssf = new HisSereServFilter();
                    ssf.TDL_SERVICE_REQ_CODE_EXACT = code;
                    var ss = new BackendAdapter(pss).Get<List<HIS_SERE_SERV>>(
                        "api/HisSereServ/Get", ApiConsumers.MosConsumer, ssf, pss);
                    if (ss != null && ss.Count > 0)
                    {
                        sereServList.AddRange(ss);
                    }
                }

                // Loại mẫu (theo cấu hình tích hợp PACS/LIS, giống print 1 mẫu).
                List<LIS_SAMPLE_TYPE> listSampleType = new List<LIS_SAMPLE_TYPE>();
                List<HIS_TEST_SAMPLE_TYPE> listTestSampleType = new List<HIS_TEST_SAMPLE_TYPE>();
                if ((PacsCFG.MosLisInterGrationVersion == "1" && PacsCFG.MosLisInterGrationOption == "1")
                    || (PacsCFG.MosLisInterGrationVersion == "2" && PacsCFG.MosLisInterGrationType == "1"))
                {
                    listSampleType = BackendDataWorker.Get<LIS_SAMPLE_TYPE>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }
                else
                {
                    listTestSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>()
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                }

                // Map các mẫu sang V_LIS_SAMPLE.
                List<V_LIS_SAMPLE> currentSamples = new List<V_LIS_SAMPLE>();
                foreach (var s in checkedSamples)
                {
                    V_LIS_SAMPLE v = new V_LIS_SAMPLE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(v, s);
                    currentSamples.Add(v);
                }

                MPS.Processor.Mps000517.PDO.Mps000517PDO pdo = new MPS.Processor.Mps000517.PDO.Mps000517PDO(
                    patientTypeAlter,
                    treatment,
                    currentSamples,
                    serviceReqs,
                    testIndexs,
                    allResults,
                    ranges,
                    genderIdLocal,
                    listService,
                    patient,
                    treatBedRoom,
                    sereServList,
                    listSampleType,
                    listTestSampleType,
                    new List<MPS.Processor.Mps000517.PDO.MLCTADO>(),
                    serviceParent);
                return pdo;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                hasError = true;
                return null;
            }
        }

        #endregion
    }
}
