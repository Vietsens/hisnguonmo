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

namespace HIS.Desktop.Plugins.RegisterV3.Run3
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
                if (hisPatientVitaminASDOSave == null
                    || hisPatientVitaminASDOSave.HisPatient == null
                    || (hisPatientVitaminASDOSave.HisVaccinationExam == null && hisPatientVitaminASDOSave.HisVitaminA == null))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                    return;
                }

                this.PrintProcess();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PrintProcess()
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000444", DelegateRunPrinter);
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
                            if (hisPatientVitaminASDOSave == null
                                || hisPatientVitaminASDOSave.HisPatient == null
                                || (hisPatientVitaminASDOSave.HisVaccinationExam == null && hisPatientVitaminASDOSave.HisVitaminA == null)
                                || actionType == GlobalVariables.ActionAdd)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                                return;
                            }

                            PrintProcess();
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
                if (this.hisPatientVitaminASDOSave == null)
                    throw new ArgumentNullException("DelegateRunPrinter => hisPatientVitaminASDOSave is null");
                if (this.hisPatientVitaminASDOSave.HisPatient == null)
                    throw new ArgumentNullException("DelegateRunPrinter => HisPatient is null");
                if (this.hisPatientVitaminASDOSave.HisVaccinationExam == null && this.hisPatientVitaminASDOSave.HisVitaminA == null)
                    throw new ArgumentNullException("DelegateRunPrinter => HisVaccinationExam is null And HisVitaminA is null");

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

                //Sửa api trả về View sẽ bỏ code get api
                V_HIS_VACCINATION_EXAM vaccinExam = null;
                V_HIS_VITAMIN_A vitamin = null;
                V_HIS_PATIENT patient = null;
                CommonParam param = new CommonParam();
                HisPatientViewFilter paFilter = new HisPatientViewFilter();
                paFilter.ID = this.hisPatientVitaminASDOSave.HisPatient.ID;
                var patientResult = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, paFilter, param);
                if (patientResult != null && patientResult.Count > 0)
                {
                    patient = patientResult.FirstOrDefault();
                }

                if (this.hisPatientVitaminASDOSave.HisVaccinationExam != null)
                {
                    HisVaccinationExamViewFilter vaccineFilter = new HisVaccinationExamViewFilter();
                    vaccineFilter.ID = this.hisPatientVitaminASDOSave.HisVaccinationExam.ID;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/GetView", ApiConsumer.ApiConsumers.MosConsumer, vaccineFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        vaccinExam = apiResult.FirstOrDefault();
                    }
                }
                else if (this.hisPatientVitaminASDOSave.HisVitaminA != null)
                {
                    HisVitaminAViewFilter vitaminFilter = new HisVitaminAViewFilter();
                    vitaminFilter.ID = this.hisPatientVitaminASDOSave.HisVitaminA.ID;
                    var apiResult = new BackendAdapter(param).Get<List<V_HIS_VITAMIN_A>>("api/HisVitaminA/GetView", ApiConsumer.ApiConsumers.MosConsumer, vitaminFilter, param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        vitamin = apiResult.FirstOrDefault();
                    }
                }

                MPS.Processor.Mps000444.PDO.Mps000444PDO pdo = new MPS.Processor.Mps000444.PDO.Mps000444PDO(patient, vaccinExam, vitamin, this.hisPatientVitaminASDOSave.HisDhst);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, printType, printerName));

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
