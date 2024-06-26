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
using AutoMapper;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicServices_NT.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.ADO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicServices_NT
{
    public partial class frmPublicServices_NT : HIS.Desktop.Utility.FormBase
    {
        internal List<MPS.Processor.Mps000225.PDO.Mps000225ADO> _Mps000116ADOs { get; set; }
        //internal List<MPS.Processor.Mps000225.PDO.Mps000225BySereServ> _Mps000225BySereServ;
        internal enum PrintType
        {
            PHIEU_CONG_KHAI_DICH_VU,
        }
        int dayCountData = 0;

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.PHIEU_CONG_KHAI_DICH_VU:
                        richEditorMain.RunPrintTemplate("Mps000225", DelegateRunPrinter);
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

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000225":
                        LoadBieuMauPhieuCongKhaiThuocTheoNgay(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuCongKhaiThuocTheoNgay(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                this._Datas = new List<Service_NT_ADO>();

                this.patientTypeHasSelecteds = GetPatientTypeGridSelected();

                if (!GetAllSereServV2())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Dịch vụ rỗng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                AddDataToDatetime();

                // Lấy thông tin bệnh nhân
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = this._treatmentId;
                var _Treatment = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, null).FirstOrDefault();

                MPS.Processor.Mps000225.PDO.SingleKeys __SingleKeys = new MPS.Processor.Mps000225.PDO.SingleKeys();
                if (this._TreatmentBedRoom != null)
                {
                    __SingleKeys.BED_ROOM_NAME = this._TreatmentBedRoom.BED_ROOM_NAME;
                    __SingleKeys.BED_NAME = this._TreatmentBedRoom.BED_NAME;
                    __SingleKeys.BED_CODE = this._TreatmentBedRoom.BED_CODE;
                }
                __SingleKeys.REQUEST_DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId) != null ? WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName : null;
                __SingleKeys.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                __SingleKeys.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                WaitingManager.Hide();
                foreach (var mps in this._Mps000116ADOs)
                {
                    List<MPS.Processor.Mps000225.PDO.Mps000225ADO> _Mps000116ADOsPrint = new List<MPS.Processor.Mps000225.PDO.Mps000225ADO>();
                    _Mps000116ADOsPrint.Add(mps);

                    MPS.Processor.Mps000225.PDO.Mps000225PDO mps000225RDO = new MPS.Processor.Mps000225.PDO.Mps000225PDO(
                    _Treatment,
                    _Mps000116ADOsPrint,
                   mps.Mps000225BySereServADOs,
                    __SingleKeys,
                    BackendDataWorker.Get<HIS_PATIENT_TYPE>(),
                    BackendDataWorker.Get<HIS_SERVICE_TYPE>()
                                );

                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000225RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000225RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                    result = MPS.MpsPrinter.Run(PrintData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        /// <summary>
        /// Ngoai Kho
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadDataNgoaiKho(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNgoaiKhoNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialeNgoaiKhoNewThread));

            try
            {
                threadMedicine.Start(param);
                threadMaterial.Start(param);

                threadMedicine.Join();
                threadMaterial.Join();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                threadMedicine.Abort();
                threadMaterial.Abort();
            }
        }

        private void LoadDataMedicineNgoaiKhoNewThread(object param)
        {
            try
            {
                LoadDataMedicineNgoaiKho((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicineNgoaiKho(List<long> _serviceReqIds)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Load V_HIS_EXP_MEST_MEDICINE_2 start");
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMetyFilter serviceReqMetyFilter = new HisServiceReqMetyFilter();
                serviceReqMetyFilter.SERVICE_REQ_IDs = _serviceReqIds;
                List<HIS_SERVICE_REQ_METY> currentServiceReqMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, serviceReqMetyFilter, param);


                if (currentServiceReqMety != null && currentServiceReqMety.Count > 0)
                {
                    var medicineGroups1 = currentServiceReqMety.Where(p => p.MEDICINE_TYPE_ID != null).GroupBy(p => new { p.MEDICINE_TYPE_ID, p.SERVICE_REQ_ID }).Select(p => p.ToList()).ToList();
                    if (medicineGroups1 != null && medicineGroups1.Count > 0)
                    {
                        foreach (var itemGroups in medicineGroups1)
                        {
                            Service_NT_ADO expMeti = new Service_NT_ADO();
                            if (itemGroups[0].MEDICINE_TYPE_ID != null)
                            {
                                var mediType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemGroups[0].MEDICINE_TYPE_ID);
                                if (mediType != null)
                                {
                                    expMeti.SERVICE_UNIT_NAME = mediType.SERVICE_UNIT_NAME;
                                }
                                expMeti.SERVICE_ID = itemGroups[0].MEDICINE_TYPE_ID ?? 0;
                            }
                            decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT);

                            expMeti.SERVICE_NAME = itemGroups[0].MEDICINE_TYPE_NAME;
                            expMeti.INTRUCTION_DATE = ConvertToOutputFormat(dicServiceReq[itemGroups[0].SERVICE_REQ_ID].USE_TIME) ?? dicServiceReq[itemGroups[0].SERVICE_REQ_ID].INTRUCTION_DATE;
                            expMeti.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                            expMeti.AMOUNT = _AMOUNT;
                            _Datas.Add(expMeti);
                        }
                    }

                    var medicineGroups2 = currentServiceReqMety.Where(p => p.MEDICINE_TYPE_ID == null).GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                    if (medicineGroups2 != null && medicineGroups2.Count > 0)
                    {
                        foreach (var itemGroups in medicineGroups2)
                        {
                            Service_NT_ADO expMeti = new Service_NT_ADO();

                            decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                            expMeti.SERVICE_UNIT_NAME = itemGroups[0].UNIT_NAME;
                            expMeti.SERVICE_NAME = itemGroups[0].MEDICINE_TYPE_NAME;
                            expMeti.INTRUCTION_DATE = ConvertToOutputFormat(dicServiceReq[itemGroups[0].SERVICE_REQ_ID].USE_TIME) ?? dicServiceReq[itemGroups[0].SERVICE_REQ_ID].INTRUCTION_DATE;
                            expMeti.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC;
                            expMeti.AMOUNT = _AMOUNT;
                            _Datas.Add(expMeti);
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("Loaded V_HIS_EXP_MEST_MEDICINE_2 end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialeNgoaiKhoNewThread(object param)
        {
            try
            {
                LoadDataMaterialNgoaiKho((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialNgoaiKho(List<long> _serviceReqIds)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Load HIS_SERVICE_REQ_MATY start");
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqMatyFilter serviceReqMatyFilter = new HisServiceReqMatyFilter();
                serviceReqMatyFilter.SERVICE_REQ_IDs = _serviceReqIds;
                var currentServiceReqMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, serviceReqMatyFilter, param);


                if (currentServiceReqMaty != null && currentServiceReqMaty.Count > 0)
                {
                    var materialGroups = currentServiceReqMaty.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.SERVICE_REQ_ID }).Select(p => p.ToList()).ToList();
                    foreach (var itemGroups in materialGroups)
                    {
                        Service_NT_ADO expMeti = new Service_NT_ADO();

                        decimal _AMOUNT = itemGroups.Sum(p => p.AMOUNT);
                        expMeti.SERVICE_UNIT_NAME = itemGroups[0].UNIT_NAME;
                        expMeti.SERVICE_NAME = itemGroups[0].MATERIAL_TYPE_NAME;
                        expMeti.SERVICE_ID = itemGroups[0].MATERIAL_TYPE_ID ?? 0;
                        expMeti.INTRUCTION_DATE = ConvertToOutputFormat(dicServiceReq[itemGroups[0].SERVICE_REQ_ID].USE_TIME) ?? dicServiceReq[itemGroups[0].SERVICE_REQ_ID].INTRUCTION_DATE;
                        expMeti.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;
                        expMeti.AMOUNT = _AMOUNT;
                        _Datas.Add(expMeti);
                    }
                }

                Inventec.Common.Logging.LogSystem.Info("Loaded HIS_SERVICE_REQ_MATY end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddDataToDatetime()
        {
            try
            {
                this._Mps000116ADOs = new List<MPS.Processor.Mps000225.PDO.Mps000225ADO>();
                if (this._Datas != null && this._Datas.Count > 0)
                {
                    List<long> distinctDates = this._Datas
                        .Select(o => o.INTRUCTION_DATE)
                        .Distinct().OrderBy(t => t).ToList();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => distinctDates), distinctDates));
                    var sereServGroups = this._Datas.GroupBy(p => new { p.SERVICE_ID, p.PRICE, p.SERVICE_TYPE_ID, p.CONCENTRA, p.IS_EXPEND, p.INTRUCTION_DATE, p.PATIENT_TYPE_ID }).ToList();

                    this.congKhaiDichVu_DaySize = (this.congKhaiDichVu_DaySize == 0 ? 10 : this.congKhaiDichVu_DaySize);
                    this.dayCountData = distinctDates.Count;
                    int dayPageSize = (int)(Inventec.Common.Number.Convert.RoundUpValue(((double)this.dayCountData / (double)congKhaiDichVu_DaySize), 0));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => congKhaiDichVu_DaySize), congKhaiDichVu_DaySize) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dayCountData), dayCountData) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => congKhaiDichVu_DaySize), congKhaiDichVu_DaySize) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dayPageSize), dayPageSize));
                    for (int i = 1; i <= dayPageSize; i++)
                    {
                        List<long> distinctDatesInPage = distinctDates.Skip((i - 1) * congKhaiDichVu_DaySize).Take(congKhaiDichVu_DaySize).ToList();

                        #region ThuatToan
                        //while (index < distinctDatesInPage.Count)
                        //{
                        MPS.Processor.Mps000225.PDO.Mps000225ADO sdo = new MPS.Processor.Mps000225.PDO.Mps000225ADO();
                        #region ---Day---
                        PropertyInfo[] ps = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000225.PDO.Mps000225ADO>();
                        for (int j = 0; j < 60; j++)
                        {
                            PropertyInfo info = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                            if (info != null)
                            {
                                info.SetValue(sdo, j < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[j]) : "");
                            }
                        }
                        #endregion

                        #region ---Day and year---
                        PropertyInfo[] py = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000225.PDO.Mps000225ADO>();
                        for (int j = 0; j < 60; j++)
                        {
                            PropertyInfo info = py.FirstOrDefault(o => o.Name == string.Format("DayAndYear{0}", j + 1));
                            if (info != null)
                            {
                                info.SetValue(sdo, j < distinctDatesInPage.Count ? TimeNumberToDateString(distinctDatesInPage[j]) : "");
                            }
                        }
                        #endregion
                        sdo.Mps000225BySereServADOs = new List<MPS.Processor.Mps000225.PDO.Mps000225BySereServ>();
                        foreach (var group in sereServGroups)
                        {
                            if (!distinctDatesInPage.Contains(group.Key.INTRUCTION_DATE) || group.ToList().Sum(o => o.AMOUNT) <= 0)
                                continue;

                            MPS.Processor.Mps000225.PDO.Mps000225BySereServ sereServPrint = new MPS.Processor.Mps000225.PDO.Mps000225BySereServ();

                            //Review gán lại từng dữ liệu
                            sereServPrint.TDL_SERVICE_NAME = group.First().SERVICE_NAME;//Check
                            sereServPrint.Service_Type_Id = group.First().SERVICE_TYPE_ID;
                            sereServPrint.SERVICE_ID = group.First().SERVICE_ID;
                            sereServPrint.SERVICE_UNIT_NAME = group.First().SERVICE_UNIT_NAME;
                            sereServPrint.AMOUNT = group.ToList().Sum(o => o.AMOUNT);
                            sereServPrint.AMOUNT_STRING = sereServPrint.AMOUNT + "";
                            sereServPrint.CONCENTRA = group.First().CONCENTRA;
                            sereServPrint.TypeExpend = group.Key.IS_EXPEND ?? 0;
                            sereServPrint.PATIENT_TYPE_ID = group.Key.PATIENT_TYPE_ID;
                            sereServPrint.PRICE = group.Key.PRICE ?? 0;
                            List<string> lstInstructionNote = group.Select(o => o.INSTRUCTION_NOTE).Distinct().ToList();
                            if (lstInstructionNote != null && lstInstructionNote.Count > 0)
                            {
                                sereServPrint.INSTRUCTION_NOTE = string.Join(", ", lstInstructionNote);
                            }
                            string amount = sereServPrint.AMOUNT + "";
                            PropertyInfo[] pp = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000225.PDO.Mps000225BySereServ>();
                            for (int j = 0; j < 60; j++)
                            {
                                PropertyInfo infoSdo = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                                PropertyInfo info = pp.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));

                                if (info != null)
                                {
                                    info.SetValue(sereServPrint, TimeNumberToDateString(group.Key.INTRUCTION_DATE) == (string)infoSdo.GetValue(sdo) ? amount : "");
                                }
                                PropertyInfo info2 = pp.FirstOrDefault(o => o.Name == string.Format("MORNING_Day{0}", j + 1));
                                if (info2 != null)
                                {
                                    string text2 = group.ToList().Sum(s => FormatSessionOfDay(s.MORNING)).ToString();
                                    info2.SetValue(sereServPrint, TimeNumberToDateString(group.Key.INTRUCTION_DATE) == (string)infoSdo.GetValue(sdo) ? text2 : "");
                                }
                                PropertyInfo info3 = pp.FirstOrDefault(o => o.Name == string.Format("NOON_Day{0}", j + 1));
                                if (info3 != null)
                                {
                                    string text3 = group.ToList().Sum(s => FormatSessionOfDay(s.NOON)).ToString();
                                    info3.SetValue(sereServPrint, TimeNumberToDateString(group.Key.INTRUCTION_DATE) == (string)infoSdo.GetValue(sdo) ? text3 : "");
                                }
                                PropertyInfo info4 = pp.FirstOrDefault(o => o.Name == string.Format("AFTERNOON_Day{0}", j + 1));
                                if (info4 != null)
                                {
                                    string text4 = group.ToList().Sum(s => FormatSessionOfDay(s.AFTERNOON)).ToString();
                                    info4.SetValue(sereServPrint, TimeNumberToDateString(group.Key.INTRUCTION_DATE) == (string)infoSdo.GetValue(sdo) ? text4 : "");
                                }
                                PropertyInfo info5 = pp.FirstOrDefault(o => o.Name == string.Format("EVENING_Day{0}", j + 1));
                                if (info5 != null)
                                {
                                    string text5= group.ToList().Sum(s => FormatSessionOfDay(s.EVENING)).ToString();
                                    info5.SetValue(sereServPrint, TimeNumberToDateString(group.Key.INTRUCTION_DATE) == (string)infoSdo.GetValue(sdo) ? text5 : "");
                                }

                            }

                            sdo.Mps000225BySereServADOs.Add(sereServPrint);
                        }

                        sdo.Mps000225BySereServADOs = sdo.Mps000225BySereServADOs.OrderBy(p => p.TDL_SERVICE_NAME).ToList();

                        this._Mps000116ADOs.Add(sdo);
                        //}
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public decimal FormatSessionOfDay(string dt)
        {
            decimal val = 0;
            try
            {
                if (!string.IsNullOrEmpty(dt) && dt.Contains("/"))
                {
                    var lst = dt.Split('/');
                    if (lst != null)
                    {
                        val = Convert.ToDecimal(lst[0].Trim()) / Convert.ToDecimal(lst[1].Trim());
                    }
                }
                else if (!string.IsNullOrEmpty(dt) && dt.Contains("+"))
                {
                    var lst = dt.Split('+');
                    if (lst != null)
                    {
                        val = Convert.ToDecimal(lst[0].Trim()) + Convert.ToDecimal(lst[1].Trim());
                    }
                }
                else if (!string.IsNullOrEmpty(dt) && dt.Contains("-"))
                {
                    var lst = dt.Split('-');
                    if (lst != null)
                    {
                        val = Convert.ToDecimal(lst[0].Trim()) - Convert.ToDecimal(lst[1].Trim());
                    }
                }
                else if (!string.IsNullOrEmpty(dt) && dt.Contains("*"))
                {
                    var lst = dt.Split('*');
                    if (lst != null)
                    {
                        val = Convert.ToDecimal(lst[0].Trim()) * Convert.ToDecimal(lst[1].Trim());
                    }
                }
                else
                {
                    val = Convert.ToDecimal(dt);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return val;
        }

        private string TimeNumberToDateString(long _Time)
        {
            string TimeString = "";
            try
            {
                TimeString = Inventec.Common.DateTime.Convert.TimeNumberToDateString(_Time).Substring(0, 5);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return TimeString;
        }
    }
}
