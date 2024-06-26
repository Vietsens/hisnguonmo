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
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Core;
using MPS;
using DevExpress.XtraBars;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using AutoMapper;
using Inventec.Common.LocalStorage.SdaConfig;
using MPS.Processor.Mps000001.PDO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using DevExpress.Utils.Menu;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        DevExpress.XtraBars.BarManager barManager = new DevExpress.XtraBars.BarManager();
        PopupMenu menu;
        private enum PrintType
        {
            InDvKham,
            InTheBenhNhan,
            InPhieuYeuCauKham
        }

        private void PrintProcess(MOS.SDO.HisServiceReqExamRegisterResultSDO data)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__SERVICE_REQ_REGISTER, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
        {
            string result = "";
            try
            {
                result = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(treatmentTypeCode, heinCardNumber, levelCode, rightRouteCode) ?? 0) * 100) + "%";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private List<V_HIS_SERE_SERV> GetSereServByServiceReqId(long serviceReqId)
        {
            Inventec.Common.Logging.LogSystem.Debug("Begin get List<V_HIS_SERE_SERV_12>");
            List<V_HIS_SERE_SERV> result = new List<V_HIS_SERE_SERV>();
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisSereServView12Filter sereServFilter = new HisSereServView12Filter();
                sereServFilter.SERVICE_REQ_ID = serviceReqId;
                var result1 = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<V_HIS_SERE_SERV_12>>(RequestUriStore.HIS_SERE_SERV_GETVIEW_12, ApiConsumers.MosConsumer, sereServFilter, paramCommon);
                if (result1 != null && result1.Count > 0)
                {
                    result1 = result1.Where(o => o.IS_NO_EXECUTE != 1).ToList();
                    foreach (var item in result1)
                    {
                        V_HIS_SERE_SERV ss = new V_HIS_SERE_SERV();
                        Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>();
                        ss = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_12, MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>(item);

                        var service = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>().FirstOrDefault(o => o.ID == ss.SERVICE_ID);
                        if (service != null)
                        {
                            ss.TDL_SERVICE_CODE = service.SERVICE_CODE;
                            ss.TDL_SERVICE_NAME = service.SERVICE_NAME;
                            ss.SERVICE_TYPE_NAME = service.SERVICE_TYPE_NAME;
                            ss.SERVICE_TYPE_CODE = service.SERVICE_TYPE_CODE;
                            ss.SERVICE_UNIT_CODE = service.SERVICE_UNIT_CODE;
                            ss.SERVICE_UNIT_NAME = service.SERVICE_UNIT_NAME;
                        }

                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == item.TDL_EXECUTE_ROOM_ID);
                        if (room != null)
                        {
                            ss.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                            ss.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                            ss.EXECUTE_DEPARTMENT_CODE = room.DEPARTMENT_CODE;
                            ss.EXECUTE_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                        }

                        if (HisConfigCFG.IsSetDefaultDepositPrice)
                            ss.VIR_PRICE = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(ss);
                        result.Add(ss);
                    }
                }
                if (result == null || result.Count == 0) Inventec.Common.Logging.LogSystem.Debug("GetSereServByServiceReqId => null____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqId), serviceReqId));
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_SERE_SERV>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            Inventec.Common.Logging.LogSystem.Debug("End get List<V_HIS_SERE_SERV_12>");
            return result;
        }

        private V_HIS_PATIENT_TYPE_ALTER GetPatientTypeAlterByPatient(HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            V_HIS_PATIENT_TYPE_ALTER result = new V_HIS_PATIENT_TYPE_ALTER();
            try
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT_TYPE_ALTER>(result, patientTypeAlter);
                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeAlter.PATIENT_TYPE_ID);
                if (patientType != null)
                {
                    result.PATIENT_TYPE_CODE = patientType.PATIENT_TYPE_CODE;
                    result.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                }
                var treatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeAlter.TREATMENT_TYPE_ID);
                if (treatmentType != null)
                {
                    result.TREATMENT_TYPE_CODE = treatmentType.TREATMENT_TYPE_CODE;
                    result.TREATMENT_TYPE_NAME = treatmentType.TREATMENT_TYPE_NAME;
                }

                if (result == null) Inventec.Common.Logging.LogSystem.Debug("GetPatientTypeAlterByPatient => null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private HIS_DEPARTMENT GetDepartmentById(long id)
        {
            HIS_DEPARTMENT result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new HIS_DEPARTMENT());
        }

        private V_HIS_ROOM GetRoomById(long id)
        {
            V_HIS_ROOM result = null;
            try
            {
                result = BackendDataWorker.Get<V_HIS_ROOM>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new V_HIS_ROOM());
        }

        private HIS_TRAN_PATI_FORM GetTranPatiFormById(long id)
        {
            HIS_TRAN_PATI_FORM result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_TRAN_PATI_FORM>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new HIS_TRAN_PATI_FORM());
        }

        private HIS_TRAN_PATI_REASON GetTranPatiReasonById(long id)
        {
            HIS_TRAN_PATI_REASON result = null;
            try
            {
                result = BackendDataWorker.Get<HIS_TRAN_PATI_REASON>().SingleOrDefault(o => o.ID == id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return (result ?? new HIS_TRAN_PATI_REASON());
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.currentHisExamServiceReqResultSDO == null)
                    throw new ArgumentNullException("DelegateRunPrinter => currentHisExamServiceReqResultSDO is null");
                if (this.currentHisExamServiceReqResultSDO.ServiceReqs == null || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0)
                    throw new ArgumentNullException("DelegateRunPrinter => ServiceReqs is null");
                if (this.currentHisExamServiceReqResultSDO.SereServs == null || this.currentHisExamServiceReqResultSDO.SereServs.Count == 0)
                    throw new ArgumentNullException("DelegateRunPrinter => SereServs is null");

                this.currentHisExamServiceReqResultSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => serviceReqPrintIds.Contains(o.ID)).ToList();
                //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 1");

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlterByPatient = this.GetPatientTypeAlterByPatient(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter);
                string strHeinRatio = "";
                if (patientTypeAlterByPatient != null)
                    strHeinRatio = this.GetDefaultHeinRatioForView(patientTypeAlterByPatient.HEIN_CARD_NUMBER, patientTypeAlterByPatient.HEIN_TREATMENT_TYPE_CODE, HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT, patientTypeAlterByPatient.RIGHT_ROUTE_CODE);

                var vHisPatient = HIS.Desktop.Print.PrintGlobalStore.GetPatientADOForPrintByIPatient(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient);

                //Duyệt danh sách yêu cầu, mỗi yêu cầu sẽ xử lý dữ liệu sau đó gọi đến thư viện thực hiện in ấn
                foreach (var serviceReq in this.currentHisExamServiceReqResultSDO.ServiceReqs)
                {
                    var room = this.GetRoomById(serviceReq.EXECUTE_ROOM_ID);
                    if (room != null)
                    {
                        serviceReq.EXECUTE_ROOM_CODE = room.ROOM_CODE;
                        serviceReq.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        serviceReq.EXECUTE_DEPARTMENT_CODE = room.DEPARTMENT_CODE;
                        serviceReq.EXECUTE_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                        serviceReq.EXECUTE_ROOM_ADDRESS = room.ADDRESS;
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 2");
                    var sereservs = currentHisExamServiceReqResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).ToList();
                    List<Mps000001_ListSereServs> mps000001_ListSereServs = (from m in sereservs select new Mps000001_ListSereServs(m)).ToList();
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 3");
                    MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001ADO = GenerateMps000001ADO(serviceReq, strHeinRatio);
                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((serviceReq != null ? serviceReq.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 4");
                    MPS.Processor.Mps000001.PDO.Mps000001PDO mps000001PDO = new MPS.Processor.Mps000001.PDO.Mps000001PDO(
                        serviceReq,
                        patientTypeAlterByPatient,
                        vHisPatient,
                        mps000001_ListSereServs,
                        this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment,
                        mps000001ADO);
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 5");
                    MPS.ProcessorBase.PrintConfig.PreviewType printType = MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog;
                    string printerName = ((GlobalVariables.dicPrinter != null && GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode])) ? GlobalVariables.dicPrinter[printTypeCode] : "");

                    if (isPrintNow && chkSignExam.Checked && !IsActionBtnPrint)
                    {
                        printType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow;
                    }
                    else if (chkSignExam.Checked && !IsActionBtnPrint)
                    {
                        printType = MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow;
                    }
                    else if (isPrintNow 
                        || AppConfigs.CheDoInPhieuDangKyDichVuKhamBenh == 2
                        || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        printType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                    }
                    // Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 6");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001PDO, printType, printerName) { EmrInputADO = inputADO });
                }
                //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 7");
                isPrintNow = false;
                IsActionBtnPrint = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private MPS.Processor.Mps000001.PDO.Mps000001ADO GenerateMps000001ADO(V_HIS_SERVICE_REQ serviceReq, string strHeinRatio)
        {
            MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001ADO = new MPS.Processor.Mps000001.PDO.Mps000001ADO();
            try
            {
                mps000001ADO.ratio_text = strHeinRatio;
                mps000001ADO.firstExamRoomName = serviceReq.EXECUTE_ROOM_NAME;
                if ((this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.IN_ROOM_ID ?? 0) > 0)
                {
                    var room = GetRoomById((this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.IN_ROOM_ID ?? 0)) ?? new V_HIS_ROOM();
                    mps000001ADO.IN_ROOM_NAME = room.ROOM_NAME;
                    mps000001ADO.IN_DEPARTMENT_NAME = room.DEPARTMENT_NAME;
                }
                //if ((this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.TRAN_PATI_FORM_ID ?? 0) > 0)
                //    mps000001ADO.TRAN_PATI_REASON_NAME = (GetTranPatiFormById((this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.TRAN_PATI_FORM_ID ?? 0)) ?? new HIS_TRAN_PATI_FORM()).TRAN_PATI_FORM_NAME;
                if ((this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.TRAN_PATI_REASON_ID ?? 0) > 0)
                    mps000001ADO.TRANSFER_IN_REASON_NAME = (GetTranPatiReasonById((this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.TRAN_PATI_REASON_ID ?? 0)) ?? new HIS_TRAN_PATI_REASON()).TRAN_PATI_REASON_NAME;
                //mps000001ADO.PHONE = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.PHONE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return mps000001ADO;
        }

        private void InitMenuPrint()
        {
            try
            {
                this.barManager.Form = this;
                if (this.menu == null)
                    this.menu = new PopupMenu(this.barManager);
                this.menu.ItemLinks.Clear();

                BarButtonItem itemInDvKham = new BarButtonItem(barManager, ResourceMessage.Title_InDichVuKham, 1);
                itemInDvKham.Tag = PrintType.InDvKham;
                itemInDvKham.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemInDvKham);

                BarButtonItem itemPrintDangKyKham = new BarButtonItem(barManager, ResourceMessage.Title_InPhieuYeuCauKham, 1);
                itemPrintDangKyKham.Tag = PrintType.InPhieuYeuCauKham;
                itemPrintDangKyKham.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemPrintDangKyKham);

                BarButtonItem itemInTheBenhNhan = new BarButtonItem(barManager, ResourceMessage.Title_InTheBenhNhan, 2);
                itemInTheBenhNhan.Tag = PrintType.InTheBenhNhan;
                itemInTheBenhNhan.ItemClick += new ItemClickEventHandler(onClick__Pluss);
                this.menu.AddItem(itemInTheBenhNhan);

                this.menu.ShowPopup(Cursor.Position);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void onClick__Pluss(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PrintType type = (PrintType)(e.Item.Tag);

                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);

                    switch (type)
                    {
                        case PrintType.InDvKham:
                            if (currentHisExamServiceReqResultSDO == null
                                || currentHisExamServiceReqResultSDO.ServiceReqs == null
                                || currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0
                                || actionType == GlobalVariables.ActionAdd)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                                return;
                            }

                            currentHisExamServiceReqResultSDO.ServiceReqs = currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => serviceReqPrintIds.Contains(o.ID)).ToList();

                            PrintProcess(currentHisExamServiceReqResultSDO);
                            break;

                        case PrintType.InTheBenhNhan:
                            richEditorMain.RunPrintTemplate("Mps000178", DelegateRunPrinterInTheBenhNhan);
                            break;
                        case PrintType.InPhieuYeuCauKham:
                            richEditorMain.RunPrintTemplate("Mps000309", DelegateRunPrinterInPhieuYeuCauKham);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinterInPhieuYeuCauKham(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (resultHisPatientProfileSDO == null
                                   || resultHisPatientProfileSDO.HisPatient == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return result;
                }
                WaitingManager.Show();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                if (resultHisPatientProfileSDO.HisPatient != null)
                {
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = resultHisPatientProfileSDO.HisPatient.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        currentPatient = rs.FirstOrDefault();
                    }
                }

                MPS.Processor.Mps000309.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000309.PDO.SingleKeyValue()
                {
                    LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(),
                    Username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName()
                };

                HIS_DHST dhst = new HIS_DHST();

                CommonParam param = new CommonParam();
                HisDhstFilter dhstFilter = new HisDhstFilter();
                dhstFilter.TREATMENT_ID = resultHisPatientProfileSDO.HisTreatment.ID;
                dhstFilter.ORDER_FIELD = "EXECUTE_TIME";
                dhstFilter.ORDER_DIRECTION = "DESC";
                dhst = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_DHST>>("api/HisDHST/Get", ApiConsumers.MosConsumer, dhstFilter, param).FirstOrDefault();


                V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>();
                patientTypeAlter = Mapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>(resultHisPatientProfileSDO.HisPatientTypeAlter);

                MPS.Processor.Mps000309.PDO.Mps000309PDO mps000309PDO = new MPS.Processor.Mps000309.PDO.Mps000309PDO(
                    currentPatient,
                    patientTypeAlter,
                    dhst,
                    resultHisPatientProfileSDO.HisTreatment,
                    singleKeyValue
                    );

                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000309PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000309PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000309PDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000309PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private bool DelegateRunPrinterInTheBenhNhan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (resultHisPatientProfileSDO == null
                                || resultHisPatientProfileSDO.HisPatient == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.DuLieuRong, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return result;
                }
                WaitingManager.Show();
                V_HIS_PATIENT currentPatient = new V_HIS_PATIENT();
                if (resultHisPatientProfileSDO.HisPatient != null)
                {
                    MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                    patientFilter.ID = resultHisPatientProfileSDO.HisPatient.ID;
                    var rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                    if (rs != null && rs.Count > 0)
                    {
                        currentPatient = rs.FirstOrDefault();
                    }
                }
                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                    currentPatient
                    );
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, GlobalVariables.dicPrinter[printTypeCode]);
                    }
                }
                else
                {
                    if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000178RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
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
