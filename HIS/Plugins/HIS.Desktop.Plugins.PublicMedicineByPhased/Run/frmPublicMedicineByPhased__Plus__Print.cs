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

                    #region ----- Groups -----
                    List<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate> _Mps000303ByServiceGroups = new List<MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate>();
                    if (this._Mps000088ByMediEndMateADOs != null && this._Mps000088ByMediEndMateADOs.Count > 0)
                    {
                        var datassss = ado88.Mps000088ByMediEndMateADOs;
                        if (datassss != null && datassss.Count > 0)
                        {
                            if (chkTachHDSD.Checked)
                            {
                                var rsGroup = datassss.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.TUTORIAL, p.PRICE, p.Service_Type_Id, p.IS_EXPEND, p.VAT_RATIO }).ToList();
                                foreach (var itemGroup in rsGroup)
                                {
                                    MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate ado = new MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate();
                                    ado.Service_Type_Id = itemGroup.FirstOrDefault().Service_Type_Id;
                                    ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                                    ado.MEDICINE_TYPE_NAME = itemGroup.FirstOrDefault().MEDICINE_TYPE_NAME;
                                    ado.MEDICINE_TYPE_ID = itemGroup.FirstOrDefault().MEDICINE_TYPE_ID;
                                    ado.IS_EXPEND = itemGroup.FirstOrDefault().IS_EXPEND;
                                    ado.PRICE = itemGroup.FirstOrDefault().PRICE;
                                    ado.SERVICE_UNIT_NAME = itemGroup.FirstOrDefault().SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = itemGroup.FirstOrDefault().CONCENTRA;
                                    ado.REQ_DEPARTMENT_ID = itemGroup.FirstOrDefault().REQ_DEPARTMENT_ID;
                                    ado.MORNING = itemGroup.Sum(s => Convert.ToDecimal(s.MORNING ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.MORNING)).ToString();
                                    ado.NOON = itemGroup.Sum(s => Convert.ToDecimal(s.NOON ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.NOON)).ToString();
                                    ado.AFTERNOON = itemGroup.Sum(s => Convert.ToDecimal(s.AFTERNOON ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.AFTERNOON)).ToString();
                                    ado.EVENING = itemGroup.Sum(s => Convert.ToDecimal(s.EVENING ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.EVENING)).ToString();
                                    //ado.REQUEST_DEPARTMENT_NAME = itemGroup.FirstOrDefault().REQUEST_DEPARTMENT_NAME;
                                    ado.TUTORIAL = itemGroup.FirstOrDefault().TUTORIAL;
                                    ado.VAT_RATIO = itemGroup.FirstOrDefault().VAT_RATIO;
                                    ado.MEDICINE_GROUP_NUM_ORDER = itemGroup.FirstOrDefault().MEDICINE_GROUP_NUM_ORDER;
                                    ado.MEDICINE_USE_FORM_NUM_ORDER = itemGroup.First().MEDICINE_USE_FORM_NUM_ORDER;
                                    ado.NUM_ORDER = itemGroup.FirstOrDefault().NUM_ORDER;

                                }
                            }
                            else
                            {
                                var rsGroup = datassss.GroupBy(p => new { p.MEDICINE_TYPE_ID, p.PRICE, p.Service_Type_Id, p.IS_EXPEND, p.VAT_RATIO }).ToList();

                                foreach (var itemGroup in rsGroup)
                                {
                                    MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate ado = new MPS.Processor.Mps000088.PDO.Mps000088ByMediEndMate();
                                    ado.Service_Type_Id = itemGroup.FirstOrDefault().Service_Type_Id;
                                    ado.AMOUNT = itemGroup.Sum(p => p.AMOUNT);
                                    ado.MEDICINE_TYPE_NAME = itemGroup.FirstOrDefault().MEDICINE_TYPE_NAME;
                                    ado.MEDICINE_TYPE_ID = itemGroup.FirstOrDefault().MEDICINE_TYPE_ID;
                                    ado.IS_EXPEND = itemGroup.FirstOrDefault().IS_EXPEND;
                                    ado.PRICE = itemGroup.FirstOrDefault().PRICE;
                                    ado.SERVICE_UNIT_NAME = itemGroup.FirstOrDefault().SERVICE_UNIT_NAME;
                                    ado.CONCENTRA = itemGroup.FirstOrDefault().CONCENTRA;
                                    ado.REQ_DEPARTMENT_ID = itemGroup.FirstOrDefault().REQ_DEPARTMENT_ID;
                                    ado.MORNING = itemGroup.Sum(s => Convert.ToDecimal(s.MORNING ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.MORNING)).ToString();
                                    ado.NOON = itemGroup.Sum(s => Convert.ToDecimal(s.NOON ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.NOON)).ToString();
                                    ado.AFTERNOON = itemGroup.Sum(s => Convert.ToDecimal(s.AFTERNOON ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.AFTERNOON)).ToString();
                                    ado.EVENING = itemGroup.Sum(s => Convert.ToDecimal(s.EVENING ?? "0")).ToString() == "0" ? "" : itemGroup.Sum(s => Convert.ToDecimal(s.EVENING)).ToString();
                                    //ado.REQUEST_DEPARTMENT_NAME = itemGroup.FirstOrDefault().REQUEST_DEPARTMENT_NAME;
                                    ado.TUTORIAL = itemGroup.FirstOrDefault().TUTORIAL;
                                    ado.VAT_RATIO = itemGroup.FirstOrDefault().VAT_RATIO;
                                    ado.MEDICINE_GROUP_NUM_ORDER = itemGroup.FirstOrDefault().MEDICINE_GROUP_NUM_ORDER;
                                    ado.MEDICINE_USE_FORM_NUM_ORDER = itemGroup.First().MEDICINE_USE_FORM_NUM_ORDER;
                                    ado.NUM_ORDER = itemGroup.FirstOrDefault().NUM_ORDER;
                                    #region old-code
                                    //decimal day1 = 0;
                                    //decimal day2 = 0;
                                    //decimal day3 = 0;
                                    //decimal day4 = 0;
                                    //decimal day5 = 0;
                                    //decimal day6 = 0;
                                    //decimal day7 = 0;
                                    //decimal day8 = 0;
                                    //decimal day9 = 0;
                                    //decimal day10 = 0;
                                    //decimal day11 = 0;
                                    //decimal day12 = 0;
                                    //decimal day13 = 0;
                                    //decimal day14 = 0;
                                    //decimal day15 = 0;
                                    //decimal day16 = 0;
                                    //decimal day17 = 0;
                                    //decimal day18 = 0;
                                    //decimal day19 = 0;
                                    //decimal day20 = 0;
                                    //decimal day21 = 0;
                                    //decimal day22 = 0;
                                    //decimal day23 = 0;
                                    //decimal day24 = 0;
                                    //decimal day25 = 0;
                                    //decimal day26 = 0;
                                    //decimal day27 = 0;
                                    //decimal day28 = 0;
                                    //decimal day29 = 0;
                                    //decimal day30 = 0;
                                    //decimal day31 = 0;
                                    //decimal day32 = 0;
                                    //decimal day33 = 0;
                                    //decimal day34 = 0;
                                    //decimal day35 = 0;
                                    //decimal day36 = 0;
                                    //decimal day37 = 0;
                                    //decimal day38 = 0;
                                    //decimal day39 = 0;
                                    //decimal day40 = 0;
                                    //decimal day41 = 0;
                                    //decimal day42 = 0;
                                    //decimal day43 = 0;
                                    //decimal day44 = 0;
                                    //decimal day45 = 0;
                                    //decimal day46 = 0;
                                    //decimal day47 = 0;
                                    //decimal day48 = 0;
                                    //decimal day49 = 0;
                                    //decimal day50 = 0;
                                    //decimal day51 = 0;
                                    //decimal day52 = 0;
                                    //decimal day53 = 0;
                                    //decimal day54 = 0;
                                    //decimal day55 = 0;
                                    //decimal day56 = 0;
                                    //decimal day57 = 0;
                                    //decimal day58 = 0;
                                    //decimal day59 = 0;
                                    //decimal day60 = 0;

                                    //decimal Morning_day1 = 0;
                                    //decimal Morning_day2 = 0;
                                    //decimal Morning_day3 = 0;
                                    //decimal Morning_day4 = 0;
                                    //decimal Morning_day5 = 0;
                                    //decimal Morning_day6 = 0;
                                    //decimal Morning_day7 = 0;
                                    //decimal Morning_day8 = 0;
                                    //decimal Morning_day9 = 0;
                                    //decimal Morning_day10 = 0;
                                    //decimal Morning_day11 = 0;
                                    //decimal Morning_day12 = 0;
                                    //decimal Morning_day13 = 0;
                                    //decimal Morning_day14 = 0;
                                    //decimal Morning_day15 = 0;
                                    //decimal Morning_day16 = 0;
                                    //decimal Morning_day17 = 0;
                                    //decimal Morning_day18 = 0;
                                    //decimal Morning_day19 = 0;
                                    //decimal Morning_day20 = 0;
                                    //decimal Morning_day21 = 0;
                                    //decimal Morning_day22 = 0;
                                    //decimal Morning_day23 = 0;
                                    //decimal Morning_day24 = 0;
                                    //decimal Morning_day25 = 0;
                                    //decimal Morning_day26 = 0;
                                    //decimal Morning_day27 = 0;
                                    //decimal Morning_day28 = 0;
                                    //decimal Morning_day29 = 0;
                                    //decimal Morning_day30 = 0;
                                    //decimal Morning_day31 = 0;
                                    //decimal Morning_day32 = 0;
                                    //decimal day33 = 0;
                                    //decimal day34 = 0;
                                    //decimal day35 = 0;
                                    //decimal day36 = 0;
                                    //decimal day37 = 0;
                                    //decimal day38 = 0;
                                    //decimal day39 = 0;
                                    //decimal day40 = 0;
                                    //decimal day41 = 0;
                                    //decimal day42 = 0;
                                    //decimal day43 = 0;
                                    //decimal day44 = 0;
                                    //decimal day45 = 0;
                                    //decimal day46 = 0;
                                    //decimal day47 = 0;
                                    //decimal day48 = 0;
                                    //decimal day49 = 0;
                                    //decimal day50 = 0;
                                    //decimal day51 = 0;
                                    //decimal day52 = 0;
                                    //decimal day53 = 0;
                                    //decimal day54 = 0;
                                    //decimal day55 = 0;
                                    //decimal day56 = 0;
                                    //decimal day57 = 0;
                                    //decimal day58 = 0;
                                    //decimal day59 = 0;
                                    //decimal day60 = 0;

                                    //foreach (var item in itemGroup)
                                    //{
                                    //    day1 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day1);
                                    //    day2 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day2);
                                    //    day3 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day3);
                                    //    day4 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day4);
                                    //    day5 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day5);
                                    //    day6 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day6);
                                    //    day7 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day7);
                                    //    day8 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day8);
                                    //    day9 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day9);
                                    //    day10 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day10);
                                    //    day11 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day11);
                                    //    day12 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day12);
                                    //    day13 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day13);
                                    //    day14 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day14);
                                    //    day15 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day15);
                                    //    day16 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day16);
                                    //    day17 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day17);
                                    //    day18 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day18);
                                    //    day19 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day19);
                                    //    day20 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day20);
                                    //    day21 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day21);
                                    //    day22 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day22);
                                    //    day23 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day23);
                                    //    day24 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day24);
                                    //    day25 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day25);
                                    //    day26 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day26);
                                    //    day27 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day27);
                                    //    day28 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day28);
                                    //    day29 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day29);
                                    //    day30 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day30);
                                    //    day31 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day31);
                                    //    day32 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day32);
                                    //    day33 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day33);
                                    //    day34 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day34);
                                    //    day35 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day35);
                                    //    day36 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day36);
                                    //    day37 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day37);
                                    //    day38 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day38);
                                    //    day39 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day39);
                                    //    day40 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day40);
                                    //    day41 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day41);
                                    //    day42 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day42);
                                    //    day43 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day43);
                                    //    day44 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day44);
                                    //    day45 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day45);
                                    //    day46 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day46);
                                    //    day47 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day47);
                                    //    day48 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day48);
                                    //    day49 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day49);
                                    //    day50 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day50);
                                    //    day51 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day51);
                                    //    day52 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day52);
                                    //    day53 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day53);
                                    //    day54 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day54);
                                    //    day55 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day55);
                                    //    day56 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day56);
                                    //    day57 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day57);
                                    //    day58 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day58);
                                    //    day59 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day59);
                                    //    day60 += Inventec.Common.TypeConvert.Parse.ToDecimal(item.Day60);
                                    //}
                                    //if (day1 > 0)
                                    //    ado.Day1 = day1 + "";
                                    //if (day2 > 0)
                                    //    ado.Day2 = day2 + "";
                                    //if (day3 > 0)
                                    //    ado.Day3 = day3 + "";
                                    //if (day4 > 0)
                                    //    ado.Day4 = day4 + "";
                                    //if (day5 > 0)
                                    //    ado.Day5 = day5 + "";
                                    //if (day6 > 0)
                                    //    ado.Day6 = day6 + "";
                                    //if (day7 > 0)
                                    //    ado.Day7 = day7 + "";
                                    //if (day8 > 0)
                                    //    ado.Day8 = day8 + "";
                                    //if (day9 > 0)
                                    //    ado.Day9 = day9 + "";
                                    //if (day10 > 0)
                                    //    ado.Day10 = day10 + "";
                                    //if (day11 > 0)
                                    //    ado.Day11 = day11 + "";
                                    //if (day12 > 0)
                                    //    ado.Day12 = day12 + "";
                                    //if (day13 > 0)
                                    //    ado.Day13 = day13 + "";
                                    //if (day14 > 0)
                                    //    ado.Day14 = day14 + "";
                                    //if (day15 > 0)
                                    //    ado.Day15 = day15 + "";
                                    //if (day16 > 0)
                                    //    ado.Day16 = day16 + "";
                                    //if (day17 > 0)
                                    //    ado.Day17 = day17 + "";
                                    //if (day18 > 0)
                                    //    ado.Day18 = day18 + "";
                                    //if (day19 > 0)
                                    //    ado.Day19 = day19 + "";
                                    //if (day20 > 0)
                                    //    ado.Day20 = day20 + "";
                                    //if (day21 > 0)
                                    //    ado.Day21 = day21 + "";
                                    //if (day22 > 0)
                                    //    ado.Day22 = day22 + "";
                                    //if (day23 > 0)
                                    //    ado.Day23 = day23 + "";
                                    //if (day24 > 0)
                                    //    ado.Day24 = day24 + "";
                                    //if (day25 > 0)
                                    //    ado.Day25 = day25 + "";
                                    //if (day26 > 0)
                                    //    ado.Day26 = day26 + "";
                                    //if (day27 > 0)
                                    //    ado.Day27 = day27 + "";
                                    //if (day28 > 0)
                                    //    ado.Day28 = day28 + "";
                                    //if (day29 > 0)
                                    //    ado.Day29 = day29 + "";
                                    //if (day30 > 0)
                                    //    ado.Day30 = day30 + "";
                                    //if (day31 > 0)
                                    //    ado.Day31 = day31 + "";
                                    //if (day32 > 0)
                                    //    ado.Day32 = day32 + "";
                                    //if (day33 > 0)
                                    //    ado.Day33 = day33 + "";
                                    //if (day34 > 0)
                                    //    ado.Day34 = day34 + "";
                                    //if (day35 > 0)
                                    //    ado.Day35 = day35 + "";
                                    //if (day36 > 0)
                                    //    ado.Day36 = day36 + "";
                                    //if (day37 > 0)
                                    //    ado.Day37 = day37 + "";
                                    //if (day38 > 0)
                                    //    ado.Day38 = day38 + "";
                                    //if (day39 > 0)
                                    //    ado.Day39 = day39 + "";
                                    //if (day40 > 0)
                                    //    ado.Day40 = day40 + "";
                                    //if (day51 > 0)
                                    //    ado.Day51 = day51 + "";
                                    //if (day52 > 0)
                                    //    ado.Day52 = day52 + "";
                                    //if (day53 > 0)
                                    //    ado.Day53 = day53 + "";
                                    //if (day54 > 0)
                                    //    ado.Day54 = day54 + "";
                                    //if (day55 > 0)
                                    //    ado.Day55 = day55 + "";
                                    //if (day56 > 0)
                                    //    ado.Day56 = day56 + "";
                                    //if (day57 > 0)
                                    //    ado.Day57 = day57 + "";
                                    //if (day58 > 0)
                                    //    ado.Day58 = day58 + "";
                                    //if (day59 > 0)
                                    //    ado.Day59 = day59 + "";
                                    //if (day60 > 0)
                                    //    ado.Day60 = day60 + "";
                                    #endregion
                                    //_Mps000303ByServiceGroups.Add(ado);
                                }
                            }
                        }
                    #endregion
                    }

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
                            p.TUTORIAL,
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
