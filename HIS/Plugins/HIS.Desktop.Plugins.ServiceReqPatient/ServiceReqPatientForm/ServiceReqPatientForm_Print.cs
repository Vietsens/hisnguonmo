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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
//using DevExpress.XtraNavBar;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utilities;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Resources;
using HIS.Desktop.Utility;
using DevExpress.XtraLayout;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.ServiceReqPatient.ADO;
using DevExpress.XtraGrid.Views.Grid;
using AutoMapper;
namespace HIS.Desktop.Plugins.ServiceReqPatient
{
    public partial class ServiceReqPatientForm : FormBase
    {
        V_HIS_TREATMENT_BED_ROOM HisTreatmentBedRoom;
        List<ServiceReqPatientADO> ListHP = new List<ServiceReqPatientADO>();
        List<ServiceReqPatientADO> ListLT = new List<ServiceReqPatientADO>();
        internal List<MPS.Processor.Mps000242.PDO.Mps000242ByMedicine> mps000242HP;
        internal List<MPS.Processor.Mps000242.PDO.Mps000242ByMedicine> mps000242LT;
        internal List<MPS.Processor.Mps000242.PDO.Mps000242ADO> mps000242ADO;
        internal enum PrintTypeVotesMedicines
        {
            PHIEU_THEO_DOI_THUOC_VTHP_TIEU_HAO,
        }
        void PrintProcess(PrintTypeVotesMedicines printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeVotesMedicines.PHIEU_THEO_DOI_THUOC_VTHP_TIEU_HAO:
                        richEditorMain.RunPrintTemplate("Mps000242", DelegateRunPrinterVotesMedicines);
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

        bool DelegateRunPrinterVotesMedicines(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printCode)
                {
                    case "Mps000242":
                        LoadPhieuTheoDoiTHuocVTYTHaoPhi(printCode, fileName, ref result);
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

        private void LoadPhieuTheoDoiTHuocVTYTHaoPhi(string printCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                // Lấy thông tin bệnh nhân
                var _TreatmentID = TreatmentID;
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = _TreatmentID;
                var _Treatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                LoadHPandLT();
                STT++;
                if (STT == 2)
                {

                    LoadCreateListDatePrintf();
                }

                AddDataToDatetimeLT();

                AddDataToDatetimeHP();

                if (ListHP.Count == 0) { mps000242HP = new List<MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>(); }
                if (ListLT.Count == 0) { mps000242LT = new List<MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>(); }
                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((_Treatment != null ? _Treatment.TREATMENT_CODE : ""), printCode, this.currentModule != null ? currentModule.RoomId : 0);
                MPS.Processor.Mps000242.PDO.Mps000242PDO mps000242RDO = new MPS.Processor.Mps000242.PDO.Mps000242PDO(mps000242LT, mps000242HP, _Treatment, mps000242ADO, HisTreatmentBedRoom);
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printCode, fileName, mps000242RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "") { EmrInputADO = inputADO };
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printCode, fileName, mps000242RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "") { EmrInputADO = inputADO };
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadCreateListDatePrintf()
        {
            STT++;
            foreach (var item in CreateListDate)
            {
                if (item.IS_EXP)
                {
                    item.Date = item.Date.Split('e')[0] + " " + FactoryIsExp(item.IS_EXP);
                }
                else
                {
                    item.Date = item.Date + " " + FactoryIsExp(item.IS_EXP);
                }

            }
        }

        private void LoadHPandLT()
        {
            ListHP = new List<ServiceReqPatientADO>();
            ListLT = new List<ServiceReqPatientADO>();
            foreach (var item in ServiceReqPatientADOList)
            {
                if (item.Type == "Hao phí")
                {
                    ListHP.Add(item);
                }
                else
                {
                    ListLT.Add(item);
                }


            }
        }
        private void AddDataToDatetimeLT()
        {
            try
            {

                if (ListLT != null && ListLT.Count > 0)
                {
                    var sereServGroups = ListLT;
                    foreach (var item in sereServGroups)
                    {
                        if (item.AmountDateList.Count() > 1)
                        {
                            item.AMOUNT = 0;
                            foreach (var item2 in item.AmountDateList)
                            {
                                item.AMOUNT = item.AMOUNT + item2.Amount;
                            }
                        }
                    }
                    int index = 0;
                    #region ThuatToan
                    while (index < CreateListDate.Count)
                    {
                        MPS.Processor.Mps000242.PDO.Mps000242ADO sdo = new MPS.Processor.Mps000242.PDO.Mps000242ADO();
                        sdo.Day1 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day2 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day3 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day4 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day5 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day6 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day7 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day8 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day9 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day10 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day11 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day12 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day13 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day14 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day15 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day16 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day17 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day18 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day19 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day20 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day21 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day22 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day23 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day24 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day25 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day26 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day27 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day28 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day29 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day30 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        mps000242LT = new List<MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>();
                        foreach (var group in sereServGroups)
                        {
                            if (group.AMOUNT <= 0)
                                continue;
                            MPS.Processor.Mps000242.PDO.Mps000242ByMedicine sereServPrint = new MPS.Processor.Mps000242.PDO.Mps000242ByMedicine();
                            //List<ServiceReqPatientADO> sereServs = new List<ServiceReqPatientADO>();
                            //sereServs.Add(group);
                            //Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE, MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>();
                            //sereServPrint = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE, MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>(group);
                            if (group.MATERIAL_TYPE_ID != null)
                            {
                                sereServPrint.SERVICE_UNIT_NAME = group.UNIT_NAME;
                                sereServPrint.MEDICINE_TYPE_NAME = group.ServiceReqPatientName;
                                sereServPrint.SERVICE_ID = (long)group.MATERIAL_TYPE_ID;
                                sereServPrint.AMOUNT = group.AMOUNT;
                            }
                            else
                            {
                                sereServPrint.AMOUNT = group.AMOUNT;
                                sereServPrint.SERVICE_UNIT_NAME = group.UNIT_NAME;
                                sereServPrint.MEDICINE_TYPE_NAME = group.ServiceReqPatientName;
                                sereServPrint.SERVICE_ID = (long)group.MEDICINE_TYPE_ID;
                            }
                            foreach (var item in group.AmountDateList)
                            {
                                if (STT == 3)
                                {

                                    if (item.IS_export)
                                    {
                                        item.DATE = item.DATE.Split('e')[0] + " " + FactoryIsExp(item.IS_export);
                                    }
                                    else
                                    {
                                        item.DATE = item.DATE + " " + FactoryIsExp(item.IS_export);
                                    }

                                }

                                var amount = item.Amount;
                                if (sereServPrint.Day1 == "" || sereServPrint.Day1 == null)
                                {
                                    sereServPrint.Day1 = sdo.Day1 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day2 == "" || sereServPrint.Day2 == null)
                                {
                                    sereServPrint.Day2 = sdo.Day2 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day3 == "" || sereServPrint.Day3 == null)
                                {
                                    sereServPrint.Day3 = sdo.Day3 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day4 == "" || sereServPrint.Day4 == null)
                                {
                                    sereServPrint.Day4 = sdo.Day4 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day5 == "" || sereServPrint.Day5 == null)
                                {
                                    sereServPrint.Day5 = sdo.Day5 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day6 == "" || sereServPrint.Day6 == null)
                                {
                                    sereServPrint.Day6 = sdo.Day6 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day7 == "" || sereServPrint.Day7 == null)
                                {
                                    sereServPrint.Day7 = sdo.Day7 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day8 == "" || sereServPrint.Day8 == null)
                                {
                                    sereServPrint.Day8 = sdo.Day8 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day9 == "" || sereServPrint.Day9 == null)
                                {
                                    sereServPrint.Day9 = sdo.Day9 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day10 == "" || sereServPrint.Day10 == null)
                                {
                                    sereServPrint.Day10 = sdo.Day10 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day11 == "" || sereServPrint.Day11 == null)
                                {
                                    sereServPrint.Day11 = sdo.Day11 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day12 == "" || sereServPrint.Day12 == null)
                                {
                                    sereServPrint.Day12 = sdo.Day12 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day13 == "" || sereServPrint.Day13 == null)
                                {
                                    sereServPrint.Day13 = sdo.Day13 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day14 == "" || sereServPrint.Day14 == null)
                                {
                                    sereServPrint.Day14 = sdo.Day14 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day15 == "" || sereServPrint.Day15 == null) { sereServPrint.Day15 = sdo.Day15 == item.DATE ? Convert.ToString(amount) : ""; }
                                if (sereServPrint.Day16 == "" || sereServPrint.Day16 == null)
                                {
                                    sereServPrint.Day16 = sdo.Day16 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day17 == "" || sereServPrint.Day17 == null)
                                {
                                    sereServPrint.Day17 = sdo.Day17 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day18 == "" || sereServPrint.Day18 == null)
                                {
                                    sereServPrint.Day18 = sdo.Day18 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day19 == "" || sereServPrint.Day19 == null)
                                {
                                    sereServPrint.Day19 = sdo.Day19 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day20 == "" || sereServPrint.Day20 == null)
                                {
                                    sereServPrint.Day20 = sdo.Day20 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day21 == "" || sereServPrint.Day21 == null)
                                {
                                    sereServPrint.Day21 = sdo.Day21 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day22 == "" || sereServPrint.Day22 == null)
                                {
                                    sereServPrint.Day22 = sdo.Day22 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day23 == "" || sereServPrint.Day23 == null)
                                {
                                    sereServPrint.Day23 = sdo.Day23 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day24 == "" || sereServPrint.Day24 == null)
                                {
                                    sereServPrint.Day24 = sdo.Day24 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day25 == "" || sereServPrint.Day25 == null)
                                {
                                    sereServPrint.Day25 = sdo.Day25 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day26 == "" || sereServPrint.Day26 == null)
                                {
                                    sereServPrint.Day26 = sdo.Day26 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day27 == "" || sereServPrint.Day27 == null)
                                {
                                    sereServPrint.Day27 = sdo.Day27 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day28 == "" || sereServPrint.Day28 == null)
                                {
                                    sereServPrint.Day28 = sdo.Day28 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day29 == "" || sereServPrint.Day29 == null)
                                {
                                    sereServPrint.Day29 = sdo.Day29 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day30 == "" || sereServPrint.Day30 == null)
                                {
                                    sereServPrint.Day30 = sdo.Day30 == item.DATE ? Convert.ToString(amount) : "";
                                }
                            }
                            mps000242LT.Add(sereServPrint);
                        }
                    #endregion
                        //sdo.Mps000242ByMedicineADOs = mps000242LT;
                        mps000242ADO = new List<MPS.Processor.Mps000242.PDO.Mps000242ADO>();
                        mps000242ADO.Add(sdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void AddDataToDatetimeHP()
        {
            try
            {

                if (ListHP != null && ListHP.Count > 0)
                {
                    var sereServGroups = ListHP;
                    foreach (var item in sereServGroups)
                    {
                        if (item.AmountDateList.Count() > 1)
                        {
                            item.AMOUNT = 0;
                            foreach (var item2 in item.AmountDateList)
                            {
                                item.AMOUNT = item.AMOUNT + item2.Amount;
                            }
                        }
                    }
                    int index = 0;
                    #region ThuatToan
                    while (index < CreateListDate.Count)
                    {
                        MPS.Processor.Mps000242.PDO.Mps000242ADO sdo = new MPS.Processor.Mps000242.PDO.Mps000242ADO();
                        sdo.Day1 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day2 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day3 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day4 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day5 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day6 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day7 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day8 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day9 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day10 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day11 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day12 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day13 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day14 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day15 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day16 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day17 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day18 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day19 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day20 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day21 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day22 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day23 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day24 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day25 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day26 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day27 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day28 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day29 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        sdo.Day30 = index < CreateListDate.Count() ? CreateListDate[index++].Date : "";
                        mps000242HP = new List<MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>();
                        foreach (var group in sereServGroups)
                        {
                            if (group.AMOUNT <= 0)
                                continue;
                            MPS.Processor.Mps000242.PDO.Mps000242ByMedicine sereServPrint = new MPS.Processor.Mps000242.PDO.Mps000242ByMedicine();
                            //List<ServiceReqPatientADO> sereServs = new List<ServiceReqPatientADO>();
                            //sereServs.Add(group);
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE, MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>();
                            sereServPrint = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE, MPS.Processor.Mps000242.PDO.Mps000242ByMedicine>(group);
                            if (group.MATERIAL_TYPE_ID != null)
                            {
                                sereServPrint.AMOUNT = group.AMOUNT;
                                sereServPrint.SERVICE_UNIT_NAME = group.UNIT_NAME;
                                sereServPrint.MEDICINE_TYPE_NAME = group.ServiceReqPatientName;
                                sereServPrint.SERVICE_ID = (long)group.MATERIAL_TYPE_ID;
                            }
                            else
                            {
                                sereServPrint.AMOUNT = group.AMOUNT;
                                sereServPrint.SERVICE_UNIT_NAME = group.UNIT_NAME;
                                sereServPrint.MEDICINE_TYPE_NAME = group.ServiceReqPatientName;
                                sereServPrint.SERVICE_ID = (long)group.MEDICINE_TYPE_ID;
                            }
                            foreach (var item in group.AmountDateList)
                            {
                                if (STT == 3)
                                {
                                    if (item.IS_export)
                                    {
                                        item.DATE = item.DATE.Split('e')[0] + " " + FactoryIsExp(item.IS_export);
                                    }
                                    else
                                    {
                                        item.DATE = item.DATE + " " + FactoryIsExp(item.IS_export);
                                    }

                                }

                                var amount = item.Amount;
                                if (sereServPrint.Day1 == "" || sereServPrint.Day1 == null)
                                {
                                    sereServPrint.Day1 = sdo.Day1 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day2 == "" || sereServPrint.Day2 == null)
                                {
                                    sereServPrint.Day2 = sdo.Day2 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day3 == "" || sereServPrint.Day3 == null)
                                {
                                    sereServPrint.Day3 = sdo.Day3 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day4 == "" || sereServPrint.Day4 == null)
                                {
                                    sereServPrint.Day4 = sdo.Day4 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day5 == "" || sereServPrint.Day5 == null)
                                {
                                    sereServPrint.Day5 = sdo.Day5 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day6 == "" || sereServPrint.Day6 == null)
                                {
                                    sereServPrint.Day6 = sdo.Day6 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day7 == "" || sereServPrint.Day7 == null)
                                {
                                    sereServPrint.Day7 = sdo.Day7 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day8 == "" || sereServPrint.Day8 == null)
                                {
                                    sereServPrint.Day8 = sdo.Day8 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day9 == "" || sereServPrint.Day9 == null)
                                {
                                    sereServPrint.Day9 = sdo.Day9 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day10 == "" || sereServPrint.Day10 == null)
                                {
                                    sereServPrint.Day10 = sdo.Day10 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day11 == "" || sereServPrint.Day11 == null)
                                {
                                    sereServPrint.Day11 = sdo.Day11 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day12 == "" || sereServPrint.Day12 == null)
                                {
                                    sereServPrint.Day12 = sdo.Day12 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day13 == "" || sereServPrint.Day13 == null)
                                {
                                    sereServPrint.Day13 = sdo.Day13 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day14 == "" || sereServPrint.Day14 == null)
                                {
                                    sereServPrint.Day14 = sdo.Day14 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day15 == "" || sereServPrint.Day15 == null) { sereServPrint.Day15 = sdo.Day15 == item.DATE ? Convert.ToString(amount) : ""; }
                                if (sereServPrint.Day16 == "" || sereServPrint.Day16 == null)
                                {
                                    sereServPrint.Day16 = sdo.Day16 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day17 == "" || sereServPrint.Day17 == null)
                                {
                                    sereServPrint.Day17 = sdo.Day17 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day18 == "" || sereServPrint.Day18 == null)
                                {
                                    sereServPrint.Day18 = sdo.Day18 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day19 == "" || sereServPrint.Day19 == null)
                                {
                                    sereServPrint.Day19 = sdo.Day19 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day20 == "" || sereServPrint.Day20 == null)
                                {
                                    sereServPrint.Day20 = sdo.Day20 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day21 == "" || sereServPrint.Day21 == null)
                                {
                                    sereServPrint.Day21 = sdo.Day21 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day22 == "" || sereServPrint.Day22 == null)
                                {
                                    sereServPrint.Day22 = sdo.Day22 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day23 == "" || sereServPrint.Day23 == null)
                                {
                                    sereServPrint.Day23 = sdo.Day23 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day24 == "" || sereServPrint.Day24 == null)
                                {
                                    sereServPrint.Day24 = sdo.Day24 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day25 == "" || sereServPrint.Day25 == null)
                                {
                                    sereServPrint.Day25 = sdo.Day25 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day26 == "" || sereServPrint.Day26 == null)
                                {
                                    sereServPrint.Day26 = sdo.Day26 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day27 == "" || sereServPrint.Day27 == null)
                                {
                                    sereServPrint.Day27 = sdo.Day27 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day28 == "" || sereServPrint.Day28 == null)
                                {
                                    sereServPrint.Day28 = sdo.Day28 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day29 == "" || sereServPrint.Day29 == null)
                                {
                                    sereServPrint.Day29 = sdo.Day29 == item.DATE ? Convert.ToString(amount) : "";
                                }
                                if (sereServPrint.Day30 == "" || sereServPrint.Day30 == null)
                                {
                                    sereServPrint.Day30 = sdo.Day30 == item.DATE ? Convert.ToString(amount) : "";
                                }
                            }
                            mps000242HP.Add(sereServPrint);
                        }

                    #endregion
                        //sdo.Mps000242ByMedicineADOs = mps000242HP;
                        mps000242ADO = new List<MPS.Processor.Mps000242.PDO.Mps000242ADO>();
                        mps000242ADO.Add(sdo);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

    }
}
