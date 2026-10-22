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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.ADO;
using HIS.Desktop.Plugins.PublicMedicineMultiPatient.Base;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000522.PDO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HIS.Desktop.Plugins.PublicMedicineMultiPatient
{
    public partial class frmPublicMedicineMultiPatient : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Day pages of all checked patients, in grid order, built before the template is chosen</summary>
        List<PrintItemADO> printItems;

        private string MessageTitle
        {
            get { return HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao); }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                this.btnCreate.Enabled = false;
                if (!ValidateBeforeCreate())
                    return;

                PublicMedicineFilterADO option = MakeFilterOption();
                if (!PreparePrintItems(GetCheckedTreatments(), option))
                    return;

                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_CONG_KHAI_THUOC_NHIEU_BN__MPS000522, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.btnCreate.Enabled = true;
            }
        }

        private bool ValidateBeforeCreate()
        {
            string message = null;
            if (GetCheckedTreatments().Count == 0)
            {
                message = Resources.ResourceMessage.ChuaChonBenhNhan;
            }
            else if (!this.chkMedicine.Checked && !this.chkMaterial.Checked && !this.chkBlood.Checked && !this.chkChemical.Checked)
            {
                message = Resources.ResourceMessage.ChuaChonLoaiCongKhai;
            }
            else if (this.dtFromTime.EditValue == null || this.dtFromTime.DateTime == DateTime.MinValue
                || this.dtToTime.EditValue == null || this.dtToTime.DateTime == DateTime.MinValue)
            {
                message = Resources.ResourceMessage.ChuaNhapThoiGian;
            }
            else if (this.dtFromTime.DateTime > this.dtToTime.DateTime)
            {
                message = Resources.ResourceMessage.TaoTuLonHonTaoDen;
            }

            if (!String.IsNullOrEmpty(message))
            {
                XtraMessageBox.Show(message, MessageTitle);
                return false;
            }
            return true;
        }

        private PublicMedicineFilterADO MakeFilterOption()
        {
            PublicMedicineFilterADO option = new PublicMedicineFilterADO();
            DateTime from = this.dtFromTime.DateTime;
            DateTime to = this.dtToTime.DateTime;
            // Times are picked to the minute: from hh:mm:00 to hh:mm:59
            option.TimeFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(new DateTime(from.Year, from.Month, from.Day, from.Hour, from.Minute, 0)) ?? 0;
            option.TimeTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(new DateTime(to.Year, to.Month, to.Day, to.Hour, to.Minute, 59)) ?? 0;
            if (this.cboDepartment.EditValue != null && this.cboDepartment.EditValue.ToString() != "")
                option.RequestDepartmentId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboDepartment.EditValue.ToString());
            if (this.cboRoom.EditValue != null && this.cboRoom.EditValue.ToString() != "")
                option.RequestRoomId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboRoom.EditValue.ToString());
            // ĐTTT "Tất cả" or nothing selected = no filter
            if (this.patientTypeSelecteds != null && this.patientTypeADOs != null && this.patientTypeSelecteds.Count > 0 && this.patientTypeSelecteds.Count < this.patientTypeADOs.Count)
                option.PatientTypeIds = this.patientTypeSelecteds.Select(o => o.ID).ToList();
            option.IsMedicine = this.chkMedicine.Checked;
            option.IsMaterial = this.chkMaterial.Checked;
            option.IsBlood = this.chkBlood.Checked;
            option.IsChemical = this.chkChemical.Checked;
            option.IsExpend = this.chkIsExpend.Checked;
            option.IsTachHDSD = this.chkTachHDSD.Checked;
            return option;
        }

        /// <summary>
        /// Loads the data of all checked patients at once and builds one Mps000522PDO per day page of each patient.
        /// Patients without any row are reported, the others are printed.
        /// </summary>
        private bool PreparePrintItems(List<TreatmentBedRoomADO> treatments, PublicMedicineFilterADO option)
        {
            this.printItems = new List<PrintItemADO>();
            try
            {
                WaitingManager.Show();
                PublicMedicineDataLoader loader = new PublicMedicineDataLoader();
                List<long> treatmentIds = treatments.Select(o => o.TREATMENT_ID).ToList();
                List<HIS_SERVICE_REQ> serviceReqs = loader.GetServiceReqs(treatmentIds, option);
                List<long> serviceReqIds = serviceReqs.Select(o => o.ID).ToList();
                List<HIS_EXP_MEST> expMests = loader.GetExpMests(serviceReqIds);
                List<V_HIS_SERE_SERV> sereServs = loader.GetSereServs(serviceReqIds);
                List<long> expMestIds = expMests.Select(o => o.ID).ToList();
                List<V_HIS_EXP_MEST_MEDICINE> medicines = option.IsMedicine ? loader.GetExpMestMedicines(expMestIds, option.PatientTypeIds) : new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> materials = (option.IsMaterial || option.IsChemical) ? loader.GetExpMestMaterials(expMestIds, option.PatientTypeIds) : new List<V_HIS_EXP_MEST_MATERIAL>();
                List<V_HIS_EXP_MEST_BLOOD> bloods = option.IsBlood ? loader.GetExpMestBloods(expMestIds, option.PatientTypeIds) : new List<V_HIS_EXP_MEST_BLOOD>();
                if (loader.HasError)
                {
                    WaitingManager.Hide();
                    ShowLoadError(loader);
                    return false;
                }

                Dictionary<long, List<ExpMestMediAndMateADO>> rowsByTreatment = PublicMedicineRowBuilder.Build(serviceReqs, expMests, sereServs, medicines, materials, bloods, option);
                Dictionary<long, List<Mps000522ADO>> pagesByTreatment = new Dictionary<long, List<Mps000522ADO>>();
                List<string> noDataPatientNames = new List<string>();
                foreach (TreatmentBedRoomADO treatment in treatments)
                {
                    List<ExpMestMediAndMateADO> rows = null;
                    rowsByTreatment.TryGetValue(treatment.TREATMENT_ID, out rows);
                    List<Mps000522ADO> pages = PublicMedicinePageBuilder.Build(rows, option.IsTachHDSD, Config.Config.CongKhaiThuocDaySize);
                    if (pages.Count > 0)
                        pagesByTreatment[treatment.TREATMENT_ID] = pages;
                    else
                        noDataPatientNames.Add(treatment.TDL_PATIENT_NAME);
                }

                if (pagesByTreatment.Count == 0)
                {
                    WaitingManager.Hide();
                    XtraMessageBox.Show(Resources.ResourceMessage.KhongCoDuLieuCongKhai, MessageTitle);
                    return false;
                }

                List<long> printTreatmentIds = pagesByTreatment.Keys.ToList();
                Dictionary<long, V_HIS_TREATMENT> dicTreatment = loader.GetTreatments(printTreatmentIds)
                    .GroupBy(o => o.ID)
                    .ToDictionary(o => o.Key, o => o.First());
                List<V_HIS_BED_LOG> bedLogs = loader.GetBedLogs(printTreatmentIds);
                if (loader.HasError)
                {
                    WaitingManager.Hide();
                    ShowLoadError(loader);
                    return false;
                }

                // Active ingredients of the medicines on the forms; a failure keeps printing without them (as PublicMedicineByPhased)
                List<long> medicineTypeIds = rowsByTreatment.Values
                    .SelectMany(o => o)
                    .Where(o => o.Service_Type_Id == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC && o.MEDICINE_TYPE_ID > 0)
                    .Select(o => o.MEDICINE_TYPE_ID)
                    .Distinct()
                    .ToList();
                List<V_HIS_MEDICINE_TYPE_ACIN> medicineTypeAcins = new PublicMedicineDataLoader().GetMedicineTypeAcins(medicineTypeIds);

                SingleKeys singleKeys = MakeSingleKeys(option);
                foreach (TreatmentBedRoomADO treatment in treatments)
                {
                    List<Mps000522ADO> pages = null;
                    if (!pagesByTreatment.TryGetValue(treatment.TREATMENT_ID, out pages))
                        continue;
                    V_HIS_TREATMENT vTreatment = null;
                    if (!dicTreatment.TryGetValue(treatment.TREATMENT_ID, out vTreatment))
                    {
                        Inventec.Common.Logging.LogSystem.Warn("PublicMedicineMultiPatient: khong lay duoc V_HIS_TREATMENT ID = " + treatment.TREATMENT_ID);
                        continue;
                    }
                    V_HIS_BED_LOG bedLog = bedLogs
                        .Where(o => o.TREATMENT_ID == treatment.TREATMENT_ID && o.BED_ROOM_ID == treatment.BED_ROOM_ID)
                        .OrderByDescending(o => o.START_TIME)
                        .FirstOrDefault();
                    foreach (Mps000522ADO page in pages)
                    {
                        Mps000522PDO pdo = new Mps000522PDO(vTreatment, new List<Mps000522ADO> { page }, page.Mps000522ByMediEndMateADOs, singleKeys, bedLog);
                        pdo._MedicineTypeAcins = medicineTypeAcins;
                        PrintItemADO item = new PrintItemADO();
                        item.TreatmentId = treatment.TREATMENT_ID;
                        item.TreatmentCode = vTreatment.TREATMENT_CODE;
                        item.PatientName = treatment.TDL_PATIENT_NAME;
                        item.Pdo = pdo;
                        this.printItems.Add(item);
                    }
                }
                WaitingManager.Hide();

                if (noDataPatientNames.Count > 0)
                {
                    XtraMessageBox.Show(String.Format(Resources.ResourceMessage.BenhNhanKhongCoDuLieu, String.Join(", ", noDataPatientNames)), MessageTitle);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.printItems = new List<PrintItemADO>();
            }
            return this.printItems.Count > 0;
        }

        private SingleKeys MakeSingleKeys(PublicMedicineFilterADO option)
        {
            SingleKeys singleKeys = new SingleKeys();
            if (option.RequestDepartmentId.HasValue)
            {
                HIS_DEPARTMENT department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == option.RequestDepartmentId.Value);
                singleKeys.REQUEST_DEPARTMENT_NAME = department != null ? department.DEPARTMENT_NAME : null;
            }
            else if (WorkPlace.WorkPlaceSDO != null && this.currentModule != null)
            {
                var workplace = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.currentModule.RoomId);
                singleKeys.REQUEST_DEPARTMENT_NAME = workplace != null ? workplace.DepartmentName : null;
            }
            singleKeys.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            singleKeys.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
            singleKeys.IsTachHDSD = option.IsTachHDSD;
            singleKeys.IsOderMedicine = this.chkSortName.Checked ? SortOrderConstant.BY_NAME : SortOrderConstant.BY_SETTING;
            return singleKeys;
        }

        private void ShowLoadError(PublicMedicineDataLoader loader)
        {
            string message = Resources.ResourceMessage.KhongTaiDuocDuLieu;
            if (loader.Param.Messages != null && loader.Param.Messages.Count > 0)
                message += "\r\n" + String.Join("\r\n", loader.Param.Messages.Distinct());
            XtraMessageBox.Show(message, MessageTitle);
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__PHIEU_CONG_KHAI_THUOC_NHIEU_BN__MPS000522:
                        result = PrintMps000522(printTypeCode, fileName);
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

        /// <summary>
        /// Ký: one signed document per day page of each patient, no merged preview.
        /// In ngay (CheDoInChoCacChucNangTrongPhanMem = 2): print every page directly.
        /// Otherwise: all pages of all patients in one preview, each page starting a new sheet.
        /// </summary>
        private bool PrintMps000522(string printTypeCode, string fileName)
        {
            if (this.printItems == null || this.printItems.Count == 0)
                return false;
            if (this.chkSign.Checked)
                return SignEachPatient(printTypeCode, fileName);
            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == PrintModeConstant.PRINT_NOW)
                return PrintEachNow(printTypeCode, fileName);
            return ShowMergedPreview(printTypeCode, fileName);
        }

        private Inventec.Common.SignLibrary.ADO.InputADO GetEmrInput(Dictionary<long, Inventec.Common.SignLibrary.ADO.InputADO> dicInput, PrintItemADO item, string printTypeCode)
        {
            Inventec.Common.SignLibrary.ADO.InputADO inputADO = null;
            if (!dicInput.TryGetValue(item.TreatmentId, out inputADO))
            {
                inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(item.TreatmentCode, printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
                dicInput[item.TreatmentId] = inputADO;
            }
            return inputADO;
        }

        private bool SignEachPatient(string printTypeCode, string fileName)
        {
            int successCount = 0;
            List<string> failedPatientNames = new List<string>();
            try
            {
                Dictionary<long, Inventec.Common.SignLibrary.ADO.InputADO> dicInput = new Dictionary<long, Inventec.Common.SignLibrary.ADO.InputADO>();
                MPS.ProcessorBase.PrintConfig.PreviewType previewType = this.chkPrintDocumentSigned.Checked
                    ? MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow
                    : MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                foreach (PrintItemADO item in this.printItems)
                {
                    bool success = false;
                    try
                    {
                        MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, item.Pdo, previewType, "") { EmrInputADO = GetEmrInput(dicInput, item, printTypeCode) };
                        success = MPS.MpsPrinter.Run(printData);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    if (success)
                        successCount++;
                    else if (!failedPatientNames.Contains(item.PatientName))
                        failedPatientNames.Add(item.PatientName);
                }

                string message = String.Format(Resources.ResourceMessage.KetQuaKySo, successCount, this.printItems.Count);
                if (failedPatientNames.Count > 0)
                    message += "\r\n" + String.Format(Resources.ResourceMessage.BenhNhanKyLoi, String.Join(", ", failedPatientNames));
                XtraMessageBox.Show(message, MessageTitle);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return successCount > 0;
        }

        private bool PrintEachNow(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                Dictionary<long, Inventec.Common.SignLibrary.ADO.InputADO> dicInput = new Dictionary<long, Inventec.Common.SignLibrary.ADO.InputADO>();
                foreach (PrintItemADO item in this.printItems)
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, item.Pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = GetEmrInput(dicInput, item, printTypeCode) };
                    if (MPS.MpsPrinter.Run(printData))
                        result = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Renders every page to memory (PreviewType.SaveFile) then shows them in one preview,
        /// the same way as the merged print of HIS.Desktop.Plugins.Library.PrintServiceReq.
        /// No EMR input is given to the merged preview: its pages belong to different treatments.
        /// </summary>
        private bool ShowMergedPreview(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo> parts = new List<Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo>();
                foreach (PrintItemADO item in this.printItems)
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, item.Pdo, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "") { saveMemoryStream = new MemoryStream() };
                    if (!MPS.MpsPrinter.Run(printData) || printData.saveMemoryStream == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("PublicMedicineMultiPatient: khong tao duoc phieu cua ho so " + item.TreatmentCode);
                        continue;
                    }

                    Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo part = new Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo();
                    part.printTypeCode = printData.printTypeCode;
                    part.fileName = printData.fileName;
                    part.data = printData.data;
                    part.printerName = printData.printerName;
                    part.numCopy = printData.numCopy;
                    part.saveFilePath = printData.saveFilePath;
                    part.isAllowExport = printData.isAllowExport;
                    part.eventLog = printData.eventLog;
                    part.eventPrint = printData.eventPrint;
                    part.ShowTutorialModule = printData.ShowTutorialModule;
                    part.IsAllowEditTemplateFile = true;
                    printData.saveMemoryStream.Position = 0;
                    part.saveMemoryStream = new MemoryStream();
                    printData.saveMemoryStream.CopyTo(part.saveMemoryStream);
                    printData.saveMemoryStream.Dispose();
                    parts.Add(part);
                }
                WaitingManager.Hide();

                if (parts.Count == 0)
                    return false;

                Inventec.Common.FlexCelPrint.Ado.PrintMergeAdo first = parts.First();
                Inventec.Common.Print.FlexCelPrintProcessor printProcess = new Inventec.Common.Print.FlexCelPrintProcessor(first.saveMemoryStream, first.printerName, first.fileName, first.numCopy, true,
                    first.isAllowExport, first.TemplateKey, first.eventLog, first.eventPrint, null, first.PrintLog, first.ShowPrintLog, first.IsAllowEditTemplateFile);
                printProcess.SetPartialFile(parts);
                printProcess.PrintPreviewShow();
                result = true;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
