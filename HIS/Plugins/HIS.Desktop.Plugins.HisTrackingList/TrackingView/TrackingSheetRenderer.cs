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
using EMR.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisTrackingList.ADO;
using HIS.Desktop.Plugins.HisTrackingList.Config;
using HIS.Desktop.Plugins.HisTrackingList.Run;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Desktop.Plugins.HisTrackingList.TrackingView
{
    /// <summary>
    /// Loads treatment sheet data and renders treatment sheets (Mps000062) without any UI,
    /// so it can run on a background thread.
    /// </summary>
    internal static class TrackingSheetRenderer
    {
        internal const string MODULE_LINK = "HIS.Desktop.Plugins.HisTrackingList";
        internal const string CONTROL_STATE_KEY__TEMPLATE = "btnTemplate";
        const string CONFIG_KEY__IS_NOT_SHOW_OUT_MEDI_AND_MATE = "HIS.Desktop.Plugins.TrackingPrint.IsNotShowOutMediAndMate";
        const string CONFIG_KEY__IS_SHOW_MEDICINE_LINE = "HIS.Desktop.Plugins.Library.Bordereau.IsShowMedicineLine";
        const string CONFIG_KEY__ORDER_OPTION = "HIS.Desktop.Plugins.TrackingPrint.OderOption";

        /// <summary>
        /// Loads all data needed to print the treatment sheets of a treatment (api/HisTracking/GetData).
        /// Returns null when the call fails.
        /// </summary>
        internal static TrackingPrintData LoadData(long treatmentId)
        {
            TrackingPrintData result = null;
            try
            {
                bool isNotShowOutMediAndMate = HisConfigs.Get<string>(CONFIG_KEY__IS_NOT_SHOW_OUT_MEDI_AND_MATE) == "1";
                string keyDoNotShowExpendMaterial = HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_DO_NOT_SHOW_EXPEND_MATERIAL) ?? "";

                CommonParam param = new CommonParam();
                TrackingDataInputSDO sdo = new TrackingDataInputSDO();
                sdo.TreatmentId = treatmentId;
                sdo.IncludeMaterial = HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MATERIAL) == "1";
                sdo.IncludeMoveBackMediMate = HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_MEDI_MATE_TH) == "1";
                sdo.IncludeBloodPres = HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_CREATE_BLOOD_PRES_OPTION) == "1";
                var trackingData = new BackendAdapter(param).Get<HisTrackingDataSDO>("api/HisTracking/GetData", ApiConsumers.MosConsumer, sdo, param);
                if (trackingData == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("api/HisTracking/GetData returns null. " + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                    return null;
                }

                result = new TrackingPrintData();
                result.Treatment = trackingData.Treatment ?? new HIS_TREATMENT();
                result.TreatmentBedRooms = trackingData.TreatmentBedRooms ?? new List<V_HIS_TREATMENT_BED_ROOM>();
                result.MobaImpMests = trackingData.vImpMests2 ?? new List<V_HIS_IMP_MEST_2>();
                result.ImpMestMedicinesTL = trackingData.vImpMestMedicines ?? new List<V_HIS_IMP_MEST_MEDICINE>();
                result.ImpMestMaterialsTL = trackingData.vImpMestMaterials ?? new List<V_HIS_IMP_MEST_MATERIAL>();
                result.ImpMestBloodsTL = trackingData.vImpMestBloods ?? new List<V_HIS_IMP_MEST_BLOOD>();
                result.Dhsts = trackingData.HisDHSTs ?? new List<HIS_DHST>();
                result.SereServRations = trackingData.vSereServRations ?? new List<V_HIS_SERE_SERV_RATION>();
                result.ExpMestBltyReq2s = trackingData.vExpMestBityReqs2 ?? new List<V_HIS_EXP_MEST_BLTY_REQ_2>();
                result.Cares = trackingData.HisCares ?? new List<HIS_CARE>();
                result.SereServExts = trackingData.SereServExts ?? new List<HIS_SERE_SERV_EXT>();
                result.CareDetails = new List<V_HIS_CARE_DETAIL>();
                if (trackingData.CareDetails != null && trackingData.CareDetails.Count > 0)
                    result.CareDetails.AddRange(trackingData.CareDetails);

                result.DicServiceReqs = new Dictionary<long, HIS_SERVICE_REQ>();
                if (trackingData.ServiceReqs != null)
                {
                    foreach (var item in trackingData.ServiceReqs)
                    {
                        if (!result.DicServiceReqs.ContainsKey(item.ID))
                            result.DicServiceReqs[item.ID] = item;
                    }
                }

                result.DicExpMests = new Dictionary<long, HIS_EXP_MEST>();
                if (trackingData.ExpMests != null)
                {
                    foreach (var item in trackingData.ExpMests)
                    {
                        result.DicExpMests[item.SERVICE_REQ_ID ?? 0] = item;
                    }
                }

                result.DicServiceReqMetys = new Dictionary<long, List<HIS_SERVICE_REQ_METY>>();
                result.DicServiceReqMatys = new Dictionary<long, List<HIS_SERVICE_REQ_MATY>>();
                if (isNotShowOutMediAndMate)
                {
                    if (trackingData.ServiceReqMetys != null)
                    {
                        foreach (var item in trackingData.ServiceReqMetys)
                        {
                            if (!result.DicServiceReqMetys.ContainsKey(item.SERVICE_REQ_ID))
                                result.DicServiceReqMetys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_METY>();
                            result.DicServiceReqMetys[item.SERVICE_REQ_ID].Add(item);
                        }
                    }
                    if (trackingData.ServiceReqMatys != null)
                    {
                        foreach (var item in trackingData.ServiceReqMatys)
                        {
                            if (!result.DicServiceReqMatys.ContainsKey(item.SERVICE_REQ_ID))
                                result.DicServiceReqMatys[item.SERVICE_REQ_ID] = new List<HIS_SERVICE_REQ_MATY>();
                            result.DicServiceReqMatys[item.SERVICE_REQ_ID].Add(item);
                        }
                    }
                }

                result.SereServs = trackingData.SereServs ?? new List<HIS_SERE_SERV>();
                result.DicSereServs = new Dictionary<long, List<HIS_SERE_SERV>>();
                foreach (var item in result.SereServs)
                {
                    if (!result.DicSereServs.ContainsKey(item.SERVICE_REQ_ID ?? 0))
                        result.DicSereServs[item.SERVICE_REQ_ID ?? 0] = new List<HIS_SERE_SERV>();
                    result.DicSereServs[item.SERVICE_REQ_ID ?? 0].Add(item);
                }

                result.ExpMestMedicines = trackingData.ExpMestMedicines ?? new List<HIS_EXP_MEST_MEDICINE>();
                result.DicExpMestMedicines = new Dictionary<long, List<HIS_EXP_MEST_MEDICINE>>();
                var medicineGroups = result.ExpMestMedicines.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MEDICINE_TYPE_ID, p.EXP_MEST_ID, p.TUTORIAL }).Select(p => p.ToList()).ToList();
                foreach (var item in medicineGroups)
                {
                    HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MEDICINE>(ado, item[0]);
                    ado.AMOUNT = item.Sum(p => p.AMOUNT);
                    ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                    if (!result.DicExpMestMedicines.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        result.DicExpMestMedicines[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MEDICINE>();
                    result.DicExpMestMedicines[ado.EXP_MEST_ID ?? 0].Add(ado);
                }

                result.ExpMestMaterials = trackingData.ExpMestMaterials ?? new List<HIS_EXP_MEST_MATERIAL>();
                if (keyDoNotShowExpendMaterial == "1")
                    result.ExpMestMaterials = result.ExpMestMaterials.Where(o => o.IS_EXPEND != 1).ToList();
                result.DicExpMestMaterials = new Dictionary<long, List<HIS_EXP_MEST_MATERIAL>>();
                var materialGroups = result.ExpMestMaterials.Where(p => p.IS_NOT_PRES != 1).GroupBy(p => new { p.TDL_MATERIAL_TYPE_ID, p.EXP_MEST_ID }).Select(p => p.ToList()).ToList();
                foreach (var item in materialGroups)
                {
                    HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_EXP_MEST_MATERIAL>(ado, item[0]);
                    ado.AMOUNT = item.Sum(p => p.AMOUNT);
                    ado.TH_AMOUNT = item.Sum(p => p.TH_AMOUNT);
                    if (!result.DicExpMestMaterials.ContainsKey(ado.EXP_MEST_ID ?? 0))
                        result.DicExpMestMaterials[ado.EXP_MEST_ID ?? 0] = new List<HIS_EXP_MEST_MATERIAL>();
                    result.DicExpMestMaterials[ado.EXP_MEST_ID ?? 0].Add(ado);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Loads once the data that does not depend on the rendered trackings and keeps a snapshot
        /// of the service requests, so rendering many sheets one by one does not call the API each time.
        /// </summary>
        internal static void PrepareForManyRenders(TrackingPrintData data, long treatmentId)
        {
            try
            {
                if (data == null)
                    return;
                long id = (data.Treatment != null && data.Treatment.ID > 0) ? data.Treatment.ID : treatmentId;
                data.BedLogs = GetBedLogs(id);
                data.SpecialistExams = GetSpecialistExams(data.Treatment);
                data.PatientTypeAlter = GetPatientTypeAlter(treatmentId);
                data.IsSharedDataLoaded = true;
                data.ServiceReqSnapshot = data.DicServiceReqs != null ? CloneServiceReqs(data.DicServiceReqs.Values).Values.ToList() : new List<HIS_SERVICE_REQ>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Builds the Mps000062 print data object for the given trackings.
        /// </summary>
        internal static MPS.Processor.Mps000062.PDO.Mps000062PDO BuildPdo(TrackingPrintData data, List<V_HIS_TRACKING> trackings, Inventec.Desktop.Common.Modules.Module module, long treatmentId)
        {
            if (data.ServiceReqSnapshot != null)
            {
                data.DicServiceReqs = CloneServiceReqs(data.ServiceReqSnapshot);
            }

            //gán lại ngày y lệnh theo thời gian y lệnh do trong mps sửa ngày
            if (data.DicServiceReqs != null && data.DicServiceReqs.Count > 0)
            {
                data.DicServiceReqs.Values.ToList().ForEach(o => o.INTRUCTION_DATE = o.INTRUCTION_TIME - (o.INTRUCTION_TIME % 1000000));
            }

            List<V_HIS_TREATMENT_BED_ROOM> treatmentBedRooms = new List<V_HIS_TREATMENT_BED_ROOM>();
            if (data.TreatmentBedRooms != null)
            {
                foreach (var item in trackings)
                {
                    var treatmentBedRoom = data.TreatmentBedRooms.Where(o => o.ADD_TIME <= item.TRACKING_TIME && o.ROOM_ID == item.ROOM_ID).OrderByDescending(o => o.ADD_TIME).FirstOrDefault();
                    if (treatmentBedRoom != null)
                        treatmentBedRooms.Add(treatmentBedRoom);
                }
            }

            long keyVienTim = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKING_IS_NUMBER_BY_MEDICINE_TYPE));
            long showKeySTT = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_OPTION));
            long formatShowKeySTT = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_FORMAT_OPTION));
            long usedDayCountingOutStockOption = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_OUT_STOCK_OPTION));
            long usedDayCountingAddPreviousUseDay = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(ConfigKeyss.DBCODE__HIS_DESKTOP_PLUGINS_TRACKINGPRINT_USED_DAY_COUNTING_ADD_PREVIOUS_USE_DAY));

            long roomId = module != null ? module.RoomId : 0;
            MOS.SDO.WorkPlaceSDO workPlaceSDO = WorkPlace.WorkPlaceSDO.SingleOrDefault(p => p.RoomId == roomId);
            MPS.Processor.Mps000062.PDO.Mps000062SingleKey singleKey = new MPS.Processor.Mps000062.PDO.Mps000062SingleKey(workPlaceSDO);
            singleKey.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            singleKey.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            singleKey.IsShowMedicineLine = (HisConfigs.Get<string>(CONFIG_KEY__IS_SHOW_MEDICINE_LINE) == "1");
            singleKey.IsOrderByType = Convert.ToInt64(HisConfigs.Get<string>(CONFIG_KEY__ORDER_OPTION));
            singleKey.keyVienTim = keyVienTim;
            singleKey.UsedDayCountingOption = showKeySTT;
            singleKey.UsedDayCountingFormatOption = formatShowKeySTT;
            singleKey.UsedDayCountingOutStockOption = usedDayCountingOutStockOption;
            singleKey.UsedDayCountingAddPreviousUseDay = usedDayCountingAddPreviousUseDay;

            List<HIS_MEDICINE_LINE> listMedicineLine = BackendDataWorker.Get<HIS_MEDICINE_LINE>();
            List<HIS_DOSAGE_FORM> listDosage = BackendDataWorker.Get<HIS_DOSAGE_FORM>();

            long treatmentIdForBedLog = (data.Treatment != null) ? data.Treatment.ID : treatmentId;
            List<V_HIS_BED_LOG> bedLogs = data.IsSharedDataLoaded ? data.BedLogs : GetBedLogs(treatmentIdForBedLog);
            V_HIS_BED_LOG selectedBedLog = null;
            if (bedLogs != null && trackings.Count > 0)
            {
                var minTrackingTime = trackings.Min(m => m.TRACKING_TIME);
                selectedBedLog = bedLogs
                    .Where(o => o.BED_ID != 0
                        && (o.START_TIME <= minTrackingTime && (o.FINISH_TIME == null || minTrackingTime <= o.FINISH_TIME)))
                    .OrderByDescending(o => o.START_TIME)
                    .ThenByDescending(o => o.ID)
                    .FirstOrDefault();
            }

            List<V_HIS_SPECIALIST_EXAM> specialistExams = data.IsSharedDataLoaded ? data.SpecialistExams : GetSpecialistExams(data.Treatment);
            HIS_PATIENT_TYPE_ALTER patientTypeAlter = data.IsSharedDataLoaded ? data.PatientTypeAlter : GetPatientTypeAlter(treatmentId);

            HashSet<long> serviceIds = new HashSet<long>((data.SereServs ?? new List<HIS_SERE_SERV>()).Select(p => p.SERVICE_ID));
            List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => serviceIds.Contains(o.ID)).ToList();

            MPS.Processor.Mps000062.PDO.Mps000062PDO mps000062RDO = new MPS.Processor.Mps000062.PDO.Mps000062PDO(
                data.Treatment,
                treatmentBedRooms,
                trackings,
                data.Dhsts,
                data.DicServiceReqs,
                data.DicSereServs,
                data.DicExpMests,
                data.DicExpMestMedicines,
                data.DicExpMestMaterials,
                data.DicServiceReqMetys,
                data.DicServiceReqMatys,
                data.Cares,
                data.CareDetails,
                singleKey,
                BackendDataWorker.Get<HIS_ICD>(),
                BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                BackendDataWorker.Get<HIS_SERVICE_TYPE>(),
                data.SereServExts,
                BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                data.SereServRations,
                data.MobaImpMests,
                data.ImpMestMedicinesTL,
                data.ImpMestMaterialsTL,
                data.ExpMestBltyReq2s,
                services,
                data.ImpMestBloodsTL,
                patientTypeAlter,
                listMedicineLine,
                listDosage,
                selectedBedLog
                );
            mps000062RDO._SpecialistExams = specialistExams ?? new List<V_HIS_SPECIALIST_EXAM>();
            return mps000062RDO;
        }

        /// <summary>
        /// Renders one tracking with the Mps000062 template into a document file (no print, no print log).
        /// Returns null when nothing was rendered.
        /// </summary>
        internal static DocumentTrackingADO RenderTracking(TrackingPrintData data, V_HIS_TRACKING tracking, string templateFile, Inventec.Desktop.Common.Modules.Module module, long treatmentId)
        {
            DocumentTrackingADO result = null;
            MemoryStream streamFile = new MemoryStream();
            try
            {
                if (String.IsNullOrWhiteSpace(templateFile))
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay file mau Mps000062 de dung to dieu tri ID = " + tracking.ID);
                    streamFile.Dispose();
                    return null;
                }

                string printTypeCode = PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_TO_DIEU_TRI__MPS000062;
                MPS.Processor.Mps000062.PDO.Mps000062PDO pdo = BuildPdo(data, new List<V_HIS_TRACKING> { tracking }, module, treatmentId);

                string saveFilePath = "";
                string ext = Path.GetExtension(templateFile);
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ext == ".doc" || ext == ".docx")
                {
                    saveFilePath = frmHisTrackingList.GenerateTempFileWithin("", ".docx");
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, templateFile, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, saveFilePath);
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, templateFile, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, streamFile);
                }

                MPS.MpsPrinter.Run(printData);

                saveFilePath = printData.saveFilePath;
                if (printData.saveMemoryStream != null)
                {
                    printData.saveMemoryStream.Position = 0;
                    printData.saveMemoryStream.CopyTo(streamFile);
                }

                if (streamFile.Length > 0 || (!String.IsNullOrEmpty(saveFilePath) && File.Exists(saveFilePath)))
                {
                    streamFile.Position = 0;
                    result = new DocumentTrackingADO()
                    {
                        DocumentFile = streamFile,
                        TRACKING_TIME = tracking.TRACKING_TIME,
                        FullTemplateFileName = templateFile,
                        saveFilePath = saveFilePath
                    };
                }
                else
                {
                    streamFile.Dispose();
                }
            }
            catch (Exception ex)
            {
                result = null;
                streamFile.Dispose();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Builds a one page PDF (A4) that shows the given message, used in place of a sheet that cannot be displayed.
        /// </summary>
        internal static DocumentTrackingADO BuildNoticeDocument(string message, long trackingTime)
        {
            DocumentTrackingADO result = null;
            try
            {
                MemoryStream pdf = new MemoryStream();
                using (DevExpress.XtraRichEdit.RichEditDocumentServer server = new DevExpress.XtraRichEdit.RichEditDocumentServer())
                {
                    server.Document.Sections[0].Page.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    server.Document.DefaultCharacterProperties.FontName = "Times New Roman";
                    server.Document.DefaultCharacterProperties.FontSize = 14;
                    server.Document.AppendText(message ?? "");
                    server.ExportToPdf(pdf);
                }
                pdf.Position = 0;
                result = new DocumentTrackingADO()
                {
                    DocumentFile = pdf,
                    TRACKING_TIME = trackingTime
                };
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Downloads the files of the signed documents (api/EmrDocument/DownloadFile). Returns null when the call fails.
        /// </summary>
        internal static List<EmrDocumentFileSDO> DownloadEmrDocumentFiles(List<long> documentIds, long roomId)
        {
            List<EmrDocumentFileSDO> result = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
                EMR.Filter.EmrDocumentViewFilter emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                emrFilter.IDs = documentIds;
                sdo.EmrDocumentViewFilter = emrFilter;
                sdo.IsMerge = false;
                sdo.IsView = true;
                var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                sdo.RoomCode = room != null ? room.ROOM_CODE : null;
                sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
                result = new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Template file of Mps000062 chosen last time with the "Mẫu in" button of the tracking list on this computer,
        /// otherwise the first template file in the Mps000062 folder.
        /// </summary>
        internal static string GetTemplateFile()
        {
            string result = null;
            try
            {
                List<string> templateFiles = GetTemplateFiles();
                HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                var controlStates = controlStateWorker.GetData(MODULE_LINK);
                var templateState = controlStates != null ? controlStates.FirstOrDefault(o => o.KEY == CONTROL_STATE_KEY__TEMPLATE && o.MODULE_LINK == MODULE_LINK) : null;
                if (templateState != null && !String.IsNullOrWhiteSpace(templateState.VALUE) && File.Exists(templateState.VALUE))
                {
                    result = templateState.VALUE;
                }
                else if (templateFiles != null && templateFiles.Count > 0)
                {
                    result = templateFiles.First();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static List<string> GetTemplateFiles()
        {
            List<string> result = null;
            try
            {
                var folderMps = Path.Combine(Inventec.Common.RichEditor.Base.FileLocalStore.Rootpath, "Mps000062");
                if (Directory.Exists(folderMps))
                {
                    string[] fileEntries = Directory.EnumerateFiles(folderMps, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(s => (s.EndsWith(".xls") || s.EndsWith(".doc") || s.EndsWith(".xlsx") || s.EndsWith(".repx") || s.EndsWith(".docx"))).ToArray();
                    if (fileEntries != null && fileEntries.Count() > 0)
                    {
                        result = fileEntries.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// All trackings of the treatment (api/HisTracking/GetView), no paging. Returns null when the call fails.
        /// </summary>
        internal static List<V_HIS_TRACKING> GetTrackings(long treatmentId)
        {
            List<V_HIS_TRACKING> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisTrackingViewFilter filter = new MOS.Filter.HisTrackingViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.ORDER_FIELD = "TRACKING_TIME";
                filter.ORDER_DIRECTION = "DESC";
                result = new BackendAdapter(param).Get<List<V_HIS_TRACKING>>(HisRequestUriStore.HIS_TRACKING_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                if (result == null)
                    Inventec.Common.Logging.LogSystem.Warn("api/HisTracking/GetView returns null. " + Inventec.Common.Logging.LogUtil.TraceData("param", param));
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Signed treatment sheet documents of the treatment (api/EmrDocument/GetView), filtered as the tracking list does.
        /// isError is true when the call fails with an exception.
        /// </summary>
        internal static List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT> GetSignedDocuments(string treatmentCode, out bool isError)
        {
            List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT> result = null;
            isError = false;
            try
            {
                CommonParam param = new CommonParam();
                var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
                emrFilter.TREATMENT_CODE__EXACT = treatmentCode;
                emrFilter.IS_DELETE = false;
                emrFilter.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING;
                emrFilter.IS_ACTIVE = IMSys.DbConfig.EMR_RS.COMMON.IS_ACTIVE__TRUE;
                result = new BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, emrFilter, param);
                if (result == null && param.HasException)
                {
                    isError = true;
                    Inventec.Common.Logging.LogSystem.Warn("api/EmrDocument/GetView failed. " + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                }
            }
            catch (Exception ex)
            {
                result = null;
                isError = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// PDF file of the signed document of the unit. Returns null when the file was not downloaded or is not a PDF.
        /// </summary>
        internal static DocumentTrackingADO CreateSignedDocument(TrackingDisplayUnit unit, Dictionary<long, EmrDocumentFileSDO> signedFiles)
        {
            DocumentTrackingADO result = null;
            try
            {
                EmrDocumentFileSDO file;
                if (unit != null && unit.Document != null && signedFiles != null
                    && signedFiles.TryGetValue(unit.Document.ID, out file)
                    && file != null
                    && !String.IsNullOrEmpty(file.Base64Data)
                    && (file.Extension ?? "").Trim().TrimStart('.').ToLower() == "pdf")
                {
                    result = new DocumentTrackingADO
                    {
                        DocumentFile = new MemoryStream(Convert.FromBase64String(file.Base64Data)),
                        TRACKING_TIME = unit.Tracking.TRACKING_TIME
                    };
                }
                else if (unit != null && unit.Document != null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc file PDF cua van ban ky ID = " + unit.Document.ID);
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal const string KEY__SHEET_NOT_SHOWN = "UcTrackingListView.SheetNotShown";

        /// <summary>
        /// Message shown in place of a sheet that cannot be displayed: {0} sheet order (empty when not numbered), {1} treatment time.
        /// </summary>
        internal static string FormatSheetNotShown(string format, V_HIS_TRACKING tracking)
        {
            string result = "";
            try
            {
                string sheetOrder = tracking.SHEET_ORDER.HasValue ? tracking.SHEET_ORDER.Value.ToString() : "";
                string trackingTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(tracking.TRACKING_TIME);
                result = String.Format(format ?? "{0} {1}", sheetOrder, trackingTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        internal static string GetText(string key)
        {
            string result = "";
            try
            {
                if (HIS.Desktop.Plugins.HisTrackingList.Resources.ResourceLanguageManager.LanguageResource__UcTrackingListView == null)
                {
                    HIS.Desktop.Plugins.HisTrackingList.Resources.ResourceLanguageManager.LanguageResource__UcTrackingListView = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisTrackingList.Resources.Lang", typeof(TrackingSheetRenderer).Assembly);
                }
                result = Inventec.Common.Resource.Get.Value(key, HIS.Desktop.Plugins.HisTrackingList.Resources.ResourceLanguageManager.LanguageResource__UcTrackingListView, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private static Dictionary<long, HIS_SERVICE_REQ> CloneServiceReqs(IEnumerable<HIS_SERVICE_REQ> serviceReqs)
        {
            Dictionary<long, HIS_SERVICE_REQ> result = new Dictionary<long, HIS_SERVICE_REQ>();
            foreach (var item in serviceReqs)
            {
                if (item == null || result.ContainsKey(item.ID))
                    continue;
                HIS_SERVICE_REQ clone = new HIS_SERVICE_REQ();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_REQ>(clone, item);
                result[clone.ID] = clone;
            }
            return result;
        }

        private static List<V_HIS_BED_LOG> GetBedLogs(long treatmentId)
        {
            CommonParam param = new CommonParam();
            MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
            bedLogViewFilter.TREATMENT_ID = treatmentId;
            return new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogViewFilter, param);
        }

        private static List<V_HIS_SPECIALIST_EXAM> GetSpecialistExams(HIS_TREATMENT treatment)
        {
            // Khoa phong moi hoi chan luu o HIS_SPECIALIST_EXAM (INVITE_TYPE = 2), cot EXAM_EXECUTE_DEPARMENT_ID.
            CommonParam param = new CommonParam();
            List<V_HIS_SPECIALIST_EXAM> result = new List<V_HIS_SPECIALIST_EXAM>();
            MOS.Filter.HisSpecialistExamViewFilter specialistExamViewFilter = new MOS.Filter.HisSpecialistExamViewFilter();
            specialistExamViewFilter.TREATMENT_CODE = (treatment != null ? treatment.TREATMENT_CODE : "");
            specialistExamViewFilter.INVITE_TYPE = 2;
            var specialistExamViews = new BackendAdapter(param).Get<List<V_HIS_SPECIALIST_EXAM>>("api/HisSpecialistExam/GetView", ApiConsumers.MosConsumer, specialistExamViewFilter, param);
            if (specialistExamViews != null && specialistExamViews.Count > 0)
                result = specialistExamViews;
            return result;
        }

        private static HIS_PATIENT_TYPE_ALTER GetPatientTypeAlter(long treatmentId)
        {
            return new BackendAdapter(new CommonParam()).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, null);
        }
    }
}
