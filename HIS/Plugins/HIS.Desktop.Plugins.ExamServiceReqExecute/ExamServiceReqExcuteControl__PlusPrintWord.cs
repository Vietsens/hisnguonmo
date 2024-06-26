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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Plugins.ExamServiceReqExecute.Config;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : System.Windows.Forms.UserControl
    {
        V_HIS_PATIENT patient { get; set; }
        HIS_DHST dhst { get; set; }
        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        List<V_HIS_DEPARTMENT_TRAN> departmentTrans { get; set; }
        //V_HIS_TRAN_PATI tranPatie { get; set; }
        List<V_HIS_SERE_SERV_5> sereServMedis { get; set; }
        List<HIS_EXP_MEST> expMests { get; set; }

        /// <summary>
        /// Khởi tạo nút in và in
        /// </summary>
        /// <param name="isAppoinment">Khởi tạo in hẹn khám và in</param>
        /// <param name="isBordereau">Khởi tạo in bệnh án ngoại trú và in</param>
        private void FillDataToButtonPrintAndAutoPrint(bool? isAppoinment = false, bool? isBordereau = false)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemKhamBenhVaoVien = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_PHIEU_KHAM_BENH_VAO_VIEN", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                itemKhamBenhVaoVien.Tag = PrintType.KHAM_BENH_VAO_VIEN;
                menu.Items.Add(itemKhamBenhVaoVien);

                DXMenuItem itemBenhAnNgoaiTru = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_BENH_AN_NGOAI_TRU", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPhieuKhamBenh));
                itemBenhAnNgoaiTru.Tag = PrintType.BENH_AN_NGOAI_TRU;
                menu.Items.Add(itemBenhAnNgoaiTru);

                if (isAppoinment.HasValue && isAppoinment.Value)
                {
                    DXMenuItem itemGiayHenKham = new DXMenuItem("Giấy hẹn khám", new EventHandler(onClickInPhieuKhamBenh));
                    itemGiayHenKham.Tag = PrintType.IN_GIAY_HEN_KHAM;
                    menu.Items.Add(itemGiayHenKham);
                    if (this.isPrintAppoinment)
                    {
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_HEN_KHAM__MPS000008, DelegateRunPrinter);
                    }
                   
                }

                if (isBordereau.HasValue && isBordereau.Value)
                {
                    bool isBHYT = false;
                    bool isVienPhi = false;
                    CheckBordereauType(ref isBHYT, ref isVienPhi);
                    if (isBHYT)
                    {
                        DXMenuItem itemNgoaiTruBHYT = new DXMenuItem("Bảng kê ngoại trú BHYT", new EventHandler(onClickInPhieuKhamBenh));
                        itemNgoaiTruBHYT.Tag = PrintType.BANG_KE_NGOAI_TRU_BHYT;
                        menu.Items.Add(itemNgoaiTruBHYT);
                        if (this.isPrintBordereau)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_BHYT__MPS000120, DelegateRunPrinter);
                        }
                    }
                    if (isVienPhi)
                    {
                        DXMenuItem itemNgoaiTruVP = new DXMenuItem("Bảng kê ngoại trú viện phí", new EventHandler(onClickInPhieuKhamBenh));
                        itemNgoaiTruVP.Tag = PrintType.BANG_KE_NOI_TRU_VIEN_PHI;
                        menu.Items.Add(itemNgoaiTruVP);
                        if (this.isPrintBordereau)
                        {
                            richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_VIEN_PHI__MPS000122, DelegateRunPrinter);
                        }
                    }
                }

                btnPrint_ExamService.DropDownControl = menu;
                ContextMenuStrip strip = new ContextMenuStrip();
                ToolStripItem item1 = new ToolStripMenuItem();
                item1.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_EXAM_SERVICE_REQ_EXCUTE_CONTROL_IN_PHIEU_YEU_CAU_KHAM", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                item1.Click += clickItemKham;
                item1.Tag = PrintType.IN_PHIEU_YEU_CAU;
                strip.Show(Cursor.Position.X, Cursor.Position.Y);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void clickItemKham(object sender, EventArgs e)
        {
            try
            {
                var bbtnItem1 = sender as ToolStripItem;
                PrintType type = (PrintType)(bbtnItem1.Tag);
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal enum PrintType
        {
            KHAM_BENH_VAO_VIEN,
            BENH_AN_NGOAI_TRU,
            KHAM_SUC_KHOE_CAN_BO,
            IN_PHIEU_YEU_CAU,
            IN_GIAY_HEN_KHAM,
            BANG_KE_NGOAI_TRU_BHYT,
            BANG_KE_NOI_TRU_VIEN_PHI
        }

        private void onClickInPhieuKhamBenh(object sender, EventArgs e)
        {
            try
            {

                var bbtnItem = sender as DXMenuItem;
                PrintType type = (PrintType)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174:
                        LoadBieuMauPhieuYCBenhAnNgoaiTru(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_BENH_VAO_VIEN__MPS000007:
                        LoadBieuMauPhieuYCKhamBenhVaoVien(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_HEN_KHAM__MPS000008:
                        LoadBieuMauGiayHenKham(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_BHYT__MPS000120:
                        LoadBangKeBHYT(printTypeCode, fileName);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_VIEN_PHI__MPS000122:
                        LoadBangKeVienPhi(printTypeCode, fileName);
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

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.KHAM_BENH_VAO_VIEN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_BENH_VAO_VIEN__MPS000007, DelegateRunPrinter);
                        break;
                    case PrintType.BENH_AN_NGOAI_TRU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_BENH_AN_NGOAI_TRU__MPS000174, DelegateRunPrinter);
                        break;
                    case PrintType.KHAM_SUC_KHOE_CAN_BO:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM_SUC_KHOE_CAN_BO__MPS000013, DelegateRunPrinter);
                        break;
                    case PrintType.IN_PHIEU_YEU_CAU:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_KHAM__MPS000025, DelegateRunPrinter);
                        break;
                    case PrintType.IN_GIAY_HEN_KHAM:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__GIAY_HEN_KHAM__MPS000008, DelegateRunPrinter);
                        break;
                    case PrintType.BANG_KE_NGOAI_TRU_BHYT:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_BHYT__MPS000120, DelegateRunPrinter);
                        break;
                    case PrintType.BANG_KE_NOI_TRU_VIEN_PHI:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BANG_KE_NGOAI_TRU_VIEN_PHI__MPS000122, DelegateRunPrinter);
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

        private void LoadBieuMauPhieuYCBenhAnNgoaiTru(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                btnSave_Click(null, null);
                WaitingManager.Show();

                List<Action> methods = new List<Action>();
                methods.Add(LoadPatient);
                methods.Add(LoadPatientTypeAlter);
                methods.Add(LoadDepartmentTran);
                //methods.Add(LoadTranPati);
                methods.Add(LoadExpMest);
                methods.Add(LoadSereServ);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                List<long> expMestIds = expMests != null ? expMests.Select(o => o.ID).ToList() : null;
                MOS.Filter.HisExpMestMedicineViewFilter medicineFilter = new MOS.Filter.HisExpMestMedicineViewFilter();
                medicineFilter.EXP_MEST_IDs = expMestIds;//TODO
                List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, medicineFilter, param);

                List<HIS_ICD> icds = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_ICD>();
                //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXAM_SERVICE_REQ_1, V_HIS_EXAM_SERVICE_REQ>();
                //V_HIS_EXAM_SERVICE_REQ examServiceReq = AutoMapper.Mapper.Map<V_HIS_EXAM_SERVICE_REQ_1, V_HIS_EXAM_SERVICE_REQ>(this.SereServExt);
                string requestDepartmentName = "";

                //requestDepartmentName = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == this.HisServiceReqView.REQUEST_DEPARTMENT_ID).DEPARTMENT_NAME;
                //WaitingManager.Hide();
                //MPS.Processor.Mps000174.PDO.Mps000174PDO rdo = new MPS.Processor.Mps000174.PDO.Mps000174PDO(
                //    patient,
                //    departmentTrans,
                //    patientTypeAlter,
                //    this.HisServiceReqView,
                //    treatment,
                //    icds,
                //    expMests,
                //    expMestMedicines,
                //    sereServMedis,
                //    requestDepartmentName,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__APPROVAL,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DRAFT,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXPORT,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REJECT,
                //    IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__REQUEST
                //    );
                //result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadBieuMauPhieuYCKhamBenhVaoVien(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                btnSave_Click(null, null);
                WaitingManager.Show();

                List<Action> methods = new List<Action>();
                methods.Add(LoadTreatment);
                methods.Add(LoadPatientTypeAlter);
                methods.Add(LoadDepartmentTran);
                //methods.Add(LoadTranPati);
                methods.Add(LoadPatient);
                methods.Add(LoadDHST);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                string userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                string executeRoomName = "";
                string executeDepartmentName = "";

                executeRoomName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == this.moduleData.RoomId).RoomName;

                var executeDepartment = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_DEPARTMENT>()
                    .FirstOrDefault(o => o.ID == this.HisServiceReqView.EXECUTE_DEPARTMENT_ID);
                if (executeDepartment != null)
                {
                    executeDepartmentName = executeDepartment.DEPARTMENT_NAME;
                }

                HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                string ratio_text = "";
                if (patientTypeAlter != null)
                    ratio_text = GetDefaultHeinRatioForView(patientTypeAlter.HEIN_CARD_NUMBER, patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, levelCode, patientTypeAlter.RIGHT_ROUTE_CODE);

                MPS.Processor.Mps000007.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000007.PDO.SingleKeyValue();
                singleKeyValue.ExecuteRoomName = executeRoomName;
                singleKeyValue.ExecuteDepartmentName = executeDepartmentName;
                singleKeyValue.RatioText = ratio_text;
                if (treatment.ICD_NAME != null)
                {
                    singleKeyValue.Icd_Name = treatment.ICD_NAME;
                }

                V_HIS_DEPARTMENT_TRAN departmentTran = null;
                if (departmentTrans != null && departmentTrans.Count > 0)
                {
                    var departmentTranTimeNull = departmentTrans.FirstOrDefault(o => o.DEPARTMENT_IN_TIME == null);
                    if (departmentTranTimeNull != null)
                        departmentTran = departmentTranTimeNull;
                    else
                        departmentTran = departmentTrans[0];
                }

                Inventec.Common.Logging.LogSystem.Warn("departmentTran :" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => departmentTran), departmentTran));

                //AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_TREATMENT, HIS_TREATMENT>();
                //HIS_TREATMENT treatmentRaw = AutoMapper.Mapper.Map<V_HIS_TREATMENT, HIS_TREATMENT>(treatment);

                WaitingManager.Hide();

                MPS.Processor.Mps000007.PDO.Mps000007PDO rdo = new MPS.Processor.Mps000007.PDO.Mps000007PDO(
                    patient,
                    patientTypeAlter,
                    departmentTran,
                    this.HisServiceReqView,
                    dhst,
                    treatment,
                    singleKeyValue
                    );
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


        }

        private void LoadBieuMauGiayHenKham(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();

                HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.HisServiceReqView.TREATMENT_ID;
                HIS_TREATMENT treatment = new BackendAdapter(new CommonParam())
                    .Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam()).FirstOrDefault();
                //Thông tin bệnh nhân
                var currentPatient = PrintGlobalStore.getPatient(treatment.ID);
                //Thông tin bảo hiểm y tế
                long instructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(treatment.ID, instructionTime, ref PatyAlterBhyt);

                //Lấy dữ liệu về Ra viện của Bn

                MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN departmentTran = PrintGlobalStore.getDepartmentTran(treatment.ID);

                MPS.Processor.Mps000008.PDO.PatientADO PatientADO = new MPS.Processor.Mps000008.PDO.PatientADO(currentPatient);

                MPS.Processor.Mps000008.PDO.Mps000008PDO mps000008RDO = new MPS.Processor.Mps000008.PDO.Mps000008PDO(
                           PatientADO,
                           PatyAlterBhyt,
                           treatment,
                           null,
                           0
                           );

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                if (treatment.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân chưa khóa viện phí");
                }
                else
                {
                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000008RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000008RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName));
                    }
                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBangKeBHYT(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                List<V_HIS_TREATMENT_FEE> treatmentFees = new List<V_HIS_TREATMENT_FEE>();
                V_HIS_TREATMENT currentTreatment = new V_HIS_TREATMENT();
                List<Action> methods = new List<Action>();
                methods.Add(LoadDepartmentTran);
                methods.Add(() => { treatmentFees = LoadTreatmentFee(); });
                methods.Add(() => { currentTreatment = LoadTreatmentView(); });
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                string departmentName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == moduleData.RoomId).DepartmentName;
                List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                long totalDayTreatment = 0;
                if (treatment != null)
                {
                    if (treatment.OUT_TIME.HasValue)
                    {
                        if (treatment.OUT_TIME.HasValue)
                            totalDayTreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(treatment.CLINICAL_IN_TIME, treatment.OUT_TIME);
                        else
                            totalDayTreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(treatment.IN_TIME, treatment.OUT_TIME);
                    }
                }


                string statusTreatmentOut = "";
                if (!String.IsNullOrEmpty(treatment.TRANSFER_IN_MEDI_ORG_CODE))
                {
                    statusTreatmentOut = "CV";
                }
                else
                {
                    if (SereServ8s != null && SereServ8s.Count > 0)
                    {
                        var sereServExam = SereServ8s.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH);
                        var sereServMedi = SereServ8s.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                        var sereServCLS = SereServ8s.FirstOrDefault(o =>
                            o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN);
                        statusTreatmentOut = (sereServMedi == null && sereServExam != null && sereServCLS != null)
                            || (sereServMedi == null && sereServExam != null && sereServCLS == null) ? "CLS" : "";
                    }
                }

                MPS.Processor.Mps000120.PDO.Config.HeinServiceTypeCFG heinServiceType = new MPS.Processor.Mps000120.PDO.Config.HeinServiceTypeCFG();
                heinServiceType.HEIN_SERVICE_TYPE__HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.HEIN_SERVICE_TYPE__EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;

                MPS.Processor.Mps000120.PDO.Config.PatientTypeCFG patientTypeCFG = new MPS.Processor.Mps000120.PDO.Config.PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                MPS.Processor.Mps000120.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000120.PDO.SingleKeyValue();
                singleKeyValue.departmentName = departmentName;
                singleKeyValue.statusTreatmentOut = statusTreatmentOut;
                singleKeyValue.today = totalDayTreatment;

                WaitingManager.Hide();

                Dictionary<string, List<V_HIS_SERE_SERV_8>> dicSereServ = new Dictionary<string, List<V_HIS_SERE_SERV_8>>();
                dicSereServ = GroupSereServByPatyAlterBhyt(SereServ8s);
                foreach (var sereServ in dicSereServ)
                {
                    HIS_PATIENT_TYPE_ALTER patyBhyt = JsonConvert.DeserializeObject<HIS_PATIENT_TYPE_ALTER>(sereServ.Value.FirstOrDefault().JSON_PATIENT_TYPE_ALTER);
                    HIS_BRANCH branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                    HIS_TREATMENT_TYPE treatmentType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_TYPE>().FirstOrDefault(o => o.ID == patyBhyt.TREATMENT_TYPE_ID);
                    if (treatmentType != null)
                    {
                        string levelCode = branch != null ? branch.HEIN_LEVEL_CODE : null;
                        string ratio_text = GetDefaultHeinRatioForView(patyBhyt.HEIN_CARD_NUMBER, treatmentType.TREATMENT_TYPE_CODE, levelCode, patyBhyt.RIGHT_ROUTE_CODE);
                        singleKeyValue.ratio = ratio_text;
                    }

                    MPS.Processor.Mps000120.PDO.Mps000120PDO rdo = new MPS.Processor.Mps000120.PDO.Mps000120PDO(patyBhyt, departmentTrans, treatmentFees, heinServiceType, patientTypeCFG, sereServ.Value, currentTreatment, heinServiceTypes, rooms, services, singleKeyValue);
                    if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                    }
                    else
                    {
                        result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadBangKeVienPhi(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                List<V_HIS_TREATMENT_FEE> treatmentFees = new List<V_HIS_TREATMENT_FEE>();
                V_HIS_TREATMENT currentTreatment = new V_HIS_TREATMENT();
                List<Action> methods = new List<Action>();
                methods.Add(LoadDepartmentTran);
                methods.Add(() => { treatmentFees = LoadTreatmentFee(); });
                methods.Add(() => { currentTreatment = LoadTreatmentView(); });
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                string departmentName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == moduleData.RoomId).DepartmentName;
                List<HIS_HEIN_SERVICE_TYPE> heinServiceTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();
                List<V_HIS_ROOM> rooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                List<V_HIS_SERVICE> services = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE>();
                long totalDayTreatment = 0;
                if (treatment != null)
                {
                    if (treatment.OUT_TIME.HasValue)
                    {
                        if (treatment.OUT_TIME.HasValue)
                            totalDayTreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(treatment.CLINICAL_IN_TIME, treatment.OUT_TIME);
                        else
                            totalDayTreatment = HIS.Common.Treatment.Calculation.DayOfTreatment(treatment.IN_TIME, treatment.OUT_TIME);
                    }
                }


                string statusTreatmentOut = "";
                if (!String.IsNullOrEmpty(treatment.TRANSFER_IN_MEDI_ORG_CODE))
                {
                    statusTreatmentOut = "CV";
                }
                else
                {
                    if (SereServ8s != null && SereServ8s.Count > 0)
                    {
                        var sereServExam = SereServ8s.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH);
                        var sereServMedi = SereServ8s.FirstOrDefault(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                        var sereServCLS = SereServ8s.FirstOrDefault(o =>
                            o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA
                            || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN);
                        statusTreatmentOut = (sereServMedi == null && sereServExam != null && sereServCLS != null)
                            || (sereServMedi == null && sereServExam != null && sereServCLS == null) ? "CLS" : "";
                    }
                }

                MPS.Processor.Mps000122.PDO.Config.HeinServiceTypeCFG heinServiceType = new MPS.Processor.Mps000122.PDO.Config.HeinServiceTypeCFG();
                heinServiceType.HEIN_SERVICE_TYPE__HIGHTECH_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                heinServiceType.HEIN_SERVICE_TYPE__EXAM_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__KH;

                MPS.Processor.Mps000122.PDO.Config.PatientTypeCFG patientTypeCFG = new MPS.Processor.Mps000122.PDO.Config.PatientTypeCFG();
                patientTypeCFG.PATIENT_TYPE__BHYT = HisPatientTypeCFG.PATIENT_TYPE_ID__BHYT;
                patientTypeCFG.PATIENT_TYPE__FEE = HisPatientTypeCFG.PATIENT_TYPE_ID__IS_FEE;

                MPS.Processor.Mps000122.PDO.SingleKeyValue singleKeyValue = new MPS.Processor.Mps000122.PDO.SingleKeyValue();
                singleKeyValue.departmentName = departmentName;
                singleKeyValue.statusTreatmentOut = statusTreatmentOut;
                singleKeyValue.today = totalDayTreatment;


                WaitingManager.Hide();

                MPS.Processor.Mps000122.PDO.Mps000122PDO rdo = new MPS.Processor.Mps000122.PDO.Mps000122PDO(departmentTrans, treatmentFees, heinServiceType, patientTypeCFG, SereServ8s, currentTreatment, heinServiceTypes, rooms, services, singleKeyValue);
                if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, ""));
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public string GetDefaultHeinRatioForView(string heinCardNumber, string treatmentTypeCode, string levelCode, string rightRouteCode)
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
    }
}
