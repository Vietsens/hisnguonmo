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
using DevExpress.Utils.Menu;
using HIS.Desktop.Plugins.DepositService.DepositService;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.Filter;
using HIS.Desktop.Utility;
using System.Reflection;

namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    // in ấn
    public partial class frmDepositService : HIS.Desktop.Utility.FormBase
    {
        public void InitMenuToButtonPrint()
        {
            try
            {
                DXPopupMenu menuMedicine = new DXPopupMenu();
                //TODO da ngon ngu
                DXMenuItem itemPhieuTamUng = new DXMenuItem("In phiếu tạm ứng", new EventHandler(OnClickInPhieuTamUng));
                itemPhieuTamUng.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000109;
                menuMedicine.Items.Add(itemPhieuTamUng);

                DXMenuItem itemPhieuTamUngTheoDichVu = new DXMenuItem("In phiếu tạm ứng theo dịch vụ", new EventHandler(OnClickInPhieuTamUng));
                itemPhieuTamUngTheoDichVu.Tag = PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102;
                menuMedicine.Items.Add(itemPhieuTamUngTheoDichVu);

                ddbPrint.DropDownControl = menuMedicine;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnClickInPhieuTamUng(object sender, EventArgs e)
        {
            try
            {
                this.isPrintNow = false;
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                var btnItem = sender as DXMenuItem;
                string type = (string)(btnItem.Tag);
                switch (type)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000109:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000109, DelegateRunPrinter);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000109:
                        InPhieuTamUng(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102:
                        InPhieuThuPhiDichVu(printTypeCode, fileName, ref result, this.isPrintNow, this.isSign);
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

        public V_HIS_SERVICE_REQ FirstExamRoom()
        {

            V_HIS_SERVICE_REQ result = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisServiceReqViewFilter serviceReqFilter = new HisServiceReqViewFilter();

                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;

                serviceReqFilter.TREATMENT_ID = this.hisTreatment.ID;

                var listServiceReq = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERVICE_REQ>>(HisRequestUriStore.HIS_SERVICE_REQ_GETVIEW, ApiConsumers.MosConsumer, serviceReqFilter, paramCommon);
                if (listServiceReq != null && listServiceReq.Count > 0)
                {
                    result = listServiceReq.OrderBy(o => o.CREATE_TIME).FirstOrDefault();
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void InPhieuThuPhiDichVu(string printTypeCode, string fileName, ref bool result, bool isPrintNow, bool isSign)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServDepositFilter dereDetailFiter = new MOS.Filter.HisSereServDepositFilter();
                dereDetailFiter.DEPOSIT_ID = this.hisDeposit.ID;
                var dereDetails = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, dereDetailFiter, param);
                var sereServIds = dereDetails.Select(o => o.SERE_SERV_ID).ToList();

                MOS.Filter.HisSereServView12Filter sereServFilter = new MOS.Filter.HisSereServView12Filter();
                sereServFilter.TREATMENT_ID = this.hisTreatment.ID;
                sereServFilter.IDs = sereServIds;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_12>>("api/HisSereServ/GetView12", ApiConsumers.MosConsumer, sereServFilter, param);
                foreach (var item in sereServs)
                {
                    var itemCheck = dereDetails.FirstOrDefault(o => o.SERE_SERV_ID == item.ID);
                    if (itemCheck != null)
                    {
                        item.VIR_TOTAL_PATIENT_PRICE = itemCheck.AMOUNT;
                    }
                }
                DepositServicePrintProcess.LoadPhieuThuPhiDichVu(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, fileName, true, sereServs, this.hisTreatment, sereServIds, this.hisDeposit, dereDetails, FirstExamRoom(), isPrintNow, isSign, this.moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public PatientADO getPatient(long treatmentId)
        {
            PatientADO currentPatientADO = new PatientADO();
            MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = new V_HIS_PATIENT();
            MOS.EFMODEL.DataModels.V_HIS_TREATMENT currentHisTreatment = new MOS.EFMODEL.DataModels.V_HIS_TREATMENT();
            try
            {
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = treatmentId;
                CommonParam param = new CommonParam();
                var treatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param);
                if (treatments != null && treatments.Count > 0)
                {
                    currentHisTreatment = treatments.FirstOrDefault();

                    MOS.Filter.HisPatientViewFilter patientViewFilter = new MOS.Filter.HisPatientViewFilter();
                    patientViewFilter.ID = currentHisTreatment.PATIENT_ID;
                    patient = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientViewFilter, param).SingleOrDefault();

                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>();
                    currentPatientADO = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_PATIENT, PatientADO>(patient);
                    //currentPatientADO.CMND_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.CMND_DATE ?? 0);
                    currentPatientADO.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientADO.DOB);
                    currentPatientADO.AGE = HIS.Desktop.Utility.AgeHelper.CalculateAgeFromYear(currentPatientADO.DOB);
                    if (currentPatientADO != null && currentPatientADO.DOB > 0)
                    {
                        currentPatientADO.DOB_YEAR = currentPatientADO.DOB.ToString().Substring(0, 4);
                    }

                    if (currentPatientADO.GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        currentPatientADO.GENDER_MALE = "";
                        currentPatientADO.GENDER_FEMALE = "X";
                    }
                    else
                    {
                        currentPatientADO.GENDER_MALE = "X";
                        currentPatientADO.GENDER_FEMALE = "";
                    }
                }
            }
            catch (Exception ex)
            {
                currentPatientADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentPatientADO;
        }

        private void InPhieuTamUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                if (this.hisDeposit == null)
                {
                    MessageManager.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.ThongBaoDuLieuTrong));
                    return;
                }
                CommonParam param = new CommonParam();
                MPS.Processor.Mps000109.PDO.PatientADO mpsPatientAdo = new MPS.Processor.Mps000109.PDO.PatientADO();
                var patientADO = getPatient(this.hisDeposit.TREATMENT_ID ?? 0);
                MOS.Filter.HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                patientTypeFilter.TREATMENT_ID = this.hisDeposit.TREATMENT_ID ?? 0;
                var patientTypeAlters = new BackendAdapter(param).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, param);
                if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                {
                    var patientTypeBhytCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                    patientTypeAlter = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                }
                if (patientADO != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000109.PDO.PatientADO>(mpsPatientAdo, patientADO);
                }

                MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_5 transaction5 = new V_HIS_TRANSACTION_5();
                AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION, MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_5>();
                transaction5 = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION_5>(this.hisDeposit);

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((hisTreatment != null ? hisTreatment.TREATMENT_CODE : ""), printTypeCode, moduleData.RoomId);
                MPS.Processor.Mps000109.PDO.Mps000109PDO mps000109RDO = new MPS.Processor.Mps000109.PDO.Mps000109PDO(
                    mpsPatientAdo,
                    patientTypeAlter,
                    transaction5,
                    this.hisTreatment
               );
                MPS.ProcessorBase.Core.PrintData printData = null;
                WaitingManager.Hide();
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000109RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null) { EmrInputADO = inputADO };
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000109RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null) { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(printData);
                //if (result)
                //    this.Close();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
