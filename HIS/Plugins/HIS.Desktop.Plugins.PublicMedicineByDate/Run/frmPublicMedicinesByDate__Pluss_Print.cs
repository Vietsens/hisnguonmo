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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PublicMedicineByDate
{
    public partial class frmPublicMedicinesByDate : HIS.Desktop.Utility.FormBase
    {
        internal List<MPS.Processor.Mps000116.PDO.Mps000116ADO> _Mps000116ADOs { get; set; }
        internal enum PrintType
        {
            PHIEU_CONG_KHAI_THUOC_THEO_NGAY,
        }

        void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.PHIEU_CONG_KHAI_THUOC_THEO_NGAY:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_CONG_KHAI_THUOC_THEO_NGAY__MPS000116, DelegateRunPrinter);
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
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_CONG_KHAI_THUOC_THEO_NGAY__MPS000116:
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
                _Mps000116ADOs = new List<MPS.Processor.Mps000116.PDO.Mps000116ADO>();
                var _WorkPlace = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId);//dang lam viec
                if (!GetAllSereServV2())
                {
                    var day = Inventec.Common.DateTime.Convert.TimeNumberToDateString(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDatePublic.DateTime) ?? 0);
                    string mess = (chkMedicine.Checked == true && chkMaterial.Checked == true) ? " thuốc, vật tư " : (chkMedicine.Checked == true) ? " thuốc " : (chkMaterial.Checked == true) ? " vật tư " : null;

                    string hourFrom = "", hourTo = "";
                    if (tmIntructionTimeFrom.EditValue != null)
                    {
                        hourFrom = String.Format("{0:00}:{1:00}", tmIntructionTimeFrom.TimeSpan.Hours, tmIntructionTimeFrom.TimeSpan.Minutes);
                    }
                    else
                    {
                        hourFrom = "00:00";
                    }

                    if (tmIntructionTimeTo.EditValue != null)
                    {
                        hourTo = String.Format("{0:00}:{1:00}", tmIntructionTimeTo.TimeSpan.Hours, tmIntructionTimeTo.TimeSpan.Minutes);
                    }
                    else
                    {
                        hourTo = "23:59";
                    }

                    if (tmIntructionTimeFrom.EditValue != null || tmIntructionTimeTo.EditValue != null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Ngày " + day + " giờ từ " + hourFrom + " đến " + hourTo + "  chưa có chỉ định" + mess + "cho bệnh nhân", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Ngày " + day + "  chưa có chỉ định" + mess + "cho bệnh nhân", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    return;
                }

                // Lấy thông tin bệnh nhân

                CommonParam param = new CommonParam();
                MOS.Filter.HisTreatmentViewFilter hisTreatmentFilter = new MOS.Filter.HisTreatmentViewFilter();
                hisTreatmentFilter.ID = this._treatmentId;
                var _Treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, hisTreatmentFilter, param).FirstOrDefault();

                //MOS.Filter.HisTreatmentBedRoomViewFilter treatmentbedRoomFilter = new HisTreatmentBedRoomViewFilter();
                //treatmentbedRoomFilter.TREATMENT_ID = this._treatmentId;
                //treatmentbedRoomFilter.ORDER_FIELD = "CREATE_TIME";
                //treatmentbedRoomFilter.ORDER_DIRECTION = "DESC";
                //var _TreatmentBedRoom = new BackendAdapter(param).Get<List<V_HIS_TREATMENT_BED_ROOM>>(HisRequestUriStore.HIS_TREATMENT_BED_ROOM_GETVIEW, ApiConsumers.MosConsumer, treatmentbedRoomFilter, param).FirstOrDefault();

                MOS.Filter.HisBedLogViewFilter filterBedLog = new MOS.Filter.HisBedLogViewFilter();
                filterBedLog.TREATMENT_ID = this._treatmentId;
                if (_TreatmentBedRoom != null && _TreatmentBedRoom.BED_ROOM_ID > 0)
                {
                    filterBedLog.BED_ROOM_ID = _TreatmentBedRoom.BED_ROOM_ID;
                }
                else
                {
                    filterBedLog.START_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDatePublic.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "000000");
                    filterBedLog.START_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtDatePublic.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "235959");
                }
                filterBedLog.ORDER_FIELD = "START_TIME";
                filterBedLog.ORDER_DIRECTION = "DESC";
                var vHisBedLog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterBedLog, param).FirstOrDefault();

                MPS.Processor.Mps000116.PDO.SingleKeys __SingleKeys = new MPS.Processor.Mps000116.PDO.SingleKeys();
                __SingleKeys.REQUEST_DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId) != null ? WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName : null;
                __SingleKeys.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                __SingleKeys.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                __SingleKeys.IsOderMedicine = chkAccordingToSetting.Checked ? 1 : 2;

                if (_Mps000116ADOs != null && _Mps000116ADOs.Count > 0)
                {
                    _Mps000116ADOs = _Mps000116ADOs.OrderBy(p => p.MEDI_MATY_TYPE_NAME).Where(p => p.AMOUNT > 0).ToList();
                }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);
                MPS.Processor.Mps000116.PDO.Mps000116PDO mps000116RDO = new MPS.Processor.Mps000116.PDO.Mps000116PDO(
                    _Treatment,
                    _Mps000116ADOs,
                    _WorkPlace,
                    vHisBedLog,
                    Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDatePublic.DateTime) ?? 0,
                    __SingleKeys
                    );
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (chkSign.Checked)
                {
                    if (chkPrintDocumentSigned.Checked)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000116RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000116RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                }

                else
                {
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000116RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000116RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                    }
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Trong Kho
        /// </summary>
        /// <param name="param"></param>
        private void CreateThreadLoadData(object param)
        {
            Thread threadMedicine = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMedicineNewThread));
            Thread threadMaterial = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(LoadDataMaterialNewThread));
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

        private void LoadDataMedicineNewThread(object param)
        {
            try
            {
                //if (this.InvokeRequired)
                //{
                //    this.Invoke(new MethodInvoker(delegate { LoadDataTreatmentWithTreatment((long)param); }));
                //}
                //else
                //{
                LoadDataMedicine((List<long>)param);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMedicine(List<long> _expMestIds)
        {
            try
            {
                if (chkMedicine.Checked)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMedicineViewFilter expMestMediFilter = new HisExpMestMedicineViewFilter();
                    expMestMediFilter.EXP_MEST_IDs = _expMestIds;
                    var currentExpMestMedi = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetView", ApiConsumers.MosConsumer, expMestMediFilter, param);


                    if (currentExpMestMedi != null && currentExpMestMedi.Count > 0)
                    {
                        var medicineGroups = currentExpMestMedi.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.VAT_RATIO,p.MORNING_IS_USED,p.NOON_IS_USED,p.AFTERNOON_IS_USED,p.EVENING_IS_USED }).Select(p => p.ToList()).ToList();
                        this._Mps000116ADOs.AddRange((from r in medicineGroups select new MPS.Processor.Mps000116.PDO.Mps000116ADO(r, this._MedicalInstruction)).ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterialNewThread(object param)
        {
            try
            {
                LoadDataMaterial((List<long>)param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMaterial(List<long> _expMestIds)
        {
            try
            {
                // Inventec.Common.Logging.LogSystem.Error("Load V_HIS_EXP_MEST_MATERIAL start");
                if (chkMaterial.Checked)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisExpMestMaterialViewFilter expMestMateFilter = new HisExpMestMaterialViewFilter();
                    expMestMateFilter.EXP_MEST_IDs = _expMestIds;
                    var currentExpMestMate = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMateFilter, param);


                    if (currentExpMestMate != null && currentExpMestMate.Count > 0)
                    {
                        var data = currentExpMestMate.Where(p => p.IS_CHEMICAL_SUBSTANCE == null).ToList();
                        if (data != null && data.Count > 0)
                        {
                            var materialGroups = data.GroupBy(p => new { p.MATERIAL_TYPE_ID, p.VAT_RATIO }).Select(p => p.ToList()).ToList();
                            this._Mps000116ADOs.AddRange((from r in materialGroups select new MPS.Processor.Mps000116.PDO.Mps000116ADO(r)).ToList());
                        }

                    }
                }

                // Inventec.Common.Logging.LogSystem.Error("Loaded V_HIS_EXP_MEST_MATERIAL end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                //   Inventec.Common.Logging.LogSystem.Error("Load V_HIS_EXP_MEST_MEDICINE_2 start");
                if (chkMedicine.Checked)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqMetyFilter serviceReqMetyFilter = new HisServiceReqMetyFilter();
                    serviceReqMetyFilter.SERVICE_REQ_IDs = _serviceReqIds;
                    List<HIS_SERVICE_REQ_METY> currentServiceReqMety = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>("api/HisServiceReqMety/Get", ApiConsumers.MosConsumer, serviceReqMetyFilter, param);


                    if (currentServiceReqMety != null && currentServiceReqMety.Count > 0)
                    {
                        var medicineGroups1 = currentServiceReqMety.Where(p => p.MEDICINE_TYPE_ID != null).GroupBy(p => p.MEDICINE_TYPE_ID).Select(p => p.ToList()).ToList();
                        if (medicineGroups1 != null && medicineGroups1.Count > 0)
                        {
                            this._Mps000116ADOs.AddRange((from r in medicineGroups1 select new MPS.Processor.Mps000116.PDO.Mps000116ADO(r, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>())).ToList());
                        }

                        var medicineGroups2 = currentServiceReqMety.Where(p => p.MEDICINE_TYPE_ID == null).GroupBy(p => new { p.MEDICINE_TYPE_ID, p.MEDICINE_TYPE_NAME,p.UNIT_NAME }).Select(p => p.ToList()).ToList();
                        if (medicineGroups2 != null && medicineGroups2.Count > 0)
                        {
                            this._Mps000116ADOs.AddRange((from r in medicineGroups2 select new MPS.Processor.Mps000116.PDO.Mps000116ADO(r, BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>())).ToList());
                        }
                    }
                }

                //   Inventec.Common.Logging.LogSystem.Error("Loaded V_HIS_EXP_MEST_MEDICINE_2 end");
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
                //  Inventec.Common.Logging.LogSystem.Error("Load HIS_SERVICE_REQ_MATY start");
                if (chkMaterial.Checked)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqMatyFilter serviceReqMatyFilter = new HisServiceReqMatyFilter();
                    serviceReqMatyFilter.SERVICE_REQ_IDs = _serviceReqIds;
                    var currentServiceReqMaty = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>("api/HisServiceReqMaty/Get", ApiConsumers.MosConsumer, serviceReqMatyFilter, param);


                    if (currentServiceReqMaty != null && currentServiceReqMaty.Count > 0)
                    {
                        var materialGroups = currentServiceReqMaty.GroupBy(p => p.MATERIAL_TYPE_ID).Select(p => p.ToList()).ToList();
                        this._Mps000116ADOs.AddRange((from r in materialGroups select new MPS.Processor.Mps000116.PDO.Mps000116ADO(r, BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>())).ToList());
                    }
                }

                //  Inventec.Common.Logging.LogSystem.Error("Loaded HIS_SERVICE_REQ_MATY end");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
