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
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraBars;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Utils;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MPS.Processor.Mps000001.PDO;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.Library.EmrGenerate;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {

        DevExpress.XtraBars.BarManager barManager = new DevExpress.XtraBars.BarManager();
        PopupMenu menu;
        private enum PrintType
        {
            InDvKham,
            InTheBenhNhan
        }

        private void Print()
        {
            try
            {
                if (this.isPrintNow)
                {
                    if (this.currentHisExamServiceReqResultSDO == null
                    || this.currentHisExamServiceReqResultSDO.ServiceReqs == null
                    || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0
                    || this.actionType == GlobalVariables.ActionAdd)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                        return;
                    }

                    this.currentHisExamServiceReqResultSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => this.serviceReqPrintIds.Contains(o.ID)).ToList();

                    this.PrintProcess(this.currentHisExamServiceReqResultSDO);
                }
                else
                {
                    this.InitMenuPrint();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);

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

                V_HIS_PATIENT_TYPE_ALTER patientTypeAlterByPatient = new V_HIS_PATIENT_TYPE_ALTER();
                if (this.resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                {
                    patientTypeAlterByPatient = this.GetPatientTypeAlterByPatient(this.resultHisPatientProfileSDO.HisPatientTypeAlter);
                }

                V_HIS_TREATMENT_4 treatment4 = new V_HIS_TREATMENT_4();
                if (this.resultHisPatientProfileSDO.HisTreatment != null)
                {
                    MOS.Filter.HisTreatmentView4Filter filter = new HisTreatmentView4Filter();
                    filter.ID = this.resultHisPatientProfileSDO.HisTreatment.ID;
                    var treatments = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_4>>("api/HisTreatment/GetView4", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (treatments != null && treatments.Count > 0)
                    {
                        treatment4 = treatments.First();
                    }
                }

                MPS.Processor.Mps000178.PDO.Mps000178PDO mps000178RDO = new MPS.Processor.Mps000178.PDO.Mps000178PDO(
                    currentPatient,
                    patientTypeAlterByPatient,
                    treatment4
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

                PrintData.EmrInputADO = inputADO;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode("", printTypeCode, this.currentModule != null ? this.currentModule.RoomId : 0);

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
                    }
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 2");
                    var sereservs = currentHisExamServiceReqResultSDO.SereServs.Where(o => o.SERVICE_REQ_ID == serviceReq.ID).ToList();
                    List<Mps000001_ListSereServs> mps000001_ListSereServs = (from m in sereservs select new Mps000001_ListSereServs(m)).ToList();
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 3");
                    MPS.Processor.Mps000001.PDO.Mps000001ADO mps000001ADO = GenerateMps000001ADO(serviceReq, strHeinRatio);
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 4");
                    MPS.Processor.Mps000001.PDO.Mps000001PDO mps000001PDO = new MPS.Processor.Mps000001.PDO.Mps000001PDO(
                        serviceReq,
                        patientTypeAlterByPatient,
                        vHisPatient,
                        mps000001_ListSereServs,
                        this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment,
                        mps000001ADO);
                    //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 5");
                    MPS.ProcessorBase.PrintConfig.PreviewType printType;
                    string printerName = ((GlobalVariables.dicPrinter != null && GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode])) ? GlobalVariables.dicPrinter[printTypeCode] : "");
                    if (isPrintNow == true
                        || AppConfigs.CheDoInPhieuDangKyDichVuKhamBenh == 2
                        || ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        printType = MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow;
                    }
                    else
                    {
                        printType = MPS.ProcessorBase.PrintConfig.PreviewType.Show;
                    }
                    // Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 6");
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000001PDO, printType, printerName) { EmrInputADO = inputADO });
                }
                //Inventec.Common.Logging.LogSystem.Debug("DelegateRunPrinter -> 7");
                isPrintNow = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

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
    }
}
