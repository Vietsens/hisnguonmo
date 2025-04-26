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
using ACS.EFMODEL.DataModels;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPaan.Config;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MPS.ProcessorBase.PrintConfig;

namespace HIS.Desktop.Plugins.AssignPaan
{
    public partial class frmAssignPaan : HIS.Desktop.Utility.FormBase
    {
        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;

                if (!(bool)icdProcessor.ValidationIcd(ucIcd))
                    return;

                if (!btnSavePrint.Enabled || !dxValidationProvider1.Validate() || this.treatment == null)
                    return;

                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.ProcessSave(ref param, ref success);
                WaitingManager.Hide();
                if (success)
                {
                    this.InPhieuYeuCauXetNghiemGiaiPhauBenhPham();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;

                if (!(bool)icdProcessor.ValidationIcd(ucIcd))
                    return;
                if (!subIcdProcessor.GetValidate(ucSecondaryIcd))
                    return;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.treatment == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.ProcessSave(ref param, ref success);
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultSDO == null)
                    return;
                this.InPhieuYeuCauXetNghiemGiaiPhauBenhPham();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefersh_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRefersh.Enabled)
                    return;
                this.ResetControlValue();
                btnSave.Enabled = true;
                btnSavePrint.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void bbtnRCRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefersh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSave(ref CommonParam param, ref bool success)
        {
            try
            {
                HisPaanServiceReqSDO serviceReqSDO = new HisPaanServiceReqSDO();
                if (this.treatment != null)
                {
                    serviceReqSDO.TreatmentId = this.treatment.ID;
                }
                var icdData = (HIS.UC.Icd.ADO.IcdInputADO)this.icdProcessor.GetValue(this.ucIcd);

                LogUtil.TraceData("serviceReqSDO____________________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => icdData), icdData);
                serviceReqSDO.IcdCode = icdData.ICD_CODE;
                serviceReqSDO.IcdName = icdData.ICD_NAME;

                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)
                    {
                        serviceReqSDO.IcdSubCode = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        serviceReqSDO.IcdText = ((HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }



                if (!String.IsNullOrEmpty(txtDescription.Text))
                {
                    serviceReqSDO.Description = txtDescription.Text;
                }
                if (checkIsSurgery.Checked)
                {
                    serviceReqSDO.IsMergency = (short)1;
                }
                serviceReqSDO.InstructionTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtInstructionTime.DateTime) ?? 0;
                serviceReqSDO.RequestRoomId = this.currentModule != null ? this.currentModule.RoomId : 0;

                if (this.cboTracking.EditValue != null)
                {
                    serviceReqSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboTracking.EditValue.ToString());
                }

                ServiceReqDetailSDO detailSDO = new ServiceReqDetailSDO();
                serviceReqSDO.Amount = 1;
                serviceReqSDO.PatientTypeId = Convert.ToInt64(cboPatientType.EditValue);
                serviceReqSDO.RoomId = Convert.ToInt64(cboExecuteRoom.EditValue);
                serviceReqSDO.ServiceId = Convert.ToInt64(cboPaanServiceType.EditValue);
                if (cboPaanPosition.EditValue != null)
                {
                    serviceReqSDO.PaanPositionId = Convert.ToInt64(cboPaanPosition.EditValue);
                }
                if (cboPaanLiquid.EditValue != null)
                {
                    serviceReqSDO.PaanLiquidId = Convert.ToInt64(cboPaanLiquid.EditValue);
                    serviceReqSDO.LiquidTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtLiquidTime.DateTime);
                }
                if (Config.AppConfig.ShowRequestUser == "1" && cboUsername.EditValue != null)
                {
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboUsername.EditValue));
                    if (user != null)
                    {
                        serviceReqSDO.RequestLoginName = user.LOGINNAME;
                        serviceReqSDO.RequestUserName = user.USERNAME;
                    }
                }
                if (this._sereServParentId > 0)
                {
                    serviceReqSDO.SereServParentId = this._sereServParentId;
                }
                if (this.serviceReqId > 0)
                {
                    serviceReqSDO.ParentServiceReqId = this.serviceReqId;
                }
                if (cboTestSampleType.EditValue != null)
                    serviceReqSDO.TestSampleTypeId = Int64.Parse(cboTestSampleType.EditValue.ToString());
                LogUtil.TraceData("serviceReqSDO____________________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqSDO), serviceReqSDO);
                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisServiceReqResultSDO>("api/HisServiceReq/PaanCreate", ApiConsumers.MosConsumer, serviceReqSDO, param);
                if (rs != null)
                {
                    LogUtil.TraceData("rs____________________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs);
                    success = true;
                    resultSDO = rs;
                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    btnPrint.Enabled = true;
                    if(delegateActionSave != null)
                    {
                        delegateActionSave();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        void InPhieuYeuCauXetNghiemGiaiPhauBenhPham()
        {
            try
            {
                if (this.treatment == null || this.resultSDO == null)
                    return;

                InPhieuYeuCauDichVu(MPS_000167);
                //Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                //store.RunPrintTemplate(MPS_000167, delegateRunPrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //get lại config
        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }

        //bool delegateRunPrintTemplate(string printTypeCode, string fileName)
        //{
        //    bool result = false;
        //    try
        //    {
        //        HisServiceReqViewFilter paanFilter = new HisServiceReqViewFilter();
        //        paanFilter.ID = resultSDO.ServiceReq.ID;
        //        var listPaan = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, paanFilter, null);
        //        if (listPaan == null || listPaan.Count != 1)
        //        {
        //            throw new Exception("Khong lay duoc HIS_SERVICE_REQ theo id" + resultSDO.ServiceReq.ID);
        //        }
        //        var paanServiceReq = listPaan.FirstOrDefault();

        //        var paanLiquilds = BackendDataWorker.Get<HIS_PAAN_LIQUID>();
        //        HIS_PAAN_LIQUID paanLiquild = paanLiquilds.SingleOrDefault(o => o.ID == resultSDO.ServiceReq.PAAN_LIQUID_ID);
        //        var paanPositions = BackendDataWorker.Get<HIS_PAAN_POSITION>();
        //        HIS_PAAN_POSITION paanPosition = paanPositions.SingleOrDefault(o => o.ID == resultSDO.ServiceReq.PAAN_POSITION_ID);

        //        HisSereServView5Filter ssFilter = new HisSereServView5Filter();
        //        ssFilter.ID = resultSDO.SereServs.FirstOrDefault().ID;
        //        var listSereServ = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
        //        if (listSereServ == null || listSereServ.Count != 1)
        //        {
        //            throw new Exception("Khong lay duoc V_HIS_SERE_SERV_5 theo Id: " + resultSDO.SereServs.FirstOrDefault().ID);
        //        }
        //        var sereServPrint = listSereServ.FirstOrDefault();

        //        int SetDefaultDepositPrice = Inventec.Common.TypeConvert.Parse.ToInt32(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SdaConfigKey.HIS_DEPOSIT__DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT));
        //        if (SetDefaultDepositPrice == 1)
        //        {
        //            if (currentPatientTypeAlter != null && currentPatientTypeAlter.PATIENT_TYPE_ID == GetId(HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")) && currentPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
        //            {
        //                V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
        //                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV>(sereServ, sereServPrint);
        //                sereServPrint.VIR_PRICE = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ);
        //            }
        //        }
        //        MPS.Processor.Mps000167.PDO.Mps000167ADO mps167ADO = new MPS.Processor.Mps000167.PDO.Mps000167ADO();
        //        if (paanLiquild != null)
        //        {
        //            mps167ADO.PAAN_LIQUID_CODE = paanLiquild.PAAN_LIQUID_CODE;
        //            mps167ADO.PAAN_LIQUID_NAME = paanLiquild.PAAN_LIQUID_NAME;
        //            mps167ADO.PAAN_POSITION_CODE = paanPosition.PAAN_POSITION_CODE;
        //            mps167ADO.PAAN_POSITION_NAME = paanPosition.PAAN_POSITION_NAME;
        //        }

        //        CommonParam param = new CommonParam();

        //        HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
        //        treatmentFilter.ID = resultSDO.ServiceReq.TREATMENT_ID;
        //        var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();

        //        mps167ADO.KSK_ORDER = treatment.KSK_ORDER;

        //        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((paanServiceReq != null ? paanServiceReq.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);
        //        MPS.Processor.Mps000167.PDO.Mps000167PDO rdo = new MPS.Processor.Mps000167.PDO.Mps000167PDO(
        //            paanServiceReq,
        //            sereServPrint,
        //            currentPatientTypeAlter,
        //            mps167ADO);
        //        if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO });
        //        }
        //        else
        //        {
        //            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "") { EmrInputADO = inputADO });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        result = false;
        //    }
        //    return result;
        //}

        private void InPhieuYeuCauDichVu(string printTypeCode)
        {
            try
            {
                HisServiceReqViewFilter paanFilter = new HisServiceReqViewFilter();
                paanFilter.ID = resultSDO.ServiceReq.ID;
                var listPaan = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, paanFilter, null);
                if (listPaan == null || listPaan.Count != 1)
                {
                    throw new Exception("Khong lay duoc HIS_SERVICE_REQ theo id" + resultSDO.ServiceReq.ID);
                }
                var currentServiceReq = listPaan.FirstOrDefault();

                CommonParam param = new CommonParam();
                if (currentServiceReq != null)
                {
                    var sdo = new HisServiceReqListResultSDO();
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("private void InPhieuYeuCau(string printTypeCode, bool printNow) currentServiceReq ", currentServiceReq));
                    if (currentServiceReq.TEST_SAMPLE_TYPE_ID.HasValue)
                    {
                        var testSampleType = BackendDataWorker.Get<HIS_TEST_SAMPLE_TYPE>().FirstOrDefault(o => o.ID == currentServiceReq.TEST_SAMPLE_TYPE_ID.Value);
                        currentServiceReq.TEST_SAMPLE_TYPE_CODE = testSampleType != null ? testSampleType.TEST_SAMPLE_TYPE_CODE : "";
                        currentServiceReq.TEST_SAMPLE_TYPE_NAME = testSampleType != null ? testSampleType.TEST_SAMPLE_TYPE_NAME : "";
                    }
                    sdo.ServiceReqs = new List<V_HIS_SERVICE_REQ>() { currentServiceReq };
                    sdo.SereServs = resultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == currentServiceReq.ID).ToList();

                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = resultSDO.ServiceReq.TREATMENT_ID;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GET, ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).FirstOrDefault();

                    HisTreatmentWithPatientTypeInfoSDO currentHisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(currentHisTreatment, treatment);

                    if (currentPatientTypeAlter != null)
                    {
                        currentHisTreatment.PATIENT_TYPE_CODE = currentPatientTypeAlter.PATIENT_TYPE_CODE;
                        currentHisTreatment.HEIN_CARD_FROM_TIME = currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                        currentHisTreatment.HEIN_CARD_NUMBER = currentPatientTypeAlter.HEIN_CARD_NUMBER;
                        currentHisTreatment.HEIN_CARD_TO_TIME = currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                        currentHisTreatment.HEIN_MEDI_ORG_CODE = currentPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                        currentHisTreatment.LEVEL_CODE = currentPatientTypeAlter.LEVEL_CODE;
                        currentHisTreatment.RIGHT_ROUTE_CODE = currentPatientTypeAlter.RIGHT_ROUTE_CODE;
                        currentHisTreatment.RIGHT_ROUTE_TYPE_CODE = currentPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                        currentHisTreatment.TREATMENT_TYPE_CODE = currentPatientTypeAlter.TREATMENT_TYPE_CODE;
                        currentHisTreatment.HEIN_CARD_ADDRESS = currentPatientTypeAlter.ADDRESS;
                    }

                    // get bedLog
                    List<V_HIS_BED_LOG> bedLogs = new List<V_HIS_BED_LOG>();
                    if (currentServiceReq != null && currentHisTreatment != null)
                    {
                        MOS.Filter.HisBedLogViewFilter bedLogViewFilter = new MOS.Filter.HisBedLogViewFilter();
                        bedLogViewFilter.DEPARTMENT_IDs = new List<long>() { currentServiceReq.REQUEST_DEPARTMENT_ID };
                        bedLogViewFilter.TREATMENT_ID = currentHisTreatment.ID;
                        bedLogs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, bedLogViewFilter, param);
                    }

                    
                    MPS.ProcessorBase.PrintConfig.PreviewType previewType;
                    //if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    printNow = true;
                    //}
                    //else
                    //{
                    //    printNow = false;
                    //}
                    bool printNow = false;
                    var allowSignValue = HisPatientTypeCFG.IsAllowSignaturePrint;
                    var allowedModules = allowSignValue
                            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => x.Trim())
                            .ToList();
                    if (!string.IsNullOrWhiteSpace(allowSignValue) && allowedModules.Contains("HIS.Desktop.Plugins.AssignPaan"))
                    {                                                
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintPreview;                        
                    }
                    else
                    {
                        previewType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
                    }
                    var PrintServiceReqProcessors = new Library.PrintServiceReq.PrintServiceReqProcessor(sdo, currentHisTreatment, bedLogs, currentModule != null ? currentModule.RoomId : 0, previewType);
                    PrintServiceReqProcessors.Print(printTypeCode, printNow);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
