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
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PublicMedicineByPhased.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.PublicMedicineByPhased.Config;
using MPS.Processor.Mps000088.PDO;
using System.Reflection;

namespace HIS.Desktop.Plugins.PublicMedicineByPhased
{
    public partial class frmPublicMedicineByPhased : HIS.Desktop.Utility.FormBase
    {
        //internal List<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate> mps000088ByMediEndMate;
        internal List<MPS.Processor.Mps000088.PDO.Mps000088ADO> mps000088ADO;
        List<Mps000088ByMediEndMate> _Mps000088ByMediEndMateADOs;

        internal enum PrintTypeVotesMedicines
        {
            IN_PHIEU_CONG_KHAI_THUOC,
        }

        void PrintProcess(PrintTypeVotesMedicines printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeVotesMedicines.IN_PHIEU_CONG_KHAI_THUOC:
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_CONG_KHAI_THUOC__MPS000088, DelegateRunPrinterVotesMedicines);
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

        bool DelegateRunPrinterVotesMedicines(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case PrintTypeCodeStore.PRINT_TYPE_CODE__BIEUMAU__PHIEU_CONG_KHAI_THUOC__MPS000088:
                        LoadBieuMauPhieuCongKhaiThuoc(printTypeCode, fileName, ref result);
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

        private void LoadBieuMauPhieuCongKhaiThuoc(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                var treatmentId = _treatmentId;
                // Lấy thông tin bệnh nhân
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = treatmentId;
                var _Treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                AddDataToDatetime();

                //MPS.Processor.Mps000088.PDO.SingleKeys __SingleKeys = new MPS.Processor.Mps000088.PDO.SingleKeys();
                //if (this._TreatmentBedRoom != null)
                //{
                //    __SingleKeys.BED_ROOM_NAME = this._TreatmentBedRoom.BED_ROOM_NAME;
                //    __SingleKeys.BED_NAME = this._TreatmentBedRoom.BED_NAME;
                //    __SingleKeys.BED_CODE = this._TreatmentBedRoom.BED_CODE;
                //}

                MOS.Filter.HisBedLogViewFilter filterBedLog = new MOS.Filter.HisBedLogViewFilter();
                filterBedLog.TREATMENT_ID = this._treatmentId;
                if (_TreatmentBedRoom != null && _TreatmentBedRoom.BED_ROOM_ID > 0)
                {
                    filterBedLog.BED_ROOM_ID = _TreatmentBedRoom.BED_ROOM_ID;
                }
                else
                {
                    filterBedLog.START_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtFromTime.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "000000");
                    filterBedLog.START_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime((dtToTime.EditValue ?? "0").ToString()).ToString("yyyyMMdd") + "235959");
                }
                filterBedLog.ORDER_FIELD = "START_TIME";
                filterBedLog.ORDER_DIRECTION = "DESC";
                var vHisBedLog = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumer.ApiConsumers.MosConsumer, filterBedLog, param).FirstOrDefault();

                MPS.Processor.Mps000088.PDO.SingleKeys __SingleKeys = new MPS.Processor.Mps000088.PDO.SingleKeys();
                if (cboDepartment.EditValue != null)
                {
                    var dataDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault
                (p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboDepartment.EditValue ?? "0").ToString()));
                    __SingleKeys.REQUEST_DEPARTMENT_NAME = dataDepartment != null ? dataDepartment.DEPARTMENT_NAME : null;
                }
                else
                    __SingleKeys.REQUEST_DEPARTMENT_NAME = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId) != null ? WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).DepartmentName : null;
                __SingleKeys.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                __SingleKeys.USER_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                __SingleKeys.IsTachHDSD = chkTachHDSD.Checked;
                __SingleKeys.IsOderMedicine = chkSortName.Checked ? 2 : 1;

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printTypeCode, this.currentModule != null ? currentModule.RoomId : 0);

                List<MPS.Processor.Mps000088.PDO.Mps000088PDO> lstmps000088RDO = new List<MPS.Processor.Mps000088.PDO.Mps000088PDO>();

                foreach (var ado88 in mps000088ADO)
                {
                    MPS.Processor.Mps000088.PDO.Mps000088PDO mps000088RDO = new MPS.Processor.Mps000088.PDO.Mps000088PDO(
    _Treatment,
    new List<MPS.Processor.Mps000088.PDO.Mps000088ADO>() { ado88 },
    ado88.Mps000088ByMediEndMateADOs,
    __SingleKeys,
    vHisBedLog);

                    lstmps000088RDO.Add(mps000088RDO);
                }
                WaitingManager.Hide();
                foreach (var mps000088RDO in lstmps000088RDO)
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (chkSign.Checked)
                    {
                        if (chkPrintDocumentSigned.Checked)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000088RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000088RDO, MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow, "") { EmrInputADO = inputADO };
                    }
                    else
                    {
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000088RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000088RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                        }
                    }

                    result = MPS.MpsPrinter.Run(PrintData);
                    if (result != null && PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignAndPrintNow || PrintData.previewType == MPS.ProcessorBase.PrintConfig.PreviewType.EmrSignNow)
                    {
                        MessageManager.Show(this.ParentForm, param, result);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void AddDataToDatetime()
        {
            try
            {
                if (ExpMestMediAndMateADOPrint != null && ExpMestMediAndMateADOPrint.Count > 0)
                {
                    List<long> distinctDates = ExpMestMediAndMateADOPrint
                        .Select(o => o.INTRUCTION_DATE)
                        .Distinct().OrderBy(t => t).ToList();

                    // ✅ Bỏ INTRUCTION_DATE ra khỏi group key
                    var sereServGroups = ExpMestMediAndMateADOPrint
                        .GroupBy(p => new {
                            p.MEDICINE_TYPE_ID,
                            V = chkTachHDSD.Checked ? p.TUTORIAL : "",
                            p.PRICE,
                            p.Service_Type_Id,
                            p.IS_EXPEND,
                            p.VAT_RATIO
                        }).ToList();

                    mps000088ADO = new List<MPS.Processor.Mps000088.PDO.Mps000088ADO>();
                    this.dayCountData = distinctDates.Count;
                    int dayPageSize = (int)(Inventec.Common.Number.Convert.RoundUpValue(
                        ((double)this.dayCountData / (double)Config.Config.congKhaiThuoc_DaySize), 0));

                    int d = 0;
                    for (int i = 1; i <= dayPageSize; i++)
                    {
                        List<long> distinctDatesInPage = distinctDates
                            .Skip((i - 1) * Config.Config.congKhaiThuoc_DaySize)
                            .Take(Config.Config.congKhaiThuoc_DaySize)
                            .ToList();
                        d++;

                        MPS.Processor.Mps000088.PDO.Mps000088ADO sdo = new MPS.Processor.Mps000088.PDO.Mps000088ADO();

                        #region ---Day---
                        PropertyInfo[] ps = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000088.PDO.Mps000088ADO>();
                        for (int j = 0; j < 60; j++)
                        {
                            PropertyInfo info = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                            if (info != null)
                            {
                                info.SetValue(sdo, j < distinctDatesInPage.Count
                                    ? TimeNumberToDateString(distinctDatesInPage[j])
                                    : "");
                            }
                        }
                        #endregion

                        #region ---Day and year---
                        PropertyInfo[] py = Inventec.Common.Repository.Properties.Get<MPS.Processor.Mps000088.PDO.Mps000088ADO>();
                        for (int j = 0; j < 60; j++)
                        {
                            PropertyInfo info = py.FirstOrDefault(o => o.Name == string.Format("DayAndYear{0}", j + 1));
                            if (info != null)
                            {
                                info.SetValue(sdo, j < distinctDatesInPage.Count
                                    ? TimeNumberToDateString(distinctDatesInPage[j])
                                    : "");
                            }
                        }
                        #endregion

                        sdo.Mps000088ByMediEndMateADOs = new List<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate>();
                        this._Mps000088ByMediEndMateADOs = new List<Mps000088ByMediEndMate>();

                        foreach (var group in sereServGroups)
                        {
                            // Bỏ qua nếu không có amount trong trang hiện tại
                            if (group.ToList().Where(o => distinctDatesInPage.Contains(o.INTRUCTION_DATE)).Sum(o => o.AMOUNT) <= 0)
                                continue;

                            MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate sereServPrint =
                                new MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate();

                            List<ExpMestMediAndMateADO> sereServs = group.ToList();

                            // Gán thông tin chung
                            sereServPrint.MEDICINE_TYPE_NAME = group.First().MEDICINE_TYPE_NAME;
                            sereServPrint.MEDICINE_TYPE_ID = group.First().MEDICINE_TYPE_ID;
                            sereServPrint.PRICE = group.First().PRICE;
                            sereServPrint.IS_EXPEND = group.First().IS_EXPEND;
                            sereServPrint.Service_Type_Id = group.First().Service_Type_Id;
                            sereServPrint.SERVICE_ID = group.First().SERVICE_ID;
                            sereServPrint.SERVICE_UNIT_NAME = group.First().SERVICE_UNIT_NAME;
                            sereServPrint.REQ_DEPARTMENT_ID = group.First().REQ_DEPARTMENT_ID;
                            sereServPrint.VAT_RATIO = group.First().VAT_RATIO;
                            sereServPrint.MEDICINE_GROUP_NUM_ORDER = group.First().MEDICINE_GROUP_NUM_ORDER;
                            sereServPrint.MEDICINE_USE_FORM_NUM_ORDER = group.First().MEDICINE_USE_FORM_NUM_ORDER;
                            sereServPrint.NUM_ORDER = group.First().NUM_ORDER;
                            sereServPrint.TUTORIAL = group.First().TUTORIAL;
                            sereServPrint.CONCENTRA = group.First().CONCENTRA;

                            // Tổng amount trong trang hiện tại
                            sereServPrint.AMOUNT = group.ToList()
                                .Where(o => distinctDatesInPage.Contains(o.INTRUCTION_DATE))
                                .Sum(o => o.AMOUNT);

                            Inventec.Common.Logging.LogSystem.Info("Done!!!______");
                            sereServPrint.AMOUNT_STRING = sereServPrint.AMOUNT + "";

                            // ✅ Set Day columns: duyệt từng slot ngày, tìm item trong group khớp ngày đó
                            PropertyInfo[] pp = Inventec.Common.Repository.Properties
                                .Get<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate>();

                            for (int j = 0; j < 60; j++)
                            {
                                // Lấy giá trị ngày tại slot j từ sdo
                                PropertyInfo infoSdo = ps.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                                if (infoSdo == null) continue;

                                string daySlotValue = (string)infoSdo.GetValue(sdo); // vd: "25/03/2026"

                                // Tìm tất cả item trong group có ngày trùng slot này
                                var itemsOnThisDay = sereServs
                                    .Where(o => TimeNumberToDateString(o.INTRUCTION_DATE) == daySlotValue)
                                    .ToList();

                                // Day (amount)
                                PropertyInfo info = pp.FirstOrDefault(o => o.Name == string.Format("Day{0}", j + 1));
                                if (info != null)
                                {
                                    decimal? dayAmount = itemsOnThisDay.Any()
                                        ? (decimal?)itemsOnThisDay.Sum(o => o.AMOUNT)
                                        : null;
                                    info.SetValue(sereServPrint, dayAmount);
                                }

                                // MORNING
                                PropertyInfo info2 = pp.FirstOrDefault(o => o.Name == string.Format("MORNING_Day{0}", j + 1));
                                if (info2 != null)
                                {
                                    string val = itemsOnThisDay.Any()
                                        ? itemsOnThisDay.Sum(s => FormatSessionOfDay(s.MORNING)).ToString()
                                        : "";
                                    info2.SetValue(sereServPrint, val);
                                }

                                // NOON
                                PropertyInfo info3 = pp.FirstOrDefault(o => o.Name == string.Format("NOON_Day{0}", j + 1));
                                if (info3 != null)
                                {
                                    string val = itemsOnThisDay.Any()
                                        ? itemsOnThisDay.Sum(s => FormatSessionOfDay(s.NOON)).ToString()
                                        : "";
                                    info3.SetValue(sereServPrint, val);
                                }

                                // AFTERNOON
                                PropertyInfo info4 = pp.FirstOrDefault(o => o.Name == string.Format("AFTERNOON_Day{0}", j + 1));
                                if (info4 != null)
                                {
                                    string val = itemsOnThisDay.Any()
                                        ? itemsOnThisDay.Sum(s => FormatSessionOfDay(s.AFTERNOON)).ToString()
                                        : "";
                                    info4.SetValue(sereServPrint, val);
                                }

                                // EVENING
                                PropertyInfo info5 = pp.FirstOrDefault(o => o.Name == string.Format("EVENING_Day{0}", j + 1));
                                if (info5 != null)
                                {
                                    string val = itemsOnThisDay.Any()
                                        ? itemsOnThisDay.Sum(s => FormatSessionOfDay(s.EVENING)).ToString()
                                        : "";
                                    info5.SetValue(sereServPrint, val);
                                }
                            }

                            sdo.Mps000088ByMediEndMateADOs.Add(sereServPrint);
                            this._Mps000088ByMediEndMateADOs.Add(sereServPrint);
                        }

                        sdo.Mps000088ByMediEndMateADOs = sdo.Mps000088ByMediEndMateADOs
                            .OrderBy(p => p.MEDICINE_TYPE_NAME)
                            .ToList();

                        this.mps000088ADO.Add(sdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private decimal FormatSessionOfDay(string dt)
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
