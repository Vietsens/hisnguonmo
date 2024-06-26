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
using DevExpress.Utils.Menu;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.PrintOtherForm;
using HIS.Desktop.Plugins.Library.PrintOtherForm.Base;
using HIS.Desktop.Plugins.RehaServiceReqExecute.ADO;
using HIS.Desktop.Plugins.RehaServiceReqExecute.Base;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MPS.Processor.Mps000207.PDO.ADO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    public partial class RehaServiceReqExecuteControl : HIS.Desktop.Utility.UserControlBase
    {
        private void FillDataToButtonPrintReha()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInKQPHCN = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_REHA_SERVICE_REQ_EXCUTE_CONTROL_IN_KET_QUA_PHUC_HOI_CHUC_NANG", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPHCN));
                itemInKQPHCN.Tag = PrintTypeEndo.IN_KET_QUA_PHUC_HOI_CHUC_NANG;
                menu.Items.Add(itemInKQPHCN);

                DXMenuItem itemInPCD = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_REHA_SERVICE_REQ_EXCUTE_CONTROL_IN_PHIEU_CHI_DINH_KY_THUAT_PHCN", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPHCN));
                itemInPCD.Tag = PrintTypeEndo.IN_PHIEU_CHI_DINH_PHCN;
                menu.Items.Add(itemInPCD);

                DXMenuItem itemInBieuMauKhac = new DXMenuItem(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_REHA_SERVICE_REQ_EXCUTE_CONTROL_IN_BIEU_MAU_KHAC", ResourceLangManager.LanguageUCRehaServiceReqExecute, LanguageManager.GetCulture()), new EventHandler(onClickInPHCN));
                itemInBieuMauKhac.Tag = PrintTypeEndo.IN_BIEU_MAU_KHAC;
                menu.Items.Add(itemInBieuMauKhac);

                cboPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintTypeEndo
        {
            IN_KET_QUA_PHUC_HOI_CHUC_NANG,
            IN_PHIEU_YEU_CAU_PHUC_HOI_CHUC_NANG,
            IN_PHIEU_CHI_DINH_PHCN,
            IN_BIEU_MAU_KHAC
        }

        private void onClickInPHCN(object sender, EventArgs e)
        {
            try
            {

                var bbtnItem = sender as DXMenuItem;
                PrintTypeEndo type = (PrintTypeEndo)(bbtnItem.Tag);
                PrintProcess(type);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            };
        }

        void PrintProcess(PrintTypeEndo printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeEndo.IN_KET_QUA_PHUC_HOI_CHUC_NANG:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_PHCN_TONG_HOP__MPS000063, DelegateRunPrinterEndo);
                        break;
                    case PrintTypeEndo.IN_PHIEU_CHI_DINH_PHCN:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_CHI_DINH_PHCN__MPS000207, DelegateRunPrinterEndo);
                        break;
                    case PrintTypeEndo.IN_BIEU_MAU_KHAC:
                        LoadBieuMauKhacPhieuPHCN();
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

        bool DelegateRunPrinterEndo(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_YEU_CAU_IN_KET_QUA_PHCN_TONG_HOP__MPS000063:
                        LoadBieuMauPhieuYCInKetQuaPhucHoiChucNang(printTypeCode, fileName, ref result);
                        break;
                    case PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__PHIEU_CHI_DINH_PHCN__MPS000207:
                        LoadBieuMauPhieuChiDinhPHCN(printTypeCode, fileName);
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

        private void LoadBieuMauKhacPhieuPHCN()
        {
            try
            {
                PrintOtherFormProcessor printOtherFormProcessor = new PrintOtherFormProcessor(this.HisServiceReqWithOrderSDO.ID, SereServ.ID, Treatment.ID, Treatment.PATIENT_ID, Library.PrintOtherForm.Base.UpdateType.TYPE.SERVICE_REQ);
                printOtherFormProcessor.Print(PrintType.TYPE.PHIEU_PHCN);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void LoadBieuMauPhieuYCInKetQuaPhucHoiChucNang(string printTypeCode, string fileName, ref bool result)
        {
            WaitingManager.Show();
            CommonParam param = new CommonParam();

            MOS.EFMODEL.DataModels.HIS_REHA_SUM hisRehaSumRow = new HIS_REHA_SUM();
            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_REHA_SUM>(hisRehaSumRow, this.HisServiceReqWithOrderSDO);
            // Lấy thông tin bệnh nhân
            treatmentId = this.HisServiceReqWithOrderSDO.TREATMENT_ID;

            HisPatientViewFilter patientFilter = new HisPatientViewFilter();
            patientFilter.ID = this.HisServiceReqWithOrderSDO.TDL_PATIENT_ID;
            V_HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumers.MosConsumer, patientFilter, param).FirstOrDefault();
            //Lấy thông tin chuyển khoa
            var departmentTran = PrintGlobalStore.getDepartmentTran(treatmentId);
            List<V_HIS_DEPARTMENT_TRAN> lstDepartmentTran = new List<V_HIS_DEPARTMENT_TRAN>();
            lstDepartmentTran.Add(departmentTran);
            //Lấy thông tin điều trị
            HisServiceReqViewFilter _viewFilter = new HisServiceReqViewFilter();
            _viewFilter.ID = this.HisServiceReqWithOrderSDO.ID;
            V_HIS_SERVICE_REQ _ServiceReq = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, _viewFilter, param).FirstOrDefault();

            HisRehaTrainViewFilter rehaTrainFilter = new HisRehaTrainViewFilter();
            rehaTrainFilter.SERE_SERV_REHA_IDs = sereServRehas.Where(o => o.choose == true).Select(o => o.ID).ToList();
            List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN> lstSereServRehas = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN>>(HisRequestUriStore.HIS_REHA_TRAIN_GETVIEW, ApiConsumers.MosConsumer, rehaTrainFilter, param);

            List<MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO> hisSereServRehaADOs = new List<MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO>();
            if (lstSereServRehas != null && lstSereServRehas.Count > 0)
            {
                var sereServRehaGroupBy = lstSereServRehas.Select(x => new { SereServReha = x, SERVICE_ID = x.REHA_TRAIN_TYPE_ID, SERVICE_CODE = x.REHA_TRAIN_TYPE_CODE, SERVICE_NAME = x.REHA_TRAIN_TYPE_NAME, REHA_TRAIN_UNIT_CODE = x.REHA_TRAIN_UNIT_CODE, REHA_TRAIN_UNIT_NAME = x.REHA_TRAIN_UNIT_NAME }).GroupBy(s => (s.SERVICE_ID), (g, exps) => new { SERVICE_ID = g, SereServRehas = exps, AMOUNT_REHA_SUM = exps.Sum(o => o.SereServReha.AMOUNT) });

                if (sereServRehaGroupBy != null)
                {
                    foreach (var itemGroup in sereServRehaGroupBy)
                    {
                        MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO sereServRehaSDO = new MPS.Processor.Mps000063.PDO.Mps000063PDO.ExeHisSereServRehaADO();

                        sereServRehaSDO.AMOUNT = itemGroup.AMOUNT_REHA_SUM;
                        var dataReha = itemGroup.SereServRehas.FirstOrDefault(o => o.SERVICE_ID == itemGroup.SERVICE_ID);
                        if (dataReha != null)
                        {
                            sereServRehaSDO.REHA_TRAIN_TYPE_CODE = dataReha.SERVICE_CODE;
                            sereServRehaSDO.REHA_TRAIN_TYPE_NAME = dataReha.SERVICE_NAME;
                            sereServRehaSDO.REHA_TRAIN_UNIT_CODE = dataReha.REHA_TRAIN_UNIT_CODE;
                            sereServRehaSDO.REHA_TRAIN_UNIT_NAME = dataReha.REHA_TRAIN_UNIT_NAME;
                        }

                        hisSereServRehaADOs.Add(sereServRehaSDO);
                    }
                }
            }

            var bedRoomName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModuleData.RoomId).RoomName;

            WaitingManager.Hide();

            MPS.Processor.Mps000063.PDO.Mps000063PDO rdo = new MPS.Processor.Mps000063.PDO.Mps000063PDO(
                patient,
                lstDepartmentTran,
                hisRehaSumRow,
                hisSereServRehaADOs,
                bedRoomName,
                _ServiceReq
                );
            result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
        }

        private void LoadBieuMauPhieuChiDinhPHCN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();

                if (this.HisServiceReqWithOrderSDO == null)
                    throw new Exception("this.HisServiceReqWithOrderSDO is null");
                List<SereServRehaADO> sereServRehaADOs = gridControlSereServReha.DataSource as List<SereServRehaADO>;
                sereServRehaADOs = sereServRehaADOs != null ? sereServRehaADOs.Where(o => o.choose == true).ToList() : null;
                if (sereServRehaADOs == null || sereServRehaADOs.Count == 0)
                    throw new Exception("Không có thông tin kỹ thuật tập nào được chọn");
                HisRehaTrainFilter rehaTrainFilter = new HisRehaTrainFilter();
                rehaTrainFilter.SERE_SERV_REHA_IDs = sereServRehaADOs.Select(o => o.ID).ToList();
                List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN> lstSereServRehas = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.V_HIS_REHA_TRAIN>>("api/HisRehaTrain/GetView", ApiConsumers.MosConsumer, rehaTrainFilter, param);

                string currentDepartmentName = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomId == currentModuleData.RoomId).DepartmentName;

                if (lstSereServRehas == null || lstSereServRehas.Count <= 0)
                {
                    MessageBox.Show("Không có dịch vụ để in");
                    return;
                }

                List<RehaTrainPrintByPage> rehaTrains = this.PrepareData(lstSereServRehas);
                foreach (var item in rehaTrains)
                {
                    MPS.Processor.Mps000207.PDO.Mps000207PDO rdo = new MPS.Processor.Mps000207.PDO.Mps000207PDO(this.HisServiceReqWithOrderSDO, item, currentDepartmentName);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
                }


            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<RehaTrainPrintByPage> PrepareData(List<V_HIS_REHA_TRAIN> rehaTrains)
        {
            List<RehaTrainPrintByPage> result = null;
            if (rehaTrains != null && rehaTrains.Count > 0)
            {
                result = new List<RehaTrainPrintByPage>();
                List<string> distinctDates = rehaTrains
                    .Select(o => o.TRAIN_TIME.ToString().Substring(0, 8))
                    .Distinct().OrderBy(t => t).ToList();
                var rehaTrainGroups = rehaTrains.GroupBy(o => new { o.REHA_TRAIN_TYPE_ID, o.REHA_TRAIN_TYPE_NAME });
                int index = 0;
                while (index < distinctDates.Count)
                {
                    RehaTrainPrintByPage sdo = new RehaTrainPrintByPage();
                    sdo.Day1 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day2 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day3 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day4 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day5 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day6 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day7 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day8 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day9 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day10 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day11 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day12 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day13 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day14 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day15 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day16 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day17 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day18 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day19 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day20 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day21 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day22 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day23 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    sdo.Day24 = index < distinctDates.Count ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(distinctDates[index++]) : "";
                    List<RehaTrainPrint> rehaTrainPrints = new List<RehaTrainPrint>();
                    foreach (var group in rehaTrainGroups)
                    {
                        RehaTrainPrint rehaTrainPrint = new RehaTrainPrint();
                        List<V_HIS_REHA_TRAIN> trains = group.ToList();
                        rehaTrainPrint.RehaTrainTypeName = group.Key.REHA_TRAIN_TYPE_NAME;
                        rehaTrainPrint.Day1 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day1).Any() ? "X" : "";
                        rehaTrainPrint.Day2 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day2).Any() ? "X" : "";
                        rehaTrainPrint.Day3 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day3).Any() ? "X" : "";
                        rehaTrainPrint.Day4 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day4).Any() ? "X" : "";
                        rehaTrainPrint.Day5 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day5).Any() ? "X" : "";
                        rehaTrainPrint.Day6 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day6).Any() ? "X" : "";
                        rehaTrainPrint.Day7 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day7).Any() ? "X" : "";
                        rehaTrainPrint.Day8 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day8).Any() ? "X" : "";
                        rehaTrainPrint.Day9 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day9).Any() ? "X" : "";
                        rehaTrainPrint.Day10 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day10).Any() ? "X" : "";
                        rehaTrainPrint.Day11 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day11).Any() ? "X" : "";
                        rehaTrainPrint.Day12 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day12).Any() ? "X" : "";
                        rehaTrainPrint.Day13 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day13).Any() ? "X" : "";
                        rehaTrainPrint.Day14 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day14).Any() ? "X" : "";
                        rehaTrainPrint.Day15 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day15).Any() ? "X" : "";
                        rehaTrainPrint.Day16 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day16).Any() ? "X" : "";
                        rehaTrainPrint.Day17 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day17).Any() ? "X" : "";
                        rehaTrainPrint.Day18 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day18).Any() ? "X" : "";
                        rehaTrainPrint.Day19 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day19).Any() ? "X" : "";
                        rehaTrainPrint.Day20 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day20).Any() ? "X" : "";
                        rehaTrainPrint.Day21 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day21).Any() ? "X" : "";
                        rehaTrainPrint.Day22 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day22).Any() ? "X" : "";
                        rehaTrainPrint.Day23 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day23).Any() ? "X" : "";
                        rehaTrainPrint.Day24 = trains.Where(o => Inventec.Common.DateTime.Convert.TimeNumberToDateString(o.TRAIN_TIME) == sdo.Day24).Any() ? "X" : "";
                        rehaTrainPrints.Add(rehaTrainPrint);
                    }
                    sdo.RehaTrainPrints = rehaTrainPrints;
                    result.Add(sdo);
                }
            }
            return result;
        }
    }
}
