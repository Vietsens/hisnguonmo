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
using FlexCel.Report;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000048.PDO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000048
{
    class Mps000048Processor : AbstractProcessor
    {
        Mps000048PDO rdo;
        List<Mps000048ADO> listAdoPrint = new List<Mps000048ADO>();

        public Mps000048Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000048PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                ProcessSingleKey();
                ProcessListPrint();

                //ghi đè PrintLogData và UniqueCodeData
                ProcessPrintLogData();
                //lấy số lần in
                SetNumOrderKey(GetNumOrderPrint(ProcessUniqueCodeData()));

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));
                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "ListMediMate1", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate2", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMate3", listAdoPrint);
                objectTag.AddObjectData(store, "ListMediMatePackage1", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMatePackage2", rdo.listAdo);
                objectTag.AddObjectData(store, "ListMediMatePackage3", rdo.listAdo);
                objectTag.SetUserFunction(store, "FuncMergeData11", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData12", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData13", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData14", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData21", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData22", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData23", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData24", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData31", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData32", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData33", new CalculateMergerData());
                objectTag.SetUserFunction(store, "FuncMergeData34", new CalculateMergerData());
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listAdoPrint), listAdoPrint));
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessListPrint()
        {
            try
            {
                if (rdo.listAdo != null && rdo.listAdo.Count > 0)
                {
                    if (rdo.keyMert == 0)
                    {
                        var dataGroup = rdo.listAdo.GroupBy(p => new { p.TYPE_ID, p.MEDI_MATE_TYPE_ID, p.PRICE }).ToList();
                        foreach (var itemGroup in dataGroup)
                        {
                            Mps000048ADO ado = new Mps000048ADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<Mps000048ADO>(ado, itemGroup.FirstOrDefault());

                            ado.PACKAGE_NUMBER_TOTAL = string.Join(", ", itemGroup.Select(s => s.PACKAGE_NUMBER).Distinct().ToList());
                            ado.TOTAL_AMOUNT = itemGroup.FirstOrDefault().TOTAL_AMOUNT;
                            ado.TOTAL_AMOUNT_IN_EXECUTE = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXECUTE);
                            ado.TOTAL_AMOUNT_IN_EXPORT = itemGroup.Sum(p => p.TOTAL_AMOUNT_IN_EXPORT);
                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGroup.FirstOrDefault().TOTAL_AMOUNT_IN_REQUEST;

                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));
                            ado.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_EXECUTE)));
                            ado.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_EXPORT)));
                            listAdoPrint.Add(ado);
                        }
                    }
                    else
                    {
                        listAdoPrint = rdo.listAdo.ToList();
                    }
                    listAdoPrint = listAdoPrint.OrderBy(p => p.TYPE_ID).ThenBy(p => p.MEDI_MATE_NUM_ORDER).ThenBy(p => p.MEDI_MATE_TYPE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSingleKey()
        {
            try
            {
                decimal totalPrice = 0;
                if (this.rdo._ChmsExpMest != null)
                {
                    SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.CREATE_TIME_STR, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.CREATE_DATE_STR, Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.CREATE_DATE_SEPARATE, Inventec.Common.DateTime.Convert.TimeNumberToDateStringSeparateString(this.rdo._ChmsExpMest.CREATE_TIME ?? 0)));
                    AddObjectKeyIntoListkey(this.rdo._ChmsExpMest, false);
                    if (rdo._MediStocks != null && rdo._MediStocks.Count > 0)
                    {
                        var dataImpMedi = rdo._MediStocks.FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.IMP_MEDI_STOCK_ID);
                        if (dataImpMedi != null)
                        {
                            SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.IMP_MEDI_STOCK_CODE, dataImpMedi.MEDI_STOCK_CODE));
                            SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.IMP_MEDI_STOCK_NAME, dataImpMedi.MEDI_STOCK_NAME));
                        }
                        var data = rdo._MediStocks.FirstOrDefault(p => p.ID == this.rdo._ChmsExpMest.MEDI_STOCK_ID);
                        if (data != null)
                        {
                            SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.EXP_IS_CABINET, data.IS_CABINET));
                        }
                    }
                }
                if (this.rdo._ExpMestMetyReqs != null && this.rdo._ExpMestMetyReqs.Count > 0)
                {
                    if (this.rdo._Medicines != null && this.rdo._Medicines.Count > 0)
                    {
                        var dataMediByMetyReqs = this.rdo._Medicines.Where(p => this.rdo._ExpMestMetyReqs.Select(o => o.MEDICINE_TYPE_ID).ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                        if (dataMediByMetyReqs != null && dataMediByMetyReqs.Count > 0)
                        {
                            var Group = dataMediByMetyReqs.GroupBy(g => new
                            {
                                g.MEDICINE_TYPE_ID,
                                g.PACKAGE_NUMBER,
                                g.SUPPLIER_ID,
                                g.IMP_PRICE,
                                g.IMP_VAT_RATIO
                            }).ToList();
                            foreach (var mediGr in Group)
                            {
                                Mps000048ADO adoMediGr = new Mps000048ADO();
                                adoMediGr.TYPE_ID = 1;
                                adoMediGr.TOTAL_AMOUNT_IN_REQUEST = this.rdo._ExpMestMetyReqs.Where(p => p.MEDICINE_TYPE_ID
                                     == mediGr.First().MEDICINE_TYPE_ID).ToList().Sum(o => o.AMOUNT);
                                adoMediGr.TOTAL_AMOUNT = adoMediGr.TOTAL_AMOUNT_IN_REQUEST;
                                adoMediGr.MEDI_MATE_TYPE_CODE = mediGr.First().MEDICINE_TYPE_CODE;
                                adoMediGr.MEDI_MATE_TYPE_ID = mediGr.First().MEDICINE_TYPE_ID;
                                adoMediGr.MEDI_MATE_TYPE_NAME = mediGr.First().MEDICINE_TYPE_NAME;
                                adoMediGr.REGISTER_NUMBER = mediGr.First().REGISTER_NUMBER;
                                adoMediGr.SERVICE_UNIT_CODE = mediGr.First().SERVICE_UNIT_CODE;
                                adoMediGr.SERVICE_UNIT_NAME = mediGr.First().SERVICE_UNIT_NAME;
                                adoMediGr.MANUFACTURER_CODE = mediGr.First().MANUFACTURER_CODE;
                                adoMediGr.MANUFACTURER_NAME = mediGr.First().MANUFACTURER_NAME;

                                adoMediGr.TOTAL_AMOUNT_IN_EXECUTE = mediGr.Sum(p => p.AMOUNT);
                                adoMediGr.PACKAGE_NUMBER = mediGr.First().PACKAGE_NUMBER;
                                adoMediGr.SUPPLIER_CODE = mediGr.First().SUPPLIER_CODE;
                                adoMediGr.CONCENTRA = mediGr.First().CONCENTRA;
                                adoMediGr.SUPPLIER_NAME = mediGr.First().SUPPLIER_NAME;
                                adoMediGr.EXPIRED_DATE_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(mediGr.First().EXPIRED_DATE ?? 0);
                                adoMediGr.PRICE = mediGr.First().PRICE;
                                adoMediGr.IMP_PRICE = mediGr.First().IMP_PRICE;
                                adoMediGr.IMP_VAT_RATIO = mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.IMP_PRICE_RATIO = mediGr.First().IMP_PRICE + mediGr.First().IMP_PRICE * mediGr.First().IMP_VAT_RATIO;
                                adoMediGr.DESCRIPTION = mediGr.First().DESCRIPTION;
                                adoMediGr.MEDI_MATE_NUM_ORDER = mediGr.First().MEDICINE_NUM_ORDER ?? 0;
                                adoMediGr.NUM_ORDER = mediGr.First().NUM_ORDER;
                                adoMediGr.ACTIVE_INGR_BHYT_CODE = mediGr.First().ACTIVE_INGR_BHYT_CODE;
                                adoMediGr.ACTIVE_INGR_BHYT_NAME = mediGr.First().ACTIVE_INGR_BHYT_NAME;
                                adoMediGr.NATIONAL_NAME = mediGr.First().NATIONAL_NAME;

                                if (rdo._ChmsExpMest.EXP_MEST_STT_ID == rdo.expMesttSttId__Export)
                                {
                                    adoMediGr.TOTAL_AMOUNT_IN_EXPORT = adoMediGr.TOTAL_AMOUNT_IN_EXECUTE;
                                }

                                adoMediGr.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_REQUEST)));
                                adoMediGr.AMOUNT_EXECUTE_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXECUTE)));
                                adoMediGr.AMOUNT_EXPORT_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(adoMediGr.TOTAL_AMOUNT_IN_EXPORT)));
                                rdo.listAdo.Add(adoMediGr);
                                var listByGroup = mediGr.ToList<V_HIS_EXP_MEST_MEDICINE>();
                                foreach (var item in listByGroup)
                                {
                                    totalPrice += (item.AMOUNT * (item.IMP_PRICE) * (item.IMP_VAT_RATIO + 1)) - (item.DISCOUNT ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        var Groups = this.rdo._ExpMestMetyReqs.GroupBy(g => new
                        {
                            g.MEDICINE_TYPE_ID
                        }).Select(p => p.ToList()).ToList();
                        foreach (var itemGr in Groups)
                        {
                            Mps000048ADO ado = new Mps000048ADO();
                            ado.TYPE_ID = 1;
                            ado.DESCRIPTION = itemGr[0].DESCRIPTION;
                            var data = rdo._MedicineTypes.FirstOrDefault(p => p.ID == itemGr[0].MEDICINE_TYPE_ID);
                            if (data != null)
                            {
                                ado.MEDI_MATE_TYPE_CODE = data.MEDICINE_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_ID = data.ID;
                                ado.MEDI_MATE_TYPE_NAME = data.MEDICINE_TYPE_NAME;
                                ado.REGISTER_NUMBER = data.REGISTER_NUMBER;
                                ado.CONCENTRA = data.CONCENTRA;
                                ado.SERVICE_UNIT_CODE = data.SERVICE_UNIT_CODE;
                                ado.SERVICE_UNIT_NAME = data.SERVICE_UNIT_NAME;
                                ado.MANUFACTURER_CODE = data.MEDICINE_GROUP_CODE;
                                ado.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                                ado.ACTIVE_INGR_BHYT_CODE = data.ACTIVE_INGR_BHYT_CODE;
                                ado.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                                ado.NATIONAL_NAME = data.NATIONAL_NAME;

                            }

                            ado.TOTAL_AMOUNT_IN_REQUEST = itemGr.Sum(o => o.AMOUNT);
                            ado.TOTAL_AMOUNT = ado.TOTAL_AMOUNT_IN_REQUEST;
                            ado.AMOUNT_REQUEST_STRING = Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(ado.TOTAL_AMOUNT_IN_REQUEST)));

                            rdo.listAdo.Add(ado);
                        }
                    }
                }

                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.IS_PLAY_CHECK, rdo._keyPhieuTra));
                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.REQ_DEPARTMENT_NAME, rdo.Req_Department_Name));
                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.REQ_ROOM_NAME, rdo.Req_Room_Name));
                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.EXP_DEPARTMENT_NAME, rdo.Exp_Department_Name));
                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.KEY_NAMES, rdo.KeyNames));
                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.SUM_TOTAL_PRICE, totalPrice));

                SetSingleKey(new KeyValue(Mps000048ExtendSingleKey.SUM_TOTAL_PRICE_TEXT, Inventec.Common.String.Convert.CurrencyToVneseString(string.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(totalPrice)))));

                rdo.listAdo = rdo.listAdo.OrderBy(o => o.TYPE_ID).ThenByDescending(t => t.NUM_ORDER).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override string ProcessPrintLogData()
        {
            string log = "";
            try
            {
                if (rdo != null && rdo._ChmsExpMest != null)
                {
                    log = LogDataImpMest("", rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.REQ_DEPARTMENT_NAME + "_" + rdo._ChmsExpMest.MEDI_STOCK_NAME);
                }
            }
            catch (Exception ex)
            {
                log = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return log;
        }

        public override string ProcessUniqueCodeData()
        {
            string result = "";
            try
            {
                if (rdo != null && rdo._ChmsExpMest != null)
                    result = String.Format("{0}_{1}_{2}_{3}", printTypeCode, rdo._ChmsExpMest.EXP_MEST_CODE, rdo._ChmsExpMest.MEDI_STOCK_CODE, listAdoPrint.First().MEDI_MATE_TYPE_CODE);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }

    class CalculateMergerData : TFlexCelUserFunction
    {
        long typeId = 0;
        long mediMateTypeId = 0;

        public override object Evaluate(object[] parameters)
        {
            if (parameters == null || parameters.Length <= 0)
                throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
            bool result = false;
            try
            {
                long servicetypeId = Convert.ToInt64(parameters[0]);
                long mediMateId = Convert.ToInt64(parameters[1]);

                if (servicetypeId > 0 && mediMateId > 0)
                {
                    if (this.typeId == servicetypeId && this.mediMateTypeId == mediMateId)
                    {
                        return true;
                    }
                    else
                    {
                        this.typeId = servicetypeId;
                        this.mediMateTypeId = mediMateId;
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
